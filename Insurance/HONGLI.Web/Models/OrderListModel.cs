using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Web.Models
{
    public class OrderListModel
    {
        public string OrderCode { get; set; }
        public decimal? AmountPayable { get; set; }    
        public decimal? PaidAmount { get; set; }
        public int? OrderStatus { get; set; }
        public string LicenseNo { get; set; }
        public int? InsuranceLogo { get; set; }
        public DateTime? ForceExpireDate { get; set; }
        public DateTime? BusinessExpireDate { get; set; }

    }
}
