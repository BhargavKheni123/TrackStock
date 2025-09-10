using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class ImportMastersDTO
    {
        public enum TableName
        {
            Room,
            BinMaster,
            CategoryMaster,
            CustomerMaster,
            FreightTypeMaster,
            GLAccountMaster,
            GXPRConsigmentJobMaster,
            JobTypeMaster,
            ShipViaMaster,
            TechnicianMaster,
            ManufacturerMaster,
            MeasurementTermMaster,
            UnitMaster,
            SupplierMaster,
            ItemMaster,
            LocationMaster,
            ToolCategoryMaster,
            CostUOMMaster,
            InventoryClassificationMaster,
            ToolMaster,
            AssetToolSchedulerMapping,
            AssetMaster,
            QuickListItems,
            InventoryLocation,
            BOMItemMaster,
            WorkOrder,
            AssetCategoryMaster,
            kitdetail,
            ItemLocationeVMISetup,
            SupplierBlanketPODetails,
            ToolCheckInOutHistory,
            ItemManufacturerDetails,
            ItemSupplierDetails,
            BarcodeMaster,
            UDF,
            ProjectMaster,
            ItemLocationQty,
            ManualCount,
            PullMaster,
            PullImportWithLotSerial,
            ItemLocationChange,
            PullMasterWithSameQty,
            PastMaintenanceDue,
            AssetToolScheduler,
            ToolAdjustmentCount,
            ToolCertificationImages,
            OrderMaster,
            OrderUOMMaster,
            MoveMaterial,
            EnterpriseQuickList,
            Requisition,
            ToolWrittenOffCategory,
            QuoteMaster,
            SupplierCatalog,
            ReturnOrders,
            BinUDF,
            EditItemMaster,
            CommonBOMToItem
        }
    }
    public class ResImportMasters
    {
        private static string resourceFile = typeof(ResImportMasters).Name;
        public static string ID
        {
            get
            {
                return "ID";// ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }
        public static string Name
        {
            get
            {
                return "Name";// ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }
        public static string Address
        {
            get
            {
                return "Address";// ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }
        public static string Save
        {
            get
            {
                return "Save";// ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }
        public static string Cancel
        {
            get
            {
                return "Cancel";// ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Rooms.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to eTurns: Rooms.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }
        public static string ErrorInvalidFile
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorInvalidFile", resourceFile);
            }
        }
        public static string ImportBlankMsg
        {
            get
            {
                return ResourceRead.GetResourceValue("ImportBlankMsg", resourceFile);
            }
        }
        public static string ImportEmailMsg
        {
            get
            {
                return ResourceRead.GetResourceValue("ImportEmailMsg", resourceFile);
            }
        }
        public static string ImportBrowseFileMsg
        {
            get
            {
                return ResourceRead.GetResourceValue("ImportBrowseFileMsg", resourceFile);
            }
        }
        public static string ImportBrowseZIPFileMsg
        {
            get
            {
                return ResourceRead.GetResourceValue("ImportBrowseZIPFileMsg", resourceFile);
            }
        }

        public static string mustbegreaterthanorequalto
        {
            get
            {
                return ResourceRead.GetResourceValue("mustbegreaterthanorequalto", resourceFile);
            }
        }
        public static string Requiredfieldsbegin
        {
            get
            {
                return ResourceRead.GetResourceValue("Requiredfieldsbegin", resourceFile);
            }
        }
        public static string MsgInvalidTableName
        {
            get
            {
                return ResourceRead.GetResourceValue("msgInvalidTableName", resourceFile);
            }
        }
        public static string MsgInvalidUDFName
        {
            get
            {
                return ResourceRead.GetResourceValue("msgInvalidUDFName", resourceFile);
            }
        }

        public static string MsgModuleNameRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("msgModuleNameRequired", resourceFile);
            }
        }

        public static string MsgUDFNameRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("msgUDFNameRequired", resourceFile);
            }
        }
        public static string DateShouldBeInFormat
        {
            get
            {
                return ResourceRead.GetResourceValue("DateShouldBeInFormat", resourceFile);
            }
        }
        public static string InvalidValuein
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidValuein", resourceFile);
            }
        }
        public static string ImportMissingSerials
        {
            get
            {
                return ResourceRead.GetResourceValue("ImportMissingSerials", resourceFile);
            }
        }
        public static string ImportMissingLots
        {
            get
            {
                return ResourceRead.GetResourceValue("ImportMissingLots", resourceFile);
            }
        }
        public static string ImportMissingExpirationDates
        {
            get
            {
                return ResourceRead.GetResourceValue("ImportMissingExpirationDates", resourceFile);
            }
        }
        public static string MsgDownloadDataNotAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDownloadDataNotAvailable", resourceFile);
            }
        }
        public static string MsgInvalidFileValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidFileValidation", resourceFile);
            }
        }
        public static string MsgInvalidFileZipValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidFileZipValidation", resourceFile);
            }
        }
        public static string MsgSupplierDefaultAccountValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSupplierDefaultAccountValidation", resourceFile);
            }
        }
        public static string MsgModuleNameValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgModuleNameValidation", resourceFile);
            }
        }
        public static string MsgSerialblankValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSerialblankValidation", resourceFile);
            }
        }
        public static string MsgPurchaseDateFormat
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPurchaseDateFormat", resourceFile);
            }
        }
        public static string MsgEnterQuantitySerialTypeItem
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterQuantitySerialTypeItem", resourceFile);
            }
        }
        public static string MsgEnterCustomerOwnQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterCustomerOwnQuantity", resourceFile);
            }
        }
        public static string MsgEnterConsignedOwnQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterConsignedOwnQuantity", resourceFile);
            }
        }
        public static string MsgEnterUniqueSerial
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterUniqueSerial", resourceFile);
            }
        }
        public static string MsgImportedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgImportedSuccessfully", resourceFile);
            }
        }
        public static string MsgImportError
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgImportError", resourceFile);
            }
        }
        public static string MsgEnterUniqueBinNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterUniqueBinNumber", resourceFile);
            }
        }
        public static string MsgEnterUniqueSupplierNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterUniqueSupplierNumber", resourceFile);
            }
        }

        public static string SelectModule { get { return ResourceRead.GetResourceValue("SelectModule", resourceFile); } }
        public static string SelectFile { get { return ResourceRead.GetResourceValue("SelectFile", resourceFile); } }
        public static string ImportOffline { get { return ResourceRead.GetResourceValue("ImportOffline", resourceFile); } }
        public static string Load { get { return ResourceRead.GetResourceValue("Load", resourceFile); } }
        public static string DownloadEmptyTemplate { get { return ResourceRead.GetResourceValue("DownloadEmptyTemplate", resourceFile); } }
        public static string DownloadData { get { return ResourceRead.GetResourceValue("DownloadData", resourceFile); } }
        public static string MsgImportProcessing { get { return ResourceRead.GetResourceValue("MsgImportProcessing", resourceFile); } }

        public static string Categories
        {
            get
            {
                return ResourceHelper.GetResourceValue("Categories", resourceFile);
            }
        }
        public static string CostUOM
        {
            get
            {
                return ResourceHelper.GetResourceValue("CostUOM", resourceFile);
            }
        }
        public static string Customers
        {
            get
            {
                return ResourceHelper.GetResourceValue("Customers", resourceFile);
            }
        }
        public static string GLAccounts
        {
            get
            {
                return ResourceHelper.GetResourceValue("GLAccounts", resourceFile);
            }
        }
        public static string InventoryClassification
        {
            get
            {
                return ResourceHelper.GetResourceValue("InventoryClassification", resourceFile);
            }
        }
        public static string AdjustmentCount
        {
            get
            {
                return ResourceHelper.GetResourceValue("AdjustmentCount", resourceFile);
            }
        }
        public static string ItemLocations
        {
            get
            {
                return ResourceHelper.GetResourceValue("ItemLocations", resourceFile);
            }
        }
        public static string Manufacturers
        {
            get
            {
                return ResourceHelper.GetResourceValue("Manufacturers", resourceFile);
            }
        }
        public static string MeasurementTerms
        {
            get
            {
                return ResourceHelper.GetResourceValue("MeasurementTerms", resourceFile);
            }
        }
        public static string ShipVias
        {
            get
            {
                return ResourceHelper.GetResourceValue("ShipVias", resourceFile);
            }
        }
        public static string Suppliers
        {
            get
            {
                return ResourceHelper.GetResourceValue("Suppliers", resourceFile);
            }
        }
        public static string Technicians
        {
            get
            {
                return ResourceHelper.GetResourceValue("Technicians", resourceFile);
            }
        }
        public static string ToolCategories
        {
            get
            {
                return ResourceHelper.GetResourceValue("ToolCategories", resourceFile);
            }
        }
        public static string ToolLocations
        {
            get
            {
                return ResourceHelper.GetResourceValue("ToolLocations", resourceFile);
            }
        }
        public static string Units
        {
            get
            {
                return ResourceHelper.GetResourceValue("Units", resourceFile);
            }
        }
        public static string Items
        {
            get
            {
                return ResourceHelper.GetResourceValue("Items", resourceFile);
            }
        }
        public static string EditItems
        {
            get
            {
                return ResourceHelper.GetResourceValue("EditItems", resourceFile);
            }
        }
        public static string Assets
        {
            get
            {
                return ResourceHelper.GetResourceValue("Assets", resourceFile);
            }
        }

        //AssetToolSchedulerMapping
        public static string AssetToolSchedulerMapping
        {
            get
            {
                return ResourceHelper.GetResourceValue("AssetToolSchedulerMapping", resourceFile);
            }
        }
        public static string Tools
        {
            get
            {
                return ResourceHelper.GetResourceValue("Tools", resourceFile);
            }
        }
        public static string QuickList
        {
            get
            {
                return ResourceHelper.GetResourceValue("QuickList", resourceFile);
            }
        }
        public static string BOMItems
        {
            get
            {
                return ResourceHelper.GetResourceValue("BOMItems", resourceFile);
            }
        }
        public static string CommonBOMItems
        {
            get
            {
                return ResourceHelper.GetResourceValue("CommonBOMItems", resourceFile);
            }
        }
        public static string Kits
        {
            get
            {
                return ResourceHelper.GetResourceValue("Kits", resourceFile);
            }
        }
        public static string ItemManufacturer
        {
            get
            {
                return ResourceHelper.GetResourceValue("ItemManufacturer", resourceFile);
            }
        }
        public static string ItemSupplier
        {
            get
            {
                return ResourceHelper.GetResourceValue("ItemSupplier", resourceFile);
            }
        }
        public static string BarcodeAssociations
        {
            get
            {
                return ResourceHelper.GetResourceValue("BarcodeAssociations", resourceFile);
            }
        }
        public static string UDF
        {
            get
            {
                return ResourceHelper.GetResourceValue("UDF", resourceFile);
            }
        }
        public static string ProjectSpends
        {
            get
            {
                return ResourceHelper.GetResourceValue("ProjectSpends", resourceFile);
            }
        }
        public static string ItemQuantityImportwithCost
        {
            get
            {
                return ResourceHelper.GetResourceValue("ItemQuantityImportwithCost", resourceFile);
            }
        }
        public static string ManualCount
        {
            get
            {
                return ResourceHelper.GetResourceValue("ManualCount", resourceFile);
            }
        }
        public static string WorkOrder
        {
            get
            {
                return ResourceHelper.GetResourceValue("WorkOrder", resourceFile);
            }
        }
        public static string PullImport
        {
            get
            {
                return ResourceHelper.GetResourceValue("PullImport", resourceFile);
            }
        }
        public static string PullImportWithLotSerial
        {
            get
            {
                return ResourceHelper.GetResourceValue("PullImportWithLotSerial", resourceFile);
            }
        }
        public static string ItemLocationChange
        {
            get
            {
                return ResourceHelper.GetResourceValue("ItemLocationChange", resourceFile);
            }
        }
        public static string PullWithSameQty
        {
            get
            {
                return ResourceHelper.GetResourceValue("PullWithSameQty", resourceFile);
            }
        }

        public static string AssetToolScheduler
        {
            get
            {
                return ResourceHelper.GetResourceValue("AssetToolScheduler", resourceFile);
            }
        }
        public static string PastMaintenanceDue
        {
            get
            {
                return ResourceHelper.GetResourceValue("PastMaintenanceDue", resourceFile);
            }
        }

        public static string ToolCheckInCheckOut
        {
            get
            {
                return ResourceHelper.GetResourceValue("ToolCheckInCheckOut", resourceFile);
            }
        }

        public static string ToolAdjustmentCount
        {
            get
            {
                return ResourceHelper.GetResourceValue("ToolAdjustmentCount", resourceFile);
            }
        }

        /// <summary>
        /// Tool Certification Images
        /// </summary>
        public static string ToolCertificationImages
        {
            get
            {
                return ResourceHelper.GetResourceValue("ToolCertificationImages", resourceFile);
            }
        }

        public static string Order
        {
            get
            {
                return ResourceHelper.GetResourceValue("Order", resourceFile);
            }
        }

        public static string MoveMaterial
        {
            get
            {
                return ResourceHelper.GetResourceValue("MoveMaterial", resourceFile);
            }
        }
        public static string EnterpriseQuickList
        {
            get
            {
                return ResourceHelper.GetResourceValue("EnterpriseQuickList", resourceFile);
            }
        }
        public static string Requisition
        {
            get
            {
                return ResourceHelper.GetResourceValue("Requisition", resourceFile);
            }
        }
        public static string Quote
        {
            get
            {
                return ResourceHelper.GetResourceValue("Quote", resourceFile);
            }
        }

        public static string ImportReceiptAddNewReceipt
        {
            get
            {
                return ResourceHelper.GetResourceValue("ImportReceiptAddNewReceipt", resourceFile);
            }
        }

        public static string SupplierCatalog
        {
            get
            {
                return ResourceHelper.GetResourceValue("SupplierCatalog", resourceFile);
            }
        }
        public static string Returns
        {
            get
            {
                return ResourceHelper.GetResourceValue("Returns", resourceFile);
            }
        }
        public static string CommonBOMToItem
        {
            get
            {
                return ResourceHelper.GetResourceValue("CommonBOMToItem", resourceFile);
            }
        }
    }

    [Serializable]
    public class CostUOMMasterMain
    {
        public System.Int32 ID { get; set; }
        public System.String CostUOM { get; set; }
        public Nullable<System.Int32> CostUOMValue { get; set; }
        public System.String UDF1 { get; set; }
        public System.String UDF2 { get; set; }
        public System.String UDF3 { get; set; }
        public System.String UDF4 { get; set; }
        public System.String UDF5 { get; set; }
        public Guid GUID { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public Nullable<System.Int64> CompanyID { get; set; }
        public Nullable<System.Int64> Room { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public System.DateTime ReceivedOn { get; set; }
        public System.DateTime ReceivedOnWeb { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
    }
    [Serializable]
    public class InventoryClassificationMasterMain
    {
        public System.Int32 ID { get; set; }
        public System.String InventoryClassification { get; set; }
        public System.String BaseOfInventory { get; set; }
        public Nullable<System.Double> RangeStart { get; set; }
        public Nullable<System.Double> RangeEnd { get; set; }
        public System.String UDF1 { get; set; }
        public System.String UDF2 { get; set; }
        public System.String UDF3 { get; set; }
        public System.String UDF4 { get; set; }
        public System.String UDF5 { get; set; }
        public Guid GUID { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public Nullable<System.Int64> CompanyID { get; set; }
        public Nullable<System.Int64> Room { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public System.DateTime ReceivedOn { get; set; }
        public System.DateTime ReceivedOnWeb { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
    }

    [Serializable]
    public class BinMasterMain
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public string BinNumber { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> LastUpdated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public bool IsStagingLocation { get; set; }
        public bool IsStagingHeader { get; set; }
        public Guid? MaterialStagingGUID { get; set; }
        public double? CriticalQuantity { get; set; }
        public double? MinimumQuantity { get; set; }
        public double? MaximumQuantity { get; set; }
        public Guid? ItemGUID { get; set; }
        public double? SuggestedOrderQuantity { get; set; }
        public bool IsDefault { get; set; }
        public long? ParentBinId { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
    }
    [Serializable]
    public class LocationMasterMain
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public string Location { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> LastUpdated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }

        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
    }
    [Serializable]
    public class ToolCategoryMasterMain
    {

        public Guid GUID { get; set; }
        public string ToolCategory { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Int64 ID { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
    }
    [Serializable]
    public class CategoryMasterMain
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Category", ResourceType = typeof(ResCategoryMaster))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Category { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public string CategoryColor { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public System.DateTime ReceivedOn { get; set; }
        public System.DateTime ReceivedOnWeb { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
    }
    [Serializable]
    public class UDFOptionsMain
    {
        public Int64 ID { get; set; }
        public Int64 UDFID { get; set; }
        public string UDFOption { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Guid GUID { get; set; }
        public bool IsDeleted { get; set; }

        public Nullable<Int64> Room { get; set; }

        public long CompanyID { get; set; }
    }
    [Serializable]
    public class CustomerMasterMain
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Customer", ResourceType = typeof(ResCustomer))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Customer { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Account", ResourceType = typeof(ResCustomer))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Account { get; set; }

        [Display(Name = "Contact", ResourceType = typeof(ResCustomer))]
        [StringLength(512, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Contact { get; set; }

        [Display(Name = "Address", ResourceType = typeof(ResCommon))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Address { get; set; }

        [Display(Name = "City", ResourceType = typeof(ResCommon))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string City { get; set; }

        [Display(Name = "State", ResourceType = typeof(ResCommon))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string State { get; set; }

        [Display(Name = "ZipCode", ResourceType = typeof(ResCommon))]
        [StringLength(24, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string ZipCode { get; set; }

        [Display(Name = "Country", ResourceType = typeof(ResCommon))]
        [StringLength(64, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Country { get; set; }

        [Display(Name = "Phone", ResourceType = typeof(ResCommon))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Phone { get; set; }

        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(ResMessage))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", ResourceType = typeof(ResCommon))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Email { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
        public string Remarks { get; set; }
    }
    [Serializable]
    public class FreightTypeMasterMain
    {
        public Guid GUID { get; set; }
        public string FreightType { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> LastUpdated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Int64 ID { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
    }
    [Serializable]
    public class GLAccountMasterMain
    {
        public Int64 ID { get; set; }
        public string GLAccount { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Guid GUID { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
    }
    [Serializable]
    public class GXPRConsignedMasterMain
    {
        public Guid GUID { get; set; }
        public string GXPRConsigmentJob { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Int64 ID { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
    [Serializable]
    public class JobTypeMasterMain
    {
        public Int64 ID { get; set; }
        public string JobType { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> LastUpdated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Guid GUID { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
    [Serializable]
    public class ShipViaMasterMain
    {
        public Guid GUID { get; set; }
        public string ShipVia { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Int64 ID { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public System.DateTime ReceivedOn { get; set; }
        public System.DateTime ReceivedOnWeb { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
    }
    [Serializable]
    public class TechnicianMasterMain
    {
        public Int64 ID { get; set; }
        public string Technician { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Guid GUID { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public string TechnicianCode { get; set; }
    }
    [Serializable]
    public class ManufacturerMasterMain
    {
        public Guid GUID { get; set; }
        public string Manufacturer { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Int64 ID { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
    }
    [Serializable]
    public class MeasurementTermMasterMain
    {
        public Guid GUID { get; set; }
        public string MeasurementTerm { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Int64 ID { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
    }
    [Serializable]
    public class UnitMasterMain
    {
        public Int64 ID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }

        public string Unit { get; set; }
        public Guid GUID { get; set; }
        public string Description { get; set; }

        [RegularExpression("([0-9]+)")]
        [Display(Name = "Odometer", ResourceType = typeof(ResUnitMaster))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Odometer { get; set; }
        public Nullable<DateTime> OdometerUpdate { get; set; }
        public Nullable<Decimal> OpHours { get; set; }
        public Nullable<DateTime> OpHoursUpdate { get; set; }

        public Nullable<long> SerialNo { get; set; }

        [StringLength(4, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidYear", ErrorMessageResourceType = typeof(ResUnitMaster))]
        [Display(Name = "Year", ResourceType = typeof(ResUnitMaster))]
        public Nullable<Int64> Year { get; set; }

        public string Make { get; set; }
        public string Model { get; set; }
        public string Plate { get; set; }
        public string EngineModel { get; set; }
        public string EngineSerialNo { get; set; }

        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InValidMarkupParts", ErrorMessageResourceType = typeof(ResUnitMaster))]
        [Display(Name = "MarkupParts", ResourceType = typeof(ResUnitMaster))]
        public Nullable<long> MarkupParts { get; set; }

        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InValidMarkuplabour", ErrorMessageResourceType = typeof(ResUnitMaster))]
        [Display(Name = "MarkupLabour", ResourceType = typeof(ResUnitMaster))]
        public Nullable<long> MarkupLabour { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
    }
    [Serializable]
    public class SupplierMasterMain
    {
        public Int64 ID { get; set; }
        public string SupplierName { get; set; }
        public string SupplierColor { get; set; }
        public string Description { get; set; }
        public string BranchNumber { get; set; }
        public int? MaximumOrderSize { get; set; }
        //public string AccountNo { get; set; }
        //public string ReceiverID { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public bool IsSendtoVendor { get; set; }
        public bool IsVendorReturnAsn { get; set; }
        public bool IsSupplierReceivesKitComponents { get; set; }

        public int? POAutoSequence { get; set; }

        public bool OrderNumberTypeBlank { get; set; }
        public bool OrderNumberTypeFixed { get; set; }
        public bool OrderNumberTypeBlanketOrderNumber { get; set; }
        public bool OrderNumberTypeIncrementingOrderNumber { get; set; }
        public bool OrderNumberTypeIncrementingbyDay { get; set; }
        public bool OrderNumberTypeDateIncrementing { get; set; }
        public bool OrderNumberTypeDate { get; set; }

        public string Email { get; set; }
        public bool IsEmailPOInBody { get; set; }
        public bool IsEmailPOInPDF { get; set; }
        public bool IsEmailPOInCSV { get; set; }
        public bool IsEmailPOInX12 { get; set; }

        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountAddress { get; set; }
        public string AccountCity { get; set; }
        public string AccountState { get; set; }
        public string AccountZip { get; set; }

        public string AccountCountry { get; set; }

        public string AccountShipToID { get; set; }

        public bool AccountIsDefault { get; set; }

        public string BlanketPONumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? MaxLimit { get; set; }
        public bool IsNotExceed { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Guid GUID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }

        public bool PullPurchaseNumberFixed { get; set; }
        public bool PullPurchaseNumberBlanketOrder { get; set; }
        public bool PullPurchaseNumberDateIncrementing { get; set; }
        public bool PullPurchaseNumberDate { get; set; }
        public int? PullPurchaseNumberType { get; set; }
        public string LastPullPurchaseNumberUsed { get; set; }
        public bool IsBlanketDeleted { get; set; }

        public string SupplierImage { get; set; }

        public string ImageType { get; set; }
        public string ImageExternalURL { get; set; }

        public double? MaxLimitQty { get; set; }
        public bool IsNotExceedQty { get; set; }
        public string POAutoReleaseNumber { get;set;}
    }
    [Serializable]
    public class ToolMasterMain
    {

        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public string ToolName { get; set; }
        public string Serial { get; set; }
        public string Description { get; set; }

        public Nullable<Int64> ToolCategoryID { get; set; }
        public double? Cost { get; set; }
        public Nullable<bool> IscheckedOut { get; set; }
        public Nullable<Int32> IsGroupOfItems { get; set; }
        public System.Double Quantity { get; set; }
        public Nullable<System.Double> CheckedOutQTY { get; set; }
        public Nullable<System.Double> CheckedOutMQTY { get; set; }
        public DateTime Created { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }

        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<Int64> LocationID { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public string WhatWhereAction { get; set; }
        public string Technician { get; set; }
        public Guid TechnicianGuid { get; set; }
        public System.Double? CheckOutQuantity { get; set; }
        public System.Double? CheckInQuantity { get; set; }

        public string CheckOutStatus { get; set; }
        public Nullable<DateTime> CheckOutDate { get; set; }
        public Nullable<DateTime> CheckInDate { get; set; }
        public string ToolCategory { get; set; }
        public string RoomName { get; set; }
        public string Location { get; set; }
        public string Action { get; set; }
        public Int64 HistoryID { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }

        public string CheckOutUDF1 { get; set; }
        public string CheckOutUDF2 { get; set; }
        public string CheckOutUDF3 { get; set; }
        public string CheckOutUDF4 { get; set; }
        public string CheckOutUDF5 { get; set; }
        public string ToolImageExternalURL { get; set; }
        public string ImageType { get; set; }
        public string ImagePath { get; set; }
        public bool SerialNumberTracking { get; set; }

        public string ToolTypeTracking { get; set; }
        public int? NoOfPastMntsToConsider { get; set; }
        public int? MaintenanceDueNoticeDays { get; set; }
    }

    /// <summary>
    /// This class is used for ToolCheckInCheckOut import
    /// </summary>
    public class ToolCheckInCheckOut
    {
        public int Id { get; set; }
        public string ToolName { get; set; }
        public string Serial { get; set; }

        public string Location { get; set; }

        public string TechnicianCode { get; set; }

        public System.Double? Quantity { get; set; }
        public string Operation { get; set; }
        public string CheckOutUDF1 { get; set; }
        public string CheckOutUDF2 { get; set; }
        public string CheckOutUDF3 { get; set; }
        public string CheckOutUDF4 { get; set; }
        public string CheckOutUDF5 { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }

    [Serializable]
    public class ToolImageImport
    {
        public int Id { get; set; }
        public string ToolName { get; set; }
        public string Serial { get; set; }
        public string ImageName { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }

    [Serializable]
    public class AssetMasterMain
    {
        public System.Int64 ID { get; set; }
        public System.String AssetName { get; set; }
        public System.String Description { get; set; }
        public System.String Make { get; set; }
        public System.String Model { get; set; }
        public System.String Serial { get; set; }
        public Nullable<System.Int64> ToolCategoryID { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public Nullable<System.Double> PurchasePrice { get; set; }
        public Nullable<System.Double> DepreciatedValue { get; set; }
        public Nullable<System.DateTime> SuggestedMaintenanceDate { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }
        public Nullable<System.Int64> Room { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Guid GUID { get; set; }
        public Nullable<System.Int64> CompanyID { get; set; }
        public System.String UDF1 { get; set; }
        public System.String UDF2 { get; set; }
        public System.String UDF3 { get; set; }
        public System.String UDF4 { get; set; }
        public System.String UDF5 { get; set; }
        public string WhatWhereAction { get; set; }

        public string RoomName { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public string ToolCategory { get; set; }
        public string Action { get; set; }
        public Int64 HistoryID { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
        public string AssetCategory { get; set; }
        public Nullable<System.Int64> AssetCategoryId { get; set; }
        public Nullable<Boolean> IsAutoMaintain { get; set; }
        public Nullable<System.Int64> MaintenanceType { get; set; }
        public Nullable<System.Int64> NoOfPastMntsToConsider { get; set; }
        public Nullable<Int32> MaintenanceCount { get; set; }
        private string _PurchaseDate;
        public string PurchaseDateString
        {
            get
            {
                if (string.IsNullOrEmpty(_PurchaseDate))
                {
                    _PurchaseDate = FnCommon.ConvertDateByTimeZone(PurchaseDate, true);
                }
                return _PurchaseDate;
            }
            set { this._PurchaseDate = value; }
        }
        public System.String UDF6 { get; set; }
        public System.String UDF7 { get; set; }
        public System.String UDF8 { get; set; }
        public System.String UDF9 { get; set; }
        public System.String UDF10 { get; set; }

        public System.String AssetImageExternalURL { get; set; }
        public string ImageType { get; set; }

        public System.String ImagePath { get; set; }
    }

    [Serializable]
    public class AssetToolSchedulerMapping
    {
        public Int64 ID { get; set; }

        public Guid? ToolSchedulerGuid { get; set; }

        public Guid? ToolGUID { get; set; }

        public Guid? AssetGUID { get; set; }

        public byte? SchedulerFor { get; set; }

        public string ScheduleForName { get; set; }

        public string SchedulerName { get; set; }

        public string AssetName { get; set; }

        public string ToolName { get; set; }

        public string Serial { get; set; }

        public Guid GUID { get; set; }


        public System.String UDF1 { get; set; }
        public System.String UDF2 { get; set; }
        public System.String UDF3 { get; set; }
        public System.String UDF4 { get; set; }
        public System.String UDF5 { get; set; }

        public Nullable<Boolean> IsDeleted { get; set; }

        public Nullable<Boolean> IsArchived { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }


        public Nullable<System.Int64> CompanyID { get; set; }

        public Nullable<System.Int64> Room { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
    }

    [Serializable]
    public class QuickListItemsMain
    {
        public System.Int64 ID { get; set; }
        public Guid QuickListGUID { get; set; }
        public Guid ItemGUID { get; set; }
        public System.Double Quantity { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }
        public Nullable<System.Int64> Room { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public Nullable<System.Int64> CompanyID { get; set; }
        public Guid GUID { get; set; }

        public System.String UDF1 { get; set; }
        public System.String UDF2 { get; set; }
        public System.String UDF3 { get; set; }
        public System.String UDF4 { get; set; }
        public System.String UDF5 { get; set; }
        public string QuickListname { get; set; }
        public string ItemNumber { get; set; }
        public string Comments { get; set; }

        public System.String Status { get; set; }
        public System.String Reason { get; set; }

        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
        public string BinNumber { get; set; }
        public long BinID { get; set; }
        public System.Double ConsignedQuantity { get; set; }
        public string Type { get; set; }
        public QuickListType? QLType { get; set; }
    }

    [Serializable]
    public class OrderMasterItemsMain
    {
        public System.String Supplier { get; set; }
        public System.String OrderNumber { get; set; }
        public System.String RequiredDate { get; set; }
        public System.String OrderStatus { get; set; }
        public System.String StagingName { get; set; }
        public System.String OrderComment { get; set; }
        public System.String CustomerName { get; set; }
        public System.String PackSlipNumber { get; set; }
        public System.String ShippingTrackNumber { get; set; }
        public System.String OrderUDF1 { get; set; }
        public System.String OrderUDF2 { get; set; }
        public System.String OrderUDF3 { get; set; }
        public System.String OrderUDF4 { get; set; }
        public System.String OrderUDF5 { get; set; }
        public System.String ShipVia { get; set; }
        public System.String OrderType { get; set; }
        public System.String ShippingVendor { get; set; }
        //public System.String AccountNumber { get; set; }
        public System.String SupplierAccount { get; set; }
        public System.String ItemNumber { get; set; }
        public System.String Bin { get; set; }
        public Nullable<System.Double> RequestedQty { get; set; }
        public Nullable<System.Double> ReceivedQty { get; set; }
        public System.String ASNNumber { get; set; }
        public Nullable<System.Double> ApprovedQty { get; set; }
        public Nullable<System.Double> InTransitQty { get; set; }
        //public System.String IsCloseItem { get; set; }
        public Boolean IsCloseItem { get; set; }
        public System.String LineNumber { get; set; }
        public System.String ControlNumber { get; set; }
        public System.String ItemComment { get; set; }
        public System.String LineItemUDF1 { get; set; }
        public System.String LineItemUDF2 { get; set; }
        public System.String LineItemUDF3 { get; set; }
        public System.String LineItemUDF4 { get; set; }
        public System.String LineItemUDF5 { get; set; }

        public System.Int64 ID { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public Guid? OrderGUID { get; set; }
        public Guid? ItemGUID { get; set; }
        public Nullable<System.Int64> RequesterID { get; set; }
        public Nullable<System.Int64> ApproverID { get; set; }
        public Nullable<System.Double> OrderCost { get; set; }
        public string SalesOrder { get; set; }
    }

    [Serializable]
    public class InventoryLocationMain
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public string BinNumber { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }

        public Nullable<System.Double> customerownedquantity { get; set; }
        public Nullable<System.Double> consignedquantity { get; set; }
        public Nullable<Guid> ItemGUID { get; set; }
        public string LotNumber { get; set; }
        public string SerialNumber { get; set; }
        public string Received { get; set; }
        public string Expiration { get; set; }
        public string displayExpiration { get; set; }
        public Nullable<System.Double> Cost { get; set; }
        public string ItemNumber { get; set; }
        public string InsertedFrom { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }

        public string ProjectSpend { get; set; }

        public string ItemDescription { get; set; }
    }
    [Serializable]
    public class InventoryLocationQuantityMain
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public string BinNumber { get; set; }
        public string ItemNumber { get; set; }
        public Nullable<System.Double> MinimumQuantity { get; set; }
        public Nullable<System.Double> MaximumQuantity { get; set; }
        public Nullable<System.Double> CriticalQuantity { get; set; }
        public string SensorPort { get; set; }
        public Nullable<System.Double> SensorId { get; set; }
        public Nullable<Boolean> IsDefault { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<bool> IsStagingLocation { get; set; }

        public Nullable<bool> IsEnforceDefaultPullQuantity { get; set; }
        public Nullable<System.Double> DefaultPullQuantity { get; set; }

        public Nullable<bool> IsEnforceDefaultReorderQuantity { get; set; }


        public Nullable<System.Double> DefaultReorderQuantity { get; set; }

        public Nullable<bool> IsArchived { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public Nullable<Guid> ItemGUID { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public System.String BinUDF1 { get; set; }
        public System.String BinUDF2 { get; set; }
        public System.String BinUDF3 { get; set; }
        public System.String BinUDF4 { get; set; }
        public System.String BinUDF5 { get; set; }


    }
    [Serializable]
    public class BOMItemMasterMain
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //ItemNumber
        [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
        [StringLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ItemNumber { get; set; }

        //ManufacturerID
        [Display(Name = "ManufacturerID", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int64> ManufacturerID { get; set; }

        //ManufacturerNumber
        [Display(Name = "ManufacturerNumber", ResourceType = typeof(ResItemMaster))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ManufacturerNumber { get; set; }

        //SupplierID
        [Display(Name = "SupplierID", ResourceType = typeof(ResItemMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64? SupplierID { get; set; }
        public System.Int64? BlanketPOID { get; set; }

        //SupplierPartNo
        [Display(Name = "SupplierPartNo", ResourceType = typeof(ResItemMaster))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String SupplierPartNo { get; set; }

        //BlanketOrderNumber
        public System.String BlanketOrderNumber { get; set; }

        //UPC
        [Display(Name = "UPC", ResourceType = typeof(ResItemMaster))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UPC { get; set; }

        //UNSPSC
        [Display(Name = "UNSPSC", ResourceType = typeof(ResItemMaster))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UNSPSC { get; set; }

        //Description
        [Display(Name = "Description", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String Description { get; set; }

        //LongDescription
        [Display(Name = "LongDescription", ResourceType = typeof(ResItemMaster))]
        [StringLength(2000, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String LongDescription { get; set; }

        //CategoryID
        [Display(Name = "CategoryID", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int64> CategoryID { get; set; }



        //GLAccountID
        [Display(Name = "GLAccountID", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int64> GLAccountID { get; set; }

        //UOMID
        [Display(Name = "UOMID", ResourceType = typeof(ResItemMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 UOMID { get; set; }


        //PricePerTerm
        [Display(Name = "PricePerTerm", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Decimal> PricePerTerm { get; set; }

        //DefaultReorderQuantity
        [Display(Name = "DefaultReorderQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[DefaultReorderQuantityCheck("MaximumQuantity", ErrorMessage = "Default Reorder quantity must be less then Maximum quantity")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> DefaultReorderQuantity { get; set; }

        //DefaultPullQuantity
        [Display(Name = "DefaultPullQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> DefaultPullQuantity { get; set; }

        ////DefaultCartQuantity
        //[Display(Name = "DefaultCartQuantity", ResourceType = typeof(ResItemMaster))]
        //[RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //public Nullable<System.Double> DefaultCartQuantity { get; set; }

        //Cost
        [Display(Name = "Cost", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Cost { get; set; }

        //Markup
        [Display(Name = "Markup", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[Range(0, 100, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Markup { get; set; }

        //SellPrice
        [Display(Name = "SellPrice", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]

        public Nullable<System.Double> SellPrice { get; set; }

        //ExtendedCost
        [Display(Name = "ExtendedCost", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ExtendedCost { get; set; }

        //LeadTimeInDays
        [Display(Name = "LeadTimeInDays", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int32> LeadTimeInDays { get; set; }

        //Link1
        [Display(Name = "Link1", ResourceType = typeof(ResItemMaster))]
        [StringLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String Link1 { get; set; }

        //Link2
        [Display(Name = "Link2", ResourceType = typeof(ResItemMaster))]
        [StringLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String Link2 { get; set; }

        //Trend
        [Display(Name = "Trend", ResourceType = typeof(ResItemMaster))]
        public Boolean Trend { get; set; }

        //Taxable
        [Display(Name = "Taxable", ResourceType = typeof(ResItemMaster))]
        public Boolean Taxable { get; set; }

        //Consignment
        [Display(Name = "Consignment", ResourceType = typeof(ResItemMaster))]
        public Boolean Consignment { get; set; }
        public Nullable<Boolean> IsBlankConsignment { get; set; }

        //StagedQuantity
        [Display(Name = "StagedQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> StagedQuantity { get; set; }

        //InTransitquantity
        [Display(Name = "InTransitquantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> InTransitquantity { get; set; }

        //OnOrderQuantity
        [Display(Name = "OnOrderQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> OnOrderQuantity { get; set; }



        //OnTransferQuantity
        [Display(Name = "OnTransferQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> OnTransferQuantity { get; set; }

        //SuggestedOrderQuantity
        [Display(Name = "SuggestedOrderQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> SuggestedOrderQuantity { get; set; }

        //RequisitionedQuantity
        [Display(Name = "RequisitionedQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> RequisitionedQuantity { get; set; }

        ////PackingQuantity
        //[Display(Name = "PackingQuantity", ResourceType = typeof(ResItemMaster))]
        //[RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //public Nullable<System.Double> PackingQuantity { get; set; }


        //AverageUsage
        [Display(Name = "AverageUsage", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> AverageUsage { get; set; }

        //Turns
        [Display(Name = "Turns", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Turns { get; set; }

        //OnHandQuantity        
        [Display(Name = "OnHandQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> OnHandQuantity { get; set; }

        //CriticalQuantity
        [Display(Name = "CriticalQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [CriticleQuantityCheck("MinimumQuantity", ErrorMessage = "Critical quantity must be less then Minimum quantity")]
        [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
        public System.Double CriticalQuantity { get; set; }

        //MinimumQuantity
        [Display(Name = "MinimumQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [MinimumQuantityCheck("MaximumQuantity", ErrorMessage = "Minimum quantity must be less then Maximum quantity")]
        [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
        public System.Double MinimumQuantity { get; set; }

        //MaximumQuantity
        [Display(Name = "MaximumQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
        public System.Double MaximumQuantity { get; set; }

        //WeightPerPiece
        [Display(Name = "WeightPerPiece", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> WeightPerPiece { get; set; }

        //ItemUniqueNumber
        [Display(Name = "ItemUniqueNumber", ResourceType = typeof(ResItemMaster))]
        [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ItemUniqueNumber { get; set; }

        //TransferOrPurchase
        [Display(Name = "IsTransfer", ResourceType = typeof(ResItemMaster))]
        public Boolean IsTransfer { get; set; }

        //IsPurchase
        [Display(Name = "IsPurchase", ResourceType = typeof(ResItemMaster))]
        public Boolean IsPurchase { get; set; }

        public Boolean IsActive { get; set; }
        public Boolean IsOrderable { get; set; }
        public Boolean IsAllowOrderCostuom { get; set; }
        //DefaultLocation
        [Display(Name = "DefaultLocation", ResourceType = typeof(ResItemMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public long DefaultLocation { get; set; }


        //DefaultLocation
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "DefaultLocation", ResourceType = typeof(ResItemMaster))]
        public string DefaultLocationName { get; set; }


        //InventoryClassification
        [Display(Name = "InventoryClassification", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int32> InventoryClassification { get; set; }


        public string InventoryClassificationName { get; set; }


        //SerialNumberTracking
        [Display(Name = "SerialNumberTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean SerialNumberTracking { get; set; }

        //LotNumberTracking
        [Display(Name = "LotNumberTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean LotNumberTracking { get; set; }

        //DateCodeTracking
        [Display(Name = "DateCodeTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean DateCodeTracking { get; set; }

        //ItemType
        [Display(Name = "ItemType", ResourceType = typeof(ResItemMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int32 ItemType { get; set; }

        //ImagePath
        [Display(Name = "ImagePath", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ImagePath { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF5 { get; set; }

        //UDF6
        [Display(Name = "UDF6", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF6 { get; set; }

        //UDF7
        [Display(Name = "UDF7", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF7 { get; set; }

        //UDF8
        [Display(Name = "UDF8", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF8 { get; set; }

        //UDF9
        [Display(Name = "UDF9", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF9 { get; set; }

        //UDF10
        [Display(Name = "UDF10", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UDF10 { get; set; }

        //GUID
        public Guid GUID { get; set; }

        //Created
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //Updated
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }
        public string CompanyNumber { get; set; }

        //Room
        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //IsLotSerialExpiryCost
        [Display(Name = "IsLotSerialExpiryCost", ResourceType = typeof(ResItemMaster))]
        [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String IsLotSerialExpiryCost { get; set; }

        //PackingQuantity
        [Display(Name = "PackingQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> PackingQuantity { get; set; }

        [Display(Name = "IsItemLevelMinMaxQtyRequired", ResourceType = typeof(ResItemMaster))]
        public bool? IsItemLevelMinMaxQtyRequired { get; set; }

        //[Display(Name="Enforce Default Reorder Quantity")]
        [Display(Name = "IsEnforceDefaultReorderQuantity", ResourceType = typeof(ResItemMaster))]
        public bool? IsEnforceDefaultReorderQuantity { get; set; }

        //IsBuildBreak
        [Display(Name = "IsBuildBreak", ResourceType = typeof(ResItemMaster))]
        public Nullable<Boolean> IsBuildBreak { get; set; }

        [Display(Name = "Bonded Inventory")]
        public string BondedInventory { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        public ICollection<ItemLocationDetailsDTO> ItemLocations { get; set; }
        public ICollection<ItemLocationQTYDTO> lstItemLocationQTY { get; set; }
        public ICollection<BinMasterDTO> ItemsLocations { get; set; }
        public string CategoryName { get; set; }

        public string CategoryColor { get; set; }

        [Display(Name = "ManufacturerName", ResourceType = typeof(ResManufacturer))]
        public string ManufacturerName { get; set; }

        [Display(Name = "Supplier", ResourceType = typeof(ResSupplierMaster))]
        public string SupplierName { get; set; }
        public string ItemTypeName { get; set; }

        [Display(Name = "UOMID", ResourceType = typeof(ResItemMaster))]
        [Required]
        public string Unit { get; set; }

        public string GLAccount { get; set; }
        public string InventryLocation { get; set; }

        public string AppendedBarcodeString { get; set; }
        public string ImageType { get; set; }

        public string UOM { get; set; }



        public string QuickListName { get; set; }
        public string QuickListGUID { get; set; }
        public double QuickListItemQTY { get; set; }

        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Double> ConsignedQuantity { get; set; }



        [Display(Name = "AverageCost", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Decimal> AverageCost { get; set; }

        public string BinNumber { get; set; }

        public long? BinID { get; set; }

        public double? CountCustomerOwnedQuantity { get; set; }
        public double? CountConsignedQuantity { get; set; }

        public string CountLineItemDescriptionEntry { get; set; }

        public Nullable<System.Int32> StockOutCount { get; set; }

        // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "CostUOMID", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int64> CostUOMID { get; set; }

        public int MonthValue { get; set; }

        public System.String WhatWhereAction { get; set; }
        //OnOrderQuantity
        [Display(Name = "OnReturnQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> OnReturnQuantity { get; set; }
        public bool IsBOMItem { get; set; }
        public long? RefBomId { get; set; }
        public Nullable<byte> TrendingSetting { get; set; }
        public bool PullQtyScanOverride { get; set; }
        public bool IsPackslipMandatoryAtReceive { get; set; }
        public bool IsAutoInventoryClassification { get; set; }

        public System.String CostUOMName { get; set; }

        public System.String Status { get; set; }
        public System.String Reason { get; set; }

        public string TrendingSettingName { get; set; }

        public Nullable<System.Double> DispExtendedCost { get; set; }
        public Nullable<System.Double> DispStagedQuantity { get; set; }
        public Nullable<System.Double> DispInTransitquantity { get; set; }
        public Nullable<System.Double> DispOnOrderQuantity { get; set; }
        public Nullable<System.Double> DispOnTransferQuantity { get; set; }
        public Nullable<System.Double> DispSuggestedOrderQuantity { get; set; }
        public Nullable<System.Double> DispRequisitionedQuantity { get; set; }
        public Nullable<System.Double> DispAverageUsage { get; set; }
        public Nullable<System.Double> DispTurns { get; set; }
        public Nullable<System.Double> DispOnHandQuantity { get; set; }

        [Display(Name = "ReceivedOn", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResItemMaster))]
        public System.String AddedFrom { get; set; }


        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResItemMaster))]
        public System.String EditedFrom { get; set; }

        [Display(Name = "ItemImageExternalURL", ResourceType = typeof(ResItemMaster))]
        [StringLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ItemImageExternalURL { get; set; }

        [Display(Name = "ItemDocExternalURL", ResourceType = typeof(ResItemMaster))]
        [StringLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ItemDocExternalURL { get; set; }


        [Display(Name = "ItemLink2ExternalURL", ResourceType = typeof(ResItemMaster))]
        [StringLength(4000, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ItemLink2ExternalURL { get; set; }

        public System.String ItemLink2ImageType { get; set; }
        public double? PerItemCost { get; set; }
        public double? OutTransferQuantity { get; set; }
        public double? OnOrderInTransitQuantity { get; set; }

        //eLabelKey
        [Display(Name = "eLabelKey", ResourceType = typeof(ResItemMaster))]
        [StringLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String eLabelKey { get; set; }

        //EnrichedProductData
        [Display(Name = "EnrichedProductData", ResourceType = typeof(ResItemMaster))]
        [StringLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EnrichedProductData { get; set; }

        //EnhancedDescription
        [Display(Name = "EnhancedDescription", ResourceType = typeof(ResItemMaster))]
        [StringLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EnhancedDescription { get; set; }
        public Nullable<int> POItemLineNumber { get; set; }
        public bool ISNullConsignment { get; set; }

    }
    [Serializable]
    public class KitDetailmain
    {

        public System.Int64 ID { get; set; }
        public Nullable<Guid> KitGUID { get; set; }
        public Nullable<Guid> ItemGUID { get; set; }
        public Nullable<System.Double> QuantityPerKit { get; set; }
        public Nullable<System.Double> QuantityReadyForAssembly { get; set; }
        //public Nullable<System.Double> AvailableItemsInWIP { get; set; }
        public Nullable<System.Double> AvailableKitQuantity { get; set; }
        public string Description { get; set; }
        //public Nullable<System.Double> KitDemand { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public System.Int64 CreatedBy { get; set; }
        public System.Int64 LastUpdatedBy { get; set; }
        public System.Int64 Room { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public System.Int64 CompanyID { get; set; }
        public Guid GUID { get; set; }

        public string KitPartNumber { get; set; }
        public string ItemNumber { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }

        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
        public Nullable<Boolean> IsBuildBreak { get; set; }
        public Nullable<System.Double> OnHandQuantity { get; set; }

        public Nullable<System.Double> CriticalQuantity { get; set; }
        public Nullable<System.Double> MinimumQuantity { get; set; }
        public Nullable<System.Double> MaximumQuantity { get; set; }
        public string ReOrderType { get; set; }
        public string KitCategory { get; set; }



        public string SupplierName { get; set; }
        public System.String SupplierPartNo { get; set; }

        public Nullable<System.Int64> DefaultLocation { get; set; }
        public string DefaultLocationName { get; set; }

        public System.String CostUOMName { get; set; }
        public Nullable<System.Int64> CostUOMID { get; set; }
        public Nullable<System.Int32> CostUOMValue { get; set; }
        public Nullable<System.Int64> UOMID { get; set; }
        public string UOM { get; set; }
        public Nullable<System.Double> DefaultReorderQuantity { get; set; }
        public Nullable<System.Double> DefaultPullQuantity { get; set; }
        //public Nullable<System.Int32> ItemType { get; set; }
        //public string ItemTypeName { get; set; }
        public bool? IsItemLevelMinMaxQtyRequired { get; set; }

        public Boolean SerialNumberTracking { get; set; }
        public Boolean LotNumberTracking { get; set; }
        public Boolean DateCodeTracking { get; set; }

        public Boolean IsActive { get; set; }
        public string BinNumber { get; set; }

    }
    public class ExportKitDTO
    {
        public string KitPartNumber { get; set; }
        public string ItemNumber { get; set; }
        public Double QuantityPerKit { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBuildBreak { get; set; }
        public Double OnHandQuantity { get; set; }

    }
    [Serializable]
    public class ItemManufacturer
    {
        public System.Int64 ID { get; set; }
        public Nullable<Boolean> IsDefault { get; set; }
        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResItemLocationQTY))]
        public Nullable<Guid> ItemGUID { get; set; }

        //ManufacturerID
        [Display(Name = "ManufacturerID", ResourceType = typeof(ResItemManufacturerDetails))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ManufacturerID { get; set; }

        //ManufacturerName
        [Display(Name = "ManufacturerName", ResourceType = typeof(ResItemManufacturerDetails))]
        [StringLength(150, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ManufacturerName { get; set; }

        //ManufacturerNumber
        [Display(Name = "ManufacturerNumber", ResourceType = typeof(ResItemManufacturerDetails))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ManufacturerNumber { get; set; }


        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
        public System.Int64 CreatedBy { get; set; }
        public System.Int64 LastUpdatedBy { get; set; }
        public System.Int64 Room { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public System.Int64 CompanyID { get; set; }
        public Guid GUID { get; set; }


        public string ItemNumber { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }

        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
    }
    [Serializable]
    public class ItemSupplier
    {
        public System.Int64 ID { get; set; }
        public Nullable<Boolean> IsDefault { get; set; }
        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResItemSupplierDetails))]
        public Nullable<Guid> ItemGUID { get; set; }

        //ManufacturerID
        [Display(Name = "SupplierID", ResourceType = typeof(ResItemSupplierDetails))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 SupplierID { get; set; }

        //ManufacturerName
        [Display(Name = "SupplierName", ResourceType = typeof(ResItemSupplierDetails))]
        [StringLength(150, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String SupplierName { get; set; }

        //ManufacturerNumber
        [Display(Name = "SupplierNumber", ResourceType = typeof(ResItemSupplierDetails))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String SupplierNumber { get; set; }


        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
        public System.Int64 CreatedBy { get; set; }
        public System.Int64 LastUpdatedBy { get; set; }
        public System.Int64 Room { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public System.Int64 CompanyID { get; set; }
        public Guid GUID { get; set; }


        public string ItemNumber { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }

        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }

        public long? BlanketPOID { get; set; }
        public string BlanketPOName { get; set; }
    }
    [Serializable]
    public class ImportBarcodeMaster
    {
        public System.Int64 ID { get; set; }

        [Display(Name = "ItemGUID", ResourceType = typeof(ResItemMaster))]
        public Guid RefGuid { get; set; }
        public string ItemNumber { get; set; }


        [Display(Name = "ItemGUID", ResourceType = typeof(ResItemMaster))]
        public Nullable<Guid> BinGuid { get; set; }
        public string BinNumber { get; set; }


        [Display(Name = "ModuleName")]
        public System.String ModuleName { get; set; }
        public Guid ModuleGuid { get; set; }


        [Display(Name = "BarcodeString", ResourceType = typeof(ResMessage))]
        [StringLength(512, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String BarcodeString { get; set; }


        public System.DateTime CreatedOn { get; set; }
        public System.DateTime UpdatedOn { get; set; }
        public System.Int64 CreatedBy { get; set; }
        public System.Int64 UpdatedBy { get; set; }
        public System.Int64 RoomID { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public System.Int64 CompanyID { get; set; }
        public Guid GUID { get; set; }



        public System.String Status { get; set; }
        public System.String Reason { get; set; }

        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
        public string BarcodeAdded { get; set; }

    }


    public class UDFMasterMain
    {
        public long ID { get; set; }
        public string ModuleName { get; set; }
        public string UDFColumnName { get; set; }
        public string UDFName { get; set; }
        public string ControlType { get; set; }
        public string DefaultValue { get; set; }
        public string OptionName { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IncludeInNarrowSearch { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }

    }
    public class ProjectMasterMain
    {
        public long ID { get; set; }
        public string ProjectSpendName { get; set; }
        public string Description { get; set; }
        public decimal? DollarLimitAmount { get; set; }
        public bool TrackAllUsageAgainstThis { get; set; }
        public bool IsClosed { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsArchived { get; set; }

        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string ItemNumber { get; set; }
        public decimal? ItemDollarLimitAmount { get; set; }
        public double? ItemQuantityLimitAmount { get; set; }
        public bool IsLineItemDeleted { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string WhatWhereAction { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
        public System.Int64 CreatedBy { get; set; }
        public System.Int64 LastUpdatedBy { get; set; }
        public System.Int64 Room { get; set; }
        public System.Int64 CompanyID { get; set; }
        public Guid GUID { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }

    }

    public class ImportModuleModel
    {
        public string[] arrcolumns { get; set; }
        public string CurModule { get; set; }
        public string CurModulevalue { get; set; }
    }
    [Serializable]
    public class WorkOrderMain
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public string WOName { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }

        public Nullable<Guid> CustomerGUID { get; set; }
        public Nullable<Int64> CustomerID { get; set; }

        public string Technician { get; set; }

        public Nullable<Int64> TechnicianID { get; set; }

        public Nullable<Guid> AssetGUID { get; set; }
        public string Asset { get; set; }

        public string Tool { get; set; }

        public Nullable<Guid> ToolGUID { get; set; }
        public string WOStatus { get; set; }
        public double? Odometer_OperationHours { get; set; }
        public Int32? UsedItems { get; set; }
        public float? UsedItemsCost { get; set; }
        public string WOType { get; set; }
        public string WhatWhereAction { get; set; }
        public string SignatureName { get; set; }

        public bool IsSignatureCapture { get; set; }

        public bool IsSignatureRequired { get; set; }
        public string Description { get; set; }

        public Nullable<Int64> SupplierId { get; set; }
        public string SupplierName { get; set; }

        public string Customer { get; set; }

        public Nullable<DateTime> ReceivedOn { get; set; }
        public Nullable<DateTime> ReceivedOnWeb { get; set; }


        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public string ReleaseNumber { get; set; }
        public string SupplierAccount { get; set; }
        public Nullable<Guid> SupplierAccountGuid { get; set; }
    }

    [Serializable]
    public class PullImport
    {
        public Int64 ID { get; set; }
        public string ItemNumber { get; set; }
        public string PullQuantity { get; set; }
        public string Location { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string ProjectSpendName { get; set; }
        public string PullOrderNumber { get; set; }
        public string WorkOrder { get; set; }
        public string Asset { get; set; }
        public string ActionType { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public string ItemSellPrice { get; set; }
    }
    [Serializable]
    public class PullImportWithSameQty
    {
        public Int64 ID { get; set; }
        public string ItemNumber { get; set; }
        public double PullQuantity { get; set; }
        public string BinNumber { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string ProjectSpendName { get; set; }
        public string PullOrderNumber { get; set; }
        public string WorkOrder { get; set; }
        public string Asset { get; set; }
        public string ActionType { get; set; }
        public string Created { get; set; }

        private DateTime? _createdDate;
        public DateTime? CreatedDate
        {
            get
            {
                if (_createdDate == null)
                {
                    DateTime CreatedDate;
                    if (DateTime.TryParse(Created, out CreatedDate))
                        _createdDate = CreatedDate;
                    if (_createdDate == null)
                    {
                        _createdDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Local);
                    }

                }
                return _createdDate;
            }
            set { this._createdDate = value; }
        }

        public string ItemCost { get; set; }
        public string CostUOMValue { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }

        public Guid? WorkOrderGuid { get; set; }
    }


    [Serializable]
    public class PastMaintenanceDueImport
    {
        public Nullable<Int64> ID { get; set; }
        public string ItemNumber { get; set; }
        public Nullable<Int64> Room { get; set; }

        public string ScheduleFor { get; set; }
        public string AssetName { get; set; }
        public string ToolName { get; set; }
        public string Serial { get; set; }
        public Nullable<Guid> ToolGUID { get; set; }
        public Nullable<Guid> AssetGUID { get; set; }
        public Nullable<Guid> SchedulerGUID { get; set; }
        public string SchedulerName { get; set; }
        public Nullable<DateTime> ScheduleDate { get; set; }
        public Nullable<DateTime> MaintenanceDate { get; set; }
        public Nullable<int> SchedulerType { get; set; }
        public string MaintenanceName { get; set; }
        public string MaintenanceType { get; set; }
        public string displayMaitenanceDate { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }


        public string WorkOrder { get; set; }
        public string ActionType { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Guid> GUID { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<double> ItemCost { get; set; }
        public Nullable<double> Quantity { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public int TrackngMeasurement { get; set; }
    }

    [Serializable]
    public class ItemLocationChangeImport
    {
        public Int64 ID { get; set; }
        public System.String ItemNumber { get; set; }
        public Guid? ItemGuid { get; set; }
        public long? OldLocation { get; set; }
        public string OldLocationName { get; set; }

        public Guid? OldLocationGUID { get; set; }

        public long? NewLocation { get; set; }

        public string NewLocationName { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }

        public Guid? NewLocationGUID { get; set; }

        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }
        public Nullable<System.Int64> CompanyID { get; set; }
        public string RoomName { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public virtual Nullable<System.DateTime> ReceivedOn { get; set; }
        public virtual Nullable<System.DateTime> ReceivedOnWeb { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public bool? IsDefault { get; set; }
        public Nullable<System.Int64> Room { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
    }

    [Serializable]
    public class AssetToolScheduler
    {
        public Int64 ID { get; set; }
        public System.String ItemNumber { get; set; }
        public Guid? ItemGUID { get; set; }

        public Double? Quantity { get; set; }

        public byte? ScheduleFor { get; set; }

        public string ScheduleForName { get; set; }

        public string SchedulerName { get; set; }

        public string SchedulerTypeName { get; set; }

        public int SchedulerType { get; set; }

        public Guid? GUID { get; set; }

        public string Description { get; set; }

        public Nullable<System.Double> OperationalHours { get; set; }
        public Nullable<System.Double> Mileage { get; set; }

        public Nullable<System.Int32> CheckOuts { get; set; }

        public int? TimeBasedFrequency { get; set; }

        public int? TimeBaseUnit { get; set; }

        public string TimeBasedUnitName { get; set; }

        public System.String UDF1 { get; set; }
        public System.String UDF2 { get; set; }
        public System.String UDF3 { get; set; }
        public System.String UDF4 { get; set; }
        public System.String UDF5 { get; set; }

        public Nullable<Boolean> IsDeleted { get; set; }

        public Nullable<Boolean> IsArchived { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }


        public Nullable<System.Int64> CompanyID { get; set; }

        public Nullable<System.Int64> Room { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
    }

    public class ResPullImport
    {
        private static string ResourceFileName = "ResPullImport";

        /// <summary>
        ///   Looks up a localized string similar to ItemNumber.
        /// </summary>
        public static string ItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNumber", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to PullCreditQuantity.
        /// </summary>
        public static string PullQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("PullQuantity", ResourceFileName);
            }
        }
        public static string BinNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("BinNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Location.
        /// </summary>
        public static string Location
        {
            get
            {
                return ResourceRead.GetResourceValue("Location", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", ResourceFileName, true);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", ResourceFileName, true);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", ResourceFileName, true);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", ResourceFileName, true);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", ResourceFileName, true);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to ProjectSpendName.
        /// </summary>
        public static string ProjectSpendName
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpendName", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to PullOrderNumber.
        /// </summary>
        public static string PullOrderNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("PullOrderNumber", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to WorkOrder.
        /// </summary>
        public static string WorkOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("WorkOrder", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Asset.
        /// </summary>
        public static string Asset
        {
            get
            {
                return ResourceRead.GetResourceValue("Asset", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ActionType.
        /// </summary>
        public static string ActionType
        {
            get
            {
                return ResourceRead.GetResourceValue("ActionType", ResourceFileName);
            }
        }

        public static string Created
        {
            get
            {
                return ResourceRead.GetResourceValue("Created", ResourceFileName);
            }
        }
        public static string ItemCost
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemCost", ResourceFileName);
            }
        }
        public static string ItemSellPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemSellPrice", ResourceFileName);
            }
        }
        public static string CostUOMValue
        {
            get
            {
                return ResourceRead.GetResourceValue("CostUOMValue", ResourceFileName);
            }
        }

        public static string SerialNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialNumber", ResourceFileName);
            }
        }
        public static string LotNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("LotNumber", ResourceFileName);
            }
        }
        public static string ExpirationDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ExpirationDate", ResourceFileName);
            }
        }
    }

    [Serializable]
    public class ToolAssetQuantityMain
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public string BinNumber { get; set; }

        public long CreatedBy { get; set; }

        public long UpdatedBy { get; set; }

        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }

        public Nullable<System.Double> Quantity { get; set; }

        public Nullable<Guid> ToolGUID { get; set; }

        public Nullable<Guid> AssetGUID { get; set; }

        public string SerialNumber { get; set; }
        public string Received { get; set; }


        public Nullable<System.Double> Cost { get; set; }
        public string ToolName { get; set; }

        public string AssetName { get; set; }

        public System.String Status { get; set; }
        public System.String Reason { get; set; }




    }
    [Serializable]
    public class PullImportWithLotSerial
    {
        public Int64 ID { get; set; }
        public string ItemNumber { get; set; }
        public string PullQuantity { get; set; }
        public string Location { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string ProjectSpendName { get; set; }
        public string PullOrderNumber { get; set; }
        public string WorkOrder { get; set; }
        public string Asset { get; set; }
        public string LotNumber { get; set; }
        public string SerialNumber { get; set; }
        public string ExpirationDate { get; set; }
        public int ItemTrackingType { get; set; }
        public string ActionType { get; set; }
        public string ItemSellPrice { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
    }

    /// <summary>
    /// This class is used for MoveMaterial import
    /// </summary>
    public class MoveMaterial
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string SourceBin { get; set; }
        public string DestinationBin { get; set; }
        public string MoveType { get; set; }
        public double Quantity { get; set; }
        public string DestinationStagingHeader { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }

    public class EnterpriseQLImport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string QLDetailNumber { get; set; }
        public double? Quantity { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }

    public class RequisitionImport
    {
        public int Id { get; set; }
        public string RequisitionNumber { get; set; }
        public string Workorder { get; set; }
        public string RequiredDate { get; set; }
        public string RequisitionStatus { get; set; }
        public string CustomerName { get; set; }
        public string ReleaseNumber { get; set; }
        public string ProjectSpend { get; set; }
        public string Description { get; set; }
        public string StagingName { get; set; }
        public string Supplier { get; set; }
        public string SupplierAccount { get; set; }
        public string BillingAccount { get; set; }
        public string Technician { get; set; }
        public string RequisitionUDF1 { get; set; }
        public string RequisitionUDF2 { get; set; }
        public string RequisitionUDF3 { get; set; }
        public string RequisitionUDF4 { get; set; }
        public string RequisitionUDF5 { get; set; }
        public string ItemNumber { get; set; }
        public string Tool { get; set; }
        public string ToolSerial { get; set; }
        public string Bin { get; set; }
        public double? QuantityRequisitioned { get; set; }
        public double? QuantityApproved { get; set; }
        public double? QuantityPulled { get; set; }
        public string LineItemRequiredDate { get; set; }
        public string LineItemProjectSpend { get; set; }
        public string LineItemSupplierAccount { get; set; }
        public string PullOrderNumber { get; set; }
        public string LineItemTechnician { get; set; }
        public string PullUDF1 { get; set; }
        public string PullUDF2 { get; set; }
        public string PullUDF3 { get; set; }
        public string PullUDF4 { get; set; }
        public string PullUDF5 { get; set; }
        public string ToolCheckoutUDF1 { get; set; }
        public string ToolCheckoutUDF2 { get; set; }
        public string ToolCheckoutUDF3 { get; set; }
        public string ToolCheckoutUDF4 { get; set; }
        public string ToolCheckoutUDF5 { get; set; }
        public Guid? ItemGuid { get; set; }
        public Guid? ToolGuid { get; set; }
        public DateTime RequiredDt { get; set; }
        public DateTime? LineItemRequiredDt { get; set; }
        public double? ItemCost { get; set; }
        public double? ItemSellPrice { get; set; }
        public long? RequisitionId { get; set; }
        public Guid? RequisitionGuid { get; set; }
        //public double? TotalCost { get; set; }
        //public double? TotalSellPrice { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
    [Serializable]
    public class QuoteMasterItemsMain
    {

        public System.String QuoteNumber { get; set; }
        public System.String RequiredDate { get; set; }
        public System.String QuoteStatus { get; set; }
        public System.String QuoteComment { get; set; }
        public System.String CustomerName { get; set; }
        public System.String PackSlipNumber { get; set; }
        public System.String ShippingTrackNumber { get; set; }
        public System.String QuoteUDF1 { get; set; }
        public System.String QuoteUDF2 { get; set; }
        public System.String QuoteUDF3 { get; set; }
        public System.String QuoteUDF4 { get; set; }
        public System.String QuoteUDF5 { get; set; }
        public System.String ShipVia { get; set; }
        public System.String ShippingVendor { get; set; }
        public System.String SupplierAccount { get; set; }
        public System.String ItemNumber { get; set; }
        public System.String Bin { get; set; }
        public Nullable<System.Double> RequestedQty { get; set; }
        public System.String ASNNumber { get; set; }
        public Nullable<System.Double> ApprovedQty { get; set; }
        public Nullable<System.Double> InTransitQty { get; set; }
        //public System.String IsCloseItem { get; set; }
        public Boolean IsCloseItem { get; set; }
        public System.String LineNumber { get; set; }
        public System.String ControlNumber { get; set; }
        public System.String ItemComment { get; set; }
        public System.String LineItemUDF1 { get; set; }
        public System.String LineItemUDF2 { get; set; }
        public System.String LineItemUDF3 { get; set; }
        public System.String LineItemUDF4 { get; set; }
        public System.String LineItemUDF5 { get; set; }

        public System.Int64 ID { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }
        public Guid? QuoteGUID { get; set; }
        public Guid? ItemGUID { get; set; }
        public Nullable<System.Int64> RequesterID { get; set; }
        public Nullable<System.Int64> ApproverID { get; set; }
        public Nullable<System.Double> QuoteCost { get; set; }
        public string SupplierName { get;set;}
        public string QuoteSupplierIdsCSV { get;set;}
    }

    public class SupplierCatalogImport
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        public double? SellPrice { get; set; }
        public double? PackingQuantity { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public string ImagePath { get; set; }
        public string UPC { get; set; }
        public string SupplierPartNumber { get; set; }
        public string SupplierName { get; set; }
        public string ManufacturerName { get; set; }
        public string ConcatedColumnText { get; set; }
        public string UOM { get; set; }
        public string CostUOM { get; set; }
        public double? Cost { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string UNSPSC { get; set; }
        public string LongDescription { get; set; }
        public string Category { get; set; }
    }
}
