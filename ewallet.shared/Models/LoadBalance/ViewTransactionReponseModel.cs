using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.shared.Models.LoadBalance
{
   public class ViewTransactionReponseModel
    {
        [Display(Name ="Gateway Transaction Id")]
        public string pmt_gateway_txn_id { get; set; }

        [Display(Name = "Transaction Id")]
        public string pmt_txn_id { get; set; }
        [Display(Name = "Amount")]
        public string amount { get; set; }
        [Display(Name = "Gateway Status")]
        public string gateway_status { get; set; }

        [Display(Name = "Agent Id")]
        public string agent_id { get; set; }

        [Display(Name = "User Name")]
        public string user_name { get; set; }
    }
}
