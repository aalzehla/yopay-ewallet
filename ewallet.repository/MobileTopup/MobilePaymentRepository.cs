using ewallet.api.Services;
using ewallet.api.Services.contracts.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ewallet.shared.Models;
using ewallet.shared.Models.MobileTopup;
using System.Configuration;

namespace ewallet.repository.MobileTopup
{
    public class MobilePaymentRepository : IMobilePaymentRepository
    {
        /// <summary>
        /// Mobile Topup Service
        /// </summary>
        /// <param name="MobileNumber"></param>
        /// <param name="Amount"></param>
        /// <returns>Extra1=BillNumber,Extra2=RefStan</returns>
        public CommonDbResponse ConsumeService(string MobileNumber, long Amount)
        {
            CommonDbResponse response = new CommonDbResponse();
            if (!Regex.IsMatch(MobileNumber, @"^\d+$"))
            {
                response.Code = ResponseCode.Failed;
                response.Message = "Please input valid Mobile Number";
                return response;
            }

            var checkMobile = MobileNumberValidate(MobileNumber, Amount);
            if (checkMobile.Code != ResponseCode.Success)
            {
                response.Code = ResponseCode.Failed;
                response.Message = checkMobile.Message;
                return response;
            }

            IServiceRepository factory = apiServicesAbstractFactory.Create(checkMobile.ServiceName);
            var package = factory.PACKAGE(checkMobile.MobileNumber, checkMobile.ServiceCode.ToString(),Amount.ToString());
            if (package.Result != "000")
            {
                response.Code = ResponseCode.Failed;
                response.Message = package.ResultMessage;
                return response;
            }
            var payment = factory.PAYMENT(Amount.ToString(), checkMobile.MobileNumber, package.BillInfo.Bill.BillNumber, package.BillInfo.Bill.RefStan, checkMobile.ServiceCode.ToString());
            response.Code = payment.Result != "000" ? ResponseCode.Failed : ResponseCode.Success;
            response.Message = payment.ResultMessage;
            response.Extra1 = package.Result == "000" ? package.BillInfo.Bill.BillNumber : "";
            response.Extra2 = package.Result == "000" ? package.BillInfo.Bill.RefStan : "";
            response.Id = package.TransactionId;
            response.Data = payment;
            return response;
        }
        private CheckMobileNumber MobileNumberValidate(string MobileNumber, long Amount)
        {
            if (MobileNumber.Length > 10 && MobileNumber.Substring(0, 3) == "977")
            {
                MobileNumber = MobileNumber.Substring(3);
            }
            if (!MobileNumberLengthValidate(MobileNumber))
            {
                return new CheckMobileNumber { Code = ResponseCode.Failed, Message = "Please Enter Valid Mobile Number of length 10" };
            }
            if (MobileNumber.Substring(0, 3) == "980" || MobileNumber.Substring(0, 3) == "981" || MobileNumber.Substring(0, 3) == "982" || (ConfigurationManager.AppSettings["phase"] != null && ConfigurationManager.AppSettings["phase"].ToString().ToUpper() == "DEVELOPMENT" && MobileNumber.Substring(0, 3) == "880"))//NCELL
            {
                if (Amount < 10 || Amount > 5000)
                {
                    return new CheckMobileNumber { Code = ResponseCode.Failed, Message = "Please enter amount between 10 and 5000" };
                }
                return new CheckMobileNumber { Code = ResponseCode.Success, ServiceCode = 0, CompanyCode = 78, MobileNumber = MobileNumber, ServiceName = "NCELL" };
            }
            else if (MobileNumber.Substring(0, 3) == "984" || MobileNumber.Substring(0, 3) == "986")//ntc prepaid
            {
                return new CheckMobileNumber { Code = ResponseCode.Success, ServiceCode = 0, CompanyCode = 585, MobileNumber = "977" + MobileNumber, ServiceName = "NTC" };
            }
            else if (MobileNumber.Substring(0, 3) == "974" || MobileNumber.Substring(0, 3) == "976" || MobileNumber.Substring(0, 3) == "975")//ntc cdma
            {
                return new CheckMobileNumber { Code = ResponseCode.Success, ServiceCode = 5, CompanyCode = 585, MobileNumber = "977" + MobileNumber, ServiceName = "NTC" };
            }
            else if (MobileNumber.Substring(0, 3) == "985")//ntc postpaid
            {
                return new CheckMobileNumber { Code = ResponseCode.Success, ServiceCode = 1, CompanyCode = 585, MobileNumber = "977" + MobileNumber, ServiceName = "NTC" };
            }
            else if (MobileNumber.Substring(0, 3) == "988" || MobileNumber.Substring(0, 3) == "961" || MobileNumber.Substring(0, 3) == "962")//smartcell
            {
                ushort? ServiceCode = null;
                if (Amount == 10)
                    ServiceCode = 10;
                else if (Amount == 20)
                    ServiceCode = 0;
                else if (Amount == 50)
                    ServiceCode = 1;
                else if (Amount == 100)
                    ServiceCode = 2;
                else if (Amount == 200)
                    ServiceCode = 3;
                else if (Amount == 500)
                    ServiceCode = 4;
                else if (Amount == 1000)
                    ServiceCode = 5;

                if (ServiceCode == null)
                {
                    return new CheckMobileNumber { Code = ResponseCode.Failed, Message = "Please Enter Valid Amount",ErrorCode=014 };
                }
                return new CheckMobileNumber { Code = ResponseCode.Success, ServiceCode = ServiceCode.Value, CompanyCode = 709, MobileNumber = "977" + MobileNumber, ServiceName = "SMARTCELL_TOPUP" };
            }
            else if (MobileNumber.Substring(0, 3) == "972")//UTL
            {
                ushort? ServiceCode = null;
                if (Amount == 10 || Amount == 20 || Amount == 50 || Amount == 100 || Amount == 250 || Amount == 500 || Amount == 1000)
                    ServiceCode = 0;

                if (ServiceCode == null)
                {
                    return new CheckMobileNumber { Code = ResponseCode.Failed, Message = "Please Enter Valid Amount" };
                }
                return new CheckMobileNumber { Code = ResponseCode.Success, ServiceCode = ServiceCode.Value, CompanyCode = 582, MobileNumber = MobileNumber, ServiceName = "UTL" };
            }
            return new CheckMobileNumber { Code = ResponseCode.Failed, Message = "Please Enter Valid Mobile Number" };
        }
        private bool MobileNumberLengthValidate(string MobileNumber)
        {
            if (Regex.IsMatch(MobileNumber, @"^\d{10}$"))
            {
                return true;
            }
            return false;
        }

