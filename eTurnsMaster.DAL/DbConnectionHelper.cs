using eTurns.DTO;
using System;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;

namespace eTurnsMaster.DAL
{
    public enum MasterDbConnectionType
    {
        EFReadonly = 1,
        EFReadWrite = 2,
        GeneralReadWrite = 3,
        GeneralReadOnly = 4
    }
    public class MasterDbConnectionHelper
    {
        public static string GeteTurnsEntityFWConnectionString(string DataBaseName, string ConnectionType)
        {
            string providerName = ConfigurationManager.AppSettings.AllKeys.Contains("DBproviderName") ? Convert.ToString(ConfigurationManager.AppSettings["DBproviderName"]) : "System.Data.SqlClient";
            string DBserverName = ConfigurationManager.AppSettings.AllKeys.Contains("DBserverName") ? Convert.ToString(ConfigurationManager.AppSettings["DBserverName"]) : "DESKTOP-LV6DASV";
            string DBName = DataBaseName != null ? Convert.ToString(DataBaseName) : GetETurnsMasterDBName();
            string DbUserName = ConfigurationManager.AppSettings.AllKeys.Contains("DbUserName") ? Convert.ToString(ConfigurationManager.AppSettings["DbUserName"]) : "sa";
            string DbPassword = ConfigurationManager.AppSettings.AllKeys.Contains("DbPassword") ? Convert.ToString(ConfigurationManager.AppSettings["DbPassword"]) : "ibm@@123";
            string DBFailoverPartner = ConfigurationManager.AppSettings.AllKeys.Contains("DBFailoverPartner") ? Convert.ToString(ConfigurationManager.AppSettings["DBFailoverPartner"]) : "172.31.12.215";
            string IsWSFC = Convert.ToString(ConfigurationManager.AppSettings["IsWSFC"]) ?? "no";
            string WSFCLSNR = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNR"]) ?? "LSNPRD1";
            string WSFCLSNRPORT = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNRPORT"]) ?? "1433";
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DBserverName;
            sqlBuilder.InitialCatalog = DBName;
            sqlBuilder.UserID = DbUserName;
            sqlBuilder.Password = DbPassword;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.FailoverPartner = DBFailoverPartner;
            string providerString = sqlBuilder.ToString();
            //sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = providerName;

            if (IsWSFC == "yes")
            {
                providerString = GetOledbConnection(ConnectionType, DBName);
            }
            else
            {
                providerString = sqlBuilder.ToString();
            }

            entityBuilder.ProviderConnectionString = providerString;

            entityBuilder.Metadata = @"res://*/eTurnsMaster.csdl|res://*/eTurnsMaster.ssdl|res://*/eTurnsMaster.msl";
            return entityBuilder.ToString();
        }
        public static string GeteTurnsSQLConnectionString(string DataBaseName, string ConnectionType)
        {
            string providerName = ConfigurationManager.AppSettings.AllKeys.Contains("DBproviderName") ? Convert.ToString(ConfigurationManager.AppSettings["DBproviderName"]) : "System.Data.SqlClient";
            string DBserverName = ConfigurationManager.AppSettings.AllKeys.Contains("DBserverName") ? Convert.ToString(ConfigurationManager.AppSettings["DBserverName"]) : "DESKTOP-LV6DASV";
            string DBName = DataBaseName != null ? Convert.ToString(DataBaseName) : GetETurnsMasterDBName();
            string DbUserName = ConfigurationManager.AppSettings.AllKeys.Contains("DbUserName") ? Convert.ToString(ConfigurationManager.AppSettings["DbUserName"]) : "sa";
            string DbPassword = ConfigurationManager.AppSettings.AllKeys.Contains("DbPassword") ? Convert.ToString(ConfigurationManager.AppSettings["DbPassword"]) : "ibm@@123";
            string DBFailoverPartner = ConfigurationManager.AppSettings.AllKeys.Contains("DBFailoverPartner") ? Convert.ToString(ConfigurationManager.AppSettings["DBFailoverPartner"]) : "172.31.12.215";
            string IsWSFC = Convert.ToString(ConfigurationManager.AppSettings["IsWSFC"]) ?? "no";
            string WSFCLSNR = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNR"]) ?? "LSNPRD1";
            string WSFCLSNRPORT = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNRPORT"]) ?? "1433";
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DBserverName;
            sqlBuilder.InitialCatalog = DBName;
            sqlBuilder.UserID = DbUserName;
            sqlBuilder.Password = DbPassword;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.FailoverPartner = DBFailoverPartner;
            string providerString = sqlBuilder.ToString();
            //sqlBuilder.PersistSecurityInfo = true;           
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = providerName;

            if (IsWSFC == "yes")
            {
                providerString = GetOledbConnection(ConnectionType, DBName);
            }
            else
            {
                providerString = sqlBuilder.ToString();
            }
            return providerString;
        }
        public static string GeteTurnsMasterEntityFWConnectionString(string DataBaseName, string ConnectionType)
        {
            string providerName = ConfigurationManager.AppSettings.AllKeys.Contains("DBproviderName") ? Convert.ToString(ConfigurationManager.AppSettings["DBproviderName"]) : "System.Data.SqlClient";
            string DBserverName = ConfigurationManager.AppSettings.AllKeys.Contains("DBserverName") ? Convert.ToString(ConfigurationManager.AppSettings["DBserverName"]) : "DESKTOP-LV6DASV";
            string DBName = DataBaseName != null ? Convert.ToString(DataBaseName) : GetETurnsMasterDBName();
            string DbUserName = ConfigurationManager.AppSettings.AllKeys.Contains("DbUserName") ? Convert.ToString(ConfigurationManager.AppSettings["DbUserName"]) : "sa";
            string DbPassword = ConfigurationManager.AppSettings.AllKeys.Contains("DbPassword") ? Convert.ToString(ConfigurationManager.AppSettings["DbPassword"]) : "ibm@@123";
            string DBFailoverPartner = ConfigurationManager.AppSettings.AllKeys.Contains("DBFailoverPartner") ? Convert.ToString(ConfigurationManager.AppSettings["DBFailoverPartner"]) : "172.31.12.215";
            string IsWSFC = Convert.ToString(ConfigurationManager.AppSettings["IsWSFC"]) ?? "no";
            string WSFCLSNR = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNR"]) ?? "LSNPRD1";
            string WSFCLSNRPORT = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNRPORT"]) ?? "1433";
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DBserverName;
            sqlBuilder.InitialCatalog = DBName;
            sqlBuilder.UserID = DbUserName;
            sqlBuilder.Password = DbPassword;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.FailoverPartner = DBFailoverPartner;
            string providerString = sqlBuilder.ToString();
            //sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = providerName;

            if (IsWSFC == "yes")
            {
                providerString = GetOledbConnection(ConnectionType, DBName);
            }
            else
            {
                providerString = sqlBuilder.ToString();
            }

            entityBuilder.ProviderConnectionString = providerString;

            entityBuilder.Metadata = @"res://*/eTurnsMaster.csdl|res://*/eTurnsMaster.ssdl|res://*/eTurnsMaster.msl";
            return entityBuilder.ToString();
        }
        public static string GeteTurnsMasterSQLConnectionString(string DataBaseName, string ConnectionType)
        {
            string providerName = ConfigurationManager.AppSettings.AllKeys.Contains("DBproviderName") ? Convert.ToString(ConfigurationManager.AppSettings["DBproviderName"]) : "System.Data.SqlClient";
            string DBserverName = ConfigurationManager.AppSettings.AllKeys.Contains("DBserverName") ? Convert.ToString(ConfigurationManager.AppSettings["DBserverName"]) : "DESKTOP-LV6DASV";
            string DBName = DataBaseName != null ? Convert.ToString(DataBaseName) : GetETurnsMasterDBName();
            string DbUserName = ConfigurationManager.AppSettings.AllKeys.Contains("DbUserName") ? Convert.ToString(ConfigurationManager.AppSettings["DbUserName"]) : "sa";
            string DbPassword = ConfigurationManager.AppSettings.AllKeys.Contains("DbPassword") ? Convert.ToString(ConfigurationManager.AppSettings["DbPassword"]) : "ibm@@123";
            string DBFailoverPartner = ConfigurationManager.AppSettings.AllKeys.Contains("DBFailoverPartner") ? Convert.ToString(ConfigurationManager.AppSettings["DBFailoverPartner"]) : "172.31.12.215";
            string IsWSFC = Convert.ToString(ConfigurationManager.AppSettings["IsWSFC"]) ?? "no";
            string WSFCLSNR = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNR"]) ?? "LSNPRD1";
            string WSFCLSNRPORT = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNRPORT"]) ?? "1433";
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DBserverName;
            sqlBuilder.InitialCatalog = DBName;
            sqlBuilder.UserID = DbUserName;
            sqlBuilder.Password = DbPassword;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.FailoverPartner = DBFailoverPartner;
            string providerString = sqlBuilder.ToString();
            //sqlBuilder.PersistSecurityInfo = true;           
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = providerName;

            if (IsWSFC == "yes")
            {
                providerString = GetOledbConnection(ConnectionType, DBName);
            }
            else
            {
                providerString = sqlBuilder.ToString();
            }
            return providerString;
        }
        public static string GetOledbConnection(string ConnectionType, string DatabaseName)
        {
            string IsWSFC = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["IsWSFC"]) ?? "no";
            if (IsWSFC == "yes")
            {
                System.Data.OleDb.OleDbConnectionStringBuilder objOleDbConnectionStringBuilder = new System.Data.OleDb.OleDbConnectionStringBuilder();
                CommonMasterDAL objCommonDAL = new CommonMasterDAL();
                string ProviderConnectionString = string.Empty;
                OLEDBConnectionInfo objOLEDBConnectionInfo = objCommonDAL.GetAllConnectionparams().FirstOrDefault(t => t.ConectionType == ConnectionType);

                if (objOLEDBConnectionInfo != null)
                {
                    if (!string.IsNullOrWhiteSpace(objOLEDBConnectionInfo.APP))
                    {
                        objOleDbConnectionStringBuilder.Add("APP", objOLEDBConnectionInfo.APP);
                    }
                    if (!string.IsNullOrWhiteSpace(objOLEDBConnectionInfo.ApplicationIntent))
                    {
                        objOleDbConnectionStringBuilder.Add("ApplicationIntent", objOLEDBConnectionInfo.ApplicationIntent);
                    }
                    if (!string.IsNullOrWhiteSpace(DatabaseName))
                    {
                        objOleDbConnectionStringBuilder.Add("Database", DatabaseName);
                    }
                    if (!string.IsNullOrWhiteSpace(objOLEDBConnectionInfo.MarsConn))
                    {
                        objOleDbConnectionStringBuilder.Add("multipleactiveresultsets", objOLEDBConnectionInfo.MarsConn);
                    }
                    if (!string.IsNullOrWhiteSpace(objOLEDBConnectionInfo.PacketSize))
                    {
                        objOleDbConnectionStringBuilder.Add("PacketSize", objOLEDBConnectionInfo.PacketSize);
                    }
                    if (!string.IsNullOrWhiteSpace(objOLEDBConnectionInfo.PWD))
                    {
                        objOleDbConnectionStringBuilder.Add("PWD", objOLEDBConnectionInfo.PWD);
                    }
                    if (!string.IsNullOrWhiteSpace(objOLEDBConnectionInfo.Server))
                    {
                        objOleDbConnectionStringBuilder.Add("Server", objOLEDBConnectionInfo.Server);
                    }
                    if (!string.IsNullOrWhiteSpace(objOLEDBConnectionInfo.Timeout))
                    {
                        objOleDbConnectionStringBuilder.Add("Timeout", objOLEDBConnectionInfo.Timeout);
                    }
                    if (!string.IsNullOrWhiteSpace(objOLEDBConnectionInfo.Trusted_Connection))
                    {
                        objOleDbConnectionStringBuilder.Add("Trusted_Connection", objOLEDBConnectionInfo.Trusted_Connection);
                    }
                    if (!string.IsNullOrWhiteSpace(objOLEDBConnectionInfo.UID))
                    {
                        objOleDbConnectionStringBuilder.Add("UID", objOLEDBConnectionInfo.UID);
                    }
                    if (!string.IsNullOrWhiteSpace(objOLEDBConnectionInfo.FailoverPartner))
                    {
                        objOleDbConnectionStringBuilder.Add("FailoverPartner", objOLEDBConnectionInfo.FailoverPartner);
                    }
                    if (!string.IsNullOrWhiteSpace(objOLEDBConnectionInfo.PersistSensitive))
                    {
                        objOleDbConnectionStringBuilder.Add("PersistSensitive", objOLEDBConnectionInfo.PersistSensitive);
                    }
                    if (!string.IsNullOrWhiteSpace(objOLEDBConnectionInfo.MultiSubnetFailover))
                    {
                        objOleDbConnectionStringBuilder.Add("MultiSubnetFailover", objOLEDBConnectionInfo.MultiSubnetFailover);
                    }
                    return objOleDbConnectionStringBuilder.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                string providerName = ConfigurationManager.AppSettings.AllKeys.Contains("DBproviderName") ? Convert.ToString(ConfigurationManager.AppSettings["DBproviderName"]) : "System.Data.SqlClient";
                string DBserverName = ConfigurationManager.AppSettings.AllKeys.Contains("DBserverName") ? Convert.ToString(ConfigurationManager.AppSettings["DBserverName"]) : "DESKTOP-LV6DASV";
                string DbUserName = ConfigurationManager.AppSettings.AllKeys.Contains("DbUserName") ? Convert.ToString(ConfigurationManager.AppSettings["DbUserName"]) : "sa";
                string DbPassword = ConfigurationManager.AppSettings.AllKeys.Contains("DbPassword") ? Convert.ToString(ConfigurationManager.AppSettings["DbPassword"]) : "ibm@@123";
                string DBFailoverPartner = ConfigurationManager.AppSettings.AllKeys.Contains("DBFailoverPartner") ? Convert.ToString(ConfigurationManager.AppSettings["DBFailoverPartner"]) : "172.31.12.215";
                SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
                sqlBuilder.DataSource = DBserverName;
                sqlBuilder.UserID = DbUserName;
                sqlBuilder.Password = DbPassword;
                sqlBuilder.FailoverPartner = DBFailoverPartner;
                sqlBuilder.InitialCatalog = DatabaseName;
                sqlBuilder.MultipleActiveResultSets = true;
                return sqlBuilder.ToString();
            }
        }

