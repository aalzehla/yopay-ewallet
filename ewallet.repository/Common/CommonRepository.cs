using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.repository.Common
{
    public class CommonRepository:ICommonRepository
    {
        RepositoryDao dao;
        public CommonRepository()
        {
            dao = new RepositoryDao();
        }
        public Dictionary<string, string> sproc_get_dropdown_list(string flag, string extra1 = "")
        {
            string sql = "sproc_get_dropdown_list";
            sql += " @flag=" + dao.FilterString(flag);
            sql += ", @search_field1=" + dao.FilterString(extra1);
            Dictionary<string, string> dict = dao.ParseSqlToDictionary(sql);
            return dict;
        }
    }
}
