using System;

namespace eTurns.DTO
{
    public class DashboardItem
    {
        public System.Int64 ID { get; set; }
        public System.String ItemNumber { get; set; }
        public System.String Description { get; set; }
        public Nullable<System.Double> OnHandQuantity { get; set; }
        public System.Double CriticalQuantity { get; set; }
        public System.Double MinimumQuantity { get; set; }
        public System.Double MaximumQuantity { get; set; }
        public Nullable<System.Double> SuggestedOrderQuantity { get; set; }
        public Nullable<System.Double> OnOrderQuantity { get; set; }
        public Nullable<System.Double> OnTransferQuantity { get; set; }
        public Nullable<System.Double> AverageUsage { get; set; }
        public Nullable<System.Double> Turns { get; set; }
        public Nullable<System.Int32> InventoryClassification { get; set; }
        public string InventoryClassificationName { get; set; }
        public Nullable<System.Double> Cost { get; set; }
        public Nullable<System.Int64> StockOutCount { get; set; }
        public System.Int64? SupplierID { get; set; }
        public System.String SupplierPartNo { get; set; }
        public string SupplierName { get; set; }
        public Nullable<System.Double> RequisitionedQuantity { get; set; }
        public System.String LongDescription { get; set; }
        public Guid GUID { get; set; }
        public Nullable<System.Double> DefaultReorderQuantity { get; set; }
        public Nullable<System.Double> RationalFactor { get; set; }
        public long? MinCount { get; set; }
        public long? MaxCount { get; set; }
        public long? CritCount { get; set; }
        public long? SlowMovingCount { get; set; }
        public long? FastMovingCount { get; set; }
        public double SlowMovingValue { get; set; }
        public double FastMovingValue { get; set; }
        public System.Int64? CategoryID { get; set; }
        public string CategoryName { get; set; }

        public System.Int64? ManufacturerID { get; set; }
        public string ManufacturerNumber { get; set; }
        public string UPC { get; set; }
        public string UNSPSC { get; set; }
        public System.Int64? GLAccountID { get; set; }
        public System.Int64? UOMID { get; set; }
        public System.Double? PricePerTerm { get; set; }
        public System.Double? DefaultPullQuantity { get; set; }
        public System.Double? Markup { get; set; }
        public System.Double? SellPrice { get; set; }
        public System.Double? ExtendedCost { get; set; }
        public System.Int32? LeadTimeInDays { get; set; }
        public string Link1 { get; set; }
        public string Link2 { get; set; }
        public System.Boolean? Trend { get; set; }
        public System.Boolean? Taxable { get; set; }
        public System.Boolean? Consignment { get; set; }
        public System.Double? StagedQuantity { get; set; }
        public System.Double? InTransitquantity { get; set; }
        public System.Double? OnOrderInTransitQuantity { get; set; }
        public System.Double? WeightPerPiece { get; set; }
        public string ItemUniqueNumber { get; set; }
        public System.Boolean? IsTransfer { get; set; }
        public System.Boolean? IsPurchase { get; set; }
        public System.Int64? DefaultLocation { get; set; }
        public System.Boolean? SerialNumberTracking { get; set; }
        public System.Boolean? LotNumberTracking { get; set; }
        public System.Boolean? DateCodeTracking { get; set; }
        public System.Int32? ItemType { get; set; }
        public string ImagePath { get; set; }
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
        public System.DateTime? Created { get; set; }
        public System.DateTime? Updated { get; set; }
        public System.Int64? CreatedBy { get; set; }
        public System.Int64? LastUpdatedBy { get; set; }
        public System.Boolean? IsDeleted { get; set; }
        public System.Boolean? IsArchived { get; set; }
        public System.Int64? CompanyID { get; set; }
        public System.Int64? Room { get; set; }
        public string IsLotSerialExpiryCost { get; set; }
        public System.Double? PackingQuantity { get; set; }
        public System.Boolean? IsItemLevelMinMaxQtyRequired { get; set; }
        public System.Boolean? IsEnforceDefaultReorderQuantity { get; set; }
        public System.Double? AverageCost { get; set; }

        public System.Double? PerItemCost { get; set; }

