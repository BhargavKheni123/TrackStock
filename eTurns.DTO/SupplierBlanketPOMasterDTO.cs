using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class SupplierBlanketPOMasterDTO
    {
        public Int64 ID { get; set; }
        public string BlanketPO { get; set; }
        public Nullable<int> HighPO { get; set; }
        public long? SupplierID { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> LastUpdated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<Int64> Room { get; set; }
        public Guid GUID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        //Added
        public string RoomName { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        [Display(Name = "CompanyID")]
        public Nullable<Int64> CompanyID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResBin))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResBin))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResBin))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResBin))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResBin))]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResBin))]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResBin))]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResBin))]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResBin))]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResBin))]
        public string UDF10 { get; set; }


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

    }
}
