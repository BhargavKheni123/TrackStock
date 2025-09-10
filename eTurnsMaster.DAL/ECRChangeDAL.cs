using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTurns.DTO;
namespace eTurnsMaster.DAL
{
    public class ECRChangeDAL : eTurnsMasterBaseDAL
    {
        public List<ECRChangesDTO> FetchECRChangesForProcessing()
        {
            var params1 = new SqlParameter[] { };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ECRChangesDTO>("exec [FetchECRChangesForProcessing]", params1).ToList();
            }
        }
        public ECRChangesDTO SetECRChangeCompletedWithSuccess(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ECRChangesDTO>("exec [SetECRChangeCompletedWithSuccess] @ID", params1).FirstOrDefault();
            }
        }
        public ECRChangesDTO SetECRChangeCompletedWithError(long ID, string ErrorException)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@ErrorException", ErrorException ?? (object)DBNull.Value) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ECRChangesDTO>("exec [SetECRChangeCompletedWithError] @ID,@ErrorException", params1).FirstOrDefault();
            }
        }
        public ECRChangesDTO UpdateECRWithException(long ID, string ErrorException)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@ErrorException", ErrorException ?? (object)DBNull.Value) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ECRChangesDTO>("exec [UpdateECRWithException] @ID,@ErrorException", params1).FirstOrDefault();
            }
        }
    }
}
