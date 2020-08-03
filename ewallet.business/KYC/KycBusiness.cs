﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ewallet.repository.KYC;
using ewallet.repository.Login;
using ewallet.shared.Models.KYC;

namespace ewallet.business.KYC
{
    public class KycBusiness : IKycBusiness
    {
        IKYCRepository _repo;
        public KycBusiness()
        {
            _repo = new KYCRepository();
        }
        public List<KYCCommon> GetAgentList()
        {
            return _repo.GetAgentList();
        }

        public KYCCommon AgentKycInfo(string AgentId)
        {
            return _repo.AgentKycInfo(AgentId);
        }

        public Dictionary<string, string> Dropdown(string flag, string search1 = "")
        {
            return _repo.Dropdown(flag, search1);
        }

        public shared.Models.CommonDbResponse UpadateKycDetails(KYCCommon kycCommon, string status)
        {
            return _repo.UpadateKycDetails(kycCommon, status);
        }
    }
}
