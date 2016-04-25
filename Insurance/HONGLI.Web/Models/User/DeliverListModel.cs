using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HONGLI.Web.Models.User
{
    public class DeliverListModel
    {
        public DeliverListModel()
        {
            DeliverList = new List<DeliverModel>();
        }

        public List<DeliverModel> DeliverList { get; set; }

        public int UserId { get; set; }
        public int ProductId { get; set; }

        public string Mobile { get; set; }
        public string Channel { get; set; }
        /// <summary>
        /// 返回订单 Url。
        /// </summary>
        public string PageUrl { get; set; }

        public  int? intentionCompany { get; set; }

        public string OrderCode { get; set; }
        public int? OrderBaseId { get; set; }
        public int? OrderItemId { get; set; }
        public int? OrderPolicyId { get; set; }
        public int? OrderDeliverId { get; set; }

    }
}