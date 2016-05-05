using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using I.Utility.Helper;
using HONGLI.Service;
using HONGLI.Web.Models;
using System.Web.Caching;
using System.Collections;

namespace HONGLI.Web.Controllers
{
    public class UserController : Controller
    {
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string channel, string mobile = "")
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
        public string clearcach()
        {
            Cache cache = HttpRuntime.Cache;
            int count = cache.Count;
            IDictionaryEnumerator CacheEnum = cache.GetEnumerator();
            while (CacheEnum.MoveNext())
            {
                string cacheItem = Server.HtmlEncode(CacheEnum.Key.ToString());
                cache.Remove(cacheItem);
            }
            return "清除缓存成功";
        }
    }
}