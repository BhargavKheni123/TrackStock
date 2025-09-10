using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class BOMItemDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //ItemNumber
        [Display(Name = "ItemNumber", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ItemNumber { get; set; }

        //ManufacturerID
        [Display(Name = "ManufacturerID", ResourceType = typeof(ResBomItemMaster))]
        public Nullable<System.Int64> ManufacturerID { get; set; }

        //ManufacturerNumber
        [Display(Name = "ManufacturerNumber", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ManufacturerNumber { get; set; }

        //SupplierID
        [Display(Name = "SupplierID", ResourceType = typeof(ResBomItemMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64? SupplierID { get; set; }

        //SupplierPartNo
        [Display(Name = "SupplierPartNo", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String SupplierPartNo { get; set; }

        //UPC
        [Display(Name = "UPC", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UPC { get; set; }

        //UNSPSC
        [Display(Name = "UNSPSC", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String UNSPSC { get; set; }

        //Description
        [Display(Name = "Description", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String Description { get; set; }

        //LongDescription
        [Display(Name = "LongDescription", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(2000, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String LongDescription { get; set; }

        [Display(Name = "EnrichedProductData", ResourceType = typeof(ResItemMaster))]
        [StringLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string EnrichedProductData { get; set; }

        [Display(Name = "EnhancedDescription", ResourceType = typeof(ResItemMaster))]
        [StringLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string EnhancedDescription { get; set; }

        //CategoryID
        [Display(Name = "CategoryID", ResourceType = typeof(ResBomItemMaster))]
        public Nullable<System.Int64> CategoryID { get; set; }



        //GLAccountID
        [Display(Name = "GLAccountID", ResourceType = typeof(ResBomItemMaster))]
        public Nullable<System.Int64> GLAccountID { get; set; }

        //UOMID
        [Display(Name = "UOMID", ResourceType = typeof(ResBomItemMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 UOMID { get; set; }


        //PricePerTerm
        [Display(Name = "PricePerTerm", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> PricePerTerm { get; set; }

        //DefaultReorderQuantity
        [Display(Name = "DefaultReorderQuantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[DefaultReorderQuantityCheck("MaximumQuantity", ErrorMessage = "Default Reorder quantity must be less then Maximum quantity")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> DefaultReorderQuantity { get; set; }

        //DefaultPullQuantity
        [Display(Name = "DefaultPullQuantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> DefaultPullQuantity { get; set; }

        ////DefaultCartQuantity
        //[Display(Name = "DefaultCartQuantity", ResourceType = typeof(ResBomItemMaster))]
        //[RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //public Nullable<System.Double> DefaultCartQuantity { get; set; }

        //Cost
        [Display(Name = "Cost", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Cost { get; set; }

        //Markup
        [Display(Name = "Markup", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[Range(0, 100, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Markup { get; set; }

        //SellPrice
        [Display(Name = "SellPrice", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]

        public Nullable<System.Double> SellPrice { get; set; }

        //ExtendedCost
        [Display(Name = "ExtendedCost", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ExtendedCost { get; set; }

        //LeadTimeInDays
        [Display(Name = "LeadTimeInDays", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int32> LeadTimeInDays { get; set; }

        //Link1
        [Display(Name = "Link1", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Link1 { get; set; }

        //Link2
        [Display(Name = "Link2", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Link2 { get; set; }

        //Trend
        [Display(Name = "Trend", ResourceType = typeof(ResBomItemMaster))]
        public Boolean Trend { get; set; }

        //Taxable
        [Display(Name = "Taxable", ResourceType = typeof(ResBomItemMaster))]
        public Boolean Taxable { get; set; }

        //Consignment
        [Display(Name = "Consignment", ResourceType = typeof(ResBomItemMaster))]
        public Boolean Consignment { get; set; }

        //StagedQuantity
        [Display(Name = "StagedQuantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> StagedQuantity { get; set; }

        //InTransitquantity
        [Display(Name = "InTransitquantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> InTransitquantity { get; set; }

        //OnOrderQuantity
        [Display(Name = "OnOrderQuantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> OnOrderQuantity { get; set; }



        //OnTransferQuantity
        [Display(Name = "OnTransferQuantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> OnTransferQuantity { get; set; }

        //SuggestedOrderQuantity
        [Display(Name = "SuggestedOrderQuantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> SuggestedOrderQuantity { get; set; }

        //RequisitionedQuantity
        [Display(Name = "RequisitionedQuantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> RequisitionedQuantity { get; set; }

        ////PackingQuantity
        //[Display(Name = "PackingQuantity", ResourceType = typeof(ResBomItemMaster))]
        //[RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //public Nullable<System.Double> PackingQuantity { get; set; }


        //AverageUsage
        [Display(Name = "AverageUsage", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> AverageUsage { get; set; }

        //Turns
        [Display(Name = "Turns", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Turns { get; set; }

        //OnHandQuantity        
        [Display(Name = "OnHandQuantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> OnHandQuantity { get; set; }

        //CriticalQuantity
        [Display(Name = "CriticalQuantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [CriticleQuantityCheck("MinimumQuantity", ErrorMessage = "Critical quantity must be less then Minimum quantity")]
        [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
        public System.Double CriticalQuantity { get; set; }

        //MinimumQuantity
        [Display(Name = "MinimumQuantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [MinimumQuantityCheck("MaximumQuantity", ErrorMessage = "Minimum quantity must be less then Maximum quantity")]
        [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
        public System.Double MinimumQuantity { get; set; }

        //MaximumQuantity
        [Display(Name = "MaximumQuantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[RequiredIf("IsItemLevelMinMaxQtyRequired", true, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0.0, 1000000000.0, ErrorMessageResourceName = "ItemCriticalMinimumMaximumQtyMaxLimit", ErrorMessageResourceType = typeof(ResItemMaster))]
        public System.Double MaximumQuantity { get; set; }

        //WeightPerPiece
        [Display(Name = "WeightPerPiece", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> WeightPerPiece { get; set; }

        //ItemUniqueNumber
        [Display(Name = "ItemUniqueNumber", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ItemUniqueNumber { get; set; }

        //TransferOrPurchase
        [Display(Name = "IsTransfer", ResourceType = typeof(ResBomItemMaster))]
        public Boolean IsTransfer { get; set; }

        //IsPurchase
        [Display(Name = "IsPurchase", ResourceType = typeof(ResBomItemMaster))]
        public Boolean IsPurchase { get; set; }

        //DefaultLocation
        [Display(Name = "DefaultLocation", ResourceType = typeof(ResBomItemMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public long DefaultLocation { get; set; }


        //DefaultLocation
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "DefaultLocation", ResourceType = typeof(ResBomItemMaster))]
        public string DefaultLocationName { get; set; }


        //InventoryClassification
        [Display(Name = "InventoryClassification", ResourceType = typeof(ResBomItemMaster))]
        public Nullable<System.Int32> InventoryClassification { get; set; }


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
        [Display(Name = "ItemType", ResourceType = typeof(ResBomItemMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int32 ItemType { get; set; }

        //ImagePath
        [Display(Name = "ImagePath", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ImagePath { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        //UDF6
        [Display(Name = "UDF6", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF6 { get; set; }

        //UDF7
        [Display(Name = "UDF7", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF7 { get; set; }

        //UDF8
        [Display(Name = "UDF8", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF8 { get; set; }

        //UDF9
        [Display(Name = "UDF9", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF9 { get; set; }

        //UDF10
        [Display(Name = "UDF10", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
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

        //Room
        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //IsLotSerialExpiryCost
        [Display(Name = "IsLotSerialExpiryCost", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String IsLotSerialExpiryCost { get; set; }

        //PackingQuantity
        [Display(Name = "PackingQuantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> PackingQuantity { get; set; }

        [Display(Name = "IsItemLevelMinMaxQtyRequired", ResourceType = typeof(ResBomItemMaster))]
        public bool? IsItemLevelMinMaxQtyRequired { get; set; }

        //[Display(Name="Enforce Default Reorder Quantity")]
        [Display(Name = "IsEnforceDefaultReorderQuantity", ResourceType = typeof(ResBomItemMaster))]
        public bool? IsEnforceDefaultReorderQuantity { get; set; }
        //public bool? PullQtyScanOverride { get; set; }
        public string ItemImageExternalURL { get; set; }
        public string ItemLink2ExternalURL { get; set; }
        public bool IsActive { get; set; }

        //IsBuildBreak
        [Display(Name = "IsBuildBreak", ResourceType = typeof(ResBomItemMaster))]
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

        [Display(Name = "UOMID", ResourceType = typeof(ResBomItemMaster))]
        [Required]
        public string Unit { get; set; }

        public string GLAccount { get; set; }
        public string InventryLocation { get; set; }

        public string AppendedBarcodeString { get; set; }






        public string QuickListName { get; set; }
        public string QuickListGUID { get; set; }
        public double QuickListItemQTY { get; set; }

        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Double> ConsignedQuantity { get; set; }


        //[RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "AverageCost", ResourceType = typeof(ResBomItemMaster))]
        public Nullable<System.Double> AverageCost { get; set; }

        public string BinNumber { get; set; }

        public long? BinID { get; set; }

        public double? CountCustomerOwnedQuantity { get; set; }
        public double? CountConsignedQuantity { get; set; }

        public string CountLineItemDescriptionEntry { get; set; }






        public Nullable<System.Int32> StockOutCount { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "CostUOMID", ResourceType = typeof(ResBomItemMaster))]
        public Nullable<System.Int64> CostUOMID { get; set; }

        public int MonthValue { get; set; }

        public System.String WhatWhereAction { get; set; }
        //OnOrderQuantity
        [Display(Name = "OnReturnQuantity", ResourceType = typeof(ResBomItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> OnReturnQuantity { get; set; }
        public bool IsBOMItem { get; set; }
        public long? RefBomI { get; set; }

        public System.String CostUOMName { get; set; }
        public System.String Status { get; set; }
        public System.String Reason { get; set; }


        public string ActionButton { get; set; }


        public double? PackingQantity { get; set; }

        public string ManufacturerPartNumber { get; set; }

        public string SupplierPartNumber { get; set; }

        public string SourcePageName { get; set; }

        public double Quantity { get; set; }
        public string inputQuantity { get; set; }
        public string DestinationModule { get; set; }
        public bool OpenPopup { get; set; }
        public string ButtonText { get; set; }
        public Guid OrderGUID { get; set; }
        public string OrderSupplier { get; set; }

        public long RoomId { get; set; }

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

        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }

        [Display(Name = "PerItemCost", ResourceType = typeof(ResBomItemMaster))]
        public Nullable<System.Double> PerItemCost { get; set; }

        [Display(Name = "PullQtyScanOverride", ResourceType = typeof(ResBomItemMaster))]
        public bool PullQtyScanOverride { get; set; }

        public string ImageType { get; set; }
        public string ItemLink2ImageType { get; set; }

        [Display(Name = "ItemDocExternalURL", ResourceType = typeof(ResBomItemMaster))]
        [StringLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ItemDocExternalURL { get; set; }

        [Display(Name = "HistoryDate", ResourceType = typeof(Resources.ResCommon))]
        public DateTime? HistoryOn { get; set; }



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

    }
    public class ResBomItemMaster
    {
        private static string ResourceFileName = "ResBomItemMaster";

        public static string PerItemCost
        {
            get
            {
                return ResourceRead.GetResourceValue("PerItemCost", ResourceFileName);
            }
        }
        public static string ItemDocExternalURL
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemDocExternalURL", ResourceFileName);
            }
        }

        public static string PullQtyScanOverride
        {
            get
            {
                return ResourceRead.GetResourceValue("PullQtyScanOverride", ResourceFileName);
            }
        }

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
        ///   Looks up a localized string similar to ItemMaster {0} already exist! Try with Another!.
        /// </summary>
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
        ///   Looks up a localized string similar to Unit.
        /// </summary>
        public static string EnrichedProductData
        {
            get
            {
                return ResourceRead.GetResourceValue("EnrichedProductData", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Unit.
        /// </summary>
        public static string EnhancedDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("EnhancedDescription", ResourceFileName);
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

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", ResourceFileName);
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

        public static string ManageMinMaxCriticalQuantityItemLevel
        {
            get
            {
                return ResourceRead.GetResourceValue("ManageMinMaxCriticalQuantityItemLevel", ResourceFileName);
            }
        }

        public static string ConsignValidate
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignValidate", ResourceFileName);
            }
        }
    }

}


