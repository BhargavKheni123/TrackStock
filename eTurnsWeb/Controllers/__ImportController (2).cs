using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eTurnsWeb.Helper;
using System.Text;
using System.IO;
using System.Data;
using eTurns.DTO.Resources;
using System.Net;
using eTurns.DTO;
using eTurns.DAL;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Net.Mail;
using Microsoft.VisualBasic.FileIO;
using NPOI.HSSF.UserModel;

//using System.Reflection;
namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class ImportController : eTurnsControllerBase
    {
        #region Property Declaration
        private const string _CURRENTBINSESSIONKEY = "CURRENTBINLIST";
        private const string _CURRENTCATEGORYSESSIONKEY = "CURRENTCATEGORYLIST";
        private const string _CURRENTCUSTOMERSESSIONKEY = "CURRENTCUSTOMERLIST";
        private const string _CURRENTFREIGHTTYPESESSIONKEY = "CURRENTFREIGHTTYPELIST";
        private const string _CURRENTGLACCOUNTSESSIONKEY = "CURRENTGLACCOUNTLIST";
        private const string _CURRENTGXPRCONSIGNEDSESSIONKEY = "CURRENTGXPRCONSIGNEDLIST";
        private const string _CURRENTJOBTYPESESSIONKEY = "CURRENTJOBTYPELIST";
        private const string _CURRENTSHIPVIASESSIONKEY = "CURRENTSHIPVIALIST";
        private const string _CURRENTTECHNICIANSESSIONKEY = "CURRENTTECHNICIANLIST";
        private const string _CURRENTMANUFACTURERSESSIONKEY = "CURRENTMANUFACTURERLIST";
        private const string _CURRENTMEASUREMENTTERMSESSIONKEY = "CURRENTMEASUREMENTTERMLIST";
        private const string _CURRENTUNITSSESSIONKEY = "CURRENTUNITSLIST";
        private const string _CURRENTSUPPLIERSESSIONKEY = "CURRENTSUPPLIERLIST";
        private const string _CURRENTITEMSESSIONKEY = "CURRENTITEMLIST";
        private const string _CURRENTLOCATIONSESSIONKEY = "CURRENTLOCATIONLIST";
        private const string _CURRENTTOOLCATEGORYSESSIONKEY = "CURRENTTOOLCATEGORYLIST";
        private const string _CURRENTINVENTORYCLASSIFICATIONSESSIONKEY = "CURRENTINVENTORYCLASSIFICATIONLIST";
        private const string _CURRENTCOSTUOMMASTERSESSIONKEY = "CURRENTCOSTUOMMASTERLIST";
        private const string _CURRENTTOOLMASTERLIST = "CURRENTTOOLMASTERLIST";
        private const string _CURRENTASSETMASTERLIST = "CURRENTASSETMASTERLIST";
        private const string _CURRENTQUICKLISTMASTERLIST = "CURRENTQUICKLISTMASTERLIST";
        private const string _CURRENTINVENTORYLOCATIONMASTERLIST = "CURRENTINVENTORYLOCATIONMASTERLIST";
        private const string _CURRENTINVENTORYLOCATIONQuantityLIST = "CURRENTINVENTORYLOCATIONQuantityLIST";
        private const string _CURRENTBOMITEMSESSIONKEY = "CURRENTBOMITEMLIST";
        private const string _CURRENTKitItemSESSIONKEY = "CURRENTKitITEMLIST";

        /// <summary>
        /// Set Current Resource in Session
        /// </summary>
        private List<InventoryLocationMain> CurrentBinList
        {
            get
            {
                if (HttpContext.Session[_CURRENTBINSESSIONKEY] != null)
                    return (List<InventoryLocationMain>)HttpContext.Session[_CURRENTBINSESSIONKEY];
                return new List<InventoryLocationMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTBINSESSIONKEY] = value;
            }
        }
        private List<CategoryMasterMain> CurrentCategoryList
        {
            get
            {
                if (HttpContext.Session[_CURRENTCATEGORYSESSIONKEY] != null)
                    return (List<CategoryMasterMain>)HttpContext.Session[_CURRENTCATEGORYSESSIONKEY];
                return new List<CategoryMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTCATEGORYSESSIONKEY] = value;
            }
        }
        private List<CostUOMMasterMain> CurrentCostUOMList
        {
            get
            {
                if (HttpContext.Session[_CURRENTCOSTUOMMASTERSESSIONKEY] != null)
                    return (List<CostUOMMasterMain>)HttpContext.Session[_CURRENTCOSTUOMMASTERSESSIONKEY];
                return new List<CostUOMMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTCOSTUOMMASTERSESSIONKEY] = value;
            }
        }
        private List<InventoryClassificationMasterMain> CurrentInventoryClassificationList
        {
            get
            {
                if (HttpContext.Session[_CURRENTINVENTORYCLASSIFICATIONSESSIONKEY] != null)
                    return (List<InventoryClassificationMasterMain>)HttpContext.Session[_CURRENTINVENTORYCLASSIFICATIONSESSIONKEY];
                return new List<InventoryClassificationMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTINVENTORYCLASSIFICATIONSESSIONKEY] = value;
            }
        }
        private List<CustomerMasterMain> CurrentCustomerList
        {
            get
            {
                if (HttpContext.Session[_CURRENTCUSTOMERSESSIONKEY] != null)
                    return (List<CustomerMasterMain>)HttpContext.Session[_CURRENTCUSTOMERSESSIONKEY];
                return new List<CustomerMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTCUSTOMERSESSIONKEY] = value;
            }
        }
        private List<FreightTypeMasterMain> CurrentFreightType
        {
            get
            {
                if (HttpContext.Session[_CURRENTFREIGHTTYPESESSIONKEY] != null)
                    return (List<FreightTypeMasterMain>)HttpContext.Session[_CURRENTFREIGHTTYPESESSIONKEY];
                return new List<FreightTypeMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTFREIGHTTYPESESSIONKEY] = value;
            }
        }
        private List<GLAccountMasterMain> CurrentGLAccountList
        {
            get
            {
                if (HttpContext.Session[_CURRENTGLACCOUNTSESSIONKEY] != null)
                    return (List<GLAccountMasterMain>)HttpContext.Session[_CURRENTGLACCOUNTSESSIONKEY];
                return new List<GLAccountMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTGLACCOUNTSESSIONKEY] = value;
            }
        }
        private List<GXPRConsignedMasterMain> CurrentGXPRConsignedList
        {
            get
            {
                if (HttpContext.Session[_CURRENTGXPRCONSIGNEDSESSIONKEY] != null)
                    return (List<GXPRConsignedMasterMain>)HttpContext.Session[_CURRENTGXPRCONSIGNEDSESSIONKEY];
                return new List<GXPRConsignedMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTGXPRCONSIGNEDSESSIONKEY] = value;
            }
        }
        private List<JobTypeMasterMain> CurrentJobTypeList
        {
            get
            {
                if (HttpContext.Session[_CURRENTJOBTYPESESSIONKEY] != null)
                    return (List<JobTypeMasterMain>)HttpContext.Session[_CURRENTJOBTYPESESSIONKEY];
                return new List<JobTypeMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTJOBTYPESESSIONKEY] = value;
            }
        }
        private List<ShipViaMasterMain> CurrentShipViaList
        {
            get
            {
                if (HttpContext.Session[_CURRENTSHIPVIASESSIONKEY] != null)
                    return (List<ShipViaMasterMain>)HttpContext.Session[_CURRENTSHIPVIASESSIONKEY];
                return new List<ShipViaMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTSHIPVIASESSIONKEY] = value;
            }
        }
        private List<TechnicianMasterMain> CurrentTechnicianList
        {
            get
            {
                if (HttpContext.Session[_CURRENTTECHNICIANSESSIONKEY] != null)
                    return (List<TechnicianMasterMain>)HttpContext.Session[_CURRENTTECHNICIANSESSIONKEY];
                return new List<TechnicianMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTTECHNICIANSESSIONKEY] = value;
            }
        }
        private List<ManufacturerMasterMain> CurrentManufacturerList
        {
            get
            {
                if (HttpContext.Session[_CURRENTMANUFACTURERSESSIONKEY] != null)
                    return (List<ManufacturerMasterMain>)HttpContext.Session[_CURRENTMANUFACTURERSESSIONKEY];
                return new List<ManufacturerMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTMANUFACTURERSESSIONKEY] = value;
            }
        }
        private List<MeasurementTermMasterMain> CurrentMeasurementTermList
        {
            get
            {
                if (HttpContext.Session[_CURRENTMEASUREMENTTERMSESSIONKEY] != null)
                    return (List<MeasurementTermMasterMain>)HttpContext.Session[_CURRENTMEASUREMENTTERMSESSIONKEY];
                return new List<MeasurementTermMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTMEASUREMENTTERMSESSIONKEY] = value;
            }
        }
        private List<UnitMasterMain> CurrentUnitList
        {
            get
            {
                if (HttpContext.Session[_CURRENTUNITSSESSIONKEY] != null)
                    return (List<UnitMasterMain>)HttpContext.Session[_CURRENTUNITSSESSIONKEY];
                return new List<UnitMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTUNITSSESSIONKEY] = value;
            }
        }
        private List<SupplierMasterMain> CurrentSupplierList
        {
            get
            {
                if (HttpContext.Session[_CURRENTSUPPLIERSESSIONKEY] != null)
                    return (List<SupplierMasterMain>)HttpContext.Session[_CURRENTSUPPLIERSESSIONKEY];
                return new List<SupplierMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTSUPPLIERSESSIONKEY] = value;
            }
        }
        private List<BOMItemMasterMain> CurrentItemList
        {
            get
            {
                if (HttpContext.Session[_CURRENTITEMSESSIONKEY] != null)
                    return (List<BOMItemMasterMain>)HttpContext.Session[_CURRENTITEMSESSIONKEY];
                return new List<BOMItemMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTITEMSESSIONKEY] = value;
            }
        }
        private List<BinMasterMain> CurrentLocationList
        {
            get
            {
                if (HttpContext.Session[_CURRENTLOCATIONSESSIONKEY] != null)
                    return (List<BinMasterMain>)HttpContext.Session[_CURRENTLOCATIONSESSIONKEY];
                return new List<BinMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTLOCATIONSESSIONKEY] = value;
            }
        }
        private List<ToolCategoryMasterMain> CurrentToolCategoryList
        {
            get
            {
                if (HttpContext.Session[_CURRENTTOOLCATEGORYSESSIONKEY] != null)
                    return (List<ToolCategoryMasterMain>)HttpContext.Session[_CURRENTTOOLCATEGORYSESSIONKEY];
                return new List<ToolCategoryMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTTOOLCATEGORYSESSIONKEY] = value;
            }
        }
        private List<ToolMasterMain> CurrentToolList
        {
            get
            {
                if (HttpContext.Session[_CURRENTTOOLMASTERLIST] != null)
                    return (List<ToolMasterMain>)HttpContext.Session[_CURRENTTOOLMASTERLIST];
                return new List<ToolMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTTOOLMASTERLIST] = value;
            }
        }
        private List<AssetMasterMain> CurrentAssetMasterList
        {
            get
            {
                if (HttpContext.Session[_CURRENTASSETMASTERLIST] != null)
                    return (List<AssetMasterMain>)HttpContext.Session[_CURRENTASSETMASTERLIST];
                return new List<AssetMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTASSETMASTERLIST] = value;
            }
        }
        private List<QuickListItemsMain> CurrentQuickListMasterList
        {
            get
            {
                if (HttpContext.Session[_CURRENTQUICKLISTMASTERLIST] != null)
                    return (List<QuickListItemsMain>)HttpContext.Session[_CURRENTQUICKLISTMASTERLIST];
                return new List<QuickListItemsMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTQUICKLISTMASTERLIST] = value;
            }
        }
        private List<InventoryLocationMain> CurrentInventoryLocationMasterList
        {
            get
            {
                if (HttpContext.Session[_CURRENTINVENTORYLOCATIONMASTERLIST] != null)
                    return (List<InventoryLocationMain>)HttpContext.Session[_CURRENTINVENTORYLOCATIONMASTERLIST];
                return new List<InventoryLocationMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTINVENTORYLOCATIONMASTERLIST] = value;
            }
        }
        private List<InventoryLocationQuantityMain> CurrentInventoryLocationQuantityList
        {
            get
            {
                if (HttpContext.Session[_CURRENTINVENTORYLOCATIONQuantityLIST] != null)
                    return (List<InventoryLocationQuantityMain>)HttpContext.Session[_CURRENTINVENTORYLOCATIONQuantityLIST];
                return new List<InventoryLocationQuantityMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTINVENTORYLOCATIONQuantityLIST] = value;
            }
        }
        private List<BOMItemMasterMain> CurrentBOMItemList
        {
            get
            {
                if (HttpContext.Session[_CURRENTBOMITEMSESSIONKEY] != null)
                    return (List<BOMItemMasterMain>)HttpContext.Session[_CURRENTBOMITEMSESSIONKEY];
                return new List<BOMItemMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTBOMITEMSESSIONKEY] = value;
            }
        }
        private List<KitDetailmain> CurrentKitItemList
        {
            get
            {
                if (HttpContext.Session[_CURRENTKitItemSESSIONKEY] != null)
                    return (List<KitDetailmain>)HttpContext.Session[_CURRENTKitItemSESSIONKEY];
                return new List<KitDetailmain>();
            }
            set
            {
                HttpContext.Session[_CURRENTKitItemSESSIONKEY] = value;
            }
        }
        #endregion

        CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
        public ActionResult ResetSession(string key, string value)
        {
            Session["importedData"] = null;
            Session["CurModulevalue"] = ImportMastersDTO.TableName.AssetMaster;
            Session["CurModule"] = ImportMastersDTO.TableName.AssetMaster;
            return this.Json(new { success = true });
        }
        public ActionResult ImportMasters()
        {
            Session["importedData"] = null;
            Session["CurModulevalue"] = ImportMastersDTO.TableName.AssetMaster;
            Session["CurModule"] = ImportMastersDTO.TableName.AssetMaster;
            return View();
        }

        public List<SelectListItem> GetModule()
        {
            List<SelectListItem> lstItem = null;
            try
            {
                eTurns.DTO.UserWiseRoomsAccessDetailsDTO lstPermission = eTurnsWeb.Helper.SessionHelper.RoomPermissions.Find(element => element.RoomID == eTurnsWeb.Helper.SessionHelper.RoomID);
                List<eTurns.DTO.UserRoleModuleDetailsDTO> objChild = (from m in lstPermission.PermissionList
                                                                      where m.ParentID != null && (m.ParentID == 2 || m.ModuleName.ToLower().Contains("item")) && (!m.ModuleName.Contains("Company") && !m.ModuleName.Contains("Enterprise"))
                                                                      select m).OrderBy(c => c.ParentID).ToList();
                if (objChild != null)
                {
                    if (objChild.Count > 0)
                    {
                        lstItem = new List<SelectListItem>();
                        foreach (var item in objChild)
                        {
                            SelectListItem obj = new SelectListItem();
                            obj.Text = SessionHelper.GetModuleName(Convert.ToInt32(item.ModuleID));
                            obj.Value = item.ModuleID.ToString();
                            lstItem.Add(obj);
                        }

                    }
                }
                return lstItem;
            }
            catch (Exception ex)
            {
                return lstItem;
                throw ex;
            }
            finally
            {
                lstItem = null;
            }

        }
        public List<SelectListItem> GetImportModule()
        {
            List<SelectListItem> lstItem = new List<SelectListItem>();
            SelectListItem obj = new SelectListItem();
            obj.Text = "Categories";
            obj.Value = ((int)SessionHelper.ModuleList.CategoryMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Cost UOM";
            obj.Value = ((int)SessionHelper.ModuleList.CostUOMMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Customers";
            obj.Value = ((int)SessionHelper.ModuleList.CustomerMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Freight Types";
            obj.Value = ((int)SessionHelper.ModuleList.FreightTypeMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "G/L Accounts";
            obj.Value = ((int)SessionHelper.ModuleList.GLAccountsMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Inventory Classification";
            obj.Value = ((int)SessionHelper.ModuleList.InventoryClassificationMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Item Locations Quantity";
            obj.Value = ((int)SessionHelper.ModuleList.BinMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Item Locations";
            obj.Value = ((int)SessionHelper.ModuleList.eVMISetup).ToString();
            lstItem.Add(obj);
            obj = new SelectListItem();
            obj.Text = "Manufacturers";
            obj.Value = ((int)SessionHelper.ModuleList.ManufacturerMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Measurement Terms";
            obj.Value = ((int)SessionHelper.ModuleList.MeasurementTermMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Ship Vias";
            obj.Value = ((int)SessionHelper.ModuleList.ShipViaMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Suppliers";
            obj.Value = ((int)SessionHelper.ModuleList.SupplierMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Technicians";
            obj.Value = ((int)SessionHelper.ModuleList.TechnicianMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Tool Categories";
            obj.Value = ((int)SessionHelper.ModuleList.ToolCategory).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Locations";
            obj.Value = ((int)SessionHelper.ModuleList.LocationMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Units";
            obj.Value = ((int)SessionHelper.ModuleList.UnitMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Items";
            obj.Value = ((int)SessionHelper.ModuleList.ItemMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Assets";
            obj.Value = ((int)SessionHelper.ModuleList.Assets).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Tools";
            obj.Value = ((int)SessionHelper.ModuleList.ToolMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "QuickList";
            obj.Value = ((int)SessionHelper.ModuleList.QuickListPermission).ToString();
            lstItem.Add(obj);

            //obj = new SelectListItem();
            //obj.Text = "Inventory Location";
            //obj.Value = "110";
            //lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "BOM Items";
            obj.Value = ((int)SessionHelper.ModuleList.BOMItemMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = "Kits";
            obj.Value = ((int)SessionHelper.ModuleList.Kits).ToString();
            lstItem.Add(obj);

            return lstItem.OrderBy(c => c.Text).ToList();


        }
        public DataTable GetCSVData(string Fields, string RequiredField, HttpPostedFileBase uploadFile)
        {
            DataTable dtCSV = new DataTable();
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

                    string[] strHeaderArray = headerLine.Replace("* ", "").ToLower().Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string item in DBReqField)
                    {
                        //if (!headerLine.ToLower().Contains(item.ToLower()))
                        //{
                        //    Session["ErrorMessage"] = "Invalid CSV file.";
                        //    return null;
                        //}
                        if (!strHeaderArray.Contains(item.ToLower()))
                        {
                            Session["ErrorMessage"] = "Invalid CSV file.";
                            return null;
                        }
                    }

                    string Datatablecolumns = string.Empty;
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

                    Session["ColuumnList"] = Datatablecolumns;
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
                                foreach (var item in value)
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
            catch (Exception)
            {

            }

            return dtCSV;

        }
        public DataTable GetExcelData(string Fields, string RequiredField, HttpPostedFileBase uploadFile)
        {
            DataTable dtExcel = new DataTable();
            try
            {

                string path = SaveUploadedFile(uploadFile);
                InitializeWorkbook(path);
                dtExcel = ConvertToDataTable(Fields);
                if (dtExcel != null && dtExcel.Rows.Count > 0)
                {
                    DataTable dtExcelCopy = new DataTable();
                    dtExcelCopy = dtExcel.Copy();
                    dtExcelCopy.AcceptChanges();
                    foreach (DataColumn col in dtExcel.Columns)
                    {

                        if (!Fields.ToLower().Contains(col.ToString().ToLower()))
                        {
                            dtExcelCopy.Columns.Remove(col.ToString().ToLower());
                        }

                    }
                    dtExcel = dtExcelCopy;
                }

            }
            catch (Exception)
            {

            }

            return dtExcel;

        }
        HSSFWorkbook hssfworkbook;

        void InitializeWorkbook(string path)
        {
            //read the template via FileStream, it is suggested to use FileAccess.Read to prevent file lock.
            //book1.xls is an Excel-2007-generated file, so some new unknown BIFF records are added. 
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
        }

        DataTable ConvertToDataTable(string Fields)
        {

            var sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            rows.MoveNext();
            DataTable dt = new DataTable();
            var rowHead = (HSSFRow)rows.Current;

            string Datatablecolumns = string.Empty;
            for (int i = 0; i < rowHead.LastCellNum; i++)
            {
                var cell = rowHead.GetCell(i);


                if (cell != null)
                {
                    dt.Columns.Add(cell.ToString());
                }
                //---------------------------------------------
                if (Fields.ToLower().Contains(cell.ToString().ToLower()))

                    if (string.IsNullOrEmpty(Datatablecolumns))
                    {
                        Datatablecolumns = cell.ToString();
                    }
                    else
                    {
                        Datatablecolumns += "," + cell.ToString();
                    }
            }
            Session["ColuumnList"] = Datatablecolumns;
            while (rows.MoveNext())
            {
                var row = (HSSFRow)rows.Current;
                DataRow dr = dt.NewRow();

                for (int i = 0; i < row.LastCellNum; i++)
                {
                    var cell = row.GetCell(i);


                    if (cell == null)
                    {
                        dr[i] = null;
                    }
                    else
                    {
                        dr[i] = cell.ToString();
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }


        public string SaveUploadedFile(HttpPostedFileBase uploadFile)
        {
            string path = string.Empty;
            // Verify that the user selected a file
            if (uploadFile != null && uploadFile.ContentLength > 0)
            {
                // extract only the fielname
                string fileName = Path.GetFileName(uploadFile.FileName);
                // store the file inside ~/App_Data/uploads folder
                string[] strfilename = fileName.Split('.');
                if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
                {
                    path = Path.Combine(Server.MapPath("~/Uploads/Import/CSV"), fileName);
                }
                else if (strfilename[strfilename.Length - 1].ToUpper() == "XLS")
                {
                    path = Path.Combine(Server.MapPath("~/Uploads/Import/Excel"), fileName);
                }
                //Excel
                uploadFile.SaveAs(path);
            }
            return path;
        }
        #region Get Import

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ImportMasters(HttpPostedFileBase uploadFile, HttpPostedFileBase uploadZIPFile)
        {

            Session["importedData"] = null;
            StringBuilder strValidations = new StringBuilder(string.Empty);

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
                    vDefaultSupplier = objSuppDTO.SupplierName;
                }
                if (objRoomDTO.DefaultBinID != null & objRoomDTO.DefaultBinID > 0)
                {
                    BinMasterDAL objBinDal = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    BinMasterDTO objBinDTO = new BinMasterDTO();
                    objBinDTO = objBinDal.GetRecord(objRoomDTO.DefaultBinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    vDefaultLocation = objBinDTO.BinNumber;
                }

                UnitMasterDAL objUnitDal = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
                try
                {
                    UnitMasterDTO objUnit = objUnitDal.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(t => t.Unit == "EA").FirstOrDefault();
                    vDefaultUOM = objUnit.Unit;

                }
                catch { }

            }


            try
            {

                if (uploadFile.ContentLength > 0)
                {
                    DataTable dtCSV = new DataTable();
                    //
                    //  DataTable dtExcel = new DataTable();
                    string fileName = Path.GetFileName(uploadFile.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    string[] strfilename = fileName.Split('.');

                    if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
                    {
                        dtCSV = GetCSVData(GetDBFields(Session["CurModule"].ToString()), GetRequiredDBFields(Session["CurModule"].ToString()), uploadFile);
                    }
                    // TO Do : To start Excell import just uncommnet the below if block
                    //else if (strfilename[strfilename.Length - 1].ToUpper() == "XLS")
                    //{
                    //    dtCSV = GetExcelData(GetDBFields(Session["CurModule"].ToString()), GetRequiredDBFields(Session["CurModule"].ToString()), uploadFile);
                    //}


                    #region "comment"

                    ////return null;
                    ////string[] value = { "" };
                    ////DataTable dt = new DataTable();
                    ////DataRow row;

                    ////if (Session["CurModule"] != null)
                    ////{
                    ////    #region Inventry Locations
                    ////    if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.BinMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportBinColumn item in Enum.GetValues(typeof(CommonUtility.ImportBinColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region Category Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.CategoryMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportCategoryColumn item in Enum.GetValues(typeof(CommonUtility.ImportCategoryColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region Customer Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.CustomerMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportCustomerColumn item in Enum.GetValues(typeof(CommonUtility.ImportCustomerColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region Freight Type Locations
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.FreightTypeMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportFreightTypeColumn item in Enum.GetValues(typeof(CommonUtility.ImportFreightTypeColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region G/L Account Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.GLAccountMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportGLAccountColumn item in Enum.GetValues(typeof(CommonUtility.ImportGLAccountColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region GXPR Consigned Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.GXPRConsigmentJobMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportGXPRConsignedColumn item in Enum.GetValues(typeof(CommonUtility.ImportGXPRConsignedColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region Job Type Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.JobTypeMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportJobTypeColumn item in Enum.GetValues(typeof(CommonUtility.ImportJobTypeColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region Ship Via Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ShipViaMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportShipViaColumn item in Enum.GetValues(typeof(CommonUtility.ImportShipViaColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region Technician Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.TechnicianMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportTechnicianColumn item in Enum.GetValues(typeof(CommonUtility.ImportTechnicianColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region Manufacturer Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ManufacturerMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportManufacturerColumn item in Enum.GetValues(typeof(CommonUtility.ImportManufacturerColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region MeasurementTerm Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.MeasurementTermMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportMeasurementTermColumn item in Enum.GetValues(typeof(CommonUtility.ImportMeasurementTermColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region Unit Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.UnitMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportUnitsColumn item in Enum.GetValues(typeof(CommonUtility.ImportUnitsColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region Supplier Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.SupplierMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportSupplierColumn item in Enum.GetValues(typeof(CommonUtility.ImportSupplierColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region Item Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ItemMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportItemColumn item in Enum.GetValues(typeof(CommonUtility.ImportItemColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region Locations
                    ////    if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.LocationMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportLocationColumn item in Enum.GetValues(typeof(CommonUtility.ImportLocationColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region Tool Category
                    ////    if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ToolCategoryMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportToolCategoryColumn item in Enum.GetValues(typeof(CommonUtility.ImportToolCategoryColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region CostUOM Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.CostUOMMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportCostUOMColumn item in Enum.GetValues(typeof(CommonUtility.ImportCostUOMColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////    #region Inventory Classification Master
                    ////    else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.InventoryClassificationMaster.ToString())
                    ////    {
                    ////        foreach (CommonUtility.ImportInventoryClassificationColumn item in Enum.GetValues(typeof(CommonUtility.ImportInventoryClassificationColumn)))
                    ////        {
                    ////            DataColumn dc = new DataColumn();
                    ////            dc.ColumnName = item.ToString();
                    ////            dt.Columns.Add(dc);
                    ////        }
                    ////    }
                    ////    #endregion
                    ////}

                    ////Stream objstr = uploadFile.InputStream;
                    ////StreamReader sr = new StreamReader(objstr);
                    ////objstr.Position = 0;
                    ////sr.DiscardBufferedData();
                    ////string separator = ConfigurationManager.AppSettings["CSVseparator"].ToString();
                    ////string headerLine = sr.ReadLine();

                    ////string[] strHeaderArray = headerLine.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                    ////Session["ErrorMessage"] = null;
                    ////if (strHeaderArray.Length != dt.Columns.Count)
                    ////{
                    ////    Session["ErrorMessage"] = "Invalid CSV file.";
                    ////    return View();
                    ////}
                    ////else
                    ////{
                    ////    for (int i = 0; i < strHeaderArray.Length; i++)
                    ////    {
                    ////        if (dt.Columns[i].ColumnName.ToLower() != strHeaderArray[i].ToLower().ToString())
                    ////        {
                    ////            Session["ErrorMessage"] = "Invalid CSV file.";
                    ////            return View();
                    ////        }
                    ////    }
                    ////}
                    ////using (TextFieldParser parser = new TextFieldParser(sr))
                    ////{
                    ////    parser.Delimiters = new string[] { separator };
                    ////    while (true)
                    ////    {
                    ////        value = parser.ReadFields();
                    ////        if (value == null)
                    ////        {
                    ////            break;
                    ////        }
                    ////        else
                    ////        {
                    ////            row = dt.NewRow();
                    ////            row.ItemArray = value;
                    ////            dt.Rows.Add(row);
                    ////        }
                    ////        //Console.WriteLine("{0} field(s)", parts.Length);
                    ////    }
                    ////}

                    //////StreamReader sr = new StreamReader(objstr);
                    //////objstr.Position = 0;
                    //////sr.DiscardBufferedData();
                    //////string headerLine = sr.ReadLine();                    
                    //////char separator = Convert.ToChar(ConfigurationManager.AppSettings["CSVseparator"].ToString());
                    //////while (!sr.EndOfStream)
                    //////{

                    //////    value = sr.ReadLine().Split(separator);

                    //////    row = dt.NewRow();
                    //////    row.ItemArray = value;
                    //////    dt.Rows.Add(row);
                    //////}
                    #endregion
                    if (dtCSV.Rows.Count > 0)
                    {
                        List<DataRow> list = dtCSV.AsEnumerable().ToList();

                        if (Session["CurModule"] != null)
                        {
                            #region Inventry Locations
                            //if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.BinMaster.ToString())
                            //{
                            //    List<BinMasterMain> lstImport = new List<BinMasterMain>();
                            //    foreach (DataRow item in list)
                            //    {
                            //        BinMasterMain obj = new BinMasterMain();
                            //        try
                            //        {
                            //            //bool AllowInsert = true;
                            //            if (item[CommonUtility.ImportBinColumn.BinNumber.ToString()].ToString() != "")
                            //            // AllowInsert = lstImport.Where(x => x.BinNumber == item[CommonUtility.ImportBinColumn.BinNumber.ToString()].ToString()).ToList().Count > 0 ? false : true;
                            //            //if (AllowInsert == true)
                            //            {
                            //                //obj.ID = Convert.ToInt32(item[CommonUtility.ImportBinColumn.ID.ToString()].ToString());
                            //                if (item.Table.Columns.Contains(CommonUtility.ImportBinColumn.BinNumber.ToString()))
                            //                {
                            //                    obj.BinNumber = item[CommonUtility.ImportBinColumn.BinNumber.ToString()].ToString();
                            //                }

                            //                if (item.Table.Columns.Contains(CommonUtility.ImportBinColumn.UDF1.ToString()))
                            //                {
                            //                    obj.UDF1 = item[CommonUtility.ImportBinColumn.UDF1.ToString()].ToString();
                            //                }

                            //                if (item.Table.Columns.Contains(CommonUtility.ImportBinColumn.UDF2.ToString()))
                            //                {
                            //                    obj.UDF2 = item[CommonUtility.ImportBinColumn.UDF2.ToString()].ToString();
                            //                }

                            //                if (item.Table.Columns.Contains(CommonUtility.ImportBinColumn.UDF3.ToString()))
                            //                {
                            //                    obj.UDF3 = item[CommonUtility.ImportBinColumn.UDF3.ToString()].ToString();
                            //                }

                            //                if (item.Table.Columns.Contains(CommonUtility.ImportBinColumn.UDF4.ToString()))
                            //                {
                            //                    obj.UDF4 = item[CommonUtility.ImportBinColumn.UDF4.ToString()].ToString();
                            //                }

                            //                if (item.Table.Columns.Contains(CommonUtility.ImportBinColumn.UDF5.ToString()))
                            //                {
                            //                    obj.UDF5 = item[CommonUtility.ImportBinColumn.UDF5.ToString()].ToString();
                            //                }
                            //                obj.Status = "N/A";
                            //                obj.Reason = "N/A";
                            //                //obj.UDF6 = item[CommonUtility.ImportBinColumn.UDF6.ToString()].ToString();
                            //                //obj.UDF7 = item[CommonUtility.ImportBinColumn.UDF7.ToString()].ToString();
                            //                //obj.UDF8 = item[CommonUtility.ImportBinColumn.UDF8.ToString()].ToString();
                            //                //obj.UDF9 = item[CommonUtility.ImportBinColumn.UDF9.ToString()].ToString();
                            //                //obj.UDF10 = item[CommonUtility.ImportBinColumn.UDF10.ToString()].ToString();

                            //                lstImport.Add(obj);
                            //            }
                            //        }
                            //        catch { }
                            //    }

                            //    Session["importedData"] = lstImport;
                            //}
                            #endregion
                            #region InventoryLocation master
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.BinMaster.ToString())
                            {
                                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                                List<InventoryLocationMain> lstImport = new List<InventoryLocationMain>();
                                foreach (DataRow item in list)
                                {
                                    InventoryLocationMain obj = new InventoryLocationMain();
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ImportInventoryLocationColumn.locationname.ToString()].ToString() != "" && item[CommonUtility.ImportInventoryLocationColumn.ItemNumber.ToString()].ToString() != "")
                                        //  AllowInsert = lstImport.Where(x => x.InventoryClassification == item[CommonUtility.ImportInventoryClassificationColumn.InventoryClassification.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryClassificationColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.locationname.ToString()))
                                            {
                                                obj.BinNumber = item[CommonUtility.ImportInventoryLocationColumn.locationname.ToString()].ToString();
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
                                                    obj.consignedquantity = Convert.ToDouble(item[CommonUtility.ImportInventoryLocationColumn.ConsignedQuantity.ToString()].ToString());
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
                                                    obj.customerownedquantity = Convert.ToDouble(item[CommonUtility.ImportInventoryLocationColumn.CustomerOwnedQuantity.ToString()].ToString());
                                                }

                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.Cost.ToString()))
                                            {
                                                if (item[CommonUtility.ImportInventoryLocationColumn.Cost.ToString()] == null || Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.Cost.ToString()]) == "")
                                                {
                                                    obj.Cost = 0;
                                                }
                                                else
                                                {
                                                    obj.Cost = Convert.ToDouble(item[CommonUtility.ImportInventoryLocationColumn.Cost.ToString()].ToString());
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
                                                obj.Expiration = item[CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()].ToString();
                                                obj.displayExpiration = item[CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ReceivedDate.ToString()))
                                            {
                                                //obj.Received = item[CommonUtility.ImportInventoryLocationColumn.ReceivedDate.ToString()].ToString();
                                            }
                                            if (obj.ItemNumber != objItemMasterDTO.ItemNumber)
                                            {
                                                objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetRecordByItemNumberLight(obj.ItemNumber, SessionHelper.RoomID, SessionHelper.CompanyID);
                                            }

                                            if (objItemMasterDTO.SerialNumberTracking && objItemMasterDTO.DateCodeTracking)
                                            {
                                                obj.LotNumber = string.Empty;
                                                if (obj.customerownedquantity != 1 && !objItemMasterDTO.Consignment)
                                                {
                                                    obj.customerownedquantity = 1;
                                                }
                                                if (obj.consignedquantity != 1 && objItemMasterDTO.Consignment)
                                                {
                                                    obj.consignedquantity = 1;
                                                }
                                            }
                                            else if (objItemMasterDTO.LotNumberTracking && objItemMasterDTO.DateCodeTracking)
                                            {
                                                obj.SerialNumber = string.Empty;
                                            }
                                            else if (objItemMasterDTO.LotNumberTracking)
                                            {
                                                obj.SerialNumber = string.Empty;
                                            }
                                            else if (objItemMasterDTO.SerialNumberTracking)
                                            {
                                                obj.LotNumber = string.Empty;
                                                if (obj.customerownedquantity != 1 && !objItemMasterDTO.Consignment)
                                                {
                                                    obj.customerownedquantity = 1;
                                                }
                                                if (obj.consignedquantity != 1 && objItemMasterDTO.Consignment)
                                                {
                                                    obj.consignedquantity = 1;
                                                }
                                            }
                                            else if (objItemMasterDTO.DateCodeTracking)
                                            {
                                                obj.LotNumber = string.Empty;
                                                obj.SerialNumber = string.Empty;
                                            }
                                            else
                                            {
                                                obj.LotNumber = string.Empty;
                                                obj.SerialNumber = string.Empty;
                                            }
                                            InventoryLocationMain oCheckExist = lstImport.Where(x => x.ItemNumber == obj.ItemNumber && x.BinNumber == obj.BinNumber && x.SerialNumber == obj.SerialNumber && x.LotNumber == obj.LotNumber).FirstOrDefault();

                                            if (oCheckExist != null)
                                            {
                                                lstImport.Remove(oCheckExist);
                                            }

                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region InventoryLocationQuantity master
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ItemLocationeVMISetup.ToString())
                            {
                                List<InventoryLocationQuantityMain> lstImport = new List<InventoryLocationQuantityMain>();
                                foreach (DataRow item in list)
                                {
                                    InventoryLocationQuantityMain obj = new InventoryLocationQuantityMain();
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ImportInventoryLocationColumn.locationname.ToString()].ToString() != "" && item[CommonUtility.ImportInventoryLocationColumn.ItemNumber.ToString()].ToString() != "")
                                        //  AllowInsert = lstImport.Where(x => x.InventoryClassification == item[CommonUtility.ImportInventoryClassificationColumn.InventoryClassification.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryClassificationColumn.ID.ToString()].ToString());
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
                                                    obj.SensorId = Convert.ToDouble(item[CommonUtility.ImportInventoryLocationColumn.SensorId.ToString()]);
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
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region Category Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.CategoryMaster.ToString())
                            {
                                List<CategoryMasterMain> lstImport = new List<CategoryMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    CategoryMasterMain obj = new CategoryMasterMain();
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ImportCategoryColumn.Category.ToString()].ToString() != "")
                                        // AllowInsert = lstImport.Where(x => x.Category == item[CommonUtility.ImportCategoryColumn.Category.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //if (string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportCategoryColumn.ID.ToString()])))
                                            //{
                                            //    obj.ID = 0;
                                            //}
                                            //else {
                                            //    obj.ID = Convert.ToInt32(Convert.ToString(item[CommonUtility.ImportCategoryColumn.ID.ToString()]));
                                            //}

                                            obj.Category = item[CommonUtility.ImportCategoryColumn.Category.ToString()].ToString();

                                            if (item.Table.Columns.Contains(CommonUtility.ImportCategoryColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportCategoryColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportCategoryColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportCategoryColumn.UDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportCategoryColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportCategoryColumn.UDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportCategoryColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportCategoryColumn.UDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportCategoryColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportCategoryColumn.UDF5.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.UDF6 = item[CommonUtility.ImportCategoryColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportCategoryColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportCategoryColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportCategoryColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportCategoryColumn.UDF10.ToString()].ToString();
                                            //obj.CategoryColor = item[CommonUtility.ImportCategoryColumn.CategoryColor.ToString()].ToString();
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region Customer Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.CustomerMaster.ToString())
                            {
                                List<CustomerMasterMain> lstImport = new List<CustomerMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    CustomerMasterMain obj = new CustomerMasterMain();
                                    try
                                    {
                                        // bool AllowInsert = true;
                                        if (item[CommonUtility.ImportCustomerColumn.Customer.ToString()].ToString() != "")
                                        // AllowInsert = lstImport.Where(x => x.Customer == item[CommonUtility.ImportCustomerColumn.Customer.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //obj.ID = Convert.ToInt32(item[CommonUtility.ImportCustomerColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.Customer.ToString()))
                                            {
                                                obj.Customer = item[CommonUtility.ImportCustomerColumn.Customer.ToString()].ToString();
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
                                                obj.UDF1 = item[CommonUtility.ImportCustomerColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportCustomerColumn.UDF2.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportCustomerColumn.UDF3.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportCustomerColumn.UDF4.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportCustomerColumn.UDF5.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportCustomerColumn.Remarks.ToString()))
                                            {
                                                obj.Remarks = item[CommonUtility.ImportCustomerColumn.Remarks.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.UDF6 = item[CommonUtility.ImportCustomerColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportCustomerColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportCustomerColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportCustomerColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportCustomerColumn.UDF10.ToString()].ToString();
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region Freight Type Locations
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.FreightTypeMaster.ToString())
                            {
                                List<FreightTypeMasterMain> lstImport = new List<FreightTypeMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    FreightTypeMasterMain obj = new FreightTypeMasterMain();
                                    try
                                    {
                                        // bool AllowInsert = true;
                                        if (item[CommonUtility.ImportFreightTypeColumn.FreightType.ToString()].ToString() != "")
                                        //   AllowInsert = lstImport.Where(x => x.FreightType == item[CommonUtility.ImportFreightTypeColumn.FreightType.ToString()].ToString()).ToList().Count > 0 ? false : true;
                                        //if (AllowInsert == true)
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportFreightTypeColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportFreightTypeColumn.FreightType.ToString()))
                                            {
                                                obj.FreightType = item[CommonUtility.ImportFreightTypeColumn.FreightType.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportFreightTypeColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportFreightTypeColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportFreightTypeColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportFreightTypeColumn.UDF2.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportFreightTypeColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportFreightTypeColumn.UDF3.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportFreightTypeColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportFreightTypeColumn.UDF4.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportFreightTypeColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportFreightTypeColumn.UDF5.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.UDF6 = item[CommonUtility.ImportFreightTypeColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportFreightTypeColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportFreightTypeColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportFreightTypeColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportFreightTypeColumn.UDF10.ToString()].ToString();

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region  G/L Account Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.GLAccountMaster.ToString())
                            {
                                List<GLAccountMasterMain> lstImport = new List<GLAccountMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    GLAccountMasterMain obj = new GLAccountMasterMain();
                                    try
                                    {
                                        // bool AllowInsert = true;
                                        if (item[CommonUtility.ImportGLAccountColumn.GLAccount.ToString()].ToString() != "")
                                        // AllowInsert = lstImport.Where(x => x.GLAccount == item[CommonUtility.ImportGLAccountColumn.GLAccount.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //obj.ID = Convert.ToInt32(item[CommonUtility.ImportGLAccountColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportGLAccountColumn.GLAccount.ToString()))
                                            {
                                                obj.GLAccount = item[CommonUtility.ImportGLAccountColumn.GLAccount.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportGLAccountColumn.Description.ToString()))
                                            {
                                                obj.Description = item[CommonUtility.ImportGLAccountColumn.Description.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportGLAccountColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportGLAccountColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportGLAccountColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportGLAccountColumn.UDF2.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportGLAccountColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportGLAccountColumn.UDF3.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportGLAccountColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportGLAccountColumn.UDF4.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportGLAccountColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportGLAccountColumn.UDF5.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.UDF6 = item[CommonUtility.ImportGLAccountColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportGLAccountColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportGLAccountColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportGLAccountColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportGLAccountColumn.UDF10.ToString()].ToString();

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region  GXPR Consigned Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.GXPRConsigmentJobMaster.ToString())
                            {
                                List<GXPRConsignedMasterMain> lstImport = new List<GXPRConsignedMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    GXPRConsignedMasterMain obj = new GXPRConsignedMasterMain();
                                    try
                                    {
                                        bool AllowInsert = true;
                                        if (item[CommonUtility.ImportGXPRConsignedColumn.GXPRConsigmentJob.ToString()].ToString() != "")
                                            AllowInsert = lstImport.Where(x => x.GXPRConsigmentJob == item[CommonUtility.ImportGXPRConsignedColumn.GXPRConsigmentJob.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        if (AllowInsert == true)
                                        {
                                            obj.ID = Convert.ToInt32(item[CommonUtility.ImportGXPRConsignedColumn.ID.ToString()].ToString());
                                            obj.GXPRConsigmentJob = item[CommonUtility.ImportGXPRConsignedColumn.GXPRConsigmentJob.ToString()].ToString();
                                            obj.UDF1 = item[CommonUtility.ImportGXPRConsignedColumn.UDF1.ToString()].ToString();
                                            obj.UDF2 = item[CommonUtility.ImportGXPRConsignedColumn.UDF2.ToString()].ToString();
                                            obj.UDF3 = item[CommonUtility.ImportGXPRConsignedColumn.UDF3.ToString()].ToString();
                                            obj.UDF4 = item[CommonUtility.ImportGXPRConsignedColumn.UDF4.ToString()].ToString();
                                            obj.UDF5 = item[CommonUtility.ImportGXPRConsignedColumn.UDF5.ToString()].ToString();
                                            //obj.UDF6 = item[CommonUtility.ImportGXPRConsignedColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportGXPRConsignedColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportGXPRConsignedColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportGXPRConsignedColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportGXPRConsignedColumn.UDF10.ToString()].ToString();

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region  Job Type Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.JobTypeMaster.ToString())
                            {
                                List<JobTypeMasterMain> lstImport = new List<JobTypeMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    JobTypeMasterMain obj = new JobTypeMasterMain();
                                    try
                                    {
                                        bool AllowInsert = true;
                                        if (item[CommonUtility.ImportJobTypeColumn.JobType.ToString()].ToString() != "")
                                            AllowInsert = lstImport.Where(x => x.JobType == item[CommonUtility.ImportJobTypeColumn.JobType.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        if (AllowInsert == true)
                                        {
                                            obj.ID = Convert.ToInt32(item[CommonUtility.ImportJobTypeColumn.ID.ToString()].ToString());
                                            obj.JobType = item[CommonUtility.ImportJobTypeColumn.JobType.ToString()].ToString();
                                            obj.UDF1 = item[CommonUtility.ImportJobTypeColumn.UDF1.ToString()].ToString();
                                            obj.UDF2 = item[CommonUtility.ImportJobTypeColumn.UDF2.ToString()].ToString();
                                            obj.UDF3 = item[CommonUtility.ImportJobTypeColumn.UDF3.ToString()].ToString();
                                            obj.UDF4 = item[CommonUtility.ImportJobTypeColumn.UDF4.ToString()].ToString();
                                            obj.UDF5 = item[CommonUtility.ImportJobTypeColumn.UDF5.ToString()].ToString();
                                            //obj.UDF6 = item[CommonUtility.ImportJobTypeColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportJobTypeColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportJobTypeColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportJobTypeColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportJobTypeColumn.UDF10.ToString()].ToString();

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region  Ship Via Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ShipViaMaster.ToString())
                            {
                                List<ShipViaMasterMain> lstImport = new List<ShipViaMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    ShipViaMasterMain obj = new ShipViaMasterMain();
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ImportShipViaColumn.ShipVia.ToString()].ToString() != "")
                                        //  AllowInsert = lstImport.Where(x => x.ShipVia == item[CommonUtility.ImportShipViaColumn.ShipVia.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //obj.ID = Convert.ToInt32(item[CommonUtility.ImportShipViaColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportShipViaColumn.ShipVia.ToString()))
                                            {
                                                obj.ShipVia = item[CommonUtility.ImportShipViaColumn.ShipVia.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportShipViaColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportShipViaColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportShipViaColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportShipViaColumn.UDF2.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportShipViaColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportShipViaColumn.UDF3.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportShipViaColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportShipViaColumn.UDF4.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportShipViaColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportShipViaColumn.UDF5.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.UDF6 = item[CommonUtility.ImportShipViaColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportShipViaColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportShipViaColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportShipViaColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportShipViaColumn.UDF10.ToString()].ToString();

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region  Technicians Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.TechnicianMaster.ToString())
                            {
                                List<TechnicianMasterMain> lstImport = new List<TechnicianMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    TechnicianMasterMain obj = new TechnicianMasterMain();
                                    try
                                    {
                                        // bool AllowInsert = true;
                                        if (item[CommonUtility.ImportTechnicianColumn.Technician.ToString()].ToString() != "")
                                        //   AllowInsert = lstImport.Where(x => x.Technician == item[CommonUtility.ImportTechnicianColumn.Technician.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportTechnicianColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportTechnicianColumn.Technician.ToString()))
                                            {
                                                obj.Technician = item[CommonUtility.ImportTechnicianColumn.Technician.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportTechnicianColumn.TechnicianCode.ToString()))
                                            {
                                                obj.TechnicianCode = item[CommonUtility.ImportTechnicianColumn.TechnicianCode.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportTechnicianColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportTechnicianColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportTechnicianColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportTechnicianColumn.UDF2.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportTechnicianColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportTechnicianColumn.UDF3.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportTechnicianColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportTechnicianColumn.UDF4.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportTechnicianColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportTechnicianColumn.UDF5.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.UDF6 = item[CommonUtility.ImportTechnicianColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportTechnicianColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportTechnicianColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportTechnicianColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportTechnicianColumn.UDF10.ToString()].ToString();

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region  Manufacturer Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ManufacturerMaster.ToString())
                            {
                                List<ManufacturerMasterMain> lstImport = new List<ManufacturerMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    ManufacturerMasterMain obj = new ManufacturerMasterMain();
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ImportManufacturerColumn.Manufacturer.ToString()].ToString() != "")
                                        //  AllowInsert = lstImport.Where(x => x.Manufacturer == item[CommonUtility.ImportManufacturerColumn.Manufacturer.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportManufacturerColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportManufacturerColumn.Manufacturer.ToString()))
                                            {
                                                obj.Manufacturer = item[CommonUtility.ImportManufacturerColumn.Manufacturer.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportManufacturerColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportManufacturerColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportManufacturerColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportManufacturerColumn.UDF2.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportManufacturerColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportManufacturerColumn.UDF3.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportManufacturerColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportManufacturerColumn.UDF4.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportManufacturerColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportManufacturerColumn.UDF5.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.UDF6 = item[CommonUtility.ImportManufacturerColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportManufacturerColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportManufacturerColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportManufacturerColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportManufacturerColumn.UDF10.ToString()].ToString();

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region  MeasurementTerm Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.MeasurementTermMaster.ToString())
                            {
                                List<MeasurementTermMasterMain> lstImport = new List<MeasurementTermMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    MeasurementTermMasterMain obj = new MeasurementTermMasterMain();
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ImportMeasurementTermColumn.MeasurementTerm.ToString()].ToString() != "")
                                        //    AllowInsert = lstImport.Where(x => x.MeasurementTerm == item[CommonUtility.ImportMeasurementTermColumn.MeasurementTerm.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //obj.ID = Convert.ToInt32(item[CommonUtility.ImportMeasurementTermColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportMeasurementTermColumn.MeasurementTerm.ToString()))
                                            {
                                                obj.MeasurementTerm = item[CommonUtility.ImportMeasurementTermColumn.MeasurementTerm.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportMeasurementTermColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportMeasurementTermColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportMeasurementTermColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportMeasurementTermColumn.UDF2.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportMeasurementTermColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportMeasurementTermColumn.UDF3.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportMeasurementTermColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportMeasurementTermColumn.UDF4.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportMeasurementTermColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportMeasurementTermColumn.UDF5.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.UDF6 = item[CommonUtility.ImportMeasurementTermColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportMeasurementTermColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportMeasurementTermColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportMeasurementTermColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportMeasurementTermColumn.UDF10.ToString()].ToString();

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region  Unit Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.UnitMaster.ToString())
                            {
                                List<UnitMasterMain> lstImport = new List<UnitMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    UnitMasterMain obj = new UnitMasterMain();
                                    try
                                    {
                                        // bool AllowInsert = true;
                                        if (item[CommonUtility.ImportUnitsColumn.Unit.ToString()].ToString() != "")
                                        //   AllowInsert = lstImport.Where(x => x.Unit == item[CommonUtility.ImportUnitsColumn.Unit.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //obj.ID = Convert.ToInt32(item[CommonUtility.ImportUnitsColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.Unit.ToString()))
                                            {
                                                obj.Unit = item[CommonUtility.ImportUnitsColumn.Unit.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.Description.ToString()))
                                            {
                                                obj.Description = item[CommonUtility.ImportUnitsColumn.Description.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.Odometer.ToString()))
                                            {
                                                obj.Odometer = item[CommonUtility.ImportUnitsColumn.Odometer.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.OpHours.ToString()))
                                            {
                                                decimal opthous;
                                                decimal.TryParse(item[CommonUtility.ImportUnitsColumn.OpHours.ToString()].ToString(), out opthous);
                                                obj.OpHours = item[CommonUtility.ImportUnitsColumn.OpHours.ToString()].ToString() == "" ? (decimal?)null : opthous;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.SerialNo.ToString()))
                                            {
                                                Int64 SerialNo;
                                                Int64.TryParse(item[CommonUtility.ImportUnitsColumn.SerialNo.ToString()].ToString(), out SerialNo);
                                                obj.SerialNo = item[CommonUtility.ImportUnitsColumn.SerialNo.ToString()].ToString() == "" ? (long?)null : SerialNo;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.Year.ToString()))
                                            {
                                                Int64 Year;
                                                Int64.TryParse(item[CommonUtility.ImportUnitsColumn.Year.ToString()].ToString(), out Year);
                                                obj.Year = item[CommonUtility.ImportUnitsColumn.Year.ToString()].ToString() == "" ? (long?)null : Year;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.Make.ToString()))
                                            {
                                                obj.Make = item[CommonUtility.ImportUnitsColumn.Make.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.Model.ToString()))
                                            {
                                                obj.Model = item[CommonUtility.ImportUnitsColumn.Model.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.Plate.ToString()))
                                            {
                                                obj.Plate = item[CommonUtility.ImportUnitsColumn.Plate.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.EngineModel.ToString()))
                                            {
                                                obj.EngineModel = item[CommonUtility.ImportUnitsColumn.EngineModel.ToString()].ToString(); ;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.EngineSerialNo.ToString()))
                                            {
                                                obj.EngineSerialNo = item[CommonUtility.ImportUnitsColumn.EngineSerialNo.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.MarkupParts.ToString()))
                                            {
                                                Int64 MarkupParts;
                                                Int64.TryParse(item[CommonUtility.ImportUnitsColumn.MarkupParts.ToString()].ToString(), out MarkupParts);
                                                obj.MarkupParts = item[CommonUtility.ImportUnitsColumn.MarkupParts.ToString()].ToString() == "" ? (long?)null : MarkupParts;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.MarkupLabour.ToString()))
                                            {
                                                Int64 MarkupLabour;
                                                Int64.TryParse(item[CommonUtility.ImportUnitsColumn.MarkupLabour.ToString()].ToString(), out MarkupLabour);
                                                obj.MarkupLabour = item[CommonUtility.ImportUnitsColumn.MarkupLabour.ToString()].ToString() == "" ? (long?)null : MarkupLabour;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportUnitsColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportUnitsColumn.UDF2.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportUnitsColumn.UDF3.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportUnitsColumn.UDF4.ToString()].ToString();

                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportUnitsColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportUnitsColumn.UDF5.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.UDF6 = item[CommonUtility.ImportUnitsColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportUnitsColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportUnitsColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportUnitsColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportUnitsColumn.UDF10.ToString()].ToString();

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region  Supplier Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.SupplierMaster.ToString())
                            {
                                List<SupplierMasterMain> lstImport = new List<SupplierMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    SupplierMasterMain obj = new SupplierMasterMain();
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ImportSupplierColumn.SupplierName.ToString()].ToString() != "")
                                        //  AllowInsert = lstImport.Where(x => x.SupplierName == item[CommonUtility.ImportSupplierColumn.SupplierName.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportSupplierColumn.ID.ToString()].ToString());
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

                                                    //decimal MaximumOrderSize;
                                                    //decimal.TryParse(item[CommonUtility.ImportSupplierColumn.MaximumOrderSize.ToString()].ToString(), out MaximumOrderSize);
                                                    //obj.MaximumOrderSize = MaximumOrderSize;
                                                }
                                                else
                                                    obj.MaximumOrderSize = null;
                                            }

                                            //obj.AccountNo = item[CommonUtility.ImportSupplierColumn.AccountNo.ToString()].ToString();
                                            //if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.ReceiverID.ToString()))
                                            //{
                                            //    obj.ReceiverID = item[CommonUtility.ImportSupplierColumn.ReceiverID.ToString()].ToString();
                                            //}

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

                                            //if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.Email.ToString()))
                                            //{
                                            //    obj.Email = item[CommonUtility.ImportSupplierColumn.Email.ToString()].ToString();
                                            //}

                                            //if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.IsEmailPOInBody.ToString()))
                                            //{
                                            //    bool IsEmailPOInBody = false;
                                            //    bool.TryParse(item[CommonUtility.ImportSupplierColumn.IsEmailPOInBody.ToString()].ToString(), out IsEmailPOInBody);
                                            //    obj.IsEmailPOInBody = IsEmailPOInBody;
                                            //}

                                            //if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.IsEmailPOInPDF.ToString()))
                                            //{
                                            //    bool IsEmailPOInPDF = false;
                                            //    bool.TryParse(item[CommonUtility.ImportSupplierColumn.IsEmailPOInPDF.ToString()].ToString(), out IsEmailPOInPDF);
                                            //    obj.IsEmailPOInPDF = IsEmailPOInPDF;
                                            //}

                                            //if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.IsEmailPOInCSV.ToString()))
                                            //{
                                            //    bool IsEmailPOInCSV = false;
                                            //    bool.TryParse(item[CommonUtility.ImportSupplierColumn.IsEmailPOInCSV.ToString()].ToString(), out IsEmailPOInCSV);
                                            //    obj.IsEmailPOInCSV = IsEmailPOInCSV;
                                            //}

                                            //if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.IsEmailPOInX12.ToString()))
                                            //{
                                            //    bool IsEmailPOInX12 = false;
                                            //    bool.TryParse(item[CommonUtility.ImportSupplierColumn.IsEmailPOInX12.ToString()].ToString(), out IsEmailPOInX12);
                                            //    obj.IsEmailPOInX12 = IsEmailPOInX12;
                                            //}

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

                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.UDF6 = item[CommonUtility.ImportSupplierColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportSupplierColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportSupplierColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportSupplierColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportSupplierColumn.UDF10.ToString()].ToString();

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region  Item Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ItemMaster.ToString())
                            {
                                List<BOMItemMasterMain> lstImport = new List<BOMItemMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    BOMItemMasterMain obj = new BOMItemMasterMain();
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
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.DefaultReorderQuantity.ToString()]), out DefaultReorderQuantity);
                                                obj.DefaultReorderQuantity = DefaultReorderQuantity;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.DefaultPullQuantity.ToString()))
                                            {
                                                double DefaultPullQuantity;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.DefaultPullQuantity.ToString()]), out DefaultPullQuantity);
                                                obj.DefaultPullQuantity = DefaultPullQuantity;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Cost.ToString()))
                                            {
                                                double Cost;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Cost.ToString()]), out Cost);
                                                obj.Cost = Cost;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Markup.ToString()))
                                            {
                                                double Markup;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Markup.ToString()]), out Markup);
                                                obj.Markup = Markup;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SellPrice.ToString()))
                                            {
                                                double SellPrice;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.SellPrice.ToString()]), out SellPrice);
                                                obj.SellPrice = SellPrice;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ExtendedCost.ToString()))
                                            {
                                                double ExtendedCost;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.ExtendedCost.ToString()]), out ExtendedCost);
                                                obj.DispExtendedCost = ExtendedCost;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.LeadTimeInDays.ToString()))
                                            {
                                                Int32 LeadTimeInDays;
                                                Int32.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.LeadTimeInDays.ToString()]), out LeadTimeInDays);
                                                obj.LeadTimeInDays = LeadTimeInDays;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Trend.ToString()))
                                            {
                                                Boolean Trend = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Trend.ToString()]), out Trend);
                                                obj.Trend = Trend;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Taxable.ToString()))
                                            {
                                                Boolean Taxable = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Taxable.ToString()]), out Taxable);
                                                obj.Taxable = Taxable;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Consignment.ToString()))
                                            {
                                                Boolean Consignment = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Consignment.ToString()]), out Consignment);
                                                obj.Consignment = Consignment;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.StagedQuantity.ToString()))
                                            {
                                                double StagedQuantity;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.StagedQuantity.ToString()]), out StagedQuantity);
                                                obj.DispStagedQuantity = StagedQuantity;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.InTransitquantity.ToString()))
                                            {
                                                double InTransitquantity;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.InTransitquantity.ToString()]), out InTransitquantity);
                                                obj.DispInTransitquantity = InTransitquantity;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OnOrderQuantity.ToString()))
                                            {
                                                double OnOrderQuantity;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OnOrderQuantity.ToString()]), out OnOrderQuantity);
                                                obj.DispOnOrderQuantity = OnOrderQuantity;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OnTransferQuantity.ToString()))
                                            {
                                                double OnTransferQuantity;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OnTransferQuantity.ToString()]), out OnTransferQuantity);
                                                obj.DispOnTransferQuantity = OnTransferQuantity;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SuggestedOrderQuantity.ToString()))
                                            {
                                                double SuggestedOrderQuantity;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.SuggestedOrderQuantity.ToString()]), out SuggestedOrderQuantity);
                                                obj.DispSuggestedOrderQuantity = SuggestedOrderQuantity;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.RequisitionedQuantity.ToString()))
                                            {
                                                double RequisitionedQuantity;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.RequisitionedQuantity.ToString()]), out RequisitionedQuantity);
                                                obj.DispRequisitionedQuantity = RequisitionedQuantity;

                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.AverageUsage.ToString()))
                                            {
                                                double AverageUsage;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.AverageUsage.ToString()]), out AverageUsage);
                                                obj.DispAverageUsage = AverageUsage;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Turns.ToString()))
                                            {
                                                double Turns;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Turns.ToString()]), out Turns);
                                                obj.DispTurns = Turns;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OnHandQuantity.ToString()))
                                            {
                                                double OnHandQuantity;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OnHandQuantity.ToString()]), out OnHandQuantity);
                                                obj.DispOnHandQuantity = OnHandQuantity;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.CriticalQuantity.ToString()))
                                            {
                                                double CriticalQuantity;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.CriticalQuantity.ToString()]), out CriticalQuantity);
                                                obj.CriticalQuantity = CriticalQuantity;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.MinimumQuantity.ToString()))
                                            {
                                                double MinimumQuantity;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.MinimumQuantity.ToString()]), out MinimumQuantity);
                                                obj.MinimumQuantity = MinimumQuantity;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.MaximumQuantity.ToString()))
                                            {
                                                double MaximumQuantity;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.MaximumQuantity.ToString()]), out MaximumQuantity);
                                                obj.MaximumQuantity = MaximumQuantity;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.WeightPerPiece.ToString()))
                                            {
                                                double WeightPerPiece;
                                                double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.WeightPerPiece.ToString()]), out WeightPerPiece);
                                                obj.WeightPerPiece = WeightPerPiece;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemUniqueNumber.ToString()))
                                            {
                                                obj.ItemUniqueNumber = Convert.ToString(item[CommonUtility.ImportItemColumn.ItemUniqueNumber.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsTransfer.ToString()))
                                            {
                                                Boolean IsTransfer = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsTransfer.ToString()]), out IsTransfer);
                                                obj.IsTransfer = IsTransfer;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsPurchase.ToString()))
                                            {
                                                Boolean IsPurchase = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsPurchase.ToString()]), out IsPurchase);
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
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.SerialNumberTracking.ToString()]), out SerialNumberTracking);
                                                obj.SerialNumberTracking = SerialNumberTracking;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.LotNumberTracking.ToString()))
                                            {
                                                Boolean LotNumberTracking = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.LotNumberTracking.ToString()]), out LotNumberTracking);
                                                obj.LotNumberTracking = LotNumberTracking;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.DateCodeTracking.ToString()))
                                            {
                                                Boolean DateCodeTracking = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.DateCodeTracking.ToString()]), out DateCodeTracking);
                                                obj.DateCodeTracking = DateCodeTracking;
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
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsLotSerialExpiryCost.ToString()))
                                            {
                                                obj.IsLotSerialExpiryCost = Convert.ToString(item[CommonUtility.ImportItemColumn.IsLotSerialExpiryCost.ToString()]);
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsItemLevelMinMaxQtyRequired.ToString()))
                                            {
                                                Boolean ItemLevelMinMaxQtyRequired = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsItemLevelMinMaxQtyRequired.ToString()]), out ItemLevelMinMaxQtyRequired);
                                                obj.IsItemLevelMinMaxQtyRequired = ItemLevelMinMaxQtyRequired;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsBuildBreak.ToString()))
                                            {
                                                Boolean BuildBreak = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsBuildBreak.ToString()]), out BuildBreak);
                                                obj.IsBuildBreak = BuildBreak;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsAutoInventoryClassification.ToString()))
                                            {
                                                Boolean AutoInventoryClassification = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsAutoInventoryClassification.ToString()]), out AutoInventoryClassification);
                                                obj.IsAutoInventoryClassification = AutoInventoryClassification;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.EnforceDefaultPullQuantity.ToString()))
                                            {
                                                Boolean IsPullQtyScanOverride = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.EnforceDefaultPullQuantity.ToString()]), out IsPullQtyScanOverride);
                                                obj.PullQtyScanOverride = IsPullQtyScanOverride;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.EnforceDefaultReorderQuantity.ToString()))
                                            {
                                                Boolean IsEnforceDefaultReorderQuantity = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.EnforceDefaultReorderQuantity.ToString()]), out IsEnforceDefaultReorderQuantity);
                                                obj.IsEnforceDefaultReorderQuantity = IsEnforceDefaultReorderQuantity;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.TrendingSetting.ToString()))
                                            {
                                                obj.TrendingSettingName = item[CommonUtility.ImportItemColumn.TrendingSetting.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsPackslipMandatoryAtReceive.ToString()))
                                            {
                                                Boolean IsPullQtyScanOverride = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsPackslipMandatoryAtReceive.ToString()]), out IsPullQtyScanOverride);
                                                obj.IsPackslipMandatoryAtReceive = IsPullQtyScanOverride;
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
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsDeleted.ToString()]), out IsPullQtyScanOverride);
                                                obj.IsDeleted = IsPullQtyScanOverride;
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.IsLotSerialExpiryCost = false;
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region Locations
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.LocationMaster.ToString())
                            {
                                List<BinMasterMain> lstImport = new List<BinMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    BinMasterMain obj = new BinMasterMain();
                                    try
                                    {
                                        // bool AllowInsert = true;
                                        if (item[CommonUtility.ImportLocationColumn.Location.ToString()].ToString() != "")
                                        //  AllowInsert = lstImport.Where(x => x.Location == item[CommonUtility.ImportLocationColumn.Location.ToString()].ToString()).ToList().Count > 0 ? false : true;
                                        // if (AllowInsert == true)
                                        {
                                            // obj.ID = Convert.ToInt32(item[CommonUtility.ImportLocationColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportLocationColumn.Location.ToString()))
                                            {
                                                obj.BinNumber = item[CommonUtility.ImportLocationColumn.Location.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportLocationColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportLocationColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportLocationColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportLocationColumn.UDF2.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportLocationColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportLocationColumn.UDF3.ToString()].ToString();

                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportLocationColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportLocationColumn.UDF4.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportLocationColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportLocationColumn.UDF5.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.UDF6 = item[CommonUtility.ImportBinColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportBinColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportBinColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportBinColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportBinColumn.UDF10.ToString()].ToString();

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region Tool Category
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ToolCategoryMaster.ToString())
                            {
                                List<ToolCategoryMasterMain> lstImport = new List<ToolCategoryMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    ToolCategoryMasterMain obj = new ToolCategoryMasterMain();
                                    try
                                    {
                                        // bool AllowInsert = true;
                                        if (item[CommonUtility.ImportToolCategoryColumn.ToolCategory.ToString()].ToString() != "")
                                        // AllowInsert = lstImport.Where(x => x.ToolCategory == item[CommonUtility.ImportToolCategoryColumn.ToolCategory.ToString()].ToString()).ToList().Count > 0 ? false : true;
                                        //if (AllowInsert == true)
                                        {
                                            //   obj.ID = Convert.ToInt32(item[CommonUtility.ImportToolCategoryColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCategoryColumn.ToolCategory.ToString()))
                                            {
                                                obj.ToolCategory = item[CommonUtility.ImportToolCategoryColumn.ToolCategory.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCategoryColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportToolCategoryColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCategoryColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportToolCategoryColumn.UDF2.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCategoryColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportToolCategoryColumn.UDF3.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCategoryColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportToolCategoryColumn.UDF4.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCategoryColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportToolCategoryColumn.UDF5.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.UDF6 = item[CommonUtility.ImportBinColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportBinColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportBinColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportBinColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportBinColumn.UDF10.ToString()].ToString();

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion

                            #region CostUOM Master

                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.CostUOMMaster.ToString())
                            {
                                List<CostUOMMasterMain> lstImport = new List<CostUOMMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    CostUOMMasterMain obj = new CostUOMMasterMain();
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ImportCostUOMColumn.CostUOM.ToString()].ToString() != "")
                                        //AllowInsert = lstImport.Where(x => x.CostUOM == item[CommonUtility.ImportCostUOMColumn.CostUOM.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            // obj.ID = Convert.ToInt32(item[CommonUtility.ImportCostUOMColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportCostUOMColumn.CostUOM.ToString()))
                                            {
                                                obj.CostUOM = item[CommonUtility.ImportCostUOMColumn.CostUOM.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportCostUOMColumn.CostUOMValue.ToString()))
                                            {
                                                obj.CostUOMValue = Convert.ToInt32(item[CommonUtility.ImportCostUOMColumn.CostUOMValue.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportCostUOMColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportCostUOMColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportCostUOMColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportCostUOMColumn.UDF2.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportCostUOMColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportCostUOMColumn.UDF3.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportCostUOMColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportCostUOMColumn.UDF4.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportCostUOMColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportCostUOMColumn.UDF5.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }

                            #endregion

                            #region InventoryClassification Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.InventoryClassificationMaster.ToString())
                            {
                                List<InventoryClassificationMasterMain> lstImport = new List<InventoryClassificationMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    InventoryClassificationMasterMain obj = new InventoryClassificationMasterMain();
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ImportInventoryClassificationColumn.InventoryClassification.ToString()].ToString() != "")
                                        //  AllowInsert = lstImport.Where(x => x.InventoryClassification == item[CommonUtility.ImportInventoryClassificationColumn.InventoryClassification.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryClassificationColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryClassificationColumn.InventoryClassification.ToString()))
                                            {
                                                obj.InventoryClassification = item[CommonUtility.ImportInventoryClassificationColumn.InventoryClassification.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryClassificationColumn.BaseOfInventory.ToString()))
                                            {
                                                obj.BaseOfInventory = item[CommonUtility.ImportInventoryClassificationColumn.BaseOfInventory.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryClassificationColumn.RangeStart.ToString()))
                                            {
                                                obj.RangeStart = Convert.ToDouble(item[CommonUtility.ImportInventoryClassificationColumn.RangeStart.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryClassificationColumn.RangeEnd.ToString()))
                                            {
                                                obj.RangeEnd = Convert.ToDouble(item[CommonUtility.ImportInventoryClassificationColumn.RangeEnd.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryClassificationColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportInventoryClassificationColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryClassificationColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportInventoryClassificationColumn.UDF2.ToString()].ToString();

                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryClassificationColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportInventoryClassificationColumn.UDF3.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryClassificationColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportInventoryClassificationColumn.UDF4.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryClassificationColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportInventoryClassificationColumn.UDF5.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.UDF6 = item[CommonUtility.ImportCategoryColumn.UDF6.ToString()].ToString();
                                            //obj.UDF7 = item[CommonUtility.ImportCategoryColumn.UDF7.ToString()].ToString();
                                            //obj.UDF8 = item[CommonUtility.ImportCategoryColumn.UDF8.ToString()].ToString();
                                            //obj.UDF9 = item[CommonUtility.ImportCategoryColumn.UDF9.ToString()].ToString();
                                            //obj.UDF10 = item[CommonUtility.ImportCategoryColumn.UDF10.ToString()].ToString();
                                            //obj.CategoryColor = item[CommonUtility.ImportCategoryColumn.CategoryColor.ToString()].ToString();
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region Tool master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ToolMaster.ToString())
                            {
                                List<ToolMasterMain> lstImport = new List<ToolMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    ToolMasterMain obj = new ToolMasterMain();
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ImportToolMasterColumn.ToolName.ToString()].ToString() != "")
                                        //  AllowInsert = lstImport.Where(x => x.InventoryClassification == item[CommonUtility.ImportInventoryClassificationColumn.InventoryClassification.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryClassificationColumn.ID.ToString()].ToString());
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
                                                obj.ToolCategory = item[CommonUtility.ImportToolMasterColumn.ToolCategory.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.Location.ToString()))
                                            {
                                                obj.Location = item[CommonUtility.ImportToolMasterColumn.Location.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.Quantity.ToString()))
                                            {
                                                if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportToolMasterColumn.Quantity.ToString()])))
                                                    obj.Quantity = Convert.ToDouble(item[CommonUtility.ImportToolMasterColumn.Quantity.ToString()]);
                                                else
                                                    obj.Quantity = 0;
                                                //obj.Quantity = Convert.ToDouble(item[CommonUtility.ImportToolMasterColumn.Quantity.ToString()]);
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.Cost.ToString()))
                                            {
                                                if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportToolMasterColumn.Cost.ToString()])))
                                                    obj.Cost = Convert.ToDouble(item[CommonUtility.ImportToolMasterColumn.Cost.ToString()]);
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

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.Technician.ToString()))
                                            {
                                                obj.Technician = item[CommonUtility.ImportToolMasterColumn.Technician.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.CheckOutQuantity.ToString()))
                                            {
                                                if (item[CommonUtility.ImportToolMasterColumn.CheckOutQuantity.ToString()] != "")
                                                {
                                                    obj.CheckOutQuantity = Convert.ToDouble(item[CommonUtility.ImportToolMasterColumn.CheckOutQuantity.ToString()]);
                                                }
                                                else
                                                {
                                                    obj.CheckOutQuantity = 0;
                                                }
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.CheckInQuantity.ToString()))
                                            {
                                                if (item[CommonUtility.ImportToolMasterColumn.CheckInQuantity.ToString()] != "")
                                                {
                                                    obj.CheckInQuantity = Convert.ToDouble(item[CommonUtility.ImportToolMasterColumn.CheckInQuantity.ToString()]);
                                                }
                                                else
                                                {
                                                    obj.CheckInQuantity = Convert.ToDouble(0);
                                                }

                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region Asset master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.AssetMaster.ToString())
                            {
                                List<AssetMasterMain> lstImport = new List<AssetMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    AssetMasterMain obj = new AssetMasterMain();
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ImportAssetMasterColumn.AssetName.ToString()].ToString() != "")
                                        //  AllowInsert = lstImport.Where(x => x.InventoryClassification == item[CommonUtility.ImportInventoryClassificationColumn.InventoryClassification.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
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
                                                obj.ToolCategory = item[CommonUtility.ImportAssetMasterColumn.ToolCategory.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.PurchaseDate.ToString()))
                                            {
                                                obj.PurchaseDate = Convert.ToDateTime(item[CommonUtility.ImportAssetMasterColumn.PurchaseDate.ToString()].ToString() == "" ? Convert.ToString(DateTime.MinValue) : item[CommonUtility.ImportAssetMasterColumn.PurchaseDate.ToString()].ToString());
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.PurchasePrice.ToString()))
                                            {
                                                obj.PurchasePrice = Convert.ToDouble(item[CommonUtility.ImportAssetMasterColumn.PurchasePrice.ToString()].ToString() == "" ? "0.0" : item[CommonUtility.ImportAssetMasterColumn.PurchasePrice.ToString()].ToString());
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.DepreciatedValue.ToString()))
                                            {
                                                obj.DepreciatedValue = Convert.ToDouble(item[CommonUtility.ImportAssetMasterColumn.DepreciatedValue.ToString()].ToString() == "" ? "0.0" : item[CommonUtility.ImportAssetMasterColumn.DepreciatedValue.ToString()].ToString());
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
                                            if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.AssetCategory.ToString()))
                                            {
                                                obj.AssetCategory = item[CommonUtility.ImportAssetMasterColumn.AssetCategory.ToString()].ToString();
                                            }


                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region QuickListItems master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.QuickListItems.ToString())
                            {
                                List<QuickListItemsMain> lstImport = new List<QuickListItemsMain>();
                                foreach (DataRow item in list)
                                {
                                    QuickListItemsMain obj = new QuickListItemsMain();
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ImportQuickListItemsColumn.Name.ToString()].ToString() != "" && item[CommonUtility.ImportQuickListItemsColumn.ItemNumber.ToString()].ToString() != "")
                                        //  AllowInsert = lstImport.Where(x => x.InventoryClassification == item[CommonUtility.ImportInventoryClassificationColumn.InventoryClassification.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryClassificationColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.Name.ToString()))
                                            {
                                                obj.QuickListname = item[CommonUtility.ImportQuickListItemsColumn.Name.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.ItemNumber.ToString()))
                                            {
                                                obj.ItemNumber = item[CommonUtility.ImportQuickListItemsColumn.ItemNumber.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.Comment.ToString()))
                                            {
                                                obj.Comments = item[CommonUtility.ImportQuickListItemsColumn.Comment.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.Quantity.ToString()))
                                            {
                                                obj.Quantity = Convert.ToDouble(item[CommonUtility.ImportQuickListItemsColumn.Quantity.ToString()].ToString());
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportQuickListItemsColumn.UDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportQuickListItemsColumn.UDF2.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportQuickListItemsColumn.UDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportQuickListItemsColumn.UDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuickListItemsColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportQuickListItemsColumn.UDF5.ToString()].ToString();
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region Kits
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.kitdetail.ToString())
                            {
                                List<KitDetailmain> lstImport = new List<KitDetailmain>();
                                foreach (DataRow item in list)
                                {
                                    KitDetailmain obj = new KitDetailmain();
                                    try
                                    {

                                        if (item[CommonUtility.ImportKitsItemsColumn.ItemNumber.ToString()].ToString() != "" && item[CommonUtility.ImportKitsItemsColumn.KitPartNumber.ToString()].ToString() != "")
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryClassificationColumn.ID.ToString()].ToString());
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
                                                obj.QuantityPerKit = Convert.ToDouble(item[CommonUtility.ImportKitsItemsColumn.QuantityPerKit.ToString()].ToString());
                                            }
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region BOM Item Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.BOMItemMaster.ToString())
                            {
                                List<BOMItemMasterMain> lstImport = new List<BOMItemMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    BOMItemMasterMain obj = new BOMItemMasterMain();
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

                                                if (string.IsNullOrEmpty(obj.SupplierName))
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
                                                Int32.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.LeadTimeInDays.ToString()]), out LeadTimeInDays);
                                                obj.LeadTimeInDays = LeadTimeInDays;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Taxable.ToString()))
                                            {
                                                Boolean Taxable = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Taxable.ToString()]), out Taxable);
                                                obj.Taxable = Taxable;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Consignment.ToString()))
                                            {
                                                Boolean Consignment = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Consignment.ToString()]), out Consignment);
                                                obj.Consignment = Consignment;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemUniqueNumber.ToString()))
                                            {
                                                obj.ItemUniqueNumber = Convert.ToString(item[CommonUtility.ImportItemColumn.ItemUniqueNumber.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsTransfer.ToString()))
                                            {
                                                Boolean IsTransfer = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsTransfer.ToString()]), out IsTransfer);
                                                obj.IsTransfer = IsTransfer;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsPurchase.ToString()))
                                            {
                                                Boolean IsPurchase = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsPurchase.ToString()]), out IsPurchase);
                                                obj.IsPurchase = IsPurchase;
                                            }
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
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.SerialNumberTracking.ToString()]), out SerialNumberTracking);
                                                obj.SerialNumberTracking = SerialNumberTracking;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.LotNumberTracking.ToString()))
                                            {
                                                Boolean LotNumberTracking = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.LotNumberTracking.ToString()]), out LotNumberTracking);
                                                obj.LotNumberTracking = LotNumberTracking;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.DateCodeTracking.ToString()))
                                            {
                                                Boolean DateCodeTracking = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.DateCodeTracking.ToString()]), out DateCodeTracking);
                                                obj.DateCodeTracking = DateCodeTracking;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemType.ToString()))
                                            {
                                                obj.ItemTypeName = item[CommonUtility.ImportItemColumn.ItemType.ToString()].ToString();
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
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";
                                            //obj.IsLotSerialExpiryCost = false;
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion

                        }
                    }
                }
            }
            catch
            {

            }

            return View();
            //return RedirectToAction("ImportMasters");
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
                        retval = "itemnumber,manufacturer,manufacturernumber,suppliername,supplierpartno,blanketordernumber,upc,unspsc,description,longdescription,categoryname,glaccount,uom,costuom,defaultreorderquantity,defaultpullquantity,cost,markup,sellprice,extendedcost,leadtimeindays,link1,link2,trend,taxable,consignment,stagedquantity,intransitquantity,onorderquantity,ontransferquantity,suggestedorderquantity,requisitionedquantity,averageusage,turns,onhandquantity,criticalquantity,minimumquantity,maximumquantity,weightperpiece,itemuniquenumber,istransfer,ispurchase,inventrylocation,inventoryclassification,serialnumbertracking,lotnumbertracking,datecodetracking,itemtype,imagepath,udf1,udf2,udf3,udf4,udf5,islotserialexpirycost,isitemlevelminmaxqtyrequired,trendingsetting,enforcedefaultpullquantity,enforcedefaultreorderquantity,isautoinventoryclassification,isbuildbreak,ispackslipmandatoryatreceive,itemimageexternalurl,itemdocexternalurl,isdeleted";
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
                        retval = "inventoryclassification,baseofinventory,rangestart,rangeend,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "BinMaster":
                        retval = "itemnumber,locationname,customerownedquantity,consignedquantity,serialnumber,lotnumber,expirationdate,cost";//,receiveddate
                        break;
                    case "ItemLocationeVMISetup":
                        retval = "itemnumber,locationname,minimumquantity,maximumquantity,criticalquantity,sensorid,sensorport,isdefault";
                        break;
                    case "ManufacturerMaster":
                        retval = "manufacturer,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "MeasurementTermMaster":
                        retval = "measurementterm,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "SupplierMaster":
                        //retval = "suppliername,description,receiverid,address,city,state,zipcode,country,contact,phone,fax,email,isemailpoinbody,isemailpoinpdf,isemailpoincsv,isemailpoinx12,udf1,udf2,udf3,udf4,udf5";
                        retval = "suppliername,suppliercolor,description,branchnumber,maximumordersize,address,city,state,zipcode,country,contact,phone,fax,issendtovendor,isvendorreturnasn,issupplierreceiveskitcomponents,ordernumbertypeblank,ordernumbertypefixed,ordernumbertypeblanketordernumber,ordernumbertypeincrementingordernumber,ordernumbertypeincrementingbyday,ordernumbertypedateincrementing,ordernumbertypedate,udf1,udf2,udf3,udf4,udf5,accountnumber,accountname,blanketponumber,startdate,enddate,maxlimit,donotexceed,PullPurchaseNumberFixed,PullPurchaseNumberBlanketOrder,PullPurchaseNumberDateIncrementing,PullPurchaseNumberDate,PullPurchaseNumberType,LastPullPurchaseNumberUsed";
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
                        retval = "toolname,serial,description,toolcategory,cost,quantity,location,udf1,udf2,udf3,udf4,udf5,isgroupofitems,Technician,CheckOutQuantity,CheckInQuantity";
                        break;
                    case "AssetMaster":
                        retval = "assetname,description,make,model,serial,toolcategory,purchasedate,purchaseprice,depreciatedvalue,udf1,udf2,udf3,udf4,udf5,assetcategory";
                        break;
                    case "QuickListItems":
                        retval = "name,comment,itemnumber,quantity,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "kitdetail":
                        retval = "kitpartnumber,itemnumber,quantityperkit";
                        break;
                    //case "InventoryLocation":
                    //    retval = "itemnumber,locationname,criticalquantity,minimumquantity,maximumquantity,sensorid,sensorport,isdefault";
                    //    break;




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
                        retval = "inventoryclassification,baseofinventory,rangestart,rangeend";
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
                        retval = "technician,techniciancode";
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
                        retval = "toolname,quantity";
                        break;
                    case "AssetMaster":
                        retval = "assetname";
                        break;
                    case "QuickListItems":
                        retval = "name,itemnumber,quantity";
                        break;
                    case "kitdetail":
                        retval = "kitpartnumber,itemnumber,quantityperkit";
                        break;
                    //case "InventoryLocation":
                    //    retval = "ItemNumber,LocationName";
                    //    break;
                }

            }
            return retval;
        }
        public JsonResult GetImportList(JQueryDataTableParamModel param)
        {
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            List<object> lstfinal = new List<object>();

            #region BinMaster
            if (ImportMastersDTO.TableName.BinMaster.ToString() == Session["CurModule"].ToString())
            {
                List<InventoryLocationMain> lst = new List<InventoryLocationMain>();
                lst = (List<InventoryLocationMain>)Session["importedData"];

                CurrentBinList = lst;
                ViewBag.TotalRecordCount = CurrentBinList.Count;
                TotalRecordCount = CurrentBinList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentBinList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Item Location Quantity
            if (ImportMastersDTO.TableName.ItemLocationeVMISetup.ToString() == Session["CurModule"].ToString())
            {
                List<InventoryLocationQuantityMain> lst = new List<InventoryLocationQuantityMain>();
                lst = (List<InventoryLocationQuantityMain>)Session["importedData"];

                CurrentInventoryLocationQuantityList = lst;
                ViewBag.TotalRecordCount = CurrentInventoryLocationQuantityList.Count;
                TotalRecordCount = CurrentInventoryLocationQuantityList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentInventoryLocationQuantityList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Category Master
            else if (ImportMastersDTO.TableName.CategoryMaster.ToString() == Session["CurModule"].ToString())
            {
                List<CategoryMasterMain> lst = new List<CategoryMasterMain>();
                lst = (List<CategoryMasterMain>)Session["importedData"];

                CurrentCategoryList = lst;
                ViewBag.TotalRecordCount = CurrentCategoryList.Count;
                TotalRecordCount = CurrentCategoryList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentCategoryList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Customer Master
            else if (ImportMastersDTO.TableName.CustomerMaster.ToString() == Session["CurModule"].ToString())
            {
                List<CustomerMasterMain> lst = new List<CustomerMasterMain>();
                lst = (List<CustomerMasterMain>)Session["importedData"];

                CurrentCustomerList = lst;
                ViewBag.TotalRecordCount = CurrentCustomerList.Count;
                TotalRecordCount = CurrentCustomerList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentCustomerList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region FreightTypeMaster
            else if (ImportMastersDTO.TableName.FreightTypeMaster.ToString() == Session["CurModule"].ToString())
            {
                List<FreightTypeMasterMain> lst = new List<FreightTypeMasterMain>();
                lst = (List<FreightTypeMasterMain>)Session["importedData"];

                CurrentFreightType = lst;
                ViewBag.TotalRecordCount = CurrentFreightType.Count;
                TotalRecordCount = CurrentFreightType.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentFreightType
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region G/L Account Master
            else if (ImportMastersDTO.TableName.GLAccountMaster.ToString() == Session["CurModule"].ToString())
            {
                List<GLAccountMasterMain> lst = new List<GLAccountMasterMain>();
                lst = (List<GLAccountMasterMain>)Session["importedData"];

                CurrentGLAccountList = lst;
                ViewBag.TotalRecordCount = CurrentGLAccountList.Count;
                TotalRecordCount = CurrentGLAccountList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentGLAccountList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region GXPR Consigned Master
            else if (ImportMastersDTO.TableName.GXPRConsigmentJobMaster.ToString() == Session["CurModule"].ToString())
            {
                List<GXPRConsignedMasterMain> lst = new List<GXPRConsignedMasterMain>();
                lst = (List<GXPRConsignedMasterMain>)Session["importedData"];

                CurrentGXPRConsignedList = lst;
                ViewBag.TotalRecordCount = CurrentGXPRConsignedList.Count;
                TotalRecordCount = CurrentGXPRConsignedList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentGXPRConsignedList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Job Type Master
            else if (ImportMastersDTO.TableName.JobTypeMaster.ToString() == Session["CurModule"].ToString())
            {
                List<JobTypeMasterMain> lst = new List<JobTypeMasterMain>();
                lst = (List<JobTypeMasterMain>)Session["importedData"];

                CurrentJobTypeList = lst;
                ViewBag.TotalRecordCount = CurrentJobTypeList.Count;
                TotalRecordCount = CurrentJobTypeList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentJobTypeList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Ship Via Master
            else if (ImportMastersDTO.TableName.ShipViaMaster.ToString() == Session["CurModule"].ToString())
            {
                List<ShipViaMasterMain> lst = new List<ShipViaMasterMain>();
                lst = (List<ShipViaMasterMain>)Session["importedData"];

                CurrentShipViaList = lst;
                ViewBag.TotalRecordCount = CurrentShipViaList.Count;
                TotalRecordCount = CurrentShipViaList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentShipViaList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Technician Master
            else if (ImportMastersDTO.TableName.TechnicianMaster.ToString() == Session["CurModule"].ToString())
            {
                List<TechnicianMasterMain> lst = new List<TechnicianMasterMain>();
                lst = (List<TechnicianMasterMain>)Session["importedData"];

                CurrentTechnicianList = lst;
                ViewBag.TotalRecordCount = CurrentTechnicianList.Count;
                TotalRecordCount = CurrentTechnicianList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentTechnicianList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Manufacturer Master
            else if (ImportMastersDTO.TableName.ManufacturerMaster.ToString() == Session["CurModule"].ToString())
            {
                List<ManufacturerMasterMain> lst = new List<ManufacturerMasterMain>();
                lst = (List<ManufacturerMasterMain>)Session["importedData"];

                CurrentManufacturerList = lst;
                ViewBag.TotalRecordCount = CurrentManufacturerList.Count;
                TotalRecordCount = CurrentManufacturerList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentManufacturerList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region MeasurementTerm Master
            else if (ImportMastersDTO.TableName.MeasurementTermMaster.ToString() == Session["CurModule"].ToString())
            {
                List<MeasurementTermMasterMain> lst = new List<MeasurementTermMasterMain>();
                lst = (List<MeasurementTermMasterMain>)Session["importedData"];

                CurrentMeasurementTermList = lst;
                ViewBag.TotalRecordCount = CurrentMeasurementTermList.Count;
                TotalRecordCount = CurrentMeasurementTermList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentMeasurementTermList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Unit Master
            else if (ImportMastersDTO.TableName.UnitMaster.ToString() == Session["CurModule"].ToString())
            {
                List<UnitMasterMain> lst = new List<UnitMasterMain>();
                lst = (List<UnitMasterMain>)Session["importedData"];

                CurrentUnitList = lst;
                ViewBag.TotalRecordCount = CurrentUnitList.Count;
                TotalRecordCount = CurrentUnitList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentUnitList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Supplier Master
            else if (ImportMastersDTO.TableName.SupplierMaster.ToString() == Session["CurModule"].ToString())
            {
                List<SupplierMasterMain> lst = new List<SupplierMasterMain>();
                lst = (List<SupplierMasterMain>)Session["importedData"];

                CurrentSupplierList = lst;
                ViewBag.TotalRecordCount = CurrentSupplierList.Count;
                TotalRecordCount = CurrentSupplierList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentSupplierList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Item Master
            else if (ImportMastersDTO.TableName.ItemMaster.ToString() == Session["CurModule"].ToString())
            {
                List<BOMItemMasterMain> lst = new List<BOMItemMasterMain>();
                lst = (List<BOMItemMasterMain>)Session["importedData"];

                CurrentItemList = lst;
                ViewBag.TotalRecordCount = CurrentItemList.Count;
                TotalRecordCount = CurrentItemList.Count;


                //return Json(new
                //{
                //    sEcho = param.sEcho,
                //    iTotalRecords = TotalRecordCount,
                //    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                //    aaData = CurrentItemList
                //}, JsonRequestBehavior.AllowGet);


                //JavaScriptSerializer serializer = new JavaScriptSerializer();

                //serializer.MaxJsonLength = Int32.MaxValue; // Whatever max length you want here

                //var resultData = new {
                //    sEcho = param.sEcho,
                //    iTotalRecords = TotalRecordCount,
                //    iTotalDisplayRecords = TotalRecordCount,
                //    aaData = CurrentItemList
                //}; // Whatever value you are serializing

                //ContentResult result = new ContentResult();

                //result.Content = serializer.Serialize(resultData);

                //result.ContentType = "application/json";

                //return result;


                JsonResult jsresult = Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = CurrentItemList
                });
                jsresult.MaxJsonLength = int.MaxValue;


                return jsresult;
            }
            #endregion
            #region LocationMaster
            if (ImportMastersDTO.TableName.LocationMaster.ToString() == Session["CurModule"].ToString())
            {
                List<BinMasterMain> lst = new List<BinMasterMain>();
                lst = (List<BinMasterMain>)Session["importedData"];

                CurrentLocationList = lst;
                ViewBag.TotalRecordCount = CurrentLocationList.Count;
                TotalRecordCount = CurrentLocationList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentLocationList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region ToolCategoryMaster
            if (ImportMastersDTO.TableName.ToolCategoryMaster.ToString() == Session["CurModule"].ToString())
            {
                List<ToolCategoryMasterMain> lst = new List<ToolCategoryMasterMain>();
                lst = (List<ToolCategoryMasterMain>)Session["importedData"];

                CurrentToolCategoryList = lst;
                ViewBag.TotalRecordCount = CurrentToolCategoryList.Count;
                TotalRecordCount = CurrentToolCategoryList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentToolCategoryList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region CostUOM Master
            else if (ImportMastersDTO.TableName.CostUOMMaster.ToString() == Session["CurModule"].ToString())
            {
                List<CostUOMMasterMain> lst = new List<CostUOMMasterMain>();
                lst = (List<CostUOMMasterMain>)Session["importedData"];

                CurrentCostUOMList = lst;
                ViewBag.TotalRecordCount = CurrentCostUOMList.Count;
                TotalRecordCount = CurrentCostUOMList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentCostUOMList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region InventoryClassification Master

            else if (ImportMastersDTO.TableName.InventoryClassificationMaster.ToString() == Session["CurModule"].ToString())
            {
                List<InventoryClassificationMasterMain> lst = new List<InventoryClassificationMasterMain>();
                lst = (List<InventoryClassificationMasterMain>)Session["importedData"];

                CurrentInventoryClassificationList = lst;
                ViewBag.TotalRecordCount = CurrentInventoryClassificationList.Count;
                TotalRecordCount = CurrentInventoryClassificationList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentInventoryClassificationList
                }, JsonRequestBehavior.AllowGet);
            }

            #endregion
            #region Tool Master
            else if (ImportMastersDTO.TableName.ToolMaster.ToString() == Session["CurModule"].ToString())
            {
                List<ToolMasterMain> lst = new List<ToolMasterMain>();
                lst = (List<ToolMasterMain>)Session["importedData"];

                CurrentToolList = lst;
                ViewBag.TotalRecordCount = CurrentToolList.Count;
                TotalRecordCount = CurrentToolList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentToolList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Asset Master
            else if (ImportMastersDTO.TableName.AssetMaster.ToString() == Session["CurModule"].ToString())
            {
                List<AssetMasterMain> lst = new List<AssetMasterMain>();
                lst = (List<AssetMasterMain>)Session["importedData"];

                CurrentAssetMasterList = lst;
                ViewBag.TotalRecordCount = CurrentAssetMasterList.Count;
                TotalRecordCount = CurrentAssetMasterList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentAssetMasterList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region QuickList Master
            else if (ImportMastersDTO.TableName.QuickListItems.ToString() == Session["CurModule"].ToString())
            {
                List<QuickListItemsMain> lst = new List<QuickListItemsMain>();
                lst = (List<QuickListItemsMain>)Session["importedData"];

                CurrentQuickListMasterList = lst;
                ViewBag.TotalRecordCount = CurrentQuickListMasterList.Count;
                TotalRecordCount = CurrentQuickListMasterList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentQuickListMasterList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region InventoryLocation Master
            else if (ImportMastersDTO.TableName.InventoryLocation.ToString() == Session["CurModule"].ToString())
            {
                List<InventoryLocationMain> lst = new List<InventoryLocationMain>();
                lst = (List<InventoryLocationMain>)Session["importedData"];

                CurrentInventoryLocationMasterList = lst;
                ViewBag.TotalRecordCount = CurrentInventoryLocationMasterList.Count;
                TotalRecordCount = CurrentInventoryLocationMasterList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentInventoryLocationMasterList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region BOM Item Master
            else if (ImportMastersDTO.TableName.BOMItemMaster.ToString() == Session["CurModule"].ToString())
            {
                List<BOMItemMasterMain> lst = new List<BOMItemMasterMain>();
                lst = (List<BOMItemMasterMain>)Session["importedData"];

                CurrentBOMItemList = lst;
                ViewBag.TotalRecordCount = CurrentBOMItemList.Count;
                TotalRecordCount = CurrentBOMItemList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentBOMItemList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Kits
            else if (ImportMastersDTO.TableName.kitdetail.ToString() == Session["CurModule"].ToString())
            {
                List<KitDetailmain> lst = new List<KitDetailmain>();
                lst = (List<KitDetailmain>)Session["importedData"];

                CurrentKitItemList = lst;
                ViewBag.TotalRecordCount = CurrentKitItemList.Count;
                TotalRecordCount = CurrentKitItemList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentKitItemList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = ""
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Save Import
        [HttpPost]
        public JsonResult SaveImport(string TableName, string para = "")
        {
            List<Guid> lstItemGUID = new List<Guid>();
            string message = "";
            string status = "";
            bool allSuccesfulRecords = true;
            string savedOnlyitemIds = string.Empty;
            JavaScriptSerializer s = new JavaScriptSerializer();
            try
            {
                ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
                List<UDFOptionsCheckDTO> lst = new List<UDFOptionsCheckDTO>();
                List<UDFOptionsMain> CurrentOptionList = null;
                ImportDAL obj = new ImportDAL(SessionHelper.EnterPriseDBName);

                #region BinMaster
                //if (ImportMastersDTO.TableName.BinMaster.ToString() == TableName)
                //{
                //    List<BinMasterMain> CurrentBlankBinList = new List<BinMasterMain>();
                //    BinMasterMain[] LstBinMaster = s.Deserialize<BinMasterMain[]>(para);
                //    if (LstBinMaster != null && LstBinMaster.Length > 0)
                //    {
                //        CurrentBinList = new List<BinMasterMain>();
                //        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.BinMaster.ToString(), UDFControlTypes.Textbox.ToString());
                //        CurrentOptionList = new List<UDFOptionsMain>();

                //        foreach (BinMasterMain item in LstBinMaster)
                //        {
                //            BinMasterMain objDTO = new BinMasterMain();
                //            objDTO.ID = item.ID;
                //            objDTO.BinNumber = item.BinNumber;
                //            objDTO.UDF1 = item.UDF1;
                //            objDTO.UDF2 = item.UDF2;
                //            objDTO.UDF3 = item.UDF3;
                //            objDTO.UDF4 = item.UDF4;
                //            objDTO.UDF5 = item.UDF5;
                //            objDTO.UDF6 = item.UDF6;
                //            objDTO.UDF7 = item.UDF7;
                //            objDTO.UDF8 = item.UDF8;
                //            objDTO.UDF9 = item.UDF9;
                //            objDTO.UDF10 = item.UDF10;
                //            objDTO.IsStagingLocation = false;
                //            objDTO.IsDeleted = false;
                //            objDTO.IsArchived = false;
                //            objDTO.Created = DateTime.Now;
                //            objDTO.LastUpdated = System.DateTime.Now;
                //            objDTO.LastUpdatedBy = SessionHelper.UserID;
                //            objDTO.Room = SessionHelper.RoomID;
                //            objDTO.CompanyID = SessionHelper.CompanyID;
                //            objDTO.CreatedBy = SessionHelper.UserID;
                //            objDTO.GUID = Guid.NewGuid();

                //            if (item.BinNumber.Trim() != "")
                //            {
                //                CurrentBinList.Add(objDTO);

                //                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportBinColumn.UDF1.ToString());
                //                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportBinColumn.UDF2.ToString());
                //                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportBinColumn.UDF3.ToString());
                //                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportBinColumn.UDF4.ToString());
                //                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportBinColumn.UDF5.ToString());
                //                //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportBinColumn.UDF6.ToString());

                //            }
                //            else
                //                CurrentBlankBinList.Add(objDTO);
                //        }

                //        List<BinMasterMain> lstreturn = new List<BinMasterMain>();
                //        if (CurrentBinList.Count > 0)
                //            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.BinMaster.ToString(), CurrentBinList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                //        if (CurrentBlankBinList.Count > 0)
                //        {
                //            foreach (BinMasterMain item in CurrentBlankBinList)
                //            {
                //                lstreturn.Add(item);
                //            }
                //        }
                //        if (lstreturn.Count == 0)
                //        {
                //            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                //            status = ResMessage.SaveMessage;
                //            ClearCurrentResourceList();
                //            Session["importedData"] = null;
                //        }
                //        else
                //        {
                //            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                //            status = ResMessage.SaveMessage;
                //            Session["importedData"] = lstreturn;
                //        }


                //        CacheHelper<IEnumerable<BinMasterDTO>>.InvalidateCache();
                //        (new BinMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);

                //    }
                //}
                #endregion
                #region InventoryLocation Master
                if (ImportMastersDTO.TableName.BinMaster.ToString() == TableName)
                {

                    List<InventoryLocationMain> CurrentBlankInventoryLocationList = new List<InventoryLocationMain>();
                    InventoryLocationMain[] LstInventoryLocation = s.Deserialize<InventoryLocationMain[]>(para);
                    if (LstInventoryLocation != null && LstInventoryLocation.Length > 0)
                    {
                        CurrentInventoryLocationMasterList = new List<InventoryLocationMain>();
                        //lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.CategoryMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        //CurrentOptionList = new List<UDFOptionsMain>();

                        List<ItemMasterDTO> oItemMasterList = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetRecordsOnlyItemsFields(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                        foreach (InventoryLocationMain item in LstInventoryLocation)
                        {
                            //ItemMasterDTO objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetRecordByItemNumber(item.ItemNumber, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                            ItemMasterDTO objItemMasterDTO = oItemMasterList.Where(x => x.ItemNumber == item.ItemNumber).FirstOrDefault();
                            InventoryLocationMain objDTO = new InventoryLocationMain();
                            objDTO.ID = item.ID;
                            objDTO.ItemNumber = item.ItemNumber;
                            objDTO.ItemGUID = objItemMasterDTO == null ? Guid.Empty : objItemMasterDTO.GUID;
                            objDTO.BinNumber = item.BinNumber;
                            objDTO.Received = DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy");
                            objDTO.consignedquantity = item.consignedquantity;
                            objDTO.customerownedquantity = item.customerownedquantity;
                            if (objItemMasterDTO != null && !objItemMasterDTO.Consignment)
                            {
                                objDTO.consignedquantity = 0;
                            }
                            if (objItemMasterDTO != null && objItemMasterDTO.Consignment)
                            {
                                objDTO.customerownedquantity = 0;
                            }
                            objDTO.displayExpiration = item.Expiration;
                            if (objItemMasterDTO != null && objItemMasterDTO.DateCodeTracking == true)
                            {
                                objDTO.Expiration = item.Expiration;
                            }
                            objDTO.SerialNumber = item.SerialNumber;
                            objDTO.LotNumber = item.LotNumber;
                            objDTO.Cost = item.Cost;
                            objDTO.IsDeleted = false;
                            objDTO.IsArchived = false;
                            objDTO.Created = DateTimeUtility.DateTimeNow;
                            objDTO.Updated = DateTimeUtility.DateTimeNow;
                            objDTO.LastUpdatedBy = SessionHelper.UserID;
                            objDTO.Room = SessionHelper.RoomID;
                            objDTO.CompanyID = SessionHelper.CompanyID;
                            objDTO.CreatedBy = SessionHelper.UserID;
                            objDTO.InsertedFrom = "Import";
                            objDTO.GUID = Guid.NewGuid();

                            if (objDTO.ItemGUID.ToString() != Guid.Empty.ToString() && item.BinNumber.Trim() != "")
                            {
                                CurrentInventoryLocationMasterList.Add(objDTO);
                            }
                            else
                            {
                                CurrentBlankInventoryLocationList.Add(objDTO);
                            }
                            if (objItemMasterDTO != null && objItemMasterDTO.Consignment && (item.customerownedquantity ?? 0) > 0)
                            {
                                objDTO = new InventoryLocationMain();
                                objDTO.ID = item.ID;
                                objDTO.ItemNumber = item.ItemNumber;
                                objDTO.ItemGUID = objItemMasterDTO == null ? Guid.NewGuid() : objItemMasterDTO.GUID;
                                objDTO.BinNumber = item.BinNumber;
                                objDTO.Received = DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy");
                                objDTO.customerownedquantity = item.customerownedquantity;
                                objDTO.consignedquantity = 0;
                                objDTO.displayExpiration = item.Expiration;
                                if (objItemMasterDTO != null && objItemMasterDTO.DateCodeTracking == true)
                                {
                                    objDTO.Expiration = item.Expiration;
                                }
                                objDTO.SerialNumber = item.SerialNumber;
                                objDTO.LotNumber = item.LotNumber;
                                objDTO.Cost = item.Cost;
                                objDTO.IsDeleted = false;
                                objDTO.IsArchived = false;
                                objDTO.Created = DateTimeUtility.DateTimeNow;
                                objDTO.Updated = DateTimeUtility.DateTimeNow;
                                objDTO.LastUpdatedBy = SessionHelper.UserID;
                                objDTO.Room = SessionHelper.RoomID;
                                objDTO.CompanyID = SessionHelper.CompanyID;
                                objDTO.CreatedBy = SessionHelper.UserID;
                                objDTO.InsertedFrom = "Import";
                                objDTO.GUID = Guid.NewGuid();

                                if (objDTO.ItemGUID.ToString() != Guid.Empty.ToString() && item.BinNumber.Trim() != "")
                                {
                                    CurrentInventoryLocationMasterList.Add(objDTO);
                                }
                                else
                                {
                                    CurrentBlankInventoryLocationList.Add(objDTO);
                                }
                            }
                        }

                        List<InventoryLocationMain> lstreturn = new List<InventoryLocationMain>();
                        if (CurrentInventoryLocationMasterList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.BinMaster.ToString(), CurrentInventoryLocationMasterList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankInventoryLocationList.Count > 0)
                        {
                            foreach (InventoryLocationMain item in CurrentBlankInventoryLocationList)
                            {
                                item.Status = "Fail";
                                item.Reason = "Item does not exist.";
                                lstreturn.Add(item);
                            }
                        }

                        //if (lstreturn.Count == 0)
                        //{
                        //    message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        //    status = ResMessage.SaveMessage;
                        //    ClearCurrentResourceList();
                        //    Session["importedData"] = null;
                        //}
                        //else
                        //{
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        Session["importedData"] = lstreturn;
                        //}
                        if (CurrentInventoryLocationMasterList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        //CacheHelper<IEnumerable<inventory>>.InvalidateCache();
                        //(new CategoryMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);

                    }

                }
                #endregion
                #region InventoryLocationQuantity Master
                if (ImportMastersDTO.TableName.ItemLocationeVMISetup.ToString() == TableName)
                {

                    List<InventoryLocationQuantityMain> CurrentBlankInventoryLocationQtyList = new List<InventoryLocationQuantityMain>();
                    InventoryLocationQuantityMain[] LstInventoryLocation = s.Deserialize<InventoryLocationQuantityMain[]>(para);
                    if (LstInventoryLocation != null && LstInventoryLocation.Length > 0)
                    {
                        CurrentInventoryLocationQuantityList = new List<InventoryLocationQuantityMain>();

                        foreach (InventoryLocationQuantityMain item in LstInventoryLocation)
                        {
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


                            if (objDTO.ItemGUID.ToString() != Guid.Empty.ToString() && item.BinNumber.Trim() != "")
                            {
                                var itemval = CurrentInventoryLocationQuantityList.FirstOrDefault(x => x.ItemGUID == objDTO.ItemGUID && x.BinNumber == objDTO.BinNumber);
                                if (itemval != null)
                                    CurrentInventoryLocationQuantityList.Remove(itemval);
                                CurrentInventoryLocationQuantityList.Add(objDTO);

                            }
                            else
                                CurrentBlankInventoryLocationQtyList.Add(objDTO);
                        }

                        List<InventoryLocationQuantityMain> lstreturn = new List<InventoryLocationQuantityMain>();
                        if (CurrentInventoryLocationQuantityList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.ItemLocationeVMISetup.ToString(), CurrentInventoryLocationQuantityList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankInventoryLocationQtyList.Count > 0)
                        {
                            foreach (InventoryLocationQuantityMain item in CurrentBlankInventoryLocationQtyList)
                            {
                                item.Status = "Fail";
                                item.Reason = "Item does not exist.";
                                lstreturn.Add(item);
                            }
                        }

                        //if (lstreturn.Count == 0)
                        //{
                        //    message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        //    status = ResMessage.SaveMessage;
                        //    ClearCurrentResourceList();
                        //    Session["importedData"] = null;
                        //}
                        //else
                        //{
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        Session["importedData"] = lstreturn;
                        //}
                        if (CurrentInventoryLocationQuantityList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        //CacheHelper<IEnumerable<inventory>>.InvalidateCache();
                        //(new CategoryMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);

                    }

                }
                #endregion
                #region Category Master
                else if (ImportMastersDTO.TableName.CategoryMaster.ToString() == TableName)
                {

                    List<CategoryMasterMain> CurrentBlankCategoryList = new List<CategoryMasterMain>();
                    CategoryMasterMain[] LstCategoryMaster = s.Deserialize<CategoryMasterMain[]>(para);
                    if (LstCategoryMaster != null && LstCategoryMaster.Length > 0)
                    {
                        CurrentCategoryList = new List<CategoryMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.CategoryMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (CategoryMasterMain item in LstCategoryMaster)
                        {
                            CategoryMasterMain objDTO = new CategoryMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.Category = item.Category;
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
                            item.CategoryColor = "";


                            if (item.Category.Trim() != "")
                            {
                                CurrentCategoryList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportCategoryColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportCategoryColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportCategoryColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportCategoryColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportCategoryColumn.UDF5.ToString());
                                //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportCategoryColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankCategoryList.Add(objDTO);
                        }

                        List<CategoryMasterMain> lstreturn = new List<CategoryMasterMain>();
                        if (CurrentCategoryList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.CategoryMaster.ToString(), CurrentCategoryList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankCategoryList.Count > 0)
                        {
                            foreach (CategoryMasterMain item in CurrentBlankCategoryList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentCategoryList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        CacheHelper<IEnumerable<CategoryMasterDTO>>.InvalidateCache();
                        (new CategoryMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);

                    }

                }
                #endregion
                #region Customer Master
                else if (ImportMastersDTO.TableName.CustomerMaster.ToString() == TableName)
                {

                    List<CustomerMasterMain> CurrentBlankCustomerList = new List<CustomerMasterMain>();
                    CustomerMasterMain[] LstCustomerMaster = s.Deserialize<CustomerMasterMain[]>(para);
                    if (LstCustomerMaster != null && LstCustomerMaster.Length > 0)
                    {
                        CurrentCustomerList = new List<CustomerMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.CustomerMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (CustomerMasterMain item in LstCustomerMaster)
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
                            objDTO.Remarks = item.Remarks;
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

                            //MailAddress m = new MailAddress(item.Email); 

                            if (item.Customer.Trim() != "" && item.Account.Trim() != "" && IsValidEmail(item.Email))
                            {
                                CurrentCustomerList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportCustomerColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportCustomerColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportCustomerColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportCustomerColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportCustomerColumn.UDF5.ToString());
                                // CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportCustomerColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankCustomerList.Add(objDTO);
                        }

                        List<CustomerMasterMain> lstreturn = new List<CustomerMasterMain>();
                        if (CurrentCustomerList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.CustomerMaster.ToString(), CurrentCustomerList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankCustomerList.Count > 0)
                        {
                            foreach (CustomerMasterMain item in CurrentBlankCustomerList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentCustomerList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        CacheHelper<IEnumerable<CustomerMasterDTO>>.InvalidateCache();
                        (new CustomerMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);
                    }

                }
                #endregion
                #region Freight Type Master
                else if (ImportMastersDTO.TableName.FreightTypeMaster.ToString() == TableName)
                {
                    List<FreightTypeMasterMain> CurrentBlankFreightTypeList = new List<FreightTypeMasterMain>();
                    FreightTypeMasterMain[] LstFreightTypeMaster = s.Deserialize<FreightTypeMasterMain[]>(para);
                    if (LstFreightTypeMaster != null && LstFreightTypeMaster.Length > 0)
                    {
                        CurrentFreightType = new List<FreightTypeMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.FreightTypeMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (FreightTypeMasterMain item in LstFreightTypeMaster)
                        {
                            FreightTypeMasterMain objDTO = new FreightTypeMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.FreightType = item.FreightType;
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
                            objDTO.Created = DateTimeUtility.DateTimeNow;
                            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            objDTO.LastUpdatedBy = SessionHelper.UserID;
                            objDTO.Room = SessionHelper.RoomID;
                            objDTO.CompanyID = SessionHelper.CompanyID;
                            objDTO.CreatedBy = SessionHelper.UserID;
                            objDTO.GUID = Guid.NewGuid();

                            if (item.FreightType.Trim() != "")
                            {
                                CurrentFreightType.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportFreightTypeColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportFreightTypeColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportFreightTypeColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportFreightTypeColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportFreightTypeColumn.UDF5.ToString());
                                // CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportFreightTypeColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankFreightTypeList.Add(objDTO);
                        }

                        List<FreightTypeMasterMain> lstreturn = new List<FreightTypeMasterMain>();
                        if (CurrentFreightType.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.FreightTypeMaster.ToString(), CurrentFreightType, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankFreightTypeList.Count > 0)
                        {
                            foreach (FreightTypeMasterMain item in CurrentBlankFreightTypeList)
                            {
                                lstreturn.Add(item);
                            }
                        }
                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentFreightType.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        CacheHelper<IEnumerable<FreightTypeMasterDTO>>.InvalidateCache();
                        (new FreightTypeMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);
                    }
                }
                #endregion
                #region G/L Account Master
                else if (ImportMastersDTO.TableName.GLAccountMaster.ToString() == TableName)
                {

                    List<GLAccountMasterMain> CurrentBlankGLAccountList = new List<GLAccountMasterMain>();
                    GLAccountMasterMain[] LstGLAccountMaster = s.Deserialize<GLAccountMasterMain[]>(para);
                    if (LstGLAccountMaster != null && LstGLAccountMaster.Length > 0)
                    {
                        CurrentGLAccountList = new List<GLAccountMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.GLAccountMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (GLAccountMasterMain item in LstGLAccountMaster)
                        {
                            GLAccountMasterMain objDTO = new GLAccountMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.GLAccount = (item.GLAccount == null) ? null : (item.GLAccount.Length > 128 ? item.GLAccount.Substring(0, 128) : item.GLAccount);
                            objDTO.Description = (item.Description == null) ? null : (item.Description.Length > 1024 ? item.Description.Substring(0, 1024) : item.Description);
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
                            objDTO.IsDeleted = false;
                            objDTO.IsArchived = false;
                            objDTO.Created = DateTimeUtility.DateTimeNow;
                            objDTO.Updated = DateTimeUtility.DateTimeNow;
                            objDTO.LastUpdatedBy = SessionHelper.UserID;
                            objDTO.Room = SessionHelper.RoomID;
                            objDTO.CompanyID = SessionHelper.CompanyID;
                            objDTO.CreatedBy = SessionHelper.UserID;
                            objDTO.GUID = Guid.NewGuid();



                            if (item.GLAccount.Trim() != "")
                            {
                                CurrentGLAccountList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportGLAccountColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportGLAccountColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportGLAccountColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportGLAccountColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportGLAccountColumn.UDF5.ToString());
                                // CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportGLAccountColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankGLAccountList.Add(objDTO);
                        }

                        List<GLAccountMasterMain> lstreturn = new List<GLAccountMasterMain>();
                        if (CurrentGLAccountList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.GLAccountMaster.ToString(), CurrentGLAccountList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankGLAccountList.Count > 0)
                        {
                            foreach (GLAccountMasterMain item in CurrentBlankGLAccountList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentGLAccountList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        CacheHelper<IEnumerable<GLAccountMasterDTO>>.InvalidateCache();
                        (new GLAccountMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);
                    }

                }
                #endregion
                #region GXPR Consigned Master
                else if (ImportMastersDTO.TableName.GXPRConsigmentJobMaster.ToString() == TableName)
                {

                    List<GXPRConsignedMasterMain> CurrentBlankGXPRConsignedList = new List<GXPRConsignedMasterMain>();
                    GXPRConsignedMasterMain[] LstGXPRConsignedMaster = s.Deserialize<GXPRConsignedMasterMain[]>(para);
                    if (LstGXPRConsignedMaster != null && LstGXPRConsignedMaster.Length > 0)
                    {
                        CurrentGXPRConsignedList = new List<GXPRConsignedMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.GXPRConsigmentJobMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (GXPRConsignedMasterMain item in LstGXPRConsignedMaster)
                        {
                            GXPRConsignedMasterMain objDTO = new GXPRConsignedMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.GXPRConsigmentJob = (item.GXPRConsigmentJob == null) ? null : (item.GXPRConsigmentJob.Length > 128 ? item.GXPRConsigmentJob.Substring(0, 128) : item.GXPRConsigmentJob);
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
                            objDTO.IsDeleted = false;
                            objDTO.IsArchived = false;
                            objDTO.Created = DateTimeUtility.DateTimeNow;
                            objDTO.Updated = DateTimeUtility.DateTimeNow;
                            objDTO.LastUpdatedBy = SessionHelper.UserID;
                            objDTO.Room = SessionHelper.RoomID;
                            objDTO.CompanyID = SessionHelper.CompanyID;
                            objDTO.CreatedBy = SessionHelper.UserID;
                            objDTO.GUID = Guid.NewGuid();



                            if (item.GXPRConsigmentJob.Trim() != "")
                            {
                                CurrentGXPRConsignedList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportGXPRConsignedColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportGXPRConsignedColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportGXPRConsignedColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportGXPRConsignedColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportGXPRConsignedColumn.UDF5.ToString());
                                // CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportGXPRConsignedColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankGXPRConsignedList.Add(objDTO);
                        }

                        List<GXPRConsignedMasterMain> lstreturn = new List<GXPRConsignedMasterMain>();
                        if (CurrentGXPRConsignedList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.GXPRConsigmentJobMaster.ToString(), CurrentGXPRConsignedList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankGXPRConsignedList.Count > 0)
                        {
                            foreach (GXPRConsignedMasterMain item in CurrentBlankGXPRConsignedList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        //if (CurrentGXPRConsignedList.Count() != lstreturn.Where(l => l.Status.ToLower() == "success").Count())
                        //{
                        //    allSuccesfulRecords = false;
                        //}
                        CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.InvalidateCache();
                        (new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);
                    }

                }
                #endregion
                #region Job Type Master
                else if (ImportMastersDTO.TableName.JobTypeMaster.ToString() == TableName)
                {

                    List<JobTypeMasterMain> CurrentBlankJobTypeList = new List<JobTypeMasterMain>();
                    JobTypeMasterMain[] LstJobTypeMaster = s.Deserialize<JobTypeMasterMain[]>(para);
                    if (LstJobTypeMaster != null && LstJobTypeMaster.Length > 0)
                    {
                        CurrentJobTypeList = new List<JobTypeMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.JobTypeMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (JobTypeMasterMain item in LstJobTypeMaster)
                        {
                            JobTypeMasterMain objDTO = new JobTypeMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.JobType = (item.JobType == null) ? null : (item.JobType.Length > 128 ? item.JobType.Substring(0, 128) : item.JobType);
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
                            objDTO.IsDeleted = false;
                            objDTO.IsArchived = false;
                            objDTO.Created = DateTimeUtility.DateTimeNow;
                            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            objDTO.LastUpdatedBy = SessionHelper.UserID;
                            objDTO.Room = SessionHelper.RoomID;
                            objDTO.CompanyID = SessionHelper.CompanyID;
                            objDTO.CreatedBy = SessionHelper.UserID;
                            objDTO.GUID = Guid.NewGuid();



                            if (item.JobType.Trim() != "")
                            {
                                CurrentJobTypeList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportJobTypeColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportJobTypeColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportJobTypeColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportJobTypeColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportJobTypeColumn.UDF5.ToString());
                                // CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportJobTypeColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankJobTypeList.Add(objDTO);
                        }

                        List<JobTypeMasterMain> lstreturn = new List<JobTypeMasterMain>();
                        if (CurrentJobTypeList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.JobTypeMaster.ToString(), CurrentJobTypeList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankJobTypeList.Count > 0)
                        {
                            foreach (JobTypeMasterMain item in CurrentBlankJobTypeList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        //if (LstJobTypeMaster.Count() != lstreturn.Where(l => l.Status.ToLower() == "success").Count())
                        //{
                        //    allSuccesfulRecords = false;
                        //}
                        CacheHelper<IEnumerable<JobTypeMasterDTO>>.InvalidateCache();
                        (new JobTypeMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);
                    }

                }
                #endregion
                #region Ship Via Master
                else if (ImportMastersDTO.TableName.ShipViaMaster.ToString() == TableName)
                {

                    List<ShipViaMasterMain> CurrentBlankShipViaList = new List<ShipViaMasterMain>();
                    ShipViaMasterMain[] LstShipViaMaster = s.Deserialize<ShipViaMasterMain[]>(para);
                    if (LstShipViaMaster != null && LstShipViaMaster.Length > 0)
                    {
                        CurrentShipViaList = new List<ShipViaMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.ShipViaMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (ShipViaMasterMain item in LstShipViaMaster)
                        {
                            ShipViaMasterMain objDTO = new ShipViaMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.ShipVia = (item.ShipVia == null) ? null : (item.ShipVia.Length > 128 ? item.ShipVia.Substring(0, 128) : item.ShipVia);
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


                            if (item.ShipVia.Trim() != "")
                            {
                                CurrentShipViaList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportShipViaColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportShipViaColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportShipViaColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportShipViaColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportShipViaColumn.UDF5.ToString());
                                // CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportShipViaColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankShipViaList.Add(objDTO);
                        }

                        List<ShipViaMasterMain> lstreturn = new List<ShipViaMasterMain>();
                        if (CurrentShipViaList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.ShipViaMaster.ToString(), CurrentShipViaList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankShipViaList.Count > 0)
                        {
                            foreach (ShipViaMasterMain item in CurrentBlankShipViaList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentShipViaList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        CacheHelper<IEnumerable<ShipViaDTO>>.InvalidateCache();
                        (new ShipViaDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);
                    }

                }
                #endregion
                #region Technician Master
                else if (ImportMastersDTO.TableName.TechnicianMaster.ToString() == TableName)
                {

                    List<TechnicianMasterMain> CurrentBlankTechnicianList = new List<TechnicianMasterMain>();
                    TechnicianMasterMain[] LstTechnicianMaster = s.Deserialize<TechnicianMasterMain[]>(para);
                    if (LstTechnicianMaster != null && LstTechnicianMaster.Length > 0)
                    {
                        CurrentTechnicianList = new List<TechnicianMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.TechnicianMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (TechnicianMasterMain item in LstTechnicianMaster)
                        {
                            TechnicianMasterMain objDTO = new TechnicianMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.Technician = (item.Technician == null) ? null : (item.Technician.Length > 128 ? item.Technician.Substring(0, 128) : item.Technician);
                            objDTO.TechnicianCode = (item.TechnicianCode.Replace(" ", "") == null) ? null : (item.TechnicianCode.Replace(" ", "").Length > 128 ? item.TechnicianCode.Replace(" ", "").Substring(0, 128) : item.TechnicianCode.Replace(" ", ""));
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
                            objDTO.IsDeleted = false;
                            objDTO.IsArchived = false;
                            objDTO.Created = DateTimeUtility.DateTimeNow;
                            objDTO.Updated = DateTimeUtility.DateTimeNow;
                            objDTO.LastUpdatedBy = SessionHelper.UserID;
                            objDTO.Room = SessionHelper.RoomID;
                            objDTO.CompanyID = SessionHelper.CompanyID;
                            objDTO.CreatedBy = SessionHelper.UserID;
                            objDTO.GUID = Guid.NewGuid();



                            if (item.Technician.Trim() != "")
                            {
                                CurrentTechnicianList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportTechnicianColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportTechnicianColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportTechnicianColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportTechnicianColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportTechnicianColumn.UDF5.ToString());
                                // CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportTechnicianColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankTechnicianList.Add(objDTO);
                        }

                        List<TechnicianMasterMain> lstreturn = new List<TechnicianMasterMain>();
                        if (CurrentTechnicianList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.TechnicianMaster.ToString(), CurrentTechnicianList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankTechnicianList.Count > 0)
                        {
                            foreach (TechnicianMasterMain item in CurrentBlankTechnicianList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentTechnicianList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        CacheHelper<IEnumerable<TechnicianMasterDTO>>.InvalidateCache();
                        (new TechnicialMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);
                    }

                }
                #endregion
                #region Manufacturer Master
                else if (ImportMastersDTO.TableName.ManufacturerMaster.ToString() == TableName)
                {

                    List<ManufacturerMasterMain> CurrentBlankManufacturerList = new List<ManufacturerMasterMain>();
                    ManufacturerMasterMain[] LstManufacturerMaster = s.Deserialize<ManufacturerMasterMain[]>(para);
                    if (LstManufacturerMaster != null && LstManufacturerMaster.Length > 0)
                    {
                        CurrentManufacturerList = new List<ManufacturerMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.ManufacturerMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (ManufacturerMasterMain item in LstManufacturerMaster)
                        {
                            ManufacturerMasterMain objDTO = new ManufacturerMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.Manufacturer = (item.Manufacturer == null) ? null : (item.Manufacturer.Length > 128 ? item.Manufacturer.Substring(0, 128) : item.Manufacturer);
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



                            if (item.Manufacturer.Trim() != "")
                            {
                                CurrentManufacturerList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportManufacturerColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportManufacturerColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportManufacturerColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportManufacturerColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportManufacturerColumn.UDF5.ToString());
                                // CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportManufacturerColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankManufacturerList.Add(objDTO);
                        }

                        List<ManufacturerMasterMain> lstreturn = new List<ManufacturerMasterMain>();
                        if (CurrentManufacturerList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.ManufacturerMaster.ToString(), CurrentManufacturerList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankManufacturerList.Count > 0)
                        {
                            foreach (ManufacturerMasterMain item in CurrentBlankManufacturerList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentManufacturerList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        CacheHelper<IEnumerable<ManufacturerMasterDTO>>.InvalidateCache();
                        (new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);
                    }

                }
                #endregion
                #region MeasurementTerm Master
                else if (ImportMastersDTO.TableName.MeasurementTermMaster.ToString() == TableName)
                {

                    List<MeasurementTermMasterMain> CurrentBlankMeasurementList = new List<MeasurementTermMasterMain>();
                    MeasurementTermMasterMain[] LstManufacturerMaster = s.Deserialize<MeasurementTermMasterMain[]>(para);
                    if (LstManufacturerMaster != null && LstManufacturerMaster.Length > 0)
                    {
                        CurrentMeasurementTermList = new List<MeasurementTermMasterMain>();
                        //lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.ManufacturerMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        lst = obj.GetUDFList(SessionHelper.CompanyID, "MeasurementTerm", UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (MeasurementTermMasterMain item in LstManufacturerMaster)
                        {
                            MeasurementTermMasterMain objDTO = new MeasurementTermMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.MeasurementTerm = (item.MeasurementTerm == null) ? null : (item.MeasurementTerm.Length > 128 ? item.MeasurementTerm.Substring(0, 128) : item.MeasurementTerm);
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
                            objDTO.IsDeleted = false;
                            objDTO.IsArchived = false;
                            objDTO.Created = DateTimeUtility.DateTimeNow;
                            objDTO.Updated = DateTimeUtility.DateTimeNow;
                            objDTO.LastUpdatedBy = SessionHelper.UserID;
                            objDTO.Room = SessionHelper.RoomID;
                            objDTO.CompanyID = SessionHelper.CompanyID;
                            objDTO.CreatedBy = SessionHelper.UserID;
                            objDTO.GUID = Guid.NewGuid();



                            if (item.MeasurementTerm.Trim() != "")
                            {
                                CurrentMeasurementTermList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportMeasurementTermColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportMeasurementTermColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportMeasurementTermColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportMeasurementTermColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportMeasurementTermColumn.UDF5.ToString());
                                //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportMeasurementTermColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankMeasurementList.Add(objDTO);
                        }

                        List<MeasurementTermMasterMain> lstreturn = new List<MeasurementTermMasterMain>();
                        if (CurrentMeasurementTermList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.MeasurementTermMaster.ToString(), CurrentMeasurementTermList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankMeasurementList.Count > 0)
                        {
                            foreach (MeasurementTermMasterMain item in CurrentBlankMeasurementList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentMeasurementTermList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.InvalidateCache();
                        (new MeasurementTermDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);

                    }

                }
                #endregion
                #region Units Master
                else if (ImportMastersDTO.TableName.UnitMaster.ToString() == TableName)
                {

                    List<UnitMasterMain> CurrentBlankUnitsList = new List<UnitMasterMain>();
                    UnitMasterMain[] LstUnitMaster = s.Deserialize<UnitMasterMain[]>(para);
                    if (LstUnitMaster != null && LstUnitMaster.Length > 0)
                    {
                        CurrentUnitList = new List<UnitMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.UnitMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();
                        decimal opthous;
                        Int64 SerialNo; Int64 Year; Int64 MarkupParts; Int64 MarkupLabour;
                        foreach (UnitMasterMain item in LstUnitMaster)
                        {
                            UnitMasterMain objDTO = new UnitMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.Unit = (item.Unit == null) ? null : (item.Unit.Length > 128 ? item.Unit.Substring(0, 128) : item.Unit);
                            objDTO.Description = (item.Description == null) ? null : (item.Description.Length > 1024 ? item.Description.Substring(0, 1024) : item.Description);
                            objDTO.Odometer = (item.Odometer == null) ? null : (item.Odometer.Length > 128 ? item.Odometer.Substring(0, 128) : item.Odometer);
                            objDTO.OdometerUpdate = (item.Odometer == null) ? (DateTime?)null : DateTimeUtility.DateTimeNow;

                            decimal.TryParse(item.OpHours.ToString(), out opthous);
                            objDTO.OpHours = (item.OpHours == null) ? (decimal?)null : opthous;
                            objDTO.OpHoursUpdate = (item.OpHours == null) ? (DateTime?)null : DateTimeUtility.DateTimeNow;

                            Int64.TryParse(item.SerialNo.ToString(), out SerialNo);
                            objDTO.SerialNo = (item.SerialNo == null) ? (Int64?)null : SerialNo;

                            Int64.TryParse(item.Year.ToString(), out Year);
                            objDTO.Year = (item.Year == null) ? (Int64?)null : Year;
                            objDTO.Make = (item.Make == null) ? null : (item.Make.Length > 128 ? item.Make.Substring(0, 128) : item.Make);
                            objDTO.Model = (item.Model == null) ? null : (item.Model.Length > 128 ? item.Model.Substring(0, 128) : item.Model);
                            objDTO.Plate = (item.Plate == null) ? null : (item.Plate.Length > 128 ? item.Plate.Substring(0, 128) : item.Plate);
                            objDTO.EngineModel = (item.EngineModel == null) ? null : (item.EngineModel.Length > 128 ? item.EngineModel.Substring(0, 128) : item.EngineModel);
                            objDTO.EngineSerialNo = (item.EngineSerialNo == null) ? null : (item.EngineSerialNo.Length > 128 ? item.EngineSerialNo.Substring(0, 128) : item.EngineSerialNo);

                            Int64.TryParse(item.MarkupParts.ToString(), out MarkupParts);
                            objDTO.MarkupParts = (item.MarkupParts == null) ? (Int64?)null : MarkupParts;

                            Int64.TryParse(item.MarkupLabour.ToString(), out MarkupLabour);
                            objDTO.MarkupLabour = (item.MarkupLabour == null) ? (Int64?)null : MarkupLabour;

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




                            if (item.Unit.Trim() != "")
                            {
                                CurrentUnitList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportUnitsColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportUnitsColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportUnitsColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportUnitsColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportUnitsColumn.UDF5.ToString());
                                //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportUnitsColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankUnitsList.Add(objDTO);
                        }

                        List<UnitMasterMain> lstreturn = new List<UnitMasterMain>();
                        if (CurrentUnitList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.UnitMaster.ToString(), CurrentUnitList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankUnitsList.Count > 0)
                        {
                            foreach (UnitMasterMain item in CurrentBlankUnitsList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentUnitList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }

                        CacheHelper<IEnumerable<UnitMasterDTO>>.InvalidateCache();
                        (new UnitMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);
                    }

                }
                #endregion
                #region Supplier Master
                else if (ImportMastersDTO.TableName.SupplierMaster.ToString() == TableName)
                {

                    List<SupplierMasterMain> CurrentBlankSupplierList = new List<SupplierMasterMain>();
                    SupplierMasterMain[] LstSupplierMaster = s.Deserialize<SupplierMasterMain[]>(para);
                    if (LstSupplierMaster != null && LstSupplierMaster.Length > 0)
                    {
                        CurrentSupplierList = new List<SupplierMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.SupplierMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        //bool IsEmailPOInBody = false; bool IsEmailPOInPDF = false; bool IsEmailPOInCSV = false; bool IsEmailPOInX12 = false;
                        foreach (SupplierMasterMain item in LstSupplierMaster)
                        {
                            SupplierMasterMain objDTO = new SupplierMasterMain();
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
                            objDTO.BlanketPONumber = (item.BlanketPONumber == null) ? null : (item.BlanketPONumber.Length > 255 ? item.BlanketPONumber.Substring(0, 255) : item.BlanketPONumber);

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

                            if (item.SupplierName.Trim() != "")
                            {
                                CurrentSupplierList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportSupplierColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportSupplierColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportSupplierColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportSupplierColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportSupplierColumn.UDF5.ToString());
                                //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportSupplierColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankSupplierList.Add(objDTO);
                        }

                        List<SupplierMasterMain> lstreturn = new List<SupplierMasterMain>();
                        if (CurrentSupplierList.Count > 0)
                            //lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.SupplierMaster.ToString(), CurrentSupplierList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                            lstreturn = objImport.BulkInsertWithChiled(ImportMastersDTO.TableName.SupplierMaster.ToString(), CurrentSupplierList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankSupplierList.Count > 0)
                        {
                            foreach (SupplierMasterMain item in CurrentBlankSupplierList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentSupplierList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }

                        CacheHelper<IEnumerable<SupplierMasterDTO>>.InvalidateCache();
                        (new SupplierMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);
                    }

                }
                #endregion
                #region Item Master
                else if (ImportMastersDTO.TableName.ItemMaster.ToString() == TableName)
                {

                    List<BOMItemMasterMain> CurrentBlankItemList = new List<BOMItemMasterMain>();
                    //List<BOMItemMasterMain> CurrentErrorItemList = new List<BOMItemMasterMain>();
                    BOMItemMasterMain[] LstItemMaster = s.Deserialize<BOMItemMasterMain[]>(para);
                    if (LstItemMaster != null && LstItemMaster.Length > 0)
                    {
                        CurrentItemList = new List<BOMItemMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.ItemMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        //bool IsEmailPOInBody = false; bool IsEmailPOInPDF = false; bool IsEmailPOInCSV = false; bool IsEmailPOInX12 = false;
                        foreach (BOMItemMasterMain item in LstItemMaster)
                        {
                            BOMItemMasterMain objDTO = new BOMItemMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.ItemNumber = (item.ItemNumber == null) ? null : (item.ItemNumber.Length > 255 ? item.ItemNumber.Substring(0, 255) : item.ItemNumber);
                            objDTO.ManufacturerName = item.ManufacturerName;
                            //Wi-1505
                            if (!string.IsNullOrWhiteSpace(item.ManufacturerName))
                            {
                                objDTO.ManufacturerID = item.ManufacturerName == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.ManufacturerMaster, item.ManufacturerName);
                            }
                            else
                            {
                                objDTO.ManufacturerID = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName).GetORInsertBlankManuFacID(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                            }
                            objDTO.ManufacturerNumber = (item.ManufacturerNumber == null) ? null : (item.ManufacturerNumber.Length > 20 ? item.ManufacturerNumber.Substring(0, 20) : item.ManufacturerNumber);
                            objDTO.Link1 = item.Link1;
                            objDTO.Link2 = item.Link2;
                            objDTO.ImageType = "ExternalImage";
                            objDTO.ItemImageExternalURL = item.ItemImageExternalURL;
                            if (string.IsNullOrWhiteSpace(item.ItemImageExternalURL) && (!string.IsNullOrEmpty(item.ImagePath)))
                            {
                                objDTO.ImageType = "ImagePath";
                            }
                            objDTO.ItemDocExternalURL = item.ItemDocExternalURL;

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
                                objDTO.SupplierPartNo = (item.SupplierPartNo == null) ? null : (item.SupplierPartNo.Length > 20 ? item.SupplierPartNo.Substring(0, 20) : item.SupplierPartNo);
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
                            objDTO.Cost = item.Cost;
                            objDTO.SellPrice = item.SellPrice;
                            objDTO.Markup = item.Markup;
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

                            if (item.Cost.HasValue && item.SellPrice.HasValue && item.Markup.HasValue)
                            {
                                objDTO.Cost = item.Cost;
                                if (item.SellPrice != 0)
                                    objDTO.SellPrice = item.SellPrice;
                                else
                                    objDTO.SellPrice = item.Cost;
                                if (objDTO.Cost > 0)
                                {
                                    //objDTO.Markup = ((objDTO.SellPrice * 100) / objDTO.Cost) - 100;
                                    objDTO.Markup = null;
                                }
                                else
                                {
                                    objDTO.Markup = null;
                                }

                            }
                            else if (item.Cost.HasValue && item.SellPrice.HasValue)
                            {
                                objDTO.Cost = item.Cost;
                                if (item.SellPrice != 0)
                                    objDTO.SellPrice = item.SellPrice;
                                else
                                    objDTO.SellPrice = item.Cost;
                                if (objDTO.Cost > 0)
                                {
                                    //objDTO.Markup = ((objDTO.SellPrice * 100) / objDTO.Cost)-100;
                                    objDTO.Markup = null;
                                }
                                else
                                {
                                    objDTO.Markup = null;
                                }

                            }
                            else if (item.Cost.HasValue && item.Markup.HasValue)
                            {
                                objDTO.Cost = item.Cost;
                                objDTO.SellPrice = objDTO.Cost + ((objDTO.Cost * objDTO.Markup) / 100);
                                if (objDTO.Markup == 0)
                                    objDTO.Markup = null;
                                else
                                    objDTO.Markup = item.Markup;
                            }
                            else if (item.Cost.HasValue)
                            {
                                objDTO.Cost = item.Cost;
                                objDTO.Markup = null;
                                objDTO.SellPrice = item.Cost;
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
                            objDTO.OnHandQuantity = 0;// item.OnHandQuantity;

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
                            objDTO.DefaultLocation = 0;// GetIDs(ImportMastersDTO.TableName.BinMaster, item.InventryLocation);
                            objDTO.InventryLocation = item.InventryLocation;
                            objDTO.InventoryClassification = Convert.ToInt32(GetIDs(ImportMastersDTO.TableName.InventoryClassificationMaster, item.InventoryClassificationName));
                            objDTO.InventoryClassificationName = item.InventoryClassificationName;
                            objDTO.ItemTypeName = item.ItemTypeName;
                            objDTO.SerialNumberTracking = item.SerialNumberTracking;
                            objDTO.LotNumberTracking = item.LotNumberTracking;
                            objDTO.DateCodeTracking = item.DateCodeTracking;
                            objDTO.ItemType = item.ItemTypeName == "Item" ? 1 : item.ItemTypeName == "Quick List" ? 2 : item.ItemTypeName == "Kit" ? 3 : item.ItemTypeName == "Labor" ? 4 : 1;
                            objDTO.ImagePath = item.ImagePath;
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
                            lstItemGUID.Add(objDTO.GUID);

                            #region ///Commented codes ///
                            //var errItem = CurrentErrorItemList.FirstOrDefault(x => x.ItemNumber == item.ItemNumber);
                            //if (errItem != null)
                            //{
                            //    continue;
                            //}

                            //string strStatus = string.Empty;
                            //string strStatusManufac = string.Empty;

                            //if (!string.IsNullOrWhiteSpace(objDTO.SupplierPartNo))
                            //{
                            //    List<BOMItemMasterMain> duplicateSupplierNo = new List<BOMItemMasterMain>();
                            //    duplicateSupplierNo = LstItemMaster.Where(x => x.SupplierPartNo == objDTO.SupplierPartNo && x.ItemNumber != objDTO.ItemNumber
                            //        && x.Status != "Success").ToList();

                            //    foreach (BOMItemMasterMain SupplierNo in duplicateSupplierNo)
                            //    {
                            //        var objItem = CurrentErrorItemList.FirstOrDefault(x => x.ItemNumber == SupplierNo.ItemNumber);
                            //        if (objItem == null)
                            //        {
                            //            SupplierNo.Status = "Fail";
                            //            SupplierNo.Reason = "Duplicate Supplier Part No";
                            //            CurrentErrorItemList.Add(SupplierNo);
                            //        }
                            //    }

                            //    ItemSupplierDetailsDAL oISD = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                            //    strStatus = oISD.CheckForDuplicateSupplierPartNo(objDTO.SupplierPartNo, objDTO.SupplierID ?? 0, objDTO.ItemNumber, SessionHelper.RoomID, SessionHelper.CompanyID);
                            //}

                            //if (!string.IsNullOrWhiteSpace(objDTO.ManufacturerNumber))
                            //{
                            //    List<BOMItemMasterMain> duplicateManufacNo = new List<BOMItemMasterMain>();
                            //    duplicateManufacNo = LstItemMaster.Where(x => x.ManufacturerNumber == objDTO.ManufacturerNumber && x.ItemNumber != objDTO.ItemNumber
                            //        && x.Status != "Success").ToList();

                            //    foreach (BOMItemMasterMain ManufacNo in duplicateManufacNo)
                            //    {
                            //        var objItem = CurrentErrorItemList.FirstOrDefault(x => x.ItemNumber == ManufacNo.ItemNumber);
                            //        if (objItem == null)
                            //        {
                            //            ManufacNo.Status = "Fail";
                            //            ManufacNo.Reason = "Duplicate Manufacturer Number";
                            //            CurrentErrorItemList.Add(ManufacNo);
                            //        }
                            //    }

                            //    ItemManufacturerDetailsDAL oIMD = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                            //    strStatusManufac = oIMD.CheckForDuplicateManufacturerNo(objDTO.ManufacturerNumber, objDTO.ManufacturerID ?? 0, objDTO.ItemNumber, SessionHelper.RoomID, SessionHelper.CompanyID);
                            //}

                            //if (strStatus == "duplicate")
                            //{
                            //    item.Status = "Fail";
                            //    item.Reason = "Duplicate SupplierPartNo";
                            //    CurrentErrorItemList.Add(item);
                            //}
                            //else if (strStatusManufac == "duplicate")
                            //{
                            //    item.Status = "Fail";
                            //    item.Reason = "Duplicate Manufacturer Number";
                            //    CurrentErrorItemList.Add(item);
                            //}
                            //else 
                            #endregion

                            if (item.ItemNumber.Trim() != "")
                            {
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
                            else
                                CurrentBlankItemList.Add(item);
                        }

                        List<BOMItemMasterMain> lstreturn = new List<BOMItemMasterMain>();
                        if (CurrentItemList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.ItemMaster.ToString(), CurrentItemList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);

                        //if (CurrentErrorItemList.Count > 0)
                        //{
                        //    foreach (BOMItemMasterMain item in CurrentErrorItemList)
                        //    {
                        //        lstreturn.Add(item);
                        //    }
                        //}

                        if (CurrentBlankItemList.Count > 0)
                        {
                            foreach (BOMItemMasterMain item in CurrentBlankItemList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            savedOnlyitemIds = string.Join(",", lstreturn.Where(i => i.ID != 0 && (!string.IsNullOrEmpty(i.ImagePath))).Select(p => p.ID.ToString()));
                            foreach (BOMItemMasterMain b in lstreturn.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.ImagePath))))
                            {
                                long id = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetIDByItemNumber(Convert.ToString(b.ItemNumber), Convert.ToInt64(b.Room), Convert.ToInt64(b.CompanyID));
                                savedOnlyitemIds += "," + id;
                            }
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentItemList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }

                        lstItemGUID = new List<Guid>();
                        foreach (BOMItemMasterMain objitemguid in lstreturn)
                        {
                            if (objitemguid.Status.ToLower() == "success")
                            {
                                lstItemGUID.Add(objitemguid.GUID);
                            }
                        }

                    }

                }
                #endregion
                #region LocationMaster
                if (ImportMastersDTO.TableName.LocationMaster.ToString() == TableName)
                {
                    List<BinMasterMain> CurrentBlankLocationList = new List<BinMasterMain>();
                    BinMasterMain[] LstLocationMaster = s.Deserialize<BinMasterMain[]>(para);
                    if (LstLocationMaster != null && LstLocationMaster.Length > 0)
                    {
                        CurrentLocationList = new List<BinMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.BinMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (BinMasterMain item in LstLocationMaster)
                        {
                            BinMasterMain objDTO = new BinMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.BinNumber = item.BinNumber;
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
                            if (item.BinNumber.Trim() != "")
                            {
                                CurrentLocationList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportLocationColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportLocationColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportLocationColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportLocationColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportLocationColumn.UDF5.ToString());
                                //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportBinColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankLocationList.Add(objDTO);
                        }

                        List<BinMasterMain> lstreturn = new List<BinMasterMain>();
                        if (CurrentLocationList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.LocationMaster.ToString(), CurrentLocationList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankLocationList.Count > 0)
                        {
                            foreach (BinMasterMain item in CurrentBlankLocationList)
                            {
                                lstreturn.Add(item);
                            }
                        }
                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentLocationList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        CacheHelper<IEnumerable<LocationMasterDTO>>.InvalidateCache();
                        (new LocationMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);
                    }
                }
                #endregion
                #region ToolCategoryMaster
                if (ImportMastersDTO.TableName.ToolCategoryMaster.ToString() == TableName)
                {
                    List<ToolCategoryMasterMain> CurrentBlankToolCategoryList = new List<ToolCategoryMasterMain>();
                    ToolCategoryMasterMain[] LstToolCategoryMaster = s.Deserialize<ToolCategoryMasterMain[]>(para);
                    if (LstToolCategoryMaster != null && LstToolCategoryMaster.Length > 0)
                    {
                        CurrentToolCategoryList = new List<ToolCategoryMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.ToolCategoryMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (ToolCategoryMasterMain item in LstToolCategoryMaster)
                        {
                            ToolCategoryMasterMain objDTO = new ToolCategoryMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.ToolCategory = item.ToolCategory;
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

                            if (item.ToolCategory.Trim() != "")
                            {
                                CurrentToolCategoryList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportToolCategoryColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportToolCategoryColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportToolCategoryColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportToolCategoryColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportToolCategoryColumn.UDF5.ToString());
                                //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportBinColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankToolCategoryList.Add(objDTO);
                        }

                        List<ToolCategoryMasterMain> lstreturn = new List<ToolCategoryMasterMain>();
                        if (CurrentToolCategoryList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.ToolCategoryMaster.ToString(), CurrentToolCategoryList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankToolCategoryList.Count > 0)
                        {
                            foreach (ToolCategoryMasterMain item in CurrentBlankToolCategoryList)
                            {
                                lstreturn.Add(item);
                            }
                        }
                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentToolCategoryList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        CacheHelper<IEnumerable<ToolCategoryMasterDTO>>.InvalidateCache();
                        (new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);
                    }
                }
                #endregion
                #region CostUOM Master
                else if (ImportMastersDTO.TableName.CostUOMMaster.ToString() == TableName)
                {
                    List<CostUOMMasterMain> CurrentBlankCategoryList = new List<CostUOMMasterMain>();
                    CostUOMMasterMain[] LstCategoryMaster = s.Deserialize<CostUOMMasterMain[]>(para);
                    if (LstCategoryMaster != null && LstCategoryMaster.Length > 0)
                    {
                        CurrentCostUOMList = new List<CostUOMMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.CostUOMMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (CostUOMMasterMain item in LstCategoryMaster)
                        {
                            CostUOMMasterMain objDTO = new CostUOMMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.CostUOM = item.CostUOM;
                            objDTO.CostUOMValue = item.CostUOMValue;
                            objDTO.UDF1 = item.UDF1;
                            objDTO.UDF2 = item.UDF2;
                            objDTO.UDF3 = item.UDF3;
                            objDTO.UDF4 = item.UDF4;
                            objDTO.UDF5 = item.UDF5;
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
                            if (item.CostUOM.Trim() != "")
                            {
                                CurrentCostUOMList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportCostUOMColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportCostUOMColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportCostUOMColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportCostUOMColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportCostUOMColumn.UDF5.ToString());
                                //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportCategoryColumn.UDF6.ToString());

                            }
                            else
                                CurrentBlankCategoryList.Add(objDTO);
                        }

                        List<CostUOMMasterMain> lstreturn = new List<CostUOMMasterMain>();
                        if (CurrentCostUOMList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.CostUOMMaster.ToString(), CurrentCostUOMList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankCategoryList.Count > 0)
                        {
                            foreach (CostUOMMasterMain item in CurrentBlankCategoryList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentCostUOMList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        CacheHelper<IEnumerable<CostUOMMasterDTO>>.InvalidateCache();
                        (new CostUOMMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);

                    }

                }
                #endregion
                #region Inventory Classification Master

                else if (ImportMastersDTO.TableName.InventoryClassificationMaster.ToString() == TableName)
                {
                    List<InventoryClassificationMasterMain> CurrentBlankCategoryList = new List<InventoryClassificationMasterMain>();
                    InventoryClassificationMasterMain[] LstCategoryMaster = s.Deserialize<InventoryClassificationMasterMain[]>(para);
                    if (LstCategoryMaster != null && LstCategoryMaster.Length > 0)
                    {
                        CurrentInventoryClassificationList = new List<InventoryClassificationMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.InventoryClassificationMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (InventoryClassificationMasterMain item in LstCategoryMaster)
                        {
                            InventoryClassificationMasterMain objDTO = new InventoryClassificationMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.BaseOfInventory = item.BaseOfInventory;
                            objDTO.InventoryClassification = item.InventoryClassification;
                            objDTO.RangeStart = item.RangeStart;
                            objDTO.RangeEnd = item.RangeEnd;
                            objDTO.UDF1 = item.UDF1;
                            objDTO.UDF2 = item.UDF2;
                            objDTO.UDF3 = item.UDF3;
                            objDTO.UDF4 = item.UDF4;
                            objDTO.UDF5 = item.UDF5;
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
                            if (item.InventoryClassification.Trim() != "")
                            {
                                CurrentInventoryClassificationList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportInventoryClassificationColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportInventoryClassificationColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportInventoryClassificationColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportInventoryClassificationColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportInventoryClassificationColumn.UDF5.ToString());
                                //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportCategoryColumn.UDF6.ToString());
                            }
                            else
                                CurrentBlankCategoryList.Add(objDTO);
                        }

                        List<InventoryClassificationMasterMain> lstreturn = new List<InventoryClassificationMasterMain>();
                        if (CurrentInventoryClassificationList.Count > 0)
                            //GetValidateList(CurrentInventoryClassificationList, TableName);
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.InventoryClassificationMaster.ToString(), CurrentInventoryClassificationList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankCategoryList.Count > 0)
                        {
                            foreach (InventoryClassificationMasterMain item in CurrentBlankCategoryList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentInventoryClassificationList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        CacheHelper<IEnumerable<InventoryClassificationMasterDTO>>.InvalidateCache();
                        (new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);

                    }

                }
                #endregion

                #region ToolMaintainance

                 else if (ImportMastersDTO.TableName.ToolMaster.ToString() == TableName)
                {
                    List<ToolMasterMain> CurrentBlankToolMasterMain = new List<ToolMasterMain>();
                    ToolMasterMain[] LstToolMasterMain = s.Deserialize<ToolMasterMain[]>(para);
                    if (LstToolMasterMain != null && LstToolMasterMain.Length > 0)
                    {
                        CurrentToolList = new List<ToolMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.ToolMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();
                        List<TechnicianMasterDTO> objTechnicialMasterDALList = new List<TechnicianMasterDTO>();
                        TechnicialMasterDAL objTechnicialMasterDAL = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
                        objTechnicialMasterDALList = objTechnicialMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                        foreach (ToolMasterMain toolList in LstToolMasterMain.GroupBy(l => l.Technician).Select(g => g.First()).ToList())
                        {
                            if (toolList.Technician.ToLower().Trim() != string.Empty)
                            {
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
                        List<ToolCategoryMasterDTO> objToolCategoryMasterDTOList = new List<ToolCategoryMasterDTO>();
                        ToolCategoryMasterDAL objToolCategoryMasterDAL = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
                        objToolCategoryMasterDTOList = objToolCategoryMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                        foreach (ToolMasterMain toolList in LstToolMasterMain.GroupBy(l => l.ToolCategory).Select(g => g.First()).ToList())
                        {
                            if (toolList.ToolCategory.ToLower().Trim() != string.Empty)
                            {
                                if ((from p in objToolCategoryMasterDTOList
                                     where (p.ToolCategory.ToLower().Trim() == (toolList.ToolCategory.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                     select p).Any())
                                {

                                }
                                else
                                {
                                    ToolCategoryMasterDAL objDAL = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
                                    ToolCategoryMasterDTO objDTO = new ToolCategoryMasterDTO();
                                    Int64 ToolCategoryID = GetIDs(ImportMastersDTO.TableName.ToolCategoryMaster, toolList.ToolCategory);
                                    objDTO = objToolCategoryMasterDAL.GetRecord(ToolCategoryID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                                    objToolCategoryMasterDTOList.Add(objDTO);

                                }
                            }
                        }
                        List<LocationMasterDTO> objLocationMasterDTOList = new List<LocationMasterDTO>();
                        LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                        objLocationMasterDTOList = objLocationMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                        foreach (ToolMasterMain toolList in LstToolMasterMain.GroupBy(l => l.Location).Select(g => g.First()).ToList())
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
                        bool SaveToolList = true;
                        ToolMasterDAL objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                        ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
                        foreach (ToolMasterMain item in LstToolMasterMain)
                        {
                            SaveToolList = true;
                            ToolMasterMain objDTO = new ToolMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.ToolName = item.ToolName;

                            objDTO.Description = item.Description;
                            objDTO.ToolCategory = item.ToolCategory;
                            //objDTO.ToolCategoryID = item.ToolCategory == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.ToolCategoryMaster, item.ToolCategory);
                            if (!string.IsNullOrWhiteSpace(item.ToolCategory))
                            {
                                if ((from p in objToolCategoryMasterDTOList
                                     where (p.ToolCategory.ToLower().Trim() == (item.ToolCategory.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                                     select p).Any())
                                {
                                    objDTO.ToolCategoryID = (from p in objToolCategoryMasterDTOList
                                                             where (p.ToolCategory.ToLower().Trim() == (item.ToolCategory.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
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
                            objDTO.ToolCategory = item.ToolCategory;
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
                            objDTO.Cost = item.Cost;
                            objDTO.Quantity = item.Quantity;
                            objDTO.UDF1 = item.UDF1;
                            objDTO.UDF2 = item.UDF2;
                            objDTO.UDF3 = item.UDF3;
                            objDTO.UDF4 = item.UDF4;
                            objDTO.UDF5 = item.UDF5;
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
                                    objDTO.Status = "Fail";
                                    objDTO.Reason = ResMessage.DuplicateSerialNumber;
                                }
                            }
                            else
                            {
                                objDTO.Serial = item.Serial;
                            }

                            if (item.ToolName.Trim() != "" && SaveToolList)
                            {
                                CurrentToolList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportInventoryClassificationColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportInventoryClassificationColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportInventoryClassificationColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportInventoryClassificationColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportInventoryClassificationColumn.UDF5.ToString());
                                //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportCategoryColumn.UDF6.ToString());
                            }
                            else
                                CurrentBlankToolMasterMain.Add(objDTO);
                        }

                        List<ToolMasterMain> lstreturn = new List<ToolMasterMain>();
                        if (CurrentToolList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.ToolMaster.ToString(), CurrentToolList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);

                        ToolCheckInHistoryDAL objCIDAL = new ToolCheckInHistoryDAL(SessionHelper.EnterPriseDBName);
                        objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                        ToolCheckInOutHistoryDAL objCICODAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
                        foreach (ToolMasterMain item in lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success" && l.CheckOutQuantity > 0))
                        {

                            ToolCheckInHistoryDTO objCIDTO = new ToolCheckInHistoryDTO();
                            ToolCheckInOutHistoryDTO objCICODTO = new ToolCheckInOutHistoryDTO();
                            ToolMasterDTO objtool = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList().Where(t => t.ToolName == item.ToolName && (t.Serial ?? string.Empty) == (item.Serial ?? string.Empty)).FirstOrDefault();

                            objCICODTO.CompanyID = item.CompanyID;
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
                            objCICODTO.TechnicianGuid = item.TechnicianGuid;


                            //Save CheckOut UDF
                            objCICODTO.UDF1 = item.UDF1;
                            objCICODTO.UDF2 = item.UDF2;
                            objCICODTO.UDF3 = item.UDF3;
                            objCICODTO.UDF4 = item.UDF4;
                            objCICODTO.UDF5 = item.UDF5;

                            objCICODTO.AddedFrom = "Web";
                            objCICODTO.EditedFrom = "Web";
                            objCICODTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objCICODTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objCICODTO.IsOnlyFromItemUI = true;

                            objCICODTO.CheckedOutQTY = objtool.Quantity >= (objtool.CheckedOutQTY + item.CheckOutQuantity) ? (objtool.CheckedOutQTY + item.CheckOutQuantity) : 0;
                            objCICODTO.CheckedOutMQTY = 0;

                            objCICODTO.CheckedOutQTYCurrent = 0;

                            objCICODTO.CheckOutDate = DateTimeUtility.DateTimeNow;
                            objCICODTO.CheckOutStatus = "Check Out";

                            if (objCICODTO.CheckedOutQTY > 0)
                            {
                                objCICODAL.Insert(objCICODTO);
                            }

                            ToolMasterDTO objToolDTO = objToolDAL.GetRecord(objtool.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                            objToolDTO.CheckedOutQTY = objToolDTO.CheckedOutQTY.GetValueOrDefault(0) + objCICODTO.CheckedOutQTY;
                            objToolDTO.EditedFrom = "Web";
                            objToolDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objToolDTO.IsOnlyFromItemUI = true;
                            objToolDAL.Edit(objToolDTO);
                        }
                        objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                        objCIDAL = new ToolCheckInHistoryDAL(SessionHelper.EnterPriseDBName);
                        objCICODAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
                        foreach (ToolMasterMain item in lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success" && l.CheckInQuantity > 0))
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
                            objCIDTO.TechnicianGuid = item.TechnicianGuid;
                            double checkinQty = item.CheckInQuantity ?? 0;
                            foreach (var COHistroy in objToolCheckInOutHistoryDTOList.Where(o => o.CheckedOutQTY > o.CheckedOutQTYCurrent))
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

                            objtool.IsOnlyFromItemUI = true;

                            objtool.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objtool.EditedFrom = "Web";
                            objtool.CheckedOutQTY = objtool.CheckedOutQTY > item.CheckInQuantity ? objtool.CheckedOutQTY - item.CheckInQuantity : objtool.CheckedOutQTY - objtool.CheckedOutQTY;
                            objToolDAL.Edit(objtool);
                        }
                        if (CurrentBlankToolMasterMain.Count > 0)
                        {
                            foreach (ToolMasterMain item in CurrentBlankToolMasterMain)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }
                        eTurns.DAL.CacheHelper<IEnumerable<ToolMasterDTO>>.InvalidateCache();

                    }

                }
                #endregion

                #region AssetMaintainance

                else if (ImportMastersDTO.TableName.AssetMaster.ToString() == TableName)
                {
                    List<AssetMasterMain> CurrentBlankAssetMasterMain = new List<AssetMasterMain>();
                    AssetMasterMain[] LstAssetMasterMain = s.Deserialize<AssetMasterMain[]>(para);
                    if (LstAssetMasterMain != null && LstAssetMasterMain.Length > 0)
                    {
                        CurrentAssetMasterList = new List<AssetMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.AssetMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (AssetMasterMain item in LstAssetMasterMain)
                        {
                            AssetMasterMain objDTO = new AssetMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.AssetName = item.AssetName;
                            objDTO.Serial = item.Serial;
                            objDTO.Description = item.Description;
                            objDTO.Make = item.Make;
                            objDTO.Model = item.Model;
                            objDTO.AssetCategoryId = item.AssetCategory == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.AssetCategoryMaster, item.AssetCategory);
                            objDTO.AssetCategory = item.AssetCategory;
                            objDTO.DepreciatedValue = item.DepreciatedValue;
                            objDTO.PurchasePrice = item.PurchasePrice;
                            objDTO.PurchaseDate = item.PurchaseDate;
                            objDTO.UDF1 = item.UDF1;
                            objDTO.UDF2 = item.UDF2;
                            objDTO.UDF3 = item.UDF3;
                            objDTO.UDF4 = item.UDF4;
                            objDTO.UDF5 = item.UDF5;
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
                            if (item.AssetName.Trim() != "")
                            {
                                CurrentAssetMasterList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportInventoryClassificationColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportInventoryClassificationColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportInventoryClassificationColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportInventoryClassificationColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportInventoryClassificationColumn.UDF5.ToString());
                                //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportCategoryColumn.UDF6.ToString());
                            }
                            else
                                CurrentBlankAssetMasterMain.Add(objDTO);
                        }

                        List<AssetMasterMain> lstreturn = new List<AssetMasterMain>();
                        if (CurrentAssetMasterList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.AssetMaster.ToString(), CurrentAssetMasterList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankAssetMasterMain.Count > 0)
                        {
                            foreach (AssetMasterMain item in CurrentBlankAssetMasterMain)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentAssetMasterList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }


                    }

                }
                #endregion
                #region Quick List Item

                else if (ImportMastersDTO.TableName.QuickListItems.ToString() == TableName)
                {
                    List<QuickListItemsMain> CurrentBlankQuickListItemsMain = new List<QuickListItemsMain>();
                    QuickListItemsMain[] LstQuickListItemsMain = s.Deserialize<QuickListItemsMain[]>(para);
                    if (LstQuickListItemsMain != null && LstQuickListItemsMain.Length > 0)
                    {
                        CurrentQuickListMasterList = new List<QuickListItemsMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.QuickListItems.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        foreach (QuickListItemsMain item in LstQuickListItemsMain)
                        {
                            QuickListItemsMain objDTO = new QuickListItemsMain();
                            Guid? ItemGUID = GetGUID(ImportMastersDTO.TableName.ItemMaster, item.ItemNumber);
                            if (ItemGUID.HasValue && ItemGUID != Guid.Empty)
                            {
                                objDTO.ItemGUID = ItemGUID.Value;
                                objDTO.QuickListGUID = item.QuickListname == "" ? Guid.NewGuid() : GetGUID(ImportMastersDTO.TableName.QuickListItems, item.QuickListname, item.Comments);
                                objDTO.Quantity = item.Quantity;
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
                            }


                            if (item.QuickListGUID.ToString().Trim() != "" && ItemGUID.HasValue && ItemGUID != Guid.Empty)
                            {
                                var itemval = CurrentQuickListMasterList.FirstOrDefault(x => x.ItemGUID == objDTO.ItemGUID && x.QuickListGUID == objDTO.QuickListGUID);
                                if (itemval != null)
                                    CurrentQuickListMasterList.Remove(itemval);
                                CurrentQuickListMasterList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportInventoryClassificationColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportInventoryClassificationColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportInventoryClassificationColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportInventoryClassificationColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportInventoryClassificationColumn.UDF5.ToString());
                                //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportCategoryColumn.UDF6.ToString());
                            }
                            else
                            {
                                objDTO = item;
                                if (ItemGUID.HasValue && ItemGUID == Guid.Empty)
                                {
                                    objDTO.Status = "Fail";
                                    objDTO.Reason = "Item does not exist.";
                                }
                                CurrentBlankQuickListItemsMain.Add(objDTO);
                            }

                        }

                        List<QuickListItemsMain> lstreturn = new List<QuickListItemsMain>();
                        if (CurrentQuickListMasterList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.QuickListItems.ToString(), CurrentQuickListMasterList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankQuickListItemsMain.Count > 0)
                        {
                            foreach (QuickListItemsMain item in CurrentBlankQuickListItemsMain)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentQuickListMasterList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }


                    }

                }
                #endregion
                #region Kits

                else if (ImportMastersDTO.TableName.kitdetail.ToString() == TableName)
                {
                    List<KitDetailmain> CurrentBlankKitDetailmain = new List<KitDetailmain>();
                    KitDetailmain[] LstKitDetailmain = s.Deserialize<KitDetailmain[]>(para);
                    if (LstKitDetailmain != null && LstKitDetailmain.Length > 0)
                    {
                        CurrentKitItemList = new List<KitDetailmain>();

                        KitMasterDAL objKitMasterDAL = new KitMasterDAL(SessionHelper.EnterPriseDBName);
                        foreach (KitDetailmain item in LstKitDetailmain)
                        {
                            KitDetailmain objDTO = new KitDetailmain();
                            KitMasterDTO objKitMasterDTO = new KitMasterDTO();
                            Guid? ItemGUID = GetGUID(ImportMastersDTO.TableName.ItemMaster, item.ItemNumber);
                            Guid? KitItemGUID = GetGUID(ImportMastersDTO.TableName.ItemMaster, item.KitPartNumber);
                            objKitMasterDTO = objKitMasterDAL.GetRecord(KitItemGUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, 0, "Import");
                            if (objKitMasterDTO != null && objKitMasterDTO.AvailableKitQuantity == 0)
                            {
                                if (ItemGUID.HasValue && ItemGUID != Guid.Empty && KitItemGUID.HasValue && KitItemGUID != Guid.Empty)
                                {
                                    objDTO.ItemGUID = ItemGUID.Value;
                                    objDTO.KitGUID = KitItemGUID.Value;
                                    objDTO.QuantityPerKit = item.QuantityPerKit;
                                    objDTO.ItemNumber = item.ItemNumber;
                                    objDTO.KitPartNumber = item.KitPartNumber;


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
                                }
                                if (ItemGUID.HasValue && ItemGUID != Guid.Empty && KitItemGUID.HasValue && KitItemGUID != Guid.Empty)
                                {
                                    var itemval = CurrentKitItemList.FirstOrDefault(x => x.ItemGUID == objDTO.ItemGUID && x.KitGUID == objDTO.KitGUID);
                                    if (itemval != null)
                                        CurrentKitItemList.Remove(itemval);
                                    CurrentKitItemList.Add(objDTO);


                                }
                                else
                                {
                                    objDTO = item;
                                    if (ItemGUID.HasValue && ItemGUID == Guid.Empty)
                                    {
                                        objDTO.Status = "Fail";
                                        objDTO.Reason = "Item does not exist.";
                                    }
                                    CurrentBlankKitDetailmain.Add(objDTO);
                                }
                            }
                            else
                            {
                                objDTO = item;
                                objDTO.Status = "Fail";
                                if (objKitMasterDTO == null)
                                {
                                    objDTO.Reason = "Kit does not exist.";
                                }
                                else
                                {
                                    objDTO.Reason = "Item Is Not Able To Edit.";
                                }

                                CurrentBlankKitDetailmain.Add(objDTO);
                            }





                        }

                        List<KitDetailmain> lstreturn = new List<KitDetailmain>();
                        if (CurrentKitItemList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.kitdetail.ToString(), CurrentKitItemList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankKitDetailmain.Count > 0)
                        {
                            foreach (KitDetailmain item in CurrentBlankKitDetailmain)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentKitItemList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }


                    }

                }
                #endregion
                #region BOM Item Master
                else if (ImportMastersDTO.TableName.BOMItemMaster.ToString() == TableName)
                {

                    List<BOMItemMasterMain> CurrentBlankBOMItemList = new List<BOMItemMasterMain>();
                    BOMItemMasterMain[] LstItemMaster = s.Deserialize<BOMItemMasterMain[]>(para);
                    if (LstItemMaster != null && LstItemMaster.Length > 0)
                    {
                        CurrentBOMItemList = new List<BOMItemMasterMain>();
                        lst = obj.GetUDFList(SessionHelper.CompanyID, ImportMastersDTO.TableName.BOMItemMaster.ToString(), UDFControlTypes.Textbox.ToString());
                        CurrentOptionList = new List<UDFOptionsMain>();

                        //bool IsEmailPOInBody = false; bool IsEmailPOInPDF = false; bool IsEmailPOInCSV = false; bool IsEmailPOInX12 = false;
                        foreach (BOMItemMasterMain item in LstItemMaster)
                        {
                            BOMItemMasterMain objDTO = new BOMItemMasterMain();
                            objDTO.ID = item.ID;
                            objDTO.ItemNumber = (item.ItemNumber == null) ? null : (item.ItemNumber.Length > 255 ? item.ItemNumber.Substring(0, 255) : item.ItemNumber);
                            objDTO.ManufacturerName = item.ManufacturerName;
                            //Wi-1505
                            if (!string.IsNullOrWhiteSpace(item.ManufacturerName))
                            {
                                objDTO.ManufacturerID = item.ManufacturerName == "" ? (long?)null : GetBOMIDs(ImportMastersDTO.TableName.ManufacturerMaster, item.ManufacturerName);
                            }
                            else
                            {
                                objDTO.ManufacturerID = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName).GetORInsertBlankManuFacID(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                            }

                            //objDTO.ManufacturerID = item.ManufacturerName == "" ? (long?)null : GetBOMIDs(ImportMastersDTO.TableName.ManufacturerMaster, item.ManufacturerName);
                            objDTO.ManufacturerNumber = (item.ManufacturerNumber == null) ? null : (item.ManufacturerNumber.Length > 20 ? item.ManufacturerNumber.Substring(0, 20) : item.ManufacturerNumber);
                            objDTO.SupplierName = item.SupplierName;
                            objDTO.SupplierPartNo = (item.SupplierPartNo == null) ? null : (item.SupplierPartNo.Length > 20 ? item.SupplierPartNo.Substring(0, 20) : item.SupplierPartNo);
                            objDTO.SupplierID = GetBOMIDs(ImportMastersDTO.TableName.SupplierMaster, item.SupplierName);
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
                            //objDTO.ItemUniqueNumber = objCommonDAL.UniqueItemId();
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
                            objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                            objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                            objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                            objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                            objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);
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
                            // objDTO.Room = SessionHelper.RoomID;
                            objDTO.CompanyID = SessionHelper.CompanyID;
                            objDTO.CreatedBy = SessionHelper.UserID;
                            objDTO.GUID = Guid.NewGuid();
                            objDTO.IsBOMItem = true;
                            objDTO.WhatWhereAction = "Import";
                            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objDTO.AddedFrom = "Web";
                            objDTO.EditedFrom = "Web";
                            lstItemGUID.Add(objDTO.GUID);


                            if (item.ItemNumber.Trim() != "")
                            {
                                CurrentBOMItemList.Add(objDTO);
                                //CurrentItemDTOList.Add(objDTO);

                                CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportItemColumn.UDF1.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportItemColumn.UDF2.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportItemColumn.UDF3.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportItemColumn.UDF4.ToString());
                                CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportItemColumn.UDF5.ToString());

                            }
                            else
                                CurrentBlankBOMItemList.Add(item);
                        }

                        List<BOMItemMasterMain> lstreturn = new List<BOMItemMasterMain>();
                        if (CurrentBOMItemList.Count > 0)
                            lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.BOMItemMaster.ToString(), CurrentBOMItemList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        if (CurrentBlankBOMItemList.Count > 0)
                        {
                            foreach (BOMItemMasterMain item in CurrentBlankBOMItemList)
                            {
                                lstreturn.Add(item);
                            }
                        }

                        if (lstreturn.Count == 0)
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            ClearCurrentResourceList();
                            Session["importedData"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                            status = ResMessage.SaveMessage;
                            Session["importedData"] = lstreturn;
                        }
                        if (CurrentBOMItemList.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                        {
                            allSuccesfulRecords = false;
                        }


                    }

                }
                #endregion
                if (ImportMastersDTO.TableName.ItemMaster.ToString() == TableName)
                {
                    //Suggested Order calling for the particullar imported items...

                    if (ImportMastersDTO.TableName.ItemMaster.ToString() == TableName)
                    {
                        foreach (Guid gid in lstItemGUID)
                        {
                            new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(gid, SessionHelper.UserID, SessionHelper.EnterPriceID, true);
                            new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetAndUpdateExtCostAndAvgCost(gid, SessionHelper.RoomID, SessionHelper.CompanyID);
                            //ItemMasterDAL objItemMasterLeveQuantityDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                            //ItemMasterDTO objItemLeveQuantity = new ItemMasterDTO();
                            //objItemLeveQuantity = objItemMasterLeveQuantityDAL.GetRecord(gid.ToString(), eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID);
                            //if (objItemLeveQuantity!=null && objItemLeveQuantity.DefaultLocation > 0)
                            //{ 
                            //ImportDAL objImporLoctDAL=new ImportDAL(SessionHelper.EnterPriseDBName);
                            //objImporLoctDAL.SaveItemLocationLevelQuantity(objItemLeveQuantity, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                            //}
                        }
                    }

                    //call default supplier function
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    //objItemMasterDAL.UpdateSupplierDetails(SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);

                    CacheHelper<IEnumerable<BinMasterDTO>>.InvalidateCache();
                    (new BinMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);

                    CacheHelper<IEnumerable<CategoryMasterDTO>>.InvalidateCache();
                    (new CategoryMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);


                    CacheHelper<IEnumerable<GLAccountMasterDTO>>.InvalidateCache();
                    (new GLAccountMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);


                    CacheHelper<IEnumerable<JobTypeMasterDTO>>.InvalidateCache();
                    (new JobTypeMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);


                    CacheHelper<IEnumerable<ManufacturerMasterDTO>>.InvalidateCache();
                    (new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);


                    CacheHelper<IEnumerable<UnitMasterDTO>>.InvalidateCache();
                    (new UnitMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);


                    CacheHelper<IEnumerable<SupplierMasterDTO>>.InvalidateCache();
                    (new SupplierMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);

                    CacheHelper<IEnumerable<UnitMasterDTO>>.InvalidateCache();
                    (new UnitMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);

                    CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.InvalidateCache();
                    (new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID);

                    CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.InvalidateCache();
                    (new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID);
                    //   CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.InvalidateCache();
                    //   (new ItemLocationLevelQuanityDAL()).GetCachedData(0, SessionHelper.CompanyID);
                    //Regenerate Cache for items only..

                    CacheHelper<IEnumerable<ItemMasterDTO>>.InvalidateCache();
                    objItemMasterDAL.GetCachedData(0, SessionHelper.CompanyID);
                }

                //assume success here and clear Cache for this company
                //GenerateCacheController objCache = new GenerateCacheController();
                //objCache.GenerateCacheCompany(SessionHelper.UserID, SessionHelper.CompanyID);

            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = "fail";
                allSuccesfulRecords = false;
            }
            finally
            {

                // resHelper = null;
            }
            var jsonResult = Json(new { Message = message, Status = status, allSuccesfulRecords = allSuccesfulRecords, savedOnlyitemIds = savedOnlyitemIds }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                if (emailaddress != "")
                {
                    MailAddress m = new MailAddress(emailaddress);
                }
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
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
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(c => (c.SupplierName ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
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
                    objDTO.CostUOMValue = 0;
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
                    obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(c => (c.AssetCategory ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
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
        public Guid GetGUID(ImportMastersDTO.TableName TableName, string strVal, string optValue = "")
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
                        returnID = obj.GUID;
                }
                else
                {
                    QuickListMasterDTO objDTO = new QuickListMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Name = strVal;
                    objDTO.Comment = optValue;
                    objDTO.Type = 1;
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
            #endregion
            return returnID;
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
                    objcheck = lst.Where(c => c.UDFColumnName == UDFs.ToString() && c.UDFOption == objDTOUDF).FirstOrDefault();
                    if (objcheck == null)
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
        #endregion

        #region Update Method
        public string UpdateDataBinSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentBinList != null && CurrentBinList.Count > 0)
            {
                int indx = CurrentBinList.FindIndex(x => x.ID == Convert.ToInt32(id));
                InventoryLocationMain obj = CurrentBinList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportBinColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportBinColumn.BinNumber.ToString())
                        obj.BinNumber = value;
                    else if (columnName == CommonUtility.ImportBinColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportBinColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportBinColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportBinColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportBinColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportBinColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportBinColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportBinColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportBinColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportBinColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentBinList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataCategorySession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentCategoryList != null && CurrentCategoryList.Count > 0)
            {
                int indx = CurrentCategoryList.FindIndex(x => x.ID == Convert.ToInt32(id));
                CategoryMasterMain obj = CurrentCategoryList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportCategoryColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportCategoryColumn.Category.ToString())
                        obj.Category = value;
                    else if (columnName == CommonUtility.ImportCategoryColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportCategoryColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportCategoryColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportCategoryColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportCategoryColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportCategoryColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportCategoryColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportCategoryColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportCategoryColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportCategoryColumn.UDF10.ToString())
                    //    obj.UDF10 = value;
                    //else if (columnName == CommonUtility.ImportCategoryColumn.CategoryColor.ToString())
                    //    obj.CategoryColor = value;

                    CurrentCategoryList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataCustomerSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentCustomerList != null && CurrentCustomerList.Count > 0)
            {
                int indx = CurrentCustomerList.FindIndex(x => x.ID == Convert.ToInt32(id));
                CustomerMasterMain obj = CurrentCustomerList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportCustomerColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportCustomerColumn.Customer.ToString())
                        obj.Customer = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.Account.ToString())
                        obj.Account = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.Contact.ToString())
                        obj.Contact = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.Address.ToString())
                        obj.Address = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.City.ToString())
                        obj.City = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.State.ToString())
                        obj.State = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.ZipCode.ToString())
                        obj.ZipCode = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.Country.ToString())
                        obj.Country = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.Phone.ToString())
                        obj.Phone = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.Email.ToString())
                        obj.Email = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportCustomerColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportCustomerColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportCustomerColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportCustomerColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportCustomerColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportCustomerColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentCustomerList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataFreightTypeSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentFreightType != null && CurrentFreightType.Count > 0)
            {
                int indx = CurrentFreightType.FindIndex(x => x.ID == Convert.ToInt32(id));
                FreightTypeMasterMain obj = CurrentFreightType.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportFreightTypeColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportFreightTypeColumn.FreightType.ToString())
                        obj.FreightType = value;
                    else if (columnName == CommonUtility.ImportFreightTypeColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportFreightTypeColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportFreightTypeColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportFreightTypeColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportFreightTypeColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportFreightTypeColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportFreightTypeColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportFreightTypeColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportFreightTypeColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportFreightTypeColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentFreightType[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataGLAccountSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentGLAccountList != null && CurrentGLAccountList.Count > 0)
            {
                int indx = CurrentGLAccountList.FindIndex(x => x.ID == Convert.ToInt32(id));
                GLAccountMasterMain obj = CurrentGLAccountList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportGLAccountColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportGLAccountColumn.GLAccount.ToString())
                        obj.GLAccount = value;
                    else if (columnName == CommonUtility.ImportGLAccountColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportGLAccountColumn.Description.ToString())
                        obj.Description = value;
                    else if (columnName == CommonUtility.ImportGLAccountColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportGLAccountColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportGLAccountColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportGLAccountColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportGLAccountColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportGLAccountColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportGLAccountColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportGLAccountColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportGLAccountColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentGLAccountList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataGXPRConsignedSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentGXPRConsignedList != null && CurrentGXPRConsignedList.Count > 0)
            {
                int indx = CurrentGXPRConsignedList.FindIndex(x => x.ID == Convert.ToInt32(id));
                GXPRConsignedMasterMain obj = CurrentGXPRConsignedList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportGXPRConsignedColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportGXPRConsignedColumn.GXPRConsigmentJob.ToString())
                        obj.GXPRConsigmentJob = value;
                    else if (columnName == CommonUtility.ImportGXPRConsignedColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportGXPRConsignedColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportGXPRConsignedColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportGXPRConsignedColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportGXPRConsignedColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportGXPRConsignedColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportGXPRConsignedColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportGXPRConsignedColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportGXPRConsignedColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportGXPRConsignedColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentGXPRConsignedList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataJobTypeSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentJobTypeList != null && CurrentJobTypeList.Count > 0)
            {
                int indx = CurrentJobTypeList.FindIndex(x => x.ID == Convert.ToInt32(id));
                JobTypeMasterMain obj = CurrentJobTypeList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportJobTypeColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportJobTypeColumn.JobType.ToString())
                        obj.JobType = value;
                    else if (columnName == CommonUtility.ImportJobTypeColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportJobTypeColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportJobTypeColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportJobTypeColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportJobTypeColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportJobTypeColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportJobTypeColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportJobTypeColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportJobTypeColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportJobTypeColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentJobTypeList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataShipViaSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentShipViaList != null && CurrentShipViaList.Count > 0)
            {
                int indx = CurrentShipViaList.FindIndex(x => x.ID == Convert.ToInt32(id));
                ShipViaMasterMain obj = CurrentShipViaList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportShipViaColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportShipViaColumn.ShipVia.ToString())
                        obj.ShipVia = value;
                    else if (columnName == CommonUtility.ImportShipViaColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportShipViaColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportShipViaColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportShipViaColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportShipViaColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportShipViaColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportShipViaColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportShipViaColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportShipViaColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportShipViaColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentShipViaList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataTechnicianSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentTechnicianList != null && CurrentTechnicianList.Count > 0)
            {
                int indx = CurrentTechnicianList.FindIndex(x => x.ID == Convert.ToInt32(id));
                TechnicianMasterMain obj = CurrentTechnicianList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportTechnicianColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportTechnicianColumn.Technician.ToString())
                        obj.Technician = value;
                    else if (columnName == CommonUtility.ImportTechnicianColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportTechnicianColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportTechnicianColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportTechnicianColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportTechnicianColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportTechnicianColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportTechnicianColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportTechnicianColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportTechnicianColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportTechnicianColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentTechnicianList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataManufacturerSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentManufacturerList != null && CurrentManufacturerList.Count > 0)
            {
                int indx = CurrentManufacturerList.FindIndex(x => x.ID == Convert.ToInt32(id));
                ManufacturerMasterMain obj = CurrentManufacturerList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportManufacturerColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportManufacturerColumn.Manufacturer.ToString())
                        obj.Manufacturer = value;
                    else if (columnName == CommonUtility.ImportManufacturerColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportManufacturerColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportManufacturerColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportManufacturerColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportManufacturerColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportManufacturerColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportManufacturerColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportManufacturerColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportManufacturerColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportManufacturerColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentManufacturerList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataMeasurementTermSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentMeasurementTermList != null && CurrentMeasurementTermList.Count > 0)
            {
                int indx = CurrentMeasurementTermList.FindIndex(x => x.ID == Convert.ToInt32(id));
                MeasurementTermMasterMain obj = CurrentMeasurementTermList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportMeasurementTermColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportMeasurementTermColumn.MeasurementTerm.ToString())
                        obj.MeasurementTerm = value;
                    else if (columnName == CommonUtility.ImportMeasurementTermColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportMeasurementTermColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportMeasurementTermColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportMeasurementTermColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportMeasurementTermColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportMeasurementTermColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportMeasurementTermColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportMeasurementTermColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportMeasurementTermColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportMeasurementTermColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentMeasurementTermList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataUnitSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentUnitList != null && CurrentUnitList.Count > 0)
            {
                int indx = CurrentUnitList.FindIndex(x => x.ID == Convert.ToInt32(id));
                UnitMasterMain obj = CurrentUnitList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportUnitsColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportUnitsColumn.Unit.ToString())
                        obj.Unit = value;
                    else if (columnName == CommonUtility.ImportUnitsColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportUnitsColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportUnitsColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportUnitsColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportUnitsColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportUnitsColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportUnitsColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportUnitsColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportUnitsColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportUnitsColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentUnitList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataSupplierSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentSupplierList != null && CurrentSupplierList.Count > 0)
            {
                int indx = CurrentSupplierList.FindIndex(x => x.ID == Convert.ToInt32(id));
                SupplierMasterMain obj = CurrentSupplierList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportSupplierColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportSupplierColumn.SupplierName.ToString())
                        obj.SupplierName = value;
                    else if (columnName == CommonUtility.ImportSupplierColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportSupplierColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportSupplierColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportSupplierColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportSupplierColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportSupplierColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportSupplierColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportSupplierColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportSupplierColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportSupplierColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentSupplierList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataItemSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentItemList != null && CurrentItemList.Count > 0)
            {
                int indx = CurrentItemList.FindIndex(x => x.ID == Convert.ToInt32(id));
                BOMItemMasterMain obj = CurrentItemList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportItemColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportItemColumn.ItemNumber.ToString())
                        obj.ItemNumber = value;
                    else if (columnName == CommonUtility.ImportItemColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportItemColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportItemColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportItemColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportItemColumn.UDF5.ToString())
                        obj.UDF5 = value;


                    CurrentItemList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataLocationSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentLocationList != null && CurrentLocationList.Count > 0)
            {
                int indx = CurrentLocationList.FindIndex(x => x.ID == Convert.ToInt32(id));
                BinMasterMain obj = CurrentLocationList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportLocationColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportLocationColumn.Location.ToString())
                        obj.BinNumber = value;
                    else if (columnName == CommonUtility.ImportLocationColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportLocationColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportLocationColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportLocationColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportLocationColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportLocationColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportLocationColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportLocationColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportLocationColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportLocationColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentLocationList[indx] = obj;
                }
            }
            return value;
        }
        public string UpdateDataToolCategorySession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentToolCategoryList != null && CurrentToolCategoryList.Count > 0)
            {
                int indx = CurrentToolCategoryList.FindIndex(x => x.ID == Convert.ToInt32(id));
                ToolCategoryMasterMain obj = CurrentToolCategoryList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportToolCategoryColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportToolCategoryColumn.ToolCategory.ToString())
                        obj.ToolCategory = value;
                    else if (columnName == CommonUtility.ImportToolCategoryColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportToolCategoryColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportToolCategoryColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportToolCategoryColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportToolCategoryColumn.UDF5.ToString())
                        obj.UDF5 = value;
                    //else if (columnName == CommonUtility.ImportToolCategoryColumn.UDF6.ToString())
                    //    obj.UDF6 = value;
                    //else if (columnName == CommonUtility.ImportToolCategoryColumn.UDF7.ToString())
                    //    obj.UDF7 = value;
                    //else if (columnName == CommonUtility.ImportToolCategoryColumn.UDF8.ToString())
                    //    obj.UDF8 = value;
                    //else if (columnName == CommonUtility.ImportToolCategoryColumn.UDF9.ToString())
                    //    obj.UDF9 = value;
                    //else if (columnName == CommonUtility.ImportToolCategoryColumn.UDF10.ToString())
                    //    obj.UDF10 = value;

                    CurrentToolCategoryList[indx] = obj;
                }
            }
            return value;
        }

        public string UpdateDataCostUOMSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentCostUOMList != null && CurrentCostUOMList.Count > 0)
            {
                int indx = CurrentCostUOMList.FindIndex(x => x.ID == Convert.ToInt32(id));
                CostUOMMasterMain obj = CurrentCostUOMList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportCostUOMColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportCostUOMColumn.CostUOM.ToString())
                        obj.CostUOM = value;
                    else if (columnName == CommonUtility.ImportCostUOMColumn.CostUOMValue.ToString())
                        obj.CostUOMValue = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportCostUOMColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportCostUOMColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportCostUOMColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportCostUOMColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportCostUOMColumn.UDF5.ToString())
                        obj.UDF5 = value;

                    CurrentCostUOMList[indx] = obj;
                }
            }
            return value;
        }

        public string UpdateDataInventoryClassificationSession(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && CurrentInventoryClassificationList != null && CurrentInventoryClassificationList.Count > 0)
            {
                int indx = CurrentInventoryClassificationList.FindIndex(x => x.ID == Convert.ToInt32(id));
                InventoryClassificationMasterMain obj = CurrentInventoryClassificationList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportInventoryClassificationColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportInventoryClassificationColumn.InventoryClassification.ToString())
                        obj.InventoryClassification = value;
                    else if (columnName == CommonUtility.ImportInventoryClassificationColumn.BaseOfInventory.ToString())
                        obj.BaseOfInventory = value;
                    else if (columnName == CommonUtility.ImportInventoryClassificationColumn.RangeStart.ToString())
                        obj.RangeStart = Convert.ToDouble(value);
                    else if (columnName == CommonUtility.ImportInventoryClassificationColumn.RangeEnd.ToString())
                        obj.RangeEnd = Convert.ToDouble(value);
                    else if (columnName == CommonUtility.ImportInventoryClassificationColumn.UDF1.ToString())
                        obj.UDF1 = value;
                    else if (columnName == CommonUtility.ImportInventoryClassificationColumn.UDF2.ToString())
                        obj.UDF2 = value;
                    else if (columnName == CommonUtility.ImportInventoryClassificationColumn.UDF3.ToString())
                        obj.UDF3 = value;
                    else if (columnName == CommonUtility.ImportInventoryClassificationColumn.UDF4.ToString())
                        obj.UDF4 = value;
                    else if (columnName == CommonUtility.ImportInventoryClassificationColumn.UDF5.ToString())
                        obj.UDF5 = value;

                    CurrentInventoryClassificationList[indx] = obj;
                }
            }
            return value;
        }


        #endregion

        [HttpPost]
        public bool ClearCurrentResourceList()
        {
            CurrentBinList = null;
            CurrentCategoryList = null;
            CurrentCustomerList = null;
            CurrentFreightType = null;
            CurrentGLAccountList = null;
            CurrentGXPRConsignedList = null;
            CurrentJobTypeList = null;
            CurrentShipViaList = null;
            CurrentTechnicianList = null;
            CurrentManufacturerList = null;
            CurrentMeasurementTermList = null;
            CurrentUnitList = null;
            CurrentSupplierList = null;
            CurrentItemList = null;
            CurrentLocationList = null;
            CurrentToolCategoryList = null;
            CurrentCostUOMList = null;
            CurrentInventoryClassificationList = null;
            CurrentToolList = null;
            CurrentAssetMasterList = null;
            CurrentQuickListMasterList = null;
            CurrentInventoryLocationMasterList = null;
            CurrentBOMItemList = null;
            CurrentKitItemList = null;
            CurrentInventoryLocationMasterList = null;
            CurrentInventoryLocationQuantityList = null;
            Session["importedData"] = null;
            return true;
        }
        public void SetSelectedModule(string CurModule, string CurModuletext)
        {
            Session["CurModuleValue"] = CurModule;
            if (CurModule == ((int)SessionHelper.ModuleList.BinMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.BinMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.eVMISetup).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.ItemLocationeVMISetup;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.CategoryMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.CategoryMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.CustomerMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.CustomerMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.FreightTypeMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.FreightTypeMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.GLAccountsMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.GLAccountMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.GXPRConsignedJobMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.GXPRConsigmentJobMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.JobTypeMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.JobTypeMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.ShipViaMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.ShipViaMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.TechnicianMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.TechnicianMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.ManufacturerMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.ManufacturerMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.MeasurementTermMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.MeasurementTermMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.UnitMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.UnitMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.SupplierMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.SupplierMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.ItemMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.ItemMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.LocationMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.LocationMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.ToolCategory).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.ToolCategoryMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.CostUOMMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.CostUOMMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.InventoryClassificationMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.InventoryClassificationMaster;
            }
            else if (CurModule == ((int)SessionHelper.ModuleList.Assets).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.AssetMaster;
            }
            else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.ToolMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.ToolMaster;
            }
            else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.QuickListPermission).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.QuickListItems;
            }
            else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.BinMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.InventoryLocation;
            }
            else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.BOMItemMaster).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.BOMItemMaster;
            }
            else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.Kits).ToString())
            {
                Session["CurModule"] = ImportMastersDTO.TableName.kitdetail;
            }
        }
        public IList<T> GetValidateList<T>(List<T> List, string tableName)
        {
            List<T> lstreturnFinal = new List<T>();
            PropertyDescriptor[] props = null;
            if (tableName == ImportMastersDTO.TableName.ToolMaster.ToString())
            {
                props = TypeDescriptor.GetProperties(typeof(T))
                            .Cast<PropertyDescriptor>()
                            .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                            && propertyInfo.Name != "CheckOutStatus" && propertyInfo.Name != "CheckOutDate" && propertyInfo.Name != "CheckInDate" &&
                                    propertyInfo.Name != "ToolCategory" && propertyInfo.Name != "RoomName" && propertyInfo.Name != "Location" &&
                                     propertyInfo.Name != "CreatedByName" && propertyInfo.Name != "UpdatedByName" &&
                                    propertyInfo.Name != "Action" && propertyInfo.Name != "HistoryID")
                            .ToArray();
            }
            else if (tableName == ImportMastersDTO.TableName.InventoryClassificationMaster.ToString())
            {
                props = TypeDescriptor.GetProperties(typeof(T))
                          .Cast<PropertyDescriptor>()
                          .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")
                          && propertyInfo.Name != "Status" && propertyInfo.Name != "Reason"
                                  )
                          .ToArray();

            }
            string reterrormsg = string.Empty;
            var values = new object[props.Length];
            foreach (var item in lstreturnFinal)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    if (props[i].GetType() == typeof(System.Decimal))
                    {
                        if (!CheckDecimalValue(props[i].GetValue(item).ToString()))
                        {
                            reterrormsg = "Invalid Value in" + props[i].Name.ToString();
                        }
                    }
                    else if (props[i].GetType() == typeof(System.Double))
                    {
                        if (!CheckDoubleValue(props[i].GetValue(item).ToString()))
                        {
                            reterrormsg = "Invalid Value in" + props[i].Name.ToString();
                        }
                    }
                    else if (props[i].GetType() == typeof(System.DateTime))
                    {
                        if (!CheckDatetimeValue(props[i].GetValue(item).ToString()))
                        {
                            reterrormsg = "Invalid Value in" + props[i].Name.ToString();
                        }
                    }

                }
                //table.Rows.Add(values);
            }
            return lstreturnFinal;
        }
        [HttpPost]
        public string CheckBlankSerial()
        {
            string ToolName = string.Empty;
            try
            {
                ToolsMaintenanceDAL objToolsMaintenanceDAL = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
                ToolName = objToolsMaintenanceDAL.BlankSerialToolName(SessionHelper.RoomID, SessionHelper.CompanyID);
            }
            catch (Exception)
            {
                ToolName = string.Empty;
            }
            return ToolName;
        }
        public bool CheckDecimalValue(string val)
        {
            decimal decval;
            bool ret = false;
            ret = decimal.TryParse(val, out decval);
            return ret;
        }
        public bool CheckDoubleValue(string val)
        {
            double decval;
            bool ret = false;
            ret = double.TryParse(val, out decval);
            return ret;
        }
        public bool CheckDatetimeValue(string val)
        {
            DateTime decval;
            bool ret = false;
            ret = DateTime.TryParse(val, out decval);
            return ret;
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
    }

}
