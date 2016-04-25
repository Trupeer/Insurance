using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using I.Utility.Extensions;
using HONGLI.Service;
using HONGLI.Entity;
using System.Web.SessionState;

namespace HONGLI.Web.Controllers
{
    /// <summary>
    /// 询价模块 by Lee 20160310
    /// </summary>    
    public class ProductController : Controller, IRequiresSessionState
    {
        public ProductService productService = new ProductService();
        // GET: Product

        #region  Step1-Index

        public ActionResult Index(string mobile, int? channel)
        {
            channel = channel ?? 1;

            ViewBag.mobile = mobile;
            ViewBag.channel = channel;

            return View();
        }

        #endregion

        #region Step2-fill info 

        public ActionResult FillInfo(int? channel, string lastYearCompany, string mobile, string carLicense, string userIdentity, string cityCode, string modelNameJson)
        {
            channel = channel ?? 1;

            ViewBag.mobile = mobile;
            ViewBag.channel = channel;
            ViewBag.carLicense = carLicense;

            #region 修改了品牌型号 20160318 需求调整成不能修改，我有点怒了
            var modelName = "";
            var carVin = "";
            var engineNo = "";
            var registerDate = "";
            if (modelNameJson.IsNotNull() && modelNameJson.Contains("modelName"))
            {
                var dict = modelNameJson.FromJsonTo<Dictionary<string, object>>();
                modelName = dict["modelName"].ToString();
                carVin = dict["carVin"].ToString();
                engineNo = dict["engineNo"].ToString();
                registerDate = dict["registerDate"].ToString();
            }
            ViewBag.modelName = modelName;
            #endregion

            var result = productService.GetReInsuranceInfo(lastYearCompany, mobile, carLicense, userIdentity, cityCode);
            var data = result.FromJsonTo<UserReInsuranceResult>();
            if (data.ErrCode == 0)
            {
                data.UserInfo.CityCode = int.Parse(cityCode);
                data.UserInfo.Mobile = mobile;

                #region 修改了品牌型号 20160318 需求调整成不能修改，我有点怒了
                data.UserInfo.MoldName = modelName.IsNull()? data.UserInfo.MoldName: modelName;
                data.UserInfo.CarVin = carVin.IsNull() ? data.UserInfo.CarVin : carVin;
                data.UserInfo.EngineNo = engineNo.IsNull() ? data.UserInfo.EngineNo : engineNo;
                data.UserInfo.RegisterDate = registerDate.IsNull() ? data.UserInfo.RegisterDate : registerDate;
                #endregion
                
                data.Result = data.Result.OrderByDescending(t => t.CreateTime).ToList();

                var key = string.Format("{0}{1}{2}", carLicense, mobile, "_insurance");
                HttpContext.Session[key] = data;
            }
            else
            {
                data.Result = new List<Result>();

                data.UserInfo = new UserInfo();
            }

            return View(data);
        }

        #endregion

        #region Step2-2 LicenseModify

        public ActionResult LicenseModify(int? channel, string carLicense, string mobile, string modlName, string cityCode)
        {
            channel = channel ?? 1;

            ViewBag.channel = channel;
            ViewBag.carLicense = carLicense;
            ViewBag.mobile = mobile;
            ViewBag.modelName = modlName;
            ViewBag.cityCode = cityCode;

            return View();
        }

        #endregion

        #region Step3-EnquiryResult

