using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Entity
{
    public class BihuData
    {
    }

    #region 接口用model


    #region 接口1用-用户投保返回值

    public class UserReInsuranceResult
    {
        /*  错误代码
            0:代表成功；
            -1000001：手机号不能为空；
            -1000002：agent配置信息不能为空；
            -1000003:secCode信息不能为空；
            -1000004:请求信息被拒绝(md5值不匹配)；
            -1000005:参数信息不正确，请配置相关信息(agent参数不正确，请联系壁虎研发)
        */
        public int ErrCode { get; set; }
        //错误信息
        public string ErrMsg { get; set; }
        //public string UserClaim { get; set; }

        public UserInfo UserInfo { get; set; }
        //列表
        public List<Result> Result { get; set; }
    }


    #region UserInfo 用户车辆信息
    public class UserInfo
    {
        //车牌号
        public string LicenseNo { get; set; }
        //手机号
        public string Mobile { get; set; }
        //城市Id
        public int CityCode { get; set; }
        //发动机号
        public string EngineNo { get; set; }
        //品牌型号
        public string MoldName { get; set; }
        //车辆注册日期
        public string RegisterDate { get; set; }
        //车辆识别代号
        public string CarVin { get; set; }
        //意向投保公司
        public int Source { get; set; }
        //往年投保公司
        public int LastYearSource { get; set; }
        //报价状态（报价状态，-1=未报价，0=报价失败，>0=报价成功）
        public int QuoteStatus { get; set; }
        //交强险到期时间
        public string ForceExpireDate { get; set; }
        //商业险到期时间
        public string BusinessExpireDate { get; set; }
    }
    #endregion

    #region Result 用户投保信息
    public class Result
    {
        public int Id { get; set; }
        //试算ID
        public int BuId { get; set; }
        //商业险总额
        public double BizTotal { get; set; }
        //交强险总额
        public double ForceTotal { get; set; }
        //车船税总额
        public double TaxTotal { get; set; }
        //0：平安 1：太平洋 2：人保
        public int Source { get; set; }
        public string CreateTime { get; set; }
        //核保状态（0：失败；1：成功；2：未核保）
        public int SubmitStatus { get; set; }
        //核保信息
        public string SubmitResult { get; set; }
        //报价状态，0=报价失败，1=报价成功
        public int QuoteStatus { get; set; }
        //报价信息
        public string QuoteResult { get; set; }
        //到期时间
        public string ExpireDate { get; set; }


        //车损保额
        public CheSun CheSun { get; set; }
        //第三方责任险保额
        public SanZhe SanZhe { get; set; }
        //全车盗抢保险保额
        public DaoQiang DaoQiang { get; set; }
        //车上人员责任险（司机）保额
        public SiJi SiJi { get; set; }
        //车上人员责任险（乘客）保额
        public ChengKe ChengKe { get; set; }
        //玻璃单独破碎险保额
        public BoLi BoLi { get; set; }
        //车身划痕损失险保额
        public HuaHen HuaHen { get; set; }
        //不计免赔险（车损）保额
        public BuJiMianCheSun BuJiMianCheSun { get; set; }
        //不计免赔险（三者）保额
        public BuJiMianSanZhe BuJiMianSanZhe { get; set; }
        //不计免赔险（盗抢）保额
        public BuJiMianDaoQiang BuJiMianDaoQiang { get; set; }
        //不计免赔险（车上人员）保额
        public BuJiMianRenYuan BuJiMianRenYuan { get; set; }
        //不计免赔险（附加险）保额
        public BuJiMianFuJia BuJiMianFuJia { get; set; }
        //涉水行驶损失险保额
        public SheShui SheShui { get; set; }
        //倒车镜、车灯单独损坏险保额
        public CheDeng CheDeng { get; set; }
        //自燃损失险保额
        public ZiRan ZiRan { get; set; }

        //自定义
        public string Description { get; set; }
    }

    #endregion

    #region Result对象中含有保额、保费的对象
    public class CheSun
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    public class SanZhe
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    public class DaoQiang
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    public class SiJi
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //车上人员责任险（乘客）保额、保额
    public class ChengKe
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //玻璃单独破碎险保额、保费
    public class BoLi
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //车身划痕损失险保额、保费
    public class HuaHen
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }

    //不计免赔险（车损）
    public class BuJiMianCheSun
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //不计免赔险（三者）
    public class BuJiMianSanZhe
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //不计免赔险（盗抢）
    public class BuJiMianDaoQiang
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //不计免赔险（车上人员）
    public class BuJiMianRenYuan
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //不计免赔险（附加险）
    public class BuJiMianFuJia
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }

    //涉水行驶损失险保额、保费
    public class SheShui
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //倒车镜、车灯单独损坏险保额、保费
    public class CheDeng
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    //自燃损失险保额、保费
    public class ZiRan
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }

    #endregion

    #endregion


    #region 接口2用

    //入参对象
    public class UserPrecisePriceInComingParamModel
    {
        //手机号
        public string mobile { get; set; }
        //车牌号
        public string carLicense { get; set; }
        //意向投保公司(-1:只报价不核保、0:平安、1:太平洋、2:人保)
        public string intentionCompany { get; set; }
        //用户标识(微信平台：openid；app平台：””)，本平台传空值    
        public string userIdentity { get; set; }
        //私家车/公用车（0：私家车；1：公用车）【重点】
        public int carType { get; set; }
        //是否新车（0：否；1：新车）
        public int isNewCar { get; set; }
        //营运/非营运（0：非营运；1：营运）
        public int useType { get; set; }
        //城市Id（目前系统只支持北京地区）（北京:110000）
        public string citycode { get; set; }
        //发动机号
        public string engineNo { get; set; }
        //车架号
        public string carVin { get; set; }
        //注册日期
        public string registerDate { get; set; }
        //品牌型号
        public string moldName { get; set; }
        //玻璃单独破碎险，0-不投保，1国产，2进口
        public double boli { get; set; }
        //不计免赔险(车损) ，0-不投保，1投保
        public double bujimianchesun { get; set; }
        //不计免赔险(盗抢) ，0-不投保，1投保
        public double bujimiandaoqiang { get; set; }
        //不计免赔险(附加险) ，0-不投保，1投保
        public double bujimianfujia { get; set; }
        //不计免赔险(车上人员) ，0-不投保，1投保
        public double bujimianrenyuan { get; set; }
        //不计免赔险(三者) ，0-不投保，1投保
        public double bujimiansanzhe { get; set; }
        //倒车镜、车灯单独损坏险，0-不投保，1-国产，2-进口
        public double chedeng { get; set; }
        //涉水行驶损失险，0-不投保，1投保
        public double sheshui { get; set; }
        //车身划痕损失险，0-不投保，>0投保(具体金额)（2，000；5，000；10，000；20，000）
        public double huahen { get; set; }
        //车上人员责任险(司机) ，0-不投保，>0投保(具体金额）（10，000；20，000；30，000；40，000；50，000；100，000；200，000）     
        public double siji { get; set; }
        //车上人员责任险(乘客) ，0-不投保，>0投保(具体金额)（10，000；20，000；30，000；40，000；50，000；100，000；200，000）
        public double chengke { get; set; }
        //机动车损失保险，0-不投保，>0投保
        public double chesun { get; set; }
        //全车盗抢保险，0-不投保，>0投保
        public double daoqiang { get; set; }
        //第三者责任保险，0-不投保，>0投保(具体金额)（50，000；100，000；150，000；200，000；300，000；500，000；1，000，000；1，500，000）
        public double sanzhe { get; set; }
        //自燃损失险，0-不投保，1投保
        public double ziran { get; set; }        
    }

    #endregion

    #endregion
}
