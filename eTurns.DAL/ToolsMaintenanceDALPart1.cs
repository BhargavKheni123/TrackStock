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
    public partial class ToolsMaintenanceDAL : eTurnsBaseDAL
    {
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
                return (from u in context.ExecuteStoreQuery<ToolsMaintenanceDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName , E.AssetName, F.ToolName,WO.WOName,RM.RequisitionNumber as 'RequisitionName'
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

        public ToolsMaintenanceDTO GetRecordsByScheduler_Mapping(Guid? AssetGUID, Guid? ToolGUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, Guid SchedulerGUID, Guid MappingGUID)
        {
            IEnumerable<ToolsMaintenanceDTO> lstToolsMaintenanceDTO = GetCachedData(AssetGUID, ToolGUID, RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
            lstToolsMaintenanceDTO = lstToolsMaintenanceDTO.Where(x => x.SchedulerGUID == SchedulerGUID && x.MappingGUID == MappingGUID).ToList();
            return lstToolsMaintenanceDTO.FirstOrDefault();
        }
        public IEnumerable<ToolsMaintenanceDTO> GetAllRecords(Guid? AssetGUID, Guid? ToolGUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(AssetGUID, ToolGUID, RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        }
        public ToolsMaintenanceDTO GetRecordsByScheduler_Mapping(Guid? AssetGUID, Guid? ToolGUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, Guid SchedulerGUID, Guid MappingGUID)
        {
            IEnumerable<ToolsMaintenanceDTO> lstToolsMaintenanceDTO = GetCachedData(AssetGUID, ToolGUID, RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
            lstToolsMaintenanceDTO = lstToolsMaintenanceDTO.Where(x => x.SchedulerGUID == SchedulerGUID && x.MappingGUID == MappingGUID).ToList();
            return lstToolsMaintenanceDTO.FirstOrDefault();
        }
        public ToolsMaintenanceDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
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
                return (from u in context.ExecuteStoreQuery<ToolsMaintenanceDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName , E.AssetName, F.ToolName ,WO.WOName,RM.RequisitionNumber as 'RequisitionName'
                        FROM ToolsMaintenance A 
                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                        LEFT OUTER JOIN AssetMaster E on A.AssetGUID = E.GUID
                        LEFT OUTER JOIN ToolMaster F on A.ToolGUID = F.GUID 
                        LEFT OUTER JOIN WorkOrder WO on A.ToolGUID = WO.GUID 
                        LEFT OUTER JOIN RequisitionMaster as RM on A.ToolGUID = RM.GUID 
                        WHERE A.ID = " + id.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND " + sSQL)
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

        public ToolsMaintenanceDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
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
                return (from u in context.ExecuteStoreQuery<ToolsMaintenanceDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName , E.AssetName, F.ToolName ,WO.WOName,RM.RequisitionNumber as 'RequisitionName'
                        FROM ToolsMaintenance A 
                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                        LEFT OUTER JOIN AssetMaster E on A.AssetGUID = E.GUID
                        LEFT OUTER JOIN ToolMaster F on A.ToolGUID = F.GUID 
                        LEFT OUTER JOIN WorkOrder WO on A.ToolGUID = WO.GUID 
                        LEFT OUTER JOIN RequisitionMaster as RM on A.ToolGUID = RM.GUID 
                        WHERE A.ID = " + id.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND " + sSQL)
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

        public ToolsMaintenanceDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
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
                return (from u in context.ExecuteStoreQuery<ToolsMaintenanceDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName , E.AssetName, F.ToolName ,WO.WOName,TSC.SchedulerName,RM.RequisitionNumber as 'RequisitionName'
                        FROM ToolsMaintenance A 
                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                        LEFT OUTER JOIN AssetMaster E on A.AssetGUID = E.GUID
                        LEFT OUTER JOIN ToolMaster F on A.ToolGUID = F.GUID 
                        LEFT OUTER JOIN WorkOrder WO on A.ToolGUID = WO.GUID 
                        LEFT OUTER JOIN RequisitionMaster as RM on A.ToolGUID = RM.GUID 
                        LEFT OUTER JOIN ToolsScheduler as TSC on A.SchedulerGUID = TSC.GUID 
                        WHERE A.GUID = '" + GUID.ToString() + "' AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND " + sSQL)
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
                            SchedulerName = u.SchedulerName,

                        }).SingleOrDefault();

            }
        }

        public ToolsMaintenanceDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
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
                return (from u in context.ExecuteStoreQuery<ToolsMaintenanceDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName , E.AssetName, F.ToolName ,WO.WOName,TSC.SchedulerName,RM.RequisitionNumber as 'RequisitionName'
                        FROM ToolsMaintenance A 
                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                        LEFT OUTER JOIN AssetMaster E on A.AssetGUID = E.GUID
                        LEFT OUTER JOIN ToolMaster F on A.ToolGUID = F.GUID 
                        LEFT OUTER JOIN WorkOrder WO on A.ToolGUID = WO.GUID 
                        LEFT OUTER JOIN RequisitionMaster as RM on A.ToolGUID = RM.GUID 
                        LEFT OUTER JOIN ToolsScheduler as TSC on A.SchedulerGUID = TSC.GUID 
                        WHERE A.GUID = '" + GUID.ToString() + "' AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND " + sSQL)
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
                            SchedulerName = u.SchedulerName,

                        }).SingleOrDefault();

            }
        }

        public bool DeleteRecords(string Guids, Int64 userid)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string[] arrGuids = Guids.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var guid in arrGuids)
                {
                    strQuery += "UPDATE ToolsMaintenance SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE GUID ='" + guid.ToString() + "';";
                    strQuery += "UPDATE ToolMaintenanceDetails SET LastUpdated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE MaintenanceGUID ='" + guid.ToString() + "';";
                }
                context.ExecuteStoreCommand(strQuery);
                return true;
            }
        }
    }
}
