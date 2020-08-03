using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.Common
{
   public interface ICommonRepository
    {
        Dictionary<string, string> sproc_get_dropdown_list(string flag,string extra1="");
    }
}
