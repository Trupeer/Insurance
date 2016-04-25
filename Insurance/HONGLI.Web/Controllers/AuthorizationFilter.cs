using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HONGLI.Web.Models;

using System.Configuration;

namespace HONGLI.Web.Controllers
{
    public class AuthorizationFilter : System.Web.Mvc.ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var channel = filterContext.HttpContext.Request.QueryString["channel"];
            var mobile = filterContext.HttpContext.Request.QueryString["mobile"];

            var currentUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;
            currentUrl = filterContext.HttpContext.Server.UrlEncode(currentUrl);

            var loginUrl = ConfigurationManager.AppSettings["loginUrl"];
            loginUrl = string.Format(loginUrl, currentUrl, mobile, channel);

            //如果获取不到当前用户信息，则跳转到登陆页面
            if (UserViewModel.CurrentUser == null)
                filterContext.Result = new RedirectResult(loginUrl);

            //在Action执行前执行      
            base.OnActionExecuting(filterContext);
        }

    }
}