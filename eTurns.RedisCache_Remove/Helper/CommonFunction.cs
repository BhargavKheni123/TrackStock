using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace eTurns.RedisCache_Remove
{
    public static class CommonFunctions
    {
        public static void SaveLogInTextFile(string sbMessage, bool SendMailAswell)
        {
            try
            {
                string ErrorMessegeToDraw = string.Empty;
                string ServiceLogFileName = "MessgeLog_" + DateTime.UtcNow.ToString("dd_MM_yyyy_HH") + ".txt";
                string LogDirectory = AppDomain.CurrentDomain.BaseDirectory + "Import Log";
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
                //if (SendMailAswell)
                //{
                //    eMailMasterDAL objeMailMasterDAL = new eMailMasterDAL(DbConnectionHelper.GeteTurnsDBName());
                //    objeMailMasterDAL.eMailToSend(GeneralHelper.LogEmailTo, GeneralHelper.LogEmailCC, "Import Scheduler win service at UTC:" + DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss"), ErrorMessegeToDraw, 0, 0, 0, 0, null, "NIL");
                //}
            }
            catch (Exception)
            {

            }

        }

        public static void SaveExceptionInTextFile(string ExceptionLocation, Exception Ex, bool SendMailAswell)
        {
            try
            {
                string ErrorMessegeToDraw = string.Empty;
                string ServiceLogFileName = "ExceptionLog_" + DateTime.UtcNow.ToString("dd_MM_yyyy_HH") + ".txt";
                string LogDirectory = AppDomain.CurrentDomain.BaseDirectory + "Import Log";
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
                //if (SendMailAswell)
                //{
                //    eMailMasterDAL objeMailMasterDAL = new eMailMasterDAL(DbConnectionHelper.GeteTurnsDBName());
                //    objeMailMasterDAL.eMailToSend(GeneralHelper.LogEmailTo, GeneralHelper.LogEmailCC, "Import Scheduler win service at UTC:" + DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss"), ErrorMessegeToDraw, 0, 0, 0, 0, null, "NIL");
                //}
            }
            catch (Exception)
            {

            }

        }

        public static List<t> ConvertExportDataList<t>(object lstreturn)
        {
            foreach (var obj in (List<t>)lstreturn)
            {
                string value = string.Empty;
                foreach (PropertyInfo pi in ((t)obj).GetType().GetProperties().Where(x => x.PropertyType == typeof(string)))
                {
                    value = pi.GetValue(obj, null) as string;
                    if (!string.IsNullOrEmpty(value))
                        pi.SetValue(obj, "'" + value, null);
                }
            }
            return (List<t>)lstreturn;
        }

    }
}

