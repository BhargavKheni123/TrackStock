using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class WrittenOfCategoryDTO
    {
        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid? GUID { get; set; }

        [Display(Name = "WrittenOfCategoryID")]
        public Int64 ID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        [Display(Name = "WrittenOffCategory", ResourceType = typeof(ResWrittenOfCategory))]
        public string WrittenOffCategory { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> Updated { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> Room { get; set; }

        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<bool> IsArchived { get; set; }
        [Display(Name = "CompanyID", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }


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

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

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

        public int? TotalRecords { get; set; }
        public bool isForBOM { get; set; }

        
    }

    public class ResWrittenOfCategory
    {
        private static string resourceFile = "ResToolWrittenOffCategory";

        /// <summary>
        ///   Looks up a localized string similar to Written Off Category Name.
        /// </summary>
        public static string WrittenOffCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("WrittenOffCategoryName", resourceFile);
            }
        }
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

    }
}
