using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.shared.Models;
using ewallet.shared.Models.Commission;
namespace ewallet.business.Commission
{
    public interface ICommissionBusiness
    {
        List<CommissionCategoryCommon> GetCommissionCategoryList(string agentid);
        CommonDbResponse ManageCommissionCategory(CommissionCategoryCommon CC);
        CommissionCategoryCommon GetCommissionCategoryById(string Id);
        List<CommissionCategoryDetailCommon> GetCommissionCategoryProductList(string Id);
        CommissionCategoryDetailCommon GetCommissioncategoryProductById(string id);
        CommonDbResponse ManageCommissionCategoryProduct(CommissionCategoryDetailCommon CDC);
        List<AssignCommissionCommon> GetAssignedCategoryList(AssignCommissionCommon ACC);
        AssignCommissionCommon GetAssignedCategoryById(string id);
        CommonDbResponse ManageAssignCategory(AssignCommissionCommon ACC);
        CommonDbResponse block_unblockCategory(CommissionCategoryCommon ccc, string status);
    }
}
