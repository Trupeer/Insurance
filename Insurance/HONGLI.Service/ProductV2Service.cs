using I.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using I.Utility.Extensions;
using HONGLI.Entity;
using HONGLI.Repository;
using System.Configuration;

namespace HONGLI.Service
{
    /// <summary>
    /// Version2
    /// </summary>
    public class ProductV2Service
    {

        #region 产品-与db相关的方法
        //保存
        public View_ProductV2UserItem SaveProductV2(ProductV2_User product_user, ProductV2_Item product_item)
        {
            var userItem = new View_ProductV2UserItem();

            try
            {
                userItem = new ProductV2Repository().SaveProduct(product_user, product_item);
            }
            catch (Exception ex)
            {
                //todo log
                LogHelper.AppError(string.Format("保存产品详细SaveProductV2异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }

            return userItem;
        }

        //获取
        public View_ProductV2UserItem GetProductV2(int productId)
        {
            var result = new View_ProductV2UserItem();

            try
            {
                result = new ProductV2Repository().GetProductV2(productId);
            }
            catch (Exception ex)
            {
                //todo log
                LogHelper.AppError(string.Format("获取产品详细GetProductV2异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }

            return result;
        }

        #endregion

        #region 91bihu第二版接口 Version2 20160321起调整

        static string API_URL = I.Utility.Util.GetConfigByKey("Api_Url");
        static string API_KEY = I.Utility.Util.GetConfigByKey("Api_key");
        static string API_AGENT = I.Utility.Util.GetConfigByKey("Api_Agent");

        #region 接口1-getreinfo-获取用户续保信息
        /// <summary>
        /// 获取用户续保信息
        /// by Lee 20160321
        /// </summary>
        /// <param name="licenseNo">车牌号</param>
        /// <param name="cityCode">城市Id</param>
        /// <param name="isPublic">是否公车：0-非（默认）；1-是</param>
        /// <returns></returns>
        public string GetReInfo(string licenseNo, int cityCode, int isPublic, int channel, string mobile)
        {
            var result = string.Empty;
            var apiUrl = API_URL + "getreinfo";
            var custKey = "";
            var custKeyMd5 = "";
            var secCode = "";
            try
            {
                #region request params

                var dict = new Dictionary<string, string>();
                dict["LicenseNo"] = licenseNo.ToUpper();    //大写
                dict["CityCode"] = cityCode.ToString();
                dict["Agent"] = API_AGENT;
                dict["IsPublic"] = isPublic.ToString();
                custKey = channel + mobile;
                custKeyMd5 = Md5(custKey);
                dict["CustKey"] = custKeyMd5;
                secCode = GetSign(dict);
                dict["SecCode"] = secCode;

                #endregion

                var watch = new Stopwatch();
                watch.Start();
                result = DoHttpGet(apiUrl, dict);
                watch.Stop();

                //todo 测试上线后注释
                LogHelper.Info(string.Format("调用bihu v2接口1-{0}，参数为：LicenseNo:{1},CityCode:{2},Agent:{3},IsPublic:{4},CustKey:{5},SecCode:{6}。CustKeyMd5前：{7}。返回值：{8}。耗时：{9}ms。",
                                   apiUrl, licenseNo, cityCode, API_AGENT, isPublic, custKeyMd5, secCode, custKey, result, watch.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                LogHelper.AppError(string.Format("调用bihu v2接口1-{0}异常，参数为：LicenseNo:{1},CityCode:{2},Agent:{3},IsPublic:{4},CustKey:{5},SecCode:{6}。CustKeyMd5前：{7}。返回值：{8}。异常信息:{9}。",
                                   apiUrl, licenseNo, cityCode, API_AGENT, isPublic, custKeyMd5, secCode, custKey, result, ex.Message + ex.StackTrace));
            }
            return result;
        }
        #endregion

        #region 接口2-PostPrecisePrice-获取报价/核保信息
        /// <summary>
        /// by Lee 20160322   testdemo 京NR4923
        /// </summary>
        /// <param name="postDataJson"></param>
        /// <returns></returns>
        public string PostPrecisePrice(string postDataJson)
        {
            var result = string.Empty;
            var apiUrl = API_URL + "PostPrecisePrice";
            var secCode = "";

            var model = postDataJson.FromJsonTo<UserPrecisePriceIncomingParamsModelV2>();
            try
            {
                #region request params 生成SecCode

                var dict = new Dictionary<string, string>();
                dict["LicenseNo"] = model.LicenseNo.ToUpper();    //大写
                dict["IsSingleSubmit"] = model.IsSingleSubmit.ToString();
                dict["IntentionCompany"] = model.IntentionCompany.ToString();
                dict["CarType"] = model.CarType.ToString();
                dict["IsNewCar"] = model.IsNewCar.ToString();
                dict["CarUsedType"] = model.CarUsedType.ToString();
                dict["CityCode"] = model.CityCode.ToString();
                dict["EngineNo"] = model.EngineNo ?? "";
                dict["CarVin"] = model.CarVin ?? "";
                dict["RegisterDate"] = model.RegisterDate;
                dict["MoldName"] = model.MoldName ?? "";
                dict["ForceTax"] = model.ForceTax.ToString();
                dict["BoLi"] = model.BoLi.ToString();
                dict["BuJiMianCheSun"] = model.BuJiMianCheSun.ToString();
                dict["BuJiMianDaoQiang"] = model.BuJiMianDaoQiang.ToString();
                dict["BuJiMianFuJia"] = model.BuJiMianFuJia.ToString();
                dict["BuJiMianRenYuan"] = model.BuJiMianRenYuan.ToString();
                dict["BuJiMianSanZhe"] = model.BuJiMianSanZhe.ToString();
                dict["CheDeng"] = model.CheDeng.ToString();
                dict["SheShui"] = model.SheShui.ToString();
                dict["HuaHen"] = model.HuaHen.ToString();
                dict["SiJi"] = model.SiJi.ToString();
                dict["ChengKe"] = model.ChengKe.ToString();
                dict["CheSun"] = model.CheSun.ToString();
                dict["DaoQiang"] = model.DaoQiang.ToString();
                dict["SanZhe"] = model.SanZhe.ToString();
                dict["ZiRan"] = model.ZiRan.ToString();
                dict["CustKey"] = model.CustKeyMd5;
                dict["Agent"] = API_AGENT;

                secCode = GetSign(dict);
                dict["SecCode"] = secCode;

                #endregion

                var watch = new Stopwatch();
                watch.Start();
                result = DoHttpGet(apiUrl, dict); // DoHttpPost(apiUrl, model.ToJsonItem());
                watch.Stop();

                //todo 测试上线后注释
                LogHelper.Info(string.Format("调用bihu v2接口2-{0}，参数为：postDataJson:{1},secCode:{2}。返回值：{3}。耗时：{4}ms。",
                                   apiUrl, postDataJson, secCode, result, watch.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                LogHelper.AppError(string.Format("调用bihu v2接口2-{0}异常，参数为：postDataJson:{1},secCode:{2}。返回值：{3}。异常信息：{4}。",
                                   apiUrl, postDataJson, secCode, result, ex.Message + ex.StackTrace));
            }
            return result;
        }

        #endregion

        #region 接口3-GetSpecialPrecisePrice-获取单个公司报价信息
        public string GetSpecialPrecisePrice(string licenseNo, int intentionCompany, int channel, string mobile)
        {
            var result = string.Empty;
            var apiUrl = API_URL + "GetSpecialPrecisePrice";
            var custKey = "";
            var custKeyMd5 = "";
            var secCode = "";
            try
            {
                #region request params

                var dict = new Dictionary<string, string>();
                dict["LicenseNo"] = licenseNo.ToUpper();    //大写
                dict["IntentionCompany"] = intentionCompany.ToString();
                custKey = channel + mobile;
                custKeyMd5 = Md5(custKey);
                dict["CustKey"] = custKeyMd5;
                dict["Agent"] = API_AGENT;
                secCode = GetSign(dict);
                dict["SecCode"] = secCode;

                #endregion

                var watch = new Stopwatch();
                watch.Start();
                result = DoHttpGet(apiUrl, dict);
                watch.Stop();

                //todo 测试上线后注释
                LogHelper.Info(string.Format("调用bihu v2接口3-{0}，参数为：LicenseNo:{1},IntentionCompany:{2},CustKey:{3},Agent:{4},SecCode:{5}。CustKeyMd5前：{6}。返回值：{7}。耗时：{8}ms。",
                                   apiUrl, licenseNo, intentionCompany, custKeyMd5, API_AGENT, secCode, custKey, result, watch.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                LogHelper.AppError(string.Format("调用bihu v2接口3-{0}异常，参数为：LicenseNo:{1},IntentionCompany:{2},CustKey:{3},Agent:{4},SecCode:{5}。CustKeyMd5前：{6}。返回值：{7}。异常信息:{8}。",
                                   apiUrl, licenseNo, intentionCompany, custKeyMd5, API_AGENT, secCode, custKey, result, ex.Message + ex.StackTrace));
            }
            return result;
        }
        #endregion

        #region 接口4-GetSubmitInfo-获取单个公司核保信息

        public string GetSubmitInfo(string licenseNo, int intentionCompany, int channel, string mobile)
        {
            var result = string.Empty;
            var apiUrl = API_URL + "GetSubmitInfo";
            var custKey = "";
            var custKeyMd5 = "";
            var secCode = "";
            try
            {
                #region request params

                var dict = new Dictionary<string, string>();
                dict["LicenseNo"] = licenseNo.ToUpper();    //大写
                dict["IntentionCompany"] = intentionCompany.ToString();
                custKey = channel + mobile;
                custKeyMd5 = Md5(custKey);
                dict["CustKey"] = custKeyMd5;
                dict["Agent"] = API_AGENT;
                secCode = GetSign(dict);
                dict["SecCode"] = secCode;

                #endregion

                var watch = new Stopwatch();
                watch.Start();
                result = DoHttpGet(apiUrl, dict);
                watch.Stop();

                //todo 测试上线后注释
                LogHelper.Info(string.Format("调用bihu v2接口4-{0}，参数为：LicenseNo:{1},IntentionCompany:{2},CustKey:{3},Agent:{4},SecCode:{5}。CustKeyMd5前：{6}。返回值：{7}。耗时：{8}ms。",
                                   apiUrl, licenseNo, intentionCompany, custKeyMd5, API_AGENT, secCode, custKey, result, watch.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                LogHelper.AppError(string.Format("调用bihu v2接口4-{0}异常，参数为：LicenseNo:{1},IntentionCompany:{2},CustKey:{3},Agent:{4},SecCode:{5}。CustKeyMd5前：{6}。返回值：{7}。异常信息:{8}。",
                                   apiUrl, licenseNo, intentionCompany, custKeyMd5, API_AGENT, secCode, custKey, result, ex.Message + ex.StackTrace));
            }
            return result;
        }

        #endregion

        #region GetSign 32位Md5加密，转小写   //todo 放到单独帮助类中

        public string GetSign(Dictionary<string, string> parameters)
        {
            var paramString = BuildRequestData(parameters);
            var value = paramString + API_KEY;
            var sign = Md5(value).ToLower();

            return sign;
        }

        //Md5方法
        public string Md5(string sourceText)
        {
            var md5String = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sourceText.Trim(), "MD5");
            return md5String;
        }

        #endregion

        #region DoHttpGet 模拟Get请求   //todo 放到单独帮助类中

        public static string DoHttpGet(string url, IDictionary<string, string> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + BuildRequestData(parameters);
                }
                else
                {
                    url = url + "?" + BuildRequestData(parameters);
                }
            }

            LogHelper.Info("请求91bihu version2接口-url:" + url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            LogHelper.Info("请求91bihu version2接口-url_return:" + retString);

            return retString;
        }

        public static string BuildRequestData(IDictionary<string, string> parameters)
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

        #region DoHttpPost 模拟Post请求  //todo 放到单独帮助类中
        public static string DoHttpPost(string url, string jsonData)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //req.AllowAutoRedirect = true;  //允许重定向
            req.Method = "POST";
            req.KeepAlive = true;
            req.UserAgent = "Top4Net";
            req.Timeout = 300000;
            req.ContentType = "application/json"; //post json串 modify by Lee 20160323

            byte[] postData = Encoding.UTF8.GetBytes(jsonData);
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            if (rsp.CharacterSet != null)
            {
                Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                return GetResponseAsString(rsp, encoding);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string DoHttpPost(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //req.AllowAutoRedirect = true;  //允许重定向
            req.Method = "POST";
            req.KeepAlive = true;
            req.UserAgent = "Top4Net";
            req.Timeout = 300000;
            req.ContentType = "text/html;charset=UTF-8";

            byte[] postData = Encoding.UTF8.GetBytes(BuildRequestData(parameters));
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            if (rsp.CharacterSet != null)
            {
                Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                return GetResponseAsString(rsp, encoding);
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        private static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            StringBuilder result = new StringBuilder();
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                //rsp.Close();
                reader = new StreamReader(stream, encoding);

                // 每次读取不大于256个字符，并写入字符串
                char[] buffer = new char[256];
                int readBytes = 0;
                while ((readBytes = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    result.Append(buffer, 0, readBytes);
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.Timeout)
                {
                    result = new StringBuilder();
                }
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }

            return result.ToString();
        }

        #endregion

        #endregion

        #region channel1各公司费率,三个数分别对应force，tax，biz费率;0：平安 1：太平洋 2：人保  by Lee 20160329

        public Dictionary<string, string> GetCouponRateList(int channel, int productCompany)
        {
            var dict = new Dictionary<string, string>();
            if (channel == -1 || productCompany == -1)
            {
                return dict;
            }

            try
            {
                var configItems = ConfigurationManager.GetSection("CouponRateConfig") as List<ConfigItem>;
                foreach (var item in configItems)
                {
                    if (item.Value.IsNull())
                    {
                        LogHelper.AppError(item.Key + "配置项，没有配置CouponRate数据！");
                        continue;
                    }

                    dict.Add(item.Key, item.Value);
                }

                return dict;
            }
            catch (Exception ex)
            {
                LogHelper.AppError(string.Format("获取费率配置GetCouponRateList异常，确认config是否有CouponRateConfig配置节。异常信息：{0}.", ex.Message + ex.StackTrace));
            }
            return dict;
        }

        #endregion

        #region 获取保险公司名称 by Lee 20160401

        public string GetCompanyName(int companyId)
        {
            var companyName = "无匹配项";

            if (companyId== Convert.ToInt32(ProductCompany.PingAn))
            {
                companyName = "平安";
            }
            else if (companyId == Convert.ToInt32(ProductCompany.TaiPingYang))
            {
                companyName = "太平洋";
            }
            else if (companyId == Convert.ToInt32(ProductCompany.RenBao))
            {
                companyName = "人保";
            }

            return companyName;
        }

        #endregion
    }
}
