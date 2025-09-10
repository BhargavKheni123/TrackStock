using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Web.Helpers;

namespace eTurns.DTO
{
    [Serializable]
    public class ItemMasterDTO
    {

        #region Constructor
        public ItemMasterDTO()
        {

        }

        #endregion

        private string _xmlItemLocations;
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public virtual System.Int64 ID { get; set; }
        public string xmlItemLocations { get { return _xmlItemLocations; } set { _xmlItemLocations = value; } }
        //ItemNumber
        [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
        [StringLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ItemNumber { get; set; }

        //ManufacturerID
        [Display(Name = "ManufacturerID", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int64> ManufacturerID { get; set; }

        //ManufacturerNumber
        [Display(Name = "ManufacturerNumber", ResourceType = typeof(ResItemMaster))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ManufacturerNumber { get; set; }

        //SupplierID
        [Display(Name = "SupplierID", ResourceType = typeof(ResItemMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64? SupplierID { get; set; }

        //SupplierPartNo
        [Display(Name = "SupplierPartNo", ResourceType = typeof(ResItemMaster))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String SupplierPartNo { get; set; }

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
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int64> UOMID { get; set; }


        //PricePerTerm
        [Display(Name = "PricePerTerm", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0, float.MaxValue)]
        public Nullable<System.Double> PricePerTerm { get; set; }

        //DefaultReorderQuantity
        [Display(Name = "DefaultReorderQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[DefaultReorderQuantityCheck("MaximumQuantity", ErrorMessage = "Default Reorder quantity must be less then Maximum quantity")]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> DefaultReorderQuantity { get; set; }

        //DefaultPullQuantity
        [Display(Name = "DefaultPullQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> DefaultPullQuantity { get; set; }

        ////DefaultCartQuantity
        //[Display(Name = "DefaultCartQuantity", ResourceType = typeof(ResItemMaster))]
        //[RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //public Nullable<System.Double> DefaultCartQuantity { get; set; }

        //Cost
        [Display(Name = "Cost", ResourceType = typeof(ResItemMaster))]
        //[RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0, float.MaxValue)]
        public Nullable<System.Double> Cost { get; set; }

        //Markup
        [Display(Name = "Markup", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[Range(0, 100, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(float.MinValue, float.MaxValue, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Markup { get; set; }
        public Nullable<System.Double> RoomGlobMarkupLabor { get; set; }
        public Nullable<System.Double> RoomGlobMarkupParts { get; set; }
        //SellPrice
        [Display(Name = "SellPrice", ResourceType = typeof(ResItemMaster))]
        //[RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]

        public Nullable<System.Double> SellPrice { get; set; }

        //ExtendedCost
        [Display(Name = "ExtendedCost", ResourceType = typeof(ResItemMaster))]

        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[Range(0, float.MaxValue)]
        public Nullable<System.Double> ExtendedCost { get; set; }

        //LeadTimeInDays
        [Display(Name = "LeadTimeInDays", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int32> LeadTimeInDays { get; set; }

        //Link1
        [Display(Name = "Link1", ResourceType = typeof(ResItemMaster))]
        [StringLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Link1 { get; set; }

        //Link2
        [Display(Name = "Link2", ResourceType = typeof(ResItemMaster))]
        [StringLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
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
        [Display(Name = "InTransferTransitQuantity", ResourceType = typeof(ResItemMaster))]
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

        //SuggestedTransferQuantity
        [Display(Name = "SuggestedTransferQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> SuggestedTransferQuantity { get; set; }

        //RequisitionedQuantity
        [Display(Name = "RequisitionedQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> RequisitionedQuantity { get; set; }

        ////PackingQuantity
        //[Display(Name = "PackingQuantity", ResourceType = typeof(ResItemMaster))]
        //[RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
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
        public Nullable<System.Double> CriticalQuantity { get; set; }

        //MinimumQuantity
        [Display(Name = "MinimumQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [MinimumQuantityCheck("MaximumQuantity", ErrorMessage = "Minimum quantity must be less then Maximum quantity")]
        [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> MinimumQuantity { get; set; }

        //MaximumQuantity
        [Display(Name = "MaximumQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> MaximumQuantity { get; set; }

        //WeightPerPiece
        [Display(Name = "WeightPerPiece", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> WeightPerPiece { get; set; }

        //ItemUniqueNumber
        [Display(Name = "ItemUniqueNumber", ResourceType = typeof(ResItemMaster))]
        [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ItemUniqueNumber { get; set; }

        //TransferOrPurchase
        [Display(Name = "IsTransfer", ResourceType = typeof(ResItemMaster))]
        public Boolean IsTransfer { get; set; }

        //IsPurchase
        [Display(Name = "IsPurchase", ResourceType = typeof(ResItemMaster))]
        public Boolean IsPurchase { get; set; }

        //DefaultLocation
        [Display(Name = "DefaultLocation", ResourceType = typeof(ResItemMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public long? DefaultLocation { get; set; }


        //DefaultLocation
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "DefaultLocation", ResourceType = typeof(ResItemMaster))]
        public string DefaultLocationName { get; set; }

        public Guid? DefaultLocationGUID { get; set; }

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
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int32 ItemType { get; set; }

        //ImagePath
        [Display(Name = "ImagePath", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ImagePath { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }



        //UDF6
        [Display(Name = "UDF6", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF6 { get; set; }

        //UDF7
        [Display(Name = "UDF7", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF7 { get; set; }

        //UDF8
        [Display(Name = "UDF8", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF8 { get; set; }

        //UDF9
        [Display(Name = "UDF9", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF9 { get; set; }

        //UDF10
        [Display(Name = "UDF10", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF10 { get; set; }

        //GUID
        public Guid GUID { get; set; }

        //Created
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        [Display(Name = "HistoryDate", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> HistoryOn { get; set; }

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

        //Room
        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //IsLotSerialExpiryCost
        [Display(Name = "IsLotSerialExpiryCost", ResourceType = typeof(ResItemMaster))]
        [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String IsLotSerialExpiryCost { get; set; }

        //PackingQuantity
        [Display(Name = "PackingQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> PackingQuantity { get; set; }

        [Display(Name = "IsItemLevelMinMaxQtyRequired", ResourceType = typeof(ResItemMaster))]
        public bool? IsItemLevelMinMaxQtyRequired { get; set; }

        //[Display(Name="Enforce Default Reorder Quantity")]
        [Display(Name = "IsEnforceDefaultReorderQuantity", ResourceType = typeof(ResItemMaster))]
        public bool? IsEnforceDefaultReorderQuantity { get; set; }

        //IsBuildBreak
        [Display(Name = "IsBuildBreak", ResourceType = typeof(ResItemMaster))]
        public Nullable<Boolean> IsBuildBreak { get; set; }

        [Display(Name = "BondedInventory", ResourceType = typeof(ResItemMaster))]
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

        public IEnumerable<ItemLocationDetailsDTO> ItemLocations { get; set; }
        public ICollection<ItemLocationQTYDTO> lstItemLocationQTY { get; set; }
        public ICollection<BinMasterDTO> ItemsLocations { get; set; }

        [Display(Name = "CategoryName", ResourceType = typeof(ResItemMaster))]
        public string CategoryName { get; set; }

        public string CategoryColor { get; set; }

        [Display(Name = "ManufacturerName", ResourceType = typeof(ResManufacturer))]
        public string ManufacturerName { get; set; }

        [Display(Name = "Supplier", ResourceType = typeof(ResSupplierMaster))]
        public string SupplierName { get; set; }
        public string ItemTypeName { get; set; }

        [Display(Name = "UOMID", ResourceType = typeof(ResItemMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string Unit { get; set; }

        [Display(Name = "ReceivedOnDate", ResourceType = typeof(ResItemMaster))]
        public virtual Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResItemMaster))]
        public virtual Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResItemMaster))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String AddedFrom { get; set; }


        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResItemMaster))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String EditedFrom { get; set; }


        [Display(Name = "ItemImageExternalURL", ResourceType = typeof(ResItemMaster))]
        [StringLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ItemImageExternalURL { get; set; }

        [Display(Name = "ItemDocExternalURL", ResourceType = typeof(ResItemMaster))]
        [StringLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ItemDocExternalURL { get; set; }


        public string GLAccount { get; set; }
        public string InventryLocation { get; set; }
        public string AppendedBarcodeString { get; set; }
        public string QuickListName { get; set; }
        public string QuickListGUID { get; set; }
        public double QuickListItemQTY { get; set; }
        public Int32 QuickListType { get; set; }
        public bool IsOnlyFromItemUI { get; set; }

        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Double> ConsignedQuantity { get; set; }
        //[RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "AverageCost", ResourceType = typeof(ResItemMaster))]
        // [Range(0, float.MaxValue)]
        public Nullable<System.Double> AverageCost { get; set; }

        public string BinNumber { get; set; }

        public long? BinID { get; set; }
        public Guid? BinGUID { get; set; }

        public double? CountCustomerOwnedQuantity { get; set; }
        public double? CountConsignedQuantity { get; set; }
        public string CountLineItemDescriptionEntry { get; set; }
        public Nullable<System.Int32> StockOutCount { get; set; }

        // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "CostUOMID", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int64> CostUOMID { get; set; }
        public Nullable<System.Int32> CostUOMValue { get; set; }
        public int MonthValue { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "OrderUOMID", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int64> OrderUOMID { get; set; }
        public Nullable<System.Int32> OrderUOMValue { get; set; }

        public System.String WhatWhereAction { get; set; }
        //OnOrderQuantity
        [Display(Name = "OnReturnQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> OnReturnQuantity { get; set; }
        public bool IsBOMItem { get; set; }
        public long? RefBomI { get; set; }

        public System.String CostUOMName { get; set; }
        public System.String OrderUOMName { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }

        public long? RefBomId { get; set; }

        public long? RownumberCost { get; set; }

        [Display(Name = "TrendingSetting", ResourceType = typeof(ResItemMaster))]
        public Nullable<byte> TrendingSetting { get; set; }

        [Display(Name = "PullQtyScanOverride", ResourceType = typeof(ResItemMaster))]
        public bool PullQtyScanOverride { get; set; }

        //Trend
        [Display(Name = "IsAutoInventoryClassification", ResourceType = typeof(ResItemMaster))]
        public Boolean IsAutoInventoryClassification { get; set; }

        [Range(0, float.MaxValue)]
        public Double LastCost { get; set; }

        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public string ItemUDF6 { get; set; }
        public string ItemUDF7 { get; set; }
        public string ItemUDF8 { get; set; }
        public string ItemUDF9 { get; set; }
        public string ItemUDF10 { get; set; }
        public int ItemTraking { get; set; }

        public string BlanketOrderNumber { get; set; }
        public List<BinAutoComplete> BinAutoComplete { get; set; }

        public Nullable<Int64> ParentBinId { get; set; }
        public string ParentBinName { get; set; }

        public string ImageType { get; set; }
        public string ItemBlanketPO { get; set; }

        public int IsItemInUse { get; set; }
        public bool IsELabel { get; set; }
        public List<BinMasterDTO> lstItemLocations
        {
            get
            {
                List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
                if (!string.IsNullOrWhiteSpace(_xmlItemLocations))
                {
                    XElement ilXElement = XElement.Parse(_xmlItemLocations);
                    if (ilXElement.HasElements)
                    {

                        ilXElement.Elements("ItemLocation").ToList().ForEach(t =>
                        {
                            Guid _itmguid = Guid.Empty;
                            double _minQty = 0;
                            double _maxQty = 0;
                            double _critQty = 0;
                            int _isDefault = 0;

                            BinMasterDTO objil = new BinMasterDTO();
                            if (Guid.TryParse(Convert.ToString(t.Element("ItemGUID").Value), out _itmguid))
                            {
                                objil.ItemGUID = _itmguid;
                            }
                            objil.BinNumber = t.Element("BinNumber").Value;
                            if (double.TryParse(Convert.ToString(t.Element("MinimumQuantity").Value), out _minQty))
                            {
                                objil.MinimumQuantity = _minQty;
                            }
                            if (double.TryParse(Convert.ToString(t.Element("MaximumQuantity").Value), out _maxQty))
                            {
                                objil.MaximumQuantity = _maxQty;
                            }
                            if (double.TryParse(Convert.ToString(t.Element("CriticalQuantity").Value), out _critQty))
                            {
                                objil.CriticalQuantity = _critQty;
                            }
                            if (t.Elements("IsDefault").Any())
                            {
                                if (int.TryParse(Convert.ToString(t.Element("IsDefault").Value), out _isDefault))
                                {
                                    if (_isDefault == 1)
                                    {
                                        objil.IsDefault = true;
                                    }
                                    else
                                    {
                                        objil.IsDefault = false;
                                    }

                                }
                            }
                            lstBins.Add(objil);
                        });


                    }
                    return lstBins.OrderBy(t => t.BinNumber).ToList();
                }
                else
                {
                    return lstBins;
                }
            }
        }

        //IsPackslipMandatoryAtReceive
        [Display(Name = "IsPackslipMandatoryAtReceive", ResourceType = typeof(ResItemMaster))]
        public Boolean IsPackslipMandatoryAtReceive { get; set; }
        public long EnterpriseId { get; set; }

        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate))
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(Created, true);
                }
                return _createdDate;
            }
            set { this._createdDate = value; }
        }
        private string _HistoryDate;
        public string HistoryDate
        {
            get
            {
                if (string.IsNullOrEmpty(_HistoryDate))
                {
                    _HistoryDate = FnCommon.ConvertDateByTimeZone(HistoryOn, true);
                }
                return _HistoryDate;
            }
            set { this._HistoryDate = value; }
        }

        private string _updatedDate;
        public string UpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_updatedDate))
                {
                    _updatedDate = FnCommon.ConvertDateByTimeZone(Updated, true);
                }
                return _updatedDate;
            }
            set { this._updatedDate = value; }
        }

        private string _ReceivedOn;
        public string ReceivedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOn))
                {
                    _ReceivedOn = FnCommon.ConvertDateByTimeZone(ReceivedOn, true);
                }
                return _ReceivedOn;
            }
            set { this._ReceivedOn = value; }
        }

        private string _ReceivedOnWeb;
        public string ReceivedOnDateWeb
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOnWeb))
                {
                    _ReceivedOnWeb = FnCommon.ConvertDateByTimeZone(ReceivedOnWeb, true);
                }
                return _ReceivedOnWeb;
            }
            set { this._ReceivedOnWeb = value; }
        }
        //EnterPriseId

        //public Nullable<System.Int64> EnterPriseId { get; set; }
        //public string SessionRoomName { get; set; }

        //public string CompanyName { get; set; }

        public string BPONumber { get; set; }
        public Nullable<DateTime> PulledDate { get; set; }
        private string _PulledDate;
        public string PulledDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_PulledDate))
                {
                    _PulledDate = FnCommon.ConvertDateByTimeZone(PulledDate, true);
                }
                return _PulledDate;
            }
            set { this._PulledDate = value; }
        }
        public Nullable<DateTime> OrderedDate { get; set; }
        private string _OrderedDate;
        public string OrderedDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_OrderedDate))
                {
                    _OrderedDate = FnCommon.ConvertDateByTimeZone(OrderedDate, true);
                }
                return _OrderedDate;
            }
            set { this._OrderedDate = value; }
        }
        public Nullable<DateTime> CountedDate { get; set; }
        private string _CountedDate;
        public string CountedDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_CountedDate))
                {
                    _CountedDate = FnCommon.ConvertDateByTimeZone(CountedDate, true);
                }
                return _CountedDate;
            }
            set { this._CountedDate = value; }
        }
        public Nullable<DateTime> TrasnferedDate { get; set; }
        private string _TrasnferedDate;
        public string TrasnferedDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_TrasnferedDate))
                {
                    _TrasnferedDate = FnCommon.ConvertDateByTimeZone(TrasnferedDate, true);
                }
                return _TrasnferedDate;
            }
            set { this._TrasnferedDate = value; }
        }
        //ManufacturerID
        [Display(Name = "PriceSavedDate", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.DateTime> PriceSavedDate { get; set; }
        private string _PriceSavedDate;


        public string PriceSavedDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_PriceSavedDate))
                {
                    _PriceSavedDate = FnCommon.ConvertDateByTimeZone(PriceSavedDate, true);
                }
                return _PriceSavedDate;
            }
            set { this._PriceSavedDate = value; }
        }

        //QuantityPerKit
        [Display(Name = "QtyToMeetDemand", ResourceType = typeof(ResKitMaster))]
        public Nullable<System.Double> QtyToMeetDemand { get; set; }

        public Nullable<System.Double> QLCreditQuantity { get; set; }
        public bool? IsDefaultProjectSpend { get; set; }
        public string DefaultProjectSpend { get; set; }
        public Guid? DefaultProjectSpendGuid { get; set; }
        //public DateTime? trasnfereddate { get; set; }
        //public DateTime? ordereddate { get; set; }
        //public DateTime? pulleddate { get; set; }
        //public DateTime? counteddate { get; set; }
        [Display(Name = "OutTransferQuantity", ResourceType = typeof(ResItemMaster))]
        public double? OutTransferQuantity { get; set; }

        //OnOrderQuantity
        [Display(Name = "OnOrderInTransitQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> OnOrderInTransitQuantity { get; set; }


        //OnTransferQuantity
        [Display(Name = "OnTransferInTransitQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> OnTransferInTransitQuantity { get; set; }

        [Display(Name = "ItemLink2ExternalURL", ResourceType = typeof(ResItemMaster))]
        [StringLength(4000, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ItemLink2ExternalURL { get; set; }
        public string ItemLink2ImageType { get; set; }
        [Display(Name = "IsActive", ResourceType = typeof(ResItemMaster))]
        public bool IsActive { get; set; }

        [Display(Name = "QuanityPulled", ResourceType = typeof(ResItemMaster))]
        public double? QuanityPulled { get; set; }
        public long? MonthlyAverageUsage { get; set; }

        [Display(Name = "PerItemCost", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> PerItemCost { get; set; }
        public bool IsstagingLocation { get; set; }

        public Nullable<System.Double> ItemDefaultPullQuantity { get; set; }
        public Nullable<System.Double> ItemDefaultReorderQuantity { get; set; }
        public Nullable<System.DateTime> ItemIsActiveDate { get; set; }

        public Nullable<System.Double> BinDefaultPullQuantity { get; set; }
        public Nullable<System.Double> BinDefaultReorderQuantity { get; set; }


        [Display(Name = "IsAllowOrderCostuom", ResourceType = typeof(ResItemMaster))]
        public bool IsAllowOrderCostuom { get; set; }

        [Display(Name = "SuggestedReturnQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> SuggestedReturnQuantity { get; set; }

        public int TotalRecords { get; set; }

        public Nullable<System.Double> OrderItemCost { get; set; }

        public string eVMISensorPort { get; set; }

        public Nullable<System.Double> eVMISensorID { get; set; }

        [Display(Name = "GetWeightPerPiece", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> GetWeightPerPiece { get; set; }


        [Display(Name = "WeightVariance", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0, 100, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> WeightVariance { get; set; }

        [Display(Name = "IsOrderable", ResourceType = typeof(ResItemMaster))]
        public bool IsOrderable { get; set; }

        [Display(Name = "OnQuotedQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> OnQuotedQuantity { get; set; }

        public bool? IsRed { get; set; }
        public bool? IsYellow { get; set; }
        public bool? IsGreen { get; set; }
        public bool? IsItemAddedFromAB { get; set; }

        public string BinUDF1 { get; set; }
        public string BinUDF2 { get; set; }
        public string BinUDF3 { get; set; }
        public string BinUDF4 { get; set; }
        public string BinUDF5 { get; set; }
        public bool? IsBinEnforceDefaultReorderQuantity { get; set; }

        [Display(Name = "eLabelKey", ResourceType = typeof(ResItemMaster))]
        [StringLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string eLabelKey { get; set; }

        [Display(Name = "EnrichedProductData", ResourceType = typeof(ResItemMaster))]
        [StringLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string EnrichedProductData { get; set; }

        [Display(Name = "EnhancedDescription", ResourceType = typeof(ResItemMaster))]
        [StringLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string EnhancedDescription { get; set; }

        [Display(Name = "POItemLineNumber", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int32> POItemLineNumber { get; set; }

        public string SolumMappedLabels { get; set; }
        public string SolumUnMappedLabels { get; set; }
    }

    public class ItemLocationSendMailDTO
    {
        public int SrNo { get; set; }
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }
        public Nullable<System.Double> Cost { get; set; }
        public string BinNumber { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.Double> ConsignedQuantity { get; set; }
        public string ItemNumber { get; set; }
    }

    public class ResItemMaster
    {
        private static string ResourceFileName = "ResItemMaster";

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string Action
        {
            get
            {
                return ResourceRead.GetResourceValue("Action", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to BinNumber.
        /// </summary>
        public static string BinNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("BinNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string TrendingSetting
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingSetting", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string TrendingSettingNone
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingSettingNone", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string TrendingSettingAutomatic
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingSettingAutomatic", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string TrendingSettingManual
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingSettingManual", ResourceFileName);
            }
        }
        public static string ItemLink2ExternalURL
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLink2ExternalURL", ResourceFileName);
            }
        }
        public static string IsActive
        {
            get
            {
                return ResourceRead.GetResourceValue("IsActive", ResourceFileName);
            }
        }
        public static string IsOrderable
        {
            get
            {
                return ResourceRead.GetResourceValue("IsOrderable", ResourceFileName);
            }
        }
        


        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string PullQtyScanOverride
        {
            get
            {
                return ResourceRead.GetResourceValue("PullQtyScanOverride", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemMaster {0} already exist! Try with Another!.
        /// </summary>
        /// 
        public static string Duplicate
        {
            get
            {
                return ResourceRead.GetResourceValue("Duplicate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to HistoryID.
        /// </summary>
        public static string HistoryID
        {
            get
            {
                return ResourceRead.GetResourceValue("HistoryID", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Include Archived:.
        /// </summary>
        public static string IncludeArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Include Deleted:.
        /// </summary>
        public static string IncludeDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeDeleted", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Search.
        /// </summary>
        public static string Search
        {
            get
            {
                return ResourceRead.GetResourceValue("Search", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemMaster.
        /// </summary>
        public static string ItemMasterHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemMasterHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemMaster.
        /// </summary>
        public static string ItemMaster
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemMaster", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to View History.
        /// </summary>
        public static string ViewHistory
        {
            get
            {
                return ResourceRead.GetResourceValue("ViewHistory", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ID.
        /// </summary>
        public static string ID
        {
            get
            {
                return ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }

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
        public static string PullButton
        {
            get
            {
                return ResourceRead.GetResourceValue("PullButton", ResourceFileName);
            }

        }

        public static string StagingHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("StagingHeader", ResourceFileName);
            }

        }

        /// <summary>
        ///   Looks up a localized string similar to ManufacturerID.
        /// </summary>
        public static string ManufacturerID
        {
            get
            {
                return ResourceRead.GetResourceValue("ManufacturerID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ManufacturerNumber.
        /// </summary>
        public static string ManufacturerNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ManufacturerNumber", ResourceFileName);
            }
        }

        public static string ManufacturerName
        {
            get
            {
                return ResourceRead.GetResourceValue("ManufacturerName", ResourceFileName);
            }
        }

        public static string Supplier
        {
            get
            {
                return ResourceRead.GetResourceValue("Supplier", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SupplierID.
        /// </summary>
        public static string SupplierID
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SupplierPartNo.
        /// </summary>
        public static string SupplierPartNo
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierPartNo", ResourceFileName);
            }
        }

        public static string BPONumber
        {
            get
            {
                return ResourceRead.GetResourceValue("BPONumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UPC.
        /// </summary>
        public static string UPC
        {
            get
            {
                return ResourceRead.GetResourceValue("UPC", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UNSPSC.
        /// </summary>
        public static string UNSPSC
        {
            get
            {
                return ResourceRead.GetResourceValue("UNSPSC", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Description.
        /// </summary>
        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LongDescription.
        /// </summary>
        public static string LongDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("LongDescription", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CategoryID.
        /// </summary>
        public static string CategoryID
        {
            get
            {
                return ResourceRead.GetResourceValue("CategoryID", ResourceFileName);
            }
        }
                
        public static string CategoryName
        {
            get
            {
                return ResourceRead.GetResourceValue("CategoryID", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to GLAccountID.
        /// </summary>
        public static string GLAccountID
        {
            get
            {
                return ResourceRead.GetResourceValue("GLAccountID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UOMID.
        /// </summary>
        public static string UOMID
        {
            get
            {
                return ResourceRead.GetResourceValue("UOMID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PricePerTerm.
        /// </summary>
        public static string PricePerTerm
        {
            get
            {
                return ResourceRead.GetResourceValue("PricePerTerm", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to DefaultReorderQuantity.
        /// </summary>
        public static string DefaultReorderQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultReorderQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to DefaultPullQuantity.
        /// </summary>
        public static string DefaultPullQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultPullQuantity", ResourceFileName);
            }
        }

        ///// <summary>
        /////   Looks up a localized string similar to DefaultCartQuantity.
        ///// </summary>
        //public static string DefaultcartQuantity
        //{
        //    get
        //    {
        //        return ResourceRead.GetResourceValue("DefaultCartQuantity", ResourceFileName);
        //    }
        //}
        /// <summary>
        ///   Looks up a localized string similar to Cost.
        /// </summary>
        public static string Cost
        {
            get
            {
                return ResourceRead.GetResourceValue("Cost", ResourceFileName);
            }
        }

        public static string CostUOMName
        {
            get
            {
                return ResourceRead.GetResourceValue("CostUOMName", ResourceFileName);
            }
        }

        public static string OrderUOM
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderUOM", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Markup.
        /// </summary>
        public static string Markup
        {
            get
            {
                return ResourceRead.GetResourceValue("Markup", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SellPrice.
        /// </summary>
        public static string SellPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("SellPrice", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ExtendedCost.
        /// </summary>
        public static string ExtendedCost
        {
            get
            {
                return ResourceRead.GetResourceValue("ExtendedCost", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LeadTimeInDays.
        /// </summary>
        public static string LeadTimeInDays
        {
            get
            {
                return ResourceRead.GetResourceValue("LeadTimeInDays", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Link1.
        /// </summary>
        public static string Link1
        {
            get
            {
                return ResourceRead.GetResourceValue("Link1", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Link2.
        /// </summary>
        public static string Link2
        {
            get
            {
                return ResourceRead.GetResourceValue("Link2", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Trend.
        /// </summary>
        public static string Trend
        {
            get
            {
                return ResourceRead.GetResourceValue("Trend", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Trend.
        /// </summary>
        public static string IsAutoInventoryClassification
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAutoInventoryClassification", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Taxable.
        /// </summary>
        public static string Taxable
        {
            get
            {
                return ResourceRead.GetResourceValue("Taxable", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Consignment.
        /// </summary>
        public static string Consignment
        {
            get
            {
                return ResourceRead.GetResourceValue("Consignment", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to StagedQuantity.
        /// </summary>
        public static string StagedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("StagedQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InTransitquantity.
        /// </summary>
        public static string InTransitquantity
        {
            get
            {
                return ResourceRead.GetResourceValue("InTransitquantity", ResourceFileName);
            }
        }
        public static string InTransferTransitQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("InTransferTransitQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OnOrderQuantity.
        /// </summary>
        public static string OnOrderQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("OnOrderQuantity", ResourceFileName);
            }
        }
        public static string OnOrderInTransitQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("OnOrderInTransitQuantity", ResourceFileName);
            }
        }
        public static string OnTransferInTransitQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("OnTransferInTransitQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OnOrderQuantity.
        /// </summary>
        public static string OnReturnQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("OnReturnQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OnTransferQuantity.
        /// </summary>
        public static string OnTransferQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("OnTransferQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SuggestedOrderQuantity.
        /// </summary>
        public static string SuggestedOrderQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedOrderQuantity", ResourceFileName);
            }
        }
        public static string RequestedTransferQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestedTransferQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SuggestedTransferQuantity.
        /// </summary>
        public static string SuggestedTransferQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedTransferQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RequisitionedQuantity.
        /// </summary>
        public static string RequisitionedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionedQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to QtyToMeetDemand.
        /// </summary>
        public static string QtyToMeetDemand
        {
            get
            {
                return ResourceRead.GetResourceValue("QtyToMeetDemand", ResourceFileName);
            }
        }

        /// <summary>
        /// Looks up a localized string similar to PackingQuantity.
        /// </summary>
        public static string PackingQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("PackingQuantity", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to AverageUsage.
        /// </summary>
        public static string AverageUsage
        {
            get
            {
                return ResourceRead.GetResourceValue("AverageUsage", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Turns.
        /// </summary>
        public static string Turns
        {
            get
            {
                return ResourceRead.GetResourceValue("Turns", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OnHandQuantity.
        /// </summary>
        public static string OnHandQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("OnHandQuantity", ResourceFileName);
            }
        }
        public static string ItemLocationOH
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocationOH", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CriticalQuantity.
        /// </summary>
        public static string CriticalQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CriticalQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MinimumQuantity.
        /// </summary>
        public static string MinimumQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MinimumQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MaximumQuantity.
        /// </summary>
        public static string MaximumQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MaximumQuantity", ResourceFileName);
            }
        }
        public static string ConsignedItemMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignedItemMessage", ResourceFileName);
            }
        }
        public static string IsAllowOrderCostuomItemMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAllowOrderCostuomItemMessage", ResourceFileName);
            }
        }

        public static string ImagePathProper
        {
            get
            {
                return ResourceRead.GetResourceValue("ImagePathProper", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to WeightPerPiece.
        /// </summary>
        public static string WeightPerPiece
        {
            get
            {
                return ResourceRead.GetResourceValue("WeightPerPiece", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemUniqueNumber.
        /// </summary>
        public static string ItemUniqueNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemUniqueNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TransferOrPurchase.
        /// </summary>
        public static string IsTransfer
        {
            get
            {
                return ResourceRead.GetResourceValue("IsTransfer", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TransferOrPurchase.
        /// </summary>
        public static string IsPurchase
        {
            get
            {
                return ResourceRead.GetResourceValue("IsPurchase", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TransferOrPurchase.
        /// </summary>
        public static string DefaultLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultLocation", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InventoryClassification.
        /// </summary>
        public static string InventoryClassification
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryClassification", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SerialNumberTracking.
        /// </summary>
        public static string SerialNumberTracking
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialNumberTracking", ResourceFileName);
            }
        }
        public static string OutOfStock
        {
            get
            {
                return ResourceRead.GetResourceValue("OutOfStock", ResourceFileName);
            }
        }
        public static string BelowCritical
        {
            get
            {
                return ResourceRead.GetResourceValue("BelowCritical", ResourceFileName);
            }
        }
        public static string BelowMinimum
        {
            get
            {
                return ResourceRead.GetResourceValue("BelowMinimum", ResourceFileName);
            }
        }
        public static string AboveMaximum
        {
            get
            {
                return ResourceRead.GetResourceValue("AboveMaximum", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to LotNumberTracking.
        /// </summary>
        public static string LotNumberTracking
        {
            get
            {
                return ResourceRead.GetResourceValue("LotNumberTracking", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to DateCodeTracking.
        /// </summary>
        public static string DateCodeTracking
        {
            get
            {
                return ResourceRead.GetResourceValue("DateCodeTracking", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemType.
        /// </summary>
        public static string ItemType
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemType", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ImagePath.
        /// </summary>
        public static string ImagePath
        {
            get
            {
                return ResourceRead.GetResourceValue("ImagePath", ResourceFileName);
            }
        }
        public static string MonthlyAverageUsage
        {
            get
            {
                return ResourceRead.GetResourceValue("MonthlyAverageUsage", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to CriticalQuantityMSG.
        /// </summary>
        public static string CriticalQuantityMSG
        {
            get
            {
                return ResourceRead.GetResourceValue("CriticalQuantityMSG", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MinimumQuantityMSG.
        /// </summary>
        public static string MinimumQuantityMSG
        {
            get
            {
                return ResourceRead.GetResourceValue("MinimumQuantityMSG", ResourceFileName);
            }
        }
        public static string InActiveItemMSG
        {
            get
            {
                return ResourceRead.GetResourceValue("InActiveItemMSG", ResourceFileName);
            }
        }

        public static string ReceivedOnDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedOnDate", ResourceFileName);
            }
        }

        public static string ReceivedOnWeb
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedOnWeb", ResourceFileName);
            }
        }


        public static string AddedFrom
        {
            get
            {
                return ResourceRead.GetResourceValue("AddedFrom", ResourceFileName);
            }
        }

        public static string EditedFrom
        {
            get
            {
                return ResourceRead.GetResourceValue("EditedFrom", ResourceFileName);
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
        ///   Looks up a localized string similar to UDF6.
        /// </summary>
        public static string UDF6
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF6", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF7.
        /// </summary>
        public static string UDF7
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF7", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF8.
        /// </summary>
        public static string UDF8
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF8", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF9.
        /// </summary>
        public static string UDF9
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF9", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF10.
        /// </summary>
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF10", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to GUID.
        /// </summary>
        public static string GUID
        {
            get
            {
                return ResourceRead.GetResourceValue("GUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Created.
        /// </summary>
        public static string Created
        {
            get
            {
                return ResourceRead.GetResourceValue("Created", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Updated.
        /// </summary>
        public static string Updated
        {
            get
            {
                return ResourceRead.GetResourceValue("Updated", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CreatedBy.
        /// </summary>
        public static string CreatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LastUpdatedBy.
        /// </summary>
        public static string LastUpdatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("LastUpdatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsDeleted.
        /// </summary>
        public static string IsDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDeleted", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsArchived.
        /// </summary>
        public static string IsArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IsArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string CompanyID
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room.
        /// </summary>
        public static string Room
        {
            get
            {
                return ResourceRead.GetResourceValue("Room", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsLotSerialExpiryCost.
        /// </summary>
        public static string IsLotSerialExpiryCost
        {
            get
            {
                return ResourceRead.GetResourceValue("IsLotSerialExpiryCost", ResourceFileName);
            }
        }


        ///   Looks up a localized string similar to eTurns: Job Types.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }///   Looks up a localized string similar to eTurns: Job Types.
         /// </summary>
        public static string lblPageMediaUploadTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("lblPageMediaUploadTitle", ResourceFileName);
            }
        }
        public static string lblAddQuantityOnLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("lblAddQuantityOnLocation", ResourceFileName);
            }
        }
        public static string lblAddLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("lblAddLocation", ResourceFileName);
            }
        }
        public static string IsBuildBreak
        {
            get
            {
                return ResourceRead.GetResourceValue("IsBuildBreak", ResourceFileName);
            }
        }

        public static string AverageCost
        {
            get
            {
                return ResourceRead.GetResourceValue("AverageCost", ResourceFileName);
            }
        }

        public static string IsEnforceDefaultReorderQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("IsEnforceDefaultReorderQuantity", ResourceFileName);
            }
        }

        public static string IsItemLevelMinMaxQtyRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("IsItemLevelMinMaxQtyRequired", ResourceFileName);
            }
        }

        public static string GeneralDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("GeneralDetails", ResourceFileName);
            }
        }

        public static string KitDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("KitDetails", ResourceFileName);
            }
        }

        public static string ManufacturerDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("ManufacturerDetails", ResourceFileName);
            }
        }

        public static string SupplierDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierDetails", ResourceFileName);
            }
        }

        public static string Prices
        {
            get
            {
                return ResourceRead.GetResourceValue("Prices", ResourceFileName);
            }
        }

        public static string OtherDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("OtherDetails", ResourceFileName);
            }
        }

        public static string QuantityDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityDetails", ResourceFileName);
            }
        }

        public static string TrackingDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("TrackingDetails", ResourceFileName);
            }
        }

        public static string UDFDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("UDFDetails", ResourceFileName);
            }
        }

        public static string KitPartNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("KitPartNumber", ResourceFileName);
            }
        }

        public static string OnedefaultSupplierisRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("OnedefaultSupplierisRequired", ResourceFileName);
            }
        }


        public static string BondedInventory
        {
            get
            {
                return ResourceRead.GetResourceValue("BondedInventory", ResourceFileName);
            }
        }

        public static string CostUOMID
        {
            get
            {
                return ResourceRead.GetResourceValue("CostUOMID", ResourceFileName);
            }
        }

        public static string OrderUOMID
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderUOMID", ResourceFileName);
            }
        }

        public static string ManageMinMaxCriticalQuantityItemLevel
        {
            get
            {
                return ResourceRead.GetResourceValue("ManageMinMaxCriticalQuantityItemLevel", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsPackslipMandatoryAtReceive.
        /// </summary>
        public static string IsPackslipMandatoryAtReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("IsPackslipMandatoryAtReceive", ResourceFileName);
            }
        }



        public static object Billing
        {
            get
            {
                return ResourceRead.GetResourceValue("Billing", ResourceFileName);
            }
        }
        public static object Expand
        {
            get
            {
                return ResourceRead.GetResourceValue("Expand", ResourceFileName);
            }
        }
        public static object Move
        {
            get
            {
                return ResourceRead.GetResourceValue("Move", ResourceFileName);
            }
        }
        public static object SetBillingAction
        {
            get
            {
                return ResourceRead.GetResourceValue("SetBillingAction", ResourceFileName);
            }
        }
        public static object PriceSavedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("PriceSavedDate", ResourceFileName);
            }
        }
        public static object PulledDate
        {
            get
            {
                return ResourceRead.GetResourceValue("PulledDate", ResourceFileName);
            }
        }
        public static object OrderedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderedDate", ResourceFileName);
            }
        }
        public static object CountedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("CountedDate", ResourceFileName);
            }
        }
        public static object TrasnferedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("TrasnferedDate", ResourceFileName);
            }
        }

        public static String ItemImageExternalURL
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemImageExternalURL", ResourceFileName);
            }
        }


        public static String ItemDocExternalURL
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemDocExternalURL", ResourceFileName);
            }
        }

        public static string BuildBreakOnHandQuantityError
        {
            get
            {
                return ResourceRead.GetResourceValue("BuildBreakOnHandQuantityError", ResourceFileName);
            }
        }

        public static string OutTransferQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("OutTransferQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SuggestedOrderQuantity.
        /// </summary>
        public static string CountQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CountQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SuggestedOrderQuantity.
        /// </summary>
        public static string KitMoveInOutQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("KitMoveInOutQuantity", ResourceFileName);
            }
        }

        public static string RequiredDate
        {
            get
            {
                return ResourceRead.GetResourceValue("RequiredDate", ResourceFileName);
            }
        }
        public static string QuanityPulled
        {
            get
            {
                return ResourceRead.GetResourceValue("QuanityPulled", ResourceFileName);
            }
        }

        public static string PerItemCost
        {
            get
            {
                return ResourceRead.GetResourceValue("PerItemCost", ResourceFileName);
            }
        }

        public static string CostUOMReorderQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("CostUOMReorderQTY", ResourceFileName);
            }
        }

        public static string IsAllowOrderCostuom
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAllowOrderCostuom", ResourceFileName);
            }
        }

        public static string CostUOMMinQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("CostUOMMinQTY", ResourceFileName);
            }
        }
        public static string CostUOMOnOrderQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("CostUOMOnOrderQTY", ResourceFileName);
            }
        }
        public static string CostUOMOnOrderInTransitQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CostUOMOnOrderInTransitQuantity", ResourceFileName);
            }
        }

        public static string SuggestedReturnQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedReturnQuantity", ResourceFileName);
            }
        }
        public static string InvalidImageURL
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidImageURL", ResourceFileName);
            }

        }

        public static string QOHBelowCritical
        {
            get
            {
                return ResourceRead.GetResourceValue("QOHBelowCritical", ResourceFileName);
            }
        }
        public static string QOHBelowMinimum
        {
            get
            {
                return ResourceRead.GetResourceValue("QOHBelowMinimum", ResourceFileName);
            }
        }
        public static string QOHBelowMaximum
        {
            get
            {
                return ResourceRead.GetResourceValue("QOHBelowMaximum", ResourceFileName);
            }
        }
        public static string QOHAboveMaximum
        {
            get
            {
                return ResourceRead.GetResourceValue("QOHAboveMaximum", ResourceFileName);
            }
        }

        public static string DefaultBinSensor
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultBinSensor", ResourceFileName);
            }
        }

        public static string GetWeightPerPiece
        {
            get
            {
                return ResourceRead.GetResourceValue("GetWeightPerPiece", ResourceFileName);
            }
        }

        public static string WeightVariance
        {
            get
            {
                return ResourceRead.GetResourceValue("WeightVariance", ResourceFileName);
            }
        }
        public static string OnQuotedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("OnQuotedQuantity", ResourceFileName);
            }
        }
        public static string QuantitygreaterthanZero
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantitygreaterthanZero", ResourceFileName);
            }
        }
        public static string RequiredTransferOrPurchase
        {
            get
            {
                return ResourceRead.GetResourceValue("RequiredTransferOrPurchase", ResourceFileName);
            }
        }
        public static string RequiredSupplierNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("RequiredSupplierNumber", ResourceFileName);
            }
        }

        public static string MsgItemDoesNotExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgItemDoesNotExist", ResourceFileName);
            }
        }

        public static string MsgSupplierNameRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSupplierNameRequired", ResourceFileName);
            }
        }
        public static string MsgManufacturerNumberExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgManufacturerNumberExist", ResourceFileName);
            }
        }
        public static string MsgManufacturerNameRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgManufacturerNameRequired", ResourceFileName);
            }
        }

        public static string MsgLocationDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLocationDeleted", ResourceFileName);
            }
        }

        public static string MsgLocationInUse
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLocationInUse", ResourceFileName);
            }
        }

        public static string MsgItemMustHaveOneLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgItemMustHaveOneLocation", ResourceFileName);
            }
        }

        public static string MsgMultipleDefaultLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgMultipleDefaultLocation", ResourceFileName);
            }
        }

        public static string MsgOneLocationMustBe
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgOneLocationMustBe", ResourceFileName);
            }
        }

        public static string MsgBinNotExistForItem
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgBinNotExistForItem", ResourceFileName);
            }
        }
        public static string msgKiCannotDelete
        {
            get
            {
                return ResourceRead.GetResourceValue("msgKiCannotDelete", ResourceFileName);
            }
        }
        public static string MsgManufacturerExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgManufacturerExist", ResourceFileName);
            }
        }
        public static string MsgDefaultManufacturerExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDefaultManufacturerExist", ResourceFileName);
            }
        }
        public static string MsgManufacturerRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgManufacturerRequired", ResourceFileName);
            }
        }
        public static string MsgManufacturerNumberRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgManufacturerNumberRequired", ResourceFileName);
            }
        }
        public static string Manufacturerdeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("Manufacturerdeleted", ResourceFileName);
            }
        }
        public static string MsgSupplierRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSupplierRequired", ResourceFileName);
            }
        }

        public static string MsgItemInUsedOfQuotes
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgItemInUsedOfQuotes", ResourceFileName);
            }
        }
        public static string MsgItemInUsedOfCart
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgItemInUsedOfCart", ResourceFileName);
            }
        }
        public static string MsgSupplierNumberRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSupplierNumberRequired", ResourceFileName);
            }
        }
        public static string MsgSupplierExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSupplierExist", ResourceFileName);
            }
        }
        public static string MsgDefaultSupplierExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDefaultSupplierExist", ResourceFileName);
            }
        }
        public static string SupplierDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierDeleted", ResourceFileName);
            }
        }
        public static string ReqQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqQuantity", ResourceFileName);
            }
        }
        public static string MsgSelectItemFromBOM
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectItemFromBOM", ResourceFileName);
            }
        }
        public static string ItemQtyAdjustment
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemQtyAdjustment", ResourceFileName);
            }
        }

        public static string EnterInventoryLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterInventoryLocation", ResourceFileName);
            }
        }
        public static string CannotDeleteConsignedItems
        {
            get
            {
                return ResourceRead.GetResourceValue("CannotDeleteConsignedItems", ResourceFileName);
            }
        }
        public static string LocalImage
        {
            get
            {
                return ResourceRead.GetResourceValue("LocalImage", ResourceFileName);
            }
        }
        public static string ExternalUrl
        {
            get
            {
                return ResourceRead.GetResourceValue("ExternalUrl", ResourceFileName);
            }
        }

        public static string InternalLink
        {
            get
            {
                return ResourceRead.GetResourceValue("InternalLink", ResourceFileName);
            }
        }
        public static string AddFromBOM
        {
            get
            {
                return ResourceRead.GetResourceValue("AddFromBOM", ResourceFileName);
            }
        }

    
        public static string MsgItemNumberNotAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgItemNumberNotAvailable", ResourceFileName);
            }
        }
        public static string MsgTransferORPurchaseRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgTransferORPurchaseRequired", ResourceFileName);
            }
        }
        public static string MsgInvalidURlForLink
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidURlForLink", ResourceFileName);
            }
        }

        public static string MsgKitDeleteValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgKitDeleteValidation", ResourceFileName);
            }
        }


        public static string MsgSelectCriticalQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectCriticalQuantity", ResourceFileName);
            }
        }
        public static string ReqSelectItem
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqSelectItem", ResourceFileName);
            }
        }
        public static string MsgSelectMinimumQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectMinimumQuantity", ResourceFileName);
            }
        }
        public static string MsgSelectMaximumQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectMaximumQuantity", ResourceFileName);
            }
        }
        public static string MsgCriticalMinimumQuantityValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCriticalMinimumQuantityValidation", ResourceFileName);
            }
        }
        public static string MsgMinimumMaximumQuantityValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgMinimumMaximumQuantityValidation", ResourceFileName);
            }
        }

        public static string MsgBlanketPOExpired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgBlanketPOExpired", ResourceFileName);
            }
        }

        public static string MsgAllItemAdded
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAllItemAdded", ResourceFileName);
            }
        }
        public static string MsgItemAddedSuccess
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgItemAddedSuccess", ResourceFileName);
            }
        }
        public static string MsgEnterWeightPieceTotal
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterWeightPieceTotal", ResourceFileName);
            }
        }

        public static string BinBarcodes { get { return ResourceRead.GetResourceValue("BinBarcodes", ResourceFileName); } }
        public static string SerialLotBarcodes { get { return ResourceRead.GetResourceValue("SerialLotBarcodes", ResourceFileName); } }
        public static string CatalogReport { get { return ResourceRead.GetResourceValue("CatalogReport", ResourceFileName); } }
        public static string ItemListReport { get { return ResourceRead.GetResourceValue("ItemListReport", ResourceFileName); } }
        public static string InstockReport { get { return ResourceRead.GetResourceValue("InstockReport", ResourceFileName); } }
        public static string MsgBinHistoryErrorPartial { get { return ResourceRead.GetResourceValue("MsgBinHistoryErrorPartial", ResourceFileName); } }
        public static string MsgFailSupplierDetails { get { return ResourceRead.GetResourceValue("MsgFailSupplierDetails", ResourceFileName); } }
        public static string MsgFailManufacturerDetails { get { return ResourceRead.GetResourceValue("MsgFailManufacturerDetails", ResourceFileName); } }
        public static string MsgFailLocationDetails { get { return ResourceRead.GetResourceValue("MsgFailLocationDetails", ResourceFileName); } }
        public static string MsgFailKitDetails { get { return ResourceRead.GetResourceValue("MsgFailKitDetails", ResourceFileName); } }        
        public static string MsgItemNumberExistforOtherGuid { get { return ResourceRead.GetResourceValue("MsgItemNumberExistforOtherGuid", ResourceFileName); } }
        public static string MsgLicenceAgreementInsertItem
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLicenceAgreementInsertItem", ResourceFileName);
            }
        }
        public static string MsgNoRightsInsertItem
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNoRightsInsertItem", ResourceFileName);
            }
        }
        public static string MsgNoRightsInsertEditItem
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNoRightsInsertEditItem", ResourceFileName);
            }
        }
        public static string MsgNoConsignItemAllowForRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNoConsignItemAllowForRoom", ResourceFileName);
            }
        }
        public static string ItemNumberEmptyValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNumberEmptyValidation", ResourceFileName);
            }
        }
        public static string ConnectionNotAvailable { get { return ResourceRead.GetResourceValue("ConnectionNotAvailable", ResourceFileName); } }
        public static string OrderAlreadyExistForItem { get { return ResourceRead.GetResourceValue("OrderAlreadyExistForItem", ResourceFileName); } }
        public static string RequisitionAlreadyExistForItem { get { return ResourceRead.GetResourceValue("RequisitionAlreadyExistForItem", ResourceFileName); } }
        public static string TransferAlreadyExistForItem { get { return ResourceRead.GetResourceValue("TransferAlreadyExistForItem", ResourceFileName); } }
        public static string WorkorderAlreadyExistForItem { get { return ResourceRead.GetResourceValue("WorkorderAlreadyExistForItem", ResourceFileName); } }
        public static string KitAlreadyExistForItem { get { return ResourceRead.GetResourceValue("KitAlreadyExistForItem", ResourceFileName); } }
        public static string CountAlreadyExistForItem { get { return ResourceRead.GetResourceValue("CountAlreadyExistForItem", ResourceFileName); } }
        
        
        public static string CantChangeConsignedToCustomer { get { return ResourceRead.GetResourceValue("CantChangeConsignedToCustomer", ResourceFileName); } }


        public static string MsgLicenceAgreementEditItem
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLicenceAgreementEditItem", ResourceFileName);
            }
        }
        public static string ReqItemTypeName
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqItemTypeName", ResourceFileName);
            }
        }
        public static string ReqItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqItemNumber", ResourceFileName);
            }
        }
        public static string ReqSupplierPartNo
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqSupplierPartNo", ResourceFileName);
            }
        }
        public static string ReqCompanyName
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqCompanyName", ResourceFileName);
            }
        }
        public static string ReqCriticalQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqCriticalQuantity", ResourceFileName);
            }
        }
        public static string ReqMinimumQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqMinimumQuantity", ResourceFileName);
            }
        }
        public static string ReqMaximumQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqMaximumQuantity", ResourceFileName);
            }
        }
        public static string MsgMinimumgreaterthancriticalQty
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgMinimumgreaterthancriticalQty", ResourceFileName);
            }
        }
        public static string MsgMaximumgreaterthanMinimumQty
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgMaximumgreaterthanMinimumQty", ResourceFileName);
            }
        }

        public static string ItemIsLaborItem { get { return ResourceRead.GetResourceValue("ItemIsLaborItem", ResourceFileName); } }
        public static string ItemIsInActiveItem { get { return ResourceRead.GetResourceValue("ItemIsInActiveItem", ResourceFileName); } }
        public static string ItemIsDeleted { get { return ResourceRead.GetResourceValue("ItemIsDeleted", ResourceFileName); } }
        public static string MsgDefaultReorderQuantityRequired { get { return ResourceRead.GetResourceValue("MsgDefaultReorderQuantityRequired", ResourceFileName); } }
        public static string MsgDefaultPullQuantityRequired { get { return ResourceRead.GetResourceValue("MsgDefaultPullQuantityRequired", ResourceFileName); } }
        public static string MsgUnitRequired { get { return ResourceRead.GetResourceValue("MsgUnitRequired", ResourceFileName); } }
        public static string MsgDefaultLocationNameRequired { get { return ResourceRead.GetResourceValue("MsgDefaultLocationNameRequired", ResourceFileName); } }
        public static string MsgItemPurchaseTransferRequired { get { return ResourceRead.GetResourceValue("MsgItemPurchaseTransferRequired", ResourceFileName); } }
        public static string MsgItemCostUOMRequired { get { return ResourceRead.GetResourceValue("MsgItemCostUOMRequired", ResourceFileName); } }
        public static string MsgEnterProperImageName { get { return ResourceRead.GetResourceValue("MsgEnterProperImageName", ResourceFileName); } }
        public static string MsgExternalURLRequired { get { return ResourceRead.GetResourceValue("MsgExternalURLRequired", ResourceFileName); } }
        public static string MsgItemNumberRequired { get { return ResourceRead.GetResourceValue("MsgItemNumberRequired", ResourceFileName); } }

        public static string InvalidGuid { get { return ResourceRead.GetResourceValue("InvalidGuid", ResourceFileName); } }
        public static string GuidRequired { get { return ResourceRead.GetResourceValue("GuidRequired", ResourceFileName); } }
        public static string ItemNoAlreadyExist { get { return ResourceRead.GetResourceValue("ItemNoAlreadyExist", ResourceFileName); } }
        public static string UserHasNoRightToUpdateItem { get { return ResourceRead.GetResourceValue("UserHasNoRightToUpdateItem", ResourceFileName); } }
        public static string ConsignItemNotAllowForThisRoom { get { return ResourceRead.GetResourceValue("ConsignItemNotAllowForThisRoom", ResourceFileName); } }
        public static string ConsignmentCantBeChanged { get { return ResourceRead.GetResourceValue("ConsignmentCantBeChanged", ResourceFileName); } }
        public static string ProvidePurchaseOrTransfer { get { return ResourceRead.GetResourceValue("ProvidePurchaseOrTransfer", ResourceFileName); } }
        public static string TrendingSettingMustBeNoneManualAutomatic { get { return ResourceRead.GetResourceValue("TrendingSettingMustBeNoneManualAutomatic", ResourceFileName); } }
        public static string InvalidGLAccount { get { return ResourceRead.GetResourceValue("InvalidGLAccount", ResourceFileName); } }
        public static string UserHasNoRightToInsertCategory { get { return ResourceRead.GetResourceValue("UserHasNoRightToInsertCategory", ResourceFileName); } }
        public static string UserHasNoRightToInsertInventoryClassification { get { return ResourceRead.GetResourceValue("UserHasNoRightToInsertInventoryClassification", ResourceFileName); } }
        public static string UserHasNoRightToInsertUOM { get { return ResourceRead.GetResourceValue("UserHasNoRightToInsertUOM", ResourceFileName); } }
        public static string UserHasNoRightToInsertCostUOM { get { return ResourceRead.GetResourceValue("UserHasNoRightToInsertCostUOM", ResourceFileName); } }
        public static string DuplicateManufacturerName { get { return ResourceRead.GetResourceValue("DuplicateManufacturerName", ResourceFileName); } }
        public static string ManufacturerNameOrNoIsRequired { get { return ResourceRead.GetResourceValue("ManufacturerNameOrNoIsRequired", ResourceFileName); } }
        public static string UserHasNoRightToInsertManufacturer { get { return ResourceRead.GetResourceValue("UserHasNoRightToInsertManufacturer", ResourceFileName); } }
        public static string ItemCantHaveMultipleDefaultSupplier { get { return ResourceRead.GetResourceValue("ItemCantHaveMultipleDefaultSupplier", ResourceFileName); } }
        public static string BinMinQtyShouldBeGreaterThanCriticalQty { get { return ResourceRead.GetResourceValue("BinMinQtyShouldBeGreaterThanCriticalQty", ResourceFileName); } }
        public static string BinMaxQtyShouldBeGreaterThanMinQty { get { return ResourceRead.GetResourceValue("BinMaxQtyShouldBeGreaterThanMinQty", ResourceFileName); } }
        public static string UserHasNoRightToInsertLocation { get { return ResourceRead.GetResourceValue("UserHasNoRightToInsertLocation", ResourceFileName); } }
        public static string OneDefaultLocationIsRequired { get { return ResourceRead.GetResourceValue("OneDefaultLocationIsRequired", ResourceFileName); } }
        public static string CantDeleteExistingBin { get { return ResourceRead.GetResourceValue("CantDeleteExistingBin", ResourceFileName); } }
        public static string ItemNoIsRequiredForKitPart { get { return ResourceRead.GetResourceValue("ItemNoIsRequiredForKitPart", ResourceFileName); } }
        public static string ItemNoProvidedAsKitPartDoesNotExist { get { return ResourceRead.GetResourceValue("ItemNoProvidedAsKitPartDoesNotExist", ResourceFileName); } }
        public static string NotAllowedToAddNewKitPart { get { return ResourceRead.GetResourceValue("NotAllowedToAddNewKitPart", ResourceFileName); } }
        public static string NotAllowedToEditQtyPerKitForKitPart { get { return ResourceRead.GetResourceValue("NotAllowedToEditQtyPerKitForKitPart", ResourceFileName); } }
        public static string AtleaseOneKitComponentRequired { get { return ResourceRead.GetResourceValue("AtleaseOneKitComponentRequired", ResourceFileName); } }
        public static string ItemCantHaveKitDetails { get { return ResourceRead.GetResourceValue("ItemCantHaveKitDetails", ResourceFileName); } }
        public static string DefaultReorderQtyIsNotMatchedWithCostUOM { get { return ResourceRead.GetResourceValue("DefaultReorderQtyIsNotMatchedWithCostUOM", ResourceFileName); } }
        public static string MinQtyShouldBeGreaterThanCriticalQty { get { return ResourceRead.GetResourceValue("MinQtyShouldBeGreaterThanCriticalQty", ResourceFileName); } }
        public static string MaxQtyShouldBeGreaterThanMinQty { get { return ResourceRead.GetResourceValue("MaxQtyShouldBeGreaterThanMinQty", ResourceFileName); } }
        public static string ProvideCorrectFileContent { get { return ResourceRead.GetResourceValue("ProvideCorrectFileContent", ResourceFileName); } }
        public static string ProvideCorrectFileName { get { return ResourceRead.GetResourceValue("ProvideCorrectFileName", ResourceFileName); } }
        public static string ProvideCorrectFileNameForLink2 { get { return ResourceRead.GetResourceValue("ProvideCorrectFileNameForLink2", ResourceFileName); } }
        public static string ImageTypeMustBeImagePathOrExternalImage { get { return ResourceRead.GetResourceValue("ImageTypeMustBeImagePathOrExternalImage", ResourceFileName); } }
        public static string ItemLink2ImageTypeMustBeInternallinkOrExternalurl { get { return ResourceRead.GetResourceValue("ItemLink2ImageTypeMustBeInternallinkOrExternalurl", ResourceFileName); } }
        public static string UserHasNoRightToInsertSupplier { get { return ResourceRead.GetResourceValue("UserHasNoRightToInsertSupplier", ResourceFileName); } }
        public static string TrackingType { get { return ResourceRead.GetResourceValue("TrackingType", ResourceFileName); } }
        public static string SerialOrLotNo { get { return ResourceRead.GetResourceValue("SerialOrLotNo", ResourceFileName); } }
        public static string Item { get { return ResourceRead.GetResourceValue("Item", ResourceFileName); } }
        public static string Kit { get { return ResourceRead.GetResourceValue("Kit", ResourceFileName); } }
        public static string Labor { get { return ResourceRead.GetResourceValue("Labor", ResourceFileName); } }

        public static string BinnumberBlankValidation { get { return ResourceRead.GetResourceValue("BinnumberBlankValidation", ResourceFileName); } }
        public static string CustomerOwnedQty { get { return ResourceRead.GetResourceValue("CustomerOwnedQty", ResourceFileName); } }
        public static string ConsignedQty { get { return ResourceRead.GetResourceValue("ConsignedQty", ResourceFileName); } }
        public static string ExpiryDate { get { return ResourceRead.GetResourceValue("ExpiryDate", ResourceFileName); } }
        public static string AddManufacturer { get { return ResourceRead.GetResourceValue("AddManufacturer", ResourceFileName); } }
        public static string AddSelectedItems { get { return ResourceRead.GetResourceValue("AddSelectedItems", ResourceFileName); } }
        public static string Quantity { get { return ResourceRead.GetResourceValue("Quantity", ResourceFileName); } }
        public static string UOM { get { return ResourceRead.GetResourceValue("UOM", ResourceFileName); } }
        public static string ManufacturerpartNumber { get { return ResourceRead.GetResourceValue("ManufacturerpartNumber", ResourceFileName); } }
        public static string SupplierPartNumber { get { return ResourceRead.GetResourceValue("SupplierPartNumber", ResourceFileName); } }
        public static string AddKitComponent { get { return ResourceRead.GetResourceValue("AddKitComponent", ResourceFileName); } }
        public static string ItemLocations { get { return ResourceRead.GetResourceValue("ItemLocations", ResourceFileName); } }
        public static string ItemMovedSuccess { get { return ResourceRead.GetResourceValue("ItemMovedSuccess", ResourceFileName); } }
        public static string ItemMovedFailure { get { return ResourceRead.GetResourceValue("ItemMovedFailure", ResourceFileName); } }
        public static string QtyToAdd { get { return ResourceRead.GetResourceValue("QtyToAdd", ResourceFileName); } }
        public static string DestStageBin { get { return ResourceRead.GetResourceValue("DestStageBin", ResourceFileName); } }
        public static string PendingQuantity { get { return ResourceRead.GetResourceValue("PendingQuantity", ResourceFileName); } }
        public static string MeasureMethod { get { return ResourceRead.GetResourceValue("MeasureMethod", ResourceFileName); } }
        public static string TransctionStartDate { get { return ResourceRead.GetResourceValue("TransctionStartDate", ResourceFileName); } }
        public static string TransctionEndDate { get { return ResourceRead.GetResourceValue("TransctionEndDate", ResourceFileName); } }
        public static string Daysofsapmple { get { return ResourceRead.GetResourceValue("Daysofsapmple", ResourceFileName); } }
        public static string txnType { get { return ResourceRead.GetResourceValue("txnType", ResourceFileName); } }
        
        public static string BinList { get { return ResourceRead.GetResourceValue("BinList", ResourceFileName); } }
        public static string ItemDefaultBinMustBeRoomDefaultBin { get { return ResourceRead.GetResourceValue("ItemDefaultBinMustBeRoomDefaultBin", ResourceFileName); } }
        public static string SelectItemsWithSerialLotHeader { get { return ResourceRead.GetResourceValue("SelectItemsWithSerialLotHeader", ResourceFileName); } }
        public static string ValidationItemEnforceReOrderQuantity { get { return ResourceRead.GetResourceValue("ValidationItemEnforceReOrderQuantity", ResourceFileName); } }
        public static string MoveAll { get { return ResourceRead.GetResourceValue("MoveAll", ResourceFileName); } }
        public static string TotalQuantity { get { return ResourceRead.GetResourceValue("TotalQuantity", ResourceFileName); } }
        public static string TrackingTypeSerial { get { return ResourceRead.GetResourceValue("TrackingTypeSerial", ResourceFileName); } }
        public static string TrackingTypeLot { get { return ResourceRead.GetResourceValue("TrackingTypeLot", ResourceFileName); } }
        public static string TrackingTypeDateCode { get { return ResourceRead.GetResourceValue("TrackingTypeDateCode", ResourceFileName); } }
        public static string TrackingTypeQL { get { return ResourceRead.GetResourceValue("TrackingTypeQL", ResourceFileName); } }
        public static string IsConsigned { get { return ResourceRead.GetResourceValue("IsConsigned", ResourceFileName); } }
        public static string ConcatedColumnText { get { return ResourceRead.GetResourceValue("ConcatedColumnText", ResourceFileName); } }
        public static string ItemCriticalMinimumMaximumQtyMaxLimit { get { return ResourceRead.GetResourceValue("ItemCriticalMinimumMaximumQtyMaxLimit", ResourceFileName); } }
        public static string AddFromABCatalog { get { return ResourceRead.GetResourceValue("AddFromABCatalog", ResourceFileName); } }
        public static string AddFromPastABOrder { get { return ResourceRead.GetResourceValue("AddFromPastABOrder", ResourceFileName); } }
        public static string Sync { get { return ResourceRead.GetResourceValue("Sync", ResourceFileName); } }
        public static string OrderedItemsSyncSuccessfully { get { return ResourceRead.GetResourceValue("OrderedItemsSyncSuccessfully", ResourceFileName); } }
        public static string FailToSyncOrderedItems { get { return ResourceRead.GetResourceValue("FailToSyncOrderedItems", ResourceFileName); } }
        public static string ItemHasDeleted { get { return ResourceRead.GetResourceValue("ItemHasDeleted", ResourceFileName); } }
        public static string eLabelKey { get { return ResourceRead.GetResourceValue("eLabelKey", ResourceFileName); } }
        public static string EnrichedProductData { get { return ResourceRead.GetResourceValue("EnrichedProductData", ResourceFileName); } }
        public static string EnhancedDescription { get { return ResourceRead.GetResourceValue("EnhancedDescription", ResourceFileName); } }
        public static string POItemLineNumber { get { return ResourceRead.GetResourceValue("POItemLineNumber", ResourceFileName); } }
        public static string AvailableLabels { get { return ResourceRead.GetResourceValue("AvailableLabels", ResourceFileName); } }
        public static string UnAssign { get { return ResourceRead.GetResourceValue("UnAssign", ResourceFileName); } }
        public static string InvalidLabelVal { get { return ResourceRead.GetResourceValue("InvalidLabelVal", ResourceFileName); } }
        public static string SolumHeader { get { return ResourceRead.GetResourceValue("SolumHeader", ResourceFileName); } }


    }

    public class ResItemeVMIList
    {
        private static string ResourceFileName = "ResItemeVMIList";

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }
        public static string InventoryTitle1
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryTitle1", ResourceFileName);
            }
        }
        public static string InventoryTitle2
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryTitle2", ResourceFileName);
            }
        }
        public static string InventorysubTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("InventorysubTitle", ResourceFileName);
            }
        }
        public static string MinOrderLabel
        {
            get
            {
                return ResourceRead.GetResourceValue("MinOrderLabel", ResourceFileName);
            }
        }
        public static string StockLow
        {
            get
            {
                return ResourceRead.GetResourceValue("StockLow", ResourceFileName);
            }
        }
        public static string ReOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("ReOrder", ResourceFileName);
            }
        }
        public static string StockOk
        {
            get
            {
                return ResourceRead.GetResourceValue("StockOk", ResourceFileName);
            }
        }
        public static string Scale
        {
            get
            {
                return ResourceRead.GetResourceValue("Scale", ResourceFileName);
            }
        }
        public static string Pad
        {
            get
            {
                return ResourceRead.GetResourceValue("Pad", ResourceFileName);
            }
        }
        public static string Settings
        {
            get
            {
                return ResourceRead.GetResourceValue("Settings", ResourceFileName);
            }
        }
        public static string Info
        {
            get
            {
                return ResourceRead.GetResourceValue("Info", ResourceFileName);
            }
        }
        public static string ItemStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemStatus", ResourceFileName);
            }
        }
        public static string BelowCritical
        {
            get
            {
                return ResourceRead.GetResourceValue("BelowCritical", ResourceFileName);
            }
        }
        public static string BelowMinimum
        {
            get
            {
                return ResourceRead.GetResourceValue("BelowMinimum", ResourceFileName);
            }
        }
        public static string AboveOrEqualMinimum
        {
            get
            {
                return ResourceRead.GetResourceValue("AboveOrEqualMinimum", ResourceFileName);
            }
        }
    }

}

