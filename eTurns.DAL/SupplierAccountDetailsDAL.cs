using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class SupplierAccountDetailsDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public SupplierAccountDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public SupplierAccountDetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SupplierAccountDetailsDTO> GetAllAccountsBySupplierID(Int64 SupplierID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //return (from u in context.Database.SqlQuery<SupplierAccountDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A2.DefaultOrderRequiredDays, SV.ShipVia as ShipVia " +
                //                                                                       @" FROM SupplierAccountDetails A " +
                //                                                                       @" INNER JOIN SupplierMaster A2 on A2.ID = A.SupplierID " +
                //                                                                       @" LEFT JOIN ShipViaMaster SV on SV.ID = A.ShipViaID " +
                //                                                                       @" LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID " +
                //                                                                       @" LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID " +
                //                                                                       @" LEFT OUTER JOIN Room D on A.Room = D.ID " +
                //                                                                       @" WHERE  A.IsDeleted = 0 AND A.IsArchived = 0 AND A.SupplierID = " + SupplierID.ToString() + " AND  A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString())

                var params1 = new SqlParameter[] { new SqlParameter("@SupplierID", SupplierID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return (from u in context.Database.SqlQuery<SupplierAccountDetailsDTO>("exec GetAllAccountsBySupplierID @SupplierID,@RoomID,@CompanyID", params1)
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
                            AccountNumnerMerge = u.AccountNo + " " + u.AccountName,
                            DefaultOrderRequiredDays = u.DefaultOrderRequiredDays,
                            ShipViaID = u.ShipViaID,
                            ShipVia = u.ShipVia,
                            Country = u.Country,
                            ShipToID = u.ShipToID
                        }).AsParallel().ToList();
            }
        }

        public List<SupplierAccountDetailsDTO> GetAllAccountsBySupplierIDPlain(long SupplierID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@SupplierID", SupplierID), 
                                                   new SqlParameter("@RoomID", RoomID), 
                                                   new SqlParameter("@CompanyID", CompanyID) 
                                                 };

                return context.Database.SqlQuery<SupplierAccountDetailsDTO>("exec GetAllAccountsBySupplierIDPlain @SupplierID,@RoomID,@CompanyID", params1).ToList();
                        
            }
        }

        /// <summary>
        /// Insert Record in the DataBase SupplierAccountDetails
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(SupplierAccountDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SupplierAccountDetail obj = new SupplierAccountDetail();
                obj.ID = 0;
                obj.SupplierID = objDTO.SupplierID;
                obj.AccountNo = objDTO.AccountNo;
                obj.IsDefault = objDTO.IsDefault;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.GUID = Guid.NewGuid();
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.Address = objDTO.Address;
                obj.City = objDTO.City;
                obj.State = objDTO.State;
                obj.ZipCode = objDTO.ZipCode;
                obj.AccountName = objDTO.AccountName;
                obj.ShipViaID = objDTO.ShipViaID;
                obj.Country = objDTO.Country;
                obj.ShipToID = objDTO.ShipToID;

                context.SupplierAccountDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                return obj.ID;
            }

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(SupplierAccountDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SupplierAccountDetail obj = context.SupplierAccountDetails.FirstOrDefault(t => t.ID == objDTO.ID);
                if (obj != null)
                {
                    obj.AccountName = objDTO.AccountName;
                    obj.AccountNo = objDTO.AccountNo;
                    obj.IsDefault = objDTO.IsDefault;
                    obj.Updated = objDTO.Updated;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.IsDeleted = objDTO.IsDeleted;
                    obj.Address = objDTO.Address;
                    obj.City = objDTO.City;
                    obj.State = objDTO.State;
                    obj.ZipCode = objDTO.ZipCode;
                    obj.ShipViaID = objDTO.ShipViaID;
                    obj.Country = objDTO.Country;
                    obj.ShipToID = objDTO.ShipToID;

                    context.SaveChanges();
                }
                return true;
            }
        }

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
                        strQuery += "UPDATE SupplierAccountDetails SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);


                return true;
            }
        }

        public List<SupplierAccountDetailsDTO> GetSupplierAccountDetails(long SupplierID, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<SupplierAccountDetailsDTO> lstSupplierAccountDetailsDTO = (from A in context.SupplierAccountDetails
                                                                                where A.SupplierID == SupplierID && !A.IsDeleted && !A.IsArchived
                                                                                && A.Room == RoomId && A.CompanyID == CompanyId
                                                                                select new SupplierAccountDetailsDTO
                                                                                {
                                                                                    ID = A.ID,
                                                                                    SupplierID = A.SupplierID,
                                                                                    AccountNo = A.AccountNo,
                                                                                    AccountName = A.AccountName != null ? (A.AccountName) : string.Empty,
                                                                                    Address = A.Address,
                                                                                    City = A.City,
                                                                                    State = A.State,
                                                                                    ZipCode = A.ZipCode,
                                                                                    IsDefault = A.IsDefault,
                                                                                    Country = A.Country,
                                                                                    ShipToID = A.ShipToID,

                                                                                }).ToList();
                return lstSupplierAccountDetailsDTO;
            }
        }


        public bool DeleteRecordsExcept(string IDs, long SupplierID, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                strQuery += "UPDATE SupplierAccountDetails SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE SupplierID = " + SupplierID.ToString() + "";

                if (IDs != "")
                {
                    strQuery += " AND ID Not in(";
                    foreach (var item in IDs.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            strQuery += item.ToString() + ",";
                        }
                    }
                    strQuery = strQuery.Substring(0, strQuery.Length - 1);
                    strQuery += ");";
                }

                context.Database.ExecuteSqlCommand(strQuery);
                return true;
            }
        }

        public List<RoomCompanyDTO> GetCompanyRoomBySupplierAccountNumber(string SupplierAccountNumber)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<RoomCompanyDTO> lstRoomCompany = (from SAD in context.SupplierAccountDetails
                                                       join C in context.CompanyMasters on SAD.CompanyID equals C.ID
                                                       join R in context.Rooms on SAD.Room equals R.ID
                                                       where (SAD.IsDeleted == false) && (SAD.IsArchived == false)
                                                             && (C.IsDeleted == null || C.IsDeleted == false) && (C.IsArchived == null || C.IsArchived == false)
                                                             && (R.IsDeleted == false) && (R.IsArchived == null || R.IsArchived == false)
                                                       where SAD.AccountNo.Trim().ToUpper() == SupplierAccountNumber.Trim().ToUpper()
                                                       select new RoomCompanyDTO()
                                                       {
                                                           RoomID = R.ID,
                                                           RoomName = R.RoomName,
                                                           CompanyID = C.ID,
                                                           CompanyName = C.Name
                                                       }
                 ).ToList();

                return lstRoomCompany;
            }
        }

        public SupplierAccountDetailsDTO GetFirstSupAccountByName(string SupplierAccountNumber)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@SupplierAcount", SupplierAccountNumber) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierAccountDetailsDTO>("exec GetSupplierACDetailsByName @SupplierAcount", params1).FirstOrDefault();
            }
        }

        public Guid? GetSupplierAccountGuid(long SupplierId, string SupplierAccountNumber, long RoomId, long CompanyId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@SupplierID", SupplierId),
                                                new SqlParameter("@SupplierAccountNumber", SupplierAccountNumber),
                                                new SqlParameter("@RoomID", RoomId),
                                                new SqlParameter("@CompanyID", CompanyId)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<Guid?>("exec GetSupplierAccountGuid @SupplierID,@SupplierAccountNumber,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public Guid CheckItemWiseSupplierAccountNumber(Guid ItemGUID, string SupplierAccountNumber, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@ItemGuid", ItemGUID),
                                                new SqlParameter("@SupplierAccountNumber", SupplierAccountNumber),
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<Guid>("exec CheckItemWiseSupplierAccountNumber @RoomID,@CompanyID,@ItemGuid,@SupplierAccountNumber", params1).FirstOrDefault();
            }
        }

        public SupplierAccountDetailsDTO GetSupplierRoomAndCompany(string SupplierAccountNumber)
        {
            var params1 = new SqlParameter[] {                                               
                                                new SqlParameter("@SupplierAccountNumber", SupplierAccountNumber),
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierAccountDetailsDTO>("exec GetSupplierRoomAndCompany @SupplierAccountNumber", params1).FirstOrDefault();
            }
        }
        
    }
}


