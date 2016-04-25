using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Web.Models
{
    public class PolicyHolderModel
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string Name { get; set; }
        public string IdCard { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? IdCardType { get; set; }
        public string IdCardFacePicUrl { get; set; }
        public string IdCardBackPicUrl { get; set; }
    }
}
