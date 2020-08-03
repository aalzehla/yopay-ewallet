﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.shared.Models;
using ewallet.shared.Models.Merchant;

namespace ewallet.repository.Merchant
{
    public interface IMerchantManagementRepository
    {
        List<MerchantCommon> GetMerchantList(string MerchantId = "", string parentid = "");
        CommonDbResponse ManageMerchant(MerchantCommon merchantCommon);
        MerchantCommon GetMerchantById(string MerchantId);
        CommonDbResponse Disable_EnableMerchant(MerchantCommon merchantCommon);
    }
}
