using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class ValidationRulesDTO : ValidationRulesMasterDTO
    {
        public ValidationRulesDTO() {
            this.IsMasterDTO = false;
        }

        public ValidationRulesDTO(ValidationRulesMasterDTO masterDTO, long enterpriseID, long companyID, long roomID)
        {
            //this.ID = 0;//masterDTO.ID
            this.DTOName = masterDTO.DTOName;
            this.DTOProperty = masterDTO.DTOProperty;
            this.IsRequired = masterDTO.IsRequired;
            this.ValidationModuleID = masterDTO.ValidationModuleID;
            this.DisplayOrder = masterDTO.DisplayOrder;
            //this.Created = masterDTO.Created;
            this.EnterpriseID = enterpriseID;
            this.CompanyID = companyID;
            this.RoomID = roomID;
            //this.CreatedBy 
            //this.Updated 
            //this.LastUpdatedBy 
            this.IsMasterDTO = true;
            this.IsRequiredDefault = masterDTO.IsRequired;
            this.ResourceFileName = masterDTO.ResourceFileName;
            this.ModulePage = masterDTO.ModulePage;
        }

        /// <summary>
        /// Is master record or default record
        /// </summary>
        public bool IsMasterDTO { get; set; }
        public long EnterpriseID { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public long? LastUpdatedBy { get; set; }

        /// <summary>
        /// To be get from resource
        /// </summary>
        public string ColumnResName { get; set; }

        /// <summary>
        /// Default or Master Rule for Required Validation
        /// </summary>
        public bool IsRequiredDefault { get; set; }

    }
}
