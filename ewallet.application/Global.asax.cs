using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ewallet.application.Library;
using ewallet.business.ErrorHandler;

namespace ewallet.application
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Bootstrapper.Initialise();
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            ErrorHandlerBusiness buss = new ErrorHandlerBusiness();
            Exception exception = HttpContext.Current.Error;

            var err = Server.GetLastError();
            if (err != null)
            {
                if (exception.GetType() == typeof(HttpException)) //It's an Http Exception
                {
                    var statusCode = ((HttpException)exception).GetHttpCode();
                    var page = HttpContext.Current.Request.Url.ToString();
                    string userName = "System";
                    if (Session["UserName"] != null)
                    {
                        userName= HttpContext.Current.Session["UserName"].ToString();
                        //userName = string.IsNullOrEmpty(Session["UserName"].ToString()) ? "" : Session["UserName"].ToString();

                    }
                    string ipAddress = ApplicationUtilities.GetIP();
                    var id = buss.LogError(err, page, userName, ipAddress);
                    switch (statusCode)
                    {
                        //case 400:
                        //    routeData.Values.Add("status", "400 - Bad Request");
                        //    break;
                        //case 401:
                        //    routeData.Values.Add("status", "401 - Access Denied");
                        //    break;
                        case 403:
                            break;
                        case 404:
                            break;
                        default:
                            //routeData.Values.Add("status", "500 - Internal Server Error");
                            HttpContext.Current.Response.Redirect("/Error/Index?Id=" + id);
                            break;
                    }
                }

            }
        }
    }
}
