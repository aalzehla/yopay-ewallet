using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ewallet.business.Services;
using ewallet.application.Models;
using ewallet.shared.Models.Services;
using ewallet.application.Library;
using ewallet.shared.Models;
using ewallet.application.Filters;
using System.IO;
using ewallet.business.Common;

namespace ewallet.application.Areas.Admin.Controllers
{
    [SessionExpiryFilterAttribute]
    public class ServicesController : Controller
    {
        

        string ControllerName = "Services";
        IServicesManagementBusiness _services;
        public ServicesController(IServicesManagementBusiness services,ICommonBusiness ICB)
        {
            _services = services;
            
        }
        // GET: Admin/Services
        public ActionResult Index(string Search = "", int Pagesize = 20)
        {
            List<ServicesModel> lst = _services.GetServicesList().MapObjects<ServicesModel>();
            foreach (var item in lst)
            {

                item.Action = StaticData.GetActions("Services", item.ProductId.ToString().EncryptParameter(), this, "", "", "");
                item.ProductLogo = "<img src='/Content/assets/images/service logos/" + item.ProductLogo + "' height='50' width='50' />";
                item.Status = "<span class='badge badge-" + (item.Status.Trim().ToUpper() == "Y" ? "success" : "danger") + "'>" + ( item.Status.Trim().ToUpper() == "Y" ? "Active" : "Blocked") + "</span>";
            }

            IDictionary<string, string> param = new Dictionary<string, string>();
            //param.Add("ProductId", "Product Id");
            param.Add("ProductLogo", "Product Logo");
            param.Add("TransactionType", "Transaction Type");
            param.Add("Company", "Company");
            param.Add("ProductLabel", "Product Label");
            param.Add("ProductType", "Product Type");
            param.Add("ProductCategory", "Product Category");           
            param.Add("Status", "Status");
            param.Add("Action", "Action");
           

            ProjectGrid.column = param;
            var grid = ProjectGrid.MakeGrid(lst, "Services", Search, Pagesize, true, "", "", "Home", "Services", "/Admin/Services", "/Admin/Services/ManageServices");
            ViewData["grid"] = grid;
            return View();


        }

