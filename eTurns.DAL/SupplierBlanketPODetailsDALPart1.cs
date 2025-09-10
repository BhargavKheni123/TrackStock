using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;

namespace eTurns.DAL
{
    public partial class SupplierBlanketPODetailsDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE SupplierBlanketPODetails SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                return true;
            }
        }

        /// <summary>
        /// Get Particullar Record from the SupplierBlanketPODetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public SupplierBlanketPODetailsDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<SupplierBlanketPODetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM SupplierBlanketPODetails_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new SupplierBlanketPODetailsDTO
                        {
                            ID = u.ID,
                            SupplierID = u.SupplierID,
                            StartDate = u.StartDate,
                            Enddate = u.Enddate,
                            BlanketPO = u.BlanketPO,
                            GUID = u.GUID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb
                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Get Particullar Record from the SupplierBlanketPODetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public SupplierBlanketPODetailsDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<SupplierBlanketPODetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM SupplierBlanketPODetails A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted = 0 AND A.IsArchived = 0 AND  A.ID = " + id.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString())
                        select new SupplierBlanketPODetailsDTO
                        {
                            ID = u.ID,
                            SupplierID = u.SupplierID,
                            StartDate = u.StartDate,
                            Enddate = u.Enddate,
                            BlanketPO = u.BlanketPO,
                            GUID = u.GUID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb
                        }).SingleOrDefault();
            }
        }


        public IEnumerable<SupplierBlanketPODetailsDTO> CheckSupplierBlanketPOExpiry(Int64 ID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<SupplierBlanketPODetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM SupplierBlanketPODetails A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE getutcdate() between isnull(A.StartDate,cast('1753-1-1' as datetime)) and isnull(a.Enddate,cast('9999-12-31' as datetime)) and A.IsDeleted = 0 AND A.IsArchived = 0 AND A.Id = " + ID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString())
                        select new SupplierBlanketPODetailsDTO
                        {
                            ID = u.ID,
                            SupplierID = u.SupplierID,
                            StartDate = u.StartDate,
                            Enddate = u.Enddate,
                            BlanketPO = u.BlanketPO,
                            GUID = u.GUID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb
                        }).AsParallel().ToList();
            }
        }



        public IEnumerable<SupplierBlanketPODetailsDTO> GetAllBlanketPOBySupplierIDItemGUID(Int64 SupplierID, Int64 RoomID, Int64 CompanyID, Guid? ItemGUID)
        {
            IEnumerable<SupplierBlanketPODetailsDTO> obj;
            IEnumerable<SupplierBlanketPODetailsDTO> obj1;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = (from u in context.ExecuteStoreQuery<SupplierBlanketPODetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM SupplierBlanketPODetails A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID  INNER JOIN ItemSupplierDetails ON A.ID = ItemSupplierDetails.BlanketPOID 
                                                                                           INNER JOIN ItemMaster ON ItemSupplierDetails.ItemGUID = ItemMaster.GUID  WHERE A.IsDeleted = 0 AND A.IsArchived = 0 and ItemGuid = '" + ItemGUID + "' AND A.SupplierID = " + SupplierID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + " and (getutcdate() between A.StartDate and a.Enddate OR A.StartDate IS NULL OR A.Enddate IS NULL) UNION SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM SupplierBlanketPODetails A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE (getutcdate() between A.StartDate and a.Enddate OR A.StartDate IS NULL OR A.Enddate IS NULL) and A.IsDeleted = 0 AND A.IsArchived = 0 AND A.SupplierID = " + SupplierID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + " AND A.Room = " + RoomID.ToString() + " ")
                       select new SupplierBlanketPODetailsDTO
                       {
                           ID = u.ID,
                           SupplierID = u.SupplierID,
                           StartDate = u.StartDate,
                           Enddate = u.Enddate,
                           BlanketPO = u.BlanketPO,
                           GUID = u.GUID,
                           Created = u.Created,
                           Updated = u.Updated,
                           CreatedBy = u.CreatedBy,
                           LastUpdatedBy = u.LastUpdatedBy,
                           IsDeleted = u.IsDeleted,
                           IsArchived = u.IsArchived,
                           CompanyID = u.CompanyID,
                           Room = u.Room,
                           CreatedByName = u.CreatedByName,
                           UpdatedByName = u.UpdatedByName,
                           RoomName = u.RoomName,
                           ExpiryPO = 0,
                           AddedFrom = u.AddedFrom,
                           EditedFrom = u.EditedFrom,
                           ReceivedOn = u.ReceivedOn,
                           ReceivedOnWeb = u.ReceivedOnWeb
                       }).AsParallel().ToList();
            }

            foreach (var val in obj)
            {
                obj1 = CheckSupplierBlanketPOExpiry(Convert.ToInt64(val.ID), Convert.ToInt64(val.Room), Convert.ToInt64(val.CompanyID));
                if (obj1.Count() == 0)
                {
                    val.ExpiryPO = val.ID;
                }
            }
            return obj;
        }


        public IEnumerable<SupplierBlanketPODetailsDTO> GetAllBlanketPOBySupplierIDNew(Int64 SupplierID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<SupplierBlanketPODetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM SupplierBlanketPODetails A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE (getutcdate() between A.StartDate and a.Enddate OR A.StartDate IS NULL OR A.Enddate IS NULL) and A.IsDeleted = 0 AND A.IsArchived = 0 AND A.SupplierID = " + SupplierID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString())
                        select new SupplierBlanketPODetailsDTO
                        {
                            ID = u.ID,
                            SupplierID = u.SupplierID,
                            StartDate = u.StartDate,
                            Enddate = u.Enddate,
                            BlanketPO = u.BlanketPO,
                            GUID = u.GUID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb
                        }).AsParallel().ToList();
            }
        }

        public IEnumerable<SupplierBlanketPODetailsDTO> GetAllBlanketPOBySupplierID(Int64 SupplierID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //return (from u in context.ExecuteStoreQuery<SupplierBlanketPODetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM SupplierBlanketPODetails A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted = 0 AND A.IsArchived = 0 AND A.SupplierID = " + SupplierID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString())
                return (from u in context.ExecuteStoreQuery<SupplierBlanketPODetailsDTO>(@";with pulled_CTE(pulledQty,PullOrderNumber) as (select isnull(sum(PM.poolQuantity),0),PullOrderNumber from PullMaster Pm inner join ItemMaster Im on pm.ItemGUID = Im.guid where Pm.Room=" + RoomID.ToString() + " and im.SupplierID=" + SupplierID.ToString() + " and pm.CompanyID= " + CompanyID.ToString() + @"group by PullOrderNumber), ordered_CTE(ReceivedQuantity,OrderNumber)as(select isnull(sum(od.ReceivedQuantity),0),OrderNumber from ordermaster om inner join OrderDetails od on om.GUID = od.OrderGUID inner join ItemMaster im on im.GUID = od.ItemGUID where om.Room=" + RoomID.ToString() + " and om.Supplier=" + SupplierID.ToString() + " and om.CompanyID= " + CompanyID.ToString() + @" group by OrderNumber)SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,isnull(pulledQty,0) as pulledQty,isnull(ReceivedQuantity,0) as OrderedQty FROM SupplierBlanketPODetails A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID left outer join pulled_CTE PC on Pc.PullOrderNumber = A.BlanketPO left outer join ordered_CTE OC on Oc.OrderNumber=A.BlanketPO WHERE A.IsDeleted = 0 AND A.IsArchived = 0 AND A.SupplierID = " + SupplierID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString())
                        select new SupplierBlanketPODetailsDTO
                        {
                            ID = u.ID,
                            SupplierID = u.SupplierID,
                            StartDate = u.StartDate,
                            Enddate = u.Enddate,
                            BlanketPO = u.BlanketPO,
                            MaxLimit = u.MaxLimit,
                            IsNotExceed = u.IsNotExceed,
                            GUID = u.GUID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            PulledQty = u.PulledQty,
                            OrderedQty = u.OrderedQty,
                            MaxLimitQty = u.MaxLimitQty,
                            IsNotExceedQty = u.IsNotExceedQty

                        }).AsParallel().ToList();
            }
        }

        public bool DeleteRecordsExcept(string IDs, long SupplierID, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (IDs != "")
                {
                    string strQuery = "";
                    //strQuery += "UPDATE SupplierBlanketPODetails SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE SupplierID = " + SupplierID.ToString() + " AND ID Not in( ";
                    strQuery += "UPDATE SupplierBlanketPODetails SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE SupplierID = " + SupplierID.ToString() + " AND ID in( ";
                    foreach (var item in IDs.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            strQuery += item.ToString() + ",";
                        }
                    }
                    strQuery = strQuery.Substring(0, strQuery.Length - 1);
                    strQuery += ");";
                    context.ExecuteStoreCommand(strQuery);


                }

                return true;
            }
        }
    }
}
