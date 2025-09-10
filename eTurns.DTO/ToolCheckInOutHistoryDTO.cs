using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class ToolCheckInOutHistoryDTO
    {
        public System.Int64 ID { get; set; }

        [Display(Name = "ToolID", ResourceType = typeof(ResToolCheckInOutHistory))]
        public Nullable<System.Guid> ToolGUID { get; set; }

        [Display(Name = "CheckOutStatus", ResourceType = typeof(ResToolCheckInOutHistory))]
        public System.String CheckOutStatus { get; set; }

        [Display(Name = "CheckedOutQTY", ResourceType = typeof(ResToolCheckInOutHistory))]
        public Nullable<System.Double> CheckedOutQTY { get; set; }

        [Display(Name = "CheckedOutMQTY", ResourceType = typeof(ResToolCheckInOutHistory))]
        public Nullable<System.Double> CheckedOutMQTY { get; set; }

        [Display(Name = "CheckOutDate", ResourceType = typeof(ResToolCheckInOutHistory))]
        public Nullable<System.DateTime> CheckOutDate { get; set; }

        [Display(Name = "CheckInDate", ResourceType = typeof(ResToolCheckInOutHistory))]
        public Nullable<System.DateTime> CheckInDate { get; set; }

        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        [Display(Name = "Updated", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        public Nullable<Boolean> IsArchived { get; set; }

        public Nullable<Boolean> IsDeleted { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResToolCheckInOutHistory))]
        public System.String UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResToolCheckInOutHistory))]
        public System.String UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResToolCheckInOutHistory))]
        public System.String UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResToolCheckInOutHistory))]
        public System.String UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResToolCheckInOutHistory))]
        public System.String UDF5 { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "CheckedOutQTYCurrent", ResourceType = typeof(ResToolCheckInOutHistory))]
        public Nullable<System.Double> CheckedOutQTYCurrent { get; set; }

        [Display(Name = "CheckedOutMQTYCurrent", ResourceType = typeof(ResToolCheckInOutHistory))]
        public Nullable<System.Double> CheckedOutMQTYCurrent { get; set; }

        public Guid GUID { get; set; }

        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        public System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        public System.String EditedFrom { get; set; }

        public bool IsOnlyFromItemUI { get; set; }

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

        private string _checkOutedDate;
        public string CheckOutedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_checkOutedDate))
                {
                    _checkOutedDate = FnCommon.ConvertDateByTimeZone(CheckOutDate, true);
                }
                return _checkOutedDate;
            }
            set { this._checkOutedDate = value; }
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
        public Nullable<global::System.Int32> IsGroupOfItems { get; set; }
        public Nullable<Guid> TechnicianGuid { get; set; }
        public string TechnicianCode { get; set; }
        public string Technician { get; set; }

        public Nullable<Guid> RequisitionDetailGuid { get; set; }

        public string RequisitionNumber { get; set; }


        public Nullable<Guid> WorkOrderGuid { get; set; }
        public string WorkOrderNumber { get; set; }

        public Nullable<System.Guid> ToolDetailGUID { get; set; }
        public string SerialNumber { get; set; }

        public bool SerialNumberTracking { get; set; }

        public long? ToolBinID { get; set; }

        [Display(Name = "Location", ResourceType = typeof(ResToolMaster))]
        public string Location { get; set; }
        public int TotalRecords { get; set; }
    }
    [Serializable]
    public class ToolCheckoutAll
    {
        public string ActionType { get; set; }
        public Int32 Quantity { get; set; }
        public bool IsForMaintance { get; set; }
        public Guid ToolGUID { get; set; }
        public Double AQty { get; set; }
        public Double CQty { get; set; }
        public Double CMQty { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string CheckInCheckOutGUID { get; set; }
        public bool IsOnlyFromUI { get; set; }
        public Guid? TechnicianGuid { get; set; }
        public string TechnicianName { get; set; }
        public string ToolName { get; set; }
    }
    public class ResToolCheckInOutHistory
    {
        private static string ResourceFileName = "ResToolCheckInOutHistory";

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
        ///   Looks up a localized string similar to ToolCheckInOutHistory {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to ToolCheckInOutHistory.
        /// </summary>
        public static string ToolCheckInOutHistoryHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCheckInOutHistoryHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ToolCheckInOutHistory.
        /// </summary>
        public static string ToolCheckInOutHistory
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCheckInOutHistory", ResourceFileName);
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
        ///   Looks up a localized string similar to ToolID.
        /// </summary>
        public static string ToolID
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CheckOutStatus.
        /// </summary>
        public static string CheckOutStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckOutStatus", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CheckedOutQTY.
        /// </summary>
        public static string CheckedOutQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedOutQTY", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CheckedOutMQTY.
        /// </summary>
        public static string CheckedOutMQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedOutMQTY", ResourceFileName);
            }
        }

        public static string CurrentAvalQty
        {
            get
            {
                return ResourceRead.GetResourceValue("CurrentAvalQty", ResourceFileName);
            }
        }

        public static string RemainingQty
        {
            get
            {
                return ResourceRead.GetResourceValue("RemainingQty", ResourceFileName);
            }
        }
        public static string CheckoutFromKit
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckoutFromKit", ResourceFileName);
            }
        }
        public static string SerialNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialNumber", ResourceFileName);
            }
        }

        public static string AvailableQty
        {
            get
            {
                return ResourceRead.GetResourceValue("AvailableQty", ResourceFileName);
            }
        }

        public static string CheckedOutQTYCurrent
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedOutQTYCurrent", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CheckedOutMQTY.
        /// </summary>
        public static string CheckedOutMQTYCurrent
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedOutMQTYCurrent", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CheckOutDate.
        /// </summary>
        public static string CheckOutDate
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckOutDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CheckInDate.
        /// </summary>
        public static string CheckInDate
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckInDate", ResourceFileName);
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


        ///   Looks up a localized string similar to eTurns: Job Types.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }

        /// <summary>
        ///   string to display check in quantity
        /// </summary>
        public static string CheckinQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckinQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   string to display check Out quantity
        /// </summary>
        public static string CheckOutQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckOutQuantity", ResourceFileName);
            }
        }
        public static string CheckOutUDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckOutUDF1", ResourceFileName);
            }
        }
        public static string CheckOutUDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckOutUDF2", ResourceFileName);
            }
        }
        public static string CheckOutUDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckOutUDF3", ResourceFileName);
            }
        }
        public static string CheckOutUDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckOutUDF4", ResourceFileName);
            }
        }
        public static string CheckOutUDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckOutUDF5", ResourceFileName);
            }

        }

        public static string Quantity
        {
            get
            {
                return ResourceRead.GetResourceValue("Quantity", ResourceFileName);
            }

        }

        public static string Operation
        {
            get
            {
                return ResourceRead.GetResourceValue("Operation", ResourceFileName);
            }

        }

        public static string MsgPossibleValuesForOperation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPossibleValuesForOperation", ResourceFileName);
            }

        }

        public static string MsgRemoveInvalidValueFromTechnician
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRemoveInvalidValueFromTechnician", ResourceFileName);
            }

        }

        public static string NotEnoughQtyToCheckOutOnLocation { get { return ResourceRead.GetResourceValue("NotEnoughQtyToCheckOutOnLocation", ResourceFileName); } }
        public static string MsgCheckOutRecordNotFound { get { return ResourceRead.GetResourceValue("MsgCheckOutRecordNotFound", ResourceFileName); } }
        public static string MsgCheckoutRecordsNotFoundforSerial { get { return ResourceRead.GetResourceValue("MsgCheckoutRecordsNotFoundforSerial", ResourceFileName); } }
        public static string MsgInvalidUserForCheckinOut { get { return ResourceRead.GetResourceValue("MsgInvalidUserForCheckinOut", ResourceFileName); } }
        public static string MsgUserDoesnotrightforCheckinOut { get { return ResourceRead.GetResourceValue("MsgUserDoesnotrightforCheckinOut", ResourceFileName); } }
        public static string MsgUserNeedacceptLicenceAgreement { get { return ResourceRead.GetResourceValue("MsgUserNeedacceptLicenceAgreement", ResourceFileName); } }
        public static string MsgUserDoesnotrightforCheckinOutForRoom { get { return ResourceRead.GetResourceValue("MsgUserDoesnotrightforCheckinOutForRoom", ResourceFileName); } }
        public static string MsgSerialRequired { get { return ResourceRead.GetResourceValue("MsgSerialRequired", ResourceFileName); } }
        public static string MsgTechnicianRequired { get { return ResourceRead.GetResourceValue("MsgTechnicianRequired", ResourceFileName); } }
        public static string MsgCheckinCheckOutQtyReq { get { return ResourceRead.GetResourceValue("MsgCheckinCheckOutQtyReq", ResourceFileName); } }
        public static string ReqCheckinCheckOut { get { return ResourceRead.GetResourceValue("ReqCheckinCheckOut", ResourceFileName); } }
        public static string MsgCheckoutCheckinShouldbeIn { get { return ResourceRead.GetResourceValue("MsgCheckoutCheckinShouldbeIn", ResourceFileName); } }
        public static string ReqToolCheckOutQty { get { return ResourceRead.GetResourceValue("ReqToolCheckOutQty", ResourceFileName); } }
        public static string ReqToolCheckINQty { get { return ResourceRead.GetResourceValue("ReqToolCheckINQty", ResourceFileName); } }
        public static string MsgEnterSerialNumber { get { return ResourceRead.GetResourceValue("MsgEnterSerialNumber", ResourceFileName); } }
        public static string MsgSomeErrorinCheckOutDetails { get { return ResourceRead.GetResourceValue("MsgSomeErrorinCheckOutDetails", ResourceFileName); } }

        public static string ToolNotFound { get { return ResourceRead.GetResourceValue("ToolNotFound", ResourceFileName); } }
        public static string ForGeneralToolCallToolCheckInCheckOutMethod { get { return ResourceRead.GetResourceValue("ForGeneralToolCallToolCheckInCheckOutMethod", ResourceFileName); } }
        public static string EnterSerialNoForSerialTrackingTool { get { return ResourceRead.GetResourceValue("EnterSerialNoForSerialTrackingTool", ResourceFileName); } }
        public static string EnterProperSerialNoAndQtyForCheckInOut { get { return ResourceRead.GetResourceValue("EnterProperSerialNoAndQtyForCheckInOut", ResourceFileName); } }
        public static string CurrentSerialNotExistForCheckout { get { return ResourceRead.GetResourceValue("CurrentSerialNotExistForCheckout", ResourceFileName); } }
        public static string CheckOutQtyShouldNotMoreThanOne { get { return ResourceRead.GetResourceValue("CheckOutQtyShouldNotMoreThanOne", ResourceFileName); } }
        public static string ErrorInSerialLineItems { get { return ResourceRead.GetResourceValue("ErrorInSerialLineItems", ResourceFileName); } }
        public static string CurrentSerialNotExistForCheckIn { get { return ResourceRead.GetResourceValue("CurrentSerialNotExistForCheckIn", ResourceFileName); } }
        public static string CheckInQtyShouldNotMoreThanOne { get { return ResourceRead.GetResourceValue("CheckInQtyShouldNotMoreThanOne", ResourceFileName); } }

    }

}


