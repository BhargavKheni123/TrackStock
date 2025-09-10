using System;

namespace eTurns.DTO
{
    public class EnterpriseDomainDTO
    {
        public long ID { get; set; }
        public long EnterpriseID { get; set; }
        public string DomainName { get; set; }
        public string DomainURL { get; set; }
        public string RedirectDomainURL { get; set; }
        public string EpUniqueKey { get; set; }
        public Guid GUID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public string EnterpriseName { get; set; }
        public bool IsSecureOnly { get; set; }


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