public class RequiredIfAttribute : RequiredAttribute
{
    private String PropertyName { get; set; }
    private Object DesiredValue { get; set; }

    public RequiredIfAttribute(String propertyName, Object desiredvalue)
    {
        PropertyName = propertyName;
        DesiredValue = desiredvalue;
    }

    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        Object instance = context.ObjectInstance;
        Type type = instance.GetType();
        Object proprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);
        if (proprtyvalue != null)
        {
            if (proprtyvalue.ToString() == DesiredValue.ToString())
            {
                ValidationResult result = base.IsValid(value, context);
                return result;
            }
        }
        return ValidationResult.Success;
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class CriticleQuantityCheckAttribute : ValidationAttribute, IClientValidatable
{
    private readonly string _MinQTYFieldName;
    public CriticleQuantityCheckAttribute(string MinQtyFieldName)
    {
        _MinQTYFieldName = MinQtyFieldName;
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(_MinQTYFieldName);
        var otherPropertyValue = property.GetValue(validationContext.ObjectInstance, null);
        if (value != null)
        {
            double CriQTY = (double)value;
            if (otherPropertyValue == null)
            {
                ValidationResult Ok = new ValidationResult("Please provide minimum quantity.");
                return Ok;
            }
            if (CriQTY > (double)otherPropertyValue)
            {
                ValidationResult Ok = new ValidationResult("Critical quantity must be less than OR equal to minimum");
                return Ok;
            }
        }
        return ValidationResult.Success;
    }
    public IEnumerable<ModelClientValidationRule>
           GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
        yield return new ModelClientValidationRule
        {
            ErrorMessage = ErrorMessage,
            ValidationType = "criticlequantitycheck"
        };
    }

}
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class MinimumQuantityCheckAttribute : ValidationAttribute, IClientValidatable
{
    private readonly string _MaxQTYFieldName;
    public MinimumQuantityCheckAttribute(string MaxQtyFieldName)
    {
        _MaxQTYFieldName = MaxQtyFieldName;
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(_MaxQTYFieldName);
        var otherPropertyValue = property.GetValue(validationContext.ObjectInstance, null);
        if (value != null)
        {
            double MinQTY = (double)value;
            if (otherPropertyValue == null)
            {
                ValidationResult Ok = new ValidationResult("Please provide maximum quantity.");
                return Ok;
            }
            if (MinQTY > (double)otherPropertyValue)
            {
                ValidationResult Ok = new ValidationResult("Minimum quantity must be less than Maximum quantity");
                return Ok;
            }
        }
        return ValidationResult.Success;
    }
    public IEnumerable<ModelClientValidationRule>
           GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
        yield return new ModelClientValidationRule
        {
            ErrorMessage = ErrorMessage,
            ValidationType = "minimumquantitycheck"
        };
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class DefaultReorderQuantityCheckAttribute : ValidationAttribute, IClientValidatable
{
    private readonly string _MaxQTYFieldName;
    public DefaultReorderQuantityCheckAttribute(string MaxQtyFieldName)
    {
        _MaxQTYFieldName = MaxQtyFieldName;
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(_MaxQTYFieldName);
        var otherPropertyValue = property.GetValue(validationContext.ObjectInstance, null);
        if (value != null)
        {
            double DefReQTY = (double)value;
            if (DefReQTY >= (double)otherPropertyValue)
            {
                ValidationResult Ok = new ValidationResult("Default Reorder quantity must be less then Maximum quantity");
                return Ok;
            }
        }
        return ValidationResult.Success;
    }
    public IEnumerable<ModelClientValidationRule>
           GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
        yield return new ModelClientValidationRule
        {
            ErrorMessage = ErrorMessage,
            ValidationType = "defaultreorderquantity"
        };
    }
}

