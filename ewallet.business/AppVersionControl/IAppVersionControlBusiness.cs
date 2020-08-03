using ewallet.shared.Models;
using ewallet.shared.Models.AppVersionControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.AppVersionControl
{
    public interface IAppVersionControlBusiness
    {
        List<AppVersionControlCommon> GetAppVersionList();
        CommonDbResponse ManageAppVersion(AppVersionControlCommon AVC);
    }
}
