using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class FTPMasterDTO
    {
        public long ID { get; set; }

        [Display(Name = "SFtpName", ResourceType = typeof(ResFTPMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string SFtpName { get; set; }

        [Display(Name = "ServerAddress", ResourceType = typeof(ResFTPMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string ServerAddress { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(ResFTPMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string UserName { get; set; }

        [Display(Name = "Password", ResourceType = typeof(ResFTPMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string Password { get; set; }

        [Display(Name = "Port", ResourceType = typeof(ResFTPMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public int Port { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public long? RoomId { get; set; }
        public long? CompanyId { get; set; }
        public long? EnterpriseID { get; set; }
        public long? UserID { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public Guid GUID { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime Created { get; set; }
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsArchived { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

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

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        public string EnterpriseDBName { get; set; }
        [Display(Name = "IsImportFTP", ResourceType = typeof(ResFTPMaster))]
        public bool IsImportFTP { get; set; }
        [Display(Name = "ContactEmail", ResourceType = typeof(ResFTPMaster))]
        public string ContactEmail { get; set; }

        public int? TotalRecords { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int32 HistoryID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        public System.String AddedFrom { get; set; }

        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        public System.String EditedFrom { get; set; }

    }
    public class ResFTPMaster
    {
        private static string ResourceFileName = "ResFTPMaster";

        public static string SFtpName
        {
            get
            {
                return ResourceRead.GetResourceValue("SFtpName", ResourceFileName);
            }
        }

        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }

        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }
        public static string ServerAddress
        {
            get
            {
                return ResourceRead.GetResourceValue("ServerAddress", ResourceFileName);
            }
        }
        public static string UserName
        {
            get
            {
                return ResourceRead.GetResourceValue("UserName", ResourceFileName);
            }
        }
        public static string Password
        {
            get
            {
                return ResourceRead.GetResourceValue("Password", ResourceFileName);
            }
        }

        public static string Port
        {
            get
            {
                return ResourceRead.GetResourceValue("Port", ResourceFileName);
            }
        }

        public static string IsImportFTP
        {
            get
            {
                return ResourceRead.GetResourceValue("IsImportFTP", ResourceFileName);
            }
        }

        public static string ContactEmail
        {
            get
            {
                return ResourceRead.GetResourceValue("ContactEmail", ResourceFileName);
            }

        }
    }
}