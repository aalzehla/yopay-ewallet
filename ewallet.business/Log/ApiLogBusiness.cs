using ewallet.repository.Log;
using ewallet.shared.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.Log
{
    public class ApiLogBusiness : IApiLogBusiness
    {
        IApiLogRepository _api;
        public ApiLogBusiness()
        {
            _api = new ApiLogRepository();
        }
        public List<ApiLogCommon> GetApiLogList(string api_log_id = "", string fromDate = "", string toDate = "")
        {
            return _api.GetApiLogList(api_log_id, fromDate, toDate);
        }
    }
}
