using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{

    public class CatalogReportTemplateMasterDTO
    {

        public Int64 ID { get; set; }

        public Int64 TemplateID { get; set; }

        public Int64 CompanyID { get; set; }

        public String TemplateName { get; set; }

        public String LabelSize { get; set; }

        public Int32 NoOfLabelPerSheet { get; set; }

        public Int32 NoOfColumns { get; set; }

        public Double PageWidth { get; set; }

        public Double PageHeight { get; set; }

        public Double LabelWidth { get; set; }

        public Double LabelHeight { get; set; }

        public Double PageMarginLeft { get; set; }

        public Double PageMarginRight { get; set; }

        public Double PageMarginTop { get; set; }

        public Double PageMarginBottom { get; set; }

        public Double LabelSpacingHorizontal { get; set; }

        public Double LabelSpacingVerticle { get; set; }

        public Double LabelPaddingLeft { get; set; }

        public Double LabelPaddingRight { get; set; }

        public Double LabelPaddingTop { get; set; }

        public Double LabelPaddingBottom { get; set; }

        public Int32 LabelType { get; set; }

        public string RoomName { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }

        public string TemplateNameWithSize { get; set; }

        public int TotalRecords { get; set; }
    }
    public class CatalogReportDTO
    {

    }

    public class CatalogReportFieldsDTO
    {
        public string ItemNumber { get; set; }
        public string ItemUniqueNumber { get; set; }
        public string ManufacturerNumber { get; set; }
        public string SupplierPartNo { get; set; }
        public string UPC { get; set; }
        public string UNSPSC { get; set; }
        public string ItemDescription { get; set; }
        public string LongDescription { get; set; }
        public string ImagePath { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public string Link1 { get; set; }
        public string Link2 { get; set; }
        public string ItemCreatedByName { get; set; }
        //  public string ItemUpdatedByName { get; set; }
        public string ItemRoomName { get; set; }
        public string ItemCompanyName { get; set; }
        public string ItemBlanketPO { get; set; }
        public string SupplierName { get; set; }
        public string ManufacturerName { get; set; }
        public string CategoryName { get; set; }
        public string GLAccount { get; set; }
        public string Unit { get; set; }
        public string DefaultLocationName { get; set; }
        public string InventoryClassificationName { get; set; }
        //public string CostUOM { get; set; }
        //public int? CostUOMValue { get; set; }
        public int? InventoryClassification { get; set; }
        public byte? TrendingSetting { get; set; }
        public int? LeadTimeInDays { get; set; }
        public string ItemTypeName { get; set; }
        public string ItemCost { get; set; }
        public string SellPrice { get; set; }
        //public string ExtendedCost { get; set; }
        //public string AverageCost { get; set; }
        //public string PricePerTerm { get; set; }
        //public string OnHandQuantity { get; set; }
        // public string StagedQuantity { get; set; }
        //public string ItemInTransitquantity { get; set; }
        // public string OnOrderQuantity { get; set; }
        //public string OnReturnQuantity { get; set; }
        //public string OnTransferQuantity { get; set; }
        //public string SuggestedOrderQuantity { get; set; }
        //public string SuggestedTransferQuantity { get; set; }
        // public string RequisitionedQuantity { get; set; }
        public string CriticalQuantity { get; set; }
        public string MinimumQuantity { get; set; }
        public string MaximumQuantity { get; set; }
        public string DefaultReorderQuantity { get; set; }
        public string DefaultPullQuantity { get; set; }
        //public string AverageUsage { get; set; }
        // public string Turns { get; set; }
        public string Markup { get; set; }
        public string WeightPerPiece { get; set; }
        //public string ItmCurrentDateTime { get; set; }
        public string ItemCreatedOn { get; set; }
        //public string ItemUpdatedOn { get; set; }
        public string Consignment { get; set; }
        public string IsTransfer { get; set; }
        public string IsPurchase { get; set; }
        public string SerialNumberTracking { get; set; }
        public string LotNumberTracking { get; set; }
        public string DateCodeTracking { get; set; }
        public string IsBOMItem { get; set; }
        public string IsBuildBreak { get; set; }
        public string IsItemLevelMinMaxQtyRequired { get; set; }
        public string IsEnforceDefaultReorderQuantity { get; set; }
        public string Trend { get; set; }
        public string Taxable { get; set; }
        public Int64? ItemID { get; set; }
        public Guid? ItemGUID { get; set; }
        public Int64? ItemCompanyID { get; set; }
        public Int64? ItemRoomID { get; set; }
        public Int64? ManufacturerID { get; set; }
        public Int64? SupplierID { get; set; }
        public Int64? CategoryID { get; set; }
        public Int64? GLAccountID { get; set; }
        public Int64? UOMID { get; set; }
        public Int64? CostUOMID { get; set; }
        public Int64? RefBomId { get; set; }
        public Int64? ItemCreatedBy { get; set; }
        public Int64? ItemLastUpdatedBy { get; set; }
        public Int64? DefaultLocationID { get; set; }
        public string ItemImage { get; set; }
        public string EnterpriseLogo { get; set; }
        public string CompanyLogo { get; set; }
        public string SupplierLogo { get; set; }

        public string ItemImagePath { get; set; }
        public string ItemImageType { get; set; }
        public string ItemImageExternalURL { get; set; }
        public string SupplierImagePath { get; set; }
        public string SupplierImageType { get; set; }
        public string SupplierImageExternalURL { get; set; }

        public string BinNumber { get; set; }

        public string CostUOMName { get; set; }


        public string ItemUDF6 { get; set; }
        public string ItemUDF7 { get; set; }
        public string ItemUDF8 { get; set; }
        public string ItemUDF9 { get; set; }
        public string ItemUDF10 { get; set; }

        public string BinDefaultReorderQuantity { get; set; }
        public string BinDefaultPullQuantity { get; set; }

        // public string PullQtyScanOverride { get; set; }
        // public string ItemIsDeleted { get; set; }
        // public string ItemIsArchived { get; set; }
        // public string PackingQuantity { get; set; }
        // public string ItemUniqueNumber { get; set; }
        // public string IsLotSerialExpiryCost { get; set; }
        // public string BondedInventory { get; set; }
        // public string ItemWhatWhereAction { get; set; }
        // public int? ItemType { get; set; }
        // public DateTime? ItemCreatedDate { get; set; }
        // public DateTime? ItemUpdatedDate { get; set; }
        // public string IsAutoInventoryClassification { get; set; }
        // public string RoomInfo { get; set; }
        // public string CompanyInfo { get; set; }
        // public string BarcodeImage_ItemNumber { get; set; }
        // public string BarcodeImage_DefaultLocationName { get; set; }
        // public int? CurrencyDecimalDigits { get; set; }
        // public int? NumberDecimalDigits { get; set; }
        // public string CultureCode { get; set; }
        // public DateTime? Pricesaveddate { get; set; }
        // public string ItemAddedFrom { get; set; }
        // public string ItemEditedFrom { get; set; }
        // public DateTime? ItemReceivedOnDate { get; set; }
        // public DateTime? ItemReceivedOnWebDate { get; set; }
        // public DateTime? ItemCreatedOnDate { get; set; }
        // public string ItemReceivedOn { get; set; }
        // public string ItemReceivedOnWeb { get; set; }
    }

    [Serializable]
    public class CatalogReportDetailDTO
    {
        public System.Int64 ID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Name", ResourceType = typeof(ResCommon))] 
        public System.String Name { get; set; }

        [Range(1, Int64.MaxValue, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Int64 TemplateID { get; set; }


        public System.String TemplateName { get; set; }
        [AllowHtml]
        public System.String LabelHTML { get; set; }

        [AllowHtml]
        public System.String LabelXML { get; set; }
        public System.Double FontSize { get; set; }
        public string SelectedFields { get; set; }

        [Display(Name = "IncludeBin", ResourceType = typeof(ResBarcodeMaster))]
        public Boolean IncludeBin { get; set; }
        [Display(Name = "IncludeQuantity", ResourceType = typeof(ResBarcodeMaster))]
        public Boolean IncludeQuantity { get; set; }

        [Display(Name = "QuantityField", ResourceType = typeof(ResBarcodeMaster))]
        public System.String QuantityField { get; set; }
        //[Display(Name = "TextFont", ResourceType = typeof(ResBarcodeMaster))]  
        public string TextFont { get; set; }
        public System.Int64 CompanyID { get; set; }
        public System.Int64 RoomID { get; set; }
        public System.Int64 CreatedBy { get; set; }
        public System.Int64 UpdatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime UpdatedOn { get; set; }
        public Boolean IsArchived { get; set; }
        public Boolean IsDeleted { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        [Display(Name = "BarcodeFont", ResourceType = typeof(ResBarcodeMaster))]
        public string BarcodeFont { get; set; }
        [Display(Name = "BarcodePattern", ResourceType = typeof(ResBarcodeMaster))]
        public string BarcodePattern { get; set; }

        [Display(Name = "BarcodeKey", ResourceType = typeof(ResBarcodeMaster))]
        public string BarcodeKey { get; set; }

        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }


        public string[] arrFieldIds { get; set; }

        public List<KeyValDTO> lstSelectedModuleFields { get; set; }
        public List<KeyValDTO> lstModuleFields { get; set; }


        //public List<LabelModuleFieldMasterDTO> lstBarcodeKey { get; set; }
        //public List<KeyValDTO> lstQuantityFields { get; set; }


        //public System.Int64 TemplateID { get; set; }

        //public System.Int64 ModuleID { get; set; }

        //public System.String Feilds { get; set; }





        //public System.Int64 BarcodeKey { get; set; }

        //public string ModuleName { get; set; }

        //public string TemplateName { get; set; }
        public int TotalRecords { get; set; }

    }




}
