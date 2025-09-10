using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{

    public class ReceiveOderLineItemsDTO : OrderDetailsDTO
    {
        public Nullable<System.Int64> ReceiveBin { get; set; }

        public Nullable<System.DateTime> ReceiveDate { get; set; }

        public Nullable<System.Double> ReceiveQuantity { get; set; }
    }

    [Serializable]
    public class ReceiveOrderDetailsDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //OrderDetailGUID
        [Display(Name = "OrderDetailGUID", ResourceType = typeof(ResReceiveOrderDetails))]
        public Nullable<Guid> OrderDetailGUID { get; set; }

        //ReceiveBin
        [Display(Name = "ReceiveBin", ResourceType = typeof(ResReceiveOrderDetails))]
        public Nullable<System.Int64> ReceiveBin { get; set; }

        //ReceiveDate
        [Display(Name = "ReceiveDate", ResourceType = typeof(ResReceiveOrderDetails))]
        public Nullable<System.DateTime> ReceiveDate { get; set; }

        //ReceiveQuantity
        [Display(Name = "ReceiveQuantity", ResourceType = typeof(ResReceiveOrderDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ReceiveQuantity { get; set; }

        //Created
        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //LastUpdated
        [Display(Name = "LastUpdated", ResourceType = typeof(ResReceiveOrderDetails))]
        public Nullable<System.DateTime> LastUpdated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        public string ReceiveBinName { get; set; }

        public OrderDetailsDTO OrderDetail { get; set; }

    }

    public class ResReceiveOrderDetails
    {
        private static string ResourceFileName = "ResReceiveOrderDetails";

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
        ///   Looks up a localized string similar to ReceiveOrderDetails {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to ReceiveOrderDetails.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReceiveOrderDetails.
        /// </summary>
        public static string ReceiveOrderDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveOrderDetails", ResourceFileName);
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
        ///   Looks up a localized string similar to OrderDetailID.
        /// </summary>
        public static string OrderDetailID
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderDetailID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OrderDetailGUID.
        /// </summary>
        public static string OrderDetailGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderDetailGUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReceiveBin.
        /// </summary>
        public static string ReceiveBin
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveBin", ResourceFileName);
            }
        }

        public static string BinOnHandQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("BinOnHandQTY", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReceiveDate.
        /// </summary>
        public static string ReceiveDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveDate", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to RequestedQuantity.
        /// </summary>
        public static string RequestedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestedQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReceiveQuantity.
        /// </summary>
        public static string ReceiveQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveQuantity", ResourceFileName);
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
        ///   Looks up a localized string similar to LastUpdated.
        /// </summary>
        public static string LastUpdated
        {
            get
            {
                return ResourceRead.GetResourceValue("LastUpdated", ResourceFileName);
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
        ///   Looks up a localized string similar to GUID.
        /// </summary>
        public static string GUID
        {
            get
            {
                return ResourceRead.GetResourceValue("GUID", ResourceFileName);
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
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string ItemID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string ItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OrderID.
        /// </summary>
        public static string Order
        {
            get
            {
                return ResourceRead.GetResourceValue("Order", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RequiredDate.
        /// </summary>
        public static string RequiredDate
        {
            get
            {
                return ResourceRead.GetResourceValue("RequiredDate", ResourceFileName);
            }
        }


        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", ResourceFileName);
            }
        }
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", ResourceFileName);
            }
        }
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", ResourceFileName);
            }
        }
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", ResourceFileName);
            }
        }
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", ResourceFileName);
            }
        }

        public static string Receive
        {
            get
            {
                return ResourceRead.GetResourceValue("Receive", ResourceFileName);
            }
        }

        public static string IsEDISent
        {
            get
            {
                return ResourceRead.GetResourceValue("IsEDISent", ResourceFileName);
            }
        }

        public static string LastEDIDate
        {
            get
            {
                return ResourceRead.GetResourceValue("LastEDIDate", ResourceFileName);
            }
        }

        public static string MsgSelectDataToReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectDataToReceive", ResourceFileName);
            }
        }

        public static string MsgOrderLineItemClosed
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgOrderLineItemClosed", ResourceFileName);
            }
        }

        public static string MsgOrderClosed
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgOrderClosed", ResourceFileName);
            }
        }

        public static string MsgBinNumberMandatory
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgBinNumberMandatory", ResourceFileName);
            }
        }

        public static string MsgPackslipMandatory
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPackslipMandatory", ResourceFileName);
            }
        }

        public static string MsgEnterDataToReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterDataToReceive", ResourceFileName);
            }
        }

        public static string MsgEnterQtyToReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterQtyToReceive", ResourceFileName);
            }
        }

        public static string MsgQtyReceiveEqualsOrderUOM
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQtyReceiveEqualsOrderUOM", ResourceFileName);
            }
        }

        public static string MsgEnterLotAndExpirationToReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterLotAndExpirationToReceive", ResourceFileName);
            }
        }
        public static string MsgDuplicateSerialFound
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDuplicateSerialFound", ResourceFileName);
            }
        }
        public static string MsgEnterExpirationToReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterExpirationToReceive", ResourceFileName);
            }
        }

        public static string MsgEnterLotToReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterLotToReceive", ResourceFileName);
            }
        }

        public static string MsgEnterSerialToReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterSerialToReceive", ResourceFileName);
            }
        }

        public static string MsgSelectRowToReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectRowToReceive", ResourceFileName);
            }
        }
        public static string MsgSaveReceiveInfoAjaxError
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSaveReceiveInfoAjaxError", ResourceFileName);
            }
        }
        public static string MsgBinNumberValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgBinNumberValidation", ResourceFileName);
            }
        }
        public static string MsgSerialNumberValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSerialNumberValidation", ResourceFileName);
            }
        }
        public static string MsgDuplicateSerialNumberValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDuplicateSerialNumberValidation", ResourceFileName);
            }
        }
        public static string MsgEnterQuantityReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterQuantityReceive", ResourceFileName);
            }
        }
        public static string MsgEnterPayslipNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterPayslipNumber", ResourceFileName);
            }
        }
        public static string MsgEnterSupplier
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterSupplier", ResourceFileName);
            }
        }
        public static string MsgEnterLotNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterLotNumber", ResourceFileName);
            }
        }
        public static string MsgExpireDateValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgExpireDateValidation", ResourceFileName);
            }
        }
        public static string MsgReceiveReasons
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgReceiveReasons", ResourceFileName);
            }
        }
        public static string MsgExceedApprovedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgExceedApprovedQuantity", ResourceFileName);
            }
        }
        public static string MsgReceivedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgReceivedSuccessfully", ResourceFileName);
            }
        }
        public static string MsgSelectLineItem
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectLineItem", ResourceFileName);
            }
        }
        public static string MsgSelectClosedLineItem
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectClosedLineItem", ResourceFileName);
            }
        }
        public static string MsgNoPreviousReceiptToEdit
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNoPreviousReceiptToEdit", ResourceFileName);
            }
        }
        public static string MsgSelectUnclosedItemValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectUnclosedItemValidation", ResourceFileName);
            }
        }
        public static string MsgSelectOnlyOneRecord
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectOnlyOneRecord", ResourceFileName);
            }
        }
        public static string MsgRecordsNotDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRecordsNotDeleted", ResourceFileName);
            }
        }
        public static string MsgSelectRecordToOrders
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectRecordToOrders", ResourceFileName);
            }
        }



        public static string msgBinSaved
        {
            get
            {
                return ResourceRead.GetResourceValue("msgBinSaved", ResourceFileName);
            }
        }
        public static string ReqRowToSaveBin
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqRowToSaveBin", ResourceFileName);
            }
        }

        public static string MsgErrorWhileReceving
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgErrorWhileReceving", ResourceFileName);
            }
        }
        public static string MsgSelectLocationToReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectLocationToReceive", ResourceFileName);
            }
        }
        public static string MsgClosedOrderNotReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgClosedOrderNotReceive", ResourceFileName);
            }
        }
        public static string EnterValidReceivedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterValidReceivedDate", ResourceFileName);
            }
        }
        public static string EnterLotAndValidExpirationDate
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterLotAndValidExpirationDate", ResourceFileName);
            }
        }
        public static string BinNoAlreadyExist
        {
            get
            {
                return ResourceRead.GetResourceValue("BinNoAlreadyExist", ResourceFileName);
            }
        }

        public static string OpenOrders
        {
            get
            {
                return ResourceRead.GetResourceValue("OpenOrders", ResourceFileName);
            }
        }
        public static string CloseOrders
        {
            get
            {
                return ResourceRead.GetResourceValue("CloseOrders", ResourceFileName);
            }
        }
        public static string NoItemToCreateOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("NoItemToCreateOrder", ResourceFileName);
            }
        }
        public static string IsDuplicate
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDuplicate", ResourceFileName);
            }
        }
        public static string BinCantEmptySelectIt
        {
            get
            {
                return ResourceRead.GetResourceValue("BinCantEmptySelectIt", ResourceFileName);
            }
        }

        public static string EnterValidReceivedData
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterValidReceivedData", ResourceFileName);
            }
        }
        public static string EnterValidSerialNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterValidSerialNumber", ResourceFileName);
            }
        }
        public static string ReqOrderNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqOrderNumber", ResourceFileName);
            }
        }
        public static string ReqReceiveBinName
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqReceiveBinName", ResourceFileName);
            }
        }
        public static string ReqRequestedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqRequestedQuantity", ResourceFileName);
            }
        }
        public static string ReqReceiveQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqReceiveQuantity", ResourceFileName);
            }
        }
        public static string MsgAllSerialsReceived
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAllSerialsReceived", ResourceFileName);
            }
        }
        public static string ReqQtyToReceiveQuickList
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqQtyToReceiveQuickList", ResourceFileName);
            }
        }
        public static string ReqSupplier
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqSupplier", ResourceFileName);
            }
        }
        public static string EmailReceivableItems { get { return ResourceRead.GetResourceValue("EmailReceivableItems", ResourceFileName); } }
        public static string EmailReceivedItems { get { return ResourceRead.GetResourceValue("EmailReceivedItems", ResourceFileName); } }
        public static string Receivable { get { return ResourceRead.GetResourceValue("Receivable", ResourceFileName); } }
        public static string Received { get { return ResourceRead.GetResourceValue("Received", ResourceFileName); } }
        public static string MsgErrorNotReceived
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgErrorNotReceived", ResourceFileName);
            }
        }
        public static string MsgNewRowTransferQuantityValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNewRowTransferQuantityValidation", ResourceFileName);
            }
        }
        public static string MsgApprovedReasons
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgApprovedReasons", ResourceFileName);
            }
        }
        public static string MsgRecieveQtyValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRecieveQtyValidation", ResourceFileName);
            }
        }
        public static string MsgFailInsertUpdateOrderReceiveDetail
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgFailInsertUpdateOrderReceiveDetail", ResourceFileName);
            }
        }
        public static string MsgTransferedSuccess
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgTransferedSuccess", ResourceFileName);
            }
        }
        public static string MsgQtyItemValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQtyItemValidation", ResourceFileName);
            }
        }
        public static string MsgQtyLocationLotValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQtyLocationLotValidation", ResourceFileName);
            }
        }
        public static string MsgTransferQtyValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgTransferQtyValidation", ResourceFileName);
            }
        }
        public static string MsgProvideOrderReleaseNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgProvideOrderReleaseNumber", ResourceFileName);
            }
        }
        public static string MsgOrderStatusNotReceivable
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgOrderStatusNotReceivable", ResourceFileName);
            }
        }
        public static string MsgProvideCorrectItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgProvideCorrectItemNumber", ResourceFileName);
            }
        }
        public static string MsgLaborItemReceivedValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLaborItemReceivedValidation", ResourceFileName);
            }
        }
        public static string MsgProvidePackslipNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgProvidePackslipNumber", ResourceFileName);
            }
        }
        public static string MsgProvideLotNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgProvideLotNumber", ResourceFileName);
            }
        }
        public static string MsgSerialNumberAlreadyExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSerialNumberAlreadyExist", ResourceFileName);
            }
        }
        public static string OrderLineItemNotHaveItem
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderLineItemNotHaveItem", ResourceFileName);
            }
        }
        public static string ReleaseNumberRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("ReleaseNumberRequired", ResourceFileName);
            }
        }
        public static string BinNameRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("BinNameRequired", ResourceFileName);
            }
        }
        public static string MsgReceiveQuantityValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgReceiveQuantityValidation", ResourceFileName);
            }
        }
        public static string MsgRecieveEnterValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRecieveEnterValidation", ResourceFileName);
            }
        }
        public static string TitlePreReceiveInfo
        {
            get
            {
                return ResourceRead.GetResourceValue("TitlePreReceiveInfo", ResourceFileName);
            }
        }
        public static string ValidateSomeItemnotReturn
        {
            get
            {
                return ResourceRead.GetResourceValue("ValidateSomeItemnotReturn", ResourceFileName);
            }
        }
        public static string MsgQuantityReturnSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQuantityReturnSuccessfully", ResourceFileName);
            }
        }
        public static string MsgAllQuantityReturnSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAllQuantityReturnSuccessfully", ResourceFileName);
            }
        }
        public static string MsgCannotReturnMoreQtyThenAvaQty
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCannotReturnMoreQtyThenAvaQty", ResourceFileName);
            }
        }
        public static string ValidateTotalEnterwithActualReturnQty
        {
            get
            {
                return ResourceRead.GetResourceValue("ValidateTotalEnterwithActualReturnQty", ResourceFileName);
            }
        }

        public static string InvalidLineItem
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidLineItem", ResourceFileName);
            }
        }
        public static string LotNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("LotNumber", ResourceFileName);
            }
        }
        public static string SerialNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialNumber", ResourceFileName);
            }
        }
        public static string ExpirationDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ExpirationDate", ResourceFileName);
            }
        }
        public static string PackSlipNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("PackSlipNumber", ResourceFileName);
            }
        }
        public static string ShippingTrackNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ShippingTrackNumber", ResourceFileName);
            }
        }
    }
    public class ResOrderDetails
    {
        private static string ResourceFileName = "ResOrderDetails";

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", ResourceFileName);
            }
        }
        public static string SaveErrorMsgApprovedQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("SaveErrorMsgApprovedQTY", ResourceFileName);
            }
        }

        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", ResourceFileName);
            }
        }
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", ResourceFileName);
            }
        }
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", ResourceFileName);
            }
        }
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", ResourceFileName);
            }
        }
        public static string MsgFailInsertUpdateOrderDetail
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgFailInsertUpdateOrderDetail", ResourceFileName);
            }
        }

        public static string BackOrderQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("BackOrderQuantity", ResourceFileName);
            }
        }
        public static string BackOrderExpectedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("BackOrderExpectedDate", ResourceFileName);
            }
        }
    }
    public class ReceiveOrderLineItemDetailsDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //OrderID
        [Display(Name = "OrderID", ResourceType = typeof(ResOrder))]
        public Nullable<System.Int64> OrderID { get; set; }

        public Nullable<Guid> GUID { get; set; }

        public Nullable<Guid> OrderGUID { get; set; }

        //ItemGUID        
        public Nullable<Guid> ItemGUID { get; set; }

        //ItemID
        [Display(Name = "ItemID", ResourceType = typeof(ResOrder))]
        public Nullable<System.Int64> ItemID { get; set; }

        //ItemID
        [Display(Name = "ItemID", ResourceType = typeof(ResOrder))]
        public Nullable<System.Int32> ItemType { get; set; }

        //ItemID
        [Display(Name = "DefaultBin", ResourceType = typeof(ResOrder))]
        public Nullable<System.Int64> DefaultBin { get; set; }

        //RequestedQuantity
        [Display(Name = "RequestedQuantity", ResourceType = typeof(ResOrder))]
        public Nullable<System.Double> RequestedQuantity { get; set; }

        //RequiredDate
        [Display(Name = "RequiredDate", ResourceType = typeof(ResOrder))]
        public Nullable<System.DateTime> RequiredDate { get; set; }

        //ReceivedQuantity
        [Display(Name = "ReceivedQuantity", ResourceType = typeof(ResOrder))]
        public Nullable<System.Double> ReceivedQuantity { get; set; }

        [Display(Name = "SupplierName", ResourceType = typeof(ResOrder))]
        public System.String SupplierName { get; set; }

        [Display(Name = "OrderNumber", ResourceType = typeof(ResOrder))]
        public System.String OrderNumber { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        public System.String ItemNumber { get; set; }

        public System.String Description { get; set; }

        public Nullable<System.Int64> Supplier { get; set; }

        //ReceivedQuantity
        [Display(Name = "ReceivedQuantity", ResourceType = typeof(ResOrder))]
        public Nullable<System.DateTime> CurrentReceivedDate { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        //Created
        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //LastUpdated
        [Display(Name = "LastUpdated", ResourceType = typeof(ResOrder))]
        public Nullable<System.DateTime> LastUpdated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        public Nullable<System.Int64> StagingID { get; set; }
        public string CustomerAddress { get; set; }
    }

    public class ReceivableItemDTO
    {
        public bool isOktaEnable { get; set; }
        public string OrderNumber { get; set; }
        public Guid OrderGUID { get; set; }
        public int OrderStatus { get; set; }
        public string OrderReleaseNumber { get; set; }
        public Int64? OrderSupplierID { get; set; }
        public string ItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public int ItemType { get; set; }
        public Guid ItemGUID { get; set; }
        public Guid OrderDetailGUID { get; set; }
        public Int64? ReceiveBinID { get; set; }
        public string ReceiveBinName { get; set; }
        public string OrderSupplierName { get; set; }
        public string OrderStatusText { get; set; }

        public char OrderStatusChar { get { return FnCommon.GetOrderStatusChar(OrderStatus); } }
        public DateTime OrderRequiredDate { get; set; }
        public DateTime OrderDetailRequiredDate { get; set; }
        public double RequestedQuantity { get; set; }
        public double ApprovedQuantity { get; set; }
        public double ReceivedQuantity { get; set; }
        public double? InTransitQuantity { get; set; }
        public int TotalRecords { get; set; }
        public Int64 OrderCreatedByID { get; set; }
        public Int64 OrderLastUpdatedByID { get; set; }
        public string OrderCreatedByName { get; set; }
        public string OrderUpdatedByName { get; set; }
        public DateTime OrderLastUpdated { get; set; }
        public DateTime OrderCreated { get; set; }
        public bool DateCodeTracking { get; set; }
        public bool SerialNumberTracking { get; set; }
        public bool LotNumberTracking { get; set; }
        public double ItemCost { get; set; }
        public double ItemAverageCost { get; set; }
        public double ItemSellPrice { get; set; }
        public bool ItemConsignment { get; set; }
        public Int64 ItemDefaultLocation { get; set; }
        public Int64 StagingID { get; set; }
        public Int64 ItemID { get; set; }
        public Int64 OrderID { get; set; }
        public Int64 OrderDetailID { get; set; }
        public string ItemDefaultLocationName { get; set; }
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

        public double PackingQuantity { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerNumber { get; set; }
        public string SupplierPartNumber { get; set; }
        public string SupplierName { get; set; }
        public string UnitName { get; set; }
        public bool IsTransfer { get; set; }
        public bool IsPurchase { get; set; }
        public string ASNNumber { get; set; }
        public string StagingName { get; set; }
        public string PackSlipNumber { get; set; }
        public string ShippingTrackNumber { get; set; }
        public string ODPackSlipNumbers { get; set; }
        public string OrderNumber_ForSorting { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public IEnumerable<ReceivedOrderTransferDetailDTO> ReceivedItemDetail { get; set; }
        public bool IsPackSlipNumberMandatory { get; set; }
        public Int64? ItemSupplierID { get; set; }
        public bool IsOnlyFromUI { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
        public DateTime ReceivedOn { get; set; }
        public string OrderRequiredDateStr { get; set; }
        public bool? IsEDIOrder { get; set; }
        public bool? IsCloseItem { get; set; }
        public bool Consignment { get; set; }
        public double ItemMinimumQuantity { get; set; }
        public double ItemMaximumQuantity { get; set; }

        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate))
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(OrderCreated, true);
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
                    _updatedDate = FnCommon.ConvertDateByTimeZone(OrderLastUpdated, true);
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

        private string _ReqDate;
        public string strReqDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ReqDate))
                {
                    _ReqDate = FnCommon.ConvertDateByTimeZone(OrderRequiredDate, true, true);
                }
                return _ReqDate;
            }
            set { this._ReqDate = value; }
        }

        private string _ReqDtlDate;
        public string strReqDtlDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ReqDtlDate))
                {
                    _ReqDtlDate = FnCommon.ConvertDateByTimeZone(OrderDetailRequiredDate, true, true);
                }
                return _ReqDtlDate;
            }
            set { this._ReqDtlDate = value; }
        }
        public string CostUOM { get; set; }
        public string OrderUOM { get; set; }
        public int? OrderUOMValue { get; set; }
        public Nullable<System.Double> OnHandQuantity { get; set; }

        public double RequestedQuantityUOM { get; set; }
        public double ApprovedQuantityUOM { get; set; }
        public double ReceivedQuantityUOM { get; set; }
        public bool IsAllowOrderCostuom { get; set; }
        public Nullable<System.Double> BinOnHandQTY { get; set; }
        public Nullable<double> OrderItemCost { get; set; }
        public Nullable<System.Double> OnOrderQuantity { get; set; }
        public List<FileAttachmentReceiveList> AttachmentFileNames { get; set; }
        public bool IsBackOrdered { get; set; }
        public Nullable<bool> OrderLineException { get; set; }
        public string OrderLineExceptionDesc { get; set; }
    }

    public class FileAttachmentReceiveList
    {
        public Guid FileGUID { get; set; }
        public string FileName { get; set; }
    }

    public class QLReceivedOrderDetail
    {
        public DateTime ReceivedDate { get; set; }

        public string ReceivedDateStr { get; set; }
        public Nullable<double> Cost { get; set; }
        public string BinNumber { get; set; }
        public Guid ItemGUID { get; set; }
        public string PackSlipNumber { get; set; }
        public Nullable<double> ConsignedQuantity { get; set; }
        public Nullable<double> CustomerOwnedQuantity { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }

        public bool LotNumberTracking { get; set; }
        public bool DateCodeTracking { get; set; }
        public bool SerialNumberTracking { get; set; }
        public int ItemType { get; set; }

        public Guid OrderDetailGUID { get; set; }

    }

    public class QLOrderItemDetail
    {
        public double RequestedQuantity { get; set; }
        public Guid ItemGUID { get; set; }
        public string BinName { get; set; }
        public Int64 BinID { get; set; }
        public Nullable<double> Cost { get; set; }
        public Nullable<double> ApprovedQuantity { get; set; }
        public Guid OrderGUID { get; set; }
        public Guid OrderDetailGuid { get; set; }
        public QLReceivedOrderDetail ReceivedOrderDetail { get; set; }

    }

    public class QLOrderHeader
    {
        public string OrderNumber { get; set; }
        public string OrderNumber_ForSorting { get; set; }
        public Int64 Supplier { get; set; }
        public DateTime RequiredDate { get; set; }
        public string RequiredDateStr { get; set; }
        public int OrderStatus { get; set; }
        public string PackSlipNumber { get; set; }
        public string Comment { get; set; }
        public string ReleaseNumber { get; set; }
        public Guid OrderGuid { get; set; }
        public List<QLOrderItemDetail> OrderItemDetail { get; set; }
    }


    public class RPT_ReceiveDTO
    {
        public Int64 ItemID { get; set; }
        public string ItemNumber { get; set; }
        public string OrderNumber { get; set; }
        public string ItemSupplier { get; set; }
        public string SupplierPartNo { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerNumber { get; set; }
        public double? ItemCost { get; set; }
        public string ReceiveBin { get; set; }
        public double? RequestedQuantity { get; set; }
        public double? ApprovedQuantity { get; set; }
        public double? ReceivedQuantity { get; set; }
        public double? Total { get; set; }
        public string LotNumber { get; set; }
        public string SerialNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string ReceivedDate { get; set; }
        public double? RecievedCost { get; set; }
        public string ReceivedUDF1 { get; set; }
        public string ReceivedUDF2 { get; set; }
        public string ReceivedUDF3 { get; set; }
        public string ReceivedUDF4 { get; set; }
        public string ReceivedUDF5 { get; set; }
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
        public string LastUpdatedBy { get; set; }
        public int? CostDecimalPoint { get; set; }
        public int? QuantityDecimalPoint { get; set; }
        public string OrderStatus { get; set; }
        public char OrderStatusChar
        {
            get
            {
                return FnCommon.GetOrderStatusChar(OrderStatus);
            }
        }
        public string OrderType { get; set; }
        public string RequiredDate { get; set; }
        public string PackslipNumber { get; set; }
        public string OrderSupplier { get; set; }
        public string LastUpdatedOn { get; set; }
        public string OrderNumber_ForSorting { get; set; }
        public Guid ItemGuid { get; set; }
        public string RoomInfo { get; set; }
        public string ItemDescription { get; set; }
        public string CategoryName { get; set; }
        public string UOM { get; set; }
        public double? OnHandQuantity { get; set; }
        public double? StagedQuantity { get; set; }
        public double? OnOrderQuantity { get; set; }
        public double? CriticalQuantity { get; set; }
        public double? MinimumQuantity { get; set; }
        public double? MaximumQuantity { get; set; }
        public string UPC { get; set; }
        public string UNSPSC { get; set; }
        public double? SellPrice { get; set; }
        public double? ExtendedCost { get; set; }
        public double? AverageCost { get; set; }
        public string ShippingTrackNumber { get; set; }
        public Int64? SupplierID { get; set; }
        //public double? OD_RequestedQuantity { get; set; }
        //public double? OD_ApprovedQuantity { get; set; }
        //public double? OD_ReceivedQuantity { get; set; }
        //public double? OD_InTransitQuantity { get; set; }
        //public double? O_OrderCost { get; set; }
        //public double? ReceivedCost { get; set; }
        //public string R_FirstReceiveDate { get; set; }
        //public string R_LastReceiveDate { get; set; }
        //public string ReceivedDate { get; set; }
        //public string OD_RequiredDate { get; set; }
        //public string OD_CreatedOn { get; set; }
        //public string OD_UpdatedOn { get; set; }
        //public string OD_LastEDIDate { get; set; }
        //public string O_RequiredDate { get; set; }
        //public string O_OrderDate { get; set; }
        //public string O_CreatedOn { get; set; }
        //public string O_UpdatedOn { get; set; }
        //public string OD_IsEDIRequired { get; set; }
        //public string OD_IsEDISent { get; set; }
        //public string OD_IsDeleted { get; set; }
        //public string OD_IsArchived { get; set; }
        //public string O_IsDeleted { get; set; }
        //public string O_IsArchived { get; set; }
        //public string OD_ASNNumber { get; set; }
        //public string OD_BinName { get; set; }
        //public string O_CreatedByName { get; set; }
        //public string O_UpdatedByName { get; set; }
        //public string OD_CreatedByName { get; set; }
        //public string OD_UpdatedByName { get; set; }
        //public string OD_RoomName { get; set; }
        //public string OD_CompanyName { get; set; }
        //public string OD_PackSlipNumbers { get; set; }
        //public string O_OrderNumber { get; set; }
        //public string O_Comment { get; set; }
        //public string O_ReleaseNumber { get; set; }
        //public string O_UDF1 { get; set; }
        //public string O_UDF2 { get; set; }
        //public string O_UDF3 { get; set; }
        //public string O_UDF4 { get; set; }
        //public string O_UDF5 { get; set; }
        //public string O_Shipping { get; set; }
        //public string O_ShippingTrackNumber { get; set; }
        //public int? O_NoOfLineItems { get; set; }
        //public Int64? O_ChangeOrderRevisionNo { get; set; }
        //public string O_Vendor { get; set; }
        //public string O_Customer { get; set; }
        //public string O_StagingHeader { get; set; }
        //public string O_Supplier { get; set; }
        //public string O_OrderStatus { get; set; }
        //public string O_OrderTypeString { get; set; }
        //public Int64? OD_BinID { get; set; }
        //public int O_OrderStatusID { get; set; }
        //public int? O_OrderTypeID { get; set; }
        //public Int64 O_SupplierID { get; set; }
        //public Int64? O_StagingID { get; set; }
        //public Int64? O_CustomerID { get; set; }
        //public Int64? O_ShipVaiID { get; set; }
        //public Int64? O_VendorID { get; set; }
        //public int? CostDecimalPoint { get; set; }
        //public string O_WhatWhereAction { get; set; }
        //public string O_OrderNumber_ForSorting { get; set; }
        //public Int64? O_ID { get; set; }
        //public Int64 OD_ID { get; set; }
        //public Guid OD_Guid { get; set; }
        //public Guid? OD_OrderGUID { get; set; }
        //public Guid? OD_ItemGUID { get; set; }
        //public Int64? OD_CreatedByID { get; set; }
        //public Int64? OD_LastUpdatedByID { get; set; }
        //public Int64? O_LastUpdatedByID { get; set; }
        //public Int64? O_CreatedByID { get; set; }
        //public Int64? OD_RoomID { get; set; }
        //public Int64? OD_CompanyID { get; set; }
        //public string ItemNumber { get; set; }
        //public string ItemUniqueNumber { get; set; }
        //public string ManufacturerNumber { get; set; }
        //public string SupplierPartNo { get; set; }
        //public string UPC { get; set; }
        //public string UNSPSC { get; set; }
        //public string ItemDescription { get; set; }
        //public string LongDescription { get; set; }
        //public string IsLotSerialExpiryCost { get; set; }
        //public string ImagePath { get; set; }
        //public string ItemUDF1 { get; set; }
        //public string ItemUDF2 { get; set; }
        //public string ItemUDF3 { get; set; }
        //public string ItemUDF4 { get; set; }
        //public string ItemUDF5 { get; set; }
        //public string Link1 { get; set; }
        //public string Link2 { get; set; }
        //public string BondedInventory { get; set; }
        //public string ItemWhatWhereAction { get; set; }
        //public string ItemCreatedByName { get; set; }
        //public string ItemUpdatedByName { get; set; }
        //public string ItemRoomName { get; set; }
        //public string ItemCompanyName { get; set; }
        public string ItemBlanketPO { get; set; }
        public string SupplierName { get; set; }
        //public string ManufacturerName { get; set; }
        //public string CategoryName { get; set; }
        //public string GLAccount { get; set; }
        //public string Unit { get; set; }
        //public string DefaultLocationName { get; set; }
        //public string InventoryClassificationName { get; set; }
        //public string CostUOM { get; set; }
        //public int? InventoryClassification { get; set; }
        //public byte? TrendingSetting { get; set; }
        //public int? ItemType { get; set; }
        //public int? LeadTimeInDays { get; set; }
        //public string ItemTypeName { get; set; }
        //public string ItemCost { get; set; }
        //public string SellPrice { get; set; }
        //public string ExtendedCost { get; set; }
        //public string AverageCost { get; set; }
        //public string PricePerTerm { get; set; }
        //public string OnHandQuantity { get; set; }
        //public string StagedQuantity { get; set; }
        //public string ItemInTransitquantity { get; set; }
        //public string OnOrderQuantity { get; set; }
        //public string OnReturnQuantity { get; set; }
        //public string OnTransferQuantity { get; set; }
        //public string SuggestedOrderQuantity { get; set; }
        //public string RequisitionedQuantity { get; set; }
        //public string PackingQuantity { get; set; }
        //public string CriticalQuantity { get; set; }
        //public string MinimumQuantity { get; set; }
        //public string MaximumQuantity { get; set; }
        //public string DefaultReorderQuantity { get; set; }
        //public string DefaultPullQuantity { get; set; }
        //public string AverageUsage { get; set; }
        //public string Turns { get; set; }
        //public string Markup { get; set; }
        //public string WeightPerPiece { get; set; }
        //public string ItemCreatedOn { get; set; }
        //public string ItemUpdatedOn { get; set; }
        //public string Consignment { get; set; }
        //public string ItemIsDeleted { get; set; }
        //public string ItemIsArchived { get; set; }
        //public string IsTransfer { get; set; }
        //public string IsPurchase { get; set; }
        //public string SerialNumberTracking { get; set; }
        //public string LotNumberTracking { get; set; }
        //public string DateCodeTracking { get; set; }
        //public string IsBOMItem { get; set; }
        //public string IsBuildBreak { get; set; }
        //public string IsItemLevelMinMaxQtyRequired { get; set; }
        //public string IsEnforceDefaultReorderQuantity { get; set; }
        //public string IsAutoInventoryClassification { get; set; }
        //public string PullQtyScanOverride { get; set; }
        //public string Trend { get; set; }
        //public string Taxable { get; set; }
        //public Int64? ItemID { get; set; }
        //public Guid? ItemGUID { get; set; }
        //public Int64? ItemCompanyID { get; set; }
        //public Int64? ItemRoomID { get; set; }
        public Int64? ManufacturerID { get; set; }
        //public Int64? SupplierID { get; set; }
        public Int64? CategoryID { get; set; }
        //public Int64? GLAccountID { get; set; }
        //public Int64? UOMID { get; set; }
        //public Int64? CostUOMID { get; set; }
        //public Int64? RefBomId { get; set; }
        //public Int64? ItemCreatedBy { get; set; }
        //public Int64? ItemLastUpdatedBy { get; set; }
        //public Int64? DefaultLocationID { get; set; }
        //public string RoomInfo { get; set; }
        //public string CompanyInfo { get; set; }
        //public string BarcodeImage_ItemNumber { get; set; }
        //public string BarcodeImage_DefaultLocationName { get; set; }
        //public string BarcodeImage_ODBinNumber { get; set; }
        //public string BarcodeImage_OrderNumber { get; set; }
        //public double? Total { get; set; }
        //public string Room_RoomInfo { get; set; }
        //public string CustomerInfo { get; set; }
    }

    public class RPT_ReceivableItemDTO
    {
        public Guid OrderDetailGuid { get; set; }
        public Guid ItemGUID { get; set; }
        public Guid GUID { get; set; }
        public string ItemNumber { get; set; }
        public string OrderSupplier { get; set; }
        public string OrderNumber { get; set; }
        public string ItemSupplier { get; set; }
        public string BinName { get; set; }
        public double? ReceivedQuantity { get; set; }
        public double? ApprovedQuantity { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public string SupplierPartNo { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerNumber { get; set; }
        public string ItemBlanketPO { get; set; }
        public string SupplierName { get; set; }
        public string CategoryName { get; set; }
        public string UNSPSC { get; set; }

    }
}


