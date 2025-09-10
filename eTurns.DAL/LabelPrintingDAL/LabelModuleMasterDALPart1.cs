using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using eTurns.DTO.LabelPrinting;

namespace eTurns.DAL.LabelPrintingDAL
{
    public class LabelModuleMasterDAL : eTurnsBaseDAL
    {
        private IEnumerable<LabelModuleMasterDTO> GetCachedData()
        {
            //Get Cached-Media
            IEnumerable<LabelModuleMasterDTO> ObjCache = CacheHelper<IEnumerable<LabelModuleMasterDTO>>.GetCacheItem("Cached_LabelModuleMaster");
            if (ObjCache == null || ObjCache.Count() < 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<LabelModuleMasterDTO> obj = (from u in context.ExecuteStoreQuery<LabelModuleMasterDTO>(@"SELECT A.* FROM LabelModuleMaster A ")
                                                             select new LabelModuleMasterDTO
                                                             {
                                                                 ID = u.ID,
                                                                 ModuleName = u.ModuleName,
                                                                 ModuleDTOName = u.ModuleDTOName,
                                                             }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<LabelModuleMasterDTO>>.AddCacheItem("Cached_LabelModuleMaster", obj);
                }
            }

            return ObjCache;
        }

        public IEnumerable<LabelModuleMasterDTO> GetAllRecords()
        {
            return GetCachedData().OrderBy("ModuleName asc");
        }

        public LabelModuleMasterDTO GetRecord(Int64 id)
        {
            return GetCachedData().Single(t => t.ID == id);
        }

        public LabelModuleMasterDTO GetRecord(string moduleName)
        {
            return GetCachedData().Single(t => t.ModuleName == moduleName);
        }

    }
}