public class RPT_ItemInStokeDTO
{
    public string LocationLevelQauntity { get; set; }
    public string BinNumber { get; set; }
    public string ItemNumber { get; set; }
    public string StagingName { get; set; }
    public string CustomerOwnedQuantity { get; set; }
    public string ConsignedQuantity { get; set; }
    public string LocationQuantity { get; set; }
    public string StageAndGeneralQuantity { get; set; }
    public string ExtendCostStageAndGeneralQuantity { get; set; }
    public string MSLocQuantity { get; set; }
    public string MSExtendedCost { get; set; }
    public string LocationExtendedCost { get; set; }
    public double? Total { get; set; }
    public string ItemUniqueNumber { get; set; }
    public string SupplierName { get; set; }
    public string SupplierPartNo { get; set; }
    public string CategoryName { get; set; }
    public string ItemDescription { get; set; }
    public string ManufacturerName { get; set; }
    public string ManufacturerNumber { get; set; }
    public string GLAccount { get; set; }
    public string UPC { get; set; }
    public string UNSPSC { get; set; }
    public string LongDescription { get; set; }
    public string ItemUDF1 { get; set; }
    public string ItemUDF2 { get; set; }
    public string ItemUDF3 { get; set; }
    public string ItemUDF4 { get; set; }
    public string ItemUDF5 { get; set; }
    public string Link1 { get; set; }
    public string Link2 { get; set; }
    public string ItemWhatWhereAction { get; set; }
    public string ItemCreatedByName { get; set; }
    public string ItemUpdatedByName { get; set; }
    public string ItemRoomName { get; set; }
    public string ItemCompanyName { get; set; }
    public string ItemBlanketPO { get; set; }
    public string Unit { get; set; }
    public string DefaultLocationName { get; set; }
    public string InventoryClassificationName { get; set; }
    public string ItemTypeName { get; set; }
    public string CostUOM { get; set; }
    public int? LeadTimeInDays { get; set; }
    public string ItemCost { get; set; }
    public string SellPrice { get; set; }
    public string ExtendedCost { get; set; }
    public string AverageCost { get; set; }
    public string PricePerTerm { get; set; }
    public string OnHandQuantity { get; set; }
    public string StagedQuantity { get; set; }
    public string ItemInTransitquantity { get; set; }
    public string OnOrderQuantity { get; set; }
    public string OnReturnQuantity { get; set; }
    public string OnTransferQuantity { get; set; }
    public string SuggestedOrderQuantity { get; set; }
    public string RequisitionedQuantity { get; set; }
    public string CriticalQuantity { get; set; }
    public string MinimumQuantity { get; set; }
    public string MaximumQuantity { get; set; }
    public string DefaultReorderQuantity { get; set; }
    public string DefaultPullQuantity { get; set; }
    public string AverageUsage { get; set; }
    public string Turns { get; set; }
    public string Markup { get; set; }
    public string WeightPerPiece { get; set; }
    public string ItemCreatedOn { get; set; }
    public string ItemUpdatedOn { get; set; }
    public string Consignment { get; set; }
    public string ItemIsDeleted { get; set; }
    public string ItemIsArchived { get; set; }
    public string IsTransfer { get; set; }
    public string IsPurchase { get; set; }
    public string SerialNumberTracking { get; set; }
    public string LotNumberTracking { get; set; }
    public string DateCodeTracking { get; set; }
    public string IsBuildBreak { get; set; }
    public string Taxable { get; set; }
    public Int64 ItemID { get; set; }
    public Guid ItemGUID { get; set; }
    public Int64 CompanyID { get; set; }
    public Int64 RoomID { get; set; }
    public string RoomInfo { get; set; }
    public string CompanyInfo { get; set; }
    public string IsItemLevelMinMaxQtyRequired { get; set; }
    public string BarcodeImage_ItemNumber { get; set; }
    public string BarcodeImage_BinNumber { get; set; }
    public int? CostDecimalPoint { get; set; }
    public int? QuantityDecimalPoint { get; set; }

}
public class RPT_temList
{
    public string ItemNumber { get; set; }
    public string ManufacturerNumber { get; set; }
    public string SupplierPartNo { get; set; }
    public string UPC { get; set; }
    public string UNSPSC { get; set; }
    public string ItemUDF1 { get; set; }
    public string ItemUDF2 { get; set; }
    public string ItemUDF3 { get; set; }
    public string ItemUDF4 { get; set; }
    public string ItemUDF5 { get; set; }
    public string SupplierName { get; set; }
    public string ManufacturerName { get; set; }
    public string CategoryName { get; set; }
    public string GLAccount { get; set; }
    public string Unit { get; set; }
    public string InventoryClassificationName { get; set; }
    public string CostUOM { get; set; }
    public string ItemTypeName { get; set; }
    public string IsTransfer { get; set; }
    public string IsPurchase { get; set; }
    public string SerialNumberTracking { get; set; }
    public string LotNumberTracking { get; set; }
    public string DateCodeTracking { get; set; }
    public string IsItemLevelMinMaxQtyRequired { get; set; }
    public string IsEnforceDefaultReorderQuantity { get; set; }
    public string IsAutoInventoryClassification { get; set; }
    public string PullQtyScanOverride { get; set; }
    public long ItemID { get; set; }
    public Guid ItemGUID { get; set; }
    public long ItemCompanyID { get; set; }
    public long ItemRoomID { get; set; }
    public Int64? CategoryID { get; set; }
    public Int64? SupplierID { get; set; }
    public Int64? ManufacturerID { get; set; }
    public string ItemUDF6 { get; set; }
    public string ItemUDF7 { get; set; }
    public string ItemUDF8 { get; set; }
    public string ItemUDF9 { get; set; }
    public string ItemUDF10 { get; set; }
}
public class RRT_InstockByBinDTO
{
    public string ItemNumber { get; set; }
    public string Bin { get; set; }
    public string StageBin { get; set; }
    public string StageAndBin { get; set; }
    public string UOM { get; set; }
    public string Supplier { get; set; }
    public string SupplierPartNo { get; set; }
    public string ItemDescription { get; set; }

