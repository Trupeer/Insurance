using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Entity
{
    /// <summary>
    /// 产品
    /// </summary>
    public class ProductEntity
    {
        public int? Channel { get; set; }
        public string ProductId { get; set; }
        public int? IsuranceLogo { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? DealPrice { get; set; }
        public string LicenseNo { get; set; }
        public DateTime? ForceExpireDate { get; set; }
        public DateTime? BusinessExpireDate { get; set; }
        public int? Buid { get; set; }
    }
}
