using System;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class RoomReferenceDeleteProcessDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public RoomReferenceDeleteProcessDAL(base.DataBaseName)
        //{

        //}

        public RoomReferenceDeleteProcessDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public RoomReferenceDeleteProcessDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public int DeleteReferencesForRoom(string RoomIds, Int64 EnterpriseId, Int64 CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomIds", RoomIds), new SqlParameter("@EnterpriseId", EnterpriseId), new SqlParameter("@CompanyId", CompanyId) };
                int? isDelete = context.Database.SqlQuery<int?>("EXEC SoftDeleteRoomReferences @RoomIds,@EnterpriseId,@CompanyId", params1).FirstOrDefault();
                return isDelete ?? 0;
            }
        }
    }
}
