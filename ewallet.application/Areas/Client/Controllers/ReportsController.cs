﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ewallet.application.Library;
using ewallet.application.Models;
using ewallet.business.Common;
using ewallet.business.DynamicReport;
using ewallet.shared.Models.DynamicReport;

namespace ewallet.application.Areas.Client.Controllers
{
    public class ReportsController : Controller
    {
        private IDynamicReportBusiness _Report;
        private ICommonBusiness _icb;
        string ControllerName = "DynamicReport";
        public ReportsController(IDynamicReportBusiness dynamicReportBusiness,ICommonBusiness icb)
        {
            _Report = dynamicReportBusiness;
            _icb = icb;
        }
        [HttpGet]
        public ActionResult Index()
        {
          
            DynamicReportFilter DRF = new DynamicReportFilter();
            DRF.UserId = Session["UserId"].ToString();

            DRF.reportlist = _Report.GetSettlementReportclient(DRF);

            foreach (var item in DRF.reportlist)
            {
                item.TxnId = item.TxnId.EncryptParameter();
            }

            LoadDropDownList(DRF);
            return View(DRF);
        }
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Indexp(DynamicReportFilter DRF )
        {

            DRF.UserId = Session["UserId"].ToString();
             

            DRF.reportlist = _Report.GetSettlementReportclient(DRF);


            foreach (var item in DRF.reportlist)
            {
                item.TxnId = item.TxnId.EncryptParameter();
            }

            LoadDropDownList(DRF);

            return View(DRF);
        }

        public void LoadDropDownList(DynamicReportFilter dynamicReportCommon)
        {
            ViewBag.Services = ApplicationUtilities.SetDDLValue(LoadDropdownList("ProductName"), dynamicReportCommon.Service, "--Services--");
            ViewBag.txnType = ApplicationUtilities.SetDDLValue(LoadDropdownList("TxnType"), dynamicReportCommon.TxnType, "--Transaction Type--");
            ViewBag.txnStatus = ApplicationUtilities.SetDDLValue(LoadDropdownList("txnstatus"), dynamicReportCommon.TxnType, "--Transaction Status--");
        }

        public Dictionary<string, string> LoadDropdownList(string flag, string search1 = "")
        {
            switch (flag)
            {
                case "ProductName":
                    return _icb.sproc_get_dropdown_list("servicelist");
                case "TxnType":
                    return _icb.sproc_get_dropdown_list("011");
               
                case "txnstatus":
                    {
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        dict.Add("Success", "Success");
                        dict.Add("Pending", "Pending");
                        dict.Add("Failed", "Failed");
                        return dict;
                    };


            }
            return null;
        }
        public ActionResult TransactionDetail(string txnid,string txntype)
        {
            DynamicReportModel dynamicReportModel = new DynamicReportModel();
            //DynamicReportCommon dynamicReportCommons = new DynamicReportCommon();
            string txnId = txnid.DecryptParameter();
            string flag = "";
            if (txntype.ToUpper()=="MOBILE TOPUP")
            {
                flag = "m";
            }
            else if(txntype.ToUpper()=="BALANCE TRANSFER" || txntype.ToUpper()=="BALANCE REFUND")
            {
                flag = "t";
            }
            else if (txntype.ToUpper() == "CASH BACK")
            {
                flag = "M";
            }
            else if(txntype.ToUpper()=="FUND TRANSFER")
            {
                flag = "F";
            }

            if (!String.IsNullOrEmpty(txnId)&& !string.IsNullOrEmpty(flag))
            {               
                dynamicReportModel = _Report.GetActivityDetail(txnId,flag).MapObject<DynamicReportModel>();                 
                return View(dynamicReportModel);
            }            
            return RedirectToAction("Index");
        }
    }
}