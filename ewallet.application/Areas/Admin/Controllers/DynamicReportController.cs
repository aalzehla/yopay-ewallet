using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ewallet.application.Library;
using ewallet.application.Models;
using ewallet.business.Balance;
using ewallet.business.Client;
using ewallet.business.DynamicReport;
using ewallet.shared.Models.Balance;
using ewallet.shared.Models.DynamicReport;
using Microsoft.Ajax.Utilities;

namespace ewallet.application.Areas.Admin.Controllers
{
    public class DynamicReportController : Controller
    {
        private IDynamicReportBusiness _dynamicReport;
        string ControllerName = "DynamicReport";
        public DynamicReportController(IDynamicReportBusiness dynamicReportBusiness)
        {
            _dynamicReport = dynamicReportBusiness;
        }

        #region Transaction Report
        public ActionResult TransactionReport(string Search = "", int Pagesize = 10)
        {
            List<DynamicReportCommon> dynamicReportCommons = _dynamicReport.GetTransactionReport();
            List<DynamicReportModel> reportModel = dynamicReportCommons.MapObjects<DynamicReportModel>();

            foreach (var item in reportModel)
            {
                item.Action = StaticData.GetActions("TransactionReport", item.AgentId.EncryptParameter(), this, "", "", item.TxnId);
            }
            //Column Creator
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("TxnDate", "Txn Date");
            param.Add("TxnId", "Txn Id");
            param.Add("ProductName", "Product");
            param.Add("AgentId", "Agent Id");
            param.Add("SubscriberNo", "Subscriber No.");
            param.Add("Amount", "Amount");
            param.Add("TxnStatus", "Txn Status");
            param.Add("UserId", "User Id");
            param.Add("Remarks", "Remarks");
            param.Add("Action", "Action");
            ProjectGrid.column = param;
            //Ends
            var grid = ProjectGrid.MakeGrid(reportModel, "", Search, Pagesize, false, "", "", "Home", "", "", "");
            ViewData["grid"] = grid;

            return View();
        }
        public ActionResult TransactionReportDetail(string ID = "", string TxnId = "")
        {
            DynamicReportModel dynamicReportModel = new DynamicReportModel();
            DynamicReportCommon dynamicReportCommons = new DynamicReportCommon();
            string id = ID.DecryptParameter();
            string txnId = TxnId;
            if (!String.IsNullOrEmpty(id))
            {
                dynamicReportCommons = _dynamicReport.GetTransactionReportDetail(txnId, id);
            }
            dynamicReportModel = dynamicReportCommons.MapObject<DynamicReportModel>();
            return View(dynamicReportModel);
        }
        #endregion

        #region Pending Transaction
        public ActionResult PendingTransaction(string Search = "", int Pagesize = 10)
        {
            List<DynamicReportCommon> dynamicReportCommons = _dynamicReport.GetPendingReport();
            List<DynamicReportModel> reportModel = dynamicReportCommons.MapObjects<DynamicReportModel>();

            foreach (var item in reportModel)
            {
                item.Action = StaticData.GetActions("PendingTransaction", item.AgentId.EncryptParameter(), this, "", "", item.TxnId);
            }
            //Column Creator
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("TxnDate", "Txn Date");
            param.Add("TxnId", "Txn Id");
            param.Add("ProductName", "Product");
            param.Add("AgentId", "Agent Id");
            param.Add("SubscriberNo", "Subscriber No.");
            param.Add("Amount", "Amount");
            param.Add("TxnStatus", "Txn Status");
            param.Add("UserId", "User Id");
            param.Add("Remarks", "Remarks");
            param.Add("Action", "Action");
            ProjectGrid.column = param;
            //Ends
            var grid = ProjectGrid.MakeGrid(reportModel, "", Search, Pagesize, false, "", "", "Home", "", "", "");
            ViewData["grid"] = grid;

            return View();
        }
        public ActionResult PendingTransactionDetail(string ID = "", string TxnId = "")
        {
            DynamicReportModel dynamicReportModel = new DynamicReportModel();
            DynamicReportCommon dynamicReportCommons = new DynamicReportCommon();
            string id = ID.DecryptParameter();
            string txnId = TxnId;
            if (!String.IsNullOrEmpty(id))
            {
                dynamicReportCommons = _dynamicReport.GetTransactionReportDetail(txnId, id);
            }
            dynamicReportModel = dynamicReportCommons.MapObject<DynamicReportModel>();
            return View(dynamicReportModel);
        }
        #endregion

        #region Settlement Report

        public ActionResult SettlementReport()
        {
            ViewBag.EmptyMessage = "False";
            return View();
        }
        [HttpPost]
        public ActionResult SettlementReport(string MobileNum)
        {
            //Start Search UserId from Mobile Number
            IWalletUserBusiness _walletUserBusiness = new WalletUserBusiness();
            var userinfo = _walletUserBusiness.UserInfo(MobileNum);
            if (string.IsNullOrEmpty(userinfo.UserId))
            {
                ViewBag.EmptyMessage = "True";
                return View();
            }

            string UserId = userinfo.UserId.ToString();
            //Ends
            List<DynamicReportCommon> dynamicReportCommons = _dynamicReport.GetSettlementReport(UserId);
            List<DynamicReportModel> reportModel = dynamicReportCommons.MapObjects<DynamicReportModel>();

            foreach (var item in reportModel)
            {
                item.Action = StaticData.GetActions("PendingTransaction", item.AgentId.EncryptParameter(), this, "", "", item.TxnId);
            }
            //Column Creator
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("TxnDate", "Txn Date");
            param.Add("TxnType", "Txn Type");
            param.Add("Remarks", "Remarks");
            param.Add("Debit", "Debit");
            param.Add("Credit", "Credit");
            param.Add("Amount", "Amount");
            ProjectGrid.column = param;
            //Ends
            var grid = ProjectGrid.MakeGrid(reportModel, "hidebreadcrumb", "", 10, false, "", "", "", "", "", "");
            ViewData["grid"] = grid;

            return View();
        }

        #endregion

        #region Manual Commission Report
        public ActionResult ManualCommissionReport()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManualCommissionReport(string mobileno)
        {
            IWalletUserBusiness _walletUserBusiness = new WalletUserBusiness();
            var userinfo = _walletUserBusiness.UserInfo(mobileno);
            if (string.IsNullOrEmpty(userinfo.UserId))
            {
                ViewBag.EmptyMessage = "True";
                return View();
            }

            string UserId = userinfo.UserId.ToString();
            
            //Ends
            List<DynamicReportCommon> dynamicReportCommons = _dynamicReport.GetManualCommissionReport(UserId);
            List<DynamicReportModel> reportModel = dynamicReportCommons.MapObjects<DynamicReportModel>();
            Decimal Total_Commission = 0;
            foreach (var item in reportModel)
            {
                Total_Commission = Total_Commission + Convert.ToDecimal(item.CommissionEarned);
            }

            ViewBag.totalcommission = (float)Total_Commission;

            IDictionary<string, string> param = new Dictionary<string, string>();

            param.Add("TxnDate", "Txn Date");
            param.Add("TxnType", "Txn Type");
            param.Add("Remarks", "Remarks");
            param.Add("ProductName", "Product Name");
            param.Add("CommissionEarned", "Commission Earned");
            param.Add("Amount", "Amount");
            ProjectGrid.column = param;
            //Ends
            var grid = ProjectGrid.MakeGrid(reportModel, "hidebreadcrumb", "", 10, false, "", "", "", "", "", "");
            ViewData["grid"] = grid;

            return View();
        }
        #endregion

    }
}