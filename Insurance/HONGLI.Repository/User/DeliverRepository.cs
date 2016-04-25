using HONGLI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Repository.User
{
    public class DeliverRepository
    {
        /// <summary>
        /// 获取配送信息。
        /// </summary>
        /// <param name="memberId">成员信息编号。</param>
        /// <returns></returns>
        public List<User_Deliver> GetDeliverByUser(int userId)
        {
            List<User_Deliver> deliverList = new List<User_Deliver>();
            try
            {
                using (var context = new E2JOINDB())
                {

                    var query = context.User_Deliver
                        .Where(c => c.UserId == userId)
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

        /// <summary>
        /// 获取配送信息。
        /// </summary>
        /// <param name="memberId">成员信息编号。</param>
        /// <returns></returns>
        public User_Deliver GetDeliverById(int id)
        {
            User_Deliver deliver = new User_Deliver();
            try
            {
                using (var context = new E2JOINDB())
                {

                    var query = context.User_Deliver
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


        /// <summary>
        /// 增加配送信息。
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int AddDeliver(User_Deliver deliver)
        {
            int orderid = 0;
            try
            {
                using (var context = new E2JOINDB())
                {
                    context.User_Deliver.Add(deliver);
                    orderid = context.SaveChanges();
                    return deliver.Id;
                }
            }
            catch (Exception)
            {
                //LogHelper.AppError(error.Message);
            }
            return orderid;
        }

        /// <summary>
        /// 更新配送信息。
        /// </summary>
        /// <param name="deliver"></param>
        /// <returns></returns>
        public int UpdateDeliver(User_Deliver deliver)
        {
            int orderid = 0;
            try
            {
                using (var context = new E2JOINDB())
                {
                    var query = context.User_Deliver
                       .Where(c => c.Id == deliver.Id)
                       .FirstOrDefault();
                    if (query != null)
                    {
                        query.Id = deliver.Id;
                        query.DeliverAddress = deliver.DeliverAddress;
                        query.DeliverLocation = deliver.DeliverLocation;
                        query.DeliverMobile = deliver.DeliverMobile;
                        query.DeliverName = deliver.DeliverName;
                        query.DeliverPrice = deliver.DeliverPrice;
                        query.DeliverTime = deliver.DeliverTime;
                        query.DeliverType = deliver.DeliverType;
                        query.CreateDate = deliver.CreateDate;
                    }
                    if (context.SaveChanges() > 0)
                    {
                        orderid = deliver.Id;
                    }
                }
            }
            catch (Exception)
            {
                //LogHelper.AppError(error.Message);
            }
            return orderid;
        }
        public int UpdateDeliverType(User_Deliver deliver)
        {
            int orderid = 0;
            try
            {
                using (var context = new E2JOINDB())
                {
                    var query = context.User_Deliver
                       .Where(c => c.Id == deliver.Id)
                       .FirstOrDefault();
                    if (query != null)
                    {
                        query.Id = deliver.Id;
                        //query.DeliverAddress = deliver.DeliverAddress;
                        //query.DeliverLocation = deliver.DeliverLocation;
                        //query.DeliverMobile = deliver.DeliverMobile;
                        //query.DeliverName = deliver.DeliverName;
                        query.PickupAddress = deliver.PickupAddress;
                        query.PickupTime = deliver.PickupTime;
                        query.DeliverType = deliver.DeliverType;
                        //query.CreateDate = deliver.CreateDate;
                    }
                    orderid = context.SaveChanges();
                }
            }
            catch (Exception)
            {
                //LogHelper.AppError(error.Message);
            }
            return orderid;
        }
    }
}
