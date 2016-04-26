using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Web.Models
{
    public class OrderDetailModels
    {
        public string OrderCode { get; set; }
        public int? DeliverType { get; set; }
        public string DeliverName { get; set; }
        public string DeliverMobile { get; set; }
        public string DeliverAddress { get; set; }
        public decimal? DeliverPrice { get; set; }
        public DateTime? DeliverTime { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductTitle { get; set; }
        public decimal? ProductOriginalPrice { get; set; }
        public decimal? ProductDealPrice { get; set; }
        public string PolicyHolderName { get; set; }
        public string PolicyHolderIdcard { get; set; }
        public int? PolicyHolderIdcardType { get; set; }
        public int? InvoiceType { get; set; }
        public string InvoiceTitle { get; set; }
        public int? PayType { get; set; }
        public int? InsuranceLogo { get; set; } 
        public decimal? AmountPayable { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? OrderStatus { get; set; }
        public string LicenseNo { get; set; }
        public DateTime? ForceExpireDate { get; set; }

        public DateTime? BusinessExpireDate { get; set; }
        public decimal? PrepaidAmount { get; set; }
    }
}
