using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ewallet.business.Dashboard
{
    public interface IDashboardBusiness
    {
        DataSet CountTransactionByProduct(String User);
    }
}
