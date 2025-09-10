using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ABOrderConsole.Helper
{
    public static class GeneralHelper
    {
        public static string eTurnsMasterDBName
        {
            get
            {
                string _eTurnsMasterDBName = "eTurnsMaster";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["MasterDBName"])))
                {
                    _eTurnsMasterDBName = Convert.ToString(ConfigurationManager.AppSettings["MasterDBName"]);
                }
                return _eTurnsMasterDBName;
            }
        }
        public static string eTurnsLoggingDBName
        {
            get
            {
                string eTurnsLoggingDBName = "eTurnsLogging";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["eTurnsLoggingDBName"])))
                {
                    eTurnsLoggingDBName = Convert.ToString(ConfigurationManager.AppSettings["eTurnsLoggingDBName"]);
                }
                return eTurnsLoggingDBName;
            }
        }
        public static string DomainName
        {
            get
            {
                string DomainName = "http://sapphire.eturns.com/";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["DomainName"])))
                {
                    DomainName = Convert.ToString(ConfigurationManager.AppSettings["DomainName"]);
                }
                return DomainName;
            }
        }
        public static string BaseFilePath
        {
            get
            {
                string BaseFilePath = @"D:\Projects\eTurns\Trunk\eTurns.TrackStock\eTurnsWeb\";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["BaseFilePath"])))
                {
                    BaseFilePath = Convert.ToString(ConfigurationManager.AppSettings["BaseFilePath"]);
                }
                return BaseFilePath;
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
        public static string LogEmailTo
        {
            get
            {
                string LogEmailTolocal = "eturnsdevmails@silvertouch.com";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["LogEmailTo"])))
                {
                    LogEmailTolocal = Convert.ToString(ConfigurationManager.AppSettings["LogEmailTo"]);
                }
                return LogEmailTolocal;
            }
        }
        public static string LogEmailCC
        {
            get
            {
                string LogEmailCClocal = "support@eturns.com";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["LogEmailCC"])))
                {
                    LogEmailCClocal = Convert.ToString(ConfigurationManager.AppSettings["LogEmailCC"]);
                }
                return LogEmailCClocal;
            }
        }
        public static bool DoSendLogsInMail
        {
            get
            {
                string DoSendLogsInMaillocal = "0";
                //if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["DoSendLogsInMail"])))
                //{
                //    DoSendLogsInMaillocal = Convert.ToString(ConfigurationManager.AppSettings["DoSendLogsInMail"]);
                //}
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
        public static Int64 EnterpiseCreatedBy
        {
            get
            {
                Int64 EnterpiseCreatedBy = 100;
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["EnterpiseCreatedBy"])))
                {
                    EnterpiseCreatedBy = Convert.ToInt64(ConfigurationManager.AppSettings["EnterpiseCreatedBy"]);
                }
                return EnterpiseCreatedBy;
            }
        }
        public static string EnterpiseUserNamePostFix
        {
            get
            {
                string EnterpiseUserNamePostFix = " SysAdmin";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["EnterpiseUserNamePostFix"])))
                {
                    EnterpiseUserNamePostFix = Convert.ToString(ConfigurationManager.AppSettings["EnterpiseUserNamePostFix"]);
                }
                return EnterpiseUserNamePostFix;
            }
        }

        public static string DefaultSupplierName
        {
            get
            {
                string DefaultSupplierName = "AMAZON";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["DefaultSupplierName"])))
                {
                    DefaultSupplierName = Convert.ToString(ConfigurationManager.AppSettings["DefaultSupplierName"]);
                }
                return DefaultSupplierName;
            }
        }
        public static string DefaultBinNamePostFix
        {
            get
            {
                string DefaultBinNamePostFix = " Bin";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["DefaultBinNamePostFix"])))
                {
                    DefaultBinNamePostFix = Convert.ToString(ConfigurationManager.AppSettings["DefaultBinNamePostFix"]);
                }
                return DefaultBinNamePostFix;
            }
        }
        public static string DefaultBarcodeTemplateName
        {
            get
            {
                string DefaultBarcodeTemplateName = "QR code Inventory default";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["DefaultBarcodeTemplateName"])))
                {
                    DefaultBarcodeTemplateName = Convert.ToString(ConfigurationManager.AppSettings["DefaultBarcodeTemplateName"]);
                }
                return DefaultBarcodeTemplateName;
            }
        }
    }
}
