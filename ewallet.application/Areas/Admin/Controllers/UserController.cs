using ewallet.application.Library;
using ewallet.business.User;
using ewallet.business.Common;
using ewallet.shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;

namespace ewallet.application.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        string ControllerName = "User";

        IUserBusiness buss;
        ICommonBusiness ICB;

        public UserController(IUserBusiness _buss, ICommonBusiness _ICB)
        {
            buss = _buss;
            ICB = _ICB;
        }


        public ActionResult Index(string Search = "", int Pagesize = 10)
        {
            string usertype = Session["UserType"].ToString();
            var list = buss.GetAllList(StaticData.GetUser(), usertype, Pagesize);

            foreach (var item in list)
            {
                item.Action = StaticData.GetActions("User", item.UserID.EncryptParameter(), this, "", "", item.IsActive);
                item.ActivityStatus = item.IsActive;
                item.ActivityStatus = "<span class='badge badge-" + (item.IsActive.Trim().ToUpper() == "Y" ? "success" : "danger") + "'>" + (item.IsActive.Trim().ToUpper() == "Y" ? "Active" : "Blocked") + "</span>";
            }

            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("FullName", "Full Name");
            param.Add("Email", "Email");
            param.Add("PhoneNo", "Phone No");
            param.Add("ActivityStatus", "Status");
            param.Add("Action", "Action");

            ProjectGrid.column = param;
            var grid = ProjectGrid.MakeGrid(list, "User", Search, Pagesize, true, "", "", "Home", "User", "/Admin/User", "/Admin/User/ManageUser");
            ViewData["grid"] = grid;
            return View();

        }
        public object LoadDropdownList(string forMethod)
        {
            switch (forMethod)
            {
                case "ManageUser":
                    return ICB.sproc_get_dropdown_list("001");
                case "searchfilter":
                    {
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        dict.Add("MobileNo", "Mobile No");
                        dict.Add("Email", "Email");
                        dict.Add("Username", "User Name");
                        dict.Add("Fullname", "Full Name");

                        return dict;
                    };
            }
            return null;
        }
        public void ModelStateValidation(string validateMode = "Insert")
        {
            switch (validateMode)
            {
                case "Update":
                    ModelState.Remove("UserName");
                    ModelState.Remove("UserPwd");
                    ModelState.Remove("IsActive");
                    ModelState.Remove("ConfirmUserPwd");
                    ModelState.Remove("RoleId");
                    break;
                case "Insert":
                    break;
                default: break;
            }
        }
        public ActionResult ManageUser(string UserId = "")
        {
            UserCommon commonModel = new UserCommon();
            if (!String.IsNullOrEmpty(UserId))
            {
                var id = UserId.DecryptParameter();
                if (string.IsNullOrEmpty(id))
                    return RedirectToAction("Index");
                commonModel = buss.GetUserById(id);
                commonModel.UserID = commonModel.UserID.EncryptParameter();
            }
            var rol = LoadDropdownList("ManageUser");
            ViewBag.Roles = ApplicationUtilities.SetDDLValue(LoadDropdownList("ManageUser") as Dictionary<string, string>, commonModel.RoleId, "--Select Role--");

            //ViewBag.Roles = LoadDropdownList("ManageUser");
            return View(commonModel);
        }
        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult ManageUser(UserCommon model)
        {
            ViewBag.Roles = ApplicationUtilities.SetDDLValue(LoadDropdownList("ManageUser") as Dictionary<string, string>, model.RoleId, "--Select Role--");

            //model.Roles = LoadDropdownList("ManageUser") as List<SelectListItem>;
            string userId = "";
            userId = model.UserID;
            if (!string.IsNullOrEmpty(model.UserID))
            {
                if (string.IsNullOrEmpty(model.UserID.DecryptParameter()))
                {
                    return RedirectToAction("Index");
                }

                model.UserID = userId.DecryptParameter();
            }
            ModelStateValidation(String.IsNullOrEmpty(userId) ? "Insert" : "Update");
            if (ModelState.IsValid)
            {
                model.ActionUser = Session["username"].ToString();
                buss.ManageUser(model).SetMessageInTempData(this);
                return RedirectToAction("Index");
            }
            model.UserID = userId;
            return View(model);
        }

        public ActionResult SearchUser()
        {
            ViewBag.SearchFilter = ApplicationUtilities.SetDDLValue(LoadDropdownList("searchfilter") as Dictionary<string, string>, "", "--Select--");

            return View();
        }
        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult SearchUser(UserCommon UC)
        {
            ViewBag.SearchFilter = ApplicationUtilities.SetDDLValue(LoadDropdownList("searchfilter") as Dictionary<string, string>, UC.SearchFilter, "--Select--");

            if (!string.IsNullOrEmpty(UC.SearchField) || !string.IsNullOrEmpty(UC.SearchFilter))
            {
                string username = Session["username"].ToString();
                var lst = buss.GetSearchUserList(UC.SearchField, UC.SearchFilter, username);

                foreach (var item in lst)
                {
                    item.Status = "<span class='badge badge-" + (item.IsActive.Trim().ToUpper() == "Y" ? "success" : "danger") + "'>" + (item.IsActive.Trim().ToUpper() == "Y" ? "Active" : "Blocked") + "</span>";

                }
                IDictionary<string, string> param = new Dictionary<string, string>();

                param.Add("AgentUserId", "Agent Id");
                param.Add("FullName", "Full Name");
                param.Add("UserName", "User Name");
                param.Add("Email", "Email");
                param.Add("Status", "Status");
                param.Add("PhoneNo", "Mobile Number");
                param.Add("CreatedBy", "Created By");
                param.Add("CreateDate", "Created On");
                ProjectGrid.column = param;
                //Ends
                var grid = ProjectGrid.MakeGrid(lst, "hidebreadcrumb", "", 10, false, "", "", "", "", "", "");
                ViewData["grid"] = grid;
            }
            else
                this.ShowPopup(1, "Please Fill the fields");
            return View(UC);
        }


        public ActionResult changepassword()
        {
           
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult changepassword(UserCommon uc)
        {
            string dbmessage = string.Empty;
            ModelState.Remove("RoleId");
            ModelState.Remove("IsActive");
            ModelState.Remove("PhoneNo");
            ModelState.Remove("Email");
            ModelState.Remove("FullName");
            ModelState.Remove("UserName");
            if (string.IsNullOrEmpty(uc.OldPassword))
            {
                ModelState.AddModelError("OldPassword", "Current Password is Required");

            }
            if (ModelState.IsValid)
            {
                string oldpwd = uc.OldPassword;
                string newpwd = uc.UserPwd;
                string username = Session["username"].ToString();
                UserCommon user = new UserCommon
                {
                    OldPassword = oldpwd,
                    UserName = username,
                    UserPwd = newpwd

                };



                CommonDbResponse dbresp = buss.ChangePassword(user);
                if (dbresp.Code == 0)
                {
                    this.ShowPopup(0, dbresp.Message);
                    return RedirectToAction("Index","home");
                }
                dbmessage = dbresp.Message;

            }
            this.ShowPopup(1, string.IsNullOrEmpty(dbmessage) ? "Error" : dbmessage);
            return View(uc);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult BlockUser(string userid)
        {
            if (!String.IsNullOrEmpty(userid))
            {
                userid = userid.DecryptParameter();
                if (string.IsNullOrEmpty(userid))
                {
                    return Json(new CommonDbResponse { ErrorCode = 1, Message = "Invalid User." });
                }
                var DbResponse = buss.block_unblockuser(userid, "N");
                if (DbResponse.ErrorCode == 0)
                {
                    DbResponse.Message = "Successfully Blocked User";
                    DbResponse.SetMessageInTempData(this);

                }
                return Json(DbResponse);
            }
            return Json(new CommonDbResponse { ErrorCode = 1, Message = "Invalid User." });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UnBlockUser(string userid)
        {
            if (!String.IsNullOrEmpty(userid))
            {
                userid = userid.DecryptParameter();
                if (string.IsNullOrEmpty(userid))
                {
                    return Json(new CommonDbResponse { ErrorCode = 1, Message = "Invalid User." });
                }
                var DbResponse = buss.block_unblockuser(userid, "Y");
                if (DbResponse.ErrorCode == 0)
                {
                    DbResponse.Message = "Successfully Un-Blocked User";
                    DbResponse.SetMessageInTempData(this);

                }
                return Json(DbResponse);
            }
            return Json(new CommonDbResponse { ErrorCode = 1, Message = "Invalid User." });
        }

        public ActionResult Profile()
        {            
            string UserId = Session["UserName"].ToString();
            Profile walletUser = buss.UserInfo(UserId);
            return View(walletUser);
        }

    }
}