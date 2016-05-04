using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HONGLI.Entity;

namespace HONGLI.Repository
{
    public class OrderRepository
    {



        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<Order_Base> GetOrderListByUserId(string userId)
        {
            List< Order_Base> list = null;
            try
            {
                using (var context = new E2JOINDB())
                {

                    var query = context.Order_Base
                        .Include("Order_Item")
                        .Include("Order_Deliver")
                        .Include("Order_Pay")
                        .Include("Order_PolicyHolder")
                        .Where(c => c.UserId == userId)
                        .ToList();

                    list = query;
                }
            }
            catch (Exception error)
            {
                //LogHelper.AppError(error.Message);
            }
            return list;
        }


        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Order_Base GetOrderByCode(string code)
        {
            Order_Base order = null;
            try
            {
                using (var context = new E2JOINDB())
                {

                    var query = context.Order_Base                        
                        .Include("Order_Item")
                        .Include("Order_Deliver")
                        .Include("Order_Pay")
                        .Include("Order_PolicyHolder")
                        .Where(c => c.OrderCode == code)
                        .FirstOrDefault();

                    order = query;
                }
            }
            catch (Exception error)
            {
                //LogHelper.AppError(error.Message);
            }
            return order;
        }


        /// <summary>
        /// 下单操作
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public string AddOrder(Order_Base order)
        {
            string code = "";
            try
            {
                using (var context = new E2JOINDB())
                {
                    context.Order_Base.Add(order);
                    context.SaveChanges();
                    code = order.OrderCode;
                }              
            }
            catch (Exception error)
            {
                //LogHelper.AppError(error.Message);
            }
            return code;
        }

        /// <summary>
        /// 更新支付状态
        /// </summary>
        /// <param name="ordercode"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdateOrderStatus(string ordercode, int status)
        {
            int result = 0;
            using (var context = new E2JOINDB())
            {
                var query = context.Order_Base.Where(c => c.OrderCode == ordercode).FirstOrDefault();
                if (query != null)
                {
                    query.Status = status;
                    result = context.SaveChanges();
                }
            }
            return result;
        }

        /// <summary>
        /// 支付后更新数据库
        /// </summary>
        /// <param name="ordercode"></param>
        /// <param name="tradeNo"></param>
        /// <param name="paidAmount"></param>
        /// <param name="payTime"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdatePay(string ordercode, string tradeNo, string paidAmount, DateTime payTime, byte status)
        {
            int result = 0;
            using (var context = new E2JOINDB())
            {
                var query = context.Order_Base.Where(c => c.OrderCode == ordercode).FirstOrDefault();
                if (query != null && query.Status == (int)EnumOrderStatus.Paid || query.Status == (int)EnumOrderStatus.Success)
                {
                    result = 1;
                    return result;
                }

                if (query != null)
                {
                    query.TradeNo = tradeNo;
                    query.PaidAmount = Convert.ToDecimal(paidAmount);
                    query.PayTime = payTime;
                    query.Status = status;
                    result = context.SaveChanges();
                }
            }
            return result;
        }

