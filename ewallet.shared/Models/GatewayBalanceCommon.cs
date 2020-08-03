using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.shared.Models;

namespace ewallet.shared.Models
{
    public class GatewayBalanceCommon:Common
    {
        public string Gatewayid { get; set; }
        public string GatewayName { get; set; }
        public string GatewayStatus { get; set; }
        public float AvaliableBalance { get; set; }
        public string GatewayCurrency { get; set; }
        public float BalanceToBeAdd { get; set; }
        public string Remarks { get; set; }
    }
}
