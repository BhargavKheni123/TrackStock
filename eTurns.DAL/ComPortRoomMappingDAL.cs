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
    public partial class ComPortRoomMappingDAL : eTurnsBaseDAL
    {
        public ComPortRoomMappingDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public List<ComPortRoomMappingDTO> GetComPortMappingByCompanyRoomID(Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID) };
                return context.Database.SqlQuery<ComPortRoomMappingDTO>("exec [GetComPortMappingByCompanyRoomID] @CompanyID,@RoomID", params1).ToList();
            }
        }
        public ComPortRoomMappingDTO GetComPortMappingByID(long ID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
                return context.Database.SqlQuery<ComPortRoomMappingDTO>("exec [GetComPortMappingByID] @ID", params1).FirstOrDefault();
            }
        }

        public bool InsertComPortmapping(ComPortRoomMappingDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@SelectedComPortMasterIDs", objDTO.SelectedComPortMasterIDs),
                                                   new SqlParameter("@CompanyID", objDTO.CompanyID),
                                                   new SqlParameter("@RoomID", objDTO.RoomID),
                                                   new SqlParameter("@CreatedBy", objDTO.CreatedBy) };

                //context.Database.SqlQuery<ComPortRoomMappingDTO>("exec []", params1);
                context.Database.ExecuteSqlCommand("EXEC [InsertComPortmapping] @SelectedComPortMasterIDs,@CompanyID,@RoomID,@CreatedBy", params1);
                return true;
            }
        }

        public bool ValidateComPortMapping(string ComPortIds, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ComPortIds", ComPortIds),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                };
                int iCount = context.Database.SqlQuery<int>("EXEC [ValidateComPortmapping] @ComPortIds,@RoomID,@CompanyID", params1).FirstOrDefault();
                if (iCount > 0)
                    return false;
                else
                    return true;
            }
        }


    }
}
