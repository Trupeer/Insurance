using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using HONGLI.Web.Models.User;
using System.Web;
using System.IO;
using System.Configuration;
using HONGLI.Entity;
using HONGLI.Service.User;
using I.Utility.Extensions;
using I.Utility.Helper;

namespace HONGLI.Web.Controllers
{
    public class PolicyHolderV2Controller : Controller
    {
        private PolicyHolderService _policyHolderService = new PolicyHolderService();

        public static string CookieDomain = ConfigurationManager.AppSettings["CookieDomain"] == null ? "" : ConfigurationManager.AppSettings["CookieDomain"].ToString();
        public static string AppRootUrl = ConfigurationManager.AppSettings["AppRootUrl"] == null ? "" : ConfigurationManager.AppSettings["AppRootUrl"].ToString();
        public static string AppRootPath = ConfigurationManager.AppSettings["AppRootPath"] == null ? "" : ConfigurationManager.AppSettings["AppRootPath"].ToString();
        public static string MaxFileSize = ConfigurationManager.AppSettings["MaxFileSize"] == null ? "0" : ConfigurationManager.AppSettings["MaxFileSize"].ToString();
        public static string ImagesUpload = ConfigurationManager.AppSettings["ImagesUpload"] == null ? String.Empty : ConfigurationManager.AppSettings["ImagesUpload"].ToString();
        public static string IdCardUpload = ConfigurationManager.AppSettings["IdCardUpload"] == null ? String.Empty : ConfigurationManager.AppSettings["IdCardUpload"].ToString();

