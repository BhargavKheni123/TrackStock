using System;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class eTurnsBaseDAL : IDisposable
    {
        private string _DataBaseName = string.Empty;
        private static string _DBServerName = string.Empty;
        private static string _DBUserName = string.Empty;
        private static string _DBPassword = string.Empty;
        private static string _DBproviderName = string.Empty;
        private static string _FailoverPartner = string.Empty;
        private static string _IsWSFC = string.Empty;
        private static string _WSFCLSNR = string.Empty;
        private static string _WSFCLSNRPORT = string.Empty;
        private static string _MultiSubFailover = string.Empty;
        private static string _AppIntent = string.Empty;
        private static string _ReportAppIntent = string.Empty;

        static eTurnsBaseDAL()
        {
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


        //public eTurnsBaseDAL()
        //{
        //    if (Assembly.GetEntryAssembly() != null && HttpContext.Current == null)
        //    {
        //        //_DBServerName = Convert.ToString(ConfigurationManager.AppSettings["DBserverName"]);
        //        //_DBUserName = Convert.ToString(ConfigurationManager.AppSettings["DbUserName"]);
        //        //_DBPassword = Convert.ToString(ConfigurationManager.AppSettings["DbPassword"]);
        //        //_DBproviderName = Convert.ToString(ConfigurationManager.AppSettings["DBproviderName"]);
        //        //_FailoverPartner = Convert.ToString(ConfigurationManager.AppSettings["DBFailoverPartner"]) ?? "172.31.12.215";
        //        //_IsWSFC = Convert.ToString(ConfigurationManager.AppSettings["IsWSFC"]) ?? "no";
        //        //_WSFCLSNR = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNR"]) ?? "LSNPRD1";
        //        //_WSFCLSNRPORT = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNRPORT"]) ?? "1433";
        //        //_MultiSubFailover = Convert.ToString(ConfigurationManager.AppSettings["MultiSubFailover"]) ?? "True";
        //        //_AppIntent = Convert.ToString(ConfigurationManager.AppSettings["AppIntent"]) ?? "ReadWrite";
        //        //_ReportAppIntent = Convert.ToString(ConfigurationManager.AppSettings["ReportAppIntent"]) ?? "ReadOnly";
        //    }
        //    else
        //    {
        //        if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["EnterPriseDBName"] != null)
        //            _DataBaseName = Convert.ToString(HttpContext.Current.Session["EnterPriseDBName"]);
        //        //_DBServerName = Convert.ToString(ConfigurationManager.AppSettings["DBserverName"]);
        //        //_DBUserName = Convert.ToString(ConfigurationManager.AppSettings["DbUserName"]);
        //        //_DBPassword = Convert.ToString(ConfigurationManager.AppSettings["DbPassword"]);
        //        //_DBproviderName = Convert.ToString(ConfigurationManager.AppSettings["DBproviderName"]);
        //        //_FailoverPartner = Convert.ToString(ConfigurationManager.AppSettings["DBFailoverPartner"]) ?? "172.31.12.215";
        //        //_IsWSFC = Convert.ToString(ConfigurationManager.AppSettings["IsWSFC"]) ?? "no";
        //        //_WSFCLSNR = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNR"]) ?? "LSNPRD1";
        //        //_WSFCLSNRPORT = Convert.ToString(ConfigurationManager.AppSettings["WSFCLSNRPORT"]) ?? "1433";
        //        //_MultiSubFailover = Convert.ToString(ConfigurationManager.AppSettings["MultiSubFailover"]) ?? "True";
        //        //_AppIntent = Convert.ToString(ConfigurationManager.AppSettings["AppIntent"]) ?? "ReadWrite";
        //        //_ReportAppIntent = Convert.ToString(ConfigurationManager.AppSettings["ReportAppIntent"]) ?? "ReadOnly";
        //    }

        //}
        public static string FailoverPartner
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

        /// <summary>
        /// Non static
        /// </summary>
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
        public static string DBServerName
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
        public static string DBUserName
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
        public static string DBPassword
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
        public static string DBproviderName
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
        public static string IsWSFC
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
        public static string WSFCLSNR
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
        public static string WSFCLSNRPORT
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
        public static string MultiSubFailover
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
        public static string AppIntent
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
        public static string ReportAppIntent
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
                providerString = DbConnectionHelper.GetOledbConnection("GeneralReadWrite", DataBaseName);
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
            //sqlBuilder.FailoverPartner = FailoverPartner;
            string providerString = sqlBuilder.ToString();
            sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = "System.Data.SqlClient";
            if (IsWSFC == "yes")
            {
                providerString = DbConnectionHelper.GetOledbConnection("EFReadWrite", DataBaseName);
            }
            else
            {
                providerString = sqlBuilder.ToString();
            }

            entityBuilder.ProviderConnectionString = providerString;
            entityBuilder.Metadata = @"res://*/eTurnsEntities.csdl|res://*/eTurnsEntities.ssdl|res://*/eTurnsEntities.msl";
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
            entityBuilder.Metadata = @"res://*/eTurnsEntities.csdl|res://*/eTurnsEntities.ssdl|res://*/eTurnsEntities.msl";
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
                providerString = DbConnectionHelper.GetOledbConnection("EFReadonly", DataBaseName);
            }
            else
            {
                providerString = sqlBuilder.ToString();
            }

            entityBuilder.ProviderConnectionString = providerString;
            entityBuilder.Metadata = @"res://*/eTurnsEntities.csdl|res://*/eTurnsEntities.ssdl|res://*/eTurnsEntities.msl";
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
                providerString = DbConnectionHelper.GetOledbConnection("GeneralReadWrite", DataBaseName);
            }
            else
            {
                providerString = sqlBuilder.ToString();
            }

            entityBuilder.ProviderConnectionString = providerString;
            entityBuilder.Metadata = @"res://*/eTurnsEntities.csdl|res://*/eTurnsEntities.ssdl|res://*/eTurnsEntities.msl";
            return entityBuilder.ToString();
        }
        private string GetETurnsMasterDBConnectionString()
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DBServerName;
            sqlBuilder.InitialCatalog = DbConnectionHelper.GetETurnsMasterDBName();
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
                providerString = DbConnectionHelper.GetOledbConnection("GeneralReadWrite", DbConnectionHelper.GetETurnsMasterDBName());
            }
            else
            {
                providerString = sqlBuilder.ToString();
            }


            entityBuilder.ProviderConnectionString = providerString;
            entityBuilder.Metadata = @"res://*/eTurnsEntities.csdl|res://*/eTurnsEntities.ssdl|res://*/eTurnsEntities.msl";
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
                    providerString = DbConnectionHelper.GetOledbConnection("GeneralReadWrite", dbName);
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
                entityBuilder.Metadata = @"res://*/eTurnsEntities.csdl|res://*/eTurnsEntities.ssdl|res://*/eTurnsEntities.msl";
                return entityBuilder.ToString();

            }
        }



        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    this.DataBaseName = string.Empty;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~eTurnsBaseDAL()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        //public string GetAGListenerConnectionstring(string ServerName, string DatabaseName, string Trusted_Connection, string UserName, string Password, string MultiSubnetFailover, string ApplicationIntent)
        //{
        //    System.Data.OleDb.OleDbConnectionStringBuilder builder = new System.Data.OleDb.OleDbConnectionStringBuilder();

        //    System.Data.Odbc.OdbcConnectionStringBuilder builder = new System.Data.Odbc.OdbcConnectionStringBuilder();
        //    if (!string.IsNullOrWhiteSpace(ServerName))
        //    {
        //        builder.Add("Server", ServerName);
        //    }
        //    if (!string.IsNullOrWhiteSpace(DatabaseName))
        //    {
        //        builder.Add("Database", DatabaseName);
        //    }
        //    if (!string.IsNullOrWhiteSpace(Trusted_Connection))
        //    {
        //        builder.Add("Trusted_Connection", DatabaseName);
        //    }
        //}

    }
}
