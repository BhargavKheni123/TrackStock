using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace eTurns.eVMIBAL
{
    public static class eVMIBALSettings
    {
        private static string GetConfigValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string eVMILogDirectory
        {
            get
            {
                string DefaultDName = "eVMILogDirectory";
                var val = GetConfigValue("eVMILogDirectory");
                if (!string.IsNullOrWhiteSpace(val))
                {
                    DefaultDName = val;
                }
                return DefaultDName;
            }
        }

        public static bool eVMISendErrorEmail
        {
            get
            {
                bool Flag = false;
                var val = GetConfigValue("eVMISendErrorEmail");
                if (!string.IsNullOrWhiteSpace(val))
                {
                    Flag = Convert.ToBoolean(val);
                }
                return Flag;
            }
        }

        public static string eVMILogEmailTo
        {
            get
            {
                string LogEmailTolocal = "eturnsdevmails@silvertouch.com";
                var val = GetConfigValue("eVMILogEmailTo");
                if (!string.IsNullOrWhiteSpace(val))
                {
                    LogEmailTolocal = val;
                }
                return LogEmailTolocal;
            }
        }

        public static string eVMILogEmailCC
        {
            get
            {
                string LogEmailCClocal = "support@eturns.com";
                var val = GetConfigValue("eVMILogEmailCC");
                if (!string.IsNullOrWhiteSpace(val))
                {
                    LogEmailCClocal = val;
                }
                return LogEmailCClocal;
            }
        }

        /// <summary>
        /// If Com Port Simulation = true =>  will return random com port data
        /// </summary>
        public static bool eVMIIsComPortSimulation
        {
            get
            {
                bool isComSimulation = false;
                var val = GetConfigValue("eVMIIsComPortSimulation");
                if (!string.IsNullOrWhiteSpace(val))
                {
                    isComSimulation = val == "1";
                }
                return isComSimulation;
            }
        }

        public static bool eVMIIsFromWebSite
        {
            get
            {
                bool IsFromWebSite = false;
                var val = GetConfigValue("eVMIIsFromWebSite");
                if (!string.IsNullOrWhiteSpace(val))
                {
                    IsFromWebSite = val == "1";
                }
                return IsFromWebSite;
            }
        }

    }// class
}
