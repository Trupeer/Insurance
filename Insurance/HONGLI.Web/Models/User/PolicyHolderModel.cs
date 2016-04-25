using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Web.Models.User
{
    public class PolicyHolderModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IdCard { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? IdCardType { get; set; }
        public string IdCardFacePicUrl { get; set; }
        public string IdCardBackPicUrl { get; set; }
        public string OrganizationCode { get; set; }
        public int? UserId { get; set; }
        public string ReturnUrl { get; set; }
        public int MaxUploadSize { get; set; }
        public int ProductId { get; set; }
        public string Mobile { get; set; }
        public string Channel { get; set; }

        public int? ViewType { get; set; }

        public string UploadUrl { get; set; }
        public string Error { get; set; }

        public int? intentionCompany { get; set; }

        public string OrderCode { get; set; }
        public int? OrderBaseId { get; set; }
        public int? OrderItemId { get; set; }
        public int? OrderPolicyId { get; set; }
        public int? OrderDeliverId { get; set; }

    }
}
