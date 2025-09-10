using eTurns.DAL;
using eTurns.DTO;
using eTurnsWeb.Helper;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eTurnsWeb.Controllers
{
    public class ImportNewController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ImportMasters()
        {
            ImportMastersNewDTO.ImportMaster objImportMasterDTO = new ImportMastersNewDTO.ImportMaster();
            objImportMasterDTO.ImportModule = Convert.ToInt32(ImportMastersNewDTO.TableName.ItemMaster);
            objImportMasterDTO.IsFileSuccessfulyUploaded = false;


            List<SelectListItem> lstModules = Enum.GetValues(typeof(ImportMastersNewDTO.TableName)).Cast<ImportMastersNewDTO.TableName>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.DDLModulelst = lstModules;

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
                CommonDAL objDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                //RoomDTO objRoomDTO = objRoomDal.GetRoomByIDPlain(SessionHelper.RoomID);
                string columnList = "ID,RoomName,DefaultSupplierID,DefaultBinID";
                RoomDTO objRoomDTO = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

                if (objRoomDTO != null)
                {
                    if (objRoomDTO.DefaultSupplierID != null & objRoomDTO.DefaultSupplierID > 0)
                    {
                        SupplierMasterDAL objSuppDal = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                        SupplierMasterDTO objSuppDTO = new SupplierMasterDTO();
                        objSuppDTO = objSuppDal.GetSupplierByIDPlain(objRoomDTO.DefaultSupplierID.GetValueOrDefault(0));
                        if (objSuppDTO != null)
                            vDefaultSupplier = objSuppDTO.SupplierName;
                    }

                    if (objRoomDTO.DefaultBinID != null & objRoomDTO.DefaultBinID > 0)
                    {
                        BinMasterDAL objBinDal = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                        BinMasterDTO objBinDTO = new BinMasterDTO();
                        objBinDTO = objBinDal.GetBinByID(objRoomDTO.DefaultBinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                        //objBinDTO = objBinDal.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, objRoomDTO.DefaultBinID.GetValueOrDefault(0), null,null).FirstOrDefault();
                        if (objBinDTO != null)
                            vDefaultLocation = objBinDTO.BinNumber;
                    }

                    UnitMasterDAL objUnitDal = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
                    try
                    {
                        UnitMasterDTO objUnit = objUnitDal.GetUnitByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, "EA");
                        if (objUnit != null && objUnit.ID > 0)
                            vDefaultUOM = objUnit.Unit;

                    }
                    catch { }

                }

                switch ((ImportMastersNewDTO.TableName)objImportMasterDTO.ImportModule)
                {
                    case (ImportMastersNewDTO.TableName.ItemMaster):
                        List<ImportMastersNewDTO.ItemMasterImport> lstImport;
                        if (GetItemMasterDataFromFile(objImportMasterDTO.UploadFile, vDefaultSupplier, vDefaultUOM, vDefaultLocation, objImportMasterDTO.ImportModule.ToString(), out lstImport, out DataTableColumns, out ErrorMessage))
                        {
                            objImportMasterDTO.objImportPageDTO = new ImportMastersNewDTO.ImportPageDTO();
                            objImportMasterDTO.objImportPageDTO.DataTableColumns = DataTableColumns.Split(',');
                            objImportMasterDTO.objImportPageDTO.lstItemMasterImportData = lstImport;
                            objImportMasterDTO.IsFileSuccessfulyUploaded = true;
                            TempData["objImportMasterDTO"] = objImportMasterDTO;
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
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.DDLModulelst = lstModules;

            return View(objImportMasterDTO);
        }

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
                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsAllowOrderCostuom.ToString()))
                        {
                            Boolean IsAllowOrderCostuom = false;
                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsAllowOrderCostuom.ToString()]), out IsAllowOrderCostuom))
                                obj.IsAllowOrderCostuom = IsAllowOrderCostuom;
                            else
                                obj.IsAllowOrderCostuom = false;
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

        public PartialViewResult ItemMasterImport4()
        {
            ImportMastersNewDTO.ImportMaster objImportMaster = (ImportMastersNewDTO.ImportMaster)TempData["objImportMasterDTO"];
            return PartialView("_ItemMasterImport4", objImportMaster);
        }

        public JsonResult ImportSave(string TableName, List<ImportMastersNewDTO.ItemMasterImport> lstItemMasterImportData)
        {
            if (ImportMastersDTO.TableName.ItemMaster.ToString() == TableName)
            {

            }

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
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
                uploadFile.SaveAs(path);
            }
            return path;
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
                        retval = "itemnumber,manufacturer,manufacturernumber,suppliername,supplierpartno,blanketordernumber,upc,unspsc,description,longdescription,categoryname,glaccount,uom,costuom,defaultreorderquantity,defaultpullquantity,cost,markup,sellprice,extendedcost,leadtimeindays,link1,link2,trend,taxable,consignment,stagedquantity,intransitquantity,onorderquantity,ontransferquantity,suggestedorderquantity,requisitionedquantity,averageusage,turns,onhandquantity,criticalquantity,minimumquantity,maximumquantity,weightperpiece,itemuniquenumber,istransfer,ispurchase,inventrylocation,inventoryclassification,serialnumbertracking,lotnumbertracking,datecodetracking,itemtype,imagepath,udf1,udf2,udf3,udf4,udf5,islotserialexpirycost,isitemlevelminmaxqtyrequired,trendingsetting,enforcedefaultpullquantity,enforcedefaultreorderquantity,isautoinventoryclassification,isbuildbreak,ispackslipmandatoryatreceive,itemimageexternalurl,itemdocexternalurl,isdeleted,itemlink2externalurl,isactive,isallowordercostuom";
                        break;
                    case "BOMItemMaster":
                        retval = "itemnumber,manufacturer,manufacturernumber,suppliername,supplierpartno,upc,unspsc,description,longdescription,categoryname,glaccount,unit,leadtimeindays,taxable,consignment,itemuniquenumber,istransfer,ispurchase,inventoryclassification,serialnumbertracking,lotnumbertracking,datecodetracking,itemtype,udf1,udf2,udf3,udf4,udf5";
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
                        retval = "suppliername,suppliercolor,description,branchnumber,maximumordersize,address,city,state,zipcode,country,contact,phone,fax,email,issendtovendor,isvendorreturnasn,issupplierreceiveskitcomponents,ordernumbertypeblank,ordernumbertypefixed,ordernumbertypeblanketordernumber,ordernumbertypeincrementingordernumber,ordernumbertypeincrementingbyday,ordernumbertypedateincrementing,ordernumbertypedate,udf1,udf2,udf3,udf4,udf5,accountnumber,accountname,accountaddress,accountcity,accountstate,accountzip,accountisdefault,blanketponumber,startdate,enddate,maxlimit,donotexceed,PullPurchaseNumberFixed,PullPurchaseNumberBlanketOrder,PullPurchaseNumberDateIncrementing,PullPurchaseNumberDate,PullPurchaseNumberType,LastPullPurchaseNumberUsed,isblanketdeleted";
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
                        retval = "toolname,serial,description,toolcategory,cost,quantity,location,udf1,udf2,udf3,udf4,udf5,isgroupofitems,Technician,CheckOutQuantity,CheckInQuantity,checkoutudf1,checkoutudf2,checkoutudf3,checkoutudf4,checkoutudf5";
                        break;
                    case "AssetMaster":
                        retval = "assetname,description,make,model,serial,toolcategory,purchasedate,purchaseprice,depreciatedvalue,udf1,udf2,udf3,udf4,udf5,udf6,udf7,udf8,udf9,udf10,assetcategory";
                        break;
                    case "QuickListItems":
                        retval = "name,type,comment,itemnumber,binnumber,quantity,consignedquantity,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "kitdetail":
                        retval = "kitpartnumber,itemnumber,quantityperkit,availablekitquantity,isbuildbreak,isdeleted,description,criticalquantity,minimumquantity,maximumquantiy,reordertype,suppliername,supplierpartno,defaultlocation,costuomname,uom,defaultreorderquantity,defaultpullquantity,isitemlevelminmaxqtyrequired,serialnumbertracking,lotnumbertracking,datecodetracking,isactive";
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
                        retval = "itemnumber,locationname,customerownedquantity,consignedquantity,serialnumber,lotnumber,expirationdate";//
                        break;
                    case "WorkOrder":
                        retval = "woname,releasenumber,wostatus,technician,customer,udf1,udf2,udf3,udf4,udf5,wotype,description,suppliername";
                        break;
                    case "PullMaster":
                        retval = "ItemNumber,PullQuantity,Location,UDF1,UDF2,UDF3,UDF4,UDF5,ProjectSpendName,PullOrderNumber,WorkOrder,ActionType";
                        break;
                    case "PullImportWithLotSerial":
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
                    case "PullImportWithLotSerial":
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
                            ErrorMessage = ResImportMasters.ErrorInvalidFile;
                            return false;
                        }
                    }

                    foreach (string itemDB in DBField)
                    {
                        foreach (string itemHeader in strHeaderArray)
                        {
                            if (itemDB.ToLower() == itemHeader.ToLower())
                            {
                                DataColumn dc = new DataColumn();
                                dc.ColumnName = itemDB.ToString();
                                dtCSV.Columns.Add(dc);
                                if (string.IsNullOrEmpty(Datatablecolumns))
                                {
                                    Datatablecolumns = itemDB.ToString();
                                }
                                else
                                {
                                    Datatablecolumns += "," + itemDB.ToString();
                                }
                            }

                        }
                    }

                    foreach (string item in strHeaderArray)
                    {
                        DataColumn dc = new DataColumn();
                        dc.ColumnName = item.ToString();
                        dtCSVTemp.Columns.Add(dc);
                    }
                    using (TextFieldParser parser = new TextFieldParser(sr))
                    {

                        parser.Delimiters = new string[] { separator };
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
                                foreach (var item in value)
                                {
                                    if (CoulumnCounter <= dtCSVTemp.Columns.Count)
                                    {
                                        if (!string.IsNullOrEmpty(item))
                                        {
                                            string tmpstringda = item;
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
            catch
            {

            }

            return true;
        }
        #endregion
    }
}
