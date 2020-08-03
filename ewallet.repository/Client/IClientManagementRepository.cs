using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.shared.Models;
using ewallet.shared.Models.WalletUser;

namespace ewallet.repository.Client
{
    public interface IClientManagementRepository
    {
        List<WalletUserInfo> WalletUserList(string agentType = "", string agentId = "", string ParentId = "");
        CommonDbResponse UserStatusChange(string userId, string agentId, string status = "");
        CommonDbResponse AddUser(WalletUserInfo walletUser);
        CommonDbResponse InsertBalance(WalletUserInfo walletUser);
    }
}
