using HONGLI.Entity;
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
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace HONGLI.Service
{
    public class PostOrderService
    {

        /* 放到配置文件去			
<add key="InsuranceApiUrl" value="http://m.91bihu.com/api/CommonInsuranceBusiness/"/>
<add key="ApiKey" value="socsecxyecsd"/>
<add key="ApiAgent" value="4712"/>  <!--本平台标识-->
*/
        static string API_URL = I.Utility.Util.GetConfigByKey("PostOrderApiUrl");
        static string API_AGENT = I.Utility.Util.GetConfigByKey("ApiAgent");
        static string API_SECCODE = I.Utility.Util.GetConfigByKey("ApiSeccode");

        /// <summary>
        /// 测试方法。
        /// </summary>
        /// <returns></returns>
        public static bool PostOrder()
        {
            bool flag = false;
            PostOrderModel model = new PostOrderModel();
            model.OrderNum = "1111111111111111";
            model.InsuredName = "卜雅艳";
            model.IdType = 1;
            model.IdNum = "511702197409284963";
            model.Buid = 1;
            model.Source = 1;
            model.Receipt = "1";
            model.PayType = 1;
            model.DistributionType = 1;
            model.DistributionAddress = "北京市海淀区中关村海龙大厦";
            model.DistributionTime = "2016-03-10";
            model.InsurancePrice = 8020.1;
            model.CarriagePrice = 20;
            model.TotalPrice = 8040.1;
            model.UserId = 12;
            model.Agent = 3712;

            flag = PostOrder(model);
            return flag;
        }


        public static bool PostOrder(PostOrderModel model)
        {
#if (DEBUG)
            string apiUrl = "http://i.91bihu.com/api/Order/PostOrder";
#else
            var apiUrl = API_URL;
#endif


            bool flag = false;
            var result = string.Empty;

            var secCode = "";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                dict["OrderNum"] = model.OrderNum.ToString();
                dict["InsuredName"] = model.InsuredName.ToString();
                dict["IdType"] = model.IdType.ToString();
                dict["IdNum"] = model.IdNum.ToString();
                dict["Buid"] = model.Buid.ToString();
                dict["Source"] = model.Source.ToString();
                dict["Receipt"] = model.Receipt.ToString();
                dict["PayType"] = model.PayType.ToString();
                dict["DistributionType"] = model.DistributionType.ToString();
                dict["DistributionAddress"] = model.DistributionAddress.ToString();
                dict["DistributionTime"] = model.DistributionTime.ToString();
                dict["InsurancePrice"] = model.InsurancePrice.ToString();
                dict["CarriagePrice"] = model.CarriagePrice.ToString();
                dict["TotalPrice"] = model.TotalPrice.ToString();
                dict["UserId"] = model.UserId.ToString();
                dict["Agent"] = model.Agent.ToString();

                var secCodes = String.Empty;
                foreach (var item in dict)
                {
                    secCodes += item.Key + "=" + item.Value + "&";
                }
                if (!String.IsNullOrEmpty(secCodes))
                {
                    secCodes = secCodes.Substring(0, secCodes.Length - 1);
                }
                // 加密参数

                secCode = GetSign(secCodes);
                dict["SecCode"] = secCode;

                var watch = new Stopwatch();
                watch.Start();
                result = DoHttpGet(apiUrl, dict);
                watch.Stop();

                //todo 测试上线后注释
                LogHelper.Info(string.Format("调用接口：{0}，参数为：OrderNum：{1}，InsuredName：{2}，IdType：{3}，IdNum：{4}，Buid，{5}，Receipt：{6}，PayType：{7}，DistributionType：{8}，DistributionAddress：{9}，DistributionTime：{10}，InsurancePrice：{11}，CarriagePrice：{12}，TotalPrice：{13}，Source：{14}，UserId：{15}，Agent：{16}，SecCode：{17}。耗时：{18}ms，返回结果：{19}", apiUrl, model.OrderNum, model.InsuredName, model.IdType, model.IdNum, model.Buid, model.Receipt, model.PayType, model.DistributionType, model.DistributionAddress, model.DistributionTime, model.InsurancePrice, model.CarriagePrice, model.TotalPrice, model.Source, model.UserId, model.Agent, secCode, watch.ElapsedMilliseconds, result));
            }
            catch (Exception ex)
            {
                LogHelper.Info(string.Format("调用接口：{0}，参数为：OrderNum：{1}，InsuredName：{2}，IdType：{3}，IdNum：{4}，Buid，{5}，Receipt：{6}，PayType：{7}，DistributionType：{8}，DistributionAddress：{9}，DistributionTime：{10}，InsurancePrice：{11}，CarriagePrice：{12}，TotalPrice：{13}，UserId：{14}，AgentId：{15}，SecCode：{16}。异常信息：：{17}", apiUrl, model.OrderNum, model.InsuredName, model.IdType, model.IdNum, model.Buid, model.Receipt, model.PayType, model.DistributionType, model.DistributionAddress, model.DistributionTime, model.InsurancePrice, model.CarriagePrice, model.TotalPrice, model.UserId, model.Agent, secCode, ex.Message + ex.StackTrace));
            }
            PostOrderResultData resultData = new PostOrderResultData();

            resultData = result.FromJsonTo<PostOrderResultData>();

            return flag;

        }

        /// <summary>
        /// 测试方法。
        /// </summary>
        /// <returns></returns>
        public static bool PostOrderStatus()
        {
            var flag = false;
            var model = new PostOrderStatusModel
            {
                OrderId = "1",
                OrderStatus = 1,
                AgentId = 3712
            };

            flag = PostOrderStatus(model);

            return flag;
        }


        public static bool PostOrderStatus(PostOrderStatusModel model)
        {
#if (DEBUG)
            var apiUrl = "http://i.91bihu.com/api/order/update";
#else
            var apiUrl = I.Utility.Util.GetConfigByKey("PostOrderStatusApiUrl");
#endif

            var flag = false;
            var result = string.Empty;

            var secCode = "";
            var dict = new Dictionary<string, string>();

            try
            {
                dict["OrderId"] = model.OrderId.ToString();
                dict["OrderStatus"] = model.OrderStatus.ToString();
                dict["AgentId"] = model.AgentId.ToString();

                var secCodes = string.Empty;
                foreach (var item in dict)
                {
                    secCodes += item.Key + "=" + item.Value + "&";
                }
                if (!String.IsNullOrEmpty(secCodes))
                {
                    secCodes = secCodes.Substring(0, secCodes.Length - 1);
                }
                // 加密参数

                secCode = GetSign(secCodes);
                dict["SecCode"] = secCode;

                var watch = new Stopwatch();
                watch.Start();
                result = DoHttpGet(apiUrl, dict);
                watch.Stop();

                //todo 测试上线后注释
                LogHelper.Info(string.Format("PostOrderStatus调用接口：{0}，参数为：OrderId：{1}，OrderStatus：{2}，AgentId：{3}，SecCode：{4}。耗时：{5}ms，返回结果：{6}", apiUrl, model.OrderId, model.OrderStatus, model.AgentId, model.SecCode, watch.ElapsedMilliseconds, result));
            }
            catch (Exception ex)
            {
                LogHelper.Info(string.Format("PostOrderStatus调用接口：{0}，参数为：OrderId：{1}，OrderStatus：{2}，AgentId：{3}，SecCode：{4}。耗时：{5}ms，返回结果：{6}", apiUrl, model.OrderId, model.OrderStatus, model.AgentId, model.SecCode, ex.Message + ex.StackTrace));
            }

            var resultData = new PostOrderResultData();

            resultData = result.FromJsonTo<PostOrderResultData>();

            return flag;
        }






















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

            //LogHelper.Info("请求91bihu version2接口-url:" + url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            //LogHelper.Info("请求91bihu version2接口-url_return:" + retString);

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
        #region DoPost 

        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求容器URL。</param>
        /// <param name="parameters">请求参数。</param>
        /// <returns>HTTP响应。</returns>
        public static string DoPost(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //req.AllowAutoRedirect = true;  //允许重定向
            req.Method = "POST";
            req.KeepAlive = true;
            req.UserAgent = "Top4Net";
            req.Timeout = 300000;
            //req.ContentType = "text/html;charset=UTF-8";
            req.ContentType = "application/json";

            byte[] postData = Encoding.UTF8.GetBytes(BuildPostData(parameters));
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

        public static string BuildPostData(IDictionary<string, string> parameters)
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

        #region GetSign 32位Md5加密，转小写   //todo 放到单独帮助类中

        public static string GetMd5(string message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                byte[] md5Bytes = md5.ComputeHash(bytes);
                foreach (byte item in md5Bytes)
                {
                    stringBuilder.Append(item.ToString("x2"));
                }
            }
            return stringBuilder.ToString();

        }

        public static string GetSign(string str)
        {
            var secretKey = I.Utility.Util.GetConfigByKey("ApiKey");
            var value = str + secretKey;
            var sign = GetMd5(value);

            return sign;
        }

        //public static string Sign(string str)
        //{
        //    var secretKey = I.Utility.Util.GetConfigByKey("ApiKey");
        //    var value = str + secretKey;
        //    var sign = Md5(value).ToLower();

        //    return sign;
        //}

        //public static string GetMd5(string str)
        //{
        //    string cl = str;
        //    string pwd = "";
        //    System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();//实例化一个md5对像
        //    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
        //    byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
        //    // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
        //    for (int i = 0; i < s.Length; i++)
        //    {
        //        // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符

        //        pwd = pwd + s[i].ToString("X");

        //    }
        //    return pwd;
        //}

        public static string Md5(string sourceText)
        {
            var md5String = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sourceText.Trim(), "MD5");
            return md5String;
        }

        #endregion

    }

    public class PostOrderStatusModel
    {


        public string OrderId { get; set; }


        public int OrderStatus { get; set; }

        public int AgentId { get; set; }

        public string SecCode { get; set; }

    }

    public class PostOrderModel
    {

        public PostOrderModel()
        {

        }

        /// <summary>
        /// 订单编号 
        /// </summary>
        public string OrderNum { get; set; }
        /// <summary>
        /// 被保险人
        /// </summary>
        public string InsuredName { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public int IdType { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string IdNum { get; set; }
        /// <summary>
        /// 试算id
        /// </summary>
        public long Buid { get; set; }
        /// <summary>
        /// 保险公司
        /// </summary>
        public int Source { get; set; }
        /// <summary>
        /// 发票
        /// </summary>
        public string Receipt { get; set; }
        /// <summary>
        /// 支付方式（1 在线支付 2 线下支付）
        /// </summary>
        public int PayType { get; set; }
        /// <summary>
        /// 配送方式（1 快递 2 上门自提）
        /// </summary>
        public int DistributionType { get; set; }
        /// <summary>
        /// 配送地址
        /// </summary>
        public string DistributionAddress { get; set; }
        /// <summary>
        /// 配送时间
        /// </summary>
        public string DistributionTime { get; set; }
        /// <summary>
        /// 保费
        /// </summary>
        public double InsurancePrice { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        public double CarriagePrice { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public double TotalPrice { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public double UserId { get; set; }
        /// <summary>
        /// 调用平台标示
        /// </summary>
        public int Agent { get; set; }
        /// <summary>
        /// 除了secCode参数之外的所有参数拼接后再加密钥的字符串后的MD5值（32位小写）（密钥请从壁虎研发处获取）
        /// </summary>
        public string SecCode { get; set; }
    }

    public class CreateOrderRequest
    {
        /// <summary>
        /// 定单号 
        /// </summary>
        [RegularExpression("^[0-9a-zA-Z]{15,32}$")]
        public string OrderNum { get; set; }

        /// <summary>
        /// 被保险人
        /// </summary>
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string InsuredName { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public int IdType { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        [RegularExpression("^[0-9a-zA-Z]{10,32}$")]
        public string IdNum { get; set; }
        /// <summary>
        /// 报价ID
        /// </summary>
        public long Buid { get; set; }
        /// <summary>
        /// 保险公司
        /// </summary>
        public int Source { get; set; }
        [StringLength(30, MinimumLength = 1)]
        public string Receipt { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public int PayType { get; set; }
        /// <summary>
        /// 配送方式
        /// </summary>
        public int DistributionType { get; set; }
        /// <summary>
        /// 配送地址
        /// </summary>
        public string DistributionAddress { get; set; }
        /// <summary>
        /// 送单时间
        /// </summary>
        public string DistributionTime { get; set; }
        /// <summary>
        /// 保费总额
        /// </summary>
        public double InsurancePrice { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        public double CarriagePrice { get; set; }
        /// <summary>
        /// 总额
        /// </summary>
        public double TotalPrice { get; set; }

        public int UserId { get; set; }
        /// <summary>
        /// 经纪人id
        /// </summary>
        [Range(1, 10000)]
        public int Agent { get; set; }
        /// <summary>
        /// 校验码
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }

    }
}