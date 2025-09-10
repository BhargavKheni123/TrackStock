using System;

namespace eTurns.DTO
{
    //Model Tool Move InOut Detail For Tool Kit
    //28-Aug-2018

    public class ToolMoveInOutDetailDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public Nullable<Guid> ToolDetailGUID { get; set; }
        public Nullable<Guid> ToolItemGUID { get; set; }
        public string MoveInOut { get; set; }
        public double Quantity { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<DateTime> ReceivedOnWeb { get; set; }
        public Nullable<DateTime> ReceivedOn { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        public string WhatWhereAction { get; set; }
        public string ReasonFromMove { get; set; }
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
        public string RoomName { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public Guid? RefMoveInOutGUID { get; set; }
        public double? MoveOutQuntity { get; set; }
        public string SerialNumber { get; set; }
    }
}
