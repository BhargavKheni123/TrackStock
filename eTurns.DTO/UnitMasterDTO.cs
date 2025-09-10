using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class UnitMasterDTO
    {
        public Int64 ID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Unit", ResourceType = typeof(ResUnitMaster))]
        [AllowHtml]
        public string Unit { get; set; }

        [AllowHtml]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Description", ResourceType = typeof(ResUnitMaster))]
        public string Description { get; set; }

        //[RegularExpression("([0-9]+)")]
        //[Display(Name = "Odometer", ResourceType = typeof(ResUnitMaster))]
        //[StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //public string Odometer { get; set; }

        //[Display(Name = "OdometerUpdate", ResourceType = typeof(ResUnitMaster))]
        //public Nullable<DateTime> OdometerUpdate { get; set; }

        //[DataType(DataType.Time, ErrorMessageResourceName = "InvalidOpHours", ErrorMessageResourceType = typeof(ResUnitMaster))]
        //[Display(Name = "OpHours", ResourceType = typeof(ResUnitMaster))]

        //public Nullable<Decimal> OpHours { get; set; }

        //[Display(Name = "OpHoursUpdate", ResourceType = typeof(ResUnitMaster))]
        //public Nullable<DateTime> OpHoursUpdate { get; set; }

        ////[StringLength(4 , ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidYear", ErrorMessageResourceType = typeof(ResUnitMaster))]
        //[Display(Name = "Year", ResourceType = typeof(ResUnitMaster))]
        //public Nullable<Int64> Year { get; set; }

        //[AllowHtml]
        //[StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Display(Name = "Make", ResourceType = typeof(ResUnitMaster))]
        //public string Make { get; set; }

        //[AllowHtml]
        //[StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Display(Name = "Model", ResourceType = typeof(ResUnitMaster))]
        //public string Model { get; set; }

        //[AllowHtml]
        //[StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Display(Name = "Plate", ResourceType = typeof(ResUnitMaster))]
        //public string Plate { get; set; }

        //[Display(Name = "SerialNo", ResourceType = typeof(ResUnitMaster))]
        //public Nullable<long> SerialNo { get; set; }

        //[AllowHtml]
        //[StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Display(Name = "EngineModel", ResourceType = typeof(ResUnitMaster))]
        //public string EngineModel { get; set; }

        //[StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Display(Name = "EngineSerialNo", ResourceType = typeof(ResUnitMaster))]
        //public string EngineSerialNo { get; set; }

        //[RegularExpression("([0-9]+)", ErrorMessageResourceName = "InValidMarkupParts", ErrorMessageResourceType = typeof(ResUnitMaster))]
        //[Display(Name = "MarkupParts", ResourceType = typeof(ResUnitMaster))]
        //public Nullable<long> MarkupParts { get; set; }

        //[RegularExpression("([0-9]+)", ErrorMessageResourceName = "InValidMarkuplabour", ErrorMessageResourceType = typeof(ResUnitMaster))]
        //[Display(Name = "MarkupLabour", ResourceType = typeof(ResUnitMaster))]
        //public Nullable<long> MarkupLabour { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> Updated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> Room { get; set; }

        [Display(Name = "IsDeleted", ResourceType = typeof(ResCommon))]
        public Nullable<bool> IsDeleted { get; set; }

        [Display(Name = "IsArchived", ResourceType = typeof(ResCommon))]
        public Nullable<bool> IsArchived { get; set; }


        public Guid GUID { get; set; }


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

        [Display(Name = "UDF1", ResourceType = typeof(ResUnitMaster))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResUnitMaster))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResUnitMaster))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResUnitMaster))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResUnitMaster))]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResUnitMaster))]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResUnitMaster))]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResUnitMaster))]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResUnitMaster))]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResUnitMaster))]
        public string UDF10 { get; set; }

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

        public Guid? ItemGUID { get; set; }
        public bool isForBOM { get; set; }
        public long? RefBomId { get; set; }

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


        public int TotalRecords { get; set; }
    }

    public class ResUnitMaster
    {
        private static string resourceFile = "ResUnitMaster";

        /// <summary>
        ///   Looks up a localized string similar to Description.
        /// </summary>
        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", resourceFile);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to EngineModel.
        /// </summary>
        public static string EngineModel
        {
            get
            {
                return ResourceRead.GetResourceValue("EngineModel", resourceFile);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to EngineSerialNo.
        /// </summary>
        public static string EngineSerialNo
        {
            get
            {
                return ResourceRead.GetResourceValue("EngineSerialNo", resourceFile);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Markup parts is number.
        /// </summary>
        public static string InValidMarkupParts
        {
            get
            {
                return ResourceRead.GetResourceValue("InValidMarkupParts", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Markup labour is number.
        /// </summary>
        public static string InValidMarkuplabour
        {
            get
            {
                return ResourceRead.GetResourceValue("InValidMarkuplabour", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Invalid OP Hours!.
        /// </summary>
        public static string InvalidOpHours
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidOpHours", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Year is numeric.
        /// </summary>
        public static string InvalidYear
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidYear", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Make.
        /// </summary>
        public static string Make
        {
            get
            {
                return ResourceRead.GetResourceValue("Make", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to MarkupLabour.
        /// </summary>
        public static string MarkupLabour
        {
            get
            {
                return ResourceRead.GetResourceValue("MarkupLabour", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to MarkupParts.
        /// </summary>
        public static string MarkupParts
        {
            get
            {
                return ResourceRead.GetResourceValue("MarkupParts", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Model.
        /// </summary>
        public static string Model
        {
            get
            {
                return ResourceRead.GetResourceValue("Model", resourceFile);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Odometer.
        /// </summary>
        public static string Odometer
        {
            get
            {
                return ResourceRead.GetResourceValue("Odometer", resourceFile);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Odometer Update.
        /// </summary>
        public static string OdometerUpdate
        {
            get
            {
                return ResourceRead.GetResourceValue("OdometerUpdate", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OpHours.
        /// </summary>
        public static string OpHours
        {
            get
            {
                return ResourceRead.GetResourceValue("OpHours", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OpHours Update.
        /// </summary>
        public static string OpHoursUpdate
        {
            get
            {
                return ResourceRead.GetResourceValue("OpHoursUpdate", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Units.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Units.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Plate.
        /// </summary>
        public static string Plate
        {
            get
            {
                return ResourceRead.GetResourceValue("Plate", resourceFile);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to SerialNo.
        /// </summary>
        public static string SerialNo
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialNo", resourceFile);
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

        /// <summary>
        ///   Looks up a localized string similar to Unit.
        /// </summary>
        public static string Unit
        {
            get
            {
                return ResourceRead.GetResourceValue("Unit", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Year.
        /// </summary>
        public static string Year
        {
            get
            {
                return ResourceRead.GetResourceValue("Year", resourceFile);
            }
        }


    }
}
