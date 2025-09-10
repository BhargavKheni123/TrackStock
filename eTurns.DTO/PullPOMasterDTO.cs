using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class PullPOMasterDTO
    {
        //ID
        [Display(Name = "ID", ResourceType = typeof(ResPullPOMaster))]
        public System.Int64 ID { get; set; }

        [Display(Name = "PullOrderNumber", ResourceType = typeof(ResPullPOMaster))]
        public System.String PullOrderNumber { get; set; }

        [Display(Name = "IsActive", ResourceType = typeof(ResPullPOMaster))]
        public bool IsActive { get; set; }


        //Created
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //Updated
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        //LastUpdatedBy
        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> UpdatedBy { get; set; }

        //Room
        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> RoomId { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public System.DateTime? ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime? ReceivedOnWeb { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        private string _Created;
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_Created))
                {
                    _Created = FnCommon.ConvertDateByTimeZone(Created, true);
                }
                return _Created;
            }
            set { this._Created = value; }
        }
        private string _Updated;
        public string UpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_Updated))
                {
                    _Updated = FnCommon.ConvertDateByTimeZone(Updated, true);
                }
                return _Updated;
            }
            set { this._Updated = value; }
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
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EditedFrom { get; set; }

        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public string RoomName { get; set; }

        public int? TotalRecords { get; set; }
    }

    public class ResPullPOMaster
    {
        private static string ResourceFileName = "ResPullPOMaster";

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>

        public static string ID
        {
            get
            {
                return ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }
        public static string PullOrderNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("PullOrderNumber", ResourceFileName);
            }
        }
        public static string IsActive
        {
            get
            {
                return ResourceRead.GetResourceValue("IsActive", ResourceFileName);
            }
        }
        public static string PullPOMasterHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PullPOMasterHeader", ResourceFileName);
            }
        }

    }
}


