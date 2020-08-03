using ewallet.repository.TransactionLimit;
using ewallet.shared.Models;
using ewallet.shared.Models.TransactionLimit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.TransactionLimit
{
   
    public interface ITransactionLimitBusiness
    {
       List<TransactionLimitCommon> GetTransactionLimitList();
        TransactionLimitCommon GetTransactionLimitById(string Id);
        CommonDbResponse ManageTransactionlimit(TransactionLimitCommon CC);
        TransactionLimitCommon GetTransactionLimitForUser(string AgentId);

    }
}
