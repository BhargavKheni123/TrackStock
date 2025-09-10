using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class LocationMasterDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Location", ResourceType = typeof(ResLocation))]
        public string Location { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> LastUpdated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> Room { get; set; }

        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }
        [Display(Name = "CompanyID", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResLocation))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResLocation))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResLocation))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResLocation))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResLocation))]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResLocation))]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResLocation))]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResLocation))]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResLocation))]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResLocation))]
        public string UDF10 { get; set; }

        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        public int? Count { get; set; }
        public int SessionSr { get; set; }



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

        public int? TotalRecords { get; set; }
        public string Serial { get; set; }
        public string ToolName { get; set; }
        public Guid? ToolGUID { get; set; }
        [Display(Name = "IsDefault", ResourceType = typeof(ResLocation))]
        public Nullable<bool> IsDefault { get; set; }
    }

    public class ResLocation
    {
        private static string resourceFile = "ResLocation";

        /// <summary>
        ///   Looks up a localized string similar to Location.
        /// </summary>
        public static string Location
        {
            get
            {
                return ResourceRead.GetResourceValue("Location", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Location.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Location.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF10.
        /// </summary>
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF10", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF6.
        /// </summary>
        public static string UDF6
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF7.
        /// </summary>
        public static string UDF7
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF7", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF8.
        /// </summary>
        public static string UDF8
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF8", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF9.
        /// </summary>
        public static string UDF9
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF9", resourceFile);
            }
        }
        public static string IsDefault
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDefault", resourceFile);
            }
        }

        public static string ReqInventoryLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqInventoryLocation", resourceFile);
            }
        }
        public static string ReqToolLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqToolLocation", resourceFile);
            }
        }
        public static string DuplicateLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("DuplicateLocation", resourceFile);
            }
        }
        public static string SelectLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectLocation", resourceFile);
            }
        }
        public static string ToolLocationDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolLocationDeleted", resourceFile);
            }
        }
        public static string ValidateLocationDelete
        {
            get
            {
                return ResourceRead.GetResourceValue("ValidateLocationDelete", resourceFile);
            }
        }
        public static string MsgLocationNotFound
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLocationNotFound", resourceFile);
            }
        }
        public static string MsgAtleastOneLocationRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAtleastOneLocationRequired", resourceFile);
            }
        }
        public static string MsgPleaseSelectDefaultLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPleaseSelectDefaultLocation", resourceFile);
            }
        }
        public static string MsgInventoryLocationAlreadyAdded
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInventoryLocationAlreadyAdded", resourceFile);
            }
        }
        public static string MsgSelectInventoryLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectInventoryLocation", resourceFile);
            }
        }
        public static string MsgLocationQuantityValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLocationQuantityValidation", resourceFile);
            }
        }
        public static string MsgInventoryLocationDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInventoryLocationDeleted", resourceFile);
            }
        }
        public static string MsgLocationNotDelete
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLocationNotDelete", resourceFile);
            }
        }

        public static string ReqBinUDF1 { get { return ResourceRead.GetResourceValue("ReqBinUDF1", resourceFile); } }
        public static string ReqBinUDF2 { get { return ResourceRead.GetResourceValue("ReqBinUDF2", resourceFile); } }
        public static string ReqBinUDF3 { get { return ResourceRead.GetResourceValue("ReqBinUDF3", resourceFile); } }
        public static string ReqBinUDF4 { get { return ResourceRead.GetResourceValue("ReqBinUDF4", resourceFile); } }
        public static string ReqBinUDF5 { get { return ResourceRead.GetResourceValue("ReqBinUDF5", resourceFile); } }

    }

}
