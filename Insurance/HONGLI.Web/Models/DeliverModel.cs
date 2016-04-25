using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HONGLI.Web.Models
{
    public class DeliverModel
    {


        public int Id { get; set; }
        public string OrderCode { get; set; }
        public int? DeliverType { get; set; }
        [Required]
        [Display(Name = "详细地址")]
        public string DeliverAddress { get; set; }
        public DateTime? DeliverTime { get; set; }
        public decimal? DeliverPrice { get; set; }
        [Required]
        [Display(Name = "收件人")]
        public string DeliverName { get; set; }
        public DateTime? CreateDate { get; set; }

        [Required]
        //[DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^((\+?86)|(\(\+86\)))?(13[012356789][0-9]{8}|15[012356789][0-9]{8}|18[02356789][0-9]{8}|147[0-9]{8}|1349[0-9]{7})$", ErrorMessage = "电话的格式不正确")]
        [Display(Name = "联系方式")]
        public string DeliverMobile { get; set; }
        public string DeliverLocation { get; set; }
        public int? UserId { get; set; }

    }
}