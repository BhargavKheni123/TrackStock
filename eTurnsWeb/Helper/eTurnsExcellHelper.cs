using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using eTurns.DTO.Resources;

namespace eTurnsWeb.Helper
{
    public class eTurnsExcellHelper
    {
        public string ExcelMain(string Filepath, string ModuleName, string ids, string SortNameString, string BinIDs)
        {
            string retval = string.Empty;
            if (ModuleName != null && ModuleName.ToLower().Trim() == "toolmaster" && SessionHelper.AllowToolOrdering == true)
            {
                ModuleName = "ToolListNew";
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
                        retval = CategoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "BinMasterList":
                        retval = InventoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CustomerMasterList":
                        retval = CustomerMasterListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CostUOMMasterList":
                        retval = CostUOMListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "InventoryClassificationMasterList":
                        retval = InventoryClassificationListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    //case "FreightTypeMasterList":
                    //    retval = FreightTypeListExcel(Filepath, ModuleName, ids, SortNameString);
                    //    break;
                    case "GLAccountMasterList":
                        retval = GLAccountListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "SupplierMasterList":
                        retval = SupplierListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ShipViaMasterList":
                        retval = ShipViaMasterListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "TechnicianList":
                        retval = TechnicianListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "UnitMasterList":
                        retval = UnitMasterListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "LocationMasterList":
                        retval = LocationMasterListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolCategoryList":
                        retval = ToolCategoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "AssetCategoryList":
                        retval = AssetCategoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ManufacturerMasterList":
                        retval = ManufacturerMasterListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "MeasurementTermList":
                        retval = MeasurementTermListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolList":
                        retval = ToolListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolHistoryList":
                        retval = ToolHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "AssetMasterList":
                        retval = AssetMasterListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ItemMasterList":
                        retval = ItemMasterListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "DashboardItemStockOutList":
                        retval = AlertStockOutListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ItemBinMasterList":
                        retval = ItemBinMasterListExcel(Filepath, ModuleName, ids, SortNameString, BinIDs);
                        break;
                    case "QuickList":
                        retval = QuickListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "BOMItemMasterList":
                        retval = BOMItemMasterListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "WorkOrder":
                        retval = WorkOrderCountExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ItemLocationChangeImport":
                        retval = ItemLocationChangeImportEXCEL(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "DashboardItemMinimumList":
                    case "DashboardItemMaximumList":
                    case "DashboardItemCriticalList":
                        retval = DashboardItemMinMaxListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "MinMaxTuningList":
                        retval = MinMaxTuningList(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "TuningList":
                        retval = TuningList(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolMasterNew":
                    case "ToolListNew":
                        retval = ToolMasterListCSVNew(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ShipViaChangeLog":
                        retval = ShipViaHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "EnterpriseChangeLog":
                        retval = EnterpriseHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "RoomChangeLog":
                        retval = RoomHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "UserChangeLog":
                        retval = UserHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CompanyChangeLog":
                        retval = CompanyHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "AssetCategoryChangeLog":
                        retval = AssetCategoryHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "BinChangeLog":
                        retval = BinHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CategoryChangeLog":
                        retval = CategoryHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CostUOMChangeLog":
                        retval = CostUOMHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CustomerChangeLog":
                        retval = CustomerHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "GLAccountChangeLog":
                        retval = GLAccountHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "InventoryClassificationChangeLog":
                        retval = InventoryClassificationHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "LocationChangeLog":
                        retval = LocationHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ManufacturerChangeLog":
                        retval = ManufacturerHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "SupplierChangeLog":
                        retval = SupplierHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "TechnicianChangeLog":
                        retval = TechnicianHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolCategoryChangeLog":
                        retval = ToolCategoryHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "UnitChangeLog":
                        retval = UnitHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "VenderChangeLog":
                        retval = VenderHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "QuickListChangeLog":
                        retval = QuickListHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CountChangeLog":
                        retval = CountHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "MaterialStagingChangeLog":
                        retval = MaterialStagingHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "RequisitionChangeLog":
                        retval = RequisitionHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "WorkOrderChangeLog":
                        retval = WorkOrderHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ProjectSpendChangeLog":
                        retval = ProjectSpendHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "CartItemsChangeLog":
                        retval = CartItemsHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "OrdersChangeLog":
                        retval = OrdersHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "TransferChangeLog":
                        retval = TransferHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolChangeLog":
                        retval = Asset_ToolHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "AssetsChangeLog":
                        retval = AssetsHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "NotificationMasterList":
                        retval = NotificationListExcel(Filepath, ModuleName, ids, ResourceHelper.CurrentCult.Name);
                        break;
                    case "NotificationMasterListChangeLog":
                        retval = NotificationHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ToolsMaintenanceList":
                        retval = ToolsMaintenanceListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "FTPChangeLog":
                        retval = FTPHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "DeletedRoom":
                        retval = DeletedRoomListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "EnterpriseQuickList":
                        retval = EnterpriseQuickListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "WrittenOffCategoryChangeLog":
                        retval = WrittenOffCategoryHistoryListExcel(Filepath, ModuleName, ids, SortNameString);
                        break;
                    case "ItemMasterChangeLog":
                        retval = ItemMasterChangeLogExcel(Filepath, ModuleName, ids,SessionHelper.RoomID,SessionHelper.CompanyID);
                        break;
                    case "QuoteChangeLog":
                        retval = QuoteMasterChangeLogExcel(Filepath, ModuleName, ids, SessionHelper.RoomID, SessionHelper.CompanyID, SortNameString);
                        break;
                    case "PermissionTemplateChangeLog":
                        retval = PermissionTemplateChangeLogExcel(Filepath, ModuleName, ids, SessionHelper.RoomID, SessionHelper.CompanyID, SortNameString);
                        break;
                        //case "ToolCheckoutStatusNew":
                        //    retval = ToolCheckoutStatusListCSVNew(Filepath, ModuleName, ids, SortNameString, out isRecordAvail);
                        //    if (isRecordAvail == false)
                        //    {
                        //        retval = eTurns.DTO.Resources.ResMessage.ExportCheckoutMessage; //"No record found for checkout.";
                        //    }
                        //    break;
                }

            }
            return retval;

        }
        static HSSFWorkbook hssfworkbook;

        static void WriteToFile(string Filepath, string ModuleName)
        {
            //Write the stream data of workbook to the root directory

            FileStream file = new FileStream(Filepath, FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();
        }

        static void InitializeWorkbook()
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;
        }
        public string CategoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            CategoryMasterDAL obj = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
            string[] arrid = ids.Split(',');
            //var DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).OrderBy(SortNameString);
            //var DataFromDB = obj.GetCategoriesByIdsByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, ids).OrderBy(SortNameString);
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
            //                            where arrid.Contains(c.ID.ToString())
            //                            select new CategoryMasterDTO
            //                            {
            //                                ID = c.ID,
            //                                Category = c.Category,
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

            CategoryMasterDTO objCategoryMasterDTO = new CategoryMasterDTO();

            if (lstCategoryMasterDTO != null && lstCategoryMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* CATEGORY");
                row.CreateCell(2).SetCellValue("UDF1");
                row.CreateCell(3).SetCellValue("UDF2");
                row.CreateCell(4).SetCellValue("UDF3");
                row.CreateCell(5).SetCellValue("UDF4");
                row.CreateCell(6).SetCellValue("UDF5");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (CategoryMasterDTO item in lstCategoryMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID.ToString());
                    DRow.CreateCell(1).SetCellValue(item.Category);
                    DRow.CreateCell(2).SetCellValue(item.UDF1);
                    DRow.CreateCell(3).SetCellValue(item.UDF2);
                    DRow.CreateCell(4).SetCellValue(item.UDF3);
                    DRow.CreateCell(5).SetCellValue(item.UDF4);
                    DRow.CreateCell(6).SetCellValue(item.UDF5);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string InventoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<BinMasterDTO> DataFromDB = objBinMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            //IEnumerable<BinMasterDTO> DataFromDB = objBinMasterDAL.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            IEnumerable<BinMasterDTO> lstBinMasterDTO = new List<BinMasterDTO>();
            if (!string.IsNullOrEmpty(ids))
            {
                lstBinMasterDTO = objBinMasterDAL.GetBinMasterbyIds(SessionHelper.RoomID, SessionHelper.CompanyID, ids).OrderBy(SortNameString);
            }
            BinMasterDTO objBinMasterDTO = new BinMasterDTO();

            if (lstBinMasterDTO != null && lstBinMasterDTO.Count() > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* LOCATION");
                row.CreateCell(2).SetCellValue("UDF1");
                row.CreateCell(3).SetCellValue("UDF2");
                row.CreateCell(4).SetCellValue("UDF3");
                row.CreateCell(5).SetCellValue("UDF4");
                row.CreateCell(6).SetCellValue("UDF5");
                //row.CreateCell(7).SetCellValue("SensorId");
                //row.CreateCell(8).SetCellValue("SensorPort");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (BinMasterDTO item in lstBinMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID.ToString());
                    DRow.CreateCell(1).SetCellValue(item.BinNumber);
                    DRow.CreateCell(2).SetCellValue(item.UDF1);
                    DRow.CreateCell(3).SetCellValue(item.UDF2);
                    DRow.CreateCell(4).SetCellValue(item.UDF3);
                    DRow.CreateCell(5).SetCellValue(item.UDF4);
                    DRow.CreateCell(6).SetCellValue(item.UDF5);
                    //DRow.CreateCell(7).SetCellValue(item.eVMISensorID ?? 0);
                    //DRow.CreateCell(8).SetCellValue(item.eVMISensorPort);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string CustomerMasterListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");

            string[] arrid = ids.Split(',');
            CustomerMasterDAL obj = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            List<CustomerMasterDTO> lstCustomerMasterDTO = new List<CustomerMasterDTO>();
            lstCustomerMasterDTO = obj.GetCustomersByIDs(ids, SessionHelper.RoomID, SessionHelper.CompanyID);
            //IEnumerable<CustomerMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstCustomerMasterDTO = (from c in DataFromDB
            //                            where arrid.Contains(c.ID.ToString())
            //                            select new CustomerMasterDTO
            //                            {
            //                                ID = c.ID,
            //                                Customer = c.Customer,
            //                                Account = c.Account,
            //                                Address = c.Address,
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
            if (lstCustomerMasterDTO != null && lstCustomerMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* CUSTOMER");
                row.CreateCell(2).SetCellValue("* ACCOUNT");
                row.CreateCell(3).SetCellValue("CONTACT");
                row.CreateCell(4).SetCellValue("ADDRESS");
                row.CreateCell(5).SetCellValue("CITY");
                row.CreateCell(6).SetCellValue("STATE");
                row.CreateCell(7).SetCellValue("ZIPCODE");
                row.CreateCell(8).SetCellValue("COUNTRY");
                row.CreateCell(9).SetCellValue("PHONE");
                row.CreateCell(10).SetCellValue("EMAIL");
                row.CreateCell(11).SetCellValue("UDF1");
                row.CreateCell(12).SetCellValue("UDF2");
                row.CreateCell(13).SetCellValue("UDF3");
                row.CreateCell(14).SetCellValue("UDF4");
                row.CreateCell(15).SetCellValue("UDF5");
                row.CreateCell(16).SetCellValue("Remarks");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (CustomerMasterDTO item in lstCustomerMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.Customer);
                    DRow.CreateCell(2).SetCellValue(item.Account);
                    DRow.CreateCell(3).SetCellValue(item.Contact);
                    DRow.CreateCell(4).SetCellValue(item.Address);
                    DRow.CreateCell(5).SetCellValue(item.City);
                    DRow.CreateCell(6).SetCellValue(item.State);
                    DRow.CreateCell(7).SetCellValue(item.ZipCode);
                    DRow.CreateCell(8).SetCellValue(item.Country);
                    DRow.CreateCell(9).SetCellValue(item.Phone);
                    DRow.CreateCell(10).SetCellValue(item.Email);
                    DRow.CreateCell(11).SetCellValue(item.UDF1);
                    DRow.CreateCell(12).SetCellValue(item.UDF2);
                    DRow.CreateCell(13).SetCellValue(item.UDF3);
                    DRow.CreateCell(14).SetCellValue(item.UDF4);
                    DRow.CreateCell(15).SetCellValue(item.UDF5);
                    DRow.CreateCell(16).SetCellValue(item.Remarks);


                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }

        public string CostUOMListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            CostUOMMasterDAL obj = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<CostUOMMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);

            List<CostUOMMasterDTO> lstCostUOMMasterDTO = new List<CostUOMMasterDTO>();
            lstCostUOMMasterDTO = obj.GetCostUOMsByIDs(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstCostUOMMasterDTO = (from u in DataFromDB
            //                           where arrid.Contains(u.ID.ToString())
            //                           select new CostUOMMasterDTO
            //                           {
            //                               ID = u.ID,
            //                               CostUOM = u.CostUOM,
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

            if (lstCostUOMMasterDTO != null && lstCostUOMMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* CostUOM");
                row.CreateCell(2).SetCellValue("* CostUOMValue");
                row.CreateCell(3).SetCellValue("UDF1");
                row.CreateCell(4).SetCellValue("UDF2");
                row.CreateCell(5).SetCellValue("UDF3");
                row.CreateCell(6).SetCellValue("UDF4");
                row.CreateCell(7).SetCellValue("UDF5");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (CostUOMMasterDTO item in lstCostUOMMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.CostUOM);
                    DRow.CreateCell(2).SetCellValue(item.CostUOMValue ?? 0);
                    DRow.CreateCell(3).SetCellValue(item.UDF1);
                    DRow.CreateCell(4).SetCellValue(item.UDF2);
                    DRow.CreateCell(5).SetCellValue(item.UDF3);
                    DRow.CreateCell(6).SetCellValue(item.UDF4);
                    DRow.CreateCell(7).SetCellValue(item.UDF5);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string InventoryClassificationListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            InventoryClassificationMasterDAL obj = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<InventoryClassificationMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).OrderBy(SortNameString);
            List<InventoryClassificationMasterDTO> lstInventoryClassificationMasterDTO = new List<InventoryClassificationMasterDTO>();
            lstInventoryClassificationMasterDTO = obj.GetInventoryClassificationByIDsPlain(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstInventoryClassificationMasterDTO = (from u in DataFromDB
            //                                           where arrid.Contains(u.ID.ToString())
            //                                           select new InventoryClassificationMasterDTO
            //                                           {
            //                                               ID = u.ID,
            //                                               InventoryClassification = u.InventoryClassification,
            //                                               BaseOfInventory = u.BaseOfInventory,
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


            if (lstInventoryClassificationMasterDTO != null && lstInventoryClassificationMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* InventoryClassification");
                row.CreateCell(2).SetCellValue("* BaseOfInventory");
                row.CreateCell(3).SetCellValue("* RangeStart");
                row.CreateCell(4).SetCellValue("* RangeEnd");
                row.CreateCell(5).SetCellValue("UDF1");
                row.CreateCell(6).SetCellValue("UDF2");
                row.CreateCell(7).SetCellValue("UDF3");
                row.CreateCell(8).SetCellValue("UDF4");
                row.CreateCell(9).SetCellValue("UDF5");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (InventoryClassificationMasterDTO item in lstInventoryClassificationMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID.ToString());
                    DRow.CreateCell(1).SetCellValue(item.InventoryClassification);
                    DRow.CreateCell(2).SetCellValue(item.BaseOfInventory ?? string.Empty);
                    DRow.CreateCell(3).SetCellValue(item.RangeStart ?? 0);
                    DRow.CreateCell(4).SetCellValue(item.RangeEnd ?? 0);
                    DRow.CreateCell(5).SetCellValue(item.UDF1);
                    DRow.CreateCell(6).SetCellValue(item.UDF2);
                    DRow.CreateCell(7).SetCellValue(item.UDF3);
                    DRow.CreateCell(8).SetCellValue(item.UDF4);
                    DRow.CreateCell(9).SetCellValue(item.UDF5);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        //public string FreightTypeListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        //{
        //    //InitializeWorkbook();

        //    //ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
        //    //string[] arrid = ids.Split(',');
        //    ////FreightTypeMasterDAL obj = new FreightTypeMasterDAL(SessionHelper.EnterPriseDBName);
        //    ////IEnumerable<FreightTypeMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);

        //    ////List<FreightTypeMasterDTO> lstFreightTypeMasterDTO = new List<FreightTypeMasterDTO>();
        //    ////lstFreightTypeMasterDTO = obj.GetFreightTypesByIDs(ids, SessionHelper.RoomID, SessionHelper.CompanyID);
        //    ////if (!string.IsNullOrEmpty(ids))
        //    ////{
        //    ////    lstFreightTypeMasterDTO = (from u in DataFromDB
        //    ////                               where arrid.Contains(u.ID.ToString())
        //    ////                               select new FreightTypeMasterDTO
        //    ////                               {
        //    ////                                   FreightType = u.FreightType,
        //    ////                                   Created = u.Created,
        //    ////                                   CreatedBy = u.CreatedBy,
        //    ////                                   ID = u.ID,
        //    ////                                   LastUpdatedBy = u.LastUpdatedBy,
        //    ////                                   Room = u.Room,
        //    ////                                   LastUpdated = u.LastUpdated,
        //    ////                                   CreatedByName = u.CreatedByName,
        //    ////                                   UpdatedByName = u.UpdatedByName,
        //    ////                                   RoomName = u.RoomName,
        //    ////                                   GUID = u.GUID,
        //    ////                                   CompanyID = u.CompanyID,
        //    ////                                   IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
        //    ////                                   IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
        //    ////                                   UDF1 = u.UDF1,
        //    ////                                   UDF2 = u.UDF2,
        //    ////                                   UDF3 = u.UDF3,
        //    ////                                   UDF4 = u.UDF4,
        //    ////                                   UDF5 = u.UDF5
        //    ////                               }).ToList();
        //    ////}

        //    //if (lstFreightTypeMasterDTO != null && lstFreightTypeMasterDTO.Count > 0)
        //    //{
        //    //    //------------Create Header--------------------
        //    //    IRow row = sheet1.CreateRow(0);
        //    //    row.CreateCell(0).SetCellValue("id");
        //    //    row.CreateCell(1).SetCellValue("* FREIGHTTYPE");
        //    //    row.CreateCell(2).SetCellValue("UDF1");
        //    //    row.CreateCell(3).SetCellValue("UDF2");
        //    //    row.CreateCell(4).SetCellValue("UDF3");
        //    //    row.CreateCell(5).SetCellValue("UDF4");
        //    //    row.CreateCell(6).SetCellValue("UDF5");
        //    //    //------------End--------------------

        //    //    //-------------Set Row Value---------------------------
        //    //    int RowId = 1;
        //    //    foreach (FreightTypeMasterDTO item in lstFreightTypeMasterDTO)
        //    //    {
        //    //        IRow DRow = sheet1.CreateRow(RowId);

        //    //        DRow.CreateCell(0).SetCellValue(item.ID.ToString());
        //    //        DRow.CreateCell(1).SetCellValue(item.FreightType);
        //    //        DRow.CreateCell(2).SetCellValue(item.UDF1);
        //    //        DRow.CreateCell(3).SetCellValue(item.UDF2);
        //    //        DRow.CreateCell(4).SetCellValue(item.UDF3);
        //    //        DRow.CreateCell(5).SetCellValue(item.UDF4);
        //    //        DRow.CreateCell(6).SetCellValue(item.UDF5);

        //    //        RowId += 1;
        //    //        //-------------End--------------------------
        //    //    }

        //    //}
        //    //string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
        //    //string FilePath = Filepath + filename;
        //    //WriteToFile(FilePath, ModuleName);
        //    //return filename;
        //}
        public string GLAccountListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            GLAccountMasterDAL obj = new GLAccountMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<GLAccountMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).OrderBy(SortNameString);
            List<GLAccountMasterDTO> lstGLAccountMasterDTO = new List<GLAccountMasterDTO>();
            lstGLAccountMasterDTO = obj.GetGLAccountsByIDs(ids, SessionHelper.RoomID, SessionHelper.CompanyID);
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstGLAccountMasterDTO = (from c in DataFromDB
            //                             where arrid.Contains(c.ID.ToString())
            //                             select new GLAccountMasterDTO
            //                             {
            //                                 ID = c.ID,
            //                                 GLAccount = c.GLAccount,
            //                                 Description = c.Description,
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
            if (lstGLAccountMasterDTO != null && lstGLAccountMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* GLAccount");
                row.CreateCell(2).SetCellValue("Description");
                row.CreateCell(3).SetCellValue("UDF1");
                row.CreateCell(4).SetCellValue("UDF2");
                row.CreateCell(5).SetCellValue("UDF3");
                row.CreateCell(6).SetCellValue("UDF4");
                row.CreateCell(7).SetCellValue("UDF5");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (GLAccountMasterDTO item in lstGLAccountMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID.ToString());
                    DRow.CreateCell(1).SetCellValue(item.GLAccount);
                    DRow.CreateCell(2).SetCellValue(item.Description);
                    DRow.CreateCell(3).SetCellValue(item.UDF1);
                    DRow.CreateCell(4).SetCellValue(item.UDF2);
                    DRow.CreateCell(5).SetCellValue(item.UDF3);
                    DRow.CreateCell(6).SetCellValue(item.UDF4);
                    DRow.CreateCell(7).SetCellValue(item.UDF5);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string SupplierListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            //string[] arrid = ids.Split(',');
            SupplierMasterDAL obj = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

            List<SupplierMasterDTO> lstSupplierMasterDTO = obj.GetExportSupplierListByIDsFull(ids, SessionHelper.RoomID, SessionHelper.CompanyID).ToList().OrderBy(SortNameString).ToList();


            if (lstSupplierMasterDTO != null && lstSupplierMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* SUPPLIERNAME");
                row.CreateCell(2).SetCellValue("SupplierColor");
                row.CreateCell(3).SetCellValue("Description");
                //row.CreateCell(3).SetCellValue("ReceiverID");
                row.CreateCell(4).SetCellValue("BranchNumber");
                row.CreateCell(5).SetCellValue("MaximumOrderSize");
                row.CreateCell(6).SetCellValue("ADDRESS");
                row.CreateCell(7).SetCellValue("CITY");
                row.CreateCell(8).SetCellValue("STATE");
                row.CreateCell(9).SetCellValue("ZIPCODE");
                row.CreateCell(10).SetCellValue("COUNTRY");
                row.CreateCell(11).SetCellValue("CONTACT");
                row.CreateCell(12).SetCellValue("PHONE");
                row.CreateCell(13).SetCellValue("Fax");
                row.CreateCell(11).SetCellValue("EMAIL");
                //row.CreateCell(13).SetCellValue("IsEmailPOInBody");
                //row.CreateCell(14).SetCellValue("IsEmailPOInPDF");
                //row.CreateCell(15).SetCellValue("IsEmailPOInCSV");
                //row.CreateCell(16).SetCellValue("IsEmailPOInX12");
                row.CreateCell(14).SetCellValue("IsSendtoVendor");
                row.CreateCell(15).SetCellValue("IsVendorReturnAsn");
                row.CreateCell(16).SetCellValue("IsSupplierReceivesKitComponents");
                row.CreateCell(17).SetCellValue("OrderNumberTypeBlank");
                row.CreateCell(18).SetCellValue("OrderNumberTypeFixed");
                row.CreateCell(19).SetCellValue("OrderNumberTypeBlanketOrderNumber");
                row.CreateCell(20).SetCellValue("OrderNumberTypeIncrementingOrderNumber");
                row.CreateCell(21).SetCellValue("OrderNumberTypeIncrementingbyDay");
                row.CreateCell(22).SetCellValue("OrderNumberTypeDateIncrementing");
                row.CreateCell(23).SetCellValue("OrderNumberTypeDate");
                row.CreateCell(24).SetCellValue("UDF1");
                row.CreateCell(25).SetCellValue("UDF2");
                row.CreateCell(26).SetCellValue("UDF3");
                row.CreateCell(27).SetCellValue("UDF4");
                row.CreateCell(28).SetCellValue("UDF5");
                row.CreateCell(29).SetCellValue("AccountNumber");
                row.CreateCell(30).SetCellValue("AccountName");

                row.CreateCell(31).SetCellValue("AccountAddress");
                row.CreateCell(32).SetCellValue("AccountCity");
                row.CreateCell(33).SetCellValue("AccountState");
                row.CreateCell(34).SetCellValue("AccountZip");

                row.CreateCell(35).SetCellValue("AccountCountry");
                row.CreateCell(36).SetCellValue("AccountShipToID");

                row.CreateCell(37).SetCellValue("AccountIsDefault");

                row.CreateCell(38).SetCellValue("BlanketPONumber");
                row.CreateCell(39).SetCellValue("StartDate");
                row.CreateCell(40).SetCellValue("EndDate");
                row.CreateCell(41).SetCellValue("MaxLimit");
                row.CreateCell(42).SetCellValue("DoNotExceed");
                row.CreateCell(43).SetCellValue("MaxLimitQty");
                row.CreateCell(44).SetCellValue("DoNotExceedQty");

                row.CreateCell(45).SetCellValue("IsBlanketDeleted");

                row.CreateCell(46).SetCellValue("SupplierImage");
                row.CreateCell(47).SetCellValue("ImageExternalURL");

                /*
                    csw.WriteField("SupplierImage");
            csw.WriteField("ImageExternalURL");
                 */

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (SupplierMasterDTO item in lstSupplierMasterDTO)
                {
                    int maxCount = item.SupplierAccountDetails.Count > item.SupplierBlanketPODetails.Count ? item.SupplierAccountDetails.Count : item.SupplierBlanketPODetails.Count;

                    if (maxCount == 0)
                    {
                        IRow DRow = sheet1.CreateRow(RowId);

                        DRow.CreateCell(0).SetCellValue(item.ID.ToString());
                        DRow.CreateCell(1).SetCellValue(item.SupplierName);
                        DRow.CreateCell(2).SetCellValue(item.SupplierColor);
                        DRow.CreateCell(3).SetCellValue(item.Description);
                        //DRow.CreateCell(3).SetCellValue(item.ReceiverID);
                        DRow.CreateCell(4).SetCellValue(item.BranchNumber);
                        if (item.MaximumOrderSize.HasValue)
                            DRow.CreateCell(5).SetCellValue(Convert.ToDouble(item.MaximumOrderSize.Value));
                        else
                            DRow.CreateCell(5).SetCellValue(0);

                        DRow.CreateCell(6).SetCellValue(item.Address);
                        DRow.CreateCell(7).SetCellValue(item.City);
                        DRow.CreateCell(8).SetCellValue(item.State);
                        DRow.CreateCell(9).SetCellValue(item.ZipCode);
                        DRow.CreateCell(10).SetCellValue(item.Country);
                        DRow.CreateCell(11).SetCellValue(item.Contact);
                        DRow.CreateCell(12).SetCellValue(item.Phone);
                        DRow.CreateCell(13).SetCellValue(item.Fax);
                        DRow.CreateCell(11).SetCellValue(item.Email);
                        //DRow.CreateCell(13).SetCellValue(item.IsEmailPOInBody);
                        //DRow.CreateCell(14).SetCellValue(item.IsEmailPOInPDF);
                        //DRow.CreateCell(15).SetCellValue(item.IsEmailPOInCSV);
                        //DRow.CreateCell(16).SetCellValue(item.IsEmailPOInX12);
                        DRow.CreateCell(14).SetCellValue(item.IsSendtoVendor);
                        DRow.CreateCell(15).SetCellValue(item.IsVendorReturnAsn);
                        DRow.CreateCell(16).SetCellValue(item.IsSupplierReceivesKitComponents);

                        if (item.POAutoSequence.HasValue)
                        {
                            if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeBlank)
                            {
                                DRow.CreateCell(17).SetCellValue("TRUE");
                                DRow.CreateCell(18).SetCellValue("FALSE");
                                DRow.CreateCell(19).SetCellValue("FALSE");
                                DRow.CreateCell(20).SetCellValue("FALSE");
                                DRow.CreateCell(21).SetCellValue("FALSE");
                                DRow.CreateCell(22).SetCellValue("FALSE");
                                DRow.CreateCell(23).SetCellValue("FALSE");
                            }
                            else if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeFixed)
                            {
                                DRow.CreateCell(17).SetCellValue("FALSE");
                                DRow.CreateCell(18).SetCellValue("TRUE");
                                DRow.CreateCell(19).SetCellValue("FALSE");
                                DRow.CreateCell(20).SetCellValue("FALSE");
                                DRow.CreateCell(21).SetCellValue("FALSE");
                                DRow.CreateCell(22).SetCellValue("FALSE");
                                DRow.CreateCell(23).SetCellValue("FALSE");
                            }
                            else if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeBlanketOrderNumber)
                            {
                                DRow.CreateCell(17).SetCellValue("FALSE");
                                DRow.CreateCell(18).SetCellValue("FALSE");
                                DRow.CreateCell(19).SetCellValue("TRUE");
                                DRow.CreateCell(20).SetCellValue("FALSE");
                                DRow.CreateCell(21).SetCellValue("FALSE");
                                DRow.CreateCell(22).SetCellValue("FALSE");
                                DRow.CreateCell(23).SetCellValue("FALSE");
                            }
                            else if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeIncrementingOrderNumber)
                            {
                                DRow.CreateCell(17).SetCellValue("FALSE");
                                DRow.CreateCell(18).SetCellValue("FALSE");
                                DRow.CreateCell(19).SetCellValue("FALSE");
                                DRow.CreateCell(20).SetCellValue("TRUE");
                                DRow.CreateCell(21).SetCellValue("FALSE");
                                DRow.CreateCell(22).SetCellValue("FALSE");
                                DRow.CreateCell(23).SetCellValue("FALSE");
                            }
                            else if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeIncrementingbyDay)
                            {
                                DRow.CreateCell(17).SetCellValue("FALSE");
                                DRow.CreateCell(18).SetCellValue("FALSE");
                                DRow.CreateCell(19).SetCellValue("FALSE");
                                DRow.CreateCell(20).SetCellValue("FALSE");
                                DRow.CreateCell(21).SetCellValue("TRUE");
                                DRow.CreateCell(22).SetCellValue("FALSE");
                                DRow.CreateCell(23).SetCellValue("FALSE");
                            }
                            else if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeDateIncrementing)
                            {
                                DRow.CreateCell(17).SetCellValue("FALSE");
                                DRow.CreateCell(18).SetCellValue("FALSE");
                                DRow.CreateCell(19).SetCellValue("FALSE");
                                DRow.CreateCell(20).SetCellValue("FALSE");
                                DRow.CreateCell(21).SetCellValue("FALSE");
                                DRow.CreateCell(22).SetCellValue("TRUE");
                                DRow.CreateCell(23).SetCellValue("FALSE");
                            }
                            else if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeDate)
                            {
                                DRow.CreateCell(17).SetCellValue("FALSE");
                                DRow.CreateCell(18).SetCellValue("FALSE");
                                DRow.CreateCell(19).SetCellValue("FALSE");
                                DRow.CreateCell(20).SetCellValue("FALSE");
                                DRow.CreateCell(21).SetCellValue("FALSE");
                                DRow.CreateCell(22).SetCellValue("FALSE");
                                DRow.CreateCell(23).SetCellValue("TRUE");
                            }
                        }
                        else
                        {
                            DRow.CreateCell(17).SetCellValue("FALSE");
                            DRow.CreateCell(18).SetCellValue("FALSE");
                            DRow.CreateCell(19).SetCellValue("FALSE");
                            DRow.CreateCell(20).SetCellValue("FALSE");
                            DRow.CreateCell(21).SetCellValue("FALSE");
                            DRow.CreateCell(22).SetCellValue("FALSE");
                            DRow.CreateCell(23).SetCellValue("FALSE");
                        }

                        DRow.CreateCell(24).SetCellValue(item.UDF1);
                        DRow.CreateCell(25).SetCellValue(item.UDF2);
                        DRow.CreateCell(26).SetCellValue(item.UDF3);
                        DRow.CreateCell(27).SetCellValue(item.UDF4);
                        DRow.CreateCell(28).SetCellValue(item.UDF5);

                        DRow.CreateCell(29).SetCellValue(string.Empty);
                        DRow.CreateCell(30).SetCellValue(string.Empty);

                        RowId += 1;
                    }

                    for (int i = 0; i < maxCount; i++)
                    {
                        IRow DRow = sheet1.CreateRow(RowId);

                        DRow.CreateCell(0).SetCellValue(item.ID.ToString());
                        DRow.CreateCell(1).SetCellValue(item.SupplierName);
                        DRow.CreateCell(2).SetCellValue(item.SupplierColor);
                        DRow.CreateCell(3).SetCellValue(item.Description);
                        //DRow.CreateCell(3).SetCellValue(item.ReceiverID);
                        DRow.CreateCell(4).SetCellValue(item.BranchNumber);
                        if (item.MaximumOrderSize.HasValue)
                            DRow.CreateCell(5).SetCellValue(Convert.ToDouble(item.MaximumOrderSize.Value));
                        else
                            DRow.CreateCell(5).SetCellValue(0);

                        DRow.CreateCell(6).SetCellValue(item.Address);
                        DRow.CreateCell(7).SetCellValue(item.City);
                        DRow.CreateCell(8).SetCellValue(item.State);
                        DRow.CreateCell(9).SetCellValue(item.ZipCode);
                        DRow.CreateCell(10).SetCellValue(item.Country);
                        DRow.CreateCell(11).SetCellValue(item.Contact);
                        DRow.CreateCell(12).SetCellValue(item.Phone);
                        DRow.CreateCell(13).SetCellValue(item.Fax);
                        DRow.CreateCell(11).SetCellValue(item.Email);
                        //DRow.CreateCell(13).SetCellValue(item.IsEmailPOInBody);
                        //DRow.CreateCell(14).SetCellValue(item.IsEmailPOInPDF);
                        //DRow.CreateCell(15).SetCellValue(item.IsEmailPOInCSV);
                        //DRow.CreateCell(16).SetCellValue(item.IsEmailPOInX12);
                        DRow.CreateCell(14).SetCellValue(item.IsSendtoVendor);
                        DRow.CreateCell(15).SetCellValue(item.IsVendorReturnAsn);
                        DRow.CreateCell(16).SetCellValue(item.IsSupplierReceivesKitComponents);

                        if (item.POAutoSequence.HasValue)
                        {
                            if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeBlank)
                            {
                                DRow.CreateCell(17).SetCellValue("TRUE");
                                DRow.CreateCell(18).SetCellValue("FALSE");
                                DRow.CreateCell(19).SetCellValue("FALSE");
                                DRow.CreateCell(20).SetCellValue("FALSE");
                                DRow.CreateCell(21).SetCellValue("FALSE");
                                DRow.CreateCell(22).SetCellValue("FALSE");
                                DRow.CreateCell(23).SetCellValue("FALSE");
                            }
                            else if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeFixed)
                            {
                                DRow.CreateCell(17).SetCellValue("FALSE");
                                DRow.CreateCell(18).SetCellValue("TRUE");
                                DRow.CreateCell(19).SetCellValue("FALSE");
                                DRow.CreateCell(20).SetCellValue("FALSE");
                                DRow.CreateCell(21).SetCellValue("FALSE");
                                DRow.CreateCell(22).SetCellValue("FALSE");
                                DRow.CreateCell(23).SetCellValue("FALSE");
                            }
                            else if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeBlanketOrderNumber)
                            {
                                DRow.CreateCell(17).SetCellValue("FALSE");
                                DRow.CreateCell(18).SetCellValue("FALSE");
                                DRow.CreateCell(19).SetCellValue("TRUE");
                                DRow.CreateCell(20).SetCellValue("FALSE");
                                DRow.CreateCell(21).SetCellValue("FALSE");
                                DRow.CreateCell(22).SetCellValue("FALSE");
                                DRow.CreateCell(23).SetCellValue("FALSE");
                            }
                            else if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeIncrementingOrderNumber)
                            {
                                DRow.CreateCell(17).SetCellValue("FALSE");
                                DRow.CreateCell(18).SetCellValue("FALSE");
                                DRow.CreateCell(19).SetCellValue("FALSE");
                                DRow.CreateCell(20).SetCellValue("TRUE");
                                DRow.CreateCell(21).SetCellValue("FALSE");
                                DRow.CreateCell(22).SetCellValue("FALSE");
                                DRow.CreateCell(23).SetCellValue("FALSE");
                            }
                            else if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeIncrementingbyDay)
                            {
                                DRow.CreateCell(17).SetCellValue("FALSE");
                                DRow.CreateCell(18).SetCellValue("FALSE");
                                DRow.CreateCell(19).SetCellValue("FALSE");
                                DRow.CreateCell(20).SetCellValue("FALSE");
                                DRow.CreateCell(21).SetCellValue("TRUE");
                                DRow.CreateCell(22).SetCellValue("FALSE");
                                DRow.CreateCell(23).SetCellValue("FALSE");
                            }
                            else if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeDateIncrementing)
                            {
                                DRow.CreateCell(17).SetCellValue("FALSE");
                                DRow.CreateCell(18).SetCellValue("FALSE");
                                DRow.CreateCell(19).SetCellValue("FALSE");
                                DRow.CreateCell(20).SetCellValue("FALSE");
                                DRow.CreateCell(21).SetCellValue("FALSE");
                                DRow.CreateCell(22).SetCellValue("TRUE");
                                DRow.CreateCell(23).SetCellValue("FALSE");
                            }
                            else if (item.POAutoSequence.Value == (int)eTurnsWeb.Helper.CommonUtility.POAutoSequence.OrderNumberTypeDate)
                            {
                                DRow.CreateCell(17).SetCellValue("FALSE");
                                DRow.CreateCell(18).SetCellValue("FALSE");
                                DRow.CreateCell(19).SetCellValue("FALSE");
                                DRow.CreateCell(20).SetCellValue("FALSE");
                                DRow.CreateCell(21).SetCellValue("FALSE");
                                DRow.CreateCell(22).SetCellValue("FALSE");
                                DRow.CreateCell(23).SetCellValue("TRUE");
                            }
                        }
                        else
                        {
                            DRow.CreateCell(17).SetCellValue("FALSE");
                            DRow.CreateCell(18).SetCellValue("FALSE");
                            DRow.CreateCell(19).SetCellValue("FALSE");
                            DRow.CreateCell(20).SetCellValue("FALSE");
                            DRow.CreateCell(21).SetCellValue("FALSE");
                            DRow.CreateCell(22).SetCellValue("FALSE");
                            DRow.CreateCell(23).SetCellValue("FALSE");
                        }

