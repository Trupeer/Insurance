using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using I.Utility.Extensions;
using System.Net;
using System.IO;
using I.Utility.Helper;
using System.Diagnostics;
using HONGLI.Entity;
using HONGLI.Repository;

namespace HONGLI.Service
{
    public class ProductService
    {

        #region 产品-与db相关的方法
        //保存
        public View_ProductUserItem SaveProduct(Product_User product_user, Product_Item product_item)
        {
            var userItem = new View_ProductUserItem();

            try
            {
                userItem = new ProductRepository().SaveProduct(product_user, product_item);
            }
            catch (Exception ex)
            {
                //todo log
                LogHelper.AppError(string.Format("保存产品详细SaveProduct异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }

            return userItem;
        }

        //获取
        public View_ProductUserItem GetProduct(int productId)
        {
            var result = new View_ProductUserItem();

            try
            {
                result = new ProductRepository().GetProduct(productId);
            }
            catch (Exception ex)
            {
                //todo log
                LogHelper.AppError(string.Format("获取产品详细GetProduct异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }

            return result;
        }

        #endregion

        #region 产品-调91bihu接口相关的方法

        static string API_URL = I.Utility.Util.GetConfigByKey("InsuranceApiUrl");
        static string API_AGENT = I.Utility.Util.GetConfigByKey("ApiAgent");
        static string API_SECCODE = I.Utility.Util.GetConfigByKey("ApiSeccode");

        #region 接口1-GetReInsuranceInfo-获取用户续保信息
        /// <summary>
        /// 获取用户续保信息
        /// </summary>
        /// <param name="lastYearCompany"></param>
        /// <param name="mobile"></param>
        /// <param name="carLicense"></param>
        /// <param name="userIdentity">本平台调用传空值，且该值不参与sign</param>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public string GetReInsuranceInfo(string lastYearCompany, string mobile, string carLicense, string userIdentity, string cityCode)
        {
            var result = string.Empty;
            var apiUrl = API_URL + "GetReInsuranceInfo";
            var secCode = "";
            try
            {
                #region request params

                var dict = new Dictionary<string, string>();
                dict["lastYearCompany"] = lastYearCompany;
                dict["mobile"] = mobile;
                dict["carLicense"] = carLicense;
                dict["userIdentity"] = "";
                dict["citycode"] = cityCode;
                dict["agent"] = API_AGENT;
                secCode = GetSign(lastYearCompany + mobile + carLicense + cityCode + API_AGENT);
                dict["secCode"] = secCode;

                #endregion

                var watch = new Stopwatch();
                watch.Start();
                result = this.DoHttpGet(apiUrl, dict);
                watch.Stop();

                //todo 测试上线后注释
                LogHelper.Info(string.Format("调用接口1-{0}，参数为：lastYearCompany:{1},mobile:{2},carLicense:{3},userIdentity:{4},citycode:{5},agent:{6},secCode:{7}。耗时：{8}ms",
                                   apiUrl, lastYearCompany, mobile, carLicense, userIdentity, cityCode, API_AGENT, secCode, watch.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                LogHelper.AppError(string.Format("调用接口1-{0}异常，参数为：lastYearCompany:{1},mobile:{2},carLicense:{3},userIdentity:{4},citycode:{5},agent:{6},secCode:{7}。异常信息：{8}",
                                   apiUrl, lastYearCompany, mobile, carLicense, userIdentity, cityCode, API_AGENT, secCode, ex.Message + ex.StackTrace));
            }
            return result;
        }
        #endregion

        #region 接口2-GetPrecisePrice-获取报价信息
        public string GetPrecisePrice(string jsonData)
        {
            var result = string.Empty;
            var apiUrl = API_URL + "GetPrecisePrice";
            var secCode = "";

            var model = jsonData.FromJsonTo<UserPrecisePriceInComingParamModel>();

            try
            {
                #region request params

                var intentionCompany = model.intentionCompany;
                var mobile = model.mobile;
                var carLicense = model.carLicense;
                var cityCode = model.citycode;
                                
                var dict = new Dictionary<string, string>();
                dict["mobile"] = mobile;
                dict["carLicense"] = carLicense;
                dict["intentionCompany"] = intentionCompany;
                dict["userIdentity"] = model.userIdentity;
                dict["carType"] = model.carType.ToString();
                dict["useType"] = model.useType.ToString();
                dict["isNewCar"] = model.isNewCar.ToString();
                dict["citycode"] = cityCode;
                dict["engineNo"] = model.engineNo;
                dict["carVin"] = model.carVin;
                dict["registerDate"] = model.registerDate;
                dict["moldName"] = model.moldName;
                dict["boli"] = model.boli.ToString();
                dict["bujimianchesun"] = model.bujimianchesun.ToString();
                dict["bujimiandaoqiang"] = model.bujimiandaoqiang.ToString();
                dict["bujimianfujia"] = model.bujimianfujia.ToString();
                dict["bujimianrenyuan"] = model.bujimianrenyuan.ToString();
                dict["bujimiansanzhe"] = model.bujimiansanzhe.ToString();
                dict["chedeng"] = model.chedeng.ToString();
                dict["sheshui"] = model.sheshui.ToString();
                dict["huahen"] = model.huahen.ToString();
                dict["siji"] = model.siji.ToString();
                dict["chengke"] = model.chengke.ToString();
                dict["chesun"] = model.chesun.ToString();
                dict["daoqiang"] = model.daoqiang.ToString();
                dict["sanzhe"] = model.sanzhe.ToString();
                dict["ziran"] = model.ziran.ToString();
                dict["agent"] = API_AGENT;
                secCode = GetSign(intentionCompany + mobile + carLicense + cityCode + API_AGENT);
                dict["secCode"] = secCode;

                #endregion

                var watch = new Stopwatch();
                watch.Start();
                result = this.DoHttpGet(apiUrl, dict);
                watch.Stop();

                //todo 测试上线后注释
                LogHelper.Info(string.Format("调用接口2-{0}，参数为：{1}。耗时：{2}ms",
                                   apiUrl, model.ToJsonItem(), watch.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                LogHelper.AppError(string.Format("调用接口2-{0}异常，参数为：{1}。异常信息：{2}",
                                   apiUrl, model.ToJsonItem(), ex.Message + ex.StackTrace));
            }
            return result;
        }
        #endregion
        


        #region GetSign 32位Md5加密，转小写   //todo 放到单独帮助类中

        public string GetSign(string str)
        {
            var secretKey = I.Utility.Util.GetConfigByKey("ApiKey");
            var value = str + secretKey;
            var sign = Md5(value).ToLower();

            return sign;
        }

        public string Md5(string sourceText)
        {
            var md5String = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sourceText.Trim(), "MD5");
            return md5String;
        }

        #endregion

        #region DoHttpGet 模拟Get请求   //todo 放到单独帮助类中

        public string DoHttpGet(string url, IDictionary<string, string> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + BuildPostData(parameters);
                }
                else
                {
                    url = url + "?" + BuildPostData(parameters);
                }
            }

            LogHelper.Info("请求91bihu接口-url:" + url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            LogHelper.Info("请求91bihu接口-url_return:" + retString);

            return retString;
        }

        public string BuildPostData(IDictionary<string, string> parameters)
        {
            var builder = new StringBuilder();
            var flag = false;
            var enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                var key = current.Key;
                var pair2 = enumerator.Current;
                var str2 = pair2.Value;
                if (!string.IsNullOrEmpty(key))// && !string.IsNullOrEmpty(str2))
                {
                    if (flag)
                    {
                        builder.Append("&");
                    }
                    builder.Append(key);
                    builder.Append("=");
                    //builder.Append(Uri.EscapeDataString(str2));
                    builder.Append(str2);
                    flag = true;
                }
            }
            return builder.ToString();
        }
        #endregion

        #endregion
    }

}
