using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using eTurns.DTO.Resources;
using System.Web;

namespace eTurns.DTO
{
    public enum CalibrateRequestType
    {
        Calibrate = 1,
        CalibrateAll = 2
    }

    [Serializable]
    public class ItemLocationCalibrateRequestDTO
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

        public System.Boolean IsStep1Started { get; set; }

        public Nullable<System.DateTime> Step1StartTime { get; set; }

        public string Step1StartTimeStr { get; set; }

        public System.Boolean IsStep1Completed { get; set; }

        public Nullable<System.DateTime> Step1CompletionTime { get; set; }

        public string Step1CompletionTimeStr { get; set; }

        public System.Boolean IsStep2Started { get; set; }

        public Nullable<System.DateTime> Step2StartTime { get; set; }

        public string Step2StartTimeStr { get; set; }

        public System.Boolean IsStep2Completed { get; set; }

        public Nullable<System.DateTime> Step2CompletionTime { get; set; }

        public string Step2CompletionTimeStr { get; set; }

        public System.Boolean IsStep3Started { get; set; }

        public Nullable<System.DateTime> Step3StartTime { get; set; }

        public string Step3StartTimeStr { get; set; }

        public System.Boolean IsStep3Completed { get; set; }

        public Nullable<System.DateTime> Step3CompletionTime { get; set; }

        public string Step3CompletionTimeStr { get; set; }


        public string ErrorDescription { get; set; }

        public System.DateTime Created { get; set; }
        public string CreatedStr { get; set; }
        public System.DateTime Updated { get; set; }
        public string UpdatedStr { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }
        public string BinNumber { get; set; }
        public string ItemNumber { get; set; }

        public Int64 CreatedBy { get; set; }

        public Int64 UpdatedBy { get; set; }

        public int? TotalRecords { get; set; }
        public long? RowNum { get; set; }

        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }

        public Nullable<System.Double> CalibrationWeight { get; set; }

        public string RequestTypeName { get; set; }

    }

    public class ItemLocationCalibrateRequestRooms {
        public long RoomID { get; set; }
        public long CompanyID { get; set; }
    }

    public class ILCalibrateRequestDTO : ItemLocationCalibrateRequestDTO
    {
        public int TotalRequest { get; set; }
        public int RecordsPerPage { get; set; }

        public int NoOfPages { get; set; }
    }

    public class CalibrateNarrowSearchData
    {
        public List<NarrowSearchFieldDTO> CreatedByList { get; set; }
        public List<NarrowSearchFieldDTO> UpdatedByList { get; set; }
    }

    public class ResItemLocationCalibrateRequest
    {
        private static string resourceFile = "ResItemLocationCalibrateRequest";
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

        public static string CalibrationWeight
        {
            get
            {
                return ResourceRead.GetResourceValue("CalibrationWeight", resourceFile);
            }
        }


public static string IsStep1Started{get{ return ResourceRead.GetResourceValue("IsStep1Started", resourceFile); } }
public static string Step1StartTime{get{ return ResourceRead.GetResourceValue("Step1StartTime", resourceFile); } }
public static string IsStep1Completed{get{ return ResourceRead.GetResourceValue("IsStep1Completed", resourceFile); } }
public static string Step1CompletionTime{get{ return ResourceRead.GetResourceValue("Step1CompletionTime", resourceFile); } }
public static string IsStep2Started{get{ return ResourceRead.GetResourceValue("IsStep2Started", resourceFile); } }
public static string Step2StartTime{get{ return ResourceRead.GetResourceValue("Step2StartTime", resourceFile); } }
public static string IsStep2Completed{get{ return ResourceRead.GetResourceValue("IsStep2Completed", resourceFile); } }
public static string Step2CompletionTime{get{ return ResourceRead.GetResourceValue("Step2CompletionTime", resourceFile); } }
public static string IsStep3Started{get{ return ResourceRead.GetResourceValue("IsStep3Started", resourceFile); } }
public static string Step3StartTime{get{ return ResourceRead.GetResourceValue("Step3StartTime", resourceFile); } }
public static string IsStep3Completed{get{ return ResourceRead.GetResourceValue("IsStep3Completed", resourceFile); } }
public static string Step3CompletionTime { get { return ResourceRead.GetResourceValue("Step3CompletionTime", resourceFile); } }

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

    }
}
