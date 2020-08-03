using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.shared.Models.DynamicReport;

namespace ewallet.repository.DynamicReport
{
    public interface IDynamicReportRepository
    {
        List<DynamicReportCommon> GetTransactionReport(string AgentId = "");
        List<DynamicReportCommon> GetPendingReport();
        DynamicReportCommon GetTransactionReportDetail(string TxnId, string AgentId = "");
        List<DynamicReportCommon> GetSettlementReport(string userid);
        List<DynamicReportCommon> GetSettlementReportclient(DynamicReportFilter DRW);
        List<DynamicReportCommon> GetManualCommissionReport(string Userid);
        DynamicReportCommon GetActivityDetail(string txnid, string flag);
    }
}
