using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ewallet.application.Library;
using ewallet.application.Models;
using ewallet.business.Common;
using ewallet.business.Merchant;
using ewallet.shared.Models;
using ewallet.shared.Models.Merchant;

namespace ewallet.application.Areas.Admin.Controllers
{
    public class MerchantManagementController : Controller
    {
        private IMerchantBusiness _merchant;
        private string ControllerName = "MerchantManagement";
        ICommonBusiness _ICB;
        public MerchantManagementController(IMerchantBusiness merchant, ICommonBusiness ICB)
        {
            _merchant = merchant;
            _ICB = ICB;
        }
        // GET: Admin/MerchantManagement
        public ActionResult Index()
        {
            List<MerchantModel> lst = _merchant.GetMerchantList().MapObjects<MerchantModel>();
            //Column Creator
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("MerchantName", "Merchant Name");
            param.Add("MerchantOperationType", "Operation Type");
            param.Add("MerchantMobileNumber", "Contact Number");
            param.Add("MerchantCreditLimit", "Credit Limit");
            param.Add("MerchantStatus", "Merchant Status");
            param.Add("Action", "Action");
            ProjectGrid.column = param;
            //Ends
            foreach (var item in lst)
            {
                // item.Action = StaticData.GetActions("Distributor", item.DistributorId.EncryptParameter(), this, "", "", "");
                item.Action = StaticData.GetActions(ControllerName, item.MerchantID.EncryptParameter(), this, "", "", item.MerchantStatus);
                item.MerchantStatus = "<span class='badge badge-" + (item.MerchantStatus.Trim().ToUpper() == "Y" ? "success" : "danger") + "'>" + (item.MerchantStatus.Trim().ToUpper() == "Y" ? "Active" : "Blocked") + "</span>";
            }
            var grid = ProjectGrid.MakeGrid(lst, "Merchant List ", "", 0, true, "", "", "Home", "Merchant", "/Admin/MerchantManagement", "/Admin/MerchantManagement/ManageMerchant");
            ViewData["grid"] = grid;
            return View();
        }
        public ActionResult ManageMerchant(string merchantId = "")
        {
            MerchantModel merchantModel = new MerchantModel();

            if (!string.IsNullOrEmpty(merchantId))
            {
                merchantModel.MerchantID = merchantId.DecryptParameter();
                if (!string.IsNullOrEmpty(merchantModel.MerchantID))
                {
                    merchantModel = _merchant.GetMerchantById(merchantModel.MerchantID).MapObject<MerchantModel>();
                    merchantModel.MerchantID = merchantModel.MerchantID.EncryptParameter();
                    merchantModel.UserID = merchantModel.UserID.EncryptParameter();
                   
                }
            }
            LoadDropDownList(merchantModel);
            return View(merchantModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageMerchant(MerchantModel merchantModel, HttpPostedFileBase Merchant_Logo, HttpPostedFileBase Pan_Certiticate, HttpPostedFileBase Registration_Certificate, string changepassword)
        {
            var Merchant_LogoPath = "";
            var Pan_CertiticatePath = "";
            var Registration_CertificatePath = "";
            LoadDropDownList(merchantModel);
            if (!string.IsNullOrEmpty(merchantModel.MerchantID.DecryptParameter()))
            {
                ModelState.Remove("UserName");
                if (string.IsNullOrEmpty(changepassword))
                {
                    ModelState.Remove("Password");
                    ModelState.Remove("ConfirmPassword");
                }
            }
            if (ModelState.IsValid)
            {
                MerchantCommon merchantCommon = new MerchantCommon();
                merchantCommon = merchantModel.MapObject<MerchantCommon>();
                if (!string.IsNullOrEmpty(merchantCommon.MerchantID))
                {
                    if (string.IsNullOrEmpty(merchantCommon.MerchantID.DecryptParameter()))
                    {
                        return View(merchantModel);
                    }
                    if (string.IsNullOrEmpty(changepassword))
                    {
                        merchantCommon.Password = "";
                        merchantCommon.ConfirmPassword = "";                        
                        merchantCommon.MerchantEmail = "";
                        merchantCommon.MerchantMobileNumber = "";
                    }
                    merchantCommon.MerchantID = merchantCommon.MerchantID.DecryptParameter();
                    merchantCommon.UserID = merchantCommon.UserID.DecryptParameter();
                }
                if (!string.IsNullOrEmpty(merchantCommon.ParentID))
                {
                    if (string.IsNullOrEmpty(merchantCommon.ParentID.DecryptParameter()))
                    {
                        return View(merchantModel);
                    }
                    merchantCommon.ParentID = merchantCommon.ParentID.DecryptParameter();

                }
                merchantCommon.ActionUser = ApplicationUtilities.GetSessionValue("UserName").ToString();
                merchantCommon.IpAddress = ApplicationUtilities.GetIP();

                if (Merchant_Logo != null)
                {
                    var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
                    var fileName = Path.GetFileName(Merchant_Logo.FileName);
                    String timeStamp = DateTime.Now.ToString();
                    var ext = Path.GetExtension(Merchant_Logo.FileName);
                    if (Merchant_Logo.ContentLength > 1 * 1024 * 1024)//1 MB
                    {
                        this.ShowPopup(1, "Image Size must be less than 1MB");
                        return View(merchantModel);
                    }
                    if (allowedExtensions.Contains(ext.ToLower()))
                    {
                        string datet = DateTime.Now.ToString().Replace('/', ' ').Replace(':', ' ');
                        string myfilename = "logo " + datet + "." + Merchant_Logo.FileName;
                        Merchant_LogoPath = Path.Combine(Server.MapPath("~/Content/userupload/merchant"), myfilename);
                        merchantCommon.MerchantLogo = "/Content/userupload/merchant/" + myfilename;
                        
                    }
                    else
                    {
                        this.ShowPopup(1, "File Must be .jpg,.png,.jpeg");
                        return View(merchantModel);
                    }
                }

                if (Pan_Certiticate != null)
                {
                    var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
                    var fileName = Path.GetFileName(Pan_Certiticate.FileName);
                    String timeStamp = DateTime.Now.ToString();
                    var ext = Path.GetExtension(Pan_Certiticate.FileName);
                    if (Pan_Certiticate.ContentLength > 1 * 1024 * 1024)//1 MB
                    {
                        this.ShowPopup(1, "Image Size must be less than 1MB");
                        return View(merchantModel);
                    }
                    if (allowedExtensions.Contains(ext.ToLower()))
                    {
                        string datet = DateTime.Now.ToString().Replace('/', ' ').Replace(':', ' ');
                        string myfilename = "pan " + datet + "." + Pan_Certiticate.FileName;
                        Pan_CertiticatePath = Path.Combine(Server.MapPath("~/Content/userupload/merchant"), myfilename);
                        merchantCommon.MerchantPanCertificate = "/Content/userupload/merchant/" + myfilename;

                    }
                    else
                    {
                        this.ShowPopup(1, "File Must be .jpg,.png,.jpeg");
                        return View(merchantModel);
                    }
                }

                if (Registration_Certificate != null)
                {
                    var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
                    var fileName = Path.GetFileName(Registration_Certificate.FileName);
                    String timeStamp = DateTime.Now.ToString();
                    var ext = Path.GetExtension(Registration_Certificate.FileName);
                    if (Registration_Certificate.ContentLength > 1 * 1024 * 1024)//1 MB
                    {
                        this.ShowPopup(1, "Image Size must be less than 1MB");
                        return View(merchantModel);
                    }
                    if (allowedExtensions.Contains(ext.ToLower()))
                    {
                        string datet = DateTime.Now.ToString().Replace('/', ' ').Replace(':', ' ');
                        string myfilename = "reg " + datet + "." + Registration_Certificate.FileName;
                        Registration_CertificatePath = Path.Combine(Server.MapPath("~/Content/userupload/merchant"), myfilename);                        
                        merchantCommon.MerchantRegistrationCertificate = "/Content/userupload/merchant/" + myfilename;

                        //Registration_Certificate.SaveAs(path);
                    }
                    else
                    {
                        this.ShowPopup(1, "File Must be .jpg,.png,.jpeg");
                        return View(merchantModel);
                    }
                }

                CommonDbResponse dbresp = _merchant.ManageMerchant(merchantCommon);
                if (dbresp.Code == 0)
                {
                    
                        if (Pan_Certiticate != null)
                        {
                            Pan_Certiticate.SaveAs(Pan_CertiticatePath);
                        }
                        if (Registration_Certificate != null)
                        {
                            Registration_Certificate.SaveAs(Registration_CertificatePath);
                        }
                    
                    if (Merchant_Logo != null)
                    {
                        Merchant_Logo.SaveAs(Merchant_LogoPath);
                    }
                    this.ShowPopup(0, dbresp.Message);
                    return RedirectToAction("Index");
                }
                merchantModel.Msg = dbresp.Message;

            }
            this.ShowPopup(1, "Error " + merchantModel.Msg);
            return View(merchantModel);

        }



        public void LoadDropDownList(MerchantModel merchantModel)
        {

            //Manage Distributor
            ViewBag.MerchantCountryList = ApplicationUtilities.SetDDLValue(LoadDropdownList("country"), merchantModel.MerchantCountry, "--Select Country--");
            ViewBag.MerchantProvienceList = ApplicationUtilities.SetDDLValue(LoadDropdownList("province", merchantModel.MerchantCountry), merchantModel.MerchantProvince, "--Select Provience--");
            ViewBag.MerchantDistrictList = ApplicationUtilities.SetDDLValue(LoadDropdownList("districtList", merchantModel.MerchantProvince) as Dictionary<string, string>, merchantModel.MerchantDistrict, "--Select District--");
            ViewBag.MerchantVDC_MuncipilityList = ApplicationUtilities.SetDDLValue(LoadDropdownList("vdc_muncipality", merchantModel.MerchantDistrict), merchantModel.MerchantVDC_Muncipality, "--Select VDC Muncipality--");
            ViewBag.IssueDistrictList = ApplicationUtilities.SetDDLValue(LoadDropdownList("issuedistrict"), merchantModel.ContactPersonIdIssueDistrict, "--Select District--");
            ViewBag.DoctypeList = ApplicationUtilities.SetDDLValue(LoadDropdownList("doctype"), merchantModel.ContactPersonIdType, "--Select Document Type--");

        }
        public Dictionary<string, string> LoadDropdownList(string flag, string search1 = "")
        {
            switch (flag)
            {

                case "country":
                    return _ICB.sproc_get_dropdown_list("004");
                case "gender":
                    return _ICB.sproc_get_dropdown_list("005");
                case "occupation":
                    return _ICB.sproc_get_dropdown_list("024");
                case "doctype":
                    return _ICB.sproc_get_dropdown_list("014");
                case "province":
                    return _ICB.sproc_get_dropdown_list("002");
                case "issuedistrict":
                    return _ICB.sproc_get_dropdown_list("007");
                case "userRole":
                    return _ICB.sproc_get_dropdown_list("035");
                case "districtList":
                    return _ICB.sproc_get_dropdown_list("007", search1);
                case "vdc_muncipality":
                    return _ICB.sproc_get_dropdown_list("008", search1);

            }
            return null;
        }

        [OverrideActionFilters]
        [HttpPost]
        public JsonResult GetDistrictsByProvince(string provinceId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = ApplicationUtilities.SetDDLValue(LoadDropdownList("districtList", provinceId) as Dictionary<string, string>, "");
            return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));
        }
        [HttpPost]
        [OverrideActionFilters]
        public JsonResult GetMuncipalityByDistrict(string district)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = ApplicationUtilities.SetDDLValue(LoadDropdownList("vdc_muncipality", district) as Dictionary<string, string>, "");
            return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

    }
}