    public string LongDescription { get; set; }

    public string Category { get; set; }

    public string CategoryName { get; set; }

    public string Staging { get; set; }
    public double? CriticalQty { get; set; }
    public double? MinQty { get; set; }
    public double? MaxQty { get; set; }
    public double? ReceivedCost { get; set; }
    public double? OnHandQuantity { get; set; }
    public double? StagedQuantity { get; set; }
    public string CostUOM { get; set; }
    public double? ItemCost { get; set; }
    public double? AverageCost { get; set; }
    public double? ExtendedCost { get; set; }
    public double? StageAndGeneralQuantity { get; set; }
    public double? TotalGeneralExtendCost { get; set; }
    public string Manufacturer { get; set; }
    public string ManufacturerPartNo { get; set; }

    public string ManufacturerName { get; set; }
    public string ManufacturerNumber { get; set; }

    public string InventoryClassificationName { get; set; }
    public double? MSExtendedCost { get; set; }
    public double? BinExtendedCost { get; set; }
    public double? Total { get; set; }
    public string RoomInfo { get; set; }
    public string UPC { get; set; }
    public string UNSPSC { get; set; }
    public string ItemUDF1 { get; set; }
    public string ItemUDF2 { get; set; }
    public string ItemUDF3 { get; set; }
    public string ItemUDF4 { get; set; }
    public string ItemUDF5 { get; set; }
    public string ItemUpdatedByName { get; set; }
    public string ItemRoomName { get; set; }
    public string ItemCompanyName { get; set; }
    public Int64 ItemID { get; set; }
    public Guid ItemGUID { get; set; }
    public int CostDecimalPoint { get; set; }
    public int QuantityDecimalPoint { get; set; }
    public string ReceivedOn { get; set; }
    public double? MSBinQty { get; set; }
    public double? BinQty { get; set; }
    public Int64 RoomID { get; set; }
    public Int64 CompanyID { get; set; }

