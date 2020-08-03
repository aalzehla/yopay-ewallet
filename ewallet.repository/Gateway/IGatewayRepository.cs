﻿using ewallet.shared.Models;
using System.Collections.Generic;

namespace ewallet.repository.Gateway
{
    public interface IGatewayRepository
    {
        List<GatewayCommon> GetGatewayList();
        GatewayCommon GetGatewayById(string gateway_id);
        CommonDbResponse ManageGateway(GatewayCommon gatewaysetup);
        CommonDbResponse updatebalance(GatewayBalanceCommon bc);
        List<GatewayProductCommon> GetGatewayProductList(string GatewayId, string ProductId);
        CommonDbResponse ManageGatewayProductCommission(GatewayProductCommon GWPC);
        List<GatewayBalanceCommon> GetGatewayReportList();
    }
}