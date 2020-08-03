﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ewallet.shared.Models.KYC
{
    public class KYCCommon
    {
        public string UserId { get; set; }
        public string AgentId { get; set; }
        public string ActionUser { get; set; }
        public string ActionIpAddress { get; set; }
        public string CreatedPlatform { get; set; }
        public string AgentType { get; set; }
        public string SubmittedDate { get; set; }
        public string Action { get; set; }
        public string KycStatus { get; set; }
        //This is For Personnel Details
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        //[DataType(DataType.Date)]
        public string DOB_Eng { get; set; }
        public string DOB_Nep { get; set; }
        public string Gender { get; set; }
        public string Occupation { get; set; }
        public string MaritalStatus { get; set; }
        public string SpouseName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string GrandFatherName { get; set; }
        public string Nationality { get; set; }
        public string Country { get; set; }
        public string PProvince { get; set; }
        public string PDistrict { get; set; }
        public string PLocalBody { get; set; }
        public string PWardNo { get; set; }
        public string PAddress { get; set; }
        public string TProvince { get; set; }
        public string TDistrict { get; set; }
        public string TLocalBody { get; set; }
        public string TWardNo { get; set; }
        public string TAddress { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string EmailAddress { get; set; }
        public string PPImage { get; set; }
        public string UpdatedBy { get; set; }
        public string Updated_IP { get; set; }

        //THIS IS FOR KYC DOCUMENTS
        public string Id_type { get; set; }
        public string Id_No { get; set; }
        public string Id_IssuedDateAD { get; set; }
        public string Id_IssuedDateBS { get; set; }
        public string Id_IssuedPlace { get; set; }
        public string Id_ExpiryDateAD { get; set; }
        public string Id_ExpiryDateBS { get; set; }
        public string Id_DocumentFront { get; set; }
        public string Id_DocumentBack { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
    }
}
