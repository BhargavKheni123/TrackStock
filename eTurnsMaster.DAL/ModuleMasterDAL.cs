using System.Collections.Generic;
using System.Data;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;

namespace eTurnsMaster.DAL
{
    public partial class ModuleMasterDAL : eTurnsMasterBaseDAL
    {
        
        public IEnumerable<ParentModuleMasterDTO> GetParentCachedData()
        {
            //Get Cached-Media
            IEnumerable<ParentModuleMasterDTO> ObjCache = CacheHelper<IEnumerable<ParentModuleMasterDTO>>.GetCacheItem("Cached_ParentModuleMaster_");
            
            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ParentModuleMasterDTO> obj = context.Database.SqlQuery<ParentModuleMasterDTO>("exec [GetAllParentModuleMasterPlain]").ToList();
                    ObjCache = CacheHelper<IEnumerable<ParentModuleMasterDTO>>.AddCacheItem("Cached_ParentModuleMaster_", obj);
                }
            }
            return ObjCache;
        }

        /// <summary>
        /// Get Particullar Record from the Module Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ModuleMasterDTO GetModuleByIdPlain(long Id)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", Id) };
            
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ModuleMasterDTO>("exec [GetModuleByIdPlain] @ID", params1).SingleOrDefault();
            }
        }

        public IEnumerable<ParentModuleMasterDTO> GetParentRecord()
        {
            return GetParentCachedData().OrderBy("ID ASC");
        }

    }
}
