<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wx_redpacket.aspx.cs" Inherits="HONGLI.Web.WXRedirect.WX_Redirect" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="../Scripts/jquery-1.10.2.min.js"></script>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="../public/css/all.css" rel="stylesheet" />
    <script src="../Scripts/base.js"></script>
    <script src="../Scripts/jquery/jquery-2.1.3.min.js"></script>
    <script src="../Scripts/all.js"></script>
    <script src="../Scripts/hongli.common.tools.js"></script>
    <script src="../Scripts/hongli.global.data.js"></script>
    <style>
    </style>
</head>
<%--<body>
    <form id="form1" runat="server">
    <div>
        <input id="txt_redPacketCode" type="text" />
        <input id="btn_getMoney" type="button" value="领取返现" onclick="_go()" />
    </div>
    </form>
</body>--%>

    <body>
        <div style="background: url(../Images/weixinbackground.png) no-repeat ; background-size:100% 100%;">
            <div style="width:12rem;height:21.3rem">
            </div>
        </div>
        <div style="margin-left:1.5rem;top:12rem;position:absolute;z-index:999;">
            <p style="color:white;font-size:.620rem; text-align:center;font-weight:600;">输入红包密令即可领取红包</p>
            <br />
            <br />
            <input id="txt_redPacketCode" type="text" class="f-input01" placeholder="请输入返现提取码" />
            <br />
            <br />
                <a href="#" onclick="_go()"  class="f-button05">领取返现</a>

            </div>
   </body>
</html>
<script type="text/javascript">
    function _go()
    {
        var redPacketCode = $("#txt_redPacketCode").val();
        if (redPacketCode.length == 0) {
            alert("请输入返现码！");
            return;
        }
        else {
            location.href = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx9714ac1c7646c25b&redirect_uri=http%3a%2f%2fwww.cloudsbao.com%2fyidaoV1.app%2fwxredirect%2fuserauth.ashx&response_type=code&scope=snsapi_userinfo&state=" + redPacketCode + "#wechat_redirect";
        }
    }
</script>