        public CommonDbResponse GetPackage(string MobileNumber, long Amount, string servicecode)
        {
            CommonDbResponse response = new CommonDbResponse();
            if (!Regex.IsMatch(MobileNumber, @"^\d+$"))
            {
                response.Code = ResponseCode.Failed;
                response.Message = "Please input valid Mobile Number";
                return response;
            }

            var checkMobile = MobileNumberValidate(MobileNumber, Amount);
            if (checkMobile.Code != ResponseCode.Success)
            {
                response.Code = ResponseCode.Failed;
                response.Message = checkMobile.Message;
                return response;
            }

            IServiceRepository factory = apiServicesAbstractFactory.Create(checkMobile.ServiceName);
            var package = factory.PACKAGE(checkMobile.MobileNumber, servicecode);
            response.Message = package.ResultMessage;
            response.Code = package.Result != "000" ? ResponseCode.Failed : ResponseCode.Success;
            response.Extra1 = package.Result == "000" ? package.BillInfo.Bill.BillNumber : "";
            response.Extra2 = package.Result == "000" ? package.BillInfo.Bill.RefStan : "";
            response.Id = package.TransactionId;
            response.Data = package;
            return response;
        }

        public CommonDbResponse Payment(string MobileNumber, long Amount, string servicecode, string billnumber, string refstan)
        {
            CommonDbResponse response = new CommonDbResponse();
            if (!Regex.IsMatch(MobileNumber, @"^\d+$"))
            {
                response.Code = ResponseCode.Failed;
                response.Message = "Please input valid Mobile Number";
                return response;
            }

            var checkMobile = MobileNumberValidate(MobileNumber, Amount);
            if (checkMobile.Code != ResponseCode.Success)
            {
                response.Code = ResponseCode.Failed;
                response.Message = checkMobile.Message;
                return response;
            }

            IServiceRepository factory = apiServicesAbstractFactory.Create(checkMobile.ServiceName);
            var payment = factory.PAYMENT(Amount.ToString(), checkMobile.MobileNumber, billnumber, refstan, servicecode);
            response.Message = payment.ResultMessage;
            response.Code = payment.Result != "000" ? ResponseCode.Failed : ResponseCode.Success;
            response.Extra1 = payment.Result == "000" ? billnumber : "";
            response.Extra2 = payment.Result == "000" ? refstan : "";
            response.Id = payment.TransactionId;
            response.Data = payment;
            return response;
        }
    }
}
