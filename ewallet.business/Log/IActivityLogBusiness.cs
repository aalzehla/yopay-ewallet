using ewallet.shared.Models;
using ewallet.shared.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.Log
{
    public interface IActivityLogBusiness
    {
        CommonDbResponse InsertActivityLog(ActivityLog al);
        List<ActivityLog> ActivityLog(string ActionUser);
    }
}
