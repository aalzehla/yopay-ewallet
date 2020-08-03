using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.shared.Models;
using ewallet.shared.Models.Merchant;

namespace ewallet.repository.Merchant
{
    public class MerchantManagementRepository : IMerchantManagementRepository
    {

        RepositoryDao DAO;
        public MerchantManagementRepository()
        {
            DAO = new RepositoryDao();
        }

        public List<MerchantCommon> GetMerchantList(string MerchantId = "", string parentid = "")
        {
            string sql = "sproc_merchant_detail";
            sql += " @flag='s'";
            sql += " ,@merchant_type='Merchant'";
            sql += " ,@parent_Id=" + DAO.FilterParameter(parentid);
            sql += " ,@agent_id=" + DAO.FilterParameter(MerchantId);
            var dt = DAO.ExecuteDataTable(sql);
            List<MerchantCommon> lst = new List<MerchantCommon>();
            if (dt != null)
            {
                foreach (DataRow item in dt.Rows)
                {
                    MerchantCommon merchantCommon = new MerchantCommon
                    {
                        ParentID = item["parent_id"].ToString(),
                        MerchantID = item["agent_id"].ToString(),
                        MerchantName = item["agent_name"].ToString(),
                        MerchantOperationType = item["agent_operation_type"].ToString(),
                        MerchantStatus = item["agent_status"].ToString(),
                        MerchantCreditLimit = item["agent_credit_limit"].ToString(),
                        MerchantMobileNumber = item["agent_mobile_no"].ToString()
                    };
                    lst.Add(merchantCommon);
                }
            }
            return lst;
        }

        public CommonDbResponse ManageMerchant(MerchantCommon merchantCommon)
        {
            string sql = "sproc_merchant_detail ";
            sql += "@flag='" + (string.IsNullOrEmpty(merchantCommon.MerchantID) ? "i" : "u") + "'";
            sql += " ,@merchant_type='Merchant'";
            sql += " ,@agent_id=" + DAO.FilterString(merchantCommon.MerchantID);
            sql += " ,@merchant_commission_type=" + merchantCommon.MerchantCommissionType;
            sql += " ,@merchant_mobile_no=" + DAO.FilterString(merchantCommon.MerchantMobileNumber);
            sql += " ,@merchant_country=" + DAO.FilterString(merchantCommon.MerchantCountry);
            sql += " ,@merchant_province=" + DAO.FilterString(merchantCommon.MerchantProvince);
            sql += " ,@merchant_district=" + DAO.FilterString(merchantCommon.MerchantDistrict);
            sql += " ,@merchant_local_body=" + DAO.FilterString(merchantCommon.MerchantVDC_Muncipality);
            sql += " ,@merchant_ward_number=" + DAO.FilterString(merchantCommon.MerchantWardNo);
            sql += " ,@merchant_street=" + DAO.FilterString(merchantCommon.MerchantStreet);
            sql += " ,@merchant_available_balance=" + DAO.FilterString(merchantCommon.MerchantBalance);
            sql += " ,@merchant_logo=" + DAO.FilterString(merchantCommon.MerchantLogo);
            //user info
            sql += " ,@user_id=" + DAO.FilterString(merchantCommon.UserID);
            sql += " ,@first_name=" + DAO.FilterString(merchantCommon.FirstName);
            sql += " ,@middle_name=" + DAO.FilterString(merchantCommon.MiddleName);
            sql += " ,@last_name=" + DAO.FilterString(merchantCommon.LastName);
            sql += " ,@action_user=" + DAO.FilterString(merchantCommon.ActionUser);
            sql += " ,@action_ip=" + DAO.FilterString(merchantCommon.IpAddress);
            sql += " ,@action_platform=''";// + DAO.FilterString(merchantCommon.IpAddress);
            sql += " ,@role_id='12'";
            sql += " ,@usr_type='Merchant'";
            sql += " ,@usr_type_id='12'";
            sql += " ,@merchant_phone_number=" + DAO.FilterString(merchantCommon.MerchantPhoneNumber);
            sql += " ,@merchant_email=" + DAO.FilterString(merchantCommon.MerchantEmail);
            sql += " ,@merchant_web_url=" + DAO.FilterString(merchantCommon.MerchantWebUrl);
            sql += " ,@merchant_registration_no=" + DAO.FilterString(merchantCommon.MerchantRegistrationNumber);
            sql += " ,@merchant_Pan_no=" + DAO.FilterString(merchantCommon.MerchantPanNumber);
            sql += " ,@merchant_contract_date=" + DAO.FilterString(merchantCommon.MerchantContractDate);
            sql += " ,@merchant_reg_certificate=" + DAO.FilterString(merchantCommon.MerchantRegistrationCertificate);
            sql += " ,@merchant_pan_Certificate=" + DAO.FilterString(merchantCommon.MerchantPanCertificate);

            if (string.IsNullOrEmpty(merchantCommon.MerchantID))
            {
                sql += " ,@merchant_name=" + DAO.FilterString(merchantCommon.FirstName+" "+ merchantCommon.MiddleName+" "+merchantCommon.LastName);
                sql += " ,@user_name=" + DAO.FilterString(merchantCommon.UserName);
                sql += " ,@password=" + DAO.FilterString(merchantCommon.Password);
                sql += " ,@confirm_password=" + DAO.FilterString(merchantCommon.ConfirmPassword);
                sql += " ,@user_mobile_number=" + DAO.FilterString(merchantCommon.MerchantMobileNumber);
                sql += " ,@user_email=" + DAO.FilterString(merchantCommon.MerchantEmail);
                sql += " ,@parent_id=" + DAO.FilterString(merchantCommon.ParentID);

            }

            return DAO.ParseCommonDbResponse(sql);
        }

