﻿using ewallet.shared.Models;
using ewallet.shared.Models.SubDistributor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.SubDistributor
{
    public interface ISubDistributorRepository
    {
        List<SubDistributorCommon> GetSubDistributorsList(string username, string Distid, string SubDistId = "");
        CommonDbResponse Manage(SubDistributorCommon sdc);
        SubDistributorCommon GetSubDistributorById(string agentid, string username);
        List<SubDistributorCommon> GetUserList(string agentid,string  userId);
        SubDistributorCommon GetUserById(string agentid, string UserId = "");
        CommonDbResponse ManageUser(SubDistributorCommon SDC, string changepassword);
        CommonDbResponse block_unblockuser(string userid, string status, string agentid);


    }
}