        //单条询价接口
        public JsonResult GetPrecisePrice(int? channel, string intentionCompany, string carLicense, string mobile, string modelNameJson)
        {
            channel = channel ?? 1;

            ViewBag.mobile = mobile;
            ViewBag.channel = channel;

            var key = carLicense + mobile + "_insurance";
            var tempData = HttpContext.Session[key];
            if (tempData == null)
            {
                return Json(null);
            }

            var model = new UserPrecisePriceInComingParamModel();

            #region 入参用户信息
            var info = tempData as UserReInsuranceResult;

            var modelName = "";
            var engineNo = "";
            var carVin = "";
            var registerDate = "";

            if (modelNameJson.IsNotNull() && modelNameJson != "\"\"")
            {
                var modelNameItem = modelNameJson.FromJsonTo<Dictionary<string, string>>();
                modelName = modelNameItem["modelName"];
                engineNo = modelNameItem["engineNo"];
                carVin = modelNameItem["carVin"];
                registerDate = modelNameItem["registerDate"];
            }
            else
            {
                modelName = info.UserInfo.MoldName;
                engineNo = info.UserInfo.EngineNo;
                carVin = info.UserInfo.CarVin;
                registerDate = info.UserInfo.RegisterDate;
            }

            model.mobile = mobile; //info.UserInfo.Mobile;
            model.carLicense = info.UserInfo.LicenseNo;
            model.intentionCompany = intentionCompany;
            model.userIdentity = "";
            model.carType = 0;  //私家车
            model.useType = 0;  //非营运
            model.isNewCar = 0;
            model.citycode = info.UserInfo.CityCode.ToString();
            model.moldName = modelName;
            model.engineNo = engineNo;
            model.carVin = carVin;
            model.registerDate = registerDate;
            #endregion

            #region 入参各种类型选择

            var key3 = string.Format("{0}{1}{2}", carLicense, mobile, "_dataitem");
            var tempDataItem = HttpContext.Session[key3];
            var incomingParamModel = tempDataItem.ToString().FromJsonTo<Dictionary<string, object>>();

            model.boli = Convert.ToDouble(incomingParamModel["boli"]);
            model.bujimianchesun = Convert.ToDouble(incomingParamModel["bujimianchesun"]);
            model.bujimiandaoqiang = Convert.ToDouble(incomingParamModel["bujimiandaoqiang"]);
            model.bujimianfujia = Convert.ToDouble(incomingParamModel["bujimianfujia"]);
            model.bujimianrenyuan = Convert.ToDouble(incomingParamModel["bujimianrenyuan"]);
            model.bujimiansanzhe = Convert.ToDouble(incomingParamModel["bujimiansanzhe"]);
            model.chedeng = Convert.ToDouble(incomingParamModel["chedeng"]);
            model.chengke = Convert.ToDouble(incomingParamModel["chengke"]);
            model.chesun = Convert.ToDouble(incomingParamModel["chesun"]);
            model.daoqiang = Convert.ToDouble(incomingParamModel["daoqiang"]);
            model.huahen = Convert.ToDouble(incomingParamModel["huahen"]);
            model.sanzhe = Convert.ToDouble(incomingParamModel["sanzhe"]);
            model.sheshui = Convert.ToDouble(incomingParamModel["sheshui"]);
            model.siji = Convert.ToDouble(incomingParamModel["siji"]);
            model.ziran = Convert.ToDouble(incomingParamModel["ziran"]);

            #endregion

            var result = productService.GetPrecisePrice(model.ToJsonItem());

            var data = result.FromJsonTo<UserReInsuranceResult>();

            //成功写数据
            if (data != null && data.ErrCode == 0)
            {
                //接口这俩值为null，在这儿再次赋值
                data.UserInfo.CityCode = int.Parse(model.citycode);
                data.UserInfo.Mobile = mobile;

                #region 组合险种描述Description字段
                if (data.Result != null && data.Result.Count > 0)
                {
                    var result0 = data.Result[0];
                    var description = "";
                    description += result0.CheSun.BaoE > 0 ? ("车损险、") : "";
                    description += result0.SanZhe.BaoE > 0 ? (string.Format("三者险({0})、", result0.SanZhe.BaoFei)) : "";
                    description += result0.DaoQiang.BaoE > 0 ? ("盗抢险、") : "";
                    description += result0.SiJi.BaoE > 0 ? (string.Format("座位险(司机{0})、", result0.SiJi.BaoFei)) : "";
                    description += result0.ChengKe.BaoE > 0 ? (string.Format("座位险(乘客{0})、", result0.ChengKe.BaoFei)) : "";
                    description += result0.HuaHen.BaoE > 0 ? (string.Format("划痕险({0})、", result0.HuaHen.BaoFei)) : "";
                    description += result0.BoLi.BaoE > 0 ? (string.Format("玻璃单独破损险({0})、", result0.BoLi.BaoFei)) : "";
                    description += result0.SheShui.BaoE > 0 ? ("涉水险、") : "";
                    description += result0.CheDeng.BaoE > 0 ? ("车灯单独损失险") : "";
                    description += result0.ZiRan.BaoE > 0 ? ("自然损失险") : "";
                    data.Result[0].Description = description.TrimEnd('、');


                }
                else
                {
                    data.Result = new List<Result>();
                }
                #endregion

                var keyPrice = carLicense + mobile + intentionCompany + "_price";
                HttpContext.Session[keyPrice] = data;
            }

            return Json(data);
        }

