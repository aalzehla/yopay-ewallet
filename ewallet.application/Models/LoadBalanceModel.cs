using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ewallet.application.Models
{
    public class LoadBalanceModel
    {
        public string pmt_txn_id { get; set; }
        public string service_charge { get; set; }
        public string amount { get; set; }
        public string card_no { get; set; }
        public string remarks { get; set; }
        public string user_id { get; set; }
        public string gateway_status { get; set; }
        public string pmt_gateway_id { get; set; }
        public string action_user { get; set; }
        public string error_code { get; set; }
        public string action_ip { get; set; }
        public string action_browser { get; set; }
        public string pmt_gateway_txn_id { get; set; }
    }
}