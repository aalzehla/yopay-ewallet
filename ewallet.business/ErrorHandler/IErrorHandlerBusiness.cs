﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.ErrorHandler
{
    public interface IErrorHandlerBusiness
    {
        string LogError(Exception ex, string Page, string UserName, string IpAddress);
    }
}
