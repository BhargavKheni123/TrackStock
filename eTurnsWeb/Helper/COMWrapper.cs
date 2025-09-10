using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualBasic;
using System.IO.Ports;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace eTurnsWeb.Helper
{
    public class COMWrapper : IDisposable
    {

        #region Declaration or Types

        private const short CMD_HEAD = 0xF2;
        private const short CMD_END = 0xF3;
        private bool disposedValue;
        private bool IsComPortSimulation = false;
        

        public enum TransmissionType { Text, Hex }

        public string PortName { get; set; }

        public class ShelfCommands
        {
            /// <summary>
            /// Get Weight Command
            /// </summary>
            public static string GetWeightCMD { get { return "W"; } }

            /// <summary>
            /// Zero Weight Command
            /// </summary>
            public static string ZeroWeightCMD { get { return "Z"; } }

            /// <summary>
            /// Reset Scale
            /// </summary>
            public static string ResetScaleCMD { get { return "r"; } }

            /// <summary>
            /// Firmware Version
            /// </summary>
            public static string FirmwareVersion { get { return "V"; } }

            /// <summary>
            /// Get Scale ID
            /// </summary>
            public static string GetScaleID { get { return "A"; } }

            /// <summary>
            /// Set Scale ID
            /// </summary>
            public static string SetScaleID { get { return "S"; } }

            /// <summary>
            /// Get Predefined Model Number
            /// </summary>
            public static string GetModelNumber { get { return "Q"; } }

            /// <summary>
            /// Get Channel Count
            /// </summary>
            public static string GetChannelCount1 { get { return "1"; } }

            /// <summary>
            /// Get Channel Count 2
            /// </summary>
            public static string GetChannelCount2 { get { return "4"; } }

            /// <summary>
            /// Request predefined model Calibration WT
            /// </summary>
            public static string GetCalibrationWeight { get { return "O"; } }

            /// <summary>
            /// Set Calibration WT when scale is in predefined model 
            /// </summary>
            public static string SetCalibrationWeight { get { return "B"; } }

            public static string CalibrationStep1 { get { return "C"; } }
            public static string CalibrationStep2 { get { return "E"; } }
            public static string CalibrationStep3 { get { return "F"; } }
        }

        #endregion

        #region Constructors
        public COMWrapper(string portName)
        {
            this.PortName = portName;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get com ports with Eltima maufacturer
        /// </summary>
        /// <returns></returns>
        public static List<ComboboxItem> GetEltimaPorts()
        {

            //List<string> portList = SerialPort.GetPortNames()
            //    .OrderBy(port => Convert.ToInt32(port.Replace("COM", string.Empty))).ToList();


            //foreach (string port in portList)
            //{
            //    SerialPort comPort = new SerialPort(port);
            //    cmbComPort.Items.Add(port);
            //}

            // Get all serial (COM)-ports you can see in the devicemanager
            List<ComboboxItem> comboboxItems = new List<ComboboxItem>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\cimv2",
                "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");


            // Add all available (COM)-ports to the combobox
            foreach (ManagementObject queryObj in searcher.Get())
            {
                string caption = Convert.ToString(queryObj["Caption"]);
                string manufacturer = Convert.ToString(queryObj["Manufacturer"]);
                string name = Convert.ToString(queryObj["Name"]);

                if (manufacturer.ToLower().Contains("eltima") && caption.Contains("(COM"))
                {
                    int indexOfCom = caption.IndexOf("(COM");
                    string comName = caption.Substring(indexOfCom + 1, caption.Length - indexOfCom - 2);
                    comboboxItems.Add(new ComboboxItem() { Text = caption, Value = comName });
                }
            }

            return comboboxItems;
        }

        /// <summary>
        /// Get ScaleID or ShelfID
        /// </summary>
        /// <returns></returns>
        public COMCmdResponse<int?> GetScaleID()
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.GetScaleID);
            string data = "";

            //if (AppSettings.IsComPortSimulation == false)
            if (IsComPortSimulation == false)
            {
                data = ExecuteCommand(ref commandData);
            }
            else
            {
                Random r = new Random();
                data = Convert.ToString(r.Next(1, 999));
            }

            if (commandData.IsError == false)
            {
                //return Convert.ToInt32(data);
                return COMCmdResponse<int?>.CreateResponse(Convert.ToInt32(data), commandData);
            }
            //return null;
            return COMCmdResponse<int?>.CreateResponse(null, commandData);
        }

        /// <summary>
        /// Set ScaleID or ShelfID
        /// </summary>
        /// <param name="scaleID"></param>
        /// <returns></returns>
        public COMCmdResponse SetScaleID(int scaleID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.SetScaleID, scaleID, null);

            string data = "";

            if (scaleID < 1 || scaleID > 999)
            {
                throw new Exception("Shelf ID must be between 1 and 999");
            }

            //if (AppSettings.IsComPortSimulation == false)
            if (IsComPortSimulation == false)
            {
                data = ExecuteCommand(ref commandData);
            }
            else
            {
                data = scaleID.ToString();
            }

            if (commandData.IsError == false)
            {
                bool b = Convert.ToInt32(data) == scaleID;
                return COMCmdResponse<bool>.CreateResponse(b, commandData);
            }

            return COMCmdResponse.CreateResponse(commandData);
        }

        public COMCmdResponse<string> GetFirmwareVersion(int scaleID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.FirmwareVersion, scaleID, null);
            string data = ExecuteCommand(ref commandData);
            return COMCmdResponse<string>.CreateResponse(data, commandData);
        }

        public COMCmdResponse<decimal?> GetWeight(int scaleID, int channelID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.GetWeightCMD, scaleID, channelID);
            string data = string.Empty;

            //f (AppSettings.IsComPortSimulation == false)
            if(IsComPortSimulation == false)
            {
                data = ExecuteCommand(ref commandData);
            }
            else
            {
                Random r = new Random();
                data = Convert.ToString(r.Next(-20, 50) + r.NextDouble());
            }

            if (commandData.IsError == false)
            {
                return COMCmdResponse<decimal?>.CreateResponse(Convert.ToDecimal(data), commandData);
            }

            return COMCmdResponse<decimal?>.CreateResponse(null, commandData);
        }

        public COMCmdResponse ZeroWeight(int scaleID, int channelID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.ZeroWeightCMD, scaleID, channelID);
            bool b = true;

            //if (AppSettings.IsComPortSimulation == false)
            if (IsComPortSimulation == false)
            {
                string data = ExecuteCommand(ref commandData);
                b = data == ShelfCommands.ZeroWeightCMD;
            }

            return COMCmdResponse.CreateResponse(commandData);
        }

        public COMCmdResponse<string> GetModelNumber(int scaleID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.GetModelNumber, scaleID, null);
            string data = ExecuteCommand(ref commandData);
            return COMCmdResponse<string>.CreateResponse(data, commandData);
        }

        public COMCmdResponse<string> GetChannelCount(int scaleID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.GetChannelCount1, scaleID, null, ShelfCommands.GetChannelCount2);
            string data = ExecuteCommand(ref commandData);
            return COMCmdResponse<string>.CreateResponse(data, commandData);
        }

        /// <summary>
        /// Get Calibration Weight
        /// </summary>
        /// <param name="scaleID"></param>
        /// <returns></returns>
        public COMCmdResponse<decimal?> GetCalibrationWeight(int scaleID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.GetCalibrationWeight, scaleID, null);
            string data = ExecuteCommand(ref commandData);


            if (commandData.IsError == false)
            {
                return COMCmdResponse<decimal?>.CreateResponse(Convert.ToDecimal(data), commandData);
            }

            return COMCmdResponse<decimal?>.CreateResponse(null, commandData);
        }

        /// <summary>
        /// Calibration Step1
        /// </summary>
        /// <param name="scaleID"></param>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public COMCmdResponse<bool> CalibrationStep1(int scaleID, int channelID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.CalibrationStep1, scaleID, channelID);
            string data = ExecuteCommand(ref commandData);

            bool isSuccess = data == "U";

            if (commandData.IsError == false)
            {
                return COMCmdResponse<bool>.CreateResponse(isSuccess, commandData);
            }

            return COMCmdResponse<bool>.CreateResponse(isSuccess, commandData);
        }

        /// <summary>
        /// Calibration Step2
        /// </summary>
        /// <param name="scaleID"></param>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public COMCmdResponse<bool> CalibrationStep2(int scaleID, int channelID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.CalibrationStep2, scaleID, channelID);
            string data = ExecuteCommand(ref commandData);
            bool isSuccess = data == "F";
            if (commandData.IsError == false)
            {
                return COMCmdResponse<bool>.CreateResponse(isSuccess, commandData);
            }

            return COMCmdResponse<bool>.CreateResponse(isSuccess, commandData);
        }

        /// <summary>
        /// Calibration Step3
        /// </summary>
        /// <param name="scaleID"></param>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public COMCmdResponse<bool> CalibrationStep3(int scaleID, int channelID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.CalibrationStep3, scaleID, channelID);
            string data = ExecuteCommand(ref commandData);
            bool isSuccess = data == "C";
            if (commandData.IsError == false)
            {
                return COMCmdResponse<bool>.CreateResponse(isSuccess, commandData);
            }

            return COMCmdResponse<bool>.CreateResponse(isSuccess, commandData);
        }


        #endregion

        #region Private Methods

        private string ExecuteCommand(ref COMCommandData commandData)
        {
            using (SerialPort comPort = GetSerialPort())
            {
                string command = BuildCommand(commandData.CommandASCII, commandData.ScaleID.ToString(), commandData.ChannelID.ToString(), commandData.CommandASCII2);
                commandData.HEXRequest = command;
                SendSerial(command, comPort);

                string data = ReadComResponse(comPort, commandData.CommandASCII, ref commandData);
                comPort.Close();

                return data;

            }
        }

        private SerialPort GetSerialPort()
        {
            SerialPort comPort = new SerialPort(this.PortName);

            //int BaudRate = comPort.BaudRate;
            //Parity parity = comPort.Parity;
            //int dataBits = comPort.DataBits;
            //StopBits stopBits = comPort.StopBits;

            // Set below as per reference docs
            comPort.BaudRate = 9600;
            comPort.DataBits = 8;
            comPort.Parity = Parity.None;
            comPort.StopBits = StopBits.One;

            comPort.ReadTimeout = 1000;
            return comPort;
        }

        private string BuildCommand(string commandASCII1, string pScaleID, string channelID, string commandASCII2 = null)
        {

            string scaleID = pScaleID == "" ? "" : pScaleID.PadLeft(4, '0');
            string sCommand = commandASCII1.Trim() + scaleID.Trim() + channelID.Trim();


            if (!string.IsNullOrWhiteSpace(commandASCII2))
            {
                sCommand = sCommand + commandASCII2;

            }

            string commandHex = sCommand.ToHex();

            int nLength = sCommand.Length + 1 + 1;

            if (!string.IsNullOrWhiteSpace(commandASCII2))
            {
                //nLength = nLength + 3;
            }

            int nChecksum = 0;

            nChecksum = Checksum(Convert.ToChar(nLength).ToString() + sCommand);
            string checksumHex = nChecksum.ToString("X2").Replace("0x", "");
            string lenHex = nLength.ToString("X2").Replace("0x", "");
            string sReturn = CMD_HEAD.ToString("X2") +
                lenHex +
                commandHex +
                checksumHex +
                CMD_END.ToString("X2");

            return sReturn;
        }

        private bool SendSerial(string command, SerialPort comPort)
        {
            WriteData(command, comPort);
            return true;
        }

        private void WriteData(string msg, SerialPort comPort)
        {

            if (!(comPort.IsOpen == true))
            {
                comPort.Open();
            }

            //convert the message to byte array
            byte[] newMsg = msg.ToByte();

            //send the message to the port
            comPort.Write(newMsg, 0, newMsg.Length);


        }

        private string ReadComResponse(SerialPort comPort, string sCommandByte, ref COMCommandData commandData)
        {

            //string sResponse = string.Empty;
            var received = "";
            bool isReading = true;
            int i = 0;
            int maxLoop = 25;

            while (isReading == true)
            {
                try
                {
                    //////LogHelper.CreateLog("ExecuteCommand - ", " Before Thread.Sleep");F2085A303030333766F3
                    Thread.Sleep(100);
                    //////LogHelper.CreateLog("ExecuteCommand - ", " Before comPort.BytesToRead");
                    int bytes = comPort.BytesToRead;

                    if (bytes > 0)
                    {

                    }

                    //create a byte array to hold the awaiting data
                    byte[] comBuffer = new byte[bytes];
                    //read the data and store it
                    int bytesRead = comPort.Read(comBuffer, 0, bytes);

                    received += comBuffer.ToHex(); //ByteToHex(comBuffer); //comPort.ReadExisting();

                    if (received.Contains(CMD_END.ToString("X2")))
                    {
                        isReading = false;
                    }

                    i++;

                    if (i >= maxLoop)
                    {
                        isReading = false;

                        //if (bytes == 0)
                        {
                            commandData.ValidationMsg += "Response Data not received. ";
                        }
                    }
                }
                catch (Exception ex)
                {
                    commandData.ExceptionMsg = ex.Message;
                    //throw ex;
                    ////LogHelper.CreateLog("ExecuteCommand - ", " while loop isReading ex : "+ ex.Message + " " + ex.InnerException);
                }
            }
            ////LogHelper.CreateLog("ExecuteCommand - line 1067: ", " received " + received);
            string[] sArray = received.Trim().Split(' ');
            string sReturn = "";
            foreach (String hex in sArray)
            {
                if (!string.IsNullOrEmpty(hex))
                {
                    // Convert the number expressed in base-16 to an integer. 
                    int value = Convert.ToInt32(hex, 16);
                    // Get the character corresponding to the integral value. 
                    string stringValue = Char.ConvertFromUtf32(value);
                    char charValue = (char)value;
                    //Console.WriteLine("hexadecimal value = {0}, int value = {1}, char value = {2} or {3}",
                    //                   hex, value, stringValue, charValue);
                    sReturn += stringValue;
                }
            }

            commandData.HEXResponse = sReturn.ToHex();

            //sResponse = sReturn;
            //string sData = sResponse;
            ////LogHelper.CreateLog("ExecuteCommand -: ", " after sResponse " + sResponse);
            if (!Validate(sReturn, ref commandData))
            {
                ////LogHelper.CreateLog("ExecuteCommand -: ", "False Validate m_sErrorMessage: " + sData);

            }
            else
            {
                ////LogHelper.CreateLog("ExecuteCommand -: ", "False ExtractData m_sErrorMessage: " + sData);
                sReturn = ExtractData(sReturn, ref commandData, ref sCommandByte);
            }

            return sReturn;
        }

        private bool Validate(string sData, ref COMCommandData commandData)
        {
            bool functionReturnValue = false;
            int nLength = 0;
            int nChecksum = 0;
            nLength = Strings.Len(sData);

            // Validate minimum length
            if (nLength < 5)
            {
                commandData.ValidationMsg += "Data is too short. ";
                return functionReturnValue;
            }

            // Validate the head character
            if (Strings.Asc(Strings.Mid(sData, 1, 1)) != CMD_HEAD)
            {
                commandData.ValidationMsg += "Invalid head character. ";
                return functionReturnValue;
            }

            // Validate the end character
            if (Strings.Asc(Strings.Mid(sData, nLength, 1)) != CMD_END)
            {
                commandData.ValidationMsg += "Invalid end character. ";
                return functionReturnValue;
            }

            // Validate the length
            if (Strings.Asc(Strings.Mid(sData, 2, 1)) != Strings.Len(Strings.Mid(sData, 3)))
            {
                commandData.ValidationMsg += "Invalid length. ";
                return functionReturnValue;
            }

            /* // Validate the command
             if (Strings.Mid(sData, 3, 1) != Strings.LCase(sCommand))
             {
                 sReturn = "Invalid command byte.";
                 return functionReturnValue;
             }*/

            // Validate the checksum
            nChecksum = Strings.Asc(Strings.Mid(sData, nLength - 1, 1));
            if (Checksum(Strings.Mid(sData, 2, nLength - 3)) != nChecksum)
            {
                commandData.ValidationMsg += "Invalid checksum. ";
                return functionReturnValue;
            }

            if (Strings.Len(Strings.Mid(sData, 4, nLength - 5)) == 0)
            {
                commandData.ValidationMsg += "Command byte and parameters are missing. ";
                return functionReturnValue;
            }

            functionReturnValue = true;
            return functionReturnValue;
        }

        private string ExtractData(string sData, ref COMCommandData commandData, ref string sCommandByte)
        {
            //LogHelper.CreateLog("ExtractData - ", " ExtractData start sData : " + sData);

            int nLength = 0;
            nLength = Strings.Len(sData);

            // Validate minimum length
            if (nLength <= 5)
            {
                commandData.ValidationMsg += "Data is too short. ";
                return null;
            }

            sCommandByte = Strings.Mid(sData, 3, 1);
            // Extract the data; exclude head, length, command byte, checksum, and end (5) characters
            string extractData = Strings.Mid(sData, 4, nLength - 5);

            if (extractData.Contains("E"))
            {
                // response error
                string hexErrorCode = extractData.Replace("E", "");
                commandData.ResponseErrorCode = hexErrorCode;
            }
            commandData.ResponseData = extractData;
            return extractData;
        }

        private int Checksum(string sData)
        {
            int i = 0;
            int nChecksum = 0;

            for (i = 1; i <= sData.Length; i++)
            {
                nChecksum = nChecksum ^ Microsoft.VisualBasic.Strings.Asc(Strings.Mid(sData, i, 1));
            }
            return nChecksum;
        }

        #endregion

        #region Dispose Pattern
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.PortName = string.Empty;
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~COMWrapper()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion


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
    }


    public static class ExtentionMethods
    {
        public static string ToHex(this byte[] comByte)
        {
            //create a new StringBuilder object
            StringBuilder builder = new StringBuilder(comByte.Length * 3);
            //loop through each byte in the array
            foreach (byte data in comByte)
            {
                //convert the byte to a string and add to the stringbuilder
                builder.Append(Convert.ToString(data, 16).PadLeft(2, '0').PadRight(3, ' '));
            }
            //return the converted value
            return builder.ToString().ToUpper();
        }

        public static byte[] ToByte(this string strHex)
        {
            //remove any spaces from the string
            strHex = strHex.Replace(" ", "");
            //create a byte array the length of the
            //divided by 2 (Hex is 2 characters in length)
            byte[] comBuffer = new byte[strHex.Length / 2];
            //loop through the length of the provided string
            for (int i = 0; i < strHex.Length; i += 2)
            {
                //convert each set of 2 characters to a byte
                //and add to the array
                comBuffer[i / 2] = (byte)Convert.ToByte(strHex.Substring(i, 2), 16);
            }
            //return the array
            return comBuffer;
        }

        public static int ToInt(this string strHex)
        {
            int num = Convert.ToInt32(strHex, 16);
            return num;
        }
        public static string ToHex(this string strValue)
        {
            string sResult = string.Empty;

            foreach (char sChar in strValue)
            {
                int value = Convert.ToInt32(sChar);
                // Convert the decimal value to a hexadecimal value in string form.
                sResult += String.Format("{0:X2}", value);
                // sResult += sChar.ToString("X2");
            }

            return sResult;
        }

        public static string ToASCII(this string strHex)
        {
            // initialize the ASCII code string as empty.
            String ascii = "";

            for (int i = 0; i < strHex.Length; i += 2)
            {

                // extract two characters from hex string
                String part = strHex.Substring(i, 2);

                // change it into base 16 and 
                // typecast as the character
                char ch = (char)Convert.ToInt32(part, 16);
                // add this char to final ASCII string
                ascii = ascii + ch;
            }
            return ascii;
        }

    }




}