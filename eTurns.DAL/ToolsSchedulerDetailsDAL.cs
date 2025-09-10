using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class ToolsSchedulerDetailsDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public ToolsSchedulerDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolsSchedulerDetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public List<ToolsSchedulerDetailsDTO> GetToolScheduleLineItemsNormal(Guid ScheduleGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ScheduleGUID", ScheduleGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolsSchedulerDetailsDTO>("exec [GetToolScheduleLineItemsNormal] @ScheduleGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<ToolsSchedulerDetailsDTO> GetToolScheduleLineItemByItemGUIDNormal(Guid ScheduleGUID, Guid ItemGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ScheduleGUID", ScheduleGUID), new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolsSchedulerDetailsDTO>("exec [GetToolScheduleLineItemByItemGUIDNormal] @ScheduleGUID,@ItemGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public ToolsSchedulerDetailsDTO GetToolScheduleLineItemByGUIDNormal(Guid LineItemGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@LineItemGUID", LineItemGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolsSchedulerDetailsDTO>("exec [GetToolScheduleLineItemByGUIDNormal] @LineItemGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public ToolsSchedulerDetailsDTO GetToolScheduleLineItemByGUIDPlain(Guid LineItemGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@LineItemGUID", LineItemGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolsSchedulerDetailsDTO>("exec [GetToolScheduleLineItemByGUIDPlain] @LineItemGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public Int64 Insert(ToolsSchedulerDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolsSchedulerDetail obj = new ToolsSchedulerDetail();
                obj.ID = 0;

                obj.ScheduleGUID = objDTO.ScheduleGUID;

                obj.ItemGUID = objDTO.ItemGUID;
                obj.ItemCost = objDTO.ItemCost;
                obj.Quantity = objDTO.Quantity;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                objDTO.GUID = Guid.NewGuid();
                obj.GUID = objDTO.GUID;
                context.ToolsSchedulerDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                return obj.ID;
            }

        }
        public bool Edit(ToolsSchedulerDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolsSchedulerDetail obj = new ToolsSchedulerDetail();
                obj.GUID = objDTO.GUID;

                ToolsSchedulerDetailsDTO DTO1 = GetToolScheduleLineItemByGUIDPlain(objDTO.GUID, objDTO.Room, objDTO.CompanyID);
                obj.ID = DTO1.ID;
                obj.CompanyID = DTO1.CompanyID;
                obj.Room = DTO1.Room;
                obj.Created = DTO1.Created;
                obj.CreatedBy = DTO1.CreatedBy;
                obj.GUID = DTO1.GUID;
                obj.IsDeleted = DTO1.IsDeleted;
                obj.IsArchived = DTO1.IsArchived;
                obj.ScheduleGUID = objDTO.ScheduleGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.ItemCost = objDTO.ItemCost;
                obj.Quantity = objDTO.Quantity;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.LastUpdated = objDTO.LastUpdated;
                context.ToolsSchedulerDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }
        public bool EditForImport(ToolsSchedulerDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolsSchedulerDetail obj = new ToolsSchedulerDetail();
                obj.GUID = objDTO.GUID;

                ToolsSchedulerDetailsDTO DTO1 = GetToolScheduleLineItemByGUIDPlain(objDTO.GUID, objDTO.Room, objDTO.CompanyID);
                obj.ID = DTO1.ID;
                obj.CompanyID = DTO1.CompanyID;
                obj.Room = DTO1.Room;
                obj.Created = DTO1.Created;
                obj.CreatedBy = DTO1.CreatedBy;
                obj.GUID = DTO1.GUID;
                obj.IsDeleted = objDTO.IsDeleted;
                obj.IsArchived = DTO1.IsArchived;
                obj.ScheduleGUID = objDTO.ScheduleGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.ItemCost = objDTO.ItemCost;
                obj.Quantity = objDTO.Quantity;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.LastUpdated = objDTO.LastUpdated;
                context.ToolsSchedulerDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
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
                        strQuery += "UPDATE ToolsSchedulerDetails SET LastUpdated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE GUID ='" + item.ToString() + "';";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);
                return true;
            }
        }

        #endregion

    }
}


