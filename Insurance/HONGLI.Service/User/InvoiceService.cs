using HONGLI.Entity;
using HONGLI.Repository.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Service.User
{
    /// <summary>
    /// 发票信息业务逻辑静态类。
    /// </summary>
    public class InvoiceService
    {
        private static InvoiceRepository _invoiceRepository = null;

        public InvoiceService()
        {
            _invoiceRepository = new InvoiceRepository();
        }

        /// <summary>
        /// 获取发票信息。
        /// </summary>
        /// <param name="userId">成员信息编号。</param>
        /// <returns></returns>
        public List<User_Invoice> GetDeliverByUser(int userId)
        {
            return _invoiceRepository.GetInvoiceByUser(userId);
        }

        /// <summary>
        ///  获取配送信息。
        /// </summary>
        /// <param name="id">成员信息编号。</param>
        /// <returns></returns>
        public User_Invoice GetDeliverById(int id)
        {
            return _invoiceRepository.GetInvoiceById(id);
        }

        /// <summary>
        /// 新增发票信息。
        /// </summary>
        /// <param name="invoice">发票信息实体。</param>
        /// <returns></returns>
        public int AddInvoice(User_Invoice invoice)
        {
            return _invoiceRepository.AddInvoice(invoice);
        }

        /// <summary>
        /// 更新发票信息。
        /// </summary>
        /// <param name="invoice">发票信息实体。</param>
        /// <returns></returns>
        public int UpdateInvoice(User_Invoice invoice)
        {
            return _invoiceRepository.UpdateInvoice(invoice);
        }
    }
}
