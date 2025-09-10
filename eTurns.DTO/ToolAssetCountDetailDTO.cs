using System;

namespace eTurns.DTO
{
    public class ToolAssetCountDetailDTO
    {
        public System.Int64 ID { get; set; }
        public Guid ToolAssetCountGUID { get; set; }
        public string ToolName { get; set; }
        public Guid ToolGUID { get; set; }
        public Guid ToolBinGUID { get; set; }
        public System.Double CountQuantity { get; set; }
        public System.String CountLineItemDescription { get; set; }
        public System.String CountLineItemDescriptionEntry { get; set; }
        public System.String ItemDescription { get; set; }
        public System.String CountItemStatus { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
        public System.Int64 CreatedBy { get; set; }
        public System.Int64 LastUpdatedBy { get; set; }
        public Boolean IsDeleted { get; set; }
        public Boolean IsArchived { get; set; }
        public Guid GUID { get; set; }
        public string RoomName { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }

        public long CompanyId { get; set; }
        public long RoomId { get; set; }

        public string CountName { get; set; }

        public string CountType { get; set; }
        public Guid CountDetailGUID { get; set; }

        public DateTime CountDate { get; set; }

        public string CountStatus { get; set; }

        public bool IsApplied { get; set; }
        public DateTime? AppliedDate { get; set; }

        public bool IsClosed { get; set; }

        public long ToolBinID { get; set; }
        public string Location { get; set; }

        public Nullable<System.Double> Quantity { get; set; }

        public System.DateTime ReceivedOn { get; set; }

        public System.DateTime ReceivedOnWeb { get; set; }

        public System.String AddedFrom { get; set; }

        public System.String EditedFrom { get; set; }

        public bool IsOnlyFromItemUI { get; set; }

        public double? ToolQuantity { get; set; }
        public bool SaveAndApply { get; set; }
        public double? QuantityDifference { get; set; }
        public double TotalDifference { get; set; }
        public Int64? ToolType { get; set; }
        public long HistoryID { get; set; }
        public long ICHistoryId { get; set; }
        public bool Consignment { get; set; }
        public string LIType { get; set; }

        public Boolean SerialNumberTracking { get; set; }

        public Boolean LotNumberTracking { get; set; }
        public Boolean DateCodeTracking { get; set; }
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
        public string WhatWhereAction { get; set; }
        public System.String UDF1 { get; set; }
        public System.String UDF2 { get; set; }
        public System.String UDF3 { get; set; }
        public System.String UDF4 { get; set; }
        public System.String UDF5 { get; set; }
    }
}
