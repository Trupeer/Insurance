using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HONGLI.Entity;
using HONGLI.Repository;

using I.Utility.Extensions;
using I.Utility.Helper;

namespace HONGLI.Service.User
{
    public class BaseInfoService
    {
        public int AddUserInfo(BaseInfo_UserInfo userInfo)
        {
            var returnVale = 0;
            if (userInfo == null)
                return returnVale;

            try
            {
                var repository = new Repository.User.BaseInfoRepository();

                returnVale = repository.AddUserInfo(userInfo);
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
            }

            return returnVale;
        }


        public BaseInfo_UserInfo GetUserInfoByMobile(string mobile, string ip = "")
        {
            var returnValue = new BaseInfo_UserInfo();
            if (mobile.IsNull())
                return returnValue;

            try
            {
                var repository = new Repository.User.BaseInfoRepository();

                returnValue = repository.GetUserInfoByMobile(mobile, ip);
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
            }

            return returnValue;
        }

        public bool Login(string mobile, string sendSMS, string inputSMS)
        {
            var returnValue = false;
            if (mobile.IsNull() || sendSMS.IsNull() || inputSMS.IsNull())
                return returnValue;

            returnValue = sendSMS.Trim() == inputSMS.Trim();

            var repository = new Repository.User.BaseInfoRepository();

            returnValue = repository.Login(mobile, sendSMS, inputSMS);

            return returnValue;
        }
    }
}
