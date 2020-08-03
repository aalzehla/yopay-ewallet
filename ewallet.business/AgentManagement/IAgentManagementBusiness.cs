using ewallet.shared.Models;
using ewallet.shared.Models.AgentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.AgentManagement
{
    public interface IAgentManagementBusiness
    {
        List<AgentManagementCommon> GetAgentList(string AgentId, string username, string parentid = "");
        CommonDbResponse ManageAgent(AgentManagementCommon AC);
        AgentManagementCommon GetAgentById(string AgentId, string username);
        //List<AgentManagementCommon> GetWalletUserList(string DistId, string UserId = "");
        CommonDbResponse ExtendCreditLimit(AgentCreditLimitCommon acc);
        CommonDbResponse Disable_EnableAgent(AgentManagementCommon amc);
    }
}