        // GET: /PolicyHolder/Edit
        [AllowAnonymous]
        public ActionResult Edit(int id = 0, int userId = 0, int productId = 0, string mobile = "", string channel = "", int intentionCompany = 0, string OrderCode = "", int OrderBaseId = 0, int OrderItemId = 0, int OrderPolicyId = 0, int OrderDeliverId = 0)
        {
            PolicyHolderModel model = new PolicyHolderModel();

            model.Channel = channel;
            model.Mobile = mobile;
            model.ProductId = productId;
            model.intentionCompany = intentionCompany;
            model.OrderCode = OrderCode;
            model.OrderBaseId = OrderBaseId;
            model.OrderItemId = OrderItemId;
            model.OrderPolicyId = OrderPolicyId;
            model.OrderDeliverId = OrderDeliverId;
            if (!string.IsNullOrEmpty(AppRootPath) && !string.IsNullOrEmpty(AppRootUrl))
            {
                model.UploadUrl = string.Format("{0}{1}", AppRootUrl, AppRootPath);
            }

            var urlReferrer = Request.UrlReferrer;
            if (urlReferrer != null)
                model.ReturnUrl = urlReferrer.AbsoluteUri;

            int maxFileSize = 209715200;
            try
            {
                maxFileSize = Convert.ToInt32(MaxFileSize);
            }
            catch (Exception)
            {
                maxFileSize = 209715200;
            }

            model.MaxUploadSize = maxFileSize;


            if (id == 0)
            {
                model.UserId = userId;
                model.IdCardType = 1;

                return View(model);
            }
            else
            {
                User_PolicyHolder policyHolder = _policyHolderService.GetPolicyHolderById(id);

                model.Id = policyHolder.Id;
                model.IdCard = policyHolder.IdCard;
                model.IdCardBackPicUrl = policyHolder.IdCardBackPicUrl;
                model.IdCardFacePicUrl = policyHolder.IdCardFacePicUrl;
                model.IdCardType = policyHolder.IdCardType;
                model.UserId = policyHolder.UserId;
                model.OrganizationCode = policyHolder.OrganizationCode;
                model.CreateDate = policyHolder.CreateDate;

                return View(model);
            }
        }


        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(PolicyHolderModel model)
        {
            model.ViewType = model.IdCardType;
            HttpFileCollectionBase files = Request.Files;

            if (!string.IsNullOrEmpty(AppRootPath) && !string.IsNullOrEmpty(AppRootUrl))
            {
                model.UploadUrl = string.Format("{0}{1}", AppRootUrl, AppRootPath);
            }

            int maxFileSize = 2097152;
            try
            {
                maxFileSize = Convert.ToInt32(MaxFileSize);
            }
            catch (Exception)
            {
                maxFileSize = 2097152;
            }
            model.MaxUploadSize = maxFileSize;
            OrderPolicyHolder orderPolicyHolder = new OrderPolicyHolder();
            if (model.IdCardType == 1)
            {
                User_PolicyHolder oldPpolicyHolder = _policyHolderService.GetPolicyHolderById(model.Id);

                if (Request.Cookies["HONGLI.order.policyHolder"] != null)
                {
                    var policyHolderCookies = Request.Cookies["HONGLI.order.policyHolder"].Value;
                    if (policyHolderCookies != null)
                    {
                        orderPolicyHolder = policyHolderCookies.FromJsonTo<OrderPolicyHolder>();
                    }
                }

                if (files.Count != 0 && files.AllKeys.Count() == 2)
                {
                    string fileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                    if (oldPpolicyHolder == null || string.IsNullOrEmpty(oldPpolicyHolder.IdCardFacePicUrl))
                    {
                        if (files["IdCardFacePic"] != null)
                        {
                            var picIdCardFace = files["IdCardFacePic"];
                            string expName = Path.GetFileName(picIdCardFace.FileName).Split('.').LastOrDefault();
                            var picIdCardFacePath = Path.Combine(Request.MapPath(string.Format("~/{0}{1}", ImagesUpload, IdCardUpload)), "IdCardFace_" + fileName + "." + expName);
                            //if (string.IsNullOrEmpty(orderPolicyHolder.idcardFacePicUrl))
                            //{
                            if (picIdCardFace.ContentLength > 0 && picIdCardFace.ContentLength < maxFileSize)
                            {
                                try
                                {
                                    picIdCardFace.SaveAs(picIdCardFacePath);
                                    model.IdCardFacePicUrl = Path.Combine(string.Format("{0}{1}", ImagesUpload, IdCardUpload), "IdCardFace_" + fileName + "." + expName);
                                }
                                catch
                                {
                                    model.Error = "上传身份证正面失败，请检查文件大小和格式重新上传！";
                                    LogHelper.Info("上传身份证正面失败，文件大小：" + picIdCardFace.ContentLength + "。");
                                    return View(model);
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(orderPolicyHolder.idcardFacePicUrl))
                                {
                                    model.Error = "上传身份证正面失败，请上传不大于 " + maxFileSize / 1024 / 1024 + "M 的图片！";
                                    LogHelper.Info("未上传身份证反面，文件大小：" + picIdCardFace.ContentLength + "。");
                                    return View(model);
                                }
                            }
                            //}
                        }
                    }
                    if (oldPpolicyHolder == null || string.IsNullOrEmpty(oldPpolicyHolder.IdCardBackPicUrl))
                    {
                        if (files["IdCardBackPic"] != null)
                        {
                            var picIdCardBack = files["IdCardBackPic"];
                            string expName = Path.GetFileName(picIdCardBack.FileName).Split('.').LastOrDefault();
                            var picIdCardBackPath = Path.Combine(Request.MapPath(string.Format("~/{0}{1}", ImagesUpload, IdCardUpload)), "IdCardBack_" + fileName + "." + expName);
                            //if (string.IsNullOrEmpty(orderPolicyHolder.idcardBackPicUrl))
                            //{
                            if (picIdCardBack.ContentLength > 0 && picIdCardBack.ContentLength < maxFileSize)
                            {
                                try
                                {
                                    picIdCardBack.SaveAs(picIdCardBackPath);
                                    model.IdCardBackPicUrl = Path.Combine(string.Format("{0}{1}", ImagesUpload, IdCardUpload), "IdCardBack_" + fileName + "." + expName);
                                }
                                catch
                                {
                                    model.Error = "上传失败，请检查文件大小和格式重新上传！";
                                    LogHelper.Info("上传身份证反面失败，文件大小：" + picIdCardBack.ContentLength + "。");
                                    return View(model);
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(orderPolicyHolder.idcardBackPicUrl))
                                {
                                    model.Error = "上传身份证反面失败，请上传不大于 " + maxFileSize / 1024 / 1024 + "M 的图片！";
                                    LogHelper.Info("未上传身份证反面，文件大小：" + picIdCardBack.ContentLength + "。");
                                    return View(model);
                                }
                            }
                            //}
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var result = 0;
                var policyHolder = new User_PolicyHolder();
                if (model.IdCardType == 0 || model.IdCardType == 1)
                {
                    policyHolder = new User_PolicyHolder
                    {
                        Id = model.Id,
                        UserId = model.UserId,
                        IdCard = model.IdCard,
                        IdCardBackPicUrl = model.IdCardBackPicUrl,
                        IdCardFacePicUrl = model.IdCardFacePicUrl,
                        Name = model.Name,
                        IdCardType = model.IdCardType,
                        CreateDate = model.CreateDate
                    };
                    if (orderPolicyHolder != null && !string.IsNullOrEmpty(orderPolicyHolder.idcardFacePicUrl))
                    {
                        policyHolder.IdCardFacePicUrl = orderPolicyHolder.idcardFacePicUrl;
                    }
                    if (orderPolicyHolder != null && !string.IsNullOrEmpty(orderPolicyHolder.idcardBackPicUrl))
                    {
                        policyHolder.IdCardBackPicUrl = orderPolicyHolder.idcardBackPicUrl;
                    }
                }
                else
                {
                    policyHolder = new User_PolicyHolder
                    {
                        Id = model.Id,
                        UserId = model.UserId,
                        Name = model.Name,
                        IdCardType = model.IdCardType,
                        OrganizationCode = model.OrganizationCode,
                        CreateDate = model.CreateDate
                    };
                }

                if (policyHolder.Id == 0)
                {
                    policyHolder.CreateDate = DateTime.Now;
                    result = _policyHolderService.AddPolicyHolder(policyHolder);
                }
                else
                {
                    result = _policyHolderService.UpdatePolicyHolder(policyHolder);
                }
                //if (policyHolderIdCardType == 1)
                //{
                //    order_policyHolder = new global.order.policyHolder(policyHolderName, policyHolderIdCardType, policyHolderIdCardNumber, "face.jpg", "back.jpg");
                //}
                //else
                //{
                //    order_policyHolder = new global.order.policyHolder(policyHolderName, policyHolderIdCardType, policyHolderOrganizationCode, "", "");
                //}
                //var order_policyHolder_str = JSON.stringify(order_policyHolder);
                //common.cookie.setCookie(global.domain, global.cookie.order.policyHolder, order_policyHolder_str, global.expire);
                if (result > 0)
                {
                    OrderPolicyHolder newOrderPolicyHolder = new OrderPolicyHolder();
                    if (model.IdCardType == 1)
                    {
                        newOrderPolicyHolder.name = model.Name;
                        newOrderPolicyHolder.idcardType = model.IdCardType.ToString();
                        newOrderPolicyHolder.idcard = model.IdCard;
                        if (orderPolicyHolder != null && !string.IsNullOrEmpty(orderPolicyHolder.idcardFacePicUrl))
                        {
                            if (!string.IsNullOrEmpty(model.IdCardFacePicUrl))
                            {
                                newOrderPolicyHolder.idcardFacePicUrl = model.IdCardFacePicUrl;
                            }
                            else
                            {
                                newOrderPolicyHolder.idcardFacePicUrl = orderPolicyHolder.idcardFacePicUrl;
                            }
                        }
                        else
                        {
                            newOrderPolicyHolder.idcardFacePicUrl = model.IdCardFacePicUrl;
                        }
                        if (orderPolicyHolder != null && !string.IsNullOrEmpty(orderPolicyHolder.idcardBackPicUrl))
                        {
                            if (!string.IsNullOrEmpty(model.IdCardBackPicUrl))
                            {
                                newOrderPolicyHolder.idcardBackPicUrl = model.IdCardBackPicUrl;
                            }
                            else
                            {
                                newOrderPolicyHolder.idcardBackPicUrl = orderPolicyHolder.idcardBackPicUrl;
                            }
                        }
                        else
                        {
                            newOrderPolicyHolder.idcardBackPicUrl = model.IdCardBackPicUrl;
                        }
                    }
                    else
                    {
                        newOrderPolicyHolder.name = model.Name;
                        newOrderPolicyHolder.idcardType = model.IdCardType.ToString();
                        newOrderPolicyHolder.idcard = model.OrganizationCode;
                    }
                    HttpCookie cookie = new HttpCookie("HONGLI.order.policyHolder");
                    cookie.Domain = CookieDomain;
                    cookie.Value = newOrderPolicyHolder.ToJsonItem();
                    Response.Cookies.Set(cookie);

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
    internal class OrderPolicyHolderV2
    {
        public string idcard { get; set; }
        public string idcardBackPicUrl { get; set; }
        public string idcardFacePicUrl { get; set; }
        public string idcardType { get; set; }
        public string name { get; set; }

    }
}