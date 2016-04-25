using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HONGLI.Entity;

namespace HONGLI.Repository.User
{
    /// <summary>
    /// 发票信息数据访问类。
    /// </summary>
    public class InvoiceRepository
    {
        /// <summary>
        /// 获取发票信息。
        /// </summary>
        /// <param name="userId">成员信息编号。</param>
        /// <returns></returns>
        public List<User_Invoice> GetInvoiceByUser(int userId)
        {
            List<User_Invoice> invoiceList = new List<User_Invoice>();
            try
            {
                using (var context = new E2JOINDB())
                {

                    var query = context.User_Invoice
                        .Where(c => c.UserId == userId)
                        .ToList();

                    invoiceList = query;
                }
            }
            catch (Exception)
            {
                //LogHelper.AppError(error.Message);
            }
            return invoiceList;
        }

        /// <summary>
        /// 获取配送信息。
        /// </summary>
        /// <param name="id">成员信息编号。</param>
        /// <returns></returns>
        public User_Invoice GetInvoiceById(int id)
        {
            User_Invoice invoice = new User_Invoice();
            try
            {
                using (var context = new E2JOINDB())
                {
                    var query = context.User_Invoice
                        .Where(c => c.Id == id)
                        .FirstOrDefault();

                    invoice = query;
                }
            }
            catch (Exception)
            {
                //LogHelper.AppError(error.Message);
            }
            return invoice;
        }

        /// <summary>
        /// 新增发票信息。
        /// </summary>
        /// <param name="invoice">发票信息实体。</param>
        /// <returns></returns>
        public int AddInvoice(User_Invoice invoice)
        {
            int invoiceid = 0;
            try
            {
                using (var context = new E2JOINDB())
                {
                    context.User_Invoice.Add(invoice);
                    invoiceid = context.SaveChanges();
                }
            }
            catch (Exception)
            {
                //LogHelper.AppError(error.Message);
            }
            return invoiceid;
        }
        /// <summary>
        /// 更新发票信息。
        /// </summary>
        /// <param name="invoice">invoice</param>
        /// <returns></returns>
        public int UpdateInvoice(User_Invoice invoice)
        {
            int invoiceid = 0;
            try
            {
                using (var context = new E2JOINDB())
                {
                    var query = context.User_Invoice
                       .Where(c => c.Id == invoice.Id)
                       .FirstOrDefault();
                    if (query != null)
                    {
                        query.Id = invoice.Id;
                        query.Context = invoice.Context;
                        query.UserId = invoice.UserId;
                        query.Type = invoice.Type;
                    }
                    invoiceid = context.SaveChanges();
                }
            }
            catch (Exception)
            {
                //LogHelper.AppError(error.Message);
            }
            return invoiceid;
        }
    }
}
