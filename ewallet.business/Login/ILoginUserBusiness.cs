using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.shared.Models;
using ewallet.shared.Models.Login;
using ewallet.shared.Models.Menus;

namespace ewallet.business.Login
{
    public interface ILoginUserBusiness
    {
        LoginResponse Login(LoginCommon request);
        UserMenuFunctions GetMenus(string UserLoginId);
        Dictionary<string, string> UserList();
        List<string> GetApplicatinFunction(string UserLoginId, bool loggedin = false);
        CommonDbResponse Signup(LoginCommon customer);
        CommonDbResponse setpassword(LoginCommon common);
        CommonDbResponse verifycode(LoginCommon verify);
        CommonDbResponse checkusername(LoginCommon common);
        CommonDbResponse Checkverifycode(LoginCommon Verify);
        CommonDbResponse changepassword(LoginCommon common);
    }
}
