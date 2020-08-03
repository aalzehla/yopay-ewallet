using ewallet.shared.Models.DynamicReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.DynamicReport
{
    public interface IDynamicReportBusiness
    {
        List<DynamicReportCommon> GetTransactionReport(string AgentId = "");
        DynamicReportCommon GetTransactionReportDetail(string TxnId, string AgentId = "");
        List<DynamicReportCommon> GetPendingReport();
        List<DynamicReportCommon> GetSettlementReport(string Userid);
        List<DynamicReportCommon> GetSettlementReportclient(DynamicReportFilter DRW);
        List<DynamicReportCommon> GetManualCommissionReport(string Userid);
        DynamicReportCommon GetActivityDetail(string txnid, string flag);
    }
}
