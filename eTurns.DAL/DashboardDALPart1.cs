using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DAL
{
    public partial class DashboardDAL : eTurnsBaseDAL
    {
        public Dictionary<int, double> GetRoomTurnsInventoryValue(long RoomId, long CompanyId, eTurnsRegionInfo eTurnsRegionInfoProp)
        {
            double InventoryValue = 0;
            double StockRoomTurns = 0;
            int MTDStockouts = 0, YTDStockouts = 0;
            Dictionary<int, double> retData = new Dictionary<int, double>();
            DateTime YearstartDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
            DateTime MonthstartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            DashboardParameterDTO objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomId, CompanyId);
            DateTime? FromDate = DateTime.Now;
            DateTime Todate = DateTime.Now;
            List<PullMaster> PullMasterList = null;
            //IQueryable<ItemLocationDetail> lstItemLocations = null;
            List<OrderDetailsDTO> lstOrderDetails = null;
            double PullCost = 0;
            double OrderedQty = 0;
            double ReturnOrderedQty = 0;
            double PullQuantity = 0;
            double ItemInventoryValue = 0;
            double AvailableQty = 0;
            double Avgminmax = 0;
            double FinalOrderedQty = 0;
            string CurrentRoomTimeZone = "UTC";

            Int64 ItemsCount = 0;
            Int64 ItemsLocationCount = 0;
            Int64 ItemsStockOutCount = 0;
            Int64 ItemsWithOutCostCount = 0;
            Int64 ItemsTuningUpdateSettingCount = 0;
            double AnnualCarryingCostPercent = 0;
            double AnnualCarryingCostPercentfromDashboard = 25;

            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            eTurnsRegionInfo objRegionalSettings = eTurnsRegionInfoProp;
            if (objRegionalSettings == null)
            {
                objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomId, CompanyId, 0);
            }
            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }
            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
            double DurationDiviser = 1;
            if (objDashboardParameterDTO != null)
            {
                FromDate = CurrentTimeofTimeZone.AddDays(1).AddDays((objDashboardParameterDTO.TurnsDaysOfUsageToSample ?? 60) * (-1)).Date;
                Todate = new DateTime(CurrentTimeofTimeZone.Year, CurrentTimeofTimeZone.Month, CurrentTimeofTimeZone.Day, 23, 59, 59);
                FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);
                DurationDiviser = 365 / (objDashboardParameterDTO.TurnsDaysOfUsageToSample ?? 60);
                //FromDate = DateTime.Now.AddMonths((objDashboardParameterDTO.TurnsMonthsOfUsageToSample ?? 0) * (-1)).Date;
                //Todate = new DateTime(DateTime.Now.AddMonths(-1).Date.Year, DateTime.Now.AddMonths(-1).Date.Month, DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Date.Year, DateTime.Now.AddMonths(-1).Date.Month), 23, 59, 0);

                AnnualCarryingCostPercentfromDashboard = objDashboardParameterDTO.AnnualCarryingCostPercent;
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IQueryable<ItemMaster> lstItems = (from Item in context.ItemMasters
                                                   where (Item.IsArchived ?? false) == false && (Item.IsDeleted ?? false) == false && Item.Room == RoomId
                                                   select Item);
                if (lstItems != null && lstItems.Any())
                {
                    InventoryValue = Convert.ToDouble(lstItems.Sum(t => t.ExtendedCost ?? 0));
                    Avgminmax = lstItems.Sum(t => ((t.MinimumQuantity + t.MaximumQuantity) / 2));
                    ItemInventoryValue = Convert.ToDouble(lstItems.Sum(t => t.ExtendedCost ?? 0));
                    AvailableQty = lstItems.Sum(t => t.OnHandQuantity ?? 0);
                    //StockRoomTurns = (Convert.ToDouble(lstItems.Sum(t => t.Turns ?? 0)) / lstItems.Count);

                    ItemsCount = lstItems.Count();
                    ItemsWithOutCostCount = lstItems.Where(x => (x.Cost ?? 0) == 0).ToList().Count();
                    ItemsTuningUpdateSettingCount = lstItems.Where(x => (x.TrendingSetting ?? 0) == 0).ToList().Count();

                    AnnualCarryingCostPercent = ((InventoryValue * AnnualCarryingCostPercentfromDashboard) / 100);
                }

                ItemsLocationCount = (from bin in context.BinMasters
                                      where bin.IsArchived == false && bin.IsDeleted == false
                                      && bin.Room == RoomId
                                      && bin.ItemGUID != null
                                      && bin.IsStagingLocation == false
                                      select bin).ToList().Count();

                if (objDashboardParameterDTO != null)
                {
                    switch ((objDashboardParameterDTO.TurnsMeasureMethod ?? 0))
                    {
                        case 1:

                            PullMasterList = (from pm in context.ExecuteStoreQuery<PullMaster>("Select * from PullMaster as PM where  pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
                                              select pm).ToList();

                            if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                            {
                                PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                            }
                            if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                            {
                                PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                            }


                            if (PullCost > 0)
                            {
                                //lstItemLocations = (from ilq in context.ItemLocationDetails
                                //                    where (ilq.IsDeleted ?? false) == false && ilq.Room == RoomId && ilq.CompanyID == CompanyId
                                //                    select ilq);

                                //if (lstItemLocations.Any())
                                //{
                                //    ItemInventoryValue = lstItemLocations.Select(t => ((t.ConsignedQuantity ?? 0) + (t.CustomerOwnedQuantity ?? 0)) * (double)(t.Cost ?? 0)).Sum();
                                //}
                                if (ItemInventoryValue > 0)
                                {
                                    StockRoomTurns = ((PullCost) / (ItemInventoryValue)) * DurationDiviser;
                                }
                                else
                                {
                                    StockRoomTurns = 0;
                                }
                            }
                            else
                            {
                                StockRoomTurns = 0;
                            }
                            break;
                        case 2:
                            PullMasterList = (from pm in context.ExecuteStoreQuery<PullMaster>("Select * from PullMaster as PM where  pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
                                              select pm).ToList();

                            if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                            {
                                PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                            }
                            if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                            {
                                PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                            }
                            if (PullQuantity > 0)
                            {
                                //lstItemLocations = (from ilq in context.ItemLocationDetails
                                //                    where (ilq.IsDeleted ?? false) == false && ilq.Room == RoomId && ilq.CompanyID == CompanyId
                                //                    select ilq);

                                //if (lstItemLocations.Any())
                                //{
                                //    AvailableQty = lstItemLocations.Select(t => ((t.ConsignedQuantity ?? 0) + (t.CustomerOwnedQuantity ?? 0))).Sum();
                                //}
                                if (AvailableQty > 0)
                                {
                                    StockRoomTurns = ((PullQuantity) / (AvailableQty)) * DurationDiviser;
                                }
                                else
                                {
                                    StockRoomTurns = 0;
                                }
                            }
                            else
                            {
                                StockRoomTurns = 0;
                            }
                            break;
                        case 3:
                            //int orderstatus = (int)OrderStatus.Approved;


                            string qry = "SELECT Om.OrderType as ItemType, OD.* FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId + " AND OD.ReceivedOn >= '" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' AND OD.ReceivedOn <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";

                            lstOrderDetails = (from pm in context.ExecuteStoreQuery<OrderDetailsDTO>(qry)
                                               select pm).ToList();

                            if (lstOrderDetails.Any())
                            {
                                OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                                ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                            }
                            FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                            //lstOrderDetails = (from od in context.OrderDetails
                            //                   join om in context.OrderMasters on od.OrderGUID equals om.GUID
                            //                   where om.OrderType == (int)OrderType.Order && OrderStatuses.Contains(om.OrderStatus) && om.IsDeleted == false && od.Room == RoomId && od.CompanyID == CompanyId && FromDate <= od.LastUpdated && Todate >= od.LastUpdated && (od.IsDeleted ?? false) == false
                            //                   select od);

                            //lstReturnOrderDetails = (from od in context.OrderDetails
                            //                         join om in context.OrderMasters on od.OrderGUID equals om.GUID
                            //                         where om.OrderType == (int)OrderType.RuturnOrder && OrderStatuses.Contains(om.OrderStatus) && om.IsDeleted == false && od.Room == RoomId && od.CompanyID == CompanyId && FromDate <= od.LastUpdated && Todate >= od.LastUpdated && (od.IsDeleted ?? false) == false
                            //                         select od);
                            //if (lstOrderDetails.Any())
                            //{
                            //    OrderedQty = lstOrderDetails.Select(t => ((t.ApprovedQuantity ?? t.RequestedQuantity) ?? 0)).Sum();
                            //}

                            //if (lstReturnOrderDetails.Any())
                            //{
                            //    ReturnOrderedQty = lstReturnOrderDetails.Select(t => ((t.ApprovedQuantity ?? t.RequestedQuantity) ?? 0)).Sum();
                            //}
                            //FinalOrderedQty = OrderedQty - ReturnOrderedQty;
                            //F/((Min+Max)/2) * 365/Period
                            if (Avgminmax > 0 && FinalOrderedQty > 0)
                            {
                                StockRoomTurns = ((FinalOrderedQty) / Avgminmax) * DurationDiviser;
                            }
                            else
                            {
                                StockRoomTurns = 0;
                            }
                            break;
                        default:
                            break;
                    }

                }
                else
                {
                    StockRoomTurns = 0;
                }

                string qryMndSt = "select count(ID) as StockOuts from ItemStockOutHistory where RoomId = " + RoomId + " and CompanyId = " + CompanyId + " and StockOutDate >= '" + MonthstartDate.ToString("yyyy-MM-dd") + "' and StockOutDate <= '" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "'";
                string qryYndSt = "select count(ID) as StockOuts from ItemStockOutHistory where RoomId = " + RoomId + " and CompanyId = " + CompanyId + " and StockOutDate >= '" + YearstartDate.ToString("yyyy-MM-dd") + "' and StockOutDate <= '" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "'";


                MTDStockouts = context.ExecuteStoreQuery<int>(qryMndSt).First();
                ItemsStockOutCount = Convert.ToInt64(MTDStockouts);
                YTDStockouts = context.ExecuteStoreQuery<int>(qryYndSt).First();
                //IQueryable<ItemStockOutHistory> qry1 = (from ist in context.ItemStockOutHistories
                //                                        where EntityFunctions.TruncateTime(ist.StockOutDate) >= EntityFunctions.TruncateTime(YearstartDate) && EntityFunctions.TruncateTime(ist.StockOutDate) <= EntityFunctions.TruncateTime(DateTime.Now)
                //                                        select ist);

                //IQueryable<ItemStockOutHistory> qry2 = (from ist in context.ItemStockOutHistories
                //                                        where EntityFunctions.TruncateTime(ist.StockOutDate) >= EntityFunctions.TruncateTime(MonthstartDate) && EntityFunctions.TruncateTime(ist.StockOutDate) <= EntityFunctions.TruncateTime(DateTime.Now)
                //                                        select ist);
                //if (qry1.Any())
                //{
                //    YTDStockouts = qry1.Count();
                //}
                //if (qry2.Any())
                //{
                //    MTDStockouts = qry2.Count();
                //}
            }
            //retData.Add(1, InventoryValue);
            //retData.Add(2, StockRoomTurns);

            retData.Add(1, 0);
            retData.Add(2, 0);

            retData.Add(3, 0);
            retData.Add(4, 0);

            //retData.Add(3, MTDStockouts);
            //retData.Add(4, YTDStockouts);
            retData.Add(5, 0);
            retData.Add(6, 0);
            retData.Add(7, 0);
            retData.Add(8, 0);

            retData.Add(9, ItemsCount);
            retData.Add(10, ItemsLocationCount);
            retData.Add(11, ItemsStockOutCount);
            retData.Add(12, ItemsWithOutCostCount);
            retData.Add(13, ItemsTuningUpdateSettingCount);
            retData.Add(14, AnnualCarryingCostPercent);
            return retData;
        }

        public bool CalcMonthendRoomTurns(long RoomId, long CompanyId, int ForMonth, int ForYear, out string ExceptionString, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null)
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
                        Avgminmax = qryitems.Sum(t => ((t.MinimumQuantity) + (t.MaximumQuantity) / 2));
                    }







                    List<PullMaster> PullMasterList = null;
                    List<OrderDetailsDTO> lstOrderDetails = null;


                    PullMasterList = (from pm in context.ExecuteStoreQuery<PullMaster>("Select * from PullMaster as PM where  pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
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

                    lstOrderDetails = (from pm in context.ExecuteStoreQuery<OrderDetailsDTO>(qry)
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
                        context.InventoryAnalysisMonthWises.AddObject(objInventoryAnalysisMonthWise);
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

        public ItemTransationInfo GetItemTxnHistory(string txnType, long RoomId, long CompanyId, long ItemId, Guid ItemGuid)
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
                                        qry = "Select pm.GUID,isnull(pm.ItemGUID, newid()) as ItemGUID,convert(float,0) as TxnClosingQty,pm.receivedon as TxnDate,pm.ID as TxnID,'pull value' as TxnModuleItemName,'' as TxnNumber,isnull(pm.PoolQuantity, 0) as TxnQty,pm.ActionType as TxnType,isnull(pm.PULLCost, 0) as TxnValue from PullMaster as pm Where pm.itemGUID = '" + ItemGuid.ToString() + "' and pm.ActionType in ('pull','credit') and isnull(pm.isdeleted, 0)= 0 and convert(date,pm.receivedon) >= '" + (FromDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(date,pm.receivedon) <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                                        objItemTransationInfo.AUMeasureMethod = "Pull Value";
                                        objItemTransationInfo.TxtHistory = context.ExecuteStoreQuery<ItemTransactionDTO>(qry).ToList();
                                        //objItemTransationInfo.TxtHistory = (from pm in context.PullMasters
                                        //                                    where pm.ItemGUID == ItemGuid && EntityFunctions.TruncateTime(FromDate) <= EntityFunctions.TruncateTime(pm.Created) && EntityFunctions.TruncateTime(Todate) >= EntityFunctions.TruncateTime(pm.Created) && (pm.IsDeleted ?? false) == false && (pm.ActionType == "pull" || pm.ActionType == "credit")
                                        //                                    select new ItemTransactionDTO
                                        //                                    {
                                        //                                        GUID = pm.GUID,
                                        //                                        ItemGUID = pm.ItemGUID ?? Guid.Empty,
                                        //                                        TxnClosingQty = 0,
                                        //                                        TxnDate = pm.Created ?? DateTime.MinValue,
                                        //                                        TxnID = pm.ID,
                                        //                                        TxnModuleItemName = "pull value",
                                        //                                        TxnNumber = string.Empty,
                                        //                                        TxnQty = pm.PoolQuantity ?? 0,
                                        //                                        TxnType = pm.ActionType,
                                        //                                        TxnValue = pm.PULLCost ?? 0
                                        //                                    }).ToList(); 

                                        break;
                                    case 2:
                                        qry = "Select pm.GUID,isnull(pm.ItemGUID, newid()) as ItemGUID,convert(float,0) as TxnClosingQty,pm.receivedon as TxnDate,pm.ID as TxnID,'pull value' as TxnModuleItemName,'' as TxnNumber,isnull(pm.PoolQuantity, 0) as TxnQty,pm.ActionType as TxnType,isnull(pm.PULLCost, 0) as TxnValue from PullMaster as pm Where pm.itemGUID = '" + ItemGuid.ToString() + "' and pm.ActionType in ('pull','credit') and isnull(pm.isdeleted, 0)= 0 and convert(date,pm.receivedon) >= '" + (FromDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(date,pm.receivedon) <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                                        objItemTransationInfo.AUMeasureMethod = "Pull";
                                        objItemTransationInfo.TxtHistory = context.ExecuteStoreQuery<ItemTransactionDTO>(qry).ToList();
                                        //objItemTransationInfo.TxtHistory = (from pm in context.PullMasters
                                        //                                    where pm.ItemGUID == ItemGuid && EntityFunctions.TruncateTime(FromDate) <= EntityFunctions.TruncateTime(pm.Created) && EntityFunctions.TruncateTime(Todate) >= EntityFunctions.TruncateTime(pm.Created) && (pm.IsDeleted ?? false) == false && (pm.ActionType == "pull" || pm.ActionType == "credit")
                                        //                                    select new ItemTransactionDTO
                                        //                                    {
                                        //                                        GUID = pm.GUID,
                                        //                                        ItemGUID = pm.ItemGUID ?? Guid.Empty,
                                        //                                        TxnClosingQty = 0,
                                        //                                        TxnDate = pm.Created ?? DateTime.MinValue,
                                        //                                        TxnID = pm.ID,
                                        //                                        TxnModuleItemName = "pull value",
                                        //                                        TxnNumber = string.Empty,
                                        //                                        TxnQty = pm.PoolQuantity ?? 0,
                                        //                                        TxnType = pm.ActionType,
                                        //                                        TxnValue = pm.PULLCost ?? 0
                                        //                                    }).ToList(); ;

                                        break;
                                    case 3:
                                        qry = "SELECT OD.GUID,isnull(OD.ItemGUID,newid()) as ItemGUID, convert(float,0) as TxnClosingQty,Od.ReceivedOn as TxnDate,od.ID as TxnID,OM.OrderNumber as TxnModuleItemName,'' as TxnNumber, case when OM.OrderType=1 then isnull(od.ApprovedQuantity,0) when OM.OrderType=1 then (isnull(od.ApprovedQuantity,0)*(-1)) ELSE convert(float,0) END as TxnQty,case when OM.OrderType=1 then 'order' when OM.OrderType=2 then 'orderreturn' ELSE '' END as TxnType,convert(float,0) as TxnValue  FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE OD.ItemGUID = '" + ItemGuid + "' AND Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId + " AND OD.ReceivedOn >= '" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' AND OD.ReceivedOn <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";

                                        objItemTransationInfo.AUMeasureMethod = "Orders";
                                        objItemTransationInfo.TxtHistory = context.ExecuteStoreQuery<ItemTransactionDTO>(qry).ToList();
                                        //objItemTransationInfo.TxtHistory = (from od in context.OrderDetails
                                        //                                    join om in context.OrderMasters on od.OrderGUID equals om.GUID
                                        //                                    where om.OrderType == (int)OrderType.Order && OrderStatuses.Contains(om.OrderStatus) && om.IsDeleted == false && od.ItemGUID == ItemGuid && EntityFunctions.TruncateTime(FromDate) <= EntityFunctions.TruncateTime(od.LastUpdated) && EntityFunctions.TruncateTime(Todate) >= EntityFunctions.TruncateTime(od.LastUpdated) && (od.IsDeleted ?? false) == false
                                        //                                    select new ItemTransactionDTO
                                        //                                    {
                                        //                                        GUID = od.GUID,
                                        //                                        ItemGUID = od.ItemGUID ?? Guid.Empty,
                                        //                                        TxnClosingQty = 0,
                                        //                                        TxnDate = od.LastUpdated ?? DateTime.MinValue,
                                        //                                        TxnID = od.ID,
                                        //                                        TxnModuleItemName = om.OrderNumber,
                                        //                                        TxnNumber = string.Empty,
                                        //                                        TxnQty = od.ApprovedQuantity ?? 0,
                                        //                                        TxnType = "order",
                                        //                                        TxnValue = 0
                                        //                                    }).Union
                                        //                                    (from od in context.OrderDetails
                                        //                                     join om in context.OrderMasters on od.OrderGUID equals om.GUID
                                        //                                     where om.OrderType == (int)OrderType.RuturnOrder && OrderStatuses.Contains(om.OrderStatus) && om.IsDeleted == false && od.ItemGUID == ItemGuid && EntityFunctions.TruncateTime(FromDate) <= EntityFunctions.TruncateTime(od.LastUpdated) && EntityFunctions.TruncateTime(Todate) >= EntityFunctions.TruncateTime(od.LastUpdated) && (od.IsDeleted ?? false) == false
                                        //                                     select new ItemTransactionDTO
                                        //                                     {
                                        //                                         GUID = od.GUID,
                                        //                                         ItemGUID = od.ItemGUID ?? Guid.Empty,
                                        //                                         TxnClosingQty = 0,
                                        //                                         TxnDate = od.LastUpdated ?? DateTime.MinValue,
                                        //                                         TxnID = od.ID,
                                        //                                         TxnModuleItemName = om.OrderNumber,
                                        //                                         TxnNumber = string.Empty,
                                        //                                         TxnQty = (od.ApprovedQuantity ?? 0) * (-1),
                                        //                                         TxnType = "orderreturn",
                                        //                                         TxnValue = 0
                                        //                                     }).ToList();

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
                                        qry = "Select pm.GUID,isnull(pm.ItemGUID, newid()) as ItemGUID,convert(float,0) as TxnClosingQty,pm.receivedon as TxnDate,pm.ID as TxnID,'pull value' as TxnModuleItemName,'' as TxnNumber,isnull(pm.PoolQuantity, 0) as TxnQty,pm.ActionType as TxnType,isnull(pm.PULLCost, 0) as TxnValue from PullMaster as pm Where pm.itemGUID = '" + ItemGuid.ToString() + "' and pm.ActionType in ('pull','credit') and isnull(pm.isdeleted, 0)= 0 and convert(date,pm.receivedon) >= '" + (FromDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(date,pm.receivedon) <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                                        objItemTransationInfo.AUMeasureMethod = "Pull Value";
                                        objItemTransationInfo.TxtHistory = context.ExecuteStoreQuery<ItemTransactionDTO>(qry).ToList();
                                        //objItemTransationInfo.TxtHistory = (from pm in context.PullMasters
                                        //                                    where pm.ItemGUID == ItemGuid && EntityFunctions.TruncateTime(FromDate) <= EntityFunctions.TruncateTime(pm.Created) && EntityFunctions.TruncateTime(Todate) >= EntityFunctions.TruncateTime(pm.Created) && (pm.IsDeleted ?? false) == false && (pm.ActionType == "pull" || pm.ActionType == "credit")
                                        //                                    select new ItemTransactionDTO
                                        //                                    {
                                        //                                        GUID = pm.GUID,
                                        //                                        ItemGUID = pm.ItemGUID ?? Guid.Empty,
                                        //                                        TxnClosingQty = 0,
                                        //                                        TxnDate = pm.Created ?? DateTime.MinValue,
                                        //                                        TxnID = pm.ID,
                                        //                                        TxnModuleItemName = "pull value",
                                        //                                        TxnNumber = string.Empty,
                                        //                                        TxnQty = pm.PoolQuantity ?? 0,
                                        //                                        TxnType = pm.ActionType,
                                        //                                        TxnValue = pm.PULLCost ?? 0
                                        //                                    }).ToList();


                                        break;
                                    case 2:
                                        qry = "Select pm.GUID,isnull(pm.ItemGUID, newid()) as ItemGUID,convert(float,0) as TxnClosingQty,pm.receivedon as TxnDate,pm.ID as TxnID,'pull value' as TxnModuleItemName,'' as TxnNumber,isnull(pm.PoolQuantity, 0) as TxnQty,pm.ActionType as TxnType,isnull(pm.PULLCost, 0) as TxnValue from PullMaster as pm Where pm.itemGUID = '" + ItemGuid.ToString() + "' and pm.ActionType in ('pull','credit') and isnull(pm.isdeleted, 0)= 0 and convert(date,pm.receivedon) >= '" + (FromDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(date,pm.receivedon) <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                                        objItemTransationInfo.AUMeasureMethod = "Pull";
                                        objItemTransationInfo.TxtHistory = context.ExecuteStoreQuery<ItemTransactionDTO>(qry).ToList();
                                        //objItemTransationInfo.TxtHistory = (from pm in context.PullMasters
                                        //                                    where pm.ItemGUID == ItemGuid && EntityFunctions.TruncateTime(FromDate) <= EntityFunctions.TruncateTime(pm.Created) && EntityFunctions.TruncateTime(Todate) >= EntityFunctions.TruncateTime(pm.Created) && (pm.IsDeleted ?? false) == false && (pm.ActionType == "pull" || pm.ActionType == "credit")
                                        //                                    select new ItemTransactionDTO
                                        //                                    {
                                        //                                        GUID = pm.GUID,
                                        //                                        ItemGUID = pm.ItemGUID ?? Guid.Empty,
                                        //                                        TxnClosingQty = 0,
                                        //                                        TxnDate = pm.Created ?? DateTime.MinValue,
                                        //                                        TxnID = pm.ID,
                                        //                                        TxnModuleItemName = "pull",
                                        //                                        TxnNumber = string.Empty,
                                        //                                        TxnQty = pm.PoolQuantity ?? 0,
                                        //                                        TxnType = pm.ActionType,
                                        //                                        TxnValue = pm.PULLCost ?? 0
                                        //                                    }).ToList(); ;

                                        break;
                                    case 3:
                                        qry = "SELECT OD.GUID,isnull(OD.ItemGUID,newid()) as ItemGUID,convert(float,0) as TxnClosingQty,Od.ReceivedOn as TxnDate,od.ID as TxnID,OM.OrderNumber as TxnModuleItemName,'' as TxnNumber, case when OM.OrderType=1 then isnull(od.ApprovedQuantity,0) when OM.OrderType=1 then (isnull(od.ApprovedQuantity,0)*(-1)) ELSE convert(float,0) END as TxnQty,case when OM.OrderType=1 then 'order' when OM.OrderType=2 then 'orderreturn' ELSE '' END as TxnType,convert(float,0) as TxnValue  FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE OD.ItemGUID = '" + ItemGuid + "' AND Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId + " AND OD.ReceivedOn >= '" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' AND OD.ReceivedOn <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";
                                        objItemTransationInfo.AUMeasureMethod = "Orders";
                                        objItemTransationInfo.TxtHistory = context.ExecuteStoreQuery<ItemTransactionDTO>(qry).ToList();
                                        //objItemTransationInfo.TxtHistory = (from od in context.OrderDetails
                                        //                                    join om in context.OrderMasters on od.OrderGUID equals om.GUID
                                        //                                    where om.OrderType == (int)OrderType.Order && OrderStatuses.Contains(om.OrderStatus) && om.IsDeleted == false && od.ItemGUID == ItemGuid && EntityFunctions.TruncateTime(FromDate) <= EntityFunctions.TruncateTime(od.LastUpdated) && EntityFunctions.TruncateTime(Todate) >= EntityFunctions.TruncateTime(od.LastUpdated) && (od.IsDeleted ?? false) == false
                                        //                                    select new ItemTransactionDTO
                                        //                                    {
                                        //                                        GUID = od.GUID,
                                        //                                        ItemGUID = od.ItemGUID ?? Guid.Empty,
                                        //                                        TxnClosingQty = 0,
                                        //                                        TxnDate = od.LastUpdated ?? DateTime.MinValue,
                                        //                                        TxnID = od.ID,
                                        //                                        TxnModuleItemName = om.OrderNumber,
                                        //                                        TxnNumber = string.Empty,
                                        //                                        TxnQty = od.ApprovedQuantity ?? 0,
                                        //                                        TxnType = "order",
                                        //                                        TxnValue = 0
                                        //                                    }).Union
                                        //                                    (from od in context.OrderDetails
                                        //                                     join om in context.OrderMasters on od.OrderGUID equals om.GUID
                                        //                                     where om.OrderType == (int)OrderType.RuturnOrder && OrderStatuses.Contains(om.OrderStatus) && om.IsDeleted == false && od.ItemGUID == ItemGuid && EntityFunctions.TruncateTime(FromDate) <= EntityFunctions.TruncateTime(od.LastUpdated) && EntityFunctions.TruncateTime(Todate) >= EntityFunctions.TruncateTime(od.LastUpdated) && (od.IsDeleted ?? false) == false
                                        //                                     select new ItemTransactionDTO
                                        //                                     {
                                        //                                         GUID = od.GUID,
                                        //                                         ItemGUID = od.ItemGUID ?? Guid.Empty,
                                        //                                         TxnClosingQty = 0,
                                        //                                         TxnDate = od.LastUpdated ?? DateTime.MinValue,
                                        //                                         TxnID = od.ID,
                                        //                                         TxnModuleItemName = om.OrderNumber,
                                        //                                         TxnNumber = string.Empty,
                                        //                                         TxnQty = (od.ApprovedQuantity ?? 0) * (-1),
                                        //                                         TxnType = "orderreturn",
                                        //                                         TxnValue = 0
                                        //                                     }).ToList(); ;

                                        break;
                                }
                            }
                            break;
                    }
                }
                if (objItemTransationInfo != null && objItemTransationInfo.TxtHistory != null && objItemTransationInfo.TxtHistory.Count() > 0)
                {
                    if (objItemTransationInfo.AUMeasureMethod == "Pull" || objItemTransationInfo.AUMeasureMethod == "Pull Value")
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

                        PullMasterList = (from pm in context.ExecuteStoreQuery<PullMaster>("Select * from PullMaster as PM where PM.ItemGUID='" + ItemGUID.ToString() + "' and pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
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

                        lstOrderDetails = (from pm in context.ExecuteStoreQuery<OrderDetailsDTO>(qry)
                                           select pm).ToList();
                        if (lstOrderDetails.Any())
                        {
                            OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                            ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                        }
                        FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                        if (PullCost > 0)
                        {
                            ItemInventoryValue = objItem.ExtendedCost ?? 0;
                            if (ItemInventoryValue > 0)
                            {
                                objDashboardAnalysisInfo.MonthizedPullValueTurns = ((PullCost) / (ItemInventoryValue)) * (DurationDiviser);
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

                        if (PullQuantity > 0 && (objItem.OnHandQuantity ?? 0) > 0)
                        {
                            objDashboardAnalysisInfo.MonthizedPullTurns = (PullQuantity / objItem.OnHandQuantity ?? 1) * DurationDiviser;
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



                        PullMasterList = (from pm in context.ExecuteStoreQuery<PullMaster>("Select * from PullMaster as PM where PM.ItemGUID='" + ItemGUID.ToString() + "' and pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
                                          select pm).ToList();


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

                        string qry = "SELECT Om.OrderType as ItemType, OD.* FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE OD.ItemGUID = '" + objItem.GUID + "' AND Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId + " AND OD.ReceivedOn >= '" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' AND OD.ReceivedOn <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";

                        lstOrderDetails = (from pm in context.ExecuteStoreQuery<OrderDetailsDTO>(qry)
                                           select pm).ToList();


                        if (lstOrderDetails.Any())
                        {
                            OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                            ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                        }

                        FinalOrderedQty = OrderedQty - ReturnOrderedQty;

                        if (PullCost > 0)
                        {
                            objDashboardAnalysisInfo.CalculatedPullValueAverageUsage = PullCost / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
                        }
                        else
                        {
                            objDashboardAnalysisInfo.CalculatedPullValueAverageUsage = 0;
                        }

                        if (PullQuantity > 0)
                        {
                            objDashboardAnalysisInfo.CalculatedPullAverageUsage = PullQuantity / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
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

                    PullMasterList = (from pm in context.ExecuteStoreQuery<PullMaster>("Select * from PullMaster as PM where pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
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
                    string qry = "SELECT Om.OrderType as ItemType, OD.* FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId + " AND OD.ReceivedOn >= '" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' AND OD.ReceivedOn <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";

                    lstOrderDetails = (from pm in context.ExecuteStoreQuery<OrderDetailsDTO>(qry)
                                       select pm).ToList();
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

                    if (PullCost > 0)
                    {
                        if (ItemInventoryValue > 0)
                        {
                            objDashboardAnalysisInfo.MonthizedPullValueTurns = ((PullCost) / (ItemInventoryValue)) * (DurationDiviser);
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

                    if (PullQuantity > 0 && OnhandQty > 0)
                    {
                        objDashboardAnalysisInfo.MonthizedPullTurns = (PullQuantity / OnhandQty) * DurationDiviser;
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

        public DashboardAnalysisInfo UpdateTurnsByItemGUIDAfterTxn(long RoomId, long CompanyId, Guid ItemGUID, long UserId, DashboardParameterDTO objDashboardParameterDTO = null, eTurnsRegionInfo objRegionalSettings = null)
        {
            DashboardAnalysisInfo objDashboardAnalysisInfo = new DashboardAnalysisInfo();
            DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            string CurrentRoomTimeZone = "UTC";
            if (objDashboardParameterDTO == null)
            {
                //if (HttpContext.Current != null && HttpContext.Current.Session != null
                //     && HttpContext.Current.Session["DashboardParams"] != null)
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
                        DateTime? FromDate = CurrentTimeofTimeZone.AddDays(1).AddDays((objDashboardParameterDTO.TurnsDaysOfUsageToSample.Value) * (-1)).Date;
                        DateTime Todate = new DateTime(CurrentTimeofTimeZone.Year, CurrentTimeofTimeZone.Month, CurrentTimeofTimeZone.Day, 23, 59, 59);
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
                                PullMasterList = (from pm in context.ExecuteStoreQuery<PullMaster>("Select * from PullMaster as PM where PM.ItemGUID='" + ItemGUID.ToString() + "' and pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
                                                  select pm).ToList();



                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                                }
                                if (PullCost > 0)
                                {
                                    //lstItemLocations = (from ilq in context.ItemLocationDetails
                                    //                    where ilq.ItemGUID == ItemGUID && (ilq.IsDeleted ?? false) == false
                                    //                    select ilq);

                                    //if (lstItemLocations.Any())
                                    //{
                                    //    ItemInventoryValue = lstItemLocations.Select(t => ((t.ConsignedQuantity ?? 0) + (t.CustomerOwnedQuantity ?? 0)) * (t.Cost ?? 0)).Sum();
                                    //}
                                    ItemInventoryValue = AverageExtendedCost;
                                    if (ItemInventoryValue > 0)
                                    {
                                        objDashboardAnalysisInfo.CalculatedTurn = ((PullCost) / (ItemInventoryValue)) * (DurationDiviser);
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
                                PullMasterList = (from pm in context.ExecuteStoreQuery<PullMaster>("Select * from PullMaster as PM where PM.ItemGUID='" + ItemGUID.ToString() + "' and pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
                                                  select pm).ToList();

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                                }
                                //F/((Min+Max)/2) * 365/Period
                                if (PullQuantity > 0 && AverageOnHandQuantity > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = (PullQuantity / AverageOnHandQuantity) * DurationDiviser;
                                    objItem.Turns = objDashboardAnalysisInfo.CalculatedTurn;
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = 0;
                                    objItem.Turns = 0;
                                }
                                break;
                            case 3:
                                //int orderstatus = (int)OrderStatus.Approved;


                                //lstOrderDetails = (from od in context.OrderDetails
                                //                   join om in context.OrderMasters on od.OrderGUID equals om.GUID
                                //                   where om.OrderType == (int)OrderType.Order && OrderStatuses.Contains(om.OrderStatus) && om.IsDeleted == false && od.ItemGUID == ItemGUID && FromDate <= od.LastUpdated && Todate >= od.LastUpdated && (od.IsDeleted ?? false) == false
                                //                   select od);

                                //lstReturnOrderDetails = (from od in context.OrderDetails
                                //                         join om in context.OrderMasters on od.OrderGUID equals om.GUID
                                //                         where om.OrderType == (int)OrderType.RuturnOrder && OrderStatuses.Contains(om.OrderStatus) && om.IsDeleted == false && od.ItemGUID == ItemGUID && FromDate <= od.LastUpdated && Todate >= od.LastUpdated && (od.IsDeleted ?? false) == false
                                //                         select od);

                                string qry = "SELECT Om.OrderType as ItemType, OD.* FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE OD.ItemGUID = '"
                                    + objItem.GUID + "' AND Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId
                                    + " AND OD.ReceivedOn >= '"
                                    + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH':'mm':'ss")
                                    + "' AND OD.ReceivedOn <= '"
                                    + (Todate).ToString("yyyy-MM-dd HH':'mm':'ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";

                                lstOrderDetails = (from pm in context.ExecuteStoreQuery<OrderDetailsDTO>(qry)
                                                   select pm).ToList();


                                if (lstOrderDetails.Any())
                                {
                                    OrderedQty = lstOrderDetails.Where(t => t.ItemType == 1).Sum(t => (t.ApprovedQuantity ?? 0));
                                    ReturnOrderedQty = lstOrderDetails.Where(t => t.ItemType == 2).Sum(t => (t.ApprovedQuantity ?? 0));
                                }


                                //if (lstOrderDetails.Any())
                                //{
                                //    OrderedQty = lstOrderDetails.Select(t => ((t.ApprovedQuantity ?? t.RequestedQuantity) ?? 0)).Sum();
                                //}

                                //if (lstReturnOrderDetails.Any())
                                //{
                                //    ReturnOrderedQty = lstReturnOrderDetails.Select(t => ((t.ApprovedQuantity ?? t.RequestedQuantity) ?? 0)).Sum();
                                //}

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
                //if (HttpContext.Current != null && HttpContext.Current.Session["DashboardParams"] != null)
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
                //if (HttpContext.Current != null && HttpContext.Current.Session["eTurnsRegionInfoProp"] != null)
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
                                PullMasterList = (from pm in context.ExecuteStoreQuery<PullMaster>("Select * from PullMaster as PM where PM.ItemGUID='" + ItemGUID.ToString() + "' and pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
                                                  select pm).ToList();



                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullCost = PullCost - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum());
                                }
                                if (PullCost > 0)
                                {

                                    ItemInventoryValue = AverageExtendedCost;
                                    if (ItemInventoryValue > 0)
                                    {
                                        objDashboardAnalysisInfo.CalculatedTurn = ((PullCost) / (ItemInventoryValue)) * (DurationDiviser);
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
                                PullMasterList = (from pm in context.ExecuteStoreQuery<PullMaster>("Select * from PullMaster as PM where PM.ItemGUID='" + ItemGUID.ToString() + "' and pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
                                                  select pm).ToList();

                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                                }
                                if (PullQuantity > 0 && AverageOnHandQuantity > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = (PullQuantity / AverageOnHandQuantity) * DurationDiviser;
                                    objItem.Turns = objDashboardAnalysisInfo.CalculatedTurn;
                                }
                                else
                                {
                                    objDashboardAnalysisInfo.CalculatedTurn = 0;
                                    objItem.Turns = 0;
                                }
                                break;
                            case 3:
                                string qry = "SELECT Om.OrderType as ItemType, OD.* FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE OD.ItemGUID = '" + objItem.GUID + "' AND Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId + " AND OD.ReceivedOn >= '" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' AND OD.ReceivedOn <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";

                                lstOrderDetails = (from pm in context.ExecuteStoreQuery<OrderDetailsDTO>(qry)
                                                   select pm).ToList();


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
                //if (HttpContext.Current != null && HttpContext.Current.Session["DashboardParams"] != null)
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
                //if (HttpContext.Current != null && HttpContext.Current.Session["eTurnsRegionInfoProp"] != null)
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
                    ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                    if (objItem != null)
                    {
                        //DateTime? FromDate = CurrentTimeofTimeZone.AddDays(1).AddDays(objDashboardParameterDTO.AUDayOfUsageToSample.Value * (-1)).Date;
                        //DateTime Todate = new DateTime(CurrentTimeofTimeZone.Year, CurrentTimeofTimeZone.Month, CurrentTimeofTimeZone.Day, 23, 59, 59);
                        //FromDate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, FromDate ?? DateTime.UtcNow);
                        //Todate = DateTimeUtility.ConvertDateToUTC(CurrentRoomTimeZone, Todate);

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
                                PullMasterList = (from pm in context.ExecuteStoreQuery<PullMaster>("Select * from PullMaster as PM where PM.ItemGUID='" + ItemGUID.ToString() + "' and pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
                                                  select pm).ToList();


                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullCost = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PULLCost ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullCost = PullCost - PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PULLCost ?? 0)).Sum();
                                }
                                if (PullCost > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = PullCost / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
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
                                PullMasterList = (from pm in context.ExecuteStoreQuery<PullMaster>("Select * from PullMaster as PM where PM.ItemGUID='" + ItemGUID.ToString() + "' and pm.Room=" + RoomId + " AND PM.CompanyID=" + CompanyId + " and pm.ActionType in ('credit','pull') and pm.isdeleted<>1 and pm.ReceivedOnWeb>='" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' and pm.ReceivedOnWeb<='" + Todate.ToString("yyyy-MM-dd HH:mm:ss") + "'")
                                                  select pm).ToList();


                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Any())
                                {
                                    PullQuantity = PullMasterList.Where(t => t.ActionType.ToLower() == "pull").Select(t => (t.PoolQuantity ?? 0)).Sum();
                                }
                                if (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Any())
                                {
                                    PullQuantity = PullQuantity - (PullMasterList.Where(t => t.ActionType.ToLower() == "credit").Select(t => (t.PoolQuantity ?? 0)).Sum());
                                }
                                //F/((Min+Max)/2) * 365/Period
                                if (PullQuantity > 0)
                                {
                                    objDashboardAnalysisInfo.CalculatedAverageUsage = PullQuantity / objDashboardParameterDTO.AUDayOfUsageToSample.Value;
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
                                //int orderstatus = (int)OrderStatus.Approved;


                                string qry = "SELECT Om.OrderType as ItemType, OD.* FROM OrderDetails AS OD INNER JOIN OrderMaster AS OM ON OD.OrderGUID = OM.[GUID] WHERE OD.ItemGUID = '" + objItem.GUID + "' AND Od.Room = " + RoomId + " AND OD.CompanyID = " + CompanyId + " AND OD.ReceivedOn >= '" + (FromDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' AND OD.ReceivedOn <= '" + (Todate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND isnull(Om.IsDeleted, 0) = 0 AND ISNULL(od.IsDeleted, 0) = 0 and OM.OrderStatus in (3,4,5,6,7,8);";

                                lstOrderDetails = (from pm in context.ExecuteStoreQuery<OrderDetailsDTO>(qry)
                                                   select pm).ToList();


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
    }
}
