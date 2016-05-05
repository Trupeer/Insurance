using HONGLI.Entity;
using HONGLI.Service;
using HONGLI.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HONGLI.Repository.OrderRepository;

namespace HONGLI.Web.Controllers
{
#if (!DEBUG)
    [AuthorizationFilter]
#endif
    public class OrderV3Controller : Controller
    {

        public OrderService _orderService = new OrderService();

        public PayService _payService = new PayService();

        public ProductV2Service _productV2Service = new ProductV2Service();


        // GET: Order/index
        public ActionResult Index(int? productId, string mobile, string channel, int? intentionCompany, string OrderCode, int? OrderBaseId, int? OrderItemId, int? OrderPolicyId, int? OrderDeliverId)
        {
            ViewBag.ProductId = productId;
            ViewBag.Mobile = mobile;
            ViewBag.Channel = channel;
            ViewBag.intentionCompany = intentionCompany;
            ViewBag.OrderCode = OrderCode;
            ViewBag.OrderBaseId = OrderBaseId;
            ViewBag.OrderItemId = OrderItemId;
            ViewBag.OrderPolicyId = OrderPolicyId;
            ViewBag.OrderDeliverId = OrderDeliverId;
      ViewBag.UserId = UserViewModel.CurrentUser.ID;

            return View();
        }

        public ActionResult Detail(string code)
        {

            var order = _orderService.GetOrderByCode(code);
            if (order != null)
            {
                var model = new OrderDetailModels()
                {
                    OrderCode = order.OrderCode,
                    DeliverType = order.Order_Deliver.FirstOrDefault().DeliverType,
                    DeliverName = order.Order_Deliver.FirstOrDefault().DeliverName,
                    DeliverMobile = order.Order_Deliver.FirstOrDefault().DeliverMobile,
                    DeliverAddress = order.Order_Deliver.FirstOrDefault().DeliverAddress,
                    DeliverPrice = order.Order_Deliver.FirstOrDefault().DeliverPrice,
                    DeliverTime = order.Order_Deliver.FirstOrDefault().DeliverTime,
                    ProductId = order.Order_Item.FirstOrDefault().ProductId,
                    ProductName = order.Order_Item.FirstOrDefault().ProductName,
                    ProductTitle = order.Order_Item.FirstOrDefault().ProductTitle,
                    ProductOriginalPrice = order.Order_Item.FirstOrDefault().ProductOriginalPrice,
                    ProductDealPrice = order.Order_Item.FirstOrDefault().ProductDealPrice,
                    InvoiceTitle = order.InvoiceTitle,
                    InvoiceType = order.InvoiceType,
                    PolicyHolderName = order.Order_PolicyHolder.FirstOrDefault().Name,
                    PolicyHolderIdcard = order.Order_PolicyHolder.FirstOrDefault().IdCard,
                    PayType = order.Order_Pay.FirstOrDefault().PayType,
                    InsuranceLogo = order.Order_Item.FirstOrDefault().InsuranceLogo,
                    AmountPayable = order.AmountPayable,
                    CreateDate = order.CreateDate,
                    OrderStatus = order.Status,
                    PrepaidAmount = order.PrepaidAmount,
                    LicenseNo = order.Order_Item.FirstOrDefault().LicenseNo,
                    ForceExpireDate = order.Order_Item.FirstOrDefault().ForceExpireDate,
                    BusinessExpireDate = order.Order_Item.FirstOrDefault().BusinessExpireDate

                };
                return View(model);
            }
            else
            {
                return View();
            }


        }

