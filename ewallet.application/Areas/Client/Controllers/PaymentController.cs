using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using ewallet.application.Library;
using ewallet.application.Models;
using ewallet.business.Card;
using ewallet.business.Client;
using ewallet.business.Client.MobileTopup;
using ewallet.business.DynamicReport;
using ewallet.business.Mobile;
using ewallet.business.TransactionLimit;
using ewallet.shared.Models.Mobile;
using paypoint.service.data;

namespace ewallet.application.Areas.Client.Controllers
{
    public class PaymentController : Controller
    {
        IWalletUserBusiness _payment;
        IMobileTopUpPaymentBusiness _mtp;
        IMobilePaymentBusiness _mpaymentPP;
        ITransactionLimitBusiness _transactionLimit;
        ICardBusiness _card;
        string ControllerName = "Payment";
        public PaymentController(IWalletUserBusiness payment, IMobileTopUpPaymentBusiness mtp, IMobilePaymentBusiness mpaymentPP, ITransactionLimitBusiness tbuss, ICardBusiness card)
        {
            _payment = payment;
            _mtp = mtp;
            _mpaymentPP = mpaymentPP;
            _transactionLimit = tbuss;
            _card = card;
        }


        #region Mobile Topup
        public ActionResult MobileTopUp()
        {
            //ViewBag.ProductId = id;
            //TempData["ProductId"] = id;
            //Session["ProductId"] = id;

            if (Session["UserType"].ToString().ToLower() == "merchant")
                return RedirectToAction("MobileTopUp3");


            string AgentId = Session["AgentId"].ToString();
            var TxnLimit = _transactionLimit.GetTransactionLimitForUser(AgentId);
            ClientModel clientModel = new ClientModel()
            {
                TxnLimitMax = TxnLimit.TxnLimitMax,
                TxnDailyLimitMax = TxnLimit.TxnDailyLimitMax,
                TxnMonthlyLimitMax = TxnLimit.TxnMonthlyLimitMax,
                TxnDailyRemainingLimit = TxnLimit.TxnDailyRemainingLimit,
                TxnMonthlyRemainingLimit = TxnLimit.TxnMonthlyRemainingLimit

            };
            return View(clientModel);
        }

