using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.repository.Common;

namespace ewallet.business.Common
{
    public class CommonBusiness:ICommonBusiness
    {
        ICommonRepository _repo;
        public CommonBusiness()
        {
            _repo = new CommonRepository();
        }
        public  Dictionary<string, string> sproc_get_dropdown_list(string flag, string extra1 = "")
        {
            return _repo.sproc_get_dropdown_list(flag, extra1);
        }
    }
}
