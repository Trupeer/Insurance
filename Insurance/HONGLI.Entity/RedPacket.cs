//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace HONGLI.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class RedPacket
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string RedPacketName { get; set; }
        public decimal RedPacketMoney { get; set; }
        public string RedPacketRemark { get; set; }
        public int State { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateName { get; set; }
        public string RedPacketCode { get; set; }
        public Nullable<System.DateTime> SendTime { get; set; }
        public string OpenId { get; set; }
        public string WX_UserInfo { get; set; }
        public string BCNo { get; set; }
    }
}