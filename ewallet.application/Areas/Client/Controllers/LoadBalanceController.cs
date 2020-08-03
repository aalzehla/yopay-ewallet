using ewallet.application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ewallet.application.Library;
using ewallet.shared.Models;
using ewallet.shared.Models.WalletUser;
using ewallet.business.Client;
using ewallet.business.LoadBalance;
using ewallet.shared.Models.LoadBalance;
using System.Configuration;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using admin.onepg.api.Models;
using ewallet.application.Models.OnePG;

namespace ewallet.application.Areas.Client.Controllers
{
    public class LoadBalanceController : Controller
    {
        IWalletUserBusiness _walletUser;
        ILoadBalanceBusiness _iLoad;
        public LoadBalanceController(IWalletUserBusiness walletUser)
        {
            _walletUser = walletUser;
            _iLoad = new LoadBalanceBusiness();
        }
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index(WalletBalanceModel walletBalance)
        {

            Dictionary<string, string> PurposeList = _walletUser.GetProposeList();
            walletBalance.PurposeList = ApplicationUtilities.SetDDLValue(PurposeList, "", "--Propose--");

            if ((Convert.ToDecimal(walletBalance.Amount) > 1000 || Convert.ToDecimal(walletBalance.Amount) < 10) && walletBalance.Type == "T")
            {
                ModelState.AddModelError("Amount", "Amount should be between 10-1000");
                return View(walletBalance);
            }

            ModelState.Remove("Purpose");
            ModelState.Remove("ReceiverAgentId");
            string usertype = Session["UserType"].ToString();
            string agentid = Session["AgentId"].ToString();
            CommonDbResponse response = _walletUser.CheckMobileNumber(agentid, walletBalance.MobileNumber, usertype, "lb");
            if (response.Code != 0)
            {
                ModelState.AddModelError("MobileNumber", "Invalid User Detail");
                return View(walletBalance);
            }
            else
            {
                ModelState.Remove("MobileNumber");
            }
            if (ModelState.IsValid)
            {
                walletBalance.AgentId = Session["AgentId"].ToString();
                walletBalance.ActionUser = Session["UserName"].ToString();
                walletBalance.IpAddress = ApplicationUtilities.GetIP();
                WalletBalanceCommon walletBalanceCommon = walletBalance.MapObject<WalletBalanceCommon>();
                CommonDbResponse dbResponse = _walletUser.AgentToWallet(walletBalanceCommon);
                if (dbResponse.Code == 0)
                {
                    dbResponse.SetMessageInTempData(this, "balanceTransfer");
                    return RedirectToAction("Index");
                }
                else
                {
                    dbResponse.SetMessageInTempData(this, "balanceTransfer");
                }
            }
            else
            {
                return View(walletBalance);
            }

            return RedirectToAction("Index");
        }
        [HttpPost, OverrideActionFilters]
        public async System.Threading.Tasks.Task<JsonResult> CheckMobileNumber(string MobileNo)
        {
            string usertype = Session["UserType"].ToString();
            string agentid = Session["AgentId"].ToString();
            CommonDbResponse response = _walletUser.CheckMobileNumber(agentid, MobileNo, usertype, "lb");
            int Code = (int)response.Code;
            return Json(new { Code }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult LoadBalanceIndex()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoadBalanceIndex(LoadBalanceModel balance)
        {
            LoadBalanceCommon ld = new LoadBalanceCommon();

            balance.action_user = Session["UserName"].ToString();
            balance.action_ip = ApplicationUtilities.GetIP();
            balance.action_browser = HttpContext.Request.Browser.ToString();

            ld = balance.MapObject<LoadBalanceCommon>();

            string redirectUrl = "";
            CommonDbResponse dbResponse = _iLoad.LoadBalance(ld);
            if (dbResponse.Code == ResponseCode.Success)
            {
                var modeleResponse = MakeHttpRequest.InvokeGetProcessId("1", "anujApi", dbResponse.Extra1, balance.amount, "", "anujApi", "Anuj@123", "AnujSecert");
                if (modeleResponse.code == "0")
                {
                    ProcessResponse midddlewareModel = ApplicationUtilities.MapObject<ProcessResponse>(modeleResponse.data);
                    Dictionary<String, string> formParams = new Dictionary<string, string>();
                    formParams.Add("MerchantId", midddlewareModel.MerchantId);
                    formParams.Add("MerchantTxnId", midddlewareModel.MerchantTxnId);
                    formParams.Add("ProcessId", midddlewareModel.ProcessId);
                    formParams.Add("Amount", midddlewareModel.Amount.ToString());
                    formParams.Add("TransactionRemarks", balance.remarks);
                    formParams.Add("MerchantName", "anujApi");

                    var responseObj = ApplicationUtilities.FormBuilder("gateway", midddlewareModel.GatewayFormMethod, midddlewareModel.GatewayUrl, formParams);
                    Response.Write(responseObj);
                    Response.End();
                }
            }
            return View(balance);
            
        }

        public GatewayConfigModel MiddlewareReponse(LoadBalanceModel balance, string functionName)
        {
            GatewayConfigModel gcc = null;
            string middlewareUrl = ConfigurationManager.AppSettings["middleware"].ToString();
            string configKey = ConfigurationManager.AppSettings["gatwayconfig"] != null ? ConfigurationManager.AppSettings["gatwayconfig"].ToString() : "gateway.json";
            string path = Server.MapPath("~/Scripts");
            string fullpath = path + "/" + configKey;
            try
            {
                using (StreamReader s = new StreamReader(fullpath))
                {

                    string json = s.ReadToEnd();
                    return gcc = json.SerializeJSON<GatewayConfigModel>();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }
            //if (functionName.ToLower() == "GetProcessId")
            //{
            //   string Url = middlewareUrl + "onepgservice/Ewallet/GetProcessId";
            //    using (WebClient client = new WebClient())
            //    {
            //        client.Credentials = new NetworkCredential(gcc.ApiUsername, gcc.ApiPassword);
            //        var postValues = new NameValueCollection();
            //        postValues["MerchantId"] = balance.user_id;
            //        postValues["MerchantName"] = balance.action_user;
            //        postValues["Amount"] = balance.amount;
            //        postValues["MerchantTxnId"] = balance.pmt_txn_id;
            //        postValues["SecretKey"] = gcc.SecretKey;
            //        var reponse = client.UploadValues(Url, "Get", postValues);
            //        return Encoding.Default.GetString(reponse);

            //    }
            //}
            //if (functionName.ToLower() == "GetTransactionDetail")
            //{
            //    string Url = middlewareUrl + "onepgservice/Ewallet/GetTransactionDetail";

            //    using (WebClient client = new WebClient())
            //    {
            //        client.Credentials = new NetworkCredential(gcc.ApiUsername, gcc.ApiPassword);
            //        var postValues = new NameValueCollection();
            //        postValues["MerchantId"] = balance.user_id;
            //        postValues["MerchantName"] = balance.action_user;
            //        postValues["Amount"] = balance.amount;
            //        postValues["MerchantTxnId"] = balance.pmt_txn_id;
            //        postValues["SecretKey"] = gcc.SecretKey;
            //        var reponse = client.UploadValues(Url, "Get", postValues);
            //        return Encoding.Default.GetString(reponse);

            //    }
            //}
            //else
            //{
            //    return "";
            //}


        }
        [OverrideActionFilters]
        [HttpGet]
        public ActionResult ReceivePaymentNotification(string MerchantTxnId, string GatewayTxnId)
        {
            //check MerchantTxnId in our db first
            var dbRes = _iLoad.CheckTrnasactionExistence(MerchantTxnId, GatewayTxnId);
            if (dbRes.Code == 0)
            {
                ewallet.application.Models.OnePG.CommonResponse resp = MakeHttpRequest.InvokeCheckTransactionStatus("1", "anujApi", MerchantTxnId, "anujApi", "Anuj@123", "AnujSecert");
                if (resp.code == "0")
                {
                    var transactionModel = ApplicationUtilities.MapObject<CheckTransactionResponse>(resp.data);
                    LoadBalanceCommon lBalance = new LoadBalanceCommon()
                    {
                        pmt_gateway_id = "",
                        pmt_gateway_txn_id = GatewayTxnId,
                        gateway_status = transactionModel.Status,
                        gateway_process_id = "",
                        action_user = "System",
                        action_ip = ApplicationUtilities.GetIP(),
                        bank_name = transactionModel.Institution,
                        payment_mode = transactionModel.Instrument,
                        pmt_txn_id = MerchantTxnId
                    };
                    _iLoad.UpdateTransaction(lBalance);

                }
            }

            // then check if transaction is already updated by merchanttxnid and gatewaytxnid

            //if transaction exists or already received or txn not found
            //return "Received";


            //
            Response.Write("Received");
            Response.End();
            return View();
        }
        [HttpGet]
        public ActionResult ReceivePaymentResponse(string MerchantTxnId, string GatewayTxnId)
        {


            ewallet.application.Models.OnePG.CommonResponse resp = MakeHttpRequest.InvokeCheckTransactionStatus("1", "anujApi", MerchantTxnId, "anujApi", "Anuj@123", "AnujSecert");
            if (resp.code == "0")
            {

                //check MerchantTxnId in our db,get detail  and print receipt
                var dbResponse = _iLoad.GetTransactionReposne(MerchantTxnId, GatewayTxnId);
                if (dbResponse.Code == 0)
                {

                    var viewTransactionModel = ApplicationUtilities.MapObject<ViewTransactionReponseModel>(dbResponse.Data);
                    return View(viewTransactionModel);
                }
                else
                {
                    return RedirectToAction("Eror");///remaining
                }

            }
            else
                return RedirectToAction("Eror");///remaining

        }
    }
}