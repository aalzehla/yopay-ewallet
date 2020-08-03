using ewallet.shared.Models;
using ewallet.shared.Models.AppVersionControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.AppVersionControl
{
    public interface IAppVersionControlRepository
    {
        List<AppVersionControlCommon> GetAppVersionList();
        CommonDbResponse ManageAppVersion(AppVersionControlCommon AVC);
    }
}
