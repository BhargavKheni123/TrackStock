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
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class ProjectMasterDAL : eTurnsBaseDAL
    {
        public IEnumerable<ProjectMasterDTO> GetProjectMaster(Int64 RoomID, Int64 CompanyID, DateTime? FromDate, DateTime? ToDate, string Status, Int32 StatusValue = 0)
        {

            IEnumerable<ProjectMasterDTO> ObjCache;

            ObjCache = CacheHelper<IEnumerable<ProjectMasterDTO>>.GetCacheItem("Cached_ProjectMaster_Count" + CompanyID.ToString());
            //if (ObjCache == null)
            //{
            string strCheck = "";
            if (StatusValue == 0)
            {
                if (Status.ToLower().Contains("project amount"))
                    strCheck = " AND CASE WHEN ISNULL(PM.DollarLimitAmount,0) > 0 THEN (100 * (ISNULL(PM.DollarUsedAmount,0)/PM.DollarLimitAmount)) ELSE 0 END > (select isnull(ProjectAmountExceed,0) FROM CompanyConfig WHERE CompanyID = " + CompanyID + ")";
                else if (Status.ToLower().Contains("item quantity"))
                    strCheck = " AND CASE WHEN ISNULL(PSI.QuantityLimit,0) > 0 THEN (100 * (ISNULL(PSI.QuantityUsed,0)/PSI.QuantityLimit)) ELSE 0 END > (select isnull(ProjectItemQuantitExceed,0) FROM CompanyConfig WHERE CompanyID = " + CompanyID + ")";
                else if (Status.ToLower().Contains("item amount"))
                    strCheck = " AND CASE WHEN ISNULL(PM.DollarLimitAmount,0) > 0 THEN (100 * (ISNULL(PM.DollarUsedAmount,0)/PM.DollarLimitAmount)) ELSE 0 END > (select isnull(ProjectAmountExceed,0) FROM CompanyConfig WHERE CompanyID = " + CompanyID + ")";
            }
            else
            {
                if (Status.ToLower().Contains("project amount"))
                    strCheck = " AND CASE WHEN ISNULL(PM.DollarLimitAmount,0) > 0 THEN (100 * (ISNULL(PM.DollarUsedAmount,0)/PM.DollarLimitAmount)) ELSE 0 END > " + StatusValue;
                else if (Status.ToLower().Contains("item quantity"))
                    strCheck = " AND CASE WHEN ISNULL(PSI.QuantityLimit,0) > 0 THEN (100 * (ISNULL(PSI.QuantityUsed,0)/PSI.QuantityLimit)) ELSE 0 END > " + StatusValue;
                else if (Status.ToLower().Contains("item amount"))
                    strCheck = " AND CASE WHEN ISNULL(PSI.QuantityLimit,0) > 0 THEN (100 * (ISNULL(PSI.DollarUsedAmount,0)/PSI.DollarLimitAmount)) ELSE 0 END > " + StatusValue;
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<ProjectMasterDTO> obj = (from u in context.ExecuteStoreQuery<ProjectMasterDTO>(@"SELECT Distinct PM.ProjectSpendName,PM.ID,PM.Description, PM.DollarLimitAmount,PM.DollarUsedAmount,PM.Room,PM.CompanyID,PM.Created
                                                         FROM ProjectMaster PM  INNER JOIN ProjectSpendItems PSI ON PM.GUID = PSI.ProjectGUID WHERE PM.IsDeleted!=1 AND PM.IsArchived != 1 AND PM.CompanyID = " + CompanyID.ToString() + strCheck + " ")
                                                     select new ProjectMasterDTO
                                                     {
                                                         ProjectSpendName = u.ProjectSpendName,
                                                         DollarLimitAmount = u.DollarLimitAmount,
                                                         DollarUsedAmount = u.DollarUsedAmount,
                                                         CompanyID = u.CompanyID,
                                                         Created = u.Created,
                                                         Room = u.Room,
                                                         ID = u.ID,
                                                         Description = u.Description,
                                                         ProjectSpendItems = new ProjectSpendItemsDAL(base.DataBaseName).GetHistoryRecordByProjectID(u.GUID).ToList()
                                                     }).AsParallel().ToList();
                ObjCache = CacheHelper<IEnumerable<ProjectMasterDTO>>.AddCacheItem("Cached_ProjectMaster_Count" + CompanyID.ToString(), obj);
            }
            //}

            if (FromDate != null && ToDate != null)
                return ObjCache.Where(t => t.Room == RoomID && (t.Created.Value.Date >= FromDate.Value.Date && t.Created.Value.Date <= ToDate.Value.Date));
            else
                return ObjCache.Where(t => t.Room == RoomID);
        }

        public CompanyConfigDTO GetCompanyConfig(Int64 CompanyID)
        {
            CompanyConfigDTO ObjCache;

            ObjCache = CacheHelper<CompanyConfigDTO>.GetCacheItem("Cached_CompanyConfig_" + CompanyID.ToString());
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (ObjCache == null)
                {
                    CompanyConfigDTO obj = (from u in context.ExecuteStoreQuery<CompanyConfigDTO>(@"SELECT *  FROM CompanyConfig WHERE CompanyID = " + CompanyID.ToString())
                                            select new CompanyConfigDTO
                                            {
                                                ID = u.ID,
                                                OperationalHoursBefore = u.OperationalHoursBefore,
                                                MileageBefore = u.MileageBefore,
                                                ScheduleDaysBefore = u.ScheduleDaysBefore,
                                                ProjectAmountExceed = u.ProjectAmountExceed,
                                                ProjectItemQuantitExceed = u.ProjectItemQuantitExceed,
                                                ProjectItemAmountExceed = u.ProjectItemAmountExceed,
                                                NOBackDays = u.NOBackDays,
                                                NODaysAve = u.NODaysAve,
                                                NOTimes = u.NOTimes,
                                                MinPer = u.MinPer,
                                                MaxPer = u.MaxPer,
                                                CompanyID = u.CompanyID
                                            }).FirstOrDefault();
                    if (obj == null)
                        ObjCache = CacheHelper<CompanyConfigDTO>.AddCacheItem("Cached_CompanyConfig_" + CompanyID.ToString(), new CompanyConfigDTO());
                    else
                        ObjCache = CacheHelper<CompanyConfigDTO>.AddCacheItem("Cached_CompanyConfig_" + CompanyID.ToString(), obj);
                }


                return ObjCache;
            }
        }


    }
}
