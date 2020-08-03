using ewallet.application.Models.OnePG;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace ewallet.application.Library
{
    public static class MakeHttpRequest
    {
        public static CommonResponse InvokeGetProcessId(string MerchantId,string MerchantName,string MerchantTxnId,string Amount,string TransactionRemarks,string ApiUsername,string ApiPassword,string ApiSecretKey)
        {
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(ApiUsername, ApiPassword);
                var postValues = new NameValueCollection();
                postValues["MerchantId"] = MerchantId;
                postValues["MerchantName"] = MerchantName;
                postValues["Amount"] = Amount.ToString();
                postValues["MerchantTxnId"] = MerchantTxnId;
                postValues["Signature"] = HMACSignatureGenerator.SHA512_ComputeHash(CommonFunctions.SingnatureGenerator<AuthenticationLogRequest>(new AuthenticationLogRequest { Amount=Amount,MerchantId=MerchantId,MerchantName=MerchantName,MerchantTxnId=MerchantTxnId,TransactionRemarks=TransactionRemarks}), ApiSecretKey);
                var response = client.UploadValues("https://apisandbox.nepalpayment.com/GetProcessId", "Post", postValues);
                var responseString = Encoding.Default.GetString(response);
                var responseModel = responseString.SerializeJSON<CommonResponse>();
                if (responseModel.code == "0")
                {
                    ProcessResponse pRes = new ProcessResponse()
                    {
                        MerchantId = MerchantId,
                        MerchantTxnId = MerchantTxnId,
                        Amount = Amount,
                        ProcessId = responseModel.data.ToString().SerializeJSON<ProcessIdResponse>().ProcessId,
                        GatewayUrl = CommonFunctions.Getwayurl(),
                        GatewayFormMethod = "Post"
                    };
                    responseModel.data = pRes;
                    return responseModel;
                }
                //var s = responseModel.data.ToString();
                ////var s1 = s.Split(new char [] { ':'});
                //var getProcessId = responseModel.data.ToString().SerializeJSON<ProcessIdResponse>();
                //if (getProcessId != null)
                //{
                //    return getProcessId.ProcessId;
                //}
                return responseModel;

            }
        }
        public static CommonResponse InvokeCheckTransactionStatus(string MerchantId, string MerchantName, string MerchantTxnId, string ApiUsername, string ApiPassword, string ApiSecretKey)
        {
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(ApiUsername, ApiPassword);
                var postValues = new NameValueCollection();
                postValues["MerchantId"] = MerchantId;
                postValues["MerchantName"] = MerchantName;
                postValues["MerchantTxnId"] = MerchantTxnId;
                postValues["Signature"] = HMACSignatureGenerator.SHA512_ComputeHash(CommonFunctions.SingnatureGenerator<AuthenticationLogRequest>(new AuthenticationLogRequest { MerchantId = MerchantId, MerchantName = MerchantName, MerchantTxnId = MerchantTxnId }), ApiSecretKey);
                var response = client.UploadValues("https://apisandbox.nepalpayment.com/CheckTransactionStatus", "Post", postValues);
                var responseString = Encoding.Default.GetString(response);
                var responseModel = responseString.SerializeJSON<CommonResponse>();
                
                //CheckTransactionResponse
                //var s = responseModel.data.ToString();
                ////var s1 = s.Split(new char [] { ':'});
                //var getProcessId = responseModel.data.ToString().SerializeJSON<ProcessIdResponse>();
                //if (getProcessId != null)
                //{
                //    return getProcessId.ProcessId;
                //}
                return responseModel;

            }
        }
    }
}