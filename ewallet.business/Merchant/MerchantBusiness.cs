using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.repository.Merchant;
using ewallet.shared.Models;
using ewallet.shared.Models.Merchant;

namespace ewallet.business.Merchant
{
    public class MerchantBusiness : IMerchantBusiness
    {
        IMerchantManagementRepository _repo;
        public MerchantBusiness(MerchantManagementRepository repo)
        {
            _repo = repo;
        }
        public List<MerchantCommon> GetMerchantList(string MerchantId = "", string parentid = "")
        {
            return _repo.GetMerchantList(MerchantId, parentid);
        }

        public CommonDbResponse ManageMerchant(MerchantCommon merchantCommon)
        {
            return _repo.ManageMerchant(merchantCommon);
        }

        public MerchantCommon GetMerchantById(string MerchantId)
        {
            return _repo.GetMerchantById(MerchantId);
        }

        public CommonDbResponse Disable_EnableMerchant(MerchantCommon merchantCommon)
        {
            return _repo.Disable_EnableMerchant(merchantCommon);
        }
    }
}
