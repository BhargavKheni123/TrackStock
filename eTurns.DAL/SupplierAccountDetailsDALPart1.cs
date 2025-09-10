using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class SupplierAccountDetailsDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Get Particullar Record from the SupplierAccountDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public SupplierAccountDetailsDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<SupplierAccountDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM SupplierAccountDetails A LEFT OUTER  JOIN ShipViaMaster SV on A.ShipViaID = SV.ID LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE  A.IsDeleted = 0 AND A.IsArchived = 0 AND A.ID = " + id.ToString() + " AND  A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString())
                        select new SupplierAccountDetailsDTO
                        {
                            ID = u.ID,
                            SupplierID = u.SupplierID,
                            AccountNo = u.AccountNo,
                            IsDefault = u.IsDefault,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            AccountName = u.AccountName,
                            Address = u.Address,
                            City = u.City,
                            State = u.State,
                            ZipCode = u.ZipCode,
                            ShipViaID = u.ShipViaID,
                            ShipVia = u.ShipVia,
                            Country = u.Country,
                            ShipToID = u.ShipToID
                        }).FirstOrDefault();
            }
        }


        /// <summary>
        /// Get Particullar Record from the SupplierAccountDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public SupplierAccountDetailsDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<SupplierAccountDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM SupplierAccountDetails_History A LEFT OUTER  JOIN ShipViaMaster SV on A.ShipViaID = SV.ID LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new SupplierAccountDetailsDTO
                        {
                            ID = u.ID,
                            SupplierID = u.SupplierID,
                            AccountNo = u.AccountNo,
                            IsDefault = u.IsDefault,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            AccountName = u.AccountName,
                            Address = u.Address,
                            City = u.City,
                            State = u.State,
                            ZipCode = u.ZipCode,
                            ShipViaID = u.ShipViaID,
                            ShipVia = u.ShipVia,
                            Country = u.Country,
                            ShipToID = u.ShipToID
                        }).SingleOrDefault();
            }
        }
    }
}