        /// <summary>
        /// 下订单
        /// </summary>
        /// <returns></returns>
        public ActionResult Do(int? productId, string mobile, string channel, int? intentionCompany, string OrderCode, int? OrderBaseId, int? OrderItemId, int? OrderPolicyId, int? OrderDeliverId)
        {
            ViewBag.ProductId = productId;
            ViewBag.Mobile = mobile;
            ViewBag.Channel = channel;
            ViewBag.intentionCompany = intentionCompany;
            ViewBag.OrderCode = OrderCode;
            ViewBag.OrderBaseId = OrderBaseId;
            ViewBag.OrderItemId = OrderItemId;
            ViewBag.OrderPolicyId = OrderPolicyId;
            ViewBag.OrderDeliverId = OrderDeliverId;
      ViewBag.UserId = UserViewModel.CurrentUser.ID;

            return View();
        }
        public string GetOrderByCode(string OrderCode)
        {
            try
            {
                AllOrderList orderlist = new OrderService().GetOrderListByOrderCode(OrderCode);
                return JsonConvert.SerializeObject(orderlist);
            }
            catch
            {
                return null;
            }

        }
        /// <summary>
        /// 订单提交
        /// </summary>
        /// <param name="product"></param>
        /// <param name="invoice"></param>
        /// <param name="deliver"></param>
        /// <param name="policyHolder"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Submit(ProductEntity product, InvoiceEntity invoice, PayAndDeliverEntity payAndDeliver, DeliverEntity deliver, PolicyHolderEntity policyHolder, string OrderCode, int? OrderBaseId, int? OrderItemId, int? OrderPolicyId, int OrderDeliverId)
        {

            HONGLI.Entity.ResultData resultData = AddOrder(product, invoice, payAndDeliver, deliver, policyHolder, OrderCode, OrderBaseId, OrderItemId, OrderPolicyId, OrderDeliverId);
            return this.Json(resultData);
        }

        public ActionResult PayConfirm(string ordercode)
        {
            var order = _orderService.GetOrderByCode(ordercode);
            var product = order.Order_Item.FirstOrDefault();

            if (product != null)
            {
                var model = new PayConfirmModel()
                {
                    OrderCode = order.OrderCode,
                    LicenseNo = product.LicenseNo,
                    DealPrice = product.ProductDealPrice
                };
                return View(model);
            }
            else
            {
                throw new Exception("产品信息异常");
            }
        }

        public ActionResult Quit(string ordercode)
        {
            var result = _orderService.UpdateOrderStatus(ordercode, (int)EnumOrderStatus.Quit);
            return RedirectToAction("List");
        }

        public ActionResult Pay(string ordercode)
        {
            var order = _orderService.GetOrderByCode(ordercode);

            var response = _payService.CreateRequest(order);
            Response.Write(response);
            return null;
        }

        public ActionResult List()
        {
            var userid = UserViewModel.CurrentUser.ID.ToString();
            List<OrderListModel> model = null;
            var orderlist = _orderService.GetOrderListByUserId(userid);
            if (orderlist != null)
            {
                model = orderlist.Select(c => new OrderListModel()
                {
                    OrderCode = c.OrderCode,
                    OrderStatus = c.Status,
                    AmountPayable = c.AmountPayable,
                    PaidAmount = c.PaidAmount,
                    InsuranceLogo = c.Order_Item.FirstOrDefault().InsuranceLogo,
                    LicenseNo = c.Order_Item.FirstOrDefault().LicenseNo,
                    ForceExpireDate = c.Order_Item.FirstOrDefault().ForceExpireDate,
                    BusinessExpireDate = c.Order_Item.FirstOrDefault().BusinessExpireDate
                }).ToList();
            }

            return View(model);
        }


