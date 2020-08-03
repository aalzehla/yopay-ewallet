using ewallet.shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.shared.Models.WalletUser;

namespace ewallet.business.Client
{
    public interface IWalletUserBusiness
    {
        List<ClientCommon> ServiceDetail(string userid = "");
        WalletUserInfo UserInfo(string UserId = "");
        Dictionary<string, string> GetProposeList();
        CommonDbResponse WalletBalanceRT(WalletBalanceCommon walletBalance);
        CommonDbResponse AgentToWallet(WalletBalanceCommon walletBalance);
        CommonDbResponse CheckMobileNumber(string agentid, string mobileno, string usertype, string mode);
    }
}
