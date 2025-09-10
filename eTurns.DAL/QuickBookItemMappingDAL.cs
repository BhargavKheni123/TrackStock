using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Data.SqlClient;
using eTurns.DTO.Resources;
using System.Web;
using System.Transactions;


namespace eTurns.DAL
{
    public class QuickBookItemMappingDAL : eTurnsBaseDAL
    {
        public QuickBookItemMappingDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public List<QuickBookItemMappingDTO> GetQuickBookItemMappingByItemGUID(Guid ItemGUID, Int64 CompanyID, Int64 RoomID)
        {
            var params1 = new SqlParameter[] {
                    new SqlParameter("@ItemGUID", ItemGUID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<QuickBookItemMappingDTO> obj = (from u in context.Database.SqlQuery<QuickBookItemMappingDTO>("Exec [GetQuickBookItemMappingByItemGUID] @ItemGUID,@CompanyID,@RoomID", params1)
                                                     select new QuickBookItemMappingDTO
                                                     {
                                                         ID = u.ID,
                                                         GUID = u.GUID,
                                                         ItemGUID = u.ItemGUID,
                                                         QuickBookItemID = u.QuickBookItemID,
                                                         CompanyID = u.CompanyID,
                                                         RoomID = u.RoomID,
                                                         IsDeleted = u.IsDeleted,
                                                         IsArchived = u.IsArchived,
                                                         Created = u.Created,
                                                         LastUpdated = u.LastUpdated,
                                                         CreatedBy = u.CreatedBy,
                                                         LastUpdatedBy = u.LastUpdatedBy,
                                                         AddedFrom = u.AddedFrom,
                                                         EditedFrom = u.EditedFrom,
                                                         ItemNumber = u.ItemNumber
                                                     }).ToList();
                return obj;
            }

        }


    }
}
