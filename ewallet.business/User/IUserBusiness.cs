using ewallet.shared.Models;
using System.Collections.Generic;

namespace ewallet.business.User
{
    public interface IUserBusiness
    {
        List<UserCommon> GetAllList(string User, string usertype, int Pagesize);
        UserCommon GetUserById(string UserId);
        CommonDbResponse ManageUser(UserCommon setup);
        CommonDbResponse ChangePassword(UserCommon user);
        CommonDbResponse ChangePin(UserCommon user);
        CommonDbResponse block_unblockuser(string userid, string status);
        List<UserCommon> GetSearchUserList(string SearchField, string SearchFilter, string username);
        Profile UserInfo(string UserId = "");
    }
}