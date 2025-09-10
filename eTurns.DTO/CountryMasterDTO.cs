using System;

namespace eTurns.DTO
{
    public class CountryMasterDTO
    {

        public Int64 ID { get; set; }
        public Guid GUID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        //[Display(Name = "Customer", ResourceType = typeof(ResCustomer))]
        //[StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNunberFormat { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }

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
    }
}
