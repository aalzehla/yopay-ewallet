using ewallet.shared.Models.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.Log
{
    public class ApiLogRepository : IApiLogRepository
    {
        RepositoryDao DAO;
        public ApiLogRepository()
        {
            DAO = new RepositoryDao();
        }

        public List<ApiLogCommon> GetApiLogList(string api_log_id = "", string fromDate = "", string toDate = "")
        {
            List<ApiLogCommon> apiLog = new List<ApiLogCommon>();

            string sql = "sproc_api_log_report @flag = 's'";

            sql += string.IsNullOrEmpty(api_log_id) ? "" : ", @api_log_id=" + DAO.FilterString(api_log_id);
            sql += string.IsNullOrEmpty(fromDate) ? "" : ", @from_date=" + DAO.FilterString(fromDate);
            sql += string.IsNullOrEmpty(toDate) ? "" : ", @to_date=" + DAO.FilterString(toDate);


            var dbres = DAO.ExecuteDataTable(sql);
            if (dbres != null)
            {
                foreach (DataRow dr in dbres.Rows)
                {
                    ApiLogCommon apilog = new ApiLogCommon();
                    apilog.apiLogId = dr["api_log_id"].ToString();
                    apilog.apiRequest = dr["api_request"].ToString();
                    apilog.apiResponse = dr["api_response"].ToString();
                    apilog.transacionId = dr["txn_id"].ToString();
                    apilog.userId = dr["user_id"].ToString();
                    apilog.functionName = dr["function_ame"].ToString();
                    apilog.createdLocalDate = dr["created_local_Date"].ToString();
                    apilog.createdUtcDate = dr["created_UTC_Date"].ToString();



                    apiLog.Add(apilog);
                }
            }
            return apiLog;
        }
    }
}
