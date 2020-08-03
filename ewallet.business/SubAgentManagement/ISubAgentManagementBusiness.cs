﻿using ewallet.shared.Models;
using ewallet.shared.Models.SubAgentManagement;
using ewallet.shared.Models.SubDistributor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.SubAgentManagement
{
   public interface ISubAgentManagementBusiness
    {
        List<SubAgentManagementCommon> GetSubAgentList( string AgentId, string username, string SubAgentId = "");

        SubAgentManagementCommon GetSubAgentById(string AgentId, string username);
        CommonDbResponse ManageSubAgent(SubAgentManagementCommon AC);
        CommonDbResponse ExtendCreditLimit(SubAgentCreditLimitCommon acc);
        CommonDbResponse Disable_EnableSubAgent(SubAgentManagementCommon amc);
    }
}
