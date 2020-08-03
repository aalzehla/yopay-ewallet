﻿using ewallet.shared.Models;
using ewallet.shared.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.Services
{
    public interface IServicesManagementRepository
    {
        List<ServicesCommon> GetServicesList(string UserId = "");
        ServicesCommon GetServicesByProductId(int ProductId);
        //CommonDbResponse UpdateServicesByProductId(ServicesCommon SC, string username);
        CommonDbResponse ManageServices(ServicesCommon SC, string username);
        Dictionary<string, string> Dropdown(string flag);
        void ServicesStatus(string[] services, string username, string ipaddress);
    }
}
