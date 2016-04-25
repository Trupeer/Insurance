if (typeof global == "undefined")
{ var global = {}; }

var global = {

    
    site: "http://localhost:29274/",
    homePage: "http://localhost/yidao.hongli.me/ProductV2/Index?channel=1",
    domain: "",
    /*
    site: "http://www.cloudsbao.com/yidao.app/",
    domain: ".cloudsbao.com",
    	*/
    expire: 1,
    cookie: {
        order: {
            invoice: "HONGLI.order.invoice",
            deliver: "HONGLI.order.deliver",
            payAndDeliver: "HONGLI.order.payAndDeliver",
            policyHolder: "HONGLI.order.policyHolder",
            policyHolderSelected: "HONGLI.order.policyHolderSelected",
            product: "HONGLI.order.product"
        },
        user: "yidao.hongli.user",
    },
    user: function (mobile, channel, memberId) {
        this.mobile = mobile;
        this.channel = channel;
        this.memberId = memberId;
    },
    order: {
        deliverArriveAddDay: 5,//5天到达
        deliverPrice: 10,//10块
        //发票
        invoice: function (invoiceType, invoiceTitle) {
            this.invoiceType = invoiceType;// 1. 个人，2企业
            this.invoiceTitle = invoiceTitle;
        },
        //支付配送
        payAndDeliver: function (payType, deliverType, deliverPrice, selfTakeAddress, selfTakeDate) {
            this.payType = payType;//支付类型 1在线配送
            this.deliverType = deliverType;//配送类型 //1配送，2自取
            this.deliverPrice = deliverPrice;//配送价格
            this.selfTakeAddress = selfTakeAddress;//自取地址
            this.selfTakeDate = selfTakeDate;//自取时间
        },
        //配送信息
        deliver: function (deliverName, deliverMobile, deliverDistrictCode, deliverAddress) {
            this.deliverName = deliverName;
            this.deliverMobile = deliverMobile;
            this.deliverDistrictCode = deliverDistrictCode;
            this.deliverAddress = deliverAddress;
        },
        //被保人信息
        policyHolder: function (name, idcardType, idcard, idcardFacePicUrl, idcardBackPicUrl) {
            this.name = name;
            this.idcardType = idcardType;//1.身份正，2组织机构代码
            this.idcard = idcard;
            this.idcardFacePicUrl = idcardFacePicUrl;
            this.idcardBackPicUrl = idcardBackPicUrl;
        },
        //产品
        product: function (channel, productId, isuranceLogo, name, title, originalPrice, dealPrice, LicenseNo, forceExpireDate, businessExpireDate, buid, PrepaidAmount) {
            this.channel = channel;//渠道
            this.productId = productId;//产品id
            this.isuranceLogo = isuranceLogo;//保险logo,有一个保险的枚举类别，识别是哪个枚举值，我在订单页面显示不同的logo图片
            this.name = name;//产品名
            this.title = title;//产品展示文本
            this.originalPrice = originalPrice;//原始价格
            this.dealPrice = dealPrice;//成交价格
            this.LicenseNo = LicenseNo;//车牌号            
            this.forceExpireDate = forceExpireDate;//交强险到期时间
            this.businessExpireDate = businessExpireDate;//商业险到期时间
            this.buid = buid;//
            this.PrepaidAmount = PrepaidAmount;
        }
    }
}


