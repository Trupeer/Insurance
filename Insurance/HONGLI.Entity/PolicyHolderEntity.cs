using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Entity
{
    /// <summary>
    /// 被保人信息
    /// </summary>
    public class PolicyHolderEntity
    {        
        public string Name { get; set; }
        public int? IdcardType { get; set; }
        public string Idcard { get; set; }
        public string IdcardFacePicUrl { get; set; }
        public string IdcardBackPicUrl { get; set; }

    }
}
