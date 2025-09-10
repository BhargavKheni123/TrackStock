using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class ToolMaintenanceDetailsDAL : eTurnsBaseDAL
    {
        public List<ToolMaintenanceDetailsDTO> GetRecordByMaintenanceId(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ToolMaintenanceDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, E.ItemNumber ,CM.CostUOMValue 
FROM ToolMaintenanceDetails A LEFT OUTER  JOIN 
ToolsMaintenance TM on A.MaintenanceGUID= Tm.[Guid]
 LEFT OUTER  JOIN 
ItemMaster IM on A.Itemguid= Im.[Guid]
Left Join CostUOMMaster CM on IM.CostUOMID= CM.Id LEFT OUTER  JOIN 
UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN 
UserMaster C on A.LastUpdatedBy = C.ID 
LEFT OUTER JOIN Room D on A.Room = D.ID 
LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID
WHERE isnull(A.Isdeleted,0)=0 and A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + "AND Tm.ID = " + id.ToString())
                        select new ToolMaintenanceDetailsDTO
                        {
                            ID = u.ID,
                            MaintenanceGUID = u.MaintenanceGUID,
                            ItemGUID = u.ItemGUID,
                            ItemCost = u.ItemCost,
                            Quantity = u.Quantity,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            ItemNumber = u.ItemNumber,
                            CostUOMValue = u.CostUOMValue
                        }).AsParallel().ToList();
            }

        }

        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE ToolMaintenanceDetails SET LastUpdated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);




                return true;
            }
        }
    }
}
