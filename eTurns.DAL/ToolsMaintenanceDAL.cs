using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class ToolsMaintenanceDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]
        public ToolsMaintenanceDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolsMaintenanceDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]
        public IEnumerable<ToolsMaintenanceDTO> GetCachedData(Guid? AssetGUID, Guid? ToolGUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            string sSQL = "";

            if (AssetGUID != null)
            {
                sSQL += " A.AssetGUID = '" + AssetGUID.Value + "'";
            }
            else if (ToolGUID != null)
            {
                sSQL += " A.ToolGUID = '" + ToolGUID.Value + "'";
            }
            sSQL += " AND ";
            if (IsArchived == false && IsDeleted == false)
            {
                sSQL += " A.IsDeleted != 1 AND A.IsArchived != 1 ";
            }
            else if (IsArchived && IsDeleted)
            {
                sSQL += " A.IsDeleted = 1 AND A.IsArchived = 1 ";
            }
            else if (IsArchived)
            {
                sSQL += " A.IsArchived = 1 ";
            }
            else if (IsDeleted)
            {
                sSQL += " A.IsDeleted =1 ";
            }

            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Database.SqlQuery<ToolsMaintenanceDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName , E.AssetName, F.ToolName,WO.WOName,RM.RequisitionNumber as 'RequisitionName'
                        FROM ToolsMaintenance A 
                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                        LEFT OUTER JOIN AssetMaster E on A.AssetGUID = E.GUID
                        LEFT OUTER JOIN ToolMaster F on A.ToolGUID = F.GUID 
                        LEFT OUTER JOIN WorkOrder WO on A.ToolGUID = WO.GUID 
                        LEFT OUTER JOIN RequisitionMaster as RM on A.ToolGUID = RM.GUID 
                        WHERE A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND " + sSQL)
                        select new ToolsMaintenanceDTO
                        {
                            ID = u.ID,
                            MaintenanceName = u.MaintenanceName,
                            MaintenanceDate = u.MaintenanceDate,
                            SchedulerGUID = u.SchedulerGUID,
                            ScheduleDate = u.ScheduleDate,
                            TrackngMeasurement = u.TrackngMeasurement,
                            TrackingMeasurementValue = u.TrackingMeasurementValue,
                            LastMaintenanceDate = u.LastMaintenanceDate,
                            LastMeasurementValue = u.LastMeasurementValue,
                            WorkorderGUID = u.WorkorderGUID,
                            WOName = u.WOName,
                            RequisitionGUID = u.RequisitionGUID,
                            RequisitionName = u.RequisitionName,
                            Status = u.Status,
                            Created = u.Created,
                            CreatedBy = u.CreatedBy,
                            Updated = u.Updated,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsArchived = u.IsArchived,
                            IsDeleted = u.IsDeleted,
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            ToolGUID = u.ToolGUID,
                            AssetGUID = u.AssetGUID,
                            ScheduleFor = u.ScheduleFor,
                            SchedulerType = u.SchedulerType,
                            ToolSchedulerGuid = u.ToolSchedulerGuid,
                            CreatedDate = u.CreatedDate,
                            MaintenanceType = u.MaintenanceType,
                            UpdatedDate = u.UpdatedDate,
                            MappingGUID = u.MappingGUID,

                        }).AsParallel().ToList();

            }

        }
        public ToolsMaintenanceDTO GetCachedData(Guid RequisitionGUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            string sSQL = "";

            if (IsArchived == false && IsDeleted == false)
            {
                sSQL += " A.IsDeleted != 1 AND A.IsArchived != 1 ";
            }
            else if (IsArchived && IsDeleted)
            {
                sSQL += " A.IsDeleted = 1 AND A.IsArchived = 1 ";
            }
            else if (IsArchived)
            {
                sSQL += " A.IsArchived = 1 ";
            }
            else if (IsDeleted)
            {
                sSQL += " A.IsDeleted =1 ";
            }

            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Database.SqlQuery<ToolsMaintenanceDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName , E.AssetName, F.ToolName ,WO.WOName,RM.RequisitionNumber as 'RequisitionName'
                        FROM ToolsMaintenance A 
                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                        LEFT OUTER JOIN AssetMaster E on A.AssetGUID = E.GUID
                        LEFT OUTER JOIN ToolMaster F on A.ToolGUID = F.GUID 
                        LEFT OUTER JOIN WorkOrder WO on A.ToolGUID = WO.GUID 
                        LEFT OUTER JOIN RequisitionMaster as RM on A.ToolGUID = RM.GUID 
                        WHERE A.RequisitionGUID = '" + RequisitionGUID.ToString() + "' AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND " + sSQL)
                        select new ToolsMaintenanceDTO
                        {
                            ID = u.ID,
                            MaintenanceName = u.MaintenanceName,
                            MaintenanceDate = u.MaintenanceDate,
                            SchedulerGUID = u.SchedulerGUID,
                            ScheduleDate = u.ScheduleDate,
                            TrackngMeasurement = u.TrackngMeasurement,
                            TrackingMeasurementValue = u.TrackingMeasurementValue,
                            LastMaintenanceDate = u.LastMaintenanceDate,
                            LastMeasurementValue = u.LastMeasurementValue,
                            WorkorderGUID = u.WorkorderGUID,
                            WOName = u.WOName,
                            RequisitionGUID = u.RequisitionGUID,
                            RequisitionName = u.RequisitionName,
                            Status = u.Status,
                            Created = u.Created,
                            CreatedBy = u.CreatedBy,
                            Updated = u.Updated,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsArchived = u.IsArchived,
                            IsDeleted = u.IsDeleted,
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            ToolGUID = u.ToolGUID,
                            AssetGUID = u.AssetGUID,
                            ScheduleFor = u.ScheduleFor,
                            SchedulerType = u.SchedulerType,
                            ToolSchedulerGuid = u.ToolSchedulerGuid,
                            CreatedDate = u.CreatedDate,
                            MaintenanceType = u.MaintenanceType,
                            UpdatedDate = u.UpdatedDate,

                        }).SingleOrDefault();

            }

        }
        
        public IEnumerable<ToolsMaintenanceDTO> GetToolsMaintenanceByFilterNormal(Guid? AssetGUID, Guid? ToolGUID, long RoomID, long CompanyId)
        {
            var params1 = new SqlParameter[] { 
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyId),
                                               new SqlParameter("@ToolGuid", ToolGUID  ?? (object)DBNull.Value),
                                               new SqlParameter("@AssetGuid", AssetGUID  ?? (object)DBNull.Value),
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolsMaintenanceDTO>("exec [GetToolsMaintenanceByFilterNormal] @RoomID,@CompanyID,@ToolGuid,@AssetGuid", params1).ToList().OrderBy("ID DESC");
            }
        }

        public ToolsMaintenanceDTO GetToolsMaintenanceSchedulerMappingPlain(Guid? AssetGUID, Guid? ToolGUID, long RoomID, long CompanyId, Guid SchedulerGUID, Guid MappingGUID)
        {
            var params1 = new SqlParameter[] {
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyId),
                                               new SqlParameter("@SchedulerGUID", SchedulerGUID  ),
                                               new SqlParameter("@MappingGUID", MappingGUID ),
                                               new SqlParameter("@ToolGuid", ToolGUID  ?? (object)DBNull.Value),
                                               new SqlParameter("@AssetGuid", AssetGUID  ?? (object)DBNull.Value),
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolsMaintenanceDTO>("exec [GetToolsMaintenanceSchedulerMappingPlain] @RoomID,@CompanyID,@SchedulerGUID,@MappingGUID,@ToolGuid,@AssetGuid", params1).FirstOrDefault();
            }
        }

        public ToolsMaintenanceDTO GetToolsMaintenanceByIdPlain(long Id)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", Id) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolsMaintenanceDTO>("exec [GetToolsMaintenanceByIdPlain] @ID", params1).FirstOrDefault();
            }
        }

        public ToolsMaintenanceDTO GetToolsMaintenanceByIdNormal(long Id)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", Id) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolsMaintenanceDTO>("exec [GetToolsMaintenanceByIdNormal] @ID", params1).FirstOrDefault();
            }
        }

        public ToolsMaintenanceDTO GetToolsMaintenanceByGuidPlain(Guid GUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@Guid", GUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolsMaintenanceDTO>("exec [GetToolsMaintenanceByGuidPlain] @Guid", params1).FirstOrDefault();
            }
        }

        public ToolsMaintenanceDTO Insert(ToolsMaintenanceDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            if (objDTO.Created <= DateTime.MinValue)
                objDTO.Created = DateTimeUtility.DateTimeNow;
            ToolsMaintenanceDTO objToolsMaintenanceDTO = GetLastMaintenance(objDTO.ToolGUID, objDTO.AssetGUID);
            if (objToolsMaintenanceDTO != null)
            {
                objDTO.LastMaintenanceDate = objToolsMaintenanceDTO.MaintenanceDate;
                objDTO.LastMeasurementValue = objToolsMaintenanceDTO.TrackingMeasurementValue;

            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolsMaintenance obj = new ToolsMaintenance();
                obj.ID = 0;
                obj.MaintenanceName = objDTO.MaintenanceName;
                obj.MaintenanceDate = objDTO.MaintenanceDate;
                obj.SchedulerGUID = objDTO.SchedulerGUID;
                obj.ScheduleDate = objDTO.ScheduleDate;
                obj.TrackngMeasurement = objDTO.TrackngMeasurement;
                obj.TrackingMeasurementValue = objDTO.TrackingMeasurementValue;
                obj.LastMaintenanceDate = objDTO.LastMaintenanceDate;
                obj.LastMeasurementValue = objDTO.LastMeasurementValue;
                obj.WorkorderGUID = objDTO.WorkorderGUID;
                obj.RequisitionGUID = objDTO.RequisitionGUID;
                obj.Status = objDTO.Status;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.Updated = objDTO.Updated;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsArchived = objDTO.IsArchived;
                obj.IsDeleted = objDTO.IsDeleted;
                obj.GUID = Guid.NewGuid();
                obj.CompanyID = objDTO.CompanyID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.ToolGUID = objDTO.ToolGUID;
                obj.AssetGUID = objDTO.AssetGUID;
                obj.MaintenanceType = objDTO.MaintenanceType;
                obj.ScheduleFor = objDTO.ScheduleFor;
                obj.SchedulerType = objDTO.SchedulerType;
                obj.MappingGUID = objDTO.MappingGUID;

                context.ToolsMaintenances.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                if ((obj.ToolGUID ?? Guid.Empty) != Guid.Empty)
                {
                    ToolMaster objTool = context.ToolMasters.FirstOrDefault(t => t.GUID == obj.ToolGUID);
                    if (objTool != null)
                    {
                        objTool.SuggestedMaintenanceDate = obj.ScheduleDate;
                        context.SaveChanges();
                    }
                }
                if ((obj.AssetGUID ?? Guid.Empty) != Guid.Empty)
                {
                    AssetMaster objAsset = context.AssetMasters.FirstOrDefault(t => t.GUID == obj.AssetGUID);
                    if (objAsset != null)
                    {
                        objAsset.SuggestedMaintenanceDate = obj.ScheduleDate;
                        context.SaveChanges();
                    }
                }
                return objDTO;

            }


        }
        public bool Edit(ToolsMaintenanceDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                ToolsMaintenance obj = context.ToolsMaintenances.FirstOrDefault(t => t.GUID == objDTO.GUID);
                if (obj != null)
                {


                    obj.ID = objDTO.ID;
                    obj.MaintenanceName = objDTO.MaintenanceName;
                    obj.MaintenanceDate = objDTO.MaintenanceDate;
                    obj.SchedulerGUID = objDTO.SchedulerGUID;
                    obj.TrackngMeasurement = objDTO.TrackngMeasurement;
                    obj.TrackingMeasurementValue = objDTO.TrackingMeasurementValue;
                    obj.LastMaintenanceDate = objDTO.LastMaintenanceDate;
                    obj.LastMeasurementValue = objDTO.LastMeasurementValue;
                    obj.WorkorderGUID = objDTO.WorkorderGUID;
                    obj.RequisitionGUID = objDTO.RequisitionGUID;
                    obj.Status = objDTO.Status;
                    obj.Updated = objDTO.Updated;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.GUID = objDTO.GUID;
                    obj.CompanyID = objDTO.CompanyID;
                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;

                    obj.AssetGUID = objDTO.AssetGUID;
                    obj.ToolGUID = objDTO.ToolGUID;
                    context.SaveChanges();
                }
                return true;
            }
        }
        public bool DeleteToolsMaintenanceByGuids(string Guids, Int64 UserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string GUIDs = string.Empty;

                if (!string.IsNullOrEmpty(Guids) && !string.IsNullOrWhiteSpace(Guids))
                {
                    var arr = Guids.Split(',');

                    if (arr != null && arr.Any())
                    {
                        GUIDs = string.Join(",", arr);
                    }

                    var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserId),
                                                    new SqlParameter("@Guids", GUIDs)
                                                };

                    context.Database.ExecuteSqlCommand("exec [DeleteToolsMaintenanceByGuids] @UserID,@Guids", params1);
                }
                return true;
            }
        }

        public List<ToolsMaintenanceDTO> ToolsAssetMaintenceDashboard(long RoomID, long CompanyID, int DaysBefore)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@DaysBefore", DaysBefore), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolsMaintenanceDTO>("exec [ToolsAssetMaintenceDashboard] @DaysBefore,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public string BlankSerialToolName(Int64 RoomID, Int64 CompanyID)
        {
            string ToolName = string.Empty;
            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolName = (from t in context.ToolMasters
                            where (t.Serial == null || t.Serial == "") && t.Room == RoomID && t.CompanyID == CompanyID && t.IsDeleted == false && t.IsArchived == false
                            select
                               t.ToolName
                           ).FirstOrDefault();
            }
            return ToolName;
        }
        public void CreateNewMaintenanceAuto(Guid? AssetGuid, Guid? ToolGuid, Guid SchedulerGuid, Int64 RoomId, Int64 CompanyId, Int64 UserId)
        {
            IEnumerable<ToolsMaintenanceDTO> schelulerAllMaintenances = null;
            IEnumerable<ToolsMaintenanceDTO> schelulerMaintenances = null;
            ToolsSchedulerDAL objSchedulerDAL = null;

            ToolsSchedulerDTO scheduler = null;

            ToolsMaintenanceDTO NextMaintaince = null;
            List<ToolsMaintenanceDTO> ExistMaintainces = null;

            int ConsiderLastClosedMaintanance = 2;
            bool IsAutoMaintainance = false;

            AssetMasterDTO asset = null;
            ToolMasterDTO tool = null;
            AssetMasterDAL assetDAL = null;
            ToolMasterDAL toolDAL = null;
            string nextMaintainceName = string.Empty;

            try
            {
                if (AssetGuid.HasValue)
                {
                    assetDAL = new AssetMasterDAL(base.DataBaseName);
                    asset = assetDAL.GetRecord(AssetGuid.GetValueOrDefault(Guid.Empty), RoomId, CompanyId, false, false);
                    ConsiderLastClosedMaintanance = asset.NoOfPastMntsToConsider.GetValueOrDefault(2);
                    IsAutoMaintainance = true;//asset.IsAutoMaintainance
                    nextMaintainceName = asset.AssetName;
                }
                else if (ToolGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    toolDAL = new ToolMasterDAL(base.DataBaseName);
                    tool = toolDAL.GetToolByGUIDPlain(ToolGuid.GetValueOrDefault(Guid.Empty));
                    ConsiderLastClosedMaintanance = tool.NoOfPastMntsToConsider.GetValueOrDefault(2);
                    IsAutoMaintainance = true;//tool.IsAutoMaintainance
                    nextMaintainceName = tool.ToolName;
                }

                if (!IsAutoMaintainance)
                    return;

                schelulerAllMaintenances = GetToolsMaintenanceByFilterNormal(AssetGuid, ToolGuid, RoomId, CompanyId);

                ExistMaintainces = schelulerAllMaintenances.Where(x => x.MaintenanceType == MaintenanceType.Scheduled.ToString() && x.Status == MaintenanceStatus.Open.ToString() && x.SchedulerGUID == SchedulerGuid).ToList();
                if (ExistMaintainces != null && ExistMaintainces.Count > 0)
                {
                    foreach (var item in ExistMaintainces)
                    {
                        DeleteToolsMaintenanceByGuids(item.GUID.ToString(), UserId);
                    }
                }

                if (ConsiderLastClosedMaintanance < 2)
                    ConsiderLastClosedMaintanance = 2;

                objSchedulerDAL = new ToolsSchedulerDAL(base.DataBaseName);
                scheduler = objSchedulerDAL.GetToolsSchedulerByGuidPlain(SchedulerGuid);

                int schltype = scheduler.SchedulerType;

                ToolsSchedulerMappingDTO scheduleMappingDTO = new ToolsSchedulerMappingDTO()
                {
                    ToolGUID = ToolGuid,
                    AssetGUID = AssetGuid,
                    ToolSchedulerGuid = SchedulerGuid,
                    Room = RoomId,
                    CompanyID = CompanyId,
                    SchedulerFor = scheduler.ScheduleFor,
                    SchedulerType = 1,
                    IsArchived = false,
                    IsDeleted = false,
                };
                List<ToolsSchedulerMappingDTO> lstmappings = objSchedulerDAL.GetScheduleMapping(ToolGuid, AssetGuid, SchedulerGuid, null);
                if (lstmappings.Count() == 0)
                    return;

                switch (schltype)
                {

                    case 1:
                    case 2:
                    case 3:
                        if (schelulerAllMaintenances != null && (schelulerAllMaintenances.Count(t => t.TrackngMeasurement == schltype) >= ConsiderLastClosedMaintanance || schltype == (int)MaintenanceScheduleType.TimeBase))
                        {
                            if (schltype == (int)MaintenanceScheduleType.Mileage)
                            {
                                schelulerMaintenances = schelulerAllMaintenances.Where(x => x.Status == MaintenanceStatus.Close.ToString() && (x.SchedulerGUID == SchedulerGuid || x.SchedulerGUID == null) && x.MaintenanceDate != null && x.TrackngMeasurement == (int)MaintenanceScheduleType.Mileage);
                            }
                            else if (schltype == (int)MaintenanceScheduleType.OperationalHours)
                            {
                                schelulerMaintenances = schelulerAllMaintenances.Where(x => x.Status == MaintenanceStatus.Close.ToString() && (x.SchedulerGUID == SchedulerGuid || x.SchedulerGUID == null) && x.MaintenanceDate != null && x.TrackngMeasurement == (int)MaintenanceScheduleType.OperationalHours);
                            }
                            else if (schltype == (int)MaintenanceScheduleType.TimeBase)
                            {
                                schelulerMaintenances = schelulerAllMaintenances.Where(x => x.Status == MaintenanceStatus.Close.ToString() && (x.SchedulerGUID == SchedulerGuid || x.SchedulerGUID == null) && x.MaintenanceDate != null && x.TrackngMeasurement == (int)MaintenanceScheduleType.TimeBase);
                            }
                            else if (schltype == (int)MaintenanceScheduleType.CheckOuts)
                            {
                                schelulerMaintenances = schelulerAllMaintenances.Where(x => x.Status == MaintenanceStatus.Close.ToString() && (x.SchedulerGUID == SchedulerGuid || x.SchedulerGUID == null) && x.MaintenanceDate != null && x.TrackngMeasurement == (int)MaintenanceScheduleType.CheckOuts);
                            }

                            schelulerMaintenances = schelulerMaintenances.OrderByDescending(x => x.MaintenanceDate.GetValueOrDefault(DateTime.MinValue)).Take(ConsiderLastClosedMaintanance);
                            schelulerMaintenances = schelulerMaintenances.OrderBy(x => x.MaintenanceDate.GetValueOrDefault(DateTime.MinValue));
                            NextMaintaince = GetCalculatedNextMaintainceDate(schelulerMaintenances.ToList(), scheduler, AssetGuid, ToolGuid, SchedulerGuid, RoomId, CompanyId, UserId, lstmappings);

                            if (NextMaintaince != null)
                            {
                                NextMaintaince.CreatedBy = UserId;
                                NextMaintaince.LastUpdatedBy = UserId;
                                NextMaintaince.Created = DateTimeUtility.DateTimeNow;
                                NextMaintaince.Updated = DateTimeUtility.DateTimeNow;
                                NextMaintaince.MaintenanceType = MaintenanceType.Scheduled.ToString();// "scheduled";
                                NextMaintaince.Status = MaintenanceStatus.Open.ToString();// "open";
                                NextMaintaince.SchedulerGUID = scheduler.GUID;
                                NextMaintaince.ID = 0;
                                NextMaintaince.GUID = Guid.Empty;
                                NextMaintaince.MaintenanceName = lstmappings.First().MaintenanceName ?? scheduler.SchedulerName;
                                NextMaintaince.MappingGUID = lstmappings.First().GUID;
                                NextMaintaince.WorkorderGUID = null;
                                NextMaintaince.RequisitionGUID = null;
                                Insert(NextMaintaince);
                            }

                        }
                        break;
                }


            }
            finally
            {
                schelulerMaintenances = null;
                objSchedulerDAL = null;
                scheduler = null;
                NextMaintaince = null;
            }


        }
        private ToolsMaintenanceDTO GetCalculatedNextMaintainceDate(List<ToolsMaintenanceDTO> schelulerMaintenances, ToolsSchedulerDTO scheduler, Guid? AssetGuid, Guid? ToolGuid, Guid SchedulerGuid, Int64 RoomId, Int64 CompanyId, Int64 UserId, List<ToolsSchedulerMappingDTO> lstmappings)
        {
            if (scheduler.SchedulerType == (int)MaintenanceScheduleType.TimeBase)
            {
                DateTime datetimetoConsider = new RegionSettingDAL(base.DataBaseName).GetCurrentDatetimebyTimeZone(RoomId, CompanyId, UserId);
                ToolsMaintenanceDTO nextMaint = schelulerMaintenances.Where(t => t.MaintenanceDate != null).OrderByDescending(x => x.MaintenanceDate).FirstOrDefault();
                if (nextMaint != null)
                {
                    DateTime Date1 = nextMaint.MaintenanceDate.Value.AddDays(scheduler.RecurringDays);
                    while (Date1 < datetimetoConsider.Date)
                    {
                        Date1 = Date1.AddDays(scheduler.RecurringDays);
                    }
                    nextMaint.ScheduleDate = Date1;
                    nextMaint.LastMaintenanceDate = nextMaint.MaintenanceDate;
                    nextMaint.LastMeasurementValue = nextMaint.TrackingMeasurementValue;
                    nextMaint.MaintenanceDate = null;
                }
                else
                {
                    datetimetoConsider = new RegionSettingDAL(base.DataBaseName).GetCurrentDatetimebyTimeZone(RoomId, CompanyId, UserId);
                    nextMaint = new ToolsMaintenanceDTO();
                    nextMaint.AssetGUID = AssetGuid;
                    nextMaint.CompanyID = CompanyId;
                    nextMaint.Created = DateTimeUtility.DateTimeNow;
                    nextMaint.CreatedBy = UserId;
                    nextMaint.GUID = Guid.NewGuid();
                    nextMaint.IsArchived = false;
                    nextMaint.IsDeleted = false;
                    nextMaint.LastMaintenanceDate = null;
                    nextMaint.LastMeasurementValue = null;
                    nextMaint.MaintenanceDate = datetimetoConsider.Date;
                    nextMaint.LastUpdatedBy = UserId;
                    nextMaint.MaintenanceName = scheduler.SchedulerName;
                    nextMaint.MaintenanceType = MaintenanceType.Scheduled.ToString();
                    nextMaint.MappingGUID = lstmappings.First().GUID;
                    nextMaint.RequisitionGUID = null;
                    nextMaint.Room = RoomId;
                    nextMaint.ScheduleDate = datetimetoConsider.Date;
                    nextMaint.ScheduleFor = lstmappings.First().SchedulerFor;
                    nextMaint.SchedulerGUID = scheduler.GUID;
                    nextMaint.SchedulerType = (byte)scheduler.SchedulerType;
                    nextMaint.Status = MaintenanceStatus.Open.ToString();
                    nextMaint.ToolGUID = ToolGuid;
                    nextMaint.TrackingMeasurementValue = null;
                    nextMaint.TrackngMeasurement = scheduler.SchedulerType;
                    nextMaint.UDF1 = null;
                    nextMaint.UDF2 = null;
                    nextMaint.UDF3 = null;
                    nextMaint.UDF4 = null;
                    nextMaint.UDF5 = null;
                    nextMaint.Updated = DateTimeUtility.DateTimeNow;
                    nextMaint.WorkorderGUID = null;
                    //nextMaint = new ToolsMaintenanceDTO();
                    //nextMaint.ScheduleDate = DateTime.UtcNow;
                    //nextMaint.LastMaintenanceDate = null;
                    //nextMaint.LastMeasurementValue = null;
                    //nextMaint.MaintenanceDate = null;
                }
                return nextMaint;


            }
            else
            {
                List<double> dailyValue = new List<double>();

                int dailyAvgValue = 0;
                for (int i = 1; i < schelulerMaintenances.Count; i++)
                {
                    int prevValue = -1;
                    int currValue = -1;
                    DateTime prevDate = DateTime.MinValue;
                    DateTime CurrDate = DateTime.MinValue;
                    int calcVal = -1;
                    int calcDays = -1;
                    if (!string.IsNullOrEmpty(schelulerMaintenances[i - 1].TrackingMeasurementValue))
                    {
                        int.TryParse(schelulerMaintenances[i - 1].TrackingMeasurementValue, out prevValue);
                        prevDate = schelulerMaintenances[i - 1].MaintenanceDate.GetValueOrDefault(DateTime.MinValue);
                    }

                    if (!string.IsNullOrEmpty(schelulerMaintenances[i].TrackingMeasurementValue))
                    {
                        int.TryParse(schelulerMaintenances[i].TrackingMeasurementValue, out currValue);
                        CurrDate = schelulerMaintenances[i].MaintenanceDate.GetValueOrDefault(DateTime.MinValue);
                    }
                    calcVal = currValue - prevValue;
                    calcDays = CurrDate.Subtract(prevDate).Days;
                    if (calcDays > 0)
                        dailyValue.Add(calcVal / calcDays);
                    else
                        dailyValue.Add(calcVal);
                }

                if (dailyValue != null && dailyValue.Count > 0)
                    dailyAvgValue = int.Parse(Math.Round(dailyValue.Average(), 0).ToString());

                ToolsMaintenanceDTO nextMaint = schelulerMaintenances.OrderByDescending(x => x.ID).FirstOrDefault();

                if (nextMaint != null)
                {
                    nextMaint.LastMaintenanceDate = nextMaint.MaintenanceDate;
                    nextMaint.LastMeasurementValue = nextMaint.TrackingMeasurementValue;
                }
                if (dailyAvgValue > 0)
                {
                    int NextTrackMeasurVal = -1;
                    int.TryParse(nextMaint.TrackingMeasurementValue, out NextTrackMeasurVal);
                    double avgdays = -1;
                    if (scheduler.SchedulerType == (int)MaintenanceScheduleType.OperationalHours)
                    {
                        avgdays = scheduler.OperationalHours.GetValueOrDefault(0) / dailyAvgValue;
                        NextTrackMeasurVal = NextTrackMeasurVal + int.Parse(Math.Round(scheduler.OperationalHours.GetValueOrDefault(0), 0).ToString());
                    }
                    else if (scheduler.SchedulerType == (int)MaintenanceScheduleType.Mileage)
                    {
                        avgdays = scheduler.Mileage.GetValueOrDefault(0) / dailyAvgValue;
                        NextTrackMeasurVal = NextTrackMeasurVal + int.Parse(Math.Round(scheduler.Mileage.GetValueOrDefault(0), 0).ToString());
                    }

                    nextMaint.ScheduleDate = nextMaint.MaintenanceDate.GetValueOrDefault(DateTime.MinValue).AddDays(int.Parse(Math.Round(avgdays, 0).ToString()));
                    nextMaint.MaintenanceDate = null;
                    nextMaint.TrackingMeasurementValue = NextTrackMeasurVal.ToString();
                }

                return nextMaint;
            }
        }
        public List<ToolsMaintenanceDTO> GetMntsUnClose(ToolsSchedulerMappingDTO obj)
        {
            string[] arrstatus = new string[] { MaintenanceStatus.Open.ToString(), MaintenanceStatus.Start.ToString() };
            List<ToolsMaintenanceDTO> lstmnts = new List<ToolsMaintenanceDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstmnts = (from u in context.ToolsMaintenances
                           where (u.AssetGUID ?? Guid.Empty) == (obj.AssetGUID ?? Guid.Empty) && (u.ToolGUID ?? Guid.Empty) == (obj.ToolGUID ?? Guid.Empty) && u.SchedulerGUID == obj.ToolSchedulerGuid && arrstatus.Contains(u.Status)
                           select new ToolsMaintenanceDTO
                           {
                               ID = u.ID,
                               MaintenanceName = u.MaintenanceName,
                               MaintenanceDate = u.MaintenanceDate,
                               SchedulerGUID = u.SchedulerGUID,
                               ScheduleDate = u.ScheduleDate,
                               TrackngMeasurement = u.TrackngMeasurement,
                               TrackingMeasurementValue = u.TrackingMeasurementValue,
                               LastMaintenanceDate = u.LastMaintenanceDate,
                               LastMeasurementValue = u.LastMeasurementValue,
                               WorkorderGUID = u.WorkorderGUID,
                               RequisitionGUID = u.RequisitionGUID,
                               Status = u.Status,
                               Created = u.Created,
                               CreatedBy = u.CreatedBy,
                               Updated = u.Updated,
                               LastUpdatedBy = u.LastUpdatedBy,
                               Room = u.Room,
                               IsArchived = u.IsArchived,
                               IsDeleted = u.IsDeleted,
                               GUID = u.GUID,
                               CompanyID = u.CompanyID,
                               UDF1 = u.UDF1,
                               UDF2 = u.UDF2,
                               UDF3 = u.UDF3,
                               UDF4 = u.UDF4,
                               UDF5 = u.UDF5,
                               ToolGUID = u.ToolGUID,
                               AssetGUID = u.AssetGUID,
                               ScheduleFor = u.ScheduleFor,
                               SchedulerType = u.SchedulerType,
                               ToolSchedulerGuid = u.SchedulerGUID,
                               MaintenanceType = u.MaintenanceType,

                           }).ToList();
            }
            return lstmnts;
        }
        public List<ToolsMaintenanceDTO> CloseMaintenanceOnWOClose(Guid WorkOrderGUID, long UserID, long RoomID, long CompanyID, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<ToolsMaintenanceDTO> lstReturns = new List<ToolsMaintenanceDTO>();
            string GUIDS = string.Empty;
            string mntsstatus = MaintenanceStatus.Start.ToString();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IQueryable<ToolsMaintenance> mnts = context.ToolsMaintenances.Where(t => t.WorkorderGUID == WorkOrderGUID && t.Status == mntsstatus && (t.IsDeleted ?? false) == false);
                List<ToolsMaintenance> lstmnts = new List<ToolsMaintenance>();
                if (mnts != null && mnts.Count() > 0)
                {
                    lstmnts = mnts.ToList();
                    foreach (var item in mnts)
                    {
                        GUIDS = GUIDS + item.GUID + "','";
                        item.Status = MaintenanceStatus.Close.ToString();
                        item.Updated = DateTimeUtility.DateTimeNow;
                        item.LastUpdatedBy = UserID;
                    }
                    context.SaveChanges();
                    foreach (var item in lstmnts)
                    {
                        CreateNewMaintenanceAuto(item.AssetGUID, item.ToolGUID, item.SchedulerGUID ?? Guid.Empty, item.Room ?? 0, item.CompanyID ?? 0, UserID);
                    }
                    int totalcount = 0;

                    GUIDS = GUIDS.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    lstReturns = new AssetMasterDAL(base.DataBaseName).GetPagedRecordsToolMaintenance(0, int.MaxValue, out totalcount, string.Empty, "ID DESC", CompanyID, RoomID, false, false, RoomDateFormat, "all", null, null, GUIDS, CurrentTimeZone);

                }
            }
            return lstReturns;

        }
        public List<ToolsSchedulerDetailsDTO> GetSchedulerItems(Guid MntsGUID)
        {
            List<ToolsSchedulerDetailsDTO> lstitems = new List<ToolsSchedulerDetailsDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstitems = (from tm in context.ToolsMaintenances
                            join ts in context.ToolsSchedulers on tm.SchedulerGUID equals ts.GUID
                            join tsd in context.ToolsSchedulerDetails on ts.GUID equals tsd.ScheduleGUID
                            join im in context.ItemMasters on tsd.ItemGUID equals im.GUID
                            where tm.GUID == MntsGUID && (tsd.IsDeleted ?? false) == false
                            select new ToolsSchedulerDetailsDTO
                            {
                                CompanyID = tsd.CompanyID,
                                Created = tsd.Created,
                                CreatedBy = tsd.CreatedBy,
                                GUID = tsd.GUID,
                                ID = tsd.ID,
                                IsArchived = tsd.IsArchived,
                                IsDeleted = tsd.IsDeleted,
                                Room = tsd.Room,
                                ItemCost = im.Cost,
                                ItemDescription = im.Description,
                                ItemGUID = tsd.ItemGUID,
                                ItemNumber = im.ItemNumber,
                                LastUpdated = tsd.LastUpdated,
                                LastUpdatedBy = tsd.LastUpdatedBy,
                                Quantity = tsd.Quantity,
                                ScheduleGUID = tsd.ScheduleGUID
                            }).ToList();
            }
            return lstitems;

        }
        public ToolsMaintenanceDTO GetLastMaintenance(Guid? ToolGUID, Guid? AssetGUID)
        {
            ToolsMaintenanceDTO objToolsMaintenanceDTO = new ToolsMaintenanceDTO();
            string CloseStatus = MaintenanceStatus.Close.ToString();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objToolsMaintenanceDTO = (from u in context.ToolsMaintenances
                                          where (u.ToolGUID ?? Guid.Empty) == (ToolGUID ?? Guid.Empty) && (u.AssetGUID ?? Guid.Empty) == (AssetGUID ?? Guid.Empty) && u.MaintenanceDate != null && u.MaintenanceDate != DateTime.MinValue && u.Status == CloseStatus && (u.TrackingMeasurementValue ?? string.Empty) != string.Empty
                                          orderby u.MaintenanceDate descending
                                          select new ToolsMaintenanceDTO
                                          {
                                              ID = u.ID,
                                              MaintenanceName = u.MaintenanceName,
                                              MaintenanceDate = u.MaintenanceDate,
                                              SchedulerGUID = u.SchedulerGUID,
                                              ScheduleDate = u.ScheduleDate,
                                              TrackngMeasurement = u.TrackngMeasurement,
                                              TrackingMeasurementValue = u.TrackingMeasurementValue,
                                              LastMaintenanceDate = u.LastMaintenanceDate,
                                              LastMeasurementValue = u.LastMeasurementValue,
                                              WorkorderGUID = u.WorkorderGUID,
                                              RequisitionGUID = u.RequisitionGUID,
                                              Status = u.Status,
                                              Created = u.Created,
                                              CreatedBy = u.CreatedBy,
                                              Updated = u.Updated,
                                              LastUpdatedBy = u.LastUpdatedBy,
                                              Room = u.Room,
                                              IsArchived = u.IsArchived,
                                              IsDeleted = u.IsDeleted,
                                              GUID = u.GUID,
                                              CompanyID = u.CompanyID,
                                              UDF1 = u.UDF1,
                                              UDF2 = u.UDF2,
                                              UDF3 = u.UDF3,
                                              UDF4 = u.UDF4,
                                              UDF5 = u.UDF5,
                                              ToolGUID = u.ToolGUID,
                                              AssetGUID = u.AssetGUID,
                                              ScheduleFor = u.ScheduleFor,
                                              SchedulerType = u.SchedulerType,
                                              MaintenanceType = u.MaintenanceType,

                                          }).FirstOrDefault();
            }
            return objToolsMaintenanceDTO;

        }
        public List<PullOnMaintenanceDTO> GetPullOnMaintenance(Guid MaintenanceGUID)
        {

            var params1 = new SqlParameter[] { new SqlParameter("@MaintenanceGUID", MaintenanceGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<PullOnMaintenanceDTO>("exec [GetItemPullScheduledMaintenance] @MaintenanceGUID", params1).ToList();
            }
        }
        public List<ToolsMaintenanceDTO> GetToolMaintenceForDashboard(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string SortColumnName, long RoomID, long CompanyID)
        {
            List<ToolsMaintenanceDTO> assets = new List<ToolsMaintenanceDTO>();
            TotalCount = 0;
            DataSet dsAsset = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return assets;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            dsAsset = SqlHelper.ExecuteDataset(EturnsConnection, "GetToolMaintenceForDashboard", StartRowIndex, MaxRows, SearchTerm, SortColumnName, CompanyID, RoomID);

            if (dsAsset != null && dsAsset.Tables.Count > 0)
            {
                assets = DataTableHelper.ToList<ToolsMaintenanceDTO>(dsAsset.Tables[0]);

                if (assets != null && assets.Any())
                {
                    TotalCount = assets.ElementAt(0).TotalRecords ?? 0;
                }
            }

            return assets;
        }
        #endregion
    }
}


