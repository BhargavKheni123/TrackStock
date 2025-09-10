using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using eTurns.DTO.Resources;

namespace eTurns.DTO
{
    [Serializable]
    public class BaseResourcesDTO
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ResourcePageID { get; set; }

        [Display(Name = "ResourceKey", ResourceType = typeof(ResBaseResources))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ResourceKey { get; set; }

        [Display(Name = "ResourceValue", ResourceType = typeof(ResBaseResources))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ResourceValue { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 LanguageID { get; set; }

        //[Display(Name = "Room", ResourceType = typeof(ResCommon))]
        //public Nullable<Int64> RoomID { get; set; }

        //[Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        //public System.Int64 CompanyID { get; set; }

        //Created
        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime Created { get; set; }

        //LastUpdated
        [Display(Name = "LastUpdated", ResourceType = typeof(ResBaseResources))]
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

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EditedFrom { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

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

        public bool chkvalue { get; set; }

    }

    [Serializable]
    public class BaseResourcesKeyValDTO
    {
        public string ResourceKey { get; set; }
        public string ResourceValue { get; set; }
    }

    public class ResBaseResources
    {
        private static string ResourceFileName = "ResBaseResources";

        /// <summary>
        ///   Looks up a localized string similar to MobileResources.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PageName.
        /// </summary>
        public static string PageName
        {
            get
            {
                return ResourceRead.GetResourceValue("PageName", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ResourceKey.
        /// </summary>
        public static string ResourceKey
        {
            get
            {
                return ResourceRead.GetResourceValue("ResourceKey", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to ResourceValue.
        /// </summary>
        public static string ResourceValue
        {
            get
            {
                return ResourceRead.GetResourceValue("ResourceValue", ResourceFileName);
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

        public static string LastUpdated
        {
            get
            {
                return ResourceRead.GetResourceValue("LastUpdated", ResourceFileName);
            }
        }
    }

    public enum ResourceSave
    {
        ButtonSave = 1,
        OnChange = 2,
    }

    public enum ResourceReadType
    {
        File = 1,
        Database = 2,
    }
}
