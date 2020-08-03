
using ewallet.repository.Mobile;
using ewallet.shared.Models;
using ewallet.shared.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.Mobile
{
    public class MobileTopUpPaymentBusiness : IMobileTopUpPaymentBusiness
    {
        MobileTopUpPaymentRepository mpb;
        public MobileTopUpPaymentBusiness(MobileTopUpPaymentRepository _mpb )
        {
            this.mpb = _mpb;
        }
        public CommonDbResponse MobileTopUpPaymentRequest(MobileTopUpPaymentRequest mr)
        {
            return mpb.MobileTopUpPaymentRequest(mr);
        }

        public CommonDbResponse MobileTopUpPaymentResponse(MobileTopUpPaymentUpdateRequest mr)
        {
            return mpb.MobileTopUpPaymentResponse(mr);
        }
    }
}
