using HONGLI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HONGLI.Web.Models
{
    public class ProductV2UserItemPolicyModels
    {
        public int Id { get; set; }
        public string CarUsedType { get; set; }
        public string LicenseNo { get; set; }
        public string LicenseOwner { get; set; }
        public string PostedName { get; set; }
        public string InsuredName { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public string IdType { get; set; }
        public string CredentislasNum { get; set; }
        public Nullable<int> CityCode { get; set; }
        public string EngineNo { get; set; }
        public string ModleName { get; set; }
        public string RegisterDate { get; set; }
        public string CarVin { get; set; }
        public string ForceExpireDate { get; set; }
        public string BusinessExpireDate { get; set; }
        public Nullable<int> SeatCount { get; set; }
        public string Mobile { get; set; }
        public Nullable<byte> Channel { get; set; }
        public string CustomerKey { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string Creator { get; set; }
        public string Memo { get; set; }
        public Nullable<int> BeforeVisitStatus { get; set; }
        public Nullable<int> VisitStatus { get; set; }
        public Nullable<System.DateTime> VisitDate { get; set; }
        public Nullable<int> VisitServiceUserId { get; set; }
        public int ItemId { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> ItemUserId { get; set; }
        public Nullable<int> ItemSource { get; set; }
        public Nullable<decimal> BizRate { get; set; }
        public Nullable<decimal> BizTotal { get; set; }
        public Nullable<decimal> ForceRate { get; set; }
        public Nullable<decimal> ForceTotal { get; set; }
        public Nullable<decimal> TaxRate { get; set; }
        public Nullable<decimal> TaxTotal { get; set; }
        public Nullable<int> QuoteStatus { get; set; }
        public string QuoteResult { get; set; }
        public Nullable<decimal> CheSun_BaoE { get; set; }
        public Nullable<decimal> CheSun_BaoFei { get; set; }
        public Nullable<decimal> SanZhe_BaoE { get; set; }
        public Nullable<decimal> SanZhe_BaoFei { get; set; }
        public bool DaoQiang_BaoE { get; set; }
        public Nullable<decimal> DaoQiang_BaoFei { get; set; }
        public Nullable<decimal> SiJi_BaoE { get; set; }
        public Nullable<decimal> SiJi_BaoFei { get; set; }
        public Nullable<decimal> ChengKe_BaoE { get; set; }
        public Nullable<decimal> ChengKe_BaoFei { get; set; }
        public Nullable<decimal> BoLi_BaoE { get; set; }
        public Nullable<decimal> BoLi_BaoFei { get; set; }
        public Nullable<decimal> HuaHen_BaoE { get; set; }
        public Nullable<decimal> HuaHen_BaoFei { get; set; }
        public bool BuJiMianCheSun_BaoE { get; set; }
        public Nullable<decimal> BuJiMianCheSun_BaoFei { get; set; }
        public bool BuJiMianSanZhe_BaoE { get; set; }
        public Nullable<decimal> BuJiMianSanZhe_BaoFei { get; set; }
        public bool BuJiMianDaoQiang_BaoE { get; set; }
        public Nullable<decimal> BuJiMianDaoQiang_BaoFei { get; set; }
        public bool BuJiMianRenYuan_BaoE { get; set; }
        public Nullable<decimal> BuJiMianRenYuan_BaoFei { get; set; }
        public bool BuJiMianFuJia_BaoE { get; set; }
        public Nullable<decimal> BuJiMianFuJia_BaoFei { get; set; }
        public bool SheShui_BaoE { get; set; }
        public Nullable<decimal> SheShui_BaoFei { get; set; }
        public bool CheDeng_BaoE { get; set; }
        public Nullable<decimal> CheDeng_BaoFei { get; set; }
        public bool ZiRan_BaoE { get; set; }
        public Nullable<decimal> ZiRan_BaoFei { get; set; }
        public Nullable<int> BuId { get; set; }
        public Nullable<byte> SubmitStatus { get; set; }
        public string SubmitResult { get; set; }
        public string BizNo { get; set; }
        public string ForceNo { get; set; }
        public Nullable<decimal> BizRate_Channel { get; set; }
        public Nullable<decimal> ForceRate_Channel { get; set; }
        public Nullable<decimal> TaxRate_Channel { get; set; }
        public Nullable<decimal> BizAfterCoupon { get; set; }
        public Nullable<decimal> ForceAfterCoupon { get; set; }
        public Nullable<decimal> TaxAfterCoupon { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> TotalAfterCoupon { get; set; }
        public Nullable<int> ServiceUserId { get; set; }
        public Nullable<int> AuditOrderStatus { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> ItemCreateTime { get; set; }
        public Nullable<bool> UserSelection { get; set; }
        public Nullable<decimal> PrepaidAmount { get; set; }

        public int OrderBaseId { get; set; }
        public string OrderBaeOrderCode { get; set; }
        public Nullable<System.DateTime> OrderBaeCreateDate { get; set; }
        public Nullable<decimal> AmountPayable { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<System.DateTime> PayTime { get; set; }
        public Nullable<int> Status { get; set; }
        public string TradeNo { get; set; }
        public Nullable<int> OrderBaeChannel { get; set; }
        public string OrderBaeUserId { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public string InvoiceTitle { get; set; }

        public int OrderDeliverId { get; set; }
        public string OrderDeliverOrderCode { get; set; }
        public Nullable<int> DeliverType { get; set; }
        public string DeliverAddress { get; set; }
        public Nullable<System.DateTime> DeliverTime { get; set; }
        public Nullable<decimal> DeliverPrice { get; set; }
        public string DeliverName { get; set; }
        public Nullable<System.DateTime> OrderDeliverCreateDate { get; set; }
        public string DeliverMobile { get; set; }
        public string DeliverDistrictCode { get; set; }
        public Nullable<int> OrderDeliverUserId { get; set; }

        public int OrderItemId { get; set; }
        public string OrderItemOrderCode { get; set; }
        public Nullable<System.DateTime> OrderItemCreateDate { get; set; }
        public string OrderItemProductId { get; set; }
        public string OrderItemProductName { get; set; }
        public string OrderItemProductTitle { get; set; }
        public Nullable<decimal> ProductOriginalPrice { get; set; }
        public Nullable<decimal> ProductDealPrice { get; set; }
        public Nullable<int> OrderItemChannel { get; set; }
        public Nullable<int> InsuranceLogo { get; set; }
        public string OrderItemLicenseNo { get; set; }
        public Nullable<System.DateTime> OrderItemForceExpireDate { get; set; }
        public Nullable<System.DateTime> OrderItemBusinessExpireDate { get; set; }
        public Nullable<int> OrderItemBuid { get; set; }

        public int OrderPayId { get; set; }
        public string OrderPayOrderCode { get; set; }
        public Nullable<int> OrderPayPayType { get; set; }
        public Nullable<int> OrderPayPayBank { get; set; }
        public Nullable<System.DateTime> OrderPayCreateDate { get; set; }


        public int PolicyId { get; set; }
        public string PolicyOrderCode { get; set; }
        public string PolicyName { get; set; }
        public Nullable<int> PolicyIdCardType { get; set; }
        public string PolicyIdCard { get; set; }
        public string PolicyIdCardFacePicUrl { get; set; }
        public string PolicyIdCardBackPicUrl { get; set; }
        public Nullable<System.DateTime> PolicyCreateDate { get; set; }
        public Nullable<int> PolicyUserId { get; set; }
        public string PolicyMobile { get; set; }
        public int RenewalId { get; set; }
        public Nullable<int> ChuXianCiShu { get; set; }
        public Nullable<int> WuPeiKuanYouHui { get; set; }
        public Nullable<int> ChuXianPeiKuanJin { get; set; }
    }
}