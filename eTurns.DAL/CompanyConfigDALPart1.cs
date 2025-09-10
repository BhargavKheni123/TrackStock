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

namespace eTurns.DAL
{
    public partial class CompanyConfigDAL : eTurnsBaseDAL
    {

       

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CompanyConfigDTO> GetCachedData(Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<CompanyConfigDTO> ObjCache = CacheHelper<IEnumerable<CompanyConfigDTO>>.GetCacheItem("Cached_CompanyConfig_" + CompanyID.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<CompanyConfigDTO> obj = (from u in context.Database.SqlQuery<CompanyConfigDTO>(@"SELECT A.* FROM CompanyConfig A WHERE A.CompanyID = " + CompanyID.ToString())
                                                         select new CompanyConfigDTO
                                                         {
                                                             ID = u.ID,
                                                             CompanyID = u.CompanyID,
                                                             ScheduleDaysBefore = u.ScheduleDaysBefore,
                                                             OperationalHoursBefore = u.OperationalHoursBefore,
                                                             MileageBefore = u.MileageBefore,
                                                             ProjectAmountExceed = u.ProjectAmountExceed,
                                                             ProjectItemQuantitExceed = u.ProjectItemQuantitExceed,
                                                             ProjectItemAmountExceed = u.ProjectItemAmountExceed,
                                                             CostDecimalPoints = u.CostDecimalPoints.GetValueOrDefault(0),
                                                             QuantityDecimalPoints = u.QuantityDecimalPoints.GetValueOrDefault(0),
                                                             DateFormat = u.DateFormat,
                                                             DateFormatCSharp = u.DateFormatCSharp,
                                                             NOBackDays = u.NOBackDays,
                                                             NODaysAve = u.NODaysAve,
                                                             NOTimes = u.NOTimes,
                                                             MinPer = u.MinPer,
                                                             MaxPer = u.MaxPer,
                                                             CurrencySymbol = u.CurrencySymbol,
                                                             CreatedByName = u.CreatedByName,
                                                             UpdatedByName = u.UpdatedByName,
                                                             RoomName = u.RoomName,
                                                             AEMTPndOrders = !string.IsNullOrEmpty(u.AEMTPndOrders) ? u.AEMTPndOrders : "00:01",
                                                             AEMTPndRequisition = !string.IsNullOrEmpty(u.AEMTPndRequisition) ? u.AEMTPndRequisition : "00:01",
                                                             AEMTPndTransfer = !string.IsNullOrEmpty(u.AEMTPndTransfer) ? u.AEMTPndTransfer : "00:01",
                                                             AEMTSggstOrdCrt = !string.IsNullOrEmpty(u.AEMTSggstOrdCrt) ? u.AEMTSggstOrdCrt : "00:01",
                                                             AEMTSggstOrdMin = !string.IsNullOrEmpty(u.AEMTSggstOrdMin) ? u.AEMTSggstOrdMin : "00:01",
                                                             AEMTAssetMntDue = !string.IsNullOrEmpty(u.AEMTAssetMntDue) ? u.AEMTAssetMntDue : "00:01",
                                                             AEMTToolsMntDue = !string.IsNullOrEmpty(u.AEMTToolsMntDue) ? u.AEMTToolsMntDue : "00:01",
                                                             AEMTItemStockOut = !string.IsNullOrEmpty(u.AEMTItemStockOut) ? u.AEMTItemStockOut : "00:01",
                                                             AEMTCycleCount = !string.IsNullOrEmpty(u.AEMTCycleCount) ? u.AEMTCycleCount : "00:01",
                                                             AEMTCycleCntItmMiss = !string.IsNullOrEmpty(u.AEMTCycleCntItmMiss) ? u.AEMTCycleCntItmMiss : "00:01",
                                                             AEMTItemReceiveRpt = !string.IsNullOrEmpty(u.AEMTItemReceiveRpt) ? u.AEMTItemReceiveRpt : "00:01",
                                                             AEMTItemUsageRpt = !string.IsNullOrEmpty(u.AEMTItemUsageRpt) ? u.AEMTItemUsageRpt : "00:01",
                                                             GridRefreshTimeInSecond = u.GridRefreshTimeInSecond.GetValueOrDefault(0),
                                                             TurnsAvgDecimalPoints = u.TurnsAvgDecimalPoints.GetValueOrDefault(0),
                                                             WeightDecimalPoints = u.WeightDecimalPoints.GetValueOrDefault(0),
                                                             IsPackSlipRequired = u.IsPackSlipRequired,
                                                             NumberOfBackDaysToSyncOverPDA = u.NumberOfBackDaysToSyncOverPDA,
                                                         }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<CompanyConfigDTO>>.AddCacheItem("Cached_CompanyConfig_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache;
        }


        /// <summary>
        /// Get Particullar Record from the CompanyConfig by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public CompanyConfigDTO GetRecord(Int64 CompanyID)
        {
            List<CompanyConfigDTO> lstCC = GetCachedData(CompanyID).Where(t => t.CompanyID == CompanyID).ToList();
            //if (lstCC == null || lstCC.Count == 0)
            //{
            //    //                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //    //                {
            //    //                    string strQuery = @"INSERT INTO [CompanyConfig]([CompanyID],[ScheduleDaysBefore],[OperationalHoursBefore],[MileageBefore],[ProjectAmountExceed]
            //    //           ,[ProjectItemQuantitExceed],[ProjectItemAmountExceed],[CostDecimalPoints],[QuantityDecimalPoints]
            //    //           ,[DateFormat],[NOBackDays],[NODaysAve],[NOTimes],[MinPer],[MaxPer],[CurrencySymbol]
            //    //           ,[AEMTPndOrders],[AEMTPndRequisition],[AEMTPndTransfer],[AEMTSggstOrdMin],[AEMTSggstOrdCrt] )
            //    //     VALUES (" + CompanyID.ToString() + ",2,25,25,NULL,NULL,NULL,3,2,'MM-DD-YYYY',NULL,NULL,NULL,NULL,NULL,'$','00:00','00:00','00:00','00:00','00:00')";

            //    //                    if (context.Database.ExecuteSqlCommand(strQuery) > 0)
            //    //                    {
            //    //                        CacheHelper<IEnumerable<CompanyConfigDTO>>.InvalidateCache();

            //    //                        GetCachedData(CompanyID);

            //    //                        return GetRecord(CompanyID);
            //    //                    }

            //    //                }
            //}
            //else
            //{
            //    return lstCC.FirstOrDefault();
            //}
            return lstCC.FirstOrDefault();
        }


        public CompanyConfigDTO GetRecord(Int64 CompanyID, string DBConnectionstring)
        {
            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                return (from cc in context.CompanyConfigs
                        where cc.CompanyID == CompanyID
                        select new CompanyConfigDTO
                        {
                            AEMTAssetMntDue = cc.AEMTAssetMntDue,
                            AEMTCycleCntItmMiss = cc.AEMTCycleCntItmMiss,
                            AEMTCycleCount = cc.AEMTCycleCount,
                            AEMTItemReceiveRpt = cc.AEMTItemReceiveRpt,
                            AEMTItemStockOut = cc.AEMTItemStockOut,
                            AEMTItemUsageRpt = cc.AEMTItemUsageRpt,
                            AEMTPndOrders = cc.AEMTPndOrders,
                            AEMTPndRequisition = cc.AEMTPndRequisition,
                            AEMTPndTransfer = cc.AEMTPndTransfer,
                            AEMTSggstOrdCrt = cc.AEMTSggstOrdCrt,
                            AEMTSggstOrdMin = cc.AEMTSggstOrdMin,
                            AEMTToolsMntDue = cc.AEMTToolsMntDue,
                            CompanyID = cc.CompanyID,
                            CostDecimalPoints = cc.CostDecimalPoints,
                            CurrencySymbol = cc.CurrencySymbol,
                            DateFormat = cc.DateFormat,
                            DateFormatCSharp = cc.DateFormatCSharp,
                            GridRefreshTimeInSecond = cc.GridRefreshTimeInSecond,
                            ID = cc.ID,
                            MaxPer = cc.MaxPer,
                            MileageBefore = cc.MileageBefore,
                            MinPer = cc.MinPer,
                            NOBackDays = cc.NOBackDays,
                            NODaysAve = cc.NODaysAve,
                            NOTimes = cc.NOTimes,
                            OperationalHoursBefore = cc.OperationalHoursBefore,
                            ProjectAmountExceed = cc.ProjectAmountExceed,
                            ProjectItemAmountExceed = cc.ProjectItemAmountExceed,
                            ProjectItemQuantitExceed = cc.ProjectItemQuantitExceed,
                            QuantityDecimalPoints = cc.QuantityDecimalPoints,
                            ScheduleDaysBefore = cc.ScheduleDaysBefore,
                            IsPackSlipRequired = cc.IsPackSlipRequired,
                            NumberOfBackDaysToSyncOverPDA = cc.NumberOfBackDaysToSyncOverPDA,
                        }).FirstOrDefault();
            }
        }


        /// <summary>
        /// Get Particullar Record from the CompanyConfig by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public CompanyConfigDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                return (from u in context.Database.SqlQuery<CompanyConfigDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM CompanyConfig_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new CompanyConfigDTO
                        {
                            ID = u.ID,
                            CompanyID = u.CompanyID,
                            ScheduleDaysBefore = u.ScheduleDaysBefore,
                            OperationalHoursBefore = u.OperationalHoursBefore,
                            MileageBefore = u.MileageBefore,
                            ProjectAmountExceed = u.ProjectAmountExceed,
                            ProjectItemQuantitExceed = u.ProjectItemQuantitExceed,
                            ProjectItemAmountExceed = u.ProjectItemAmountExceed,
                            CostDecimalPoints = u.CostDecimalPoints,
                            QuantityDecimalPoints = u.QuantityDecimalPoints,
                            DateFormat = u.DateFormat,
                            DateFormatCSharp = u.DateFormatCSharp,
                            NOBackDays = u.NOBackDays,
                            NODaysAve = u.NODaysAve,
                            NOTimes = u.NOTimes,
                            MinPer = u.MinPer,
                            MaxPer = u.MaxPer,
                            CurrencySymbol = u.CurrencySymbol,
                            AEMTPndOrders = u.AEMTPndOrders,
                            AEMTPndRequisition = u.AEMTPndRequisition,
                            AEMTPndTransfer = u.AEMTPndTransfer,
                            AEMTSggstOrdCrt = u.AEMTSggstOrdCrt,
                            AEMTSggstOrdMin = u.AEMTSggstOrdMin,
                            AEMTAssetMntDue = u.AEMTAssetMntDue,
                            AEMTCycleCntItmMiss = u.AEMTCycleCntItmMiss,
                            AEMTCycleCount = u.AEMTCycleCount,
                            AEMTItemReceiveRpt = u.AEMTItemReceiveRpt,
                            AEMTItemStockOut = u.AEMTItemStockOut,
                            AEMTItemUsageRpt = u.AEMTItemUsageRpt,
                            AEMTToolsMntDue = u.AEMTToolsMntDue,
                            IsPackSlipRequired = u.IsPackSlipRequired,
                            NumberOfBackDaysToSyncOverPDA = u.NumberOfBackDaysToSyncOverPDA,
                        }).SingleOrDefault();
            }
        }

        
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE CompanyConfig SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);


                //Get Cached-Media
                IEnumerable<CompanyConfigDTO> ObjCache = CacheHelper<IEnumerable<CompanyConfigDTO>>.GetCacheItem("Cached_CompanyConfig_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<CompanyConfigDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<CompanyConfigDTO>>.AppendToCacheItem("Cached_CompanyConfig_" + CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }


    }
}


