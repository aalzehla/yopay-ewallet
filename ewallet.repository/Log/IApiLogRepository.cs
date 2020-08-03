using ewallet.shared.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.Log
{
    public interface IApiLogRepository
    {
        List<ApiLogCommon> GetApiLogList(string api_log_id = "", string fromDate = "", string toDate = "");
    }
}
