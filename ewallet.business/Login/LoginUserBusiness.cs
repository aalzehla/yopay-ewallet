using System.Collections.Generic;
using System.Data;
using ewallet.repository.Login;
using ewallet.shared.Models;
using ewallet.shared.Models.Login;
using ewallet.shared.Models.Menus;

namespace ewallet.business.Login
{
    public class LoginUserBusiness : ILoginUserBusiness
    {
        ILoginUserRepository _repo;
        public LoginUserBusiness()
        {
            _repo = new LoginUserRepository();
        }

        public List<string> GetApplicatinFunction(string UserLoginId, bool loggedin = false)
        {
            return _repo.GetApplicationFunction(UserLoginId,loggedin);
        }

        public UserMenuFunctions GetMenus(string UserLoginId)
        {
            return _repo.GetMenus(UserLoginId);
        }

        public LoginResponse Login(LoginCommon request)
        {
            return _repo.Login(request);
        }

        public Dictionary<string, string> UserList()
        {
            return _repo.UserList();
        }
        public CommonDbResponse Signup(LoginCommon customer)
        {
            return _repo.Signup(customer);
        }
        public CommonDbResponse verifycode(LoginCommon verify)
        {
            return _repo.verifycode(verify);
        }
        public CommonDbResponse setpassword(LoginCommon common)
        {
            return  _repo.setpassword( common);
        }
        public CommonDbResponse checkusername(LoginCommon common)
        {
            return _repo.checkusername(common);
        }
        public CommonDbResponse Checkverifycode(LoginCommon Verify)
        { return _repo.Checkverifycode(Verify); 
        }
        public CommonDbResponse changepassword(LoginCommon common)
        {
            return _repo.changepassword(common);
        }


    }
}
