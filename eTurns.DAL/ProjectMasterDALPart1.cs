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
using eTurns.DTO.Resources;
using System.Web;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class ProjectMasterDAL : eTurnsBaseDAL
    {

        public IEnumerable<ProjectMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                return context.ExecuteStoreQuery<ProjectMasterDTO>("exec [GetProjectMasterByRoom] @RoomID,@CompanyID,@IsDeleted,@IsArchived", params1).ToList();
            }
        }

        public bool Delete(Int64 id, Int64 userID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ProjectMaster obj = context.ProjectMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userID;
                obj.IsArchived = false;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.EditedFrom = "Web";
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Unchanged);
                context.ProjectMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<ProjectMasterDTO> ObjCache = CacheHelper<IEnumerable<ProjectMasterDTO>>.GetCacheItem("Cached_ProjectMaster_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<ProjectMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<ProjectMasterDTO>>.AppendToCacheItem("Cached_ProjectMaster_" + obj.CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        public IEnumerable<ProjectMasterDTO> GetProjectsForReport(long[] arrRoomIDs, long[] arrCompanyIds, string[] filterStatus, string fromDate, string ToDate)
        {
            string RoomIDs = string.Empty;
            string CompanyIds = string.Empty;
            string WhereCondition = " WHERE ISNULL(A.IsDeleted,0)!=1 AND ISNULL(A.IsArchived,0) != 1 ";
            if (arrRoomIDs != null && arrRoomIDs.Length > 0)
            {
                RoomIDs = string.Join(",", arrRoomIDs);
                WhereCondition += " And A.Room in (" + RoomIDs + ")";
            }

            if (arrCompanyIds != null && arrCompanyIds.Length > 0)
            {
                CompanyIds = string.Join(",", arrCompanyIds);
                WhereCondition += " And A.CompanyId in (" + CompanyIds + ")";
            }

            if (!string.IsNullOrEmpty(fromDate) && Convert.ToDateTime(fromDate) > DateTime.MinValue)
            {
                WhereCondition += " And A.Created >= Convert(DateTime,'" + Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd HH:mm:ss") + "')";
            }

            if (!string.IsNullOrEmpty(ToDate) && Convert.ToDateTime(ToDate) > DateTime.MinValue)
            {
                WhereCondition += " And A.Created <= Convert(DateTime,'" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd HH:mm:ss") + "')";
            }
            string NewWhereClause = string.Empty;
            if (filterStatus != null && filterStatus.Length > 0)
            {
                foreach (var item in filterStatus)
                {
                    int Sts = 0;
                    int.TryParse(item, out Sts);

                    switch (Sts)
                    {
                        case (int)RPT_PSStatus.Closed:
                            if (!string.IsNullOrWhiteSpace(NewWhereClause))
                            {
                                NewWhereClause += " OR ";
                            }
                            NewWhereClause += " (ISNULL(A.IsClosed,0) = 1) ";
                            break;
                        case (int)RPT_PSStatus.Open:
                            if (!string.IsNullOrWhiteSpace(NewWhereClause))
                            {
                                NewWhereClause += " OR ";
                            }
                            NewWhereClause += "  (ISNULL(A.IsClosed,0) = 0) ";
                            break;
                        case (int)RPT_PSStatus.ItemDollarUsedGreaterDollarLimit:
                            if (!string.IsNullOrWhiteSpace(NewWhereClause))
                            {
                                NewWhereClause += " OR ";
                            }
                            NewWhereClause += "  (A.[Guid] in (SELECT B.ProjectGuid FROM ProjectSpendItems B Where ISNULL(B.DollarUsedAmount,0) > ISNULL(B.DollarLimitAmount,0)) ) ";
                            break;
                        case (int)RPT_PSStatus.ItemDollarUsedLessDollarLimit:
                            if (!string.IsNullOrWhiteSpace(NewWhereClause))
                            {
                                NewWhereClause += " OR ";
                            }
                            NewWhereClause += "  (A.[Guid] in (SELECT B.ProjectGuid FROM ProjectSpendItems B Where ISNULL(B.DollarUsedAmount,0) < ISNULL(B.DollarLimitAmount,0))) ";
                            break;
                        case (int)RPT_PSStatus.ItemQtyUsedGreaterQtyLimit:
                            if (!string.IsNullOrWhiteSpace(NewWhereClause))
                            {
                                NewWhereClause += " OR ";
                            }
                            NewWhereClause += "  (A.[Guid] in (SELECT B.ProjectGuid FROM ProjectSpendItems B Where ISNULL(B.QuantityUsed,0) > ISNULL(B.QuantityLimit,0))) ";
                            break;
                        case (int)RPT_PSStatus.ItemQtyUsedLessQtyLimit:
                            if (!string.IsNullOrWhiteSpace(NewWhereClause))
                            {
                                NewWhereClause += " OR ";
                            }
                            NewWhereClause += "  (A.[Guid] in (SELECT B.ProjectGuid FROM ProjectSpendItems B Where ISNULL(B.QuantityUsed,0) < ISNULL(B.QuantityLimit,0))) ";
                            break;
                        case (int)RPT_PSStatus.ProjDollarUsedGreaterDollarLimit:
                            if (!string.IsNullOrWhiteSpace(NewWhereClause))
                            {
                                NewWhereClause += " OR ";
                            }
                            NewWhereClause += "  (ISNULL(A.DollarUsedAmount,0) > ISNULL(A.DollarLimitAmount,0)) ";
                            break;
                        case (int)RPT_PSStatus.ProjDollarUsedLessDollarLimit:
                            if (!string.IsNullOrWhiteSpace(NewWhereClause))
                            {
                                NewWhereClause += " OR ";
                            }
                            NewWhereClause += "  (ISNULL(A.DollarUsedAmount,0) < ISNULL(A.DollarLimitAmount,0)) ";
                            break;
                    }

                }
                if (!string.IsNullOrWhiteSpace(NewWhereClause))
                {
                    NewWhereClause = "AND ( " + NewWhereClause + " )";
                }
            }

            if (!string.IsNullOrWhiteSpace(NewWhereClause))
            {
                WhereCondition = WhereCondition + NewWhereClause;
            }

            IEnumerable<ProjectMasterDTO> obj = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = (from u in context.ExecuteStoreQuery<ProjectMasterDTO>(@"SELECT A.ID,A.[Guid],A.ProjectSpendName,ISClosed
                                                                            FROM ProjectMaster A  " + WhereCondition)
                       select new ProjectMasterDTO
                       {
                           ID = u.ID,
                           GUID = u.GUID,
                           ProjectSpendName = u.ProjectSpendName,
                       }).AsParallel().ToList();

            }

            return obj;
        }
    }
}
