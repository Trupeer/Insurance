<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="HONGLI.Web.WXRedirect.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script>
        $(function () {
            location.href = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxb23c6fac97818318&redirect_uri=http%3a%2f%2fwww.cloudsbao.com%2fyidaoV1.app%2fwxredirect%2ftest.ashx&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    </div>
    </form>
</body>
</html>
