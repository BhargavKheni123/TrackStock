using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO.LabelPrinting
{
    [Serializable]
    public class LabelFieldModuleTemplateDetailDTO
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        [Display(Name = "UserDefineTemplateName", ResourceType = typeof(ResLabelPrinting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String Name { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, Int64.MaxValue, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 TemplateID { get; set; }

        [Display(Name = "CaptionModuleDropdown", ResourceType = typeof(ResLabelPrinting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, Int64.MaxValue, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ModuleID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String FeildIDs { get; set; }

        [Display(Name = "IncludeBin", ResourceType = typeof(ResLabelPrinting))]
        public Boolean IncludeBin { get; set; }

        [Display(Name = "CaptionIncludeQuantity", ResourceType = typeof(ResLabelPrinting))]
        public Boolean IncludeQuantity { get; set; }

        [AllowHtml]
        [Display(Name = "CaptionQuantityField", ResourceType = typeof(ResLabelPrinting))]
        [StringLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String QuantityField { get; set; }

        [Display(Name = "BarcodeKey", ResourceType = typeof(ResLabelPrinting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, Int64.MaxValue, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 BarcodeKey { get; set; }

        [AllowHtml]
        public System.String LabelHTML { get; set; }

        [AllowHtml]
        public System.String LabelXML { get; set; }

        //FontSize
        [Display(Name = "FontSize", ResourceType = typeof(ResLabelPrinting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 72, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Double FontSize { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CompanyID { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 UpdatedBy { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime CreatedOn { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime UpdatedOn { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsArchived { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsDeleted { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "CaptionModuleDropdown", ResourceType = typeof(ResLabelPrinting))]
        public string ModuleName { get; set; }

        [Display(Name = "CaptionTemplateDropdown", ResourceType = typeof(ResLabelPrinting))]
        public string TemplateName { get; set; }

        [Display(Name = "BarcodeKey", ResourceType = typeof(ResLabelPrinting))]
        public string BarcodeKeyName { get; set; }

        [Display(Name = "CaptionSelectField", ResourceType = typeof(ResLabelPrinting))]
        public string SelectedFieldsName { get; set; }

        [Display(Name = "CaptionTextFont", ResourceType = typeof(ResLabelPrinting))]
        public string TextFont { get; set; }

        [Display(Name = "CaptionBarcodeFont", ResourceType = typeof(ResLabelPrinting))]
        public string BarcodeFont { get; set; }

        [Display(Name = "BarcodePattern", ResourceType = typeof(ResLabelPrinting))]
        public string BarcodePattern { get; set; }


        public List<LabelModuleFieldMasterDTO> lstModuleFields { get; set; }
        public List<LabelModuleFieldMasterDTO> lstBarcodeKey { get; set; }
        public List<KeyValDTO> lstQuantityFields { get; set; }
        public List<LabelModuleFieldMasterDTO> lstSelectedModuleFields { get; set; }
        public string[] arrFieldIds { get; set; }

        public bool IsSaveForEnterprise { get; set; }
        public bool IsSelectedInModuleConfig { get; set; }

        public bool IsBaseLabelEdit { get; set; }
        public string BaseLabelTemplateName { get; set; }

        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

        public int TotalRecords { get; set; }
    }

}


