using ewallet.shared.Models;
using ewallet.shared.Models.MobileNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.MobileNotification
{
    public interface IMobileNotificationRepository
    {
        List<MobileNotificationCommon> GetNotificationList();
        MobileNotificationCommon GetNotificationById(string notificationid, string username);
        CommonDbResponse ManageNotification(MobileNotificationCommon mnc);
        CommonDbResponse Disable_EnableNotification(string notificationid, string status, string username);
    }
}
