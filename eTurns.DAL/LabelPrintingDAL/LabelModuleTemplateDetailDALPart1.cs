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
using System.Data.SqlClient;

namespace eTurns.DAL.LabelPrintingDAL
{
    public class LabelModuleTemplateDetailDAL : eTurnsBaseDAL
    {
        public IEnumerable<LabelModuleTemplateDetailDTO> GetCachedData()
        {
            //Get Cached-Media
            IEnumerable<LabelModuleTemplateDetailDTO> ObjCache = null;// CacheHelper<IEnumerable<LabelModuleTemplateDetailDTO>>.GetCacheItem("Cached_LabelModuleTemplateDetail_" + CompanyID.ToString());
            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<LabelModuleTemplateDetailDTO> obj = (from u in context.ExecuteStoreQuery<LabelModuleTemplateDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName  FROM LabelModuleTemplateDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.UpdatedBy = C.ID ")
                                                                     select new LabelModuleTemplateDetailDTO
                                                                     {
                                                                         ID = u.ID,
                                                                         ModuleID = u.ModuleID,
                                                                         TemplateDetailID = u.TemplateDetailID,
                                                                         CompanyID = u.CompanyID,
                                                                         CreatedBy = u.CreatedBy,
                                                                         UpdatedBy = u.UpdatedBy,
                                                                         CreatedOn = u.CreatedOn,
                                                                         UpdatedOn = u.UpdatedOn,
                                                                         CreatedByName = u.CreatedByName,
                                                                         UpdatedByName = u.UpdatedByName,
                                                                         RoomID = u.RoomID
                                                                     }).AsParallel().ToList();
                    ObjCache = obj;// CacheHelper<IEnumerable<LabelModuleTemplateDetailDTO>>.AddCacheItem("Cached_LabelModuleTemplateDetail_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache;
        }

        public IEnumerable<LabelModuleTemplateDetailDTO> GetCachedData(Int64 CompanyID, Int64 RoomID)
        {
            //Get Cached-Media
            IEnumerable<LabelModuleTemplateDetailDTO> ObjCache = null;// CacheHelper<IEnumerable<LabelModuleTemplateDetailDTO>>.GetCacheItem("Cached_LabelModuleTemplateDetail_" + CompanyID.ToString());
            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<LabelModuleTemplateDetailDTO> obj = (from u in context.ExecuteStoreQuery<LabelModuleTemplateDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName  FROM LabelModuleTemplateDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.UpdatedBy = C.ID   WHERE A.CompanyID = " + CompanyID.ToString() + " and isnull(A.Roomid,0)=" + RoomID.ToString())
                                                                     select new LabelModuleTemplateDetailDTO
                                                                     {
                                                                         ID = u.ID,
                                                                         ModuleID = u.ModuleID,
                                                                         TemplateDetailID = u.TemplateDetailID,
                                                                         CompanyID = u.CompanyID,
                                                                         CreatedBy = u.CreatedBy,
                                                                         UpdatedBy = u.UpdatedBy,
                                                                         CreatedOn = u.CreatedOn,
                                                                         UpdatedOn = u.UpdatedOn,
                                                                         CreatedByName = u.CreatedByName,
                                                                         UpdatedByName = u.UpdatedByName,
                                                                         RoomID = u.RoomID
                                                                     }).AsParallel().ToList();
                    ObjCache = obj;// CacheHelper<IEnumerable<LabelModuleTemplateDetailDTO>>.AddCacheItem("Cached_LabelModuleTemplateDetail_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache;
        }

        public IEnumerable<LabelModuleTemplateDetailDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 CompanyID, Int64 RoomID)
        {
            //Get Cached-Media
            IEnumerable<LabelModuleTemplateDetailDTO> ObjCache = GetCachedData(CompanyID, RoomID);

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<LabelModuleTemplateDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.CreatedOn >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.CreatedOn <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.UpdatedOn >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.UpdatedOn <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<LabelModuleTemplateDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm)
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm)
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public IEnumerable<LabelModuleTemplateDetailDTO> GetAllRecords(Int64 CompanyId, Int64 RoomID)
        {
            return GetCachedData(CompanyId, RoomID).OrderBy("ID DESC");
        }

        public IEnumerable<LabelModuleTemplateDetailDTO> GetAllRecords(Int64 CompanyId, Int64 RoomID)
        {
            return GetCachedData(CompanyId, RoomID).OrderBy("ID DESC");
        }

        public LabelModuleTemplateDetailDTO GetRecord(Int64 id, Int64 CompanyID, Int64 RoomID)
        {
            return GetCachedData(CompanyID, RoomID).Single(t => t.ID == id);
        }


    }
}
