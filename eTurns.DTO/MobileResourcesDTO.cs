using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class MobileResourcesDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        [Display(Name = "ResourceKey", ResourceType = typeof(ResMobileResources))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ResourceKey { get; set; }

        [Display(Name = "ResourceValue", ResourceType = typeof(ResMobileResources))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ResourceValue { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 LanguageID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ResourcePageID { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public System.Int64 CompanyID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public System.Int64 CreatedBy { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public System.Int64 UpdatedBy { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "CreatedOn", ResourceType = typeof(ResMobileResources))]
        public System.DateTime CreatedOn { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "UpdatedOn", ResourceType = typeof(ResMobileResources))]
        public System.DateTime UpdatedOn { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "Room", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> Roomid { get; set; }


    }


    public class ResMobileResources
    {
        private static string ResourceFileName = "ResMobileResources";

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
    }
}