        public static string GeteTurnsDBName()
        {
            try
            {
                return ConfigurationManager.AppSettings["TemplateDbName"].ToString();
            }
            catch
            {

                return "eTurns";
            }
        }

        public static string GetETurnsMasterDBName()
        {
            try
            {
                return ConfigurationManager.AppSettings["MasterDBName"].ToString();
            }
            catch
            {
                return "eTurnsMaster";
            }
        }

        public static string GeteTurnsScheduleDBName()
        {
            try
            {
                return ConfigurationManager.AppSettings["eTurnsScheduleDBName"].ToString();
            }
            catch
            {
                return "eTurnsSchedule";
            }
        }
        public static string GeteTurnsLoggingDBName()
        {
            try
            {
                return ConfigurationManager.AppSettings["eTurnsLoggingDBName"].ToString();
            }
            catch
            {
                return "eTurnsLogging";
            }
        }
        public static string GeteTurnsImportDBName()
        {
            try
            {
                return ConfigurationManager.AppSettings["ImportDBName"].ToString();
            }
            catch
            {
                return "eTurnsImport";
            }
        }
        public static string GeteTurnsHistoryDBName()
        {
            try
            {
                return ConfigurationManager.AppSettings["eTurnsHistoryDBName"].ToString();
            }
            catch
            {
                return "eTurnsHistory";
            }
        }
        public static string GeteTurnsEmailsDBName()
        {
            try
            {
                return ConfigurationManager.AppSettings["eTurnsEMailDBName"].ToString();
            }
            catch
            {
                return "eTurnsEmails";
            }
        }

        public static string GeteTurnsChangeLogDBName()
        {
            try
            {
                return ConfigurationManager.AppSettings["eTurnsChangeLogDBName"].ToString();
            }
            catch
            {
                return "eTurnsChangeLog";
            }
        }
        public static string GeteTurnsArchiveDBName()
        {
            try
            {
                return ConfigurationManager.AppSettings["eTurnsArchiveDBName"].ToString();
            }
            catch
            {
                return "eTurnsArchive";
            }
        }

        public static string GeteTurnsResourceDBName()
        {
            try
            {
                return ConfigurationManager.AppSettings["eTurnsResourceDBName"].ToString();
            }
            catch
            {
                return "eTurnsResource";
            }
        }

    }

}
