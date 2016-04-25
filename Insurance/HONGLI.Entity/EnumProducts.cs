using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Entity
{
    //class EnumProducts
    //{
    //}

    //平台
    public enum Channel
    {
        Wap = 1 //本平台（默认值）
    }

    //车辆使用性质
    public enum CarUsedType
    {
        Operation = 1,   //非营运
        NotOperation = 0  //营运 
    }

    //公司标识： 0-平安 1-太平洋 2-人保  20160329
    public enum ProductCompany
    {
        PingAn = 0,
        TaiPingYang = 1,
        RenBao = 2
    }

    //核保状态0=核保失败，1=核保成功,2=未核保,3=核保中
    public enum SubmitStatus
    {
        Fail = 0,
        Success = 1,
        UnCheck = 2,
        Checking = 3
    }
}