        /// <summary>
        /// 获取用户下单前需要的数据
        /// </summary>
        /// <param name="ordercode"></param>
        /// <returns></returns>
        public AllOrderList GetOrderListByOrderCode(string ordercode)
        {
            var result = new AllOrderList();
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT A.Id AS OrderBaseId,A.OrderCode,AmountPayable, ");
            sql.Append("Discount,PaidAmount,PrepaidAmount,PayTime,Status,TradeNo,A.Channel,A.UserId,InvoiceType, ");
            sql.Append("InvoiceTitle,B.Id AS OrderItemId,ProductId,ProductName,ProductTitle,ProductOriginalPrice,A.UserId, ");
            sql.Append("ProductDealPrice,InsuranceLogo,LicenseNo,Convert(varchar(10),ForceExpireDate,120) AS ForceExpireDate,Convert(varchar(10),BusinessExpireDate,120) AS BusinessExpireDate, ");
            sql.Append("Buid,C.Id AS DeliverId,DeliverType,DeliverAddress,DeliverTime,DeliverPrice,DeliverName,B.CreateDate AS DeliverCreateDate, ");
            sql.Append("DeliverDistrictCode,Name,IdCard,IdCardType,IdCardBackPicUrl,IdCardFacePicUrl ");
            sql.Append("FROM dbo.Order_Base A WITH(NOLOCK) ");
            sql.Append("LEFT JOIN dbo.Order_Item B WITH(NOLOCK) ON A.OrderCode = B.OrderCode ");
            sql.Append("LEFT JOIN dbo.Order_Deliver C WITH(NOLOCK) ON A.OrderCode = C.OrderCode ");
            sql.Append("LEFT JOIN dbo.Order_PolicyHolder D WITH(NOLOCK) ON A.OrderCode=D.OrderCode ");
            sql.AppendFormat(" WHERE A.OrderCode='{0}'", ordercode);
            using (var db = new E2JOINDB())
            {
                var query = db.Database.SqlQuery<AllOrderList>(sql.ToString());
                result = query.FirstOrDefault();
            }
            return result;
        }
        public class AllOrderList
        {
            public int OrderBaseId { get; set; }
            public string OrderCode { get; set; }
            public Nullable<decimal> AmountPayable { get; set; }
            public Nullable<decimal> Discount { get; set; }
            public Nullable<decimal> PaidAmount { get; set; }
            public Nullable<System.DateTime> PayTime { get; set; }
            public Nullable<int> Status { get; set; }
            public string TradeNo { get; set; }
            public Nullable<int> Channel { get; set; }
            public string UserId { get; set; }
            public Nullable<int> InvoiceType { get; set; }
            public string InvoiceTitle { get; set; }
            public Nullable<decimal> PrepaidAmount { get; set; }

            public Nullable<int> OrderItemId { get; set; }
            public string ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductTitle { get; set; }
            public Nullable<decimal> ProductOriginalPrice { get; set; }
            public Nullable<decimal> ProductDealPrice { get; set; }
            public Nullable<int> InsuranceLogo { get; set; }
            public string LicenseNo { get; set; }
            public string ForceExpireDate { get; set; }
            public string BusinessExpireDate { get; set; }
            public Nullable<int> Buid { get; set; }
            public Nullable<int> DeliverId { get; set; }
            public Nullable<int> DeliverType { get; set; }
            public string DeliverAddress { get; set; }
            public Nullable<System.DateTime> DeliverTime { get; set; }
            public Nullable<decimal> DeliverPrice { get; set; }
            public string DeliverName { get; set; }
            public Nullable<System.DateTime> DeliverCreateDate { get; set; }
            public string DeliverMobile { get; set; }
            public string DeliverDistrictCode { get; set; }

            public Nullable<int> PolicyId { get; set; }
            public string Name { get; set; }
            public Nullable<int> IdCardType { get; set; }
            public string IdCard { get; set; }
            public string IdCardFacePicUrl { get; set; }
            public string IdCardBackPicUrl { get; set; }
            public string Mobile { get; set; }
        }

        /// <summary>
        /// 添加支付状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddPay(Order_Pay model)
        {
            int result = 0;
            using (var context = new E2JOINDB())
            {
                context.Order_Pay.Add(model);
                context.SaveChanges();
                result = model.Id;
            }
            return result;
        }
        /// <summary>
        /// 修改发票信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditOrderInvoice(Order_Base model)
        {
            int result = -1;
            using (var context = new E2JOINDB())
            {
                context.Order_Base.Attach(model);
                context.Entry(model).Property(t => t.Id).IsModified = true;
                context.Entry(model).Property(t => t.OrderCode).IsModified = true;
                context.Entry(model).Property(t => t.InvoiceTitle).IsModified = true;
                context.Entry(model).Property(t => t.InvoiceType).IsModified = true;
                context.Entry(model).Property(t => t.AmountPayable).IsModified = true;
                context.Entry(model).Property(t => t.PaidAmount).IsModified = true;
                context.SaveChanges();
                result = model.Id;
            }

            return result;
        }
    }
}
