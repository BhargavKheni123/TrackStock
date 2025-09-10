using eTurns.DTO;
using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class EnterPriseConfigDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public EnterPriseConfigDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public EnterPriseConfigDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]
        public EnterPriseConfigDTO GetRecord(long EnterPriseID)
        {
            var param = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterPriseID) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Database.SqlQuery<EnterPriseConfigDTO>("exec GetEnterPriseConfigByEnterpriseIDPlain @EnterpriseID ", param)
                        select new EnterPriseConfigDTO
                        {
                            ID = u.ID,
                            EnterPriseID = u.EnterPriseID,
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
                            PasswordExpiryDays = u.PasswordExpiryDays,
                            NumberOfBackDaysToSyncOverPDA = u.NumberOfBackDaysToSyncOverPDA,
                            PasswordExpiryWarningDays = u.PasswordExpiryWarningDays,
                            PreviousLastAllowedPWD = u.PreviousLastAllowedPWD,
                            DisplayAgreePopupDays = u.DisplayAgreePopupDays ?? 0,
                            NotAllowedCharacter = u.NotAllowedCharacter
                        }).AsParallel().FirstOrDefault();
            }
        }

        public Int64 Insert(EnterPriseConfigDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                EnterPriseConfig obj = new EnterPriseConfig();
                obj.ID = 0;
                obj.EnterpriseID = objDTO.EnterPriseID;
                obj.ScheduleDaysBefore = objDTO.ScheduleDaysBefore;
                obj.OperationalHoursBefore = objDTO.OperationalHoursBefore;
                obj.MileageBefore = objDTO.MileageBefore;
                obj.ProjectAmountExceed = objDTO.ProjectAmountExceed;
                obj.ProjectItemQuantitExceed = objDTO.ProjectItemQuantitExceed;
                obj.ProjectItemAmountExceed = objDTO.ProjectItemAmountExceed;
                obj.CostDecimalPoints = objDTO.CostDecimalPoints;
                obj.QuantityDecimalPoints = objDTO.QuantityDecimalPoints;
                obj.DateFormat = objDTO.DateFormat;
                obj.DateFormatCSharp = objDTO.DateFormatCSharp;
                obj.NOBackDays = objDTO.NOBackDays;
                obj.NODaysAve = objDTO.NODaysAve;
                obj.NOTimes = objDTO.NOTimes;
                obj.MinPer = objDTO.MinPer;
                obj.MaxPer = objDTO.MaxPer;
                obj.CurrencySymbol = objDTO.CurrencySymbol;
                obj.AEMTPndOrders = objDTO.AEMTPndOrders;
                obj.AEMTPndRequisition = objDTO.AEMTPndRequisition;
                obj.AEMTPndTransfer = objDTO.AEMTPndTransfer;
                obj.AEMTSggstOrdCrt = objDTO.AEMTSggstOrdCrt;
                obj.AEMTSggstOrdMin = objDTO.AEMTSggstOrdMin;
                obj.AEMTAssetMntDue = objDTO.AEMTAssetMntDue;
                obj.AEMTCycleCntItmMiss = objDTO.AEMTCycleCntItmMiss;
                obj.AEMTCycleCount = objDTO.AEMTCycleCount;
                obj.AEMTItemReceiveRpt = objDTO.AEMTItemReceiveRpt;
                obj.AEMTItemStockOut = objDTO.AEMTItemStockOut;
                obj.AEMTItemUsageRpt = objDTO.AEMTItemUsageRpt;
                obj.AEMTToolsMntDue = objDTO.AEMTToolsMntDue;
                obj.GridRefreshTimeInSecond = objDTO.GridRefreshTimeInSecond;
                obj.PasswordExpiryDays = objDTO.PasswordExpiryDays;
                obj.DisplayAgreePopupDays = objDTO.DisplayAgreePopupDays;
                obj.NotAllowedCharacter = objDTO.NotAllowedCharacter;
                context.EnterPriseConfigs.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                return obj.ID;
            }

        }
        public bool Edit(EnterPriseConfigDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                EnterPriseConfig obj = new EnterPriseConfig();
                obj.ID = objDTO.ID;
                obj.EnterpriseID = objDTO.EnterPriseID;
                obj.ScheduleDaysBefore = objDTO.ScheduleDaysBefore;
                obj.OperationalHoursBefore = objDTO.OperationalHoursBefore;
                obj.MileageBefore = objDTO.MileageBefore;
                obj.ProjectAmountExceed = objDTO.ProjectAmountExceed;
                obj.ProjectItemQuantitExceed = objDTO.ProjectItemQuantitExceed;
                obj.ProjectItemAmountExceed = objDTO.ProjectItemAmountExceed;
                obj.CostDecimalPoints = objDTO.CostDecimalPoints;
                obj.QuantityDecimalPoints = objDTO.QuantityDecimalPoints;
                obj.DateFormat = objDTO.DateFormat;
                obj.DateFormatCSharp = objDTO.DateFormatCSharp;
                obj.NOBackDays = objDTO.NOBackDays;
                obj.NODaysAve = objDTO.NODaysAve;
                obj.NOTimes = objDTO.NOTimes;
                obj.MinPer = objDTO.MinPer;
                obj.MaxPer = objDTO.MaxPer;
                obj.CurrencySymbol = objDTO.CurrencySymbol;
                obj.AEMTPndOrders = objDTO.AEMTPndOrders;
                obj.AEMTPndRequisition = objDTO.AEMTPndRequisition;
                obj.AEMTPndTransfer = objDTO.AEMTPndTransfer;
                obj.AEMTSggstOrdCrt = objDTO.AEMTSggstOrdCrt;
                obj.AEMTSggstOrdMin = objDTO.AEMTSggstOrdMin;
                obj.AEMTAssetMntDue = objDTO.AEMTAssetMntDue;
                obj.AEMTCycleCntItmMiss = objDTO.AEMTCycleCntItmMiss;
                obj.AEMTCycleCount = objDTO.AEMTCycleCount;
                obj.AEMTItemReceiveRpt = objDTO.AEMTItemReceiveRpt;
                obj.AEMTItemStockOut = objDTO.AEMTItemStockOut;
                obj.AEMTItemUsageRpt = objDTO.AEMTItemUsageRpt;
                obj.AEMTToolsMntDue = objDTO.AEMTToolsMntDue;
                obj.GridRefreshTimeInSecond = objDTO.GridRefreshTimeInSecond;
                obj.PasswordExpiryDays = objDTO.PasswordExpiryDays;
                obj.NumberOfBackDaysToSyncOverPDA = objDTO.NumberOfBackDaysToSyncOverPDA;
                obj.PasswordExpiryWarningDays = objDTO.PasswordExpiryWarningDays;
                obj.PreviousLastAllowedPWD = objDTO.PreviousLastAllowedPWD;
                obj.DisplayAgreePopupDays = objDTO.DisplayAgreePopupDays;
                obj.NotAllowedCharacter = objDTO.NotAllowedCharacter;
                context.EnterPriseConfigs.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }

        #endregion
    }
}


