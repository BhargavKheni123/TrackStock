using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class SupplierBlanketPODetailsDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //SupplierID
        [Display(Name = "SupplierID", ResourceType = typeof(ResSupplierBlanketPODetails))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 SupplierID { get; set; }

        //BlanketPO
        [Display(Name = "BlanketPO", ResourceType = typeof(ResSupplierBlanketPODetails))]
        [StringLength(22, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String BlanketPO { get; set; }

        //StartDate
        [Display(Name = "StartDate", ResourceType = typeof(ResSupplierBlanketPODetails))]
        public Nullable<System.DateTime> StartDate { get; set; }

        //Enddate
        [Display(Name = "Enddate", ResourceType = typeof(ResSupplierBlanketPODetails))]
        public Nullable<System.DateTime> Enddate { get; set; }


        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        //Created
        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //Updated
        [Display(Name = "Updated", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

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

        public Nullable<Double> MaxLimit { get; set; }

        public bool IsNotExceed { get; set; }

        public Int32 SessionSr { get; set; }

        public Int64 ExpiryPO { get; set; }
        public Double PulledQty { get; set; }
        

        public bool IsBOM { get; set; }
        private string _StartDate;
        public string ValidStartDate
        {
            get
            {
                if (string.IsNullOrEmpty(_StartDate))
                {
                    _StartDate = FnCommon.ConvertDateByTimeZone(StartDate, false, true);
                }
                return _StartDate;
            }
            set { this._StartDate = value; }
        }

        private string _EndDate;
        public string ValidEndDate
        {
            get
            {
                if (string.IsNullOrEmpty(_EndDate))
                {
                    _EndDate = FnCommon.ConvertDateByTimeZone(Enddate, false, true);
                }
                return _EndDate;
            }
            set { this._EndDate = value; }
        }

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

        public Nullable<Double> MaxLimitQty { get; set; }

        public bool IsNotExceedQty { get; set; }
        public Nullable<Double> OrderedQty { get; set; }

        public Nullable<Double> OrderUsed { get; set; }
        public Nullable<Double> TotalOrder { get; set; }

        public string OrderRemainCost { get; set; }

    }

    public class ResSupplierBlanketPODetails
    {
        private static string ResourceFileName = "ResSupplierBlanketPODetails";

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
        ///   Looks up a localized string similar to SupplierBlanketPODetails {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to SupplierBlanketPODetails.
        /// </summary>
        public static string SupplierBlanketPODetailsHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierBlanketPODetailsHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SupplierBlanketPODetails.
        /// </summary>
        public static string SupplierBlanketPODetails
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierBlanketPODetails", ResourceFileName);
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
        ///   Looks up a localized string similar to SupplierID.
        /// </summary>
        public static string SupplierID
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to BlanketPO.
        /// </summary>
        public static string BlanketPO
        {
            get
            {
                return ResourceRead.GetResourceValue("BlanketPO", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to StartDate.
        /// </summary>
        public static string StartDate
        {
            get
            {
                return ResourceRead.GetResourceValue("StartDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Enddate.
        /// </summary>
        public static string Enddate
        {
            get
            {
                return ResourceRead.GetResourceValue("Enddate", ResourceFileName);
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
        ///   Looks up a localized string similar to Room.
        /// </summary>
        public static string Room
        {
            get
            {
                return ResourceRead.GetResourceValue("Room", ResourceFileName);
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
        ///   Looks up a localized string similar to MaxLimit.
        /// </summary>
        public static string MaxLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxLimit", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsNotExceed.
        /// </summary>
        public static string IsNotExceed
        {
            get
            {
                return ResourceRead.GetResourceValue("IsNotExceed", ResourceFileName);
            }
        }

        public static string PullUsed
        {
            get
            {
                return ResourceRead.GetResourceValue("PullUsed", ResourceFileName);
            }
        }
        public static string OrderUsed
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderUsed", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MaxLimitQty.
        /// </summary>
        public static string MaxLimitQty
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxLimitQty", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsNotExceedQty.
        /// </summary>
        public static string IsNotExceedQty
        {
            get
            {
                return ResourceRead.GetResourceValue("IsNotExceedQty", ResourceFileName);
            }
        }

        public static string OrderedUsedCost
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderedUsedCost", ResourceFileName);
            }
        }

        public static string OrderRemainCost
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderRemainCost", ResourceFileName);
            }
        }
        public static string EndDategreaterthanstartDate
        {
            get
            {
                return ResourceRead.GetResourceValue("EndDategreaterthanstartDate", ResourceFileName);
            }
        }

        public static string AddBlanketPO { get { return ResourceRead.GetResourceValue("AddBlanketPO", ResourceFileName); } }
    }
}


