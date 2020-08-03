
using ewallet.repository.AgentManagement;
using ewallet.shared.Models;
using ewallet.shared.Models.AgentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.AgentManagement
{
    public class AgentManagementBusiness:IAgentManagementBusiness
    {
        IAgentManagementRepository repo;
        public AgentManagementBusiness(AgentManagementRepository _repo)
        {
            repo = _repo;
        }
        public List<AgentManagementCommon> GetAgentList(string AgentId, string username, string parentid = "")
        {
            return repo.GetAgentList(AgentId, username, parentid);
        }
        public CommonDbResponse ManageAgent(AgentManagementCommon AC)
        {
            return repo.ManageAgent(AC);
        }
        public AgentManagementCommon GetAgentById(string AgentId, string username)
        {
            return repo.GetAgentById(AgentId, username);
        }
        //public List<AgentManagementCommon> GetWalletUserList(string DistId, string UserId = "")
        //{
        //    return repo.GetWalletUserList(DistId, UserId);
        //}
        public CommonDbResponse ExtendCreditLimit(AgentCreditLimitCommon acc)
        {
            return repo.ExtendCreditLimit(acc);
        }
        public CommonDbResponse Disable_EnableAgent(AgentManagementCommon amc)
        {
            return repo.Disable_EnableAgent(amc);
        }
    }
}