        [OverrideActionFilters]
        public ActionResult MobileTopUp3()
        {
            string AgentId = Session["AgentId"].ToString();
            var TxnLimit = _transactionLimit.GetTransactionLimitForUser(AgentId);
            ClientModel clientModel = new ClientModel()
            {
                TxnLimitMax = TxnLimit.TxnLimitMax,
                TxnDailyLimitMax = TxnLimit.TxnDailyLimitMax,
                TxnMonthlyLimitMax = TxnLimit.TxnMonthlyLimitMax,
                TxnDailyRemainingLimit = TxnLimit.TxnDailyRemainingLimit,
                TxnMonthlyRemainingLimit = TxnLimit.TxnMonthlyRemainingLimit

            };
            return View(clientModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MobileTopup(ClientModel clientModel)
        {
            //string productid = clientModel.ProductId;
            var validMobileNo = MobileNumberValidate(clientModel.MobileNo);
            if (validMobileNo.Code != "0")
            {
                ModelState.AddModelError("MobileNo", validMobileNo.Message);
                return View(clientModel);
            }

            MobileTopUpPaymentRequest mtpr = new MobileTopUpPaymentRequest()
            {
                action_user = Session["UserName"].ToString(),
                product_id = clientModel.ProductId,
                amount = clientModel.Amount,
                subscriber_no = clientModel.MobileNo,
                quantity = "",
                additonal_data = "",
                CardNo= clientModel.CardNo,
                CardAmount = clientModel.CardAmount
            };
            var response = _mtp.MobileTopUpPaymentRequest(mtpr);
            if (response.Code == 0)
            {
                var amt = clientModel.Amount.Contains(".") ? clientModel.Amount.Split('.')[0].ToString() : clientModel.Amount;
                var payment = _mpaymentPP.ConsumeService(clientModel.MobileNo, long.Parse(amt));
                bool failed = false;
                if (payment.Code == shared.Models.ResponseCode.Success)
                {
                    var billNo = payment.Extra1;
                    var refStan = payment.Extra2;
                    var ppresponse = (PPResponse)payment.Data;
                    var data = new MobileTopUpPaymentUpdateRequest();
                    data.action_user = Session["UserName"].ToString();
                    data.transaction_id = response.Extra1;
                    data.additonal_data = Newtonsoft.Json.JsonConvert.SerializeObject(ppresponse);
                    data.amount = clientModel.Amount;
                    data.bill_number = billNo;
                    data.refstan = refStan;
                    data.status_code = ppresponse.Result;
                    data.remarks = ppresponse.ResultMessage;
                    data.ip_address = ApplicationUtilities.GetIP();
                    data.product_id = clientModel.ProductId;
                    data.partner_txn_id = ppresponse.TransactionId;
                    response = _mtp.MobileTopUpPaymentResponse(data);
                }
                else
                {
                    var ppresponse = (PPResponse)payment.Data;
                    var data = new MobileTopUpPaymentUpdateRequest();
                    data.action_user = Session["UserName"].ToString();
                    data.transaction_id = response.Extra1;
                    data.additonal_data = Newtonsoft.Json.JsonConvert.SerializeObject(ppresponse);
                    data.amount = clientModel.Amount;
                    data.status_code = ((int)payment.Code).ToString();
                    data.remarks = payment.Message;
                    data.ip_address = ApplicationUtilities.GetIP();
                    data.product_id = clientModel.ProductId;
                    response = _mtp.MobileTopUpPaymentResponse(data);
                    failed = true;

                }
                if (failed)
                {
                    response.Code = shared.Models.ResponseCode.Failed;
                    response.Message = "Transaction Failed";
                }
                response.SetMessageInTempData(this, "MobileTopup");
                if (failed)
                    return RedirectToAction("MobileTopup");
                return RedirectToAction("ResultPage", ControllerName, new { txnid = response.Extra1.EncryptParameter() });
                //return RedirectToAction("MobileTopup");
            }
            response.SetMessageInTempData(this, "MobileTopup");

            if (Session["UserType"].ToString().ToLower() == "merchant")
                return RedirectToAction("MobileTopUp3");


            return RedirectToAction("MobileTopup");

        }
        public ActionResult MobileTopUp2()
        {
            //ViewBag.ProductId = id;
            //TempData["ProductId"] = id;
            //Session["ProductId"] = id;
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MobileTopup2(ClientModel clientModel)
        {
            //string productid = clientModel.ProductId;
            var validMobileNo = MobileNumberValidate2(clientModel.MobileNo, long.Parse(clientModel.Amount == "" || clientModel.Amount == null ? "0" : (long.Parse(clientModel.Amount) * 100).ToString()));
            if (validMobileNo.Code != "0")
            {
                ModelState.AddModelError("MobileNo", validMobileNo.Message);
                return View(clientModel);
            }

            MobileTopUpPaymentRequest mtpr = new MobileTopUpPaymentRequest()
            {
                action_user = Session["UserName"].ToString(),
                product_id = clientModel.ProductId,
                amount = clientModel.Amount,
                subscriber_no = clientModel.MobileNo,
                quantity = "",
                additonal_data = ""
            };
            var response = _mtp.MobileTopUpPaymentRequest(mtpr);
            if (response.Code == 0)
            {
                var amt = clientModel.Amount.Contains(".") ? clientModel.Amount.Split('.')[0].ToString() : clientModel.Amount;
                var package = _mpaymentPP.GetPackage(clientModel.MobileNo, long.Parse(amt), validMobileNo.ServiceCode);
                if (package.Code == shared.Models.ResponseCode.Success)
                {
                    var billNo = package.Extra1;
                    var refStan = package.Extra2;
                    var payment = _mpaymentPP.Payment(clientModel.MobileNo, long.Parse(clientModel.Amount), validMobileNo.ServiceCode, billNo, refStan);
                    var ppresponse = (PPResponse)payment.Data;
                    var data = new MobileTopUpPaymentUpdateRequest();
                    data.action_user = Session["UserName"].ToString();
                    data.transaction_id = response.Extra1;
                    data.additonal_data = Newtonsoft.Json.JsonConvert.SerializeObject(ppresponse);
                    data.amount = clientModel.Amount;
                    data.bill_number = package.Extra1;
                    data.refstan = package.Extra2;
                    data.status_code = ppresponse.Result;
                    data.remarks = ppresponse.ResultMessage;
                    data.ip_address = ApplicationUtilities.GetIP();
                    data.product_id = clientModel.ProductId;
                    response = _mtp.MobileTopUpPaymentResponse(data);
                }
                else
                {
                    var ppresponse = (PPResponse)package.Data;
                    var data = new MobileTopUpPaymentUpdateRequest();
                    data.action_user = Session["UserName"].ToString();
                    data.transaction_id = response.Extra1;
                    data.additonal_data = Newtonsoft.Json.JsonConvert.SerializeObject(ppresponse);
                    data.amount = clientModel.Amount;
                    data.status_code = ppresponse.Result;
                    data.remarks = ppresponse.ResultMessage;
                    data.ip_address = ApplicationUtilities.GetIP();
                    data.product_id = clientModel.ProductId;
                    data.partner_txn_id = ppresponse.TransactionId;
                    response = _mtp.MobileTopUpPaymentResponse(data);
                }

                response.SetMessageInTempData(this, "MobileTopup");
                return RedirectToAction("ResultPage", ControllerName, new { txnid = response.Extra1.EncryptParameter() });
                //return RedirectToAction("MobileTopup");
            }
            return View(clientModel);

        }
        #endregion Mobile Topup

        public ActionResult ResultPage(string txnid)
        {
            DynamicReportModel dynamicReportModel = new DynamicReportModel();
            //DynamicReportCommon dynamicReportCommons = new DynamicReportCommon();
            string txnId = txnid.DecryptParameter();
            if (!String.IsNullOrEmpty(txnId))
            {
                IDynamicReportBusiness _dynamicReport = new DynamicReportBusiness();
                dynamicReportModel = _dynamicReport.GetTransactionReportDetail(txnId).MapObject<DynamicReportModel>(); ;
                return View(dynamicReportModel);
            }
            //dynamicReportModel = dynamicReportCommons.MapObject<DynamicReportModel>();

            if (Session["UserType"].ToString().ToLower() == "merchant")
                return RedirectToAction("MobileTopUp3");


            return RedirectToAction("MobileTopup");
        }
        [HttpPost, OverrideActionFilters]
        public async System.Threading.Tasks.Task<JsonResult> CheckMobileNumber2(string MobileNo)
        {
            MobileNumberClass2 response = MobileNumberValidate2(MobileNo);
            string Code = response.Code;
            string Message = response.Message;
            string LogoUrl = response.ProductLogo;
            string MinAmount = response.MinAmount;
            string MaxAmount = response.MaxAmount;
            string ProductId = response.ProductId;
            string CommissionValue = response.CommissionValue;
            string CommissionType = response.CommissionType;
            return Json(new { Code, Message, LogoUrl, MinAmount, MaxAmount, ProductId, CommissionValue, CommissionType, response.CompanyCode, response.ServiceCode }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, OverrideActionFilters]
        public async System.Threading.Tasks.Task<JsonResult> CheckMobileNumber(string MobileNo)
        {
            MobileNumberClass response = MobileNumberValidate(MobileNo);
            string Code = response.Code;
            string Message = response.Message;
            string LogoUrl = response.ProductLogo;
            string MinAmount = response.MinAmount;
            string MaxAmount = response.MaxAmount;
            string ProductId = response.ProductId;
            string CommissionValue = response.CommissionValue;
            string CommissionType = response.CommissionType;
            return Json(new { Code, Message, LogoUrl, MinAmount, MaxAmount, ProductId, CommissionValue, CommissionType }, JsonRequestBehavior.AllowGet);
        }
        private MobileNumberClass MobileNumberValidate(string MobileNumber)
        {
            //string type = "1";
            string userid = Session["UserId"].ToString();
            var serviceslist = _payment.ServiceDetail(userid);
            MobileNumberClass response = new MobileNumberClass();
            response.Code = "1";
            response.Message = "Invalid Number!!";
            if (string.IsNullOrEmpty(MobileNumber))
            {
                response.Message = "Number is Invalid!!";
                return response;
            }
            if (MobileNumber.Length > 10 && MobileNumber.Substring(0, 3) == "977")
            {
                MobileNumber = MobileNumber.Substring(3);
            }
            if (!MobileNumberLengthValidate(MobileNumber))
            {
                response.Code = "1";
                response.Message = "Please Enter Valid Mobile Number of length 10";
                return response;
            }
            if (MobileNumber.Substring(0, 3) == "980" || MobileNumber.Substring(0, 3) == "981" || MobileNumber.Substring(0, 3) == "982" || (ConfigurationManager.AppSettings["phase"] != null && ConfigurationManager.AppSettings["phase"].ToString().ToUpper() == "DEVELOPMENT" && MobileNumber.Substring(0, 3) == "880"))//NCELL
            {
                var serviceInfo = serviceslist.FirstOrDefault(x => x.ProductId == "2");
                response.Code = "0";
                response.Message = serviceInfo.ProductLabel;
                response.ProductLogo = "/Content/assets/images/service logos/" + serviceInfo.ProductLogo;
                response.MinAmount = string.IsNullOrEmpty(serviceInfo.MinAmount) ? "1" : serviceInfo.MinAmount;
                response.MaxAmount += string.IsNullOrEmpty(serviceInfo.MaxAmount) ? "1000" :serviceInfo.MaxAmount;
                response.ProductId += serviceInfo.ProductId;
                response.CommissionValue += serviceInfo.CommissionValue;
                response.CommissionType += serviceInfo.CommissionType;
                return response;
            }
            else if (MobileNumber.Substring(0, 3) == "984" || MobileNumber.Substring(0, 3) == "986")//ntc prepaid
            {
                var serviceInfo = serviceslist.FirstOrDefault(x => x.ProductId == "1");
                response.Code = "0";
                response.Message = serviceInfo.ProductLabel;
                response.ProductLogo = "/Content/assets/images/service logos/" + serviceInfo.ProductLogo;
                response.MinAmount = string.IsNullOrEmpty(serviceInfo.MinAmount) ? "1" : serviceInfo.MinAmount;
                response.MaxAmount += string.IsNullOrEmpty(serviceInfo.MaxAmount) ? "1000" : serviceInfo.MaxAmount;
                response.ProductId += serviceInfo.ProductId;
                response.CommissionValue += serviceInfo.CommissionValue;
                response.CommissionType += serviceInfo.CommissionType;
                return response;
            }
            else if (MobileNumber.Substring(0, 3) == "974" || MobileNumber.Substring(0, 3) == "976" || MobileNumber.Substring(0, 3) == "975")//ntc cdma
            {
                var serviceInfo = serviceslist.FirstOrDefault(x => x.ProductId == "9");
                response.Code = "0";
                response.Message = serviceInfo.ProductLabel;
                response.ProductLogo = "/Content/assets/images/service logos/" + serviceInfo.ProductLogo;
                response.MinAmount = string.IsNullOrEmpty(serviceInfo.MinAmount) ? "1" : serviceInfo.MinAmount;
                response.MaxAmount += string.IsNullOrEmpty(serviceInfo.MaxAmount) ? "1000" : serviceInfo.MaxAmount;
                response.ProductId += serviceInfo.ProductId;
                response.CommissionValue += serviceInfo.CommissionValue;
                response.CommissionType += serviceInfo.CommissionType;
                return response;
            }
            else if (MobileNumber.Substring(0, 3) == "985")//ntc postpaid
            {
                var serviceInfo = serviceslist.FirstOrDefault(x => x.ProductId == "14");
                response.Code = "0";
                response.Message = serviceInfo.ProductLabel;
                response.ProductLogo = "/Content/assets/images/service logos/" + serviceInfo.ProductLogo;
                response.MinAmount = string.IsNullOrEmpty(serviceInfo.MinAmount) ? "1" : serviceInfo.MinAmount;
                response.MaxAmount += string.IsNullOrEmpty(serviceInfo.MaxAmount) ? "1000" : serviceInfo.MaxAmount;
                response.ProductId += serviceInfo.ProductId;
                response.CommissionValue += serviceInfo.CommissionValue;
                response.CommissionType += serviceInfo.CommissionType;
                return response;
            }
            else if (MobileNumber.Substring(0, 3) == "988" || MobileNumber.Substring(0, 3) == "961" || MobileNumber.Substring(0, 3) == "962")//smartcell
            {
                var serviceInfo = serviceslist.FirstOrDefault(x => x.ProductId == "4");
                response.Code = "0";
                response.Message = serviceInfo.ProductLabel;
                response.ProductLogo = "/Content/assets/images/service logos/" + serviceInfo.ProductLogo;
                response.MinAmount = string.IsNullOrEmpty(serviceInfo.MinAmount) ? "1" : serviceInfo.MinAmount;
                response.MaxAmount += string.IsNullOrEmpty(serviceInfo.MaxAmount) ? "1000" :serviceInfo.MaxAmount;
                response.ProductId += serviceInfo.ProductId;
                response.CommissionValue += serviceInfo.CommissionValue;
                response.CommissionType += serviceInfo.CommissionType;
                return response;
            }
            else if (MobileNumber.Substring(0, 3) == "972")//UTL
            {
                var serviceInfo = serviceslist.FirstOrDefault(x => x.ProductId == "3");
                response.Code = "0";
                response.Message = serviceInfo.ProductLabel;
                response.ProductLogo = "/Content/assets/images/service logos/" + serviceInfo.ProductLogo;
                response.MinAmount = string.IsNullOrEmpty(serviceInfo.MinAmount) ? "1" : serviceInfo.MinAmount;
                response.MaxAmount += string.IsNullOrEmpty(serviceInfo.MaxAmount) ? "1000" :serviceInfo.MaxAmount;
                response.ProductId += serviceInfo.ProductId;
                response.CommissionValue += serviceInfo.CommissionValue;
                response.CommissionType += serviceInfo.CommissionType;
                return response;
            }
            else
            {
                return response;
            }
        }
        private MobileNumberClass2 MobileNumberValidate2(string MobileNumber, long Amount = 0)
        {
            //string type = "1";
            string userid = Session["UserId"].ToString();
            var serviceslist = _payment.ServiceDetail(userid);
            MobileNumberClass2 response = new MobileNumberClass2();
            response.Code = "1";
            response.Message = "Invalid Number!!";
            if (string.IsNullOrEmpty(MobileNumber))
            {
                response.Message = "Number is Invalid!!";
                return response;
            }
            if (MobileNumber.Length > 10 && MobileNumber.Substring(0, 3) == "977")
            {
                MobileNumber = MobileNumber.Substring(3);
            }
            if (!MobileNumberLengthValidate(MobileNumber))
            {
                response.Code = "1";
                response.Message = "Please Enter Valid Mobile Number of length 10";
                return response;
            }
            if (MobileNumber.Substring(0, 3) == "980" || MobileNumber.Substring(0, 3) == "981" || MobileNumber.Substring(0, 3) == "982")//NCELL
            {
                var serviceInfo = serviceslist.FirstOrDefault(x => x.ProductId == "2");
                response.Code = "0";
                response.Message = serviceInfo.ProductLabel;
                response.ProductLogo = "/Content/assets/images/service logos/" + serviceInfo.ProductLogo;
                response.MinAmount = string.IsNullOrEmpty(serviceInfo.MinAmount) ? "1" : serviceInfo.MinAmount;
                response.MaxAmount += string.IsNullOrEmpty(serviceInfo.MaxAmount) ? "1000" : "|" + serviceInfo.MaxAmount;
                response.ProductId += serviceInfo.ProductId;
                response.CommissionValue += serviceInfo.CommissionValue;
                response.CommissionType += serviceInfo.CommissionType;
                response.ServiceCode = "0";
                response.CompanyCode = "78";
                return response;
            }
            else if (MobileNumber.Substring(0, 3) == "984" || MobileNumber.Substring(0, 3) == "986")//ntc prepaid
            {
                var serviceInfo = serviceslist.FirstOrDefault(x => x.ProductId == "1");
                response.Code = "0";
                response.Message = serviceInfo.ProductLabel;
                response.ProductLogo = "/Content/assets/images/service logos/" + serviceInfo.ProductLogo;
                response.MinAmount = string.IsNullOrEmpty(serviceInfo.MinAmount) ? "1" : serviceInfo.MinAmount;
                response.MaxAmount += string.IsNullOrEmpty(serviceInfo.MaxAmount) ? "1000" : serviceInfo.MaxAmount;
                response.ProductId += serviceInfo.ProductId;
                response.CommissionValue += serviceInfo.CommissionValue;
                response.CommissionType += serviceInfo.CommissionType;
                response.ServiceCode = "0";
                response.CompanyCode = "585";
                return response;
            }
            else if (MobileNumber.Substring(0, 3) == "974" || MobileNumber.Substring(0, 3) == "976" || MobileNumber.Substring(0, 3) == "975")//ntc cdma
            {
                var serviceInfo = serviceslist.FirstOrDefault(x => x.ProductId == "9");
                response.Code = "0";
                response.Message = serviceInfo.ProductLabel;
                response.ProductLogo = "/Content/assets/images/service logos/" + serviceInfo.ProductLogo;
                response.MinAmount = string.IsNullOrEmpty(serviceInfo.MinAmount) ? "1" : serviceInfo.MinAmount;
                response.MaxAmount += string.IsNullOrEmpty(serviceInfo.MaxAmount) ? "1000" : "|" + serviceInfo.MaxAmount;
                response.ProductId += serviceInfo.ProductId;
                response.CommissionValue += serviceInfo.CommissionValue;
                response.CommissionType += serviceInfo.CommissionType;
                response.ServiceCode = "5";
                response.CompanyCode = "585";
                return response;
            }
            else if (MobileNumber.Substring(0, 3) == "985")//ntc postpaid
            {
                var serviceInfo = serviceslist.FirstOrDefault(x => x.ProductId == "14");
                response.Code = "0";
                response.Message = serviceInfo.ProductLabel;
                response.ProductLogo = "/Content/assets/images/service logos/" + serviceInfo.ProductLogo;
                response.MinAmount = string.IsNullOrEmpty(serviceInfo.MinAmount) ? "1" : serviceInfo.MinAmount;
                response.MaxAmount += string.IsNullOrEmpty(serviceInfo.MaxAmount) ? "1000" : "|" + serviceInfo.MaxAmount;
                response.ProductId += serviceInfo.ProductId;
                response.CommissionValue += serviceInfo.CommissionValue;
                response.CommissionType += serviceInfo.CommissionType;
                response.ServiceCode = "1";
                response.CompanyCode = "585";
                return response;
            }
            else if (MobileNumber.Substring(0, 3) == "988" || MobileNumber.Substring(0, 3) == "961" || MobileNumber.Substring(0, 3) == "962")//smartcell
            {
                var serviceInfo = serviceslist.FirstOrDefault(x => x.ProductId == "4");
                response.Code = "0";
                response.Message = serviceInfo.ProductLabel;
                response.ProductLogo = "/Content/assets/images/service logos/" + serviceInfo.ProductLogo;
                response.MinAmount = string.IsNullOrEmpty(serviceInfo.MinAmount) ? "1" : serviceInfo.MinAmount;
                response.MaxAmount += string.IsNullOrEmpty(serviceInfo.MaxAmount) ? "1000" : "|" + serviceInfo.MaxAmount;
                response.ProductId += serviceInfo.ProductId;
                response.CommissionValue += serviceInfo.CommissionValue;
                response.CommissionType += serviceInfo.CommissionType;
                ushort ServiceCode = 0;
                if (Amount == 1000)
                    ServiceCode = 10;
                else if (Amount == 2000)
                    ServiceCode = 0;
                else if (Amount == 5000)
                    ServiceCode = 1;
                else if (Amount == 10000)
                    ServiceCode = 2;
                else if (Amount == 20000)
                    ServiceCode = 3;
                else if (Amount == 50000)
                    ServiceCode = 4;
                else if (Amount == 100000)
                    ServiceCode = 5;
                response.ServiceCode = Amount != 2000 && ServiceCode == 0 ? "" : ServiceCode.ToString();
                response.CompanyCode = "709";
                return response;
            }
            else if (MobileNumber.Substring(0, 3) == "972")//UTL
            {
                var serviceInfo = serviceslist.FirstOrDefault(x => x.ProductId == "3");
                response.Code = "0";
                response.Message = serviceInfo.ProductLabel;
                response.ProductLogo = "/Content/assets/images/service logos/" + serviceInfo.ProductLogo;
                response.MinAmount = string.IsNullOrEmpty(serviceInfo.MinAmount) ? "1" : serviceInfo.MinAmount;
                response.MaxAmount += string.IsNullOrEmpty(serviceInfo.MaxAmount) ? "1000" : "|" + serviceInfo.MaxAmount;
                response.ProductId += serviceInfo.ProductId;
                response.CommissionValue += serviceInfo.CommissionValue;
                response.CommissionType += serviceInfo.CommissionType;
                ushort ServiceCode = 1;
                if (Amount == 1000 || Amount == 2000 || Amount == 5000 || Amount == 10000 || Amount == 25000 || Amount == 50000 || Amount == 100000)
                    ServiceCode = 0;
                response.ServiceCode = ServiceCode == 1 ? "" : ServiceCode.ToString();
                response.CompanyCode = "582";
                return response;
            }
            else
            {
                return response;
            }
        }
        private bool MobileNumberLengthValidate(string MobileNumber)
        {
            if (Regex.IsMatch(MobileNumber, @"^\d{10}$"))
            {
                return true;
            }
            return false;
        }
        //[HttpPost, OverrideActionFilters]
        //public async System.Threading.Tasks.Task<JsonResult> GetCards(string cardno)
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    string userid = Session["UserId"].ToString();
        //    var cardsList = _card.GetCardList(userid).MapObjects<CardModel>();
        //    var cardType = _card.GetCardType();
        //    Dictionary<string, string> cardNo = new Dictionary<string, string>();
        //    foreach (var item in cardsList.Where(x => x.CardNo == cardno).Where(x => x.Status.ToUpper() == "Y"))
        //    {
        //        cardNo.Add(item.CardId, item.CardNo);
        //    }
        //    list = ApplicationUtilities.SetDDLValue(cardNo, "");
        //    return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));
        //}

        private class MobileNumberClass
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public string ProductLogo { get; set; }
            public string MinAmount { get; set; }
            public string MaxAmount { get; set; }
            public string ProductId { get; set; }
            public string ProductLabel { get; set; }
            public string CommissionValue { get; set; }
            public string CommissionType { get; set; }
        }
        private class MobileNumberClass2
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public string ProductLogo { get; set; }
            public string MinAmount { get; set; }
            public string MaxAmount { get; set; }
            public string ProductId { get; set; }
            public string ProductLabel { get; set; }
            public string CommissionValue { get; set; }
            public string CommissionType { get; set; }
            public string ServiceCode { get; set; }
            public string CompanyCode { get; set; }
        }
    }



}