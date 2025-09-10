using System;
using System.ComponentModel.DataAnnotations;
using eTurns.DTO.Resources;

namespace eTurns.DTO
{
    [Serializable]
    public class QuoteDetailDTO: ItemViewFields
    {
        private string _createdDate;
        private string _updatedDate;
        private string _ReceivedOn;
        private string _ReceivedOnWeb;

        public long ID { get; set; }
        public Guid GUID { get; set; }
        public Guid QuoteGUID { get; set; }
        public Guid ItemGUID { get; set; }
        public long? BinID { get; set; }
        public double? RequestedQuantity { get; set; }
        public double? ApprovedQuantity { get; set; }
        public double? OrderedQuantity { get; set; }
        public double? InTransitQuantity { get; set; }
        public double? QuoteLineItemExtendedCost { get; set; }
        public double? QuoteLineItemExtendedPrice { get; set; }
        public new Nullable<System.Int64> CreatedBy { get; set; }
        public long LastUpdatedBy { get; set; }
        public long Room { get; set; }
        public long CompanyID { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
        public DateTime ReceivedOn { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        public DateTime? RequiredDate { get; set; }
        public string ASNNumber { get; set; }
        public bool? IsEDIRequired { get; set; }
        public bool? IsEDISent { get; set; }
        public DateTime? LastEDIDate { get; set; }
        public bool? IsCloseItem { get; set; }
        public string LineNumber { get; set; }
        public string ControlNumber { get; set; }
        public string Comment { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public double? RequestedQuantityUOM { get; set; }
        public double? ApprovedQuantityUOM { get; set; }
        public double? OrderedQuantityUOM { get; set; }
        public double? InTransitQuantityUOM { get; set; }
        public double? ItemCost { get; set; }
        public double? ItemCostUOM { get; set; }
        public int? ItemCostUOMValue { get; set; }
        public double? ItemMarkup { get; set; }
        public double? ItemSellPrice { get; set; }
        public string QuoteNumber { get; set; }
        public string BinNumber { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public Nullable<System.Int32> QuoteUOMValue { get; set; }
        public Boolean IsAllowQuoteCostuom { get; set; }
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
                    _updatedDate = FnCommon.ConvertDateByTimeZone(LastUpdated, true);
                }
                return _updatedDate;
            }
            set { this._updatedDate = value; }
        }
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
            set
            { this._ReceivedOnWeb = value; }
        }
        public bool IsOrdered { get; set; }
        public int TotalRecords { get; set; }
        public int? QuoteUOMValue_LineItem { get; set; }
        public bool IsAllowQuoteCostuom_LineItem { get; set; }
        [Display(Name = "RequiredDate", ResourceType = typeof(ResOrder))]
        public string RequiredDateStr { get; set; }
        public string BinName { get; set; }
        public Guid? tempDetailsGUID { get; set; }
        public string CostUOM { get; set; }
        //public int? CostUOMValue { get; set; }
        public Nullable<double> ItemLocationOH { get; set; }
        public long? BlanketPOID { get; set; }
        public string BlanketPONumber { get; set; }
        public decimal? MaxOrderSize { get; set; }
        public Nullable<int> POItemLineNumber { get; set; }
        public bool hasPOItemNumber { get; set; }
        public int QuoteStatus { get; set; }
    }

    public class QuoteLineItemDetailDTO
    {
        public Int64 ID { get; set; }
        public Guid? GUID { get; set; }


        public System.String QuoteNumber { get; set; }
        public System.String ReleaseNumber { get; set; }
        public Nullable<DateTime> RequiredDate { get; set; }
        public System.String QuoteStatus { get; set; }
        public System.String QuoteComment { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResQuoteMaster))]
        public System.String QuoteUDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResQuoteMaster))]
        public System.String QuoteUDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResQuoteMaster))]
        public System.String QuoteUDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResQuoteMaster))]
        public System.String QuoteUDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResQuoteMaster))]
        public System.String QuoteUDF5 { get; set; }

        public System.String ItemNumber { get; set; }
        public System.String Bin { get; set; }
        public Nullable<System.Double> RequestedQty { get; set; }
        public System.String ASNNumber { get; set; }
        public Nullable<System.Double> ApprovedQty { get; set; }
        public Nullable<System.Double> InTransitQty { get; set; }
        public Nullable<Boolean> IsCloseItem { get; set; }
        public System.String LineNumber { get; set; }
        public System.String ControlNumber { get; set; }
        public System.String ItemComment { get; set; }

        [Display(Name = "OrdDtlUDF1", ResourceType = typeof(ResQuoteMaster))]
        public System.String LineItemUDF1 { get; set; }

        [Display(Name = "OrdDtlUDF2", ResourceType = typeof(ResQuoteMaster))]
        public System.String LineItemUDF2 { get; set; }

        [Display(Name = "OrdDtlUDF3", ResourceType = typeof(ResQuoteMaster))]
        public System.String LineItemUDF3 { get; set; }

        [Display(Name = "OrdDtlUDF4", ResourceType = typeof(ResQuoteMaster))]
        public System.String LineItemUDF4 { get; set; }

        [Display(Name = "OrdDtlUDF5", ResourceType = typeof(ResQuoteMaster))]
        public System.String LineItemUDF5 { get; set; }

        //
        public Nullable<System.Double> RequestedQtyUOM { get; set; }
        public Nullable<System.Double> ApprovedQtyUOM { get; set; }
        public Nullable<System.Double> InTransitQtyUOM { get; set; }
        public Boolean IsAllowOrderCostuom { get; set; }
        public System.String SupplierName { get; set; }
    }
    public class ResQuoteDetail
    {
        private static string resourceFileName = "ResQuoteDetail";

        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", resourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", resourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", resourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", resourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", resourceFileName, true);
            }
        }
        public static string ConfirmOrderWillCreatedFromQuote
        {
            get
            {
                return ResourceRead.GetResourceValue("ConfirmOrderWillCreatedFromQuote", resourceFileName);
            }
        }
    }
}
