using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace eTurnsMaster.DAL
{
    public class RedisCacheKeyDAL : eTurnsMasterBaseDAL
    {
        #region [Class Methods]

        public List<RedisCacheKeyDTO> GetRedisCacheKeysToRemove()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { };
                return context.Database.SqlQuery<RedisCacheKeyDTO>("exec [GetRedisCacheKeysToRemove]", params1).ToList();
            }
        }
        public List<RedisCacheKeyDTO> UpdateRedisCacheKeyStatus(long Id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<RedisCacheKeyDTO>("exec [UpdateRedisCacheKeyStatus] @Id", params1).ToList();
            }
        }
        #endregion
    }
}