        //询价列表
        public ActionResult ResultList(int? channel, string carLicense, string mobile, string dataItem)
        {
            channel = channel ?? 1;

            ViewBag.channel = channel;
            ViewBag.carLicense = carLicense;
            ViewBag.mobile = mobile;

            var key = string.Format("{0}{1}{2}", carLicense, mobile, "_dataitem");
            HttpContext.Session[key] = dataItem;

            return View();
        }

        #endregion

        #region Step4-resultdetail

        public ActionResult ResultDetail(int? channel, string carLicense, string mobile, int intentionCompany)
        {
            channel = channel ?? 1;

            ViewBag.channel = channel;
            ViewBag.intentionCompany = intentionCompany;
            ViewBag.carLicense = carLicense;
            ViewBag.mobile = mobile;

            var bizCouponRateArray = I.Utility.Util.GetConfigByKey("BizCouponRate").Split(',').ToArray();
            var forceCouponRateArray = I.Utility.Util.GetConfigByKey("ForceCouponRate").Split(',').ToArray();
            var taxCouponRateArray = I.Utility.Util.GetConfigByKey("TaxCouponRate").Split(',').ToArray();

            ViewBag.bizCouponRate = int.Parse(bizCouponRateArray[intentionCompany]);
            ViewBag.forceCouponRate = int.Parse(forceCouponRateArray[intentionCompany]);
            ViewBag.taxCouponRate = int.Parse(taxCouponRateArray[intentionCompany]);

            var keyPrice = carLicense + mobile + intentionCompany + "_price";
            var data = HttpContext.Session[keyPrice] as UserReInsuranceResult;

            return View(data);
        }

