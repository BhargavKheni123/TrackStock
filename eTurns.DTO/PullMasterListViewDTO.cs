using System;

namespace eTurns.DTO
{

    public class PullMasterListViewDTO
    {
        public long ID { get; set; }
        public short ScheduleMode { get; set; }
        public bool? Billing { get; set; }

        public bool? IsEDISent { get; set; }

        public bool Consignment { get; set; }
        public Guid GUID { get; set; }
        public long? SupplierID { get; set; }
        public string PullOrderNumber { get; set; }
        public string PullCredit { get; set; }
        public Guid? ItemGUID { get; set; }
        public double? PoolQuantity { get; set; }
        public string PoolQuantity_LabelView { get; set; }
        public double? PullCost { get; set; }
        public string PullCost_LabelView { get; set; }
        public double? PullPrice { get; set; }
        public string PullPrice_LabelView { get; set; }
        public string BinNumber { get; set; }
        public string ProjectSpendName { get; set; }
        public string ItemNumber { get; set; }
        public DateTime? Created { get; set; }
        //  public string CreatedDate { get; set; }
        public DateTime? Updated { get; set; }
        //public string UpdatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public long? LastUpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public int? ItemType { get; set; }
        public string ItemType_LabelView { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        public DateTime? ReceivedOn { get; set; }
        //   public string ReceivedOnDate { get; set; }
        public DateTime? ReceivedOnWeb { get; set; }
        //    public string ReceivedOnWebDate { get; set; }
        public double? Markup { get; set; }
        public double? SellPrice { get; set; }
        public string SellPrice_LabelView { get; set; }
        public double? ItemCost { get; set; }
        public string ItemCost_LabelView { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string CategoryName { get; set; }
        public double? DefaultPullQuantity { get; set; }
        public string DefaultPullQuantity_LabelView { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerNumber { get; set; }
        public string SupplierName { get; set; }
        public string SupplierPartNo { get; set; }
        public string GLAccount { get; set; }
        public double? OnHandQuantity { get; set; }
        public string OnHandQuantity_LabelView { get; set; }
        public double? ItemOnhandQty { get; set; }
        public string ItemOnhandQty_LabelView { get; set; }
        public double? ItemLocationOnHandQty { get; set; }
        public string ItemLocationOnHandQty_LabelView { get; set; }
        public double? CriticalQuantity { get; set; }
        public string CriticalQuantity_LabelView { get; set; }
        public double? MinimumQuantity { get; set; }
        public string MinimumQuantity_LabelView { get; set; }
        public double? MaximumQuantity { get; set; }
        public string MaximumQuantity_LabelView { get; set; }
        public bool Taxable { get; set; }
        public string Taxable_LabelView { get; set; }
        public double? AverageUsage { get; set; }
        public string AverageUsage_LabelView { get; set; }
        public double? Turns { get; set; }
        public string Turns_LabelView { get; set; }
        public double? OnOrderQuantity { get; set; }
        public string OnOrderQuantity_LabelView { get; set; }
        public double? OnTransferQuantity { get; set; }
        public string OnTransferQuantity_LabelView { get; set; }
        public double? InTransitquantity { get; set; }
        public string InTransitquantity_LabelView { get; set; }
        public string LongDescription { get; set; }
        public string RequisitionNumber { get; set; }
        public string WOName { get; set; }
        public string CostUOMName { get; set; }
        public string ItemBlanketPO { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
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
        public string RequisitionUDF1 { get; set; }
        public string RequisitionUDF2 { get; set; }
        public string RequisitionUDF3 { get; set; }
        public string RequisitionUDF4 { get; set; }
        public string RequisitionUDF5 { get; set; }
        public string WorkOrderUDF1 { get; set; }
        public string WorkOrderUDF2 { get; set; }
        public string WorkOrderUDF3 { get; set; }
        public string WorkOrderUDF4 { get; set; }
        public string WorkOrderUDF5 { get; set; }
        public int? TotalCount { get; set; }
        public bool? IsItemLevelMinMaxQtyRequired { get; set; }

        public Nullable<System.Double> ConsignedQuantity { get; set; }
        public Guid? SupplierAccountGuid { get; set; }
        public string SupplierAccountNo { get; set; }

        private string _createdDate;
        private string _updatedDate;
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
        public string ReceivedOnWebDate
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

        public Guid? requisitiondetailguid { get; set; }

        public Guid? countlineitemguid { get; set; }
        public string ActionType { get; set; }
        public bool SerialNumberTracking { get; set; }
        public bool LotNumberTracking { get; set; }
        public bool DateCodeTracking { get; set; }
        public Nullable<System.Double> creditcustomerownedquantity { get; set; }
        public Nullable<System.Double> creditconsignedquantity { get; set; }

        public double? PullItemCost { get; set; }

        public double? PullMasterItemCost { get; set; }
        public double? PullMasterItemSellPrice { get; set; }
        public double? PullMasterItemAverageCost { get; set; }
        public double? PullMasterItemMarkup { get; set; }
        public int? PullMasterItemCostUOMValue { get; set; }
        
        public string ImagePath { get; set; }
        public string ImageType { get; set; }
        public string ItemImageExternalURL { get; set; }
        public long? ItemID { get; set; }

        public bool IsCustomerEDISent { get; set; }
        public string IsCustomerEDISent_LabelView { get; set; }
    }

}


