﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ewallet.application.Models
{
    public class ActivityLogModel
    {
        public string page_url { get; set; }
        public string page_name { get; set; }
        public string log_type { get; set; }
        public string ipaddress { get; set; }
        public string browser_detail { get; set; }
        public string user_name { get; set; }
        public string CreatedLocalDate { get; set; }
        public string CreatedBy { get; set; }
        public string ActionUser { get; set; }
    }
}