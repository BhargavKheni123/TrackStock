using System;

namespace eTurns.DTO
{
    public class SolumTokenDetailDTO
    {
        public long ID { get; set; }
        public Guid GUID { get; set; }
        public long? EnterpriseID { get; set; }
        public long? CompanyID { get; set; }
        public long? RoomID { get; set; }
        public string AccessToken { get; set; }
        public DateTime? AccessTokenExpireDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireDate { get; set; }
        //public string Code { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsArchived { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public long CreatedBy { get; set; }
        public long LastUpdatedBy { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        public string AuthError { get; set; }
        public string CreatedByName { get; set; }

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

        public string RoomName { get; set; }

        public string UpdatedByName { get; set; }

        public bool? IsSuccess { get; set; }

        public bool? IsSuccessDisplay { get; set; }
        public long? UserId { get; set; }

    }
}
