using eTurns.DTO;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class CompanyConfigDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public CompanyConfigDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public CompanyConfigDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]
        public CompanyConfigDTO GetCompanyConfigByCompanyID(Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<CompanyConfigDTO>("exec [GetCompanyConfigByCompanyID] @CompanyID", params1).FirstOrDefault();
            }
        }

        public Int64 Insert(CompanyConfigDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CompanyConfig obj = new CompanyConfig();
                obj.ID = 0;
                obj.CompanyID = objDTO.CompanyID;
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
                obj.IsPackSlipRequired = objDTO.IsPackSlipRequired;

                context.CompanyConfigs.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                return obj.ID;
            }

        }

        public bool Edit(CompanyConfigDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CompanyConfig obj = new CompanyConfig();
                obj.ID = objDTO.ID;
                obj.CompanyID = objDTO.CompanyID;
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
                obj.WeightDecimalPoints = objDTO.WeightDecimalPoints;
                obj.TurnsAvgDecimalPoints = objDTO.TurnsAvgDecimalPoints;
                obj.IsPackSlipRequired = objDTO.IsPackSlipRequired;
                obj.NumberOfBackDaysToSyncOverPDA = objDTO.NumberOfBackDaysToSyncOverPDA;

                context.CompanyConfigs.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }

        #endregion
    }
}