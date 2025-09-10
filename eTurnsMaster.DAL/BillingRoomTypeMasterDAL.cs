using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace eTurnsMaster.DAL
{
    public class BillingRoomTypeMasterDAL : eTurnsMasterBaseDAL
    {

        public List<BillingRoomTypeMasterDTO> GetBillingRoomTypeMaster(long EntId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] para = new SqlParameter[] {
                    new SqlParameter("@EnterpriseID",EntId)
                };

                List<BillingRoomTypeMasterDTO> list = context.Database.SqlQuery<BillingRoomTypeMasterDTO>
                    ("exec [GetBillingRoomTypeMaster] " + para.ToSQLParaNameCSV(), para).ToList();

                return list;
            }
        }

        public List<BillingRoomTypeMasterDTO> GetAllBillingRoomTypeResourceKeys()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                List<BillingRoomTypeMasterDTO> list = context.Database.SqlQuery<BillingRoomTypeMasterDTO>("exec [GetAllBillingRoomTypeResourceKeys]").ToList();
                return list;
            }
        }

        public BillingRoomTypeMasterDTO GetBillingRoomTypeMasterByName(string ZohoProductName, long EnterpriseId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ZohoProductName", ZohoProductName), new SqlParameter("@EnterpriseID", EnterpriseId) };
                return context.Database.SqlQuery<BillingRoomTypeMasterDTO>("exec [GetBillingRoomTypeMasterByName] " + para.ToSQLParaNameCSV(), para).FirstOrDefault();
            }
        }

        public bool MigrateNewBillingRoomTypesForNewEnterprise(long EnterpriseId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] para = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseId) };

                int status = context.Database.SqlQuery<int>("exec [MigrateNewBillingRoomTypesForNewEnterprise] @EnterpriseID", para).FirstOrDefault();
                return status >= 0;
            }
        }
    }
}
