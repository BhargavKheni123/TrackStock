using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class ToolsSchedulerDetailsDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //ScheduleGUID
        [Display(Name = "ScheduleGUID", ResourceType = typeof(ResToolsSchedulerDetails))]
        public Nullable<Guid> ScheduleGUID { get; set; }

        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResToolsSchedulerDetails))]
        public Nullable<Guid> ItemGUID { get; set; }

        //ItemCost
        [Display(Name = "ItemCost", ResourceType = typeof(ResToolsSchedulerDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ItemCost { get; set; }

        //Quantity
        [Display(Name = "Quantity", ResourceType = typeof(ResToolsSchedulerDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Quantity { get; set; }

        //Created
        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime Created { get; set; }

        //LastUpdated
        [Display(Name = "LastUpdated", ResourceType = typeof(ResToolsSchedulerDetails))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime LastUpdated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 LastUpdatedBy { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 Room { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CompanyID { get; set; }

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

        [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
        public string ItemNumber { get; set; }

        public System.String ItemDescription { get; set; }



        //SerialNumberTracking
        [Display(Name = "SerialNumberTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean SerialNumberTracking { get; set; }


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

    }

    public class ResToolsSchedulerDetails
    {
        private static string ResourceFileName = "ResToolsSchedulerDetails";

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
        ///   Looks up a localized string similar to ToolsSchedulerDetails {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to ToolsSchedulerDetails.
        /// </summary>
        public static string ToolsSchedulerDetailsHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolsSchedulerDetailsHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ToolsSchedulerDetails.
        /// </summary>
        public static string ToolsSchedulerDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolsSchedulerDetails", ResourceFileName);
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
        ///   Looks up a localized string similar to ScheduleID.
        /// </summary>
        public static string ScheduleID
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ScheduleGUID.
        /// </summary>
        public static string ScheduleGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleGUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemID.
        /// </summary>
        public static string ItemID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemID", ResourceFileName);
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

        /// <summary>
        ///   Looks up a localized string similar to ItemCost.
        /// </summary>
        public static string ItemCost
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemCost", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Quantity.
        /// </summary>
        public static string Quantity
        {
            get
            {
                return ResourceRead.GetResourceValue("Quantity", ResourceFileName);
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
    }
}