        /// <summary>
        /// 下订单
        /// </summary>
        /// <param name="product"></param>
        /// <param name="invoice"></param>
        /// <param name="deliver"></param>
        /// <param name="policyHolder"></param>
        /// <returns></returns>
        public HONGLI.Entity.ResultData AddOrder(ProductEntity product, InvoiceEntity invoice, PayAndDeliverEntity payAndDeliver, DeliverEntity deliver, PolicyHolderEntity policyHolder, string OrderCode, int? OrderBaseId, int? OrderItemId, int? OrderPolicyId, int OrderDeliverId)
        {
            HONGLI.Entity.ResultData result = new Entity.ResultData();

            try
            {


#if (!DEBUG)
    var userid = UserViewModel.CurrentUser.ID.ToString();
#else
                var userid = "3";
#endif
                decimal productDealPrice = -1;
                try
                {
                    productDealPrice = _productV2Service.GetProductV2(Convert.ToInt32(product.ProductId)).TotalAfterCoupon.Value;
                }
                catch (Exception ex)
                {
                    I.Utility.Helper.LogHelper.AppError("获取产品金额失败" + product.ProductId + ",error:" + ex.Message);
                    result.Status = 0;
                    result.Error = "获取产品金额失败";
                    return result;
                }

                if (productDealPrice <= 0)
                {
                    result.Status = 0;
                    result.Error = "产品金额小于0,异常";
                    return result;
                }


                int _deliverArriveAddDay = 5;
                try
                {
                    var deliverArriveAddDay = I.Utility.Util.GetConfigByKey("DeliverArriveAddDay");
                    if (deliverArriveAddDay != null)
                    {
                        _deliverArriveAddDay = Convert.ToInt32(deliverArriveAddDay);
                    }

                }
                catch (Exception ex)
                { }
                Order_Base order_base = new Order_Base();
                order_base.Id = OrderBaseId == null ? 0 : Convert.ToInt32(OrderBaseId);
                order_base.OrderCode = OrderCode;
                order_base.InvoiceTitle = invoice.InvoiceTitle;
                order_base.InvoiceType = invoice.InvoiceType;
                order_base.AmountPayable = productDealPrice + payAndDeliver.DeliverPrice;
                order_base.PaidAmount = productDealPrice + payAndDeliver.DeliverPrice;
                new OrderService().EditOrderInvoice(order_base);
                Order_Deliver order_deliver = new Order_Deliver();
                if (payAndDeliver.DeliverType == 1)
                {
                    order_deliver.DeliverDistrictCode = deliver.deliverDistrictCode;
                    order_deliver.OrderCode = OrderCode;
                    order_deliver.DeliverMobile = deliver.deliverMobile;
                    order_deliver.DeliverAddress = deliver.deliverAddress;
                    order_deliver.DeliverName = deliver.deliverName;
                    order_deliver.DeliverPrice = payAndDeliver.DeliverPrice;
                    order_deliver.DeliverType = payAndDeliver.DeliverType;
                    order_deliver.DeliverTime = DateTime.Now.AddDays(_deliverArriveAddDay);
                    order_deliver.CreateDate = DateTime.Now;
                }
                else
                {
                    order_deliver.DeliverType = 2;
                    order_deliver.OrderCode = OrderCode;
                    order_deliver.DeliverAddress = payAndDeliver.SelfTakeAddress;
                    order_deliver.DeliverTime = payAndDeliver.SelfTakeDate;
                    order_deliver.DeliverPrice = payAndDeliver.DeliverPrice;
                    order_deliver.CreateDate = DateTime.Now;
                }
                if (OrderDeliverId != 0)
                {
                    order_deliver.Id = OrderDeliverId;
                    new AdminService().EditOrderDeliver(order_deliver);
                }
                else
                {
                    new AdminService().SaveOrderDeliver(order_deliver);
                }

                Order_PolicyHolder order_policyHolder = new Order_PolicyHolder()
                {
                    Name = policyHolder.Name,
                    IdCard = policyHolder.Idcard,
                    IdCardType = policyHolder.IdcardType,
                    IdCardFacePicUrl = policyHolder.IdcardFacePicUrl,
                    IdCardBackPicUrl = policyHolder.IdcardBackPicUrl,
                    CreateDate = DateTime.Now
                };
                if (OrderPolicyId != 0 && OrderPolicyId != null)
                {
                    order_policyHolder.Id = (OrderPolicyId == null ? 0 : Convert.ToInt32(OrderPolicyId));
                    new AdminService().SavePolicyHolder(order_policyHolder);
                }
                else
                {
                    new AdminService().EditPolicyHolder(order_policyHolder);
                }
                Order_Pay order_pay = new Order_Pay()
                {
                    PayBank = 1,
                    PayType = 1,
                    CreateDate = DateTime.Now,
                    OrderCode = OrderCode
                };

                new OrderService().AddPay(order_pay);
                if (string.IsNullOrEmpty(OrderCode))
                {
                    result.Status = 0;
                    result.Error = "下单失败";
                    return result;
                }
                else
                {

                    result.Status = 1;
                    result.Data = OrderCode;
                    return result;
                }
            }
            catch (Exception ex)
            {
                I.Utility.Helper.LogHelper.AppError(string.Format("下单失败，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
                result.Status = 0;
                result.Error = ex.Message;
                return result;
            }

        }
    }
}