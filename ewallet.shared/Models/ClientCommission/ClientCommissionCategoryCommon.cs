﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.shared.Models.ClientCommission
{
    public class ClientCommissionCategoryCommon:Common
    {
        public string AgentId { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string IsActive { get; set; }

        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}
