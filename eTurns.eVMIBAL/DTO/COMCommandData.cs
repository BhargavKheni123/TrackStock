using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.eVMIBAL.DTO
{
    public class COMCommandData
    {
        public COMCommandData(string commandASCII)
        {
            this.CommandASCII = commandASCII;
        }
                

        public COMCommandData(string commandASCII, int? scaleID, int? channelID = null, string commandASCII2 = null)
        {
            this.CommandASCII = commandASCII;
            this.ScaleID = scaleID;
            this.ChannelID = channelID;
            this.CommandASCII2 = commandASCII2;
        }

        public string CommandASCII { get; set; }
        public string CommandASCII2 { get; set; }
        public int? ScaleID { get; set; }

        public int? ChannelID { get; set; }

        public double? CalibrationWeight { get; set; }

        public string ModelNo { get; set; }

        public string HEXRequest { get; set; }
        public string HEXResponse { get; set; }

        public string ResponseErrorCode { get; set; }

        public string ResponseError
        {
            get
            {
                return GetResponseError(ResponseErrorCode);
            }
        }

        public string ExceptionMsg { get; set; }
        public string ValidationMsg { get; set; }

        public bool IsError
        {
            get
            {
                bool b = false;
                if (!string.IsNullOrWhiteSpace(ResponseErrorCode))
                {
                    b = true;
                }
                else if (string.IsNullOrWhiteSpace(ValidationMsg) == false)
                {
                    b = true;
                }
                else if (string.IsNullOrWhiteSpace(ExceptionMsg) == false)
                {
                    b = true;
                }

                return b;
            }
        }

        public string ErrorInfo
        {
            get
            {
                string msg = "";
                if (!string.IsNullOrWhiteSpace(ResponseErrorCode))
                {
                    msg += "COM Port Response Error: [Code = " + ResponseErrorCode + "] " + ResponseError;
                }
                else if (string.IsNullOrWhiteSpace(ValidationMsg) == false)
                {
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        msg += Environment.NewLine;
                    }

                    msg += "COM Port Error: " + ValidationMsg;
                }
                else if (string.IsNullOrWhiteSpace(ExceptionMsg) == false)
                {
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        msg += Environment.NewLine;
                    }

                    msg += "COM Port Error: " + ExceptionMsg;
                }

                return msg;
            }
        }

        public string ResponseData { get; set; }

        public static string GetResponseError(string responseErrorCode)
        {
            string error = "";
            switch (responseErrorCode)
            {
                case "1":
                    error = "Load cell error. Load cell data is not correct.";
                    break;
                case "2":
                    error = "Calibrate data empty. Channel is not calibrated. Please calibrate the weight pad on the channel.";
                    break;
                case "3":
                    error = "In motion. Pad is in motion status, can’t get stable weight. Wait for stable pad.";
                    break;
                case "4":
                    error = "Scale model not set. Scale model has not been set. Set the scale Model. For example 'A60008'.";
                    break;
                case "5":
                    error = "Scale channel number error.";
                    break;
                case "6":
                    error = "Command error. Command is not correct, check syntax.";
                    break;
                case "7":
                    error = "Eprom rd/wr error. ";
                    break;
                case "8":
                    error = "Error calibration weight.";
                    break;
                case "10":
                    error = "Pad disabled, can‘t weight. Scale is not in Padmode, but a weight has been requested.";
                    break;
                case "11":
                    error = "Shelf mode can’t run command of pad. Scale is set in Shelf mode. Padmode setting commands cannot be used.";
                    break;
                case "12":
                    error = "Pad mode can’t run command of shelf. Scale is set in Padmode. Shelf mode setting commands cannot be used.";
                    break;
                case "PW":
                    error = "Scale is in power up mode, please wait several seconds to send command.";
                    break;
                default:
                    error = "Some error in command response";
                    break;
            }
            return error;
        }

        public string CommandText { get; set; }
    }

    public class COMCmdResponse
    {
        public COMCommandData CommandData { get; set; }

        public bool IsSuccess
        {
            get
            {
                return !CommandData.IsError;
            }
        }
        public static COMCmdResponse CreateResponse(COMCommandData commandData)
        {
            COMCmdResponse response = new COMCmdResponse();
            response.CommandData = commandData;
            return response;
        }
    }

    public class COMCmdResponse<T> : COMCmdResponse
    {
        public T data { get; set; }

        public static COMCmdResponse<T> CreateResponse(T data, COMCommandData commandData)
        {
            COMCmdResponse<T> response = new COMCmdResponse<T>();
            response.data = data;
            response.CommandData = commandData;
            return response;
        }

    }

    public class ComboboxItem
    {
        public string Text { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
