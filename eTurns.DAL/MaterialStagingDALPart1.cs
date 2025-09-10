using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using System.Data.SqlClient;
using eTurns.DTO.Resources;
using System.Web;


namespace eTurns.DAL
{
    public class MaterialStagingDAL : eTurnsBaseDAL
    {

        public IEnumerable<MaterialStagingDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            List<MaterialStagingDTO> ObjCache;
            string sSQL = "";
            if (IsArchived && IsDeleted)
            {
                sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
            }
            if (!IsArchived && !IsDeleted)
            {
                sSQL += "  ISNULL(A.IsDeleted,0) = 0 AND ISNULL(A.IsArchived,0) = 0";
            }
            else if (IsArchived)
            {
                sSQL += "A.IsArchived = 1";
            }
            else if (IsDeleted)
            {
                sSQL += "A.IsDeleted =1";
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ObjCache = (from u in context.ExecuteStoreQuery<MaterialStagingDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM MaterialStaging A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyId.ToString() + " AND A.Room=" + RoomID.ToString() + " AND " + sSQL)
                            select new MaterialStagingDTO
                            {
                                ID = u.ID,
                                StagingName = u.StagingName,
                                Description = u.Description,
                                BinID = u.BinID,
                                GUID = u.GUID,
                                UDF1 = u.UDF1,
                                UDF2 = u.UDF2,
                                UDF3 = u.UDF3,
                                UDF4 = u.UDF4,
                                UDF5 = u.UDF5,
                                CompanyID = u.CompanyID,
                                Room = u.Room,
                                IsDeleted = u.IsDeleted,
                                IsArchived = u.IsArchived,
                                Created = u.Created,
                                Updated = u.Updated,
                                CreatedBy = u.CreatedBy,
                                LastUpdatedBy = u.LastUpdatedBy,
                                CreatedByName = u.CreatedByName,
                                UpdatedByName = u.UpdatedByName,
                                RoomName = u.RoomName,
                                AppendedBarcodeString = string.Empty,
                                StagingLocationName = u.StagingLocationName,
                                StagingStatus = u.StagingStatus,
                                AddedFrom = u.AddedFrom,
                                EditedFrom = u.EditedFrom,
                                ReceivedOn = u.ReceivedOn,
                                ReceivedOnWeb = u.ReceivedOnWeb
                            }).AsParallel().ToList();
            }

            return ObjCache;
        }


    }
}
