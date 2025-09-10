using eTurns.eVMIBAL.DTO;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eTurns.eVMIBAL.Wrappers
{
    public class COMWrapper : IDisposable
    {
        /*
         * 
         * Useful docs – 
         * 1) For Commands -> Communication Protocol NG-RIE-V2 - 10232020.pdf
         * 2) For Configuration -> SmartShelf USR-W610 Cloud Converter Configuration Guide (VCOM v3.7).pdf
         * 3) Smart Shelf info and Speed tool commands - "SmartShelf Reference Guide DRAFT TT033021.pdf"
         */

        #region Declaration or Types

        private const short CMD_HEAD = 0xF2;
        private const short CMD_END = 0xF3;
        private bool disposedValue;

        public enum TransmissionType { Text, Hex }

        public string PortName { get; set; }

        public class ShelfCommands
        {
            /// <summary>
            /// Get Weight Command
            /// </summary>
            public static string GetWeightCMD { get { return "W"; } }

            /// <summary>
            /// Get High Resolution Weight Command
            /// </summary>
            public static string GetHighResWeightCMD { get { return "H"; } }

            /// <summary>
            /// Zero Weight Command
            /// </summary>
            public static string ZeroWeightCMD { get { return "Z"; } }

            /// <summary>
            /// Reset Scale
            /// </summary>
            public static string ResetScaleCMD { get { return "R"; } }

            /// <summary>
            /// Firmware Version
            /// </summary>
            public static string FirmwareVersion { get { return "V"; } }

            /// <summary>
            /// SerialNo
            /// </summary>
            public static string SerialNo { get { return "1"; } }

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
            /// Set Model Number
            /// </summary>
            public static string SetModelNumber { get { return "M"; } }

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

            /// <summary>
            /// This step is used to set the zero to empty
            /// </summary>
            public static string CalibrationStep1 { get { return "C"; } }

            /// <summary>
            /// This step is used to adjust the preload and sets the zero with a tare
            /// </summary>
            public static string CalibrationStep2 { get { return "E"; } }

            /// <summary>
            /// This step is used to calibrate the Pad with the predetermined weight
            /// </summary>
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

            if (eVMIBALSettings.eVMIIsComPortSimulation == false)
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

            if (eVMIBALSettings.eVMIIsComPortSimulation == false)
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

        public COMCmdResponse<string> GetSerialNo(int scaleID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.SerialNo, scaleID, null, ShelfCommands.SerialNo);
            string data = ExecuteCommand(ref commandData);
            return COMCmdResponse<string>.CreateResponse(data, commandData);
        }

        public COMCmdResponse<decimal?> GetWeight(int scaleID, int channelID, string weightcommand, out string commandDataValue)
        {
            COMCommandData commandData = null;
            if (weightcommand == eTurns.DTO.eVMISiteSettings.PollCommand_H)
                commandData = new COMCommandData(ShelfCommands.GetHighResWeightCMD, scaleID, channelID);
            else
                commandData = new COMCommandData(ShelfCommands.GetWeightCMD, scaleID, channelID);

            string data = string.Empty;

            if (eVMIBALSettings.eVMIIsComPortSimulation == false)
            {
                data = ExecuteCommand(ref commandData);
            }
            else
            {
                Random r = new Random();
                data = Convert.ToString(r.Next(-20, 50) + r.NextDouble());
            }
            commandDataValue = data;
            if (commandData.IsError == false && !string.IsNullOrWhiteSpace(data) && !string.IsNullOrEmpty(data) && data != null && data != string.Empty)
            {
                decimal output;
                if (decimal.TryParse(data, out output))
                {
                    return COMCmdResponse<decimal?>.CreateResponse(Convert.ToDecimal(data), commandData);
                }
                else
                {
                    return COMCmdResponse<decimal?>.CreateResponse(null, commandData);
                }
            }

            return COMCmdResponse<decimal?>.CreateResponse(null, commandData);
        }

        public COMCmdResponse<decimal?> GetHighResWeight(int scaleID, int channelID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.GetHighResWeightCMD, scaleID, channelID);
            string data = string.Empty;

            if (eVMIBALSettings.eVMIIsComPortSimulation == false)
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

        /// <summary>
        /// Tare
        /// </summary>
        /// <param name="scaleID"></param>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public COMCmdResponse ZeroWeight(int scaleID, int channelID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.ZeroWeightCMD, scaleID, channelID);
            bool b = true;

            if (eVMIBALSettings.eVMIIsComPortSimulation == false)
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

        public COMCmdResponse<string> SetModelNumber(int scaleID, string modelNo)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.SetModelNumber, scaleID, null);
            commandData.ModelNo = modelNo;
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

        public COMCmdResponse<bool> SetCalibrationWeight(int scaleID, double weight)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.SetCalibrationWeight, scaleID);
            commandData.CalibrationWeight = weight;

            string data = ExecuteCommand(ref commandData);
            bool isSuccess = true; //data == "r";
            if (commandData.IsError == false)
            {
                return COMCmdResponse<bool>.CreateResponse(isSuccess, commandData);
            }

            return COMCmdResponse<bool>.CreateResponse(isSuccess, commandData);
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

        /// <summary>
        /// This command is only a hardware reboot. Nothing is changed in the parameters.
        /// </summary>
        /// <param name="scaleID"></param>
        /// <returns></returns>
        public COMCmdResponse<bool> ResetScale(int scaleID)
        {
            COMCommandData commandData = new COMCommandData(ShelfCommands.ResetScaleCMD, scaleID);
            string data = ExecuteCommand(ref commandData);
            bool isSuccess = true; //data == "r";
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
                string commandtext = string.Empty;
                string command = BuildCommand(commandData.CommandASCII, commandData.ScaleID.ToString(), commandData.ChannelID.ToString(), out commandtext,
                    commandData.CommandASCII2, commandData.CalibrationWeight, commandData.ModelNo);
                commandData.HEXRequest = command;
                commandData.CommandText = commandtext;
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

        private string BuildCommand(string commandASCII1, string pScaleID, string channelID,
            out string CommandText, string commandASCII2 = null, double? calibrationWeight = null,
            string modelNo = ""
            )
        {

            string scaleID = pScaleID == "" ? "" : pScaleID.PadLeft(4, '0');
            string sCommand = commandASCII1.Trim() + scaleID.Trim() + channelID.Trim();

            if (calibrationWeight.HasValue)
            {
                sCommand = sCommand + calibrationWeight.Value.ToString("00.00");
            }

            if (!string.IsNullOrWhiteSpace(modelNo))
            {
                sCommand = sCommand + modelNo;
            }

            //CommandText = string.Empty;

            if (!string.IsNullOrWhiteSpace(commandASCII2))
            {
                sCommand = sCommand + commandASCII2;
            }

            string commandHex = sCommand.ToHex();

            int nLength = sCommand.Length + 1 + 1;



            //if (!string.IsNullOrWhiteSpace(commandASCII2))
            //{
            //nLength = nLength + 3;
            //}

            int nChecksum = 0;


            nChecksum = Checksum(Convert.ToChar(nLength).ToString() + sCommand);


            string checksumHex = nChecksum.ToString("X2").Replace("0x", "");

            string lenHex = nLength.ToString("X2").Replace("0x", "");
            string sReturn = CMD_HEAD.ToString("X2") +
                lenHex +
                commandHex +
                checksumHex +
                CMD_END.ToString("X2");

            CommandText = sCommand + " | " + commandHex + " | " + sReturn;
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


}
