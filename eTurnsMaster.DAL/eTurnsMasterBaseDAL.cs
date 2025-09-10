using System;
using System.Linq;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Data.EntityClient;

namespace eTurnsMaster.DAL
{
    public class eTurnsMasterBaseDAL
    {
        private string _DataBaseName = string.Empty;
        private string _DBServerName = string.Empty;
        private string _DBUserName = string.Empty;
        private string _DBPassword = string.Empty;
        private string _DBproviderName = string.Empty;
        private string _FailoverPartner = string.Empty;
        private string _IsWSFC = string.Empty;
        private string _WSFCLSNR = string.Empty;
        private string _WSFCLSNRPORT = string.Empty;
        private string _MultiSubFailover = string.Empty;
        private string _AppIntent = string.Empty;
        private string _ReportAppIntent = string.Empty;
        public eTurnsMasterBaseDAL()
        {
            _DataBaseName = MasterDbConnectionHelper.GetETurnsMasterDBName();
            _DBServerName = Convert.ToString(ConfigurationManager.AppSettings["DBserverName"]);
            _DBUserName = Convert.ToString(ConfigurationManager.AppSettings["DbUserName"]);
            _DBPassword = Convert.ToString(ConfigurationManager.AppSettings["DbPassword"]);
            _DBproviderName = Convert.ToString(ConfigurationManager.AppSettings["DBproviderName"]);
            _FailoverPartner = Convert.ToString(ConfigurationManager.AppSettings["DBFailoverPartner"]) ?? "172.31.12.215";
            _IsWSFC = Convert.ToString(ConfigurationManager.AppSettings["IsWSFC"]) ?? "no";
            _WSFCLSNR = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNR"]) ?? "LSNPRD1";
            _WSFCLSNRPORT = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNRPORT"]) ?? "1433";
            _MultiSubFailover = Convert.ToString(ConfigurationManager.AppSettings["MultiSubFailover"]) ?? "True";
            _AppIntent = Convert.ToString(ConfigurationManager.AppSettings["AppIntent"]) ?? "ReadWrite";
            _ReportAppIntent = Convert.ToString(ConfigurationManager.AppSettings["ReportAppIntent"]) ?? "ReadOnly";
        }
        public string FailoverPartner
        {
            get
            {
                return _FailoverPartner;
            }
            set
            {
                _FailoverPartner = value;
            }
        }
        public string DataBaseName
        {
            get
            {
                return _DataBaseName;
            }
            set
            {
                _DataBaseName = value;
            }
        }
        public string DBServerName
        {
            get
            {
                return _DBServerName;
            }
            set
            {
                _DBServerName = value;
            }
        }
        public string DBUserName
        {
            get
            {
                return _DBUserName;
            }
            set
            {
                _DBUserName = value;
            }
        }
        public string DBPassword
        {
            get
            {
                return _DBPassword;
            }
            set
            {
                _DBPassword = value;
            }
        }
        public string DBproviderName
        {
            get
            {
                return _DBproviderName;
            }
            set
            {
                _DBproviderName = value;
            }
        }
        public string IsWSFC
        {
            get
            {
                return _IsWSFC;
            }
            set
            {
                _IsWSFC = value;
            }
        }
        public string WSFCLSNR
        {
            get
            {
                return _WSFCLSNR;
            }
            set
            {
                _WSFCLSNR = value;
            }
        }
        public string WSFCLSNRPORT
        {
            get
            {
                return _WSFCLSNRPORT;
            }
            set
            {
                _WSFCLSNRPORT = value;
            }
        }
        public string MultiSubFailover
        {
            get
            {
                return _MultiSubFailover;
            }
            set
            {
                _MultiSubFailover = value;
            }
        }
        public string AppIntent
        {
            get
            {
                return _AppIntent;
            }
            set
            {
                _AppIntent = value;
            }
        }
        public string ReportAppIntent
        {
            get
            {
                return _ReportAppIntent;
            }
            set
            {
                _ReportAppIntent = value;
            }
        }
        public string DataBaseEntityConnectionString
        {
            get
            {
                return GetConnectionString();
            }
        }
        public string DataBaseEntityConnectionStringeTurns
        {
            get
            {
                return GetConnectionStringeTurns();
            }
        }
        public string DataBaseEntityConnectionStringForReport
        {
            get
            {
                return GetReportConnectionString();
            }
        }
        public string DataBaseMasterEntityConnectionString
        {
            get
            {
                return GetMasterDBConnectionString();
            }
        }
        public string DataBaseConnectionString
        {
            get
            {
                return GetSQLConnectionString();
            }
        }
        public string DataBaseETurnsMasterEntityConnectionString
        {
            get
            {
                return GetETurnsMasterDBConnectionString();
            }
        }
        private string GetSQLConnectionString()
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DBServerName;
            sqlBuilder.InitialCatalog = DataBaseName;
            sqlBuilder.UserID = DBUserName;
            sqlBuilder.Password = DBPassword;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.FailoverPartner = FailoverPartner;
            string providerString = string.Empty;

            if (IsWSFC == "yes")
            {
                providerString = MasterDbConnectionHelper.GetOledbConnection("GeneralReadWrite", DataBaseName);
            }
            else
            {
                providerString = sqlBuilder.ToString();
            }

