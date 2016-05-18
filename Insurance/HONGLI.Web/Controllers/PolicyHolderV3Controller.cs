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
using System.Drawing;
using System.Web.UI.HtmlControls;

namespace HONGLI.Web.Controllers
{
#if (!DEBUG)
    [AuthorizationFilter]
#endif
    public class PolicyHolderV3Controller : Controller
    {

        private PolicyHolderService _policyHolderService = new PolicyHolderService();

        public static string CookieDomain = ConfigurationManager.AppSettings["CookieDomain"] == null ? "" : ConfigurationManager.AppSettings["CookieDomain"].ToString();
        public static string AppRootUrl = ConfigurationManager.AppSettings["AppRootUrl"] == null ? "" : ConfigurationManager.AppSettings["AppRootUrl"].ToString();
        public static string AppRootPath = ConfigurationManager.AppSettings["AppRootPath"] == null ? "" : ConfigurationManager.AppSettings["AppRootPath"].ToString();
        public static string MaxFileSize = ConfigurationManager.AppSettings["MaxFileSize"] == null ? "0" : ConfigurationManager.AppSettings["MaxFileSize"].ToString();

        public static int ImagesZipSize = ConfigurationManager.AppSettings["ImagesZipSize"] == null ? 1048576 : Convert.ToInt32(ConfigurationManager.AppSettings["ImagesZipSize"].ToString());

        public static int ImagesZipWidth = ConfigurationManager.AppSettings["ImagesZipWidth"] == null ? 1200 : Convert.ToInt32(ConfigurationManager.AppSettings["ImagesZipWidth"].ToString());

        public static int ImagesZipHeight = ConfigurationManager.AppSettings["ImagesZipHeight"] == null ? 1200 : Convert.ToInt32(ConfigurationManager.AppSettings["ImagesZipHeight"].ToString());

        public static int ImagesThumbWidth = ConfigurationManager.AppSettings["ImagesThumbWidth"] == null ? 120 : Convert.ToInt32(ConfigurationManager.AppSettings["ImagesThumbWidth"].ToString());

        public static int ImagesThumbHeight = ConfigurationManager.AppSettings["ImagesThumbHeight"] == null ? 120 : Convert.ToInt32(ConfigurationManager.AppSettings["ImagesThumbHeight"].ToString());

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
            OrderPolicyHolderV3 orderPolicyHolder = new OrderPolicyHolderV3();
            OrderPolicyHolderPicV3 orderPolicyHolderPic = new OrderPolicyHolderPicV3();
            if (model.IdCardType == 1)
            {
                User_PolicyHolder oldPpolicyHolder = _policyHolderService.GetPolicyHolderById(model.Id);

                if (Request.Cookies["HONGLI.order.policyHolder"] != null)
                {
                    var policyHolderCookies = Request.Cookies["HONGLI.order.policyHolder"].Value;
                    if (policyHolderCookies != null)
                    {
                        orderPolicyHolder = policyHolderCookies.FromJsonTo<OrderPolicyHolderV3>();
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
                                    UploadImageZip(picIdCardFace, picIdCardFacePath, ImagesZipWidth, ImagesZipHeight);
                                    //picIdCardFace.SaveAs(picIdCardFacePath);
                                    model.IdCardFacePicUrl = Path.Combine(string.Format("{0}{1}", ImagesUpload, IdCardUpload), "IdCardFace_" + fileName + "." + expName);
                                    var picIdCardFaceThumbPath = Path.Combine(Request.MapPath(string.Format("~/{0}{1}", ImagesUpload, IdCardUpload)), "IdCardFace_Thumb_" + fileName + "." + expName);
                                    MakeThumbnail(picIdCardFace.InputStream, picIdCardFaceThumbPath, ImagesThumbWidth, ImagesThumbHeight, "W");
                                    orderPolicyHolderPic.idcardFacePicUrl = Path.Combine(string.Format("{0}{1}", ImagesUpload, IdCardUpload), "IdCardFace_Thumb_" + fileName + "." + expName);
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
                                model.Error = "上传身份证正面失败，请上传不大于 " + maxFileSize / 1024 / 1024 + "M 的图片！";
                                LogHelper.Info("未上传身份证反面，文件大小：" + picIdCardFace.ContentLength + "。");
                                return View(model);
                            }
                        }
                        //}
                        //}
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
                                    UploadImageZip(picIdCardBack, picIdCardBackPath, ImagesZipWidth, ImagesZipHeight);
                                    //picIdCardBack.SaveAs(picIdCardBackPath);
                                    model.IdCardBackPicUrl = Path.Combine(string.Format("{0}{1}", ImagesUpload, IdCardUpload), "IdCardBack_" + fileName + "." + expName);
                                    var picIdCardBackThumbPath = Path.Combine(Request.MapPath(string.Format("~/{0}{1}", ImagesUpload, IdCardUpload)), "IdCardBack_Thumb_" + fileName + "." + expName);
                                    MakeThumbnail(picIdCardBack.InputStream, picIdCardBackThumbPath, ImagesThumbWidth, ImagesThumbHeight, "W");
                                    orderPolicyHolderPic.idcardBackPicUrl = Path.Combine(string.Format("{0}{1}", ImagesUpload, IdCardUpload), "IdCardBack_Thumb_" + fileName + "." + expName);
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
                                model.Error = "上传身份证反面失败，请上传不大于 " + maxFileSize / 1024 / 1024 + "M 的图片！";
                                LogHelper.Info("未上传身份证反面，文件大小：" + picIdCardBack.ContentLength + "。");
                                return View(model);
                            }
                        }
                        //}
                        //}
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
                    OrderPolicyHolderV3 newOrderPolicyHolder = new OrderPolicyHolderV3();
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


