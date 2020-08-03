﻿using ewallet.repository.MobileTopup;
using ewallet.shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.Client.MobileTopup
{
    public class MobilePaymentBusiness : IMobilePaymentBusiness
    {
        IMobilePaymentRepository repo;
        public MobilePaymentBusiness()
        {
            repo = new MobilePaymentRepository();
        }
        public CommonDbResponse ConsumeService(string MobileNumber, long Amount)
        {
            return repo.ConsumeService(MobileNumber, Amount);
        }

        public CommonDbResponse GetPackage(string MobileNumber, long Amount, string servicecode)
        {
            return repo.GetPackage(MobileNumber, Amount, servicecode);
        }

        public CommonDbResponse Payment(string MobileNumber, long Amount, string servicecode, string billnumber, string refstan)
        {
            return repo.Payment(MobileNumber, Amount, servicecode, billnumber, refstan);
        }
    }
}
