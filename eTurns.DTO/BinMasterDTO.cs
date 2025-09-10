using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    [Serializable]
    public class BinMasterDTO
    {

        public Int64 ID { get; set; }

        public Guid GUID { get; set; }
        public long? ParentBinId { get; set; }

        //[RequiredLocalized("BinMaster", "ermsgMaxLength")]
        //[RequiredLocalized(ErrorMessageResourceType = typeof(BinMasterDTO), ErrorMessageResourceName = "ermsgMaxLength")]

        //[LocalizationStringLengthAttribute(5, "BinMaster", "ermsgMaxLength")]
        //[Display(Name = "Bin number")]
        //[StringLength(5, ErrorMessage = "Max length 128")]
        //[Required(ErrorMessage = "Bin number is required")]
        [Display(Name = "BinNumber", ResourceType = typeof(ResBin))]
        //[StringLength(128, ErrorMessageResourceName = "ermsgMaxLength", ErrorMessageResourceType = typeof(ResBin))]
        //[Required(ErrorMessageResourceName = "BinRequired", ErrorMessageResourceType = typeof(ResBin))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string BinNumber { get; set; }

        //[Display(Name = "Created On")]
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<DateTime> Created { get; set; }

        //[Display(Name = "Updated On")]
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<DateTime> LastUpdated { get; set; }

        //[Display(Name = "Created By")]
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        //[Display(Name = "Updated By")]
        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        //[Display(Name = "Room")]
        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> Room { get; set; }

        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }

        //Added
        //[Display(Name = "Room")]
        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public string RoomName { get; set; }

        //[Display(Name = "Created By")]
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string CreatedByName { get; set; }

        //[Display(Name = "Updated By")]
        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string UpdatedByName { get; set; }

        //[Display(Name = "CompanyID")]
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }


        [Display(Name = "UDF1", ResourceType = typeof(ResBin))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResBin))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResBin))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResBin))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResBin))]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResBin))]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResBin))]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResBin))]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResBin))]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResBin))]
        public string UDF10 { get; set; }

        public bool IsStagingLocation { get; set; }

        public bool IsStagingHeader { get; set; }
        public string CompanyName { get; set; }

        public Guid? MaterialStagingGUID { get; set; }

        //CriticalQuantity
        [Display(Name = "CriticalQuantity", ResourceType = typeof(ResBin))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CriticalQuantity { get; set; }

        //MinimumQuantity
        [Display(Name = "MinimumQuantity", ResourceType = typeof(ResBin))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> MinimumQuantity { get; set; }

        //MaximumQuantity
        [Display(Name = "MaximumQuantity", ResourceType = typeof(ResBin))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> MaximumQuantity { get; set; }

        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResBin))]
        public Nullable<Guid> ItemGUID { get; set; }

        //MinimumQuantity
        [Display(Name = "SuggestedOrderQuantity", ResourceType = typeof(ResBin))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> SuggestedOrderQuantity { get; set; }

        //IsDefault
        [Display(Name = "IsDefault", ResourceType = typeof(ResItemSupplierDetails))]
        public Nullable<Boolean> IsDefault { get; set; }

        //MinimumQuantity
        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResBin))]
        public Nullable<System.Double> ConsignedQuantity { get; set; }

        //MinimumQuantity
        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResBin))]
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        [Display(Name = "eVMISensorPort", ResourceType = typeof(ResBin))]
        public string eVMISensorPort { get; set; }

        [Display(Name = "eVMISensorID", ResourceType = typeof(ResBin))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public double? eVMISensorID { get; set; }

        [Display(Name = "ReceivedOnDate", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String AddedFrom { get; set; }


        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EditedFrom { get; set; }

        public int Count { get; set; }
        public int SessionSr { get; set; }

        public string ItemNumber { get; set; }


        public double? OnHandQuantity { get; set; }
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

        private string _updatedDate;
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

        public Nullable<System.Double> CountQuantity { get; set; }
        public Nullable<System.Double> KitMoveInOutQuantity { get; set; }


        [Display(Name = "IsEnforceDefaultReorderQuantity", ResourceType = typeof(ResBin))]
        public bool? IsEnforceDefaultReorderQuantity { get; set; }

        [Display(Name = "IsEnforceDefaultPullQuantity", ResourceType = typeof(ResBin))]
        public bool? IsEnforceDefaultPullQuantity { get; set; }

        //DefaultReorderQuantity
        [Display(Name = "DefaultReorderQuantity", ResourceType = typeof(ResBin))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[DefaultReorderQuantityCheck("MaximumQuantity", ErrorMessage = "Default Reorder quantity must be less then Maximum quantity")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> DefaultReorderQuantity { get; set; }

        //DefaultPullQuantity
        [Display(Name = "DefaultPullQuantity", ResourceType = typeof(ResBin))]
        [RegularExpression(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> DefaultPullQuantity { get; set; }

        public String callFor { get; set; }

        public int? TotalRecords { get; set; }

        public String ItemBin { get; set; }

        [Display(Name = "BinUDF1", ResourceType = typeof(ResBin))]
        public string BinUDF1 { get; set; }

        [Display(Name = "BinUDF2", ResourceType = typeof(ResBin))]
        public string BinUDF2 { get; set; }

        [Display(Name = "BinUDF3", ResourceType = typeof(ResBin))]
        public string BinUDF3 { get; set; }

        [Display(Name = "BinUDF4", ResourceType = typeof(ResBin))]
        public string BinUDF4 { get; set; }

        [Display(Name = "BinUDF5", ResourceType = typeof(ResBin))]
        public string BinUDF5 { get; set; }
    }

    [Serializable]
    public class ItemsBins
    {
        public Int64 BinID { get; set; }
        public Guid BinGuid { get; set; }
        public Int64 ItemID { get; set; }
        public Guid ItemGuid { get; set; }
        public string ItemNumber { get; set; }
        public string BinNumber { get; set; }
        public double? OnHandQuantity { get; set; }
        public double? BinQuantity { get; set; }
        public double? BinCriticalQuantity { get; set; }
        public double? BinMinimumQuantity { get; set; }
        public double? BinMaximumQuantity { get; set; }

    }

    public class ResBin
    {
        private static string ResourceFileName = "ResBin";

        /// <summary>
        ///   Looks up a localized string similar to Bin  Number.
        /// </summary>
        public static string BinNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("BinNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF10.
        /// </summary>
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF10", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF6.
        /// </summary>
        public static string UDF6
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF6", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF7.
        /// </summary>
        public static string UDF7
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF7", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF8.
        /// </summary>
        public static string UDF8
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF8", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF9.
        /// </summary>
        public static string UDF9
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF9", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Job Types.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Job Types.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }

        public static string ConsignedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignedQuantity", ResourceFileName);
            }
        }
        public static string RequisitionedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionedQuantity", ResourceFileName);
            }
        }

        public static string RequestedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestedQuantity", ResourceFileName);
            }
        }

        public static string CustomerOwnedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerOwnedQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MinimumQuantity.
        /// </summary>
        public static string eVMISensorPort
        {
            get
            {
                return ResourceRead.GetResourceValue("eVMISensorPort", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MinimumQuantity.
        /// </summary>
        public static string eVMISensorID
        {
            get
            {
                return ResourceRead.GetResourceValue("eVMISensorID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CriticalQuantity.
        /// </summary>
        public static string CriticalQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CriticalQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MinimumQuantity.
        /// </summary>
        public static string MinimumQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MinimumQuantity", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to SuggestedOrderQuantity.
        /// </summary>
        public static string SuggestedOrderQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedOrderQuantity", ResourceFileName);
            }
        }
        public static string SuggestedTransferQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedTransferQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MaximumQuantity.
        /// </summary>
        public static string MaximumQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MaximumQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemGUID.
        /// </summary>
        public static string ItemGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemGUID", ResourceFileName);
            }
        }
        public static string IsDefault
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDefault", ResourceFileName);
            }
        }

        //IsStagingLocation
        public static string IsStagingLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("IsStagingLocation", ResourceFileName);
            }
        }

        public static string IsEnforceDefaultPullQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("IsEnforceDefaultPullQuantity", ResourceFileName);
            }
        }

        public static string IsEnforceDefaultReorderQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("IsEnforceDefaultReorderQuantity", ResourceFileName);
            }
        }

        public static string DefaultPullQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultPullQuantity", ResourceFileName);
            }
        }

        public static string DefaultReorderQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultReorderQuantity", ResourceFileName);
            }
        }
        public static string CannotDeleteDefaultBin
        {
            get
            {
                return ResourceRead.GetResourceValue("CannotDeleteDefaultBin", ResourceFileName);
            }
        }
        public static string MoreLocations
        {
            get
            {
                return ResourceRead.GetResourceValue("MoreLocations", ResourceFileName);
            }
        }

        public static string BinHaveQtySoCantDelete
        {
            get
            {
                return ResourceRead.GetResourceValue("BinHaveQtySoCantDelete", ResourceFileName);
            }
        }
        public static string BinNotAllowEmpty
        {
            get
            {
                return ResourceRead.GetResourceValue("BinNotAllowEmpty", ResourceFileName);
            }
        }
        public static string MsgEnterValidBinName
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterValidBinName", ResourceFileName);
            }
        }
    }

    public class ResBinUDF
    {
        private static string ResourceFileName = "ResBinUDF";
        public static string BinUDF1 { get { return ResourceRead.GetResourceValue("UDF1", ResourceFileName, true); } }
        public static string BinUDF2 { get { return ResourceRead.GetResourceValue("UDF2", ResourceFileName, true); } }
        public static string BinUDF3 { get { return ResourceRead.GetResourceValue("UDF3", ResourceFileName, true); } }
        public static string BinUDF4 { get { return ResourceRead.GetResourceValue("UDF4", ResourceFileName, true); } }
        public static string BinUDF5 { get { return ResourceRead.GetResourceValue("UDF5", ResourceFileName, true); } }
        public static string BinUDFSetup { get { return ResourceRead.GetResourceValue("BinUDFSetup", ResourceFileName); } }
    }
    public class BinForDefaultCheckDTO
    {
        public string ItemNumber { get; set; }
        public string BinNumber { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDefault { get; set; }
        public bool IsStagingLocation { get; set; }
    }
}
