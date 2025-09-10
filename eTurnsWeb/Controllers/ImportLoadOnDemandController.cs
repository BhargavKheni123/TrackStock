using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class ImportLoadOnDemandController : eTurnsControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ImportMasters()
        {

            ImportMastersNewDTO.ImportMaster objImportMasterDTO = new ImportMastersNewDTO.ImportMaster();
            //objImportMasterDTO.ImportModule = Convert.ToInt32(ImportMastersNewDTO.TableName.ItemMaster);
            //objImportMasterDTO.ImportModule = Convert.ToInt32(ImportMastersNewDTO.TableName.CategoryMaster);
            //objImportMasterDTO.ImportModule = Convert.ToInt32(ImportMastersNewDTO.TableName.CustomerMaster);
            //objImportMasterDTO.ImportModule = Convert.ToInt32(ImportMastersNewDTO.TableName.QuickListItems);
            objImportMasterDTO.ImportModule = Convert.ToInt32(ImportMastersNewDTO.TableName.BinMaster);
            objImportMasterDTO.IsFileSuccessfulyUploaded = false;


            List<SelectListItem> lstModules = Enum.GetValues(typeof(ImportMastersNewDTO.TableName)).Cast<ImportMastersNewDTO.TableName>().Select(v => new SelectListItem
            {
                Text = CommonUtility.GetEnumDescription((ImportMastersNewDTO.TableName)v),
                Value = ((int)v).ToString()
            }).ToList();



            ViewBag.DDLModulelst = lstModules.OrderBy(c => c.Text).ToList();

            return View(objImportMasterDTO);
        }

        [HttpPost]
        public ActionResult ImportMasters(ImportMastersNewDTO.ImportMaster objImportMasterDTO)
        {
            string DataTableColumns = string.Empty;
            string ErrorMessage = string.Empty;

            //-------------------------------SAVE UPLOAD FILE-------------------------------------
            //
            if (objImportMasterDTO.UploadFile != null && objImportMasterDTO.UploadFile.ContentLength > 0)
            {
                SaveUploadedFile(objImportMasterDTO.UploadFile, objImportMasterDTO.ImportModule.ToString());

                //------------------------GET DEFAULT SUPPLIER LOCATION AND UNIT--------------------------------------------
                //
                string vDefaultSupplier = "";
                string vDefaultUOM = "";
                string vDefaultLocation = "";

                RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
                RoomDTO objRoomDTO = objRoomDal.GetRoomByID(SessionHelper.RoomID);

                if (objRoomDTO != null)
                {
                    if (objRoomDTO.DefaultSupplierID != null & objRoomDTO.DefaultSupplierID > 0)
                    {
                        SupplierMasterDAL objSuppDal = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                        SupplierMasterDTO objSuppDTO = new SupplierMasterDTO();
                        objSuppDTO = objSuppDal.GetRecord(objRoomDTO.DefaultSupplierID, SessionHelper.RoomID, SessionHelper.CompanyID, false);
                        if (objSuppDTO != null)
                            vDefaultSupplier = objSuppDTO.SupplierName;
                    }

                    if (objRoomDTO.DefaultBinID != null & objRoomDTO.DefaultBinID > 0)
                    {
                        BinMasterDAL objBinDal = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                        BinMasterDTO objBinDTO = new BinMasterDTO();
                        objBinDTO = objBinDal.GetRecord(objRoomDTO.DefaultBinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                        //objBinDTO = objBinDal.GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, objRoomDTO.DefaultBinID.GetValueOrDefault(0),null,null).FirstOrDefault();
                        if (objBinDTO != null)
                            vDefaultLocation = objBinDTO.BinNumber;
                    }

                    UnitMasterDAL objUnitDal = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
                    try
                    {
                        UnitMasterDTO objUnit = objUnitDal.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(t => t.Unit == "EA").FirstOrDefault();
                        if (objUnit != null)
                            vDefaultUOM = objUnit.Unit;

                    }
                    catch { }

                }

                switch ((ImportMastersNewDTO.TableName)objImportMasterDTO.ImportModule)
                {
                    case (ImportMastersNewDTO.TableName.ItemMaster):
                        List<ImportMastersNewDTO.ItemMasterImport> lstImport;
                        if (GetItemMasterDataFromFile(objImportMasterDTO.UploadFile, vDefaultSupplier, vDefaultUOM, vDefaultLocation, ImportMastersNewDTO.TableName.ItemMaster.ToString(), out lstImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            //objImportMasterDTO.objImportPageDTO.lstItemMasterImportData = lstImport;
                            Session["ImportDataSession"] = lstImport;
                            Session["ColuumnList"] = DataTableColumns;//objImportMasterDTO.objImportPageDTO.DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = lstImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.CategoryMaster):
                        List<ImportMastersNewDTO.CategoryMasterImport> catListImport;
                        if (GetCategoryMasterDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.CategoryMaster.ToString(), out catListImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = catListImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = catListImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.CustomerMaster):
                        List<ImportMastersNewDTO.CustomerMasterImport> custListImport;
                        if (GetCustomerMasterDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.CustomerMaster.ToString(), out custListImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = custListImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = custListImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.AssetMaster):
                        List<ImportMastersNewDTO.AssetMasterImport> assetListImport;
                        if (GetAssetMasterDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.AssetMaster.ToString(), out assetListImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = assetListImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = assetListImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.ToolMaster):
                        List<ImportMastersNewDTO.ToolMasterImport> toolListImport;
                        if (GetToolMasterDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.ToolMaster.ToString(), out toolListImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = toolListImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = toolListImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.SupplierMaster):
                        List<ImportMastersNewDTO.SupplierMasterImport> supplierListImport;
                        if (GetSupplierMasterDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.SupplierMaster.ToString(), out supplierListImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = supplierListImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = supplierListImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.QuickListItems):
                        List<ImportMastersNewDTO.QuickListItemsImport> quickListItemsImport;
                        if (GetQuickListItemsDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.QuickListItems.ToString(), out quickListItemsImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = quickListItemsImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = quickListItemsImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.ManualCount):
                        List<ImportMastersNewDTO.InventoryLocationImport> inventoryLocationImport;
                        if (GetInventoryLocationDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.ManualCount.ToString(), out inventoryLocationImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = inventoryLocationImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = inventoryLocationImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.BinMaster):
                        List<ImportMastersNewDTO.InventoryLocationImport> adjustmentCountBMImport;
                        if (GetAdjustmentCountBMDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.BinMaster.ToString(), out adjustmentCountBMImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = adjustmentCountBMImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = adjustmentCountBMImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.WorkOrder):
                        List<ImportMastersNewDTO.WorkOrderImport> wokOrderImport;
                        if (GetWorkOrderDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.WorkOrder.ToString(), out wokOrderImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = wokOrderImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = wokOrderImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.BOMItemMaster):
                        List<ImportMastersNewDTO.BOMItemMasterImport> bomItemMasterImport;
                        if (GetBOMItemMasterDataFromFile(objImportMasterDTO.UploadFile, vDefaultSupplier, vDefaultUOM, vDefaultLocation, ImportMastersNewDTO.TableName.BOMItemMaster.ToString(), out bomItemMasterImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = bomItemMasterImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = bomItemMasterImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.kitdetail):
                        List<ImportMastersNewDTO.KitDetailImport> kitdetailImport;
                        if (GetKitDetailDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.kitdetail.ToString(), out kitdetailImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = kitdetailImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = kitdetailImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.ProjectMaster):
                        List<ImportMastersNewDTO.ProjectMasterImport> projectMasterImport;
                        if (GetProjectMasterDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.ProjectMaster.ToString(), out projectMasterImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = projectMasterImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = projectMasterImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.ItemLocationeVMISetup):
                        List<ImportMastersNewDTO.InventoryLocationQuantityImport> itemLocationsImport;
                        if (GetItemLocationseVMIDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.ItemLocationeVMISetup.ToString(), out itemLocationsImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = itemLocationsImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = itemLocationsImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.ItemManufacturerDetails):
                        List<ImportMastersNewDTO.ItemManufacturerImport> itemManufacturerImport;
                        if (GetItemManufacturerDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.ItemManufacturerDetails.ToString(), out itemManufacturerImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = itemManufacturerImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = itemManufacturerImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.ItemSupplierDetails):
                        List<ImportMastersNewDTO.ItemSupplierImport> itemSupplierImport;
                        if (GetItemSupplierDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.ItemSupplierDetails.ToString(), out itemSupplierImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = itemSupplierImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = itemSupplierImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    case (ImportMastersNewDTO.TableName.BarcodeMaster):
                        List<ImportMastersNewDTO.BarcodeMasterImport> barcodeMasterImport;
                        if (GetBarcodeMasterDataFromFile(objImportMasterDTO.UploadFile, ImportMastersNewDTO.TableName.BarcodeMaster.ToString(), out barcodeMasterImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            Session["ImportDataSession"] = barcodeMasterImport;
                            Session["ColuumnList"] = DataTableColumns;
                            objImportMasterDTO.objImportPageDTO.RecordCount = barcodeMasterImport.Count();
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (objImportMasterDTO.objImportPageDTO != null && objImportMasterDTO.objImportPageDTO.lstItemMasterImportData != null && objImportMasterDTO.objImportPageDTO.lstItemMasterImportData.Count > 0)
            {
                objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                if (objImportMasterDTO.objImportPageDTO.DataTableColumns == null && Request["hdnDataTableColumns"] != null)
                {
                    string strDataTableColumns = Convert.ToString(Request["hdnDataTableColumns"]);
                    objImportMasterDTO.objImportPageDTO.DataTableColumns = strDataTableColumns.Split(',');
                }
            }

            //--------------------------------------------------------------------
            //
            List<SelectListItem> lstModules = Enum.GetValues(typeof(ImportMastersNewDTO.TableName)).Cast<ImportMastersNewDTO.TableName>().Select(v => new SelectListItem
            {
                Text = CommonUtility.GetEnumDescription((ImportMastersNewDTO.TableName)v),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.DDLModulelst = lstModules.OrderBy(t => t.Text).ToList();
            ViewBag.ErrorMessage = ErrorMessage;
            return View(objImportMasterDTO);
        }

        #region [PRIVATE METHOD READ DATA FROM FILE]

        #region [ITEM MASTER]
        private bool GetItemMasterDataFromFile(HttpPostedFileBase uploadFile, string vDefaultSupplier, string vDefaultUOM, string vDefaultLocation, string ImportModule,
                                                   out List<ImportMastersNewDTO.ItemMasterImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.ItemMasterImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.ItemMasterImport obj = new ImportMastersNewDTO.ItemMasterImport();
                try
                {
                    //bool AllowInsert = true;
                    if (item[CommonUtility.ImportItemColumn.ItemNumber.ToString()].ToString() != "")
                    //  AllowInsert = lstImport.Where(x => x.ItemNumber == item[CommonUtility.ImportItemColumn.ItemNumber.ToString()].ToString()).ToList().Count > 0 ? false : true;

                    //if (AllowInsert == true)
                    {
                        // obj.ID = Convert.ToInt32(item[CommonUtility.ImportItemColumn.ID.ToString()].ToString());

                        obj.ItemNumber = Convert.ToString(item[CommonUtility.ImportItemColumn.ItemNumber.ToString()]);

                        //Int64 ManufacturerID;
                        //Int64.TryParse(item[CommonUtility.ImportItemColumn.ManufacturerID.ToString()].ToString(), out ManufacturerID);
                        //obj.ManufacturerID = ManufacturerID;
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Manufacturer.ToString()))
                        {
                            obj.ManufacturerName = Convert.ToString(item[CommonUtility.ImportItemColumn.Manufacturer.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ManufacturerNumber.ToString()))
                        {
                            obj.ManufacturerNumber = Convert.ToString(item[CommonUtility.ImportItemColumn.ManufacturerNumber.ToString()]);
                        }


                        //Int64 SupplierID;
                        //Int64.TryParse(item[CommonUtility.ImportItemColumn.SupplierID.ToString()].ToString(), out SupplierID);
                        //obj.SupplierID = SupplierID;                                            
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SupplierName.ToString()))
                        {
                            obj.SupplierName = Convert.ToString(item[CommonUtility.ImportItemColumn.SupplierName.ToString()]);

                            string ItemTypeName = string.Empty;
                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemType.ToString()))
                            {
                                ItemTypeName = item[CommonUtility.ImportItemColumn.ItemType.ToString()].ToString();
                            }

                            if (string.IsNullOrEmpty(obj.SupplierName) && ItemTypeName != "Labor")
                            {
                                obj.SupplierName = vDefaultSupplier;
                            }
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SupplierPartNo.ToString()))
                        {
                            obj.SupplierPartNo = Convert.ToString(item[CommonUtility.ImportItemColumn.SupplierPartNo.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.BlanketOrderNumber.ToString()))
                        {
                            obj.BlanketOrderNumber = Convert.ToString(item[CommonUtility.ImportItemColumn.BlanketOrderNumber.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UPC.ToString()))
                        {
                            obj.UPC = Convert.ToString(item[CommonUtility.ImportItemColumn.UPC.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UNSPSC.ToString()))
                        {
                            obj.UNSPSC = Convert.ToString(item[CommonUtility.ImportItemColumn.UNSPSC.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Description.ToString()))
                        {
                            obj.Description = Convert.ToString(item[CommonUtility.ImportItemColumn.Description.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.LongDescription.ToString()))
                        {
                            obj.LongDescription = Convert.ToString(item[CommonUtility.ImportItemColumn.LongDescription.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.CategoryName.ToString()))
                        {
                            obj.CategoryName = Convert.ToString(item[CommonUtility.ImportItemColumn.CategoryName.ToString()]);
                        }



                        //Int64 GLAccountID;
                        //Int64.TryParse(item[CommonUtility.ImportItemColumn.GLAccountID.ToString()].ToString(), out GLAccountID);
                        //obj.GLAccountID = GLAccountID;
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.GLAccount.ToString()))
                        {
                            obj.GLAccount = Convert.ToString(item[CommonUtility.ImportItemColumn.GLAccount.ToString()]);
                        }

                        //Int64 UOMID;
                        //Int64.TryParse(item[CommonUtility.ImportItemColumn.Unit.ToString()].ToString(), out UOMID);
                        //obj.UOMID = UOMID;
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UOM.ToString()))
                        {
                            obj.Unit = Convert.ToString(item[CommonUtility.ImportItemColumn.UOM.ToString()]);

                            if (string.IsNullOrEmpty(obj.Unit))
                            {
                                obj.Unit = vDefaultUOM;
                            }
                        }

                        //if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.CostUOM.ToString()))
                        //{
                        //    decimal CostUOM;
                        //    decimal.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.CostUOM.ToString()]), out CostUOM);
                        //    obj.CostUOMName = CostUOM;
                        //}

                        //Added : Esha, For Cost UOM IMport

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.CostUOM.ToString()))
                        {
                            obj.CostUOMName = Convert.ToString(item[CommonUtility.ImportItemColumn.CostUOM.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.DefaultReorderQuantity.ToString()))
                        {
                            double DefaultReorderQuantity;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.DefaultReorderQuantity.ToString()]), out DefaultReorderQuantity))
                                obj.DefaultReorderQuantity = DefaultReorderQuantity;
                            else
                                obj.DefaultReorderQuantity = 0;
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.DefaultPullQuantity.ToString()))
                        {
                            double DefaultPullQuantity;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.DefaultPullQuantity.ToString()]), out DefaultPullQuantity))
                                obj.DefaultPullQuantity = DefaultPullQuantity;
                            else
                                obj.DefaultPullQuantity = 0;
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Cost.ToString()))
                        {
                            double Cost;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Cost.ToString()]), out Cost))
                                obj.Cost = Cost;
                            else
                                obj.Cost = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Markup.ToString()))
                        {
                            double Markup;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Markup.ToString()]), out Markup))
                                obj.Markup = Markup;
                            else
                                obj.Markup = 0;
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SellPrice.ToString()))
                        {
                            double SellPrice;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.SellPrice.ToString()]), out SellPrice))
                                obj.SellPrice = SellPrice;
                            else
                                obj.SellPrice = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.LeadTimeInDays.ToString()))
                        {
                            Int32 LeadTimeInDays;
                            if (Int32.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.LeadTimeInDays.ToString()]), out LeadTimeInDays))
                                obj.LeadTimeInDays = LeadTimeInDays;
                            else
                                obj.LeadTimeInDays = 0;

                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Trend.ToString()))
                        {
                            Boolean Trend = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Trend.ToString()]), out Trend))
                                obj.Trend = Trend;
                            else
                                obj.Trend = false;
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Taxable.ToString()))
                        {
                            Boolean Taxable = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Taxable.ToString()]), out Taxable))
                                obj.Taxable = Taxable;
                            else
                                obj.Taxable = false;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Consignment.ToString()))
                        {
                            Boolean Consignment = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Consignment.ToString()]), out Consignment))
                                obj.Consignment = Consignment;
                            else
                                obj.Consignment = false;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.CriticalQuantity.ToString()))
                        {
                            double CriticalQuantity;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.CriticalQuantity.ToString()]), out CriticalQuantity))
                                obj.CriticalQuantity = CriticalQuantity;
                            else
                                obj.CriticalQuantity = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.MinimumQuantity.ToString()))
                        {
                            double MinimumQuantity;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.MinimumQuantity.ToString()]), out MinimumQuantity))
                                obj.MinimumQuantity = MinimumQuantity;
                            else
                                obj.MinimumQuantity = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.MaximumQuantity.ToString()))
                        {
                            double MaximumQuantity;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.MaximumQuantity.ToString()]), out MaximumQuantity))
                                obj.MaximumQuantity = MaximumQuantity;
                            else
                                obj.MaximumQuantity = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.WeightPerPiece.ToString()))
                        {
                            double WeightPerPiece;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.WeightPerPiece.ToString()]), out WeightPerPiece))
                                obj.WeightPerPiece = WeightPerPiece;
                            else
                                obj.WeightPerPiece = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemUniqueNumber.ToString()))
                        {
                            obj.ItemUniqueNumber = Convert.ToString(item[CommonUtility.ImportItemColumn.ItemUniqueNumber.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsTransfer.ToString()))
                        {
                            Boolean IsTransfer = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsTransfer.ToString()]), out IsTransfer))
                                obj.IsTransfer = IsTransfer;
                            else
                                obj.IsTransfer = false;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsPurchase.ToString()))
                        {
                            Boolean IsPurchase = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsPurchase.ToString()]), out IsPurchase))
                            {
                                if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsTransfer.ToString()))
                                {
                                    if (Convert.ToString(item[CommonUtility.ImportItemColumn.IsTransfer.ToString()]) == "" && Convert.ToString(item[CommonUtility.ImportItemColumn.IsPurchase.ToString()]) == "")
                                    {
                                        obj.IsPurchase = true;
                                    }
                                    else
                                    {
                                        obj.IsPurchase = IsPurchase;
                                    }

                                }
                                else if (Convert.ToString(item[CommonUtility.ImportItemColumn.IsPurchase.ToString()]) == "")
                                {
                                    obj.IsPurchase = true;
                                }
                                else
                                {

                                    obj.IsPurchase = IsPurchase;
                                }
                            }
                            else
                            {
                                obj.IsPurchase = false;
                            }
                        }


                        //Int64 DefaultLocation;
                        //Int64.TryParse(item[CommonUtility.ImportItemColumn.DefaultLocation.ToString()].ToString(), out DefaultLocation);
                        //obj.DefaultLocation = DefaultLocation;
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.InventryLocation.ToString()))
                        {
                            obj.InventryLocation = Convert.ToString(item[CommonUtility.ImportItemColumn.InventryLocation.ToString()]);

                            if (string.IsNullOrEmpty(obj.InventryLocation))
                            {
                                obj.InventryLocation = vDefaultLocation;
                            }
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.InventoryClassification.ToString()))
                        {
                            string InventoryClassification;
                            InventoryClassification = Convert.ToString(item[CommonUtility.ImportItemColumn.InventoryClassification.ToString()]);
                            obj.InventoryClassificationName = InventoryClassification;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SerialNumberTracking.ToString()))
                        {
                            Boolean SerialNumberTracking = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.SerialNumberTracking.ToString()]), out SerialNumberTracking))
                                obj.SerialNumberTracking = SerialNumberTracking;
                            else
                                obj.SerialNumberTracking = false;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.LotNumberTracking.ToString()))
                        {
                            Boolean LotNumberTracking = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.LotNumberTracking.ToString()]), out LotNumberTracking))
                                obj.LotNumberTracking = LotNumberTracking;
                            else
                                obj.LotNumberTracking = false;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.DateCodeTracking.ToString()))
                        {
                            Boolean DateCodeTracking = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.DateCodeTracking.ToString()]), out DateCodeTracking))
                                obj.DateCodeTracking = DateCodeTracking;
                            else
                                obj.DateCodeTracking = false;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemType.ToString()))
                        {
                            obj.ItemTypeName = item[CommonUtility.ImportItemColumn.ItemType.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ImagePath.ToString()))
                        {
                            obj.ImagePath = item[CommonUtility.ImportItemColumn.ImagePath.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF1.ToString()))
                        {
                            obj.UDF1 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF1.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF2.ToString()))
                        {
                            obj.UDF2 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF2.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF3.ToString()))
                        {
                            obj.UDF3 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF3.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF4.ToString()))
                        {
                            obj.UDF4 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF4.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF5.ToString()))
                        {
                            obj.UDF5 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF5.ToString()]);
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Link1.ToString()))
                        {
                            obj.Link1 = Convert.ToString(item[CommonUtility.ImportItemColumn.Link1.ToString()]);
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Link2.ToString()))
                        {
                            obj.Link2 = Convert.ToString(item[CommonUtility.ImportItemColumn.Link2.ToString()]);
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemLink2ExternalURL.ToString()))
                        {
                            obj.ItemLink2ExternalURL = Convert.ToString(item[CommonUtility.ImportItemColumn.ItemLink2ExternalURL.ToString()]);
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsLotSerialExpiryCost.ToString()))
                        {
                            obj.IsLotSerialExpiryCost = Convert.ToString(item[CommonUtility.ImportItemColumn.IsLotSerialExpiryCost.ToString()]);
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsItemLevelMinMaxQtyRequired.ToString()))
                        {
                            Boolean ItemLevelMinMaxQtyRequired = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsItemLevelMinMaxQtyRequired.ToString()]), out ItemLevelMinMaxQtyRequired))
                                obj.IsItemLevelMinMaxQtyRequired = ItemLevelMinMaxQtyRequired;
                            else
                                obj.IsItemLevelMinMaxQtyRequired = false;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsBuildBreak.ToString()))
                        {
                            Boolean BuildBreak = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsBuildBreak.ToString()]), out BuildBreak))
                                obj.IsBuildBreak = BuildBreak;
                            else
                                obj.IsBuildBreak = false;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsAutoInventoryClassification.ToString()))
                        {
                            Boolean AutoInventoryClassification = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsAutoInventoryClassification.ToString()]), out AutoInventoryClassification))
                                obj.IsAutoInventoryClassification = AutoInventoryClassification;
                            else
                                obj.IsAutoInventoryClassification = false;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.EnforceDefaultPullQuantity.ToString()))
                        {
                            Boolean IsPullQtyScanOverride = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.EnforceDefaultPullQuantity.ToString()]), out IsPullQtyScanOverride))
                                obj.PullQtyScanOverride = IsPullQtyScanOverride;
                            else
                                obj.PullQtyScanOverride = false;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.EnforceDefaultReorderQuantity.ToString()))
                        {
                            Boolean IsEnforceDefaultReorderQuantity = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.EnforceDefaultReorderQuantity.ToString()]), out IsEnforceDefaultReorderQuantity))
                                obj.IsEnforceDefaultReorderQuantity = IsEnforceDefaultReorderQuantity;
                            else
                                obj.IsEnforceDefaultReorderQuantity = false;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.TrendingSetting.ToString()))
                        {
                            obj.TrendingSettingName = item[CommonUtility.ImportItemColumn.TrendingSetting.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsPackslipMandatoryAtReceive.ToString()))
                        {
                            Boolean IsPullQtyScanOverride = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsPackslipMandatoryAtReceive.ToString()]), out IsPullQtyScanOverride))
                                obj.IsPackslipMandatoryAtReceive = IsPullQtyScanOverride;
                            else
                                obj.IsPackslipMandatoryAtReceive = false;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemImageExternalURL.ToString()))
                        {
                            obj.ItemImageExternalURL = Convert.ToString(item[CommonUtility.ImportItemColumn.ItemImageExternalURL.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemDocExternalURL.ToString()))
                        {
                            obj.ItemDocExternalURL = Convert.ToString(item[CommonUtility.ImportItemColumn.ItemDocExternalURL.ToString()]);
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsDeleted.ToString()))
                        {
                            Boolean IsPullQtyScanOverride = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsDeleted.ToString()]), out IsPullQtyScanOverride))
                                obj.IsDeleted = IsPullQtyScanOverride;
                            else
                                obj.IsDeleted = false;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsActive.ToString()))
                        {
                            Boolean IsActive = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsActive.ToString()]), out IsActive))
                                obj.IsActive = IsActive;
                            else
                                obj.IsActive = false;
                        }
                        else
                        {
                            obj.IsActive = true;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ExtendedCost.ToString()))
                        {
                            double ExtendedCost;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.ExtendedCost.ToString()]), out ExtendedCost))
                                obj.ExtendedCost = ExtendedCost;
                            else
                                obj.ExtendedCost = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.StagedQuantity.ToString()))
                        {
                            double StagedQuantity;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.StagedQuantity.ToString()]), out StagedQuantity))
                                obj.StagedQuantity = StagedQuantity;
                            else
                                obj.StagedQuantity = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.InTransitquantity.ToString()))
                        {
                            double InTransitquantity;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.InTransitquantity.ToString()]), out InTransitquantity))
                                obj.InTransitquantity = InTransitquantity;
                            else
                                obj.InTransitquantity = 0;
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OnOrderQuantity.ToString()))
                        {
                            double OnOrderQuantity;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OnOrderQuantity.ToString()]), out OnOrderQuantity))
                                obj.OnOrderQuantity = OnOrderQuantity;
                            else
                                obj.OnOrderQuantity = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OnTransferQuantity.ToString()))
                        {
                            double OnTransferQuantity;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OnTransferQuantity.ToString()]), out OnTransferQuantity))
                                obj.OnTransferQuantity = OnTransferQuantity;
                            else
                                obj.OnTransferQuantity = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SuggestedOrderQuantity.ToString()))
                        {
                            double SuggestedOrderQuantity;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.SuggestedOrderQuantity.ToString()]), out SuggestedOrderQuantity))
                                obj.SuggestedOrderQuantity = SuggestedOrderQuantity;
                            else
                                obj.SuggestedOrderQuantity = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.RequisitionedQuantity.ToString()))
                        {
                            double RequisitionedQuantity;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.RequisitionedQuantity.ToString()]), out RequisitionedQuantity))
                                obj.RequisitionedQuantity = RequisitionedQuantity;
                            else
                                obj.RequisitionedQuantity = 0;

                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.AverageUsage.ToString()))
                        {
                            double AverageUsage;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.AverageUsage.ToString()]), out AverageUsage))
                                obj.AverageUsage = AverageUsage;
                            else
                                obj.AverageUsage = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Turns.ToString()))
                        {
                            double Turns;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Turns.ToString()]), out Turns))
                                obj.Turns = Turns;
                            else
                                obj.Turns = 0;
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OnHandQuantity.ToString()))
                        {
                            double OnHandQuantity;
                            if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OnHandQuantity.ToString()]), out OnHandQuantity))
                                obj.OnHandQuantity = OnHandQuantity;
                            else
                                obj.OnHandQuantity = 0;
                        }

                        obj.Status = "";
                        obj.Reason = "";
                        obj.Index = list.IndexOf(item);

                        //obj.IsLotSerialExpiryCost = false;
                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [CATEGORY MASTER]
        private bool GetCategoryMasterDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.CategoryMasterImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.CategoryMasterImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.CategoryMasterImport obj = new ImportMastersNewDTO.CategoryMasterImport();
                try
                {
                    if (item[CommonUtility.ImportCategoryColumn.Category.ToString()].ToString() != "")
                    {
                        // obj.ID = Convert.ToInt32(item[CommonUtility.ImportCategoryColumn.ID.ToString()].ToString());

                        obj.Category = Convert.ToString(item[CommonUtility.ImportCategoryColumn.Category.ToString()]);

                        if (item.Table.Columns.Contains(CommonUtility.ImportCategoryColumn.UDF1.ToString()))
                        {
                            obj.UDF1 = Convert.ToString(item[CommonUtility.ImportCategoryColumn.UDF1.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCategoryColumn.UDF2.ToString()))
                        {
                            obj.UDF2 = Convert.ToString(item[CommonUtility.ImportCategoryColumn.UDF2.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCategoryColumn.UDF3.ToString()))
                        {
                            obj.UDF3 = Convert.ToString(item[CommonUtility.ImportCategoryColumn.UDF3.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCategoryColumn.UDF4.ToString()))
                        {
                            obj.UDF4 = Convert.ToString(item[CommonUtility.ImportCategoryColumn.UDF4.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCategoryColumn.UDF5.ToString()))
                        {
                            obj.UDF5 = Convert.ToString(item[CommonUtility.ImportCategoryColumn.UDF5.ToString()]);
                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [CUSTOMER MASTER]
        private bool GetCustomerMasterDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.CustomerMasterImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.CustomerMasterImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.CustomerMasterImport obj = new ImportMastersNewDTO.CustomerMasterImport();
                try
                {
                    if (item[CommonUtility.ImportCustomerColumn.Customer.ToString()].ToString() != "")
                    {
                        // obj.ID = Convert.ToInt32(item[CommonUtility.ImportCustomerColumn.ID.ToString()].ToString());
                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.Customer.ToString()))
                        {
                            obj.Customer = Convert.ToString(item[CommonUtility.ImportCustomerColumn.Customer.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.Account.ToString()))
                        {
                            obj.Account = item[CommonUtility.ImportCustomerColumn.Account.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.Contact.ToString()))
                        {
                            obj.Contact = item[CommonUtility.ImportCustomerColumn.Contact.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.Address.ToString()))
                        {
                            obj.Address = item[CommonUtility.ImportCustomerColumn.Address.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.City.ToString()))
                        {
                            obj.City = item[CommonUtility.ImportCustomerColumn.City.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.State.ToString()))
                        {
                            obj.State = item[CommonUtility.ImportCustomerColumn.State.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.ZipCode.ToString()))
                        {
                            obj.ZipCode = item[CommonUtility.ImportCustomerColumn.ZipCode.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.Country.ToString()))
                        {
                            obj.Country = item[CommonUtility.ImportCustomerColumn.Country.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.Phone.ToString()))
                        {
                            obj.Phone = item[CommonUtility.ImportCustomerColumn.Phone.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.Email.ToString()))
                        {
                            obj.Email = item[CommonUtility.ImportCustomerColumn.Email.ToString()].ToString();
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF1.ToString()))
                        {
                            obj.UDF1 = Convert.ToString(item[CommonUtility.ImportCustomerColumn.UDF1.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF2.ToString()))
                        {
                            obj.UDF2 = Convert.ToString(item[CommonUtility.ImportCustomerColumn.UDF2.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF3.ToString()))
                        {
                            obj.UDF3 = Convert.ToString(item[CommonUtility.ImportCustomerColumn.UDF3.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF4.ToString()))
                        {
                            obj.UDF4 = Convert.ToString(item[CommonUtility.ImportCustomerColumn.UDF4.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF5.ToString()))
                        {
                            obj.UDF5 = Convert.ToString(item[CommonUtility.ImportCustomerColumn.UDF5.ToString()]);
                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [ASSET MASTER]
        private bool GetAssetMasterDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.AssetMasterImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.AssetMasterImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.AssetMasterImport obj = new ImportMastersNewDTO.AssetMasterImport();
                try
                {
                    if (item[CommonUtility.ImportAssetMasterColumn.AssetName.ToString()].ToString() != "")
                    {

                        //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryClassificationColumn.ID.ToString()].ToString());
                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.AssetName.ToString()))
                        {
                            obj.AssetName = item[CommonUtility.ImportAssetMasterColumn.AssetName.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.Serial.ToString()))
                        {
                            obj.Serial = item[CommonUtility.ImportAssetMasterColumn.Serial.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.Description.ToString()))
                        {
                            obj.Description = item[CommonUtility.ImportAssetMasterColumn.Description.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.Make.ToString()))
                        {
                            obj.Make = item[CommonUtility.ImportAssetMasterColumn.Make.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.Model.ToString()))
                        {
                            obj.Model = item[CommonUtility.ImportAssetMasterColumn.Model.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.ToolCategory.ToString()))
                        {
                            obj.ToolCategoryName = item[CommonUtility.ImportAssetMasterColumn.ToolCategory.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.PurchaseDate.ToString()))
                        {
                            //obj.PurchaseDate = Convert.ToDateTime(item[CommonUtility.ImportAssetMasterColumn.PurchaseDate.ToString()].ToString() == "" ? Convert.ToString(DateTime.MinValue) : item[CommonUtility.ImportAssetMasterColumn.PurchaseDate.ToString()].ToString());
                            if (item[CommonUtility.ImportAssetMasterColumn.PurchaseDate.ToString()].ToString() != "")
                            {
                                DateTime dt;
                                DateTime.TryParseExact(item[CommonUtility.ImportAssetMasterColumn.PurchaseDate.ToString()].ToString().Split(' ')[0], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);
                                if (dt != DateTime.MinValue)
                                    obj.PurchaseDate = dt;
                                else
                                {
                                    obj.PurchaseDate = null;
                                    obj.Reason = "PurchaseDate should be in " + SessionHelper.RoomDateFormat + " format";
                                    obj.Status = "Fail";
                                }
                            }
                            else
                            {
                                obj.PurchaseDate = null;
                            }

                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.PurchasePrice.ToString()))
                        {
                            //obj.PurchasePrice = Convert.ToDouble(item[CommonUtility.ImportAssetMasterColumn.PurchasePrice.ToString()].ToString() == "" ? "0.0" : item[CommonUtility.ImportAssetMasterColumn.PurchasePrice.ToString()].ToString());
                            if (item[CommonUtility.ImportAssetMasterColumn.PurchasePrice.ToString()].ToString() != "")
                            {
                                double price;
                                double.TryParse(item[CommonUtility.ImportAssetMasterColumn.PurchasePrice.ToString()].ToString(), out price);
                                obj.PurchasePrice = price;
                            }
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.DepreciatedValue.ToString()))
                        {
                            // obj.DepreciatedValue = Convert.ToDouble(item[CommonUtility.ImportAssetMasterColumn.DepreciatedValue.ToString()].ToString() == "" ? "0.0" : item[CommonUtility.ImportAssetMasterColumn.DepreciatedValue.ToString()].ToString());
                            if (item[CommonUtility.ImportAssetMasterColumn.DepreciatedValue.ToString()].ToString() != "")
                            {
                                double price;
                                double.TryParse(item[CommonUtility.ImportAssetMasterColumn.DepreciatedValue.ToString()].ToString(), out price);
                                obj.DepreciatedValue = price;
                            }
                            else
                            {
                                obj.DepreciatedValue = 0.0;
                            }
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.UDF1.ToString()))
                        {
                            obj.UDF1 = item[CommonUtility.ImportAssetMasterColumn.UDF1.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.UDF2.ToString()))
                        {
                            obj.UDF2 = item[CommonUtility.ImportAssetMasterColumn.UDF2.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.UDF3.ToString()))
                        {
                            obj.UDF3 = item[CommonUtility.ImportAssetMasterColumn.UDF3.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.UDF4.ToString()))
                        {
                            obj.UDF4 = item[CommonUtility.ImportAssetMasterColumn.UDF4.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.UDF5.ToString()))
                        {
                            obj.UDF5 = item[CommonUtility.ImportAssetMasterColumn.UDF5.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.UDF6.ToString()))
                        {
                            obj.UDF6 = item[CommonUtility.ImportAssetMasterColumn.UDF6.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.UDF7.ToString()))
                        {
                            obj.UDF7 = item[CommonUtility.ImportAssetMasterColumn.UDF7.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.UDF8.ToString()))
                        {
                            obj.UDF8 = item[CommonUtility.ImportAssetMasterColumn.UDF8.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.UDF9.ToString()))
                        {
                            obj.UDF9 = item[CommonUtility.ImportAssetMasterColumn.UDF9.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.UDF10.ToString()))
                        {
                            obj.UDF10 = item[CommonUtility.ImportAssetMasterColumn.UDF10.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.AssetCategory.ToString()))
                        {
                            obj.AssetCategoryName = item[CommonUtility.ImportAssetMasterColumn.AssetCategory.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.ImagePath.ToString()))
                        {
                            obj.ImagePath = item[CommonUtility.ImportAssetMasterColumn.ImagePath.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.AssetImageExternalURL.ToString()))
                        {
                            obj.AssetImageExternalURL = Convert.ToString(item[CommonUtility.ImportAssetMasterColumn.AssetImageExternalURL.ToString()]);
                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [TOOL MASTER]
        private bool GetToolMasterDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.ToolMasterImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.ToolMasterImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.ToolMasterImport obj = new ImportMastersNewDTO.ToolMasterImport();
                try
                {
                    //if (item[CommonUtility.ImportToolMasterColumn.ToolName.ToString()].ToString() != "" && item[CommonUtility.ImportToolMasterColumn.Serial.ToString()].ToString() != "")
                    if (item[CommonUtility.ImportToolMasterColumn.ToolName.ToString()].ToString() != "")
                    {

                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.ToolName.ToString()))
                        {
                            obj.ToolName = item[CommonUtility.ImportToolMasterColumn.ToolName.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.Serial.ToString()))
                        {
                            obj.Serial = item[CommonUtility.ImportToolMasterColumn.Serial.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.Description.ToString()))
                        {
                            obj.Description = item[CommonUtility.ImportToolMasterColumn.Description.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.ToolCategory.ToString()))
                        {
                            obj.ToolCategoryName = item[CommonUtility.ImportToolMasterColumn.ToolCategory.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.Location.ToString()))
                        {
                            obj.Location = item[CommonUtility.ImportToolMasterColumn.Location.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.Quantity.ToString()))
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportToolMasterColumn.Quantity.ToString()])))
                            {
                                double _Quantity = 0;
                                if (double.TryParse(item[CommonUtility.ImportToolMasterColumn.Quantity.ToString()].ToString(), out _Quantity))
                                    obj.Quantity = _Quantity;
                                else
                                    obj.Quantity = 0;
                            }
                            else
                                obj.Quantity = 0;
                            //obj.Quantity = Convert.ToDouble(item[CommonUtility.ImportToolMasterColumn.Quantity.ToString()]);
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.Cost.ToString()))
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportToolMasterColumn.Cost.ToString()])))
                            {
                                double _Cost = 0;
                                if (double.TryParse(item[CommonUtility.ImportToolMasterColumn.Cost.ToString()].ToString(), out _Cost))
                                    obj.Cost = _Cost;
                                else
                                    obj.Cost = 0;
                            }
                            else
                                obj.Cost = 0;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.IsGroupOfItems.ToString()))
                        {
                            obj.IsGroupOfItems = item[CommonUtility.ImportToolMasterColumn.IsGroupOfItems.ToString()].ToString().ToLower() == "yes" ? 1 : 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.UDF1.ToString()))
                        {
                            obj.UDF1 = item[CommonUtility.ImportToolMasterColumn.UDF1.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.UDF2.ToString()))
                        {
                            obj.UDF2 = item[CommonUtility.ImportToolMasterColumn.UDF2.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.UDF3.ToString()))
                        {
                            obj.UDF3 = item[CommonUtility.ImportToolMasterColumn.UDF3.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.UDF4.ToString()))
                        {
                            obj.UDF4 = item[CommonUtility.ImportToolMasterColumn.UDF4.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.UDF5.ToString()))
                        {
                            obj.UDF5 = item[CommonUtility.ImportToolMasterColumn.UDF5.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.CheckOutUDF1.ToString()))
                        {
                            obj.CheckOutUDF1 = item[CommonUtility.ImportToolMasterColumn.CheckOutUDF1.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.CheckOutUDF2.ToString()))
                        {
                            obj.CheckOutUDF2 = item[CommonUtility.ImportToolMasterColumn.CheckOutUDF2.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.CheckOutUDF3.ToString()))
                        {
                            obj.CheckOutUDF3 = item[CommonUtility.ImportToolMasterColumn.CheckOutUDF3.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.CheckOutUDF4.ToString()))
                        {
                            obj.CheckOutUDF4 = item[CommonUtility.ImportToolMasterColumn.CheckOutUDF4.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.CheckOutUDF5.ToString()))
                        {
                            obj.CheckOutUDF5 = item[CommonUtility.ImportToolMasterColumn.CheckOutUDF5.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.Technician.ToString()))
                        {
                            obj.Technician = item[CommonUtility.ImportToolMasterColumn.Technician.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.CheckOutQuantity.ToString()))
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportToolMasterColumn.CheckOutQuantity.ToString()])))
                            {
                                //obj.CheckOutQuantity = Convert.ToDouble(item[CommonUtility.ImportToolMasterColumn.CheckOutQuantity.ToString()]);
                                double _CheckOutQuantityValue = 0;
                                if (double.TryParse(item[CommonUtility.ImportToolMasterColumn.CheckOutQuantity.ToString()].ToString(), out _CheckOutQuantityValue))
                                    obj.CheckOutQuantity = _CheckOutQuantityValue;
                                else
                                    obj.CheckOutQuantity = 0;
                            }
                            else
                            {
                                obj.CheckOutQuantity = 0;
                            }
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.CheckInQuantity.ToString()))
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportToolMasterColumn.CheckInQuantity.ToString()])))
                            {
                                double _CheckInQuantityValue = 0;
                                if (double.TryParse(item[CommonUtility.ImportToolMasterColumn.CheckInQuantity.ToString()].ToString(), out _CheckInQuantityValue))
                                    obj.CheckInQuantity = _CheckInQuantityValue;
                                else
                                    obj.CheckInQuantity = 0;
                            }
                            else
                            {
                                obj.CheckInQuantity = Convert.ToDouble(0);
                            }

                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.ImagePath.ToString()))
                        {
                            obj.ImagePath = item[CommonUtility.ImportToolMasterColumn.ImagePath.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.ToolImageExternalURL.ToString()))
                        {
                            obj.ToolImageExternalURL = item[CommonUtility.ImportToolMasterColumn.ToolImageExternalURL.ToString()].ToString();
                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [SUPPLIER MASTER]
        private bool GetSupplierMasterDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.SupplierMasterImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.SupplierMasterImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.SupplierMasterImport obj = new ImportMastersNewDTO.SupplierMasterImport();
                try
                {
                    if (item[CommonUtility.ImportSupplierColumn.SupplierName.ToString()].ToString() != ""
                        && item[CommonUtility.ImportSupplierColumn.SupplierColor.ToString()].ToString() != ""
                        //&& item[CommonUtility.ImportSupplierColumn.Contact.ToString()].ToString() != ""
                        //&& item[CommonUtility.ImportSupplierColumn.Phone.ToString()].ToString() != ""
                        )
                    {

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.SupplierName.ToString()))
                        {
                            obj.SupplierName = item[CommonUtility.ImportSupplierColumn.SupplierName.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.SupplierColor.ToString()))
                        {
                            obj.SupplierColor = item[CommonUtility.ImportSupplierColumn.SupplierColor.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.Description.ToString()))
                        {
                            obj.Description = item[CommonUtility.ImportSupplierColumn.Description.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.BranchNumber.ToString()))
                        {
                            obj.BranchNumber = item[CommonUtility.ImportSupplierColumn.BranchNumber.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.MaximumOrderSize.ToString()))
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportSupplierColumn.MaximumOrderSize.ToString()])))
                            {
                                int MaximumOrderSize;
                                int.TryParse(item[CommonUtility.ImportSupplierColumn.MaximumOrderSize.ToString()].ToString(), out MaximumOrderSize);
                                obj.MaximumOrderSize = MaximumOrderSize;

                            }
                            else
                                obj.MaximumOrderSize = null;
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.Address.ToString()))
                        {
                            obj.Address = item[CommonUtility.ImportSupplierColumn.Address.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.City.ToString()))
                        {
                            obj.City = item[CommonUtility.ImportSupplierColumn.City.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.State.ToString()))
                        {
                            obj.State = item[CommonUtility.ImportSupplierColumn.State.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.ZipCode.ToString()))
                        {
                            obj.ZipCode = item[CommonUtility.ImportSupplierColumn.ZipCode.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.Country.ToString()))
                        {
                            obj.Country = item[CommonUtility.ImportSupplierColumn.Country.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.Contact.ToString()))
                        {
                            obj.Contact = item[CommonUtility.ImportSupplierColumn.Contact.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.Phone.ToString()))
                        {
                            obj.Phone = item[CommonUtility.ImportSupplierColumn.Phone.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.Fax.ToString()))
                        {
                            obj.Fax = item[CommonUtility.ImportSupplierColumn.Fax.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.IsSendtoVendor.ToString()))
                        {
                            bool IsSendtoVendor = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.IsSendtoVendor.ToString()].ToString(), out IsSendtoVendor);
                            obj.IsSendtoVendor = IsSendtoVendor;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.IsVendorReturnAsn.ToString()))
                        {
                            bool IsVendorReturnAsn = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.IsVendorReturnAsn.ToString()].ToString(), out IsVendorReturnAsn);
                            obj.IsVendorReturnAsn = IsVendorReturnAsn;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.IsSupplierReceivesKitComponents.ToString()))
                        {
                            bool IsSupplierReceivesKitComponents = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.IsSupplierReceivesKitComponents.ToString()].ToString(), out IsSupplierReceivesKitComponents);
                            obj.IsSupplierReceivesKitComponents = IsSupplierReceivesKitComponents;
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.OrderNumberTypeBlank.ToString()))
                        {
                            bool OrderNumberTypeBlank = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.OrderNumberTypeBlank.ToString()].ToString(), out OrderNumberTypeBlank);
                            obj.OrderNumberTypeBlank = OrderNumberTypeBlank;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.OrderNumberTypeFixed.ToString()))
                        {
                            bool OrderNumberTypeFixed = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.OrderNumberTypeFixed.ToString()].ToString(), out OrderNumberTypeFixed);
                            obj.OrderNumberTypeFixed = OrderNumberTypeFixed;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.OrderNumberTypeBlanketOrderNumber.ToString()))
                        {
                            bool OrderNumberTypeBlanketOrderNumber = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.OrderNumberTypeBlanketOrderNumber.ToString()].ToString(), out OrderNumberTypeBlanketOrderNumber);
                            obj.OrderNumberTypeBlanketOrderNumber = OrderNumberTypeBlanketOrderNumber;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.OrderNumberTypeIncrementingOrderNumber.ToString()))
                        {
                            bool OrderNumberTypeIncrementingOrderNumber = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.OrderNumberTypeIncrementingOrderNumber.ToString()].ToString(), out OrderNumberTypeIncrementingOrderNumber);
                            obj.OrderNumberTypeIncrementingOrderNumber = OrderNumberTypeIncrementingOrderNumber;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.OrderNumberTypeIncrementingbyDay.ToString()))
                        {
                            bool OrderNumberTypeIncrementingbyDay = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.OrderNumberTypeIncrementingbyDay.ToString()].ToString(), out OrderNumberTypeIncrementingbyDay);
                            obj.OrderNumberTypeIncrementingbyDay = OrderNumberTypeIncrementingbyDay;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.OrderNumberTypeDateIncrementing.ToString()))
                        {
                            bool OrderNumberTypeDateIncrementing = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.OrderNumberTypeDateIncrementing.ToString()].ToString(), out OrderNumberTypeDateIncrementing);
                            obj.OrderNumberTypeDateIncrementing = OrderNumberTypeDateIncrementing;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.OrderNumberTypeDate.ToString()))
                        {
                            bool OrderNumberTypeDate = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.OrderNumberTypeDate.ToString()].ToString(), out OrderNumberTypeDate);
                            obj.OrderNumberTypeDate = OrderNumberTypeDate;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.UDF1.ToString()))
                        {
                            obj.UDF1 = item[CommonUtility.ImportSupplierColumn.UDF1.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.UDF2.ToString()))
                        {
                            obj.UDF2 = item[CommonUtility.ImportSupplierColumn.UDF2.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.UDF3.ToString()))
                        {
                            obj.UDF3 = item[CommonUtility.ImportSupplierColumn.UDF3.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.UDF4.ToString()))
                        {
                            obj.UDF4 = item[CommonUtility.ImportSupplierColumn.UDF4.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.UDF5.ToString()))
                        {
                            obj.UDF5 = item[CommonUtility.ImportSupplierColumn.UDF5.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.AccountNumber.ToString()))
                        {
                            obj.AccountNumber = item[CommonUtility.ImportSupplierColumn.AccountNumber.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.AccountName.ToString()))
                        {
                            obj.AccountName = item[CommonUtility.ImportSupplierColumn.AccountName.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.AccountAddress.ToString()))
                        {
                            obj.AccountAddress = item[CommonUtility.ImportSupplierColumn.AccountAddress.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.AccountCity.ToString()))
                        {
                            obj.AccountCity = item[CommonUtility.ImportSupplierColumn.AccountCity.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.AccountState.ToString()))
                        {
                            obj.AccountState = item[CommonUtility.ImportSupplierColumn.AccountState.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.AccountZip.ToString()))
                        {
                            obj.AccountZip = item[CommonUtility.ImportSupplierColumn.AccountZip.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.AccountIsDefault.ToString()))
                        {
                            bool AccountIsDefault = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.AccountIsDefault.ToString()].ToString(), out AccountIsDefault);
                            obj.AccountIsDefault = AccountIsDefault;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.BlanketPONumber.ToString()))
                        {
                            obj.BlanketPONumber = item[CommonUtility.ImportSupplierColumn.BlanketPONumber.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.StartDate.ToString()))
                        {
                            DateTime StartDate;
                            DateTime.TryParse(item[CommonUtility.ImportSupplierColumn.StartDate.ToString()].ToString(), out StartDate);
                            if (StartDate != DateTime.MinValue)
                                obj.StartDate = StartDate;
                            else
                                obj.StartDate = null;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.EndDate.ToString()))
                        {
                            DateTime EndDate;
                            DateTime.TryParse(item[CommonUtility.ImportSupplierColumn.EndDate.ToString()].ToString(), out EndDate);
                            if (EndDate != DateTime.MinValue)
                                obj.EndDate = EndDate;
                            else
                                obj.EndDate = null;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.MaxLimit.ToString()))
                        {
                            double MaxLimit;
                            double.TryParse(item[CommonUtility.ImportSupplierColumn.MaxLimit.ToString()].ToString(), out MaxLimit);
                            obj.MaxLimit = MaxLimit;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.DoNotExceed.ToString()))
                        {
                            bool DoNotExceed = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.DoNotExceed.ToString()].ToString(), out DoNotExceed);
                            obj.IsNotExceed = DoNotExceed;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.MaxLimitQty.ToString()))
                        {
                            double MaxLimitQty;
                            double.TryParse(item[CommonUtility.ImportSupplierColumn.MaxLimitQty.ToString()].ToString(), out MaxLimitQty);
                            obj.MaxLimitQty = MaxLimitQty;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.DoNotExceedQty.ToString()))
                        {
                            bool DoNotExceedQty = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.DoNotExceedQty.ToString()].ToString(), out DoNotExceedQty);
                            obj.IsNotExceedQty = DoNotExceedQty;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.PullPurchaseNumberFixed.ToString()))
                        {
                            bool PullPurchaseNumberFixed = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.PullPurchaseNumberFixed.ToString()].ToString(), out PullPurchaseNumberFixed);
                            obj.PullPurchaseNumberFixed = PullPurchaseNumberFixed;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.PullPurchaseNumberBlanketOrder.ToString()))
                        {
                            bool PullPurchaseNumberBlanketOrder = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.PullPurchaseNumberBlanketOrder.ToString()].ToString(), out PullPurchaseNumberBlanketOrder);
                            obj.PullPurchaseNumberBlanketOrder = PullPurchaseNumberBlanketOrder;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.PullPurchaseNumberDateIncrementing.ToString()))
                        {
                            bool PullPurchaseNumberDateIncrementing = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.PullPurchaseNumberDateIncrementing.ToString()].ToString(), out PullPurchaseNumberDateIncrementing);
                            obj.PullPurchaseNumberDateIncrementing = PullPurchaseNumberDateIncrementing;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.PullPurchaseNumberDate.ToString()))
                        {
                            bool PullPurchaseNumberDate = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.PullPurchaseNumberDate.ToString()].ToString(), out PullPurchaseNumberDate);
                            obj.PullPurchaseNumberDate = PullPurchaseNumberDate;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.LastPullPurchaseNumberUsed.ToString()))
                        {
                            obj.LastPullPurchaseNumberUsed = item[CommonUtility.ImportSupplierColumn.LastPullPurchaseNumberUsed.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.IsBlanketDeleted.ToString()))
                        {
                            bool IsBlanketDeleted = false;
                            bool.TryParse(item[CommonUtility.ImportSupplierColumn.IsBlanketDeleted.ToString()].ToString(), out IsBlanketDeleted);
                            obj.IsBlanketDeleted = IsBlanketDeleted;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.SupplierImage.ToString()))
                        {
                            obj.SupplierImage = item[CommonUtility.ImportSupplierColumn.SupplierImage.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.ImageExternalURL.ToString()))
                        {
                            obj.ImageExternalURL = item[CommonUtility.ImportSupplierColumn.ImageExternalURL.ToString()].ToString();
                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [QUICKLIST ITEMS]
        private bool GetQuickListItemsDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.QuickListItemsImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.QuickListItemsImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.QuickListItemsImport obj = new ImportMastersNewDTO.QuickListItemsImport();
                try
                {
                    if (item[CommonUtility.ImportQuickListItemsColumn.Name.ToString()].ToString() != "" && item[CommonUtility.ImportQuickListItemsColumn.ItemNumber.ToString()].ToString() != "")
                    {
                        // obj.ID = Convert.ToInt32(item[CommonUtility.ImportQuickListItemsColumn.ID.ToString()].ToString());
                        if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.Name.ToString()))
                        {
                            obj.QuickListname = Convert.ToString(item[CommonUtility.ImportQuickListItemsColumn.Name.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.Type.ToString()))
                        {
                            obj.Type = item[CommonUtility.ImportQuickListItemsColumn.Type.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.ItemNumber.ToString()))
                        {
                            obj.ItemNumber = item[CommonUtility.ImportQuickListItemsColumn.ItemNumber.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.BinNumber.ToString()))
                        {
                            obj.BinNumber = item[CommonUtility.ImportQuickListItemsColumn.BinNumber.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.Comment.ToString()))
                        {
                            obj.Comments = item[CommonUtility.ImportQuickListItemsColumn.Comment.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.Quantity.ToString()))
                        {
                            double _QtyValue = 0;
                            if (double.TryParse(item[CommonUtility.ImportQuickListItemsColumn.Quantity.ToString()].ToString(), out _QtyValue))
                                obj.Quantity = _QtyValue;
                            else
                                obj.Quantity = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.ConsignedQuantity.ToString()))
                        {
                            double _ConsignedQtyValue = 0;
                            if (double.TryParse(item[CommonUtility.ImportQuickListItemsColumn.ConsignedQuantity.ToString()].ToString(), out _ConsignedQtyValue))
                                obj.ConsignedQuantity = _ConsignedQtyValue;
                            else
                                obj.ConsignedQuantity = 0;
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF1.ToString()))
                        {
                            obj.UDF1 = Convert.ToString(item[CommonUtility.ImportCustomerColumn.UDF1.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF2.ToString()))
                        {
                            obj.UDF2 = Convert.ToString(item[CommonUtility.ImportCustomerColumn.UDF2.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF3.ToString()))
                        {
                            obj.UDF3 = Convert.ToString(item[CommonUtility.ImportCustomerColumn.UDF3.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF4.ToString()))
                        {
                            obj.UDF4 = Convert.ToString(item[CommonUtility.ImportCustomerColumn.UDF4.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF5.ToString()))
                        {
                            obj.UDF5 = Convert.ToString(item[CommonUtility.ImportCustomerColumn.UDF5.ToString()]);
                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [INVENTORY LOCATION IMPORT (MANUAL COUNT)]
        private bool GetInventoryLocationDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.InventoryLocationImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            lstImport = new List<ImportMastersNewDTO.InventoryLocationImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.InventoryLocationImport obj = new ImportMastersNewDTO.InventoryLocationImport();
                try
                {
                    if (item[CommonUtility.ImportInventoryLocationColumn.locationname.ToString()].ToString() != "" && item[CommonUtility.ImportInventoryLocationColumn.ItemNumber.ToString()].ToString() != "")
                    {
                        // obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryLocationColumn.ID.ToString()].ToString());
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.locationname.ToString()))
                        {
                            obj.BinNumber = Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.locationname.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ItemNumber.ToString()))
                        {
                            obj.ItemNumber = item[CommonUtility.ImportInventoryLocationColumn.ItemNumber.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ConsignedQuantity.ToString()))
                        {
                            if (item[CommonUtility.ImportInventoryLocationColumn.ConsignedQuantity.ToString()] == null || Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.ConsignedQuantity.ToString()]) == "")
                            {
                                obj.consignedquantity = 0;
                            }
                            else
                            {
                                double _consignedValue = 0.0;
                                if (double.TryParse(item[CommonUtility.ImportInventoryLocationColumn.ConsignedQuantity.ToString()].ToString(), out _consignedValue))
                                    obj.consignedquantity = _consignedValue;
                                else
                                    obj.consignedquantity = 0;
                            }

                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.CustomerOwnedQuantity.ToString()))
                        {
                            if (item[CommonUtility.ImportInventoryLocationColumn.CustomerOwnedQuantity.ToString()] == null || Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.CustomerOwnedQuantity.ToString()]) == "")
                            {
                                obj.customerownedquantity = 0;
                            }
                            else
                            {
                                double _customerOwnedQtyValue = 0.0;
                                if (double.TryParse(item[CommonUtility.ImportInventoryLocationColumn.CustomerOwnedQuantity.ToString()].ToString(), out _customerOwnedQtyValue))
                                    obj.customerownedquantity = _customerOwnedQtyValue;
                                else
                                    obj.customerownedquantity = 0;
                            }

                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.SerialNumber.ToString()))
                        {
                            obj.SerialNumber = item[CommonUtility.ImportInventoryLocationColumn.SerialNumber.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.LotNumber.ToString()))
                        {
                            obj.LotNumber = item[CommonUtility.ImportInventoryLocationColumn.LotNumber.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()))
                        {
                            if (item[CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()].ToString() != "")
                            {
                                DateTime dt;
                                DateTime.TryParseExact(item[CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()].ToString().Split(' ')[0], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);
                                if (dt != DateTime.MinValue)
                                {
                                    obj.Expiration = dt.ToString(SessionHelper.RoomDateFormat);
                                    obj.displayExpiration = dt.ToString(SessionHelper.RoomDateFormat);
                                }
                                else
                                {
                                    obj.Expiration = null;
                                    obj.displayExpiration = null;
                                    obj.Reason = "ExpirationDate should be in " + SessionHelper.RoomDateFormat + " format";
                                    obj.Status = "Fail";
                                }
                            }
                            else
                            {
                                obj.Expiration = null;
                                obj.displayExpiration = null;
                            }
                            //obj.Expiration = item[CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()].ToString();
                            //obj.displayExpiration = item[CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ReceivedDate.ToString()))
                        {
                            if (item[CommonUtility.ImportInventoryLocationColumn.ReceivedDate.ToString()].ToString() != "")
                            {
                                DateTime dt;
                                DateTime.TryParseExact(item[CommonUtility.ImportInventoryLocationColumn.ReceivedDate.ToString()].ToString().Split(' ')[0], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);
                                if (dt != DateTime.MinValue)
                                {
                                    obj.Received = dt.ToString(SessionHelper.RoomDateFormat);
                                }
                                else
                                {
                                    obj.Received = null;
                                    obj.Reason = "Receive date should be in " + SessionHelper.RoomDateFormat + " format";
                                    obj.Status = "Fail";
                                }
                            }
                            else
                            {
                                obj.Received = null;
                            }
                        }
                        if (objItemMasterDTO == null || obj.ItemNumber != objItemMasterDTO.ItemNumber)
                        {
                            objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetRecordByItemNumberLight(obj.ItemNumber, SessionHelper.RoomID, SessionHelper.CompanyID);
                        }
                        if (objItemMasterDTO != null && !objItemMasterDTO.Consignment)
                        {
                            obj.consignedquantity = 0;
                        }
                        if (objItemMasterDTO != null && objItemMasterDTO.SerialNumberTracking && objItemMasterDTO.DateCodeTracking)
                        {
                            obj.LotNumber = string.Empty;
                            obj.ItemGUID = objItemMasterDTO.GUID;
                            if (!objItemMasterDTO.Consignment)
                            {
                                obj.consignedquantity = 0;
                            }

                        }
                        else if (objItemMasterDTO != null && objItemMasterDTO.LotNumberTracking && objItemMasterDTO.DateCodeTracking)
                        {
                            obj.SerialNumber = string.Empty;
                        }
                        else if (objItemMasterDTO != null && objItemMasterDTO.LotNumberTracking)
                        {
                            obj.SerialNumber = string.Empty;
                        }
                        else if (objItemMasterDTO != null && objItemMasterDTO.SerialNumberTracking)
                        {
                            obj.LotNumber = string.Empty;

                        }
                        else if (objItemMasterDTO != null && objItemMasterDTO.DateCodeTracking)
                        {
                            obj.LotNumber = string.Empty;
                            obj.SerialNumber = string.Empty;
                        }
                        else
                        {
                            obj.LotNumber = string.Empty;
                            obj.SerialNumber = string.Empty;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ProjectSpend.ToString()))
                        {
                            obj.ProjectSpend = item[CommonUtility.ImportInventoryLocationColumn.ProjectSpend.ToString()].ToString();
                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }

                    List<ImportMastersNewDTO.InventoryLocationImport> lstImportNew = (from ilq in lstImport.ToList()
                                                                                      group ilq by new { ilq.BinNumber, ilq.Expiration, ilq.ItemNumber, ilq.LotNumber, ilq.SerialNumber, ilq.ItemGUID } into groupedilq
                                                                                      select new ImportMastersNewDTO.InventoryLocationImport
                                                                                      {
                                                                                          BinNumber = groupedilq.Key.BinNumber,
                                                                                          CompanyID = SessionHelper.CompanyID,
                                                                                          consignedquantity = groupedilq.Sum(t => (Convert.ToDouble(t.consignedquantity ?? 0))),
                                                                                          //Cost = groupedilq.Key.Cost,
                                                                                          Created = DateTime.UtcNow,
                                                                                          CreatedBy = SessionHelper.UserID,
                                                                                          customerownedquantity = groupedilq.Sum(t => (Convert.ToDouble(t.customerownedquantity ?? 0))),
                                                                                          Expiration = groupedilq.Key.Expiration,
                                                                                          GUID = Guid.NewGuid(),
                                                                                          InsertedFrom = "import",
                                                                                          IsArchived = false,
                                                                                          IsDeleted = false,
                                                                                          ItemGUID = groupedilq.Key.ItemGUID,
                                                                                          ItemNumber = groupedilq.Key.ItemNumber,
                                                                                          LastUpdatedBy = SessionHelper.UserID,
                                                                                          LotNumber = groupedilq.Key.LotNumber,
                                                                                          //  Received = groupedilq.Key.Received,
                                                                                          Room = SessionHelper.RoomID,
                                                                                          SerialNumber = groupedilq.Key.SerialNumber,
                                                                                          Updated = DateTime.UtcNow,
                                                                                          Status = string.Join(",", groupedilq.Select(t => t.Status).ToArray()),
                                                                                          Reason = string.Join(",", groupedilq.Select(t => t.Reason).ToArray()),

                                                                                      }).ToList();

                    lstImport = lstImportNew;
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [ADJUSTMENT COUNT IMPORT (BIN MASTER)]
        private bool GetAdjustmentCountBMDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.InventoryLocationImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            lstImport = new List<ImportMastersNewDTO.InventoryLocationImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.InventoryLocationImport obj = new ImportMastersNewDTO.InventoryLocationImport();
                try
                {
                    if (item[CommonUtility.ImportInventoryLocationColumn.locationname.ToString()].ToString() != "" && item[CommonUtility.ImportInventoryLocationColumn.ItemNumber.ToString()].ToString() != "")
                    {
                        // obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryLocationColumn.ID.ToString()].ToString());
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.locationname.ToString()))
                        {
                            obj.BinNumber = Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.locationname.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ItemNumber.ToString()))
                        {
                            obj.ItemNumber = item[CommonUtility.ImportInventoryLocationColumn.ItemNumber.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ConsignedQuantity.ToString()))
                        {
                            if (item[CommonUtility.ImportInventoryLocationColumn.ConsignedQuantity.ToString()] == null || Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.ConsignedQuantity.ToString()]) == "")
                            {
                                obj.consignedquantity = 0;
                            }
                            else
                            {
                                double _consignedValue = 0.0;
                                if (double.TryParse(item[CommonUtility.ImportInventoryLocationColumn.ConsignedQuantity.ToString()].ToString(), out _consignedValue))
                                    obj.consignedquantity = _consignedValue;
                                else
                                    obj.consignedquantity = 0;
                            }

                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.CustomerOwnedQuantity.ToString()))
                        {
                            if (item[CommonUtility.ImportInventoryLocationColumn.CustomerOwnedQuantity.ToString()] == null || Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.CustomerOwnedQuantity.ToString()]) == "")
                            {
                                obj.customerownedquantity = 0;
                            }
                            else
                            {
                                double _customerOwnedQtyValue = 0.0;
                                if (double.TryParse(item[CommonUtility.ImportInventoryLocationColumn.CustomerOwnedQuantity.ToString()].ToString(), out _customerOwnedQtyValue))
                                    obj.customerownedquantity = _customerOwnedQtyValue;
                                else
                                    obj.customerownedquantity = 0;
                            }

                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.SerialNumber.ToString()))
                        {
                            obj.SerialNumber = item[CommonUtility.ImportInventoryLocationColumn.SerialNumber.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.LotNumber.ToString()))
                        {
                            obj.LotNumber = item[CommonUtility.ImportInventoryLocationColumn.LotNumber.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()))
                        {
                            if (item[CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()].ToString() != "")
                            {
                                DateTime dt;
                                DateTime.TryParseExact(item[CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()].ToString().Split(' ')[0], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);
                                if (dt != DateTime.MinValue)
                                {
                                    obj.Expiration = dt.ToString(SessionHelper.RoomDateFormat);
                                    obj.displayExpiration = dt.ToString(SessionHelper.RoomDateFormat);
                                }
                                else
                                {
                                    obj.Expiration = null;
                                    obj.displayExpiration = null;
                                    obj.Reason = "ExpirationDate should be in " + SessionHelper.RoomDateFormat + " format";
                                    obj.Status = "Fail";
                                }
                            }
                            else
                            {
                                obj.Expiration = null;
                                obj.displayExpiration = null;
                            }
                            //obj.Expiration = item[CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()].ToString();
                            //obj.displayExpiration = item[CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ReceivedDate.ToString()))
                        {
                            if (item[CommonUtility.ImportInventoryLocationColumn.ReceivedDate.ToString()].ToString() != "")
                            {
                                DateTime dt;
                                DateTime.TryParseExact(item[CommonUtility.ImportInventoryLocationColumn.ReceivedDate.ToString()].ToString().Split(' ')[0], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);
                                if (dt != DateTime.MinValue)
                                {
                                    obj.Received = dt.ToString(SessionHelper.RoomDateFormat);
                                }
                                else
                                {
                                    obj.Received = null;
                                    obj.Reason = "Receive date should be in " + SessionHelper.RoomDateFormat + " format";
                                    obj.Status = "Fail";
                                }
                            }
                            else
                            {
                                obj.Received = null;
                            }
                        }
                        if (objItemMasterDTO == null || obj.ItemNumber != objItemMasterDTO.ItemNumber)
                        {
                            objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetRecordByItemNumberLight(obj.ItemNumber, SessionHelper.RoomID, SessionHelper.CompanyID);
                        }
                        if (objItemMasterDTO != null && !objItemMasterDTO.Consignment)
                        {
                            obj.consignedquantity = 0;
                        }
                        if (objItemMasterDTO != null && objItemMasterDTO.SerialNumberTracking && objItemMasterDTO.DateCodeTracking)
                        {
                            obj.LotNumber = string.Empty;
                            obj.ItemGUID = objItemMasterDTO.GUID;
                            if (!objItemMasterDTO.Consignment)
                            {
                                obj.consignedquantity = 0;
                            }

                        }
                        else if (objItemMasterDTO != null && objItemMasterDTO.LotNumberTracking && objItemMasterDTO.DateCodeTracking)
                        {
                            obj.SerialNumber = string.Empty;
                        }
                        else if (objItemMasterDTO != null && objItemMasterDTO.LotNumberTracking)
                        {
                            obj.SerialNumber = string.Empty;
                        }
                        else if (objItemMasterDTO != null && objItemMasterDTO.SerialNumberTracking)
                        {
                            obj.LotNumber = string.Empty;

                        }
                        else if (objItemMasterDTO != null && objItemMasterDTO.DateCodeTracking)
                        {
                            obj.LotNumber = string.Empty;
                            obj.SerialNumber = string.Empty;
                        }
                        else
                        {
                            obj.LotNumber = string.Empty;
                            obj.SerialNumber = string.Empty;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ProjectSpend.ToString()))
                        {
                            obj.ProjectSpend = item[CommonUtility.ImportInventoryLocationColumn.ProjectSpend.ToString()].ToString();
                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }

                    List<ImportMastersNewDTO.InventoryLocationImport> lstImportNew = (from ilq in lstImport.ToList()
                                                                                      group ilq by new { ilq.BinNumber, ilq.Expiration, ilq.ItemNumber, ilq.LotNumber, ilq.SerialNumber, ilq.ItemGUID } into groupedilq
                                                                                      select new ImportMastersNewDTO.InventoryLocationImport
                                                                                      {
                                                                                          BinNumber = groupedilq.Key.BinNumber,
                                                                                          CompanyID = SessionHelper.CompanyID,
                                                                                          consignedquantity = groupedilq.Sum(t => (Convert.ToDouble(t.consignedquantity ?? 0))),
                                                                                          //Cost = groupedilq.Key.Cost,
                                                                                          Created = DateTime.UtcNow,
                                                                                          CreatedBy = SessionHelper.UserID,
                                                                                          customerownedquantity = groupedilq.Sum(t => (Convert.ToDouble(t.customerownedquantity ?? 0))),
                                                                                          Expiration = groupedilq.Key.Expiration,
                                                                                          GUID = Guid.NewGuid(),
                                                                                          InsertedFrom = "import",
                                                                                          IsArchived = false,
                                                                                          IsDeleted = false,
                                                                                          ItemGUID = groupedilq.Key.ItemGUID,
                                                                                          ItemNumber = groupedilq.Key.ItemNumber,
                                                                                          LastUpdatedBy = SessionHelper.UserID,
                                                                                          LotNumber = groupedilq.Key.LotNumber,
                                                                                          //  Received = groupedilq.Key.Received,
                                                                                          Room = SessionHelper.RoomID,
                                                                                          SerialNumber = groupedilq.Key.SerialNumber,
                                                                                          Updated = DateTime.UtcNow,
                                                                                          Status = string.Join(",", groupedilq.Select(t => t.Status).ToArray()),
                                                                                          Reason = string.Join(",", groupedilq.Select(t => t.Reason).ToArray()),

                                                                                      }).ToList();

                    lstImport = lstImportNew;
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [WORK ORDER]
        private bool GetWorkOrderDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.WorkOrderImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.WorkOrderImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.WorkOrderImport obj = new ImportMastersNewDTO.WorkOrderImport();
                try
                {
                    if (item[CommonUtility.WorkOrderColumn.WOName.ToString()].ToString() != "")
                    {
                        // obj.ID = Convert.ToInt32(item[CommonUtility.WorkOrderColumn.ID.ToString()].ToString());
                        if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.WOName.ToString()))
                        {
                            obj.WOName = item[CommonUtility.WorkOrderColumn.WOName.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.ReleaseNumber.ToString()))
                        {
                            obj.ReleaseNumber = item[CommonUtility.WorkOrderColumn.ReleaseNumber.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.WOStatus.ToString()))
                        {
                            obj.WOStatus = item[CommonUtility.WorkOrderColumn.WOStatus.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.Technician.ToString()))
                        {
                            obj.Technician = item[CommonUtility.WorkOrderColumn.Technician.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.Customer.ToString()))
                        {
                            obj.Customer = item[CommonUtility.WorkOrderColumn.Customer.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.UDF1.ToString()))
                        {
                            obj.UDF1 = item[CommonUtility.WorkOrderColumn.UDF1.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.UDF2.ToString()))
                        {
                            obj.UDF2 = item[CommonUtility.WorkOrderColumn.UDF2.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.UDF3.ToString()))
                        {
                            obj.UDF3 = item[CommonUtility.WorkOrderColumn.UDF3.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.UDF4.ToString()))
                        {
                            obj.UDF4 = item[CommonUtility.WorkOrderColumn.UDF4.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.UDF5.ToString()))
                        {
                            obj.UDF5 = item[CommonUtility.WorkOrderColumn.UDF5.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.WOType.ToString()))
                        {
                            obj.WOType = item[CommonUtility.WorkOrderColumn.WOType.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.Description.ToString()))
                        {
                            obj.Description = item[CommonUtility.WorkOrderColumn.Description.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.SupplierName.ToString()))
                        {
                            obj.SupplierName = item[CommonUtility.WorkOrderColumn.SupplierName.ToString()].ToString();
                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [BOM ITEM MASTER]
        private bool GetBOMItemMasterDataFromFile(HttpPostedFileBase uploadFile, string vDefaultSupplier, string vDefaultUOM, string vDefaultLocation, string ImportModule,
                                                   out List<ImportMastersNewDTO.BOMItemMasterImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.BOMItemMasterImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.BOMItemMasterImport obj = new ImportMastersNewDTO.BOMItemMasterImport();
                try
                {
                    //bool AllowInsert = true;
                    if (item[CommonUtility.ImportItemColumn.ItemNumber.ToString()].ToString() != "")
                    //  AllowInsert = lstImport.Where(x => x.ItemNumber == item[CommonUtility.ImportItemColumn.ItemNumber.ToString()].ToString()).ToList().Count > 0 ? false : true;

                    //if (AllowInsert == true)
                    {
                        // obj.ID = Convert.ToInt32(item[CommonUtility.ImportItemColumn.ID.ToString()].ToString());

                        obj.ItemNumber = Convert.ToString(item[CommonUtility.ImportItemColumn.ItemNumber.ToString()]);

                        //Int64 ManufacturerID;
                        //Int64.TryParse(item[CommonUtility.ImportItemColumn.ManufacturerID.ToString()].ToString(), out ManufacturerID);
                        //obj.ManufacturerID = ManufacturerID;
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Manufacturer.ToString()))
                        {
                            obj.ManufacturerName = Convert.ToString(item[CommonUtility.ImportItemColumn.Manufacturer.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ManufacturerNumber.ToString()))
                        {
                            obj.ManufacturerNumber = Convert.ToString(item[CommonUtility.ImportItemColumn.ManufacturerNumber.ToString()]);
                        }



                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SupplierName.ToString()))
                        {
                            obj.SupplierName = Convert.ToString(item[CommonUtility.ImportItemColumn.SupplierName.ToString()]);

                            string ItemTypeName = string.Empty;
                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemType.ToString()))
                            {
                                ItemTypeName = item[CommonUtility.ImportItemColumn.ItemType.ToString()].ToString();
                            }

                            if (string.IsNullOrEmpty(obj.SupplierName) && ItemTypeName != "Labor")
                            {
                                obj.SupplierName = vDefaultSupplier;
                            }
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SupplierPartNo.ToString()))
                        {
                            obj.SupplierPartNo = Convert.ToString(item[CommonUtility.ImportItemColumn.SupplierPartNo.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UPC.ToString()))
                        {
                            obj.UPC = Convert.ToString(item[CommonUtility.ImportItemColumn.UPC.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UNSPSC.ToString()))
                        {
                            obj.UNSPSC = Convert.ToString(item[CommonUtility.ImportItemColumn.UNSPSC.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Description.ToString()))
                        {
                            obj.Description = Convert.ToString(item[CommonUtility.ImportItemColumn.Description.ToString()]);
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.CategoryName.ToString()))
                        {
                            obj.CategoryName = Convert.ToString(item[CommonUtility.ImportItemColumn.CategoryName.ToString()]);
                        }



                        //Int64 GLAccountID;
                        //Int64.TryParse(item[CommonUtility.ImportItemColumn.GLAccountID.ToString()].ToString(), out GLAccountID);
                        //obj.GLAccountID = GLAccountID;
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.GLAccount.ToString()))
                        {
                            obj.GLAccount = Convert.ToString(item[CommonUtility.ImportItemColumn.GLAccount.ToString()]);
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Unit.ToString()))
                        {
                            obj.Unit = Convert.ToString(item[CommonUtility.ImportItemColumn.Unit.ToString()]);

                            if (string.IsNullOrEmpty(obj.Unit))
                            {
                                obj.Unit = vDefaultUOM;
                            }
                        }




                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.LeadTimeInDays.ToString()))
                        {
                            Int32 LeadTimeInDays;
                            if (Int32.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.LeadTimeInDays.ToString()]), out LeadTimeInDays))
                                obj.LeadTimeInDays = LeadTimeInDays;
                            else
                                obj.LeadTimeInDays = 0;

                        }



                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Taxable.ToString()))
                        {
                            Boolean Taxable = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Taxable.ToString()]), out Taxable))
                                obj.Taxable = Taxable;
                            else
                                obj.Taxable = false;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Consignment.ToString()))
                        {
                            Boolean Consignment = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Consignment.ToString()]), out Consignment))
                                obj.Consignment = Consignment;
                            else
                                obj.Consignment = false;
                        }



                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemUniqueNumber.ToString()))
                        {
                            obj.ItemUniqueNumber = Convert.ToString(item[CommonUtility.ImportItemColumn.ItemUniqueNumber.ToString()]);
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsTransfer.ToString()))
                        {
                            Boolean IsTransfer = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsTransfer.ToString()]), out IsTransfer))
                                obj.IsTransfer = IsTransfer;
                            else
                                obj.IsTransfer = false;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsPurchase.ToString()))
                        {
                            Boolean IsPurchase = false;
                            if (string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportItemColumn.IsPurchase.ToString()])))
                            {
                                if (obj.IsTransfer == null || obj.IsTransfer == false)
                                {
                                    obj.IsPurchase = true;
                                }
                            }
                            else
                            {
                                if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsPurchase.ToString()]), out IsPurchase))
                                {
                                    if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsTransfer.ToString()))
                                    {
                                        if (Convert.ToString(item[CommonUtility.ImportItemColumn.IsTransfer.ToString()]) == "" && Convert.ToString(item[CommonUtility.ImportItemColumn.IsPurchase.ToString()]) == "")
                                        {
                                            obj.IsPurchase = true;
                                        }
                                        else
                                        {
                                            obj.IsPurchase = IsPurchase;
                                        }
                                    }
                                    else if (Convert.ToString(item[CommonUtility.ImportItemColumn.IsPurchase.ToString()]) == "")
                                    {
                                        obj.IsPurchase = true;
                                    }
                                    else
                                    {
                                        obj.IsPurchase = IsPurchase;
                                    }
                                }
                                else
                                {
                                    if (obj.IsTransfer == null || obj.IsTransfer == false)
                                    {
                                        obj.IsPurchase = true;
                                    }
                                    else
                                    {
                                        obj.IsPurchase = false;
                                    }
                                }
                            }
                        }


                        //Int64 DefaultLocation;
                        //Int64.TryParse(item[CommonUtility.ImportItemColumn.DefaultLocation.ToString()].ToString(), out DefaultLocation);
                        //obj.DefaultLocation = DefaultLocation;
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.InventryLocation.ToString()))
                        {
                            obj.InventryLocation = Convert.ToString(item[CommonUtility.ImportItemColumn.InventryLocation.ToString()]);

                            if (string.IsNullOrEmpty(obj.InventryLocation))
                            {
                                obj.InventryLocation = vDefaultLocation;
                            }
                        }


                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.InventoryClassification.ToString()))
                        {
                            string InventoryClassification;
                            InventoryClassification = Convert.ToString(item[CommonUtility.ImportItemColumn.InventoryClassification.ToString()]);
                            obj.InventoryClassificationName = InventoryClassification;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SerialNumberTracking.ToString()))
                        {
                            Boolean SerialNumberTracking = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.SerialNumberTracking.ToString()]), out SerialNumberTracking))
                                obj.SerialNumberTracking = SerialNumberTracking;
                            else
                                obj.SerialNumberTracking = false;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.LotNumberTracking.ToString()))
                        {
                            Boolean LotNumberTracking = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.LotNumberTracking.ToString()]), out LotNumberTracking))
                                obj.LotNumberTracking = LotNumberTracking;
                            else
                                obj.LotNumberTracking = false;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.DateCodeTracking.ToString()))
                        {
                            Boolean DateCodeTracking = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.DateCodeTracking.ToString()]), out DateCodeTracking))
                                obj.DateCodeTracking = DateCodeTracking;
                            else
                                obj.DateCodeTracking = false;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemType.ToString()))
                        {
                            obj.ItemTypeName = item[CommonUtility.ImportItemColumn.ItemType.ToString()].ToString();
                        }

                        obj.Status = "";
                        obj.Reason = "";
                        obj.Index = list.IndexOf(item);

                        //obj.IsLotSerialExpiryCost = false;
                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [KIT DETAIL]
        private bool GetKitDetailDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.KitDetailImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.KitDetailImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.KitDetailImport obj = new ImportMastersNewDTO.KitDetailImport();
                try
                {
                    //bool AllowInsert = true;
                    if (item[CommonUtility.ImportKitsItemsColumn.ItemNumber.ToString()].ToString() != ""
                                            && item[CommonUtility.ImportKitsItemsColumn.KitPartNumber.ToString()].ToString() != ""
                                            && item[CommonUtility.ImportKitsItemsColumn.QuantityPerKit.ToString()].ToString() != ""
                                            && item[CommonUtility.ImportKitsItemsColumn.SupplierName.ToString()].ToString() != ""
                                            && item[CommonUtility.ImportKitsItemsColumn.SupplierPartNo.ToString()].ToString() != ""
                                            && item[CommonUtility.ImportKitsItemsColumn.DefaultLocation.ToString()].ToString() != ""
                                            && item[CommonUtility.ImportKitsItemsColumn.CostUOMName.ToString()].ToString() != ""
                                            && item[CommonUtility.ImportKitsItemsColumn.UOM.ToString()].ToString() != ""
                                            && item[CommonUtility.ImportKitsItemsColumn.DefaultReorderQuantity.ToString()].ToString() != ""
                                            && item[CommonUtility.ImportKitsItemsColumn.DefaultPullQuantity.ToString()].ToString() != ""

                                            && item[CommonUtility.ImportKitsItemsColumn.IsItemLevelMinMaxQtyRequired.ToString()].ToString() != ""
                                            )
                    {
                        // obj.ID = Convert.ToInt32(item[CommonUtility.ImportKitsItemsColumn.ID.ToString()].ToString());


                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.KitPartNumber.ToString()))
                        {
                            obj.KitPartNumber = item[CommonUtility.ImportKitsItemsColumn.KitPartNumber.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.ItemNumber.ToString()))
                        {
                            obj.ItemNumber = item[CommonUtility.ImportKitsItemsColumn.ItemNumber.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.QuantityPerKit.ToString()))
                        {
                            double QuantityPerKitValue = 0;
                            if (double.TryParse(item[CommonUtility.ImportKitsItemsColumn.QuantityPerKit.ToString()].ToString(), out QuantityPerKitValue))
                                obj.QuantityPerKit = QuantityPerKitValue;
                            else
                                obj.QuantityPerKit = 0;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.OnHandQuantity.ToString()))
                        {
                            double OnHandQuantity = 0;
                            if (double.TryParse(item[CommonUtility.ImportKitsItemsColumn.OnHandQuantity.ToString()].ToString(), out OnHandQuantity))
                                obj.OnHandQuantity = OnHandQuantity;
                            else
                                obj.OnHandQuantity = 0;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.IsDeleted.ToString()))
                        {
                            Boolean IsDeleted = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportKitsItemsColumn.IsDeleted.ToString()]), out IsDeleted))
                                obj.IsDeleted = IsDeleted;
                            else
                                obj.IsDeleted = false;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.IsBuildBreak.ToString()))
                        {
                            Boolean IsBuildBreak = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportKitsItemsColumn.IsBuildBreak.ToString()]), out IsBuildBreak))
                                obj.IsBuildBreak = IsBuildBreak;
                            else
                                obj.IsBuildBreak = false;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.AvailableKitQuantity.ToString()))
                        {
                            double AvailableKitQuantity = 0;
                            if (double.TryParse(item[CommonUtility.ImportKitsItemsColumn.AvailableKitQuantity.ToString()].ToString(), out AvailableKitQuantity))
                                obj.AvailableKitQuantity = AvailableKitQuantity;
                            else
                                obj.AvailableKitQuantity = 0;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.Description.ToString()))
                        {
                            obj.Description = item[CommonUtility.ImportKitsItemsColumn.Description.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.CriticalQuantity.ToString()))
                        {
                            double CriticalQuantity = 0;
                            if (double.TryParse(item[CommonUtility.ImportKitsItemsColumn.CriticalQuantity.ToString()].ToString(), out CriticalQuantity))
                                obj.CriticalQuantity = CriticalQuantity;
                            else
                                obj.CriticalQuantity = 0;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.MinimumQuantity.ToString()))
                        {
                            double MinimumQuantity = 0;
                            if (double.TryParse(item[CommonUtility.ImportKitsItemsColumn.MinimumQuantity.ToString()].ToString(), out MinimumQuantity))
                                obj.MinimumQuantity = MinimumQuantity;
                            else
                                obj.MinimumQuantity = 0;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.MaximumQuantity.ToString()))
                        {
                            double MaximumQuantity = 0;
                            if (double.TryParse(item[CommonUtility.ImportKitsItemsColumn.MaximumQuantity.ToString()].ToString(), out MaximumQuantity))
                                obj.MaximumQuantity = MaximumQuantity;
                            else
                                obj.MaximumQuantity = 0;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.ReOrderType.ToString()))
                        {
                            obj.ReOrderType = item[CommonUtility.ImportKitsItemsColumn.ReOrderType.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.SupplierName.ToString()))
                        {
                            obj.SupplierName = item[CommonUtility.ImportKitsItemsColumn.SupplierName.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.SupplierPartNo.ToString()))
                        {
                            obj.SupplierPartNo = item[CommonUtility.ImportKitsItemsColumn.SupplierPartNo.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.DefaultLocation.ToString()))
                        {
                            obj.DefaultLocationName = item[CommonUtility.ImportKitsItemsColumn.DefaultLocation.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.CostUOMName.ToString()))
                        {
                            obj.CostUOMName = item[CommonUtility.ImportKitsItemsColumn.CostUOMName.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.UOM.ToString()))
                        {
                            obj.UOM = item[CommonUtility.ImportKitsItemsColumn.UOM.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.DefaultReorderQuantity.ToString()))
                        {
                            // obj.DefaultReorderQuantity = item[CommonUtility.ImportKitsItemsColumn.DefaultReorderQuantity.ToString()].ToString();
                            double DefaultReorderQuantity = 0;
                            if (double.TryParse(item[CommonUtility.ImportKitsItemsColumn.DefaultReorderQuantity.ToString()].ToString(), out DefaultReorderQuantity))
                                obj.DefaultReorderQuantity = DefaultReorderQuantity;
                            else
                                obj.DefaultReorderQuantity = 0;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.DefaultPullQuantity.ToString()))
                        {
                            // obj.DefaultReorderQuantity = item[CommonUtility.ImportKitsItemsColumn.DefaultReorderQuantity.ToString()].ToString();
                            double DefaultPullQuantity = 0;
                            if (double.TryParse(item[CommonUtility.ImportKitsItemsColumn.DefaultPullQuantity.ToString()].ToString(), out DefaultPullQuantity))
                                obj.DefaultPullQuantity = DefaultPullQuantity;
                            else
                                obj.DefaultPullQuantity = 0;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.IsItemLevelMinMaxQtyRequired.ToString()))
                        {
                            bool IsItemLevelMinMaxQtyRequired = true;
                            if (bool.TryParse(item[CommonUtility.ImportKitsItemsColumn.IsItemLevelMinMaxQtyRequired.ToString()].ToString(), out IsItemLevelMinMaxQtyRequired))
                                obj.IsItemLevelMinMaxQtyRequired = IsItemLevelMinMaxQtyRequired;
                            else
                                obj.IsItemLevelMinMaxQtyRequired = true;

                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.SerialNumberTracking.ToString()))
                        {
                            Boolean IsSerialNumber = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.SerialNumberTracking.ToString()]), out IsSerialNumber))
                                obj.SerialNumberTracking = IsSerialNumber;
                            else
                                obj.SerialNumberTracking = false;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.LotNumberTracking.ToString()))
                        {
                            Boolean IsLotNumberActive = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.LotNumberTracking.ToString()]), out IsLotNumberActive))
                                obj.LotNumberTracking = IsLotNumberActive;
                            else
                                obj.LotNumberTracking = false;
                        }
                        if (obj.LotNumberTracking == true)
                        {
                            obj.SerialNumberTracking = false;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.DateCodeTracking.ToString()))
                        {
                            Boolean IsExpirationDateActive = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.DateCodeTracking.ToString()]), out IsExpirationDateActive))
                                obj.DateCodeTracking = IsExpirationDateActive;
                            else
                                obj.DateCodeTracking = false;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.IsActive.ToString()))
                        {
                            Boolean IsActive = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsActive.ToString()]), out IsActive))
                                obj.IsActive = IsActive;
                            else
                                obj.IsActive = false;
                        }

                        obj.Status = "";
                        obj.Reason = "";
                        obj.Index = list.IndexOf(item);

                        //obj.IsLotSerialExpiryCost = false;
                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [PROJECT MASTER]
        private bool GetProjectMasterDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.ProjectMasterImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.ProjectMasterImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.ProjectMasterImport obj = new ImportMastersNewDTO.ProjectMasterImport();
                try
                {
                    if (item[CommonUtility.ImportProjectMaster.ProjectSpendName.ToString()].ToString() != "" && item[CommonUtility.ImportProjectMaster.DollarLimitAmount.ToString()].ToString() != "" && item[CommonUtility.ImportProjectMaster.ItemNumber.ToString()].ToString() != "")
                    {
                        // obj.ID = Convert.ToInt32(item[CommonUtility.ImportQuickListItemsColumn.ID.ToString()].ToString());

                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.ProjectSpendName.ToString()))
                        {
                            obj.ProjectSpendName = item[CommonUtility.ImportProjectMaster.ProjectSpendName.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.Description.ToString()))
                        {
                            obj.Description = item[CommonUtility.ImportProjectMaster.Description.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.ItemNumber.ToString()))
                        {
                            obj.ItemNumber = item[CommonUtility.ImportProjectMaster.ItemNumber.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.DollarLimitAmount.ToString()))
                        {
                            decimal Tempdouble = 0;
                            decimal.TryParse(item[CommonUtility.ImportProjectMaster.DollarLimitAmount.ToString()].ToString(), out Tempdouble);
                            obj.DollarLimitAmount = Tempdouble;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.TrackAllUsageAgainstThis.ToString()))
                        {
                            bool Tempbool = false;
                            bool.TryParse(item[CommonUtility.ImportProjectMaster.TrackAllUsageAgainstThis.ToString()].ToString(), out Tempbool);
                            obj.TrackAllUsageAgainstThis = Tempbool;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.IsClosed.ToString()))
                        {
                            bool Tempbool = false;
                            bool.TryParse(item[CommonUtility.ImportProjectMaster.IsClosed.ToString()].ToString(), out Tempbool);
                            obj.IsClosed = Tempbool;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.IsDeleted.ToString()))
                        {
                            bool Tempbool = false;
                            bool.TryParse(item[CommonUtility.ImportProjectMaster.IsDeleted.ToString()].ToString(), out Tempbool);
                            obj.IsDeleted = Tempbool;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.UDF1.ToString()))
                        {
                            obj.UDF1 = item[CommonUtility.ImportProjectMaster.UDF1.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.UDF2.ToString()))
                        {
                            obj.UDF2 = item[CommonUtility.ImportProjectMaster.UDF2.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.UDF3.ToString()))
                        {
                            obj.UDF3 = item[CommonUtility.ImportProjectMaster.UDF3.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.UDF4.ToString()))
                        {
                            obj.UDF4 = item[CommonUtility.ImportProjectMaster.UDF4.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.UDF5.ToString()))
                        {
                            obj.UDF5 = item[CommonUtility.ImportProjectMaster.UDF5.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.ItemDollarLimitAmount.ToString()))
                        {
                            decimal Tempdouble = 0;
                            decimal.TryParse(item[CommonUtility.ImportProjectMaster.ItemDollarLimitAmount.ToString()].ToString(), out Tempdouble);
                            obj.ItemDollarLimitAmount = Tempdouble;
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.ItemQuantityLimitAmount.ToString()))
                        {
                            double Tempdouble = 0;
                            double.TryParse(item[CommonUtility.ImportProjectMaster.ItemQuantityLimitAmount.ToString()].ToString(), out Tempdouble);
                            obj.ItemQuantityLimitAmount = Tempdouble;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportProjectMaster.IsLineItemDeleted.ToString()))
                        {
                            bool Tempbool = false;
                            bool.TryParse(item[CommonUtility.ImportProjectMaster.IsLineItemDeleted.ToString()].ToString(), out Tempbool);
                            obj.IsLineItemDeleted = Tempbool;
                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [ITEM LOCATION eVMI (INVENTORY LOCATION QUANTITY)]
        private bool GetItemLocationseVMIDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.InventoryLocationQuantityImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.InventoryLocationQuantityImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.InventoryLocationQuantityImport obj = new ImportMastersNewDTO.InventoryLocationQuantityImport();
                try
                {
                    if (item[CommonUtility.ImportInventoryLocationColumn.locationname.ToString()].ToString() != "" && item[CommonUtility.ImportInventoryLocationColumn.ItemNumber.ToString()].ToString() != "")
                    {

                        //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryLocationColumn.ID.ToString()].ToString());
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.locationname.ToString()))
                        {
                            obj.BinNumber = item[CommonUtility.ImportInventoryLocationColumn.locationname.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ItemNumber.ToString()))
                        {
                            obj.ItemNumber = item[CommonUtility.ImportInventoryLocationColumn.ItemNumber.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.MinimumQuantity.ToString()))
                        {
                            if (item[CommonUtility.ImportInventoryLocationColumn.MinimumQuantity.ToString()] == null || Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.MinimumQuantity.ToString()]) == "")
                            {
                                obj.MinimumQuantity = 0;
                            }
                            else
                            {
                                //Wi-1190 (exception throw when "N/A" value pass in MinimumQuantity
                                string strMinQty = string.Empty;
                                double MinQtyValue = 0;
                                strMinQty = item[CommonUtility.ImportInventoryLocationColumn.MinimumQuantity.ToString()].ToString();

                                if (Double.TryParse(strMinQty, out MinQtyValue))
                                    obj.MinimumQuantity = MinQtyValue;
                                else
                                    obj.MinimumQuantity = 0;
                                //obj.MinimumQuantity = Convert.ToDouble(item[CommonUtility.ImportInventoryLocationColumn.MinimumQuantity.ToString()].ToString());
                            }

                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.MaximumQuantity.ToString()))
                        {
                            if (item[CommonUtility.ImportInventoryLocationColumn.MaximumQuantity.ToString()] == null || Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.MaximumQuantity.ToString()]) == "")
                            {
                                obj.MaximumQuantity = 0;
                            }
                            else
                            {
                                //Wi-1190 (exception throw when "N/A" value pass in MaximumQuantity
                                string strMaxQty = string.Empty;
                                double MaxQtyValue = 0;
                                strMaxQty = item[CommonUtility.ImportInventoryLocationColumn.MaximumQuantity.ToString()].ToString();

                                if (Double.TryParse(strMaxQty, out MaxQtyValue))
                                    obj.MaximumQuantity = MaxQtyValue;
                                else
                                    obj.MaximumQuantity = 0;
                                //obj.MaximumQuantity = Convert.ToDouble(item[CommonUtility.ImportInventoryLocationColumn.MaximumQuantity.ToString()].ToString());
                            }

                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.CriticalQuantity.ToString()))
                        {
                            if (item[CommonUtility.ImportInventoryLocationColumn.CriticalQuantity.ToString()] == null || Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.CriticalQuantity.ToString()]) == "")
                            {
                                obj.CriticalQuantity = 0;
                            }
                            else
                            {
                                //Wi-1190 (exception throw when "N/A" value pass in Critical Qty
                                string strCriticalQty = string.Empty;
                                double CriQtyValue = 0;
                                strCriticalQty = item[CommonUtility.ImportInventoryLocationColumn.CriticalQuantity.ToString()].ToString();

                                if (Double.TryParse(strCriticalQty, out CriQtyValue))
                                    obj.CriticalQuantity = CriQtyValue;
                                else
                                    obj.CriticalQuantity = 0;
                                //obj.CriticalQuantity = Convert.ToDouble(item[CommonUtility.ImportInventoryLocationColumn.CriticalQuantity.ToString()].ToString());
                            }

                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.SensorId.ToString()))
                        {
                            if (Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.SensorId.ToString()]) == "" || item[CommonUtility.ImportInventoryLocationColumn.SensorId.ToString()] == null)
                            {
                                obj.SensorId = 0;
                            }
                            else
                            {
                                double SensorIdValue = 0;
                                if (Double.TryParse(item[CommonUtility.ImportInventoryLocationColumn.SensorId.ToString()].ToString(), out SensorIdValue))
                                    obj.SensorId = SensorIdValue;
                                else
                                    obj.SensorId = 0;
                            }

                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.SensorPort.ToString()))
                        {
                            obj.SensorPort = item[CommonUtility.ImportInventoryLocationColumn.SensorPort.ToString()].ToString();
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.IsDefault.ToString()))
                        {
                            Boolean IsDefault = false;
                            Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.IsDefault.ToString()]), out IsDefault);
                            obj.IsDefault = IsDefault;
                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.IsDeleted.ToString()))
                        {
                            Boolean IsDeleted = false;
                            Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.IsDeleted.ToString()]), out IsDeleted);
                            obj.IsDeleted = IsDeleted;
                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [ITEM MANUFACTURER]
        private bool GetItemManufacturerDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.ItemManufacturerImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.ItemManufacturerImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.ItemManufacturerImport obj = new ImportMastersNewDTO.ItemManufacturerImport();
                try
                {
                    if (item[CommonUtility.ImportItemManufacturer.ManufacturerName.ToString()].ToString() != "")
                    {
                        // obj.ID = Convert.ToInt32(item[CommonUtility.ImportItemManufacturer.ID.ToString()].ToString());
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemManufacturer.ItemNumber.ToString()))
                        {
                            obj.ItemNumber = item[CommonUtility.ImportItemManufacturer.ItemNumber.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemManufacturer.ManufacturerName.ToString()))
                        {
                            obj.ManufacturerName = item[CommonUtility.ImportItemManufacturer.ManufacturerName.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemManufacturer.ManufacturerNumber.ToString()))
                        {
                            obj.ManufacturerNumber = item[CommonUtility.ImportItemManufacturer.ManufacturerNumber.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemManufacturer.IsDefault.ToString()))
                        {
                            bool IsDefault = false;
                            bool.TryParse(item[CommonUtility.ImportItemManufacturer.IsDefault.ToString()].ToString(), out IsDefault);
                            obj.IsDefault = IsDefault;

                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [ITEM SUPPLIER]
        private bool GetItemSupplierDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.ItemSupplierImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.ItemSupplierImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.ItemSupplierImport obj = new ImportMastersNewDTO.ItemSupplierImport();
                try
                {
                    if (item[CommonUtility.ImportItemSupplier.SupplierName.ToString()].ToString() != "")
                    {
                        // obj.ID = Convert.ToInt32(item[CommonUtility.ImportItemSupplier.ID.ToString()].ToString());
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemSupplier.ItemNumber.ToString()))
                        {
                            obj.ItemNumber = item[CommonUtility.ImportItemSupplier.ItemNumber.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemSupplier.SupplierName.ToString()))
                        {
                            obj.SupplierName = item[CommonUtility.ImportItemSupplier.SupplierName.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemSupplier.SupplierNumber.ToString()))
                        {
                            obj.SupplierNumber = item[CommonUtility.ImportItemSupplier.SupplierNumber.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportItemSupplier.IsDefault.ToString()))
                        {
                            bool IsDefault = false;
                            bool.TryParse(item[CommonUtility.ImportItemSupplier.IsDefault.ToString()].ToString(), out IsDefault);
                            obj.IsDefault = IsDefault;

                        }
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemSupplier.BlanketPOName.ToString()))
                        {
                            obj.BlanketPOName = item[CommonUtility.ImportItemSupplier.BlanketPOName.ToString()].ToString();

                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region [BARCODE MASTER]
        private bool GetBarcodeMasterDataFromFile(HttpPostedFileBase uploadFile, string ImportModule,
                                                   out List<ImportMastersNewDTO.BarcodeMasterImport> lstImport, out string DataTableColumns, out string ErrorMessage)
        {
            lstImport = new List<ImportMastersNewDTO.BarcodeMasterImport>();
            DataTableColumns = string.Empty;
            ErrorMessage = string.Empty;

            DataTable dtCSV = new DataTable();
            string fileName = Path.GetFileName(uploadFile.FileName);
            string[] strfilename = fileName.Split('.');
            List<DataRow> list = new List<DataRow>();

            if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
            {
                if (!GetCSVData(GetDBFields(ImportModule), GetRequiredDBFields(ImportModule), uploadFile, out dtCSV, out DataTableColumns, out ErrorMessage))
                {
                    return false;
                }
            }

            if (dtCSV.Rows.Count > 0)
            {
                list = dtCSV.AsEnumerable().ToList();
            }

            foreach (DataRow item in list)
            {
                ImportMastersNewDTO.BarcodeMasterImport obj = new ImportMastersNewDTO.BarcodeMasterImport();
                try
                {
                    if (item[CommonUtility.ImportBarcode.ItemNumber.ToString()].ToString() != "" && item[CommonUtility.ImportBarcode.ModuleName.ToString()].ToString() != "" && item[CommonUtility.ImportBarcode.BarcodeString.ToString()].ToString() != "")
                    {
                        // obj.ID = Convert.ToInt32(item[CommonUtility.ImportBarcode.ID.ToString()].ToString());
                        if (item.Table.Columns.Contains(CommonUtility.ImportBarcode.ItemNumber.ToString()))
                        {
                            obj.ItemNumber = item[CommonUtility.ImportBarcode.ItemNumber.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportBarcode.ModuleName.ToString()))
                        {
                            obj.ModuleName = item[CommonUtility.ImportBarcode.ModuleName.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportBarcode.BarcodeString.ToString()))
                        {
                            obj.BarcodeString = item[CommonUtility.ImportBarcode.BarcodeString.ToString()].ToString();
                        }

                        if (item.Table.Columns.Contains(CommonUtility.ImportBarcode.BinNumber.ToString()))
                        {
                            //if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportBarcode.BinNumber.ToString()].ToString())))
                            {
                                obj.BinNumber = item[CommonUtility.ImportBarcode.BinNumber.ToString()].ToString();

                            }
                            //else
                            //{
                            //    List<BinMasterDTO> objBinMasterListDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(ItemGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();

                            //    obj.BinGuid = objBinMasterListDTO.Where(b => b.IsDefault == true).FirstOrDefault().GUID;

                            //}
                        }

                        obj.Status = "";
                        obj.Reason = "";

                        obj.Index = list.IndexOf(item);

                        lstImport.Add(obj);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #endregion


        public PartialViewResult ItemMasterImport4()
        {
            ImportMastersNewDTO.ImportMaster objImportMaster = (ImportMastersNewDTO.ImportMaster)TempData["objImportMasterDTO"];
            return PartialView("_ItemMasterImport4", objImportMaster);
        }

        public JsonResult GetNextImportDataChunk(int[] RowIndexes)
        {
            List<ImportMastersNewDTO.ItemMasterImport> lstImport = (List<ImportMastersNewDTO.ItemMasterImport>)Session["ImportDataSession"];
            List<ImportMastersNewDTO.ItemMasterImport> _lstDataChunk = lstImport.Where(x => RowIndexes.Contains(x.Index)).ToList();
            return Json(new { lstDataChunk = _lstDataChunk }, JsonRequestBehavior.AllowGet);
        }

        #region [Load Data on Page]
        public JsonResult GetNextImportDataPage(string tableName, int MaxRowIndex, int MaxPageSize)
        {
            dynamic _lstDataPage = null;
            switch (tableName)
            {
                case "ItemMaster":
                    //List<ImportMastersNewDTO.ItemMasterImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.ItemMasterImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.ItemMasterImport> lstImport = (List<ImportMastersNewDTO.ItemMasterImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "CategoryMaster":
                    //List<ImportMastersNewDTO.CategoryMasterImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.CategoryMasterImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.CategoryMasterImport> lstImport = (List<ImportMastersNewDTO.CategoryMasterImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "CustomerMaster":
                    //List<ImportMastersNewDTO.CustomerMasterImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.CustomerMasterImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.CustomerMasterImport> lstImport = (List<ImportMastersNewDTO.CustomerMasterImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "AssetMaster":
                    //List<ImportMastersNewDTO.AssetMasterImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.AssetMasterImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.AssetMasterImport> lstImport = (List<ImportMastersNewDTO.AssetMasterImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "ToolMaster":
                    //List<ImportMastersNewDTO.ToolMasterImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.ToolMasterImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.ToolMasterImport> lstImport = (List<ImportMastersNewDTO.ToolMasterImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "SupplierMaster":
                    //List<ImportMastersNewDTO.SupplierMasterImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.SupplierMasterImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.SupplierMasterImport> lstImport = (List<ImportMastersNewDTO.SupplierMasterImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "QuickListItems":
                    //List<ImportMastersNewDTO.QuickListItemsImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.QuickListItemsImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.QuickListItemsImport> lstImport = (List<ImportMastersNewDTO.QuickListItemsImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "ManualCount":
                    //List<ImportMastersNewDTO.InventoryLocationImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.InventoryLocationImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.InventoryLocationImport> lstImport = (List<ImportMastersNewDTO.InventoryLocationImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "BinMaster":
                    //List<ImportMastersNewDTO.InventoryLocationImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.InventoryLocationImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.InventoryLocationImport> lstImport = (List<ImportMastersNewDTO.InventoryLocationImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "WorkOrder":
                    //List<ImportMastersNewDTO.WorkOrderImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.WorkOrderImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.WorkOrderImport> lstImport = (List<ImportMastersNewDTO.WorkOrderImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "BOMItemMaster":
                    //List<ImportMastersNewDTO.BOMItemMasterImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.BOMItemMasterImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.BOMItemMasterImport> lstImport = (List<ImportMastersNewDTO.BOMItemMasterImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "kitdetail":
                    //List<ImportMastersNewDTO.KitDetailImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.KitDetailImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.KitDetailImport> lstImport = (List<ImportMastersNewDTO.KitDetailImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "ProjectMaster":
                    //List<ImportMastersNewDTO.ProjectMasterImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.ProjectMasterImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.ProjectMasterImport> lstImport = (List<ImportMastersNewDTO.ProjectMasterImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "ItemLocationeVMISetup":
                    //List<ImportMastersNewDTO.InventoryLocationQuantityImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.InventoryLocationQuantityImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.InventoryLocationQuantityImport> lstImport = (List<ImportMastersNewDTO.InventoryLocationQuantityImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "ItemManufacturerDetails":
                    //List<ImportMastersNewDTO.ItemManufacturerImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.ItemManufacturerImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.ItemManufacturerImport> lstImport = (List<ImportMastersNewDTO.ItemManufacturerImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "ItemSupplierDetails":
                    //List<ImportMastersNewDTO.ItemSupplierImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.ItemSupplierImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.ItemSupplierImport> lstImport = (List<ImportMastersNewDTO.ItemSupplierImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
                case "BarcodeMaster":
                    //List<ImportMastersNewDTO.BarcodeMasterImport> _lstDataPage = null;
                    _lstDataPage = new List<ImportMastersNewDTO.BarcodeMasterImport>();
                    if (Session["ImportDataSession"] != null)
                    {
                        List<ImportMastersNewDTO.BarcodeMasterImport> lstImport = (List<ImportMastersNewDTO.BarcodeMasterImport>)Session["ImportDataSession"];
                        _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                    }
                    break;
            }
            return Json(new { lstDataPage = _lstDataPage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult SaveImport(string TableName, List<ImportMastersNewDTO.ImportDataChange> lstImportDataChange, int MaxRowIndex, int MaxPageSize, bool isImgZipAvail = false, bool isLink2ZipAvail = false)
        {
            string _Status = "";
            string _Reason = "";
            string savedOnlyitemIds = string.Empty;
            string savedItemIdsWithLink2 = string.Empty;
            object _lstDataPage = null;
            bool _ClearChangeObject = false;
            #region [SaveImport - ItemMaster]
            if (ImportMastersDTO.TableName.ItemMaster.ToString() == TableName)
            {
                List<ImportMastersNewDTO.ItemMasterImport> lstImport = null;
                ImportMastersNewDTO.ItemMasterImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.ItemMasterImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    objImport.Status = "";
                                    objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }

                    //----------------------CHECK VALIDATION----------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.ItemMasterImport objItemMasterImport in lstImport)
                        {
                            var context = new ValidationContext(objItemMasterImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objItemMasterImport, context, results, true);
                            if (!isValid)
                            {
                                ////_Status = "Fail";
                                ////_Reason = "Import failed. Please check reason.";
                                objItemMasterImport.Status = "Fail";
                                objItemMasterImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objItemMasterImport.Reason = objItemMasterImport.Reason + (objItemMasterImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }
                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.ItemMasterImport> lstReturn;
                        if (!ImportItem(lstImport, isImgZipAvail, isLink2ZipAvail, out lstReturn, out savedOnlyitemIds, out savedItemIdsWithLink2, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                        }
                        return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject, savedOnlyitemIds = savedOnlyitemIds, savedItemIdsWithLink2 = savedItemIdsWithLink2 }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - CategoryMaster]
            if (ImportMastersNewDTO.TableName.CategoryMaster.ToString() == TableName)
            {
                List<ImportMastersNewDTO.CategoryMasterImport> lstImport = null;
                ImportMastersNewDTO.CategoryMasterImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.CategoryMasterImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    objImport.Status = "";
                                    objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    //// _Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }
                    //----------------------CHECK VALIDATION----------------------
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.CategoryMasterImport objCategoryMasterImport in lstImport)
                        {
                            var context = new ValidationContext(objCategoryMasterImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objCategoryMasterImport, context, results, true);
                            if (!isValid)
                            {
                                ////_Status = "Fail";
                                ////_Reason = "Import failed. Please check reason.";
                                objCategoryMasterImport.Status = "Fail";
                                objCategoryMasterImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objCategoryMasterImport.Reason = objCategoryMasterImport.Reason + (objCategoryMasterImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }
                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.CategoryMasterImport> lstReturn;
                        if (!ImportCategory(lstImport, out lstReturn, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                            return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - CustomerMaster]
            if (ImportMastersNewDTO.TableName.CustomerMaster.ToString() == TableName)
            {
                List<ImportMastersNewDTO.CustomerMasterImport> lstImport = null;
                ImportMastersNewDTO.CustomerMasterImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.CustomerMasterImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    ////objImport.Status = "";
                                    ////objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }
                    //----------------------CHECK VALIDATION----------------------
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.CustomerMasterImport objCustomerMasterImport in lstImport)
                        {
                            var context = new ValidationContext(objCustomerMasterImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objCustomerMasterImport, context, results, true);
                            if (!isValid)
                            {
                                ////_Status = "Fail";
                                ////_Reason = "Import failed. Please check reason.";
                                objCustomerMasterImport.Status = "Fail";
                                objCustomerMasterImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objCustomerMasterImport.Reason = objCustomerMasterImport.Reason + (objCustomerMasterImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }

                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.CustomerMasterImport> lstReturn;
                        if (!ImportCustomer(lstImport, out lstReturn, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                            return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - AssetMaster]
            if (ImportMastersNewDTO.TableName.AssetMaster.ToString() == TableName)
            {
                List<ImportMastersNewDTO.AssetMasterImport> lstImport = null;
                ImportMastersNewDTO.AssetMasterImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.AssetMasterImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    ////objImport.Status = "";
                                    ////objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }
                    //----------------------CHECK VALIDATION----------------------
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.AssetMasterImport objAssetMasterImport in lstImport)
                        {
                            var context = new ValidationContext(objAssetMasterImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objAssetMasterImport, context, results, true);
                            if (!isValid)
                            {
                                ////_Status = "Fail";
                                ////_Reason = "Import failed. Please check reason.";
                                objAssetMasterImport.Status = "Fail";
                                objAssetMasterImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objAssetMasterImport.Reason = objAssetMasterImport.Reason + (objAssetMasterImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }

                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.AssetMasterImport> lstReturn;
                        if (!ImportAsset(lstImport, isImgZipAvail, out lstReturn, out savedOnlyitemIds, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                        }

                        return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject, savedOnlyitemIds = savedOnlyitemIds }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - ToolMaster]
            if (ImportMastersNewDTO.TableName.ToolMaster.ToString() == TableName)
            {
                List<ImportMastersNewDTO.ToolMasterImport> lstImport = null;
                ImportMastersNewDTO.ToolMasterImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.ToolMasterImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    ////objImport.Status = "";
                                    ////objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }
                    //----------------------CHECK VALIDATION----------------------
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.ToolMasterImport objToolMasterImport in lstImport)
                        {
                            var context = new ValidationContext(objToolMasterImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objToolMasterImport, context, results, true);
                            if (!isValid)
                            {
                                ////_Status = "Fail";
                                ////_Reason = "Import failed. Please check reason.";
                                objToolMasterImport.Status = "Fail";
                                objToolMasterImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objToolMasterImport.Reason = objToolMasterImport.Reason + (objToolMasterImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }

                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.ToolMasterImport> lstReturn;
                        if (!ImportTool(lstImport, isImgZipAvail, out lstReturn, out savedOnlyitemIds, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                        }

                        return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject, savedOnlyitemIds = savedOnlyitemIds }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - SupplierMaster]
            if (ImportMastersNewDTO.TableName.SupplierMaster.ToString() == TableName)
            {
                List<ImportMastersNewDTO.SupplierMasterImport> lstImport = null;
                ImportMastersNewDTO.SupplierMasterImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.SupplierMasterImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    ////objImport.Status = "";
                                    ////objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }
                    //----------------------CHECK VALIDATION----------------------
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.SupplierMasterImport objSupplierMasterImport in lstImport)
                        {
                            var context = new ValidationContext(objSupplierMasterImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objSupplierMasterImport, context, results, true);
                            if (!isValid)
                            {
                                ////_Status = "Fail";
                                ////_Reason = "Import failed. Please check reason.";
                                objSupplierMasterImport.Status = "Fail";
                                objSupplierMasterImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objSupplierMasterImport.Reason = objSupplierMasterImport.Reason + (objSupplierMasterImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }

                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.SupplierMasterImport> lstReturn;
                        if (!ImportSupplier(lstImport, isImgZipAvail, out lstReturn, out savedOnlyitemIds, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                        }

                        return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject, savedOnlyitemIds = savedOnlyitemIds }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - QuickListItems]
            if (ImportMastersNewDTO.TableName.QuickListItems.ToString() == TableName)
            {
                List<ImportMastersNewDTO.QuickListItemsImport> lstImport = null;
                ImportMastersNewDTO.QuickListItemsImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.QuickListItemsImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    ////objImport.Status = "";
                                    ////objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }
                    //----------------------CHECK VALIDATION----------------------
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.QuickListItemsImport objQuickListItemsImport in lstImport)
                        {
                            var context = new ValidationContext(objQuickListItemsImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objQuickListItemsImport, context, results, true);
                            if (!isValid)
                            {
                                ////_Status = "Fail";
                                ////_Reason = "Import failed. Please check reason.";
                                objQuickListItemsImport.Status = "Fail";
                                objQuickListItemsImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objQuickListItemsImport.Reason = objQuickListItemsImport.Reason + (objQuickListItemsImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }
                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.QuickListItemsImport> lstReturn;
                        if (!ImportQuickListItems(lstImport, out lstReturn, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                            return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion


            #region [SaveImport - Inventory Location Import (MANUAL COUNT)]
            if (ImportMastersNewDTO.TableName.ManualCount.ToString() == TableName)
            {
                List<ImportMastersNewDTO.InventoryLocationImport> lstImport = null;
                ImportMastersNewDTO.InventoryLocationImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.InventoryLocationImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    ////objImport.Status = "";
                                    ////objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }



                    //----------------------CHECK VALIDATION----------------------
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.InventoryLocationImport objInventoryLocationImport in lstImport)
                        {
                            var context = new ValidationContext(objInventoryLocationImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objInventoryLocationImport, context, results, true);
                            if (!isValid)
                            {
                                ////_Status = "Fail";
                                ////_Reason = "Import failed. Please check reason.";
                                objInventoryLocationImport.Status = "Fail";
                                objInventoryLocationImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objInventoryLocationImport.Reason = objInventoryLocationImport.Reason + (objInventoryLocationImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }
                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.InventoryLocationImport> lstReturn;
                        if (!ImportInventoryLocationItems(lstImport, out lstReturn, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                            return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - Adjustment Count Import (BIN MASTER)]
            if (ImportMastersNewDTO.TableName.BinMaster.ToString() == TableName)
            {
                List<ImportMastersNewDTO.InventoryLocationImport> lstImport = null;
                ImportMastersNewDTO.InventoryLocationImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.InventoryLocationImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    ////objImport.Status = "";
                                    ////objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }



                    //----------------------CHECK VALIDATION----------------------
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.InventoryLocationImport objInventoryLocationImport in lstImport)
                        {
                            var context = new ValidationContext(objInventoryLocationImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objInventoryLocationImport, context, results, true);
                            if (!isValid)
                            {
                                ////_Status = "Fail";
                                ////_Reason = "Import failed. Please check reason.";
                                objInventoryLocationImport.Status = "Fail";
                                objInventoryLocationImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objInventoryLocationImport.Reason = objInventoryLocationImport.Reason + (objInventoryLocationImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }
                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.InventoryLocationImport> lstReturn;
                        if (!ImportAdjustmentCountBMItems(lstImport, out lstReturn, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                            return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - WorkOrder]
            if (ImportMastersNewDTO.TableName.WorkOrder.ToString() == TableName)
            {
                List<ImportMastersNewDTO.WorkOrderImport> lstImport = null;
                ImportMastersNewDTO.WorkOrderImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.WorkOrderImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    ////objImport.Status = "";
                                    ////objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }
                    //----------------------CHECK VALIDATION----------------------
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.WorkOrderImport objWorkOrderImport in lstImport)
                        {
                            var context = new ValidationContext(objWorkOrderImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objWorkOrderImport, context, results, true);
                            if (!isValid)
                            {
                                ////_Status = "Fail";
                                ////_Reason = "Import failed. Please check reason.";
                                objWorkOrderImport.Status = "Fail";
                                objWorkOrderImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objWorkOrderImport.Reason = objWorkOrderImport.Reason + (objWorkOrderImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }

                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.WorkOrderImport> lstReturn;
                        if (!ImportWorkOrder(lstImport, out lstReturn, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                            return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - BOM Item Master]
            if (ImportMastersDTO.TableName.BOMItemMaster.ToString() == TableName)
            {
                List<ImportMastersNewDTO.BOMItemMasterImport> lstImport = null;
                ImportMastersNewDTO.BOMItemMasterImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.BOMItemMasterImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    objImport.Status = "";
                                    objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }

                    //----------------------CHECK VALIDATION----------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.BOMItemMasterImport objBOMItemMasterImport in lstImport)
                        {
                            var context = new ValidationContext(objBOMItemMasterImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objBOMItemMasterImport, context, results, true);
                            if (!isValid)
                            {
                                ////_Status = "Fail";
                                ////_Reason = "Import failed. Please check reason.";
                                objBOMItemMasterImport.Status = "Fail";
                                objBOMItemMasterImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objBOMItemMasterImport.Reason = objBOMItemMasterImport.Reason + (objBOMItemMasterImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }
                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.BOMItemMasterImport> lstReturn;
                        if (!ImportBOMItem(lstImport, out lstReturn, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                        }
                        return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - KitDetail]
            if (ImportMastersDTO.TableName.kitdetail.ToString() == TableName)
            {
                List<ImportMastersNewDTO.KitDetailImport> lstImport = null;
                ImportMastersNewDTO.KitDetailImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.KitDetailImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    objImport.Status = "";
                                    objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }

                    //----------------------CHECK VALIDATION----------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.KitDetailImport objKitDetailImport in lstImport)
                        {
                            var context = new ValidationContext(objKitDetailImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objKitDetailImport, context, results, true);
                            if (!isValid)
                            {
                                ////_Status = "Fail";
                                ////_Reason = "Import failed. Please check reason.";
                                objKitDetailImport.Status = "Fail";
                                objKitDetailImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objKitDetailImport.Reason = objKitDetailImport.Reason + (objKitDetailImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }
                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.KitDetailImport> lstReturn;
                        if (!ImportKitDetail(lstImport, out lstReturn, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                        }
                        return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - Project Master]
            if (ImportMastersDTO.TableName.ProjectMaster.ToString() == TableName)
            {
                List<ImportMastersNewDTO.ProjectMasterImport> lstImport = null;
                ImportMastersNewDTO.ProjectMasterImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.ProjectMasterImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    objImport.Status = "";
                                    objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }

                    //----------------------CHECK VALIDATION----------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.ProjectMasterImport objProjectMasterImport in lstImport)
                        {
                            var context = new ValidationContext(objProjectMasterImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objProjectMasterImport, context, results, true);
                            if (!isValid)
                            {
                                ////_Status = "Fail";
                                ////_Reason = "Import failed. Please check reason.";
                                objProjectMasterImport.Status = "Fail";
                                objProjectMasterImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objProjectMasterImport.Reason = objProjectMasterImport.Reason + (objProjectMasterImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }
                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.ProjectMasterImport> lstReturn;
                        if (!ImportProjectMaster(lstImport, out lstReturn, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                        }
                        return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - Item Location eVMI]
            if (ImportMastersDTO.TableName.ItemLocationeVMISetup.ToString() == TableName)
            {
                List<ImportMastersNewDTO.InventoryLocationQuantityImport> lstImport = null;
                ImportMastersNewDTO.InventoryLocationQuantityImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.InventoryLocationQuantityImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    objImport.Status = "";
                                    objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }

                    //----------------------CHECK VALIDATION----------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.InventoryLocationQuantityImport objILQtyImport in lstImport)
                        {
                            var context = new ValidationContext(objILQtyImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objILQtyImport, context, results, true);
                            if (!isValid)
                            {
                                objILQtyImport.Status = "Fail";
                                objILQtyImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objILQtyImport.Reason = objILQtyImport.Reason + (objILQtyImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }
                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.InventoryLocationQuantityImport> lstReturn;
                        if (!ImportItemLocationeVMI(lstImport, out lstReturn, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                        }
                        return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - Item Manufacturer]
            if (ImportMastersDTO.TableName.ItemManufacturerDetails.ToString() == TableName)
            {
                List<ImportMastersNewDTO.ItemManufacturerImport> lstImport = null;
                ImportMastersNewDTO.ItemManufacturerImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.ItemManufacturerImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    objImport.Status = "";
                                    objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }

                    //----------------------CHECK VALIDATION----------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.ItemManufacturerImport objIMDImport in lstImport)
                        {
                            var context = new ValidationContext(objIMDImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objIMDImport, context, results, true);
                            if (!isValid)
                            {
                                objIMDImport.Status = "Fail";
                                objIMDImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objIMDImport.Reason = objIMDImport.Reason + (objIMDImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }
                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.ItemManufacturerImport> lstReturn;
                        if (!ImportItemManufacturer(lstImport, out lstReturn, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                        }
                        return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - Item Supplier]
            if (ImportMastersDTO.TableName.ItemSupplierDetails.ToString() == TableName)
            {
                List<ImportMastersNewDTO.ItemSupplierImport> lstImport = null;
                ImportMastersNewDTO.ItemSupplierImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.ItemSupplierImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    objImport.Status = "";
                                    objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }

                    //----------------------CHECK VALIDATION----------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.ItemSupplierImport objISDImport in lstImport)
                        {
                            var context = new ValidationContext(objISDImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objISDImport, context, results, true);
                            if (!isValid)
                            {
                                objISDImport.Status = "Fail";
                                objISDImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objISDImport.Reason = objISDImport.Reason + (objISDImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }
                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.ItemSupplierImport> lstReturn;
                        if (!ImportItemSupplier(lstImport, out lstReturn, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                        }
                        return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            #region [SaveImport - Barcode Master]
            if (ImportMastersDTO.TableName.BarcodeMaster.ToString() == TableName)
            {
                List<ImportMastersNewDTO.BarcodeMasterImport> lstImport = null;
                ImportMastersNewDTO.BarcodeMasterImport objImport;

                if (Session["ImportDataSession"] != null)
                {
                    _Status = "success";
                    _Reason = "";
                    lstImport = (List<ImportMastersNewDTO.BarcodeMasterImport>)Session["ImportDataSession"];
                    foreach (var item in lstImport)
                    {
                        item.Status = "";
                        item.Reason = "";
                    }
                    //---------------------SET CHANGED VALUES---------------------
                    //
                    if (lstImportDataChange != null && lstImportDataChange.Count > 0)
                    {
                        foreach (ImportMastersNewDTO.ImportDataChange objImportDataChange in lstImportDataChange)
                        {
                            objImport = lstImport.Where(x => x.Index == objImportDataChange.RowIndex).FirstOrDefault();
                            if (objImport != null)
                            {
                                try
                                {
                                    var property = objImport.GetType().GetProperty(objImportDataChange.FieldName);
                                    if (property != null)
                                    {
                                        Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                        object safeValue = (objImportDataChange.Value == null) ? null : Convert.ChangeType(objImportDataChange.Value, t);
                                        property.SetValue(objImport, safeValue, null);
                                    }
                                    //objImport.GetType().GetProperty(objImportDataChange.FieldName).SetValue(objImport, objImportDataChange.Value, null);
                                    objImport.Status = "";
                                    objImport.Reason = "";
                                }
                                catch (ArgumentException ArgEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (FormatException FrmtEx)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Value of '" + objImportDataChange.FieldName + "' not matching with data type";
                                    ////_Status = "Fail";
                                    ////_Reason = "Value not matching with data type";
                                }
                                catch (Exception ex)
                                {
                                    objImport.Status = "Fail";
                                    objImport.Reason = "Undefined error occured";
                                    ////_Status = "Fail";
                                    ////_Reason = "Undefined error occured";
                                }
                            }
                        }
                    }

                    //----------------------CHECK VALIDATION----------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        _ClearChangeObject = true;
                        foreach (ImportMastersNewDTO.BarcodeMasterImport objBMImport in lstImport)
                        {
                            var context = new ValidationContext(objBMImport, serviceProvider: null, items: null);
                            var results = new List<ValidationResult>();
                            bool isValid = Validator.TryValidateObject(objBMImport, context, results, true);
                            if (!isValid)
                            {
                                objBMImport.Status = "Fail";
                                objBMImport.Reason = "";
                                foreach (var validationResult in results)
                                {
                                    objBMImport.Reason = objBMImport.Reason + (objBMImport.Reason == "" ? "" : "," + Environment.NewLine) + validationResult.ErrorMessage;
                                }
                            }
                        }
                    }

                    //-------------------------DO IMPORT-------------------------
                    //
                    if (_Status.ToUpper() != "FAIL")
                    {
                        List<ImportMastersNewDTO.BarcodeMasterImport> lstReturn;
                        if (!SaveImportBarcodeMaster(lstImport, out lstReturn, out _Reason, out _Status))
                        {
                            Session["ImportDataSession"] = lstReturn;
                            _lstDataPage = lstReturn.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                        }
                        return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    _Status = "Fail";
                    _Reason = "Import session expired";
                }

                if (_Status.ToUpper() == "FAIL")
                {
                    lstImport = lstImport.OrderByDescending(x => x.Status).ToList();
                    Session["ImportDataSession"] = lstImport;
                    _lstDataPage = lstImport.Skip(MaxRowIndex).Take(MaxPageSize).ToList();
                }
            }
            #endregion

            return Json(new { status = _Status, reason = _Reason, lstDataPage = _lstDataPage, ClearChangeObject = _ClearChangeObject }, JsonRequestBehavior.AllowGet);
        }

        #region [Import Private Mehtod]
        #region [Import Item]
        private bool ImportItem(List<ImportMastersNewDTO.ItemMasterImport> LstItemMaster, bool isImgZipAvail, bool isLink2ZipAvail, out List<ImportMastersNewDTO.ItemMasterImport> lstreturn, out string savedOnlyitemIds, out string savedItemIdsWithLink2, out string reason, out string status)
        {
            savedOnlyitemIds = string.Empty;
            savedItemIdsWithLink2 = string.Empty;
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.ItemMasterImport>();
            List<ImportMastersNewDTO.ItemMasterImport> CurrentBlankItemList = new List<ImportMastersNewDTO.ItemMasterImport>();
            List<BOMItemMasterMain> CurrentItemList = null;
            List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            List<Guid> lstItemGUID = new List<Guid>();

            if (LstItemMaster != null && LstItemMaster.Count > 0)
            {
                CurrentItemList = new List<BOMItemMasterMain>();
                lst = objImport.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.ItemMaster.ToString(), UDFControlTypes.Textbox.ToString());
                eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetAllRecordsNew(SessionHelper.CompanyID, "ItemMaster", SessionHelper.RoomID);
                CurrentOptionList = new List<UDFOptionsMain>();

                //bool IsEmailPOInBody = false; bool IsEmailPOInPDF = false; bool IsEmailPOInCSV = false; bool IsEmailPOInX12 = false;
                foreach (ImportMastersNewDTO.ItemMasterImport item in LstItemMaster)
                {
                    if (string.IsNullOrEmpty(item.SupplierPartNo) && ((!string.IsNullOrWhiteSpace(item.ItemTypeName)) && item.ItemTypeName != "Labor"))
                    {
                        item.Status = "Fail";
                        item.Reason = string.Format(ResMessage.Required, "SupplierPartNo"); ;
                    }

                    string errorMsg = string.Empty;
                    CheckUDFIsRequired(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, out errorMsg);
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        item.Status = "Fail";
                        if (!string.IsNullOrEmpty(item.Reason))
                            item.Reason += errorMsg;
                        else
                            item.Reason = errorMsg;

                    }

                    if (item.Status.Trim().ToLowerInvariant() == "fail" || string.IsNullOrEmpty(item.ItemNumber))
                    {
                        CurrentBlankItemList.Add(item);
                        continue;
                    }
                    else
                    {
                        BOMItemMasterMain objDTO = new BOMItemMasterMain();

                        objDTO.ID = item.ID;
                        objDTO.ItemNumber = (item.ItemNumber == null) ? null : (item.ItemNumber.Length > 255 ? item.ItemNumber.Substring(0, 255) : item.ItemNumber);
                        objDTO.ManufacturerName = item.ManufacturerName;
                        //Wi-1505
                        //if (!string.IsNullOrWhiteSpace(item.ManufacturerName))
                        //{
                        //    objDTO.ManufacturerID = item.ManufacturerName == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.ManufacturerMaster, item.ManufacturerName);
                        //}
                        //else
                        //{
                        //    objDTO.ManufacturerID = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName).GetORInsertBlankManuFacID(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                        //}
                        if (!string.IsNullOrWhiteSpace(item.ManufacturerName))
                        {
                            objDTO.ManufacturerID = item.ManufacturerName == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.ManufacturerMaster, item.ManufacturerName);
                        }
                        else
                        {
                            long ManuID = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName).GetORInsertBlankManuFacID(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                            if (!string.IsNullOrWhiteSpace(item.ManufacturerNumber))
                            {
                                objDTO.ManufacturerID = ManuID;
                            }

                        }
                        objDTO.ManufacturerNumber = (item.ManufacturerNumber == null) ? null : (item.ManufacturerNumber.Length > 50 ? item.ManufacturerNumber.Substring(0, 50) : item.ManufacturerNumber);
                        objDTO.Link1 = item.Link1;
                        objDTO.Link2 = item.Link2;
                        objDTO.ItemLink2ImageType = "InternalLink";
                        objDTO.ImageType = "ExternalImage";
                        objDTO.ItemImageExternalURL = item.ItemImageExternalURL;
                        objDTO.ItemLink2ExternalURL = item.ItemLink2ExternalURL;
                        if (string.IsNullOrWhiteSpace(item.ItemImageExternalURL) && (!string.IsNullOrEmpty(item.ImagePath)))
                        {
                            objDTO.ImageType = "ImagePath";
                        }
                        if (string.IsNullOrWhiteSpace(item.Link2) && (!string.IsNullOrEmpty(item.ItemLink2ExternalURL)))
                        {
                            objDTO.ItemLink2ImageType = "ExternalURL";
                        }
                        objDTO.ItemDocExternalURL = item.ItemDocExternalURL;
                        objDTO.IsActive = item.IsActive;
                        if (item.ItemTypeName == "Labor")
                        {
                            objDTO.SupplierName = string.Empty;
                            objDTO.SupplierPartNo = null;
                            objDTO.SupplierID = null;
                            objDTO.BlanketOrderNumber = string.Empty;
                            objDTO.BlanketPOID = null;
                        }
                        else
                        {
                            objDTO.SupplierName = item.SupplierName;
                            objDTO.SupplierPartNo = (item.SupplierPartNo == null) ? null : (item.SupplierPartNo.Length > 50 ? item.SupplierPartNo.Substring(0, 50) : item.SupplierPartNo);
                            objDTO.SupplierID = GetIDs(ImportMastersDTO.TableName.SupplierMaster, item.SupplierName);
                            objDTO.BlanketOrderNumber = item.BlanketOrderNumber;
                            if (objDTO.SupplierID.HasValue && objDTO.SupplierID.Value > 0 && !string.IsNullOrWhiteSpace(item.BlanketOrderNumber))
                                objDTO.BlanketPOID = GetIDs(ImportMastersDTO.TableName.SupplierBlanketPODetails, item.BlanketOrderNumber, objDTO.SupplierID.Value);
                        }

                        objDTO.UPC = item.UPC;
                        objDTO.UNSPSC = item.UNSPSC;
                        objDTO.Description = item.Description;
                        objDTO.LongDescription = item.LongDescription;
                        objDTO.CategoryID = item.CategoryName == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.CategoryMaster, item.CategoryName);
                        objDTO.GLAccountID = item.GLAccount == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.GLAccountMaster, item.GLAccount);
                        objDTO.CategoryName = item.CategoryName;
                        objDTO.GLAccount = item.GLAccount;
                        objDTO.UOMID = GetIDs(ImportMastersDTO.TableName.UnitMaster, item.Unit);
                        objDTO.Unit = item.Unit;
                        objDTO.PricePerTerm = item.PricePerTerm;
                        objDTO.CostUOMID = item.CostUOMName == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.CostUOMMaster, item.CostUOMName);
                        objDTO.CostUOMName = item.CostUOMName;
                        objDTO.DefaultReorderQuantity = item.DefaultReorderQuantity;
                        objDTO.DefaultPullQuantity = item.DefaultPullQuantity;

                        //if (objDTO.Markup > 0)
                        //{
                        //    double? markup = (item.Cost * objDTO.Markup) / 100;
                        //    objDTO.SellPrice = objDTO.Cost + markup;
                        //}
                        //else
                        //{
                        //    if (objDTO.Cost != null)
                        //        objDTO.SellPrice = objDTO.Cost;
                        //}
                        // objDTO.LastCost = 0.0;
                        item.Cost = item.Cost ?? 0;
                        item.SellPrice = item.SellPrice ?? 0;
                        item.Markup = item.Markup ?? 0;

                        objDTO.Cost = item.Cost;
                        objDTO.SellPrice = item.SellPrice;
                        objDTO.Markup = item.Markup;

                        if (item.Cost != 0 && item.SellPrice != 0 && item.Markup != 0)
                        {
                            objDTO.Markup = Convert.ToDouble(((Convert.ToDecimal(objDTO.SellPrice) * Convert.ToDecimal(100)) / Convert.ToDecimal(objDTO.Cost)) - Convert.ToDecimal(100));
                            // Calculate MArkup based on price and cost
                        }
                        else if (item.Cost != 0 && item.SellPrice == 0 && item.Markup == 0)
                        {
                            // Prise will become same as cost
                            objDTO.SellPrice = objDTO.Cost;
                        }
                        else if (item.Cost != 0 && item.SellPrice != 0 && item.Markup == 0)
                        {
                            // Calculate MArkup based on price and cost
                            objDTO.Markup = Convert.ToDouble(((Convert.ToDecimal(objDTO.SellPrice) * Convert.ToDecimal(100)) / Convert.ToDecimal(objDTO.Cost)) - Convert.ToDecimal(100));
                        }
                        else if (item.Cost != 0 && item.SellPrice == 0 && item.Markup != 0)
                        {
                            //Calculate prise based on cost and markup
                            objDTO.SellPrice = Convert.ToDouble(Convert.ToDecimal(objDTO.Cost) + ((Convert.ToDecimal(objDTO.Cost) * Convert.ToDecimal(objDTO.Markup)) / Convert.ToDecimal(100)));
                        }
                        else if (item.Cost == 0 && item.SellPrice != 0 && item.Markup != 0)
                        {
                            // Calculate cost based on prise and markup                                 
                            //objDTO.Cost = objDTO.SellPrice - ((objDTO.SellPrice * objDTO.Markup) / 100);
                            objDTO.Cost = Convert.ToDouble((Convert.ToDecimal(100) * Convert.ToDecimal(item.SellPrice)) / (Convert.ToDecimal(item.Markup) + Convert.ToDecimal(100)));
                            //Please follow Markup= (((sell-cost)/cost)*100)
                        }
                        else if (item.Cost == 0 && item.SellPrice == 0 && item.Markup == 0)
                        {
                            // All are zero so no calc
                        }
                        else if (item.Cost == 0 && item.SellPrice != 0 && item.Markup == 0)
                        {
                            // cost will become same as prise
                            objDTO.Cost = objDTO.SellPrice;
                        }
                        else if (item.Cost == 0 && item.SellPrice == 0 && item.Markup != 0)
                        {
                            objDTO.Markup = 0;
                            // Cost and prise will remain zero and save markup only Or make markup zero because no prise and cost
                        }

                        objDTO.ExtendedCost = 0;// item.ExtendedCost;
                        objDTO.DispExtendedCost = item.ExtendedCost;
                        objDTO.LeadTimeInDays = item.LeadTimeInDays;
                        objDTO.Trend = item.Trend;
                        objDTO.Taxable = item.Taxable;
                        objDTO.Consignment = item.Consignment;
                        objDTO.StagedQuantity = 0;// item.StagedQuantity;
                        objDTO.InTransitquantity = 0;// item.InTransitquantity;
                        objDTO.OnOrderQuantity = 0;// item.OnOrderQuantity;
                        objDTO.OnTransferQuantity = 0;// item.OnTransferQuantity;
                        objDTO.SuggestedOrderQuantity = 0;// item.SuggestedOrderQuantity;
                        objDTO.RequisitionedQuantity = 0;// item.RequisitionedQuantity;
                        objDTO.AverageUsage = 0;// item.AverageUsage;
                        objDTO.Turns = 0;// item.Turns;
                        objDTO.OnHandQuantity = item.OnHandQuantity;

                        objDTO.DispStagedQuantity = item.StagedQuantity;
                        objDTO.DispInTransitquantity = item.InTransitquantity;
                        objDTO.DispOnOrderQuantity = item.OnOrderQuantity;
                        objDTO.DispOnTransferQuantity = item.OnTransferQuantity;
                        objDTO.DispSuggestedOrderQuantity = item.SuggestedOrderQuantity;
                        objDTO.DispRequisitionedQuantity = item.RequisitionedQuantity;
                        objDTO.DispAverageUsage = item.AverageUsage;
                        objDTO.DispTurns = item.Turns;
                        objDTO.DispOnHandQuantity = item.OnHandQuantity;
                        objDTO.IsPackslipMandatoryAtReceive = item.IsPackslipMandatoryAtReceive;
                        objDTO.IsItemLevelMinMaxQtyRequired = item.IsItemLevelMinMaxQtyRequired;
                        objDTO.CriticalQuantity = item.CriticalQuantity;
                        objDTO.MinimumQuantity = item.MinimumQuantity;
                        objDTO.MaximumQuantity = item.MaximumQuantity;
                        objDTO.WeightPerPiece = item.WeightPerPiece;
                        //objDTO.ItemUniqueNumber = objCommonDAL.UniqueItemId();
                        objDTO.ItemUniqueNumber = objCommonDAL.GetItemUniqueIdByRoom(item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));
                        //objDTO.TransferOrPurchase = item.TransferOrPurchase;
                        objDTO.IsTransfer = item.IsTransfer;
                        objDTO.IsPurchase = item.IsPurchase;
                        if ((item.IsDeleted ?? false) == false)
                        {
                            objDTO.DefaultLocation = objImport.GetBinForImportItem(item.InventryLocation, objDTO.ItemNumber, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID); // GetIDs(ImportMastersDTO.TableName.BinMaster, item.InventryLocation);
                        }
                        objDTO.InventryLocation = item.InventryLocation;
                        objDTO.InventoryClassification = Convert.ToInt32(GetIDs(ImportMastersDTO.TableName.InventoryClassificationMaster, item.InventoryClassificationName));
                        objDTO.InventoryClassificationName = item.InventoryClassificationName;
                        objDTO.ItemTypeName = item.ItemTypeName;
                        objDTO.SerialNumberTracking = item.SerialNumberTracking;
                        objDTO.LotNumberTracking = item.LotNumberTracking;
                        objDTO.DateCodeTracking = item.DateCodeTracking;
                        objDTO.ItemType = item.ItemTypeName == "Item" ? 1 : item.ItemTypeName == "Quick List" ? 2 : item.ItemTypeName == "Kit" ? 3 : item.ItemTypeName == "Labor" ? 4 : 1;
                        objDTO.ImagePath = item.ImagePath;
                        objDTO.OnHandQuantity = (!objDTO.SerialNumberTracking && !objDTO.LotNumberTracking && !objDTO.DateCodeTracking) ? objDTO.OnHandQuantity : 0;
                        objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);

                        objDTO.CategoryColor = "";
                        objDTO.IsLotSerialExpiryCost = item.IsLotSerialExpiryCost;
                        if (objDTO.ItemType == 3)
                        {
                            objDTO.IsBuildBreak = item.IsBuildBreak;
                        }
                        objDTO.IsAutoInventoryClassification = item.IsAutoInventoryClassification;
                        objDTO.PullQtyScanOverride = item.PullQtyScanOverride;
                        objDTO.IsEnforceDefaultReorderQuantity = item.IsEnforceDefaultReorderQuantity ?? false;
                        objDTO.TrendingSetting = getTrendingID(item.TrendingSettingName);
                        objDTO.TrendingSettingName = item.TrendingSettingName;
                        objDTO.IsDeleted = item.IsDeleted ?? false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.WhatWhereAction = "Import";
                        if (objDTO.ItemType == 4 && (objDTO.DefaultPullQuantity == null || objDTO.DefaultPullQuantity <= 0))
                        {
                            objDTO.DefaultPullQuantity = 1;
                        }
                        lstItemGUID.Add(objDTO.GUID);




                        //if (!string.IsNullOrWhiteSpace(item.ManufacturerNumber))
                        //{
                        //    ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                        //    bool Result = objItemManufacturerDetailsDAL.CheckManufacturerNoDuplicateByItemNumber(item.ManufacturerNumber.Trim(), SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.ItemNumber, false);
                        //    if (!Result)
                        //    {
                        //        SaveToolList = false;
                        //        objDTO.Status = "Fail";
                        //        objDTO.Reason = ResMessage.DuplicateManufacturerNumber;
                        //        item.Status = "Fail";
                        //        item.Reason = ResMessage.DuplicateManufacturerNumber;
                        //    }
                        //}

                        var itemval = CurrentItemList.FirstOrDefault(x => x.ItemNumber == item.ItemNumber);
                        if (itemval != null)
                            CurrentItemList.Remove(itemval);
                        CurrentItemList.Add(objDTO);
                        //CurrentItemDTOList.Add(objDTO);

                        item.Status = "Success";
                        item.Reason = "N/A";

                        CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportItemColumn.UDF1.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportItemColumn.UDF2.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportItemColumn.UDF3.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportItemColumn.UDF4.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportItemColumn.UDF5.ToString());

                    }


                }

                ImportMastersNewDTO.ItemMasterImport objItemMasterImport;
                List<BOMItemMasterMain> lstreturn1 = new List<BOMItemMasterMain>();
                if (CurrentItemList.Count > 0)
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.ItemMaster.ToString(), CurrentItemList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList, isImgZipAvail, isLink2ZipAvail);

                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {
                    /* ------- FOR SET SAVE ID AND ID WITH LINK2 ----------*/

                    List<BOMItemMasterMain> successSaveList = new List<BOMItemMasterMain>();
                    successSaveList = lstreturn1.Where(x => x.Status.ToUpper() == "SUCCESS").ToList();
                    if (isImgZipAvail)
                    {
                        savedOnlyitemIds = string.Join(",", successSaveList.Where(i => i.ID != 0 && (!string.IsNullOrEmpty(i.ImagePath))).Select(p => p.ID.ToString() + "#" + p.ImagePath.ToString()));

                        foreach (BOMItemMasterMain b in successSaveList.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.ImagePath))))
                        {
                            ItemMasterDTO objItemMaster = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetRecordByItemNumber(Convert.ToString(b.ItemNumber), Convert.ToInt64(b.Room), Convert.ToInt64(b.CompanyID));
                            if (objItemMaster != null)
                            {
                                if (string.IsNullOrEmpty(savedOnlyitemIds))
                                {
                                    savedOnlyitemIds = objItemMaster.ID + "#" + objItemMaster.ImagePath.ToString();
                                }
                                else
                                {
                                    savedOnlyitemIds += "," + objItemMaster.ID + "#" + objItemMaster.ImagePath.ToString();
                                }
                            }
                        }
                    }
                    if (isLink2ZipAvail)
                    {
                        savedItemIdsWithLink2 = string.Join(",", successSaveList.Where(i => i.ID != 0 && (!string.IsNullOrEmpty(i.Link2))).Select(p => p.ID.ToString() + "#" + p.Link2.ToString()));
                        foreach (BOMItemMasterMain b in successSaveList.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.Link2))))
                        {
                            ItemMasterDTO objItemMaster = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetRecordByItemNumber(Convert.ToString(b.ItemNumber), Convert.ToInt64(b.Room), Convert.ToInt64(b.CompanyID));
                            if (objItemMaster != null)
                            {
                                if (string.IsNullOrEmpty(savedItemIdsWithLink2))
                                {
                                    savedItemIdsWithLink2 = objItemMaster.ID + "#" + objItemMaster.Link2.ToString();
                                }
                                else
                                {
                                    savedItemIdsWithLink2 += "," + objItemMaster.ID + "#" + objItemMaster.Link2.ToString();
                                }
                            }
                        }
                    }
                    /* ------- FOR SET SAVE ID AND ID WITH LINK2 ----------*/

                    List<BOMItemMasterMain> lst1 = new List<BOMItemMasterMain>();
                    List<BOMItemMasterMain> lst2 = (List<BOMItemMasterMain>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();

                    foreach (BOMItemMasterMain item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objItemMasterImport = LstItemMaster.Where(x => x.ItemNumber.Trim().ToUpper() == item.ItemNumber.Trim().ToUpper()).FirstOrDefault();
                        if (objItemMasterImport != null)
                        {
                            objItemMasterImport.Status = item.Status;
                            objItemMasterImport.Reason = item.Reason;
                            lstreturn.Add(objItemMasterImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankItemList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.ItemMasterImport item in CurrentBlankItemList)
                    {
                        lstreturn.Add(item);
                    }
                }

                /*==============FOR AUTOCART UPDATE=====================*/

                lstItemGUID = new List<Guid>();
                foreach (BOMItemMasterMain objitemguid in lstreturn1.Where(t => t.IsDeleted == false))
                {
                    if (objitemguid.Status.ToLower() == "success")
                    {
                        lstItemGUID.Add(objitemguid.GUID);
                    }
                }

                foreach (Guid gid in lstItemGUID)
                {
                    //new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(gid, SessionHelper.UserID, "Web", "ImportControler>> SaveImport");
                    new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(gid, SessionHelper.UserID, "Web", "BulkImport >> SaveItem");
                    new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetAndUpdateExtCostAndAvgCost(gid, SessionHelper.RoomID, SessionHelper.CompanyID);
                    new DashboardDAL(SessionHelper.EnterPriseDBName).SetItemsAutoClassification(gid, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, 1);
                }

                /*==============FOR AUTOCART UPDATE=====================*/


                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    //message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                    //status = ResMessage.SaveMessage;
                    //ClearCurrentResourceList();
                    //if (HasMoreRecords == false)
                    //    Session["importedData"] = null;
                    return true;
                }
                else
                {
                    //savedOnlyitemIds = string.Join(",", lstreturn.Where(i => i.ID != 0 && (!string.IsNullOrEmpty(i.ImagePath))).Select(p => p.ID.ToString()));
                    //savedItemIdsWithLink2 = string.Join(",", lstreturn.Where(i => i.ID != 0 && (!string.IsNullOrEmpty(i.Link2))).Select(p => p.ID.ToString() + "#" + p.Link2.ToString()));

                    ////objDTO.Link2 = item.Link2;
                    ////objDTO.ItemLink2ImageType = "InternalLink";

                    //foreach (BOMItemMasterMain b in lstreturn.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.ImagePath))))
                    //{
                    //    long id = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetIDByItemNumber(Convert.ToString(b.ItemNumber), Convert.ToInt64(b.Room), Convert.ToInt64(b.CompanyID));
                    //    savedOnlyitemIds += "," + id;
                    //}
                    //foreach (BOMItemMasterMain b in lstreturn.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.Link2))))
                    //{
                    //    ItemMasterDTO objItemMaster = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetRecordByItemNumber(Convert.ToString(b.ItemNumber), Convert.ToInt64(b.Room), Convert.ToInt64(b.CompanyID));
                    //    if (objItemMaster != null)
                    //    {
                    //        savedItemIdsWithLink2 += "," + objItemMaster.ID + "#" + objItemMaster.Link2.ToString();
                    //    }
                    //}
                    //message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                    //status = ResMessage.SaveMessage;
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    //SaveImportDataListSession<BOMItemMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                    //Session["importedData"] = lstreturn;
                    return false;
                }

                //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                //{
                //    allSuccesfulRecords = false;
                //}

                //lstItemGUID = new List<Guid>();
                //foreach (BOMItemMasterMain objitemguid in lstreturn.Where(t => t.IsDeleted == false))
                //{
                //    if (objitemguid.Status.ToLower() == "success")
                //    {
                //        lstItemGUID.Add(objitemguid.GUID);
                //    }
                //}

            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Import Category]
        private bool ImportCategory(List<ImportMastersNewDTO.CategoryMasterImport> LstCategoryMaster, out List<ImportMastersNewDTO.CategoryMasterImport> lstreturn, out string reason, out string status)
        {
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.CategoryMasterImport>();
            List<ImportMastersNewDTO.CategoryMasterImport> CurrentBlankCategoryList = new List<ImportMastersNewDTO.CategoryMasterImport>();
            List<CategoryMasterMain> CurrentCategoryList = null;
            List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            List<Guid> lstCategoryGUID = new List<Guid>();

            if (LstCategoryMaster != null && LstCategoryMaster.Count > 0)
            {
                CurrentCategoryList = new List<CategoryMasterMain>();
                lst = objImport.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.CategoryMaster.ToString(), UDFControlTypes.Textbox.ToString());
                eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetAllRecordsNew(SessionHelper.CompanyID, "CategoryMaster", SessionHelper.RoomID);
                CurrentOptionList = new List<UDFOptionsMain>();
                foreach (ImportMastersNewDTO.CategoryMasterImport item in LstCategoryMaster)
                {
                    string errorMsg = string.Empty;
                    CheckUDFIsRequired(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, out errorMsg);
                    if (!string.IsNullOrWhiteSpace(errorMsg))
                    {
                        item.Status = "Fail";
                        if (!string.IsNullOrEmpty(item.Reason))
                            item.Reason += errorMsg;
                        else
                            item.Reason = errorMsg;


                    }
                    if (string.IsNullOrEmpty(item.Category) || item.Status.Trim().ToLowerInvariant() == "fail")
                    {
                        CurrentBlankCategoryList.Add(item);
                        continue;
                    }
                    else
                    {
                        CategoryMasterMain objDTO = new CategoryMasterMain();
                        objDTO.ID = item.ID;
                        objDTO.Category = item.Category;

                        objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);

                        objDTO.CategoryColor = "";
                        objDTO.IsDeleted = item.IsDeleted ?? false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        lstCategoryGUID.Add(objDTO.GUID);


                        var itemval = CurrentCategoryList.FirstOrDefault(x => x.Category.Trim().ToUpperInvariant() == item.Category.Trim().ToUpperInvariant());
                        if (itemval != null)
                            CurrentCategoryList.Remove(itemval);
                        CurrentCategoryList.Add(objDTO);

                        item.Status = "Success";
                        item.Reason = "N/A";

                        CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportCategoryColumn.UDF1.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportCategoryColumn.UDF2.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportCategoryColumn.UDF3.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportCategoryColumn.UDF4.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportCategoryColumn.UDF5.ToString());


                    }
                }

                ImportMastersNewDTO.CategoryMasterImport objCategoryMasterImport;
                List<CategoryMasterMain> lstreturn1 = new List<CategoryMasterMain>();
                if (CurrentCategoryList.Count > 0)
                {
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.CategoryMaster.ToString(), CurrentCategoryList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                }
                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {
                    List<CategoryMasterMain> lst1 = new List<CategoryMasterMain>();
                    List<CategoryMasterMain> lst2 = (List<CategoryMasterMain>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();


                    foreach (CategoryMasterMain item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objCategoryMasterImport = LstCategoryMaster.Where(x => x.Category.Trim().ToUpper() == item.Category.Trim().ToUpper()).FirstOrDefault();
                        if (objCategoryMasterImport != null)
                        {
                            objCategoryMasterImport.Status = item.Status;
                            objCategoryMasterImport.Reason = item.Reason;
                            lstreturn.Add(objCategoryMasterImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankCategoryList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.CategoryMasterImport item in CurrentBlankCategoryList)
                    {
                        lstreturn.Add(item);
                    }
                }

                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }
            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Import Customer]
        private bool ImportCustomer(List<ImportMastersNewDTO.CustomerMasterImport> LstCustomerMaster, out List<ImportMastersNewDTO.CustomerMasterImport> lstreturn, out string reason, out string status)
        {
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.CustomerMasterImport>();
            List<ImportMastersNewDTO.CustomerMasterImport> CurrentBlankCustomerList = new List<ImportMastersNewDTO.CustomerMasterImport>();
            List<CustomerMasterMain> CurrentCustomerList = null;
            List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            List<Guid> lstCustomerGUID = new List<Guid>();

            if (LstCustomerMaster != null && LstCustomerMaster.Count > 0)
            {
                CurrentCustomerList = new List<CustomerMasterMain>();
                lst = objImport.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.CustomerMaster.ToString(), UDFControlTypes.Textbox.ToString());
                CurrentOptionList = new List<UDFOptionsMain>();
                foreach (ImportMastersNewDTO.CustomerMasterImport item in LstCustomerMaster)
                {

                    if (string.IsNullOrEmpty(item.Customer) || string.IsNullOrEmpty(item.Account) || item.Status.Trim().ToLowerInvariant() == "fail")
                    {
                        CurrentBlankCustomerList.Add(item);
                        continue;
                    }
                    else
                    {

                        CustomerMasterMain objDTO = new CustomerMasterMain();
                        objDTO.ID = item.ID;
                        objDTO.Customer = item.Customer;
                        objDTO.Account = item.Account;
                        objDTO.Contact = item.Contact;
                        objDTO.Address = item.Address;
                        objDTO.City = item.City;
                        objDTO.State = item.State;
                        objDTO.ZipCode = item.ZipCode;
                        objDTO.Country = item.Country;
                        objDTO.Phone = item.Phone;
                        objDTO.Email = item.Email;


                        objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);


                        objDTO.IsDeleted = item.IsDeleted ?? false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        lstCustomerGUID.Add(objDTO.GUID);


                        var itemval = CurrentCustomerList.FirstOrDefault(x => x.Customer.Trim().ToUpperInvariant() == item.Customer.Trim().ToUpperInvariant());
                        if (itemval != null)
                            CurrentCustomerList.Remove(itemval);
                        CurrentCustomerList.Add(objDTO);

                        item.Status = "Success";
                        item.Reason = "N/A";

                        CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportCustomerColumn.UDF1.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportCustomerColumn.UDF2.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportCustomerColumn.UDF3.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportCustomerColumn.UDF4.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportCustomerColumn.UDF5.ToString());

                    }
                }

                ImportMastersNewDTO.CustomerMasterImport objCustomerMasterImport;
                List<CustomerMasterMain> lstreturn1 = new List<CustomerMasterMain>();
                if (CurrentCustomerList.Count > 0)
                {
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.CustomerMaster.ToString(), CurrentCustomerList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                }
                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {
                    List<CustomerMasterMain> lst1 = new List<CustomerMasterMain>();
                    List<CustomerMasterMain> lst2 = (List<CustomerMasterMain>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();


                    foreach (CustomerMasterMain item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objCustomerMasterImport = LstCustomerMaster.Where(x => x.Customer.Trim().ToUpper() == item.Customer.Trim().ToUpper()).FirstOrDefault();
                        if (objCustomerMasterImport != null)
                        {
                            objCustomerMasterImport.Status = item.Status;
                            objCustomerMasterImport.Reason = item.Reason;
                            lstreturn.Add(objCustomerMasterImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankCustomerList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.CustomerMasterImport item in CurrentBlankCustomerList)
                    {
                        lstreturn.Add(item);
                    }
                }

                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }
            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Import Asset]
        private bool ImportAsset(List<ImportMastersNewDTO.AssetMasterImport> LstAssetMaster, bool isImgZipAvail, out List<ImportMastersNewDTO.AssetMasterImport> lstreturn, out string savedOnlyitemIds, out string reason, out string status)
        {
            savedOnlyitemIds = string.Empty;
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.AssetMasterImport>();
            List<ImportMastersNewDTO.AssetMasterImport> CurrentBlankAssetList = new List<ImportMastersNewDTO.AssetMasterImport>();
            List<AssetMasterMain> CurrentAssetList = null;
            List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            List<Guid> lstAssetGUID = new List<Guid>();

            if (LstAssetMaster != null && LstAssetMaster.Count > 0)
            {
                CurrentAssetList = new List<AssetMasterMain>();
                lst = objImport.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.AssetMaster.ToString(), UDFControlTypes.Textbox.ToString());
                eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetAllRecordsNew(SessionHelper.CompanyID, "AssetMaster", SessionHelper.RoomID);
                CurrentOptionList = new List<UDFOptionsMain>();
                bool saveData = true;
                foreach (ImportMastersNewDTO.AssetMasterImport item in LstAssetMaster)
                {
                    saveData = true;
                    AssetMasterMain objDTO = new AssetMasterMain();
                    if (!string.IsNullOrEmpty(item.PurchaseDateString))
                    {
                        try
                        {
                            objDTO.PurchaseDate = DateTime.ParseExact(item.PurchaseDateString, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                        }
                        catch
                        {
                            item.PurchaseDate = null;
                            item.Status = "Fail";
                            if (!string.IsNullOrEmpty(item.Reason))
                                item.Reason = item.Reason + Environment.NewLine + "Please enter " + SessionHelper.RoomDateFormat + " purchase date format";
                            else
                                item.Reason = "Please enter " + SessionHelper.RoomDateFormat + " purchase date format";

                            saveData = false;
                        }
                    }

                    string errorMsg = string.Empty;
                    CheckUDFIsRequired_Asset(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, item.UDF6, item.UDF7, item.UDF8, item.UDF9, item.UDF10, out errorMsg);
                    if (!string.IsNullOrWhiteSpace(errorMsg))
                    {
                        item.Status = "Fail";
                        if (!string.IsNullOrEmpty(item.Reason))
                            item.Reason += errorMsg;
                        else
                            item.Reason = errorMsg;
                        saveData = false;

                    }

                    if (string.IsNullOrEmpty(item.AssetName) || item.Status.Trim().ToLowerInvariant() == "fail" || saveData == false)
                    {
                        CurrentBlankAssetList.Add(item);
                        continue;
                    }
                    else
                    {

                        objDTO.ID = item.ID;
                        objDTO.AssetName = item.AssetName;
                        objDTO.Serial = item.Serial;
                        objDTO.Description = item.Description;
                        objDTO.Make = item.Make;
                        objDTO.Model = item.Model;
                        objDTO.AssetCategoryId = item.AssetCategoryName == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.AssetCategoryMaster, item.AssetCategoryName);
                        objDTO.AssetCategory = item.AssetCategoryName;
                        objDTO.DepreciatedValue = item.DepreciatedValue;
                        objDTO.PurchasePrice = item.PurchasePrice;

                        objDTO.UDF1 = item.UDF1;
                        objDTO.UDF2 = item.UDF2;
                        objDTO.UDF3 = item.UDF3;
                        objDTO.UDF4 = item.UDF4;
                        objDTO.UDF5 = item.UDF5;
                        objDTO.UDF6 = item.UDF6;
                        objDTO.UDF7 = item.UDF7;
                        objDTO.UDF8 = item.UDF8;
                        objDTO.UDF9 = item.UDF9;
                        objDTO.UDF10 = item.UDF10;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.IsAutoMaintain = true;
                        objDTO.MaintenanceType = 0;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        objDTO.ImageType = "ExternalImage";
                        objDTO.ImagePath = item.ImagePath;
                        objDTO.AssetImageExternalURL = item.AssetImageExternalURL;
                        if (string.IsNullOrWhiteSpace(item.AssetImageExternalURL) && (!string.IsNullOrEmpty(item.ImagePath)))
                        {
                            objDTO.ImageType = "ImagePath";
                        }

                        lstAssetGUID.Add(objDTO.GUID);

                        var itemval = CurrentAssetList.FirstOrDefault(x => x.AssetName.Trim().ToUpperInvariant() == item.AssetName.Trim().ToUpperInvariant());
                        if (itemval != null)
                            CurrentAssetList.Remove(itemval);
                        CurrentAssetList.Add(objDTO);

                        item.Status = "Success";
                        item.Reason = "N/A";

                        CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportAssetMasterColumn.UDF1.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportAssetMasterColumn.UDF2.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportAssetMasterColumn.UDF3.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportAssetMasterColumn.UDF4.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportAssetMasterColumn.UDF5.ToString());

                    }
                }

                ImportMastersNewDTO.AssetMasterImport objAssetMasterImport;
                List<AssetMasterMain> lstreturn1 = new List<AssetMasterMain>();
                if (CurrentAssetList.Count > 0)
                {
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.AssetMaster.ToString(), CurrentAssetList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList, isImgZipAvail);
                }
                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {
                    /* ------- FOR SET ID WITH IMAGEPATH ----------*/
                    if (isImgZipAvail)
                    {
                        List<AssetMasterMain> successSaveList = new List<AssetMasterMain>();
                        successSaveList = lstreturn1.Where(x => x.Status.ToUpper() == "SUCCESS").ToList();

                        savedOnlyitemIds = string.Join(",", successSaveList.Where(i => i.ID != 0 && (!string.IsNullOrEmpty(i.ImagePath))).Select(p => p.ID.ToString() + "#" + p.ImagePath.ToString()));

                        string AssetNameList = string.Join("@", successSaveList.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.ImagePath)) && i.Status != null && i.Status.ToLower() == "success").Select(a => a.AssetName));

                        List<AssetMasterDTO> assetList = new AssetMasterDAL(SessionHelper.EnterPriseDBName).GetRecordByAssetName(AssetNameList, SessionHelper.RoomID, SessionHelper.CompanyID);

                        foreach (AssetMasterDTO b in assetList)
                        {
                            AssetMasterMain objAssetMaster = successSaveList.Where(i => i.AssetName == b.AssetName && (!string.IsNullOrEmpty(i.ImagePath))).FirstOrDefault();
                            if (objAssetMaster != null)
                            {
                                if (string.IsNullOrEmpty(savedOnlyitemIds))
                                {
                                    savedOnlyitemIds = b.ID + "#" + objAssetMaster.ImagePath.ToString();
                                }
                                else
                                {
                                    savedOnlyitemIds += "," + b.ID + "#" + objAssetMaster.ImagePath.ToString();
                                }
                            }
                        }
                    }
                    /* ------- FOR SET ID WITH IMAGEPATH ----------*/

                    List<AssetMasterMain> lst1 = new List<AssetMasterMain>();
                    List<AssetMasterMain> lst2 = (List<AssetMasterMain>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();

                    foreach (AssetMasterMain item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objAssetMasterImport = LstAssetMaster.Where(x => x.AssetName.Trim().ToUpper() == item.AssetName.Trim().ToUpper()).FirstOrDefault();
                        if (objAssetMasterImport != null)
                        {
                            objAssetMasterImport.Status = item.Status;
                            objAssetMasterImport.Reason = item.Reason;
                            lstreturn.Add(objAssetMasterImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankAssetList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.AssetMasterImport item in CurrentBlankAssetList)
                    {
                        lstreturn.Add(item);
                    }
                }

                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }
            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Import Tool]
        private bool ImportTool(List<ImportMastersNewDTO.ToolMasterImport> LstToolMaster, bool isImgZipAvail, out List<ImportMastersNewDTO.ToolMasterImport> lstreturn, out string savedOnlyitemIds, out string reason, out string status)
        {
            savedOnlyitemIds = string.Empty;
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.ToolMasterImport>();
            List<ImportMastersNewDTO.ToolMasterImport> CurrentBlankToolList = new List<ImportMastersNewDTO.ToolMasterImport>();
            List<ToolMasterMain> CurrentToolList = null;
            List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            List<Guid> lstToolGUID = new List<Guid>();

            if (LstToolMaster != null && LstToolMaster.Count > 0)
            {
                CurrentToolList = new List<ToolMasterMain>();
                lst = objImport.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.ToolMaster.ToString(), UDFControlTypes.Textbox.ToString());
                eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetAllRecordsNew(SessionHelper.CompanyID, "ToolMaster", SessionHelper.RoomID);
                IEnumerable<UDFDTO> UDFDataFromDB_ToolCheckOut = objUDFDAL.GetAllRecordsNew(SessionHelper.CompanyID, "ToolCheckInOutHistory", SessionHelper.RoomID);
                CurrentOptionList = new List<UDFOptionsMain>();
                List<TechnicianMasterDTO> objTechnicialMasterDALList = new List<TechnicianMasterDTO>();
                TechnicialMasterDAL objTechnicialMasterDAL = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
                objTechnicialMasterDALList = objTechnicialMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();

                bool saveData = true;

                /***********TOOL TECHNICIAN*************/
                foreach (ImportMastersNewDTO.ToolMasterImport toolList in LstToolMaster.GroupBy(l => l.Technician).Select(g => g.First()).ToList())
                {
                    if (!string.IsNullOrEmpty(toolList.Technician))
                    {
                        string _Technician = string.Empty;
                        string _TechnicianCode = string.Empty;
                        if (toolList.Technician.Contains(" --- "))
                        {
                            string[] techNameAndCode = toolList.Technician.Split(new string[1] { " --- " }, StringSplitOptions.RemoveEmptyEntries);
                            if (techNameAndCode.Length > 1)
                            {
                                _TechnicianCode = techNameAndCode[0];
                                _Technician = techNameAndCode[1];
                                toolList.Technician = _Technician;
                                toolList.TechnicianCode = _TechnicianCode;
                            }
                        }
                        if ((from p in objTechnicialMasterDALList
                             where (p.TechnicianCode.ToLower().Trim() == (toolList.Technician.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                             select p).Any())
                        {

                        }
                        else
                        {
                            TechnicianMasterDTO objTechnicianMasterDTO = new TechnicianMasterDTO();
                            objTechnicianMasterDTO.TechnicianCode = toolList.Technician;
                            objTechnicianMasterDTO.Room = SessionHelper.RoomID;
                            objTechnicianMasterDTO.CompanyID = SessionHelper.CompanyID;
                            objTechnicianMasterDTO.CreatedBy = SessionHelper.UserID;
                            objTechnicianMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                            objTechnicianMasterDTO.Created = DateTimeUtility.DateTimeNow;
                            objTechnicianMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                            objTechnicianMasterDTO.GUID = Guid.NewGuid();
                            objTechnicianMasterDTO.IsArchived = false;
                            objTechnicianMasterDTO.IsDeleted = false;
                            Int64 TechnicanID = objTechnicialMasterDAL.Insert(objTechnicianMasterDTO);
                            objTechnicianMasterDTO = objTechnicialMasterDAL.GetRecord(TechnicanID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                            objTechnicialMasterDALList.Add(objTechnicianMasterDTO);

                        }
                    }
                }
                /***********TOOL TECHNICIAN*************/

                /***********TOOL CATEGORY*************/
                List<ToolCategoryMasterDTO> objToolCategoryMasterDTOList = new List<ToolCategoryMasterDTO>();
                ToolCategoryMasterDAL objToolCategoryMasterDAL = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
                objToolCategoryMasterDTOList = objToolCategoryMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                foreach (ImportMastersNewDTO.ToolMasterImport toolList in LstToolMaster.GroupBy(l => l.ToolCategoryName).Select(g => g.First()).ToList())
                {
                    if (toolList.ToolCategoryName.ToLower().Trim() != string.Empty)
                    {
                        if ((from p in objToolCategoryMasterDTOList
                             where (p.ToolCategory.ToLower().Trim() == (toolList.ToolCategoryName.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                             select p).Any())
                        {

                        }
                        else
                        {
                            ToolCategoryMasterDAL objDAL = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
                            ToolCategoryMasterDTO objDTO = new ToolCategoryMasterDTO();
                            Int64 ToolCategoryID = GetIDs(ImportMastersDTO.TableName.ToolCategoryMaster, toolList.ToolCategoryName);
                            objDTO = objToolCategoryMasterDAL.GetRecord(ToolCategoryID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                            objToolCategoryMasterDTOList.Add(objDTO);

                        }
                    }
                }
                /***********TOOL CATEGORY*************/

                /***********TOOL LOCATION*************/
                List<LocationMasterDTO> objLocationMasterDTOList = new List<LocationMasterDTO>();
                LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                objLocationMasterDTOList = objLocationMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                foreach (ImportMastersNewDTO.ToolMasterImport toolList in LstToolMaster.GroupBy(l => l.Location).Select(g => g.First()).ToList())
                {
                    if (toolList.Location.ToLower().Trim() != string.Empty)
                    {
                        if ((from p in objLocationMasterDTOList
                             where (p.Location.ToLower().Trim() == (toolList.Location.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                             select p).Any())
                        {

                        }
                        else
                        {
                            LocationMasterDAL objDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                            LocationMasterDTO objDTO = new LocationMasterDTO();
                            Int64 LocationMasterID = GetIDs(ImportMastersDTO.TableName.LocationMaster, toolList.Location);
                            objDTO = objDAL.GetRecord(LocationMasterID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                            objLocationMasterDTOList.Add(objDTO);

                        }
                    }
                }
                /***********TOOL LOCATION*************/
                bool SaveToolList = true;
                ToolMasterDAL objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
                foreach (ImportMastersNewDTO.ToolMasterImport item in LstToolMaster)
                {
                    ToolMasterMain objDTO = new ToolMasterMain();
                    if ((item.IsGroupOfItemsBool == false && item.Quantity <= 0) && (item.IsGroupOfItems.HasValue && item.IsGroupOfItems <= 0))
                    {
                        SaveToolList = false;
                        item.Status = "Fail";
                        if (!string.IsNullOrEmpty(item.Reason))
                            item.Reason = item.Reason + Environment.NewLine + ResMessage.DuplicateSerialNumber;
                        else
                            item.Reason = ResMessage.DuplicateSerialNumber;
                    }
                    objToolMasterDTO = objToolDAL.GetToolNameBySerial(item.Serial, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objToolMasterDTO != null)
                    {
                        if (objToolMasterDTO.ToolName == item.ToolName)
                        {
                            objDTO.Serial = item.Serial;
                        }
                        else
                        {
                            SaveToolList = false;
                            item.Status = "Fail";
                            if (!string.IsNullOrEmpty(item.Reason))
                                item.Reason = item.Reason + Environment.NewLine + ResMessage.DuplicateSerialNumber;
                            else
                                item.Reason = ResMessage.DuplicateSerialNumber;
                            SaveToolList = false;
                        }
                    }
                    else
                    {
                        objDTO.Serial = item.Serial;
                    }

                    string errorMsg = string.Empty;
                    CheckUDFIsRequired(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, out errorMsg);
                    if (!string.IsNullOrWhiteSpace(errorMsg))
                    {
                        item.Status = "Fail";
                        if (!string.IsNullOrEmpty(item.Reason))
                            item.Reason += errorMsg;
                        else
                            item.Reason = errorMsg;
                        SaveToolList = false;
                    }

                    if (item.CheckOutQuantity.HasValue && item.CheckOutQuantity.Value > 0)
                    {
                        string checkOutErrorMsg = string.Empty;
                        CheckUDFIsRequired(UDFDataFromDB_ToolCheckOut, item.CheckOutUDF1, item.CheckOutUDF2, item.CheckOutUDF3, item.CheckOutUDF4, item.CheckOutUDF5, out checkOutErrorMsg, "CheckOut");
                        if (!string.IsNullOrWhiteSpace(checkOutErrorMsg))
                        {
                            item.Status = "Fail";
                            if (!string.IsNullOrEmpty(item.Reason))
                                item.Reason += checkOutErrorMsg;
                            else
                                item.Reason = checkOutErrorMsg;
                            SaveToolList = false;
                        }
                    }
                    /*////////CHECK FOR CHECKOUT AND CHECKIN QTY IS ENTERED OR NOT FOR TECHNICIAN//////////*/
                    if ((item.CheckInQuantity > 0 || item.CheckOutQuantity > 0) && string.IsNullOrEmpty(item.Technician))
                    {
                        SaveToolList = false;
                        item.Status = "Fail";
                        if (!string.IsNullOrEmpty(item.Reason))
                            item.Reason = item.Reason + Environment.NewLine + "Technician is Required.";
                        else
                            item.Reason = "Technician is Required.";
                    }
                    /*////////CHECK FOR CHECKOUT AND CHECKIN QTY IS ENTERED OR NOT FOR TECHNICIAN//////////*/

                    if (string.IsNullOrEmpty(item.ToolName) || item.Status.Trim().ToLowerInvariant() == "fail" || saveData == false)
                    {
                        CurrentBlankToolList.Add(item);
                        continue;
                    }
                    else
                    {

                        objDTO.ID = item.ID;
                        objDTO.ToolName = item.ToolName;

                        objDTO.Description = item.Description;
                        objDTO.ToolCategory = item.ToolCategoryName;
                        //objDTO.ToolCategoryID = item.ToolCategory == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.ToolCategoryMaster, item.ToolCategory);
                        if (!string.IsNullOrWhiteSpace(item.ToolCategoryName))
                        {
                            if ((from p in objToolCategoryMasterDTOList
                                 where (p.ToolCategory.ToLower().Trim() == (item.ToolCategoryName.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                 select p).Any())
                            {
                                objDTO.ToolCategoryID = (from p in objToolCategoryMasterDTOList
                                                         where (p.ToolCategory.ToLower().Trim() == (item.ToolCategoryName.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                                         select p).FirstOrDefault().ID;
                            }
                            else
                            {
                                objDTO.ToolCategoryID = (long?)null;
                            }

                        }
                        else
                        {
                            objDTO.ToolCategoryID = (long?)null;
                        }
                        objDTO.ToolCategory = item.ToolCategoryName;

                        //objDTO.LocationID = item.Location == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.LocationMaster, item.Location);
                        if (!string.IsNullOrWhiteSpace(item.Location))
                        {
                            if ((from p in objLocationMasterDTOList
                                 where (p.Location.ToLower().Trim() == (item.Location.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                 select p).Any())
                            {
                                objDTO.LocationID = (from p in objLocationMasterDTOList
                                                     where (p.Location.ToLower().Trim() == (item.Location.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                                     select p).FirstOrDefault().ID;
                            }
                            else
                            {
                                objDTO.LocationID = (long?)null;
                            }

                        }
                        else
                        {
                            objDTO.LocationID = (long?)null;
                        }
                        objDTO.Location = item.Location;


                        objDTO.IsGroupOfItems = item.IsGroupOfItems;
                        //objDTO.IsGroupOfItems = 0;// item.IsGroupOfItems == "Yes" ? 1 : 0;
                        objDTO.Cost = item.Cost ?? 0;
                        if (!item.IsGroupOfItemsBool && item.IsGroupOfItems.HasValue && item.IsGroupOfItems <= 0)
                        {
                            objDTO.Quantity = 1;
                        }
                        else
                        {
                            objDTO.Quantity = item.Quantity;
                        }
                        objDTO.UDF1 = item.UDF1;
                        objDTO.UDF2 = item.UDF2;
                        objDTO.UDF3 = item.UDF3;
                        objDTO.UDF4 = item.UDF4;
                        objDTO.UDF5 = item.UDF5;
                        objDTO.CheckOutUDF1 = item.CheckOutUDF1;
                        objDTO.CheckOutUDF2 = item.CheckOutUDF2;
                        objDTO.CheckOutUDF3 = item.CheckOutUDF3;
                        objDTO.CheckOutUDF4 = item.CheckOutUDF4;
                        objDTO.CheckOutUDF5 = item.CheckOutUDF5;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.CheckOutQuantity = item.CheckOutQuantity;
                        objDTO.CheckInQuantity = item.CheckInQuantity;

                        objDTO.ImageType = "ExternalImage";
                        objDTO.ImagePath = item.ImagePath;
                        objDTO.ToolImageExternalURL = item.ToolImageExternalURL;
                        if (string.IsNullOrWhiteSpace(item.ToolImageExternalURL) && (!string.IsNullOrEmpty(item.ImagePath)))
                        {
                            objDTO.ImageType = "ImagePath";
                        }

                        if (!string.IsNullOrWhiteSpace(item.Technician))
                        {
                            if ((from p in objTechnicialMasterDALList
                                 where (p.TechnicianCode.ToLower().Trim() == (item.Technician.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                 select p).Any())
                            {
                                objDTO.TechnicianGuid = (from p in objTechnicialMasterDALList
                                                         where (p.TechnicianCode.ToLower().Trim() == (item.Technician.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                                         select p).FirstOrDefault().GUID;
                            }

                        }
                        objDTO.Technician = item.Technician;

                        lstToolGUID.Add(objDTO.GUID);

                        var itemval = CurrentToolList.FirstOrDefault(x => x.ToolName == item.ToolName && (x.Serial ?? string.Empty) == (item.Serial ?? string.Empty));
                        if (itemval != null)
                            CurrentToolList.Remove(itemval);

                        CurrentToolList.Add(objDTO);

                        CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportToolMasterColumn.UDF1.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportToolMasterColumn.UDF2.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportToolMasterColumn.UDF3.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportToolMasterColumn.UDF4.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportToolMasterColumn.UDF5.ToString());
                    }
                }

                ImportMastersNewDTO.ToolMasterImport objToolMasterImport;
                List<ToolMasterMain> lstreturn1 = new List<ToolMasterMain>();
                if (CurrentToolList.Count > 0)
                {
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.ToolMaster.ToString(), CurrentToolList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList, isImgZipAvail);
                }
                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {

                    /***********QUANTITY CHECKIN CHECKOUT************/
                    ImportDAL obj = new ImportDAL(SessionHelper.EnterPriseDBName);
                    ToolCheckInHistoryDAL objCIDAL = new ToolCheckInHistoryDAL(SessionHelper.EnterPriseDBName);
                    objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                    ToolCheckInOutHistoryDAL objCICODAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
                    bool AllowCheckinCheckOut = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowCheckInCheckout);
                    if (AllowCheckinCheckOut)
                    {
                        lst = obj.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.ToolCheckInOutHistory.ToString(), UDFControlTypes.Textbox.ToString());
                        foreach (ToolMasterMain item in lstreturn1.Where(l => l.Status != null && l.Status.ToLower() == "success" && l.CheckOutQuantity > 0))
                        {
                            ToolCheckInHistoryDTO objCIDTO = new ToolCheckInHistoryDTO();
                            ToolCheckInOutHistoryDTO objCICODTO = new ToolCheckInOutHistoryDTO();
                            ToolMasterDTO objtool = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList().Where(t => t.ToolName == item.ToolName && (t.Serial ?? string.Empty) == (item.Serial ?? string.Empty)).FirstOrDefault();

                            objCICODTO.CompanyID = SessionHelper.CompanyID;
                            objCICODTO.Created = DateTimeUtility.DateTimeNow;
                            objCICODTO.CreatedBy = SessionHelper.UserID;
                            objCICODTO.CreatedByName = SessionHelper.UserName;
                            objCICODTO.IsArchived = false;
                            objCICODTO.IsDeleted = false;
                            objCICODTO.LastUpdatedBy = SessionHelper.UserID;
                            objCICODTO.Room = SessionHelper.RoomID;
                            objCICODTO.RoomName = SessionHelper.RoomName;
                            objCICODTO.ToolGUID = objtool.GUID;
                            objCICODTO.Updated = DateTimeUtility.DateTimeNow;
                            objCICODTO.UpdatedByName = SessionHelper.UserName;
                            if (!string.IsNullOrEmpty(item.Technician))
                            {
                                if ((from p in objTechnicialMasterDALList
                                     where (p.TechnicianCode.ToLower().Trim() == (item.Technician.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                     select p).Any())
                                {
                                    objCICODTO.TechnicianGuid = (from p in objTechnicialMasterDALList
                                                                 where (p.TechnicianCode.ToLower().Trim() == (item.Technician.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                                                 select p).FirstOrDefault().GUID;
                                }
                            }
                            // objCICODTO.TechnicianGuid = item.TechnicianGuid;


                            //Save CheckOut UDF
                            objCICODTO.UDF1 = item.CheckOutUDF1;
                            objCICODTO.UDF2 = item.CheckOutUDF2;
                            objCICODTO.UDF3 = item.CheckOutUDF3;
                            objCICODTO.UDF4 = item.CheckOutUDF4;
                            objCICODTO.UDF5 = item.CheckOutUDF5;

                            objCICODTO.AddedFrom = "Web";
                            objCICODTO.EditedFrom = "Web";
                            objCICODTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objCICODTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objCICODTO.IsOnlyFromItemUI = true;

                            if (objtool.Quantity == null)
                                objtool.Quantity = 0;

                            if (objtool.CheckedOutQTY == null)
                                objtool.CheckedOutQTY = 0;

                            if (objtool.CheckedOutMQTY == null)
                                objtool.CheckedOutMQTY = 0;

                            if (item.CheckOutQuantity == null)
                                item.CheckOutQuantity = 0;

                            objCICODTO.CheckedOutQTY = objtool.Quantity >= (objtool.CheckedOutQTY + objtool.CheckedOutMQTY + item.CheckOutQuantity) ? (item.CheckOutQuantity) : (objtool.Quantity - (objtool.CheckedOutQTY + objtool.CheckedOutMQTY));
                            objCICODTO.CheckedOutMQTY = 0;

                            objCICODTO.CheckedOutQTYCurrent = 0;

                            objCICODTO.CheckOutDate = DateTimeUtility.DateTimeNow;
                            objCICODTO.CheckOutStatus = "Check Out";

                            if (objCICODTO.CheckedOutQTY > 0)
                            {
                                objCICODAL.Insert(objCICODTO);
                            }

                            ToolMasterDTO objToolDTO = objToolDAL.GetToolListByID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objtool.GUID, null);
                            objToolDTO.CheckedOutQTY = objToolDTO.CheckedOutQTY.GetValueOrDefault(0) + objCICODTO.CheckedOutQTY;
                            objToolDTO.EditedFrom = "Web";
                            objToolDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objToolDTO.IsOnlyFromItemUI = true;
                            if (item.CheckOutUDF1 != null && item.CheckOutUDF1 != string.Empty)
                                InsertCheckOutUDf(item.CheckOutUDF1, CommonUtility.ImportToolMasterColumn.UDF1.ToString());
                            if (item.CheckOutUDF2 != null && item.CheckOutUDF2 != string.Empty)
                                InsertCheckOutUDf(item.CheckOutUDF2, CommonUtility.ImportToolMasterColumn.UDF2.ToString());
                            if (item.CheckOutUDF3 != null && item.CheckOutUDF3 != string.Empty)
                                InsertCheckOutUDf(item.CheckOutUDF3, CommonUtility.ImportToolMasterColumn.UDF3.ToString());
                            if (item.CheckOutUDF4 != null && item.CheckOutUDF4 != string.Empty)
                                InsertCheckOutUDf(item.CheckOutUDF4, CommonUtility.ImportToolMasterColumn.UDF4.ToString());
                            if (item.CheckOutUDF5 != null && item.CheckOutUDF5 != string.Empty)
                                InsertCheckOutUDf(item.CheckOutUDF5, CommonUtility.ImportToolMasterColumn.UDF5.ToString());

                            objToolDAL.Edit(objToolDTO);

                        }
                    }
                    objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                    objCIDAL = new ToolCheckInHistoryDAL(SessionHelper.EnterPriseDBName);
                    objCICODAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
                    if (AllowCheckinCheckOut)
                    {
                        foreach (ToolMasterMain item in lstreturn1.Where(l => l.Status != null && l.Status.ToLower() == "success" && l.CheckInQuantity > 0))
                        {

                            ToolCheckInHistoryDTO objCIDTO = new ToolCheckInHistoryDTO();
                            ToolCheckInOutHistoryDTO objCICODTO = new ToolCheckInOutHistoryDTO();

                            List<ToolCheckInOutHistoryDTO> objToolCheckInOutHistoryDTOList = objCICODAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                            ToolMasterDTO objtool = objToolDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList().Where(t => t.ToolName == item.ToolName && (t.Serial ?? string.Empty) == (item.Serial ?? string.Empty)).FirstOrDefault();

                            objToolCheckInOutHistoryDTOList = objToolCheckInOutHistoryDTOList.Where(co => co.ToolGUID == objtool.GUID).ToList();

                            objCIDTO.CompanyID = SessionHelper.CompanyID;
                            objCIDTO.Created = DateTimeUtility.DateTimeNow;
                            objCIDTO.CreatedBy = SessionHelper.UserID;
                            objCIDTO.CreatedByName = SessionHelper.UserName;
                            objCIDTO.IsArchived = false;
                            objCIDTO.IsDeleted = false;
                            objCIDTO.LastUpdatedBy = SessionHelper.UserID;
                            objCIDTO.Room = SessionHelper.RoomID;
                            objCIDTO.RoomName = SessionHelper.RoomName;

                            //if (CheckInCheckOutGUID != "" && Guid.Parse(CheckInCheckOutGUID) != Guid.Empty)
                            //    objCIDTO.CheckInCheckOutGUID = Guid.Parse(CheckInCheckOutGUID);
                            objCIDTO.Updated = DateTimeUtility.DateTimeNow;
                            objCIDTO.UpdatedByName = SessionHelper.UserName;


                            objCIDTO.CheckedOutMQTY = 0;

                            objCIDTO.CheckInDate = DateTimeUtility.DateTimeNow;
                            objCIDTO.CheckOutStatus = "Check In";
                            objCIDTO.IsOnlyFromItemUI = true;
                            objCIDTO.AddedFrom = "Web";
                            objCIDTO.EditedFrom = "Web";
                            objCIDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objCIDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            //objCIDTO.TechnicianGuid = item.TechnicianGuid;
                            if (!string.IsNullOrEmpty(item.Technician))
                            {
                                if ((from p in objTechnicialMasterDALList
                                     where (p.TechnicianCode.ToLower().Trim() == (item.Technician.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                     select p).Any())
                                {
                                    objCIDTO.TechnicianGuid = (from p in objTechnicialMasterDALList
                                                               where (p.TechnicianCode.ToLower().Trim() == (item.Technician.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                                               select p).FirstOrDefault().GUID;
                                }
                            }
                            double checkinQty = item.CheckInQuantity ?? 0;
                            foreach (var COHistroy in objToolCheckInOutHistoryDTOList.Where(o => o.CheckedOutQTY > o.CheckedOutQTYCurrent && o.TechnicianGuid == objCIDTO.TechnicianGuid))
                            {
                                if (checkinQty > 0)
                                {
                                    objCIDTO.CheckedOutQTY = COHistroy.CheckedOutQTY - COHistroy.CheckedOutQTYCurrent >= checkinQty ? checkinQty : COHistroy.CheckedOutQTY - COHistroy.CheckedOutQTYCurrent;
                                    objCIDTO.CheckInCheckOutGUID = COHistroy.GUID;
                                    objCIDTO.TechnicianGuid = COHistroy.TechnicianGuid;
                                    objCIDAL.Insert(objCIDTO);
                                    checkinQty = checkinQty - objCIDTO.CheckedOutQTY ?? 0;

                                    ToolCheckInOutHistoryDTO objPrvCICODTO = objCICODAL.GetRecord(COHistroy.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                                    if (objPrvCICODTO != null)
                                    {
                                        objPrvCICODTO.CheckedOutQTYCurrent = objPrvCICODTO.CheckedOutQTYCurrent.GetValueOrDefault(0) + objCIDTO.CheckedOutQTY;

                                        objPrvCICODTO.IsOnlyFromItemUI = true;
                                        objPrvCICODTO.EditedFrom = "Web";
                                        objPrvCICODTO.ReceivedOn = DateTimeUtility.DateTimeNow;

                                        objCICODAL.Edit(objPrvCICODTO);
                                    }
                                }
                            }
                            checkinQty = (item.CheckInQuantity ?? 0) - checkinQty;
                            objtool.IsOnlyFromItemUI = true;

                            objtool.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objtool.EditedFrom = "Web";
                            objtool.CheckedOutQTY = objtool.CheckedOutQTY > checkinQty ? objtool.CheckedOutQTY - checkinQty : objtool.CheckedOutQTY - objtool.CheckedOutQTY;
                            objToolDAL.Edit(objtool);

                        }
                    }
                    /***********QUANTITY CHECKIN CHECKOUT*************/


                    /* ------- FOR SET ID WITH IMAGEPATH ----------*/
                    if (isImgZipAvail)
                    {
                        List<ToolMasterMain> successSaveList = new List<ToolMasterMain>();
                        successSaveList = lstreturn1.Where(x => x.Status.ToUpper() == "SUCCESS").ToList();

                        savedOnlyitemIds = string.Join(",", successSaveList.Where(i => i.ID != 0 && (!string.IsNullOrEmpty(i.ImagePath))).Select(p => p.ID.ToString() + "#" + p.ImagePath.ToString()));

                        string SerialNameList = string.Join("@", successSaveList.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.ImagePath)) && i.Status != null && i.Status.ToLower() == "success").Select(a => a.Serial));

                        string ToolNameList = string.Join("@", successSaveList.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.ImagePath)) && i.Status != null && i.Status.ToLower() == "success").Select(a => a.ToolName));

                        List<ToolMasterDTO> toolList = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetRecordBySerial(ToolNameList, SerialNameList, SessionHelper.RoomID, SessionHelper.CompanyID);

                        foreach (ToolMasterDTO b in toolList)
                        {
                            ToolMasterMain objToolMaster = successSaveList.Where(i => i.ToolName == b.ToolName && i.Serial == b.Serial && (!string.IsNullOrEmpty(i.ImagePath))).FirstOrDefault();
                            if (objToolMaster != null)
                            {
                                if (string.IsNullOrEmpty(savedOnlyitemIds))
                                {
                                    savedOnlyitemIds = b.ID + "#" + objToolMaster.ImagePath.ToString();
                                }
                                else
                                {
                                    savedOnlyitemIds += "," + b.ID + "#" + objToolMaster.ImagePath.ToString();
                                }
                            }
                        }
                    }
                    /* ------- FOR SET ID WITH IMAGEPATH ----------*/

                    List<ToolMasterMain> lst1 = new List<ToolMasterMain>();
                    List<ToolMasterMain> lst2 = (List<ToolMasterMain>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();

                    foreach (ToolMasterMain item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objToolMasterImport = LstToolMaster.Where(x => x.ToolName.Trim().ToUpper() == item.ToolName.Trim().ToUpper() && x.Serial.Trim().ToUpper() == item.Serial.ToUpper()).FirstOrDefault();
                        if (objToolMasterImport != null)
                        {
                            objToolMasterImport.Status = item.Status;
                            objToolMasterImport.Reason = item.Reason;
                            lstreturn.Add(objToolMasterImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankToolList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.ToolMasterImport item in CurrentBlankToolList)
                    {
                        lstreturn.Add(item);
                    }
                }

                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }
            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Import Supplier]
        private bool ImportSupplier(List<ImportMastersNewDTO.SupplierMasterImport> LstSupplierMaster, bool isImgZipAvail, out List<ImportMastersNewDTO.SupplierMasterImport> lstreturn, out string savedOnlyitemIds, out string reason, out string status)
        {
            savedOnlyitemIds = string.Empty;
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.SupplierMasterImport>();
            List<ImportMastersNewDTO.SupplierMasterImport> CurrentBlankSupplierList = new List<ImportMastersNewDTO.SupplierMasterImport>();
            List<SupplierMasterMain> CurrentSupplierList = null;
            List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            List<Guid> lstSupplierGUID = new List<Guid>();

            if (LstSupplierMaster != null && LstSupplierMaster.Count > 0)
            {
                CurrentSupplierList = new List<SupplierMasterMain>();
                lst = objImport.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.SupplierMaster.ToString(), UDFControlTypes.Textbox.ToString());
                CurrentOptionList = new List<UDFOptionsMain>();

                bool SaveSupplierList = true;
                SupplierMasterDAL objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                SupplierMasterDTO objSupplierMasterDTO = new SupplierMasterDTO();
                foreach (ImportMastersNewDTO.SupplierMasterImport item in LstSupplierMaster)
                {
                    SupplierMasterMain objDTO = new SupplierMasterMain();

                    if (!string.IsNullOrEmpty(item.StartDateString))
                    {
                        try
                        {
                            objDTO.StartDate = DateTime.ParseExact(item.StartDateString, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                        }
                        catch
                        {
                            item.StartDate = null;
                            item.Status = "Fail";
                            if (!string.IsNullOrEmpty(item.Reason))
                                item.Reason = item.Reason + Environment.NewLine + "Please enter " + SessionHelper.RoomDateFormat + " start date format";
                            else
                                item.Reason = "Please enter " + SessionHelper.RoomDateFormat + " start date format";


                        }
                    }

                    if (!string.IsNullOrEmpty(item.EndDateString))
                    {
                        try
                        {
                            objDTO.EndDate = DateTime.ParseExact(item.EndDateString, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                        }
                        catch
                        {
                            item.EndDate = null;
                            item.Status = "Fail";
                            if (!string.IsNullOrEmpty(item.Reason))
                                item.Reason = item.Reason + Environment.NewLine + "Please enter " + SessionHelper.RoomDateFormat + " end date format";
                            else
                                item.Reason = "Please enter " + SessionHelper.RoomDateFormat + " end date format";
                        }
                    }

                    if (objDTO.StartDate.HasValue && objDTO.EndDate.HasValue)
                    {
                        if (objDTO.EndDate.Value.Date < objDTO.StartDate.Value.Date)
                        {
                            item.StartDate = null;
                            item.EndDate = null;
                            objDTO.StartDate = null;
                            objDTO.EndDate = null;
                            item.Status = "Fail";
                            if (!string.IsNullOrEmpty(item.Reason))
                                item.Reason = item.Reason + Environment.NewLine + "Enddate should not be less then start date.";
                            else
                                item.Reason = "Enddate should not be less then start date.";
                        }
                    }

                    if (string.IsNullOrEmpty(item.SupplierName) || item.Status.Trim().ToLowerInvariant() == "fail")
                    {
                        CurrentBlankSupplierList.Add(item);
                        continue;
                    }
                    else
                    {
                        objDTO.ID = item.ID;
                        objDTO.SupplierName = (item.SupplierName == null) ? null : (item.SupplierName.Length > 255 ? item.SupplierName.Substring(0, 255) : item.SupplierName);
                        objDTO.SupplierColor = (item.SupplierColor == null) ? null : (item.SupplierColor.Length > 255 ? item.SupplierColor.Substring(0, 255) : item.SupplierColor);
                        objDTO.Description = (item.Description == null) ? null : (item.Description.Length > 1024 ? item.Description.Substring(0, 1024) : item.Description);
                        objDTO.BranchNumber = (item.BranchNumber == null) ? null : (item.BranchNumber.Length > 255 ? item.BranchNumber.Substring(0, 255) : item.BranchNumber);
                        objDTO.MaximumOrderSize = item.MaximumOrderSize;
                        //objDTO.AccountNo = (item.AccountNo == null) ? null : (item.AccountNo.Length > 64 ? item.AccountNo.Substring(0, 64) : item.AccountNo);
                        //objDTO.ReceiverID = (item.ReceiverID == null) ? null : (item.ReceiverID.Length > 64 ? item.ReceiverID.Substring(0, 64) : item.ReceiverID);
                        objDTO.Address = (item.Address == null) ? null : (item.Address.Length > 1027 ? item.Address.Substring(0, 1027) : item.Address);
                        objDTO.City = (item.City == null) ? null : (item.City.Length > 127 ? item.City.Substring(0, 127) : item.City);
                        objDTO.State = (item.State == null) ? null : (item.State.Length > 255 ? item.State.Substring(0, 255) : item.State);
                        objDTO.ZipCode = (item.ZipCode == null) ? null : (item.ZipCode.Length > 20 ? item.ZipCode.Substring(0, 20) : item.ZipCode);
                        objDTO.Country = (item.Country == null) ? null : (item.Country.Length > 127 ? item.Country.Substring(0, 127) : item.Country);
                        objDTO.Contact = (item.Contact == null) ? null : (item.Contact.Length > 127 ? item.Contact.Substring(0, 127) : item.Contact);
                        objDTO.Phone = (item.Phone == null) ? null : (item.Phone.Length > 20 ? item.Phone.Substring(0, 20) : item.Phone);
                        objDTO.Fax = (item.Fax == null) ? null : (item.Fax.Length > 20 ? item.Fax.Substring(0, 20) : item.Fax);
                        //objDTO.Email = (item.Email == null) ? null : (item.Email.Length > 255 ? item.Email.Substring(0, 255) : item.Email);

                        objDTO.IsSendtoVendor = item.IsSendtoVendor;
                        objDTO.IsVendorReturnAsn = item.IsVendorReturnAsn;
                        objDTO.IsSupplierReceivesKitComponents = item.IsSupplierReceivesKitComponents;

                        objDTO.IsEmailPOInBody = item.IsEmailPOInBody;
                        objDTO.IsEmailPOInPDF = item.IsEmailPOInPDF;
                        objDTO.IsEmailPOInCSV = item.IsEmailPOInCSV;
                        objDTO.IsEmailPOInX12 = item.IsEmailPOInX12;

                        objDTO.OrderNumberTypeBlank = item.OrderNumberTypeBlank;
                        objDTO.OrderNumberTypeFixed = item.OrderNumberTypeFixed;
                        objDTO.OrderNumberTypeBlanketOrderNumber = item.OrderNumberTypeBlanketOrderNumber;
                        objDTO.OrderNumberTypeIncrementingOrderNumber = item.OrderNumberTypeIncrementingOrderNumber;
                        objDTO.OrderNumberTypeIncrementingbyDay = item.OrderNumberTypeIncrementingbyDay;
                        objDTO.OrderNumberTypeDateIncrementing = item.OrderNumberTypeDateIncrementing;
                        objDTO.OrderNumberTypeDate = item.OrderNumberTypeDate;

                        if (objDTO.OrderNumberTypeBlank)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeBlank;
                        else if (objDTO.OrderNumberTypeFixed)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeFixed;
                        else if (objDTO.OrderNumberTypeBlanketOrderNumber)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeBlanketOrderNumber;
                        else if (objDTO.OrderNumberTypeIncrementingOrderNumber)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeIncrementingOrderNumber;
                        else if (objDTO.OrderNumberTypeIncrementingbyDay)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeIncrementingbyDay;
                        else if (objDTO.OrderNumberTypeDateIncrementing)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeDateIncrementing;
                        else if (objDTO.OrderNumberTypeDate)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeDate;
                        else
                            objDTO.POAutoSequence = null;

                        objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);
                        objDTO.UDF6 = (item.UDF6 == null) ? null : (item.UDF6.Length > 255 ? item.UDF6.Substring(0, 255) : item.UDF6);
                        objDTO.UDF7 = (item.UDF7 == null) ? null : (item.UDF7.Length > 255 ? item.UDF7.Substring(0, 255) : item.UDF7);
                        objDTO.UDF8 = (item.UDF8 == null) ? null : (item.UDF8.Length > 255 ? item.UDF8.Substring(0, 255) : item.UDF8);
                        objDTO.UDF9 = (item.UDF9 == null) ? null : (item.UDF9.Length > 255 ? item.UDF9.Substring(0, 255) : item.UDF9);
                        objDTO.UDF10 = (item.UDF10 == null) ? null : (item.UDF10.Length > 255 ? item.UDF10.Substring(0, 255) : item.UDF10);

                        objDTO.AccountNumber = (item.AccountNumber == null) ? null : (item.AccountNumber.Length > 255 ? item.AccountNumber.Substring(0, 255) : item.AccountNumber);
                        objDTO.AccountName = (item.AccountName == null) ? null : (item.AccountName.Length > 255 ? item.AccountName.Substring(0, 255) : item.AccountName);
                        objDTO.AccountAddress = (item.AccountAddress == null) ? null : (item.AccountAddress.Length > 1027 ? item.AccountAddress.Substring(0, 1027) : item.AccountAddress);
                        objDTO.AccountCity = (item.AccountCity == null) ? null : (item.AccountCity.Length > 128 ? item.AccountCity.Substring(0, 128) : item.AccountCity);
                        objDTO.AccountState = (item.AccountState == null) ? null : (item.AccountState.Length > 256 ? item.AccountState.Substring(0, 256) : item.AccountState);
                        objDTO.AccountZip = (item.AccountZip == null) ? null : (item.AccountZip.Length > 20 ? item.AccountZip.Substring(0, 20) : item.AccountZip);
                        if (item.AccountIsDefault == true)
                            objDTO.AccountIsDefault = item.AccountIsDefault;

                        objDTO.BlanketPONumber = (item.BlanketPONumber == null) ? null : (item.BlanketPONumber.Length > 255 ? item.BlanketPONumber.Substring(0, 255) : item.BlanketPONumber);
                        if (item.IsBlanketDeleted == true)
                            objDTO.IsBlanketDeleted = item.IsBlanketDeleted;
                        objDTO.StartDate = item.StartDate;
                        objDTO.EndDate = item.EndDate;

                        objDTO.MaxLimit = item.MaxLimit;
                        objDTO.IsNotExceed = item.IsNotExceed;

                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        //Wi-1195 added PullPurchaseNumberType fields in import of supplier
                        objDTO.PullPurchaseNumberFixed = item.PullPurchaseNumberFixed;
                        objDTO.PullPurchaseNumberBlanketOrder = item.PullPurchaseNumberBlanketOrder;
                        objDTO.PullPurchaseNumberDateIncrementing = item.PullPurchaseNumberDateIncrementing;
                        objDTO.PullPurchaseNumberDate = item.PullPurchaseNumberDate;

                        if (objDTO.PullPurchaseNumberFixed)
                            objDTO.PullPurchaseNumberType = (int)CommonUtility.PullPurchaseNumberType.PullPurchaseNumberFixed;
                        else if (objDTO.PullPurchaseNumberBlanketOrder)
                            objDTO.PullPurchaseNumberType = (int)CommonUtility.PullPurchaseNumberType.PullPurchaseNumberBlanketOrder;
                        else if (objDTO.PullPurchaseNumberDateIncrementing)
                            objDTO.PullPurchaseNumberType = (int)CommonUtility.PullPurchaseNumberType.PullPurchaseNumberDateIncrementing;
                        else if (objDTO.PullPurchaseNumberDate)
                            objDTO.PullPurchaseNumberType = (int)CommonUtility.PullPurchaseNumberType.PullPurchaseNumberDate;
                        else
                            objDTO.PullPurchaseNumberType = null;

                        objDTO.LastPullPurchaseNumberUsed = item.LastPullPurchaseNumberUsed;

                        objDTO.SupplierImage = item.SupplierImage;
                        objDTO.ImageExternalURL = item.ImageExternalURL;
                        objDTO.ImageType = "ExternalImage";
                        if (string.IsNullOrWhiteSpace(item.ImageExternalURL) && (!string.IsNullOrEmpty(item.SupplierImage)))
                        {
                            objDTO.ImageType = "ImagePath";
                        }

                        lstSupplierGUID.Add(objDTO.GUID);

                        CurrentSupplierList.Add(objDTO);

                        CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportSupplierColumn.UDF1.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportSupplierColumn.UDF2.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportSupplierColumn.UDF3.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportSupplierColumn.UDF4.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportSupplierColumn.UDF5.ToString());

                    }
                }

                ImportMastersNewDTO.SupplierMasterImport objSupplierMasterImport;
                List<SupplierMasterMain> lstreturn1 = new List<SupplierMasterMain>();
                if (CurrentSupplierList.Count > 0)
                {
                    lstreturn1 = objImport.BulkInsertWithChiled(ImportMastersDTO.TableName.SupplierMaster.ToString(), CurrentSupplierList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList, isImgZipAvail);
                }
                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {

                    /* ------- FOR SET ID WITH IMAGEPATH ----------*/
                    if (isImgZipAvail)
                    {
                        List<SupplierMasterMain> distSupplierList = lstreturn1.GroupBy(l => l.SupplierName).Select(g => g.First()).ToList();

                        savedOnlyitemIds = string.Join(",", distSupplierList.Where(i => i.ID != 0 && (!string.IsNullOrEmpty(i.SupplierImage))).Select(p => p.ID.ToString() + "#" + p.SupplierImage.ToString()));

                        string SupplierNameList = string.Join("@", distSupplierList.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.SupplierImage)) && i.Status != null && i.Status.ToLower() == "success").Select(a => a.SupplierName));

                        string SupplierColorList = string.Join("@", distSupplierList.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.SupplierImage)) && i.Status != null && i.Status.ToLower() == "success").Select(a => a.SupplierColor));

                        List<SupplierMasterDTO> supplierList = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetRecordByNameAndColor(SupplierNameList, SupplierColorList, SessionHelper.RoomID, SessionHelper.CompanyID);

                        foreach (SupplierMasterDTO b in supplierList)
                        {
                            SupplierMasterMain objSupplierMaster = distSupplierList.Where(i => i.SupplierName == b.SupplierName && i.SupplierColor == b.SupplierColor && (!string.IsNullOrEmpty(i.SupplierImage))).FirstOrDefault();
                            if (objSupplierMaster != null)
                            {
                                if (string.IsNullOrEmpty(savedOnlyitemIds))
                                {
                                    savedOnlyitemIds = b.ID + "#" + objSupplierMaster.SupplierImage.ToString();
                                }
                                else
                                {
                                    savedOnlyitemIds += "," + b.ID + "#" + objSupplierMaster.SupplierImage.ToString();
                                }
                            }
                        }
                    }
                    /* ------- FOR SET ID WITH IMAGEPATH ----------*/

                    List<SupplierMasterMain> lst1 = new List<SupplierMasterMain>();
                    List<SupplierMasterMain> lst2 = (List<SupplierMasterMain>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();

                    foreach (SupplierMasterMain item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objSupplierMasterImport = LstSupplierMaster.Where(x => x.SupplierName.Trim().ToUpper() == item.SupplierName.Trim().ToUpper() && x.SupplierColor.Trim().ToUpper() == item.SupplierColor.ToUpper()).FirstOrDefault();
                        if (objSupplierMasterImport != null)
                        {
                            objSupplierMasterImport.Status = item.Status;
                            objSupplierMasterImport.Reason = item.Reason;
                            lstreturn.Add(objSupplierMasterImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankSupplierList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.SupplierMasterImport item in CurrentBlankSupplierList)
                    {
                        lstreturn.Add(item);
                    }
                }

                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }
            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Import QuickList Items]
        private bool ImportQuickListItems(List<ImportMastersNewDTO.QuickListItemsImport> LstQuickListItems, out List<ImportMastersNewDTO.QuickListItemsImport> lstreturn, out string reason, out string status)
        {
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.QuickListItemsImport>();
            List<ImportMastersNewDTO.QuickListItemsImport> CurrentBlankQuickListItemsList = new List<ImportMastersNewDTO.QuickListItemsImport>();
            List<QuickListItemsMain> CurrentQuickListItemsList = null;
            List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            List<Guid> lstQuickListItemsGUID = new List<Guid>();

            if (LstQuickListItems != null && LstQuickListItems.Count > 0)
            {
                CurrentQuickListItemsList = new List<QuickListItemsMain>();
                lst = objImport.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.QuickListItems.ToString(), UDFControlTypes.Textbox.ToString());
                CurrentOptionList = new List<UDFOptionsMain>();
                BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                foreach (ImportMastersNewDTO.QuickListItemsImport item in LstQuickListItems)
                {
                    QuickListItemsMain objDTO = new QuickListItemsMain();
                    QuickListType _QLType;
                    Guid? ItemGUID = Guid.Empty;
                    if (Enum.TryParse(item.Type, true, out _QLType))
                        objDTO.QLType = _QLType;
                    else
                        objDTO.QLType = null;

                    if (objDTO.QLType == null)
                    {
                        item.Status = "Fail";
                        if (!string.IsNullOrEmpty(item.Reason))
                        {
                            item.Reason += Environment.NewLine + "Quick list type is invalid.";
                        }
                        else
                            item.Reason = "Quick list type is invalid.";
                        objDTO.Status = "Fail";
                        objDTO.Reason = "Quick list type is invalid.";

                    }
                    if (objDTO.QLType != null)
                    {
                        bool isActive = true;
                        int itemType = 0;
                        if (!string.IsNullOrEmpty(item.ItemNumber))
                        {
                            ItemGUID = GetItemGUID_IsActive(ImportMastersDTO.TableName.ItemMaster, item.ItemNumber, out isActive, out itemType);
                            if (item.QuickListGUID.ToString().Trim() == "" || ItemGUID.HasValue && ItemGUID == Guid.Empty)
                            {
                                if (ItemGUID.HasValue && ItemGUID == Guid.Empty)
                                {
                                    item.Status = "Fail";
                                    if (!string.IsNullOrEmpty(item.Reason))
                                    {
                                        item.Reason += Environment.NewLine + "Item does not exist.";
                                    }
                                    else
                                        item.Reason = "Item does not exist.";
                                    objDTO.Status = "Fail";
                                    objDTO.Reason = "Item does not exist.";
                                }
                                else
                                {
                                    if (!isActive)
                                    {
                                        item.Status = "Fail";
                                        if (!string.IsNullOrEmpty(item.Reason))
                                        {
                                            item.Reason += Environment.NewLine + "Item does not active.";
                                        }
                                        else
                                            item.Reason = "Item does not active.";
                                        objDTO.Status = "Fail";
                                        objDTO.Reason = "Item does not active.";
                                    }
                                }
                            }
                            else
                            {
                                objDTO.QuickListGUID = item.QuickListname == "" ? Guid.NewGuid() : GetGUID(ImportMastersDTO.TableName.QuickListItems, item.QuickListname, item.Comments, (QuickListType)objDTO.QLType);
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(item.QuickListname) || string.IsNullOrEmpty(item.ItemNumber) || item.Status.Trim().ToLowerInvariant() == "fail")
                    {
                        CurrentBlankQuickListItemsList.Add(item);
                        continue;
                    }
                    else
                    {

                        objDTO.ItemGUID = ItemGUID.Value;
                        BinMasterDTO objBinMasterDTO = objItemLocationDetailsDAL.GetItemBin(ItemGUID ?? Guid.Empty, item.BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                        if (objBinMasterDTO != null)
                            objDTO.BinID = objBinMasterDTO.ID;
                        objDTO.Quantity = item.Quantity;
                        objDTO.ConsignedQuantity = item.ConsignedQuantity;
                        objDTO.ItemNumber = item.ItemNumber;
                        objDTO.QuickListname = item.QuickListname;
                        objDTO.Comments = item.Comments;
                        objDTO.UDF1 = item.UDF1;
                        objDTO.UDF2 = item.UDF2;
                        objDTO.UDF3 = item.UDF3;
                        objDTO.UDF4 = item.UDF4;
                        objDTO.UDF5 = item.UDF5;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.Type = item.Type;
                        objDTO.BinNumber = item.BinNumber;

                        lstQuickListItemsGUID.Add(objDTO.GUID);

                        var itemval = CurrentQuickListItemsList.FirstOrDefault(x => x.ItemGUID == objDTO.ItemGUID && x.QuickListGUID == objDTO.QuickListGUID && x.BinNumber == objDTO.BinNumber && x.BinID == objDTO.BinID);
                        if (itemval != null)
                            CurrentQuickListItemsList.Remove(itemval);
                        CurrentQuickListItemsList.Add(objDTO);

                        item.Status = "Success";
                        item.Reason = "N/A";
                    }
                }

                ImportMastersNewDTO.QuickListItemsImport objQuickListItemsImport;
                List<QuickListItemsMain> lstreturn1 = new List<QuickListItemsMain>();
                if (CurrentQuickListItemsList.Count > 0)
                {
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.QuickListItems.ToString(), CurrentQuickListItemsList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                }
                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {
                    List<QuickListItemsMain> lst1 = new List<QuickListItemsMain>();
                    List<QuickListItemsMain> lst2 = (List<QuickListItemsMain>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();


                    foreach (QuickListItemsMain item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objQuickListItemsImport = LstQuickListItems.Where(x => x.QuickListname.Trim().ToUpper() == item.QuickListname.Trim().ToUpper()).FirstOrDefault();
                        if (objQuickListItemsImport != null)
                        {
                            objQuickListItemsImport.Status = item.Status;
                            objQuickListItemsImport.Reason = item.Reason;
                            lstreturn.Add(objQuickListItemsImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankQuickListItemsList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.QuickListItemsImport item in CurrentBlankQuickListItemsList)
                    {
                        lstreturn.Add(item);
                    }
                }

                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }
            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Inventory Location Import (MANUAL COUNT)]
        private bool ImportInventoryLocationItems(List<ImportMastersNewDTO.InventoryLocationImport> LstInventoryLocation, out List<ImportMastersNewDTO.InventoryLocationImport> lstreturn, out string reason, out string status)
        {
            reason = "";
            status = "";
            XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ILQImportNew = Settinfile.Element("ILQImportNew").Value;
            lstreturn = new List<ImportMastersNewDTO.InventoryLocationImport>();
            List<ImportMastersNewDTO.InventoryLocationImport> CurrentBlankInventoryLocationList = new List<ImportMastersNewDTO.InventoryLocationImport>();
            List<InventoryLocationMain> CurrentInventoryLocationList = null;
            List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);

            if (LstInventoryLocation != null && LstInventoryLocation.Count > 0)
            {
                if (ILQImportNew == "1")
                {

                    List<ItemLocationDetailsDTO> lstProperRecords = new List<ItemLocationDetailsDTO>();
                    List<ItemLocationDetailsDTO> lstNotProperRecords = new List<ItemLocationDetailsDTO>();
                    List<ItemLocationDetailsDTO> lstValidated = new List<ItemLocationDetailsDTO>();
                    ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    List<ImportMastersNewDTO.InventoryLocationImport> failRecord = LstInventoryLocation.Where(x => (x.Status ?? string.Empty).ToUpperInvariant() == "FAIL").ToList();
                    lstValidated = objItemLocationDetailsDAL.ValidateILQRecords_NewImport(LstInventoryLocation.Where(x => (x.Status ?? string.Empty).ToUpperInvariant() != "FAIL").ToList(), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    lstProperRecords = lstValidated.Where(t => (t.ErrorMessege ?? "") == "").ToList();
                    CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);

                    lstNotProperRecords = lstValidated.Where(t => (t.ErrorMessege ?? "") != "").ToList();
                    if (lstProperRecords != null && lstProperRecords.Count > 0)
                    {
                        DataTable dtItemLocations = GetTableFromList(lstProperRecords);
                        objItemLocationDetailsDAL.ApplyManualCountLineitem(dtItemLocations, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                        lstProperRecords.ForEach(t =>
                        {                            
                            //objCartItemDAL.AutoCartUpdateByCode(t.ItemGUID ?? Guid.Empty, SessionHelper.UserID, "Web", "ImportControler>> SaveImport");
                            objCartItemDAL.AutoCartUpdateByCode(t.ItemGUID ?? Guid.Empty, SessionHelper.UserID, "Web", "BulkImport >> Import ItemLocation");
                        });
                    }
                    lstreturn = new List<ImportMastersNewDTO.InventoryLocationImport>();
                    foreach (ItemLocationDetailsDTO t in lstNotProperRecords)
                    {
                        ImportMastersNewDTO.InventoryLocationImport objInventoryLocationMain = new ImportMastersNewDTO.InventoryLocationImport();
                        objInventoryLocationMain.BinNumber = t.BinNumber;
                        objInventoryLocationMain.ItemNumber = t.ItemNumber;
                        objInventoryLocationMain.customerownedquantity = t.CustomerOwnedQuantity;
                        objInventoryLocationMain.consignedquantity = t.ConsignedQuantity;
                        objInventoryLocationMain.Cost = t.Cost;
                        objInventoryLocationMain.Expiration = t.Expiration;
                        objInventoryLocationMain.SerialNumber = t.SerialNumber;
                        objInventoryLocationMain.LotNumber = t.LotNumber;
                        objInventoryLocationMain.Received = t.Received;
                        objInventoryLocationMain.Status = "fail";
                        objInventoryLocationMain.Reason = t.ErrorMessege;
                        objInventoryLocationMain.ProjectSpend = t.ProjectSpend;
                        lstreturn.Add(objInventoryLocationMain);
                    }
                    foreach (ImportMastersNewDTO.InventoryLocationImport t in failRecord)
                    {
                        ImportMastersNewDTO.InventoryLocationImport objInventoryLocationMain = new ImportMastersNewDTO.InventoryLocationImport();
                        objInventoryLocationMain.BinNumber = t.BinNumber;
                        objInventoryLocationMain.ItemNumber = t.ItemNumber;
                        objInventoryLocationMain.customerownedquantity = t.customerownedquantity;
                        objInventoryLocationMain.consignedquantity = t.consignedquantity;
                        objInventoryLocationMain.Cost = t.Cost;
                        objInventoryLocationMain.Expiration = t.Expiration;
                        objInventoryLocationMain.SerialNumber = t.SerialNumber;
                        objInventoryLocationMain.LotNumber = t.LotNumber;
                        objInventoryLocationMain.Received = t.Received;
                        objInventoryLocationMain.Status = t.Status;
                        objInventoryLocationMain.Reason = t.Reason;
                        objInventoryLocationMain.ProjectSpend = t.ProjectSpend;
                        lstreturn.Add(objInventoryLocationMain);
                    }



                    if (lstreturn.Count == 0)
                    {
                        status = "success";
                        reason = "";
                        return true;
                    }
                    else
                    {
                        status = "Fail";
                        reason = lstreturn.Count.ToString() + " records not imported successfully";
                        return false;
                    }
                }
                else
                {


                    status = "Fail";
                    reason = "No records found to import";
                    return false;
                }

            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Adjustment Count Import (BIN MASTER)]
        private bool ImportAdjustmentCountBMItems(List<ImportMastersNewDTO.InventoryLocationImport> LstInventoryLocation, out List<ImportMastersNewDTO.InventoryLocationImport> lstreturn, out string reason, out string status)
        {
            reason = "";
            status = "";
            XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ILQImportNew = Settinfile.Element("ILQImportNew").Value;
            lstreturn = new List<ImportMastersNewDTO.InventoryLocationImport>();
            List<ImportMastersNewDTO.InventoryLocationImport> CurrentBlankInventoryLocationList = new List<ImportMastersNewDTO.InventoryLocationImport>();
            List<InventoryLocationMain> CurrentInventoryLocationList = null;
            List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);


            if (LstInventoryLocation != null && LstInventoryLocation.Count > 0)
            {
                if (ILQImportNew == "1")
                {

                    List<ItemLocationDetailsDTO> lstProperRecords = new List<ItemLocationDetailsDTO>();
                    List<ItemLocationDetailsDTO> lstNotProperRecords = new List<ItemLocationDetailsDTO>();
                    List<ItemLocationDetailsDTO> lstValidated = new List<ItemLocationDetailsDTO>();
                    ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    List<ImportMastersNewDTO.InventoryLocationImport> failRecord = LstInventoryLocation.Where(x => (x.Status ?? string.Empty).ToUpperInvariant() == "FAIL").ToList();
                    lstValidated = objItemLocationDetailsDAL.ValidateILQRecords_NewImport(LstInventoryLocation.Where(x => (x.Status ?? string.Empty).ToUpperInvariant() != "FAIL").ToList(), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    lstProperRecords = lstValidated.Where(t => (t.ErrorMessege ?? "") == "").ToList();
                    CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);

                    lstNotProperRecords = lstValidated.Where(t => (t.ErrorMessege ?? "") != "").ToList();
                    if (lstProperRecords != null && lstProperRecords.Count > 0)
                    {
                        DataTable dtItemLocations = GetTableFromList(lstProperRecords);
                        objItemLocationDetailsDAL.ApplyCountLineitem(dtItemLocations, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                        lstProperRecords.ForEach(t =>
                        {
                            //objCartItemDAL.AutoCartUpdateByCode(t.ItemGUID ?? Guid.Empty, SessionHelper.UserID, "Web", "ImportControler>> SaveImport");
                            objCartItemDAL.AutoCartUpdateByCode(t.ItemGUID ?? Guid.Empty, SessionHelper.UserID, "Web", "BulkImport >> Import Adjustment Count");
                        });
                    }
                    lstreturn = new List<ImportMastersNewDTO.InventoryLocationImport>();
                    foreach (ItemLocationDetailsDTO t in lstNotProperRecords)
                    {
                        ImportMastersNewDTO.InventoryLocationImport objInventoryLocationMain = new ImportMastersNewDTO.InventoryLocationImport();
                        objInventoryLocationMain.BinNumber = t.BinNumber;
                        objInventoryLocationMain.ItemNumber = t.ItemNumber;
                        objInventoryLocationMain.customerownedquantity = t.CustomerOwnedQuantity;
                        objInventoryLocationMain.consignedquantity = t.ConsignedQuantity;
                        objInventoryLocationMain.Cost = t.Cost;
                        objInventoryLocationMain.Expiration = t.Expiration;
                        objInventoryLocationMain.SerialNumber = t.SerialNumber;
                        objInventoryLocationMain.LotNumber = t.LotNumber;
                        objInventoryLocationMain.Received = t.Received;
                        objInventoryLocationMain.Status = "fail";
                        objInventoryLocationMain.Reason = t.ErrorMessege;
                        objInventoryLocationMain.ProjectSpend = t.ProjectSpend;
                        lstreturn.Add(objInventoryLocationMain);
                    }
                    foreach (ImportMastersNewDTO.InventoryLocationImport t in failRecord)
                    {
                        ImportMastersNewDTO.InventoryLocationImport objInventoryLocationMain = new ImportMastersNewDTO.InventoryLocationImport();
                        objInventoryLocationMain.BinNumber = t.BinNumber;
                        objInventoryLocationMain.ItemNumber = t.ItemNumber;
                        objInventoryLocationMain.customerownedquantity = t.customerownedquantity;
                        objInventoryLocationMain.consignedquantity = t.consignedquantity;
                        objInventoryLocationMain.Cost = t.Cost;
                        objInventoryLocationMain.Expiration = t.Expiration;
                        objInventoryLocationMain.SerialNumber = t.SerialNumber;
                        objInventoryLocationMain.LotNumber = t.LotNumber;
                        objInventoryLocationMain.Received = t.Received;
                        objInventoryLocationMain.Status = t.Status;
                        objInventoryLocationMain.Reason = t.Reason;
                        objInventoryLocationMain.ProjectSpend = t.ProjectSpend;
                        lstreturn.Add(objInventoryLocationMain);
                    }



                    if (lstreturn.Count == 0)
                    {
                        status = "success";
                        reason = "";
                        return true;
                    }
                    else
                    {
                        status = "Fail";
                        reason = lstreturn.Count.ToString() + " records not imported successfully";
                        return false;
                    }
                }
                else
                {


                    status = "Fail";
                    reason = "No records found to import";
                    return false;
                }

            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Import WorkOrder]
        private bool ImportWorkOrder(List<ImportMastersNewDTO.WorkOrderImport> LstWorkOrder, out List<ImportMastersNewDTO.WorkOrderImport> lstreturn, out string reason, out string status)
        {
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.WorkOrderImport>();
            List<ImportMastersNewDTO.WorkOrderImport> CurrentBlankWorkOrderList = new List<ImportMastersNewDTO.WorkOrderImport>();
            List<WorkOrderMain> CurrentWorkOrderList = null;
            List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            List<Guid> lstWorkOrderGUID = new List<Guid>();

            if (LstWorkOrder != null && LstWorkOrder.Count > 0)
            {
                List<TechnicianMasterDTO> objTechnicialMasterDALList = new List<TechnicianMasterDTO>();
                CurrentWorkOrderList = new List<WorkOrderMain>();
                lst = objImport.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.WorkOrder.ToString(), UDFControlTypes.Textbox.ToString());
                CurrentOptionList = new List<UDFOptionsMain>();

                TechnicialMasterDAL objTechnicialMasterDAL = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
                objTechnicialMasterDALList = objTechnicialMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                foreach (ImportMastersNewDTO.WorkOrderImport woList in LstWorkOrder.GroupBy(l => l.Technician).Select(g => g.First()).ToList())
                {
                    if (woList.Technician.ToLower().Trim() != string.Empty)
                    {
                        if ((from p in objTechnicialMasterDALList
                             where (p.TechnicianCode.ToLower().Trim() == (woList.Technician.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                             select p).Any())
                        {

                        }
                        else
                        {
                            TechnicianMasterDTO objTechnicianMasterDTO = new TechnicianMasterDTO();
                            objTechnicianMasterDTO.TechnicianCode = woList.Technician;
                            objTechnicianMasterDTO.Room = SessionHelper.RoomID;
                            objTechnicianMasterDTO.CompanyID = SessionHelper.CompanyID;
                            objTechnicianMasterDTO.CreatedBy = SessionHelper.UserID;
                            objTechnicianMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                            objTechnicianMasterDTO.Created = DateTimeUtility.DateTimeNow;
                            objTechnicianMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                            objTechnicianMasterDTO.GUID = Guid.NewGuid();
                            objTechnicianMasterDTO.IsArchived = false;
                            objTechnicianMasterDTO.IsDeleted = false;
                            Int64 TechnicanID = objTechnicialMasterDAL.Insert(objTechnicianMasterDTO);
                            objTechnicianMasterDTO = objTechnicialMasterDAL.GetRecord(TechnicanID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                            objTechnicialMasterDALList.Add(objTechnicianMasterDTO);

                        }
                    }
                }

                foreach (ImportMastersNewDTO.WorkOrderImport item in LstWorkOrder)
                {

                    if (string.IsNullOrEmpty(item.WOName) || item.Status.Trim().ToLowerInvariant() == "fail")
                    {
                        CurrentBlankWorkOrderList.Add(item);
                        continue;
                    }
                    else
                    {

                        WorkOrderMain objDTO = new WorkOrderMain();

                        objDTO.ID = item.ID;
                        objDTO.WOName = item.WOName;
                        objDTO.ReleaseNumber = item.ReleaseNumber;
                        objDTO.Description = (item.Description == null) ? string.Empty : (item.Description.Length > 1024 ? item.Description.Substring(0, 1024) : item.Description);
                        objDTO.SupplierId = GetIDs(ImportMastersDTO.TableName.SupplierMaster, item.SupplierName);
                        if (!string.IsNullOrWhiteSpace(item.Technician))
                        {
                            if ((from p in objTechnicialMasterDALList
                                 where (p.TechnicianCode.ToLower().Trim() == (item.Technician.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                 select p).Any())
                            {

                                TechnicianMasterDTO objTechnician = (from p in objTechnicialMasterDALList
                                                                     where (p.TechnicianCode.ToLower().Trim() == (item.Technician.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                                                     select p).FirstOrDefault();
                                if (objTechnician != null)
                                {
                                    objDTO.TechnicianID = objTechnician.ID;
                                    objDTO.Technician = item.Technician;
                                }

                            }

                        }
                        if (!string.IsNullOrWhiteSpace(item.Customer))
                        {
                            CustomerMasterDTO objCustomer = new CustomerMasterDTO();
                            objCustomer = GetCustomerMaster(ImportMastersDTO.TableName.CustomerMaster, item.Customer);
                            if (objCustomer != null)
                            {
                                objDTO.CustomerID = objCustomer.ID;
                                objDTO.CustomerGUID = objCustomer.GUID;
                            }
                        }
                        if (item.WOStatus != null && (item.WOStatus.ToLower() == "open" || item.WOStatus.ToLower() == "close"))
                        {
                            if (item.WOStatus.ToLower() == "open")
                            {
                                objDTO.WOStatus = "Open";
                            }
                            else if (item.WOStatus.ToLower() == "close")
                            {
                                objDTO.WOStatus = "Close";
                            }
                            else
                            {
                                objDTO.WOStatus = "Open";
                            }
                        }
                        else
                        {
                            objDTO.WOStatus = "Open";

                        }
                        if (item.WOType != null && (item.WOType.ToLower() == "workorder"))
                        {
                            objDTO.WOType = item.WOType;
                        }
                        else
                        {
                            objDTO.WOType = "WorkOrder";

                        }
                        objDTO.Odometer_OperationHours = 0;
                        objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.WhatWhereAction = "Import Workorder";
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.IsSignatureCapture = false;
                        objDTO.IsSignatureRequired = false;


                        lstWorkOrderGUID.Add(objDTO.GUID);

                        WorkOrderMain itemval = null;
                        if (String.IsNullOrEmpty(item.ReleaseNumber) || String.IsNullOrWhiteSpace(item.ReleaseNumber))
                        {
                            itemval = CurrentWorkOrderList.FirstOrDefault(x => x.WOName == item.WOName && (String.IsNullOrEmpty(x.ReleaseNumber) || String.IsNullOrWhiteSpace(x.ReleaseNumber)));
                        }
                        else
                        {
                            itemval = CurrentWorkOrderList.FirstOrDefault(x => x.WOName == item.WOName && x.ReleaseNumber == item.ReleaseNumber);
                        }
                        if (itemval != null)
                            CurrentWorkOrderList.Remove(itemval);
                        CurrentWorkOrderList.Add(objDTO);

                        CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.WorkOrderColumn.UDF1.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.WorkOrderColumn.UDF2.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.WorkOrderColumn.UDF3.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.WorkOrderColumn.UDF4.ToString());
                        CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.WorkOrderColumn.UDF5.ToString());


                        item.Status = "Success";
                        item.Reason = "N/A";



                    }
                }

                ImportMastersNewDTO.WorkOrderImport objWorkOrderImport;
                List<WorkOrderMain> lstreturn1 = new List<WorkOrderMain>();
                if (CurrentWorkOrderList.Count > 0)
                {
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.WorkOrder.ToString(), CurrentWorkOrderList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                }
                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {
                    List<WorkOrderMain> lst1 = new List<WorkOrderMain>();
                    List<WorkOrderMain> lst2 = (List<WorkOrderMain>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();


                    foreach (WorkOrderMain item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objWorkOrderImport = LstWorkOrder.Where(x => x.WOName.Trim().ToUpper() == item.WOName.Trim().ToUpper()).FirstOrDefault();
                        if (objWorkOrderImport != null)
                        {
                            objWorkOrderImport.Status = item.Status;
                            objWorkOrderImport.Reason = item.Reason;
                            lstreturn.Add(objWorkOrderImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankWorkOrderList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.WorkOrderImport item in CurrentBlankWorkOrderList)
                    {
                        lstreturn.Add(item);
                    }
                }

                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }
            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Import BOM Item]
        private bool ImportBOMItem(List<ImportMastersNewDTO.BOMItemMasterImport> LstItemMaster, out List<ImportMastersNewDTO.BOMItemMasterImport> lstreturn, out string reason, out string status)
        {
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.BOMItemMasterImport>();
            List<ImportMastersNewDTO.BOMItemMasterImport> CurrentBlankItemList = new List<ImportMastersNewDTO.BOMItemMasterImport>();
            List<BOMItemMasterMain> CurrentItemList = null;
            List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            List<Guid> lstItemGUID = new List<Guid>();

            if (LstItemMaster != null && LstItemMaster.Count > 0)
            {
                CurrentItemList = new List<BOMItemMasterMain>();
                //lst = objImport.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.BOMItemMaster.ToString(), UDFControlTypes.Textbox.ToString());
                CurrentOptionList = new List<UDFOptionsMain>();


                foreach (ImportMastersNewDTO.BOMItemMasterImport item in LstItemMaster)
                {
                    if (string.IsNullOrEmpty(item.SupplierPartNo) && ((!string.IsNullOrWhiteSpace(item.ItemTypeName)) && item.ItemTypeName != "Labor"))
                    {
                        item.Status = "Fail";
                        if (!string.IsNullOrEmpty(item.Reason))
                        {
                            item.Reason += Environment.NewLine + string.Format(ResMessage.Required, "SupplierPartNo");
                        }
                        else
                            item.Reason = string.Format(ResMessage.Required, "SupplierPartNo");
                    }

                    if (item.Status.Trim().ToLowerInvariant() == "fail" || string.IsNullOrEmpty(item.ItemNumber))
                    {
                        CurrentBlankItemList.Add(item);
                        continue;
                    }
                    else
                    {
                        BOMItemMasterMain objDTO = new BOMItemMasterMain();

                        objDTO.ID = item.ID;
                        objDTO.ItemNumber = (item.ItemNumber == null) ? null : (item.ItemNumber.Length > 255 ? item.ItemNumber.Substring(0, 255) : item.ItemNumber);
                        objDTO.ManufacturerName = item.ManufacturerName;
                        //Wi-1505
                        //if (!string.IsNullOrWhiteSpace(item.ManufacturerName))
                        //{
                        //    objDTO.ManufacturerID = item.ManufacturerName == "" ? (long?)null : GetBOMIDs(ImportMastersDTO.TableName.ManufacturerMaster, item.ManufacturerName);
                        //}
                        //else
                        //{
                        //    objDTO.ManufacturerID = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName).GetORInsertBlankManuFacID(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                        //}

                        if (!string.IsNullOrWhiteSpace(item.ManufacturerName))
                        {
                            objDTO.ManufacturerID = item.ManufacturerName == "" ? (long?)null : GetBOMIDs(ImportMastersDTO.TableName.ManufacturerMaster, item.ManufacturerName);
                        }
                        else
                        {
                            long ManuID = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName).GetORInsertBlankManuFacID(0, SessionHelper.CompanyID, SessionHelper.UserID);
                            if (!string.IsNullOrWhiteSpace(item.ManufacturerNumber))
                            {
                                objDTO.ManufacturerID = ManuID;
                            }

                        }

                        objDTO.ManufacturerNumber = (item.ManufacturerNumber == null) ? null : (item.ManufacturerNumber.Length > 50 ? item.ManufacturerNumber.Substring(0, 50) : item.ManufacturerNumber);
                        objDTO.IsActive = item.IsActive;
                        if (item.ItemTypeName == "Labor")
                        {
                            objDTO.SupplierName = string.Empty;
                            objDTO.SupplierPartNo = null;
                            objDTO.SupplierID = null;
                        }
                        else
                        {
                            objDTO.SupplierName = item.SupplierName;
                            objDTO.SupplierPartNo = (item.SupplierPartNo == null) ? null : (item.SupplierPartNo.Length > 50 ? item.SupplierPartNo.Substring(0, 50) : item.SupplierPartNo);
                            objDTO.SupplierID = GetBOMIDs(ImportMastersDTO.TableName.SupplierMaster, item.SupplierName);

                        }

                        objDTO.UPC = item.UPC;
                        objDTO.UNSPSC = item.UNSPSC;
                        objDTO.Description = item.Description;
                        objDTO.LongDescription = item.LongDescription;
                        objDTO.CategoryID = item.CategoryName == "" ? (long?)null : GetBOMIDs(ImportMastersDTO.TableName.CategoryMaster, item.CategoryName);
                        objDTO.GLAccountID = item.GLAccount == "" ? (long?)null : GetBOMIDs(ImportMastersDTO.TableName.GLAccountMaster, item.GLAccount);
                        objDTO.CategoryName = item.CategoryName;
                        objDTO.GLAccount = item.GLAccount;
                        objDTO.UOMID = GetBOMIDs(ImportMastersDTO.TableName.UnitMaster, item.Unit);
                        objDTO.Unit = item.Unit;

                        objDTO.LeadTimeInDays = item.LeadTimeInDays;
                        objDTO.Taxable = item.Taxable;
                        objDTO.Consignment = item.Consignment;


                        objDTO.ItemUniqueNumber = objCommonDAL.GetItemUniqueIdByRoom(item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));

                        objDTO.IsTransfer = item.IsTransfer;
                        objDTO.IsPurchase = item.IsPurchase;
                        objDTO.DefaultLocation = 0;// GetIDs(ImportMastersDTO.TableName.BinMaster, item.InventryLocation);
                        objDTO.InventryLocation = item.InventryLocation;
                        objDTO.InventoryClassification = Convert.ToInt32(GetBOMIDs(ImportMastersDTO.TableName.InventoryClassificationMaster, item.InventoryClassificationName));
                        objDTO.InventoryClassificationName = item.InventoryClassificationName;

                        objDTO.ItemTypeName = item.ItemTypeName;
                        objDTO.SerialNumberTracking = item.SerialNumberTracking;
                        objDTO.LotNumberTracking = item.LotNumberTracking;
                        objDTO.DateCodeTracking = item.DateCodeTracking;
                        objDTO.ItemType = item.ItemTypeName == "Item" ? 1 : item.ItemTypeName == "Quick List" ? 2 : item.ItemTypeName == "Kit" ? 3 : item.ItemTypeName == "Labor" ? 4 : 1;
                        objDTO.ImagePath = item.ImagePath;

                        //objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        //objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        //objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        //objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        //objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);

                        objDTO.DefaultPullQuantity = 0;
                        objDTO.DefaultReorderQuantity = 0;
                        objDTO.CriticalQuantity = 0;
                        objDTO.MinimumQuantity = 0;
                        objDTO.MaximumQuantity = 0;
                        objDTO.Trend = false;
                        objDTO.CategoryColor = "";
                        objDTO.IsLotSerialExpiryCost = "";
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        //objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.IsBOMItem = true;
                        objDTO.WhatWhereAction = "Import";
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        // objDTO.LastCost = 0.0;
                        lstItemGUID.Add(objDTO.GUID);

                        var itemval = CurrentItemList.FirstOrDefault(x => x.ItemNumber == item.ItemNumber);
                        if (itemval != null)
                            CurrentItemList.Remove(itemval);
                        CurrentItemList.Add(objDTO);
                        //CurrentItemDTOList.Add(objDTO);

                        item.Status = "Success";
                        item.Reason = "N/A";

                        //CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportItemColumn.UDF1.ToString());
                        //CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportItemColumn.UDF2.ToString());
                        //CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportItemColumn.UDF3.ToString());
                        //CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportItemColumn.UDF4.ToString());
                        //CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportItemColumn.UDF5.ToString());

                    }


                }

                ImportMastersNewDTO.BOMItemMasterImport objItemMasterImport;
                List<BOMItemMasterMain> lstreturn1 = new List<BOMItemMasterMain>();
                if (CurrentItemList.Count > 0)
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.BOMItemMaster.ToString(), CurrentItemList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);

                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {


                    List<BOMItemMasterMain> lst1 = new List<BOMItemMasterMain>();
                    List<BOMItemMasterMain> lst2 = (List<BOMItemMasterMain>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();

                    foreach (BOMItemMasterMain item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objItemMasterImport = LstItemMaster.Where(x => x.ItemNumber.Trim().ToUpper() == item.ItemNumber.Trim().ToUpper()).FirstOrDefault();
                        if (objItemMasterImport != null)
                        {
                            objItemMasterImport.Status = item.Status;
                            objItemMasterImport.Reason = item.Reason;
                            lstreturn.Add(objItemMasterImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankItemList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.BOMItemMasterImport item in CurrentBlankItemList)
                    {
                        lstreturn.Add(item);
                    }
                }

                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }

            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Import KitDetail]
        private bool ImportKitDetail(List<ImportMastersNewDTO.KitDetailImport> LstKitDetail, out List<ImportMastersNewDTO.KitDetailImport> lstreturn, out string reason, out string status)
        {
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.KitDetailImport>();
            List<ImportMastersNewDTO.KitDetailImport> CurrentBlankKitDetailList = new List<ImportMastersNewDTO.KitDetailImport>();
            List<KitDetailmain> CurrentKitDetailList = null;
            //List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            List<Guid> lstItemGUID = new List<Guid>();

            if (LstKitDetail != null && LstKitDetail.Count > 0)
            {
                CurrentKitDetailList = new List<KitDetailmain>();
                //lst = objImport.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.ItemMaster.ToString(), UDFControlTypes.Textbox.ToString());
                //CurrentOptionList = new List<UDFOptionsMain>();

                KitMasterDAL objKitMasterDAL = new KitMasterDAL(SessionHelper.EnterPriseDBName);


                foreach (ImportMastersNewDTO.KitDetailImport item in LstKitDetail)
                {

                    if (item.Status.Trim().ToLowerInvariant() == "fail")
                    {
                        CurrentBlankKitDetailList.Add(item);
                        continue;
                    }
                    else
                    {

                        KitDetailmain objDTO = new KitDetailmain();
                        KitMasterDTO objKitMasterDTO = new KitMasterDTO();
                        bool isActive = true;
                        int itemType = 0;
                        Guid? ItemGUID = GetItemGUID_IsActive(ImportMastersDTO.TableName.ItemMaster, item.ItemNumber, out isActive, out itemType);
                        Guid? KitItemGUID = GetGUID(ImportMastersDTO.TableName.ItemMaster, item.KitPartNumber);
                        objKitMasterDTO = objKitMasterDAL.GetRecord(KitItemGUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, 0, "Import");
                        if (objKitMasterDTO != null)//&& objKitMasterDTO.AvailableKitQuantity == 0
                        {
                            if (ItemGUID.HasValue && ItemGUID != Guid.Empty && KitItemGUID.HasValue && KitItemGUID != Guid.Empty)
                            {
                                if (isActive)
                                {
                                    if (itemType != 3)
                                    {
                                        objDTO.ItemGUID = ItemGUID.Value;
                                        objDTO.KitGUID = KitItemGUID.Value;
                                        objDTO.QuantityPerKit = item.QuantityPerKit;
                                        objDTO.ItemNumber = item.ItemNumber;
                                        objDTO.KitPartNumber = item.KitPartNumber;
                                        objDTO.IsActive = item.IsActive;

                                        objDTO.IsDeleted = item.IsDeleted;
                                        objDTO.IsBuildBreak = item.IsBuildBreak;
                                        objDTO.IsArchived = false;
                                        objDTO.Created = DateTimeUtility.DateTimeNow;
                                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                                        objDTO.Room = SessionHelper.RoomID;
                                        objDTO.CompanyID = SessionHelper.CompanyID;
                                        objDTO.CreatedBy = SessionHelper.UserID;
                                        objDTO.GUID = Guid.NewGuid();
                                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objDTO.AddedFrom = "Web";
                                        objDTO.EditedFrom = "Web";


                                        var itemval = CurrentKitDetailList.FirstOrDefault(x => x.ItemGUID == objDTO.ItemGUID && x.KitGUID == objDTO.KitGUID);
                                        if (itemval != null)
                                            CurrentKitDetailList.Remove(itemval);
                                        CurrentKitDetailList.Add(objDTO);

                                        item.Status = "Success";
                                        item.Reason = "N/A";
                                    }
                                    else
                                    {
                                        item.Status = "Fail";
                                        if (!string.IsNullOrEmpty(item.Reason))
                                        {
                                            item.Reason += Environment.NewLine + "Kit type item can not import.";
                                        }
                                        else
                                        {
                                            item.Reason = "Kit type item can not import";
                                        }
                                        CurrentBlankKitDetailList.Add(item);
                                    }
                                }
                                else
                                {
                                    //objDTO = item;
                                    //objDTO.Status = "Fail";
                                    //objDTO.Reason = "Item does not active.";
                                    item.Status = "Fail";
                                    if (!string.IsNullOrEmpty(item.Reason))
                                    {
                                        item.Reason += Environment.NewLine + "Item does not active.";
                                    }
                                    else
                                    {
                                        item.Reason = "Item does not active.";
                                    }
                                    CurrentBlankKitDetailList.Add(item);
                                }
                            }
                            else
                            {
                                //objDTO = item;
                                if (ItemGUID.HasValue && ItemGUID == Guid.Empty)
                                {
                                    //objDTO.Status = "Fail";
                                    //objDTO.Reason = "Item does not exist.";
                                    item.Status = "Fail";
                                    if (!string.IsNullOrEmpty(item.Reason))
                                    {
                                        item.Reason += Environment.NewLine + "Item does not exist.";
                                    }
                                    else
                                    {
                                        item.Reason = "Item does not exist.";
                                    }
                                }
                                CurrentBlankKitDetailList.Add(item);
                            }
                        }
                        else
                        {
                            //objDTO = item;
                            //objDTO.Status = "Fail";
                            if (objKitMasterDTO == null)
                            {
                                //if (ItemGUID.HasValue && ItemGUID != Guid.Empty && KitItemGUID.HasValue && KitItemGUID != Guid.Empty)
                                if (ItemGUID.HasValue && ItemGUID != Guid.Empty)
                                {
                                    if (isActive)
                                    {
                                        if (itemType != 3)
                                        {

                                            /*///////SAVE KIT ITEM IN ITEM MASTER/////////*/
                                            //insert Item Master
                                            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                                            objItemMasterDTO.ID = item.ID;
                                            if (item.LotNumberTracking == false)
                                            {
                                                objItemMasterDTO.SerialNumberTracking = item.SerialNumberTracking;
                                            }
                                            else
                                            {
                                                objItemMasterDTO.SerialNumberTracking = false;
                                            }
                                            objItemMasterDTO.LotNumberTracking = item.LotNumberTracking;
                                            objItemMasterDTO.DateCodeTracking = item.DateCodeTracking;
                                            objItemMasterDTO.IsActive = item.IsActive;
                                            objItemMasterDTO.ItemNumber = (item.KitPartNumber == null) ? null : (item.KitPartNumber.Length > 255 ? item.KitPartNumber.Substring(0, 255) : item.KitPartNumber);
                                            objItemMasterDTO.SupplierName = item.SupplierName;
                                            objItemMasterDTO.SupplierPartNo = (item.SupplierPartNo == null) ? null : (item.SupplierPartNo.Length > 50 ? item.SupplierPartNo.Substring(0, 50) : item.SupplierPartNo);
                                            objItemMasterDTO.SupplierID = GetIDs(ImportMastersDTO.TableName.SupplierMaster, item.SupplierName);
                                            objItemMasterDTO.UOMID = GetIDs(ImportMastersDTO.TableName.UnitMaster, item.UOM);
                                            objItemMasterDTO.Unit = item.UOM;
                                            objItemMasterDTO.CostUOMID = item.CostUOMName == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.CostUOMMaster, item.CostUOMName);
                                            objItemMasterDTO.CostUOMName = item.CostUOMName;
                                            objItemMasterDTO.DefaultReorderQuantity = item.DefaultReorderQuantity;
                                            objItemMasterDTO.DefaultPullQuantity = item.DefaultPullQuantity;
                                            objItemMasterDTO.IsItemLevelMinMaxQtyRequired = item.IsItemLevelMinMaxQtyRequired;
                                            objItemMasterDTO.CriticalQuantity = item.CriticalQuantity;
                                            objItemMasterDTO.MinimumQuantity = item.MinimumQuantity;
                                            objItemMasterDTO.MaximumQuantity = item.MaximumQuantity;

                                            //objItemMasterDTO.DefaultLocation = GetIDs(ImportMastersDTO.TableName.BinMaster, item.DefaultLocationName);

                                            //on hand qty? 

                                            //objItemMasterDTO.ItemTypeName = item.ItemTypeName;
                                            //objItemMasterDTO.ItemType = item.ItemTypeName == "Item" ? 1 : item.ItemTypeName == "Quick List" ? 2 : item.ItemTypeName == "Kit" ? 3 : item.ItemTypeName == "Labor" ? 4 : 1;

                                            objItemMasterDTO.ItemTypeName = "Kit";
                                            objItemMasterDTO.ItemType = 3;
                                            objItemMasterDTO.IsBuildBreak = item.IsBuildBreak;
                                            objItemMasterDTO.IsDeleted = item.IsDeleted ?? false;
                                            objItemMasterDTO.IsArchived = false;
                                            objItemMasterDTO.Created = DateTimeUtility.DateTimeNow;
                                            objItemMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                                            objItemMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                            objItemMasterDTO.Room = SessionHelper.RoomID;
                                            objItemMasterDTO.CompanyID = SessionHelper.CompanyID;
                                            objItemMasterDTO.CreatedBy = SessionHelper.UserID;
                                            objItemMasterDTO.GUID = Guid.NewGuid();
                                            objItemMasterDTO.AddedFrom = "Web";
                                            objItemMasterDTO.EditedFrom = "Web";
                                            objItemMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objItemMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                            objItemMasterDTO.WhatWhereAction = "Import";
                                            objItemMasterDTO.IsPurchase = true;
                                            //objItemMasterDTO.Consignment = false;
                                            //objItemMasterDTO.SuggestedOrderQuantity = item.KitDemand;
                                            objItemMasterDTO.OnHandQuantity = item.AvailableKitQuantity;
                                            objItemMasterDTO.Description = item.Description;
                                            objItemMasterDTO.ImageType = "ExternalImage";
                                            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                                            objItemMasterDAL.Insert(objItemMasterDTO);
                                            //insert code objItemMasterDTO


                                            Guid? NewKitItemGUID = GetGUID(ImportMastersDTO.TableName.ItemMaster, objItemMasterDTO.ItemNumber);

                                            //Insert bin

                                            long BinId = GetIDs(ImportMastersDTO.TableName.BinMaster, item.DefaultLocationName);
                                            BinMasterDTO objBinMasterDTO = new BinMasterDTO();
                                            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                                            objBinMasterDTO.ID = 1;
                                            objBinMasterDTO.BinNumber = item.DefaultLocationName.Length > 128 ? item.DefaultLocationName.Substring(0, 128) : item.DefaultLocationName;
                                            objBinMasterDTO.IsDeleted = false;
                                            objBinMasterDTO.IsArchived = false;
                                            objBinMasterDTO.Created = DateTimeUtility.DateTimeNow;
                                            objBinMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                                            objBinMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                            objBinMasterDTO.Room = SessionHelper.RoomID;
                                            objBinMasterDTO.CompanyID = SessionHelper.CompanyID;
                                            objBinMasterDTO.CreatedBy = SessionHelper.UserID;
                                            objBinMasterDTO.GUID = Guid.NewGuid();
                                            objBinMasterDTO.ItemGUID = NewKitItemGUID;
                                            objBinMasterDTO.AddedFrom = "Web";
                                            objBinMasterDTO.EditedFrom = "Web";
                                            objBinMasterDTO.ParentBinId = BinId;
                                            objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                            long returnNewBinID = objBinMasterDAL.Insert(objBinMasterDTO);
                                            // insert bin complete
                                            //update default Bin
                                            objItemMasterDTO.DefaultLocation = returnNewBinID;

                                            objItemMasterDAL.Edit(objItemMasterDTO);
                                            //update default Bin complete

                                            //insert Item SupplierDetails
                                            ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                                            ItemSupplierDetailsDTO objItemSupplierDetailsDTO = new ItemSupplierDetailsDTO();
                                            objItemSupplierDetailsDTO.AddedFrom = "Web";
                                            objItemSupplierDetailsDTO.CompanyID = SessionHelper.CompanyID;
                                            objItemSupplierDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                                            objItemSupplierDetailsDTO.CreatedBy = SessionHelper.UserID;
                                            objItemSupplierDetailsDTO.EditedFrom = "Web";
                                            objItemSupplierDetailsDTO.GUID = Guid.NewGuid();
                                            objItemSupplierDetailsDTO.ID = 0;
                                            objItemSupplierDetailsDTO.IsArchived = false;
                                            objItemSupplierDetailsDTO.IsDefault = true;
                                            objItemSupplierDetailsDTO.IsDeleted = false;
                                            objItemSupplierDetailsDTO.ItemGUID = NewKitItemGUID;
                                            objItemSupplierDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                                            objItemSupplierDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objItemSupplierDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                            objItemSupplierDetailsDTO.Room = SessionHelper.RoomID;
                                            objItemSupplierDetailsDTO.SupplierID = objItemMasterDTO.SupplierID ?? 0;
                                            objItemSupplierDetailsDTO.SupplierName = objItemMasterDTO.SupplierName;
                                            objItemSupplierDetailsDTO.SupplierNumber = objItemMasterDTO.SupplierPartNo;
                                            objItemSupplierDetailsDTO.Updated = DateTimeUtility.DateTimeNow;

                                            objItemSupplierDetailsDAL.Insert(objItemSupplierDetailsDTO);

                                            KitItemGUID = GetGUID(ImportMastersDTO.TableName.ItemMaster, item.KitPartNumber);
                                            objKitMasterDTO = objKitMasterDAL.GetRecord(KitItemGUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, 0, "Import");
                                            // lstItemGUID.Add(objItemMasterDTO.GUID);


                                            /*///////SAVE KIT ITEM IN ITEM MASTER////////*/


                                            objDTO.ItemGUID = ItemGUID.Value;
                                            objDTO.KitGUID = objItemMasterDTO.GUID;
                                            objDTO.QuantityPerKit = item.QuantityPerKit;
                                            objDTO.ItemNumber = item.ItemNumber;
                                            objDTO.KitPartNumber = item.KitPartNumber;

                                            //objDTO.QuantityReadyForAssembly = item.AvailableItemsInWIP;

                                            objDTO.IsDeleted = item.IsDeleted;
                                            objDTO.IsBuildBreak = item.IsBuildBreak;
                                            objDTO.IsArchived = false;
                                            objDTO.Created = DateTimeUtility.DateTimeNow;
                                            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                                            objDTO.LastUpdatedBy = SessionHelper.UserID;
                                            objDTO.Room = SessionHelper.RoomID;
                                            objDTO.CompanyID = SessionHelper.CompanyID;
                                            objDTO.CreatedBy = SessionHelper.UserID;
                                            objDTO.GUID = Guid.NewGuid();
                                            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objDTO.AddedFrom = "Web";
                                            objDTO.EditedFrom = "Web";

                                            item.Status = "Success";
                                            item.Reason = "N/A";

                                            var itemval = CurrentKitDetailList.FirstOrDefault(x => x.ItemGUID == objDTO.ItemGUID && x.KitGUID == objDTO.KitGUID);
                                            if (itemval != null)
                                                CurrentKitDetailList.Remove(itemval);
                                            CurrentKitDetailList.Add(objDTO);
                                        }
                                        else
                                        {
                                            item.Status = "Fail";
                                            if (!string.IsNullOrEmpty(item.Reason))
                                            {
                                                item.Reason += Environment.NewLine + "Kit type item can not import.";
                                            }
                                            else
                                            {
                                                item.Reason = "Kit type item can not import";
                                            }
                                            CurrentBlankKitDetailList.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        item.Status = "Fail";
                                        if (!string.IsNullOrEmpty(item.Reason))
                                        {
                                            item.Reason += Environment.NewLine + "Item does not active.";
                                        }
                                        else
                                        {
                                            item.Reason = "Item does not active.";
                                        }
                                        CurrentBlankKitDetailList.Add(item);
                                    }
                                }
                                else
                                {
                                    //objDTO = item;
                                    if (ItemGUID.HasValue && ItemGUID == Guid.Empty)
                                    {
                                        //objDTO.Status = "Fail";
                                        //objDTO.Reason = "Item does not exist.";
                                        item.Status = "Fail";
                                        if (!string.IsNullOrEmpty(item.Reason))
                                        {
                                            item.Reason += Environment.NewLine + "Item does not exist.";
                                        }
                                        else
                                            item.Reason = "Item does not exist.";
                                    }
                                    CurrentBlankKitDetailList.Add(item);
                                }
                            }
                            else
                            {
                                //objDTO.Status = "Fail";
                                //objDTO.Reason = "Item Is Not Able To Edit.";
                                item.Status = "Fail";
                                if (!string.IsNullOrEmpty(item.Reason))
                                {
                                    item.Reason += Environment.NewLine + "Item Is Not Able To Edit.";
                                }
                                else
                                    item.Reason = "Item Is Not Able To Edit.";
                                CurrentBlankKitDetailList.Add(item);
                            }
                        }

                        //item.Status = "Success";
                        //item.Reason = "N/A";
                    }
                }

                ImportMastersNewDTO.KitDetailImport objKitDetailImport;
                List<KitDetailmain> lstreturn1 = new List<KitDetailmain>();
                if (CurrentKitDetailList.Count > 0)
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.kitdetail.ToString(), CurrentKitDetailList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);

                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {
                    List<KitDetailmain> lst1 = new List<KitDetailmain>();
                    List<KitDetailmain> lst2 = (List<KitDetailmain>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();

                    foreach (KitDetailmain item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objKitDetailImport = LstKitDetail.Where(x => x.ItemNumber.Trim().ToUpper() == item.ItemNumber.Trim().ToUpper() && x.KitPartNumber.Trim().ToUpper() == item.KitPartNumber.Trim().ToUpper()).FirstOrDefault();
                        if (objKitDetailImport != null)
                        {
                            objKitDetailImport.Status = item.Status;
                            objKitDetailImport.Reason = item.Reason;
                            lstreturn.Add(objKitDetailImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankKitDetailList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.KitDetailImport item in CurrentBlankKitDetailList)
                    {
                        lstreturn.Add(item);
                    }
                }


                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }

            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Import ProjectMaster]
        private bool ImportProjectMaster(List<ImportMastersNewDTO.ProjectMasterImport> LstProjectMaster, out List<ImportMastersNewDTO.ProjectMasterImport> lstreturn, out string reason, out string status)
        {
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.ProjectMasterImport>();
            List<ImportMastersNewDTO.ProjectMasterImport> CurrentBlankProjectMasterList = new List<ImportMastersNewDTO.ProjectMasterImport>();
            List<ProjectMasterMain> CurrentProjectMasterList = null;
            List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            List<Guid> lstProjectMasterGUID = new List<Guid>();

            if (LstProjectMaster != null && LstProjectMaster.Count > 0)
            {
                ProjectMasterMain objBarcodeDAL = new ProjectMasterMain();
                List<ProjectMasterDTO> objProjectSpendList = new List<ProjectMasterDTO>();
                CurrentProjectMasterList = new List<ProjectMasterMain>();
                lst = objImport.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.ProjectMaster.ToString(), UDFControlTypes.Textbox.ToString());
                CurrentOptionList = new List<UDFOptionsMain>();

                foreach (ImportMastersNewDTO.ProjectMasterImport item in LstProjectMaster)
                {
                    Guid? ItemGUID;

                    ItemMasterDAL objItemmasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                    ItemGUID = objItemmasterDAL.GetGuidByItemNumber(item.ItemNumber, SessionHelper.RoomID, SessionHelper.CompanyID);

                    ProjectMasterMain objDTO = new ProjectMasterMain();

                    if (ItemGUID == null || ItemGUID == Guid.Empty)
                    {
                        item.Status = "Fail";
                        item.Reason = "Item does not exists";
                        CurrentBlankProjectMasterList.Add(item);
                    }

                    if (string.IsNullOrEmpty(item.ItemNumber) || string.IsNullOrEmpty(item.ProjectSpendName) || item.DollarLimitAmount == null || item.Status.Trim().ToLowerInvariant() == "fail")
                    {
                        CurrentBlankProjectMasterList.Add(item);
                        continue;
                    }
                    else
                    {
                        objDTO.ProjectSpendName = item.ProjectSpendName;
                        objDTO.Description = item.Description;
                        objDTO.DollarLimitAmount = item.DollarLimitAmount;
                        objDTO.TrackAllUsageAgainstThis = item.TrackAllUsageAgainstThis;
                        objDTO.IsClosed = item.IsClosed;
                        objDTO.UDF1 = item.UDF1;
                        objDTO.UDF2 = item.UDF2;
                        objDTO.UDF3 = item.UDF3;
                        objDTO.UDF4 = item.UDF4;
                        objDTO.UDF5 = item.UDF5;
                        objDTO.ItemNumber = item.ItemNumber;
                        objDTO.ItemDollarLimitAmount = item.ItemDollarLimitAmount;
                        objDTO.ItemQuantityLimitAmount = item.ItemQuantityLimitAmount;
                        objDTO.IsLineItemDeleted = item.IsLineItemDeleted;

                        objDTO.IsDeleted = item.IsDeleted ?? false;
                        objDTO.IsArchived = item.IsArchived ?? false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.WhatWhereAction = "Import";


                        var itemval = CurrentProjectMasterList.FirstOrDefault(x => x.ItemNumber == objDTO.ItemNumber && x.ProjectSpendName == objDTO.ProjectSpendName);
                        ProjectMasterMain objProjectMasterMain = CurrentProjectMasterList.Where(x => x.ProjectSpendName == item.ProjectSpendName).FirstOrDefault();

                        if (itemval != null)
                            CurrentProjectMasterList.Remove(itemval);
                        if (objProjectMasterMain != null)
                        {
                            objDTO.DollarLimitAmount = objProjectMasterMain.DollarLimitAmount;
                            objDTO.UDF1 = objProjectMasterMain.UDF1;
                            objDTO.UDF2 = objProjectMasterMain.UDF2;
                            objDTO.UDF3 = objProjectMasterMain.UDF3;
                            objDTO.UDF4 = objProjectMasterMain.UDF4;
                            objDTO.UDF5 = objProjectMasterMain.UDF5;
                            objDTO.TrackAllUsageAgainstThis = objProjectMasterMain.TrackAllUsageAgainstThis;
                            objDTO.IsClosed = objProjectMasterMain.IsClosed;
                            objDTO.IsDeleted = objProjectMasterMain.IsDeleted;
                        }
                        CurrentProjectMasterList.Add(objDTO);

                        item.Status = "Success";
                        item.Reason = "N/A";

                    }
                }

                ImportMastersNewDTO.ProjectMasterImport objProjectMasterImport;
                List<ProjectMasterMain> lstreturn1 = new List<ProjectMasterMain>();
                if (CurrentProjectMasterList.Count > 0)
                {
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.ProjectMaster.ToString(), CurrentProjectMasterList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                }
                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {
                    List<ProjectMasterMain> lst1 = new List<ProjectMasterMain>();
                    List<ProjectMasterMain> lst2 = (List<ProjectMasterMain>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();

                    /*//////////CODE FOR SAVE PROJECT LINE ITEMS/////////////////*/
                    ProjectMasterDAL objProjectMasterDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                    List<ProjectMasterDTO> objProjectMasterList = new List<ProjectMasterDTO>();
                    objProjectMasterList = objProjectMasterDAL.GetAllRecords(Convert.ToInt64(SessionHelper.RoomID), Convert.ToInt64(SessionHelper.CompanyID), false, false).ToList();

                    List<ProjectMasterMain> successList = new List<ProjectMasterMain>();
                    successList = lst1.Where(x => x.Status.ToUpper() == "SUCCESS").ToList();
                    foreach (ProjectMasterMain p in successList)
                    {
                        ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                        Guid? ItemGuid = new Guid();
                        Guid ProjectMasterGUID = new Guid();


                        ItemGuid = objItemMasterDAL.GetItemGuIDOnlyByItemNumber(p.ItemNumber, SessionHelper.RoomID);
                        ProjectMasterGUID = objProjectMasterList.Where(p1 => p1.ProjectSpendName == p.ProjectSpendName).FirstOrDefault().GUID;

                        ProjectSpendItemsDAL objProjectSpendItemsDAL = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName);
                        ProjectSpendItemsDTO objProjectSpendItem = objProjectSpendItemsDAL.GetCachedData(ProjectMasterGUID, SessionHelper.RoomID, SessionHelper.CompanyID, 0).Where(i => i.ItemGUID == ItemGuid).FirstOrDefault();

                        if (objProjectSpendItem == null)
                        {
                            objProjectSpendItem = new ProjectSpendItemsDTO();
                            objProjectSpendItem.GUID = new Guid();
                            objProjectSpendItem.ID = 0;
                            objProjectSpendItem.AddedFrom = "Web";
                            objProjectSpendItem.Created = DateTimeUtility.DateTimeNow;
                            objProjectSpendItem.CreatedBy = SessionHelper.UserID;
                            objProjectSpendItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        }

                        //ProjectSpendItemsDTO objProjectSpendItem = new ProjectSpendItemsDTO();

                        objProjectSpendItem.CompanyID = SessionHelper.CompanyID;

                        objProjectSpendItem.QuantityLimit = p.ItemQuantityLimitAmount;
                        objProjectSpendItem.DollarLimitAmount = p.ItemDollarLimitAmount;
                        objProjectSpendItem.EditedFrom = "Web";
                        objProjectSpendItem.IsArchived = p.IsArchived;
                        objProjectSpendItem.IsDeleted = p.IsLineItemDeleted;
                        objProjectSpendItem.IsArchived = p.IsLineItemDeleted;
                        objProjectSpendItem.LastUpdated = DateTimeUtility.DateTimeNow;
                        objProjectSpendItem.LastUpdatedBy = SessionHelper.UserID;
                        objProjectSpendItem.ProjectGUID = ProjectMasterGUID;
                        objProjectSpendItem.ReceivedOn = DateTimeUtility.DateTimeNow;

                        objProjectSpendItem.Room = SessionHelper.RoomID;
                        if (ItemGuid != null)
                            objProjectSpendItem.ItemGUID = ItemGuid;

                        //Extra Fields to set
                        objProjectSpendItem.QuantityUsed = 0;
                        objProjectSpendItem.DollarUsedAmount = 0;


                        if (objProjectSpendItem.ID == 0)
                        {
                            objProjectSpendItemsDAL.Insert(objProjectSpendItem);
                        }
                        else
                        {
                            objProjectSpendItemsDAL.Edit(objProjectSpendItem);
                        }
                    }
                    /*//////////CODE FOR SAVE PROJECT LINE ITEMS/////////////////*/

                    foreach (ProjectMasterMain item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objProjectMasterImport = LstProjectMaster.Where(x => x.ItemNumber.Trim().ToUpper() == item.ItemNumber.Trim().ToUpper() && x.ProjectSpendName.Trim().ToUpper() == item.ProjectSpendName.Trim().ToUpper()).FirstOrDefault();
                        if (objProjectMasterImport != null)
                        {
                            objProjectMasterImport.Status = item.Status;
                            objProjectMasterImport.Reason = item.Reason;
                            lstreturn.Add(objProjectMasterImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankProjectMasterList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.ProjectMasterImport item in CurrentBlankProjectMasterList)
                    {
                        lstreturn.Add(item);
                    }
                }

                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }
            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion


        #region [Import Item Location eVMI]
        private bool ImportItemLocationeVMI(List<ImportMastersNewDTO.InventoryLocationQuantityImport> LstInventoryLocation, out List<ImportMastersNewDTO.InventoryLocationQuantityImport> lstreturn, out string reason, out string status)
        {
            reason = "";
            status = "";
            List<UDFOptionsMain> CurrentOptionList = null;
            lstreturn = new List<ImportMastersNewDTO.InventoryLocationQuantityImport>();
            List<ImportMastersNewDTO.InventoryLocationQuantityImport> CurrentBlankInventoryLocationQtyList = new List<ImportMastersNewDTO.InventoryLocationQuantityImport>();
            List<InventoryLocationQuantityMain> CurrentInventoryLocationQtyList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);

            if (LstInventoryLocation != null && LstInventoryLocation.Count > 0)
            {

                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                List<BinMasterDTO> lstBinMaster = objItemMasterDAL.GetItemLocationsByItemNumberList(LstInventoryLocation.Select(x => x.ItemNumber).Distinct().ToList(), SessionHelper.RoomID, SessionHelper.CompanyID);

                List<BinForDefaultCheckDTO> lstCompleteBinMaster2 = (from I3 in LstInventoryLocation //.Where(y => !lstBinMaster.Select(x => x.ItemNumber.Trim().ToUpper() + "_" + x.BinNumber.Trim().ToUpper()).Contains(y.ItemNumber.Trim().ToUpper() + "_" + y.BinNumber.Trim().ToUpper()))

                                                                     join _I1 in lstBinMaster on new { ItemNumber = I3.ItemNumber.Trim().ToUpper(), BinNumber = I3.BinNumber.Trim().ToUpper() } equals new { ItemNumber = _I1.ItemNumber.Trim().ToUpper(), BinNumber = _I1.BinNumber.Trim().ToUpper() } into _I11
                                                                     from I1 in _I11.DefaultIfEmpty()

                                                                     where I1 == null

                                                                     select new BinForDefaultCheckDTO
                                                                     {
                                                                         ItemNumber = I3.ItemNumber,
                                                                         BinNumber = I3.BinNumber,
                                                                         IsDeleted = (I3.IsDeleted == null ? false : I3.IsDeleted.Value),
                                                                         IsDefault = (I3.IsDefault == null ? false : I3.IsDefault.Value)
                                                                     }).OrderBy(x => x.ItemNumber).GroupBy(x => new { x.ItemNumber, x.BinNumber, x.IsDeleted, x.IsDefault }).Select(x => new BinForDefaultCheckDTO
                                                                     {
                                                                         ItemNumber = x.Key.ItemNumber,
                                                                         BinNumber = x.Key.BinNumber,
                                                                         IsDeleted = x.Key.IsDeleted,
                                                                         IsDefault = x.Key.IsDefault
                                                                     }).ToList();

                var lstNewDefaultBins = (from I in lstCompleteBinMaster2
                                         where I.IsDeleted == false && I.IsDefault == true
                                         group I.BinNumber by I.ItemNumber into g
                                         select new { ItemNumber = g.Key, Count = g.Count() }).ToList();

                List<BinForDefaultCheckDTO> lstCompleteBinMaster1 = (from I1 in lstBinMaster

                                                                     join I2 in LstInventoryLocation
                                                                     on new { ItemNumber = I1.ItemNumber.Trim().ToUpper(), BinNumber = I1.BinNumber.Trim().ToUpper() } equals new { ItemNumber = I2.ItemNumber.Trim().ToUpper(), BinNumber = I2.BinNumber.Trim().ToUpper() }

                                                                     //join I3 in lstNewDefaultBins
                                                                     //on new { ItemNumber = I1.ItemNumber.Trim().ToUpper() } equals new { ItemNumber = I3.ItemNumber.Trim().ToUpper() }

                                                                     select new BinForDefaultCheckDTO
                                                                     {
                                                                         ItemNumber = I1.ItemNumber,
                                                                         BinNumber = I1.BinNumber,
                                                                         IsDeleted = (I2 != null ? (I2.IsDeleted == null ? false : I2.IsDeleted.Value) : (I1.IsDeleted == null ? false : I1.IsDeleted.Value)),
                                                                         IsDefault = (I2 != null ? (I2.IsDefault == null ? false : I2.IsDefault.Value) : (I1.IsDefault == null ? false : I1.IsDefault.Value))
                                                                         //IsDefault = (I2 != null ? (I2.IsDefault == null ? false : I2.IsDefault.Value) :
                                                                         //                          (I3 != null ? false : (I1.IsDefault == null ? false : I1.IsDefault.Value)))
                                                                     }).OrderBy(x => x.ItemNumber).GroupBy(x => new { x.ItemNumber, x.BinNumber, x.IsDeleted, x.IsDefault }).Select(x => new BinForDefaultCheckDTO
                                                                     {
                                                                         ItemNumber = x.Key.ItemNumber,
                                                                         BinNumber = x.Key.BinNumber,
                                                                         IsDeleted = x.Key.IsDeleted,
                                                                         IsDefault = x.Key.IsDefault
                                                                     }).ToList();

                List<BinForDefaultCheckDTO> lstCompleteBinMaster3 = (from I4 in lstBinMaster.Where(y => !LstInventoryLocation.Select(x => x.ItemNumber.Trim().ToUpper() + "_" + x.BinNumber.Trim().ToUpper()).Contains(y.ItemNumber.Trim().ToUpper() + "_" + y.BinNumber.Trim().ToUpper()))

                                                                     join _I3 in lstNewDefaultBins on new { ItemNumber = I4.ItemNumber.Trim().ToUpper() } equals new { ItemNumber = _I3.ItemNumber.Trim().ToUpper() } into _I31
                                                                     from I3 in _I31.DefaultIfEmpty()

                                                                     select new BinForDefaultCheckDTO
                                                                     {
                                                                         ItemNumber = I4.ItemNumber,
                                                                         BinNumber = I4.BinNumber,
                                                                         IsDeleted = (I4.IsDeleted == null ? false : I4.IsDeleted.Value),
                                                                         //IsDefault = (I4.IsDefault == null ? false : I4.IsDefault.Value)
                                                                         IsDefault = (I3 != null ? false : (I4.IsDefault == null ? false : I4.IsDefault.Value))
                                                                     }).OrderBy(x => x.ItemNumber).GroupBy(x => new { x.ItemNumber, x.BinNumber, x.IsDeleted, x.IsDefault }).Select(x => new BinForDefaultCheckDTO
                                                                     {
                                                                         ItemNumber = x.Key.ItemNumber,
                                                                         BinNumber = x.Key.BinNumber,
                                                                         IsDeleted = x.Key.IsDeleted,
                                                                         IsDefault = x.Key.IsDefault
                                                                     }).ToList();

                List<BinForDefaultCheckDTO> lstCompleteBinMaster = lstCompleteBinMaster1.Union(lstCompleteBinMaster2).Union(lstCompleteBinMaster3).OrderBy(x => x.ItemNumber).ToList();

                var lstValidItemNumbers = (from I in lstCompleteBinMaster
                                           where I.IsDeleted == false && I.IsDefault == true
                                           group I.BinNumber by I.ItemNumber into g
                                           select new { ItemNumber = g.Key, Count = g.Count() }).ToList();


                CurrentInventoryLocationQtyList = new List<InventoryLocationQuantityMain>();
                bool saveData = true;

                foreach (ImportMastersNewDTO.InventoryLocationQuantityImport item in LstInventoryLocation)
                {
                    saveData = true;
                    InventoryLocationQuantityMain objDTO = new InventoryLocationQuantityMain();
                    objDTO.ID = item.ID;
                    objDTO.ItemNumber = item.ItemNumber;
                    objDTO.ItemGUID = item.ItemNumber == "" ? Guid.NewGuid() : GetGUID(ImportMastersDTO.TableName.ItemMaster, item.ItemNumber);
                    objDTO.BinNumber = item.BinNumber;
                    objDTO.CriticalQuantity = item.CriticalQuantity;
                    objDTO.MinimumQuantity = item.MinimumQuantity;
                    objDTO.MaximumQuantity = item.MaximumQuantity;
                    objDTO.IsDefault = item.IsDefault;
                    objDTO.SensorId = item.SensorId;
                    objDTO.SensorPort = item.SensorPort;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();

                    var ValidItemNumber = lstValidItemNumbers.Where(x => x.ItemNumber == item.ItemNumber).FirstOrDefault();
                    if (ValidItemNumber == null)
                    {
                        item.Status = "Fail";
                        item.Reason = "ONE_DEFAULT_NEEDED";
                        saveData = false;
                    }
                    else if (ValidItemNumber.Count != 1)
                    {
                        item.Status = "Fail";
                        item.Reason = "MOTE_THEN_ONE_DEFAULT";
                        saveData = false;
                    }

                    if (!item.IsDeleted ?? false)
                    {
                        if (CheckBinStatus(item.BinNumber))
                            objDTO.IsDeleted = item.IsDeleted;
                        else
                        {
                            item.Status = "Fail";
                            item.Reason = "Location Already deleted.";
                            saveData = false;
                        }
                    }
                    else
                    {
                        BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                        BinMasterDTO objBin = objBinMasterDAL.GetRecordByItemGuid(item.BinNumber, objDTO.ItemGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                        //BinMasterDTO objBin = objBinMasterDAL.GetBinByLocationNameItemGuid( SessionHelper.RoomID, SessionHelper.CompanyID, false, false, item.BinNumber, objDTO.ItemGUID ?? Guid.Empty).FirstOrDefault();
                        if (objBin != null)
                        {
                            List<BinMasterDTO> objBinMasterList = objBinMasterDAL.CheckBinInUse_New(SessionHelper.RoomID, SessionHelper.CompanyID, objBin.ID, objDTO.ItemGUID ?? Guid.Empty);
                            foreach (var objBinMaster in objBinMasterList)
                            {
                                if (((objBinMaster.MinimumQuantity ?? 0) + (objBinMaster.MaximumQuantity ?? 0) + (objBinMaster.CriticalQuantity ?? 0) + (objBinMaster.SuggestedOrderQuantity ?? 0) + (objBinMaster.CountQuantity ?? 0) + (objBinMaster.KitMoveInOutQuantity ?? 0)) != 0)
                                {
                                    string ErrorMessage = "";
                                    if ((objBinMaster.MinimumQuantity ?? 0) > 0)
                                    {
                                        status = "referencecount";
                                        if (string.IsNullOrWhiteSpace(ErrorMessage))
                                        {
                                            ErrorMessage = eTurns.DTO.ResBin.RequisitionedQuantity;
                                        }
                                        else
                                        {
                                            ErrorMessage += ", " + eTurns.DTO.ResBin.RequisitionedQuantity;
                                        }
                                    }
                                    if ((objBinMaster.MaximumQuantity ?? 0) > 0)
                                    {
                                        status = "referencecount";
                                        if (string.IsNullOrWhiteSpace(ErrorMessage))
                                        {
                                            ErrorMessage = ResItemMaster.RequestedTransferQuantity;
                                        }
                                        else
                                        {
                                            ErrorMessage += ", " + ResItemMaster.RequestedTransferQuantity;
                                        }
                                    }
                                    if ((objBinMaster.CriticalQuantity ?? 0) > 0)
                                    {
                                        status = "referencecount";
                                        if (string.IsNullOrWhiteSpace(ErrorMessage))
                                        {
                                            ErrorMessage = eTurns.DTO.ResBin.RequestedQuantity;
                                        }
                                        else
                                        {
                                            ErrorMessage += ", " + eTurns.DTO.ResBin.RequestedQuantity;
                                        }

                                    }
                                    if ((objBinMaster.SuggestedOrderQuantity ?? 0) > 0)
                                    {
                                        status = "referencecount";
                                        if (string.IsNullOrWhiteSpace(ErrorMessage))
                                        {
                                            ErrorMessage = ResItemMaster.SuggestedOrderQuantity;
                                        }
                                        else
                                        {
                                            ErrorMessage += ", " + ResItemMaster.SuggestedOrderQuantity;
                                        }
                                    }
                                    if ((objBinMaster.CountQuantity ?? 0) > 0)
                                    {
                                        status = "referencecount";
                                        if (string.IsNullOrWhiteSpace(ErrorMessage))
                                        {
                                            ErrorMessage = ResItemMaster.CountQuantity;
                                        }
                                        else
                                        {
                                            ErrorMessage += ", " + ResItemMaster.CountQuantity;
                                        }
                                    }
                                    if ((objBinMaster.KitMoveInOutQuantity ?? 0) > 0)
                                    {
                                        status = "referencecount";
                                        if (string.IsNullOrWhiteSpace(ErrorMessage))
                                        {
                                            ErrorMessage = ResItemMaster.KitMoveInOutQuantity;
                                        }
                                        else
                                        {
                                            ErrorMessage += ", " + ResItemMaster.KitMoveInOutQuantity;
                                        }
                                    }

                                    item.Status = "ITEM_LOCATION_IN_USE";
                                    item.Reason = ErrorMessage;
                                    saveData = false;
                                }
                                else
                                {

                                    objDTO.IsDeleted = item.IsDeleted;
                                    if (objDTO.IsDeleted == true)
                                    {
                                        objDTO.IsDefault = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (objDTO.ItemGUID.ToString() != Guid.Empty.ToString() && item.BinNumber.Trim() != "" && saveData)
                    {
                        var itemval = CurrentInventoryLocationQtyList.FirstOrDefault(x => x.ItemGUID == objDTO.ItemGUID && x.BinNumber == objDTO.BinNumber);
                        if (itemval != null)
                            CurrentInventoryLocationQtyList.Remove(itemval);
                        CurrentInventoryLocationQtyList.Add(objDTO);

                    }
                    else
                        CurrentBlankInventoryLocationQtyList.Add(item);
                }

                ImportMastersNewDTO.InventoryLocationQuantityImport objInventoryLocationQtyImport;
                List<InventoryLocationQuantityMain> lstreturn1 = new List<InventoryLocationQuantityMain>();
                if (CurrentInventoryLocationQtyList.Count > 0)
                {
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.ItemLocationeVMISetup.ToString(), CurrentInventoryLocationQtyList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                }
                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {
                    List<InventoryLocationQuantityMain> lst1 = new List<InventoryLocationQuantityMain>();
                    List<InventoryLocationQuantityMain> lst2 = (List<InventoryLocationQuantityMain>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();


                    foreach (InventoryLocationQuantityMain item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objInventoryLocationQtyImport = LstInventoryLocation.Where(x => x.ItemNumber.Trim().ToUpper() == item.ItemNumber.Trim().ToUpper()).FirstOrDefault();
                        if (objInventoryLocationQtyImport != null)
                        {
                            objInventoryLocationQtyImport.Status = item.Status;
                            objInventoryLocationQtyImport.Reason = item.Reason;
                            lstreturn.Add(objInventoryLocationQtyImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankInventoryLocationQtyList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.InventoryLocationQuantityImport item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status == null))
                    {
                        item.Status = "Fail";
                        item.Reason = "Item does not exist.";
                        lstreturn.Add(item);
                    }
                    foreach (ImportMastersNewDTO.InventoryLocationQuantityImport item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status.ToLower() == "fail" && l.Reason.IndexOf("use") >= 0))
                    {
                        item.Status = "Fail";
                        item.Reason = "Location Already in use.";
                        lstreturn.Add(item);
                    }
                    foreach (ImportMastersNewDTO.InventoryLocationQuantityImport item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status.ToLower() == "fail" && l.Reason.IndexOf("deleted") >= 0))
                    {
                        item.Status = "Fail";
                        item.Reason = "Location Already deleted.";
                        lstreturn.Add(item);
                    }
                    foreach (ImportMastersNewDTO.InventoryLocationQuantityImport item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status.ToLower() == "fail" && l.Reason == "ONE_DEFAULT_NEEDED"))
                    {
                        item.Status = "Fail";
                        item.Reason = "Item must have one default location.";
                        lstreturn.Add(item);
                    }
                    foreach (ImportMastersNewDTO.InventoryLocationQuantityImport item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status.ToLower() == "fail" && l.Reason == "MOTE_THEN_ONE_DEFAULT"))
                    {
                        item.Status = "Fail";
                        item.Reason = "Item can not have more than one default location.";
                        lstreturn.Add(item);
                    }
                    foreach (ImportMastersNewDTO.InventoryLocationQuantityImport item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status.ToLower() == "item_location_in_use"))
                    {
                        item.Status = "Fail";
                        lstreturn.Add(item);
                    }

                }

                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }
            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion


        #region [Import Item Manufacturer]
        private bool ImportItemManufacturer(List<ImportMastersNewDTO.ItemManufacturerImport> LstItemManufacturer, out List<ImportMastersNewDTO.ItemManufacturerImport> lstreturn, out string reason, out string status)
        {
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.ItemManufacturerImport>();
            List<ImportMastersNewDTO.ItemManufacturerImport> CurrentBlankItemManufacturerList = new List<ImportMastersNewDTO.ItemManufacturerImport>();
            List<ItemManufacturer> CurrentItemManufacturerList = null;

            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);


            if (LstItemManufacturer != null && LstItemManufacturer.Count > 0)
            {
                CurrentItemManufacturerList = new List<ItemManufacturer>();
                CurrentOptionList = new List<UDFOptionsMain>();

                /***********MANUFACTURE MASTER*************/
                List<ManufacturerMasterDTO> objManufacturerMasterDALList = new List<ManufacturerMasterDTO>();
                ManufacturerMasterDAL objManufacturerMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                objManufacturerMasterDALList = objManufacturerMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).ToList();
                foreach (ImportMastersNewDTO.ItemManufacturerImport ManufacturerMasterList in LstItemManufacturer.GroupBy(l => l.ManufacturerName).Select(g => g.First()).ToList())
                {
                    if (ManufacturerMasterList.ManufacturerName.ToLower().Trim() != string.Empty)
                    {
                        if ((from p in objManufacturerMasterDALList
                             where ((p.Manufacturer ?? string.Empty).ToLower().Trim() == (ManufacturerMasterList.ManufacturerName.ToLower().Trim()) && p.isForBOM == false && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                             select p).Any())
                        {
                        }
                        else
                        {
                            ManufacturerMasterDTO objManufacturerMasterDTO = new ManufacturerMasterDTO();
                            objManufacturerMasterDTO.Manufacturer = ManufacturerMasterList.ManufacturerName;
                            objManufacturerMasterDTO.Room = SessionHelper.RoomID;
                            objManufacturerMasterDTO.CompanyID = SessionHelper.CompanyID;
                            objManufacturerMasterDTO.CreatedBy = SessionHelper.UserID;
                            objManufacturerMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                            objManufacturerMasterDTO.Created = DateTimeUtility.DateTimeNow;
                            objManufacturerMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                            objManufacturerMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objManufacturerMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objManufacturerMasterDTO.AddedFrom = "Web";
                            objManufacturerMasterDTO.EditedFrom = "Web";
                            objManufacturerMasterDTO.GUID = Guid.NewGuid();
                            objManufacturerMasterDTO.IsArchived = false;
                            objManufacturerMasterDTO.IsDeleted = false;
                            objManufacturerMasterDTO.isForBOM = false;
                            objManufacturerMasterDTO.RefBomId = null;
                            Int64 ManufacturerMasterID = objManufacturerMasterDAL.Insert(objManufacturerMasterDTO);
                            objManufacturerMasterDTO = objManufacturerMasterDAL.GetRecord(ManufacturerMasterID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false);
                            objManufacturerMasterDALList.Add(objManufacturerMasterDTO);
                        }
                    }
                }
                /***********MANUFACTURE MASTER*************/

                bool SaveList = true;

                foreach (ImportMastersNewDTO.ItemManufacturerImport item in LstItemManufacturer)
                {
                    SaveList = true;
                    ItemManufacturer objDTO = new ItemManufacturer();
                    Guid? ItemGUID = GetGUID(ImportMastersDTO.TableName.ItemMaster, item.ItemNumber);
                    if (string.IsNullOrEmpty(item.ManufacturerName))
                    {
                        item.Status = "Fail";
                        item.Reason = "ManufacturerName is Required.";
                    }
                    if (ItemGUID.HasValue && ItemGUID == Guid.Empty)
                    {
                        item.Status = "Fail";
                        if (!string.IsNullOrWhiteSpace(item.Reason))
                        {
                            item.Reason += Environment.NewLine + "Item does not exist.";
                        }
                        else
                        {
                            item.Reason = "Item does not exist.";
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(item.ManufacturerNumber))
                    {
                        ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                        if (objItemManufacturerDetailsDAL.CheckManufacturerNoDuplicate(item.ManufacturerNumber.Trim(), SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.ItemGUID))
                        {
                            objDTO.ManufacturerNumber = item.ManufacturerNumber;
                        }
                        else
                        {
                            item.Status = "Fail";
                            if (!string.IsNullOrWhiteSpace(item.Reason))
                            {
                                item.Reason += Environment.NewLine + item.ManufacturerNumber.Trim() + " ManufacturerNumber is already exists.";
                            }
                            else
                            {
                                item.Reason = item.ManufacturerNumber.Trim() + " ManufacturerNumber is already exists.";
                            }
                        }
                    }
                    if (item.Status.Trim().ToLowerInvariant() == "fail")
                    {
                        CurrentBlankItemManufacturerList.Add(item);
                        continue;
                    }
                    else
                    {
                        objDTO.ItemGUID = ItemGUID.Value;
                        objDTO.ManufacturerName = item.ManufacturerName;

                        objDTO.IsDefault = item.IsDefault;
                        objDTO.ManufacturerID = objManufacturerMasterDALList.ToList().Where(m => m.Manufacturer == item.ManufacturerName && m.Room == SessionHelper.RoomID && m.CompanyID == SessionHelper.CompanyID && m.IsDeleted == false && m.IsArchived == false && m.isForBOM == false).FirstOrDefault().ID;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";

                        var itemval = CurrentItemManufacturerList.FirstOrDefault(x => x.ItemGUID == objDTO.ItemGUID && x.ManufacturerName == objDTO.ManufacturerName);
                        if (itemval != null)
                            CurrentItemManufacturerList.Remove(itemval);
                        CurrentItemManufacturerList.Add(objDTO);
                    }
                }

                ImportMastersNewDTO.ItemManufacturerImport objItemManufacturerImport;
                List<ItemManufacturer> lstreturn1 = new List<ItemManufacturer>();
                if (CurrentItemManufacturerList.Count > 0)
                {
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.ItemManufacturerDetails.ToString(), CurrentItemManufacturerList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                }
                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {
                    List<ItemManufacturer> lst1 = new List<ItemManufacturer>();
                    List<ItemManufacturer> lst2 = (List<ItemManufacturer>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();

                    foreach (ItemManufacturer item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objItemManufacturerImport = LstItemManufacturer.Where(x => x.ItemNumber.Trim().ToUpper() == item.ItemNumber.Trim().ToUpper() && x.ManufacturerName.Trim().ToUpper() == item.ManufacturerName.ToUpper() && x.ManufacturerNumber.Trim().ToUpper() == item.ManufacturerNumber.ToUpper()).FirstOrDefault();
                        if (objItemManufacturerImport != null)
                        {
                            objItemManufacturerImport.Status = item.Status;
                            objItemManufacturerImport.Reason = item.Reason;
                            lstreturn.Add(objItemManufacturerImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankItemManufacturerList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.ItemManufacturerImport item in CurrentBlankItemManufacturerList)
                    {
                        lstreturn.Add(item);
                    }
                }


                if (CurrentItemManufacturerList.Count > 0)
                {
                    eTurns.DAL.CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.InvalidateCache();
                    ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                    List<ItemManufacturerDetailsDTO> objItemManufacturerDetailsDTO = new List<ItemManufacturerDetailsDTO>();
                    objItemManufacturerDetailsDTO = objItemManufacturerDetailsDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                    foreach (ItemManufacturer objCurrentItemManu in CurrentItemManufacturerList.GroupBy(l => l.ItemGUID).Select(g => g.First()).ToList())
                    {
                        foreach (ItemManufacturerDetailsDTO objItemManufacturer in objItemManufacturerDetailsDTO.ToList().Where(l => l.IsDefault == true && l.ItemGUID == objCurrentItemManu.ItemGUID))
                        {
                            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                            if (objItemManufacturer.ItemGUID != Guid.Empty)
                                objItemMasterDTO = objItemMasterDAL.GetRecordOnlyItemsFields(objItemManufacturer.ItemGUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);

                            if (objItemMasterDTO != null)
                            {
                                objItemMasterDTO.ManufacturerID = objItemManufacturer.ManufacturerID;
                                objItemMasterDTO.ManufacturerNumber = objItemManufacturer.ManufacturerNumber;
                                objItemMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                objItemMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                                objItemMasterDAL.Edit(objItemMasterDTO);
                            }
                        }
                    }
                }


                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }
            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Import Item Supplier]
        private bool ImportItemSupplier(List<ImportMastersNewDTO.ItemSupplierImport> LstItemSupplier, out List<ImportMastersNewDTO.ItemSupplierImport> lstreturn, out string reason, out string status)
        {
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.ItemSupplierImport>();
            List<ImportMastersNewDTO.ItemSupplierImport> CurrentBlankItemSupplierList = new List<ImportMastersNewDTO.ItemSupplierImport>();
            List<ItemSupplier> CurrentItemSupplierList = null;

            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);


            if (LstItemSupplier != null && LstItemSupplier.Count > 0)
            {
                CurrentItemSupplierList = new List<ItemSupplier>();
                CurrentOptionList = new List<UDFOptionsMain>();

                /***********SUPPLIER MASTER*************/
                List<SupplierMasterDTO> objSupplierMasterDALList = new List<SupplierMasterDTO>();
                SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                objSupplierMasterDALList = objSupplierMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).ToList();
                foreach (ImportMastersNewDTO.ItemSupplierImport SupplierMasterList in LstItemSupplier.GroupBy(l => l.SupplierName).Select(g => g.First()).ToList())
                {
                    if (SupplierMasterList.SupplierName.ToLower().Trim() != string.Empty)
                    {
                        if ((from p in objSupplierMasterDALList
                             where (p.SupplierName.ToLower().Trim() == (SupplierMasterList.SupplierName.ToLower().Trim()) && p.isForBOM == false && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                             select p).Any())
                        {

                        }
                        else
                        {
                            SupplierMasterDTO objSupplierMasterDTO = new SupplierMasterDTO();
                            objSupplierMasterDTO.SupplierName = SupplierMasterList.SupplierName.Trim();

                            objSupplierMasterDTO.Room = SessionHelper.RoomID;
                            objSupplierMasterDTO.CompanyID = SessionHelper.CompanyID;
                            objSupplierMasterDTO.CreatedBy = SessionHelper.UserID;
                            objSupplierMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                            objSupplierMasterDTO.Created = DateTimeUtility.DateTimeNow;
                            objSupplierMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            objSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objSupplierMasterDTO.AddedFrom = "Web";
                            objSupplierMasterDTO.EditedFrom = "Web";
                            objSupplierMasterDTO.GUID = Guid.NewGuid();
                            objSupplierMasterDTO.IsArchived = false;
                            objSupplierMasterDTO.IsDeleted = false;
                            objSupplierMasterDTO.isForBOM = false;
                            objSupplierMasterDTO.RefBomId = null;
                            objSupplierMasterDTO.IsEmailPOInBody = false;
                            objSupplierMasterDTO.IsEmailPOInPDF = false;
                            objSupplierMasterDTO.IsEmailPOInCSV = false;
                            objSupplierMasterDTO.IsEmailPOInX12 = false;
                            objSupplierMasterDTO.IsSendtoVendor = false;
                            objSupplierMasterDTO.IsVendorReturnAsn = false;
                            objSupplierMasterDTO.IsSupplierReceivesKitComponents = false;
                            Int64 SupplierMasterID = objSupplierMasterDAL.Insert(objSupplierMasterDTO);
                            objSupplierMasterDTO = objSupplierMasterDAL.GetRecord(SupplierMasterID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false);
                            objSupplierMasterDALList.Add(objSupplierMasterDTO);

                        }
                    }
                }
                /***********SUPPLIER MASTER*************/

                bool SaveList = true;

                foreach (ImportMastersNewDTO.ItemSupplierImport item in LstItemSupplier)
                {
                    SaveList = true;
                    ItemSupplier objDTO = new ItemSupplier();
                    Guid? ItemGUID = GetGUID(ImportMastersDTO.TableName.ItemMaster, item.ItemNumber);
                    if (string.IsNullOrEmpty(item.SupplierName))
                    {
                        item.Status = "Fail";
                        item.Reason = "SupplierName is Required.";
                    }
                    if (string.IsNullOrEmpty(item.SupplierNumber))
                    {
                        item.Status = "Fail";
                        if (!string.IsNullOrWhiteSpace(item.Reason))
                        {
                            item.Reason += Environment.NewLine + "SupplierNumber is Required.";
                        }
                        else
                        {
                            item.Reason = "SupplierNumber is Required.";
                        }
                    }
                    if (ItemGUID.HasValue && ItemGUID == Guid.Empty)
                    {
                        item.Status = "Fail";
                        if (!string.IsNullOrWhiteSpace(item.Reason))
                        {
                            item.Reason += Environment.NewLine + "Item does not exist.";
                        }
                        else
                        {
                            item.Reason = "Item does not exist.";
                        }
                    }
                    if (item.Status.Trim().ToLowerInvariant() == "fail")
                    {
                        CurrentBlankItemSupplierList.Add(item);
                        continue;
                    }
                    else
                    {
                        objDTO.ItemGUID = ItemGUID.Value;
                        objDTO.SupplierName = item.SupplierName.Trim();

                        objDTO.SupplierNumber = item.SupplierNumber;

                        objDTO.IsDefault = item.IsDefault;
                        objDTO.SupplierID = objSupplierMasterDALList.ToList().Where(m => m.SupplierName.ToLower().Trim() == item.SupplierName.ToLower().Trim() && m.Room == SessionHelper.RoomID && m.CompanyID == SessionHelper.CompanyID && m.IsDeleted == false && m.IsArchived == false && m.isForBOM == false).FirstOrDefault().ID;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        if (!string.IsNullOrEmpty(item.BlanketPOName))
                        {
                            objDTO.BlanketPOID = GetIDs(ImportMastersDTO.TableName.SupplierBlanketPODetails, Convert.ToString(item.BlanketPOName.Trim()), objDTO.SupplierID);
                        }
                        else
                            objDTO.BlanketPOID = null;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";

                        var itemval = CurrentItemSupplierList.FirstOrDefault(x => x.ItemGUID == objDTO.ItemGUID && x.SupplierName == objDTO.SupplierName);
                        if (itemval != null)
                            CurrentItemSupplierList.Remove(itemval);
                        CurrentItemSupplierList.Add(objDTO);
                    }
                }

                ImportMastersNewDTO.ItemSupplierImport objItemSupplierImport;
                List<ItemSupplier> lstreturn1 = new List<ItemSupplier>();
                if (CurrentItemSupplierList.Count > 0)
                {
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.ItemSupplierDetails.ToString(), CurrentItemSupplierList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                }
                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {
                    List<ItemSupplier> lst1 = new List<ItemSupplier>();
                    List<ItemSupplier> lst2 = (List<ItemSupplier>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();

                    foreach (ItemSupplier item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objItemSupplierImport = LstItemSupplier.Where(x => x.ItemNumber.Trim().ToUpper() == item.ItemNumber.Trim().ToUpper() && x.SupplierName.Trim().ToUpper() == item.SupplierName.ToUpper() && x.SupplierNumber.Trim().ToUpper() == item.SupplierNumber.ToUpper()).FirstOrDefault();
                        if (objItemSupplierImport != null)
                        {
                            objItemSupplierImport.Status = item.Status;
                            objItemSupplierImport.Reason = item.Reason;
                            lstreturn.Add(objItemSupplierImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankItemSupplierList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.ItemSupplierImport item in CurrentBlankItemSupplierList)
                    {
                        lstreturn.Add(item);
                    }
                }


                if (CurrentItemSupplierList.Count > 0)
                {
                    eTurns.DAL.CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.InvalidateCache();

                    eTurns.DAL.CacheHelper<IEnumerable<ItemSupplier>>.InvalidateCache();
                    ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                    List<ItemSupplierDetailsDTO> objItemSupplierDetailsDTO = new List<ItemSupplierDetailsDTO>();
                    objItemSupplierDetailsDTO = objItemSupplierDetailsDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                    foreach (ItemSupplier objCurrentItemSupp in CurrentItemSupplierList.GroupBy(l => l.ItemGUID).Select(g => g.First()).ToList())
                    {
                        foreach (ItemSupplierDetailsDTO objItemSupplier in objItemSupplierDetailsDTO.ToList().Where(l => l.IsDefault == true && l.ItemGUID == objCurrentItemSupp.ItemGUID))
                        {
                            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                            if (objItemSupplier.ItemGUID != Guid.Empty)
                                objItemMasterDTO = objItemMasterDAL.GetRecordOnlyItemsFields(objItemSupplier.ItemGUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);

                            if (objItemMasterDTO != null)
                            {
                                objItemMasterDTO.SupplierID = objItemSupplier.SupplierID;
                                objItemMasterDTO.SupplierPartNo = objItemSupplier.SupplierNumber;
                                objItemMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                objItemMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                                objItemMasterDAL.Edit(objItemMasterDTO);
                            }
                        }
                    }
                }


                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }
            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #region [Import Barcode]
        private bool SaveImportBarcodeMaster(List<ImportMastersNewDTO.BarcodeMasterImport> LstBarcodeMaster, out List<ImportMastersNewDTO.BarcodeMasterImport> lstreturn, out string reason, out string status)
        {
            reason = "";
            status = "";
            lstreturn = new List<ImportMastersNewDTO.BarcodeMasterImport>();
            List<ImportMastersNewDTO.BarcodeMasterImport> CurrentBlankBarcodeList = new List<ImportMastersNewDTO.BarcodeMasterImport>();
            List<ImportBarcodeMaster> CurrentBarcodeList = null;
            List<UDFOptionsCheckDTO> lst = null;
            List<UDFOptionsMain> CurrentOptionList = null;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);


            if (LstBarcodeMaster != null && LstBarcodeMaster.Count > 0)
            {
                CurrentBarcodeList = new List<ImportBarcodeMaster>();
                CurrentOptionList = new List<UDFOptionsMain>();

                List<BinMasterDTO> objBinMasterListDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                //List<BinMasterDTO> objBinMasterListDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();

                foreach (ImportMastersNewDTO.BarcodeMasterImport item in LstBarcodeMaster)
                {
                    ImportBarcodeMaster objDTO = new ImportBarcodeMaster();
                    ModuleMasterDTO objModuleMasterDTO = new ModuleMasterDAL(SessionHelper.EnterPriseDBName).GetModuleNameByName(item.ModuleName, SessionHelper.RoomID);
                    BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
                    BarcodeMasterDTO objBarcodeMasterDTO = new BarcodeMasterDTO();

                    Guid? ItemGUID = null;

                    if (item.ModuleName.Trim().ToLower() == "item master" || item.ModuleName.Trim().ToLower() == "assets" || item.ModuleName.Trim().ToLower() == "tool master")
                    {
                        if (item.ModuleName.Trim().ToLower() == "item master")
                        {
                            if (!string.IsNullOrEmpty(item.ItemNumber))
                            {
                                ItemMasterDAL objItemmasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                                ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                                ItemGUID = objItemmasterDAL.GetGuidByItemNumber(item.ItemNumber, SessionHelper.RoomID, SessionHelper.CompanyID);
                                if (ItemGUID == null)
                                {
                                    ItemGUID = Guid.Empty;
                                    item.Status = "Fail";
                                    if (!string.IsNullOrEmpty(item.Reason))
                                    {
                                        item.Reason += Environment.NewLine + "ItemNumber does not exists.";
                                    }
                                    else
                                        item.Reason = "ItemNumber does not exists.";
                                }
                            }
                        }
                        if (item.ModuleName.ToLower().Trim() == "assets")
                        {
                            if (!string.IsNullOrEmpty(item.ItemNumber))
                            {
                                AssetMasterDAL objAssetmasterDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                                List<AssetMasterDTO> objAssetmasterDTOList = new List<AssetMasterDTO>();
                                objAssetmasterDTOList = objAssetmasterDAL.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                                if (objAssetmasterDTOList != null && objAssetmasterDTOList.Count > 0)
                                {
                                    AssetMasterDTO objAssetMasterDTO = objAssetmasterDTOList.Where(a => (a.AssetName ?? string.Empty).Trim().ToLower() == item.ItemNumber.Trim().ToLower()).FirstOrDefault();
                                    if (objAssetMasterDTO != null)
                                    {
                                        ItemGUID = objAssetMasterDTO.GUID;
                                    }
                                    else
                                    {
                                        ItemGUID = Guid.Empty;
                                        item.Status = "Fail";
                                        if (!string.IsNullOrEmpty(item.Reason))
                                        {
                                            item.Reason += Environment.NewLine + "Asset does not exists.";
                                        }
                                        else
                                            item.Reason = "Asset does not exists.";
                                    }
                                }
                                else
                                {
                                    ItemGUID = Guid.Empty;
                                    item.Status = "Fail";
                                    if (!string.IsNullOrEmpty(item.Reason))
                                    {
                                        item.Reason += Environment.NewLine + "Asset does not exists.";
                                    }
                                    else
                                        item.Reason = "Asset does not exists.";
                                }
                            }
                        }
                        if (item.ModuleName.ToLower().Trim() == "tool master")
                        {
                            if (!string.IsNullOrEmpty(item.ItemNumber))
                            {
                                ToolMasterDAL objToolmasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                                List<ToolMasterDTO> objToolmasterDTOList = new List<ToolMasterDTO>();
                                objToolmasterDTOList = objToolmasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                                if (objToolmasterDTOList != null && objToolmasterDTOList.Count > 0)
                                {
                                    ToolMasterDTO objToolMasterDTO = objToolmasterDTOList.Where(a => (a.Serial ?? string.Empty).Trim().ToLower() == item.ItemNumber.Trim().ToLower()).FirstOrDefault();
                                    if (objToolMasterDTO != null)
                                    {
                                        ItemGUID = objToolMasterDTO.GUID;
                                    }
                                    else
                                    {
                                        ItemGUID = Guid.Empty;
                                        item.Status = "Fail";
                                        if (!string.IsNullOrEmpty(item.Reason))
                                        {
                                            item.Reason += Environment.NewLine + "Tool serial does not exists.";
                                        }
                                        else
                                            item.Reason = "Tool serial does not exists.";
                                    }
                                }
                                else
                                {
                                    ItemGUID = Guid.Empty;
                                    item.Status = "Fail";
                                    if (!string.IsNullOrEmpty(item.Reason))
                                    {
                                        item.Reason += Environment.NewLine + "Tool serial does not exists.";
                                    }
                                    else
                                        item.Reason = "Tool serial does not exists.";
                                }
                            }
                        }
                    }
                    else
                    {
                        ItemGUID = Guid.Empty;
                        item.Status = "Fail";
                        if (!string.IsNullOrEmpty(item.Reason))
                        {
                            item.Reason += Environment.NewLine + "ModuleName should be Item master or Assets or Tool Master.";
                        }
                        else
                            item.Reason = "ModuleName should be Item master or Assets or Tool Master.";
                    }

                    if (ItemGUID.HasValue && ItemGUID != Guid.Empty)
                    {
                        objBarcodeMasterDTO = objBarcodeMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).ToList().Where(b => b.BarcodeString == item.BarcodeString && b.RefGUID == ItemGUID.Value).FirstOrDefault();
                    }
                    else
                    {
                        objBarcodeMasterDTO = null;
                    }

                    if (objBarcodeMasterDTO != null)
                    {
                        item.Status = "Fail";
                        if (!string.IsNullOrEmpty(item.Reason))
                        {
                            item.Reason += Environment.NewLine + "Barcode string already exists.";
                        }
                        else
                            item.Reason = "Barcode string already exists.";
                    }

                    if (item.Status.Trim().ToLowerInvariant() == "fail")
                    {
                        CurrentBlankBarcodeList.Add(item);
                        continue;
                    }
                    else
                    {
                        objDTO.RefGuid = ItemGUID.Value;
                        objDTO.BarcodeString = item.BarcodeString;
                        objDTO.ModuleGuid = objModuleMasterDTO.GUID;

                        if (item.ModuleName == "Item Master")
                        {

                            if (!string.IsNullOrEmpty(item.BinNumber))
                            {
                                BinMasterDTO objBinMasterDTO = objBinMasterListDTO.Where(b => b.BinNumber.ToLower().Trim() == item.BinNumber.ToLower().Trim() && b.ItemGUID == ItemGUID.Value).FirstOrDefault();
                                if (objBinMasterDTO != null)
                                {
                                    objDTO.BinGuid = objBinMasterDTO.GUID;
                                }
                                else
                                {
                                    BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                                    BinMasterDTO objBinMasterDTOToInsert = null;
                                    {
                                        objBinMasterDTOToInsert = new BinMasterDTO();
                                        objBinMasterDTOToInsert.BinNumber = item.BinNumber.Trim();
                                        objBinMasterDTOToInsert.ParentBinId = null;
                                        objBinMasterDTOToInsert.CreatedBy = SessionHelper.UserID;
                                        objBinMasterDTOToInsert.LastUpdatedBy = SessionHelper.UserID;
                                        objBinMasterDTOToInsert.Created = DateTimeUtility.DateTimeNow;
                                        objBinMasterDTOToInsert.LastUpdated = DateTimeUtility.DateTimeNow;
                                        objBinMasterDTOToInsert.Room = SessionHelper.RoomID;
                                        objBinMasterDTOToInsert.CompanyID = SessionHelper.CompanyID;
                                        objBinMasterDTOToInsert.AddedFrom = "Web";
                                        objBinMasterDTOToInsert.EditedFrom = "Web";
                                        objBinMasterDTOToInsert.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objBinMasterDTOToInsert.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objBinMasterDTOToInsert.IsOnlyFromItemUI = true;
                                        objBinMasterDTOToInsert = objBinMasterDAL.InsertBin(objBinMasterDTOToInsert);

                                    }
                                    objDTO.BinGuid = (objBinMasterDTOToInsert.GUID);
                                    BinMasterDTO objInventoryLocation = objBinMasterDAL.GetInventoryLocation(objBinMasterDTOToInsert.ID, ItemGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID);

                                    if (objInventoryLocation == null)
                                    {
                                        objInventoryLocation = new BinMasterDTO();
                                        objInventoryLocation.BinNumber = objBinMasterDTOToInsert.BinNumber;
                                        objInventoryLocation.ParentBinId = objBinMasterDTOToInsert.ID;
                                        objInventoryLocation.CreatedBy = SessionHelper.UserID;
                                        objInventoryLocation.LastUpdatedBy = SessionHelper.UserID;
                                        objInventoryLocation.Created = DateTimeUtility.DateTimeNow;
                                        objInventoryLocation.LastUpdated = DateTimeUtility.DateTimeNow;
                                        objInventoryLocation.MinimumQuantity = objBinMasterDTOToInsert.MinimumQuantity;
                                        objInventoryLocation.MaximumQuantity = objBinMasterDTOToInsert.MaximumQuantity;
                                        objInventoryLocation.CriticalQuantity = objBinMasterDTOToInsert.CriticalQuantity;
                                        objInventoryLocation.eVMISensorID = objBinMasterDTOToInsert.eVMISensorID;
                                        objInventoryLocation.eVMISensorPort = objBinMasterDTOToInsert.eVMISensorPort;
                                        objInventoryLocation.IsDefault = objBinMasterDTOToInsert.IsDefault;
                                        objInventoryLocation.ItemGUID = ItemGUID.Value;
                                        objInventoryLocation.Room = SessionHelper.RoomID;
                                        objInventoryLocation.CompanyID = SessionHelper.CompanyID;
                                        objInventoryLocation.AddedFrom = "Web";
                                        objInventoryLocation.EditedFrom = "Web";
                                        objInventoryLocation.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objInventoryLocation.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objInventoryLocation.IsOnlyFromItemUI = true;
                                        objInventoryLocation = objBinMasterDAL.InsertBin(objInventoryLocation);
                                        objDTO.BinGuid = (objInventoryLocation.GUID);
                                    }
                                    else
                                    {
                                        objInventoryLocation.LastUpdated = DateTimeUtility.DateTimeNow;
                                        objInventoryLocation.LastUpdatedBy = SessionHelper.UserID;
                                        objInventoryLocation.MinimumQuantity = objBinMasterDTOToInsert.MinimumQuantity;
                                        objInventoryLocation.MaximumQuantity = objBinMasterDTOToInsert.MaximumQuantity;
                                        objInventoryLocation.CriticalQuantity = objBinMasterDTOToInsert.CriticalQuantity;
                                        objInventoryLocation.eVMISensorID = objBinMasterDTOToInsert.eVMISensorID;
                                        objInventoryLocation.eVMISensorPort = objBinMasterDTOToInsert.eVMISensorPort;
                                        objInventoryLocation.IsDefault = objBinMasterDTOToInsert.IsDefault;
                                        objInventoryLocation.EditedFrom = "Web";
                                        objInventoryLocation.IsOnlyFromItemUI = true;
                                        objInventoryLocation.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objBinMasterDAL.Edit(objInventoryLocation);
                                    }
                                }
                            }
                            else
                            {
                                BinMasterDTO objBinMasterDTO = objBinMasterListDTO.Where(b => b.IsDefault == true && b.ItemGUID == ItemGUID.Value).FirstOrDefault();
                                if (objBinMasterDTO != null)
                                    objDTO.BinGuid = objBinMasterDTO.GUID;
                            }

                        }

                        objDTO.ModuleName = item.ModuleName;
                        objDTO.ItemNumber = item.ItemNumber;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.CreatedOn = DateTimeUtility.DateTimeNow;
                        objDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                        objDTO.UpdatedBy = SessionHelper.UserID;
                        objDTO.RoomID = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.BarcodeAdded = "Manual";

                        var itemval = CurrentBarcodeList.FirstOrDefault(x => x.ItemNumber.Trim().ToUpperInvariant() == item.ItemNumber.Trim().ToUpperInvariant() && x.BarcodeString.Trim().ToUpperInvariant() == item.BarcodeString.Trim().ToUpperInvariant() && x.ModuleName.Trim().ToUpperInvariant() == item.ModuleName.Trim().ToUpperInvariant());
                        if (itemval != null)
                            CurrentBarcodeList.Remove(itemval);
                        CurrentBarcodeList.Add(objDTO);

                        item.Status = "Success";
                        item.Reason = "N/A";
                    }
                }

                ImportMastersNewDTO.BarcodeMasterImport objBarcodeMasterImport;
                List<ImportBarcodeMaster> lstreturn1 = new List<ImportBarcodeMaster>();
                if (CurrentBarcodeList.Count > 0)
                {
                    lstreturn1 = objImport.BulkInsert(ImportMastersDTO.TableName.BarcodeMaster.ToString(), CurrentBarcodeList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                }
                //--------------------------------------------------------
                //
                if (lstreturn1.Count > 0)
                {
                    List<ImportBarcodeMaster> lst1 = new List<ImportBarcodeMaster>();
                    List<ImportBarcodeMaster> lst2 = (List<ImportBarcodeMaster>)lstreturn1;
                    lst1 = lst1.Union(lst2).ToList();


                    foreach (ImportBarcodeMaster item in lst1.Where(x => x.Status.ToUpper() != "SUCCESS"))
                    {
                        objBarcodeMasterImport = LstBarcodeMaster.Where(x => x.ItemNumber.Trim().ToUpper() == item.ItemNumber.Trim().ToUpper() && x.BarcodeString.Trim().ToUpper() == item.BarcodeString.Trim().ToUpper() && x.ModuleName.Trim().ToUpper() == item.ModuleName.Trim().ToUpper()).FirstOrDefault();
                        if (objBarcodeMasterImport != null)
                        {
                            objBarcodeMasterImport.Status = item.Status;
                            objBarcodeMasterImport.Reason = item.Reason;
                            lstreturn.Add(objBarcodeMasterImport);
                        }
                    }
                }

                //--------------------------------------------------------
                //
                if (CurrentBlankBarcodeList.Count > 0)
                {
                    foreach (ImportMastersNewDTO.BarcodeMasterImport item in CurrentBlankBarcodeList)
                    {
                        lstreturn.Add(item);
                    }
                }

                if (lstreturn.Count == 0)
                {
                    status = "success";
                    reason = "";
                    return true;
                }
                else
                {
                    status = "Fail";
                    reason = lstreturn.Count.ToString() + " records not imported successfully";
                    return false;
                }
            }
            else
            {
                status = "Fail";
                reason = "No records found to import";
                return false;
            }
        }
        #endregion

        #endregion
        private static Int64 GetIDs(ImportMastersDTO.TableName TableName, string strVal, long longID = 0)
        {
            Int64 returnID = 0;
            CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            if (string.IsNullOrEmpty(strVal)) return returnID;

            #region Get Manufacture ID
            if (TableName == ImportMastersDTO.TableName.ManufacturerMaster)
            {
                ManufacturerMasterDAL objDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.ManufacturerMaster.ToString(), "Manufacturer", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    ManufacturerMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(c => (c.Manufacturer ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    ManufacturerMasterDTO objDTO = new ManufacturerMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Manufacturer = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion

            #region Get Supplier ID
            else if (TableName == ImportMastersDTO.TableName.SupplierMaster)
            {
                SupplierMasterDAL objDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

                //string strOK = objCDal.SupplierDuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.SupplierMaster.ToString(), "SupplierName", SessionHelper.RoomID, SessionHelper.CompanyID);
                string strOK = objDAL.SupplierDuplicateCheck(0, strVal, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    SupplierMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(c => (c.SupplierName ?? string.Empty).ToLower().Trim() == strVal.ToLower().Trim()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    SupplierMasterDTO objDTO = new SupplierMasterDTO();
                    objDTO.ID = 1;
                    objDTO.SupplierName = strVal.Length > 255 ? strVal.Trim().Substring(0, 255) : strVal.Trim();
                    objDTO.IsEmailPOInBody = false;
                    objDTO.IsEmailPOInPDF = false;
                    objDTO.IsEmailPOInCSV = false;
                    objDTO.IsEmailPOInX12 = false;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get SupplierBlanketPODetails ID
            else if (TableName == ImportMastersDTO.TableName.SupplierBlanketPODetails)
            {
                SupplierBlanketPODetailsDAL objDAL = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName);
                returnID = objDAL.SupplierBlanketPODetailsDuplicateCheck(0, strVal, longID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (returnID == 0)
                {
                    SupplierBlanketPODetailsDTO oSupplierBlanketPODetailsDTO = new SupplierBlanketPODetailsDTO();
                    oSupplierBlanketPODetailsDTO.SupplierID = longID;
                    oSupplierBlanketPODetailsDTO.BlanketPO = strVal;
                    oSupplierBlanketPODetailsDTO.GUID = Guid.NewGuid();
                    oSupplierBlanketPODetailsDTO.Created = DateTime.Now;
                    oSupplierBlanketPODetailsDTO.CreatedBy = SessionHelper.UserID;
                    oSupplierBlanketPODetailsDTO.Updated = DateTime.Now;
                    oSupplierBlanketPODetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                    oSupplierBlanketPODetailsDTO.CompanyID = SessionHelper.CompanyID;
                    oSupplierBlanketPODetailsDTO.Room = SessionHelper.RoomID;
                    oSupplierBlanketPODetailsDTO.IsArchived = false;
                    oSupplierBlanketPODetailsDTO.IsDeleted = false;
                    oSupplierBlanketPODetailsDTO.AddedFrom = "Web";
                    oSupplierBlanketPODetailsDTO.EditedFrom = "Web";
                    oSupplierBlanketPODetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    oSupplierBlanketPODetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName).Insert(oSupplierBlanketPODetailsDTO);
                }
            }
            #endregion
            #region Get Category ID
            else if (TableName == ImportMastersDTO.TableName.CategoryMaster)
            {
                CategoryMasterDAL objDAL = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.CategoryMaster.ToString(), "Category", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    CategoryMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(c => (c.Category ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    CategoryMasterDTO objDTO = new CategoryMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Category = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get GLAccount ID
            else if (TableName == ImportMastersDTO.TableName.GLAccountMaster)
            {
                GLAccountMasterDAL objDAL = new GLAccountMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.GLAccountMaster.ToString(), "GLAccount", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    GLAccountMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(c => (c.GLAccount ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    GLAccountMasterDTO objDTO = new GLAccountMasterDTO();
                    objDTO.ID = 1;
                    objDTO.GLAccount = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();

                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get Unit ID
            else if (TableName == ImportMastersDTO.TableName.UnitMaster)
            {
                UnitMasterDAL objDAL = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.UnitMaster.ToString(), "Unit", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    UnitMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(c => (c.Unit ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    UnitMasterDTO objDTO = new UnitMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Unit = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get Location ID
            else if (TableName == ImportMastersDTO.TableName.BinMaster)
            {
                BinMasterDAL objDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.BinMaster.ToString(), "BinNumber", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    BinMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(c => (c.BinNumber ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    //obj = objDAL.GetBinMasterByLocationName(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, strVal).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    BinMasterDTO objDTO = new BinMasterDTO();
                    objDTO.ID = 1;
                    objDTO.BinNumber = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get LocationMaster ID
            else if (TableName == ImportMastersDTO.TableName.LocationMaster)
            {
                LocationMasterDAL objDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.LocationMaster.ToString(), "Location", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    LocationMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(c => (c.Location ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    LocationMasterDTO objDTO = new LocationMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Location = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = Guid.NewGuid();

                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get ToolCategoryMaster ID
            else if (TableName == ImportMastersDTO.TableName.ToolCategoryMaster)
            {
                ToolCategoryMasterDAL objDAL = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.ToolCategoryMaster.ToString(), "ToolCategory", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    ToolCategoryMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(c => (c.ToolCategory ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    ToolCategoryMasterDTO objDTO = new ToolCategoryMasterDTO();
                    objDTO.ID = 1;
                    objDTO.ToolCategory = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get InventoryClassificationMaster ID
            else if (TableName == ImportMastersDTO.TableName.InventoryClassificationMaster)
            {
                InventoryClassificationMasterDAL objInventoryClassificationMasterDAL = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.InventoryClassificationMaster.ToString(), "InventoryClassification", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    InventoryClassificationMasterDTO obj = null;
                    obj = objInventoryClassificationMasterDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(c => (c.InventoryClassification ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    InventoryClassificationMasterDTO objDTO = new InventoryClassificationMasterDTO();
                    objDTO.ID = 1;
                    objDTO.InventoryClassification = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;

                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    returnID = objInventoryClassificationMasterDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get CostUOM ID
            if (TableName == ImportMastersDTO.TableName.CostUOMMaster)
            {
                CostUOMMasterDAL objDAL = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.CostUOMMaster.ToString(), "CostUOM", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    CostUOMMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(c => (c.CostUOM ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    CostUOMMasterDTO objDTO = new CostUOMMasterDTO();
                    objDTO.ID = 0;
                    objDTO.CostUOM = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.CostUOMValue = 1;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.isForBOM = false;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion

            #region Get AssetCategoryMaster ID
            else if (TableName == ImportMastersDTO.TableName.AssetCategoryMaster)
            {
                AssetCategoryMasterDAL objDAL = new AssetCategoryMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.AssetCategoryMaster.ToString(), "AssetCategory", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    AssetCategoryMasterDTO obj = null;
                    obj = objDAL.GetAssetCategoryByName(strVal, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    AssetCategoryMasterDTO objDTO = new AssetCategoryMasterDTO();
                    objDTO.ID = 1;
                    objDTO.AssetCategory = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            return returnID;
        }

        public Guid GetGUID(ImportMastersDTO.TableName TableName, string strVal, string optValue = "", QuickListType QLType = QuickListType.General)
        {
            Guid returnID = Guid.NewGuid();
            CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            #region Get QuickList GUID
            if (TableName == ImportMastersDTO.TableName.QuickListItems)
            {
                QuickListDAL objDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, "QuickListMaster", "Name", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    QuickListMasterDTO obj = null;
                    //obj = objDAL.GetQuickListMasterCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(c => c.Name == strVal).FirstOrDefault();
                    obj = objDAL.GetQuickListMasterByName(strVal, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    if (obj != null)
                    {
                        obj.Type = (int)QLType;
                        objDAL.Edit(obj);
                        returnID = obj.GUID;
                    }
                }
                else
                {
                    QuickListMasterDTO objDTO = new QuickListMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Name = strVal;
                    objDTO.Comment = optValue;
                    objDTO.Type = (int)QLType;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();

                    returnID = objDAL.InsertQuickList(objDTO);
                }
            }
            else if (TableName == ImportMastersDTO.TableName.ItemMaster)
            {
                ItemMasterDAL objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                returnID = objDAL.GetGuidByItemNumber(strVal, SessionHelper.RoomID, SessionHelper.CompanyID) ?? Guid.Empty;
            }
            else if (TableName == ImportMastersDTO.TableName.AssetMaster)
            {
                AssetMasterDAL objDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                AssetMasterDTO objDTO = new AssetMasterDTO();
                objDTO = objDAL.GetAssetsByName(strVal, SessionHelper.RoomID, SessionHelper.CompanyID).FirstOrDefault();
                if (objDTO != null)
                {
                    returnID = objDTO.GUID;
                }
            }
            #endregion
            return returnID;
        }

        public Guid GetItemGUID_IsActive(ImportMastersDTO.TableName TableName, string strVal, out bool isActive, out int itemType)
        {
            isActive = true;
            itemType = 0;
            Guid returnID = Guid.NewGuid();

            if (TableName == ImportMastersDTO.TableName.ItemMaster)
            {
                ItemMasterDAL objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                new ItemMasterDTO();
                ItemMasterDTO objItemMaster = objDAL.GetRecordByItemNumber(strVal, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objItemMaster != null)
                {
                    returnID = objItemMaster.GUID;
                    isActive = objItemMaster.IsActive;
                    itemType = objItemMaster.ItemType;
                }
                else
                {
                    returnID = Guid.Empty;
                }
            }

            return returnID;
        }

        public byte? getTrendingID(string name)
        {
            byte id = 0;
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            else
            {
                if (name == "None")
                {
                    id = 0;
                }
                else if (name == "Manual")
                {
                    id = 1;
                }
                else if (name == "Automatic")
                {
                    id = 2;
                }
                else
                {
                    return null;
                }

            }
            return id;
        }

        private static void CheckUDF(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList, string objDTOUDF, string UDFs)
        {

            if (objDTOUDF.Trim() != "")
            {
                List<UDFOptionsCheckDTO> lstcount = new List<UDFOptionsCheckDTO>();
                lstcount = lst.Where(c => c.UDFColumnName == UDFs.ToString()).ToList();
                if (lstcount.Count > 0)
                {
                    UDFOptionsCheckDTO objcheck = new UDFOptionsCheckDTO();
                    objcheck = lst.Where(c => c.UDFColumnName == UDFs.ToString() && c.UDFOption == objDTOUDF && c.UDFID == lstcount[0].UDFID).FirstOrDefault();
                    int objcheckCount = CurrentOptionList.Where(c => c.UDFOption == objDTOUDF && c.UDFID == lstcount[0].UDFID).Count();
                    if (objcheck == null && objcheckCount == 0)
                    {
                        UDFOptionsMain objoptionDTO = new UDFOptionsMain();
                        objoptionDTO.ID = 0;
                        objoptionDTO.Created = DateTimeUtility.DateTimeNow;
                        objoptionDTO.CreatedBy = SessionHelper.UserID;
                        objoptionDTO.Updated = DateTimeUtility.DateTimeNow;
                        objoptionDTO.LastUpdatedBy = SessionHelper.UserID;
                        objoptionDTO.IsDeleted = false;

                        objoptionDTO.UDFOption = objDTOUDF;
                        objoptionDTO.UDFID = lstcount[0].UDFID;
                        objoptionDTO.GUID = Guid.NewGuid();
                        CurrentOptionList.Add(objoptionDTO);
                    }
                }
            }
        }

        private static bool CheckUDFIsRequired(IEnumerable<UDFDTO> DataFromDB, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, out string Reason, string forCheckout = null)
        {
            bool isRequired = false;
            Reason = string.Empty;

            if (DataFromDB != null && DataFromDB.Count() > 0)
            {
                foreach (var i in DataFromDB)
                {
                    if (i.UDFColumnName == "UDF1" && string.IsNullOrEmpty(UDF1))
                        Reason = Reason + " " + (forCheckout ?? string.Empty) + i.UDFColumnName + " is required.";
                    if (i.UDFColumnName == "UDF2" && string.IsNullOrEmpty(UDF2))
                        Reason = Reason + " " + (forCheckout ?? string.Empty) + i.UDFColumnName + " is required.";
                    if (i.UDFColumnName == "UDF3" && string.IsNullOrEmpty(UDF3))
                        Reason = Reason + " " + (forCheckout ?? string.Empty) + i.UDFColumnName + " is required.";
                    if (i.UDFColumnName == "UDF4" && string.IsNullOrEmpty(UDF4))
                        Reason = Reason + " " + (forCheckout ?? string.Empty) + i.UDFColumnName + " is required.";
                    if (i.UDFColumnName == "UDF5" && string.IsNullOrEmpty(UDF5))
                        Reason = Reason + " " + (forCheckout ?? string.Empty) + i.UDFColumnName + " is required.";
                }

                if (!string.IsNullOrEmpty(Reason))
                {
                    isRequired = true;
                }
            }

            return isRequired;
        }

        private static bool CheckUDFIsRequired_Asset(IEnumerable<UDFDTO> DataFromDB, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, string UDF6, string UDF7, string UDF8, string UDF9, string UDF10, out string Reason)
        {
            bool isRequired = false;
            Reason = string.Empty;

            if (DataFromDB != null && DataFromDB.Count() > 0)
            {
                foreach (var i in DataFromDB)
                {
                    if (i.UDFColumnName == "UDF1" && string.IsNullOrEmpty(UDF1))
                        Reason = Reason + " " + i.UDFColumnName + " is required.";
                    if (i.UDFColumnName == "UDF2" && string.IsNullOrEmpty(UDF2))
                        Reason = Reason + " " + i.UDFColumnName + " is required.";
                    if (i.UDFColumnName == "UDF3" && string.IsNullOrEmpty(UDF3))
                        Reason = Reason + " " + i.UDFColumnName + " is required.";
                    if (i.UDFColumnName == "UDF4" && string.IsNullOrEmpty(UDF4))
                        Reason = Reason + " " + i.UDFColumnName + " is required.";
                    if (i.UDFColumnName == "UDF5" && string.IsNullOrEmpty(UDF5))
                        Reason = Reason + " " + i.UDFColumnName + " is required.";

                    if (i.UDFColumnName == "UDF6" && string.IsNullOrEmpty(UDF6))
                        Reason = Reason + " " + i.UDFColumnName + " is required.";
                    if (i.UDFColumnName == "UDF7" && string.IsNullOrEmpty(UDF7))
                        Reason = Reason + " " + i.UDFColumnName + " is required.";
                    if (i.UDFColumnName == "UDF8" && string.IsNullOrEmpty(UDF8))
                        Reason = Reason + " " + i.UDFColumnName + " is required.";
                    if (i.UDFColumnName == "UDF9" && string.IsNullOrEmpty(UDF9))
                        Reason = Reason + " " + i.UDFColumnName + " is required.";
                    if (i.UDFColumnName == "UDF10" && string.IsNullOrEmpty(UDF10))
                        Reason = Reason + " " + i.UDFColumnName + " is required.";
                }

                if (!string.IsNullOrEmpty(Reason))
                {
                    isRequired = true;
                }
            }

            return isRequired;
        }


        #region General Methods
        private string SaveUploadedFile(HttpPostedFileBase uploadFile, string ImportModule)
        {
            string path = string.Empty;
            // Verify that the user selected a file
            if (uploadFile != null && uploadFile.ContentLength > 0)
            {

                string RenamedFileName = ImportModule + "_" + ((SessionHelper.EnterPriceName ?? string.Empty) == string.Empty ? ("No Enterprise(" + SessionHelper.EnterPriceID + ")") : SessionHelper.EnterPriceName + "(" + SessionHelper.EnterPriceID + ")") + "_" + SessionHelper.CompanyName + "(" + SessionHelper.CompanyID + ")" + "_" + SessionHelper.RoomName + "(" + SessionHelper.RoomID + ")" + "_" + DateTime.Now.Ticks.ToString();
                // extract only the fielname
                string fileName = Path.GetFileName(uploadFile.FileName);
                // store the file inside ~/App_Data/uploads folder
                string[] strfilename = fileName.Split('.');


                if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
                {
                    RenamedFileName = RenamedFileName + "_O_" + fileName.Substring(0, fileName.Length - 4);
                    path = Path.Combine(Server.MapPath("~/Uploads/Import/CSV"), RenamedFileName + ".csv");
                }
                else if (strfilename[strfilename.Length - 1].ToUpper() == "XLS")
                {
                    path = Path.Combine(Server.MapPath("~/Uploads/Import/Excel"), RenamedFileName + ".xls");
                }
                //Excel
                if (!string.IsNullOrEmpty(path))
                    uploadFile.SaveAs(path);
            }
            return path;
        }

        public bool CheckBinStatus(string BinNumber)
        {
            bool result = false;
            try
            {
                BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                List<BinMasterDTO> objBinMasterList = objBinMasterDAL.GetListBinByName(BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, false);

                if (objBinMasterList != null)
                {
                    int deletedCount = objBinMasterList.Where(b => b.IsDeleted == true).Count();
                    int totalCount = objBinMasterList.Count();
                    if (totalCount > 0 && totalCount == deletedCount)
                        result = false;
                    else
                    {
                        result = true;
                    }
                }
                else
                {
                    result = true;
                }
            }
            catch
            {
                return false;
            }
            return result;
        }

        public string GetDBFields(string ModuleName)
        {
            string retval = string.Empty;
            if (!string.IsNullOrEmpty(ModuleName))
            {
                switch (ModuleName)
                {
                    case "CategoryMaster":
                        retval = "category,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "ItemMaster":
                        retval = "itemnumber,manufacturer,manufacturernumber,suppliername,supplierpartno,blanketordernumber,upc,unspsc,description,longdescription,categoryname,glaccount,uom,costuom,defaultreorderquantity,defaultpullquantity,cost,markup,sellprice,extendedcost,leadtimeindays,link1,link2,trend,taxable,consignment,stagedquantity,intransitquantity,onorderquantity,ontransferquantity,suggestedorderquantity,requisitionedquantity,averageusage,turns,onhandquantity,criticalquantity,minimumquantity,maximumquantity,weightperpiece,itemuniquenumber,istransfer,ispurchase,inventrylocation,inventoryclassification,serialnumbertracking,lotnumbertracking,datecodetracking,itemtype,imagepath,udf1,udf2,udf3,udf4,udf5,islotserialexpirycost,isitemlevelminmaxqtyrequired,trendingsetting,enforcedefaultpullquantity,enforcedefaultreorderquantity,isautoinventoryclassification,isbuildbreak,ispackslipmandatoryatreceive,itemimageexternalurl,itemdocexternalurl,isdeleted,itemlink2externalurl,isactive";
                        break;
                    case "BOMItemMaster":
                        retval = "itemnumber,manufacturer,manufacturernumber,suppliername,supplierpartno,upc,unspsc,description,longdescription,categoryname,glaccount,unit,leadtimeindays,taxable,consignment,itemuniquenumber,istransfer,ispurchase,inventoryclassification,serialnumbertracking,lotnumbertracking,datecodetracking,itemtype";
                        break;
                    case "CostUOMMaster":
                        retval = "costuom,costuomvalue,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "CustomerMaster":
                        retval = "customer,account,contact,address,city,state,zipcode,country,phone,email,udf1,udf2,udf3,udf4,udf5,remarks";
                        break;
                    case "FreightTypeMaster":
                        retval = "freighttype,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "GLAccountMaster":
                        retval = "glaccount,description,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "InventoryClassificationMaster":
                        retval = "inventoryclassification,rangestart,rangeend,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "BinMaster":
                        retval = "itemnumber,locationname,customerownedquantity,consignedquantity,serialnumber,lotnumber,expirationdate";//,receiveddate
                        break;
                    case "ItemLocationeVMISetup":
                        retval = "itemnumber,locationname,minimumquantity,maximumquantity,criticalquantity,sensorid,sensorport,isdefault,isdeleted";
                        break;
                    case "ManufacturerMaster":
                        retval = "manufacturer,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "MeasurementTermMaster":
                        retval = "measurementterm,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "SupplierMaster":
                        //retval = "suppliername,description,receiverid,address,city,state,zipcode,country,contact,phone,fax,email,isemailpoinbody,isemailpoinpdf,isemailpoincsv,isemailpoinx12,udf1,udf2,udf3,udf4,udf5";
                        retval = "suppliername,suppliercolor,description,branchnumber,maximumordersize,address,city,state,zipcode,country,contact,phone,fax,issendtovendor,isvendorreturnasn,issupplierreceiveskitcomponents,ordernumbertypeblank,ordernumbertypefixed,ordernumbertypeblanketordernumber,ordernumbertypeincrementingordernumber,ordernumbertypeincrementingbyday,ordernumbertypedateincrementing,ordernumbertypedate,udf1,udf2,udf3,udf4,udf5,accountnumber,accountname,accountaddress,accountcity,accountstate,accountzip,accountisdefault,blanketponumber,startdate,enddate,maxlimit,donotexceed,PullPurchaseNumberFixed,PullPurchaseNumberBlanketOrder,PullPurchaseNumberDateIncrementing,PullPurchaseNumberDate,PullPurchaseNumberType,LastPullPurchaseNumberUsed,isblanketdeleted,supplierimage,imageexternalurl,maxlimitqty,donotexceedqty";
                        break;
                    case "ShipViaMaster":
                        retval = "shipvia,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "TechnicianMaster":
                        retval = "technician,techniciancode,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "ToolCategoryMaster":
                        retval = "toolcategory,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "LocationMaster":
                        retval = "location,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "UnitMaster":
                        retval = "unit,description,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "ToolMaster":
                        retval = "toolname,serial,description,toolcategory,cost,quantity,location,udf1,udf2,udf3,udf4,udf5,isgroupofitems,Technician,CheckOutQuantity,CheckInQuantity,checkoutudf1,checkoutudf2,checkoutudf3,checkoutudf4,checkoutudf5,imagepath,toolimageexternalurl";
                        break;
                    case "AssetMaster":
                        retval = "assetname,description,make,model,serial,toolcategory,purchasedate,purchaseprice,depreciatedvalue,udf1,udf2,udf3,udf4,udf5,udf6,udf7,udf8,udf9,udf10,assetcategory,imagepath,assetimageexternalurl";
                        break;
                    case "QuickListItems":
                        retval = "name,type,comment,itemnumber,binnumber,quantity,consignedquantity,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "kitdetail":
                        retval = "kitpartnumber,itemnumber,quantityperkit,availablekitquantity,isbuildbreak,isdeleted,description,criticalquantity,minimumquantity,maximumquantity,reordertype,suppliername,supplierpartno,defaultlocation,costuomname,uom,defaultreorderquantity,defaultpullquantity,isitemlevelminmaxqtyrequired,serialnumbertracking,lotnumbertracking,datecodetracking,isactive";
                        break;
                    case "ItemManufacturerDetails":
                        retval = "itemnumber,manufacturername,manufacturernumber,isdefault";
                        break;
                    case "ItemSupplierDetails":
                        retval = "itemnumber,suppliername,suppliernumber,blanketponame,isdefault";
                        break;
                    case "BarcodeMaster":
                        retval = "itemnumber,modulename,barcodestring,binnumber";
                        break;
                    case "UDF":
                        retval = "modulename,udfcolumnname,udfname,controltype,defaultvalue,optionname,isrequired,isdeleted,includeinnarrowsearch";
                        break;
                    case "ProjectMaster":
                        retval = "projectspendname,description,dollarlimitamount,trackallusageagainstthis,isclosed,isdeleted,udf1,udf2,udf3,udf4,udf5,itemnumber,itemdollarlimitamount,itemquantitylimitamount,islineitemdeleted";
                        break;
                    //case "InventoryLocation":
                    //    retval = "itemnumber,locationname,criticalquantity,minimumquantity,maximumquantity,sensorid,sensorport,isdefault";
                    //    break;
                    case "ItemLocationQty":
                        retval = "itemnumber,locationname,customerownedquantity,consignedquantity,serialnumber,lotnumber,expirationdate,cost,receivedate";//
                        break;
                    case "ManualCount":
                        retval = "itemnumber,locationname,customerownedquantity,consignedquantity,serialnumber,lotnumber,expirationdate,projectspend";//
                        break;
                    case "WorkOrder":
                        retval = "woname,releasenumber,wostatus,technician,customer,udf1,udf2,udf3,udf4,udf5,wotype,description,suppliername";
                        break;
                    case "PullMaster":
                        retval = "ItemNumber,PullQuantity,Location,UDF1,UDF2,UDF3,UDF4,UDF5,ProjectSpendName,PullOrderNumber,WorkOrder,ActionType";
                        break;

                }

            }
            return retval;
        }

        public string GetRequiredDBFields(string ModuleName)
        {
            string retval = string.Empty;
            if (!string.IsNullOrEmpty(ModuleName))
            {
                switch (ModuleName)
                {
                    case "CategoryMaster":
                        retval = "category";
                        break;
                    case "ItemMaster":
                        retval = "itemtype,defaultpullquantity,defaultreorderquantity,itemnumber,suppliername,supplierpartno,uom,costuom,inventrylocation,isitemlevelminmaxqtyrequired";
                        break;
                    case "BOMItemMaster":
                        retval = "serialnumbertracking,lotnumbertracking,datecodetracking,itemtype,consignment,taxable,itemnumber,suppliername,unit";
                        break;
                    case "CostUOMMaster":
                        retval = "costuom,costuomvalue";
                        break;
                    case "CustomerMaster":
                        retval = "customer,account";
                        break;
                    case "FreightTypeMaster":
                        retval = "freighttype";
                        break;
                    case "GLAccountMaster":
                        retval = "glaccount";
                        break;
                    case "InventoryClassificationMaster":
                        retval = "inventoryclassification,rangestart,rangeend";
                        break;
                    case "BinMaster":
                        retval = "itemnumber,locationname";
                        break;
                    case "ItemLocationeVMISetup":
                        retval = "itemnumber,locationname";
                        break;
                    case "ManufacturerMaster":
                        retval = "manufacturer";
                        break;
                    case "MeasurementTermMaster":
                        retval = "measurementterm";
                        break;
                    case "SupplierMaster":
                        retval = "suppliername";
                        break;
                    case "ShipViaMaster":
                        retval = "shipvia";
                        break;
                    case "TechnicianMaster":
                        retval = "techniciancode";
                        break;
                    case "ToolCategoryMaster":
                        retval = "toolcategory";
                        break;
                    case "LocationMaster":
                        retval = "location";
                        break;
                    case "UnitMaster":
                        retval = "unit";
                        break;
                    case "ToolMaster":
                        retval = "toolname,serial,quantity";
                        break;
                    case "AssetMaster":
                        retval = "assetname";
                        break;
                    case "QuickListItems":
                        retval = "name,type,itemnumber,quantity,consignedquantity";
                        break;
                    case "kitdetail":
                        retval = "kitpartnumber,itemnumber,quantityperkit,suppliername,supplierpartno,defaultlocation,costuomname,uom,defaultreorderquantity,defaultpullquantity,isitemlevelminmaxqtyrequired";
                        break;
                    case "ItemManufacturerDetails":
                        retval = "itemnumber,manufacturername";
                        break;
                    case "ItemSupplierDetails":
                        retval = "itemnumber,suppliername";
                        break;
                    case "BarcodeMaster":
                        retval = "modulename,itemnumber,barcodestring";
                        break;
                    case "UDF":
                        retval = "modulename,udfcolumnname,udfname,controltype";
                        break;
                    case "ProjectMaster":
                        retval = "projectspendname,dollarlimitamount,itemnumber";
                        break;
                    //case "InventoryLocation":
                    //    retval = "ItemNumber,LocationName";
                    //    break;
                    case "ItemLocationQty":
                        retval = "itemnumber,locationname";
                        break;
                    case "ManualCount":
                        retval = "itemnumber,locationname";
                        break;
                    case "WorkOrder":
                        retval = "woname";
                        break;
                    case "PullMaster":
                        retval = "ItemNumber,PullQuantity,Location,ActionType";
                        break;
                }

            }
            return retval;
        }

        public bool GetCSVData(string Fields, string RequiredField, HttpPostedFileBase uploadFile, out DataTable dtCSV, out string Datatablecolumns, out string ErrorMessage)
        {
            dtCSV = new DataTable();
            Datatablecolumns = string.Empty;
            ErrorMessage = string.Empty;
            try
            {

                DataRow dr;
                string[] DBReqField = RequiredField.ToLower().Split(',');
                string[] DBField = Fields.ToLower().Split(',');
                //foreach (string item in DBField)
                //{
                //    DataColumn dc = new DataColumn();
                //    dc.ColumnName = item.ToString();
                //    dtCSV.Columns.Add(dc);
                //}

                if (uploadFile.ContentLength > 0)
                {
                    string[] value = { "" };
                    DataTable dtCSVTemp = new DataTable();
                    Stream objstr = uploadFile.InputStream;
                    StreamReader sr = new StreamReader(objstr);
                    objstr.Position = 0;
                    sr.DiscardBufferedData();
                    string separator = ConfigurationManager.AppSettings["CSVseparator"].ToString();
                    string headerLine = sr.ReadLine();

                    string[] strHeaderArray = headerLine.Replace("*", "").ToLower().Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();

                    foreach (string item in DBReqField)
                    {
                        //if (!headerLine.ToLower().Contains(item.ToLower()))
                        //{
                        //    Session["ErrorMessage"] = "Invalid CSV file.";
                        //    return null;
                        //}
                        if (!strHeaderArray.Contains(item.ToLower()))
                        {
                            ErrorMessage = "Invalid CSV file.";
                            return false;
                        }
                    }

                    //foreach (string itemDB in DBField)
                    //{
                    //    foreach (string itemHeader in strHeaderArray)
                    //    {
                    //        if (itemDB.ToLower() == itemHeader.ToLower())
                    //        {
                    //            DataColumn dc = new DataColumn();
                    //            dc.ColumnName = itemDB.ToString();
                    //            dtCSV.Columns.Add(dc);
                    //            if (string.IsNullOrEmpty(Datatablecolumns))
                    //            {
                    //                Datatablecolumns = itemDB.ToString();
                    //            }
                    //            else
                    //            {
                    //                Datatablecolumns += "," + itemDB.ToString();
                    //            }
                    //        }

                    //    }
                    //}

                    /* ============================================================ */
                    DataColumn dcCommon;
                    var CommonAvailableFields = DBField.Intersect(strHeaderArray);
                    foreach (var itemDB in CommonAvailableFields)
                    {
                        dcCommon = new DataColumn();
                        dcCommon.ColumnName = itemDB.ToString();
                        dtCSV.Columns.Add(dcCommon);
                        if (string.IsNullOrEmpty(Datatablecolumns))
                        {
                            Datatablecolumns = itemDB.ToString();
                        }
                        else
                        {
                            Datatablecolumns += "," + itemDB.ToString();
                        }
                    }
                    /* ============================================================ */

                    foreach (string item in strHeaderArray)
                    {
                        DataColumn dc = new DataColumn();
                        dc.ColumnName = item.ToString();
                        dtCSVTemp.Columns.Add(dc);
                    }
                    using (TextFieldParser parser = new TextFieldParser(sr))
                    {

                        parser.Delimiters = new string[] { separator };
                        parser.HasFieldsEnclosedInQuotes = true;
                        while (true)
                        {
                            value = parser.ReadFields();

                            if (value == null)
                            {
                                break;
                            }
                            else
                            {
                                char[] trimchar = new char[] { '\'' };
                                List<string> artmp = new List<string>();
                                int CoulumnCounter = 1;
                                foreach (string item in value)
                                {
                                    if (CoulumnCounter <= dtCSVTemp.Columns.Count)
                                    {
                                        if (!string.IsNullOrEmpty(item))
                                        {
                                            string tmpstringda;
                                            if (item.ToString().ToUpperInvariant() == "NULL")
                                            {
                                                tmpstringda = string.Empty;
                                            }
                                            else
                                            {
                                                tmpstringda = item;
                                            }
                                            tmpstringda = tmpstringda.TrimStart(trimchar);
                                            artmp.Add(tmpstringda);
                                        }
                                        else
                                        {
                                            artmp.Add(item);
                                        }
                                    }
                                    CoulumnCounter++;
                                }
                                value = artmp.ToArray();
                                dr = dtCSVTemp.NewRow();
                                dr.ItemArray = value;
                                dtCSVTemp.Rows.Add(dr);

                            }

                        }
                    }
                    //if (dtCSVTemp != null && dtCSVTemp.Rows.Count > 0)
                    //{
                    //    foreach (DataRow dtcsvtemprow in dtCSVTemp.Rows)
                    //    {
                    //        dr = dtCSV.NewRow();

                    //        foreach (DataColumn column in dtCSVTemp.Columns)
                    //        {
                    //            foreach (DataColumn dtcolumn in dtCSV.Columns)
                    //            {
                    //                if (column.ColumnName.ToLower() == dtcolumn.ColumnName.ToLower())
                    //                {
                    //                    dr[dtcolumn] = dtcsvtemprow[column];
                    //                }
                    //            }
                    //        }
                    //        dtCSV.Rows.Add(dr);
                    //    }
                    //}
                    if (dtCSVTemp != null && dtCSVTemp.Rows.Count > 0)
                    {
                        DataTable dtCSVCopy = new DataTable();
                        dtCSVCopy = dtCSVTemp.Copy();
                        dtCSVCopy.AcceptChanges();
                        foreach (DataColumn col in dtCSVTemp.Columns)
                        {

                            if (!Fields.ToLower().Contains(col.ToString().ToLower()))
                            {
                                dtCSVCopy.Columns.Remove(col.ToString().ToLower());
                            }

                        }
                        dtCSV = dtCSVCopy;
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return true;
        }

        private CustomerMasterDTO GetCustomerMaster(ImportMastersDTO.TableName TableName, string strVal, long longID = 0)
        {
            #region Get Customer ID
            CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            CustomerMasterDTO objDTO = new CustomerMasterDTO();
            if (TableName == ImportMastersDTO.TableName.CustomerMaster)
            {
                CustomerMasterDAL objDAL = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.CustomerMaster.ToString(), "Customer", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    CustomerMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(c => (c.Customer ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        return obj;
                }
                else
                {

                    objDTO.ID = 1;
                    objDTO.Customer = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.Account = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    Int64 returnID = objDAL.Insert(objDTO);
                    objDTO.ID = returnID;

                }
            }
            return objDTO;
            #endregion
        }

        public void InsertCheckOutUDf(string UDFOption, string UDFColumn)
        {
            string UdfTablename = ImportMastersDTO.TableName.ToolCheckInOutHistory.ToString();
            Int64 UDFID = 0;
            List<UDFDTO> objUDFDTOList = new List<UDFDTO>();
            UDFDAL objUDFDAL = new UDFDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            objUDFDTOList = objUDFDAL.GetPagedRecords(0, 10, out TotalRecordCount, string.Empty, "ID desc", SessionHelper.CompanyID, UdfTablename, SessionHelper.RoomID).ToList();
            if (objUDFDTOList != null && objUDFDTOList.Count > 0)
            {
                UDFID = objUDFDTOList.Where(u => u.UDFColumnName == UDFColumn).FirstOrDefault().ID;
                CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                string strOK = objCDAL.DuplicateUFDOptionCheck(UDFOption, "add", 0, "UDFOptions", "UDFOption", UDFID);
                if (strOK == "duplicate")
                {

                }
                else
                {
                    //UDFOptionApiController obj = new UDFOptionApiController();
                    UDFOptionDAL obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);

                    UDFOptionsDTO objDTO = new UDFOptionsDTO();
                    objDTO.ID = 0;
                    objDTO.UDFOption = UDFOption;
                    objDTO.UDFID = UDFID;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;

                    objDTO.CompanyID = SessionHelper.CompanyID;
                    //objDTO.Roo = 0;

                    objDTO.GUID = Guid.NewGuid();

                    var ResponseStatus = obj.Insert(objDTO);

                }
            }
        }

        public DataTable GetTableFromList(List<ItemLocationDetailsDTO> lstItemLocs)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
            DataTable ReturnDT = new DataTable("ItemLocationParam");
            try
            {
                DataColumn[] arrColumns = new DataColumn[]            {
                new DataColumn() { AllowDBNull=true,ColumnName="ItemGUID",DataType=typeof(Guid)},
                new DataColumn() { AllowDBNull=true,ColumnName="ItemNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="BinID",DataType=typeof(Int64)},
                new DataColumn() { AllowDBNull=true,ColumnName="BinNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="Expiration",DataType=typeof(DateTime)},
                new DataColumn() { AllowDBNull=true,ColumnName="Received",DataType=typeof(DateTime)},
                new DataColumn() { AllowDBNull=true,ColumnName="LotNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="SerialNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="ConsignedQuantity",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="CustomerOwnedQuantity",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="ReceiptCost",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF1",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF2",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF3",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF4",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF5",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="ProjectSpend",DataType=typeof(String)}
            };
                ReturnDT.Columns.AddRange(arrColumns);

                if (lstItemLocs != null && lstItemLocs.Count > 0)
                {
                    foreach (var item in lstItemLocs)
                    {
                        DateTime tempDT = DateTime.Now;
                        DateTime.TryParseExact(item.Expiration, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out tempDT);
                        DataRow row = ReturnDT.NewRow();
                        row["ItemGUID"] = item.ItemGUID;
                        row["ItemNumber"] = item.ItemNumber;
                        row["BinID"] = (item.BinID ?? 0) > 0 ? (object)item.BinID : DBNull.Value;
                        row["BinNumber"] = item.BinNumber;
                        row["Expiration"] = tempDT != DateTime.MinValue ? (object)tempDT : DBNull.Value;
                        row["Received"] = datetimetoConsider;
                        row["LotNumber"] = item.LotNumber;
                        row["SerialNumber"] = item.SerialNumber;
                        row["ConsignedQuantity"] = item.ConsignedQuantity;
                        row["CustomerOwnedQuantity"] = item.CustomerOwnedQuantity;
                        row["ReceiptCost"] = (item.Cost ?? 0) > 0 ? (object)item.Cost : DBNull.Value;
                        row["UDF1"] = string.Empty;
                        row["UDF2"] = string.Empty;
                        row["UDF3"] = string.Empty;
                        row["UDF4"] = string.Empty;
                        row["UDF5"] = string.Empty;
                        row["ProjectSpend"] = item.ProjectSpend;
                        ReturnDT.Rows.Add(row);
                    }
                }

                return ReturnDT;
            }
            catch (Exception ex)
            {
                return ReturnDT;
            }
        }

        private static Int64 GetBOMIDs(ImportMastersDTO.TableName TableName, string strVal)
        {
            Int64 returnID = 0;
            CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            if (string.IsNullOrEmpty(strVal)) return returnID;

            #region Get Manufacture ID
            if (TableName == ImportMastersDTO.TableName.ManufacturerMaster)
            {
                ManufacturerMasterDAL objDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.ManufacturerMaster.ToString(), "Manufacturer", 0, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    ManufacturerMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).Where(c => (c.Manufacturer ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    ManufacturerMasterDTO objDTO = new ManufacturerMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Manufacturer = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.isForBOM = true;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get Supplier ID
            else if (TableName == ImportMastersDTO.TableName.SupplierMaster)
            {
                SupplierMasterDAL objDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.SupplierMaster.ToString(), "SupplierName", 0, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    SupplierMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).Where(c => (c.SupplierName ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    SupplierMasterDTO objDTO = new SupplierMasterDTO();
                    objDTO.ID = 1;
                    objDTO.SupplierName = strVal.Length > 255 ? strVal.Substring(0, 255) : strVal;
                    objDTO.IsEmailPOInBody = false;
                    objDTO.IsEmailPOInPDF = false;
                    objDTO.IsEmailPOInCSV = false;
                    objDTO.IsEmailPOInX12 = false;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.isForBOM = true;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get Category ID
            else if (TableName == ImportMastersDTO.TableName.CategoryMaster)
            {
                CategoryMasterDAL objDAL = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.CategoryMaster.ToString(), "Category", 0, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    CategoryMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).Where(c => (c.Category ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    CategoryMasterDTO objDTO = new CategoryMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Category = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.isForBOM = true;
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get GLAccount ID
            else if (TableName == ImportMastersDTO.TableName.GLAccountMaster)
            {
                GLAccountMasterDAL objDAL = new GLAccountMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.GLAccountMaster.ToString(), "GLAccount", 0, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    GLAccountMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).Where(c => (c.GLAccount ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    GLAccountMasterDTO objDTO = new GLAccountMasterDTO();
                    objDTO.ID = 1;
                    objDTO.GLAccount = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.isForBOM = true;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get Unit ID
            else if (TableName == ImportMastersDTO.TableName.UnitMaster)
            {
                UnitMasterDAL objDAL = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.UnitMaster.ToString(), "Unit", 0, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    UnitMasterDTO obj = null;
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).Where(c => (c.Unit ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    UnitMasterDTO objDTO = new UnitMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Unit = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.isForBOM = true;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion

            #region Get InventoryClassificationMaster ID
            else if (TableName == ImportMastersDTO.TableName.InventoryClassificationMaster)
            {
                InventoryClassificationMasterDAL objInventoryClassificationMasterDAL = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.InventoryClassificationMaster.ToString(), "InventoryClassification", 0, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    InventoryClassificationMasterDTO obj = null;
                    obj = objInventoryClassificationMasterDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).Where(c => (c.InventoryClassification ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    InventoryClassificationMasterDTO objDTO = new InventoryClassificationMasterDTO();
                    objDTO.ID = 1;
                    objDTO.InventoryClassification = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;

                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.isForBOM = true;
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    returnID = objInventoryClassificationMasterDAL.Insert(objDTO);
                }
            }
            #endregion

            return returnID;
        }

        #endregion
    }
}