                        DRow.CreateCell(24).SetCellValue(item.UDF1);
                        DRow.CreateCell(25).SetCellValue(item.UDF2);
                        DRow.CreateCell(26).SetCellValue(item.UDF3);
                        DRow.CreateCell(27).SetCellValue(item.UDF4);
                        DRow.CreateCell(28).SetCellValue(item.UDF5);

                        if (item.SupplierAccountDetails.Count > i)
                        {
                            DRow.CreateCell(29).SetCellValue(item.SupplierAccountDetails[i].AccountNo);
                            DRow.CreateCell(30).SetCellValue(item.SupplierAccountDetails[i].AccountName);

                            DRow.CreateCell(31).SetCellValue(item.SupplierAccountDetails[i].Address);
                            DRow.CreateCell(32).SetCellValue(item.SupplierAccountDetails[i].City);
                            DRow.CreateCell(33).SetCellValue(item.SupplierAccountDetails[i].State);
                            DRow.CreateCell(34).SetCellValue(item.SupplierAccountDetails[i].ZipCode);
                            DRow.CreateCell(35).SetCellValue(item.SupplierAccountDetails[i].Country);
                            DRow.CreateCell(36).SetCellValue(item.SupplierAccountDetails[i].ShipToID);
                            DRow.CreateCell(37).SetCellValue((item.SupplierAccountDetails[i].IsDefault == true ? "TRUE" : "FALSE"));

                        }
                        else
                        {
                            DRow.CreateCell(29).SetCellValue(string.Empty);
                            DRow.CreateCell(30).SetCellValue(string.Empty);

                            DRow.CreateCell(31).SetCellValue(string.Empty);
                            DRow.CreateCell(32).SetCellValue(string.Empty);
                            DRow.CreateCell(33).SetCellValue(string.Empty);
                            DRow.CreateCell(34).SetCellValue(string.Empty);
                            DRow.CreateCell(35).SetCellValue(string.Empty);

                            DRow.CreateCell(36).SetCellValue(string.Empty);
                            DRow.CreateCell(37).SetCellValue(string.Empty);

                        }

