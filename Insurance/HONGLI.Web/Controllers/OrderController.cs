using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HONGLI.Entity;
using HONGLI.Service;
using HONGLI.Web.Models;
using X3;
using static HONGLI.Repository.OrderRepository;
using Newtonsoft.Json;

namespace HONGLI.Web.Controllers
{
#if (!DEBUG)
    [AuthorizationFilter]
#endif
    public class OrderController : Controller
    {
        
        public OrderService _orderService = new OrderService();

        public PayService _payService = new PayService();

        public ProductV2Service _productV2Service = new ProductV2Service();
        

        // GET: Order/index
        public ActionResult Index()
        {
           

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
                    OrderStatus = order.Status

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
        public ActionResult Do(int? productId,string mobile, string channel,int? intentionCompany, string OrderCode)
        {
            ViewBag.ProductId = productId;
            ViewBag.Mobile = mobile;
            ViewBag.Channel = channel;
            ViewBag.intentionCompany = intentionCompany;
#if (!DEBUG)
      ViewBag.UserId = UserViewModel.CurrentUser.ID;
#else
            ViewBag.UserId = 3;
#endif

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
        public ActionResult Submit(ProductEntity product, InvoiceEntity invoice,PayAndDeliverEntity payAndDeliver, DeliverEntity deliver,PolicyHolderEntity policyHolder)
        {
            
            HONGLI.Entity.ResultData resultData = AddOrder(product, invoice,payAndDeliver, deliver, policyHolder);           
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
        public HONGLI.Entity.ResultData AddOrder(ProductEntity product,InvoiceEntity invoice, PayAndDeliverEntity payAndDeliver, DeliverEntity deliver, PolicyHolderEntity policyHolder)
        {
            HONGLI.Entity.ResultData result = new Entity.ResultData();

            try
            {


#if (!DEBUG)
    var userid = UserViewModel.CurrentUser.ID.ToString();
#else
                var userid = "3";
#endif



                string orderCode = "100001" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

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

                Order_Deliver order_deliver = new Order_Deliver();
                if (payAndDeliver.DeliverType == 1)
                {
                    order_deliver.DeliverDistrictCode = deliver.deliverDistrictCode;
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
                    order_deliver.DeliverAddress = payAndDeliver.SelfTakeAddress;
                    order_deliver.DeliverTime = payAndDeliver.SelfTakeDate;
                    order_deliver.DeliverPrice = payAndDeliver.DeliverPrice;
                    order_deliver.CreateDate = DateTime.Now;
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

                Order_Item order_item = new Order_Item()
                {
                    Channel = product.Channel,
                    ProductId = product.ProductId,
                    LicenseNo = product.LicenseNo,
                    InsuranceLogo = product.IsuranceLogo,
                    ProductName = product.Name,
                    ProductDealPrice = productDealPrice,
                    ProductOriginalPrice = product.OriginalPrice,
                    ProductTitle = product.Title,
                    ForceExpireDate = product.ForceExpireDate,
                    BusinessExpireDate = product.BusinessExpireDate,
                    Buid = product.Buid,
                    CreateDate = DateTime.Now
                };

                Order_Pay order_pay = new Order_Pay()
                {
                    PayBank = 1,
                    PayType = 1,
                    CreateDate = DateTime.Now
                };

                Order_Base order_base = new Order_Base()
                {
                    OrderCode = orderCode,
                    Channel = product.Channel,
                    AmountPayable = productDealPrice + order_deliver.DeliverPrice,//订单应付价格 = 产品价格+配送价格                
                    UserId = userid,
                    InvoiceType = invoice.InvoiceType,
                    InvoiceTitle = invoice.InvoiceTitle,
                    Status = (int)EnumOrderStatus.UnPay,
                    CreateDate = DateTime.Now,
                    Order_Deliver = new List<Order_Deliver>() { order_deliver },
                    Order_Item = new List<Order_Item>() { order_item },
                    Order_Pay = new List<Order_Pay>() { order_pay },
                    Order_PolicyHolder = new List<Order_PolicyHolder>() { order_policyHolder }
                };

                var code = _orderService.AddOrder(order_base);

                if (string.IsNullOrEmpty(code))
                {
                    result.Status = 0;
                    result.Error = "下单失败";
                    return result;
                }
                else
                {
                    #region postOrder

                    bool flag = false;
                    PostOrderModel model = new PostOrderModel();
                    model.OrderNum = order_base.OrderCode;
                    model.InsuredName = order_policyHolder.Name;
                    model.IdType = order_policyHolder.IdCardType.Value;
                    model.IdNum = order_policyHolder.IdCard;
                    model.Buid = order_item.Buid.Value;
                    model.Receipt = order_base.InvoiceTitle;
                    model.PayType = 1;
                    model.DistributionType = order_deliver.DeliverType.Value;
                    model.DistributionAddress = order_deliver.DeliverAddress;
                    model.DistributionTime = order_deliver.CreateDate.Value.ToString("yyyy-MM-dd");

                    model.InsurancePrice = Convert.ToDouble(order_item.ProductDealPrice);
                    model.CarriagePrice = Convert.ToDouble(order_deliver.DeliverPrice);
                    model.TotalPrice = Convert.ToDouble(order_base.AmountPayable);
                    model.UserId = Convert.ToDouble(order_base.UserId.ToString());

                    model.Agent = Convert.ToInt32(I.Utility.Util.GetConfigByKey("ApiAgent"));


                    flag = PostOrderService.PostOrder(model);

                    #endregion

                    result.Status = 1;
                    result.Data = code;
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