/*----------------------------------------------------------------
* 系统名称：         
* 变更履历：         
* 版本，	日期，	    操作人，	描述 

----------------------------------------------------------------*/
if (typeof common == "undefined")
{ var common = {}; }


common.url = {
    enCode: function (code) {
        return encodeURIComponent(code);
        //return escape(code);
    },
    deCode: function (code) {
        return decodeURIComponent(code);
        //return unescape(code);
    },

    Request: function (key) {
        key = key.toLowerCase();
        var url = location.href.toLowerCase();
        var keyValues = url.replace("?", "?&").split("&");
        var value = "";
        for (i = 1; i < keyValues.length; i++) {
            if (keyValues[i].indexOf(key + "=") == 0)
                value = keyValues[i].replace(key + "=", "");
        }
        return value;
    },
    //para_name 参数名称 para_value 参数值 url所要更改参数的网址
    setUrlParam: function (para_name, para_value, url) {
        var strNewUrl = new String();
        var strUrl = url;
        //alert(strUrl);
        if (strUrl.indexOf("?") != -1) {
            strUrl = strUrl.substr(strUrl.indexOf("?") + 1);
            //alert(strUrl);
            if (strUrl.toLowerCase().indexOf(para_name.toLowerCase()) == -1) {
                strNewUrl = url + "&" + para_name + "=" + para_value;
                return strNewUrl;
            } else {
                var aParam = strUrl.split("&");
                //alert(aParam.length);
                for (var i = 0; i < aParam.length; i++) {
                    if (aParam[i].substr(0, aParam[i].indexOf("=")).toLowerCase() == para_name.toLowerCase()) {
                        aParam[i] = aParam[i].substr(0, aParam[i].indexOf("=")) + "=" + para_value;
                    }
                }

                strNewUrl = url.substr(0, url.indexOf("?") + 1) + aParam.join("&");
                // alert(strNewUrl);
                return strNewUrl;
            }

        } else {
            strUrl += "?" + para_name + "=" + para_value;
            //alert(strUrl);
            return strUrl
        }
    }

}


common.cookie = {
    getCookie: function (name) {
        var cookieValue = "";
        var search = name + "=";
        if (document.cookie.length > 0) {
            offset = document.cookie.indexOf(search);
            if (offset != -1) {
                offset += search.length;
                end = document.cookie.indexOf(";", offset);
                if (end == -1) end = document.cookie.length;
                cookieValue = common.url.deCode(document.cookie.substring(offset, end))
            }
        }
        return cookieValue;
    },


    //跨二级域设置cookie,domain 写为一级域名，则二级域名下的网站可共享cookie
    setCookie: function (domain, cookieName, cookieValue, DayValue) {

        var expire = "";
        var day_value = 1;
        if (DayValue != null) {
            day_value = DayValue;
        }
        expire = new Date((new Date()).getTime() + day_value * 86400000);
        expire = "; expires=" + expire.toGMTString();

        var url = window.location.href;
        if (url.indexOf("localhost") > 0) {
            document.cookie = cookieName + "=" + common.url.enCode(cookieValue) + ";path=/" + expire;
        }
        else {
            document.cookie = cookieName + "=" + common.url.enCode(cookieValue) + expire + ";path=/;domain=" + domain;
        }
    },


    delCookie: function (cookieName, domain) {
        var expire = "";
        expire = new Date((new Date()).getTime() - 1);
        expire = "; expires=" + expire.toGMTString();

        var url = window.location.href;
        if (url.indexOf("localhost") > 0) {
            document.cookie = cookieName + "=" + common.url.enCode("") + expire + ";path=/";
        }
        else {
            document.cookie = cookieName + "=" + common.url.enCode("") + expire + ";path=/;domain=" + domain;
        }
        //path=/
    }
}

// 文件上传
common.upload = {
    validate: function (filepath, filesize) {
        var extStart = filepath.lastIndexOf(".");
        var ext = filepath.substring(extStart, filepath.length).toUpperCase();
        if (ext != ".BMP" && ext != ".PNG" && ext != ".JPG" && ext != ".JPEG") {
            alert("只能上传 bmp，png，jpeg，jpg 格式的图片！");
            return false;
        }
        var img = new Image();
        img.src = filepath;
        if (img.fileSize > 0) {
            console.log(img.fileSize);
            if (img.fileSize > filesize) {
                alert("上传失败");
                return false;
            }
        }
        alert("上传成功");
        return true;
    }
}

//add by Lee 20160403
common.operateNum = {
    formatNum: function formatNum(num, n) {//参数说明：num 要格式化的数字 n 保留小数位  
        num = String(num.toFixed(n));
        var re = /(-?\d+)(\d{3})/;
        while (re.test(num)) num = num.replace(re, "$1,$2")

        if (num % 1 == 0) {
            num = num / 1;
        } else {
            if (num.substr(num.length - 1, num.length) == 0) {
                num = num.substr(0, num.length - 1);
            }

            if (num.substr(num.length - 1, num.length) == 0) {
                num = num.substr(0, num.length - 1);
            }

            if (num.substr(num.length - 1, num.length) == 0) {
                num = num.substr(0, num.length - 1);
            }
        }
        return num;
    }

}