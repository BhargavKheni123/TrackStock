using System;
using System.Linq;
using eTurns.DTO;
using System.Data;

namespace eTurns.DAL
{
    public partial class EnterPriseConfigDAL : eTurnsBaseDAL
    {
        public EnterPriseConfigDTO GetRecord(Int64 EnterPriseID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string qry = "SELECT A.* FROM EnterPriseConfig A WHERE A.EnterpriseID =" + EnterPriseID;

                return (from u in context.ExecuteStoreQuery<EnterPriseConfigDTO>(qry)
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
    }
}
