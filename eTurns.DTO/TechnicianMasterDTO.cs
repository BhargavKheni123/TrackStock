using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class TechnicianMasterDTO
    {
        public Int64 ID { get; set; }

        public Guid GUID { get; set; }

        [AllowHtml]
        [Display(Name = "Technician", ResourceType = typeof(ResTechnician))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string Technician { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<DateTime> Updated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> Room { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<bool> IsArchived { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "Action", ResourceType = typeof(ResTechnician))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(ResTechnician))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResTechnician))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResTechnician))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResTechnician))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResTechnician))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResTechnician))]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResTechnician))]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResTechnician))]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResTechnician))]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResTechnician))]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResTechnician))]
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
        [Display(Name = "TechnicianCode", ResourceType = typeof(ResTechnician))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string TechnicianCode { get; set; }

        public int TotalRecords { get; set; }


    }

    public class ResTechnician
    {
        private static string ResourceFileName = "ResTechnician";

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
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string TechnicianCode
        {
            get
            {
                return ResourceRead.GetResourceValue("TechnicianCode", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Technician {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to Technicians.
        /// </summary>
        public static string TechnicainHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("TechnicainHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Technician.
        /// </summary>
        public static string Technician
        {
            get
            {
                return ResourceRead.GetResourceValue("Technician", ResourceFileName);
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

        ///   Looks up a localized string similar to eTurns: Job Types.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }
        public static string MsgKindlyFillTechnician
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgKindlyFillTechnician", ResourceFileName);
            }
        }
        public static string MsgInvalidTechnician
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidTechnician", ResourceFileName);
            }
        }
        public static string MsgInvalidTechnicianName
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidTechnicianName", ResourceFileName);
            }
        }
        public static string MsgInvalidTechniciancode
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidTechniciancode", ResourceFileName);
            }
        }
        public static string MsgTechniciancodeRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgTechniciancodeRequired", ResourceFileName);
            }
        }
        
    }
}
