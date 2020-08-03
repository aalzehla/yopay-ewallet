using ewallet.shared.Models;
using ewallet.shared.Models.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.Error
{
   public interface IErrorLogRepository
    {
        CommonDbResponse InsertErrors(ErrorsLog el);
    }
}
