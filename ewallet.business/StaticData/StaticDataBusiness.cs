using ewallet.repository.StaticData;
using ewallet.shared;
using ewallet.shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ewallet.business.StaticData
{
    public class StaticDataBusiness : IStaticDataBusiness
    {
        IStaticDataRepository repo;
        public StaticDataBusiness(StaticDataRepository _repo)
        {
            repo = _repo;
        }

        //public List<ewallet.shared.Models.StaticDataCommon> GetList(string User, string Id, string Search, int Pagesize)
        //{
        //    return repo.GetList(User, Id, Search, Pagesize);
        //}

        public List<StaticDataCommon> GetStaticDataTypeList()
        {
            return repo.GetStaticDataTypeList();
        }
        public List<StaticDataCommon> GetStaticDataList(string staticdatatypeid)
        {
            return repo.GetStaticDataList( staticdatatypeid);
        }
        public StaticDataCommon GetStaticDataById(string staticdataId, string staticdatatypeId)
        {
            return repo.GetStaticDataById(staticdataId, staticdatatypeId);
        }
        public CommonDbResponse ManageStaticData(StaticDataCommon sdc)
        {
            return repo.ManageStaticData( sdc);
        }
        public CommonDbResponse block_unblockStaticData(StaticDataCommon SDC, string status)
        {
            return repo.block_unblockStaticData(SDC, status);
        }
    }
}
