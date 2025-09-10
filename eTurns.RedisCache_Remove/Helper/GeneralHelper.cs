using System;
using System.Configuration;

namespace eTurns.RedisCache_Remove
{
    public static class GeneralHelper
    {
        public static string FtpServerIP
        {
            get
            {
                string _FtpServerIP = "18.210.133.206";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["FtpServerIP"])))
                {
                    _FtpServerIP = Convert.ToString(ConfigurationManager.AppSettings["FtpServerIP"]);
                }
                return _FtpServerIP;
            }
        }
        public static string FtpServerPort
        {
            get
            {
                string _FtpServerPort = @"22";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["FtpServerPort"])))
                {
                    _FtpServerPort = Convert.ToString(ConfigurationManager.AppSettings["FtpServerPort"]);
                }
                return _FtpServerPort;
            }
        }
        public static string FtpUserID
        {
            get
            {
                string _FtpUserID = @"BSEDemoCatalog";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["FtpUserID"])))
                {
                    _FtpUserID = Convert.ToString(ConfigurationManager.AppSettings["FtpUserID"]);
                }
                return _FtpUserID;
            }
        }

        public static string FtpUserPassword
        {
            get
            {
                string _FtpUserPassword = @"SaveNature#070224";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["FtpUserPassword"])))
                {
                    _FtpUserPassword = Convert.ToString(ConfigurationManager.AppSettings["FtpUserPassword"]);
                }
                return _FtpUserPassword;
            }
        }

        public static double TimerIntervalMSFTPDownloadFiles
        {
            get
            {
                double _TimerIntervalMS = 60000;
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["TimerIntervalMSFTPDownloadFiles"])))
                {
                    _TimerIntervalMS = Convert.ToDouble(ConfigurationManager.AppSettings["TimerIntervalMSFTPDownloadFiles"]);
                }
                return _TimerIntervalMS;
            }
        }
        public static double TimerCommonBOMImport
        {
            get
            {
                double _TimerIntervalMS = 60000;
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["TimerCommonBOMImport"])))
                {
                    _TimerIntervalMS = Convert.ToDouble(ConfigurationManager.AppSettings["TimerCommonBOMImport"]);
                }
                return _TimerIntervalMS;
            }
        }
        public static double TimerAutoSotImport
        {
            get
            {
                double _TimerIntervalMS = 60000;
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["TimerAutoSotImport"])))
                {
                    _TimerIntervalMS = Convert.ToDouble(ConfigurationManager.AppSettings["TimerAutoSotImport"]);
                }
                return _TimerIntervalMS;
            }
        }
        public static string DefaultSupplierName
        {
            get
            {
                string _DefaultSupplierName = "Border States";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["DefaultSupplierName"])))
                {
                    _DefaultSupplierName = Convert.ToString(ConfigurationManager.AppSettings["DefaultSupplierName"]);
                }
                return _DefaultSupplierName;
            }
        }
        public static string LogDirectory
        {
            get
            {
                string DefaultDName = "ErrorLogs";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["LogDirectory"])))
                {
                    DefaultDName = Convert.ToString(ConfigurationManager.AppSettings["LogDirectory"]);
                }
                return DefaultDName;
            }
        }
        public static bool DoSendLogsInMail
        {
            get
            {
                string DoSendLogsInMaillocal = "0";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["DoSendLogsInMail"])))
                {
                    DoSendLogsInMaillocal = Convert.ToString(ConfigurationManager.AppSettings["DoSendLogsInMail"]);
                }
                if (DoSendLogsInMaillocal == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
        public static string RemoteZipFilesFolderName
        {
            get
            {
                string _RemoteZipFilesFolderName = @"RemoteZipFiles";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["RemoteZipFilesFolderName"])))
                {
                    _RemoteZipFilesFolderName = Convert.ToString(ConfigurationManager.AppSettings["RemoteZipFilesFolderName"]);
                }
                return _RemoteZipFilesFolderName;
            }
        }
        public static string RemoteZipFilesDoneFolderName
        {
            get
            {
                string _RemoteZipFilesDoneFolderName = @"RemoteZipFilesDone";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["RemoteZipFilesDoneFolderName"])))
                {
                    _RemoteZipFilesDoneFolderName = Convert.ToString(ConfigurationManager.AppSettings["RemoteZipFilesDoneFolderName"]);
                }
                return _RemoteZipFilesDoneFolderName;
            }
        }
        public static string CommonBOMImport
        {
            get
            {
                string _CommonBOMImport = @"CommonBOMImport";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["CommonBOMImport"])))
                {
                    _CommonBOMImport = Convert.ToString(ConfigurationManager.AppSettings["CommonBOMImport"]);
                }
                return _CommonBOMImport;
            }
        }
        public static string BOMItemImportPath
        {
            get
            {
                string BOMItemImportPath = "";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["ImportFilePath"])))
                {
                    BOMItemImportPath = Convert.ToString(ConfigurationManager.AppSettings["ImportFilePath"]);
                }
                return BOMItemImportPath;
            }
        }

        public static string BOMItemProcessImportPath
        {
            get
            {
                string BOMItemImportPath = "";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["ProcessFilePath"])))
                {
                    BOMItemImportPath = Convert.ToString(ConfigurationManager.AppSettings["ProcessFilePath"]);
                }
                return BOMItemImportPath;
            }
        }
        public static string BOMOriginalImportFilePath
        {
            get
            {
                string BOMItemImportPath = "";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["OriginalImportFilePath"])))
                {
                    BOMItemImportPath = Convert.ToString(ConfigurationManager.AppSettings["OriginalImportFilePath"]);
                }
                return BOMItemImportPath;
            }
        }

        public static string TemplatePath
        {
            get
            {
                string ItemImportPath = "";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["TemplatePath"])))
                {
                    ItemImportPath = Convert.ToString(ConfigurationManager.AppSettings["TemplatePath"]);
                }
                return ItemImportPath;
            }
        }

        public static string LogEmailCC
        {
            get
            {
                string LogEmailCClocal = "";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["LogEmailCC"])))
                {
                    LogEmailCClocal = Convert.ToString(ConfigurationManager.AppSettings["LogEmailCC"]);
                }
                return LogEmailCClocal;
            }
        }

        public static string LogEmailTo
        {
            get
            {
                string LogEmailTolocal = "harshida@eturns.com";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["LogEmailTo"])))
                {
                    LogEmailTolocal = Convert.ToString(ConfigurationManager.AppSettings["LogEmailTo"]);
                }
                return LogEmailTolocal;
            }
        }

        public static bool IsSendProcessedFileInMail
        {
            get
            {
                string DoSendLogsInMaillocal = "0";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["IsSendProcessedFileInMail"])))
                {
                    DoSendLogsInMaillocal = Convert.ToString(ConfigurationManager.AppSettings["IsSendProcessedFileInMail"]);
                }
                if (DoSendLogsInMaillocal == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
    }
}
