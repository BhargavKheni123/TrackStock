using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class ImportMastersNewDTO
    {
        public enum TableName
        {
            Room,
            [Description("Adjustment Count")]
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
            [Description("Tool Locations")]
            LocationMaster,
            ToolCategoryMaster,
            CostUOMMaster,
            InventoryClassificationMaster,
            ToolMaster,
            AssetMaster,
            QuickListItems,
            InventoryLocation,
            BOMItemMaster,
            WorkOrder,
            AssetCategoryMaster,
            kitdetail,
            [Description("Item Locations")]
            ItemLocationeVMISetup,
            SupplierBlanketPODetails,
            ToolCheckInOutHistory,
            ItemManufacturerDetails,
            ItemSupplierDetails,
            [Description("Barcode Associations")]
            BarcodeMaster,
            UDF,
            ProjectMaster,
            ItemLocationQty,
            ManualCount,
            PullMaster,
            OrderMaster
        }

        public class ImportDataChange
        {
            public int RowIndex { get; set; }
            public string FieldName { get; set; }
            public object Value { get; set; }
        }

        public class ImportMaster
        {
            public Int32 ImportModule { get; set; }
            public HttpPostedFileBase UploadFile { get; set; }
            public HttpPostedFileBase UploadZIPFile { get; set; }
            public HttpPostedFileBase UploadZIPFile2 { get; set; }
            public bool IsFileSuccessfulyUploaded { get; set; }
            public ImportPageDTO objImportPageDTO { get; set; }
        }

        public class ImportPageDTO : IDisposable
        {
            public string[] DataTableColumns { get; set; }
            public List<ItemMasterImport> lstItemMasterImportData { get; set; }

            public List<CategoryMasterImport> lstCategoryMasterImportData { get; set; }

            public List<CustomerMasterImport> lstCustomerMasterImportData { get; set; }

            public List<AssetMasterImport> lstAssetMasterImportData { get; set; }

            public List<ToolMasterImport> lstToolMasterImportData { get; set; }

            public List<SupplierMasterImport> lstSupplierMasterImportData { get; set; }

            public List<QuickListItemsImport> lstQuickListItemsImportData { get; set; }

            public List<InventoryLocationImport> lstInventoryLocationImportData { get; set; }

            public List<WorkOrderImport> lstWorkOrderImportData { get; set; }

            public List<BOMItemMasterImport> lstBOMItemMasterImportData { get; set; }

            public List<KitDetailImport> lstKitDetailImportData { get; set; }

            public List<ProjectMasterImport> lstProjectMasterImportData { get; set; }

            public List<InventoryLocationQuantityImport> lstInventoryLocationQuantityImportData { get; set; }

            public List<ItemManufacturerImport> lstItemManufacturerImportData { get; set; }

            public List<ItemSupplierImport> lstItemSupplierImportData { get; set; }

            public List<BarcodeMasterImport> lstBarcodeMasterImportData { get; set; }

            public int RecordCount { get; set; }

            public void Dispose()
            {
                //throw new NotImplementedException();
            }
        }

        #region [Item Master Import]
        public class ItemMasterImport
        {
            //ID
            public System.Int64 ID { get; set; }

            //ItemNumber
            [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
            [StringLength(100, ErrorMessage = "Item number can not be more then 100 character.")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ItemNumber { get; set; }

            //ManufacturerID
            [Display(Name = "ManufacturerID", ResourceType = typeof(ResItemMaster))]
            public Nullable<System.Int64> ManufacturerID { get; set; }

            //ManufacturerNumber
            [Display(Name = "ManufacturerNumber", ResourceType = typeof(ResItemMaster))]
            [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ManufacturerNumber { get; set; }

            //SupplierID
            [Display(Name = "SupplierID", ResourceType = typeof(ResItemMaster))]
            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.Int64? SupplierID { get; set; }
            public System.Int64? BlanketPOID { get; set; }

            //SupplierPartNo
            [Display(Name = "SupplierPartNo", ResourceType = typeof(ResItemMaster))]
            [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [RequiredIf("ItemTypeName", "Item", ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String SupplierPartNo { get; set; }

            //BlanketOrderNumber
            [Display(Name = "BlanketOrderNumber", ResourceType = typeof(ResItemMaster))]
            public System.String BlanketOrderNumber { get; set; }

            //UPC
            [Display(Name = "UPC", ResourceType = typeof(ResItemMaster))]
            [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
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
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Decimal> PricePerTerm { get; set; }

            //DefaultReorderQuantity
            [Display(Name = "DefaultReorderQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //[DefaultReorderQuantityCheck("MaximumQuantity", ErrorMessage = "Default Reorder quantity must be less then Maximum quantity")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> DefaultReorderQuantity { get; set; }

            //DefaultPullQuantity
            [Display(Name = "DefaultPullQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> DefaultPullQuantity { get; set; }

            ////DefaultCartQuantity
            //[Display(Name = "DefaultCartQuantity", ResourceType = typeof(ResItemMaster))]
            //[RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //public Nullable<System.Double> DefaultCartQuantity { get; set; }

            //Cost
            [Display(Name = "Cost", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> Cost { get; set; }

            //Markup
            [Display(Name = "Markup", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //[Range(float.MinValue, float.MaxValue, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> Markup { get; set; }

            //SellPrice
            [Display(Name = "SellPrice", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]

            public Nullable<System.Double> SellPrice { get; set; }

            //ExtendedCost
            [Display(Name = "ExtendedCost", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> ExtendedCost { get; set; }

            //LeadTimeInDays
            [Display(Name = "LeadTimeInDays", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Int32> LeadTimeInDays { get; set; }

            //Link1
            [Display(Name = "Link1", ResourceType = typeof(ResItemMaster))]
            [StringLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String Link1 { get; set; }

            //Link2
            [Display(Name = "Link2", ResourceType = typeof(ResItemMaster))]
            [StringLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"([a-z0-9A-Z\s]*)\.(jpg|jpeg|bmp|pdf|xls|doc|xlsx|docx|png|gif)$", ErrorMessageResourceName = "InvalidFilename", ErrorMessageResourceType = typeof(ResMessage))]
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

            //StagedQuantity
            [Display(Name = "StagedQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> StagedQuantity { get; set; }

            //InTransitquantity
            [Display(Name = "InTransitquantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> InTransitquantity { get; set; }

            //OnOrderQuantity
            [Display(Name = "OnOrderQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> OnOrderQuantity { get; set; }



            //OnTransferQuantity
            [Display(Name = "OnTransferQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> OnTransferQuantity { get; set; }

            //SuggestedOrderQuantity
            [Display(Name = "SuggestedOrderQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> SuggestedOrderQuantity { get; set; }

            //RequisitionedQuantity
            [Display(Name = "RequisitionedQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> RequisitionedQuantity { get; set; }

            ////PackingQuantity
            //[Display(Name = "PackingQuantity", ResourceType = typeof(ResItemMaster))]
            //[RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //public Nullable<System.Double> PackingQuantity { get; set; }


            //AverageUsage
            [Display(Name = "AverageUsage", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> AverageUsage { get; set; }

            //Turns
            [Display(Name = "Turns", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> Turns { get; set; }

            //OnHandQuantity        
            [Display(Name = "OnHandQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> OnHandQuantity { get; set; }

            //CriticalQuantity
            [Display(Name = "CriticalQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [CriticleQuantityCheck("MinimumQuantity", ErrorMessage = "Critical quantity must be less then Minimum quantity")]
            [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
            public System.Double CriticalQuantity { get; set; }

            //MinimumQuantity
            [Display(Name = "MinimumQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [MinimumQuantityCheck("MaximumQuantity", ErrorMessage = "Minimum quantity must be less then Maximum quantity")]
            [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
            public System.Double MinimumQuantity { get; set; }

            //MaximumQuantity
            [Display(Name = "MaximumQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
            public System.Double MaximumQuantity { get; set; }

            //WeightPerPiece
            [Display(Name = "WeightPerPiece", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
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

            public Boolean IsAllowOrderCostuom { get; set; }

            ////DefaultLocation
            //[Display(Name = "DefaultLocation", ResourceType = typeof(ResItemMaster))]
            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            //public long DefaultLocation { get; set; }


            ////DefaultLocation
            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            //[Display(Name = "DefaultLocation", ResourceType = typeof(ResItemMaster))]
            //public string DefaultLocationName { get; set; }


            ////InventoryClassification
            //[Display(Name = "InventoryClassification", ResourceType = typeof(ResItemMaster))]
            //public Nullable<System.Int32> InventoryClassification { get; set; }


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
            public System.Int32 ItemType { get; set; }

            //ImagePath
            [Display(Name = "ImagePath", ResourceType = typeof(ResItemMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"([a-z0-9A-Z\s]*)\.(jpg|jpeg|bmp|png|gif)$", ErrorMessageResourceName = "InvalidFilename", ErrorMessageResourceType = typeof(ResMessage))]
            //
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
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            //Room
            [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> Room { get; set; }

            //IsLotSerialExpiryCost
            [Display(Name = "IsLotSerialExpiryCost", ResourceType = typeof(ResItemMaster))]
            [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String IsLotSerialExpiryCost { get; set; }

            //PackingQuantity
            [Display(Name = "PackingQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> PackingQuantity { get; set; }

            [Display(Name = "IsItemLevelMinMaxQtyRequired", ResourceType = typeof(ResItemMaster))]
            public bool IsItemLevelMinMaxQtyRequired { get; set; }

            //[Display(Name="Enforce Default Reorder Quantity")]
            [Display(Name = "IsEnforceDefaultReorderQuantity", ResourceType = typeof(ResItemMaster))]
            public bool? IsEnforceDefaultReorderQuantity { get; set; }

            //IsBuildBreak
            [Display(Name = "IsBuildBreak", ResourceType = typeof(ResItemMaster))]
            public bool IsBuildBreak { get; set; }

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

            [Display(Name = "Supplier", ResourceType = typeof(ResItemMaster))]
            [RequiredIf("ItemTypeName", "Item", ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string SupplierName { get; set; }

            [Display(Name = "ItemType", ResourceType = typeof(ResItemMaster))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string ItemTypeName { get; set; }

            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string Unit { get; set; }

            public string GLAccount { get; set; }

            [Display(Name = "DefaultLocation", ResourceType = typeof(ResItemMaster))]
            [Required(ErrorMessage = "Default bin required.")]
            public string InventryLocation { get; set; }

            public string AppendedBarcodeString { get; set; }
            public string ImageType { get; set; }





            public string QuickListName { get; set; }
            public string QuickListGUID { get; set; }
            public double QuickListItemQTY { get; set; }

            [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResItemLocationDetails))]
            public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

            [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResItemLocationDetails))]
            public Nullable<System.Double> ConsignedQuantity { get; set; }



            [Display(Name = "AverageCost", ResourceType = typeof(ResItemMaster))]
            public Nullable<System.Decimal> AverageCost { get; set; }

            //public string BinNumber { get; set; }

            public long? BinID { get; set; }

            public double? CountCustomerOwnedQuantity { get; set; }
            public double? CountConsignedQuantity { get; set; }

            public string CountLineItemDescriptionEntry { get; set; }

            public Nullable<System.Int32> StockOutCount { get; set; }

            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "CostUOMID", ResourceType = typeof(ResItemMaster))]
            public Nullable<System.Int64> CostUOMID { get; set; }

            public int MonthValue { get; set; }

            public System.String WhatWhereAction { get; set; }
            //OnOrderQuantity
            [Display(Name = "OnReturnQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> OnReturnQuantity { get; set; }
            public bool IsBOMItem { get; set; }
            public long? RefBomId { get; set; }
            //public Nullable<byte> TrendingSetting { get; set; }
            public bool PullQtyScanOverride { get; set; }
            public bool IsPackslipMandatoryAtReceive { get; set; }
            public bool IsAutoInventoryClassification { get; set; }

            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "CostUOMName", ResourceType = typeof(ResItemMaster))]
            public System.String CostUOMName { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }
            public int Index { get; set; }

            public string TrendingSettingName { get; set; }

            //public Nullable<System.Double> DispExtendedCost { get; set; }
            //public Nullable<System.Double> DispStagedQuantity { get; set; }
            //public Nullable<System.Double> DispInTransitquantity { get; set; }
            //public Nullable<System.Double> DispOnOrderQuantity { get; set; }
            //public Nullable<System.Double> DispOnTransferQuantity { get; set; }
            //public Nullable<System.Double> DispSuggestedOrderQuantity { get; set; }
            //public Nullable<System.Double> DispRequisitionedQuantity { get; set; }
            //public Nullable<System.Double> DispAverageUsage { get; set; }
            //public Nullable<System.Double> DispTurns { get; set; }
            //public Nullable<System.Double> DispOnHandQuantity { get; set; }

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
            [RegularExpression(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", ErrorMessageResourceName = "InvalidURL", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ItemImageExternalURL { get; set; }

            [Display(Name = "ItemDocExternalURL", ResourceType = typeof(ResItemMaster))]
            [StringLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", ErrorMessageResourceName = "InvalidURL", ErrorMessageResourceType = typeof(ResMessage))]

            public System.String ItemDocExternalURL { get; set; }


            [Display(Name = "ItemLink2ExternalURL", ResourceType = typeof(ResItemMaster))]
            [StringLength(4000, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", ErrorMessageResourceName = "InvalidURL", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ItemLink2ExternalURL { get; set; }

            public System.String ItemLink2ImageType { get; set; }
        }
        #endregion

        #region [Category Master Import]
        public class CategoryMasterImport
        {

            //ID
            public System.Int64 ID { get; set; }

            //Category
            [Display(Name = "Category", ResourceType = typeof(ResCategoryMaster))]
            [StringLength(100, ErrorMessage = "Category Name can not be more then 100 character.")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String Category { get; set; }

            //UDF1
            [Display(Name = "UDF1", ResourceType = typeof(ResCategoryMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF1 { get; set; }

            //UDF2
            [Display(Name = "UDF2", ResourceType = typeof(ResCategoryMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF2 { get; set; }

            //UDF3
            [Display(Name = "UDF3", ResourceType = typeof(ResCategoryMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF3 { get; set; }

            //UDF4
            [Display(Name = "UDF4", ResourceType = typeof(ResCategoryMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF4 { get; set; }

            //UDF5
            [Display(Name = "UDF5", ResourceType = typeof(ResCategoryMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF5 { get; set; }


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
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            //Room
            [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> Room { get; set; }

            [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
            public string RoomName { get; set; }

            [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
            public string CreatedByName { get; set; }

            [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
            public string UpdatedByName { get; set; }

            [Display(Name = "ReceivedOn", ResourceType = typeof(ResCategoryMaster))]
            public Nullable<System.DateTime> ReceivedOn { get; set; }

            [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCategoryMaster))]
            public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

            //AddedFrom
            [Display(Name = "AddedFrom", ResourceType = typeof(ResCategoryMaster))]
            public System.String AddedFrom { get; set; }


            //EditedFrom
            [Display(Name = "EditedFrom", ResourceType = typeof(ResCategoryMaster))]
            public System.String EditedFrom { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }

            public int Index { get; set; }
        }
        #endregion

        #region [Customer Master Import]
        public class CustomerMasterImport
        {

            //ID
            public System.Int64 ID { get; set; }

            //Customer
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "Customer", ResourceType = typeof(ResCustomer))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public string Customer { get; set; }

            //Account
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "Account", ResourceType = typeof(ResCustomer))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public string Account { get; set; }

            /// <summary>
            /// Contact
            /// </summary>
            [Display(Name = "Contact", ResourceType = typeof(ResCustomer))]
            [StringLength(512, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public string Contact { get; set; }

            /// <summary>
            /// Address
            /// </summary>
            [Display(Name = "Address", ResourceType = typeof(ResCommon))]
            [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public string Address { get; set; }

            /// <summary>
            /// City
            /// </summary>
            [Display(Name = "City", ResourceType = typeof(ResCommon))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public string City { get; set; }

            /// <summary>
            /// State
            /// </summary>
            [Display(Name = "State", ResourceType = typeof(ResCommon))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public string State { get; set; }

            /// <summary>
            /// ZipCode
            /// </summary>
            [Display(Name = "ZipCode", ResourceType = typeof(ResCommon))]
            [StringLength(24, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public string ZipCode { get; set; }

            /// <summary>
            /// Country
            /// </summary>
            [Display(Name = "Country", ResourceType = typeof(ResCommon))]
            [StringLength(64, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public string Country { get; set; }

            /// <summary>
            /// Phone
            /// </summary>
            [Display(Name = "Phone", ResourceType = typeof(ResCommon))]
            [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public string Phone { get; set; }

            /// <summary>
            /// Email
            /// </summary>
            [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(ResMessage))]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Email", ResourceType = typeof(ResCommon))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public string Email { get; set; }


            //UDF1
            [Display(Name = "UDF1", ResourceType = typeof(ResCustomer))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF1 { get; set; }

            //UDF2
            [Display(Name = "UDF2", ResourceType = typeof(ResCustomer))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF2 { get; set; }

            //UDF3
            [Display(Name = "UDF3", ResourceType = typeof(ResCustomer))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF3 { get; set; }

            //UDF4
            [Display(Name = "UDF4", ResourceType = typeof(ResCustomer))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF4 { get; set; }

            //UDF5
            [Display(Name = "UDF5", ResourceType = typeof(ResCustomer))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF5 { get; set; }


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
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            //Room
            [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> Room { get; set; }

            [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
            public string RoomName { get; set; }

            [Display(Name = "CreatedBy", ResourceType = typeof(ResCustomer))]
            public string CreatedByName { get; set; }

            [Display(Name = "UpdatedBy", ResourceType = typeof(ResCustomer))]
            public string UpdatedByName { get; set; }

            [Display(Name = "ReceivedOn", ResourceType = typeof(ResCustomer))]
            public Nullable<System.DateTime> ReceivedOn { get; set; }

            [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCustomer))]
            public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

            //AddedFrom
            [Display(Name = "AddedFrom", ResourceType = typeof(ResCustomer))]
            public System.String AddedFrom { get; set; }


            //EditedFrom
            [Display(Name = "EditedFrom", ResourceType = typeof(ResCustomer))]
            public System.String EditedFrom { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }

            public int Index { get; set; }
        }
        #endregion

        #region [Asset Master Import]
        public class AssetMasterImport
        {

            //ID
            public System.Int64 ID { get; set; }

            //AssetName
            [Display(Name = "AssetName", ResourceType = typeof(ResAssetMaster))]
            [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String AssetName { get; set; }

            //Description
            [Display(Name = "Description", ResourceType = typeof(ResAssetMaster))]
            [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String Description { get; set; }

            //Make
            [Display(Name = "Make", ResourceType = typeof(ResAssetMaster))]
            [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String Make { get; set; }

            //Model
            [Display(Name = "Model", ResourceType = typeof(ResAssetMaster))]
            [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String Model { get; set; }

            //Serial
            [Display(Name = "Serial", ResourceType = typeof(ResAssetMaster))]
            [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String Serial { get; set; }

            //ToolCategoryID
            [Display(Name = "ToolCategoryID", ResourceType = typeof(ResAssetMaster))]
            public Nullable<System.Int64> ToolCategoryID { get; set; }

            [Display(Name = "ToolCategory", ResourceType = typeof(ResAssetMaster))]
            public string ToolCategoryName { get; set; }

            //PurchaseDate
            [Display(Name = "PurchaseDate", ResourceType = typeof(ResAssetMaster))]
            public Nullable<System.DateTime> PurchaseDate { get; set; }

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

            //PurchasePrice
            [Display(Name = "PurchasePrice", ResourceType = typeof(ResAssetMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> PurchasePrice { get; set; }

            //DepreciatedValue
            [Display(Name = "DepreciatedValue", ResourceType = typeof(ResAssetMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> DepreciatedValue { get; set; }

            //SuggestedMaintenanceDate
            [Display(Name = "SuggestedMaintenanceDate", ResourceType = typeof(ResAssetMaster))]
            public Nullable<System.DateTime> SuggestedMaintenanceDate { get; set; }

            [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
            public string Action { get; set; }

            [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
            public Int64 HistoryID { get; set; }

            //UDF1
            [Display(Name = "UDF1", ResourceType = typeof(ResAssetMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF1 { get; set; }

            //UDF2
            [Display(Name = "UDF2", ResourceType = typeof(ResAssetMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF2 { get; set; }

            //UDF3
            [Display(Name = "UDF3", ResourceType = typeof(ResAssetMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF3 { get; set; }

            //UDF4
            [Display(Name = "UDF4", ResourceType = typeof(ResAssetMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF4 { get; set; }

            //UDF5
            [Display(Name = "UDF5", ResourceType = typeof(ResAssetMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF5 { get; set; }

            [Display(Name = "UDF6", ResourceType = typeof(ResAssetMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF6 { get; set; }

            [Display(Name = "UDF7", ResourceType = typeof(ResAssetMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF7 { get; set; }

            [Display(Name = "UDF8", ResourceType = typeof(ResAssetMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF8 { get; set; }

            [Display(Name = "UDF9", ResourceType = typeof(ResAssetMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF9 { get; set; }

            [Display(Name = "UDF10", ResourceType = typeof(ResAssetMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF10 { get; set; }

            //AssetCategoryID
            [Display(Name = "AssetCategoryID", ResourceType = typeof(ResAssetMaster))]
            public Nullable<System.Int64> AssetCategoryID { get; set; }

            [Display(Name = "AssetCategory", ResourceType = typeof(ResAssetMaster))]
            public string AssetCategoryName { get; set; }

            [Display(Name = "NoOfPastMntsToConsider", ResourceType = typeof(ResAssetMaster))]
            [Range(2, 10, ErrorMessageResourceName = "NoOfPastMntsToConsider", ErrorMessageResourceType = typeof(ResAssetMaster))]
            public int? NoOfPastMntsToConsider { get; set; }

            [Display(Name = "MaintenanceType", ResourceType = typeof(ResAssetMaster))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public int MaintenanceType { get; set; }
            public int DaysDiff { get; set; }

            public Nullable<Boolean> IsAutoMaintain { get; set; }



            public string ImageType { get; set; }

            //ImagePath
            [Display(Name = "ImagePath", ResourceType = typeof(ResAssetMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"^[a-zA-Z0-9](?:[a-zA-Z0-9 ._-]*[a-zA-Z0-9])?\.(jpg|JPG|png|PNG|gif|GIF|bmp|BMP|jpeg|JPEG)+$", ErrorMessageResourceName = "InvalidFilename", ErrorMessageResourceType = typeof(ResMessage))]
            //
            public System.String ImagePath { get; set; }

            [Display(Name = "AssetImageExternalURL", ResourceType = typeof(ResAssetMaster))]
            [StringLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", ErrorMessageResourceName = "InvalidURL", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String AssetImageExternalURL { get; set; }


            public System.String MaintenanceName { get; set; }


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
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            //Room
            [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> Room { get; set; }

            [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
            public string RoomName { get; set; }

            [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
            public string CreatedByName { get; set; }

            [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
            public string UpdatedByName { get; set; }

            [Display(Name = "ReceivedOn", ResourceType = typeof(ResAssetMaster))]
            public Nullable<System.DateTime> ReceivedOn { get; set; }

            [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResAssetMaster))]
            public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

            //AddedFrom
            [Display(Name = "AddedFrom", ResourceType = typeof(ResAssetMaster))]
            public System.String AddedFrom { get; set; }


            //EditedFrom
            [Display(Name = "EditedFrom", ResourceType = typeof(ResAssetMaster))]
            public System.String EditedFrom { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }

            public int Index { get; set; }
        }
        #endregion

        #region [Tool Master Import]
        public class ToolMasterImport
        {

            //ID
            public System.Int64 ID { get; set; }

            //Tool Name
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "ToolName", ResourceType = typeof(ResToolMaster))]
            [AllowHtml]
            public string ToolName { get; set; }


            //Tool Serial
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "Serial", ResourceType = typeof(ResToolMaster))]
            [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public string Serial { get; set; }

            //Tool Description
            [Display(Name = "Description", ResourceType = typeof(ResToolMaster))]
            [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public string Description { get; set; }

            //ToolCategoryID
            [Display(Name = "ToolCategoryID", ResourceType = typeof(ResToolMaster))]
            public Nullable<System.Int64> ToolCategoryID { get; set; }

            [Display(Name = "ToolCategory", ResourceType = typeof(ResToolMaster))]
            public string ToolCategoryName { get; set; }

            //Tool Value
            [Display(Name = "Cost", ResourceType = typeof(ResToolMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public double? Cost { get; set; }

            //Tool Checked Out?
            [Display(Name = "IsCheckedOut", ResourceType = typeof(ResToolMaster))]
            public Nullable<bool> IsCheckedOut { get; set; }


            //Tool Category Name
            [Display(Name = "Location", ResourceType = typeof(ResToolMaster))]
            public Nullable<Int64> LocationID { get; set; }

            //Tool Category Name
            [Display(Name = "TechnicianGuID", ResourceType = typeof(ResToolMaster))]
            public Nullable<Guid> TechnicianGuID { get; set; }



            [Display(Name = "Location", ResourceType = typeof(ResToolMaster))]
            public string Location { get; set; }

            [Display(Name = "Technician", ResourceType = typeof(ResToolMaster))]
            public string Technician { get; set; }

            public string TechnicianCode { get; set; }

            [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
            public string Action { get; set; }

            [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
            public Int64 HistoryID { get; set; }

            //UDF1
            [Display(Name = "UDF1", ResourceType = typeof(ResToolMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF1 { get; set; }

            //UDF2
            [Display(Name = "UDF2", ResourceType = typeof(ResToolMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF2 { get; set; }

            //UDF3
            [Display(Name = "UDF3", ResourceType = typeof(ResToolMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF3 { get; set; }

            //UDF4
            [Display(Name = "UDF4", ResourceType = typeof(ResToolMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF4 { get; set; }

            //UDF5
            [Display(Name = "UDF5", ResourceType = typeof(ResToolMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF5 { get; set; }

            [Display(Name = "UDF6", ResourceType = typeof(ResToolMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF6 { get; set; }

            [Display(Name = "UDF7", ResourceType = typeof(ResToolMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF7 { get; set; }

            [Display(Name = "UDF8", ResourceType = typeof(ResToolMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF8 { get; set; }

            [Display(Name = "UDF9", ResourceType = typeof(ResToolMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF9 { get; set; }

            [Display(Name = "UDF10", ResourceType = typeof(ResToolMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF10 { get; set; }


            [Display(Name = "Quantity", ResourceType = typeof(ResToolMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.Double Quantity { get; set; }

            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "IsGroupOfItems", ResourceType = typeof(ResToolMaster))]
            public Nullable<Int32> IsGroupOfItems { get; set; }

            private bool _IsGroupOfItemsBool;
            public bool IsGroupOfItemsBool
            {
                get
                {
                    if (IsGroupOfItems.HasValue && IsGroupOfItems > 0)
                    {
                        _IsGroupOfItemsBool = true;
                    }
                    else
                    {
                        _IsGroupOfItemsBool = false;
                    }
                    return _IsGroupOfItemsBool;
                }
                set
                {
                    this._IsGroupOfItemsBool = value;
                }
            }


            [Display(Name = "CheckOutStatus", ResourceType = typeof(ResToolCheckInOutHistory))]
            public string CheckOutStatus { get; set; }

            [Display(Name = "CheckedOutQTY", ResourceType = typeof(ResToolCheckInOutHistory))]
            public Nullable<System.Double> CheckedOutQTY { get; set; }

            [Display(Name = "CheckedOutMQTY", ResourceType = typeof(ResToolCheckInOutHistory))]
            public Nullable<System.Double> CheckedOutMQTY { get; set; }


            [Display(Name = "CheckOutDate", ResourceType = typeof(ResToolCheckInOutHistory))]
            public Nullable<DateTime> CheckOutDate { get; set; }

            [Display(Name = "CheckInDate", ResourceType = typeof(ResToolCheckInOutHistory))]
            public Nullable<DateTime> CheckInDate { get; set; }
            public Nullable<Int64> CheckInCheckOutID { get; set; }


            [Display(Name = "NoOfPastMntsToConsider", ResourceType = typeof(ResToolMaster))]
            [Range(2, 10, ErrorMessageResourceName = "NoOfPastMntsToConsider", ErrorMessageResourceType = typeof(ResToolMaster))]
            public int? NoOfPastMntsToConsider { get; set; }

            [Display(Name = "MaintenanceType", ResourceType = typeof(ResToolMaster))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public int MaintenanceType { get; set; }

            public bool IsAutoMaintain { get; set; }

            [Display(Name = "CheckOutQuantity", ResourceType = typeof(ResToolCheckInOutHistory))]
            public Nullable<System.Double> CheckOutQuantity { get; set; }
            [Display(Name = "CheckinQuantity", ResourceType = typeof(ResToolCheckInOutHistory))]
            public Nullable<System.Double> CheckInQuantity { get; set; }
            public int DaysDiff { get; set; }

            public string ImageType { get; set; }

            //ImagePath
            [Display(Name = "ImagePath", ResourceType = typeof(ResToolMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"^[a-zA-Z0-9](?:[a-zA-Z0-9 ._-]*[a-zA-Z0-9])?\.(jpg|JPG|png|PNG|gif|GIF|bmp|BMP|jpeg|JPEG)+$", ErrorMessageResourceName = "InvalidFilename", ErrorMessageResourceType = typeof(ResMessage))]
            //
            public System.String ImagePath { get; set; }

            [Display(Name = "ToolImageExternalURL", ResourceType = typeof(ResToolMaster))]
            [StringLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", ErrorMessageResourceName = "InvalidURL", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ToolImageExternalURL { get; set; }

            [Display(Name = "CheckOutUDF1", ResourceType = typeof(ResToolCheckInOutHistory))]
            public string CheckOutUDF1 { get; set; }

            [Display(Name = "CheckOutUDF2", ResourceType = typeof(ResToolCheckInOutHistory))]
            public string CheckOutUDF2 { get; set; }

            [Display(Name = "CheckOutUDF3", ResourceType = typeof(ResToolCheckInOutHistory))]
            public string CheckOutUDF3 { get; set; }

            [Display(Name = "CheckOutUDF4", ResourceType = typeof(ResToolCheckInOutHistory))]
            public string CheckOutUDF4 { get; set; }

            [Display(Name = "CheckOutUDF5", ResourceType = typeof(ResToolCheckInOutHistory))]
            public string CheckOutUDF5 { get; set; }


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
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            //Room
            [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> Room { get; set; }

            [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
            public string RoomName { get; set; }

            [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
            public string CreatedByName { get; set; }

            [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
            public string UpdatedByName { get; set; }

            [Display(Name = "ReceivedOn", ResourceType = typeof(ResToolMaster))]
            public Nullable<System.DateTime> ReceivedOn { get; set; }

            [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResToolMaster))]
            public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

            //AddedFrom
            [Display(Name = "AddedFrom", ResourceType = typeof(ResToolMaster))]
            public System.String AddedFrom { get; set; }


            //EditedFrom
            [Display(Name = "EditedFrom", ResourceType = typeof(ResToolMaster))]
            public System.String EditedFrom { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }

            public int Index { get; set; }
        }
        #endregion

        #region [Supplier Master Import]
        public class SupplierMasterImport
        {

            //ID
            public System.Int64 ID { get; set; }

            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "SupplierName", ResourceType = typeof(ResSupplierMaster))]
            [AllowHtml]
            public string SupplierName { get; set; }

            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "SupplierColor", ResourceType = typeof(ResSupplierMaster))]
            public string SupplierColor { get; set; }

            [Display(Name = "Description", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public string Description { get; set; }

            //BranchNumber
            [Display(Name = "BranchNumber", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [AllowHtml]
            public System.String BranchNumber { get; set; }

            [Display(Name = "MaximumOrderSize", ResourceType = typeof(ResSupplierMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            [Range(1, int.MaxValue, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Int32> MaximumOrderSize { get; set; }

            [StringLength(1027, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "Address", ResourceType = typeof(ResSupplierMaster))]
            public string Address { get; set; }

            [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "City", ResourceType = typeof(ResSupplierMaster))]
            public string City { get; set; }

            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "State", ResourceType = typeof(ResSupplierMaster))]
            public string State { get; set; }

            [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "ZipCode", ResourceType = typeof(ResSupplierMaster))]
            //[RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = "NumberOnly", ErrorMessageResourceType = typeof(ResMessage))]
            public string ZipCode { get; set; }

            [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "Country", ResourceType = typeof(ResSupplierMaster))]
            public string Country { get; set; }

            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "Contact", ResourceType = typeof(ResSupplierMaster))]
            public string Contact { get; set; }

            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "Phone", ResourceType = typeof(ResSupplierMaster))]
            public string Phone { get; set; }

            [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "Fax", ResourceType = typeof(ResSupplierMaster))]
            public string Fax { get; set; }


            //IsSendtoVendor
            [Display(Name = "IsSendtoVendor", ResourceType = typeof(ResSupplierMaster))]

            public Boolean IsSendtoVendor { get; set; }

            //IsVendorReturnAsn
            [Display(Name = "IsVendorReturnAsn", ResourceType = typeof(ResSupplierMaster))]

            public Boolean IsVendorReturnAsn { get; set; }

            //IsSupplierReceivesKitComponents
            [Display(Name = "IsSupplierReceivesKitComponents", ResourceType = typeof(ResSupplierMaster))]

            public Boolean IsSupplierReceivesKitComponents { get; set; }

            public bool OrderNumberTypeBlank { get; set; }
            public bool OrderNumberTypeFixed { get; set; }
            public bool OrderNumberTypeBlanketOrderNumber { get; set; }
            public bool OrderNumberTypeIncrementingOrderNumber { get; set; }
            public bool OrderNumberTypeIncrementingbyDay { get; set; }
            public bool OrderNumberTypeDateIncrementing { get; set; }
            public bool OrderNumberTypeDate { get; set; }



            [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
            public string Action { get; set; }

            [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
            public Int64 HistoryID { get; set; }

            //UDF1
            [Display(Name = "UDF1", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF1 { get; set; }

            //UDF2
            [Display(Name = "UDF2", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF2 { get; set; }

            //UDF3
            [Display(Name = "UDF3", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF3 { get; set; }

            //UDF4
            [Display(Name = "UDF4", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF4 { get; set; }

            //UDF5
            [Display(Name = "UDF5", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF5 { get; set; }

            //UDF6
            [Display(Name = "UDF6", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF6 { get; set; }

            //UDF7
            [Display(Name = "UDF7", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF7 { get; set; }

            //UDF8
            [Display(Name = "UDF8", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF8 { get; set; }

            //UDF9
            [Display(Name = "UDF9", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF9 { get; set; }

            //UDF10
            [Display(Name = "UDF10", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF10 { get; set; }


            #region [SUPPLIER ACCOUNT DETAILS]

            [Display(Name = "AccountName", ResourceType = typeof(ResSupplierAccountDetails))]
            // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [RequiredIf("IsSendtoVendor", "True", ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [StringLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String AccountName { get; set; }

            [Display(Name = "AccountNo", ResourceType = typeof(ResSupplierAccountDetails))]
            // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [RequiredIf("IsSendtoVendor", "True", ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [StringLength(64, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [AllowHtml]
            public System.String AccountNumber { get; set; }


            public string AccountAddress { get; set; }
            public string AccountCity { get; set; }
            public string AccountState { get; set; }
            public string AccountZip { get; set; }
            public bool AccountIsDefault { get; set; }

            #endregion

            #region [SUPPLIER BLANKET DETAILS]

            //BlanketPO
            [Display(Name = "BlanketPO", ResourceType = typeof(ResSupplierBlanketPODetails))]
            [StringLength(22, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [AllowHtml]
            public System.String BlanketPONumber { get; set; }

            //StartDate
            [Display(Name = "StartDate", ResourceType = typeof(ResSupplierBlanketPODetails))]
            public Nullable<System.DateTime> StartDate { get; set; }

            private string _StartDate;
            public string StartDateString
            {
                get
                {
                    if (string.IsNullOrEmpty(_StartDate))
                    {
                        _StartDate = FnCommon.ConvertDateByTimeZone(StartDate, true);
                    }
                    return _StartDate;
                }
                set { this._StartDate = value; }
            }

            //Enddate
            [Display(Name = "Enddate", ResourceType = typeof(ResSupplierBlanketPODetails))]
            public Nullable<System.DateTime> EndDate { get; set; }

            private string _EndDate;
            public string EndDateString
            {
                get
                {
                    if (string.IsNullOrEmpty(_EndDate))
                    {
                        _EndDate = FnCommon.ConvertDateByTimeZone(EndDate, true);
                    }
                    return _EndDate;
                }
                set { this._EndDate = value; }
            }

            public double? MaxLimit { get; set; }
            public bool IsNotExceed { get; set; }

            public double? MaxLimitQty { get; set; }
            public bool IsNotExceedQty { get; set; }

            public bool IsBlanketDeleted { get; set; }

            #endregion


            public bool PullPurchaseNumberFixed { get; set; }
            public bool PullPurchaseNumberBlanketOrder { get; set; }
            public bool PullPurchaseNumberDateIncrementing { get; set; }
            public bool PullPurchaseNumberDate { get; set; }
            public int? PullPurchaseNumberType { get; set; }
            public string LastPullPurchaseNumberUsed { get; set; }

            public int? POAutoSequence { get; set; }

            public bool IsEmailPOInBody { get; set; }
            public bool IsEmailPOInPDF { get; set; }
            public bool IsEmailPOInCSV { get; set; }
            public bool IsEmailPOInX12 { get; set; }

            public string ImageType { get; set; }

            //ImagePath
            [Display(Name = "SupplierImage", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"^[a-zA-Z0-9](?:[a-zA-Z0-9 ._-]*[a-zA-Z0-9])?\.(jpg|png|gif|bmp|jpeg)+$", ErrorMessageResourceName = "InvalidFilename", ErrorMessageResourceType = typeof(ResMessage))]
            //
            public System.String SupplierImage { get; set; }

            [Display(Name = "ImageExternalURL", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", ErrorMessageResourceName = "InvalidURL", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ImageExternalURL { get; set; }


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
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            //Room
            [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> Room { get; set; }

            [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
            public string RoomName { get; set; }

            [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
            public string CreatedByName { get; set; }

            [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
            public string UpdatedByName { get; set; }

            [Display(Name = "ReceivedOn", ResourceType = typeof(ResSupplierMaster))]
            public Nullable<System.DateTime> ReceivedOn { get; set; }

            [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResSupplierMaster))]
            public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

            //AddedFrom
            [Display(Name = "AddedFrom", ResourceType = typeof(ResSupplierMaster))]
            public System.String AddedFrom { get; set; }


            //EditedFrom
            [Display(Name = "EditedFrom", ResourceType = typeof(ResSupplierMaster))]
            public System.String EditedFrom { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }

            public int Index { get; set; }
        }
        #endregion

        #region [QuickList Items Import]
        public class QuickListItemsImport
        {

            //ID
            public System.Int64 ID { get; set; }


            [Display(Name = "Quantity", ResourceType = typeof(ResQuickList))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public double Quantity { get; set; }


            [Display(Name = "Name", ResourceType = typeof(ResQuickList))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string QuickListname { get; set; }

            [Display(Name = "ItemNumber", ResourceType = typeof(ResQuickList))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string ItemNumber { get; set; }

            [Display(Name = "Comments", ResourceType = typeof(ResQuickList))]
            public string Comments { get; set; }

            [Display(Name = "BinNumber", ResourceType = typeof(ResQuickList))]
            public string BinNumber { get; set; }

            public long BinID { get; set; }

            [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResQuickList))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.Double ConsignedQuantity { get; set; }

            [Display(Name = "ListType", ResourceType = typeof(ResQuickList))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string Type { get; set; }
            public QuickListType? QLType { get; set; }

            public Guid QuickListGUID { get; set; }

            public Guid ItemGUID { get; set; }

            //UDF1
            [Display(Name = "UDF1", ResourceType = typeof(ResQuickList))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF1 { get; set; }

            //UDF2
            [Display(Name = "UDF2", ResourceType = typeof(ResQuickList))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF2 { get; set; }

            //UDF3
            [Display(Name = "UDF3", ResourceType = typeof(ResQuickList))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF3 { get; set; }

            //UDF4
            [Display(Name = "UDF4", ResourceType = typeof(ResQuickList))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF4 { get; set; }

            //UDF5
            [Display(Name = "UDF5", ResourceType = typeof(ResQuickList))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF5 { get; set; }


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
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            //Room
            [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> Room { get; set; }

            [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
            public string RoomName { get; set; }

            [Display(Name = "CreatedBy", ResourceType = typeof(ResCustomer))]
            public string CreatedByName { get; set; }

            [Display(Name = "UpdatedBy", ResourceType = typeof(ResCustomer))]
            public string UpdatedByName { get; set; }

            [Display(Name = "ReceivedOn", ResourceType = typeof(ResCustomer))]
            public Nullable<System.DateTime> ReceivedOn { get; set; }

            [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCustomer))]
            public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

            //AddedFrom
            [Display(Name = "AddedFrom", ResourceType = typeof(ResCustomer))]
            public System.String AddedFrom { get; set; }


            //EditedFrom
            [Display(Name = "EditedFrom", ResourceType = typeof(ResCustomer))]
            public System.String EditedFrom { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }

            public int Index { get; set; }
        }
        #endregion

        #region [Inventory Location Import (MANUAL COUNT)]
        public class InventoryLocationImport
        {

            //ID
            public System.Int64 ID { get; set; }


            [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResBin))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> customerownedquantity { get; set; }

            [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string ItemNumber { get; set; }

            [Display(Name = "BinNumber", ResourceType = typeof(ResBin))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string BinNumber { get; set; }

            public long BinID { get; set; }

            [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResBin))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> consignedquantity { get; set; }


            //LotNumber
            [Display(Name = "LotNumber", ResourceType = typeof(ResItemLocationDetails))]
            [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [AllowHtml]
            public System.String LotNumber { get; set; }

            //SerialNumber
            [Display(Name = "SerialNumber", ResourceType = typeof(ResItemLocationDetails))]
            [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [AllowHtml]
            public System.String SerialNumber { get; set; }

            [Display(Name = "ExpirationDate", ResourceType = typeof(ResItemLocationDetails))]
            public Nullable<System.DateTime> ExpirationDate { get; set; }

            //ReceivedDate
            [Display(Name = "ReceivedDate", ResourceType = typeof(ResItemLocationDetails))]
            public Nullable<System.DateTime> ReceivedDate { get; set; }

            //Received
            [Display(Name = "Received", ResourceType = typeof(ResItemLocationDetails))]
            public string Received { get; set; }

            //ExpirationDate
            [Display(Name = "Expiration", ResourceType = typeof(ResItemLocationDetails))]
            public string Expiration { get; set; }

            public string displayExpiration { get; set; }


            //Cost
            [Display(Name = "Cost", ResourceType = typeof(ResItemLocationDetails))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> Cost { get; set; }

            public Guid ItemGUID { get; set; }

            //UDF1
            [Display(Name = "UDF1", ResourceType = typeof(ResItemLocationDetails))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF1 { get; set; }

            //UDF2
            [Display(Name = "UDF2", ResourceType = typeof(ResItemLocationDetails))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF2 { get; set; }

            //UDF3
            [Display(Name = "UDF3", ResourceType = typeof(ResItemLocationDetails))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF3 { get; set; }

            //UDF4
            [Display(Name = "UDF4", ResourceType = typeof(ResItemLocationDetails))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF4 { get; set; }

            //UDF5
            [Display(Name = "UDF5", ResourceType = typeof(ResItemLocationDetails))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF5 { get; set; }

            //UDF6
            [Display(Name = "UDF6", ResourceType = typeof(ResItemLocationDetails))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [AllowHtml]
            public System.String UDF6 { get; set; }

            //UDF7
            [Display(Name = "UDF7", ResourceType = typeof(ResItemLocationDetails))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [AllowHtml]
            public System.String UDF7 { get; set; }

            //UDF8
            [Display(Name = "UDF8", ResourceType = typeof(ResItemLocationDetails))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [AllowHtml]
            public System.String UDF8 { get; set; }

            //UDF9
            [Display(Name = "UDF9", ResourceType = typeof(ResItemLocationDetails))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [AllowHtml]
            public System.String UDF9 { get; set; }

            //UDF10
            [Display(Name = "UDF10", ResourceType = typeof(ResItemLocationDetails))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [AllowHtml]
            public System.String UDF10 { get; set; }
            public string ProjectSpend { get; set; }

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
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            //Room
            [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> Room { get; set; }

            [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
            public string RoomName { get; set; }

            [Display(Name = "CreatedBy", ResourceType = typeof(ResCustomer))]
            public string CreatedByName { get; set; }

            [Display(Name = "UpdatedBy", ResourceType = typeof(ResCustomer))]
            public string UpdatedByName { get; set; }

            [Display(Name = "ReceivedOn", ResourceType = typeof(ResCustomer))]
            public Nullable<System.DateTime> ReceivedOn { get; set; }

            [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCustomer))]
            public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

            //AddedFrom
            [Display(Name = "AddedFrom", ResourceType = typeof(ResCustomer))]
            public System.String AddedFrom { get; set; }


            //EditedFrom
            [Display(Name = "EditedFrom", ResourceType = typeof(ResCustomer))]
            public System.String EditedFrom { get; set; }

            public string InsertedFrom { get; set; }
            public System.String Status { get; set; }
            public System.String Reason { get; set; }

            public int Index { get; set; }
        }
        #endregion

        #region [Work Order Import]
        public class WorkOrderImport
        {

            //ID
            public System.Int64 ID { get; set; }

            [Display(Name = "WOName", ResourceType = typeof(ResWorkOrder))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [StringLength(256, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]

            public System.String WOName { get; set; }

            public Nullable<System.Int64> TechnicianID { get; set; }

            [Display(Name = "Technician", ResourceType = typeof(ResTechnician))]
            public System.String Technician { get; set; }

            [Display(Name = "Supplier", ResourceType = typeof(ResOrder))]
            public string SupplierName { get; set; }

            public Nullable<System.Int64> SupplierId { get; set; }

            [Display(Name = "Description", ResourceType = typeof(ResWorkOrder))]
            public System.String Description { get; set; }

            public Nullable<System.Int64> CustomerID { get; set; }
            [Display(Name = "Customer", ResourceType = typeof(ResCustomer))]
            public string Customer { get; set; }
            public Nullable<Guid> CustomerGUID { get; set; }

            [Display(Name = "WOStatus", ResourceType = typeof(ResWorkOrder))]
            public string WOStatus { get; set; }

            [Display(Name = "Odometer_OperationHours", ResourceType = typeof(ResWorkOrder))]
            [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Int64? Odometer_OperationHours { get; set; }

            [Display(Name = "WOType", ResourceType = typeof(ResWorkOrder))]
            public string WOType { get; set; }
            public string WhatWhereAction { get; set; }

            public bool IsSignatureCapture { get; set; }

            public bool IsSignatureRequired { get; set; }

            public string ReleaseNumber { get; set; }

            //UDF1
            [Display(Name = "UDF1", ResourceType = typeof(ResWorkOrder))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF1 { get; set; }

            //UDF2
            [Display(Name = "UDF2", ResourceType = typeof(ResWorkOrder))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF2 { get; set; }

            //UDF3
            [Display(Name = "UDF3", ResourceType = typeof(ResWorkOrder))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF3 { get; set; }

            //UDF4
            [Display(Name = "UDF4", ResourceType = typeof(ResWorkOrder))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF4 { get; set; }

            //UDF5
            [Display(Name = "UDF5", ResourceType = typeof(ResWorkOrder))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF5 { get; set; }


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
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            //Room
            [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> Room { get; set; }

            [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
            public string RoomName { get; set; }

            [Display(Name = "CreatedBy", ResourceType = typeof(ResCustomer))]
            public string CreatedByName { get; set; }

            [Display(Name = "UpdatedBy", ResourceType = typeof(ResCustomer))]
            public string UpdatedByName { get; set; }

            [Display(Name = "ReceivedOn", ResourceType = typeof(ResCustomer))]
            public Nullable<System.DateTime> ReceivedOn { get; set; }

            [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCustomer))]
            public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

            //AddedFrom
            [Display(Name = "AddedFrom", ResourceType = typeof(ResCustomer))]
            public System.String AddedFrom { get; set; }


            //EditedFrom
            [Display(Name = "EditedFrom", ResourceType = typeof(ResCustomer))]
            public System.String EditedFrom { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }

            public int Index { get; set; }
        }
        #endregion

        #region [BOM Item Master Import]
        public class BOMItemMasterImport
        {
            //ID
            public System.Int64 ID { get; set; }

            //ItemNumber
            [Display(Name = "ItemNumber", ResourceType = typeof(ResBomItemMaster))]
            [StringLength(100, ErrorMessage = "Item number can not be more then 100 character.")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ItemNumber { get; set; }

            //ManufacturerID
            [Display(Name = "ManufacturerID", ResourceType = typeof(ResItemMaster))]
            public Nullable<System.Int64> ManufacturerID { get; set; }

            //ManufacturerNumber
            [Display(Name = "ManufacturerNumber", ResourceType = typeof(ResBomItemMaster))]
            [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ManufacturerNumber { get; set; }

            //SupplierID
            [Display(Name = "SupplierID", ResourceType = typeof(ResItemMaster))]
            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.Int64? SupplierID { get; set; }
            public System.Int64? BlanketPOID { get; set; }

            //SupplierPartNo
            [Display(Name = "SupplierPartNo", ResourceType = typeof(ResBomItemMaster))]
            [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [RequiredIf("ItemTypeName", "Item", ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String SupplierPartNo { get; set; }

            //BlanketOrderNumber
            [Display(Name = "BlanketOrderNumber", ResourceType = typeof(ResItemMaster))]
            public System.String BlanketOrderNumber { get; set; }

            //UPC
            [Display(Name = "UPC", ResourceType = typeof(ResBomItemMaster))]
            [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UPC { get; set; }

            //UNSPSC
            [Display(Name = "UNSPSC", ResourceType = typeof(ResBomItemMaster))]
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

            ////UOMID
            //[Display(Name = "UOMID", ResourceType = typeof(ResItemMaster))]
            //public System.Int64 UOMID { get; set; }


            //PricePerTerm
            [Display(Name = "PricePerTerm", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Decimal> PricePerTerm { get; set; }

            //DefaultReorderQuantity
            [Display(Name = "DefaultReorderQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //[DefaultReorderQuantityCheck("MaximumQuantity", ErrorMessage = "Default Reorder quantity must be less then Maximum quantity")]
            //   [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> DefaultReorderQuantity { get; set; }

            //DefaultPullQuantity
            [Display(Name = "DefaultPullQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //   [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> DefaultPullQuantity { get; set; }

            ////DefaultCartQuantity
            //[Display(Name = "DefaultCartQuantity", ResourceType = typeof(ResItemMaster))]
            //[RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //public Nullable<System.Double> DefaultCartQuantity { get; set; }

            //Cost
            [Display(Name = "Cost", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> Cost { get; set; }

            //Markup
            [Display(Name = "Markup", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            [Range(float.MinValue, float.MaxValue, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
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
            [Display(Name = "LeadTimeInDays", ResourceType = typeof(ResBomItemMaster))]
            [RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Int32> LeadTimeInDays { get; set; }

            //Link1
            [Display(Name = "Link1", ResourceType = typeof(ResItemMaster))]
            [StringLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String Link1 { get; set; }

            //Link2
            [Display(Name = "Link2", ResourceType = typeof(ResItemMaster))]
            [StringLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"([a-z0-9A-Z\s]*)\.(jpg|jpeg|bmp|pdf|xls|doc|xlsx|docx|png|gif)$", ErrorMessageResourceName = "InvalidFilename", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String Link2 { get; set; }

            //Trend
            [Display(Name = "Trend", ResourceType = typeof(ResItemMaster))]
            public Boolean Trend { get; set; }

            //Taxable
            [Display(Name = "Taxable", ResourceType = typeof(ResBomItemMaster))]
            public Boolean Taxable { get; set; }

            //Consignment
            [Display(Name = "Consignment", ResourceType = typeof(ResBomItemMaster))]
            public Boolean Consignment { get; set; }

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
            [Display(Name = "ItemUniqueNumber", ResourceType = typeof(ResBomItemMaster))]
            [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ItemUniqueNumber { get; set; }

            //TransferOrPurchase
            [Display(Name = "IsTransfer", ResourceType = typeof(ResBomItemMaster))]
            public Boolean IsTransfer { get; set; }

            //IsPurchase
            [Display(Name = "IsPurchase", ResourceType = typeof(ResBomItemMaster))]
            public Boolean IsPurchase { get; set; }

            public Boolean IsActive { get; set; }

            ////DefaultLocation
            //[Display(Name = "DefaultLocation", ResourceType = typeof(ResItemMaster))]
            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            //public long DefaultLocation { get; set; }


            ////DefaultLocation
            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            //[Display(Name = "DefaultLocation", ResourceType = typeof(ResItemMaster))]
            //public string DefaultLocationName { get; set; }


            ////InventoryClassification
            //[Display(Name = "InventoryClassification", ResourceType = typeof(ResItemMaster))]
            //public Nullable<System.Int32> InventoryClassification { get; set; }

            [Display(Name = "InventoryClassification", ResourceType = typeof(ResBomItemMaster))]
            public string InventoryClassificationName { get; set; }


            //SerialNumberTracking
            [Display(Name = "SerialNumberTracking", ResourceType = typeof(ResBomItemMaster))]
            public Boolean SerialNumberTracking { get; set; }

            //LotNumberTracking
            [Display(Name = "LotNumberTracking", ResourceType = typeof(ResBomItemMaster))]
            public Boolean LotNumberTracking { get; set; }

            //DateCodeTracking
            [Display(Name = "DateCodeTracking", ResourceType = typeof(ResBomItemMaster))]
            public Boolean DateCodeTracking { get; set; }

            //ItemType
            [Display(Name = "ItemType", ResourceType = typeof(ResItemMaster))]
            public System.Int32 ItemType { get; set; }

            //ImagePath
            [Display(Name = "ImagePath", ResourceType = typeof(ResItemMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"([a-z0-9A-Z\s]*)\.(jpg|jpeg|bmp|png|gif)$", ErrorMessageResourceName = "InvalidFilename", ErrorMessageResourceType = typeof(ResMessage))]
            //
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
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

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
            public bool IsItemLevelMinMaxQtyRequired { get; set; }

            //[Display(Name="Enforce Default Reorder Quantity")]
            [Display(Name = "IsEnforceDefaultReorderQuantity", ResourceType = typeof(ResItemMaster))]
            public bool? IsEnforceDefaultReorderQuantity { get; set; }

            //IsBuildBreak
            [Display(Name = "IsBuildBreak", ResourceType = typeof(ResItemMaster))]
            public bool IsBuildBreak { get; set; }

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

            [Display(Name = "Category", ResourceType = typeof(ResBomItemMaster))]
            public string CategoryName { get; set; }

            public string CategoryColor { get; set; }

            [Display(Name = "ManufacturerName", ResourceType = typeof(ResBomItemMaster))]
            public string ManufacturerName { get; set; }

            [Display(Name = "Supplier", ResourceType = typeof(ResBomItemMaster))]
            [RequiredIf("ItemTypeName", "Item", ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string SupplierName { get; set; }

            [Display(Name = "ItemType", ResourceType = typeof(ResBomItemMaster))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string ItemTypeName { get; set; }

            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "Unit", ResourceType = typeof(ResUnitMaster))]
            public string Unit { get; set; }

            [Display(Name = "GLAccount", ResourceType = typeof(ResGLAccount))]
            public string GLAccount { get; set; }

            [Display(Name = "DefaultLocation", ResourceType = typeof(ResItemMaster))]
            //  [Required(ErrorMessage = "Default bin required.")]
            public string InventryLocation { get; set; }

            public string AppendedBarcodeString { get; set; }
            public string ImageType { get; set; }





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

            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
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

            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "CostUOMID", ResourceType = typeof(ResItemMaster))]
            public System.String CostUOMName { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }
            public int Index { get; set; }

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
            [RegularExpression(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", ErrorMessageResourceName = "InvalidURL", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ItemImageExternalURL { get; set; }

            [Display(Name = "ItemDocExternalURL", ResourceType = typeof(ResItemMaster))]
            [StringLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", ErrorMessageResourceName = "InvalidURL", ErrorMessageResourceType = typeof(ResMessage))]

            public System.String ItemDocExternalURL { get; set; }


            [Display(Name = "ItemLink2ExternalURL", ResourceType = typeof(ResItemMaster))]
            [StringLength(4000, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [RegularExpression(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", ErrorMessageResourceName = "InvalidURL", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ItemLink2ExternalURL { get; set; }

            public System.String ItemLink2ImageType { get; set; }
        }
        #endregion


        #region [Kit Detail Import]
        public class KitDetailImport
        {
            //ID
            public System.Int64 ID { get; set; }

            public Nullable<Guid> KitGUID { get; set; }

            public Nullable<Guid> ItemGUID { get; set; }

            //QuantityPerKit
            [Display(Name = "QuantityPerKit", ResourceType = typeof(ResKitMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> QuantityPerKit { get; set; }

            //QuantityReadyForAssembly
            [Display(Name = "QuantityReadyForAssembly", ResourceType = typeof(ResKitMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> QuantityReadyForAssembly { get; set; }

            //ReOrderType
            [Display(Name = "ReOrderType", ResourceType = typeof(ResKitMaster))]
            // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string ReOrderType { get; set; }

            //KitCategory
            [Display(Name = "KitCategory", ResourceType = typeof(ResKitMaster))]
            public Nullable<System.Int32> KitCategory { get; set; }

            //AvailableKitQuantity
            [Display(Name = "AvailableKitQuantity", ResourceType = typeof(ResKitMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> AvailableKitQuantity { get; set; }


            //KitPartNumber
            [Display(Name = "KitPartNumber", ResourceType = typeof(ResKitMaster))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String KitPartNumber { get; set; }

            //Description
            [Display(Name = "Description", ResourceType = typeof(ResKitMaster))]
            [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [AllowHtml]
            public System.String Description { get; set; }

            //ItemNumber
            [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
            [StringLength(100, ErrorMessage = "Item number can not be more then 100 character.")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ItemNumber { get; set; }


            //SupplierID
            [Display(Name = "SupplierID", ResourceType = typeof(ResItemMaster))]
            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.Int64? SupplierID { get; set; }

            //SupplierPartNo
            [Display(Name = "SupplierPartNo", ResourceType = typeof(ResItemMaster))]
            [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String SupplierPartNo { get; set; }


            //DefaultReorderQuantity
            [Display(Name = "DefaultReorderQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //[DefaultReorderQuantityCheck("MaximumQuantity", ErrorMessage = "Default Reorder quantity must be less then Maximum quantity")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> DefaultReorderQuantity { get; set; }

            //DefaultPullQuantity
            [Display(Name = "DefaultPullQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> DefaultPullQuantity { get; set; }

            //CriticalQuantity
            [Display(Name = "CriticalQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [CriticleQuantityCheck("MinimumQuantity", ErrorMessage = "Critical quantity must be less then Minimum quantity")]
            [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
            public System.Double CriticalQuantity { get; set; }

            //MinimumQuantity
            [Display(Name = "MinimumQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [MinimumQuantityCheck("MaximumQuantity", ErrorMessage = "Minimum quantity must be less then Maximum quantity")]
            [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
            public System.Double MinimumQuantity { get; set; }

            //MaximumQuantity
            [Display(Name = "MaximumQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
            public System.Double MaximumQuantity { get; set; }


            //OnHandQuantity        
            [Display(Name = "OnHandQuantity", ResourceType = typeof(ResItemMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Double> OnHandQuantity { get; set; }

            public Boolean IsActive { get; set; }

            public Nullable<System.Int64> DefaultLocation { get; set; }


            //DefaultLocation
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "DefaultLocation", ResourceType = typeof(ResItemMaster))]
            public string DefaultLocationName { get; set; }


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
            public System.Int32 ItemType { get; set; }


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
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            //Room
            [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> Room { get; set; }

            [Display(Name = "IsItemLevelMinMaxQtyRequired", ResourceType = typeof(ResItemMaster))]
            public bool IsItemLevelMinMaxQtyRequired { get; set; }

            //IsBuildBreak
            [Display(Name = "IsBuildBreak", ResourceType = typeof(ResItemMaster))]
            public bool IsBuildBreak { get; set; }


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


            [Display(Name = "Supplier", ResourceType = typeof(ResItemMaster))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string SupplierName { get; set; }

            [Display(Name = "ItemType", ResourceType = typeof(ResItemMaster))]
            // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string ItemTypeName { get; set; }


            public long? BinID { get; set; }


            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "CostUOMID", ResourceType = typeof(ResItemMaster))]
            public Nullable<System.Int64> CostUOMID { get; set; }

            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "CostUOMName", ResourceType = typeof(ResItemMaster))]
            public System.String CostUOMName { get; set; }

            public System.String WhatWhereAction { get; set; }

            public Nullable<System.Int32> CostUOMValue { get; set; }
            public Nullable<System.Int64> UOMID { get; set; }


            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "UOM", ResourceType = typeof(ResItemMaster))]
            public string UOM { get; set; }


            public bool IsBOMItem { get; set; }
            public long? RefBomId { get; set; }
            //public Nullable<byte> TrendingSetting { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }
            public int Index { get; set; }


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

        }
        #endregion


        #region [Project Master Import]
        public class ProjectMasterImport
        {

            //ID
            public System.Int64 ID { get; set; }


            [Display(Name = "ProjectSpendName", ResourceType = typeof(ResProjectMaster))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ProjectSpendName { get; set; }

            //Description
            [Display(Name = "Description", ResourceType = typeof(ResProjectMaster))]
            [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [AllowHtml]
            public System.String Description { get; set; }

            //DollarLimitAmount
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "DollarLimitAmount", ResourceType = typeof(ResProjectMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public Nullable<System.Decimal> DollarLimitAmount { get; set; }

            [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string ItemNumber { get; set; }

            [Display(Name = "TrackAllUsageAgainstThis", ResourceType = typeof(ResProjectMaster))]
            public Boolean TrackAllUsageAgainstThis { get; set; }

            [Display(Name = "ItemDollarLimitAmount", ResourceType = typeof(ResProjectMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public decimal? ItemDollarLimitAmount { get; set; }

            [Display(Name = "ItemQuantityLimitAmount", ResourceType = typeof(ResProjectMaster))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            public double? ItemQuantityLimitAmount { get; set; }

            [Display(Name = "IsLineItemDeleted", ResourceType = typeof(ResProjectMaster))]
            public bool IsLineItemDeleted { get; set; }

            [Display(Name = "IsClosed", ResourceType = typeof(ResProjectMaster))]
            public bool IsClosed { get; set; }

            public Guid ItemGUID { get; set; }

            //UDF1
            [Display(Name = "UDF1", ResourceType = typeof(ResProjectMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF1 { get; set; }

            //UDF2
            [Display(Name = "UDF2", ResourceType = typeof(ResProjectMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF2 { get; set; }

            //UDF3
            [Display(Name = "UDF3", ResourceType = typeof(ResProjectMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF3 { get; set; }

            //UDF4
            [Display(Name = "UDF4", ResourceType = typeof(ResProjectMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF4 { get; set; }

            //UDF5
            [Display(Name = "UDF5", ResourceType = typeof(ResProjectMaster))]
            [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String UDF5 { get; set; }


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
            [Display(Name = "IsDeleted", ResourceType = typeof(ResProjectMaster))]
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }


            public Nullable<System.Int64> Room { get; set; }

            [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
            public string RoomName { get; set; }

            [Display(Name = "CreatedBy", ResourceType = typeof(ResCustomer))]
            public string CreatedByName { get; set; }

            [Display(Name = "UpdatedBy", ResourceType = typeof(ResCustomer))]
            public string UpdatedByName { get; set; }

            [Display(Name = "ReceivedOn", ResourceType = typeof(ResCustomer))]
            public Nullable<System.DateTime> ReceivedOn { get; set; }

            [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCustomer))]
            public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

            //AddedFrom
            [Display(Name = "AddedFrom", ResourceType = typeof(ResCustomer))]
            public System.String AddedFrom { get; set; }


            //EditedFrom
            [Display(Name = "EditedFrom", ResourceType = typeof(ResCustomer))]
            public System.String EditedFrom { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }

            public int Index { get; set; }
        }
        #endregion


        #region [Item Location (Inventory Location Qunantity) Import]
        public class InventoryLocationQuantityImport
        {
            //ID
            public System.Int64 ID { get; set; }

            public Nullable<Guid> ItemGUID { get; set; }

            //ItemNumber
            [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
            [StringLength(100, ErrorMessage = "Item number can not be more then 100 character.")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ItemNumber { get; set; }


            //CriticalQuantity
            [Display(Name = "CriticalQuantity", ResourceType = typeof(ResBin))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [CriticleQuantityCheck("MinimumQuantity", ErrorMessage = "Critical quantity must be less then Minimum quantity")]
            [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
            public System.Double CriticalQuantity { get; set; }

            //MinimumQuantity
            [Display(Name = "MinimumQuantity", ResourceType = typeof(ResBin))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [MinimumQuantityCheck("MaximumQuantity", ErrorMessage = "Minimum quantity must be less then Maximum quantity")]
            [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
            public System.Double MinimumQuantity { get; set; }

            //MaximumQuantity
            [Display(Name = "MaximumQuantity", ResourceType = typeof(ResBin))]
            [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
            //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
            public System.Double MaximumQuantity { get; set; }

            //DefaultLocation
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [Display(Name = "BinNumber", ResourceType = typeof(ResBin))]
            public string BinNumber { get; set; }

            public long? BinID { get; set; }

            [Display(Name = "eVMISensorPort", ResourceType = typeof(ResBin))]
            public string SensorPort { get; set; }

            [Display(Name = "eVMISensorID", ResourceType = typeof(ResBin))]
            public Nullable<System.Double> SensorId { get; set; }


            //IsDefault
            [Display(Name = "IsDefault", ResourceType = typeof(ResBin))]
            public Nullable<Boolean> IsDefault { get; set; }

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
            [Display(Name = "IsDeleted", ResourceType = typeof(ResItemMaster))]
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            //Room

            public Nullable<System.Int64> Room { get; set; }



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






            public System.String Status { get; set; }
            public System.String Reason { get; set; }
            public int Index { get; set; }


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

        }
        #endregion


        #region [Item Manufacturer Import]
        public class ItemManufacturerImport
        {

            //ID
            public System.Int64 ID { get; set; }



            [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
            [StringLength(100, ErrorMessage = "Item number can not be more then 100 character.")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ItemNumber { get; set; }

            //ManufacturerID
            [Display(Name = "ManufacturerID", ResourceType = typeof(ResItemManufacturerDetails))]
            public Nullable<System.Int64> ManufacturerID { get; set; }

            //ManufacturerNumber
            [Display(Name = "ManufacturerNumber", ResourceType = typeof(ResItemManufacturerDetails))]
            [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ManufacturerNumber { get; set; }

            [Display(Name = "ManufacturerName", ResourceType = typeof(ResManufacturer))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public string ManufacturerName { get; set; }

            public Nullable<Guid> ItemGUID { get; set; }

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

            [Display(Name = "IsDefault", ResourceType = typeof(ResItemManufacturerDetails))]
            public Nullable<Boolean> IsDefault { get; set; }

            //IsDeleted
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            public Nullable<System.Int64> Room { get; set; }

            [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
            public string RoomName { get; set; }

            [Display(Name = "CreatedBy", ResourceType = typeof(ResItemManufacturerDetails))]
            public string CreatedByName { get; set; }

            [Display(Name = "UpdatedBy", ResourceType = typeof(ResItemManufacturerDetails))]
            public string UpdatedByName { get; set; }

            public Nullable<System.DateTime> ReceivedOn { get; set; }

            public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

            public System.String AddedFrom { get; set; }

            public System.String EditedFrom { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }

            public int Index { get; set; }
        }
        #endregion

        #region [Item Supplier Import]
        public class ItemSupplierImport
        {

            //ID
            public System.Int64 ID { get; set; }



            [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
            [StringLength(100, ErrorMessage = "Item number can not be more then 100 character.")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ItemNumber { get; set; }

            //ItemGUID
            public Nullable<Guid> ItemGUID { get; set; }

            //ManufacturerID
            [Display(Name = "SupplierID", ResourceType = typeof(ResItemSupplierDetails))]

            public System.Int64 SupplierID { get; set; }

            //ManufacturerName
            [Display(Name = "Supplier", ResourceType = typeof(ResSupplierMaster))]
            [StringLength(150, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String SupplierName { get; set; }

            //ManufacturerNumber
            [Display(Name = "SupplierNumber", ResourceType = typeof(ResItemSupplierDetails))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String SupplierNumber { get; set; }


            public long? BlanketPOID { get; set; }

            [Display(Name = "BlanketPONAME", ResourceType = typeof(ResItemSupplierDetails))]
            public string BlanketPOName { get; set; }

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

            public Nullable<Boolean> IsDefault { get; set; }

            //IsDeleted
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            public Nullable<System.Int64> Room { get; set; }

            [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
            public string RoomName { get; set; }

            [Display(Name = "CreatedBy", ResourceType = typeof(ResItemManufacturerDetails))]
            public string CreatedByName { get; set; }

            [Display(Name = "UpdatedBy", ResourceType = typeof(ResItemManufacturerDetails))]
            public string UpdatedByName { get; set; }

            public Nullable<System.DateTime> ReceivedOn { get; set; }

            public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

            public System.String AddedFrom { get; set; }

            public System.String EditedFrom { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }

            public int Index { get; set; }
        }
        #endregion


        #region [Barcode Master Import]
        public class BarcodeMasterImport
        {

            //ID
            public System.Int64 ID { get; set; }

            [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
            [StringLength(100, ErrorMessage = "Item number can not be more then 100 character.")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ItemNumber { get; set; }

            //ItemGUID
            //public Nullable<Guid> ItemGUID { get; set; }
            public Guid RefGuid { get; set; }


            [Display(Name = "ModuleName", ResourceType = typeof(ResBarcodeMaster))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String ModuleName { get; set; }
            public Guid ModuleGuid { get; set; }


            [Display(Name = "BarcodeString", ResourceType = typeof(ResBarcodeMaster))]
            [StringLength(512, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
            public System.String BarcodeString { get; set; }


            [Display(Name = "BinNumber", ResourceType = typeof(ResBarcodeMaster))]

            public string BinNumber { get; set; }

            public Nullable<Guid> BinGuid { get; set; }


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
            public bool? IsDeleted { get; set; }

            //IsArchived
            public Nullable<Boolean> IsArchived { get; set; }

            //CompanyID
            [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> CompanyID { get; set; }

            //Room
            [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
            public Nullable<System.Int64> Room { get; set; }

            [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
            public string RoomName { get; set; }

            [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
            public string CreatedByName { get; set; }

            [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
            public string UpdatedByName { get; set; }

            [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
            public Nullable<System.DateTime> ReceivedOn { get; set; }

            [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
            public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

            //AddedFrom
            [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
            public System.String AddedFrom { get; set; }


            //EditedFrom
            [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
            public System.String EditedFrom { get; set; }

            public System.String Status { get; set; }
            public System.String Reason { get; set; }

            public int Index { get; set; }
        }
        #endregion

    }
}


