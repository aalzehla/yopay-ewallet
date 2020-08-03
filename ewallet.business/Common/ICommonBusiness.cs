using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.Common
{
    public interface ICommonBusiness
    {
        Dictionary<string, string> sproc_get_dropdown_list(string flag,string extra1="");
    }
}
