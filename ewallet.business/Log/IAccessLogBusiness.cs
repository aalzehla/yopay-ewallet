using ewallet.shared.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.Log
{
    public interface IAccessLogBusiness
    {
        List<AccessLogCommon> GetAccessLogList(string from, string to);
    }
}
