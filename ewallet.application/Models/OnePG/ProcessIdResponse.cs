﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ewallet.application.Models.OnePG
{
    public class ProcessIdResponse
    {
        public string ProcessId { get; set; }
    }
    public class CheckTransactionResponse
    {
        public string MerchantTxnId { get; set; }
        public string GatewayReferenceNo { get; set; }
        public string Status { get; set; }
        public string Institution { get; set; }
        public string Instrument { get; set; }
    }
}