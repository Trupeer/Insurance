using HONGLI.Entity;
using I.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Repository.User
{
    public class PolicyHolderRepository
    {
        /// <summary>
        /// 获取配送信息。
        /// </summary>
        /// <param name="userId">成员信息编号。</param>
        /// <returns></returns>
        public List<User_PolicyHolder> GetPolicyHolderByUser(int userId)
        {
            List<User_PolicyHolder> deliverList = new List<User_PolicyHolder>();
            try
            {
                using (var context = new E2JOINDB())
                {

                    var query = context.User_PolicyHolder
                        .Where(c => c.Id == userId)
                        .ToList();

                    deliverList = query;
                }
            }
            catch (Exception)
            {
                //LogHelper.AppError(error.Message);
            }
            return deliverList;
        }

        public User_PolicyHolder GetPolicyHolderById(int id)
        {
            User_PolicyHolder deliver = new User_PolicyHolder();
            try
            {
                using (var context = new E2JOINDB())
                {

                    var query = context.User_PolicyHolder
                        .Where(c => c.Id == id)
                        .FirstOrDefault();

                    deliver = query;
                }
            }
            catch (Exception)
            {
                //LogHelper.AppError(error.Message);
            }
            return deliver;
        }

        //public List<User_PolicyHolder> GetPolicyHolderByUser(int memberId)
        //{
        //    throw new NotImplementedException();
        //}

        public int UpdatePolicyHolder(User_PolicyHolder policyHolder)
        {
            int result = 0;
            try
            {
                using (var context = new E2JOINDB())
                {

                    var query = context.User_PolicyHolder
                        .Where(c => c.Id == policyHolder.Id)
                        .FirstOrDefault();
                    query = policyHolder;
                    result = context.SaveChanges();
                }
            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
            }
            return result;
        }



        /// <summary>
        /// 下单操作
        /// </summary>
        /// <param name="policyHolder"></param>
        /// <returns></returns>
        public int AddPolicyHolder(User_PolicyHolder policyHolder)
        {
            int result = 0;
            try
            {
                using (var context = new E2JOINDB())
                {
                    context.User_PolicyHolder.Add(policyHolder);
                    result = context.SaveChanges();
                }

            }
            catch (Exception error)
            {
                LogHelper.AppError(error.Message);
            }
            return result;
        }
    }
}