        public MerchantCommon GetMerchantById(string MerchantId)
        {
            string sql = "sproc_merchant_detail";
            sql += " @flag='md'";
            sql += ", @agent_id=" + DAO.FilterString(MerchantId);
            var dt = DAO.ExecuteDataRow(sql);
            MerchantCommon merchantCommon = new MerchantCommon();
            if (dt != null)
            {
                merchantCommon.MerchantType = dt["agent_type"].ToString();
                merchantCommon.MerchantID = dt["agent_id"].ToString();
                merchantCommon.ParentID = dt["parent_id"].ToString();
                merchantCommon.MerchantOperationType = dt["agent_operation_type"].ToString();
                string test = dt["is_auto_commission"].ToString();
                merchantCommon.MerchantCommissionType = dt["is_auto_commission"].ToString().ToUpper() == "TRUE" ? true : false;//dt[""].ToString();
                merchantCommon.MerchantName = dt["agent_name"].ToString();
                merchantCommon.MerchantPhoneNumber = dt["agent_phone_no"].ToString();
                merchantCommon.MerchantMobileNumber = dt["agent_mobile_no"].ToString();
                merchantCommon.MerchantEmail = dt["agent_email_address"].ToString();
                merchantCommon.MerchantWebUrl = dt["web_url"].ToString();
                merchantCommon.MerchantRegistrationNumber = dt["agent_registration_no"].ToString();
                merchantCommon.MerchantPanNumber = dt["agent_pan_no"].ToString();
                merchantCommon.MerchantContractDate = dt["agent_contract_local_date"].ToString();
                merchantCommon.MerchantContractDate_BS = dt["agent_contract_nepali_date"].ToString();
                merchantCommon.MerchantCountry = dt["agent_country"].ToString();

                merchantCommon.MerchantProvince = dt["permanent_province"].ToString();
                merchantCommon.MerchantDistrict = dt["permanent_district"].ToString();
                merchantCommon.MerchantVDC_Muncipality = dt["permanent_localbody"].ToString();
                merchantCommon.MerchantWardNo = dt["permanent_wardno"].ToString();
                merchantCommon.MerchantStreet = dt["permanent_address"].ToString();

                merchantCommon.MerchantCreditLimit = dt["agent_credit_limit"].ToString();
                merchantCommon.MerchantBalance = dt["available_balance"].ToString();
                merchantCommon.MerchantLogo = dt["agent_logo_img"].ToString();
                merchantCommon.MerchantPanCertificate = dt["agent_document_img_front"].ToString();
                merchantCommon.MerchantRegistrationCertificate = dt["agent_document_img_back"].ToString();
                merchantCommon.UserID = dt["user_id"].ToString();
                merchantCommon.UserName = dt["user_name"].ToString();
                merchantCommon.FirstName = dt["first_name"].ToString();
                merchantCommon.MiddleName = dt["middle_name"].ToString();
                merchantCommon.LastName = dt["last_name"].ToString();
                merchantCommon.UserMobileNumber = dt["user_mobile_no"].ToString();
                merchantCommon.UserEmail = dt["user_email"].ToString();

            }
            return merchantCommon;
        }

        public CommonDbResponse Disable_EnableMerchant(MerchantCommon merchantCommon)
        {
            string sql = "sproc_merchant_detail ";
            sql += " @flag='edm'";
            sql += ",@agent_id=" + DAO.FilterString(merchantCommon.MerchantID);
            sql += ",@action_user=" + DAO.FilterString(merchantCommon.ActionUser);
            sql += ",@user_status=" + DAO.FilterString(merchantCommon.UserStatus);

            return DAO.ParseCommonDbResponse(sql);
        }
    }
}
