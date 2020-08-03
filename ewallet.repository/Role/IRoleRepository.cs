﻿using ewallet.shared.Models;
using ewallet.shared.Models.Role;
using System;
using System.Collections.Generic;

namespace ewallet.repository.Role
{
    public interface IRoleRepository
    {
        List<RoleCommon> GetRoles();
        CommonDbResponse Manage(RoleCommon setup);
        List<Tuple<string, string, string, bool>> GetRoleList(string RoleID);
        CommonDbResponse AssignRoles(string RoleId, string RoleList, string user);
        List<Tuple<string, string, string, bool>> GetFunctionList(string RoleId);
        CommonDbResponse AssignFunctions(string Roleid, string FunctionList, string user);
    }
}