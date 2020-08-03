using ewallet.repository.Log;
using ewallet.shared.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.Log
{
    public class AccessLogBusiness : IAccessLogBusiness
    {
        IAccessLogRepository _acc;
        public AccessLogBusiness()
        {
            _acc = new AccessLogRepository();
        }
        public List<AccessLogCommon> GetAccessLogList(string from, string to)
        {
            return _acc.GetAccessLogList(from,to);
        }
    }
}
