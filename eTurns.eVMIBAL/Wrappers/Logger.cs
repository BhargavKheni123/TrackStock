using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using eTurns.DAL;

namespace eTurns.eVMIBAL.Wrappers
{
    public static class Logger
    {
        public static string LogDirectory
        {
            get
            {
                string logDirectory = AppDomain.CurrentDomain.BaseDirectory + eVMIBALSettings.eVMILogDirectory;
                return logDirectory;
            }
        }

        public static string LogFilePath
        {
            get
            {
                string ServiceLogFileName = "MessgeLog_" + DateTime.UtcNow.ToString("dd_MMM_yyyy") + ".txt";
                string logFile = LogDirectory + "\\" + ServiceLogFileName;
                return logFile;
            }
        }

        public static string ExceptionLogFilePath
        {
            get
            {
                string ServiceLogFileName = "ExceptionLog_" + DateTime.UtcNow.ToString("dd_MMM_yyyy") + ".txt";

                string logFile = LogDirectory + "\\" + ServiceLogFileName;
                return logFile;
            }
        }

        public static void WriteLog(string sbMessage)
        {
            //bool SendMailAswell = AppSettings.SendErrorEmail;
            try
            {
                string ErrorMessegeToDraw = string.Empty;

                if (!System.IO.Directory.Exists(LogDirectory))
                {
                    System.IO.Directory.CreateDirectory(LogDirectory);
                }
                //ErrorMessegeToDraw += Environment.NewLine;
                //ErrorMessegeToDraw += "==================================================================[Log Messege START]==================================================================";
                //ErrorMessegeToDraw += Environment.NewLine;
                //ErrorMessegeToDraw += "Log Written at >> Local Time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "  and UTC Time:" + DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");
                //ErrorMessegeToDraw += Environment.NewLine;
                //ErrorMessegeToDraw += "your Loging messege as below:";
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " >> [[!ActualMessge!]]";
                //ErrorMessegeToDraw += Environment.NewLine;
                //ErrorMessegeToDraw += "******************************************************************[Log Messege END]********************************************************************";
                //if (!string.IsNullOrWhiteSpace(sbMessage))
                //{
                ErrorMessegeToDraw = ErrorMessegeToDraw.Replace("[[!ActualMessge!]]", sbMessage);
                //}
                //else
                //{
                //    ErrorMessegeToDraw = ErrorMessegeToDraw.Replace("[[!ActualMessge!]]", "There is Empty OR NULL value Sent in sbMessage parameter.");
                //}
                if (System.IO.File.Exists(LogFilePath))
                {
                    using (FileStream fs = new FileStream(LogFilePath, FileMode.Append))
                    {
                        StreamWriter sw = new StreamWriter(fs);
                        sw.Write(Environment.NewLine);
                        sw.Write(ErrorMessegeToDraw);
                        sw.Close();
                    }
                }
                else
                {
                    using (FileStream fs = new FileStream(LogFilePath, FileMode.OpenOrCreate))
                    {
                        StreamWriter sw = new StreamWriter(fs);
                        sw.Write(Environment.NewLine);
                        sw.Write(ErrorMessegeToDraw);
                        sw.Close();
                    }
                }
                //if (SendMailAswell)
                //{
                //    SendEmail(ErrorMessegeToDraw);
                //}
            }
            catch (Exception)
            {

            }

        }

        private static void SendEmail(string msg)
        {
            eMailMasterDAL objeMailMasterDAL = new eMailMasterDAL(DbConnectionHelper.GeteTurnsDBName());
            objeMailMasterDAL.eMailToSend(eVMIBALSettings.eVMILogEmailTo, eVMIBALSettings.eVMILogEmailCC, "eVMI Windows Service Error", msg, 0, 0, 0, 0, null, "NIL");
        }

        public static void WriteExceptionLog(string ExceptionLocation, Exception ex)
        {
            bool SendMailAswell = eVMIBALSettings.eVMISendErrorEmail;
            try
            {
                string ErrorMessegeToDraw = string.Empty;

                string LogFile = ExceptionLogFilePath;

                if (!System.IO.Directory.Exists(LogDirectory))
                {
                    System.IO.Directory.CreateDirectory(LogDirectory);
                }
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "==================================================================[Log Messege START]==================================================================";
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "Log Written at >> Local Time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "  and UTC Time:" + DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");
                ErrorMessegeToDraw += Environment.NewLine;
                //ErrorMessegeToDraw += "your Loging messege as below:";
                //ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += " >> [[!ActualMessge!]]";
                //ErrorMessegeToDraw += Environment.NewLine;
                //ErrorMessegeToDraw += "Your Exception messege as below:";
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "[[!ActualEXEPTION!]]";
                ErrorMessegeToDraw += Environment.NewLine;
                ErrorMessegeToDraw += "******************************************************************[Log Messege END]******************************************************************";
                if (!string.IsNullOrWhiteSpace(ExceptionLocation))
                {
                    ErrorMessegeToDraw = ErrorMessegeToDraw.Replace("[[!ActualMessge!]]", ExceptionLocation);
                }
                else
                {
                    ErrorMessegeToDraw = ErrorMessegeToDraw.Replace("[[!ActualMessge!]]", "There is Empty OR NULL value Sent in ExceptionLocation parameter.");
                }
                if (ex != null)
                {

                    string logstr = GetExceptionDetails(ex);

                    ErrorMessegeToDraw = ErrorMessegeToDraw.Replace("[[!ActualEXEPTION!]]", logstr);
                    //ErrorMessegeToDraw = ErrorMessegeToDraw.Replace("[!ActualEXEPTION!]", (Convert.ToString(Ex) ?? string.Empty));
                }
                //else
                //{
                //    ErrorMessegeToDraw = ErrorMessegeToDraw.Replace("[[!ActualEXEPTION!]]", "There is Empty OR NULL value Sent in ExceptionLocation parameter.");
                //}
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
                if (SendMailAswell)
                {
                    SendEmail(ErrorMessegeToDraw);
                }
            }
            catch (Exception)
            {

            }

        }


        public static string GetExceptionDetails(Exception ex)
        {
            string logstr = string.Empty;

            if (ex != null)
            {
                if (ex != null && !string.IsNullOrWhiteSpace(ex.Message))
                {
                    logstr += Environment.NewLine;
                    logstr += " >> Message : " + ex.Message;
                    logstr += Environment.NewLine;

                }
                if (ex != null && !string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    logstr += Environment.NewLine;
                    logstr += " >> StackTrace :" + ex.StackTrace;
                    logstr += Environment.NewLine;
                }

                if (ex != null && ex.InnerException != null && ex.InnerException.ToString() != string.Empty)
                {
                    logstr += Environment.NewLine;
                    logstr += " >> InnerException : " + ex.InnerException.ToString();
                    logstr += Environment.NewLine;
                }

            }

            return logstr;
        }

        public static void WriteEventLog(EventLog eventLog, string msg, bool isWriteInFile = true)
        {
            try
            {
                eventLog.WriteEntry(msg);
                if (isWriteInFile)
                {
                    WriteLog(msg);
                }
            }
            catch (Exception ex)
            {

            }

        }

    }
}
