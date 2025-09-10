using System;

namespace eTurns.DTO
{
    public class EnterpriseQLDetailDTO
    {
        public string Name { get; set; }
        public string QLDetailNumber { get; set; }
        public double Quantity { get; set; }
        public Guid GUID { get; set; }
        public long ID { get; set; }
        public Guid QLMasterGUID { get; set; }
        public int SessionSr { get; set; }
        public DateTime? Created { get; set; }
    }

    public class EnterpriseQLExportDTO
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public long EnterpriseId { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }

        public long? CreatedBy { get; set; }

        public long? LastUpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public bool? IsArchived { get; set; }

        public Guid GUID { get; set; }


        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }

        public DateTime? ReceivedOn { get; set; }

        public DateTime? ReceivedOnWeb { get; set; }

        public string AddedFrom { get; set; }

        public string EditedFrom { get; set; }

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

        private string _ReceivedOn;
        public string ReceivedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOn))
                {
                    _ReceivedOn = FnCommon.ConvertDateByTimeZone(ReceivedOn, true);
                }
                return _ReceivedOn;
            }
            set { this._ReceivedOn = value; }
        }

        private string _ReceivedOnWeb;
        public string ReceivedOnDateWeb
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOnWeb))
                {
                    _ReceivedOnWeb = FnCommon.ConvertDateByTimeZone(ReceivedOnWeb, true);
                }
                return _ReceivedOnWeb;
            }
            set { this._ReceivedOnWeb = value; }
        }

        public string QLDetailNumber { get; set; }
        public double Quantity { get; set; }
    }    

    public class EnterpriseQLDetailTbl
    {
        public long ID { get; set; }
        public string QLDetailNumber { get; set; }
        public double Quantity { get; set; }
    }

}
