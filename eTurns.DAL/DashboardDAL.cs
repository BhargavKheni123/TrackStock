using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class DashboardDAL : eTurnsBaseDAL
    {
        #region [Globals]

        public int[] OrderStatuses = new int[] { (int)OrderStatus.UnSubmitted, (int)OrderStatus.Submitted, (int)OrderStatus.Approved, (int)OrderStatus.Transmitted, (int)OrderStatus.TransmittedIncomplete, (int)OrderStatus.TransmittedInCompletePastDue, (int)OrderStatus.TransmittedPastDue, (int)OrderStatus.Closed };

        #endregion

        #region [Class Constructor]

        public DashboardDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public DashboardDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]
        public DashboardAnalysisInfo UpdateAvgUsageByItemGUIDAfterTxn(long RoomId, long CompanyId, Guid ItemGUID, long UserId, long SessionUserId, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null)
        {
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            int _NumberDecimalDigits = 0;

            string CurrentRoomTimeZone = "UTC";
            if (objDashboardParameterDTO == null)
            {
                //if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["DashboardParams"] != null)
                //{
                //    objDashboardParameterDTO = (DashboardParameterDTO)HttpContext.Current.Session["DashboardParams"];
                //}
                //else
                {
                    objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
                }
            }
            if (objRegionalSettings == null)
            {
                //if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["eTurnsRegionInfoProp"] != null)
                //{
                //    objRegionalSettings = (eTurnsRegionInfo)HttpContext.Current.Session["eTurnsRegionInfoProp"];
                //}
                //else
                {
                    objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
                }
            }
            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
                _NumberDecimalDigits = objRegionalSettings.NumberDecimalDigits;
            }
            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.AUDayOfUsageToSample ?? 0) > 0)
                {
                    ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                    if (objItem != null)
                    {
                        DateTime? FromDate = CurrentTimeofTimeZone.AddDays(1).AddDays(objDashboardParameterDTO.AUDayOfUsageToSample.Value * (-1)).Date;
                        DateTime Todate = new DateTime(CurrentTimeofTimeZone.Year, CurrentTimeofTimeZone.Month, CurrentTimeofTimeZone.Day, 23, 59, 59);
                        FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                        Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                        double PullCost = 0;
                        double PullQuantity = 0;
                        double OrderedQty = 0;
                        double ReturnOrderedQty = 0;
                        double FinalOrderedQty = 0;

                        List<PullMaster> PullMasterList = null;
                        List<OrderDetailsDTO> lstOrderDetails = null;

                        switch (objDashboardParameterDTO.AUMeasureMethod)
                        {
                            case 1:
                                PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate);//context.Database.SqlQuery<PullMaster>("exec GetPullByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramsInner).ToList();
                                var transferList = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate,true);
                                double TransferCost = 0;

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullCost -= PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum();
                                }

                                if (transferList != null && transferList.Any() && transferList.Count() > 0)
                                {
                                    TransferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                                    TransferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                                }
                                //if (transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Any())
                                //{
                                //    TransferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                                //}

                                //if (transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Any())
                                //{
                                //    TransferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                                //}

                                var PullTransferCost = PullCost + TransferCost;

                                if (PullTransferCost > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = PullTransferCost / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                                    objItem.AverageUsage = objDashboardAnalysisInfo.CalculatedAverageUsage;
                                    objDashboardAnalysisInfo.CalculatedMinimun = objDashboardParameterDTO.MinMaxDayOfAverage * objDashboardAnalysisInfo.CalculatedAverageUsage;
                                    objDashboardAnalysisInfo.CalculatedMaximun = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax * objDashboardAnalysisInfo.CalculatedMinimun;
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = 0;
                                    objItem.AverageUsage = 0;
                                }
                                break;
                            case 2:
                                PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate); //context.Database.SqlQuery<PullMaster>("exec GetPullByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramsInnerCase).ToList();
                                var transfers = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate, false);
                                double transferedQty = 0;

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullQuantity -= (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                                }

                                if (transfers != null && transfers.Any() && transfers.Count() > 0)
                                {
                                    transferedQty = transfers.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                                    transferedQty -= transfers.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                                }

                                //if (transfers.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Any())
                                //{
                                //    transferedQty = transfers.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                                //}

                                //if (transfers.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Any())
                                //{
                                //    transferedQty -= transfers.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                                //}

                                var PullTransferQty = PullQuantity + transferedQty;
                                
                                if (PullTransferQty > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = PullTransferQty / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                                    objItem.AverageUsage = objDashboardAnalysisInfo.CalculatedAverageUsage;
                                    objDashboardAnalysisInfo.CalculatedMinimun = objDashboardParameterDTO.MinMaxDayOfAverage * objDashboardAnalysisInfo.CalculatedAverageUsage;
                                    objDashboardAnalysisInfo.CalculatedMaximun = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax * objDashboardAnalysisInfo.CalculatedMinimun;
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = 0;
                                    objItem.AverageUsage = 0;
                                }
                                break;
                            case 3:
                                lstOrderDetails = GetOrderByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate);//context.Database.SqlQuery<OrderDetailsDTO>("exec GetOrderByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramInnerCase).ToList();

                                if (lstOrderDetails.Any())
                                {
                                    OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                                    ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                                }

                                FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                                if (FinalOrderedQty > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = FinalOrderedQty / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                                    objItem.AverageUsage = objDashboardAnalysisInfo.CalculatedAverageUsage;
                                    objDashboardAnalysisInfo.CalculatedMinimun = objDashboardParameterDTO.MinMaxDayOfAverage * objDashboardAnalysisInfo.CalculatedAverageUsage;
                                    objDashboardAnalysisInfo.CalculatedMaximun = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax * objDashboardAnalysisInfo.CalculatedMinimun;
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = 0;
                                    objItem.AverageUsage = 0;
                                }
                                break;
                        }

                        if (UserId <= 0)
                        {
                            //TODO: Added By CP on 07-Sep-2017: Whatwhere action used in Item Update trigger for Audit Trail log
                            UserMasterDTO systemUser = CommonDAL.GeteTurnsSystemUser(base.DataBaseName);
                            if (systemUser != null && systemUser.ID > 0)
                                UserId = systemUser.ID;
                        }

                        if ((objItem.TrendingSetting ?? 0) == 2)
                        {
                            if (objItem.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false))
                            {

                                objItem.MinimumQuantity = Math.Round((objDashboardAnalysisInfo.CalculatedMinimun ?? 0), 0, MidpointRounding.AwayFromZero);
                                objItem.MaximumQuantity = Math.Round((objDashboardAnalysisInfo.CalculatedMaximun ?? 0), 0, MidpointRounding.AwayFromZero);

                                if (objItem.CriticalQuantity > objItem.MinimumQuantity)
                                    objItem.CriticalQuantity = objItem.MinimumQuantity;
                                if (objItem.MinimumQuantity > objItem.MaximumQuantity)
                                    objItem.MaximumQuantity = objItem.MinimumQuantity;
                            }
                            else
                            {
                                List<BinMaster> lstBinMaster = context.BinMasters.Where(t => t.ItemGUID == ItemGUID && t.IsDeleted == false).ToList();
                                foreach (BinMaster itemBin in lstBinMaster)
                                {
                                    itemBin.MinimumQuantity = Math.Round((objDashboardAnalysisInfo.CalculatedMinimun ?? 0), 0, MidpointRounding.AwayFromZero);
                                    itemBin.MaximumQuantity = Math.Round((objDashboardAnalysisInfo.CalculatedMaximun ?? 0), 0, MidpointRounding.AwayFromZero);

                                    if (itemBin.CriticalQuantity > itemBin.MinimumQuantity)
                                        itemBin.CriticalQuantity = itemBin.MinimumQuantity;
                                    if (itemBin.MinimumQuantity > itemBin.MaximumQuantity)
                                        itemBin.MaximumQuantity = itemBin.MinimumQuantity;

                                    itemBin.LastUpdated = DateTimeUtility.DateTimeNow;
                                    itemBin.EditedFrom = "DashboardDAL>UpdateAvgUsage";
                                    itemBin.LastUpdatedBy = UserId;
                                }
                            }
                        }                      

                        objItem.LastUpdatedBy = UserId;
                        //objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objItem.Updated = DateTimeUtility.DateTimeNow;
                        objItem.WhatWhereAction = "DashboardDAL>UpdateAvgUsage";

                        context.SaveChanges();
                        if ((objItem.TrendingSetting ?? 0) == 2)
                        {
                            //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objItem.GUID, UserId, "web", "DashboardDAL>UpdateAvgUsageByItemGUIDAfterTxn");
                            new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objItem.GUID, UserId, "web", "Dashbord >> Save Average Usage", SessionUserId);
                        }
                    }
                }
            }

            return objDashboardAnalysisInfo;
        }

        public void SetItemsAutoClassification(Guid ItemGUID, long RoomId, long CompanyId, long UserId, int calledfrom)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool RoomAutoClassification = false;
                Room objRoom = context.Rooms.FirstOrDefault(t => t.ID == RoomId);
                DashboardParameter objDashboardParameter = context.DashboardParameters.FirstOrDefault(t => t.RoomId == RoomId);

                if (objDashboardParameter == null)
                {
                    RoomAutoClassification = false;
                }
                else
                {
                    RoomAutoClassification = objDashboardParameter.AutoClassification;
                }
                ItemMaster objItem = context.ItemMasters.Where(t => t.GUID == ItemGUID).FirstOrDefault();

                if (objItem != null && objItem.IsAutoInventoryClassification && RoomAutoClassification)
                {
                    double Itemcost = (objItem.ExtendedCost ?? 0);
                    double ItemTurns = (objItem.Turns ?? 0);

                    if (objItem != null && objRoom != null)
                    {
                        InventoryClassificationMasterDAL objInventoryClassificationMasterDAL = new InventoryClassificationMasterDAL(base.DataBaseName);
                        List<InventoryClassificationMasterDTO> lstRoomClassifications = objInventoryClassificationMasterDAL.GetInventoryClassificationByRoomNormal(RoomId, CompanyId, false).ToList();

                        if (lstRoomClassifications != null && lstRoomClassifications.Any())
                        {
                            lstRoomClassifications = lstRoomClassifications.OrderBy(t => (t.RangeStart ?? 0)).ThenBy(t => ((t.RangeStart ?? 0) + (t.RangeEnd ?? 0))).ThenBy(t => t.ID).ToList();
                            InventoryClassificationMasterDTO objmax = lstRoomClassifications.FirstOrDefault(t => (t.RangeStart ?? 0) > 0 && (t.RangeEnd ?? 0) <= 0);

                            if (objmax != null)
                            {
                                objmax.RangeEnd = double.MaxValue;
                                lstRoomClassifications = lstRoomClassifications.Where(t => t.ID != objmax.ID).ToList();
                                lstRoomClassifications.Add(objmax);
                            }

                            if (objRoom.BaseOfInventory == 1) // Cost
                            {
                                InventoryClassificationMasterDTO objItemclass = lstRoomClassifications.FirstOrDefault(t => (t.RangeStart ?? 0) <= Itemcost && t.RangeEnd >= Itemcost);

                                if (objItemclass != null)
                                {
                                    objItem.InventoryClassification = objItemclass.ID;
                                }
                                else
                                {
                                    objItem.InventoryClassification = null;
                                }
                            }
                            else if (objRoom.BaseOfInventory == 2) //Turns
                            {
                                InventoryClassificationMasterDTO objItemclass = lstRoomClassifications.FirstOrDefault(t => (t.RangeStart ?? 0) <= ItemTurns && t.RangeEnd >= ItemTurns);

                                if (objItemclass != null)
                                {
                                    objItem.InventoryClassification = objItemclass.ID;
                                }
                                else
                                {
                                    objItem.InventoryClassification = null;
                                }
                            }
                        }
                        else
                        {
                            objItem.InventoryClassification = null;
                        }
                    }
                    context.SaveChanges();
                }
            }
        }

        public DashboardParameterDTO GetDashboardParameters(long? RoomId, long CompanyId)
        {
            //if (httpco)
            //{

            //}
            //return GetCachedDashboardParams(CompanyId).FirstOrDefault(t => t.RoomId == RoomId && t.CompanyId == CompanyId);
            DashboardParameterDTO objDashboardParameterDTO = new DashboardParameterDTO();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objDashboardParameterDTO = (from dp in context.DashboardParameters
                                            where dp.RoomId == RoomId && dp.CompanyId == CompanyId
                                            select new DashboardParameterDTO
                                            {
                                                AUDayOfUsageToSample = dp.AUDayOfUsageToSample,
                                                AUDaysOfDailyUsage = dp.AUDaysOfDailyUsage,
                                                AUMeasureMethod = dp.AUMeasureMethod,
                                                CompanyId = dp.CompanyId,
                                                CreatedBy = dp.CreatedBy,
                                                CreatedOn = dp.CreatedOn,
                                                ID = dp.ID,
                                                MinMaxDayOfAverage = dp.MinMaxDayOfAverage,
                                                MinMaxDayOfUsageToSample = dp.MinMaxDayOfUsageToSample,
                                                MinMaxMeasureMethod = dp.MinMaxMeasureMethod,
                                                MinMaxMinNumberOfTimesMax = dp.MinMaxMinNumberOfTimesMax,
                                                MinMaxOptValue1 = dp.MinMaxOptValue1,
                                                MinMaxOptValue2 = dp.MinMaxOptValue2,
                                                RoomId = dp.RoomId ?? 0,
                                                TurnsMonthsOfUsageToSample = dp.TurnsMonthsOfUsageToSample,
                                                TurnsDaysOfUsageToSample = dp.TurnsDaysOfUsageToSample,
                                                TurnsMeasureMethod = dp.TurnsMeasureMethod,
                                                UpdatedBy = dp.UpdatedBy,
                                                UpdatedOn = dp.UpdatedOn,
                                                PieChartmetricOn = dp.PieChartmetricOn,
                                                IsTrendingEnabled = dp.IsTrendingEnabled,
                                                GraphFromMonth = dp.GraphFromMonth,
                                                GraphFromYear = dp.GraphFromYear,
                                                GraphToMonth = dp.GraphToMonth,
                                                GraphToYear = dp.GraphToYear,
                                                TurnsCalculatedStockRoomTurn = dp.TurnsCalculatedStockRoomTurn,
                                                AUCalculatedDailyUsageOverSample = dp.AUCalculatedDailyUsageOverSample,
                                                AutoClassification = dp.AutoClassification,
                                                MonthlyAverageUsage = dp.MonthlyAverageUsage,
                                                AnnualCarryingCostPercent = dp.AnnualCarryingCostPercent,
                                                LargestAnnualCashSavings = dp.LargestAnnualCashSavings
                                            }).FirstOrDefault();

            }
            return objDashboardParameterDTO;
        }
        public DashboardParameterDTO SaveDashboardParameters(DashboardParameterDTO objDashboardParameterDTO, long SessionUserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                DashboardParameter objDashboardParameter = null;
                if (objDashboardParameterDTO.ID < 1)
                {
                    objDashboardParameter = new DashboardParameter();
                    objDashboardParameter.RoomId = objDashboardParameterDTO.RoomId;
                    objDashboardParameter.CompanyId = objDashboardParameterDTO.CompanyId;
                    objDashboardParameter.CreatedOn = objDashboardParameterDTO.CreatedOn;
                    objDashboardParameter.CreatedBy = objDashboardParameterDTO.CreatedBy;
                    objDashboardParameter.UpdatedOn = objDashboardParameterDTO.UpdatedOn;
                    objDashboardParameter.UpdatedBy = objDashboardParameterDTO.UpdatedBy;
                    objDashboardParameter.TurnsMeasureMethod = objDashboardParameterDTO.TurnsMeasureMethod;
                    objDashboardParameter.TurnsMonthsOfUsageToSample = objDashboardParameterDTO.TurnsMonthsOfUsageToSample;
                    objDashboardParameter.TurnsDaysOfUsageToSample = objDashboardParameterDTO.TurnsDaysOfUsageToSample;
                    objDashboardParameter.AUDayOfUsageToSample = objDashboardParameterDTO.AUDayOfUsageToSample;
                    objDashboardParameter.MonthlyAverageUsage = objDashboardParameterDTO.MonthlyAverageUsage;
                    objDashboardParameter.AUMeasureMethod = objDashboardParameterDTO.AUMeasureMethod;
                    objDashboardParameter.AUDaysOfDailyUsage = objDashboardParameterDTO.AUDaysOfDailyUsage;
                    objDashboardParameter.MinMaxMeasureMethod = objDashboardParameterDTO.MinMaxMeasureMethod;
                    objDashboardParameter.MinMaxDayOfUsageToSample = objDashboardParameterDTO.MinMaxDayOfUsageToSample;
                    objDashboardParameter.MinMaxDayOfAverage = objDashboardParameterDTO.MinMaxDayOfAverage;
                    objDashboardParameter.MinMaxMinNumberOfTimesMax = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax;
                    objDashboardParameter.MinMaxOptValue1 = objDashboardParameterDTO.MinMaxOptValue1;
                    objDashboardParameter.MinMaxOptValue2 = objDashboardParameterDTO.MinMaxOptValue2;
                    objDashboardParameter.IsTrendingEnabled = objDashboardParameterDTO.IsTrendingEnabled;
                    objDashboardParameter.PieChartmetricOn = objDashboardParameterDTO.PieChartmetricOn;
                    objDashboardParameter.GraphFromMonth = objDashboardParameterDTO.GraphFromMonth;
                    objDashboardParameter.GraphFromYear = objDashboardParameterDTO.GraphFromYear;
                    objDashboardParameter.GraphToMonth = objDashboardParameterDTO.GraphToMonth;
                    objDashboardParameter.GraphToYear = objDashboardParameterDTO.GraphToYear;
                    objDashboardParameter.AutoClassification = objDashboardParameterDTO.AutoClassification;
                    objDashboardParameter.AnnualCarryingCostPercent = objDashboardParameterDTO.AnnualCarryingCostPercent;
                    objDashboardParameter.LargestAnnualCashSavings = objDashboardParameterDTO.LargestAnnualCashSavings;
                    context.DashboardParameters.Add(objDashboardParameter);
                    context.SaveChanges();
                    objDashboardParameterDTO.ID = objDashboardParameter.ID;

                    objDashboardParameterDTO.TurnsCalculatedStockRoomTurn = CalculateStockRoomTurn(objDashboardParameterDTO.RoomId, objDashboardParameterDTO.CompanyId);
                    objDashboardParameterDTO.AUCalculatedDailyUsageOverSample = CalculateStockRoomAvgUsage(objDashboardParameterDTO.RoomId, objDashboardParameterDTO.CompanyId, objDashboardParameterDTO.UpdatedBy, SessionUserId);
                    if (objDashboardParameterDTO.AutoClassification)
                    {
                        ReclassifyAllItems(objDashboardParameterDTO.RoomId, objDashboardParameterDTO.CompanyId, objDashboardParameterDTO.CreatedBy);
                    }
                }
                else
                {
                    objDashboardParameter = context.DashboardParameters.FirstOrDefault(t => t.ID == objDashboardParameterDTO.ID);
                    if (objDashboardParameter != null)
                    {
                        bool Reclassfyitems = false;
                        bool RecalculateStockRoomTurn = false;
                        bool RecalculateStockRoomAvgUsage = false;

                        if (objDashboardParameterDTO.AutoClassification && !objDashboardParameter.AutoClassification)
                        {
                            Reclassfyitems = true;
                        }

                        if (objDashboardParameter.TurnsMeasureMethod != objDashboardParameterDTO.TurnsMeasureMethod
                            || objDashboardParameter.TurnsDaysOfUsageToSample != objDashboardParameterDTO.TurnsDaysOfUsageToSample)
                        {
                            RecalculateStockRoomTurn = true;
                        }

                        if (objDashboardParameter.AUDayOfUsageToSample != objDashboardParameterDTO.AUDayOfUsageToSample
                            || objDashboardParameter.AUMeasureMethod != objDashboardParameterDTO.AUMeasureMethod
                            || objDashboardParameter.AUDaysOfDailyUsage != objDashboardParameterDTO.AUDaysOfDailyUsage)
                        {
                            RecalculateStockRoomAvgUsage = true;
                        }

                        objDashboardParameter.RoomId = objDashboardParameterDTO.RoomId;
                        objDashboardParameter.CompanyId = objDashboardParameterDTO.CompanyId;
                        objDashboardParameter.UpdatedOn = objDashboardParameterDTO.UpdatedOn;
                        objDashboardParameter.UpdatedBy = objDashboardParameterDTO.UpdatedBy;
                        objDashboardParameter.TurnsMeasureMethod = objDashboardParameterDTO.TurnsMeasureMethod;
                        objDashboardParameter.TurnsMonthsOfUsageToSample = objDashboardParameterDTO.TurnsMonthsOfUsageToSample;
                        objDashboardParameter.TurnsDaysOfUsageToSample = objDashboardParameterDTO.TurnsDaysOfUsageToSample;
                        objDashboardParameter.AUDayOfUsageToSample = objDashboardParameterDTO.AUDayOfUsageToSample;
                        objDashboardParameter.MonthlyAverageUsage = objDashboardParameterDTO.MonthlyAverageUsage;
                        objDashboardParameter.AUMeasureMethod = objDashboardParameterDTO.AUMeasureMethod;
                        objDashboardParameter.AUDaysOfDailyUsage = objDashboardParameterDTO.AUDaysOfDailyUsage;
                        objDashboardParameter.MinMaxMeasureMethod = objDashboardParameterDTO.MinMaxMeasureMethod;
                        objDashboardParameter.MinMaxDayOfUsageToSample = objDashboardParameterDTO.MinMaxDayOfUsageToSample;
                        objDashboardParameter.MinMaxDayOfAverage = objDashboardParameterDTO.MinMaxDayOfAverage;
                        objDashboardParameter.MinMaxMinNumberOfTimesMax = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax;
                        objDashboardParameter.MinMaxOptValue1 = objDashboardParameterDTO.MinMaxOptValue1;
                        objDashboardParameter.MinMaxOptValue2 = objDashboardParameterDTO.MinMaxOptValue2;
                        objDashboardParameter.IsTrendingEnabled = objDashboardParameterDTO.IsTrendingEnabled;
                        objDashboardParameter.PieChartmetricOn = objDashboardParameterDTO.PieChartmetricOn;
                        objDashboardParameter.GraphFromMonth = objDashboardParameterDTO.GraphFromMonth;
                        objDashboardParameter.GraphFromYear = objDashboardParameterDTO.GraphFromYear;
                        objDashboardParameter.GraphToMonth = objDashboardParameterDTO.GraphToMonth;
                        objDashboardParameter.GraphToYear = objDashboardParameterDTO.GraphToYear;
                        objDashboardParameter.AutoClassification = objDashboardParameterDTO.AutoClassification;
                        objDashboardParameter.AnnualCarryingCostPercent = objDashboardParameterDTO.AnnualCarryingCostPercent;
                        objDashboardParameter.LargestAnnualCashSavings = objDashboardParameterDTO.LargestAnnualCashSavings;
                        context.SaveChanges();
                        if (Reclassfyitems)
                        {
                            ReclassifyAllItems(objDashboardParameterDTO.RoomId, objDashboardParameterDTO.CompanyId, objDashboardParameterDTO.CreatedBy);
                        }
                        if (RecalculateStockRoomTurn)
                        {
                            objDashboardParameterDTO.TurnsCalculatedStockRoomTurn = CalculateStockRoomTurn(objDashboardParameterDTO.RoomId, objDashboardParameterDTO.CompanyId);
                        }
                        if (RecalculateStockRoomAvgUsage)
                        {
                            objDashboardParameterDTO.AUCalculatedDailyUsageOverSample = CalculateStockRoomAvgUsage(objDashboardParameterDTO.RoomId, objDashboardParameterDTO.CompanyId, objDashboardParameterDTO.UpdatedBy, SessionUserId);
                        }
                    }
                }

                //if (HttpContext.Current != null && HttpContext.Current.Session != null)
                //{
                //    HttpContext.Current.Session["DashboardParams"] = objDashboardParameterDTO;
                //}
            }

            //DashParamCache(objDashboardParameterDTO);

            return objDashboardParameterDTO;
        }
        //private void DashParamCache(DashboardParameterDTO objDashboardParameterDTO)
        //{
        //    if (objDashboardParameterDTO.ID > 0)
        //    {
        //        //Get Cached-Media
        //        IEnumerable<DashboardParameterDTO> ObjCache = CacheHelper<IEnumerable<DashboardParameterDTO>>.GetCacheItem("Cached_DashboardParameter_" + objDashboardParameterDTO.EnterpriseId);
        //        if (ObjCache != null)
        //        {
        //            if (ObjCache.Any(t => t.ID == objDashboardParameterDTO.ID))
        //            {
        //                List<DashboardParameterDTO> objTemp = ObjCache.ToList();
        //                objTemp.RemoveAll(i => i.ID == objDashboardParameterDTO.ID);
        //                ObjCache = objTemp.AsEnumerable();

        //                List<DashboardParameterDTO> tempC = new List<DashboardParameterDTO>();
        //                tempC.Add(objDashboardParameterDTO);
        //                IEnumerable<DashboardParameterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //                CacheHelper<IEnumerable<DashboardParameterDTO>>.AppendToCacheItem("Cached_JobTypeMaster_" + objDashboardParameterDTO.EnterpriseId.ToString(), NewCache);
        //            }
        //            else
        //            {
        //                List<DashboardParameterDTO> tempC = new List<DashboardParameterDTO>();
        //                tempC.Add(objDashboardParameterDTO);
        //                IEnumerable<DashboardParameterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //                CacheHelper<IEnumerable<DashboardParameterDTO>>.AppendToCacheItem("Cached_JobTypeMaster_" + objDashboardParameterDTO.EnterpriseId.ToString(), NewCache);
        //            }

        //        }
        //    }
        //}
        public DashboardParameterDTO SaveDashboardTurnParameters(DashboardParameterDTO objDashboardParameterDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                DashboardParameter objDashboardParameter = null;
                if (objDashboardParameterDTO.ID < 1)
                {
                    objDashboardParameter = new DashboardParameter();
                    objDashboardParameter.RoomId = objDashboardParameterDTO.RoomId;
                    objDashboardParameter.CompanyId = objDashboardParameterDTO.CompanyId;
                    objDashboardParameter.CreatedOn = objDashboardParameterDTO.CreatedOn;
                    objDashboardParameter.CreatedBy = objDashboardParameterDTO.CreatedBy;
                    objDashboardParameter.UpdatedOn = objDashboardParameterDTO.UpdatedOn;
                    objDashboardParameter.UpdatedBy = objDashboardParameterDTO.UpdatedBy;
                    objDashboardParameter.TurnsMeasureMethod = objDashboardParameterDTO.TurnsMeasureMethod;
                    objDashboardParameter.TurnsMonthsOfUsageToSample = objDashboardParameterDTO.TurnsMonthsOfUsageToSample;
                    objDashboardParameter.TurnsDaysOfUsageToSample = objDashboardParameterDTO.TurnsDaysOfUsageToSample;
                    objDashboardParameter.AUDayOfUsageToSample = objDashboardParameterDTO.AUDayOfUsageToSample;
                    objDashboardParameter.MonthlyAverageUsage = objDashboardParameterDTO.MonthlyAverageUsage;
                    objDashboardParameter.AUMeasureMethod = objDashboardParameterDTO.AUMeasureMethod;
                    objDashboardParameter.AUDaysOfDailyUsage = objDashboardParameterDTO.AUDaysOfDailyUsage;
                    objDashboardParameter.MinMaxMeasureMethod = objDashboardParameterDTO.MinMaxMeasureMethod;
                    objDashboardParameter.MinMaxDayOfUsageToSample = objDashboardParameterDTO.MinMaxDayOfUsageToSample;
                    objDashboardParameter.MinMaxDayOfAverage = objDashboardParameterDTO.MinMaxDayOfAverage;
                    objDashboardParameter.MinMaxMinNumberOfTimesMax = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax;
                    objDashboardParameter.MinMaxOptValue1 = objDashboardParameterDTO.MinMaxOptValue1;
                    objDashboardParameter.MinMaxOptValue2 = objDashboardParameterDTO.MinMaxOptValue2;
                    objDashboardParameter.IsTrendingEnabled = objDashboardParameterDTO.IsTrendingEnabled;
                    objDashboardParameter.PieChartmetricOn = objDashboardParameterDTO.PieChartmetricOn;
                    objDashboardParameter.GraphFromMonth = objDashboardParameterDTO.GraphFromMonth;
                    objDashboardParameter.GraphFromYear = objDashboardParameterDTO.GraphFromYear;
                    objDashboardParameter.GraphToMonth = objDashboardParameterDTO.GraphToMonth;
                    objDashboardParameter.GraphToYear = objDashboardParameterDTO.GraphToYear;
                    objDashboardParameter.AutoClassification = objDashboardParameterDTO.AutoClassification;
                    context.DashboardParameters.Add(objDashboardParameter);
                    context.SaveChanges();
                    objDashboardParameterDTO.ID = objDashboardParameter.ID;
                }
                else
                {
                    objDashboardParameter = context.DashboardParameters.FirstOrDefault(t => t.ID == objDashboardParameterDTO.ID);
                    if (objDashboardParameter != null)
                    {
                        objDashboardParameter.RoomId = objDashboardParameterDTO.RoomId;
                        objDashboardParameter.CompanyId = objDashboardParameterDTO.CompanyId;
                        objDashboardParameter.UpdatedOn = objDashboardParameterDTO.UpdatedOn;
                        objDashboardParameter.UpdatedBy = objDashboardParameterDTO.UpdatedBy;
                        objDashboardParameter.TurnsMeasureMethod = objDashboardParameterDTO.TurnsMeasureMethod;
                        objDashboardParameter.TurnsMonthsOfUsageToSample = objDashboardParameterDTO.TurnsMonthsOfUsageToSample;
                        objDashboardParameter.TurnsDaysOfUsageToSample = objDashboardParameterDTO.TurnsDaysOfUsageToSample;
                    }
                }
                context.SaveChanges();
                objDashboardParameterDTO.TurnsCalculatedStockRoomTurn = CalculateStockRoomTurn(objDashboardParameterDTO.RoomId, objDashboardParameterDTO.CompanyId);
            }
            //DashParamCache(objDashboardParameterDTO);
            return objDashboardParameterDTO;
        }
        public DashboardParameterDTO SaveDashboardAUParameters(DashboardParameterDTO objDashboardParameterDTO, long SessionUserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                DashboardParameter objDashboardParameter = null;
                if (objDashboardParameterDTO.ID < 1)
                {
                    objDashboardParameter = new DashboardParameter();
                    objDashboardParameter.RoomId = objDashboardParameterDTO.RoomId;
                    objDashboardParameter.CompanyId = objDashboardParameterDTO.CompanyId;
                    objDashboardParameter.CreatedOn = objDashboardParameterDTO.CreatedOn;
                    objDashboardParameter.CreatedBy = objDashboardParameterDTO.CreatedBy;
                    objDashboardParameter.UpdatedOn = objDashboardParameterDTO.UpdatedOn;
                    objDashboardParameter.UpdatedBy = objDashboardParameterDTO.UpdatedBy;
                    objDashboardParameter.TurnsMeasureMethod = objDashboardParameterDTO.TurnsMeasureMethod;
                    objDashboardParameter.TurnsMonthsOfUsageToSample = objDashboardParameterDTO.TurnsMonthsOfUsageToSample;
                    objDashboardParameter.TurnsDaysOfUsageToSample = objDashboardParameterDTO.TurnsDaysOfUsageToSample;
                    objDashboardParameter.AUDayOfUsageToSample = objDashboardParameterDTO.AUDayOfUsageToSample;
                    objDashboardParameter.MonthlyAverageUsage = objDashboardParameterDTO.MonthlyAverageUsage;
                    objDashboardParameter.AUMeasureMethod = objDashboardParameterDTO.AUMeasureMethod;
                    objDashboardParameter.AUDaysOfDailyUsage = objDashboardParameterDTO.AUDaysOfDailyUsage;
                    objDashboardParameter.MinMaxMeasureMethod = objDashboardParameterDTO.MinMaxMeasureMethod;
                    objDashboardParameter.MinMaxDayOfUsageToSample = objDashboardParameterDTO.MinMaxDayOfUsageToSample;
                    objDashboardParameter.MinMaxDayOfAverage = objDashboardParameterDTO.MinMaxDayOfAverage;
                    objDashboardParameter.MinMaxMinNumberOfTimesMax = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax;
                    objDashboardParameter.MinMaxOptValue1 = objDashboardParameterDTO.MinMaxOptValue1;
                    objDashboardParameter.MinMaxOptValue2 = objDashboardParameterDTO.MinMaxOptValue2;
                    objDashboardParameter.IsTrendingEnabled = objDashboardParameterDTO.IsTrendingEnabled;
                    objDashboardParameter.PieChartmetricOn = objDashboardParameterDTO.PieChartmetricOn;
                    objDashboardParameter.GraphFromMonth = objDashboardParameterDTO.GraphFromMonth;
                    objDashboardParameter.GraphFromYear = objDashboardParameterDTO.GraphFromYear;
                    objDashboardParameter.GraphToMonth = objDashboardParameterDTO.GraphToMonth;
                    objDashboardParameter.GraphToYear = objDashboardParameterDTO.GraphToYear;
                    objDashboardParameter.AutoClassification = objDashboardParameterDTO.AutoClassification;
                    context.DashboardParameters.Add(objDashboardParameter);
                    context.SaveChanges();
                    objDashboardParameterDTO.ID = objDashboardParameter.ID;
                }
                else
                {
                    objDashboardParameter = context.DashboardParameters.FirstOrDefault(t => t.ID == objDashboardParameterDTO.ID);
                    if (objDashboardParameter != null)
                    {
                        objDashboardParameter.RoomId = objDashboardParameterDTO.RoomId;
                        objDashboardParameter.CompanyId = objDashboardParameterDTO.CompanyId;
                        objDashboardParameter.UpdatedOn = objDashboardParameterDTO.UpdatedOn;
                        objDashboardParameter.UpdatedBy = objDashboardParameterDTO.UpdatedBy;
                        objDashboardParameter.AUDayOfUsageToSample = objDashboardParameterDTO.AUDayOfUsageToSample;
                        objDashboardParameter.MonthlyAverageUsage = objDashboardParameterDTO.MonthlyAverageUsage;
                        objDashboardParameter.AUMeasureMethod = objDashboardParameterDTO.AUMeasureMethod;
                        objDashboardParameter.AUDaysOfDailyUsage = objDashboardParameterDTO.AUDaysOfDailyUsage;
                        context.SaveChanges();
                    }
                }

                //if (HttpContext.Current != null && HttpContext.Current.Session != null)
                //{
                //    HttpContext.Current.Session["DashboardParams"] = objDashboardParameterDTO;
                //}

                objDashboardParameterDTO.AUCalculatedDailyUsageOverSample = CalculateStockRoomAvgUsage(objDashboardParameterDTO.RoomId, objDashboardParameterDTO.CompanyId, objDashboardParameterDTO.UpdatedBy, SessionUserId);
            }


            //DashParamCache(objDashboardParameterDTO);
            return objDashboardParameterDTO;
        }
        public DashboardParameterDTO SaveDashboardOtherParameters(DashboardParameterDTO objDashboardParameterDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                DashboardParameter objDashboardParameter = null;
                if (objDashboardParameterDTO.ID < 1)
                {
                    objDashboardParameter = new DashboardParameter();
                    objDashboardParameter.RoomId = objDashboardParameterDTO.RoomId;
                    objDashboardParameter.CompanyId = objDashboardParameterDTO.CompanyId;
                    objDashboardParameter.CreatedOn = objDashboardParameterDTO.CreatedOn;
                    objDashboardParameter.CreatedBy = objDashboardParameterDTO.CreatedBy;
                    objDashboardParameter.UpdatedOn = objDashboardParameterDTO.UpdatedOn;
                    objDashboardParameter.UpdatedBy = objDashboardParameterDTO.UpdatedBy;
                    objDashboardParameter.TurnsMeasureMethod = objDashboardParameterDTO.TurnsMeasureMethod;
                    objDashboardParameter.TurnsMonthsOfUsageToSample = objDashboardParameterDTO.TurnsMonthsOfUsageToSample;
                    objDashboardParameter.TurnsDaysOfUsageToSample = objDashboardParameterDTO.TurnsDaysOfUsageToSample;
                    objDashboardParameter.AUDayOfUsageToSample = objDashboardParameterDTO.AUDayOfUsageToSample;
                    objDashboardParameter.MonthlyAverageUsage = objDashboardParameterDTO.MonthlyAverageUsage;
                    objDashboardParameter.AUMeasureMethod = objDashboardParameterDTO.AUMeasureMethod;
                    objDashboardParameter.AUDaysOfDailyUsage = objDashboardParameterDTO.AUDaysOfDailyUsage;
                    objDashboardParameter.MinMaxMeasureMethod = objDashboardParameterDTO.MinMaxMeasureMethod;
                    objDashboardParameter.MinMaxDayOfUsageToSample = objDashboardParameterDTO.MinMaxDayOfUsageToSample;
                    objDashboardParameter.MinMaxDayOfAverage = objDashboardParameterDTO.MinMaxDayOfAverage;
                    objDashboardParameter.MinMaxMinNumberOfTimesMax = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax;
                    objDashboardParameter.MinMaxOptValue1 = objDashboardParameterDTO.MinMaxOptValue1;
                    objDashboardParameter.MinMaxOptValue2 = objDashboardParameterDTO.MinMaxOptValue2;
                    objDashboardParameter.IsTrendingEnabled = objDashboardParameterDTO.IsTrendingEnabled;
                    objDashboardParameter.PieChartmetricOn = objDashboardParameterDTO.PieChartmetricOn;
                    objDashboardParameter.GraphFromMonth = objDashboardParameterDTO.GraphFromMonth;
                    objDashboardParameter.GraphFromYear = objDashboardParameterDTO.GraphFromYear;
                    objDashboardParameter.GraphToMonth = objDashboardParameterDTO.GraphToMonth;
                    objDashboardParameter.GraphToYear = objDashboardParameterDTO.GraphToYear;
                    objDashboardParameter.AutoClassification = objDashboardParameterDTO.AutoClassification;
                    context.DashboardParameters.Add(objDashboardParameter);
                    context.SaveChanges();
                    objDashboardParameterDTO.ID = objDashboardParameter.ID;
                    if (objDashboardParameterDTO.AutoClassification)
                    {
                        ReclassifyAllItems(objDashboardParameterDTO.RoomId, objDashboardParameterDTO.CompanyId, objDashboardParameterDTO.CreatedBy);
                    }

                    //if (HttpContext.Current != null && HttpContext.Current.Session != null)
                    //{
                    //    HttpContext.Current.Session["DashboardParams"] = objDashboardParameterDTO;
                    //}
                }
                else
                {
                    objDashboardParameter = context.DashboardParameters.FirstOrDefault(t => t.ID == objDashboardParameterDTO.ID);
                    if (objDashboardParameter != null)
                    {
                        bool Reclassfyitems = false;
                        if (objDashboardParameterDTO.AutoClassification && !objDashboardParameter.AutoClassification)
                        {
                            Reclassfyitems = true;
                        }

                        objDashboardParameter.RoomId = objDashboardParameterDTO.RoomId;
                        objDashboardParameter.CompanyId = objDashboardParameterDTO.CompanyId;
                        objDashboardParameter.UpdatedOn = objDashboardParameterDTO.UpdatedOn;
                        objDashboardParameter.UpdatedBy = objDashboardParameterDTO.UpdatedBy;
                        objDashboardParameter.IsTrendingEnabled = objDashboardParameterDTO.IsTrendingEnabled;
                        objDashboardParameter.PieChartmetricOn = objDashboardParameterDTO.PieChartmetricOn;
                        objDashboardParameter.GraphFromMonth = objDashboardParameterDTO.GraphFromMonth;
                        objDashboardParameter.GraphFromYear = objDashboardParameterDTO.GraphFromYear;
                        objDashboardParameter.GraphToMonth = objDashboardParameterDTO.GraphToMonth;
                        objDashboardParameter.GraphToYear = objDashboardParameterDTO.GraphToYear;
                        objDashboardParameter.AutoClassification = objDashboardParameterDTO.AutoClassification;
                        context.SaveChanges();

                        if (Reclassfyitems)
                        {
                            ReclassifyAllItems(objDashboardParameterDTO.RoomId, objDashboardParameterDTO.CompanyId, objDashboardParameterDTO.CreatedBy);
                        }

                        //if (HttpContext.Current != null && HttpContext.Current.Session != null)
                        //{
                        //var dashboardParameter = (DashboardParameterDTO)HttpContext.Current.Session["DashboardParams"];
                        //if (dashboardParameter != null)
                        //{
                        //    dashboardParameter.RoomId = objDashboardParameterDTO.RoomId;
                        //    dashboardParameter.CompanyId = objDashboardParameterDTO.CompanyId;
                        //    dashboardParameter.UpdatedOn = objDashboardParameterDTO.UpdatedOn;
                        //    dashboardParameter.UpdatedBy = objDashboardParameterDTO.UpdatedBy;
                        //    dashboardParameter.IsTrendingEnabled = objDashboardParameterDTO.IsTrendingEnabled;
                        //    dashboardParameter.PieChartmetricOn = objDashboardParameterDTO.PieChartmetricOn;
                        //    dashboardParameter.GraphFromMonth = objDashboardParameterDTO.GraphFromMonth;
                        //    dashboardParameter.GraphFromYear = objDashboardParameterDTO.GraphFromYear;
                        //    dashboardParameter.GraphToMonth = objDashboardParameterDTO.GraphToMonth;
                        //    dashboardParameter.GraphToYear = objDashboardParameterDTO.GraphToYear;
                        //    dashboardParameter.AutoClassification = objDashboardParameterDTO.AutoClassification;
                        //    HttpContext.Current.Session["DashboardParams"] = dashboardParameter;
                        //}
                        //}
                    }
                }


            }
            //DashParamCache(objDashboardParameterDTO);
            return objDashboardParameterDTO;
        }
        public DashboardParameterDTO SaveDashboardMinMaxParameters(DashboardParameterDTO objDashboardParameterDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                DashboardParameter objDashboardParameter = null;
                if (objDashboardParameterDTO.ID < 1)
                {
                    objDashboardParameter = new DashboardParameter();
                    objDashboardParameter.RoomId = objDashboardParameterDTO.RoomId;
                    objDashboardParameter.CompanyId = objDashboardParameterDTO.CompanyId;
                    objDashboardParameter.CreatedOn = objDashboardParameterDTO.CreatedOn;
                    objDashboardParameter.CreatedBy = objDashboardParameterDTO.CreatedBy;
                    objDashboardParameter.UpdatedOn = objDashboardParameterDTO.UpdatedOn;
                    objDashboardParameter.UpdatedBy = objDashboardParameterDTO.UpdatedBy;
                    objDashboardParameter.TurnsMeasureMethod = objDashboardParameterDTO.TurnsMeasureMethod;
                    objDashboardParameter.TurnsMonthsOfUsageToSample = objDashboardParameterDTO.TurnsMonthsOfUsageToSample;
                    objDashboardParameter.TurnsDaysOfUsageToSample = objDashboardParameterDTO.TurnsDaysOfUsageToSample;
                    objDashboardParameter.AUDayOfUsageToSample = objDashboardParameterDTO.AUDayOfUsageToSample;
                    objDashboardParameter.MonthlyAverageUsage = objDashboardParameterDTO.MonthlyAverageUsage;
                    objDashboardParameter.AUMeasureMethod = objDashboardParameterDTO.AUMeasureMethod;
                    objDashboardParameter.AUDaysOfDailyUsage = objDashboardParameterDTO.AUDaysOfDailyUsage;
                    objDashboardParameter.MinMaxMeasureMethod = objDashboardParameterDTO.MinMaxMeasureMethod;
                    objDashboardParameter.MinMaxDayOfUsageToSample = objDashboardParameterDTO.MinMaxDayOfUsageToSample;
                    objDashboardParameter.MinMaxDayOfAverage = objDashboardParameterDTO.MinMaxDayOfAverage;
                    objDashboardParameter.MinMaxMinNumberOfTimesMax = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax;
                    objDashboardParameter.MinMaxOptValue1 = objDashboardParameterDTO.MinMaxOptValue1;
                    objDashboardParameter.MinMaxOptValue2 = objDashboardParameterDTO.MinMaxOptValue2;
                    objDashboardParameter.IsTrendingEnabled = objDashboardParameterDTO.IsTrendingEnabled;
                    objDashboardParameter.PieChartmetricOn = objDashboardParameterDTO.PieChartmetricOn;
                    objDashboardParameter.GraphFromMonth = objDashboardParameterDTO.GraphFromMonth;
                    objDashboardParameter.GraphFromYear = objDashboardParameterDTO.GraphFromYear;
                    objDashboardParameter.GraphToMonth = objDashboardParameterDTO.GraphToMonth;
                    objDashboardParameter.GraphToYear = objDashboardParameterDTO.GraphToYear;
                    objDashboardParameter.AutoClassification = objDashboardParameterDTO.AutoClassification;
                    objDashboardParameter.AnnualCarryingCostPercent = objDashboardParameterDTO.AnnualCarryingCostPercent;
                    objDashboardParameter.LargestAnnualCashSavings = objDashboardParameterDTO.LargestAnnualCashSavings;
                    context.DashboardParameters.Add(objDashboardParameter);
                    context.SaveChanges();
                    objDashboardParameterDTO.ID = objDashboardParameter.ID;
                }
                else
                {
                    objDashboardParameter = context.DashboardParameters.FirstOrDefault(t => t.ID == objDashboardParameterDTO.ID);
                    if (objDashboardParameter != null)
                    {
                        objDashboardParameter.RoomId = objDashboardParameterDTO.RoomId;
                        objDashboardParameter.CompanyId = objDashboardParameterDTO.CompanyId;
                        objDashboardParameter.UpdatedOn = objDashboardParameterDTO.UpdatedOn;
                        objDashboardParameter.UpdatedBy = objDashboardParameterDTO.UpdatedBy;
                        objDashboardParameter.MinMaxMeasureMethod = objDashboardParameterDTO.MinMaxMeasureMethod;
                        objDashboardParameter.MinMaxDayOfUsageToSample = objDashboardParameterDTO.MinMaxDayOfUsageToSample;
                        objDashboardParameter.MinMaxDayOfAverage = objDashboardParameterDTO.MinMaxDayOfAverage;
                        objDashboardParameter.MinMaxMinNumberOfTimesMax = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax;
                        objDashboardParameter.MinMaxOptValue1 = objDashboardParameterDTO.MinMaxOptValue1;
                        objDashboardParameter.MinMaxOptValue2 = objDashboardParameterDTO.MinMaxOptValue2;
                        objDashboardParameter.AnnualCarryingCostPercent = objDashboardParameterDTO.AnnualCarryingCostPercent;
                        objDashboardParameter.LargestAnnualCashSavings = objDashboardParameterDTO.LargestAnnualCashSavings;
                        context.SaveChanges();
                    }
                }
            }
            //DashParamCache(objDashboardParameterDTO);
            return objDashboardParameterDTO;
        }
        public double CalculateStockRoomAvgUsage(long RoomId, long CompanyId, long UserId, long SessionUserId)
        {
            DashboardParameterDTO objDashboardParameterDTO = new DashboardParameterDTO();
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            double TotalAvgUsage = 0;
            long TotalItems = 0;
            double StockRoomAvg = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ItemMaster> lstItems = new List<ItemMaster>();
                lstItems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).ToList();
                TotalItems = lstItems.Count;
                foreach (var item in lstItems)
                {

                    objDashboardAnalysisInfo = objDashboardDAL.UpdateAvgUsageByItemGUIDAfterTxn(RoomId, CompanyId, item.GUID, UserId, SessionUserId, null);
                    TotalAvgUsage += objDashboardAnalysisInfo.CalculatedAverageUsage;
                }
                if (TotalItems > 0)
                {
                    StockRoomAvg = TotalAvgUsage / TotalItems;
                }
                objDashboardParameterDTO.TurnsCalculatedStockRoomTurn = StockRoomAvg;
                if (context.DashboardParameters.FirstOrDefault(t => t.RoomId == RoomId && t.CompanyId == CompanyId) != null)
                {
                    context.DashboardParameters.FirstOrDefault(t => t.RoomId == RoomId && t.CompanyId == CompanyId).AUCalculatedDailyUsageOverSample = StockRoomAvg;
                }
                context.SaveChanges();
            }
            return StockRoomAvg;
        }
        public List<MinMaxDataTableInfo> GetMinMaxTable(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, int AutoMinPercent, int AutoMaxPercent)
        {
            List<MinMaxDataTableInfo> lstItems = new List<MinMaxDataTableInfo>();
            TotalCount = 0;
            MinMaxDataTableInfo objMinMaxDataTableInfo = new MinMaxDataTableInfo();
            DataSet dsMinMaxItems = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsMinMaxItems = SqlHelper.ExecuteDataset(EturnsConnection, "MinMaxDashboardTable", StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomID, CompanyID);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                dsMinMaxItems = SqlHelper.ExecuteDataset(EturnsConnection, "MinMaxDashboardTable", StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomID, CompanyID);
            }
            else
            {
                dsMinMaxItems = SqlHelper.ExecuteDataset(EturnsConnection, "MinMaxDashboardTable", StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomID, CompanyID);
            }
            if (dsMinMaxItems != null && dsMinMaxItems.Tables.Count > 0)
            {
                DataTable dtMinMaxItems = dsMinMaxItems.Tables[0];
                if (dtMinMaxItems != null && dtMinMaxItems.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtMinMaxItems.Rows[0]["TotalRecords"]);
                    //lstItems = dtMinMaxItems.DataTableToList<MinMaxDataTableInfo>();
                    lstItems = (from dr in dtMinMaxItems.AsEnumerable()
                                select new MinMaxDataTableInfo
                                {
                                    TurnsAvgDecimalPoints = dr.Field<int>("TurnsAvgDecimalPoints"),
                                    NumberDecimalDigits = dr.Field<int>("NumberDecimalDigits"),
                                    IsTrendingEnabledOnItem = dr.Field<bool>("IsTrendingEnabledOnItem"),
                                    IsTrendingEnabledOnRoom = dr.Field<bool>("IsTrendingEnabledOnRoom"),
                                    InventoryClassification = dr.Field<string>("InventoryClassification"),
                                    ItemTrendingSetting = dr.Field<byte>("ItemTrendingSetting"),
                                    IsItemLevelMinMaxQtyRequired = dr.Field<bool?>("IsItemLevelMinMaxQtyRequired"),
                                    //ItemLocations = dr.Field<string>("ItemLocations"),
                                    MinSliderValue = AutoMinPercent,
                                    MaxSliderValue = AutoMaxPercent,
                                    ID = dr.Field<long>("ID"),
                                    GUID = dr.Field<Guid>("GUID"),
                                    ItemNumber = dr.Field<string>("ItemNumber"),
                                    Description = dr.Field<string>("Description"),
                                    Category = dr.Field<string>("Category"),
                                    SupplierName = dr.Field<string>("SupplierName"),
                                    ManufacturerNumber = dr.Field<string>("ManufacturerNumber"),
                                    Manufacturer = dr.Field<string>("Manufacturer"),
                                    SupplierPartNo = dr.Field<string>("SupplierPartNo"),
                                    BinNumber = dr.Field<string>("BinNumber"),
                                    BinID = dr.Field<long>("BinID"),
                                    AverageCost = dr.Field<double>("AverageCost"),
                                    OnHandQuantity = dr.Field<double>("OnHandQuantity"),
                                    CriticalQuantity = dr.Field<double>("CriticalQuantity"),
                                    MinimumQuantity = dr.Field<double>("MinimumQuantity"),
                                    MaximumQuantity = dr.Field<double>("MaximumQuantity"),
                                    PullQuantity = dr.Field<double>("PullQuantity"),
                                    PullCost = dr.Field<double>("PullCost"),
                                    ApprovedQuantity = dr.Field<double>("ApprovedQuantity"),
                                    RequestedQuantity = dr.Field<double>("RequestedQuantity"),
                                    ReceivedQuantity = dr.Field<double>("ReceivedQuantity"),
                                    retApprovedQuantity = dr.Field<double>("retApprovedQuantity"),
                                    retRequestedQuantity = dr.Field<double>("retRequestedQuantity"),
                                    retReceivedQuantity = dr.Field<double>("retReceivedQuantity"),
                                    ItemInventoryValue = dr.Field<double>("ItemInventoryValue"),
                                    DayOfUsageToSample = dr.Field<int>("DayOfUsageToSample"),
                                    MinMaxDayOfAverage = dr.Field<int>("MinMaxDayOfAverage"),
                                    MinMaxMeasureMethod = dr.Field<byte>("MinMaxMeasureMethod"),
                                    MinMaxMinNumberOfTimesMax = dr.Field<double>("MinMaxMinNumberOfTimesMax"),
                                    MinMaxOptValue1 = dr.Field<int>("MinMaxOptValue1"),
                                    MinMaxOptValue2 = dr.Field<int>("MinMaxOptValue2"),
                                    QuantumFromDate = dr.Field<DateTime>("QuantumFromDate"),
                                    QuantumToDate = dr.Field<DateTime>("QuantumToDate"),
                                    CostUOMValue = (dr.Field<int?>("CostUOMValue")) ?? 1,
                                    Cost = (dr.Field<double?>("Cost")) ?? 0,
                                    DefaultReorderQuantity = (dr.Field<double?>("DefaultReorderQuantity")) ?? 0,
                                    IsActive = dr.Field<string>("ItemActive"),
                                    DateCreated = dr.Field<string>("DateCreated"),
                                    IsEnforceDefaultReorderQuantity = dr.Field<bool?>("IsEnforceDefaultReorderQuantity"),
                                    QtyUntilOrder = dr.Field<double>("QtyUntilOrder"),
                                    TrendingSetting = dr.Field<byte>("TrendingSetting"),

                                    UDF1 = dr.Field<string>("UDF1"),
                                    UDF2 = dr.Field<string>("UDF2"),
                                    UDF3 = dr.Field<string>("UDF3"),
                                    UDF4 = dr.Field<string>("UDF4"),
                                    UDF5 = dr.Field<string>("UDF5"),
                                    UDF6 = dr.Field<string>("UDF6"),
                                    UDF7 = dr.Field<string>("UDF7"),
                                    UDF8 = dr.Field<string>("UDF8"),
                                    UDF9 = dr.Field<string>("UDF9"),
                                    UDF10 = dr.Field<string>("UDF10"),
                                    LeadTimeInDays = dr.Field<int>("LeadTimeInDays"),
                                    ItemTrackingType = dr.Field<int>("ItemTrackingType"),
                                    ItemType = dr.Field<int>("ItemTypeValue"),
                                    ItemStockStatus = dr.Field<int>("ItemStockStatus"),
                                    ExtendedCost = (dr.Field<double?>("ExtendedCost")) ?? 0,
                                    DateUpdated = dr.Field<DateTime>("DateUpdated"),
                                    Created = dr.Field<DateTime>("Created"),
                                    CreatedByUser = dr.Field<string>("CreatedByUser"),
                                    LastUpdatedByUser = dr.Field<string>("LastUpdatedByUser"),
                                    ItemExtendedCostValue = (dr.Field<double?>("ItemExtendedCostValue")) ?? 0,
                                    AverageOnHandQuantity = dr.Field<double>("AverageOnHandQuantity"),
                                    AverageExtendedCost = dr.Field<double>("AverageExtendedCost")
                                }).ToList();
                }
            }

            return lstItems.OrderBy(sortColumnName).ToList();
        }

        public List<TurnsDataTableInfo> GetTurnsTable(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID)
        {
            List<TurnsDataTableInfo> lstItems = new List<TurnsDataTableInfo>();
            TotalCount = 0;
            TurnsDataTableInfo objTurnsDataTableInfo = new TurnsDataTableInfo();
            DataSet dsMinMaxItems = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstItems;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            dsMinMaxItems = SqlHelper.ExecuteDataset(EturnsConnection, "TurnsDashboardTable", StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomID, CompanyID);

            if (dsMinMaxItems != null && dsMinMaxItems.Tables.Count > 0)
            {
                DataTable dtMinMaxItems = dsMinMaxItems.Tables[0];

                if (dtMinMaxItems != null && dtMinMaxItems.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtMinMaxItems.Rows[0]["TotalRecords"]);
                    lstItems = DataTableHelper.ToList<TurnsDataTableInfo>(dtMinMaxItems);
                }
            }

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                lstItems = (from itm in lstItems
                            where (itm.ItemNumber ?? string.Empty).ToLower().Contains(SearchTerm.ToLower()) || (itm.Description ?? string.Empty).ToLower().Contains(SearchTerm.ToLower())
                            select itm).ToList();
                if (lstItems != null)
                {
                    TotalCount = lstItems.Count;
                }
            }

            return lstItems.OrderBy(sortColumnName).ToList();
        }

        public List<TurnsDataTableInfo> GetTurnsTableForExport(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, string Guids)
        {
            List<TurnsDataTableInfo> lstItems = new List<TurnsDataTableInfo>();
            TotalCount = 0;
            TurnsDataTableInfo objTurnsDataTableInfo = new TurnsDataTableInfo();
            DataSet dsMinMaxItems = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            dsMinMaxItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetTurnsTableForExport", StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomID, CompanyID, Guids);

            if (dsMinMaxItems != null && dsMinMaxItems.Tables.Count > 0)
            {
                DataTable dtMinMaxItems = dsMinMaxItems.Tables[0];

                if (dtMinMaxItems != null && dtMinMaxItems.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtMinMaxItems.Rows[0]["TotalRecords"]);
                    lstItems = DataTableHelper.ToList<TurnsDataTableInfo>(dtMinMaxItems);
                }
            }

            return lstItems.OrderBy(sortColumnName).ToList();
        }

        public List<MinMaxDataTableInfo> GetMinMaxFilterdTable(string sSearch, int iDisplayStart, int iDisplayLength, string sortColumnName, List<MinMaxDataTableInfo> DataFromDB)
        {
            if (DataFromDB != null && DataFromDB.Count > 0 && !string.IsNullOrWhiteSpace(sSearch))
            {
                DataFromDB = (from itm in DataFromDB
                              where (itm.ItemNumber ?? string.Empty).ToLower().Contains(sSearch.ToLower())
                                    || (itm.Description ?? string.Empty).ToLower().Contains(sSearch.ToLower())
                                    || (itm.SupplierName ?? string.Empty).ToLower().Contains(sSearch.ToLower())
                                    || (itm.SupplierPartNo ?? string.Empty).ToLower().Contains(sSearch.ToLower())
                                    || (itm.Manufacturer ?? string.Empty).ToLower().Contains(sSearch.ToLower())
                                    || (itm.ManufacturerNumber ?? string.Empty).ToLower().Contains(sSearch.ToLower())
                                    || (itm.ItemLocations ?? string.Empty).ToLower().Contains(sSearch.ToLower())
                              select itm).ToList();
            }
            return DataFromDB.OrderBy(sortColumnName).ToList();
        }
        public double CalculateStockRoomTurn(long RoomId, long CompanyId)
        {
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            DashboardParameterDTO objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);

            return 0;
            //if (objDashboardParameterDTO != null)
            //{
            //    if ((objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0)
            //    {
            //        switch (objDashboardParameterDTO.TurnsMeasureMethod.Value)
            //        {
            //            case 1:
            //                break;
            //            case 2:
            //                break;
            //            case 3:
            //                break;
            //        }
            //    }
            //}

            //DashboardParameterDTO objDashboardParameterDTO = new DashboardParameterDTO();
            //DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            //DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            //double TotalTurn = 0;
            //long TotalItems = 0;
            //double StockRoomTurn = 0;
            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    List<ItemMaster> lstItems = new List<ItemMaster>();
            //    lstItems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).ToList();
            //    TotalItems = lstItems.Count;
            //    foreach (var item in lstItems)
            //    {
            //        objDashboardAnalysisInfo = objDashboardDAL.UpdateTurnsByItemGUID(RoomId, CompanyId, item.GUID);
            //        TotalTurn += objDashboardAnalysisInfo.CalculatedTurn;
            //    }
            //    if (TotalItems > 0)
            //    {
            //        StockRoomTurn = TotalTurn / TotalItems;
            //    }
            //    objDashboardParameterDTO.TurnsCalculatedStockRoomTurn = StockRoomTurn;
            //    if (context.DashboardParameters.FirstOrDefault(t => t.RoomId == RoomId && t.CompanyId == CompanyId) != null)
            //    {
            //        context.DashboardParameters.FirstOrDefault(t => t.RoomId == RoomId && t.CompanyId == CompanyId).TurnsCalculatedStockRoomTurn = StockRoomTurn;
            //    }
            //    context.SaveChanges();
            //}
            //return StockRoomTurn;
        }

        public bool CalculateMonthisedItemAnalytics(long RoomId, long CompanyId, int ForMonth, int ForYear, out string ExceptionString, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null)
        {
            try
            {
                ExceptionString = "";
                DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                if (objRegionalSettings == null)
                {
                    objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
                }

                if (objDashboardParameterDTO == null)
                {
                    objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
                }
                string CurrentRoomTimeZone = "UTC";
                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName;
                    if (string.IsNullOrWhiteSpace(CurrentRoomTimeZone))
                    {
                        CurrentRoomTimeZone = "UTC";
                    }
                }

                DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IQueryable<ItemMaster> lstItems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false);
                    if (lstItems != null && lstItems.Any())
                    {
                        foreach (var objItem in lstItems)
                        {
                            try
                            {
                                double PullCost = 0;
                                double ItemInventoryValue = 0;
                                double OnHandQuantity = 0;
                                double PullQuantity = 0;
                                double OrderedQty = 0;
                                double ReturnOrderedQty = 0;
                                double FinalOrderedQty = 0;
                                int daysinmonth = DateTime.DaysInMonth(ForYear, ForMonth);
                                DateTime? FromDate = new DateTime(ForYear, ForMonth, 1, 0, 0, 0);
                                DateTime Todate = new DateTime(ForYear, ForMonth, daysinmonth, 23, 59, 0);
                                FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                                Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                                double minQty = objItem.MinimumQuantity;
                                double maxQty = objItem.MaximumQuantity;
                                BinMasterDAL objBinMaster = new BinMasterDAL(base.DataBaseName);
                                InventoryAnalysisMonthWise objInventoryAnalysisMonthWise = context.InventoryAnalysisMonthWises.FirstOrDefault(t => t.RoomId == RoomId && t.CompanyId == CompanyId && t.ItemGUID == objItem.GUID && t.CalculationMonth == ForMonth && t.CalculationYear == ForYear);
                                int TobeAdded = 0;
                                if (objInventoryAnalysisMonthWise == null)
                                {
                                    objInventoryAnalysisMonthWise = new InventoryAnalysisMonthWise();
                                    TobeAdded = 1;
                                }
                                objInventoryAnalysisMonthWise.ItemGUID = objItem.GUID;
                                objInventoryAnalysisMonthWise.CalculationDate = DateTime.UtcNow;
                                objInventoryAnalysisMonthWise.CalculationFor = 1;
                                objInventoryAnalysisMonthWise.CalculationMonth = ForMonth;
                                objInventoryAnalysisMonthWise.CalculationYear = ForYear;
                                objInventoryAnalysisMonthWise.CompanyId = objItem.CompanyID ?? 0;
                                objInventoryAnalysisMonthWise.RoomId = objItem.Room ?? 0;

                                if (objItem.IsItemLevelMinMaxQtyRequired == false)
                                {
                                    //List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID)).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber).ToList();
                                    List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID), false, string.Empty, null, null).OrderBy(x => x.BinNumber).ToList();
                                    //List<BinMasterDTO> Itemlocs = objBinMaster.GetItemLocation(RoomId, CompanyId, false, false, (objItem.GUID),0,null,false).OrderBy(x => x.BinNumber).ToList();//.Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber).ToList();
                                    if (Itemlocs.Count > 0)
                                    {
                                        minQty = Itemlocs.First().MinimumQuantity ?? 0;
                                        maxQty = Itemlocs.First().MaximumQuantity ?? 0;
                                    }
                                }


                                //double DurationDiviser = 12 / objDashboardParameterDTO.TurnsMonthsOfUsageToSample.Value;

                                double Avgminmax = (maxQty + minQty) / 2;
                                ItemInventoryValue = objItem.ExtendedCost ?? 0;
                                OnHandQuantity = objItem.OnHandQuantity ?? 0;

                                double AverageExtendedCost = 0, AverageOnHandQuantity = 0;
                                ItemAvgCostQtyInfo objItemAvgCostQtyInfo = objDashboardDAL.GetItemOrRoomAvgCostQty(objItem.GUID, objItem.Room ?? 0, objItem.CompanyID ?? 0, (FromDate ?? DateTime.Now.AddDays(1)), Todate);
                                if (objItemAvgCostQtyInfo != null)
                                {
                                    AverageExtendedCost = objItemAvgCostQtyInfo.AverageExtendedCost;
                                    AverageOnHandQuantity = objItemAvgCostQtyInfo.AverageOnHandQuantity;
                                }

                                List<PullMaster> PullMasterList = null;
                                List<OrderDetailsDTO> lstOrderDetails = null;
                                PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate);//context.Database.SqlQuery<PullMaster>("exec GetPullByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramsInner).ToList();
                                var transferList = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate, true);
                                double TransferCost = 0;
                                double transferedQty = 0;

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                                    PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                                    PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                                }

                                if (transferList != null && transferList.Any() && transferList.Count() > 0)
                                {
                                    TransferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                                    TransferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                                    transferedQty = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                                    transferedQty -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                                }

                                var PullTransferCost = PullCost + TransferCost;
                                var PullTransferQty = PullQuantity + transferedQty;

                                lstOrderDetails = GetOrderByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate);//context.Database.SqlQuery<OrderDetailsDTO>("exec GetOrderByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramInnerCase).ToList();
                                //string qry = "SELECT Om.OrderType as ItemType, OD.* FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE OD.ItemGUID = '" + objItem.GUID + "' AND Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId + " AND OD.ReceivedOn >= '" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' AND OD.ReceivedOn <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";
                                //lstOrderDetails = (from pm in context.Database.SqlQuery<OrderDetailsDTO>(qry)
                                //                   select pm).ToList();

                                if (lstOrderDetails.Any())
                                {
                                    OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                                    ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                                }
                                FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                                objInventoryAnalysisMonthWise.MaximumQuantity = maxQty;
                                objInventoryAnalysisMonthWise.MinimumQuantity = minQty;
                                objInventoryAnalysisMonthWise.MonthizedInventoryValue = ItemInventoryValue;
                                objInventoryAnalysisMonthWise.MonthizedOnHandQuantity = OnHandQuantity;
                                objInventoryAnalysisMonthWise.MonthizedOrderQuantity = FinalOrderedQty;
                                objInventoryAnalysisMonthWise.MonthizedPullQuantity = PullTransferQty;
                                objInventoryAnalysisMonthWise.MonthizedPullCost = PullTransferCost;

                                if (AverageExtendedCost > 0)
                                {
                                    objInventoryAnalysisMonthWise.MonthizedPullValueTurns = (PullTransferCost / AverageExtendedCost) * 12;
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedPullValueTurns = 0;
                                }
                                if (AverageOnHandQuantity > 0)
                                {
                                    objInventoryAnalysisMonthWise.MonthizedPullTurns = (PullTransferQty / AverageOnHandQuantity) * 12;
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedPullTurns = 0;
                                }
                                if (Avgminmax > 0)
                                {
                                    objInventoryAnalysisMonthWise.MonthizedOrderTurns = (FinalOrderedQty / Avgminmax) * 12;
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedOrderTurns = 0;
                                }

                                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4)
                                {
                                    if (objDashboardParameterDTO.TurnsMeasureMethod == 1)
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullValueTurns;
                                    }
                                    else if (objDashboardParameterDTO.TurnsMeasureMethod == 2)
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullTurns;
                                    }
                                    else
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedOrderTurns;
                                    }
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullTurns;
                                }

                                objInventoryAnalysisMonthWise.MonthizedPullValueAverageUsage = PullTransferCost / daysinmonth;
                                objInventoryAnalysisMonthWise.MonthizedPullAverageUsage = PullTransferQty / daysinmonth;
                                objInventoryAnalysisMonthWise.MonthizedOrderAverageUsage = FinalOrderedQty / daysinmonth;

                                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4)
                                {
                                    if (objDashboardParameterDTO.AUMeasureMethod == 1)
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullValueAverageUsage;
                                    }
                                    else if (objDashboardParameterDTO.AUMeasureMethod == 2)
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullAverageUsage;
                                    }
                                    else
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedOrderAverageUsage;
                                    }
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullAverageUsage;
                                }

                                DashboardAnalysisInfo objDashboardAnalysisInfoTurn = GetTurnsByItemGUID(RoomId, CompanyId, objItem.GUID, 0, objDashboardParameterDTO, objRegionalSettings);
                                DashboardAnalysisInfo objDashboardAnalysisInfoAU = GetAvgUsageByItemGUID(RoomId, CompanyId, objItem.GUID, 0, objDashboardParameterDTO, objRegionalSettings);

                                objInventoryAnalysisMonthWise.AnnualizedOrderTurns = objDashboardAnalysisInfoTurn.MonthizedOrderTurns ?? 0;
                                objInventoryAnalysisMonthWise.AnnualizedPullTurns = objDashboardAnalysisInfoTurn.MonthizedPullTurns ?? 0;
                                objInventoryAnalysisMonthWise.AnnualizedPullValueTurns = objDashboardAnalysisInfoTurn.MonthizedPullValueTurns ?? 0;
                                objInventoryAnalysisMonthWise.AnnualizedPullAverageUsage = objDashboardAnalysisInfoAU.CalculatedPullAverageUsage;
                                objInventoryAnalysisMonthWise.AnnualizedOrderAverageUsage = objDashboardAnalysisInfoAU.CalculatedOrderAverageUsage;
                                objInventoryAnalysisMonthWise.AnnualizedPullValueAverageUsage = objDashboardAnalysisInfoAU.CalculatedPullValueAverageUsage;                              

                                //----------------------NEW COLUMNS ADDED FOR TRACKING----------------------
                                //
                                objInventoryAnalysisMonthWise.ManufacturerNumber = objItem.ManufacturerNumber;
                                objInventoryAnalysisMonthWise.SupplierPartNo = objItem.SupplierPartNo;
                                objInventoryAnalysisMonthWise.UPC = objItem.UPC;
                                objInventoryAnalysisMonthWise.DefaultReorderQuantity = objItem.DefaultReorderQuantity;
                                objInventoryAnalysisMonthWise.DefaultPullQuantity = objItem.DefaultPullQuantity;
                                objInventoryAnalysisMonthWise.Cost = objItem.Cost;
                                objInventoryAnalysisMonthWise.Markup = objItem.Markup;
                                objInventoryAnalysisMonthWise.SellPrice = objItem.SellPrice;
                                objInventoryAnalysisMonthWise.ExtendedCost = objItem.ExtendedCost;
                                objInventoryAnalysisMonthWise.Trend = objItem.Trend;
                                objInventoryAnalysisMonthWise.Taxable = objItem.Taxable;
                                objInventoryAnalysisMonthWise.Consignment = objItem.Consignment;
                                objInventoryAnalysisMonthWise.StagedQuantity = objItem.StagedQuantity;
                                objInventoryAnalysisMonthWise.InTransitquantity = objItem.InTransitquantity;
                                objInventoryAnalysisMonthWise.OnOrderQuantity = objItem.OnOrderQuantity;
                                objInventoryAnalysisMonthWise.OnTransferQuantity = objItem.OnTransferQuantity;
                                objInventoryAnalysisMonthWise.SuggestedOrderQuantity = objItem.SuggestedOrderQuantity;
                                objInventoryAnalysisMonthWise.RequisitionedQuantity = objItem.RequisitionedQuantity;
                                objInventoryAnalysisMonthWise.AverageUsage = objItem.AverageUsage;
                                objInventoryAnalysisMonthWise.Turns = objItem.Turns;
                                objInventoryAnalysisMonthWise.OnHandQuantity = objItem.OnHandQuantity;
                                objInventoryAnalysisMonthWise.CriticalQuantity = objItem.CriticalQuantity;
                                objInventoryAnalysisMonthWise.WeightPerPiece = objItem.WeightPerPiece;
                                objInventoryAnalysisMonthWise.ItemUniqueNumber = objItem.ItemUniqueNumber;
                                objInventoryAnalysisMonthWise.IsTransfer = objItem.IsTransfer;
                                objInventoryAnalysisMonthWise.IsPurchase = objItem.IsPurchase;
                                objInventoryAnalysisMonthWise.DefaultLocation = objItem.DefaultLocation;
                                objInventoryAnalysisMonthWise.InventoryClassification = objItem.InventoryClassification;
                                objInventoryAnalysisMonthWise.SerialNumberTracking = objItem.SerialNumberTracking;
                                objInventoryAnalysisMonthWise.LotNumberTracking = objItem.LotNumberTracking;
                                objInventoryAnalysisMonthWise.DateCodeTracking = objItem.DateCodeTracking;
                                objInventoryAnalysisMonthWise.ItemType = objItem.ItemType;
                                objInventoryAnalysisMonthWise.IsLotSerialExpiryCost = objItem.IsLotSerialExpiryCost;
                                objInventoryAnalysisMonthWise.IsItemLevelMinMaxQtyRequired = objItem.IsItemLevelMinMaxQtyRequired;
                                objInventoryAnalysisMonthWise.IsEnforceDefaultReorderQuantity = objItem.IsEnforceDefaultReorderQuantity;
                                objInventoryAnalysisMonthWise.AverageCost = objItem.AverageCost;
                                objInventoryAnalysisMonthWise.IsBuildBreak = objItem.IsBuildBreak;
                                objInventoryAnalysisMonthWise.BondedInventory = objItem.BondedInventory;
                                objInventoryAnalysisMonthWise.OnReturnQuantity = objItem.OnReturnQuantity;
                                objInventoryAnalysisMonthWise.TrendingSetting = objItem.TrendingSetting;
                                objInventoryAnalysisMonthWise.PullQtyScanOverride = objItem.PullQtyScanOverride;
                                objInventoryAnalysisMonthWise.IsAutoInventoryClassification = objItem.IsAutoInventoryClassification;
                                objInventoryAnalysisMonthWise.IsPackslipMandatoryAtReceive = objItem.IsPackslipMandatoryAtReceive;
                                objInventoryAnalysisMonthWise.SuggestedTransferQuantity = objItem.SuggestedTransferQuantity;
                                objInventoryAnalysisMonthWise.QtyToMeetDemand = objItem.QtyToMeetDemand;

                                if (TobeAdded == 1)
                                {
                                    context.InventoryAnalysisMonthWises.Add(objInventoryAnalysisMonthWise);
                                }
                            }
                            catch (Exception EX)
                            {
                                ExceptionString = ExceptionString + (ExceptionString == "" ? "" : Environment.NewLine) +
                                                  "--==========[" + base.DataBaseName + ">>" + CompanyId.ToString() + ">>" + RoomId.ToString() + ">>" + objItem.ItemNumber + "]==========--";
                                ExceptionString = ExceptionString + Environment.NewLine + "Exception Message:" + Environment.NewLine + EX.Message;
                                if (EX.InnerException != null && !String.IsNullOrEmpty(EX.InnerException.Message))
                                {
                                    ExceptionString = ExceptionString + Environment.NewLine + "Inner Exception Message:" + Environment.NewLine + EX.InnerException.Message;
                                }
                            }
                        }
                    }
                    context.SaveChanges();

                }

                return true;
            }
            catch (Exception EX)
            {
                ExceptionString = "--==========[" + base.DataBaseName + ">>" + CompanyId.ToString() + ">>" + RoomId.ToString() + "]==========--";
                ExceptionString = ExceptionString + Environment.NewLine + "Exception Message:" + Environment.NewLine + EX.Message;
                if (EX.InnerException != null && !String.IsNullOrEmpty(EX.InnerException.Message))
                {
                    ExceptionString = ExceptionString + Environment.NewLine + "Inner Exception Message:" + Environment.NewLine + EX.InnerException.Message;
                }
                return false;
            }
        }
        public bool CalculateMonthisedRoomAnalytics(long RoomId, long CompanyId, int ForMonth, int ForYear, out string ExceptionString, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null)
        {
            try
            {
                ExceptionString = "";
                DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                if (objRegionalSettings == null)
                {
                    objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
                }
                if (objDashboardParameterDTO == null)
                {
                    objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
                }
                string CurrentRoomTimeZone = "UTC";

                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName;
                    if (string.IsNullOrWhiteSpace(CurrentRoomTimeZone))
                    {
                        CurrentRoomTimeZone = "UTC";
                    }
                }

                DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    double PullCost = 0;
                    double ItemInventoryValue = 0;
                    double OnHandQuantity = 0;
                    double PullQuantity = 0;
                    double OrderedQty = 0;
                    double ReturnOrderedQty = 0;
                    double FinalOrderedQty = 0;
                    int daysinmonth = DateTime.DaysInMonth(ForYear, ForMonth);
                    DateTime? FromDate = new DateTime(ForYear, ForMonth, 1, 0, 0, 0);
                    DateTime Todate = new DateTime(ForYear, ForMonth, daysinmonth, 23, 59, 0);
                    FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                    Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                    double minQty = 0;
                    double maxQty = 0;
                    BinMasterDAL objBinMaster = new BinMasterDAL(base.DataBaseName);
                    InventoryAnalysisMonthWise objInventoryAnalysisMonthWise = context.InventoryAnalysisMonthWises.FirstOrDefault(t => t.RoomId == RoomId && t.CompanyId == CompanyId && t.ItemGUID == Guid.Empty && t.CalculationMonth == ForMonth && t.CalculationYear == ForYear);
                    int TobeAdded = 0;
                    if (objInventoryAnalysisMonthWise == null)
                    {
                        objInventoryAnalysisMonthWise = new InventoryAnalysisMonthWise();
                        TobeAdded = 1;
                    }
                    objInventoryAnalysisMonthWise.ItemGUID = Guid.Empty;
                    objInventoryAnalysisMonthWise.CalculationDate = DateTime.UtcNow;
                    objInventoryAnalysisMonthWise.CalculationFor = 2;
                    objInventoryAnalysisMonthWise.CalculationMonth = ForMonth;
                    objInventoryAnalysisMonthWise.CalculationYear = ForYear;
                    objInventoryAnalysisMonthWise.CompanyId = CompanyId;
                    objInventoryAnalysisMonthWise.RoomId = RoomId;

                    double Avgminmax = 0;
                    var qryitems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false);
                    if (qryitems.Any())
                    {
                        OnHandQuantity = qryitems.Sum(t => (t.OnHandQuantity ?? 0));
                        ItemInventoryValue = qryitems.Sum(t => (t.ExtendedCost ?? 0));
                        //Avgminmax = qryitems.Sum(t => ((t.MinimumQuantity) + (t.MaximumQuantity) / 2));
                    }

                    Avgminmax = GetMinMaxAvgByItemAndLocation(RoomId, CompanyId);

                    List<PullMaster> PullMasterList = null;
                    List<OrderDetailsDTO> lstOrderDetails = null;
                    List<TransferDetailDTO> transferList = null;
                    double transferCost = 0;
                    double transferQuantity = 0;
                    

                    PullMasterList = GetPullByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);//context.Database.SqlQuery<PullMaster>("exec GetPullByRoomAndDateRange @RoomID,@CompanyID,@FromDate,@ToDate ", paramsInner).ToList();
                    transferList = GetTransferByRoomAndDateRange(RoomId,CompanyId,FromDate,Todate);

                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                    {
                        PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                        PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                    }

                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                    {
                        PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                        PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                    }

                    if (transferList != null && transferList.Any() && transferList.Count() > 0)
                    {
                        transferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                        transferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                        transferQuantity = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                        transferQuantity -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                    }

                    double pullTransferQty = PullQuantity + transferQuantity;
                    double pullTransferCost = PullCost + transferCost;

                    lstOrderDetails = GetOrderByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);//context.Database.SqlQuery<OrderDetailsDTO>("exec GetOrderByRoomAndDateRange @RoomID,@CompanyID,@FromDate,@ToDate ", paramInnerCase).ToList();

                    if (lstOrderDetails.Any())
                    {
                        OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                        ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                    }
                    FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                    objInventoryAnalysisMonthWise.MaximumQuantity = maxQty;
                    objInventoryAnalysisMonthWise.MinimumQuantity = minQty;
                    objInventoryAnalysisMonthWise.MonthizedInventoryValue = ItemInventoryValue;
                    objInventoryAnalysisMonthWise.MonthizedOnHandQuantity = OnHandQuantity;
                    objInventoryAnalysisMonthWise.MonthizedOrderQuantity = FinalOrderedQty;
                    objInventoryAnalysisMonthWise.MonthizedPullQuantity = pullTransferQty;
                    objInventoryAnalysisMonthWise.MonthizedPullCost = pullTransferCost;
                    double AverageExtendedCost = 0, AverageOnHandQuantity = 0;
                    ItemAvgCostQtyInfo objItemAvgCostQtyInfo = objDashboardDAL.GetItemOrRoomAvgCostQty(Guid.Empty, RoomId, CompanyId, (FromDate ?? DateTime.Now.AddDays(1)), Todate);
                    if (objItemAvgCostQtyInfo != null)
                    {
                        AverageExtendedCost = objItemAvgCostQtyInfo.AverageExtendedCost;
                        AverageOnHandQuantity = objItemAvgCostQtyInfo.AverageOnHandQuantity;
                    }

                    if (AverageExtendedCost > 0)
                    {
                        objInventoryAnalysisMonthWise.MonthizedPullValueTurns = (pullTransferCost / AverageExtendedCost) * 12;
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedPullValueTurns = 0;
                    }
                    if (AverageOnHandQuantity > 0)
                    {
                        objInventoryAnalysisMonthWise.MonthizedPullTurns = (pullTransferQty / AverageOnHandQuantity) * 12;
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedPullTurns = 0;
                    }
                    if (Avgminmax > 0)
                    {
                        objInventoryAnalysisMonthWise.MonthizedOrderTurns = (FinalOrderedQty / Avgminmax);
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedOrderTurns = 0;
                    }

                    if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4)
                    {
                        if (objDashboardParameterDTO.TurnsMeasureMethod == 1)
                        {
                            objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullValueTurns;
                        }
                        else if (objDashboardParameterDTO.TurnsMeasureMethod == 2)
                        {
                            objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullTurns;
                        }
                        else
                        {
                            objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedOrderTurns;
                        }
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullTurns;
                    }

                    objInventoryAnalysisMonthWise.MonthizedPullValueAverageUsage = pullTransferCost / daysinmonth;
                    objInventoryAnalysisMonthWise.MonthizedPullAverageUsage = pullTransferQty / daysinmonth;
                    objInventoryAnalysisMonthWise.MonthizedOrderAverageUsage = FinalOrderedQty / daysinmonth;

                    if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4)
                    {
                        if (objDashboardParameterDTO.AUMeasureMethod == 1)
                        {
                            objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullValueAverageUsage;
                        }
                        else if (objDashboardParameterDTO.AUMeasureMethod == 2)
                        {
                            objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullAverageUsage;
                        }
                        else
                        {
                            objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedOrderAverageUsage;
                        }
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullAverageUsage;
                    }



                    DashboardAnalysisInfo objDashboardAnalysisInfoTurn = GetTurnsByRoom(RoomId, CompanyId, 0, objDashboardParameterDTO, objRegionalSettings);
                    DashboardAnalysisInfo objDashboardAnalysisInfoAU = GetAvgUsageByRoom(RoomId, CompanyId, 0, objDashboardParameterDTO, objRegionalSettings);

                    objInventoryAnalysisMonthWise.AnnualizedOrderTurns = objDashboardAnalysisInfoTurn.MonthizedOrderTurns ?? 0;
                    objInventoryAnalysisMonthWise.AnnualizedPullTurns = objDashboardAnalysisInfoTurn.MonthizedPullTurns ?? 0;
                    objInventoryAnalysisMonthWise.AnnualizedPullValueTurns = objDashboardAnalysisInfoTurn.MonthizedPullValueTurns ?? 0;

                    objInventoryAnalysisMonthWise.AnnualizedPullAverageUsage = objDashboardAnalysisInfoAU.CalculatedPullAverageUsage;
                    objInventoryAnalysisMonthWise.AnnualizedOrderAverageUsage = objDashboardAnalysisInfoAU.CalculatedOrderAverageUsage;
                    objInventoryAnalysisMonthWise.AnnualizedPullValueAverageUsage = objDashboardAnalysisInfoAU.CalculatedPullValueAverageUsage;


                    //if (objDashboardParameterDTO != null)
                    //{

                    //}
                    //if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.TurnsMonthsOfUsageToSample ?? 0) > 0)
                    //{

                    //}
                    //else
                    //{

                    //}
                    //if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.AUDayOfUsageToSample ?? 0) > 0)
                    //{

                    //}
                    //else
                    //{

                    //}




                    //MonthizedTurns
                    //MonthizedAverageUsage

                    //MonthizedPullValueTurns
                    //MonthizedPullTurns
                    //MonthizedOrderTurns                        
                    //MonthizedPullValueAverageUsage
                    //MonthizedPullAverageUsage
                    //MonthizedOrderAverageUsage



                    //----------------------NEW COLUMNS ADDED FOR TRACKING----------------------
                    //
                    //objInventoryAnalysisMonthWise.ManufacturerNumber = objItem.ManufacturerNumber;
                    //objInventoryAnalysisMonthWise.SupplierPartNo = objItem.SupplierPartNo;
                    //objInventoryAnalysisMonthWise.UPC = objItem.UPC;
                    //objInventoryAnalysisMonthWise.DefaultReorderQuantity = objItem.DefaultReorderQuantity;
                    //objInventoryAnalysisMonthWise.DefaultPullQuantity = objItem.DefaultPullQuantity;
                    //objInventoryAnalysisMonthWise.Cost = objItem.Cost;
                    //objInventoryAnalysisMonthWise.Markup = objItem.Markup;
                    //objInventoryAnalysisMonthWise.SellPrice = objItem.SellPrice;
                    //objInventoryAnalysisMonthWise.ExtendedCost = objItem.ExtendedCost;
                    //objInventoryAnalysisMonthWise.Trend = objItem.Trend;
                    //objInventoryAnalysisMonthWise.Taxable = objItem.Taxable;
                    //objInventoryAnalysisMonthWise.Consignment = objItem.Consignment;
                    //objInventoryAnalysisMonthWise.StagedQuantity = objItem.StagedQuantity;
                    //objInventoryAnalysisMonthWise.InTransitquantity = objItem.InTransitquantity;
                    //objInventoryAnalysisMonthWise.OnOrderQuantity = objItem.OnOrderQuantity;
                    //objInventoryAnalysisMonthWise.OnTransferQuantity = objItem.OnTransferQuantity;
                    //objInventoryAnalysisMonthWise.SuggestedOrderQuantity = objItem.SuggestedOrderQuantity;
                    //objInventoryAnalysisMonthWise.RequisitionedQuantity = objItem.RequisitionedQuantity;
                    //objInventoryAnalysisMonthWise.AverageUsage = objItem.AverageUsage;
                    //objInventoryAnalysisMonthWise.Turns = objItem.Turns;
                    //objInventoryAnalysisMonthWise.OnHandQuantity = objItem.OnHandQuantity;
                    //objInventoryAnalysisMonthWise.CriticalQuantity = objItem.CriticalQuantity;
                    //objInventoryAnalysisMonthWise.WeightPerPiece = objItem.WeightPerPiece;
                    //objInventoryAnalysisMonthWise.ItemUniqueNumber = objItem.ItemUniqueNumber;
                    //objInventoryAnalysisMonthWise.IsTransfer = objItem.IsTransfer;
                    //objInventoryAnalysisMonthWise.IsPurchase = objItem.IsPurchase;
                    //objInventoryAnalysisMonthWise.DefaultLocation = objItem.DefaultLocation;
                    //objInventoryAnalysisMonthWise.InventoryClassification = objItem.InventoryClassification;
                    //objInventoryAnalysisMonthWise.SerialNumberTracking = objItem.SerialNumberTracking;
                    //objInventoryAnalysisMonthWise.LotNumberTracking = objItem.LotNumberTracking;
                    //objInventoryAnalysisMonthWise.DateCodeTracking = objItem.DateCodeTracking;
                    //objInventoryAnalysisMonthWise.ItemType = objItem.ItemType;
                    //objInventoryAnalysisMonthWise.IsLotSerialExpiryCost = objItem.IsLotSerialExpiryCost;
                    //objInventoryAnalysisMonthWise.IsItemLevelMinMaxQtyRequired = objItem.IsItemLevelMinMaxQtyRequired;
                    //objInventoryAnalysisMonthWise.IsEnforceDefaultReorderQuantity = objItem.IsEnforceDefaultReorderQuantity;
                    //objInventoryAnalysisMonthWise.AverageCost = objItem.AverageCost;
                    //objInventoryAnalysisMonthWise.IsBuildBreak = objItem.IsBuildBreak;
                    //objInventoryAnalysisMonthWise.BondedInventory = objItem.BondedInventory;
                    //objInventoryAnalysisMonthWise.OnReturnQuantity = objItem.OnReturnQuantity;
                    //objInventoryAnalysisMonthWise.TrendingSetting = objItem.TrendingSetting;
                    //objInventoryAnalysisMonthWise.PullQtyScanOverride = objItem.PullQtyScanOverride;
                    //objInventoryAnalysisMonthWise.IsAutoInventoryClassification = objItem.IsAutoInventoryClassification;
                    //objInventoryAnalysisMonthWise.IsPackslipMandatoryAtReceive = objItem.IsPackslipMandatoryAtReceive;
                    //objInventoryAnalysisMonthWise.SuggestedTransferQuantity = objItem.SuggestedTransferQuantity;
                    //objInventoryAnalysisMonthWise.QtyToMeetDemand = objItem.QtyToMeetDemand;

                    if (TobeAdded == 1)
                    {
                        context.InventoryAnalysisMonthWises.Add(objInventoryAnalysisMonthWise);
                    }

                    context.SaveChanges();

                }

                return true;
            }
            catch (Exception EX)
            {
                ExceptionString = "--==========[" + base.DataBaseName + ">>" + CompanyId.ToString() + ">>" + RoomId.ToString() + "]==========--";
                ExceptionString = ExceptionString + Environment.NewLine + "Exception Message:" + Environment.NewLine + EX.Message;
                if (EX.InnerException != null && !String.IsNullOrEmpty(EX.InnerException.Message))
                {
                    ExceptionString = ExceptionString + Environment.NewLine + "Inner Exception Message:" + Environment.NewLine + EX.InnerException.Message;
                }
                return false;
            }
        }

        public List<ItemMaster> GetItemMasterHistory_InventoryMonthlyAnalysis_All(long CompanyId, long RoomId, DateTime? LastDate)
        {
            var paramInnerCase = new SqlParameter[] {
                                                new SqlParameter("@CompanyId", CompanyId),
                                                new SqlParameter("@RoomId", RoomId),
                                                new SqlParameter("@LastDate", LastDate),
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMaster>("exec GetItemMasterHistory_InventoryMonthlyAnalysis_All @CompanyId,@RoomId,@LastDate", paramInnerCase).ToList();
            }
        }

        public ItemMaster GetItemMasterHistory_InventoryMonthlyAnalysis(long ItemId, string ItemNumber, DateTime? LastDate)
        {
            var paramInnerCase = new SqlParameter[] {
                                                new SqlParameter("@ItemId", ItemId),
                                                new SqlParameter("@ItemNumber", ItemNumber),
                                                new SqlParameter("@LastDate", LastDate),
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMaster>("exec GetItemMasterHistory_InventoryMonthlyAnalysis @ItemId,@ItemNumber,@LastDate", paramInnerCase).FirstOrDefault();
            }
        }
        public DashboardAnalysisInfo GetTurnsByItemGUID_History(long RoomId, long CompanyId, Guid ItemGUID, string ItemNumber, long UserId, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null, DateTime? CurrentUTCDateTime = null) //,byte? TurnsMeasureMethod,int? TurnsMonthsOfUsageToSample
        {
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            string CurrentRoomTimeZone = "UTC";
            if (objDashboardParameterDTO == null)
            {
                //if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["DashboardParams"] != null)
                //{
                //    objDashboardParameterDTO = (DashboardParameterDTO)HttpContext.Current.Session["DashboardParams"];
                //}
                //else
                {
                    objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
                }
            }
            if (objRegionalSettings == null)
            {
                //if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["eTurnsRegionInfoProp"] != null)
                //{
                //    objRegionalSettings = (eTurnsRegionInfo)HttpContext.Current.Session["eTurnsRegionInfoProp"];
                //}
                //else
                {
                    objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
                }
            }
            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }
            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, CurrentUTCDateTime ?? DateTime.UtcNow);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.TurnsDaysOfUsageToSample ?? 0) > 0)
                {
                    ItemMaster objItem = objDashboardDAL.GetItemMasterHistory_InventoryMonthlyAnalysis(0, ItemNumber, CurrentUTCDateTime);

                    if (objItem != null)
                    {
                        double PullCost = 0;
                        double ItemInventoryValue = 0;
                        double PullQuantity = 0;
                        double OrderedQty = 0;
                        double ReturnOrderedQty = 0;
                        double FinalOrderedQty = 0;
                        //double onHandQty = 0;
                        DateTime? FromDate = CurrentTimeofTimeZone.AddDays(objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value * (-1)).Date;
                        if (FromDate.HasValue)
                        {
                            FromDate = new DateTime(FromDate.Value.Year, FromDate.Value.Month, 1, 0, 0, 0);
                        }

                        DateTime Todate = new DateTime(CurrentTimeofTimeZone.AddMonths(-1).Date.Year, CurrentTimeofTimeZone.AddMonths(-1).Date.Month, DateTime.DaysInMonth(CurrentTimeofTimeZone.AddMonths(-1).Date.Year, CurrentTimeofTimeZone.AddMonths(-1).Date.Month), 23, 59, 59);
                        FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                        Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                        double minQty = objItem.MaximumQuantity;
                        double maxQty = objItem.MinimumQuantity;
                        BinMasterDAL objBinMaster = new BinMasterDAL(base.DataBaseName);
                        if (objItem.IsItemLevelMinMaxQtyRequired == false)
                        {
                            //List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID)).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber).ToList();
                            List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID), false, string.Empty, null, null).OrderBy(x => x.BinNumber).ToList();
                            //List<BinMasterDTO> Itemlocs = objBinMaster.GetItemLocation(RoomId, CompanyId, false, false,(objItem.GUID),0,null,false).OrderBy(x => x.BinNumber).ToList();//.Where(x => !x.IsStagingLocation)
                            if (Itemlocs.Count > 0)
                            {
                                minQty = Itemlocs.First().MinimumQuantity ?? 0;
                                maxQty = Itemlocs.First().MaximumQuantity ?? 0;
                            }
                        }

                        double DurationDiviser = 12 / (objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value / 30); //double DurationDiviser = 12 / objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value;
                        double Avgminmax = (maxQty + minQty) / 2;
                        List<PullMaster> PullMasterList = null;
                        List<OrderDetailsDTO> lstOrderDetails = null;

                        PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate);//context.Database.SqlQuery<PullMaster>("exec GetPullByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramsInner).ToList();
                        var transferList = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate, true);
                        double TransferCost = 0;
                        double transferedQty = 0;

                        if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                        {
                            PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                            PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                        }
                        if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                        {
                            PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                            PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                        }

                        if (transferList != null && transferList.Any() && transferList.Count() > 0)
                        {
                            TransferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                            TransferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                            transferedQty = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                            transferedQty -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                        }

                        var PullTransferCost = PullCost + TransferCost;
                        var PullTransferQty = PullQuantity + transferedQty;

                        lstOrderDetails = GetOrderByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate);//context.Database.SqlQuery<OrderDetailsDTO>("exec GetOrderByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramInnerCase).ToList();

                        if (lstOrderDetails.Any())
                        {
                            OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                            ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                        }
                        FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                        if (PullTransferCost > 0)
                        {
                            ItemInventoryValue = objItem.ExtendedCost ?? 0;
                            if (ItemInventoryValue > 0)
                            {
                                objDashboardAnalysisInfo.MonthizedPullValueTurns = ((PullTransferCost) / (ItemInventoryValue)) * (DurationDiviser);
                            }
                            else
                            {
                                objDashboardAnalysisInfo.MonthizedPullValueTurns = 0;
                            }
                        }
                        else
                        {
                            objDashboardAnalysisInfo.MonthizedPullValueTurns = 0;
                        }

                        if (PullTransferQty > 0 && (objItem.OnHandQuantity ?? 0) > 0)
                        {
                            objDashboardAnalysisInfo.MonthizedPullTurns = (PullTransferQty / objItem.OnHandQuantity ?? 1) * DurationDiviser;
                        }
                        else
                        {
                            objDashboardAnalysisInfo.MonthizedPullTurns = 0;
                        }
                        if (Avgminmax > 0 && FinalOrderedQty > 0)
                        {
                            objDashboardAnalysisInfo.MonthizedOrderTurns = ((FinalOrderedQty) / Avgminmax) * DurationDiviser;
                        }
                        else
                        {
                            objDashboardAnalysisInfo.MonthizedOrderTurns = 0;
                        }
                    }

                }
            }
            return objDashboardAnalysisInfo;
        }
        public DashboardAnalysisInfo GetAvgUsageByItemGUID_History(long RoomId, long CompanyId, Guid ItemGUID, string ItemNumber, long UserId, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null, DateTime? CurrentUTCDateTime = null)
        {
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            string CurrentRoomTimeZone = "UTC";

            if (objDashboardParameterDTO == null)
            {
                objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
            }

            if (objRegionalSettings == null)
            {
                objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
            }

            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }
            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, CurrentUTCDateTime ?? DateTime.UtcNow);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.AUDayOfUsageToSample ?? 0) > 0)
                {
                    //ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                    ItemMaster objItem = objDashboardDAL.GetItemMasterHistory_InventoryMonthlyAnalysis(0, ItemNumber, CurrentUTCDateTime);
                    if (objItem != null)
                    {
                        DateTime? FromDate = CurrentTimeofTimeZone.AddDays(objDashboardParameterDTO.AUDayOfUsageToSample.Value * (-1)).Date;
                        DateTime Todate = new DateTime(CurrentTimeofTimeZone.Year, CurrentTimeofTimeZone.Month, CurrentTimeofTimeZone.Day, 23, 59, 0);
                        FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                        Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                        double PullCost = 0;
                        double PullQuantity = 0;
                        double OrderedQty = 0;
                        double ReturnOrderedQty = 0;
                        double FinalOrderedQty = 0;

                        List<PullMaster> PullMasterList = null;
                        List<OrderDetailsDTO> lstOrderDetails = null;

                        PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate);
                        var transferList = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate, true);
                        double TransferCost = 0;
                        double transferedQty = 0;

                        if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                        {
                            PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                            PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                        }
                        if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                        {
                            PullCost = PullCost - PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum();
                            PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                        }

                        if (transferList != null && transferList.Any() && transferList.Count() > 0)
                        {
                            TransferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                            TransferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                            transferedQty = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                            transferedQty -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                        }

                        var PullTransferCost = PullCost + TransferCost;
                        var PullTransferQty = PullQuantity + transferedQty;

                        lstOrderDetails = GetOrderByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate);//context.Database.SqlQuery<OrderDetailsDTO>("exec GetOrderByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramInnerCase).ToList();

                        if (lstOrderDetails.Any())
                        {
                            OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                            ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                        }

                        FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                        if (PullTransferCost > 0)
                        {
                            objDashboardAnalysisInfo.CalculatedPullValueAverageUsage = PullTransferCost / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                        }
                        else
                        {
                            objDashboardAnalysisInfo.CalculatedPullValueAverageUsage = 0;
                        }

                        if (PullTransferQty > 0)
                        {
                            objDashboardAnalysisInfo.CalculatedPullAverageUsage = PullTransferQty / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                        }
                        else
                        {
                            objDashboardAnalysisInfo.CalculatedPullAverageUsage = 0;
                        }

                        if (FinalOrderedQty > 0)
                        {
                            objDashboardAnalysisInfo.CalculatedOrderAverageUsage = FinalOrderedQty / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                        }
                        else
                        {
                            objDashboardAnalysisInfo.CalculatedOrderAverageUsage = 0;
                        }
                    }
                }


            }
            return objDashboardAnalysisInfo;
        }
        public bool CalculateMonthisedItemAnalytics_History(long RoomId, long CompanyId, int ForMonth, int ForYear, out string ExceptionString, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null, DateTime? CurrentUTCDateTime = null)
        {
            try
            {
                ExceptionString = "";
                DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                if (objRegionalSettings == null)
                {
                    objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
                }

                if (objDashboardParameterDTO == null)
                {
                    objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
                }
                string CurrentRoomTimeZone = "UTC";
                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName;
                    if (string.IsNullOrWhiteSpace(CurrentRoomTimeZone))
                    {
                        CurrentRoomTimeZone = "UTC";
                    }
                }

                //DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    List<ItemMaster> lstItems = objDashboardDAL.GetItemMasterHistory_InventoryMonthlyAnalysis_All(CompanyId, RoomId, CurrentUTCDateTime).ToList();
                    if (lstItems != null && lstItems.Any())
                    {
                        foreach (var objItem in lstItems)
                        {
                            try
                            {
                                //ItemMaster itemHistoryObj = objDashboardDAL.GetItemMasterHistory_InventoryMonthlyAnalysis(objItem1.ID, objItem1.ItemNumber, CurrentUTCDateTime);
                                //ItemMaster objItem = new ItemMaster();
                                //if (itemHistoryObj != null)
                                //{
                                //    objItem = itemHistoryObj;
                                //}
                                //else
                                //{
                                //    objItem = objItem1;
                                //}
                                double PullCost = 0;
                                double ItemInventoryValue = 0;
                                double OnHandQuantity = 0;
                                double PullQuantity = 0;
                                double OrderedQty = 0;
                                double ReturnOrderedQty = 0;
                                double FinalOrderedQty = 0;
                                int daysinmonth = DateTime.DaysInMonth(ForYear, ForMonth);
                                DateTime? FromDate = new DateTime(ForYear, ForMonth, 1, 0, 0, 0);
                                DateTime Todate = new DateTime(ForYear, ForMonth, daysinmonth, 23, 59, 0);
                                FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                                Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                                double minQty = objItem.MinimumQuantity;
                                double maxQty = objItem.MaximumQuantity;
                                BinMasterDAL objBinMaster = new BinMasterDAL(base.DataBaseName);
                                InventoryAnalysisMonthWise objInventoryAnalysisMonthWise = context.InventoryAnalysisMonthWises.FirstOrDefault(t => t.RoomId == RoomId && t.CompanyId == CompanyId && t.ItemGUID == objItem.GUID && t.CalculationMonth == ForMonth && t.CalculationYear == ForYear);
                                int TobeAdded = 0;
                                if (objInventoryAnalysisMonthWise == null)
                                {
                                    objInventoryAnalysisMonthWise = new InventoryAnalysisMonthWise();
                                    TobeAdded = 1;
                                }
                                objInventoryAnalysisMonthWise.ItemGUID = objItem.GUID;
                                objInventoryAnalysisMonthWise.CalculationDate = DateTime.UtcNow;
                                objInventoryAnalysisMonthWise.CalculationFor = 1;
                                objInventoryAnalysisMonthWise.CalculationMonth = ForMonth;
                                objInventoryAnalysisMonthWise.CalculationYear = ForYear;
                                objInventoryAnalysisMonthWise.CompanyId = objItem.CompanyID ?? 0;
                                objInventoryAnalysisMonthWise.RoomId = objItem.Room ?? 0;

                                if (objItem.IsItemLevelMinMaxQtyRequired == false)
                                {
                                    //List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID)).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber).ToList();
                                    List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID), false, string.Empty, null, null).OrderBy(x => x.BinNumber).ToList();
                                    //List<BinMasterDTO> Itemlocs = objBinMaster.GetItemLocation(RoomId, CompanyId, false, false, (objItem.GUID),0,null,false).OrderBy(x => x.BinNumber).ToList();//.Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber).ToList();
                                    if (Itemlocs.Count > 0)
                                    {
                                        minQty = Itemlocs.First().MinimumQuantity ?? 0;
                                        maxQty = Itemlocs.First().MaximumQuantity ?? 0;
                                    }
                                }


                                //double DurationDiviser = 12 / objDashboardParameterDTO.TurnsMonthsOfUsageToSample.Value;

                                double Avgminmax = (maxQty + minQty) / 2;
                                ItemInventoryValue = objItem.ExtendedCost ?? 0;
                                OnHandQuantity = objItem.OnHandQuantity ?? 0;

                                double AverageExtendedCost = 0, AverageOnHandQuantity = 0;
                                ItemAvgCostQtyInfo objItemAvgCostQtyInfo = objDashboardDAL.GetItemOrRoomAvgCostQty(objItem.GUID, objItem.Room ?? 0, objItem.CompanyID ?? 0, (FromDate ?? DateTime.Now.AddDays(1)), Todate);
                                if (objItemAvgCostQtyInfo != null)
                                {
                                    AverageExtendedCost = objItemAvgCostQtyInfo.AverageExtendedCost;
                                    AverageOnHandQuantity = objItemAvgCostQtyInfo.AverageOnHandQuantity;
                                }

                                List<PullMaster> PullMasterList = null;
                                List<OrderDetailsDTO> lstOrderDetails = null;
                                PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate);//context.Database.SqlQuery<PullMaster>("exec GetPullByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramsInner).ToList();
                                var transferList = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate, true);
                                double TransferCost = 0;
                                double transferedQty = 0;

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                                    PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                                    PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                                }

                                if (transferList != null && transferList.Any() && transferList.Count() > 0)
                                {
                                    TransferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                                    TransferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                                    transferedQty = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                                    transferedQty -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                                }

                                var PullTransferCost = PullCost + TransferCost;
                                var PullTransferQty = PullQuantity + transferedQty;

                                lstOrderDetails = GetOrderByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate);//context.Database.SqlQuery<OrderDetailsDTO>("exec GetOrderByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramInnerCase).ToList();
                                //string qry = "SELECT Om.OrderType as ItemType, OD.* FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE OD.ItemGUID = '" + objItem.GUID + "' AND Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId + " AND OD.ReceivedOn >= '" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' AND OD.ReceivedOn <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";
                                //lstOrderDetails = (from pm in context.Database.SqlQuery<OrderDetailsDTO>(qry)
                                //                   select pm).ToList();

                                if (lstOrderDetails.Any())
                                {
                                    OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                                    ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                                }
                                FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                                objInventoryAnalysisMonthWise.MaximumQuantity = maxQty;
                                objInventoryAnalysisMonthWise.MinimumQuantity = minQty;
                                objInventoryAnalysisMonthWise.MonthizedInventoryValue = ItemInventoryValue;
                                objInventoryAnalysisMonthWise.MonthizedOnHandQuantity = OnHandQuantity;
                                objInventoryAnalysisMonthWise.MonthizedOrderQuantity = FinalOrderedQty;
                                objInventoryAnalysisMonthWise.MonthizedPullQuantity = PullTransferQty;
                                objInventoryAnalysisMonthWise.MonthizedPullCost = PullTransferCost;

                                if (AverageExtendedCost > 0)
                                {
                                    objInventoryAnalysisMonthWise.MonthizedPullValueTurns = (PullTransferCost / AverageExtendedCost) * 12;
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedPullValueTurns = 0;
                                }
                                if (AverageOnHandQuantity > 0)
                                {
                                    objInventoryAnalysisMonthWise.MonthizedPullTurns = (PullTransferQty / AverageOnHandQuantity) * 12;
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedPullTurns = 0;
                                }
                                if (Avgminmax > 0)
                                {
                                    objInventoryAnalysisMonthWise.MonthizedOrderTurns = (FinalOrderedQty / Avgminmax) * 12;
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedOrderTurns = 0;
                                }

                                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4)
                                {
                                    if (objDashboardParameterDTO.TurnsMeasureMethod == 1)
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullValueTurns;
                                    }
                                    else if (objDashboardParameterDTO.TurnsMeasureMethod == 2)
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullTurns;
                                    }
                                    else
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedOrderTurns;
                                    }
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullTurns;
                                }

                                objInventoryAnalysisMonthWise.MonthizedPullValueAverageUsage = PullTransferCost / daysinmonth;
                                objInventoryAnalysisMonthWise.MonthizedPullAverageUsage = PullTransferQty / daysinmonth;
                                objInventoryAnalysisMonthWise.MonthizedOrderAverageUsage = FinalOrderedQty / daysinmonth;

                                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4)
                                {
                                    if (objDashboardParameterDTO.AUMeasureMethod == 1)
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullValueAverageUsage;
                                    }
                                    else if (objDashboardParameterDTO.AUMeasureMethod == 2)
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullAverageUsage;
                                    }
                                    else
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedOrderAverageUsage;
                                    }
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullAverageUsage;
                                }

                                DashboardAnalysisInfo objDashboardAnalysisInfoTurn = GetTurnsByItemGUID_History(RoomId, CompanyId, objItem.GUID, objItem.ItemNumber, 0, objDashboardParameterDTO, objRegionalSettings, CurrentUTCDateTime);
                                DashboardAnalysisInfo objDashboardAnalysisInfoAU = GetAvgUsageByItemGUID_History(RoomId, CompanyId, objItem.GUID, objItem.ItemNumber, 0, objDashboardParameterDTO, objRegionalSettings, CurrentUTCDateTime);

                                objInventoryAnalysisMonthWise.AnnualizedOrderTurns = objDashboardAnalysisInfoTurn.MonthizedOrderTurns ?? 0;
                                objInventoryAnalysisMonthWise.AnnualizedPullTurns = objDashboardAnalysisInfoTurn.MonthizedPullTurns ?? 0;
                                objInventoryAnalysisMonthWise.AnnualizedPullValueTurns = objDashboardAnalysisInfoTurn.MonthizedPullValueTurns ?? 0;
                                objInventoryAnalysisMonthWise.AnnualizedPullAverageUsage = objDashboardAnalysisInfoAU.CalculatedPullAverageUsage;
                                objInventoryAnalysisMonthWise.AnnualizedOrderAverageUsage = objDashboardAnalysisInfoAU.CalculatedOrderAverageUsage;
                                objInventoryAnalysisMonthWise.AnnualizedPullValueAverageUsage = objDashboardAnalysisInfoAU.CalculatedPullValueAverageUsage;

                                //----------------------NEW COLUMNS ADDED FOR TRACKING----------------------
                                //
                                objInventoryAnalysisMonthWise.ManufacturerNumber = objItem.ManufacturerNumber;
                                objInventoryAnalysisMonthWise.SupplierPartNo = objItem.SupplierPartNo;
                                objInventoryAnalysisMonthWise.UPC = objItem.UPC;
                                objInventoryAnalysisMonthWise.DefaultReorderQuantity = objItem.DefaultReorderQuantity;
                                objInventoryAnalysisMonthWise.DefaultPullQuantity = objItem.DefaultPullQuantity;
                                objInventoryAnalysisMonthWise.Cost = objItem.Cost;
                                objInventoryAnalysisMonthWise.Markup = objItem.Markup;
                                objInventoryAnalysisMonthWise.SellPrice = objItem.SellPrice;
                                objInventoryAnalysisMonthWise.ExtendedCost = objItem.ExtendedCost;
                                objInventoryAnalysisMonthWise.Trend = objItem.Trend;
                                objInventoryAnalysisMonthWise.Taxable = objItem.Taxable;
                                objInventoryAnalysisMonthWise.Consignment = objItem.Consignment;
                                objInventoryAnalysisMonthWise.StagedQuantity = objItem.StagedQuantity;
                                objInventoryAnalysisMonthWise.InTransitquantity = objItem.InTransitquantity;
                                objInventoryAnalysisMonthWise.OnOrderQuantity = objItem.OnOrderQuantity;
                                objInventoryAnalysisMonthWise.OnTransferQuantity = objItem.OnTransferQuantity;
                                objInventoryAnalysisMonthWise.SuggestedOrderQuantity = objItem.SuggestedOrderQuantity;
                                objInventoryAnalysisMonthWise.RequisitionedQuantity = objItem.RequisitionedQuantity;
                                objInventoryAnalysisMonthWise.AverageUsage = objItem.AverageUsage;
                                objInventoryAnalysisMonthWise.Turns = objItem.Turns;
                                objInventoryAnalysisMonthWise.OnHandQuantity = objItem.OnHandQuantity;
                                objInventoryAnalysisMonthWise.CriticalQuantity = objItem.CriticalQuantity;
                                objInventoryAnalysisMonthWise.WeightPerPiece = objItem.WeightPerPiece;
                                objInventoryAnalysisMonthWise.ItemUniqueNumber = objItem.ItemUniqueNumber;
                                objInventoryAnalysisMonthWise.IsTransfer = objItem.IsTransfer;
                                objInventoryAnalysisMonthWise.IsPurchase = objItem.IsPurchase;
                                objInventoryAnalysisMonthWise.DefaultLocation = objItem.DefaultLocation;
                                objInventoryAnalysisMonthWise.InventoryClassification = objItem.InventoryClassification;
                                objInventoryAnalysisMonthWise.SerialNumberTracking = objItem.SerialNumberTracking;
                                objInventoryAnalysisMonthWise.LotNumberTracking = objItem.LotNumberTracking;
                                objInventoryAnalysisMonthWise.DateCodeTracking = objItem.DateCodeTracking;
                                objInventoryAnalysisMonthWise.ItemType = objItem.ItemType;
                                objInventoryAnalysisMonthWise.IsLotSerialExpiryCost = objItem.IsLotSerialExpiryCost;
                                objInventoryAnalysisMonthWise.IsItemLevelMinMaxQtyRequired = objItem.IsItemLevelMinMaxQtyRequired;
                                objInventoryAnalysisMonthWise.IsEnforceDefaultReorderQuantity = objItem.IsEnforceDefaultReorderQuantity;
                                objInventoryAnalysisMonthWise.AverageCost = objItem.AverageCost;
                                objInventoryAnalysisMonthWise.IsBuildBreak = objItem.IsBuildBreak;
                                objInventoryAnalysisMonthWise.BondedInventory = objItem.BondedInventory;
                                objInventoryAnalysisMonthWise.OnReturnQuantity = objItem.OnReturnQuantity;
                                objInventoryAnalysisMonthWise.TrendingSetting = objItem.TrendingSetting;
                                objInventoryAnalysisMonthWise.PullQtyScanOverride = objItem.PullQtyScanOverride;
                                objInventoryAnalysisMonthWise.IsAutoInventoryClassification = objItem.IsAutoInventoryClassification;
                                objInventoryAnalysisMonthWise.IsPackslipMandatoryAtReceive = objItem.IsPackslipMandatoryAtReceive;
                                objInventoryAnalysisMonthWise.SuggestedTransferQuantity = objItem.SuggestedTransferQuantity;
                                objInventoryAnalysisMonthWise.QtyToMeetDemand = objItem.QtyToMeetDemand;

                                if (TobeAdded == 1)
                                {
                                    context.InventoryAnalysisMonthWises.Add(objInventoryAnalysisMonthWise);
                                }
                            }
                            catch (Exception EX)
                            {
                                ExceptionString = ExceptionString + (ExceptionString == "" ? "" : Environment.NewLine) +
                                                  "--==========[" + base.DataBaseName + ">>" + CompanyId.ToString() + ">>" + RoomId.ToString() + ">>" + objItem.ItemNumber + "]==========--";
                                ExceptionString = ExceptionString + Environment.NewLine + "Exception Message:" + Environment.NewLine + EX.Message;
                                if (EX.InnerException != null && !String.IsNullOrEmpty(EX.InnerException.Message))
                                {
                                    ExceptionString = ExceptionString + Environment.NewLine + "Inner Exception Message:" + Environment.NewLine + EX.InnerException.Message;
                                }
                            }
                        }
                    }
                    context.SaveChanges();

                }

                return true;
            }
            catch (Exception EX)
            {
                ExceptionString = "--==========[" + base.DataBaseName + ">>" + CompanyId.ToString() + ">>" + RoomId.ToString() + "]==========--";
                ExceptionString = ExceptionString + Environment.NewLine + "Exception Message:" + Environment.NewLine + EX.Message;
                if (EX.InnerException != null && !String.IsNullOrEmpty(EX.InnerException.Message))
                {
                    ExceptionString = ExceptionString + Environment.NewLine + "Inner Exception Message:" + Environment.NewLine + EX.InnerException.Message;
                }
                return false;
            }
        }
        public bool CalculateMonthisedRoomAnalytics_History(long RoomId, long CompanyId, int ForMonth, int ForYear, out string ExceptionString, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null, DateTime? CurrentUTCDateTime = null)
        {
            try
            {
                ExceptionString = "";
                DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                if (objRegionalSettings == null)
                {
                    objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
                }
                if (objDashboardParameterDTO == null)
                {
                    objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
                }
                string CurrentRoomTimeZone = "UTC";

                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName;
                    if (string.IsNullOrWhiteSpace(CurrentRoomTimeZone))
                    {
                        CurrentRoomTimeZone = "UTC";
                    }
                }

                //DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    double PullCost = 0;
                    double ItemInventoryValue = 0;
                    double OnHandQuantity = 0;
                    double PullQuantity = 0;
                    double OrderedQty = 0;
                    double ReturnOrderedQty = 0;
                    double FinalOrderedQty = 0;
                    int daysinmonth = DateTime.DaysInMonth(ForYear, ForMonth);
                    DateTime? FromDate = new DateTime(ForYear, ForMonth, 1, 0, 0, 0);
                    DateTime Todate = new DateTime(ForYear, ForMonth, daysinmonth, 23, 59, 0);
                    FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                    Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                    double minQty = 0;
                    double maxQty = 0;
                    BinMasterDAL objBinMaster = new BinMasterDAL(base.DataBaseName);
                    InventoryAnalysisMonthWise objInventoryAnalysisMonthWise = context.InventoryAnalysisMonthWises.FirstOrDefault(t => t.RoomId == RoomId && t.CompanyId == CompanyId && t.ItemGUID == Guid.Empty && t.CalculationMonth == ForMonth && t.CalculationYear == ForYear);
                    int TobeAdded = 0;
                    if (objInventoryAnalysisMonthWise == null)
                    {
                        objInventoryAnalysisMonthWise = new InventoryAnalysisMonthWise();
                        TobeAdded = 1;
                    }
                    objInventoryAnalysisMonthWise.ItemGUID = Guid.Empty;
                    objInventoryAnalysisMonthWise.CalculationDate = DateTime.UtcNow;
                    objInventoryAnalysisMonthWise.CalculationFor = 2;
                    objInventoryAnalysisMonthWise.CalculationMonth = ForMonth;
                    objInventoryAnalysisMonthWise.CalculationYear = ForYear;
                    objInventoryAnalysisMonthWise.CompanyId = CompanyId;
                    objInventoryAnalysisMonthWise.RoomId = RoomId;

                    double Avgminmax = 0;
                    //var qryitems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false);

                    List<ItemMaster> qryitems = objDashboardDAL.GetItemMasterHistory_InventoryMonthlyAnalysis_All(CompanyId, RoomId, CurrentUTCDateTime);


                    if (qryitems.Any())
                    {
                        OnHandQuantity = qryitems.Sum(t => (t.OnHandQuantity ?? 0));
                        ItemInventoryValue = qryitems.Sum(t => (t.ExtendedCost ?? 0));
                        //Avgminmax = qryitems.Sum(t => ((t.MinimumQuantity) + (t.MaximumQuantity) / 2));
                    }

                    Avgminmax = GetMinMaxAvgByItemAndLocation(RoomId, CompanyId);

                    List<PullMaster> PullMasterList = null;
                    List<OrderDetailsDTO> lstOrderDetails = null;
                    List<TransferDetailDTO> transferList = null;
                    double transferCost = 0;
                    double transferQuantity = 0;


                    PullMasterList = GetPullByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);//context.Database.SqlQuery<PullMaster>("exec GetPullByRoomAndDateRange @RoomID,@CompanyID,@FromDate,@ToDate ", paramsInner).ToList();
                    transferList = GetTransferByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);

                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                    {
                        PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                        PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                    }

                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                    {
                        PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                        PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                    }

                    if (transferList != null && transferList.Any() && transferList.Count() > 0)
                    {
                        transferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                        transferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                        transferQuantity = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                        transferQuantity -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                    }

                    double pullTransferQty = PullQuantity + transferQuantity;
                    double pullTransferCost = PullCost + transferCost;

                    lstOrderDetails = GetOrderByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);//context.Database.SqlQuery<OrderDetailsDTO>("exec GetOrderByRoomAndDateRange @RoomID,@CompanyID,@FromDate,@ToDate ", paramInnerCase).ToList();

                    if (lstOrderDetails.Any())
                    {
                        OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                        ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                    }
                    FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                    objInventoryAnalysisMonthWise.MaximumQuantity = maxQty;
                    objInventoryAnalysisMonthWise.MinimumQuantity = minQty;
                    objInventoryAnalysisMonthWise.MonthizedInventoryValue = ItemInventoryValue;
                    objInventoryAnalysisMonthWise.MonthizedOnHandQuantity = OnHandQuantity;
                    objInventoryAnalysisMonthWise.MonthizedOrderQuantity = FinalOrderedQty;
                    objInventoryAnalysisMonthWise.MonthizedPullQuantity = pullTransferQty;
                    objInventoryAnalysisMonthWise.MonthizedPullCost = pullTransferCost;
                    double AverageExtendedCost = 0, AverageOnHandQuantity = 0;
                    ItemAvgCostQtyInfo objItemAvgCostQtyInfo = objDashboardDAL.GetItemOrRoomAvgCostQty(Guid.Empty, RoomId, CompanyId, (FromDate ?? DateTime.Now.AddDays(1)), Todate);
                    if (objItemAvgCostQtyInfo != null)
                    {
                        AverageExtendedCost = objItemAvgCostQtyInfo.AverageExtendedCost;
                        AverageOnHandQuantity = objItemAvgCostQtyInfo.AverageOnHandQuantity;
                    }

                    if (AverageExtendedCost > 0)
                    {
                        objInventoryAnalysisMonthWise.MonthizedPullValueTurns = (pullTransferCost / AverageExtendedCost) * 12;
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedPullValueTurns = 0;
                    }
                    if (AverageOnHandQuantity > 0)
                    {
                        objInventoryAnalysisMonthWise.MonthizedPullTurns = (pullTransferQty / AverageOnHandQuantity) * 12;
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedPullTurns = 0;
                    }
                    if (Avgminmax > 0)
                    {
                        objInventoryAnalysisMonthWise.MonthizedOrderTurns = (FinalOrderedQty / Avgminmax);
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedOrderTurns = 0;
                    }

                    if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4)
                    {
                        if (objDashboardParameterDTO.TurnsMeasureMethod == 1)
                        {
                            objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullValueTurns;
                        }
                        else if (objDashboardParameterDTO.TurnsMeasureMethod == 2)
                        {
                            objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullTurns;
                        }
                        else
                        {
                            objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedOrderTurns;
                        }
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullTurns;
                    }

                    objInventoryAnalysisMonthWise.MonthizedPullValueAverageUsage = pullTransferCost / daysinmonth;
                    objInventoryAnalysisMonthWise.MonthizedPullAverageUsage = pullTransferQty / daysinmonth;
                    objInventoryAnalysisMonthWise.MonthizedOrderAverageUsage = FinalOrderedQty / daysinmonth;

                    if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4)
                    {
                        if (objDashboardParameterDTO.AUMeasureMethod == 1)
                        {
                            objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullValueAverageUsage;
                        }
                        else if (objDashboardParameterDTO.AUMeasureMethod == 2)
                        {
                            objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullAverageUsage;
                        }
                        else
                        {
                            objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedOrderAverageUsage;
                        }
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullAverageUsage;
                    }



                    DashboardAnalysisInfo objDashboardAnalysisInfoTurn = GetTurnsByRoom_History(RoomId, CompanyId, 0, objDashboardParameterDTO, objRegionalSettings, CurrentUTCDateTime);
                    DashboardAnalysisInfo objDashboardAnalysisInfoAU = GetAvgUsageByRoom_History(RoomId, CompanyId, 0, objDashboardParameterDTO, objRegionalSettings, CurrentUTCDateTime);

                    objInventoryAnalysisMonthWise.AnnualizedOrderTurns = objDashboardAnalysisInfoTurn.MonthizedOrderTurns ?? 0;
                    objInventoryAnalysisMonthWise.AnnualizedPullTurns = objDashboardAnalysisInfoTurn.MonthizedPullTurns ?? 0;
                    objInventoryAnalysisMonthWise.AnnualizedPullValueTurns = objDashboardAnalysisInfoTurn.MonthizedPullValueTurns ?? 0;

                    objInventoryAnalysisMonthWise.AnnualizedPullAverageUsage = objDashboardAnalysisInfoAU.CalculatedPullAverageUsage;
                    objInventoryAnalysisMonthWise.AnnualizedOrderAverageUsage = objDashboardAnalysisInfoAU.CalculatedOrderAverageUsage;
                    objInventoryAnalysisMonthWise.AnnualizedPullValueAverageUsage = objDashboardAnalysisInfoAU.CalculatedPullValueAverageUsage;


                    //if (objDashboardParameterDTO != null)
                    //{

                    //}
                    //if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.TurnsMonthsOfUsageToSample ?? 0) > 0)
                    //{

                    //}
                    //else
                    //{

                    //}
                    //if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.AUDayOfUsageToSample ?? 0) > 0)
                    //{

                    //}
                    //else
                    //{

                    //}




                    //MonthizedTurns
                    //MonthizedAverageUsage

                    //MonthizedPullValueTurns
                    //MonthizedPullTurns
                    //MonthizedOrderTurns                        
                    //MonthizedPullValueAverageUsage
                    //MonthizedPullAverageUsage
                    //MonthizedOrderAverageUsage



                    //----------------------NEW COLUMNS ADDED FOR TRACKING----------------------
                    //
                    //objInventoryAnalysisMonthWise.ManufacturerNumber = objItem.ManufacturerNumber;
                    //objInventoryAnalysisMonthWise.SupplierPartNo = objItem.SupplierPartNo;
                    //objInventoryAnalysisMonthWise.UPC = objItem.UPC;
                    //objInventoryAnalysisMonthWise.DefaultReorderQuantity = objItem.DefaultReorderQuantity;
                    //objInventoryAnalysisMonthWise.DefaultPullQuantity = objItem.DefaultPullQuantity;
                    //objInventoryAnalysisMonthWise.Cost = objItem.Cost;
                    //objInventoryAnalysisMonthWise.Markup = objItem.Markup;
                    //objInventoryAnalysisMonthWise.SellPrice = objItem.SellPrice;
                    //objInventoryAnalysisMonthWise.ExtendedCost = objItem.ExtendedCost;
                    //objInventoryAnalysisMonthWise.Trend = objItem.Trend;
                    //objInventoryAnalysisMonthWise.Taxable = objItem.Taxable;
                    //objInventoryAnalysisMonthWise.Consignment = objItem.Consignment;
                    //objInventoryAnalysisMonthWise.StagedQuantity = objItem.StagedQuantity;
                    //objInventoryAnalysisMonthWise.InTransitquantity = objItem.InTransitquantity;
                    //objInventoryAnalysisMonthWise.OnOrderQuantity = objItem.OnOrderQuantity;
                    //objInventoryAnalysisMonthWise.OnTransferQuantity = objItem.OnTransferQuantity;
                    //objInventoryAnalysisMonthWise.SuggestedOrderQuantity = objItem.SuggestedOrderQuantity;
                    //objInventoryAnalysisMonthWise.RequisitionedQuantity = objItem.RequisitionedQuantity;
                    //objInventoryAnalysisMonthWise.AverageUsage = objItem.AverageUsage;
                    //objInventoryAnalysisMonthWise.Turns = objItem.Turns;
                    //objInventoryAnalysisMonthWise.OnHandQuantity = objItem.OnHandQuantity;
                    //objInventoryAnalysisMonthWise.CriticalQuantity = objItem.CriticalQuantity;
                    //objInventoryAnalysisMonthWise.WeightPerPiece = objItem.WeightPerPiece;
                    //objInventoryAnalysisMonthWise.ItemUniqueNumber = objItem.ItemUniqueNumber;
                    //objInventoryAnalysisMonthWise.IsTransfer = objItem.IsTransfer;
                    //objInventoryAnalysisMonthWise.IsPurchase = objItem.IsPurchase;
                    //objInventoryAnalysisMonthWise.DefaultLocation = objItem.DefaultLocation;
                    //objInventoryAnalysisMonthWise.InventoryClassification = objItem.InventoryClassification;
                    //objInventoryAnalysisMonthWise.SerialNumberTracking = objItem.SerialNumberTracking;
                    //objInventoryAnalysisMonthWise.LotNumberTracking = objItem.LotNumberTracking;
                    //objInventoryAnalysisMonthWise.DateCodeTracking = objItem.DateCodeTracking;
                    //objInventoryAnalysisMonthWise.ItemType = objItem.ItemType;
                    //objInventoryAnalysisMonthWise.IsLotSerialExpiryCost = objItem.IsLotSerialExpiryCost;
                    //objInventoryAnalysisMonthWise.IsItemLevelMinMaxQtyRequired = objItem.IsItemLevelMinMaxQtyRequired;
                    //objInventoryAnalysisMonthWise.IsEnforceDefaultReorderQuantity = objItem.IsEnforceDefaultReorderQuantity;
                    //objInventoryAnalysisMonthWise.AverageCost = objItem.AverageCost;
                    //objInventoryAnalysisMonthWise.IsBuildBreak = objItem.IsBuildBreak;
                    //objInventoryAnalysisMonthWise.BondedInventory = objItem.BondedInventory;
                    //objInventoryAnalysisMonthWise.OnReturnQuantity = objItem.OnReturnQuantity;
                    //objInventoryAnalysisMonthWise.TrendingSetting = objItem.TrendingSetting;
                    //objInventoryAnalysisMonthWise.PullQtyScanOverride = objItem.PullQtyScanOverride;
                    //objInventoryAnalysisMonthWise.IsAutoInventoryClassification = objItem.IsAutoInventoryClassification;
                    //objInventoryAnalysisMonthWise.IsPackslipMandatoryAtReceive = objItem.IsPackslipMandatoryAtReceive;
                    //objInventoryAnalysisMonthWise.SuggestedTransferQuantity = objItem.SuggestedTransferQuantity;
                    //objInventoryAnalysisMonthWise.QtyToMeetDemand = objItem.QtyToMeetDemand;

                    if (TobeAdded == 1)
                    {
                        context.InventoryAnalysisMonthWises.Add(objInventoryAnalysisMonthWise);
                    }

                    context.SaveChanges();

                }

                return true;
            }
            catch (Exception EX)
            {
                ExceptionString = "--==========[" + base.DataBaseName + ">>" + CompanyId.ToString() + ">>" + RoomId.ToString() + "]==========--";
                ExceptionString = ExceptionString + Environment.NewLine + "Exception Message:" + Environment.NewLine + EX.Message;
                if (EX.InnerException != null && !String.IsNullOrEmpty(EX.InnerException.Message))
                {
                    ExceptionString = ExceptionString + Environment.NewLine + "Inner Exception Message:" + Environment.NewLine + EX.InnerException.Message;
                }
                return false;
            }
        }

        public DashboardAnalysisInfo GetTurnsByRoom_History(long RoomId, long CompanyId, long UserId, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null, DateTime? CurrentUTCDateTime = null) //,byte? TurnsMeasureMethod,int? TurnsMonthsOfUsageToSample
        {
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            string CurrentRoomTimeZone = "UTC";

            if (objDashboardParameterDTO == null)
            {
                objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
            }

            if (objRegionalSettings == null)
            {
                objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
            }

            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }

            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, CurrentUTCDateTime ?? DateTime.UtcNow);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.TurnsDaysOfUsageToSample ?? 0) > 0)
                {
                    double OnhandQty = 0;
                    double PullCost = 0;
                    double ItemInventoryValue = 0;
                    double PullQuantity = 0;
                    double OrderedQty = 0;
                    double ReturnOrderedQty = 0;
                    double FinalOrderedQty = 0;
                    //double onHandQty = 0;
                    DateTime? FromDate = CurrentTimeofTimeZone.AddDays(objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value * (-1)).Date;
                    if (FromDate.HasValue)
                    {
                        FromDate = new DateTime(FromDate.Value.Year, FromDate.Value.Month, 1, FromDate.Value.Hour, FromDate.Value.Minute, FromDate.Value.Second);
                    }

                    DateTime Todate = new DateTime(CurrentTimeofTimeZone.AddMonths(-1).Date.Year, CurrentTimeofTimeZone.AddMonths(-1).Date.Month, DateTime.DaysInMonth(CurrentTimeofTimeZone.AddMonths(-1).Date.Year, CurrentTimeofTimeZone.AddMonths(-1).Date.Month), 23, 59, 0);
                    FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                    Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);
                    double minQty = 0;
                    double maxQty = 0;
                    double DurationDiviser = 12 / (objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value / 30); //12 / objDashboardParameterDTO.TurnsMonthsOfUsageToSample.Value;
                    double Avgminmax = (maxQty + minQty) / 2;
                    List<PullMaster> PullMasterList = null;
                    List<OrderDetailsDTO> lstOrderDetails = null;
                    List<TransferDetailDTO> transferList = null;
                    double transferCost = 0;
                    double transferQuantity = 0;

                    PullMasterList = GetPullByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate); //context.Database.SqlQuery<PullMaster>("exec GetPullByRoomAndDateRange @RoomID,@CompanyID,@FromDate,@ToDate ", paramsInner).ToList();
                    transferList = GetTransferByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);

                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                    {
                        PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                        PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                    }
                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                    {
                        PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                        PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                    }

                    if (transferList != null && transferList.Any() && transferList.Count() > 0)
                    {
                        transferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                        transferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                        transferQuantity = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                        transferQuantity -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                    }

                    double pullTransferQty = PullQuantity + transferQuantity;
                    double pullTransferCost = PullCost + transferCost;
                    lstOrderDetails = GetOrderByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);

                    if (lstOrderDetails.Any())
                    {
                        OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                        ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                    }
                    FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                    //var qryitems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false);

                    List<ItemMaster> qryitems = objDashboardDAL.GetItemMasterHistory_InventoryMonthlyAnalysis_All(CompanyId, RoomId, CurrentUTCDateTime);
                    if (qryitems.Any())
                    {
                        OnhandQty = qryitems.Sum(t => (t.OnHandQuantity ?? 0));
                        ItemInventoryValue = qryitems.Sum(t => (t.ExtendedCost ?? 0));
                        Avgminmax = qryitems.Sum(t => ((t.MinimumQuantity) + (t.MaximumQuantity) / 2));
                    }

                    if (pullTransferCost > 0)
                    {
                        if (ItemInventoryValue > 0)
                        {
                            objDashboardAnalysisInfo.MonthizedPullValueTurns = ((pullTransferCost) / (ItemInventoryValue)) * (DurationDiviser);
                        }
                        else
                        {
                            objDashboardAnalysisInfo.MonthizedPullValueTurns = 0;
                        }
                    }
                    else
                    {
                        objDashboardAnalysisInfo.MonthizedPullValueTurns = 0;
                    }

                    if (pullTransferQty > 0 && OnhandQty > 0)
                    {
                        objDashboardAnalysisInfo.MonthizedPullTurns = (pullTransferQty / OnhandQty) * DurationDiviser;
                    }
                    else
                    {
                        objDashboardAnalysisInfo.MonthizedPullTurns = 0;
                    }
                    if (Avgminmax > 0 && FinalOrderedQty > 0)
                    {
                        objDashboardAnalysisInfo.MonthizedOrderTurns = ((FinalOrderedQty) / Avgminmax) * DurationDiviser;
                    }
                    else
                    {
                        objDashboardAnalysisInfo.MonthizedOrderTurns = 0;
                    }


                }
            }
            return objDashboardAnalysisInfo;
        }
        public DashboardAnalysisInfo GetAvgUsageByRoom_History(long RoomId, long CompanyId, long UserId, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null, DateTime? CurrentUTCDateTime = null)
        {
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            string CurrentRoomTimeZone = "UTC";
            if (objDashboardParameterDTO == null)
            {
                //if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["DashboardParams"] != null)
                //{
                //    objDashboardParameterDTO = (DashboardParameterDTO)HttpContext.Current.Session["DashboardParams"];
                //}
                //else
                {
                    objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
                }
            }
            if (objRegionalSettings == null)
            {
                //if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["eTurnsRegionInfoProp"] != null)
                //{
                //    objRegionalSettings = (eTurnsRegionInfo)HttpContext.Current.Session["eTurnsRegionInfoProp"];
                //}
                //else
                {
                    objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
                }
            }
            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }
            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, CurrentUTCDateTime ?? DateTime.UtcNow);


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.AUDayOfUsageToSample ?? 0) > 0)
                {

                    DateTime? FromDate = CurrentTimeofTimeZone.AddDays(objDashboardParameterDTO.AUDayOfUsageToSample.Value * (-1)).Date;
                    DateTime Todate = new DateTime(CurrentTimeofTimeZone.Year, CurrentTimeofTimeZone.Month, CurrentTimeofTimeZone.Day, 23, 59, 0);
                    FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                    Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                    double PullCost = 0;
                    double PullQuantity = 0;
                    double OrderedQty = 0;
                    double ReturnOrderedQty = 0;
                    double FinalOrderedQty = 0;

                    List<PullMaster> PullMasterList = null;
                    List<OrderDetailsDTO> lstOrderDetails = null;
                    List<TransferDetailDTO> transferList = null;
                    double transferCost = 0;
                    double transferQuantity = 0;
                    PullMasterList = GetPullByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);
                    transferList = GetTransferByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);

                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                    {
                        PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                        PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                    }
                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                    {
                        PullCost = PullCost - PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum();
                        PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                    }

                    if (transferList != null && transferList.Any() && transferList.Count() > 0)
                    {
                        transferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                        transferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                        transferQuantity = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                        transferQuantity -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                    }

                    double pullTransferQty = PullQuantity + transferQuantity;
                    double pullTransferCost = PullCost + transferCost;

                    lstOrderDetails = GetOrderByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);

                    if (lstOrderDetails.Any())
                    {
                        OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                        ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                    }

                    FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                    if (pullTransferCost > 0)
                    {
                        objDashboardAnalysisInfo.CalculatedPullValueAverageUsage = pullTransferCost / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                    }
                    else
                    {
                        objDashboardAnalysisInfo.CalculatedPullValueAverageUsage = 0;
                    }

                    if (pullTransferQty > 0)
                    {
                        objDashboardAnalysisInfo.CalculatedPullAverageUsage = pullTransferQty / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                    }
                    else
                    {
                        objDashboardAnalysisInfo.CalculatedPullAverageUsage = 0;
                    }

                    if (FinalOrderedQty > 0)
                    {
                        objDashboardAnalysisInfo.CalculatedOrderAverageUsage = FinalOrderedQty / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                    }
                    else
                    {
                        objDashboardAnalysisInfo.CalculatedOrderAverageUsage = 0;
                    }
                }
            }
            return objDashboardAnalysisInfo;
        }


        public List<DashboardItem> GetItemsForItemDashboardDB(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, List<long> SupplierIds, bool? UserConsignmentAllowed, DateTime? FromDate, DateTime? ToDate, int MaxItemsInGraph, bool ApplyDateFilter, string TabName, bool GetTabCounts)
        {
            DataSet dsDashitems = new DataSet();
            List<DashboardItem> lstItems = new List<DashboardItem>();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string SpName = "GetPagedItemsDashboard";
            TotalCount = 0;

            if (string.IsNullOrWhiteSpace(Connectionstring))
            {
                return lstItems;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            switch (TabName.ToLower())
            {
                case "stock out":
                    SpName = "GetPagedstockoutsDashboard";
                    break;
                case "critical":
                case "minimum":
                case "maximum":
                    SpName = "GetPagedMinMaxCritItemsDashboard";
                    break;
                case "slow moving":
                case "fast moving":
                    SpName = "GetPagedItemsDashboard";
                    break;

            }

            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsDashitems = SqlHelper.ExecuteDataset(EturnsConnection, SpName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, FromDate, ToDate, MaxItemsInGraph, ApplyDateFilter, TabName.ToLower(), GetTabCounts);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                dsDashitems = SqlHelper.ExecuteDataset(EturnsConnection, SpName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, FromDate, ToDate, MaxItemsInGraph, ApplyDateFilter, TabName.ToLower(), GetTabCounts);
            }
            else
            {
                dsDashitems = SqlHelper.ExecuteDataset(EturnsConnection, SpName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomID, CompanyID, strSupplierIds, UserConsignmentAllowed, FromDate, ToDate, MaxItemsInGraph, ApplyDateFilter, TabName.ToLower(), GetTabCounts);
            }

            if (dsDashitems != null && dsDashitems.Tables.Count > 0)
            {
                DataTable dtItems = dsDashitems.Tables[0];
                if (dtItems != null && dtItems.Rows.Count > 0)
                {

                    if (!GetTabCounts)
                    {
                        TotalCount = Convert.ToInt32(dtItems.Rows[0]["TotalRecords"]);
                        lstItems = (from dr in dtItems.AsEnumerable()
                                    select new DashboardItem
                                    {
                                        ID = dr.Field<long>("ID"),
                                        GUID = dr.Field<Guid>("GUID"),
                                        Description = dr.Field<string>("Description"),
                                        InventoryClassificationName = dr.Field<string>("InventoryClassificationName"),
                                        ItemNumber = dr.Field<string>("ItemNumber"),
                                        LongDescription = dr.Field<string>("LongDescription"),
                                        SupplierName = dr.Field<string>("SupplierName"),
                                        SupplierPartNo = dr.Field<string>("SupplierPartNo"),
                                        AverageUsage = dr.Field<double?>("AverageUsage"),
                                        Cost = dr.Field<double?>("Cost"),
                                        CriticalQuantity = dr.Field<double>("CriticalQuantity"),
                                        DefaultReorderQuantity = dr.Field<double?>("DefaultReorderQuantity"),
                                        InventoryClassification = dr.Field<int?>("InventoryClassification"),
                                        MaximumQuantity = dr.Field<double?>("MaximumQuantity") ?? 0.0,
                                        MinimumQuantity = dr.Field<double?>("MinimumQuantity") ?? 0.0,
                                        OnHandQuantity = dr.Field<double?>("OnHandQuantity"),
                                        OnOrderQuantity = dr.Field<double?>("OnOrderQuantity"),
                                        OnTransferQuantity = dr.Field<double?>("OnTransferQuantity"),
                                        //RationalFactor = dr.Field<double?>("RationalFactor"),
                                        RequisitionedQuantity = dr.Field<double?>("RequisitionedQuantity"),
                                        StockOutCount = dr.Field<int>("StockOutCount"),
                                        SuggestedOrderQuantity = dr.Field<double?>("SuggestedOrderQuantity"),
                                        SupplierID = dr.Field<long?>("SupplierID"),
                                        Turns = dr.Field<double?>("Turns"),
                                        SlowMovingValue = dr.Field<double>("SlowMovingValue"),
                                        FastMovingValue = dr.Field<double>("FastMovingValue"),
                                        CategoryID = dr.Field<long?>("CategoryID"),
                                        CategoryName = dr.Field<string>("CategoryName"),

                                        ManufacturerID = dr.Field<long?>("ManufacturerID"),
                                        ManufacturerNumber = dr.Field<string>("ManufacturerNumber"),
                                        UPC = dr.Field<string>("UPC"),
                                        UNSPSC = dr.Field<string>("UNSPSC"),
                                        GLAccountID = dr.Field<long?>("GLAccountID"),
                                        UOMID = dr.Field<long?>("UOMID"),
                                        PricePerTerm = dr.Field<double?>("PricePerTerm"),
                                        DefaultPullQuantity = dr.Field<double?>("DefaultPullQuantity"),
                                        Markup = dr.Field<double?>("Markup"),
                                        SellPrice = dr.Field<double?>("SellPrice"),
                                        ExtendedCost = dr.Field<double?>("ExtendedCost"),
                                        LeadTimeInDays = dr.Field<int?>("LeadTimeInDays"),
                                        Link1 = dr.Field<string>("Link1"),
                                        Link2 = dr.Field<string>("Link2"),
                                        Trend = dr.Field<bool?>("Trend"),
                                        Taxable = dr.Field<bool?>("Taxable"),
                                        Consignment = dr.Field<bool?>("Consignment"),
                                        StagedQuantity = dr.Field<double?>("StagedQuantity"),
                                        InTransitquantity = dr.Field<double?>("InTransitquantity"),
                                        OnOrderInTransitQuantity = dr.Field<double?>("OnOrderInTransitQuantity"),
                                        WeightPerPiece = dr.Field<double?>("WeightPerPiece"),
                                        ItemUniqueNumber = dr.Field<string>("ItemUniqueNumber"),
                                        IsTransfer = dr.Field<bool?>("IsTransfer"),
                                        IsPurchase = dr.Field<bool?>("IsPurchase"),
                                        DefaultLocation = dr.Field<long?>("DefaultLocation"),
                                        SerialNumberTracking = dr.Field<bool?>("SerialNumberTracking"),
                                        LotNumberTracking = dr.Field<bool?>("LotNumberTracking"),
                                        DateCodeTracking = dr.Field<bool?>("DateCodeTracking"),
                                        ItemType = dr.Field<int?>("ItemType"),
                                        ImagePath = dr.Field<string>("ImagePath"),
                                        UDF1 = dr.Field<string>("UDF1"),
                                        UDF2 = dr.Field<string>("UDF2"),
                                        UDF3 = dr.Field<string>("UDF3"),
                                        UDF4 = dr.Field<string>("UDF4"),
                                        UDF5 = dr.Field<string>("UDF5"),
                                        Created = dr.Field<System.DateTime?>("Created"),
                                        Updated = dr.Field<System.DateTime?>("Updated"),
                                        CreatedBy = dr.Field<long?>("CreatedBy"),
                                        LastUpdatedBy = dr.Field<long?>("LastUpdatedBy"),
                                        CreatedByName = dr.Field<string>("CreatedByName"),
                                        UpdatedByName = dr.Field<string>("UpdatedByName"),
                                        IsDeleted = dr.Field<bool?>("IsDeleted"),
                                        IsArchived = dr.Field<bool?>("IsArchived"),
                                        CompanyID = dr.Field<long?>("CompanyID"),
                                        Room = dr.Field<long?>("Room"),
                                        IsLotSerialExpiryCost = dr.Field<string>("IsLotSerialExpiryCost"),
                                        PackingQuantity = dr.Field<double?>("PackingQuantity"),
                                        IsItemLevelMinMaxQtyRequired = dr.Field<bool?>("IsItemLevelMinMaxQtyRequired"),
                                        IsEnforceDefaultReorderQuantity = dr.Field<bool?>("IsEnforceDefaultReorderQuantity"),
                                        AverageCost = dr.Field<double?>("AverageCost"),
                                        IsBuildBreak = dr.Field<bool?>("IsBuildBreak"),
                                        BondedInventory = dr.Field<string>("BondedInventory"),
                                        CostUOMID = dr.Field<long?>("CostUOMID"),
                                        WhatWhereAction = dr.Field<string>("WhatWhereAction"),
                                        OnReturnQuantity = dr.Field<double?>("OnReturnQuantity"),
                                        IsBOMItem = dr.Field<bool?>("IsBOMItem"),
                                        RefBomId = dr.Field<long?>("RefBomId"),
                                        TrendingSetting = dr.Field<Byte?>("TrendingSetting"),
                                        PullQtyScanOverride = dr.Field<bool?>("PullQtyScanOverride"),
                                        IsAutoInventoryClassification = dr.Field<bool?>("IsAutoInventoryClassification"),
                                        IsPackslipMandatoryAtReceive = dr.Field<bool?>("IsPackslipMandatoryAtReceive"),
                                        SuggestedTransferQuantity = dr.Field<double?>("SuggestedTransferQuantity"),
                                        LastCost = dr.Field<double?>("LastCost"),
                                        ReceivedOn = dr.Field<System.DateTime?>("ReceivedOn"),
                                        ReceivedOnWeb = dr.Field<System.DateTime?>("ReceivedOnWeb"),
                                        AddedFrom = dr.Field<string>("AddedFrom"),
                                        EditedFrom = dr.Field<string>("EditedFrom"),
                                        ordereddate = dr.Field<System.DateTime?>("ordereddate"),
                                        pulleddate = dr.Field<System.DateTime?>("pulleddate"),
                                        counteddate = dr.Field<System.DateTime?>("counteddate"),
                                        trasnfereddate = dr.Field<System.DateTime?>("trasnfereddate"),
                                        Pricesaveddate = dr.Field<System.DateTime?>("Pricesaveddate"),
                                        ItemImageExternalURL = dr.Field<string>("ItemImageExternalURL"),
                                        ItemDocExternalURL = dr.Field<string>("ItemDocExternalURL"),
                                        ImageType = dr.Field<string>("ImageType"),
                                        QtyToMeetDemand = dr.Field<double?>("QtyToMeetDemand"),
                                        OutTransferQuantity = dr.Field<double?>("OutTransferQuantity"),
                                        ItemLink2ExternalURL = dr.Field<string>("ItemLink2ExternalURL"),
                                        ItemLink2ImageType = dr.Field<string>("ItemLink2ImageType"),
                                        IsActive = dr.Field<bool?>("IsActive"),
                                        ManufacturerName = dr.Field<string>("ManufacturerName"),
                                        DefaultLocationName = dr.Field<string>("DefaultLocationName"),
                                        DefaultLocationGUID = dr.Field<System.Guid?>("DefaultLocationGUID"),
                                        BinDeleted = dr.Field<bool?>("BinDeleted"),
                                        CostUOMName = dr.Field<string>("CostUOMName"),
                                        BPONumber = dr.Field<string>("BPONumber"),
                                        ItemTypeName = dr.Field<string>("ItemTypeName"),
                                        ItemWithBin = dr.Field<string>("ItemWithBin"),
                                        BinNumber = dr.Field<string>("BinNumber"),
                                        PerItemCost = dr.Field<double?>("PerItemCost"),
                                        UDF6 = dr.Field<string>("UDF6"),
                                        UDF7 = dr.Field<string>("UDF7"),
                                        UDF8 = dr.Field<string>("UDF8"),
                                        UDF9 = dr.Field<string>("UDF9"),
                                        UDF10 = dr.Field<string>("UDF10")
                                    }).ToList();
                    }
                    else
                    {
                        //lstItems = (from dr in dtItems.AsEnumerable()
                        //            select new DashboardItem
                        //            {
                        //                ID = dr.Field<long>("ID"),
                        //                CritCount = dtItems.Columns.Contains("CritCount") ? dr.Field<int>("CritCount") : 0,
                        //                MaxCount = dtItems.Columns.Contains("MaxCount") ? dr.Field<int>("MaxCount") : 0,
                        //                MinCount = dtItems.Columns.Contains("MinCount") ? dr.Field<int>("MinCount") : 0,
                        //                FastMovingCount = dtItems.Columns.Contains("FastMovingCount") ? dr.Field<int>("FastMovingCount") : 0,
                        //                SlowMovingCount = dtItems.Columns.Contains("SlowMovingCount") ? dr.Field<int>("SlowMovingCount") : 0
                        //            }).ToList();

                        for (int i = 0; i < dtItems.Rows.Count; i++)
                        {
                            var dr = dtItems.Rows[i];
                            var item = new DashboardItem();
                            long lngVal = 0;
                            item.ID = Convert.ToInt64(dr["ID"].ToString());
                            if (dtItems.Columns.Contains("CritCount"))
                            {
                                if (long.TryParse(dr["CritCount"].ToString(), out lngVal))
                                {
                                    item.CritCount = lngVal;
                                }
                            }

                            if (dtItems.Columns.Contains("MaxCount"))
                            {
                                if (long.TryParse(dr["MaxCount"].ToString(), out lngVal))
                                {
                                    item.MaxCount = lngVal;
                                }
                            }

                            if (dtItems.Columns.Contains("MinCount"))
                            {
                                if (long.TryParse(dr["MinCount"].ToString(), out lngVal))
                                {
                                    item.MinCount = lngVal;
                                }
                            }
                            if (dtItems.Columns.Contains("FastMovingCount"))
                            {
                                if (long.TryParse(dr["FastMovingCount"].ToString(), out lngVal))
                                {
                                    item.FastMovingCount = lngVal;
                                }
                            }

                            if (dtItems.Columns.Contains("SlowMovingCount"))
                            {
                                if (long.TryParse(dr["SlowMovingCount"].ToString(), out lngVal))
                                {
                                    item.SlowMovingCount = lngVal;
                                }
                            }

                            lstItems.Add(item);
                        }

                    }
                }
            }
            return lstItems;
        }

        public List<DashboardItem> GetPagedItemsForDashboardInventory(out IEnumerable<DashboardItem> SupplierList, out IEnumerable<DashboardItem> CategoryList, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, List<long> UserSupplierIds, bool? UserConsignmentAllowed, string TabName, bool GetTabCounts, string SelectedSupplierIds, string SelectedCategoryIds)
        {
            DataSet dsDashitems = new DataSet();
            List<DashboardItem> lstItems = new List<DashboardItem>();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string SpName = "GetPagedItemsForDashboardInventory";
            TotalCount = 0;

            if (string.IsNullOrWhiteSpace(Connectionstring))
            {
                SupplierList = null;
                CategoryList = null;
                return lstItems;
            }

            if (MaxRows < 1)
            {
                MaxRows = 10;
            }

            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            switch (TabName.ToLower())
            {
                case "stock out":
                    SpName = "GetPagedStockoutsForDashboardInventory";
                    break;
                case "critical":
                case "minimum":
                case "maximum":
                    SpName = "GetPagedMinMaxCriticalForDashboardInventory";
                    break;
                case "slow moving":
                case "fast moving":
                    SpName = "GetPagedItemsForDashboardInventory";
                    break;

            }

            string strUserSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strUserSupplierIds = string.Join(",", UserSupplierIds);
            }

            string strSelectedSupplierIds = string.Empty;

            if (!string.IsNullOrEmpty(SelectedSupplierIds) && !string.IsNullOrWhiteSpace(SelectedSupplierIds))
            {
                strSelectedSupplierIds = SelectedSupplierIds;
            }

            string strSelectedCategoryIds = string.Empty;

            if (!string.IsNullOrEmpty(SelectedCategoryIds) && !string.IsNullOrWhiteSpace(SelectedCategoryIds))
            {
                strSelectedCategoryIds = SelectedCategoryIds;
            }

            dsDashitems = SqlHelper.ExecuteDataset(EturnsConnection, SpName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomID, CompanyID, strUserSupplierIds, UserConsignmentAllowed, TabName.ToLower(), GetTabCounts, strSelectedSupplierIds, strSelectedCategoryIds);

            if (dsDashitems != null && dsDashitems.Tables.Count > 0)
            {
                SupplierList = DataTableHelper.ToList<DashboardItem>(dsDashitems.Tables[0]);
                CategoryList = DataTableHelper.ToList<DashboardItem>(dsDashitems.Tables[1]);

                if (dsDashitems.Tables.Count > 2)
                {
                    lstItems = DataTableHelper.ToList<DashboardItem>(dsDashitems.Tables[2]);

                    if (lstItems != null && lstItems.Count() > 0)
                    {
                        TotalCount = lstItems.ElementAt(0).TotalRecords;
                    }
                }
            }
            else
            {
                SupplierList = null;
                CategoryList = null;
            }
            return lstItems;
        }

        public List<DashboardItem> GetPagedItemsForDashboardInventoryChart(out IEnumerable<DashboardItem> SupplierList, out IEnumerable<DashboardItem> CategoryList, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, List<long> UserSupplierIds, bool? UserConsignmentAllowed, string TabName, bool GetTabCounts, string SelectedSupplierIds, string SelectedCategoryIds)
        {
            DataSet dsDashitems = new DataSet();
            List<DashboardItem> lstItems = new List<DashboardItem>();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string SpName = "GetPagedItemsForDashboardInventoryChart";
            TotalCount = 0;

            if (string.IsNullOrWhiteSpace(Connectionstring))
            {
                SupplierList = null;
                CategoryList = null;
                return lstItems;
            }

            if (MaxRows < 1)
            {
                MaxRows = 10;
            }

            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            switch (TabName.ToLower())
            {
                case "stock out":
                    SpName = "GetPagedStockoutsForDashboardInventoryChart";
                    break;
                case "critical":
                case "minimum":
                case "maximum":
                    SpName = "GetPagedMinMaxCriticalForDashboardInventoryChart";
                    break;
                case "slow moving":
                case "fast moving":
                    SpName = "GetPagedItemsForDashboardInventoryChart";
                    break;

            }

            string strUserSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strUserSupplierIds = string.Join(",", UserSupplierIds);
            }

            string strSelectedSupplierIds = string.Empty;

            if (!string.IsNullOrEmpty(SelectedSupplierIds) && !string.IsNullOrWhiteSpace(SelectedSupplierIds))
            {
                strSelectedSupplierIds = SelectedSupplierIds;
            }

            string strSelectedCategoryIds = string.Empty;

            if (!string.IsNullOrEmpty(SelectedCategoryIds) && !string.IsNullOrWhiteSpace(SelectedCategoryIds))
            {
                strSelectedCategoryIds = SelectedCategoryIds;
            }

            dsDashitems = SqlHelper.ExecuteDataset(EturnsConnection, SpName, StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomID, CompanyID, strUserSupplierIds, UserConsignmentAllowed, TabName.ToLower(), GetTabCounts, strSelectedSupplierIds, strSelectedCategoryIds);

            if (dsDashitems != null && dsDashitems.Tables.Count > 0)
            {
                SupplierList = DataTableHelper.ToList<DashboardItem>(dsDashitems.Tables[0]);
                CategoryList = DataTableHelper.ToList<DashboardItem>(dsDashitems.Tables[1]);

                if (dsDashitems.Tables.Count > 2)
                {
                    lstItems = DataTableHelper.ToList<DashboardItem>(dsDashitems.Tables[2]);

                    if (lstItems != null && lstItems.Count() > 0)
                    {
                        TotalCount = lstItems.ElementAt(0).TotalRecords;
                    }
                }
            }
            else
            {
                SupplierList = null;
                CategoryList = null;
            }
            return lstItems;
        }

        public ItemTransationInfo GetItemTxnHistory(string txnType, long RoomId, long CompanyId, long ItemId, Guid ItemGuid, string ResMeasurementPullTransfer, string ResMeasurementPullTransferValue
            , string ResMeasurementOrder)
        {
            string qry = string.Empty;
            ItemTransationInfo objItemTransationInfo = new ItemTransationInfo();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                DashboardParameterDTO RoomAnalyticSettings = null;

                RoomAnalyticSettings = (from dp in context.DashboardParameters
                                        where dp.RoomId == RoomId && dp.CompanyId == CompanyId
                                        select new DashboardParameterDTO
                                        {
                                            ID = dp.ID,
                                            RoomId = dp.RoomId ?? 0,
                                            CompanyId = dp.CompanyId,
                                            CreatedOn = dp.CreatedOn,
                                            CreatedBy = dp.CreatedBy,
                                            UpdatedBy = dp.UpdatedBy,
                                            UpdatedOn = dp.UpdatedOn,
                                            TurnsMeasureMethod = dp.TurnsMeasureMethod,
                                            TurnsMonthsOfUsageToSample = dp.TurnsMonthsOfUsageToSample,
                                            TurnsDaysOfUsageToSample = dp.TurnsDaysOfUsageToSample,
                                            AUDayOfUsageToSample = dp.AUDayOfUsageToSample,
                                            AUMeasureMethod = dp.AUMeasureMethod,
                                            AUDaysOfDailyUsage = dp.AUDaysOfDailyUsage,
                                            MinMaxMeasureMethod = dp.MinMaxMeasureMethod,
                                            MinMaxDayOfUsageToSample = dp.MinMaxDayOfUsageToSample,
                                            MinMaxDayOfAverage = dp.MinMaxDayOfAverage,
                                            MinMaxMinNumberOfTimesMax = dp.MinMaxMinNumberOfTimesMax,
                                            MinMaxOptValue1 = dp.MinMaxOptValue1,
                                            MinMaxOptValue2 = dp.MinMaxOptValue2,
                                            GraphFromMonth = dp.GraphFromMonth,
                                            GraphToMonth = dp.GraphToMonth,
                                            GraphFromYear = dp.GraphFromYear,
                                            GraphToYear = dp.GraphToYear,
                                            IsTrendingEnabled = dp.IsTrendingEnabled,
                                            PieChartmetricOn = dp.PieChartmetricOn,
                                            TurnsCalculatedStockRoomTurn = dp.TurnsCalculatedStockRoomTurn,
                                            AUCalculatedDailyUsageOverSample = dp.AUCalculatedDailyUsageOverSample,
                                            MinMaxCalculatedDailyUsageOverSample = dp.MinMaxCalculatedDailyUsageOverSample,
                                            MinMaxCalcAvgPullByDay = dp.MinMaxCalcAvgPullByDay ?? 0,
                                            MinMaxCalcualtedMax = dp.MinMaxCalcualtedMax,
                                            AutoClassification = dp.AutoClassification,
                                            MonthlyAverageUsage = dp.MonthlyAverageUsage,
                                            AnnualCarryingCostPercent = dp.AnnualCarryingCostPercent,
                                            LargestAnnualCashSavings = dp.LargestAnnualCashSavings
                                        }).FirstOrDefault();

                if (RoomAnalyticSettings != null)
                {
                    objItemTransationInfo.RoomAnalyticSettings = RoomAnalyticSettings;
                    switch (txnType)
                    {
                        case "averageusage":
                            if ((RoomAnalyticSettings.AUDayOfUsageToSample ?? 0) > 0 && (RoomAnalyticSettings.AUMeasureMethod ?? 0) > 0)
                            {
                                DateTime? FromDate = DateTime.Now.AddDays((RoomAnalyticSettings.AUDayOfUsageToSample.Value) * (-1)).Date;
                                DateTime Todate = DateTime.Now;
                                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                                objItemMasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, ItemGuid);
                                objItemTransationInfo.TxnStartDate = FromDate;
                                objItemTransationInfo.TxnEndDate = Todate;
                                objItemTransationInfo.ItemId = ItemId;
                                objItemTransationInfo.ItemGUID = ItemGuid;
                                objItemTransationInfo.ItemNumber = objItemMasterDTO.ItemNumber;
                                switch (RoomAnalyticSettings.AUMeasureMethod)
                                {
                                    case 1:
                                        //qry = "Select pm.GUID,isnull(pm.ItemGUID, newid()) as ItemGUID,convert(float,0) as TxnClosingQty,pm.receivedon as TxnDate,pm.ID as TxnID,'pull value' as TxnModuleItemName,'' as TxnNumber,isnull(pm.PoolQuantity, 0) as TxnQty,pm.ActionType as TxnType,isnull(pm.PULLCost, 0) as TxnValue from PullMaster as pm Where pm.itemGUID = '" + ItemGuid.ToString() + "' and pm.ActionType in ('pull','credit') and isnull(pm.isdeleted, 0)= 0 and convert(date,pm.receivedon) >= '" + (FromDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(date,pm.receivedon) <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                                        var paramsInner = new SqlParameter[] {
                                                new SqlParameter("@ItemGuid", ItemGuid),
                                                new SqlParameter("@FromDate", (FromDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@ToDate", (Todate).ToString("yyyy-MM-dd HH:mm:ss"))
                                            };

                                        objItemTransationInfo.AUMeasureMethod = ResMeasurementPullTransferValue;
                                        var pulls = context.Database.SqlQuery<ItemTransactionDTO>("exec GetItemPullTransactionHistoryPlain @ItemGuid,@FromDate,@ToDate ", paramsInner).ToList();
                                        var transferList = GetItemTransferTransactionHistoryPlain(ItemGuid, FromDate, Todate);
                                        List<ItemTransactionDTO> txnHistory = new List<ItemTransactionDTO>();
                                        
                                        if (pulls != null && pulls.Any() && pulls.Count() > 0)
                                        {
                                            txnHistory.AddRange(pulls);
                                        }

                                        if (transferList != null && transferList.Any() && transferList.Count() > 0)
                                        {
                                            txnHistory.AddRange(transferList);
                                        }

                                        objItemTransationInfo.TxtHistory = txnHistory;
                                        break;
                                    case 2:
                                        //qry = "Select pm.GUID,isnull(pm.ItemGUID, newid()) as ItemGUID,convert(float,0) as TxnClosingQty,pm.receivedon as TxnDate,pm.ID as TxnID,'pull value' as TxnModuleItemName,'' as TxnNumber,isnull(pm.PoolQuantity, 0) as TxnQty,pm.ActionType as TxnType,isnull(pm.PULLCost, 0) as TxnValue from PullMaster as pm Where pm.itemGUID = '" + ItemGuid.ToString() + "' and pm.ActionType in ('pull','credit') and isnull(pm.isdeleted, 0)= 0 and convert(date,pm.receivedon) >= '" + (FromDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(date,pm.receivedon) <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                                        var paramsInnerCase2 = new SqlParameter[] {
                                                new SqlParameter("@ItemGuid", ItemGuid),
                                                new SqlParameter("@FromDate", (FromDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@ToDate", (Todate).ToString("yyyy-MM-dd HH:mm:ss"))
                                            };
                                        objItemTransationInfo.AUMeasureMethod = ResMeasurementPullTransfer;
                                        var pull = context.Database.SqlQuery<ItemTransactionDTO>("exec GetItemPullTransactionHistoryPlain @ItemGuid,@FromDate,@ToDate ", paramsInnerCase2).ToList();
                                        var transfer = GetItemTransferTransactionHistoryPlain(ItemGuid, FromDate, Todate);
                                        List<ItemTransactionDTO> txnHistoryList = new List<ItemTransactionDTO>();
                                        
                                        if (pull != null && pull.Any() && pull.Count() > 0)
                                        {
                                            txnHistoryList.AddRange(pull);
                                        }

                                        if (transfer != null && transfer.Any() && transfer.Count() > 0)
                                        {
                                            txnHistoryList.AddRange(transfer);
                                        }

                                        objItemTransationInfo.TxtHistory = txnHistoryList;
                                        break;
                                    case 3:
                                        var paramInnerCase3 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomId),
                                                new SqlParameter("@CompanyID", CompanyId),
                                                new SqlParameter("@ItemGuid", ItemGuid),
                                                new SqlParameter("@FromDate", (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@ToDate", (Todate).ToString("yyyy-MM-dd HH:mm:ss"))
                                            };

                                        //qry = "SELECT OD.GUID,isnull(OD.ItemGUID,newid()) as ItemGUID, convert(float,0) as TxnClosingQty,Od.ReceivedOn as TxnDate,od.ID as TxnID,OM.OrderNumber as TxnModuleItemName,'' as TxnNumber, case when OM.OrderType=1 then isnull(od.ApprovedQuantity,0) when OM.OrderType=1 then (isnull(od.ApprovedQuantity,0)*(-1)) ELSE convert(float,0) END as TxnQty,case when OM.OrderType=1 then 'order' when OM.OrderType=2 then 'orderreturn' ELSE '' END as TxnType,convert(float,0) as TxnValue  FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE OD.ItemGUID = '" + ItemGuid + "' AND Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId + " AND OD.ReceivedOn >= '" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' AND OD.ReceivedOn <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";
                                        objItemTransationInfo.AUMeasureMethod = ResMeasurementOrder;
                                        objItemTransationInfo.TxtHistory = context.Database.SqlQuery<ItemTransactionDTO>("exec GetItemOrderTransactionHistoryNormal @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramInnerCase3).ToList();
                                        break;
                                }
                            }
                            break;
                        case "turns":
                            if ((RoomAnalyticSettings.TurnsMeasureMethod ?? 0) > 0 && (RoomAnalyticSettings.TurnsDaysOfUsageToSample ?? 0) > 0)
                            {
                                DateTime today = DateTime.Today;
                                DateTime month = new DateTime(today.Year, today.Month, 1);
                                //DateTime? FromDate = month.AddMonths(-(RoomAnalyticSettings.TurnsMonthsOfUsageToSample.Value));
                                //DateTime Todate = month.AddDays(-1);
                                DateTime? FromDate = DateTime.Now.AddDays(RoomAnalyticSettings.TurnsDaysOfUsageToSample.Value * (-1)).Date;
                                if (FromDate.HasValue)
                                {
                                    FromDate = new DateTime(FromDate.Value.Year, FromDate.Value.Month, 1, FromDate.Value.Hour, FromDate.Value.Minute, FromDate.Value.Second);
                                }
                                DateTime Todate = new DateTime(DateTime.Now.AddMonths(-1).Date.Year, DateTime.Now.AddMonths(-1).Date.Month, DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Date.Year, DateTime.Now.AddMonths(-1).Date.Month), 23, 59, 0);
                                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                                objItemMasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, ItemGuid);
                                objItemTransationInfo.TxnStartDate = FromDate.Value.Date;
                                objItemTransationInfo.TxnEndDate = Todate.Date;
                                objItemTransationInfo.ItemId = objItemMasterDTO.ID;
                                objItemTransationInfo.ItemGUID = ItemGuid;
                                objItemTransationInfo.ItemNumber = objItemMasterDTO.ItemNumber;
                                //switch (RoomAnalyticSettings.AUMeasureMethod)
                                switch (RoomAnalyticSettings.TurnsMeasureMethod)
                                {
                                    case 1:
                                        var paramsInner = new SqlParameter[] {
                                                new SqlParameter("@ItemGuid", ItemGuid),
                                                new SqlParameter("@FromDate", (FromDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@ToDate", (Todate).ToString("yyyy-MM-dd HH:mm:ss"))
                                            };
                                        //qry = "Select pm.GUID,isnull(pm.ItemGUID, newid()) as ItemGUID,convert(float,0) as TxnClosingQty,pm.receivedon as TxnDate,pm.ID as TxnID,'pull value' as TxnModuleItemName,'' as TxnNumber,isnull(pm.PoolQuantity, 0) as TxnQty,pm.ActionType as TxnType,isnull(pm.PULLCost, 0) as TxnValue from PullMaster as pm Where pm.itemGUID = '" + ItemGuid.ToString() + "' and pm.ActionType in ('pull','credit') and isnull(pm.isdeleted, 0)= 0 and convert(date,pm.receivedon) >= '" + (FromDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(date,pm.receivedon) <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                                        objItemTransationInfo.AUMeasureMethod = ResMeasurementPullTransferValue;
                                        var pulls = context.Database.SqlQuery<ItemTransactionDTO>("exec GetItemPullTransactionHistoryPlain @ItemGuid,@FromDate,@ToDate ", paramsInner).ToList();
                                        var transferList = GetItemTransferTransactionHistoryPlain(ItemGuid, FromDate, Todate);
                                        List<ItemTransactionDTO> txnHistory = new List<ItemTransactionDTO>();
                                        if (pulls != null && pulls.Any() && pulls.Count() > 0)
                                        {
                                            txnHistory.AddRange(pulls);
                                        }

                                        if (transferList != null && transferList.Any() && transferList.Count() > 0)
                                        {
                                            txnHistory.AddRange(transferList);
                                        }
                                        
                                        objItemTransationInfo.TxtHistory = txnHistory;
                                        break;
                                    case 2:
                                        var paramsInnerCase2 = new SqlParameter[] {
                                                new SqlParameter("@ItemGuid", ItemGuid),
                                                new SqlParameter("@FromDate", (FromDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@ToDate", (Todate).ToString("yyyy-MM-dd HH:mm:ss"))
                                            };
                                        //qry = "Select pm.GUID,isnull(pm.ItemGUID, newid()) as ItemGUID,convert(float,0) as TxnClosingQty,pm.receivedon as TxnDate,pm.ID as TxnID,'pull value' as TxnModuleItemName,'' as TxnNumber,isnull(pm.PoolQuantity, 0) as TxnQty,pm.ActionType as TxnType,isnull(pm.PULLCost, 0) as TxnValue from PullMaster as pm Where pm.itemGUID = '" + ItemGuid.ToString() + "' and pm.ActionType in ('pull','credit') and isnull(pm.isdeleted, 0)= 0 and convert(date,pm.receivedon) >= '" + (FromDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(date,pm.receivedon) <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                                        objItemTransationInfo.AUMeasureMethod = ResMeasurementPullTransfer;
                                        var pull = context.Database.SqlQuery<ItemTransactionDTO>("exec GetItemPullTransactionHistoryPlain @ItemGuid,@FromDate,@ToDate ", paramsInnerCase2).ToList();
                                        var transfer = GetItemTransferTransactionHistoryPlain(ItemGuid, FromDate, Todate);
                                        List<ItemTransactionDTO> txnHistoryList = new List<ItemTransactionDTO>();
                                        if (pull != null && pull.Any() && pull.Count() > 0)
                                        {
                                            txnHistoryList.AddRange(pull);
                                        }

                                        if (transfer != null && transfer.Any() && transfer.Count() > 0)
                                        {
                                            txnHistoryList.AddRange(transfer);
                                        }

                                        objItemTransationInfo.TxtHistory = txnHistoryList;                                        
                                        break;
                                    case 3:
                                        var paramInnerCase3 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomId),
                                                new SqlParameter("@CompanyID", CompanyId),
                                                new SqlParameter("@ItemGuid", ItemGuid),
                                                new SqlParameter("@FromDate", (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@ToDate", (Todate).ToString("yyyy-MM-dd HH:mm:ss"))
                                            };
                                        //qry = "SELECT OD.GUID,isnull(OD.ItemGUID,newid()) as ItemGUID,convert(float,0) as TxnClosingQty,Od.ReceivedOn as TxnDate,od.ID as TxnID,OM.OrderNumber as TxnModuleItemName,'' as TxnNumber, case when OM.OrderType=1 then isnull(od.ApprovedQuantity,0) when OM.OrderType=1 then (isnull(od.ApprovedQuantity,0)*(-1)) ELSE convert(float,0) END as TxnQty,case when OM.OrderType=1 then 'order' when OM.OrderType=2 then 'orderreturn' ELSE '' END as TxnType,convert(float,0) as TxnValue  FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE OD.ItemGUID = '" + ItemGuid + "' AND Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId + " AND OD.ReceivedOn >= '" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' AND OD.ReceivedOn <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";
                                        objItemTransationInfo.AUMeasureMethod = ResMeasurementOrder;
                                        objItemTransationInfo.TxtHistory = context.Database.SqlQuery<ItemTransactionDTO>("exec GetItemOrderTransactionHistoryNormal @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramInnerCase3).ToList();
                                        break;
                                }
                            }
                            break;
                    }
                }
                if (objItemTransationInfo != null && objItemTransationInfo.TxtHistory != null && objItemTransationInfo.TxtHistory.Count() > 0)
                {
                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 1);

                    if (objeTurnsRegionInfo == null)
                    {
                        objeTurnsRegionInfo = new eTurnsRegionInfo();
                        objeTurnsRegionInfo.CompanyId = CompanyId;
                        objeTurnsRegionInfo.CultureCode = "en-US";
                        objeTurnsRegionInfo.CultureDisplayName = "en-US";
                        objeTurnsRegionInfo.CultureName = "en-US";
                        objeTurnsRegionInfo.CurrencyDecimalDigits = 2;
                        objeTurnsRegionInfo.CurrencyGroupSeparator = null;
                        objeTurnsRegionInfo.RoomId = RoomId;
                        objeTurnsRegionInfo.ShortDatePattern = "M/d/yyyy";
                        objeTurnsRegionInfo.ShortTimePattern = "h:mm:ss tt";
                        objeTurnsRegionInfo.TimeZoneName = TimeZoneInfo.Utc.StandardName;
                        objeTurnsRegionInfo.TimeZoneOffSet = 0;
                    }
                    objItemTransationInfo.TxtHistory.ToList().ForEach(z => z.TxnstringDate = DateTimeUtility.ConvertDateByTimeZone(z.TxnDate, objeTurnsRegionInfo.TimeZoneName, objeTurnsRegionInfo.ShortDatePattern, objeTurnsRegionInfo.CultureCode, false));
                    if (objItemTransationInfo.AUMeasureMethod == ResMeasurementPullTransfer || objItemTransationInfo.AUMeasureMethod == ResMeasurementPullTransferValue)
                    {
                        if (objItemTransationInfo.TxtHistory.Where(t => t.TxnType.ToLower() == "pull").Any())
                        {
                            objItemTransationInfo.TotalQty = objItemTransationInfo.TxtHistory.Where(t => t.TxnType.ToLower() == "pull").Sum(t => t.TxnQty);
                            objItemTransationInfo.TotalTxnValue = objItemTransationInfo.TxtHistory.Where(t => t.TxnType.ToLower() == "pull").Sum(t => t.TxnValue);
                        }
                        if (objItemTransationInfo.TxtHistory.Where(t => t.TxnType.ToLower() == "credit").Any())
                        {
                            objItemTransationInfo.TotalQty = objItemTransationInfo.TotalQty - (objItemTransationInfo.TxtHistory.Where(t => t.TxnType.ToLower() == "credit").Sum(t => t.TxnQty));
                            objItemTransationInfo.TotalTxnValue = objItemTransationInfo.TotalTxnValue - (objItemTransationInfo.TxtHistory.Where(t => t.TxnType.ToLower() == "credit").Sum(t => t.TxnValue));
                        }

                        var transferOutTxn = objItemTransationInfo.TxtHistory.Where(t => t.TxnModuleItemName.ToLower() == "transfer" && t.TxnType.ToLower() == "1").ToList();
                        var transferInTxn = objItemTransationInfo.TxtHistory.Where(t => t.TxnModuleItemName.ToLower() == "transfer" && t.TxnType.ToLower() == "0").ToList();
                        
                        if (transferOutTxn != null && transferOutTxn.Any() && transferOutTxn.Count() > 0)
                        {
                            objItemTransationInfo.TotalQty += transferOutTxn.Sum(t => t.TxnQty);
                            objItemTransationInfo.TotalTxnValue += transferOutTxn.Sum(t => t.TxnValue);
                            transferOutTxn.ForEach(e => e.TxnType = "Transfer Out");
                        }
                        if (transferInTxn != null && transferInTxn.Any() && transferInTxn.Count() > 0)
                        {
                            objItemTransationInfo.TotalQty -= transferInTxn.Sum(t => t.TxnQty);
                            objItemTransationInfo.TotalTxnValue -= transferInTxn.Sum(t => t.TxnValue);
                            transferInTxn.ForEach(e => e.TxnType = "Transfer In");
                        }
                    }
                    else
                    {
                        objItemTransationInfo.TotalQty = objItemTransationInfo.TxtHistory.Sum(t => t.TxnQty);
                        objItemTransationInfo.TotalTxnValue = objItemTransationInfo.TxtHistory.Sum(t => t.TxnValue);
                    }

                }
                //
                return objItemTransationInfo;
            }

        }
        public void ReclassifyAllItems(long RoomID, long CompanyID, long UserId)
        {
            List<ItemMaster> ItemGuids = new List<ItemMaster>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemGuids = context.ItemMasters.Where(t => t.Room == RoomID && t.CompanyID == CompanyID && t.IsDeleted == false && t.IsArchived == false).ToList();
            }
            if (ItemGuids != null && ItemGuids.Count > 0)
            {
                foreach (var objItem in ItemGuids)
                {
                    SetItemsAutoClassification(objItem.GUID, objItem.Room ?? 0, objItem.CompanyID ?? 0, objItem.LastUpdatedBy ?? 0, 1);
                }
            }
        }
        public CostDTO GetAvgExtendedCost(Guid ItemGUID)
        {
            CostDTO objCostDTO = new CostDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int CostUOMVal = 1;
                double ExtendedCost = 0;
                double AverageCost = 0;
                ItemMaster objItemMaster = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                CostUOMMaster objCostUOM = context.CostUOMMasters.FirstOrDefault(t => t.ID == objItemMaster.CostUOMID);
                if (objCostUOM != null && (objCostUOM.CostUOMValue ?? 0) > 0)
                {
                    CostUOMVal = objCostUOM.CostUOMValue.Value;
                }
                if (objItemMaster.Consignment)
                {
                    ExtendedCost = ((objItemMaster.Cost ?? 0) / CostUOMVal) * (objItemMaster.OnHandQuantity ?? 0);
                }
                else
                {
                    var ILQ = context.ItemLocationDetails.Where(t => t.ItemGUID == ItemGUID && (t.IsDeleted ?? false) == false);
                    if (ILQ != null && ILQ.Any())
                    {
                        ExtendedCost = ILQ.Sum(t => ((t.Cost ?? 0) / CostUOMVal) * ((t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0)));
                    }
                }
                if ((objItemMaster.OnHandQuantity ?? 0) > 0)
                {
                    AverageCost = (ExtendedCost / objItemMaster.OnHandQuantity.Value) * CostUOMVal;
                }

                objCostDTO.ExtCost = ExtendedCost;
                objCostDTO.AvgCost = AverageCost;
                return objCostDTO;
            }
        }
        public DashboardAnalysisInfo GetTurnsByItemGUID(long RoomId, long CompanyId, Guid ItemGUID, long UserId, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null) //,byte? TurnsMeasureMethod,int? TurnsMonthsOfUsageToSample
        {
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            string CurrentRoomTimeZone = "UTC";
            if (objDashboardParameterDTO == null)
            {
                //if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["DashboardParams"] != null)
                //{
                //    objDashboardParameterDTO = (DashboardParameterDTO)HttpContext.Current.Session["DashboardParams"];
                //}
                //else
                {
                    objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
                }
            }
            if (objRegionalSettings == null)
            {
                //if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["eTurnsRegionInfoProp"] != null)
                //{
                //    objRegionalSettings = (eTurnsRegionInfo)HttpContext.Current.Session["eTurnsRegionInfoProp"];
                //}
                //else
                {
                    objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
                }
            }
            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }
            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.TurnsDaysOfUsageToSample ?? 0) > 0)
                {
                    ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                    if (objItem != null)
                    {
                        double PullCost = 0;
                        double ItemInventoryValue = 0;
                        double PullQuantity = 0;
                        double OrderedQty = 0;
                        double ReturnOrderedQty = 0;
                        double FinalOrderedQty = 0;
                        //double onHandQty = 0;
                        DateTime? FromDate = CurrentTimeofTimeZone.AddDays(objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value * (-1)).Date;
                        if (FromDate.HasValue)
                        {
                            FromDate = new DateTime(FromDate.Value.Year, FromDate.Value.Month, 1, 0, 0, 0);
                        }

                        DateTime Todate = new DateTime(CurrentTimeofTimeZone.AddMonths(-1).Date.Year, CurrentTimeofTimeZone.AddMonths(-1).Date.Month, DateTime.DaysInMonth(CurrentTimeofTimeZone.AddMonths(-1).Date.Year, CurrentTimeofTimeZone.AddMonths(-1).Date.Month), 23, 59, 59);
                        FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                        Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                        double minQty = objItem.MaximumQuantity;
                        double maxQty = objItem.MinimumQuantity;
                        BinMasterDAL objBinMaster = new BinMasterDAL(base.DataBaseName);
                        if (objItem.IsItemLevelMinMaxQtyRequired == false)
                        {
                            //List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID)).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber).ToList();
                            List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID), false, string.Empty, null, null).OrderBy(x => x.BinNumber).ToList();
                            //List<BinMasterDTO> Itemlocs = objBinMaster.GetItemLocation(RoomId, CompanyId, false, false,(objItem.GUID),0,null,false).OrderBy(x => x.BinNumber).ToList();//.Where(x => !x.IsStagingLocation)
                            if (Itemlocs.Count > 0)
                            {
                                minQty = Itemlocs.First().MinimumQuantity ?? 0;
                                maxQty = Itemlocs.First().MaximumQuantity ?? 0;
                            }
                        }

                        double DurationDiviser = 12 / (objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value / 30); //double DurationDiviser = 12 / objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value;
                        double Avgminmax = (maxQty + minQty) / 2;
                        List<PullMaster> PullMasterList = null;
                        List<OrderDetailsDTO> lstOrderDetails = null;

                        PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate);//context.Database.SqlQuery<PullMaster>("exec GetPullByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramsInner).ToList();
                        var transferList = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate, true);
                        double TransferCost = 0;
                        double transferedQty = 0;
                        
                        if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                        {
                            PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                            PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                        }
                        if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                        {
                            PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                            PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                        }

                        if (transferList != null && transferList.Any() && transferList.Count() > 0)
                        {
                            TransferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                            TransferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                            transferedQty = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                            transferedQty -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                        }

                        var PullTransferCost = PullCost + TransferCost;
                        var PullTransferQty = PullQuantity + transferedQty;

                        lstOrderDetails = GetOrderByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate);//context.Database.SqlQuery<OrderDetailsDTO>("exec GetOrderByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramInnerCase).ToList();
                        
                        if (lstOrderDetails.Any())
                        {
                            OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                            ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                        }
                        FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                        if (PullTransferCost > 0)
                        {
                            ItemInventoryValue = objItem.ExtendedCost ?? 0;
                            if (ItemInventoryValue > 0)
                            {
                                objDashboardAnalysisInfo.MonthizedPullValueTurns = ((PullTransferCost) / (ItemInventoryValue)) * (DurationDiviser);
                            }
                            else
                            {
                                objDashboardAnalysisInfo.MonthizedPullValueTurns = 0;
                            }
                        }
                        else
                        {
                            objDashboardAnalysisInfo.MonthizedPullValueTurns = 0;
                        }

                        if (PullTransferQty > 0 && (objItem.OnHandQuantity ?? 0) > 0)
                        {
                            objDashboardAnalysisInfo.MonthizedPullTurns = (PullTransferQty / objItem.OnHandQuantity ?? 1) * DurationDiviser;
                        }
                        else
                        {
                            objDashboardAnalysisInfo.MonthizedPullTurns = 0;
                        }
                        if (Avgminmax > 0 && FinalOrderedQty > 0)
                        {
                            objDashboardAnalysisInfo.MonthizedOrderTurns = ((FinalOrderedQty) / Avgminmax) * DurationDiviser;
                        }
                        else
                        {
                            objDashboardAnalysisInfo.MonthizedOrderTurns = 0;
                        }
                    }

                }
            }
            return objDashboardAnalysisInfo;
        }
        public DashboardAnalysisInfo GetAvgUsageByItemGUID(long RoomId, long CompanyId, Guid ItemGUID, long UserId, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null)
        {
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            string CurrentRoomTimeZone = "UTC";

            if (objDashboardParameterDTO == null)
            {
                objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
            }

            if (objRegionalSettings == null)
            {
                objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
            }

            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }
            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.AUDayOfUsageToSample ?? 0) > 0)
                {
                    ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                    if (objItem != null)
                    {
                        DateTime? FromDate = CurrentTimeofTimeZone.AddDays(objDashboardParameterDTO.AUDayOfUsageToSample.Value * (-1)).Date;
                        DateTime Todate = new DateTime(CurrentTimeofTimeZone.Year, CurrentTimeofTimeZone.Month, CurrentTimeofTimeZone.Day, 23, 59, 0);
                        FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                        Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                        double PullCost = 0;
                        double PullQuantity = 0;
                        double OrderedQty = 0;
                        double ReturnOrderedQty = 0;
                        double FinalOrderedQty = 0;

                        List<PullMaster> PullMasterList = null;
                        List<OrderDetailsDTO> lstOrderDetails = null;

                        PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate);
                        var transferList = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate, true);
                        double TransferCost = 0;
                        double transferedQty = 0;

                        if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                        {
                            PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                            PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                        }
                        if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                        {
                            PullCost = PullCost - PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum();
                            PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                        }

                        if (transferList != null && transferList.Any() && transferList.Count() > 0)
                        {
                            TransferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                            TransferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                            transferedQty = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                            transferedQty -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                        }
                        
                        var PullTransferCost = PullCost + TransferCost;
                        var PullTransferQty = PullQuantity + transferedQty;

                        lstOrderDetails = GetOrderByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate);//context.Database.SqlQuery<OrderDetailsDTO>("exec GetOrderByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramInnerCase).ToList();

                        if (lstOrderDetails.Any())
                        {
                            OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                            ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                        }

                        FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                        if (PullTransferCost > 0)
                        {
                            objDashboardAnalysisInfo.CalculatedPullValueAverageUsage = PullTransferCost / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                        }
                        else
                        {
                            objDashboardAnalysisInfo.CalculatedPullValueAverageUsage = 0;
                        }

                        if (PullTransferQty > 0)
                        {
                            objDashboardAnalysisInfo.CalculatedPullAverageUsage = PullTransferQty / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                        }
                        else
                        {
                            objDashboardAnalysisInfo.CalculatedPullAverageUsage = 0;
                        }

                        if (FinalOrderedQty > 0)
                        {
                            objDashboardAnalysisInfo.CalculatedOrderAverageUsage = FinalOrderedQty / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                        }
                        else
                        {
                            objDashboardAnalysisInfo.CalculatedOrderAverageUsage = 0;
                        }
                    }
                }


            }
            return objDashboardAnalysisInfo;
        }
        public DashboardAnalysisInfo GetTurnsByRoom(long RoomId, long CompanyId, long UserId, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null) //,byte? TurnsMeasureMethod,int? TurnsMonthsOfUsageToSample
        {
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            string CurrentRoomTimeZone = "UTC";

            if (objDashboardParameterDTO == null)
            {
                objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
            }

            if (objRegionalSettings == null)
            {
                objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
            }

            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }

            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.TurnsDaysOfUsageToSample ?? 0) > 0)
                {
                    double OnhandQty = 0;
                    double PullCost = 0;
                    double ItemInventoryValue = 0;
                    double PullQuantity = 0;
                    double OrderedQty = 0;
                    double ReturnOrderedQty = 0;
                    double FinalOrderedQty = 0;
                    //double onHandQty = 0;
                    DateTime? FromDate = CurrentTimeofTimeZone.AddDays(objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value * (-1)).Date;
                    if (FromDate.HasValue)
                    {
                        FromDate = new DateTime(FromDate.Value.Year, FromDate.Value.Month, 1, FromDate.Value.Hour, FromDate.Value.Minute, FromDate.Value.Second);
                    }

                    DateTime Todate = new DateTime(CurrentTimeofTimeZone.AddMonths(-1).Date.Year, CurrentTimeofTimeZone.AddMonths(-1).Date.Month, DateTime.DaysInMonth(CurrentTimeofTimeZone.AddMonths(-1).Date.Year, CurrentTimeofTimeZone.AddMonths(-1).Date.Month), 23, 59, 0);
                    FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                    Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);
                    double minQty = 0;
                    double maxQty = 0;
                    double DurationDiviser = 12 / (objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value / 30); //12 / objDashboardParameterDTO.TurnsMonthsOfUsageToSample.Value;
                    double Avgminmax = (maxQty + minQty) / 2;
                    List<PullMaster> PullMasterList = null;
                    List<OrderDetailsDTO> lstOrderDetails = null;
                    List<TransferDetailDTO> transferList = null;
                    double transferCost = 0;
                    double transferQuantity = 0;

                    PullMasterList = GetPullByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate); //context.Database.SqlQuery<PullMaster>("exec GetPullByRoomAndDateRange @RoomID,@CompanyID,@FromDate,@ToDate ", paramsInner).ToList();
                    transferList = GetTransferByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);

                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                    {
                        PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                        PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                    }
                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                    {
                        PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                        PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                    }

                    if (transferList != null && transferList.Any() && transferList.Count() > 0)
                    {
                        transferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                        transferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                        transferQuantity = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                        transferQuantity -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                    }

                    double pullTransferQty = PullQuantity + transferQuantity;
                    double pullTransferCost = PullCost + transferCost;
                    lstOrderDetails = GetOrderByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);
                   
                    if (lstOrderDetails.Any())
                    {
                        OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                        ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                    }
                    FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                    var qryitems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false);
                    if (qryitems.Any())
                    {
                        OnhandQty = qryitems.Sum(t => (t.OnHandQuantity ?? 0));
                        ItemInventoryValue = qryitems.Sum(t => (t.ExtendedCost ?? 0));
                        Avgminmax = qryitems.Sum(t => ((t.MinimumQuantity) + (t.MaximumQuantity) / 2));
                    }

                    if (pullTransferCost > 0)
                    {
                        if (ItemInventoryValue > 0)
                        {
                            objDashboardAnalysisInfo.MonthizedPullValueTurns = ((pullTransferCost) / (ItemInventoryValue)) * (DurationDiviser);
                        }
                        else
                        {
                            objDashboardAnalysisInfo.MonthizedPullValueTurns = 0;
                        }
                    }
                    else
                    {
                        objDashboardAnalysisInfo.MonthizedPullValueTurns = 0;
                    }

                    if (pullTransferQty > 0 && OnhandQty > 0)
                    {
                        objDashboardAnalysisInfo.MonthizedPullTurns = (pullTransferQty / OnhandQty) * DurationDiviser;
                    }
                    else
                    {
                        objDashboardAnalysisInfo.MonthizedPullTurns = 0;
                    }
                    if (Avgminmax > 0 && FinalOrderedQty > 0)
                    {
                        objDashboardAnalysisInfo.MonthizedOrderTurns = ((FinalOrderedQty) / Avgminmax) * DurationDiviser;
                    }
                    else
                    {
                        objDashboardAnalysisInfo.MonthizedOrderTurns = 0;
                    }


                }
            }
            return objDashboardAnalysisInfo;
        }
        public DashboardAnalysisInfo GetAvgUsageByRoom(long RoomId, long CompanyId, long UserId, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null)
        {
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            string CurrentRoomTimeZone = "UTC";
            if (objDashboardParameterDTO == null)
            {
                //if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["DashboardParams"] != null)
                //{
                //    objDashboardParameterDTO = (DashboardParameterDTO)HttpContext.Current.Session["DashboardParams"];
                //}
                //else
                {
                    objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
                }
            }
            if (objRegionalSettings == null)
            {
                //if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["eTurnsRegionInfoProp"] != null)
                //{
                //    objRegionalSettings = (eTurnsRegionInfo)HttpContext.Current.Session["eTurnsRegionInfoProp"];
                //}
                //else
                {
                    objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
                }
            }
            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }
            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.AUDayOfUsageToSample ?? 0) > 0)
                {

                    DateTime? FromDate = CurrentTimeofTimeZone.AddDays(objDashboardParameterDTO.AUDayOfUsageToSample.Value * (-1)).Date;
                    DateTime Todate = new DateTime(CurrentTimeofTimeZone.Year, CurrentTimeofTimeZone.Month, CurrentTimeofTimeZone.Day, 23, 59, 0);
                    FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                    Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                    double PullCost = 0;
                    double PullQuantity = 0;
                    double OrderedQty = 0;
                    double ReturnOrderedQty = 0;
                    double FinalOrderedQty = 0;

                    List<PullMaster> PullMasterList = null;
                    List<OrderDetailsDTO> lstOrderDetails = null;
                    List<TransferDetailDTO> transferList = null;
                    double transferCost = 0;
                    double transferQuantity = 0;
                    PullMasterList = GetPullByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);
                    transferList = GetTransferByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);

                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                    {
                        PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                        PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                    }
                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                    {
                        PullCost = PullCost - PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum();
                        PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                    }

                    if (transferList != null && transferList.Any() && transferList.Count() > 0)
                    {
                        transferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                        transferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                        transferQuantity = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                        transferQuantity -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                    }

                    double pullTransferQty = PullQuantity + transferQuantity;
                    double pullTransferCost = PullCost + transferCost;

                    lstOrderDetails = GetOrderByRoomAndDateRange(RoomId, CompanyId, FromDate, Todate);

                    if (lstOrderDetails.Any())
                    {
                        OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                        ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                    }

                    FinalOrderedQty = OrderedQty - ReturnOrderedQty;
                    
                    if (pullTransferCost > 0)
                    {
                        objDashboardAnalysisInfo.CalculatedPullValueAverageUsage = pullTransferCost / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                    }
                    else
                    {
                        objDashboardAnalysisInfo.CalculatedPullValueAverageUsage = 0;
                    }

                    if (pullTransferQty > 0)
                    {
                        objDashboardAnalysisInfo.CalculatedPullAverageUsage = pullTransferQty / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                    }
                    else
                    {
                        objDashboardAnalysisInfo.CalculatedPullAverageUsage = 0;
                    }

                    if (FinalOrderedQty > 0)
                    {
                        objDashboardAnalysisInfo.CalculatedOrderAverageUsage = FinalOrderedQty / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                    }
                    else
                    {
                        objDashboardAnalysisInfo.CalculatedOrderAverageUsage = 0;
                    }
                }
            }
            return objDashboardAnalysisInfo;
        }
        public DashboardAnalysisInfo UpdateTurnsByItemGUIDAfterTxn(long RoomId, long CompanyId, Guid ItemGUID, long UserId, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null)
        {
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            string CurrentRoomTimeZone = "UTC";

            if (objDashboardParameterDTO == null)
            {
                objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
            }

            if (objRegionalSettings == null)
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
            }

            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }
            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.TurnsDaysOfUsageToSample ?? 0) > 0)
                {
                    ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                    if (objItem != null)
                    {
                        double PullCost = 0;
                        double ItemInventoryValue = 0;
                        double PullQuantity = 0;
                        double OrderedQty = 0;
                        double ReturnOrderedQty = 0;
                        double FinalOrderedQty = 0;
                        //double onHandQty = 0;
                        DateTime? FromDate = CurrentTimeofTimeZone.AddDays(1).AddDays((objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value) * (-1)).Date;
                        DateTime Todate = new DateTime(CurrentTimeofTimeZone.Year, CurrentTimeofTimeZone.Month, CurrentTimeofTimeZone.Day, 23, 59, 59);
                        FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                        Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                        double minQty = objItem.MaximumQuantity;
                        double maxQty = objItem.MinimumQuantity;
                        if (objItem.IsItemLevelMinMaxQtyRequired == false)
                        {
                            BinMasterDAL objBinMaster = new BinMasterDAL(base.DataBaseName);
                            //List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID)).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber).ToList();
                            List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID), false, string.Empty, null, null).OrderBy(x => x.BinNumber).ToList();
                            //List<BinMasterDTO> Itemlocs = objBinMaster.GetItemLocation(RoomId, CompanyId, false, false,(objItem.GUID),0,null,false).OrderBy(x => x.BinNumber).ToList();//.Where(x => !x.IsStagingLocation)
                            if (Itemlocs.Count > 0)
                            {
                                minQty = Itemlocs.First().MinimumQuantity ?? 0;
                                maxQty = Itemlocs.First().MaximumQuantity ?? 0;
                            }
                        }

                        double DurationDiviser = 365 / (objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value);
                        double Avgminmax = (maxQty + minQty) / 2;
                        List<PullMaster> PullMasterList = null;
                        List<OrderDetailsDTO> lstOrderDetails = null;
                        double AverageExtendedCost = 0, AverageOnHandQuantity = 0;
                        ItemAvgCostQtyInfo objItemAvgCostQtyInfo = objDashboardDAL.GetItemOrRoomAvgCostQty(objItem.GUID, objItem.Room ?? 0, objItem.CompanyID ?? 0, (FromDate ?? DateTime.Now.AddDays(1)), Todate);
                        if (objItemAvgCostQtyInfo != null)
                        {
                            AverageExtendedCost = objItemAvgCostQtyInfo.AverageExtendedCost;
                            AverageOnHandQuantity = objItemAvgCostQtyInfo.AverageOnHandQuantity;
                        }

                        switch (objDashboardParameterDTO.TurnsMeasureMethod)
                        {
                            case 1:
                                PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate);                                
                                var transferList = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate, true);
                                double TransferCost = 0;                                

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                                }

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                                }

                                if (transferList != null && transferList.Any() && transferList.Count() > 0)
                                {
                                    TransferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                                    TransferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                                }
                                
                                var PullTransferCost = PullCost + TransferCost;

                                if (PullTransferCost > 0)
                                {
                                    ItemInventoryValue = AverageExtendedCost;
                                    if (ItemInventoryValue > 0)
                                    {
                                        objDashboardAnalysisInfo.CalculatedTurn = ((PullTransferCost) / (ItemInventoryValue)) * (DurationDiviser);
                                        objItem.Turns = objDashboardAnalysisInfo.CalculatedTurn;
                                    }
                                    else
                                    {
                                        objDashboardAnalysisInfo.CalculatedTurn = 0;
                                        objItem.Turns = 0;
                                    }
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = 0;
                                    objItem.Turns = 0;
                                }
                                break;
                            case 2:
                                PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate);
                                var transfers = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate, false);
                                double transferedQty = 0;                      


                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullQuantity -= (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                                }

                                if (transfers != null && transfers.Any() && transfers.Count() > 0)
                                {
                                    transferedQty = transfers.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                                    transferedQty -= transfers.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                                }

                                var PullTransferQty = PullQuantity + transferedQty;

                                if (PullTransferQty > 0 && AverageOnHandQuantity > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = (PullTransferQty / AverageOnHandQuantity) * DurationDiviser;
                                    objItem.Turns = objDashboardAnalysisInfo.CalculatedTurn;
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = 0;
                                    objItem.Turns = 0;
                                }
                                break;
                            case 3:
                                lstOrderDetails = GetOrderByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate);

                                if (lstOrderDetails.Any())
                                {
                                    OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                                    ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                                }

                                FinalOrderedQty = OrderedQty - ReturnOrderedQty;
                                //F/((Min+Max)/2) * 365/Period
                                if (Avgminmax > 0 && FinalOrderedQty > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = ((FinalOrderedQty) / Avgminmax) * DurationDiviser;
                                    objItem.Turns = objDashboardAnalysisInfo.CalculatedTurn;
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = 0;
                                    objItem.Turns = 0;
                                }
                                break;

                        }

                        ////TODO: Added By CP on 07-Sep-2017: Whatwhere action and system Userid used in Item Update trigger for Audit Trail log
                        //UserMasterDTO systemUser = CommonDAL.GeteTurnsSystemUser(base.DataBaseName);
                        //if (systemUser != null && systemUser.ID > 0)
                        //{
                        //    objItem.LastUpdatedBy = systemUser.ID;
                        //}
                        //else
                        //    objItem.LastUpdatedBy = UserId;


                        if (UserId > 0)
                        {
                            objItem.LastUpdatedBy = UserId;
                        }
                        else
                        {
                            //TODO: Added By CP on 07-Sep-2017: Whatwhere action and system Userid used in Item Update trigger for Audit Trail log
                            UserMasterDTO systemUser = CommonDAL.GeteTurnsSystemUser(base.DataBaseName);
                            if (systemUser != null && systemUser.ID > 0)
                            {
                                objItem.LastUpdatedBy = systemUser.ID;
                            }
                            else
                                objItem.LastUpdatedBy = UserId;
                        }

                        //objItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objItem.Updated = DateTimeUtility.DateTimeNow;

                        objItem.WhatWhereAction = "DashboardDAL>UpdateTurns";

                        context.SaveChanges();
                        if (objItem.IsAutoInventoryClassification)
                        {
                            SetItemsAutoClassification(objItem.GUID, objItem.Room ?? 0, objItem.CompanyID ?? 0, objItem.LastUpdatedBy ?? 0, 1);
                        }
                    }

                }
            }
            return objDashboardAnalysisInfo;
        }
        public DashboardAnalysisInfo UpdateTurnsByItemGUIDDaily(long RoomId, long CompanyId, Guid ItemGUID, long UserId, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null)
        {
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            string CurrentRoomTimeZone = "UTC";

            if (objDashboardParameterDTO == null)
            {
                objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
            }

            if (objRegionalSettings == null)
            {
                objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
            }

            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }

            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.TurnsDaysOfUsageToSample ?? 0) > 0)
                {
                    ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                    if (objItem != null)
                    {
                        double PullCost = 0;
                        double ItemInventoryValue = 0;
                        double PullQuantity = 0;
                        double OrderedQty = 0;
                        double ReturnOrderedQty = 0;
                        double FinalOrderedQty = 0;
                        //double onHandQty = 0;
                        DateTime prevDay = CurrentTimeofTimeZone.AddDays(-1);
                        DateTime? FromDate = prevDay.AddDays(1).AddDays((objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value) * (-1)).Date;
                        DateTime Todate = new DateTime(prevDay.Year, prevDay.Month, prevDay.Day, 23, 59, 59);
                        FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                        Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                        double minQty = objItem.MaximumQuantity;
                        double maxQty = objItem.MinimumQuantity;
                        BinMasterDAL objBinMaster = new BinMasterDAL(base.DataBaseName);
                        if (objItem.IsItemLevelMinMaxQtyRequired == false)
                        {
                            //List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID)).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber).ToList();
                            List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID), false, string.Empty, null, null).OrderBy(x => x.BinNumber).ToList();
                            //List<BinMasterDTO> Itemlocs = objBinMaster.GetItemLocation(RoomId, CompanyId, false, false, (objItem.GUID),0,null,false).OrderBy(x => x.BinNumber).ToList();//.Where(x => !x.IsStagingLocation)
                            if (Itemlocs.Count > 0)
                            {
                                minQty = Itemlocs.First().MinimumQuantity ?? 0;
                                maxQty = Itemlocs.First().MaximumQuantity ?? 0;
                            }
                        }

                        double DurationDiviser = 365 / (objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value);
                        double Avgminmax = (maxQty + minQty) / 2;
                        List<PullMaster> PullMasterList = null;
                        List<OrderDetailsDTO> lstOrderDetails = null;
                        double AverageExtendedCost = 0, AverageOnHandQuantity = 0;
                        ItemAvgCostQtyInfo objItemAvgCostQtyInfo = objDashboardDAL.GetItemOrRoomAvgCostQty(objItem.GUID, objItem.Room ?? 0, objItem.CompanyID ?? 0, (FromDate ?? DateTime.Now.AddDays(1)), Todate);
                        if (objItemAvgCostQtyInfo != null)
                        {
                            AverageExtendedCost = objItemAvgCostQtyInfo.AverageExtendedCost;
                            AverageOnHandQuantity = objItemAvgCostQtyInfo.AverageOnHandQuantity;
                        }
                        switch (objDashboardParameterDTO.TurnsMeasureMethod)
                        {
                            case 1:
                                PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate);
                                var transferList = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate, true);
                                double TransferCost = 0;                               

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                                }

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullCost -= (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                                }

                                if (transferList != null && transferList.Any() && transferList.Count() > 0)
                                {
                                    TransferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                                    TransferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                                }

                                var PullTransferCost = PullCost + TransferCost;

                                if (PullTransferCost > 0)
                                {
                                    ItemInventoryValue = AverageExtendedCost;
                                    if (ItemInventoryValue > 0)
                                    {
                                        objDashboardAnalysisInfo.CalculatedTurn = ((PullTransferCost) / (ItemInventoryValue)) * (DurationDiviser);
                                        objItem.Turns = objDashboardAnalysisInfo.CalculatedTurn;
                                    }
                                    else
                                    {
                                        objDashboardAnalysisInfo.CalculatedTurn = 0;
                                        objItem.Turns = 0;
                                    }
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = 0;
                                    objItem.Turns = 0;
                                }
                                break;
                            case 2:
                                PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate);
                                var transfers = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate, false);
                                double transferedQty = 0;                                

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                                }

                                if (transfers != null && transfers.Any() && transfers.Count() > 0)
                                {
                                    transferedQty = transfers.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                                    transferedQty -= transfers.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                                }

                                var PullTransferQty = PullQuantity + transferedQty;

                                if (PullTransferQty > 0 && AverageOnHandQuantity > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = (PullTransferQty / AverageOnHandQuantity) * DurationDiviser;
                                    objItem.Turns = objDashboardAnalysisInfo.CalculatedTurn;
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = 0;
                                    objItem.Turns = 0;
                                }
                                break;
                            case 3:
                                lstOrderDetails = GetOrderByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate);

                                if (lstOrderDetails.Any())
                                {
                                    OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                                    ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                                }
                                FinalOrderedQty = OrderedQty - ReturnOrderedQty;
                                if (Avgminmax > 0 && FinalOrderedQty > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = ((FinalOrderedQty) / Avgminmax) * DurationDiviser;
                                    objItem.Turns = objDashboardAnalysisInfo.CalculatedTurn;
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = 0;
                                    objItem.Turns = 0;
                                }
                                break;

                        }
                        context.SaveChanges();
                        if (objItem.IsAutoInventoryClassification)
                        {
                            SetItemsAutoClassification(objItem.GUID, objItem.Room ?? 0, objItem.CompanyID ?? 0, objItem.LastUpdatedBy ?? 0, 1);
                        }
                    }

                }
            }
            return objDashboardAnalysisInfo;
        }

        public DashboardAnalysisInfo UpdateAvgUsageByItemGUIDDaily(long RoomId, long CompanyId, Guid ItemGUID, long UserId, long SessionUserId, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null)
        {
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            string CurrentRoomTimeZone = "UTC";

            if (objDashboardParameterDTO == null)
            {
                objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
            }

            if (objRegionalSettings == null)
            {
                objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
            }

            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }
            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.AUDayOfUsageToSample ?? 0) > 0)
                {
                    ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);

                    if (objItem != null)
                    {
                        DateTime prevDay = CurrentTimeofTimeZone.AddDays(-1);
                        DateTime? FromDate = prevDay.AddDays(1).AddDays(objDashboardParameterDTO.AUDayOfUsageToSample.Value * (-1)).Date;
                        DateTime Todate = new DateTime(prevDay.Year, prevDay.Month, prevDay.Day, 23, 59, 59);
                        FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                        Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);
                        double PullCost = 0;
                        double PullQuantity = 0;
                        double OrderedQty = 0;
                        double ReturnOrderedQty = 0;
                        double FinalOrderedQty = 0;
                        List<PullMaster> PullMasterList = null;
                        List<OrderDetailsDTO> lstOrderDetails = null;

                        switch (objDashboardParameterDTO.AUMeasureMethod)
                        {
                            case 1:
                                PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate);
                                var transferList = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate, true);
                                double TransferCost = 0;                                

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                                }

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullCost = PullCost - PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum();
                                }

                                if (transferList != null && transferList.Any() && transferList.Count() > 0)
                                {
                                    TransferCost = transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.TransferCost ?? 0)).Sum();
                                    TransferCost -= transferList.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.TransferCost ?? 0)).Sum();
                                }

                                var PullTransferCost = PullCost + TransferCost;

                                if (PullTransferCost > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = PullTransferCost / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                                    objItem.AverageUsage = objDashboardAnalysisInfo.CalculatedAverageUsage;
                                    objDashboardAnalysisInfo.CalculatedMinimun = objDashboardParameterDTO.MinMaxDayOfAverage * objDashboardAnalysisInfo.CalculatedAverageUsage;
                                    objDashboardAnalysisInfo.CalculatedMaximun = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax * objDashboardAnalysisInfo.CalculatedMinimun;
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = 0;
                                    objItem.AverageUsage = 0;
                                }
                                break;
                            case 2:
                                PullMasterList = GetPullByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate);
                                var transfers = GetTransferByItemGuidAndDateRange(RoomId, CompanyId, ItemGUID, FromDate, Todate, false);
                                double transferedQty = 0;
                                
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullQuantity -= (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                                }

                                if (transfers != null && transfers.Any() && transfers.Count() > 0)
                                {
                                    transferedQty = transfers.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.Out).Select(t => (t.FulFillQuantity ?? 0)).Sum();
                                    transferedQty -= transfers.Where(t => t.TransferRequestType.GetValueOrDefault(0) == (int)RequestType.In).Select(t => (t.ReceivedQuantity ?? 0)).Sum();
                                }

                                var PullTransferQty = PullQuantity + transferedQty;
                                
                                if (PullTransferQty > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = PullTransferQty / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                                    objItem.AverageUsage = objDashboardAnalysisInfo.CalculatedAverageUsage;
                                    objDashboardAnalysisInfo.CalculatedMinimun = objDashboardParameterDTO.MinMaxDayOfAverage * objDashboardAnalysisInfo.CalculatedAverageUsage;
                                    objDashboardAnalysisInfo.CalculatedMaximun = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax * objDashboardAnalysisInfo.CalculatedMinimun;
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = 0;
                                    objItem.AverageUsage = 0;
                                }
                                break;
                            case 3:
                                lstOrderDetails = GetOrderByItemGuidAndDateRange(RoomId, CompanyId, objItem.GUID, FromDate, Todate);

                                if (lstOrderDetails.Any())
                                {
                                    OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                                    ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                                }

                                FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                                if (FinalOrderedQty > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = FinalOrderedQty / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                                    objItem.AverageUsage = objDashboardAnalysisInfo.CalculatedAverageUsage;
                                    objDashboardAnalysisInfo.CalculatedMinimun = objDashboardParameterDTO.MinMaxDayOfAverage * objDashboardAnalysisInfo.CalculatedAverageUsage;
                                    objDashboardAnalysisInfo.CalculatedMaximun = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax * objDashboardAnalysisInfo.CalculatedMinimun;
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = 0;
                                    objItem.AverageUsage = 0;
                                }
                                break;

                        }
                        if ((objItem.TrendingSetting ?? 0) == 2)
                        {
                            objItem.MinimumQuantity = objDashboardAnalysisInfo.CalculatedMinimun ?? 0;
                            objItem.MaximumQuantity = objDashboardAnalysisInfo.CalculatedMaximun ?? 0;

                            if (objItem.MinimumQuantity > objItem.MaximumQuantity)
                            {
                                objItem.MaximumQuantity = objItem.MinimumQuantity;
                            }
                        }
                        context.SaveChanges();

                        if ((objItem.TrendingSetting ?? 0) == 2)
                        {
                            new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objItem.GUID, UserId, "Web", "Dashbord >> Save Average Usage", SessionUserId);
                        }
                    }
                }
            }
            return objDashboardAnalysisInfo;
        }
        public void SaveDailyItemInventory(string RoomIDs, string CompanyIDs, long EnterpriseID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomIDs", RoomIDs ?? (object)DBNull.Value), new SqlParameter("@CompanyIDs", CompanyIDs ?? (object)DBNull.Value), new SqlParameter("@EnterpriseID", EnterpriseID) };
            using (var Context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Context.Database.ExecuteSqlCommand("EXEC SaveDailyItemData @RoomIDs,@CompanyIDs,@EnterpriseID", params1);
            }
        }

        public List<ItemMasterDTO> GetCategoryWiseCost(long CompanyID, long RoomId, byte PieChartmetricOn, int HowManyItemOnChart)
        {
            if (HowManyItemOnChart < 5)
            {
                HowManyItemOnChart = 10;
            }
            if (HowManyItemOnChart > 10)
            {
                HowManyItemOnChart = 10;
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@CompanyId", CompanyID),
                                                    new SqlParameter("@RoomId", RoomId),
                                                    new SqlParameter("@PieChartmetricOn", PieChartmetricOn),
                                                    new SqlParameter("@NoOfRowsInChart", HowManyItemOnChart)
                                                };
                return context.Database.SqlQuery<ItemMasterDTO>("exec [GetCategoryWiseCost] @CompanyId,@RoomId,@PieChartmetricOn,@NoOfRowsInChart", params1).ToList();
            }

            //List<ItemMasterDTO> lst = new List<ItemMasterDTO>();
            //string strQuery = string.Empty;

            //if (PieChartmetricOn == 1)
            //{
            //    strQuery = @";WITH ItemWithCost AS ( 
            //                select Row_number() Over(ORDER BY IM.ExtendedCost desc) as RownumberCost,IM.ItemNumber,ISNULL(IM.ExtendedCost,0) AS ExtendedCost
            //                from ItemMaster IM  WHERE IM.CompanyID=" + CompanyID.ToString() + " and IM.Room=" + RoomId.ToString() + " and ISNULL(IM.ExtendedCost,0)>0) Select RownumberCost,ItemNumber as CategoryName,ExtendedCost as Cost,'' as CategoryColor from ItemWithCost Where RownumberCost < " + (HowManyItemOnChart + 1).ToString() + " union Select " + (HowManyItemOnChart + 1).ToString() + " as RownumberCost,'[!restofothers!]' as CategoryName,sum(ExtendedCost) as Cost,'' as CategoryColor   from ItemWithCost Where RownumberCost > " + HowManyItemOnChart.ToString();
            //}
            //else if (PieChartmetricOn == 2)
            //{
            //    strQuery = @";with CatWithCost as (
            //                Select ROW_NUMBER() over(Order by qry.Cost desc) as RownumberCost,CategoryName,Cost,CategoryColor from (
            //                select SUM(ISNULL(IM.ExtendedCost,0)) AS Cost,CM.ID AS CategoryID,CM.Category AS CategoryName,CM.CategoryColor AS CategoryColor 
            //                from CategoryMaster CM  Left Outer JOin ItemMaster IM ON IM.CategoryID = CM.ID WHERE CM.CompanyID=" + CompanyID.ToString() + " and CM.Room=" + RoomId.ToString() + " Group by CM.ID,CM.Category,CM.CategoryColor having SUM(ISNULL(IM.ExtendedCost,0)) > 0 ) qry) Select RownumberCost,CategoryName,Cost,CategoryColor from CatWithCost Where RownumberCost < " + (HowManyItemOnChart + 1).ToString() + " union Select " + (HowManyItemOnChart + 1).ToString() + " as RownumberCost,'[!restofothers!]' as ItemNumber,isnull(sum(Cost),0) as Cost,'' as CategoryColor   from CatWithCost Where RownumberCost > " + HowManyItemOnChart.ToString();
            //}
            //else //if (PieChartmetricOn == 3) for Manufacturer
            //{
            //    strQuery = @";with ManufWithCost as (
            //                Select ROW_NUMBER() over(Order by qry.Cost desc) as RownumberCost,ManufacturerName,Cost from (
            //                select SUM(ISNULL(IM.ExtendedCost,0)) AS Cost,MM.ID AS ManufacturerID,MM.Manufacturer AS ManufacturerName
            //                from ManufacturerMaster MM Left Outer JOin ItemMaster IM ON IM.ManufacturerID = MM.ID WHERE MM.CompanyID=" + CompanyID.ToString() + " and MM.Room=" + RoomId.ToString() + " Group by MM.ID,MM.Manufacturer having SUM(ISNULL(IM.ExtendedCost,0)) > 0 ) qry) Select RownumberCost,ManufacturerName AS CategoryName,Cost,'' as CategoryColor from ManufWithCost Where RownumberCost < " + (HowManyItemOnChart + 1).ToString() + " union Select " + (HowManyItemOnChart + 1).ToString() + " as RownumberCost,'[!restofothers!]' as ManufacturerName,isnull(sum(Cost),0) as Cost,'' as CategoryColor from ManufWithCost Where RownumberCost > " + HowManyItemOnChart.ToString();
            //}
            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    lst = (from u in context.Database.SqlQuery<ItemMasterDTO>(strQuery)
            //           select new ItemMasterDTO
            //           {
            //               RownumberCost = u.RownumberCost,
            //               Cost = u.Cost,
            //               CategoryName = u.CategoryName,
            //               CategoryColor = u.CategoryColor
            //           }).ToList();


            //    return lst;
            //}
        }

        #endregion

        public List<ProjectSpendItemsDTO> GetProjectsForDashboard(long RoomId, long CompanyId, int HowmanyTop, bool IsForCount, string WidgetType)
        {
            List<ProjectSpendItemsDTO> lstProjects = new List<ProjectSpendItemsDTO>();
            DataSet dsProjects = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstProjects;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            dsProjects = SqlHelper.ExecuteDataset(EturnsConnection, "ProjectSpendTop", RoomId, CompanyId, HowmanyTop, IsForCount, WidgetType);

            if (dsProjects != null && dsProjects.Tables.Count > 0)
            {
                DataTable dtProjects = dsProjects.Tables[0];
                if (dtProjects != null && dtProjects.Rows.Count > 0)
                {
                    if (IsForCount)
                    {
                        lstProjects = (from dr in dtProjects.AsEnumerable()
                                       select new ProjectSpendItemsDTO()
                                       {
                                           ProjectSpendName = dr.Field<string>("CountType"),
                                           RownumberPS = dr.Field<int>("RedCount")
                                       }).ToList();

                    }
                    else
                    {
                        lstProjects = (from dr in dtProjects.AsEnumerable()
                                       select new ProjectSpendItemsDTO()
                                       {
                                           ID = dr.Field<long>("ID"),
                                           ProjectGUID = dr.Field<Guid>("GUID"),
                                           ProjectSpendName = dr.Field<string>("ProjectSpendName"),
                                           ItemNumber = dr.Field<string>("itemNumber"),
                                           RownumberPS = dr.Field<int>("RownumberPS"),
                                           ProjectPercent = (dr.Field<double?>("ProjectPercent")) ?? 0,
                                       }).ToList();

                    }
                }
            }
            return lstProjects;
        }

        public void SaveLargestAnnualCashSavingsForDashboard(Int64 DashBoardParmID, double LargestAnnualCashSavings)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                DashboardParameter objDashboardParameter = null;
                objDashboardParameter = context.DashboardParameters.FirstOrDefault(t => t.ID == DashBoardParmID);
                if (objDashboardParameter != null)
                {
                    objDashboardParameter.LargestAnnualCashSavings = LargestAnnualCashSavings;
                    context.SaveChanges();
                }
            }
        }

        public void ForwardDatesofPDPForDistributorWarehouse(Int64 RoomID, Int64 CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var Context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Context.Database.ExecuteSqlCommand("EXEC ForwardDatesofPDP @RoomID,@CompanyID", params1);
            }
        }

        public void RegenerateDataofPDPForDistributorWarehouse(Int64 RoomID, Int64 CompanyID, Int64 UserID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@UserID", UserID) };
            using (var Context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Context.Database.CommandTimeout = 3600;
                Context.Database.ExecuteSqlCommand("EXEC RegeneratePDPData @RoomID,@CompanyID,@UserID", params1);
            }
        }

        public void ResetMinMaxCritDataofPDPForDistributorWarehouse(Int64 RoomID, Int64 CompanyID, Int64 UserID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@UserID", UserID) };
            using (var Context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Context.Database.ExecuteSqlCommand("EXEC ResetMinMaxCritData @RoomID,@CompanyID,@UserID", params1);
            }
        }

        public ItemAvgCostQtyInfo GetItemOrRoomAvgCostQty(Guid ItemGUID, long RoomID, long CompanyID, DateTime StartDate, DateTime EndDate)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@StartDate", StartDate), new SqlParameter("@EndDate", EndDate) };
            using (var Context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return Context.Database.SqlQuery<ItemAvgCostQtyInfo>("EXEC [dbo].[GetItemOrRoomAvgCostQty] @ItemGUID,@RoomID,@CompanyID,@StartDate,@EndDate", params1).FirstOrDefault();
            }
        }

        public List<PullMaster> GetPullByRoomAndDateRange(long RoomId, long CompanyId, DateTime? FromDate, DateTime Todate)
        {
            var paramsInner = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomId),
                                                new SqlParameter("@CompanyID", CompanyId),
                                                new SqlParameter("@FromDate", (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@ToDate", Todate.ToString("yyyy-MM-dd HH:mm:ss"))
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<PullMaster>("exec GetPullByRoomAndDateRange @RoomID,@CompanyID,@FromDate,@ToDate ", paramsInner).ToList();
            }
        }

        public List<TransferDetailDTO> GetTransferByRoomAndDateRange(long RoomId, long CompanyId, DateTime? FromDate, DateTime Todate)
        {
            var paramsInner = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomId),
                                                new SqlParameter("@CompanyID", CompanyId),
                                                new SqlParameter("@FromDate", (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@ToDate", Todate.ToString("yyyy-MM-dd HH:mm:ss"))
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<TransferDetailDTO>("exec GetTransferByRoomAndDateRange @RoomID,@CompanyID,@FromDate,@ToDate ", paramsInner).ToList();
            }
        }

        public List<OrderDetailsDTO> GetOrderByRoomAndDateRange(long RoomId, long CompanyId, DateTime? FromDate, DateTime Todate)
        {
            var paramInnerCase = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomId),
                                                new SqlParameter("@CompanyID", CompanyId),
                                                new SqlParameter("@FromDate", (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@ToDate", (Todate).ToString("yyyy-MM-dd HH:mm:ss"))
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderDetailsDTO>("exec GetOrderByRoomAndDateRange @RoomID,@CompanyID,@FromDate,@ToDate ", paramInnerCase).ToList();
            }
        }

        public List<PullMaster> GetPullByItemGuidAndDateRange(long RoomId, long CompanyId, Guid ItemGUID, DateTime? FromDate, DateTime Todate)
        {
            var paramsInner = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomId),
                                                new SqlParameter("@CompanyID", CompanyId),
                                                new SqlParameter("@ItemGuid", ItemGUID),
                                                new SqlParameter("@FromDate", (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@ToDate", Todate.ToString("yyyy-MM-dd HH:mm:ss"))
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<PullMaster>("exec GetPullByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramsInner).ToList();
            }
        }

        public List<TransferDetailDTO> GetTransferByItemGuidAndDateRange(long RoomId, long CompanyId, Guid ItemGUID, DateTime? FromDate, DateTime Todate, bool IsIncludeCost)
        {
            var paramsInner = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomId),
                                                new SqlParameter("@CompanyID", CompanyId),
                                                new SqlParameter("@ItemGuid", ItemGUID),
                                                new SqlParameter("@FromDate", (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@ToDate", Todate.ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@IsIncludeCost", IsIncludeCost)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<TransferDetailDTO>("exec GetTransferByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate,@IsIncludeCost ", paramsInner).ToList();
            }
        }

        public List<ItemTransactionDTO> GetItemTransferTransactionHistoryPlain(Guid ItemGUID, DateTime? FromDate, DateTime Todate)
        {
            var paramsInner = new SqlParameter[] {
                                                new SqlParameter("@ItemGuid", ItemGUID),
                                                new SqlParameter("@FromDate", (FromDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss")),
                                                new SqlParameter("@ToDate", (Todate).ToString("yyyy-MM-dd HH:mm:ss"))
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemTransactionDTO>("exec [GetItemTransferTransactionHistoryPlain] @ItemGuid,@FromDate,@ToDate ", paramsInner).ToList();
            }
        }

        public List<OrderDetailsDTO> GetOrderByItemGuidAndDateRange(long RoomId, long CompanyId, Guid ItemGUID, DateTime? FromDate, DateTime Todate)
        {
            var paramInnerCase = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomId),
                                                new SqlParameter("@CompanyID", CompanyId),
                                                new SqlParameter("@ItemGuid", ItemGUID),
                                                new SqlParameter("@FromDate", (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH':'mm':'ss")),
                                                new SqlParameter("@ToDate", (Todate).ToString("yyyy-MM-dd HH':'mm':'ss"))
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderDetailsDTO>("exec GetOrderByItemGuidAndDateRange @RoomID,@CompanyID,@ItemGuid,@FromDate,@ToDate ", paramInnerCase).ToList();
            }
        }
        public bool CalculateMonthisedItemAnalyticsInsert(long RoomId, long CompanyId, int ForMonth, int ForYear, out string ExceptionString, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null)
        {
            try
            {
                ExceptionString = "";
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);

                DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                if (objRegionalSettings == null)
                {
                    objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
                }

                if (objDashboardParameterDTO == null)
                {
                    objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
                }
                string CurrentRoomTimeZone = "UTC";
                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName;
                    if (string.IsNullOrWhiteSpace(CurrentRoomTimeZone))
                    {
                        CurrentRoomTimeZone = "UTC";
                    }
                }

                DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    //IQueryable<ItemMaster> lstItems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false);
                    List<ItemMasterDTO> lstItems = objItemMasterDAL.GetMissingmonthEndCalcItems(RoomId, CompanyId, ForMonth, ForYear);
                    if (lstItems != null && lstItems.Any())
                    {
                        foreach (var objItem in lstItems)
                        {
                            try
                            {
                                double PullCost = 0;
                                double ItemInventoryValue = 0;
                                double OnHandQuantity = 0;
                                double PullQuantity = 0;
                                double OrderedQty = 0;
                                double ReturnOrderedQty = 0;
                                double FinalOrderedQty = 0;
                                int daysinmonth = DateTime.DaysInMonth(ForYear, ForMonth);
                                DateTime? FromDate = new DateTime(ForYear, ForMonth, 1, 0, 0, 0);
                                DateTime Todate = new DateTime(ForYear, ForMonth, daysinmonth, 23, 59, 0);
                                FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                                Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                                double minQty = objItem.MinimumQuantity ?? 0;
                                double maxQty = objItem.MaximumQuantity ?? 0;
                                BinMasterDAL objBinMaster = new BinMasterDAL(base.DataBaseName);
                                InventoryAnalysisMonthWise objInventoryAnalysisMonthWise = context.InventoryAnalysisMonthWises.FirstOrDefault(t => t.RoomId == RoomId && t.CompanyId == CompanyId && t.ItemGUID == objItem.GUID && t.CalculationMonth == ForMonth && t.CalculationYear == ForYear);
                                int TobeAdded = 0;
                                if (objInventoryAnalysisMonthWise == null)
                                {
                                    objInventoryAnalysisMonthWise = new InventoryAnalysisMonthWise();
                                    TobeAdded = 1;
                                }
                                else
                                {
                                    continue;
                                }
                                objInventoryAnalysisMonthWise.ItemGUID = objItem.GUID;
                                objInventoryAnalysisMonthWise.CalculationDate = DateTime.UtcNow;
                                objInventoryAnalysisMonthWise.CalculationFor = 1;
                                objInventoryAnalysisMonthWise.CalculationMonth = ForMonth;
                                objInventoryAnalysisMonthWise.CalculationYear = ForYear;
                                objInventoryAnalysisMonthWise.CompanyId = objItem.CompanyID ?? 0;
                                objInventoryAnalysisMonthWise.RoomId = objItem.Room ?? 0;

                                if (objItem.IsItemLevelMinMaxQtyRequired == false)
                                {
                                    //List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID)).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber).ToList();
                                    List<BinMasterDTO> Itemlocs = objBinMaster.GetAllRecordsByItemLocationLevelQuanity(RoomId, CompanyId, false, false, Convert.ToString(objItem.GUID), false, string.Empty, null, null).OrderBy(x => x.BinNumber).ToList();
                                    //List<BinMasterDTO> Itemlocs = objBinMaster.GetItemLocation(RoomId, CompanyId, false, false, (objItem.GUID),0,null,false).OrderBy(x => x.BinNumber).ToList();//.Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber).ToList();
                                    if (Itemlocs.Count > 0)
                                    {
                                        minQty = Itemlocs.First().MinimumQuantity ?? 0;
                                        maxQty = Itemlocs.First().MaximumQuantity ?? 0;
                                    }
                                }


                                //double DurationDiviser = 12 / objDashboardParameterDTO.TurnsMonthsOfUsageToSample.Value;

                                double Avgminmax = (maxQty + minQty) / 2;
                                ItemInventoryValue = objItem.ExtendedCost ?? 0;
                                OnHandQuantity = objItem.OnHandQuantity ?? 0;

                                double AverageExtendedCost = 0, AverageOnHandQuantity = 0;
                                ItemAvgCostQtyInfo objItemAvgCostQtyInfo = objDashboardDAL.GetItemOrRoomAvgCostQty(objItem.GUID, objItem.Room ?? 0, objItem.CompanyID ?? 0, (FromDate ?? DateTime.Now.AddDays(1)), Todate);
                                if (objItemAvgCostQtyInfo != null)
                                {
                                    AverageExtendedCost = objItemAvgCostQtyInfo.AverageExtendedCost;
                                    AverageOnHandQuantity = objItemAvgCostQtyInfo.AverageOnHandQuantity;
                                }


                                List<PullMaster> PullMasterList = null;
                                List<OrderDetailsDTO> lstOrderDetails = null;


                                PullMasterList = (from pm in context.Database.SqlQuery<PullMaster>("Select * from PullMaster as PM where pm.ItemGUID='" + objItem.GUID + "' and  pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
                                                  select pm).ToList();

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                                    PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                                    PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                                }

                                string qry = "SELECT Om.OrderType as ItemType, OD.* FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE OD.ItemGUID = '" + objItem.GUID + "' AND Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId + " AND OD.ReceivedOn >= '" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' AND OD.ReceivedOn <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";

                                lstOrderDetails = (from pm in context.Database.SqlQuery<OrderDetailsDTO>(qry)
                                                   select pm).ToList();

                                if (lstOrderDetails.Any())
                                {
                                    OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                                    ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                                }
                                FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                                objInventoryAnalysisMonthWise.MaximumQuantity = maxQty;
                                objInventoryAnalysisMonthWise.MinimumQuantity = minQty;
                                objInventoryAnalysisMonthWise.MonthizedInventoryValue = ItemInventoryValue;
                                objInventoryAnalysisMonthWise.MonthizedOnHandQuantity = OnHandQuantity;
                                objInventoryAnalysisMonthWise.MonthizedOrderQuantity = FinalOrderedQty;
                                objInventoryAnalysisMonthWise.MonthizedPullQuantity = PullQuantity;
                                objInventoryAnalysisMonthWise.MonthizedPullCost = PullCost;

                                if (AverageExtendedCost > 0)
                                {
                                    objInventoryAnalysisMonthWise.MonthizedPullValueTurns = (PullCost / AverageExtendedCost) * 12;
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedPullValueTurns = 0;
                                }
                                if (AverageOnHandQuantity > 0)
                                {
                                    objInventoryAnalysisMonthWise.MonthizedPullTurns = (PullQuantity / AverageOnHandQuantity) * 12;
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedPullTurns = 0;
                                }
                                if (Avgminmax > 0)
                                {
                                    objInventoryAnalysisMonthWise.MonthizedOrderTurns = (FinalOrderedQty / Avgminmax) * 12;
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedOrderTurns = 0;
                                }

                                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4)
                                {
                                    if (objDashboardParameterDTO.TurnsMeasureMethod == 1)
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullValueTurns;
                                    }
                                    else if (objDashboardParameterDTO.TurnsMeasureMethod == 2)
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullTurns;
                                    }
                                    else
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedOrderTurns;
                                    }
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullTurns;
                                }

                                objInventoryAnalysisMonthWise.MonthizedPullValueAverageUsage = PullCost / daysinmonth;
                                objInventoryAnalysisMonthWise.MonthizedPullAverageUsage = PullQuantity / daysinmonth;
                                objInventoryAnalysisMonthWise.MonthizedOrderAverageUsage = FinalOrderedQty / daysinmonth;

                                if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4)
                                {
                                    if (objDashboardParameterDTO.AUMeasureMethod == 1)
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullValueAverageUsage;
                                    }
                                    else if (objDashboardParameterDTO.AUMeasureMethod == 2)
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullAverageUsage;
                                    }
                                    else
                                    {
                                        objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedOrderAverageUsage;
                                    }
                                }
                                else
                                {
                                    objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullAverageUsage;
                                }



                                DashboardAnalysisInfo objDashboardAnalysisInfoTurn = GetTurnsByItemGUID(RoomId, CompanyId, objItem.GUID, 0, objDashboardParameterDTO, objRegionalSettings);
                                DashboardAnalysisInfo objDashboardAnalysisInfoAU = GetAvgUsageByItemGUID(RoomId, CompanyId, objItem.GUID, 0, objDashboardParameterDTO, objRegionalSettings);

                                objInventoryAnalysisMonthWise.AnnualizedOrderTurns = objDashboardAnalysisInfoTurn.MonthizedOrderTurns ?? 0;
                                objInventoryAnalysisMonthWise.AnnualizedPullTurns = objDashboardAnalysisInfoTurn.MonthizedPullTurns ?? 0;
                                objInventoryAnalysisMonthWise.AnnualizedPullValueTurns = objDashboardAnalysisInfoTurn.MonthizedPullValueTurns ?? 0;

                                objInventoryAnalysisMonthWise.AnnualizedPullAverageUsage = objDashboardAnalysisInfoAU.CalculatedPullAverageUsage;
                                objInventoryAnalysisMonthWise.AnnualizedOrderAverageUsage = objDashboardAnalysisInfoAU.CalculatedOrderAverageUsage;
                                objInventoryAnalysisMonthWise.AnnualizedPullValueAverageUsage = objDashboardAnalysisInfoAU.CalculatedPullValueAverageUsage;



                                //if (objDashboardParameterDTO != null)
                                //{

                                //}
                                //if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.TurnsMonthsOfUsageToSample ?? 0) > 0)
                                //{

                                //}
                                //else
                                //{

                                //}
                                //if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.AUDayOfUsageToSample ?? 0) > 0)
                                //{

                                //}
                                //else
                                //{

                                //}




                                //MonthizedTurns
                                //MonthizedAverageUsage

                                //MonthizedPullValueTurns
                                //MonthizedPullTurns
                                //MonthizedOrderTurns                        
                                //MonthizedPullValueAverageUsage
                                //MonthizedPullAverageUsage
                                //MonthizedOrderAverageUsage



                                //----------------------NEW COLUMNS ADDED FOR TRACKING----------------------
                                //
                                objInventoryAnalysisMonthWise.ManufacturerNumber = objItem.ManufacturerNumber;
                                objInventoryAnalysisMonthWise.SupplierPartNo = objItem.SupplierPartNo;
                                objInventoryAnalysisMonthWise.UPC = objItem.UPC;
                                objInventoryAnalysisMonthWise.DefaultReorderQuantity = objItem.DefaultReorderQuantity;
                                objInventoryAnalysisMonthWise.DefaultPullQuantity = objItem.DefaultPullQuantity;
                                objInventoryAnalysisMonthWise.Cost = objItem.Cost;
                                objInventoryAnalysisMonthWise.Markup = objItem.Markup;
                                objInventoryAnalysisMonthWise.SellPrice = objItem.SellPrice;
                                objInventoryAnalysisMonthWise.ExtendedCost = objItem.ExtendedCost;
                                objInventoryAnalysisMonthWise.Trend = objItem.Trend;
                                objInventoryAnalysisMonthWise.Taxable = objItem.Taxable;
                                objInventoryAnalysisMonthWise.Consignment = objItem.Consignment;
                                objInventoryAnalysisMonthWise.StagedQuantity = objItem.StagedQuantity;
                                objInventoryAnalysisMonthWise.InTransitquantity = objItem.InTransitquantity;
                                objInventoryAnalysisMonthWise.OnOrderQuantity = objItem.OnOrderQuantity;
                                objInventoryAnalysisMonthWise.OnTransferQuantity = objItem.OnTransferQuantity;
                                objInventoryAnalysisMonthWise.SuggestedOrderQuantity = objItem.SuggestedOrderQuantity;
                                objInventoryAnalysisMonthWise.RequisitionedQuantity = objItem.RequisitionedQuantity;
                                objInventoryAnalysisMonthWise.AverageUsage = objItem.AverageUsage;
                                objInventoryAnalysisMonthWise.Turns = objItem.Turns;
                                objInventoryAnalysisMonthWise.OnHandQuantity = objItem.OnHandQuantity;
                                objInventoryAnalysisMonthWise.CriticalQuantity = objItem.CriticalQuantity;
                                objInventoryAnalysisMonthWise.WeightPerPiece = objItem.WeightPerPiece;
                                objInventoryAnalysisMonthWise.ItemUniqueNumber = objItem.ItemUniqueNumber;
                                objInventoryAnalysisMonthWise.IsTransfer = objItem.IsTransfer;
                                objInventoryAnalysisMonthWise.IsPurchase = objItem.IsPurchase;
                                objInventoryAnalysisMonthWise.DefaultLocation = objItem.DefaultLocation;
                                objInventoryAnalysisMonthWise.InventoryClassification = objItem.InventoryClassification;
                                objInventoryAnalysisMonthWise.SerialNumberTracking = objItem.SerialNumberTracking;
                                objInventoryAnalysisMonthWise.LotNumberTracking = objItem.LotNumberTracking;
                                objInventoryAnalysisMonthWise.DateCodeTracking = objItem.DateCodeTracking;
                                objInventoryAnalysisMonthWise.ItemType = objItem.ItemType;
                                objInventoryAnalysisMonthWise.IsLotSerialExpiryCost = objItem.IsLotSerialExpiryCost;
                                objInventoryAnalysisMonthWise.IsItemLevelMinMaxQtyRequired = objItem.IsItemLevelMinMaxQtyRequired;
                                objInventoryAnalysisMonthWise.IsEnforceDefaultReorderQuantity = objItem.IsEnforceDefaultReorderQuantity;
                                objInventoryAnalysisMonthWise.AverageCost = objItem.AverageCost;
                                objInventoryAnalysisMonthWise.IsBuildBreak = objItem.IsBuildBreak;
                                objInventoryAnalysisMonthWise.BondedInventory = objItem.BondedInventory;
                                objInventoryAnalysisMonthWise.OnReturnQuantity = objItem.OnReturnQuantity;
                                objInventoryAnalysisMonthWise.TrendingSetting = objItem.TrendingSetting;
                                objInventoryAnalysisMonthWise.PullQtyScanOverride = objItem.PullQtyScanOverride;
                                objInventoryAnalysisMonthWise.IsAutoInventoryClassification = objItem.IsAutoInventoryClassification;
                                objInventoryAnalysisMonthWise.IsPackslipMandatoryAtReceive = objItem.IsPackslipMandatoryAtReceive;
                                objInventoryAnalysisMonthWise.SuggestedTransferQuantity = objItem.SuggestedTransferQuantity;
                                objInventoryAnalysisMonthWise.QtyToMeetDemand = objItem.QtyToMeetDemand;

                                if (TobeAdded == 1)
                                {
                                    context.InventoryAnalysisMonthWises.Add(objInventoryAnalysisMonthWise);
                                }
                            }
                            catch (Exception EX)
                            {
                                ExceptionString = ExceptionString + (ExceptionString == "" ? "" : Environment.NewLine) +
                                                  "--==========[" + base.DataBaseName + ">>" + CompanyId.ToString() + ">>" + RoomId.ToString() + ">>" + objItem.ItemNumber + "]==========--";
                                ExceptionString = ExceptionString + Environment.NewLine + "Exception Message:" + Environment.NewLine + EX.Message;
                                if (EX.InnerException != null && !String.IsNullOrEmpty(EX.InnerException.Message))
                                {
                                    ExceptionString = ExceptionString + Environment.NewLine + "Inner Exception Message:" + Environment.NewLine + EX.InnerException.Message;
                                }
                            }
                        }
                    }
                    context.SaveChanges();

                }

                return true;
            }
            catch (Exception EX)
            {
                ExceptionString = "--==========[" + base.DataBaseName + ">>" + CompanyId.ToString() + ">>" + RoomId.ToString() + "]==========--";
                ExceptionString = ExceptionString + Environment.NewLine + "Exception Message:" + Environment.NewLine + EX.Message;
                if (EX.InnerException != null && !String.IsNullOrEmpty(EX.InnerException.Message))
                {
                    ExceptionString = ExceptionString + Environment.NewLine + "Inner Exception Message:" + Environment.NewLine + EX.InnerException.Message;
                }
                return false;
            }
        }

        public bool CalculateMonthisedRoomAnalyticsInsert(long RoomId, long CompanyId, int ForMonth, int ForYear, out string ExceptionString, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null)
        {
            try
            {
                ExceptionString = "";
                DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                if (objRegionalSettings == null)
                {
                    objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
                }
                if (objDashboardParameterDTO == null)
                {
                    objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
                }
                string CurrentRoomTimeZone = "UTC";
                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName;
                    if (string.IsNullOrWhiteSpace(CurrentRoomTimeZone))
                    {
                        CurrentRoomTimeZone = "UTC";
                    }
                }

                DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    double PullCost = 0;
                    double ItemInventoryValue = 0;
                    double OnHandQuantity = 0;
                    double PullQuantity = 0;
                    double OrderedQty = 0;
                    double ReturnOrderedQty = 0;
                    double FinalOrderedQty = 0;
                    int daysinmonth = DateTime.DaysInMonth(ForYear, ForMonth);
                    DateTime? FromDate = new DateTime(ForYear, ForMonth, 1, 0, 0, 0);
                    DateTime Todate = new DateTime(ForYear, ForMonth, daysinmonth, 23, 59, 0);
                    FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                    Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

                    double minQty = 0;
                    double maxQty = 0;
                    BinMasterDAL objBinMaster = new BinMasterDAL(base.DataBaseName);
                    InventoryAnalysisMonthWise objInventoryAnalysisMonthWise = context.InventoryAnalysisMonthWises.FirstOrDefault(t => t.RoomId == RoomId && t.CompanyId == CompanyId && t.ItemGUID == Guid.Empty && t.CalculationMonth == ForMonth && t.CalculationYear == ForYear);
                    int TobeAdded = 0;
                    if (objInventoryAnalysisMonthWise == null)
                    {
                        objInventoryAnalysisMonthWise = new InventoryAnalysisMonthWise();
                        TobeAdded = 1;
                    }
                    else
                    {
                        return true;
                    }
                    objInventoryAnalysisMonthWise.ItemGUID = Guid.Empty;
                    objInventoryAnalysisMonthWise.CalculationDate = DateTime.UtcNow;
                    objInventoryAnalysisMonthWise.CalculationFor = 2;
                    objInventoryAnalysisMonthWise.CalculationMonth = ForMonth;
                    objInventoryAnalysisMonthWise.CalculationYear = ForYear;
                    objInventoryAnalysisMonthWise.CompanyId = CompanyId;
                    objInventoryAnalysisMonthWise.RoomId = RoomId;


                    double Avgminmax = 0;
                    var qryitems = context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false);
                    if (qryitems.Any())
                    {
                        OnHandQuantity = qryitems.Sum(t => (t.OnHandQuantity ?? 0));
                        ItemInventoryValue = qryitems.Sum(t => (t.ExtendedCost ?? 0));
                        Avgminmax = qryitems.Sum(t => ((t.MinimumQuantity) + (t.MaximumQuantity) / 2));
                    }







                    List<PullMaster> PullMasterList = null;
                    List<OrderDetailsDTO> lstOrderDetails = null;


                    PullMasterList = (from pm in context.Database.SqlQuery<PullMaster>("Select * from PullMaster as PM where  pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
                                      select pm).ToList();

                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                    {
                        PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                        PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                    }
                    if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                    {
                        PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                        PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                    }

                    string qry = "SELECT Om.OrderType as ItemType, OD.* FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE  Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId + " AND OD.ReceivedOn >= '" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' AND OD.ReceivedOn <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";

                    lstOrderDetails = (from pm in context.Database.SqlQuery<OrderDetailsDTO>(qry)
                                       select pm).ToList();

                    if (lstOrderDetails.Any())
                    {
                        OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                        ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                    }
                    FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                    objInventoryAnalysisMonthWise.MaximumQuantity = maxQty;
                    objInventoryAnalysisMonthWise.MinimumQuantity = minQty;
                    objInventoryAnalysisMonthWise.MonthizedInventoryValue = ItemInventoryValue;
                    objInventoryAnalysisMonthWise.MonthizedOnHandQuantity = OnHandQuantity;
                    objInventoryAnalysisMonthWise.MonthizedOrderQuantity = FinalOrderedQty;
                    objInventoryAnalysisMonthWise.MonthizedPullQuantity = PullQuantity;
                    objInventoryAnalysisMonthWise.MonthizedPullCost = PullCost;
                    double AverageExtendedCost = 0, AverageOnHandQuantity = 0;
                    ItemAvgCostQtyInfo objItemAvgCostQtyInfo = objDashboardDAL.GetItemOrRoomAvgCostQty(Guid.Empty, RoomId, CompanyId, (FromDate ?? DateTime.Now.AddDays(1)), Todate);
                    if (objItemAvgCostQtyInfo != null)
                    {
                        AverageExtendedCost = objItemAvgCostQtyInfo.AverageExtendedCost;
                        AverageOnHandQuantity = objItemAvgCostQtyInfo.AverageOnHandQuantity;
                    }

                    if (AverageExtendedCost > 0)
                    {
                        objInventoryAnalysisMonthWise.MonthizedPullValueTurns = (PullCost / AverageExtendedCost) * 12;
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedPullValueTurns = 0;
                    }
                    if (AverageOnHandQuantity > 0)
                    {
                        objInventoryAnalysisMonthWise.MonthizedPullTurns = (PullQuantity / AverageOnHandQuantity) * 12;
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedPullTurns = 0;
                    }
                    if (Avgminmax > 0)
                    {
                        objInventoryAnalysisMonthWise.MonthizedOrderTurns = (FinalOrderedQty / Avgminmax);
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedOrderTurns = 0;
                    }

                    if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4)
                    {
                        if (objDashboardParameterDTO.TurnsMeasureMethod == 1)
                        {
                            objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullValueTurns;
                        }
                        else if (objDashboardParameterDTO.TurnsMeasureMethod == 2)
                        {
                            objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullTurns;
                        }
                        else
                        {
                            objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedOrderTurns;
                        }
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedTurns = objInventoryAnalysisMonthWise.MonthizedPullTurns;
                    }

                    objInventoryAnalysisMonthWise.MonthizedPullValueAverageUsage = PullCost / daysinmonth;
                    objInventoryAnalysisMonthWise.MonthizedPullAverageUsage = PullQuantity / daysinmonth;
                    objInventoryAnalysisMonthWise.MonthizedOrderAverageUsage = FinalOrderedQty / daysinmonth;

                    if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4)
                    {
                        if (objDashboardParameterDTO.AUMeasureMethod == 1)
                        {
                            objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullValueAverageUsage;
                        }
                        else if (objDashboardParameterDTO.AUMeasureMethod == 2)
                        {
                            objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullAverageUsage;
                        }
                        else
                        {
                            objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedOrderAverageUsage;
                        }
                    }
                    else
                    {
                        objInventoryAnalysisMonthWise.MonthizedAverageUsage = objInventoryAnalysisMonthWise.MonthizedPullAverageUsage;
                    }



                    DashboardAnalysisInfo objDashboardAnalysisInfoTurn = GetTurnsByRoom(RoomId, CompanyId, 0, objDashboardParameterDTO, objRegionalSettings);
                    DashboardAnalysisInfo objDashboardAnalysisInfoAU = GetAvgUsageByRoom(RoomId, CompanyId, 0, objDashboardParameterDTO, objRegionalSettings);

                    objInventoryAnalysisMonthWise.AnnualizedOrderTurns = objDashboardAnalysisInfoTurn.MonthizedOrderTurns ?? 0;
                    objInventoryAnalysisMonthWise.AnnualizedPullTurns = objDashboardAnalysisInfoTurn.MonthizedPullTurns ?? 0;
                    objInventoryAnalysisMonthWise.AnnualizedPullValueTurns = objDashboardAnalysisInfoTurn.MonthizedPullValueTurns ?? 0;

                    objInventoryAnalysisMonthWise.AnnualizedPullAverageUsage = objDashboardAnalysisInfoAU.CalculatedPullAverageUsage;
                    objInventoryAnalysisMonthWise.AnnualizedOrderAverageUsage = objDashboardAnalysisInfoAU.CalculatedOrderAverageUsage;
                    objInventoryAnalysisMonthWise.AnnualizedPullValueAverageUsage = objDashboardAnalysisInfoAU.CalculatedPullValueAverageUsage;


                    //if (objDashboardParameterDTO != null)
                    //{

                    //}
                    //if (objDashboardParameterDTO != null && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.TurnsMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.TurnsMonthsOfUsageToSample ?? 0) > 0)
                    //{

                    //}
                    //else
                    //{

                    //}
                    //if (objDashboardParameterDTO != null && (objDashboardParameterDTO.AUMeasureMethod ?? 0) > 0 && (objDashboardParameterDTO.AUMeasureMethod ?? 0) < 4 && (objDashboardParameterDTO.AUDayOfUsageToSample ?? 0) > 0)
                    //{

                    //}
                    //else
                    //{

                    //}




                    //MonthizedTurns
                    //MonthizedAverageUsage

                    //MonthizedPullValueTurns
                    //MonthizedPullTurns
                    //MonthizedOrderTurns                        
                    //MonthizedPullValueAverageUsage
                    //MonthizedPullAverageUsage
                    //MonthizedOrderAverageUsage



                    //----------------------NEW COLUMNS ADDED FOR TRACKING----------------------
                    //
                    //objInventoryAnalysisMonthWise.ManufacturerNumber = objItem.ManufacturerNumber;
                    //objInventoryAnalysisMonthWise.SupplierPartNo = objItem.SupplierPartNo;
                    //objInventoryAnalysisMonthWise.UPC = objItem.UPC;
                    //objInventoryAnalysisMonthWise.DefaultReorderQuantity = objItem.DefaultReorderQuantity;
                    //objInventoryAnalysisMonthWise.DefaultPullQuantity = objItem.DefaultPullQuantity;
                    //objInventoryAnalysisMonthWise.Cost = objItem.Cost;
                    //objInventoryAnalysisMonthWise.Markup = objItem.Markup;
                    //objInventoryAnalysisMonthWise.SellPrice = objItem.SellPrice;
                    //objInventoryAnalysisMonthWise.ExtendedCost = objItem.ExtendedCost;
                    //objInventoryAnalysisMonthWise.Trend = objItem.Trend;
                    //objInventoryAnalysisMonthWise.Taxable = objItem.Taxable;
                    //objInventoryAnalysisMonthWise.Consignment = objItem.Consignment;
                    //objInventoryAnalysisMonthWise.StagedQuantity = objItem.StagedQuantity;
                    //objInventoryAnalysisMonthWise.InTransitquantity = objItem.InTransitquantity;
                    //objInventoryAnalysisMonthWise.OnOrderQuantity = objItem.OnOrderQuantity;
                    //objInventoryAnalysisMonthWise.OnTransferQuantity = objItem.OnTransferQuantity;
                    //objInventoryAnalysisMonthWise.SuggestedOrderQuantity = objItem.SuggestedOrderQuantity;
                    //objInventoryAnalysisMonthWise.RequisitionedQuantity = objItem.RequisitionedQuantity;
                    //objInventoryAnalysisMonthWise.AverageUsage = objItem.AverageUsage;
                    //objInventoryAnalysisMonthWise.Turns = objItem.Turns;
                    //objInventoryAnalysisMonthWise.OnHandQuantity = objItem.OnHandQuantity;
                    //objInventoryAnalysisMonthWise.CriticalQuantity = objItem.CriticalQuantity;
                    //objInventoryAnalysisMonthWise.WeightPerPiece = objItem.WeightPerPiece;
                    //objInventoryAnalysisMonthWise.ItemUniqueNumber = objItem.ItemUniqueNumber;
                    //objInventoryAnalysisMonthWise.IsTransfer = objItem.IsTransfer;
                    //objInventoryAnalysisMonthWise.IsPurchase = objItem.IsPurchase;
                    //objInventoryAnalysisMonthWise.DefaultLocation = objItem.DefaultLocation;
                    //objInventoryAnalysisMonthWise.InventoryClassification = objItem.InventoryClassification;
                    //objInventoryAnalysisMonthWise.SerialNumberTracking = objItem.SerialNumberTracking;
                    //objInventoryAnalysisMonthWise.LotNumberTracking = objItem.LotNumberTracking;
                    //objInventoryAnalysisMonthWise.DateCodeTracking = objItem.DateCodeTracking;
                    //objInventoryAnalysisMonthWise.ItemType = objItem.ItemType;
                    //objInventoryAnalysisMonthWise.IsLotSerialExpiryCost = objItem.IsLotSerialExpiryCost;
                    //objInventoryAnalysisMonthWise.IsItemLevelMinMaxQtyRequired = objItem.IsItemLevelMinMaxQtyRequired;
                    //objInventoryAnalysisMonthWise.IsEnforceDefaultReorderQuantity = objItem.IsEnforceDefaultReorderQuantity;
                    //objInventoryAnalysisMonthWise.AverageCost = objItem.AverageCost;
                    //objInventoryAnalysisMonthWise.IsBuildBreak = objItem.IsBuildBreak;
                    //objInventoryAnalysisMonthWise.BondedInventory = objItem.BondedInventory;
                    //objInventoryAnalysisMonthWise.OnReturnQuantity = objItem.OnReturnQuantity;
                    //objInventoryAnalysisMonthWise.TrendingSetting = objItem.TrendingSetting;
                    //objInventoryAnalysisMonthWise.PullQtyScanOverride = objItem.PullQtyScanOverride;
                    //objInventoryAnalysisMonthWise.IsAutoInventoryClassification = objItem.IsAutoInventoryClassification;
                    //objInventoryAnalysisMonthWise.IsPackslipMandatoryAtReceive = objItem.IsPackslipMandatoryAtReceive;
                    //objInventoryAnalysisMonthWise.SuggestedTransferQuantity = objItem.SuggestedTransferQuantity;
                    //objInventoryAnalysisMonthWise.QtyToMeetDemand = objItem.QtyToMeetDemand;

                    if (TobeAdded == 1)
                    {
                        context.InventoryAnalysisMonthWises.Add(objInventoryAnalysisMonthWise);
                    }

                    context.SaveChanges();

                }

                return true;
            }
            catch (Exception EX)
            {
                ExceptionString = "--==========[" + base.DataBaseName + ">>" + CompanyId.ToString() + ">>" + RoomId.ToString() + "]==========--";
                ExceptionString = ExceptionString + Environment.NewLine + "Exception Message:" + Environment.NewLine + EX.Message;
                if (EX.InnerException != null && !String.IsNullOrEmpty(EX.InnerException.Message))
                {
                    ExceptionString = ExceptionString + Environment.NewLine + "Inner Exception Message:" + Environment.NewLine + EX.InnerException.Message;
                }
                return false;
            }
        }

        public long InsertMinMaxActionAuditTrail(MinMaxDashboardAuditTrailDTO dashboardAuditTrailDTO)
        {
            long id = 0;
            if (dashboardAuditTrailDTO != null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    MinMaxDashboardAuditTrail objMinMaxDashboardAuditTrail = null;
                    objMinMaxDashboardAuditTrail = new MinMaxDashboardAuditTrail();
                    objMinMaxDashboardAuditTrail.GUID = dashboardAuditTrailDTO.GUID;
                    objMinMaxDashboardAuditTrail.RoomID = dashboardAuditTrailDTO.RoomID;
                    objMinMaxDashboardAuditTrail.CompanyID = dashboardAuditTrailDTO.CompanyID;
                    objMinMaxDashboardAuditTrail.ItemGuid = dashboardAuditTrailDTO.ItemGuid.GetValueOrDefault(Guid.Empty);
                    objMinMaxDashboardAuditTrail.BinID = dashboardAuditTrailDTO.BinID.GetValueOrDefault(0);
                    objMinMaxDashboardAuditTrail.UserID = dashboardAuditTrailDTO.UserID;
                    objMinMaxDashboardAuditTrail.Action = dashboardAuditTrailDTO.Action;
                    objMinMaxDashboardAuditTrail.ActionDate = DateTimeUtility.DateTimeNow;
                    context.MinMaxDashboardAuditTrails.Add(objMinMaxDashboardAuditTrail);
                    context.SaveChanges();
                    id = objMinMaxDashboardAuditTrail.ID;
                }
            }
            return id;
        }

        public IEnumerable<MinMaxDashboardAuditTrailDTO> GetMinMaxActionAuditTrail(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, DateTime? FromDate, DateTime? ToDate)
        {
            List<MinMaxDashboardAuditTrailDTO> lstMinMaxs = new List<MinMaxDashboardAuditTrailDTO>();
            TotalCount = 0;
            MinMaxDashboardAuditTrailDTO objItemDTO = new MinMaxDashboardAuditTrailDTO();

            DataSet dsMinMax = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstMinMaxs;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsMinMax = SqlHelper.ExecuteDataset(EturnsConnection, "GetMinMaxActionAuditTrail", RoomID, CompanyID, (FromDate ?? (object)DBNull.Value), (ToDate ?? (object)DBNull.Value), StartRowIndex, MaxRows, SearchTerm, sortColumnName);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                    {
                        SearchTerm = Fields[2];
                    }
                    else
                    {
                        SearchTerm = string.Empty;
                    }
                }
                else
                {
                    SearchTerm = string.Empty;
                }
                dsMinMax = SqlHelper.ExecuteDataset(EturnsConnection, "GetMinMaxActionAuditTrail", RoomID, CompanyID, (FromDate ?? (object)DBNull.Value), (ToDate ?? (object)DBNull.Value), StartRowIndex, MaxRows, SearchTerm, sortColumnName);
            }
            else
            {
                dsMinMax = SqlHelper.ExecuteDataset(EturnsConnection, "GetMinMaxActionAuditTrail", RoomID, CompanyID, (FromDate ?? (object)DBNull.Value), (ToDate ?? (object)DBNull.Value), StartRowIndex, MaxRows, SearchTerm, sortColumnName);
            }
            if (dsMinMax != null && dsMinMax.Tables.Count > 0)
            {
                DataTable dtMinMax = dsMinMax.Tables[0];
                if (dtMinMax.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtMinMax.Rows[0]["TotalRecords"]);
                    lstMinMaxs = dtMinMax.AsEnumerable()
                    .Select(row => new MinMaxDashboardAuditTrailDTO
                    {
                        ID = row.Field<long>("ID"),
                        CompanyID = row.Field<long>("CompanyID"),
                        RoomID = row.Field<long>("RoomID"),
                        Action = row.Field<string>("Action"),
                        ActionDate = row.Field<DateTime>("ActionDate"),
                        UserID = row.Field<long>("UserID"),
                        CompanyName = row.Field<string>("CompanyName"),
                        RoomName = row.Field<string>("RoomName"),
                        UserName = row.Field<string>("UserName"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        BinNumber = row.Field<string>("BinNumber")
                    }).ToList();
                }
            }
            return lstMinMaxs;
        }

        public double GetMinMaxAvgByItemAndLocation(long RoomId, long CompanyId)
        {
            var paramInnerCase = new SqlParameter[] {
                                                new SqlParameter("@CompanyId", CompanyId),
                                                new SqlParameter("@RoomId", RoomId)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<double>("exec GetMinMaxAvgByItemAndLocation @CompanyId,@RoomId", paramInnerCase).FirstOrDefault();
            }
        }

    }
}
