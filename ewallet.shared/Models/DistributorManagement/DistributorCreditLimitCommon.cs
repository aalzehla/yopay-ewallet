﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.shared.Models.DistributorManagement
{
    public class DistributorCreditLimitCommon:Common
    {
        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentCreditLimit { get; set; }
        public string AgentCurrentCreditLimit { get; set; }
        public string Remarks { get; set; }
    }
}
