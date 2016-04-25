using HONGLI.Entity;
using HONGLI.Service.User;
using HONGLI.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Configuration;
using HONGLI.Service;
using I.Utility.Extensions;
using System.Text;

namespace HONGLI.Web.Controllers
{

#if (!DEBUG)
    [AuthorizationFilter]
#endif
    public class DeliverController : Controller
    {

        /// <summary>
        /// 自提地址。
        /// </summary>
        private static string _deliverPickupAddress = ConfigurationManager.AppSettings["DeliverPickupAddress"] == null ? String.Empty : ConfigurationManager.AppSettings["DeliverPickupAddress"].ToString();

        public static string CookieDomain = ConfigurationManager.AppSettings["CookieDomain"] == null ? "" : ConfigurationManager.AppSettings["CookieDomain"].ToString();

        public DeliverService _deliverService = new DeliverService();

        public ActionResult Index()
        {
#if (DEBUG)
            PostOrderService.PostOrder();
#endif
            return View();
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Edit(int id = 0, int userId = 0, int productId = 0, string mobile = "", string channel = "")
        {
            DeliverModel model = new DeliverModel();
            model.Channel = channel;
            model.Mobile = mobile;
            model.ProductId = productId;
            model.PickupAddress = _deliverPickupAddress;
            if (id == 0)
            {
                model.UserId = userId;
                return View(model);
            }
            else
            {
                User_Deliver deliver = _deliverService.GetDeliverById(id);
                model.Id = deliver.Id;
                model.UserId = deliver.UserId;
                model.DeliverAddress = deliver.DeliverAddress;
                model.DeliverLocation = deliver.DeliverLocation;
                model.DeliverMobile = deliver.DeliverMobile;
                model.DeliverName = deliver.DeliverName;
                model.DeliverPrice = deliver.DeliverPrice;
                model.DeliverTime = deliver.DeliverTime;
                model.DeliverType = deliver.DeliverType;
                model.PickupAddress = deliver.PickupAddress;
                model.PickupTime = deliver.PickupTime;
                model.CreateDate = deliver.CreateDate;
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(DeliverModel model)
        {

            //PostOrderService.PostOrder();
            string returnUrl = string.Empty;
            var urlReferrer = Request.UrlReferrer;
            if (urlReferrer != null)
            {
                string urlQuery = urlReferrer.Query;
                List<string> urlList = new List<string>();
                if (model.Id == 0)
                {
                    urlList = urlQuery.Split('&').ToList();
                }
                else
                {
                    urlList = urlQuery.Split('?').ToList();
                }
                var pageurl = string.Empty;
                foreach (var urlItem in urlList)
                {
                    if (urlItem.Contains("pageurl"))
                    {
                        pageurl = urlItem;
                    }
                }
                returnUrl = pageurl.ToLower().Replace("pageurl=", "");
                returnUrl = HttpUtility.UrlDecode(returnUrl);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["PageUrl"]))
            {
                returnUrl = Request.QueryString["PageUrl"];
            }
            if (ModelState.IsValid)
            {
                var result = 0;
                var deliver = new User_Deliver
                {
                    Id = model.Id,
                    UserId = model.UserId,
                    DeliverAddress = model.DeliverAddress,
                    DeliverLocation = model.DeliverLocation,
                    DeliverMobile = model.DeliverMobile,
                    DeliverName = model.DeliverName,
                    DeliverPrice = model.DeliverPrice,
                    DeliverTime = model.DeliverTime,
                    DeliverType = model.DeliverType,
                    PickupAddress = model.PickupAddress,
                    PickupTime = model.PickupTime,
                    //PickupAddress = model.PickupAddress,
                    //PickupTime = model.PickupTime,
                    CreateDate = model.CreateDate
                };
                if (deliver.Id == 0)
                {
                    result = _deliverService.AddDeliver(deliver);
                }
                else
                {
                    result = _deliverService.UpdateDeliver(deliver);
                }
                if (result > 0)
                {
                    try
                    {
                        OrderDeliver orderDeliver = new OrderDeliver();
                        orderDeliver.deliverAddress = model.DeliverAddress;
                        orderDeliver.deliverDistrictCode = model.DeliverLocation;
                        orderDeliver.deliverMobile = model.DeliverMobile;
                        orderDeliver.deliverName = model.DeliverName;
                        HttpCookie cookie = new HttpCookie("HONGLI.order.deliver");
                        cookie.Domain = CookieDomain;
                        cookie.Value = orderDeliver.ToJsonItem();
                        Response.Cookies.Set(cookie);
                        //Response.Cookies["HONGLI.order.deliver"].Value = orderDeliver.ToJsonItem();
                    }
                    catch (Exception)
                    {

                    }
                    Response.Cookies["HONGLI.order.policyHolderSelected"].Value = result.ToString();
                    return RedirectToAction("List", "Deliver", new { UserId = model.UserId, PageType = "Edit", productId = model.ProductId, mobile = model.Mobile, channel = model.Channel });
                }
                else
                {
                    ModelState.AddModelError("", "保存失败！");
                }
            }
            model.PickupAddress = _deliverPickupAddress;

            return View(model);
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult TypeEdit(int id = 0, int userId = 0, int productId = 0, string mobile = "", string channel = "")
        {
            DeliverModel model = new DeliverModel();
            model.Channel = channel;
            model.Mobile = mobile;
            model.ProductId = productId;
            var urlReferrer = Request.UrlReferrer;
            if (urlReferrer != null)
            {
                model.ReturnUrl = urlReferrer.AbsoluteUri;
            }
            if (id == 0)
            {
                model.UserId = userId;
                model.PickupAddress = _deliverPickupAddress;
                return View(model);
            }
            else
            {
                User_Deliver deliver = _deliverService.GetDeliverById(id);
                model.Id = deliver.Id;
                model.UserId = deliver.UserId;
                model.DeliverAddress = deliver.DeliverAddress;
                model.DeliverLocation = deliver.DeliverLocation;
                model.DeliverMobile = deliver.DeliverMobile;
                model.DeliverName = deliver.DeliverName;
                model.DeliverPrice = deliver.DeliverPrice;
                model.DeliverTime = deliver.DeliverTime;
                model.DeliverType = deliver.DeliverType;
                model.PickupAddress = deliver.PickupAddress;
                model.PickupTime = deliver.PickupTime;
                model.CreateDate = deliver.CreateDate;
                if (string.IsNullOrEmpty(deliver.PickupAddress))
                {
                    model.PickupAddress = _deliverPickupAddress;
                }
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult TypeEdit(DeliverModel model)
        {
            //if (ModelState.IsValid)
            //{
            var result = 0;
            User_Deliver deliver = new User_Deliver();
            if (model.DeliverType == 1)
            {
                deliver = new User_Deliver
                {
                    Id = model.Id,
                    UserId = model.UserId,
                    //DeliverAddress = model.DeliverAddress,
                    //DeliverLocation = model.DeliverLocation,
                    //DeliverMobile = model.DeliverMobile,
                    //DeliverName = model.DeliverName,
                    //DeliverPrice = model.DeliverPrice,
                    //DeliverTime = model.DeliverTime,
                    DeliverType = model.DeliverType,
                    //PickupAddress = model.PickupAddress,
                    //PickupTime = model.PickupTime
                    //CreateDate = model.CreateDate
                };
            }
            else
            {
                deliver = new User_Deliver
                {
                    Id = model.Id,
                    UserId = model.UserId,
                    //DeliverAddress = model.DeliverAddress,
                    //DeliverLocation = model.DeliverLocation,
                    //DeliverMobile = model.DeliverMobile,
                    //DeliverName = model.DeliverName,
                    //DeliverPrice = model.DeliverPrice,
                    //DeliverTime = model.DeliverTime,
                    DeliverType = model.DeliverType,
                    PickupAddress = model.PickupAddress,
                    PickupTime = model.PickupTime
                    //CreateDate = model.CreateDate
                };
            }
            if (deliver.Id == 0)
            {
                deliver.CreateDate = DateTime.Now;
                result = _deliverService.AddDeliver(deliver);
            }
            else
            {
                result = _deliverService.UpdateDeliverType(deliver);
            }
            if (string.IsNullOrEmpty(model.PickupAddress))
            {
                model.PickupAddress = _deliverPickupAddress;
            }
            if (result > 0)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    string reurl = string.Empty;
                    string returnurl = model.ReturnUrl.ToLower();
                    string[] urlArr = returnurl.Split('?');
                    if (urlArr.Length > 0) {
                        reurl = urlArr[0];
                    }
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("UserId", model.UserId.ToString());
                    dict.Add("productId", model.ProductId.ToString());
                    dict.Add("mobile", model.Mobile);
                    dict.Add("channel", model.Channel);
                    string url = GetUrl(reurl, dict);
                    if (!string.IsNullOrEmpty(url))
                    {
                        return Redirect(url);
                    }
                    else
                    {
                        return RedirectToAction("Do", "Order", new { UserId = model.UserId, productId = model.ProductId, mobile = model.Mobile, channel = model.Channel });
                    }
                }
                else
                {
                    return RedirectToAction("Do", "Order", new { UserId = model.UserId, productId = model.ProductId, mobile = model.Mobile, channel = model.Channel });
                    //return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "保存失败！");
            }
            //}
            //model.PickupAddress = _deliverPickupAddress;
            return View(model);
        }


        public ActionResult List(int userId = 0, int productId = 0, string mobile = "", string channel = "")
        {
            //int productId = 0;
            //string mobile = string.Empty;
            //string channel = string.Empty;
            string pageType = string.Empty;
            string returnPageUrl = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["PageType"]))
            {
                pageType = Request.QueryString["PageType"];
            }
            var urlReferrer = Request.UrlReferrer;
            if (pageType != "Edit")
            {
                if (urlReferrer != null)
                {
                    returnPageUrl = urlReferrer.AbsoluteUri;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.QueryString["PageUrl"]))
                {
                    returnPageUrl = Request.QueryString["PageUrl"];
                }
            }
            DeliverListModel model = new DeliverListModel();
            model.Channel = channel;
            model.Mobile = mobile;
            model.ProductId = productId;
            List<User_Deliver> deliverList = _deliverService.GetDeliverByUser(userId);
            deliverList.Where(d => d.DeliverName != "" && d.DeliverName != null).ToList();
            deliverList.ForEach(deliver => model.DeliverList.Add(
               new DeliverModel
               {
                   Id = deliver.Id,
                   DeliverType = deliver.DeliverType,
                   DeliverAddress = deliver.DeliverAddress,
                   DeliverTime = deliver.DeliverTime,
                   DeliverPrice = deliver.DeliverPrice,
                   DeliverName = deliver.DeliverName,
                   CreateDate = deliver.CreateDate,
                   DeliverMobile = deliver.DeliverMobile,
                   DeliverLocation = deliver.DeliverLocation,
                   PickupAddress = deliver.PickupAddress,
                   PickupTime = deliver.PickupTime,
                   UserId = deliver.UserId
               }));
            model.UserId = userId;
            model.PageUrl = returnPageUrl;
            return View(model);
        }


        public static string GetUrl(string url, IDictionary<string, string> parameters)
        {
            var resultUrl = String.Empty;
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    resultUrl = url + "&" + BuildRequestData(parameters);
                }
                else
                {
                    resultUrl = url + "?" + BuildRequestData(parameters);
                }
            }

            return resultUrl;
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
    }

    internal class OrderDeliver
    {
        public string deliverName { get; set; }
        public string deliverMobile { get; set; }
        public string deliverDistrictCode { get; set; }
        public string deliverAddress { get; set; }

    }

}