    public Int64? CategoryID { get; set; }

    public Int64? SupplierID { get; set; }

    public Int64? BinID { get; set; }
    public Int32? InventoryClassificationID { get; set; }

    public Int64? ManufacturerID { get; set; }
    public string ItemUDF6 { get; set; }
    public string ItemUDF7 { get; set; }
    public string ItemUDF8 { get; set; }
    public string ItemUDF9 { get; set; }
    public string ItemUDF10 { get; set; }

    public string EnterpriseQLItemGuids { get; set; }
    public int? CostUOMValue { get; set; }
    public double? DefaultReorderQuantity { get; set; }
    public double? DefaultPullQuantity { get; set; }
}
public class RPT_ItemAuditTrailDTO
{
    public Int64 HistoryID { get; set; }
    public string ModuleName { get; set; }
    public string OnAction { get; set; }
    public string ItemNumber { get; set; }
    public string BinName { get; set; }
    public double? TransactionQuantity { get; set; }
    public double? BinQuantity { get; set; }
    public double? OnHandQuantity { get; set; }
    public string Supplier { get; set; }
    public string SupplierPartNumber { get; set; }
    public string UpdatedOn { get; set; }
    public string ValueField { get; set; }
    public string RoomInfo { get; set; }
    public double? Total { get; set; }
    public int? QuantityDecimalPoint { get; set; }
    public int? CostDecimalPoint { get; set; }
    public Int64? ItemID { get; set; }
    public Guid? ItemGuid { get; set; }
    public string UpdatedByName { get; set; }
    public string PackSlipNumber { get; set; }
    public string AddedFrom { get; set; }
    public string CurrentDateTime { get; set; }
    public string Category { get; set; }
    public string Manufacturer { get; set; }
    public string ManufacturerPartNo { get; set; }
    public string ManufacturerPartNumber { get; set; }
    public string UNSPSC { get; set; }
    public string ItemUDF1 { get; set; }
    public string ItemUDF2 { get; set; }
    public string ItemUDF3 { get; set; }
    public string ItemUDF4 { get; set; }
    public string ItemUDF5 { get; set; }
    public string ItemBlanketPO { get; set; }

}

