using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using System.Web;
using eTurns.DTO.Resources;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class ComPortMasterDAL : eTurnsBaseDAL
    {
        public ComPortMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public List<ComPortMasterDTO> GetComPortMaster()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ComPortMasterDTO>("exec [GetComPortMaster]").ToList();
            }
        }

        public List<ComPortMasterDTO> GetComPortMasterByCompanyRoom(Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID) };
                return context.Database.SqlQuery<ComPortMasterDTO>("exec [GetComPortMasterByCompanyRoom] @CompanyID,@RoomID", params1).ToList();
            }
        }

        public ComPortMasterDTO GetComPortMasterByRoom(Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID) };
                return context.Database.SqlQuery<ComPortMasterDTO>("exec [GetComPortMasterByCompanyIDRoomID] @CompanyID,@RoomID", params1).FirstOrDefault();
            }
        }


    }
}
