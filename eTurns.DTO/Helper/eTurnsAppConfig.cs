using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO.Helper
{
    public static class eTurnsAppConfig
    {
        public static string BaseFilePath
        {
            get
            {
                string _BaseFilePath = @"D:\WebSites\eTurns4040\";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["BaseFilePath"])))
                {
                    _BaseFilePath = Convert.ToString(ConfigurationManager.AppSettings["BaseFilePath"]);
                }
                return _BaseFilePath;
            }
        }

        public static string BaseFileSharedPath
        {
            get
            {
                string strBaseFileSharedPath = @"\\amznfsxyfdhnm2f.eturns.local\share\WebSites\eTurns4040\";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["BaseFileSharedPath"])))
                {
                    strBaseFileSharedPath = Convert.ToString(ConfigurationManager.AppSettings["BaseFileSharedPath"]);
                }
                return strBaseFileSharedPath;
            }
        }

        public static string eVMIWebAPIURL
        {
            get
            {
                string _eVMIWebAPIURL = @"http://localhost:3132/";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["eVMIWebAPIURL"])))
                {
                    _eVMIWebAPIURL = Convert.ToString(ConfigurationManager.AppSettings["eVMIWebAPIURL"]);
                }
                return _eVMIWebAPIURL;
            }
        }

        public static string redisCacheEndPoint
        {
            get
            {
                string _redisCacheEndPoint = "serverless-test-cache-gddcfo.serverless.use1.cache.amazonaws.com:6379,ssl=True,sslprotocols=tls12";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["redisCacheEndPoint"])))
                {
                    _redisCacheEndPoint = Convert.ToString(ConfigurationManager.AppSettings["redisCacheEndPoint"]);
                }
                return _redisCacheEndPoint;
            }
        }

    }
}
