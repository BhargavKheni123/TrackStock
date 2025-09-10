using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTurns.DTO;

namespace eTurnsMaster.DAL
{
    public class UsersUISettingsQueueDAL : eTurnsMasterBaseDAL
    {
        public List<UsersUISettingsQueueDTO> FetchUsersUISettingsChangesForProcessing()
        {
            var params1 = new SqlParameter[] { };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UsersUISettingsQueueDTO>("exec [FetchUsersUISettingsChangesForProcessing]", params1).ToList();
            }
        }
        public UsersUISettingsQueueDTO SetUsersUISettingsCompletedWithSuccess(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UsersUISettingsQueueDTO>("exec [SetUsersUISettingsCompletedWithSuccess] @ID", params1).FirstOrDefault();
            }
        }
        public UsersUISettingsQueueDTO SetUsersUISettingsCompletedWithError(long ID, string ErrorException)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@ErrorException", ErrorException ?? (object)DBNull.Value) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UsersUISettingsQueueDTO>("exec [SetUsersUISettingsCompletedWithError] @ID,@ErrorException", params1).FirstOrDefault();
            }
        }
        public UsersUISettingsQueueDTO UpdateUsersUISettingsWithException(long ID, string ErrorException)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@ErrorException", ErrorException ?? (object)DBNull.Value) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UsersUISettingsQueueDTO>("exec [UpdateUsersUISettingsWithException] @ID,@ErrorException", params1).FirstOrDefault();
            }
        }
    }
}
