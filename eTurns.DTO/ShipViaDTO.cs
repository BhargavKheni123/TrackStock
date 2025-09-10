using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class ShipViaDTO
    {
        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 ID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string ShipVia { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public DateTime? Created { get; set; }
        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public DateTime? Updated { get; set; }
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> Room { get; set; }
        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }
        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        [Display(Name = "CompanyID", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResShipVia))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResShipVia))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResShipVia))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResShipVia))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResShipVia))]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResShipVia))]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResShipVia))]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResShipVia))]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResShipVia))]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResShipVia))]
        public string UDF10 { get; set; }


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

        public int TotalRecords { get; set; }
    }

    public class ResShipVia
    {
        private static string resourceCulture = "ResShipVia";


        /// <summary>
        ///   Looks up a localized string similar to Ship-Via.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Ship-via.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ShipVia.
        /// </summary>
        public static string ShipVia
        {
            get
            {
                return ResourceRead.GetResourceValue("ShipVia", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", resourceCulture, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF10.
        /// </summary>
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF10", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", resourceCulture, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", resourceCulture, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", resourceCulture, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", resourceCulture, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF6.
        /// </summary>
        public static string UDF6
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF6", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF7.
        /// </summary>
        public static string UDF7
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF7", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF8.
        /// </summary>
        public static string UDF8
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF8", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF9.
        /// </summary>
        public static string UDF9
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF9", resourceCulture);
            }
        }
    }
}
