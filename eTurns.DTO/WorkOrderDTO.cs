using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    [Serializable]
    public class WorkOrderDTO
    {

        public System.Int64 ID { get; set; }

        public Guid GUID { get; set; }

        [Display(Name = "WOName", ResourceType = typeof(ResWorkOrder))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(256, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]

        public System.String WOName { get; set; }

        [Display(Name = "SubJob", ResourceType = typeof(ResWorkOrder))]
        [StringLength(256, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String SubJob { get; set; }

        [Display(Name = "TechnicianID", ResourceType = typeof(ResWorkOrder))]
        public Nullable<System.Int64> TechnicianID { get; set; }

        [Display(Name = "Technician", ResourceType = typeof(ResTechnician))]
        public System.String Technician { get; set; }

        [Display(Name = "ProjectSpendName", ResourceType = typeof(ResWorkOrder))]
        public System.String ProjectSpendName { get; set; }

        [Display(Name = "CustomerID", ResourceType = typeof(ResWorkOrder))]
        public Nullable<System.Int64> CustomerID { get; set; }

        [Display(Name = "Customer", ResourceType = typeof(ResCustomer))]
        public System.String Customer { get; set; }

        [Display(Name = "JobType", ResourceType = typeof(ResWorkOrder))]
        public Nullable<System.Int64> JobTypeID { get; set; }

        [Display(Name = "GXPRConsigmentJobID", ResourceType = typeof(ResWorkOrder))]
        public Nullable<System.Int64> GXPRConsigmentJobID { get; set; }

        [Display(Name = "AssetGUID", ResourceType = typeof(ResWorkOrder))]
        public Nullable<System.Guid> AssetGUID { get; set; }

        [Display(Name = "AssetName", ResourceType = typeof(ResAssetMaster))]
        public System.String Asset { get; set; }

        [Display(Name = "ToolGUID", ResourceType = typeof(ResWorkOrder))]
        public Nullable<System.Guid> ToolGUID { get; set; }

        [Display(Name = "ToolName", ResourceType = typeof(ResToolMaster))]
        public System.String Tool { get; set; }

        [Display(Name = "AssetName", ResourceType = typeof(ResAssetMaster))]
        public string AssetName { get; set; }

        [Display(Name = "ToolName", ResourceType = typeof(ResToolMaster))]
        public string ToolName { get; set; }

        [Display(Name = "WOType", ResourceType = typeof(ResWorkOrder))]
        public System.String WOType { get; set; }

        [Display(Name = "WOStatus", ResourceType = typeof(ResWorkOrder))]
        public System.String WOStatus { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        public Nullable<Boolean> IsDeleted { get; set; }

        public Nullable<Boolean> IsArchived { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResWorkOrder))]
        public System.String UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResWorkOrder))]
        public System.String UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResWorkOrder))]
        public System.String UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResWorkOrder))]
        public System.String UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResWorkOrder))]
        public System.String UDF5 { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "TrackingMeasurement", ResourceType = typeof(ResToolsMaintenance))]
        public int TrackngMeasurement { get; set; }

        public bool IsHistory { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "Odometer_OperationHours", ResourceType = typeof(ResWorkOrder))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Odometer_OperationHours { get; set; }

        //NumberofItemsrequisitioned
        [Display(Name = "UsedItems", ResourceType = typeof(ResWorkOrder))]
        public Nullable<System.Int32> UsedItems { get; set; }

        //TotalCost
        [Display(Name = "UsedItemsCost", ResourceType = typeof(ResWorkOrder))]
        //[RegularExpression(@"^-?[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(float.MinValue, float.MaxValue)]
        public Nullable<System.Double> UsedItemsCost { get; set; }

        //TotalSellPrice
        [Display(Name = "UsedItemsSellPrice", ResourceType = typeof(ResWorkOrder))]
        [Range(float.MinValue, float.MaxValue)]
        public System.Double UsedItemsSellPrice { get; set; }

        public List<PullMasterViewDTO> WorkOrderListItem { get; set; }
        public string AppendedBarcodeString { get; set; }

        public System.String WhatWhereAction { get; set; }

        [Display(Name = "Description", ResourceType = typeof(ResWorkOrder))]
        public System.String Description { get; set; }

        [Display(Name = "RequisitionNumber", ResourceType = typeof(ResRequisitionMaster))]
        public System.String RequisitionNumber { get; set; }

        [Display(Name = "CustomerGUID", ResourceType = typeof(ResWorkOrder))]
        public Nullable<System.Guid> CustomerGUID { get; set; }

        public Guid? MaintenanceGUID { get; set; }

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


        [Display(Name = "Signature", ResourceType = typeof(ResWorkOrder))]
        public string SignatureName { get; set; }

        public bool IsSignatureCapture { get; set; }
        public bool IsSignatureRequired { get; set; }
        public string SignaturePath { get; set; }


        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EditedFrom { get; set; }

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


        public string CompanyName { get; set; }
        public string EnterpriseName { get; set; }

        [Display(Name = "ReleaseNumber", ResourceType = typeof(ResWorkOrder))]
        public string ReleaseNumber { get; set; }

        [Display(Name = "Supplier", ResourceType = typeof(ResOrder))]
        public Nullable<System.Int64> SupplierId { get; set; }
        [Display(Name = "SupplierAccountGuid", ResourceType = typeof(ResOrder))]
        public Nullable<Guid> SupplierAccountGuid { get; set; }
        public string SupplierName { get; set; }

        public string SupplierAccountNumberName { get; set; }

        public string SupplierAccountNumber { get; set; }
        public string SupplierAccountName { get; set; }
        public string SupplierAccountAddress { get; set; }
        public string SupplierAccountCity { get; set; }
        public string SupplierAccountState { get; set; }
        public string SupplierAccountZipcode { get; set; }
        public string SupplierAccountDetailWithFullAddress { get; set; }

        public int? CreatedFrom { get; set; }
        public int PriseSelectionOption { get; set; }

        public string TechnicianCode { get; set; }
        public string TechnicianName { get; set; }

        private string _TechnicianCodeNameStr;
        public string TechnicianCodeNameStr
        {
            get
            {
                if (string.IsNullOrEmpty(_TechnicianCodeNameStr))
                {
                    _TechnicianCodeNameStr = string.Empty;

                    if (!string.IsNullOrEmpty(TechnicianCode) && !string.IsNullOrWhiteSpace(TechnicianCode))
                    {
                        _TechnicianCodeNameStr = TechnicianCode;

                        if (!string.IsNullOrEmpty(TechnicianName) && !string.IsNullOrWhiteSpace(TechnicianName))
                        {
                            _TechnicianCodeNameStr += " --- " + TechnicianName;
                        }
                    }
                }
                return _TechnicianCodeNameStr;
            }
            set { this._TechnicianCodeNameStr = value; }
        }

        public int? TotalRecords { get; set; }
        public string SupplierAccount { get; set; }
    }

    public class RPT_WorkOrder
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public string WOName { get; set; }
        public string Technician { get; set; }
        public string Customer { get; set; }
        public string AssetName { get; set; }
        public string ToolName { get; set; }
        public string WOStatus { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string WOType { get; set; }
        public string RequisitionNumber { get; set; }
        public string ReleaseNumber { get; set; }

        public Int64? TechnicianID { get; set; }
        public Guid? CustomerGUID { get; set; }
        public Guid? AssetGUID { get; set; }
        public Guid? ToolGUID { get; set; }

        public string RoomName { get; set; }
        public string CompanyName { get; set; }
    }

    public class ResWorkOrder
    {
        private static string ResourceFileName = "ResWorkOrder";

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
        public static string Open
        {
            get
            {
                return ResourceRead.GetResourceValue("Open", ResourceFileName);
            }
        }

        public static string Close
        {
            get
            {
                return ResourceRead.GetResourceValue("Close", ResourceFileName);
            }
        }

        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", ResourceFileName);
            }
        }


        public static string Odometer_OperationHours
        {
            get
            {
                return ResourceRead.GetResourceValue("Odometer_OperationHours", ResourceFileName);
            }
        }

        public static string UncloseWO
        {
            get
            {
                return ResourceRead.GetResourceValue("UncloseWO", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to WorkOrder {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to WorkOrder.
        /// </summary>
        public static string WorkOrderHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("WorkOrderHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to WorkOrder.
        /// </summary>
        public static string WorkOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("WorkOrder", ResourceFileName);
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
        ///   Looks up a localized string similar to WOName.
        /// </summary>
        public static string WOName
        {
            get
            {
                return ResourceRead.GetResourceValue("WOName", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SubJob.
        /// </summary>
        public static string SubJob
        {
            get
            {
                return ResourceRead.GetResourceValue("SubJob", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TechnicianID.
        /// </summary>
        public static string TechnicianID
        {
            get
            {
                return ResourceRead.GetResourceValue("TechnicianID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CustomerID.
        /// </summary>
        public static string CustomerID
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CustomerGUID.
        /// </summary>
        public static string CustomerGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerGUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to JobType.
        /// </summary>
        public static string JobType
        {
            get
            {
                return ResourceRead.GetResourceValue("JobType", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to GXPRConsigmentJobID.
        /// </summary>
        public static string GXPRConsigmentJobID
        {
            get
            {
                return ResourceRead.GetResourceValue("GXPRConsigmentJobID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AssetID.
        /// </summary>
        public static string AssetID
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetID", ResourceFileName);
            }
        }
                
        public static string AssetGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetID", ResourceFileName);
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

        
        public static string ToolGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to WOType.
        /// </summary>
        public static string WOType
        {
            get
            {
                return ResourceRead.GetResourceValue("WOType", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to WOStatus.
        /// </summary>
        public static string WOStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("WOStatus", ResourceFileName);
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

        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }

        public static string ModelHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ModelHeader", ResourceFileName);
            }
        }

        public static string ToolModelHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolModelHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to NumberofItemsrequisitioned.
        /// </summary>
        public static string UsedItems
        {
            get
            {
                return ResourceRead.GetResourceValue("UsedItems", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TotalCost.
        /// </summary>
        public static string UsedItemsCost
        {
            get
            {
                return ResourceRead.GetResourceValue("UsedItemsCost", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UsedItemsSellPrice.
        /// </summary>
        public static string UsedItemsSellPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("UsedItemsSellPrice", ResourceFileName);
            }
        }

        public static string Requisition
        {
            get
            {
                return ResourceRead.GetResourceValue("Requisition", ResourceFileName);
            }
        }

        public static string AssetService
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetService", ResourceFileName);
            }
        }

        public static string ToolService
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolService", ResourceFileName);
            }
        }

        public static string WorkOrderType
        {
            get
            {
                return ResourceRead.GetResourceValue("WorkOrderType", ResourceFileName);
            }
        }

        public static string Signature
        {
            get
            {
                return ResourceRead.GetResourceValue("Signature", ResourceFileName);
            }
        }

        public static string ProjectSpendName
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpendName", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to WOName.
        /// </summary>
        public static string ReleaseNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ReleaseNumber", ResourceFileName);
            }
        }

        /// <summary>
        /// Get or Set Asset Name
        /// </summary>
        public static string Asset
        {
            get
            {
                return ResourceRead.GetResourceValue("Asset", ResourceFileName);
            }
        }

        public static string CloseSelectedWorkorders
        {
            get
            {
                return ResourceRead.GetResourceValue("CloseSelectedWorkorders", ResourceFileName);
            }
        }
        public static string MsgInvalidFileSelected
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidFileSelected", ResourceFileName);
            }
        }
        public static string MsgvalidFileList
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgvalidFileList", ResourceFileName);
            }
        }
        public static string MsgRemoveDuplicateFileName
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRemoveDuplicateFileName", ResourceFileName);
            }
        }
        public static string MsgUseBrowserToPrint
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUseBrowserToPrint", ResourceFileName);
            }
        }


        public static string DuplicateWorkorderAlreadyExist { get { return ResourceRead.GetResourceValue("DuplicateWorkorderAlreadyExist", ResourceFileName); } }
        public static string MsgWorkOrderValidation { get { return ResourceRead.GetResourceValue("MsgWorkOrderValidation", ResourceFileName); } }
        public static string MsgWorkOrderExists { get { return ResourceRead.GetResourceValue("MsgWorkOrderExists", ResourceFileName); } }
        public static string MsgAcceptLicenseWorkOrderValidation { get { return ResourceRead.GetResourceValue("MsgAcceptLicenseWorkOrderValidation", ResourceFileName); } }
        public static string MsgWorkOrderUserRights { get { return ResourceRead.GetResourceValue("MsgWorkOrderUserRights", ResourceFileName); } }
        public static string MsgWorkOrderNameValidation { get { return ResourceRead.GetResourceValue("MsgWorkOrderNameValidation", ResourceFileName); } }
        public static string WorkOrderGuidDelete { get { return ResourceRead.GetResourceValue("WorkOrderGuidDelete", ResourceFileName); } }
        public static string MsgWorkOrderNotAvailable { get { return ResourceRead.GetResourceValue("MsgWorkOrderNotAvailable", ResourceFileName); } }
        public static string MsgWorkOrderDeleted { get { return ResourceRead.GetResourceValue("MsgWorkOrderDeleted", ResourceFileName); } }
        public static string WorkOrderTypeRequired { get { return ResourceRead.GetResourceValue("WorkOrderTypeRequired", ResourceFileName); } }
        public static string WorkOrderStatusRequired { get { return ResourceRead.GetResourceValue("WorkOrderStatusRequired", ResourceFileName); } }
        public static string WorkOrderGuidRequired { get { return ResourceRead.GetResourceValue("WorkOrderGuidRequired", ResourceFileName); } }
        public static string ResWorkOrderTypeRequired { get { return ResourceRead.GetResourceValue("ResWorkOrderTypeRequired", ResourceFileName); } }
        public static string WorkOrderImageError { get { return ResourceRead.GetResourceValue("WorkOrderImageError", ResourceFileName); } }
        public static string WorkOrderPullItemsError { get { return ResourceRead.GetResourceValue("WorkOrderPullItemsError", ResourceFileName); } }
        public static string SameSerialExistMultiple { get { return ResourceRead.GetResourceValue("SameSerialExistMultiple", ResourceFileName); } }
        public static string ErrorWorkOrderPull { get { return ResourceRead.GetResourceValue("ErrorWorkOrderPull", ResourceFileName); } }
        public static string ValidExtensionValidation { get { return ResourceRead.GetResourceValue("ValidExtensionValidation", ResourceFileName); } }
        public static string MsgSignatureValidation { get { return ResourceRead.GetResourceValue("MsgSignatureValidation", ResourceFileName); } }
        public static string WorkOrderListHistoryHeader { get { return ResourceRead.GetResourceValue("WorkOrderListHistoryHeader", ResourceFileName); } }
        public static string WorkOrderFilesLabel { get { return ResourceRead.GetResourceValue("WorkOrderFilesLabel", ResourceFileName); } }
        public static string WorkOrderIsEDISent { get { return ResourceRead.GetResourceValue("WorkOrderIsEDISent", ResourceFileName); } }
        public static string PrintWorkOrder { get { return ResourceRead.GetResourceValue("PrintWorkOrder", ResourceFileName); } }
        public static string Closed { get { return ResourceRead.GetResourceValue("Closed", ResourceFileName); } }




    }

    public enum WorkOrderStatus
    {
        Close, Open
    }
}


