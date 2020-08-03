using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ewallet.shared.Models.KYC;

namespace ewallet.business.KYC
{
    public interface IKycBusiness
    {
        List<KYCCommon> GetAgentList();
        KYCCommon AgentKycInfo(string AgentId);
        Dictionary<string, string> Dropdown(string flag, string search1 = "");
        shared.Models.CommonDbResponse UpadateKycDetails(KYCCommon kycCommon, string status);
    }
}
