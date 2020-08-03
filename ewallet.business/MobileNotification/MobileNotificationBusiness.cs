using ewallet.repository.MobileNotification;
using ewallet.shared.Models;
using ewallet.shared.Models.MobileNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.MobileNotification
{
    public class MobileNotificationBusiness:IMobileNotificationBusiness
    {
        IMobileNotificationRepository repo;
        public MobileNotificationBusiness()
        {
            repo = new MobileNotificationRepository();
        }
        public List<MobileNotificationCommon> GetNotificationList()
        {
            return repo.GetNotificationList();
        }
        public MobileNotificationCommon GetNotificationById(string notificationid, string username)
        {
            return repo.GetNotificationById(notificationid, username);

        }
        public CommonDbResponse ManageNotification(MobileNotificationCommon mnc)
        {
            return repo.ManageNotification(mnc);
        }
        public CommonDbResponse Disable_EnableNotification(string notificationid, string status, string username)
        {
            return repo.Disable_EnableNotification( notificationid,  status,  username);
        }
    }
}
