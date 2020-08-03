﻿using ewallet.shared.Models;
using ewallet.shared.Models.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.Log
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        RepositoryDao DAO;
        public ActivityLogRepository()
        {
            DAO = new RepositoryDao();
        }
        public CommonDbResponse InsertActivityLog(ActivityLog al)
        {
            CommonDbResponse cResponse = new CommonDbResponse();
            string sqlCommand = " Execute sproc_activity_log @flag = 'i',";
            sqlCommand += "@action_user = " + DAO.FilterString(al.user_name) + ",";
            sqlCommand += "@page_name = " + DAO.FilterString(al.page_name) + ",";
            sqlCommand += "@page_url = " + DAO.FilterString(al.page_url) + ",";
            sqlCommand += "@log_type = " + DAO.FilterString(al.log_type) + ",";
            sqlCommand += "@action_ip_address = " + DAO.FilterString(al.ipaddress) + ",";
            sqlCommand += "@action_browser = " + DAO.FilterString(al.browser_detail);
            cResponse = DAO.ParseCommonDbResponse(sqlCommand);
            return cResponse;
        }
        public List<ActivityLog> ActivityLog(string ActionUser)
        {
            List<ActivityLog> activityLogs = new List<ActivityLog>();
            string sql = "sproc_activity_log @flag = 's', @action_user= '" + ActionUser + "'";
            var dbres = DAO.ExecuteDataTable(sql);
            if (dbres != null)
            {
                foreach (DataRow dr in dbres.Rows)
                {
                    ActivityLog ActivityLog = new ActivityLog();
                    ActivityLog.page_name = dr["page_name"].ToString();
                    ActivityLog.page_url = dr["Page_url"].ToString();
                    ActivityLog.ipaddress = dr["from_ip_address"].ToString();
                    ActivityLog.browser_detail = dr["from_browser"].ToString();
                    ActivityLog.CreatedBy = dr["created_by"].ToString();
                    ActivityLog.CreatedLocalDate = dr["created_local_Date"].ToString();

                    activityLogs.Add(ActivityLog);
                }
            }
            return activityLogs;
        }
    }
}
