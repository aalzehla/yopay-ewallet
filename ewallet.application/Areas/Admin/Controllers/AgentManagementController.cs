using ewallet.application.Library;
using ewallet.application.Models;
using ewallet.business.AgentManagement;
using ewallet.business.Common;
using ewallet.shared.Models;
using ewallet.shared.Models.AgentManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ewallet.application.Areas.Admin.Controllers
{
    public class AgentManagementController : Controller
    {
        // GET: Admin/AgentManagement
        IAgentManagementBusiness buss;
        ICommonBusiness ICB;
        public AgentManagementController(IAgentManagementBusiness _buss, ICommonBusiness _ICB)
        {
            buss = _buss;
            ICB = _ICB;

        }
        
        public ActionResult Index(string AgentId = "", string parent_id = "")
        {
            
            var UserType = Session["UserType"].ToString();
            string IsPrimary = ApplicationUtilities.GetSessionValue("IsPrimaryUser").ToString();
            if(!string.IsNullOrEmpty(AgentId))
            {
                AgentId = AgentId.DecryptParameter();                
            }
            if (UserType.ToUpper() == "SUB-AGENT")
            {
                return RedirectToAction("Index", "SubAgentManagement");
            }
            else if (UserType.ToUpper() == "AGENT")
            {
                AgentId = Session["AgentId"].ToString();
            }
            if(!string.IsNullOrEmpty(parent_id) && !string.IsNullOrEmpty(parent_id.DecryptParameter()))
            {
                parent_id = parent_id.DecryptParameter();
            }
            else
            {
                parent_id = Session["AgentId"].ToString();
            }
          
            string username = ApplicationUtilities.GetSessionValue("UserName").ToString();

            var AgentCommon = buss.GetAgentList(AgentId, username, parent_id);
            //Column Creator
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("AgentName", "Agent Name");
            param.Add("AgentOperationType", "Operation Type");
            param.Add("AgentMobileNumber", "Contact Number");
            param.Add("AgentCreditLimit", "Credit Limit");
            param.Add("AgentStatus", "Agent Status");
            param.Add("Action", "Action");
            ProjectGrid.column = param;
            //Ends
            foreach (var item in AgentCommon)
            {
                // item.Action = StaticData.GetActions("Distributor", item.DistributorId.EncryptParameter(), this, "", "", "");
                item.Action = StaticData.GetActions("AgentManagement", item.AgentID.EncryptParameter(), this, "", "", username.EncryptParameter(),item.AgentStatus);
                item.AgentStatus = "<span class='badge badge-" + (item.AgentStatus.Trim().ToUpper() == "Y" ? "success" : "danger") + "'>" + (item.AgentStatus.Trim().ToUpper() == "Y" ? "Active" : "Blocked") + "</span>";
            }
            var grid = ProjectGrid.MakeGrid(AgentCommon, "Agent List", "", 0, true, "", "", "Home", "Agent", "/Admin/AgentManagement", String.IsNullOrEmpty(IsPrimary) == false && IsPrimary.ToUpper().Trim() == "Y" ? "/Admin/AgentManagement/ManageAgent?ParentId="+parent_id.EncryptParameter() : "");
            ViewData["grid"] = grid;
            return View();
        }

        public ActionResult ManageAgent(string User_Name, string agentId = "",string ParentId="")
        {
            AgentManagementModel AM = new AgentManagementModel();
            if (!string.IsNullOrEmpty(ParentId) && string.IsNullOrEmpty(ParentId.DecryptParameter()))
            {
                return RedirectToAction("Index", new { parent_id = ParentId });
            }
            else
            {
                AM.ParentID = ParentId;
            }
            if (!string.IsNullOrEmpty(agentId))
            {
                AM.AgentID = agentId.DecryptParameter();
                if (!string.IsNullOrEmpty(AM.AgentID))
                {
                    AgentManagementCommon AC = buss.GetAgentById(AM.AgentID, User_Name.DecryptParameter());
                    AM = AC.MapObject<AgentManagementModel>();
                    AM.AgentID = AM.AgentID.EncryptParameter();
                    AM.UserID = AM.UserID.EncryptParameter();
                    AM.ParentID = AM.ParentID.EncryptParameter();
                    RemoveupdateValidation(AM);
                }
            }
            LoadDropDownList(AM);
            return View(AM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageAgent(AgentManagementModel agentModel, HttpPostedFileBase Agent_Logo, HttpPostedFileBase Pan_Certiticate, HttpPostedFileBase Registration_Certificate,string changepassword="")
        {
            var Agent_LogoPath = "";
            var Pan_CertiticatePath = "";
            var Registration_CertificatePath = "";
            ModelState.Remove("AgentContractDate_BS");
            LoadDropDownList(agentModel);
            if(!string.IsNullOrEmpty(agentModel.AgentID))
            {
               agentModel.AgentID = agentModel.AgentID.DecryptParameter();
                ModelState.Remove("userName");
                if (changepassword.ToUpper() != "ON")
                {
                    RemoveupdateValidation(agentModel);
                    agentModel.Password = "";
                    agentModel.ConfirmPassword = "";
                }
            }
            if(agentModel.AgentOperationType.ToUpper()!="BUSINESS")
            {
                agentModel.AgentMobileNumber = agentModel.UserMobileNumber;
                agentModel.AgentEmail = agentModel.UserEmail;
                RemoveBusinessValidation(agentModel);
            }
            if (ModelState.IsValid)
            {
                AgentManagementCommon AC = new AgentManagementCommon();
                AC = agentModel.MapObject<AgentManagementCommon>();
                if (!string.IsNullOrEmpty(AC.AgentID))
                {
                    if (string.IsNullOrEmpty(AC.AgentID))
                    {
                        return View(agentModel);
                    }
                    //if (string.IsNullOrEmpty(changepassword))
                    //{
                    //    AC.Password = "";
                    //    AC.ConfirmPassword = "";
                    //}
                    AC.AgentID = AC.AgentID;//.DecryptParameter();
                    AC.UserID = AC.UserID.DecryptParameter();
                }
                if (!string.IsNullOrEmpty(AC.ParentID))
                {
                    if (string.IsNullOrEmpty(AC.ParentID.DecryptParameter()))
                    {
                        return View(agentModel);
                    }
                    AC.ParentID = AC.ParentID.DecryptParameter();

                }           

                AC.ActionUser = ApplicationUtilities.GetSessionValue("UserName").ToString();
                AC.IpAddress = ApplicationUtilities.GetIP();

                if (Agent_Logo != null)
                {
                    var contentType=Agent_Logo.ContentType;
                    var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
                    var fileName = Path.GetFileName(Agent_Logo.FileName);
                    String timeStamp = DateTime.Now.ToString();
                    var ext = Path.GetExtension(Agent_Logo.FileName);
                    if (Agent_Logo.ContentLength > 1 * 1024 * 1024)//1 MB
                    {
                        this.ShowPopup(1, "Image Size must be less than 1MB");
                        return View(agentModel);
                    }
                    if (allowedExtensions.Contains(ext.ToLower()))
                    {
                        string datet = DateTime.Now.ToString().Replace('/', ' ').Replace(':', ' ');
                        string myfilename = "logo " + datet + ext.ToLower();
                        Agent_LogoPath = Path.Combine(Server.MapPath("~/Content/userupload/Agent"), myfilename);
                        AC.AgentLogo = "/Content/userupload/Agent/" + myfilename;
                    }
                    else
                    {
                        this.ShowPopup(1, "File Must be .jpg,.png,.jpeg");
                        return View(agentModel);
                    }
                }
                if (AC.AgentOperationType.ToUpper() == "BUSINESS")
                {
                    if (Pan_Certiticate != null)
                    {
                        var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
                        var fileName = Path.GetFileName(Pan_Certiticate.FileName);
                        String timeStamp = DateTime.Now.ToString();
                        var ext = Path.GetExtension(Pan_Certiticate.FileName);
                        if (Pan_Certiticate.ContentLength > 1 * 1024 * 1024)//1 MB
                        {
                            this.ShowPopup(1, "Image Size must be less than 1MB");
                            return View(agentModel);
                        }
                        if (allowedExtensions.Contains(ext.ToLower()))
                        {
                            string datet = DateTime.Now.ToString().Replace('/', ' ').Replace(':', ' ');
                            string myfilename = "pan " + datet + ext.ToLower();
                            Pan_CertiticatePath = Path.Combine(Server.MapPath("~/Content/userupload/Agent"), myfilename);
                            AC.AgentPanCertificate = "/Content/userupload/Agent/" + myfilename;
                        }
                        else
                        {
                            this.ShowPopup(1, "File Must be .jpg,.png,.jpeg");
                            return View(agentModel);
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
                            return View(agentModel);
                        }
                        if (allowedExtensions.Contains(ext.ToLower()))
                        {
                            string datet = DateTime.Now.ToString().Replace('/', ' ').Replace(':', ' ');
                            string myfilename = "reg" + datet + ext.ToLower();
                            Registration_CertificatePath = Path.Combine(Server.MapPath("~/Content/userupload/Agent"), myfilename);
                            AC.AgentRegistrationCertificate = "/Content/userupload/Agent/"+myfilename;
                            //Registration_Certificate.SaveAs(path);

                        }
                        else
                        {
                            this.ShowPopup(1, "File Must be .jpg,.png,.jpeg");
                            return View(agentModel);
                        }
                    }
                }
                CommonDbResponse dbresp = buss.ManageAgent(AC);
                if (dbresp.Code == 0)
                {
                    if (AC.AgentOperationType.ToUpper() == "BUSINESS")
                    {
                        if (Pan_Certiticate != null)
                        {
                            Pan_Certiticate.SaveAs(Pan_CertiticatePath);
                        }
                        if (Registration_Certificate != null)
                        {
                            Registration_Certificate.SaveAs(Registration_CertificatePath);
                        }
                    }
                    if (Agent_Logo != null)
                    {
                        Agent_Logo.SaveAs(Agent_LogoPath);
                    }
                    this.ShowPopup(0, dbresp.Message);
                    return RedirectToAction("Index",new { parent_id = agentModel.ParentID });
                }
                agentModel.Msg = dbresp.Message;

            }
            this.ShowPopup(1, "Error " + agentModel.Msg);
            return View(agentModel);

        }

        public ActionResult ViewAgentWalletUser(string AgentId = "")
        {
            var UserType = Session["UserType"].ToString();
            string id = "", IsPrimary = ApplicationUtilities.GetSessionValue("IsPrimaryUser").ToString().Trim();
            if (UserType.ToUpper() == "SUB-DISTRIBUTOR")
            {
                return RedirectToAction("Index", "SubDistributor", new { DistId = Session["AgentId"].ToString() });
            }
            else if (UserType.ToUpper() == "DISTRIBUTOR")
            {
                id = Session["AgentId"].ToString();
            }
            else
            {
                id = AgentId.DecryptParameter();
            }
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            var userId = "";
            if (String.IsNullOrEmpty(IsPrimary) == false && (IsPrimary.ToUpper().Trim() == "N" || IsPrimary.ToUpper().Trim() == ""))
            {
                userId = Session["UserId"].ToString();
            }
            //var WalletUser = buss.GetWalletUserList(id, userId);
            dynamic WalletUser = null;

            //Actions
            foreach (var item in WalletUser)
            {
                item.Action = StaticData.GetActions("ViewDistributorUser", item.UserId.EncryptParameter(), this, "", "", item.AgentID.EncryptParameter(), item.UserStatus, item.isPrimary, DisableAddEdit: Session["UserId"].ToString() == item.UserId);
            }
            //Column Creator
            IDictionary<string, string> param = new Dictionary<string, string>();
            //param.Add("DistributorId", "Agent Id");
            param.Add("UserFullName", "Fullname");
            param.Add("UserName", "Username");
            param.Add("UserEmail", "Email");
            param.Add("UserMobileNo", "Mobile No");
            param.Add("UserType", "User Type");
            param.Add("isPrimary", "Is primary");
            param.Add("UserStatus", "Status");
            param.Add("Action", "Action");
            ProjectGrid.column = param;
            //Ends
            //Add New
            var grid = ProjectGrid.MakeGrid(WalletUser, "Distributor Users", "", 0, true, "", "", "Home", "Distributor", "/Admin/Distributor", String.IsNullOrEmpty(IsPrimary) == false && IsPrimary.ToUpper().Trim() == "Y" ? "/Admin/Distributor/ManageDistributorUsers?distid=" + id.EncryptParameter() : "");
            ViewData["grid"] = grid;
            return View();
        }

        public void LoadDropDownList(AgentManagementModel agentmodel)
        {

            //Manage Distributor
            ViewBag.AgentCountryList = ApplicationUtilities.SetDDLValue(LoadDropdownList("country"), agentmodel.AgentCountry, "--select Country--");
            ViewBag.AgentProvinceList = ApplicationUtilities.SetDDLValue(LoadDropdownList("province", agentmodel.AgentCountry), agentmodel.AgentProvince, "--select Province--");
            ViewBag.AgentDistrictList = ApplicationUtilities.SetDDLValue(LoadDropdownList("districtList", agentmodel.AgentProvince) as Dictionary<string, string>, agentmodel.AgentDistrict, "--select District--");
            ViewBag.AgentVDC_MuncipilityList = ApplicationUtilities.SetDDLValue(LoadDropdownList("vdc_muncipality", agentmodel.AgentDistrict), agentmodel.AgentVDC_Muncipality, "--select VDC Muncipality--");
            ViewBag.IssueDistrictList = ApplicationUtilities.SetDDLValue(LoadDropdownList("issuedistrict"), agentmodel.ContactPersonIdIssueDistrict, "--Select District--");
            ViewBag.DoctypeList = ApplicationUtilities.SetDDLValue(LoadDropdownList("doctype"), agentmodel.ContactPersonIdType, "--Select Document Type--");

        }
        public Dictionary<string, string> LoadDropdownList(string flag, string search1 = "")
        {
            switch (flag)
            {

                case "country":
                    return ICB.sproc_get_dropdown_list("004");
                case "gender":
                    return ICB.sproc_get_dropdown_list("005");
                case "occupation":
                    return ICB.sproc_get_dropdown_list("024");
                case "doctype":
                    return ICB.sproc_get_dropdown_list("014");
                case "province":
                    return ICB.sproc_get_dropdown_list("002");
                case "issuedistrict":
                    return ICB.sproc_get_dropdown_list("007");
                case "userRole":
                    return ICB.sproc_get_dropdown_list("035");
                case "districtList":
                    return ICB.sproc_get_dropdown_list("007", search1);
                case "vdc_muncipality":
                    return ICB.sproc_get_dropdown_list("008", search1);

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

        
        public JsonResult ExtendCreditLimit(string agentid)
        {
            AgentManagementCommon amc = new AgentManagementCommon();
            AgentCreditLimitModel aclm = new AgentCreditLimitModel();
            if(!string.IsNullOrEmpty(agentid))
            {
                string agent_id = agentid.DecryptParameter();
                if(!string.IsNullOrEmpty(agent_id))
                {
                    string username=ApplicationUtilities.GetSessionValue("UserName").ToString();
                    amc = buss.GetAgentById(agent_id, username);
                    aclm.AgentId = amc.AgentID.EncryptParameter();
                    aclm.AgentName = amc.AgentName;
                    aclm.AgentCurrentCreditLimit = amc.AgentCreditLimit;

                    string value = string.Empty;
                    value = JsonConvert.SerializeObject(aclm, Formatting.Indented, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    return Json(value, JsonRequestBehavior.AllowGet);
                }
            }
            this.ShowPopup(1, "Error");
            return null;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void ExtendCreditLimit (AgentCreditLimitModel model)
        {
            if (ModelState.IsValid)
            {
                AgentCreditLimitCommon clc = new AgentCreditLimitCommon();
                clc = model.MapObject<AgentCreditLimitCommon>();
                clc.AgentId = clc.AgentId.DecryptParameter();
                clc.ActionUser = ApplicationUtilities.GetSessionValue("UserName").ToString();
                clc.IpAddress = ApplicationUtilities.GetIP();
                CommonDbResponse dbresp = buss.ExtendCreditLimit(clc);
                if (dbresp.Code == 0)
                {
                    this.ShowPopup(0, dbresp.Message);
                    return;
                }
                this.ShowPopup(1, dbresp.Message);
                return;
            }
            this.ShowPopup(1, "Credit Limit Not Changed");
            return;
        }
        public JsonResult EnableAgent(string AgentId)
        {
            if(!string.IsNullOrEmpty(AgentId))
            {
                if(!string.IsNullOrEmpty(AgentId.DecryptParameter()))
                {
                    AgentManagementCommon amc = new AgentManagementCommon();
                    amc.AgentID = AgentId.DecryptParameter();
                    amc.IpAddress = ApplicationUtilities.GetIP();
                    amc.ActionUser = ApplicationUtilities.GetSessionValue("username").ToString();
                    amc.UserStatus = "Y";
                    CommonDbResponse dbresp = buss.Disable_EnableAgent(amc);
                    if (dbresp.ErrorCode == 0)
                    {
                        dbresp.Message = "Successfully Un-Blocked Agent";
                        dbresp.SetMessageInTempData(this);

                    }
                    return Json(dbresp);
                }

            }
            return Json(new CommonDbResponse { ErrorCode = 1, Message = "Invalid Agent." });
        }
        public JsonResult DisableAgent(string AgentId)
        {
            if (!string.IsNullOrEmpty(AgentId))
            {
                if (!string.IsNullOrEmpty(AgentId.DecryptParameter()))
                {
                    AgentManagementCommon amc = new AgentManagementCommon();
                    amc.AgentID = AgentId.DecryptParameter();
                    amc.IpAddress = ApplicationUtilities.GetIP();
                    amc.ActionUser = ApplicationUtilities.GetSessionValue("username").ToString();
                    amc.UserStatus = "N";
                    CommonDbResponse dbresp = buss.Disable_EnableAgent(amc);
                    if (dbresp.ErrorCode == 0)
                    {
                        dbresp.Message = "Successfully Blocked Agent";
                        dbresp.SetMessageInTempData(this);

                    }
                    return Json(dbresp);
                }

            }
            return Json(new CommonDbResponse { ErrorCode = 1, Message = "Invalid Agent." });
        }

        public void RemoveBusinessValidation(AgentManagementModel AMM)
        {
            ModelState.Remove("AgentPhoneNumber"); 
            ModelState.Remove("AgentMobileNumber"); 
            ModelState.Remove("AgentEmail");
            ModelState.Remove("AgentWebUrl");
            ModelState.Remove("AgentRegistrationNumber");
            ModelState.Remove("AgentPanNumber");
            ModelState.Remove("AgentContractDate");
            ModelState.Remove("AgentContractDate_BS");
            ModelState.Remove("AgentRegistrationCertificate");
            ModelState.Remove("AgentPanCertificate");
            ModelState.Remove("ContactPersonName");
            ModelState.Remove("ContactPersonMobileNumber");
            ModelState.Remove("ContactPersonIdType");
            ModelState.Remove("ContactPersonIdNumber");
            ModelState.Remove("ContactPersonIdIssueCountry");
            ModelState.Remove("ContactPersonIdIssueDistrict");
            ModelState.Remove("ContactPersonIdIssueDate");
            ModelState.Remove("ContactPersonIdIssueDate_BS");
            ModelState.Remove("ContactPersonIdExpiryDate");
            ModelState.Remove("ContactPersonIdExpiryDate_BS");
            AMM.AgentPhoneNumber = "";
            AMM.AgentMobileNumber = "";
            AMM.AgentEmail = "";
            AMM.AgentWebUrl = "";
            AMM.AgentRegistrationNumber = "";
            AMM.AgentPanNumber = "";
            AMM.AgentContractDate = "";
            AMM.AgentContractDate_BS = "";
            AMM.ContactPersonName = "";
            AMM.ContactPersonMobileNumber = "";
            AMM.ContactPersonIdType = "";
            AMM.ContactPersonIdNumber = "";
            AMM.ContactPersonIdIssueCountry = "";
            AMM.ContactPersonIdIssueDistrict = "";
            AMM.ContactPersonIdIssueDate = "";
            AMM.ContactPersonIdIssueDate_BS = "";
            AMM.ContactPersonIdExpiryDate = "";
            AMM.ContactPersonIdExpiryDate_BS = "";
        }
        public void RemoveupdateValidation(AgentManagementModel AMM)
        {
            ModelState.Remove("Password");
            ModelState.Remove("confirmPassword");           
        }

    }
}