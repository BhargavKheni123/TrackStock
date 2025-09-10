using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using eTurns.DTO.Resources;

namespace eTurns.DAL
{
    public partial class PullPOMasterDAL : eTurnsBaseDAL
    {
        public PullPOMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.ID == id).SingleOrDefault();
        }

        public IEnumerable<PullPOMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyID, bool Isdeleted, bool IsArchieved)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string Isdelete = "0";
                // string IsArchieve = "0";
                if (Isdeleted)
                {
                    Isdelete = "1";
                }
                //if (IsArchieved)
                //{
                //    IsArchieve = "1";
                //}

                return (from x in context.ExecuteStoreQuery<PullPOMasterDTO>(@"SELECT A.Id,A.PullOrderNumber,Isnull(A.IsActive,1) as IsActive,A.RoomId,A.CompanyID,A.Isdeleted,A.Created,A.Updated,A.ReceivedOnWeb
                                                                            ,A.ReceivedOn,A.CreatedBy,A.UpdatedBy,A.AddedFrom
                                                                            ,A.EditedFrom, B.UserName AS CreatedByName, C.UserName AS UpdatedByName
                                                                                ,R.RoomName AS RoomName
                                                                                FROM pullpomaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
					                                                                                 LEFT OUTER JOIN UserMaster C on A.UpdatedBy = C.ID 
					                                                                                 LEFT OUTER JOIN Room R on A.RoomID = R.ID
                                                                                WHERE   ISNULL(A.IsDeleted,0)= " + Isdelete + "  AND  A.RoomID = " + RoomID + @" AND A.CompanyID = " + CompanyID)
                        select new PullPOMasterDTO
                        {
                            ID = x.ID,
                            Created = x.Created,
                            Updated = x.Updated,
                            CreatedBy = x.CreatedBy,
                            UpdatedBy = x.UpdatedBy,
                            IsDeleted = x.IsDeleted,
                            PullOrderNumber = x.PullOrderNumber,
                            UpdatedByName = x.UpdatedByName,
                            CompanyID = x.CompanyID,
                            CreatedByName = x.CreatedByName,
                            IsActive = x.IsActive,
                            //IsArchived = x.IsArchived,
                            RoomId = x.RoomId,
                            RoomName = x.RoomName,
                            ReceivedOn = x.ReceivedOn,
                            ReceivedOnWeb = x.ReceivedOnWeb,
                            AddedFrom = x.AddedFrom,
                            EditedFrom = x.EditedFrom
                        }).ToList();

            }
        }

        public void DeletePORecord(string IDs, long userid, Int64 RoomID, Int64 CompanyId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string[] strIds = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in strIds)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE PullPOMaster SET EditedFrom='Web', Updated = '" + DateTime.Now.ToString() + "' , UpdatedBy = " + userid.ToString() + ",ReceivedOn='" + DateTime.Now.ToString() + "' , IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);
            }
        }

        public void UnDeletePORecord(string IDs, Int64 userid, Int64 RoomID, Int64 CompanyId)
        {

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string[] strIds = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in strIds)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE PullPOMaster SET EditedFrom='Web', Updated = '" + DateTime.Now.ToString() + "' , UpdatedBy = " + userid.ToString() + ",ReceivedOn='" + DateTime.Now.ToString() + "' , IsDeleted=0 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);
            }
        }
    }
}
