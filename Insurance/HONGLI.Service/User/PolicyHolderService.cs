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
    public class PolicyHolderService
    {
        private static PolicyHolderRepository _policyHolderRepository = null;

        public PolicyHolderService()
        {
            _policyHolderRepository = new PolicyHolderRepository();
        }

        public int AddPolicyHolder(User_PolicyHolder policyHolder)
        {
            return _policyHolderRepository.AddPolicyHolder(policyHolder);
        }

        public int UpdatePolicyHolder(User_PolicyHolder policyHolder)
        {
            return _policyHolderRepository.UpdatePolicyHolder(policyHolder);
        }

        public User_PolicyHolder GetPolicyHolderById(int id)
        {
            return _policyHolderRepository.GetPolicyHolderById(id);
        }

        public  List<User_PolicyHolder> GetPolicyHolderByUser(int memberId)
        {
            return _policyHolderRepository.GetPolicyHolderByUser(memberId);
        }
    }
}
