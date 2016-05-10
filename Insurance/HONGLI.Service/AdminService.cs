using HONGLI.Entity;
using I.Utility.Extensions;
using I.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Service
{
  public  class AdminService
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
                var repository = new Repository.AdminRepository();

                returnValue = repository.CheckUser(username, password);
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
            DataTable returnValue=new DataTable();
            try
            {
                var repository = new Repository.AdminRepository();
                returnValue = repository.GetOrderList( Page,  CarID,  username,  ChannelName,  BeginInsuranceEndDate,  EndInsuranceEndDate,  BeginVisitDate,  EndVisitDate,  BeginDate,  EndDate,  InsuranceCompany,  VistState,  Auditdocuments,  OrderState,examineState);
                return returnValue;
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
                return returnValue;           }

        }
        public decimal GetOrderListCount(string CarID, string username, string ChannelName, string BeginInsuranceEndDate, string EndInsuranceEndDate, string BeginVisitDate, string EndVisitDate, string BeginDate, string EndDate, string InsuranceCompany, string VistState, string Auditdocuments, string OrderState, string examineState)
        {
            return new Repository.AdminRepository().GetOrderListCount(CarID, username, ChannelName, BeginInsuranceEndDate, EndInsuranceEndDate, BeginVisitDate, EndVisitDate, BeginDate, EndDate, InsuranceCompany, VistState, Auditdocuments, OrderState, examineState);
        }
        /// <summary>
        /// 获取用户选择核保信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ProductV2_Item GetProductItemSelection(int? UserId)
        {
            var repository = new Repository.AdminRepository();
            return repository.GetProductItemSelection(UserId);
        }
        /// <summary>
        /// 获取用户信息（车牌号）
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ProductV2_User GetProductUser(int? ID)
        {
            var repository = new Repository.AdminRepository();
            return repository.GetProductUser(ID);
        }
        /// <summary>
        /// 根据手机号获取用户ID
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public BaseInfo_UserInfo GetUserIdByMobile(string mobile)
        {
            var repository = new Repository.AdminRepository();
            return repository.GetUserIdByMobile(mobile);
        }
        /// <summary>
        /// 获取用户续保信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ProductV2_Renewal GetProductRenewal(int? ID)
        {
            var repository = new Repository.AdminRepository();
            return repository.GetProductRenewal(ID);
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditProductUser(ProductV2_User model)
        {
            int result;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.EditProductUser(model);
            }
            catch (Exception ex)
            {
                result = -1;
                //todo log
                LogHelper.AppError(string.Format("修改产品详细SaveProductUser异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
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
            int result;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.EditOrder_Base(model);
            }
            catch (Exception ex)
            {
                result = -1;
                //todo log
                LogHelper.AppError(string.Format("修改订单order_base详细SaveProductUser异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
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
            var repository = new Repository.AdminRepository();
            return repository.GetProductItem(UserId,InsuranceCompany);
        }
        /// <summary>
        /// 修改核保信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditProductItem(ProductV2_Item model)
        {
            int result;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.EditProductItem(model);
            }
            catch (Exception ex)
            {
                result = -1;
                //todo log
                LogHelper.AppError(string.Format("保存产品详细SaveProductUser异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }
            return result;
        }
        /// <summary>
        /// 修改续保信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditProductRenewal(ProductV2_Renewal model)
        {
            int result;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.EditProductRenewal(model);
            }
            catch (Exception ex)
            {
                result = -1;
                //todo log
                LogHelper.AppError(string.Format("保存产品详细SaveProductUser异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
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
            int result;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.EditPolicyHolder(model);
            }
            catch (Exception ex)
            {
                result = -1;
                //todo log
                LogHelper.AppError(string.Format("保存产品详细SaveProductUser异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
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
            int result;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.SavePolicyHolder(model);
            }
            catch (Exception ex)
            {
                result = -1;
                //todo log
                LogHelper.AppError(string.Format("保存产品详细SaveProductUser异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }
            return result;
        }

        /// <summary>
        /// 添加核保信息
        /// </summary>
        /// <param name="product_user"></param>
        /// <returns></returns>
        public int SaveProductItem(ProductV2_Item productv2_item)
        {
            int userid;
            try
            {
                var repository = new Repository.AdminRepository();
                userid = repository.SaveProductItem(productv2_item);
            }
            catch (Exception ex)
            {
                userid = -1;
                //todo log
                LogHelper.AppError(string.Format("保存产品详细SaveProductUser异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }
            return userid;
        }
        /// <summary>
        /// 修改订单地址信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditOrderDeliver(Order_Deliver model)
        {
            int result;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.EditOrderDeliver(model);
            }
            catch (Exception ex)
            {
                result = -1;
                //todo log
                LogHelper.AppError(string.Format("修改订单地址详细EditOrderDeliver异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
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
            int result;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.EditOrderPay(model);
            }
            catch (Exception ex)
            {
                result = -1;
                //todo log
                LogHelper.AppError(string.Format("EditOrderPay，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }
            return result;
        }
        public int SaveOrderDeliver(Order_Deliver order_deliver)
        {
            int userid;
            try
            {
                var repository = new Repository.AdminRepository();
                userid = repository.SaveOrderDeliver(order_deliver);
            }
            catch (Exception ex)
            {
                userid = -1;
                //todo log
                LogHelper.AppError(string.Format("保存产品详细SaveProductUser异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }
            return userid;
        }

        /// <summary>
        /// 判断收货地址是否存在
        /// </summary>
        /// <param name="ordercode"></param>
        /// <returns></returns>
        public int CHeckDeliverAdd(string ordercode)
        {
            int deliverid;
            try
            {
                var repository = new Repository.AdminRepository();
                deliverid = repository.CHeckDeliverAdd(ordercode);
            }
            catch (Exception ex)
            {
                deliverid = -1;
                LogHelper.Info(ex.Message);
            }
            return deliverid;
        }
        /// <summary>
        /// 判断被保人信息是否存在
        /// </summary>
        /// <param name="ordercode"></param>
        /// <returns></returns>
        public int CHeckpolicyHolderAdd(string ordercode)
        {
            int policyHoldeId;
            try
            {
                var repository = new Repository.AdminRepository();
                policyHoldeId = repository.CHeckpolicyHolderAdd(ordercode);
            }
            catch (Exception ex)
            {
                policyHoldeId = -1;
                LogHelper.Info(ex.Message);
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
            int payId;
            try
            {
                var repository = new Repository.AdminRepository();
                payId = repository.CHeckpayAdd(ordercode);
            }
            catch (Exception ex)
            {
                payId = -1;
                LogHelper.Info(ex.Message);
            }
            return payId;
        }
        /// <summary>
        /// 核单
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ItemId"></param>
        /// <returns></returns>
        public int ChangeAuditDocuments(int UserId, int ItemId)
        {
            int result;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.ChangeAuditDocuments(UserId, ItemId);
            }
            catch (Exception ex)
            {
                result = -1;
                //todo log
                LogHelper.AppError(string.Format("核单详细ChangeAuditDocuments异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
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
            int result;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.ChangeOrderList(order_base);
            }
            catch (Exception ex)
            {
                result = -1;
                //todo log
                LogHelper.AppError(string.Format("核单通过后修改订单内容详细ChangeOrderList异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
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
        public DataTable GetBillList(int Page, string licenseNo, string Name, string BillCode, DateTime? Createtime, int? status)
        {
            DataTable returnValue = new DataTable();
            try
            {
                var repository = new Repository.AdminRepository();
                returnValue = repository.GetBillList(Page, licenseNo, Name, BillCode, Createtime, status);
                return returnValue;
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
                return returnValue;
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
            return new Repository.AdminRepository().GetBillListCount(licenseNo, Name, BillCode, Createtime, status);
        }
        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ProductId"></param>
        /// <param name="OrderCode"></param>
        /// <returns></returns>
        public DataRow GetBillDetails(int? UserId, int? ProductId, string OrderCode)
        {
            return new Repository.AdminRepository().GetBillDetails(UserId, ProductId, OrderCode);
        }
        public int EditOrderStatus(int orderbaseid, string ordercode, int status)
        {
            int result = -1;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.EditOrderStatus(orderbaseid, ordercode, status);
                return result;
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
                return result;
            }
        }
        /// <summary>
        /// 修改发票信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditInvoice(Order_Base model)
        {
            int result;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.EditInvoice(model);
            }
            catch (Exception ex)
            {
                result = -1;
                //todo log
                LogHelper.AppError(string.Format("核单通过后修改订单内容详细ChangeOrderList异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }
            return result;
        }
        public int AddRedPacket(RedPacket model, int OrderBaseId)
        {
            int result;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.AddRedPacket(model, OrderBaseId);
                AddLog("生成红包码，message：" + model.RedPacketName, "生成红包码，message：" + model.RedPacketName, 12, Convert.ToInt32(model.CreateName));
            }
            catch (Exception ex)
            {
                result = -1;
                //todo log
                LogHelper.AppError(string.Format("生成红包码异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }
            return result;
        }
        public int EditRedPacket(string RedPacketCode, string openid, string infoStr,string BCNo)
        {
            int result;
            try
            {
                var repository = new Repository.AdminRepository();
                result = repository.EditRedPacket(RedPacketCode, openid, infoStr, BCNo);
                AddLog("用户领取返现，openid：" + openid, "红包码：" + RedPacketCode, 12, 0);
            }
            catch (Exception ex)
            {
                result = -1;
                //todo log
                LogHelper.AppError(string.Format("生成红包码异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }
            return result;
        }
        public RedPacket GetRedPacket(string RedPacketCode)
        {
            RedPacket redpacket = new RedPacket();
            try
            {
                var repository = new Repository.AdminRepository();
                redpacket = repository.GetRedPacket(RedPacketCode);
            }
            catch (Exception ex)
            {
                //todo log
                LogHelper.AppError(string.Format("生成红包码异常，异常信息：{0}，异常跟踪：{1}.", ex.Message, ex.StackTrace));
            }
            return redpacket;
        }
        public void AddLog(string info, string memo, byte type, int createMemberId)
        {
            try
            {
                using (var context = new E2JOINDB())
                {
                    var log = new SYS_LogInfo
                    {
                        ActionMemo = memo,
                        ActionType = type,
                        CreateMemberID = createMemberId,
                        ActionMsg = info
                    };

                    context.SYS_LogInfo.Add(log);

                    context.SaveChanges();
                }
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
            }
        }

        public DataTable GetReportSummary(string OrderCode, string BeginDate, string EndDate, string Source, string UserName, string LicenseNo)
        {
            DataTable returnValue = new DataTable();
            try
            {
                var repository = new Repository.AdminRepository();
                returnValue = repository.GetReportSummary(OrderCode, BeginDate, EndDate, Source, UserName, LicenseNo);
                return returnValue;
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
                return returnValue;
            }
        }

        public DataTable GetReportList(string OrderCode, int Page, string BeginDate, string EndDate, string Source, string UserName, string LicenseNo)
        {
            DataTable returnValue = new DataTable();
            try
            {
                var repository = new Repository.AdminRepository();
                returnValue = repository.GetReportList(OrderCode, Page, BeginDate, EndDate, Source, UserName, LicenseNo);
                return returnValue;
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
                return returnValue;
            }
        }

        public DataTable ExportReport(string OrderCode, string BeginDate, string EndDate, string Source, string UserName, string LicenseNo)
        {
            DataTable returnValue = new DataTable();
            try
            {
                var repository = new Repository.AdminRepository();
                returnValue = repository.ExportReport(OrderCode, BeginDate, EndDate, Source, UserName, LicenseNo);
                return returnValue;
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
                return returnValue;
            }
        }

        public decimal GetReportListCount(string OrderCode, int Page, string BeginDate, string EndDate, string Source, string UserName, string LicenseNo)
        {
            try
            {
                return new Repository.AdminRepository().GetReportListCount(OrderCode, Page, BeginDate, EndDate, Source, UserName, LicenseNo);
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
    }
}
