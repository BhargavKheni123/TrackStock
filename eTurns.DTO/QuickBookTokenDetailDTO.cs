using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using eTurns.DTO.Resources;
using System.Web;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class QuickBookTokenDetailDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public Int64 EnterpriseID { get; set; }
        public Int64 CompanyID { get; set; }
        public Int64 RoomID { get; set; }
        public string AccessToken { get; set; }
        public DateTime? AccessTokenExpireDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireDate { get; set; }
        public string Code { get; set; }
        public Int64 RealmCompanyID { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<DateTime> Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<DateTime> LastUpdated { get; set; }
        public Int64 CreatedBy { get; set; }
        public Int64 LastUpdatedBy { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public System.String AuthError { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
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

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        public Nullable<Boolean> IsSuccess { get; set; }

        public Nullable<Boolean> IsSuccessDisplay { get; set; }

        public string QuickBookCompanyName { get; set; }
    }


    public class ABTokenDetailDTO
    {
        public Int64 ID { get; set; }
        public string ABAppAuthCode { get; set; }
        public string AccessToken { get; set; }
        public DateTime? AccessTokenExpireDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public Int64 CreatedBy { get; set; }
        public Int64 LastUpdatedBy { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public Nullable<Boolean> IsSuccess { get; set; }
        public Nullable<Boolean> IsSuccessDisplay { get; set; }

    }

    //public class ABIntegrationCodeDTO
    //{
    //    public long ID { get; set; }
    //    public long EnterpriseID { get; set; }
    //    public long CompanyID { get; set; }
    //    public long RoomID { get; set; }
    //    public long ConsentCode { get; set; }
    //    public long IsActive { get; set; }
    //    public long Created { get; set; }
    //}
}
