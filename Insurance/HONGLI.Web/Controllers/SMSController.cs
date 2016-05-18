using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

using I.Utility.Extensions;
using I.Utility.Helper;

using HONGLI.Entity;

namespace HONGLI.Web.Controllers
{
    public class SMSController : Controller
    {
        // GET: SMS
        public JsonResult Index()
        {
            var result = new JsonResult();
            var method = Request.Form["method"];

            switch (method.ToLower())
            {
                case "checksms":
                    result = this.CheckSMS();

                    break;
                case "sendsms":
                    result = this.SendSMS();

                    break;
                default:
                    var message = string.Format("{0}.{1}方法不存在", "SMSController", method);

                    LogHelper.Info(message);

                    result = Json(new ResultData() { State = 0, Error = message, Data = null });

                    break;
            }

            return result;
        }

        JsonResult CheckSMS()
        {
            var returnValue = new JsonResult();
            var resultData = new ResultData();

            LogHelper.Info(this.Request.RawUrl);

            try
            {
                var mobile = this.Request.Form["mobile"];
                var sms = this.Request.Form["sms"];
                var channel = this.Request.Form["channel"];

                var compare = false;
                var result = 0;
                var _key = "YiDao-SendSMS-Mobile-" + mobile;
                try
                {
                    var sendMessage = this.HttpContext.Cache.Get(_key);
                    if (sendMessage != null)
                    {
                        compare = sms.ToString() == sendMessage.ToString();
                        if (compare)
                            result = this.LoginOrRegist(mobile, sendMessage.ToString(), sms, channel);
                    }
                }
                catch (Exception error)
                {
                    LogHelper.AppError(error.Message);
                }

                var content = compare ? result.ToString() : "0";
                var logInfo = "interface checkSMS --- mobile : {0} ,sms ： {1}, Time : {2}, result : {3}";

                LogHelper.Info(string.Format(logInfo, mobile, sms, DateTime.Now, content));

                resultData.State = 1;
                resultData.Data = result;
            }
            catch (Exception error)
            {
                resultData.Error = error.Message;
                resultData.State = 0;

                LogHelper.AppError(error.Message);
            }

            returnValue = Json(resultData);

            LogHelper.Info(returnValue.ToString());

            return returnValue;
        }

        int LoginOrRegist(string mobile, string sendSMS, string inputSMS,string channel)
        {
            var result = 0;
            if (mobile.IsNull())
                return result;

            var ip = this.GetClientIp ();

            var service = new Service.User.BaseInfoService();
            var userInfo = service.GetUserInfoByMobile(mobile, ip);

            if (userInfo == null || userInfo.ID <= 0)
            {
                userInfo = new BaseInfo_UserInfo();

                userInfo.CreateIP = ip;
                userInfo.Mobile = mobile;
                userInfo.Channel = channel.ParseTo<byte>();

                userInfo.Status = 1;
                userInfo.CreateTime = DateTime.Now;

                result = service.AddUserInfo(userInfo);
            }
            else
            {
                result = userInfo.ID;

                service.Login(mobile, sendSMS , inputSMS);
            }

            return result;
        }


        JsonResult SendSMS()
        {
            var returnValue = new JsonResult();
            var resultData = new ResultData();

            LogHelper.Info(this.Request.RawUrl);

            try
            {
                var mobile = this.Request.Form["mobile"];

                var random = new Random();
                var _num = random.Next(1000, 9999);
                var _sms = _num.ToString();
                var result = "1";

                try
                {
                    var key = "YiDao-SendSMS-Mobile-" + mobile;
                    var sendMessage = this.HttpContext.Cache.Get(key);
                    if (sendMessage != null)
                        _sms = sendMessage.ToString();

                    new Util().SendRegisteSMS(mobile, new string[] { _sms, "5" });

                    result = "1";

                    this.HttpContext.Cache.Insert(key, _sms, null, DateTime.Now.AddMinutes(30), new TimeSpan(0), CacheItemPriority.Normal, null);
                }
                catch (Exception error)
                {
                    result = "0";
                    LogHelper.AppError(error.Message);
                }

                var logInfo = "interface sendSMS --- mobile : {0} ,sms ： {1}, Time : {2}, result : {3}";

                LogHelper.Info(string.Format(logInfo, mobile, _sms, DateTime.Now, 0));

                resultData.State = 1;
                resultData.Data = result;
            }
            catch (Exception error)
            {
                resultData.Error = error.Message;
                resultData.State = 0;

                LogHelper.AppError(error.Message);
            }

            returnValue = Json(resultData);

            LogHelper.Info(returnValue.ToString());

            return returnValue;
        }

