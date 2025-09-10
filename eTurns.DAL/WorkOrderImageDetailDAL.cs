using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class WorkOrderImageDetailDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public WorkOrderImageDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        #endregion

        #region [Class Methods]
        public List<WorkOrderImageDetailDTO> GetWorkOrderImageDetailByWOGUID(Guid WorkOrderGuid, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WorkOrderGuid", WorkOrderGuid), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderImageDetailDTO>("exec [GetWorkOrderImageDetailByWOGUID] @WorkOrderGuid,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public Int64 Insert(WorkOrderImageDetailDTO objDTO)
        {

            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                WorkOrderImageDetail obj = new WorkOrderImageDetail();
                obj.ID = 0;
                obj.WOImageName = objDTO.WOImageName;

                obj.Created = DateTimeUtility.DateTimeNow;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.RoomId = objDTO.RoomId;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.WorkOrderGuid = objDTO.WorkOrderGUID;
                obj.CompanyId = objDTO.CompanyID;
                obj.Guid = Guid.NewGuid();
                obj.WhatWhereAction = "From web";
                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                if (string.IsNullOrWhiteSpace(objDTO.AddedFrom))
                    obj.AddedFrom = objDTO.AddedFrom = "Web";
                obj.AddedFrom = objDTO.AddedFrom;
                if (string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                    obj.EditedFrom = objDTO.EditedFrom = "Web";
                obj.EditedFrom = objDTO.EditedFrom;
                context.WorkOrderImageDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.Guid.GetValueOrDefault(Guid.Empty);
                return obj.ID;
            }
        }

        public void DeleteRecords(string IDs, long UserId, Guid WorkOrderGuid, string WhatWhereAction = "")
        {
            if (string.IsNullOrEmpty(WhatWhereAction))
            {
                WhatWhereAction = "From Web";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { 
                                                    new SqlParameter("@GUIDs", IDs), 
                                                    new SqlParameter("@UserId", UserId), 
                                                    new SqlParameter("@WorkOrderGuid", WorkOrderGuid), 
                                                    new SqlParameter("@WhatWhereAction", WhatWhereAction) 
                                                };
                context.Database.ExecuteSqlCommand("exec [DeleteWorkOrderImageDetail] @GUIDs, @UserId,@WorkOrderGuid,@WhatWhereAction", params1);
            }
        }

        public string DeleteSingleRecord(string WhatWhereAction, long UserId, string guid, Guid WorkOrderGUID)
        {
            DeleteRecords(guid, UserId, WorkOrderGUID, WhatWhereAction);
            return "success";
        }
        public WorkOrderImageDetailDTO GetWOImageByGUID(Guid imageGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@imageGUID", imageGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderImageDetailDTO>("exec [GetWOImageByGUID] @imageGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public WorkOrderImageDetailDTO GetWOImageByImageName(Guid WOGUID, string WOimageName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WOGUID", WOGUID), new SqlParameter("@WOimageName", WOimageName), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderImageDetailDTO>("exec [GetWOImageByImageName] @WOGUID,@WOimageName,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public List<WorkOrderImageDetailDTO> GetWorkOrderAllImageDetailByWOGUID(Guid WorkOrderGuid, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@WorkOrderGuid", WorkOrderGuid), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WorkOrderImageDetailDTO>("exec [GetWorkOrderAllImageDetailByWOGUID] @WorkOrderGuid,@RoomID,@CompanyID", params1).ToList();
            }
        }

        #endregion
    }
}