                        if (item.SupplierBlanketPODetails.Count > i)
                        {
                            DRow.CreateCell(38).SetCellValue(item.SupplierBlanketPODetails[i].BlanketPO);
                            if (item.SupplierBlanketPODetails[i].StartDate.HasValue)
                                DRow.CreateCell(39).SetCellValue(item.SupplierBlanketPODetails[i].StartDate.Value);
                            if (item.SupplierBlanketPODetails[i].Enddate.HasValue)
                                DRow.CreateCell(40).SetCellValue(item.SupplierBlanketPODetails[i].Enddate.Value);

                            DRow.CreateCell(41).SetCellValue(item.SupplierBlanketPODetails[i].MaxLimit ?? 0);
                            DRow.CreateCell(42).SetCellValue(item.SupplierBlanketPODetails[i].IsNotExceed);

                            DRow.CreateCell(43).SetCellValue(item.SupplierBlanketPODetails[i].MaxLimitQty ?? 0);
                            DRow.CreateCell(44).SetCellValue(item.SupplierBlanketPODetails[i].IsNotExceedQty);

                            DRow.CreateCell(45).SetCellValue("FALSE");
                        }
                        else
                        {
                            DRow.CreateCell(38).SetCellValue(string.Empty);
                            DRow.CreateCell(39).SetCellValue(string.Empty);

                            DRow.CreateCell(40).SetCellValue(string.Empty);
                            DRow.CreateCell(41).SetCellValue(string.Empty);
                            DRow.CreateCell(42).SetCellValue(string.Empty);
                            DRow.CreateCell(43).SetCellValue(string.Empty);
                            DRow.CreateCell(44).SetCellValue(string.Empty);

                            DRow.CreateCell(45).SetCellValue("FALSE");
                        }
                        DRow.CreateCell(46).SetCellValue(item.SupplierImage ?? string.Empty);
                        DRow.CreateCell(47).SetCellValue(item.ImageExternalURL ?? string.Empty);
                        RowId += 1;
                    }

                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string ShipViaMasterListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            ShipViaDAL obj = new ShipViaDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<ShipViaDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            List<ShipViaDTO> lstShipViaDTO = new List<ShipViaDTO>();
            lstShipViaDTO = obj.GetShipViaByIDsPlain(ids, SessionHelper.RoomID, SessionHelper.CompanyID);
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstShipViaDTO = (from c in DataFromDB
            //                     where arrid.Contains(c.ID.ToString())
            //                     select new ShipViaDTO
            //                     {
            //                         ID = c.ID,
            //                         ShipVia = c.ShipVia,
            //                         RoomName = c.RoomName,
            //                         Created = c.Created,
            //                         CreatedBy = c.CreatedBy,
            //                         Updated = c.Updated,
            //                         LastUpdatedBy = c.LastUpdatedBy,
            //                         UpdatedByName = c.UpdatedByName,
            //                         Room = c.Room,
            //                         CreatedByName = c.CreatedByName,
            //                         IsDeleted = c.IsDeleted,
            //                         IsArchived = c.IsArchived,
            //                         UDF1 = c.UDF1,
            //                         UDF2 = c.UDF2,
            //                         UDF3 = c.UDF3,
            //                         UDF4 = c.UDF4,
            //                         UDF5 = c.UDF5
            //                     }).ToList();
            //}

            if (lstShipViaDTO != null && lstShipViaDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* SHIPVIA");
                row.CreateCell(2).SetCellValue("UDF1");
                row.CreateCell(3).SetCellValue("UDF2");
                row.CreateCell(4).SetCellValue("UDF3");
                row.CreateCell(5).SetCellValue("UDF4");
                row.CreateCell(6).SetCellValue("UDF5");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ShipViaDTO item in lstShipViaDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID.ToString());
                    DRow.CreateCell(1).SetCellValue(item.ShipVia);
                    DRow.CreateCell(2).SetCellValue(item.UDF1);
                    DRow.CreateCell(3).SetCellValue(item.UDF2);
                    DRow.CreateCell(4).SetCellValue(item.UDF3);
                    DRow.CreateCell(5).SetCellValue(item.UDF4);
                    DRow.CreateCell(6).SetCellValue(item.UDF5);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string ShipViaHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            ShipViaDAL obj = new ShipViaDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<ShipViaDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            List<ShipViaDTO> lstShipViaDTO = new List<ShipViaDTO>();
            lstShipViaDTO = obj.GetShipViaHistoryByIDsNormal(ids, SessionHelper.RoomID, SessionHelper.CompanyID);


            if (lstShipViaDTO != null && lstShipViaDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("Action");
                row.CreateCell(1).SetCellValue("* ShipVia");
                row.CreateCell(2).SetCellValue("RoomName");
                row.CreateCell(3).SetCellValue("AddedFrom");
                row.CreateCell(3).SetCellValue("EditedFrom");
                row.CreateCell(4).SetCellValue("ReceivedOnDate");
                row.CreateCell(5).SetCellValue("ReceivedOnWebDate");
                row.CreateCell(6).SetCellValue("CreatedOn");
                row.CreateCell(7).SetCellValue("UpdatedOn");
                row.CreateCell(8).SetCellValue("UpdatedBy");
                row.CreateCell(9).SetCellValue("CreatedBy");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ShipViaDTO item in lstShipViaDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.ShipVia);
                    DRow.CreateCell(2).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(3).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(4).SetCellValue(item.ReceivedOnWeb.ToString());
                    DRow.CreateCell(5).SetCellValue(item.ReceivedOn.ToString());
                    DRow.CreateCell(6).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(7).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(8).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(9).SetCellValue(item.UpdatedByName);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string EnterpriseHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            eTurnsMaster.DAL.CommonMasterDAL obj = new eTurnsMaster.DAL.CommonMasterDAL();
            //IEnumerable<ShipViaDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            List<EnterpriseDTO> lstEnterpDTO = new List<EnterpriseDTO>();
            lstEnterpDTO = obj.GetEnterpriseHistoryByIDsNormal(ids);


            if (lstEnterpDTO != null && lstEnterpDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("EnterpriseName");
                row.CreateCell(2).SetCellValue("Address");
                row.CreateCell(3).SetCellValue("City");
                row.CreateCell(4).SetCellValue("State");
                row.CreateCell(5).SetCellValue("PostalCode");
                row.CreateCell(6).SetCellValue("Country");
                row.CreateCell(7).SetCellValue("ContactPhone");
                row.CreateCell(8).SetCellValue("ContactEmail");
                row.CreateCell(9).SetCellValue("CreatedBy");
                row.CreateCell(10).SetCellValue("UpdatedBy");
                row.CreateCell(11).SetCellValue("CreatedOn");
                row.CreateCell(12).SetCellValue("UpdatedOn");
                
                
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (EnterpriseDTO item in lstEnterpDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.Name);
                    DRow.CreateCell(2).SetCellValue(item.Address);
                    DRow.CreateCell(3).SetCellValue(item.City);
                    DRow.CreateCell(4).SetCellValue(item.State);
                    DRow.CreateCell(5).SetCellValue(item.PostalCode);
                    DRow.CreateCell(6).SetCellValue(item.Country);
                    DRow.CreateCell(7).SetCellValue(item.ContactPhone);
                    DRow.CreateCell(8).SetCellValue(item.ContactEmail);
                    DRow.CreateCell(9).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(10).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(11).SetCellValue(item.CreatedDate);
                    DRow.CreateCell(12).SetCellValue(item.UpdatedDate);


                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string RoomHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            RoomDAL obj = new RoomDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<ShipViaDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            List<RoomHistoryDTO> lstRoomDTO = new List<RoomHistoryDTO>();
            SortNameString = SortNameString.Replace("MethodOfValuingInventoryName", "MethodOfValuingInventory").Replace("AttachingWOWithRequisitionName", "AttachingWOWithRequisition")
           .Replace("PreventMaxOrderQtyName", "PreventMaxOrderQty").Replace("PreventMaxOrderQtyName", "PreventMaxOrderQty").Replace("DefaultCountTypeName", "DefaultCountType")
           .Replace("CountAutoSequenceName", "CountAutoSequence").Replace("POAutoSequenceName", "POAutoSequence").Replace("PullPurchaseNumberTypeName", "PullPurchaseNumberType")
           .Replace("ReqAutoSequenceName", "ReqAutoSequence").Replace("StagingAutoSequenceName", "StagingAutoSequence").Replace("TransferAutoSequenceName", "TransferAutoSequence")
           .Replace("WorkOrderAutoSequenceName", "WorkOrderAutoSequence").Replace("TAOAutoSequenceName", "TAOAutoSequence").Replace("ToolCountAutoSequenceName", "ToolCountAutoSequence")
           .Replace("PullRejectionTypeName", "PullRejectionType").Replace("ReplenishmentTypeName", "ReplenishmentType").Replace("BaseOfInventoryName", "BaseOfInventory");
            
            lstRoomDTO = obj.GetRoomHistoryByIDsNormal(ids, SessionHelper.CompanyID, SessionHelper.EnterPriceID).OrderBy(SortNameString).ToList();


            if (lstRoomDTO != null && lstRoomDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("RoomName");
                row.CreateCell(2).SetCellValue("CompanyName");
                row.CreateCell(3).SetCellValue("ContactName");
                row.CreateCell(4).SetCellValue("Address");
                row.CreateCell(5).SetCellValue("City");
                row.CreateCell(6).SetCellValue("State");
                row.CreateCell(7).SetCellValue("PostalCode");
                row.CreateCell(8).SetCellValue("Country");
                row.CreateCell(9).SetCellValue("Email");
                row.CreateCell(10).SetCellValue("Phone");
                row.CreateCell(11).SetCellValue("IsRoomActive");
                row.CreateCell(12).SetCellValue("InvoiceBranch");
                row.CreateCell(13).SetCellValue("CustomerNumber");
                row.CreateCell(14).SetCellValue("BlanketPO");
                row.CreateCell(15).SetCellValue("LastOrderDate");
                row.CreateCell(16).SetCellValue("LastPullDate");
                row.CreateCell(17).SetCellValue("IsConsignment");
                row.CreateCell(18).SetCellValue("Trending");
                row.CreateCell(29).SetCellValue("Value1");
                row.CreateCell(20).SetCellValue("Value2");
                row.CreateCell(21).SetCellValue("Value3");
                row.CreateCell(22).SetCellValue("SuggestedOrder");
                row.CreateCell(23).SetCellValue("SuggestedTransfer");
                row.CreateCell(24).SetCellValue("ReplineshmentRoom");
                row.CreateCell(25).SetCellValue("IseVMI");
                row.CreateCell(26).SetCellValue("MaxOrderSize");
                row.CreateCell(27).SetCellValue("HighPO");
                row.CreateCell(28).SetCellValue("HighJob");
                row.CreateCell(29).SetCellValue("HighTransfer");
                row.CreateCell(30).SetCellValue("HighCount");
                row.CreateCell(31).SetCellValue("GlobalMarkupParts");
                row.CreateCell(32).SetCellValue("GlobalMarkupLabor");
                row.CreateCell(33).SetCellValue("Tax1Parts");
                row.CreateCell(34).SetCellValue("Tax1Labor");
                row.CreateCell(35).SetCellValue("Tax1name");
                row.CreateCell(36).SetCellValue("Tax1percent");
                row.CreateCell(37).SetCellValue("Tax2Parts");
                row.CreateCell(38).SetCellValue("Tax2Labor");
                row.CreateCell(39).SetCellValue("Tax2name");
                row.CreateCell(40).SetCellValue("Tax2percent");
                row.CreateCell(41).SetCellValue("Tax2onTax1");
                row.CreateCell(42).SetCellValue("GXPRConsJob");
                row.CreateCell(43).SetCellValue("CostCenter");
                row.CreateCell(44).SetCellValue("UniqueID");
                row.CreateCell(45).SetCellValue("eVMIWaitCommand");
                row.CreateCell(46).SetCellValue("eVMIWaitPort");
                row.CreateCell(47).SetCellValue("CreatedOn");
                row.CreateCell(48).SetCellValue("UpdatedOn");
                row.CreateCell(49).SetCellValue("UpdatedBy");
                row.CreateCell(50).SetCellValue("CreatedBy");
                row.CreateCell(51).SetCellValue("BillingRoomType");
                row.CreateCell(52).SetCellValue("CountAutoSequence");
                row.CreateCell(53).SetCellValue("NextCountNo");
                row.CreateCell(54).SetCellValue("POAutoSequence");
                row.CreateCell(55).SetCellValue("POAutoNrFixedValue");
                row.CreateCell(56).SetCellValue("NextOrderNo");
                row.CreateCell(57).SetCellValue("PullPurchaseNumberType");
                row.CreateCell(58).SetCellValue("PullPurchaseNrFixedValue");
                row.CreateCell(59).SetCellValue("LastPullPurchaseNumberUsed");
                row.CreateCell(60).SetCellValue("RequisitionAutoSequence");
                row.CreateCell(61).SetCellValue("ReqAutoNrFixedValue");
                row.CreateCell(62).SetCellValue("NextRequisitionNo");
                row.CreateCell(63).SetCellValue("IsAllowRequisitionDuplicate");
                row.CreateCell(64).SetCellValue("IsAllowOrderDuplicate");
                row.CreateCell(65).SetCellValue("IsAllowWorkOrdersDuplicate");
                row.CreateCell(66).SetCellValue("StagingAutoSequence");
                row.CreateCell(67).SetCellValue("StagingAutoNrFixedValue");
                row.CreateCell(68).SetCellValue("NextStagingNo");
                row.CreateCell(69).SetCellValue("TransferAutoSequence");
                row.CreateCell(70).SetCellValue("TransferAutoNrFixedValue");
                row.CreateCell(71).SetCellValue("NextTransferNo");
                row.CreateCell(72).SetCellValue("WorkOrderAutoSequence");
                row.CreateCell(73).SetCellValue("WorkOrderAutoNrFixedValue");
                row.CreateCell(74).SetCellValue("NextWorkOrderNo");
                row.CreateCell(75).SetCellValue("TAOAutoSequence");
                row.CreateCell(76).SetCellValue("NextToolAssetOrderNo");
                row.CreateCell(77).SetCellValue("ToolCountAutoSequence");
                row.CreateCell(78).SetCellValue("NextToolCountNo");
                row.CreateCell(79).SetCellValue("AllowInsertingItemOnScan");
                row.CreateCell(80).SetCellValue("AllowPullBeyondAvailableQty");
                row.CreateCell(81).SetCellValue("PullRejectionType");
                row.CreateCell(82).SetCellValue("ReplenishmentType");
                row.CreateCell(83).SetCellValue("DefaultBinID");
                row.CreateCell(84).SetCellValue("DefaultSupplier");
                row.CreateCell(85).SetCellValue("SlowMovingValue");
                row.CreateCell(86).SetCellValue("FastMovingValue");
                row.CreateCell(87).SetCellValue("InventoryConsuptionMethod");
                row.CreateCell(88).SetCellValue("ValuingInventoryMethod");
                row.CreateCell(89).SetCellValue("BaseOfInventory");
                row.CreateCell(90).SetCellValue("LicenseBilled");
                row.CreateCell(91).SetCellValue("IsProjectSpendMandatory");
                row.CreateCell(92).SetCellValue("WarnUserOnAssigningNonDefaultBin");
                row.CreateCell(93).SetCellValue("IsWOSignatureRequired");
                row.CreateCell(94).SetCellValue("IsIgnoreCreditRule");
                row.CreateCell(95).SetCellValue("ForceSupplierFilter");
                row.CreateCell(96).SetCellValue("IsAllowOrderCostuom");
                row.CreateCell(97).SetCellValue("RequestedXDays");
                row.CreateCell(98).SetCellValue("ShelfLifeleadtimeOrdRpt");
                row.CreateCell(99).SetCellValue("LeadTimeOrdRpt");
                row.CreateCell(100).SetCellValue("MaintenanceDueNoticeDays");
                row.CreateCell(101).SetCellValue("DefaultRequisitionRequiredDays");
                row.CreateCell(102).SetCellValue("AttachingWOWithRequisition");
                row.CreateCell(103).SetCellValue("PreventMaxOrderQty");
                row.CreateCell(104).SetCellValue("DefaultCountType");
                row.CreateCell(105).SetCellValue("SuggestedReturn"); ;
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (RoomHistoryDTO item in lstRoomDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.RoomName);
                    DRow.CreateCell(2).SetCellValue(item.CompanyName);
                    DRow.CreateCell(3).SetCellValue(item.ContactName);
                    DRow.CreateCell(4).SetCellValue(item.streetaddress);
                    DRow.CreateCell(5).SetCellValue(item.City);
                    DRow.CreateCell(6).SetCellValue(item.State);
                    DRow.CreateCell(7).SetCellValue(item.PostalCode);
                    DRow.CreateCell(8).SetCellValue(item.Country);
                    DRow.CreateCell(9).SetCellValue(item.Email);
                    DRow.CreateCell(10).SetCellValue(item.PhoneNo);
                    DRow.CreateCell(11).SetCellValue(item.IsRoomActive == true ? "Yes" : "No");
                    DRow.CreateCell(12).SetCellValue(item.InvoiceBranch);
                    DRow.CreateCell(13).SetCellValue(item.CustomerNumber);
                    DRow.CreateCell(14).SetCellValue(item.BlanketPO);
                    DRow.CreateCell(15).SetCellValue(item.LastOrderDate ?? DateTime.MinValue);
                    DRow.CreateCell(16).SetCellValue(item.LastPullDate ?? DateTime.MinValue);
                    DRow.CreateCell(17).SetCellValue(item.IsConsignment == true ? "Yes" : "No");
                    DRow.CreateCell(18).SetCellValue(item.IsTrending == true ? "Yes" : "No");
                    DRow.CreateCell(29).SetCellValue(item.TrendingFormulaDays ?? 0);
                    DRow.CreateCell(20).SetCellValue(item.TrendingFormulaOverDays ?? 0);
                    DRow.CreateCell(21).SetCellValue(item.TrendingFormulaAvgDays ?? 0);
                    DRow.CreateCell(22).SetCellValue(item.SuggestedOrder == true ? "Yes" : "No");
                    DRow.CreateCell(23).SetCellValue(item.SuggestedTransfer == true ? "Yes" : "No");
                    DRow.CreateCell(24).SetCellValue(item.ReplineshmentRoom ?? 0);
                    DRow.CreateCell(25).SetCellValue(item.IseVMI == true ? "Yes" : "No");
                    DRow.CreateCell(26).SetCellValue(item.MaxOrderSize ?? 0);
                    DRow.CreateCell(27).SetCellValue(item.HighPO);
                    DRow.CreateCell(28).SetCellValue(item.HighJob);
                    DRow.CreateCell(29).SetCellValue(item.HighTransfer);
                    DRow.CreateCell(30).SetCellValue(item.HighCount);
                    DRow.CreateCell(31).SetCellValue(item.GlobMarkupParts ?? 0);
                    DRow.CreateCell(32).SetCellValue(item.GlobMarkupLabor ?? 0);
                    DRow.CreateCell(33).SetCellValue(item.IsTax1Parts == true ? "Yes" : "No");
                    DRow.CreateCell(34).SetCellValue(item.IsTax1Labor == true ? "Yes" : "No");
                    DRow.CreateCell(35).SetCellValue(item.Tax1name);
                    DRow.CreateCell(36).SetCellValue(item.Tax1Rate ?? 0);
                    DRow.CreateCell(37).SetCellValue(item.IsTax2Parts == true ? "Yes" : "No");
                    DRow.CreateCell(38).SetCellValue(item.IsTax2Labor == true ? "Yes" : "No");
                    DRow.CreateCell(39).SetCellValue(item.tax2name);
                    DRow.CreateCell(40).SetCellValue(item.Tax2Rate ?? 0);
                    DRow.CreateCell(41).SetCellValue(item.IsTax2onTax1 == true ? "Yes" : "No");
                    DRow.CreateCell(42).SetCellValue(item.GXPRConsJob);
                    DRow.CreateCell(43).SetCellValue(item.CostCenter);
                    DRow.CreateCell(44).SetCellValue(item.UniqueID);
                    DRow.CreateCell(45).SetCellValue(item.eVMIWaitCommand);
                    DRow.CreateCell(46).SetCellValue(item.eVMIWaitPort);
                    DRow.CreateCell(47).SetCellValue(item.Created);
                    DRow.CreateCell(48).SetCellValue(item.Updated ?? DateTime.MinValue);
                    DRow.CreateCell(49).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(50).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(51).SetCellValue(item.BillingRoomTypeStr);
                    DRow.CreateCell(52).SetCellValue(item.CountAutoSequence ?? 0);
                    DRow.CreateCell(53).SetCellValue(item.NextCountNo);
                    DRow.CreateCell(54).SetCellValue(item.POAutoSequenceStr);
                    DRow.CreateCell(55).SetCellValue(item.POAutoNrFixedValue);
                    DRow.CreateCell(56).SetCellValue(item.NextOrderNo);
                    DRow.CreateCell(57).SetCellValue(item.PullPurchaseNumberTypeStr);
                    DRow.CreateCell(58).SetCellValue(item.PullPurchaseNrFixedValue);
                    DRow.CreateCell(59).SetCellValue(item.LastPullPurchaseNumberUsed);
                    DRow.CreateCell(60).SetCellValue(item.ReqAutoSequenceStr);
                    DRow.CreateCell(61).SetCellValue(item.ReqAutoNrFixedValue);
                    DRow.CreateCell(62).SetCellValue(item.NextRequisitionNo ?? 0);
                    DRow.CreateCell(63).SetCellValue(item.IsAllowRequisitionDuplicate == true ? "Yes" : "No");
                    DRow.CreateCell(64).SetCellValue(item.IsAllowOrderDuplicate == true ? "Yes" : "No");
                    DRow.CreateCell(65).SetCellValue(item.IsAllowWorkOrdersDuplicate == true ? "Yes" : "No");
                    DRow.CreateCell(66).SetCellValue(item.StagingAutoSequenceStr);
                    DRow.CreateCell(67).SetCellValue(item.StagingAutoNrFixedValue);
                    DRow.CreateCell(68).SetCellValue(item.NextStagingNo ?? 0);
                    DRow.CreateCell(69).SetCellValue(item.TransferAutoSequenceStr);
                    DRow.CreateCell(70).SetCellValue(item.TransferAutoNrFixedValue);
                    DRow.CreateCell(71).SetCellValue(item.NextTransferNo ?? 0);
                    DRow.CreateCell(72).SetCellValue(item.WorkOrderAutoSequenceStr);
                    DRow.CreateCell(73).SetCellValue(item.WorkOrderAutoNrFixedValue);
                    DRow.CreateCell(74).SetCellValue(item.NextWorkOrderNo ?? 0);
                    DRow.CreateCell(75).SetCellValue(item.TAOAutoSequenceStr);
                    DRow.CreateCell(76).SetCellValue(item.NextToolAssetOrderNo);
                    DRow.CreateCell(77).SetCellValue(item.ToolCountAutoSequenceStr);
                    DRow.CreateCell(78).SetCellValue(item.NextToolCountNo);
                    DRow.CreateCell(79).SetCellValue(item.AllowInsertingItemOnScan == true ? "Yes" : "No");
                    DRow.CreateCell(80).SetCellValue(item.AllowPullBeyondAvailableQty == true ? "Yes" : "No");
                    DRow.CreateCell(81).SetCellValue(item.PullRejectionTypeStr);
                    DRow.CreateCell(82).SetCellValue(item.ReplenishmentTypeStr);
                    DRow.CreateCell(83).SetCellValue(item.DefaultBinName);
                    DRow.CreateCell(84).SetCellValue(item.DefaultSupplierName);
                    DRow.CreateCell(85).SetCellValue(item.SlowMovingValue);
                    DRow.CreateCell(86).SetCellValue(item.FastMovingValue);
                    DRow.CreateCell(87).SetCellValue(item.InventoryConsuptionMethod);
                    DRow.CreateCell(88).SetCellValue(item.MethodOfValuingInventoryStr);
                    DRow.CreateCell(89).SetCellValue(item.BaseOfInventoryStr);
                    DRow.CreateCell(90).SetCellValue(item.LicenseBilled ?? DateTime.MinValue);
                    DRow.CreateCell(91).SetCellValue(item.IsProjectSpendMandatory == true ? "Yes" : "No");
                    DRow.CreateCell(92).SetCellValue(item.WarnUserOnAssigningNonDefaultBin == true ? "Yes" : "No");
                    DRow.CreateCell(93).SetCellValue(item.IsWOSignatureRequired == true ? "Yes" : "No");
                    DRow.CreateCell(94).SetCellValue(item.IsIgnoreCreditRule == true ? "Yes" : "No");
                    DRow.CreateCell(95).SetCellValue(item.ForceSupplierFilter == true ? "Yes" : "No");
                    DRow.CreateCell(96).SetCellValue(item.IsAllowOrderCostuom == true ? "Yes" : "No");
                    DRow.CreateCell(97).SetCellValue(item.RequestedXDays ?? 0);
                    DRow.CreateCell(98).SetCellValue(item.ShelfLifeleadtimeOrdRpt ?? 0);
                    DRow.CreateCell(99).SetCellValue(item.LeadTimeOrdRpt ?? 0);
                    DRow.CreateCell(100).SetCellValue(item.MaintenanceDueNoticeDays ?? 0);
                    DRow.CreateCell(101).SetCellValue(item.DefaultRequisitionRequiredDays ?? 0);
                    DRow.CreateCell(102).SetCellValue(item.AttachingWOWithRequisitionStr);
                    DRow.CreateCell(103).SetCellValue(item.PreventMaxOrderQtyStr);
                    DRow.CreateCell(104).SetCellValue(item.DefaultCountTypeStr);
                    DRow.CreateCell(105).SetCellValue(item.SuggestedReturn == true ? "Yes" : "No");


                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }

        public string DeletedRoomListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            RoomDAL obj = new RoomDAL(SessionHelper.EnterPriseDBName);
            List<RoomDTO> lstRoomDTO = new List<RoomDTO>();
            lstRoomDTO = obj.GetDeletedRoomByIDs(ids);


            if (lstRoomDTO != null && lstRoomDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("ID");
                row.CreateCell(1).SetCellValue("RoomName");
                row.CreateCell(2).SetCellValue("EnterpriseName");
                row.CreateCell(3).SetCellValue("CompanyName");
                row.CreateCell(4).SetCellValue("ContactName");
                row.CreateCell(5).SetCellValue("streetaddress");
                row.CreateCell(6).SetCellValue("City");
                row.CreateCell(7).SetCellValue("State");
                row.CreateCell(8).SetCellValue("PostalCode");
                row.CreateCell(9).SetCellValue("Country");
                row.CreateCell(10).SetCellValue("Email");
                row.CreateCell(11).SetCellValue("PhoneNo");
                row.CreateCell(12).SetCellValue("IsRoomActive");
                row.CreateCell(13).SetCellValue("CustomerNumber");
                row.CreateCell(14).SetCellValue("BlanketPO");
                row.CreateCell(15).SetCellValue("LastOrderDate");
                row.CreateCell(16).SetCellValue("LastPullDate");
                row.CreateCell(17).SetCellValue("IsConsignment");
                row.CreateCell(18).SetCellValue("IsTrending");
                row.CreateCell(19).SetCellValue("TrendingFormulaDays");
                row.CreateCell(20).SetCellValue("TrendingFormulaOverDays");
                row.CreateCell(21).SetCellValue("TrendingFormulaAvgDays");
                row.CreateCell(22).SetCellValue("SuggestedOrder");
                row.CreateCell(23).SetCellValue("SuggestedTransfer");
                row.CreateCell(24).SetCellValue("ReplineshmentRoom");
                row.CreateCell(25).SetCellValue("IseVMI");
                row.CreateCell(26).SetCellValue("MaxOrderSize");
                row.CreateCell(27).SetCellValue("HighPO");
                row.CreateCell(28).SetCellValue("HighJob");
                row.CreateCell(29).SetCellValue("HighTransfer");
                row.CreateCell(30).SetCellValue("HighCount");
                row.CreateCell(31).SetCellValue("GlobMarkupParts");
                row.CreateCell(32).SetCellValue("GlobMarkupLabor");
                row.CreateCell(33).SetCellValue("IsTax1Parts");
                row.CreateCell(34).SetCellValue("IsTax1Labor");
                row.CreateCell(35).SetCellValue("tax1name");
                row.CreateCell(36).SetCellValue("Tax1Rate");
                row.CreateCell(37).SetCellValue("IsTax2Parts");
                row.CreateCell(38).SetCellValue("IsTax2Labor");
                row.CreateCell(39).SetCellValue("tax2name");
                row.CreateCell(40).SetCellValue("Tax2Rate");
                row.CreateCell(41).SetCellValue("IsTax2onTax1");
                row.CreateCell(42).SetCellValue("GXPRConsJob");
                row.CreateCell(43).SetCellValue("CostCenter");
                row.CreateCell(44).SetCellValue("UniqueID");
                row.CreateCell(45).SetCellValue("eVMIWaitCommand");
                row.CreateCell(46).SetCellValue("eVMIWaitPort");
                row.CreateCell(47).SetCellValue("Created");
                row.CreateCell(48).SetCellValue("Updated");
                row.CreateCell(49).SetCellValue("UpdatedByName");
                row.CreateCell(50).SetCellValue("CreatedByName");
                row.CreateCell(51).SetCellValue("ActiveOn");
                row.CreateCell(52).SetCellValue("LastSyncDateTime");
                row.CreateCell(53).SetCellValue("PDABuildVersion");
                row.CreateCell(54).SetCellValue("LastSyncUserName");
                row.CreateCell(55).SetCellValue("BillingRoomType");
                row.CreateCell(56).SetCellValue("AttachingWOWithRequisition");
                row.CreateCell(57).SetCellValue("PreventMaxOrderQty");
                row.CreateCell(58).SetCellValue("DefaultCountType");
                row.CreateCell(59).SetCellValue("IsWOSignatureRequired");
                row.CreateCell(60).SetCellValue("IsIgnoreCreditRule");
                row.CreateCell(61).SetCellValue("ForceSupplierFilter");
                row.CreateCell(62).SetCellValue("SuggestedReturn");
                row.CreateCell(63).SetCellValue("NoOfItems");
                if (SessionHelper.RoleID == -1)
                {
                    row.CreateCell(64).SetCellValue("ReportAppIntent");
                }
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (RoomDTO item in lstRoomDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.RoomName);
                    DRow.CreateCell(2).SetCellValue(item.EnterpriseName);
                    DRow.CreateCell(3).SetCellValue(item.CompanyName);
                    DRow.CreateCell(4).SetCellValue(item.ContactName);
                    DRow.CreateCell(5).SetCellValue(item.streetaddress);
                    DRow.CreateCell(6).SetCellValue(item.City);
                    DRow.CreateCell(7).SetCellValue(item.State);
                    DRow.CreateCell(8).SetCellValue(item.PostalCode);
                    DRow.CreateCell(9).SetCellValue(item.Country);
                    DRow.CreateCell(10).SetCellValue(item.Email);
                    DRow.CreateCell(11).SetCellValue(item.PhoneNo);
                    DRow.CreateCell(12).SetCellValue(item.IsRoomActive == true ? "Yes" : "No");
                    DRow.CreateCell(13).SetCellValue(item.CustomerNumber);
                    DRow.CreateCell(14).SetCellValue(item.BlanketPO);
                    string LastOrderDate = string.Empty;
                    if (item.LastOrderDate.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                    {
                        LastOrderDate = Convert.ToString(item.LastOrderDate.GetValueOrDefault(DateTime.MinValue));
                    }
                    DRow.CreateCell(15).SetCellValue(LastOrderDate);

                    string LastPullDate = string.Empty;
                    if (item.LastPullDate.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                    {
                        LastPullDate = Convert.ToString(item.LastPullDate.GetValueOrDefault(DateTime.MinValue));
                    }
                    DRow.CreateCell(16).SetCellValue(LastPullDate);
                    DRow.CreateCell(17).SetCellValue(item.IsConsignment == true ? "Yes" : "No");
                    DRow.CreateCell(18).SetCellValue(item.IsTrending == true ? "Yes" : "No");
                    DRow.CreateCell(19).SetCellValue(item.TrendingFormulaDays ?? 0);
                    DRow.CreateCell(20).SetCellValue(item.TrendingFormulaOverDays ?? 0);
                    DRow.CreateCell(21).SetCellValue(item.TrendingFormulaAvgDays ?? 0);
                    DRow.CreateCell(22).SetCellValue(item.SuggestedOrder == true ? "Yes" : "No");
                    DRow.CreateCell(23).SetCellValue(item.SuggestedTransfer == true ? "Yes" : "No");
                    DRow.CreateCell(24).SetCellValue(item.ReplineshmentRoom ?? 0);
                    DRow.CreateCell(25).SetCellValue(item.IseVMI == true ? "Yes" : "No");
                    DRow.CreateCell(26).SetCellValue(item.MaxOrderSize ?? 0);
                    DRow.CreateCell(27).SetCellValue(item.HighPO);
                    DRow.CreateCell(28).SetCellValue(item.HighJob);
                    DRow.CreateCell(29).SetCellValue(item.HighTransfer);
                    DRow.CreateCell(30).SetCellValue(item.HighCount);
                    DRow.CreateCell(31).SetCellValue(item.GlobMarkupParts ?? 0);
                    DRow.CreateCell(32).SetCellValue(item.GlobMarkupLabor ?? 0);
                    DRow.CreateCell(33).SetCellValue(item.IsTax1Parts == true ? "Yes" : "No");
                    DRow.CreateCell(34).SetCellValue(item.IsTax1Labor == true ? "Yes" : "No");
                    DRow.CreateCell(35).SetCellValue(item.Tax1name);
                    DRow.CreateCell(36).SetCellValue(item.Tax1Rate ?? 0);
                    DRow.CreateCell(37).SetCellValue(item.IsTax2Parts == true ? "Yes" : "No");
                    DRow.CreateCell(38).SetCellValue(item.IsTax2Labor == true ? "Yes" : "No");
                    DRow.CreateCell(39).SetCellValue(item.tax2name);
                    DRow.CreateCell(40).SetCellValue(item.Tax2Rate ?? 0);
                    DRow.CreateCell(41).SetCellValue(item.IsTax2onTax1 == true ? "Yes" : "No");
                    DRow.CreateCell(42).SetCellValue(item.GXPRConsJob);
                    DRow.CreateCell(43).SetCellValue(item.CostCenter);
                    DRow.CreateCell(44).SetCellValue(item.UniqueID);
                    DRow.CreateCell(45).SetCellValue(item.eVMIWaitCommand);
                    DRow.CreateCell(46).SetCellValue(item.eVMIWaitPort);

                    string Created = string.Empty;
                    if (item.Created.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                    {
                        Created = Convert.ToString(item.Created.GetValueOrDefault(DateTime.MinValue));
                    }
                    DRow.CreateCell(47).SetCellValue(Created);

                    string Updated = string.Empty;
                    if (item.Updated.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                    {
                        Updated = Convert.ToString(item.Updated.GetValueOrDefault(DateTime.MinValue));
                    }
                    DRow.CreateCell(48).SetCellValue(Updated);
                    DRow.CreateCell(49).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(50).SetCellValue(item.CreatedByName);
                    string ActiveOn = string.Empty;
                    if (item.ActiveOn.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                    {
                        ActiveOn = Convert.ToString(item.ActiveOn.GetValueOrDefault(DateTime.MinValue));
                    }
                    DRow.CreateCell(51).SetCellValue(ActiveOn);

                    string LastSyncDateTime = string.Empty;
                    if (item.LastSyncDateTime.GetValueOrDefault(DateTime.MinValue) != DateTime.MinValue)
                    {
                        LastSyncDateTime = Convert.ToString(item.LastSyncDateTime.GetValueOrDefault(DateTime.MinValue));
                    }
                    DRow.CreateCell(52).SetCellValue(LastSyncDateTime);
                    DRow.CreateCell(53).SetCellValue(item.PDABuildVersion);
                    DRow.CreateCell(54).SetCellValue(item.LastSyncUserName);
                    DRow.CreateCell(55).SetCellValue(item.BillingRoomType ?? 0);
                    DRow.CreateCell(56).SetCellValue(item.AttachingWOWithRequisition ?? (int)AttachingWOWithRequisition.New);
                    DRow.CreateCell(57).SetCellValue(item.PreventMaxOrderQty);
                    DRow.CreateCell(58).SetCellValue(item.DefaultCountType);
                    DRow.CreateCell(59).SetCellValue(item.IsWOSignatureRequired);
                    DRow.CreateCell(60).SetCellValue(item.IsIgnoreCreditRule);
                    DRow.CreateCell(61).SetCellValue(item.ForceSupplierFilter);
                    DRow.CreateCell(62).SetCellValue(item.SuggestedReturn == true ? "Yes" : "No");
                    DRow.CreateCell(63).SetCellValue(item.NoOfItems ?? 0);
                    if (SessionHelper.RoleID == -1)
                    {
                        DRow.CreateCell(64).SetCellValue(item.ReportAppIntent);
                    }
                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string UserHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            UserMasterDAL obj = new UserMasterDAL(SessionHelper.EnterPriseDBName);
            List<UserRoleModuleDetailsDTO> lstUserDTO = new List<UserRoleModuleDetailsDTO>();
            lstUserDTO = obj.GetUserHistoryByIdNormal(ids);


            if (lstUserDTO != null && lstUserDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryID");
                row.CreateCell(1).SetCellValue("CompanyName");
                row.CreateCell(2).SetCellValue("RoomName");
                row.CreateCell(3).SetCellValue("Role");
                row.CreateCell(4).SetCellValue("HistoryDate");
                row.CreateCell(5).SetCellValue("PermissionChanges");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (UserRoleModuleDetailsDTO item in lstUserDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.UserRoleChangeLogID);
                    DRow.CreateCell(1).SetCellValue(item.CompanyName);
                    DRow.CreateCell(2).SetCellValue(item.RoomName);
                    DRow.CreateCell(3).SetCellValue(item.RoleName);
                    DRow.CreateCell(4).SetCellValue(item.HistoryDate);
                    DRow.CreateCell(5).SetCellValue(item.PermissionChanges);


                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string CompanyHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            CompanyMasterDAL obj = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
            List<CompanyMasterDTO> lstCompanyDTO = new List<CompanyMasterDTO>();
            lstCompanyDTO = obj.GetCompanyMasterChangeLog(ids);


            if (lstCompanyDTO != null && lstCompanyDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue(ResCommon.HistoryID);
                row.CreateCell(1).SetCellValue(ResCommon.HistoryAction);
                row.CreateCell(2).SetCellValue(ResCommon.CompanyName);
                row.CreateCell(3).SetCellValue(ResCommon.Address);
                row.CreateCell(4).SetCellValue(ResCommon.City);
                row.CreateCell(5).SetCellValue(ResCommon.State);
                row.CreateCell(6).SetCellValue(ResCommon.PostalCode);
                row.CreateCell(7).SetCellValue(ResCommon.Country);
                row.CreateCell(8).SetCellValue(ResCommon.Phone);
                row.CreateCell(9).SetCellValue(ResCommon.Email);
                row.CreateCell(10).SetCellValue(ResCommon.CreatedOn);
                row.CreateCell(11).SetCellValue(ResCommon.UpdatedOn);
                row.CreateCell(12).SetCellValue(ResCommon.UpdatedBy);
                row.CreateCell(13).SetCellValue(ResCommon.CreatedBy);
                row.CreateCell(14).SetCellValue(ResCommon.IsActive);

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (CompanyMasterDTO item in lstCompanyDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.HistoryID);
                    DRow.CreateCell(1).SetCellValue(item.Action);
                    DRow.CreateCell(2).SetCellValue(item.Name);
                    DRow.CreateCell(3).SetCellValue(item.Address);
                    DRow.CreateCell(4).SetCellValue(item.City);
                    DRow.CreateCell(5).SetCellValue(item.State);
                    DRow.CreateCell(6).SetCellValue(item.PostalCode);
                    DRow.CreateCell(7).SetCellValue(item.Country);
                    DRow.CreateCell(8).SetCellValue(item.ContactPhone);
                    DRow.CreateCell(9).SetCellValue(item.ContactEmail);
                    DRow.CreateCell(10).SetCellValue(item.CreatedDate);
                    DRow.CreateCell(11).SetCellValue(item.UpdatedDate);
                    DRow.CreateCell(12).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(13).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(14).SetCellValue(item.IsActive);
                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string AssetCategoryHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            AssetCategoryMasterDAL obj = new AssetCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            List<AssetCategoryMasterDTO> lstAssetCatDTO = new List<AssetCategoryMasterDTO>();
            lstAssetCatDTO = obj.GetAssetCategoryMasterChangeLog(ids);

            if (lstAssetCatDTO != null && lstAssetCatDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("AssetCategory");
                row.CreateCell(2).SetCellValue("RoomName");
                row.CreateCell(3).SetCellValue("CreatedOn");
                row.CreateCell(4).SetCellValue("UpdatedOn");
                row.CreateCell(5).SetCellValue("UpdatedBy");
                row.CreateCell(6).SetCellValue("CreatedBy");
                row.CreateCell(7).SetCellValue("AddedFrom");
                row.CreateCell(8).SetCellValue("EditedFrom");
                row.CreateCell(9).SetCellValue("ReceivedOnDate");
                row.CreateCell(10).SetCellValue("ReceivedOnWebDate");


                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (AssetCategoryMasterDTO item in lstAssetCatDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.AssetCategory);
                    DRow.CreateCell(2).SetCellValue(item.RoomName);
                    DRow.CreateCell(3).SetCellValue(item.Created ?? DateTime.MinValue);
                    DRow.CreateCell(4).SetCellValue(item.Updated ?? DateTime.MinValue);
                    DRow.CreateCell(5).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(6).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(7).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(8).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(9).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(10).SetCellValue(item.ReceivedOnDateWeb);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string BinHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            BinMasterDAL obj = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<BinMasterDTO> lstBinDTO = new List<BinMasterDTO>();
            lstBinDTO = obj.GetBinMasterByIdNormal(ids);
            if (lstBinDTO != null && lstBinDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("BinNumber");
                row.CreateCell(2).SetCellValue("RoomName");
                row.CreateCell(3).SetCellValue("CreatedOn");
                row.CreateCell(4).SetCellValue("UpdatedOn");
                row.CreateCell(5).SetCellValue("UpdatedBy");
                row.CreateCell(6).SetCellValue("CreatedBy");
                row.CreateCell(7).SetCellValue("AddedFrom");
                row.CreateCell(8).SetCellValue("EditedFrom");
                row.CreateCell(9).SetCellValue("ReceivedOnDate");
                row.CreateCell(10).SetCellValue("ReceivedOnWebDate");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (BinMasterDTO item in lstBinDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.BinNumber);
                    DRow.CreateCell(2).SetCellValue(item.RoomName);
                    DRow.CreateCell(3).SetCellValue(item.Created ?? DateTime.MinValue);
                    DRow.CreateCell(4).SetCellValue(item.LastUpdated ?? DateTime.MinValue);
                    DRow.CreateCell(5).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(6).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(7).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(8).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(9).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(10).SetCellValue(item.ReceivedOnDateWeb);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string CategoryHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            CategoryMasterDAL obj = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
            List<CategoryMasterDTO> lstCategoryDTO = new List<CategoryMasterDTO>();
            lstCategoryDTO = obj.GetCategoryMasterChangeLog(ids);
            if (lstCategoryDTO != null && lstCategoryDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("Category");
                row.CreateCell(2).SetCellValue("RoomName");
                row.CreateCell(3).SetCellValue("CreatedOn");
                row.CreateCell(4).SetCellValue("UpdatedOn");
                row.CreateCell(5).SetCellValue("UpdatedBy");
                row.CreateCell(6).SetCellValue("CreatedBy");
                row.CreateCell(7).SetCellValue("AddedFrom");
                row.CreateCell(8).SetCellValue("EditedFrom");
                row.CreateCell(9).SetCellValue("ReceivedOnDate");
                row.CreateCell(10).SetCellValue("ReceivedOnWebDate");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (CategoryMasterDTO item in lstCategoryDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.Category);
                    DRow.CreateCell(2).SetCellValue(item.RoomName);
                    DRow.CreateCell(3).SetCellValue(item.Created ?? DateTime.MinValue);
                    DRow.CreateCell(4).SetCellValue(item.Updated ?? DateTime.MinValue);
                    DRow.CreateCell(5).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(6).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(7).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(8).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(9).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(10).SetCellValue(item.ReceivedOnWebDate);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string CostUOMHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            CostUOMMasterDAL obj = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
            List<CostUOMMasterDTO> lstCostUOMDTO = new List<CostUOMMasterDTO>();
            lstCostUOMDTO = obj.GetCostUOMMasterChangeLog(ids);
            if (lstCostUOMDTO != null && lstCostUOMDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("CostUOM");
                row.CreateCell(2).SetCellValue("CostUOMValue");
                row.CreateCell(3).SetCellValue("RoomName");
                row.CreateCell(4).SetCellValue("AddedFrom");
                row.CreateCell(5).SetCellValue("EditedFrom");
                row.CreateCell(6).SetCellValue("ReceivedOnDate");
                row.CreateCell(7).SetCellValue("ReceivedOnWebDate");
                row.CreateCell(8).SetCellValue("CreatedOn");
                row.CreateCell(9).SetCellValue("UpdatedOn");
                row.CreateCell(10).SetCellValue("UpdatedBy");
                row.CreateCell(11).SetCellValue("CreatedBy");
                //------------End-------------("CreatedBy");-------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (CostUOMMasterDTO item in lstCostUOMDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.CostUOM);
                    DRow.CreateCell(2).SetCellValue(item.CostUOMValue ?? 0);
                    DRow.CreateCell(3).SetCellValue(item.RoomName);
                    DRow.CreateCell(4).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(5).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(6).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(7).SetCellValue(item.ReceivedOnWebDate);
                    DRow.CreateCell(8).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(9).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(10).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(11).SetCellValue(item.CreatedByName);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string CustomerHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            CustomerMasterDAL obj = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            List<CustomerMasterDTO> lstCustomerDTO = new List<CustomerMasterDTO>();
            lstCustomerDTO = obj.GetCustomerMasterChangeLog(ids);
            if (lstCustomerDTO != null && lstCustomerDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("Customer");
                row.CreateCell(2).SetCellValue("Account");
                row.CreateCell(3).SetCellValue("Address");
                row.CreateCell(4).SetCellValue("City");
                row.CreateCell(5).SetCellValue("State");
                row.CreateCell(6).SetCellValue("Country");
                row.CreateCell(7).SetCellValue("ZipCode");
                row.CreateCell(8).SetCellValue("Contact");
                row.CreateCell(9).SetCellValue("Email");
                row.CreateCell(10).SetCellValue("Phone");
                row.CreateCell(11).SetCellValue("RoomName");
                row.CreateCell(12).SetCellValue("CreatedOn");
                row.CreateCell(13).SetCellValue("UpdatedOn");
                row.CreateCell(14).SetCellValue("UpdatedBy");
                row.CreateCell(15).SetCellValue("CreatedBy");
                row.CreateCell(16).SetCellValue("AddedFrom");
                row.CreateCell(17).SetCellValue("EditedFrom");
                row.CreateCell(18).SetCellValue("ReceivedOnDate");
                row.CreateCell(19).SetCellValue("ReceivedOnWebDate");
                //------------End-------------("CreatedBy");-------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (CustomerMasterDTO item in lstCustomerDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.Customer);
                    DRow.CreateCell(2).SetCellValue(item.Account);
                    DRow.CreateCell(3).SetCellValue(item.Address);
                    DRow.CreateCell(4).SetCellValue(item.City);
                    DRow.CreateCell(5).SetCellValue(item.State);
                    DRow.CreateCell(6).SetCellValue(item.Country);
                    DRow.CreateCell(7).SetCellValue(item.ZipCode);
                    DRow.CreateCell(8).SetCellValue(item.Contact);
                    DRow.CreateCell(9).SetCellValue(item.Email);
                    DRow.CreateCell(10).SetCellValue(item.Phone);
                    DRow.CreateCell(11).SetCellValue(item.RoomName);
                    DRow.CreateCell(12).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(13).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(14).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(15).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(16).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(17).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(18).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(19).SetCellValue(item.ReceivedOnDateWeb);
                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }

        public string FTPHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            SFTPDAL obj = new SFTPDAL(SessionHelper.EnterPriseDBName);
            List<FTPMasterDTO> lstFTPDTO = new List<FTPMasterDTO>();
            lstFTPDTO = obj.GetFTPMasterChangeLog(ids);
            if (lstFTPDTO != null && lstFTPDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("SFtpName");
                row.CreateCell(2).SetCellValue("ServerAddress");
                row.CreateCell(3).SetCellValue("UserName");
                row.CreateCell(4).SetCellValue("Password");
                row.CreateCell(5).SetCellValue("Port");
                row.CreateCell(6).SetCellValue("IsImportFTP");
                row.CreateCell(7).SetCellValue("RoomName");
                row.CreateCell(8).SetCellValue("CreatedOn");
                row.CreateCell(9).SetCellValue("UpdatedOn");
                row.CreateCell(10).SetCellValue("UpdatedBy");
                row.CreateCell(11).SetCellValue("CreatedBy");
                //------------End-------------("CreatedBy");-------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (FTPMasterDTO item in lstFTPDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.SFtpName);
                    DRow.CreateCell(2).SetCellValue(item.ServerAddress);
                    DRow.CreateCell(3).SetCellValue(item.UserName);
                    DRow.CreateCell(4).SetCellValue(item.Password);
                    DRow.CreateCell(5).SetCellValue(item.Port);
                    DRow.CreateCell(6).SetCellValue(item.IsImportFTP);
                    DRow.CreateCell(7).SetCellValue(item.RoomName);
                    string strCreated = CommonUtility.ConvertDateByTimeZone((DateTime)item.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    string strUpdated = CommonUtility.ConvertDateByTimeZone((DateTime)item.LastUpdated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    DRow.CreateCell(8).SetCellValue(strCreated.ToString());
                    DRow.CreateCell(9).SetCellValue(strUpdated.ToString());
                    DRow.CreateCell(10).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(11).SetCellValue(item.CreatedByName);
                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }

        public string GLAccountHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            GLAccountMasterDAL obj = new GLAccountMasterDAL(SessionHelper.EnterPriseDBName);
            List<GLAccountMasterDTO> lstGLAccountDTO = new List<GLAccountMasterDTO>();
            lstGLAccountDTO = obj.GetGLAccountMasterChangeLog(ids);
            if (lstGLAccountDTO != null && lstGLAccountDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("GLAccount");
                row.CreateCell(2).SetCellValue("Description");
                row.CreateCell(3).SetCellValue("RoomName");
                row.CreateCell(4).SetCellValue("CreatedOn");
                row.CreateCell(5).SetCellValue("UpdatedOn");
                row.CreateCell(6).SetCellValue("UpdatedBy");
                row.CreateCell(7).SetCellValue("CreatedBy");

                //------------End-------------("CreatedBy");-------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (GLAccountMasterDTO item in lstGLAccountDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.GLAccount);
                    DRow.CreateCell(2).SetCellValue(item.Description);
                    DRow.CreateCell(3).SetCellValue(item.RoomName);
                    DRow.CreateCell(4).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(5).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(6).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(7).SetCellValue(item.CreatedByName);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string ItemMasterChangeLogExcel(string Filepath, string ModuleName, string ids, long RoomID,long companyID)
        {
            InitializeWorkbook();
            string[] arrid = ids.Split(',');  
            Guid itemID = Guid.Parse(arrid[0]);
            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            ItemMasterDAL objdal = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            var lstoflogs = objdal.GetPagedRecordsNew_ChnageLog(itemID,0,0, out TotalRecordCount,null,null,RoomID,companyID,false,false,null);
            if (lstoflogs != null && lstoflogs.Count() > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue(ResCommon.HistoryID);
                row.CreateCell(1).SetCellValue(ResCommon.HistoryAction);
                row.CreateCell(2).SetCellValue(ResItemMaster.ItemType);
                row.CreateCell(3).SetCellValue(ResItemMaster.ItemNumber);
                row.CreateCell(4).SetCellValue(ResItemMaster.IsActive);
                row.CreateCell(5).SetCellValue(ResItemMaster.Description);
                row.CreateCell(6).SetCellValue(ResItemMaster.OnHandQuantity);
                row.CreateCell(7).SetCellValue(ResItemMaster.OnOrderQuantity);
                row.CreateCell(8).SetCellValue(ResItemMaster.OnOrderInTransitQuantity);
                row.CreateCell(9).SetCellValue(ResItemMaster.OrderedDate.ToString());
                row.CreateCell(10).SetCellValue(ResItemMaster.IsItemLevelMinMaxQtyRequired);
                row.CreateCell(11).SetCellValue(ResItemMaster.MinimumQuantity);
                row.CreateCell(12).SetCellValue(ResItemMaster.MaximumQuantity);
                row.CreateCell(13).SetCellValue(ResCategoryMaster.Category);
                row.CreateCell(14).SetCellValue(ResItemMaster.InventoryClassification);
                row.CreateCell(15).SetCellValue(ResItemMaster.AverageUsage);
                row.CreateCell(16).SetCellValue(ResItemMaster.Cost);
                row.CreateCell(17).SetCellValue(ResItemMaster.Markup);
                row.CreateCell(18).SetCellValue(ResItemMaster.SellPrice);
                row.CreateCell(19).SetCellValue(ResItemMaster.ExtendedCost);
                row.CreateCell(20).SetCellValue(ResItemMaster.LongDescription);
                row.CreateCell(21).SetCellValue(ResItemMaster.Supplier); 
                row.CreateCell(22).SetCellValue(ResItemMaster.SupplierPartNo);
                row.CreateCell(23).SetCellValue(ResItemMaster.ManufacturerName);
                row.CreateCell(24).SetCellValue(ResItemMaster.ManufacturerNumber);
                row.CreateCell(25).SetCellValue(ResItemMaster.UPC);
                row.CreateCell(26).SetCellValue(ResItemMaster.UNSPSC);
                row.CreateCell(27).SetCellValue(ResItemMaster.LeadTimeInDays);
                row.CreateCell(28).SetCellValue(ResItemMaster.CriticalQuantity);
                row.CreateCell(29).SetCellValue(ResItemMaster.SerialNumberTracking);
                row.CreateCell(30).SetCellValue(ResItemMaster.LotNumberTracking);
                row.CreateCell(31).SetCellValue(ResItemMaster.DateCodeTracking);
                row.CreateCell(32).SetCellValue(ResItemMaster.IsLotSerialExpiryCost);
                row.CreateCell(33).SetCellValue(ResGLAccount.GLAccount);
                row.CreateCell(34).SetCellValue(ResCommon.ID);
                row.CreateCell(35).SetCellValue(ResItemMaster.PricePerTerm);
                row.CreateCell(36).SetCellValue(ResItemMaster.DefaultReorderQuantity);
                row.CreateCell(37).SetCellValue(ResItemMaster.DefaultPullQuantity);
                row.CreateCell(38).SetCellValue(ResItemMaster.DefaultLocation);
                row.CreateCell(39).SetCellValue(ResItemMaster.Trend);
                row.CreateCell(40).SetCellValue(ResItemMaster.QtyToMeetDemand);
                row.CreateCell(41).SetCellValue(ResItemMaster.Taxable);
                row.CreateCell(42).SetCellValue(ResItemMaster.Consignment);
                row.CreateCell(43).SetCellValue(ResItemMaster.StagedQuantity);
                row.CreateCell(44).SetCellValue(ResItemMaster.WeightPerPiece);
                row.CreateCell(45).SetCellValue(ResItemMaster.IsTransfer);
                row.CreateCell(46).SetCellValue(ResItemMaster.IsPurchase);
                row.CreateCell(47).SetCellValue(ResCommon.RoomName);
                row.CreateCell(48).SetCellValue(ResCommon.CreatedOn);
                row.CreateCell(49).SetCellValue(ResCommon.UpdatedOn);
                row.CreateCell(50).SetCellValue(ResCommon.UpdatedBy);
                row.CreateCell(51).SetCellValue(ResCommon.CreatedBy);
                row.CreateCell(52).SetCellValue(ResUnitMaster.Unit);
                row.CreateCell(53).SetCellValue(ResItemMaster.AverageCost);
                row.CreateCell(54).SetCellValue(ResItemMaster.Link2);
                row.CreateCell(55).SetCellValue(ResItemMaster.TrendingSetting);
                row.CreateCell(56).SetCellValue(ResItemMaster.PullQtyScanOverride);
                row.CreateCell(57).SetCellValue(ResCommon.AddedFrom);
                row.CreateCell(58).SetCellValue(ResCommon.EditedFrom);
                row.CreateCell(59).SetCellValue(ResCommon.ReceivedOnDate);
                row.CreateCell(60).SetCellValue(ResCommon.ReceivedOnWebDate);
                //------------End-------------("CreatedBy");-------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ItemMasterDTO item in lstoflogs)
                {
                    string trendingsettingval = "";
                    if (item.TrendingSetting == null || item.TrendingSetting == 0)
                        trendingsettingval =  ResItemMaster.TrendingSettingNone;
                    else if (item.TrendingSetting == 1)
                        trendingsettingval = ResItemMaster.TrendingSettingManual;
                    else if (item.TrendingSetting == 2)
                        trendingsettingval = ResItemMaster.TrendingSettingAutomatic;
         
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.HistoryID);
                    DRow.CreateCell(1).SetCellValue(item.Action);
                    DRow.CreateCell(2).SetCellValue(item.ItemType);
                    DRow.CreateCell(3).SetCellValue(item.ItemNumber);
                    DRow.CreateCell(4).SetCellValue((item.IsOrderable == true ? "Yes" : "No"));
                    DRow.CreateCell(5).SetCellValue(item.Description);
                    DRow.CreateCell(6).SetCellValue(Convert.ToString(item.OnHandQuantity));
                    DRow.CreateCell(7).SetCellValue(Convert.ToString(item.OnOrderQuantity));
                    DRow.CreateCell(8).SetCellValue(Convert.ToString(item.OnOrderInTransitQuantity));
                    DRow.CreateCell(9).SetCellValue(Convert.ToString(item.OrderedDate));
                    DRow.CreateCell(10).SetCellValue((item.IsItemLevelMinMaxQtyRequired == true ? "Yes" : "No"));
                    DRow.CreateCell(11).SetCellValue(Convert.ToString(item.MinimumQuantity));
                    DRow.CreateCell(12).SetCellValue(item.MaximumQuantity.ToString());
                    DRow.CreateCell(13).SetCellValue(item.CategoryName);
                    DRow.CreateCell(14).SetCellValue(item.InventoryClassificationName);
                    DRow.CreateCell(15).SetCellValue(Convert.ToString(item.AverageUsage));
                    DRow.CreateCell(16).SetCellValue(Convert.ToString(item.Cost));
                    DRow.CreateCell(17).SetCellValue(Convert.ToString(item.Markup));
                    DRow.CreateCell(18).SetCellValue(Convert.ToString(item.SellPrice));
                    DRow.CreateCell(19).SetCellValue(Convert.ToString(item.ExtendedCost));
                    DRow.CreateCell(20).SetCellValue(Convert.ToString(item.LongDescription));
                    DRow.CreateCell(21).SetCellValue(Convert.ToString(item.SupplierName));
                    DRow.CreateCell(22).SetCellValue(Convert.ToString(item.SupplierPartNo));
                    DRow.CreateCell(23).SetCellValue(item.ManufacturerName);
                    DRow.CreateCell(24).SetCellValue(item.ManufacturerNumber);
                    DRow.CreateCell(25).SetCellValue(item.UPC);
                    DRow.CreateCell(26).SetCellValue(item.UNSPSC);
                    DRow.CreateCell(27).SetCellValue(Convert.ToString(item.LeadTimeInDays));
                    DRow.CreateCell(28).SetCellValue(Convert.ToString(item.CriticalQuantity));
                    DRow.CreateCell(29).SetCellValue((item.SerialNumberTracking == true ? "Yes" : "No"));
                    DRow.CreateCell(30).SetCellValue((item.LotNumberTracking == true ? "Yes" : "No"));
                    DRow.CreateCell(31).SetCellValue((item.DateCodeTracking == true ? "Yes" : "No"));
                    DRow.CreateCell(32).SetCellValue(Convert.ToString(item.IsLotSerialExpiryCost));
                    DRow.CreateCell(33).SetCellValue(item.GLAccount);
                    DRow.CreateCell(34).SetCellValue(Convert.ToString(item.ID));
                    DRow.CreateCell(35).SetCellValue(Convert.ToString(item.CostUOMName));
                    DRow.CreateCell(36).SetCellValue(Convert.ToString(item.DefaultReorderQuantity));
                    DRow.CreateCell(37).SetCellValue(Convert.ToString(item.DefaultPullQuantity));
                    DRow.CreateCell(38).SetCellValue(Convert.ToString(item.DefaultLocationName));
                    DRow.CreateCell(39).SetCellValue((item.Trend == true ? "Yes" : "No"));
                    DRow.CreateCell(40).SetCellValue(Convert.ToString(item.QtyToMeetDemand));
                    DRow.CreateCell(41).SetCellValue((item.Taxable == true ? "Yes" : "No"));
                    DRow.CreateCell(42).SetCellValue((item.Consignment == true ? "Yes" : "No"));
                    DRow.CreateCell(43).SetCellValue(Convert.ToString(item.StagedQuantity));
                    DRow.CreateCell(44).SetCellValue(Convert.ToString(item.WeightPerPiece));
                    DRow.CreateCell(45).SetCellValue((item.IsTransfer == true ? "Yes" : "No"));
                    DRow.CreateCell(46).SetCellValue((item.IsPurchase == true ? "Yes" : "No"));
                    DRow.CreateCell(47).SetCellValue(Convert.ToString(item.RoomName));
                    DRow.CreateCell(48).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(49).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(50).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(51).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(52).SetCellValue(item.Unit);
                    DRow.CreateCell(53).SetCellValue(Convert.ToString(item.AverageCost));
                    DRow.CreateCell(54).SetCellValue(item.Link2);
                    DRow.CreateCell(55).SetCellValue(trendingsettingval);
                    DRow.CreateCell(56).SetCellValue((item.IsPurchase == true ? "Yes" : "No"));
                    DRow.CreateCell(57).SetCellValue(Convert.ToString(item.AddedFrom));
                    DRow.CreateCell(58).SetCellValue(Convert.ToString(item.EditedFrom));
                    DRow.CreateCell(59).SetCellValue(Convert.ToString(item.ReceivedOnDate));
                    DRow.CreateCell(60).SetCellValue(Convert.ToString(item.ReceivedOnDateWeb));
                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }

        public string WrittenOffCategoryHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            WrittenOffCategoryDAL objdal=new WrittenOffCategoryDAL(SessionHelper.EnterPriseDBName);
            var lstoflogs = objdal.WrittenOffCategoryHistoryChangeLog(ids);
            if (lstoflogs != null && lstoflogs.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("ToolWrittenOffCategoryName");
                row.CreateCell(2).SetCellValue("CreatedOn");
                row.CreateCell(3).SetCellValue("UpdatedOn");
                row.CreateCell(4).SetCellValue("UpdatedBy");
                row.CreateCell(5).SetCellValue("CreatedBy");
                row.CreateCell(6).SetCellValue("AddedFrom");
                row.CreateCell(7).SetCellValue("EditedFrom");
                row.CreateCell(8).SetCellValue("ReceivedOnDate");
                row.CreateCell(9).SetCellValue("ReceivedOnWebDate");

                //------------End-------------("CreatedBy");-------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (WrittenOfCategoryDTO item in lstoflogs)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.WrittenOffCategory);
                    DRow.CreateCell(2).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(3).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(4).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(5).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(6).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(7).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(8).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(9).SetCellValue(item.ReceivedOnDateWeb);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string InventoryClassificationHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            InventoryClassificationMasterDAL obj = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
            List<InventoryClassificationMasterDTO> lstInventoryClassficationDTO = new List<InventoryClassificationMasterDTO>();
            lstInventoryClassficationDTO = obj.GetInventoryClassificationMasterChangeLog(ids);
            if (lstInventoryClassficationDTO != null && lstInventoryClassficationDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("InventoryClassification");
                row.CreateCell(2).SetCellValue("RangeStart");
                row.CreateCell(3).SetCellValue("RangeEnd");
                row.CreateCell(4).SetCellValue("RoomName");
                row.CreateCell(5).SetCellValue("AddedFrom");
                row.CreateCell(6).SetCellValue("EditedFrom");
                row.CreateCell(7).SetCellValue("ReceivedOnDate");
                row.CreateCell(8).SetCellValue("ReceivedOnWebDate");
                row.CreateCell(9).SetCellValue("CreatedOn");
                row.CreateCell(10).SetCellValue("UpdatedOn");
                row.CreateCell(11).SetCellValue("UpdatedBy");
                row.CreateCell(12).SetCellValue("CreatedBy");

                //------------End-------------("CreatedBy");-------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (InventoryClassificationMasterDTO item in lstInventoryClassficationDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.InventoryClassification);
                    DRow.CreateCell(2).SetCellValue(item.RangeStart ?? 0);
                    DRow.CreateCell(3).SetCellValue(item.RangeEnd ?? 0);
                    DRow.CreateCell(4).SetCellValue(item.RoomName);
                    DRow.CreateCell(5).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(6).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(7).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(8).SetCellValue(item.ReceivedOnWebDate);
                    DRow.CreateCell(9).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(10).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(11).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(12).SetCellValue(item.CreatedByName);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string LocationHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            LocationMasterDAL obj = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            List<LocationMasterDTO> lstLocationDTO = new List<LocationMasterDTO>();
            lstLocationDTO = obj.GetLocationMasterChangeLog(ids);
            if (lstLocationDTO != null && lstLocationDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("Location");
                row.CreateCell(2).SetCellValue("RoomName");
                row.CreateCell(3).SetCellValue("CreatedOn");
                row.CreateCell(4).SetCellValue("UpdatedOn");
                row.CreateCell(5).SetCellValue("UpdatedBy");
                row.CreateCell(6).SetCellValue("CreatedBy");

                //------------End-------------("CreatedBy");-------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (LocationMasterDTO item in lstLocationDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.Location);
                    DRow.CreateCell(2).SetCellValue(item.RoomName);
                    DRow.CreateCell(3).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(4).SetCellValue(item.LastUpdated.ToString());
                    DRow.CreateCell(5).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(6).SetCellValue(item.CreatedByName);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string ManufacturerHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            ManufacturerMasterDAL obj = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            List<ManufacturerMasterDTO> lstManufacturerDTO = new List<ManufacturerMasterDTO>();
            lstManufacturerDTO = obj.GetManufacturerMasterChangeLog(ids);
            if (lstManufacturerDTO != null && lstManufacturerDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("ManufacturerName");
                row.CreateCell(2).SetCellValue("RoomName");
                row.CreateCell(3).SetCellValue("CreatedOn");
                row.CreateCell(4).SetCellValue("UpdatedOn");
                row.CreateCell(5).SetCellValue("UpdatedBy");
                row.CreateCell(6).SetCellValue("CreatedBy");
                row.CreateCell(7).SetCellValue("AddedFrom");
                row.CreateCell(8).SetCellValue("EditedFrom");
                row.CreateCell(9).SetCellValue("ReceivedOnDate");
                row.CreateCell(10).SetCellValue("ReceivedOnWebDate");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ManufacturerMasterDTO item in lstManufacturerDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.Manufacturer);
                    DRow.CreateCell(2).SetCellValue(item.RoomName);
                    DRow.CreateCell(3).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(4).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(5).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(6).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(7).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(8).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(9).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(10).SetCellValue(item.ReceivedOnDateWeb);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string SupplierHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            SupplierMasterDAL obj = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            List<SupplierMasterDTO> lstSupplierDTO = new List<SupplierMasterDTO>();
            lstSupplierDTO = obj.GetSupplierMasterChangeLog(ids);
            if (lstSupplierDTO != null && lstSupplierDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("Supplier");
                row.CreateCell(2).SetCellValue("Description");
                row.CreateCell(3).SetCellValue("ReceiverID");
                row.CreateCell(4).SetCellValue("Address");
                row.CreateCell(5).SetCellValue("City");
                row.CreateCell(6).SetCellValue("State");
                row.CreateCell(7).SetCellValue("ZipCode");
                row.CreateCell(8).SetCellValue("Country");
                row.CreateCell(9).SetCellValue("Contact");
                row.CreateCell(10).SetCellValue("Phone");
                row.CreateCell(11).SetCellValue("Fax");
                row.CreateCell(12).SetCellValue("Email");
                row.CreateCell(13).SetCellValue("DefaultOrderRequiredDays");
                row.CreateCell(14).SetCellValue("BranchNumber");
                row.CreateCell(15).SetCellValue("MaximumOrderSize");
                row.CreateCell(16).SetCellValue("IsSendtoVendor");
                row.CreateCell(17).SetCellValue("IsVendorReturnAsn");
                row.CreateCell(18).SetCellValue("IsSupplierReceivesKitComponents");
                row.CreateCell(19).SetCellValue("EmailPOInBody");
                row.CreateCell(20).SetCellValue("EmailPOInPDF");
                row.CreateCell(21).SetCellValue("EmailPOInCSV");
                row.CreateCell(22).SetCellValue("EmailPOInX12");
                row.CreateCell(23).SetCellValue("RoomName");
                row.CreateCell(24).SetCellValue("CreatedOn");
                row.CreateCell(25).SetCellValue("UpdatedOn");
                row.CreateCell(26).SetCellValue("UpdatedBy");
                row.CreateCell(27).SetCellValue("CreatedBy");
                row.CreateCell(28).SetCellValue("AddedFrom");
                row.CreateCell(29).SetCellValue("EditedFrom");
                row.CreateCell(30).SetCellValue("ReceivedOnDate");
                row.CreateCell(31).SetCellValue("ReceivedOnWebDate");
                row.CreateCell(32).SetCellValue("Account");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (SupplierMasterDTO item in lstSupplierDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.SupplierName);
                    DRow.CreateCell(2).SetCellValue(item.Description);
                    DRow.CreateCell(3).SetCellValue(item.ReceiverID);
                    DRow.CreateCell(4).SetCellValue(item.Address);
                    DRow.CreateCell(5).SetCellValue(item.City);
                    DRow.CreateCell(6).SetCellValue(item.State);
                    DRow.CreateCell(7).SetCellValue(item.ZipCode);
                    DRow.CreateCell(8).SetCellValue(item.Country);
                    DRow.CreateCell(9).SetCellValue(item.Contact);
                    DRow.CreateCell(10).SetCellValue(item.Phone);
                    DRow.CreateCell(11).SetCellValue(item.Fax);
                    DRow.CreateCell(12).SetCellValue(item.Email);
                    DRow.CreateCell(13).SetCellValue(item.DefaultOrderRequiredDays ?? 0);
                    DRow.CreateCell(14).SetCellValue(item.BranchNumber);
                    DRow.CreateCell(15).SetCellValue(Convert.ToDouble(item.MaximumOrderSize));
                    DRow.CreateCell(16).SetCellValue(item.IsSendtoVendor == true ? "Yes" : "No");
                    DRow.CreateCell(17).SetCellValue(item.IsVendorReturnAsn == true ? "Yes" : "No");
                    DRow.CreateCell(18).SetCellValue(item.IsSupplierReceivesKitComponents == true ? "Yes" : "No");
                    DRow.CreateCell(19).SetCellValue(item.IsEmailPOInBody == true ? "Yes" : "No");
                    DRow.CreateCell(20).SetCellValue(item.IsEmailPOInPDF == true ? "Yes" : "No");
                    DRow.CreateCell(21).SetCellValue(item.IsEmailPOInCSV == true ? "Yes" : "No");
                    DRow.CreateCell(22).SetCellValue(item.IsEmailPOInX12 == true ? "Yes" : "No");
                    DRow.CreateCell(23).SetCellValue(item.RoomName);
                    DRow.CreateCell(24).SetCellValue(item.Created);
                    DRow.CreateCell(25).SetCellValue(item.LastUpdated.ToString());
                    DRow.CreateCell(26).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(27).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(28).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(29).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(30).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(31).SetCellValue(item.ReceivedOnDateWeb);
                    DRow.CreateCell(32).SetCellValue(item.AccountNo);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string TechnicianHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            TechnicialMasterDAL obj = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            List<TechnicianMasterDTO> lstTechnicianDTO = new List<TechnicianMasterDTO>();
            lstTechnicianDTO = obj.GetTechnicianMasterChangeLog(ids);
            if (lstTechnicianDTO != null && lstTechnicianDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("Technician");
                row.CreateCell(1).SetCellValue("CreatedOn");
                row.CreateCell(2).SetCellValue("UpdatedOn");
                row.CreateCell(3).SetCellValue("UpdatedBy");
                row.CreateCell(4).SetCellValue("CreatedBy");
                row.CreateCell(5).SetCellValue("Action");
                //------------End-------------("CreatedBy");-------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (TechnicianMasterDTO item in lstTechnicianDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Technician);
                    DRow.CreateCell(1).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(2).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(3).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(4).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(5).SetCellValue(item.Action);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string ToolCategoryHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            ToolCategoryMasterDAL obj = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolCategoryMasterDTO> lstToolCategoryDTO = new List<ToolCategoryMasterDTO>();
            lstToolCategoryDTO = obj.GetToolCategoryMasterChangeLog(ids);
            if (lstToolCategoryDTO != null && lstToolCategoryDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("ToolCategory");
                row.CreateCell(2).SetCellValue("RoomName");
                row.CreateCell(3).SetCellValue("CreatedOn");
                row.CreateCell(4).SetCellValue("UpdatedOn");
                row.CreateCell(5).SetCellValue("UpdatedBy");
                row.CreateCell(6).SetCellValue("CreatedBy");
                row.CreateCell(7).SetCellValue("AddedFrom");
                row.CreateCell(8).SetCellValue("EditedFrom");
                row.CreateCell(9).SetCellValue("ReceivedOnDate");
                row.CreateCell(10).SetCellValue("ReceivedOnWebDate");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ToolCategoryMasterDTO item in lstToolCategoryDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.ToolCategory);
                    DRow.CreateCell(2).SetCellValue(item.RoomName);
                    DRow.CreateCell(3).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(4).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(5).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(6).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(7).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(8).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(9).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(10).SetCellValue(item.ReceivedOnDateWeb);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string UnitHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            UnitMasterDAL obj = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            List<UnitMasterDTO> lstUnitDTO = new List<UnitMasterDTO>();
            lstUnitDTO = obj.GetUnitMasterChangeLog(ids);
            if (lstUnitDTO != null && lstUnitDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("Unit");
                row.CreateCell(2).SetCellValue("Description");
                row.CreateCell(3).SetCellValue("RoomName");
                row.CreateCell(4).SetCellValue("CreatedOn");
                row.CreateCell(5).SetCellValue("UpdatedOn");
                row.CreateCell(6).SetCellValue("UpdatedBy");
                row.CreateCell(7).SetCellValue("CreatedBy");
                row.CreateCell(8).SetCellValue("AddedFrom");
                row.CreateCell(9).SetCellValue("EditedFrom");
                row.CreateCell(10).SetCellValue("ReceivedOnDate");
                row.CreateCell(11).SetCellValue("ReceivedOnWebDate");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (UnitMasterDTO item in lstUnitDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.Unit);
                    DRow.CreateCell(2).SetCellValue(item.Description);
                    DRow.CreateCell(3).SetCellValue(item.RoomName);
                    DRow.CreateCell(4).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(5).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(6).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(7).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(8).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(9).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(10).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(11).SetCellValue(item.ReceivedOnDateWeb);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string VenderHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            VenderMasterDAL obj = new VenderMasterDAL(SessionHelper.EnterPriseDBName);
            List<VenderMasterDTO> lstVenderDTO = new List<VenderMasterDTO>();
            lstVenderDTO = obj.GetVenderMasterChangeLog(ids);
            if (lstVenderDTO != null && lstVenderDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("ID");
                row.CreateCell(1).SetCellValue("HistoryID");
                row.CreateCell(2).SetCellValue("HistoryAction");
                row.CreateCell(3).SetCellValue("Vender");
                row.CreateCell(4).SetCellValue("RoomName");
                row.CreateCell(5).SetCellValue("CreatedOn");
                row.CreateCell(6).SetCellValue("UpdatedOn");
                row.CreateCell(7).SetCellValue("UpdatedBy");
                row.CreateCell(8).SetCellValue("CreatedBy");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (VenderMasterDTO item in lstVenderDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.HistoryID);
                    DRow.CreateCell(2).SetCellValue(item.Action);
                    DRow.CreateCell(3).SetCellValue(item.Vender);
                    DRow.CreateCell(4).SetCellValue(item.RoomName);
                    DRow.CreateCell(5).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(6).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(7).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(8).SetCellValue(item.CreatedByName);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string QuickListHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            QuickListDAL obj = new QuickListDAL(SessionHelper.EnterPriseDBName);
            List<QuickListMasterDTO> lstQuickListDTO = new List<QuickListMasterDTO>();
            lstQuickListDTO = obj.GetQuickListMasterChangeLog(ids);
            if (lstQuickListDTO != null && lstQuickListDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("Name");
                row.CreateCell(2).SetCellValue("FromWhere");
                row.CreateCell(3).SetCellValue("Comment");
                row.CreateCell(4).SetCellValue("ListType");
                row.CreateCell(5).SetCellValue("NoOfItems");
                row.CreateCell(6).SetCellValue("RoomName");
                row.CreateCell(7).SetCellValue("AddedFrom");
                row.CreateCell(8).SetCellValue("EditedFrom");
                row.CreateCell(9).SetCellValue("ReceivedOnDate");
                row.CreateCell(10).SetCellValue("ReceivedOnWebDate");
                row.CreateCell(11).SetCellValue("UpdatedBy");
                row.CreateCell(12).SetCellValue("CreatedBy");
                row.CreateCell(13).SetCellValue("CreatedOn");
                row.CreateCell(14).SetCellValue("UpdatedOn");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (QuickListMasterDTO item in lstQuickListDTO)
                {
                    string Type = item.Type == 1 ? "General" : (item.Type == 2 ? "Asset" : "");
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.Name);
                    DRow.CreateCell(2).SetCellValue(item.WhatWhereAction);
                    DRow.CreateCell(3).SetCellValue(item.Comment);
                    DRow.CreateCell(4).SetCellValue(Type);
                    DRow.CreateCell(5).SetCellValue(item.NoOfItems);
                    DRow.CreateCell(6).SetCellValue(item.RoomName);
                    DRow.CreateCell(7).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(8).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(9).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(10).SetCellValue(item.ReceivedOnDateWeb);
                    DRow.CreateCell(11).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(12).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(13).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(14).SetCellValue(item.LastUpdated.ToString());


                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string CountHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            InventoryCountDAL obj = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            List<InventoryCountDTO> lstCountDTO = new List<InventoryCountDTO>();
            lstCountDTO = obj.GetInventoryCountChangeLog(ids);
            if (lstCountDTO != null && lstCountDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("CountName");
                row.CreateCell(1).SetCellValue("ReleaseNumber");
                row.CreateCell(2).SetCellValue("CountDate");
                row.CreateCell(3).SetCellValue("CountItemDescription");
                row.CreateCell(4).SetCellValue("CountType");
                row.CreateCell(5).SetCellValue("IsApplied");
                row.CreateCell(6).SetCellValue("TotalItemsWithinCount");
                row.CreateCell(7).SetCellValue("IsClosed");
                row.CreateCell(8).SetCellValue("Created");
                row.CreateCell(9).SetCellValue("Updated");
                row.CreateCell(10).SetCellValue("LastUpdatedBy");
                row.CreateCell(11).SetCellValue("CreatedBy");
                row.CreateCell(12).SetCellValue("AddedFrom");
                row.CreateCell(13).SetCellValue("EditedFrom");
                row.CreateCell(14).SetCellValue("ReceivedOnDate");
                row.CreateCell(15).SetCellValue("ReceivedOnWebDate");


                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (InventoryCountDTO item in lstCountDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.CountName);
                    DRow.CreateCell(1).SetCellValue(item.ReleaseNumber);
                    DRow.CreateCell(2).SetCellValue(item.CountDate);
                    DRow.CreateCell(3).SetCellValue(item.CountItemDescription);
                    DRow.CreateCell(4).SetCellValue(item.CountType);
                    DRow.CreateCell(5).SetCellValue(item.IsApplied == true ? "Yes" : "No");
                    DRow.CreateCell(6).SetCellValue(item.TotalItemsWithinCount);
                    DRow.CreateCell(7).SetCellValue(item.IsClosed == true ? "Yes" : "No");
                    DRow.CreateCell(8).SetCellValue(item.Created);
                    DRow.CreateCell(9).SetCellValue(item.Updated);
                    DRow.CreateCell(10).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(11).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(12).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(13).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(14).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(15).SetCellValue(item.ReceivedOnDateWeb);


                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string MaterialStagingHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            MaterialStagingDAL obj = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
            List<MaterialStagingDTO> lstMaterialStagingDTO = new List<MaterialStagingDTO>();
            lstMaterialStagingDTO = obj.GetMaterialStagingChangeLog(ids);
            if (lstMaterialStagingDTO != null && lstMaterialStagingDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("StagingName");
                row.CreateCell(2).SetCellValue("From Where");
                row.CreateCell(3).SetCellValue("Description");
                row.CreateCell(4).SetCellValue("BinName");
                row.CreateCell(5).SetCellValue("CreatedOn");
                row.CreateCell(6).SetCellValue("UpdatedOn");
                row.CreateCell(7).SetCellValue("CreatedBy");
                row.CreateCell(8).SetCellValue("UpdatedBy");
                row.CreateCell(9).SetCellValue("RoomName");
                row.CreateCell(10).SetCellValue("AddedFrom");
                row.CreateCell(11).SetCellValue("EditedFrom");
                row.CreateCell(12).SetCellValue("ReceivedOnDate");
                row.CreateCell(13).SetCellValue("ReceivedOnWebDate");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (MaterialStagingDTO item in lstMaterialStagingDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.StagingName);
                    DRow.CreateCell(2).SetCellValue(item.WhatWhereAction);
                    DRow.CreateCell(3).SetCellValue(item.Description);
                    DRow.CreateCell(4).SetCellValue(item.StagingLocationName);
                    DRow.CreateCell(5).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(6).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(7).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(8).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(9).SetCellValue(item.RoomName);
                    DRow.CreateCell(10).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(11).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(12).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(13).SetCellValue(item.ReceivedOnDateWeb);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string RequisitionHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            RequisitionMasterDAL obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            List<RequisitionMasterDTO> lstRequisitionDTO = new List<RequisitionMasterDTO>();
            lstRequisitionDTO = obj.GetRequisitionMasterChangeLog(ids);
            if (lstRequisitionDTO != null && lstRequisitionDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("RequisitionStatus");
                row.CreateCell(2).SetCellValue("From Where");
                row.CreateCell(3).SetCellValue("RequisitionNumber");
                row.CreateCell(4).SetCellValue("ReleaseNumber");
                row.CreateCell(5).SetCellValue("Description");
                row.CreateCell(6).SetCellValue("RequiredDate");
                row.CreateCell(7).SetCellValue("NumberofItemsrequisitioned");
                row.CreateCell(8).SetCellValue("Customer");
                row.CreateCell(9).SetCellValue("RequisitionType");
                row.CreateCell(10).SetCellValue("RoomName");
                row.CreateCell(11).SetCellValue("CreatedOn");
                row.CreateCell(12).SetCellValue("UpdatedOn");
                row.CreateCell(13).SetCellValue("CreatedBy");
                row.CreateCell(14).SetCellValue("UpdatedBy");
                row.CreateCell(15).SetCellValue("AddedFrom");
                row.CreateCell(16).SetCellValue("EditedFrom");
                row.CreateCell(17).SetCellValue("ReceivedOnDate");
                row.CreateCell(18).SetCellValue("ReceivedOnWebDate");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (RequisitionMasterDTO item in lstRequisitionDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.RequisitionStatus);
                    DRow.CreateCell(2).SetCellValue(item.WhatWhereAction);
                    DRow.CreateCell(3).SetCellValue(item.RequisitionNumber);
                    DRow.CreateCell(4).SetCellValue(item.ReleaseNumber);
                    DRow.CreateCell(5).SetCellValue(item.Description);
                    DRow.CreateCell(6).SetCellValue(item.RequiredDate.ToString());
                    DRow.CreateCell(7).SetCellValue(item.NumberofItemsrequisitioned ?? 0);
                    DRow.CreateCell(8).SetCellValue(item.Customer);
                    DRow.CreateCell(9).SetCellValue(item.RequisitionType);
                    DRow.CreateCell(10).SetCellValue(item.RoomName);
                    DRow.CreateCell(11).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(12).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(13).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(14).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(15).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(16).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(17).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(18).SetCellValue(item.ReceivedOnWebDate);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string WorkOrderHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            List<WorkOrderDTO> lstWorkOrderDTO = new List<WorkOrderDTO>();
            lstWorkOrderDTO = obj.GetWorkOrderMasterChangeLog(ids);
            if (lstWorkOrderDTO != null && lstWorkOrderDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("WOName");
                row.CreateCell(2).SetCellValue("ReleaseNumber");
                row.CreateCell(3).SetCellValue("From Where");
                row.CreateCell(4).SetCellValue("WOStatus");
                row.CreateCell(5).SetCellValue("Technician");
                row.CreateCell(6).SetCellValue("Customer");
                row.CreateCell(7).SetCellValue("AssetName");
                row.CreateCell(8).SetCellValue("ToolName");
                row.CreateCell(9).SetCellValue("UsedItems");
                row.CreateCell(10).SetCellValue("UsedItemsCost");
                row.CreateCell(11).SetCellValue("Description");
                row.CreateCell(12).SetCellValue("RoomName");
                row.CreateCell(13).SetCellValue("CreatedOn");
                row.CreateCell(14).SetCellValue("UpdatedOn");
                row.CreateCell(15).SetCellValue("Created");
                row.CreateCell(16).SetCellValue("Updated");
                row.CreateCell(17).SetCellValue("AddedFrom");
                row.CreateCell(18).SetCellValue("EditedFrom");
                row.CreateCell(19).SetCellValue("ReceivedOnDate");
                row.CreateCell(20).SetCellValue("ReceivedOnWebDate");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (WorkOrderDTO item in lstWorkOrderDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.WOName);
                    DRow.CreateCell(2).SetCellValue(item.ReleaseNumber);
                    DRow.CreateCell(3).SetCellValue(item.WhatWhereAction);
                    DRow.CreateCell(4).SetCellValue(item.WOStatus);
                    DRow.CreateCell(5).SetCellValue(item.Technician);
                    DRow.CreateCell(6).SetCellValue(item.Customer);
                    DRow.CreateCell(7).SetCellValue(item.AssetName);
                    DRow.CreateCell(8).SetCellValue(item.ToolName);
                    DRow.CreateCell(9).SetCellValue(item.UsedItems ?? 0);
                    DRow.CreateCell(10).SetCellValue(item.UsedItemsCost ?? 0);
                    DRow.CreateCell(11).SetCellValue(item.Description);
                    DRow.CreateCell(12).SetCellValue(item.RoomName);
                    DRow.CreateCell(13).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(14).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(15).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(16).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(17).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(18).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(19).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(20).SetCellValue(item.ReceivedOnWebDate);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string ProjectSpendHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            ProjectMasterDAL obj = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            List<ProjectMasterDTO> lstProjectSpendDTO = new List<ProjectMasterDTO>();
            lstProjectSpendDTO = obj.GetProjectSpendMasterChangeLog(ids);
            if (lstProjectSpendDTO != null && lstProjectSpendDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("ProjectSpendName");
                row.CreateCell(2).SetCellValue("From Where");
                row.CreateCell(3).SetCellValue("Description");
                row.CreateCell(4).SetCellValue("DollarLimitAmount");
                row.CreateCell(5).SetCellValue("DollarUsedAmount");
                row.CreateCell(6).SetCellValue("TrackAllUsageAgainstThis");
                row.CreateCell(7).SetCellValue("IsClosed");
                row.CreateCell(8).SetCellValue("RoomName");
                row.CreateCell(9).SetCellValue("AddedFrom");
                row.CreateCell(10).SetCellValue("EditedFrom");
                row.CreateCell(11).SetCellValue("ReceivedOnDate");
                row.CreateCell(12).SetCellValue("ReceivedOnWebDate");
                row.CreateCell(13).SetCellValue("CreatedOn");
                row.CreateCell(14).SetCellValue("UpdatedOn");
                row.CreateCell(15).SetCellValue("Created");
                row.CreateCell(16).SetCellValue("Updated");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ProjectMasterDTO item in lstProjectSpendDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.ProjectSpendName);
                    DRow.CreateCell(2).SetCellValue(item.WhatWhereAction);
                    DRow.CreateCell(3).SetCellValue(item.Description);
                    DRow.CreateCell(4).SetCellValue(Convert.ToDouble(item.DollarLimitAmount));
                    DRow.CreateCell(5).SetCellValue(Convert.ToDouble(item.DollarUsedAmount));
                    DRow.CreateCell(6).SetCellValue(item.TrackAllUsageAgainstThis == true ? "Yes" : "No");
                    DRow.CreateCell(7).SetCellValue(item.IsClosed == true ? "Yes" : "No");
                    DRow.CreateCell(8).SetCellValue(item.RoomName);
                    DRow.CreateCell(9).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(10).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(11).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(12).SetCellValue(item.ReceivedOnWebDate);
                    DRow.CreateCell(13).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(14).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(15).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(16).SetCellValue(item.UpdatedByName);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string CartItemsHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            CartItemDAL obj = new CartItemDAL(SessionHelper.EnterPriseDBName);
            List<CartItemDTO> lstCartItemsDTO = new List<CartItemDTO>();
            lstCartItemsDTO = obj.GetCartItemsMasterChangeLog(ids);
            if (lstCartItemsDTO != null && lstCartItemsDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("ItemNumber");
                row.CreateCell(1).SetCellValue("Quantity");
                row.CreateCell(2).SetCellValue("From Where");
                row.CreateCell(3).SetCellValue("ReplenishType");
                row.CreateCell(4).SetCellValue("CreatedOn");
                row.CreateCell(5).SetCellValue("UpdatedOn");
                row.CreateCell(6).SetCellValue("UpdatedBy");
                row.CreateCell(7).SetCellValue("CreatedBy");
                row.CreateCell(8).SetCellValue("AddedFrom");
                row.CreateCell(9).SetCellValue("EditedFrom");
                row.CreateCell(10).SetCellValue("ReceivedOnDate");
                row.CreateCell(11).SetCellValue("ReceivedOnWebDate");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (CartItemDTO item in lstCartItemsDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.ItemNumber);
                    DRow.CreateCell(1).SetCellValue(item.Quantity ?? 0);
                    DRow.CreateCell(2).SetCellValue(item.WhatWhereAction);
                    DRow.CreateCell(3).SetCellValue(item.ReplenishType);
                    DRow.CreateCell(4).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(5).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(6).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(7).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(8).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(9).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(10).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(11).SetCellValue(item.ReceivedOnDateWeb);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string OrdersHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            OrderMasterDAL obj = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            List<OrderMasterDTO> lstOrdersDTO = new List<OrderMasterDTO>();
            lstOrdersDTO = obj.GetOrdersMasterChangeLog(ids);
            if (lstOrdersDTO != null && lstOrdersDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("OrderNumber");
                row.CreateCell(2).SetCellValue("From Where");
                row.CreateCell(3).SetCellValue("ReleaseNumber");
                row.CreateCell(4).SetCellValue("ShippingMethod");
                row.CreateCell(5).SetCellValue("Supplier");
                row.CreateCell(6).SetCellValue("StagingName");
                row.CreateCell(7).SetCellValue("Comment");
                row.CreateCell(8).SetCellValue("RequiredDate");
                row.CreateCell(9).SetCellValue("OrderStatus");
                row.CreateCell(10).SetCellValue("RejectedReason");
                row.CreateCell(11).SetCellValue("Customer");
                row.CreateCell(12).SetCellValue("PackSlipNumber");
                row.CreateCell(13).SetCellValue("ShippingTrackNumber");
                row.CreateCell(14).SetCellValue("CreatedOn");
                row.CreateCell(15).SetCellValue("CreatedBy");
                row.CreateCell(16).SetCellValue("UpdatedOn");
                row.CreateCell(17).SetCellValue("UpdatedBy");
                row.CreateCell(18).SetCellValue("RoomName");
                row.CreateCell(19).SetCellValue("IsArchived");
                row.CreateCell(20).SetCellValue("IsDeleted");
                row.CreateCell(21).SetCellValue("AddedFrom");
                row.CreateCell(22).SetCellValue("EditedFrom");
                row.CreateCell(23).SetCellValue("ReceivedOnDate");
                row.CreateCell(24).SetCellValue("ReceivedOnWebDate");


                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (OrderMasterDTO item in lstOrdersDTO)
                {
                    string OrderStatus = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)item.OrderStatus).ToString());
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.OrderNumber);
                    DRow.CreateCell(2).SetCellValue(item.WhatWhereAction);
                    DRow.CreateCell(3).SetCellValue(item.ReleaseNumber);
                    DRow.CreateCell(4).SetCellValue(item.ShipVia ?? 0);
                    DRow.CreateCell(5).SetCellValue(item.Supplier ?? 0);
                    DRow.CreateCell(6).SetCellValue(item.StagingName);
                    DRow.CreateCell(7).SetCellValue(item.Comment);
                    DRow.CreateCell(8).SetCellValue(item.RequiredDate);
                    DRow.CreateCell(9).SetCellValue(OrderStatus);
                    DRow.CreateCell(10).SetCellValue(item.RejectionReason);
                    DRow.CreateCell(11).SetCellValue(item.CustomerName);
                    DRow.CreateCell(12).SetCellValue(item.PackSlipNumber);
                    DRow.CreateCell(13).SetCellValue(item.ShippingTrackNumber);
                    DRow.CreateCell(14).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(15).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(16).SetCellValue(item.LastUpdated.ToString());
                    DRow.CreateCell(17).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(18).SetCellValue(item.RoomName);
                    DRow.CreateCell(19).SetCellValue(item.IsArchived == true ? "Yes" : "No");
                    DRow.CreateCell(20).SetCellValue(item.IsDeleted == true ? "Yes" : "No");
                    DRow.CreateCell(21).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(22).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(23).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(24).SetCellValue(item.ReceivedOnDateWeb);



                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string TransferHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            TransferMasterDAL obj = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            List<TransferMasterDTO> lstTransferDTO = new List<TransferMasterDTO>();
            lstTransferDTO = obj.GetTransferMasterChangeLog(ids);
            if (lstTransferDTO != null && lstTransferDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("TransferNumber");
                row.CreateCell(2).SetCellValue("From Where");
                row.CreateCell(3).SetCellValue("ReplinishRoom");
                row.CreateCell(4).SetCellValue("StagingName");
                row.CreateCell(5).SetCellValue("Comment");
                row.CreateCell(6).SetCellValue("RequestType");
                row.CreateCell(7).SetCellValue("TransferStatus");
                row.CreateCell(8).SetCellValue("RejectedReason");
                row.CreateCell(9).SetCellValue("RequireDate");
                row.CreateCell(10).SetCellValue("CreatedOn");
                row.CreateCell(11).SetCellValue("UpdatedOn");
                row.CreateCell(12).SetCellValue("CreatedBy");
                row.CreateCell(13).SetCellValue("UpdatedBy");
                row.CreateCell(14).SetCellValue("RoomName");
                row.CreateCell(15).SetCellValue("AddedFrom");
                row.CreateCell(16).SetCellValue("EditedFrom");
                row.CreateCell(17).SetCellValue("ReceivedOnDate");
                row.CreateCell(18).SetCellValue("ReceivedOnWebDate");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (TransferMasterDTO item in lstTransferDTO)
                {
                    string TransferStatus = ResTransfer.GetTransferStatusText(((eTurns.DTO.TransferStatus)item.TransferStatus).ToString());
                    string RequestType = (Enum.Parse(typeof(RequestType), item.RequestType.ToString()).ToString());
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.TransferNumber);
                    DRow.CreateCell(2).SetCellValue(item.WhatWhereAction);
                    DRow.CreateCell(3).SetCellValue(item.ReplenishingRoomName);
                    DRow.CreateCell(4).SetCellValue(item.StagingName);
                    DRow.CreateCell(5).SetCellValue(item.Comment);
                    DRow.CreateCell(6).SetCellValue(RequestType);
                    DRow.CreateCell(7).SetCellValue(TransferStatus);
                    DRow.CreateCell(8).SetCellValue(item.RejectionReason);
                    DRow.CreateCell(9).SetCellValue(item.RequireDate);
                    DRow.CreateCell(10).SetCellValue(item.Created);
                    DRow.CreateCell(11).SetCellValue(item.Updated);
                    DRow.CreateCell(12).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(13).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(14).SetCellValue(item.RoomName);
                    DRow.CreateCell(15).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(16).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(17).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(18).SetCellValue(item.ReceivedOnWebDate);



                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string Asset_ToolHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstToolDTO = new List<ToolMasterDTO>();
            lstToolDTO = obj.GetToolMasterChangeLog(ids);
            if (lstToolDTO != null && lstToolDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("ToolName");
                row.CreateCell(2).SetCellValue("Serial");
                row.CreateCell(3).SetCellValue("Description");
                row.CreateCell(4).SetCellValue("IsGroupOfItems");
                row.CreateCell(5).SetCellValue("Quantity");
                row.CreateCell(6).SetCellValue("Cost");
                row.CreateCell(7).SetCellValue("ToolCategory");
                row.CreateCell(8).SetCellValue("Location");
                row.CreateCell(9).SetCellValue("RoomName");
                row.CreateCell(10).SetCellValue("CreatedOn");
                row.CreateCell(11).SetCellValue("UpdatedOn");
                row.CreateCell(12).SetCellValue("UpdatedBy");
                row.CreateCell(13).SetCellValue("CreatedBy");
                row.CreateCell(14).SetCellValue("AddedFrom");
                row.CreateCell(15).SetCellValue("EditedFrom");
                row.CreateCell(16).SetCellValue("ReceivedOnDate");
                row.CreateCell(17).SetCellValue("ReceivedOnWebDate");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ToolMasterDTO item in lstToolDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.ToolName);
                    DRow.CreateCell(2).SetCellValue(item.Serial);
                    DRow.CreateCell(3).SetCellValue(item.Description);
                    DRow.CreateCell(4).SetCellValue(item.IsGroupOfItems == 0 ? "No" : "Yes");
                    DRow.CreateCell(5).SetCellValue(item.Quantity);
                    DRow.CreateCell(6).SetCellValue(item.Cost ?? 0);
                    DRow.CreateCell(7).SetCellValue(item.ToolCategory);
                    DRow.CreateCell(8).SetCellValue(item.Location);
                    DRow.CreateCell(9).SetCellValue(item.RoomName);
                    DRow.CreateCell(10).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(11).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(12).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(13).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(14).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(15).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(16).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(17).SetCellValue(item.ReceivedOnDateWeb);




                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string AssetsHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            AssetMasterDAL obj = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            List<AssetMasterDTO> lstAssetsDTO = new List<AssetMasterDTO>();
            lstAssetsDTO = obj.GetAssetMasterChangeLog(ids);
            if (lstAssetsDTO != null && lstAssetsDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryAction");
                row.CreateCell(1).SetCellValue("AssetName");
                row.CreateCell(2).SetCellValue("Description");
                row.CreateCell(3).SetCellValue("Make");
                row.CreateCell(4).SetCellValue("Model");
                row.CreateCell(5).SetCellValue("Serial");
                row.CreateCell(6).SetCellValue("ToolCategory");
                row.CreateCell(7).SetCellValue("PurchaseDate");
                row.CreateCell(8).SetCellValue("PurchasePrice");
                row.CreateCell(9).SetCellValue("DepreciatedValue");
                row.CreateCell(10).SetCellValue("SuggestedMaintenanceDate");
                row.CreateCell(11).SetCellValue("RoomName");
                row.CreateCell(12).SetCellValue("CreatedOn");
                row.CreateCell(13).SetCellValue("UpdatedOn");
                row.CreateCell(14).SetCellValue("UpdatedBy");
                row.CreateCell(15).SetCellValue("CreatedBy");
                row.CreateCell(16).SetCellValue("AddedFrom");
                row.CreateCell(17).SetCellValue("EditedFrom");
                row.CreateCell(18).SetCellValue("ReceivedOnDate");
                row.CreateCell(19).SetCellValue("ReceivedOnWebDate");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (AssetMasterDTO item in lstAssetsDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.AssetName);
                    DRow.CreateCell(2).SetCellValue(item.Description);
                    DRow.CreateCell(3).SetCellValue(item.Make);
                    DRow.CreateCell(4).SetCellValue(item.Model);
                    DRow.CreateCell(5).SetCellValue(item.Serial);
                    DRow.CreateCell(6).SetCellValue(item.ToolCategory);
                    DRow.CreateCell(7).SetCellValue(item.PurchaseDate.ToString());
                    DRow.CreateCell(8).SetCellValue(item.PurchasePrice ?? 0);
                    DRow.CreateCell(9).SetCellValue(item.DepreciatedValue ?? 0);
                    DRow.CreateCell(10).SetCellValue(item.SuggestedMaintenanceDate.ToString());
                    DRow.CreateCell(11).SetCellValue(item.RoomName);
                    DRow.CreateCell(12).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(13).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(14).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(15).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(16).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(17).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(18).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(19).SetCellValue(item.ReceivedOnDateWeb);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string NotificationHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            //string[] arrid = ids.Split(',');
            NotificationDAL obj = new NotificationDAL(SessionHelper.EnterPriseDBName);
            //List<NotificationDTO> lstNotificationDTO = new List<NotificationDTO>();
            var lstNotificationDTO = obj.GetNotificationMasterChangeLog(ids);
            if (lstNotificationDTO != null && lstNotificationDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("HistoryID");
                row.CreateCell(1).SetCellValue("HistoryAction");
                row.CreateCell(2).SetCellValue("ScheduleID");
                row.CreateCell(3).SetCellValue("ScheduleName");
                row.CreateCell(4).SetCellValue("TemplateName");
                row.CreateCell(5).SetCellValue("ReportName");
                row.CreateCell(6).SetCellValue("EmailAddress");
                row.CreateCell(7).SetCellValue("EmailSubject");
                row.CreateCell(8).SetCellValue("IsActive");
                row.CreateCell(9).SetCellValue("NextRunDate");
                row.CreateCell(10).SetCellValue("CreatedOn");
                row.CreateCell(11).SetCellValue("UpdatedOn");
                row.CreateCell(12).SetCellValue("LastUpdatedBy");
                row.CreateCell(13).SetCellValue("CreatedBy");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (NotificationDTO item in lstNotificationDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.HistoryID);
                    DRow.CreateCell(1).SetCellValue(item.Action);
                    DRow.CreateCell(2).SetCellValue(item.ID);
                    DRow.CreateCell(3).SetCellValue(item.ScheduleName);
                    DRow.CreateCell(4).SetCellValue(item.TemplateName);
                    DRow.CreateCell(5).SetCellValue(item.ReportName);
                    DRow.CreateCell(6).SetCellValue(item.EmailAddress);
                    DRow.CreateCell(7).SetCellValue(item.EmailSubject);
                    DRow.CreateCell(8).SetCellValue(item.IsActive == true ? "Yes" : "No");
                    DRow.CreateCell(9).SetCellValue(Convert.ToString(item.NextRunDate));
                    DRow.CreateCell(10).SetCellValue(item.Created);
                    DRow.CreateCell(11).SetCellValue(item.Updated);
                    DRow.CreateCell(12).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(13).SetCellValue(item.CreatedByName);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string NotificationListExcel(string Filepath, string ModuleName, string Ids, string CurrentCulture)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            NotificationDAL obj = new NotificationDAL(SessionHelper.EnterPriseDBName);
            var lstNotificationDTO = obj.GetNotificationByIDsNormal(Ids, CurrentCulture);

            if (lstNotificationDTO != null && lstNotificationDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("ID");
                row.CreateCell(1).SetCellValue("ScheduleName");
                row.CreateCell(2).SetCellValue("TemplateName");
                row.CreateCell(3).SetCellValue("ReportName");
                row.CreateCell(4).SetCellValue("EmailAddress");
                row.CreateCell(5).SetCellValue("EmailSubject");
                row.CreateCell(6).SetCellValue("IsActive");
                row.CreateCell(7).SetCellValue("NextRunDate");
                row.CreateCell(8).SetCellValue("CreatedOn");
                row.CreateCell(9).SetCellValue("UpdatedOn");
                row.CreateCell(10).SetCellValue("LastUpdatedBy");
                row.CreateCell(11).SetCellValue("CreatedBy");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (NotificationDTO item in lstNotificationDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.ScheduleName);
                    DRow.CreateCell(2).SetCellValue(item.TemplateName);
                    DRow.CreateCell(3).SetCellValue(item.ReportName);
                    DRow.CreateCell(4).SetCellValue(item.EmailAddress);
                    DRow.CreateCell(5).SetCellValue(item.EmailSubject);
                    DRow.CreateCell(6).SetCellValue(item.IsActive == true ? "Yes" : "No");
                    DRow.CreateCell(7).SetCellValue(Convert.ToString(item.NextRunDate));
                    DRow.CreateCell(8).SetCellValue(Convert.ToString(item.Created));
                    DRow.CreateCell(9).SetCellValue(Convert.ToString(item.Updated));
                    DRow.CreateCell(10).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(11).SetCellValue(item.CreatedByName);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string TechnicianListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");

            string[] arrid = ids.Split(',');
            TechnicialMasterDAL obj = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            List<TechnicianMasterDTO> lstTechnicianMasterDTO = new List<TechnicianMasterDTO>();
            lstTechnicianMasterDTO = obj.GetTechnicianByIDsPlain(ids, SessionHelper.RoomID, SessionHelper.CompanyID);
            //IEnumerable<TechnicianMasterDTO> DataFromDB = obj.GetTechnicianByRoomIDNormal(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString);
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstTechnicianMasterDTO = (from c in DataFromDB
            //                              where arrid.Contains(c.ID.ToString())
            //                              select new TechnicianMasterDTO
            //                              {
            //                                  ID = c.ID,
            //                                  Technician = c.Technician,
            //                                  TechnicianCode = c.TechnicianCode.Replace(" ", ""),
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
            if (lstTechnicianMasterDTO != null && lstTechnicianMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("TECHNICIAN");
                row.CreateCell(2).SetCellValue("* TECHNICIANCODE");
                row.CreateCell(3).SetCellValue("UDF1");
                row.CreateCell(4).SetCellValue("UDF2");
                row.CreateCell(5).SetCellValue("UDF3");
                row.CreateCell(6).SetCellValue("UDF4");
                row.CreateCell(7).SetCellValue("UDF5");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (TechnicianMasterDTO item in lstTechnicianMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID.ToString());
                    DRow.CreateCell(1).SetCellValue(item.Technician);
                    DRow.CreateCell(2).SetCellValue(item.TechnicianCode);
                    DRow.CreateCell(3).SetCellValue(item.UDF1);
                    DRow.CreateCell(4).SetCellValue(item.UDF2);
                    DRow.CreateCell(5).SetCellValue(item.UDF3);
                    DRow.CreateCell(6).SetCellValue(item.UDF4);
                    DRow.CreateCell(7).SetCellValue(item.UDF5);


                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string UnitMasterListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            UnitMasterDAL obj = new UnitMasterDAL(SessionHelper.EnterPriseDBName);

            List<UnitMasterDTO> lstUnitMasterDTO = new List<UnitMasterDTO>();
            lstUnitMasterDTO = obj.GetUnitByIDsPlain(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();


            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstUnitMasterDTO = (from u in DataFromDB
            //                        where arrid.Contains(u.ID.ToString())
            //                        select new UnitMasterDTO
            //                        {
            //                            Unit = u.Unit,
            //                            Created = u.Created,
            //                            CreatedBy = u.CreatedBy,
            //                            ID = u.ID,
            //                            LastUpdatedBy = u.LastUpdatedBy,
            //                            Room = u.Room,
            //                            Updated = u.Updated,
            //                            Description = u.Description,
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

            if (lstUnitMasterDTO != null && lstUnitMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* Unit");
                row.CreateCell(2).SetCellValue("Description");
                row.CreateCell(3).SetCellValue("UDF1");
                row.CreateCell(4).SetCellValue("UDF2");
                row.CreateCell(5).SetCellValue("UDF3");
                row.CreateCell(6).SetCellValue("UDF4");
                row.CreateCell(7).SetCellValue("UDF5");


                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (UnitMasterDTO item in lstUnitMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.Unit);
                    DRow.CreateCell(2).SetCellValue(item.Description ?? string.Empty);
                    DRow.CreateCell(3).SetCellValue(item.UDF1);
                    DRow.CreateCell(4).SetCellValue(item.UDF2);
                    DRow.CreateCell(5).SetCellValue(item.UDF3);
                    DRow.CreateCell(6).SetCellValue(item.UDF4);
                    DRow.CreateCell(7).SetCellValue(item.UDF5);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string LocationMasterListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            LocationMasterDAL obj = new LocationMasterDAL(SessionHelper.EnterPriseDBName);


            List<LocationMasterDTO> lstLocationMasterDTO = new List<LocationMasterDTO>();
            lstLocationMasterDTO = obj.GetLocationByIDsPlain(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();

            if (lstLocationMasterDTO != null && lstLocationMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* LOCATION");
                row.CreateCell(2).SetCellValue("UDF1");
                row.CreateCell(3).SetCellValue("UDF2");
                row.CreateCell(4).SetCellValue("UDF3");
                row.CreateCell(5).SetCellValue("UDF4");
                row.CreateCell(6).SetCellValue("UDF5");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (LocationMasterDTO item in lstLocationMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID.ToString());
                    DRow.CreateCell(1).SetCellValue(item.Location);
                    DRow.CreateCell(2).SetCellValue(item.UDF1);
                    DRow.CreateCell(3).SetCellValue(item.UDF2);
                    DRow.CreateCell(4).SetCellValue(item.UDF3);
                    DRow.CreateCell(5).SetCellValue(item.UDF4);
                    DRow.CreateCell(6).SetCellValue(item.UDF5);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string ToolCategoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            ToolCategoryMasterDAL obj = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<ToolCategoryMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            List<ToolCategoryMasterDTO> lstToolCategoryMasterDTO = new List<ToolCategoryMasterDTO>();
            lstToolCategoryMasterDTO = obj.GetToolCategoryByIDsNormal(ids, SessionHelper.RoomID, SessionHelper.CompanyID);
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    lstToolCategoryMasterDTO = (from c in DataFromDB
            //                                where arrid.Contains(c.ID.ToString())
            //                                select new ToolCategoryMasterDTO
            //                                {
            //                                    ID = c.ID,
            //                                    ToolCategory = c.ToolCategory,
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

            if (lstToolCategoryMasterDTO != null && lstToolCategoryMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);

                row.CreateCell(0).SetCellValue("Id");
                row.CreateCell(1).SetCellValue("TOOLCATEGORY");
                row.CreateCell(2).SetCellValue("UDF2");
                row.CreateCell(3).SetCellValue("Room");
                row.CreateCell(4).SetCellValue("Created On");
                row.CreateCell(5).SetCellValue("Updated On");
                row.CreateCell(6).SetCellValue("Updated By");
                row.CreateCell(7).SetCellValue("Created By");
                row.CreateCell(8).SetCellValue("AddedFrom");
                row.CreateCell(9).SetCellValue("EditFrom");
                row.CreateCell(10).SetCellValue("ReceivedOnDate");
                row.CreateCell(11).SetCellValue("ReceivedOnWebDate");
                row.CreateCell(12).SetCellValue("UDF3");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ToolCategoryMasterDTO item in lstToolCategoryMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.ToolCategory);
                    DRow.CreateCell(2).SetCellValue(item.UDF2);
                    DRow.CreateCell(3).SetCellValue(item.Room.ToString());
                    DRow.CreateCell(4).SetCellValue(item.Created.ToString());
                    DRow.CreateCell(5).SetCellValue(item.Updated.ToString());
                    DRow.CreateCell(6).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(7).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(8).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(9).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(10).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(11).SetCellValue(item.ReceivedOnDateWeb);
                    DRow.CreateCell(12).SetCellValue(item.UDF3);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }

        public string AssetCategoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
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
                                                 AssetCategory = c.AssetCategory,
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

            if (lstAssetCategoryMasterDTO != null && lstAssetCategoryMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* AssetCATEGORY");
                row.CreateCell(2).SetCellValue("UDF1");
                row.CreateCell(3).SetCellValue("UDF2");
                row.CreateCell(4).SetCellValue("UDF3");
                row.CreateCell(5).SetCellValue("UDF4");
                row.CreateCell(6).SetCellValue("UDF5");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (AssetCategoryMasterDTO item in lstAssetCategoryMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID.ToString());
                    DRow.CreateCell(1).SetCellValue(item.AssetCategory);
                    DRow.CreateCell(2).SetCellValue(item.UDF1);
                    DRow.CreateCell(3).SetCellValue(item.UDF2);
                    DRow.CreateCell(4).SetCellValue(item.UDF3);
                    DRow.CreateCell(5).SetCellValue(item.UDF4);
                    DRow.CreateCell(6).SetCellValue(item.UDF5);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }


        public string ManufacturerMasterListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            ManufacturerMasterDAL obj = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            List<ManufacturerMasterDTO> DataFromDB = obj.GetManufacturerByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).OrderBy(SortNameString).ToList();
            List<ManufacturerMasterDTO> lstManufacturerMasterDTO = new List<ManufacturerMasterDTO>();
            if (!string.IsNullOrEmpty(ids))
            {
                lstManufacturerMasterDTO = (from c in DataFromDB
                                            where arrid.Contains(c.ID.ToString())
                                            select new ManufacturerMasterDTO
                                            {
                                                ID = c.ID,
                                                Manufacturer = c.Manufacturer,
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

            if (lstManufacturerMasterDTO != null && lstManufacturerMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* MANUFACTURER");

                row.CreateCell(2).SetCellValue("UDF1");
                row.CreateCell(3).SetCellValue("UDF2");
                row.CreateCell(4).SetCellValue("UDF3");
                row.CreateCell(5).SetCellValue("UDF4");
                row.CreateCell(6).SetCellValue("UDF5");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ManufacturerMasterDTO item in lstManufacturerMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID.ToString());
                    DRow.CreateCell(1).SetCellValue(item.Manufacturer);

                    DRow.CreateCell(2).SetCellValue(item.UDF1);
                    DRow.CreateCell(3).SetCellValue(item.UDF2);
                    DRow.CreateCell(4).SetCellValue(item.UDF3);
                    DRow.CreateCell(5).SetCellValue(item.UDF4);
                    DRow.CreateCell(6).SetCellValue(item.UDF5);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string MeasurementTermListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            MeasurementTermDAL obj = new MeasurementTermDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<MeasurementTermMasterDTO> DataFromDB = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            List<MeasurementTermMasterDTO> lstMeasurementTermMasterDTO = new List<MeasurementTermMasterDTO>();
            if (!string.IsNullOrEmpty(ids))
            {
                lstMeasurementTermMasterDTO = (from c in DataFromDB
                                               where arrid.Contains(c.ID.ToString())
                                               select new MeasurementTermMasterDTO
                                               {
                                                   ID = c.ID,
                                                   MeasurementTerm = c.MeasurementTerm,
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




            if (lstMeasurementTermMasterDTO != null && lstMeasurementTermMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* MEASUREMENTTERM");

                row.CreateCell(2).SetCellValue("UDF1");
                row.CreateCell(3).SetCellValue("UDF2");
                row.CreateCell(4).SetCellValue("UDF3");
                row.CreateCell(5).SetCellValue("UDF4");
                row.CreateCell(6).SetCellValue("UDF5");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (MeasurementTermMasterDTO item in lstMeasurementTermMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID.ToString());
                    DRow.CreateCell(1).SetCellValue(item.MeasurementTerm);

                    DRow.CreateCell(2).SetCellValue(item.UDF1);
                    DRow.CreateCell(3).SetCellValue(item.UDF2);
                    DRow.CreateCell(4).SetCellValue(item.UDF3);
                    DRow.CreateCell(5).SetCellValue(item.UDF4);
                    DRow.CreateCell(6).SetCellValue(item.UDF5);


                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string ToolListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
            lstToolMasterDTO = obj.GetToolByIDsFull(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();


            if (lstToolMasterDTO != null && lstToolMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* ToolName");
                row.CreateCell(2).SetCellValue("* Serial");
                row.CreateCell(3).SetCellValue("Description");
                row.CreateCell(4).SetCellValue("Cost");
                row.CreateCell(5).SetCellValue("* Quantity");
                row.CreateCell(6).SetCellValue("ToolCategory");
                row.CreateCell(7).SetCellValue("Location");
                row.CreateCell(8).SetCellValue("CheckedOutQTY");
                row.CreateCell(9).SetCellValue("CheckedOutMQTY");
                row.CreateCell(10).SetCellValue("UDF1");
                row.CreateCell(11).SetCellValue("UDF2");
                row.CreateCell(12).SetCellValue("UDF3");
                row.CreateCell(13).SetCellValue("UDF4");
                row.CreateCell(14).SetCellValue("UDF5");
                row.CreateCell(15).SetCellValue("IsGroupOfItems");
                row.CreateCell(16).SetCellValue("Technician");
                row.CreateCell(17).SetCellValue("CheckOutQuantity");
                row.CreateCell(18).SetCellValue("CheckInQuantity");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ToolMasterDTO item in lstToolMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.ToolName ?? string.Empty);
                    DRow.CreateCell(2).SetCellValue(item.Serial);
                    DRow.CreateCell(3).SetCellValue(item.Description ?? string.Empty);
                    DRow.CreateCell(4).SetCellValue(item.Cost ?? 0);
                    DRow.CreateCell(5).SetCellValue(item.Quantity);
                    DRow.CreateCell(6).SetCellValue(item.ToolCategory ?? string.Empty);
                    DRow.CreateCell(7).SetCellValue(item.Location ?? string.Empty);
                    DRow.CreateCell(8).SetCellValue(item.CheckedOutQTY ?? 0);
                    DRow.CreateCell(9).SetCellValue(item.CheckedOutMQTY ?? 0);
                    DRow.CreateCell(10).SetCellValue(item.UDF1);
                    DRow.CreateCell(11).SetCellValue(item.UDF2);
                    DRow.CreateCell(12).SetCellValue(item.UDF3);
                    DRow.CreateCell(13).SetCellValue(item.UDF4);
                    DRow.CreateCell(14).SetCellValue(item.UDF5);
                    if (item.IsGroupOfItems == 1)
                        DRow.CreateCell(15).SetCellValue("Yes");
                    else
                        DRow.CreateCell(15).SetCellValue("No");
                    DRow.CreateCell(16).SetCellValue(item.Technician ?? string.Empty);
                    DRow.CreateCell(17).SetCellValue(item.CheckedOutQuantity.GetValueOrDefault(0));
                    DRow.CreateCell(18).SetCellValue(item.CheckedInQuantity.GetValueOrDefault(0));
                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string ToolMasterListCSVNew(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
            lstToolMasterDTO = obj.GetToolByIDsNormal(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();



            if (lstToolMasterDTO != null && lstToolMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* ToolName");
                row.CreateCell(2).SetCellValue("* Serial");
                row.CreateCell(3).SetCellValue("Description");
                row.CreateCell(4).SetCellValue("Cost");
                row.CreateCell(5).SetCellValue("* Quantity");
                row.CreateCell(6).SetCellValue("ToolCategory");

                row.CreateCell(7).SetCellValue("Location");
                row.CreateCell(8).SetCellValue("CheckedOutQTY");
                row.CreateCell(9).SetCellValue("CheckedOutMQTY");

                row.CreateCell(10).SetCellValue("UDF1");
                row.CreateCell(11).SetCellValue("UDF2");
                row.CreateCell(12).SetCellValue("UDF3");
                row.CreateCell(13).SetCellValue("UDF4");
                row.CreateCell(14).SetCellValue("UDF5");
                row.CreateCell(15).SetCellValue("IsGroupOfItems");
                row.CreateCell(16).SetCellValue("Technician");



                row.CreateCell(17).SetCellValue("CheckoutUDF1");
                row.CreateCell(18).SetCellValue("CheckoutUDF2");
                row.CreateCell(19).SetCellValue("CheckoutUDF3");
                row.CreateCell(20).SetCellValue("CheckoutUDF4");
                row.CreateCell(21).SetCellValue("CheckoutUDF5");

                row.CreateCell(22).SetCellValue("ImagePath");
                row.CreateCell(23).SetCellValue("ToolImageExternalURL");
                row.CreateCell(24).SetCellValue("ToolTypeTracking");
                row.CreateCell(25).SetCellValue("SerialNumberTracking");

                int RowId = 1;
                foreach (ToolMasterDTO rec in lstToolMasterDTO)
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
                                    IRow DRow = sheet1.CreateRow(RowId);
                                    DRow.CreateCell(0).SetCellValue(rec.ID);
                                    DRow.CreateCell(1).SetCellValue(rec.ToolName);
                                    if (rec.SerialNumberTracking == true)
                                    {
                                        DRow.CreateCell(2).SetCellValue(ToolQty.SerialNumber);
                                    }
                                    else
                                    {
                                        DRow.CreateCell(2).SetCellValue(rec.Serial);
                                    }
                                    DRow.CreateCell(3).SetCellValue(rec.Description);
                                    DRow.CreateCell(4).SetCellValue(rec.Cost ?? 0);
                                    DRow.CreateCell(5).SetCellValue(ToolQty.Quantity ?? 0);
                                    DRow.CreateCell(6).SetCellValue(rec.ToolCategory);

                                    //DRow.CreateCell(0).SetCellValue(rec.Location);
                                    DRow.CreateCell(7).SetCellValue(l.ToolLocationName);
                                    DRow.CreateCell(8).SetCellValue(rec.CheckedOutQTY ?? 0);
                                    DRow.CreateCell(9).SetCellValue(rec.CheckedOutMQTY ?? 0);

                                    DRow.CreateCell(10).SetCellValue(rec.UDF1);
                                    DRow.CreateCell(11).SetCellValue(rec.UDF2);
                                    DRow.CreateCell(12).SetCellValue(rec.UDF3);
                                    DRow.CreateCell(13).SetCellValue(rec.UDF4);
                                    DRow.CreateCell(14).SetCellValue(rec.UDF5);
                                    if (rec.IsGroupOfItems == 1)
                                        DRow.CreateCell(15).SetCellValue("Yes");
                                    else
                                        DRow.CreateCell(15).SetCellValue("No");

                                    DRow.CreateCell(16).SetCellValue("");

                                    DRow.CreateCell(17).SetCellValue("");
                                    DRow.CreateCell(18).SetCellValue("");
                                    DRow.CreateCell(19).SetCellValue("");
                                    DRow.CreateCell(20).SetCellValue("");
                                    DRow.CreateCell(21).SetCellValue("");

                                    DRow.CreateCell(22).SetCellValue(rec.ImagePath);
                                    DRow.CreateCell(23).SetCellValue(rec.ToolImageExternalURL);
                                    DRow.CreateCell(24).SetCellValue(rec.ToolTypeTracking);
                                    if (rec.SerialNumberTracking == true)
                                        DRow.CreateCell(25).SetCellValue("Yes");
                                    else
                                        DRow.CreateCell(25).SetCellValue("No");
                                    RowId += 1;
                                }
                            }
                            else
                            {
                                IRow DRow = sheet1.CreateRow(RowId);
                                DRow.CreateCell(0).SetCellValue(rec.ID);
                                DRow.CreateCell(1).SetCellValue(rec.ToolName);
                                if (rec.SerialNumberTracking == true)
                                {
                                    DRow.CreateCell(2).SetCellValue("");
                                }
                                else
                                {
                                    DRow.CreateCell(2).SetCellValue(rec.Serial);
                                }
                                DRow.CreateCell(3).SetCellValue(rec.Description);
                                DRow.CreateCell(4).SetCellValue(rec.Cost ?? 0);
                                DRow.CreateCell(5).SetCellValue("0");
                                DRow.CreateCell(6).SetCellValue(rec.ToolCategory);

                                //DRow.CreateCell(0).SetCellValue(rec.Location);
                                DRow.CreateCell(7).SetCellValue(l.ToolLocationName);
                                DRow.CreateCell(8).SetCellValue(rec.CheckedOutQTY ?? 0);
                                DRow.CreateCell(9).SetCellValue(rec.CheckedOutMQTY ?? 0);

                                DRow.CreateCell(10).SetCellValue(rec.UDF1);
                                DRow.CreateCell(11).SetCellValue(rec.UDF2);
                                DRow.CreateCell(12).SetCellValue(rec.UDF3);
                                DRow.CreateCell(13).SetCellValue(rec.UDF4);
                                DRow.CreateCell(14).SetCellValue(rec.UDF5);
                                if (rec.IsGroupOfItems == 1)
                                    DRow.CreateCell(15).SetCellValue("Yes");
                                else
                                    DRow.CreateCell(15).SetCellValue("No");

                                DRow.CreateCell(16).SetCellValue("");

                                DRow.CreateCell(17).SetCellValue("");
                                DRow.CreateCell(18).SetCellValue("");
                                DRow.CreateCell(19).SetCellValue("");
                                DRow.CreateCell(20).SetCellValue("");
                                DRow.CreateCell(21).SetCellValue("");

                                DRow.CreateCell(22).SetCellValue(rec.ImagePath);
                                DRow.CreateCell(23).SetCellValue(rec.ToolImageExternalURL);
                                DRow.CreateCell(24).SetCellValue(rec.ToolTypeTracking);
                                if (rec.SerialNumberTracking == true)
                                    DRow.CreateCell(25).SetCellValue("Yes");
                                else
                                    DRow.CreateCell(25).SetCellValue("No");
                                RowId += 1;
                            }


                        }
                    }


                }
            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;

        }

        public string ToolCheckoutStatusListCSVNew(string Filepath, string ModuleName, string ids, string SortNameString, out bool isRecordAvail)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            isRecordAvail = false;
            //string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.csv.ToString());
            //string filePath = Filepath + filename;
            string[] arrid = ids.Split(',');
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
            lstToolMasterDTO = obj.GetToolCheckoutStatusExportData(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();




            if (lstToolMasterDTO != null && lstToolMasterDTO.Count > 0)
            {
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(0).SetCellValue("* ToolName");
                row.CreateCell(0).SetCellValue("* Serial");
                row.CreateCell(0).SetCellValue("Description");
                row.CreateCell(0).SetCellValue("Cost");
                row.CreateCell(0).SetCellValue("* Quantity");
                row.CreateCell(0).SetCellValue("ToolCategory");

                row.CreateCell(0).SetCellValue("Location");
                row.CreateCell(0).SetCellValue("CheckedOutQTY");
                row.CreateCell(0).SetCellValue("CheckedOutMQTY");

                row.CreateCell(0).SetCellValue("UDF1");
                row.CreateCell(0).SetCellValue("UDF2");
                row.CreateCell(0).SetCellValue("UDF3");
                row.CreateCell(0).SetCellValue("UDF4");
                row.CreateCell(0).SetCellValue("UDF5");
                row.CreateCell(0).SetCellValue("IsGroupOfItems");
                row.CreateCell(0).SetCellValue("Technician");
                // row.CreateCell(0).SetCellValue("CheckOutQuantity");
                // row.CreateCell(0).SetCellValue("CheckInQuantity");

                row.CreateCell(0).SetCellValue("CheckoutUDF1");
                row.CreateCell(0).SetCellValue("CheckoutUDF2");
                row.CreateCell(0).SetCellValue("CheckoutUDF3");
                row.CreateCell(0).SetCellValue("CheckoutUDF4");
                row.CreateCell(0).SetCellValue("CheckoutUDF5");

                row.CreateCell(0).SetCellValue("ImagePath");
                row.CreateCell(0).SetCellValue("ToolImageExternalURL");
                row.CreateCell(0).SetCellValue("SerialNumberTracking");

                int RowId = 1;
                if (lstToolMasterDTO != null && lstToolMasterDTO.Count > 0)
                {
                    isRecordAvail = true;
                    foreach (ToolMasterDTO rec in lstToolMasterDTO)
                    {

                        if (rec.SerialNumberTracking == true)
                        {
                            LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                            LocationMasterDTO objLocationMasterDTO = objLocationMasterDAL.GetLocationBySerialPlain(rec.Serial, rec.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                            {
                                IRow DRow = sheet1.CreateRow(RowId);
                                DRow.CreateCell(0).SetCellValue(rec.ID);
                                DRow.CreateCell(0).SetCellValue(rec.ToolName);
                                DRow.CreateCell(0).SetCellValue(rec.Serial);
                                DRow.CreateCell(0).SetCellValue(rec.Description);
                                DRow.CreateCell(0).SetCellValue(rec.Cost ?? 0);
                                DRow.CreateCell(0).SetCellValue(rec.Quantity);
                                DRow.CreateCell(0).SetCellValue(rec.ToolCategory);

                                //csw.WriteField(rec.Location);
                                DRow.CreateCell(0).SetCellValue(objLocationMasterDTO.Location);
                                DRow.CreateCell(0).SetCellValue(rec.CheckedOutQTY ?? 0);
                                DRow.CreateCell(0).SetCellValue(rec.CheckedOutMQTY ?? 0);

                                DRow.CreateCell(0).SetCellValue(rec.UDF1);
                                DRow.CreateCell(0).SetCellValue(rec.UDF2);
                                DRow.CreateCell(0).SetCellValue(rec.UDF3);
                                DRow.CreateCell(0).SetCellValue(rec.UDF4);
                                DRow.CreateCell(0).SetCellValue(rec.UDF5);
                                if (rec.IsGroupOfItems == 1)
                                    DRow.CreateCell(0).SetCellValue("Yes");
                                else
                                    DRow.CreateCell(0).SetCellValue("No");

                                DRow.CreateCell(0).SetCellValue("");//(rec.Technician);
                                                                    //csw.WriteField(rec.CheckedOutQuantity);
                                                                    //csw.WriteField(rec.CheckedInQuantity);

                                DRow.CreateCell(0).SetCellValue("");
                                DRow.CreateCell(0).SetCellValue("");
                                DRow.CreateCell(0).SetCellValue("");
                                DRow.CreateCell(0).SetCellValue("");
                                DRow.CreateCell(0).SetCellValue("");

                                DRow.CreateCell(0).SetCellValue(rec.ImagePath);
                                DRow.CreateCell(0).SetCellValue(rec.ToolImageExternalURL);
                                if (rec.SerialNumberTracking == true)
                                    DRow.CreateCell(0).SetCellValue("Yes");
                                else
                                    DRow.CreateCell(0).SetCellValue("No");
                                RowId += 1;
                            }
                        }
                        else
                        {
                            LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                            LocationMasterDTO objLocationMasterDTO = objLocationMasterDAL.GetLocationUsingGeneralCO(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, rec.GUID, rec.ToolCheckoutGUID ?? Guid.Empty);

                            {
                                IRow DRow = sheet1.CreateRow(RowId);
                                DRow.CreateCell(0).SetCellValue(rec.ID);
                                DRow.CreateCell(0).SetCellValue(rec.ToolName);
                                DRow.CreateCell(0).SetCellValue(rec.Serial);
                                DRow.CreateCell(0).SetCellValue(rec.Description);
                                DRow.CreateCell(0).SetCellValue(rec.Cost ?? 0);
                                DRow.CreateCell(0).SetCellValue(rec.Quantity);
                                DRow.CreateCell(0).SetCellValue(rec.ToolCategory);

                                //csw.WriteField(rec.Location);
                                DRow.CreateCell(0).SetCellValue(objLocationMasterDTO.Location);
                                DRow.CreateCell(0).SetCellValue(rec.CheckedOutQTY ?? 0);
                                DRow.CreateCell(0).SetCellValue(rec.CheckedOutMQTY ?? 0);

                                DRow.CreateCell(0).SetCellValue(rec.UDF1);
                                DRow.CreateCell(0).SetCellValue(rec.UDF2);
                                DRow.CreateCell(0).SetCellValue(rec.UDF3);
                                DRow.CreateCell(0).SetCellValue(rec.UDF4);
                                DRow.CreateCell(0).SetCellValue(rec.UDF5);
                                if (rec.IsGroupOfItems == 1)
                                    DRow.CreateCell(0).SetCellValue("Yes");
                                else
                                    DRow.CreateCell(0).SetCellValue("No");

                                DRow.CreateCell(0).SetCellValue("");//(rec.Technician);
                                                                    //csw.WriteField(rec.CheckedOutQuantity);
                                                                    //csw.WriteField(rec.CheckedInQuantity);

                                DRow.CreateCell(0).SetCellValue("");
                                DRow.CreateCell(0).SetCellValue("");
                                DRow.CreateCell(0).SetCellValue("");
                                DRow.CreateCell(0).SetCellValue("");
                                DRow.CreateCell(0).SetCellValue("");

                                DRow.CreateCell(0).SetCellValue(rec.ImagePath);
                                DRow.CreateCell(0).SetCellValue(rec.ToolImageExternalURL);
                                if (rec.SerialNumberTracking == true)
                                    DRow.CreateCell(0).SetCellValue("Yes");
                                else
                                    DRow.CreateCell(0).SetCellValue("No");
                                RowId += 1;
                            }
                        }

                    }
                }
            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;

        }


        public string ToolHistoryListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolMasterDTO> lstToolMasterDTO = new List<ToolMasterDTO>();
            lstToolMasterDTO = obj.GetToolByIDsFull(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();

            if (lstToolMasterDTO != null && lstToolMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* ToolName");
                row.CreateCell(2).SetCellValue("* Serial");
                row.CreateCell(3).SetCellValue("Description");
                row.CreateCell(4).SetCellValue("Cost");
                row.CreateCell(5).SetCellValue("* Quantity");
                row.CreateCell(6).SetCellValue("ToolCategory");
                row.CreateCell(7).SetCellValue("Location");
                row.CreateCell(8).SetCellValue("CheckedOutQTY");
                row.CreateCell(9).SetCellValue("CheckedOutMQTY");
                row.CreateCell(10).SetCellValue("UDF1");
                row.CreateCell(11).SetCellValue("UDF2");
                row.CreateCell(12).SetCellValue("UDF3");
                row.CreateCell(13).SetCellValue("UDF4");
                row.CreateCell(14).SetCellValue("UDF5");
                row.CreateCell(15).SetCellValue("IsGroupOfItems");
                row.CreateCell(16).SetCellValue("Technician");
                row.CreateCell(17).SetCellValue("CheckOutQuantity");
                row.CreateCell(18).SetCellValue("CheckInQuantity");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ToolMasterDTO item in lstToolMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.ToolName ?? string.Empty);
                    DRow.CreateCell(2).SetCellValue(item.Serial);
                    DRow.CreateCell(3).SetCellValue(item.Description ?? string.Empty);
                    DRow.CreateCell(4).SetCellValue(item.Cost ?? 0);
                    DRow.CreateCell(5).SetCellValue(item.Quantity);
                    DRow.CreateCell(6).SetCellValue(item.ToolCategory ?? string.Empty);

                    DRow.CreateCell(7).SetCellValue(item.Location ?? string.Empty);
                    DRow.CreateCell(8).SetCellValue(item.CheckedOutQTY ?? 0);
                    DRow.CreateCell(9).SetCellValue(item.CheckedOutMQTY ?? 0);
                    DRow.CreateCell(10).SetCellValue(item.UDF1);
                    DRow.CreateCell(11).SetCellValue(item.UDF2);
                    DRow.CreateCell(12).SetCellValue(item.UDF3);
                    DRow.CreateCell(13).SetCellValue(item.UDF4);
                    DRow.CreateCell(14).SetCellValue(item.UDF5);
                    if (item.IsGroupOfItems == 1)
                        DRow.CreateCell(15).SetCellValue("Yes");
                    else
                        DRow.CreateCell(15).SetCellValue("No");
                    DRow.CreateCell(16).SetCellValue(item.Technician ?? string.Empty);
                    DRow.CreateCell(17).SetCellValue(item.CheckedOutQuantity.GetValueOrDefault(0));
                    DRow.CreateCell(18).SetCellValue(item.CheckedInQuantity.GetValueOrDefault(0));
                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string AssetMasterListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            AssetMasterDAL obj = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID);
            IEnumerable<AssetMasterDTO> DataFromDB = obj.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(SortNameString);
            List<AssetMasterDTO> lstAssetMasterDTO = new List<AssetMasterDTO>();
            if (!string.IsNullOrEmpty(ids))
            {
                lstAssetMasterDTO = (from u in DataFromDB
                                     where arrid.Contains(u.ID.ToString())
                                     select new AssetMasterDTO
                                     {
                                         ID = u.ID,
                                         AssetName = u.AssetName,
                                         Description = u.Description,
                                         Make = u.Make,
                                         Model = u.Model,
                                         Serial = u.Serial,
                                         AssetCategoryID = u.AssetCategoryID,
                                         AssetCategory = u.AssetCategory,
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
                                         AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Assets")
                                     }).ToList();
            }




            if (lstAssetMasterDTO != null && lstAssetMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* AssetName");
                row.CreateCell(2).SetCellValue("Description");
                row.CreateCell(3).SetCellValue("Make");
                row.CreateCell(4).SetCellValue("Model");
                row.CreateCell(5).SetCellValue("Serial");
                row.CreateCell(6).SetCellValue("AssetCategory");
                row.CreateCell(7).SetCellValue("PurchaseDate");
                row.CreateCell(8).SetCellValue("PurchasePrice");
                row.CreateCell(9).SetCellValue("DepreciatedValue");
                row.CreateCell(10).SetCellValue("SuggestedMaintenanceDate");

                row.CreateCell(11).SetCellValue("UDF1");
                row.CreateCell(12).SetCellValue("UDF2");
                row.CreateCell(13).SetCellValue("UDF3");
                row.CreateCell(14).SetCellValue("UDF4");
                row.CreateCell(15).SetCellValue("UDF5");
                row.CreateCell(16).SetCellValue("UDF6");
                row.CreateCell(17).SetCellValue("UDF7");
                row.CreateCell(18).SetCellValue("UDF8");
                row.CreateCell(19).SetCellValue("UDF9");
                row.CreateCell(20).SetCellValue("UDF10");

                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (AssetMasterDTO item in lstAssetMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.AssetName);
                    DRow.CreateCell(2).SetCellValue(item.Description);
                    DRow.CreateCell(3).SetCellValue(item.Make);
                    DRow.CreateCell(4).SetCellValue(item.Model);
                    DRow.CreateCell(5).SetCellValue(item.Serial);
                    DRow.CreateCell(6).SetCellValue(item.AssetCategory);
                    DateTime dttopass = item.PurchaseDate == null ? DateTime.MinValue : item.PurchaseDate.Value;
                    DRow.CreateCell(7).SetCellValue(dttopass);
                    DRow.CreateCell(8).SetCellValue(Convert.ToDouble(item.PurchasePrice));//== null ? 0 : Convert.ToDouble(item.PurchasePrice));
                    DRow.CreateCell(9).SetCellValue(item.DepreciatedValue ?? 0);
                    DRow.CreateCell(10).SetCellValue(item.SuggestedMaintenanceDate == null ? DateTime.MinValue : item.SuggestedMaintenanceDate.Value);

                    DRow.CreateCell(11).SetCellValue(item.UDF1);
                    DRow.CreateCell(12).SetCellValue(item.UDF2);
                    DRow.CreateCell(13).SetCellValue(item.UDF3);
                    DRow.CreateCell(14).SetCellValue(item.UDF4);
                    DRow.CreateCell(15).SetCellValue(item.UDF5);

                    DRow.CreateCell(16).SetCellValue(item.UDF6);
                    DRow.CreateCell(17).SetCellValue(item.UDF7);
                    DRow.CreateCell(18).SetCellValue(item.UDF8);
                    DRow.CreateCell(19).SetCellValue(item.UDF9);
                    DRow.CreateCell(20).SetCellValue(item.UDF10);


                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string ItemMasterListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();
            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            //string[] arrid = ids.Split(',');
            string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> lstIds = arrid.Select(t => Guid.Parse(t)).ToList();
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID);
            //IEnumerable<ItemMasterDTO> DataFromDB = obj.GetAllItemsWithJoins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null).OrderBy(SortNameString);
            SortNameString = (SortNameString ?? string.Empty).Replace("OrderCost", "ItemNumber");
            IEnumerable<ItemMasterDTO> DataFromDB = obj.GetItemsByArray(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString);//.Where(i => i.Room == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID).OrderBy(SortNameString);
            List<ItemMasterDTO> lstItemMasterDTO = new List<ItemMasterDTO>();

            if (!string.IsNullOrEmpty(ids))
            {
                lstItemMasterDTO = (from u in DataFromDB
                                    where arrid.Contains(u.GUID.ToString())
                                    select new ItemMasterDTO
                                    {
                                        ID = u.ID,
                                        ItemNumber = u.ItemNumber,
                                        ManufacturerID = u.ManufacturerID,
                                        ManufacturerNumber = u.ManufacturerNumber,
                                        ManufacturerName = u.ManufacturerName,
                                        SupplierID = u.SupplierID,
                                        SupplierPartNo = u.SupplierPartNo,
                                        SupplierName = u.SupplierName,
                                        UPC = u.UPC,
                                        UNSPSC = u.UNSPSC,
                                        Description = u.Description,
                                        LongDescription = u.LongDescription,
                                        CategoryID = u.CategoryID,
                                        GLAccountID = u.GLAccountID,
                                        UOMID = u.UOMID,

                                        PricePerTerm = u.PricePerTerm,
                                        CostUOMID = u.CostUOMID,
                                        CostUOMName = u.CostUOMName,
                                        DefaultReorderQuantity = u.DefaultReorderQuantity,
                                        DefaultPullQuantity = u.DefaultPullQuantity,
                                        Cost = u.Cost,
                                        Markup = u.Markup,
                                        SellPrice = u.SellPrice,
                                        ExtendedCost = u.ExtendedCost,
                                        AverageCost = u.AverageCost,
                                        LeadTimeInDays = u.LeadTimeInDays,
                                        Link1 = u.Link1,
                                        Link2 = u.Link2,
                                        Trend = u.Trend,
                                        Taxable = u.Taxable,
                                        Consignment = u.Consignment,
                                        StagedQuantity = u.StagedQuantity,
                                        InTransitquantity = u.InTransitquantity,
                                        OnOrderQuantity = u.OnOrderQuantity,
                                        OnReturnQuantity = u.OnReturnQuantity,
                                        OnTransferQuantity = u.OnTransferQuantity,
                                        SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                        SuggestedTransferQuantity = u.SuggestedTransferQuantity,
                                        RequisitionedQuantity = u.RequisitionedQuantity,
                                        PackingQuantity = u.PackingQuantity,
                                        AverageUsage = u.AverageUsage,
                                        Turns = u.Turns,
                                        OnHandQuantity = u.OnHandQuantity,
                                        CriticalQuantity = u.CriticalQuantity,
                                        MinimumQuantity = u.MinimumQuantity,
                                        MaximumQuantity = u.MaximumQuantity,
                                        WeightPerPiece = u.WeightPerPiece,
                                        ItemUniqueNumber = u.ItemUniqueNumber,
                                        IsPurchase = u.IsPurchase,
                                        IsTransfer = u.IsTransfer,
                                        DefaultLocation = u.DefaultLocation,
                                        DefaultLocationName = u.DefaultLocationName,
                                        InventoryClassification = u.InventoryClassification,
                                        InventoryClassificationName = u.InventoryClassificationName,
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
                                        IsItemLevelMinMaxQtyRequired = (u.IsItemLevelMinMaxQtyRequired.HasValue ? u.IsItemLevelMinMaxQtyRequired : false),
                                        IsEnforceDefaultReorderQuantity = (u.IsEnforceDefaultReorderQuantity.HasValue ? u.IsEnforceDefaultReorderQuantity : false),
                                        IsBuildBreak = (u.IsBuildBreak.HasValue ? u.IsBuildBreak : false),
                                        BondedInventory = u.BondedInventory,
                                        ItemLocations = objLocationDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, u.GUID, null, "ID ASC").ToList(),
                                        AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Item Master"),
                                        IsPackslipMandatoryAtReceive = u.IsPackslipMandatoryAtReceive,
                                        BlanketOrderNumber = u.BlanketOrderNumber,
                                        ItemImageExternalURL = u.ItemImageExternalURL,
                                        ItemDocExternalURL = u.ItemDocExternalURL,
                                        TrendingSetting = u.TrendingSetting,
                                        PullQtyScanOverride = u.PullQtyScanOverride,
                                        IsAutoInventoryClassification = u.IsAutoInventoryClassification,
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
                                        IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                        eLabelKey = u.eLabelKey,
                                        EnrichedProductData = u.EnrichedProductData,
                                        EnhancedDescription = u.EnhancedDescription,
                                        POItemLineNumber = u.POItemLineNumber
                                    }).ToList();
            }




            if (lstItemMasterDTO != null && lstItemMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* ItemNumber");
                row.CreateCell(2).SetCellValue("Manufacturer");
                row.CreateCell(3).SetCellValue("ManufacturerNumber");
                row.CreateCell(4).SetCellValue("* SupplierName");
                row.CreateCell(5).SetCellValue("* SupplierPartNo");
                row.CreateCell(6).SetCellValue("BlanketOrderNumber");
                row.CreateCell(7).SetCellValue("UPC");
                row.CreateCell(8).SetCellValue("UNSPSC");
                row.CreateCell(9).SetCellValue("Description");
                row.CreateCell(10).SetCellValue("LongDescription");
                row.CreateCell(11).SetCellValue("CategoryName");
                row.CreateCell(12).SetCellValue("GLAccount");
                row.CreateCell(13).SetCellValue("* UOM");
                row.CreateCell(14).SetCellValue("* CostUOM");
                row.CreateCell(15).SetCellValue("* DefaultReorderQuantity");
                row.CreateCell(16).SetCellValue("* DefaultPullQuantity");
                row.CreateCell(17).SetCellValue("Cost");
                row.CreateCell(18).SetCellValue("Markup");
                row.CreateCell(19).SetCellValue("SellPrice");
                row.CreateCell(20).SetCellValue("ExtendedCost");
                row.CreateCell(21).SetCellValue("AverageCost");
                row.CreateCell(22).SetCellValue("LeadTimeInDays");
                row.CreateCell(23).SetCellValue("Link1");
                row.CreateCell(24).SetCellValue("Link2");
                //row.CreateCell(25).SetCellValue("Trend");
                row.CreateCell(25).SetCellValue("Taxable");
                row.CreateCell(26).SetCellValue("Consignment");
                row.CreateCell(27).SetCellValue("StagedQuantity");
                row.CreateCell(28).SetCellValue("InTransitquantity");
                row.CreateCell(29).SetCellValue("OnOrderQuantity");
                row.CreateCell(30).SetCellValue("OnReturnQuantity");
                row.CreateCell(31).SetCellValue("OnTransferQuantity");
                row.CreateCell(32).SetCellValue("SuggestedOrderQuantity");
                row.CreateCell(33).SetCellValue("RequisitionedQuantity");
                //row.CreateCell(33).SetCellValue("PackingQuantity");
                row.CreateCell(34).SetCellValue("AverageUsage");
                row.CreateCell(35).SetCellValue("Turns");
                row.CreateCell(36).SetCellValue("OnHandQuantity");
                row.CreateCell(37).SetCellValue("CriticalQuantity");
                row.CreateCell(38).SetCellValue("MinimumQuantity");
                row.CreateCell(39).SetCellValue("MaximumQuantity");
                row.CreateCell(40).SetCellValue("WeightPerPiece");
                row.CreateCell(41).SetCellValue("ItemUniqueNumber");
                row.CreateCell(42).SetCellValue("IsTransfer");
                row.CreateCell(43).SetCellValue("IsPurchase");
                row.CreateCell(44).SetCellValue("* InventryLocation");
                row.CreateCell(45).SetCellValue("InventoryClassification");
                row.CreateCell(46).SetCellValue("SerialNumberTracking");
                row.CreateCell(47).SetCellValue("LotNumberTracking");
                row.CreateCell(48).SetCellValue("DateCodeTracking");
                row.CreateCell(49).SetCellValue("* ItemType");
                row.CreateCell(50).SetCellValue("ImagePath");
                row.CreateCell(51).SetCellValue("UDF1");
                row.CreateCell(52).SetCellValue("UDF2");
                row.CreateCell(53).SetCellValue("UDF3");
                row.CreateCell(54).SetCellValue("UDF4");
                row.CreateCell(55).SetCellValue("UDF5");
                row.CreateCell(56).SetCellValue("IsLotSerialExpiryCost");
                row.CreateCell(57).SetCellValue("* IsItemLevelMinMaxQtyRequired");

                row.CreateCell(58).SetCellValue("TrendingSetting");
                row.CreateCell(59).SetCellValue("EnforceDefaultPullQuantity");
                row.CreateCell(60).SetCellValue("EnforceDefaultReorderQuantity");
                row.CreateCell(61).SetCellValue("IsAutoInventoryClassification");
                row.CreateCell(62).SetCellValue("IsBuildBreak");


                row.CreateCell(63).SetCellValue("IsPackslipMandatoryAtReceive");
                row.CreateCell(64).SetCellValue("ItemImageExternalURL");
                row.CreateCell(65).SetCellValue("ItemDocExternalURL");
                row.CreateCell(66).SetCellValue("IsDeleted");
                row.CreateCell(67).SetCellValue("OutTransferQuantity");
                row.CreateCell(68).SetCellValue("OnOrderInTransitQuantity");
                row.CreateCell(69).SetCellValue("ItemLink2ExternalURL");
                row.CreateCell(70).SetCellValue("IsActive");
                row.CreateCell(71).SetCellValue("UDF6");
                row.CreateCell(72).SetCellValue("UDF7");
                row.CreateCell(73).SetCellValue("UDF8");
                row.CreateCell(74).SetCellValue("UDF9");
                row.CreateCell(75).SetCellValue("UDF10");
                row.CreateCell(76).SetCellValue("IsAllowOrderCostuom");
                row.CreateCell(77).SetCellValue("eLabelKey");
                row.CreateCell(78).SetCellValue("EnrichedProductData");
                row.CreateCell(79).SetCellValue("EnhancedDescription");
                row.CreateCell(80).SetCellValue("POItemLineNumber");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ItemMasterDTO item in lstItemMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.ItemNumber);
                    DRow.CreateCell(2).SetCellValue(item.ManufacturerName);
                    DRow.CreateCell(3).SetCellValue(item.ManufacturerNumber);
                    if (item.ItemType == 4)
                        DRow.CreateCell(4).SetCellValue("");
                    else
                        DRow.CreateCell(4).SetCellValue(item.SupplierName);
                    DRow.CreateCell(5).SetCellValue(item.SupplierPartNo);
                    DRow.CreateCell(6).SetCellValue(item.BlanketOrderNumber);
                    DRow.CreateCell(7).SetCellValue(item.UPC);
                    DRow.CreateCell(8).SetCellValue(item.UNSPSC);
                    DRow.CreateCell(9).SetCellValue(item.Description);
                    DRow.CreateCell(10).SetCellValue(item.LongDescription);
                    DRow.CreateCell(11).SetCellValue(item.CategoryName);
                    DRow.CreateCell(12).SetCellValue(item.GLAccount);
                    DRow.CreateCell(13).SetCellValue(item.Unit);
                    //DRow.CreateCell(13).SetCellValue(Convert.ToDouble(item.PricePerTerm) == null ? 0 : Convert.ToDouble(item.PricePerTerm));
                    DRow.CreateCell(14).SetCellValue(item.CostUOMName);
                    DRow.CreateCell(15).SetCellValue(item.DefaultReorderQuantity ?? 0);
                    DRow.CreateCell(16).SetCellValue(item.DefaultPullQuantity ?? 0);
                    DRow.CreateCell(17).SetCellValue(Convert.ToDouble(item.Cost));// == null ? 0 : Convert.ToDouble(item.Cost));
                    DRow.CreateCell(18).SetCellValue(Convert.ToDouble(item.Markup));// == null ? 0 : Convert.ToDouble(item.Markup));
                    DRow.CreateCell(19).SetCellValue(Convert.ToDouble(item.SellPrice));// == null ? 0 : Convert.ToDouble(item.SellPrice));
                    DRow.CreateCell(20).SetCellValue(Convert.ToDouble(item.ExtendedCost));// == null ? 0 : Convert.ToDouble(item.ExtendedCost));
                    DRow.CreateCell(21).SetCellValue(Convert.ToDouble(item.AverageCost));// == null ? 0 : Convert.ToDouble(item.AverageCost));
                    DRow.CreateCell(22).SetCellValue(item.LeadTimeInDays ?? 0);
                    DRow.CreateCell(23).SetCellValue(item.Link1);
                    DRow.CreateCell(24).SetCellValue(item.Link2);
                    //DRow.CreateCell(25).SetCellValue(item.Trend);
                    DRow.CreateCell(25).SetCellValue(item.Taxable);
                    DRow.CreateCell(26).SetCellValue(item.Consignment);
                    DRow.CreateCell(27).SetCellValue(item.StagedQuantity ?? 0);
                    DRow.CreateCell(28).SetCellValue(item.InTransitquantity ?? 0);
                    DRow.CreateCell(29).SetCellValue(item.OnOrderQuantity ?? 0);
                    DRow.CreateCell(30).SetCellValue(item.OnReturnQuantity ?? 0);
                    DRow.CreateCell(31).SetCellValue(item.OnTransferQuantity ?? 0);
                    DRow.CreateCell(32).SetCellValue(item.SuggestedOrderQuantity ?? 0);
                    DRow.CreateCell(33).SetCellValue(item.RequisitionedQuantity ?? 0);
                    //DRow.CreateCell(33).SetCellValue(item.PackingQuantity ?? 0);
                    DRow.CreateCell(34).SetCellValue(item.AverageUsage ?? 0);
                    DRow.CreateCell(35).SetCellValue(item.Turns ?? 0);
                    DRow.CreateCell(36).SetCellValue(item.OnHandQuantity ?? 0);
                    if (item.IsItemLevelMinMaxQtyRequired == true)
                    {
                        DRow.CreateCell(37).SetCellValue(item.CriticalQuantity ?? 0);
                        DRow.CreateCell(38).SetCellValue(item.MinimumQuantity.GetValueOrDefault(0));
                        DRow.CreateCell(39).SetCellValue(item.MaximumQuantity.GetValueOrDefault(0));
                    }
                    else
                    {
                        //BinMasterDTO objBinMasterDTO = new BinMasterDTO();
                        //BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                        //objBinMasterDTO = objBinMasterDAL.GetRecord(item.DefaultLocationName, item.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                        //if (objBinMasterDTO != null)
                        //{
                        //    DRow.CreateCell(38).SetCellValue(objBinMasterDTO.CriticalQuantity ?? 0);
                        //    DRow.CreateCell(39).SetCellValue(objBinMasterDTO.MinimumQuantity ?? 0);
                        //    DRow.CreateCell(40).SetCellValue(objBinMasterDTO.MaximumQuantity ?? 0);
                        //}
                        DRow.CreateCell(37).SetCellValue("N/A");
                        DRow.CreateCell(38).SetCellValue("N/A");
                        DRow.CreateCell(39).SetCellValue("N/A");
                    }

                    DRow.CreateCell(40).SetCellValue(item.WeightPerPiece ?? 0);
                    DRow.CreateCell(41).SetCellValue(item.ItemUniqueNumber);
                    DRow.CreateCell(42).SetCellValue(item.IsTransfer);
                    DRow.CreateCell(43).SetCellValue(item.IsPurchase);
                    if (item.ItemType == 4)
                        DRow.CreateCell(4).SetCellValue("");
                    else
                        DRow.CreateCell(44).SetCellValue(item.DefaultLocationName);
                    DRow.CreateCell(45).SetCellValue(item.InventoryClassificationName);
                    DRow.CreateCell(46).SetCellValue(item.SerialNumberTracking);
                    DRow.CreateCell(47).SetCellValue(item.LotNumberTracking);
                    DRow.CreateCell(48).SetCellValue(item.DateCodeTracking);
                    DRow.CreateCell(49).SetCellValue(GetItemType(item.ItemType));

                    DRow.CreateCell(50).SetCellValue(item.ImagePath);
                    DRow.CreateCell(51).SetCellValue(item.UDF1);
                    DRow.CreateCell(52).SetCellValue(item.UDF2);
                    DRow.CreateCell(53).SetCellValue(item.UDF3);
                    DRow.CreateCell(54).SetCellValue(item.UDF4);
                    DRow.CreateCell(55).SetCellValue(item.UDF5);
                    DRow.CreateCell(56).SetCellValue(item.IsLotSerialExpiryCost);
                    DRow.CreateCell(57).SetCellValue(item.IsItemLevelMinMaxQtyRequired.ToString());
                    DRow.CreateCell(58).SetCellValue(GetTrendingSetting(item.TrendingSetting));
                    DRow.CreateCell(59).SetCellValue(item.PullQtyScanOverride);
                    DRow.CreateCell(60).SetCellValue(item.IsEnforceDefaultReorderQuantity ?? false);
                    DRow.CreateCell(61).SetCellValue(item.IsAutoInventoryClassification);
                    DRow.CreateCell(62).SetCellValue(item.IsBuildBreak ?? false);


                    DRow.CreateCell(63).SetCellValue(item.IsPackslipMandatoryAtReceive.ToString());
                    DRow.CreateCell(64).SetCellValue(item.ItemImageExternalURL);
                    DRow.CreateCell(65).SetCellValue(item.ItemDocExternalURL);
                    DRow.CreateCell(66).SetCellValue(item.IsDeleted ?? false);
                    DRow.CreateCell(67).SetCellValue(item.OutTransferQuantity ?? 0);
                    DRow.CreateCell(68).SetCellValue(item.OnOrderInTransitQuantity ?? 0);
                    DRow.CreateCell(69).SetCellValue(item.ItemLink2ExternalURL);
                    DRow.CreateCell(70).SetCellValue(item.IsOrderable);
                    DRow.CreateCell(71).SetCellValue(item.UDF6);
                    DRow.CreateCell(72).SetCellValue(item.UDF7);
                    DRow.CreateCell(73).SetCellValue(item.UDF8);
                    DRow.CreateCell(74).SetCellValue(item.UDF9);
                    DRow.CreateCell(75).SetCellValue(item.UDF10);
                    DRow.CreateCell(76).SetCellValue(item.IsAllowOrderCostuom);
                    DRow.CreateCell(77).SetCellValue(item.eLabelKey);
                    DRow.CreateCell(78).SetCellValue(item.EnrichedProductData);
                    DRow.CreateCell(79).SetCellValue(item.EnhancedDescription);
                    DRow.CreateCell(80).SetCellValue(item.POItemLineNumber?? Convert.ToDouble(item.POItemLineNumber));
                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }

        public string AlertStockOutListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();
            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> lstIds = arrid.Select(t => Guid.Parse(t)).ToList();
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID);
            SortNameString = (SortNameString ?? string.Empty).Replace("OrderCost", "ItemNumber");
            IEnumerable<ItemMasterDTO> DataFromDB = obj.GetItemsByArray(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString);
            List<ItemMasterDTO> lstItemMasterDTO = new List<ItemMasterDTO>();

            if (!string.IsNullOrEmpty(ids))
            {
                lstItemMasterDTO = (from u in DataFromDB
                                    where arrid.Contains(u.GUID.ToString())
                                    select new ItemMasterDTO
                                    {
                                        ID = u.ID,
                                        ItemNumber = u.ItemNumber,
                                        ManufacturerID = u.ManufacturerID,
                                        ManufacturerNumber = u.ManufacturerNumber,
                                        ManufacturerName = u.ManufacturerName,
                                        SupplierID = u.SupplierID,
                                        SupplierPartNo = u.SupplierPartNo,
                                        SupplierName = u.SupplierName,
                                        UPC = u.UPC,
                                        UNSPSC = u.UNSPSC,
                                        Description = u.Description,
                                        LongDescription = u.LongDescription,
                                        CategoryID = u.CategoryID,
                                        GLAccountID = u.GLAccountID,
                                        UOMID = u.UOMID,

                                        PricePerTerm = u.PricePerTerm,
                                        CostUOMID = u.CostUOMID,
                                        CostUOMName = u.CostUOMName,
                                        DefaultReorderQuantity = u.DefaultReorderQuantity,
                                        DefaultPullQuantity = u.DefaultPullQuantity,
                                        Cost = u.Cost,
                                        Markup = u.Markup,
                                        SellPrice = u.SellPrice,
                                        ExtendedCost = u.ExtendedCost,
                                        AverageCost = u.AverageCost,
                                        LeadTimeInDays = u.LeadTimeInDays,
                                        Link1 = u.Link1,
                                        Link2 = u.Link2,
                                        Trend = u.Trend,
                                        Taxable = u.Taxable,
                                        Consignment = u.Consignment,
                                        StagedQuantity = u.StagedQuantity,
                                        InTransitquantity = u.InTransitquantity,
                                        OnOrderQuantity = u.OnOrderQuantity,
                                        OnReturnQuantity = u.OnReturnQuantity,
                                        OnTransferQuantity = u.OnTransferQuantity,
                                        SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                        SuggestedTransferQuantity = u.SuggestedTransferQuantity,
                                        RequisitionedQuantity = u.RequisitionedQuantity,
                                        PackingQuantity = u.PackingQuantity,
                                        AverageUsage = u.AverageUsage,
                                        Turns = u.Turns,
                                        OnHandQuantity = 0,
                                        CriticalQuantity = u.CriticalQuantity,
                                        MinimumQuantity = u.MinimumQuantity,
                                        MaximumQuantity = u.MaximumQuantity,
                                        WeightPerPiece = u.WeightPerPiece,
                                        ItemUniqueNumber = u.ItemUniqueNumber,
                                        IsPurchase = u.IsPurchase,
                                        IsTransfer = u.IsTransfer,
                                        DefaultLocation = u.DefaultLocation,
                                        DefaultLocationName = u.DefaultLocationName,
                                        InventoryClassification = u.InventoryClassification,
                                        InventoryClassificationName = u.InventoryClassificationName,
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
                                        IsItemLevelMinMaxQtyRequired = (u.IsItemLevelMinMaxQtyRequired.HasValue ? u.IsItemLevelMinMaxQtyRequired : false),
                                        IsEnforceDefaultReorderQuantity = (u.IsEnforceDefaultReorderQuantity.HasValue ? u.IsEnforceDefaultReorderQuantity : false),
                                        IsBuildBreak = (u.IsBuildBreak.HasValue ? u.IsBuildBreak : false),
                                        BondedInventory = u.BondedInventory,
                                        ItemLocations = objLocationDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, u.GUID, null, "ID ASC").ToList(),
                                        AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Item Master"),
                                        IsPackslipMandatoryAtReceive = u.IsPackslipMandatoryAtReceive,
                                        BlanketOrderNumber = u.BlanketOrderNumber,
                                        ItemImageExternalURL = u.ItemImageExternalURL,
                                        ItemDocExternalURL = u.ItemDocExternalURL,
                                        TrendingSetting = u.TrendingSetting,
                                        PullQtyScanOverride = u.PullQtyScanOverride,
                                        IsAutoInventoryClassification = u.IsAutoInventoryClassification,
                                        OutTransferQuantity = u.OutTransferQuantity,
                                        OnOrderInTransitQuantity = u.OnOrderInTransitQuantity ?? 0,
                                        ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                                        IsActive = (u.IsActive),
                                        UDF6 = u.UDF6,
                                        UDF7 = u.UDF7,
                                        UDF8 = u.UDF8,
                                        UDF9 = u.UDF9,
                                        UDF10 = u.UDF10,
                                        IsAllowOrderCostuom = u.IsAllowOrderCostuom
                                    }).ToList();
            }




            if (lstItemMasterDTO != null && lstItemMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* ItemNumber");
                row.CreateCell(2).SetCellValue("Manufacturer");
                row.CreateCell(3).SetCellValue("ManufacturerNumber");
                row.CreateCell(4).SetCellValue("* SupplierName");
                row.CreateCell(5).SetCellValue("* SupplierPartNo");
                row.CreateCell(6).SetCellValue("BlanketOrderNumber");
                row.CreateCell(7).SetCellValue("UPC");
                row.CreateCell(8).SetCellValue("UNSPSC");
                row.CreateCell(9).SetCellValue("Description");
                row.CreateCell(10).SetCellValue("LongDescription");
                row.CreateCell(11).SetCellValue("CategoryName");
                row.CreateCell(12).SetCellValue("GLAccount");
                row.CreateCell(13).SetCellValue("* UOM");
                row.CreateCell(14).SetCellValue("* CostUOM");
                row.CreateCell(15).SetCellValue("* DefaultReorderQuantity");
                row.CreateCell(16).SetCellValue("* DefaultPullQuantity");
                row.CreateCell(17).SetCellValue("Cost");
                row.CreateCell(18).SetCellValue("Markup");
                row.CreateCell(19).SetCellValue("SellPrice");
                row.CreateCell(20).SetCellValue("ExtendedCost");
                row.CreateCell(21).SetCellValue("AverageCost");
                row.CreateCell(22).SetCellValue("LeadTimeInDays");
                row.CreateCell(23).SetCellValue("Link1");
                row.CreateCell(24).SetCellValue("Link2");
                //row.CreateCell(25).SetCellValue("Trend");
                row.CreateCell(25).SetCellValue("Taxable");
                row.CreateCell(26).SetCellValue("Consignment");
                row.CreateCell(27).SetCellValue("StagedQuantity");
                row.CreateCell(28).SetCellValue("InTransitquantity");
                row.CreateCell(29).SetCellValue("OnOrderQuantity");
                row.CreateCell(30).SetCellValue("OnReturnQuantity");
                row.CreateCell(31).SetCellValue("OnTransferQuantity");
                row.CreateCell(32).SetCellValue("SuggestedOrderQuantity");
                row.CreateCell(33).SetCellValue("RequisitionedQuantity");
                //row.CreateCell(33).SetCellValue("PackingQuantity");
                row.CreateCell(34).SetCellValue("AverageUsage");
                row.CreateCell(35).SetCellValue("Turns");
                row.CreateCell(36).SetCellValue("OnHandQuantity");
                row.CreateCell(37).SetCellValue("CriticalQuantity");
                row.CreateCell(38).SetCellValue("MinimumQuantity");
                row.CreateCell(39).SetCellValue("MaximumQuantity");
                row.CreateCell(40).SetCellValue("WeightPerPiece");
                row.CreateCell(41).SetCellValue("ItemUniqueNumber");
                row.CreateCell(42).SetCellValue("IsTransfer");
                row.CreateCell(43).SetCellValue("IsPurchase");
                row.CreateCell(44).SetCellValue("* InventryLocation");
                row.CreateCell(45).SetCellValue("InventoryClassification");
                row.CreateCell(46).SetCellValue("SerialNumberTracking");
                row.CreateCell(47).SetCellValue("LotNumberTracking");
                row.CreateCell(48).SetCellValue("DateCodeTracking");
                row.CreateCell(49).SetCellValue("* ItemType");
                row.CreateCell(50).SetCellValue("ImagePath");
                row.CreateCell(51).SetCellValue("UDF1");
                row.CreateCell(52).SetCellValue("UDF2");
                row.CreateCell(53).SetCellValue("UDF3");
                row.CreateCell(54).SetCellValue("UDF4");
                row.CreateCell(55).SetCellValue("UDF5");
                row.CreateCell(56).SetCellValue("IsLotSerialExpiryCost");
                row.CreateCell(57).SetCellValue("* IsItemLevelMinMaxQtyRequired");

                row.CreateCell(58).SetCellValue("TrendingSetting");
                row.CreateCell(59).SetCellValue("EnforceDefaultPullQuantity");
                row.CreateCell(60).SetCellValue("EnforceDefaultReorderQuantity");
                row.CreateCell(61).SetCellValue("IsAutoInventoryClassification");
                row.CreateCell(62).SetCellValue("IsBuildBreak");


                row.CreateCell(63).SetCellValue("IsPackslipMandatoryAtReceive");
                row.CreateCell(64).SetCellValue("ItemImageExternalURL");
                row.CreateCell(65).SetCellValue("ItemDocExternalURL");
                row.CreateCell(66).SetCellValue("IsDeleted");
                row.CreateCell(67).SetCellValue("OutTransferQuantity");
                row.CreateCell(68).SetCellValue("OnOrderInTransitQuantity");
                row.CreateCell(69).SetCellValue("ItemLink2ExternalURL");
                row.CreateCell(70).SetCellValue("IsActive");
                row.CreateCell(71).SetCellValue("UDF6");
                row.CreateCell(72).SetCellValue("UDF7");
                row.CreateCell(73).SetCellValue("UDF8");
                row.CreateCell(74).SetCellValue("UDF9");
                row.CreateCell(75).SetCellValue("UDF10");
                row.CreateCell(76).SetCellValue("IsAllowOrderCostuom");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ItemMasterDTO item in lstItemMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.ItemNumber);
                    DRow.CreateCell(2).SetCellValue(item.ManufacturerName);
                    DRow.CreateCell(3).SetCellValue(item.ManufacturerNumber);
                    DRow.CreateCell(4).SetCellValue(item.SupplierName);
                    DRow.CreateCell(5).SetCellValue(item.SupplierPartNo);
                    DRow.CreateCell(6).SetCellValue(item.BlanketOrderNumber);
                    DRow.CreateCell(7).SetCellValue(item.UPC);
                    DRow.CreateCell(8).SetCellValue(item.UNSPSC);
                    DRow.CreateCell(9).SetCellValue(item.Description);
                    DRow.CreateCell(10).SetCellValue(item.LongDescription);
                    DRow.CreateCell(11).SetCellValue(item.CategoryName);
                    DRow.CreateCell(12).SetCellValue(item.GLAccount);
                    DRow.CreateCell(13).SetCellValue(item.Unit);
                    DRow.CreateCell(14).SetCellValue(item.CostUOMName);
                    DRow.CreateCell(15).SetCellValue(item.DefaultReorderQuantity ?? 0);
                    DRow.CreateCell(16).SetCellValue(item.DefaultPullQuantity ?? 0);
                    DRow.CreateCell(17).SetCellValue(Convert.ToDouble(item.Cost));
                    DRow.CreateCell(18).SetCellValue(Convert.ToDouble(item.Markup));
                    DRow.CreateCell(19).SetCellValue(Convert.ToDouble(item.SellPrice));
                    DRow.CreateCell(20).SetCellValue(Convert.ToDouble(item.ExtendedCost));
                    DRow.CreateCell(21).SetCellValue(Convert.ToDouble(item.AverageCost));
                    DRow.CreateCell(22).SetCellValue(item.LeadTimeInDays ?? 0);
                    DRow.CreateCell(23).SetCellValue(item.Link1);
                    DRow.CreateCell(24).SetCellValue(item.Link2);
                    DRow.CreateCell(25).SetCellValue(item.Taxable);
                    DRow.CreateCell(26).SetCellValue(item.Consignment);
                    DRow.CreateCell(27).SetCellValue(item.StagedQuantity ?? 0);
                    DRow.CreateCell(28).SetCellValue(item.InTransitquantity ?? 0);
                    DRow.CreateCell(29).SetCellValue(item.OnOrderQuantity ?? 0);
                    DRow.CreateCell(30).SetCellValue(item.OnReturnQuantity ?? 0);
                    DRow.CreateCell(31).SetCellValue(item.OnTransferQuantity ?? 0);
                    DRow.CreateCell(32).SetCellValue(item.SuggestedOrderQuantity ?? 0);
                    DRow.CreateCell(33).SetCellValue(item.RequisitionedQuantity ?? 0);
                    DRow.CreateCell(34).SetCellValue(item.AverageUsage ?? 0);
                    DRow.CreateCell(35).SetCellValue(item.Turns ?? 0);
                    DRow.CreateCell(36).SetCellValue(item.OnHandQuantity ?? 0);
                    if (item.IsItemLevelMinMaxQtyRequired == true)
                    {
                        DRow.CreateCell(37).SetCellValue(item.CriticalQuantity ?? 0);
                        DRow.CreateCell(38).SetCellValue(item.MinimumQuantity.GetValueOrDefault(0));
                        DRow.CreateCell(39).SetCellValue(item.MaximumQuantity.GetValueOrDefault(0));
                    }
                    else
                    {
                        DRow.CreateCell(37).SetCellValue("N/A");
                        DRow.CreateCell(38).SetCellValue("N/A");
                        DRow.CreateCell(39).SetCellValue("N/A");
                    }

                    DRow.CreateCell(40).SetCellValue(item.WeightPerPiece ?? 0);
                    DRow.CreateCell(41).SetCellValue(item.ItemUniqueNumber);
                    DRow.CreateCell(42).SetCellValue(item.IsTransfer);
                    DRow.CreateCell(43).SetCellValue(item.IsPurchase);
                    DRow.CreateCell(44).SetCellValue(item.DefaultLocationName);
                    DRow.CreateCell(45).SetCellValue(item.InventoryClassificationName);
                    DRow.CreateCell(46).SetCellValue(item.SerialNumberTracking);
                    DRow.CreateCell(47).SetCellValue(item.LotNumberTracking);
                    DRow.CreateCell(48).SetCellValue(item.DateCodeTracking);
                    DRow.CreateCell(49).SetCellValue(GetItemType(item.ItemType));

                    DRow.CreateCell(50).SetCellValue(item.ImagePath);
                    DRow.CreateCell(51).SetCellValue(item.UDF1);
                    DRow.CreateCell(52).SetCellValue(item.UDF2);
                    DRow.CreateCell(53).SetCellValue(item.UDF3);
                    DRow.CreateCell(54).SetCellValue(item.UDF4);
                    DRow.CreateCell(55).SetCellValue(item.UDF5);
                    DRow.CreateCell(56).SetCellValue(item.IsLotSerialExpiryCost);
                    DRow.CreateCell(57).SetCellValue(item.IsItemLevelMinMaxQtyRequired.ToString());
                    DRow.CreateCell(58).SetCellValue(GetTrendingSetting(item.TrendingSetting));
                    DRow.CreateCell(59).SetCellValue(item.PullQtyScanOverride);
                    DRow.CreateCell(60).SetCellValue(item.IsEnforceDefaultReorderQuantity ?? false);
                    DRow.CreateCell(61).SetCellValue(item.IsAutoInventoryClassification);
                    DRow.CreateCell(62).SetCellValue(item.IsBuildBreak ?? false);
                    DRow.CreateCell(63).SetCellValue(item.IsPackslipMandatoryAtReceive.ToString());
                    DRow.CreateCell(64).SetCellValue(item.ItemImageExternalURL);
                    DRow.CreateCell(65).SetCellValue(item.ItemDocExternalURL);
                    DRow.CreateCell(66).SetCellValue(item.IsDeleted ?? false);
                    DRow.CreateCell(67).SetCellValue(item.OutTransferQuantity ?? 0);
                    DRow.CreateCell(68).SetCellValue(item.OnOrderInTransitQuantity ?? 0);
                    DRow.CreateCell(69).SetCellValue(item.ItemLink2ExternalURL);
                    DRow.CreateCell(70).SetCellValue(item.IsActive);
                    DRow.CreateCell(71).SetCellValue(item.UDF6);
                    DRow.CreateCell(72).SetCellValue(item.UDF7);
                    DRow.CreateCell(73).SetCellValue(item.UDF8);
                    DRow.CreateCell(74).SetCellValue(item.UDF9);
                    DRow.CreateCell(75).SetCellValue(item.UDF10);
                    DRow.CreateCell(76).SetCellValue(item.IsAllowOrderCostuom);
                    RowId += 1;
                    //-------------End--------------------------
                }
            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }

        public string ItemBinMasterListExcel(string Filepath, string ModuleName, string ids, string SortNameString, string BinIDs)
        {
            InitializeWorkbook();
            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            //string[] arrid = ids.Split(',');
            string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> lstIds = arrid.Select(t => Guid.Parse(t)).ToList();
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID);
            //////IEnumerable<ItemMasterDTO> DataFromDB = obj.GetAllItemsWithJoins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null).OrderBy(SortNameString);
            SortNameString = (SortNameString ?? string.Empty).Replace("OrderCost", "ItemNumber");
            IEnumerable<ItemMasterDTO> DataFromDB = obj.GetItemsBinByArray(lstIds, SessionHelper.RoomID, SessionHelper.CompanyID, BinIDs).OrderBy(SortNameString);//.Where(i => i.Room == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID).OrderBy(SortNameString);
            List<ItemMasterDTO> lstItemMasterDTO = new List<ItemMasterDTO>();

            if (!string.IsNullOrEmpty(ids))
            {
                lstItemMasterDTO = (from u in DataFromDB
                                    where arrid.Contains(u.GUID.ToString())
                                    select new ItemMasterDTO
                                    {
                                        ID = u.ID,
                                        ItemNumber = u.ItemNumber,
                                        ManufacturerID = u.ManufacturerID,
                                        ManufacturerNumber = u.ManufacturerNumber,
                                        ManufacturerName = u.ManufacturerName,
                                        SupplierID = u.SupplierID,
                                        SupplierPartNo = u.SupplierPartNo,
                                        SupplierName = u.SupplierName,
                                        UPC = u.UPC,
                                        UNSPSC = u.UNSPSC,
                                        Description = u.Description,
                                        LongDescription = u.LongDescription,
                                        CategoryID = u.CategoryID,
                                        GLAccountID = u.GLAccountID,
                                        UOMID = u.UOMID,

                                        PricePerTerm = u.PricePerTerm,
                                        CostUOMID = u.CostUOMID,
                                        CostUOMName = u.CostUOMName,
                                        DefaultReorderQuantity = u.DefaultReorderQuantity,
                                        DefaultPullQuantity = u.DefaultPullQuantity,
                                        Cost = u.Cost,
                                        Markup = u.Markup,
                                        SellPrice = u.SellPrice,
                                        ExtendedCost = u.ExtendedCost,
                                        AverageCost = u.AverageCost,
                                        LeadTimeInDays = u.LeadTimeInDays,
                                        Link1 = u.Link1,
                                        Link2 = u.Link2,
                                        Trend = u.Trend,
                                        Taxable = u.Taxable,
                                        Consignment = u.Consignment,
                                        StagedQuantity = u.StagedQuantity,
                                        InTransitquantity = u.InTransitquantity,
                                        OnOrderQuantity = u.OnOrderQuantity,
                                        OnReturnQuantity = u.OnReturnQuantity,
                                        OnTransferQuantity = u.OnTransferQuantity,
                                        SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                        SuggestedTransferQuantity = u.SuggestedTransferQuantity,
                                        RequisitionedQuantity = u.RequisitionedQuantity,
                                        PackingQuantity = u.PackingQuantity,
                                        AverageUsage = u.AverageUsage,
                                        Turns = u.Turns,
                                        OnHandQuantity = u.OnHandQuantity,
                                        CriticalQuantity = u.CriticalQuantity,
                                        MinimumQuantity = u.MinimumQuantity,
                                        MaximumQuantity = u.MaximumQuantity,
                                        WeightPerPiece = u.WeightPerPiece,
                                        ItemUniqueNumber = u.ItemUniqueNumber,
                                        IsPurchase = u.IsPurchase,
                                        IsTransfer = u.IsTransfer,
                                        DefaultLocation = u.DefaultLocation,
                                        DefaultLocationName = u.DefaultLocationName,
                                        InventoryClassification = u.InventoryClassification,
                                        InventoryClassificationName = u.InventoryClassificationName,
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
                                        IsItemLevelMinMaxQtyRequired = (u.IsItemLevelMinMaxQtyRequired.HasValue ? u.IsItemLevelMinMaxQtyRequired : false),
                                        IsEnforceDefaultReorderQuantity = (u.IsEnforceDefaultReorderQuantity.HasValue ? u.IsEnforceDefaultReorderQuantity : false),
                                        IsBuildBreak = (u.IsBuildBreak.HasValue ? u.IsBuildBreak : false),
                                        BondedInventory = u.BondedInventory,
                                        ItemLocations = objLocationDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, u.GUID, null, "ID ASC").ToList(),
                                        AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Item Master"),
                                        IsPackslipMandatoryAtReceive = u.IsPackslipMandatoryAtReceive,
                                        BlanketOrderNumber = u.BlanketOrderNumber,
                                        ItemImageExternalURL = u.ItemImageExternalURL,
                                        ItemDocExternalURL = u.ItemDocExternalURL,
                                        TrendingSetting = u.TrendingSetting,
                                        PullQtyScanOverride = u.PullQtyScanOverride,
                                        IsAutoInventoryClassification = u.IsAutoInventoryClassification,
                                        OutTransferQuantity = u.OutTransferQuantity,
                                        OnOrderInTransitQuantity = u.OnOrderInTransitQuantity ?? 0,
                                        ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                                        IsActive = (u.IsActive),
                                        UDF6 = u.UDF6,
                                        UDF7 = u.UDF7,
                                        UDF8 = u.UDF8,
                                        UDF9 = u.UDF9,
                                        UDF10 = u.UDF10,
                                        IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                        BinID = u.BinID,
                                        BinNumber = u.BinNumber
                                    }).ToList();
            }




            if (lstItemMasterDTO != null && lstItemMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* ItemNumber");
                row.CreateCell(2).SetCellValue("Manufacturer");
                row.CreateCell(3).SetCellValue("ManufacturerNumber");
                row.CreateCell(4).SetCellValue("* SupplierName");
                row.CreateCell(5).SetCellValue("* SupplierPartNo");
                row.CreateCell(6).SetCellValue("BlanketOrderNumber");
                row.CreateCell(7).SetCellValue("UPC");
                row.CreateCell(8).SetCellValue("UNSPSC");
                row.CreateCell(9).SetCellValue("Description");
                row.CreateCell(10).SetCellValue("LongDescription");
                row.CreateCell(11).SetCellValue("CategoryName");
                row.CreateCell(12).SetCellValue("GLAccount");
                row.CreateCell(13).SetCellValue("* UOM");
                row.CreateCell(14).SetCellValue("* CostUOM");
                row.CreateCell(15).SetCellValue("* DefaultReorderQuantity");
                row.CreateCell(16).SetCellValue("* DefaultPullQuantity");
                row.CreateCell(17).SetCellValue("Cost");
                row.CreateCell(18).SetCellValue("Markup");
                row.CreateCell(19).SetCellValue("SellPrice");
                row.CreateCell(20).SetCellValue("ExtendedCost");
                row.CreateCell(21).SetCellValue("AverageCost");
                row.CreateCell(22).SetCellValue("LeadTimeInDays");
                row.CreateCell(23).SetCellValue("Link1");
                row.CreateCell(24).SetCellValue("Link2");
                //row.CreateCell(25).SetCellValue("Trend");
                row.CreateCell(25).SetCellValue("Taxable");
                row.CreateCell(26).SetCellValue("Consignment");
                row.CreateCell(27).SetCellValue("StagedQuantity");
                row.CreateCell(28).SetCellValue("InTransitquantity");
                row.CreateCell(29).SetCellValue("OnOrderQuantity");
                row.CreateCell(30).SetCellValue("OnReturnQuantity");
                row.CreateCell(31).SetCellValue("OnTransferQuantity");
                row.CreateCell(32).SetCellValue("SuggestedOrderQuantity");
                row.CreateCell(33).SetCellValue("RequisitionedQuantity");
                //row.CreateCell(33).SetCellValue("PackingQuantity");
                row.CreateCell(34).SetCellValue("AverageUsage");
                row.CreateCell(35).SetCellValue("Turns");
                row.CreateCell(36).SetCellValue("OnHandQuantity");
                row.CreateCell(37).SetCellValue("CriticalQuantity");
                row.CreateCell(38).SetCellValue("MinimumQuantity");
                row.CreateCell(39).SetCellValue("MaximumQuantity");
                row.CreateCell(40).SetCellValue("WeightPerPiece");
                row.CreateCell(41).SetCellValue("ItemUniqueNumber");
                row.CreateCell(42).SetCellValue("IsTransfer");
                row.CreateCell(43).SetCellValue("IsPurchase");
                row.CreateCell(44).SetCellValue("* InventryLocation");
                row.CreateCell(45).SetCellValue("InventoryClassification");
                row.CreateCell(46).SetCellValue("SerialNumberTracking");
                row.CreateCell(47).SetCellValue("LotNumberTracking");
                row.CreateCell(48).SetCellValue("DateCodeTracking");
                row.CreateCell(49).SetCellValue("* ItemType");
                row.CreateCell(50).SetCellValue("ImagePath");
                row.CreateCell(51).SetCellValue("UDF1");
                row.CreateCell(52).SetCellValue("UDF2");
                row.CreateCell(53).SetCellValue("UDF3");
                row.CreateCell(54).SetCellValue("UDF4");
                row.CreateCell(55).SetCellValue("UDF5");
                row.CreateCell(56).SetCellValue("IsLotSerialExpiryCost");
                row.CreateCell(57).SetCellValue("* IsItemLevelMinMaxQtyRequired");

                row.CreateCell(58).SetCellValue("TrendingSetting");
                row.CreateCell(59).SetCellValue("EnforceDefaultPullQuantity");
                row.CreateCell(60).SetCellValue("EnforceDefaultReorderQuantity");
                row.CreateCell(61).SetCellValue("IsAutoInventoryClassification");
                row.CreateCell(62).SetCellValue("IsBuildBreak");


                row.CreateCell(63).SetCellValue("IsPackslipMandatoryAtReceive");
                row.CreateCell(64).SetCellValue("ItemImageExternalURL");
                row.CreateCell(65).SetCellValue("ItemDocExternalURL");
                row.CreateCell(66).SetCellValue("IsDeleted");
                row.CreateCell(67).SetCellValue("OutTransferQuantity");
                row.CreateCell(68).SetCellValue("OnOrderInTransitQuantity");
                row.CreateCell(69).SetCellValue("ItemLink2ExternalURL");
                row.CreateCell(70).SetCellValue("IsActive");
                row.CreateCell(71).SetCellValue("UDF6");
                row.CreateCell(72).SetCellValue("UDF7");
                row.CreateCell(73).SetCellValue("UDF8");
                row.CreateCell(74).SetCellValue("UDF9");
                row.CreateCell(75).SetCellValue("UDF10");
                row.CreateCell(76).SetCellValue("IsAllowOrderCostuom");
                row.CreateCell(77).SetCellValue("BinNumber");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ItemMasterDTO item in lstItemMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.ItemNumber);
                    DRow.CreateCell(2).SetCellValue(item.ManufacturerName);
                    DRow.CreateCell(3).SetCellValue(item.ManufacturerNumber);
                    DRow.CreateCell(4).SetCellValue(item.SupplierName);
                    DRow.CreateCell(5).SetCellValue(item.SupplierPartNo);
                    DRow.CreateCell(6).SetCellValue(item.BlanketOrderNumber);
                    DRow.CreateCell(7).SetCellValue(item.UPC);
                    DRow.CreateCell(8).SetCellValue(item.UNSPSC);
                    DRow.CreateCell(9).SetCellValue(item.Description);
                    DRow.CreateCell(10).SetCellValue(item.LongDescription);
                    DRow.CreateCell(11).SetCellValue(item.CategoryName);
                    DRow.CreateCell(12).SetCellValue(item.GLAccount);
                    DRow.CreateCell(13).SetCellValue(item.Unit);
                    //DRow.CreateCell(13).SetCellValue(Convert.ToDouble(item.PricePerTerm) == null ? 0 : Convert.ToDouble(item.PricePerTerm));
                    DRow.CreateCell(14).SetCellValue(item.CostUOMName);
                    DRow.CreateCell(15).SetCellValue(item.DefaultReorderQuantity ?? 0);
                    DRow.CreateCell(16).SetCellValue(item.DefaultPullQuantity ?? 0);
                    DRow.CreateCell(17).SetCellValue(Convert.ToDouble(item.Cost));// == null ? 0 : Convert.ToDouble(item.Cost));
                    DRow.CreateCell(18).SetCellValue(Convert.ToDouble(item.Markup));// == null ? 0 : Convert.ToDouble(item.Markup));
                    DRow.CreateCell(19).SetCellValue(Convert.ToDouble(item.SellPrice));// == null ? 0 : Convert.ToDouble(item.SellPrice));
                    DRow.CreateCell(20).SetCellValue(Convert.ToDouble(item.ExtendedCost));// == null ? 0 : Convert.ToDouble(item.ExtendedCost));
                    DRow.CreateCell(21).SetCellValue(Convert.ToDouble(item.AverageCost));// == null ? 0 : Convert.ToDouble(item.AverageCost));
                    DRow.CreateCell(22).SetCellValue(item.LeadTimeInDays ?? 0);
                    DRow.CreateCell(23).SetCellValue(item.Link1);
                    DRow.CreateCell(24).SetCellValue(item.Link2);
                    //DRow.CreateCell(25).SetCellValue(item.Trend);
                    DRow.CreateCell(25).SetCellValue(item.Taxable);
                    DRow.CreateCell(26).SetCellValue(item.Consignment);
                    DRow.CreateCell(27).SetCellValue(item.StagedQuantity ?? 0);
                    DRow.CreateCell(28).SetCellValue(item.InTransitquantity ?? 0);
                    DRow.CreateCell(29).SetCellValue(item.OnOrderQuantity ?? 0);
                    DRow.CreateCell(30).SetCellValue(item.OnReturnQuantity ?? 0);
                    DRow.CreateCell(31).SetCellValue(item.OnTransferQuantity ?? 0);
                    DRow.CreateCell(32).SetCellValue(item.SuggestedOrderQuantity ?? 0);
                    DRow.CreateCell(33).SetCellValue(item.RequisitionedQuantity ?? 0);
                    //DRow.CreateCell(33).SetCellValue(item.PackingQuantity ?? 0);
                    DRow.CreateCell(34).SetCellValue(item.AverageUsage ?? 0);
                    DRow.CreateCell(35).SetCellValue(item.Turns ?? 0);
                    DRow.CreateCell(36).SetCellValue(item.OnHandQuantity ?? 0);
                    //if (item.IsItemLevelMinMaxQtyRequired == true)
                    //{
                    DRow.CreateCell(37).SetCellValue(item.CriticalQuantity ?? 0);
                    DRow.CreateCell(38).SetCellValue(item.MinimumQuantity.GetValueOrDefault(0));
                    DRow.CreateCell(39).SetCellValue(item.MaximumQuantity.GetValueOrDefault(0));
                    //}
                    //else
                    //{
                    //    //BinMasterDTO objBinMasterDTO = new BinMasterDTO();
                    //    //BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    //    //objBinMasterDTO = objBinMasterDAL.GetRecord(item.DefaultLocationName, item.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    //    //if (objBinMasterDTO != null)
                    //    //{
                    //    //    DRow.CreateCell(38).SetCellValue(objBinMasterDTO.CriticalQuantity ?? 0);
                    //    //    DRow.CreateCell(39).SetCellValue(objBinMasterDTO.MinimumQuantity ?? 0);
                    //    //    DRow.CreateCell(40).SetCellValue(objBinMasterDTO.MaximumQuantity ?? 0);
                    //    //}
                    //    DRow.CreateCell(37).SetCellValue("N/A");
                    //    DRow.CreateCell(38).SetCellValue("N/A");
                    //    DRow.CreateCell(39).SetCellValue("N/A");
                    //}

                    DRow.CreateCell(40).SetCellValue(item.WeightPerPiece ?? 0);
                    DRow.CreateCell(41).SetCellValue(item.ItemUniqueNumber);
                    DRow.CreateCell(42).SetCellValue(item.IsTransfer);
                    DRow.CreateCell(43).SetCellValue(item.IsPurchase);
                    DRow.CreateCell(44).SetCellValue(item.DefaultLocationName);
                    DRow.CreateCell(45).SetCellValue(item.InventoryClassificationName);
                    DRow.CreateCell(46).SetCellValue(item.SerialNumberTracking);
                    DRow.CreateCell(47).SetCellValue(item.LotNumberTracking);
                    DRow.CreateCell(48).SetCellValue(item.DateCodeTracking);
                    DRow.CreateCell(49).SetCellValue(GetItemType(item.ItemType));

                    DRow.CreateCell(50).SetCellValue(item.ImagePath);
                    DRow.CreateCell(51).SetCellValue(item.UDF1);
                    DRow.CreateCell(52).SetCellValue(item.UDF2);
                    DRow.CreateCell(53).SetCellValue(item.UDF3);
                    DRow.CreateCell(54).SetCellValue(item.UDF4);
                    DRow.CreateCell(55).SetCellValue(item.UDF5);
                    DRow.CreateCell(56).SetCellValue(item.IsLotSerialExpiryCost);
                    DRow.CreateCell(57).SetCellValue(item.IsItemLevelMinMaxQtyRequired.ToString());
                    DRow.CreateCell(58).SetCellValue(GetTrendingSetting(item.TrendingSetting));
                    DRow.CreateCell(59).SetCellValue(item.PullQtyScanOverride);
                    DRow.CreateCell(60).SetCellValue(item.IsEnforceDefaultReorderQuantity ?? false);
                    DRow.CreateCell(61).SetCellValue(item.IsAutoInventoryClassification);
                    DRow.CreateCell(62).SetCellValue(item.IsBuildBreak ?? false);


                    DRow.CreateCell(63).SetCellValue(item.IsPackslipMandatoryAtReceive.ToString());
                    DRow.CreateCell(64).SetCellValue(item.ItemImageExternalURL);
                    DRow.CreateCell(65).SetCellValue(item.ItemDocExternalURL);
                    DRow.CreateCell(66).SetCellValue(item.IsDeleted ?? false);
                    DRow.CreateCell(67).SetCellValue(item.OutTransferQuantity ?? 0);
                    DRow.CreateCell(68).SetCellValue(item.OnOrderInTransitQuantity ?? 0);
                    DRow.CreateCell(69).SetCellValue(item.ItemLink2ExternalURL);
                    DRow.CreateCell(70).SetCellValue(item.IsActive);
                    DRow.CreateCell(71).SetCellValue(item.UDF6);
                    DRow.CreateCell(72).SetCellValue(item.UDF7);
                    DRow.CreateCell(73).SetCellValue(item.UDF8);
                    DRow.CreateCell(74).SetCellValue(item.UDF9);
                    DRow.CreateCell(75).SetCellValue(item.UDF10);
                    DRow.CreateCell(76).SetCellValue(item.IsAllowOrderCostuom);
                    DRow.CreateCell(77).SetCellValue(item.BinNumber);
                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }

        public string DashboardItemMinMaxListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();
            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            //string[] arrid = ids.Split(',');
            string[] arrid = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> lstIds = arrid.Select(t => Guid.Parse(t)).ToList();
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID);
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

            if (!string.IsNullOrEmpty(ids))
            {
                lstItemMasterDTO = (from u in DataFromDB
                                    where arrid.Contains(u.GUID.ToString())
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




            if (lstItemMasterDTO != null && lstItemMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("ItemNumber");
                row.CreateCell(2).SetCellValue("BinNumber");
                row.CreateCell(3).SetCellValue("Manufacturer");
                row.CreateCell(4).SetCellValue("ManufacturerNumber");
                row.CreateCell(5).SetCellValue("SupplierName");
                row.CreateCell(6).SetCellValue("SupplierPartNo");
                row.CreateCell(7).SetCellValue("BlanketOrderNumber");
                row.CreateCell(8).SetCellValue("UPC");
                row.CreateCell(9).SetCellValue("UNSPSC");
                row.CreateCell(10).SetCellValue("OnHandQuantity");
                row.CreateCell(11).SetCellValue("CriticalQuantity");
                row.CreateCell(12).SetCellValue("MinimumQuantity");
                row.CreateCell(13).SetCellValue("MaximumQuantity");
                row.CreateCell(14).SetCellValue("Description");
                row.CreateCell(15).SetCellValue("LongDescription");
                row.CreateCell(16).SetCellValue("CategoryName");
                row.CreateCell(17).SetCellValue("CostUOM");
                row.CreateCell(18).SetCellValue("DefaultReorderQuantity");
                row.CreateCell(19).SetCellValue("DefaultPullQuantity");
                row.CreateCell(20).SetCellValue("Cost");
                row.CreateCell(21).SetCellValue("Markup");
                row.CreateCell(22).SetCellValue("SellPrice");
                row.CreateCell(23).SetCellValue("ExtendedCost");
                row.CreateCell(24).SetCellValue("AverageCost");
                row.CreateCell(25).SetCellValue("PerItemCost");
                row.CreateCell(26).SetCellValue("LeadTimeInDays");
                row.CreateCell(27).SetCellValue("Link1");
                row.CreateCell(28).SetCellValue("Link2");
                row.CreateCell(29).SetCellValue("Trend");
                row.CreateCell(30).SetCellValue("Taxable");
                row.CreateCell(31).SetCellValue("Consignment");
                row.CreateCell(32).SetCellValue("StagedQuantity");
                row.CreateCell(33).SetCellValue("InTransitquantity");
                row.CreateCell(34).SetCellValue("OnOrderQuantity");
                row.CreateCell(35).SetCellValue("OnReturnQuantity");
                row.CreateCell(36).SetCellValue("OnTransferQuantity");
                row.CreateCell(37).SetCellValue("SuggestedOrderQuantity");
                row.CreateCell(38).SetCellValue("RequisitionedQuantity");
                row.CreateCell(39).SetCellValue("AverageUsage");
                row.CreateCell(40).SetCellValue("Turns");
                row.CreateCell(41).SetCellValue("WeightPerPiece");
                row.CreateCell(42).SetCellValue("ItemUniqueNumber");
                row.CreateCell(43).SetCellValue("IsTransfer");
                row.CreateCell(44).SetCellValue("IsPurchase");
                row.CreateCell(45).SetCellValue("InventryLocation");
                row.CreateCell(46).SetCellValue("InventoryClassification");
                row.CreateCell(47).SetCellValue("SerialNumberTracking");
                row.CreateCell(48).SetCellValue("LotNumberTracking");
                row.CreateCell(49).SetCellValue("DateCodeTracking");
                row.CreateCell(50).SetCellValue("ItemType");
                row.CreateCell(51).SetCellValue("ImagePath");
                row.CreateCell(52).SetCellValue("UDF1");
                row.CreateCell(53).SetCellValue("UDF2");
                row.CreateCell(54).SetCellValue("UDF3");
                row.CreateCell(55).SetCellValue("UDF4");
                row.CreateCell(56).SetCellValue("UDF5");
                row.CreateCell(57).SetCellValue("IsLotSerialExpiryCost");
                row.CreateCell(58).SetCellValue("IsItemLevelMinMaxQtyRequired");
                row.CreateCell(59).SetCellValue("TrendingSetting");
                row.CreateCell(60).SetCellValue("EnforceDefaultPullQuantity");
                row.CreateCell(61).SetCellValue("EnforceDefaultReorderQuantity");
                row.CreateCell(62).SetCellValue("IsAutoInventoryClassification");
                row.CreateCell(63).SetCellValue("IsBuildBreak");
                row.CreateCell(64).SetCellValue("IsPackslipMandatoryAtReceive");
                row.CreateCell(65).SetCellValue("ItemImageExternalURL");
                row.CreateCell(66).SetCellValue("ItemDocExternalURL");
                row.CreateCell(67).SetCellValue("OutTransferQuantity");
                row.CreateCell(68).SetCellValue("OnOrderInTransitQuantity");
                row.CreateCell(69).SetCellValue("ItemLink2ExternalURL");
                //------------E7165nd--------------------

                //-------------68Set Row Value---------------------------
                int RowId = 1;
                foreach (ItemMasterDTO item in lstItemMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.ItemNumber);
                    DRow.CreateCell(2).SetCellValue(item.BinNumber);
                    DRow.CreateCell(3).SetCellValue(item.ManufacturerName);
                    DRow.CreateCell(4).SetCellValue(item.ManufacturerNumber);
                    DRow.CreateCell(5).SetCellValue(item.SupplierName);
                    DRow.CreateCell(6).SetCellValue(item.SupplierPartNo);
                    DRow.CreateCell(7).SetCellValue(item.BlanketOrderNumber);
                    DRow.CreateCell(8).SetCellValue(item.UPC);
                    DRow.CreateCell(9).SetCellValue(item.UNSPSC);
                    DRow.CreateCell(10).SetCellValue(item.OnHandQuantity ?? 0);
                    DRow.CreateCell(11).SetCellValue(item.CriticalQuantity ?? 0);
                    DRow.CreateCell(12).SetCellValue(item.MinimumQuantity.GetValueOrDefault(0));
                    DRow.CreateCell(13).SetCellValue(item.MaximumQuantity.GetValueOrDefault(0));
                    DRow.CreateCell(14).SetCellValue(item.Description);
                    DRow.CreateCell(15).SetCellValue(item.LongDescription);
                    DRow.CreateCell(16).SetCellValue(item.CategoryName);
                    DRow.CreateCell(17).SetCellValue(item.CostUOMName);
                    DRow.CreateCell(18).SetCellValue(item.DefaultReorderQuantity ?? 0);
                    DRow.CreateCell(19).SetCellValue(item.DefaultPullQuantity ?? 0);
                    DRow.CreateCell(20).SetCellValue(Convert.ToDouble(item.Cost));// == null ? 0 : Convert.ToDouble(item.Cost));
                    DRow.CreateCell(21).SetCellValue(Convert.ToDouble(item.Markup));// == null ? 0 : Convert.ToDouble(item.Markup));
                    DRow.CreateCell(22).SetCellValue(Convert.ToDouble(item.SellPrice));// == null ? 0 : Convert.ToDouble(item.SellPrice));
                    DRow.CreateCell(23).SetCellValue(Convert.ToDouble(item.ExtendedCost));// == null ? 0 : Convert.ToDouble(item.ExtendedCost));
                    DRow.CreateCell(24).SetCellValue(Convert.ToDouble(item.AverageCost));// == null ? 0 : Convert.ToDouble(item.AverageCost));
                    DRow.CreateCell(25).SetCellValue(Convert.ToDouble(item.PerItemCost));// == null ? 0 : Convert.ToDouble(item.AverageCost));
                    DRow.CreateCell(26).SetCellValue(item.LeadTimeInDays ?? 0);
                    DRow.CreateCell(27).SetCellValue(item.Link1);
                    DRow.CreateCell(28).SetCellValue(item.Link2);
                    DRow.CreateCell(29).SetCellValue(item.Trend);
                    DRow.CreateCell(30).SetCellValue(item.Taxable);
                    DRow.CreateCell(31).SetCellValue(item.Consignment);
                    DRow.CreateCell(32).SetCellValue(item.StagedQuantity ?? 0);
                    DRow.CreateCell(33).SetCellValue(item.InTransitquantity ?? 0);
                    DRow.CreateCell(34).SetCellValue(item.OnOrderQuantity ?? 0);
                    DRow.CreateCell(35).SetCellValue(item.OnReturnQuantity ?? 0);
                    DRow.CreateCell(36).SetCellValue(item.OnTransferQuantity ?? 0);
                    DRow.CreateCell(37).SetCellValue(item.SuggestedOrderQuantity ?? 0);
                    DRow.CreateCell(38).SetCellValue(item.RequisitionedQuantity ?? 0);
                    DRow.CreateCell(39).SetCellValue(item.AverageUsage ?? 0);
                    DRow.CreateCell(40).SetCellValue(item.Turns ?? 0);
                    DRow.CreateCell(41).SetCellValue(item.WeightPerPiece ?? 0);
                    DRow.CreateCell(42).SetCellValue(item.ItemUniqueNumber);
                    DRow.CreateCell(43).SetCellValue(item.IsTransfer);
                    DRow.CreateCell(44).SetCellValue(item.IsPurchase);
                    DRow.CreateCell(45).SetCellValue(item.DefaultLocationName);
                    DRow.CreateCell(46).SetCellValue(item.InventoryClassificationName);
                    DRow.CreateCell(47).SetCellValue(item.SerialNumberTracking);
                    DRow.CreateCell(48).SetCellValue(item.LotNumberTracking);
                    DRow.CreateCell(49).SetCellValue(item.DateCodeTracking);
                    DRow.CreateCell(50).SetCellValue(GetItemType(item.ItemType));
                    DRow.CreateCell(51).SetCellValue(item.ImagePath);
                    DRow.CreateCell(52).SetCellValue(item.UDF1);
                    DRow.CreateCell(53).SetCellValue(item.UDF2);
                    DRow.CreateCell(54).SetCellValue(item.UDF3);
                    DRow.CreateCell(55).SetCellValue(item.UDF4);
                    DRow.CreateCell(56).SetCellValue(item.UDF5);
                    DRow.CreateCell(57).SetCellValue(item.IsLotSerialExpiryCost);
                    DRow.CreateCell(58).SetCellValue(item.IsItemLevelMinMaxQtyRequired.ToString());
                    DRow.CreateCell(59).SetCellValue(GetTrendingSetting(item.TrendingSetting));
                    DRow.CreateCell(60).SetCellValue(item.PullQtyScanOverride);
                    DRow.CreateCell(61).SetCellValue(item.IsEnforceDefaultReorderQuantity ?? false);
                    DRow.CreateCell(62).SetCellValue(item.IsAutoInventoryClassification);
                    DRow.CreateCell(63).SetCellValue(item.IsBuildBreak ?? false);
                    DRow.CreateCell(64).SetCellValue(item.IsPackslipMandatoryAtReceive.ToString());
                    DRow.CreateCell(65).SetCellValue(item.ItemImageExternalURL);
                    DRow.CreateCell(66).SetCellValue(item.ItemDocExternalURL);
                    DRow.CreateCell(67).SetCellValue(item.OutTransferQuantity ?? 0);
                    DRow.CreateCell(68).SetCellValue(item.OnOrderInTransitQuantity ?? 0);
                    DRow.CreateCell(69).SetCellValue(item.ItemLink2ExternalURL);
                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }


        public string MinMaxTuningList(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();
            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            //string[] arrid = ids.Split(',');
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



            if (DataFromDB != null && DataFromDB.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);

                row.CreateCell(0).SetCellValue(ResInventoryAnalysis.MinAnalysis);
                row.CreateCell(1).SetCellValue(ResInventoryAnalysis.MaxAnalysis);
                row.CreateCell(2).SetCellValue(ResInventoryAnalysis.ItemNumber);
                row.CreateCell(3).SetCellValue(ResInventoryAnalysis.IsActive);
                row.CreateCell(4).SetCellValue(ResInventoryAnalysis.DefaultReorderQuantity);
                row.CreateCell(5).SetCellValue(ResInventoryAnalysis.CostUOMValue);
                row.CreateCell(6).SetCellValue(ResInventoryAnalysis.DateCreated);
                row.CreateCell(7).SetCellValue(ResInventoryAnalysis.IsitemLevelMinMax);
                row.CreateCell(8).SetCellValue(ResInventoryAnalysis.Description);
                row.CreateCell(9).SetCellValue(ResInventoryAnalysis.InventoryClassification);
                row.CreateCell(10).SetCellValue(ResInventoryAnalysis.Category);
                row.CreateCell(11).SetCellValue(ResInventoryAnalysis.SupplierName);
                row.CreateCell(12).SetCellValue(ResInventoryAnalysis.SupplierPartNo);
                row.CreateCell(13).SetCellValue(ResInventoryAnalysis.Manufacturer);
                row.CreateCell(14).SetCellValue(ResInventoryAnalysis.ManufacturerNumber);
                row.CreateCell(15).SetCellValue(ResInventoryAnalysis.Location);
                row.CreateCell(16).SetCellValue(ResInventoryAnalysis.AvailableQty);
                row.CreateCell(17).SetCellValue(ResInventoryAnalysis.InventoryValue);
                row.CreateCell(18).SetCellValue(ResInventoryAnalysis.AverageCost);
                row.CreateCell(19).SetCellValue(ResInventoryAnalysis.PeriodPullValueUsage);
                row.CreateCell(20).SetCellValue(ResInventoryAnalysis.AvgDailyPullValueUsage);
                row.CreateCell(21).SetCellValue(ResInventoryAnalysis.PeriodPullUsage);
                row.CreateCell(22).SetCellValue(ResInventoryAnalysis.AvgDailyPullUsage);
                row.CreateCell(23).SetCellValue(ResInventoryAnalysis.QtyUntilOrder);
                row.CreateCell(24).SetCellValue(ResInventoryAnalysis.NoOfDaysUntilOrder);
                row.CreateCell(25).SetCellValue(ResInventoryAnalysis.LeadTimeInDays);
                row.CreateCell(26).SetCellValue(ResInventoryAnalysis.DateofOrder);
                row.CreateCell(27).SetCellValue(ResInventoryAnalysis.DemandPlanningQtyToOrder);
                row.CreateCell(28).SetCellValue(ResInventoryAnalysis.PeriodOrdersUsage);
                row.CreateCell(29).SetCellValue(ResInventoryAnalysis.AvgDailyOrdersUsage);
                row.CreateCell(30).SetCellValue(ResInventoryAnalysis.PullValueTurns);
                row.CreateCell(31).SetCellValue(ResInventoryAnalysis.PullTurns);
                row.CreateCell(32).SetCellValue(ResInventoryAnalysis.OrderTurns);
                row.CreateCell(33).SetCellValue(ResInventoryAnalysis.Critical);
                row.CreateCell(34).SetCellValue(ResInventoryAnalysis.CurrentMinimum);
                row.CreateCell(35).SetCellValue(ResInventoryAnalysis.CalculatedMinimum);
                row.CreateCell(36).SetCellValue(HttpUtility.HtmlDecode(ResInventoryAnalysis.AbsvaluediffcurrcalcMinimum));
                row.CreateCell(37).SetCellValue(ResInventoryAnalysis.TrialCalculatedMinimum);
                row.CreateCell(38).SetCellValue(HttpUtility.HtmlDecode(ResInventoryAnalysis.AbsValDifCurrCalcMinimum));
                row.CreateCell(39).SetCellValue(ResInventoryAnalysis.AutocurrentminPercentage);
                row.CreateCell(40).SetCellValue(ResInventoryAnalysis.OptimizedInvValueUsesQOHofAvgCalcdMinMax);
                row.CreateCell(41).SetCellValue(ResInventoryAnalysis.OptimizedInvValueChange);
                row.CreateCell(42).SetCellValue(ResInventoryAnalysis.TrialCalcInvValueUsesQOHofAvgTrialMinMax);
                row.CreateCell(43).SetCellValue(ResInventoryAnalysis.TrialInvValueChange);
                row.CreateCell(44).SetCellValue(ResInventoryAnalysis.CurrentMaximum);
                row.CreateCell(45).SetCellValue(ResInventoryAnalysis.CalculatedMaximum);
                row.CreateCell(46).SetCellValue(HttpUtility.HtmlDecode(ResInventoryAnalysis.AbsvaluediffcurrcalcMaximum));
                row.CreateCell(47).SetCellValue(ResInventoryAnalysis.TrialCalculatedMaximum);
                row.CreateCell(48).SetCellValue(HttpUtility.HtmlDecode(ResInventoryAnalysis.AbsValDifCurrCalcMaximum));
                row.CreateCell(49).SetCellValue(ResInventoryAnalysis.AutocurrentmaxPercentage);

                row.CreateCell(50).SetCellValue(eTurns.DTO.ResItemMaster.UDF1);
                row.CreateCell(51).SetCellValue(eTurns.DTO.ResItemMaster.UDF2);
                row.CreateCell(52).SetCellValue(eTurns.DTO.ResItemMaster.UDF3);
                row.CreateCell(53).SetCellValue(eTurns.DTO.ResItemMaster.UDF4);
                row.CreateCell(54).SetCellValue(eTurns.DTO.ResItemMaster.UDF5);
                row.CreateCell(55).SetCellValue(eTurns.DTO.ResItemMaster.UDF6);
                row.CreateCell(56).SetCellValue(eTurns.DTO.ResItemMaster.UDF7);
                row.CreateCell(57).SetCellValue(eTurns.DTO.ResItemMaster.UDF8);
                row.CreateCell(58).SetCellValue(eTurns.DTO.ResItemMaster.UDF9);
                row.CreateCell(59).SetCellValue(eTurns.DTO.ResItemMaster.UDF10);

                //------------E7165nd--------------------

                //-------------68Set Row Value---------------------------
                int RowId = 1;
                foreach (MinMaxDataTableInfo rec in DataFromDB)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    /*
                    DRow.CreateCell(0).SetCellValue(rec.MinAnalysis);
                    DRow.CreateCell(1).SetCellValue(rec.MaxAnalysis);
                    DRow.CreateCell(2).SetCellValue(rec.ItemNumber);
                    DRow.CreateCell(3).SetCellValue(rec.IsItemLevelMinMaxQtyRequired ?? false);
                    DRow.CreateCell(4).SetCellValue(rec.Description);
                    DRow.CreateCell(5).SetCellValue(rec.InventoryClassification);
                    DRow.CreateCell(6).SetCellValue(rec.Category);
                    DRow.CreateCell(7).SetCellValue(rec.SupplierName);
                    DRow.CreateCell(8).SetCellValue(rec.SupplierPartNo);
                    DRow.CreateCell(9).SetCellValue(rec.Manufacturer);
                    DRow.CreateCell(10).SetCellValue(rec.ManufacturerNumber);
                    DRow.CreateCell(11).SetCellValue(rec.BinNumber);
                    DRow.CreateCell(12).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.OnHandQuantity, numberDecimalDigits)));
                    DRow.CreateCell(13).SetCellValue(string.Format("{0:N" + currencyDecimalDigits + "}", Math.Round(rec.ItemInventoryValue, currencyDecimalDigits)));

                    DRow.CreateCell(14).SetCellValue(string.Format("{0:N" + currencyDecimalDigits + "}", Math.Round(rec.AverageCost, currencyDecimalDigits)));
                    DRow.CreateCell(15).SetCellValue(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.PullCost, turnUsageLimit)));
                    DRow.CreateCell(16).SetCellValue(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.AvgDailyPullValueUsage, turnUsageLimit)));
                    DRow.CreateCell(17).SetCellValue(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.PullQuantity, turnUsageLimit)));
                    DRow.CreateCell(18).SetCellValue(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.AvgDailyPullUsage, turnUsageLimit)));
                    DRow.CreateCell(19).SetCellValue(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.QtyUntilOrder, turnUsageLimit)));
                    DRow.CreateCell(20).SetCellValue(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.NoOfDaysUntilOrder, turnUsageLimit)));
                    DRow.CreateCell(21).SetCellValue(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.DemandPlanningQtyToOrder, turnUsageLimit)));
                    DRow.CreateCell(22).SetCellValue(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.OrderedQuantity, turnUsageLimit)));
                    DRow.CreateCell(23).SetCellValue(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.AvgDailyOrderUsage, turnUsageLimit)));
                    DRow.CreateCell(24).SetCellValue(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.PullValueTurn, turnUsageLimit)));
                    DRow.CreateCell(25).SetCellValue(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.PullTurn, turnUsageLimit)));
                    DRow.CreateCell(26).SetCellValue(string.Format("{0:N" + turnUsageLimit + "}", Math.Round(rec.OrderTurn, turnUsageLimit)));
                    DRow.CreateCell(27).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.CriticalQuantity, numberDecimalDigits)));
                    DRow.CreateCell(28).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.MinimumQuantity, numberDecimalDigits)));
                    DRow.CreateCell(29).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.CalulatedMinimum, numberDecimalDigits)));
                    DRow.CreateCell(30).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AbsoluteMinPerCent, numberDecimalDigits)));
                    DRow.CreateCell(31).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AutoCurrentMin, numberDecimalDigits)));
                    DRow.CreateCell(32).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AbsValDifCurrCalcMinimum, numberDecimalDigits)));
                    DRow.CreateCell(33).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AutoCurrentMinPercent, numberDecimalDigits)));
                    DRow.CreateCell(34).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.MaximumQuantity, numberDecimalDigits)));
                    DRow.CreateCell(35).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.CalulatedMaximum, numberDecimalDigits)));
                    DRow.CreateCell(36).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AbsoluteMAXPerCent, numberDecimalDigits)));
                    DRow.CreateCell(37).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AutoCurrentMax, numberDecimalDigits)));
                    DRow.CreateCell(38).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AbsValDifCurrCalcMax, numberDecimalDigits)));
                    DRow.CreateCell(39).SetCellValue(string.Format("{0:N" + numberDecimalDigits + "}", Math.Round(rec.AutoCurrentMaxPercent, numberDecimalDigits)));
                    DRow.CreateCell(40).SetCellValue(rec.DefaultReorderQuantity);
                    DRow.CreateCell(41).SetCellValue(rec.CostUOMValue);
                    DRow.CreateCell(42).SetCellValue(rec.IsActive);
                    DRow.CreateCell(43).SetCellValue(rec.DateCreated);
                    DRow.CreateCell(44).SetCellValue(rec.UDF1);
                    DRow.CreateCell(45).SetCellValue(rec.UDF2);
                    DRow.CreateCell(46).SetCellValue(rec.UDF3);
                    DRow.CreateCell(47).SetCellValue(rec.UDF4);
                    DRow.CreateCell(48).SetCellValue(rec.UDF5);
                    DRow.CreateCell(49).SetCellValue(rec.UDF6);
                    DRow.CreateCell(50).SetCellValue(rec.UDF7);
                    DRow.CreateCell(51).SetCellValue(rec.UDF8);
                    DRow.CreateCell(52).SetCellValue(rec.UDF9);
                    DRow.CreateCell(53).SetCellValue(rec.UDF10);
                    */

                    DRow.CreateCell(0).SetCellValue(rec.MinAnalysis);
                    DRow.CreateCell(1).SetCellValue(rec.MaxAnalysis);
                    DRow.CreateCell(2).SetCellValue(rec.ItemNumber);
                    DRow.CreateCell(3).SetCellValue(rec.IsActive);
                    DRow.CreateCell(4).SetCellValue(rec.DefaultReorderQuantity);
                    DRow.CreateCell(5).SetCellValue(rec.CostUOMValue);
                    DRow.CreateCell(6).SetCellValue(rec.DateCreated);
                    DRow.CreateCell(7).SetCellValue((rec.IsItemLevelMinMaxQtyRequired ?? false) ? "Yes" : "No");
                    DRow.CreateCell(8).SetCellValue(rec.Description);
                    DRow.CreateCell(9).SetCellValue(rec.InventoryClassification);
                    DRow.CreateCell(10).SetCellValue(rec.Category);
                    DRow.CreateCell(11).SetCellValue(rec.SupplierName);
                    DRow.CreateCell(12).SetCellValue(rec.SupplierPartNo);
                    DRow.CreateCell(13).SetCellValue(rec.Manufacturer);
                    DRow.CreateCell(14).SetCellValue(rec.ManufacturerNumber);
                    DRow.CreateCell(15).SetCellValue(rec.BinNumber);
                    DRow.CreateCell(16).SetCellValue(rec.OnHandQuantity);
                    DRow.CreateCell(17).SetCellValue(rec.ItemInventoryValue);
                    DRow.CreateCell(18).SetCellValue(rec.AverageCost);
                    DRow.CreateCell(19).SetCellValue(rec.PullCost+rec.TransferCost);
                    DRow.CreateCell(20).SetCellValue(rec.AvgDailyPullValueUsage);
                    DRow.CreateCell(21).SetCellValue(rec.PullQuantity + rec.TransferQuantity);
                    DRow.CreateCell(22).SetCellValue(rec.AvgDailyPullUsage);
                    DRow.CreateCell(23).SetCellValue(rec.QtyUntilOrder);
                    DRow.CreateCell(24).SetCellValue(rec.NoOfDaysUntilOrder);
                    DRow.CreateCell(25).SetCellValue(rec.LeadTimeInDays);
                    DRow.CreateCell(26).SetCellValue(rec.DateofOrder);
                    DRow.CreateCell(27).SetCellValue(rec.DemandPlanningQtyToOrder);
                    DRow.CreateCell(28).SetCellValue(rec.OrderedQuantity);
                    DRow.CreateCell(29).SetCellValue(rec.AvgDailyOrderUsage);
                    DRow.CreateCell(30).SetCellValue(rec.PullValueTurn);
                    DRow.CreateCell(31).SetCellValue(rec.PullTurn);
                    DRow.CreateCell(32).SetCellValue(rec.OrderTurn);
                    DRow.CreateCell(33).SetCellValue(rec.CriticalQuantity);
                    DRow.CreateCell(34).SetCellValue(rec.MinimumQuantity);
                    DRow.CreateCell(35).SetCellValue(rec.CalulatedMinimum);
                    DRow.CreateCell(36).SetCellValue(rec.AbsoluteMinPerCent);
                    DRow.CreateCell(37).SetCellValue(rec.AutoCurrentMin);
                    DRow.CreateCell(38).SetCellValue(rec.AbsValDifCurrCalcMinimum);
                    DRow.CreateCell(39).SetCellValue(rec.AutoCurrentMinPercent);
                    DRow.CreateCell(40).SetCellValue(rec.OptimizedInvValueUsesQOHofAvgCalcdMinMax);
                    DRow.CreateCell(41).SetCellValue(rec.OptimizedInvValueChange);
                    DRow.CreateCell(42).SetCellValue(rec.TrialCalcInvValueUsesQOHofAvgTrialMinMax);
                    DRow.CreateCell(43).SetCellValue(rec.TrialInvValueChange);
                    DRow.CreateCell(44).SetCellValue(rec.MaximumQuantity);
                    DRow.CreateCell(45).SetCellValue(rec.CalulatedMaximum);
                    DRow.CreateCell(46).SetCellValue(rec.AbsoluteMAXPerCent);
                    DRow.CreateCell(47).SetCellValue(rec.AutoCurrentMax);
                    DRow.CreateCell(48).SetCellValue(rec.AbsValDifCurrCalcMax);
                    DRow.CreateCell(49).SetCellValue(rec.AutoCurrentMaxPercent);
                    DRow.CreateCell(50).SetCellValue(rec.UDF1);
                    DRow.CreateCell(51).SetCellValue(rec.UDF2);
                    DRow.CreateCell(52).SetCellValue(rec.UDF3);
                    DRow.CreateCell(53).SetCellValue(rec.UDF4);
                    DRow.CreateCell(54).SetCellValue(rec.UDF5);
                    DRow.CreateCell(55).SetCellValue(rec.UDF6);
                    DRow.CreateCell(56).SetCellValue(rec.UDF7);
                    DRow.CreateCell(57).SetCellValue(rec.UDF8);
                    DRow.CreateCell(58).SetCellValue(rec.UDF9);
                    DRow.CreateCell(59).SetCellValue(rec.UDF10);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }


        public string TuningList(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();
            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            //string[] arrid = ids.Split(',');
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


            if (DataFromDB != null && DataFromDB.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);

                row.CreateCell(0).SetCellValue(ResInventoryAnalysis.ItemNumber);
                row.CreateCell(1).SetCellValue(ResInventoryAnalysis.IsitemLevelMinMax);
                row.CreateCell(2).SetCellValue(ResInventoryAnalysis.Description);
                row.CreateCell(3).SetCellValue(ResInventoryAnalysis.InventoryClassification);
                row.CreateCell(4).SetCellValue(ResInventoryAnalysis.Category);
                row.CreateCell(5).SetCellValue(ResInventoryAnalysis.SupplierName);
                row.CreateCell(6).SetCellValue(ResInventoryAnalysis.Location);
                row.CreateCell(7).SetCellValue(ResInventoryAnalysis.AvailableQty);
                row.CreateCell(8).SetCellValue(ResInventoryAnalysis.InventoryValue);
                row.CreateCell(9).SetCellValue(ResInventoryAnalysis.AverageCost);
                row.CreateCell(10).SetCellValue(ResInventoryAnalysis.PeriodPullValueUsage);
                row.CreateCell(11).SetCellValue(ResInventoryAnalysis.AvgDailyPullValueUsage);
                row.CreateCell(12).SetCellValue(ResInventoryAnalysis.PeriodPullUsage);
                row.CreateCell(13).SetCellValue(ResInventoryAnalysis.AvgDailyPullUsage);
                row.CreateCell(14).SetCellValue(ResInventoryAnalysis.PeriodOrdersUsage);
                row.CreateCell(15).SetCellValue(ResInventoryAnalysis.AvgDailyOrdersUsage);
                row.CreateCell(16).SetCellValue(ResInventoryAnalysis.PullValueTurns);
                row.CreateCell(17).SetCellValue(ResInventoryAnalysis.PullTurns);
                row.CreateCell(18).SetCellValue(ResInventoryAnalysis.OrderTurns);

                //------------E7165nd--------------------

                //-------------68Set Row Value---------------------------
                int RowId = 1;
                foreach (TurnsDataTableInfo rec in DataFromDB)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(rec.ItemNumber);
                    DRow.CreateCell(1).SetCellValue(rec.IsItemLevelMinMaxQtyRequired ?? false);
                    DRow.CreateCell(2).SetCellValue(rec.Description);
                    DRow.CreateCell(3).SetCellValue(rec.InventoryClassification);
                    DRow.CreateCell(4).SetCellValue(rec.Category);
                    DRow.CreateCell(5).SetCellValue(rec.SupplierName);
                    DRow.CreateCell(6).SetCellValue(rec.BinNumber);
                    DRow.CreateCell(7).SetCellValue(rec.OnHandQuantity);
                    DRow.CreateCell(8).SetCellValue(rec.ItemInventoryValue);
                    DRow.CreateCell(9).SetCellValue(rec.AverageCost);
                    DRow.CreateCell(10).SetCellValue(rec.PullCost);
                    DRow.CreateCell(11).SetCellValue(rec.AvgDailyPullValueUsage);
                    DRow.CreateCell(12).SetCellValue(rec.PullQuantity);
                    DRow.CreateCell(13).SetCellValue(rec.AvgDailyPullUsage);
                    DRow.CreateCell(14).SetCellValue(rec.OrderedQuantity);
                    DRow.CreateCell(15).SetCellValue(rec.AvgDailyOrderUsage);
                    DRow.CreateCell(16).SetCellValue(rec.PullValueTurn);
                    DRow.CreateCell(17).SetCellValue(rec.PullTurn);
                    DRow.CreateCell(18).SetCellValue(rec.OrderTurn);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
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
        public string QuickListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            QuickListDAL obj = new QuickListDAL(SessionHelper.EnterPriseDBName);
            List<QuickListLineItemDetailDTO> lstQuickListLineItemDetailDTO = obj.GetAllQuicklistWiseLineItem(SessionHelper.CompanyID, SessionHelper.RoomID, false, false, ids, SessionHelper.UserSupplierIds).OrderBy(SortNameString).ToList();



            if (lstQuickListLineItemDetailDTO != null && lstQuickListLineItemDetailDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* Name");
                row.CreateCell(2).SetCellValue("* Type");
                row.CreateCell(3).SetCellValue("Comment");
                row.CreateCell(4).SetCellValue("* Item Number");
                row.CreateCell(5).SetCellValue("Bin Number");
                row.CreateCell(6).SetCellValue("Quantity");
                row.CreateCell(7).SetCellValue("UDF1");
                row.CreateCell(8).SetCellValue("UDF2");
                row.CreateCell(9).SetCellValue("UDF3");
                row.CreateCell(10).SetCellValue("UDF4");
                row.CreateCell(11).SetCellValue("UDF5");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (QuickListLineItemDetailDTO item in lstQuickListLineItemDetailDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.Name);
                    DRow.CreateCell(2).SetCellValue(((QuickListType)item.Type).ToString());
                    DRow.CreateCell(3).SetCellValue(item.Comment);
                    DRow.CreateCell(4).SetCellValue(item.ItemNumber);
                    DRow.CreateCell(5).SetCellValue(item.BinNumber);
                    DRow.CreateCell(6).SetCellValue(item.Quantity);
                    DRow.CreateCell(7).SetCellValue(item.UDF1);
                    DRow.CreateCell(8).SetCellValue(item.UDF2);
                    DRow.CreateCell(9).SetCellValue(item.UDF3);
                    DRow.CreateCell(10).SetCellValue(item.UDF4);
                    DRow.CreateCell(11).SetCellValue(item.UDF5);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string EnterpriseUserList(string Filepath, string Id)
        {

            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = Id.Split(',');
            EmailUserConfigurationDAL objEmailUserConfigurationDAL = new EmailUserConfigurationDAL(SessionHelper.EnterPriseDBName);
            List<EnterpriseUserDetailDTO> lstEnterpriseUserDetailDTO = new List<EnterpriseUserDetailDTO>();
            lstEnterpriseUserDetailDTO = objEmailUserConfigurationDAL.GetEnterpriseUserList(Id);




            //if (lstEnterpriseUserDetailDTO != null && lstEnterpriseUserDetailDTO.Count > 0)
            //{
            //------------Create Header--------------------
            IRow row = sheet1.CreateRow(0);
            row.CreateCell(0).SetCellValue("User name");
            row.CreateCell(1).SetCellValue("Company");
            row.CreateCell(2).SetCellValue("Stockroom");
            row.CreateCell(3).SetCellValue("Phone #");
            row.CreateCell(4).SetCellValue("email");
            row.CreateCell(5).SetCellValue("Date Created");

            //------------End--------------------

            //-------------Set Row Value---------------------------
            int RowId = 1;
            foreach (EnterpriseUserDetailDTO item in lstEnterpriseUserDetailDTO)
            {
                IRow DRow = sheet1.CreateRow(RowId);

                DRow.CreateCell(0).SetCellValue(item.UserName);
                DRow.CreateCell(1).SetCellValue(item.Company);
                DRow.CreateCell(2).SetCellValue(item.Stockroom);
                DRow.CreateCell(3).SetCellValue(item.Phone);
                DRow.CreateCell(4).SetCellValue(item.email);
                DRow.CreateCell(5).SetCellValue(Convert.ToDateTime(item.CreatedOn).ToString());

                RowId += 1;
                //-------------End--------------------------
            }

            //}
            string filename = DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy HH:mm:ss").Replace("/", "").Replace(":", "") + "_UserList.xls";
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, "");
            return filename;
        }
        public string BOMItemMasterListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            BOMItemMasterDAL obj = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);

            IEnumerable<BOMItemDTO> DataFromDB = obj.CompanyBOMItems(SessionHelper.CompanyID, false, false).OrderBy(SortNameString);

            List<ItemMasterDTO> lstItemMasterDTO = new List<ItemMasterDTO>();

            if (!string.IsNullOrEmpty(ids))
            {
                lstItemMasterDTO = (from u in DataFromDB
                                    where arrid.Contains(u.GUID.ToString())
                                    select new ItemMasterDTO
                                    {
                                        ID = u.ID,
                                        ItemNumber = u.ItemNumber,
                                        ManufacturerID = u.ManufacturerID,
                                        ManufacturerNumber = u.ManufacturerNumber,
                                        ManufacturerName = u.ManufacturerName,
                                        SupplierID = u.SupplierID,
                                        SupplierPartNo = u.SupplierPartNo,
                                        SupplierName = u.SupplierName,
                                        UPC = u.UPC,
                                        UNSPSC = u.UNSPSC,
                                        Description = u.Description,
                                        CategoryID = u.CategoryID,
                                        GLAccountID = u.GLAccountID,
                                        UOMID = u.UOMID,
                                        LeadTimeInDays = u.LeadTimeInDays,
                                        Taxable = u.Taxable,
                                        Consignment = u.Consignment,
                                        ItemUniqueNumber = u.ItemUniqueNumber,
                                        IsPurchase = u.IsPurchase,
                                        IsTransfer = u.IsTransfer,
                                        DefaultLocation = u.DefaultLocation,
                                        DefaultLocationName = u.DefaultLocationName,
                                        InventoryClassification = u.InventoryClassification,
                                        InventoryClassificationName = u.InventoryClassificationName,
                                        SerialNumberTracking = u.SerialNumberTracking,
                                        LotNumberTracking = u.LotNumberTracking,
                                        DateCodeTracking = u.DateCodeTracking,
                                        ItemType = u.ItemType,
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
                                        BondedInventory = u.BondedInventory,
                                        ItemLocations = objLocationDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, u.GUID, null, "ID ASC").ToList()

                                    }).ToList();
            }




            if (lstItemMasterDTO != null && lstItemMasterDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* ItemNumber");
                row.CreateCell(2).SetCellValue("Manufacturer");
                row.CreateCell(3).SetCellValue("ManufacturerNumber");
                row.CreateCell(4).SetCellValue("* SupplierName");
                row.CreateCell(5).SetCellValue("* SupplierPartNo");
                row.CreateCell(6).SetCellValue("UPC");
                row.CreateCell(7).SetCellValue("UNSPSC");
                row.CreateCell(8).SetCellValue("Description");
                row.CreateCell(9).SetCellValue("CategoryName");
                row.CreateCell(10).SetCellValue("GLAccount");
                row.CreateCell(11).SetCellValue("* UOM");
                row.CreateCell(12).SetCellValue("LeadTimeInDays");
                row.CreateCell(13).SetCellValue("* Taxable");
                row.CreateCell(14).SetCellValue("* Consignment");
                row.CreateCell(15).SetCellValue("ItemUniqueNumber");
                row.CreateCell(16).SetCellValue("IsTransfer");
                row.CreateCell(17).SetCellValue("IsPurchase");
                row.CreateCell(18).SetCellValue("InventryLocation");
                row.CreateCell(19).SetCellValue("InventoryClassification");
                row.CreateCell(20).SetCellValue("* SerialNumberTracking");
                row.CreateCell(21).SetCellValue("* LotNumberTracking");
                row.CreateCell(22).SetCellValue("* DateCodeTracking");
                row.CreateCell(23).SetCellValue("* ItemType");
                row.CreateCell(24).SetCellValue("UDF1");
                row.CreateCell(25).SetCellValue("UDF2");
                row.CreateCell(26).SetCellValue("UDF3");
                row.CreateCell(27).SetCellValue("UDF4");
                row.CreateCell(28).SetCellValue("UDF5");
                row.CreateCell(29).SetCellValue("IsLotSerialExpiryCost");


                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ItemMasterDTO item in lstItemMasterDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.ItemNumber);
                    DRow.CreateCell(2).SetCellValue(item.ManufacturerName);
                    DRow.CreateCell(3).SetCellValue(item.ManufacturerNumber);
                    DRow.CreateCell(4).SetCellValue(item.SupplierName);
                    DRow.CreateCell(5).SetCellValue(item.SupplierPartNo);
                    DRow.CreateCell(6).SetCellValue(item.UPC);
                    DRow.CreateCell(7).SetCellValue(item.UNSPSC);
                    DRow.CreateCell(8).SetCellValue(item.Description);
                    DRow.CreateCell(9).SetCellValue(item.CategoryName);
                    DRow.CreateCell(10).SetCellValue(item.GLAccount);
                    DRow.CreateCell(11).SetCellValue(item.Unit);
                    DRow.CreateCell(12).SetCellValue(item.LeadTimeInDays ?? 0);
                    DRow.CreateCell(13).SetCellValue(item.Taxable);
                    DRow.CreateCell(14).SetCellValue(item.Consignment);
                    DRow.CreateCell(15).SetCellValue(item.ItemUniqueNumber);
                    DRow.CreateCell(16).SetCellValue(item.IsTransfer);
                    DRow.CreateCell(17).SetCellValue(item.IsPurchase);
                    DRow.CreateCell(18).SetCellValue(item.DefaultLocationName);
                    DRow.CreateCell(19).SetCellValue(item.InventoryClassificationName);
                    DRow.CreateCell(20).SetCellValue(item.SerialNumberTracking);
                    DRow.CreateCell(21).SetCellValue(item.LotNumberTracking);
                    DRow.CreateCell(22).SetCellValue(item.DateCodeTracking);
                    DRow.CreateCell(23).SetCellValue(GetItemType(item.ItemType));
                    DRow.CreateCell(24).SetCellValue(item.UDF1);
                    DRow.CreateCell(25).SetCellValue(item.UDF2);
                    DRow.CreateCell(26).SetCellValue(item.UDF3);
                    DRow.CreateCell(27).SetCellValue(item.UDF4);
                    DRow.CreateCell(28).SetCellValue(item.UDF5);
                    DRow.CreateCell(29).SetCellValue(item.IsLotSerialExpiryCost);



                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
        public string WorkOrderCountExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDAL objLocationDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            List<WorkOrderDTO> lstWOMain = obj.WorkOrderExport(SessionHelper.RoomID, SessionHelper.CompanyID, ids);//.OrderBy(SortNameString).ToList();

            IRow row = sheet1.CreateRow(0);
            row.CreateCell(0).SetCellValue("* WOName");
            row.CreateCell(0).SetCellValue("ReleaseNumber");
            row.CreateCell(1).SetCellValue("Description");
            row.CreateCell(2).SetCellValue("Technician");
            row.CreateCell(3).SetCellValue("Customer");
            row.CreateCell(4).SetCellValue("WOStatus");
            row.CreateCell(5).SetCellValue("WOType");
            row.CreateCell(6).SetCellValue("UDF1");
            row.CreateCell(7).SetCellValue("UDF2");
            row.CreateCell(8).SetCellValue("UDF3");
            row.CreateCell(9).SetCellValue("UDF4");
            row.CreateCell(10).SetCellValue("UDF5");
            row.CreateCell(11).SetCellValue("SupplierName");
            row.CreateCell(12).SetCellValue("Asset");
            row.CreateCell(13).SetCellValue("Odometer");
            //csw.WriteField("ExpirationDate");
            //csw.WriteField("Cost");
            //csw.NextRecord();

            if (lstWOMain != null && lstWOMain.Count > 0)
            {

                int RowId = 1;
                foreach (WorkOrderDTO rec in lstWOMain)
                {

                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(rec.WOName);
                    DRow.CreateCell(0).SetCellValue(rec.ReleaseNumber);
                    DRow.CreateCell(1).SetCellValue(rec.Description);
                    DRow.CreateCell(2).SetCellValue(rec.TechnicianCodeNameStr);
                    DRow.CreateCell(3).SetCellValue(rec.Customer);
                    DRow.CreateCell(4).SetCellValue(rec.WOStatus);
                    DRow.CreateCell(5).SetCellValue(rec.WOType);
                    DRow.CreateCell(6).SetCellValue(rec.UDF1);
                    DRow.CreateCell(7).SetCellValue(rec.UDF2);
                    DRow.CreateCell(8).SetCellValue(rec.UDF3);
                    DRow.CreateCell(9).SetCellValue(rec.UDF4);
                    DRow.CreateCell(10).SetCellValue(rec.UDF5);
                    DRow.CreateCell(11).SetCellValue(rec.SupplierName);
                    DRow.CreateCell(12).SetCellValue(rec.AssetName);
                    DRow.CreateCell(13).SetCellValue(rec.Odometer_OperationHours.GetValueOrDefault(0));
                    //csw.WriteField(rec.ExpirationDate);
                    //csw.WriteField(rec.Cost);

                    RowId += 1;

                }
            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;

        }
        public string ItemLocationChangeImportEXCEL(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");

            string[] arrid = ids.Split(',');
            ItemMasterBinDAL obj = new ItemMasterBinDAL(SessionHelper.EnterPriseDBName);
            List<ItemMasterBinDTO> lstPullImport = obj.ItemBinListChangeImportExport(SessionHelper.RoomID, SessionHelper.CompanyID, ids);
            if (lstPullImport != null && lstPullImport.Count > 0)
            {
                lstPullImport = (from u in lstPullImport
                                 where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.OldLocationGUID.ToString())
                                 select u).ToList();
            }
            IRow row = sheet1.CreateRow(0);
            row.CreateCell(0).SetCellValue("id");
            row.CreateCell(1).SetCellValue("* ItemNumber");
            row.CreateCell(2).SetCellValue("* OldLocationName");
            row.CreateCell(3).SetCellValue("* NewLocationName");


            if (lstPullImport != null && lstPullImport.Count > 0)
            {

                int RowId = 1;
                foreach (ItemMasterBinDTO rec in lstPullImport)
                {

                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(rec.ID);
                    DRow.CreateCell(1).SetCellValue(rec.ItemNumber);
                    DRow.CreateCell(2).SetCellValue(rec.OldLocationName);
                    DRow.CreateCell(3).SetCellValue(rec.NewLocationName);


                    RowId += 1;

                }
            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
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

        public string ToolsMaintenanceListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            AssetMasterDAL obj = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolsMaintenanceDTO> lstToolsMaintenanceDTODTO = new List<ToolsMaintenanceDTO>();
            lstToolsMaintenanceDTODTO = obj.GetToolsMaintenanceByIDsNormal(ids, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(SortNameString).ToList();


            if (lstToolsMaintenanceDTODTO != null && lstToolsMaintenanceDTODTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* MaintenanceName");
                row.CreateCell(2).SetCellValue("* MaintenanceDate");
                row.CreateCell(3).SetCellValue("* ScheduleDate");
                row.CreateCell(4).SetCellValue("* SchedulerName");
                row.CreateCell(5).SetCellValue("* SchedulerForName");
                row.CreateCell(6).SetCellValue("ToolName");
                row.CreateCell(7).SetCellValue("AssetName");
                row.CreateCell(8).SetCellValue("TrackingMeasurement");
                row.CreateCell(9).SetCellValue("TrackingMeasurementValue");
                row.CreateCell(10).SetCellValue("LastMaintenanceDate");
                row.CreateCell(11).SetCellValue("LastMeasurementValue");
                row.CreateCell(12).SetCellValue("WOName");
                row.CreateCell(13).SetCellValue("RequisitionName");
                row.CreateCell(14).SetCellValue("Serial");
                row.CreateCell(15).SetCellValue("TotalCost");
                row.CreateCell(16).SetCellValue("UDF1");
                row.CreateCell(17).SetCellValue("UDF2");
                row.CreateCell(18).SetCellValue("UDF3");
                row.CreateCell(19).SetCellValue("UDF4");
                row.CreateCell(20).SetCellValue("UDF5");
                row.CreateCell(21).SetCellValue("Created");
                row.CreateCell(22).SetCellValue("Updated");
                row.CreateCell(23).SetCellValue("CreatedBy");
                row.CreateCell(24).SetCellValue("UpdatedBy");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (ToolsMaintenanceDTO item in lstToolsMaintenanceDTODTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.MaintenanceName ?? string.Empty);
                    DRow.CreateCell(2).SetCellValue(Convert.ToString(item.MaintenanceDateStr));
                    DRow.CreateCell(3).SetCellValue(Convert.ToString(item.ScheduleDateStrOnlyDate));
                    DRow.CreateCell(4).SetCellValue(item.SchedulerName);
                    DRow.CreateCell(5).SetCellValue(item.SchedulerForName);
                    DRow.CreateCell(6).SetCellValue(item.ToolName);
                    DRow.CreateCell(7).SetCellValue(item.AssetName);
                    if (item.TrackngMeasurement == 1)
                    {
                        if (item.TrackingMeasurementMapping == 2)
                        {
                            DRow.CreateCell(8).SetCellValue(@ResToolsScheduler.OperationalHours);
                        }
                        else if (item.TrackingMeasurementMapping == 3)
                        {
                            DRow.CreateCell(8).SetCellValue(@ResToolsScheduler.Mileage);
                        }
                        else
                        {
                            DRow.CreateCell(8).SetCellValue("");
                        }
                    }
                    else if (item.TrackngMeasurement == 2)
                    {
                        DRow.CreateCell(8).SetCellValue(@ResToolsScheduler.OperationalHours);
                    }
                    else if (item.TrackngMeasurement == 3)
                    {
                        DRow.CreateCell(8).SetCellValue(@ResToolsScheduler.Mileage);
                    }
                    else if (item.TrackngMeasurement == 4)
                    {
                        DRow.CreateCell(8).SetCellValue(@ResToolsScheduler.CheckOuts);
                    }
                    else
                    {
                        DRow.CreateCell(8).SetCellValue("");
                    }
                    DRow.CreateCell(9).SetCellValue(item.TrackingMeasurementValue);
                    DRow.CreateCell(10).SetCellValue(item.LastMaintenanceDateStr);
                    DRow.CreateCell(11).SetCellValue(item.LastMeasurementValue);
                    DRow.CreateCell(12).SetCellValue(item.WOName);
                    DRow.CreateCell(13).SetCellValue(item.RequisitionName);
                    DRow.CreateCell(14).SetCellValue(item.Serial);
                    DRow.CreateCell(15).SetCellValue(item.TotalCost ?? 0);
                    DRow.CreateCell(16).SetCellValue(item.UDF1);
                    DRow.CreateCell(17).SetCellValue(item.UDF2);
                    DRow.CreateCell(18).SetCellValue(item.UDF3);
                    DRow.CreateCell(19).SetCellValue(item.UDF4);
                    DRow.CreateCell(20).SetCellValue(item.UDF5);
                    DRow.CreateCell(21).SetCellValue(Convert.ToString(item.Created));
                    DRow.CreateCell(22).SetCellValue(Convert.ToString(item.Updated));
                    DRow.CreateCell(23).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(24).SetCellValue(item.UpdatedByName);
                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }

        public string EnterpriseQuickListExcel(string Filepath, string ModuleName, string ids, string SortNameString)
        {
            InitializeWorkbook();

            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            string[] arrid = ids.Split(',');
            EnterpriseQuickListDAL obj = new EnterpriseQuickListDAL(SessionHelper.EnterPriseDBName);
            var lstEnterpriseQLDTO = obj.GetAllEnterpriseQuickListWiseLineItem(ids).OrderBy(SortNameString).ToList();

            if (lstEnterpriseQLDTO != null && lstEnterpriseQLDTO.Count > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue("id");
                row.CreateCell(1).SetCellValue("* QuickListName");
                row.CreateCell(2).SetCellValue("* QLDetailNumber");
                row.CreateCell(3).SetCellValue("Quantity");
                //------------End--------------------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (var item in lstEnterpriseQLDTO)
                {
                    IRow DRow = sheet1.CreateRow(RowId);

                    DRow.CreateCell(0).SetCellValue(item.ID);
                    DRow.CreateCell(1).SetCellValue(item.Name);
                    DRow.CreateCell(2).SetCellValue(item.QLDetailNumber);
                    DRow.CreateCell(3).SetCellValue(item.Quantity);

                    RowId += 1;
                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }

        public string QuoteMasterChangeLogExcel(string Filepath, string ModuleName, string ids, long RoomID, long companyID,string sortColumnName)
        {
            InitializeWorkbook();
            string[] arrid = ids.Split(',');
            long QuoteId;
            long.TryParse(arrid[0], out QuoteId);
            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            QuoteMasterDAL quoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            var lstoflogs = quoteMasterDAL.GetPagedQuoteMasterChangeLog(QuoteId, 0, int.MaxValue, out TotalRecordCount,string.Empty, sortColumnName).ToList();
            if (lstoflogs != null && lstoflogs.Count() > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                //row.CreateCell(0).SetCellValue(ResCommon.HistoryID);
                row.CreateCell(0).SetCellValue(ResCommon.HistoryAction);
                row.CreateCell(1).SetCellValue(ResQuoteMaster.QuoteNumber);
                row.CreateCell(2).SetCellValue("From Where");
                row.CreateCell(3).SetCellValue(ResQuoteMaster.ReleaseNumber);
                row.CreateCell(4).SetCellValue(ResQuoteMaster.Comment);
                row.CreateCell(5).SetCellValue(ResQuoteMaster.RequiredDate);
                row.CreateCell(6).SetCellValue(ResQuoteMaster.QuoteStatus);
                row.CreateCell(7).SetCellValue(ResQuoteMaster.NoOfLineItems);
                row.CreateCell(8).SetCellValue(ResQuoteMaster.QuoteCost);
                row.CreateCell(9).SetCellValue(ResQuoteMaster.ChangeQuoteRevisionNo);                
                row.CreateCell(10).SetCellValue(ResCommon.CreatedOn);
                row.CreateCell(11).SetCellValue(ResCommon.CreatedBy);
                row.CreateCell(12).SetCellValue(ResCommon.UpdatedOn);
                row.CreateCell(13).SetCellValue(ResCommon.UpdatedBy);
                row.CreateCell(14).SetCellValue(ResCommon.RoomName);
                row.CreateCell(15).SetCellValue(ResCommon.IsDeleted);
                row.CreateCell(16).SetCellValue(ResCommon.AddedFrom);
                row.CreateCell(17).SetCellValue(ResCommon.EditedFrom);
                row.CreateCell(18).SetCellValue(ResCommon.ReceivedOnDate);
                row.CreateCell(19).SetCellValue(ResCommon.ReceivedOnWebDate);
                //------------End-------------("CreatedBy");-------

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (var item in lstoflogs)
                {
                    string noOfLineItems = item.NoOfLineItems.HasValue ? Convert.ToString(item.NoOfLineItems): string.Empty;
                    string quoteCost = item.QuoteCost.HasValue ? Convert.ToString(item.QuoteCost) : string.Empty;
                    string changeQuoteRevisionNo = item.ChangeQuoteRevisionNo.HasValue ? Convert.ToString(item.ChangeQuoteRevisionNo) : string.Empty;

                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.Action);
                    DRow.CreateCell(1).SetCellValue(item.QuoteNumber);
                    DRow.CreateCell(2).SetCellValue(item.WhatWhereAction);
                    DRow.CreateCell(3).SetCellValue(item.ReleaseNumber);
                    DRow.CreateCell(4).SetCellValue(item.Comment);
                    DRow.CreateCell(5).SetCellValue(item.RequiredDateStr);
                    DRow.CreateCell(6).SetCellValue(item.QuoteStatusChar);
                    DRow.CreateCell(7).SetCellValue(noOfLineItems);
                    DRow.CreateCell(8).SetCellValue(quoteCost);
                    DRow.CreateCell(9).SetCellValue(changeQuoteRevisionNo);
                    DRow.CreateCell(10).SetCellValue(item.CreatedDate);
                    DRow.CreateCell(11).SetCellValue(item.CreatedByName);
                    DRow.CreateCell(12).SetCellValue(item.UpdatedDate);
                    DRow.CreateCell(13).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(14).SetCellValue(item.RoomName);
                    DRow.CreateCell(15).SetCellValue((item.IsDeleted == true ? "Yes" : "No"));
                    DRow.CreateCell(16).SetCellValue(item.AddedFrom);
                    DRow.CreateCell(17).SetCellValue(item.EditedFrom);
                    DRow.CreateCell(18).SetCellValue(item.ReceivedOnDate);
                    DRow.CreateCell(19).SetCellValue(item.ReceivedOnDateWeb);
                    RowId += 1;

                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }

        public string PermissionTemplateChangeLogExcel(string Filepath, string ModuleName, string ids, long RoomID, long companyID, string sortColumnName)
        {
            InitializeWorkbook();
            string[] arrid = ids.Split(',');
            long TemplateId;
            long.TryParse(arrid[0], out TemplateId);
            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            PermissionTemplateDAL permissionTemplateDAL = new PermissionTemplateDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            var lstoflogs = permissionTemplateDAL.GetPagedPermissionTemplateChangeLog(TemplateId, 0, int.MaxValue, out TotalRecordCount, string.Empty, sortColumnName).ToList();
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
            if (lstoflogs != null && lstoflogs.Count() > 0)
            {
                //------------Create Header--------------------
                IRow row = sheet1.CreateRow(0);
                row.CreateCell(0).SetCellValue(ResCommon.HistoryID);
                row.CreateCell(1).SetCellValue(ResCommon.HistoryAction);
                row.CreateCell(2).SetCellValue(ResPermissionTemplate.TemplateName);
                row.CreateCell(3).SetCellValue(ResPermissionTemplate.Description);
                row.CreateCell(4).SetCellValue(ResCommon.CreatedOn);
                row.CreateCell(5).SetCellValue(ResCommon.UpdatedOn);
                row.CreateCell(6).SetCellValue(ResCommon.LastUpdatedBy);
                row.CreateCell(7).SetCellValue(ResCommon.CreatedBy);

                //-------------Set Row Value---------------------------
                int RowId = 1;
                foreach (var item in lstoflogs)
                {
                    IRow DRow = sheet1.CreateRow(RowId);
                    DRow.CreateCell(0).SetCellValue(item.HistoryID);
                    DRow.CreateCell(1).SetCellValue(item.Action);
                    DRow.CreateCell(2).SetCellValue(item.TemplateName);
                    DRow.CreateCell(3).SetCellValue(item.Description);
                    DRow.CreateCell(4).SetCellValue(item.CreatedDate);
                    DRow.CreateCell(5).SetCellValue(item.UpdatedDate);
                    DRow.CreateCell(6).SetCellValue(item.UpdatedByName);
                    DRow.CreateCell(7).SetCellValue(item.CreatedByName);
                    RowId += 1;

                    //-------------End--------------------------
                }

            }
            string filename = CommonUtility.GetExportFileName(ModuleName, ExportType.xls.ToString());
            string FilePath = Filepath + filename;
            WriteToFile(FilePath, ModuleName);
            return filename;
        }
    }
}