        [HttpGet]
        public ActionResult ManageServices(string id="")
        {
            ServicesCommon servicesCommon = new ServicesCommon();
            var productid = id.DecryptParameter();
            if (!String.IsNullOrEmpty(productid))
            {               
                //return RedirectToAction("Index");
                servicesCommon = _services.GetServicesByProductId(Int32.Parse(productid));
                servicesCommon.ProductId = servicesCommon.ProductId.EncryptParameter();
            }
            

            servicesCommon.StatusList = LoadDropdownList("status") as List<SelectListItem>;
            servicesCommon.CompanyList = ApplicationUtilities.SetDDLValue(LoadDropdownList("company") as Dictionary<string, string>, servicesCommon.Company, "Select Company");
            servicesCommon.TransactionTypeList = ApplicationUtilities.SetDDLValue(LoadDropdownList("txntype") as Dictionary<string, string>, servicesCommon.TransactionType, "Select Transaction Type");
            servicesCommon.ProductTypeList = ApplicationUtilities.SetDDLValue(LoadDropdownList("producttype") as Dictionary<string, string>, servicesCommon.ProductType, "Select Product Type");
            servicesCommon.ProductCategoryList = ApplicationUtilities.SetDDLValue(LoadDropdownList("productcategory") as Dictionary<string, string>, servicesCommon.ProductCategory, "Select Product Category");
            servicesCommon.PrimaryGatewayList = ApplicationUtilities.SetDDLValue(LoadDropdownList("primarygateway") as Dictionary<string, string>, servicesCommon.PrimaryGateway, "Select Primary Gateway");
            servicesCommon.SecondaryGatewayList = ApplicationUtilities.SetDDLValue(LoadDropdownList("secondarygateway") as Dictionary<string, string>, servicesCommon.SecondaryGateway, "Select Secondary Gateway");


            ServicesModel lst = servicesCommon.MapObject<ServicesModel>();
            return View(lst);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageServices(ServicesModel SM, HttpPostedFileBase file)
        {
            string productid = "";
            if (ModelState.IsValid)
            {
                string username = Session["UserName"].ToString();
                SM.StatusList = LoadDropdownList("status") as List<SelectListItem>;
                
                productid = SM.ProductId;
                if (!string.IsNullOrEmpty(productid))
                {
                    if (string.IsNullOrEmpty(productid.DecryptParameter()))
                    {
                        return RedirectToAction("ManageServices", SM);
                    }
                    SM.ProductId = productid.DecryptParameter();
                    SM.Status = "";

                }
                var path = "";
                #region "logo"
                if (file != null)
                {
                    var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
                    var fileName = Path.GetFileName(file.FileName);
                    String timeStamp = DateTime.Now.ToString();
                    var ext = Path.GetExtension(file.FileName);
                    if (file.ContentLength > 1 * 1024 * 1024)//1 MB
                    {
                        this.ShowPopup(1, "Image Size must be less than 1MB");
                        return RedirectToAction("ManageServices", ControllerName, productid);
                    }
                    if (allowedExtensions.Contains(ext.ToLower()))
                    {
                        string datet = DateTime.Now.ToString().Replace('/', ' ').Replace(':', ' ');
                        string myfilename = SM.ProductType.Replace('|', '.') + "." + file.FileName;
                         path = Path.Combine(Server.MapPath("~/Content/assets/images/service logos"), myfilename);
                        SM.ProductLogo = myfilename;
                       
                    }
                    else
                    {
                        this.ShowPopup(1, "File Must be .jpg,.png,.jpeg");
                        return RedirectToAction("ManageServices", ControllerName, productid);
                    }
                }
                #endregion
                

                ServicesCommon service = SM.MapObject<ServicesCommon>();
                CommonDbResponse dbresp = _services.ManageServices(service, username);
                if (dbresp.Code == 0)
                {
                    if(path!="")
                    {
                        file.SaveAs(path);

                    }
                    this.ShowPopup(0, "Save Succesfully");
                   // dbresp.SetMessageInTempData(this);
                    return RedirectToAction("Index");
                }
            }
            this.ShowPopup(1, "Error");
           // dbresp.SetMessageInTempData(this);

            return RedirectToAction("ManageServices", SM);
        }
        [HttpGet]
        public ActionResult ServicesStatus()
        {
            var lst = _services.GetServicesList().MapObjects<ServicesModel>();
            List<ServicesModel> servicesModels = new List<ServicesModel>();
            foreach(var item in lst)
            {
                ServicesModel model = new ServicesModel();
                model.ProductId = item.ProductId.EncryptParameter();
                model.ProductLogo = item.ProductLogo;
                model.ProductLabel = item.ProductLabel;
                model.Status = item.Status.Trim();//== "Y" ? "checked" : "";
                servicesModels.Add(model);
            }
            return View(servicesModels);

        }
        [HttpPost, ValidateAntiForgeryToken]
        public void ServicesStatus(string[] servicelist)
        {
           string[] services = servicelist;
            for(Int32 i=0;i<services.Length;i++)
            {
                services[i] = services[i].DecryptParameter();
            }
            string username = Session["username"].ToString();
            string ipaddress = ApplicationUtilities.GetIP();
            _services.ServicesStatus(services, username,ipaddress);
            this.ShowPopup(0, "Completed");
            //return null;
        }
        public object LoadDropdownList(string type)
        {
            switch (type)
            {
                case "status":
                    return new List<SelectListItem> { new SelectListItem { Text = "Select Status", Value = "" }, new SelectListItem { Text = "Enable", Value = "Y" }, new SelectListItem { Text = "Disable", Value = "N" } };
                case "company":
                    return _services.Dropdown("010");
                case "txntype":
                    return _services.Dropdown("011");
                case "producttype":
                    return _services.Dropdown("028");
                case "productcategory":
                    return _services.Dropdown("027");
                case "primarygateway":
                    return _services.Dropdown("012");
                case "secondarygateway":
                    return _services.Dropdown("012");

            }
            return null;
        }

        /*



        [HttpGet]
        public ActionResult Edit(string  id)
        {
            ServicesCommon servicesCommon = new ServicesCommon();
            if (!String.IsNullOrEmpty(id))
            {
                var productid = id.DecryptParameter();
                if (string.IsNullOrEmpty(id))
                    return RedirectToAction("Index");
                 servicesCommon = _services.GetServicesByProductId(Int32.Parse(productid));
                servicesCommon.ProductId = servicesCommon.ProductId.EncryptParameter();
            }
            servicesCommon.StatusList = StatusDropdownList() as List<SelectListItem>;
            ServicesModel lst = servicesCommon.MapObject<ServicesModel>();
            return View(lst);
        }
        [HttpPost]
        public ActionResult Edit(ServicesModel SM)
        {
            string username = Session["UserName"].ToString();
            SM.ProductId = SM.ProductId.DecryptParameter();
            ServicesCommon service = SM.MapObject<ServicesCommon>();

            CommonDbResponse dbresp = _services.UpdateServicesByProductId(service,username);
            return RedirectToAction("Index");
        }*/




    }
}