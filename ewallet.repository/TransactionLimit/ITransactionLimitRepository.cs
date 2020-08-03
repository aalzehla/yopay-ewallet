using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.shared.Models;
using ewallet.shared.Models.TransactionLimit;

namespace ewallet.repository.TransactionLimit
{
    public interface ITransactionLimitRepository
    {
        List<TransactionLimitCommon> GetTransactionLimitList();

        TransactionLimitCommon GetTransactionLimitById(string Id);

        CommonDbResponse ManageTransactionlimit(TransactionLimitCommon CC);
        TransactionLimitCommon GetTransactionLimitForUser(string AgentId);
    }
}
