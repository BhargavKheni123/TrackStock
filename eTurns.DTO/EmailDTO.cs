using System;
using System.Collections.Generic;

namespace eTurns.DTO
{
    public class eMailToSendDTO
    {
        public Int64 ID { get; set; }
        public string ToAddress { get; set; }
        public string CCAddress { get; set; }
        public string BCCAddress { get; set; }
        public string Subject { get; set; }
        public string MailBody { get; set; }
        public string Remarks { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Int64? UserID { get; set; }
        public Int64? EnterpriseID { get; set; }
        public Int64? CompanyID { get; set; }
        public Int64? RoomID { get; set; }
        public int? SendingTried { get; set; }
        public List<eMailAttachmentDTO> Attachments { get; set; }
        public string ErrorDescription { get; set; }
        public string FromEmailId { get; set; }
    }

    public class eMailAttachmentDTO
    {
        public Int64 ID { get; set; }
        public Int64 eMailToSendID { get; set; }
        public string MimeType { get; set; }
        public string AttachedFileName { get; set; }
        public byte[] FileData { get; set; }
    }

    public class eMailExceptionDTO
    {
        public Int64 ID { get; set; }
        public string ToAddress { get; set; }
        public string CCAddress { get; set; }
        public string BCCAddress { get; set; }
        public string Subject { get; set; }
        public string MailBody { get; set; }
        public string ErrorMessage { get; set; }
        public string CallFrom { get; set; }
        public string Remarks { get; set; }
        public string FullException { get; set; }

        public DateTime CreatedOn { get; set; }
        public Int64 UserID { get; set; }
        public Int64 EnterpriseID { get; set; }
        public Int64 CompanyID { get; set; }
        public Int64 RoomID { get; set; }

    }


    public class eMailToSendListDTO
    {

        public Int64 eMailToSendID { get; set; }
        public Int64 emailHistoryID { get; set; }
        public string ToAddress { get; set; }
        public string CCAddress { get; set; }
        public string BCCAddress { get; set; }
        public string Subject { get; set; }
        public string MailBody { get; set; }
        public string Remarks { get; set; }
        public DateTime? CreatedOn { get; set; }

        public DateTime? HistoryCreatedOn { get; set; }

        public Int64? UserID { get; set; }
        public Int64? EnterpriseID { get; set; }
        public Int64? CompanyID { get; set; }
        public Int64? RoomID { get; set; }
        public int? SendingTried { get; set; }
        public string ErrorDescription { get; set; }

        public bool HasAttachment { get; set; }

        public Int64? attachHistoryID { get; set; }

        public string MimeType { get; set; }
        public string AttachedFileName { get; set; }
        public byte[] FileData { get; set; }

        public long TotalRecords { get; set; }

        private string _createdDate;

        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate))
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(CreatedOn, false);
                }
                return _createdDate;
            }
            set { this._createdDate = value; }
        }

        private string _HistoryDate;
        public string HistoryDate
        {
            get
            {
                if (string.IsNullOrEmpty(_HistoryDate))
                {
                    _HistoryDate = FnCommon.ConvertDateByTimeZone(HistoryCreatedOn, false);
                }
                return _HistoryDate;
            }
            set { this._HistoryDate = value; }
        }
    }

}
