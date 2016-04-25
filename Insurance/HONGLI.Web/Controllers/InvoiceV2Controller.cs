using HONGLI.Entity;
using HONGLI.Service.User;
using HONGLI.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HONGLI.Web.Controllers
{
    public class InvoiceV2Controller : Controller
    {
        public InvoiceService _invoiceService = new InvoiceService();
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Edit(int id = 0, int userId = 0, int productId = 0, string mobile = "", string channel = "", int intentionCompany = 0, string OrderCode = "", int OrderBaseId = 0, int OrderItemId = 0, int OrderPolicyId = 0, int OrderDeliverId = 0)
        {
            InvoiceModel model = new InvoiceModel();
            model.Channel = channel;
            model.Mobile = mobile;
            model.ProductId = productId;
            model.intentionCompany = intentionCompany;
            model.OrderCode = OrderCode;
            model.OrderBaseId = OrderBaseId;
            model.OrderItemId = OrderItemId;
            model.OrderPolicyId = OrderPolicyId;
            model.OrderDeliverId = OrderDeliverId;
            var urlReferrer = Request.UrlReferrer;
            if (urlReferrer != null)
            {
                model.ReturnUrl = urlReferrer.AbsoluteUri;
            }
            if (id == 0)
            {
                model.UserId = userId;
                return View(model);
            }
            else
            {
                User_Invoice invoice = _invoiceService.GetDeliverById(id);
                model.Id = invoice.Id;
                model.UserId = invoice.UserId;
                model.Context = invoice.Context;
                model.Type = invoice.Type;
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(InvoiceModel model)
        {
            if (ModelState.IsValid)
            {
                var result = 0;
                var invoice = new User_Invoice
                {
                    Id = model.Id,
                    Context = model.Context,
                    UserId = model.UserId,
                    Type = model.Type

                };
                if (invoice.Id == 0)
                {
                    result = _invoiceService.AddInvoice(invoice);
                }
                else
                {
                    result = _invoiceService.UpdateInvoice(invoice);
                }
                if (result > 0)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        string reurl = string.Empty;
                        string returnurl = model.ReturnUrl.ToLower();
                        string[] urlArr = returnurl.Split('?');
                        if (urlArr.Length > 0)
                        {
                            reurl = urlArr[0];
                        }
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        dict.Add("UserId", model.UserId.ToString());
                        dict.Add("productId", model.ProductId.ToString());
                        dict.Add("mobile", model.Mobile);
                        dict.Add("channel", model.Channel);
                        dict.Add("intentionCompany", model.intentionCompany.ToString());
                        dict.Add("OrderCode", model.OrderCode);
                        dict.Add("OrderBaseId", model.OrderBaseId.ToString());
                        dict.Add("OrderItemId", model.OrderItemId.ToString());
                        dict.Add("OrderPolicyId", model.OrderPolicyId.ToString());
                        dict.Add("OrderDeliverId", model.OrderDeliverId.ToString());
                        string url = GetUrl(reurl, dict);
                        if (!string.IsNullOrEmpty(url))
                        {
                            return Redirect(url);
                        }
                        else
                        {
                            return RedirectToAction("Do", "OrderV2", new { UserId = model.UserId, productId = model.ProductId, mobile = model.Mobile, channel = model.Channel, intentionCompany = model.intentionCompany, OrderCode = model.OrderCode, OrderBaseId = model.OrderBaseId, OrderItemId = model.OrderItemId, OrderPolicyId = model.OrderPolicyId, OrderDeliverId = model.OrderDeliverId });
                        }
                    }
                    else
                    {
                        return RedirectToAction("Do", "OrderV2", new { UserId = model.UserId, productId = model.ProductId, mobile = model.Mobile, channel = model.Channel, intentionCompany = model.intentionCompany, OrderCode = model.OrderCode, OrderBaseId = model.OrderBaseId, OrderItemId = model.OrderItemId, OrderPolicyId = model.OrderPolicyId, OrderDeliverId = model.OrderDeliverId });
                        //return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "保存失败！");
                }
            }
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
            var builder = new System.Text.StringBuilder();
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
}