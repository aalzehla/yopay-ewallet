using ewallet.shared.Models;
using ewallet.shared.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.Log
{
    public interface IActivityLogRepository
    {
        CommonDbResponse InsertActivityLog(ActivityLog al);
        List<ActivityLog> ActivityLog(string ActionUser);
    }
}
