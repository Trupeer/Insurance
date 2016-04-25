using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HONGLI.Service
{
    public class Util
    {
        #region cookie 操作

        /// <summary>
        /// 设置cookies
        /// </summary>
        /// <param name="mainName">cookie名称</param>
        /// <param name="mainValue">cookie值</param>
        /// <param name="days">有效期</param>
        /// <param name="domain">域</param>
        /// <param name="AbsoluteUri">绝对Url</param>
        /// <returns>是否设置成功</returns>
        public static bool SetCookies(string mainName, string mainValue, int days, string domain)
        {
            try
            {
                HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[mainName];
                if (cookie == null)
                {
                    cookie = new HttpCookie(mainName, mainValue);
                }
                else
                {
                    cookie.Value = mainValue;
                }
                cookie.Expires = DateTime.Now.AddDays(days);
                cookie.Domain = domain;
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <param name="mainName">主键</param>
        /// <returns>cookie值</returns>
        public static string GetCookies(string mainName)
        {
            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[mainName];
            if (cookie != null)
            {
                string value = cookie.Value;
                return value;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 删除Cookies
        /// </summary>
        /// <param name="mainName">cookie名</param>
        /// <param name="domain">域</param>
        /// <returns>是否删除成功</returns>
        public static bool DelCookies(string mainName, string domain)
        {
            try
            {
                HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[mainName];
                if (cookie != null)
                {
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    cookie.Domain = domain;
                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
