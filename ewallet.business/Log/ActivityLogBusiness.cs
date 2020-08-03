
using ewallet.repository.Log;
using ewallet.shared.Models;
using ewallet.shared.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.Log
{
  public  class ActivityLogBusiness : IActivityLogBusiness
    {
        IActivityLogRepository _act;
        public ActivityLogBusiness()
        {
            _act = new ActivityLogRepository() ;
        }
        public CommonDbResponse InsertActivityLog(ActivityLog al)
        {
            return _act.InsertActivityLog(al);
        }
        public List<ActivityLog> ActivityLog(string ActionUser)
        {
            return _act.ActivityLog(ActionUser);
        }
    }
}
