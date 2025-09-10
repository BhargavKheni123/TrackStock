using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DAL
{
    public class AssetMasterDAL : eTurnsBaseDAL
    {
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE AssetMaster SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);
                return true;
            }
        }

        public string DeleteMappingRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            int cnt = 0;
            string strmsg = string.Empty;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        cnt += 1;
                        strQuery += "UPDATE ToolsSchedulerMapping SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE GUID ='" + item.ToString() + "';";
                    }
                }
                context.ExecuteStoreCommand(strQuery);
                strmsg = cnt + " Record(s) deleted successfully.";

                return strmsg;
            }
        }

        public string UnDeleteMappingRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            int cnt = 0;
            string strmsg = string.Empty;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        cnt += 1;
                        strQuery += "UPDATE ToolsSchedulerMapping SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=0 WHERE GUID ='" + item.ToString() + "';";
                    }
                }
                context.ExecuteStoreCommand(strQuery);
                strmsg = cnt + " Record(s) deleted successfully.";

                return strmsg;
            }
        }

        public ToolsSchedulerMappingDTO GetSchedulerMappingRecord(Guid MappingGUID, long CompanyId, long RoomId, bool IsArchive, bool Isdeleted)
        {
            ToolsSchedulerMappingDTO objDTO = new ToolsSchedulerMappingDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objDTO = (from u in context.ToolsSchedulerMappings
                          join um in context.UserMasters on u.CreatedBy equals um.ID into im_UMC_join
                          from im_UMC in im_UMC_join.DefaultIfEmpty()

                          join umU in context.UserMasters on u.LastUpdatedBy equals umU.ID into im_UMU_join
                          from im_UMU in im_UMU_join.DefaultIfEmpty()
                          join rm in context.Rooms on u.Room equals rm.ID into im_RM_join
                          from im_RM in im_RM_join.DefaultIfEmpty()
                          where u.GUID == MappingGUID && u.CompanyID == CompanyId && u.Room == RoomId && u.IsArchived == IsArchive && u.IsDeleted == Isdeleted
                          select new ToolsSchedulerMappingDTO
                          {
                              ID = u.ID,
                              SchedulerFor = u.SchedulerFor,
                              //SchedulerType = u.SchedulerType,
                              ToolSchedulerGuid = u.ToolSchedulerGuid,
                              ToolGUID = u.ToolGUID,
                              AssetGUID = u.AssetGUID,
                              Created = u.Created,
                              CreatedBy = u.CreatedBy,
                              Updated = u.Updated,
                              LastUpdatedBy = u.LastUpdatedBy,
                              Room = u.Room ?? 0,
                              IsArchived = u.IsArchived,
                              IsDeleted = u.IsDeleted,
                              GUID = u.GUID,
                              CompanyID = u.CompanyID ?? 0,
                              UDF1 = u.UDF1,
                              UDF2 = u.UDF2,
                              UDF3 = u.UDF3,
                              UDF4 = u.UDF4,
                              UDF5 = u.UDF5,
                              CreatedByName = im_UMC.UserName,
                              UpdatedByName = im_UMU.UserName,
                              RoomName = im_RM.RoomName,
                              MaintenanceName = u.MaintenanceName,
                              TrackingMeasurement = u.TrackingMeasurement
                          }
                    ).SingleOrDefault();
            }
            return objDTO;
        }


    }
}
