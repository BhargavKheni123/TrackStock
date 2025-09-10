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
    public class ReceivedToolAssetOrderTransferDetailDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ReceivedToolAssetOrderTransferDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ReceivedToolAssetOrderTransferDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        string stryQry = "EXEC [GetToolAssetReceivedOrderTransferDetail] @RoomID,@CompanyID,@ToolAssetOrderGuid,@ToolAssetOrderDetailGuid,@ToolGuid,@BinID,@IsDeleted,@IsArchived,@ROTDGuid,@CreatedFrom,@CreatedTo";

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> GetAllRecordsRoomAndCompanyWise(Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@ToolAssetOrderGuid", DBNull.Value),
                    new SqlParameter("@ToolAssetOrderDetailGuid", DBNull.Value),
                    new SqlParameter("@ItemGuid", DBNull.Value),
                    new SqlParameter("@ToolGuid", DBNull.Value),
                    new SqlParameter("@IsArchived", DBNull.Value),
                    new SqlParameter("@IsDeleted", DBNull.Value),
                    new SqlParameter("@ROTDGuid", DBNull.Value),
                    new SqlParameter("@CreatedFrom", DBNull.Value),
                    new SqlParameter("@CreatedTo", DBNull.Value)
                };
                IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> obj = context.Database.SqlQuery<ReceivedToolAssetOrderTransferDetailDTO>(stryQry, params1).ToList();
                return obj;

            }
        }

        public IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> GetAllRecordsRoomAndCompanyWise(Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived)
        {
            IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> lst = GetAllRecordsRoomAndCompanyWise(RoomID, CompanyID).Where(x => x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false);
            return lst;

        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> GetCachedData(Guid ToolGUID, Int64 RoomID, Int64 CompanyID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@ToolAssetOrderGuid", DBNull.Value),
                    new SqlParameter("@ToolAssetOrderDetailGuid", DBNull.Value),
                    new SqlParameter("@ToolGuid", ToolGUID),
                    new SqlParameter("@BinID", DBNull.Value),
                    new SqlParameter("@IsArchived", DBNull.Value),
                    new SqlParameter("@IsDeleted", DBNull.Value),
                    new SqlParameter("@ROTDGuid", DBNull.Value),
                    new SqlParameter("@CreatedFrom", DBNull.Value),
                    new SqlParameter("@CreatedTo", DBNull.Value)
                };
                IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> obj = context.Database.SqlQuery<ReceivedToolAssetOrderTransferDetailDTO>(stryQry, params1).ToList();
                return obj;
            }

        }

        /// <summary>
        /// Get Paged Records from the ReceivedOrderTransferDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, Guid ToolGUID, Guid? ToolAssetOrderDetailGuid, string ColumnName)
        {
            // var result = GetCachedData(RoomID, CompanyId).Where(t => (t.ItemID == ItemID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> result = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] objParams = new SqlParameter[] {
                    new SqlParameter("@RoomID",RoomID),
                    new SqlParameter("@CompanyID", CompanyId),
                    new SqlParameter("@ToolAssetOrderGuid", DBNull.Value),
                    new SqlParameter("@ToolAssetOrderDetailGuid", ToolAssetOrderDetailGuid??(object)DBNull.Value),
                    new SqlParameter("@ToolGuid", ToolGUID),
                    new SqlParameter("@BinID", DBNull.Value),
                    new SqlParameter("@IsArchived", false),
                    new SqlParameter("@IsDeleted", false),
                    new SqlParameter("@ROTDGuid", DBNull.Value),
                    new SqlParameter("@CreatedFrom", DBNull.Value),
                    new SqlParameter("@CreatedTo", DBNull.Value)
                };

                result = context.Database.SqlQuery<ReceivedToolAssetOrderTransferDetailDTO>(stryQry, objParams).OrderBy(ColumnName).ToList();
                return result;
            }

            /*
                 if (OrderDetailGUID == null || OrderDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                     result = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => ((t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
                 else
                     result = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => (t.OrderDetailGUID.GetValueOrDefault(Guid.Empty) == OrderDetailGUID.GetValueOrDefault(Guid.Empty) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);

                 return result;
             */
        }

        /// <summary>
        /// Get Paged Records from the ReceivedOrderTransferDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> GetAllRecordByOrderMasterGuid(Int64 RoomId, Int64 CompanyID, Guid OderGuid, string ColumnName)
        {
            // var result = GetCachedData(RoomID, CompanyId).Where(t => (t.ItemID == ItemID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> result = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] objParams = new SqlParameter[] {
                    new SqlParameter("@RoomID",RoomId),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@ToolAssetOrderGuid", OderGuid),
                    new SqlParameter("@ToolAssetOrderDetailGuid",DBNull.Value),
                    new SqlParameter("@ToolGuid", DBNull.Value),
                    new SqlParameter("@BinID", DBNull.Value),
                    new SqlParameter("@IsArchived", false),
                    new SqlParameter("@IsDeleted", false),
                    new SqlParameter("@ROTDGuid", DBNull.Value),
                    new SqlParameter("@CreatedFrom", DBNull.Value),
                    new SqlParameter("@CreatedTo", DBNull.Value)
                };

                result = context.Database.SqlQuery<ReceivedToolAssetOrderTransferDetailDTO>(stryQry, objParams).OrderBy(ColumnName).ToList();
                return result;
            }


        }
        /// <summary>
        /// Get Paged Records from the ReceivedOrderTransferDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> GetAllRecordByOrderDetailGuid(Int64 RoomId, Int64 CompanyID, Guid OrderDetailGuid, string ColumnName)
        {
            // var result = GetCachedData(RoomID, CompanyId).Where(t => (t.ItemID == ItemID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> result = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter[] objParams = new SqlParameter[] {
                    new SqlParameter("@RoomID",RoomId),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@ToolAssetOrderGuid", DBNull.Value),
                    new SqlParameter("@ToolAssetOrderDetailGuid",OrderDetailGuid),
                    new SqlParameter("@ToolGuid", DBNull.Value),
                    new SqlParameter("@BinID", DBNull.Value),
                    new SqlParameter("@IsArchived", false),
                    new SqlParameter("@IsDeleted", false),
                    new SqlParameter("@ROTDGuid", DBNull.Value),
                    new SqlParameter("@CreatedFrom", DBNull.Value),
                    new SqlParameter("@CreatedTo", DBNull.Value)
                };

                result = context.Database.SqlQuery<ReceivedToolAssetOrderTransferDetailDTO>(stryQry, objParams).OrderBy(ColumnName).ToList();
                return result;
            }


        }

        /// <summary>
        /// Get Paged Records from the ReceivedOrderTransferDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ItemGUID, Guid? OrderDetailGUID)
        {
            //Get Cached-Media
            IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> ObjCache = GetCachedData(ItemGUID, RoomID, CompanyID);
            IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> ObjGlobalCache = null;

            if (OrderDetailGUID == null)
            {
                ObjGlobalCache = ObjCache;
                ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false && t.ToolGUID == ItemGUID));
            }
            else
            {
                ObjGlobalCache = ObjCache.Where(t => t.ToolAssetOrderDetailGUID == OrderDetailGUID);
                ObjCache = ObjCache.Where(t => (t.IsArchived.GetValueOrDefault(false) == false && t.IsDeleted.GetValueOrDefault(false) == false && t.ToolAssetOrderDetailGUID == OrderDetailGUID));
            }

            if (IsArchived && IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsArchived)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<ReceivedOrderTransferDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<ReceivedOrderTransferDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).Count();
                return ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        /// <summary>
        /// Get Particullar Record from the ReceivedOrderTransferDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ReceivedToolAssetOrderTransferDetailDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@ToolAssetOrderGuid", DBNull.Value),
                    new SqlParameter("@ToolAssetOrderDetailGuid", DBNull.Value),
                    new SqlParameter("@ToolGuid", DBNull.Value),
                    new SqlParameter("@BinID", DBNull.Value),
                    new SqlParameter("@IsArchived", DBNull.Value),
                    new SqlParameter("@IsDeleted", DBNull.Value),
                    new SqlParameter("@ROTDGuid", GUID),
                    new SqlParameter("@CreatedFrom", DBNull.Value),
                    new SqlParameter("@CreatedTo",  DBNull.Value)
                };
                ReceivedToolAssetOrderTransferDetailDTO obj = context.Database.SqlQuery<ReceivedToolAssetOrderTransferDetailDTO>(stryQry, params1).FirstOrDefault();
                return obj;
            }

        }

        /// <summary>
        /// Insert Record in the DataBase ReceivedOrderTransferDetails
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(ToolAssetQuantityDetailDTO objDTO)
        {

            ReceivedToolAssetOrderTransferDetailDTO objNewDTO = new ReceivedToolAssetOrderTransferDetailDTO()
            {
                Action = objDTO.Action,
                ToolBinID = objDTO.ToolBinID,

                CompanyID = objDTO.CompanyID,
                Quantity = objDTO.Quantity,
                Cost = objDTO.Cost,
                Created = DateTimeUtility.DateTimeNow,
                CreatedBy = objDTO.CreatedBy,
                CreatedByName = objDTO.CreatedByName,

                GUID = Guid.NewGuid(),
                HistoryID = 0,
                ID = 0,
                IsArchived = false,

                IsDeleted = false,
                ToolGUID = objDTO.ToolGUID,
                ToolAssetOrderDetailGUID = objDTO.GUID,
                ToolName = objDTO.ToolName,
                Serial = objDTO.Serial,
                Type = objDTO.Type,

                LastUpdatedBy = objDTO.UpdatedBy,

                ReceivedDate = objDTO.ReceivedDate,
                Room = objDTO.RoomID,
                RoomName = objDTO.RoomName,

                Updated = objDTO.Updated,
                UpdatedByName = objDTO.UpdatedByName,
                PackSlipNumber = objDTO.PackSlipNumber,
                AddedFrom = (objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom),
                EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom),
                ReceivedOn = objDTO.ReceivedOn,
                ReceivedOnWeb = objDTO.ReceivedOnWeb,
                SerialNumber = objDTO.SerialNumber,
                LotNumber = objDTO.LotNumber,
            };

            objNewDTO.Updated = DateTimeUtility.DateTimeNow;
            objNewDTO.Created = DateTimeUtility.DateTimeNow;


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReceivedToolAssetOrderTransferDetail obj = new ReceivedToolAssetOrderTransferDetail();
                obj.ID = 0;

                obj.ToolBinID = objNewDTO.ToolBinID;
                obj.Quantity = objNewDTO.Quantity;


                obj.Cost = objNewDTO.Cost;

                obj.UDF1 = objNewDTO.UDF1;
                obj.UDF2 = objNewDTO.UDF2;
                obj.UDF3 = objNewDTO.UDF3;
                obj.UDF4 = objNewDTO.UDF4;
                obj.UDF5 = objNewDTO.UDF5;
                obj.GUID = Guid.NewGuid();
                obj.ToolGUID = objNewDTO.ToolGUID;
                obj.Created = objNewDTO.Created;
                obj.Updated = objNewDTO.Updated;
                obj.CreatedBy = objNewDTO.CreatedBy;
                obj.LastUpdatedBy = objNewDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objNewDTO.CompanyID;
                obj.Room = objNewDTO.Room;
                obj.ToolAssetOrderDetailGUID = objNewDTO.OrderDetailGUID;

                obj.PackSlipNumber = objNewDTO.PackSlipNumber;


                if (!string.IsNullOrEmpty(objNewDTO.Received))
                {
                    obj.Received = objNewDTO.Received.Replace("-", "/");
                }
                obj.ToolAssetOrderDetailGUID = objNewDTO.ToolAssetOrderDetailGUID;
                obj.ReceivedDate = objNewDTO.ReceivedDate;
                obj.AddedFrom = (objNewDTO.AddedFrom == null ? "Web" : objNewDTO.AddedFrom);
                obj.EditedFrom = (objNewDTO.EditedFrom == null ? "Web" : objNewDTO.EditedFrom);
                obj.ReceivedOn = objNewDTO.ReceivedOn == null ? DateTimeUtility.DateTimeNow : Convert.ToDateTime(objNewDTO.ReceivedOn);
                obj.ReceivedOnWeb = objNewDTO.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : Convert.ToDateTime(objNewDTO.ReceivedOnWeb);

                obj.SerialNumber = objNewDTO.SerialNumber;
                obj.LotNumber = objNewDTO.LotNumber;

                context.ReceivedToolAssetOrderTransferDetails.Add(obj);
                context.SaveChanges();
                objNewDTO.ID = obj.ID;
                objNewDTO.GUID = obj.GUID;



                return objDTO.ID;
            }
        }

        /// <summary>
        /// Insert Record in the DataBase ReceivedOrderTransferDetails
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(ReceivedToolAssetOrderTransferDetailDTO objNewDTO)
        {

            objNewDTO.Updated = DateTimeUtility.DateTimeNow;
            objNewDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReceivedToolAssetOrderTransferDetail obj = new ReceivedToolAssetOrderTransferDetail();
                obj.ID = 0;
                obj.ToolBinID = objNewDTO.ToolBinID;
                obj.Quantity = objNewDTO.Quantity;

                obj.Cost = objNewDTO.Cost;

                obj.UDF1 = objNewDTO.UDF1;
                obj.UDF2 = objNewDTO.UDF2;
                obj.UDF3 = objNewDTO.UDF3;
                obj.UDF4 = objNewDTO.UDF4;
                obj.UDF5 = objNewDTO.UDF5;
                obj.GUID = Guid.NewGuid();
                obj.ToolGUID = objNewDTO.ToolGUID;
                obj.Created = objNewDTO.Created;
                obj.Updated = objNewDTO.Updated;
                obj.CreatedBy = objNewDTO.CreatedBy;
                obj.LastUpdatedBy = objNewDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objNewDTO.CompanyID;
                obj.Room = objNewDTO.Room;
                obj.ToolAssetOrderDetailGUID = objNewDTO.ToolAssetOrderDetailGUID;

                obj.PackSlipNumber = objNewDTO.PackSlipNumber;


                if (!string.IsNullOrEmpty(objNewDTO.Received))
                {
                    obj.Received = objNewDTO.Received.Replace("-", "/");
                }
                obj.ToolAssetQuantityDetailGUID = objNewDTO.ToolAssetQuantityDetailGUID;
                obj.ReceivedDate = objNewDTO.ReceivedDate;

                obj.AddedFrom = (objNewDTO.AddedFrom == null ? "Web" : objNewDTO.AddedFrom);
                obj.EditedFrom = (objNewDTO.EditedFrom == null ? "Web" : objNewDTO.EditedFrom);
                obj.ReceivedOn = objNewDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow);
                obj.ReceivedOnWeb = objNewDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow);
                obj.SerialNumber = objNewDTO.SerialNumber;
                obj.LotNumber = objNewDTO.LotNumber;

                context.ReceivedToolAssetOrderTransferDetails.Add(obj);
                context.SaveChanges();
                objNewDTO.ID = obj.ID;
                objNewDTO.GUID = obj.GUID;



                return objNewDTO.ID;
            }
        }


        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(ToolAssetQuantityDetailDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReceivedToolAssetOrderTransferDetailDTO objNewDTO = GetAllRecords(objDTO.RoomID, objDTO.CompanyID, objDTO.ToolGUID.GetValueOrDefault(Guid.Empty), objDTO.ToolAssetOrderDetailGUID.GetValueOrDefault(Guid.Empty), "ID Desc").FirstOrDefault(x => x.OrderDetailGUID == objDTO.GUID);
                if (objNewDTO == null)
                    return false;

                ReceivedToolAssetOrderTransferDetail obj = new ReceivedToolAssetOrderTransferDetail();
                obj.ID = objNewDTO.ID;
                obj.ToolBinID = objNewDTO.ToolBinID;

                obj.Quantity = objNewDTO.Quantity;

                obj.Cost = objNewDTO.Cost;

                obj.UDF1 = objNewDTO.UDF1;
                obj.UDF2 = objNewDTO.UDF2;
                obj.UDF3 = objNewDTO.UDF3;
                obj.UDF4 = objNewDTO.UDF4;
                obj.UDF5 = objNewDTO.UDF5;
                obj.GUID = objNewDTO.GUID;
                obj.ToolGUID = objNewDTO.ToolGUID;
                obj.Created = objNewDTO.Created;
                obj.Updated = objNewDTO.Updated;
                obj.CreatedBy = objNewDTO.CreatedBy;
                obj.LastUpdatedBy = objNewDTO.LastUpdatedBy;
                obj.PackSlipNumber = objNewDTO.PackSlipNumber;
                obj.IsDeleted = objNewDTO.IsDeleted.HasValue ? (bool)objNewDTO.IsDeleted : false;
                obj.IsArchived = objNewDTO.IsArchived.HasValue ? (bool)objNewDTO.IsArchived : false;
                obj.CompanyID = objNewDTO.CompanyID;
                obj.Room = objNewDTO.Room;
                obj.ToolAssetOrderDetailGUID = objNewDTO.OrderDetailGUID;
                obj.SerialNumber = objNewDTO.SerialNumber;
                obj.LotNumber = objNewDTO.LotNumber;


                if (!string.IsNullOrEmpty(objNewDTO.Received))
                {
                    obj.Received = objNewDTO.Received.Replace("-", "/");
                }

                obj.AddedFrom = (objNewDTO.AddedFrom == null ? "Web" : objNewDTO.AddedFrom);
                obj.ReceivedOnWeb = objNewDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow);

                obj.EditedFrom = (objNewDTO.EditedFrom == null ? "Web" : objNewDTO.EditedFrom);
                obj.ReceivedOn = objNewDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow);


                obj.ReceivedDate = objNewDTO.ReceivedDate;
                obj.ReceivedDate = objNewDTO.ReceivedDate;
                obj.IsEDISent = objNewDTO.IsEDISent;
                obj.LastEDIDate = objNewDTO.LastEDIDate;
                context.ReceivedToolAssetOrderTransferDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();


                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 RoomID, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string strItemLocationIDs = "";

                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        ReceivedToolAssetOrderTransferDetailDTO objDTO = GetRecord(Guid.Parse(item), RoomID, CompanyID);
                        LocationMasterDTO binDTO = new LocationMasterDAL(base.DataBaseName).GetLocationByIDPlain(objDTO.ToolBinID.GetValueOrDefault(0), RoomID, CompanyID);
                        //BinMasterDTO binDTO = new BinMasterDAL(base.DataBaseName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, objDTO.BinID.GetValueOrDefault(0),null,null).FirstOrDefault();


                        if (!string.IsNullOrEmpty(strItemLocationIDs))
                            strItemLocationIDs += ",";

                        // strItemLocationIDs += objDTO.LocationGUID;
                        //need to update

                        strQuery += "UPDATE ReceivedToolAssetOrderTransferDetail SET  Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE   GUID ='" + item.ToString() + "';";
                    }
                }
                if (context.Database.ExecuteSqlCommand(strQuery) > 0)
                {
                    if (!string.IsNullOrEmpty(strItemLocationIDs))
                    {

                        // ToolAssetQuantityDetailDAL itemLocDal = new ToolAssetQuantityDetailDAL(base.DataBaseName);
                        //  itemLocDal.DeleteRecords(strItemLocationIDs, userid, RoomID, CompanyID, "ReceiveToolAssetOrderTransfer Delete Records");

                    }


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
        public bool DeleteRecievedRecords(Guid[] guIDs, bool IsStaging, Int64 userid, Int64 RoomID, Int64 CompanyID)
        {
            string strQuery = "UPDATE ReceivedToolAssetOrderTransferDetail SET  Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted = 1 WHERE ISNULL(IsEDISent,0) = 0 AND GUID  IN (";
            string strItemLocationIDs = "";

            try
            {

                return true;

            }
            finally
            {
                strQuery = string.Empty;
                strItemLocationIDs = string.Empty;

            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteReturnedRecords(Guid[] guIDs, bool IsStaging, Int64 userid, Int64 RoomID, Int64 CompanyID)
        {
            //ItemLocationDetailsDTO itmLocDTO = null;
            List<ItemLocationDetailsDTO> itemLocDTOList = new List<ItemLocationDetailsDTO>();
            try
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {


                    return true;

                }
            }
            finally
            {
                // itmLocDTO = null;

            }
        }

        /// <summary>
        /// Set RecieveOrderTransferDetail
        /// WI-1298
        /// </summary>
        /// <param name="rotd"></param>
        private ReceivedToolAssetOrderTransferDetail SetRecieveOrderTransferDetail(ReceivedToolAssetOrderTransferDetail objROTD, ReceivedToolAssetOrderTransferDetailDTO objNewDTO)
        {
            objROTD.ToolBinID = objNewDTO.ToolBinID;

            objROTD.Quantity = objNewDTO.Quantity;

            objROTD.Cost = objNewDTO.Cost;
            objROTD.UDF1 = objNewDTO.UDF1;
            objROTD.UDF2 = objNewDTO.UDF2;
            objROTD.UDF3 = objNewDTO.UDF3;
            objROTD.UDF4 = objNewDTO.UDF4;
            objROTD.UDF5 = objNewDTO.UDF5;
            objROTD.SerialNumber = objNewDTO.SerialNumber;
            objROTD.LotNumber = objNewDTO.LotNumber;
            objROTD.PackSlipNumber = objNewDTO.PackSlipNumber;

            objROTD.IsDeleted = false;
            objROTD.IsArchived = false;


            if (!string.IsNullOrEmpty(objNewDTO.Received))
            {
                objROTD.Received = objNewDTO.Received.Replace("-", "/");
            }
            objROTD.ReceivedDate = objNewDTO.ReceivedDate;

            objROTD.Updated = DateTimeUtility.DateTimeNow;
            objROTD.LastUpdatedBy = objNewDTO.LastUpdatedBy;

            return objROTD;
        }

        /// <summary>
        /// Set ItemLocationDetail
        /// WI-1298
        /// </summary>
        /// <param name="rotd"></param>
        private ToolAssetQuantityDetail SetItemLocationDetail(ToolAssetQuantityDetail objILD, ReceivedToolAssetOrderTransferDetailDTO objNewDTO, double PrevReceivedQty, ToolAssetOrderType ot, bool IsBinDeleted)
        {
            ToolAssetOrderDetailsDTO ordDetailDTO = null;
            ToolAssetOrderDetailsDAL ordDetailDAL = new ToolAssetOrderDetailsDAL(base.DataBaseName);
            ordDetailDTO = ordDetailDAL.GetRecord(objNewDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty), objNewDTO.Room.GetValueOrDefault(0), objNewDTO.CompanyID.GetValueOrDefault(0));
            ToolAssetOrderMasterDTO ordDTO = null;
            ToolAssetOrderMasterDAL ordDAL = new ToolAssetOrderMasterDAL(base.DataBaseName);
            ordDTO = ordDAL.GetRecord(ordDetailDTO.ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty), objNewDTO.Room.GetValueOrDefault(0), objNewDTO.CompanyID.GetValueOrDefault(0));
            if (ordDTO.OrderType.GetValueOrDefault(1) == 1 && !IsBinDeleted)
            {
                objILD.Quantity = (objILD.Quantity.GetValueOrDefault(0) - (PrevReceivedQty)) + objNewDTO.Quantity.GetValueOrDefault(0);

            }

            if (ot == ToolAssetOrderType.Order && !IsBinDeleted)
            {
                objILD.InitialQuantityWeb = (objNewDTO.Quantity.GetValueOrDefault(0));

                if (objILD.InitialQuantityWeb > 0)
                    objILD.InitialQuantityWeb = (objNewDTO.Quantity.GetValueOrDefault(0));
                else if (objILD.InitialQuantityPDA > 0)
                    objILD.InitialQuantityPDA = (objNewDTO.Quantity.GetValueOrDefault(0));
            }

            objILD.ReceivedDate = objNewDTO.ReceivedDate;
            objILD.Cost = objNewDTO.Cost;
            objILD.Description = objNewDTO.Description;
            objILD.UpdatedBy = objNewDTO.LastUpdatedBy ?? 0;
            objILD.Updated = DateTimeUtility.DateTimeNow;
            objILD.EditedFrom = "Web";
            objILD.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            return objILD;
        }

        /// <summary>
        /// Set ItemLocationQty
        /// WI-1298
        /// </summary>
        /// <param name="rotd"></param>
        private ToolAssetQuantityDetail SetItemLocationQty(ToolAssetQuantityDetail objItemLocatinQty, ReceivedToolAssetOrderTransferDetailDTO objNewDTO, double PrevCustQty, double PrevConsQty)
        {

            objItemLocatinQty.Quantity = (objItemLocatinQty.Quantity.GetValueOrDefault(0) - PrevCustQty) + objNewDTO.Quantity.GetValueOrDefault(0);
            objItemLocatinQty.Quantity = objItemLocatinQty.Quantity.GetValueOrDefault(0);

            objItemLocatinQty.Updated = DateTimeUtility.DateTimeNow;
            objItemLocatinQty.UpdatedBy = objNewDTO.LastUpdatedBy ?? 0;
            if (string.IsNullOrEmpty(objItemLocatinQty.AddedFrom))
            {
                objItemLocatinQty.AddedFrom = "Web";
                objItemLocatinQty.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            }


            return objItemLocatinQty;
        }

        /// <summary>
        /// Edit Received Record
        /// Dated: 01-Dec-2014
        /// </summary>
        /// <param name="objDTO"></param>
        public void Edit(List<ReceivedToolAssetOrderTransferDetailDTO> ROTDs)
        {

            if (ROTDs == null || ROTDs.Count <= 0)
                return;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReceivedToolAssetOrderTransferDetail objROTD = null;
                ToolAssetQuantityDetail objILD = null;
                ToolAssetOrderDetail objOD = null;
                ToolMasterDAL objItemDAL = null;
                double PrevReceivedCustOwnQty = 0;
                List<ToolAssetQuantityDetailDTO> objListQty = new List<ToolAssetQuantityDetailDTO>();

                foreach (var objNewDTO in ROTDs)
                {
                    try
                    {
                        //Step 1: SET ReceivedOrderTransferDetail Table record
                        objROTD = context.ReceivedToolAssetOrderTransferDetails.FirstOrDefault(x => x.GUID == objNewDTO.GUID);
                        PrevReceivedCustOwnQty = objROTD.Quantity.GetValueOrDefault(0);

                        objROTD = SetRecieveOrderTransferDetail(objROTD, objNewDTO);
                        //Step 2: SET Respected Location Detail Record. UpdateReceivedOrderTransferDetail Table record

                        objILD = objNewDTO.ToolAssetQuantityDetailGUID != Guid.Empty ?
                            context.ToolAssetQuantityDetails.FirstOrDefault(x => x.GUID == objNewDTO.ToolAssetQuantityDetailGUID)
                            : context.ToolAssetQuantityDetails.FirstOrDefault(x => x.ToolAssetOrderDetailGUID == objNewDTO.ToolAssetOrderDetailGUID);

                        ToolAssetOrderDetail od = context.ToolAssetOrderDetails.FirstOrDefault<ToolAssetOrderDetail>(x => x.GUID == (objNewDTO.OrderDetailGUID ?? Guid.Empty));
                        ToolAssetOrderMaster om = context.ToolAssetOrderMasters.FirstOrDefault(x => x.GUID == (od.ToolAssetOrderGUID ?? Guid.Empty));
                        LocationMaster bin = null;
                        if (objILD != null)
                        {
                            bin = context.LocationMasters.FirstOrDefault(x => x.GUID == (context.ToolLocationDetails.Where(c => c.ID == objILD.ToolBinID).FirstOrDefault().LocationGuid));
                            bool IsBinDelete = false;
                            if (bin != null && bin.ID > 0)
                                IsBinDelete = bin.IsDeleted ?? false;

                            objILD = SetItemLocationDetail(objILD, objNewDTO, PrevReceivedCustOwnQty, (ToolAssetOrderType)om.OrderType, IsBinDelete);
                        }
                        else
                        {

                        }

                        context.SaveChanges();

                        ////Step 3: Delete & Insert ItemLocationQty

                        //objLocQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
                        //objListQty = objLocQtyDAL.GetListOfItemLocQty(objNewDTO.ItemGUID ?? Guid.Empty, objNewDTO.Room.GetValueOrDefault(0), objNewDTO.CompanyID.GetValueOrDefault(0), objNewDTO.LastUpdatedBy.GetValueOrDefault(0));

                        //if (objListQty != null && objListQty.Count() > 0)
                        //{
                        //    objLocQtyDAL.Save(objListQty);
                        //}
                        //if (objILD != null)
                        //{
                        //    IEnumerable<ToolAssetQuantityDetail> objItemLocationQtyDelete = context.ToolAssetQuantityDetails.Where(x => x.ToolGUID == objNewDTO.ToolGUID);
                        //    foreach (var item in objItemLocationQtyDelete)
                        //    {
                        //        context.ToolAssetQuantityDetails.Remove(item);
                        //    }

                        //    IEnumerable<ToolAssetQuantityDetail> objItemLocationQtyAdd = GetToolAssetQuantity(objNewDTO, context);
                        //    foreach (var item in objItemLocationQtyAdd)
                        //    {
                        //        context.ToolAssetQuantityDetails.Add(item);
                        //    }
                        //}

                        context.SaveChanges();

                        //Step 4: Update Order detail record for Update Received quantity
                        objOD = context.ToolAssetOrderDetails.FirstOrDefault(x => x.GUID == objNewDTO.OrderDetailGUID);
                        objOD.ReceivedQuantity = context.ReceivedToolAssetOrderTransferDetails.Where(x => x.ToolAssetOrderDetailGUID == objNewDTO.OrderDetailGUID && !(x.IsDeleted ?? false) && !(x.IsArchived ?? false)).Sum(x => ((x.Quantity ?? 0)));
                        objOD.LastUpdated = DateTimeUtility.DateTimeNow;
                        objOD.LastUpdatedBy = objNewDTO.LastUpdatedBy;
                        //if (objNewDTO.IsOnlyFromUI)
                        //{
                        //    objOD.EditedFrom = "Web";
                        //    objOD.ReceivedOn = DateTimeUtility.DateTimeNow;
                        //}

                        objOD.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        context.SaveChanges();

                        //Step 5: Update Order status
                        ToolAssetOrderDetailsDAL ordDetailDAL = new DAL.ToolAssetOrderDetailsDAL(base.DataBaseName);
                        ordDetailDAL.UpdateOrderStatusByReceive(objOD.ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty), objNewDTO.Room.GetValueOrDefault(0), objNewDTO.CompanyID.GetValueOrDefault(0), objNewDTO.LastUpdatedBy.GetValueOrDefault(0));




                        //Step 8: Update Cost mark up cell price and update cart for edited item.
                        UpdateToolMaster(objNewDTO, context);

                        objItemDAL = new ToolMasterDAL(base.DataBaseName);
                        objItemDAL.UpdateToolCost(objNewDTO.ToolGUID ?? Guid.Empty, objNewDTO.Room.GetValueOrDefault(0), objNewDTO.CompanyID.GetValueOrDefault(0));

                    }
                    finally
                    {
                        objROTD = null;
                        objILD = null;
                        objOD = null;
                        objItemDAL = null;
                    }
                }
            }
        }

        /// <summary>
        /// Edit Received Record
        /// Dated: 01-Dec-2014
        /// </summary>
        /// <param name="objDTO"></param>
        public void DeleteReturnedItems(ReceivedToolAssetOrderTransferDetailDTO ROTDs, Int64 UserID)
        {

            if (ROTDs == null)
                return;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                ReceivedToolAssetOrderTransferDetail objROTD = null;
                ToolAssetQuantityDetail objILD = null;

                ToolMasterDAL objItemDAL = null;
                ToolAssetOrderDetailsDAL objLocQtyDAL = null;
                double ReturnedCustOwnQty = 0;
                List<ItemLocationQTYDTO> objListQty = new List<ItemLocationQTYDTO>();

                try
                {
                    //Step 1: SET ReceivedOrderTransferDetail Table record
                    objROTD = context.ReceivedToolAssetOrderTransferDetails.FirstOrDefault(x => x.GUID == ROTDs.GUID);
                    ReturnedCustOwnQty = objROTD.Quantity.GetValueOrDefault(0);

                    objROTD.IsDeleted = true;
                    objROTD.EditedFrom = "Web";
                    objROTD.Updated = DateTimeUtility.DateTimeNow;
                    objROTD.LastUpdatedBy = UserID;
                    objROTD.ReceivedOn = DateTimeUtility.DateTimeNow;
                    context.SaveChanges();

                    //Step 2: SET Respected Location Detail Record. UpdateReceivedOrderTransferDetail Table record
                    objILD = context.ToolAssetQuantityDetails.FirstOrDefault(x => x.GUID == ROTDs.ToolAssetOrderDetailGUID);
                    objILD.Quantity = (objILD.Quantity + ReturnedCustOwnQty);


                    objILD.UpdatedBy = UserID;
                    objILD.Updated = DateTimeUtility.DateTimeNow;

                    objILD.EditedFrom = "Web";
                    objILD.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    context.SaveChanges();

                    objLocQtyDAL = new ToolAssetOrderDetailsDAL(base.DataBaseName);
                    // objListQty = objLocQtyDAL.GetListOfItemLocQty(ROTDs.ToolGUID ?? Guid.Empty, ROTDs.Room.GetValueOrDefault(0), ROTDs.CompanyID.GetValueOrDefault(0), UserID);

                    //if (objListQty != null && objListQty.Count() > 0)
                    //{
                    //    objLocQtyDAL.Save(objListQty);
                    //}

                    //Step 5: Update Cost mark up cell price and update cart for edited item.
                    objItemDAL = new ToolMasterDAL(base.DataBaseName);
                    objItemDAL.UpdateToolCost(ROTDs.ToolGUID ?? Guid.Empty, ROTDs.Room.GetValueOrDefault(0), ROTDs.CompanyID.GetValueOrDefault(0));





                }
                finally
                {
                    objROTD = null;
                    objILD = null;
                    objItemDAL = null;
                }

            }
        }

        /// <summary>
        /// Insert Record in the DataBase ReceivedOrderTransferDetails
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 InsertReceive(ReceivedToolAssetOrderTransferDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.CommandTimeout = 150;
                ToolAssetQuantityDetail objILD = GetToolLocationDetailObject(objDTO);
                context.ToolAssetQuantityDetails.Add(objILD);

                ReceivedToolAssetOrderTransferDetail objROTD = GetReceiveOrderTransferDetailObject(objDTO);

                objROTD.ToolBinID = objILD.ToolBinID;
                objROTD.ToolAssetQuantityDetailGUID = objILD.GUID;
                context.ReceivedToolAssetOrderTransferDetails.Add(objROTD);

                PreReceivOrderToolDetail objPreRecieve = GetPreReceiveDetail(objDTO, context);
                context.SaveChanges();



                //IEnumerable<ToolLocationDetail> objItemLocationQtyDelete = context.ToolLocationDetails.Where(x => x.ToolGuid == objDTO.ToolGUID);
                //foreach (var item in objItemLocationQtyDelete)
                //{
                //    context.ToolLocationDetails.Remove(item);
                //}

                //UpdateToolLocationDetail();
                //IEnumerable<ToolLocationDetail> objItemLocationQtyAdd = GetItemLocationQty(objDTO, context);
                //foreach (var item in objItemLocationQtyAdd)
                //{
                //    //if (item.BinID == objROTD.BinID)
                //    //{
                //    //    item.CustomerOwnedQuantity = (item.CustomerOwnedQuantity ?? 0) + (objROTD.CustomerOwnedQuantity ?? 0);
                //    //    item.ConsignedQuantity = (item.ConsignedQuantity ?? 0) + (objROTD.ConsignedQuantity ?? 0);
                //    //    item.Quantity = (item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0);
                //    //}

                //    context.ToolLocationDetails.Add(item);
                //}

                //context.SaveChanges();
                //ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                //objToolAssetQuantityDetailDTO.ToolGUID = objDTO.ToolGUID;

                //objToolAssetQuantityDetailDTO.AssetGUID = null;
                //objToolAssetQuantityDetailDTO.Cost = objDTO.Cost;


                //objToolAssetQuantityDetailDTO.ToolBinID = objILD != null ? objILD.ID : 0;
                //objToolAssetQuantityDetailDTO.Quantity = objDTO.Quantity ?? 0;
                //objToolAssetQuantityDetailDTO.RoomID = objDTO.Room ?? 0;
                //objToolAssetQuantityDetailDTO.CompanyID = objDTO.CompanyID ?? 0;
                //objToolAssetQuantityDetailDTO.Created = objDTO.Created ?? DateTimeUtility.DateTimeNow;
                //objToolAssetQuantityDetailDTO.Updated = objDTO.Updated ?? DateTimeUtility.DateTimeNow;
                //objToolAssetQuantityDetailDTO.ReceivedOnWeb = objDTO.ReceivedOnWeb ?? DateTimeUtility.DateTimeNow;
                //objToolAssetQuantityDetailDTO.ReceivedOn = objDTO.ReceivedOn ?? DateTimeUtility.DateTimeNow;
                //objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                //objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                //objToolAssetQuantityDetailDTO.WhatWhereAction = "ReceiveToolAssetOrderTransferDetailDAL>>InsertReceive";
                //objToolAssetQuantityDetailDTO.ReceivedDate = null;
                //objToolAssetQuantityDetailDTO.InitialQuantityWeb = objDTO.Quantity ?? 0;
                //objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                //objToolAssetQuantityDetailDTO.ExpirationDate = null;
                //objToolAssetQuantityDetailDTO.EditedOnAction = "Receive Quantity From Order.";
                //objToolAssetQuantityDetailDTO.CreatedBy = objDTO.LastUpdatedBy ?? 0;
                //objToolAssetQuantityDetailDTO.UpdatedBy = objDTO.LastUpdatedBy ?? 0;
                //objToolAssetQuantityDetailDTO.IsDeleted = false;
                //objToolAssetQuantityDetailDTO.IsArchived = false;

                //ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(base.DataBaseName);
                //objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO);

                objDTO.ID = objROTD.ID;
                objDTO.GUID = objROTD.GUID;


                IEnumerable<ReceivedToolAssetOrderTransferDetail> lstROTDs = context.ReceivedToolAssetOrderTransferDetails.Where(x => (x.ToolAssetOrderDetailGUID == objDTO.ToolAssetOrderDetailGUID) && !(x.IsDeleted ?? false) && !(x.IsArchived ?? false));
                double receivedQuantity = lstROTDs.Sum(x => (x.Quantity ?? 0));

                UpdateOrderDetails(objDTO, receivedQuantity);

                UpdateToolMaster(objDTO, context);

                return objDTO.ID;
            }


        }

        private void UpdateOrderDetails(ReceivedToolAssetOrderTransferDetailDTO objDTO, double RcvQty)
        {
            ToolAssetOrderDetailsDAL ordDetailDAL = new ToolAssetOrderDetailsDAL(base.DataBaseName);
            ToolAssetOrderDetailsDTO OrdDetailDTO = ordDetailDAL.GetRecord(objDTO.ToolAssetOrderDetailGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
            OrdDetailDTO.ReceivedQuantity = RcvQty;

            OrdDetailDTO.IsEDISent = objDTO.IsEDISent;
            OrdDetailDTO.LastEDIDate = objDTO.LastEDIDate;
            OrdDetailDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
            OrdDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            OrdDetailDTO.EditedFrom = objDTO.EditedFrom;
            OrdDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            ordDetailDAL.UpdateOrderDetail(OrdDetailDTO);
        }

        private void UpdateToolMaster(ReceivedToolAssetOrderTransferDetailDTO objDTO, eTurnsEntities context)
        {
            ToolMasterDAL objItem = new ToolMasterDAL(base.DataBaseName);
            ToolMasterDTO oItemMaster = objItem.GetToolByGUIDPlain(objDTO.ToolGUID.GetValueOrDefault(Guid.Empty));
            //Guid? UsedToolGUId = objItem.GetUsedToolGuidinQuantity(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, Guid.Empty, oItemMaster.ToolName);
            Guid? UsedToolGUId = Guid.Empty;
            ToolMasterDTO oItemMasterQTYUsed = null;

            if (oItemMaster != null && UsedToolGUId.GetValueOrDefault(Guid.Empty) != oItemMaster.GUID)
            {
                oItemMasterQTYUsed = objItem.GetToolByGUIDPlain(UsedToolGUId.GetValueOrDefault(Guid.Empty));
            }


            if ((oItemMaster != null && (string.IsNullOrWhiteSpace(oItemMaster.Serial))) || oItemMaster.SerialNumberTracking == false)
            {




                //ItemMaster oItemMaster = context.ItemMasters.Where(x => x.GUID == objDTO.ItemGUID && x.CompanyID == objDTO.CompanyID && x.Room == objDTO.Room).FirstOrDefault();
                Room oRoom = context.Rooms.Where(x => x.ID == oItemMaster.Room && x.CompanyID == oItemMaster.CompanyID).FirstOrDefault();
                //if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString() )
                //    objItem.UpdateItemAndPastReceiptCostByReceiveCost(oItemMaster.GUID, oItemMaster.Room.GetValueOrDefault(0), oItemMaster.CompanyID.GetValueOrDefault(0), false, objDTO.EditedFrom, objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow));
                //else if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() )
                //    objItem.UpdateItemAndPastReceiptCostByReceiveCost(oItemMaster.GUID, oItemMaster.Room.GetValueOrDefault(0), oItemMaster.CompanyID.GetValueOrDefault(0), true, objDTO.EditedFrom, objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow));
                if (oItemMaster.SerialNumberTracking == true)
                {
                    oItemMaster.Serial = objDTO.SerialNumber;
                }
                //if (oItemMaster.SerialNumberTracking == false && UsedToolGUId.GetValueOrDefault(Guid.Empty) == oItemMaster.GUID)
                if (oItemMaster.SerialNumberTracking == false)
                {
                    if (context.ToolAssetQuantityDetails.Where(x => x.ToolGUID == objDTO.ToolGUID).Any())
                    {
                        oItemMaster.Quantity = (oItemMaster.Quantity + objDTO.Quantity) ?? 0;
                    }
                    else
                    {
                        oItemMaster.Quantity = objDTO.Quantity ?? 0;
                    }
                }
                else
                {
                    oItemMaster.Quantity = 1;
                }
                List<ToolAssetQuantityDetail> lstToolAssetQuantityDetail = context.ToolAssetQuantityDetails.Where(t => t.ToolGUID == oItemMaster.GUID && t.RoomID == oItemMaster.Room).ToList();

                if (lstToolAssetQuantityDetail != null && lstToolAssetQuantityDetail.Any() && lstToolAssetQuantityDetail.Count() > 0)
                {
                    oItemMaster.AvailableToolQty = lstToolAssetQuantityDetail.Sum(t => t.Quantity);
                }
                else
                {
                    oItemMaster.AvailableToolQty = 0;
                }

                oItemMaster.WhatWhereAction = "Receive";
                oItemMaster.Updated = DateTimeUtility.DateTimeNow;
                oItemMaster.LastUpdatedBy = objDTO.LastUpdatedBy;
                oItemMaster.Cost = objDTO.Cost;
                objItem.EditFromOrder(oItemMaster);
            }

            else
            {

                List<ToolAssetQuantityDetail> lstToolAssetQuantityDetail = context.ToolAssetQuantityDetails.Where(t => t.ToolGUID == oItemMaster.GUID && t.RoomID == oItemMaster.Room).ToList();

                if (lstToolAssetQuantityDetail != null && lstToolAssetQuantityDetail.Any() && lstToolAssetQuantityDetail.Count() > 0)
                {
                    oItemMaster.AvailableToolQty = lstToolAssetQuantityDetail.Sum(t => t.Quantity);
                }
                else
                {
                    oItemMaster.AvailableToolQty = 0;
                }

                oItemMaster.Quantity = context.ToolAssetQuantityDetails.Where(x => x.ToolGUID == objDTO.ToolGUID).Sum(x => (x.Quantity ?? 0));


                oItemMaster.WhatWhereAction = "Receive";
                oItemMaster.Updated = DateTimeUtility.DateTimeNow;
                oItemMaster.LastUpdatedBy = objDTO.LastUpdatedBy;
                oItemMaster.Cost = objDTO.Cost;
                objItem.EditFromOrder(oItemMaster);
                //ToolMasterDTO oItemMasterNew = objItem.GetRecordBySerialAndTool(oItemMaster.ToolName, objDTO.Room ?? 0, objDTO.CompanyID ?? 0, objDTO.SerialNumber);
                //if (oItemMasterNew == null)
                //{


                //    oItemMasterNew = new ToolMasterDTO();
                //    oItemMasterNew.GUID = Guid.NewGuid();
                //    oItemMasterNew.ToolName = oItemMaster.ToolName;
                //    oItemMasterNew.Serial = objDTO.SerialNumber;
                //    oItemMasterNew.Description = string.Empty;
                //    oItemMasterNew.ToolCategoryID = null;
                //    oItemMasterNew.Cost = objDTO.Cost;
                //    oItemMasterNew.IsCheckedOut = false;
                //    oItemMasterNew.IsGroupOfItems = 1;
                //    oItemMasterNew.Quantity = 1;
                //    oItemMasterNew.CheckedOutQTY = 0;
                //    oItemMasterNew.CheckedOutMQTY = 0;
                //    oItemMasterNew.Created = DateTimeUtility.DateTimeNow;
                //    oItemMasterNew.CreatedBy = objDTO.LastUpdatedBy;
                //    oItemMasterNew.Updated = DateTimeUtility.DateTimeNow;
                //    oItemMasterNew.LastUpdatedBy = objDTO.LastUpdatedBy;
                //    oItemMasterNew.Room = objDTO.Room;
                //    oItemMasterNew.IsArchived = false;
                //    oItemMasterNew.IsDeleted = false;
                //    oItemMasterNew.LocationID = 0;
                //    oItemMasterNew.CompanyID = objDTO.CompanyID;
                //    oItemMasterNew.UDF1 = null;
                //    oItemMasterNew.UDF2 = null;
                //    oItemMasterNew.UDF3 = null;
                //    oItemMasterNew.UDF4 = null;
                //    oItemMasterNew.UDF5 = null;
                //    oItemMasterNew.UDF6 = null;
                //    oItemMasterNew.UDF7 = null;
                //    oItemMasterNew.UDF8 = null;
                //    oItemMasterNew.UDF9 = null;
                //    oItemMasterNew.UDF10 = null;


                //    oItemMasterNew.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                //    oItemMasterNew.ReceivedOn = DateTimeUtility.DateTimeNow;
                //    oItemMasterNew.AddedFrom = "Web";
                //    oItemMasterNew.EditedFrom = "Web";


                //    oItemMasterNew.ToolImageExternalURL = string.Empty;
                //    oItemMasterNew.ImageType = "ExternalImage";
                //    oItemMasterNew.ImagePath = string.Empty;
                //    oItemMasterNew.Type = 1;
                //    oItemMasterNew.IsBuildBreak = false;

                //        oItemMaster.AvailableToolQty = 1;

                //    oItemMasterNew.ToolTypeTracking = "1";
                //    oItemMasterNew.SerialNumberTracking = false;
                //    oItemMasterNew.LotNumberTracking = false;
                //    oItemMasterNew.DateCodeTracking = false;
                //    objItem.Insert(oItemMasterNew);

                //    oItemMasterNew = objItem.GetRecord(0, objDTO.Room ?? 0, objDTO.CompanyID ?? 0, UsedToolGUId.GetValueOrDefault(Guid.Empty));

                //List<ToolAssetQuantityDetail> lstToolAssetQuantityDetail = context.ToolAssetQuantityDetails.Where(t => t.ToolGUID == oItemMaster.GUID && t.RoomID == oItemMaster.Room).ToList();

                //    if (lstToolAssetQuantityDetail != null && lstToolAssetQuantityDetail.Any() && lstToolAssetQuantityDetail.Count() > 0)
                //    {
                //        oItemMasterNew.AvailableToolQty = lstToolAssetQuantityDetail.Sum(t => t.Quantity);

                //    }
                //    else
                //    {
                //        oItemMasterNew.AvailableToolQty = 1;
                //    }
                //    oItemMasterNew.Updated = DateTimeUtility.DateTimeNow;
                //    oItemMasterNew.LastUpdatedBy = objDTO.LastUpdatedBy;
                //    oItemMasterNew.Cost = objDTO.Cost;
                //    oItemMasterNew.ReceivedOn = DateTimeUtility.DateTimeNow;
                //    objItem.EditFromOrder(oItemMasterNew);
                //}
            }

            if (oItemMaster != null && (oItemMaster.SerialNumberTracking) == false)
            {
                ToolDetailDAL objToolDetailDAL = new ToolDetailDAL(base.DataBaseName);
                ToolMoveInOutDetailDAL objToolMoveInOutDetailDAL = new ToolMoveInOutDetailDAL(base.DataBaseName);
                ToolCheckInOutHistoryDAL objToolCheckInOutHistoryDAL = new ToolCheckInOutHistoryDAL(base.DataBaseName);

                List<ToolDetailDTO> lstToolDetailDTO = objToolDetailDAL.GetAllRecordsByKitGUID(objDTO.ToolGUID.GetValueOrDefault(Guid.Empty), objDTO.Room ?? 0, objDTO.CompanyID ?? 0, false, false, false).ToList();
                if (lstToolDetailDTO != null && lstToolDetailDTO.Any() && lstToolDetailDTO.Count() > 0)
                {
                    foreach (ToolDetailDTO t in lstToolDetailDTO)
                    {
                        //ToolCheckInOutHistoryDTO objToolCheckInOutHistoryDTO = new ToolCheckInOutHistoryDTO();
                        //objToolCheckInOutHistoryDTO.AddedFrom = "Web";
                        //objToolCheckInOutHistoryDTO.CheckedOutMQTY = 0;
                        //objToolCheckInOutHistoryDTO.CheckedOutMQTYCurrent = 0;
                        //objToolCheckInOutHistoryDTO.CheckedOutQTY = (t.QuantityPerKit ?? 0) * (objDTO.Quantity ?? 0);
                        //objToolCheckInOutHistoryDTO.CheckedOutQTYCurrent = 0;
                        //objToolCheckInOutHistoryDTO.CheckInDate = null;
                        //objToolCheckInOutHistoryDTO.CheckOutDate = DateTimeUtility.DateTimeNow;
                        //objToolCheckInOutHistoryDTO.CompanyID = objDTO.CompanyID ?? 0;
                        //objToolCheckInOutHistoryDTO.Created = DateTimeUtility.DateTimeNow;
                        //objToolCheckInOutHistoryDTO.CreatedBy = objDTO.LastUpdatedBy ?? 0;
                        //objToolCheckInOutHistoryDTO.EditedFrom = "Web";
                        //objToolCheckInOutHistoryDTO.GUID = Guid.NewGuid();
                        //objToolCheckInOutHistoryDTO.ID = 0;
                        //objToolCheckInOutHistoryDTO.IsArchived = false;
                        //objToolCheckInOutHistoryDTO.IsDeleted = false;
                        //objToolCheckInOutHistoryDTO.LastUpdatedBy = objDTO.LastUpdatedBy ?? 0;
                        //objToolCheckInOutHistoryDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        //objToolCheckInOutHistoryDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        //objToolCheckInOutHistoryDTO.Room = objDTO.Room ?? 0;
                        //objToolCheckInOutHistoryDTO.ToolDetailGUID = t.GUID;
                        //objToolCheckInOutHistoryDTO.ToolGUID = t.GUID;
                        //objToolCheckInOutHistoryDTO.Updated = DateTimeUtility.DateTimeNow;
                        //objToolCheckInOutHistoryDAL.Insert(objToolCheckInOutHistoryDTO);


                        ToolMoveInOutDetailDTO objToolMoveInOutDetailDTO = new ToolMoveInOutDetailDTO();
                        objToolMoveInOutDetailDTO.AddedFrom = "Web";
                        objToolMoveInOutDetailDTO.CompanyID = objDTO.CompanyID ?? 0;
                        objToolMoveInOutDetailDTO.Created = DateTimeUtility.DateTimeNow;
                        objToolMoveInOutDetailDTO.CreatedBy = objDTO.LastUpdatedBy ?? 0;
                        objToolMoveInOutDetailDTO.EditedFrom = "Web";
                        objToolMoveInOutDetailDTO.GUID = Guid.NewGuid();
                        objToolMoveInOutDetailDTO.ID = 0;
                        objToolMoveInOutDetailDTO.IsArchived = false;
                        objToolMoveInOutDetailDTO.IsDeleted = false;
                        objToolMoveInOutDetailDTO.LastUpdatedBy = objDTO.LastUpdatedBy ?? 0;
                        objToolMoveInOutDetailDTO.MoveInOut = "In";
                        objToolMoveInOutDetailDTO.Quantity = (t.QuantityPerKit ?? 0) * (objDTO.Quantity ?? 0);
                        objToolMoveInOutDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolMoveInOutDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objToolMoveInOutDetailDTO.Room = objDTO.Room ?? 0;
                        objToolMoveInOutDetailDTO.ToolDetailGUID = t.GUID;
                        objToolMoveInOutDetailDTO.ToolItemGUID = t.ToolItemGUID;
                        objToolMoveInOutDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                        objToolMoveInOutDetailDTO.ReasonFromMove = "Order Kit Received";
                        objToolMoveInOutDetailDTO.WhatWhereAction = "ReceivedToolAssetOrderTransferDetailDAL>>UpdateToolMaster";
                        objToolMoveInOutDetailDAL.Insert(objToolMoveInOutDetailDTO);



                    }
                }
            }

        }

        private PreReceivOrderToolDetail GetPreReceiveDetail(ReceivedToolAssetOrderTransferDetailDTO objROTD, eTurnsEntities context)
        {
            PreReceivOrderToolDetail objPreReceiveOrderTool = null;
            double qty = (objROTD.Quantity.GetValueOrDefault(0));
            //if (!string.IsNullOrEmpty(objROTD.LotNumber))
            //{
            //    if (objROTD.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
            //    {
            //        objPreReceive = context.PreReceivOrderDetails.FirstOrDefault(x => ((x.Quantity ?? 0) == qty)
            //                    && x.LotNumber.Trim() == objROTD.LotNumber.Trim() && x.ExpirationDate == objROTD.ExpirationDate
            //                    && x.ItemGUID == objROTD.ItemGUID && x.OrderDetailGUID == objROTD.OrderDetailGUID);
            //    }
            //    else
            //    {
            //        objPreReceive = context.PreReceivOrderToolDetails.FirstOrDefault(x => ((x.Quantity ?? 0) == qty)
            //                    x.ToolGUID == objROTD.ToolGUID && x.ToolAssetOrderDetailGUID == objROTD.ToolAssetOrderDetailGUID);
            //    }
            //}
            //            else 
            if (!string.IsNullOrEmpty(objROTD.SerialNumber))
            {
                //if (objROTD.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                //{
                //    objPreReceive = context.PreReceivOrderDetails.FirstOrDefault(x => ((x.Quantity ?? 0) == qty)
                //                && x.SerialNumber.Trim() == x.SerialNumber.Trim() && x.ExpirationDate == objROTD.ExpirationDate
                //                && x.ItemGUID == objROTD.ItemGUID && x.OrderDetailGUID == objROTD.OrderDetailGUID);
                //}
                //else
                {
                    objPreReceiveOrderTool = context.PreReceivOrderToolDetails.FirstOrDefault(x => ((x.Quantity ?? 0) == qty)
                                && x.SerialNumber.Trim() == x.SerialNumber.Trim() && x.ToolGUID == objROTD.ToolGUID && x.ToolAssetOrderDetailGUID == objROTD.ToolAssetOrderDetailGUID);
                }
            }
            //else if (objROTD.ExpirationDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
            //{
            //    objPreReceive = context.PreReceivOrderDetails.FirstOrDefault(x => ((x.Quantity ?? 0) == qty)
            //                                   && x.ExpirationDate == objROTD.ExpirationDate
            //                                   && x.ItemGUID == objROTD.ItemGUID && x.OrderDetailGUID == objROTD.OrderDetailGUID);
            //}
            else
            {
                objPreReceiveOrderTool = context.PreReceivOrderToolDetails.FirstOrDefault(x => ((x.Quantity ?? 0) == qty)
                                              && x.ToolGUID == objROTD.ToolGUID
                                              && x.ToolAssetOrderDetailGUID == objROTD.ToolAssetOrderDetailGUID);
                //&& string.IsNullOrEmpty(x.SerialNumber) && string.IsNullOrEmpty(x.LotNumber) && !x.ExpirationDate.HasValue);
            }

            if (objPreReceiveOrderTool != null && objPreReceiveOrderTool.ID > 0)
            {
                objPreReceiveOrderTool.IsReceived = true;
                objPreReceiveOrderTool.Updated = DateTimeUtility.DateTimeNow;
            }

            return objPreReceiveOrderTool;


        }
        private void UpdateToolLocationDetail()
        {
            try
            {

            }
            catch (Exception)
            {

            }
        }
        private IEnumerable<ToolAssetQuantityDetail> GetToolAssetQuantity(ReceivedToolAssetOrderTransferDetailDTO objDTO, eTurnsEntities context)
        {
            var objilq = (from x in context.ToolAssetQuantityDetails
                          join tl in context.ToolLocationDetails on x.ToolBinID equals tl.ID
                          join b in context.LocationMasters on tl.LocationGuid equals b.GUID
                          where (tl.LocationGuid != null && tl.LocationGuid != Guid.Empty)
                          && x.ToolGUID == objDTO.ToolGUID
                          && (x.IsDeleted) == false
                          && (x.IsArchived) == false
                          && b.IsDeleted == false
                          && (b.IsArchived) == false
                          group x by new { x.ToolGUID, x.ToolBinID } into grp
                          select new ToolAssetQuantityDetailDTO
                          {
                              ToolBinID = grp.Key.ToolBinID ?? 0,
                              ToolGUID = grp.Key.ToolGUID,
                              CompanyID = objDTO.CompanyID ?? 0,
                              RoomID = objDTO.Room ?? 0,
                              Updated = DateTimeUtility.DateTimeNow,
                              Created = DateTimeUtility.DateTimeNow,
                              CreatedBy = objDTO.CreatedBy ?? 0,
                              UpdatedBy = objDTO.LastUpdatedBy ?? 0,
                              Quantity = grp.Sum(x => x.Quantity ?? 0),
                              WhatWhereAction = "ReceiveToolAssetOrderTransferDetail>>GetToolAssetQuantity",
                              Cost = objDTO.Cost,
                              GUID = Guid.NewGuid(),
                              AddedFrom = objDTO.AddedFrom,
                              EditedFrom = objDTO.EditedFrom,
                              ReceivedOn = DateTimeUtility.DateTimeNow,
                              ReceivedOnWeb = DateTimeUtility.DateTimeNow,

                          }).ToList();

            List<ToolAssetQuantityDetail> lstQty = new List<ToolAssetQuantityDetail>();
            foreach (var item in objilq)
            {
                ToolAssetQuantityDetail qty = new ToolAssetQuantityDetail()
                {
                    //   AddedFrom = item.AddedFrom,
                    ToolBinID = item.ToolBinID,
                    CompanyID = item.CompanyID,
                    Quantity = item.Quantity,
                    Created = item.Created,
                    CreatedBy = item.CreatedBy,
                    EditedFrom = item.EditedFrom,
                    AddedFrom = item.EditedFrom,
                    WhatWhereAction = item.WhatWhereAction,
                    Cost = item.Cost,
                    EditedOnAction = "Web",
                    AssetGUID = Guid.Empty,
                    ID = item.ID,
                    ToolGUID = item.ToolGUID,
                    Updated = item.Updated,
                    UpdatedBy = item.UpdatedBy,
                    //LotNumber = item.LotNumber,
                    ReceivedOn = DateTimeUtility.DateTimeNow,

                    ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                    RoomID = item.RoomID,
                };

                lstQty.Add(qty);
            }
            return lstQty;
        }
        private IEnumerable<ToolLocationDetail> GetItemLocationQty(ReceivedToolAssetOrderTransferDetailDTO objDTO, eTurnsEntities context)
        {
            var objilq = (from x in context.ToolLocationDetails
                          join b in context.LocationMasters on x.LocationGuid equals b.GUID
                          where (x.LocationGuid != null && x.LocationGuid != Guid.Empty)
                          && x.ToolGuid == objDTO.ToolGUID
                          && (x.IsDeleted ?? false) == false
                          && (x.IsArchieved ?? false) == false
                          && b.IsDeleted == false
                          && (b.IsArchived ?? false) == false
                          group x by new { x.ToolGuid, x.LocationGuid } into grp
                          select new ToolLocationDetailsDTO
                          {
                              LocationGUID = grp.Key.LocationGuid ?? Guid.Empty,
                              ToolGuid = grp.Key.ToolGuid,
                              CompanyID = objDTO.CompanyID,
                              RoomID = objDTO.Room,
                              LastUpdatedOn = DateTimeUtility.DateTimeNow,
                              Createdon = DateTimeUtility.DateTimeNow,
                              CreatedBy = objDTO.CreatedBy,
                              LastUpdatedBy = objDTO.LastUpdatedBy,
                              Quantity = grp.Sum(x => x.Quantity ?? 0),
                              WhatWhereAction = "ReceiveToolAssetOrderTransferDetail>>GetItemlocationQty",
                              Cost = objDTO.Cost,
                              //GUID = Guid.N ewGuid(),
                              AddedFrom = objDTO.AddedFrom,
                              EditedFrom = objDTO.EditedFrom,
                              ReceivedOn = DateTimeUtility.DateTimeNow,
                              ReceivedOnWeb = DateTimeUtility.DateTimeNow,

                          }).ToList();

            List<ToolLocationDetail> lstQty = new List<ToolLocationDetail>();
            foreach (var item in objilq)
            {
                ToolLocationDetail qty = new ToolLocationDetail()
                {
                    //   AddedFrom = item.AddedFrom,
                    LocationGuid = item.LocationGUID,
                    CompanyID = item.CompanyID,
                    Quantity = item.Quantity,
                    Createdon = item.Createdon,
                    CreatedBy = item.CreatedBy,
                    EditedFrom = item.EditedFrom,

                    WhatWhereAction = item.WhatWhereAction,
                    Cost = item.Cost,
                    LocationID = 0,
                    AssetGuid = Guid.Empty,
                    ID = item.ID,
                    ToolGuid = item.ToolGuid,
                    LastUpdatedOn = item.LastUpdatedOn,
                    LastUpdatedBy = item.LastUpdatedBy,
                    //LotNumber = item.LotNumber,
                    ReceivedOn = DateTimeUtility.DateTimeNow,

                    ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                    RoomID = item.RoomID,
                };

                lstQty.Add(qty);
            }
            return lstQty;
        }

        private ToolAssetQuantityDetail GetToolLocationDetailObject(ReceivedToolAssetOrderTransferDetailDTO objDTO)
        {
            ToolAssetQuantityDetail objILD = new ToolAssetQuantityDetail();
            objILD.ID = 0;
            objILD.Created = DateTimeUtility.DateTimeNow;
            objILD.Updated = DateTimeUtility.DateTimeNow;
            objILD.GUID = Guid.NewGuid();
            objILD.IsDeleted = false;
            objILD.IsArchived = false;
            objILD.EditedOnAction = "web";
            objILD.AddedFrom = objDTO.AddedFrom;
            objILD.EditedFrom = objDTO.AddedFrom;
            objILD.SerialNumber = objDTO.SerialNumber;
            //objILD.LotNumber = objDTO.LotNumber;
            objILD.ReceivedOn = DateTimeUtility.DateTimeNow;
            objILD.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

            objILD.ToolBinID = objDTO.ToolBinID;
            objILD.Quantity = objDTO.Quantity;
            objILD.SerialNumber = (!string.IsNullOrWhiteSpace(objDTO.SerialNumber)) ? objDTO.SerialNumber.Trim() : string.Empty;
            //objILD.ExpirationDate = objDTO.ExpirationDate;
            objILD.Cost = objDTO.Cost;
            objILD.ReceivedDate = objDTO.ReceivedDate;
            objILD.ToolGUID = objDTO.ToolGUID;
            objILD.ToolAssetOrderDetailGUID = objDTO.ToolAssetOrderDetailGUID;
            objILD.UDF1 = objDTO.UDF1;
            objILD.UDF2 = objDTO.UDF2;
            objILD.UDF3 = objDTO.UDF3;
            objILD.UDF4 = objDTO.UDF4;
            objILD.UDF5 = objDTO.UDF5;
            objILD.CreatedBy = objDTO.CreatedBy ?? 0;
            objILD.UpdatedBy = objDTO.LastUpdatedBy ?? 0;
            objILD.CompanyID = objDTO.CompanyID ?? 0;
            objILD.RoomID = objDTO.Room ?? 0;

            objILD.InitialQuantityWeb = (objDTO.Quantity ?? 0);
            objILD.Description = objDTO.Description;




            if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                objILD.AddedFrom = objDTO.AddedFrom;

            if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                objILD.EditedFrom = objDTO.EditedFrom;

            return objILD;
        }

        private ReceivedToolAssetOrderTransferDetail GetReceiveOrderTransferDetailObject(ReceivedToolAssetOrderTransferDetailDTO objDTO)
        {
            ReceivedToolAssetOrderTransferDetail objROTD = new ReceivedToolAssetOrderTransferDetail();

            objROTD.ID = 0;
            objROTD.Created = DateTimeUtility.DateTimeNow;
            objROTD.Updated = DateTimeUtility.DateTimeNow;
            objROTD.GUID = Guid.NewGuid();
            objROTD.IsDeleted = false;
            objROTD.IsArchived = false;
            objROTD.AddedFrom = "Unknown";
            objROTD.EditedFrom = "Unknown";
            objROTD.SerialNumber = objDTO.SerialNumber;
            objROTD.LotNumber = objDTO.LotNumber;
            objROTD.ReceivedOn = DateTimeUtility.DateTimeNow;
            objROTD.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

            objROTD.ToolBinID = objDTO.ToolBinID;
            objROTD.Quantity = objDTO.Quantity;

            objROTD.Cost = objDTO.Cost;
            objROTD.ReceivedDate = objDTO.ReceivedDate;
            objROTD.ToolGUID = objDTO.ToolGUID;
            objROTD.ToolAssetOrderDetailGUID = objDTO.ToolAssetOrderDetailGUID;
            objROTD.UDF1 = objDTO.UDF1;
            objROTD.UDF2 = objDTO.UDF2;
            objROTD.UDF3 = objDTO.UDF3;
            objROTD.UDF4 = objDTO.UDF4;
            objROTD.UDF5 = objDTO.UDF5;
            objROTD.CreatedBy = objDTO.CreatedBy;
            objROTD.LastUpdatedBy = objDTO.LastUpdatedBy;
            objROTD.CompanyID = objDTO.CompanyID;
            objROTD.Room = objDTO.Room;
            objROTD.IsEDISent = objDTO.IsEDISent;
            objROTD.LastEDIDate = objDTO.LastEDIDate;
            objROTD.PackSlipNumber = objDTO.PackSlipNumber;


            if (objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                objROTD.Received = objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");

            if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                objROTD.AddedFrom = objDTO.AddedFrom;

            if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                objROTD.EditedFrom = objDTO.EditedFrom;


            return objROTD;

        }

    }
}
