using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace eTurnsMaster.DAL
{
    public class SSOUsersMapDAL : eTurnsMasterBaseDAL
    {
        public SSOUsersMapDTO GetSSOUserMap(
            string SSOClient,
            string SSOType,
            string SSOUserEmail
            )
        {
            var params1 = new SqlParameter[] { new SqlParameter("@SSOClient", SSOClient),
                new SqlParameter("@SSOType", SSOType),
                new SqlParameter("@SSOUserEmail", SSOUserEmail)
            };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SSOUsersMapDTO>("exec [GetSSOUserMap] " + params1.ToSQLParaNameCSV(), params1).FirstOrDefault();
            }
        }

        public SSOAttributeMapDTO GetSSOAttributeMap()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {             };
                return context.Database.SqlQuery<SSOAttributeMapDTO>("exec [GetSSOAttributeMap] ", params1).FirstOrDefault();
            }
        }
        public SSOUsersMapDTO GetSSOUserMapWithWebSVC(
            string SSOUserEmail
            )
        {
            var params1 = new SqlParameter[] {
                new SqlParameter("@SSOUserEmail", SSOUserEmail)
            };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SSOUsersMapDTO>("exec [GetSSOUserMapWithWebSVC] " + params1.ToSQLParaNameCSV(), params1).FirstOrDefault();
            }
        }
    }// class
}
