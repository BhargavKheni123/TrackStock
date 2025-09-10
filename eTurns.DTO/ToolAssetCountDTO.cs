using System;
using System.Collections.Generic;

namespace eTurns.DTO
{
    public class ToolAssetCountDTO
    {
        public System.Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public System.String CountName { get; set; }
        public System.String CountItemDescription { get; set; }
        public System.String CountType { get; set; }
        public System.String CountStatus { get; set; }
        public System.String UDF1 { get; set; }
        public System.String UDF2 { get; set; }
        public System.String UDF3 { get; set; }
        public System.String UDF4 { get; set; }
        public System.String UDF5 { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
        public System.Int64 CreatedBy { get; set; }
        public System.Int64 LastUpdatedBy { get; set; }
        public Boolean IsDeleted { get; set; }
        public Boolean IsArchived { get; set; }
        public Int16 Year { get; set; }
        public System.Int64 CompanyId { get; set; }
        public System.Int64 RoomId { get; set; }
        public System.DateTime CountDate { get; set; }
        public string CountDateDisplay { get; set; }
        public Nullable<System.DateTime> CountCompletionDate { get; set; }
        public Nullable<Boolean> IsAutomatedCompletion { get; set; }
        public Nullable<Guid> CompleteCauseCountGUID { get; set; }
        public System.DateTime ReceivedOn { get; set; }
        public System.DateTime ReceivedOnWeb { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public bool IsOnlyFromItemUI { get; set; }
        public string RoomName { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public int TotalToolsWithinCount { get; set; }
        public bool IsClosed { get; set; }
        public bool IsApplied { get; set; }
        public long HistoryID { get; set; }
        public Int32 inventorycount { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public List<ToolAssetCountDetailDTO> CountLineItemsList { get; set; }


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
        public bool IsApplyAllDisable { get; set; }
        public Guid? ProjectSpendGUID { get; set; }
        public string ProjectSpendName { get; set; }
        public int? TotalRecords { get; set; }
    }
}
