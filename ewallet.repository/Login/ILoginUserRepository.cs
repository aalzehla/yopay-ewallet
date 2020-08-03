using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.shared.Models;
using ewallet.shared.Models.Login;
using ewallet.shared.Models.Menus;

namespace ewallet.repository.Login
{
    public interface ILoginUserRepository
    {
        LoginResponse Login(LoginCommon request);
        UserMenuFunctions GetMenus(string UserLoginId);
        Dictionary<string, string> UserList();
        List<string> GetApplicationFunction(string RoleId, bool loggedin = false);
        CommonDbResponse Signup(LoginCommon customer);
        CommonDbResponse verifycode(LoginCommon verify);
        CommonDbResponse setpassword(LoginCommon common);
        CommonDbResponse checkusername(LoginCommon common);
        CommonDbResponse Checkverifycode(LoginCommon Verify);
        CommonDbResponse changepassword(LoginCommon common);
    }
}
