using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web.Mvc;
using System.Web.Routing;

using I.Utility.Helper;
using HONGLI.Web.Models;


namespace HONGLI.Web.Controllers
{
    public class ExceptionFilter: HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            var userId = 0;
            try
            {
                userId = UserViewModel.CurrentUser.ID;
            }
            catch
            {

            }

            LogHelper.AppError(string.Format("userId:{0}异常,{1}", userId, filterContext.Exception));

            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(
                new {
                    controller = "User",
                    action = "Notice",
                    Error = filterContext.HttpContext.Server.UrlEncode(filterContext.Exception.Message) }));

            filterContext.ExceptionHandled = true;

            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

        }

    }
}