using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using Microsoft.VisualBasic.FileIO;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using eTurnsWeb.Helper;
using System.Text;
using System.IO;
using System.Data;
using eTurns.DTO.Resources;
using eTurns.DTO;
using eTurns.DAL;
using System.ComponentModel;
using System.Configuration;
using Microsoft.VisualBasic.FileIO;
using NPOI.HSSF.UserModel;
using eTurnsWeb.BAL;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public partial class ImportMultiRoomController : eTurnsControllerBase
    {
        #region Property Declaration
        private const string _CURRENTBINSESSIONKEY = "CURRENTBINLIST";
        private const string _CURRENTITEMLOCATIONQTYSESSIONKEY = "CURRENTITEMLOCATIONQTYLIST";
        private const string _CURRENTMANUALCOUNTSESSIONKEY = "CURRENTMANUALCOUNTLIST";
        private const string _CURRENTWORKORDERSESSIONKEY = "CURRENTWORKORDERLIST";
        private const string _CURRENTPULLIMPORTLIST = "CURRENTPULLIMPORTLIST";
        private const string _CURRENTPULLIMPORTWITHLOTSERIALLIST = "CURRENTPULLIMPORTWITHLOTSERIALLIST";


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
        private const string _CURRENTASSETTOOLSCHEDULERMAPPINGLIST = "CURRENTASSETTOOLSCHEDULERMAPPINGLIST";//
        private const string _CURRENTQUICKLISTMASTERLIST = "CURRENTQUICKLISTMASTERLIST";
        private const string _CURRENTINVENTORYLOCATIONMASTERLIST = "CURRENTINVENTORYLOCATIONMASTERLIST";
        private const string _CURRENTINVENTORYLOCATIONQuantityLIST = "CURRENTINVENTORYLOCATIONQuantityLIST";
        private const string _CURRENTBOMITEMSESSIONKEY = "CURRENTBOMITEMLIST";
        private const string _CURRENTKitItemSESSIONKEY = "CURRENTKitITEMLIST";
        private const string _CURRENTITEMMANUFACTURERSESSIONKEY = "CURRENTITEMMANUFACTURERLIST";
        private const string _CURRENTITEMSUPPLIERSESSIONKEY = "CURRENTITEMSUPPLIERLIST";
        private const string _CURRENTBARCODESESSIONKEY = "CURRENTBARCODELIST";
        private const string _CURRENTUDFSESSIONKEY = "CURRENTUDFLIST";
        private const string _CURRENTPROJECTMASTERSESSIONKEY = "CURRENTPROJECTMASTERSESSIONKEY";
        private const string _CURRENTLOCATIONCHANGESESSIONKEY = "CURRENTITEMLOCATIONCHANGELIST";
        private const string _CURRENTPULLIMPORTWITHSAMEQTYLIST = "CURRENTPULLIMPORTWITHSAMEQTYLIST";
        private const string _CURRENTASSETTOOLSCHEDULERLIST = "CURRENTASSETTOOLSCHEDULERLIST";
        private const string _CURRENTPASTMAINTENANCEDUELIST = "CURRENTPASTMAINTENANCEDUELIST";
        private const string _CURRENTTOOLCHECKINCHECKOUTLIST = "CURRENTTOOLCHECKINCHECKOUTLIST";

        private const string _CURRENTTOOLADJUSTMENTCOUNT = "CURRENTTOOLADJUSTMENTCOUNTLIST";
        private const string _CURRENTTOOLIMAGEIMPORTLIST = "CURRENTTOOLIMAGEIMPORTLIST";
        private const string _CURRENTORDERMASTERLIST = "CURRENTORDERMASTERLIST";
        private const string _CURRENTMOVEMATERIALLIST = "CURRENTMOVEMATERIALLIST";
        private const string _CURRENTENTERPRISEQUICKLIST = "CURRENTENTERPRISEQUICKLIST";
        private const string _CURRENTREQUISITIONLIST = "CURRENTREQUISITIONLIST";
        private const string _CURRENTQUOTEMASTERLIST = "CURRENTQUOTELIST";
        private const string _CURRENTSUPPLIERCATALOGLIST = "CURRENTSUPPLIERCATALOGLIST";

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
        private List<ItemLocationChangeImport> CurrentLocationChangeList
        {
            get
            {
                if (HttpContext.Session[_CURRENTLOCATIONCHANGESESSIONKEY] != null)
                    return (List<ItemLocationChangeImport>)HttpContext.Session[_CURRENTLOCATIONCHANGESESSIONKEY];
                return new List<ItemLocationChangeImport>();
            }
            set
            {
                HttpContext.Session[_CURRENTLOCATIONCHANGESESSIONKEY] = value;
            }
        }
        private List<InventoryLocationMain> CurrentItemLocationQtyList
        {
            get
            {
                if (HttpContext.Session[_CURRENTITEMLOCATIONQTYSESSIONKEY] != null)
                    return (List<InventoryLocationMain>)HttpContext.Session[_CURRENTITEMLOCATIONQTYSESSIONKEY];
                return new List<InventoryLocationMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTITEMLOCATIONQTYSESSIONKEY] = value;
            }
        }
        private List<InventoryLocationMain> CurrentManualCountList
        {
            get
            {
                if (HttpContext.Session[_CURRENTMANUALCOUNTSESSIONKEY] != null)
                    return (List<InventoryLocationMain>)HttpContext.Session[_CURRENTMANUALCOUNTSESSIONKEY];
                return new List<InventoryLocationMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTMANUALCOUNTSESSIONKEY] = value;
            }
        }
        private List<WorkOrderMain> CurrentWorkOrderList
        {
            get
            {
                if (HttpContext.Session[_CURRENTWORKORDERSESSIONKEY] != null)
                    return (List<WorkOrderMain>)HttpContext.Session[_CURRENTWORKORDERSESSIONKEY];
                return new List<WorkOrderMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTWORKORDERSESSIONKEY] = value;
            }
        }

        private List<PullImport> CurrentPullImportList
        {
            get
            {
                if (HttpContext.Session[_CURRENTPULLIMPORTLIST] != null)
                    return (List<PullImport>)HttpContext.Session[_CURRENTPULLIMPORTLIST];
                return new List<PullImport>();
            }
            set
            {
                HttpContext.Session[_CURRENTPULLIMPORTLIST] = value;
            }
        }

        private List<PullImportWithLotSerial> CurrentPullImportWithLotSerialList
        {
            get
            {
                if (HttpContext.Session[_CURRENTPULLIMPORTWITHLOTSERIALLIST] != null)
                    return (List<PullImportWithLotSerial>)HttpContext.Session[_CURRENTPULLIMPORTWITHLOTSERIALLIST];
                return new List<PullImportWithLotSerial>();
            }
            set
            {
                HttpContext.Session[_CURRENTPULLIMPORTWITHLOTSERIALLIST] = value;
            }
        }

        private List<PullImportWithSameQty> CurrentPullImportWitSameQtyList
        {
            get
            {
                if (HttpContext.Session[_CURRENTPULLIMPORTWITHSAMEQTYLIST] != null)
                    return (List<PullImportWithSameQty>)HttpContext.Session[_CURRENTPULLIMPORTWITHSAMEQTYLIST];
                return new List<PullImportWithSameQty>();
            }
            set
            {
                HttpContext.Session[_CURRENTPULLIMPORTWITHSAMEQTYLIST] = value;
            }
        }
        private List<PastMaintenanceDueImport> CurrentPastMaintenanceDueImportList
        {
            get
            {
                if (HttpContext.Session[_CURRENTPASTMAINTENANCEDUELIST] != null)
                    return (List<PastMaintenanceDueImport>)HttpContext.Session[_CURRENTPASTMAINTENANCEDUELIST];
                return new List<PastMaintenanceDueImport>();
            }
            set
            {
                HttpContext.Session[_CURRENTPASTMAINTENANCEDUELIST] = value;
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
        private List<LocationMasterMain> CurrentLocationList
        {
            get
            {
                if (HttpContext.Session[_CURRENTLOCATIONSESSIONKEY] != null)
                    return (List<LocationMasterMain>)HttpContext.Session[_CURRENTLOCATIONSESSIONKEY];
                return new List<LocationMasterMain>();
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

        private List<AssetToolSchedulerMapping> CurrentAssetToolSchedulerMappingList
        {
            get
            {
                if (HttpContext.Session[_CURRENTASSETTOOLSCHEDULERMAPPINGLIST] != null)
                    return (List<AssetToolSchedulerMapping>)HttpContext.Session[_CURRENTASSETTOOLSCHEDULERMAPPINGLIST];
                return new List<AssetToolSchedulerMapping>();
            }
            set
            {
                HttpContext.Session[_CURRENTASSETTOOLSCHEDULERMAPPINGLIST] = value;
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
        private List<OrderMasterItemsMain> CurrentOrderMasterList
        {
            get
            {
                if (HttpContext.Session[_CURRENTORDERMASTERLIST] != null)
                    return (List<OrderMasterItemsMain>)HttpContext.Session[_CURRENTORDERMASTERLIST];
                return new List<OrderMasterItemsMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTORDERMASTERLIST] = value;
            }
        }

        private List<MoveMaterial> CurrentMoveMaterialList
        {
            get
            {
                if (HttpContext.Session[_CURRENTMOVEMATERIALLIST] != null)
                    return (List<MoveMaterial>)HttpContext.Session[_CURRENTMOVEMATERIALLIST];
                return new List<MoveMaterial>();
            }
            set
            {
                HttpContext.Session[_CURRENTMOVEMATERIALLIST] = value;
            }
        }

        private List<RequisitionImport> CurrentRequisitionList
        {
            get
            {
                if (HttpContext.Session[_CURRENTREQUISITIONLIST] != null)
                    return (List<RequisitionImport>)HttpContext.Session[_CURRENTREQUISITIONLIST];
                return new List<RequisitionImport>();
            }
            set
            {
                HttpContext.Session[_CURRENTREQUISITIONLIST] = value;
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
        private List<ItemManufacturer> CurrentItemManufacturerList
        {
            get
            {
                if (HttpContext.Session[_CURRENTITEMMANUFACTURERSESSIONKEY] != null)
                    return (List<ItemManufacturer>)HttpContext.Session[_CURRENTITEMMANUFACTURERSESSIONKEY];
                return new List<ItemManufacturer>();
            }
            set
            {
                HttpContext.Session[_CURRENTITEMMANUFACTURERSESSIONKEY] = value;
            }
        }
        private List<ItemSupplier> CurrentItemSupplierList
        {
            get
            {
                if (HttpContext.Session[_CURRENTITEMSUPPLIERSESSIONKEY] != null)
                    return (List<ItemSupplier>)HttpContext.Session[_CURRENTITEMSUPPLIERSESSIONKEY];
                return new List<ItemSupplier>();
            }
            set
            {
                HttpContext.Session[_CURRENTITEMSUPPLIERSESSIONKEY] = value;
            }
        }
        private List<ImportBarcodeMaster> CurrentBarcodeList
        {
            get
            {
                if (HttpContext.Session[_CURRENTBARCODESESSIONKEY] != null)
                    return (List<ImportBarcodeMaster>)HttpContext.Session[_CURRENTBARCODESESSIONKEY];
                return new List<ImportBarcodeMaster>();
            }
            set
            {
                HttpContext.Session[_CURRENTBARCODESESSIONKEY] = value;
            }
        }
        private List<UDFMasterMain> CurrentUDFList
        {
            get
            {
                if (HttpContext.Session[_CURRENTUDFSESSIONKEY] != null)
                    return (List<UDFMasterMain>)HttpContext.Session[_CURRENTUDFSESSIONKEY];
                return new List<UDFMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTUDFSESSIONKEY] = value;
            }
        }
        private List<ProjectMasterMain> CurrentProjectMasterList
        {
            get
            {
                if (HttpContext.Session[_CURRENTPROJECTMASTERSESSIONKEY] != null)
                    return (List<ProjectMasterMain>)HttpContext.Session[_CURRENTPROJECTMASTERSESSIONKEY];
                return new List<ProjectMasterMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTPROJECTMASTERSESSIONKEY] = value;
            }
        }

        private List<AssetToolScheduler> CurrentAssetToolSchedulerList
        {
            get
            {
                if (HttpContext.Session[_CURRENTASSETTOOLSCHEDULERLIST] != null)
                    return (List<AssetToolScheduler>)HttpContext.Session[_CURRENTASSETTOOLSCHEDULERLIST];
                return new List<AssetToolScheduler>();
            }
            set
            {
                HttpContext.Session[_CURRENTASSETTOOLSCHEDULERLIST] = value;
            }
        }

        private List<ToolCheckInCheckOut> CurrentToolCheckInCheckOut
        {
            get
            {
                if (HttpContext.Session[_CURRENTTOOLCHECKINCHECKOUTLIST] != null)
                    return (List<ToolCheckInCheckOut>)HttpContext.Session[_CURRENTTOOLCHECKINCHECKOUTLIST];
                return new List<ToolCheckInCheckOut>();
            }
            set
            {
                HttpContext.Session[_CURRENTTOOLCHECKINCHECKOUTLIST] = value;
            }
        }


        private List<ToolAssetQuantityMain> CurrentToolAdjustmentCount
        {
            get
            {
                if (HttpContext.Session[_CURRENTTOOLADJUSTMENTCOUNT] != null)
                    return (List<ToolAssetQuantityMain>)HttpContext.Session[_CURRENTTOOLADJUSTMENTCOUNT];
                return new List<ToolAssetQuantityMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTTOOLADJUSTMENTCOUNT] = value;
            }
        }

        private List<ToolImageImport> CurrentToolImageImport
        {
            get
            {
                if (HttpContext.Session[_CURRENTTOOLIMAGEIMPORTLIST] != null)
                    return (List<ToolImageImport>)HttpContext.Session[_CURRENTTOOLIMAGEIMPORTLIST];
                return new List<ToolImageImport>();
            }
            set
            {
                HttpContext.Session[_CURRENTTOOLIMAGEIMPORTLIST] = value;
            }
        }
        private List<EnterpriseQLImport> CurrentEnterpriseQuickList
        {
            get
            {
                if (HttpContext.Session[_CURRENTENTERPRISEQUICKLIST] != null)
                    return (List<EnterpriseQLImport>)HttpContext.Session[_CURRENTENTERPRISEQUICKLIST];
                return new List<EnterpriseQLImport>();
            }
            set
            {
                HttpContext.Session[_CURRENTENTERPRISEQUICKLIST] = value;
            }
        }
        private List<QuoteMasterItemsMain> CurrentQuoteMasterList
        {
            get
            {
                if (HttpContext.Session[_CURRENTQUOTEMASTERLIST] != null)
                    return (List<QuoteMasterItemsMain>)HttpContext.Session[_CURRENTQUOTEMASTERLIST];
                return new List<QuoteMasterItemsMain>();
            }
            set
            {
                HttpContext.Session[_CURRENTQUOTEMASTERLIST] = value;
            }
        }

        private List<SupplierCatalogImport> CurrentSupplierCatalogList
        {
            get
            {
                if (HttpContext.Session[_CURRENTSUPPLIERCATALOGLIST] != null)
                    return (List<SupplierCatalogImport>)HttpContext.Session[_CURRENTSUPPLIERCATALOGLIST];
                return new List<SupplierCatalogImport>();
            }
            set
            {
                HttpContext.Session[_CURRENTSUPPLIERCATALOGLIST] = value;
            }
        }

        #endregion

        CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
        public ActionResult ResetSession(string key, string value)
        {
            Session["importedData"] = null;
            //Session["CurModuleValue"] = ImportMastersDTO.TableName.BinMaster;
            //Session["CurModule"] = ImportMastersDTO.TableName.BinMaster;
            return this.Json(new { success = true });
        }
        public ActionResult MultiRoomImport()
        {
            Session["importedData"] = null;

            if (TempData["CurModule"] == null)
            {

                Session["CurModuleValue"] = ImportMastersDTO.TableName.BinMaster;
                Session["CurModule"] = ImportMastersDTO.TableName.BinMaster;
            }
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
            obj.Text = ResImportMasters.Categories;
            obj.Value = ((int)SessionHelper.ModuleList.CategoryMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.CostUOM;
            obj.Value = ((int)SessionHelper.ModuleList.CostUOMMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.Customers;
            obj.Value = ((int)SessionHelper.ModuleList.CustomerMaster).ToString();
            lstItem.Add(obj);

            //obj = new SelectListItem();
            //obj.Text = "Freight Types";
            //obj.Value = ((int)SessionHelper.ModuleList.FreightTypeMaster).ToString();
            //lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.GLAccounts;
            obj.Value = ((int)SessionHelper.ModuleList.GLAccountsMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.InventoryClassification;
            obj.Value = ((int)SessionHelper.ModuleList.InventoryClassificationMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.AdjustmentCount;// "Item Locations Quantity";
            obj.Value = ((int)SessionHelper.ModuleList.BinMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.ItemLocations;
            obj.Value = ((int)SessionHelper.ModuleList.eVMISetup).ToString();
            lstItem.Add(obj);
            obj = new SelectListItem();
            obj.Text = ResImportMasters.Manufacturers;
            obj.Value = ((int)SessionHelper.ModuleList.ManufacturerMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.MeasurementTerms;
            obj.Value = ((int)SessionHelper.ModuleList.MeasurementTermMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.ShipVias;
            obj.Value = ((int)SessionHelper.ModuleList.ShipViaMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.Suppliers;
            obj.Value = ((int)SessionHelper.ModuleList.SupplierMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.Technicians;
            obj.Value = ((int)SessionHelper.ModuleList.TechnicianMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.ToolCategories;
            obj.Value = ((int)SessionHelper.ModuleList.ToolCategory).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.ToolLocations;
            obj.Value = ((int)SessionHelper.ModuleList.LocationMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.Units;
            obj.Value = ((int)SessionHelper.ModuleList.UnitMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.Items;
            obj.Value = ((int)SessionHelper.ModuleList.ItemMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.EditItems;
            obj.Value = ((int)SessionHelper.ModuleList.EditItemMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.AssetToolSchedulerMapping;
            obj.Value = ((int)SessionHelper.ModuleList.AssetToolSchedulerMapping).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.Assets;
            obj.Value = ((int)SessionHelper.ModuleList.Assets).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.Tools;
            obj.Value = ((int)SessionHelper.ModuleList.ToolMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.QuickList;
            obj.Value = ((int)SessionHelper.ModuleList.QuickListPermission).ToString();
            lstItem.Add(obj);

            //obj = new SelectListItem();
            //obj.Text = "Inventory Location";
            //obj.Value = "110";
            //lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.CommonBOMItems;
            obj.Value = ((int)SessionHelper.ModuleList.BOMItemMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.Kits;
            obj.Value = ((int)SessionHelper.ModuleList.Kits).ToString();
            lstItem.Add(obj);


            obj = new SelectListItem();
            obj.Text = ResImportMasters.ItemManufacturer;
            obj.Value = ((int)SessionHelper.ModuleList.ItemManufacturer).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.ItemSupplier;
            obj.Value = ((int)SessionHelper.ModuleList.ItemSupplier).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.BarcodeAssociations;
            obj.Value = ((int)SessionHelper.ModuleList.BarcodeMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.UDF;
            obj.Value = ((int)SessionHelper.ModuleList.UDF).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.ProjectSpends;
            obj.Value = ((int)SessionHelper.ModuleList.ProjectMaster).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.ItemQuantityImportwithCost;
            obj.Value = ((int)SessionHelper.ModuleList.ItemLocationQty).ToString();
            lstItem.Add(obj);



            obj = new SelectListItem();
            obj.Text = ResImportMasters.ManualCount;
            obj.Value = ((int)SessionHelper.ModuleList.ManualCount).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.WorkOrder;
            obj.Value = ((int)SessionHelper.ModuleList.WorkOrders).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.PullImport;
            obj.Value = ((int)SessionHelper.ModuleList.PullImport).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.PullImportWithLotSerial;
            obj.Value = ((int)SessionHelper.ModuleList.PullImportWithLotSerial).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.ItemLocationChange;
            obj.Value = ((int)SessionHelper.ModuleList.ItemLocationChange).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.PullWithSameQty;
            obj.Value = ((int)SessionHelper.ModuleList.PullImportWithSameQty).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.AssetToolScheduler;
            obj.Value = ((int)SessionHelper.ModuleList.AssetToolScheduler).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.PastMaintenanceDue;
            obj.Value = ((int)SessionHelper.ModuleList.AssetMaintenance).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.ToolCheckInCheckOut;
            obj.Value = ((int)SessionHelper.ModuleList.ToolCheckInCheckOut).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem();
            obj.Text = ResImportMasters.ToolAdjustmentCount;
            obj.Value = ((int)SessionHelper.ModuleList.ToolAdjustmentCount).ToString();
            lstItem.Add(obj);

            if (SessionHelper.AllowToolOrdering)
            {
                obj = new SelectListItem();
                obj.Text = ResImportMasters.ToolCertificationImages;
                obj.Value = ((int)SessionHelper.ModuleList.ToolCertificationImages).ToString();
                lstItem.Add(obj);
            }

            obj = new SelectListItem();
            obj.Text = ResImportMasters.Order;
            obj.Value = ((int)SessionHelper.ModuleList.Orders).ToString();
            lstItem.Add(obj);

            obj = new SelectListItem
            {
                Text = ResImportMasters.MoveMaterial,
                Value = ((int)SessionHelper.ModuleList.MoveMaterial).ToString()
            };
            lstItem.Add(obj);

            obj = new SelectListItem
            {
                Text = ResImportMasters.EnterpriseQuickList,
                Value = ((int)SessionHelper.ModuleList.EnterpriseQuickList).ToString()
            };
            lstItem.Add(obj);
            obj = new SelectListItem
            {
                Text = ResImportMasters.Requisition,
                Value = ((int)SessionHelper.ModuleList.Requisitions).ToString()
            };
            lstItem.Add(obj);
            obj = new SelectListItem
            {
                Text = ResImportMasters.Quote,
                Value = ((int)SessionHelper.ModuleList.Quote).ToString()
            };
            lstItem.Add(obj);
            obj = new SelectListItem
            {
                Text = ResImportMasters.SupplierCatalog,
                Value = ((int)SessionHelper.ModuleList.Suppliercatalog).ToString()
            };
            lstItem.Add(obj);
            obj = new SelectListItem
            {
                Text = ResImportMasters.Returns,
                Value = ((int)SessionHelper.ModuleList.ReturnOrder).ToString()
            };
            lstItem.Add(obj);
            obj = new SelectListItem
            {
                Text = ResImportMasters.CommonBOMToItem,
                Value = ((int)SessionHelper.ModuleList.CommonBOMToItem).ToString()
            };
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
                            Session["ErrorMessage"] = ResImportMasters.ErrorInvalidFile;
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
                                int CoulumnCounter = 1;
                                foreach (var item in value)
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
                                            //tmpstringda = tmpstringda.TrimStart(trimchar);
                                            tmpstringda = (tmpstringda ?? string.Empty).TrimStart(trimchar);//.Replace("\"", "&quot;").Replace("'", "&apos;");
                                            //tmpstringda = Server.HtmlEncode(tmpstringda);
                                            tmpstringda = HttpUtility.HtmlEncode(tmpstringda);
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


        public string SaveUploadedFile(HttpPostedFileBase uploadFile, bool isOffline = false)
        {
            string path = string.Empty;
            // Verify that the user selected a file
            if (uploadFile != null && uploadFile.ContentLength > 0)
            {
                string RenamedFileName = Session["CurModule"].ToString() + "_" + ((SessionHelper.EnterPriceName ?? string.Empty) == string.Empty ? ("No Enterprise(" + SessionHelper.EnterPriceID + ")") : SessionHelper.EnterPriceName + "(" + SessionHelper.EnterPriceID + ")") + "_" + SessionHelper.CompanyName + "(" + SessionHelper.CompanyID + ")" + "_" + SessionHelper.RoomName + "(" + SessionHelper.RoomID + ")" + "_" + DateTime.Now.Ticks.ToString();
                RenamedFileName = GetWithoutSpecChar(RenamedFileName);
                // extract only the fielname
                string fileName = Path.GetFileName(uploadFile.FileName);
                // store the file inside ~/App_Data/uploads folder
                string[] strfilename = fileName.Split('.');

                if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
                {
                    RenamedFileName = RenamedFileName + "_O_" + fileName.Substring(0, fileName.Length - 4);
                    if (RenamedFileName.Substring(0, RenamedFileName.Length - 4).Length > 160)
                        RenamedFileName = RenamedFileName.Substring(0, 160);

                    if (isOffline)
                        path = Path.Combine(Server.MapPath("~/Uploads/Import/CSV/" + Session["CurModule"].ToString() + "/" + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID), RenamedFileName + ".csv");
                    else
                        path = Path.Combine(Server.MapPath("~/Uploads/Import/CSV"), RenamedFileName + ".csv");
                }
                else if (strfilename[strfilename.Length - 1].ToUpper() == "XLS")
                {
                    path = Path.Combine(Server.MapPath("~/Uploads/Import/Excel"), RenamedFileName + ".xls");
                }
                //Excel
                if (!string.IsNullOrEmpty(path.Trim()))
                {
                    uploadFile.SaveAs(path);
                }
            }
            return path;
        }

        public string GetWithoutSpecChar(string FileName)
        {
            string strReplace = "_";    //   \/:*?"<>|
            if (FileName.ToLower().Trim().Contains("\\"))
                FileName = FileName.Replace("\\", strReplace);
            else if (FileName.ToLower().Trim().Contains("/"))
                FileName = FileName.Replace("/", strReplace);
            else if (FileName.ToLower().Trim().Contains(":"))
                FileName = FileName.Replace(":", strReplace);
            else if (FileName.ToLower().Trim().Contains("*"))
                FileName = FileName.Replace("*", strReplace);
            else if (FileName.ToLower().Trim().Contains("?"))
                FileName = FileName.Replace("?", strReplace);
            else if (FileName.ToLower().Trim().Contains("\""))
                FileName = FileName.Replace("\"", strReplace);
            else if (FileName.ToLower().Trim().Contains("<"))
                FileName = FileName.Replace("<", strReplace);
            else if (FileName.ToLower().Trim().Contains(">"))
                FileName = FileName.Replace(">", strReplace);
            else if (FileName.ToLower().Trim().Contains("|"))
                FileName = FileName.Replace("|", strReplace);
            return FileName;
        }

        #region Get Import

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MultiRoomImport(HttpPostedFileBase uploadFile, HttpPostedFileBase uploadZIPFile, FormCollection formCollection)
        {
            Session["importedData"] = null;
            StringBuilder strValidations = new StringBuilder(string.Empty);

            string vDefaultSupplier = "";
            string vDefaultUOM = "";
            string vDefaultLocation = "";
            double RoomGlobMarkupParts = 0;
            double RoomGlobMarkupLabor = 0;

            RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            //RoomDTO objRoomDTO = objRoomDal.GetRoomByIDPlain(SessionHelper.RoomID);
            string columnList = "ID,RoomName,DefaultSupplierID,GlobMarkupParts,GlobMarkupLabor,DefaultBinID";
            RoomDTO objRoomDTO = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
            Session["importedRoomIds"] = formCollection["selectedRoomsToImport"];

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
                    //objBinDTO = objBinDal.GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, objRoomDTO.DefaultBinID.GetValueOrDefault(0),null,null).FirstOrDefault();
                    if (objBinDTO != null)
                        vDefaultLocation = objBinDTO.BinNumber;
                }

                UnitMasterDAL objUnitDal = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
                UnitMasterDTO objUnit = objUnitDal.GetUnitByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, "EA");
                if (objUnit != null && objUnit.ID > 0)
                    vDefaultUOM = objUnit.Unit;

                if (objRoomDTO.GlobMarkupParts.GetValueOrDefault(0) > 0)
                {
                    RoomGlobMarkupParts = objRoomDTO.GlobMarkupParts.GetValueOrDefault(0);
                }
                else
                    RoomGlobMarkupParts = 0;

                if (objRoomDTO.GlobMarkupLabor.GetValueOrDefault(0) > 0)
                    RoomGlobMarkupLabor = objRoomDTO.GlobMarkupLabor.GetValueOrDefault(0);
                else
                    RoomGlobMarkupLabor = 0;
            }

            try
            {
                if (uploadFile.ContentLength > 0)
                {
                    byte[] data;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        uploadFile.InputStream.CopyTo(ms);
                        data = ms.ToArray();
                    }

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
                    ImportDAL importDAL = new ImportDAL(SessionHelper.EnterPriseDBName);
                    OfflineImportFileHistoryDTO offlineImportFileHistory = new OfflineImportFileHistoryDTO();
                    offlineImportFileHistory.ModuleId = Convert.ToInt32(formCollection["ddlModule"]);


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
                    if (dtCSV != null && dtCSV.Rows.Count > 0)
                    {
                        bool isOffline = !string.IsNullOrEmpty(formCollection["isOffline"]);
                        if (data != null && data.Length > 0)
                        {
                            SaveStreamAsFile(data, fileName, offlineImportFileHistory, isOffline);
                            if (isOffline)
                            {
                                TempData["SuccessMessage"] = true;
                                try
                                {
                                    importDAL.InsertUpdateFileHistory(offlineImportFileHistory);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                                return View();
                            }
                            //SaveUploadedFile(uploadFile, isOffline);
                        }
                        if (uploadZIPFile != null && uploadZIPFile.ContentLength > 0)
                        {
                            SaveUploadedFile(uploadZIPFile);
                        }
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
                            #region Count Adjustment master
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.BinMaster.ToString())
                            {
                                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                                List<InventoryLocationMain> lstImport = new List<InventoryLocationMain>();
                                foreach (DataRow item in list)
                                {
                                    InventoryLocationMain obj = new InventoryLocationMain();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
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

                                            //if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.Cost.ToString()))
                                            //{
                                            //    if (item[CommonUtility.ImportInventoryLocationColumn.Cost.ToString()] == null || Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.Cost.ToString()]) == "")
                                            //    {
                                            //        obj.Cost = 0;
                                            //    }
                                            //    else
                                            //    {
                                            //        double _costValue = 0.0;
                                            //        if (double.TryParse(item[CommonUtility.ImportInventoryLocationColumn.Cost.ToString()].ToString(), out _costValue))
                                            //            obj.Cost = _costValue;
                                            //        else
                                            //            obj.Cost = 0;
                                            //    }

                                            //}

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
                                                    string _expirationDt = item[CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()].ToString().Split(' ')[0];
                                                    DateTime.TryParseExact(_expirationDt, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);
                                                    if (dt != DateTime.MinValue)
                                                    {
                                                        obj.Expiration = dt.ToString(SessionHelper.RoomDateFormat);
                                                        obj.displayExpiration = dt.ToString(SessionHelper.RoomDateFormat);
                                                    }
                                                    else
                                                    {
                                                        obj.Expiration = null;
                                                        obj.displayExpiration = null;
                                                        obj.Reason = string.Format(ResImportMasters.DateShouldBeInFormat, ResItemLocationDetails.ExpirationDate, SessionHelper.RoomDateFormat);
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
                                                        obj.Reason = string.Format(ResImportMasters.DateShouldBeInFormat, ResItemLocationDetails.ReceivedDate, SessionHelper.RoomDateFormat);
                                                        obj.Status = "Fail";
                                                    }
                                                }
                                                else
                                                {
                                                    obj.Received = null;
                                                }
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ItemDescription.ToString()))
                                            {
                                                obj.ItemDescription = item[CommonUtility.ImportInventoryLocationColumn.ItemDescription.ToString()].ToString();
                                            }
                                            if (objItemMasterDTO == null || obj.ItemNumber != objItemMasterDTO.ItemNumber)
                                            {
                                                objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByItemNumberPlain(HttpUtility.HtmlDecode(obj.ItemNumber), SessionHelper.RoomID, SessionHelper.CompanyID);
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
                                                //if (obj.customerownedquantity != 1 && !objItemMasterDTO.Consignment)
                                                //{
                                                //    obj.customerownedquantity = 1;
                                                //}
                                                //if (obj.consignedquantity != 1 && objItemMasterDTO.Consignment)
                                                //{
                                                //    obj.consignedquantity = 1;
                                                //}
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
                                                //if (obj.customerownedquantity != 1 && !objItemMasterDTO.Consignment)
                                                //{
                                                //    obj.customerownedquantity = 1;
                                                //}
                                                //if (obj.consignedquantity != 1 && objItemMasterDTO.Consignment)
                                                //{
                                                //    obj.consignedquantity = 1;
                                                //}
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

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportInventoryLocationColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportInventoryLocationColumn.UDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportInventoryLocationColumn.UDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportInventoryLocationColumn.UDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportInventoryLocationColumn.UDF5.ToString()].ToString();
                                            }

                                            //InventoryLocationMain oCheckExist = lstImport.Where(x => x.ItemNumber == obj.ItemNumber && x.BinNumber == obj.BinNumber && x.SerialNumber == obj.SerialNumber && x.LotNumber == obj.LotNumber).FirstOrDefault();

                                            //if (oCheckExist != null)
                                            //{
                                            //    lstImport.Remove(oCheckExist);
                                            //}


                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }
                                lstImport = (from ilq in lstImport
                                             group ilq by new
                                             {
                                                 ilq.BinNumber,
                                                 ilq.ItemNumber,
                                                 ilq.LotNumber,
                                                 ilq.SerialNumber,
                                                 ilq.ItemGUID,
                                                 ilq.Expiration,
                                                 ilq.ItemDescription,
                                                 ilq.UDF1,
                                                 ilq.UDF2,
                                                 ilq.UDF3,
                                                 ilq.UDF4,
                                                 ilq.UDF5
                                             } into groupedilq
                                             select new InventoryLocationMain
                                             {
                                                 BinNumber = groupedilq.Key.BinNumber,
                                                 CompanyID = SessionHelper.CompanyID,
                                                 consignedquantity = groupedilq.Sum(t => (t.consignedquantity ?? 0)),
                                                 Created = DateTime.UtcNow,
                                                 CreatedBy = SessionHelper.UserID,
                                                 customerownedquantity = groupedilq.Sum(t => (t.customerownedquantity ?? 0)),
                                                 GUID = Guid.NewGuid(),
                                                 InsertedFrom = "import",
                                                 IsArchived = false,
                                                 IsDeleted = false,
                                                 ItemGUID = groupedilq.Key.ItemGUID,
                                                 ItemNumber = groupedilq.Key.ItemNumber,
                                                 LastUpdatedBy = SessionHelper.UserID,
                                                 LotNumber = groupedilq.Key.LotNumber,
                                                 Room = SessionHelper.RoomID,
                                                 SerialNumber = groupedilq.Key.SerialNumber,
                                                 Updated = DateTime.UtcNow,
                                                 Status = string.Join(",", groupedilq.Select(t => t.Status).ToArray()),
                                                 Reason = string.Join(",", groupedilq.Select(t => t.Reason).ToArray()),
                                                 Expiration = groupedilq.Key.Expiration,
                                                 ItemDescription = groupedilq.Key.ItemDescription,
                                                 UDF1 = groupedilq.Key.UDF1,
                                                 UDF2 = groupedilq.Key.UDF2,
                                                 UDF3 = groupedilq.Key.UDF3,
                                                 UDF4 = groupedilq.Key.UDF4,
                                                 UDF5 = groupedilq.Key.UDF5
                                             }).ToList();
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

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.IsStagingLocation.ToString()))
                                            {
                                                Boolean IsStagingLocation = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.IsStagingLocation.ToString()]), out IsStagingLocation);
                                                obj.IsStagingLocation = IsStagingLocation;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.IsEnforceDefaultPullQuantity.ToString()))
                                            {
                                                Boolean IsEnforceDefaultPullQuantity = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.IsEnforceDefaultPullQuantity.ToString()]), out IsEnforceDefaultPullQuantity);
                                                obj.IsEnforceDefaultPullQuantity = IsEnforceDefaultPullQuantity;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.IsEnforceDefaultReorderQuantity.ToString()))
                                            {
                                                Boolean IsEnforceDefaultReorderQuantity = false;
                                                Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.IsEnforceDefaultReorderQuantity.ToString()]), out IsEnforceDefaultReorderQuantity);
                                                obj.IsEnforceDefaultReorderQuantity = IsEnforceDefaultReorderQuantity;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.DefaultPullQuantity.ToString()))
                                            {
                                                if (item[CommonUtility.ImportInventoryLocationColumn.DefaultPullQuantity.ToString()] == null || Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.DefaultPullQuantity.ToString()]) == "")
                                                {
                                                    obj.DefaultPullQuantity = 0;
                                                }
                                                else
                                                {
                                                    string strMinQty = string.Empty;
                                                    double MinQtyValue = 0;
                                                    strMinQty = item[CommonUtility.ImportInventoryLocationColumn.DefaultPullQuantity.ToString()].ToString();

                                                    if (Double.TryParse(strMinQty, out MinQtyValue))
                                                        obj.DefaultPullQuantity = MinQtyValue;
                                                    else
                                                        obj.DefaultPullQuantity = 0;
                                                }

                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.DefaultReorderQuantity.ToString()))
                                            {
                                                if (item[CommonUtility.ImportInventoryLocationColumn.DefaultReorderQuantity.ToString()] == null || Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.DefaultReorderQuantity.ToString()]) == "")
                                                {
                                                    obj.DefaultReorderQuantity = 0;
                                                }
                                                else
                                                {
                                                    string strMinQty = string.Empty;
                                                    double MinQtyValue = 0;
                                                    strMinQty = item[CommonUtility.ImportInventoryLocationColumn.DefaultReorderQuantity.ToString()].ToString();

                                                    if (Double.TryParse(strMinQty, out MinQtyValue))
                                                        obj.DefaultReorderQuantity = MinQtyValue;
                                                    else
                                                        obj.DefaultReorderQuantity = 0;
                                                }

                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.BinUDF1.ToString()))
                                            {
                                                obj.BinUDF1 = item[CommonUtility.ImportInventoryLocationColumn.BinUDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.BinUDF2.ToString()))
                                            {
                                                obj.BinUDF2 = item[CommonUtility.ImportInventoryLocationColumn.BinUDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.BinUDF3.ToString()))
                                            {
                                                obj.BinUDF3 = item[CommonUtility.ImportInventoryLocationColumn.BinUDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.BinUDF4.ToString()))
                                            {
                                                obj.BinUDF4 = item[CommonUtility.ImportInventoryLocationColumn.BinUDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.BinUDF5.ToString()))
                                            {
                                                obj.BinUDF5 = item[CommonUtility.ImportInventoryLocationColumn.BinUDF5.ToString()].ToString();
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
                                        if (item[CommonUtility.ImportTechnicianColumn.TechnicianCode.ToString()].ToString() != "")
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

                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.Email.ToString()))
                                            {
                                                obj.Email = item[CommonUtility.ImportSupplierColumn.Email.ToString()].ToString();
                                            }

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
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.UDF6.ToString()))
                                            {
                                                obj.UDF6 = item[CommonUtility.ImportSupplierColumn.UDF6.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.UDF7.ToString()))
                                            {
                                                obj.UDF7 = item[CommonUtility.ImportSupplierColumn.UDF7.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.UDF8.ToString()))
                                            {
                                                obj.UDF8 = item[CommonUtility.ImportSupplierColumn.UDF8.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.UDF9.ToString()))
                                            {
                                                obj.UDF9 = item[CommonUtility.ImportSupplierColumn.UDF9.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.UDF10.ToString()))
                                            {
                                                obj.UDF10 = item[CommonUtility.ImportSupplierColumn.UDF10.ToString()].ToString();
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

                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.AccountCountry.ToString()))
                                            {
                                                obj.AccountCountry = item[CommonUtility.ImportSupplierColumn.AccountCountry.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierColumn.AccountShipToID.ToString()))
                                            {
                                                obj.AccountShipToID = item[CommonUtility.ImportSupplierColumn.AccountShipToID.ToString()].ToString();
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

                                                if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemType.ToString()))
                                                {
                                                    if (Convert.ToString(item[CommonUtility.ImportItemColumn.ItemType.ToString()]) == "4")
                                                    {
                                                        if (RoomGlobMarkupLabor > 0)
                                                        {
                                                            if (string.IsNullOrWhiteSpace(Convert.ToString(item[CommonUtility.ImportItemColumn.Markup.ToString()])))
                                                            {
                                                                obj.Markup = RoomGlobMarkupLabor;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (RoomGlobMarkupParts > 0)
                                                        {
                                                            if (string.IsNullOrWhiteSpace(Convert.ToString(item[CommonUtility.ImportItemColumn.Markup.ToString()])))
                                                            {
                                                                obj.Markup = RoomGlobMarkupParts;
                                                            }
                                                        }
                                                    }
                                                }
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SellPrice.ToString()))
                                            {
                                                double SellPrice;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.SellPrice.ToString()]), out SellPrice))
                                                    obj.SellPrice = SellPrice;
                                                else
                                                    obj.SellPrice = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ExtendedCost.ToString()))
                                            {
                                                double ExtendedCost;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.ExtendedCost.ToString()]), out ExtendedCost))
                                                    obj.DispExtendedCost = ExtendedCost;
                                                else
                                                    obj.DispExtendedCost = 0;
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

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.StagedQuantity.ToString()))
                                            {
                                                double StagedQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.StagedQuantity.ToString()]), out StagedQuantity))
                                                    obj.DispStagedQuantity = StagedQuantity;
                                                else
                                                    obj.DispStagedQuantity = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.InTransitquantity.ToString()))
                                            {
                                                double InTransitquantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.InTransitquantity.ToString()]), out InTransitquantity))
                                                    obj.DispInTransitquantity = InTransitquantity;
                                                else
                                                    obj.DispInTransitquantity = 0;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OnOrderQuantity.ToString()))
                                            {
                                                double OnOrderQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OnOrderQuantity.ToString()]), out OnOrderQuantity))
                                                    obj.DispOnOrderQuantity = OnOrderQuantity;
                                                else
                                                    obj.DispOnOrderQuantity = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OnTransferQuantity.ToString()))
                                            {
                                                double OnTransferQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OnTransferQuantity.ToString()]), out OnTransferQuantity))
                                                    obj.DispOnTransferQuantity = OnTransferQuantity;
                                                else
                                                    obj.DispOnTransferQuantity = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SuggestedOrderQuantity.ToString()))
                                            {
                                                double SuggestedOrderQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.SuggestedOrderQuantity.ToString()]), out SuggestedOrderQuantity))
                                                    obj.DispSuggestedOrderQuantity = SuggestedOrderQuantity;
                                                else
                                                    obj.DispSuggestedOrderQuantity = 0;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.RequisitionedQuantity.ToString()))
                                            {
                                                double RequisitionedQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.RequisitionedQuantity.ToString()]), out RequisitionedQuantity))
                                                    obj.DispRequisitionedQuantity = RequisitionedQuantity;
                                                else
                                                    obj.DispRequisitionedQuantity = 0;

                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.AverageUsage.ToString()))
                                            {
                                                double AverageUsage;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.AverageUsage.ToString()]), out AverageUsage))
                                                    obj.DispAverageUsage = AverageUsage;
                                                else
                                                    obj.DispAverageUsage = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Turns.ToString()))
                                            {
                                                double Turns;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Turns.ToString()]), out Turns))
                                                    obj.DispTurns = Turns;
                                                else
                                                    obj.DispTurns = 0;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OnHandQuantity.ToString()))
                                            {
                                                double OnHandQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OnHandQuantity.ToString()]), out OnHandQuantity))
                                                {
                                                    //obj.DispOnHandQuantity = OnHandQuantity;
                                                    obj.OnHandQuantity = OnHandQuantity;
                                                }
                                                else
                                                {
                                                    //obj.DispOnHandQuantity = 0;
                                                    obj.OnHandQuantity = 0;
                                                }
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
                                                Boolean ItemLevelMinMaxQtyRequired = true;
                                                if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsItemLevelMinMaxQtyRequired.ToString()]), out ItemLevelMinMaxQtyRequired))
                                                    obj.IsItemLevelMinMaxQtyRequired = ItemLevelMinMaxQtyRequired;
                                                else
                                                    obj.IsItemLevelMinMaxQtyRequired = true;
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
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF6.ToString()))
                                            {
                                                obj.UDF6 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF6.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF7.ToString()))
                                            {
                                                obj.UDF7 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF7.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF8.ToString()))
                                            {
                                                obj.UDF8 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF8.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF9.ToString()))
                                            {
                                                obj.UDF9 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF9.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF10.ToString()))
                                            {
                                                obj.UDF10 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF10.ToString()]);
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.PerItemCost.ToString()))
                                            {
                                                double PerItemCost;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.PerItemCost.ToString()]), out PerItemCost))
                                                    obj.PerItemCost = PerItemCost;
                                                else
                                                    obj.PerItemCost = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OutTransferQuantity.ToString()))
                                            {
                                                double OutTransferQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OutTransferQuantity.ToString()]), out OutTransferQuantity))
                                                    obj.OutTransferQuantity = OutTransferQuantity;
                                                else
                                                    obj.OutTransferQuantity = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsAllowOrderCostuom.ToString()))
                                            {
                                                Boolean IsAllowOrderCostuom = false;
                                                if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsAllowOrderCostuom.ToString()]), out IsAllowOrderCostuom))
                                                    obj.IsAllowOrderCostuom = IsAllowOrderCostuom;
                                                else
                                                    obj.IsAllowOrderCostuom = false;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.eLabelKey.ToString()))
                                            {
                                                obj.eLabelKey = item[CommonUtility.ImportItemColumn.eLabelKey.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.EnrichedProductData.ToString()))
                                            {
                                                obj.EnrichedProductData = item[CommonUtility.ImportItemColumn.EnrichedProductData.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.EnhancedDescription.ToString()))
                                            {
                                                obj.EnhancedDescription = item[CommonUtility.ImportItemColumn.EnhancedDescription.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.POItemLineNumber.ToString()))
                                            {
                                                int POItemLineNumber;
                                                if (int.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.POItemLineNumber.ToString()]), out POItemLineNumber))
                                                    obj.POItemLineNumber = POItemLineNumber;
                                                else
                                                    obj.POItemLineNumber = null;
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
                            #region Edit Item Master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.EditItemMaster.ToString())
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

                                                if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemType.ToString()))
                                                {
                                                    if (Convert.ToString(item[CommonUtility.ImportItemColumn.ItemType.ToString()]) == "4")
                                                    {
                                                        if (RoomGlobMarkupLabor > 0)
                                                        {
                                                            if (string.IsNullOrWhiteSpace(Convert.ToString(item[CommonUtility.ImportItemColumn.Markup.ToString()])))
                                                            {
                                                                obj.Markup = RoomGlobMarkupLabor;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (RoomGlobMarkupParts > 0)
                                                        {
                                                            if (string.IsNullOrWhiteSpace(Convert.ToString(item[CommonUtility.ImportItemColumn.Markup.ToString()])))
                                                            {
                                                                obj.Markup = RoomGlobMarkupParts;
                                                            }
                                                        }
                                                    }
                                                }
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SellPrice.ToString()))
                                            {
                                                double SellPrice;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.SellPrice.ToString()]), out SellPrice))
                                                    obj.SellPrice = SellPrice;
                                                else
                                                    obj.SellPrice = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ExtendedCost.ToString()))
                                            {
                                                double ExtendedCost;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.ExtendedCost.ToString()]), out ExtendedCost))
                                                    obj.DispExtendedCost = ExtendedCost;
                                                else
                                                    obj.DispExtendedCost = 0;
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

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.StagedQuantity.ToString()))
                                            {
                                                double StagedQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.StagedQuantity.ToString()]), out StagedQuantity))
                                                    obj.DispStagedQuantity = StagedQuantity;
                                                else
                                                    obj.DispStagedQuantity = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.InTransitquantity.ToString()))
                                            {
                                                double InTransitquantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.InTransitquantity.ToString()]), out InTransitquantity))
                                                    obj.DispInTransitquantity = InTransitquantity;
                                                else
                                                    obj.DispInTransitquantity = 0;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OnOrderQuantity.ToString()))
                                            {
                                                double OnOrderQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OnOrderQuantity.ToString()]), out OnOrderQuantity))
                                                    obj.DispOnOrderQuantity = OnOrderQuantity;
                                                else
                                                    obj.DispOnOrderQuantity = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OnTransferQuantity.ToString()))
                                            {
                                                double OnTransferQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OnTransferQuantity.ToString()]), out OnTransferQuantity))
                                                    obj.DispOnTransferQuantity = OnTransferQuantity;
                                                else
                                                    obj.DispOnTransferQuantity = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.SuggestedOrderQuantity.ToString()))
                                            {
                                                double SuggestedOrderQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.SuggestedOrderQuantity.ToString()]), out SuggestedOrderQuantity))
                                                    obj.DispSuggestedOrderQuantity = SuggestedOrderQuantity;
                                                else
                                                    obj.DispSuggestedOrderQuantity = 0;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.RequisitionedQuantity.ToString()))
                                            {
                                                double RequisitionedQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.RequisitionedQuantity.ToString()]), out RequisitionedQuantity))
                                                    obj.DispRequisitionedQuantity = RequisitionedQuantity;
                                                else
                                                    obj.DispRequisitionedQuantity = 0;

                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.AverageUsage.ToString()))
                                            {
                                                double AverageUsage;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.AverageUsage.ToString()]), out AverageUsage))
                                                    obj.DispAverageUsage = AverageUsage;
                                                else
                                                    obj.DispAverageUsage = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Turns.ToString()))
                                            {
                                                double Turns;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.Turns.ToString()]), out Turns))
                                                    obj.DispTurns = Turns;
                                                else
                                                    obj.DispTurns = 0;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OnHandQuantity.ToString()))
                                            {
                                                double OnHandQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OnHandQuantity.ToString()]), out OnHandQuantity))
                                                {
                                                    //obj.DispOnHandQuantity = OnHandQuantity;
                                                    obj.OnHandQuantity = OnHandQuantity;
                                                }
                                                else
                                                {
                                                    //obj.DispOnHandQuantity = 0;
                                                    obj.OnHandQuantity = 0;
                                                }
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
                                                Boolean ItemLevelMinMaxQtyRequired = true;
                                                if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsItemLevelMinMaxQtyRequired.ToString()]), out ItemLevelMinMaxQtyRequired))
                                                    obj.IsItemLevelMinMaxQtyRequired = ItemLevelMinMaxQtyRequired;
                                                else
                                                    obj.IsItemLevelMinMaxQtyRequired = true;
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
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF6.ToString()))
                                            {
                                                obj.UDF6 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF6.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF7.ToString()))
                                            {
                                                obj.UDF7 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF7.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF8.ToString()))
                                            {
                                                obj.UDF8 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF8.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF9.ToString()))
                                            {
                                                obj.UDF9 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF9.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF10.ToString()))
                                            {
                                                obj.UDF10 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF10.ToString()]);
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.PerItemCost.ToString()))
                                            {
                                                double PerItemCost;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.PerItemCost.ToString()]), out PerItemCost))
                                                    obj.PerItemCost = PerItemCost;
                                                else
                                                    obj.PerItemCost = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.OutTransferQuantity.ToString()))
                                            {
                                                double OutTransferQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.OutTransferQuantity.ToString()]), out OutTransferQuantity))
                                                    obj.OutTransferQuantity = OutTransferQuantity;
                                                else
                                                    obj.OutTransferQuantity = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsAllowOrderCostuom.ToString()))
                                            {
                                                Boolean IsAllowOrderCostuom = false;
                                                if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsAllowOrderCostuom.ToString()]), out IsAllowOrderCostuom))
                                                    obj.IsAllowOrderCostuom = IsAllowOrderCostuom;
                                                else
                                                    obj.IsAllowOrderCostuom = false;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.eLabelKey.ToString()))
                                            {
                                                obj.eLabelKey = item[CommonUtility.ImportItemColumn.eLabelKey.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.EnrichedProductData.ToString()))
                                            {
                                                obj.EnrichedProductData = item[CommonUtility.ImportItemColumn.EnrichedProductData.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.EnhancedDescription.ToString()))
                                            {
                                                obj.EnhancedDescription = item[CommonUtility.ImportItemColumn.EnhancedDescription.ToString()].ToString();
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
                                List<LocationMasterMain> lstImport = new List<LocationMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    LocationMasterMain obj = new LocationMasterMain();
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
                                                obj.Location = item[CommonUtility.ImportLocationColumn.Location.ToString()].ToString();
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
                                                //obj.CostUOMValue = Convert.ToInt32(item[CommonUtility.ImportCostUOMColumn.CostUOMValue.ToString()]);
                                                if (item[CommonUtility.ImportCostUOMColumn.CostUOMValue.ToString()].ToString() != "")
                                                {
                                                    int costuomValue;
                                                    int.TryParse(item[CommonUtility.ImportCostUOMColumn.CostUOMValue.ToString()].ToString(), out costuomValue);
                                                    obj.CostUOMValue = costuomValue;
                                                }
                                                else
                                                {
                                                    obj.CostUOMValue = 0;
                                                }
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
                                                double _rangeStart = 0;
                                                double.TryParse(item[CommonUtility.ImportInventoryClassificationColumn.RangeStart.ToString()].ToString(), out _rangeStart);
                                                obj.RangeStart = _rangeStart;// Convert.ToDouble(item[CommonUtility.ImportInventoryClassificationColumn.RangeStart.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryClassificationColumn.RangeEnd.ToString()))
                                            {
                                                double _rangeEnd = 0;
                                                double.TryParse(item[CommonUtility.ImportInventoryClassificationColumn.RangeStart.ToString()].ToString(), out _rangeEnd);
                                                obj.RangeEnd = _rangeEnd;// Convert.ToDouble(item[CommonUtility.ImportInventoryClassificationColumn.RangeEnd.ToString()]);
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
                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.SerialNumberTracking.ToString()))
                                            {
                                                Boolean SerialNumberTracking = false;
                                                if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportToolMasterColumn.SerialNumberTracking.ToString()]), out SerialNumberTracking))
                                                    obj.SerialNumberTracking = SerialNumberTracking;
                                                else
                                                    obj.SerialNumberTracking = false;
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.NoOfPastMntsToConsider.ToString()))
                                            {
                                                if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportToolMasterColumn.NoOfPastMntsToConsider.ToString()])))
                                                {
                                                    int _NoOfPastMntsToConsider = 0;
                                                    if (int.TryParse(item[CommonUtility.ImportToolMasterColumn.NoOfPastMntsToConsider.ToString()].ToString(), out _NoOfPastMntsToConsider))
                                                        obj.NoOfPastMntsToConsider = _NoOfPastMntsToConsider;
                                                    else
                                                        obj.NoOfPastMntsToConsider = 0;
                                                }
                                                else
                                                {
                                                    obj.NoOfPastMntsToConsider = Convert.ToInt32(0);
                                                }

                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolMasterColumn.MaintenanceDueNoticeDays.ToString()))
                                            {
                                                if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportToolMasterColumn.MaintenanceDueNoticeDays.ToString()])))
                                                {
                                                    int _MaintenanceDueNoticeDays = 0;
                                                    if (int.TryParse(item[CommonUtility.ImportToolMasterColumn.MaintenanceDueNoticeDays.ToString()].ToString(), out _MaintenanceDueNoticeDays))
                                                        obj.MaintenanceDueNoticeDays = _MaintenanceDueNoticeDays;
                                                    else
                                                        obj.MaintenanceDueNoticeDays = 0;
                                                }
                                                else
                                                {
                                                    obj.MaintenanceDueNoticeDays = Convert.ToInt32(0);
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

                            #region Asset Tool Scheduler
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.AssetToolSchedulerMapping.ToString())
                            {
                                ToolsSchedulerDTO objToolsSchedulerDTO = new ToolsSchedulerDTO();
                                List<AssetToolSchedulerMapping> lstImport = new List<AssetToolSchedulerMapping>();
                                foreach (DataRow item in list)
                                {
                                    AssetToolSchedulerMapping obj = new AssetToolSchedulerMapping();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.AssetToolSchedulerMappingColumn.SchedulerName.ToString()].ToString() != "")
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.AssetToolSchedulerMappingColumn.ID.ToString()].ToString());

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerMappingColumn.ScheduleFor.ToString()))
                                            {
                                                obj.ScheduleForName = item[CommonUtility.AssetToolSchedulerMappingColumn.ScheduleFor.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerMappingColumn.SchedulerName.ToString()))
                                            {
                                                obj.SchedulerName = item[CommonUtility.AssetToolSchedulerMappingColumn.SchedulerName.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerMappingColumn.ToolName.ToString()))
                                            {
                                                obj.ToolName = item[CommonUtility.AssetToolSchedulerMappingColumn.ToolName.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerMappingColumn.Serial.ToString()))
                                            {
                                                obj.Serial = item[CommonUtility.AssetToolSchedulerMappingColumn.Serial.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerMappingColumn.AssetName.ToString()))
                                            {
                                                obj.AssetName = item[CommonUtility.AssetToolSchedulerMappingColumn.AssetName.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerMappingColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.AssetToolSchedulerMappingColumn.UDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerMappingColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.AssetToolSchedulerMappingColumn.UDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerMappingColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.AssetToolSchedulerMappingColumn.UDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerMappingColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.AssetToolSchedulerMappingColumn.UDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerMappingColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.AssetToolSchedulerMappingColumn.UDF5.ToString()].ToString();
                                            }

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
                                                        obj.Reason = string.Format(ResImportMasters.DateShouldBeInFormat, ResAssetMaster.PurchaseDate, SessionHelper.RoomDateFormat);
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
                                                obj.AssetCategory = item[CommonUtility.ImportAssetMasterColumn.AssetCategory.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.ImagePath.ToString()))
                                            {
                                                obj.ImagePath = item[CommonUtility.ImportAssetMasterColumn.ImagePath.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportAssetMasterColumn.AssetImageExternalURL.ToString()))
                                            {
                                                obj.AssetImageExternalURL = Convert.ToString(item[CommonUtility.ImportAssetMasterColumn.AssetImageExternalURL.ToString()]);
                                            }

                                            if (obj.Status == null)
                                                obj.Status = "N/A";
                                            if (obj.Reason == null)
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

                                            )
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
                                            //if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.AvailableItemsInWIP.ToString()))
                                            //{
                                            //    double AvailableItemsInWIP = 0;
                                            //    if (double.TryParse(item[CommonUtility.ImportKitsItemsColumn.AvailableItemsInWIP.ToString()].ToString(), out AvailableItemsInWIP))
                                            //        obj.AvailableItemsInWIP = AvailableItemsInWIP;
                                            //    else
                                            //        obj.AvailableItemsInWIP = 0;
                                            //}
                                            //if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.KitDemand.ToString()))
                                            //{
                                            //    double KitDemand = 0;
                                            //    if (double.TryParse(item[CommonUtility.ImportKitsItemsColumn.KitDemand.ToString()].ToString(), out KitDemand))
                                            //        obj.KitDemand = KitDemand;
                                            //    else
                                            //        obj.KitDemand = 0;
                                            //}
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
                                            //if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.KitCategory.ToString()))
                                            //{
                                            //    obj.KitCategory = item[CommonUtility.ImportKitsItemsColumn.KitCategory.ToString()].ToString();
                                            //}
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
                                            //if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.ItemTypeName.ToString()))
                                            //{
                                            //    obj.ItemTypeName = item[CommonUtility.ImportKitsItemsColumn.ItemTypeName.ToString()].ToString();
                                            //}
                                            if (item.Table.Columns.Contains(CommonUtility.ImportKitsItemsColumn.IsItemLevelMinMaxQtyRequired.ToString()))
                                            {
                                                bool IsItemLevelMinMaxQtyRequired = false;
                                                if (bool.TryParse(item[CommonUtility.ImportKitsItemsColumn.IsItemLevelMinMaxQtyRequired.ToString()].ToString(), out IsItemLevelMinMaxQtyRequired))
                                                    obj.IsItemLevelMinMaxQtyRequired = IsItemLevelMinMaxQtyRequired;
                                                else
                                                    obj.IsItemLevelMinMaxQtyRequired = IsItemLevelMinMaxQtyRequired;

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
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UOM.ToString()))
                                            {
                                                obj.UOM = Convert.ToString(item[CommonUtility.ImportItemColumn.UOM.ToString()]);

                                                if (string.IsNullOrEmpty(obj.UOM))
                                                {
                                                    obj.UOM = vDefaultUOM;
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

                                            //if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsPurchase.ToString()))
                                            //{
                                            //    Boolean IsPurchase = false;
                                            //    if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsPurchase.ToString()]), out IsPurchase))
                                            //        obj.IsPurchase = IsPurchase;
                                            //    else
                                            //        obj.IsPurchase = false;
                                            //}

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsPurchase.ToString()))
                                            {
                                                Boolean IsPurchase = false;
                                                if (string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportItemColumn.IsPurchase.ToString()])))
                                                {
                                                    if (obj.IsTransfer == false)
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
                                                        if (obj.IsTransfer == false)
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

                                            //if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF1.ToString()))
                                            //{
                                            //    obj.UDF1 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF1.ToString()]);
                                            //}

                                            //if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF2.ToString()))
                                            //{
                                            //    obj.UDF2 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF2.ToString()]);
                                            //}

                                            //if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF3.ToString()))
                                            //{
                                            //    obj.UDF3 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF3.ToString()]);
                                            //}

                                            //if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF4.ToString()))
                                            //{
                                            //    obj.UDF4 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF4.ToString()]);
                                            //}

                                            //if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.UDF5.ToString()))
                                            //{
                                            //    obj.UDF5 = Convert.ToString(item[CommonUtility.ImportItemColumn.UDF5.ToString()]);
                                            //}
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
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Link1.ToString()))
                                            {
                                                obj.Link1 = Convert.ToString(item[CommonUtility.ImportItemColumn.Link1.ToString()]);
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.Link2.ToString()))
                                            {
                                                obj.Link2 = Convert.ToString(item[CommonUtility.ImportItemColumn.Link2.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.InventoryClassification.ToString()))
                                            {
                                                string InventoryClassification;
                                                InventoryClassification = Convert.ToString(item[CommonUtility.ImportItemColumn.InventoryClassification.ToString()]);
                                                obj.InventoryClassificationName = InventoryClassification;
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

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsItemLevelMinMaxQtyRequired.ToString()))
                                            {
                                                Boolean ItemLevelMinMaxQtyRequired = false;
                                                if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsItemLevelMinMaxQtyRequired.ToString()]), out ItemLevelMinMaxQtyRequired))
                                                    obj.IsItemLevelMinMaxQtyRequired = ItemLevelMinMaxQtyRequired;
                                                else
                                                    obj.IsItemLevelMinMaxQtyRequired = false;
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
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemLink2ExternalURL.ToString()))
                                            {
                                                obj.ItemLink2ExternalURL = Convert.ToString(item[CommonUtility.ImportItemColumn.ItemLink2ExternalURL.ToString()]);
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemImageExternalURL.ToString()))
                                            {
                                                obj.ItemImageExternalURL = Convert.ToString(item[CommonUtility.ImportItemColumn.ItemImageExternalURL.ToString()]);
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ItemDocExternalURL.ToString()))
                                            {
                                                obj.ItemDocExternalURL = Convert.ToString(item[CommonUtility.ImportItemColumn.ItemDocExternalURL.ToString()]);
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsActive.ToString()))
                                            {
                                                Boolean IsActive = false;
                                                if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportItemColumn.IsActive.ToString()]), out IsActive))
                                                    obj.IsActive = IsActive;
                                                else
                                                    obj.IsActive = false;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.ImagePath.ToString()))
                                            {
                                                obj.ImagePath = item[CommonUtility.ImportItemColumn.ImagePath.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.LongDescription.ToString()))
                                            {
                                                obj.LongDescription = item[CommonUtility.ImportItemColumn.LongDescription.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.EnrichedProductData.ToString()))
                                            {
                                                obj.EnrichedProductData = item[CommonUtility.ImportItemColumn.EnrichedProductData.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.EnhancedDescription.ToString()))
                                            {
                                                obj.EnhancedDescription = item[CommonUtility.ImportItemColumn.EnhancedDescription.ToString()].ToString();
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

                            #region Item Manufacturer
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ItemManufacturerDetails.ToString())
                            {
                                List<ItemManufacturer> lstImport = new List<ItemManufacturer>();
                                int count = 1;
                                foreach (DataRow item in list)
                                {
                                    ItemManufacturer obj = new ItemManufacturer();
                                    try
                                    {
                                        // bool AllowInsert = true;
                                        if (item[CommonUtility.ImportItemManufacturer.ManufacturerName.ToString()].ToString() != "")
                                        //  AllowInsert = lstImport.Where(x => x.Location == item[CommonUtility.ImportLocationColumn.Location.ToString()].ToString()).ToList().Count > 0 ? false : true;
                                        // if (AllowInsert == true)
                                        {
                                            // obj.ID = Convert.ToInt32(item[CommonUtility.ImportLocationColumn.ID.ToString()].ToString());
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
                                            obj.ID = count;
                                            count++;
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
                            #region Item Supplier
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ItemSupplierDetails.ToString())
                            {
                                List<ItemSupplier> lstImport = new List<ItemSupplier>();
                                int count = 1;
                                foreach (DataRow item in list)
                                {
                                    ItemSupplier obj = new ItemSupplier();
                                    try
                                    {
                                        // bool AllowInsert = true;
                                        if (item[CommonUtility.ImportItemSupplier.SupplierName.ToString()].ToString() != "")
                                        //  AllowInsert = lstImport.Where(x => x.Location == item[CommonUtility.ImportLocationColumn.Location.ToString()].ToString()).ToList().Count > 0 ? false : true;
                                        // if (AllowInsert == true)
                                        {
                                            // obj.ID = Convert.ToInt32(item[CommonUtility.ImportLocationColumn.ID.ToString()].ToString());
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
                                            obj.ID = count;
                                            count++;
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

                            #region BarCode Master
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.BarcodeMaster.ToString())
                            {
                                List<ImportBarcodeMaster> lstImport = new List<ImportBarcodeMaster>();
                                int count = 1;
                                foreach (DataRow item in list)
                                {
                                    ImportBarcodeMaster obj = new ImportBarcodeMaster();
                                    try
                                    {
                                        // bool AllowInsert = true;
                                        if (item[CommonUtility.ImportBarcode.ItemNumber.ToString()].ToString() != "" && item[CommonUtility.ImportBarcode.ModuleName.ToString()].ToString() != "" && item[CommonUtility.ImportBarcode.BarcodeString.ToString()].ToString() != "")
                                        {
                                            // obj.ID = Convert.ToInt32(item[CommonUtility.ImportLocationColumn.ID.ToString()].ToString());
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


                                            obj.ID = count;
                                            count++;
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

                            #region [UDF]
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.UDF.ToString())
                            {
                                List<UDFMasterMain> lstImport = new List<UDFMasterMain>();
                                Dictionary<string, int> modulewiseUDF = new Dictionary<string, int>();
                                UDFDAL objUDFDAL = new UDFDAL(SessionHelper.EnterPriseDBName);
                                int count = 1;
                                foreach (DataRow item in list)
                                {
                                    UDFMasterMain obj = new UDFMasterMain();
                                    try
                                    {
                                        // bool AllowInsert = true;
                                        if (item[CommonUtility.ImportUDF.ModuleName.ToString()].ToString() != "" && item[CommonUtility.ImportUDF.ControlType.ToString()].ToString() != "" && item[CommonUtility.ImportUDF.UDFName.ToString()].ToString() != "")
                                        {
                                            if (item.Table.Columns.Contains(CommonUtility.ImportUDF.ModuleName.ToString()))
                                            {
                                                obj.ModuleName = item[CommonUtility.ImportUDF.ModuleName.ToString()].ToString();
                                                if (!modulewiseUDF.ContainsKey(obj.ModuleName))
                                                {
                                                    UDFModule objUDFModule = objUDFDAL.GetUDFModule(obj.ModuleName);
                                                    if (objUDFModule != null)
                                                    {
                                                        if (objUDFModule.NoOfUdfs > 5)
                                                        {
                                                            modulewiseUDF.Add(obj.ModuleName, objUDFModule.NoOfUdfs);
                                                        }
                                                    }
                                                }
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUDF.UDFColumnName.ToString()))
                                            {
                                                obj.UDFColumnName = item[CommonUtility.ImportUDF.UDFColumnName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportUDF.UDFName.ToString()))
                                            {
                                                obj.UDFName = item[CommonUtility.ImportUDF.UDFName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportUDF.ControlType.ToString()))
                                            {
                                                obj.ControlType = item[CommonUtility.ImportUDF.ControlType.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportUDF.DefaultValue.ToString()))
                                            {
                                                obj.DefaultValue = item[CommonUtility.ImportUDF.DefaultValue.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportUDF.OptionName.ToString()))
                                            {
                                                obj.OptionName = item[CommonUtility.ImportUDF.OptionName.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportUDF.IsRequired.ToString()))
                                            {
                                                bool Tempbool = false;
                                                bool.TryParse(item[CommonUtility.ImportUDF.IsRequired.ToString()].ToString(), out Tempbool);
                                                obj.IsRequired = Tempbool;

                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportUDF.IsDeleted.ToString()))
                                            {
                                                bool Tempbool = false;
                                                bool.TryParse(item[CommonUtility.ImportUDF.IsDeleted.ToString()].ToString(), out Tempbool);
                                                obj.IsDeleted = Tempbool;

                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportUDF.IncludeInNarrowSearch.ToString()))
                                            {
                                                bool Tempbool = false;
                                                bool.TryParse(item[CommonUtility.ImportUDF.IncludeInNarrowSearch.ToString()].ToString(), out Tempbool);
                                                obj.IncludeInNarrowSearch = Tempbool;

                                            }
                                            obj.ID = count;
                                            count++;
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";


                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }
                                count = 0;
                                string[] arrUdfs = new string[] { "udf1", "udf2", "udf3", "udf4", "udf5" };
                                string[] arrextraUdf = new string[] { "udf6", "udf7", "udf8", "udf9", "udf10" };
                                string[] arrControlTypes = new string[] { "textbox", "dropdown", "dropdown editable" };
                                if (lstImport != null)
                                {
                                    lstImport.ForEach(t =>
                                    {
                                        if (!arrUdfs.Contains((t.UDFColumnName ?? string.Empty).ToLower().Trim()))
                                        {
                                            if (!modulewiseUDF.ContainsKey(t.ModuleName))
                                            {
                                                t.Status = "fail";
                                                t.Reason = t.Reason + ResUDFSetup.InvalidUDFColumnName;
                                            }
                                            else if ((!arrextraUdf.Contains((t.UDFColumnName ?? string.Empty).ToLower().Trim())))
                                            {
                                                t.Status = "fail";
                                                t.Reason = t.Reason + ResUDFSetup.InvalidUDFColumnName;
                                            }
                                        }
                                        if (!arrControlTypes.Contains((t.ControlType ?? string.Empty).ToLower().Trim()))
                                        {
                                            t.Status = "fail";
                                            t.Reason = t.Reason + string.Format(ResCommon.MsgInvalid, ResUDFSetup.ControlType);
                                        }
                                        if (!eTurnsWeb.Models.UDFDictionaryTables.IsVaidImportUDFTable((t.ModuleName ?? string.Empty).ToLower()))
                                        {
                                            t.Status = "fail";
                                            t.Reason = t.Reason + ResUDFSetup.InvalidUDFTableName;
                                        }
                                    });
                                }
                                else
                                {
                                    lstImport = new List<UDFMasterMain>();
                                }

                                lstImport = (from itm in lstImport.Where(t => t.Status != "fail")
                                             group itm by new { itm.ModuleName, itm.UDFName, itm.UDFColumnName, optname = (itm.OptionName ?? string.Empty) } into groupedview
                                             select new UDFMasterMain
                                             {
                                                 ID = count++,
                                                 ControlType = groupedview.Select(t => t.ControlType).Any() ? groupedview.Select(t => t.ControlType).First() : string.Empty,
                                                 DefaultValue = groupedview.Where(t => t.DefaultValue != "").Select(t => t.DefaultValue).Any() ? groupedview.Where(t => t.DefaultValue != "").Select(t => t.DefaultValue).First() : string.Empty,
                                                 IncludeInNarrowSearch = groupedview.Where(t => t.IncludeInNarrowSearch != null).Any() ? (groupedview.Where(t => t.IncludeInNarrowSearch != null).First().IncludeInNarrowSearch ?? false) : false,
                                                 IsDeleted = groupedview.Where(t => t.IsDeleted != null).Any() ? (groupedview.Where(t => t.IsDeleted != null).First().IsDeleted ?? false) : false,
                                                 IsRequired = groupedview.Where(t => t.IsRequired != null).Any() ? (groupedview.Where(t => t.IsRequired != null).First().IsRequired ?? false) : false,
                                                 ModuleName = groupedview.Key.ModuleName,
                                                 OptionName = groupedview.Key.optname,
                                                 UDFColumnName = groupedview.Key.UDFColumnName,
                                                 UDFName = groupedview.Key.UDFName
                                             }).ToList();
                                Session["importedData"] = lstImport;
                            }
                            #endregion

                            #region [Project Master]
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ProjectMaster.ToString())
                            {
                                List<ProjectMasterMain> lstImport = new List<ProjectMasterMain>();
                                int count = 1;
                                foreach (DataRow item in list)
                                {
                                    ProjectMasterMain obj = new ProjectMasterMain();
                                    try
                                    {
                                        // bool AllowInsert = true;
                                        if (item[CommonUtility.ImportProjectMaster.ProjectSpendName.ToString()].ToString() != "" && item[CommonUtility.ImportProjectMaster.DollarLimitAmount.ToString()].ToString() != "" && item[CommonUtility.ImportProjectMaster.ItemNumber.ToString()].ToString() != "")
                                        {
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

                                            obj.ID = count;
                                            count++;
                                            obj.Status = "N/A";
                                            obj.Reason = "N/A";


                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }
                                count = 0;

                                Session["importedData"] = lstImport;
                            }
                            #endregion


                            #region item Quantity Import and Cost
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ItemLocationQty.ToString())
                            {
                                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                                List<InventoryLocationMain> lstImport = new List<InventoryLocationMain>();
                                foreach (DataRow item in list)
                                {
                                    InventoryLocationMain obj = new InventoryLocationMain();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
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
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.Cost.ToString()))
                                            {
                                                if (item[CommonUtility.ImportInventoryLocationColumn.Cost.ToString()] == null || Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.Cost.ToString()]) == "")
                                                {
                                                    obj.Cost = 0;
                                                }
                                                else
                                                {
                                                    double _costValue = 0.0;
                                                    if (double.TryParse(item[CommonUtility.ImportInventoryLocationColumn.Cost.ToString()].ToString(), out _costValue))
                                                        obj.Cost = _costValue;
                                                    else
                                                        obj.Cost = 0;
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
                                                        obj.Reason = string.Format(ResImportMasters.DateShouldBeInFormat, ResItemLocationDetails.ExpirationDate, SessionHelper.RoomDateFormat);
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
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ReceiveDate.ToString()))
                                            {
                                                if (item[CommonUtility.ImportInventoryLocationColumn.ReceiveDate.ToString()].ToString() != "")
                                                {
                                                    DateTime dt;
                                                    DateTime.TryParseExact(item[CommonUtility.ImportInventoryLocationColumn.ReceiveDate.ToString()].ToString().Split(' ')[0], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);
                                                    if (dt != DateTime.MinValue)
                                                    {
                                                        obj.Received = dt.ToString(SessionHelper.RoomDateFormat);
                                                    }
                                                    else
                                                    {
                                                        obj.Received = null;
                                                        obj.Reason = string.Format(ResImportMasters.DateShouldBeInFormat, ResItemLocationDetails.ReceivedDate, SessionHelper.RoomDateFormat);
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
                                                objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByItemNumberPlain(HttpUtility.HtmlDecode(obj.ItemNumber), SessionHelper.RoomID, SessionHelper.CompanyID);
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
                                                //if (obj.customerownedquantity != 1 && !objItemMasterDTO.Consignment)
                                                //{
                                                //    obj.customerownedquantity = 1;
                                                //}
                                                //if (obj.consignedquantity != 1 && objItemMasterDTO.Consignment)
                                                //{
                                                //    obj.consignedquantity = 1;
                                                //}
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
                                                //if (obj.customerownedquantity != 1 && !objItemMasterDTO.Consignment)
                                                //{
                                                //    obj.customerownedquantity = 1;
                                                //}
                                                //if (obj.consignedquantity != 1 && objItemMasterDTO.Consignment)
                                                //{
                                                //    obj.consignedquantity = 1;
                                                //}
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
                                            //InventoryLocationMain oCheckExist = lstImport.Where(x => x.ItemNumber == obj.ItemNumber && x.BinNumber == obj.BinNumber && x.SerialNumber == obj.SerialNumber && x.LotNumber == obj.LotNumber).FirstOrDefault();

                                            //if (oCheckExist != null)
                                            //{
                                            //    lstImport.Remove(oCheckExist);
                                            //}


                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }
                                lstImport = (from ilq in lstImport
                                             group ilq by new { ilq.BinNumber, ilq.Cost, ilq.Expiration, ilq.ItemNumber, ilq.LotNumber, ilq.Received, ilq.SerialNumber, ilq.ItemGUID } into groupedilq
                                             select new InventoryLocationMain
                                             {
                                                 BinNumber = groupedilq.Key.BinNumber,
                                                 CompanyID = SessionHelper.CompanyID,
                                                 consignedquantity = groupedilq.Sum(t => (t.consignedquantity ?? 0)),
                                                 Cost = groupedilq.Key.Cost,
                                                 Created = DateTime.UtcNow,
                                                 CreatedBy = SessionHelper.UserID,
                                                 customerownedquantity = groupedilq.Sum(t => (t.customerownedquantity ?? 0)),
                                                 Expiration = groupedilq.Key.Expiration,
                                                 GUID = Guid.NewGuid(),
                                                 InsertedFrom = "import",
                                                 IsArchived = false,
                                                 IsDeleted = false,
                                                 ItemGUID = groupedilq.Key.ItemGUID,
                                                 ItemNumber = groupedilq.Key.ItemNumber,
                                                 LastUpdatedBy = SessionHelper.UserID,
                                                 LotNumber = groupedilq.Key.LotNumber,
                                                 Received = groupedilq.Key.Received,
                                                 Room = SessionHelper.RoomID,
                                                 SerialNumber = groupedilq.Key.SerialNumber,
                                                 Updated = DateTime.UtcNow,
                                                 Status = string.Join(",", groupedilq.Select(t => t.Status).ToArray()),
                                                 Reason = string.Join(",", groupedilq.Select(t => t.Reason).ToArray()),

                                             }).ToList();
                                Session["importedData"] = lstImport;
                            }
                            #endregion

                            #region Manual Count master
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ManualCount.ToString())
                            {
                                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                                List<InventoryLocationMain> lstImport = new List<InventoryLocationMain>();
                                foreach (DataRow item in list)
                                {
                                    InventoryLocationMain obj = new InventoryLocationMain();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
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
                                            //if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.Cost.ToString()))
                                            //{
                                            //    if (item[CommonUtility.ImportInventoryLocationColumn.Cost.ToString()] == null || Convert.ToString(item[CommonUtility.ImportInventoryLocationColumn.Cost.ToString()]) == "")
                                            //    {
                                            //        obj.Cost = 0;
                                            //    }
                                            //    else
                                            //    {
                                            //        double _costValue = 0.0;
                                            //        if (double.TryParse(item[CommonUtility.ImportInventoryLocationColumn.Cost.ToString()].ToString(), out _costValue))
                                            //            obj.Cost = _costValue;
                                            //        else
                                            //            obj.Cost = 0;
                                            //    }

                                            //}

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
                                                    string _expirationDt = item[CommonUtility.ImportInventoryLocationColumn.ExpirationDate.ToString()].ToString().Split(' ')[0];
                                                    DateTime.TryParseExact(_expirationDt, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);
                                                    if (dt != DateTime.MinValue)
                                                    {
                                                        obj.Expiration = dt.ToString(SessionHelper.RoomDateFormat);
                                                        obj.displayExpiration = dt.ToString(SessionHelper.RoomDateFormat);
                                                    }
                                                    else
                                                    {
                                                        obj.Expiration = null;
                                                        obj.displayExpiration = null;
                                                        obj.Reason = string.Format(ResImportMasters.DateShouldBeInFormat, ResItemLocationDetails.ExpirationDate, SessionHelper.RoomDateFormat);
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
                                                        obj.Reason = string.Format(ResImportMasters.DateShouldBeInFormat, ResItemLocationDetails.ReceivedDate, SessionHelper.RoomDateFormat);
                                                        obj.Status = "Fail";
                                                    }
                                                }
                                                else
                                                {
                                                    obj.Received = null;
                                                }
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.ItemDescription.ToString()))
                                            {
                                                obj.ItemDescription = item[CommonUtility.ImportInventoryLocationColumn.ItemDescription.ToString()].ToString();
                                            }
                                            if (objItemMasterDTO == null || obj.ItemNumber != objItemMasterDTO.ItemNumber)
                                            {
                                                objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByItemNumberPlain(HttpUtility.HtmlDecode(obj.ItemNumber), SessionHelper.RoomID, SessionHelper.CompanyID);
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
                                                //if (obj.customerownedquantity != 1 && !objItemMasterDTO.Consignment)
                                                //{
                                                //    obj.customerownedquantity = 1;
                                                //}
                                                //if (obj.consignedquantity != 1 && objItemMasterDTO.Consignment)
                                                //{
                                                //    obj.consignedquantity = 1;
                                                //}
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
                                                //if (obj.customerownedquantity != 1 && !objItemMasterDTO.Consignment)
                                                //{
                                                //    obj.customerownedquantity = 1;
                                                //}
                                                //if (obj.consignedquantity != 1 && objItemMasterDTO.Consignment)
                                                //{
                                                //    obj.consignedquantity = 1;
                                                //}
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
                                            //InventoryLocationMain oCheckExist = lstImport.Where(x => x.ItemNumber == obj.ItemNumber && x.BinNumber == obj.BinNumber && x.SerialNumber == obj.SerialNumber && x.LotNumber == obj.LotNumber).FirstOrDefault();

                                            //if (oCheckExist != null)
                                            //{
                                            //    lstImport.Remove(oCheckExist);
                                            //}


                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportInventoryLocationColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportInventoryLocationColumn.UDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportInventoryLocationColumn.UDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportInventoryLocationColumn.UDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportInventoryLocationColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportInventoryLocationColumn.UDF5.ToString()].ToString();
                                            }

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }
                                lstImport = (from ilq in lstImport
                                             group ilq by new
                                             {
                                                 ilq.BinNumber,
                                                 ilq.Expiration,
                                                 ilq.ItemNumber,
                                                 ilq.LotNumber,
                                                 ilq.SerialNumber,
                                                 ilq.ItemGUID,
                                                 ilq.ItemDescription,
                                                 ilq.UDF1,
                                                 ilq.UDF2,
                                                 ilq.UDF3,
                                                 ilq.UDF4,
                                                 ilq.UDF5
                                             } into groupedilq
                                             select new InventoryLocationMain
                                             {
                                                 BinNumber = groupedilq.Key.BinNumber,
                                                 CompanyID = SessionHelper.CompanyID,
                                                 consignedquantity = groupedilq.Sum(t => (t.consignedquantity ?? 0)),
                                                 //Cost = groupedilq.Key.Cost,
                                                 Created = DateTime.UtcNow,
                                                 CreatedBy = SessionHelper.UserID,
                                                 customerownedquantity = groupedilq.Sum(t => (t.customerownedquantity ?? 0)),
                                                 Expiration = groupedilq.Key.Expiration,
                                                 GUID = Guid.NewGuid(),
                                                 InsertedFrom = "import",
                                                 IsArchived = false,
                                                 IsDeleted = false,
                                                 ItemGUID = groupedilq.Key.ItemGUID,
                                                 ItemNumber = groupedilq.Key.ItemNumber,
                                                 LastUpdatedBy = SessionHelper.UserID,
                                                 LotNumber = groupedilq.Key.LotNumber,
                                                 ItemDescription = groupedilq.Key.ItemDescription,
                                                 //  Received = groupedilq.Key.Received,
                                                 Room = SessionHelper.RoomID,
                                                 SerialNumber = groupedilq.Key.SerialNumber,
                                                 Updated = DateTime.UtcNow,
                                                 Status = string.Join(",", groupedilq.Select(t => t.Status).ToArray()),
                                                 Reason = string.Join(",", groupedilq.Select(t => t.Reason).ToArray()),
                                                 UDF1 = groupedilq.Key.UDF1,
                                                 UDF2 = groupedilq.Key.UDF2,
                                                 UDF3 = groupedilq.Key.UDF3,
                                                 UDF4 = groupedilq.Key.UDF4,
                                                 UDF5 = groupedilq.Key.UDF5
                                             }).ToList();
                                Session["importedData"] = lstImport;
                            }
                            #endregion

                            #region Workorder
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.WorkOrder.ToString())
                            {
                                WorkOrderDTO objWorkOrderDTO = new WorkOrderDTO();
                                List<WorkOrderMain> lstImport = new List<WorkOrderMain>();
                                foreach (DataRow item in list)
                                {
                                    WorkOrderMain obj = new WorkOrderMain();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.WorkOrderColumn.WOName.ToString()].ToString() != "")
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryClassificationColumn.ID.ToString()].ToString());
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
                                            if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.Asset.ToString()))
                                            {
                                                obj.Asset = item[CommonUtility.WorkOrderColumn.Asset.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.Odometer.ToString()))
                                            {
                                                double TempOdometer_OperationHours = 0;
                                                double.TryParse(item[CommonUtility.WorkOrderColumn.Odometer.ToString()].ToString(), out TempOdometer_OperationHours);
                                                obj.Odometer_OperationHours = TempOdometer_OperationHours;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.WorkOrderColumn.SupplierAccount.ToString()))
                                            {
                                                obj.SupplierAccount = item[CommonUtility.WorkOrderColumn.SupplierAccount.ToString()].ToString();
                                            }
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion

                            #region Pull Import
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.PullMaster.ToString())
                            {
                                List<PullImport> lstImport = new List<PullImport>();
                                foreach (DataRow item in list)
                                {
                                    PullImport obj = new PullImport();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.PullImportColumn.ItemNumber.ToString()].ToString() != "")
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryClassificationColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.ItemNumber.ToString()))
                                            {
                                                obj.ItemNumber = item[CommonUtility.PullImportColumn.ItemNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.PullQuantity.ToString()))
                                            {
                                                double TempPullQty = 0;
                                                double.TryParse(item[CommonUtility.PullImportColumn.PullQuantity.ToString()].ToString(), out TempPullQty);
                                                obj.PullQuantity = TempPullQty.ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.Location.ToString()))
                                            {
                                                obj.Location = item[CommonUtility.PullImportColumn.Location.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.PullImportColumn.UDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.PullImportColumn.UDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.PullImportColumn.UDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.PullImportColumn.UDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.PullImportColumn.UDF5.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.ProjectSpendName.ToString()))
                                            {
                                                obj.ProjectSpendName = item[CommonUtility.PullImportColumn.ProjectSpendName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.PullOrderNumber.ToString()))
                                            {
                                                obj.PullOrderNumber = item[CommonUtility.PullImportColumn.PullOrderNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.WorkOrder.ToString()))
                                            {
                                                obj.WorkOrder = item[CommonUtility.PullImportColumn.WorkOrder.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.Asset.ToString()))
                                            {
                                                obj.Asset = item[CommonUtility.PullImportColumn.Asset.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.ActionType.ToString()))
                                            {
                                                obj.ActionType = item[CommonUtility.PullImportColumn.ActionType.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.ItemSellPrice.ToString()))
                                            {
                                                if (!string.IsNullOrWhiteSpace(Convert.ToString(item[CommonUtility.PullImportColumn.ItemSellPrice.ToString()])))
                                                {
                                                    double TempItemSellPrice = 0;
                                                    double.TryParse(item[CommonUtility.PullImportColumn.ItemSellPrice.ToString()].ToString(), out TempItemSellPrice);
                                                    obj.ItemSellPrice = TempItemSellPrice.ToString();
                                                }
                                            }
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion

                            #region PullImportWithLotSerial
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.PullImportWithLotSerial.ToString())
                            {
                                List<PullImportWithLotSerial> lstImport = new List<PullImportWithLotSerial>();
                                foreach (DataRow item in list)
                                {
                                    PullImportWithLotSerial obj = new PullImportWithLotSerial();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.PullImportWithLotSerialColumn.ItemNumber.ToString()].ToString() != "")
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryClassificationColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.ItemNumber.ToString()))
                                            {
                                                obj.ItemNumber = item[CommonUtility.PullImportWithLotSerialColumn.ItemNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.PullQuantity.ToString()))
                                            {
                                                double TempPullQty = 0;
                                                double.TryParse(item[CommonUtility.PullImportWithLotSerialColumn.PullQuantity.ToString()].ToString(), out TempPullQty);
                                                obj.PullQuantity = TempPullQty.ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.Location.ToString()))
                                            {
                                                obj.Location = item[CommonUtility.PullImportWithLotSerialColumn.Location.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.PullImportWithLotSerialColumn.UDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.PullImportColumn.UDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.PullImportWithLotSerialColumn.UDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.PullImportWithLotSerialColumn.UDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.PullImportWithLotSerialColumn.UDF5.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.ProjectSpendName.ToString()))
                                            {
                                                obj.ProjectSpendName = item[CommonUtility.PullImportWithLotSerialColumn.ProjectSpendName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportColumn.PullOrderNumber.ToString()))
                                            {
                                                obj.PullOrderNumber = item[CommonUtility.PullImportWithLotSerialColumn.PullOrderNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.WorkOrder.ToString()))
                                            {
                                                obj.WorkOrder = item[CommonUtility.PullImportWithLotSerialColumn.WorkOrder.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.Asset.ToString()))
                                            {
                                                obj.Asset = item[CommonUtility.PullImportWithLotSerialColumn.Asset.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.ActionType.ToString()))
                                            {
                                                obj.ActionType = item[CommonUtility.PullImportWithLotSerialColumn.ActionType.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.SerialNumber.ToString()))
                                            {
                                                obj.SerialNumber = item[CommonUtility.PullImportWithLotSerialColumn.SerialNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.LotNumber.ToString()))
                                            {
                                                obj.LotNumber = item[CommonUtility.PullImportWithLotSerialColumn.LotNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.ExpirationDate.ToString()))
                                            {
                                                obj.ExpirationDate = item[CommonUtility.PullImportWithLotSerialColumn.ExpirationDate.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithLotSerialColumn.ItemSellPrice.ToString()))
                                            {
                                                if (!string.IsNullOrWhiteSpace(Convert.ToString(item[CommonUtility.PullImportWithLotSerialColumn.ItemSellPrice.ToString()])))
                                                {
                                                    double TempItemSellPrice = 0;
                                                    double.TryParse(item[CommonUtility.PullImportWithLotSerialColumn.ItemSellPrice.ToString()].ToString(), out TempItemSellPrice);
                                                    obj.ItemSellPrice = TempItemSellPrice.ToString();
                                                }
                                            }
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion

                            #region Pull Import With Same Qty
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.PullMasterWithSameQty.ToString())
                            {
                                List<PullImportWithSameQty> lstImport = new List<PullImportWithSameQty>();
                                foreach (DataRow item in list)
                                {
                                    PullImportWithSameQty obj = new PullImportWithSameQty();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.PullImportWithSameQtyColumn.ItemNumber.ToString()].ToString() != "")
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryClassificationColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.ItemNumber.ToString()))
                                            {
                                                obj.ItemNumber = item[CommonUtility.PullImportWithSameQtyColumn.ItemNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.PullQuantity.ToString()))
                                            {
                                                double PullQuantity;
                                                if (double.TryParse(Convert.ToString(item[CommonUtility.PullImportWithSameQtyColumn.PullQuantity.ToString()]), out PullQuantity))
                                                    obj.PullQuantity = PullQuantity;
                                                else
                                                    obj.PullQuantity = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.BinNumber.ToString()))
                                            {
                                                obj.BinNumber = item[CommonUtility.PullImportWithSameQtyColumn.BinNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.PullImportWithSameQtyColumn.UDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.PullImportWithSameQtyColumn.UDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.PullImportWithSameQtyColumn.UDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.PullImportWithSameQtyColumn.UDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.PullImportWithSameQtyColumn.UDF5.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.ProjectSpendName.ToString()))
                                            {
                                                obj.ProjectSpendName = item[CommonUtility.PullImportWithSameQtyColumn.ProjectSpendName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.PullOrderNumber.ToString()))
                                            {
                                                obj.PullOrderNumber = item[CommonUtility.PullImportWithSameQtyColumn.PullOrderNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.WorkOrder.ToString()))
                                            {
                                                obj.WorkOrder = item[CommonUtility.PullImportWithSameQtyColumn.WorkOrder.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.Asset.ToString()))
                                            {
                                                obj.Asset = item[CommonUtility.PullImportWithSameQtyColumn.Asset.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.ActionType.ToString()))
                                            {
                                                obj.ActionType = item[CommonUtility.PullImportWithSameQtyColumn.ActionType.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.Created.ToString()))
                                            {
                                                obj.Created = item[CommonUtility.PullImportWithSameQtyColumn.Created.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.ItemCost.ToString()))
                                            {
                                                obj.ItemCost = item[CommonUtility.PullImportWithSameQtyColumn.ItemCost.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PullImportWithSameQtyColumn.CostUOMValue.ToString()))
                                            {
                                                obj.CostUOMValue = item[CommonUtility.PullImportWithSameQtyColumn.CostUOMValue.ToString()].ToString();
                                            }
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion

                            #region Item Location Import
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ItemLocationChange.ToString())
                            {
                                List<ItemLocationChangeImport> lstImport = new List<ItemLocationChangeImport>();
                                foreach (DataRow item in list)
                                {
                                    ItemLocationChangeImport obj = new ItemLocationChangeImport();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ItemLocationChangeImportColumn.ItemNumber.ToString()].ToString() != "" && item[CommonUtility.ItemLocationChangeImportColumn.OldLocationName.ToString()].ToString() != "" && item[CommonUtility.ItemLocationChangeImportColumn.NewLocationName.ToString()].ToString() != "")
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryClassificationColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.ItemLocationChangeImportColumn.ItemNumber.ToString()))
                                            {
                                                obj.ItemNumber = item[CommonUtility.ItemLocationChangeImportColumn.ItemNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ItemLocationChangeImportColumn.OldLocationName.ToString()))
                                            {
                                                obj.OldLocationName = item[CommonUtility.ItemLocationChangeImportColumn.OldLocationName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ItemLocationChangeImportColumn.NewLocationName.ToString()))
                                            {
                                                obj.NewLocationName = item[CommonUtility.ItemLocationChangeImportColumn.NewLocationName.ToString()].ToString();
                                            }

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion

                            #region Asset Tool Scheduler
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.AssetToolScheduler.ToString())
                            {
                                ToolsSchedulerDTO objToolsSchedulerDTO = new ToolsSchedulerDTO();
                                List<AssetToolScheduler> lstImport = new List<AssetToolScheduler>();
                                foreach (DataRow item in list)
                                {
                                    AssetToolScheduler obj = new AssetToolScheduler();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.AssetToolSchedulerColumn.SchedulerName.ToString()].ToString() != "")
                                        {
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.AssetToolSchedulerColumn.ID.ToString()].ToString());

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.ScheduleFor.ToString()))
                                            {
                                                obj.ScheduleForName = item[CommonUtility.AssetToolSchedulerColumn.ScheduleFor.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.SchedulerName.ToString()))
                                            {
                                                obj.SchedulerName = item[CommonUtility.AssetToolSchedulerColumn.SchedulerName.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.Description.ToString()))
                                            {
                                                obj.Description = item[CommonUtility.AssetToolSchedulerColumn.Description.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.SchedulerType.ToString()))
                                            {
                                                obj.SchedulerTypeName = item[CommonUtility.AssetToolSchedulerColumn.SchedulerType.ToString()].ToString();
                                            }



                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.TimeBasedUnit.ToString()))
                                            {
                                                obj.TimeBasedUnitName = item[CommonUtility.AssetToolSchedulerColumn.TimeBasedUnit.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.TimeBasedFrequency.ToString()))
                                            {
                                                if (item[CommonUtility.AssetToolSchedulerColumn.TimeBasedFrequency.ToString()] == null ||
                                                    Convert.ToString(item[CommonUtility.AssetToolSchedulerColumn.TimeBasedFrequency.ToString()]) == "")
                                                {
                                                    obj.TimeBasedFrequency = 0;
                                                }
                                                else
                                                {
                                                    int _TimeBasedFrequency = 0;
                                                    if (int.TryParse(item[CommonUtility.AssetToolSchedulerColumn.TimeBasedFrequency.ToString()].ToString(), out _TimeBasedFrequency))
                                                        obj.TimeBasedFrequency = _TimeBasedFrequency;
                                                    else
                                                        obj.TimeBasedFrequency = 0;
                                                }

                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.OperationalHours.ToString()))
                                            {
                                                if (item[CommonUtility.AssetToolSchedulerColumn.OperationalHours.ToString()] == null ||
                                                    Convert.ToString(item[CommonUtility.AssetToolSchedulerColumn.OperationalHours.ToString()]) == "")
                                                {
                                                    obj.OperationalHours = null;
                                                }
                                                else
                                                {
                                                    double _OperationalHours = 0.0;
                                                    if (double.TryParse(item[CommonUtility.AssetToolSchedulerColumn.OperationalHours.ToString()].ToString(), out _OperationalHours))
                                                        obj.OperationalHours = _OperationalHours;
                                                    else
                                                        obj.OperationalHours = 0;
                                                }

                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.CheckOuts.ToString()))
                                            {
                                                if (item[CommonUtility.AssetToolSchedulerColumn.CheckOuts.ToString()] == null ||
                                                    Convert.ToString(item[CommonUtility.AssetToolSchedulerColumn.CheckOuts.ToString()]) == "")
                                                {
                                                    obj.CheckOuts = null;
                                                }
                                                else
                                                {
                                                    int _CheckOuts = 0;
                                                    if (int.TryParse(item[CommonUtility.AssetToolSchedulerColumn.CheckOuts.ToString()].ToString(), out _CheckOuts))
                                                        obj.CheckOuts = _CheckOuts;
                                                    else
                                                        obj.CheckOuts = 0;
                                                }

                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.Mileage.ToString()))
                                            {
                                                if (item[CommonUtility.AssetToolSchedulerColumn.Mileage.ToString()] == null ||
                                                    Convert.ToString(item[CommonUtility.AssetToolSchedulerColumn.Mileage.ToString()]) == "")
                                                {
                                                    obj.Mileage = null;
                                                }
                                                else
                                                {
                                                    double _Mileage = 0.0;
                                                    if (double.TryParse(item[CommonUtility.AssetToolSchedulerColumn.Mileage.ToString()].ToString(), out _Mileage))
                                                        obj.Mileage = _Mileage;
                                                    else
                                                        obj.Mileage = 0;
                                                }

                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.Quantity.ToString()))
                                            {
                                                if (item[CommonUtility.AssetToolSchedulerColumn.Quantity.ToString()] == null ||
                                                    Convert.ToString(item[CommonUtility.AssetToolSchedulerColumn.Quantity.ToString()]) == "")
                                                {
                                                    obj.Quantity = 0;
                                                }
                                                else
                                                {
                                                    double _Quantity = 0.0;
                                                    if (double.TryParse(item[CommonUtility.AssetToolSchedulerColumn.Quantity.ToString()].ToString(), out _Quantity))
                                                        obj.Quantity = _Quantity;
                                                    else
                                                        obj.Quantity = 0;
                                                }

                                            }



                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.AssetToolSchedulerColumn.UDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.AssetToolSchedulerColumn.UDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.AssetToolSchedulerColumn.UDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.AssetToolSchedulerColumn.UDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.AssetToolSchedulerColumn.UDF5.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.ItemNumber.ToString()))
                                            {
                                                obj.ItemNumber = item[CommonUtility.AssetToolSchedulerColumn.ItemNumber.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.IsDeleted.ToString()))
                                            {
                                                bool Tempbool = false;
                                                bool.TryParse(item[CommonUtility.AssetToolSchedulerColumn.IsDeleted.ToString()].ToString(), out Tempbool);
                                                obj.IsDeleted = Tempbool;
                                            }
                                            //if (item.Table.Columns.Contains(CommonUtility.AssetToolSchedulerColumn.IsDeleted.ToString()))
                                            //{
                                            //    obj.IsDeleted = item[CommonUtility.AssetToolSchedulerColumn.IsDeleted.ToString()].ToString();
                                            //}

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion


                            #region Past Maintenance Due
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.PastMaintenanceDue.ToString())
                            {
                                Int64 i = 1;
                                List<PastMaintenanceDueImport> lstImport = new List<PastMaintenanceDueImport>();
                                foreach (DataRow item in list)
                                {
                                    PastMaintenanceDueImport obj = new PastMaintenanceDueImport();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.PastMaintenanceDueImportColumn.ScheduleFor.ToString()].ToString() != "" && item[CommonUtility.PastMaintenanceDueImportColumn.MaintenanceDate.ToString()].ToString() != "" && item[CommonUtility.PastMaintenanceDueImportColumn.SchedulerName.ToString()].ToString() != "")
                                        {
                                            obj.ID = i;
                                            //  obj.ID = Convert.ToInt32(item[CommonUtility.ImportInventoryClassificationColumn.ID.ToString()].ToString());
                                            if (item.Table.Columns.Contains(CommonUtility.PastMaintenanceDueImportColumn.ScheduleFor.ToString()))
                                            {
                                                obj.ScheduleFor = item[CommonUtility.PastMaintenanceDueImportColumn.ScheduleFor.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PastMaintenanceDueImportColumn.MaintenanceDate.ToString()))
                                            {
                                                if (item[CommonUtility.PastMaintenanceDueImportColumn.MaintenanceDate.ToString()].ToString() != "")
                                                {
                                                    DateTime dt;
                                                    DateTime.TryParseExact(item[CommonUtility.PastMaintenanceDueImportColumn.MaintenanceDate.ToString()].ToString().Split(' ')[0], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);
                                                    if (dt != DateTime.MinValue)
                                                    {
                                                        obj.MaintenanceDate = dt;
                                                        obj.displayMaitenanceDate = dt.ToString(SessionHelper.RoomDateFormat);
                                                    }
                                                    else
                                                    {
                                                        obj.MaintenanceDate = DateTime.Now;
                                                        obj.displayMaitenanceDate = DateTime.Now.ToString(SessionHelper.RoomDateFormat);
                                                        obj.Reason = string.Format(ResImportMasters.DateShouldBeInFormat, ResToolsMaintenance.MaintenanceDate, SessionHelper.RoomDateFormat);
                                                        obj.Status = "Fail";
                                                    }
                                                }
                                                else
                                                {
                                                    obj.MaintenanceDate = DateTime.Now;
                                                    obj.displayMaitenanceDate = DateTime.Now.ToString(SessionHelper.RoomDateFormat);
                                                }

                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PastMaintenanceDueImportColumn.AssetName.ToString()))
                                            {
                                                obj.AssetName = item[CommonUtility.PastMaintenanceDueImportColumn.AssetName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PastMaintenanceDueImportColumn.ToolName.ToString()))
                                            {
                                                obj.ToolName = item[CommonUtility.PastMaintenanceDueImportColumn.ToolName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PastMaintenanceDueImportColumn.Serial.ToString()))
                                            {
                                                obj.Serial = item[CommonUtility.PastMaintenanceDueImportColumn.Serial.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PastMaintenanceDueImportColumn.SchedulerName.ToString()))
                                            {
                                                obj.SchedulerName = item[CommonUtility.PastMaintenanceDueImportColumn.SchedulerName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PastMaintenanceDueImportColumn.ItemNumber.ToString()))
                                            {
                                                obj.ItemNumber = item[CommonUtility.PastMaintenanceDueImportColumn.ItemNumber.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.PastMaintenanceDueImportColumn.ItemCost.ToString()))
                                            {
                                                if (item[CommonUtility.PastMaintenanceDueImportColumn.ItemCost.ToString()] == null || Convert.ToString(item[CommonUtility.PastMaintenanceDueImportColumn.ItemCost.ToString()]) == "")
                                                {
                                                    obj.ItemCost = 0;
                                                }
                                                else
                                                {
                                                    double _ItemCostValue = 0.0;
                                                    if (double.TryParse(item[CommonUtility.PastMaintenanceDueImportColumn.ItemCost.ToString()].ToString(), out _ItemCostValue))
                                                        obj.ItemCost = _ItemCostValue;
                                                    else
                                                        obj.ItemCost = 0;
                                                }

                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.PastMaintenanceDueImportColumn.Quantity.ToString()))
                                            {
                                                if (item[CommonUtility.PastMaintenanceDueImportColumn.Quantity.ToString()] == null || Convert.ToString(item[CommonUtility.PastMaintenanceDueImportColumn.Quantity.ToString()]) == "")
                                                {
                                                    obj.Quantity = 0;
                                                }
                                                else
                                                {
                                                    double _QuantityValue = 0.0;
                                                    if (double.TryParse(item[CommonUtility.ImportInventoryLocationColumn.Quantity.ToString()].ToString(), out _QuantityValue))
                                                        obj.Quantity = _QuantityValue;
                                                    else
                                                        obj.Quantity = 0;
                                                }

                                            }
                                            i++;
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion

                            #region Tool master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ToolCheckInOutHistory.ToString())
                            {
                                List<ToolCheckInCheckOut> lstImport = new List<ToolCheckInCheckOut>();
                                int recordCount = 1;
                                var isAllowedToolOrdering = eTurnsWeb.Helper.SessionHelper.AllowToolOrdering;
                                foreach (DataRow item in list)
                                {
                                    ToolCheckInCheckOut obj = new ToolCheckInCheckOut();
                                    try
                                    {
                                        if ((isAllowedToolOrdering && item[CommonUtility.ImportToolCheckInCheckOutColumn.ToolName.ToString()].ToString() != "") || (!isAllowedToolOrdering && item[CommonUtility.ImportToolCheckInCheckOutColumn.Serial.ToString()].ToString() != ""))
                                        {
                                            if (isAllowedToolOrdering)
                                            {
                                                if (item.Table.Columns.Contains(CommonUtility.ImportToolCheckInCheckOutColumn.ToolName.ToString()))
                                                {
                                                    obj.ToolName = item[CommonUtility.ImportToolCheckInCheckOutColumn.ToolName.ToString()].ToString();
                                                }

                                                if (item.Table.Columns.Contains(CommonUtility.ImportToolCheckInCheckOutColumn.Location.ToString()))
                                                {
                                                    obj.Location = item[CommonUtility.ImportToolCheckInCheckOutColumn.Location.ToString()].ToString();
                                                }
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCheckInCheckOutColumn.Serial.ToString()))
                                            {
                                                obj.Serial = item[CommonUtility.ImportToolCheckInCheckOutColumn.Serial.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCheckInCheckOutColumn.TechnicianCode.ToString()))
                                            {
                                                obj.TechnicianCode = item[CommonUtility.ImportToolCheckInCheckOutColumn.TechnicianCode.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCheckInCheckOutColumn.Quantity.ToString()))
                                            {
                                                if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportToolCheckInCheckOutColumn.Quantity.ToString()])))
                                                {
                                                    double _Quantity = 0;
                                                    if (double.TryParse(item[CommonUtility.ImportToolCheckInCheckOutColumn.Quantity.ToString()].ToString(), out _Quantity))
                                                        obj.Quantity = _Quantity;
                                                    else
                                                        obj.Quantity = 0;
                                                }
                                                else
                                                    obj.Quantity = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCheckInCheckOutColumn.Operation.ToString()))
                                            {
                                                obj.Operation = item[CommonUtility.ImportToolCheckInCheckOutColumn.Operation.ToString()].ToString();
                                            }


                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCheckInCheckOutColumn.CheckOutUDF1.ToString()))
                                            {
                                                obj.CheckOutUDF1 = item[CommonUtility.ImportToolCheckInCheckOutColumn.CheckOutUDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCheckInCheckOutColumn.CheckOutUDF2.ToString()))
                                            {
                                                obj.CheckOutUDF2 = item[CommonUtility.ImportToolCheckInCheckOutColumn.CheckOutUDF2.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCheckInCheckOutColumn.CheckOutUDF3.ToString()))
                                            {
                                                obj.CheckOutUDF3 = item[CommonUtility.ImportToolCheckInCheckOutColumn.CheckOutUDF3.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCheckInCheckOutColumn.CheckOutUDF4.ToString()))
                                            {
                                                obj.CheckOutUDF4 = item[CommonUtility.ImportToolCheckInCheckOutColumn.CheckOutUDF4.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCheckInCheckOutColumn.CheckOutUDF5.ToString()))
                                            {
                                                obj.CheckOutUDF5 = item[CommonUtility.ImportToolCheckInCheckOutColumn.CheckOutUDF5.ToString()].ToString();
                                            }
                                            obj.Id = recordCount++;
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

                            #region Tool Adjustment Count
                            if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ToolAdjustmentCount.ToString())
                            {
                                ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
                                List<ToolAssetQuantityMain> lstImport = new List<ToolAssetQuantityMain>();
                                foreach (DataRow item in list)
                                {
                                    ToolAssetQuantityMain obj = new ToolAssetQuantityMain();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        if (item[CommonUtility.ImportToolAssetQuantityDetailsColumn.ToolName.ToString()].ToString() != "")
                                        //  AllowInsert = lstImport.Where(x => x.InventoryClassification == item[CommonUtility.ImportInventoryClassificationColumn.InventoryClassification.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)
                                        {

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolAssetQuantityDetailsColumn.LocationName.ToString()))
                                            {
                                                obj.BinNumber = item[CommonUtility.ImportToolAssetQuantityDetailsColumn.LocationName.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolAssetQuantityDetailsColumn.ToolName.ToString()))
                                            {
                                                obj.ToolName = item[CommonUtility.ImportToolAssetQuantityDetailsColumn.ToolName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolAssetQuantityDetailsColumn.Quantity.ToString()))
                                            {
                                                if (item[CommonUtility.ImportToolAssetQuantityDetailsColumn.Quantity.ToString()] == null || Convert.ToString(item[CommonUtility.ImportToolAssetQuantityDetailsColumn.Quantity.ToString()]) == "")
                                                {
                                                    obj.Quantity = 0;
                                                }
                                                else
                                                {
                                                    double _customerOwnedQtyValue = 0.0;
                                                    if (double.TryParse(item[CommonUtility.ImportToolAssetQuantityDetailsColumn.Quantity.ToString()].ToString(), out _customerOwnedQtyValue))
                                                        obj.Quantity = _customerOwnedQtyValue;
                                                    else
                                                        obj.Quantity = 0;
                                                }
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolAssetQuantityDetailsColumn.SerialNumber.ToString()))
                                            {
                                                obj.SerialNumber = item[CommonUtility.ImportToolAssetQuantityDetailsColumn.SerialNumber.ToString()].ToString();
                                            }


                                            if (objToolMasterDTO == null || obj.ToolName != objToolMasterDTO.ToolName)
                                            {
                                                objToolMasterDTO = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolByName(HttpUtility.HtmlDecode(obj.ToolName), SessionHelper.RoomID, SessionHelper.CompanyID);
                                            }

                                            if (objToolMasterDTO != null && (objToolMasterDTO.SerialNumberTracking))
                                            {
                                                obj.ToolGUID = objToolMasterDTO.GUID;
                                            }
                                            else if (objToolMasterDTO != null)
                                            {
                                                obj.ToolGUID = objToolMasterDTO.GUID;
                                                obj.SerialNumber = string.Empty;
                                            }
                                            else
                                            {
                                                obj.SerialNumber = string.Empty;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolAssetQuantityDetailsColumn.UDF1.ToString()))
                                            {
                                                obj.UDF1 = item[CommonUtility.ImportToolAssetQuantityDetailsColumn.UDF1.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolAssetQuantityDetailsColumn.UDF2.ToString()))
                                            {
                                                obj.UDF2 = item[CommonUtility.ImportToolAssetQuantityDetailsColumn.UDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolAssetQuantityDetailsColumn.UDF3.ToString()))
                                            {
                                                obj.UDF3 = item[CommonUtility.ImportToolAssetQuantityDetailsColumn.UDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolAssetQuantityDetailsColumn.UDF4.ToString()))
                                            {
                                                obj.UDF4 = item[CommonUtility.ImportToolAssetQuantityDetailsColumn.UDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolAssetQuantityDetailsColumn.UDF5.ToString()))
                                            {
                                                obj.UDF5 = item[CommonUtility.ImportToolAssetQuantityDetailsColumn.UDF5.ToString()].ToString();
                                            }

                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }

                                lstImport = (from ilq in lstImport
                                             group ilq by new { ilq.BinNumber, ilq.ToolName, ilq.SerialNumber, ilq.ToolGUID } into groupedilq
                                             select new ToolAssetQuantityMain
                                             {
                                                 BinNumber = groupedilq.Key.BinNumber,
                                                 CompanyID = SessionHelper.CompanyID,
                                                 Created = DateTime.UtcNow,
                                                 CreatedBy = SessionHelper.UserID,
                                                 Quantity = groupedilq.Sum(t => (t.Quantity ?? 0)),
                                                 GUID = Guid.NewGuid(),
                                                 ToolGUID = groupedilq.Key.ToolGUID,
                                                 ToolName = groupedilq.Key.ToolName,
                                                 UpdatedBy = SessionHelper.UserID,
                                                 Room = SessionHelper.RoomID,
                                                 SerialNumber = groupedilq.Key.SerialNumber,
                                                 Updated = DateTime.UtcNow,
                                                 Status = string.Join(",", groupedilq.Select(t => t.Status).ToArray()),
                                                 Reason = string.Join(",", groupedilq.Select(t => t.Reason).ToArray())

                                             }).ToList();
                                Session["importedData"] = lstImport;
                            }
                            #endregion

                            #region Tool Certification Images
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ToolCertificationImages.ToString())
                            {
                                List<ToolImageImport> lstImport = new List<ToolImageImport>();
                                int recordCount = 1;
                                foreach (DataRow item in list)
                                {
                                    ToolImageImport obj = new ToolImageImport();
                                    try
                                    {
                                        if ((item[CommonUtility.ImportToolCheckInCheckOutColumn.ToolName.ToString()].ToString() != ""))
                                        {
                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCertificationImagesColumn.ToolName.ToString()))
                                            {
                                                obj.ToolName = item[CommonUtility.ImportToolCertificationImagesColumn.ToolName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCertificationImagesColumn.Serial.ToString()))
                                            {
                                                obj.Serial = item[CommonUtility.ImportToolCertificationImagesColumn.Serial.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportToolCertificationImagesColumn.ImageName.ToString()))
                                            {
                                                obj.ImageName = item[CommonUtility.ImportToolCertificationImagesColumn.ImageName.ToString()].ToString();
                                            }

                                            obj.Id = recordCount++;
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

                            #region Order master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.OrderMaster.ToString())
                            {
                                //List<QuickListItemsMain> lstImport = new List<QuickListItemsMain>();
                                List<OrderMasterItemsMain> lstImport = new List<OrderMasterItemsMain>();
                                foreach (DataRow item in list)
                                {
                                    OrderMasterItemsMain obj = new OrderMasterItemsMain();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        if (item[CommonUtility.ImportOrderItemsColumn.Supplier.ToString()].ToString() != "")
                                        {
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.Supplier.ToString()))
                                            {
                                                obj.Supplier = item[CommonUtility.ImportOrderItemsColumn.Supplier.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.OrderNumber.ToString()))
                                            {
                                                obj.OrderNumber = item[CommonUtility.ImportOrderItemsColumn.OrderNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.RequiredDate.ToString()))
                                            {
                                                if (item[CommonUtility.ImportOrderItemsColumn.RequiredDate.ToString()].ToString() != "")
                                                {
                                                    DateTime dt;
                                                    string _expirationDt = item[CommonUtility.ImportOrderItemsColumn.RequiredDate.ToString()].ToString().Split(' ')[0];
                                                    DateTime.TryParseExact(_expirationDt, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);
                                                    if (dt != DateTime.MinValue)
                                                    {
                                                        obj.RequiredDate = dt.ToString(SessionHelper.RoomDateFormat);
                                                        //obj.displayExpiration = dt.ToString(SessionHelper.RoomDateFormat);
                                                    }
                                                    else
                                                    {
                                                        obj.RequiredDate = null;
                                                        //obj.displayExpiration = null;
                                                        obj.Reason = string.Format(ResImportMasters.DateShouldBeInFormat, ResOrder.RequiredDate, SessionHelper.RoomDateFormat);
                                                        obj.Status = "Fail";
                                                    }
                                                }
                                                else
                                                {
                                                    obj.RequiredDate = null;
                                                    //obj.displayExpiration = null;
                                                }
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.OrderStatus.ToString()))
                                            {
                                                obj.OrderStatus = item[CommonUtility.ImportOrderItemsColumn.OrderStatus.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.StagingName.ToString()))
                                            {
                                                obj.StagingName = item[CommonUtility.ImportOrderItemsColumn.StagingName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.OrderComment.ToString()))
                                            {
                                                obj.OrderComment = item[CommonUtility.ImportOrderItemsColumn.OrderComment.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.CustomerName.ToString()))
                                            {
                                                obj.CustomerName = item[CommonUtility.ImportOrderItemsColumn.CustomerName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.PackSlipNumber.ToString()))
                                            {
                                                obj.PackSlipNumber = item[CommonUtility.ImportOrderItemsColumn.PackSlipNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.ShippingTrackNumber.ToString()))
                                            {
                                                obj.ShippingTrackNumber = item[CommonUtility.ImportOrderItemsColumn.ShippingTrackNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.OrderUDF1.ToString()))
                                            {
                                                obj.OrderUDF1 = item[CommonUtility.ImportOrderItemsColumn.OrderUDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.OrderUDF2.ToString()))
                                            {
                                                obj.OrderUDF2 = item[CommonUtility.ImportOrderItemsColumn.OrderUDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.OrderUDF3.ToString()))
                                            {
                                                obj.OrderUDF3 = item[CommonUtility.ImportOrderItemsColumn.OrderUDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.OrderUDF4.ToString()))
                                            {
                                                obj.OrderUDF4 = item[CommonUtility.ImportOrderItemsColumn.OrderUDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.OrderUDF5.ToString()))
                                            {
                                                obj.OrderUDF5 = item[CommonUtility.ImportOrderItemsColumn.OrderUDF5.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.ShipVia.ToString()))
                                            {
                                                obj.ShipVia = item[CommonUtility.ImportOrderItemsColumn.ShipVia.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.OrderType.ToString()))
                                            {
                                                obj.OrderType = item[CommonUtility.ImportOrderItemsColumn.OrderType.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.ShippingVendor.ToString()))
                                            {
                                                obj.ShippingVendor = item[CommonUtility.ImportOrderItemsColumn.ShippingVendor.ToString()].ToString();
                                            }
                                            //if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.AccountNumber.ToString()))
                                            //{
                                            //    obj.AccountNumber = item[CommonUtility.ImportOrderItemsColumn.AccountNumber.ToString()].ToString();
                                            //}
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.SupplierAccount.ToString()))
                                            {
                                                obj.SupplierAccount = item[CommonUtility.ImportOrderItemsColumn.SupplierAccount.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.ItemNumber.ToString()))
                                            {
                                                obj.ItemNumber = item[CommonUtility.ImportOrderItemsColumn.ItemNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.Bin.ToString()))
                                            {
                                                obj.Bin = item[CommonUtility.ImportOrderItemsColumn.Bin.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.RequestedQty.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportOrderItemsColumn.RequestedQty.ToString()].ToString(), out _QtyValue))
                                                    obj.RequestedQty = _QtyValue;
                                                else
                                                    obj.RequestedQty = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.ReceivedQty.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportOrderItemsColumn.ReceivedQty.ToString()].ToString(), out _QtyValue))
                                                    obj.ReceivedQty = _QtyValue;
                                                else
                                                    obj.ReceivedQty = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.ASNNumber.ToString()))
                                            {
                                                obj.ASNNumber = item[CommonUtility.ImportOrderItemsColumn.ASNNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.ApprovedQty.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportOrderItemsColumn.ApprovedQty.ToString()].ToString(), out _QtyValue))
                                                    obj.ApprovedQty = _QtyValue;
                                                else
                                                    obj.ApprovedQty = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.InTransitQty.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportOrderItemsColumn.InTransitQty.ToString()].ToString(), out _QtyValue))
                                                    obj.InTransitQty = _QtyValue;
                                                else
                                                    obj.InTransitQty = 0;
                                            }
                                            //if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.IsCloseItem.ToString()))
                                            //{
                                            //    //Boolean IsCloseItem = false;
                                            //    //Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportOrderItemsColumn.IsCloseItem.ToString()]), out IsCloseItem);
                                            //    //obj.IsCloseItem = IsCloseItem;
                                            //    obj.IsCloseItem = item[CommonUtility.ImportOrderItemsColumn.IsCloseItem.ToString()].ToString();
                                            //}
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.IsCloseItem.ToString()))
                                            {
                                                Boolean IsCloseItem = false;
                                                if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportOrderItemsColumn.IsCloseItem.ToString()]), out IsCloseItem))
                                                    obj.IsCloseItem = IsCloseItem;
                                                else
                                                    obj.IsCloseItem = false;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.LineNumber.ToString()))
                                            {
                                                obj.LineNumber = item[CommonUtility.ImportOrderItemsColumn.LineNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.ControlNumber.ToString()))
                                            {
                                                obj.ControlNumber = item[CommonUtility.ImportOrderItemsColumn.ControlNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.ItemComment.ToString()))
                                            {
                                                obj.ItemComment = item[CommonUtility.ImportOrderItemsColumn.ItemComment.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.LineItemUDF1.ToString()))
                                            {
                                                obj.LineItemUDF1 = item[CommonUtility.ImportOrderItemsColumn.LineItemUDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.LineItemUDF2.ToString()))
                                            {
                                                obj.LineItemUDF2 = item[CommonUtility.ImportOrderItemsColumn.LineItemUDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.LineItemUDF3.ToString()))
                                            {
                                                obj.LineItemUDF3 = item[CommonUtility.ImportOrderItemsColumn.LineItemUDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.LineItemUDF4.ToString()))
                                            {
                                                obj.LineItemUDF4 = item[CommonUtility.ImportOrderItemsColumn.LineItemUDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.LineItemUDF5.ToString()))
                                            {
                                                obj.LineItemUDF5 = item[CommonUtility.ImportOrderItemsColumn.LineItemUDF5.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.SalesOrder.ToString()))
                                            {
                                                obj.SalesOrder = item[CommonUtility.ImportOrderItemsColumn.SalesOrder.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.OrderCost.ToString()))
                                            {
                                                double _OrderCostValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportOrderItemsColumn.OrderCost.ToString()].ToString(), out _OrderCostValue))
                                                    obj.OrderCost = _OrderCostValue;
                                                else
                                                    obj.OrderCost = null;
                                            }
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion
                            #region Move Material
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.MoveMaterial.ToString())
                            {
                                List<MoveMaterial> lstImport = new List<MoveMaterial>();
                                int recordCount = 1;
                                foreach (DataRow item in list)
                                {
                                    MoveMaterial obj = new MoveMaterial();
                                    try
                                    {
                                        if (item[CommonUtility.ImportMoveMaterialColumn.ItemNumber.ToString()].ToString() != "")
                                        {

                                            if (item.Table.Columns.Contains(CommonUtility.ImportMoveMaterialColumn.ItemNumber.ToString()))
                                            {
                                                obj.ItemNumber = item[CommonUtility.ImportMoveMaterialColumn.ItemNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportMoveMaterialColumn.SourceBin.ToString()))
                                            {
                                                obj.SourceBin = item[CommonUtility.ImportMoveMaterialColumn.SourceBin.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportMoveMaterialColumn.DestinationBin.ToString()))
                                            {
                                                obj.DestinationBin = item[CommonUtility.ImportMoveMaterialColumn.DestinationBin.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportMoveMaterialColumn.MoveType.ToString()))
                                            {
                                                obj.MoveType = item[CommonUtility.ImportMoveMaterialColumn.MoveType.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportMoveMaterialColumn.Quantity.ToString()))
                                            {
                                                if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportMoveMaterialColumn.Quantity.ToString()])))
                                                {
                                                    double _Quantity = 0;
                                                    if (double.TryParse(item[CommonUtility.ImportMoveMaterialColumn.Quantity.ToString()].ToString(), out _Quantity))
                                                        obj.Quantity = _Quantity;
                                                    else
                                                        obj.Quantity = 0;
                                                }
                                                else
                                                    obj.Quantity = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportMoveMaterialColumn.DestinationStagingHeader.ToString()))
                                            {
                                                obj.DestinationStagingHeader = item[CommonUtility.ImportMoveMaterialColumn.DestinationStagingHeader.ToString()].ToString();
                                            }
                                            obj.Id = recordCount++;
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
                            #region Enterprise List
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.EnterpriseQuickList.ToString())
                            {
                                List<EnterpriseQLImport> lstImport = new List<EnterpriseQLImport>();
                                int recordCount = 1;
                                foreach (DataRow item in list)
                                {
                                    EnterpriseQLImport obj = new EnterpriseQLImport();
                                    try
                                    {
                                        if (item[CommonUtility.ImportEnterpriseQuickListColumn.Name.ToString()].ToString() != "")
                                        {
                                            if (item.Table.Columns.Contains(CommonUtility.ImportEnterpriseQuickListColumn.Name.ToString()))
                                            {
                                                obj.Name = item[CommonUtility.ImportEnterpriseQuickListColumn.Name.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportEnterpriseQuickListColumn.QLDetailNumber.ToString()))
                                            {
                                                obj.QLDetailNumber = item[CommonUtility.ImportEnterpriseQuickListColumn.QLDetailNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportEnterpriseQuickListColumn.Quantity.ToString()))
                                            {
                                                if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportEnterpriseQuickListColumn.Quantity.ToString()])))
                                                {
                                                    double _Quantity = 1;
                                                    if (double.TryParse(item[CommonUtility.ImportEnterpriseQuickListColumn.Quantity.ToString()].ToString(), out _Quantity))
                                                        obj.Quantity = _Quantity;
                                                    else
                                                        obj.Quantity = 1;
                                                }
                                                else
                                                    obj.Quantity = 1;
                                            }
                                            obj.Id = recordCount++;
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

                            #region Requisition
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.Requisition.ToString())
                            {
                                List<RequisitionImport> lstImport = new List<RequisitionImport>();
                                int recordCount = 1;
                                foreach (DataRow item in list)
                                {
                                    RequisitionImport obj = new RequisitionImport();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportRequisitionColumn.RequisitionNumber.ToString()]))
                                            && !string.IsNullOrWhiteSpace(Convert.ToString(item[CommonUtility.ImportRequisitionColumn.RequisitionNumber.ToString()])))
                                        {
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.RequisitionNumber.ToString()))
                                            {
                                                obj.RequisitionNumber = item[CommonUtility.ImportRequisitionColumn.RequisitionNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.Workorder.ToString()))
                                            {
                                                obj.Workorder = item[CommonUtility.ImportRequisitionColumn.Workorder.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.RequiredDate.ToString()))
                                            {
                                                if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportRequisitionColumn.RequiredDate.ToString()].ToString()))
                                                 && !string.IsNullOrWhiteSpace(Convert.ToString(item[CommonUtility.ImportRequisitionColumn.RequiredDate.ToString()].ToString())))
                                                {
                                                    DateTime dt;
                                                    string _expirationDt = item[CommonUtility.ImportRequisitionColumn.RequiredDate.ToString()].ToString().Split(' ')[0];
                                                    DateTime.TryParseExact(_expirationDt, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);

                                                    if (dt != DateTime.MinValue)
                                                    {
                                                        obj.RequiredDate = dt.ToString(SessionHelper.RoomDateFormat);
                                                    }
                                                    else
                                                    {
                                                        obj.RequiredDate = null;
                                                        obj.Reason = string.Format(ResImportMasters.DateShouldBeInFormat, ResRequisitionMaster.RequiredDate, SessionHelper.RoomDateFormat);
                                                        obj.Status = "Fail";
                                                    }
                                                }
                                                else
                                                {
                                                    obj.RequiredDate = null;
                                                }
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.RequisitionStatus.ToString()))
                                            {
                                                obj.RequisitionStatus = item[CommonUtility.ImportRequisitionColumn.RequisitionStatus.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.CustomerName.ToString()))
                                            {
                                                obj.CustomerName = item[CommonUtility.ImportRequisitionColumn.CustomerName.ToString()].ToString();
                                            }
                                            //if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.ReleaseNumber.ToString()))
                                            //{
                                            //    obj.ReleaseNumber = item[CommonUtility.ImportRequisitionColumn.ReleaseNumber.ToString()].ToString();
                                            //}
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.ProjectSpend.ToString()))
                                            {
                                                obj.ProjectSpend = item[CommonUtility.ImportRequisitionColumn.ProjectSpend.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.Description.ToString()))
                                            {
                                                obj.Description = item[CommonUtility.ImportRequisitionColumn.Description.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.StagingName.ToString()))
                                            {
                                                obj.StagingName = item[CommonUtility.ImportRequisitionColumn.StagingName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.Supplier.ToString()))
                                            {
                                                obj.Supplier = item[CommonUtility.ImportRequisitionColumn.Supplier.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.SupplierAccount.ToString()))
                                            {
                                                obj.SupplierAccount = item[CommonUtility.ImportRequisitionColumn.SupplierAccount.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.BillingAccount.ToString()))
                                            {
                                                obj.BillingAccount = item[CommonUtility.ImportRequisitionColumn.BillingAccount.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.Technician.ToString()))
                                            {
                                                obj.Technician = item[CommonUtility.ImportRequisitionColumn.Technician.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.RequisitionUDF1.ToString()))
                                            {
                                                obj.RequisitionUDF1 = item[CommonUtility.ImportRequisitionColumn.RequisitionUDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.RequisitionUDF2.ToString()))
                                            {
                                                obj.RequisitionUDF2 = item[CommonUtility.ImportRequisitionColumn.RequisitionUDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.RequisitionUDF3.ToString()))
                                            {
                                                obj.RequisitionUDF3 = item[CommonUtility.ImportRequisitionColumn.RequisitionUDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.RequisitionUDF4.ToString()))
                                            {
                                                obj.RequisitionUDF4 = item[CommonUtility.ImportRequisitionColumn.RequisitionUDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.RequisitionUDF5.ToString()))
                                            {
                                                obj.RequisitionUDF5 = item[CommonUtility.ImportRequisitionColumn.RequisitionUDF5.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.ItemNumber.ToString()))
                                            {
                                                obj.ItemNumber = item[CommonUtility.ImportRequisitionColumn.ItemNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.Tool.ToString()))
                                            {
                                                obj.Tool = item[CommonUtility.ImportRequisitionColumn.Tool.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.ToolSerial.ToString()))
                                            {
                                                obj.ToolSerial = item[CommonUtility.ImportRequisitionColumn.ToolSerial.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.Bin.ToString()))
                                            {
                                                obj.Bin = item[CommonUtility.ImportRequisitionColumn.Bin.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.QuantityRequisitioned.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportRequisitionColumn.QuantityRequisitioned.ToString()].ToString(), out _QtyValue))
                                                    obj.QuantityRequisitioned = _QtyValue;
                                                else
                                                    obj.QuantityRequisitioned = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.QuantityApproved.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportRequisitionColumn.QuantityApproved.ToString()].ToString(), out _QtyValue))
                                                    obj.QuantityApproved = _QtyValue;
                                                else
                                                    obj.QuantityApproved = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.QuantityPulled.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportRequisitionColumn.QuantityPulled.ToString()].ToString(), out _QtyValue))
                                                    obj.QuantityPulled = _QtyValue;
                                                else
                                                    obj.QuantityPulled = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.LineItemRequiredDate.ToString()))
                                            {
                                                if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportRequisitionColumn.LineItemRequiredDate.ToString()].ToString()))
                                                 && !string.IsNullOrWhiteSpace(Convert.ToString(item[CommonUtility.ImportRequisitionColumn.LineItemRequiredDate.ToString()].ToString())))
                                                {
                                                    DateTime dt;
                                                    string _expirationDt = item[CommonUtility.ImportRequisitionColumn.LineItemRequiredDate.ToString()].ToString().Split(' ')[0];
                                                    DateTime.TryParseExact(_expirationDt, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);

                                                    if (dt != DateTime.MinValue)
                                                    {
                                                        obj.LineItemRequiredDate = dt.ToString(SessionHelper.RoomDateFormat);
                                                    }
                                                    else
                                                    {
                                                        obj.LineItemRequiredDate = null;
                                                        obj.Reason = string.Format(ResImportMasters.DateShouldBeInFormat, ResRequisitionDetails.LineItemRequiredDate, SessionHelper.RoomDateFormat);
                                                        obj.Status = "Fail";
                                                    }
                                                }
                                                else
                                                {
                                                    obj.LineItemRequiredDate = null;
                                                }
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.LineItemProjectSpend.ToString()))
                                            {
                                                obj.LineItemProjectSpend = item[CommonUtility.ImportRequisitionColumn.LineItemProjectSpend.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.LineItemSupplierAccount.ToString()))
                                            {
                                                obj.LineItemSupplierAccount = item[CommonUtility.ImportRequisitionColumn.LineItemSupplierAccount.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.LineItemTechnician.ToString()))
                                            {
                                                obj.LineItemTechnician = item[CommonUtility.ImportRequisitionColumn.LineItemTechnician.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.PullOrderNumber.ToString()))
                                            {
                                                obj.PullOrderNumber = item[CommonUtility.ImportRequisitionColumn.PullOrderNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.PullUDF1.ToString()))
                                            {
                                                obj.PullUDF1 = item[CommonUtility.ImportRequisitionColumn.PullUDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.PullUDF2.ToString()))
                                            {
                                                obj.PullUDF2 = item[CommonUtility.ImportRequisitionColumn.PullUDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.PullUDF3.ToString()))
                                            {
                                                obj.PullUDF3 = item[CommonUtility.ImportRequisitionColumn.PullUDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.PullUDF4.ToString()))
                                            {
                                                obj.PullUDF4 = item[CommonUtility.ImportRequisitionColumn.PullUDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.PullUDF5.ToString()))
                                            {
                                                obj.PullUDF5 = item[CommonUtility.ImportRequisitionColumn.PullUDF5.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.ToolCheckoutUDF1.ToString()))
                                            {
                                                obj.ToolCheckoutUDF1 = item[CommonUtility.ImportRequisitionColumn.ToolCheckoutUDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.ToolCheckoutUDF2.ToString()))
                                            {
                                                obj.ToolCheckoutUDF2 = item[CommonUtility.ImportRequisitionColumn.ToolCheckoutUDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.ToolCheckoutUDF3.ToString()))
                                            {
                                                obj.ToolCheckoutUDF3 = item[CommonUtility.ImportRequisitionColumn.ToolCheckoutUDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.ToolCheckoutUDF4.ToString()))
                                            {
                                                obj.ToolCheckoutUDF4 = item[CommonUtility.ImportRequisitionColumn.ToolCheckoutUDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportRequisitionColumn.ToolCheckoutUDF5.ToString()))
                                            {
                                                obj.ToolCheckoutUDF5 = item[CommonUtility.ImportRequisitionColumn.ToolCheckoutUDF5.ToString()].ToString();
                                            }
                                            obj.Id = recordCount++;
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion


                            #region Quote master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.QuoteMaster.ToString())
                            {
                                //List<QuickListItemsMain> lstImport = new List<QuickListItemsMain>();
                                List<QuoteMasterItemsMain> lstImport = new List<QuoteMasterItemsMain>();
                                foreach (DataRow item in list)
                                {
                                    QuoteMasterItemsMain obj = new QuoteMasterItemsMain();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        if (item[CommonUtility.ImportQuoteItemsColumn.SupplierName.ToString()].ToString() != "")
                                        {
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.SupplierName.ToString()))
                                            {
                                                obj.SupplierName = item[CommonUtility.ImportQuoteItemsColumn.SupplierName.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.QuoteNumber.ToString()))
                                            {
                                                obj.QuoteNumber = item[CommonUtility.ImportQuoteItemsColumn.QuoteNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.RequiredDate.ToString()))
                                            {
                                                if (item[CommonUtility.ImportQuoteItemsColumn.RequiredDate.ToString()].ToString() != "")
                                                {
                                                    DateTime dt;
                                                    string _expirationDt = item[CommonUtility.ImportQuoteItemsColumn.RequiredDate.ToString()].ToString().Split(' ')[0];
                                                    DateTime.TryParseExact(_expirationDt, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);
                                                    if (dt != DateTime.MinValue)
                                                    {
                                                        obj.RequiredDate = dt.ToString(SessionHelper.RoomDateFormat);
                                                        //obj.displayExpiration = dt.ToString(SessionHelper.RoomDateFormat);
                                                    }
                                                    else
                                                    {
                                                        obj.RequiredDate = null;
                                                        //obj.displayExpiration = null;
                                                        obj.Reason = string.Format(ResImportMasters.DateShouldBeInFormat, ResQuoteMaster.RequiredDate, SessionHelper.RoomDateFormat);
                                                        obj.Status = "Fail";
                                                    }
                                                }
                                                else
                                                {
                                                    obj.RequiredDate = null;
                                                    //obj.displayExpiration = null;
                                                }
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.QuoteStatus.ToString()))
                                            {
                                                obj.QuoteStatus = item[CommonUtility.ImportQuoteItemsColumn.QuoteStatus.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.QuoteComment.ToString()))
                                            {
                                                obj.QuoteComment = item[CommonUtility.ImportQuoteItemsColumn.QuoteComment.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.CustomerName.ToString()))
                                            {
                                                obj.CustomerName = item[CommonUtility.ImportQuoteItemsColumn.CustomerName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.PackSlipNumber.ToString()))
                                            {
                                                obj.PackSlipNumber = item[CommonUtility.ImportQuoteItemsColumn.PackSlipNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.ShippingTrackNumber.ToString()))
                                            {
                                                obj.ShippingTrackNumber = item[CommonUtility.ImportQuoteItemsColumn.ShippingTrackNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.QuoteUDF1.ToString()))
                                            {
                                                obj.QuoteUDF1 = item[CommonUtility.ImportQuoteItemsColumn.QuoteUDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.QuoteUDF2.ToString()))
                                            {
                                                obj.QuoteUDF2 = item[CommonUtility.ImportQuoteItemsColumn.QuoteUDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.QuoteUDF3.ToString()))
                                            {
                                                obj.QuoteUDF3 = item[CommonUtility.ImportQuoteItemsColumn.QuoteUDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.QuoteUDF4.ToString()))
                                            {
                                                obj.QuoteUDF4 = item[CommonUtility.ImportQuoteItemsColumn.QuoteUDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.QuoteUDF5.ToString()))
                                            {
                                                obj.QuoteUDF5 = item[CommonUtility.ImportQuoteItemsColumn.QuoteUDF5.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.ShipVia.ToString()))
                                            {
                                                obj.ShipVia = item[CommonUtility.ImportQuoteItemsColumn.ShipVia.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.ShippingVendor.ToString()))
                                            {
                                                obj.ShippingVendor = item[CommonUtility.ImportQuoteItemsColumn.ShippingVendor.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.SupplierAccount.ToString()))
                                            {
                                                obj.SupplierAccount = item[CommonUtility.ImportQuoteItemsColumn.SupplierAccount.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.ItemNumber.ToString()))
                                            {
                                                obj.ItemNumber = item[CommonUtility.ImportQuoteItemsColumn.ItemNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.Bin.ToString()))
                                            {
                                                obj.Bin = item[CommonUtility.ImportQuoteItemsColumn.Bin.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.RequestedQty.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportQuoteItemsColumn.RequestedQty.ToString()].ToString(), out _QtyValue))
                                                    obj.RequestedQty = _QtyValue;
                                                else
                                                    obj.RequestedQty = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.ASNNumber.ToString()))
                                            {
                                                obj.ASNNumber = item[CommonUtility.ImportQuoteItemsColumn.ASNNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.ApprovedQty.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportQuoteItemsColumn.ApprovedQty.ToString()].ToString(), out _QtyValue))
                                                    obj.ApprovedQty = _QtyValue;
                                                else
                                                    obj.ApprovedQty = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.InTransitQty.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportQuoteItemsColumn.InTransitQty.ToString()].ToString(), out _QtyValue))
                                                    obj.InTransitQty = _QtyValue;
                                                else
                                                    obj.InTransitQty = 0;
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.IsCloseItem.ToString()))
                                            {
                                                Boolean IsCloseItem = false;
                                                if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportQuoteItemsColumn.IsCloseItem.ToString()]), out IsCloseItem))
                                                    obj.IsCloseItem = IsCloseItem;
                                                else
                                                    obj.IsCloseItem = false;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.LineNumber.ToString()))
                                            {
                                                obj.LineNumber = item[CommonUtility.ImportQuoteItemsColumn.LineNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.ControlNumber.ToString()))
                                            {
                                                obj.ControlNumber = item[CommonUtility.ImportQuoteItemsColumn.ControlNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.ItemComment.ToString()))
                                            {
                                                obj.ItemComment = item[CommonUtility.ImportQuoteItemsColumn.ItemComment.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.LineItemUDF1.ToString()))
                                            {
                                                obj.LineItemUDF1 = item[CommonUtility.ImportQuoteItemsColumn.LineItemUDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.LineItemUDF2.ToString()))
                                            {
                                                obj.LineItemUDF2 = item[CommonUtility.ImportQuoteItemsColumn.LineItemUDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.LineItemUDF3.ToString()))
                                            {
                                                obj.LineItemUDF3 = item[CommonUtility.ImportQuoteItemsColumn.LineItemUDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.LineItemUDF4.ToString()))
                                            {
                                                obj.LineItemUDF4 = item[CommonUtility.ImportQuoteItemsColumn.LineItemUDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.LineItemUDF5.ToString()))
                                            {
                                                obj.LineItemUDF5 = item[CommonUtility.ImportQuoteItemsColumn.LineItemUDF5.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportQuoteItemsColumn.QuoteCost.ToString()))
                                            {
                                                double _QuoteCostValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportQuoteItemsColumn.QuoteCost.ToString()].ToString(), out _QuoteCostValue))
                                                    obj.QuoteCost = _QuoteCostValue;
                                                else
                                                    obj.QuoteCost = null;
                                            }
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion

                            #region Supplier Catalog Import

                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.SupplierCatalog.ToString())
                            {
                                List<SupplierCatalogImport> lstImport = new List<SupplierCatalogImport>();
                                int recordCount = 1;
                                foreach (DataRow item in list)
                                {
                                    SupplierCatalogImport obj = new SupplierCatalogImport();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(item[CommonUtility.ImportSupplierCatalogColumn.ItemNumber.ToString()]))
                                            && !string.IsNullOrWhiteSpace(Convert.ToString(item[CommonUtility.ImportSupplierCatalogColumn.ItemNumber.ToString()])))
                                        {
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.ItemNumber.ToString()))
                                            {
                                                obj.ItemNumber = item[CommonUtility.ImportSupplierCatalogColumn.ItemNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.Description.ToString()))
                                            {
                                                obj.Description = item[CommonUtility.ImportSupplierCatalogColumn.Description.ToString()].ToString();
                                            }

                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.SellPrice.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportSupplierCatalogColumn.SellPrice.ToString()].ToString(), out _QtyValue))
                                                    obj.SellPrice = _QtyValue;
                                                else
                                                    obj.SellPrice = 0;
                                            }
                                            //if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.PackingQuantity.ToString()))
                                            //{
                                            //    double _QtyValue = 0;
                                            //    if (double.TryParse(item[CommonUtility.ImportSupplierCatalogColumn.PackingQuantity.ToString()].ToString(), out _QtyValue))
                                            //        obj.PackingQuantity = _QtyValue;
                                            //    else
                                            //        obj.PackingQuantity = 0;
                                            //}
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.ManufacturerPartNumber.ToString()))
                                            {
                                                obj.ManufacturerPartNumber = item[CommonUtility.ImportSupplierCatalogColumn.ManufacturerPartNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.ImagePath.ToString()))
                                            {
                                                obj.ImagePath = item[CommonUtility.ImportSupplierCatalogColumn.ImagePath.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.UPC.ToString()))
                                            {
                                                obj.UPC = item[CommonUtility.ImportSupplierCatalogColumn.UPC.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.SupplierPartNumber.ToString()))
                                            {
                                                obj.SupplierPartNumber = item[CommonUtility.ImportSupplierCatalogColumn.SupplierPartNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.SupplierName.ToString()))
                                            {
                                                obj.SupplierName = item[CommonUtility.ImportSupplierCatalogColumn.SupplierName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.ManufacturerName.ToString()))
                                            {
                                                obj.ManufacturerName = item[CommonUtility.ImportSupplierCatalogColumn.ManufacturerName.ToString()].ToString();
                                            }
                                            //if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.ConcatedColumnText.ToString()))
                                            //{
                                            //    obj.ConcatedColumnText = item[CommonUtility.ImportSupplierCatalogColumn.ConcatedColumnText.ToString()].ToString();
                                            //}
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.UOM.ToString()))
                                            {
                                                obj.UOM = item[CommonUtility.ImportSupplierCatalogColumn.UOM.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.CostUOM.ToString()))
                                            {
                                                obj.CostUOM = item[CommonUtility.ImportSupplierCatalogColumn.CostUOM.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportSupplierCatalogColumn.Cost.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportSupplierCatalogColumn.Cost.ToString()].ToString(), out _QtyValue))
                                                    obj.Cost = _QtyValue;
                                                else
                                                    obj.Cost = 0;
                                            }

                                            obj.Id = recordCount++;
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }

                            #endregion

                            #region Return Order master
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.ReturnOrders.ToString())
                            {
                                //List<QuickListItemsMain> lstImport = new List<QuickListItemsMain>();
                                List<OrderMasterItemsMain> lstImport = new List<OrderMasterItemsMain>();
                                foreach (DataRow item in list)
                                {
                                    OrderMasterItemsMain obj = new OrderMasterItemsMain();
                                    obj.Status = "N/A";
                                    obj.Reason = "N/A";
                                    try
                                    {
                                        if (item[CommonUtility.ImportReturnOrderItemsColumn.Supplier.ToString()].ToString() != "")
                                        {
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.Supplier.ToString()))
                                            {
                                                obj.Supplier = item[CommonUtility.ImportReturnOrderItemsColumn.Supplier.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderNumber.ToString()))
                                            {
                                                obj.OrderNumber = item[CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ReturnDate.ToString()))
                                            {
                                                if (item[CommonUtility.ImportReturnOrderItemsColumn.ReturnDate.ToString()].ToString() != "")
                                                {
                                                    DateTime dt;
                                                    string _expirationDt = item[CommonUtility.ImportReturnOrderItemsColumn.ReturnDate.ToString()].ToString().Split(' ')[0];
                                                    DateTime.TryParseExact(_expirationDt, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dt);
                                                    if (dt != DateTime.MinValue)
                                                    {
                                                        obj.RequiredDate = dt.ToString(SessionHelper.RoomDateFormat);
                                                        //obj.displayExpiration = dt.ToString(SessionHelper.RoomDateFormat);
                                                    }
                                                    else
                                                    {
                                                        obj.RequiredDate = null;
                                                        //obj.displayExpiration = null;
                                                        obj.Reason = string.Format(ResImportMasters.DateShouldBeInFormat, ResOrder.RequiredDate, SessionHelper.RoomDateFormat);
                                                        obj.Status = "Fail";
                                                    }
                                                }
                                                else
                                                {
                                                    obj.RequiredDate = null;
                                                    //obj.displayExpiration = null;
                                                }
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderStatus.ToString()))
                                            {
                                                obj.OrderStatus = item[CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderStatus.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.StagingName.ToString()))
                                            {
                                                obj.StagingName = item[CommonUtility.ImportReturnOrderItemsColumn.StagingName.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderComment.ToString()))
                                            {
                                                obj.OrderComment = item[CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderComment.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.PackSlipNumber.ToString()))
                                            {
                                                obj.PackSlipNumber = item[CommonUtility.ImportReturnOrderItemsColumn.PackSlipNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ShippingTrackNumber.ToString()))
                                            {
                                                obj.ShippingTrackNumber = item[CommonUtility.ImportReturnOrderItemsColumn.ShippingTrackNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderUDF1.ToString()))
                                            {
                                                obj.OrderUDF1 = item[CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderUDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderUDF2.ToString()))
                                            {
                                                obj.OrderUDF2 = item[CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderUDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderUDF3.ToString()))
                                            {
                                                obj.OrderUDF3 = item[CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderUDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderUDF4.ToString()))
                                            {
                                                obj.OrderUDF4 = item[CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderUDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderUDF5.ToString()))
                                            {
                                                obj.OrderUDF5 = item[CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderUDF5.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ShipVia.ToString()))
                                            {
                                                obj.ShipVia = item[CommonUtility.ImportReturnOrderItemsColumn.ShipVia.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.OrderType.ToString()))
                                            {
                                                obj.OrderType = item[CommonUtility.ImportReturnOrderItemsColumn.OrderType.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ShippingVendor.ToString()))
                                            {
                                                obj.ShippingVendor = item[CommonUtility.ImportReturnOrderItemsColumn.ShippingVendor.ToString()].ToString();
                                            }
                                            //if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.AccountNumber.ToString()))
                                            //{
                                            //    obj.AccountNumber = item[CommonUtility.ImportOrderItemsColumn.AccountNumber.ToString()].ToString();
                                            //}
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.SupplierAccount.ToString()))
                                            {
                                                obj.SupplierAccount = item[CommonUtility.ImportReturnOrderItemsColumn.SupplierAccount.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ItemNumber.ToString()))
                                            {
                                                obj.ItemNumber = item[CommonUtility.ImportReturnOrderItemsColumn.ItemNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.Bin.ToString()))
                                            {
                                                obj.Bin = item[CommonUtility.ImportReturnOrderItemsColumn.Bin.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.RequestedReturnedQty.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportReturnOrderItemsColumn.RequestedReturnedQty.ToString()].ToString(), out _QtyValue))
                                                    obj.RequestedQty = _QtyValue;
                                                else
                                                    obj.RequestedQty = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ReceivedReturnedQty.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportReturnOrderItemsColumn.ReceivedReturnedQty.ToString()].ToString(), out _QtyValue))
                                                    obj.ReceivedQty = _QtyValue;
                                                else
                                                    obj.ReceivedQty = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ASNNumber.ToString()))
                                            {
                                                obj.ASNNumber = item[CommonUtility.ImportReturnOrderItemsColumn.ASNNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ApprovedQty.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportReturnOrderItemsColumn.ApprovedQty.ToString()].ToString(), out _QtyValue))
                                                    obj.ApprovedQty = _QtyValue;
                                                else
                                                    obj.ApprovedQty = 0;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.InTransitQty.ToString()))
                                            {
                                                double _QtyValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportReturnOrderItemsColumn.InTransitQty.ToString()].ToString(), out _QtyValue))
                                                    obj.InTransitQty = _QtyValue;
                                                else
                                                    obj.InTransitQty = 0;
                                            }
                                            //if (item.Table.Columns.Contains(CommonUtility.ImportOrderItemsColumn.IsCloseItem.ToString()))
                                            //{
                                            //    //Boolean IsCloseItem = false;
                                            //    //Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportOrderItemsColumn.IsCloseItem.ToString()]), out IsCloseItem);
                                            //    //obj.IsCloseItem = IsCloseItem;
                                            //    obj.IsCloseItem = item[CommonUtility.ImportOrderItemsColumn.IsCloseItem.ToString()].ToString();
                                            //}
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.IsCloseItem.ToString()))
                                            {
                                                Boolean IsCloseItem = false;
                                                if (Boolean.TryParse(Convert.ToString(item[CommonUtility.ImportReturnOrderItemsColumn.IsCloseItem.ToString()]), out IsCloseItem))
                                                    obj.IsCloseItem = IsCloseItem;
                                                else
                                                    obj.IsCloseItem = false;
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.LineNumber.ToString()))
                                            {
                                                obj.LineNumber = item[CommonUtility.ImportReturnOrderItemsColumn.LineNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ControlNumber.ToString()))
                                            {
                                                obj.ControlNumber = item[CommonUtility.ImportReturnOrderItemsColumn.ControlNumber.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ItemComment.ToString()))
                                            {
                                                obj.ItemComment = item[CommonUtility.ImportReturnOrderItemsColumn.ItemComment.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.LineItemUDF1.ToString()))
                                            {
                                                obj.LineItemUDF1 = item[CommonUtility.ImportReturnOrderItemsColumn.LineItemUDF1.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.LineItemUDF2.ToString()))
                                            {
                                                obj.LineItemUDF2 = item[CommonUtility.ImportReturnOrderItemsColumn.LineItemUDF2.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.LineItemUDF3.ToString()))
                                            {
                                                obj.LineItemUDF3 = item[CommonUtility.ImportReturnOrderItemsColumn.LineItemUDF3.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.LineItemUDF4.ToString()))
                                            {
                                                obj.LineItemUDF4 = item[CommonUtility.ImportReturnOrderItemsColumn.LineItemUDF4.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.LineItemUDF5.ToString()))
                                            {
                                                obj.LineItemUDF5 = item[CommonUtility.ImportReturnOrderItemsColumn.LineItemUDF5.ToString()].ToString();
                                            }
                                            if (item.Table.Columns.Contains(CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderCost.ToString()))
                                            {
                                                double _OrderCostValue = 0;
                                                if (double.TryParse(item[CommonUtility.ImportReturnOrderItemsColumn.ReturnOrderCost.ToString()].ToString(), out _OrderCostValue))
                                                    obj.OrderCost = _OrderCostValue;
                                                else
                                                    obj.OrderCost = null;
                                            }
                                            lstImport.Add(obj);
                                        }
                                    }
                                    catch { }
                                }

                                Session["importedData"] = lstImport;
                            }
                            #endregion

                            #region Coomon BOM To Item
                            else if (Session["CurModule"].ToString() == ImportMastersDTO.TableName.CommonBOMToItem.ToString())
                            {
                                List<BOMItemMasterMain> lstImport = new List<BOMItemMasterMain>();
                                foreach (DataRow item in list)
                                {
                                    BOMItemMasterMain obj = new BOMItemMasterMain();
                                    try
                                    {
                                        //bool AllowInsert = true;
                                        //  AllowInsert = lstImport.Where(x => x.ItemNumber == item[CommonUtility.ImportItemColumn.ItemNumber.ToString()].ToString()).ToList().Count > 0 ? false : true;

                                        //if (AllowInsert == true)

                                        // obj.ID = Convert.ToInt32(item[CommonUtility.ImportItemColumn.ID.ToString()].ToString());

                                        obj.ItemNumber = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.ItemNumber.ToString()]);
                                        obj.RoomName = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.RoomName.ToString()]);
                                        //Int64 ManufacturerID;
                                        //Int64.TryParse(item[CommonUtility.ImportItemColumn.ManufacturerID.ToString()].ToString(), out ManufacturerID);
                                        //obj.ManufacturerID = ManufacturerID;
                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.BlanketOrderNumber.ToString()))
                                        {
                                            obj.BlanketOrderNumber = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.BlanketOrderNumber.ToString()]);
                                        }
                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.DefaultReorderQuantity.ToString()))
                                        {
                                            double DefaultReorderQuantity;
                                            if (double.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.DefaultReorderQuantity.ToString()]), out DefaultReorderQuantity))
                                                obj.DefaultReorderQuantity = DefaultReorderQuantity;
                                            else
                                                obj.DefaultReorderQuantity = 0;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.DefaultPullQuantity.ToString()))
                                        {
                                            double DefaultPullQuantity;
                                            if (double.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.DefaultPullQuantity.ToString()]), out DefaultPullQuantity))
                                                obj.DefaultPullQuantity = DefaultPullQuantity;
                                            else
                                                obj.DefaultPullQuantity = 0;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.LeadTimeInDays.ToString()))
                                        {
                                            Int32 LeadTimeInDays;
                                            if (Int32.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.LeadTimeInDays.ToString()]), out LeadTimeInDays))
                                                obj.LeadTimeInDays = LeadTimeInDays;
                                            else
                                                obj.LeadTimeInDays = 0;

                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.Link1.ToString()))
                                        {
                                            obj.Link1 = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.Link1.ToString()]);
                                        }
                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.Link2.ToString()))
                                        {
                                            obj.Link2 = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.Link2.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.Taxable.ToString()))
                                        {
                                            Boolean Taxable = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.Taxable.ToString()]), out Taxable))
                                                obj.Taxable = Taxable;
                                            else
                                                obj.Taxable = false;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.Consignment.ToString()))
                                        {
                                                if (item[CommonUtility.CommonBOMTOItemColumn.Consignment.ToString()] == "")
                                                {
                                                    obj.IsBlankConsignment = true;
                                                }
                                                Boolean Consignment = false;
                                                if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.Consignment.ToString()]), out Consignment))
                                                    obj.Consignment = Consignment;
                                                else
                                                    obj.Consignment = false;
                                                
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.OnHandQuantity.ToString()))
                                        {
                                            double OnHandQuantity;
                                            if (double.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.OnHandQuantity.ToString()]), out OnHandQuantity))
                                            {
                                                //obj.DispOnHandQuantity = OnHandQuantity;
                                                obj.OnHandQuantity = OnHandQuantity;
                                            }
                                            else
                                            {
                                                //obj.DispOnHandQuantity = 0;
                                                obj.OnHandQuantity = 0;
                                            }
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.CriticalQuantity.ToString()))
                                        {
                                            double CriticalQuantity;
                                            if (double.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.CriticalQuantity.ToString()]), out CriticalQuantity))
                                                obj.CriticalQuantity = CriticalQuantity;
                                            else
                                                obj.CriticalQuantity = 0;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.MinimumQuantity.ToString()))
                                        {
                                            double MinimumQuantity;
                                            if (double.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.MinimumQuantity.ToString()]), out MinimumQuantity))
                                                obj.MinimumQuantity = MinimumQuantity;
                                            else
                                                obj.MinimumQuantity = 0;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.MaximumQuantity.ToString()))
                                        {
                                            double MaximumQuantity;
                                            if (double.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.MaximumQuantity.ToString()]), out MaximumQuantity))
                                                obj.MaximumQuantity = MaximumQuantity;
                                            else
                                                obj.MaximumQuantity = 0;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.WeightPerPiece.ToString()))
                                        {
                                            double WeightPerPiece;
                                            if (double.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.WeightPerPiece.ToString()]), out WeightPerPiece))
                                                obj.WeightPerPiece = WeightPerPiece;
                                            else
                                                obj.WeightPerPiece = 0;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.WeightPerPiece.ToString()))
                                        {
                                            double WeightPerPiece;
                                            if (double.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.WeightPerPiece.ToString()]), out WeightPerPiece))
                                                obj.WeightPerPiece = WeightPerPiece;
                                            else
                                                obj.WeightPerPiece = 0;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.ItemUniqueNumber.ToString()))
                                        {
                                            obj.ItemUniqueNumber = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.ItemUniqueNumber.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.IsTransfer.ToString()))
                                        {
                                            Boolean IsTransfer = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.IsTransfer.ToString()]), out IsTransfer))
                                                obj.IsTransfer = IsTransfer;
                                            else
                                                obj.IsTransfer = false;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.IsPurchase.ToString()))
                                        {
                                            Boolean IsPurchase = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.IsPurchase.ToString()]), out IsPurchase))
                                            {
                                                if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.IsTransfer.ToString()))
                                                {
                                                    if (Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.IsTransfer.ToString()]) == "" && Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.IsPurchase.ToString()]) == "")
                                                    {
                                                        obj.IsPurchase = true;
                                                    }
                                                    else
                                                    {
                                                        obj.IsPurchase = IsPurchase;
                                                    }

                                                }
                                                else if (Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.IsPurchase.ToString()]) == "")
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
                                                obj.IsPurchase = true;
                                            }
                                        }

                                        //Int64 DefaultLocation;
                                        //Int64.TryParse(item[CommonUtility.ImportItemColumn.DefaultLocation.ToString()].ToString(), out DefaultLocation);
                                        //obj.DefaultLocation = DefaultLocation;
                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.InventryLocation.ToString()))
                                        {
                                            obj.InventryLocation = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.InventryLocation.ToString()]);

                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.InventoryClassification.ToString()))
                                        {
                                            string InventoryClassification;
                                            InventoryClassification = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.InventoryClassification.ToString()]);
                                            obj.InventoryClassificationName = InventoryClassification;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.SerialNumberTracking.ToString()))
                                        {
                                            Boolean SerialNumberTracking = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.SerialNumberTracking.ToString()]), out SerialNumberTracking))
                                                obj.SerialNumberTracking = SerialNumberTracking;
                                            else
                                                obj.SerialNumberTracking = false;
                                        }


                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.LotNumberTracking.ToString()))
                                        {
                                            Boolean LotNumberTracking = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.LotNumberTracking.ToString()]), out LotNumberTracking))
                                                obj.LotNumberTracking = LotNumberTracking;
                                            else
                                                obj.LotNumberTracking = false;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.DateCodeTracking.ToString()))
                                        {
                                            Boolean DateCodeTracking = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.DateCodeTracking.ToString()]), out DateCodeTracking))
                                                obj.DateCodeTracking = DateCodeTracking;
                                            else
                                                obj.DateCodeTracking = false;
                                        }

                                        string ItemTypeName = string.Empty;
                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.ItemType.ToString()))
                                        {
                                            obj.ItemTypeName = item[CommonUtility.CommonBOMTOItemColumn.ItemType.ToString()].ToString();
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.ImagePath.ToString()))
                                        {
                                            obj.ImagePath = item[CommonUtility.CommonBOMTOItemColumn.ImagePath.ToString()].ToString();
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.UDF1.ToString()))
                                        {
                                            obj.UDF1 = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.UDF1.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.UDF2.ToString()))
                                        {
                                            obj.UDF2 = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.UDF2.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.UDF3.ToString()))
                                        {
                                            obj.UDF3 = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.UDF3.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.UDF4.ToString()))
                                        {
                                            obj.UDF4 = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.UDF4.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.UDF5.ToString()))
                                        {
                                            obj.UDF5 = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.UDF5.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.IsLotSerialExpiryCost.ToString()))
                                        {
                                            obj.IsLotSerialExpiryCost = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.IsLotSerialExpiryCost.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.ImportItemColumn.IsItemLevelMinMaxQtyRequired.ToString()))
                                        {
                                            Boolean ItemLevelMinMaxQtyRequired = true;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.IsItemLevelMinMaxQtyRequired.ToString()]), out ItemLevelMinMaxQtyRequired))
                                                obj.IsItemLevelMinMaxQtyRequired = ItemLevelMinMaxQtyRequired;
                                            else
                                                obj.IsItemLevelMinMaxQtyRequired = true;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.TrendingSetting.ToString()))
                                        {
                                            obj.TrendingSettingName = item[CommonUtility.CommonBOMTOItemColumn.TrendingSetting.ToString()].ToString();
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.EnforceDefaultPullQuantity.ToString()))
                                        {
                                            Boolean IsPullQtyScanOverride = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.EnforceDefaultPullQuantity.ToString()]), out IsPullQtyScanOverride))
                                                obj.PullQtyScanOverride = IsPullQtyScanOverride;
                                            else
                                                obj.PullQtyScanOverride = false;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.EnforceDefaultReorderQuantity.ToString()))
                                        {
                                            Boolean IsEnforceDefaultReorderQuantity = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.EnforceDefaultReorderQuantity.ToString()]), out IsEnforceDefaultReorderQuantity))
                                                obj.IsEnforceDefaultReorderQuantity = IsEnforceDefaultReorderQuantity;
                                            else
                                                obj.IsEnforceDefaultReorderQuantity = false;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.IsAutoInventoryClassification.ToString()))
                                        {
                                            Boolean AutoInventoryClassification = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.IsAutoInventoryClassification.ToString()]), out AutoInventoryClassification))
                                                obj.IsAutoInventoryClassification = AutoInventoryClassification;
                                            else
                                                obj.IsAutoInventoryClassification = false;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.IsBuildBreak.ToString()))
                                        {
                                            Boolean BuildBreak = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.IsBuildBreak.ToString()]), out BuildBreak))
                                                obj.IsBuildBreak = BuildBreak;
                                            else
                                                obj.IsBuildBreak = false;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.IsPackslipMandatoryAtReceive.ToString()))
                                        {
                                            Boolean IsPullQtyScanOverride = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.IsPackslipMandatoryAtReceive.ToString()]), out IsPullQtyScanOverride))
                                                obj.IsPackslipMandatoryAtReceive = IsPullQtyScanOverride;
                                            else
                                                obj.IsPackslipMandatoryAtReceive = false;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.ItemImageExternalURL.ToString()))
                                        {
                                            obj.ItemImageExternalURL = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.ItemImageExternalURL.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.ItemDocExternalURL.ToString()))
                                        {
                                            obj.ItemDocExternalURL = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.ItemDocExternalURL.ToString()]);
                                        }
                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.IsDeleted.ToString()))
                                        {
                                            Boolean IsPullQtyScanOverride = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.IsDeleted.ToString()]), out IsPullQtyScanOverride))
                                                obj.IsDeleted = IsPullQtyScanOverride;
                                            else
                                                obj.IsDeleted = false;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.ItemLink2ExternalURL.ToString()))
                                        {
                                            obj.ItemLink2ExternalURL = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.ItemLink2ExternalURL.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.IsActive.ToString()))
                                        {
                                            Boolean IsActive = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.IsActive.ToString()]), out IsActive))
                                                obj.IsActive = IsActive;
                                            else
                                                obj.IsActive = true;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.UDF6.ToString()))
                                        {
                                            obj.UDF6 = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.UDF6.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.UDF7.ToString()))
                                        {
                                            obj.UDF7 = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.UDF7.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.UDF8.ToString()))
                                        {
                                            obj.UDF8 = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.UDF8.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.UDF9.ToString()))
                                        {
                                            obj.UDF9 = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.UDF9.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.UDF10.ToString()))
                                        {
                                            obj.UDF10 = Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.UDF10.ToString()]);
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.IsAllowOrderCostuom.ToString()))
                                        {
                                            Boolean IsAllowOrderCostuom = false;
                                            if (Boolean.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.IsAllowOrderCostuom.ToString()]), out IsAllowOrderCostuom))
                                                obj.IsAllowOrderCostuom = IsAllowOrderCostuom;
                                            else
                                                obj.IsAllowOrderCostuom = false;
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.eLabelKey.ToString()))
                                        {
                                            obj.eLabelKey = item[CommonUtility.CommonBOMTOItemColumn.eLabelKey.ToString()].ToString();
                                        }

                                        if (item.Table.Columns.Contains(CommonUtility.CommonBOMTOItemColumn.POItemLineNumber.ToString()))
                                        {
                                            int POItemLineNumber;
                                            if (int.TryParse(Convert.ToString(item[CommonUtility.CommonBOMTOItemColumn.POItemLineNumber.ToString()]), out POItemLineNumber))
                                                obj.POItemLineNumber = POItemLineNumber;
                                            else
                                                obj.POItemLineNumber = null;
                                        }

                                        obj.Status = "N/A";
                                        obj.Reason = "N/A";
                                        //obj.IsLotSerialExpiryCost = false;
                                        lstImport.Add(obj);

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
            catch (Exception ex)
            {
                Session["ErrorMessage"] = ex.Message;
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
                    case "EditItemMaster":
                        retval = "itemnumber,manufacturer,manufacturernumber,suppliername,supplierpartno,blanketordernumber,upc,unspsc,description,longdescription,categoryname,glaccount,uom,costuom,defaultreorderquantity,defaultpullquantity,cost,markup,sellprice,extendedcost,leadtimeindays,link1,link2,trend,taxable,consignment,stagedquantity,intransitquantity,onorderquantity,ontransferquantity,suggestedorderquantity,requisitionedquantity,averageusage,turns,onhandquantity,criticalquantity,minimumquantity,maximumquantity,weightperpiece,itemuniquenumber,istransfer,ispurchase,inventrylocation,inventoryclassification,serialnumbertracking,lotnumbertracking,datecodetracking,itemtype,imagepath,udf1,udf2,udf3,udf4,udf5,islotserialexpirycost,isitemlevelminmaxqtyrequired,trendingsetting,enforcedefaultpullquantity,enforcedefaultreorderquantity,isautoinventoryclassification,isbuildbreak,ispackslipmandatoryatreceive,itemimageexternalurl,itemdocexternalurl,isdeleted,itemlink2externalurl,isactive,udf6,udf7,udf8,udf9,udf10,peritemcost,outtransferquantity,isallowordercostuom,elabelkey,enrichedproductdata,enhanceddescription,poitemlinenumber";
                        break;
                    case "BOMItemMaster":
                        retval = "itemnumber,manufacturer,manufacturernumber,suppliername,supplierpartno,upc,unspsc,description,longdescription,categoryname,glaccount,uom,leadtimeindays,taxable,consignment,itemuniquenumber,istransfer,ispurchase,inventoryclassification,serialnumbertracking,lotnumbertracking,datecodetracking,itemtype,criticalquantity,minimumquantity,maximumquantity,cost,markup,sellprice,costuom,defaultreorderquantity,defaultpullquantity,link1,link2,udf1,udf2,udf3,udf4,udf5,isitemlevelminmaxqtyrequired,enforcedefaultpullquantity,enforcedefaultreorderquantity,itemimageexternalurl,itemdocexternalurl,itemlink2externalurl,isactive,imagepath,enrichedproductdata,enhanceddescription";
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
                        retval = "itemnumber,locationname,customerownedquantity,consignedquantity,serialnumber,lotnumber,expirationdate,itemdescription,udf1,udf2,udf3,udf4,udf5";//,receiveddate
                        break;
                    case "ItemLocationeVMISetup":
                        retval = "itemnumber,locationname,minimumquantity,maximumquantity,criticalquantity,sensorid,sensorport,isdefault,isdeleted,isstaginglocation,isenforcedefaultpullquantity,defaultpullquantity,isenforcedefaultreorderquantity,defaultreorderquantity,binudf1,binudf2,binudf3,binudf4,binudf5";
                        break;
                    case "ManufacturerMaster":
                        retval = "manufacturer,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "MeasurementTermMaster":
                        retval = "measurementterm,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "SupplierMaster":
                        //retval = "suppliername,description,receiverid,address,city,state,zipcode,country,contact,phone,fax,email,isemailpoinbody,isemailpoinpdf,isemailpoincsv,isemailpoinx12,udf1,udf2,udf3,udf4,udf5";
                        retval = "suppliername,suppliercolor,description,branchnumber,maximumordersize,address,city,state,zipcode,country,contact,phone,fax,email,issendtovendor,isvendorreturnasn,issupplierreceiveskitcomponents,ordernumbertypeblank,ordernumbertypefixed,ordernumbertypeblanketordernumber,ordernumbertypeincrementingordernumber,ordernumbertypeincrementingbyday,ordernumbertypedateincrementing,ordernumbertypedate,udf1,udf2,udf3,udf4,udf5,udf6,udf7,udf8,udf9,udf10,accountnumber,accountname,accountaddress,accountcity,accountstate,accountzip,accountcountry,accountshiptoid,accountisdefault,blanketponumber,startdate,enddate,maxlimit,donotexceed,PullPurchaseNumberFixed,PullPurchaseNumberBlanketOrder,PullPurchaseNumberDateIncrementing,PullPurchaseNumberDate,PullPurchaseNumberType,LastPullPurchaseNumberUsed,isblanketdeleted,supplierimage,imageexternalurl,maxlimitqty,donotexceedqty";
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
                        retval = "toolname,serial,description,toolcategory,cost,quantity,location,udf1,udf2,udf3,udf4,udf5,isgroupofitems,Technician,CheckOutQuantity,CheckInQuantity,checkoutudf1,checkoutudf2,checkoutudf3,checkoutudf4,checkoutudf5,imagepath,toolimageexternalurl,serialnumbertracking,noofpastmntstoconsider,maintenanceduenoticedays";
                        break;
                    case "AssetToolSchedulerMapping":
                        retval = "schedulefor,schedulername,assetname,toolname,serial,udf1,udf2,udf3,udf4,udf5";
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
                        retval = "itemnumber,locationname,customerownedquantity,consignedquantity,serialnumber,lotnumber,expirationdate,projectspend,itemdescription,udf1,udf2,udf3,udf4,udf5";//
                        break;
                    case "WorkOrder":
                        retval = "woname,releasenumber,wostatus,technician,customer,udf1,udf2,udf3,udf4,udf5,wotype,description,suppliername,asset,odometer,SupplierAccount"; 
                        break;
                    case "PullMaster":
                        retval = "ItemNumber,PullQuantity,Location,UDF1,UDF2,UDF3,UDF4,UDF5,ProjectSpendName,PullOrderNumber,WorkOrder,Asset,ActionType,ItemSellPrice";
                        break;
                    case "PullImportWithLotSerial":
                        retval = "ItemNumber,PullQuantity,Location,UDF1,UDF2,UDF3,UDF4,UDF5,ProjectSpendName,PullOrderNumber,WorkOrder,Asset,LotNumber,SerialNumber,ExpirationDate,ActionType,ItemSellPrice";
                        break;
                    case "ItemLocationChange":
                        retval = "itemnumber,oldlocationname,newlocationname";
                        break;
                    case "PullMasterWithSameQty":
                        retval = "ItemNumber,PullQuantity,BinNumber,UDF1,UDF2,UDF3,UDF4,UDF5,ProjectSpendName,PullOrderNumber,WorkOrder,Asset,ActionType,Created,ItemCost,CostUOMValue";
                        break;
                    case "AssetToolScheduler":
                        retval = "schedulefor,schedulername,description,schedulertype,timebasedunit,timebasedfrequency,checkouts,operationalhours,mileage,udf1,udf2,udf3,udf4,udf5,itemnumber,quantity,isdeleted";
                        break;
                    case "PastMaintenanceDue":
                        retval = "id,schedulefor,maintenancedate,assetname,toolname,serial,schedulername,itemnumber,itemcost,quantity";
                        break;
                    case "ToolCheckInOutHistory":
                        if (eTurnsWeb.Helper.SessionHelper.AllowToolOrdering)
                        {
                            retval = "toolname,serial,location,techniciancode,quantity,operation,checkoutudf1,checkoutudf2,checkoutudf3,checkoutudf4,checkoutudf5";
                        }
                        else
                        {
                            retval = "serial,techniciancode,quantity,operation,checkoutudf1,checkoutudf2,checkoutudf3,checkoutudf4,checkoutudf5";
                        }
                        break;
                    case "ToolAdjustmentCount":
                        retval = "toolname,locationname,quantity,serialnumber,udf1,udf2,udf3,udf4,udf5";
                        break;
                    case "ToolCertificationImages":
                        retval = "toolname,serial,imagename";
                        break;
                    case "OrderMaster":
                        retval = "Supplier,OrderNumber,RequiredDate,OrderStatus,StagingName,OrderComment,CustomerName,PackSlipNumber,ShippingTrackNumber,OrderUDF1,OrderUDF2,OrderUDF3,OrderUDF4,OrderUDF5,ShipVia,OrderType,ShippingVendor,SupplierAccount,ItemNumber,Bin,RequestedQty,ReceivedQty,ASNNumber,ApprovedQty,InTransitQty,IsCloseItem,LineNumber,ControlNumber,ItemComment,LineItemUDF1,LineItemUDF2,LineItemUDF3,LineItemUDF4,LineItemUDF5,OrderCost,SalesOrder";
                        break;
                    case "MoveMaterial":
                        retval = "itemnumber,sourcebin,destinationbin,movetype,quantity,destinationstagingheader";
                        break;
                    case "EnterpriseQuickList":
                        retval = "name,qldetailnumber,quantity";
                        break;
                    case "Requisition":
                        //retval = "RequisitionNumber,Workorder,RequiredDate,RequisitionStatus,CustomerName,ReleaseNumber,ProjectSpend,Description,StagingName,Supplier,SupplierAccount,BillingAccount,Technician,RequisitionUDF1,RequisitionUDF2,RequisitionUDF3,RequisitionUDF4,RequisitionUDF5,ItemNumber,Tool,ToolSerial,Bin,QuantityRequisitioned,QuantityApproved,QuantityPulled,LineItemRequiredDate,LineItemProjectSpend,LineItemSupplierAccount,PullOrderNumber,LineItemTechnician,PullUDF1,PullUDF2,PullUDF3,PullUDF4,PullUDF5,ToolCheckoutUDF1,ToolCheckoutUDF2,ToolCheckoutUDF3,ToolCheckoutUDF4,ToolCheckoutUDF5";
                        retval = "RequisitionNumber,Workorder,RequiredDate,RequisitionStatus,CustomerName,ProjectSpend,Description,StagingName,Supplier,SupplierAccount,BillingAccount,Technician,RequisitionUDF1,RequisitionUDF2,RequisitionUDF3,RequisitionUDF4,RequisitionUDF5,ItemNumber,Tool,ToolSerial,Bin,QuantityRequisitioned,QuantityApproved,QuantityPulled,LineItemRequiredDate,LineItemProjectSpend,LineItemSupplierAccount,PullOrderNumber,LineItemTechnician,PullUDF1,PullUDF2,PullUDF3,PullUDF4,PullUDF5,ToolCheckoutUDF1,ToolCheckoutUDF2,ToolCheckoutUDF3,ToolCheckoutUDF4,ToolCheckoutUDF5";
                        break;
                    case "QuoteMaster":
                        retval = "SupplierName,QuoteNumber,RequiredDate,QuoteStatus,QuoteComment,QuoteUDF1,QuoteUDF2,QuoteUDF3,QuoteUDF4,QuoteUDF5,ItemNumber,Bin,RequestedQty,ASNNumber,ApprovedQty,InTransitQty,IsCloseItem,LineNumber,ControlNumber,ItemComment,LineItemUDF1,LineItemUDF2,LineItemUDF3,LineItemUDF4,LineItemUDF5,QuoteCost";
                        break;
                    case "SupplierCatalog":
                        //retval = "ItemNumber,Description,SellPrice,PackingQuantity,ManufacturerPartNumber,ImagePath,UPC,SupplierPartNumber,SupplierName,ManufacturerName,ConcatedColumnText,UOM,CostUOM,Cost";
                        retval = "ItemNumber,Description,SellPrice,ManufacturerPartNumber,ImagePath,UPC,SupplierPartNumber,SupplierName,ManufacturerName,UOM,CostUOM,Cost";
                        break;
                    case "ReturnOrders":
                        retval = "Supplier,ReturnOrderNumber,ReturnDate,ReturnOrderStatus,StagingName,ReturnOrderComment,PackSlipNumber,ShippingTrackNumber,ReturnOrderUDF1,ReturnOrderUDF2,ReturnOrderUDF3,ReturnOrderUDF4,ReturnOrderUDF5,ShipVia,OrderType,ShippingVendor,SupplierAccount,ItemNumber,Bin,RequestedReturnedQty,ReceivedReturnedQty,ASNNumber,ApprovedQty,InTransitQty,IsCloseItem,LineNumber,ControlNumber,ItemComment,LineItemUDF1,LineItemUDF2,LineItemUDF3,LineItemUDF4,LineItemUDF5,ReturnOrderCost";
                        break;
                    case "CommonBOMToItem":
                        retval = "roomname,itemnumber,blanketordernumber,defaultreorderquantity,defaultpullquantity,leadtimeindays,link1,link2,taxable,consignment,onhandquantity,criticalquantity,minimumquantity,maximumquantity,weightperpiece,itemuniquenumber,istransfer,ispurchase,inventrylocation,inventoryclassification,serialnumbertracking,lotnumbertracking,datecodetracking,itemtype,imagepath,udf1,udf2,udf3,udf4,udf5,islotserialexpirycost,isitemlevelminmaxqtyrequired,trendingsetting,enforcedefaultpullquantity,enforcedefaultreorderquantity,isautoinventoryclassification,isbuildbreak,ispackslipmandatoryatreceive,itemimageexternalurl,itemdocexternalurl,isdeleted,itemlink2externalurl,isactive,udf6,udf7,udf8,udf9,udf10,isallowordercostuom,elabelkey,poitemlinenumber";
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
                    case "EditItemMaster":
                        retval = "itemtype,defaultpullquantity,defaultreorderquantity,itemnumber,suppliername,supplierpartno,uom,costuom,inventrylocation,isitemlevelminmaxqtyrequired";
                        break;
                    case "BOMItemMaster":
                        retval = "serialnumbertracking,lotnumbertracking,datecodetracking,itemtype,consignment,taxable,itemnumber,suppliername,uom";
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
                    case "AssetToolSchedulerMapping":
                        retval = "schedulefor,schedulername";
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
                    case "ItemLocationChange":
                        retval = "itemnumber,oldlocationname,newlocationname";
                        break;
                    case "PullMasterWithSameQty":
                        retval = "ItemNumber,PullQuantity,ActionType";
                        break;
                    case "AssetToolScheduler":
                        retval = "schedulefor,schedulername";
                        break;
                    case "PastMaintenanceDue":
                        retval = "schedulefor,maintenancedate,schedulername";
                        break;
                    case "ToolCheckInOutHistory":
                        if (SessionHelper.AllowToolOrdering)
                        {
                            retval = "toolname,serial,techniciancode,quantity,operation";
                        }
                        else
                        {
                            retval = "serial,techniciancode,quantity,operation";
                        }
                        break;
                    case "ToolAdjustmentCount":
                        retval = "toolname,locationname";
                        break;
                    case "ToolCertificationImages":
                        retval = "toolname,serial,imagename";
                        break;
                    case "OrderMaster":
                        retval = "Supplier,OrderNumber,RequiredDate";
                        break;
                    case "MoveMaterial":
                        retval = "itemnumber,sourcebin,destinationbin,movetype,quantity,destinationstagingheader";
                        break;
                    case "EnterpriseQuickList":
                        retval = "name,qldetailnumber,quantity";
                        break;
                    case "Requisition":
                        retval = "RequisitionNumber,RequiredDate,RequisitionStatus";
                        break;
                    case "QuoteMaster":
                        retval = "SupplierName,QuoteNumber,RequiredDate";
                        break;
                    case "SupplierCatalog":
                        retval = "ItemNumber,SupplierName";
                        break;
                    case "ReturnOrders":
                        retval = "Supplier,ReturnOrderNumber,ReturnDate";
                        break;
                    case "CommonBOMToItem":
                        retval = "RoomName,itemnumber,InventryLocation";
                        break;
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
            #region Edit Item Master
            else if (ImportMastersDTO.TableName.EditItemMaster.ToString() == Session["CurModule"].ToString())
            {
                List<BOMItemMasterMain> lst = new List<BOMItemMasterMain>();
                lst = (List<BOMItemMasterMain>)Session["importedData"];

                CurrentItemList = lst;
                ViewBag.TotalRecordCount = CurrentItemList.Count;
                TotalRecordCount = CurrentItemList.Count;

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
                List<LocationMasterMain> lst = new List<LocationMasterMain>();
                lst = (List<LocationMasterMain>)Session["importedData"];

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

            #region Asset Tool Scheduler Mapping
            if (ImportMastersDTO.TableName.AssetToolSchedulerMapping.ToString() == Session["CurModule"].ToString())
            {
                List<AssetToolSchedulerMapping> lst = new List<AssetToolSchedulerMapping>();
                lst = (List<AssetToolSchedulerMapping>)Session["importedData"];

                CurrentAssetToolSchedulerMappingList = lst;
                ViewBag.TotalRecordCount = CurrentAssetToolSchedulerMappingList.Count;
                TotalRecordCount = CurrentAssetToolSchedulerMappingList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = CurrentAssetToolSchedulerMappingList
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
            #region Item Manufacturer
            else if (ImportMastersDTO.TableName.ItemManufacturerDetails.ToString() == Session["CurModule"].ToString())
            {
                List<ItemManufacturer> lst = new List<ItemManufacturer>();
                lst = (List<ItemManufacturer>)Session["importedData"];

                CurrentItemManufacturerList = lst;
                ViewBag.TotalRecordCount = CurrentItemManufacturerList.Count;
                TotalRecordCount = CurrentItemManufacturerList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentItemManufacturerList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Item SupplierDetails
            else if (ImportMastersDTO.TableName.ItemSupplierDetails.ToString() == Session["CurModule"].ToString())
            {
                List<ItemSupplier> lst = new List<ItemSupplier>();
                lst = (List<ItemSupplier>)Session["importedData"];

                CurrentItemSupplierList = lst;
                ViewBag.TotalRecordCount = CurrentItemSupplierList.Count;
                TotalRecordCount = CurrentItemSupplierList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentItemSupplierList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Barcode Association
            else if (ImportMastersDTO.TableName.BarcodeMaster.ToString() == Session["CurModule"].ToString())
            {
                List<ImportBarcodeMaster> lst = new List<ImportBarcodeMaster>();
                lst = (List<ImportBarcodeMaster>)Session["importedData"];

                CurrentBarcodeList = lst;
                ViewBag.TotalRecordCount = CurrentBarcodeList.Count;
                TotalRecordCount = CurrentBarcodeList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentBarcodeList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region UDF
            else if (ImportMastersDTO.TableName.UDF.ToString() == Session["CurModule"].ToString())
            {
                List<UDFMasterMain> lst = new List<UDFMasterMain>();
                lst = (List<UDFMasterMain>)Session["importedData"];

                CurrentUDFList = lst;
                ViewBag.TotalRecordCount = CurrentUDFList.Count;
                TotalRecordCount = CurrentUDFList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentUDFList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region Project Master
            else if (ImportMastersDTO.TableName.ProjectMaster.ToString() == Session["CurModule"].ToString())
            {
                List<ProjectMasterMain> lst = new List<ProjectMasterMain>();
                lst = (List<ProjectMasterMain>)Session["importedData"];

                CurrentProjectMasterList = lst;
                ViewBag.TotalRecordCount = CurrentProjectMasterList.Count;
                TotalRecordCount = CurrentProjectMasterList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentProjectMasterList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion


            #region ItemLocationQty
            if (ImportMastersDTO.TableName.ItemLocationQty.ToString() == Session["CurModule"].ToString())
            {
                List<InventoryLocationMain> lst = new List<InventoryLocationMain>();
                lst = (List<InventoryLocationMain>)Session["importedData"];

                CurrentItemLocationQtyList = lst;
                ViewBag.TotalRecordCount = CurrentItemLocationQtyList.Count;
                TotalRecordCount = CurrentItemLocationQtyList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentItemLocationQtyList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region Manual Count
            if (ImportMastersDTO.TableName.ManualCount.ToString() == Session["CurModule"].ToString())
            {
                List<InventoryLocationMain> lst = new List<InventoryLocationMain>();
                lst = (List<InventoryLocationMain>)Session["importedData"];

                CurrentManualCountList = lst;
                ViewBag.TotalRecordCount = CurrentManualCountList.Count;
                TotalRecordCount = CurrentManualCountList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentManualCountList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region Workorder
            if (ImportMastersDTO.TableName.WorkOrder.ToString() == Session["CurModule"].ToString())
            {
                List<WorkOrderMain> lst = new List<WorkOrderMain>();
                lst = (List<WorkOrderMain>)Session["importedData"];

                CurrentWorkOrderList = lst;
                ViewBag.TotalRecordCount = CurrentWorkOrderList.Count;
                TotalRecordCount = CurrentWorkOrderList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentWorkOrderList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region PullImport
            if (ImportMastersDTO.TableName.PullMaster.ToString() == Session["CurModule"].ToString())
            {
                List<PullImport> lst = new List<PullImport>();
                lst = (List<PullImport>)Session["importedData"];

                CurrentPullImportList = lst;
                ViewBag.TotalRecordCount = CurrentPullImportList.Count;
                TotalRecordCount = CurrentPullImportList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentPullImportList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region PullImportWithLotSerial
            if (ImportMastersDTO.TableName.PullImportWithLotSerial.ToString() == Session["CurModule"].ToString())
            {
                List<PullImportWithLotSerial> lst = new List<PullImportWithLotSerial>();
                lst = (List<PullImportWithLotSerial>)Session["importedData"];

                CurrentPullImportWithLotSerialList = lst;
                ViewBag.TotalRecordCount = CurrentPullImportWithLotSerialList.Count;
                TotalRecordCount = CurrentPullImportWithLotSerialList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentPullImportWithLotSerialList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region BinMaster
            if (ImportMastersDTO.TableName.ItemLocationChange.ToString() == Session["CurModule"].ToString())
            {
                List<ItemLocationChangeImport> lst = new List<ItemLocationChangeImport>();
                lst = (List<ItemLocationChangeImport>)Session["importedData"];

                CurrentLocationChangeList = lst;
                ViewBag.TotalRecordCount = CurrentLocationChangeList.Count;
                TotalRecordCount = CurrentLocationChangeList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentLocationChangeList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region PullImportWithSameQty
            if (ImportMastersDTO.TableName.PullMasterWithSameQty.ToString() == Session["CurModule"].ToString())
            {
                List<PullImportWithSameQty> lst = new List<PullImportWithSameQty>();
                lst = (List<PullImportWithSameQty>)Session["importedData"];

                CurrentPullImportWitSameQtyList = lst;
                ViewBag.TotalRecordCount = CurrentPullImportWitSameQtyList.Count;
                TotalRecordCount = CurrentPullImportWitSameQtyList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentPullImportWitSameQtyList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region Asset Tool Scheduler
            if (ImportMastersDTO.TableName.AssetToolScheduler.ToString() == Session["CurModule"].ToString())
            {
                List<AssetToolScheduler> lst = new List<AssetToolScheduler>();
                lst = (List<AssetToolScheduler>)Session["importedData"];

                CurrentAssetToolSchedulerList = lst;
                ViewBag.TotalRecordCount = CurrentAssetToolSchedulerList.Count;
                TotalRecordCount = CurrentAssetToolSchedulerList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = CurrentAssetToolSchedulerList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region PastMaintenanceDue
            if (ImportMastersDTO.TableName.PastMaintenanceDue.ToString() == Session["CurModule"].ToString())
            {
                List<PastMaintenanceDueImport> lst = new List<PastMaintenanceDueImport>();
                lst = (List<PastMaintenanceDueImport>)Session["importedData"];

                CurrentPastMaintenanceDueImportList = lst;
                ViewBag.TotalRecordCount = CurrentPastMaintenanceDueImportList.Count;
                TotalRecordCount = CurrentPastMaintenanceDueImportList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentPastMaintenanceDueImportList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region ToolCheckInCheckOut

            else if (ImportMastersDTO.TableName.ToolCheckInOutHistory.ToString() == Session["CurModule"].ToString())
            {
                List<ToolCheckInCheckOut> lst = new List<ToolCheckInCheckOut>();
                lst = (List<ToolCheckInCheckOut>)Session["importedData"];

                CurrentToolCheckInCheckOut = lst;
                ViewBag.TotalRecordCount = CurrentToolCheckInCheckOut.Count;
                TotalRecordCount = CurrentToolCheckInCheckOut.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = CurrentToolCheckInCheckOut
                }, JsonRequestBehavior.AllowGet);
            }

            #endregion


            #region Tool Adjustment Count
            if (ImportMastersDTO.TableName.ToolAdjustmentCount.ToString() == Session["CurModule"].ToString())
            {
                List<ToolAssetQuantityMain> lst = new List<ToolAssetQuantityMain>();
                lst = (List<ToolAssetQuantityMain>)Session["importedData"];

                CurrentToolAdjustmentCount = lst;
                ViewBag.TotalRecordCount = CurrentToolAdjustmentCount.Count;
                TotalRecordCount = CurrentToolAdjustmentCount.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                    aaData = CurrentToolAdjustmentCount
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region Tool Certification Images Import

            if (ImportMastersDTO.TableName.ToolCertificationImages.ToString() == Session["CurModule"].ToString())
            {
                List<ToolImageImport> lst = new List<ToolImageImport>();
                lst = (List<ToolImageImport>)Session["importedData"];

                CurrentToolImageImport = lst;
                ViewBag.TotalRecordCount = CurrentToolImageImport.Count;
                TotalRecordCount = CurrentToolImageImport.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = CurrentToolImageImport
                }, JsonRequestBehavior.AllowGet);
            }

            #endregion
            #region Order Master 
            else if (ImportMastersDTO.TableName.OrderMaster.ToString() == Session["CurModule"].ToString())
            {
                List<OrderMasterItemsMain> lst = new List<OrderMasterItemsMain>();
                lst = (List<OrderMasterItemsMain>)Session["importedData"];


                CurrentOrderMasterList = lst;
                ViewBag.TotalRecordCount = CurrentOrderMasterList.Count;
                TotalRecordCount = CurrentOrderMasterList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = CurrentOrderMasterList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Move Material
            else if (ImportMastersDTO.TableName.MoveMaterial.ToString() == Session["CurModule"].ToString())
            {
                List<MoveMaterial> lst = new List<MoveMaterial>();
                lst = (List<MoveMaterial>)Session["importedData"];
                CurrentMoveMaterialList = lst;
                ViewBag.TotalRecordCount = CurrentMoveMaterialList.Count;
                TotalRecordCount = CurrentMoveMaterialList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = CurrentMoveMaterialList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Enterprise Quick List
            else if (ImportMastersDTO.TableName.EnterpriseQuickList.ToString() == Session["CurModule"].ToString())
            {
                List<EnterpriseQLImport> lst = new List<EnterpriseQLImport>();
                lst = (List<EnterpriseQLImport>)Session["importedData"];
                CurrentEnterpriseQuickList = lst;
                ViewBag.TotalRecordCount = CurrentEnterpriseQuickList.Count;
                TotalRecordCount = CurrentEnterpriseQuickList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = CurrentEnterpriseQuickList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region Requisition
            else if (ImportMastersDTO.TableName.Requisition.ToString() == Session["CurModule"].ToString())
            {
                List<RequisitionImport> lst = new List<RequisitionImport>();
                lst = (List<RequisitionImport>)Session["importedData"];
                CurrentRequisitionList = lst;
                ViewBag.TotalRecordCount = CurrentRequisitionList.Count;
                TotalRecordCount = CurrentRequisitionList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = CurrentRequisitionList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region Quote Master 
            else if (ImportMastersDTO.TableName.QuoteMaster.ToString() == Session["CurModule"].ToString())
            {
                List<QuoteMasterItemsMain> lst = new List<QuoteMasterItemsMain>();
                lst = (List<QuoteMasterItemsMain>)Session["importedData"];


                CurrentQuoteMasterList = lst;
                ViewBag.TotalRecordCount = CurrentQuoteMasterList.Count;
                TotalRecordCount = CurrentQuoteMasterList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = CurrentQuoteMasterList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region Supplier Catalog
            else if (ImportMastersDTO.TableName.SupplierCatalog.ToString() == Session["CurModule"].ToString())
            {
                List<SupplierCatalogImport> lst = new List<SupplierCatalogImport>();
                lst = (List<SupplierCatalogImport>)Session["importedData"];
                CurrentSupplierCatalogList = lst;
                ViewBag.TotalRecordCount = CurrentSupplierCatalogList.Count;
                TotalRecordCount = CurrentSupplierCatalogList.Count;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = CurrentSupplierCatalogList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region Order Master 
            else if (ImportMastersDTO.TableName.ReturnOrders.ToString() == Session["CurModule"].ToString())
            {
                List<OrderMasterItemsMain> lst = new List<OrderMasterItemsMain>();
                lst = (List<OrderMasterItemsMain>)Session["importedData"];


                CurrentOrderMasterList = lst;
                ViewBag.TotalRecordCount = CurrentOrderMasterList.Count;
                TotalRecordCount = CurrentOrderMasterList.Count;


                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = CurrentOrderMasterList
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region Common BOM Item to Master
            else if (ImportMastersDTO.TableName.CommonBOMToItem.ToString() == Session["CurModule"].ToString())
            {
                List<BOMItemMasterMain> lst = new List<BOMItemMasterMain>();
                lst = (List<BOMItemMasterMain>)Session["importedData"];

                CurrentItemList = lst;
                ViewBag.TotalRecordCount = CurrentItemList.Count;
                TotalRecordCount = CurrentItemList.Count;

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
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = ""
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //public bool IsValidEmail(string emailaddress)
        //{
        //    try
        //    {
        //        if (emailaddress != "")
        //        {
        //            MailAddress m = new MailAddress(emailaddress);
        //        }
        //        return true;
        //    }
        //    catch (FormatException)
        //    {
        //        return false;
        //    }
        //}

        private static Int64 GetIDs(ImportMastersDTO.TableName TableName, string strVal, long longID = 0, long RoomID = 0)
        {
            return ImportMultiRoomBAL.GetIDs(TableName, strVal, longID, RoomID);
            //Int64 returnID = 0;
            //CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            //if (string.IsNullOrEmpty(strVal)) return returnID;

            //#region Get Manufacture ID
            //if (TableName == ImportMastersDTO.TableName.ManufacturerMaster)
            //{
            //    ManufacturerMasterDAL objDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            //    string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.ManufacturerMaster.ToString(), "Manufacturer", SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (strOK == "duplicate")
            //    {
            //        ManufacturerMasterDTO obj = null;
            //        obj = objDAL.GetManufacturerByNameNormal(strVal.ToLower(),SessionHelper.RoomID, SessionHelper.CompanyID, false);
            //        if (obj != null)
            //            returnID = obj.ID;
            //    }
            //    else
            //    {
            //        ManufacturerMasterDTO objDTO = new ManufacturerMasterDTO();
            //        objDTO.ID = 1;
            //        objDTO.Manufacturer = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
            //        objDTO.IsDeleted = false;
            //        objDTO.IsArchived = false;
            //        objDTO.Created = DateTimeUtility.DateTimeNow;
            //        objDTO.Updated = DateTimeUtility.DateTimeNow;
            //        objDTO.LastUpdatedBy = SessionHelper.UserID;
            //        objDTO.Room = SessionHelper.RoomID;
            //        objDTO.CompanyID = SessionHelper.CompanyID;
            //        objDTO.CreatedBy = SessionHelper.UserID;
            //        objDTO.GUID = Guid.NewGuid();
            //        objDTO.AddedFrom = "Web";
            //        objDTO.EditedFrom = "Web";
            //        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            //        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            //        returnID = objDAL.Insert(objDTO);
            //    }
            //}
            //#endregion

            //#region Get Supplier ID
            //else if (TableName == ImportMastersDTO.TableName.SupplierMaster)
            //{
            //    SupplierMasterDAL objDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

            //    //string strOK = objCDal.SupplierDuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.SupplierMaster.ToString(), "SupplierName", SessionHelper.RoomID, SessionHelper.CompanyID);
            //    string strOK = objDAL.SupplierDuplicateCheck(0, strVal, SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (strOK == "duplicate")
            //    {
            //        SupplierMasterDTO obj = null;
            //        obj = objDAL.GetSupplierByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, strVal);
            //        if (obj != null)
            //            returnID = obj.ID;
            //    }
            //    else
            //    {
            //        SupplierMasterDTO objDTO = new SupplierMasterDTO();
            //        objDTO.ID = 1;
            //        objDTO.SupplierName = strVal.Length > 255 ? strVal.Trim().Substring(0, 255) : strVal.Trim();
            //        objDTO.IsEmailPOInBody = false;
            //        objDTO.IsEmailPOInPDF = false;
            //        objDTO.IsEmailPOInCSV = false;
            //        objDTO.IsEmailPOInX12 = false;
            //        objDTO.IsDeleted = false;
            //        objDTO.IsArchived = false;
            //        objDTO.Created = DateTimeUtility.DateTimeNow;
            //        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            //        objDTO.LastUpdatedBy = SessionHelper.UserID;
            //        objDTO.Room = SessionHelper.RoomID;
            //        objDTO.CompanyID = SessionHelper.CompanyID;
            //        objDTO.CreatedBy = SessionHelper.UserID;
            //        objDTO.GUID = Guid.NewGuid();
            //        objDTO.AddedFrom = "Web";
            //        objDTO.EditedFrom = "Web";
            //        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            //        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            //        returnID = objDAL.Insert(objDTO);
            //    }
            //}
            //#endregion
            //#region Get SupplierBlanketPODetails ID
            //else if (TableName == ImportMastersDTO.TableName.SupplierBlanketPODetails)
            //{
            //    SupplierBlanketPODetailsDAL objDAL = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName);
            //    returnID = objDAL.SupplierBlanketPODetailsDuplicateCheck(0, strVal, longID, SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (returnID == 0)
            //    {
            //        SupplierBlanketPODetailsDTO oSupplierBlanketPODetailsDTO = new SupplierBlanketPODetailsDTO();
            //        oSupplierBlanketPODetailsDTO.SupplierID = longID;
            //        oSupplierBlanketPODetailsDTO.BlanketPO = strVal;
            //        oSupplierBlanketPODetailsDTO.GUID = Guid.NewGuid();
            //        oSupplierBlanketPODetailsDTO.Created = DateTime.Now;
            //        oSupplierBlanketPODetailsDTO.CreatedBy = SessionHelper.UserID;
            //        oSupplierBlanketPODetailsDTO.Updated = DateTime.Now;
            //        oSupplierBlanketPODetailsDTO.LastUpdatedBy = SessionHelper.UserID;
            //        oSupplierBlanketPODetailsDTO.CompanyID = SessionHelper.CompanyID;
            //        oSupplierBlanketPODetailsDTO.Room = SessionHelper.RoomID;
            //        oSupplierBlanketPODetailsDTO.IsArchived = false;
            //        oSupplierBlanketPODetailsDTO.IsDeleted = false;
            //        oSupplierBlanketPODetailsDTO.AddedFrom = "Web";
            //        oSupplierBlanketPODetailsDTO.EditedFrom = "Web";
            //        oSupplierBlanketPODetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            //        oSupplierBlanketPODetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            //        returnID = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName).Insert(oSupplierBlanketPODetailsDTO);
            //    }
            //}
            //#endregion
            //#region Get Category ID
            //else if (TableName == ImportMastersDTO.TableName.CategoryMaster)
            //{
            //    CategoryMasterDAL objDAL = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
            //    string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.CategoryMaster.ToString(), "Category", SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (strOK == "duplicate")
            //    {
            //        CategoryMasterDTO obj = null;
            //        //obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(c => (c.Category ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
            //        obj = objDAL.GetSingleCategoryByNameByRoomID(strVal, SessionHelper.RoomID, SessionHelper.CompanyID);
            //        if (obj != null)
            //            returnID = obj.ID;
            //    }
            //    else
            //    {
            //        CategoryMasterDTO objDTO = new CategoryMasterDTO();
            //        objDTO.ID = 1;
            //        objDTO.Category = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
            //        objDTO.IsDeleted = false;
            //        objDTO.IsArchived = false;
            //        objDTO.Created = DateTimeUtility.DateTimeNow;
            //        objDTO.Updated = DateTimeUtility.DateTimeNow;
            //        objDTO.LastUpdatedBy = SessionHelper.UserID;
            //        objDTO.Room = SessionHelper.RoomID;
            //        objDTO.CompanyID = SessionHelper.CompanyID;
            //        objDTO.CreatedBy = SessionHelper.UserID;
            //        objDTO.GUID = Guid.NewGuid();
            //        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            //        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            //        objDTO.AddedFrom = "Web";
            //        objDTO.EditedFrom = "Web";
            //        returnID = objDAL.Insert(objDTO);
            //    }
            //}
            //#endregion
            //#region Get GLAccount ID
            //else if (TableName == ImportMastersDTO.TableName.GLAccountMaster)
            //{
            //    GLAccountMasterDAL objDAL = new GLAccountMasterDAL(SessionHelper.EnterPriseDBName);
            //    string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.GLAccountMaster.ToString(), "GLAccount", SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (strOK == "duplicate")
            //    {
            //        GLAccountMasterDTO obj = null;
            //        //obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(c => (c.GLAccount ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
            //        obj = objDAL.GetGLAccountByName(SessionHelper.RoomID, SessionHelper.CompanyID, false, strVal);
            //        if (obj != null)
            //            returnID = obj.ID;
            //    }
            //    else
            //    {
            //        GLAccountMasterDTO objDTO = new GLAccountMasterDTO();
            //        objDTO.ID = 1;
            //        objDTO.GLAccount = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
            //        objDTO.IsDeleted = false;
            //        objDTO.IsArchived = false;
            //        objDTO.Created = DateTimeUtility.DateTimeNow;
            //        objDTO.Updated = DateTimeUtility.DateTimeNow;
            //        objDTO.LastUpdatedBy = SessionHelper.UserID;
            //        objDTO.Room = SessionHelper.RoomID;
            //        objDTO.CompanyID = SessionHelper.CompanyID;
            //        objDTO.CreatedBy = SessionHelper.UserID;
            //        objDTO.GUID = Guid.NewGuid();

            //        returnID = objDAL.Insert(objDTO);
            //    }
            //}
            //#endregion
            //#region Get Unit ID
            //else if (TableName == ImportMastersDTO.TableName.UnitMaster)
            //{
            //    UnitMasterDAL objDAL = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            //    string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.UnitMaster.ToString(), "Unit", SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (strOK == "duplicate")
            //    {
            //        UnitMasterDTO obj = null;
            //        obj = objDAL.GetUnitByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false,strVal);
            //        if (obj != null && obj.ID > 0)
            //            returnID = obj.ID;
            //    }
            //    else
            //    {
            //        UnitMasterDTO objDTO = new UnitMasterDTO();
            //        objDTO.ID = 1;
            //        objDTO.Unit = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
            //        objDTO.IsDeleted = false;
            //        objDTO.IsArchived = false;
            //        objDTO.Created = DateTimeUtility.DateTimeNow;
            //        objDTO.Updated = DateTimeUtility.DateTimeNow;
            //        objDTO.LastUpdatedBy = SessionHelper.UserID;
            //        objDTO.Room = SessionHelper.RoomID;
            //        objDTO.CompanyID = SessionHelper.CompanyID;
            //        objDTO.CreatedBy = SessionHelper.UserID;
            //        objDTO.GUID = Guid.NewGuid();
            //        objDTO.AddedFrom = "Web";
            //        objDTO.EditedFrom = "Web";
            //        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            //        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            //        returnID = objDAL.Insert(objDTO);
            //    }
            //}
            //#endregion           
            //#region Get LocationMaster ID
            //else if (TableName == ImportMastersDTO.TableName.LocationMaster)
            //{
            //    LocationMasterDAL objDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            //    string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.LocationMaster.ToString(), "Location", SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (strOK == "duplicate")
            //    {
            //        LocationMasterDTO obj = null;
            //        obj = objDAL.GetLocationByNamePlain(strVal ?? string.Empty,SessionHelper.RoomID, SessionHelper.CompanyID);
            //        if (obj != null)
            //            returnID = obj.ID;
            //    }
            //    else
            //    {
            //        LocationMasterDTO objDTO = new LocationMasterDTO();
            //        objDTO.ID = 1;
            //        objDTO.Location = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
            //        objDTO.IsDeleted = false;
            //        objDTO.IsArchived = false;
            //        objDTO.Created = DateTimeUtility.DateTimeNow;
            //        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            //        objDTO.LastUpdatedBy = SessionHelper.UserID;
            //        objDTO.Room = SessionHelper.RoomID;
            //        objDTO.CompanyID = SessionHelper.CompanyID;
            //        objDTO.CreatedBy = SessionHelper.UserID;
            //        objDTO.AddedFrom = "Web";
            //        objDTO.EditedFrom = "Web";
            //        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            //        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            //        objDTO.GUID = Guid.NewGuid();

            //        returnID = objDAL.Insert(objDTO);
            //    }
            //}
            //#endregion
            //#region Get ToolCategoryMaster ID
            //else if (TableName == ImportMastersDTO.TableName.ToolCategoryMaster)
            //{
            //    ToolCategoryMasterDAL objDAL = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            //    string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.ToolCategoryMaster.ToString(), "ToolCategory", SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (strOK == "duplicate")
            //    {
            //        ToolCategoryMasterDTO obj = null;
            //        obj = objDAL.GetToolCategoryByNamePlain(strVal,SessionHelper.RoomID, SessionHelper.CompanyID);
            //        if (obj != null)
            //            returnID = obj.ID;
            //    }
            //    else
            //    {
            //        ToolCategoryMasterDTO objDTO = new ToolCategoryMasterDTO();
            //        objDTO.ID = 1;
            //        objDTO.ToolCategory = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
            //        objDTO.IsDeleted = false;
            //        objDTO.IsArchived = false;
            //        objDTO.Created = DateTimeUtility.DateTimeNow;
            //        objDTO.Updated = DateTimeUtility.DateTimeNow;
            //        objDTO.LastUpdatedBy = SessionHelper.UserID;
            //        objDTO.Room = SessionHelper.RoomID;
            //        objDTO.CompanyID = SessionHelper.CompanyID;
            //        objDTO.CreatedBy = SessionHelper.UserID;
            //        objDTO.GUID = Guid.NewGuid();
            //        objDTO.AddedFrom = "Web";
            //        objDTO.EditedFrom = "Web";
            //        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            //        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            //        returnID = objDAL.Insert(objDTO);
            //    }
            //}
            //#endregion
            //#region Get InventoryClassificationMaster ID
            //else if (TableName == ImportMastersDTO.TableName.InventoryClassificationMaster)
            //{
            //    InventoryClassificationMasterDAL objInventoryClassificationMasterDAL = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
            //    string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.InventoryClassificationMaster.ToString(), "InventoryClassification", SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (strOK == "duplicate")
            //    {
            //        InventoryClassificationMasterDTO obj = null;
            //        obj = objInventoryClassificationMasterDAL.GetInventoryClassificationByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false,strVal);
            //        if (obj != null)
            //            returnID = obj.ID;
            //    }
            //    else
            //    {
            //        InventoryClassificationMasterDTO objDTO = new InventoryClassificationMasterDTO();
            //        objDTO.ID = 1;
            //        objDTO.InventoryClassification = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;

            //        objDTO.IsDeleted = false;
            //        objDTO.IsArchived = false;
            //        objDTO.Created = DateTimeUtility.DateTimeNow;
            //        objDTO.Updated = DateTimeUtility.DateTimeNow;
            //        objDTO.LastUpdatedBy = SessionHelper.UserID;
            //        objDTO.Room = SessionHelper.RoomID;
            //        objDTO.CompanyID = SessionHelper.CompanyID;
            //        objDTO.CreatedBy = SessionHelper.UserID;
            //        objDTO.GUID = Guid.NewGuid();
            //        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            //        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            //        objDTO.AddedFrom = "Web";
            //        objDTO.EditedFrom = "Web";
            //        returnID = objInventoryClassificationMasterDAL.Insert(objDTO);
            //    }
            //}
            //#endregion
            //#region Get CostUOM ID
            //if (TableName == ImportMastersDTO.TableName.CostUOMMaster)
            //{
            //    CostUOMMasterDAL objDAL = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
            //    string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.CostUOMMaster.ToString(), "CostUOM", SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (strOK == "duplicate")
            //    {
            //        CostUOMMasterDTO obj = null;
            //        //obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(c => (c.CostUOM ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
            //        obj = objDAL.GetCostUOMByName(strVal, SessionHelper.RoomID, SessionHelper.CompanyID);
            //        if (obj != null)
            //            returnID = obj.ID;
            //    }
            //    else
            //    {
            //        CostUOMMasterDTO objDTO = new CostUOMMasterDTO();
            //        objDTO.ID = 0;
            //        objDTO.CostUOM = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
            //        objDTO.CostUOMValue = 1;
            //        objDTO.GUID = Guid.NewGuid();
            //        objDTO.Created = DateTimeUtility.DateTimeNow;
            //        objDTO.Updated = DateTimeUtility.DateTimeNow;
            //        objDTO.CreatedBy = SessionHelper.UserID;
            //        objDTO.LastUpdatedBy = SessionHelper.UserID;
            //        objDTO.IsDeleted = false;
            //        objDTO.IsArchived = false;
            //        objDTO.CompanyID = SessionHelper.CompanyID;
            //        objDTO.Room = SessionHelper.RoomID;
            //        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            //        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            //        objDTO.AddedFrom = "Web";
            //        objDTO.EditedFrom = "Web";
            //        objDTO.isForBOM = false;
            //        returnID = objDAL.Insert(objDTO);
            //    }
            //}
            //#endregion

            //#region Get AssetCategoryMaster ID
            //else if (TableName == ImportMastersDTO.TableName.AssetCategoryMaster)
            //{
            //    AssetCategoryMasterDAL objDAL = new AssetCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            //    string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.AssetCategoryMaster.ToString(), "AssetCategory", SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (strOK == "duplicate")
            //    {
            //        AssetCategoryMasterDTO obj = null;
            //        obj = objDAL.GetAssetCategoryByName(strVal, SessionHelper.RoomID, SessionHelper.CompanyID);
            //        if (obj != null)
            //            returnID = obj.ID;
            //    }
            //    else
            //    {
            //        AssetCategoryMasterDTO objDTO = new AssetCategoryMasterDTO();
            //        objDTO.ID = 1;
            //        objDTO.AssetCategory = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
            //        objDTO.IsDeleted = false;
            //        objDTO.IsArchived = false;
            //        objDTO.Created = DateTimeUtility.DateTimeNow;
            //        objDTO.Updated = DateTimeUtility.DateTimeNow;
            //        objDTO.LastUpdatedBy = SessionHelper.UserID;
            //        objDTO.Room = SessionHelper.RoomID;
            //        objDTO.CompanyID = SessionHelper.CompanyID;
            //        objDTO.CreatedBy = SessionHelper.UserID;
            //        objDTO.GUID = Guid.NewGuid();
            //        objDTO.AddedFrom = "Web";
            //        objDTO.EditedFrom = "Web";
            //        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            //        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            //        returnID = objDAL.Insert(objDTO);
            //    }
            //}
            //#endregion
            //return returnID;
        }

        private CustomerMasterDTO GetCustomerMaster(ImportMastersDTO.TableName TableName, string strVal, long RoomID, long longID = 0)
        {
            #region Get Customer ID
            CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            CustomerMasterDTO objDTO = new CustomerMasterDTO();
            if (TableName == ImportMastersDTO.TableName.CustomerMaster)
            {
                CustomerMasterDAL objDAL = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.CustomerMaster.ToString(), "Customer", RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    CustomerMasterDTO obj = null;
                    obj = objDAL.GetCustomerByName(strVal, RoomID, SessionHelper.CompanyID);
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
                    objDTO.Room = RoomID;
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

        private static Int64 GetBOMIDs(ImportMastersDTO.TableName TableName, string strVal, long RoomID)
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
                    obj = objDAL.GetManufacturerByNameNormal(strVal.ToLower(), RoomID, SessionHelper.CompanyID, true);
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
                    objDTO.Room = RoomID;
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
                    obj = objDAL.GetSupplierByNamePlain(RoomID, SessionHelper.CompanyID, true, strVal);
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
                    objDTO.Room = RoomID;
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
                    //obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).Where(c => (c.Category ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    obj = objDAL.GetSingleCategoryByNameByCompanyIDBOM(strVal, SessionHelper.CompanyID);
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
                    objDTO.Room = RoomID;
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
                    //obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).Where(c => (c.GLAccount ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    obj = objDAL.GetGLAccountByName(RoomID, SessionHelper.CompanyID, false, strVal);
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
                    objDTO.Room = RoomID;
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
                    obj = objDAL.GetUnitByNamePlain(RoomID, SessionHelper.CompanyID, true, strVal);
                    if (obj != null && obj.ID > 0)
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
                    objDTO.Room = RoomID;
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
                    obj = objInventoryClassificationMasterDAL.GetInventoryClassificationByNamePlain(RoomID, SessionHelper.CompanyID, true, strVal);
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
                    objDTO.Room = RoomID;
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

            #region Get CostUOM ID
            if (TableName == ImportMastersDTO.TableName.CostUOMMaster)
            {
                CostUOMMasterDAL objDAL = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.CostUOMMaster.ToString(), "CostUOM", 0, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    CostUOMMasterDTO obj = null;
                    //obj = objDAL.GetCachedData(0, SessionHelper.CompanyID, false, false).Where(c => (c.CostUOM ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    obj = objDAL.GetBOMCostUOMByName(strVal, SessionHelper.CompanyID);
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
                    objDTO.Room = RoomID;
                    objDTO.isForBOM = true;
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            return returnID;
        }

        public Guid GetGUID(ImportMastersDTO.TableName TableName, string strVal, string optValue = "", QuickListType QLType = QuickListType.General, long RoomID = 0)
        {
            return ImportMultiRoomBAL.GetGUID(TableName, strVal, optValue, QLType, RoomID);

            //Guid returnID = Guid.NewGuid();
            //CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            //#region Get QuickList GUID
            //if (TableName == ImportMastersDTO.TableName.QuickListItems)
            //{
            //    QuickListDAL objDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
            //    string strOK = objCDal.DuplicateCheck(strVal, "add", 1, "QuickListMaster", "Name", SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (strOK == "duplicate")
            //    {
            //        QuickListMasterDTO obj = null;
            //        //obj = objDAL.GetQuickListMasterCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(c => c.Name == strVal).FirstOrDefault();
            //        obj = objDAL.GetQuickListMasterByName(strVal, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            //        if (obj != null)
            //        {
            //            //obj.Type = (int)QLType;
            //            objDAL.Edit(obj);
            //            returnID = obj.GUID;
            //        }
            //    }
            //    else
            //    {
            //        QuickListMasterDTO objDTO = new QuickListMasterDTO();
            //        objDTO.ID = 1;
            //        objDTO.Name = strVal;
            //        objDTO.Comment = optValue;
            //        objDTO.Type = (int)QLType;
            //        objDTO.IsDeleted = false;
            //        objDTO.IsArchived = false;
            //        objDTO.Created = DateTimeUtility.DateTimeNow;
            //        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            //        objDTO.LastUpdatedBy = SessionHelper.UserID;
            //        objDTO.Room = SessionHelper.RoomID;
            //        objDTO.CompanyID = SessionHelper.CompanyID;
            //        objDTO.CreatedBy = SessionHelper.UserID;
            //        objDTO.GUID = Guid.NewGuid();

            //        returnID = objDAL.InsertQuickList(objDTO);
            //    }
            //}
            //else if (TableName == ImportMastersDTO.TableName.ItemMaster)
            //{
            //    ItemMasterDAL objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            //    returnID = objDAL.GetGuidByItemNumber(strVal, SessionHelper.RoomID, SessionHelper.CompanyID) ?? Guid.Empty;
            //}
            //else if (TableName == ImportMastersDTO.TableName.AssetMaster)
            //{
            //    AssetMasterDAL objDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            //    AssetMasterDTO objDTO = objDAL.GetAssetsByName(strVal, SessionHelper.RoomID, SessionHelper.CompanyID).FirstOrDefault();
            //    if (objDTO != null)
            //    {
            //        returnID = objDTO.GUID;
            //    }
            //}
            //#endregion
            //return returnID;
        }

        private static void CheckUDF(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList, string objDTOUDF, string UDFs, long RoomID)
        {

            ImportMultiRoomBAL.CheckUDF(lst, CurrentOptionList, objDTOUDF, UDFs, RoomID);

            //if (objDTOUDF.Trim() != "")
            //{
            //    List<UDFOptionsCheckDTO> lstcount = new List<UDFOptionsCheckDTO>();
            //    lstcount = lst.Where(c => c.UDFColumnName == UDFs.ToString()).ToList();
            //    if (lstcount.Count > 0)
            //    {
            //        UDFOptionsCheckDTO objcheck = new UDFOptionsCheckDTO();
            //        objcheck = lst.Where(c => c.UDFColumnName == UDFs.ToString() && c.UDFOption == objDTOUDF && c.UDFID == lstcount[0].UDFID).FirstOrDefault();
            //        int objcheckCount = 0;
            //        if (CurrentOptionList != null)
            //        {
            //            objcheckCount = CurrentOptionList.Where(c => c.UDFOption == objDTOUDF && c.UDFID == lstcount[0].UDFID).Count();
            //            if ((objcheck == null && objcheckCount == 0))
            //            {
            //                UDFOptionsMain objoptionDTO = new UDFOptionsMain();
            //                objoptionDTO.ID = 0;
            //                objoptionDTO.Created = DateTimeUtility.DateTimeNow;
            //                objoptionDTO.CreatedBy = SessionHelper.UserID;
            //                objoptionDTO.Updated = DateTimeUtility.DateTimeNow;
            //                objoptionDTO.LastUpdatedBy = SessionHelper.UserID;
            //                objoptionDTO.IsDeleted = false;

            //                objoptionDTO.UDFOption = objDTOUDF;
            //                objoptionDTO.UDFID = lstcount[0].UDFID;
            //                objoptionDTO.GUID = Guid.NewGuid();

            //                objoptionDTO.CompanyID = SessionHelper.CompanyID;
            //                objoptionDTO.Room = SessionHelper.RoomID;

            //                CurrentOptionList.Add(objoptionDTO);
            //            }
            //        }


            //    }
            //}
        }

        public void InsertCheckOutUDf(string UDFOption, string UDFColumn)
        {
            string UdfTablename = ImportMastersDTO.TableName.ToolCheckInOutHistory.ToString();
            Int64 UDFID = 0;
            //List<UDFDTO> objUDFDTOList = new List<UDFDTO>();
            UDFDAL objUDFDAL = new UDFDAL(SessionHelper.EnterPriseDBName);
            //int TotalRecordCount = 0;
            var udf = objUDFDAL.GetUDFByUDFColumnNamePlain(UDFColumn, UdfTablename, SessionHelper.RoomID, SessionHelper.CompanyID);

            if (udf != null && udf.ID > 0)
            {
                UDFID = udf.ID;
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
                LocationMasterMain obj = CurrentLocationList.Where(c => c.ID == Convert.ToInt32(id)).SingleOrDefault();
                if (obj != null)
                {
                    if (columnName == CommonUtility.ImportLocationColumn.ID.ToString())
                        obj.ID = Convert.ToInt32(value);
                    else if (columnName == CommonUtility.ImportLocationColumn.Location.ToString())
                        obj.Location = value;
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
            CurrentLocationChangeList = null;
            CurrentToolCheckInCheckOut = null;
            CurrentToolAdjustmentCount = null;
            CurrentOrderMasterList = null;
            CurrentMoveMaterialList = null;
            CurrentEnterpriseQuickList = null;
            CurrentRequisitionList = null;
            CurrentSupplierCatalogList = null;
            Session["importedData"] = null;
            Session["importedRoomIds"] = null;
            return true;
        }
        [HttpPost]
        public JsonResult SetSelectedModule(int CurModule, bool isGridFill = false)
        {
            try
            {
                Session["CurModuleValue"] = CurModule;
                if (CurModule == ((int)SessionHelper.ModuleList.BinMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.BinMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.eVMISetup))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ItemLocationeVMISetup;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.CategoryMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.CategoryMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.CustomerMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.CustomerMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.FreightTypeMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.FreightTypeMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.GLAccountsMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.GLAccountMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.GXPRConsignedJobMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.GXPRConsigmentJobMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.JobTypeMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.JobTypeMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.ShipViaMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ShipViaMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.TechnicianMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.TechnicianMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.ManufacturerMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ManufacturerMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.MeasurementTermMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.MeasurementTermMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.UnitMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.UnitMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.SupplierMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.SupplierMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.ItemMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ItemMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.LocationMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.LocationMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.ToolCategory))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ToolCategoryMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.CostUOMMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.CostUOMMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.InventoryClassificationMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.InventoryClassificationMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.Assets))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.AssetMaster;
                }

                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolSchedulerMapping))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.AssetToolSchedulerMapping;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.ToolMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ToolMaster;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.QuickListPermission))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.QuickListItems;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.BinMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.InventoryLocation;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.BOMItemMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.BOMItemMaster;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.Kits))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.kitdetail;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.ItemManufacturer))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ItemManufacturerDetails;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.ItemSupplier))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ItemSupplierDetails;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.BarcodeMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.BarcodeMaster;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.UDF))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.UDF;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ProjectMaster;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.ItemLocationQty))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ItemLocationQty;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.ManualCount))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ManualCount;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.WorkOrder;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.PullImport))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.PullMaster;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.PullImportWithLotSerial))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.PullImportWithLotSerial;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.ItemLocationChange))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ItemLocationChange;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.PullImportWithSameQty))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.PullMasterWithSameQty;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolScheduler))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.AssetToolScheduler;
                }
                else if (CurModule == ((int)eTurnsWeb.Helper.SessionHelper.ModuleList.AssetMaintenance))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.PastMaintenanceDue;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.ToolCheckInCheckOut))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ToolCheckInOutHistory;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.ToolAdjustmentCount))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ToolAdjustmentCount;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.ToolCertificationImages))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ToolCertificationImages;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.Orders))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.OrderMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.MoveMaterial))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.MoveMaterial;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.EnterpriseQuickList))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.EnterpriseQuickList;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.Requisitions))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.Requisition;
                }
                if (CurModule == ((int)SessionHelper.ModuleList.Quote))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.QuoteMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.Suppliercatalog))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.SupplierCatalog;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.ReturnOrder))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.ReturnOrders;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.EditItemMaster))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.EditItemMaster;
                }
                else if (CurModule == ((int)SessionHelper.ModuleList.CommonBOMToItem))
                {
                    Session["CurModule"] = ImportMastersDTO.TableName.CommonBOMToItem;
                }
                if (isGridFill)
                {
                    TempData["CurModule"] = Session["CurModule"];
                }
                else
                {
                    TempData["CurModule"] = null;
                }

                return Json(new { CurModuleValue = Convert.ToString(Session["CurModuleValue"]), CurModule = Convert.ToString(Session["CurModule"]), ErrorMessege = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { CurModuleValue = Convert.ToString(Session["CurModuleValue"]), CurModule = Convert.ToString(Session["CurModule"]), ErrorMessege = Convert.ToString(ex) });
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
                            reterrormsg = string.Format(ResImportMasters.InvalidValuein, props[i].Name.ToString());
                        }
                    }
                    else if (props[i].GetType() == typeof(System.Double))
                    {
                        if (!CheckDoubleValue(props[i].GetValue(item).ToString()))
                        {
                            reterrormsg = string.Format(ResImportMasters.InvalidValuein, props[i].Name.ToString());
                        }
                    }
                    else if (props[i].GetType() == typeof(System.DateTime))
                    {
                        if (!CheckDatetimeValue(props[i].GetValue(item).ToString()))
                        {
                            reterrormsg = string.Format(ResImportMasters.InvalidValuein, props[i].Name.ToString());
                        }
                    }

                }
                //table.Rows.Add(values);
            }
            return lstreturnFinal;
        }
        public bool ValidateToDeleteLocation(string BinNumber, Guid ItemGuid)
        {
            bool result = false;
            try
            {
                ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                BinMasterDTO objBinMaster = objBinMasterDAL.GetBinByName(BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, false);
                if (objBinMaster != null)
                {
                    result = objItemLocationDetailsDAL.CheckBinForDelete(BinNumber, ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID);
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
        public bool CheckBinStatus(string BinNumber)
        {
            return ImportBAL.CheckBinStatus(BinNumber);
            //bool result = false;
            //try
            //{
            //    BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //    List<BinMasterDTO> objBinMasterList = objBinMasterDAL.GetListBinByName(BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, false);

            //    if (objBinMasterList != null)
            //    {
            //        int deletedCount = objBinMasterList.Where(b => b.IsDeleted == true).Count();
            //        int totalCount = objBinMasterList.Count();
            //        if (totalCount > 0 && totalCount == deletedCount)
            //        {

            //            long binId = objBinMasterList.OrderByDescending(t => t.ID).Select(t => t.ID).First();
            //            objBinMasterDAL.UndeleteparentLocation(binId);
            //            result = true;
            //        }
            //        else
            //        {
            //            result = true;
            //        }
            //    }
            //    else
            //    {
            //        result = true;
            //    }
            //}
            //catch
            //{
            //    return false;
            //}
            //return result;
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
            return ImportBAL.getTrendingID(name);
            //byte id = 0;
            //if (string.IsNullOrWhiteSpace(name))
            //{
            //    return null;
            //}
            //else
            //{
            //    if (name == "None")
            //    {
            //        id = 0;
            //    }
            //    else if (name == "Manual")
            //    {
            //        id = 1;
            //    }
            //    else if (name == "Automatic")
            //    {
            //        id = 2;
            //    }
            //    else
            //    {
            //        return null;
            //    }

            //}
            //return id;
        }

        public DataTable GetTableFromList(List<ItemLocationDetailsDTO> lstItemLocs)
        {
            //RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = DateTime.UtcNow; //objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
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
                new DataColumn() { AllowDBNull=true,ColumnName="ReceiptCost",DataType=typeof(double)},
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
                        row["LotNumber"] = (!string.IsNullOrWhiteSpace(item.LotNumber)) ? item.LotNumber.Trim() : string.Empty;
                        row["SerialNumber"] = (!string.IsNullOrWhiteSpace(item.SerialNumber)) ? item.SerialNumber.Trim() : string.Empty;
                        row["ConsignedQuantity"] = item.ConsignedQuantity.GetValueOrDefault(0);
                        row["CustomerOwnedQuantity"] = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                        row["ReceiptCost"] = item.Cost ?? 0; //(item.Cost ?? 0) > 0 ? (object)item.Cost : DBNull.Value;
                        row["UDF1"] = item.UDF1;
                        row["UDF2"] = item.UDF2;
                        row["UDF3"] = item.UDF3;
                        row["UDF4"] = item.UDF4;
                        row["UDF5"] = item.UDF5;
                        row["ProjectSpend"] = item.ProjectSpend;
                        ReturnDT.Rows.Add(row);
                    }
                }

                return ReturnDT;
            }
            catch
            {
                return ReturnDT;
            }
        }

        public DataTable GetToolTableFromList(List<ToolAssetQuantityDetailDTO> lstItemLocs)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
            DataTable ReturnDT = new DataTable("ToolAssetLocationParam");
            try
            {
                DataColumn[] arrColumns = new DataColumn[]            {
                new DataColumn() { AllowDBNull=true,ColumnName="ToolGUID",DataType=typeof(Guid)},
                new DataColumn() { AllowDBNull=true,ColumnName="AssetGUID",DataType=typeof(Guid)},
                new DataColumn() { AllowDBNull=true,ColumnName="ToolName",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="AssetName",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="ToolBinID",DataType=typeof(Int64)},
                new DataColumn() { AllowDBNull=true,ColumnName="BinNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="Received",DataType=typeof(DateTime)},
                new DataColumn() { AllowDBNull=true,ColumnName="SerialNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="Quantity",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="ReceiptCost",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF1",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF2",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF3",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF4",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF5",DataType=typeof(String)}
            };
                ReturnDT.Columns.AddRange(arrColumns);

                if (lstItemLocs != null && lstItemLocs.Count > 0)
                {
                    foreach (var item in lstItemLocs)
                    {
                        DateTime tempDT = DateTime.Now;
                        DataRow row = ReturnDT.NewRow();
                        row["ToolGUID"] = (item.ToolGUID ?? Guid.Empty) == Guid.Empty ? DBNull.Value : (object)item.ToolGUID;
                        row["AssetGUID"] = (item.AssetGUID ?? Guid.Empty) == Guid.Empty ? DBNull.Value : (object)item.AssetGUID; ;
                        row["ToolName"] = item.ToolName ?? string.Empty;
                        row["AssetName"] = string.Empty;
                        row["ToolBinID"] = (item.ToolBinID ?? 0) > 0 ? (object)item.ToolBinID : DBNull.Value;
                        row["BinNumber"] = item.Location ?? string.Empty;
                        row["Received"] = datetimetoConsider;
                        row["SerialNumber"] = (!string.IsNullOrWhiteSpace(item.SerialNumber)) ? item.SerialNumber.Trim() : string.Empty;
                        row["Quantity"] = item.Quantity;
                        row["ReceiptCost"] = (item.Cost ?? 0) > 0 ? (object)item.Cost : DBNull.Value;
                        row["UDF1"] = string.Empty;
                        row["UDF2"] = string.Empty;
                        row["UDF3"] = string.Empty;
                        row["UDF4"] = string.Empty;
                        row["UDF5"] = string.Empty;

                        ReturnDT.Rows.Add(row);
                    }
                }

                return ReturnDT;
            }
            catch
            {
                return ReturnDT;
            }
        }

        #region [New Import]


        public ActionResult Import()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFile CSVFile, HttpPostedFile ImageZipFile)
        {
            return View();
        }

        [HttpPost]
        public ActionResult ItemMasterImport()
        {
            return View();
        }

        #endregion

        public DataTable GetToolCountTableFromList(List<ToolAssetQuantityDetailDTO> lstItemLocs, long RoomID)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
            DataTable ReturnDT = new DataTable("ToolAssetLocationParam");
            try
            {
                DataColumn[] arrColumns = new DataColumn[]            {
                new DataColumn() { AllowDBNull=true,ColumnName="ToolGUID",DataType=typeof(Guid)},
                new DataColumn() { AllowDBNull=true,ColumnName="AssetGUID",DataType=typeof(Guid)},
                new DataColumn() { AllowDBNull=true,ColumnName="ToolName",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="AssetName",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="ToolBinID",DataType=typeof(Int64)},
                new DataColumn() { AllowDBNull=true,ColumnName="BinNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="Received",DataType=typeof(DateTime)},
                new DataColumn() { AllowDBNull=true,ColumnName="SerialNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="Quantity",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="ReceiptCost",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF1",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF2",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF3",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF4",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF5",DataType=typeof(String)}
            };
                ReturnDT.Columns.AddRange(arrColumns);

                if (lstItemLocs != null && lstItemLocs.Count > 0)
                {
                    foreach (var item in lstItemLocs)
                    {
                        DateTime tempDT = DateTime.Now;
                        DataRow row = ReturnDT.NewRow();
                        row["ToolGUID"] = (item.ToolGUID ?? Guid.Empty) == Guid.Empty ? DBNull.Value : (object)item.ToolGUID;
                        row["ToolName"] = item.ToolName ?? string.Empty;
                        row["AssetName"] = string.Empty;
                        row["ToolBinID"] = (item.ToolBinID ?? 0) > 0 ? (object)item.ToolBinID : DBNull.Value;
                        row["BinNumber"] = item.Location ?? string.Empty;
                        row["Received"] = datetimetoConsider;
                        row["SerialNumber"] = (!string.IsNullOrWhiteSpace(item.SerialNumber)) ? item.SerialNumber.Trim() : string.Empty;
                        row["Quantity"] = item.Quantity;
                        row["ReceiptCost"] = (item.Cost ?? 0) > 0 ? (object)item.Cost : DBNull.Value;
                        row["UDF1"] = string.Empty;
                        row["UDF2"] = string.Empty;
                        row["UDF3"] = string.Empty;
                        row["UDF4"] = string.Empty;
                        row["UDF5"] = string.Empty;

                        ReturnDT.Rows.Add(row);
                    }
                }

                return ReturnDT;
            }
            catch
            {
                return ReturnDT;
            }
        }

        public void SaveStreamAsFile(byte[] data, string fileName, OfflineImportFileHistoryDTO offlineImportFileHistory, bool isOffline = false)
        {
            string filePath = string.Empty;
            // Verify that the user selected a file
            if (data != null && data.Length > 0)
            {
                string RenamedFileName = Session["CurModule"].ToString() + "_" + ((SessionHelper.EnterPriceName ?? string.Empty) == string.Empty ? ("(" + SessionHelper.EnterPriceID + ")") : SessionHelper.EnterPriceName + "(" + SessionHelper.EnterPriceID + ")") + "_" + "(" + SessionHelper.CompanyID + ")" + "_" + "(" + SessionHelper.RoomID + ")" + "_" + DateTime.Now.Ticks.ToString();
                RenamedFileName = GetWithoutSpecChar(RenamedFileName);
                // extract only the fielname
                //string fileName = Path.GetFileName(fileName);
                string[] strfilename = fileName.Split('.');

                if (strfilename[strfilename.Length - 1].ToUpper() == "CSV")
                {
                    RenamedFileName = RenamedFileName + "_O_" + fileName.Substring(0, fileName.Length - 4);
                    if (RenamedFileName.Substring(0, RenamedFileName.Length - 4).Length > 125)
                        RenamedFileName = RenamedFileName.Substring(0, 125);

                    if (isOffline)
                    {
                        filePath = Path.Combine(Server.MapPath("~/Uploads/Import/CSV/" + Session["CurModule"].ToString() + "/" + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID));
                    }
                    else
                    {
                        filePath = Path.Combine(Server.MapPath("~/Uploads/Import/CSV"));
                    }
                }
                else if (strfilename[strfilename.Length - 1].ToUpper() == "XLS")
                {
                    filePath = Path.Combine(Server.MapPath("~/Uploads/Import/Excel"), RenamedFileName + ".xls");
                }

                DirectoryInfo info = new DirectoryInfo(filePath);
                if (!info.Exists)
                    info.Create();

                offlineImportFileHistory.CompanyID = SessionHelper.CompanyID;
                offlineImportFileHistory.Room = SessionHelper.RoomID;
                offlineImportFileHistory.CreatedBy = SessionHelper.UserID;
                offlineImportFileHistory.FileName = fileName;
                offlineImportFileHistory.FileUniqueName = RenamedFileName + ".csv";
                offlineImportFileHistory.FilePath = filePath;
                offlineImportFileHistory.IsProcessed = false;
                offlineImportFileHistory.ProcessStart = null;
                offlineImportFileHistory.ProcessEnd = null;

                string path = Path.Combine(filePath, RenamedFileName + ".csv");
                System.IO.File.WriteAllBytes(path, data);
            }
        }
    }
}