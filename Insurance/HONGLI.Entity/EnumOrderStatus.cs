using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using I.Utility.Extensions;
namespace HONGLI.Entity
{
    public enum EnumOrderStatus
    {
        /// <summary>
        /// 0 未确认
        /// </summary>
        [AttachData(EnmuRemark.Text, "未确认")]
        NAK = 0,

        /// <summary>
        /// 1 已订未支付
        /// </summary>
        [AttachData(EnmuRemark.Text, "已订未支付")]
        UnPay = 1,

        /// <summary>
        /// 2上门收款
        /// </summary>
        [AttachData(EnmuRemark.Text, "上门收款")]
        Homecollection = 2,

        /// <summary>
        /// 3 已支付
        /// </summary>
        [AttachData(EnmuRemark.Text, "已支付")]
        Paid = 3,

        /// <summary>
        /// 4 邮寄保单
        /// </summary>
        [AttachData(EnmuRemark.Text, "邮寄保单")]
        Mailingpolicy = 4,

        /// <summary>
        /// 5 已完成
        /// </summary>
        [AttachData(EnmuRemark.Text, "已完成")]
        Success = 5,
        /// <summary>
        /// 6 失败
        /// </summary>
        [AttachData(EnmuRemark.Text, "失败")]
        Failed = 6,
        

        /// <summary>
        /// 9 已取消
        /// </summary>
        [AttachData(EnmuRemark.Text, "已取消")]
        Cancel = 9


    }
}
