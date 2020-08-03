using ewallet.shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ewallet.application.Models
{
    public class StaticDataModel:Common
    {

        public string StaticDataTypeId { get; set; }
        public string StaticDataTypeName { get; set; }
        public string StaticDataTypeDescription { get; set; }

        public string StaticDataId { get; set; }
        public string StaticDataValue { get; set; }
        public string StaticDataLabel { get; set; }
        public string StaticDataDescription { get; set; }
        public string AdditionalValue1 { get; set; }
        public string AdditionalValue2 { get; set; }
        public string AdditionalValue3 { get; set; }
        public string AdditionalValue4 { get; set; }
        public string IsDeleted { get; set; }
    }
}