public class RPT_ToolAuditTrailDTO
{
    public Int64 HistoryID { get; set; }
    public string ModuleName { get; set; }
    public string OnAction { get; set; }
    public string ToolName { get; set; }
    public string SerialNumber { get; set; }
    public string BinName { get; set; }
    public double? TransactionQuantity { get; set; }
    public double? BinQuantity { get; set; }
    public double? OnHandQuantity { get; set; }
    public string UpdatedOn { get; set; }
    public string ValueField { get; set; }
    public string RoomInfo { get; set; }
    public double? Total { get; set; }
    public int? QuantityDecimalPoint { get; set; }
    public int? CostDecimalPoint { get; set; }
    public Int64? ToolID { get; set; }
    public Guid? ToolGuid { get; set; }
    public string UpdatedByName { get; set; }
    public string AddedFrom { get; set; }
    public string CurrentDateTime { get; set; }
    public string UDF1 { get; set; }
    public string UDF2 { get; set; }
    public string UDF3 { get; set; }
    public string UDF4 { get; set; }
    public string UDF5 { get; set; }
}

public class BinAutoComplete
{
    public Int64 ID { get; set; }
    public string BinNumber { get; set; }
}
public enum InventoryValuationMethod
{
    AverageCost = 3,
    LastCost = 4
}
public class ExportItemLocationDetailsDTO
{
    public string BinNumber { get; set; }
    public System.String ItemNumber { get; set; }
    public Nullable<System.Double> CustomerOwnedQuantity { get; set; }
    public Nullable<System.Double> ConsignedQuantity { get; set; }
    public System.String SerialNumber { get; set; }
    public System.String LotNumber { get; set; }
    public string Expiration { get; set; }
    public string Received { get; set; }

