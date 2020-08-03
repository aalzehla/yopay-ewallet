﻿using ewallet.shared.Models;
using ewallet.shared.Models.SubDistributor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.NewAgent
{
    public interface IAgentRepository
    {
        List<AgentNewCommon> GetAgentList(string AgentId, string username, string parentid = "");
        CommonDbResponse ManageAgent(AgentNewCommon AC);
        AgentNewCommon GetAgentById(string AgentId, string username);
        List<AgentNewCommon> GetWalletUserList(string DistId, string UserId = "");
    }
}
