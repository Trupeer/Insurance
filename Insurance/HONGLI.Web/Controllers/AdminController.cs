using HONGLI.Entity;
using HONGLI.Service;
using HONGLI.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HONGLI.Web.Controllers
{
    public class AdminController : Controller
    {
        public AdminService adminservice = new AdminService();
        public ProductV2Service productv2service = new ProductV2Service();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult OrderList()
        {
            return View();
        }
        public ActionResult OrderListTable()
        {
            int Page = Convert.ToInt32(Request["Page"]);
            string CarID = Request["CarID"];
            string username = Request["username"];
            string ChannelName = Request["ChannelName"];
            string BeginInsuranceEndDate = Request["BeginInsuranceEndDate"];
            string EndInsuranceEndDate = Request["EndInsuranceEndDate"];
            string BeginVisitDate = Request["BeginVisitDate"];
            string EndVisitDate = Request["EndVisitDate"];
            string BeginDate = Request["BeginDate"];
            string EndDate = Request["EndDate"];
            string InsuranceCompany = Request["InsuranceCompany"];
            string VistState = Request["VistState"];
            string Auditdocuments = Request["Auditdocuments"];
            string OrderState = Request["OrderState"];
            string examineState = Request["examineState"];
            DataTable orderlist = adminservice.GetOrderList(Page, CarID, username, ChannelName, BeginInsuranceEndDate, EndInsuranceEndDate, BeginVisitDate, EndVisitDate, BeginDate, EndDate, InsuranceCompany, VistState, Auditdocuments, OrderState, examineState);
            ViewBag.list = orderlist;
            return PartialView();
        }
        [HttpGet]
        public decimal GetOrderListCount()
        {
            string CarID = Request["CarID"];
            string username = Request["username"];
            string ChannelName = Request["ChannelName"];
            string BeginInsuranceEndDate = Request["BeginInsuranceEndDate"];
            string EndInsuranceEndDate = Request["EndInsuranceEndDate"];
            string BeginVisitDate = Request["BeginVisitDate"];
            string EndVisitDate = Request["EndVisitDate"];
            string BeginDate = Request["BeginDate"];
            string EndDate = Request["EndDate"];
            string InsuranceCompany = Request["InsuranceCompany"];
            string VistState = Request["VistState"];
            string Auditdocuments = Request["Auditdocuments"];
            string OrderState = Request["OrderState"];
            string examineState = Request["examineState"];
            decimal SumCount = adminservice.GetOrderListCount(CarID, username, ChannelName, BeginInsuranceEndDate, EndInsuranceEndDate, BeginVisitDate, EndVisitDate, BeginDate, EndDate, InsuranceCompany, VistState, Auditdocuments, OrderState, examineState);
            return SumCount;
        }
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// 获取用户信息、判断登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public object GetLogin(string username,string password)
        {
            //对密码进行MD5加密
            password = Md5Hex(password);
            var result = adminservice.CheckUser(username, password);
            Dictionary<object, object> list = new Dictionary<object, object>();
            if(result!=null)
            {
                list.Add("ID", result.ID);
                list.Add("Name", result.Name);
                list.Add("Status", result.Status);
                list.Add("Mobile", result.Mobile);
                HttpContext.Session["UserId"] = result.ID;
                HttpContext.Session["UserName"] = result.Name;
            }
            else
            {
                return null;
            }
            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderDetail(int? UserId, int? ItemId,string OrderCode,int? InsuranceCompany)
        {
            var orderList = new OrderService().GetOrderByCode(OrderCode);
            if (orderList == null)
            {
                orderList = new Order_Base();
                orderList.Id = 0;
            }
            if (orderList.Order_Deliver.Count == 0)
            {
                Order_Deliver order_deliver = new Order_Deliver();
                order_deliver.Id = 0;
                orderList.Order_Deliver.Add(order_deliver);
            }
            if (orderList.Order_Item.Count == 0)
            {
                Order_Item order_item = new Order_Item();
                order_item.Id = 0;
                orderList.Order_Item.Add(order_item);
            }
            if (orderList.Order_PolicyHolder.Count == 0)
            {
                Order_PolicyHolder order_policyholder = new Order_PolicyHolder();
                order_policyholder.Id = 0;
                orderList.Order_PolicyHolder.Add(order_policyholder);
            }
            if(orderList.Order_Pay.Count==0)
            {
                Order_Pay order_pay = new Order_Pay();
                order_pay.Id = 0;
                orderList.Order_Pay.Add(order_pay);
            }
            ViewBag.OrderList = orderList;
            ViewBag.Product_User = new AdminService().GetProductUser(UserId);
            ViewBag.Product_Renewal = new AdminService().GetProductRenewal(UserId);
            if(ViewBag.Product_Renewal==null)
            {
                ProductV2_Renewal product_renewal = new ProductV2_Renewal();
                product_renewal.Id = 0;
                ViewBag.Product_Renewal = product_renewal;
            }
            ViewBag.Prduct_ItemSelect = new AdminService().GetProductItemSelection(UserId);
            if(ViewBag.Prduct_ItemSelect ==null)
            {
                ProductV2_Item product_item = new ProductV2_Item();
                product_item.Id = 0;
                ViewBag.Prduct_ItemSelect = product_item;
            }
            ViewBag.InsuranceCompany = InsuranceCompany;
            ViewBag.UserId = UserId;
            return View();
        }
        public object GetProductItem(int? UserId, int? InsuranceCompany)
        {
            ProductV2_Item model = new AdminService().GetProductItem(UserId, InsuranceCompany);
            return JsonConvert.SerializeObject(model);
        }
        [HttpPost]
        public ActionResult OrderDetail(ProductV2UserItemPolicyModels model)
        {
            if (HttpContext.Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            int userId = -1;
            int ProductItemId = -1;
            int ClientuserId = 0;
            BaseInfo_UserInfo baseinfo_userinfo = new BaseInfo_UserInfo();
            baseinfo_userinfo = new AdminService().GetUserIdByMobile(model.Mobile);
            if(baseinfo_userinfo!=null)
            {
                ClientuserId = baseinfo_userinfo.ID;
            }
            #region 用户表数据重写
            ProductV2_User productv2_user = new ProductV2_User();
            productv2_user.Id = model.Id;
            productv2_user.LicenseNo = model.LicenseNo;
            productv2_user.LicenseOwner = model.LicenseOwner;
            productv2_user.Mobile = model.Mobile;
            productv2_user.ModleName = model.ModleName;
            productv2_user.RegisterDate = model.RegisterDate;
            productv2_user.ForceExpireDate = model.ForceExpireDate;
            productv2_user.EngineNo = model.EngineNo;
            productv2_user.CredentislasNum = model.CredentislasNum;
            productv2_user.CarVin = model.CarVin;
            productv2_user.BusinessExpireDate = model.BusinessExpireDate;
            productv2_user.VisitStatus = model.VisitStatus;
            if(model.VisitStatus!=model.BeforeVisitStatus)
            {
                productv2_user.VisitDate = DateTime.Now;
            }
            else
            {
                productv2_user.VisitDate = model.VisitDate;
            }
            if(productv2_user.VisitStatus!=null|| productv2_user.VisitStatus !=0)
            {
                productv2_user.VisitServiceUserId = Convert.ToInt32(HttpContext.Session["UserId"]);
            }
            if (productv2_user.Id != 0)
            {
                adminservice.EditProductUser(productv2_user);
            }
            #endregion
            #region  核保信息表信息重新
            ProductV2_Item product_item = new ProductV2_Item();
            product_item.Id = model.ItemId;
            product_item.ProductName = model.ProductName;
            product_item.UserId = model.ItemUserId;
            product_item.Source = model.ItemSource;
            product_item.BizRate = model.BizRate;
            product_item.BizTotal = model.BizTotal;
            product_item.ForceRate = model.ForceRate;
            product_item.ForceTotal = model.ForceTotal;
            product_item.TaxRate = model.TaxRate;
            product_item.TaxTotal = model.TaxTotal;
            product_item.QuoteStatus = model.QuoteStatus;
            product_item.QuoteResult = model.QuoteResult;
            product_item.CheSun_BaoE = model.CheSun_BaoE;
            product_item.CheSun_BaoFei = model.CheSun_BaoFei;
            product_item.SanZhe_BaoE = model.SanZhe_BaoE;
            product_item.SanZhe_BaoFei = model.SanZhe_BaoFei;
            product_item.DaoQiang_BaoE = model.DaoQiang_BaoE == true ? 1 : 0;
            product_item.DaoQiang_BaoFei = model.DaoQiang_BaoFei;
            product_item.SiJi_BaoE = model.SiJi_BaoE;
            product_item.SiJi_BaoFei = model.SiJi_BaoFei;
            product_item.ChengKe_BaoE = model.ChengKe_BaoE;
            product_item.ChengKe_BaoFei = model.ChengKe_BaoFei;
            product_item.BoLi_BaoE = model.BoLi_BaoE;
            product_item.BoLi_BaoFei = model.BoLi_BaoFei;
            product_item.HuaHen_BaoE = model.HuaHen_BaoE;
            product_item.HuaHen_BaoFei = model.HuaHen_BaoFei;
            product_item.BuJiMianCheSun_BaoE = model.BuJiMianCheSun_BaoE == true ? 1 : 0;
            product_item.BuJiMianCheSun_BaoFei = model.BuJiMianCheSun_BaoFei;
            product_item.BuJiMianSanZhe_BaoE = model.BuJiMianSanZhe_BaoE == true ? 1 : 0;
            product_item.BuJiMianSanZhe_BaoFei = model.BuJiMianSanZhe_BaoFei;
            product_item.BuJiMianDaoQiang_BaoE = model.BuJiMianDaoQiang_BaoE == true ? 1 : 0;
            product_item.BuJiMianDaoQiang_BaoFei = model.BuJiMianDaoQiang_BaoFei;
            product_item.BuJiMianRenYuan_BaoE = model.BuJiMianRenYuan_BaoE == true ? 1 : 0;
            product_item.BuJiMianRenYuan_BaoFei = model.BuJiMianRenYuan_BaoFei;
            product_item.BuJiMianFuJia_BaoE = model.BuJiMianFuJia_BaoE == true ? 1 : 0;
            product_item.BuJiMianFuJia_BaoFei = model.BuJiMianFuJia_BaoFei;
            product_item.SheShui_BaoE = model.SheShui_BaoE == true ? 1 : 0;
            product_item.SheShui_BaoFei = model.SheShui_BaoFei;
            product_item.CheDeng_BaoE = model.CheDeng_BaoE == true ? 1 : 0;
            product_item.CheDeng_BaoFei = model.CheDeng_BaoFei;
            product_item.ZiRan_BaoE = model.ZiRan_BaoE == true ? 1 : 0;
            product_item.ZiRan_BaoFei = model.ZiRan_BaoFei;
            product_item.BuId = model.BuId;
            product_item.SubmitStatus = model.SubmitStatus;
            product_item.SubmitResult = model.SubmitResult;
            product_item.BizNo = model.BizNo;
            product_item.ForceNo = model.ForceNo;
            product_item.BizRate_Channel = model.BizRate_Channel;
            product_item.ForceRate_Channel = model.ForceRate_Channel;
            product_item.TaxRate_Channel = model.TaxRate_Channel;
            product_item.BizAfterCoupon = model.BizAfterCoupon;
            product_item.ForceAfterCoupon = model.ForceAfterCoupon;
            product_item.TaxAfterCoupon = model.TaxAfterCoupon;
            product_item.Total = model.Total;
            product_item.TotalAfterCoupon = model.TotalAfterCoupon;
            product_item.Description = model.Description;
            product_item.CreateTime = DateTime.Now;

            var description = "";
            description += product_item.CheSun_BaoE > 0 ? (string.Format("车损险({0})、", FormatNumber(Convert.ToDouble(product_item.CheSun_BaoE)))) : "";
            description += product_item.BuJiMianCheSun_BaoE > 0 ? ("不计免赔车损险、") : "";
            description += product_item.SanZhe_BaoE > 0 ? (string.Format("三者险({0})、", FormatNumber(Convert.ToDouble(product_item.SanZhe_BaoE)))) : "";
            description += product_item.BuJiMianSanZhe_BaoE > 0 ? ("不计免赔三者险、") : "";
            description += product_item.DaoQiang_BaoE > 0 ? ("盗抢险、") : "";
            description += product_item.BuJiMianDaoQiang_BaoE > 0 ? ("不计免赔盗抢险、") : "";
            description += product_item.SiJi_BaoE> 0 ? (string.Format("座位险(司机{0})、", FormatNumber(Convert.ToDouble(product_item.SiJi_BaoE)))) : "";
            description += product_item.ChengKe_BaoE > 0 ? (string.Format("座位险(乘客{0})、", FormatNumber(Convert.ToDouble(product_item.ChengKe_BaoE)))) : "";
            description += product_item.BuJiMianRenYuan_BaoE > 0 ? ("不计免赔车上人员、") : "";
            description += product_item.HuaHen_BaoE> 0 ? (string.Format("划痕险({0})、", FormatNumber(Convert.ToDouble(product_item.HuaHen_BaoE)))) : "";
            description += product_item.BoLi_BaoE > 0 ? (string.Format("玻璃单独破损险({0})、", BoLiType(Convert.ToDouble(product_item.BoLi_BaoE)))) : "";
            description += product_item.SheShui_BaoE > 0 ? ("发动机特别损失险、") : "";
            description += product_item.ZiRan_BaoE > 0 ? ("自燃险、") : "";
            if (product_item.Source == Convert.ToInt32(ProductCompany.PingAn)) //只有平安有此险种
            {
                description += product_item.CheDeng_BaoE > 0 ? ("倒车镜、车灯单独损失险、") : "";
            }
            description += product_item.BuJiMianFuJia_BaoE > 0 ? ("不计免赔附加险、") : "";
            product_item.Description = description.TrimEnd('、');
            if (product_item.Id == 0)
            {
                product_item.Id=ProductItemId = adminservice.SaveProductItem(product_item);
            }
            #endregion

            #region 订单信息 orderbase
            string ordercode = "100001" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            Order_Base order_base = new Order_Base();
            order_base.Id = model.OrderBaseId;
            order_base.OrderCode = model.OrderBaeOrderCode == null ? ordercode : model.OrderBaeOrderCode;
            order_base.CreateDate = model.OrderBaeCreateDate == null ? DateTime.Now : model.OrderBaeCreateDate;
            order_base.Channel = model.OrderBaeChannel == null ? Convert.ToInt32(model.Channel) : model.OrderBaeChannel;
            order_base.UserId = model.OrderBaeUserId == null ? ClientuserId.ToString() : model.OrderBaeUserId;
            order_base.ProductItemId = product_item.Id;
            order_base.BackStatus = 0;
            //order_item订单详情表
            Order_Item order_item = new Order_Item();
            order_item.Id = model.OrderItemId;
                order_item.OrderCode = order_base.OrderCode;
                order_item.CreateDate = model.OrderItemCreateDate==null?DateTime.Now:model.OrderItemCreateDate;
                order_item.ProductId = product_item.Id.ToString();
                order_item.ProductName = new ProductV2Service().GetCompanyName(Convert.ToInt32(model.ItemSource));
                order_item.InsuranceLogo = product_item.Source;
                order_item.LicenseNo = model.LicenseNo;
                order_base.Order_Item = new List < Order_Item >() { order_item};
            if (order_base.Id != 0)
            {
                adminservice.EditOrder_Base(order_base);
            }
            else
            {
                new OrderService().AddOrder(order_base);
            }
            //order_deliver订单地址
            Order_Deliver order_deliver = new Order_Deliver();
            order_deliver.Id = model.OrderDeliverId;
            order_deliver.OrderCode = model.OrderDeliverOrderCode == null ? order_base.OrderCode : model.OrderDeliverOrderCode;
            order_deliver.DeliverType = model.DeliverType;
            order_deliver.DeliverAddress = model.DeliverAddress;
            order_deliver.DeliverPrice = model.DeliverPrice;
            order_deliver.DeliverName = model.DeliverName;
            order_deliver.DeliverMobile = model.DeliverMobile;
            order_deliver.DeliverDistrictCode = model.DeliverDistrictCode;
            order_deliver.CreateDate = model.OrderBaeCreateDate==null?DateTime.Now:model.OrderBaeCreateDate;
            order_deliver.UserId = order_base.UserId==null?0:Convert.ToInt32(order_base.UserId);
            //order_base.Order_Deliver = new List<Order_Deliver>() { order_deliver };
            if(order_deliver.Id!=0)
            {
                adminservice.EditOrderDeliver(order_deliver);
            }
            else
            {
                adminservice.SaveOrderDeliver(order_deliver);   
            }

            #endregion

            #region 支付方式
            Order_Pay order_pay = new Order_Pay();
            order_pay.Id = model.OrderPayId;
            order_pay.OrderCode = order_base.OrderCode;
            order_pay.PayType = model.OrderPayPayType;
            order_pay.PayBank = model.OrderPayPayBank;
            order_pay.CreateDate = model.OrderPayCreateDate == null ? DateTime.Now : model.OrderPayCreateDate;
            if (order_pay.Id!=0)
            {
                adminservice.EditOrderPay(order_pay);
            }
            else
            {
                adminservice.SaveOrderPay(order_pay);
            }
            #endregion
            #region 被保人信息
            Order_PolicyHolder user_policyholder = new Order_PolicyHolder();
            user_policyholder.Id = Convert.ToInt32(model.PolicyId);
            user_policyholder.OrderCode = model.PolicyOrderCode == null ? order_base.OrderCode : model.PolicyOrderCode;
            user_policyholder.Name = model.PolicyName;
            user_policyholder.IdCardType = model.PolicyIdCardType;
            user_policyholder.IdCard = model.PolicyIdCard;
            user_policyholder.IdCardFacePicUrl = model.PolicyIdCardFacePicUrl;
            user_policyholder.IdCardBackPicUrl = model.PolicyIdCardBackPicUrl;
            user_policyholder.UserId = model.Id;
            user_policyholder.CreateDate = model.PolicyCreateDate;
            if (user_policyholder.Id != 0)
            {
                adminservice.EditPolicyHolder(user_policyholder);
            }
            else
            {
                adminservice.SavePolicyHolder(user_policyholder);
            }

            #endregion

            //修改用户选择项
                new ProductV3Service().EditProductItemUserCheck(model.ItemId, model.Id);

            var orderList = new OrderService().GetOrderByCode(order_base.OrderCode);
            if (orderList == null)
            {
                orderList = new Order_Base();
                orderList.Id = 0;
            }
            if (orderList.Order_Deliver.Count == 0)
            {
                Order_Deliver order_delivernew = new Order_Deliver();
                order_deliver.Id = 0;
                orderList.Order_Deliver.Add(order_delivernew);
            }
            if (orderList.Order_Item.Count == 0)
            {
                Order_Item order_itemnew = new Order_Item();
                order_item.Id = 0;
                orderList.Order_Item.Add(order_itemnew);
            }
            if (orderList.Order_PolicyHolder.Count == 0)
            {
                Order_PolicyHolder order_policyholder = new Order_PolicyHolder();
                order_policyholder.Id = 0;
                orderList.Order_PolicyHolder.Add(order_policyholder);
            }
            if (orderList.Order_Pay.Count == 0)
            {
                Order_Pay order_paylist = new Order_Pay();
                order_paylist.Id = 0;
                orderList.Order_Pay.Add(order_paylist);
            }
            ViewBag.OrderList = orderList;
            ViewBag.Product_User = new AdminService().GetProductUser(model.Id);
            ViewBag.Product_Renewal = new AdminService().GetProductRenewal(model.Id);

            ViewBag.Prduct_ItemSelect = new AdminService().GetProductItemSelection(model.Id);
            ViewBag.InsuranceCompany = model.ItemSource;
            ViewBag.UserId = model.Id;
            return View(model);
        }
        public string FormatNumber(double number)
        {
            if (number < 10000)
            {
                return number.ToString() + "元";
            }
            else
            {
                var index = number.ToString().LastIndexOf('0') - 3;
                return number.ToString().Substring(0, index) + "万";
            }
        }
        public string BoLiType(double type)
        {
            if (type == 1)
            {
                return "国产";
            }
            else if (type == 2)
            {
                return "进口";
            }
            return "";
        }
        [HttpPost]
        public int ChangeAuditDocuments(int ItemId,int Clientuser)
        {
            if(HttpContext.Session["UserId"]==null)
            {
                return 3;
            }
            int UserId = Convert.ToInt32(HttpContext.Session["UserId"].ToString());
            new ProductV3Service().EditProductItemUserCheck(ItemId, Clientuser);
            int result = new AdminService().ChangeAuditDocuments(UserId, ItemId);
            if(result>0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        [HttpPost]
        public int ChangeOrderList(int ProductItemId, int orderbaseId,int orderitemId,string ordercode, decimal? PrepaidAmount, int? Status, decimal? AmountPayable,int? buid,DateTime? BusinessExpireDate,DateTime? ForceExpireDate,int? ItemId,decimal? ProductDealPrice,decimal? ProductOriginalPrice,string description)
        {
            Order_Base order_base = new Order_Base();
            order_base.Id = orderbaseId;
            order_base.OrderCode = ordercode;
            order_base.Status = Status;
            //order_base.PrepaidAmount = PrepaidAmount;
            //order_base.AmountPayable = AmountPayable;
            order_base.CreateDate = DateTime.Now;
            order_base.ProductItemId = ProductItemId;
            Order_Item order_item = new Order_Item();
            order_item.Id = orderitemId;
            order_item.OrderCode = ordercode;
            order_item.ProductId = ItemId.ToString();
            order_item.ProductName = new ProductV2Service().GetCompanyName(Convert.ToInt32(ItemId));
            order_item.BusinessExpireDate = BusinessExpireDate;
            order_item.ForceExpireDate = ForceExpireDate;
            order_item.ProductDealPrice = ProductDealPrice;
            order_item.ProductOriginalPrice = ProductOriginalPrice;
            order_item.ProductTitle = description;
            order_item.Buid = buid;
            order_base.Order_Item.Add(order_item);
            int result = new AdminService().ChangeOrderList(order_base);
            if (result>0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #region 订单详情
        public ActionResult BillList()
        {
            return View();
        }
        public ActionResult BillListTable(int Page,string CarID,string PolicyName,DateTime? BillCreateDate,int? ddlOrderState,string BillCode)
        {
            DataTable orderlist = adminservice.GetBillList(Page, CarID, PolicyName, BillCode, BillCreateDate, ddlOrderState);
            ViewBag.list = orderlist;
            return PartialView();
        }
        [HttpGet]
        public decimal GetBillListCount(string CarID, string PolicyName, DateTime? BillCreateDate, int? ddlOrderState, string BillCode)
        {
           
            decimal SumCount = adminservice.GetBillListCount(CarID, PolicyName, BillCode, BillCreateDate, ddlOrderState);
            return SumCount;
        }
        public ActionResult BillDetail(int? UserId, int? ProductId, string OrderCode)
        {
            ViewBag.list = adminservice.GetBillDetails(UserId, ProductId, OrderCode);
            return View();
        }
        public int EditOrderStatus(int orderbaseid,string ordercode,int status)
        {
            int result = adminservice.EditOrderStatus(orderbaseid, ordercode, status);
            if(result>0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public int EditDeliverDetail(int? InvoiceType,string InvoiceTitle, int? PayType,int? DeliverType,int? beforeDeliverType,string DeliverName,string DeliverMobile,string DeliverDistrictCode,string DeliverAddress,int? OrderBaseId,int? OrderDeliverId,string orderCode,decimal? DeliverPrice,int? UserId,DateTime? OrderBaeCreateDate)
        {
            int result = -1;
            Order_Base order_base = new Order_Base();
            order_base.Id = OrderBaseId == null ? 0 :Convert.ToInt32(OrderBaseId);
            order_base.OrderCode = orderCode;
            order_base.InvoiceTitle = InvoiceTitle;
            order_base.InvoiceType = InvoiceType;
            Order_Deliver order_deliver = new Order_Deliver();
            order_deliver.Id = OrderDeliverId == null ? 0 : Convert.ToInt32(OrderDeliverId);
            order_deliver.OrderCode = orderCode;
            order_deliver.DeliverType = DeliverType;
            order_deliver.DeliverAddress = DeliverAddress;
            order_deliver.DeliverPrice = DeliverPrice;
            order_deliver.DeliverName = DeliverName;
            order_deliver.DeliverMobile =DeliverMobile;
            order_deliver.DeliverDistrictCode = DeliverDistrictCode;
            order_deliver.CreateDate = OrderBaeCreateDate == null ? DateTime.Now : OrderBaeCreateDate;
            order_deliver.UserId = UserId;
            Order_Pay order_pay = new Order_Pay();
            order_pay.OrderCode = orderCode;
            order_pay.PayType = PayType;
            order_base.Order_Pay.Add(order_pay);
            if (DeliverType!=null|| DeliverAddress!=""|| DeliverPrice!=null|| DeliverPrice!=0|| DeliverName!=""|| DeliverMobile!=""|| DeliverDistrictCode!="")
            {
                result=adminservice.EditOrderDeliver(order_deliver);
            }
            result = adminservice.EditInvoice(order_base);
            if (result==-1)
            {
                return 0;
            }
            else
            {
                return 1;
            }
            
        }
        public int AddRedPacket(int Id, string OrderCode,string name,decimal money,string mobile)
        {
            if (HttpContext.Session["UserId"] == null)
            {
                return -3;
            }
            RedPacket redpacket = new RedPacket();
            redpacket.OrderCode = OrderCode;
            redpacket.RedPacketMoney = money;
            redpacket.RedPacketName = "返现给" + name + "保费" + money + "元"; 
            redpacket.CreateTime = DateTime.Now;
            redpacket.State = 0;
            redpacket.RedPacketRemark = "返现给" + name + "保费" + money + "元";
            redpacket.CreateName = HttpContext.Session["UserId"].ToString();
            redpacket.RedPacketCode = GetRedPacketCode();
            int result = new AdminService().AddRedPacket(redpacket,Id);
            if(result>-1)
            {
                SMSController sms = new SMSController();
                try
                {
                    new Util().SendRedPacketSMS(mobile, new string[] { redpacket.RedPacketCode, "5" });
                }
                catch
                {
                    result = -2;
                }
            }
            return result;
        }
        #endregion
        #region 对账
        public ActionResult ReportStatistics()
        {
            return View();
        }

        public ActionResult ReportDetail()
        {
            return View();
        }

        public ActionResult ExportExcel()
        {
            string OrderCode = Request["OrderCode"];
            string Name = Request["Name"];
            string LicenseNo = Request["LicenseNo"];
            string BeginDate = Request["BeginDate"];
            string EndDate = Request["EndDate"];
            string InsuranceCompany = Request["InsuranceCompany"];
            DataTable orderlist = adminservice.ExportReport(OrderCode, BeginDate, EndDate, InsuranceCompany, Name, LicenseNo);
            System.IO.MemoryStream ms = HONGLI.Service.NPOIHelper.RenderDataTableToExcel(orderlist, "");
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", HttpUtility.UrlEncode("报表信息" + "_" + DateTime.Now.ToString("yyyy-MM-dd"), System.Text.Encoding.UTF8)));
            Response.BinaryWrite(ms.ToArray());
            Response.End();
            ms.Close();
            ms.Dispose();
            return View();
        }

        public ActionResult ReportStatisticsListTable()
        {
            int Page = Convert.ToInt32(Request["Page"]);
            string OrderCode = Request["OrderCode"];
            string Name = Request["Name"];
            string LicenseNo = Request["LicenseNo"];
            string BeginDate = Request["BeginDate"];
            string EndDate = Request["EndDate"];
            string InsuranceCompany = Request["InsuranceCompany"];
            DataTable orderlist = adminservice.GetReportList(OrderCode, Page, BeginDate, EndDate, InsuranceCompany, Name, LicenseNo);
            ViewBag.list = orderlist;
            return PartialView();
        }
        [HttpGet]
        public decimal GetReportListCount()
        {
            int Page = Convert.ToInt32(Request["Page"]);
            string OrderCode = Request["OrderCode"];
            string Name = Request["Name"];
            string LicenseNo = Request["LicenseNo"];
            string BeginDate = Request["BeginDate"];
            string EndDate = Request["EndDate"];
            string InsuranceCompany = Request["InsuranceCompany"];
            decimal SumCount = adminservice.GetReportListCount(OrderCode, Page, BeginDate, EndDate, InsuranceCompany, Name, LicenseNo);
            return SumCount;
        }

        public ActionResult ReportSummary()
        {
            string OrderCode = Request["OrderCode"];
            string Name = Request["Name"];
            string LicenseNo = Request["LicenseNo"];
            string BeginDate = Request["BeginDate"];
            string EndDate = Request["EndDate"];
            string InsuranceCompany = Request["InsuranceCompany"];
            DataTable orderlist = adminservice.GetReportSummary(OrderCode, BeginDate, EndDate, InsuranceCompany, Name, LicenseNo);
            ViewBag.list = orderlist;
            return PartialView();
        }
        #endregion
        /// <summary>   
        /// MD5加密并输出十六进制字符串   
        /// </summary>   
        /// <param name="str"></param>   
        /// <returns></returns>   
        public static string Md5Hex(string str)
        {
            string dest = "";
            //实例化一个md5对像   
            MD5 md5 = MD5.Create();
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　   
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得   
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是大写的字母   
                if (s[i] < 16)
                {
                    dest = dest + "0" + s[i].ToString("X");
                }
                else
                {
                    dest = dest + s[i].ToString("X");
                }
            }
            return dest;
        }
        public static string GetRedPacketCode()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
    }
}