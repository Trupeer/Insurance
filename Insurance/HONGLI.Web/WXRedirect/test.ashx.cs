using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace HONGLI.Web.WXRedirect
{
    /// <summary>
    /// test 的摘要说明
    /// </summary>
    public class test : IHttpHandler
    {

        private string AppId = "wxb23c6fac97818318";
        private string AppSecret = "922228ea5a9e17d4406c0f3950622fbc";
        public void ProcessRequest(HttpContext context)
        {
            var code = context.Request.QueryString["code"];
            if (string.IsNullOrEmpty(code))
            {
                context.Response.Redirect("wx_redpacket.aspx");
            }
            else
            {
                //获取用户信息
                var client = new System.Net.WebClient();
                client.Encoding = System.Text.Encoding.UTF8;

                var url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", AppId, AppSecret, code);
                var data = client.DownloadString(url);

                var serializer = new JavaScriptSerializer();
                var obj = serializer.Deserialize<Dictionary<string, string>>(data);
                string accessToken;
                if (!obj.TryGetValue("access_token", out accessToken))
                    return;
                //获取openId
                var openid = obj["openid"];
                context.Response.Write(openid);
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}