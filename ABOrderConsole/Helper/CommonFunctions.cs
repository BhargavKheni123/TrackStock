using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using eTurns.DAL;
using eTurnsMaster.DAL;
using System.Security.Cryptography;

namespace ABOrderConsole.Helper
{
    public static class CommonFunctions
    {
        public static void SaveLogInTextFile(string sbMessage, bool SendMailAswell)
        {
            try
            {
                string ErrorMessegeToDraw = string.Empty;
                string ServiceLogFileName = "MessgeLog_" + DateTime.UtcNow.ToString("dd_MM_yyyy_HH") + ".txt";
                string LogDirectory = AppDomain.CurrentDomain.BaseDirectory + GeneralHelper.LogDirectory;
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
                if (SendMailAswell)
                {
                    eMailMasterDAL objeMailMasterDAL = new eMailMasterDAL(DbConnectionHelper.GeteTurnsDBName());
                    objeMailMasterDAL.eMailToSend(GeneralHelper.LogEmailTo, GeneralHelper.LogEmailCC, "Import Scheduler win service at UTC:" + DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss"), ErrorMessegeToDraw, 0, 0, 0, 0, null, "NIL");
                }
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
                string LogDirectory = AppDomain.CurrentDomain.BaseDirectory + GeneralHelper.LogDirectory;
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
                if (SendMailAswell)
                {
                    eMailMasterDAL objeMailMasterDAL = new eMailMasterDAL(DbConnectionHelper.GeteTurnsDBName());
                    objeMailMasterDAL.eMailToSend(GeneralHelper.LogEmailTo, GeneralHelper.LogEmailCC, "Import Scheduler win service at UTC:" + DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss"), ErrorMessegeToDraw, 0, 0, 0, 0, null, "NIL");
                }
            }
            catch (Exception)
            {

            }
        }

        public static string CheckDuplicateEnterprise(string EntName, Int64 appendstring,bool firstCall = true)
        {
            //string mainEntName = EntName;
            CommonMasterDAL objCommonMasterDAL = new CommonMasterDAL();
            string strOK = objCommonMasterDAL.EnterPriseDuplicateCheck(0,(firstCall ? EntName : (EntName + "_" + appendstring)));
            string FinalEntName = (firstCall ? EntName : (EntName + "_" + appendstring));
            if (strOK == "duplicate")
            {
                return CheckDuplicateEnterprise(EntName, (appendstring + 1),false);
            }
            else
            {
                return FinalEntName;
            }
        }
        public static string CheckDuplicateUserName(string UserName, Int64 appendstring, bool firstCall)
        {
            CommonMasterDAL objCommonMasterDAL = new CommonMasterDAL();
            string strOK = objCommonMasterDAL.UserDuplicateCheckUserName(0, (firstCall ? UserName : (UserName + "_" + appendstring)));
            string FinalUserName = (firstCall ? UserName : (UserName + "_" + appendstring));
            
            if (strOK == "duplicate")
            {
                return CheckDuplicateUserName(UserName, (appendstring + 1), false);
            }
            else
            {
                return FinalUserName;
            }
        }

        public static string CheckDuplicateRoleName(string RoleName, long appendstring, bool firstCall)
        {
            CommonMasterDAL objCommonMasterDAL = new CommonMasterDAL();
            string strOK = objCommonMasterDAL.RoleDuplicateCheck(0, (firstCall ? RoleName : (RoleName + "_" + appendstring)));
            string FinalRoleName = (firstCall ? RoleName : (RoleName + "_" + appendstring));

            if (strOK == "duplicate")
            {
                return CheckDuplicateRoleName(RoleName, (appendstring + 1), false);
            }
            else
            {
                return FinalRoleName;
            }
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string getSHA15Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            SHA1CryptoServiceProvider Sha1Hasher = new SHA1CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = Sha1Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
    }
}
