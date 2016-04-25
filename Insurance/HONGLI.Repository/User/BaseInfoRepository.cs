using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using HONGLI.Entity;
using Newtonsoft.Json;

using I.Utility.Helper;
using I.Utility.Extensions;

namespace HONGLI.Repository.User
{
    public class BaseInfoRepository 
    {
        public int AddUserInfo(BaseInfo_UserInfo userInfo)
        {
            var returnVale = 0;
            if (userInfo == null)
                return returnVale;

            try
            {
                using (var context = new E2JOINDB())
                {
                    context.BaseInfo_UserInfo.Add(userInfo);

                    context.SaveChanges();

                    returnVale = userInfo.ID;
                }

                this.AddLog("增加用户", JsonConvert.SerializeObject(userInfo), 10, returnVale);
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
            }

            return returnVale;
        }


        public BaseInfo_UserInfo GetUserInfoByMobile(string mobile, string ip)
        {
            var returnValue = new BaseInfo_UserInfo ();
            if (mobile.IsNull())
                return returnValue;

            try
            {
                using (var context = new E2JOINDB())
                {
                    returnValue = (from x in context.BaseInfo_UserInfo
                                   where
                                     x.Mobile == mobile
                                   select x).FirstOrDefault();
                }

                this.AddLog("通过手机检查用户是否存在", "ip :" + ip + " ,通过手机"+ mobile + "检查用户是否存在", 11, 0);
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
            }

            return returnValue;
        }


        public void AddLog(string info, string memo, byte type, int createMemberId)
        {
            try
            {
                using (var context = new E2JOINDB())
                {
                    var log = new SYS_LogInfo
                    {
                        ActionMemo = memo,
                        ActionType = type,
                        CreateMemberID = createMemberId,
                        ActionMsg = info
                    };

                    context.SYS_LogInfo.Add(log);

                    context.SaveChanges();
                }
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
            }
        }

        public bool Login( string mobile , string sendSMS ,string inputSMS )
        {
            var returnValue = false;
            if (mobile.IsNull () || sendSMS.IsNull () || inputSMS.IsNull())
                return returnValue;

            returnValue = sendSMS.Trim() == inputSMS.Trim();

            try
            {
                this.AddLog(
                    "用户登录日志",
                    returnValue ? string.Format("用户{0}登录成功，发送的验证码{1},回发的验证码{2}，时间{3}", mobile, sendSMS, inputSMS, DateTime.Now) :
                                    string.Format("用户{0}登录失败，发送的验证码{1},回发的验证码{2}，时间{3}", mobile, sendSMS, inputSMS, DateTime.Now), 15, 0);

                LogHelper.Info("手机号：" + mobile + "发送的验证码：" + sendSMS + "输入验证码：" + inputSMS + "时间:" + DateTime.Now );
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
            }

            return returnValue;
        }
    }
}
