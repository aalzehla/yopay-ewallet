using ewallet.shared.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.Log
{
   public interface IApiLogBusiness
    {
        List<ApiLogCommon> GetApiLogList(string api_log_id = "", string fromDate = "", string toDate = "");
    }
}
