﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ewallet.application.Library;
using ewallet.application.Models;
using ewallet.business.Client;
using ewallet.business.Log;
using ewallet.shared.Models.DynamicReport;
using ewallet.shared.Models.Log;

namespace ewallet.application.Areas.Admin.Controllers
{
    public class LogController : Controller
    {
        IActivityLogBusiness _log;
        IAccessLogBusiness _access;
        IApiLogBusiness _api;

        public LogController(IActivityLogBusiness log, IAccessLogBusiness access, IApiLogBusiness api)
        {
            _log = log;
            _access = access;
            _api = api;

        }

        #region Activity Log
        [HttpGet]
        public ActionResult ActivityLog()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActivityLog(string username)
        {
            //Start Search UserId from Mobile Number
            //IWalletUserBusiness _walletUserBusiness = new WalletUserBusiness();
            //string UserName = _walletUserBusiness.UserInfo(username).UserId.ToString();
            //Ends
            List<ActivityLogModel> log = _log.ActivityLog(username).MapObjects<ActivityLogModel>();
            //List<LogModel> logModels = log.MapObjects<LogModel>();

            //foreach (var item in log)
            //{
            //    item.Action = StaticData.GetActions("PendingTransaction", item.AgentId.EncryptParameter(), this, "", "", item.TxnId);
            //}
            //Column Creator
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("page_name", "Page Name");
            param.Add("page_url", "Page Url");
            param.Add("ipaddress", "IP Address");
            param.Add("browser_detail", "Browser Detail");
            param.Add("CreatedBy", "Created By");
            param.Add("CreatedLocalDate", "Date");
            ProjectGrid.column = param;
            //Ends
            var grid = ProjectGrid.MakeGrid(log, "hidebreadcrumb", "", 10, false, "", "", "", "", "", "");
            ViewData["grid"] = grid;

            return View();
        }

        #endregion

        #region Access Log
        [HttpGet]
        public ActionResult AccessLog(string from, string to)
        {
            AccessLogCommon alc = new AccessLogCommon();
            alc.fromDate = from;
            alc.toDate = to;
            //var UserType = Session["UserType"].ToString();
            var list = _access.GetAccessLogList(alc.fromDate, alc.toDate);
            foreach (var item in list)
            {
                item.Action = StaticData.GetActions("AccessLog", "", this, "", "");

            }
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("pageName", "Page Name");
            param.Add("logType", "Log Status");
            param.Add("browser", "Browser");
            param.Add("msg", "Remarks");
            param.Add("createdBy", "Access By");
            param.Add("actionIpAddress", "Ip Address");
            param.Add("createdLocalDate", "Date Time");

            ProjectGrid.column = param;
            var grid = ProjectGrid.MakeGrid(list, "", "", 0, true, "", "", "Home", "Access Log", "#", "#");
            ViewData["grid"] = grid;
            return View();
        }

        public ActionResult AccessLogByDate(string fromDate, string toDate)
        {
            AccessLogCommon alc = new AccessLogCommon();
            alc.fromDate = fromDate;
            alc.toDate = toDate;
            //var UserType = Session["UserType"].ToString();
            var list = _access.GetAccessLogList(alc.fromDate, alc.toDate);
            foreach (var item in list)
            {
                item.Action = StaticData.GetActions("AccessLog", "", this, "", "");

            }
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("pageName", "Login User");
            param.Add("logType", "Log Type");
            param.Add("browser", "Browser");
            param.Add("msg", "Remarks");
            param.Add("createdBy", "Created By");
            param.Add("createdUtcDate", "Created UTC Date");
            param.Add("createdLocalDate", "Created Local Date");

            ProjectGrid.column = param;
            var grid = ProjectGrid.MakeGrid(list, "", "", 0, true, "", "", "Home", "Access Log", "#", "#");
            ViewData["grid"] = grid;
            return View();
        }
        #endregion

        #region API Log
        [HttpGet]
        public ActionResult ApiLog(string ApiRequestLog, string fromDate, string toDate)
        {
            var list = _api.GetApiLogList(ApiRequestLog, fromDate, toDate);

            foreach (var item in list)
            {
                item.Action = StaticData.GetActions("APILog", item.apiLogId, this, "", "", "");
            }
            IDictionary<string, string> param = new Dictionary<string, string>();
            // param.Add("api_log_id", "Transaction Id");
            param.Add("userId", "Request User");
            param.Add("functionName", "Function Name");
            param.Add("createdLocalDate", "Created Date");
            param.Add("transacionId", "Transaction Id");
            param.Add("Action", "Action");

            ProjectGrid.column = param;
            var grid = ProjectGrid.MakeGrid(list, "", "", 0, true, "", "", "Home", "Api Log", "#", "#");
            ViewData["grid"] = grid;

            return View();
        }
        #endregion

       
        public ActionResult ApiLogByDate(string ApiRequestLog, string fromDate, string toDate)
        {
            var list = _api.GetApiLogList(ApiRequestLog, fromDate, toDate);
            foreach (var item in list)
            {
                item.Action = StaticData.GetActions("APILog", item.apiLogId, this, "", "", "");
            }
            IDictionary<string, string> param = new Dictionary<string, string>();
            // param.Add("api_log_id", "Transaction Id");
            param.Add("userId", "Request User");
            param.Add("functionName", "Function Name");
            param.Add("createdLocalDate", "Date");
            param.Add("transacionId", "Transaction Id");
            param.Add("Action", "Action");

            ProjectGrid.column = param;
            var grid = ProjectGrid.MakeGrid(list, "", "", 0, true, "", "", "Home", "Api Log", "#", "#");
            ViewData["grid"] = grid;

            return View();
        }
        public ActionResult ApiRequestLog(string ApiRequestLog)
        {
            var list = _api.GetApiLogList(ApiRequestLog);
            foreach (var item in list)
            {
                item.Action = StaticData.GetActions("APILog", "", this, "", "");
            }
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("apiRequest", "API Request");
            param.Add("apiResponse", "API Response");
            ProjectGrid.column = param;
            var grid = ProjectGrid.MakeGrid(list, "", "", 0, true, "", "", "Home", "Api Log", "#", "#");
            ViewData["grid"] = grid;
            return View();
        }


    }
}