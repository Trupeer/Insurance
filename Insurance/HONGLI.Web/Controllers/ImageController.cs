using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HONGLI.Web.Models;

namespace HONGLI.Web.Controllers
{
    public class ImageController : Controller
    {

        [HttpPost]
        public ActionResult Upload(PolicyHolderModel model, HttpPostedFileBase file)
        {
            if (file == null)
            {
                return Content("没有文件！", "text/plain");
            }
            var fileName = Path.Combine(Request.MapPath("~/Upload"), Path.GetFileName(file.FileName));
            try
            {
                file.SaveAs(fileName);
                //tm.AttachmentPath = fileName;//得到全部model信息
                //model.AttachmentPath = "../upload/" + Path.GetFileName(file.FileName);
                //return Content("上传成功！", "text/plain");
                return RedirectToAction("Show", model);
            }
            catch
            {
                return Content("上传异常 ！", "text/plain");
            }
        }


    }
}