                    HttpCookie policyHolderPicCookie = new HttpCookie("HONGLI.order.policyHolderPic");
                    policyHolderPicCookie.Domain = CookieDomain;
                    policyHolderPicCookie.Value = orderPolicyHolderPic.ToJsonItem();
                    Response.Cookies.Set(policyHolderPicCookie);

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
                            return RedirectToAction("Do", "OrderV3", new { UserId = model.UserId, productId = model.ProductId, mobile = model.Mobile, channel = model.Channel, intentionCompany = model.intentionCompany, OrderCode = model.OrderCode, OrderBaseId = model.OrderBaseId, OrderItemId = model.OrderItemId, OrderPolicyId = model.OrderPolicyId, OrderDeliverId = model.OrderDeliverId });
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

        public string UploadImageZip(HttpPostedFileBase httpPostedFile, string savePath, int thumbWidth, int thumbHeight)
        {
            //, out string error
            string error = string.Empty;

            string newFileName = string.Empty;

            int contentLength = httpPostedFile.ContentLength;

            if (contentLength == 0)
                error = "没有选择上传图片。";
            //获取upImage选择文件的扩展名
            string extendName = Path.GetExtension(httpPostedFile.FileName).ToLower();
            //判断是否为图片格式
            if (extendName != ".jpg" && extendName != ".jpeg" && extendName != ".gif" && extendName != ".bmp" && extendName != ".png")
                error = "图片格式不正确。";


            byte[] fileData = new byte[contentLength];
            httpPostedFile.InputStream.Read(fileData, 0, contentLength);
            //sFilename = Path.GetFileName(httpPostedFile.FileName);

            //int file_append = 0;
            //检查当前文件夹下是否有同名图片,有则在文件名+1
            //while (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(sSavePath + sFilename)))
            //{
            //    file_append++;
            //    sFilename = Path.GetFileNameWithoutExtension(myFile.FileName)
            //        + file_append.ToString() + extendName;
            //}


            if (contentLength > ImagesZipSize)
            {
                using (Image sourceImage = Image.FromStream(httpPostedFile.InputStream))
                {
                    //原图宽度和高度
                    int width = sourceImage.Width;
                    int height = sourceImage.Height;
                    int smallWidth;
                    int smallHeight;
                    //获取第一张绘制图的大小,(比较 原图的宽/缩略图的宽  和 原图的高/缩略图的高)
                    if (((decimal)width) / height <= ((decimal)thumbWidth) / thumbHeight)
                    {
                        smallWidth = thumbWidth;
                        smallHeight = thumbWidth * height / width;
                    }
                    else
                    {
                        smallWidth = thumbHeight * width / height;
                        smallHeight = thumbHeight;
                    }
                    MakeThumbnail(httpPostedFile.InputStream, savePath, thumbWidth, thumbHeight, "W");

                }
            }
            else
            {
                FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
                fileStream.Write(fileData, 0, fileData.Length);
                fileStream.Close();
            }


            return newFileName;
        }


        /// <summary> 
        /// 生成缩略图 
        /// </summary> 
        /// <param name="originalImagePath">源图路径（物理路径）</param> 
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param> 
        /// <param name="width">缩略图宽度</param> 
        /// <param name="height">缩略图高度</param> 
        /// <param name="mode">生成缩略图的方式</param>     
        public static void MakeThumbnail(Stream stream, string thumbnailPath, int width, int height, string mode)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromStream(stream);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                 
                    break;
                case "W"://指定宽，高按比例                     
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例 
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                 
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片 
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板 

            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充 

