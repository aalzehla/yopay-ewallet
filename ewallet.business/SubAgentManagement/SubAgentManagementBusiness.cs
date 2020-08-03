﻿using ewallet.business.SubDistributor;
using ewallet.repository.SubAgentManagement;
using ewallet.shared.Models;
using ewallet.shared.Models.SubAgentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.SubAgentManagement
{
    public class SubAgentManagementBusiness : ISubAgentManagementBusiness
    {
        ISubAgentManagementRepository repo;
        public SubAgentManagementBusiness(SubAgentManagementRepository _repo)
        {
            repo = _repo;
        }

        public SubAgentManagementCommon GetSubAgentById(string AgentId, string username)
        {
            return repo.GetSubAgentById(AgentId, username);
        }

        public List<SubAgentManagementCommon> GetSubAgentList( string AgentId, string username, string SubAgentId = "")
        {
            return repo.GetSubAgentList(AgentId, username, SubAgentId);
        }
        public CommonDbResponse ManageSubAgent(SubAgentManagementCommon AC)
        {
            return repo.ManageSubAgent( AC);
        }
        public CommonDbResponse ExtendCreditLimit(SubAgentCreditLimitCommon acc)
        {
            return repo.ExtendCreditLimit( acc);
        }
        public CommonDbResponse Disable_EnableSubAgent(SubAgentManagementCommon amc)
        {
            return repo.Disable_EnableSubAgent(amc);
        }
    }
}
