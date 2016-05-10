using BeeCloud;
using BeeCloud.Model;
using HONGLI.Entity;
using HONGLI.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace HONGLI.Web.WXRedirect
{
    /// <summary>
    /// UserAuth 的摘要说明
    /// </summary>
    public class UserAuth : IHttpHandler
    {

        private string AppId = "wx9714ac1c7646c25b";
        private string AppSecret = "7c1f649bc4a3a0e0be3e187a3f38972a";
        public void ProcessRequest(HttpContext context)
        {
            var code = context.Request.QueryString["code"];
            var state = context.Request.QueryString["state"];
            //context.Response.Write(code+"|" +state);
            //return;
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
                url = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN", accessToken, openid);
                data = client.DownloadString(url);
                var userInfo = serializer.Deserialize<Dictionary<string, object>>(data);
                //用户详细信息
                string infoStr = string.Empty;
                foreach (var key in userInfo.Keys)
                {
                    infoStr += (string.Format("{0}: {1}", key, userInfo[key]) + "<br/>");
                }
                //插入用户信息到返现表
                AdminService adminservice = new AdminService();
                RedPacket redpacket = adminservice.GetRedPacket(state);
                if(redpacket==null)
                {
                    context.Response.Write("<Script Language=JavaScript>alert('返现领取失败！');window.location.href='http://www.cloudsbao.com/yidaoV1.app/wxredirect/wx_redpacket.aspx';</Script>");
                    return;
                }
                else
                {
                    if (redpacket.State == 1)
                    {
                        context.Response.Write("<Script Language=JavaScript>alert('该返现已领取，不可重复领取！');window.location.href='http://www.cloudsbao.com/yidaoV1.app/wxredirect/wx_redpacket.aspx';</Script>");
                        return;
                    }
                    try
                    {
                        //设置上线模式，真实货币交易（也可不设置，默认为上线模式）
                        BeeCloud.BeeCloud.setTestMode(false);
                        BeeCloud.BeeCloud.registerApp(BCConfig.AppId, BCConfig.AppSecret, BCConfig.MasterSecret, BCConfig.TestSecret);
                        BCTransferParameter para = new BCTransferParameter();
                        para.channel = BCPay.TransferChannel.WX_TRANSFER.ToString();
                        para.transferNo = GetOrderNumber();
                        para.totalFee = Convert.ToInt32(redpacket.RedPacketMoney * 100);
                        para.desc = "返现提取";
                        para.channelUserId = openid;
                        string BCResult = BCPay.BCTransfer(para);
                        //添加提取记录
                        int result = adminservice.EditRedPacket(state, openid, infoStr, para.transferNo);
                        if (result > -1)
                        {
                            context.Response.Write("<Script Language=JavaScript>alert('返现领取成功！请在微信-钱包中查询。');window.location.href='http://www.cloudsbao.com/yidaoV1.app/wxredirect/wx_redpacket.aspx';</Script>");
                            return;
                        }
                        else
                        {
                            adminservice.AddLog("用户领取返现成功，添加记录失败，openid：" + openid, "，红包码：" + state+"，订单号："+redpacket.OrderCode, 12, 0);
                            return;
                        }
                    }
                    catch (Exception excption)
                    {
                        //错误处理
                        string message = excption.Message;
                        adminservice.AddLog("返现接口调取失败，openid：" + openid, "，红包码：" + state + "，订单号：" + redpacket.OrderCode+",error:"+ message, 12, 0);
                        context.Response.Write("<Script Language=JavaScript>alert('返现领取失败！请联系客服。');window.location.href='http://www.cloudsbao.com/yidaoV1.app/wxredirect/wx_redpacket.aspx';</Script>");
                        return;

                    }
                }
                
            }
        }

        public string GetOrderNumber()
        {
            string Tag = "BCWXT";
            string Number = DateTime.Now.ToString("yyMMddHHmmss");//yyyyMMddHHmmssms
            return Tag + Number + Next(1000, 1).ToString();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        private static int Next(int numSeeds, int length)
        {
            // Create a byte array to hold the random value.  
            byte[] buffer = new byte[length];
            // Create a new instance of the RNGCryptoServiceProvider.  
            System.Security.Cryptography.RNGCryptoServiceProvider Gen = new System.Security.Cryptography.RNGCryptoServiceProvider();
            // Fill the array with a random value.  
            Gen.GetBytes(buffer);
            // Convert the byte to an uint value to make the modulus operation easier.  
            uint randomResult = 0x0;//这里用uint作为生成的随机数  
            for (int i = 0; i < length; i++)
            {
                randomResult |= ((uint)buffer[i] << ((length - 1 - i) * 8));
            }
            // Return the random number mod the number  
            // of sides. The possible values are zero-based  
            return (int)(randomResult % numSeeds);
        }
    }
}