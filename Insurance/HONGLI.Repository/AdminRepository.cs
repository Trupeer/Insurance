using HONGLI.Entity;
using I.Utility.Extensions;
using I.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Repository
{
    public class AdminRepository
    {
        #region 登录
        /// <summary>
        /// 判断用户登录，获取用户信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public BaseInfo_UserInfo CheckUser(string username, string password)
        {
            var returnValue = new BaseInfo_UserInfo();
            if (username.IsNull())
                return returnValue;

            try
            {
                using (var context = new E2JOINDB())
                {
                    returnValue = (from x in context.BaseInfo_UserInfo
                                   where
                                     x.Name == username
                                   where
                                   x.Password == password
                                   where
                                   x.Status == 2
                                   select x).FirstOrDefault();
                }

            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
            }

            return returnValue;
        }
        #endregion
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="CarID"></param>
        /// <param name="username"></param>
        /// <param name="ChannelName"></param>
        /// <param name="BeginTrialDate"></param>
        /// <param name="EndTrialDate"></param>
        /// <param name="BeginInsuranceEndDate"></param>
        /// <param name="EndInsuranceEndDate"></param>
        /// <param name="BeginVisitDate"></param>
        /// <param name="EndVisitDate"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="InsuranceCompany"></param>
        /// <param name="VistState"></param>
        /// <param name="Auditdocuments"></param>
        /// <param name="OrderState"></param>
        /// <param name="offerState"></param>
        /// <param name="examineState"></param>
        /// <returns></returns>
        public DataTable GetOrderList(int Page, string CarID, string username, string ChannelName, string BeginInsuranceEndDate, string EndInsuranceEndDate, string BeginVisitDate, string EndVisitDate, string BeginDate, string EndDate, string InsuranceCompany, string VistState, string Auditdocuments, string OrderState, string examineState)
        {
            
            #region 查询条件
            StringBuilder where = new StringBuilder();
            if (!string.IsNullOrEmpty(username))
            {
                where.AppendFormat(" AND UserName LIKE '%{0}%'", username);
            }
            if (!string.IsNullOrEmpty(CarID))
            {
                where.AppendFormat(" AND LicenseNo LIKE '%{0}%'", CarID);
            }
            if (!string.IsNullOrEmpty(ChannelName))
            {
                where.AppendFormat(" AND T.Channel ={0}", ChannelName);
            }
            if (!string.IsNullOrEmpty(BeginInsuranceEndDate) && !string.IsNullOrEmpty(EndInsuranceEndDate))
            {
                where.AppendFormat(" AND BusinessExpireDate BETWEEN '{0}' AND '{1}'", BeginInsuranceEndDate, EndInsuranceEndDate);
            }
            if (!string.IsNullOrEmpty(BeginVisitDate) && !string.IsNullOrEmpty(EndVisitDate))
            {
                where.AppendFormat(" AND VisitDate BETWEEN '{0}' AND '{1}'", BeginVisitDate, EndVisitDate);
            }
            if (!string.IsNullOrEmpty(BeginDate) && !string.IsNullOrEmpty(EndDate))
            {
                where.AppendFormat(" AND T.CreateTime BETWEEN '{0}' AND '{1}'", BeginDate, EndDate);
            }
            if (!string.IsNullOrEmpty(InsuranceCompany))
            {
                where.AppendFormat(" AND InsuranceCompany ={0}", InsuranceCompany);
            }
            if (!string.IsNullOrEmpty(VistState))
            {
                if (VistState == "0")
                {
                    where.AppendFormat(" AND (VisitState ={0} OR VisitState IS NULL)", VistState);
                }
                else
                {
                    where.AppendFormat(" AND VisitState ={0}", VistState);
                }
            }
            if (!string.IsNullOrEmpty(Auditdocuments))
            {
                where.AppendFormat(" AND AuditOrderState ={0}", Auditdocuments);
            }
            if (!string.IsNullOrEmpty(OrderState))
            {
                where.AppendFormat(" AND OrderreviewState ={0}", OrderState);
            }
            if (!string.IsNullOrEmpty(examineState))
            {
                where.AppendFormat(" AND (RbOffer ={0} OR TpyOffer={0} OR PaOffer={0})", examineState);
            }
            #endregion
            #region 查询内容
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM  ");
            sql.Append("(SELECT ROW_NUMBER() OVER(ORDER BY T.CreateTime DESC) AS PageIndex, ");
            sql.Append("T.*,T2.Name AS ServiceUserName, ");
            sql.Append("T3.Name AS VisitServiceUserName ");
            sql.Append("FROM (SELECT A.Id,MAX(LicenseOwner) AS UserName, MAX(LicenseNo) AS LicenseNo, ");
            sql.Append("MAX(A.Mobile) AS Mobile,MAX(BusinessExpireDate) AS BusinessExpireDate, ");
            sql.Append("MAX(CASE WHEN B.UserSelection=1 THEN B.Id ELSE 0 END) AS ProdunctItemID, ");
            sql.Append("MAX(A.CreateTime) AS CreateTime,MAX(A.Channel) AS Channel, ");
            sql.Append("MAX(CASE WHEN B.UserSelection=1 THEN B.Source ELSE 0 END) AS InsuranceCompany, ");
            sql.Append("RbOffer=MAX(CASE WHEN B.Source=2 AND SubmitStatus=1 THEN 1 ELSE 0 END), ");
            sql.Append("TpyOffer=MAX(CASE WHEN B.Source=1 AND SubmitStatus=1 THEN 1 ELSE 0 END), ");
            sql.Append("PaOffer=MAX(CASE WHEN B.Source=0 AND SubmitStatus=1 THEN 1 ELSE 0 END), ");
            sql.Append("MAX(B.AuditOrderStatus) AS AuditOrderState,MAX(B.ServiceUserId) AS ServiceUserId, ");
            sql.Append("MAX(VisitDate) AS VisitDate,MAX(VisitStatus) AS VisitState,MAX(VisitServiceUserId) AS VisitServiceUserId, ");
            sql.Append("AuditOrderDate=MAX(B.CreateTime),MAX(C.ProductItemId) AS  ProductItemId ,MAX(C.OrderCode) AS OrderCode,MAX(C.Status) AS OrderreviewState  FROM dbo.ProductV2_User A WITH(NOLOCK) ");
            sql.Append("LEFT JOIN dbo.ProductV2_Item B  WITH(NOLOCK) ON A.Id = B.UserId ");
            sql.Append(" LEFT JOIN dbo.Order_Base C  WITH(NOLOCK) ON B.Id=C.ProductItemId ");
            sql.Append(" GROUP BY A.Id) T ");
            sql.Append(" LEFT JOIN dbo.Order_Base T1  WITH(NOLOCK) ON T.ProductItemId=T1.ProductItemId ");
            sql.Append(" LEFT JOIN dbo.BaseInfo_UserInfo T2  WITH(NOLOCK) ON T.ServiceUserId=T2.ID ");
            sql.Append(" LEFT JOIN dbo.BaseInfo_UserInfo T3  WITH(NOLOCK) ON T.VisitServiceUserId=T3.ID ");
            sql.Append(" WHERE 1=1 ");
            sql.Append(where.ToString());
            sql.AppendFormat(") T4 WHERE PageIndex BETWEEN {0} AND {1}", Page * 10 - 9, Page * 10);
            #endregion
            using (var db = new E2JOINDB())
            {
                DataTable dt = new DataTable();
                var query = db.Database.SqlQuery<OrderList>(sql.ToString());
                dt.Columns.Add("Id");
                dt.Columns.Add("UserName");
                dt.Columns.Add("LicenseNo");
                dt.Columns.Add("Mobile");
                dt.Columns.Add("BusinessExpireDate");
                dt.Columns.Add("ProdunctItemID");
                dt.Columns.Add("CreateTime");
                dt.Columns.Add("InsuranceCompany");
                dt.Columns.Add("RbOffer");
                dt.Columns.Add("TpyOffer");
                dt.Columns.Add("PaOffer");
                dt.Columns.Add("AuditOrderState");
                dt.Columns.Add("Channel");
                dt.Columns.Add("ServiceUserId");
                dt.Columns.Add("VisitDate");
                dt.Columns.Add("VisitState");
                dt.Columns.Add("VisitServiceUserId");
                dt.Columns.Add("AuditOrderDate");
                dt.Columns.Add("OrderCode");
                dt.Columns.Add("OrderreviewState");
                dt.Columns.Add("ServiceUserName");
                dt.Columns.Add("VisitServiceUserName");
                foreach(var item in query)
                {
                    DataRow dr = dt.NewRow();
                    dr["Id"]=item.Id;
                    dr["UserName"]=item.UserName;
                    dr["LicenseNo"]=item.LicenseNo;
                    dr["Mobile"]=item.Mobile;
                    dr["BusinessExpireDate"]=item.BusinessExpireDate;
                    dr["ProdunctItemID"]=item.ProdunctItemID;
                    dr["CreateTime"]=item.CreateTime;
                    dr["InsuranceCompany"]=item.InsuranceCompany;
                    dr["RbOffer"]=item.RbOffer;
                    dr["TpyOffer"]=item.TpyOffer;
                    dr["PaOffer"]=item.PaOffer;
                    dr["AuditOrderState"]=item.AuditOrderState;
                    dr["Channel"]=item.Channel;
                    dr["ServiceUserId"]=item.ServiceUserId;
                    dr["VisitDate"]=item.VisitDate;
                    dr["VisitState"]=item.VisitState;
                    dr["VisitServiceUserId"]=item.VisitServiceUserId;
                    dr["AuditOrderDate"]=item.AuditOrderDate;
                    dr["OrderCode"]=item.OrderCode;
                    dr["OrderreviewState"]=item.OrderreviewState;
                    dr["ServiceUserName"]=item.ServiceUserName;
                    dr["VisitServiceUserName"]=item.VisitServiceUserName;
                    dt.Rows.Add(dr);
                }
                return dt;
            }
        }
        public decimal GetOrderListCount( string CarID, string username, string ChannelName, string BeginInsuranceEndDate, string EndInsuranceEndDate, string BeginVisitDate, string EndVisitDate, string BeginDate, string EndDate, string InsuranceCompany, string VistState, string Auditdocuments, string OrderState, string examineState)
        {

            #region 查询条件
            StringBuilder where = new StringBuilder();
            if (!string.IsNullOrEmpty(username))
            {
                where.AppendFormat(" AND UserName LIKE '%{0}%'", username);
            }
            if (!string.IsNullOrEmpty(CarID))
            {
                where.AppendFormat(" AND LicenseNo LIKE '%{0}%'", CarID);
            }
            if (!string.IsNullOrEmpty(ChannelName))
            {
                where.AppendFormat(" AND T.Channel ={0}", ChannelName);
            }
            if (!string.IsNullOrEmpty(BeginInsuranceEndDate) && !string.IsNullOrEmpty(EndInsuranceEndDate))
            {
                where.AppendFormat(" AND BusinessExpireDate BETWEEN '{0}' AND '{1}'", BeginInsuranceEndDate, EndInsuranceEndDate);
            }
            if (!string.IsNullOrEmpty(BeginVisitDate) && !string.IsNullOrEmpty(EndVisitDate))
            {
                where.AppendFormat(" AND VisitDate BETWEEN '{0}' AND '{1}'", BeginVisitDate, EndVisitDate);
            }
            if (!string.IsNullOrEmpty(BeginDate) && !string.IsNullOrEmpty(EndDate))
            {
                where.AppendFormat(" AND T.CreateTime BETWEEN '{0}' AND '{1}'", BeginDate, EndDate);
            }
            if (!string.IsNullOrEmpty(InsuranceCompany))
            {
                where.AppendFormat(" AND InsuranceCompany ={0}", InsuranceCompany);
            }
            if (!string.IsNullOrEmpty(VistState))
            {
                if(VistState=="0")
                {
                    where.AppendFormat(" AND (VisitState ={0} OR VisitState IS NULL)", VistState);
                }
                else
                {
                    where.AppendFormat(" AND VisitState ={0}", VistState);
                }
            }
            if (!string.IsNullOrEmpty(Auditdocuments))
            {
                where.AppendFormat(" AND AuditOrderState ={0}", Auditdocuments);
            }
            if (!string.IsNullOrEmpty(OrderState))
            {
                where.AppendFormat(" AND OrderreviewState ={0}", OrderState);
            }
            if (!string.IsNullOrEmpty(examineState))
            {
                where.AppendFormat(" AND (RbOffer ={0} OR TpyOffer={0} OR PaOffer={0})", examineState);
            }
            #endregion
            #region 查询内容
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT COUNT(*) AS SumCount FROM  ");
            sql.Append("(SELECT ROW_NUMBER() OVER(ORDER BY T.CreateTime DESC) AS PageIndex, ");
            sql.Append("T.*,T2.Name AS ServiceUserName, ");
            sql.Append("T3.Name AS VisitServiceUserName ");
            sql.Append("FROM (SELECT A.Id, MAX(LicenseOwner) AS UserName, MAX(LicenseNo) AS LicenseNo, ");
            sql.Append("MAX(A.Mobile) AS Mobile,MAX(BusinessExpireDate) AS BusinessExpireDate, ");
            sql.Append("MAX(CASE WHEN B.UserSelection=1 THEN B.Id ELSE 0 END) AS ProdunctItemID, ");
            sql.Append("MAX(A.CreateTime) AS CreateTime,MAX(A.Channel) AS Channel, ");
            sql.Append("MAX(CASE WHEN B.UserSelection=1 THEN B.Source ELSE 0 END) AS InsuranceCompany, ");
            sql.Append("RbOffer=MAX(CASE WHEN B.Source=2 AND SubmitStatus=1 THEN 1 ELSE 0 END), ");
            sql.Append("TpyOffer=MAX(CASE WHEN B.Source=1 AND SubmitStatus=1 THEN 1 ELSE 0 END), ");
            sql.Append("PaOffer=MAX(CASE WHEN B.Source=0 AND SubmitStatus=1 THEN 1 ELSE 0 END), ");
            sql.Append("MAX(B.AuditOrderStatus) AS AuditOrderState,MAX(B.ServiceUserId) AS ServiceUserId, ");
            sql.Append("MAX(VisitDate) AS VisitDate,MAX(VisitStatus) AS VisitState,MAX(VisitServiceUserId) AS VisitServiceUserId, ");
            sql.Append("AuditOrderDate=MAX(B.CreateTime) ,MAX(C.ProductItemId) AS  ProductItemId,MAX(C.OrderCode) AS OrderCode,MAX(C.Status) AS OrderreviewState  FROM dbo.ProductV2_User A WITH(NOLOCK) ");
            sql.Append("LEFT JOIN dbo.ProductV2_Item B  WITH(NOLOCK) ON A.Id = B.UserId ");
            sql.Append(" LEFT JOIN dbo.Order_Base C  WITH(NOLOCK) ON B.Id=C.ProductItemId ");
            sql.Append(" GROUP BY A.Id) T ");
            sql.Append(" LEFT JOIN dbo.Order_Base T1  WITH(NOLOCK) ON T.ProductItemId=T1.ProductItemId ");
            sql.Append(" LEFT JOIN dbo.BaseInfo_UserInfo T2  WITH(NOLOCK) ON T.ServiceUserId=T2.ID ");
            sql.Append(" LEFT JOIN dbo.BaseInfo_UserInfo T3  WITH(NOLOCK) ON T.VisitServiceUserId=T3.ID ");
            sql.Append(" WHERE 1=1  ");
            sql.Append(where.ToString());
            sql.Append(") T4 ");
            #endregion
            using (var db = new E2JOINDB())
            {
                int SumCount = 0;
                var query = db.Database.SqlQuery<OrderListCount>(sql.ToString()).ToList();
              if(query.Count>0)
                {
                    SumCount =string.IsNullOrEmpty(query.FirstOrDefault().SumCount.ToString())?0:Convert.ToInt32(query.FirstOrDefault().SumCount);
                }
                return Math.Ceiling(Convert.ToDecimal(SumCount)/10);
            }
        }
        public class OrderList
        {
            public int? Id { get; set; }
            public string UserName { get; set; }
            public string LicenseNo { get; set; }

            public string Mobile { get; set; }
            public string BusinessExpireDate { get; set; }
            public Nullable<int> ProdunctItemID { get; set; }
            public Nullable<DateTime> CreateTime { get; set; }
            
            public Nullable<int> InsuranceCompany { get; set; }
            public Nullable<int> RbOffer { get; set; }
            public Nullable<int> TpyOffer { get; set; }
            public Nullable<int> PaOffer { get; set; }
            public Nullable<int> AuditOrderState { get; set; }
            public Nullable<byte> Channel { get; set; }
            public Nullable<int> ServiceUserId { get; set; }
            public Nullable<DateTime> VisitDate { get; set; }
            public Nullable<int> VisitState { get; set; }
            public Nullable<int> VisitServiceUserId { get; set; }
            public Nullable<DateTime> AuditOrderDate { get; set; }
            public string OrderCode { get; set; }
            public Nullable<int> OrderreviewState { get; set; }
            public string ServiceUserName { get; set; }
            public string VisitServiceUserName { get; set; }
        }

        public class OrderListCount
        {
            public Nullable<int> SumCount { get; set; }
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditProductUser(ProductV2_User model)
        {
            int result = -1;
            using (var context = new E2JOINDB())
            {
                context.ProductV2_User.Attach(model);
                context.Entry(model).Property(t => t.Id).IsModified = true;
                context.Entry(model).Property(t => t.LicenseNo).IsModified = true;
                context.Entry(model).Property(t => t.LicenseOwner).IsModified = true;
                context.Entry(model).Property(t => t.Mobile).IsModified = true;
                context.Entry(model).Property(t => t.ModleName).IsModified = true;
                context.Entry(model).Property(t => t.RegisterDate).IsModified = true;
                context.Entry(model).Property(t => t.ForceExpireDate).IsModified = true;
                context.Entry(model).Property(t => t.EngineNo).IsModified = true;
                context.Entry(model).Property(t => t.CredentislasNum).IsModified = true;
                context.Entry(model).Property(t => t.CarVin).IsModified = true;
                context.Entry(model).Property(t => t.BusinessExpireDate).IsModified = true;
                context.Entry(model).Property(t => t.VisitDate).IsModified = true;
                context.Entry(model).Property(t => t.VisitServiceUserId).IsModified = true;
                context.Entry(model).Property(t => t.VisitStatus).IsModified = true;
                context.SaveChanges();
                result = model.Id;
            }

            return result;
        }
        /// <summary>
        /// 修改订单信息（order_base）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditOrder_Base(Order_Base model)
        {
            int result = -1;
            using (var context = new E2JOINDB())
            {
                context.Order_Base.Attach(model);
                context.Entry(model).Property(t => t.Id).IsModified = true;
                context.Entry(model).Property(t => t.OrderCode).IsModified = true;
                context.Entry(model).Property(t => t.CreateDate).IsModified = true;
                context.Entry(model).Property(t => t.Channel).IsModified = true;
                context.Entry(model).Property(t => t.ProductItemId).IsModified = true;
                context.Entry(model).Property(t => t.Order_Item.FirstOrDefault().Id).IsModified = true;
                context.Entry(model).Property(t => t.Order_Item.FirstOrDefault().OrderCode).IsModified = true;
                context.Entry(model).Property(t => t.Order_Item.FirstOrDefault().CreateDate).IsModified = true;
                context.Entry(model).Property(t => t.Order_Item.FirstOrDefault().ProductId).IsModified = true;
                context.Entry(model).Property(t => t.Order_Item.FirstOrDefault().ProductName).IsModified = true;
                context.Entry(model).Property(t => t.Order_Item.FirstOrDefault().InsuranceLogo).IsModified = true;
                context.Entry(model).Property(t => t.Order_Item.FirstOrDefault().LicenseNo).IsModified = true;
                context.SaveChanges();
                result = model.Id;
            }

            return result;
        }
        /// <summary>
        /// 获取用户信息（车牌号）
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ProductV2_User GetProductUser(int? ID)
        {
            ProductV2_User ProductUser = new ProductV2_User();
            try
            {
                using (var context = new E2JOINDB())
                {

                    var query = context.ProductV2_User
                        .Where(c => c.Id == ID)
                        .FirstOrDefault();

                    ProductUser = query;
                }
            }
            catch (Exception error)
            {
                //LogHelper.AppError(error.Message);
            }
            return ProductUser;
        }
        /// <summary>
        /// 根据手机号获取用户ID
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public BaseInfo_UserInfo GetUserIdByMobile(string mobile)
        {
            BaseInfo_UserInfo model = new BaseInfo_UserInfo();
            try
            {
                using (var context = new E2JOINDB())
                {

                    var query = context.BaseInfo_UserInfo
                        .Where(c => c.Mobile==mobile)
                        .FirstOrDefault();

                    model = query;
                }
            }
            catch (Exception error)
            {
                //LogHelper.AppError(error.Message);
            }
            return model;
        }

        /// <summary>
        /// 获取用户续保信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ProductV2_Renewal GetProductRenewal(int? ID)
        {
            ProductV2_Renewal productrenewal = new ProductV2_Renewal();
            try
            {
                using (var context = new E2JOINDB())
                {

                    var query = context.ProductV2_Renewal
                        .Where(c => c.UserId == ID)
                        .FirstOrDefault();

                    productrenewal = query;
                }
            }
            catch (Exception error)
            {
                //LogHelper.AppError(error.Message);
            }
            return productrenewal;
        }
        /// <summary>
        /// 修改核保信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditProductItem(ProductV2_Item model)
        {
            int result = -1;
            using (var context = new E2JOINDB())
            {
                context.Entry(model).State = EntityState.Modified;
                context.SaveChanges();
                result = model.Id;
            }

            return result;
        }
        /// <summary>
        /// 获取核保信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ProductV2_Item GetProductItem(int? UserId, int? InsuranceCompany)
        {
            ProductV2_Item ProductItem = new ProductV2_Item();
            try
            {
                using (var context = new E2JOINDB())
                {

                    var query = context.ProductV2_Item
                        .Where(c => c.UserId == UserId && c.Source == InsuranceCompany)
                        .OrderByDescending(c => c.CreateTime)
                        .FirstOrDefault();

                    ProductItem = query;
                }
            }
            catch (Exception error)
            {
                //LogHelper.AppError(error.Message);
            }
            return ProductItem;
        }

        /// <summary>
        /// 获取用户选择核保信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ProductV2_Item GetProductItemSelection(int? UserId)
        {
            ProductV2_Item ProductItem = new ProductV2_Item();
            try
            {
                using (var context = new E2JOINDB())
                {

                    var query = context.ProductV2_Item
                        .Where(c => c.UserId == UserId && c.UserSelection == true)
                        .OrderByDescending(c => c.CreateTime)
                        .FirstOrDefault();
                    if (query == null)
                    {
                        query = context.ProductV2_Item
                        .Where(c => c.UserId == UserId)
                        .OrderByDescending(c => c.CreateTime)
                        .FirstOrDefault();
                    }
                    ProductItem = query;
                }
            }
            catch (Exception error)
            {
                //LogHelper.AppError(error.Message);
            }
            return ProductItem;
        }
        /// <summary>
        /// 修改续保信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditProductRenewal(ProductV2_Renewal model)
        {
            int result = -1;
            using (var context = new E2JOINDB())
            {
                context.Entry(model).State = EntityState.Modified;
                context.SaveChanges();
                result = model.Id;
            }

            return result;
        }
        /// <summary>
        /// 修改被保人信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditPolicyHolder(Order_PolicyHolder model)
        {
            int result = -1;
            using (var context = new E2JOINDB())
            {
                context.Entry(model).State = EntityState.Modified;
                context.SaveChanges();
                result = model.Id;
            }

            return result;
        }
        /// <summary>
        /// 添加被保人信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int SavePolicyHolder(Order_PolicyHolder model)
        {
            int result = -1;
            using (var context = new E2JOINDB())
            {
                context.Order_PolicyHolder.Add(model);
                context.SaveChanges();
                result = model.Id;
            }

            return result;
        }
        /// <summary>
        /// 添加核保信息
        /// </summary>
        /// <param name="productv2_item"></param>
        /// <returns></returns>
        public int SaveProductItem(ProductV2_Item productv2_item)
        {
            int itemid;
            using (var context = new E2JOINDB())
            {
                //事务提交
                context.ProductV2_Item.Add(productv2_item);
                context.SaveChanges();
                itemid = productv2_item.Id;
            }

            return itemid;
        }

        /// <summary>
        /// 修改订单地址信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditOrderDeliver(Order_Deliver model)
        {
            int result = -1;
            using (var context = new E2JOINDB())
            {
                context.Entry(model).State = EntityState.Modified;
                context.SaveChanges();
                result = model.Id;
            }

            return result;
        }
        /// <summary>
        /// 修改支付信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditOrderPay(Order_Pay model)
        {
            int result = -1;
            using (var context = new E2JOINDB())
            {
                context.Entry(model).State = EntityState.Modified;
                context.SaveChanges();
                result = model.Id;
            }

            return result;
        }
        /// <summary>
        /// 添加订单地址信息
        /// </summary>
        /// <param name="order_deliver"></param>
        /// <returns></returns>
        public int SaveOrderDeliver(Order_Deliver order_deliver)
        {
            int itemid;
            using (var context = new E2JOINDB())
            {
                //事务提交
                context.Order_Deliver.Add(order_deliver);
                context.SaveChanges();
                itemid = order_deliver.Id;
            }

            return itemid;
        }
        /// <summary>
        /// 判断收货地址是否存在
        /// </summary>
        /// <param name="ordercode"></param>
        /// <returns></returns>
        public int CHeckDeliverAdd(string ordercode)
        {
            int deliverId = -1;
            using (var context = new E2JOINDB())
            {
                Order_Deliver model = new Order_Deliver();
                model = context.Order_Deliver.Where(c => c.OrderCode == ordercode).FirstOrDefault();
                if(model!=null)
                {
                    deliverId = model.Id;
                }
            }

            return deliverId;
        }

        /// <summary>
        /// 判断被保人信息是否存在
        /// </summary>
        /// <param name="ordercode"></param>
        /// <returns></returns>
        public int CHeckpolicyHolderAdd(string ordercode)
        {
            int policyHoldeId = -1;
            using (var context = new E2JOINDB())
            {
                Order_PolicyHolder model = new Order_PolicyHolder();
                model = context.Order_PolicyHolder.Where(c => c.OrderCode == ordercode).FirstOrDefault();
                if (model != null)
                {
                    policyHoldeId = model.Id;
                }
            }

            return policyHoldeId;
        }
        /// <summary>
        /// 判断支付信息是否存在
        /// </summary>
        /// <param name="ordercode"></param>
        /// <returns></returns>
        public int CHeckpayAdd(string ordercode)
        {
            int payId = -1;
            using (var context = new E2JOINDB())
            {
                Order_Pay model = new Order_Pay();
                model = context.Order_Pay.Where(c => c.OrderCode == ordercode).FirstOrDefault();
                if (model != null)
                {
                    payId = model.Id;
                }
            }

            return payId;
        }
        /// <summary>
        /// 核单
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ItemId"></param>
        /// <returns></returns>
        public int ChangeAuditDocuments(int UserId,int ItemId)
        {
            int result = -1;
            ProductV2_Item model = new ProductV2_Item();
            model.Id = ItemId;
            model.ServiceUserId = UserId;
            model.AuditOrderStatus = 1;
            using (var context = new E2JOINDB())
            {
                context.ProductV2_Item.Attach(model);
                context.Entry(model).Property(t => t.Id).IsModified = true;
                context.Entry(model).Property(t => t.ServiceUserId).IsModified = true;
                context.Entry(model).Property(t => t.AuditOrderStatus).IsModified = true;
                context.SaveChanges();
                result = model.Id;
            }

            return result;
        }
        /// <summary>
        /// 核单通过后修改订单内容
        /// </summary>
        /// <param name="order_base"></param>
        /// <returns></returns>
        public int ChangeOrderList(Order_Base order_base)
        {
            int result = -1;

            using (var context = new E2JOINDB())
            {
                context.Order_Base.Attach(order_base);
                context.Entry(order_base).Property(t => t.Id).IsModified = true;
                context.Entry(order_base).Property(t => t.Status).IsModified = true;
                //context.Entry(order_base).Property(t => t.PrepaidAmount).IsModified = true;
                context.Entry(order_base).Property(t => t.CreateDate).IsModified = true;
                context.Entry(order_base).Property(t => t.ProductItemId).IsModified = true;
                //context.Entry(order_base).Property(t => t.AmountPayable).IsModified = true;
                context.SaveChanges();
                context.Order_Item.Attach(order_base.Order_Item.FirstOrDefault());
                context.Entry(order_base.Order_Item.FirstOrDefault()).Property(t => t.Id).IsModified = true;
                context.Entry(order_base.Order_Item.FirstOrDefault()).Property(t => t.ProductId).IsModified = true;
                context.Entry(order_base.Order_Item.FirstOrDefault()).Property(t => t.ProductName).IsModified = true;
                context.Entry(order_base.Order_Item.FirstOrDefault()).Property(t => t.BusinessExpireDate).IsModified = true;
                context.Entry(order_base.Order_Item.FirstOrDefault()).Property(t => t.ForceExpireDate).IsModified = true;
                context.Entry(order_base.Order_Item.FirstOrDefault()).Property(t => t.ProductDealPrice).IsModified = true;
                context.Entry(order_base.Order_Item.FirstOrDefault()).Property(t => t.ProductOriginalPrice).IsModified = true;
                context.Entry(order_base.Order_Item.FirstOrDefault()).Property(t => t.ProductTitle).IsModified = true;
                context.Entry(order_base.Order_Item.FirstOrDefault()).Property(t => t.Buid).IsModified = true;
                context.SaveChanges();
                result = order_base.Id;
            }

            return result;
        }

        /// <summary>
        /// 查询订单详情
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="licenseNo"></param>
        /// <param name="Name"></param>
        /// <param name="BillCode"></param>
        /// <param name="Createtime"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public DataTable GetBillList(int Page, string licenseNo,string Name,string BillCode,DateTime? Createtime,int? status)
        {
            #region 查询内容
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM ");
            sql.Append("(SELECT ROW_NUMBER() OVER(ORDER BY A.CreateDate DESC) AS PageIndex,A.Id, A.OrderCode,A.CreateDate,A.Status,A.BackStatus,B.LicenseNo,B.ProductOriginalPrice,B.ProductDealPrice, ");
            sql.Append("D.Name,IdCardType,IdCard,C.DeliverMobile AS Mobile,E.PayType,ProductId,A.UserId,B.Channel,DeliverType,G.State AS receiveStatus ");
            sql.Append("FROM dbo.Order_Base A WITH(NOLOCK) ");
            sql.Append("LEFT JOIN dbo.Order_Item B WITH(NOLOCK) ON A.OrderCode = B.OrderCode ");
            sql.Append("LEFT JOIN dbo.Order_Deliver C WITH(NOLOCK) ON A.OrderCode = C.OrderCode ");
            sql.Append("LEFT JOIN dbo.Order_PolicyHolder D WITH(NOLOCK) ON A.OrderCode=D.OrderCode ");
            sql.Append("LEFT JOIN dbo.Order_Pay E WITH(NOLOCK) ON A.OrderCode = E.OrderCode ");
            //sql.Append("LEFT JOIN dbo.ProductV2_User F WITH(NOLOCK) ON A.UserId=F.Id ");
            sql.Append("LEFT JOIN dbo.RedPacket G WITH(NOLOCK) ON A.OrderCode=G.OrderCode ");
            sql.Append("WHERE 1=1 AND A.Status IS NOT NULL ");//AND A.Status IS NOT NULL
            if (!string.IsNullOrEmpty(licenseNo))
            {
                sql.AppendFormat(" AND B.LicenseNo LIKE '%{0}%' ", licenseNo);
            }
            if (!string.IsNullOrEmpty(Name))
            {
                sql.AppendFormat(" AND Name LIKE '%{0}%' ", Name);
            }
            if (!string.IsNullOrEmpty(BillCode))
            {
                sql.AppendFormat(" AND A.OrderCode LIKE '%{0}%' ", BillCode);
            }
            if (!string.IsNullOrEmpty(Createtime.ToString()))
            {
                sql.AppendFormat(" AND A.CreateDate ='{0}' ", Createtime);
            }
            if (!string.IsNullOrEmpty(status.ToString()))
            {
                sql.AppendFormat(" AND A.status ={0} ", status);
            }
            sql.AppendFormat(") T WHERE PageIndex BETWEEN {0} AND {1}", Page * 10 - 9, Page * 10);
            #endregion

            using (var db = new E2JOINDB())
            {
                DataTable dt = new DataTable();
                var query = db.Database.SqlQuery<BillList>(sql.ToString());
                dt.Columns.Add("Id");
                dt.Columns.Add("OrderCode");
                dt.Columns.Add("CreateDate");
                dt.Columns.Add("Status");
                dt.Columns.Add("BackStatus");
                dt.Columns.Add("receiveStatus");
                dt.Columns.Add("LicenseNo");
                dt.Columns.Add("Name");
                dt.Columns.Add("IdCardType");
                dt.Columns.Add("IdCard");
                dt.Columns.Add("Mobile");
                dt.Columns.Add("PayType");
                dt.Columns.Add("DeliverType");
                dt.Columns.Add("ProductId");
                dt.Columns.Add("UserId");
                dt.Columns.Add("Channel");
                dt.Columns.Add("ProductOriginalPrice");
                dt.Columns.Add("ProductDealPrice");
                foreach (var item in query)
                {
                    DataRow dr = dt.NewRow();
                    dr["Id"] = item.Id;
                    dr["OrderCode"] = item.OrderCode;
                    dr["CreateDate"] = item.CreateDate;
                    dr["Status"] = item.Status;
                    dr["BackStatus"] = item.BackStatus; ;
                    dr["receiveStatus"] = item.receiveStatus; 
                    dr["LicenseNo"] = item.LicenseNo;
                    dr["Name"] = item.Name;
                    dr["IdCardType"] = item.IdCardType;
                    dr["IdCard"] = item.IdCard;
                    dr["Mobile"] = item.Mobile;
                    dr["PayType"] = item.PayType;
                    dr["DeliverType"] = item.DeliverType;
                    dr["ProductId"] = item.ProductId;
                    dr["UserId"] = item.UserId;
                    dr["Channel"] = item.Channel;
                    dr["ProductOriginalPrice"] = item.ProductOriginalPrice;
                    dr["ProductDealPrice"] = item.ProductDealPrice;
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            }
        /// <summary>
        /// 查询订单总条数
        /// </summary>
        /// <param name="licenseNo"></param>
        /// <param name="Name"></param>
        /// <param name="BillCode"></param>
        /// <param name="Createtime"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public decimal GetBillListCount(string licenseNo, string Name, string BillCode, DateTime? Createtime, int? status)
        {
            #region 查询内容
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT COUNT(*) AS SumCount FROM ");
            sql.Append("(SELECT ROW_NUMBER() OVER(ORDER BY A.CreateDate DESC) AS PageIndex, A.OrderCode,A.CreateDate,A.Status,B.LicenseNo, ");
            sql.Append("D.Name,IdCardType,IdCard,C.DeliverMobile AS Mobile,E.PayType,ProductId,A.UserId,B.Channel,DeliverType ");
            sql.Append("FROM dbo.Order_Base A WITH(NOLOCK) ");
            sql.Append("LEFT JOIN dbo.Order_Item B WITH(NOLOCK) ON A.OrderCode = B.OrderCode ");
            sql.Append("LEFT JOIN dbo.Order_Deliver C WITH(NOLOCK) ON A.OrderCode = C.OrderCode ");
            sql.Append("LEFT JOIN dbo.Order_PolicyHolder D WITH(NOLOCK) ON A.OrderCode=D.OrderCode ");
            sql.Append("LEFT JOIN dbo.Order_Pay E WITH(NOLOCK) ON A.OrderCode = E.OrderCode ");
            //sql.Append("LEFT JOIN dbo.ProductV2_User F WITH(NOLOCK) ON A.UserId=F.Id ");
            sql.Append("LEFT JOIN dbo.RedPacket G WITH(NOLOCK) ON A.OrderCode=G.OrderCode ");
            sql.Append("WHERE 1=1   AND A.Status IS NOT NULL");//AND A.Status IS NOT NULL
            if (!string.IsNullOrEmpty(licenseNo))
            {
                sql.AppendFormat(" AND B.LicenseNo LIKE '%{0}%' ", licenseNo);
            }
            if (!string.IsNullOrEmpty(Name))
            {
                sql.AppendFormat(" AND Name LIKE '%{0}%' ", Name);
            }
            if (!string.IsNullOrEmpty(BillCode))
            {
                sql.AppendFormat(" AND A.OrderCode LIKE '%{0}%' ", BillCode);
            }
            if (!string.IsNullOrEmpty(Createtime.ToString()))
            {
                sql.AppendFormat(" AND A.CreateDate ='{0}' ", Createtime);
            }
            if (!string.IsNullOrEmpty(status.ToString()))
            {
                sql.AppendFormat(" AND A.status ={0} ", status);
            }
            sql.Append(") T");
            #endregion

            using (var db = new E2JOINDB())
            {
                int SumCount = 0;
                var query = db.Database.SqlQuery<OrderListCount>(sql.ToString()).ToList();
                if (query.Count > 0)
                {
                    SumCount = string.IsNullOrEmpty(query.FirstOrDefault().SumCount.ToString()) ? 0 : Convert.ToInt32(query.FirstOrDefault().SumCount);
                }
                return Math.Ceiling(Convert.ToDecimal(SumCount) / 10);
            }
        }
        public class BillList
        {
            public int Id { get; set; }
            public string OrderCode { get; set; }
            public Nullable<DateTime> CreateDate { get; set; }
            public Nullable<int> Status { get; set; }
            public Nullable<int> BackStatus { get; set; }
            public string LicenseNo { get; set; }
            public string Name { get; set; }
            public Nullable<int> IdCardType { get; set; }
            public Nullable<int> receiveStatus { get; set; }
            public string IdCard { get; set; }
            public string Mobile { get; set; }
            public Nullable<int> PayType { get;  set; }
            public Nullable<int> DeliverType { get; set; }
            public string ProductId { get; set; }
            public string UserId { get; set; }
            public Nullable<int> Channel { get; set; }
            public Nullable<decimal> ProductOriginalPrice { get; set; }
            public Nullable<decimal> ProductDealPrice { get; set; }
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ProductId"></param>
        /// <param name="OrderCode"></param>
        /// <returns></returns>
        public DataRow GetBillDetails(int? UserId,int? ProductId,string OrderCode)
        {
            using (var db = new E2JOINDB())
            {
                #region table
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("ModleName");
                dt.Columns.Add("ProductName");
                dt.Columns.Add("Mobile");
                dt.Columns.Add("ForceExpireDate");
                dt.Columns.Add("InvoiceTitle");
                dt.Columns.Add("InvoiceType");
                dt.Columns.Add("orderBaseId");
                dt.Columns.Add("OrderBaeCreateDate");
                dt.Columns.Add("itemId");
                dt.Columns.Add("BizRate");
                dt.Columns.Add("BizTotal");
                dt.Columns.Add("ForceRate");
                dt.Columns.Add("ForceTotal");
                dt.Columns.Add("TaxRate");
                dt.Columns.Add("TaxTotal");
                dt.Columns.Add("QuoteStatus");
                dt.Columns.Add("QuoteResult");
                dt.Columns.Add("CheSun_BaoE");
                dt.Columns.Add("CheSun_BaoFei");
                dt.Columns.Add("SanZhe_BaoE");
                dt.Columns.Add("SanZhe_BaoFei");
                dt.Columns.Add("DaoQiang_BaoE");
                dt.Columns.Add("DaoQiang_BaoFei");
                dt.Columns.Add("SiJi_BaoE");
                dt.Columns.Add("SiJi_BaoFei");
                dt.Columns.Add("ChengKe_BaoE");
                dt.Columns.Add("ChengKe_BaoFei");
                dt.Columns.Add("BoLi_BaoE");
                dt.Columns.Add("BoLi_BaoFei");
                dt.Columns.Add("HuaHen_BaoE");
                dt.Columns.Add("HuaHen_BaoFei");
                dt.Columns.Add("BuJiMianCheSun_BaoE");
                dt.Columns.Add("BuJiMianCheSun_BaoFei");
                dt.Columns.Add("BuJiMianSanZhe_BaoE");
                dt.Columns.Add("BuJiMianSanZhe_BaoFei");
                dt.Columns.Add("BuJiMianDaoQiang_BaoE");
                dt.Columns.Add("BuJiMianDaoQiang_BaoFei");
                dt.Columns.Add("BuJiMianRenYuan_BaoE");
                dt.Columns.Add("BuJiMianRenYuan_BaoFei");
                dt.Columns.Add("BuJiMianFuJia_BaoE");
                dt.Columns.Add("BuJiMianFuJia_BaoFei");
                dt.Columns.Add("SheShui_BaoE");
                dt.Columns.Add("SheShui_BaoFei");
                dt.Columns.Add("CheDeng_BaoE");
                dt.Columns.Add("CheDeng_BaoFei");
                dt.Columns.Add("ZiRan_BaoE");
                dt.Columns.Add("ZiRan_BaoFei");
                dt.Columns.Add("BuId");
                dt.Columns.Add("SubmitStatus");
                dt.Columns.Add("SubmitResult");
                dt.Columns.Add("BizNo");
                dt.Columns.Add("ForceNo");
                dt.Columns.Add("BizRate_Channel");
                dt.Columns.Add("ForceRate_Channel");
                dt.Columns.Add("TaxRate_Channel");
                dt.Columns.Add("BizAfterCoupon");
                dt.Columns.Add("ForceAfterCoupon");
                dt.Columns.Add("TaxAfterCoupon");
                dt.Columns.Add("Total");
                dt.Columns.Add("TotalAfterCoupon");
                dt.Columns.Add("AuditOrderStatus");
                dt.Columns.Add("Description");
                dt.Columns.Add("PrepaidAmount");
                dt.Columns.Add("PayType");
                dt.Columns.Add("OrderDeliverId");
                dt.Columns.Add("DeliverType");
                dt.Columns.Add("DeliverAddress");
                dt.Columns.Add("DeliverTime");
                dt.Columns.Add("DeliverPrice");
                dt.Columns.Add("DeliverName");
                dt.Columns.Add("DeliverMobile");
                dt.Columns.Add("DeliverDistrictCode");
                #endregion
                DataRow dr = dt.NewRow();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT A.Id,A.ModleName,A.Mobile,B.ProductName,A.ForceExpireDate, ");
                sql.Append("C.InvoiceTitle,C.InvoiceType,C.Id AS orderBaseId,C.CreateDate AS OrderBaeCreateDate,B.Id AS itemId, ");
                sql.Append("B.BizRate,B.BizTotal,B.ForceRate,B.ForceTotal,B.TaxRate,B.TaxTotal ");
                sql.Append(",B.QuoteStatus,B.QuoteResult,B.CheSun_BaoE,B.CheSun_BaoFei,B.SanZhe_BaoE ");
                sql.Append(",B.SanZhe_BaoFei,B.DaoQiang_BaoE,B.DaoQiang_BaoFei,B.SiJi_BaoE,B.SiJi_BaoFei ");
                sql.Append(",B.ChengKe_BaoE,B.ChengKe_BaoFei,B.BoLi_BaoE,B.BoLi_BaoFei,B.HuaHen_BaoE ");
                sql.Append(",B.HuaHen_BaoFei,B.BuJiMianCheSun_BaoE,B.BuJiMianCheSun_BaoFei,B.BuJiMianSanZhe_BaoE ");
                sql.Append(",B.BuJiMianSanZhe_BaoFei,B.BuJiMianDaoQiang_BaoE,B.BuJiMianDaoQiang_BaoFei ");
                sql.Append(",B.BuJiMianRenYuan_BaoE,B.BuJiMianRenYuan_BaoFei,B.BuJiMianFuJia_BaoE,B.BuJiMianFuJia_BaoFei ");
                sql.Append(",B.SheShui_BaoE,B.SheShui_BaoFei,B.CheDeng_BaoE,B.CheDeng_BaoFei,B.ZiRan_BaoE,B.ZiRan_BaoFei ");
                sql.Append(",B.BuId,B.SubmitStatus,B.SubmitResult,B.BizNo,B.ForceNo,B.BizRate_Channel,B.ForceRate_Channel ");
                sql.Append(",B.TaxRate_Channel,B.BizAfterCoupon,B.ForceAfterCoupon,B.TaxAfterCoupon,B.Total ");
                sql.Append(",B.TotalAfterCoupon,B.AuditOrderStatus,B.Description,B.PrepaidAmount,F.PayType,D.Id AS OrderDeliverId, ");
                sql.Append("D.OrderCode,D.DeliverType,D.DeliverAddress,D.DeliverTime,D.DeliverPrice,D.DeliverName ");
                sql.Append(",D.DeliverMobile,D.DeliverDistrictCode FROM dbo.ProductV2_User A WITH(NOLOCK) ");
                sql.AppendFormat("LEFT JOIN dbo.ProductV2_Item B WITH(NOLOCK) ON A.Id = B.UserId  ");
                sql.AppendFormat("LEFT JOIN dbo.Order_Base C WITH(NOLOCK) ON B.Id=C.ProductItemId AND C.OrderCode='{0}' ", OrderCode);
                sql.Append("LEFT JOIN dbo.Order_Deliver D WITH(NOLOCK) ON C.OrderCode=D.OrderCode ");
                sql.Append("LEFT JOIN dbo.Order_PolicyHolder E WITH(NOLOCK) ON C.OrderCode = E.OrderCode ");
                sql.Append("LEFT JOIN dbo.Order_Pay F WITH(NOLOCK) ON C.OrderCode = F.OrderCode ");
                sql.AppendFormat("WHERE B.Id={0} AND  c.UserId={1}  ", ProductId,UserId);
                var query = db.Database.SqlQuery<BillDetails>(sql.ToString());
                if(query.FirstOrDefault()!=null)
                {
                    dr["Id"]=query.FirstOrDefault().Id;
                    dr["ModleName"]=query.FirstOrDefault().ModleName;
                    dr["ProductName"] = query.FirstOrDefault().ProductName;
                    dr["Mobile"] = query.FirstOrDefault().Mobile;
                    dr["ForceExpireDate"]=query.FirstOrDefault().ForceExpireDate;
                    dr["InvoiceTitle"]=query.FirstOrDefault().InvoiceTitle;
                    dr["InvoiceType"]=query.FirstOrDefault().InvoiceType;
                    dr["orderBaseId"]=query.FirstOrDefault().orderBaseId;
                    dr["OrderBaeCreateDate"] = query.FirstOrDefault().OrderBaeCreateDate;
                    dr["itemId"]=query.FirstOrDefault().itemId;
                    dr["BizRate"]=query.FirstOrDefault().BizRate;
                    dr["BizTotal"]=query.FirstOrDefault().BizTotal;
                    dr["ForceRate"]=query.FirstOrDefault().ForceRate;
                    dr["ForceTotal"]=query.FirstOrDefault().ForceTotal;
                    dr["TaxRate"]=query.FirstOrDefault().TaxRate;
                    dr["TaxTotal"]=query.FirstOrDefault().TaxTotal;
                    dr["QuoteStatus"]=query.FirstOrDefault().QuoteStatus;
                    dr["QuoteResult"]=query.FirstOrDefault().QuoteResult;
                    dr["CheSun_BaoE"]=query.FirstOrDefault().CheSun_BaoE;
                    dr["CheSun_BaoFei"]=query.FirstOrDefault().CheSun_BaoFei;
                    dr["SanZhe_BaoE"]=query.FirstOrDefault().SanZhe_BaoE;
                    dr["SanZhe_BaoFei"]=query.FirstOrDefault().SanZhe_BaoFei;
                    dr["DaoQiang_BaoE"]=query.FirstOrDefault().DaoQiang_BaoE;
                    dr["DaoQiang_BaoFei"]=query.FirstOrDefault().DaoQiang_BaoFei;
                    dr["SiJi_BaoE"]=query.FirstOrDefault().SiJi_BaoE;
                    dr["SiJi_BaoFei"]=query.FirstOrDefault().SiJi_BaoFei;
                    dr["ChengKe_BaoE"]=query.FirstOrDefault().ChengKe_BaoE;
                    dr["ChengKe_BaoFei"]=query.FirstOrDefault().ChengKe_BaoFei;
                    dr["BoLi_BaoE"]=query.FirstOrDefault().BoLi_BaoE;
                    dr["BoLi_BaoFei"]=query.FirstOrDefault().BoLi_BaoFei;
                    dr["HuaHen_BaoE"]=query.FirstOrDefault().HuaHen_BaoE;
                    dr["HuaHen_BaoFei"]=query.FirstOrDefault().HuaHen_BaoFei;
                    dr["BuJiMianCheSun_BaoE"]=query.FirstOrDefault().BuJiMianCheSun_BaoE;
                    dr["BuJiMianCheSun_BaoFei"]=query.FirstOrDefault().BuJiMianCheSun_BaoFei;
                    dr["BuJiMianSanZhe_BaoE"]=query.FirstOrDefault().BuJiMianSanZhe_BaoE;
                    dr["BuJiMianSanZhe_BaoFei"]=query.FirstOrDefault().BuJiMianSanZhe_BaoFei;
                    dr["BuJiMianDaoQiang_BaoE"]=query.FirstOrDefault().BuJiMianDaoQiang_BaoE;
                    dr["BuJiMianDaoQiang_BaoFei"]=query.FirstOrDefault().BuJiMianDaoQiang_BaoFei;
                    dr["BuJiMianRenYuan_BaoE"]=query.FirstOrDefault().BuJiMianRenYuan_BaoE;
                    dr["BuJiMianRenYuan_BaoFei"]=query.FirstOrDefault().BuJiMianRenYuan_BaoFei;
                    dr["BuJiMianFuJia_BaoE"]=query.FirstOrDefault().BuJiMianFuJia_BaoE;
                    dr["BuJiMianFuJia_BaoFei"]=query.FirstOrDefault().BuJiMianFuJia_BaoFei;
                    dr["SheShui_BaoE"]=query.FirstOrDefault().SheShui_BaoE;
                    dr["SheShui_BaoFei"]=query.FirstOrDefault().SheShui_BaoFei;
                    dr["CheDeng_BaoE"]=query.FirstOrDefault().CheDeng_BaoE;
                    dr["CheDeng_BaoFei"]=query.FirstOrDefault().CheDeng_BaoFei;
                    dr["ZiRan_BaoE"]=query.FirstOrDefault().ZiRan_BaoE;
                    dr["ZiRan_BaoFei"]=query.FirstOrDefault().ZiRan_BaoFei;
                    dr["BuId"]=query.FirstOrDefault().BuId;
                    dr["SubmitStatus"]=query.FirstOrDefault().SubmitStatus;
                    dr["SubmitResult"]=query.FirstOrDefault().SubmitResult;
                    dr["BizNo"]=query.FirstOrDefault().BizNo;
                    dr["ForceNo"]=query.FirstOrDefault().ForceNo;
                    dr["BizRate_Channel"]=query.FirstOrDefault().BizRate_Channel;
                    dr["ForceRate_Channel"]=query.FirstOrDefault().ForceRate_Channel;
                    dr["TaxRate_Channel"]=query.FirstOrDefault().TaxRate_Channel;
                    dr["BizAfterCoupon"]=query.FirstOrDefault().BizAfterCoupon;
                    dr["ForceAfterCoupon"]=query.FirstOrDefault().ForceAfterCoupon;
                    dr["TaxAfterCoupon"]=query.FirstOrDefault().TaxAfterCoupon;
                    dr["Total"]=query.FirstOrDefault().Total;
                    dr["TotalAfterCoupon"]=query.FirstOrDefault().TotalAfterCoupon;
                    dr["AuditOrderStatus"]=query.FirstOrDefault().AuditOrderStatus;
                    dr["Description"]=query.FirstOrDefault().Description;
                    dr["PrepaidAmount"]=query.FirstOrDefault().PrepaidAmount;
                    dr["PayType"]=query.FirstOrDefault().PayType;
                    dr["OrderDeliverId"]=query.FirstOrDefault().OrderDeliverId;
                    dr["DeliverType"]=query.FirstOrDefault().DeliverType;
                    dr["DeliverAddress"]=query.FirstOrDefault().DeliverAddress;
                    dr["DeliverTime"]=query.FirstOrDefault().DeliverTime;
                    dr["DeliverPrice"]=query.FirstOrDefault().DeliverPrice;
                    dr["DeliverName"]=query.FirstOrDefault().DeliverName;
                    dr["DeliverMobile"]=query.FirstOrDefault().DeliverMobile;
                    dr["DeliverDistrictCode"]=query.FirstOrDefault().DeliverDistrictCode;
                }
                return dr;
            }
        }

        public class BillDetails
        {
            public int Id { get; set; }
            public string ModleName { get; set; }
            public string Mobile { get; set; }
            public string ProductName { get; set; }
            public string ForceExpireDate { get; set; }
            public string InvoiceTitle { get; set; }
            public Nullable<int> InvoiceType { get; set; }
            public Nullable<int> orderBaseId { get; set; }
            public Nullable<DateTime> OrderBaeCreateDate { get; set; }
            public Nullable<int> itemId { get; set; }
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
            public Nullable<decimal> DaoQiang_BaoE { get; set; }
            public Nullable<decimal> DaoQiang_BaoFei { get; set; }
            public Nullable<decimal> SiJi_BaoE { get; set; }
            public Nullable<decimal> SiJi_BaoFei { get; set; }
            public Nullable<decimal> ChengKe_BaoE { get; set; }
            public Nullable<decimal> ChengKe_BaoFei { get; set; }
            public Nullable<decimal> BoLi_BaoE { get; set; }
            public Nullable<decimal> BoLi_BaoFei { get; set; }
            public Nullable<decimal> HuaHen_BaoE { get; set; }
            public Nullable<decimal> HuaHen_BaoFei { get; set; }
            public Nullable<decimal> BuJiMianCheSun_BaoE { get; set; }
            public Nullable<decimal> BuJiMianCheSun_BaoFei { get; set; }
            public Nullable<decimal> BuJiMianSanZhe_BaoE { get; set; }
            public Nullable<decimal> BuJiMianSanZhe_BaoFei { get; set; }
            public Nullable<decimal> BuJiMianDaoQiang_BaoE { get; set; }
            public Nullable<decimal> BuJiMianDaoQiang_BaoFei { get; set; }
            public Nullable<decimal> BuJiMianRenYuan_BaoE { get; set; }
            public Nullable<decimal> BuJiMianRenYuan_BaoFei { get; set; }
            public Nullable<decimal> BuJiMianFuJia_BaoE { get; set; }
            public Nullable<decimal> BuJiMianFuJia_BaoFei { get; set; }
            public Nullable<decimal> SheShui_BaoE { get; set; }
            public Nullable<decimal> SheShui_BaoFei { get; set; }
            public Nullable<decimal> CheDeng_BaoE { get; set; }
            public Nullable<decimal> CheDeng_BaoFei { get; set; }
            public Nullable<decimal> ZiRan_BaoE { get; set; }
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
            public Nullable<int> AuditOrderStatus { get; set; }
            public string Description { get; set; }
            public Nullable<decimal> PrepaidAmount { get; set; }

            public Nullable<int> PayType { get; set; }
            public Nullable<int> OrderDeliverId { get; set; }
            public Nullable<int> DeliverType { get; set; }
            public string DeliverAddress { get; set; }
            public Nullable<System.DateTime> DeliverTime { get; set; }
            public Nullable<decimal> DeliverPrice { get; set; }
            public string DeliverName { get; set; }
            public string DeliverMobile { get; set; }
            public string DeliverDistrictCode { get; set; }
        }

        public int EditOrderStatus(int orderbaseid, string ordercode, int status)
        {
            int result = -1;
            Order_Base model = new Order_Base();
            model.Id = orderbaseid;
            model.OrderCode = ordercode;
            model.Status = status;
            using (var context = new E2JOINDB())
            {
                context.Order_Base.Attach(model);
                context.Entry(model).Property(t => t.Id).IsModified = true;
                context.Entry(model).Property(t => t.Status).IsModified = true;
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
        public int EditInvoice(Order_Base model)
        {
            int result = -1;
            using (var context = new E2JOINDB())
            {
                context.Order_Base.Attach(model);
                context.Entry(model).Property(t => t.Id).IsModified = true;
                context.Entry(model).Property(t => t.InvoiceTitle).IsModified = true;
                context.Entry(model).Property(t => t.InvoiceType).IsModified = true;
                context.SaveChanges();
                result = model.Id;
            }

            return result;
        }
        public int AddRedPacket(RedPacket model,int OrderBaseId)
        {
            int result = -1;
            using (var context = new E2JOINDB())
            {
                context.RedPacket.Add(model);
                Order_Base order_base = new Order_Base();
                order_base.Id = OrderBaseId;
                order_base.BackStatus = 1;
                order_base.OrderCode = model.OrderCode;
                context.Order_Base.Attach(order_base);
                context.Entry(order_base).Property(t => t.Id).IsModified = true;
                context.Entry(order_base).Property(t => t.BackStatus).IsModified = true;
                context.SaveChanges();
                result = model.Id;
            }
            return result;
        }
        public int EditRedPacket(string RedPacketCode,string openid,string infoStr,string BCNo)
        {
            int result = -1;
            using (var context = new E2JOINDB())
            {
                RedPacket redpacket = new RedPacket();
                redpacket=context.RedPacket.Where(c => c.RedPacketCode == RedPacketCode).FirstOrDefault();
                redpacket.OpenId = openid;
                redpacket.WX_UserInfo = infoStr;
                redpacket.State = 1;
                redpacket.SendTime = DateTime.Now;
                redpacket.BCNo = BCNo;
                context.SaveChanges();
                result = redpacket.Id;
            }
            return result;
        }
        public RedPacket GetRedPacket(string RedPacketCode)
        {
            using (var context = new E2JOINDB())
            {
                RedPacket redpacket = new RedPacket();
                redpacket = context.RedPacket.Where(c => c.RedPacketCode == RedPacketCode).FirstOrDefault();
                return redpacket;
            }
        }
        /// <summary>
        /// 获取报表
        /// </summary>
        /// <param name="OrderCode"></param>
        /// <param name="Page"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Source"></param>
        /// <param name="UserName"></param>
        /// <param name="LicenseNo"></param>
        /// <returns></returns>
        public DataTable GetReportList(string OrderCode, int Page, string BeginDate, string EndDate, string Source, string UserName, string LicenseNo)
        {
            #region SQL
            //            SELECT ob.OrderCode,ob.CreateDate,items.LicenseNo,ph.Name,v2Items.ProductName,v2Items.ForceNo,v2Items.BizNo,v2Items.BizRate_Channel,ForceRate_Channel,v2Items.ForceRate,v2Items.BizTotal,v2Items.TaxTotal,(ISNULL(v2Items.BizRate, 0) * ISNULL(v2Items.BizTotal, 0) / 100) + (ISNULL(v2Items.ForceRate, 0) * ISNULL(v2Items.ForceTotal, 0) / 100) + (ISNULL(v2Items.TaxRate, 0) * ISNULL(v2Items.TaxTotal, 0) / 100) AS 'Commission',ob.PrepaidAmount,ISNULL(v2Items.TotalAfterCoupon, 0) - (ISNULL(ob.PrepaidAmount, 0)) AS 'RemainMoney'
            //FROM dbo.Order_Base ob
            //LEFT JOIN dbo.Order_Item items ON ob.OrderCode = items.OrderCode
            //LEFT JOIN dbo.Order_PolicyHolder ph ON items.OrderCode = ph.OrderCode
            //LEFT JOIN dbo.ProductV2_Item v2Items ON items.ProductId = v2Items.Id
            //WHERE 1 = 1
            //ORDER BY ob.CreateDate DESC
            #endregion

            #region 查询条件
            StringBuilder where = new StringBuilder();
            if (!string.IsNullOrEmpty(UserName))
            {
                where.AppendFormat(" AND ph.Name LIKE '%{0}%'", UserName);
            }
            if (!string.IsNullOrEmpty(LicenseNo))
            {
                where.AppendFormat(" AND items.LicenseNo LIKE '%{0}%'", LicenseNo);
            }
            if (!string.IsNullOrEmpty(OrderCode))
            {
                where.AppendFormat(" AND ob.OrderCode LIKE '%{0}%'", OrderCode);
            }
            if (!string.IsNullOrEmpty(BeginDate) && !string.IsNullOrEmpty(EndDate))
            {
                where.AppendFormat(" AND ob.CreateDate BETWEEN '{0}' AND '{1}'", BeginDate, EndDate);
            }

            if (!string.IsNullOrEmpty(Source))
            {
                where.AppendFormat(" AND v2Items.Source ={0}", Source);
            }
            #endregion
            #region 查询内容
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM  ");
            sql.Append("(SELECT ROW_NUMBER() OVER(ORDER BY ob.CreateDate DESC) AS PageIndex, ");
            sql.Append("ob.OrderCode,");
            sql.Append("ob.CreateDate,");
            sql.Append("items.LicenseNo,");
            sql.Append("ph.Name,");
            sql.Append("v2Items.ProductName,");
            sql.Append("v2Items.ForceNo,");
            sql.Append("v2Items.BizNo,");
            sql.Append("ISNULL(v2Items.BizRate_Channel,0) as BizRate_Channel,");
            sql.Append("ISNULL(ForceRate_Channel,0) as ForceRate_Channel,");
            sql.Append("ISNULL(v2Items.ForceTotal,0) as ForceTotal,");
            sql.Append("ISNULL(v2Items.BizTotal,0) as BizTotal,");
            sql.Append("ISNULL(v2Items.TaxTotal,0) as TaxTotal,");
            sql.Append("(ISNULL(v2Items.BizRate, 0) * ISNULL(v2Items.BizTotal, 0) / 100) + (ISNULL(v2Items.ForceRate, 0) * ISNULL(v2Items.ForceTotal, 0) / 100) + (ISNULL(v2Items.TaxRate, 0) * ISNULL(v2Items.TaxTotal, 0) / 100) AS 'Commission',");
            sql.Append("ISNULL(ob.PrepaidAmount,0) as PrepaidAmount,");
            sql.Append("(ISNULL(v2Items.TotalAfterCoupon, 0) - ISNULL(ob.PrepaidAmount, 0) + 7) AS 'RemainMoney'");
            sql.Append(" FROM dbo.Order_Base ob");
            sql.Append(" LEFT JOIN dbo.Order_Item items ON ob.OrderCode = items.OrderCode");
            sql.Append(" LEFT JOIN dbo.Order_PolicyHolder ph ON items.OrderCode = ph.OrderCode");
            sql.Append(" LEFT JOIN dbo.ProductV2_Item v2Items ON items.ProductId = v2Items.Id");
            sql.Append(" Where 1=1");
            sql.Append(" AND ob.Status IN (3,4,5)");

            sql.Append(where.ToString());
            sql.AppendFormat(") T4 WHERE PageIndex BETWEEN {0} AND {1}", Page * 10 - 9, Page * 10);
            #endregion
            using (var db = new E2JOINDB())
            {
                DataTable dt = new DataTable();
                var query = db.Database.SqlQuery<ReportList>(sql.ToString());
                object o = query as object;
                dt.Columns.Add("OrderCode");
                dt.Columns.Add("CreateDate");
                dt.Columns.Add("LicenseNo");
                dt.Columns.Add("Name");
                dt.Columns.Add("ProductName");
                dt.Columns.Add("ForceNo");
                dt.Columns.Add("BizNo");
                dt.Columns.Add("BizRate_Channel");
                dt.Columns.Add("ForceRate_Channel");
                dt.Columns.Add("ForceTotal");
                dt.Columns.Add("BizTotal");
                dt.Columns.Add("TaxTotal");
                dt.Columns.Add("Commission");
                dt.Columns.Add("PrepaidAmount");
                dt.Columns.Add("RemainMoney");
                foreach (var item in query)
                {
                    DataRow dr = dt.NewRow();
                    dr["OrderCode"] = item.OrderCode;
                    dr["CreateDate"] = item.CreateDate;
                    dr["LicenseNo"] = item.LicenseNo;
                    dr["Name"] = item.Name;
                    dr["ProductName"] = item.ProductName;
                    dr["ForceNo"] = item.ForceNo;
                    dr["BizNo"] = item.BizNo;
                    dr["BizRate_Channel"] = item.BizRate_Channel;
                    dr["ForceRate_Channel"] = item.ForceRate_Channel;
                    dr["ForceTotal"] = item.ForceTotal;
                    dr["BizTotal"] = item.BizTotal;
                    dr["TaxTotal"] = item.TaxTotal;
                    dr["Commission"] = item.Commission;
                    dr["PrepaidAmount"] = item.PrepaidAmount;
                    dr["RemainMoney"] = item.RemainMoney;
                    dt.Rows.Add(dr);
                }
                return dt;
            }
        }

        public DataTable ExportReport(string OrderCode, string BeginDate, string EndDate, string Source, string UserName, string LicenseNo)
        {
            #region SQL
            //            SELECT ob.OrderCode,ob.CreateDate,items.LicenseNo,ph.Name,v2Items.ProductName,v2Items.ForceNo,v2Items.BizNo,v2Items.BizRate_Channel,ForceRate_Channel,v2Items.ForceRate,v2Items.BizTotal,v2Items.TaxTotal,(ISNULL(v2Items.BizRate, 0) * ISNULL(v2Items.BizTotal, 0) / 100) + (ISNULL(v2Items.ForceRate, 0) * ISNULL(v2Items.ForceTotal, 0) / 100) + (ISNULL(v2Items.TaxRate, 0) * ISNULL(v2Items.TaxTotal, 0) / 100) AS 'Commission',ob.PrepaidAmount,ISNULL(v2Items.TotalAfterCoupon, 0) - (ISNULL(ob.PrepaidAmount, 0)) AS 'RemainMoney'
            //FROM dbo.Order_Base ob
            //LEFT JOIN dbo.Order_Item items ON ob.OrderCode = items.OrderCode
            //LEFT JOIN dbo.Order_PolicyHolder ph ON items.OrderCode = ph.OrderCode
            //LEFT JOIN dbo.ProductV2_Item v2Items ON items.ProductId = v2Items.Id
            //WHERE 1 = 1
            //ORDER BY ob.CreateDate DESC
            #endregion

            #region 查询条件
            StringBuilder where = new StringBuilder();
            if (!string.IsNullOrEmpty(UserName))
            {
                where.AppendFormat(" AND ph.Name LIKE '%{0}%'", UserName);
            }
            if (!string.IsNullOrEmpty(LicenseNo))
            {
                where.AppendFormat(" AND items.LicenseNo LIKE '%{0}%'", LicenseNo);
            }
            if (!string.IsNullOrEmpty(OrderCode))
            {
                where.AppendFormat(" AND ob.OrderCode LIKE '%{0}%'", OrderCode);
            }
            if (!string.IsNullOrEmpty(BeginDate) && !string.IsNullOrEmpty(EndDate))
            {
                where.AppendFormat(" AND ob.CreateDate BETWEEN '{0}' AND '{1}'", BeginDate, EndDate);
            }

            if (!string.IsNullOrEmpty(Source))
            {
                where.AppendFormat(" AND v2Items.Source ={0}", Source);
            }
            #endregion
            #region 查询内容
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT  ");
            sql.Append("ob.OrderCode,");
            sql.Append("ob.CreateDate,");
            sql.Append("items.LicenseNo,");
            sql.Append("ph.Name,");
            sql.Append("v2Items.ProductName,");
            sql.Append("v2Items.ForceNo,");
            sql.Append("v2Items.BizNo,");
            sql.Append("ISNULL(v2Items.BizRate_Channel,0) as BizRate_Channel,");
            sql.Append("ISNULL(ForceRate_Channel,0) as ForceRate_Channel,");
            sql.Append("ISNULL(v2Items.ForceTotal,0) as ForceTotal,");
            sql.Append("ISNULL(v2Items.BizTotal,0) as BizTotal,");
            sql.Append("ISNULL(v2Items.TaxTotal,0) as TaxTotal,");
            sql.Append("(ISNULL(v2Items.BizRate, 0) * ISNULL(v2Items.BizTotal, 0) / 100) + (ISNULL(v2Items.ForceRate, 0) * ISNULL(v2Items.ForceTotal, 0) / 100) + (ISNULL(v2Items.TaxRate, 0) * ISNULL(v2Items.TaxTotal, 0) / 100) AS 'Commission',");
            sql.Append("ISNULL(ob.PrepaidAmount,0) as PrepaidAmount,");
            sql.Append("ISNULL(v2Items.TotalAfterCoupon, 0) - (ISNULL(ob.PrepaidAmount, 0) + 7) AS 'RemainMoney'");
            sql.Append(" FROM dbo.Order_Base ob");
            sql.Append(" LEFT JOIN dbo.Order_Item items ON ob.OrderCode = items.OrderCode");
            sql.Append(" LEFT JOIN dbo.Order_PolicyHolder ph ON items.OrderCode = ph.OrderCode");
            sql.Append(" LEFT JOIN dbo.ProductV2_Item v2Items ON items.ProductId = v2Items.Id");
            sql.Append(" Where 1=1");
            sql.Append(" AND ob.Status IN (3,4,5)");
            sql.Append(where.ToString());
            sql.Append(" ORDER BY ob.CreateDate DESC");
            #endregion
            using (var db = new E2JOINDB())
            {
                DataTable dt = new DataTable();
                var query = db.Database.SqlQuery<ReportList>(sql.ToString());
                object o = query as object;
                dt.Columns.Add("订单编号");
                dt.Columns.Add("出单日期");
                dt.Columns.Add("车牌号");
                dt.Columns.Add("被保人");
                dt.Columns.Add("承保公司");
                dt.Columns.Add("交强险保单号");
                dt.Columns.Add("商业险保单号");
                dt.Columns.Add("商业险平台费率");
                dt.Columns.Add("交强险平台费率");
                dt.Columns.Add("交强险保险费");
                dt.Columns.Add("商业险保险费");
                dt.Columns.Add("车船税保险费");
                dt.Columns.Add("佣金");
                dt.Columns.Add("预付金额");
                dt.Columns.Add("待付金额");
                foreach (var item in query)
                {
                    DataRow dr = dt.NewRow();
                    dr["订单编号"] = item.OrderCode;
                    dr["出单日期"] = item.CreateDate;
                    dr["车牌号"] = item.LicenseNo;
                    dr["被保人"] = item.Name;
                    dr["承保公司"] = item.ProductName;
                    dr["交强险保单号"] = item.ForceNo;
                    dr["商业险保单号"] = item.BizNo;
                    dr["商业险平台费率"] = Convert.ToDecimal(item.BizRate_Channel).ToString("F2");
                    dr["交强险平台费率"] = Convert.ToDecimal(item.ForceRate_Channel).ToString("F2");
                    dr["交强险保险费"] = Convert.ToDecimal(item.ForceTotal).ToString("F2");
                    dr["商业险保险费"] = Convert.ToDecimal(item.BizTotal).ToString("F2");
                    dr["车船税保险费"] = Convert.ToDecimal(item.TaxTotal).ToString("F2");
                    dr["佣金"] = Convert.ToDecimal(item.Commission).ToString("F2");
                    dr["预付金额"] = Convert.ToDecimal(item.PrepaidAmount).ToString("F2");
                    dr["待付金额"] = Convert.ToDecimal(item.RemainMoney).ToString("F2");
                    dt.Rows.Add(dr);
                }
                return dt;
            }
        }

        public DataTable GetReportSummary(string OrderCode, string BeginDate, string EndDate, string Source, string UserName, string LicenseNo)
        {
            #region 查询条件
            StringBuilder where = new StringBuilder();
            if (!string.IsNullOrEmpty(UserName))
            {
                where.AppendFormat(" AND ph.Name LIKE '%{0}%'", UserName);
            }
            if (!string.IsNullOrEmpty(LicenseNo))
            {
                where.AppendFormat(" AND items.LicenseNo LIKE '%{0}%'", LicenseNo);
            }
            if (!string.IsNullOrEmpty(OrderCode))
            {
                where.AppendFormat(" AND ob.OrderCode LIKE '%{0}%'", OrderCode);
            }
            if (!string.IsNullOrEmpty(BeginDate) && !string.IsNullOrEmpty(EndDate))
            {
                where.AppendFormat(" AND ob.CreateDate BETWEEN '{0}' AND '{1}'", BeginDate, EndDate);
            }

            if (!string.IsNullOrEmpty(Source))
            {
                where.AppendFormat(" AND v2Items.Source ={0}", Source);
            }
            #endregion
            #region 查询内容
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            sql.Append("ISNULL(SUM(v2Items.ForceTotal),0) ForceTotal,");
            sql.Append("ISNULL(SUM(v2Items.BizTotal),0) BizTotal,");
            sql.Append("ISNULL(SUM(v2Items.TaxTotal),0) TaxTotal,");
            sql.Append("ISNULL(SUM((ISNULL(v2Items.BizRate, 0) * ISNULL(v2Items.BizTotal, 0) / 100) + (ISNULL(v2Items.ForceRate, 0) * ISNULL(v2Items.ForceTotal, 0) / 100) + (ISNULL(v2Items.TaxRate, 0) * ISNULL(v2Items.TaxTotal, 0) / 100)),0) AS 'Commission',");
            sql.Append("ISNULL(SUM(ob.PrepaidAmount),0) PrepaidAmount,");
            sql.Append("(ISNULL(SUM(ISNULL(v2Items.TotalAfterCoupon, 0) - (ISNULL(ob.PrepaidAmount, 0))),0) + COUNT(ob.Id)*7) AS 'RemainMoney'");
            sql.Append(" FROM dbo.Order_Base ob");
            sql.Append(" LEFT JOIN dbo.Order_Item items ON ob.OrderCode = items.OrderCode");
            sql.Append(" LEFT JOIN dbo.Order_PolicyHolder ph ON items.OrderCode = ph.OrderCode");
            sql.Append(" LEFT JOIN dbo.ProductV2_Item v2Items ON items.ProductId = v2Items.Id");
            sql.Append(" Where 1=1");
            sql.Append(" AND ob.Status IN (3,4,5)");
            sql.Append(where.ToString());
            #endregion
            using (var db = new E2JOINDB())
            {
                DataTable dt = new DataTable();
                var query = db.Database.SqlQuery<ReportSummary>(sql.ToString());
                object o = query as object;
                dt.Columns.Add("ForceTotal");
                dt.Columns.Add("BizTotal");
                dt.Columns.Add("TaxTotal");
                dt.Columns.Add("Commission");
                dt.Columns.Add("PrepaidAmount");
                dt.Columns.Add("RemainMoney");
                foreach (var item in query)
                {
                    DataRow dr = dt.NewRow();
                    dr["ForceTotal"] = item.ForceTotal;
                    dr["BizTotal"] = item.BizTotal;
                    dr["TaxTotal"] = item.TaxTotal;
                    dr["Commission"] = item.Commission;
                    dr["PrepaidAmount"] = item.PrepaidAmount;
                    dr["RemainMoney"] = item.RemainMoney;
                    dt.Rows.Add(dr);
                }
                return dt;
            }
        }

        public decimal GetReportListCount(string OrderCode, int Page, string BeginDate, string EndDate, string Source, string UserName, string LicenseNo)
        {
            #region 查询条件
            StringBuilder where = new StringBuilder();
            if (!string.IsNullOrEmpty(UserName))
            {
                where.AppendFormat(" AND ph.Name LIKE '%{0}%'", UserName);
            }
            if (!string.IsNullOrEmpty(LicenseNo))
            {
                where.AppendFormat(" AND items.LicenseNo LIKE '%{0}%'", LicenseNo);
            }
            if (!string.IsNullOrEmpty(OrderCode))
            {
                where.AppendFormat(" AND ob.OrderCode LIKE '%{0}%'", OrderCode);
            }
            if (!string.IsNullOrEmpty(BeginDate) && !string.IsNullOrEmpty(EndDate))
            {
                where.AppendFormat(" AND ob.CreateDate BETWEEN '{0}' AND '{1}'", BeginDate, EndDate);
            }

            if (!string.IsNullOrEmpty(Source))
            {
                where.AppendFormat(" AND v2Items.Source ={0}", Source);
            }
            #endregion
            #region 查询内容
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT COUNT(*) AS SumCount  ");
            //sql.Append("(SELECT ROW_NUMBER() OVER(ORDER BY ob.CreateDate DESC) AS PageIndex ");
            //sql.Append("ob.OrderCode,");
            //sql.Append("ob.CreateDate,");
            //sql.Append("items.LicenseNo,");
            //sql.Append("ph.Name,");
            //sql.Append("v2Items.ProductName,");
            //sql.Append("v2Items.ForceNo,");
            //sql.Append("v2Items.BizNo,");
            //sql.Append("v2Items.BizRate_Channel,");
            //sql.Append("ForceRate_Channel,");
            //sql.Append("v2Items.ForceRate,");
            //sql.Append("v2Items.ForceTotal,");
            //sql.Append("v2Items.BizTotal,");
            //sql.Append("v2Items.TaxTotal,");
            //sql.Append("(ISNULL(v2Items.BizRate, 0) * ISNULL(v2Items.BizTotal, 0) / 100) + (ISNULL(v2Items.ForceRate, 0) * ISNULL(v2Items.ForceTotal, 0) / 100) + (ISNULL(v2Items.TaxRate, 0) * ISNULL(v2Items.TaxTotal, 0) / 100) AS 'Commission',");
            //sql.Append("ob.PrepaidAmount,");
            //sql.Append("ISNULL(v2Items.TotalAfterCoupon, 0) - (ISNULL(ob.PrepaidAmount, 0)) AS 'RemainMoney'");
            sql.Append(" FROM dbo.Order_Base ob");
            sql.Append(" LEFT JOIN dbo.Order_Item items ON ob.OrderCode = items.OrderCode");
            sql.Append(" LEFT JOIN dbo.Order_PolicyHolder ph ON items.OrderCode = ph.OrderCode");
            sql.Append(" LEFT JOIN dbo.ProductV2_Item v2Items ON items.ProductId = v2Items.Id");
            sql.Append(" Where 1=1");
            sql.Append(" AND ob.Status IN (3,4,5)");
            sql.Append(where.ToString());
            #endregion
            using (var db = new E2JOINDB())
            {
                int SumCount = 0;
                var query = db.Database.SqlQuery<OrderListCount>(sql.ToString()).ToList();
                if (query.Count > 0)
                {
                    SumCount = string.IsNullOrEmpty(query.FirstOrDefault().SumCount.ToString()) ? 0 : Convert.ToInt32(query.FirstOrDefault().SumCount);
                }
                return Math.Ceiling(Convert.ToDecimal(SumCount) / 10);
            }
        }

        public class ReportSummary
        {
            public decimal? ForceTotal { get; set; }
            public decimal? BizTotal { get; set; }
            public decimal? TaxTotal { get; set; }
            public decimal? Commission { get; set; }
            public decimal? PrepaidAmount { get; set; }
            public decimal? RemainMoney { get; set; }
        }

        public class ReportList
        {
            public string OrderCode { get; set; }
            public DateTime? CreateDate { get; set; }
            public string LicenseNo { get; set; }
            public string Name { get; set; }
            public string ProductName { get; set; }
            public string ForceNo { get; set; }
            public string BizNo { get; set; }
            public decimal? BizRate_Channel { get; set; }
            public decimal? ForceRate_Channel { get; set; }
            public decimal? ForceTotal { get; set; }
            public decimal? BizTotal { get; set; }
            public decimal? TaxTotal { get; set; }
            public decimal? Commission { get; set; }
            public decimal? PrepaidAmount { get; set; }
            public decimal? RemainMoney { get; set; }
        }

    }

}