    public DateTime? ExpirationDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public Nullable<System.Double> Cost { get; set; }
    public System.String eVMISensorPort { get; set; }
    public string eVMISensorPortstr { get; set; }
    public double? eVMISensorIDdbl { get; set; }
    public bool IsDefault { get; set; }
    public Guid GUID { get; set; }
    public Nullable<Guid> ItemGUID { get; set; }
    public System.String eVMISensorID { get; set; }

    public string UDF1 { get; set; }
    public string UDF2 { get; set; }
    public string UDF3 { get; set; }
    public string UDF4 { get; set; }
    public string UDF5 { get; set; }
    public string CountItemDescription { get; set; }
    public int ItemType { get; set; }
}

//public class RPT_ItemExpiringDTO
//{
//    public string LocationLevelQauntity { get; set; }
//    public string BinNumber { get; set; }
//    public string ItemNumber { get; set; }
//    //public string StagingName { get; set; }
//    //public string CustomerOwnedQuantity { get; set; }
//    //public string ConsignedQuantity { get; set; }
//    public string LocationQuantity { get; set; }
//    public string StageAndGeneralQuantity { get; set; }
//    public string ExtendCostStageAndGeneralQuantity { get; set; }
//    //public string MSLocQuantity { get; set; }
//    //public string MSExtendedCost { get; set; }
//    public string LocationExtendedCost { get; set; }
//    public double? Total { get; set; }
//    public string ItemUniqueNumber { get; set; }
//    public string SupplierName { get; set; }
//    public string SupplierPartNo { get; set; }
//    //public string CategoryName { get; set; }
//    public string ItemDescription { get; set; }
//    //public string ManufacturerName { get; set; }
//    //public string ManufacturerNumber { get; set; }
//    //public string GLAccount { get; set; }
//    //public string UPC { get; set; }
//    //public string UNSPSC { get; set; }
//    //public string LongDescription { get; set; }
//    public string ItemUDF1 { get; set; }
//    public string ItemUDF2 { get; set; }
//    public string ItemUDF3 { get; set; }
//    public string ItemUDF4 { get; set; }
//    public string ItemUDF5 { get; set; }
//    //public string Link1 { get; set; }
//   // public string Link2 { get; set; }
//   // public string ItemWhatWhereAction { get; set; }
//    public string ItemCreatedByName { get; set; }
//    public string ItemUpdatedByName { get; set; }
//    public string ItemRoomName { get; set; }
//    public string ItemCompanyName { get; set; }
//   // public string ItemBlanketPO { get; set; }
//   // public string Unit { get; set; }
//    public string DefaultLocationName { get; set; }
//    //public string InventoryClassificationName { get; set; }
//    //public string ItemTypeName { get; set; }
//    public string CostUOM { get; set; }
//    public int? LeadTimeInDays { get; set; }
//    public double ItemCost { get; set; }
//   // public string SellPrice { get; set; }
//   // public string ExtendedCost { get; set; }
//  //  public string AverageCost { get; set; }
//  //  public string PricePerTerm { get; set; }
//    public double OnHandQuantity { get; set; }
//   // public string StagedQuantity { get; set; }
//   // public string ItemInTransitquantity { get; set; }
//    //public string OnOrderQuantity { get; set; }
//   // public string OnReturnQuantity { get; set; }
//   // public string OnTransferQuantity { get; set; }
//   // public string SuggestedOrderQuantity { get; set; }
//   // public string RequisitionedQuantity { get; set; }
//   // public string CriticalQuantity { get; set; }
//    public string MinimumQuantity { get; set; }
//    public string MaximumQuantity { get; set; }
//   // public string DefaultReorderQuantity { get; set; }
//   // public string DefaultPullQuantity { get; set; }
//   // public string AverageUsage { get; set; }
//    //public string Turns { get; set; }
//    //public string Markup { get; set; }
//    //public string WeightPerPiece { get; set; }
//    public string ItemCreatedOn { get; set; }
//    public string ItemUpdatedOn { get; set; }
//   // public string Consignment { get; set; }
//    //public string ItemIsDeleted { get; set; }
//    //public string ItemIsArchived { get; set; }
//    //public string IsTransfer { get; set; }
//   // public string IsPurchase { get; set; }
//    //public string SerialNumberTracking { get; set; }
//    //public string LotNumberTracking { get; set; }
//   // public string DateCodeTracking { get; set; }
//   // public string IsBuildBreak { get; set; }
//   // public string Taxable { get; set; }
//    public Int64 ItemID { get; set; }
//    public Guid ItemGUID { get; set; }
//    public Int64 CompanyID { get; set; }
//    public Int64 RoomID { get; set; }
//    public string RoomInfo { get; set; }
//    public string CompanyInfo { get; set; }
//  //  public string IsItemLevelMinMaxQtyRequired { get; set; }
// //   public string BarcodeImage_ItemNumber { get; set; }
////public string BarcodeImage_BinNumber { get; set; }
// //   public int? CostDecimalPoint { get; set; }
//    //public int? QuantityDecimalPoint { get; set; }

