using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Entity
{
    public class ResultData
    {
        /// <summary>
        /// [状态]1成功，0失败
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 数据
        /// </summary>   
        public object Data { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; }
    }
}
