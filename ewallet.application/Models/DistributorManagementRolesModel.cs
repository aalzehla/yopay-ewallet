﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ewallet.application.Models
{
    public class DistributorManagementRolesModel
    {
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Role is required")]
        [Display(Name = "User Role")]
        public string RoleId { get; set; }
        //public List<SelectListItem> RolesList { get; set; }

        [Display(Name = "Is Primary")]
        public string IsPrimary { get; set; }

        [Display(Name = "Agent ID")]
        public string AgentId { get; set; }

        [Display(Name = "User ID")]
        public string UserId { get; set; }
    }
}