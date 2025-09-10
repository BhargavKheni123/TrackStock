using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Transactions;

namespace eTurns.DAL
{
    public partial class RoomDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public RoomDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public RoomDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        /// <summary>
        /// Insert Record in the DataBase Room Master
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(RoomDTO objDTO)
        {

            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                long? supID = null;
                long? DefaulBinID = null;
                Room obj = new Room();
                obj.ID = 0;
                obj.RoomName = objDTO.RoomName;
                obj.CompanyName = objDTO.CompanyName;
                obj.CompanyID = objDTO.CompanyID;
                obj.ContactName = objDTO.ContactName;
                obj.StreetAddress = objDTO.streetaddress;
                obj.City = objDTO.City;
                obj.State = objDTO.State;
                obj.PostalCode = objDTO.PostalCode;
                obj.Country = objDTO.Country;
                obj.Email = objDTO.Email;
                obj.PhoneNo = objDTO.PhoneNo;
                obj.InvoiceBranch = objDTO.InvoiceBranch;
                obj.CustomerNumber = objDTO.CustomerNumber;
                obj.BlanketPO = objDTO.BlanketPO;
                obj.IsConsignment = objDTO.IsConsignment;
                obj.IsTrending = objDTO.IsTrending;
                obj.SourceOfTrending = objDTO.SourceOfTrending;
                obj.TrendingFormula = objDTO.TrendingFormula;
                obj.TrendingFormulaType = objDTO.TrendingFormulaType;
                obj.TrendingFormulaDays = objDTO.TrendingFormulaDays;
                obj.TrendingFormulaOverDays = objDTO.TrendingFormulaOverDays;
                obj.TrendingFormulaAvgDays = objDTO.TrendingFormulaAvgDays;
                obj.TrendingFormulaCounts = objDTO.TrendingFormulaCounts;
                // obj.ManualMin = objDTO.ManualMin;
                obj.SuggestedOrder = objDTO.SuggestedOrder;
                obj.SuggestedTransfer = objDTO.SuggestedTransfer;
                obj.ReplineshmentRoom = objDTO.ReplineshmentRoom;
                //obj.IsItemReplenishment = objDTO.IsItemReplenishment;
                //obj.IsBinReplenishment = objDTO.IsBinReplenishment;
                obj.ReplenishmentType = objDTO.ReplenishmentType;
                obj.IseVMI = objDTO.IseVMI;
                obj.MaxOrderSize = objDTO.MaxOrderSize;
                obj.HighPO = objDTO.HighPO;
                obj.HighJob = objDTO.HighJob;
                obj.HighTransfer = objDTO.HighTransfer;
                obj.HighCount = objDTO.HighCount;
                obj.GlobMarkupParts = objDTO.GlobMarkupParts;
                obj.GlobMarkupLabor = objDTO.GlobMarkupLabor;
                obj.IsTax1Parts = objDTO.IsTax1Parts;
                obj.IsTax1Labor = objDTO.IsTax1Labor;
                obj.Tax1Name = objDTO.Tax1name;
                obj.Tax1Rate = objDTO.Tax1Rate;
                obj.IsTax2Parts = objDTO.IsTax2Parts;
                obj.IsTax2Labor = objDTO.IsTax2Labor;
                obj.Tax2Name = objDTO.tax2name;
                obj.Tax2Rate = objDTO.Tax2Rate;
                obj.IsTax2onTax1 = objDTO.IsTax2onTax1 ?? false;
                obj.GXPRConsJob = objDTO.GXPRConsJob;
                obj.CostCenter = objDTO.CostCenter;
                obj.UniqueID = objDTO.UniqueID;
                obj.Updated = objDTO.Updated ?? DateTime.UtcNow;
                obj.Created = objDTO.Created ?? DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.GUID = Guid.NewGuid();
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.BaseOfInventory = objDTO.BaseOfInventory;
                obj.MethodOfValuingInventory = objDTO.MethodOfValuingInventory;
                obj.AutoCreateTransferFrequency = objDTO.AutoCreateTransferFrequency;
                obj.AutoCreateTransferTime = objDTO.AutoCreateTransferTime;
                obj.AutoCreateTransferSubmit = objDTO.AutoCreateTransferSubmit;
                obj.IsActive = objDTO.IsActive;
                obj.LicenseBilled = objDTO.LicenseBilled;
                obj.NextCountNo = objDTO.NextCountNo;
                obj.POAutoSequence = objDTO.POAutoSequence;
                obj.IsProjectSpendMandatory = objDTO.IsProjectSpendMandatory;
                obj.IsAverageUsageBasedOnPull = objDTO.IsAverageUsageBasedOnPull;
                obj.SlowMovingValue = objDTO.SlowMovingValue;
                obj.FastMovingValue = objDTO.FastMovingValue;
                obj.NextOrderNo = objDTO.NextOrderNo;
                obj.NextRequisitionNo = objDTO.NextRequisitionNo;
                obj.NextStagingNo = objDTO.NextStagingNo;
                obj.NextTransferNo = objDTO.NextTransferNo;
                obj.NextWorkOrderNo = objDTO.NextWorkOrderNo;
                obj.RoomGrouping = objDTO.RoomGrouping;

                obj.TransferFrequencyOption = objDTO.TransferFrequencyOption;
                obj.TransferFrequencyDays = objDTO.TransferFrequencyDays;
                obj.TransferFrequencyMonth = objDTO.TransferFrequencyMonth;
                obj.TransferFrequencyNumber = objDTO.TransferFrequencyNumber;
                obj.TransferFrequencyWeek = objDTO.TransferFrequencyWeek;
                obj.TransferFrequencyMainOption = objDTO.TransferFrequencyMainOption;

                obj.TrendingSampleSize = objDTO.TrendingSampleSize;
                obj.TrendingSampleSizeDivisor = objDTO.TrendingSampleSizeDivisor;

                obj.AverageUsageTransactions = objDTO.AverageUsageTransactions;
                obj.AverageUsageSampleSize = objDTO.AverageUsageSampleSize;
                obj.AverageUsageSampleSizeDivisor = objDTO.AverageUsageSampleSizeDivisor;

                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;

                obj.NextAssetNo = objDTO.NextAssetNo;
                obj.NextBinNo = objDTO.NextBinNo;
                obj.NextKitNo = objDTO.NextKitNo;
                obj.NextItemNo = objDTO.NextItemNo;
                obj.NextProjectSpendNo = objDTO.NextProjectSpendNo;
                obj.NextToolNo = objDTO.NextToolNo;
                obj.InventoryConsuptionMethod = objDTO.InventoryConsuptionMethod;
                obj.DefaultBinID = objDTO.DefaultBinID;
                obj.DefaultSupplierID = supID;
                obj.IsRoomActive = objDTO.IsRoomActive;
                obj.RequestedXDays = objDTO.RequestedXDays.GetValueOrDefault(0);
                obj.RequestedYDays = objDTO.RequestedYDays.GetValueOrDefault(0);
                obj.DefaultBinName = objDTO.DefaultBinName;
                obj.eVMIWaitCommand = objDTO.eVMIWaitCommand;
                obj.eVMIWaitPort = objDTO.eVMIWaitPort;
                obj.CountAutoSequence = objDTO.CountAutoSequence;
                obj.ShelfLifeleadtimeOrdRpt = objDTO.ShelfLifeleadtimeOrdRpt;
                obj.LeadTimeOrdRpt = objDTO.LeadTimeOrdRpt;
                obj.PullPurchaseNumberType = objDTO.PullPurchaseNumberType;
                obj.LastPullPurchaseNumberUsed = objDTO.LastPullPurchaseNumberUsed;
                obj.IsAllowRequisitionDuplicate = objDTO.IsAllowRequisitionDuplicate ?? true;
                obj.ExtPhoneNo = objDTO.ExtPhoneNo;
                obj.ReqAutoSequence = objDTO.ReqAutoSequence;
                obj.AllowInsertingItemOnScan = objDTO.AllowInsertingItemOnScan ?? false;
                obj.IsAllowOrderDuplicate = objDTO.IsAllowOrderDuplicate;
                obj.IsAllowWorkOrdersDuplicate = objDTO.IsAllowWorkOrdersDuplicate;
                obj.AllowPullBeyondAvailableQty = objDTO.AllowPullBeyondAvailableQty;
                obj.PullRejectionType = objDTO.PullRejectionType;
                obj.CountAutoNrFixedValue = objDTO.CountAutoNrFixedValue;
                obj.POAutoNrFixedValue = objDTO.POAutoNrFixedValue;
                obj.PullPurchaseNrFixedValue = objDTO.PullPurchaseNrFixedValue;
                obj.ReqAutoNrFixedValue = objDTO.ReqAutoNrFixedValue;
                obj.BillingRoomType = objDTO.BillingRoomType;

                obj.StagingAutoSequence = objDTO.StagingAutoSequence;
                obj.TransferAutoSequence = objDTO.TransferAutoSequence;
                obj.WorkOrderAutoSequence = objDTO.WorkOrderAutoSequence;
                obj.StagingAutoNrFixedValue = objDTO.StagingAutoNrFixedValue;
                obj.TransferAutoNrFixedValue = objDTO.TransferAutoNrFixedValue;
                obj.WorkOrderAutoNrFixedValue = objDTO.WorkOrderAutoNrFixedValue;

                obj.WarnUserOnAssigningNonDefaultBin = objDTO.WarnUserOnAssigningNonDefaultBin;
                obj.MaintenanceDueNoticeDays = objDTO.MaintenanceDueNoticeDays;

                obj.DefaultRequisitionRequiredDays = objDTO.DefaultRequisitionRequiredDays;
                obj.AttachingWOWithRequisition = objDTO.AttachingWOWithRequisition;
                obj.PreventMaxOrderQty = objDTO.PreventMaxOrderQty;
                obj.DefaultCountType = objDTO.DefaultCountType;
                obj.TAOAutoSequence = objDTO.TAOAutoSequence;
                obj.TAOAutoNrFixedValue = objDTO.TAOAutoNrFixedValue;
                obj.NextToolAssetOrderNo = objDTO.NextToolAssetOrderNo;
                obj.AllowToolOrdering = objDTO.AllowToolOrdering;
                obj.IsWOSignatureRequired = objDTO.IsWOSignatureRequired;
                obj.IsIgnoreCreditRule = objDTO.IsIgnoreCreditRule;
                obj.IsAllowOrderCostuom = objDTO.IsAllowOrderCostuom;
                obj.ToolCountAutoSequence = objDTO.ToolCountAutoSequence;
                obj.NextToolCountNo = objDTO.NextToolCountNo;
                obj.SuggestedReturn = objDTO.SuggestedReturn;
                obj.ForceSupplierFilter = objDTO.ForceSupplierFilter;

                if (!string.IsNullOrEmpty(objDTO.ReportAppIntent) && !string.IsNullOrWhiteSpace(objDTO.ReportAppIntent))
                {
                    obj.ReportAppIntent = objDTO.ReportAppIntent;
                }
                else
                {
                    obj.ReportAppIntent = "ReadOnly";
                }
                obj.IsOrderReleaseNumberEditable = objDTO.IsOrderReleaseNumberEditable;
                obj.QuoteAutoSequence = objDTO.QuoteAutoSequence;
                obj.NextQuoteNo = objDTO.NextQuoteNo;
                obj.IsAllowQuoteDuplicate = objDTO.IsAllowQuoteDuplicate;
                obj.QuoteAutoNrFixedValue = objDTO.QuoteAutoNrFixedValue;
                obj.DoGroupSupplierQuoteToOrder = objDTO.DoGroupSupplierQuoteToOrder;
                obj.DoSendQuotetoVendor = objDTO.DoSendQuotetoVendor;
                obj.AllowABIntegration = objDTO.AllowABIntegration;
                obj.AllowOrderCloseAfterDays = objDTO.AllowOrderCloseAfterDays;
                obj.IsELabel = objDTO.IsELabel;
                obj.UserName = objDTO.UserName;
                obj.Password = objDTO.Password;
                obj.CompanyCode = objDTO.CompanyCode;
                obj.StoreCode = objDTO.StoreCode;
                obj.TrialStartDate = objDTO.TrialStartDate;
                obj.AllowAutoReceiveFromEDI = objDTO.AllowAutoReceiveFromEDI ?? false;
                context.Rooms.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(base.DataBaseName);
                SupplierMasterDTO objSupplierMasterDTO = null;
                if (!string.IsNullOrWhiteSpace(objDTO.DefaultSupplierName))
                {
                    objSupplierMasterDTO = new SupplierMasterDTO();
                    objSupplierMasterDTO.ID = 0;
                    objSupplierMasterDTO.SupplierName = objDTO.DefaultSupplierName;
                    objSupplierMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objSupplierMasterDTO.CreatedBy = objDTO.CreatedBy;
                    objSupplierMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objSupplierMasterDTO.LastUpdatedBy = objDTO.CreatedBy;
                    objSupplierMasterDTO.CompanyID = objDTO.CompanyID;
                    objSupplierMasterDTO.Room = objDTO.ID;
                    objSupplierMasterDTO.AddedFrom = "Web";
                    objSupplierMasterDTO.EditedFrom = "Web";
                    objSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    supID = objSupplierMasterDAL.Insert(objSupplierMasterDTO);
                }
                obj.DefaultSupplierID = supID;
                BinMasterDTO objBinMasterDTO = new BinMasterDTO();
                BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
                if (!string.IsNullOrWhiteSpace(objDTO.DefaultBinName))
                {
                    objBinMasterDTO = new BinMasterDTO();
                    objBinMasterDTO.ID = 0;
                    objBinMasterDTO.BinNumber = objDTO.DefaultBinName;
                    objBinMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.CreatedBy = objDTO.CreatedBy;
                    objBinMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.LastUpdatedBy = objDTO.CreatedBy;
                    objBinMasterDTO.CompanyID = objDTO.CompanyID;
                    objBinMasterDTO.Room = objDTO.ID;
                    objBinMasterDTO.AddedFrom = "Web";
                    objBinMasterDTO.EditedFrom = "Web";
                    objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO = objBinMasterDAL.InsertBin(objBinMasterDTO);
                    DefaulBinID = objBinMasterDTO.ID;
                }
                obj.DefaultBinID = DefaulBinID;

                //Add Default Cost UOM when new room created
                CostUOMMasterDAL oCostUOMMasterDAL = new CostUOMMasterDAL(base.DataBaseName);
                oCostUOMMasterDAL.InsertDefaultCostUOMByNameAndValue("E", 1, objDTO.CreatedBy, objDTO.ID, objDTO.CompanyID, false);
                oCostUOMMasterDAL.InsertDefaultCostUOMByNameAndValue("EA", 1, objDTO.CreatedBy, objDTO.ID, objDTO.CompanyID, false);
                oCostUOMMasterDAL.InsertDefaultCostUOMByNameAndValue("C", 100, objDTO.CreatedBy, objDTO.ID, objDTO.CompanyID, false);
                oCostUOMMasterDAL.InsertDefaultCostUOMByNameAndValue("M", 1000, objDTO.CreatedBy, objDTO.ID, objDTO.CompanyID, false);

                //Add Default Order UOM when new room created
                //OrderUOMMasterDAL oOrderUOMMasterDAL = new OrderUOMMasterDAL(base.DataBaseName);
                //oOrderUOMMasterDAL.InsertDefaultOrderUOMByNameAndValue("E", 1, objDTO.CreatedBy, objDTO.ID, objDTO.CompanyID, false);
                //oOrderUOMMasterDAL.InsertDefaultOrderUOMByNameAndValue("EA", 1, objDTO.CreatedBy, objDTO.ID, objDTO.CompanyID, false);
                //oOrderUOMMasterDAL.InsertDefaultOrderUOMByNameAndValue("C", 100, objDTO.CreatedBy, objDTO.ID, objDTO.CompanyID, false);
                //oOrderUOMMasterDAL.InsertDefaultOrderUOMByNameAndValue("M", 1000, objDTO.CreatedBy, objDTO.ID, objDTO.CompanyID, false);

                //Add Default UOM when new room created
                UnitMasterDAL oUnitMasterDAL = new UnitMasterDAL(base.DataBaseName);
                oUnitMasterDAL.InsertDefaultUOMByName("E", objDTO.CreatedBy, objDTO.ID, objDTO.CompanyID);
                oUnitMasterDAL.InsertDefaultUOMByName("EA", objDTO.CreatedBy, objDTO.ID, objDTO.CompanyID);

                //Add default Tool Written Off Category when new room created
                WrittenOffCategoryDAL objWOC = new WrittenOffCategoryDAL(base.DataBaseName);
                objWOC.InsertDefaultEntry("Stolen", objDTO.CreatedBy, objDTO.ID, objDTO.CompanyID);
                objWOC.InsertDefaultEntry("Broken", objDTO.CreatedBy, objDTO.ID, objDTO.CompanyID);
                objWOC.InsertDefaultEntry("Expired", objDTO.CreatedBy, objDTO.ID, objDTO.CompanyID);

                context.SaveChanges();
                objDTO.DefaultSupplierID = supID;
                if (objDTO.ID > 0)
                {
                    if (objDTO.lstRoomModleSettings != null && objDTO.lstRoomModleSettings.Count > 0)
                    {
                        SaveRoomModuleSettings(objDTO.lstRoomModleSettings, objDTO.CompanyID, objDTO.ID, objDTO.CreatedBy);
                    }

                }

                return obj.ID;
            }
        }

        public void Insert_UPDATE_SensorBinRoomSettings(eVMISiteSettings SensorBinRoomSettings)
        {
            var params1 = new SqlParameter[] {
                new SqlParameter("@ID", SensorBinRoomSettings.ID),
                new SqlParameter("@RoomID", SensorBinRoomSettings.RoomID ),
                new SqlParameter("@EnterpriseID", SensorBinRoomSettings.EnterpriseID ),
                new SqlParameter("@CompanyID", SensorBinRoomSettings.CompanyID ),
                new SqlParameter("@Guid", SensorBinRoomSettings.Guid ),
                new SqlParameter("@GetFirmWareVersionRequestType", SensorBinRoomSettings.GetFirmWareVersionRequestType ),
                new SqlParameter("@GetSerialNoRequestType", SensorBinRoomSettings.GetSerialNoRequestType ),
                new SqlParameter("@GetModelRequestType", SensorBinRoomSettings.GetModelRequestType ),
                new SqlParameter("@SetModelRequestType", SensorBinRoomSettings.SetModelRequestType ),
                new SqlParameter("@GetShelfIDRequestType", SensorBinRoomSettings.GetShelfIDRequestType),
                new SqlParameter("@SetShelfIDRequestType", SensorBinRoomSettings.SetShelfIDRequestType ),
                new SqlParameter("@GetCalibrationWeightRequestType", SensorBinRoomSettings.GetCalibrationWeightRequestType ),
                new SqlParameter("@SetCalibrationWeightRequestType", SensorBinRoomSettings.SetCalibrationWeightRequestType),
                new SqlParameter("@CalibrateRequestType", SensorBinRoomSettings.CalibrateRequestType),
                new SqlParameter("@PollRequestType", SensorBinRoomSettings.PollRequestType),
                new SqlParameter("@TareRequestType", SensorBinRoomSettings.TareRequestType),
                new SqlParameter("@ItemWeightPerPieceRequestType", SensorBinRoomSettings.ItemWeightPerPieceRequestType),
                new SqlParameter("@ResetRequestType", SensorBinRoomSettings.ResetRequestType),
                new SqlParameter("@PollCommand", SensorBinRoomSettings.PollCommand),
                new SqlParameter("@Created", SensorBinRoomSettings.Created?? (object)DBNull.Value ),
                new SqlParameter("@LastUpdated", SensorBinRoomSettings.LastUpdated?? (object)DBNull.Value ),
                new SqlParameter("@CreatedBy", SensorBinRoomSettings.CreatedBy?? (object)DBNull.Value  ),
                new SqlParameter("@LastUpdatedBy", SensorBinRoomSettings.LastUpdatedBy?? (object)DBNull.Value  ),
                new SqlParameter("@IsDeleted", SensorBinRoomSettings.IsDeleted?? (object)DBNull.Value ),
                new SqlParameter("@IsActive", SensorBinRoomSettings.IsActive?? (object)DBNull.Value )
            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC  [dbo].[Insert_UPDATE_SensorBinRoomSettings] @ID,@RoomID,@CompanyID,@EnterpriseID,@Guid,@GetFirmWareVersionRequestType,@GetSerialNoRequestType,@GetModelRequestType,@SetModelRequestType,@GetShelfIDRequestType,@SetShelfIDRequestType,@GetCalibrationWeightRequestType,@SetCalibrationWeightRequestType,@CalibrateRequestType,@PollRequestType,@TareRequestType,@ItemWeightPerPieceRequestType,@ResetRequestType,@PollCommand,@Created,@LastUpdated,@CreatedBy,@LastUpdatedBy,@IsDeleted,@IsActive", params1);
            }
        }

        public void SaveRoomModuleSettings(List<RoomModuleSettingsDTO> lstRoomModleSettings, long? CompanyId, long? RoomId, long? UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RoomModuleSetting objRoomModuleSetting;
                foreach (RoomModuleSettingsDTO objRoomModuleSettingsDTO in lstRoomModleSettings)
                {
                    objRoomModuleSetting = (from RMS in context.RoomModuleSettings
                                            where RMS.CompanyId == CompanyId
                                                   && RMS.RoomId == RoomId
                                                   && RMS.ModuleId == objRoomModuleSettingsDTO.ModuleId
                                            select RMS).FirstOrDefault();
                    if (objRoomModuleSetting != null)
                    {
                        objRoomModuleSetting.PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption;
                        objRoomModuleSetting.LastUpdatedBy = UserId;
                        objRoomModuleSetting.LastUpdatedDate = DateTime.UtcNow;
                        context.SaveChanges();
                    }
                    else
                    {
                        objRoomModuleSetting = new RoomModuleSetting();
                        objRoomModuleSetting.CompanyId = CompanyId;
                        objRoomModuleSetting.RoomId = RoomId;
                        objRoomModuleSetting.ModuleId = objRoomModuleSettingsDTO.ModuleId;
                        objRoomModuleSetting.PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption;
                        objRoomModuleSetting.CreatedBy = UserId;
                        objRoomModuleSetting.CreatedDate = DateTime.UtcNow;
                        objRoomModuleSetting.LastUpdatedBy = UserId;
                        objRoomModuleSetting.LastUpdatedDate = DateTime.UtcNow;
                        context.RoomModuleSettings.Add(objRoomModuleSetting);
                        context.SaveChanges();
                    }
                }
            }
        }


        public RoomDTO Edit(RoomDTO objDTO, out bool isBillingTypeChanged)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            Room objExistedRoom = new Room();
            isBillingTypeChanged = false;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objExistedRoom = context.Rooms.FirstOrDefault(t => t.ID == objDTO.ID);
                if (objExistedRoom.IsRoomActive != objDTO.IsRoomActive)
                {
                    objDTO.RoomActiveStatus = 1;
                }
                else
                {
                    objDTO.RoomActiveStatus = 0;
                }
                //if (objExistedRoom.IsRoomActive != objDTO.IsRoomActive)
                //{
                //    RoomActiveInactive objRoomActive = new RoomActiveInactive();
                //    objRoomActive.CreatedBy = objDTO.CreatedBy;
                //    objRoomActive.LastUpdatedBy = objDTO.LastUpdatedBy;
                //    objRoomActive.CreatedOn = DateTimeUtility.DateTimeNow;
                //    objRoomActive.UpdatedOn = DateTimeUtility.DateTimeNow;
                //    objRoomActive.FromUpdated = "Web";
                //    objRoomActive.PageFrom = "CreateRoom";
                //    objRoomActive.Isactive = objDTO.IsRoomActive;
                //    objRoomActive.RoomId = objDTO.ID;
                //    context.RoomActiveInactives(objRoomActive);
                //    context.SaveChanges();
                //}
                if (objExistedRoom.BaseOfInventory != objDTO.BaseOfInventory)
                {
                    objDTO.ReclassifyAllItems = 1;
                }
                else
                {
                    objDTO.ReclassifyAllItems = 0;
                }

                if (objExistedRoom.RoomName != objDTO.RoomName)
                {
                    objDTO.RoomNameChange = 1;
                }
                else
                {
                    objDTO.RoomNameChange = 0;
                }
                if (objExistedRoom.SuggestedOrder != objDTO.SuggestedOrder)
                {
                    objDTO.SOSettingChanged = 1;
                }
                else
                {
                    objDTO.SOSettingChanged = 0;
                }
                if (objExistedRoom.SuggestedTransfer != objDTO.SuggestedTransfer)
                {
                    objDTO.STSettingChanged = 1;
                }
                else
                {
                    objDTO.STSettingChanged = 0;
                }
                if (objExistedRoom.SuggestedReturn != objDTO.SuggestedReturn)
                {
                    objDTO.SRSettingChanged = 1;
                }
                else
                {
                    objDTO.SRSettingChanged = 0;
                }

                if (objExistedRoom.BillingRoomType != objDTO.BillingRoomType)
                {
                    isBillingTypeChanged = true;
                }

                long? DefaultSupplierId = null;
                if (!string.IsNullOrWhiteSpace(objDTO.DefaultSupplierName))
                {
                    SupplierMasterDTO objSupplierMaster = new SupplierMasterDTO();
                    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(base.DataBaseName);
                    objSupplierMaster = objSupplierMasterDAL.GetSupplierByNamePlain(objDTO.ID, objDTO.CompanyID ?? 0, false, objDTO.DefaultSupplierName);
                    if (objSupplierMaster == null)
                    {
                        SupplierMasterDTO objSupplierMasterDTO = new SupplierMasterDTO();
                        objSupplierMasterDTO = new SupplierMasterDTO();
                        objSupplierMasterDTO.ID = 0;
                        objSupplierMasterDTO.SupplierName = objDTO.DefaultSupplierName;
                        objSupplierMasterDTO.Created = DateTimeUtility.DateTimeNow;
                        objSupplierMasterDTO.CreatedBy = objDTO.CreatedBy;
                        objSupplierMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objSupplierMasterDTO.LastUpdatedBy = objDTO.CreatedBy;
                        objSupplierMasterDTO.CompanyID = objDTO.CompanyID;
                        objSupplierMasterDTO.Room = objDTO.ID;
                        objSupplierMasterDTO.AddedFrom = "Web";
                        objSupplierMasterDTO.EditedFrom = "Web";
                        objSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        DefaultSupplierId = objSupplierMasterDAL.Insert(objSupplierMasterDTO);
                    }
                    else
                    {
                        DefaultSupplierId = objSupplierMaster.ID;
                    }

                }


                if (!string.IsNullOrWhiteSpace(objDTO.DefaultBinName))
                {
                    BinMaster objBinMaster = new BinMaster();
                    BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
                    objBinMaster = context.BinMasters.FirstOrDefault(t => t.Room == objDTO.ID && t.CompanyID == objDTO.CompanyID && t.BinNumber == objDTO.DefaultBinName && t.IsDeleted == false && t.ParentBinId == null);
                    BinMasterDTO objBinMasterDTO = null;
                    if (objBinMaster == null)
                    {
                        objBinMasterDTO = new BinMasterDTO();
                        objBinMasterDTO.ID = 0;
                        objBinMasterDTO.BinNumber = objDTO.DefaultBinName;
                        objBinMasterDTO.Created = DateTimeUtility.DateTimeNow;
                        objBinMasterDTO.CreatedBy = objDTO.CreatedBy;
                        objBinMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objBinMasterDTO.LastUpdatedBy = objDTO.CreatedBy;
                        objBinMasterDTO.CompanyID = objDTO.CompanyID;
                        objBinMasterDTO.Room = objDTO.ID;
                        objBinMasterDTO.AddedFrom = "Web";
                        objBinMasterDTO.EditedFrom = "Web";
                        objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objBinMasterDTO = objBinMasterDAL.InsertBin(objBinMasterDTO);
                        objDTO.DefaultBinID = objBinMasterDTO.ID;
                    }
                    else
                    {
                        objDTO.DefaultBinID = objBinMaster.ID;
                    }
                }
                else
                {
                    objDTO.DefaultBinID = null;
                }
                if (objExistedRoom != null)
                {
                    if (objExistedRoom.AllowPullBeyondAvailableQty == true && objDTO.AllowPullBeyondAvailableQty == false)
                    {
                        ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                        long RoomId = (objDTO.RoomId != null ? (long)objDTO.RoomId : objDTO.ID);
                        int NegativeQtyItmCount = objItemMasterDAL.GetNegativeQuantityItemCountByRoomId(RoomId);
                        if (NegativeQtyItmCount > 0)
                        {
                            if (objDTO.PullRejectionType == 1)
                            {
                                BinMasterDAL objBinDAL = new BinMasterDAL(base.DataBaseName);
                                objBinDAL.CSP_DeletePull_ForAllNegativeItem((long)RoomId, (long)objDTO.CompanyID, (long)objDTO.LastUpdatedBy, "Pull Neg. Del. From Web", "Web");
                            }
                            else if (objDTO.PullRejectionType == 2)
                            {
                                BinMasterDAL objBinDAL = new BinMasterDAL(base.DataBaseName);
                                objBinDAL.CSP_InsertCreditPullForNegativeQtyBins("", (long)objDTO.CompanyID, RoomId, (long)objDTO.LastUpdatedBy, "Pull Neg. Adj. From Web (Room Update)", "Web");
                            }
                        }
                    }

                    objExistedRoom.ID = objDTO.ID;
                    objExistedRoom.RoomName = objDTO.RoomName;
                    objExistedRoom.LastUpdatedBy = objDTO.LastUpdatedBy;
                    objExistedRoom.CompanyName = objDTO.CompanyName;
                    objExistedRoom.CompanyID = objDTO.CompanyID;
                    objExistedRoom.ContactName = objDTO.ContactName;
                    objExistedRoom.StreetAddress = objDTO.streetaddress;
                    objExistedRoom.City = objDTO.City;
                    objExistedRoom.State = objDTO.State;
                    objExistedRoom.PostalCode = objDTO.PostalCode;
                    objExistedRoom.Country = objDTO.Country;
                    objExistedRoom.Email = objDTO.Email;
                    objExistedRoom.PhoneNo = objDTO.PhoneNo;
                    objExistedRoom.InvoiceBranch = objDTO.InvoiceBranch;
                    objExistedRoom.CustomerNumber = objDTO.CustomerNumber;
                    objExistedRoom.BlanketPO = objDTO.BlanketPO;
                    objExistedRoom.IsConsignment = objDTO.IsConsignment;
                    objExistedRoom.IsTrending = objDTO.IsTrending;
                    objExistedRoom.SourceOfTrending = objDTO.SourceOfTrending;
                    objExistedRoom.TrendingFormula = objDTO.TrendingFormula;
                    objExistedRoom.TrendingFormulaType = objDTO.TrendingFormulaType;
                    objExistedRoom.TrendingFormulaDays = objDTO.TrendingFormulaDays;
                    objExistedRoom.TrendingFormulaOverDays = objDTO.TrendingFormulaOverDays;
                    objExistedRoom.TrendingFormulaAvgDays = objDTO.TrendingFormulaAvgDays;
                    objExistedRoom.TrendingFormulaCounts = objDTO.TrendingFormulaCounts;
                    objExistedRoom.SuggestedOrder = objDTO.SuggestedOrder;
                    objExistedRoom.SuggestedTransfer = objDTO.SuggestedTransfer;
                    objExistedRoom.ReplineshmentRoom = objDTO.ReplineshmentRoom;
                    objExistedRoom.ReplenishmentType = objDTO.ReplenishmentType;
                    objExistedRoom.IseVMI = objDTO.IseVMI;
                    objExistedRoom.MaxOrderSize = objDTO.MaxOrderSize;
                    objExistedRoom.HighPO = objDTO.HighPO;
                    objExistedRoom.HighJob = objDTO.HighJob;
                    objExistedRoom.HighTransfer = objDTO.HighTransfer;
                    objExistedRoom.HighCount = objDTO.HighCount;
                    objExistedRoom.GlobMarkupParts = objDTO.GlobMarkupParts;
                    objExistedRoom.GlobMarkupLabor = objDTO.GlobMarkupLabor;
                    objExistedRoom.IsTax1Parts = objDTO.IsTax1Parts;
                    objExistedRoom.IsTax1Labor = objDTO.IsTax1Labor;
                    objExistedRoom.Tax1Name = objDTO.Tax1name;
                    objExistedRoom.Tax1Rate = objDTO.Tax1Rate;
                    objExistedRoom.IsTax2Parts = objDTO.IsTax2Parts;
                    objExistedRoom.IsTax2Labor = objDTO.IsTax2Labor;
                    objExistedRoom.Tax2Name = objDTO.tax2name;
                    objExistedRoom.Tax2Rate = objDTO.Tax2Rate;
                    objExistedRoom.IsTax2onTax1 = objDTO.IsTax2onTax1 ?? false;
                    objExistedRoom.GXPRConsJob = objDTO.GXPRConsJob;
                    objExistedRoom.CostCenter = objDTO.CostCenter;
                    objExistedRoom.UniqueID = objDTO.UniqueID;
                    objExistedRoom.Updated = objDTO.Updated ?? DateTime.UtcNow;
                    objExistedRoom.Created = objDTO.Created;
                    objExistedRoom.CreatedBy = objDTO.CreatedBy;
                    objExistedRoom.GUID = objDTO.GUID;
                    objExistedRoom.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                    objExistedRoom.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                    objExistedRoom.LastUpdatedBy = objDTO.LastUpdatedBy;
                    objExistedRoom.BaseOfInventory = objDTO.BaseOfInventory;
                    objExistedRoom.MethodOfValuingInventory = objDTO.MethodOfValuingInventory;
                    objExistedRoom.IsActive = objDTO.IsActive;
                    objExistedRoom.LicenseBilled = objDTO.LicenseBilled;
                    objExistedRoom.NextCountNo = objDTO.NextCountNo;
                    objExistedRoom.NextOrderNo = objDTO.NextOrderNo;
                    objExistedRoom.POAutoSequence = objDTO.POAutoSequence;
                    objExistedRoom.IsProjectSpendMandatory = objDTO.IsProjectSpendMandatory;
                    objExistedRoom.IsAverageUsageBasedOnPull = objDTO.IsAverageUsageBasedOnPull;
                    objExistedRoom.SlowMovingValue = objDTO.SlowMovingValue;
                    objExistedRoom.FastMovingValue = objDTO.FastMovingValue;
                    objExistedRoom.NextRequisitionNo = objDTO.NextRequisitionNo;
                    objExistedRoom.NextStagingNo = objDTO.NextStagingNo;
                    objExistedRoom.NextTransferNo = objDTO.NextTransferNo;
                    objExistedRoom.NextWorkOrderNo = objDTO.NextWorkOrderNo;
                    objExistedRoom.RoomGrouping = objDTO.RoomGrouping;
                    objExistedRoom.AutoCreateTransferFrequency = objDTO.AutoCreateTransferFrequency;
                    objExistedRoom.AutoCreateTransferTime = objDTO.AutoCreateTransferTime;
                    objExistedRoom.AutoCreateTransferSubmit = objDTO.AutoCreateTransferSubmit;
                    objExistedRoom.TransferFrequencyOption = objDTO.TransferFrequencyOption;
                    objExistedRoom.TransferFrequencyDays = objDTO.TransferFrequencyDays;
                    objExistedRoom.TransferFrequencyMonth = objDTO.TransferFrequencyMonth;
                    objExistedRoom.TransferFrequencyNumber = objDTO.TransferFrequencyNumber;
                    objExistedRoom.TransferFrequencyWeek = objDTO.TransferFrequencyWeek;
                    objExistedRoom.TransferFrequencyMainOption = objDTO.TransferFrequencyMainOption;
                    objExistedRoom.TrendingSampleSize = objDTO.TrendingSampleSize;
                    objExistedRoom.TrendingSampleSizeDivisor = objDTO.TrendingSampleSizeDivisor;
                    objExistedRoom.AverageUsageTransactions = objDTO.AverageUsageTransactions;
                    objExistedRoom.AverageUsageSampleSize = objDTO.AverageUsageSampleSize;
                    objExistedRoom.AverageUsageSampleSizeDivisor = objDTO.AverageUsageSampleSizeDivisor;
                    objExistedRoom.UDF1 = objDTO.UDF1;
                    objExistedRoom.UDF2 = objDTO.UDF2;
                    objExistedRoom.UDF3 = objDTO.UDF3;
                    objExistedRoom.UDF4 = objDTO.UDF4;
                    objExistedRoom.UDF5 = objDTO.UDF5;
                    objExistedRoom.NextAssetNo = objDTO.NextAssetNo;
                    objExistedRoom.NextBinNo = objDTO.NextBinNo;
                    objExistedRoom.NextKitNo = objDTO.NextKitNo;
                    objExistedRoom.NextItemNo = objDTO.NextItemNo;
                    objExistedRoom.NextProjectSpendNo = objDTO.NextProjectSpendNo;
                    objExistedRoom.NextToolNo = objDTO.NextToolNo;
                    objExistedRoom.InventoryConsuptionMethod = objDTO.InventoryConsuptionMethod;
                    objExistedRoom.DefaultBinID = objDTO.DefaultBinID;
                    objExistedRoom.DefaultSupplierID = DefaultSupplierId;
                    objExistedRoom.IsRoomActive = objDTO.IsRoomActive;
                    objExistedRoom.RequestedXDays = objDTO.RequestedXDays.GetValueOrDefault(0);
                    objExistedRoom.RequestedYDays = objDTO.RequestedYDays.GetValueOrDefault(0);
                    objExistedRoom.DefaultBinName = objDTO.DefaultBinName;
                    objExistedRoom.eVMIWaitCommand = objDTO.eVMIWaitCommand;
                    objExistedRoom.eVMIWaitPort = objDTO.eVMIWaitPort;
                    objExistedRoom.CountAutoSequence = objDTO.CountAutoSequence;
                    objExistedRoom.ShelfLifeleadtimeOrdRpt = objDTO.ShelfLifeleadtimeOrdRpt;
                    objExistedRoom.LeadTimeOrdRpt = objDTO.LeadTimeOrdRpt;
                    objExistedRoom.PullPurchaseNumberType = objDTO.PullPurchaseNumberType;
                    objExistedRoom.LastPullPurchaseNumberUsed = objDTO.LastPullPurchaseNumberUsed;
                    objExistedRoom.IsAllowRequisitionDuplicate = objDTO.IsAllowRequisitionDuplicate ?? true;
                    objExistedRoom.ExtPhoneNo = objDTO.ExtPhoneNo;
                    objExistedRoom.ReqAutoSequence = objDTO.ReqAutoSequence;
                    objExistedRoom.AllowInsertingItemOnScan = objDTO.AllowInsertingItemOnScan ?? false;
                    objExistedRoom.IsAllowOrderDuplicate = objDTO.IsAllowOrderDuplicate;
                    objExistedRoom.IsAllowWorkOrdersDuplicate = objDTO.IsAllowWorkOrdersDuplicate;
                    objExistedRoom.AllowPullBeyondAvailableQty = objDTO.AllowPullBeyondAvailableQty;
                    objExistedRoom.PullRejectionType = objDTO.PullRejectionType;
                    objExistedRoom.CountAutoNrFixedValue = objDTO.CountAutoNrFixedValue;
                    objExistedRoom.POAutoNrFixedValue = objDTO.POAutoNrFixedValue;
                    objExistedRoom.PullPurchaseNrFixedValue = objDTO.PullPurchaseNrFixedValue;
                    objExistedRoom.ReqAutoNrFixedValue = objDTO.ReqAutoNrFixedValue;
                    objExistedRoom.BillingRoomType = objDTO.BillingRoomType;

                    objExistedRoom.StagingAutoSequence = objDTO.StagingAutoSequence;
                    objExistedRoom.TransferAutoSequence = objDTO.TransferAutoSequence;
                    objExistedRoom.WorkOrderAutoSequence = objDTO.WorkOrderAutoSequence;
                    objExistedRoom.StagingAutoNrFixedValue = objDTO.StagingAutoNrFixedValue;
                    objExistedRoom.TransferAutoNrFixedValue = objDTO.TransferAutoNrFixedValue;
                    objExistedRoom.WorkOrderAutoNrFixedValue = objDTO.WorkOrderAutoNrFixedValue;

                    objExistedRoom.WarnUserOnAssigningNonDefaultBin = objDTO.WarnUserOnAssigningNonDefaultBin;
                    objExistedRoom.MaintenanceDueNoticeDays = objDTO.MaintenanceDueNoticeDays;

                    objExistedRoom.DefaultRequisitionRequiredDays = objDTO.DefaultRequisitionRequiredDays;
                    objExistedRoom.AttachingWOWithRequisition = objDTO.AttachingWOWithRequisition;
                    objExistedRoom.PreventMaxOrderQty = objDTO.PreventMaxOrderQty;
                    objExistedRoom.DefaultCountType = objDTO.DefaultCountType;
                    objExistedRoom.TAOAutoSequence = objDTO.TAOAutoSequence;
                    objExistedRoom.TAOAutoNrFixedValue = objDTO.TAOAutoNrFixedValue;
                    objExistedRoom.NextToolAssetOrderNo = objDTO.NextToolAssetOrderNo;
                    objExistedRoom.AllowToolOrdering = objDTO.AllowToolOrdering;
                    objExistedRoom.IsWOSignatureRequired = objDTO.IsWOSignatureRequired;
                    objExistedRoom.IsIgnoreCreditRule = objDTO.IsIgnoreCreditRule;
                    objExistedRoom.IsAllowOrderCostuom = objDTO.IsAllowOrderCostuom;
                    objExistedRoom.ToolCountAutoSequence = objDTO.ToolCountAutoSequence;
                    objExistedRoom.NextToolCountNo = objDTO.NextToolCountNo;
                    objExistedRoom.SuggestedReturn = objDTO.SuggestedReturn;
                    objExistedRoom.ForceSupplierFilter = objDTO.ForceSupplierFilter;
                    objExistedRoom.DoGroupSupplierQuoteToOrder = objDTO.DoGroupSupplierQuoteToOrder;
                    objExistedRoom.DoSendQuotetoVendor = objDTO.DoSendQuotetoVendor;
                    if (!string.IsNullOrEmpty(objDTO.ReportAppIntent) && !string.IsNullOrWhiteSpace(objDTO.ReportAppIntent))
                    {
                        objExistedRoom.ReportAppIntent = objDTO.ReportAppIntent;
                    }
                    objExistedRoom.IsOrderReleaseNumberEditable = objDTO.IsOrderReleaseNumberEditable;
                    objExistedRoom.QuoteAutoSequence = objDTO.QuoteAutoSequence;
                    objExistedRoom.NextQuoteNo = objDTO.NextQuoteNo;
                    objExistedRoom.QuoteAutoNrFixedValue = objDTO.QuoteAutoNrFixedValue;
                    objExistedRoom.IsAllowQuoteDuplicate = objDTO.IsAllowQuoteDuplicate;
                    objExistedRoom.AllowABIntegration = objDTO.AllowABIntegration;
                    objExistedRoom.AllowOrderCloseAfterDays = objDTO.AllowOrderCloseAfterDays;
                    objExistedRoom.IsELabel = objDTO.IsELabel;
                    objExistedRoom.UserName = objDTO.UserName;
                    objExistedRoom.Password = (objDTO.IsELabel && (objDTO.Password == "" || objDTO.Password == null)) ? objExistedRoom.Password : objDTO.Password;
                    objExistedRoom.CompanyCode = objDTO.CompanyCode;
                    objExistedRoom.StoreCode = objDTO.StoreCode;
                    objExistedRoom.AllowAutoReceiveFromEDI = objDTO.AllowAutoReceiveFromEDI ?? false;
                    context.SaveChanges();
                }

                if (isBillingTypeChanged)
                {
                    //new UserMasterDAL(base.DataBaseName).EnableDisableModuleByRoomBillingType(objExistedRoom.ID);

                }

                if (objDTO.lstRoomModleSettings != null && objDTO.lstRoomModleSettings.Count > 0)
                {
                    SaveRoomModuleSettings(objDTO.lstRoomModleSettings, objDTO.CompanyID, objDTO.ID, objDTO.CreatedBy);
                }

                if (objDTO.ReclassifyAllItems == 1)
                {
                    DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                    objDashboardDAL.ReclassifyAllItems(objExistedRoom.ID, objExistedRoom.CompanyID ?? 0, objExistedRoom.LastUpdatedBy ?? 0);
                }

                return objDTO;
            }
        }

        public void UpdateNextCounterNumber(long RoomID, long CompanyID, long? NextCounterNumber, string fieldName)
        {
            if (NextCounterNumber.GetValueOrDefault(0) > 0)
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@NextCounterNumber", NextCounterNumber), new SqlParameter("@ColumnName", fieldName) };
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    context.Database.ExecuteSqlCommand("exec [UpdateNextCounterNumber] @RoomID,@CompanyID,@ColumnName,@NextCounterNumber", params1);
                }
            }
        }

        public void UpdateNextCounterNumberString(long RoomID, long CompanyID, string NextCounterNumber, string fieldName)
        {
            long _NextCounter = 0;
            long.TryParse(NextCounterNumber, out _NextCounter);
            if (_NextCounter > 0)
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@NextCounterNumber", NextCounterNumber), new SqlParameter("@ColumnName", fieldName) };
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    context.Database.ExecuteSqlCommand("exec [UpdateNextCounterNumberString] @RoomID,@CompanyID,@ColumnName,@NextCounterNumber", params1);
                }
            }
        }

        public RoomModuleSettingsDTO GetRoomModuleSettings(long CompanyId, long RoomId, long ModuleId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RoomModuleSettingsDTO objRoomModuleSetting = (from RMS in context.RoomModuleSettings
                                                              join M in context.ModuleMasters on RMS.ModuleId equals M.ID
                                                              where RMS.CompanyId == CompanyId
                                                                     && RMS.RoomId == RoomId
                                                                     && RMS.ModuleId == ModuleId
                                                              select new RoomModuleSettingsDTO()
                                                              {
                                                                  ID = RMS.ID,
                                                                  CompanyId = RMS.CompanyId,
                                                                  RoomId = RMS.RoomId,
                                                                  ModuleId = RMS.ModuleId,
                                                                  ModuleName = M.ModuleName,
                                                                  PriseSelectionOption = RMS.PriseSelectionOption,
                                                                  CreatedBy = RMS.CreatedBy,
                                                                  CreatedDate = RMS.CreatedDate,
                                                                  LastUpdatedBy = RMS.LastUpdatedBy,
                                                                  LastUpdatedDate = RMS.LastUpdatedDate
                                                              }).FirstOrDefault();
                return objRoomModuleSetting;
            }
        }


        public List<RoomModuleSettingsDTO> GetDummyRoomModuleSettingsDTO(List<long> arrValidModules, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from M in context.ModuleMasters
                        join L in arrValidModules on M.ID equals L
                        where arrValidModules.Contains(M.ID)
                        select new RoomModuleSettingsDTO
                        {
                            ID = 0,
                            CompanyId = CompanyId,
                            RoomId = RoomId,
                            ModuleId = M.ID,
                            ModuleName = M.ModuleName,
                            PriseSelectionOption = 1,
                            CreatedBy = null,
                            CreatedDate = null,
                            LastUpdatedBy = null,
                            LastUpdatedDate = null,
                            resourcekey = M.ResourceKey
                        }
                       ).ToList();
            }
        }

        public bool checkProjectSpendMandatorybyId(Int64 id)
        {
            bool result = false;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool res = (from u in context.Rooms
                            where u.ID == id
                            select new
                            {
                                IsProjectSpendMandatory = u.IsProjectSpendMandatory
                            }
                           ).FirstOrDefault().IsProjectSpendMandatory;
                result = res;
            }

            return result;
        }
        public bool checkRoomConsignedOrNotbyRoomId(Int64 id)
        {
            bool result = false;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var roomData = (from u in context.Rooms
                                where u.ID == id
                                select u).FirstOrDefault();
                if (roomData != null)
                {
                    result = roomData.IsConsignment;
                }

            }

            return result;
        }
        public bool checkRoomAllowOrderCostuomOrNotbyRoomId(Int64 id)
        {
            bool result = false;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var roomData = (from u in context.Rooms
                                where u.ID == id
                                select u).FirstOrDefault();
                if (roomData != null)
                {
                    result = roomData.IsAllowOrderCostuom;
                }

            }

            return result;
        }
        public bool HasConsignedItems(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ItemMasters.Any(ee => ee.Room == id && ee.IsDeleted != true && ee.Consignment == true);
            }
        }
        public bool HasSupplierWithEditableRelease(Int64 id, Int64? companyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.SupplierMasters.Any(ee => ee.Room == id && ee.CompanyID == companyId && ee.IsDeleted == false && ee.IsOrderReleaseNumberEditable == true);
            }
        }
        public bool HasOrderCostUOMItems(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ItemMasters.Any(ee => ee.Room == id && ee.IsDeleted != true && ee.IsAllowOrderCostuom == true);
            }
        }

        public bool HasTrendingSettingItems(long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ItemMasters.Any(ee => ee.Room == RoomID && ee.IsDeleted != true && (ee.TrendingSetting ?? 0) > 0);
            }
        }

        public bool CheckDuplicateREquisition(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var res = (from r in context.RequisitionMasters
                           join r1 in context.RequisitionMasters on r.RequisitionNumber equals r1.RequisitionNumber
                           where r.ID != r1.ID && r.Room == r1.Room && r.CompanyID == r1.CompanyID && r.Room == id
                           && (r.IsDeleted ?? false) == false && (r1.IsDeleted ?? false) == false
                           select r);
                if (res.Any())
                {
                    return true;
                }
                return false;
            }
        }

        public bool CheckDuplicateOrder(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var res = (from r in context.OrderMasters
                           join r1 in context.OrderMasters on r.OrderNumber equals r1.OrderNumber
                           where r.ID != r1.ID && r.Room == r1.Room && r.CompanyID == r1.CompanyID && r.Room == id
                           && r.IsDeleted == false && r1.IsDeleted == false
                           select r);
                if (res.Any())
                {
                    return true;
                }
                return false;
            }
        }

        public bool CheckDuplicateWorkOrder(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var res = (from r in context.WorkOrders
                           join r1 in context.WorkOrders on r.WOName equals r1.WOName
                           where r.ID != r1.ID && r.Room == r1.Room && r.CompanyID == r1.CompanyID && r.Room == id
                           && (r.IsDeleted ?? false) == false && (r1.IsDeleted ?? false) == false
                           select r);
                if (res.Any())
                {
                    return true;
                }
                return false;
            }
        }

        public string RoomDuplicateCheck(long ID, string RoomName, long CompanyID)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.Rooms
                           where em.RoomName == RoomName && em.IsArchived == false && em.IsDeleted == false && em.ID != ID && em.CompanyID == CompanyID
                           select em);
                if (qry.Any())
                {
                    msg = "duplicate";
                }
                else
                {
                    msg = "ok";
                }
            }
            return msg;
        }

        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    string strQuery = "UPDATE Room SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
            //    context.Database.ExecuteSqlCommand(strQuery);
            //}

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {
                                                        new SqlParameter("@ColumnName", columnName),
                                                        new SqlParameter("@Value", value ?? string.Empty),
                                                        new SqlParameter("@ID", id)
                                                        };

                string strQuery = @"EXEC UpdateRoomMaster @ColumnName,@Value,@ID";
                context.Database.ExecuteSqlCommand(strQuery, params1);


            }

            return value;
        }


        public bool checkCreditRuleApplybyId(Int64 id)
        {
            bool result = false;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool res = (from u in context.Rooms
                            where u.ID == id
                            select new
                            {
                                IsIgnoreCreditRule = u.IsIgnoreCreditRule
                            }
                           ).FirstOrDefault().IsIgnoreCreditRule;
                result = res;
            }

            return result;
        }

        public string UpdateNextToolCountNo(Int64 RoomID, Int64 CompanyID, string NextToolCountNo)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Room objRoom = context.Rooms.Where(x => x.ID == RoomID).FirstOrDefault();
                if (objRoom != null)
                {
                    objRoom.NextToolCountNo = NextToolCountNo;
                    context.SaveChanges();
                }
            }
            return NextToolCountNo;
        }

        public RoomDTO GetRoomByIDForModuleAccessList(long RoomID, List<long> arrValidModules = null)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RoomDTO objRoomDTO = GetRoomByIDFull(RoomID);

                if (objRoomDTO != null && arrValidModules != null)
                {
                    objRoomDTO.lstRoomModleSettings = new List<RoomModuleSettingsDTO>();
                    objRoomDTO.lstRoomModleSettings = (from M in context.ModuleMasters
                                                       join rms in context.RoomModuleSettings
                                                       on new { ModuleId = (long?)M.ID, CompanyId = objRoomDTO.CompanyID, RoomId = (long?)objRoomDTO.ID }
                                                       equals new { ModuleId = rms.ModuleId, CompanyId = rms.CompanyId, RoomId = rms.RoomId } into rms_join
                                                       from RMS in rms_join.DefaultIfEmpty()
                                                       where arrValidModules.Contains(M.ID)
                                                       select new RoomModuleSettingsDTO
                                                       {
                                                           ID = (RMS == null ? 0 : RMS.ID),
                                                           CompanyId = objRoomDTO.CompanyID,
                                                           RoomId = objRoomDTO.ID,
                                                           ModuleId = M.ID,
                                                           ModuleName = M.ModuleName,
                                                           PriseSelectionOption = (RMS == null ? 1 : RMS.PriseSelectionOption),
                                                           CreatedBy = (RMS == null ? 0 : RMS.CreatedBy),
                                                           CreatedDate = (RMS == null ? null : RMS.CreatedDate),
                                                           LastUpdatedBy = (RMS == null ? 0 : RMS.LastUpdatedBy),
                                                           LastUpdatedDate = (RMS == null ? null : RMS.LastUpdatedDate)
                                                       }
                                                      ).ToList();
                }

                return objRoomDTO;
            }
        }

        public List<RoomDTO> GetAllRoomWithTimeZoneOffSet(string DBName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<RoomDTO> lstRoomDTO = (from R in context.Rooms
                                            join C in context.CompanyMasters on R.CompanyID equals C.ID
                                            join regset in context.RegionalSettings on R.ID equals regset.RoomId into rs_regset_join
                                            from rs_regset in rs_regset_join.DefaultIfEmpty()
                                            where R.IsDeleted == false && (C.IsDeleted == null || C.IsDeleted == false)
                                            select new RoomDTO()
                                            {
                                                ID = R.ID,
                                                RoomId = R.ID,
                                                CompanyID = R.CompanyID,
                                                intTimeZoneOffSet = rs_regset.TimeZoneOffSet,
                                                TimeZoneName = rs_regset.TimeZoneName,
                                                dtServiceRunTime = DateTime.Now,
                                                IsServiceExecuted = false,
                                                CreatedBy = R.CreatedBy,
                                                EnterpriseDBName = DBName,
                                                AllowInsertingItemOnScan = R.AllowInsertingItemOnScan ?? false,
                                                IsAllowOrderDuplicate = R.IsAllowOrderDuplicate,
                                                IsAllowWorkOrdersDuplicate = R.IsAllowWorkOrdersDuplicate,
                                                AllowPullBeyondAvailableQty = R.AllowPullBeyondAvailableQty,
                                                PullRejectionType = R.PullRejectionType,
                                                BillingRoomType = R.BillingRoomType,
                                                IsAllowOrderCostuom = R.IsAllowOrderCostuom,
                                                IsRoomActive = R.IsRoomActive
                                            }).ToList();

                return lstRoomDTO;
            }
        }

        public List<RoomDTO> GetRoomsByUserAccessAndRoomSupplierFilter(List<UserAccessDTO> UserAccessList)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<RoomDTO> lstAllRooms = (from itm in UserAccessList
                                             join rm in context.Rooms on itm.RoomId equals rm.ID
                                             join usrmst in context.UserSupplierFilters on new { itm.UserId, itm.CompanyId, itm.RoomId } equals new { usrmst.UserId, usrmst.CompanyId, usrmst.RoomId } into rls_usrmst_join
                                             from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                             where ((!rm.ForceSupplierFilter || (rls_usrmst != null && rls_usrmst.ID > 0)))
                                             orderby itm.IsRoomActive descending, itm.RoomName ascending
                                             group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.RoomId, itm.RoomName, itm.IsRoomActive, itm.isEVMI } into gropedentrprs
                                             select new RoomDTO
                                             {
                                                 EnterpriseId = gropedentrprs.Key.EnterpriseId,
                                                 EnterpriseName = gropedentrprs.Key.EnterpriseName,
                                                 CompanyID = gropedentrprs.Key.CompanyId,
                                                 CompanyName = gropedentrprs.Key.CompanyName,
                                                 ID = gropedentrprs.Key.RoomId,
                                                 RoomName = gropedentrprs.Key.RoomName,
                                                 IsRoomActive = gropedentrprs.Key.IsRoomActive,
                                                 IseVMI = gropedentrprs.Key.isEVMI
                                             }).ToList();

                return lstAllRooms;
            }

        }

        public List<RoomDTO> GetRoomsByUserAccessAndRoomSupplierFilterForSuperAdmin(List<UserAccessDTO> UserAccessList, Dictionary<long, string> EnterpriseList)
        {
            List<RoomDTO> lstAllRooms = new List<RoomDTO>();

            foreach (KeyValuePair<long, string> enterprise in EnterpriseList.Where(e => !string.IsNullOrEmpty(e.Value)))
            {
                using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(enterprise.Value, DbConnectionType.EFReadWrite.ToString("F"))))
                {
                    var tmpUserAccessList = UserAccessList.Where(e => e.EnterpriseId == enterprise.Key).ToList();

                    List<RoomDTO> rooms = (from itm in tmpUserAccessList
                                           join rm in context.Rooms on itm.RoomId equals rm.ID
                                           join usrmst in context.UserSupplierFilters on new { itm.UserId, itm.CompanyId, itm.RoomId } equals new { usrmst.UserId, usrmst.CompanyId, usrmst.RoomId } into rls_usrmst_join
                                           from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                           where ((!rm.ForceSupplierFilter || (rls_usrmst != null && rls_usrmst.ID > 0)))
                                           orderby itm.IsRoomActive descending, itm.RoomName ascending
                                           group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.RoomId, itm.RoomName, itm.IsRoomActive, itm.isEVMI } into gropedentrprs
                                           select new RoomDTO
                                           {
                                               EnterpriseId = gropedentrprs.Key.EnterpriseId,
                                               EnterpriseName = gropedentrprs.Key.EnterpriseName,
                                               CompanyID = gropedentrprs.Key.CompanyId,
                                               CompanyName = gropedentrprs.Key.CompanyName,
                                               ID = gropedentrprs.Key.RoomId,
                                               RoomName = gropedentrprs.Key.RoomName,
                                               IsRoomActive = gropedentrprs.Key.IsRoomActive,
                                               IseVMI = gropedentrprs.Key.isEVMI
                                           }).ToList();

                    lstAllRooms.AddRange(rooms);
                }
            }
            return lstAllRooms;
        }


        public RoomDTO GetRoomByIDPlain(long RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("exec [GetRoomByIDPlain] @RoomID", params1).FirstOrDefault();
            }
        }


        public List<RoomDTO> GetRoomByIDsNormal(string strRoomIDs)
        {

            var params1 = new SqlParameter[] { new SqlParameter("@RoomIDs", strRoomIDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("exec [GetRoomByIDsNormal] @RoomIDs", params1).ToList();
            }
        }

        public RoomDTO GetRoomByIDNormal(long RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("exec [GetRoomByIDNormal] @RoomID", params1).FirstOrDefault();
            }
        }

        public RoomDTO GetRoomByIDFull(long RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("exec [GetRoomByIDFull] @RoomID", params1).FirstOrDefault();
            }
        }

        public RoomDTO GetRoomByNamePlain(long CompanyID, string RoomName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomName", RoomName), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("exec [GetRoomByNamePlain] @RoomName,@CompanyID", params1).FirstOrDefault();
            }
        }

        //public RoomDTO GetRoomByNameNormal(string RoomName, long CompanyID)
        //{
        //    var params1 = new SqlParameter[] { new SqlParameter("@RoomName", RoomName), new SqlParameter("@CompanyID", CompanyID) };
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        return context.Database.SqlQuery<RoomDTO>("exec [GetRoomByNameNormal] @RoomName,@CompanyID", params1).FirstOrDefault();
        //    }
        //}

        //public List<RoomDTO> GetRoomByIDsPlain(string RoomIDs, long CompanyID)
        //{
        //    var params1 = new SqlParameter[] { new SqlParameter("@RoomIDs", RoomIDs), new SqlParameter("@CompanyID", CompanyID) };
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        return context.Database.SqlQuery<RoomDTO>("exec [GetRoomByIDsPlain] @RoomIDs,@CompanyID", params1).ToList();
        //    }
        //}

        //public List<RoomDTO> GetRoomByIDsNormal(string RoomIDs, long CompanyID)
        //{
        //    var params1 = new SqlParameter[] { new SqlParameter("@RoomIDs", RoomIDs), new SqlParameter("@CompanyID", CompanyID) };
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        return context.Database.SqlQuery<RoomDTO>("exec [GetRoomByIDsNormal] @RoomIDs,@CompanyID", params1).ToList();
        //    }
        //}
        public eVMISiteSettings GetSensorBinRoomSettings(long RoomId, long EnterpriseId, long CompanyId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomId),
                                               new SqlParameter("@EnterpriseId", EnterpriseId), 
                                               new SqlParameter("@CompanyId", CompanyId) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<eVMISiteSettings>("exec [GetSensorBinRoomSettings] @RoomId,@CompanyID,@EnterpriseId", params1).FirstOrDefault();
            }
        }

        public List<RoomDTO> GetRoomByCompanyIDPlain(long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("exec [GetRoomByCompanyIDPlain] @CompanyID", params1).ToList();
            }
        }

        //public List<RoomDTO> GetRoomByCompanyIDNormal(long CompanyID)
        //{
        //    var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        return context.Database.SqlQuery<RoomDTO>("exec [GetRoomByCompanyIDNormal] @CompanyID", params1).ToList();
        //    }
        //}

        public List<RoomDTO> GetRoomByCompanyIDsPlain(string strCompanyIDs)
        {

            var params1 = new SqlParameter[] { new SqlParameter("@CompanyIDs", strCompanyIDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("exec [GetRoomByCompanyIDsPlain] @CompanyIDs", params1).ToList();
            }
        }


        public List<RoomDTO> GetRoomByCompanyAndRoomList(string CompanyIDs, string RoomNames)
        {
            List<RoomDTO> objRoomList = GetRoomByCompanyIDsPlain(CompanyIDs).ToList();
            List<string> lstRoomName = RoomNames.Split(',').ToList();
            return objRoomList.Where(x => lstRoomName.Contains(x.RoomName)).ToList();

        }




        #endregion




        public List<RoomDTO> GetPagedRooms(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm,
            string sortColumnName, bool IsArchived, bool IsDeleted, long CompanyId, string UserRooms, string RoomDateFormat,
            TimeZoneInfo CurrentTimeZone, List<CompanyMasterDTO> CompanyList, Int64 RoleID, List<EnterpriseDTO> EnterPriseList,
            DataTable RoomResourcesTable)
        {
            List<RoomDTO> lstRooms = new List<RoomDTO>();
            TotalCount = 0;
            RoomDTO objRoomMaster = new RoomDTO();
            DataSet dsRooms = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstRooms;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string RoomCreaters = null;
            string RoomUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string EnterpriseIds = null;
            string CompanyIds = null;
            string RoomStatus = null;
            string BillingRoomTypes = null;
            string InvoiceBranches = null;

            List<CompanyMasterDTO> oCompanyList = CompanyList;
            string UserCompanies = string.Empty;
            if (oCompanyList == null)
                oCompanyList = new List<CompanyMasterDTO>();

            List<EnterpriseDTO> oEnterPriseList = EnterPriseList;
            string UserEnterPrises = string.Empty;
            if (oEnterPriseList == null)
                oEnterPriseList = new List<EnterpriseDTO>();

            long RoleId = -1;
            long.TryParse(Convert.ToString(RoleID), out RoleId);

            if (RoleId != -1)
            {
                UserCompanies = string.Join(",", oCompanyList.Select(t => t.ID).ToArray());
                UserEnterPrises = string.Join(",", oEnterPriseList.Select(t => t.ID).ToArray());
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                CompanyIds = UserCompanies;
                EnterpriseIds = UserEnterPrises;
                dsRooms = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedRooms", StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomCreaters, RoomUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, CompanyId, UserRooms, EnterpriseIds, CompanyIds, RoomStatus, BillingRoomTypes, InvoiceBranches, RoomResourcesTable);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }

                //SearchTerm = string.Empty;
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    RoomCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    RoomUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    //CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    //UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    /// UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[56]))
                {
                    EnterpriseIds = FieldsPara[56].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[57]))
                {
                    CompanyIds = FieldsPara[57].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(UserCompanies))
                {
                    if (!string.IsNullOrWhiteSpace(CompanyIds))
                    {
                        var selected = (from c in CompanyIds.Split(',')
                                        join nc in UserCompanies.Split(',') on c equals nc
                                        select c);

                        CompanyIds = string.Join(",", selected);
                        if (string.IsNullOrWhiteSpace(CompanyIds))
                            CompanyIds = "0";
                    }
                    else
                    {
                        CompanyIds = UserCompanies;
                    }
                }

                if (!string.IsNullOrWhiteSpace(UserEnterPrises))
                {
                    if (!string.IsNullOrWhiteSpace(EnterpriseIds))
                    {
                        var selected = (from c in EnterpriseIds.Split(',')
                                        join nc in UserEnterPrises.Split(',') on c equals nc
                                        select c);

                        EnterpriseIds = string.Join(",", selected);
                        if (string.IsNullOrWhiteSpace(EnterpriseIds))
                            EnterpriseIds = "0";
                    }
                    else
                    {
                        EnterpriseIds = UserEnterPrises;
                    }
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[70]))
                {
                    RoomStatus = FieldsPara[70].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[103]))
                {
                    InvoiceBranches = FieldsPara[103].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[102]))
                {
                    BillingRoomTypes = FieldsPara[102].TrimEnd(',');
                }

                dsRooms = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedRooms", StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomCreaters, RoomUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, CompanyId, UserRooms, EnterpriseIds, CompanyIds, RoomStatus, BillingRoomTypes, InvoiceBranches, RoomResourcesTable);
            }
            else
            {
                CompanyIds = UserCompanies;
                EnterpriseIds = UserEnterPrises;
                dsRooms = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedRooms", StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomCreaters, RoomUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, CompanyId, UserRooms, EnterpriseIds, CompanyIds, RoomStatus, BillingRoomTypes, InvoiceBranches, RoomResourcesTable);
            }

            if (dsRooms != null && dsRooms.Tables.Count > 0)
            {
                DataTable dtRooms = dsRooms.Tables[0];
                if (dtRooms.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtRooms.Rows[0]["TotalRecords"]);
                }
                lstRooms = (from rm in dtRooms.AsEnumerable()
                            select new RoomDTO
                            {
                                ID = rm.Field<long>("ID"),
                                RoomName = rm.Field<string>("RoomName"),
                                CompanyName = rm.Field<string>("CompanyName"),
                                ContactName = rm.Field<string>("ContactName"),
                                streetaddress = rm.Field<string>("StreetAddress"),
                                City = rm.Field<string>("City"),
                                State = rm.Field<string>("State"),
                                PostalCode = rm.Field<string>("PostalCode"),
                                Country = rm.Field<string>("Country"),
                                PhoneNo = rm.Field<string>("ExtPhoneNo") != null ? rm.Field<string>("PhoneNo") + " x " + rm.Field<string>("ExtPhoneNo") : rm.Field<string>("PhoneNo"),
                                Email = rm.Field<string>("Email"),
                                InvoiceBranch = rm.Field<string>("InvoiceBranch"),
                                CustomerNumber = rm.Field<string>("CustomerNumber"),
                                BlanketPO = rm.Field<string>("BlanketPO"),
                                IsConsignment = rm.Field<bool>("IsConsignment"),
                                IseVMI = rm.Field<bool>("IseVMI"),
                                IsTax1Parts = rm.Field<bool>("IsTax1Parts"),
                                IsTax1Labor = rm.Field<bool>("IsTax1Labor"),
                                Tax1name = rm.Field<string>("Tax1Name"),
                                Tax1Rate = rm.Field<double?>("Tax1Rate"),
                                IsTax2Parts = rm.Field<bool>("IsTax2Parts"),
                                IsTax2Labor = rm.Field<bool>("IsTax2Labor"),
                                tax2name = rm.Field<string>("Tax2Name"),
                                Tax2Rate = rm.Field<double?>("Tax2Rate"),
                                ReplineshmentRoom = rm.Field<int?>("ReplineshmentRoom"),
                                IsTrending = rm.Field<bool>("IsTrending"),
                                SourceOfTrending = rm.Field<int?>("SourceOfTrending"),
                                TrendingFormula = rm.Field<string>("TrendingFormula"),
                                TrendingFormulaType = rm.Field<int?>("TrendingFormulaType"),
                                TrendingFormulaDays = rm.Field<int?>("TrendingFormulaDays"),
                                TrendingFormulaOverDays = rm.Field<int?>("TrendingFormulaOverDays"),
                                SuggestedOrder = rm.Field<bool>("SuggestedOrder"),
                                SuggestedTransfer = rm.Field<bool>("SuggestedTransfer"),
                                MethodOfValuingInventory = rm.Field<string>("MethodOfValuingInventory"),
                                AutoCreateTransferFrequency = rm.Field<string>("AutoCreateTransferFrequency"),
                                AutoCreateTransferTime = rm.Field<string>("AutoCreateTransferTime"),
                                AutoCreateTransferSubmit = rm.Field<bool>("AutoCreateTransferSubmit"),
                                IsActive = rm.Field<bool>("IsActive"),
                                LicenseBilled = (rm.Field<DateTime?>("LicenseBilled") != null && rm.Field<DateTime?>("LicenseBilled").Value.Year != 1951) ? rm.Field<DateTime?>("LicenseBilled") : (DateTime?)null,
                                NextCountNo = rm.Field<string>("NextCountNo"),
                                NextOrderNo = rm.Field<string>("NextOrderNo"),
                                NextRequisitionNo = rm.Field<long?>("NextRequisitionNo"),
                                NextStagingNo = rm.Field<long?>("NextStagingNo"),
                                NextTransferNo = rm.Field<long?>("NextTransferNo"),
                                NextWorkOrderNo = rm.Field<long?>("NextWorkOrderNo"),
                                RoomGrouping = rm.Field<string>("RoomGrouping"),
                                Created = ((rm.Field<DateTime?>("Created") ?? DateTime.MinValue).Year != 1951) ? rm.Field<DateTime?>("Created") : DateTime.MinValue,
                                Updated = (rm.Field<DateTime?>("Updated") != null && rm.Field<DateTime?>("Updated").Value.Year != 1951) ? rm.Field<DateTime?>("Updated") : (DateTime?)null,
                                CreatedBy = rm.Field<long?>("CreatedBy"),
                                LastUpdatedBy = rm.Field<long?>("LastUpdatedBy"),
                                IsDeleted = rm.Field<bool?>("IsDeleted"),
                                MaxOrderSize = rm.Field<double?>("MaxOrderSize"),
                                HighPO = rm.Field<string>("HighPO"),
                                HighJob = rm.Field<string>("HighJob"),
                                HighTransfer = rm.Field<string>("HighTransfer"),
                                HighCount = rm.Field<string>("HighCount"),
                                GlobMarkupParts = rm.Field<double?>("GlobMarkupParts"),
                                GlobMarkupLabor = rm.Field<double?>("GlobMarkupLabor"),
                                UniqueID = rm.Field<string>("UniqueID"),
                                CostCenter = rm.Field<string>("CostCenter"),
                                GXPRConsJob = rm.Field<string>("GXPRConsJob"),
                                IsArchived = rm.Field<bool?>("IsArchived"),
                                GUID = rm.Field<Guid>("GUID"),
                                IsTax2onTax1 = rm.Field<bool?>("IsTax2onTax1") ?? false,
                                TrendingFormulaAvgDays = rm.Field<int?>("TrendingFormulaAvgDays"),
                                TrendingFormulaCounts = rm.Field<int?>("TrendingFormulaCounts"),
                                TransferFrequencyOption = rm.Field<int?>("TransferFrequencyOption"),
                                TransferFrequencyDays = rm.Field<string>("TransferFrequencyDays"),
                                TransferFrequencyMonth = rm.Field<int?>("TransferFrequencyMonth"),
                                TransferFrequencyNumber = rm.Field<int?>("TransferFrequencyNumber"),
                                TransferFrequencyWeek = rm.Field<int?>("TransferFrequencyWeek"),
                                TransferFrequencyMainOption = rm.Field<int?>("TransferFrequencyMainOption"),
                                TrendingSampleSize = rm.Field<int?>("TrendingSampleSize"),
                                TrendingSampleSizeDivisor = rm.Field<int?>("TrendingSampleSizeDivisor"),
                                AverageUsageSampleSize = rm.Field<int?>("AverageUsageSampleSize"),
                                AverageUsageSampleSizeDivisor = rm.Field<int?>("AverageUsageSampleSizeDivisor"),
                                AverageUsageTransactions = rm.Field<int?>("AverageUsageTransactions"),
                                UDF1 = rm.Field<string>("UDF1"),
                                UDF2 = rm.Field<string>("UDF2"),
                                UDF3 = rm.Field<string>("UDF3"),
                                UDF4 = rm.Field<string>("UDF4"),
                                UDF5 = rm.Field<string>("UDF5"),
                                UDF6 = rm.Field<string>("UDF6"),
                                UDF7 = rm.Field<string>("UDF7"),
                                UDF8 = rm.Field<string>("UDF8"),
                                UDF9 = rm.Field<string>("UDF9"),
                                UDF10 = rm.Field<string>("UDF10"),
                                CompanyID = rm.Field<long?>("CompanyID"),
                                DefaultSupplierID = rm.Field<long?>("DefaultSupplierID"),
                                NextAssetNo = rm.Field<long?>("NextAssetNo"),
                                NextBinNo = rm.Field<long?>("NextBinNo"),
                                NextKitNo = rm.Field<long?>("NextKitNo"),
                                NextItemNo = rm.Field<long?>("NextItemNo"),
                                NextProjectSpendNo = rm.Field<long?>("NextProjectSpendNo"),
                                NextToolNo = rm.Field<long?>("NextToolNo"),
                                InventoryConsuptionMethod = rm.Field<string>("InventoryConsuptionMethod"),
                                ReplenishmentType = rm.Field<string>("ReplenishmentType"),
                                DefaultBinID = rm.Field<long?>("DefaultBinID"),
                                IsRoomActive = rm.Field<bool>("IsRoomActive"),
                                RequestedXDays = rm.Field<int?>("RequestedXDays"),
                                RequestedYDays = rm.Field<int?>("RequestedYDays"),
                                IsProjectSpendMandatory = rm.Field<bool>("IsProjectSpendMandatory"),
                                POAutoSequence = rm.Field<int?>("POAutoSequence"),
                                IsAverageUsageBasedOnPull = rm.Field<bool>("IsAverageUsageBasedOnPull"),
                                SlowMovingValue = rm.Field<double>("SlowMovingValue"),
                                FastMovingValue = rm.Field<double>("FastMovingValue"),
                                CreatedByName = rm.Field<string>("CreatedByName"),
                                UpdatedByName = rm.Field<string>("UpdatedByName"),
                                DefaultLocationName = rm.Field<string>("DefaultLocationName"),
                                ReplineshmentRoomName = rm.Field<string>("ReplineshmentRoomName"),
                                LastOrderDate = (rm.Field<DateTime?>("LastOrderDate") != null && rm.Field<DateTime?>("LastOrderDate").Value.Year != 1951) ? rm.Field<DateTime?>("LastOrderDate") : (DateTime?)null,
                                LastPullDate = (rm.Field<DateTime?>("LastPullDate") != null && rm.Field<DateTime?>("LastPullDate").Value.Year != 1951) ? rm.Field<DateTime?>("LastPullDate") : (DateTime?)null,
                                eVMIWaitCommand = rm.Field<Int64>("eVMIWaitCommand"),
                                eVMIWaitPort = rm.Field<Int64>("eVMIWaitPort"),
                                CountAutoSequence = rm.Field<int?>("CountAutoSequence"),
                                EnterpriseId = rm.Field<long>("EnterpriseId"),
                                EnterpriseName = rm.Field<string>("EnterpriseName"),
                                ActiveOn = (rm.Field<DateTime?>("ActiveOn") != null && rm.Field<DateTime?>("ActiveOn").Value.Year != 1951) ? rm.Field<DateTime>("ActiveOn") : (DateTime?)null,
                                //LastSyncDateTime = rm.Field<DateTime?>("LastSyncDateTime"),
                                LastSyncDateTime = (rm.Field<DateTime?>("LastSyncDateTime") != null && rm.Field<DateTime?>("LastSyncDateTime").Value.Year != 1951) ? rm.Field<DateTime?>("LastSyncDateTime") : (DateTime?)null,
                                PDABuildVersion = rm.Field<string>("PDABuildVersion"),
                                LastSyncUserName = rm.Field<string>("LastSyncUserName"),
                                IsAllowOrderDuplicate = rm.Field<bool>("IsAllowOrderDuplicate"),
                                IsAllowRequisitionDuplicate = rm.Field<bool?>("IsAllowRequisitionDuplicate") ?? true,
                                IsAllowWorkOrdersDuplicate = rm.Field<bool>("IsAllowWorkOrdersDuplicate"),
                                LastReceivedDate = (rm.Field<DateTime?>("LastPullDate") != null && rm.Field<DateTime?>("LastPullDate").Value.Year != 1951) ? rm.Field<DateTime?>("LastReceivedDate") : (DateTime?)null,
                                LastTrasnferedDate = (rm.Field<DateTime?>("LastTrasnferedDate") != null && rm.Field<DateTime?>("LastTrasnferedDate").Value.Year != 1951) ? rm.Field<DateTime?>("LastTrasnferedDate") : (DateTime?)null,
                                ReqAutoSequence = rm.Field<int?>("ReqAutoSequence"),
                                //ActiveOnDateStr = rm.Field<string>("ActiveOnDateStr"),
                                AllowInsertingItemOnScan = rm.Field<bool?>("AllowInsertingItemOnScan") ?? false,
                                BillingRoomType = rm.Field<int?>("BillingRoomType"),

                                StagingAutoSequence = rm.Field<int?>("StagingAutoSequence"),
                                TransferAutoSequence = rm.Field<int?>("TransferAutoSequence"),
                                WorkOrderAutoSequence = rm.Field<int?>("WorkOrderAutoSequence"),
                                WarnUserOnAssigningNonDefaultBin = rm.Field<bool>("WarnUserOnAssigningNonDefaultBin"),
                                MaintenanceDueNoticeDays = rm.Field<int?>("MaintenanceDueNoticeDays"),
                                DefaultRequisitionRequiredDays = rm.Field<int?>("DefaultRequisitionRequiredDays"),
                                AttachingWOWithRequisition = rm.Field<int?>("AttachingWOWithRequisition"),
                                PreventMaxOrderQty = rm.Field<int>("PreventMaxOrderQty"),
                                DefaultCountType = rm.Field<string>("DefaultCountType"),
                                TAOAutoSequence = rm.Field<int?>("TAOAutoSequence"),
                                TAOAutoNrFixedValue = rm.Field<string>("TAOAutoNrFixedValue"),
                                NextToolAssetOrderNo = rm.Field<string>("NextToolAssetOrderNo"),
                                AllowToolOrdering = rm.Field<bool>("AllowToolOrdering"),
                                IsWOSignatureRequired = rm.Field<bool>("IsWOSignatureRequired"),
                                IsIgnoreCreditRule = rm.Field<bool>("IsIgnoreCreditRule"),
                                IsAllowOrderCostuom = rm.Field<bool>("IsAllowOrderCostuom"),
                                ToolCountAutoSequence = rm.Field<int?>("ToolCountAutoSequence"),
                                NextToolCountNo = rm.Field<string>("NextToolCountNo"),
                                SuggestedReturn = rm.Field<bool>("SuggestedReturn"),
                                ForceSupplierFilter = rm.Field<bool>("ForceSupplierFilter"),
                                NoOfItems = rm.Field<int?>("NoOfItems"),
                                ReportAppIntent = rm.Field<string>("ReportAppIntent"),
                                DefaultCountTypeName = rm.Field<string>("DefaultCountTypeName"),
                                PreventMaxOrderQtyName = rm.Field<string>("PreventMaxOrderQtyName"),
                                AttachingWOWithRequisitionName = rm.Field<string>("AttachingWOWithRequisitionName"),
                                BaseOfInventoryName = rm.Field<string>("BaseOfInventoryName"),
                                MethodOfValuingInventoryName = rm.Field<string>("MethodOfValuingInventoryName"),
                                ReplenishmentTypeName = rm.Field<string>("ReplenishmentTypeName"),
                                PullRejectionTypeName = rm.Field<string>("PullRejectionTypeName"),
                                ToolCountAutoSequenceName = rm.Field<string>("ToolCountAutoSequenceName"),
                                TAOAutoSequenceName = rm.Field<string>("TAOAutoSequenceName"),
                                WorkOrderAutoSequenceName = rm.Field<string>("WorkOrderAutoSequenceName"),
                                TransferAutoSequenceName = rm.Field<string>("TransferAutoSequenceName"),
                                StagingAutoSequenceName = rm.Field<string>("StagingAutoSequenceName"),
                                ReqAutoSequenceName = rm.Field<string>("ReqAutoSequenceName"),
                                PullPurchaseNumberTypeName = rm.Field<string>("PullPurchaseNumberTypeName"),
                                POAutoSequenceName = rm.Field<string>("POAutoSequenceName"),
                                CountAutoSequenceName = rm.Field<string>("CountAutoSequenceName"),
                                BillingRoomTypeName = rm.Field<string>("BillingRoomTypeName"),
                                PullPurchaseNrFixedValue = rm.Field<string>("PullPurchaseNrFixedValue"),
                                POAutoNrFixedValue = rm.Field<string>("POAutoNrFixedValue"),
                                LastPullPurchaseNumberUsed = rm.Field<string>("LastPullPurchaseNumberUsed"),
                                StagingAutoNrFixedValue = rm.Field<string>("StagingAutoNrFixedValue"),
                                TransferAutoNrFixedValue = rm.Field<string>("TransferAutoNrFixedValue"),
                                WorkOrderAutoNrFixedValue = rm.Field<string>("WorkOrderAutoNrFixedValue"),
                                //DefaultBinName = rm.Field<string>("DefaultLocationName"),
                                AllowPullBeyondAvailableQty = rm.Field<bool>("AllowPullBeyondAvailableQty"),
                                ShelfLifeleadtimeOrdRpt = rm.Field<int?>("ShelfLifeleadtimeOrdRpt"),
                                LeadTimeOrdRpt = rm.Field<int?>("LeadTimeOrdRpt"),
                                NextQuoteNo = rm.Field<string>("NextQuoteNo"),
                                IsAllowQuoteDuplicate = rm.Field<bool>("IsAllowQuoteDuplicate"),
                                QuoteAutoSequence = rm.Field<int?>("QuoteAutoSequence"),
                                QuoteAutoSequenceName = rm.Field<string>("QuoteAutoSequenceName"),
                                QuoteAutoNrFixedValue = rm.Field<string>("QuoteAutoNrFixedValue"),
                                DoGroupSupplierQuoteToOrder = rm.Field<bool>("DoGroupSupplierQuoteToOrder"),
                                DoSendQuotetoVendor = rm.Field<bool>("DoSendQuotetoVendor"),
                                AllowABIntegration = rm.Field<bool>("AllowABIntegration"),
                                AllowOrderCloseAfterDays = rm.Field<int?>("AllowOrderCloseAfterDays"),
                                IsELabel = rm.Field<bool>("IsELabel"),
                                UserName = rm.Field<string>("UserName"),
                                Password = rm.Field<string>("Password"),
                                CompanyCode = rm.Field<string>("CompanyCode"),
                                StoreCode = rm.Field<string>("StoreCode")
                            }).ToList();
            }
            return lstRooms;
        }


        public List<RoomDTO> GetAllRoomsFromETurnsMaster(long CompanyId, bool IsDeleted, bool IsArchived, List<RoomDTO> lstRoomsIds, string EnterpriseIds, Int64 RoleID, Int64 Session_EnterPriceID)
        {
            string UserRooms = string.Empty;
            if (lstRoomsIds == null)
            {
                lstRoomsIds = new List<RoomDTO>();
            }

            long RoleId = -1;
            long.TryParse(Convert.ToString(RoleID), out RoleId);

            if (RoleId != -1)
                UserRooms = string.Join(",", lstRoomsIds.Select(t => t.ID).ToArray());
            if (RoleId != -1 && string.IsNullOrWhiteSpace(UserRooms))
            {
                UserRooms = "0,0";
            }

            if (RoleId == -2)
            {
                long EnterPriceID = -1;
                long.TryParse(Convert.ToString(Session_EnterPriceID), out EnterPriceID);
                EnterpriseIds = EnterPriceID.ToString();
                UserRooms = string.Empty;
            }

            List<RoomDTO> lstRooms = new List<RoomDTO>();
            var params1 = new SqlParameter[] { new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@UserRooms", UserRooms ?? (object)DBNull.Value), new SqlParameter("@EnterpriseIds", EnterpriseIds ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseETurnsMasterEntityConnectionString))
            {
                lstRooms = context.Database.SqlQuery<RoomDTO>("EXEC dbo.GetAllRooms @IsDeleted,@IsArchived,@UserRooms,@EnterpriseIds", params1).ToList();

            }
            return lstRooms;
        }

        public List<RoomHistoryDTO> GetRoomHistoryByIDsNormal(string IDs, long CompanyID, long EnterPriseID)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", Convert.ToInt32(IDs)), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@EnterpriseId", EnterPriseID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomHistoryDTO>("exec [GetRoomHistoryByIDsNormal] @IDs,@CompanyID,@EnterpriseId", params1).ToList();
            }
        }

        public RoomDTO GetRoomByNameOnlyPlain(string RoomName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomName", RoomName ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("exec [GetRoomByNameOnlyPlain] @RoomName", params1).FirstOrDefault();
            }
        }

        public List<RoomDTO> GetDeletedRoomByIDs(string strRoomIDs)
        {
            List<RoomDTO> lstRooms = new List<RoomDTO>();
            RoomDTO objRoomMaster = new RoomDTO();
            DataSet dsRooms = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstRooms;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            dsRooms = SqlHelper.ExecuteDataset(EturnsConnection, "GetDeletedRoomByIDs", strRoomIDs);

            if (dsRooms != null && dsRooms.Tables.Count > 0)
            {
                DataTable dtRooms = dsRooms.Tables[0];
                lstRooms = (from rm in dtRooms.AsEnumerable()
                            select new RoomDTO
                            {
                                ID = rm.Field<long>("ID"),
                                RoomName = rm.Field<string>("RoomName"),
                                CompanyName = rm.Field<string>("CompanyName"),
                                ContactName = rm.Field<string>("ContactName"),
                                streetaddress = rm.Field<string>("StreetAddress"),
                                City = rm.Field<string>("City"),
                                State = rm.Field<string>("State"),
                                PostalCode = rm.Field<string>("PostalCode"),
                                Country = rm.Field<string>("Country"),
                                PhoneNo = rm.Field<string>("ExtPhoneNo") != null ? rm.Field<string>("PhoneNo") + " x " + rm.Field<string>("ExtPhoneNo") : rm.Field<string>("PhoneNo"),
                                Email = rm.Field<string>("Email"),
                                InvoiceBranch = rm.Field<string>("InvoiceBranch"),
                                CustomerNumber = rm.Field<string>("CustomerNumber"),
                                BlanketPO = rm.Field<string>("BlanketPO"),
                                IsConsignment = rm.Field<bool>("IsConsignment"),
                                IseVMI = rm.Field<bool>("IseVMI"),
                                IsTax1Parts = rm.Field<bool>("IsTax1Parts"),
                                IsTax1Labor = rm.Field<bool>("IsTax1Labor"),
                                Tax1name = rm.Field<string>("Tax1Name"),
                                Tax1Rate = rm.Field<double?>("Tax1Rate"),
                                IsTax2Parts = rm.Field<bool>("IsTax2Parts"),
                                IsTax2Labor = rm.Field<bool>("IsTax2Labor"),
                                tax2name = rm.Field<string>("Tax2Name"),
                                Tax2Rate = rm.Field<double?>("Tax2Rate"),
                                ReplineshmentRoom = rm.Field<int?>("ReplineshmentRoom"),
                                IsTrending = rm.Field<bool>("IsTrending"),
                                SourceOfTrending = rm.Field<int?>("SourceOfTrending"),
                                TrendingFormula = rm.Field<string>("TrendingFormula"),
                                TrendingFormulaType = rm.Field<int?>("TrendingFormulaType"),
                                TrendingFormulaDays = rm.Field<int?>("TrendingFormulaDays"),
                                TrendingFormulaOverDays = rm.Field<int?>("TrendingFormulaOverDays"),
                                SuggestedOrder = rm.Field<bool>("SuggestedOrder"),
                                SuggestedTransfer = rm.Field<bool>("SuggestedTransfer"),
                                MethodOfValuingInventory = rm.Field<string>("MethodOfValuingInventory"),
                                AutoCreateTransferFrequency = rm.Field<string>("AutoCreateTransferFrequency"),
                                AutoCreateTransferTime = rm.Field<string>("AutoCreateTransferTime"),
                                AutoCreateTransferSubmit = rm.Field<bool>("AutoCreateTransferSubmit"),
                                IsActive = rm.Field<bool>("IsActive"),
                                LicenseBilled = (rm.Field<DateTime?>("LicenseBilled") != null && rm.Field<DateTime?>("LicenseBilled").Value.Year != 1951) ? rm.Field<DateTime?>("LicenseBilled") : (DateTime?)null,
                                NextCountNo = rm.Field<string>("NextCountNo"),
                                NextOrderNo = rm.Field<string>("NextOrderNo"),
                                NextRequisitionNo = rm.Field<long?>("NextRequisitionNo"),
                                NextStagingNo = rm.Field<long?>("NextStagingNo"),
                                NextTransferNo = rm.Field<long?>("NextTransferNo"),
                                NextWorkOrderNo = rm.Field<long?>("NextWorkOrderNo"),
                                RoomGrouping = rm.Field<string>("RoomGrouping"),
                                Created = ((rm.Field<DateTime?>("Created") ?? DateTime.MinValue).Year != 1951) ? rm.Field<DateTime?>("Created") : DateTime.MinValue,
                                Updated = (rm.Field<DateTime?>("Updated") != null && rm.Field<DateTime?>("Updated").Value.Year != 1951) ? rm.Field<DateTime?>("Updated") : (DateTime?)null,
                                CreatedBy = rm.Field<long?>("CreatedBy"),
                                LastUpdatedBy = rm.Field<long?>("LastUpdatedBy"),
                                IsDeleted = rm.Field<bool?>("IsDeleted"),
                                MaxOrderSize = rm.Field<double?>("MaxOrderSize"),
                                HighPO = rm.Field<string>("HighPO"),
                                HighJob = rm.Field<string>("HighJob"),
                                HighTransfer = rm.Field<string>("HighTransfer"),
                                HighCount = rm.Field<string>("HighCount"),
                                GlobMarkupParts = rm.Field<double?>("GlobMarkupParts"),
                                GlobMarkupLabor = rm.Field<double?>("GlobMarkupLabor"),
                                UniqueID = rm.Field<string>("UniqueID"),
                                CostCenter = rm.Field<string>("CostCenter"),
                                GXPRConsJob = rm.Field<string>("GXPRConsJob"),
                                IsArchived = rm.Field<bool?>("IsArchived"),
                                GUID = rm.Field<Guid>("GUID"),
                                IsTax2onTax1 = rm.Field<bool?>("IsTax2onTax1") ?? false,
                                TrendingFormulaAvgDays = rm.Field<int?>("TrendingFormulaAvgDays"),
                                TrendingFormulaCounts = rm.Field<int?>("TrendingFormulaCounts"),
                                TransferFrequencyOption = rm.Field<int?>("TransferFrequencyOption"),
                                TransferFrequencyDays = rm.Field<string>("TransferFrequencyDays"),
                                TransferFrequencyMonth = rm.Field<int?>("TransferFrequencyMonth"),
                                TransferFrequencyNumber = rm.Field<int?>("TransferFrequencyNumber"),
                                TransferFrequencyWeek = rm.Field<int?>("TransferFrequencyWeek"),
                                TransferFrequencyMainOption = rm.Field<int?>("TransferFrequencyMainOption"),
                                TrendingSampleSize = rm.Field<int?>("TrendingSampleSize"),
                                TrendingSampleSizeDivisor = rm.Field<int?>("TrendingSampleSizeDivisor"),
                                AverageUsageSampleSize = rm.Field<int?>("AverageUsageSampleSize"),
                                AverageUsageSampleSizeDivisor = rm.Field<int?>("AverageUsageSampleSizeDivisor"),
                                AverageUsageTransactions = rm.Field<int?>("AverageUsageTransactions"),
                                UDF1 = rm.Field<string>("UDF1"),
                                UDF2 = rm.Field<string>("UDF2"),
                                UDF3 = rm.Field<string>("UDF3"),
                                UDF4 = rm.Field<string>("UDF4"),
                                UDF5 = rm.Field<string>("UDF5"),
                                UDF6 = rm.Field<string>("UDF6"),
                                UDF7 = rm.Field<string>("UDF7"),
                                UDF8 = rm.Field<string>("UDF8"),
                                UDF9 = rm.Field<string>("UDF9"),
                                UDF10 = rm.Field<string>("UDF10"),
                                CompanyID = rm.Field<long?>("CompanyID"),
                                DefaultSupplierID = rm.Field<long?>("DefaultSupplierID"),
                                NextAssetNo = rm.Field<long?>("NextAssetNo"),
                                NextBinNo = rm.Field<long?>("NextBinNo"),
                                NextKitNo = rm.Field<long?>("NextKitNo"),
                                NextItemNo = rm.Field<long?>("NextItemNo"),
                                NextProjectSpendNo = rm.Field<long?>("NextProjectSpendNo"),
                                NextToolNo = rm.Field<long?>("NextToolNo"),
                                InventoryConsuptionMethod = rm.Field<string>("InventoryConsuptionMethod"),
                                ReplenishmentType = rm.Field<string>("ReplenishmentType"),
                                DefaultBinID = rm.Field<long?>("DefaultBinID"),
                                IsRoomActive = rm.Field<bool>("IsRoomActive"),
                                RequestedXDays = rm.Field<int?>("RequestedXDays"),
                                RequestedYDays = rm.Field<int?>("RequestedYDays"),
                                IsProjectSpendMandatory = rm.Field<bool>("IsProjectSpendMandatory"),
                                POAutoSequence = rm.Field<int?>("POAutoSequence"),
                                IsAverageUsageBasedOnPull = rm.Field<bool>("IsAverageUsageBasedOnPull"),
                                SlowMovingValue = rm.Field<double>("SlowMovingValue"),
                                FastMovingValue = rm.Field<double>("FastMovingValue"),
                                CreatedByName = rm.Field<string>("CreatedByName"),
                                UpdatedByName = rm.Field<string>("UpdatedByName"),
                                DefaultLocationName = rm.Field<string>("DefaultLocationName"),
                                ReplineshmentRoomName = rm.Field<string>("ReplineshmentRoomName"),
                                LastOrderDate = (rm.Field<DateTime?>("LastOrderDate") != null && rm.Field<DateTime?>("LastOrderDate").Value.Year != 1951) ? rm.Field<DateTime?>("LastOrderDate") : (DateTime?)null,
                                LastPullDate = (rm.Field<DateTime?>("LastPullDate") != null && rm.Field<DateTime?>("LastPullDate").Value.Year != 1951) ? rm.Field<DateTime?>("LastPullDate") : (DateTime?)null,
                                eVMIWaitCommand = rm.Field<Int64>("eVMIWaitCommand"),
                                eVMIWaitPort = rm.Field<Int64>("eVMIWaitPort"),
                                CountAutoSequence = rm.Field<int?>("CountAutoSequence"),
                                EnterpriseId = rm.Field<long>("EnterpriseId"),
                                EnterpriseName = rm.Field<string>("EnterpriseName"),
                                ActiveOn = (rm.Field<DateTime?>("ActiveOn") != null && rm.Field<DateTime?>("ActiveOn").Value.Year != 1951) ? rm.Field<DateTime>("ActiveOn") : (DateTime?)null,
                                LastSyncDateTime = (rm.Field<DateTime?>("LastSyncDateTime") != null && rm.Field<DateTime?>("LastSyncDateTime").Value.Year != 1951) ? rm.Field<DateTime?>("LastSyncDateTime") : (DateTime?)null,
                                PDABuildVersion = rm.Field<string>("PDABuildVersion"),
                                LastSyncUserName = rm.Field<string>("LastSyncUserName"),
                                IsAllowOrderDuplicate = rm.Field<bool>("IsAllowOrderDuplicate"),
                                IsAllowRequisitionDuplicate = rm.Field<bool?>("IsAllowRequisitionDuplicate") ?? true,
                                IsAllowWorkOrdersDuplicate = rm.Field<bool>("IsAllowWorkOrdersDuplicate"),
                                LastReceivedDate = (rm.Field<DateTime?>("LastPullDate") != null && rm.Field<DateTime?>("LastPullDate").Value.Year != 1951) ? rm.Field<DateTime?>("LastReceivedDate") : (DateTime?)null,
                                LastTrasnferedDate = (rm.Field<DateTime?>("LastTrasnferedDate") != null && rm.Field<DateTime?>("LastTrasnferedDate").Value.Year != 1951) ? rm.Field<DateTime?>("LastTrasnferedDate") : (DateTime?)null,
                                ReqAutoSequence = rm.Field<int?>("ReqAutoSequence"),
                                AllowInsertingItemOnScan = rm.Field<bool?>("AllowInsertingItemOnScan") ?? false,
                                BillingRoomType = rm.Field<int?>("BillingRoomType"),

                                StagingAutoSequence = rm.Field<int?>("StagingAutoSequence"),
                                TransferAutoSequence = rm.Field<int?>("TransferAutoSequence"),
                                WorkOrderAutoSequence = rm.Field<int?>("WorkOrderAutoSequence"),
                                WarnUserOnAssigningNonDefaultBin = rm.Field<bool>("WarnUserOnAssigningNonDefaultBin"),
                                MaintenanceDueNoticeDays = rm.Field<int?>("MaintenanceDueNoticeDays"),
                                DefaultRequisitionRequiredDays = rm.Field<int?>("DefaultRequisitionRequiredDays"),
                                AttachingWOWithRequisition = rm.Field<int?>("AttachingWOWithRequisition"),
                                PreventMaxOrderQty = rm.Field<int>("PreventMaxOrderQty"),
                                DefaultCountType = rm.Field<string>("DefaultCountType"),
                                TAOAutoSequence = rm.Field<int?>("TAOAutoSequence"),
                                TAOAutoNrFixedValue = rm.Field<string>("TAOAutoNrFixedValue"),
                                NextToolAssetOrderNo = rm.Field<string>("NextToolAssetOrderNo"),
                                AllowToolOrdering = rm.Field<bool>("AllowToolOrdering"),
                                IsWOSignatureRequired = rm.Field<bool>("IsWOSignatureRequired"),
                                IsIgnoreCreditRule = rm.Field<bool>("IsIgnoreCreditRule"),
                                IsAllowOrderCostuom = rm.Field<bool>("IsAllowOrderCostuom"),
                                ToolCountAutoSequence = rm.Field<int?>("ToolCountAutoSequence"),
                                NextToolCountNo = rm.Field<string>("NextToolCountNo"),
                                SuggestedReturn = rm.Field<bool>("SuggestedReturn"),
                                ForceSupplierFilter = rm.Field<bool>("ForceSupplierFilter"),
                                NoOfItems = rm.Field<int?>("NoOfItems"),
                                ReportAppIntent = rm.Field<string>("ReportAppIntent"),
                                NextQuoteNo = rm.Field<string>("NextQuoteNo"),
                                IsAllowQuoteDuplicate = rm.Field<bool>("IsAllowQuoteDuplicate"),
                                QuoteAutoSequence = rm.Field<int?>("QuoteAutoSequence"),
                                DoGroupSupplierQuoteToOrder = rm.Field<bool>("DoGroupSupplierQuoteToOrder"),
                                DoSendQuotetoVendor = rm.Field<bool>("DoSendQuotetoVendor"),
                                AllowABIntegration = rm.Field<bool>("AllowABIntegration"),
                                AllowOrderCloseAfterDays = rm.Field<int?>("AllowOrderCloseAfterDays"),
                                IsELabel = rm.Field<bool>("IsELabel"),
                                UserName = rm.Field<string>("UserName"),
                                Password = rm.Field<string>("Password"),
                                CompanyCode = rm.Field<string>("CompanyCode"),
                                StoreCode = rm.Field<string>("StoreCode")
                            }).ToList();
            }
            return lstRooms;
        }

        public List<RoomDTO> GetSyncRoom(int SyncMonth)
        {

            var params1 = new SqlParameter[] { new SqlParameter("@SyncMonth", SyncMonth) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("exec [GetSyncRoom] @SyncMonth", params1).ToList();
            }
        }

        public List<RoomDTO> GetAllABIntegrationRooms()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("exec [GetAllABIntegrationRooms]").ToList();
            }
        }
        public void SetRoomAllowABIntegration(long ID, bool AllowABIntegration)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@AllowABIntegration", AllowABIntegration) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [SetRoomAllowABIntegration] @ID,@AllowABIntegration", params1);
            }
        }

        public bool IsELabelDetailsDifferentInRoom(long ID, long CompanyID, string UserName, string CompanyCode, string StoreCode)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.Rooms
                           where em.ID == ID && em.CompanyID == CompanyID && (em.UserName == UserName && em.CompanyCode == CompanyCode && em.StoreCode == StoreCode)
                           select em);
                return !qry.Any();
            }
        }
        public RoomDTO IsRoomActive(long RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("exec [IsRoomActive] @RoomID", params1).FirstOrDefault();
            }
        }

    }
}