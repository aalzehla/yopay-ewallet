using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.repository.ErrorHandler;

namespace ewallet.business.ErrorHandler
{
    public class ErrorHandlerBusiness : IErrorHandlerBusiness
    {
        IErrorHandlerRepository repo;
        public ErrorHandlerBusiness()
        {
            repo = new ErrorHandlerRepository();
        }
        public string LogError(Exception ex, string Page, string UserName, string IpAddress)
        {
            return repo.LogError(ex, Page, UserName, IpAddress);
        }
    }
}
