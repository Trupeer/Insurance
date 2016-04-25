using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HONGLI.Web.Models.User
{
    public class InvoiceModel
    {

        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Context { get; set; }
        public int Type { get; set; }

        public int ProductId { get; set; }

        public string Mobile { get; set; }
        public string Channel { get; set; }

        public string ReturnUrl { get; set; }
        public int? intentionCompany { get; set; }

        public string OrderCode { get; set; }
        public int? OrderBaseId { get; set; }
        public int? OrderItemId { get; set; }
        public int? OrderPolicyId { get; set; }
        public int? OrderDeliverId { get; set; }
    }
}