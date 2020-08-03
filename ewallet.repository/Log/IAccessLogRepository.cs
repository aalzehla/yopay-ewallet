using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.shared.Models.Log;

namespace ewallet.repository.Log
{
    public interface IAccessLogRepository
    {
        List<AccessLogCommon> GetAccessLogList(string from , string to);      

    }
}
