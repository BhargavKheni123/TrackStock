using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class ToolMaintenanceDetailsDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]



        public ToolMaintenanceDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolMaintenanceDetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public List<ToolMaintenanceDetailsDTO> GetMaintenanceLineItemsBymntsGUID(Guid MaintenanceGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@MaintenanceGUID", MaintenanceGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMaintenanceDetailsDTO>("exec [GetMaintenanceLineItemsBymntsGUID] @MaintenanceGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<ToolMaintenanceDetailsDTO> GetToolMaintenanceDetailsByMaintenanceIdNormal(long MaintenanceId, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", MaintenanceId),
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyID)
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolMaintenanceDetailsDTO>("exec [GetToolMaintenanceDetailsByMIdNormal] @ID,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public Int64 Insert(ToolMaintenanceDetailsDTO objDTO)
        {

            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolMaintenanceDetail obj = new ToolMaintenanceDetail();
                obj.ID = 0;
                obj.MaintenanceGUID = objDTO.MaintenanceGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.ItemCost = objDTO.ItemCost;
                obj.Quantity = objDTO.Quantity;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                objDTO.GUID = Guid.NewGuid();
                obj.GUID = objDTO.GUID;
                context.ToolMaintenanceDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                return obj.ID;
            }

        }

        public bool Edit(ToolMaintenanceDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolMaintenanceDetail obj = new ToolMaintenanceDetail();
                obj.ID = objDTO.ID;

                obj.MaintenanceGUID = objDTO.MaintenanceGUID;

                obj.ItemGUID = objDTO.ItemGUID;
                obj.ItemCost = objDTO.ItemCost;
                obj.Quantity = objDTO.Quantity;
                obj.Created = objDTO.Created;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.GUID = objDTO.GUID;
                context.ToolMaintenanceDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }

        public bool DeleteToolMaintenanceDetailsByids(string IDs, long UserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string Ids = string.Empty;

                if (!string.IsNullOrEmpty(IDs) && !string.IsNullOrWhiteSpace(IDs))
                {
                    var arr = IDs.Split(',');

                    if (arr != null && arr.Any())
                    {
                        Ids = string.Join(",", arr);
                    }

                    var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserId),
                                                    new SqlParameter("@Ids", Ids)
                                                };

                    context.Database.ExecuteSqlCommand("exec [DeleteToolMaintenanceDetailsByids] @UserID,@Ids", params1);
                }


                return true;
            }
        }

        public List<ToolMaintenanceDetailsDTO> GetMainteNanceDetails(Guid MaintenanceGUID)
        {
            List<ToolMaintenanceDetailsDTO> lstDetils = new List<ToolMaintenanceDetailsDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstDetils = (from tm in context.ToolsMaintenances
                             join wo in context.WorkOrders on tm.WorkorderGUID equals wo.GUID
                             join pm in context.PullMasters on wo.GUID equals pm.WorkOrderDetailGUID
                             join im in context.ItemMasters on pm.ItemGUID equals im.GUID
                             where tm.GUID == MaintenanceGUID && pm.IsDeleted != true
                             group new { tm, wo, pm, im } by new { pm.ItemGUID, im.ItemNumber, pm.UDF1, pm.UDF2, pm.UDF3, pm.UDF4, pm.UDF5 } into GroupedView
                             select new ToolMaintenanceDetailsDTO
                             {
                                 ItemGUID = GroupedView.Key.ItemGUID,
                                 ItemNumber = GroupedView.Key.ItemNumber,
                                 ItemCost = GroupedView.Sum(t => (t.pm.PULLCost ?? 0)),
                                 Quantity = GroupedView.Sum(t => (t.pm.PoolQuantity ?? 0)),
                                 PullUDF1 = GroupedView.Key.UDF1,
                                 PullUDF2 = GroupedView.Key.UDF2,
                                 PullUDF3 = GroupedView.Key.UDF3,
                                 PullUDF4 = GroupedView.Key.UDF4,
                                 PullUDF5 = GroupedView.Key.UDF5,
                             }).ToList();
            }
            return lstDetils;

        }

        public List<PastMaintenanceDueImport> PastMaintenanceDueExportData(string GUIDs, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUIDs", GUIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<PastMaintenanceDueImport>("exec [GetPastMaintenanceDueDataForExport] @RoomID,@CompanyID,@GUIDs", params1).ToList();
            }
        }
        #endregion
    }
}


