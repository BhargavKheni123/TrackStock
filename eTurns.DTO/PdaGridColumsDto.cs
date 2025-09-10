using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
namespace eTurns.DTO
{
    public class PdaGridColumsDto
    {
        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 Id { get; set; }

        public string ListName { get; set; }
        public string TableName { get; set; }
        public string GridColumnName { get; set; }
        public string GridColumnValue { get; set; }
        public bool IsFixedColumn { get; set; }
        public Int16 FixedColumnOrder { get; set; }
        public bool IsActive { get; set; }
        public bool? IsSearch { get; set; }
        public Int64? CompanyID { get; set; }
        public Int64? Room { get; set; }
        public Int64? UserID { get; set; }
        public Int64? CreatedBy { get; set; }
        public Int64? LastUpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string ListIds { get; set; }
        public string ColumnNames { get; set; }
        public string GridColumnValueWithoutAlias { get; set; }
        public string GridColumnAlias { get; set; }
        public string GridPDAColumnValue { get; set; }
    }

}
