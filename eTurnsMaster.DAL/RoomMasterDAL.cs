using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace eTurnsMaster.DAL
{
    public class RoomMasterDAL : eTurnsMasterBaseDAL
    {
        public List<RoomDTO> GetAllRooms(string UserRooms, string EnterpriseIds)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IsDeleted", false), new SqlParameter("@IsArchived", false), new SqlParameter("@UserRooms", UserRooms ?? (object)DBNull.Value), new SqlParameter("@EnterpriseIds", EnterpriseIds ?? (object)DBNull.Value) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("EXEC dbo.GetAllRooms @IsDeleted,@IsArchived,@UserRooms,@EnterpriseIds", params1).ToList();

            }

        }
        public List<RoomDTO> GetRoomsByOrderToBeClose()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("exec [GetRoomsByOrderToBeClose]").ToList();
            }
        }
    }
}
