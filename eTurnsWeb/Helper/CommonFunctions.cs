using eTurns.DAL;
using eTurns.DTO;
using eTurnsMaster.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurnsWeb.Helper
{

    public static class CommonFunctions
    {
        public static void SaveLogInTextFile(string sbMessage)
        {
            try
            {
                string ErrorMessegeToDraw = string.Empty;
                string ServiceLogFileName = "MessgeLog_" + DateTime.UtcNow.ToString("dd_MM_yyyy_HH") + ".txt";
                string LogDirectory = AppDomain.CurrentDomain.BaseDirectory + "ABerrorLogs";
                string LogFile = LogDirectory + "\\" + ServiceLogFileName;
                if (!System.IO.Directory.Exists(LogDirectory))
                {
                    System.IO.Directory.CreateDirectory(LogDirectory);
                }
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "==================================================================[Log Messege START]==================================================================";
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "Log Written at >> Local Time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "  and UTC Time:" + DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "your Loging messege as below:";
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "[[!ActualMessge!]]";
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "******************************************************************[Log Messege END]********************************************************************";
                if (!string.IsNullOrWhiteSpace(sbMessage))
                {
                    ErrorMessegeToDraw = ErrorMessegeToDraw.Replace("[!ActualMessge!]", sbMessage);
                }
                else
                {
                    ErrorMessegeToDraw = ErrorMessegeToDraw.Replace("[!ActualMessge!]", "There is Empty OR NULL value Sent in sbMessage parameter.");
                }
                if (System.IO.File.Exists(LogFile))
                {
                    using (FileStream fs = new FileStream(LogFile, FileMode.Append))
                    {
                        StreamWriter sw = new StreamWriter(fs);
                        sw.Write(Environment.NewLine);
                        sw.Write(ErrorMessegeToDraw);
                        sw.Close();
                    }
                }
                else
                {
                    using (FileStream fs = new FileStream(LogFile, FileMode.OpenOrCreate))
                    {
                        StreamWriter sw = new StreamWriter(fs);
                        sw.Write(Environment.NewLine);
                        sw.Write(ErrorMessegeToDraw);
                        sw.Close();
                    }
                }

            }
            catch (Exception)
            {

            }

        }
        public static void SaveExceptionInTextFile(string ExceptionLocation, Exception Ex)
        {
            try
            {
                string ErrorMessegeToDraw = string.Empty;
                string ServiceLogFileName = "ExceptionLog_" + DateTime.UtcNow.ToString("dd_MM_yyyy_HH") + ".txt";
                string LogDirectory = AppDomain.CurrentDomain.BaseDirectory + "ABerrorLogs";
                string LogFile = LogDirectory + "\\" + ServiceLogFileName;
                if (!System.IO.Directory.Exists(LogDirectory))
                {
                    System.IO.Directory.CreateDirectory(LogDirectory);
                }
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "==================================================================[Log Messege START]==================================================================";
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "Log Written at >> Local Time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "  and UTC Time:" + DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "your Loging messege as below:";
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "[[!ActualMessge!]]";
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "your Exception messege as below:";
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "[[!ActualEXEPTION!]]";
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "******************************************************************[Log Messege END]******************************************************************";
                if (!string.IsNullOrWhiteSpace(ExceptionLocation))
                {
                    ErrorMessegeToDraw = ErrorMessegeToDraw.Replace("[!ActualMessge!]", ExceptionLocation);
                }
                else
                {
                    ErrorMessegeToDraw = ErrorMessegeToDraw.Replace("[!ActualMessge!]", "There is Empty OR NULL value Sent in ExceptionLocation parameter.");
                }
                if (Ex != null)
                {
                    ErrorMessegeToDraw = ErrorMessegeToDraw.Replace("[!ActualEXEPTION!]", (Convert.ToString(Ex) ?? string.Empty));
                }
                else
                {
                    ErrorMessegeToDraw = ErrorMessegeToDraw.Replace("[!ActualEXEPTION!]", "There is Empty OR NULL value Sent in ExceptionLocation parameter.");
                }
                if (System.IO.File.Exists(LogFile))
                {
                    using (FileStream fs = new FileStream(LogFile, FileMode.Append))
                    {
                        StreamWriter sw = new StreamWriter(fs);
                        sw.Write(Environment.NewLine);
                        sw.Write(ErrorMessegeToDraw);
                        sw.Close();
                    }
                }
                else
                {
                    using (FileStream fs = new FileStream(LogFile, FileMode.OpenOrCreate))
                    {
                        StreamWriter sw = new StreamWriter(fs);
                        sw.Write(Environment.NewLine);
                        sw.Write(ErrorMessegeToDraw);
                        sw.Close();
                    }
                }

            }
            catch (Exception)
            {

            }

        }
        public static void SetSensorBinRoomSettings(string EnterPriseDBName, long RoomID, long EnterPriceID, long CompanyID)
        {
            SessionHelper.SensorBinRoomSettings = null;
            eVMISiteSettings SensorBinRoomSettings = null;
            if (EnterPriseDBName != string.Empty)
            {
                RoomDAL RoomDAL = new RoomDAL(EnterPriseDBName);
                SensorBinRoomSettings = RoomDAL.GetSensorBinRoomSettings(RoomID, EnterPriceID, CompanyID);
            }
            if (SensorBinRoomSettings != null)
            {
                SessionHelper.SensorBinRoomSettings = SensorBinRoomSettings;
            }
            else
            {
                eVMISiteSettings DefaultSensorBinRoomSettings = new eVMISiteSettings();
                SessionHelper.SensorBinRoomSettings = DefaultSensorBinRoomSettings;
            }
        }
    }
}
