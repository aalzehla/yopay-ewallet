﻿using System.Web;
using System.Web.Mvc;
using ewallet.application.Filters;

namespace ewallet.application
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new SessionExpiryFilterAttribute());
            //filters.Add(new ClientKycStatusRedirectFilter());
            filters.Add(new ActivityLogFilter());
        }
    }
}
