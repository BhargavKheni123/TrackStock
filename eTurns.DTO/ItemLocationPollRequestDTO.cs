using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using eTurns.DTO.Resources;
using System.Web;

namespace eTurns.DTO
{
    public enum PollRequestType
    {
        Poll = 1,
        PollAll = 2,
        TimePoll = 3,
        SchedulePoll = 4,
        PollImmediate = 5
    }

    [Serializable]
    public class ItemLocationPollRequestDTO
    {
        public System.Int64 ID { get; set; }
        public Guid ItemGUID { get; set; }
        public System.Int64 BinID { get; set; }
        public System.Int32 ScaleID { get; set; }

        public System.Int32 ChannelID { get; set; }

        public System.Int64 RoomID { get; set; }
        public System.Int64 CompanyID { get; set; }

        public System.Int64 ComPortMasterID { get; set; }

        public string ComPortName { get; set; }

        public System.Int32 RequestType { get; set; }

        public System.Boolean IsPollStarted { get; set; }

        public Nullable<System.DateTime> PollStartTime { get; set; }

        public System.Boolean IsPollCompleted { get; set; }

        public Nullable<System.DateTime> PollCompletionTime { get; set; }

        public System.Double WeightReading { get; set; }

        public string ErrorDescription { get; set; }

        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }

        public System.Boolean IsPostProcessDone { get; set; }

        public string RoomName { get; set; }
        public string ItemNumber { get; set; }

        public double? WeightPerPiece { get; set; }

        public string CompanyName { get; set; }

        public string BinNumber { get; set; }

        public Int64 CreatedBy { get; set; }

        public Int64 UpdatedBy { get; set; }

        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }

        public int? TotalRecords { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }
        public Guid BinGUID { get; set; }

        public string RequestTypes { get; set; }

        public string ErrorType { get; set; }

        public EVMIPollErrorTypeEnum ErrorTypeEnum
        {
            get
            {
                EVMIPollErrorTypeEnum errorTypeEnum = EVMIPollErrorTypeEnum.None;

                if (string.IsNullOrWhiteSpace(ErrorType) == false)
                {
                    bool b = Enum.TryParse<EVMIPollErrorTypeEnum>(ErrorType, out errorTypeEnum);
                }

                return errorTypeEnum;
            }
        }

        public double? WeightVariance { get; set; }

        public string CommandText { get; set; }
        public Guid? CountGUID { get; set; }
    }

    public class ResItemLocationPollRequest
    {
        private static string resourceFile = "ResItemLocationPollRequest";
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        public static string ScaleID
        {
            get
            {
                return ResourceRead.GetResourceValue("ScaleID", resourceFile);
            }
        }

        public static string ChannelID
        {
            get
            {
                return ResourceRead.GetResourceValue("ChannelID", resourceFile);
            }
        }

        public static string ComPort
        {
            get
            {
                return ResourceRead.GetResourceValue("ComPort", resourceFile);
            }
        }

        public static string IsPollStarted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsPollStarted", resourceFile);
            }
        }

        public static string IsPollCompleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsPollCompleted", resourceFile);
            }
        }

        public static string IsPostProcessDone
        {
            get
            {
                return ResourceRead.GetResourceValue("IsPostProcessDone", resourceFile);
            }
        }

        public static string PollStartTime
        {
            get
            {
                return ResourceRead.GetResourceValue("PollStartTime", resourceFile);
            }
        }

        public static string PollCompletionTime
        {
            get
            {
                return ResourceRead.GetResourceValue("PollCompletionTime", resourceFile);
            }
        }


        public static string WeightReading
        {
            get
            {
                return ResourceRead.GetResourceValue("WeightReading", resourceFile);
            }
        }

        public static string ErrorDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorDescription", resourceFile);
            }
        }

        public static string RequestType
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestType", resourceFile);
            }
        }

        public static string CommandText
        {
            get
            {
                return ResourceRead.GetResourceValue("CommandText", resourceFile);
            }
        }



    }


    public class ILPollRequestDTO : ItemLocationPollRequestDTO
    {
        public int TotalRequest { get; set; }
        public int RecordsPerPage { get; set; }

        public int NoOfPages { get; set; }
    }

    public class ItemLocationPollRequestRooms
    {
        public long RoomID { get; set; }
        public long CompanyID { get; set; }
    }
}