        //立即购买
        public JsonResult Buy(int? channel, string carLicense, string mobile, string intentionCompany)
        {
            channel = channel ?? 1;

            ViewBag.channel = channel;
            ViewBag.carLicense = carLicense;
            ViewBag.mobile = mobile;
            ViewBag.intentionCompany = intentionCompany;

            var keyPrice = carLicense + mobile + intentionCompany + "_price";
            var data = HttpContext.Session[keyPrice] as UserReInsuranceResult;

            View_ProductUserItem userItem = null;

            if (data != null && data.ErrCode == 0)
            {
                #region product_user

                var product_user = new Product_User();
                data.UserInfo.MapToEntity<Product_User>(product_user);
                product_user.CreateTime = DateTime.Now;
                product_user.Creator = mobile;

                #endregion

                #region product_item

                var product_item = new Product_Item();
                var result = data.Result[0];
                result.MapToEntity<Product_Item>(product_item);

                product_item.BoLi_BaoE = Convert.ToDecimal(result.BoLi.BaoE);
                product_item.BoLi_BaoFei = Convert.ToDecimal(result.BoLi.BaoFei);
                product_item.BuJiMianCheSun_BaoE = Convert.ToDecimal(result.BuJiMianCheSun.BaoE);
                product_item.BuJiMianCheSun_BaoFei = Convert.ToDecimal(result.BuJiMianCheSun.BaoFei);
                product_item.BuJiMianDaoQiang_BaoE = Convert.ToDecimal(result.BuJiMianDaoQiang.BaoE);
                product_item.BuJiMianDaoQiang_BaoFei = Convert.ToDecimal(result.BuJiMianDaoQiang.BaoFei);
                product_item.BuJiMianFuJia_BaoE = Convert.ToDecimal(result.BuJiMianFuJia.BaoE);
                product_item.BuJiMianFuJia_BaoFei = Convert.ToDecimal(result.BuJiMianFuJia.BaoFei);
                product_item.BuJiMianRenYuan_BaoE = Convert.ToDecimal(result.BuJiMianRenYuan.BaoE);
                product_item.BuJiMianRenYuan_BaoFei = Convert.ToDecimal(result.BuJiMianRenYuan.BaoFei);
                product_item.BuJiMianSanZhe_BaoE = Convert.ToDecimal(result.BuJiMianSanZhe.BaoE);
                product_item.BuJiMianSanZhe_BaoFei = Convert.ToDecimal(result.BuJiMianSanZhe.BaoFei);
                product_item.CheDeng_BaoE = Convert.ToDecimal(result.CheDeng.BaoE);
                product_item.CheDeng_BaoFei = Convert.ToDecimal(result.CheDeng.BaoFei);
                product_item.ChengKe_BaoE = Convert.ToDecimal(result.ChengKe.BaoE);
                product_item.ChengKe_BaoFei = Convert.ToDecimal(result.ChengKe.BaoFei);
                product_item.CheSun_BaoE = Convert.ToDecimal(result.CheSun.BaoE);
                product_item.CheSun_BaoFei = Convert.ToDecimal(result.CheSun.BaoFei);
                product_item.DaoQiang_BaoE = Convert.ToDecimal(result.DaoQiang.BaoE);
                product_item.DaoQiang_BaoFei = Convert.ToDecimal(result.DaoQiang.BaoFei);
                product_item.HuaHen_BaoE = Convert.ToDecimal(result.HuaHen.BaoE);
                product_item.HuaHen_BaoFei = Convert.ToDecimal(result.HuaHen.BaoFei);
                product_item.ZiRan_BaoE = Convert.ToDecimal(result.ZiRan.BaoE);
                product_item.ZiRan_BaoFei = Convert.ToDecimal(result.ZiRan.BaoFei);


                var bizCouponRateArray = I.Utility.Util.GetConfigByKey("BizCouponRate").Split(',').ToArray();
                var forceCouponRateArray = I.Utility.Util.GetConfigByKey("ForceCouponRate").Split(',').ToArray();
                var taxCouponRateArray = I.Utility.Util.GetConfigByKey("TaxCouponRate").Split(',').ToArray();

                var bizCouponRate = int.Parse(bizCouponRateArray[int.Parse(intentionCompany)]);
                var forceCouponRate = int.Parse(forceCouponRateArray[int.Parse(intentionCompany)]);
                var taxCouponRate = int.Parse(taxCouponRateArray[int.Parse(intentionCompany)]);


                product_item.BizTotal = Convert.ToDecimal(result.BizTotal);
                product_item.BussinessCouponRate = bizCouponRate;
                product_item.BussinessAfterCoupon = Convert.ToDecimal(result.BizTotal * bizCouponRate * 0.01);

                product_item.ForceTotal = Convert.ToDecimal(result.ForceTotal);
                product_item.ForceCouponRate = forceCouponRate;
                product_item.ForceAfterCoupon = Convert.ToDecimal(result.ForceTotal * forceCouponRate * 0.01);

                product_item.TaxTotal = Convert.ToDecimal(result.TaxTotal); ;
                product_item.TaxCouponRate = taxCouponRate;
                product_item.TaxAfterCoupon = Convert.ToDecimal(result.TaxTotal * forceCouponRate * 0.01);

                product_item.Total = product_item.BussinessAfterCoupon + product_item.ForceAfterCoupon + product_item.TaxAfterCoupon;
                product_item.OriginalTotal = product_item.BizTotal + product_item.ForceTotal + product_item.TaxTotal;

                //product_item.ExpireDate
                //product_item.UserId =0 ; //外键对应user表的Id

                #endregion

                //
                userItem = new ProductService().SaveProduct(product_user, product_item);

            }
            return Json(userItem);
        }

        //订单点击过来，获取产品详情
        public ActionResult Detail(int? channel, string mobile, int? productId)
        {
            channel = channel ?? 1;
            productId = productId ?? 0;

            ViewBag.channel = channel;
            ViewBag.mobile = mobile;

            var data = new ProductService().GetProduct(productId.Value);

            return View(data);
        }
        #endregion
    }
}