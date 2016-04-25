using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Entity
{
    public class PayAndDeliverEntity
    {
        public int? PayType { get; set; }
        public int? DeliverType { get; set; }
        public decimal? DeliverPrice { get; set; }        
        public string SelfTakeAddress { get; set; }
        public DateTime? SelfTakeDate { get; set; }
    }
}
