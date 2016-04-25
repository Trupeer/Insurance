using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using I.Utility.Helper;
using HONGLI.Service;
using HONGLI.Web.Models;

namespace HONGLI.Web.Controllers
{
    public class UserController : Controller
    {
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login( string returnUrl, string mobile,string channel )
        {
            //PostOrderService.PostOrderStatus();

            returnUrl = this.HttpContext.Server.UrlDecode(returnUrl);

            ViewBag.ReturnUrl = HttpUtility.UrlDecode(returnUrl);
            ViewBag.Mobile = mobile;
            ViewBag.Channel = channel;

            //LogHelper.Info("ori:"+returnUrl);
            //LogHelper.Info("HttpUtility.HtmlDecode:" + HttpUtility.HtmlDecode(returnUrl));
            //LogHelper.Info("HttpUtility.UrlDecode:" + ViewBag.ReturnUrl);

            return View();
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Test()
        {


            return View();
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Notice()
        {


            return View();
        }
    }
}