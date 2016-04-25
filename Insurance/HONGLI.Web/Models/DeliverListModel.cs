using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HONGLI.Web.Models
{
    public class DeliverListModel
    {
        public DeliverListModel()
        {
            DeliverList = new List<DeliverModel>();
        }

        public List<DeliverModel> DeliverList { get; set; }

        public int UserId { get; set; }

    }
}