        string GetClientIp()
        {
            return this.HttpContext.Request.UserHostAddress;
        }
    }

    [Serializable]
    public class ResultData
    {
        /// <summary>
        /// [状态]1成功，0失败
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 数据
        /// </summary>   
        public object Data { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 数据条数
        /// </summary>
        public int DataCount { get; set; }

        public static ResultData Success(object Data, int DataCount)
        {
            return new ResultData() { State = 1, Data = Data, Error = String.Empty, DataCount = DataCount };
        }

        public static ResultData Success()
        {
            return new ResultData() { State = 1 };
        }

        public static ResultData Fail(string Error)
        {
            return new ResultData() { State = 0, Error = Error };
        }

        public static ResultData Fail(string Error, object Data)
        {
            return new ResultData() { State = 0, Error = Error, Data = Data };
        }

        public static ResultData Fail()
        {
            return new ResultData() { State = 0 };
        }
    }

    public class Util
    {
        public void SendSMS(string templateId, string mobile, string[] content)
        {
            var ret = string.Empty;
            var api = new CCPRestSDK.CCPRestSDK();
            var isInit = api.init(this.SMS_SiteUrl, this.SMS_SitePort);

            api.setAppId(this.SMS_AppID);
            api.setAccount(this.SMS_Account, this.SMS_AccountPassword);

            try
            {
                if (isInit)
                {
                    Dictionary<string, object> retData = api.SendTemplateSMS(mobile, templateId, content);
                    ret = GetDictionaryData(retData);
                }
                else
                {
                    LogHelper.Info("连接短信服务器失败！");
                }
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
            }

            LogHelper.Info(ret);
        }

        public void SendRegisteSMS(string mobile, string[] content)
        {
            this.SendSMS(this.SMS_RegisteTemplateID, mobile, content);
        }
        public void SendRedPacketSMS(string mobile,string[] content)
        {
            this.SendSMS(this.SMS_RedPacketTemplateID, mobile, content);
        }
        public void SendUserInvitationSMS(string mobile, string[] content)
        {
            this.SendSMS(this.SMS_UserInvitationTemplateID, mobile, content);
        }
        public void SendUserRemindSMS(string mobile, string[] content)
        {
            this.SendSMS(this.SMS_UserRemindTemplateID, mobile, content);
        }
        string GetDictionaryData(Dictionary<string, object> data)
        {
            string ret = null;
            foreach (KeyValuePair<string, object> item in data)
            {
                if (item.Value != null && item.Value.GetType() == typeof(Dictionary<string, object>))
                {
                    ret += item.Key.ToString() + "={";
                    ret += GetDictionaryData((Dictionary<string, object>)item.Value);
                    ret += "};";
                }
                else
                {
                    ret += item.Key.ToString() + "=" + (item.Value == null ? "null" : item.Value.ToString()) + ";";
                }
            }
            return ret;
        }

        string SMS_RegisteTemplateID
        {
            get
            {
                return ConfigurationManager.AppSettings["SMS_RegisteTemplateID"];
            }
        }
        string SMS_RedPacketTemplateID
        {
            get
            {
                return ConfigurationManager.AppSettings["SMS_RedPacketTemplateID"];
            }
        }
        string SMS_UserInvitationTemplateID
        {
            get
            {
                return ConfigurationManager.AppSettings["SMS_UserInvitationTemplateID"];
            }
        }
        string SMS_UserRemindTemplateID
        {
            get
            {
                return ConfigurationManager.AppSettings["SMS_UserRemindTemplateID"];
            }
        }
        string SMS_Account
        {
            get
            {
                return ConfigurationManager.AppSettings["SMS_Account"];
            }
        }

        string SMS_AccountPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["SMS_AccountPassword"];
            }
        }

        string SMS_SiteUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SMS_SiteUrl"];
            }
        }

        string SMS_SitePort
        {
            get
            {
                return ConfigurationManager.AppSettings["SMS_SitePort"];
            }
        }

        string SMS_AppID
        {
            get
            {
                return ConfigurationManager.AppSettings["SMS_AppID"];
            }
        }

        public string ApiUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ApiUrl"];
            }
        }


        public string ApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["ApiKey"];
            }
        }


        public string ApiChannelId
        {
            get
            {
                return ConfigurationManager.AppSettings["ApiChannelId"];
            }
        }
    }
}