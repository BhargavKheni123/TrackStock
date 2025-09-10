using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class MeasurementTermMasterDTO
    {

        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 ID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "MeasurementTerm", ResourceType = typeof(ResMeasurementTerm))]
        public string MeasurementTerm { get; set; }

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
        public Guid GUID { get; set; }
        [Display(Name = "CompanyID", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResMeasurementTerm))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResMeasurementTerm))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResMeasurementTerm))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResMeasurementTerm))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResMeasurementTerm))]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResMeasurementTerm))]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResMeasurementTerm))]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResMeasurementTerm))]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResMeasurementTerm))]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResMeasurementTerm))]
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


    }

    public class ResMeasurementTerm
    {
        private static string resourceFile = "ResMeasurementTerm";


        /// <summary>
        ///   Looks up a localized string similar to Measurement Term.
        /// </summary>
        public static string MeasurementTerm
        {
            get
            {
                return ResourceRead.GetResourceValue("MeasurementTerm", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Measurement Terms.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Measurement Terms.
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
                return ResourceRead.GetResourceValue("UDF1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF10.
        /// </summary>
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF10", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", resourceFile);
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
    }
}
