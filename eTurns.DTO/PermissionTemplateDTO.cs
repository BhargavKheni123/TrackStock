using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class PermissionTemplateDTO
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        [Display(Name = "TemplateName", ResourceType = typeof(ResPermissionTemplate))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String TemplateName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(ResPermissionTemplate))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string Description { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime Updated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Int64 CreatedBy { get; set; }

        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public System.Int64 UpdatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        public bool IsDeleted { get; set; }
        public System.Int64 EnterpriseID { get; set; }
        public IEnumerable<PermissionTemplateDetailDTO> lstPermissions { get; set; }
        public IList<PermissionTemplateDetailDTO> IlstPermissions { get; set; }
        public PermissionTemplateDetailedAccessDTO PermissionTemplateDetailedAccess { get; set; }

        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }


        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int32 HistoryID { get; set; }

        public int TotalRecords { get; set; }

    }
    public class PermissionTemplateDetailedAccessDTO
    {

        public long PermissionTemplate { get; set; }

        public List<PermissionTemplateDetailDTO> PermissionList { get; set; }

        public List<PermissionTemplateDetailDTO> ModuleMasterList { get; set; }

        public List<PermissionTemplateDetailDTO> OtherModuleList { get; set; }

        public List<PermissionTemplateDetailDTO> NonModuleList { get; set; }

        public List<PermissionTemplateDetailDTO> OtherDefaultSettings { get; set; }
    }

    public class PermissionTemplateDetailDTO
    {
        public Int64 ID { get; set; }
        public Int64 PermissionTemplateID { get; set; }
        public Int64 ModuleID { get; set; }
        public string ModuleName { get; set; }
        public bool IsInsert { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsView { get; set; }
        public bool? IsModule { get; set; }
        public int? GroupId { get; set; }
        public bool IsChecked { get; set; }
        public string ModuleValue { get; set; }
        public Int64? ParentID { get; set; }
        public string ModuleURL { get; set; }
        public string ImageName { get; set; }
        public bool ShowDeleted { get; set; }
        public bool ShowArchived { get; set; }
        public bool ShowUDF { get; set; }
        public long EnterpriseID { get; set; }
        public int? DisplayOrderNumber { get; set; }
        public string DisplayOrderName { get; set; }
        public string resourcekey { get; set; }
        public bool IsModuleDeleted { get; set; }
        public string ModuleNameFromResource
        {
            get
            {
                if (!string.IsNullOrEmpty(resourcekey))
                    return ResourceRead.GetResourceValue(resourcekey, "ResModuleName");
                else
                    return ModuleName;
            }
        }
        public bool RowSelectAll { get; set; }

    }

    public class ResPermissionTemplate
    {
        private static string ResourceFileName = "ResPermissionTemplate";
        public static string TemplateName
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("TemplateName", ResourceFileName);
            }

        }
        public static string Description
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("Description", ResourceFileName);
            }

        }
        public static string PageHeader
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PageHeader", ResourceFileName);
            }
        }
        public static string PageTitle
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PageTitle", ResourceFileName);
            }
        }
    }

}
