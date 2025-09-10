using CsvHelper;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using eTurns.DTO.Resources;
using System.Reflection;

namespace eTurnsWeb.Helper
{
    public class eTurnsCSVHelper
    {
        public string CSVlMain(string Filepath, string ModuleName, string ids, string SortNameString, bool? IsDeleted, string TableName = "", string CallFromPage = "", string BinIDs = "")
        {
            bool isRecordAvail = true;
            string retval = string.Empty;
            if (ModuleName != null && ModuleName.ToLower().Trim() == "toolmaster" && SessionHelper.AllowToolOrdering == true)
            {
                ModuleName = "ToolMasterNew";
            }
            if (ModuleName != null && ModuleName.ToLower().Trim() == "toolcheckoutstatus" && SessionHelper.AllowToolOrdering == true)
            {
                ModuleName = "ToolCheckoutStatusNew";
            }
            if (!string.IsNullOrEmpty(ModuleName))
            {
                switch (ModuleName)
                {
                    case "CategoryMasterList":
                        retval = CategoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "BinMasterList":
                        retval = InventoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CustomerMasterList":
                        retval = CustomerMasterListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CostUOMMasterList":
                        retval = CostUOMListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "InventoryClassificationMasterList":
                        retval = InventoryClassificationListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    //case "FreightTypeMasterList":
                    //    retval = FreightTypeListCSV(Filepath, ModuleName, ids, SortNameString);
                    //    break;
                    case "GLAccountMasterList":
                        retval = GLAccountListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "SupplierMasterList":
                        retval = SupplierListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ShipViaMasterList":
                        retval = ShipViaMasterListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "TechnicianList":
                        retval = TechnicianListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "UnitMasterList":
                        retval = UnitMasterListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "LocationMasterList":
                        retval = LocationMasterListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolCategoryList":
                        retval = ToolCategoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "AssetCategoryList":
                        retval = AssetCategoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ManufacturerMasterList":
                        retval = ManufacturerMasterListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "MeasurementTermList":
                        retval = MeasurementTermListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolList":
                        retval = ToolListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolHistoryList":
                        retval = ToolHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolMaster":
                        retval = ToolMasterListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolCheckoutStatus":
                        retval = ToolCheckoutStatusListCSV(Filepath, ModuleName, ids, SortNameString, out isRecordAvail);
                        if (isRecordAvail == false)
                        {
                            retval = eTurns.DTO.Resources.ResMessage.ExportCheckoutMessage; //"No record found for checkout.";
                        }
                        break;
                    case "AssetMasterList":
                        retval = AssetMasterListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ItemMasterList":
                    case "SupplierPNList":
                        retval = ItemMasterListCSV(Filepath, ModuleName, ids, SortNameString, CallFromPage);
                        break;
                    case "DashboardItemStockOutList":
                        retval = AlertStockOutListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ItemBinMasterList":
                        retval = ItemBinMasterListCSV(Filepath, ModuleName, ids, SortNameString, CallFromPage, BinIDs);
                        break;
                    case "QuickList":
                        retval = QuickListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "BOMItemMasterList":
                        retval = BOMItemMasterListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ItemLocationCSV":
                        if (string.IsNullOrEmpty(BinIDs))
                            retval = ItemLocationCSV(Filepath, ModuleName, ids, SortNameString);
                        else
                            retval = ItemLocationByBinCSV(Filepath, ModuleName, ids, SortNameString, BinIDs);
                        break;
                    case "ItemLocationQtyCSV":
                        if (string.IsNullOrEmpty(BinIDs))
                            retval = ItemLocationQtyCSV(Filepath, ModuleName, ids, SortNameString);
                        else
                            retval = ItemBinLocationQtyCSV(Filepath, ModuleName, ids, SortNameString, BinIDs);
                        break;
                    case "KitsCSV":
                    case "KitMasterList":
                        if (string.IsNullOrEmpty(BinIDs))
                            retval = KitsCSV(Filepath, ModuleName, ids, SortNameString);
                        else
                            retval = KitsBinCSV(Filepath, ModuleName, ids, SortNameString, BinIDs);
                        break;
                    case "ItemSupplierCSV":
                        retval = ItemSupplierCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ItemManufacturerCSV":
                        retval = ItemmanufacturerCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "BarcodeMasterCSV":
                        retval = BarCodeMasterCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "UDFMasterCSV":
                        retval = UDFMasterCSV(Filepath, ModuleName, TableName, IsDeleted);
                        break;
                    case "ProjectMasterCSV":
                    case "ProjectList":
                        retval = ProjectMasterCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ManualCountCSV":
                        retval = ManualCountCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "AdjustmentCountCSV":
                        retval = AdjustmentCountCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "WorkOrder":
                        retval = WorkOrderCountCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "PullImport":
                        retval = PullImportCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ItemLocationChangeImport":
                        retval = ItemLocationChangeImportCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "PullImportWithSameQty":
                        retval = PullImportWithSameQty(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "AssetToolScheduler":
                        retval = AssetToolSchedulerCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "AssetToolSchedulerMapping":
                        retval = AssetToolSchedulerMappingCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    //case "KitMasterList":
                    //    retval = KITMasterCSV(Filepath, ModuleName, ids, SortNameString);
                    //    break;
                    case "DashboardItemMinimumList":
                    case "DashboardItemMaximumList":
                    case "DashboardItemCriticalList":
                        retval = DashboardItemMinMaxList(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "MinMaxTuningList":
                        retval = MinMaxTuningList(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "TuningList":
                        retval = TuningList(Filepath, ModuleName, ids, SortNameString);
                        break;
                    //PastMaintenanceDue
                    case "PastMaintenanceDue":
                        retval = PastMaintenanceDueCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolMasterNew":
                        retval = ToolMasterListCSVNew(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolCheckoutStatusNew":
                        retval = ToolCheckoutStatusListCSVNew(Filepath, ModuleName, ids, SortNameString, out isRecordAvail);
                        if (isRecordAvail == false)
                        {
                            retval = eTurns.DTO.Resources.ResMessage.ExportCheckoutMessage; //"No record found for checkout.";
                        }
                        break;
                    case "ToolListNew":
                        retval = ToolListCSVNew(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "OrderMaster":
                    case "ReturnOrder":
                        retval = OrderMasterCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolAdjustmentCount":
                        retval = ToolAdjustmentCountCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ShipViaChangeLog":
                        retval = ShipViaHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "EnterpriseChangeLog":
                        retval = EnterpriseHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "RoomChangeLog":
                        retval = RoomHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "UserChangeLog":
                        retval = UserHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CompanyChangeLog":
                        retval = CompanyHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "AssetCategoryChangeLog":
                        retval = AssetCategoryHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "BinChangeLog":
                        retval = BinHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CategoryChangeLog":
                        retval = CategoryHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CostUOMChangeLog":
                        retval = CostUOMHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CustomerChangeLog":
                        retval = CustomerHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "GLAccountChangeLog":
                        retval = GLAccountHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "InventoryClassificationChangeLog":
                        retval = InventoryClassificationHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "LocationChangeLog":
                        retval = LocationHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ManufacturerChangeLog":
                        retval = ManufacturerHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "SupplierChangeLog":
                        retval = SupplierHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "TechnicianChangeLog":
                        retval = TechnicianHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolCategoryChangeLog":
                        retval = ToolCategoryHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "UnitChangeLog":
                        retval = UnitHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "VenderChangeLog":
                        retval = VenderHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "QuickListChangeLog":
                        retval = QuickListHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CountChangeLog":
                        retval = CountHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "MaterialStagingChangeLog":
                        retval = MaterialStagingHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "RequisitionChangeLog":
                        retval = RequisitionHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "WorkOrderChangeLog":
                        retval = WorkOrderHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ProjectSpendChangeLog":
                        retval = ProjectSpendHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CartItemsChangeLog":
                        retval = CartItemsHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "OrdersChangeLog":
                        retval = OrdersHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "TransferChangeLog":
                        retval = TransferHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolChangeLog":
                        retval = Asset_ToolHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "AssetsChangeLog":
                        retval = AssetsHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "NotificationMasterList":
                        retval = NotificationListCSV(Filepath, ModuleName, ids, ResourceHelper.CurrentCult.Name);
                        break;
                    case "NotificationMasterListChangeLog":
                        retval = NotificationHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;

                    case "ToolsMaintenanceList":
                        retval = ToolsMaintenanceListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "FTPChangeLog":
                        retval = FTPHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "DeletedRoom":
                        retval = DeletedRoomListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "EnterpriseQuickList":
                        retval = EnterpriseQuickListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "WrittenOffCategoryChangeLog":
                        retval = WrittenOffCategoryHistoryListCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "QuoteMaster":
                        retval = QuoteMasterCSV(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ItemMasterChangeLog":
                        retval = ItemMasterChangeLogCSV(Filepath, ModuleName, ids, SessionHelper.RoomID, SessionHelper.CompanyID);
                        break;
                    case "QuoteChangeLog":
                        retval = QuoteMasterChangeLogCSV(Filepath, ModuleName, ids, SessionHelper.RoomID, SessionHelper.CompanyID, SortNameString);
                        break;
                    case "SupplierCatalog":
                        retval = SupplierCatalogListCSV(Filepath, ModuleName, ids, SortNameString, CallFromPage);
                        break;
                    case "BOMItemToItemMaster":
                        retval = SupplierCatalogListCSV(Filepath, ModuleName, ids, SortNameString, CallFromPage);
                        break;
                    case "PermissionTemplateChangeLog":
                        retval = PermissionTemplateChangeLogCSV(Filepath, ModuleName, ids,SortNameString);
                        break;

                }
            }
            return retval;

        }

        public string ItemMasterChangeLogCSV(string Filepath, string ModuleName, string ids, long RoomID, long companyID)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            Guid itemID = Guid.Parse(arrid[0]);

            ItemMasterDAL objdal = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            var lstoflogs = objdal.GetPagedRecordsNew_ChnageLog(itemID, 0, 0, out TotalRecordCount, null, null, RoomID, companyID, false, false, null);
            if (lstoflogs != null && lstoflogs.Count() > 0)
            {
                //------------Create Header--------------------
                StreamWriter write = new StreamWriter(filePath);
                CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);

                csw.WriteField(ResCommon.HistoryID);
                csw.WriteField(ResCommon.HistoryAction);
                csw.WriteField(ResItemMaster.ItemType);
                csw.WriteField(ResItemMaster.ItemNumber);
                csw.WriteField(ResItemMaster.IsActive);
                csw.WriteField(ResItemMaster.Description);
                csw.WriteField(ResItemMaster.OnHandQuantity);
                csw.WriteField(ResItemMaster.OnOrderQuantity);
                csw.WriteField(ResItemMaster.OnOrderInTransitQuantity);
                csw.WriteField(ResItemMaster.OrderedDate.ToString());
                csw.WriteField(ResItemMaster.IsItemLevelMinMaxQtyRequired);
                csw.WriteField(ResItemMaster.MinimumQuantity);
                csw.WriteField(ResItemMaster.MaximumQuantity);
                csw.WriteField(ResCategoryMaster.Category);
                csw.WriteField(ResItemMaster.InventoryClassification);
                csw.WriteField(ResItemMaster.AverageUsage);
                csw.WriteField(ResItemMaster.Cost);
                csw.WriteField(ResItemMaster.Markup);
                csw.WriteField(ResItemMaster.SellPrice);
                csw.WriteField(ResItemMaster.ExtendedCost);
                csw.WriteField(ResItemMaster.LongDescription);
                csw.WriteField(ResItemMaster.Supplier);
                csw.WriteField(ResItemMaster.SupplierPartNo);
                csw.WriteField(ResItemMaster.ManufacturerName);
                csw.WriteField(ResItemMaster.ManufacturerNumber);
                csw.WriteField(ResItemMaster.UPC);
                csw.WriteField(ResItemMaster.UNSPSC);
                csw.WriteField(ResItemMaster.LeadTimeInDays);
                csw.WriteField(ResItemMaster.CriticalQuantity);
                csw.WriteField(ResItemMaster.SerialNumberTracking);
                csw.WriteField(ResItemMaster.LotNumberTracking);
                csw.WriteField(ResItemMaster.DateCodeTracking);
                csw.WriteField(ResItemMaster.IsLotSerialExpiryCost);
                csw.WriteField(ResGLAccount.GLAccount);
                csw.WriteField(ResCommon.ID);
                csw.WriteField(ResItemMaster.PricePerTerm);
                csw.WriteField(ResItemMaster.DefaultReorderQuantity);
                csw.WriteField(ResItemMaster.DefaultPullQuantity);
                csw.WriteField(ResItemMaster.DefaultLocation);
                csw.WriteField(ResItemMaster.Trend);
                csw.WriteField(ResItemMaster.QtyToMeetDemand);
                csw.WriteField(ResItemMaster.Taxable);
                csw.WriteField(ResItemMaster.Consignment);
                csw.WriteField(ResItemMaster.StagedQuantity);
                csw.WriteField(ResItemMaster.WeightPerPiece);
                csw.WriteField(ResItemMaster.IsTransfer);
                csw.WriteField(ResItemMaster.IsPurchase);
                csw.WriteField(ResCommon.RoomName);
                csw.WriteField(ResCommon.CreatedOn);
                csw.WriteField(ResCommon.UpdatedOn);
                csw.WriteField(ResCommon.UpdatedBy);
                csw.WriteField(ResCommon.CreatedBy);
                csw.WriteField(ResUnitMaster.Unit);
                csw.WriteField(ResItemMaster.AverageCost);
                csw.WriteField(ResItemMaster.Link2);
                csw.WriteField(ResItemMaster.TrendingSetting);
                csw.WriteField(ResItemMaster.PullQtyScanOverride);
                csw.WriteField(ResCommon.AddedFrom);
                csw.WriteField(ResCommon.EditedFrom);
                csw.WriteField(ResCommon.ReceivedOnDate);
                csw.WriteField(ResCommon.ReceivedOnWebDate);
                csw.NextRecord();
                //------------End-------------("CreatedBy");-------

                //-------------Set Row Value---------------------------
                foreach (ItemMasterDTO item in lstoflogs)
                {
                    string trendingsettingval = "";
                    if (item.TrendingSetting == null || item.TrendingSetting == 0)
                        trendingsettingval = ResItemMaster.TrendingSettingNone;
                    else if (item.TrendingSetting == 1)
                        trendingsettingval = ResItemMaster.TrendingSettingManual;
                    else if (item.TrendingSetting == 2)
                        trendingsettingval = ResItemMaster.TrendingSettingAutomatic;

                    csw.WriteField(item.HistoryID);
                    csw.WriteField(item.Action);
                    csw.WriteField(item.ItemType);
                    csw.WriteField(item.ItemNumber);
                    csw.WriteField((item.IsOrderable == true ? "Yes" : "No"));
                    csw.WriteField(item.Description);
                    csw.WriteField(Convert.ToString(item.OnHandQuantity));
                    csw.WriteField(Convert.ToString(item.OnOrderQuantity));
                    csw.WriteField(Convert.ToString(item.OnOrderInTransitQuantity));
                    csw.WriteField(Convert.ToString(item.OrderedDate));
                    csw.WriteField((item.IsItemLevelMinMaxQtyRequired == true ? "Yes" : "No"));
                    csw.WriteField(Convert.ToString(item.MinimumQuantity));
                    csw.WriteField(item.MaximumQuantity.ToString());
                    csw.WriteField(item.CategoryName);
                    csw.WriteField(item.InventoryClassificationName);
                    csw.WriteField(Convert.ToString(item.AverageUsage));
                    csw.WriteField(Convert.ToString(item.Cost));
                    csw.WriteField(Convert.ToString(item.Markup));
                    csw.WriteField(Convert.ToString(item.SellPrice));
                    csw.WriteField(Convert.ToString(item.ExtendedCost));
                    csw.WriteField(Convert.ToString(item.LongDescription));
                    csw.WriteField(Convert.ToString(item.SupplierName));
                    csw.WriteField(Convert.ToString(item.SupplierPartNo));
                    csw.WriteField(item.ManufacturerName);
                    csw.WriteField(item.ManufacturerNumber);
                    csw.WriteField(item.UPC);
                    csw.WriteField(item.UNSPSC);
                    csw.WriteField(Convert.ToString(item.LeadTimeInDays));
                    csw.WriteField(Convert.ToString(item.CriticalQuantity));
                    csw.WriteField((item.SerialNumberTracking == true ? "Yes" : "No"));
                    csw.WriteField((item.LotNumberTracking == true ? "Yes" : "No"));
                    csw.WriteField((item.DateCodeTracking == true ? "Yes" : "No"));
                    csw.WriteField(Convert.ToString(item.IsLotSerialExpiryCost));
                    csw.WriteField(item.GLAccount);
                    csw.WriteField(Convert.ToString(item.ID));
                    csw.WriteField(Convert.ToString(item.CostUOMName));
                    csw.WriteField(Convert.ToString(item.DefaultReorderQuantity));
                    csw.WriteField(Convert.ToString(item.DefaultPullQuantity));
                    csw.WriteField(Convert.ToString(item.DefaultLocationName));
                    csw.WriteField((item.Trend == true ? "Yes" : "No"));
                    csw.WriteField(Convert.ToString(item.QtyToMeetDemand));
                    csw.WriteField((item.Taxable == true ? "Yes" : "No"));
                    csw.WriteField((item.Consignment == true ? "Yes" : "No"));
                    csw.WriteField(Convert.ToString(item.StagedQuantity));
                    csw.WriteField(Convert.ToString(item.WeightPerPiece));
                    csw.WriteField((item.IsTransfer == true ? "Yes" : "No"));
                    csw.WriteField((item.IsPurchase == true ? "Yes" : "No"));
                    csw.WriteField(Convert.ToString(item.RoomName));
                    csw.WriteField(item.Created.ToString());
                    csw.WriteField(item.Updated.ToString());
                    csw.WriteField(item.UpdatedByName);
                    csw.WriteField(item.CreatedByName);
                    csw.WriteField(item.Unit);
                    csw.WriteField(Convert.ToString(item.AverageCost));
                    csw.WriteField(item.Link2);
                    csw.WriteField(trendingsettingval);
                    csw.WriteField((item.IsPurchase == true ? "Yes" : "No"));
                    csw.WriteField(Convert.ToString(item.AddedFrom));
                    csw.WriteField(Convert.ToString(item.EditedFrom));
                    csw.WriteField(Convert.ToString(item.ReceivedOnDate));
                    csw.WriteField(Convert.ToString(item.ReceivedOnDateWeb));
                    csw.NextRecord();
                    //-------------End--------------------------
                }
                write.Close();
            }
            return filename;

        }

        public string WrittenOffCategoryHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            WrittenOffCategoryDAL objdal = new WrittenOffCategoryDAL(SessionHelper.EnterPriseDBName);
            var lstoflogs = objdal.WrittenOffCategoryHistoryChangeLog(ids);
            if (lstoflogs != null && lstoflogs.Count > 0)
            {
                StreamWriter write = new StreamWriter(filePath);
                CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
                csw.WriteField("HistoryAction");
                csw.WriteField("ToolWrittenOffCategoryName");
                csw.WriteField("CreatedOn");
                csw.WriteField("UpdatedOn");
                csw.WriteField("UpdatedBy");
                csw.WriteField("CreatedBy");
                csw.WriteField("AddedFrom");
                csw.WriteField("EditedFrom");
                csw.WriteField("ReceivedOnDate");
                csw.WriteField("ReceivedOnWebDate");
                csw.NextRecord();
                if (lstoflogs != null && lstoflogs.Count > 0)
                {
                    foreach (var rec in ConvertExportDataList<WrittenOfCategoryDTO>(lstoflogs).ToList())
                    {
                        //csw.WriteRecord<CategoryMasterDTO>(rec);
                        csw.WriteField(rec.Action);
                        csw.WriteField(rec.WrittenOffCategory);
                        csw.WriteField(rec.Created.ToString());
                        csw.WriteField(rec.Updated.ToString());
                        csw.WriteField(rec.UpdatedByName);
                        csw.WriteField(rec.CreatedByName);
                        csw.WriteField(rec.AddedFrom);
                        csw.WriteField(rec.EditedFrom);
                        csw.WriteField(rec.ReceivedOnDate);
                        csw.WriteField(rec.ReceivedOnDateWeb);

                        csw.NextRecord();
                    }
                }
                write.Close();
                return filename;
            }
            return filename;
        }
        public string CategoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            //string[] arrid = ids.Split(',');
            CategoryMasterDAL obj = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);

            //var DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).OrderBy(SortNameString);
            List<CategoryMasterDTO> lstCategoryMasterDTO = new List<CategoryMasterDTO>();
            if (!string.IsNullOrEmpty(ids))
            {
                lstCategoryMasterDTO = obj.GetCategoriesByIdsByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, ids).OrderBy(SortNameString).ToList();
            }
            else
            {
                lstCategoryMasterDTO = obj.GetCategoriesByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();
            }
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstCategoryMasterDTO = (from c in DataFromDB
            //                            where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(c.ID.ToString())
            //                            select new CategoryMasterDTO
            //                            {
            //                                ID = c.ID,
            //                                Category = c.Category != null ? Convert.ToString(c.Category) : string.Empty,
            //                                RoomName = c.RoomName,
            //                                Created = c.Created,
            //                                Updated = c.Updated,
            //                                UpdatedByName = c.UpdatedByName,
            //                                CreatedByName = c.CreatedByName,
            //                                IsDeleted = c.IsDeleted,
            //                                IsArchived = c.IsArchived,
            //                                UDF1 = c.UDF1,
            //                                UDF2 = c.UDF2,
            //                                UDF3 = c.UDF3,
            //                                UDF4 = c.UDF4,
            //                                UDF5 = c.UDF5
            //                            }).ToList();
            //}



            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* CATEGORY");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.NextRecord();
            if (lstCategoryMasterDTO != null && lstCategoryMasterDTO.Count > 0)
            {
                //csw.WriteHeader<CategoryMasterDTO>();


                foreach (var rec in ConvertExportDataList<CategoryMasterDTO>(lstCategoryMasterDTO).ToList())
                {
                    //csw.WriteRecord<CategoryMasterDTO>(rec);
                    csw.WriteField(rec.ID);

                    csw.WriteField(rec.Category);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string InventoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            //  string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<BinMasterDTO> DataFromDB = objBinMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            // IEnumerable<AssetCategoryMasterDTO> DataFromDB = obj.GetAssetCategoryByRoom(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString);
            IEnumerable<BinMasterDTO> DataFromDB = objBinMasterDAL.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            //  IEnumerable<BinMasterDTO> lstBinMasterDTO = new List<BinMasterDTO>();

            List<BinMasterDTO> lstBinMasterDTO = new List<BinMasterDTO>();
            if (!string.IsNullOrEmpty(ids))
            {
                lstBinMasterDTO = (from c in DataFromDB
                                   where arrid.Contains(c.ID.ToString())
                                   select new BinMasterDTO
                                   {
                                       ID = c.ID,
                                       BinNumber = c.BinNumber != null ? Convert.ToString(c.BinNumber) : string.Empty,
                                       ItemNumber = c.ItemNumber,
                                       CriticalQuantity = c.CriticalQuantity,
                                       MinimumQuantity = c.MinimumQuantity,
                                       MaximumQuantity = c.MaximumQuantity,
                                       IsDefault = c.IsDefault,
                                       eVMISensorID = c.eVMISensorID,
                                       eVMISensorPort = c.eVMISensorPort,
                                       UDF1 = c.UDF1,
                                       UDF2 = c.UDF2,
                                       UDF3 = c.UDF3,
                                       UDF4 = c.UDF4,
                                       UDF5 = c.UDF5
                                   }).ToList();
            }
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstBinMasterDTO = objBinMasterDAL.GetBinMasterbyIds(SessionHelper.RoomID, SessionHelper.CompanyID, ids).OrderBy(SortNameString);

            //}


            StreamWriter write = new StreamWriter(filePath, true);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* LOCATION");
            //csw.WriteField("* ItemNumber");
            //csw.WriteField("CriticalQuantity");
            //csw.WriteField("MinimumQuantity");
            //csw.WriteField("MaximumQuantity");
            //csw.WriteField("IsDefault");
            //csw.WriteField("SensorId");
            //csw.WriteField("SensorPort");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.NextRecord();
            if (lstBinMasterDTO != null && lstBinMasterDTO.Count() > 0)
            {
                foreach (BinMasterDTO rec in ConvertExportDataList<BinMasterDTO>(lstBinMasterDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.BinNumber);
                    //csw.WriteField(rec.ItemNumber);
                    //csw.WriteField(rec.CriticalQuantity);
                    //csw.WriteField(rec.MinimumQuantity);
                    //csw.WriteField(rec.MaximumQuantity);
                    //csw.WriteField(rec.IsDefault);
                    //csw.WriteField(rec.eVMISensorID ?? 0);
                    //csw.WriteField(rec.eVMISensorPort);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string CustomerMasterListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            CustomerMasterDAL obj = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            List<CustomerMasterDTO> lstCustomerMasterDTO = new List<CustomerMasterDTO>();
            lstCustomerMasterDTO = obj.GetCustomersByIDs(ids, SessionHelper.RoomID, SessionHelper.CompanyID);
            // if (!string.IsNullOrEmpty(ids))
            //{
            //    lstCustomerMasterDTO = (from c in DataFromDB
            //                            where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(c.ID.ToString())
            //                            select new CustomerMasterDTO
            //                            {
            //                                ID = c.ID,
            //                                Customer = c.Customer != null ? c.Customer : string.Empty,
            //                                Account = c.Account != null ? c.Account : string.Empty,
            //                                Address = c.Address != null ? c.Address : string.Empty,
            //                                City = c.City,
            //                                State = c.State,
            //                                Country = c.Country,
            //                                ZipCode = c.ZipCode,
            //                                Contact = c.Contact,
            //                                Email = c.Email,
            //                                Phone = c.Phone,
            //                                RoomName = c.RoomName,
            //                                Created = c.Created,
            //                                CreatedBy = c.CreatedBy,
            //                                Updated = c.Updated,
            //                                LastUpdatedBy = c.LastUpdatedBy,
            //                                UpdatedByName = c.UpdatedByName,
            //                                Room = c.Room,
            //                                CreatedByName = c.CreatedByName,
            //                                IsDeleted = c.IsDeleted,
            //                                IsArchived = c.IsArchived,
            //                                UDF1 = c.UDF1,
            //                                UDF2 = c.UDF2,
            //                                UDF3 = c.UDF3,
            //                                UDF4 = c.UDF4,
            //                                UDF5 = c.UDF5,
            //                                Remarks = c.Remarks
            //                            }).ToList();
            //}


            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* CUSTOMER");
            csw.WriteField("* ACCOUNT");
            csw.WriteField("CONTACT");
            csw.WriteField("ADDRESS");
            csw.WriteField("CITY");
            csw.WriteField("STATE");
            csw.WriteField("ZIPCODE");
            csw.WriteField("COUNTRY");
            csw.WriteField("PHONE");
            csw.WriteField("EMAIL");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("Remarks");

            csw.NextRecord();
            if (lstCustomerMasterDTO != null && lstCustomerMasterDTO.Count > 0)
            {


                foreach (CustomerMasterDTO rec in ConvertExportDataList<CustomerMasterDTO>(lstCustomerMasterDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.Customer);
                    csw.WriteField(rec.Account);
                    csw.WriteField(rec.Contact);
                    csw.WriteField(rec.Address);
                    csw.WriteField(rec.City);
                    csw.WriteField(rec.State);
                    csw.WriteField(rec.ZipCode);
                    csw.WriteField(rec.Country);
                    csw.WriteField(rec.Phone);
                    csw.WriteField(rec.Email);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.WriteField(rec.Remarks);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string CostUOMListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            CostUOMMasterDAL obj = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<CostUOMMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);

            List<CostUOMMasterDTO> lstCostUOMMasterDTO = new List<CostUOMMasterDTO>();
            lstCostUOMMasterDTO = obj.GetCostUOMsByIDs(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstCostUOMMasterDTO = (from u in DataFromDB
            //                           where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.ID.ToString())
            //                           select new CostUOMMasterDTO
            //                           {
            //                               ID = u.ID,
            //                               CostUOM = u.CostUOM != null ? Convert.ToString(u.CostUOM) : string.Empty,
            //                               CostUOMValue = u.CostUOMValue,
            //                               UDF1 = u.UDF1,
            //                               UDF2 = u.UDF2,
            //                               UDF3 = u.UDF3,
            //                               UDF4 = u.UDF4,
            //                               UDF5 = u.UDF5,
            //                               GUID = u.GUID,
            //                               Created = u.Created,
            //                               Updated = u.Updated,
            //                               CreatedBy = u.CreatedBy,
            //                               LastUpdatedBy = u.LastUpdatedBy,
            //                               IsDeleted = u.IsDeleted,
            //                               IsArchived = u.IsArchived,
            //                               CompanyID = u.CompanyID,
            //                               Room = u.Room,
            //                               CreatedByName = u.CreatedByName,
            //                               UpdatedByName = u.UpdatedByName,
            //                               RoomName = u.RoomName,
            //                           }).ToList();
            //}

            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* CostUOM");
            csw.WriteField("* CostUOMValue");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.NextRecord();
            if (lstCostUOMMasterDTO != null && lstCostUOMMasterDTO.Count > 0)
            {


                foreach (CostUOMMasterDTO rec in ConvertExportDataList<CostUOMMasterDTO>(lstCostUOMMasterDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.CostUOM);
                    csw.WriteField(rec.CostUOMValue);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string InventoryClassificationListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            InventoryClassificationMasterDAL obj = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<InventoryClassificationMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).OrderBy(SortNameString);
            List<InventoryClassificationMasterDTO> lstInventoryClassificationMasterDTO = new List<InventoryClassificationMasterDTO>();
            lstInventoryClassificationMasterDTO = obj.GetInventoryClassificationByIDsPlain(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstInventoryClassificationMasterDTO = (from u in DataFromDB
            //                                           where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.ID.ToString())
            //                                           select new InventoryClassificationMasterDTO
            //                                           {
            //                                               ID = u.ID,
            //                                               InventoryClassification = u.InventoryClassification != null ? Convert.ToString(u.InventoryClassification) : string.Empty,
            //                                               BaseOfInventory = u.BaseOfInventory != null ? Convert.ToString(u.BaseOfInventory) : string.Empty,
            //                                               RangeStart = u.RangeStart,
            //                                               RangeEnd = u.RangeEnd,
            //                                               UDF1 = u.UDF1,
            //                                               UDF2 = u.UDF2,
            //                                               UDF3 = u.UDF3,
            //                                               UDF4 = u.UDF4,
            //                                               UDF5 = u.UDF5,
            //                                               GUID = u.GUID,
            //                                               Created = u.Created,
            //                                               Updated = u.Updated,
            //                                               CreatedBy = u.CreatedBy,
            //                                               LastUpdatedBy = u.LastUpdatedBy,
            //                                               IsDeleted = u.IsDeleted,
            //                                               IsArchived = u.IsArchived,
            //                                               CompanyID = u.CompanyID,
            //                                               Room = u.Room,
            //                                               CreatedByName = u.CreatedByName,
            //                                               UpdatedByName = u.UpdatedByName,
            //                                               RoomName = u.RoomName,
            //                                           }).ToList();
            //}



            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* InventoryClassification");
            csw.WriteField("* BaseOfInventory");
            csw.WriteField("* RangeStart");
            csw.WriteField("* RangeEnd");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.NextRecord();
            if (lstInventoryClassificationMasterDTO != null && lstInventoryClassificationMasterDTO.Count > 0)
            {


                foreach (InventoryClassificationMasterDTO rec in ConvertExportDataList<InventoryClassificationMasterDTO>(lstInventoryClassificationMasterDTO).ToList())
                {
                    //csw.WriteRecord<CategoryMasterDTO>(rec);
                    csw.WriteField(rec.ID);

                    csw.WriteField(rec.InventoryClassification);
                    csw.WriteField(rec.BaseOfInventory ?? string.Empty);
                    csw.WriteField(rec.RangeStart);
                    csw.WriteField(rec.RangeEnd);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        //public string FreightTypeListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        //{
        //    string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
        //    string filePath = Filepath + filename;
        //    string[] arrid = ids.Split(',');
        //    FreightTypeMasterDAL obj = new FreightTypeMasterDAL(SessionHelper.EnterPriseDBName);
        //    //IEnumerable<FreightTypeMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);

        //    List<FreightTypeMasterDTO> lstFreightTypeMasterDTO = new List<FreightTypeMasterDTO>();
        //    lstFreightTypeMasterDTO = obj.GetFreightTypesByIDs(ids, SessionHelper.RoomID, SessionHelper.CompanyID);

        //    //// if (!string.IsNullOrEmpty(ids))
        //    //{
        //    //    lstFreightTypeMasterDTO = (from u in DataFromDB
        //    //                               where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.ID.ToString())
        //    //                               select new FreightTypeMasterDTO
        //    //                               {
        //    //                                   FreightType = u.FreightType != null ? Convert.ToString(u.FreightType) : string.Empty,
        //    //                                   Created = u.Created,
        //    //                                   CreatedBy = u.CreatedBy,
        //    //                                   ID = u.ID,
        //    //                                   LastUpdatedBy = u.LastUpdatedBy,
        //    //                                   Room = u.Room,
        //    //                                   LastUpdated = u.LastUpdated,
        //    //                                   CreatedByName = u.CreatedByName,
        //    //                                   UpdatedByName = u.UpdatedByName,
        //    //                                   RoomName = u.RoomName,
        //    //                                   GUID = u.GUID,
        //    //                                   CompanyID = u.CompanyID,
        //    //                                   IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
        //    //                                   IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
        //    //                                   UDF1 = u.UDF1,
        //    //                                   UDF2 = u.UDF2,
        //    //                                   UDF3 = u.UDF3,
        //    //                                   UDF4 = u.UDF4,
        //    //                                   UDF5 = u.UDF5
        //    //                               }).ToList();
        //    //}

        //    StreamWriter write = new StreamWriter(filePath);
        //    CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
        //    csw.WriteField("id");
        //    csw.WriteField("* FREIGHTTYPE");
        //    csw.WriteField("UDF1");
        //    csw.WriteField("UDF2");
        //    csw.WriteField("UDF3");
        //    csw.WriteField("UDF4");
        //    csw.WriteField("UDF5");

        //    csw.NextRecord();
        //    if (lstFreightTypeMasterDTO != null && lstFreightTypeMasterDTO.Count > 0)
        //    {


        //        foreach (FreightTypeMasterDTO rec in lstFreightTypeMasterDTO)
        //        {

        //            csw.WriteField(rec.ID);
        //            csw.WriteField(rec.FreightType);
        //            csw.WriteField(rec.UDF1);
        //            csw.WriteField(rec.UDF2);
        //            csw.WriteField(rec.UDF3);
        //            csw.WriteField(rec.UDF4);
        //            csw.WriteField(rec.UDF5);
        //            csw.NextRecord();

        //        }
        //    }
        //    write.Close();

        //    return filename;

        //}
        public string GLAccountListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            GLAccountMasterDAL obj = new GLAccountMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<GLAccountMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).OrderBy(SortNameString);
            List<GLAccountMasterDTO> lstGLAccountMasterDTO = new List<GLAccountMasterDTO>();
            lstGLAccountMasterDTO = obj.GetGLAccountsByIDs(ids, SessionHelper.RoomID, SessionHelper.CompanyID);
            // if (!string.IsNullOrEmpty(ids))
            //{
            //    lstGLAccountMasterDTO = (from c in DataFromDB
            //                             where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(c.ID.ToString())
            //                             select new GLAccountMasterDTO
            //                             {
            //                                 ID = c.ID,
            //                                 GLAccount = c.GLAccount != null ? Convert.ToString(c.GLAccount) : string.Empty,
            //                                 Description = c.Description != null ? Convert.ToString(c.Description) : string.Empty,
            //                                 RoomName = c.RoomName,
            //                                 Created = c.Created,
            //                                 Updated = c.Updated,
            //                                 UpdatedByName = c.UpdatedByName,
            //                                 CreatedByName = c.CreatedByName,
            //                                 IsArchived = c.IsArchived,
            //                                 IsDeleted = c.IsDeleted,
            //                                 UDF1 = c.UDF1,
            //                                 UDF2 = c.UDF2,
            //                                 UDF3 = c.UDF3,
            //                                 UDF4 = c.UDF4,
            //                                 UDF5 = c.UDF5
            //                             }).ToList();
            //}
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* GLAccount");
            csw.WriteField("Description");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.NextRecord();
            if (lstGLAccountMasterDTO != null && lstGLAccountMasterDTO.Count > 0)
            {
                //csw.WriteHeader<CategoryMasterDTO>();


                foreach (GLAccountMasterDTO rec in ConvertExportDataList<GLAccountMasterDTO>(lstGLAccountMasterDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.GLAccount);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string SupplierListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            //string[] arrid = ids.Split(',');
            SupplierMasterDAL obj = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<SupplierMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false);
            List<SupplierMasterDTO> lstSupplierMasterDTO = obj.GetExportSupplierListByIDsFull(ids, SessionHelper.RoomID, SessionHelper.CompanyID).ToList().OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* SUPPLIERNAME");
            csw.WriteField("* SupplierColor");
            csw.WriteField("Description");
            //csw.WriteField("ReceiverID");
            csw.WriteField("BranchNumber");
            csw.WriteField("MaximumOrderSize");
            csw.WriteField("ADDRESS");
            csw.WriteField("CITY");
            csw.WriteField("STATE");
            csw.WriteField("ZIPCODE");
            csw.WriteField("COUNTRY");

            csw.WriteField("* CONTACT");
            csw.WriteField("* PHONE");

            csw.WriteField("Fax");
            csw.WriteField("EMAIL");
            //csw.WriteField("IsEmailPOInBody");
            //csw.WriteField("IsEmailPOInPDF");
            //csw.WriteField("IsEmailPOInCSV");
            //csw.WriteField("IsEmailPOInX12");
            csw.WriteField("IsSendtoVendor");
            csw.WriteField("IsVendorReturnAsn");
            csw.WriteField("IsSupplierReceivesKitComponents");
            csw.WriteField("OrderNumberTypeBlank");
            csw.WriteField("OrderNumberTypeFixed");
            csw.WriteField("OrderNumberTypeBlanketOrderNumber");
            csw.WriteField("OrderNumberTypeIncrementingOrderNumber");
            csw.WriteField("OrderNumberTypeIncrementingbyDay");
            csw.WriteField("OrderNumberTypeDateIncrementing");
            csw.WriteField("OrderNumberTypeDate");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("UDF6");
            csw.WriteField("UDF7");
            csw.WriteField("UDF8");
            csw.WriteField("UDF9");
            csw.WriteField("UDF10");
            csw.WriteField("* AccountNumber");
            csw.WriteField("* AccountName");
            csw.WriteField("AccountAddress");
            csw.WriteField("AccountCity");
            csw.WriteField("AccountState");
            csw.WriteField("AccountZip");
            csw.WriteField("AccountCountry");
            csw.WriteField("AccountShipToID");
            csw.WriteField("AccountIsDefault");
            csw.WriteField("BlanketPONumber");
            csw.WriteField("StartDate");
            csw.WriteField("EndDate");
            csw.WriteField("MaxLimit");
            csw.WriteField("DoNotExceed");
            csw.WriteField("MaxLimitQty");
            csw.WriteField("DoNotExceedQty");
            csw.WriteField("IsBlanketDeleted");
            csw.WriteField("SupplierImage");
            csw.WriteField("ImageExternalURL");


            csw.NextRecord();
            if (lstSupplierMasterDTO != null && lstSupplierMasterDTO.Count > 0)
            {


                foreach (SupplierMasterDTO rec in ConvertExportDataList<SupplierMasterDTO>(lstSupplierMasterDTO).ToList())
                {
                    int maxCount = rec.SupplierAccountDetails.Count > rec.SupplierBlanketPODetails.Count ? rec.SupplierAccountDetails.Count : rec.SupplierBlanketPODetails.Count;

                    if (maxCount == 0)
                    {
                        csw.WriteField(rec.ID);
                        csw.WriteField(rec.SupplierName);
                        csw.WriteField(rec.SupplierColor);
                        csw.WriteField(rec.Description);
                        //csw.WriteField(rec.ReceiverID);
                        csw.WriteField(rec.BranchNumber);
                        csw.WriteField(rec.MaximumOrderSize);
                        csw.WriteField(rec.Address);
                        csw.WriteField(rec.City);
                        csw.WriteField(rec.State);
                        csw.WriteField(rec.ZipCode);
                        csw.WriteField(rec.Country);
                        csw.WriteField(rec.Contact);
                        csw.WriteField(rec.Phone);
                        csw.WriteField(rec.Fax);
                        csw.WriteField(rec.Email);
                        //csw.WriteField(rec.IsEmailPOInBody);
                        //csw.WriteField(rec.IsEmailPOInPDF);
                        //csw.WriteField(rec.IsEmailPOInCSV);
                        //csw.WriteField(rec.IsEmailPOInX12);
                        csw.WriteField(rec.IsSendtoVendor);
                        csw.WriteField(rec.IsVendorReturnAsn);
                        csw.WriteField(rec.IsSupplierReceivesKitComponents);

                        if (rec.POAutoSequence.HasValue)
                        {
                            if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeBlank)
                            {
                                csw.WriteField("TRUE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                            }
                            else if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeFixed)
                            {
                                csw.WriteField("FALSE");
                                csw.WriteField("TRUE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                            }
                            else if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeBlanketOrderNumber)
                            {
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("TRUE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                            }
                            else if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeIncrementingOrderNumber)
                            {
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("TRUE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                            }
                            else if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeIncrementingbyDay)
                            {
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("TRUE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                            }
                            else if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeDateIncrementing)
                            {
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("TRUE");
                                csw.WriteField("FALSE");
                            }
                            else if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeDate)
                            {
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("TRUE");
                            }
                        }
                        else
                        {
                            csw.WriteField("FALSE");
                            csw.WriteField("FALSE");
                            csw.WriteField("FALSE");
                            csw.WriteField("FALSE");
                            csw.WriteField("FALSE");
                            csw.WriteField("FALSE");
                            csw.WriteField("FALSE");
                        }

                        csw.WriteField(rec.UDF1);
                        csw.WriteField(rec.UDF2);
                        csw.WriteField(rec.UDF3);
                        csw.WriteField(rec.UDF4);
                        csw.WriteField(rec.UDF5);
                        csw.WriteField(rec.UDF6);
                        csw.WriteField(rec.UDF7);
                        csw.WriteField(rec.UDF8);
                        csw.WriteField(rec.UDF9);
                        csw.WriteField(rec.UDF10);

                        csw.WriteField(string.Empty);
                        csw.WriteField(string.Empty);
                        csw.WriteField(string.Empty);
                        csw.WriteField(string.Empty);
                        csw.WriteField(string.Empty);
                        csw.WriteField(string.Empty);
                        csw.WriteField(string.Empty);

                        csw.WriteField(string.Empty);
                        csw.WriteField(string.Empty);
                        csw.WriteField(string.Empty);
                        csw.WriteField(string.Empty);
                        csw.WriteField(string.Empty);
                        csw.WriteField(string.Empty);
                        csw.WriteField(string.Empty);

                        csw.WriteField("FALSE");



                        csw.WriteField(string.Empty);
                        csw.WriteField(string.Empty);

                        csw.NextRecord();
                    }

                    for (int i = 0; i < maxCount; i++)
                    {
                        csw.WriteField(rec.ID);
                        csw.WriteField(rec.SupplierName);
                        csw.WriteField(rec.SupplierColor);
                        csw.WriteField(rec.Description);
                        //csw.WriteField(rec.ReceiverID);
                        csw.WriteField(rec.BranchNumber);
                        csw.WriteField(rec.MaximumOrderSize);
                        csw.WriteField(rec.Address);
                        csw.WriteField(rec.City);
                        csw.WriteField(rec.State);
                        csw.WriteField(rec.ZipCode);
                        csw.WriteField(rec.Country);
                        csw.WriteField(rec.Contact);
                        csw.WriteField(rec.Phone);
                        csw.WriteField(rec.Fax);
                        csw.WriteField(rec.Email);
                        //csw.WriteField(rec.IsEmailPOInBody);
                        //csw.WriteField(rec.IsEmailPOInPDF);
                        //csw.WriteField(rec.IsEmailPOInCSV);
                        //csw.WriteField(rec.IsEmailPOInX12);
                        csw.WriteField(rec.IsSendtoVendor);
                        csw.WriteField(rec.IsVendorReturnAsn);
                        csw.WriteField(rec.IsSupplierReceivesKitComponents);

                        if (rec.POAutoSequence.HasValue)
                        {
                            if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeBlank)
                            {
                                csw.WriteField("TRUE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                            }
                            else if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeFixed)
                            {
                                csw.WriteField("FALSE");
                                csw.WriteField("TRUE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                            }
                            else if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeBlanketOrderNumber)
                            {
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("TRUE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                            }
                            else if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeIncrementingOrderNumber)
                            {
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("TRUE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                            }
                            else if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeIncrementingbyDay)
                            {
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("TRUE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                            }
                            else if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeDateIncrementing)
                            {
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("TRUE");
                                csw.WriteField("FALSE");
                            }
                            else if (rec.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeDate)
                            {
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("FALSE");
                                csw.WriteField("TRUE");
                            }
                        }
                        else
                        {
                            csw.WriteField("FALSE");
                            csw.WriteField("FALSE");
                            csw.WriteField("FALSE");
                            csw.WriteField("FALSE");
                            csw.WriteField("FALSE");
                            csw.WriteField("FALSE");
                            csw.WriteField("FALSE");
                        }

                        csw.WriteField(rec.UDF1);
                        csw.WriteField(rec.UDF2);
                        csw.WriteField(rec.UDF3);
                        csw.WriteField(rec.UDF4);
                        csw.WriteField(rec.UDF5);
                        csw.WriteField(rec.UDF6);
                        csw.WriteField(rec.UDF7);
                        csw.WriteField(rec.UDF8);
                        csw.WriteField(rec.UDF9);
                        csw.WriteField(rec.UDF10);

                        if (rec.SupplierAccountDetails.Count > i)
                        {
                            csw.WriteField(rec.SupplierAccountDetails[i].AccountNo);
                            csw.WriteField(rec.SupplierAccountDetails[i].AccountName);
                            csw.WriteField(rec.SupplierAccountDetails[i].Address);
                            csw.WriteField(rec.SupplierAccountDetails[i].City);
                            csw.WriteField(rec.SupplierAccountDetails[i].State);
                            csw.WriteField(rec.SupplierAccountDetails[i].ZipCode);
                            csw.WriteField(rec.SupplierAccountDetails[i].Country);
                            csw.WriteField(rec.SupplierAccountDetails[i].ShipToID);
                            csw.WriteField((rec.SupplierAccountDetails[i].IsDefault == true ? "TRUE" : "FALSE"));
                        }
                        else
                        {
                            csw.WriteField(string.Empty);
                            csw.WriteField(string.Empty);

                            csw.WriteField(string.Empty);
                            csw.WriteField(string.Empty);
                            csw.WriteField(string.Empty);
                            csw.WriteField(string.Empty);
                            csw.WriteField(string.Empty);

                            csw.WriteField(string.Empty);
                            csw.WriteField(string.Empty);
                        }

                        if (rec.SupplierBlanketPODetails.Count > i)
                        {
                            csw.WriteField(rec.SupplierBlanketPODetails[i].BlanketPO);
                            csw.WriteField(rec.SupplierBlanketPODetails[i].StartDate);
                            csw.WriteField(rec.SupplierBlanketPODetails[i].Enddate);
                            csw.WriteField(rec.SupplierBlanketPODetails[i].MaxLimit);
                            csw.WriteField(rec.SupplierBlanketPODetails[i].IsNotExceed);
                            csw.WriteField(rec.SupplierBlanketPODetails[i].MaxLimitQty);
                            csw.WriteField(rec.SupplierBlanketPODetails[i].IsNotExceedQty);
                        }
                        else
                        {
                            csw.WriteField(string.Empty);
                            csw.WriteField(string.Empty);
                            csw.WriteField(string.Empty);
                            csw.WriteField(string.Empty);
                            csw.WriteField(string.Empty);
                            csw.WriteField(string.Empty);
                            csw.WriteField(string.Empty);
                        }
                        csw.WriteField("FALSE");
                        csw.WriteField(rec.SupplierImage);
                        csw.WriteField(rec.ImageExternalURL);
                        csw.NextRecord();
                    }
                }
            }
            write.Close();

            return filename;

        }
        public string ShipViaMasterListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ShipViaDAL obj = new ShipViaDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<ShipViaDTO> DataFromDB = obj.GetShipViaByIDs(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString);
            List<ShipViaDTO> lstShipViaDTO = new List<ShipViaDTO>();
            lstShipViaDTO = obj.GetShipViaByIDsPlain(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstShipViaDTO = (from c in DataFromDB
            //                     where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(c.ID.ToString())
            //                     select new ShipViaDTO
            //                     {
            //                         ID = c.ID,
            //                         ShipVia = c.ShipVia != null ? Convert.ToString(c.ShipVia) : string.Empty,
            //                         UDF1 = c.UDF1,
            //                         UDF2 = c.UDF2,
            //                         UDF3 = c.UDF3,
            //                         UDF4 = c.UDF4,
            //                         UDF5 = c.UDF5
            //                     }).ToList();
            //}



            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* SHIPVIA");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.NextRecord();
            if (lstShipViaDTO != null && lstShipViaDTO.Count > 0)
            {


                foreach (ShipViaDTO rec in ConvertExportDataList<ShipViaDTO>(lstShipViaDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ShipVia);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string ShipViaHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ShipViaDAL obj = new ShipViaDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<ShipViaDTO> DataFromDB = obj.GetShipViaByIDs(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString);
            List<ShipViaDTO> lstShipViaDTO = new List<ShipViaDTO>();
            lstShipViaDTO = obj.GetShipViaHistoryByIDsNormal(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("Action");
            csw.WriteField("* ShipVia");
            csw.WriteField("RoomName");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");

            csw.NextRecord();
            if (lstShipViaDTO != null && lstShipViaDTO.Count > 0)
            {


                foreach (ShipViaDTO rec in ConvertExportDataList<ShipViaDTO>(lstShipViaDTO).ToList())
                {

                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.ShipVia);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnWeb);
                    csw.WriteField(rec.ReceivedOn);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.UpdatedByName);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string EnterpriseHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            eTurnsMaster.DAL.CommonMasterDAL obj = new eTurnsMaster.DAL.CommonMasterDAL();
            //IEnumerable<ShipViaDTO> DataFromDB = obj.GetShipViaByIDs(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString);
            List<EnterpriseDTO> lstEnterpDTO = new List<EnterpriseDTO>();
            lstEnterpDTO = obj.GetEnterpriseHistoryByIDsNormal(ids).OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("EnterpriseName");
            csw.WriteField("Address");
            csw.WriteField("City");
            csw.WriteField("State");
            csw.WriteField("PostalCode");
            csw.WriteField("Country");
            csw.WriteField("ContactPhone");
            csw.WriteField("ContactEmail");
            csw.WriteField("CreatedBy");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");


            csw.NextRecord();
            if (lstEnterpDTO != null && lstEnterpDTO.Count > 0)
            {


                foreach (EnterpriseDTO rec in ConvertExportDataList<EnterpriseDTO>(lstEnterpDTO).ToList())
                {

                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.Name);
                    csw.WriteField(rec.Address);
                    csw.WriteField(rec.City);
                    csw.WriteField(rec.State);
                    csw.WriteField(rec.PostalCode);
                    csw.WriteField(rec.Country);
                    csw.WriteField(rec.ContactPhone);
                    csw.WriteField(rec.ContactEmail);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedDate);
                    csw.WriteField(rec.UpdatedDate);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string RoomHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            RoomDAL obj = new RoomDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<ShipViaDTO> DataFromDB = obj.GetShipViaByIDs(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString);
            List<RoomHistoryDTO> lstRoomDTO = new List<RoomHistoryDTO>();
            SortNameString = SortNameString.Replace("MethodOfValuingInventoryName", "MethodOfValuingInventory").Replace("AttachingWOWithRequisitionName", "AttachingWOWithRequisition")
                .Replace("PreventMaxOrderQtyName", "PreventMaxOrderQty").Replace("PreventMaxOrderQtyName", "PreventMaxOrderQty").Replace("DefaultCountTypeName", "DefaultCountType")
                .Replace("CountAutoSequenceName", "CountAutoSequence").Replace("POAutoSequenceName", "POAutoSequence").Replace("PullPurchaseNumberTypeName", "PullPurchaseNumberType")
                .Replace("ReqAutoSequenceName", "ReqAutoSequence").Replace("StagingAutoSequenceName", "StagingAutoSequence").Replace("TransferAutoSequenceName", "TransferAutoSequence")
                .Replace("WorkOrderAutoSequenceName", "WorkOrderAutoSequence").Replace("TAOAutoSequenceName", "TAOAutoSequence").Replace("ToolCountAutoSequenceName", "ToolCountAutoSequence")
                .Replace("PullRejectionTypeName", "PullRejectionType").Replace("ReplenishmentTypeName", "ReplenishmentType").Replace("BaseOfInventoryName", "BaseOfInventory");

            lstRoomDTO = obj.GetRoomHistoryByIDsNormal(ids, SessionHelper.CompanyID, SessionHelper.EnterPriceID).OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);

            csw.WriteField("HistoryAction");
            csw.WriteField("RoomName");
            csw.WriteField("CompanyName");
            csw.WriteField("ContactName");
            csw.WriteField("Address");
            csw.WriteField("City");
            csw.WriteField("State");
            csw.WriteField("PostalCode");
            csw.WriteField("Country");
            csw.WriteField("Email");
            csw.WriteField("Phone");
            csw.WriteField("IsRoomActive");
            csw.WriteField("InvoiceBranch");
            csw.WriteField("CustomerNumber");
            csw.WriteField("BlanketPO");
            csw.WriteField("LastOrderDate");
            csw.WriteField("LastPullDate");
            csw.WriteField("IsConsignment");
            csw.WriteField("Trending");
            csw.WriteField("Value1");
            csw.WriteField("Value2");
            csw.WriteField("Value3");
            csw.WriteField("SuggestedOrder");
            csw.WriteField("SuggestedTransfer");
            csw.WriteField("ReplineshmentRoom");
            csw.WriteField("IseVMI");
            csw.WriteField("MaxOrderSize");
            csw.WriteField("HighPO");
            csw.WriteField("HighJob");
            csw.WriteField("HighTransfer");
            csw.WriteField("HighCount");
            csw.WriteField("GlobalMarkupParts");
            csw.WriteField("GlobalMarkupLabor");
            csw.WriteField("Tax1Parts");
            csw.WriteField("Tax1Labor");
            csw.WriteField("Tax1name");
            csw.WriteField("Tax1percent");
            csw.WriteField("Tax2Parts");
            csw.WriteField("Tax2Labor");
            csw.WriteField("Tax2name");
            csw.WriteField("Tax2percent");
            csw.WriteField("Tax2onTax1");
            csw.WriteField("GXPRConsJob");
            csw.WriteField("CostCenter");
            csw.WriteField("UniqueID");
            csw.WriteField("eVMIWaitCommand");
            csw.WriteField("eVMIWaitPort");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("BillingRoomType");
            csw.WriteField("CountAutoSequence");
            csw.WriteField("NextCountNo");
            csw.WriteField("POAutoSequence");
            csw.WriteField("POAutoNrFixedValue");
            csw.WriteField("NextOrderNo");
            csw.WriteField("PullPurchaseNumberType");
            csw.WriteField("PullPurchaseNrFixedValue");
            csw.WriteField("LastPullPurchaseNumberUsed");
            csw.WriteField("RequisitionAutoSequence");
            csw.WriteField("ReqAutoNrFixedValue");
            csw.WriteField("NextRequisitionNo");
            csw.WriteField("IsAllowRequisitionDuplicate");
            csw.WriteField("IsAllowOrderDuplicate");
            csw.WriteField("IsAllowWorkOrdersDuplicate");
            csw.WriteField("StagingAutoSequence");
            csw.WriteField("StagingAutoNrFixedValue");
            csw.WriteField("NextStagingNo");
            csw.WriteField("TransferAutoSequence");
            csw.WriteField("TransferAutoNrFixedValue");
            csw.WriteField("NextTransferNo");
            csw.WriteField("WorkOrderAutoSequence");
            csw.WriteField("WorkOrderAutoNrFixedValue");
            csw.WriteField("NextWorkOrderNo");
            csw.WriteField("TAOAutoSequence");
            csw.WriteField("NextToolAssetOrderNo");
            csw.WriteField("ToolCountAutoSequence");
            csw.WriteField("NextToolCountNo");
            csw.WriteField("AllowInsertingItemOnScan");
            csw.WriteField("AllowPullBeyondAvailableQty");
            csw.WriteField("PullRejectionType");
            csw.WriteField("ReplenishmentType");
            csw.WriteField("DefaultBinID");
            csw.WriteField("DefaultSupplier");
            csw.WriteField("SlowMovingValue");
            csw.WriteField("FastMovingValue");
            csw.WriteField("InventoryConsuptionMethod");
            csw.WriteField("ValuingInventoryMethod");
            csw.WriteField("BaseOfInventory");
            csw.WriteField("LicenseBilled");
            csw.WriteField("IsProjectSpendMandatory");
            csw.WriteField("WarnUserOnAssigningNonDefaultBin");
            csw.WriteField("IsWOSignatureRequired");
            csw.WriteField("IsIgnoreCreditRule");
            csw.WriteField("ForceSupplierFilter");
            csw.WriteField("IsAllowOrderCostuom");
            csw.WriteField("RequestedXDays");
            csw.WriteField("ShelfLifeleadtimeOrdRpt");
            csw.WriteField("LeadTimeOrdRpt");
            csw.WriteField("MaintenanceDueNoticeDays");
            csw.WriteField("DefaultRequisitionRequiredDays");
            csw.WriteField("AttachingWOWithRequisition");
            csw.WriteField("PreventMaxOrderQty");
            csw.WriteField("DefaultCountType");
            csw.WriteField("SuggestedReturn");


            csw.NextRecord();
            if (lstRoomDTO != null && lstRoomDTO.Count > 0)
            {


                foreach (RoomHistoryDTO rec in ConvertExportDataList<RoomHistoryDTO>(lstRoomDTO).ToList())
                {

                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.CompanyName);
                    csw.WriteField(rec.ContactName);
                    csw.WriteField(rec.streetaddress);
                    csw.WriteField(rec.City);
                    csw.WriteField(rec.State);
                    csw.WriteField(rec.PostalCode);
                    csw.WriteField(rec.Country);
                    csw.WriteField(rec.Email);
                    csw.WriteField(rec.PhoneNo);
                    csw.WriteField(rec.IsRoomActive == true ? "Yes" : "No");
                    csw.WriteField(rec.InvoiceBranch);
                    csw.WriteField(rec.CustomerNumber);
                    csw.WriteField(rec.BlanketPO);
                    csw.WriteField(rec.LastOrderDate);
                    csw.WriteField(rec.LastPullDate);
                    csw.WriteField(rec.IsConsignment == true ? "Yes" : "No");
                    csw.WriteField(rec.IsTrending == true ? "Yes" : "No");
                    csw.WriteField(rec.TrendingFormulaDays);
                    csw.WriteField(rec.TrendingFormulaOverDays);
                    csw.WriteField(rec.TrendingFormulaAvgDays);
                    csw.WriteField(rec.SuggestedOrder == true ? "Yes" : "No");
                    csw.WriteField(rec.SuggestedTransfer == true ? "Yes" : "No");
                    csw.WriteField(rec.ReplineshmentRoom);
                    csw.WriteField(rec.IseVMI == true ? "Yes" : "No");
                    csw.WriteField(rec.MaxOrderSize);
                    csw.WriteField(rec.HighPO);
                    csw.WriteField(rec.HighJob);
                    csw.WriteField(rec.HighTransfer);
                    csw.WriteField(rec.HighCount);
                    csw.WriteField(rec.GlobMarkupParts);
                    csw.WriteField(rec.GlobMarkupLabor);
                    csw.WriteField(rec.IsTax1Parts == true ? "Yes" : "No");
                    csw.WriteField(rec.IsTax1Labor == true ? "Yes" : "No");
                    csw.WriteField(rec.Tax1name);
                    csw.WriteField(rec.Tax1Rate);
                    csw.WriteField(rec.IsTax2Parts == true ? "Yes" : "No");
                    csw.WriteField(rec.IsTax2Labor == true ? "Yes" : "No");
                    csw.WriteField(rec.tax2name);
                    csw.WriteField(rec.Tax2Rate);
                    csw.WriteField(rec.IsTax2onTax1 == true ? "Yes" : "No");
                    csw.WriteField(rec.GXPRConsJob);
                    csw.WriteField(rec.CostCenter);
                    csw.WriteField(rec.UniqueID);
                    csw.WriteField(rec.eVMIWaitCommand);
                    csw.WriteField(rec.eVMIWaitPort);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.BillingRoomTypeStr);
                    csw.WriteField(rec.CountAutoSequence);
                    csw.WriteField(rec.NextCountNo);
                    csw.WriteField(rec.POAutoSequenceStr);
                    csw.WriteField(rec.POAutoNrFixedValue);
                    csw.WriteField(rec.NextOrderNo);
                    csw.WriteField(rec.PullPurchaseNumberTypeStr);
                    csw.WriteField(rec.PullPurchaseNrFixedValue);
                    csw.WriteField(rec.LastPullPurchaseNumberUsed);
                    csw.WriteField(rec.ReqAutoSequenceStr);
                    csw.WriteField(rec.ReqAutoNrFixedValue);
                    csw.WriteField(rec.NextRequisitionNo);
                    csw.WriteField(rec.IsAllowRequisitionDuplicate == true ? "Yes" : "No");
                    csw.WriteField(rec.IsAllowOrderDuplicate == true ? "Yes" : "No");
                    csw.WriteField(rec.IsAllowWorkOrdersDuplicate == true ? "Yes" : "No");
                    csw.WriteField(rec.StagingAutoSequenceStr);
                    csw.WriteField(rec.StagingAutoNrFixedValue);
                    csw.WriteField(rec.NextStagingNo);
                    csw.WriteField(rec.TransferAutoSequenceStr);
                    csw.WriteField(rec.TransferAutoNrFixedValue);
                    csw.WriteField(rec.NextTransferNo);
                    csw.WriteField(rec.WorkOrderAutoSequenceStr);
                    csw.WriteField(rec.WorkOrderAutoNrFixedValue);
                    csw.WriteField(rec.NextWorkOrderNo);
                    csw.WriteField(rec.TAOAutoSequenceStr);
                    csw.WriteField(rec.NextToolAssetOrderNo);
                    csw.WriteField(rec.ToolCountAutoSequenceStr);
                    csw.WriteField(rec.NextToolCountNo);
                    csw.WriteField(rec.AllowInsertingItemOnScan == true ? "Yes" : "No");
                    csw.WriteField(rec.AllowPullBeyondAvailableQty == true ? "Yes" : "No");
                    csw.WriteField(rec.PullRejectionTypeStr);
                    csw.WriteField(rec.ReplenishmentTypeStr);
                    csw.WriteField(rec.DefaultBinName);
                    csw.WriteField(rec.DefaultSupplierName);
                    csw.WriteField(rec.SlowMovingValue);
                    csw.WriteField(rec.FastMovingValue);
                    csw.WriteField(rec.InventoryConsuptionMethod);
                    csw.WriteField(rec.MethodOfValuingInventoryStr);
                    csw.WriteField(rec.BaseOfInventoryStr);
                    csw.WriteField(rec.LicenseBilled);
                    csw.WriteField(rec.IsProjectSpendMandatory == true ? "Yes" : "No");
                    csw.WriteField(rec.WarnUserOnAssigningNonDefaultBin == true ? "Yes" : "No");
                    csw.WriteField(rec.IsWOSignatureRequired == true ? "Yes" : "No");
                    csw.WriteField(rec.IsIgnoreCreditRule == true ? "Yes" : "No");
                    csw.WriteField(rec.ForceSupplierFilter == true ? "Yes" : "No");
                    csw.WriteField(rec.IsAllowOrderCostuom == true ? "Yes" : "No");
                    csw.WriteField(rec.RequestedXDays);
                    csw.WriteField(rec.ShelfLifeleadtimeOrdRpt);
                    csw.WriteField(rec.LeadTimeOrdRpt);
                    csw.WriteField(rec.MaintenanceDueNoticeDays);
                    csw.WriteField(rec.DefaultRequisitionRequiredDays);
                    csw.WriteField(rec.AttachingWOWithRequisitionStr);
                    csw.WriteField(rec.PreventMaxOrderQtyStr);
                    csw.WriteField(rec.DefaultCountTypeStr);
                    csw.WriteField(rec.SuggestedReturn == true ? "Yes" : "No");

                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string DeletedRoomListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            RoomDAL obj = new RoomDAL(SessionHelper.EnterPriseDBName);
            List<RoomDTO> lstRoomDTO = new List<RoomDTO>();
            lstRoomDTO = obj.GetDeletedRoomByIDs(ids).OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);

            csw.WriteField("ID");
            csw.WriteField("RoomName");
            csw.WriteField("EnterpriseName");
            csw.WriteField("CompanyName");
            csw.WriteField("ContactName");
            csw.WriteField("streetaddress");
            csw.WriteField("City");
            csw.WriteField("State");
            csw.WriteField("PostalCode");
            csw.WriteField("Country");
            csw.WriteField("Email");
            csw.WriteField("PhoneNo");
            csw.WriteField("IsRoomActive");
            csw.WriteField("CustomerNumber");
            csw.WriteField("BlanketPO");
            csw.WriteField("LastOrderDate");
            csw.WriteField("LastPullDate");
            csw.WriteField("IsConsignment");
            csw.WriteField("IsTrending");
            csw.WriteField("TrendingFormulaDays");
            csw.WriteField("TrendingFormulaOverDays");
            csw.WriteField("TrendingFormulaAvgDays");
            csw.WriteField("SuggestedOrder");
            csw.WriteField("SuggestedTransfer");
            csw.WriteField("ReplineshmentRoom");
            csw.WriteField("IseVMI");
            csw.WriteField("MaxOrderSize");
            csw.WriteField("HighPO");
            csw.WriteField("HighJob");
            csw.WriteField("HighTransfer");
            csw.WriteField("HighCount");
            csw.WriteField("GlobMarkupParts");
            csw.WriteField("GlobMarkupLabor");
            csw.WriteField("IsTax1Parts");
            csw.WriteField("IsTax1Labor");
            csw.WriteField("tax1name");
            csw.WriteField("Tax1Rate");
            csw.WriteField("IsTax2Parts");
            csw.WriteField("IsTax2Labor");
            csw.WriteField("tax2name");
            csw.WriteField("Tax2Rate");
            csw.WriteField("IsTax2onTax1");
            csw.WriteField("GXPRConsJob");
            csw.WriteField("CostCenter");
            csw.WriteField("UniqueID");
            csw.WriteField("eVMIWaitCommand");
            csw.WriteField("eVMIWaitPort");
            csw.WriteField("Created");
            csw.WriteField("Updated");
            csw.WriteField("UpdatedByName");
            csw.WriteField("CreatedByName");
            csw.WriteField("ActiveOn");
            csw.WriteField("LastSyncDateTime");
            csw.WriteField("PDABuildVersion");
            csw.WriteField("LastSyncUserName");
            csw.WriteField("BillingRoomType");
            csw.WriteField("AttachingWOWithRequisition");
            csw.WriteField("PreventMaxOrderQty");
            csw.WriteField("DefaultCountType");
            csw.WriteField("IsWOSignatureRequired");
            csw.WriteField("IsIgnoreCreditRule");
            csw.WriteField("ForceSupplierFilter");
            csw.WriteField("SuggestedReturn");
            csw.WriteField("NoOfItems");
            if (SessionHelper.RoleID == -1)
            {
                csw.WriteField("ReportAppIntent");
            }

            csw.NextRecord();
            if (lstRoomDTO != null && lstRoomDTO.Count > 0)
            {
                foreach (RoomDTO item in ConvertExportDataList<RoomDTO>(lstRoomDTO).ToList())
                {
                    csw.WriteField(item.ID);
                    csw.WriteField(item.RoomName);
                    csw.WriteField(item.EnterpriseName);
                    csw.WriteField(item.CompanyName);
                    csw.WriteField(item.ContactName);
                    csw.WriteField(item.streetaddress);
                    csw.WriteField(item.City);
                    csw.WriteField(item.State);
                    csw.WriteField(item.PostalCode);
                    csw.WriteField(item.Country);
                    csw.WriteField(item.Email);
                    csw.WriteField(item.PhoneNo);
                    csw.WriteField(item.IsRoomActive == true ? "Yes" : "No");
                    csw.WriteField(item.CustomerNumber);
                    csw.WriteField(item.BlanketPO);
                    string LastOrderDate = string.Empty;
                    if (item.LastOrderDate.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                    {
                        LastOrderDate = Convert.ToString(item.LastOrderDate.GetValueOrDefault(DateTime.MinValue));
                    }
                    csw.WriteField(LastOrderDate);

                    string LastPullDate = string.Empty;
                    if (item.LastPullDate.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                    {
                        LastPullDate = Convert.ToString(item.LastPullDate.GetValueOrDefault(DateTime.MinValue));
                    }
                    csw.WriteField(LastPullDate);
                    csw.WriteField(item.IsConsignment == true ? "Yes" : "No");
                    csw.WriteField(item.IsTrending == true ? "Yes" : "No");
                    csw.WriteField(item.TrendingFormulaDays ?? 0);
                    csw.WriteField(item.TrendingFormulaOverDays ?? 0);
                    csw.WriteField(item.TrendingFormulaAvgDays ?? 0);
                    csw.WriteField(item.SuggestedOrder == true ? "Yes" : "No");
                    csw.WriteField(item.SuggestedTransfer == true ? "Yes" : "No");
                    csw.WriteField(item.ReplineshmentRoom ?? 0);
                    csw.WriteField(item.IseVMI == true ? "Yes" : "No");
                    csw.WriteField(item.MaxOrderSize ?? 0);
                    csw.WriteField(item.HighPO);
                    csw.WriteField(item.HighJob);
                    csw.WriteField(item.HighTransfer);
                    csw.WriteField(item.HighCount);
                    csw.WriteField(item.GlobMarkupParts ?? 0);
                    csw.WriteField(item.GlobMarkupLabor ?? 0);
                    csw.WriteField(item.IsTax1Parts == true ? "Yes" : "No");
                    csw.WriteField(item.IsTax1Labor == true ? "Yes" : "No");
                    csw.WriteField(item.Tax1name);
                    csw.WriteField(item.Tax1Rate ?? 0);
                    csw.WriteField(item.IsTax2Parts == true ? "Yes" : "No");
                    csw.WriteField(item.IsTax2Labor == true ? "Yes" : "No");
                    csw.WriteField(item.tax2name);
                    csw.WriteField(item.Tax2Rate ?? 0);
                    csw.WriteField(item.IsTax2onTax1 == true ? "Yes" : "No");
                    csw.WriteField(item.GXPRConsJob);
                    csw.WriteField(item.CostCenter);
                    csw.WriteField(item.UniqueID);
                    csw.WriteField(item.eVMIWaitCommand);
                    csw.WriteField(item.eVMIWaitPort);

                    string Created = string.Empty;
                    if (item.Created.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                    {
                        Created = Convert.ToString(item.Created.GetValueOrDefault(DateTime.MinValue));
                    }
                    csw.WriteField(Created);

                    string Updated = string.Empty;
                    if (item.Updated.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                    {
                        Updated = Convert.ToString(item.Updated.GetValueOrDefault(DateTime.MinValue));
                    }
                    csw.WriteField(Updated);
                    csw.WriteField(item.UpdatedByName);
                    csw.WriteField(item.CreatedByName);

                    string ActiveOn = string.Empty;
                    if (item.ActiveOn.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                    {
                        ActiveOn = Convert.ToString(item.ActiveOn.GetValueOrDefault(DateTime.MinValue));
                    }
                    csw.WriteField(ActiveOn);

                    string LastSyncDateTime = string.Empty;
                    if (item.LastSyncDateTime.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                    {
                        LastSyncDateTime = Convert.ToString(item.LastSyncDateTime.GetValueOrDefault(DateTime.MinValue));
                    }
                    csw.WriteField(LastSyncDateTime);
                    csw.WriteField(item.PDABuildVersion);
                    csw.WriteField(item.LastSyncUserName);
                    csw.WriteField(item.BillingRoomType ?? 0);
                    csw.WriteField(item.AttachingWOWithRequisition ?? (int)AttachingWOWithRequisition.New);
                    csw.WriteField(item.PreventMaxOrderQty);
                    csw.WriteField(item.DefaultCountType);
                    csw.WriteField(item.IsWOSignatureRequired);
                    csw.WriteField(item.IsIgnoreCreditRule);
                    csw.WriteField(item.ForceSupplierFilter);
                    csw.WriteField(item.SuggestedReturn == true ? "Yes" : "No");
                    csw.WriteField(item.NoOfItems ?? 0);
                    if (SessionHelper.RoleID == -1)
                    {
                        csw.WriteField(item.ReportAppIntent);
                    }
                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string UserHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            UserMasterDAL obj = new UserMasterDAL(SessionHelper.EnterPriseDBName);
            List<UserRoleModuleDetailsDTO> lstUserDTO = new List<UserRoleModuleDetailsDTO>();
            lstUserDTO = obj.GetUserHistoryByIdNormal(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryID");
            csw.WriteField("CompanyName");
            csw.WriteField("RoomName");
            csw.WriteField("Role");
            csw.WriteField("HistoryDate");
            csw.WriteField("PermissionChanges");

            csw.NextRecord();
            if (lstUserDTO != null && lstUserDTO.Count > 0)
            {
                foreach (UserRoleModuleDetailsDTO rec in ConvertExportDataList<UserRoleModuleDetailsDTO>(lstUserDTO).ToList())
                {
                    csw.WriteField(rec.UserRoleChangeLogID);
                    csw.WriteField(rec.CompanyName);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.RoleName);
                    csw.WriteField(rec.HistoryDate);
                    csw.WriteField(rec.PermissionChanges);
                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string CompanyHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            CompanyMasterDAL obj = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
            List<CompanyMasterDTO> lstCompanyDTO = new List<CompanyMasterDTO>();
            lstCompanyDTO = obj.GetCompanyMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField(ResCommon.HistoryID);
            csw.WriteField(ResCommon.HistoryAction);
            csw.WriteField(ResCommon.CompanyName);
            csw.WriteField(ResCommon.Address);
            csw.WriteField(ResCommon.City);
            csw.WriteField(ResCommon.State);
            csw.WriteField(ResCommon.PostalCode);
            csw.WriteField(ResCommon.Country);
            csw.WriteField(ResCommon.Phone);
            csw.WriteField(ResCommon.Email);
            csw.WriteField(ResCommon.CreatedOn);
            csw.WriteField(ResCommon.UpdatedOn);
            csw.WriteField(ResCommon.UpdatedBy);
            csw.WriteField(ResCommon.CreatedBy);
            csw.WriteField(ResCommon.IsActive);



            csw.NextRecord();
            if (lstCompanyDTO != null && lstCompanyDTO.Count > 0)
            {
                foreach (CompanyMasterDTO rec in ConvertExportDataList<CompanyMasterDTO>(lstCompanyDTO).ToList())
                {
                    csw.WriteField(rec.HistoryID);
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.Name);
                    csw.WriteField(rec.Address);
                    csw.WriteField(rec.City);
                    csw.WriteField(rec.State);
                    csw.WriteField(rec.PostalCode);
                    csw.WriteField(rec.Country);
                    csw.WriteField(rec.ContactPhone);
                    csw.WriteField(rec.ContactEmail);
                    csw.WriteField(rec.CreatedDate);
                    csw.WriteField(rec.UpdatedDate);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.IsActive);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string AssetCategoryHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            AssetCategoryMasterDAL obj = new AssetCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            List<AssetCategoryMasterDTO> lstAssetCatDTO = new List<AssetCategoryMasterDTO>();
            lstAssetCatDTO = obj.GetAssetCategoryMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("AssetCategory");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstAssetCatDTO != null && lstAssetCatDTO.Count > 0)
            {
                foreach (AssetCategoryMasterDTO rec in ConvertExportDataList<AssetCategoryMasterDTO>(lstAssetCatDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.AssetCategory);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);


                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string BinHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            BinMasterDAL obj = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<BinMasterDTO> lstBinDTO = new List<BinMasterDTO>();
            lstBinDTO = obj.GetBinMasterByIdNormal(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("BinNumber");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstBinDTO != null && lstBinDTO.Count > 0)
            {
                foreach (BinMasterDTO rec in ConvertExportDataList<BinMasterDTO>(lstBinDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.BinNumber);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.LastUpdated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);


                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string CategoryHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            CategoryMasterDAL obj = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
            List<CategoryMasterDTO> lstCategoryDTO = new List<CategoryMasterDTO>();
            lstCategoryDTO = obj.GetCategoryMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("Category");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstCategoryDTO != null && lstCategoryDTO.Count > 0)
            {
                foreach (CategoryMasterDTO rec in ConvertExportDataList<CategoryMasterDTO>(lstCategoryDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.Category);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnWebDate);


                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string CostUOMHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            CostUOMMasterDAL obj = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
            List<CostUOMMasterDTO> lstCostUOMDTO = new List<CostUOMMasterDTO>();
            lstCostUOMDTO = obj.GetCostUOMMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("CostUOM");
            csw.WriteField("CostUOMValue");
            csw.WriteField("RoomName");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");

            csw.NextRecord();
            if (lstCostUOMDTO != null && lstCostUOMDTO.Count > 0)
            {
                foreach (CostUOMMasterDTO rec in ConvertExportDataList<CostUOMMasterDTO>(lstCostUOMDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.CostUOM);
                    csw.WriteField(rec.CostUOMValue);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnWebDate);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string CustomerHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            CustomerMasterDAL obj = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            List<CustomerMasterDTO> lstCustomerDTO = new List<CustomerMasterDTO>();
            lstCustomerDTO = obj.GetCustomerMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("Customer");
            csw.WriteField("Account");
            csw.WriteField("Address");
            csw.WriteField("City");
            csw.WriteField("State");
            csw.WriteField("Country");
            csw.WriteField("ZipCode");
            csw.WriteField("Contact");
            csw.WriteField("Email");
            csw.WriteField("Phone");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstCustomerDTO != null && lstCustomerDTO.Count > 0)
            {
                foreach (CustomerMasterDTO rec in ConvertExportDataList<CustomerMasterDTO>(lstCustomerDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.Customer);
                    csw.WriteField(rec.Account);
                    csw.WriteField(rec.Address);
                    csw.WriteField(rec.City);
                    csw.WriteField(rec.State);
                    csw.WriteField(rec.Country);
                    csw.WriteField(rec.ZipCode);
                    csw.WriteField(rec.Contact);
                    csw.WriteField(rec.Email);
                    csw.WriteField(rec.Phone);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);


                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }

        public string FTPHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            SFTPDAL obj = new SFTPDAL(SessionHelper.EnterPriseDBName);
            List<FTPMasterDTO> lstFTPDTO = new List<FTPMasterDTO>();
            lstFTPDTO = obj.GetFTPMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("SFtpName");
            csw.WriteField("ServerAddress");
            csw.WriteField("UserName");
            csw.WriteField("Password");
            csw.WriteField("Port");
            csw.WriteField("IsImportFTP");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");

            csw.NextRecord();
            if (lstFTPDTO != null && lstFTPDTO.Count > 0)
            {
                foreach (FTPMasterDTO rec in ConvertExportDataList<FTPMasterDTO>(lstFTPDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.SFtpName);
                    csw.WriteField(rec.ServerAddress);
                    csw.WriteField(rec.UserName);
                    csw.WriteField(rec.Password);
                    csw.WriteField(rec.Port);
                    csw.WriteField(rec.IsImportFTP);
                    csw.WriteField(rec.RoomName);
                    string strCreated = CommonUtility.ConvertDateByTimeZone((DateTime)rec.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    string strUpdated = CommonUtility.ConvertDateByTimeZone((DateTime)rec.LastUpdated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    csw.WriteField(strCreated.ToString());
                    csw.WriteField(strUpdated.ToString());
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }

        public string GLAccountHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            GLAccountMasterDAL obj = new GLAccountMasterDAL(SessionHelper.EnterPriseDBName);
            List<GLAccountMasterDTO> lstGLAccountDTO = new List<GLAccountMasterDTO>();
            lstGLAccountDTO = obj.GetGLAccountMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("GLAccount");
            csw.WriteField("Description");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.NextRecord();
            if (lstGLAccountDTO != null && lstGLAccountDTO.Count > 0)
            {
                foreach (GLAccountMasterDTO rec in ConvertExportDataList<GLAccountMasterDTO>(lstGLAccountDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.GLAccount);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.UpdatedByName);
                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string InventoryClassificationHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            InventoryClassificationMasterDAL obj = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
            List<InventoryClassificationMasterDTO> lstInventoryClassficationDTO = new List<InventoryClassificationMasterDTO>();
            lstInventoryClassficationDTO = obj.GetInventoryClassificationMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("InventoryClassification");
            csw.WriteField("RangeStart");
            csw.WriteField("RangeEnd");
            csw.WriteField("RoomName");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");

            csw.NextRecord();
            if (lstInventoryClassficationDTO != null && lstInventoryClassficationDTO.Count > 0)
            {
                foreach (InventoryClassificationMasterDTO rec in ConvertExportDataList<InventoryClassificationMasterDTO>(lstInventoryClassficationDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.InventoryClassification);
                    csw.WriteField(rec.RangeStart);
                    csw.WriteField(rec.RangeEnd);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnWebDate);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string LocationHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            LocationMasterDAL obj = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            List<LocationMasterDTO> lstLocationDTO = new List<LocationMasterDTO>();
            lstLocationDTO = obj.GetLocationMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("Location");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy"); ;
            csw.NextRecord();
            if (lstLocationDTO != null && lstLocationDTO.Count > 0)
            {
                foreach (LocationMasterDTO rec in ConvertExportDataList<LocationMasterDTO>(lstLocationDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.Location);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created.ToString());
                    csw.WriteField(rec.LastUpdated.ToString());
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string ManufacturerHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ManufacturerMasterDAL obj = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            List<ManufacturerMasterDTO> lstManufacturerDTO = new List<ManufacturerMasterDTO>();
            lstManufacturerDTO = obj.GetManufacturerMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("ManufacturerName");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");
            csw.NextRecord();
            if (lstManufacturerDTO != null && lstManufacturerDTO.Count > 0)
            {
                foreach (ManufacturerMasterDTO rec in ConvertExportDataList<ManufacturerMasterDTO>(lstManufacturerDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.Manufacturer);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);
                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string SupplierHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            SupplierMasterDAL obj = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            List<SupplierMasterDTO> lstSupplierDTO = new List<SupplierMasterDTO>();
            lstSupplierDTO = obj.GetSupplierMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("Supplier");
            csw.WriteField("Description");
            csw.WriteField("ReceiverID");
            csw.WriteField("Address");
            csw.WriteField("City");
            csw.WriteField("State");
            csw.WriteField("ZipCode");
            csw.WriteField("Country");
            csw.WriteField("Contact");
            csw.WriteField("Phone");
            csw.WriteField("Fax");
            csw.WriteField("Email");
            csw.WriteField("DefaultOrderRequiredDays");
            csw.WriteField("BranchNumber");
            csw.WriteField("MaximumOrderSize");
            csw.WriteField("IsSendtoVendor");
            csw.WriteField("IsVendorReturnAsn");
            csw.WriteField("IsSupplierReceivesKitComponents");
            csw.WriteField("EmailPOInBody");
            csw.WriteField("EmailPOInPDF");
            csw.WriteField("EmailPOInCSV");
            csw.WriteField("EmailPOInX12");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");
            csw.WriteField("Account");

            csw.NextRecord();
            if (lstSupplierDTO != null && lstSupplierDTO.Count > 0)
            {
                foreach (SupplierMasterDTO rec in ConvertExportDataList<SupplierMasterDTO>(lstSupplierDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.SupplierName);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.ReceiverID);
                    csw.WriteField(rec.Address);
                    csw.WriteField(rec.City);
                    csw.WriteField(rec.State);
                    csw.WriteField(rec.ZipCode);
                    csw.WriteField(rec.Country);
                    csw.WriteField(rec.Contact);
                    csw.WriteField(rec.Phone);
                    csw.WriteField(rec.Fax);
                    csw.WriteField(rec.Email);
                    csw.WriteField(rec.DefaultOrderRequiredDays);
                    csw.WriteField(rec.BranchNumber);
                    csw.WriteField(rec.MaximumOrderSize);
                    csw.WriteField(rec.IsSendtoVendor == true ? "Yes" : "No");
                    csw.WriteField(rec.IsVendorReturnAsn == true ? "Yes" : "No");
                    csw.WriteField(rec.IsSupplierReceivesKitComponents == true ? "Yes" : "No");
                    csw.WriteField(rec.IsEmailPOInBody == true ? "Yes" : "No");
                    csw.WriteField(rec.IsEmailPOInPDF == true ? "Yes" : "No");
                    csw.WriteField(rec.IsEmailPOInCSV == true ? "Yes" : "No");
                    csw.WriteField(rec.IsEmailPOInX12 == true ? "Yes" : "No");
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.LastUpdated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);
                    csw.WriteField(rec.AccountNo);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string TechnicianHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            TechnicialMasterDAL obj = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            List<TechnicianMasterDTO> lstTechnicianDTO = new List<TechnicianMasterDTO>();
            lstTechnicianDTO = obj.GetTechnicianMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("Technician");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("Action");
            csw.NextRecord();
            if (lstTechnicianDTO != null && lstTechnicianDTO.Count > 0)
            {
                foreach (TechnicianMasterDTO rec in ConvertExportDataList<TechnicianMasterDTO>(lstTechnicianDTO).ToList())
                {
                    csw.WriteField(rec.Technician);
                    csw.WriteField(rec.Created.ToString());
                    csw.WriteField(rec.Updated.ToString());
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.Action);
                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string ToolCategoryHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolCategoryMasterDAL obj = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolCategoryMasterDTO> lstToolCategoryDTO = new List<ToolCategoryMasterDTO>();
            lstToolCategoryDTO = obj.GetToolCategoryMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("ToolCategory");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstToolCategoryDTO != null && lstToolCategoryDTO.Count > 0)
            {
                foreach (ToolCategoryMasterDTO rec in ConvertExportDataList<ToolCategoryMasterDTO>(lstToolCategoryDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.ToolCategory);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);


                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string UnitHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            UnitMasterDAL obj = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            List<UnitMasterDTO> lstUnitDTO = new List<UnitMasterDTO>();
            lstUnitDTO = obj.GetUnitMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("Unit");
            csw.WriteField("Description");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstUnitDTO != null && lstUnitDTO.Count > 0)
            {
                foreach (UnitMasterDTO rec in ConvertExportDataList<UnitMasterDTO>(lstUnitDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.Unit);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);


                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string VenderHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            VenderMasterDAL obj = new VenderMasterDAL(SessionHelper.EnterPriseDBName);
            List<VenderMasterDTO> lstVenderDTO = new List<VenderMasterDTO>();
            lstVenderDTO = obj.GetVenderMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("Id");
            csw.WriteField("HistoryID");
            csw.WriteField("HistoryAction");
            csw.WriteField("Vender");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");


            csw.NextRecord();
            if (lstVenderDTO != null && lstVenderDTO.Count > 0)
            {
                foreach (VenderMasterDTO rec in ConvertExportDataList<VenderMasterDTO>(lstVenderDTO).ToList())
                {
                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.HistoryID);
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.Vender);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string QuickListHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            QuickListDAL obj = new QuickListDAL(SessionHelper.EnterPriseDBName);
            List<QuickListMasterDTO> lstQuickListDTO = new List<QuickListMasterDTO>();
            lstQuickListDTO = obj.GetQuickListMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("Name");
            csw.WriteField("FromWhere");
            csw.WriteField("Comment");
            csw.WriteField("ListType");
            csw.WriteField("NoOfItems");
            csw.WriteField("RoomName");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");


            csw.NextRecord();
            if (lstQuickListDTO != null && lstQuickListDTO.Count > 0)
            {
                foreach (QuickListMasterDTO rec in ConvertExportDataList<QuickListMasterDTO>(lstQuickListDTO).ToList())
                {
                    string Type = rec.Type == 1 ? "General" : (rec.Type == 2 ? "Asset" : "");
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.Name);
                    csw.WriteField(rec.WhatWhereAction);
                    csw.WriteField(rec.Comment);
                    csw.WriteField(Type);
                    csw.WriteField(rec.NoOfItems);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.LastUpdated);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string CountHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            InventoryCountDAL obj = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            List<InventoryCountDTO> lstCountDTO = new List<InventoryCountDTO>();
            lstCountDTO = obj.GetInventoryCountChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("CountName");
            csw.WriteField("ReleaseNumber");
            csw.WriteField("CountDate");
            csw.WriteField("CountItemDescription");
            csw.WriteField("CountType");
            csw.WriteField("IsApplied");
            csw.WriteField("TotalItemsWithinCount");
            csw.WriteField("IsClosed");
            csw.WriteField("Created");
            csw.WriteField("Updated");
            csw.WriteField("LastUpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstCountDTO != null && lstCountDTO.Count > 0)
            {
                foreach (InventoryCountDTO rec in ConvertExportDataList<InventoryCountDTO>(lstCountDTO).ToList())
                {
                    csw.WriteField(rec.CountName);
                    csw.WriteField(rec.ReleaseNumber);
                    csw.WriteField(rec.CountDate);
                    csw.WriteField(rec.CountItemDescription);
                    csw.WriteField(rec.CountType);
                    csw.WriteField(rec.IsApplied == true ? "Yes" : "No");
                    csw.WriteField(rec.TotalItemsWithinCount);
                    csw.WriteField(rec.IsClosed == true ? "Yes" : "No");
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string MaterialStagingHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            MaterialStagingDAL obj = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
            List<MaterialStagingDTO> lstMaterialStagingDTO = new List<MaterialStagingDTO>();
            lstMaterialStagingDTO = obj.GetMaterialStagingChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("StagingName");
            csw.WriteField("From Where");
            csw.WriteField("Description");
            csw.WriteField("BinName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("CreatedBy");
            csw.WriteField("UpdatedBy");
            csw.WriteField("RoomName");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstMaterialStagingDTO != null && lstMaterialStagingDTO.Count > 0)
            {
                foreach (MaterialStagingDTO rec in ConvertExportDataList<MaterialStagingDTO>(lstMaterialStagingDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.StagingName);
                    csw.WriteField(rec.WhatWhereAction);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.StagingLocationName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string RequisitionHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            RequisitionMasterDAL obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            List<RequisitionMasterDTO> lstRequisitionDTO = new List<RequisitionMasterDTO>();
            lstRequisitionDTO = obj.GetRequisitionMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("RequisitionStatus");
            csw.WriteField("From Where");
            csw.WriteField("RequisitionNumber");
            csw.WriteField("ReleaseNumber");
            csw.WriteField("Description");
            csw.WriteField("RequiredDate");
            csw.WriteField("NumberofItemsrequisitioned");
            csw.WriteField("Customer");
            csw.WriteField("RequisitionType");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("CreatedBy");
            csw.WriteField("UpdatedBy");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstRequisitionDTO != null && lstRequisitionDTO.Count > 0)
            {
                foreach (RequisitionMasterDTO rec in ConvertExportDataList<RequisitionMasterDTO>(lstRequisitionDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.RequisitionStatus);
                    csw.WriteField(rec.WhatWhereAction);
                    csw.WriteField(rec.RequisitionNumber);
                    csw.WriteField(rec.ReleaseNumber);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.RequiredDate);
                    csw.WriteField(rec.NumberofItemsrequisitioned);
                    csw.WriteField(rec.Customer);
                    csw.WriteField(rec.RequisitionType);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnWebDate);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string WorkOrderHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            List<WorkOrderDTO> lstWorkOrderDTO = new List<WorkOrderDTO>();
            lstWorkOrderDTO = obj.GetWorkOrderMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("WOName");
            csw.WriteField("ReleaseNumber");
            csw.WriteField("From Where");
            csw.WriteField("WOStatus");
            csw.WriteField("Technician");
            csw.WriteField("Customer");
            csw.WriteField("AssetName");
            csw.WriteField("ToolName");
            csw.WriteField("UsedItems");
            csw.WriteField("UsedItemsCost");
            csw.WriteField("Description");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("Created");
            csw.WriteField("Updated");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstWorkOrderDTO != null && lstWorkOrderDTO.Count > 0)
            {
                foreach (WorkOrderDTO rec in ConvertExportDataList<WorkOrderDTO>(lstWorkOrderDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.WOName);
                    csw.WriteField(rec.ReleaseNumber);
                    csw.WriteField(rec.WhatWhereAction);
                    csw.WriteField(rec.WOStatus);
                    csw.WriteField(rec.Technician);
                    csw.WriteField(rec.Customer);
                    csw.WriteField(rec.AssetName);
                    csw.WriteField(rec.ToolName);
                    csw.WriteField(rec.UsedItems);
                    csw.WriteField(rec.UsedItemsCost);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnWebDate);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string ProjectSpendHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ProjectMasterDAL obj = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            List<ProjectMasterDTO> lstProjectSpendDTO = new List<ProjectMasterDTO>();
            lstProjectSpendDTO = obj.GetProjectSpendMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("ProjectSpendName");
            csw.WriteField("From Where");
            csw.WriteField("Description");
            csw.WriteField("DollarLimitAmount");
            csw.WriteField("DollarUsedAmount");
            csw.WriteField("TrackAllUsageAgainstThis");
            csw.WriteField("IsClosed");
            csw.WriteField("RoomName");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("Created");
            csw.WriteField("Updated");

            csw.NextRecord();
            if (lstProjectSpendDTO != null && lstProjectSpendDTO.Count > 0)
            {
                foreach (ProjectMasterDTO rec in ConvertExportDataList<ProjectMasterDTO>(lstProjectSpendDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.ProjectSpendName);
                    csw.WriteField(rec.WhatWhereAction);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.DollarLimitAmount);
                    csw.WriteField(rec.DollarUsedAmount);
                    csw.WriteField(rec.TrackAllUsageAgainstThis == true ? "Yes" : "No");
                    csw.WriteField(rec.IsClosed == true ? "Yes" : "No");
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnWebDate);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.UpdatedByName);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string CartItemsHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            CartItemDAL obj = new CartItemDAL(SessionHelper.EnterPriseDBName);
            List<CartItemDTO> lstCartItemsDTO = new List<CartItemDTO>();
            lstCartItemsDTO = obj.GetCartItemsMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("ItemNumber");
            csw.WriteField("Quantity");
            csw.WriteField("From Where");
            csw.WriteField("ReplenishType");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstCartItemsDTO != null && lstCartItemsDTO.Count > 0)
            {
                foreach (CartItemDTO rec in ConvertExportDataList<CartItemDTO>(lstCartItemsDTO).ToList())
                {
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.Quantity);
                    csw.WriteField(rec.WhatWhereAction);
                    csw.WriteField(rec.ReplenishType);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string OrdersHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            OrderMasterDAL obj = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            List<OrderMasterDTO> lstOrdersDTO = new List<OrderMasterDTO>();
            lstOrdersDTO = obj.GetOrdersMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("OrderNumber");
            csw.WriteField("From Where");
            csw.WriteField("ReleaseNumber");
            csw.WriteField("Shipping Method");
            csw.WriteField("Supplier");
            csw.WriteField("StagingName");
            csw.WriteField("Comment");
            csw.WriteField("RequiredDate");
            csw.WriteField("OrderStatus");
            csw.WriteField("RejectedReason");
            csw.WriteField("Customer");
            csw.WriteField("PackSlipNumber");
            csw.WriteField("ShippingTrackNumber");
            csw.WriteField("CreatedOn");
            csw.WriteField("CreatedBy");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("RoomName");
            csw.WriteField("IsArchived");
            csw.WriteField("IsDeleted");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstOrdersDTO != null && lstOrdersDTO.Count > 0)
            {
                foreach (OrderMasterDTO rec in ConvertExportDataList<OrderMasterDTO>(lstOrdersDTO).ToList())
                {
                    string OrderStatus = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)rec.OrderStatus).ToString());
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.OrderNumber);
                    csw.WriteField(rec.WhatWhereAction);
                    csw.WriteField(rec.ReleaseNumber);
                    csw.WriteField(rec.ShipVia);
                    csw.WriteField(rec.Supplier);
                    csw.WriteField(rec.StagingName);
                    csw.WriteField(rec.Comment);
                    csw.WriteField(rec.RequiredDate);
                    csw.WriteField(OrderStatus);
                    csw.WriteField(rec.RejectionReason);
                    csw.WriteField(rec.CustomerName);
                    csw.WriteField(rec.PackSlipNumber);
                    csw.WriteField(rec.ShippingTrackNumber);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.LastUpdated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.IsArchived == true ? "Yes" : "No");
                    csw.WriteField(rec.IsDeleted == true ? "Yes" : "No");
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string TransferHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            TransferMasterDAL obj = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            List<TransferMasterDTO> lstTransferDTO = new List<TransferMasterDTO>();
            lstTransferDTO = obj.GetTransferMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("TransferNumber");
            csw.WriteField("From Where");
            csw.WriteField("ReplinishRoom");
            csw.WriteField("StagingName");
            csw.WriteField("Comment");
            csw.WriteField("RequestType");
            csw.WriteField("TransferStatus");
            csw.WriteField("RejectedReason");
            csw.WriteField("RequireDate");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("CreatedBy");
            csw.WriteField("UpdatedBy");
            csw.WriteField("RoomName");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstTransferDTO != null && lstTransferDTO.Count > 0)
            {
                foreach (TransferMasterDTO rec in ConvertExportDataList<TransferMasterDTO>(lstTransferDTO).ToList())
                {
                    string TransferStatus = ResTransfer.GetTransferStatusText(((eTurns.DTO.TransferStatus)rec.TransferStatus).ToString());
                    string RequestType = (Enum.Parse(typeof(RequestType), rec.RequestType.ToString()).ToString());
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.TransferNumber);
                    csw.WriteField(rec.WhatWhereAction);
                    csw.WriteField(rec.ReplenishingRoomName);
                    csw.WriteField(rec.StagingName);
                    csw.WriteField(rec.Comment);
                    csw.WriteField(RequestType);
                    csw.WriteField(TransferStatus);
                    csw.WriteField(rec.RejectionReason);
                    csw.WriteField(rec.RequireDate);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnWebDate);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string Asset_ToolHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstToolDTO = new List<ToolMasterDTO>();
            lstToolDTO = obj.GetToolMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("ToolName");
            csw.WriteField("Serial");
            csw.WriteField("Description");
            csw.WriteField("IsGroupOfItems");
            csw.WriteField("Quantity");
            csw.WriteField("Cost");
            csw.WriteField("ToolCategory");
            csw.WriteField("Location");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");


            csw.NextRecord();
            if (lstToolDTO != null && lstToolDTO.Count > 0)
            {
                foreach (ToolMasterDTO rec in ConvertExportDataList<ToolMasterDTO>(lstToolDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.ToolName);
                    csw.WriteField(rec.Serial);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.IsGroupOfItems == 0 ? "No" : "Yes");
                    csw.WriteField(rec.Quantity);
                    csw.WriteField(rec.Cost ?? 0);
                    csw.WriteField(rec.ToolCategory);
                    csw.WriteField(rec.Location);
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created.ToString());
                    csw.WriteField(rec.Updated.ToString());
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string AssetsHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            AssetMasterDAL obj = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            List<AssetMasterDTO> lstAssetsDTO = new List<AssetMasterDTO>();
            lstAssetsDTO = obj.GetAssetMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryAction");
            csw.WriteField("AssetName");
            csw.WriteField("Description");
            csw.WriteField("Make");
            csw.WriteField("Model");
            csw.WriteField("Serial");
            csw.WriteField("ToolCategory");
            csw.WriteField("PurchaseDate");
            csw.WriteField("PurchasePrice");
            csw.WriteField("DepreciatedValue");
            csw.WriteField("SuggestedMaintenanceDate");
            csw.WriteField("RoomName");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("UpdatedBy");
            csw.WriteField("CreatedBy");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditedFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");

            csw.NextRecord();
            if (lstAssetsDTO != null && lstAssetsDTO.Count > 0)
            {
                foreach (AssetMasterDTO rec in ConvertExportDataList<AssetMasterDTO>(lstAssetsDTO).ToList())
                {
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.AssetName);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.Make);
                    csw.WriteField(rec.Model);
                    csw.WriteField(rec.Serial);
                    csw.WriteField(rec.ToolCategory);
                    csw.WriteField(rec.PurchaseDate.ToString());
                    csw.WriteField(rec.PurchasePrice);
                    csw.WriteField(rec.DepreciatedValue);
                    csw.WriteField(rec.SuggestedMaintenanceDate.ToString());
                    csw.WriteField(rec.RoomName);
                    csw.WriteField(rec.Created.ToString());
                    csw.WriteField(rec.Updated.ToString());
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);

                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string NotificationHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            NotificationDAL obj = new NotificationDAL(SessionHelper.EnterPriseDBName);
            List<NotificationDTO> lstNotificationDTO = new List<NotificationDTO>();
            lstNotificationDTO = obj.GetNotificationMasterChangeLog(ids);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("HistoryID");
            csw.WriteField("HistoryAction");
            csw.WriteField("ScheduleID");
            csw.WriteField("ScheduleName");
            csw.WriteField("TemplateName");
            csw.WriteField("ReportName");
            csw.WriteField("EmailAddress");
            csw.WriteField("EmailSubject");
            csw.WriteField("IsActive");
            csw.WriteField("NextRunDate");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("LastUpdatedBy");
            csw.WriteField("CreatedBy");

            csw.NextRecord();
            if (lstNotificationDTO != null && lstNotificationDTO.Count > 0)
            {
                foreach (NotificationDTO rec in ConvertExportDataList<NotificationDTO>(lstNotificationDTO).ToList())
                {
                    csw.WriteField(rec.HistoryID);
                    csw.WriteField(rec.Action);
                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ScheduleName);
                    csw.WriteField(rec.TemplateName);
                    csw.WriteField(rec.ReportName);
                    csw.WriteField(rec.EmailAddress);
                    csw.WriteField(rec.EmailSubject);
                    csw.WriteField(rec.IsActive == true ? "Yes" : "No");
                    csw.WriteField(rec.NextRunDate);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }

        public string NotificationListCSV(string Filepath, string ModuleName, string ids, string CurrentCulture)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            //string[] arrid = ids.Split(',');
            NotificationDAL obj = new NotificationDAL(SessionHelper.EnterPriseDBName);
            //List<NotificationDTO> lstNotificationDTO = new List<NotificationDTO>();
            var lstNotificationDTO = obj.GetNotificationByIDsNormal(ids, CurrentCulture);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("ID");
            csw.WriteField("ScheduleName");
            csw.WriteField("TemplateName");
            csw.WriteField("ReportName");
            csw.WriteField("EmailAddress");
            csw.WriteField("EmailSubject");
            csw.WriteField("IsActive");
            csw.WriteField("NextRunDate");
            csw.WriteField("CreatedOn");
            csw.WriteField("UpdatedOn");
            csw.WriteField("LastUpdatedBy");
            csw.WriteField("CreatedBy");

            csw.NextRecord();
            if (lstNotificationDTO != null && lstNotificationDTO.Count > 0)
            {
                foreach (NotificationDTO rec in ConvertExportDataList<NotificationDTO>(lstNotificationDTO).ToList())
                {
                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ScheduleName);
                    csw.WriteField(rec.TemplateName);
                    csw.WriteField(rec.ReportName);
                    csw.WriteField(rec.EmailAddress);
                    csw.WriteField(rec.EmailSubject);
                    csw.WriteField(rec.IsActive == true ? "Yes" : "No");
                    csw.WriteField(rec.NextRunDate);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }
        public string TechnicianListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            TechnicialMasterDAL obj = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            List<TechnicianMasterDTO> lstTechnicianMasterDTO = new List<TechnicianMasterDTO>();
            lstTechnicianMasterDTO = obj.GetTechnicianByIDsPlain(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();
            //IEnumerable<TechnicianMasterDTO> DataFromDB = obj.GetTechnicianByRoomIDNormal(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString);
            //{
            //    lstTechnicianMasterDTO = (from c in DataFromDB
            //                              where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(c.ID.ToString())
            //                              select new TechnicianMasterDTO
            //                              {
            //                                  ID = c.ID,
            //                                  Technician = c.Technician != null ? Convert.ToString(c.Technician) : string.Empty,
            //                                  TechnicianCode = c.TechnicianCode != null ? Convert.ToString(c.TechnicianCode).Replace(" ", "") : string.Empty,
            //                                  RoomName = c.RoomName,
            //                                  Created = c.Created,
            //                                  Updated = c.Updated,
            //                                  IsDeleted = c.IsDeleted,
            //                                  IsArchived = c.IsArchived,
            //                                  UpdatedByName = c.UpdatedByName,
            //                                  CreatedByName = c.CreatedByName,
            //                                  UDF1 = c.UDF1,
            //                                  UDF2 = c.UDF2,
            //                                  UDF3 = c.UDF3,
            //                                  UDF4 = c.UDF4,
            //                                  UDF5 = c.UDF5
            //                              }).ToList();
            //}

            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("TECHNICIAN");
            csw.WriteField("* TECHNICIANCODE");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.NextRecord();
            if (lstTechnicianMasterDTO != null && lstTechnicianMasterDTO.Count > 0)
            {


                foreach (TechnicianMasterDTO rec in ConvertExportDataList<TechnicianMasterDTO>(lstTechnicianMasterDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.Technician);
                    csw.WriteField(rec.TechnicianCode);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string UnitMasterListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            UnitMasterDAL obj = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            List<UnitMasterDTO> lstUnitMasterDTO = new List<UnitMasterDTO>();
            lstUnitMasterDTO = obj.GetUnitByIDsPlain(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();
            ////if (!string.IsNullOrEmpty(ids))
            //{
            //    lstUnitMasterDTO = (from u in DataFromDB
            //                        where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.ID.ToString())
            //                        select new UnitMasterDTO
            //                        {
            //                            Unit = u.Unit != null ? Convert.ToString(u.Unit) : string.Empty,
            //                            Created = u.Created,
            //                            CreatedBy = u.CreatedBy,
            //                            ID = u.ID,
            //                            LastUpdatedBy = u.LastUpdatedBy,
            //                            Room = u.Room,
            //                            Updated = u.Updated,
            //                            Description = u.Description != null ? Convert.ToString(u.Description) : string.Empty,
            //                            IsArchived = u.IsArchived,
            //                            IsDeleted = u.IsDeleted,
            //                            CreatedByName = u.CreatedByName,
            //                            UpdatedByName = u.UpdatedByName,
            //                            RoomName = u.RoomName,
            //                            UDF1 = u.UDF1,
            //                            UDF2 = u.UDF2,
            //                            UDF3 = u.UDF3,
            //                            UDF4 = u.UDF4,
            //                            UDF5 = u.UDF5
            //                        }).ToList();
            //}
            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* Unit");
            csw.WriteField("Description");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.NextRecord();
            if (lstUnitMasterDTO != null && lstUnitMasterDTO.Count > 0)
            {


                foreach (var rec in ConvertExportDataList<UnitMasterDTO>(lstUnitMasterDTO).ToList())
                {

                    csw.WriteField(rec.ID);

                    csw.WriteField(rec.Unit);
                    csw.WriteField(rec.Description ?? string.Empty);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string LocationMasterListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            LocationMasterDAL obj = new LocationMasterDAL(SessionHelper.EnterPriseDBName);

            List<LocationMasterDTO> lstLocationMasterDTO = new List<LocationMasterDTO>();

            lstLocationMasterDTO = obj.GetLocationByIDsPlain(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* LOCATION");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.NextRecord();
            if (lstLocationMasterDTO != null && lstLocationMasterDTO.Count > 0)
            {


                foreach (LocationMasterDTO rec in ConvertExportDataList<LocationMasterDTO>(lstLocationMasterDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.Location);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string ToolCategoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolCategoryMasterDAL obj = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<ToolCategoryMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            List<ToolCategoryMasterDTO> lstToolCategoryMasterDTO = new List<ToolCategoryMasterDTO>();
            lstToolCategoryMasterDTO = obj.GetToolCategoryByIDsNormal(ids, SessionHelper.RoomID, SessionHelper.CompanyID);
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstToolCategoryMasterDTO = (from c in DataFromDB
            //                                where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(c.ID.ToString())
            //                                select new ToolCategoryMasterDTO
            //                                {
            //                                    ID = c.ID,
            //                                    ToolCategory = c.ToolCategory != null ? Convert.ToString(c.ToolCategory) : string.Empty,
            //                                    RoomName = c.RoomName,
            //                                    Created = c.Created,
            //                                    CreatedBy = c.CreatedBy,
            //                                    Updated = c.Updated,
            //                                    LastUpdatedBy = c.LastUpdatedBy,
            //                                    UpdatedByName = c.UpdatedByName,
            //                                    Room = c.Room,
            //                                    CreatedByName = c.CreatedByName,
            //                                    IsArchived = c.IsArchived,
            //                                    IsDeleted = c.IsDeleted,
            //                                    UDF1 = c.UDF1,
            //                                    UDF2 = c.UDF2,
            //                                    UDF3 = c.UDF3,
            //                                    UDF4 = c.UDF4,
            //                                    UDF5 = c.UDF5
            //                                }).ToList();
            //}



            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("Id");
            csw.WriteField("TOOLCATEGORY");
            csw.WriteField("UDF2");
            csw.WriteField("Room");
            csw.WriteField("Created On");
            csw.WriteField("Updated On");
            csw.WriteField("Updated By");
            csw.WriteField("Created By");
            csw.WriteField("AddedFrom");
            csw.WriteField("EditFrom");
            csw.WriteField("ReceivedOnDate");
            csw.WriteField("ReceivedOnWebDate");
            csw.WriteField("UDF3");

            csw.NextRecord();
            if (lstToolCategoryMasterDTO != null && lstToolCategoryMasterDTO.Count > 0)
            {
                //csw.WriteHeader<CategoryMasterDTO>();


                foreach (ToolCategoryMasterDTO rec in ConvertExportDataList<ToolCategoryMasterDTO>(lstToolCategoryMasterDTO).ToList())
                {
                    //csw.WriteRecord<CategoryMasterDTO>(rec);
                    csw.WriteField(rec.ID);

                    csw.WriteField(rec.ToolCategory);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.Room);
                    csw.WriteField(rec.Created);
                    csw.WriteField(rec.Updated);
                    csw.WriteField(rec.UpdatedByName);
                    csw.WriteField(rec.CreatedByName);
                    csw.WriteField(rec.AddedFrom);
                    csw.WriteField(rec.EditedFrom);
                    csw.WriteField(rec.ReceivedOnDate);
                    csw.WriteField(rec.ReceivedOnDateWeb);
                    csw.WriteField(rec.UDF3);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string AssetCategoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            AssetCategoryMasterDAL obj = new AssetCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<AssetCategoryMasterDTO> DataFromDB = obj.GetAssetCategoryByRoom(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString);

            List<AssetCategoryMasterDTO> lstAssetCategoryMasterDTO = new List<AssetCategoryMasterDTO>();
            if (!string.IsNullOrEmpty(ids))
            {
                lstAssetCategoryMasterDTO = (from c in DataFromDB
                                             where arrid.Contains(c.ID.ToString())
                                             select new AssetCategoryMasterDTO
                                             {
                                                 ID = c.ID,
                                                 AssetCategory = c.AssetCategory != null ? Convert.ToString(c.AssetCategory) : string.Empty,
                                                 RoomName = c.RoomName,
                                                 Created = c.Created,
                                                 CreatedBy = c.CreatedBy,
                                                 Updated = c.Updated,
                                                 LastUpdatedBy = c.LastUpdatedBy,
                                                 UpdatedByName = c.UpdatedByName,
                                                 Room = c.Room,
                                                 CreatedByName = c.CreatedByName,
                                                 IsArchived = c.IsArchived,
                                                 IsDeleted = c.IsDeleted,
                                                 UDF1 = c.UDF1,
                                                 UDF2 = c.UDF2,
                                                 UDF3 = c.UDF3,
                                                 UDF4 = c.UDF4,
                                                 UDF5 = c.UDF5
                                             }).ToList();
            }



            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* AssetCATEGORY");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.NextRecord();
            if (lstAssetCategoryMasterDTO != null && lstAssetCategoryMasterDTO.Count > 0)
            {
                //csw.WriteHeader<CategoryMasterDTO>();


                foreach (AssetCategoryMasterDTO rec in ConvertExportDataList<AssetCategoryMasterDTO>(lstAssetCategoryMasterDTO).ToList())
                {
                    //csw.WriteRecord<CategoryMasterDTO>(rec);
                    csw.WriteField(rec.ID);

                    csw.WriteField(rec.AssetCategory);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string ManufacturerMasterListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ManufacturerMasterDAL obj = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            List<ManufacturerMasterDTO> DataFromDB = obj.GetManufacturerByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).OrderBy(SortNameString).ToList();
            List<ManufacturerMasterDTO> lstManufacturerMasterDTO = new List<ManufacturerMasterDTO>();
            //if (!string.IsNullOrEmpty(ids))
            {
                lstManufacturerMasterDTO = (from c in DataFromDB
                                            where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(c.ID.ToString())
                                            select new ManufacturerMasterDTO
                                            {
                                                ID = c.ID,
                                                Manufacturer = c.Manufacturer != null ? Convert.ToString(c.Manufacturer) : string.Empty,
                                                RoomName = c.RoomName,
                                                Created = c.Created,
                                                CreatedBy = c.CreatedBy,
                                                Updated = c.Updated,
                                                LastUpdatedBy = c.LastUpdatedBy,
                                                UpdatedByName = c.UpdatedByName,
                                                Room = c.Room,
                                                CreatedByName = c.CreatedByName,
                                                IsArchived = c.IsArchived,
                                                IsDeleted = c.IsDeleted,
                                                UDF1 = c.UDF1,
                                                UDF2 = c.UDF2,
                                                UDF3 = c.UDF3,
                                                UDF4 = c.UDF4,
                                                UDF5 = c.UDF5
                                            }).ToList();
            }

            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* MANUFACTURER");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.NextRecord();
            if (lstManufacturerMasterDTO != null && lstManufacturerMasterDTO.Count > 0)
            {


                foreach (ManufacturerMasterDTO rec in ConvertExportDataList<ManufacturerMasterDTO>(lstManufacturerMasterDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.Manufacturer);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string MeasurementTermListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            MeasurementTermDAL obj = new MeasurementTermDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<MeasurementTermMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            List<MeasurementTermMasterDTO> lstMeasurementTermMasterDTO = new List<MeasurementTermMasterDTO>();
            //if (!string.IsNullOrEmpty(ids))
            {
                lstMeasurementTermMasterDTO = (from c in DataFromDB
                                               where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(c.ID.ToString())
                                               select new MeasurementTermMasterDTO
                                               {
                                                   ID = c.ID,
                                                   MeasurementTerm = c.MeasurementTerm != null ? Convert.ToString(c.MeasurementTerm) : string.Empty,
                                                   RoomName = c.RoomName,
                                                   Created = c.Created,
                                                   Updated = c.Updated,
                                                   UpdatedByName = c.UpdatedByName,
                                                   CreatedByName = c.CreatedByName,
                                                   IsArchived = c.IsArchived,
                                                   IsDeleted = c.IsDeleted,
                                                   UDF1 = c.UDF1,
                                                   UDF2 = c.UDF2,
                                                   UDF3 = c.UDF3,
                                                   UDF4 = c.UDF4,
                                                   UDF5 = c.UDF5
                                               }).ToList();
            }



            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* MEASUREMENTTERM");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.NextRecord();
            if (lstMeasurementTermMasterDTO != null && lstMeasurementTermMasterDTO.Count > 0)
            {

                foreach (MeasurementTermMasterDTO rec in ConvertExportDataList<MeasurementTermMasterDTO>(lstMeasurementTermMasterDTO).ToList())
                {

                    csw.WriteField(rec.ID);

                    csw.WriteField(rec.MeasurementTerm);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string ToolListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
            lstToolMasterDTO = obj.GetToolByIDsFull(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();


            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ToolName");
            csw.WriteField("* Serial");
            csw.WriteField("Description");
            csw.WriteField("Cost");
            csw.WriteField("* Quantity");
            csw.WriteField("ToolCategory");

            csw.WriteField("Location");
            csw.WriteField("CheckedOutQTY");
            csw.WriteField("CheckedOutMQTY");

            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("IsGroupOfItems");
            csw.WriteField("Technician");
            csw.WriteField("CheckOutQuantity");
            csw.WriteField("CheckInQuantity");

            csw.WriteField("CheckoutUDF1");
            csw.WriteField("CheckoutUDF2");
            csw.WriteField("CheckoutUDF3");
            csw.WriteField("CheckoutUDF4");
            csw.WriteField("CheckoutUDF5");

            csw.WriteField("ImagePath");
            csw.WriteField("ToolImageExternalURL");
            csw.WriteField("ToolTrackingType");

            csw.NextRecord();
            if (lstToolMasterDTO != null && lstToolMasterDTO.Count > 0)
            {



                foreach (ToolMasterDTO rec in ConvertExportDataList<ToolMasterDTO>(lstToolMasterDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ToolName ?? string.Empty);
                    csw.WriteField(rec.Serial);
                    csw.WriteField(rec.Description ?? string.Empty);
                    csw.WriteField(rec.Cost ?? 0);
                    csw.WriteField(rec.Quantity);
                    csw.WriteField(rec.ToolCategory ?? string.Empty);

                    csw.WriteField(rec.Location ?? string.Empty);
                    csw.WriteField(rec.CheckedOutQTY ?? 0);
                    csw.WriteField(rec.CheckedOutMQTY ?? 0);

                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    if (rec.IsGroupOfItems == 1)
                        csw.WriteField("Yes");
                    else
                        csw.WriteField("No");
                    csw.WriteField(rec.Technician);
                    csw.WriteField(rec.CheckedOutQuantity ?? 0);
                    csw.WriteField(rec.CheckedInQuantity ?? 0);

                    csw.WriteField("");
                    csw.WriteField("");
                    csw.WriteField("");
                    csw.WriteField("");
                    csw.WriteField("");

                    csw.WriteField(rec.ImagePath);
                    csw.WriteField(rec.ToolImageExternalURL);
                    csw.WriteField(rec.ToolTypeTracking);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string ToolListCSVNew(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
            lstToolMasterDTO = obj.GetToolByIDsFull(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();
            //if (!string.IsNullOrEmpty(ids))


            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ToolName");
            csw.WriteField("* Serial");
            csw.WriteField("Description");
            csw.WriteField("Cost");
            csw.WriteField("* Quantity");
            csw.WriteField("ToolCategory");

            csw.WriteField("Location");
            csw.WriteField("CheckedOutQTY");
            csw.WriteField("CheckedOutMQTY");

            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("IsGroupOfItems");
            csw.WriteField("Technician");
            csw.WriteField("CheckOutQuantity");
            csw.WriteField("CheckInQuantity");

            csw.WriteField("CheckoutUDF1");
            csw.WriteField("CheckoutUDF2");
            csw.WriteField("CheckoutUDF3");
            csw.WriteField("CheckoutUDF4");
            csw.WriteField("CheckoutUDF5");

            csw.WriteField("ImagePath");
            csw.WriteField("ToolImageExternalURL");
            csw.WriteField("ToolTrackingType");

            csw.NextRecord();
            if (lstToolMasterDTO != null && lstToolMasterDTO.Count > 0)
            {



                foreach (ToolMasterDTO rec in ConvertExportDataList<ToolMasterDTO>(lstToolMasterDTO).ToList())
                {
                    List<ToolLocationDetailsDTO> lstToolLocationDetail = new List<ToolLocationDetailsDTO>();
                    ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    lstToolLocationDetail = objToolLocationDetailsDAL.GetToolAllLocationsByToolGUID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, rec.GUID).ToList();
                    foreach (ToolLocationDetailsDTO l in lstToolLocationDetail)
                    {

                        ToolAssetCICOTransactionDAL objPullDetails = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
                        List<ToolQuantityLotSerialDTO> objToolQuantityLotSerialDTO = objPullDetails.GetToolLocationsWithLotSerials(rec.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, l.LocationGUID);

                        foreach (ToolQuantityLotSerialDTO ToolQty in objToolQuantityLotSerialDTO)
                        {
                            csw.WriteField(rec.ID);
                            csw.WriteField(rec.ToolName);
                            if (rec.SerialNumberTracking == true)
                            {
                                csw.WriteField(ToolQty.SerialNumber);
                            }
                            else
                            {
                                csw.WriteField(rec.Serial);
                            }
                            csw.WriteField(rec.Description ?? string.Empty);
                            csw.WriteField(rec.Cost ?? 0);
                            if (rec.SerialNumberTracking == false)
                            {
                                csw.WriteField(rec.Quantity);
                            }
                            else
                            {
                                csw.WriteField(ToolQty.Quantity);
                            }
                            csw.WriteField(rec.ToolCategory ?? string.Empty);

                            csw.WriteField(rec.Location ?? string.Empty);
                            if (rec.SerialNumberTracking == true)
                            {
                                if (ToolQty.Quantity == 0)
                                {
                                    csw.WriteField("1");
                                }
                                else
                                {
                                    csw.WriteField("0");
                                }
                            }
                            else
                            {
                                csw.WriteField(rec.Quantity);
                            }
                            csw.WriteField(rec.CheckedOutMQTY);

                            csw.WriteField(rec.UDF1);
                            csw.WriteField(rec.UDF2);
                            csw.WriteField(rec.UDF3);
                            csw.WriteField(rec.UDF4);
                            csw.WriteField(rec.UDF5);
                            if (rec.IsGroupOfItems == 1)
                                csw.WriteField("Yes");
                            else
                                csw.WriteField("No");
                            csw.WriteField(rec.Technician);
                            csw.WriteField(rec.CheckedOutQuantity);
                            csw.WriteField(rec.CheckedInQuantity);

                            csw.WriteField("");
                            csw.WriteField("");
                            csw.WriteField("");
                            csw.WriteField("");
                            csw.WriteField("");

                            csw.WriteField(rec.ImagePath);
                            csw.WriteField(rec.ToolImageExternalURL);
                            csw.WriteField(rec.ToolTypeTracking);
                            csw.NextRecord();
                        }
                    }

                }
            }
            write.Close();

            return filename;

        }


        public string ToolHistoryListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
            lstToolMasterDTO = obj.GetToolByIDsFull(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();
            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ToolName");
            csw.WriteField("* Serial");
            csw.WriteField("Description");
            csw.WriteField("Cost");
            csw.WriteField("* Quantity");
            csw.WriteField("ToolCategory");

            csw.WriteField("Location");
            csw.WriteField("CheckedOutQTY");
            csw.WriteField("CheckedOutMQTY");

            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("IsGroupOfItems");
            csw.WriteField("Technician");
            csw.WriteField("CheckOutQuantity");
            csw.WriteField("CheckInQuantity");

            csw.WriteField("CheckoutUDF1");
            csw.WriteField("CheckoutUDF2");
            csw.WriteField("CheckoutUDF3");
            csw.WriteField("CheckoutUDF4");
            csw.WriteField("CheckoutUDF5");

            csw.WriteField("ImagePath");
            csw.WriteField("ToolImageExternalURL");
            csw.WriteField("ToolTypeTracking");

            csw.NextRecord();
            if (lstToolMasterDTO != null && lstToolMasterDTO.Count > 0)
            {

                foreach (ToolMasterDTO rec in ConvertExportDataList<ToolMasterDTO>(lstToolMasterDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ToolName ?? string.Empty);
                    csw.WriteField(rec.Serial);
                    csw.WriteField(rec.Description ?? string.Empty);
                    csw.WriteField(rec.Cost ?? 0);
                    csw.WriteField(rec.Quantity);
                    csw.WriteField(rec.ToolCategory ?? string.Empty);

                    csw.WriteField(rec.Location ?? string.Empty);
                    csw.WriteField(rec.CheckedOutQTY ?? 0);
                    csw.WriteField(rec.CheckedOutMQTY ?? 0);

                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    if (rec.IsGroupOfItems == 1)
                        csw.WriteField("Yes");
                    else
                        csw.WriteField("No");
                    csw.WriteField(rec.Technician);
                    csw.WriteField(rec.CheckedOutQuantity ?? 0);
                    csw.WriteField(rec.CheckedInQuantity ?? 0);

                    csw.WriteField("");
                    csw.WriteField("");
                    csw.WriteField("");
                    csw.WriteField("");
                    csw.WriteField("");

                    csw.WriteField(rec.ImagePath);
                    csw.WriteField(rec.ToolImageExternalURL);
                    csw.WriteField(rec.ToolTypeTracking);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string ToolMasterListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
            lstToolMasterDTO = obj.GetToolByIDsNormal(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();


            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ToolName");
            csw.WriteField("* Serial");
            csw.WriteField("Description");
            csw.WriteField("Cost");
            csw.WriteField("* Quantity");
            csw.WriteField("ToolCategory");

            csw.WriteField("Location");
            csw.WriteField("CheckedOutQTY");
            csw.WriteField("CheckedOutMQTY");

            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("IsGroupOfItems");
            csw.WriteField("Technician");


            csw.WriteField("CheckoutUDF1");
            csw.WriteField("CheckoutUDF2");
            csw.WriteField("CheckoutUDF3");
            csw.WriteField("CheckoutUDF4");
            csw.WriteField("CheckoutUDF5");

            csw.WriteField("ImagePath");
            csw.WriteField("ToolImageExternalURL");
            if (SessionHelper.AllowToolOrdering)
            {
                csw.WriteField("ToolTypeTracking");
                csw.WriteField("SerialNumberTracking");
            }
            csw.NextRecord();
            if (lstToolMasterDTO != null && lstToolMasterDTO.Count > 0)
            {

                foreach (ToolMasterDTO rec in ConvertExportDataList<ToolMasterDTO>(lstToolMasterDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ToolName);
                    csw.WriteField(rec.Serial);
                    csw.WriteField(rec.Description ?? string.Empty);
                    csw.WriteField(rec.Cost ?? 0);
                    csw.WriteField(rec.Quantity);
                    csw.WriteField(rec.ToolCategory ?? string.Empty);

                    csw.WriteField(rec.Location ?? string.Empty);
                    csw.WriteField(rec.CheckedOutQTY ?? 0);
                    csw.WriteField(rec.CheckedOutMQTY ?? 0);

                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    if (rec.IsGroupOfItems == 1)
                        csw.WriteField("Yes");
                    else
                        csw.WriteField("No");

                    csw.WriteField("");



                    csw.WriteField("");
                    csw.WriteField("");
                    csw.WriteField("");
                    csw.WriteField("");
                    csw.WriteField("");

                    csw.WriteField(rec.ImagePath);
                    csw.WriteField(rec.ToolImageExternalURL);
                    if (SessionHelper.AllowToolOrdering)
                    {
                        csw.WriteField(rec.ToolTypeTracking);
                        csw.WriteField(rec.SerialNumberTracking);
                    }
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string ToolMasterListCSVNew(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
            lstToolMasterDTO = obj.GetToolByIDsNormal(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();


            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ToolName");
            csw.WriteField("* Serial");
            csw.WriteField("Description");
            csw.WriteField("Cost");
            csw.WriteField("* Quantity");
            csw.WriteField("ToolCategory");

            csw.WriteField("Location");
            csw.WriteField("CheckedOutQTY");
            csw.WriteField("CheckedOutMQTY");

            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("IsGroupOfItems");
            csw.WriteField("Technician");



            csw.WriteField("CheckoutUDF1");
            csw.WriteField("CheckoutUDF2");
            csw.WriteField("CheckoutUDF3");
            csw.WriteField("CheckoutUDF4");
            csw.WriteField("CheckoutUDF5");

            csw.WriteField("ImagePath");
            csw.WriteField("ToolImageExternalURL");
            csw.WriteField("ToolTypeTracking");
            csw.WriteField("SerialNumberTracking");

            csw.WriteField("NoOfPastMntsToConsider");
            csw.WriteField("MaintenanceDueNoticeDays");

            csw.NextRecord();
            if (lstToolMasterDTO != null && lstToolMasterDTO.Count > 0)
            {

                foreach (ToolMasterDTO rec in ConvertExportDataList<ToolMasterDTO>(lstToolMasterDTO).ToList())
                {

                    {
                        List<ToolLocationDetailsDTO> lstToolLocationDetail = new List<ToolLocationDetailsDTO>();
                        ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                        lstToolLocationDetail = objToolLocationDetailsDAL.GetToolAllLocationsByToolGUID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, rec.GUID).ToList();
                        foreach (ToolLocationDetailsDTO l in lstToolLocationDetail)
                        {

                            ToolAssetCICOTransactionDAL objPullDetails = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
                            List<ToolQuantityLotSerialDTO> objToolQuantityLotSerialDTO = objPullDetails.GetToolLocationsWithLotSerials(rec.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, l.LocationGUID);
                            if (objToolQuantityLotSerialDTO != null && objToolQuantityLotSerialDTO.Count() > 0)
                            {
                                foreach (ToolQuantityLotSerialDTO ToolQty in objToolQuantityLotSerialDTO)
                                {
                                    csw.WriteField(rec.ID);
                                    csw.WriteField(rec.ToolName);
                                    if (rec.SerialNumberTracking == true)
                                    {
                                        csw.WriteField(ToolQty.SerialNumber);
                                    }
                                    else
                                    {
                                        csw.WriteField(rec.Serial);
                                    }
                                    csw.WriteField(rec.Description ?? string.Empty);
                                    csw.WriteField(rec.Cost ?? 0);
                                    csw.WriteField(ToolQty.Quantity);
                                    csw.WriteField(rec.ToolCategory ?? string.Empty);


                                    csw.WriteField(l.ToolLocationName ?? string.Empty);
                                    csw.WriteField(rec.CheckedOutQTY ?? 0);
                                    csw.WriteField(rec.CheckedOutMQTY ?? 0);

                                    csw.WriteField(rec.UDF1);
                                    csw.WriteField(rec.UDF2);
                                    csw.WriteField(rec.UDF3);
                                    csw.WriteField(rec.UDF4);
                                    csw.WriteField(rec.UDF5);
                                    if (rec.IsGroupOfItems == 1)
                                        csw.WriteField("Yes");
                                    else
                                        csw.WriteField("No");

                                    csw.WriteField("");

                                    csw.WriteField("");
                                    csw.WriteField("");
                                    csw.WriteField("");
                                    csw.WriteField("");
                                    csw.WriteField("");

                                    csw.WriteField(rec.ImagePath);
                                    csw.WriteField(rec.ToolImageExternalURL);
                                    csw.WriteField(rec.ToolTypeTracking);
                                    if (rec.SerialNumberTracking == true)
                                        csw.WriteField("Yes");
                                    else
                                        csw.WriteField("No");


                                    csw.WriteField(rec.NoOfPastMntsToConsider);
                                    csw.WriteField(rec.MaintenanceDueNoticeDays);
                                    csw.NextRecord();
                                }
                            }
                            else
                            {
                                csw.WriteField(rec.ID);
                                csw.WriteField(rec.ToolName);
                                if (rec.SerialNumberTracking == true)
                                {
                                    csw.WriteField("");
                                }
                                else
                                {
                                    csw.WriteField(rec.Serial);
                                }
                                csw.WriteField(rec.Description ?? string.Empty);
                                csw.WriteField(rec.Cost ?? 0);
                                csw.WriteField("0");
                                csw.WriteField(rec.ToolCategory ?? string.Empty);


                                csw.WriteField(l.ToolLocationName ?? string.Empty);
                                csw.WriteField(rec.CheckedOutQTY ?? 0);
                                csw.WriteField(rec.CheckedOutMQTY ?? 0);

                                csw.WriteField(rec.UDF1);
                                csw.WriteField(rec.UDF2);
                                csw.WriteField(rec.UDF3);
                                csw.WriteField(rec.UDF4);
                                csw.WriteField(rec.UDF5);
                                if (rec.IsGroupOfItems == 1)
                                    csw.WriteField("Yes");
                                else
                                    csw.WriteField("No");

                                csw.WriteField("");

                                csw.WriteField("");
                                csw.WriteField("");
                                csw.WriteField("");
                                csw.WriteField("");
                                csw.WriteField("");

                                csw.WriteField(rec.ImagePath);
                                csw.WriteField(rec.ToolImageExternalURL);
                                csw.WriteField(rec.ToolTypeTracking);
                                if (rec.SerialNumberTracking == true)
                                    csw.WriteField("Yes");
                                else
                                    csw.WriteField("No");
                                csw.WriteField(rec.NoOfPastMntsToConsider);
                                csw.WriteField(rec.MaintenanceDueNoticeDays);
                                csw.NextRecord();
                            }


                        }
                    }


                }
            }
            write.Close();

            return filename;

        }

        //ToolCheckoutStatusListCSV
        public string ToolCheckoutStatusListCSV(string Filepath, string ModuleName, string ids, string SortNameString, out bool isRecordAvail)
        {
            isRecordAvail = false;
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
            lstToolMasterDTO = obj.GetToolCheckoutStatusExportData(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();


            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ToolName");
            csw.WriteField("* Serial");
            csw.WriteField("Description");
            csw.WriteField("Cost");
            csw.WriteField("* Quantity");
            csw.WriteField("ToolCategory");

            csw.WriteField("Location");
            csw.WriteField("CheckedOutQTY");
            csw.WriteField("CheckedOutMQTY");

            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("IsGroupOfItems");
            csw.WriteField("Technician");



            csw.WriteField("CheckoutUDF1");
            csw.WriteField("CheckoutUDF2");
            csw.WriteField("CheckoutUDF3");
            csw.WriteField("CheckoutUDF4");
            csw.WriteField("CheckoutUDF5");

            csw.WriteField("ImagePath");
            csw.WriteField("ToolImageExternalURL");


            csw.NextRecord();
            if (lstToolMasterDTO != null && lstToolMasterDTO.Count > 0)
            {
                isRecordAvail = true;
                foreach (ToolMasterDTO rec in ConvertExportDataList<ToolMasterDTO>(lstToolMasterDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ToolName);
                    csw.WriteField(rec.Serial);
                    csw.WriteField(rec.Description ?? string.Empty);
                    csw.WriteField(rec.Cost ?? 0);
                    csw.WriteField(rec.Quantity);
                    csw.WriteField(rec.ToolCategory ?? string.Empty);

                    csw.WriteField(rec.Location ?? string.Empty);
                    csw.WriteField(rec.CheckedOutQTY ?? 0);
                    csw.WriteField(rec.CheckedOutMQTY ?? 0);

                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    if (rec.IsGroupOfItems == 1)
                        csw.WriteField("Yes");
                    else
                        csw.WriteField("No");

                    csw.WriteField("");

                    csw.WriteField("");
                    csw.WriteField("");
                    csw.WriteField("");
                    csw.WriteField("");
                    csw.WriteField("");

                    csw.WriteField(rec.ImagePath);
                    csw.WriteField(rec.ToolImageExternalURL);

                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }


        public string ToolCheckoutStatusListCSVNew(string Filepath, string ModuleName, string ids, string SortNameString, out bool isRecordAvail)
        {
            isRecordAvail = false;
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
            lstToolMasterDTO = obj.GetToolCheckoutStatusExportData(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();



            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ToolName");
            csw.WriteField("* Serial");
            csw.WriteField("Description");
            csw.WriteField("Cost");
            csw.WriteField("* Quantity");
            csw.WriteField("ToolCategory");

            csw.WriteField("Location");
            csw.WriteField("CheckedOutQTY");
            csw.WriteField("CheckedOutMQTY");

            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("IsGroupOfItems");
            csw.WriteField("Technician");
            //csw.WriteField("CheckOutQuantity");
            //csw.WriteField("CheckInQuantity");

            csw.WriteField("CheckoutUDF1");
            csw.WriteField("CheckoutUDF2");
            csw.WriteField("CheckoutUDF3");
            csw.WriteField("CheckoutUDF4");
            csw.WriteField("CheckoutUDF5");

            csw.WriteField("ImagePath");
            csw.WriteField("ToolImageExternalURL");
            csw.WriteField("SerialNumberTracking");


            csw.NextRecord();
            if (lstToolMasterDTO != null && lstToolMasterDTO.Count > 0)
            {
                isRecordAvail = true;
                foreach (ToolMasterDTO rec in ConvertExportDataList<ToolMasterDTO>(lstToolMasterDTO).ToList())
                {

                    if (rec.SerialNumberTracking == true)
                    {
                        LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                        LocationMasterDTO objLocationMasterDTO = objLocationMasterDAL.GetLocationBySerialPlain(rec.Serial, rec.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                        {
                            csw.WriteField(rec.ID);
                            csw.WriteField(rec.ToolName);
                            csw.WriteField(rec.Serial);
                            csw.WriteField(rec.Description);
                            csw.WriteField(rec.Cost);
                            csw.WriteField(rec.Quantity);
                            csw.WriteField(rec.ToolCategory);

                            //csw.WriteField(rec.Location);
                            csw.WriteField(objLocationMasterDTO.Location);
                            csw.WriteField(rec.CheckedOutQTY);
                            csw.WriteField(rec.CheckedOutMQTY);

                            csw.WriteField(rec.UDF1);
                            csw.WriteField(rec.UDF2);
                            csw.WriteField(rec.UDF3);
                            csw.WriteField(rec.UDF4);
                            csw.WriteField(rec.UDF5);
                            if (rec.IsGroupOfItems == 1)
                                csw.WriteField("Yes");
                            else
                                csw.WriteField("No");

                            csw.WriteField("");//(rec.Technician);
                                               //csw.WriteField(rec.CheckedOutQuantity);
                                               //csw.WriteField(rec.CheckedInQuantity);

                            csw.WriteField("");
                            csw.WriteField("");
                            csw.WriteField("");
                            csw.WriteField("");
                            csw.WriteField("");

                            csw.WriteField(rec.ImagePath);
                            csw.WriteField(rec.ToolImageExternalURL);
                            if (rec.SerialNumberTracking == true)
                                csw.WriteField("Yes");
                            else
                                csw.WriteField("No");


                            csw.NextRecord();
                        }
                    }
                    else
                    {
                        LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                        LocationMasterDTO objLocationMasterDTO = objLocationMasterDAL.GetLocationUsingGeneralCO(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, rec.GUID, rec.ToolCheckoutGUID ?? Guid.Empty);

                        {
                            csw.WriteField(rec.ID);
                            csw.WriteField(rec.ToolName);
                            csw.WriteField(rec.Serial);
                            csw.WriteField(rec.Description);
                            csw.WriteField(rec.Cost);
                            csw.WriteField(rec.Quantity);
                            csw.WriteField(rec.ToolCategory);

                            //csw.WriteField(rec.Location);
                            csw.WriteField(objLocationMasterDTO.Location);
                            csw.WriteField(rec.CheckedOutQTY);
                            csw.WriteField(rec.CheckedOutMQTY);

                            csw.WriteField(rec.UDF1);
                            csw.WriteField(rec.UDF2);
                            csw.WriteField(rec.UDF3);
                            csw.WriteField(rec.UDF4);
                            csw.WriteField(rec.UDF5);
                            if (rec.IsGroupOfItems == 1)
                                csw.WriteField("Yes");
                            else
                                csw.WriteField("No");

                            csw.WriteField("");//(rec.Technician);
                                               //csw.WriteField(rec.CheckedOutQuantity);
                                               //csw.WriteField(rec.CheckedInQuantity);

                            csw.WriteField("");
                            csw.WriteField("");
                            csw.WriteField("");
                            csw.WriteField("");
                            csw.WriteField("");

                            csw.WriteField(rec.ImagePath);
                            csw.WriteField(rec.ToolImageExternalURL);
                            if (rec.SerialNumberTracking == true)
                                csw.WriteField("Yes");
                            else
                                csw.WriteField("No");

                            csw.NextRecord();
                        }
                    }

                }
            }
            write.Close();

            return filename;

        }


        public string AssetMasterListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            AssetMasterDAL obj = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID);
            IEnumerable<AssetMasterDTO> DataFromDB = obj.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            List<AssetMasterDTO> lstAssetMasterDTO = new List<AssetMasterDTO>();
            // if (!string.IsNullOrEmpty(ids))
            {
                lstAssetMasterDTO = (from u in DataFromDB
                                     where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.ID.ToString())
                                     select new AssetMasterDTO
                                     {
                                         ID = u.ID,
                                         AssetName = u.AssetName != null ? Convert.ToString(u.AssetName) : string.Empty,
                                         Description = u.Description != null ? Convert.ToString(u.Description) : string.Empty,
                                         Make = u.Make != null ? Convert.ToString(u.Make) : string.Empty,
                                         Model = u.Model != null ? Convert.ToString(u.Model) : string.Empty,
                                         Serial = u.Serial != null ? Convert.ToString(u.Serial) : string.Empty,
                                         ToolCategoryID = u.ToolCategoryID,
                                         ToolCategory = u.ToolCategory != null ? Convert.ToString(u.ToolCategory) : string.Empty,
                                         PurchaseDate = u.PurchaseDate,
                                         PurchasePrice = u.PurchasePrice,
                                         DepreciatedValue = u.DepreciatedValue,
                                         SuggestedMaintenanceDate = u.SuggestedMaintenanceDate,
                                         Created = u.Created,
                                         CreatedBy = u.CreatedBy,
                                         Updated = u.Updated,
                                         LastUpdatedBy = u.LastUpdatedBy,
                                         Room = u.Room,
                                         IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                         IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                         GUID = u.GUID,
                                         CompanyID = u.CompanyID,
                                         UDF1 = u.UDF1,
                                         UDF2 = u.UDF2,
                                         UDF3 = u.UDF3,
                                         UDF4 = u.UDF4,
                                         UDF5 = u.UDF5,
                                         UDF6 = u.UDF6,
                                         UDF7 = u.UDF7,
                                         UDF8 = u.UDF8,
                                         UDF9 = u.UDF9,
                                         UDF10 = u.UDF10,
                                         CreatedByName = u.CreatedByName,
                                         UpdatedByName = u.UpdatedByName,
                                         RoomName = u.RoomName,
                                         AssetCategory = u.AssetCategory != null ? Convert.ToString(u.AssetCategory) : string.Empty,
                                         AssetCategoryID = u.AssetCategoryID,
                                         AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Assets"),
                                         ImageType = u.ImageType,
                                         ImagePath = u.ImagePath,
                                         AssetImageExternalURL = u.AssetImageExternalURL
                                     }).ToList();
            }



            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* AssetName");
            csw.WriteField("Description");
            csw.WriteField("Make");
            csw.WriteField("Model");
            csw.WriteField("Serial");
            csw.WriteField("AssetCategory");
            csw.WriteField("PurchaseDate");
            csw.WriteField("PurchasePrice");
            csw.WriteField("DepreciatedValue");
            csw.WriteField("SuggestedMaintenanceDate");

            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");


            csw.WriteField("UDF6");
            csw.WriteField("UDF7");
            csw.WriteField("UDF8");
            csw.WriteField("UDF9");
            csw.WriteField("UDF10");

            csw.WriteField("ImagePath");
            csw.WriteField("AssetImageExternalURL");

            csw.NextRecord();
            if (lstAssetMasterDTO != null && lstAssetMasterDTO.Count > 0)
            {


                foreach (AssetMasterDTO rec in ConvertExportDataList<AssetMasterDTO>(lstAssetMasterDTO).ToList())
                {
                    csw.WriteField(rec.ID);

                    csw.WriteField(rec.AssetName);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.Make);
                    csw.WriteField(rec.Model);
                    csw.WriteField(rec.Serial);
                    csw.WriteField(rec.AssetCategory);
                    csw.WriteField(rec.PurchaseDate);
                    csw.WriteField(rec.PurchasePrice);
                    csw.WriteField(rec.DepreciatedValue);
                    csw.WriteField(rec.SuggestedMaintenanceDate);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.WriteField(rec.UDF6);
                    csw.WriteField(rec.UDF7);
                    csw.WriteField(rec.UDF8);
                    csw.WriteField(rec.UDF9);
                    csw.WriteField(rec.UDF10);

                    csw.WriteField(rec.ImagePath);
                    csw.WriteField(rec.AssetImageExternalURL);

                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string ItemMasterListCSV(string Filepath, string ModuleName, string ids, string SortNameString, string CallFromPage = "")
        {
            //string filename = SessionHelper.CompanyID + "_" + SessionHelper.RoomID + "_" + DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy HH:mm:ss").Replace("/", "").Replace(":", "") + ModuleName + ".csv";
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> lstIds = arrid.Select(t => Guid.Parse(t)).ToList();
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<ItemMasterDTO> DataFromDB = null;
            SortNameString = (SortNameString ?? string.Empty).Replace("OrderCost", "ItemNumber");
            if (!string.IsNullOrEmpty(CallFromPage) && CallFromPage.Trim().ToLower() == "import")
            {
                if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                {
                    DataFromDB = obj.GetItemsByArrayForImport(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds).OrderBy(SortNameString); //).Where(i => i.IsDeleted == false && i.Room == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && SessionHelper.UserSupplierIds.Contains(i.SupplierID.GetValueOrDefault(0))).OrderBy(SortNameString);
                }
                else
                {
                    DataFromDB = obj.GetItemsByArrayForImport(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID, new List<long>()).OrderBy(SortNameString); //.Where(i => i.IsDeleted == false && i.Room == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID).OrderBy(SortNameString);
                }
            }
            else
            {
                DataFromDB = obj.GetItemsByArray(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString);//.Where(i => i.Room == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID).OrderBy(SortNameString);
            }
            List<ItemMasterDTO> lstItemMasterDTO = new List<ItemMasterDTO>();

            //if (!string.IsNullOrEmpty(ids))
            {
                lstItemMasterDTO = (from u in DataFromDB
                                    where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.GUID.ToString())
                                    select new ItemMasterDTO
                                    {
                                        ID = u.ID,
                                        ItemNumber = u.ItemNumber != null ? Convert.ToString(u.ItemNumber) : string.Empty,
                                        ManufacturerID = u.ManufacturerID,
                                        ManufacturerNumber = u.ManufacturerNumber != null ? Convert.ToString(u.ManufacturerNumber) : string.Empty,
                                        ManufacturerName = u.ManufacturerName != null ? Convert.ToString(u.ManufacturerName) : string.Empty,
                                        SupplierID = u.SupplierID,
                                        SupplierPartNo = u.SupplierPartNo != null ? Convert.ToString(u.SupplierPartNo) : string.Empty,
                                        SupplierName = u.SupplierName != null ? Convert.ToString(u.SupplierName) : string.Empty,
                                        UPC = u.UPC,
                                        UNSPSC = u.UNSPSC,
                                        Description = u.Description != null ? Convert.ToString(u.Description) : string.Empty,
                                        LongDescription = u.LongDescription != null ? Convert.ToString(u.LongDescription) : string.Empty,
                                        CategoryID = u.CategoryID,
                                        GLAccountID = u.GLAccountID,
                                        UOMID = u.UOMID,

                                        PricePerTerm = u.PricePerTerm,
                                        CostUOMID = u.CostUOMID,
                                        CostUOMName = u.CostUOMName != null ? Convert.ToString(u.CostUOMName) : string.Empty,
                                        DefaultReorderQuantity = u.DefaultReorderQuantity ?? 0,
                                        DefaultPullQuantity = u.DefaultPullQuantity ?? 0,
                                        Cost = u.Cost ?? 0,
                                        Markup = u.Markup ?? 0,
                                        SellPrice = u.SellPrice ?? 0,
                                        ExtendedCost = u.ExtendedCost ?? 0,
                                        AverageCost = u.AverageCost ?? 0,
                                        PerItemCost = u.PerItemCost ?? 0,
                                        LeadTimeInDays = u.LeadTimeInDays ?? 0,
                                        Link1 = u.Link1 != null ? Convert.ToString(u.Link1) : string.Empty,
                                        Link2 = u.Link2 != null ? Convert.ToString(u.Link2) : string.Empty,
                                        Trend = u.Trend,
                                        Taxable = u.Taxable,
                                        Consignment = u.Consignment,
                                        StagedQuantity = u.StagedQuantity,
                                        InTransitquantity = u.InTransitquantity ?? 0,
                                        OnOrderQuantity = u.OnOrderQuantity ?? 0,
                                        OnReturnQuantity = u.OnReturnQuantity ?? 0,
                                        OnTransferQuantity = u.OnTransferQuantity ?? 0,
                                        SuggestedOrderQuantity = u.SuggestedOrderQuantity ?? 0,
                                        SuggestedTransferQuantity = u.SuggestedTransferQuantity ?? 0,
                                        RequisitionedQuantity = u.RequisitionedQuantity,
                                        PackingQuantity = u.PackingQuantity ?? 0,
                                        AverageUsage = u.AverageUsage ?? 0,
                                        Turns = u.Turns ?? 0,
                                        OnHandQuantity = u.OnHandQuantity ?? 0,
                                        CriticalQuantity = u.CriticalQuantity,
                                        MinimumQuantity = u.MinimumQuantity,
                                        MaximumQuantity = u.MaximumQuantity,
                                        WeightPerPiece = u.WeightPerPiece,
                                        ItemUniqueNumber = u.ItemUniqueNumber != null ? Convert.ToString(u.ItemUniqueNumber) : string.Empty,
                                        IsPurchase = u.IsPurchase,
                                        IsTransfer = u.IsTransfer,
                                        DefaultLocation = u.DefaultLocation,
                                        DefaultLocationName = u.DefaultLocationName != null ? Convert.ToString(u.DefaultLocationName).Replace("[|EmptyStagingBin|]", "") : string.Empty,
                                        InventoryClassification = u.InventoryClassification,
                                        InventoryClassificationName = u.InventoryClassificationName != null ? Convert.ToString(u.InventoryClassificationName) : string.Empty,
                                        SerialNumberTracking = u.SerialNumberTracking,
                                        LotNumberTracking = u.LotNumberTracking,
                                        DateCodeTracking = u.DateCodeTracking,
                                        ItemType = u.ItemType,
                                        ImagePath = u.ImagePath,
                                        UDF1 = u.UDF1,
                                        UDF2 = u.UDF2,
                                        UDF3 = u.UDF3,
                                        UDF4 = u.UDF4,
                                        UDF5 = u.UDF5,
                                        GUID = u.GUID,
                                        Created = u.Created,
                                        Updated = u.Updated,
                                        CreatedBy = u.CreatedBy,
                                        LastUpdatedBy = u.LastUpdatedBy,
                                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                        CompanyID = u.CompanyID,
                                        Room = u.Room,
                                        CreatedByName = u.CreatedByName,
                                        UpdatedByName = u.UpdatedByName,
                                        RoomName = u.RoomName,
                                        IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                                        ItemTypeName = u.ItemTypeName != null ? Convert.ToString(u.ItemTypeName) : string.Empty,
                                        CategoryName = u.CategoryName != null ? Convert.ToString(u.CategoryName) : string.Empty,
                                        Unit = u.Unit,
                                        GLAccount = u.GLAccount != null ? Convert.ToString(u.GLAccount) : string.Empty,
                                        IsItemLevelMinMaxQtyRequired = (u.IsItemLevelMinMaxQtyRequired.HasValue ? u.IsItemLevelMinMaxQtyRequired : false),
                                        IsEnforceDefaultReorderQuantity = (u.IsEnforceDefaultReorderQuantity.HasValue ? u.IsEnforceDefaultReorderQuantity : false),
                                        IsBuildBreak = (u.IsBuildBreak.HasValue ? u.IsBuildBreak : false),
                                        BondedInventory = u.BondedInventory != null ? Convert.ToString(u.BondedInventory) : string.Empty,
                                        //ItemLocations = objLocationDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, u.GUID, null, "ID ASC").ToList(),
                                        //AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Item Master"),
                                        TrendingSetting = u.TrendingSetting,
                                        IsAutoInventoryClassification = u.IsAutoInventoryClassification,
                                        PullQtyScanOverride = u.PullQtyScanOverride,
                                        IsPackslipMandatoryAtReceive = u.IsPackslipMandatoryAtReceive,
                                        BlanketOrderNumber = u.BlanketOrderNumber != null ? Convert.ToString(u.BlanketOrderNumber) : string.Empty,
                                        ItemImageExternalURL = u.ItemImageExternalURL != null ? Convert.ToString(u.ItemImageExternalURL) : string.Empty,
                                        ItemDocExternalURL = u.ItemDocExternalURL != null ? Convert.ToString(u.ItemDocExternalURL) : string.Empty,
                                        OutTransferQuantity = u.OutTransferQuantity,
                                        OnOrderInTransitQuantity = u.OnOrderInTransitQuantity ?? 0,
                                        ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                                        IsActive = (u.IsActive),
                                        IsOrderable = (u.IsOrderable),
                                        UDF6 = u.UDF6,
                                        UDF7 = u.UDF7,
                                        UDF8 = u.UDF8,
                                        UDF9 = u.UDF9,
                                        UDF10 = u.UDF10,
                                        IsAllowOrderCostuom = (u.IsAllowOrderCostuom),
                                        eLabelKey = u.eLabelKey,
                                        EnrichedProductData = u.EnrichedProductData,
                                        EnhancedDescription = u.EnhancedDescription,
                                        POItemLineNumber = u.POItemLineNumber
                                    }).ToList();
            }



            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ItemNumber");
            csw.WriteField("Manufacturer");
            csw.WriteField("ManufacturerNumber");
            csw.WriteField("* SupplierName");
            csw.WriteField("* SupplierPartNo");
            csw.WriteField("BlanketOrderNumber");
            csw.WriteField("UPC");
            csw.WriteField("UNSPSC");
            csw.WriteField("Description");
            csw.WriteField("LongDescription");
            csw.WriteField("CategoryName");
            csw.WriteField("GLAccount");
            csw.WriteField("* UOM");
            csw.WriteField("* CostUOM");
            csw.WriteField("* DefaultReorderQuantity");
            csw.WriteField("* DefaultPullQuantity");
            csw.WriteField("Cost");
            csw.WriteField("Markup");
            csw.WriteField("SellPrice");
            csw.WriteField("ExtendedCost");
            csw.WriteField("AverageCost");
            csw.WriteField("PerItemCost");
            csw.WriteField("LeadTimeInDays");
            csw.WriteField("Link1");
            csw.WriteField("Link2");
            //csw.WriteField("Trend");
            csw.WriteField("Taxable");
            csw.WriteField("Consignment");
            csw.WriteField("StagedQuantity");
            csw.WriteField("InTransitquantity");
            csw.WriteField("OnOrderQuantity");
            csw.WriteField("OnReturnQuantity");
            csw.WriteField("OnTransferQuantity");
            csw.WriteField("SuggestedOrderQuantity");
            csw.WriteField("RequisitionedQuantity");
            //csw.WriteField("PackingQuantity");
            csw.WriteField("AverageUsage");
            csw.WriteField("Turns");
            csw.WriteField("OnHandQuantity");

            csw.WriteField("CriticalQuantity");
            csw.WriteField("MinimumQuantity");
            csw.WriteField("MaximumQuantity");
            csw.WriteField("WeightPerPiece");
            csw.WriteField("ItemUniqueNumber");
            csw.WriteField("IsTransfer");
            csw.WriteField("IsPurchase");
            csw.WriteField("* InventryLocation");
            csw.WriteField("InventoryClassification");
            csw.WriteField("SerialNumberTracking");
            csw.WriteField("LotNumberTracking");
            csw.WriteField("DateCodeTracking");
            csw.WriteField("* ItemType");
            csw.WriteField("ImagePath");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("IsLotSerialExpiryCost");
            csw.WriteField("* IsItemLevelMinMaxQtyRequired");
            csw.WriteField("TrendingSetting");
            csw.WriteField("EnforceDefaultPullQuantity");
            csw.WriteField("EnforceDefaultReorderQuantity");
            csw.WriteField("IsAutoInventoryClassification");
            csw.WriteField("IsBuildBreak");
            csw.WriteField("IsPackslipMandatoryAtReceive");
            csw.WriteField("ItemImageExternalURL");
            csw.WriteField("ItemDocExternalURL");
            csw.WriteField("IsDeleted");
            csw.WriteField("OutTransferQuantity");
            csw.WriteField("OnOrderInTransitQuantity");
            csw.WriteField("ItemLink2ExternalURL");
            csw.WriteField("IsActive");
            csw.WriteField("UDF6");
            csw.WriteField("UDF7");
            csw.WriteField("UDF8");
            csw.WriteField("UDF9");
            csw.WriteField("UDF10");
            csw.WriteField("IsAllowOrderCostuom");
            csw.WriteField("eLabelKey");
            csw.WriteField("EnrichedProductData");
            csw.WriteField("EnhancedDescription");
            csw.WriteField("POItemLineNumber");
            csw.NextRecord();
            if (lstItemMasterDTO != null && lstItemMasterDTO.Count > 0)
            {
                foreach (ItemMasterDTO rec in ConvertExportDataList<ItemMasterDTO>(lstItemMasterDTO).ToList())
                {
                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.ManufacturerName);
                    csw.WriteField(rec.ManufacturerNumber);
                    if (rec.ItemType == 4)
                        csw.WriteField("");
                    else
                        csw.WriteField(rec.SupplierName);
                    csw.WriteField(rec.SupplierPartNo);
                    csw.WriteField(rec.BlanketOrderNumber);
                    csw.WriteField(rec.UPC);
                    csw.WriteField(rec.UNSPSC);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.LongDescription);
                    csw.WriteField(rec.CategoryName);
                    csw.WriteField(rec.GLAccount);
                    csw.WriteField(rec.Unit);
                    csw.WriteField(rec.CostUOMName);
                    csw.WriteField(rec.DefaultReorderQuantity);
                    csw.WriteField(rec.DefaultPullQuantity);
                    csw.WriteField(rec.Cost);
                    csw.WriteField(rec.Markup);
                    csw.WriteField(rec.SellPrice);
                    csw.WriteField(rec.ExtendedCost);
                    csw.WriteField(rec.AverageCost);
                    csw.WriteField(rec.PerItemCost);
                    csw.WriteField(rec.LeadTimeInDays);
                    csw.WriteField(rec.Link1);
                    csw.WriteField(rec.Link2);
                    //csw.WriteField(rec.Trend);
                    csw.WriteField(rec.Taxable);
                    csw.WriteField(rec.Consignment);
                    csw.WriteField(rec.StagedQuantity ?? 0);
                    csw.WriteField(rec.InTransitquantity);
                    csw.WriteField(rec.OnOrderQuantity);
                    csw.WriteField(rec.OnReturnQuantity);
                    csw.WriteField(rec.OnTransferQuantity);
                    csw.WriteField(rec.SuggestedOrderQuantity);
                    csw.WriteField(rec.RequisitionedQuantity);
                    //csw.WriteField(rec.PackingQuantity);
                    csw.WriteField(rec.AverageUsage);
                    csw.WriteField(rec.Turns);
                    csw.WriteField(rec.OnHandQuantity);

                    if (rec.IsItemLevelMinMaxQtyRequired == true)
                    {
                        csw.WriteField(rec.CriticalQuantity);
                        csw.WriteField(rec.MinimumQuantity);
                        csw.WriteField(rec.MaximumQuantity);
                    }
                    else //if (!string.IsNullOrEmpty(rec.DefaultLocationName) && rec.ItemType != 4)
                    {
                        //BinMasterDTO objBinMasterDTO = new BinMasterDTO();
                        //BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                        //objBinMasterDTO = objBinMasterDAL.GetRecord(rec.DefaultLocationName, rec.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                        //if (objBinMasterDTO != null)
                        //{
                        //    csw.WriteField(objBinMasterDTO.CriticalQuantity ?? 0);
                        //    csw.WriteField(objBinMasterDTO.MinimumQuantity ?? 0);
                        //    csw.WriteField(objBinMasterDTO.MaximumQuantity ?? 0);
                        //}
                        csw.WriteField("N/A");
                        csw.WriteField("N/A");
                        csw.WriteField("N/A");
                    }

                    csw.WriteField(rec.WeightPerPiece);
                    csw.WriteField(rec.ItemUniqueNumber);
                    csw.WriteField(rec.IsTransfer);
                    csw.WriteField(rec.IsPurchase);
                    if (rec.ItemType == 4)
                        csw.WriteField("");
                    else
                        csw.WriteField(" " + rec.DefaultLocationName);
                    csw.WriteField(rec.InventoryClassificationName);
                    csw.WriteField(rec.SerialNumberTracking);
                    csw.WriteField(rec.LotNumberTracking);
                    csw.WriteField(rec.DateCodeTracking);
                    csw.WriteField(GetItemType(rec.ItemType));
                    csw.WriteField(rec.ImagePath);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.WriteField(rec.IsLotSerialExpiryCost);
                    csw.WriteField(rec.IsItemLevelMinMaxQtyRequired);
                    csw.WriteField(GetTrendingSetting(rec.TrendingSetting));
                    csw.WriteField(rec.PullQtyScanOverride);
                    csw.WriteField(rec.IsEnforceDefaultReorderQuantity ?? false);
                    csw.WriteField(rec.IsAutoInventoryClassification);
                    csw.WriteField(rec.IsBuildBreak);
                    csw.WriteField(rec.IsPackslipMandatoryAtReceive);
                    csw.WriteField(rec.ItemImageExternalURL);
                    csw.WriteField(rec.ItemDocExternalURL);
                    csw.WriteField(rec.IsDeleted);
                    csw.WriteField(rec.OutTransferQuantity);
                    csw.WriteField(rec.OnOrderInTransitQuantity);
                    csw.WriteField(rec.ItemLink2ExternalURL);
                    csw.WriteField(rec.IsOrderable);
                    csw.WriteField(rec.UDF6);
                    csw.WriteField(rec.UDF7);
                    csw.WriteField(rec.UDF8);
                    csw.WriteField(rec.UDF9);
                    csw.WriteField(rec.UDF10);
                    csw.WriteField(rec.IsAllowOrderCostuom);
                    csw.WriteField(rec.eLabelKey);
                    csw.WriteField(rec.EnrichedProductData);
                    csw.WriteField(rec.EnhancedDescription);
                    csw.WriteField(rec.POItemLineNumber);
                    csw.NextRecord();
                }
            }
            write.Close();
            write.Dispose();
            return filename;
        }

        public string AlertStockOutListCSV(string Filepath, string ModuleName, string ids, string SortNameString, string CallFromPage = "")
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> lstIds = arrid.Select(t => Guid.Parse(t)).ToList();
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<ItemMasterDTO> DataFromDB = null;
            SortNameString = (SortNameString ?? string.Empty).Replace("OrderCost", "ItemNumber");
            if (!string.IsNullOrEmpty(CallFromPage) && CallFromPage.Trim().ToLower() == "import")
            {
                if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                {
                    DataFromDB = obj.GetItemsByArrayForImport(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds).OrderBy(SortNameString); //).Where(i => i.IsDeleted == false && i.Room == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && SessionHelper.UserSupplierIds.Contains(i.SupplierID.GetValueOrDefault(0))).OrderBy(SortNameString);
                }
                else
                {
                    DataFromDB = obj.GetItemsByArrayForImport(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID, new List<long>()).OrderBy(SortNameString); //.Where(i => i.IsDeleted == false && i.Room == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID).OrderBy(SortNameString);
                }
            }
            else
            {
                DataFromDB = obj.GetItemsByArray(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString);//.Where(i => i.Room == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID).OrderBy(SortNameString);
            }
            List<ItemMasterDTO> lstItemMasterDTO = new List<ItemMasterDTO>();

            //if (!string.IsNullOrEmpty(ids))
            {
                lstItemMasterDTO = (from u in DataFromDB
                                    where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.GUID.ToString())
                                    select new ItemMasterDTO
                                    {
                                        ID = u.ID,
                                        ItemNumber = u.ItemNumber != null ? Convert.ToString(u.ItemNumber) : string.Empty,
                                        ManufacturerID = u.ManufacturerID,
                                        ManufacturerNumber = u.ManufacturerNumber != null ? Convert.ToString(u.ManufacturerNumber) : string.Empty,
                                        ManufacturerName = u.ManufacturerName != null ? Convert.ToString(u.ManufacturerName) : string.Empty,
                                        SupplierID = u.SupplierID,
                                        SupplierPartNo = u.SupplierPartNo != null ? Convert.ToString(u.SupplierPartNo) : string.Empty,
                                        SupplierName = u.SupplierName != null ? Convert.ToString(u.SupplierName) : string.Empty,
                                        UPC = u.UPC,
                                        UNSPSC = u.UNSPSC,
                                        Description = u.Description != null ? Convert.ToString(u.Description) : string.Empty,
                                        LongDescription = u.LongDescription != null ? Convert.ToString(u.LongDescription) : string.Empty,
                                        CategoryID = u.CategoryID,
                                        GLAccountID = u.GLAccountID,
                                        UOMID = u.UOMID,

                                        PricePerTerm = u.PricePerTerm,
                                        CostUOMID = u.CostUOMID,
                                        CostUOMName = u.CostUOMName != null ? Convert.ToString(u.CostUOMName) : string.Empty,
                                        DefaultReorderQuantity = u.DefaultReorderQuantity ?? 0,
                                        DefaultPullQuantity = u.DefaultPullQuantity ?? 0,
                                        Cost = u.Cost ?? 0,
                                        Markup = u.Markup ?? 0,
                                        SellPrice = u.SellPrice ?? 0,
                                        ExtendedCost = u.ExtendedCost ?? 0,
                                        AverageCost = u.AverageCost ?? 0,
                                        PerItemCost = u.PerItemCost ?? 0,
                                        LeadTimeInDays = u.LeadTimeInDays ?? 0,
                                        Link1 = u.Link1 != null ? Convert.ToString(u.Link1) : string.Empty,
                                        Link2 = u.Link2 != null ? Convert.ToString(u.Link2) : string.Empty,
                                        Trend = u.Trend,
                                        Taxable = u.Taxable,
                                        Consignment = u.Consignment,
                                        StagedQuantity = u.StagedQuantity,
                                        InTransitquantity = u.InTransitquantity ?? 0,
                                        OnOrderQuantity = u.OnOrderQuantity ?? 0,
                                        OnReturnQuantity = u.OnReturnQuantity ?? 0,
                                        OnTransferQuantity = u.OnTransferQuantity ?? 0,
                                        SuggestedOrderQuantity = u.SuggestedOrderQuantity ?? 0,
                                        SuggestedTransferQuantity = u.SuggestedTransferQuantity ?? 0,
                                        RequisitionedQuantity = u.RequisitionedQuantity,
                                        PackingQuantity = u.PackingQuantity ?? 0,
                                        AverageUsage = u.AverageUsage ?? 0,
                                        Turns = u.Turns ?? 0,
                                        OnHandQuantity = 0,
                                        CriticalQuantity = u.CriticalQuantity,
                                        MinimumQuantity = u.MinimumQuantity,
                                        MaximumQuantity = u.MaximumQuantity,
                                        WeightPerPiece = u.WeightPerPiece,
                                        ItemUniqueNumber = u.ItemUniqueNumber != null ? Convert.ToString(u.ItemUniqueNumber) : string.Empty,
                                        IsPurchase = u.IsPurchase,
                                        IsTransfer = u.IsTransfer,
                                        DefaultLocation = u.DefaultLocation,
                                        DefaultLocationName = u.DefaultLocationName != null ? Convert.ToString(u.DefaultLocationName).Replace("[|EmptyStagingBin|]", "") : string.Empty,
                                        InventoryClassification = u.InventoryClassification,
                                        InventoryClassificationName = u.InventoryClassificationName != null ? Convert.ToString(u.InventoryClassificationName) : string.Empty,
                                        SerialNumberTracking = u.SerialNumberTracking,
                                        LotNumberTracking = u.LotNumberTracking,
                                        DateCodeTracking = u.DateCodeTracking,
                                        ItemType = u.ItemType,
                                        ImagePath = u.ImagePath,
                                        UDF1 = u.UDF1,
                                        UDF2 = u.UDF2,
                                        UDF3 = u.UDF3,
                                        UDF4 = u.UDF4,
                                        UDF5 = u.UDF5,
                                        GUID = u.GUID,
                                        Created = u.Created,
                                        Updated = u.Updated,
                                        CreatedBy = u.CreatedBy,
                                        LastUpdatedBy = u.LastUpdatedBy,
                                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                        CompanyID = u.CompanyID,
                                        Room = u.Room,
                                        CreatedByName = u.CreatedByName,
                                        UpdatedByName = u.UpdatedByName,
                                        RoomName = u.RoomName,
                                        IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                                        ItemTypeName = u.ItemTypeName != null ? Convert.ToString(u.ItemTypeName) : string.Empty,
                                        CategoryName = u.CategoryName != null ? Convert.ToString(u.CategoryName) : string.Empty,
                                        Unit = u.Unit,
                                        GLAccount = u.GLAccount != null ? Convert.ToString(u.GLAccount) : string.Empty,
                                        IsItemLevelMinMaxQtyRequired = (u.IsItemLevelMinMaxQtyRequired.HasValue ? u.IsItemLevelMinMaxQtyRequired : false),
                                        IsEnforceDefaultReorderQuantity = (u.IsEnforceDefaultReorderQuantity.HasValue ? u.IsEnforceDefaultReorderQuantity : false),
                                        IsBuildBreak = (u.IsBuildBreak.HasValue ? u.IsBuildBreak : false),
                                        BondedInventory = u.BondedInventory != null ? Convert.ToString(u.BondedInventory) : string.Empty,
                                        TrendingSetting = u.TrendingSetting,
                                        IsAutoInventoryClassification = u.IsAutoInventoryClassification,
                                        PullQtyScanOverride = u.PullQtyScanOverride,
                                        IsPackslipMandatoryAtReceive = u.IsPackslipMandatoryAtReceive,
                                        BlanketOrderNumber = u.BlanketOrderNumber != null ? Convert.ToString(u.BlanketOrderNumber) : string.Empty,
                                        ItemImageExternalURL = u.ItemImageExternalURL != null ? Convert.ToString(u.ItemImageExternalURL) : string.Empty,
                                        ItemDocExternalURL = u.ItemDocExternalURL != null ? Convert.ToString(u.ItemDocExternalURL) : string.Empty,
                                        OutTransferQuantity = u.OutTransferQuantity,
                                        OnOrderInTransitQuantity = u.OnOrderInTransitQuantity ?? 0,
                                        ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                                        IsActive = (u.IsActive),
                                        UDF6 = u.UDF6,
                                        UDF7 = u.UDF7,
                                        UDF8 = u.UDF8,
                                        UDF9 = u.UDF9,
                                        UDF10 = u.UDF10,
                                        IsAllowOrderCostuom = (u.IsAllowOrderCostuom),
                                    }).ToList();
            }



            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ItemNumber");
            csw.WriteField("Manufacturer");
            csw.WriteField("ManufacturerNumber");
            csw.WriteField("* SupplierName");
            csw.WriteField("* SupplierPartNo");
            csw.WriteField("BlanketOrderNumber");
            csw.WriteField("UPC");
            csw.WriteField("UNSPSC");
            csw.WriteField("Description");
            csw.WriteField("LongDescription");
            csw.WriteField("CategoryName");
            csw.WriteField("GLAccount");
            csw.WriteField("* UOM");
            csw.WriteField("* CostUOM");
            csw.WriteField("* DefaultReorderQuantity");
            csw.WriteField("* DefaultPullQuantity");
            csw.WriteField("Cost");
            csw.WriteField("Markup");
            csw.WriteField("SellPrice");
            csw.WriteField("ExtendedCost");
            csw.WriteField("AverageCost");
            csw.WriteField("PerItemCost");
            csw.WriteField("LeadTimeInDays");
            csw.WriteField("Link1");
            csw.WriteField("Link2");
            csw.WriteField("Taxable");
            csw.WriteField("Consignment");
            csw.WriteField("StagedQuantity");
            csw.WriteField("InTransitquantity");
            csw.WriteField("OnOrderQuantity");
            csw.WriteField("OnReturnQuantity");
            csw.WriteField("OnTransferQuantity");
            csw.WriteField("SuggestedOrderQuantity");
            csw.WriteField("RequisitionedQuantity");
            csw.WriteField("AverageUsage");
            csw.WriteField("Turns");
            csw.WriteField("OnHandQuantity");

            csw.WriteField("CriticalQuantity");
            csw.WriteField("MinimumQuantity");
            csw.WriteField("MaximumQuantity");
            csw.WriteField("WeightPerPiece");
            csw.WriteField("ItemUniqueNumber");
            csw.WriteField("IsTransfer");
            csw.WriteField("IsPurchase");
            csw.WriteField("* InventryLocation");
            csw.WriteField("InventoryClassification");
            csw.WriteField("SerialNumberTracking");
            csw.WriteField("LotNumberTracking");
            csw.WriteField("DateCodeTracking");
            csw.WriteField("* ItemType");
            csw.WriteField("ImagePath");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("IsLotSerialExpiryCost");
            csw.WriteField("* IsItemLevelMinMaxQtyRequired");
            csw.WriteField("TrendingSetting");
            csw.WriteField("EnforceDefaultPullQuantity");
            csw.WriteField("EnforceDefaultReorderQuantity");
            csw.WriteField("IsAutoInventoryClassification");
            csw.WriteField("IsBuildBreak");
            csw.WriteField("IsPackslipMandatoryAtReceive");
            csw.WriteField("ItemImageExternalURL");
            csw.WriteField("ItemDocExternalURL");
            csw.WriteField("IsDeleted");
            csw.WriteField("OutTransferQuantity");
            csw.WriteField("OnOrderInTransitQuantity");
            csw.WriteField("ItemLink2ExternalURL");
            csw.WriteField("IsActive");
            csw.WriteField("UDF6");
            csw.WriteField("UDF7");
            csw.WriteField("UDF8");
            csw.WriteField("UDF9");
            csw.WriteField("UDF10");
            csw.WriteField("IsAllowOrderCostuom");
            csw.NextRecord();
            if (lstItemMasterDTO != null && lstItemMasterDTO.Count > 0)
            {
                foreach (ItemMasterDTO rec in ConvertExportDataList<ItemMasterDTO>(lstItemMasterDTO).ToList())
                {
                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.ManufacturerName);
                    csw.WriteField(rec.ManufacturerNumber);
                    csw.WriteField(rec.SupplierName);
                    csw.WriteField(rec.SupplierPartNo);
                    csw.WriteField(rec.BlanketOrderNumber);
                    csw.WriteField(rec.UPC);
                    csw.WriteField(rec.UNSPSC);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.LongDescription);
                    csw.WriteField(rec.CategoryName);
                    csw.WriteField(rec.GLAccount);
                    csw.WriteField(rec.Unit);
                    csw.WriteField(rec.CostUOMName);
                    csw.WriteField(rec.DefaultReorderQuantity);
                    csw.WriteField(rec.DefaultPullQuantity);
                    csw.WriteField(rec.Cost);
                    csw.WriteField(rec.Markup);
                    csw.WriteField(rec.SellPrice);
                    csw.WriteField(rec.ExtendedCost);
                    csw.WriteField(rec.AverageCost);
                    csw.WriteField(rec.PerItemCost);
                    csw.WriteField(rec.LeadTimeInDays);
                    csw.WriteField(rec.Link1);
                    csw.WriteField(rec.Link2);
                    csw.WriteField(rec.Taxable);
                    csw.WriteField(rec.Consignment);
                    csw.WriteField(rec.StagedQuantity ?? 0);
                    csw.WriteField(rec.InTransitquantity);
                    csw.WriteField(rec.OnOrderQuantity);
                    csw.WriteField(rec.OnReturnQuantity);
                    csw.WriteField(rec.OnTransferQuantity);
                    csw.WriteField(rec.SuggestedOrderQuantity);
                    csw.WriteField(rec.RequisitionedQuantity);
                    csw.WriteField(rec.AverageUsage);
                    csw.WriteField(rec.Turns);
                    csw.WriteField(rec.OnHandQuantity);

                    if (rec.IsItemLevelMinMaxQtyRequired == true)
                    {
                        csw.WriteField(rec.CriticalQuantity);
                        csw.WriteField(rec.MinimumQuantity);
                        csw.WriteField(rec.MaximumQuantity);
                    }
                    else //if (!string.IsNullOrEmpty(rec.DefaultLocationName) && rec.ItemType != 4)
                    {
                        csw.WriteField("N/A");
                        csw.WriteField("N/A");
                        csw.WriteField("N/A");
                    }

                    csw.WriteField(rec.WeightPerPiece);
                    csw.WriteField(rec.ItemUniqueNumber);
                    csw.WriteField(rec.IsTransfer);
                    csw.WriteField(rec.IsPurchase);
                    csw.WriteField(" " + rec.DefaultLocationName);
                    csw.WriteField(rec.InventoryClassificationName);
                    csw.WriteField(rec.SerialNumberTracking);
                    csw.WriteField(rec.LotNumberTracking);
                    csw.WriteField(rec.DateCodeTracking);
                    csw.WriteField(GetItemType(rec.ItemType));
                    csw.WriteField(rec.ImagePath);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.WriteField(rec.IsLotSerialExpiryCost);
                    csw.WriteField(rec.IsItemLevelMinMaxQtyRequired);
                    csw.WriteField(GetTrendingSetting(rec.TrendingSetting));
                    csw.WriteField(rec.PullQtyScanOverride);
                    csw.WriteField(rec.IsEnforceDefaultReorderQuantity ?? false);
                    csw.WriteField(rec.IsAutoInventoryClassification);
                    csw.WriteField(rec.IsBuildBreak);
                    csw.WriteField(rec.IsPackslipMandatoryAtReceive);
                    csw.WriteField(rec.ItemImageExternalURL);
                    csw.WriteField(rec.ItemDocExternalURL);
                    csw.WriteField(rec.IsDeleted);
                    csw.WriteField(rec.OutTransferQuantity);
                    csw.WriteField(rec.OnOrderInTransitQuantity);
                    csw.WriteField(rec.ItemLink2ExternalURL);
                    csw.WriteField(rec.IsActive);
                    csw.WriteField(rec.UDF6);
                    csw.WriteField(rec.UDF7);
                    csw.WriteField(rec.UDF8);
                    csw.WriteField(rec.UDF9);
                    csw.WriteField(rec.UDF10);
                    csw.WriteField(rec.IsAllowOrderCostuom);
                    csw.NextRecord();
                }
            }
            write.Close();

            return filename;
        }

        public string ItemBinMasterListCSV(string Filepath, string ModuleName, string ids, string SortNameString, string CallFromPage = "", string BinIDs = "")
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> lstIds = arrid.Select(t => Guid.Parse(t)).ToList();
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<ItemMasterDTO> DataFromDB = null;
            SortNameString = (SortNameString ?? string.Empty).Replace("OrderCost", "ItemNumber");
            if (!string.IsNullOrEmpty(CallFromPage) && CallFromPage.Trim().ToLower() == "import")
            {
                if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                {
                    //DataFromDB = obj.GetItemsByArrayForImport(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds).OrderBy(SortNameString); 
                    DataFromDB = obj.GetItemsBinByArrayForImport(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds, BinIDs).OrderBy(SortNameString);
                }
                else
                {
                    //DataFromDB = obj.GetItemsByArrayForImport(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID, new List<long>()).OrderBy(SortNameString); 
                    DataFromDB = obj.GetItemsBinByArrayForImport(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID, new List<long>(), BinIDs).OrderBy(SortNameString);
                }
            }
            else
            {
                //DataFromDB = obj.GetItemsByArray(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString);
                DataFromDB = obj.GetItemsBinByArray(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID, BinIDs).OrderBy(SortNameString);
            }
            List<ItemMasterDTO> lstItemMasterDTO = new List<ItemMasterDTO>();

            //if (!string.IsNullOrEmpty(ids))
            {
                lstItemMasterDTO = (from u in DataFromDB
                                    where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.GUID.ToString())
                                    select new ItemMasterDTO
                                    {
                                        ID = u.ID,
                                        ItemNumber = u.ItemNumber != null ? Convert.ToString(u.ItemNumber) : string.Empty,
                                        ManufacturerID = u.ManufacturerID,
                                        ManufacturerNumber = u.ManufacturerNumber != null ? Convert.ToString(u.ManufacturerNumber) : string.Empty,
                                        ManufacturerName = u.ManufacturerName != null ? Convert.ToString(u.ManufacturerName) : string.Empty,
                                        SupplierID = u.SupplierID,
                                        SupplierPartNo = u.SupplierPartNo != null ? Convert.ToString(u.SupplierPartNo) : string.Empty,
                                        SupplierName = u.SupplierName != null ? Convert.ToString(u.SupplierName) : string.Empty,
                                        UPC = u.UPC,
                                        UNSPSC = u.UNSPSC,
                                        Description = u.Description != null ? Convert.ToString(u.Description) : string.Empty,
                                        LongDescription = u.LongDescription != null ? Convert.ToString(u.LongDescription) : string.Empty,
                                        CategoryID = u.CategoryID,
                                        GLAccountID = u.GLAccountID,
                                        UOMID = u.UOMID,

                                        PricePerTerm = u.PricePerTerm,
                                        CostUOMID = u.CostUOMID,
                                        CostUOMName = u.CostUOMName != null ? Convert.ToString(u.CostUOMName) : string.Empty,
                                        DefaultReorderQuantity = u.DefaultReorderQuantity ?? 0,
                                        DefaultPullQuantity = u.DefaultPullQuantity ?? 0,
                                        Cost = u.Cost ?? 0,
                                        Markup = u.Markup ?? 0,
                                        SellPrice = u.SellPrice ?? 0,
                                        ExtendedCost = u.ExtendedCost ?? 0,
                                        AverageCost = u.AverageCost ?? 0,
                                        PerItemCost = u.PerItemCost ?? 0,
                                        LeadTimeInDays = u.LeadTimeInDays ?? 0,
                                        Link1 = u.Link1 != null ? Convert.ToString(u.Link1) : string.Empty,
                                        Link2 = u.Link2 != null ? Convert.ToString(u.Link2) : string.Empty,
                                        Trend = u.Trend,
                                        Taxable = u.Taxable,
                                        Consignment = u.Consignment,
                                        StagedQuantity = u.StagedQuantity,
                                        InTransitquantity = u.InTransitquantity ?? 0,
                                        OnOrderQuantity = u.OnOrderQuantity ?? 0,
                                        OnReturnQuantity = u.OnReturnQuantity ?? 0,
                                        OnTransferQuantity = u.OnTransferQuantity ?? 0,
                                        SuggestedOrderQuantity = u.SuggestedOrderQuantity ?? 0,
                                        SuggestedTransferQuantity = u.SuggestedTransferQuantity ?? 0,
                                        RequisitionedQuantity = u.RequisitionedQuantity,
                                        PackingQuantity = u.PackingQuantity ?? 0,
                                        AverageUsage = u.AverageUsage ?? 0,
                                        Turns = u.Turns ?? 0,
                                        OnHandQuantity = u.OnHandQuantity ?? 0,
                                        CriticalQuantity = u.CriticalQuantity,
                                        MinimumQuantity = u.MinimumQuantity,
                                        MaximumQuantity = u.MaximumQuantity,
                                        WeightPerPiece = u.WeightPerPiece,
                                        ItemUniqueNumber = u.ItemUniqueNumber != null ? Convert.ToString(u.ItemUniqueNumber) : string.Empty,
                                        IsPurchase = u.IsPurchase,
                                        IsTransfer = u.IsTransfer,
                                        DefaultLocation = u.DefaultLocation,
                                        DefaultLocationName = u.DefaultLocationName != null ? Convert.ToString(u.DefaultLocationName).Replace("[|EmptyStagingBin|]", "") : string.Empty,
                                        InventoryClassification = u.InventoryClassification,
                                        InventoryClassificationName = u.InventoryClassificationName != null ? Convert.ToString(u.InventoryClassificationName) : string.Empty,
                                        SerialNumberTracking = u.SerialNumberTracking,
                                        LotNumberTracking = u.LotNumberTracking,
                                        DateCodeTracking = u.DateCodeTracking,
                                        ItemType = u.ItemType,
                                        ImagePath = u.ImagePath,
                                        UDF1 = u.UDF1,
                                        UDF2 = u.UDF2,
                                        UDF3 = u.UDF3,
                                        UDF4 = u.UDF4,
                                        UDF5 = u.UDF5,
                                        GUID = u.GUID,
                                        Created = u.Created,
                                        Updated = u.Updated,
                                        CreatedBy = u.CreatedBy,
                                        LastUpdatedBy = u.LastUpdatedBy,
                                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                        CompanyID = u.CompanyID,
                                        Room = u.Room,
                                        CreatedByName = u.CreatedByName,
                                        UpdatedByName = u.UpdatedByName,
                                        RoomName = u.RoomName,
                                        IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                                        ItemTypeName = u.ItemTypeName != null ? Convert.ToString(u.ItemTypeName) : string.Empty,
                                        CategoryName = u.CategoryName != null ? Convert.ToString(u.CategoryName) : string.Empty,
                                        Unit = u.Unit,
                                        GLAccount = u.GLAccount != null ? Convert.ToString(u.GLAccount) : string.Empty,
                                        IsItemLevelMinMaxQtyRequired = (u.IsItemLevelMinMaxQtyRequired.HasValue ? u.IsItemLevelMinMaxQtyRequired : false),
                                        IsEnforceDefaultReorderQuantity = (u.IsEnforceDefaultReorderQuantity.HasValue ? u.IsEnforceDefaultReorderQuantity : false),
                                        IsBuildBreak = (u.IsBuildBreak.HasValue ? u.IsBuildBreak : false),
                                        BondedInventory = u.BondedInventory != null ? Convert.ToString(u.BondedInventory) : string.Empty,
                                        //ItemLocations = objLocationDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, u.GUID, null, "ID ASC").ToList(),
                                        //AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Item Master"),
                                        TrendingSetting = u.TrendingSetting,
                                        IsAutoInventoryClassification = u.IsAutoInventoryClassification,
                                        PullQtyScanOverride = u.PullQtyScanOverride,
                                        IsPackslipMandatoryAtReceive = u.IsPackslipMandatoryAtReceive,
                                        BlanketOrderNumber = u.BlanketOrderNumber != null ? Convert.ToString(u.BlanketOrderNumber) : string.Empty,
                                        ItemImageExternalURL = u.ItemImageExternalURL != null ? Convert.ToString(u.ItemImageExternalURL) : string.Empty,
                                        ItemDocExternalURL = u.ItemDocExternalURL != null ? Convert.ToString(u.ItemDocExternalURL) : string.Empty,
                                        OutTransferQuantity = u.OutTransferQuantity,
                                        OnOrderInTransitQuantity = u.OnOrderInTransitQuantity ?? 0,
                                        ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                                        IsActive = (u.IsActive),
                                        UDF6 = u.UDF6,
                                        UDF7 = u.UDF7,
                                        UDF8 = u.UDF8,
                                        UDF9 = u.UDF9,
                                        UDF10 = u.UDF10,
                                        IsAllowOrderCostuom = (u.IsAllowOrderCostuom),
                                        BinNumber = u.BinNumber
                                    }).ToList();
            }



            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ItemNumber");
            csw.WriteField("Manufacturer");
            csw.WriteField("ManufacturerNumber");
            csw.WriteField("* SupplierName");
            csw.WriteField("* SupplierPartNo");
            csw.WriteField("BlanketOrderNumber");
            csw.WriteField("UPC");
            csw.WriteField("UNSPSC");
            csw.WriteField("Description");
            csw.WriteField("LongDescription");
            csw.WriteField("CategoryName");
            csw.WriteField("GLAccount");
            csw.WriteField("* UOM");
            csw.WriteField("* CostUOM");
            csw.WriteField("* DefaultReorderQuantity");
            csw.WriteField("* DefaultPullQuantity");
            csw.WriteField("Cost");
            csw.WriteField("Markup");
            csw.WriteField("SellPrice");
            csw.WriteField("ExtendedCost");
            csw.WriteField("AverageCost");
            csw.WriteField("PerItemCost");
            csw.WriteField("LeadTimeInDays");
            csw.WriteField("Link1");
            csw.WriteField("Link2");
            //csw.WriteField("Trend");
            csw.WriteField("Taxable");
            csw.WriteField("Consignment");
            csw.WriteField("StagedQuantity");
            csw.WriteField("InTransitquantity");
            csw.WriteField("OnOrderQuantity");
            csw.WriteField("OnReturnQuantity");
            csw.WriteField("OnTransferQuantity");
            csw.WriteField("SuggestedOrderQuantity");
            csw.WriteField("RequisitionedQuantity");
            //csw.WriteField("PackingQuantity");
            csw.WriteField("AverageUsage");
            csw.WriteField("Turns");
            csw.WriteField("OnHandQuantity");

            csw.WriteField("CriticalQuantity");
            csw.WriteField("MinimumQuantity");
            csw.WriteField("MaximumQuantity");
            csw.WriteField("WeightPerPiece");
            csw.WriteField("ItemUniqueNumber");
            csw.WriteField("IsTransfer");
            csw.WriteField("IsPurchase");
            csw.WriteField("* InventryLocation");
            csw.WriteField("InventoryClassification");
            csw.WriteField("SerialNumberTracking");
            csw.WriteField("LotNumberTracking");
            csw.WriteField("DateCodeTracking");
            csw.WriteField("* ItemType");
            csw.WriteField("ImagePath");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("IsLotSerialExpiryCost");
            csw.WriteField("* IsItemLevelMinMaxQtyRequired");
            csw.WriteField("TrendingSetting");
            csw.WriteField("EnforceDefaultPullQuantity");
            csw.WriteField("EnforceDefaultReorderQuantity");
            csw.WriteField("IsAutoInventoryClassification");
            csw.WriteField("IsBuildBreak");
            csw.WriteField("IsPackslipMandatoryAtReceive");
            csw.WriteField("ItemImageExternalURL");
            csw.WriteField("ItemDocExternalURL");
            csw.WriteField("IsDeleted");
            csw.WriteField("OutTransferQuantity");
            csw.WriteField("OnOrderInTransitQuantity");
            csw.WriteField("ItemLink2ExternalURL");
            csw.WriteField("IsActive");
            csw.WriteField("UDF6");
            csw.WriteField("UDF7");
            csw.WriteField("UDF8");
            csw.WriteField("UDF9");
            csw.WriteField("UDF10");
            csw.WriteField("IsAllowOrderCostuom");
            csw.WriteField("BinNumber");
            csw.NextRecord();
            if (lstItemMasterDTO != null && lstItemMasterDTO.Count > 0)
            {


                foreach (ItemMasterDTO rec in ConvertExportDataList<ItemMasterDTO>(lstItemMasterDTO).ToList())
                {
                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.ManufacturerName);
                    csw.WriteField(rec.ManufacturerNumber);
                    csw.WriteField(rec.SupplierName);
                    csw.WriteField(rec.SupplierPartNo);
                    csw.WriteField(rec.BlanketOrderNumber);
                    csw.WriteField(rec.UPC);
                    csw.WriteField(rec.UNSPSC);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.LongDescription);
                    csw.WriteField(rec.CategoryName);
                    csw.WriteField(rec.GLAccount);
                    csw.WriteField(rec.Unit);
                    csw.WriteField(rec.CostUOMName);
                    csw.WriteField(rec.DefaultReorderQuantity);
                    csw.WriteField(rec.DefaultPullQuantity);
                    csw.WriteField(rec.Cost);
                    csw.WriteField(rec.Markup);
                    csw.WriteField(rec.SellPrice);
                    csw.WriteField(rec.ExtendedCost);
                    csw.WriteField(rec.AverageCost);
                    csw.WriteField(rec.PerItemCost);
                    csw.WriteField(rec.LeadTimeInDays);
                    csw.WriteField(rec.Link1);
                    csw.WriteField(rec.Link2);
                    //csw.WriteField(rec.Trend);
                    csw.WriteField(rec.Taxable);
                    csw.WriteField(rec.Consignment);
                    csw.WriteField(rec.StagedQuantity ?? 0);
                    csw.WriteField(rec.InTransitquantity);
                    csw.WriteField(rec.OnOrderQuantity);
                    csw.WriteField(rec.OnReturnQuantity);
                    csw.WriteField(rec.OnTransferQuantity);
                    csw.WriteField(rec.SuggestedOrderQuantity);
                    csw.WriteField(rec.RequisitionedQuantity);
                    //csw.WriteField(rec.PackingQuantity);
                    csw.WriteField(rec.AverageUsage);
                    csw.WriteField(rec.Turns);
                    csw.WriteField(rec.OnHandQuantity);

                    //if (rec.IsItemLevelMinMaxQtyRequired == true)
                    //{
                    csw.WriteField(rec.CriticalQuantity);
                    csw.WriteField(rec.MinimumQuantity);
                    csw.WriteField(rec.MaximumQuantity);
                    //}
                    //else //if (!string.IsNullOrEmpty(rec.DefaultLocationName) && rec.ItemType != 4)
                    //{
                    //    //BinMasterDTO objBinMasterDTO = new BinMasterDTO();
                    //    //BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    //    //objBinMasterDTO = objBinMasterDAL.GetRecord(rec.DefaultLocationName, rec.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    //    //if (objBinMasterDTO != null)
                    //    //{
                    //    //    csw.WriteField(objBinMasterDTO.CriticalQuantity ?? 0);
                    //    //    csw.WriteField(objBinMasterDTO.MinimumQuantity ?? 0);
                    //    //    csw.WriteField(objBinMasterDTO.MaximumQuantity ?? 0);
                    //    //}
                    //    csw.WriteField("N/A");
                    //    csw.WriteField("N/A");
                    //    csw.WriteField("N/A");
                    //}

                    csw.WriteField(rec.WeightPerPiece);
                    csw.WriteField(rec.ItemUniqueNumber);
                    csw.WriteField(rec.IsTransfer);
                    csw.WriteField(rec.IsPurchase);
                    csw.WriteField(rec.DefaultLocationName);
                    csw.WriteField(rec.InventoryClassificationName);
                    csw.WriteField(rec.SerialNumberTracking);
                    csw.WriteField(rec.LotNumberTracking);
                    csw.WriteField(rec.DateCodeTracking);
                    csw.WriteField(GetItemType(rec.ItemType));
                    csw.WriteField(rec.ImagePath);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.WriteField(rec.IsLotSerialExpiryCost);
                    csw.WriteField(rec.IsItemLevelMinMaxQtyRequired);
                    csw.WriteField(GetTrendingSetting(rec.TrendingSetting));
                    csw.WriteField(rec.PullQtyScanOverride);
                    csw.WriteField(rec.IsEnforceDefaultReorderQuantity ?? false);
                    csw.WriteField(rec.IsAutoInventoryClassification);
                    csw.WriteField(rec.IsBuildBreak);
                    csw.WriteField(rec.IsPackslipMandatoryAtReceive);
                    csw.WriteField(rec.ItemImageExternalURL);
                    csw.WriteField(rec.ItemDocExternalURL);
                    csw.WriteField(rec.IsDeleted);
                    csw.WriteField(rec.OutTransferQuantity);
                    csw.WriteField(rec.OnOrderInTransitQuantity);
                    csw.WriteField(rec.ItemLink2ExternalURL);
                    csw.WriteField(rec.IsActive);
                    csw.WriteField(rec.UDF6);
                    csw.WriteField(rec.UDF7);
                    csw.WriteField(rec.UDF8);
                    csw.WriteField(rec.UDF9);
                    csw.WriteField(rec.UDF10);
                    csw.WriteField(rec.IsAllowOrderCostuom);
                    csw.WriteField(rec.BinNumber);

                    csw.NextRecord();
                }
            }
            write.Close();

            return filename;
        }
        //DashboardItemMinMaxList
        public string DashboardItemMinMaxList(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> lstIds = arrid.Select(t => Guid.Parse(t)).ToList();
            int TotalRecordCount = 0;
            IEnumerable<DashboardItem> DataFromDB = null;
            DashboardDAL obj = new DashboardDAL(SessionHelper.EnterPriseDBName);
            string Criteria = "minimum";
            if (ModuleName.ToLower().Contains("minimum"))
            {
                Criteria = "minimum";
            }
            else if (ModuleName.ToLower().Contains("maximum"))
            {
                Criteria = "maximum";
            }
            else if (ModuleName.ToLower().Contains("critical"))
            {
                Criteria = "critical";
            }
            DataFromDB = obj.GetItemsForItemDashboardDB(0, Int32.MaxValue, out TotalRecordCount, string.Empty, SortNameString, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds, true, null, null, 0, false, Criteria, false).ToList();

            List<ItemMasterDTO> lstItemMasterDTO = new List<ItemMasterDTO>();

            //if (!string.IsNullOrEmpty(ids))
            {
                lstItemMasterDTO = (from u in DataFromDB
                                    where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.GUID.ToString())
                                    select new ItemMasterDTO
                                    {
                                        ID = u.ID,
                                        ItemNumber = u.ItemNumber != null ? Convert.ToString(u.ItemNumber) : string.Empty,
                                        ManufacturerID = u.ManufacturerID,
                                        ManufacturerNumber = u.ManufacturerNumber != null ? Convert.ToString(u.ManufacturerNumber) : string.Empty,
                                        ManufacturerName = u.ManufacturerName != null ? Convert.ToString(u.ManufacturerName) : string.Empty,
                                        SupplierID = u.SupplierID,
                                        SupplierPartNo = u.SupplierPartNo != null ? Convert.ToString(u.SupplierPartNo) : string.Empty,
                                        SupplierName = u.SupplierName != null ? Convert.ToString(u.SupplierName) : string.Empty,
                                        UPC = u.UPC,
                                        UNSPSC = u.UNSPSC,
                                        Description = u.Description != null ? Convert.ToString(u.Description) : string.Empty,
                                        LongDescription = u.LongDescription != null ? Convert.ToString(u.LongDescription) : string.Empty,
                                        CategoryID = u.CategoryID,
                                        GLAccountID = u.GLAccountID,
                                        UOMID = u.UOMID,
                                        ItemType = u.ItemType.GetValueOrDefault(1),
                                        PricePerTerm = u.PricePerTerm,
                                        CostUOMID = u.CostUOMID,
                                        CostUOMName = u.CostUOMName != null ? Convert.ToString(u.CostUOMName) : string.Empty,
                                        DefaultReorderQuantity = u.DefaultReorderQuantity ?? 0,
                                        DefaultPullQuantity = u.DefaultPullQuantity ?? 0,
                                        Cost = u.Cost ?? 0,
                                        Markup = u.Markup ?? 0,
                                        SellPrice = u.SellPrice ?? 0,
                                        ExtendedCost = u.ExtendedCost ?? 0,
                                        AverageCost = u.AverageCost ?? 0,
                                        PerItemCost = u.PerItemCost ?? 0,
                                        LeadTimeInDays = u.LeadTimeInDays ?? 0,
                                        Link1 = u.Link1 != null ? Convert.ToString(u.Link1) : string.Empty,
                                        Link2 = u.Link2 != null ? Convert.ToString(u.Link2) : string.Empty,
                                        Trend = u.Trend.GetValueOrDefault(false),
                                        Taxable = u.Taxable.GetValueOrDefault(false),
                                        Consignment = u.Consignment.GetValueOrDefault(false),
                                        StagedQuantity = u.StagedQuantity,
                                        InTransitquantity = u.InTransitquantity ?? 0,
                                        OnOrderQuantity = u.OnOrderQuantity ?? 0,
                                        OnReturnQuantity = u.OnReturnQuantity ?? 0,
                                        OnTransferQuantity = u.OnTransferQuantity ?? 0,
                                        SuggestedOrderQuantity = u.SuggestedOrderQuantity ?? 0,
                                        SuggestedTransferQuantity = u.SuggestedTransferQuantity ?? 0,
                                        RequisitionedQuantity = u.RequisitionedQuantity,
                                        PackingQuantity = u.PackingQuantity ?? 0,
                                        AverageUsage = u.AverageUsage ?? 0,
                                        Turns = u.Turns ?? 0,
                                        OnHandQuantity = u.OnHandQuantity ?? 0,
                                        CriticalQuantity = u.CriticalQuantity,
                                        MinimumQuantity = u.MinimumQuantity,
                                        MaximumQuantity = u.MaximumQuantity,
                                        WeightPerPiece = u.WeightPerPiece,
                                        SerialNumberTracking = u.SerialNumberTracking.GetValueOrDefault(false),
                                        LotNumberTracking = u.LotNumberTracking.GetValueOrDefault(false),
                                        DateCodeTracking = u.DateCodeTracking.GetValueOrDefault(false),
                                        ItemUniqueNumber = u.ItemUniqueNumber != null ? Convert.ToString(u.ItemUniqueNumber) : string.Empty,
                                        IsPurchase = u.IsPurchase.GetValueOrDefault(false),
                                        IsTransfer = u.IsTransfer.GetValueOrDefault(false),
                                        DefaultLocation = u.DefaultLocation,
                                        DefaultLocationName = u.DefaultLocationName != null ? Convert.ToString(u.DefaultLocationName).Replace("[|EmptyStagingBin|]", "") : string.Empty,
                                        InventoryClassification = u.InventoryClassification,
                                        InventoryClassificationName = u.InventoryClassificationName != null ? Convert.ToString(u.InventoryClassificationName) : string.Empty,
                                        ImagePath = u.ImagePath,
                                        UDF1 = u.UDF1,
                                        UDF2 = u.UDF2,
                                        UDF3 = u.UDF3,
                                        UDF4 = u.UDF4,
                                        UDF5 = u.UDF5,
                                        GUID = u.GUID,
                                        Created = u.Created,
                                        Updated = u.Updated,
                                        CreatedBy = u.CreatedBy,
                                        LastUpdatedBy = u.LastUpdatedBy,
                                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                        CompanyID = u.CompanyID,
                                        Room = u.Room,
                                        CreatedByName = u.CreatedByName,
                                        UpdatedByName = u.UpdatedByName,
                                        IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                                        ItemTypeName = u.ItemTypeName != null ? Convert.ToString(u.ItemTypeName) : string.Empty,
                                        CategoryName = u.CategoryName != null ? Convert.ToString(u.CategoryName) : string.Empty,
                                        IsItemLevelMinMaxQtyRequired = (u.IsItemLevelMinMaxQtyRequired.HasValue ? u.IsItemLevelMinMaxQtyRequired : false),
                                        IsEnforceDefaultReorderQuantity = (u.IsEnforceDefaultReorderQuantity.HasValue ? u.IsEnforceDefaultReorderQuantity : false),
                                        IsBuildBreak = (u.IsBuildBreak.HasValue ? u.IsBuildBreak : false),
                                        BondedInventory = u.BondedInventory != null ? Convert.ToString(u.BondedInventory) : string.Empty,
                                        TrendingSetting = u.TrendingSetting,
                                        ItemImageExternalURL = u.ItemImageExternalURL != null ? Convert.ToString(u.ItemImageExternalURL) : string.Empty,
                                        ItemDocExternalURL = u.ItemDocExternalURL != null ? Convert.ToString(u.ItemDocExternalURL) : string.Empty,
                                        OutTransferQuantity = u.OutTransferQuantity,
                                        OnOrderInTransitQuantity = u.OnOrderInTransitQuantity ?? 0,
                                        ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                                        BinNumber = u.BinNumber,
                                        BlanketOrderNumber = u.BPONumber ?? string.Empty

                                    }).ToList();
            }



            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("ItemNumber");
            csw.WriteField("BinNumber");
            csw.WriteField("Manufacturer");
            csw.WriteField("ManufacturerNumber");
            csw.WriteField("SupplierName");
            csw.WriteField("SupplierPartNo");
            csw.WriteField("BlanketOrderNumber");
            csw.WriteField("UPC");
            csw.WriteField("UNSPSC");
            csw.WriteField("CriticalQuantity");
            csw.WriteField("MinimumQuantity");
            csw.WriteField("MaximumQuantity");
            csw.WriteField("OnHandQuantity");
            csw.WriteField("Description");
            csw.WriteField("LongDescription");
            csw.WriteField("CategoryName");
            csw.WriteField("CostUOM");
            csw.WriteField("DefaultReorderQuantity");
            csw.WriteField("DefaultPullQuantity");
            csw.WriteField("Cost");
            csw.WriteField("Markup");
            csw.WriteField("SellPrice");
            csw.WriteField("ExtendedCost");
            csw.WriteField("AverageCost");
            csw.WriteField("PerItemCost");
            csw.WriteField("LeadTimeInDays");
            csw.WriteField("Link1");
            csw.WriteField("Link2");
            csw.WriteField("Trend");
            csw.WriteField("Taxable");
            csw.WriteField("Consignment");
            csw.WriteField("StagedQuantity");
            csw.WriteField("InTransitquantity");
            csw.WriteField("OnOrderQuantity");
            csw.WriteField("OnReturnQuantity");
            csw.WriteField("OnTransferQuantity");
            csw.WriteField("SuggestedOrderQuantity");
            csw.WriteField("RequisitionedQuantity");
            csw.WriteField("AverageUsage");
            csw.WriteField("Turns");
            csw.WriteField("WeightPerPiece");
            csw.WriteField("ItemUniqueNumber");
            csw.WriteField("IsTransfer");
            csw.WriteField("IsPurchase");
            csw.WriteField("InventryLocation");
            csw.WriteField("InventoryClassification");
            csw.WriteField("SerialNumberTracking");
            csw.WriteField("LotNumberTracking");
            csw.WriteField("DateCodeTracking");
            csw.WriteField("ItemType");
            csw.WriteField("ImagePath");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("IsLotSerialExpiryCost");
            csw.WriteField("IsItemLevelMinMaxQtyRequired");
            csw.WriteField("TrendingSetting");
            csw.WriteField("EnforceDefaultPullQuantity");
            csw.WriteField("EnforceDefaultReorderQuantity");
            csw.WriteField("IsAutoInventoryClassification");
            csw.WriteField("IsBuildBreak");
            csw.WriteField("IsPackslipMandatoryAtReceive");
            csw.WriteField("ItemImageExternalURL");
            csw.WriteField("ItemDocExternalURL");
            csw.WriteField("Out Transfer Quantity");
            csw.WriteField("OnOrderInTransitQuantity");
            csw.WriteField("ItemLink2ExternalURL");
            csw.NextRecord();
            if (lstItemMasterDTO != null && lstItemMasterDTO.Count > 0)
            {


                foreach (ItemMasterDTO rec in ConvertExportDataList<ItemMasterDTO>(lstItemMasterDTO).ToList())
                {
                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.BinNumber);
                    csw.WriteField(rec.ManufacturerName);
                    csw.WriteField(rec.ManufacturerNumber);
                    csw.WriteField(rec.SupplierName);
                    csw.WriteField(rec.SupplierPartNo);
                    csw.WriteField(rec.BlanketOrderNumber);
                    csw.WriteField(rec.UPC);
                    csw.WriteField(rec.UNSPSC);
                    csw.WriteField(rec.CriticalQuantity);
                    csw.WriteField(rec.MinimumQuantity);
                    csw.WriteField(rec.MaximumQuantity);
                    csw.WriteField(rec.OnHandQuantity);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.LongDescription);
                    csw.WriteField(rec.CategoryName);
                    csw.WriteField(rec.CostUOMName);
                    csw.WriteField(rec.DefaultReorderQuantity);
                    csw.WriteField(rec.DefaultPullQuantity);
                    csw.WriteField(rec.Cost);
                    csw.WriteField(rec.Markup);
                    csw.WriteField(rec.SellPrice);
                    csw.WriteField(rec.ExtendedCost);
                    csw.WriteField(rec.AverageCost);
                    csw.WriteField(rec.PerItemCost);
                    csw.WriteField(rec.LeadTimeInDays);
                    csw.WriteField(rec.Link1);
                    csw.WriteField(rec.Link2);
                    csw.WriteField(rec.Trend);
                    csw.WriteField(rec.Taxable);
                    csw.WriteField(rec.Consignment);
                    csw.WriteField(rec.StagedQuantity ?? 0);
                    csw.WriteField(rec.InTransitquantity);
                    csw.WriteField(rec.OnOrderQuantity);
                    csw.WriteField(rec.OnReturnQuantity);
                    csw.WriteField(rec.OnTransferQuantity);
                    csw.WriteField(rec.SuggestedOrderQuantity);
                    csw.WriteField(rec.RequisitionedQuantity);
                    csw.WriteField(rec.AverageUsage);
                    csw.WriteField(rec.Turns);

                    csw.WriteField(rec.WeightPerPiece);
                    csw.WriteField(rec.ItemUniqueNumber);
                    csw.WriteField(rec.IsTransfer);
                    csw.WriteField(rec.IsPurchase);
                    csw.WriteField(rec.DefaultLocationName);
                    csw.WriteField(rec.InventoryClassificationName);
                    csw.WriteField(rec.SerialNumberTracking);
                    csw.WriteField(rec.LotNumberTracking);
                    csw.WriteField(rec.DateCodeTracking);
                    csw.WriteField(rec.ItemTypeName);
                    csw.WriteField(rec.ImagePath);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.WriteField(rec.IsLotSerialExpiryCost);
                    csw.WriteField(rec.IsItemLevelMinMaxQtyRequired);
                    csw.WriteField(GetTrendingSetting(rec.TrendingSetting));
                    csw.WriteField(rec.PullQtyScanOverride);
                    csw.WriteField(rec.IsEnforceDefaultReorderQuantity ?? false);
                    csw.WriteField(rec.IsAutoInventoryClassification);
                    csw.WriteField(rec.IsBuildBreak);
                    csw.WriteField(rec.IsPackslipMandatoryAtReceive);
                    csw.WriteField(rec.ItemImageExternalURL);
                    csw.WriteField(rec.ItemDocExternalURL);
                    csw.WriteField(rec.OutTransferQuantity);
                    csw.WriteField(rec.OnOrderInTransitQuantity);
                    csw.WriteField(rec.ItemLink2ExternalURL);
                    csw.NextRecord();
                }
            }
            write.Close();

            return filename;
        }

        public string MinMaxTuningList(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //List<Guid> lstIds = arrid.Select(t => Guid.Parse(t)).ToList();
            int TotalRecordCount = 0;
            int AutoMinPer = 0, AutoMaxPer = 0;
            List<MinMaxDataTableInfo> DataFromDB = null;
            DashboardDAL obj = new DashboardDAL(SessionHelper.EnterPriseDBName);
            var turnUsageLimit = !string.IsNullOrWhiteSpace(SessionHelper.NumberAvgDecimalPoints) ? Convert.ToInt32(SessionHelper.NumberAvgDecimalPoints) : 2;
            var numberDecimalDigits = !string.IsNullOrWhiteSpace(SessionHelper.NumberDecimalDigits) ? Convert.ToInt32(SessionHelper.NumberDecimalDigits) : 2;
            var currencyDecimalDigits = !string.IsNullOrWhiteSpace(SessionHelper.CurrencyDecimalDigits) ? Convert.ToInt32(SessionHelper.CurrencyDecimalDigits) : 0;

            if (HttpContext.Current.Session["SessionMinMaxTable"] != null)
            {
                DataFromDB = (List<MinMaxDataTableInfo>)HttpContext.Current.Session["SessionMinMaxTable"];
                TotalRecordCount = DataFromDB.Count;
            }
            else
            {
                //DataFromDB = objDashboardDAL.GetMinMaxTable(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, AutoMinPer, AutoMaxPer);
                DataFromDB = obj.GetMinMaxTable(0, 1000000, out TotalRecordCount, string.Empty, SortNameString, SessionHelper.RoomID, SessionHelper.CompanyID, AutoMinPer, AutoMaxPer);
                HttpContext.Current.Session["SessionMinMaxTable"] = DataFromDB;
            }

            if (arrid.Length > 0 && !string.IsNullOrEmpty(ids))
            {
                DataFromDB = DataFromDB.Where(x => arrid.Contains(x.GUID.ToString() + "~" + x.BinID.ToString())).ToList();
                //DataFromDB = DataFromDB.Where(x => arrid.Contains(x.GUID.ToString())).ToList();
                if (string.IsNullOrWhiteSpace(SortNameString))
                {
                    DataFromDB = DataFromDB.OrderBy(t => t.MinAnalysisOrderNumber).ThenBy(t => t.MaxAnalysisOrderNumber).ToList();
                }
                else
                {
                    DataFromDB = DataFromDB.OrderBy(SortNameString).ToList();
                }

            }
            else
            {
                DataFromDB = null;
            }


            ////if (!string.IsNullOrEmpty(ids))
            //{
            //    lstDBMasterDTO = (from u in DataFromDB
            //                        where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.GUID.ToString())
            //                        select new MinMaxDataTableInfo
            //                        {
            //                            ID = u.ID,
            //                            ItemNumber = u.ItemNumber != null ? Convert.ToString(u.ItemNumber) : string.Empty,
            //                            Cost = u.Cost,
            //                            OnHandQuantity = u.OnHandQuantity,
            //                            SupplierName = u.SupplierName ?? string.Empty,
            //                            CriticalQuantity = u.CriticalQuantity,
            //                            MinimumQuantity = u.MinimumQuantity,
            //                            MaximumQuantity = u.MaximumQuantity,
            //                            BinNumber = u.BinNumber ?? string.Empty,
            //                            MinAnalysis = u.MinAnalysis,
            //                            MaxAnalysis = u.MaxAnalysis,
            //                            IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
            //                            Description = u.Description ?? string.Empty,
            //                            InventoryClassification = u.InventoryClassification ?? string.Empty,
            //                            Category = u.Category ?? string.Empty,
            //                            ItemInventoryValue = u.ItemInventoryValue,

            //                            AverageCost = u.AverageCost,
            //                            PullCost = u.PullCost,

            //                            PullQuantity = u.PullQuantity,
            //                            AvgDailyPullValueUsage = u.AvgDailyPullValueUsage,

            //                            AvgDailyPullUsage = u.AvgDailyPullUsage,
            //                            AvgDailyOrderUsage = u.AvgDailyOrderUsage,

            //                            PullValueTurn = u.PullValueTurn,
            //                            PullTurn = u.PullTurn,
            //                            OrderTurn = u.OrderTurn,

            //                            CalulatedMinimum = u.CalulatedMinimum,
            //                            AbsoluteMinPerCent = u.AbsoluteMinPerCent,
            //                            AutoCurrentMin = u.AutoCurrentMin,
            //                            AbsValDifCurrCalcMinimum = u.AbsValDifCurrCalcMinimum,
            //                            AutoCurrentMinPercent = u.AutoCurrentMinPercent,
            //                            CalulatedMaximum = u.CalulatedMaximum,
            //                            AbsoluteMAXPerCent = u.AbsoluteMAXPerCent,
            //                            AutoCurrentMax = u.AutoCurrentMax,
            //                            AbsValDifCurrCalcMax = u.AbsValDifCurrCalcMax,
            //                            AutoCurrentMaxPercent = u.AutoCurrentMaxPercent,

            //                        }).ToList();
            //}



            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);

            /*
            csw.WriteField("Min Analysis");
            csw.WriteField("Max Analysis");
            csw.WriteField("Item Number");
            csw.WriteField("Is item Level MinMax");
            csw.WriteField("Description");
            csw.WriteField("Inventory Classification");
            csw.WriteField("Category");
            csw.WriteField("Supplier Name");
            csw.WriteField("Supplier Part No");
            csw.WriteField("Manufacturer Name");
            csw.WriteField("Manufacturer Number");
            csw.WriteField("Location");
            csw.WriteField("Available Qty");
            csw.WriteField("Inventory Value");
            csw.WriteField("Average Cost");
            csw.WriteField("Period Pull Value Usage");
            csw.WriteField("Avg Daily Pull Value Usage");
            csw.WriteField("Period Pull Usage");
            csw.WriteField("Avg Daily Pull Usage");
            csw.WriteField("Qty Until Order");
            csw.WriteField("No Of Days Until Order");
            csw.WriteField("Demand Planning Qty To Order");
            csw.WriteField("Period Orders Usage");
            csw.WriteField("Avg Daily Orders Usage");
            csw.WriteField("Pull Value Turns");
            csw.WriteField("Pull Turns");
            csw.WriteField("Order Turns");
            csw.WriteField("Critical");
            csw.WriteField("Current Minimum");
            csw.WriteField("Calculated Minimum");
            csw.WriteField("Abs value diff curr calc Minimum");
            csw.WriteField("Trial Calculated Minimum");
            csw.WriteField("Abs Val Dif Curr Calc Minimum");
            csw.WriteField("Auto current min Percentage");
            csw.WriteField("Current Maximum");
            csw.WriteField("Calculated Maximum");
            csw.WriteField("AbsvaluediffcurrcalcMaximum");
            csw.WriteField("TrialCalculatedMaximum");
            csw.WriteField("AbsValDifCurrCalcMaximum");
            csw.WriteField("AutocurrentmaxPercentage");
            csw.WriteField("DefaultReorderQuantity");
            csw.WriteField("CostUOMValue");
            csw.WriteField("IsActive");
            csw.WriteField("DateCreated");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("UDF6");
            csw.WriteField("UDF7");
            csw.WriteField("UDF8");
            csw.WriteField("UDF9");
            csw.WriteField("UDF10");
            */

            csw.WriteField(ResInventoryAnalysis.MinAnalysis);
            csw.WriteField(ResInventoryAnalysis.MaxAnalysis);
            csw.WriteField(ResInventoryAnalysis.ItemNumber);
            csw.WriteField(ResInventoryAnalysis.IsActive);
            csw.WriteField(ResInventoryAnalysis.DefaultReorderQuantity);
            csw.WriteField(ResInventoryAnalysis.CostUOMValue);
            csw.WriteField(ResInventoryAnalysis.DateCreated);
            csw.WriteField(ResInventoryAnalysis.IsitemLevelMinMax);
            csw.WriteField(ResInventoryAnalysis.Description);
            csw.WriteField(ResInventoryAnalysis.InventoryClassification);
            csw.WriteField(ResInventoryAnalysis.Category);
            csw.WriteField(ResInventoryAnalysis.SupplierName);
            csw.WriteField(ResInventoryAnalysis.SupplierPartNo);
            csw.WriteField(ResInventoryAnalysis.Manufacturer);
            csw.WriteField(ResInventoryAnalysis.ManufacturerNumber);
            csw.WriteField(ResInventoryAnalysis.Location);
            csw.WriteField(ResInventoryAnalysis.AvailableQty);
            csw.WriteField(ResInventoryAnalysis.InventoryValue);
            csw.WriteField(ResInventoryAnalysis.AverageCost);
            csw.WriteField(ResInventoryAnalysis.PeriodPullValueUsage);
            csw.WriteField(ResInventoryAnalysis.AvgDailyPullValueUsage);
            csw.WriteField(ResInventoryAnalysis.PeriodPullUsage);
            csw.WriteField(ResInventoryAnalysis.AvgDailyPullUsage);
            csw.WriteField(ResInventoryAnalysis.QtyUntilOrder);
            csw.WriteField(ResInventoryAnalysis.NoOfDaysUntilOrder);
            csw.WriteField(ResInventoryAnalysis.LeadTimeInDays);
            csw.WriteField(ResInventoryAnalysis.DateofOrder);
            csw.WriteField(ResInventoryAnalysis.DemandPlanningQtyToOrder);
            csw.WriteField(ResInventoryAnalysis.PeriodOrdersUsage);
            csw.WriteField(ResInventoryAnalysis.AvgDailyOrdersUsage);
            csw.WriteField(ResInventoryAnalysis.PullValueTurns);
            csw.WriteField(ResInventoryAnalysis.PullTurns);
            csw.WriteField(ResInventoryAnalysis.OrderTurns);
            csw.WriteField(ResInventoryAnalysis.Critical);
            csw.WriteField(ResInventoryAnalysis.CurrentMinimum);
            csw.WriteField(ResInventoryAnalysis.CalculatedMinimum);
            csw.WriteField(HttpUtility.HtmlDecode(ResInventoryAnalysis.AbsvaluediffcurrcalcMinimum));
            csw.WriteField(ResInventoryAnalysis.TrialCalculatedMinimum);
            csw.WriteField(HttpUtility.HtmlDecode(ResInventoryAnalysis.AbsValDifCurrCalcMinimum));
            csw.WriteField(ResInventoryAnalysis.AutocurrentminPercentage);
            csw.WriteField(ResInventoryAnalysis.OptimizedInvValueUsesQOHofAvgCalcdMinMax);
            csw.WriteField(ResInventoryAnalysis.OptimizedInvValueChange);
            csw.WriteField(ResInventoryAnalysis.TrialCalcInvValueUsesQOHofAvgTrialMinMax);
            csw.WriteField(ResInventoryAnalysis.TrialInvValueChange);
            csw.WriteField(ResInventoryAnalysis.CurrentMaximum);
            csw.WriteField(ResInventoryAnalysis.CalculatedMaximum);
            csw.WriteField(HttpUtility.HtmlDecode(ResInventoryAnalysis.AbsvaluediffcurrcalcMaximum));
            csw.WriteField(ResInventoryAnalysis.TrialCalculatedMaximum);
            csw.WriteField(HttpUtility.HtmlDecode(ResInventoryAnalysis.AbsValDifCurrCalcMaximum));
            csw.WriteField(ResInventoryAnalysis.AutocurrentmaxPercentage);
            csw.WriteField(eTurns.DTO.ResItemMaster.UDF1);
            csw.WriteField(eTurns.DTO.ResItemMaster.UDF2);
            csw.WriteField(eTurns.DTO.ResItemMaster.UDF3);
            csw.WriteField(eTurns.DTO.ResItemMaster.UDF4);
            csw.WriteField(eTurns.DTO.ResItemMaster.UDF5);
            csw.WriteField(eTurns.DTO.ResItemMaster.UDF6);
            csw.WriteField(eTurns.DTO.ResItemMaster.UDF7);
            csw.WriteField(eTurns.DTO.ResItemMaster.UDF8);
            csw.WriteField(eTurns.DTO.ResItemMaster.UDF9);
            csw.WriteField(eTurns.DTO.ResItemMaster.UDF10);


            csw.NextRecord();

            if (DataFromDB != null && DataFromDB.Count > 0)
            {


                foreach (MinMaxDataTableInfo rec in ConvertExportDataList<MinMaxDataTableInfo>(DataFromDB).ToList())
                {

                    /*
                      csw.WriteField(rec.MinAnalysis);
                      csw.WriteField(rec.MaxAnalysis);
                      csw.WriteField(rec.ItemNumber);
                      csw.WriteField(rec.IsItemLevelMinMaxQtyRequired);
                      csw.WriteField(rec.Description);
                      csw.WriteField(rec.InventoryClassification);
                      csw.WriteField(rec.Category);
                      csw.WriteField(rec.SupplierName);
                      csw.WriteField(rec.SupplierPartNo);
                      csw.WriteField(rec.Manufacturer);
                      csw.WriteField(rec.ManufacturerNumber);
                      csw.WriteField(rec.BinNumber);
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.OnHandQuantity, numberDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + currencyDecimalDigits + "}", Math.Round(rec.ItemInventoryValue, currencyDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + currencyDecimalDigits + "}", Math.Round(rec.AverageCost, currencyDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.PullCost, turnUsageLimit)));
                      csw.WriteField(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.AvgDailyPullValueUsage, turnUsageLimit)));
                      csw.WriteField(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.PullQuantity, turnUsageLimit)));
                      csw.WriteField(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.AvgDailyPullUsage, turnUsageLimit)));
                      csw.WriteField(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.QtyUntilOrder, turnUsageLimit)));
                      csw.WriteField(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.NoOfDaysUntilOrder, turnUsageLimit)));
                      csw.WriteField(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.DemandPlanningQtyToOrder, turnUsageLimit)));
                      csw.WriteField(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.OrderedQuantity, turnUsageLimit)));
                      csw.WriteField(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.AvgDailyOrderUsage, turnUsageLimit)));
                      csw.WriteField(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.PullValueTurn, turnUsageLimit)));
                      csw.WriteField(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.PullTurn, turnUsageLimit)));
                      csw.WriteField(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.OrderTurn, turnUsageLimit)));
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.CriticalQuantity, numberDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.MinimumQuantity, numberDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.CalulatedMinimum, numberDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AbsoluteMinPerCent, numberDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AutoCurrentMin, numberDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AbsValDifCurrCalcMinimum, numberDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AutoCurrentMinPercent, numberDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.MaximumQuantity, numberDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.CalulatedMaximum, numberDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AbsoluteMAXPerCent, numberDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AutoCurrentMax, numberDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AbsValDifCurrCalcMax, numberDecimalDigits)));
                      csw.WriteField(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AutoCurrentMaxPercent, numberDecimalDigits)));
                      csw.WriteField(rec.DefaultReorderQuantity);
                      csw.WriteField(rec.CostUOMValue);
                      csw.WriteField(rec.IsActive);
                      csw.WriteField(rec.DateCreated);
                      csw.WriteField(rec.UDF1);
                      csw.WriteField(rec.UDF2);
                      csw.WriteField(rec.UDF3);
                      csw.WriteField(rec.UDF4);
                      csw.WriteField(rec.UDF5);
                      csw.WriteField(rec.UDF6);
                      csw.WriteField(rec.UDF7);
                      csw.WriteField(rec.UDF8);
                      csw.WriteField(rec.UDF9);
                      csw.WriteField(rec.UDF10);
                   */


                    csw.WriteField(rec.MinAnalysis);
                    csw.WriteField(rec.MaxAnalysis);
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.IsActive);
                    csw.WriteField(rec.DefaultReorderQuantity);
                    csw.WriteField(rec.CostUOMValue);
                    csw.WriteField(rec.DateCreated);
                    csw.WriteField((rec.IsItemLevelMinMaxQtyRequired ?? false) ? "Yes" : "No");
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.InventoryClassification);
                    csw.WriteField(rec.Category);
                    csw.WriteField(rec.SupplierName);
                    csw.WriteField(rec.SupplierPartNo);
                    csw.WriteField(rec.Manufacturer);
                    csw.WriteField(rec.ManufacturerNumber);
                    csw.WriteField(rec.BinNumber);
                    csw.WriteField(rec.OnHandQuantity);
                    csw.WriteField(rec.ItemInventoryValue);
                    csw.WriteField(rec.AverageCost);
                    csw.WriteField(rec.PullCost + rec.TransferCost);
                    csw.WriteField(rec.AvgDailyPullValueUsage);
                    csw.WriteField(rec.PullQuantity + rec.TransferQuantity);
                    csw.WriteField(rec.AvgDailyPullUsage);
                    csw.WriteField(rec.QtyUntilOrder);
                    csw.WriteField(rec.NoOfDaysUntilOrder);
                    csw.WriteField(rec.LeadTimeInDays);
                    csw.WriteField(rec.DateofOrder);
                    csw.WriteField(rec.DemandPlanningQtyToOrder);
                    csw.WriteField(rec.OrderedQuantity);
                    csw.WriteField(rec.AvgDailyOrderUsage);
                    csw.WriteField(rec.PullValueTurn);
                    csw.WriteField(rec.PullTurn);
                    csw.WriteField(rec.OrderTurn);
                    csw.WriteField(rec.CriticalQuantity);
                    csw.WriteField(rec.MinimumQuantity);
                    csw.WriteField(rec.CalulatedMinimum);
                    csw.WriteField(rec.AbsoluteMinPerCent);
                    csw.WriteField(rec.AutoCurrentMin);
                    csw.WriteField(rec.AbsValDifCurrCalcMinimum);
                    csw.WriteField(rec.AutoCurrentMinPercent);
                    csw.WriteField(rec.OptimizedInvValueUsesQOHofAvgCalcdMinMax);
                    csw.WriteField(rec.OptimizedInvValueChange);
                    csw.WriteField(rec.TrialCalcInvValueUsesQOHofAvgTrialMinMax);
                    csw.WriteField(rec.TrialInvValueChange);
                    csw.WriteField(rec.MaximumQuantity);
                    csw.WriteField(rec.CalulatedMaximum);
                    csw.WriteField(rec.AbsoluteMAXPerCent);
                    csw.WriteField(rec.AutoCurrentMax);
                    csw.WriteField(rec.AbsValDifCurrCalcMax);
                    csw.WriteField(rec.AutoCurrentMaxPercent);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.WriteField(rec.UDF6);
                    csw.WriteField(rec.UDF7);
                    csw.WriteField(rec.UDF8);
                    csw.WriteField(rec.UDF9);
                    csw.WriteField(rec.UDF10);

                    csw.NextRecord();
                }
            }
            write.Close();

            return filename;
        }

        public string TuningList(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> lstIds = arrid.Select(t => Guid.Parse(t)).ToList();
            int TotalRecordCount = 0;
            DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            List<TurnsDataTableInfo> DataFromDB;

            if (arrid.Length > 0 && !string.IsNullOrEmpty(ids))
            {
                string Guids = string.Join(",", lstIds);
                DataFromDB = objDashboardDAL.GetTurnsTableForExport(0, int.MaxValue, out TotalRecordCount, "", SortNameString, SessionHelper.RoomID, SessionHelper.CompanyID, Guids);
            }
            else
            {
                DataFromDB = null;
            }

            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);




            csw.WriteField(ResInventoryAnalysis.ItemNumber);
            csw.WriteField(ResInventoryAnalysis.IsitemLevelMinMax);
            csw.WriteField(ResInventoryAnalysis.Description);
            csw.WriteField(ResInventoryAnalysis.InventoryClassification);
            csw.WriteField(ResInventoryAnalysis.Category);
            csw.WriteField(ResInventoryAnalysis.SupplierName);
            csw.WriteField(ResInventoryAnalysis.Location);
            csw.WriteField(ResInventoryAnalysis.AvailableQty);
            csw.WriteField(ResInventoryAnalysis.InventoryValue);
            csw.WriteField(ResInventoryAnalysis.AverageCost);
            csw.WriteField(ResInventoryAnalysis.PeriodPullValueUsage);
            csw.WriteField(ResInventoryAnalysis.AvgDailyPullValueUsage);
            csw.WriteField(ResInventoryAnalysis.PeriodPullUsage);
            csw.WriteField(ResInventoryAnalysis.AvgDailyPullUsage);
            csw.WriteField(ResInventoryAnalysis.PeriodOrdersUsage);
            csw.WriteField(ResInventoryAnalysis.AvgDailyOrdersUsage);
            csw.WriteField(ResInventoryAnalysis.PullValueTurns);
            csw.WriteField(ResInventoryAnalysis.PullTurns);
            csw.WriteField(ResInventoryAnalysis.OrderTurns);


            csw.NextRecord();

            if (DataFromDB != null && DataFromDB.Count > 0)
            {
                foreach (TurnsDataTableInfo rec in ConvertExportDataList<TurnsDataTableInfo>(DataFromDB).ToList())
                {
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.IsItemLevelMinMaxQtyRequired);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.InventoryClassification);
                    csw.WriteField(rec.Category);
                    csw.WriteField(rec.SupplierName);
                    csw.WriteField(rec.BinNumber);
                    csw.WriteField(rec.OnHandQuantity);
                    csw.WriteField(rec.ItemInventoryValue);
                    csw.WriteField(rec.AverageCost);
                    csw.WriteField(rec.PullCost);
                    csw.WriteField(rec.AvgDailyPullValueUsage);
                    csw.WriteField(rec.PullQuantity);
                    csw.WriteField(rec.AvgDailyPullUsage);
                    csw.WriteField(rec.OrderedQuantity);
                    csw.WriteField(rec.AvgDailyOrderUsage);
                    csw.WriteField(rec.PullValueTurn);
                    csw.WriteField(rec.PullTurn);
                    csw.WriteField(rec.OrderTurn);

                    csw.NextRecord();
                }
            }
            write.Close();

            return filename;
        }


        public string QuickListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            QuickListDAL obj = new QuickListDAL(SessionHelper.EnterPriseDBName);
            List<QuickListLineItemDetailDTO> lstQuickListLineItemDetailDTO = obj.GetAllQuicklistWiseLineItem(SessionHelper.CompanyID, SessionHelper.RoomID, false, false, ids, SessionHelper.UserSupplierIds).OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* Name");
            csw.WriteField("* Type");
            csw.WriteField("Comment");
            csw.WriteField("* ItemNumber");
            csw.WriteField("BinNumber");
            csw.WriteField("* Quantity");
            csw.WriteField("* ConsignedQuantity");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.NextRecord();
            if (lstQuickListLineItemDetailDTO != null && lstQuickListLineItemDetailDTO.Count > 0)
            {
                //csw.WriteHeader<CategoryMasterDTO>();


                foreach (QuickListLineItemDetailDTO rec in ConvertExportDataList<QuickListLineItemDetailDTO>(lstQuickListLineItemDetailDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.Name);
                    csw.WriteField(((QuickListType)rec.Type).ToString());
                    csw.WriteField(rec.Comment);
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.BinNumber);
                    csw.WriteField(rec.Quantity);
                    csw.WriteField(rec.ConsignedQuantity);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string OrderMasterCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            OrderDetailsDAL objOrdDtlDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            int orderType = (ModuleName.ToLower() == "ordermaster") ? (int)OrderType.Order : (int)OrderType.RuturnOrder;
            List<OrderLineItemDetailDTO> lstOrderLineItemDetailDTO = objOrdDtlDAL.GetOrderDetailExport(SessionHelper.RoomID, SessionHelper.CompanyID, orderType).OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            #region Order csv

            if (orderType == (int)OrderType.Order)
            {
                csw.WriteField("ID");
                csw.WriteField("*Supplier");
                csw.WriteField("*OrderNumber");
                csw.WriteField("ReleaseNumber");
                csw.WriteField("*RequiredDate");
                csw.WriteField("*OrderStatus");
                csw.WriteField("StagingName");
                csw.WriteField("OrderComment");
                csw.WriteField("CustomerName");
                csw.WriteField("PackSlipNumber");
                csw.WriteField("ShippingTrackNumber");
                csw.WriteField("OrderUDF1");
                csw.WriteField("OrderUDF2");
                csw.WriteField("OrderUDF3");
                csw.WriteField("OrderUDF4");
                csw.WriteField("OrderUDF5");
                csw.WriteField("ShipVia");
                csw.WriteField("*OrderType");
                csw.WriteField("ShippingVendor");
                //csw.WriteField("AccountNumber");
                csw.WriteField("SupplierAccount");
                csw.WriteField("ItemNumber");
                csw.WriteField("Bin");
                csw.WriteField("RequestedQty");
                csw.WriteField("ReceivedQty");
                csw.WriteField("ASNNumber");
                csw.WriteField("ApprovedQty");
                csw.WriteField("InTransitQty");
                csw.WriteField("IsCloseItem");
                csw.WriteField("LineNumber");
                csw.WriteField("ControlNumber");
                csw.WriteField("ItemComment");
                csw.WriteField("LineItemUDF1");
                csw.WriteField("LineItemUDF2");
                csw.WriteField("LineItemUDF3");
                csw.WriteField("LineItemUDF4");
                csw.WriteField("LineItemUDF5");
                csw.WriteField("SalesOrder");

                csw.NextRecord();
                if (lstOrderLineItemDetailDTO != null && lstOrderLineItemDetailDTO.Count > 0)
                {
                    foreach (OrderLineItemDetailDTO rec in ConvertExportDataList<OrderLineItemDetailDTO>(lstOrderLineItemDetailDTO).ToList())
                    {
                        csw.WriteField(rec.ID);
                        csw.WriteField(rec.Supplier);
                        csw.WriteField(rec.OrderNumber);
                        csw.WriteField(rec.ReleaseNumber);
                        csw.WriteField(Convert.ToDateTime(rec.RequiredDate).ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture));
                        csw.WriteField(rec.OrderStatus);
                        csw.WriteField(rec.StagingName);
                        csw.WriteField(Convert.ToString(rec.OrderComment));
                        csw.WriteField(rec.CustomerName);
                        csw.WriteField(rec.PackSlipNumber);
                        csw.WriteField(rec.ShippingTrackNumber);
                        csw.WriteField(rec.OrderUDF1);
                        csw.WriteField(rec.OrderUDF2);
                        csw.WriteField(rec.OrderUDF3);
                        csw.WriteField(rec.OrderUDF4);
                        csw.WriteField(rec.OrderUDF5);
                        csw.WriteField(rec.ShipVia);
                        csw.WriteField(rec.OrderType);
                        csw.WriteField(rec.ShippingVendor);
                        //csw.WriteField(rec.AccountNumber);
                        csw.WriteField(rec.SupplierAccount);
                        csw.WriteField(rec.ItemNumber);
                        csw.WriteField(rec.Bin);
                        if (rec.IsAllowOrderCostuom)
                            csw.WriteField(rec.RequestedQtyUOM);
                        else
                            csw.WriteField(rec.RequestedQty);
                        if (rec.IsAllowOrderCostuom)
                            csw.WriteField(rec.ReceivedQtyUOM);
                        else
                            csw.WriteField(rec.ReceivedQty);
                        csw.WriteField(rec.ASNNumber);
                        if (rec.IsAllowOrderCostuom)
                            csw.WriteField(rec.ApprovedQtyUOM);
                        else
                            csw.WriteField(rec.ApprovedQty);
                        if (rec.IsAllowOrderCostuom)
                            csw.WriteField(rec.InTransitQtyUOM);
                        else
                            csw.WriteField(rec.InTransitQty);
                        csw.WriteField(rec.IsCloseItem);
                        csw.WriteField(rec.LineNumber);
                        csw.WriteField(rec.ControlNumber);
                        csw.WriteField(rec.ItemComment);
                        csw.WriteField(rec.LineItemUDF1);
                        csw.WriteField(rec.LineItemUDF2);
                        csw.WriteField(rec.LineItemUDF3);
                        csw.WriteField(rec.LineItemUDF4);
                        csw.WriteField(rec.LineItemUDF5);
                        csw.WriteField(rec.SalesOrder);
                        csw.NextRecord();

                    }
                }
            }
            #endregion
            #region return Order
            else if (orderType == (int)OrderType.RuturnOrder)
            {
                csw.WriteField("ID");
                csw.WriteField("*Supplier");
                csw.WriteField("*ReturnOrderNumber");
                csw.WriteField("*ReturnDate");
                csw.WriteField("ReturnOrderComment");
                csw.WriteField("PackSlipNumber");
                csw.WriteField("ShippingTrackNumber");
                csw.WriteField("ReturnOrderUDF1");
                csw.WriteField("ReturnOrderUDF2");
                csw.WriteField("ReturnOrderUDF3");
                csw.WriteField("ReturnOrderUDF4");
                csw.WriteField("ReturnOrderUDF5");
                csw.WriteField("ShipVia");
                csw.WriteField("ShippingVendor");
                //csw.WriteField("AccountNumber");
                csw.WriteField("SupplierAccount");
                csw.WriteField("ItemNumber");
                csw.WriteField("Bin");
                csw.WriteField("RequestedReturnedQty");
                csw.WriteField("ASNNumber");
                csw.WriteField("ItemComment");
                csw.WriteField("LineItemUDF1");
                csw.WriteField("LineItemUDF2");
                csw.WriteField("LineItemUDF3");
                csw.WriteField("LineItemUDF4");
                csw.WriteField("LineItemUDF5");
                csw.WriteField("ReturnOrderCost");
                csw.NextRecord();
                if (lstOrderLineItemDetailDTO != null && lstOrderLineItemDetailDTO.Count > 0)
                {
                    foreach (OrderLineItemDetailDTO rec in ConvertExportDataList<OrderLineItemDetailDTO>(lstOrderLineItemDetailDTO).ToList())
                    {
                        csw.WriteField(rec.ID);
                        csw.WriteField(rec.Supplier);
                        csw.WriteField(rec.OrderNumber);
                        csw.WriteField(Convert.ToDateTime(rec.RequiredDate).ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture));
                        csw.WriteField(rec.OrderComment);
                        csw.WriteField(rec.PackSlipNumber);
                        csw.WriteField(rec.ShippingTrackNumber);
                        csw.WriteField(rec.OrderUDF1);
                        csw.WriteField(rec.OrderUDF2);
                        csw.WriteField(rec.OrderUDF3);
                        csw.WriteField(rec.OrderUDF4);
                        csw.WriteField(rec.OrderUDF5);
                        csw.WriteField(rec.ShipVia);
                        csw.WriteField(rec.ShippingVendor);
                        //csw.WriteField(rec.AccountNumber);
                        csw.WriteField(rec.SupplierAccount);
                        csw.WriteField(rec.ItemNumber);
                        csw.WriteField(rec.Bin);
                        //if (rec.IsAllowOrderCostuom)
                        //    csw.WriteField(rec.RequestedQtyUOM);
                        //else
                        //    csw.WriteField(rec.RequestedQty);
                        if (rec.IsAllowOrderCostuom)
                            csw.WriteField(rec.ReceivedQtyUOM);
                        else
                            csw.WriteField(rec.ReceivedQty);
                        csw.WriteField(rec.ASNNumber);
                        //if (rec.IsAllowOrderCostuom)
                        //    csw.WriteField(rec.ApprovedQtyUOM);
                        //else
                        //    csw.WriteField(rec.ApprovedQty);
                        //if (rec.IsAllowOrderCostuom)
                        //    csw.WriteField(rec.InTransitQtyUOM);
                        //else
                        //    csw.WriteField(rec.InTransitQty);
                        //csw.WriteField(rec.IsCloseItem);
                        //csw.WriteField(rec.LineNumber);
                        //csw.WriteField(rec.ControlNumber);
                        csw.WriteField(rec.ItemComment);
                        csw.WriteField(rec.LineItemUDF1);
                        csw.WriteField(rec.LineItemUDF2);
                        csw.WriteField(rec.LineItemUDF3);
                        csw.WriteField(rec.LineItemUDF4);
                        csw.WriteField(rec.LineItemUDF5);
                        csw.WriteField(rec.Cost);
                        csw.NextRecord();

                    }
                }

            }

            #endregion
            write.Close();

            return filename;

        }
        public string QuoteMasterCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            QuoteDetailDAL objOrdDtlDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
            List<QuoteLineItemDetailDTO> lstQuoteLineItemDetailDTO = objOrdDtlDAL.GetQuoteDetailExport(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("ID");
            csw.WriteField("* SupplierName");
            csw.WriteField("* QuoteNumber");
            csw.WriteField("ReleaseNumber");
            csw.WriteField("* RequiredDate");
            csw.WriteField("QuoteStatus");
            csw.WriteField("QuoteComment");
            csw.WriteField("QuoteUDF1");
            csw.WriteField("QuoteUDF2");
            csw.WriteField("QuoteUDF3");
            csw.WriteField("QuoteUDF4");
            csw.WriteField("QuoteUDF5");
            csw.WriteField("ItemNumber");
            csw.WriteField("Bin");
            csw.WriteField("RequestedQty");
            csw.WriteField("ASNNumber");
            csw.WriteField("ApprovedQty");
            csw.WriteField("InTransitQty");
            csw.WriteField("IsCloseItem");
            csw.WriteField("LineNumber");
            csw.WriteField("ControlNumber");
            csw.WriteField("ItemComment");
            csw.WriteField("LineItemUDF1");
            csw.WriteField("LineItemUDF2");
            csw.WriteField("LineItemUDF3");
            csw.WriteField("LineItemUDF4");
            csw.WriteField("LineItemUDF5");

            csw.NextRecord();
            if (lstQuoteLineItemDetailDTO != null && lstQuoteLineItemDetailDTO.Count > 0)
            {
                foreach (QuoteLineItemDetailDTO rec in ConvertExportDataList<QuoteLineItemDetailDTO>(lstQuoteLineItemDetailDTO).ToList())
                {
                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.SupplierName);
                    csw.WriteField(rec.QuoteNumber);
                    csw.WriteField(rec.ReleaseNumber);
                    csw.WriteField(Convert.ToDateTime(rec.RequiredDate).ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture));
                    csw.WriteField(rec.QuoteStatus);
                    csw.WriteField(rec.QuoteComment);
                    csw.WriteField(rec.QuoteUDF1);
                    csw.WriteField(rec.QuoteUDF2);
                    csw.WriteField(rec.QuoteUDF3);
                    csw.WriteField(rec.QuoteUDF4);
                    csw.WriteField(rec.QuoteUDF5);
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.Bin);
                    if (rec.IsAllowOrderCostuom)
                        csw.WriteField(rec.RequestedQtyUOM);
                    else
                        csw.WriteField(rec.RequestedQty);
                    csw.WriteField(rec.ASNNumber);
                    if (rec.IsAllowOrderCostuom)
                        csw.WriteField(rec.ApprovedQtyUOM);
                    else
                        csw.WriteField(rec.ApprovedQty);
                    if (rec.IsAllowOrderCostuom)
                        csw.WriteField(rec.InTransitQtyUOM);
                    else
                        csw.WriteField(rec.InTransitQty);
                    csw.WriteField(rec.IsCloseItem);
                    csw.WriteField(rec.LineNumber);
                    csw.WriteField(rec.ControlNumber);
                    csw.WriteField(rec.ItemComment);
                    csw.WriteField(rec.LineItemUDF1);
                    csw.WriteField(rec.LineItemUDF2);
                    csw.WriteField(rec.LineItemUDF3);
                    csw.WriteField(rec.LineItemUDF4);
                    csw.WriteField(rec.LineItemUDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string BOMItemMasterListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = "BOM_" + SessionHelper.CompanyID + "_" + DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy HH:mm:ss").Replace("/", "").Replace(":", "") + ModuleName + ".csv";
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            BOMItemMasterDAL obj = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);

            IEnumerable<BOMItemDTO> DataFromDB = obj.CompanyBOMItems(SessionHelper.CompanyID, false, false).OrderBy(SortNameString);

            List<ItemMasterDTO> lstItemMasterDTO = new List<ItemMasterDTO>();

            //  if (!string.IsNullOrEmpty(ids))
            {
                lstItemMasterDTO = (from u in DataFromDB
                                    where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.GUID.ToString())
                                    select new ItemMasterDTO
                                    {
                                        ID = u.ID,
                                        ItemNumber = u.ItemNumber != null ? Convert.ToString(u.ItemNumber) : string.Empty,
                                        ManufacturerID = u.ManufacturerID,
                                        ManufacturerNumber = u.ManufacturerNumber != null ? Convert.ToString(u.ManufacturerNumber) : string.Empty,
                                        ManufacturerName = u.ManufacturerName != null ? Convert.ToString(u.ManufacturerName) : string.Empty,
                                        SupplierID = u.SupplierID,
                                        SupplierPartNo = u.SupplierPartNo,
                                        SupplierName = u.SupplierName != null ? Convert.ToString(u.SupplierName) : string.Empty,
                                        UPC = u.UPC,
                                        UNSPSC = u.UNSPSC,
                                        Description = u.Description != null ? Convert.ToString(u.Description) : string.Empty,
                                        CategoryID = u.CategoryID,
                                        GLAccountID = u.GLAccountID,
                                        UOMID = u.UOMID,
                                        LeadTimeInDays = u.LeadTimeInDays,
                                        Taxable = u.Taxable,
                                        Consignment = u.Consignment,
                                        AverageUsage = u.AverageUsage,
                                        ItemUniqueNumber = u.ItemUniqueNumber != null ? Convert.ToString(u.ItemUniqueNumber) : string.Empty,
                                        IsPurchase = u.IsPurchase,
                                        IsTransfer = u.IsTransfer,
                                        DefaultLocation = u.DefaultLocation,
                                        DefaultLocationName = u.DefaultLocationName != null ? Convert.ToString(u.DefaultLocationName).Replace("[|EmptyStagingBin|]", "") : string.Empty,
                                        InventoryClassification = u.InventoryClassification,
                                        InventoryClassificationName = u.InventoryClassificationName != null ? Convert.ToString(u.InventoryClassificationName) : string.Empty,
                                        SerialNumberTracking = u.SerialNumberTracking,
                                        LotNumberTracking = u.LotNumberTracking,
                                        DateCodeTracking = u.DateCodeTracking,
                                        ItemType = u.ItemType,
                                        ImagePath = u.ImagePath,
                                        UDF1 = u.UDF1,
                                        UDF2 = u.UDF2,
                                        UDF3 = u.UDF3,
                                        UDF4 = u.UDF4,
                                        UDF5 = u.UDF5,
                                        GUID = u.GUID,
                                        Created = u.Created,
                                        Updated = u.Updated,
                                        CreatedBy = u.CreatedBy,
                                        LastUpdatedBy = u.LastUpdatedBy,
                                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                        CompanyID = u.CompanyID,
                                        Room = u.Room,
                                        CreatedByName = u.CreatedByName,
                                        UpdatedByName = u.UpdatedByName,
                                        RoomName = u.RoomName,
                                        IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                                        ItemTypeName = u.ItemTypeName,
                                        CategoryName = u.CategoryName,
                                        Unit = u.Unit,
                                        GLAccount = u.GLAccount,
                                        IsBuildBreak = (u.IsBuildBreak.HasValue ? u.IsBuildBreak : false),
                                        BondedInventory = u.BondedInventory != null ? Convert.ToString(u.BondedInventory) : string.Empty,
                                        CriticalQuantity = u.CriticalQuantity,
                                        MinimumQuantity = u.MinimumQuantity,
                                        MaximumQuantity = u.MaximumQuantity,
                                        Cost = u.Cost,
                                        Markup = u.Markup,
                                        SellPrice = u.SellPrice,
                                        CostUOMID = u.CostUOMID,
                                        CostUOMName = u.CostUOMName,
                                        DefaultReorderQuantity = u.DefaultReorderQuantity,
                                        DefaultPullQuantity = u.DefaultPullQuantity,
                                        Link1 = u.Link1,
                                        Link2 = u.Link2,
                                        IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
                                        PullQtyScanOverride = u.PullQtyScanOverride,
                                        IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity ?? false,
                                        ItemImageExternalURL = u.ItemImageExternalURL,
                                        ItemDocExternalURL = u.ItemDocExternalURL,
                                        ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                                        IsActive = u.IsActive,
                                        LongDescription = u.LongDescription,
                                        EnhancedDescription = u.EnhancedDescription,
                                        EnrichedProductData = u.EnrichedProductData
                                    }).ToList();
            }



            StreamWriter write = new StreamWriter(filePath);


            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ItemNumber");
            csw.WriteField("Manufacturer");
            csw.WriteField("ManufacturerNumber");
            csw.WriteField("* SupplierName");
            csw.WriteField("* SupplierPartNo");
            csw.WriteField("UPC");
            csw.WriteField("UNSPSC");
            csw.WriteField("Description");
            csw.WriteField("CategoryName");
            csw.WriteField("GLAccount");
            csw.WriteField("* UOM");
            csw.WriteField("LeadTimeInDays");
            csw.WriteField("* Taxable");
            csw.WriteField("* Consignment");
            csw.WriteField("ItemUniqueNumber");
            csw.WriteField("IsTransfer");
            csw.WriteField("IsPurchase");
            //csw.WriteField("InventryLocation");
            csw.WriteField("InventoryClassification");
            csw.WriteField("* SerialNumberTracking");
            csw.WriteField("* LotNumberTracking");
            csw.WriteField("* DateCodeTracking");
            csw.WriteField("* ItemType");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("IsLotSerialExpiryCost");


            csw.WriteField("CriticalQuantity");
            csw.WriteField("MinimumQuantity");
            csw.WriteField("MaximumQuantity");
            csw.WriteField("Cost");
            csw.WriteField("Markup");
            csw.WriteField("SellPrice");
            csw.WriteField("CostUOM");
            csw.WriteField("DefaultReorderQuantity");
            csw.WriteField("DefaultPullQuantity");
            csw.WriteField("Link1");
            csw.WriteField("Link2");

            csw.WriteField("* IsItemLevelMinMaxQtyRequired");
            csw.WriteField("EnforceDefaultPullQuantity");
            csw.WriteField("EnforceDefaultReorderQuantity");
            csw.WriteField("ItemImageExternalURL");
            csw.WriteField("ItemDocExternalURL");
            csw.WriteField("ItemLink2ExternalURL");
            csw.WriteField("IsActive");
            csw.WriteField("ImagePath");
            csw.WriteField("LongDescription");
            csw.WriteField("EnhancedDescription");
            csw.WriteField("EnrichedProductData");


            csw.NextRecord();
            if (lstItemMasterDTO != null && lstItemMasterDTO.Count > 0)
            {


                foreach (ItemMasterDTO rec in ConvertExportDataList<ItemMasterDTO>(lstItemMasterDTO).ToList())
                {
                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.ManufacturerName);
                    csw.WriteField(rec.ManufacturerNumber);
                    csw.WriteField(rec.SupplierName);
                    csw.WriteField(rec.SupplierPartNo);
                    csw.WriteField(rec.UPC);
                    csw.WriteField(rec.UNSPSC);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.CategoryName);
                    csw.WriteField(rec.GLAccount);
                    csw.WriteField(rec.Unit);
                    csw.WriteField(rec.LeadTimeInDays);
                    csw.WriteField(rec.Taxable);
                    csw.WriteField(rec.Consignment);
                    csw.WriteField(rec.ItemUniqueNumber);
                    csw.WriteField(rec.IsTransfer);
                    csw.WriteField(rec.IsPurchase);
                    // csw.WriteField(rec.DefaultLocationName);
                    csw.WriteField(rec.InventoryClassificationName);
                    csw.WriteField(rec.SerialNumberTracking);
                    csw.WriteField(rec.LotNumberTracking);
                    csw.WriteField(rec.DateCodeTracking);
                    csw.WriteField(GetItemType(rec.ItemType));
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.WriteField(rec.IsLotSerialExpiryCost);
                    csw.WriteField(rec.CriticalQuantity);
                    csw.WriteField(rec.MinimumQuantity);
                    csw.WriteField(rec.MaximumQuantity);
                    csw.WriteField(rec.Cost);
                    csw.WriteField(rec.Markup);
                    csw.WriteField(rec.SellPrice);
                    csw.WriteField(rec.CostUOMName);
                    csw.WriteField(rec.DefaultReorderQuantity);
                    csw.WriteField(rec.DefaultPullQuantity);
                    csw.WriteField(rec.Link1);
                    csw.WriteField(rec.Link2);

                    csw.WriteField(rec.IsItemLevelMinMaxQtyRequired);
                    csw.WriteField(rec.PullQtyScanOverride);
                    csw.WriteField(rec.IsEnforceDefaultReorderQuantity);
                    csw.WriteField(rec.ItemImageExternalURL);
                    csw.WriteField(rec.ItemDocExternalURL);
                    csw.WriteField(rec.ItemLink2ExternalURL);
                    csw.WriteField(rec.IsActive);
                    csw.WriteField(rec.ImagePath);
                    csw.WriteField(rec.LongDescription);
                    csw.WriteField(rec.EnhancedDescription);
                    csw.WriteField(rec.EnrichedProductData);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string ItemLocationQtyCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ExportItemLocationDetailsDTO> lstInventoryLocationMain = obj.GetItemLocationDetailsQtyExport(ids, SessionHelper.RoomID, SessionHelper.CompanyID);//.OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("* ItemNumber");
            csw.WriteField("* LocationName");
            csw.WriteField("CustomerOwnedQuantity");
            csw.WriteField("ConsignedQuantity");
            csw.WriteField("SerialNumber");
            csw.WriteField("lotnumber");
            csw.WriteField("ExpirationDate");
            csw.WriteField("Cost");
            csw.NextRecord();

            if (lstInventoryLocationMain != null && lstInventoryLocationMain.Count > 0)
            {


                foreach (ExportItemLocationDetailsDTO rec in ConvertExportDataList<ExportItemLocationDetailsDTO>(lstInventoryLocationMain).ToList())
                {

                    csw.WriteField(rec.ItemNumber);
                    //if (!obj.IsLabor(rec.ItemNumber))
                    //{
                    //    csw.WriteField(rec.BinNumber);
                    //}
                    if (rec.ItemType == 4)
                    {
                        csw.WriteField("");  // Write empty for BinNumber if labor
                    }
                    else
                    {
                        csw.WriteField(rec.BinNumber);  // Write actual BinNumber if not labor
                    }
                    csw.WriteField(rec.CustomerOwnedQuantity);
                    csw.WriteField(rec.ConsignedQuantity);
                    csw.WriteField(rec.SerialNumber);
                    csw.WriteField(rec.LotNumber);
                    //csw.WriteField(rec.ExpirationDate);
                    if (rec.ExpirationDate.HasValue)
                    {
                        csw.WriteField(rec.ExpirationDate.Value.ToString("yyyy-MM-dd"));  // Format to date only
                    }
                    else
                    {
                        csw.WriteField("");  // Write an empty field if ExpirationDate is null
                    }
                    csw.WriteField(rec.Cost);

                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string ItemBinLocationQtyCSV(string Filepath, string ModuleName, string ids, string SortNameString, string BinIDs)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ExportItemLocationDetailsDTO> lstInventoryLocationMain = obj.GetItemBinLocationDetailsQtyExport(ids, SessionHelper.RoomID, SessionHelper.CompanyID, BinIDs);

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("* ItemNumber");
            csw.WriteField("* LocationName");
            csw.WriteField("CustomerOwnedQuantity");
            csw.WriteField("ConsignedQuantity");
            csw.WriteField("SerialNumber");
            csw.WriteField("lotnumber");
            csw.WriteField("ExpirationDate");
            csw.WriteField("Cost");
            csw.NextRecord();

            if (lstInventoryLocationMain != null && lstInventoryLocationMain.Count > 0)
            {


                foreach (ExportItemLocationDetailsDTO rec in ConvertExportDataList<ExportItemLocationDetailsDTO>(lstInventoryLocationMain).ToList())
                {

                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.BinNumber);
                    csw.WriteField(rec.CustomerOwnedQuantity);
                    csw.WriteField(rec.ConsignedQuantity);
                    csw.WriteField(rec.SerialNumber);
                    csw.WriteField(rec.LotNumber);
                    csw.WriteField(rec.ExpirationDate);
                    csw.WriteField(rec.Cost);

                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string ManualCountCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ExportItemLocationDetailsDTO> lstInventoryLocationMain = obj.InventoryCountItemLocationExport(ids, SessionHelper.RoomID, SessionHelper.CompanyID);//.OrderBy(SortNameString).ToList();
            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("* ItemNumber");
            csw.WriteField("* LocationName");
            csw.WriteField("CustomerOwnedQuantity");
            csw.WriteField("ConsignedQuantity");
            csw.WriteField("SerialNumber");
            csw.WriteField("lotnumber");
            csw.WriteField("ExpirationDate");
            //csw.WriteField("Cost");
            csw.WriteField("projectspend");
            csw.WriteField("ItemDescription");
            csw.NextRecord();

            if (lstInventoryLocationMain != null && lstInventoryLocationMain.Count > 0)
            {
                string Expdate = string.Empty;
                foreach (ExportItemLocationDetailsDTO rec in ConvertExportDataList<ExportItemLocationDetailsDTO>(lstInventoryLocationMain).ToList())
                {
                    Expdate = rec.ExpirationDate != null ? rec.ExpirationDate.Value.ToString(SessionHelper.DateTimeFormat) : string.Empty;
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.BinNumber);
                    csw.WriteField(rec.CustomerOwnedQuantity);
                    csw.WriteField(rec.ConsignedQuantity);
                    csw.WriteField(rec.SerialNumber);
                    csw.WriteField(rec.LotNumber);
                    csw.WriteField(Expdate);
                    //csw.WriteField(rec.Cost);
                    csw.WriteField("");
                    csw.WriteField(rec.CountItemDescription);
                    csw.NextRecord();
                }
            }

            write.Close();
            return filename;

        }
        public string AdjustmentCountCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ExportItemLocationDetailsDTO> lstInventoryLocationMain = obj.InventoryCountItemLocationExport(ids, SessionHelper.RoomID, SessionHelper.CompanyID);//.OrderBy(SortNameString).ToList();
            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("* ItemNumber");
            csw.WriteField("* LocationName");
            csw.WriteField("CustomerOwnedQuantity");
            csw.WriteField("ConsignedQuantity");
            csw.WriteField("SerialNumber");
            csw.WriteField("lotnumber");
            csw.WriteField("ExpirationDate");
            csw.WriteField("ItemDescription");
            csw.NextRecord();

            if (lstInventoryLocationMain != null && lstInventoryLocationMain.Count > 0)
            {
                string Expdate = string.Empty;

                foreach (ExportItemLocationDetailsDTO rec in ConvertExportDataList<ExportItemLocationDetailsDTO>(lstInventoryLocationMain).ToList())
                {
                    Expdate = rec.ExpirationDate != null ? rec.ExpirationDate.Value.ToString(SessionHelper.DateTimeFormat) : string.Empty;
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.BinNumber);
                    csw.WriteField(rec.CustomerOwnedQuantity);
                    csw.WriteField(rec.ConsignedQuantity);
                    csw.WriteField(rec.SerialNumber);
                    csw.WriteField(rec.LotNumber);
                    csw.WriteField(Expdate);
                    csw.WriteField(rec.CountItemDescription);
                    //csw.WriteField(rec.Cost);

                    csw.NextRecord();
                }
            }

            write.Close();
            return filename;

        }

        public string ToolAdjustmentCountCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolMasterDAL toolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstInventoryLocationMain = toolMasterDAL.ToolAdjustmentCountExport(SessionHelper.RoomID, SessionHelper.CompanyID);//.OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("* " + ResToolMaster.ToolName);
            csw.WriteField(ResLocation.Location);
            csw.WriteField(ResToolMaster.Quantity);
            csw.WriteField(ResToolMaster.Serial);
            csw.NextRecord();

            if (lstInventoryLocationMain != null && lstInventoryLocationMain.Count > 0)
            {
                foreach (ToolMasterDTO rec in ConvertExportDataList<ToolMasterDTO>(lstInventoryLocationMain).ToList())
                {

                    csw.WriteField(rec.ToolName);
                    csw.WriteField(rec.Location);
                    csw.WriteField(rec.Quantity);
                    csw.WriteField(rec.Serial);
                    csw.NextRecord();
                }
            }
            write.Close();

            return filename;

        }
        public string WorkOrderCountCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDAL objLocationDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            List<WorkOrderDTO> lstWOMain = obj.WorkOrderExport(SessionHelper.RoomID, SessionHelper.CompanyID, ids);//.OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("* WOName");
            csw.WriteField("ReleaseNumber");
            csw.WriteField("Description");
            csw.WriteField("Technician");
            csw.WriteField("Customer");
            csw.WriteField("WOStatus");
            csw.WriteField("WOType");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("SupplierName");
            csw.WriteField("Asset");
            csw.WriteField("Odometer");
            csw.WriteField("SupplierAccount");
            //csw.WriteField("ExpirationDate");
            //csw.WriteField("Cost");
            csw.NextRecord();

            if (lstWOMain != null && lstWOMain.Count > 0)
            {


                foreach (WorkOrderDTO rec in ConvertExportDataList<WorkOrderDTO>(lstWOMain).ToList())
                {

                    csw.WriteField(rec.WOName);
                    csw.WriteField(rec.ReleaseNumber);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.TechnicianCodeNameStr);
                    csw.WriteField(rec.Customer);
                    csw.WriteField(rec.WOStatus);
                    csw.WriteField(rec.WOType);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);
                    csw.WriteField(rec.SupplierName);
                    csw.WriteField(rec.AssetName);
                    csw.WriteField(rec.Odometer_OperationHours);
                    csw.WriteField(rec.SupplierAccount); 
                    //csw.WriteField(rec.ExpirationDate);
                    //csw.WriteField(rec.Cost);

                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string PullImportCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            List<PullImport> lstPullImport = obj.PullImportExport(SessionHelper.RoomID, SessionHelper.CompanyID);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("ItemNumber");
            csw.WriteField("PullQuantity");
            csw.WriteField("Location");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("ProjectSpendName");
            csw.WriteField("PullOrderNumber");
            csw.WriteField("WorkOrder");
            csw.WriteField("Asset");
            csw.WriteField("ActionType");
            csw.NextRecord();

            if (lstPullImport != null && lstPullImport.Count > 0)
            {
                foreach (PullImport objPullImport in ConvertExportDataList<PullImport>(lstPullImport).ToList())
                {
                    csw.WriteField(objPullImport.ItemNumber);
                    csw.WriteField(objPullImport.PullQuantity);
                    csw.WriteField(objPullImport.Location);
                    csw.WriteField(objPullImport.UDF1);
                    csw.WriteField(objPullImport.UDF2);
                    csw.WriteField(objPullImport.UDF3);
                    csw.WriteField(objPullImport.UDF4);
                    csw.WriteField(objPullImport.UDF5);
                    csw.WriteField(objPullImport.ProjectSpendName);
                    csw.WriteField(objPullImport.PullOrderNumber);
                    csw.WriteField(objPullImport.WorkOrder);
                    csw.WriteField(objPullImport.Asset);
                    csw.WriteField(objPullImport.ActionType);
                    csw.NextRecord();
                }
            }

            write.Close();

            return filename;

        }
        public string PullImportWithSameQty(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            List<PullImport> lstPullImport = obj.PullImportExport(SessionHelper.RoomID, SessionHelper.CompanyID);
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("ItemNumber");
            csw.WriteField("PullQuantity");
            csw.WriteField("BinNumber");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("ProjectSpendName");
            csw.WriteField("PullOrderNumber");
            csw.WriteField("WorkOrder");
            csw.WriteField("Asset");
            csw.WriteField("ActionType");
            csw.NextRecord();

            if (lstPullImport != null && lstPullImport.Count > 0)
            {
                foreach (PullImport objPullImport in ConvertExportDataList<PullImport>(lstPullImport).ToList())
                {
                    csw.WriteField(objPullImport.ItemNumber);
                    csw.WriteField(objPullImport.PullQuantity);
                    csw.WriteField(objPullImport.Location);
                    csw.WriteField(objPullImport.UDF1);
                    csw.WriteField(objPullImport.UDF2);
                    csw.WriteField(objPullImport.UDF3);
                    csw.WriteField(objPullImport.UDF4);
                    csw.WriteField(objPullImport.UDF5);
                    csw.WriteField(objPullImport.ProjectSpendName);
                    csw.WriteField(objPullImport.PullOrderNumber);
                    csw.WriteField(objPullImport.WorkOrder);
                    csw.WriteField(objPullImport.Asset);
                    csw.WriteField(objPullImport.ActionType);
                    csw.NextRecord();
                }
            }

            write.Close();

            return filename;

        }
        public string ItemLocationChangeImportCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ItemMasterBinDAL obj = new ItemMasterBinDAL(SessionHelper.EnterPriseDBName);
            List<ItemMasterBinDTO> lstPullImport = obj.ItemBinListChangeImportExport(SessionHelper.RoomID, SessionHelper.CompanyID, ids);
            if (lstPullImport != null && lstPullImport.Count > 0)
            {
                lstPullImport = (from u in lstPullImport
                                 where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.OldLocationGUID.ToString())
                                 select u).ToList();
            }
            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ItemNumber");
            csw.WriteField("* OldLocationName");
            csw.WriteField("* NewLocationName");
            csw.NextRecord();

            if (lstPullImport != null && lstPullImport.Count > 0)
            {
                foreach (ItemMasterBinDTO objPullImport in ConvertExportDataList<ItemMasterBinDTO>(lstPullImport).ToList())
                {
                    csw.WriteField(objPullImport.ID);
                    csw.WriteField(objPullImport.ItemNumber);
                    csw.WriteField(objPullImport.OldLocationName);
                    csw.WriteField(objPullImport.NewLocationName);
                    csw.NextRecord();
                }
            }

            write.Close();

            return filename;

        }

        public string ItemLocationCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationDetailsDTO> lstInventoryLocationMain = obj.GetItemLocationDetailsExport(ids, SessionHelper.RoomID, SessionHelper.CompanyID);//.OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);

            csw.WriteField("* ItemNumber");
            csw.WriteField("* LocationName");
            csw.WriteField("MinimumQuantity");
            csw.WriteField("MaximumQuantity");
            csw.WriteField("CriticalQuantity");
            csw.WriteField("SensorId");
            csw.WriteField("SensorPort");
            csw.WriteField("Isdefault");
            csw.WriteField("IsStagingLocation");
            csw.WriteField("DefaultPullQuantity");
            csw.WriteField("IsEnforceDefaultPullQuantity");
            csw.WriteField("DefaultReorderQuantity");
            csw.WriteField("IsEnforceDefaultReorderQuantity");
            csw.WriteField("Isdeleted");
            csw.WriteField("BinUDF1");
            csw.WriteField("BinUDF2");
            csw.WriteField("BinUDF3");
            csw.WriteField("BinUDF4");
            csw.WriteField("BinUDF5");


            csw.NextRecord();
            if (lstInventoryLocationMain != null && lstInventoryLocationMain.Count > 0)
            {

                foreach (ItemLocationDetailsDTO rec in ConvertExportDataList<ItemLocationDetailsDTO>(lstInventoryLocationMain).ToList())
                {

                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.BinNumber);
                    if (rec.IsItemLevelMinMax)
                    {
                        csw.WriteField("N/A");
                        csw.WriteField("N/A");
                        csw.WriteField("N/A");
                    }
                    else
                    {
                        csw.WriteField(rec.MinimumQuantity);
                        csw.WriteField(rec.MaximumQuantity);
                        csw.WriteField(rec.CriticalQuantity);
                    }

                    csw.WriteField(rec.eVMISensorIDdbl);
                    csw.WriteField(rec.eVMISensorPortstr);
                    csw.WriteField(rec.IsDefault);
                    //IsStagingLocation
                    csw.WriteField(rec.IsStagingLocation);
                    csw.WriteField(rec.DefaultPullQuantity);
                    csw.WriteField(rec.IsEnforceDefaultPullQuantity);
                    csw.WriteField(rec.DefaultReorderQuantity);
                    csw.WriteField(rec.IsEnforceDefaultReorderQuantity);
                    csw.WriteField(rec.IsDeleted);
                    csw.WriteField(rec.BinUDF1);
                    csw.WriteField(rec.BinUDF2);
                    csw.WriteField(rec.BinUDF3);
                    csw.WriteField(rec.BinUDF4);
                    csw.WriteField(rec.BinUDF5);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string ItemLocationByBinCSV(string Filepath, string ModuleName, string ids, string SortNameString, string BinIDs)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationDetailsDTO> lstInventoryLocationMain = obj.GetItemLocationDetailsExport(ids, SessionHelper.RoomID, SessionHelper.CompanyID, BinIDs);//.OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);

            csw.WriteField("* ItemNumber");
            csw.WriteField("* LocationName");
            csw.WriteField("MinimumQuantity");
            csw.WriteField("MaximumQuantity");
            csw.WriteField("CriticalQuantity");
            csw.WriteField("SensorId");
            csw.WriteField("SensorPort");
            csw.WriteField("Isdefault");
            csw.WriteField("IsStagingLocation");
            csw.WriteField("DefaultPullQuantity");
            csw.WriteField("IsEnforceDefaultPullQuantity");
            csw.WriteField("DefaultReorderQuantity");
            csw.WriteField("IsEnforceDefaultReorderQuantity");
            csw.WriteField("Isdeleted");
            csw.WriteField("BinUDF1");
            csw.WriteField("BinUDF2");
            csw.WriteField("BinUDF3");
            csw.WriteField("BinUDF4");
            csw.WriteField("BinUDF5");


            csw.NextRecord();
            if (lstInventoryLocationMain != null && lstInventoryLocationMain.Count > 0)
            {

                foreach (ItemLocationDetailsDTO rec in ConvertExportDataList<ItemLocationDetailsDTO>(lstInventoryLocationMain).ToList())
                {

                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(" " + rec.BinNumber);
                    //if (rec.IsItemLevelMinMax)
                    //{
                    //    csw.WriteField("N/A");
                    //    csw.WriteField("N/A");
                    //    csw.WriteField("N/A");
                    //}
                    //else
                    //{
                    csw.WriteField(rec.MinimumQuantity);
                    csw.WriteField(rec.MaximumQuantity);
                    csw.WriteField(rec.CriticalQuantity);
                    //}

                    csw.WriteField(rec.eVMISensorIDdbl);
                    csw.WriteField(rec.eVMISensorPortstr);
                    csw.WriteField(rec.IsDefault);
                    //IsStagingLocation
                    csw.WriteField(rec.IsStagingLocation);
                    csw.WriteField(rec.DefaultPullQuantity);
                    csw.WriteField(rec.IsEnforceDefaultPullQuantity);
                    csw.WriteField(rec.DefaultReorderQuantity);
                    csw.WriteField(rec.IsEnforceDefaultReorderQuantity);
                    csw.WriteField(rec.IsDeleted);
                    csw.WriteField(rec.BinUDF1);
                    csw.WriteField(rec.BinUDF2);
                    csw.WriteField(rec.BinUDF3);
                    csw.WriteField(rec.BinUDF4);
                    csw.WriteField(rec.BinUDF5);

                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string KitsCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            KitDetailDAL obj = new KitDetailDAL(SessionHelper.EnterPriseDBName);

            IEnumerable<KitDetailmain> DataFromDB = obj.GetKitDetailExport(SessionHelper.CompanyID, SessionHelper.RoomID);//.OrderBy(SortNameString);
            List<KitDetailmain> lstKitDetailDTO = new List<KitDetailmain>();


            //if (!string.IsNullOrEmpty(ids))
            {
                lstKitDetailDTO = (from u in DataFromDB
                                   where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.KitGUID.ToString())
                                   select new KitDetailmain
                                   {
                                       ID = u.ID,
                                       KitPartNumber = u.KitPartNumber != null ? Convert.ToString(u.KitPartNumber) : string.Empty,
                                       ItemNumber = u.ItemNumber != null ? Convert.ToString(u.ItemNumber) : string.Empty,
                                       QuantityPerKit = u.QuantityPerKit,
                                       // OnHandQuantity = u.OnHandQuantity,
                                       IsDeleted = u.IsDeleted != null ? u.IsDeleted : false,
                                       IsBuildBreak = u.IsBuildBreak != null ? u.IsBuildBreak : false,
                                       // AvailableItemsInWIP = u.AvailableItemsInWIP,
                                       //KitDemand = u.KitDemand ?? 0,
                                       AvailableKitQuantity = u.AvailableKitQuantity ?? 0,
                                       Description = u.Description,
                                       CriticalQuantity = u.CriticalQuantity ?? 0,
                                       MinimumQuantity = u.MinimumQuantity ?? 0,
                                       MaximumQuantity = u.MaximumQuantity ?? 0,
                                       //ReOrderType = u.ReOrderType,
                                       //KitCategory = u.KitCategory,
                                       SupplierName = u.SupplierName,
                                       SupplierPartNo = u.SupplierPartNo,
                                       DefaultLocationName = u.DefaultLocationName,
                                       CostUOMName = u.CostUOMName,
                                       UOM = u.UOM,
                                       DefaultReorderQuantity = u.DefaultReorderQuantity,
                                       DefaultPullQuantity = u.DefaultPullQuantity,
                                       //ItemTypeName = u.ItemTypeName,
                                       IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
                                       SerialNumberTracking = u.SerialNumberTracking,
                                       LotNumberTracking = u.LotNumberTracking,
                                       DateCodeTracking = u.DateCodeTracking,
                                       IsActive = u.IsActive
                                   }).ToList();
            }

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);

            csw.WriteField("* KitPartNumber");
            csw.WriteField("* ItemNumber");
            csw.WriteField("* QuantityPerKit");
            //csw.WriteField("OnHandQuantity");
            csw.WriteField("IsDeleted");
            csw.WriteField("IsBuildBreak");
            //  csw.WriteField("AvailableItemsInWIP");
            // csw.WriteField("KitDemand");
            csw.WriteField("AvailableKitQuantity");
            csw.WriteField("Description");
            csw.WriteField("CriticalQuantity");
            csw.WriteField("MinimumQuantity");
            csw.WriteField("MaximumQuantity");
            //csw.WriteField("ReOrderType");
            //csw.WriteField("KitCategory");

            csw.WriteField("* SupplierName");
            csw.WriteField("* SupplierPartNo");
            csw.WriteField("* DefaultLocation");
            csw.WriteField("* CostUOMName");
            csw.WriteField("* UOM");
            csw.WriteField("* DefaultReorderQuantity");
            csw.WriteField("* DefaultPullQuantity");
            //csw.WriteField("* ItemTypeName");
            csw.WriteField("* IsItemLevelMinMaxQtyRequired");
            csw.WriteField("SerialNumberTracking");
            csw.WriteField("LotNumberTracking");
            csw.WriteField("DateCodeTracking");
            csw.WriteField("IsActive");
            csw.NextRecord();
            if (lstKitDetailDTO != null && lstKitDetailDTO.Count > 0)
            {
                foreach (KitDetailmain rec in ConvertExportDataList<KitDetailmain>(lstKitDetailDTO).ToList())
                {
                    csw.WriteField(rec.KitPartNumber);

                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.QuantityPerKit);
                    //csw.WriteField(rec.OnHandQuantity);
                    csw.WriteField(rec.IsDeleted);
                    csw.WriteField(rec.IsBuildBreak);
                    //  csw.WriteField(rec.AvailableItemsInWIP);
                    // csw.WriteField(rec.KitDemand);
                    csw.WriteField(rec.AvailableKitQuantity);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.CriticalQuantity);
                    csw.WriteField(rec.MinimumQuantity);
                    csw.WriteField(rec.MaximumQuantity);
                    //csw.WriteField(rec.ReOrderType);
                    //csw.WriteField(rec.KitCategory);

                    csw.WriteField(rec.SupplierName);
                    csw.WriteField(rec.SupplierPartNo);
                    csw.WriteField(rec.DefaultLocationName);
                    csw.WriteField(rec.CostUOMName);
                    csw.WriteField(rec.UOM);
                    csw.WriteField(rec.DefaultReorderQuantity);
                    csw.WriteField(rec.DefaultPullQuantity);
                    // csw.WriteField(rec.ItemTypeName);
                    csw.WriteField(rec.IsItemLevelMinMaxQtyRequired);
                    csw.WriteField(rec.SerialNumberTracking);
                    csw.WriteField(rec.LotNumberTracking);
                    csw.WriteField(rec.DateCodeTracking);
                    csw.WriteField(rec.IsActive);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string KitsBinCSV(string Filepath, string ModuleName, string ids, string SortNameString, string BinIDs)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            KitDetailDAL obj = new KitDetailDAL(SessionHelper.EnterPriseDBName);

            IEnumerable<KitDetailmain> DataFromDB = obj.GetKitBinDetailExport(SessionHelper.CompanyID, SessionHelper.RoomID, BinIDs);//.OrderBy(SortNameString);
            List<KitDetailmain> lstKitDetailDTO = new List<KitDetailmain>();


            //if (!string.IsNullOrEmpty(ids))
            {
                lstKitDetailDTO = (from u in DataFromDB
                                   where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.KitGUID.ToString())
                                   select new KitDetailmain
                                   {
                                       ID = u.ID,
                                       KitPartNumber = u.KitPartNumber != null ? Convert.ToString(u.KitPartNumber) : string.Empty,
                                       ItemNumber = u.ItemNumber != null ? Convert.ToString(u.ItemNumber) : string.Empty,
                                       QuantityPerKit = u.QuantityPerKit,
                                       // OnHandQuantity = u.OnHandQuantity,
                                       IsDeleted = u.IsDeleted != null ? u.IsDeleted : false,
                                       IsBuildBreak = u.IsBuildBreak != null ? u.IsBuildBreak : false,
                                       // AvailableItemsInWIP = u.AvailableItemsInWIP,
                                       //KitDemand = u.KitDemand ?? 0,
                                       AvailableKitQuantity = u.AvailableKitQuantity ?? 0,
                                       Description = u.Description,
                                       CriticalQuantity = u.CriticalQuantity ?? 0,
                                       MinimumQuantity = u.MinimumQuantity ?? 0,
                                       MaximumQuantity = u.MaximumQuantity ?? 0,
                                       //ReOrderType = u.ReOrderType,
                                       //KitCategory = u.KitCategory,
                                       SupplierName = u.SupplierName,
                                       SupplierPartNo = u.SupplierPartNo,
                                       DefaultLocationName = u.DefaultLocationName,
                                       CostUOMName = u.CostUOMName,
                                       UOM = u.UOM,
                                       DefaultReorderQuantity = u.DefaultReorderQuantity,
                                       DefaultPullQuantity = u.DefaultPullQuantity,
                                       //ItemTypeName = u.ItemTypeName,
                                       IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
                                       SerialNumberTracking = u.SerialNumberTracking,
                                       LotNumberTracking = u.LotNumberTracking,
                                       DateCodeTracking = u.DateCodeTracking,
                                       IsActive = u.IsActive,
                                       BinNumber = u.BinNumber
                                   }).ToList();
            }

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);

            csw.WriteField("* KitPartNumber");
            csw.WriteField("* ItemNumber");
            csw.WriteField("* QuantityPerKit");
            //csw.WriteField("OnHandQuantity");
            csw.WriteField("IsDeleted");
            csw.WriteField("IsBuildBreak");
            csw.WriteField("BinNumber");
            //  csw.WriteField("AvailableItemsInWIP");
            // csw.WriteField("KitDemand");
            csw.WriteField("AvailableKitQuantity");
            csw.WriteField("Description");
            csw.WriteField("CriticalQuantity");
            csw.WriteField("MinimumQuantity");
            csw.WriteField("MaximumQuantity");
            //csw.WriteField("ReOrderType");
            //csw.WriteField("KitCategory");

            csw.WriteField("* SupplierName");
            csw.WriteField("* SupplierPartNo");
            csw.WriteField("* DefaultLocation");
            csw.WriteField("* CostUOMName");
            csw.WriteField("* UOM");
            csw.WriteField("* DefaultReorderQuantity");
            csw.WriteField("* DefaultPullQuantity");
            //csw.WriteField("* ItemTypeName");
            csw.WriteField("* IsItemLevelMinMaxQtyRequired");
            csw.WriteField("SerialNumberTracking");
            csw.WriteField("LotNumberTracking");
            csw.WriteField("DateCodeTracking");
            csw.WriteField("IsActive");
            csw.NextRecord();
            if (lstKitDetailDTO != null && lstKitDetailDTO.Count > 0)
            {
                foreach (KitDetailmain rec in ConvertExportDataList<KitDetailmain>(lstKitDetailDTO).ToList())
                {
                    csw.WriteField(rec.KitPartNumber);

                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.QuantityPerKit);
                    //csw.WriteField(rec.OnHandQuantity);
                    csw.WriteField(rec.IsDeleted);
                    csw.WriteField(rec.IsBuildBreak);
                    csw.WriteField(rec.BinNumber);
                    // csw.WriteField(rec.AvailableItemsInWIP);
                    // csw.WriteField(rec.KitDemand);
                    csw.WriteField(rec.AvailableKitQuantity);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.CriticalQuantity);
                    csw.WriteField(rec.MinimumQuantity);
                    csw.WriteField(rec.MaximumQuantity);
                    //csw.WriteField(rec.ReOrderType);
                    //csw.WriteField(rec.KitCategory);

                    csw.WriteField(rec.SupplierName);
                    csw.WriteField(rec.SupplierPartNo);
                    csw.WriteField(rec.DefaultLocationName);
                    csw.WriteField(rec.CostUOMName);
                    csw.WriteField(rec.UOM);
                    csw.WriteField(rec.DefaultReorderQuantity);
                    csw.WriteField(rec.DefaultPullQuantity);
                    // csw.WriteField(rec.ItemTypeName);
                    csw.WriteField(rec.IsItemLevelMinMaxQtyRequired);
                    csw.WriteField(rec.SerialNumberTracking);
                    csw.WriteField(rec.LotNumberTracking);
                    csw.WriteField(rec.DateCodeTracking);
                    csw.WriteField(rec.IsActive);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string ItemmanufacturerCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ItemManufacturerDetailsDAL obj = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);

            IEnumerable<ItemManufacturer> DataFromDB = obj.GetItemManufacturerExport(SessionHelper.CompanyID, SessionHelper.RoomID).OrderBy(SortNameString);
            List<ItemManufacturer> lstItemManufacturerDTO = new List<ItemManufacturer>();


            //if (!string.IsNullOrEmpty(ids))
            {
                lstItemManufacturerDTO = (from u in DataFromDB
                                          where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.ItemGUID.ToString())
                                          select new ItemManufacturer
                                          {
                                              ID = u.ID,
                                              ItemNumber = u.ItemNumber,
                                              ManufacturerNumber = u.ManufacturerNumber,
                                              ManufacturerName = u.ManufacturerName,
                                              IsDefault = u.IsDefault
                                          }).ToList();
            }

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ItemNumber");
            csw.WriteField("* ManufacturerName");
            csw.WriteField("ManufacturerNumber");
            csw.WriteField("IsDefault");

            csw.NextRecord();
            if (lstItemManufacturerDTO != null && lstItemManufacturerDTO.Count > 0)
            {
                foreach (ItemManufacturer rec in ConvertExportDataList<ItemManufacturer>(lstItemManufacturerDTO).ToList())
                {
                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.ManufacturerName);
                    csw.WriteField(rec.ManufacturerNumber);

                    csw.WriteField(rec.IsDefault);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string BarCodeMasterCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            BarcodeMasterDAL obj = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);

            IEnumerable<BarcodeMasterDTO> DataFromDB = obj.GetBarcodeExport(SessionHelper.CompanyID, SessionHelper.RoomID).OrderBy(SortNameString);
            List<BarcodeMasterDTO> lstBarcodeMasterDTO = new List<BarcodeMasterDTO>();


            //if (!string.IsNullOrEmpty(ids))
            {
                lstBarcodeMasterDTO = (from u in DataFromDB
                                       where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.ID.ToString())
                                       select new BarcodeMasterDTO
                                       {
                                           ID = u.ID,
                                           ModuleName = u.ModuleName,
                                           items = u.items,
                                           BarcodeString = u.BarcodeString,
                                           BinNumber = u.BinNumber
                                       }).ToList();
            }

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ModuleName");
            csw.WriteField("* ItemNumber");
            csw.WriteField("* BarcodeString");
            csw.WriteField("Binnumber");

            csw.NextRecord();
            if (lstBarcodeMasterDTO != null && lstBarcodeMasterDTO.Count > 0)
            {
                foreach (BarcodeMasterDTO rec in ConvertExportDataList<BarcodeMasterDTO>(lstBarcodeMasterDTO).ToList())
                {
                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ModuleName);
                    csw.WriteField(rec.items);
                    csw.WriteField(rec.BarcodeString);

                    csw.WriteField(rec.BinNumber);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string UDFMasterCSV(string Filepath, string ModuleName, string TableName, bool? IsDeleted)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;

            UDFDAL obj = new UDFDAL(SessionHelper.EnterPriseDBName);

            IEnumerable<ExportUDFDTO> DataFromDB = obj.GetUDFExport(SessionHelper.RoomID, SessionHelper.CompanyID, TableName, IsDeleted);

            foreach (var item in DataFromDB)
            {
                if (TableName.ToLower() == "enterprises")
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(item.ModuleName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, false, true, false);
                    if (!string.IsNullOrEmpty(val))
                        item.UDFName = val;
                    else
                        item.UDFName = item.UDFColumnName;
                }
                else if (TableName.ToLower() == "companies" || TableName.ToLower() == "rooms")
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(item.ModuleName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, false, true, false);
                    if (!string.IsNullOrEmpty(val))
                        item.UDFName = val;
                    else
                        item.UDFName = item.UDFColumnName;
                }
                else
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(item.ModuleName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, true, true, false);
                    if (!string.IsNullOrEmpty(val))
                        item.UDFName = val;
                    else
                        item.UDFName = item.UDFColumnName;
                }
            }


            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);

            csw.WriteField("* ModuleName");
            csw.WriteField("* UDFColumnName");
            csw.WriteField("* UDFName");
            csw.WriteField("* ControlType");
            csw.WriteField("DefaultValue");
            csw.WriteField("OptionName");
            csw.WriteField("IsRequired");
            csw.WriteField("IsDeleted");
            csw.WriteField("IncludeInNarrowSearch");
            csw.WriteField("UDFMaxLength");

            csw.NextRecord();
            if (DataFromDB != null && DataFromDB.Count() > 0)
            {
                foreach (ExportUDFDTO rec in ConvertExportDataList<ExportUDFDTO>(DataFromDB).ToList())
                {
                    csw.WriteField(rec.ModuleName);
                    csw.WriteField(rec.UDFColumnName);
                    csw.WriteField(rec.UDFName);
                    csw.WriteField(rec.ControlType);
                    csw.WriteField(rec.UDFDefaultValue);
                    csw.WriteField(rec.OptionName);
                    csw.WriteField(rec.IsRequired);
                    csw.WriteField(rec.IsDeleted);
                    csw.WriteField(rec.IncludeInNarrowSearch);
                    csw.WriteField(rec.UDFMaxLength);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string ProjectMasterCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;

            ProjectMasterDAL obj = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<ProjectMasterMain> DataFromDB = obj.GetProjectExport(SessionHelper.RoomID, SessionHelper.CompanyID, ids, SessionHelper.UserSupplierIds);
            List<ProjectMasterMain> lstProjectMasterDTO = new List<ProjectMasterMain>();

            if (DataFromDB != null && DataFromDB.Any())
            {
                lstProjectMasterDTO = DataFromDB.ToList();
            }

            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);

            csw.WriteField("* ProjectSpendName");
            csw.WriteField("* ItemNumber");
            csw.WriteField("* DollarLimitAmount");
            csw.WriteField("TrackAllUsageAgainstThis");
            csw.WriteField("IsClosed");
            csw.WriteField("IsDeleted");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.WriteField("ItemDollarLimitAmount");
            csw.WriteField("ItemQuantityLimitAmount");
            csw.WriteField("IsLineItemDeleted");

            csw.NextRecord();
            if (lstProjectMasterDTO != null && lstProjectMasterDTO.Count() > 0)
            {
                foreach (ProjectMasterMain rec in ConvertExportDataList<ProjectMasterMain>(lstProjectMasterDTO).ToList())
                {
                    csw.WriteField(rec.ProjectSpendName);
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.DollarLimitAmount);
                    csw.WriteField(rec.TrackAllUsageAgainstThis);
                    csw.WriteField(rec.IsClosed);
                    csw.WriteField(rec.IsDeleted);
                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);

                    csw.WriteField(rec.ItemDollarLimitAmount);
                    csw.WriteField(rec.ItemQuantityLimitAmount);
                    csw.WriteField(rec.IsLineItemDeleted);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string KITMasterCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;

            KitMasterDAL obj = new KitMasterDAL(SessionHelper.EnterPriseDBName);
            List<ExportKitDTO> lstInventoryLocationMain = obj.GetKitExport(ids, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("* KitPartNumber");
            csw.WriteField("* ItemNumber");
            csw.WriteField("* QuantityPerKit");
            csw.WriteField("OnHandQuantity");
            csw.WriteField("IsDeleted");
            csw.WriteField("IsBuildBreak");

            csw.NextRecord();

            if (lstInventoryLocationMain != null && lstInventoryLocationMain.Count > 0)
            {


                foreach (ExportKitDTO rec in ConvertExportDataList<ExportKitDTO>(lstInventoryLocationMain).ToList())
                {

                    csw.WriteField(rec.KitPartNumber);
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.QuantityPerKit);
                    csw.WriteField(rec.OnHandQuantity);
                    csw.WriteField(rec.IsDeleted);
                    csw.WriteField(rec.IsBuildBreak);

                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        public string ItemSupplierCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ItemSupplierDetailsDAL obj = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);

            //IEnumerable<ItemSupplier> DataFromDB = obj.GetItemSupplierExport(SessionHelper.CompanyID, SessionHelper.RoomID).OrderBy(SortNameString);
            IEnumerable<ItemSupplier> DataFromDB = obj.GetItemSupplierExportByRoomNormal(SessionHelper.CompanyID, SessionHelper.RoomID, SortNameString);
            List<ItemSupplier> lstItemSupplierDTO = new List<ItemSupplier>();


            //if (!string.IsNullOrEmpty(ids))
            {
                lstItemSupplierDTO = (from u in DataFromDB
                                      where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.ItemGUID.ToString())
                                      select new ItemSupplier
                                      {
                                          ID = u.ID,
                                          ItemNumber = u.ItemNumber,
                                          SupplierNumber = u.SupplierNumber,
                                          SupplierName = u.SupplierName,
                                          BlanketPOName = u.BlanketPOName,
                                          IsDefault = u.IsDefault
                                      }).ToList();
            }

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ItemNumber");
            csw.WriteField("* SupplierName");
            csw.WriteField("SupplierNumber");
            csw.WriteField("BlanketPOName");
            csw.WriteField("IsDefault");

            csw.NextRecord();
            if (lstItemSupplierDTO != null && lstItemSupplierDTO.Count > 0)
            {
                foreach (ItemSupplier rec in ConvertExportDataList<ItemSupplier>(lstItemSupplierDTO).ToList())
                {
                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.SupplierName);
                    csw.WriteField(rec.SupplierNumber);
                    csw.WriteField(rec.BlanketPOName);
                    csw.WriteField(rec.IsDefault);
                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }
        private string GetItemType(int ItemType)
        {
            string ItemTypeName = string.Empty;
            if (ItemType == 1)
            {
                ItemTypeName = "Item";
            }
            else if (ItemType == 3)
            {
                ItemTypeName = "Kit";
            }
            else if (ItemType == 4)
            {
                ItemTypeName = "Labor";
            }
            return ItemTypeName;
        }
        private string GetTrendingSetting(int? TrendingSetting)
        {
            if (TrendingSetting == null)
            {
                return "";
            }
            string TrendingSettingName = string.Empty;
            if (TrendingSetting == 0)
            {
                TrendingSettingName = "None";
            }
            else if (TrendingSetting == 1)
            {
                TrendingSettingName = "Manual";
            }
            else if (TrendingSetting == 2)
            {
                TrendingSettingName = "Automatic";
            }
            return TrendingSettingName;
        }


        public string AssetToolSchedulerCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolsSchedulerDAL obj = new ToolsSchedulerDAL(SessionHelper.EnterPriseDBName);
            List<ToolsSchedulerDTO> DataFromDB = obj.ToolsScheduleExportData(ids, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).ToList();
            List<ToolsSchedulerDTO> lstToolsSchedulerDTO = new List<ToolsSchedulerDTO>();
            //if (!string.IsNullOrEmpty(ids))
            {
                lstToolsSchedulerDTO = (from u in DataFromDB
                                        where ((string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.ID.ToString()))
                                        && ((u.IsDeleted ?? false) == false)
                                        && ((u.IsLineItemDeleted ?? false) == false)
                                        select new ToolsSchedulerDTO
                                        {
                                            ID = u.ID,
                                            ScheduleFor = u.ScheduleFor,
                                            ScheduleForName = u.ScheduleForName,
                                            SchedulerName = u.SchedulerName,
                                            SchedulerType = u.SchedulerType,
                                            SchedulerTypeName = u.SchedulerTypeName,
                                            Mileage = u.Mileage,
                                            OperationalHours = u.OperationalHours,
                                            CheckOuts = u.CheckOuts,
                                            TimeBasedFrequency = u.TimeBasedFrequency,
                                            TimeBaseUnit = u.TimeBaseUnit,
                                            TimeBaseUnitName = u.TimeBaseUnitName,
                                            ItemNumber = u.ItemNumber,
                                            Quantity = u.Quantity,
                                            IsLineItemDeleted = u.IsLineItemDeleted,
                                            UDF1 = u.UDF1,
                                            UDF2 = u.UDF2,
                                            UDF3 = u.UDF3,
                                            UDF4 = u.UDF4,
                                            UDF5 = u.UDF5

                                        }).ToList();
            }

            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ScheduleFor");
            csw.WriteField("* SchedulerName");
            csw.WriteField("Description");

            csw.WriteField("SchedulerType");

            csw.WriteField("TimeBasedUnit");
            csw.WriteField("TimeBasedFrequency");

            csw.WriteField("Checkouts");

            csw.WriteField("OperationalHours");

            csw.WriteField("Mileage");

            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");

            csw.WriteField("ItemNumber");
            csw.WriteField("Quantity");

            csw.WriteField("IsDeleted");

            csw.NextRecord();

            if (lstToolsSchedulerDTO != null && lstToolsSchedulerDTO.Count > 0)
            {

                foreach (ToolsSchedulerDTO rec in ConvertExportDataList<ToolsSchedulerDTO>(lstToolsSchedulerDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ScheduleForName);
                    csw.WriteField(rec.SchedulerName);
                    csw.WriteField(rec.Description);
                    csw.WriteField(rec.ScheduleTypeName);
                    csw.WriteField(rec.TimeBaseUnitName);
                    csw.WriteField(rec.TimeBasedFrequency);

                    csw.WriteField(rec.CheckOuts);
                    csw.WriteField(rec.OperationalHours);
                    csw.WriteField(rec.Mileage);

                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);

                    csw.WriteField(rec.ItemNumber);

                    csw.WriteField(rec.Quantity);

                    csw.WriteField(rec.IsLineItemDeleted);

                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;

        }

        public string AssetToolSchedulerMappingCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolsSchedulerDAL obj = new ToolsSchedulerDAL(SessionHelper.EnterPriseDBName);
            List<ToolsSchedulerMappingDTO> DataFromDB = obj.ToolsScheduleMappingExportData(ids, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            List<ToolsSchedulerMappingDTO> lstToolsSchedulerMapDTO = new List<ToolsSchedulerMappingDTO>();
            //if (!string.IsNullOrEmpty(ids))
            {
                lstToolsSchedulerMapDTO = (from u in DataFromDB
                                           where ((string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.ID.ToString()))
                                           && ((u.IsDeleted ?? false) == false)
                                           select new ToolsSchedulerMappingDTO
                                           {
                                               ID = u.ID,
                                               GUID = u.GUID,
                                               SchedulerFor = u.SchedulerFor,
                                               SchedulerForName = u.SchedulerForName,
                                               SchedulerName = u.SchedulerName,
                                               ToolName = u.ToolName,
                                               AssetName = u.AssetName,
                                               Serial = u.Serial,
                                               UDF1 = u.UDF1,
                                               UDF2 = u.UDF2,
                                               UDF3 = u.UDF3,
                                               UDF4 = u.UDF4,
                                               UDF5 = u.UDF5,

                                           }).ToList();
            }

            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ScheduleFor");
            csw.WriteField("ToolName");
            csw.WriteField("Serial");
            csw.WriteField("AssetName");
            csw.WriteField("* SchedulerName");


            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");


            csw.NextRecord();

            if (lstToolsSchedulerMapDTO != null && lstToolsSchedulerMapDTO.Count > 0)
            {

                foreach (ToolsSchedulerMappingDTO rec in ConvertExportDataList<ToolsSchedulerMappingDTO>(lstToolsSchedulerMapDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.SchedulerForName);

                    csw.WriteField(rec.ToolName);
                    csw.WriteField(rec.Serial);
                    csw.WriteField(rec.AssetName);

                    csw.WriteField(rec.SchedulerName);


                    csw.WriteField(rec.UDF1);
                    csw.WriteField(rec.UDF2);
                    csw.WriteField(rec.UDF3);
                    csw.WriteField(rec.UDF4);
                    csw.WriteField(rec.UDF5);

                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;


        }



        public string PastMaintenanceDueCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolMaintenanceDetailsDAL obj = new ToolMaintenanceDetailsDAL(SessionHelper.EnterPriseDBName);
            List<PastMaintenanceDueImport> DataFromDB = obj.PastMaintenanceDueExportData(ids, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            List<PastMaintenanceDueImport> lstToolsSchedulerMapDTO = new List<PastMaintenanceDueImport>();
            //if (!string.IsNullOrEmpty(ids))
            {
                lstToolsSchedulerMapDTO = (from u in DataFromDB
                                           where ((string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.ID.ToString()))
                                           && ((u.IsDeleted ?? false) == false)
                                           select new PastMaintenanceDueImport
                                           {
                                               ID = u.ID,
                                               GUID = u.GUID,
                                               ScheduleFor = u.ScheduleFor ?? string.Empty,
                                               MaintenanceDate = u.MaintenanceDate,
                                               AssetName = u.AssetName ?? string.Empty,
                                               SchedulerName = u.SchedulerName ?? string.Empty,
                                               ToolName = u.ToolName ?? string.Empty,
                                               Serial = u.Serial ?? string.Empty,
                                               ItemNumber = u.ItemNumber ?? string.Empty,
                                               ItemCost = u.ItemCost ?? 0,
                                               Quantity = u.Quantity ?? 0

                                           }).ToList();
            }

            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* ScheduleFor");
            csw.WriteField("* MaintenanceDate");
            csw.WriteField("AssetName");
            csw.WriteField("ToolName");
            csw.WriteField("Serial");
            csw.WriteField("* SchedulerName");
            csw.WriteField("ItemNumber");
            csw.WriteField("ItemCost");
            csw.WriteField("Quantity");

            csw.NextRecord();

            if (lstToolsSchedulerMapDTO != null && lstToolsSchedulerMapDTO.Count > 0)
            {

                foreach (PastMaintenanceDueImport rec in ConvertExportDataList<PastMaintenanceDueImport>(lstToolsSchedulerMapDTO).ToList())
                {

                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.ScheduleFor);
                    csw.WriteField(rec.MaintenanceDate);//MaintenanceDate
                    csw.WriteField(rec.AssetName);
                    csw.WriteField(rec.ToolName);
                    csw.WriteField(rec.Serial);
                    csw.WriteField(rec.SchedulerName);
                    csw.WriteField(rec.ItemNumber);
                    csw.WriteField(rec.ItemCost);
                    csw.WriteField(rec.Quantity);

                    csw.NextRecord();

                }
            }
            write.Close();

            return filename;


        }

        public string ToolsMaintenanceListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            AssetMasterDAL obj = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolsMaintenanceDTO> lstToolsMaintenanceDTO = new List<ToolsMaintenanceDTO>();
            lstToolsMaintenanceDTO = obj.GetToolsMaintenanceByIDsNormal(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* MaintenanceName");
            csw.WriteField("* MaintenanceDate");
            csw.WriteField("* ScheduleDate");
            csw.WriteField("* SchedulerName");
            csw.WriteField("* SchedulerForName");
            csw.WriteField("ToolName");
            csw.WriteField("AssetName");
            csw.WriteField("TrackingMeasurement");
            csw.WriteField("TrackingMeasurementValue");
            csw.WriteField("LastMaintenanceDate");
            csw.WriteField("LastMeasurementValue");
            csw.WriteField("WOName");
            csw.WriteField("RequisitionName");
            csw.WriteField("Serial");
            csw.WriteField("TotalCost");
            csw.WriteField("UDF1");
            csw.WriteField("UDF2");
            csw.WriteField("UDF3");
            csw.WriteField("UDF4");
            csw.WriteField("UDF5");
            csw.WriteField("Created");
            csw.WriteField("Updated");
            csw.WriteField("CreatedBy");
            csw.WriteField("UpdatedBy");

            csw.NextRecord();
            if (lstToolsMaintenanceDTO != null && lstToolsMaintenanceDTO.Count > 0)
            {
                foreach (ToolsMaintenanceDTO item in ConvertExportDataList<ToolsMaintenanceDTO>(lstToolsMaintenanceDTO).ToList())
                {
                    csw.WriteField(item.ID);
                    csw.WriteField(item.MaintenanceName ?? string.Empty);
                    csw.WriteField(Convert.ToString(item.MaintenanceDateStr));
                    csw.WriteField(Convert.ToString(item.ScheduleDateStrOnlyDate));
                    csw.WriteField(item.SchedulerName);
                    csw.WriteField(item.SchedulerForName);
                    csw.WriteField(item.ToolName);
                    csw.WriteField(item.AssetName);
                    if (item.TrackngMeasurement == 1)
                    {
                        if (item.TrackingMeasurementMapping == 2)
                        {
                            csw.WriteField(@ResToolsScheduler.OperationalHours);
                        }
                        else if (item.TrackingMeasurementMapping == 3)
                        {
                            csw.WriteField(@ResToolsScheduler.Mileage);
                        }
                        else
                        {
                            csw.WriteField("");
                        }
                    }
                    else if (item.TrackngMeasurement == 2)
                    {
                        csw.WriteField(@ResToolsScheduler.OperationalHours);
                    }
                    else if (item.TrackngMeasurement == 3)
                    {
                        csw.WriteField(@ResToolsScheduler.Mileage);
                    }
                    else if (item.TrackngMeasurement == 4)
                    {
                        csw.WriteField(@ResToolsScheduler.CheckOuts);
                    }
                    else
                    {
                        csw.WriteField("");
                    }
                    csw.WriteField(item.TrackingMeasurementValue);
                    csw.WriteField(Convert.ToString(item.LastMaintenanceDateStr));
                    csw.WriteField(item.LastMeasurementValue);
                    csw.WriteField(item.WOName);
                    csw.WriteField(item.RequisitionName);
                    csw.WriteField(item.Serial);
                    csw.WriteField(item.TotalCost ?? 0);
                    csw.WriteField(item.UDF1);
                    csw.WriteField(item.UDF2);
                    csw.WriteField(item.UDF3);
                    csw.WriteField(item.UDF4);
                    csw.WriteField(item.UDF5);
                    csw.WriteField(Convert.ToString(item.Created));
                    csw.WriteField(Convert.ToString(item.Updated));
                    csw.WriteField(item.CreatedByName);
                    csw.WriteField(item.UpdatedByName);
                    csw.NextRecord();
                }
            }
            write.Close();
            return filename;
        }

        //private static string GetCSVFileName(string ModuleName)
        //{
        //    string CSVFileName = ModuleName;
        //    string dateFormat = "yyyy/MM/dd";
        //    //CSVFileName = SessionHelper.CompanyID + "_" + SessionHelper.RoomID + "_" + DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy HH:mm:ss").Replace("/", "").Replace(":", "") + ModuleName + ".csv";
        //    CSVFileName = ModuleName + "_" + DateTimeUtility.DateTimeNow.ToString(dateFormat).Replace("/", "") + ".csv";
        //    return CSVFileName;
        //}
        public string EnterpriseQuickListCSV(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            EnterpriseQuickListDAL obj = new EnterpriseQuickListDAL(SessionHelper.EnterPriseDBName);
            var lstEnterpriseQLDTO = obj.GetAllEnterpriseQuickListWiseLineItem(ids).OrderBy(SortNameString).ToList();

            StreamWriter write = new StreamWriter(filePath);

            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("id");
            csw.WriteField("* Name");
            csw.WriteField("* QLDetailNumber");
            csw.WriteField("* Quantity");

            csw.NextRecord();
            if (lstEnterpriseQLDTO != null && lstEnterpriseQLDTO.Count > 0)
            {
                foreach (EnterpriseQLExportDTO rec in ConvertExportDataList<EnterpriseQLExportDTO>(lstEnterpriseQLDTO).ToList())
                {
                    csw.WriteField(rec.ID);
                    csw.WriteField(rec.Name);
                    csw.WriteField(rec.QLDetailNumber);
                    csw.WriteField(rec.Quantity);

                    csw.NextRecord();
                }
            }
            write.Close();

            return filename;
        }

        public List<t> ConvertExportDataList<t>(object lstreturn)
        {
            foreach (var obj in (List<t>)lstreturn)
            {
                string value = string.Empty;
                foreach (PropertyInfo pi in ((t)obj).GetType().GetProperties().Where(x => x.PropertyType == typeof(string)
                                                && x.CanRead && x.CanWrite))
                {
                    value = pi.GetValue(obj, null) as string;
                    if (!string.IsNullOrEmpty(value))
                        pi.SetValue(obj, "'" + value, null);
                }
            }
            return (List<t>)lstreturn;
        }

        public string QuoteMasterChangeLogCSV(string Filepath, string ModuleName, string ids, long RoomID, long companyID, string SortNameString)
        {
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            long QuoteId;
            long.TryParse(arrid[0], out QuoteId);

            QuoteMasterDAL quoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            var lstoflogs = quoteMasterDAL.GetPagedQuoteMasterChangeLog(QuoteId, 0, int.MaxValue, out TotalRecordCount, string.Empty, SortNameString).ToList();

            if (lstoflogs != null && lstoflogs.Count() > 0)
            {
                //------------Create Header--------------------
                StreamWriter write = new StreamWriter(filePath);
                CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);

                csw.WriteField(ResCommon.HistoryAction);
                csw.WriteField(ResQuoteMaster.QuoteNumber);
                csw.WriteField("From Where");
                csw.WriteField(ResQuoteMaster.ReleaseNumber);
                csw.WriteField(ResQuoteMaster.Comment);
                csw.WriteField(ResQuoteMaster.RequiredDate);
                csw.WriteField(ResQuoteMaster.QuoteStatus);
                csw.WriteField(ResQuoteMaster.NoOfLineItems);
                csw.WriteField(ResQuoteMaster.QuoteCost);
                csw.WriteField(ResQuoteMaster.ChangeQuoteRevisionNo);
                csw.WriteField(ResCommon.CreatedOn);
                csw.WriteField(ResCommon.CreatedBy);
                csw.WriteField(ResCommon.UpdatedOn);
                csw.WriteField(ResCommon.UpdatedBy);
                csw.WriteField(ResCommon.RoomName);
                csw.WriteField(ResCommon.IsDeleted);
                csw.WriteField(ResCommon.AddedFrom);
                csw.WriteField(ResCommon.EditedFrom);
                csw.WriteField(ResCommon.ReceivedOnDate);
                csw.WriteField(ResCommon.ReceivedOnWebDate);
                csw.NextRecord();
                //------------End-------------("CreatedBy");-------

                //-------------Set Row Value---------------------------
                foreach (var item in lstoflogs)
                {
                    string noOfLineItems = item.NoOfLineItems.HasValue ? Convert.ToString(item.NoOfLineItems) : string.Empty;
                    string quoteCost = item.QuoteCost.HasValue ? Convert.ToString(item.QuoteCost) : string.Empty;
                    string changeQuoteRevisionNo = item.ChangeQuoteRevisionNo.HasValue ? Convert.ToString(item.ChangeQuoteRevisionNo) : string.Empty;

                    csw.WriteField(item.Action);
                    csw.WriteField(item.QuoteNumber);
                    csw.WriteField(item.WhatWhereAction);
                    csw.WriteField(item.ReleaseNumber);
                    csw.WriteField(item.Comment);
                    csw.WriteField(item.RequiredDateStr);
                    csw.WriteField(item.QuoteStatusChar);
                    csw.WriteField(noOfLineItems);
                    csw.WriteField(quoteCost);
                    csw.WriteField(changeQuoteRevisionNo);
                    csw.WriteField(item.CreatedDate);
                    csw.WriteField(item.CreatedByName);
                    csw.WriteField(item.UpdatedDate);
                    csw.WriteField(item.UpdatedByName);
                    csw.WriteField(item.RoomName);
                    csw.WriteField((item.IsDeleted == true ? "Yes" : "No"));
                    csw.WriteField(item.AddedFrom);
                    csw.WriteField(item.EditedFrom);
                    csw.WriteField(item.ReceivedOnDate);
                    csw.WriteField(item.ReceivedOnDateWeb);
                    csw.NextRecord();
                    //-------------End--------------------------
                }
                write.Close();
            }
            return filename;

        }

        #region Supplier Catalog List
        public string SupplierCatalogListCSV(string Filepath, string ModuleName, string ids, string SortNameString, string CallFromPage = "")
        {
            //string filename = SessionHelper.CompanyID + "_" + SessionHelper.RoomID + "_" + DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy HH:mm:ss").Replace("/", "").Replace(":", "") + ModuleName + ".csv";
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> lstIds = arrid.Select(t => Guid.Parse(t)).ToList();
            CatalogItemMasterDAL objCatalogItemMasterDAL = new CatalogItemMasterDAL(SessionHelper.EnterPriseDBName);
            List<SupplierMasterDTO> lstSupplier = new List<SupplierMasterDTO>();
            var objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
            {

                var strSupplierIds = string.Join(",", SessionHelper.UserSupplierIds);
                var suppliers = objSupDAL.GetSupplierByIDsPlain(strSupplierIds, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (suppliers != null && suppliers.Any())
                {
                    lstSupplier.AddRange(suppliers);
                }

                if (lstSupplier != null && lstSupplier.Any() && lstSupplier.Count() > 0)
                {
                    lstSupplier = lstSupplier.OrderBy(x => x.SupplierName).ToList();
                }
            }
            else
            {
                lstSupplier = objSupDAL.GetSupplierByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false).OrderBy(x => x.SupplierName).ToList();
            }
            string SupplierNames = string.Empty;
            if (lstSupplier != null && lstSupplier.Count > 0)
            {
                SupplierNames = string.Join(",", lstSupplier.Select(x => x.SupplierName));
            }
            List<SupplierCatalogItemDTO> supplierCatalogItems = objCatalogItemMasterDAL.SupplierCatalogsCSV(SupplierNames, null, SessionHelper.RoomID);

            List<ItemMasterDTO> lstItemMasterDTO = new List<ItemMasterDTO>();
            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField("* ItemNumber");
            csw.WriteField("Description");
            csw.WriteField("SellPrice");
            csw.WriteField("ManufacturerPartNumber");
            csw.WriteField("ImagePath");
            csw.WriteField("UPC");
            csw.WriteField("SupplierPartNumber");
            csw.WriteField("* SupplierName");
            csw.WriteField("ManufacturerName");
            csw.WriteField("UOM");
            csw.WriteField("CostUOM");
            csw.WriteField("Cost");
            csw.WriteField("UNSPSC");
            csw.WriteField("Category");
            csw.WriteField("LongDescription");
            csw.NextRecord();
            if (supplierCatalogItems != null && supplierCatalogItems.Count > 0)
            {
                foreach (SupplierCatalogItemDTO recrd in ConvertExportDataList<SupplierCatalogItemDTO>(supplierCatalogItems).ToList())
                {
                    csw.WriteField(recrd.ItemNumber);
                    csw.WriteField(recrd.Description);
                    csw.WriteField(recrd.SellPrice);
                    csw.WriteField(recrd.ManufacturerPartNumber);
                    csw.WriteField(recrd.ImagePath);
                    csw.WriteField(recrd.UPC);
                    csw.WriteField(recrd.SupplierPartNumber);
                    csw.WriteField(recrd.SupplierName);
                    csw.WriteField(recrd.ManufacturerName);
                    csw.WriteField(recrd.UOM);
                    csw.WriteField(recrd.CostUOM);
                    csw.WriteField(recrd.Cost);
                    csw.WriteField(recrd.UNSPSC);
                    csw.WriteField(recrd.LongDescription);
                    csw.WriteField(recrd.Category);
                    csw.NextRecord();
                }
            }
            write.Close();
            write.Dispose();
            return filename;
        }
        #endregion

        #region Permission Template ChangeLog CSV
        public string PermissionTemplateChangeLogCSV(string Filepath, string ModuleName, string ids, string SortNameString, string CallFromPage = "")
        {
            //string filename = SessionHelper.CompanyID + "_" + SessionHelper.RoomID + "_" + DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy HH:mm:ss").Replace("/", "").Replace(":", "") + ModuleName + ".csv";
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            string filePath = Filepath + filename;
            string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //List<Guid> lstIds = arrid.Select(t => Guid.Parse(t)).ToList();
            //CatalogItemMasterDAL objCatalogItemMasterDAL = new CatalogItemMasterDAL(SessionHelper.EnterPriseDBName);
            //List<SupplierMasterDTO> lstSupplier = new List<SupplierMasterDTO>();
            //var objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            long TemplateId;
            long.TryParse(arrid[0], out TemplateId);

            PermissionTemplateDAL permissionTemplateDAL = new PermissionTemplateDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            var lstoflogs = permissionTemplateDAL.GetPagedPermissionTemplateChangeLog(TemplateId, 0, int.MaxValue, out TotalRecordCount, string.Empty, SortNameString).ToList();
            lstoflogs.ToList().ForEach(t =>
            {

                object GetvalC = t.GetType().GetProperty("Created").GetValue(t, null);
                object GetvalU = t.GetType().GetProperty("Updated").GetValue(t, null); ;
                if (GetvalC != null)
                {
                    string sc = CommonUtility.ConvertDateByTimeZone((DateTime)GetvalC, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    t.GetType().GetProperty("CreatedDate").SetValue(t, sc, null);
                }
                if (GetvalU != null)
                {
                    string su = CommonUtility.ConvertDateByTimeZone((DateTime)GetvalU, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    t.GetType().GetProperty("UpdatedDate").SetValue(t, su, null);
                }
            });

            StreamWriter write = new StreamWriter(filePath);
            CsvWriter csw = new CsvWriter(write, ResourceHelper.CurrentCult);
            csw.WriteField(ResCommon.HistoryID);
            csw.WriteField(ResCommon.HistoryAction);
            csw.WriteField(ResPermissionTemplate.TemplateName);
            csw.WriteField(ResPermissionTemplate.Description);
            csw.WriteField(ResCommon.CreatedOn);
            csw.WriteField(ResCommon.UpdatedOn);
            csw.WriteField(ResCommon.LastUpdatedBy);
            csw.WriteField(ResCommon.CreatedBy);
            csw.NextRecord();
            if (lstoflogs != null && lstoflogs.Count > 0)
            {
                foreach (PermissionTemplateDTO recrd in ConvertExportDataList<PermissionTemplateDTO>(lstoflogs).ToList())
                {
                    csw.WriteField(recrd.HistoryID);
                    csw.WriteField(recrd.Action);
                    csw.WriteField(recrd.TemplateName);
                    csw.WriteField(recrd.Description);
                    csw.WriteField(recrd.CreatedDate);
                    csw.WriteField(recrd.UpdatedDate);
                    csw.WriteField(recrd.UpdatedByName);
                    csw.WriteField(recrd.CreatedByName);
                    csw.NextRecord();
                }
            }
            write.Close();
            write.Dispose();
            return filename;
        }
        #endregion

    }
}