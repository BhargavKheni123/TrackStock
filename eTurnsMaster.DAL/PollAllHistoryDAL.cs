using System.Linq;
using eTurns.DTO;
using eTurnsMaster.DAL;

namespace eTurnsMaster.DAL
{
    public class PollAllHistoryDAL : eTurnsMasterBaseDAL
    {
        public void GetPollAllFailedData()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                context.ExecuteStoreQuery<RolePermissionInfo>("exec dbo.[GetFailedPollAllData]").ToList();
            }
        }
    }
}
