using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using HONGLI.Entity;
using HONGLI.Service;

using Newtonsoft.Json;

using I.Utility.Helper;
using I.Utility.Extensions;
using X3;
using System.Web;

namespace HONGLI.Web.Models
{
    public class UserViewModel
    {
        public static BaseInfo_UserInfo CurrentUser
        {
            get
            {
                BaseInfo_UserInfo userInfo = null;

                var cookie = UtilX3.GetCookie(ConfigurationManager.AppSettings["userCookieName"]);
                if (cookie.IsNull())
                    return userInfo;

                var cookieUser = JsonConvert.DeserializeObject<CookieUser>(HttpUtility.UrlDecode(cookie));
                var service = new Service.User.BaseInfoService();

                userInfo = service.GetUserInfoByMobile(cookieUser.mobile);

                return userInfo;
            }
        }
    }

    class CookieUser
    {
        public string mobile { get; set; }
        public string memberId { get; set; }
        public string channel { get; set; }
    }
}
