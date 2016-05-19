using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Entity
{
    /// <summary>
    /// bihu 接口返回值数据 第二版Version2 20160321
    /// </summary>
    public class BihuApiData
    {
    }

    #region 接口1-获取用户续保信息 20160321
    [Serializable]
    //接口1返回值对象
    public class UserInsuranceInfoResultV2
    {
        /* 1:获取续保信息成功,
            2:需要完善行驶证信息，
            3.获取用户信息成功，但报价失败
            -10000：输入的参数是否有空或者长度不符合要求；
            ？？请提供各入参长度验证标准
            -10001：校验参数错误；
            -10002:获取续保信息失败；
            -10003:服务器发生异常
        */
        public int BusinessStatus { get; set; }
        //信息描述
        public string StatusMessage { get; set; }
        public User UserInfo { get; set; }
        public Quote SaveQuote { get; set; }
        //用户标识（20160329新增）
        public string CustKey { get; set; }
        //用户id
        public int UserId { get; set; }
    }
    [Serializable]
    public class User
    {
        //使用性质：非营运、营运
        public string CarUsedType { get; set; }
        //车牌号
        public string LicenseNo { get; set; }
        //车主姓名
        public string LicenseOwner { get; set; }
        //投保人（20160323新增）
        public string PostedName { get; set; }
        //被保险人（20160323新增）
        public string InsuredName { get; set; }
        //新车购置价格（20160323新增）
        public double PurchasePrice { get; set; }
        //证件类型：身份类型
        public string IdType { get; set; }
        //证件号码
        public string CredentislasNum { get; set; }
        //城市Id，北京10
        public int CityCode { get; set; }
        //发动机号
        public string EngineNo { get; set; }
        //品牌型号
        public string ModleName { get; set; }
        //车辆注册日期
        public string RegisterDate { get; set; }
        //车辆识别代号
        public string CarVin { get; set; }
        //交强险到期时间
        public string ForceExpireDate { get; set; }
        //商业险到期时间
        public string BusinessExpireDate { get; set; }
        //座位数量（20160324新增） 
        public int SeatCount { get; set; }

    }
    [Serializable]
    public class Quote
    {
        //车险来源：0：平安 1：太平洋 2：人保
        public int Source { get; set; }
        //车损保额
        public double CheSun { get; set; }
        //第三方责任险保额
        public double SanZhe { get; set; }
        //全车盗抢保险保额
        public double DaoQiang { get; set; }
        //车上人员责任险（司机）保额
        public double SiJi { get; set; }
        //车上人员责任险（乘客）保额
        public double ChengKe { get; set; }
        //玻璃单独破碎险保额，0-不投保，1国产，2进口
        public double BoLi { get; set; }
        //车身划痕损失险保额
        public double HuaHen { get; set; }
        //不计免赔险（车损）保额
        public double BuJiMianCheSun { get; set; }
        //不计免赔险（三者）保额
        public double BuJiMianSanZhe { get; set; }
        //不计免赔险（盗抢）保额
        public double BuJiMianDaoQiang { get; set; }
        //不计免赔险（车上人员）保额
        public double BuJiMianRenYuan { get; set; }
        //不计免赔险（附加险）保额
        public double BuJiMianFuJia { get; set; }
        //涉水行驶损失险保额
        public double SheShui { get; set; }
        //倒车镜、车灯单独损坏险保额
        public double CheDeng { get; set; }
        //自燃损失险保额
        public double ZiRan { get; set; }
    }


    #endregion

    #region 接口2-获取核保/报价信息 20160321

    /// <summary>
    /// 接口入参对象
    /// </summary>
    [Serializable]
    public class UserPrecisePriceIncomingParamsModelV2
    {
        //车牌号
        public string LicenseNo { get; set; }
        //是否对单个保险公司核保，1=是，0=否
        public int IsSingleSubmit { get; set; }
        //意向投保公司(-1:只报价不核保、0:平安、1:太平洋、2:人保)
        public int IntentionCompany { get; set; }
        //车辆类型：0客车，1货车
        public int CarType { get; set; }
        //是否新车（0：否；1：新车）
        public int IsNewCar { get; set; }
        //使用性质（0营运、非营运）
        public int CarUsedType { get; set; }
        //城市Id（目前系统只支持北京地区）（北京:10）
        public int CityCode { get; set; }
        //发动机号
        public string EngineNo { get; set; }
        //车架号
        public string CarVin { get; set; }
        //注册日期
        public string RegisterDate { get; set; }
        //品牌型号
        public string MoldName { get; set; }
        //交强险+车船税(1:报价交强车船，0：不报价交强车船) 
        public int ForceTax { get; set; }
        //商业险到期时间
        public string BizStartDate { get; set; }
        //玻璃单独破碎险，0-不投保，1国产，2进口
        public double BoLi { get; set; }
        //不计免赔险(车损) ，0-不投保，1投保
        public double BuJiMianCheSun { get; set; }
        //不计免赔险(盗抢) ，0-不投保，1投保
        public double BuJiMianDaoQiang { get; set; }
        //不计免赔险(附加险) ，0-不投保，1投保
        public double BuJiMianFuJia { get; set; }
        //不计免赔险(车上人员) ，0-不投保，1投保
        public double BuJiMianRenYuan { get; set; }
        //不计免赔险(三者) ，0-不投保，1投保
        public double BuJiMianSanZhe { get; set; }
        //倒车镜、车灯单独损坏险，0-不投保，1-国产，2-进口
        public double CheDeng { get; set; }
        //涉水行驶损失险，0-不投保，1投保
        public double SheShui { get; set; }
        //车身划痕损失险，0-不投保，>0投保(具体金额)（2，000；5，000；10，000；20，000）
        public double HuaHen { get; set; }
        //车上人员责任险(司机) ，0-不投保，>0投保(具体金额）（10，000；20，000；30，000；40，000；50，000；100，000；200，000）   
        public double SiJi { get; set; }
        //车上人员责任险(乘客) ，0-不投保，>0投保(具体金额)（10，000；20，000；30，000；40，000；50，000；100，000；200，000）
        public double ChengKe { get; set; }
        //机动车损失保险，0-不投保，>0投保
        public double CheSun { get; set; }
        //全车盗抢保险，0-不投保，>0投保
        public double DaoQiang { get; set; }
        //第三者责任保险，0-不投保，>0投保(具体金额)（50，000；100，000；150，000；200，000；300，000；500，000；1，000，000；1，500，000）
        public double SanZhe { get; set; }
        //自燃损失险，0-不投保，1投保
        public double ZiRan { get; set; }
        //用户标识（20160329新增）
        public string CustKey { get; set; }
        public string CustKeyMd5 { get; set; }

        //public int Agent { get; set; }
        //public string SecCode { get; set; }
    }

    [Serializable]
    //接口2返回值对象
    public class UserPrecisePriceV2
    {
        /*
            1:请求成功
            -10000：输入的参数是否有空或者长度不符合要求；
            -10001：校验参数错误；
            -10002:请求报价信息失败；
        */
        public int BusinessStatus { get; set; }
        public string StatusMessage { get; set; }
    }

    #endregion

    #region 接口3-获取报价信息 20160323
    [Serializable]
    public class PrecisePriceResultV2
    {
        /*1:请求成功
        2:需要完善行驶证
        -10000：输入的参数是否有空或者长度不符合要求；
        -10001：校验参数错误；
        -10002:获取信息失败；
        -10003：服务器发生异常
        */
        public int BusinessStatus { get; set; }
        public string StatusMessage { get; set; }
        public User UserInfo { get; set; }
        public PrecisePriceItem Item { get; set; }
        //用户标识（20160329新增）
        public string CustKey { get; set; }
    }

    [Serializable]
    public class PrecisePriceItem
    {
        public int Id { get; set; }
        //保险资源（0：平安 2人保 1 太平洋）
        public int Source { get; set; }
        //商业险费率
        public double BizRate { get; set; }
        //交强车船费率
        public double ForceRate { get; set; }
        //商业险总额
        public double BizTotal { get; set; }
        //交强险总额
        public double ForceTotal { get; set; }
        //车船税总额
        public double TaxTotal { get; set; }
        //报价状态，-1=未报价， 0=报价失败，>0报价成功
        public int QuoteStatus { get; set; }
        //报价信息
        public string QuoteResult { get; set; }
        //

        //车损保额
        public CheSunV2 CheSun { get; set; }
        //第三方责任险保额
        public SanZhe SanZhe { get; set; }
        //全车盗抢保险保额
        public DaoQiangV2 DaoQiang { get; set; }
        //车上人员责任险（司机）保额
        public SiJiV2 SiJi { get; set; }
        //车上人员责任险（乘客）保额
        public ChengKeV2 ChengKe { get; set; }
        //玻璃单独破碎险保额
        public BoLiV2 BoLi { get; set; }
        //车身划痕损失险保额
        public HuaHenV2 HuaHen { get; set; }
        //不计免赔险（车损）保额
        public BuJiMianCheSunV2 BuJiMianCheSun { get; set; }
        //不计免赔险（三者）保额
        public BuJiMianSanZheV2 BuJiMianSanZhe { get; set; }
        //不计免赔险（盗抢）保额
        public BuJiMianDaoQiangV2 BuJiMianDaoQiang { get; set; }
        //不计免赔险（车上人员）保额
        public BuJiMianRenYuanV2 BuJiMianRenYuan { get; set; }
        //不计免赔险（附加险）保额
        public BuJiMianFuJiaV2 BuJiMianFuJia { get; set; }
        //涉水行驶损失险保额
        public SheShuiV2 SheShui { get; set; }
        //倒车镜、车灯单独损坏险保额
        public CheDengV2 CheDeng { get; set; }
        //自燃损失险保额
        public ZiRanV2 ZiRan { get; set; }

        //获取续保接口生成的Id（20160324新增）
        public int BuId { get; set;  }

        //自定义
        public string Description { get; set; }
    }

    #region Result对象中含有保额、保费的对象
    [Serializable]
    public class CheSunV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    [Serializable]
    public class SanZheV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    [Serializable]
    public class DaoQiangV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    [Serializable]
    public class SiJiV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //车上人员责任险（乘客）保额、保额
    [Serializable]
    public class ChengKeV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //玻璃单独破碎险保额、保费
    [Serializable]
    public class BoLiV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //车身划痕损失险保额、保费
    [Serializable]
    public class HuaHenV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }

    //不计免赔险（车损）
    [Serializable]
    public class BuJiMianCheSunV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //不计免赔险（三者）
    [Serializable]
    public class BuJiMianSanZheV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //不计免赔险（盗抢）
    [Serializable]
    public class BuJiMianDaoQiangV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //不计免赔险（车上人员）
    [Serializable]
    public class BuJiMianRenYuanV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //不计免赔险（附加险）
    [Serializable]
    public class BuJiMianFuJiaV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }

    //涉水行驶损失险保额、保费
    [Serializable]
    public class SheShuiV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //倒车镜、车灯单独损坏险保额、保费
    [Serializable]
    public class CheDengV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //自燃损失险保额、保费
    [Serializable]
    public class ZiRanV2
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }

    #endregion

    #endregion

    #region 接口4-获取核保信息 20160323
    [Serializable]
    public class SubmitInfoResultV2
    {
        /*1:获取核保信息成功
        -10000：输入的参数是否有空或者长度不符合要求；
        -10001：校验参数错误；
        -10002：获取核保消息失败
        -10003：服务器发生异常
        */
        public int BusinessStatus { get; set; }
        public string StatusMessage { get; set; }
        public SubmitItem Item { get; set; }
        //用户标识（20160329新增）
        public string CustKey { get; set; }
    }
    [Serializable]
    public class SubmitItem
    {
        //保险资源(0:平安，1太平洋 2：人保)
        public int Souce { get; set; }
        //核保状态 （核保状态，0=核保失败，1=核保成功,2=未核保,3=核保中）
        public int SubmitStatus { get; set; }
        //核保状态描述
        public string SubmitResult { get; set; }
        //商业险投保单号
        public string BizNo { get; set; }
        //交强险投保单号
        public string ForceNo { get; set; }
        //商业险费率（核保通过才会有值） (20160402新增)
        public double BizRate { get; set; }
        //交强车船险费率（核保通过之后才会有值）  (20160402新增)
        public double ForceRate { get; set; }
    }
    #endregion
}