        public System.Boolean? IsBuildBreak { get; set; }
        public string BondedInventory { get; set; }
        public System.Int64? CostUOMID { get; set; }
        public string WhatWhereAction { get; set; }
        public System.Double? OnReturnQuantity { get; set; }
        public System.Boolean? IsBOMItem { get; set; }
        public System.Int64? RefBomId { get; set; }
        public System.Byte? TrendingSetting { get; set; }
        public System.Boolean? PullQtyScanOverride { get; set; }
        public System.Boolean? IsAutoInventoryClassification { get; set; }
        public System.Boolean? IsPackslipMandatoryAtReceive { get; set; }
        public System.Double? SuggestedTransferQuantity { get; set; }
        public System.Double? LastCost { get; set; }
        public System.DateTime? ReceivedOn { get; set; }
        public System.DateTime? ReceivedOnWeb { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        public System.DateTime? ordereddate { get; set; }
        public System.DateTime? pulleddate { get; set; }
        public System.DateTime? counteddate { get; set; }
        public System.DateTime? trasnfereddate { get; set; }
        public System.DateTime? Pricesaveddate { get; set; }
        public string ItemImageExternalURL { get; set; }
        public string ItemDocExternalURL { get; set; }
        public string ImageType { get; set; }
        public System.Double? QtyToMeetDemand { get; set; }
        public System.Double? OutTransferQuantity { get; set; }
        public string ItemLink2ExternalURL { get; set; }
        public string ItemLink2ImageType { get; set; }
        public System.Boolean? IsActive { get; set; }
        public string ManufacturerName { get; set; }
        public string DefaultLocationName { get; set; }
        public System.Guid? DefaultLocationGUID { get; set; }
        public System.Boolean? BinDeleted { get; set; }
        public string CostUOMName { get; set; }
        public string BPONumber { get; set; }
        //public string CreatedStr { get; set; }
        //public string UpdatedStr { get; set; }
        //public string ReceivedOnStr { get; set; }
        //public string ReceivedOnWebStr { get; set; }
        //public string OrderedDateStr { get; set; }
        //public string PulledDateStr { get; set; }
        //public string CountedDateStr { get; set; }
        //public string TrasnferedDateStr { get; set; }
        //public string PricesavedDateStr { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public string ItemTypeName { get; set; }

        private string _CreatedStr;
        public string CreatedStr
        {
            get
            {
                if (string.IsNullOrEmpty(_CreatedStr))
                {
                    _CreatedStr = FnCommon.ConvertDateByTimeZone(Created, true);
                }
                return _CreatedStr;
            }
            set { this._CreatedStr = value; }
        }

        private string _UpdatedStr;
        public string UpdatedStr
        {
            get
            {
                if (string.IsNullOrEmpty(_UpdatedStr))
                {
                    _UpdatedStr = FnCommon.ConvertDateByTimeZone(Updated, true);
                }
                return _UpdatedStr;
            }
            set { this._UpdatedStr = value; }
        }

        private string _ReceivedOnStr;
        public string ReceivedOnStr
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOnStr))
                {
                    _ReceivedOnStr = FnCommon.ConvertDateByTimeZone(ReceivedOn, true);
                }
                return _ReceivedOnStr;
            }
            set { this._ReceivedOnStr = value; }
        }

        private string _ReceivedOnWebStr;
        public string ReceivedOnWebStr
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOnWebStr))
                {
                    _ReceivedOnWebStr = FnCommon.ConvertDateByTimeZone(ReceivedOnWeb, true);
                }
                return _ReceivedOnWebStr;
            }
            set { this._ReceivedOnWebStr = value; }
        }

        private string _OrderedDateStr;
        public string OrderedDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_OrderedDateStr))
                {
                    _OrderedDateStr = FnCommon.ConvertDateByTimeZone(ordereddate, true);
                }
                return _OrderedDateStr;
            }
            set { this._OrderedDateStr = value; }
        }

        private string _PulledDateStr;
        public string PulledDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_PulledDateStr))
                {
                    _PulledDateStr = FnCommon.ConvertDateByTimeZone(pulleddate, true);
                }
                return _PulledDateStr;
            }
            set { this._PulledDateStr = value; }
        }

        private string _CountedDateStr;
        public string CountedDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_CountedDateStr))
                {
                    _CountedDateStr = FnCommon.ConvertDateByTimeZone(counteddate, true);
                }
                return _CountedDateStr;
            }
            set { this._CountedDateStr = value; }
        }

        private string _TrasnferedDateStr;
        public string TrasnferedDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_TrasnferedDateStr))
                {
                    _TrasnferedDateStr = FnCommon.ConvertDateByTimeZone(trasnfereddate, true);
                }
                return _TrasnferedDateStr;
            }
            set { this._TrasnferedDateStr = value; }
        }

        private string _PricesavedDateStr;
        public string PricesavedDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_PricesavedDateStr))
                {
                    _PricesavedDateStr = FnCommon.ConvertDateByTimeZone(Pricesaveddate, true);
                }
                return _PricesavedDateStr;
            }
            set { this._PricesavedDateStr = value; }
        }

        public string ItemWithBin { get; set; }

        public string BinNumber { get; set; }

        public int TotalRecords { get; set; }
    }
}
