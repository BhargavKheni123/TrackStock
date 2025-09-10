using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class EulaMasterDTO
    {
        public Int64 ID { get; set; }


        [Display(Name = "EulaName")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(512, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string EulaName { get; set; }


        [Display(Name = "EulaDescription")]
        [StringLength(512, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string EulaDescription { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime UpdatedOn { get; set; }

        public Int64 CreatedBy { get; set; }
        public Int64 UpdatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }


        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }


        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }


        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        private string _CreatedOn;
        public string CreatedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_CreatedOn))
                {
                    _CreatedOn = FnCommon.ConvertDateByTimeZone(CreatedOn, true);
                }
                return _CreatedOn;
            }
            set { this._CreatedOn = value; }
        }

        private string _UpdatedOn;
        public string UpdatedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_UpdatedOn))
                {
                    _UpdatedOn = FnCommon.ConvertDateByTimeZone(UpdatedOn, true);
                }
                return _UpdatedOn;
            }
            set { this._ReceivedOnWeb = value; }
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
        public System.String AddedFrom { get; set; }

        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        public System.String EditedFrom { get; set; }
        public string EulaFileName { get; set; }
    }
    public class ResEulaMaster
    {
        private static string ResourceFileName = "ResEulaMaster";


        public static string ID
        {
            get
            {
                return ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }
        public static string EulaName
        {
            get
            {
                return ResourceRead.GetResourceValue("EulaName", ResourceFileName);
            }
        }
        public static string EulaDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("EulaDescription", ResourceFileName);
            }
        }

        public static string EulaFileName
        {
            get
            {
                return ResourceRead.GetResourceValue("EulaFileName", ResourceFileName);
            }
        }
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }
        public static string UpdatedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("UpdatedDate", ResourceFileName);
            }
        }

        public static string PageTitle { get { return ResourceRead.GetResourceValue("PageTitle", ResourceFileName); } }
        public static string EnterEulaNameAndSelectFile { get { return ResourceRead.GetResourceValue("EnterEulaNameAndSelectFile", ResourceFileName); } }
        public static string SelectPDFFileOnly { get { return ResourceRead.GetResourceValue("SelectPDFFileOnly", ResourceFileName); } }
        public static string ChooseFile { get { return ResourceRead.GetResourceValue("ChooseFile", ResourceFileName); } }


    }
}
