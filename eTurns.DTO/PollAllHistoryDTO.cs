using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class PollAllHistoryDTO
    {
        [Display(Name = "ID", ResourceType = typeof(ResPollAllMaster))]
        public Int64 ID { get; set; }


        [Display(Name = "LastPollAllTime", ResourceType = typeof(ResPollAllMaster))]
        public DateTime LastPollAllTime { get; set; }


        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime UpdatedOn { get; set; }

        [Display(Name = "UserId", ResourceType = typeof(ResPollAllMaster))]
        public Int64 UserId { get; set; }

        public Int64 RoomID { get; set; }
        public Int64 CompanyID { get; set; }



        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string CreatedByName { get; set; }



        [Display(Name = "Remarks", ResourceType = typeof(ResPollAllMaster))]
        public string Remarks { get; set; }

        [Display(Name = "ActionType", ResourceType = typeof(ResPollAllMaster))]

        public string ActionType { get; set; }


        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }



        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        private string _CreatedOn;
        public string CreatedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_CreatedOn))
                {
                    _CreatedOn = FnCommon.ConvertDateByTimeZone(CreatedOn, true);
                }
                return _CreatedOn;
            }
            set { this._CreatedOn = value; }
        }

        private string _UpdatedOn;
        public string UpdatedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_UpdatedOn))
                {
                    _UpdatedOn = FnCommon.ConvertDateByTimeZone(UpdatedOn, true);
                }
                return _UpdatedOn;
            }
            set { this._UpdatedOn = value; }
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

    }
    public class ResPollAllMaster
    {
        private static string ResourceFileName = "ResPollAllMaster";


        public static string ID
        {
            get
            {
                return ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }
        public static string LastPollAllTime
        {
            get
            {
                return ResourceRead.GetResourceValue("LastPollAllTime", ResourceFileName);
            }
        }
        public static string UserId
        {
            get
            {
                return ResourceRead.GetResourceValue("UserId", ResourceFileName);
            }
        }
        public static string Remarks
        {
            get
            {
                return ResourceRead.GetResourceValue("Remarks", ResourceFileName);
            }
        }
        public static string ActionType
        {
            get
            {
                return ResourceRead.GetResourceValue("ActionType", ResourceFileName);
            }
        }



    }
}
