using ewallet.shared.Models;
using ewallet.shared.Models.Bank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.Bank
{
    public interface IBankRepository
    {
        List<BankCommon> GetBankList();
        CommonDbResponse AddBank(BankCommon bank);
        CommonDbResponse UpdateBank(BankCommon bank);
    }
}
