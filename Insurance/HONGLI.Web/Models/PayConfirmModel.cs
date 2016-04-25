using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Web.Models
{
    public class PayConfirmModel
    {
        public string OrderCode { get; set; }
        public string LicenseNo { get; set; }
        public decimal? DealPrice { get; set; }

    }
}
