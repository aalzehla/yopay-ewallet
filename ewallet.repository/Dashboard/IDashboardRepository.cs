using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ewallet.repository.Dashboard
{
    public interface IDashboardRepository
    {
        DataSet CountTransactionByProduct(string User);
    }
}