            return providerString;
        }
        private string GetConnectionString()
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DBServerName;
            sqlBuilder.InitialCatalog = DataBaseName;
            sqlBuilder.UserID = DBUserName;
            sqlBuilder.Password = DBPassword;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.FailoverPartner = FailoverPartner;
            string providerString = sqlBuilder.ToString();
            sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = "System.Data.SqlClient";
            if (IsWSFC == "yes")
            {
                providerString = MasterDbConnectionHelper.GetOledbConnection("EFReadWrite", DataBaseName);
            }
            else
            {
                providerString = sqlBuilder.ToString();
            }

            entityBuilder.ProviderConnectionString = providerString;
            entityBuilder.Metadata = @"res://*/eTurnsMaster.csdl|res://*/eTurnsMaster.ssdl|res://*/eTurnsMaster.msl";
            return entityBuilder.ToString();
        }
        private string GetConnectionStringeTurns()
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DBServerName;
            sqlBuilder.InitialCatalog = DataBaseName;
            sqlBuilder.UserID = DBUserName;
            sqlBuilder.Password = DBPassword;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.FailoverPartner = FailoverPartner;
            string providerString = sqlBuilder.ToString();
            sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = "System.Data.SqlClient";
            providerString = sqlBuilder.ToString();
            entityBuilder.ProviderConnectionString = providerString;
            entityBuilder.Metadata = @"res://*/eTurnsMaster.csdl|res://*/eTurnsMaster.ssdl|res://*/eTurnsMaster.msl";
            return entityBuilder.ToString();
        }
        private string GetReportConnectionString()
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DBServerName;
            sqlBuilder.InitialCatalog = DataBaseName;
            sqlBuilder.UserID = DBUserName;
            sqlBuilder.Password = DBPassword;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.FailoverPartner = FailoverPartner;
            string providerString = sqlBuilder.ToString();
            sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = "System.Data.SqlClient";
            if (IsWSFC == "yes")
            {
                providerString = MasterDbConnectionHelper.GetOledbConnection("EFReadonly", DataBaseName);
            }
            else
            {
                providerString = sqlBuilder.ToString();
            }

            entityBuilder.ProviderConnectionString = providerString;
            entityBuilder.Metadata = @"res://*/eTurnsMaster.csdl|res://*/eTurnsMaster.ssdl|res://*/eTurnsMaster.msl";
            return entityBuilder.ToString();
        }
        private string GetMasterDBConnectionString()
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DBServerName;
            sqlBuilder.InitialCatalog = DataBaseName;
            sqlBuilder.UserID = DBUserName;
            sqlBuilder.Password = DBPassword;
            sqlBuilder.FailoverPartner = FailoverPartner;
            sqlBuilder.MultipleActiveResultSets = true;
            string providerString = sqlBuilder.ToString();
            sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = "System.Data.SqlClient";

            if (IsWSFC == "yes")
            {
                providerString = MasterDbConnectionHelper.GetOledbConnection("GeneralReadWrite", DataBaseName);
            }
            else
            {
                providerString = sqlBuilder.ToString();
            }

            entityBuilder.ProviderConnectionString = providerString;
            entityBuilder.Metadata = @"res://*/eTurnsMaster.csdl|res://*/eTurnsMaster.ssdl|res://*/eTurnsMaster.msl";
            return entityBuilder.ToString();
        }
        private string GetETurnsMasterDBConnectionString()
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DBServerName;
            sqlBuilder.InitialCatalog = MasterDbConnectionHelper.GetETurnsMasterDBName();
            sqlBuilder.UserID = DBUserName;
            sqlBuilder.Password = DBPassword;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.FailoverPartner = FailoverPartner;
            string providerString = sqlBuilder.ToString();
            sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = "System.Data.SqlClient";

            if (IsWSFC == "yes")
            {
                providerString = MasterDbConnectionHelper.GetOledbConnection("GeneralReadWrite", MasterDbConnectionHelper.GetETurnsMasterDBName());
            }
            else
            {
                providerString = sqlBuilder.ToString();
            }


            entityBuilder.ProviderConnectionString = providerString;
            entityBuilder.Metadata = @"res://*/eTurnsMaster.csdl|res://*/eTurnsMaster.ssdl|res://*/eTurnsMaster.msl";
            return entityBuilder.ToString();
        }
        public string GetEmailDBConnectionString
        {
            get
            {
                string providerString = string.Empty;
                string dbName = ConfigurationManager.AppSettings.AllKeys.Contains("eTurnsEMailDBName") ? Convert.ToString(ConfigurationManager.AppSettings["eTurnsEMailDBName"]) : "eTurnsEmails";
                SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();

                sqlBuilder.DataSource = DBServerName;
                sqlBuilder.InitialCatalog = dbName;
                sqlBuilder.UserID = DBUserName;
                sqlBuilder.Password = DBPassword;
                sqlBuilder.FailoverPartner = FailoverPartner;
                sqlBuilder.MultipleActiveResultSets = true;
                if (IsWSFC == "yes")
                {
                    providerString = MasterDbConnectionHelper.GetOledbConnection("GeneralReadWrite", dbName);
                }
                else
                {
                    providerString = sqlBuilder.ToString();
                }

                sqlBuilder.PersistSecurityInfo = true;
                //sqlBuilder.MultipleActiveResultSets = true;
                sqlBuilder.ApplicationName = "EntityFramework";
                EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
                entityBuilder.Provider = "System.Data.SqlClient";
                entityBuilder.ProviderConnectionString = providerString;
                entityBuilder.Metadata = @"res://*/eTurnsMaster.csdl|res://*/eTurnsMaster.ssdl|res://*/eTurnsMaster.msl";
                return entityBuilder.ToString();

            }
        }

    }
}
