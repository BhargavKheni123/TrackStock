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
    public class LabelModuleFieldMasterDAL : eTurnsBaseDAL
    {
        private IEnumerable<LabelModuleFieldMasterDTO> GetCachedData()
        {
            //Get Cached-Media
            IEnumerable<LabelModuleFieldMasterDTO> ObjCache = null;//CacheHelper<IEnumerable<LabelModuleFieldMasterDTO>>.GetCacheItem("Cached_LabelModuleFieldMaster");
            if (ObjCache == null || ObjCache.Count() < 0)
            {
                LabelModuleMasterDAL objLabelModuleDAL = new LabelModuleMasterDAL(base.DataBaseName);
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<LabelModuleFieldMasterDTO> obj = (from u in context.ExecuteStoreQuery<LabelModuleFieldMasterDTO>(@"SELECT A.*  FROM LabelModuleFieldMaster A Where ID Not In (221) and FieldName not in ('PackageQuantity') Order by A.FieldDisplayOrder ASC, A.FieldName ASC")
                                                                  select new LabelModuleFieldMasterDTO
                                                                  {
                                                                      ID = u.ID,
                                                                      FieldName = u.FieldName,
                                                                      FieldDisplayName = u.FieldDisplayName,
                                                                      IncludeInBarcode = u.IncludeInBarcode,
                                                                      ModuleID = u.ModuleID,
                                                                      FieldDisplayOrder = u.FieldDisplayOrder,
                                                                      //ModuleMasterDTO = objLabelModuleDAL.GetRecord(u.ModuleID),
                                                                  }).AsParallel().ToList();
                    ObjCache = obj;//CacheHelper<IEnumerable<LabelModuleFieldMasterDTO>>.AddCacheItem("Cached_LabelModuleFieldMaster", obj);
                }
            }



            return ObjCache;
        }

    }
}
