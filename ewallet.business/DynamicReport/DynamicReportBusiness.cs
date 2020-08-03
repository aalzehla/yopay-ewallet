using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.repository.DynamicReport;
using ewallet.shared.Models.DynamicReport;

namespace ewallet.business.DynamicReport
{
    public class DynamicReportBusiness:IDynamicReportBusiness
    {
        IDynamicReportRepository _repo;
        public DynamicReportBusiness()
        {
            _repo = new DynamicReportRepository();
        }
        public List<DynamicReportCommon> GetTransactionReport(string AgentId = "")
        {
            return _repo.GetTransactionReport(AgentId);
        }
        public DynamicReportCommon GetTransactionReportDetail(string TxnId, string AgentId = "")
        {
            return _repo.GetTransactionReportDetail(TxnId,AgentId);
        }
        public List<DynamicReportCommon> GetPendingReport()
        {
            return _repo.GetPendingReport();
        }
        public List<DynamicReportCommon> GetSettlementReport(string userid)
        {
            return _repo.GetSettlementReport(userid);
        }
        public List<DynamicReportCommon> GetSettlementReportclient(DynamicReportFilter DRF)
        {
            return _repo.GetSettlementReportclient(DRF);
        }
        public List<DynamicReportCommon> GetManualCommissionReport(string Userid)
        {
            return _repo.GetManualCommissionReport(Userid);
        }
        public DynamicReportCommon GetActivityDetail(string txnid, string flag)
        {
            return _repo.GetActivityDetail(txnid, flag);
        }
    }
}
