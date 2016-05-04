using HONGLI.Entity;
using HONGLI.Service;
using I.Utility.Extensions;
using I.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HONGLI.Web.Controllers
{
    public class ProductV4Controller : Controller
    {
        // GET: ProductV3
        #region page1-车险询价页 20160321
        public ActionResult Index(int? channel, string mobile)
        {
            var channelValue = channel.HasValue ? channel.Value : Convert.ToInt32(Channel.Wap);
            ViewBag.channel = channelValue;
            ViewBag.mobile = mobile;

            var loginUrl = I.Utility.Util.GetConfigByKey("loginUrl");
            var currentUrl = HttpContext.Request.Url;
            loginUrl = string.Format(loginUrl, currentUrl, mobile, channelValue);
            ViewBag.loginUrl = loginUrl;

            return View();
        }

        #endregion

        #region page2-填写信息页 20160321

#if (!DEBUG)
    [AuthorizationFilter]
#endif
        public ActionResult FillIn(int? channel, string mobile, string licenseNo, int? cityCode, int? isPublic)
        {
            var channelValue = channel.HasValue ? channel.Value : Convert.ToInt32(Channel.Wap);
            ViewBag.channel = channelValue;
            ViewBag.mobile = mobile;
            ViewBag.licenseNo = licenseNo;
            var cityCodeX = cityCode.HasValue ? cityCode.Value : 10;
            ViewBag.cityCode = cityCodeX;

            return View();
        }
        #endregion

        #region page2-ajax 获取用户续保信息 add by Lee 20160403

#if (!DEBUG)
    [AuthorizationFilter]
#endif
        public JsonResult GetReInfo(int? channel, string mobile, string licenseNo, int? cityCode, int? isPublic)
        {
            var channelValue = channel.HasValue ? channel.Value : Convert.ToInt32(Channel.Wap);
            ViewBag.channel = channelValue;
            ViewBag.mobile = mobile;
            ViewBag.licenseNo = licenseNo;
            var cityCodeX = cityCode.HasValue ? cityCode.Value : 10;
            ViewBag.cityCode = cityCodeX;

            //#if (!DEBUG)
            //            channelValue = UserViewModel.CurrentUser.Channel.HasValue? Convert.ToInt32(UserViewModel.CurrentUser.Channel.Value): Convert.ToInt32(Channel.Wap); 
            //            mobile = UserViewModel.CurrentUser.Mobile;
            //#endif

            var result = "";
            var data = new UserInsuranceInfoResultV2();

            /*缓存读取*/
            var key = string.Format("{0}/{1}/{2}/{3}", channelValue, mobile, licenseNo, "insurance");
            //定义用户id
            int userid = -1;
            try
            {
                //有cache读cache
                if (HttpContext.Cache.Get(key) != null)
                {
                    data = HttpContext.Cache.Get(key) as UserInsuranceInfoResultV2;
                }
                else
                {
                    HttpContext.Cache.Remove(key);
                    result = new ProductV3Service().GetReInfo(licenseNo, cityCodeX, isPublic ?? 0, channelValue, mobile);
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }

                    data = result.FromJsonTo<UserInsuranceInfoResultV2>();
                    if (data != null)
                    {
                        //获取续保信息成功
                        if (data.BusinessStatus == 1)
                        {
                            //HttpContext.Session[key] = data;
                            HttpContext.Cache.Insert(key, data);
                        }
                        else if (data.BusinessStatus == 2) //驾驶证信息需完善，这部分内容砍掉了
                        {
                            if (data.SaveQuote == null)
                                data.SaveQuote = new Quote();
                        }
                        else if (data.BusinessStatus == 3) // 获取用户信息成功，但获取续保信息失败
                        {
                            data.SaveQuote = new Quote();
                            //HttpContext.Session[key] = data;
                            HttpContext.Cache.Insert(key, data);
                        }
                        else
                        {
                            data.UserInfo = new Entity.User();
                            data.SaveQuote = new Quote();
                        }
                        if (data.BusinessStatus == 1 || data.BusinessStatus == 2 || data.BusinessStatus == 3)
                        {
                            var product_user = new ProductV2_User();
                            product_user.CarUsedType = data.UserInfo.CarUsedType;
                            product_user.LicenseNo = data.UserInfo.LicenseNo;
                            product_user.LicenseOwner = data.UserInfo.LicenseOwner;
                            product_user.PostedName = data.UserInfo.PostedName;
                            product_user.InsuredName = data.UserInfo.InsuredName;
                            product_user.PurchasePrice = Convert.ToDecimal(data.UserInfo.PurchasePrice);
                            product_user.IdType = data.UserInfo.IdType;
                            product_user.CredentislasNum = data.UserInfo.CredentislasNum.Trim();
                            product_user.CityCode = data.UserInfo.CityCode;
                            product_user.EngineNo = data.UserInfo.EngineNo;
                            product_user.ModleName = data.UserInfo.ModleName;
                            product_user.RegisterDate = data.UserInfo.RegisterDate;
                            product_user.CarVin = data.UserInfo.CarVin;
                            product_user.ForceExpireDate = data.UserInfo.ForceExpireDate;
                            product_user.BusinessExpireDate = data.UserInfo.BusinessExpireDate;
                            product_user.SeatCount = data.UserInfo.SeatCount;
                            product_user.Mobile = mobile;
                            product_user.Channel = (byte)channel;
                            product_user.CustomerKey = data.CustKey;
                            product_user.CreateTime = DateTime.Now;
                            product_user.SeatCount = data.UserInfo.SeatCount;
                            product_user.Creator = mobile;
                            userid = new ProductV3Service().SaveProductUser(product_user);
                            //续保信息
                            var productV2_renewal = new ProductV2_Renewal();
                            productV2_renewal.UserId = userid;
                            productV2_renewal.Source = data.SaveQuote.Source;
                            productV2_renewal.CheSun = Convert.ToDecimal(data.SaveQuote.CheSun);
                            productV2_renewal.SanZhe = Convert.ToDecimal(data.SaveQuote.SanZhe);
                            productV2_renewal.DaoQiang = Convert.ToDecimal(data.SaveQuote.DaoQiang);
                            productV2_renewal.SiJi = Convert.ToDecimal(data.SaveQuote.SiJi);
                            productV2_renewal.ChengKe = Convert.ToDecimal(data.SaveQuote.ChengKe);
                            productV2_renewal.BoLi = Convert.ToInt32(data.SaveQuote.BoLi);
                            productV2_renewal.BuJiMianCheSun = Convert.ToDecimal(data.SaveQuote.BuJiMianCheSun);
                            productV2_renewal.BuJiMianSanZhe = Convert.ToDecimal(data.SaveQuote.BuJiMianSanZhe);
                            productV2_renewal.BuJiMianDaoQiang = Convert.ToDecimal(data.SaveQuote.BuJiMianDaoQiang);
                            productV2_renewal.BuJiMianRenYuan = Convert.ToDecimal(data.SaveQuote.BuJiMianRenYuan);
                            productV2_renewal.BuJiMianFuJia = Convert.ToDecimal(data.SaveQuote.BuJiMianFuJia);
                            productV2_renewal.SheShui = Convert.ToDecimal(data.SaveQuote.SheShui);
                            productV2_renewal.CheDeng = Convert.ToDecimal(data.SaveQuote.CheDeng);
                            productV2_renewal.ZiRan = Convert.ToDecimal(data.SaveQuote.ZiRan);
                            int id = new ProductV3Service().SaveProductRenewal(productV2_renewal);
                            data.UserId = userid;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogHelper.AppError(string.Format("获取续保信息GetReInfo异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region page3-询价列表 20160321

#if (!DEBUG)
    [AuthorizationFilter]
#endif
        public ActionResult QuoteList(int? channel, string mobile, string licenseNo, string dataItem)
        {
            var channelValue = channel.HasValue ? channel.Value : Convert.ToInt32(Channel.Wap);
            ViewBag.channel = channelValue;
            ViewBag.mobile = mobile;
            ViewBag.licenseNo = licenseNo;

            var key = string.Format("{0}/{1}/{2}/{3}", channelValue, mobile, licenseNo, "dataItem");
            HttpContext.Cache.Remove(key);
            HttpContext.Cache.Insert(key, dataItem);
            HttpContext.Cache.Remove(string.Format("{0}/{1}/{2}/{3}/{4}", channelValue, mobile, licenseNo, 0, "price"));
            HttpContext.Cache.Remove(string.Format("{0}/{1}/{2}/{3}/{4}", channelValue, mobile, licenseNo, 1, "price"));
            HttpContext.Cache.Remove(string.Format("{0}/{1}/{2}/{3}/{4}", channelValue, mobile, licenseNo, 2, "price"));
            return View();
        }

        #region page3-ajax 三家报价不核保 by Lee 20160403

        public JsonResult GetPrecisePrice(int? channel, string mobile, string licenseNo, int? intentionCompany)
        {
            var channelValue = channel.HasValue ? channel.Value : Convert.ToInt32(Channel.Wap);
            ViewBag.channel = channelValue;
            ViewBag.mobile = mobile;
            ViewBag.licenseNo = licenseNo;

            var data = new UserPrecisePriceV2();

            try
            {
                var key = string.Format("{0}/{1}/{2}/{3}", channelValue, mobile, licenseNo, "insurance");
                var tempData = HttpContext.Cache.Get(key);
                if (tempData == null)
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                //入参的各种参数
                var dataItemkey = string.Format("{0}/{1}/{2}/{3}", channelValue, mobile, licenseNo, "dataItem");
                var tempDataItem = HttpContext.Cache.Get(dataItemkey);
                if (tempDataItem == null)
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                /*isSingleSubmit=0&intentionCompany =-1,三家只报价不核保;; isSingleSubmit=1&intentionCompany>-1,单独核保单独报价*/
                var intentionCompanyValue = intentionCompany.HasValue ? intentionCompany.Value : -1;
                var isSingleSubmit = intentionCompanyValue == -1 ? 0 : 1;

                var model = new UserPrecisePriceIncomingParamsModelV2();

                #region 入参用户信息
                var info = tempData as UserInsuranceInfoResultV2;
                model.LicenseNo = info.UserInfo.LicenseNo;
                model.CarType = 0;  //客车
                model.IsNewCar = string.IsNullOrWhiteSpace(info.UserInfo.ForceExpireDate) ? 1 : 0;  //是否新车，
                model.CarUsedType = string.IsNullOrWhiteSpace(info.UserInfo.CarUsedType) ? 1 : 0; //营运；非营运
                model.CityCode = info.UserInfo.CityCode;
                model.EngineNo = info.UserInfo.EngineNo;
                model.CarVin = info.UserInfo.CarVin;
                model.RegisterDate = info.UserInfo.RegisterDate;
                model.MoldName = info.UserInfo.ModleName;
                #endregion                

                model.IsSingleSubmit = isSingleSubmit;
                model.IntentionCompany = intentionCompanyValue;

                #region 入参各种类型选择
                var incomingParamModel = tempDataItem.ToString().FromJsonTo<Dictionary<string, object>>();
                model.ForceTax = 1;     //1.报价交强车船 0.不报价交强车船
                model.BoLi = Convert.ToDouble(incomingParamModel["boli"]);
                model.BuJiMianCheSun = Convert.ToDouble(incomingParamModel["bujimianchesun"]);
                model.BuJiMianDaoQiang = Convert.ToDouble(incomingParamModel["bujimiandaoqiang"]);
                model.BuJiMianFuJia = Convert.ToDouble(incomingParamModel["bujimianfujia"]);
                model.BuJiMianRenYuan = Convert.ToDouble(incomingParamModel["bujimianrenyuan"]);
                model.BuJiMianSanZhe = Convert.ToDouble(incomingParamModel["bujimiansanzhe"]);
                model.CheDeng = Convert.ToDouble(incomingParamModel["chedeng"]);
                model.ChengKe = Convert.ToDouble(incomingParamModel["chengke"]);
                model.CheSun = Convert.ToDouble(incomingParamModel["chesun"]);
                model.DaoQiang = Convert.ToDouble(incomingParamModel["daoqiang"]);
                model.HuaHen = Convert.ToDouble(incomingParamModel["huahen"]);
                model.SanZhe = Convert.ToDouble(incomingParamModel["sanzhe"]);
                model.SheShui = Convert.ToDouble(incomingParamModel["sheshui"]);
                model.SiJi = Convert.ToDouble(incomingParamModel["siji"]);
                model.ZiRan = Convert.ToDouble(incomingParamModel["ziran"]);
                model.CustKey = channelValue + mobile;
                model.CustKeyMd5 = new ProductV3Service().Md5(model.CustKey);

                #endregion

                var result = new ProductV3Service().PostPrecisePrice(model.ToJsonItem());       //方法名是post，实际上是get方式。

                if (string.IsNullOrWhiteSpace(result))
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                data = result.FromJsonTo<UserPrecisePriceV2>();

                //请求报价成功
                if (data != null && data.BusinessStatus == 1)
                {
                    var keyQuoteList = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", channelValue, mobile, licenseNo, isSingleSubmit, intentionCompany, "quoteList");
                    HttpContext.Session[keyQuoteList] = data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.AppError(string.Format("核保/报价信息GetPrecisePrice异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //获取报价接口、同时核保(同时插入数据)
#if (!DEBUG)
    [AuthorizationFilter]
#endif
        public JsonResult GetSpecialPrecisePrice(int? channel, string mobile, string licenseNo, int intentionCompany, int UserID, string check = "")
        {
            var channelValue = channel.HasValue ? channel.Value : Convert.ToInt32(Channel.Wap);
            ViewBag.channel = channelValue;
            ViewBag.mobile = mobile;
            ViewBag.licenseNo = licenseNo;
            var keyPrice = string.Format("{0}/{1}/{2}/{3}/{4}", channelValue, mobile, licenseNo, intentionCompany, "price");
            var data = new PrecisePriceResultV2();
            try
            {
                if (check == "")
                {
                    var cachresult = HttpContext.Session[keyPrice];
                    if (cachresult != null)
                    {
                        return Json(cachresult, JsonRequestBehavior.AllowGet);
                    }
                }
                var result = new ProductV3Service().GetSpecialPrecisePrice(licenseNo, intentionCompany, channelValue, mobile);

                if (string.IsNullOrWhiteSpace(result))
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                data = result.FromJsonTo<PrecisePriceResultV2>();

                //请求报价成功
                if (data != null && data.BusinessStatus == 1)
                {
                    #region 组合险种描述Description字段
                    if (data.Item != null)
                    {
                        var item = data.Item;
                        var description = "";
                        description += item.CheSun.BaoE > 0 ? (string.Format("车损险({0})、", FormatNumber(item.CheSun.BaoE))) : "";
                        description += item.BuJiMianCheSun.BaoE > 0 ? ("不计免赔车损险、") : "";
                        description += item.SanZhe.BaoE > 0 ? (string.Format("三者险({0})、", FormatNumber(item.SanZhe.BaoE))) : "";
                        description += item.BuJiMianSanZhe.BaoE > 0 ? ("不计免赔三者险、") : "";
                        description += item.DaoQiang.BaoE > 0 ? ("盗抢险、") : "";
                        description += item.BuJiMianDaoQiang.BaoE > 0 ? ("不计免赔盗抢险、") : "";
                        description += item.SiJi.BaoE > 0 ? (string.Format("座位险(司机{0})、", FormatNumber(item.SiJi.BaoE))) : "";
                        description += item.ChengKe.BaoE > 0 ? (string.Format("座位险(乘客{0})、", FormatNumber(item.ChengKe.BaoE))) : "";
                        description += item.BuJiMianRenYuan.BaoE > 0 ? ("不计免赔车上人员、") : "";
                        description += item.HuaHen.BaoE > 0 ? (string.Format("划痕险({0})、", FormatNumber(item.HuaHen.BaoE))) : "";
                        description += item.BoLi.BaoE > 0 ? (string.Format("玻璃单独破损险({0})、", BoLiType(item.BoLi.BaoE))) : "";
                        description += item.SheShui.BaoE > 0 ? ("发动机特别损失险、") : "";
                        description += item.ZiRan.BaoE > 0 ? ("自燃险、") : "";
                        if (intentionCompany == Convert.ToInt32(ProductCompany.PingAn)) //只有平安有此险种
                        {
                            description += item.CheDeng.BaoE > 0 ? ("倒车镜、车灯单独损失险") : "";
                        }
                        description += item.BuJiMianFuJia.BaoE > 0 ? ("不计免赔附加险、") : "";
                        data.Item.Description = description.TrimEnd('、');
                        #region 插入报价信息

                        var product_item = new ProductV2_Item();
                        result.MapToEntity<ProductV2_Item>(product_item);
                        product_item.UserId = UserID;
                        product_item.BoLi_BaoE = Convert.ToDecimal(data.Item.BoLi.BaoE);
                        product_item.BoLi_BaoFei = Convert.ToDecimal(data.Item.BoLi.BaoFei);
                        product_item.BuJiMianCheSun_BaoE = Convert.ToDecimal(data.Item.BuJiMianCheSun.BaoE);
                        product_item.BuJiMianCheSun_BaoFei = Convert.ToDecimal(data.Item.BuJiMianCheSun.BaoFei);
                        product_item.BuJiMianDaoQiang_BaoE = Convert.ToDecimal(data.Item.BuJiMianDaoQiang.BaoE);
                        product_item.BuJiMianDaoQiang_BaoFei = Convert.ToDecimal(data.Item.BuJiMianDaoQiang.BaoFei);
                        product_item.BuJiMianFuJia_BaoE = Convert.ToDecimal(data.Item.BuJiMianFuJia.BaoE);
                        product_item.BuJiMianFuJia_BaoFei = Convert.ToDecimal(data.Item.BuJiMianFuJia.BaoFei);
                        product_item.BuJiMianRenYuan_BaoE = Convert.ToDecimal(data.Item.BuJiMianRenYuan.BaoE);
                        product_item.BuJiMianRenYuan_BaoFei = Convert.ToDecimal(data.Item.BuJiMianRenYuan.BaoFei);
                        product_item.BuJiMianSanZhe_BaoE = Convert.ToDecimal(data.Item.BuJiMianSanZhe.BaoE);
                        product_item.BuJiMianSanZhe_BaoFei = Convert.ToDecimal(data.Item.BuJiMianSanZhe.BaoFei);
                        product_item.CheDeng_BaoE = Convert.ToDecimal(data.Item.CheDeng.BaoE);
                        product_item.CheDeng_BaoFei = Convert.ToDecimal(data.Item.CheDeng.BaoFei);
                        product_item.ChengKe_BaoE = Convert.ToDecimal(data.Item.ChengKe.BaoE);
                        product_item.ChengKe_BaoFei = Convert.ToDecimal(data.Item.ChengKe.BaoFei);
                        product_item.CheSun_BaoE = Convert.ToDecimal(data.Item.CheSun.BaoE);
                        product_item.CheSun_BaoFei = Convert.ToDecimal(data.Item.CheSun.BaoFei);
                        product_item.DaoQiang_BaoE = Convert.ToDecimal(data.Item.DaoQiang.BaoE);
                        product_item.DaoQiang_BaoFei = Convert.ToDecimal(data.Item.DaoQiang.BaoFei);
                        product_item.HuaHen_BaoE = Convert.ToDecimal(data.Item.HuaHen.BaoE);
                        product_item.HuaHen_BaoFei = Convert.ToDecimal(data.Item.HuaHen.BaoFei);
                        product_item.ZiRan_BaoE = Convert.ToDecimal(data.Item.ZiRan.BaoE);
                        product_item.ZiRan_BaoFei = Convert.ToDecimal(data.Item.ZiRan.BaoFei);
                        #region 平台自用费率，即用户的价格=总价*(1-保险公司费率+平台费率)
                        var rateList = new ProductV3Service().GetCouponRateList(channelValue, Convert.ToInt32(intentionCompany));
                        var key = string.Format("{0}_{1}_CouponRate", channelValue, intentionCompany);
                        var couponRate = rateList[key];

                        var forceRate_Channel = Convert.ToDouble(0);
                        var taxRate_Channel = Convert.ToDouble(0);
                        var bizRate_Channel = Convert.ToDouble(0);
                        if (couponRate != null)
                        {
                            var array = couponRate.Split(',').ToArray();
                            forceRate_Channel = Convert.ToDouble(array[0]);
                            taxRate_Channel = Convert.ToDouble(array[1]);
                            bizRate_Channel = Convert.ToDouble(array[2]);
                        }

                        product_item.ForceRate_Channel = Convert.ToDecimal(forceRate_Channel);
                        product_item.TaxRate_Channel = Convert.ToDecimal(taxRate_Channel);
                        product_item.BizRate_Channel = Convert.ToDecimal(bizRate_Channel);
                        #endregion

                        product_item.BizTotal = Convert.ToDecimal(data.Item.BizTotal);
                        product_item.BizRate = Convert.ToDecimal(data.Item.BizRate);
                        product_item.BizAfterCoupon = decimal.Round(Convert.ToDecimal(data.Item.BizTotal * ((100 - data.Item.BizRate + bizRate_Channel) * 0.01)), 2);

                        product_item.ForceTotal = Convert.ToDecimal(data.Item.ForceTotal);
                        product_item.ForceRate = Convert.ToDecimal(data.Item.ForceRate);
                        product_item.ForceAfterCoupon = decimal.Round(Convert.ToDecimal(data.Item.ForceTotal * ((100 - data.Item.ForceRate + forceRate_Channel) * 0.01)), 2);

                        product_item.TaxTotal = Convert.ToDecimal(data.Item.TaxTotal); ;
                        product_item.TaxRate = Convert.ToDecimal(data.Item.ForceRate); //核保后，交强险费率和车船税费率相同
                        product_item.TaxAfterCoupon = decimal.Round(Convert.ToDecimal(data.Item.TaxTotal * ((100 - data.Item.ForceRate + taxRate_Channel) * 0.01)), 2);

                        product_item.Total = product_item.BizTotal + product_item.ForceTotal + product_item.TaxTotal;
                        product_item.TotalAfterCoupon = decimal.Round(Convert.ToDecimal(product_item.BizAfterCoupon + product_item.ForceAfterCoupon + product_item.TaxAfterCoupon), 2);

                        #region 预付比例
                        var PrepaidAmountkey = string.Format("{0}_PrepaidAmount", channelValue);
                        var prepaidlist = new ProductV3Service().GetPrepaidAmount(channelValue);
                        decimal PrepaidAmount = Convert.ToDecimal(prepaidlist[PrepaidAmountkey]);
                        product_item.PrepaidAmount = product_item.TotalAfterCoupon * PrepaidAmount;
                        #endregion
                        product_item.CreateTime = DateTime.Now;

                        product_item.BuId = data.Item.BuId;    //续保buid

                        product_item.ProductName = new ProductV3Service().GetCompanyName(intentionCompany);  //产品名称（即保险公司名）
                        product_item.Source = intentionCompany;
                        product_item.QuoteStatus = data.BusinessStatus;
                        product_item.QuoteResult = data.StatusMessage;
                        product_item.Description = data.Item.Description;
                        data.Item.Id = new ProductV3Service().SaveProductItem(product_item);
                        #endregion
                        //进行核保，每家公司只核保一次，核保会会插入缓存
                        var keySubmit = string.Format("{0}/{1}/{2}/{3}/{4}", channelValue, mobile, licenseNo, intentionCompany, "submit");
                        HttpContext.Cache.Remove(keySubmit);//核保前清除缓存
                        //GetPrecisePrice(channel, mobile, licenseNo, intentionCompany);
                        //GetSubmitInfo(channel, mobile, licenseNo, intentionCompany, data.Item.Id);

                    }
                    else
                    {
                        data.Item = new PrecisePriceItem();
                    }
                    #endregion


                    HttpContext.Session[keyPrice] = data;
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.AppError(string.Format("单独报价GetSpecialPrecisePrice异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }

            return Json(data, JsonRequestBehavior.AllowGet);

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
        #endregion

        #region page4-报价详情 20160323
#if (!DEBUG)
    [AuthorizationFilter]
#endif
        public ActionResult QuoteDetail(int? channel, string mobile, string licenseNo, int intentionCompany)
        {
            var channelValue = channel.HasValue ? channel.Value : Convert.ToInt32(Channel.Wap);
            ViewBag.channel = channelValue;
            ViewBag.mobile = mobile;
            ViewBag.licenseNo = licenseNo;
            ViewBag.intentionCompany = intentionCompany;

            var submitStatus = Convert.ToInt32(SubmitStatus.UnCheck);
            var submitResult = "";
            var data = new PrecisePriceResultV2();

            try
            {
                #region 平台自用费率，即用户的价格=总价*(1-保险公司费率+平台费率)
                var rateList = new ProductV3Service().GetCouponRateList(channelValue, intentionCompany);
                var key = string.Format("{0}_{1}_CouponRate", channelValue, intentionCompany);
                var couponRate = rateList[key];

                var forceRate_Channel = Convert.ToDouble(0);
                var taxRate_Channel = Convert.ToDouble(0);
                var bizRate_Channel = Convert.ToDouble(0);
                if (couponRate != null)
                {
                    var array = couponRate.Split(',').ToArray();
                    forceRate_Channel = Convert.ToDouble(array[0]);
                    taxRate_Channel = Convert.ToDouble(array[1]);
                    bizRate_Channel = Convert.ToDouble(array[2]);
                }

                ViewBag.channel_ForceCouponRate = forceRate_Channel;
                ViewBag.channel_TaxCouponRate = taxRate_Channel;
                ViewBag.channel_BizCouponRate = bizRate_Channel;
                #endregion          
            }
            catch (Exception ex)
            {
                LogHelper.AppError(string.Format("ProductV2Controller.QuoteDetail异常，参数：channel={0},mobile={1},licenseNo={2},intentionCompany={3}。异常信息：{4}。",
                                   channelValue, mobile, licenseNo, intentionCompany, ex.Message + ex.StackTrace));
            }

            ViewBag.submitStatus = submitStatus;
            ViewBag.submitResult = submitResult;

            return View(data);
        }

        //获取核保信息接口（第2个接口是报价+核保接口，这个接口仅获取核保信息）同时同步核保数据
#if (!DEBUG)
    [AuthorizationFilter]
#endif
        public JsonResult GetSubmitInfo(int? channel, string mobile, string licenseNo, int intentionCompany, int ProductItemId)
        {
            var channelValue = channel.HasValue ? channel.Value : Convert.ToInt32(Channel.Wap);
            ViewBag.channel = channelValue;
            ViewBag.mobile = mobile;
            ViewBag.licenseNo = licenseNo;
            ViewBag.intentionCompany = intentionCompany;

            var data = new SubmitInfoResultV2();
            try
            {
                var keySubmit = string.Format("{0}/{1}/{2}/{3}/{4}", channelValue, mobile, licenseNo, intentionCompany, "submit");
                if (HttpContext.Cache.Get(keySubmit) != null)
                {
                    data = HttpContext.Cache.Get(keySubmit) as SubmitInfoResultV2;
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                HttpContext.Cache.Remove(keySubmit);
                var result = new ProductV3Service().GetSubmitInfo(licenseNo, intentionCompany, channelValue, mobile);

                if (string.IsNullOrWhiteSpace(result))
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                data = result.FromJsonTo<SubmitInfoResultV2>();

                //核保成功，成功再记cache
                if (data != null)
                {
                    //HttpContext.Session[keySubmit] = data;
                    HttpContext.Cache.Insert(keySubmit, data);
                }
                #region 同步核保数据
                var product_item = new ProductV2_Item();
                result.MapToEntity<ProductV2_Item>(product_item);
                product_item.Id = ProductItemId;
                product_item.SubmitStatus = (byte)(data.Item.SubmitStatus);
                product_item.SubmitResult = data.Item.SubmitResult;
                product_item.BizNo = data.Item.BizNo;
                product_item.ForceNo = data.Item.ForceNo;
                new ProductV3Service().Editproductitem(product_item);
                #endregion
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.AppError(string.Format("核保GetSubmitInfo异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //立即购买
#if (!DEBUG)
    [AuthorizationFilter]
#endif
        public JsonResult Buy(int? channel, string mobile, string licenseNo, string intentionCompany, int userID, int ProductItemId)
        {

            View_ProductV2UserItem userItem = null;
            try
            {
                var keyPrice = string.Format("{0}/{1}/{2}/{3}/{4}", channel, mobile, licenseNo, intentionCompany, "price");
                var data = new PrecisePriceResultV2();
                data= HttpContext.Session[keyPrice] as PrecisePriceResultV2;
                //修改用户选择购买公司
                new ProductV3Service().EditProductItemUserCheck(ProductItemId, userID);
                //增加订单信息
                string ordercode = "100001" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                Order_Item order_item = new Order_Item();
                order_item.OrderCode = ordercode;
                order_item.CreateDate = DateTime.Now;
                order_item.ProductId = ProductItemId.ToString();
                order_item.ProductName = new ProductV3Service().GetCompanyName(intentionCompany == "" ? 0 : Convert.ToInt32(intentionCompany));
                order_item.InsuranceLogo = intentionCompany == "" ? 0 : Convert.ToInt32(intentionCompany);
                order_item.LicenseNo = licenseNo;
                order_item.Channel = channel;
                if(!string.IsNullOrEmpty(data.UserInfo.BusinessExpireDate))
                {
                    order_item.BusinessExpireDate = Convert.ToDateTime(data.UserInfo.BusinessExpireDate);
                }
                if (!string.IsNullOrEmpty(data.UserInfo.ForceExpireDate))
                {
                    order_item.BusinessExpireDate = Convert.ToDateTime(data.UserInfo.ForceExpireDate);
                }
                #region 平台自用费率，即用户的价格=总价*(1-保险公司费率+平台费率)
                var rateList = new ProductV3Service().GetCouponRateList(Convert.ToInt32(channel), Convert.ToInt32(intentionCompany));
                var key = string.Format("{0}_{1}_CouponRate", channel, intentionCompany);
                var couponRate = rateList[key];

                var forceRate_Channel = Convert.ToDouble(0);
                var taxRate_Channel = Convert.ToDouble(0);
                var bizRate_Channel = Convert.ToDouble(0);
                if (couponRate != null)
                {
                    var array = couponRate.Split(',').ToArray();
                    forceRate_Channel = Convert.ToDouble(array[0]);
                    taxRate_Channel = Convert.ToDouble(array[1]);
                    bizRate_Channel = Convert.ToDouble(array[2]);
                }

                ViewBag.channel_ForceCouponRate = forceRate_Channel;
                ViewBag.channel_TaxCouponRate = taxRate_Channel;
                ViewBag.channel_BizCouponRate = bizRate_Channel;
                #endregion          
                order_item.ProductDealPrice =decimal.Round(Convert.ToDecimal(data.Item.BizTotal* ((100 - data.Item.BizRate + bizRate_Channel) * 0.01)),2)+
                decimal.Round(Convert.ToDecimal(data.Item.TaxTotal * ((100 - data.Item.ForceRate + taxRate_Channel) * 0.01)), 2)+
                decimal.Round(Convert.ToDecimal(data.Item.ForceTotal * ((100 - data.Item.ForceRate + forceRate_Channel) * 0.01)), 2);
                order_item.ProductOriginalPrice = decimal.Round(Convert.ToDecimal(data.Item.BizTotal+data.Item.TaxTotal+data.Item.ForceTotal),2);
                order_item.ProductTitle =data.Item.Description;
                order_item.Buid =data.Item.BuId;
                Order_Base order_base = new Order_Base();
                order_base.OrderCode = ordercode;
                order_base.Channel = channel.HasValue ? channel.Value : Convert.ToInt32(Channel.Wap);
                order_base.CreateDate = DateTime.Now;
                order_base.UserId = userID.ToString();
                order_base.Order_Item = new List<Order_Item>() { order_item };
                var code = new OrderService().AddOrder(order_base);
                userItem = new ProductV3Service().GetProductV2(userID);
                Dictionary<object, object> List = new Dictionary<object, object>();
                List.Add("ItemId", ProductItemId);
                List.Add("mobeil", mobile);
                List.Add("channel", channel);
                List.Add("source", intentionCompany);
                List.Add("OrderBaeOrderCode", order_base.OrderCode);
                List.Add("OrderBaseId", order_base.Id);
                List.Add("OrderItemId", order_item.Id);
                List.Add("PolicyId", 0);
                List.Add("OrderDeliverId", 0);
                return Json(List);
            }
            catch (Exception ex)
            {
                LogHelper.AppError(string.Format("购买产品Buy异常，异常信息：{0}，异常跟踪：{1}.参数：channel={}", ex.Message, ex.StackTrace));
            }

            return Json(userItem);
        }

        //订单点击过来，获取产品详情
#if (!DEBUG)
    [AuthorizationFilter]
#endif
        public ActionResult Detail(int? channel, string mobile, int? productId)
        {
            var channelValue = channel.HasValue ? channel.Value : Convert.ToInt32(Channel.Wap);
            ViewBag.channel = channelValue;
            ViewBag.mobile = mobile;
            ViewBag.productId = productId ?? 0;

            var data = new ProductV3Service().GetProductV2(productId.Value);

            return View(data);
        }
        #endregion
    }
}