//}
public class RPT_ItemExpiringDTO
{
    public string LocationLevelQauntity { get; set; }
    public string BinNumber { get; set; }
    public string ItemNumber { get; set; }
    //public string StagingName { get; set; }
    //public string CustomerOwnedQuantity { get; set; }
    //public string ConsignedQuantity { get; set; }
    public string LocationQuantity { get; set; }
    public string StageAndGeneralQuantity { get; set; }
    public string ExtendCostStageAndGeneralQuantity { get; set; }
    //public string MSLocQuantity { get; set; }
    //public string MSExtendedCost { get; set; }
    public string LocationExtendedCost { get; set; }
    public double? Total { get; set; }
    public string ItemUniqueNumber { get; set; }
    public string SupplierName { get; set; }
    public string SupplierPartNo { get; set; }
    //public string CategoryName { get; set; }
    public string ItemDescription { get; set; }
    //public string ManufacturerName { get; set; }
    //public string ManufacturerNumber { get; set; }
    //public string GLAccount { get; set; }
    //public string UPC { get; set; }
    //public string UNSPSC { get; set; }
    //public string LongDescription { get; set; }
    public string ItemUDF1 { get; set; }
    public string ItemUDF2 { get; set; }
    public string ItemUDF3 { get; set; }
    public string ItemUDF4 { get; set; }
    public string ItemUDF5 { get; set; }
    //public string Link1 { get; set; }
    // public string Link2 { get; set; }
    // public string ItemWhatWhereAction { get; set; }
    public string ItemCreatedByName { get; set; }
    public string ItemUpdatedByName { get; set; }
    public string ItemRoomName { get; set; }
    public string ItemCompanyName { get; set; }
    // public string ItemBlanketPO { get; set; }
    public string Unit { get; set; }
    public string DefaultLocationName { get; set; }
    //public string InventoryClassificationName { get; set; }
    //public string ItemTypeName { get; set; }
    public string CostUOM { get; set; }
    public int? LeadTimeInDays { get; set; }
    public double? ItemCost { get; set; }
    // public string SellPrice { get; set; }
    // public string ExtendedCost { get; set; }
    public double? AverageCost { get; set; }
    //  public string PricePerTerm { get; set; }
    public double OnHandQuantity { get; set; }
    // public string StagedQuantity { get; set; }
    // public string ItemInTransitquantity { get; set; }
    //public string OnOrderQuantity { get; set; }
    // public string OnReturnQuantity { get; set; }
    // public string OnTransferQuantity { get; set; }
    public double SuggestedOrderQuantity { get; set; }
    // public string RequisitionedQuantity { get; set; }
    // public string CriticalQuantity { get; set; }
    public double MinimumQuantity { get; set; }
    public double MaximumQuantity { get; set; }
    // public string DefaultReorderQuantity { get; set; }
    // public string DefaultPullQuantity { get; set; }
    // public string AverageUsage { get; set; }
    //public string Turns { get; set; }
    //public string Markup { get; set; }
    //public string WeightPerPiece { get; set; }
    public string ItemCreatedOn { get; set; }
    public string ItemUpdatedOn { get; set; }
    // public string Consignment { get; set; }
    //public string ItemIsDeleted { get; set; }
    //public string ItemIsArchived { get; set; }
    //public string IsTransfer { get; set; }
    // public string IsPurchase { get; set; }
    //public string SerialNumberTracking { get; set; }
    public string LotNumberTracking { get; set; }
    // public string DateCodeTracking { get; set; }
    // public string IsBuildBreak { get; set; }
    // public string Taxable { get; set; }
    public Int64 ItemID { get; set; }
    public Guid ItemGUID { get; set; }
    public Int64 CompanyID { get; set; }
    public Int64 RoomID { get; set; }
    public string RoomInfo { get; set; }
    public string CompanyInfo { get; set; }
    //  public string IsItemLevelMinMaxQtyRequired { get; set; }
    //   public string BarcodeImage_ItemNumber { get; set; }
    //public string BarcodeImage_BinNumber { get; set; }
    //   public int? CostDecimalPoint { get; set; }
    //public int? QuantityDecimalPoint { get; set; }


    public string ItemUDF6 { get; set; }
    public string ItemUDF7 { get; set; }
    public string ItemUDF8 { get; set; }
    public string ItemUDF9 { get; set; }
    public string ItemUDF10 { get; set; }

}


public class RPT_InventoryDailyHistory
{
    public string ItemNumber { get; set; }
    public string ManufacturerNumber { get; set; }
    public string SupplierPartNo { get; set; }
    public string UPC { get; set; }
    public string UNSPSC { get; set; }
    public string UDF1 { get; set; }
    public string UDF2 { get; set; }
    public string UDF3 { get; set; }
    public string UDF4 { get; set; }
    public string UDF5 { get; set; }
    public string SupplierName { get; set; }
    public string ManufacturerName { get; set; }
    public string CategoryName { get; set; }
    public Guid ItemGUID { get; set; }
    public Guid GUID { get; set; }
    public string UDF6 { get; set; }
    public string UDF7 { get; set; }
    public string UDF8 { get; set; }
    public string UDF9 { get; set; }
    public string UDF10 { get; set; }
    public Int64? CategoryID { get; set; }
    public Int64? SupplierID { get; set; }
    public Int64? ManufacturerID { get; set; }
}
public class RPT_SerialNumberList
{
    public Guid ItemGUID { get; set; }
    public string ItemNumber { get; set; }
    public string BinNumber { get; set; }
    public string SerialNumber { get; set; }
    public string ReceivedDate { get; set; }
    public string PULLDate { get; set; }
    public string PullOrderNumber { get; set; }
    public string OrderNumber { get; set; }
    public string RoomInfo { get; set; }
    public string CompanyInfo { get; set; }
    public string CurrentDateTime { get; set; }
    //public double? Total { get; set; }
}

public class RRT_InStockForSerialLotDateCodeDTO
{
    public string ItemNumber { get; set; }
    public string Bin { get; set; }
    public string StageBin { get; set; }
    public string StageAndBin { get; set; }
    public string SerialNumber { get; set; }
    public string LotNumber { get; set; }
    public string ExpirationDate { get; set; }
    public string Supplier { get; set; }
    public string SupplierPartNo { get; set; }
    public string ItemDescription { get; set; }

    public string LongDescription { get; set; }

    public string Category { get; set; }

    public string CategoryName { get; set; }

    public string Staging { get; set; }
    public double? CriticalQty { get; set; }
    public double? MinQty { get; set; }
    public double? MaxQty { get; set; }
    public double? ReceivedCost { get; set; }
    public double? OnHandQuantity { get; set; }
    public double? StagedQuantity { get; set; }
    public string CostUOM { get; set; }
    public double? ItemCost { get; set; }
    public double? AverageCost { get; set; }
    public double? ExtendedCost { get; set; }
    public double? StageAndGeneralQuantity { get; set; }
    public double? TotalGeneralExtendCost { get; set; }
    public string Manufacturer { get; set; }
    public string ManufacturerPartNo { get; set; }

    public string ManufacturerName { get; set; }
    public string ManufacturerNumber { get; set; }

    public string InventoryClassificationName { get; set; }
    public double? MSExtendedCost { get; set; }
    public double? BinExtendedCost { get; set; }
    public double? Total { get; set; }
    public string RoomInfo { get; set; }
    public string UPC { get; set; }
    public string UNSPSC { get; set; }
    public string ItemUDF1 { get; set; }
    public string ItemUDF2 { get; set; }
    public string ItemUDF3 { get; set; }
    public string ItemUDF4 { get; set; }
    public string ItemUDF5 { get; set; }
    public string ItemUpdatedByName { get; set; }
    public string ItemRoomName { get; set; }
    public string ItemCompanyName { get; set; }
    public Int64 ItemID { get; set; }
    public Guid ItemGUID { get; set; }
    public int CostDecimalPoint { get; set; }
    public int QuantityDecimalPoint { get; set; }
    public string ReceivedOn { get; set; }
    public double? MSBinQty { get; set; }
    public double? BinQty { get; set; }
    public Int64 RoomID { get; set; }
    public Int64 CompanyID { get; set; }

    public Int64? CategoryID { get; set; }

    public Int64? SupplierID { get; set; }

    public Int64? BinID { get; set; }

    public string ItemUDF6 { get; set; }
    public string ItemUDF7 { get; set; }
    public string ItemUDF8 { get; set; }
    public string ItemUDF9 { get; set; }
    public string ItemUDF10 { get; set; }

}

public class RPT_EnterpriseQL
{ 
    public string QLName { get; set; }
    public string EnterpriseQLItemGuids { get; set; }
}

public class SolumFlags
{
    public bool IsTransit { get; set; }
    public bool IsBackOrder { get; set; }
    public bool IsOnOrder { get; set; }
    public bool IsClosedOrder { get; set; }
    public Guid? OrderDetailsGUID { get; set; }
    public Guid? OrderGUID { get; set; }
    public double? InTransitQuantity { get; set; }
    public int OrderStatus { get; set; }
    public bool DisplayOrderException { get; set; }

}

public class UnAssignedLabelsDeletedItems
{
    public Nullable<Guid> ItemGUID { get; set; }
    public Nullable<Guid> GUID { get; set; }
    public string SupPartNo { get; set; }
    public long UserID { get; set; }
    public long EnterpriseID { get; set; }
    public long CompanyID { get; set; }
    public long RoomID { get; set; }
    public bool IsStarted { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsError { get; set; }
    public string Error { get; set; }

}