            g.Clear(Color.White);

            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图 

                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        /// <summary>
        /// asp.net上传图片并生成缩略图
        /// </summary>
        /// <param name="upImage">HtmlInputFile控件</param>
        /// <param name="sSavePath">保存的路径,些为相对服务器路径的下的文件夹</param>
        /// <param name="sThumbExtension">缩略图的thumb</param>
        /// <param name="intThumbWidth">生成缩略图的宽度</param>
        /// <param name="intThumbHeight">生成缩略图的高度</param>
        /// <returns>缩略图名称</returns>
        public string UpLoadImage(HtmlInputFile upImage, string sSavePath, string sThumbExtension, int intThumbWidth, int intThumbHeight)
        {
            string sThumbFile = "";
            string sFilename = "";
            if (upImage.PostedFile != null)
            {
                HttpPostedFile myFile = upImage.PostedFile;
                int nFileLen = myFile.ContentLength;
                if (nFileLen == 0)
                    return "没有选择上传图片";
                //获取upImage选择文件的扩展名
                string extendName = Path.GetExtension(myFile.FileName).ToLower();
                //判断是否为图片格式
                if (extendName != ".jpg" && extendName != ".jpge" && extendName != ".gif" && extendName != ".bmp" && extendName != ".png")
                    return "图片格式不正确";

                byte[] myData = new byte[nFileLen];
                myFile.InputStream.Read(myData, 0, nFileLen);
                sFilename = Path.GetFileName(myFile.FileName);
                int file_append = 0;
                //检查当前文件夹下是否有同名图片,有则在文件名+1
                while (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(sSavePath + sFilename)))
                {
                    file_append++;
                    sFilename = Path.GetFileNameWithoutExtension(myFile.FileName)
                        + file_append.ToString() + extendName;
                }
                FileStream newFile
                    = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sSavePath + sFilename),
                    FileMode.Create, FileAccess.Write);
                newFile.Write(myData, 0, myData.Length);
                newFile.Close();
                //以上为上传原图
                try
                {
                    //原图加载
                    using (System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(sSavePath + sFilename)))
                    {
                        //原图宽度和高度
                        int width = sourceImage.Width;
                        int height = sourceImage.Height;
                        int smallWidth;
                        int smallHeight;
                        //获取第一张绘制图的大小,(比较 原图的宽/缩略图的宽  和 原图的高/缩略图的高)
                        if (((decimal)width) / height <= ((decimal)intThumbWidth) / intThumbHeight)
                        {
                            smallWidth = intThumbWidth;
                            smallHeight = intThumbWidth * height / width;
                        }
                        else
                        {
                            smallWidth = intThumbHeight * width / height;
                            smallHeight = intThumbHeight;
                        }
                        //判断缩略图在当前文件夹下是否同名称文件存在
                        file_append = 0;
                        sThumbFile = sThumbExtension + Path.GetFileNameWithoutExtension(myFile.FileName) + extendName;
                        while (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(sSavePath + sThumbFile)))
                        {
                            file_append++;
                            sThumbFile = sThumbExtension + System.IO.Path.GetFileNameWithoutExtension(myFile.FileName) +
                                file_append.ToString() + extendName;
                        }
                        //缩略图保存的绝对路径
                        string smallImagePath = System.Web.HttpContext.Current.Server.MapPath(sSavePath) + sThumbFile;
                        //新建一个图板,以最小等比例压缩大小绘制原图
                        using (System.Drawing.Image bitmap = new Bitmap(smallWidth, smallHeight))
                        {
                            //绘制中间图
                            using (Graphics g = Graphics.FromImage(bitmap))
                            {
                                //高清,平滑
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                g.Clear(Color.Black);
                                g.DrawImage(
                                sourceImage,
                                new Rectangle(0, 0, smallWidth, smallHeight),
                                new Rectangle(0, 0, width, height),
                                GraphicsUnit.Pixel
                                );
                            }
                            //新建一个图板,以缩略图大小绘制中间图
                            using (System.Drawing.Image bitmap1 = new System.Drawing.Bitmap(intThumbWidth, intThumbHeight))
                            {
                                //绘制缩略图  http://www.cnblogs.com/sosoft/
                                using (Graphics g = Graphics.FromImage(bitmap1))
                                {
                                    //高清,平滑
                                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                    g.Clear(Color.Black);
                                    int lwidth = (smallWidth - intThumbWidth) / 2;
                                    int bheight = (smallHeight - intThumbHeight) / 2;
                                    g.DrawImage(bitmap, new Rectangle(0, 0, intThumbWidth, intThumbHeight), lwidth, bheight, intThumbWidth, intThumbHeight, GraphicsUnit.Pixel);
                                    g.Dispose();
                                    bitmap1.Save(smallImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    //出错则删除
                    System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(sSavePath + sFilename));
                    return "图片格式不正确";
                }
                //返回缩略图名称
                return sThumbFile;
            }
            return "没有选择图片";
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

    internal class OrderPolicyHolderPicV3
    {
        public string idcardBackPicUrl { get; set; }
        public string idcardFacePicUrl { get; set; }
    }

    internal class OrderPolicyHolderV3
    {
        public string idcard { get; set; }
        public string idcardBackPicUrl { get; set; }
        public string idcardFacePicUrl { get; set; }
        public string idcardType { get; set; }
        public string name { get; set; }

    }
}
