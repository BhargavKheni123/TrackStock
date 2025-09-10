using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
namespace eTurns.DTO
{
    [Serializable]
    public class UserLicenceDTO
    {


        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 ID { get; set; }


        [Display(Name = "UserID", ResourceType = typeof(ResCommon))]
        public Int64 UserID { get; set; }

        [Display(Name = "LicenceAcceptDate", ResourceType = typeof(ResCommon))]
        public DateTime LicenceAcceptDate { get; set; }

        private string _licenceAcceptDate;
        public string LicenceAcceptDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_licenceAcceptDate))
                {
                    _licenceAcceptDate = FnCommon.ConvertDateByTimeZone(LicenceAcceptDate, true);
                }
                return _licenceAcceptDate;
            }
            set { this._licenceAcceptDate = value; }
        }


    }

}

