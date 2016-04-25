using HONGLI.Entity;
using HONGLI.Repository;
using HONGLI.Repository.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Service.User
{
    public class DeliverService
    {
        private static DeliverRepository _deliverRepository = null;

        public DeliverService()
        {
            _deliverRepository = new DeliverRepository();
        }

        public int AddDeliver(User_Deliver deliver)
        {
            return _deliverRepository.AddDeliver(deliver);
        }

        public int UpdateDeliver(User_Deliver deliver)
        {
            return _deliverRepository.UpdateDeliver(deliver);
        }

        public int UpdateDeliverType(User_Deliver deliver)
        {
            return _deliverRepository.UpdateDeliverType(deliver);
        }

        public  List<User_Deliver> GetDeliverByUser(int userId)
        {
            return _deliverRepository.GetDeliverByUser(userId);
        }

        public User_Deliver GetDeliverById(int id)
        {
            return _deliverRepository.GetDeliverById(id);
        }

    }
}
