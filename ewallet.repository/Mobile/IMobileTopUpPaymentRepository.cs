using ewallet.shared.Models;
using ewallet.shared.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.Mobile
{
    interface IMobileTopUpPaymentRepository
    {
        CommonDbResponse MobileTopUpPaymentRequest(MobileTopUpPaymentRequest mr);
        CommonDbResponse MobileTopUpPaymentResponse(MobileTopUpPaymentUpdateRequest mr);
    }
}
