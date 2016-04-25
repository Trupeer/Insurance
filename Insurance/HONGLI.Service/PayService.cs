using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HONGLI.Entity;
using I.Utility;
using Com.Alipay;

namespace HONGLI.Service
{
    public class PayService
    {
        public OrderService _orderService = new OrderService();

        public string CreateRequest(Order_Base order)
        {
            ////////////////////////////////////////////请求参数////////////////////////////////////////////

            //支付类型
            string payment_type = "1";
            //必填，不能修改
            //服务器异步通知页面路径
            string notify_url = I.Utility.Util.GetConfigByKey("alipayNotifyUrl");
            //需http://格式的完整路径，不能加?id=123这类自定义参数

            //页面跳转同步通知页面路径
            string return_url = I.Utility.Util.GetConfigByKey("alipayReturnUrl");
            //需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/

            //商户订单号
            string out_trade_no = order.OrderCode;
            //商户网站订单系统中唯一订单号，必填

            //订单名称
            string orderName = "宏利保险";
            var orderItem = order.Order_Item.FirstOrDefault();
            if (orderItem != null && !string.IsNullOrEmpty(orderItem.ProductName))
            {
                orderName = orderItem.ProductName;
            }
            else
            {
                orderName = "宏利保险";
            }
            string subject = orderName;
            //必填

            //付款金额
            string total_fee = order.AmountPayable.ToString();
            var AlipayPayTestSwitch = I.Utility.Util.GetConfigByKey("AlipayPayTestSwitch");
            if (AlipayPayTestSwitch == null || AlipayPayTestSwitch == "0")
                total_fee = order.AmountPayable.ToString();
            else
                total_fee = "0.01";
            //必填

            //商品展示地址
            string show_url = I.Utility.Util.GetConfigByKey("alipayShowUrl");
            //必填，需以http://开头的完整路径，例如：http://www.商户网址.com/myorder.html

            //订单描述
            string body = "orderCode=" + order.OrderCode;
            //选填

            //超时时间
            string it_b_pay = "";
            //选填

            //钱包token
            string extern_token = "";
            //选填


            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("seller_id", Config.Seller_id);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("service", "alipay.wap.create.direct.pay.by.user");
            sParaTemp.Add("payment_type", payment_type);
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("return_url", return_url);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("show_url", show_url);
            sParaTemp.Add("body", body);
            sParaTemp.Add("it_b_pay", it_b_pay);
            sParaTemp.Add("extern_token", extern_token);

            //建立请求
            string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
            return sHtmlText;
        }

        /// <summary>
        /// 支付后更新
        /// </summary>
        /// <param name="ordercode"></param>
        /// <param name="payAccount"></param>
        /// <param name="payTime"></param>
        /// <returns></returns>
        public EnumOrderStatus Do(string ordercode, string tradeNo, string payAccount, DateTime payTime)
        {
            ResultData resultData = new ResultData();

            //支付成功，记录日志
            string payOkLog = string.Format("[支付成功]->ordercode={0}&tradeno={1}&payaccount={2}", ordercode, tradeNo, payAccount);
            I.Utility.Helper.LogHelper.Info(payOkLog);

            
            var updateResult = _orderService.UpdatePay(ordercode, tradeNo, payAccount.ToString(), payTime, (int)EnumOrderStatus.Paid);
            if (updateResult > 0)
            {
                bool result = false;
                try
                {
                    //(调用订单通知接口)                 
                    var model = new PostOrderStatusModel
                    {
                        OrderId = ordercode,
                        OrderStatus = 1,
                        AgentId = Convert.ToInt32(I.Utility.Util.GetConfigByKey("ApiAgent"))
                    };
                    result = PostOrderService.PostOrderStatus(model);
                }
                catch (Exception ex)
                {                    
                }
                             
                //更新订单状态为已通知
                if (result)
                {
                    string policyOkLog = string.Format("[通知成功]->ordercode={0}&tradeno={1}&payaccount={2}", ordercode, tradeNo, payAccount);
                    I.Utility.Helper.LogHelper.Info(policyOkLog);
                    _orderService.UpdateOrderStatus(ordercode, (int)EnumOrderStatus.Success);
                    return EnumOrderStatus.Success;
                }
                else
                {
                    I.Utility.Helper.LogHelper.Info(string.Format("[通知失败]->ordercode={0}&tradeno={1}&payaccount={2}", ordercode, tradeNo, payAccount));
                    return EnumOrderStatus.Paid;
                }
            }
            else
            {
                I.Utility.Helper.LogHelper.Info(string.Format("[支付成功-更新数据库-已支付-失败]->ordercode={0}&tradeno={1}&payaccount={2}", ordercode, tradeNo, payAccount));
                return EnumOrderStatus.Failed;
            }
        }

    }
}
