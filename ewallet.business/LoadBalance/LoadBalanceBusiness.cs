using ewallet.repository.LoadBalance;
using ewallet.shared.Models;
using ewallet.shared.Models.LoadBalance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.LoadBalance
{
    public class LoadBalanceBusiness : ILoadBalanceBusiness
    {
        ILoadBalanceRepositroy _repo;

        public LoadBalanceBusiness()
        {
            _repo = new LoadBalanceRepository();
        }

        public CommonDbResponse CheckTrnasactionExistence(string MerchantTxnId, string GatewayTxnId)
        {
            return _repo.CheckTrnasactionExistence(MerchantTxnId, GatewayTxnId);
        }

        public CommonDbResponse GetTransactionReposne(string MerchantTxnId, string GatewayTxnId)
        {
            return _repo.GetTransactionReposne(MerchantTxnId, GatewayTxnId);
        }

        public CommonDbResponse LoadBalance(LoadBalanceCommon balance)
        {
            return _repo.LoadBalance(balance);
        }

        public CommonDbResponse UpdateTransaction(LoadBalanceCommon balance)
        {
           return _repo.UpdateTransaction(balance);
        }
    }
}
