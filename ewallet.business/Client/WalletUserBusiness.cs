using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.repository.Client;
using ewallet.shared.Models;
using ewallet.shared.Models.WalletUser;

namespace ewallet.business.Client
{
    public class WalletUserBusiness : IWalletUserBusiness
    {
        IWalletUserRepository _repo;
        public WalletUserBusiness()
        {
            _repo = new WalletUserRepository();
        }
        public List<ClientCommon> ServiceDetail(string userid = "")
        {
            return _repo.ServiceDetail(userid);
        }
        public WalletUserInfo UserInfo(string UserId = "")
        {
            return _repo.UserInfo(UserId);
        }
        public Dictionary<string, string> GetProposeList()
        {
            return _repo.GetProposeList();
        }

        public CommonDbResponse WalletBalanceRT(WalletBalanceCommon walletBalance)
        {
            return _repo.WalletBalanceRT(walletBalance);
        }

        public CommonDbResponse AgentToWallet(WalletBalanceCommon walletBalance)
        {
            return _repo.AgentToWallet(walletBalance);
        }

        public CommonDbResponse CheckMobileNumber(string agentid, string mobileno, string usertype, string mode)
        {
            return _repo.CheckMobileNumber(agentid, mobileno, usertype, mode);
        }
    }
}
