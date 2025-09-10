using eTurns.DTO;
using eTurnsWeb.BAL;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eTurnsWeb.Models
{
    public class RoomViewModel : RoomDTO
    {
       
        public RoomViewModel(long enterpriseID)
        {
            
        }

        public RoomViewModel(RoomDTO obj)
        {
            if (obj == null)
            {
                return;
            }
            

            this.ID = obj.ID;
            this.GUID = obj.GUID;
            this.RoomName = obj.RoomName;
            this.CompanyName = obj.CompanyName;
            this.ContactName = obj.ContactName;
            this.streetaddress = obj.streetaddress;

            this.City = obj.City;

            this.State = obj.State;

            this.PostalCode = obj.PostalCode;

            this.Country = obj.Country;
            this.Email = obj.Email;
            this.PhoneNo = obj.PhoneNo;
            this.InvoiceBranch = obj.InvoiceBranch;

            this.CustomerNumber = obj.CustomerNumber;

            this.BillingRoomType = obj.BillingRoomType;

            this.BlanketPO = obj.BlanketPO;

            this.IsConsignment = obj.IsConsignment;

            this.SuggestedOrder = obj.SuggestedOrder;
            this.SuggestedTransfer = obj.SuggestedTransfer;
            this.ReplineshmentRoom = obj.ReplineshmentRoom;
            this.ReplenishmentType = obj.ReplenishmentType;

            this.IseVMI = obj.IseVMI;

            this.MaxOrderSize = obj.MaxOrderSize;

            this.HighPO = obj.HighPO;
            this.HighJob = obj.HighJob;
            this.HighTransfer = obj.HighTransfer;
            this.HighCount = obj.HighCount;
            this.GlobMarkupParts = obj.GlobMarkupParts;
            this.GlobMarkupLabor = obj.GlobMarkupLabor;
            this.IsTax1Parts = obj.IsTax1Parts;
            this.IsTax1Labor = obj.IsTax1Labor;
            this.Tax1name = obj.Tax1name;
            this.Tax1Rate = obj.Tax1Rate;

            this.IsTax2Parts = obj.IsTax2Parts;
            this.IsTax2Labor = obj.IsTax2Labor;
            this.tax2name = obj.tax2name;
            this.Tax2Rate = obj.Tax2Rate;
            this.IsTax2onTax1 = obj.IsTax2onTax1;
            this.IsTrending = obj.IsTrending;


            this.TrendingFormula = obj.TrendingFormula;
            this.TrendingFormulaType = obj.TrendingFormulaType;
            this.TrendingFormulaDays = obj.TrendingFormulaDays;
            this.TrendingFormulaOverDays = obj.TrendingFormulaOverDays;
            this.TrendingFormulaAvgDays = obj.TrendingFormulaAvgDays;
            this.TrendingFormulaCounts = obj.TrendingFormulaCounts;
            this.GXPRConsJob = obj.GXPRConsJob;
            this.CostCenter = obj.CostCenter;
            this.UniqueID = obj.UniqueID;


            this.Created = obj.Created;
            this.Updated = obj.Updated;
            this.ActiveOn = obj.ActiveOn;
            this.ActiveOnDateStr = obj.ActiveOnDateStr;

            this.CreatedBy = obj.CreatedBy;
            this.LastUpdatedBy = obj.LastUpdatedBy;
            this.IsDeleted = obj.IsDeleted;
            this.IsArchived = obj.IsArchived;
            this.CreatedByName = obj.CreatedByName;
            this.UpdatedByName = obj.UpdatedByName;

            this.MethodOfValuingInventory = obj.MethodOfValuingInventory;
            this.IsActive = obj.IsActive;
            this.LicenseBilled = obj.LicenseBilled;
            this.LicenseBilledStri = obj.LicenseBilledStri;
            this.NextCountNo = obj.NextCountNo;
            this.POAutoSequence = obj.POAutoSequence;
            this.CountAutoSequence = obj.CountAutoSequence;
            this.NextOrderNo = obj.NextOrderNo;
            this.NextRequisitionNo = obj.NextRequisitionNo;
            this.NextStagingNo = obj.NextStagingNo;
            this.NextTransferNo = obj.NextTransferNo;
            this.NextWorkOrderNo = obj.NextWorkOrderNo;
            this.RoomGrouping = obj.RoomGrouping;
            this.AutoCreateTransferFrequency = obj.AutoCreateTransferFrequency;
            this.AutoCreateTransferTime = obj.AutoCreateTransferTime;
            this.AutoCreateTransferSubmit = obj.AutoCreateTransferSubmit;
            this.TransferFrequencyOption = obj.TransferFrequencyOption;
            this.TransferFrequencyDays = obj.TransferFrequencyDays;
            this.TransferFrequencyMonth = obj.TransferFrequencyMonth;
            this.TransferFrequencyNumber = obj.TransferFrequencyNumber;
            this.TransferFrequencyWeek = obj.TransferFrequencyWeek;
            this.TransferFrequencyMainOption = obj.TransferFrequencyMainOption;
            this.TrendingSampleSize = obj.TrendingSampleSize;
            this.TrendingSampleSizeDivisor = obj.TrendingSampleSizeDivisor;
            this.CompanyID = obj.CompanyID;
            this.SourceOfTrending = obj.SourceOfTrending;
            this.AverageUsageTransactions = obj.AverageUsageTransactions;
            this.AverageUsageSampleSize = obj.AverageUsageSampleSize;
            this.AverageUsageSampleSizeDivisor = obj.AverageUsageSampleSizeDivisor;
            this.UDF1 = obj.UDF1;
            this.UDF2 = obj.UDF2;
            this.UDF3 = obj.UDF3;
            this.UDF4 = obj.UDF4;
            this.UDF5 = obj.UDF5;
            this.UDF6 = obj.UDF6;
            this.UDF7 = obj.UDF7;
            this.UDF8 = obj.UDF8;
            this.UDF9 = obj.UDF9;
            this.UDF10 = obj.UDF10;
            this.DefaultSupplierID = obj.DefaultSupplierID;
            this.DefaultSupplierName = obj.DefaultSupplierName;
            this.EnterpriseId = obj.EnterpriseId;
            this.EnterpriseName = obj.EnterpriseName;
            this.NextAssetNo = obj.NextAssetNo;
            this.NextBinNo = obj.NextBinNo;
            this.NextKitNo = obj.NextKitNo;
            this.NextItemNo = obj.NextItemNo;
            this.NextProjectSpendNo = obj.NextProjectSpendNo;
            this.NextToolNo = obj.NextToolNo;
            this.InventoryConsuptionMethod = obj.InventoryConsuptionMethod;
            this.DefaultBinID = obj.DefaultBinID;
            this.DefaultBinName = obj.DefaultBinName;
            this.DefaultLocationName = obj.DefaultLocationName;
            this.IsRoomActive = obj.IsRoomActive;
            this.IsRoomInactive = obj.IsRoomInactive;
            this.IsCompanyActive = obj.IsCompanyActive;
            this.IsProjectSpendMandatory = obj.IsProjectSpendMandatory;
            this.IsAverageUsageBasedOnPull = obj.IsAverageUsageBasedOnPull;
            this.SlowMovingValue = obj.SlowMovingValue;
            this.FastMovingValue = obj.FastMovingValue;
            this.ReplineshmentRoomName = obj.ReplineshmentRoomName;
            this.RoomActiveStatus = obj.RoomActiveStatus;
            this.ReclassifyAllItems = obj.ReclassifyAllItems;
            this.RoomNameChange = obj.RoomNameChange;
            this.SOSettingChanged = obj.SOSettingChanged;
            this.STSettingChanged = obj.STSettingChanged;
            this.SRSettingChanged = obj.SRSettingChanged;
            this.RequestedXDays = obj.RequestedXDays;
            this.RequestedYDays = obj.RequestedYDays;
            this.LastPullDate = obj.LastPullDate;
            this.LastOrderDate = obj.LastOrderDate;
            this.BaseOfInventory = obj.BaseOfInventory;
            this.eVMIWaitCommand = obj.eVMIWaitCommand;
            this.eVMIWaitPort = obj.eVMIWaitPort;
            this.ShelfLifeleadtimeOrdRpt = obj.ShelfLifeleadtimeOrdRpt;
            this.LeadTimeOrdRpt = obj.LeadTimeOrdRpt;
            this.MaintenanceDueNoticeDays = obj.MaintenanceDueNoticeDays;
            this.CreatedDate = obj.CreatedDate;

            this.UpdatedDate = obj.UpdatedDate;
            this.ActiveOnDate = obj.ActiveOnDate;
            this.Action = obj.Action;
            this.HistoryID = obj.HistoryID;
            this.PullPurchaseNumberType = obj.PullPurchaseNumberType;
            this.LastPullPurchaseNumberUsed = obj.LastPullPurchaseNumberUsed;

            this.ReqAutoSequence = obj.ReqAutoSequence;
            this.LastOrderDateStr = obj.LastOrderDateStr;
            this.LicenseBilledStr = obj.LicenseBilledStr;
            this.LastPullDateStr = obj.LastPullDateStr;

            this.LastSyncDateTime = obj.LastSyncDateTime;
            this.PDABuildVersion = obj.PDABuildVersion;
            this.LastSyncUserName = obj.LastSyncUserName;

            this.LastSyncDateTimeStr = obj.LastSyncDateTimeStr;
            this.IsAllowRequisitionDuplicate = obj.IsAllowRequisitionDuplicate;
            this.RoomId = obj.RoomId;
            this.ExtPhoneNo = obj.ExtPhoneNo;
            this.LastReceivedDate = obj.LastReceivedDate;
            this.LastTrasnferedDate = obj.LastTrasnferedDate;

            this.LastTrasnferedDateStr = obj.LastTrasnferedDateStr;
            this.LastReceivedDateStr = obj.LastReceivedDateStr;
            this.intTimeZoneOffSet = obj.intTimeZoneOffSet;
            this.TimeZoneName = obj.TimeZoneName;
            this.dtServiceRunTime = obj.dtServiceRunTime;
            this.IsServiceExecuted = obj.IsServiceExecuted;
            this.EnterpriseDBName = obj.EnterpriseDBName;
            this.CalculateDaily = obj.CalculateDaily;
            this.CalculateMonthly = obj.CalculateMonthly;
            this.AllowInsertingItemOnScan = obj.AllowInsertingItemOnScan;
            this.IsAllowOrderDuplicate = obj.IsAllowOrderDuplicate;
            this.IsAllowWorkOrdersDuplicate = obj.IsAllowWorkOrdersDuplicate;
            this.AllowPullBeyondAvailableQty = obj.AllowPullBeyondAvailableQty;
            this.PullRejectionType = obj.PullRejectionType;
            this.lstRoomModleSettings = obj.lstRoomModleSettings;
            this.CountAutoNrFixedValue = obj.CountAutoNrFixedValue;
            this.POAutoNrFixedValue = obj.POAutoNrFixedValue;
            this.PullPurchaseNrFixedValue = obj.PullPurchaseNrFixedValue;
            this.ReqAutoNrFixedValue = obj.ReqAutoNrFixedValue;
            this.StagingAutoSequence = obj.StagingAutoSequence;
            this.TransferAutoSequence = obj.TransferAutoSequence;
            this.WorkOrderAutoSequence = obj.WorkOrderAutoSequence;
            this.StagingAutoNrFixedValue = obj.StagingAutoNrFixedValue;
            this.TransferAutoNrFixedValue = obj.TransferAutoNrFixedValue;
            this.WorkOrderAutoNrFixedValue = obj.WorkOrderAutoNrFixedValue;
            this.WarnUserOnAssigningNonDefaultBin = obj.WarnUserOnAssigningNonDefaultBin;
            this.DefaultRequisitionRequiredDays = obj.DefaultRequisitionRequiredDays;
            this.AttachingWOWithRequisition = obj.AttachingWOWithRequisition;
            this.PreventMaxOrderQty = obj.PreventMaxOrderQty;
            this.DefaultCountType = obj.DefaultCountType;

            this.TAOAutoSequence = obj.TAOAutoSequence;
            this.TAOAutoNrFixedValue = obj.TAOAutoNrFixedValue;
            this.NextToolAssetOrderNo = obj.NextToolAssetOrderNo;

            this.AllowToolOrdering = obj.AllowToolOrdering;
            this.IsWOSignatureRequired = obj.IsWOSignatureRequired;
            this.IsIgnoreCreditRule = obj.IsIgnoreCreditRule;
            this.IsAllowOrderCostuom = obj.IsAllowOrderCostuom;
            this.ToolCountAutoSequence = obj.ToolCountAutoSequence;
            this.NextToolCountNo = obj.NextToolCountNo;
            this.ToolCountAutoNrFixedValue = obj.ToolCountAutoNrFixedValue;
            this.ForceSupplierFilter = obj.ForceSupplierFilter;
            this.SuggestedReturn = obj.SuggestedReturn;
            this.NoOfItems = obj.NoOfItems;
            this.ReportAppIntent = obj.ReportAppIntent;
            this.IsOrderReleaseNumberEditable = obj.IsOrderReleaseNumberEditable;
            this.QuoteAutoSequence = obj.QuoteAutoSequence;
            this.NextQuoteNo = obj.NextQuoteNo;
            this.IsAllowQuoteDuplicate = obj.IsAllowQuoteDuplicate;
            this.QuoteAutoNrFixedValue = obj.QuoteAutoNrFixedValue;
            this.DoGroupSupplierQuoteToOrder = obj.DoGroupSupplierQuoteToOrder;
            this.DoSendQuotetoVendor=obj.DoSendQuotetoVendor;
            this.AllowABIntegration = obj.AllowABIntegration;
            this.AllowOrderCloseAfterDays = obj.AllowOrderCloseAfterDays;
            this.IsELabel = obj.IsELabel;
            this.UserName = obj.UserName;
            this.Password = obj.Password;
            this.CompanyCode = obj.CompanyCode;
            this.StoreCode = obj.StoreCode;
            this.AllowAutoReceiveFromEDI = obj.AllowAutoReceiveFromEDI;

        }

        public SelectList Days
        {
            get
            {
                var list = new SelectList(new[]{
                          new {ID="0",Name=""},new {ID="1",Name="1"},new{ID="2",Name="2"},
                          new{ID="3",Name="3"},new {ID="4",Name="4"},new{ID="5",Name="5"},new{ID="6",Name="6"},
                          new {ID="7",Name="7"},new{ID="8",Name="8"},new{ID="9",Name="9"},new{ID="10",Name="10"},},
                           "ID", "Name", 1);
                return list;
            }
        }

        public SelectList AvgDays
        {
            get {
                var list2 = new SelectList(new[]{
                        new {ID="0",Name=""},new {ID="5",Name="5"},new{ID="10",Name="10"},new {ID="15",Name="15"},new{ID="20",Name="20"},new {ID="25",Name="25"},new{ID="30",Name="30"},new {ID="35",Name="35"},new{ID="40",Name="40"},new {ID="45",Name="45"},new{ID="50",Name="50"},
                        new {ID="55",Name="55"},new{ID="60",Name="60"},new {ID="65",Name="65"},new{ID="70",Name="70"},new {ID="75",Name="75"},new{ID="80",Name="80"},new {ID="85",Name="85"},new{ID="90",Name="90"},new {ID="95",Name="95"},new{ID="100",Name="100"},
                        new {ID="105",Name="105"},new{ID="110",Name="110"},new {ID="115",Name="115"},new{ID="120",Name="120"},new {ID="125",Name="125"},new{ID="130",Name="130"},new {ID="135",Name="135"},new{ID="140",Name="140"},new {ID="145",Name="145"},new{ID="150",Name="150"},
                        new {ID="155",Name="155"},new{ID="160",Name="160"},new {ID="165",Name="165"},new{ID="170",Name="170"},new {ID="175",Name="175"},new{ID="180",Name="180"},},
                              "ID", "Name", 1);
                return list2;
            }
        }

        public SelectList MethodOfValuing
        {
            get {
                var listMethodOfValuing = new SelectList(
                    new[]{
                    new SelectListItem{Value="3",Text=ResItemMaster.AverageCost},
                    new SelectListItem { Value="4",Text=ResRoomMaster.LastCost}
                    }, 
                    "Value", "Text"
                );
                return listMethodOfValuing;
            }

        }

        //public SelectList Weeks
        //{
        //    get {
        //        var listWeeks = new SelectList(new[]{
        //                  new {ID="1",Name="First"},new{ID="2",Name="Second"},
        //                  new{ID="3",Name="Third"},new{ID="4",Name="Fourth"},new{ID="5",Name="Fifth"},},
        //                "ID", "Name", 1);
        //        return listWeeks;
        //    }
        //}

        //public SelectList DayNames
        //{
        //    get
        //    {
        //        var listdayName = new SelectList(new[]{
        //                  new {ID="Monday",Name="Monday"},
        //            new{ID="Tuesday",Name="Tuesday"},
        //                  new{ID="Wednesday",Name="Wednesday"},new{ID="Thursday",Name="Thursday"},new{ID="Friday",Name="Friday"},
        //                new{ID="Saturday",Name="Saturday"},
        //            new{ID="Sunday",Name="Sunday"}
        //        },
        //              "ID", "Name", 1);
        //        return listdayName;
        //    }


        //}

        public List<CommonDTO> InventoryConsuptionMethodBAG
        {
            get {

                List<CommonDTO> ItemType = new List<CommonDTO>();
                ItemType.Add(new CommonDTO() { Text = "Lifo" });
                ItemType.Add(new CommonDTO() { Text = "Fifo" });
                return ItemType;
            }
        }

        SelectList _BillingRoomTypeList;

        public SelectList BillingRoomTypeList
        {
            get
            {
                _BillingRoomTypeList = GetBillingRoomTypeList(this.EnterpriseId);
                return _BillingRoomTypeList;
            }
        }

        public SelectList GetBillingRoomTypeList(long entId)
        {
            
                //  var list = new SelectList(
                //new[] {
                //                  new SelectListItem{ Value = "1", Text = ResRoomMaster.BillingRoomType_AssetOnly},
                //                  new SelectListItem{Value = "2", Text = ResRoomMaster.BillingRoomType_eVMI},
                //                  new SelectListItem{Value = "3", Text = ResRoomMaster.BillingRoomType_Manage},
                //                  new SelectListItem{Value = "4", Text = ResRoomMaster.BillingRoomType_Replenish},
                //                  new SelectListItem{Value = "5", Text = ResRoomMaster.BillingRoomType_RFID},
                //                  new SelectListItem{Value = "6", Text = ResRoomMaster.BillingRoomType_ToolandAssetOnly},
                //                  new SelectListItem{Value = "7", Text = ResRoomMaster.BillingRoomType_ToolOnly},
                //                  new SelectListItem{Value = "8", Text = ResRoomMaster.BillingRoomType_Truck}
                //}, "Value", "Text");
                //  return list;
                if(_BillingRoomTypeList == null)
                {
                    using (BillingRoomTypeMasterBAL bal = new BillingRoomTypeMasterBAL())
                    {
                        List<BillingRoomTypeMasterDTO> list = bal.GetBillingRoomTypeMaster(entId);
                        _BillingRoomTypeList = new SelectList(list, "ID", "ResourceValue");
                    }
                }

                return _BillingRoomTypeList;
            
        }

        public List<RoomDTO> ReplineshmentRoomList { get; set; }
        public List<CommonDTO> ReplenishmentTypeList
        {
            get
            {
                List<CommonDTO> ItemType = new List<CommonDTO>();
                ItemType.Add(new CommonDTO() { ID = 1, Text = ResRoomMaster.ReplanishmentType_Itemreplenish });//"Itemreplenish"
                ItemType.Add(new CommonDTO() { ID = 2, Text = ResRoomMaster.ReplanishmentType_Locationreplenish });//"Location replenish"
                ItemType.Add(new CommonDTO() { ID = 3, Text = ResRoomMaster.ReplanishmentType_Both }); //"Both"

                return ItemType;
            }
        }

        public List<SupplierMasterDTO> DefaultSupplierList { get; set; }
    }
}