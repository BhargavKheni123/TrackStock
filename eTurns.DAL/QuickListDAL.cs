using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public class QuickListDAL : eTurnsBaseDAL
    {
        //private const string InitCacheKey = "Cached_QuickListMaster_";
        //private const string InitCacheKeyDetail = "Cached_QuickListDetail_";

        #region [Class Constructor]

        public QuickListDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public QuickListDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public QuickListMasterDTO GetQuickListMasterByName(string Name, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            QuickListMasterDTO oQuickList;
            List<long> supplierIds = new List<long>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Name", Name),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@IsDeleted", IsDeleted),
                                                   new SqlParameter("@IsArchived", IsArchived) };

                oQuickList = (from u in context.Database.SqlQuery<QuickListMasterDTO>("exec [GetQuickListMasterByName] @Name,@RoomID,@CompanyID,@IsDeleted,@IsArchived", params1)
                              select new QuickListMasterDTO
                              {
                                  ID = u.ID,
                                  Name = u.Name,
                                  Comment = u.Comment,
                                  CompanyID = u.CompanyID,
                                  IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                  IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                  //NoOfItems = u.NoOfItems,
                                  Type = u.Type,
                                  Created = u.Created,
                                  LastUpdated = u.LastUpdated,
                                  CreatedByName = u.CreatedByName,
                                  UpdatedByName = u.UpdatedByName,
                                  RoomName = u.RoomName,
                                  CreatedBy = u.CreatedBy,
                                  LastUpdatedBy = u.LastUpdatedBy,
                                  Room = u.Room,
                                  GUID = u.GUID,
                                  UDF1 = u.UDF1,
                                  UDF2 = u.UDF2,
                                  UDF3 = u.UDF3,
                                  UDF4 = u.UDF4,
                                  UDF5 = u.UDF5,
                                  ReceivedOn = u.ReceivedOn,
                                  ReceivedOnWeb = u.ReceivedOnWeb,
                                  AddedFrom = u.AddedFrom,
                                  EditedFrom = u.EditedFrom,
                                  AppendedBarcodeString = string.Empty,
                                  NoOfItems = GetQuickListItemsRecords(RoomID, CompanyID, u.GUID.ToString(), supplierIds).Count(),
                              }).FirstOrDefault();   //.AsParallel().ToList().FirstOrDefault();
            }

            return oQuickList;
        }

        public QuickListMaster GetQuickListMasterByNameLite(string Name, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.QuickListMasters.FirstOrDefault(t => t.Name == Name && t.IsDeleted != true && t.Room == RoomID && t.CompanyID == CompanyID);
            }
        }

        /// <summary>
        /// Get Paged Records from the Bin Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<QuickListMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string QuickListType = "")
        {
            return DB_GetQLMasterData(CompanyId, RoomID, IsDeleted, IsArchived, null, null, QuickListType);
            //return GetQuickListMasterCachedData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        }

        /// <summary>
        /// Get Paged Records from the Bin Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<QuickListMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string QuickListType, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            return DB_GetQLMasterRecordsByPaging(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyId, IsArchived, IsDeleted, QuickListType, RoomDateFormat, CurrentTimeZone);

            //if (String.IsNullOrEmpty(SearchTerm))
            //{
            //    //Get Cached-Media
            //    IEnumerable<QuickListMasterDTO> ObjCache = GetQuickListMasterCachedData(RoomID, CompanyId, IsArchived, IsDeleted);
            //    TotalCount = 0;
            //    if (ObjCache != null)
            //        TotalCount = ObjCache.Count();
            //    else
            //        ObjCache = new List<QuickListMasterDTO>().AsEnumerable<QuickListMasterDTO>();


            //    return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            //}
            //else if (SearchTerm.Contains("[###]"))
            //{
            //    //Get Cached-Media
            //    IEnumerable<QuickListMasterDTO> ObjCache = GetQuickListMasterCachedData(RoomID, CompanyId, IsArchived, IsDeleted);
            //    string[] stringSeparators = new string[] { "[###]" };
            //    string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

            //    ObjCache = ObjCache.Where(t =>
            //           ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
            //        && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
            //        && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
            //        && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
            //        && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
            //        && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
            //        && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
            //        && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
            //        && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
            //        );

            //    TotalCount = ObjCache.Count();
            //    return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            //}
            //else
            //{
            //    //Get Cached-Media
            //    IEnumerable<QuickListMasterDTO> ObjCache = GetQuickListMasterCachedData(RoomID, CompanyId, IsArchived, IsDeleted);
            //    TotalCount = ObjCache.Where
            //        (
            //            t => t.ID.ToString().Contains(SearchTerm) ||
            //            (t.Name ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.Comment ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.RoomName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.Created.GetValueOrDefault(DateTime.MinValue).ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.LastUpdated.GetValueOrDefault(DateTime.MinValue).ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (Enum.Parse(typeof(QuickListType), t.Type.ToString()).ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0

            //        ).Count();
            //    return ObjCache.Where
            //        (t => t.ID.ToString().Contains(SearchTerm) ||
            //            (t.Name ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.Comment ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.RoomName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.Created.GetValueOrDefault(DateTime.MinValue).ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.LastUpdated.GetValueOrDefault(DateTime.MinValue).ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (Enum.Parse(typeof(QuickListType), t.Type.ToString()).ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
            //            (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
            //        ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            //}
        }

        /// <summary>
        /// Get Particullar Record from the Bin Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public QuickListMasterDTO GetRecord(string GUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return DB_GetQLMasterRecord(GUID);
            //return GetQuickListMasterCachedData(RoomID, CompanyId, IsArchived, IsDeleted).SingleOrDefault(t => t.GUID == Guid.Parse(GUID));
        }

        /// <summary>
        /// Insert Record in the DataBase Bin Master
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(QuickListMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                QuickListMaster obj = new QuickListMaster();
                obj.ID = 0;
                obj.Name = objDTO.Name;
                obj.Comment = objDTO.Comment;
                obj.Type = objDTO.Type;

                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.GUID = Guid.NewGuid();

                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "QuickList";

                obj.WhatWhereAction = objDTO.WhatWhereAction;

                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.AddedFrom = objDTO.AddedFrom = "Web";
                obj.EditedFrom = objDTO.EditedFrom = "Web";

                context.QuickListMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                if (objDTO.ID > 0)
                {
                    //    if (objDTO.QuickListDetailList != null && objDTO.QuickListDetailList.Count > 0)
                    //    {
                    //        foreach (var item in objDTO.QuickListDetailList)
                    //        {
                    //            if (item.ID <= 0)
                    //            {
                    //                item.QuickListID = objDTO.ID;
                    //                item.QuickListGUID = obj.GUID;
                    //                QuickListItemInsert(item);
                    //            }
                    //        }
                    //    }

                    //Get Cached-Media
                    //IEnumerable<QuickListMasterDTO> ObjCache = CacheHelper<IEnumerable<QuickListMasterDTO>>.GetCacheItem(InitCacheKey + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                    //    List<QuickListMasterDTO> tempC = new List<QuickListMasterDTO>();
                    //    tempC.Add(objDTO);

                    //    IEnumerable<QuickListMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    //    CacheHelper<IEnumerable<QuickListMasterDTO>>.AppendToCacheItem(InitCacheKey + objDTO.CompanyID.ToString(), NewCache);
                    //}
                }

                return obj.ID;
            }
        }
        /// <summary>
        /// Insert Record in the DataBase Bin Master
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Guid InsertQuickList(QuickListMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                QuickListMaster obj = new QuickListMaster();
                obj.ID = 0;
                obj.Name = objDTO.Name;
                obj.Comment = objDTO.Comment;
                obj.Type = objDTO.Type;

                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.GUID = Guid.NewGuid();

                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "QuickList";

                obj.WhatWhereAction = objDTO.WhatWhereAction;

                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.AddedFrom = objDTO.AddedFrom = "Web";
                obj.EditedFrom = objDTO.EditedFrom = "Web";

                context.QuickListMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                if (objDTO.ID > 0)
                {
                    //    if (objDTO.QuickListDetailList != null && objDTO.QuickListDetailList.Count > 0)
                    //    {
                    //        foreach (var item in objDTO.QuickListDetailList)
                    //        {
                    //            if (item.ID <= 0)
                    //            {
                    //                item.QuickListID = objDTO.ID;
                    //                item.QuickListGUID = obj.GUID;
                    //                QuickListItemInsert(item);
                    //            }
                    //        }
                    //    }

                    //Get Cached-Media
                    //IEnumerable<QuickListMasterDTO> ObjCache = CacheHelper<IEnumerable<QuickListMasterDTO>>.GetCacheItem(InitCacheKey + objDTO.CompanyID.ToString());
                    //if (ObjCache != null)
                    //{
                    //    List<QuickListMasterDTO> tempC = new List<QuickListMasterDTO>();
                    //    tempC.Add(objDTO);

                    //    IEnumerable<QuickListMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    //    CacheHelper<IEnumerable<QuickListMasterDTO>>.AppendToCacheItem(InitCacheKey + objDTO.CompanyID.ToString(), NewCache);
                    //}
                }

                return obj.GUID;
            }
        }
        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(QuickListMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                QuickListMaster obj = new QuickListMaster();
                obj.ID = objDTO.ID;
                obj.Name = objDTO.Name;
                obj.Comment = objDTO.Comment;
                obj.Type = objDTO.Type;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.GUID = objDTO.GUID;
                obj.IsDeleted = objDTO.IsDeleted.GetValueOrDefault(false);
                obj.IsArchived = objDTO.IsArchived.GetValueOrDefault(false);

                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;

                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;

                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "QuickList";

                obj.WhatWhereAction = objDTO.WhatWhereAction;

                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;

                context.QuickListMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                //IEnumerable<QuickListMasterDTO> ObjCache = CacheHelper<IEnumerable<QuickListMasterDTO>>.GetCacheItem(InitCacheKey + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<QuickListMasterDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                //    ObjCache = objTemp.AsEnumerable();

                //    List<QuickListMasterDTO> tempC = new List<QuickListMasterDTO>();
                //    tempC.Add(objDTO);
                //    IEnumerable<QuickListMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //    CacheHelper<IEnumerable<QuickListMasterDTO>>.AppendToCacheItem(InitCacheKey + objDTO.CompanyID.ToString(), NewCache);
                //}

                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId, Int64 RoomID)
        {
            return DB_DeleteQLMasterRecords(IDs, userid);
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public List<QuickListDetailDTO> GetQuickListItemsRecords(Int64 RoomID, Int64 CompanyID, string QuickListGUID, List<long> SupplierIds)
        {
            return DB_GetQLLineItemsRecords(RoomID, CompanyID, false, false, null, null, QuickListGUID, SupplierIds);
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public List<QuickListDetailDTO> GetQuickListItemsRecords(Int64 RoomID, Int64 CompanyID, string QuickListGUID, bool IsDelete, bool IsArchived, List<long> SupplierIds)
        {
            return DB_GetQLLineItemsRecords(RoomID, CompanyID, IsDelete, IsArchived, null, null, QuickListGUID, SupplierIds);
        }

        /// <summary>
        /// Get Paged Records from the Bin Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public List<QuickListDetailDTO> GetQuickListItemsPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, string QuickListGUID, bool IsDeleted, bool IsArchived, List<long> SupplierIds)
        {
            if (QuickListGUID == string.Empty)
            {
                TotalCount = 0;
                return new List<QuickListDetailDTO>(); ;
            }

            return DB_GetQLLineItemsRecordsByPaging(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyId, QuickListGUID, IsDeleted, IsArchived, SupplierIds);
        }

        /// <summary>
        /// Insert Record in the DataBase Bin Master
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 QuickListItemInsert(QuickListDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                QuickListItem obj = new QuickListItem();
                obj.ID = 0;
                obj.ItemGUID = objDTO.ItemGUID;

                obj.Quantity = objDTO.Quantity;
                obj.QuickListGUID = objDTO.QuickListGUID;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;

                obj.Room = objDTO.Room;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.GUID = Guid.NewGuid();

                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.AddedFrom = "Web";
                obj.EditedFrom = "Web";
                obj.BinID = objDTO.BinID;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;

                context.QuickListItems.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    //IEnumerable<QuickListDetailDTO> ObjCache = CacheHelper<IEnumerable<QuickListDetailDTO>>.GetCacheItem(InitCacheKeyDetail + objDTO.CompanyID.ToString() + "_" + objDTO.QuickListGUID);
                    //if (ObjCache != null)
                    //{
                    //    List<QuickListDetailDTO> tempC = new List<QuickListDetailDTO>();
                    //    tempC.Add(objDTO);

                    //    IEnumerable<QuickListDetailDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    //    CacheHelper<IEnumerable<QuickListDetailDTO>>.AppendToCacheItem(InitCacheKeyDetail + objDTO.CompanyID.ToString() + "_" + objDTO.QuickListGUID, NewCache);
                    //}
                }

                return obj.ID;
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool QuickListItemsEdit(QuickListDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                QuickListItem obj = context.QuickListItems.Single(t => t.GUID == objDTO.GUID);
                obj.Quantity = objDTO.Quantity;
                obj.CompanyID = objDTO.CompanyID;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.BinID = objDTO.BinID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;

                //context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                //context.QuickListItems.Attach(obj);
                //context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                //context.QuickListItems.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                //IEnumerable<QuickListDetailDTO> ObjCache = CacheHelper<IEnumerable<QuickListDetailDTO>>.GetCacheItem(InitCacheKeyDetail + objDTO.CompanyID.ToString() + "_" + obj.QuickListGUID);
                //if (ObjCache != null)
                //{
                //    ObjCache = null;
                //    //List<QuickListDetailDTO> objTemp = ObjCache.ToList();
                //    //objTemp.RemoveAll(i => i.ID == obj.ID);
                //    //ObjCache = objTemp.AsEnumerable();

                //    //List<QuickListDetailDTO> tempC = new List<QuickListDetailDTO>();
                //    //tempC.Add(objDTO);
                //    //IEnumerable<QuickListDetailDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //    CacheHelper<IEnumerable<QuickListDetailDTO>>.AppendToCacheItem(InitCacheKeyDetail + objDTO.CompanyID.ToString() + "_" + obj.QuickListGUID, new List<QuickListDetailDTO>());
                //}

                return true;
            }
        }

        /// <summary>
        /// Delete Particullar Record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool QuickListItemDelete(string GUID, Int64 userid)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Guid temp = Guid.Parse(GUID);
                QuickListItem obj = context.QuickListItems.Single(t => t.GUID == temp);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.EditedFrom = "Web";
                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.QuickListItems.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                //IEnumerable<QuickListDetailDTO> ObjCache = CacheHelper<IEnumerable<QuickListDetailDTO>>.GetCacheItem(InitCacheKeyDetail + obj.CompanyID.ToString() + "_" + obj.QuickListGUID);
                //if (ObjCache != null)
                //{
                //    List<QuickListDetailDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => i.GUID == Guid.Parse(GUID));
                //    ObjCache = objTemp.AsEnumerable();
                //    CacheHelper<IEnumerable<QuickListDetailDTO>>.AppendToCacheItem(InitCacheKeyDetail + obj.CompanyID.ToString() + "_" + obj.QuickListGUID, ObjCache);
                //}


                return true;
            }
        }

        /// <summary>
        /// Delete Quicklist detail Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteQuickListItemsRecords(string IDs, Int64 userid, Int64 CompanyId)
        {
            return DB_DeleteQLLineITemRecords(IDs, userid);

            //using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    foreach (var item in IDs.Split(','))
            //    {
            //        if (!string.IsNullOrEmpty(item.Trim()))
            //        {
            //            QuickListItemDelete(item, userid);
            //        }
            //    }

            //    return true;
            //}
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public List<QuickListDetailDTO> GetQuickListLineItemHistory(string QuickListMasterHistoryGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@QuickListMasterHistoryGUID", QuickListMasterHistoryGUID) };

                return (from u in context.Database.SqlQuery<QuickListDetailDTO>("exec [GetQuickListLineItemHistory] @QuickListMasterHistoryGUID", params1)
                        select new QuickListDetailDTO
                        {
                            ID = u.ID,

                            Quantity = u.Quantity,
                            CompanyID = u.CompanyID,
                            IsArchived = u.IsArchived,
                            IsDeleted = u.IsDeleted,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            GUID = u.GUID,
                            ItemGUID = u.ItemGUID,
                            QuickListGUID = u.QuickListGUID,
                            CreatedByName = u.CreatedByName,
                            RoomName = u.RoomName,
                            UpdatedByName = u.UpdatedByName,
                            Action = u.Action,
                            HistoryID = u.HistoryID,
                            IsHistory = true,
                            QuickListHistoryID = u.QuickListHistoryID,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            ItemDetail = new ItemMasterDAL(base.DataBaseName).GetItemWithMasterTableJoins(null, u.ItemGUID, u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)),
                            ConsignedQuantity = u.ConsignedQuantity
                        }).AsParallel().ToList();
            }

        }

        /// <summary>
        /// Get Particullar Record from the OrderMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public QuickListMasterDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", id) };

                return (from u in context.Database.SqlQuery<QuickListMasterDTO>("exec [GetQuickListMasterHistory] @ID", params1)
                        select new QuickListMasterDTO
                        {
                            ID = u.ID,
                            Name = u.Name,
                            Comment = u.Comment,
                            CompanyID = u.CompanyID,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            HistoryID = u.HistoryID,
                            Action = u.Action,
                            IsHistory = true,
                            Type = u.Type,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            GUID = u.GUID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            //QuickListDetailList = GetQuickListLineItemHistory(u.HistoryID)
                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Get Paged Records from the Bin Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public List<QuickListDetailDTO> GetQuickListItemsPagedRecordsOfHistory(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, string QuickListHistoryGUID)
        {

            if (string.IsNullOrEmpty(QuickListHistoryGUID))
            {
                TotalCount = 0;
                return new List<QuickListDetailDTO>(); ;
            }
            IEnumerable<QuickListDetailDTO> ObjCache = GetQuickListLineItemHistory(QuickListHistoryGUID);
            if (String.IsNullOrEmpty(SearchTerm))
            {
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows).ToList();
            }
            else
            {
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                            (t.Action ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Quantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.OnHandQuantity.GetValueOrDefault(0).ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.OnTransferQuantity.GetValueOrDefault(0).ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.SupplierName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.CategoryName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0

                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.Action ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Quantity.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.OnHandQuantity.GetValueOrDefault(0).ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.OnTransferQuantity.GetValueOrDefault(0).ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.SupplierName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDetail.CategoryName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows).ToList();
            }
        }

        #region QL Master Without Cache

        /// <summary>
        /// DB_GetQuickListMasterData
        /// </summary>
        /// <param name="CompanyID"></param>
        /// <param name="RoomID"></param>
        /// <param name="IsDeleted"></param>
        /// <param name="IsArchived"></param>
        /// <param name="ID"></param>
        /// <param name="GuID"></param>
        /// <returns></returns>
        public IEnumerable<QuickListMasterDTO> DB_GetQLMasterData(Int64? CompanyID, Int64? RoomID, bool? IsDeleted, bool? IsArchived, Int64? ID, Guid? GuID, string QuickListType)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC QL_GetQuickListData ";

                if (CompanyID.HasValue)
                    strCommand += CompanyID.Value.ToString();
                else
                    strCommand += "null";
                if (RoomID.HasValue)
                    strCommand += ", " + RoomID.Value.ToString();
                else
                    strCommand += ", " + "null";

                if (IsDeleted.HasValue)
                    strCommand += ", " + (IsDeleted.Value ? "1" : "0");
                else
                    strCommand += ", " + "null";

                if (IsArchived.HasValue)
                    strCommand += ", " + (IsArchived.Value ? "1" : "0");
                else
                    strCommand += ", " + "null";

                if (ID.HasValue)
                    strCommand += ", " + ID.Value.ToString();
                else
                    strCommand += ", " + "null";

                if (GuID.HasValue)
                    strCommand += ", '" + GuID.Value.ToString() + "'";
                else
                    strCommand += ", " + "null";

                if (!string.IsNullOrEmpty(QuickListType) && QuickListType.Trim().Length > 0)
                    strCommand += ", '" + QuickListType + "'";
                else
                    strCommand += ", " + "null";

                IEnumerable<QuickListMasterDTO> obj = (from u in context.Database.SqlQuery<QuickListMasterDTO>(strCommand)
                                                       select new QuickListMasterDTO
                                                       {
                                                           ID = u.ID,
                                                           Name = u.Name,
                                                           Comment = u.Comment,
                                                           CompanyID = u.CompanyID,
                                                           IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                           IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                           NoOfItems = u.NoOfItems,
                                                           Type = u.Type,
                                                           Created = u.Created,
                                                           LastUpdated = u.LastUpdated,
                                                           CreatedByName = u.CreatedByName,
                                                           UpdatedByName = u.UpdatedByName,
                                                           RoomName = u.RoomName,
                                                           CreatedBy = u.CreatedBy,
                                                           LastUpdatedBy = u.LastUpdatedBy,
                                                           Room = u.Room,
                                                           GUID = u.GUID,
                                                           UDF1 = u.UDF1,
                                                           UDF2 = u.UDF2,
                                                           UDF3 = u.UDF3,
                                                           UDF4 = u.UDF4,
                                                           UDF5 = u.UDF5,
                                                           ReceivedOn = u.ReceivedOn,
                                                           ReceivedOnWeb = u.ReceivedOnWeb,
                                                           AddedFrom = u.AddedFrom,
                                                           EditedFrom = u.EditedFrom,
                                                           AppendedBarcodeString = u.AppendedBarcodeString,
                                                       }).AsParallel().ToList();
                return obj;
            }
        }

        /// <summary>
        /// Get Paged Records from the Bin Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<QuickListMasterDTO> DB_GetQLMasterRecordsByPaging(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string QuickListType, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            List<QuickListMasterDTO> lstQuickList = new List<QuickListMasterDTO>();
            TotalCount = 0;
            DataSet dsQuickList = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            //string strQuer = "";
            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
            {

                string Creaters = null;
                string Updators = null;
                string CreatedDateFrom = null;
                string CreatedDateTo = null;
                string UpdatedDateFrom = null;
                string UpdatedDateTo = null;
                string UDF1 = null;
                string UDF2 = null;
                string UDF3 = null;
                string UDF4 = null;
                string UDF5 = null;

                //Get Cached-Media

                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('~');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {

                    //string[] arrCreators = FieldsPara[0].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //foreach (var item in arrCreators)
                    //{
                    //    Creaters += "''" + item + "'',";
                    //}
                    Creaters = FieldsPara[0].TrimEnd(',');


                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    //Updators = FieldsPara[1].TrimEnd(',');
                    //string[] arrCreators = FieldsPara[1].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //foreach (var item in arrCreators)
                    //{
                    //    Updators += "''" + item + "'',";
                    //}
                    Updators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {

                    //  CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //  CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[2].Split(',')[0]), RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[2].Split(',')[1]), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    //UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    //UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[3].Split(',')[0]), RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(FieldsPara[3].Split(',')[1]), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    //UDF1 = Convert.ToString(FieldsPara[4]).TrimEnd();
                    string[] arrstr = FieldsPara[4].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in arrstr)
                    {
                        UDF1 = UDF1 + item + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    //UDF2 = Convert.ToString(FieldsPara[5]).TrimEnd();
                    string[] arrstr = FieldsPara[5].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in arrstr)
                    {
                        UDF2 = UDF2 + item + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    //UDF3 = Convert.ToString(FieldsPara[6]).TrimEnd();
                    string[] arrstr = FieldsPara[6].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in arrstr)
                    {
                        UDF3 = UDF3 + item + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    //UDF4 = Convert.ToString(FieldsPara[7]).TrimEnd();
                    string[] arrstr = FieldsPara[7].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in arrstr)
                    {
                        UDF4 = UDF4 + item + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    //UDF5 = Convert.ToString(FieldsPara[7]).TrimEnd();
                    string[] arrstr = FieldsPara[8].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in arrstr)
                    {
                        UDF5 = UDF5 + item + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);

                }
                dsQuickList = SqlHelper.ExecuteDataset(EturnsConnection, "QL_GetQuickListPagedRecord", StartRowIndex, MaxRows, SearchTerm,
                                   sortColumnName, Creaters, Updators, CreatedDateFrom, CreatedDateTo
                                   , UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3
                                   , UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, QuickListType);

                //strQuer = @"EXEC [QL_GetQuickListPagedRecord] " + StartRowIndex + "," + MaxRows + ",'" + SearchTerm + "','"
                //                   + sortColumnName + "','" + Creaters + "','" + Updators + "','" + CreatedDateFrom + "','" + CreatedDateTo
                //                   + "','" + UpdatedDateFrom + "','" + UpdatedDateTo + "','" + UDF1 + "','" + UDF2 + "','" + UDF3
                //                   + "','" + UDF4 + "','" + UDF5 + "'," + (IsDeleted ? "1" : "0") + "," + (IsArchived ? "1" : "0") + "," + RoomID + "," + CompanyId;


            }
            else
            {
                dsQuickList = SqlHelper.ExecuteDataset(EturnsConnection, "QL_GetQuickListPagedRecord", StartRowIndex, MaxRows, SearchTerm,
                                 sortColumnName, null, null, null, null
                                 , null, null, null, null, null
                                 , null, null, IsDeleted, IsArchived, RoomID, CompanyId, QuickListType);
                //strQuer = @"EXEC [QL_GetQuickListPagedRecord] " + StartRowIndex + "," + MaxRows + ",'" + SearchTerm + "','" + sortColumnName + "','','','','','','','','','','',''," + (IsDeleted ? "1" : "0") + "," + (IsArchived ? "1" : "0") + "," + RoomID + "," + CompanyId;
            }

            //if (!string.IsNullOrEmpty(QuickListType) && QuickListType.Trim().Length > 0)
            //{
            //    strQuer += ",'" + QuickListType + "'";
            //}
            if (dsQuickList != null && dsQuickList.Tables.Count > 0)
            {
                DataTable dtQuickList = dsQuickList.Tables[0];
                if (dtQuickList.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtQuickList.Rows[0]["TotalRecords"]);
                    lstQuickList = dtQuickList.AsEnumerable()
                    .Select(row => new QuickListMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        Name = row.Field<string>("Name"),
                        Comment = row.Field<string>("Comment"),
                        CompanyID = row.Field<long?>("CompanyID"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        NoOfItems = row.Field<Int32>("NoOfItems"),
                        Type = row.Field<Int32>("Type"),
                        Created = row.Field<DateTime?>("Created"),
                        LastUpdated = row.Field<DateTime?>("LastUpdated"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        Room = row.Field<long?>("Room"),
                        GUID = row.Field<Guid>("GUID"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        AppendedBarcodeString = row.Field<string>("AppendedBarcodeString"),
                        ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                        AddedFrom = row.Field<string>("AddedFrom"),
                        EditedFrom = row.Field<string>("EditedFrom")
                    }).ToList();
                }
            }
            return lstQuickList;

            //IEnumerable<QuickListMasterDTO> obj = (from u in context.Database.SqlQuery<QuickListMasterDTO>(strQuer)
            //                                           select new QuickListMasterDTO
            //                                           {
            //                                               ID = u.ID,
            //                                               Name = u.Name,
            //                                               Comment = u.Comment,
            //                                               CompanyID = u.CompanyID,
            //                                               IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
            //                                               IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
            //                                               NoOfItems = u.NoOfItems,
            //                                               Type = u.Type,
            //                                               Created = u.Created,
            //                                               LastUpdated = u.LastUpdated,
            //                                               CreatedByName = u.CreatedByName,
            //                                               UpdatedByName = u.UpdatedByName,
            //                                               RoomName = u.RoomName,
            //                                               CreatedBy = u.CreatedBy,
            //                                               LastUpdatedBy = u.LastUpdatedBy,
            //                                               Room = u.Room,
            //                                               GUID = u.GUID,
            //                                               UDF1 = u.UDF1,
            //                                               UDF2 = u.UDF2,
            //                                               UDF3 = u.UDF3,
            //                                               UDF4 = u.UDF4,
            //                                               UDF5 = u.UDF5,
            //                                               AppendedBarcodeString = u.AppendedBarcodeString,
            //                                               TotalRecords = u.TotalRecords,
            //                                               ReceivedOn = u.ReceivedOn,
            //                                               ReceivedOnWeb = u.ReceivedOnWeb,
            //                                               AddedFrom = u.AddedFrom,
            //                                               EditedFrom = u.EditedFrom,                                         
            //                                           }).AsParallel().ToList();

            //    TotalCount = 0;
            //    if (obj != null && obj.Count() > 0)
            //    {
            //        TotalCount = obj.ElementAt(0).TotalRecords;
            //    }
            //    return obj;
            //}

        }

        /// <summary>
        /// Get Particullar Record from the Bin Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public QuickListMasterDTO DB_GetQLMasterRecord(string GUID)
        {
            IEnumerable<QuickListMasterDTO> objDB = DB_GetQLMasterData(null, null, null, null, null, Guid.Parse(GUID), null);
            if (objDB != null && objDB.Count() > 0)
                return objDB.ElementAt(0);

            return null;
        }

        /// <summary>
        /// Get Particullar Record from the Bin Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public QuickListMasterDTO DB_GetQLMasterRecord(Int64 ID)
        {
            IEnumerable<QuickListMasterDTO> objDB = DB_GetQLMasterData(null, null, null, null, ID, null, null);
            if (objDB != null && objDB.Count() > 0)
                return objDB.ElementAt(0);

            return null;
        }

        /// <summary>
        /// Get Paged Records from the Bin Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<QuickListMasterDTO> DB_GetAllQLMasterRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return DB_GetQLMasterData(CompanyId, RoomID, IsDeleted, IsArchived, null, null, "");
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DB_DeleteQLMasterRecords(string IDs, Int64 userid)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string strIDs = "";
                string[] strGUIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in strGUIDs)
                {
                    strIDs += item + ",";
                }

                strIDs = strIDs.TrimEnd(',');

                strQuery += "Exec QL_DeleteQuickList " + userid.ToString() + ", '" + strIDs + "'";
                if (userid > 0 && !string.IsNullOrWhiteSpace(strQuery))
                {
                    context.Database.ExecuteSqlCommand(strQuery);
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// GetCachedData
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <returns></returns>
        public IEnumerable<CommonDTO> DB_GetQuickListNarrowSearchData(Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC  [QL_GetQuiclListMasterNarrowSearchData] ";
                strCommand += CompanyID.ToString();
                strCommand += ", " + RoomID.ToString();
                strCommand += ", " + (IsDeleted ? "1" : "0");
                strCommand += ", " + (IsArchived ? "1" : "0");
                IEnumerable<CommonDTO> obj = (from u in context.Database.SqlQuery<CommonDTO>(strCommand)
                                              select new CommonDTO
                                              {
                                                  ID = u.ID,
                                                  ControlID = "",
                                                  Count = u.Count,
                                                  PageName = u.PageName,
                                                  Text = u.Text
                                              }).AsParallel().ToList();
                return obj;
            }
        }
        public List<QuickListLineItemDetailDTO> GetAllQuicklistWiseLineItem(Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, string ids,List<long> UserSupplierIds)
        {
            List<QuickListLineItemDetailDTO> obj = new List<QuickListLineItemDetailDTO>();
            string[] arrids = ids.Split(',');
            Guid[] arrguids = new Guid[] { };
            List<Guid> lstids = new List<Guid>();
            foreach (string guiditem in arrids)
            {
                Guid temp = Guid.Empty;
                if (Guid.TryParse(guiditem, out temp))
                {
                    lstids.Add(temp);
                }
            }
            arrguids = lstids.ToArray();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {


                obj = (from qlm in context.QuickListMasters
                       join qli in context.QuickListItems on new { qlmguid = qlm.GUID, isdel = false, isarc = false } equals new { qlmguid = qli.QuickListGUID ?? Guid.Empty, isdel = qli.IsDeleted ?? false, isarc = qli.IsArchived ?? false } into qlm_qli_join
                       from qlm_qli in qlm_qli_join.DefaultIfEmpty()
                       join im in context.ItemMasters on qlm_qli.ItemGUID equals im.GUID into qli_im_join
                       from qli_im in qli_im_join.DefaultIfEmpty()
                       join uum in context.UserMasters on qlm_qli.LastUpdatedBy equals uum.ID into qli_uum_join
                       from qli_uum in qli_uum_join.DefaultIfEmpty()
                       join cum in context.UserMasters on qlm_qli.CreatedBy equals cum.ID into qli_cum_join
                       from qli_cum in qli_cum_join.DefaultIfEmpty()
                       join bm in context.BinMasters on qlm_qli.BinID equals bm.ID into qli_bm_join
                       from qli_bm in qli_bm_join.DefaultIfEmpty()
                       where (ids == null || ids == "" || arrguids.Contains(qlm.GUID))
                       && (qlm.Room ?? 0) == RoomID && (qlm.CompanyID ?? 0) == CompanyID && (qlm.IsDeleted) == IsDeleted && (qlm.IsArchived ?? false) == IsArchived
                       && (UserSupplierIds.Count > 0 ? UserSupplierIds.Contains(qli_im.SupplierID ?? 0) : 1 == 1)
                       select new QuickListLineItemDetailDTO
                       {
                           ID = qlm.ID,
                           Name = qlm.Name != null ? (qlm.Name) : string.Empty,
                           Comment = qlm.Comment != null ? (qlm.Comment) : string.Empty,
                           ItemNumber = qli_im.ItemNumber != null ? (qli_im.ItemNumber) : string.Empty,
                           Quantity = qlm_qli.Quantity ?? 0,
                           ConsignedQuantity = qlm_qli.ConsignedQuantity ?? 0,
                           ItemGUID = qlm_qli.ItemGUID,
                           QuickListGUID = qlm.GUID,
                           GUID = qlm_qli.GUID,
                           UDF1 = qlm_qli.UDF1,
                           UDF2 = qlm_qli.UDF2,
                           UDF3 = qlm_qli.UDF3,
                           UDF4 = qlm_qli.UDF4,
                           UDF5 = qlm_qli.UDF5,
                           IsDeleted = qlm.IsDeleted,
                           Type = qlm.Type,
                           NoOfItems = 0,
                           //RoomName=Convert.ToString(HttpContext.Current.Session["RoomName"]),
                           AddedFrom = qlm.AddedFrom,
                           EditedFrom = qlm.EditedFrom,
                           ReceivedOn = qlm.ReceivedOn,
                           ReceivedOnWeb = qlm.ReceivedOnWeb,
                           CreatedBy = qlm.CreatedBy,
                           LastUpdatedBy = qlm.LastUpdatedBy,
                           Created = qlm.Created,
                           LastUpdated = qlm.LastUpdated,
                           IsArchived = qlm.IsArchived,
                           UpdatedByName = qli_uum.UserName,
                           CreatedByName = qli_uum.UserName,
                           BinNumber = qli_bm.BinNumber
                       }).ToList();
            }

            return obj;
        }
        #endregion

        #region QL Detail Without Cache

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public List<QuickListDetailDTO> DB_GetQLLineItemsRecords(Int64? RoomID, Int64? CompanyID, bool? IsDeleted, bool? IsArchived, Int64? ID, Guid? GuID, string QuickListGUID, List<long> SupplierIds)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC QL_GetQLLineItemData ";

                if (CompanyID.HasValue)
                    strCommand += CompanyID.Value.ToString();
                else
                    strCommand += "null";

                if (RoomID.HasValue)
                    strCommand += ", " + RoomID.Value.ToString();
                else
                    strCommand += ", " + "null";

                if (IsDeleted.HasValue)
                    strCommand += ", " + (IsDeleted.Value ? "1" : "0");
                else
                    strCommand += ", " + "null";

                if (IsArchived.HasValue)
                    strCommand += ", " + (IsArchived.Value ? "1" : "0");
                else
                    strCommand += ", " + "null";

                if (ID.HasValue)
                    strCommand += ", " + ID.Value.ToString();
                else
                    strCommand += ", " + "null";

                if (GuID.HasValue)
                    strCommand += ", '" + GuID.Value.ToString() + "'";
                else
                    strCommand += ", " + "null";

                if (!string.IsNullOrEmpty(QuickListGUID))
                    strCommand += ", '" + QuickListGUID + "'";
                else
                    strCommand += ", " + "null";

                if (SupplierIds != null && SupplierIds.Any())
                    strCommand += ", '" + string.Join(",", SupplierIds) + "'";
                else
                    strCommand += ", " + "null";


                List<QuickListDetailDTO> obj = (from u in context.Database.SqlQuery<QuickListDetailDTO>(strCommand)
                                                select new QuickListDetailDTO
                                                {
                                                    ID = u.ID,
                                                    Quantity = u.Quantity,
                                                    QuickListGUID = u.QuickListGUID,
                                                    CompanyID = u.CompanyID,
                                                    IsArchived = u.IsArchived,
                                                    IsDeleted = u.IsDeleted,
                                                    Created = u.Created,
                                                    LastUpdated = u.LastUpdated,
                                                    CreatedBy = u.CreatedBy,
                                                    LastUpdatedBy = u.LastUpdatedBy,
                                                    Room = u.Room,
                                                    GUID = u.GUID,
                                                    ItemGUID = u.ItemGUID,
                                                    CreatedByName = u.CreatedByName,
                                                    RoomName = u.RoomName,
                                                    UpdatedByName = u.UpdatedByName,
                                                    Action = string.Empty,
                                                    AverageCost = u.AverageCost,
                                                    CategoryName = u.CategoryName,
                                                    Cost = u.Cost,
                                                    Description = u.Description,
                                                    HistoryID = 0,
                                                    IsHistory = false,
                                                    ItemID = u.ItemID,
                                                    ItemNumber = u.ItemNumber,
                                                    LongDescription = u.LongDescription,
                                                    ManufacturerName = u.ManufacturerName,
                                                    ManufacturerNumber = u.ManufacturerNumber,
                                                    OnHandQuantity = u.OnHandQuantity,
                                                    OnOrderQuantity = u.OnOrderQuantity,
                                                    OnTransferQuantity = u.OnTransferQuantity,
                                                    QuickListHistoryID = 0,
                                                    SupplierName = u.SupplierName,
                                                    SupplierPartNo = u.SupplierPartNo,
                                                    DateCodeTracking = u.DateCodeTracking,
                                                    ItemType = u.ItemType,
                                                    LotNumberTracking = u.LotNumberTracking,
                                                    Markup = u.Markup,
                                                    Unit = u.Unit,
                                                    PackingQuantity = u.PackingQuantity,
                                                    SellPrice = u.SellPrice,
                                                    SerialNumberTracking = u.SerialNumberTracking,
                                                    TotalRecords = 0,
                                                    DefaultPullQuantity = u.DefaultPullQuantity,
                                                    DefaultReorderQuantity = u.DefaultReorderQuantity,
                                                    DefaultLocation = u.DefaultLocation,
                                                    DefaultLocationName = u.DefaultLocationName,
                                                    Consignment = u.Consignment,
                                                    ReceivedOn = u.ReceivedOn,
                                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                                    AddedFrom = u.AddedFrom,
                                                    EditedFrom = u.EditedFrom,
                                                    BinID = u.BinID,
                                                    BinName = u.BinName,
                                                    ConsignedQuantity = u.ConsignedQuantity,
                                                    QuickListType = u.QuickListType,
                                                    PullQtyScanOverride = u.PullQtyScanOverride
                                                }).ToList();
                return obj;
            }

        }

        /// <summary>
        /// Get Paged Records from the Bin Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public List<QuickListDetailDTO> DB_GetQLLineItemsRecordsByPaging(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, string QuickListGUID, bool IsDeleted, bool IsArchived, List<long> SuplierIds)
        {

            if (QuickListGUID == string.Empty)
            {
                TotalCount = 0;
                return new List<QuickListDetailDTO>();
            }
            Guid guidQuickListGUID = Guid.Empty;
            if(!string.IsNullOrWhiteSpace(QuickListGUID))
            {
                guidQuickListGUID = Guid.Parse(QuickListGUID);
            }

            List<QuickListDetailDTO> lstQuickListDetail = new List<QuickListDetailDTO>();
            TotalCount = 0;
            DataSet dsQuickListDetail = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string strSupplierIds = string.Empty;
            if (SuplierIds != null && SuplierIds.Any())
            {
                strSupplierIds = string.Join(",", SuplierIds);
            }

            dsQuickListDetail = SqlHelper.ExecuteDataset(EturnsConnection, "QL_GetQLLineItemPagedRecord", StartRowIndex, MaxRows, SearchTerm, sortColumnName, guidQuickListGUID, IsDeleted, IsArchived, RoomID, CompanyId, strSupplierIds);
            if (dsQuickListDetail != null && dsQuickListDetail.Tables.Count > 0)
            {
                DataTable dtQuickListDetail = dsQuickListDetail.Tables[0];
                if (dtQuickListDetail.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtQuickListDetail.Rows[0]["TotalRecords"]);
                    lstQuickListDetail = dtQuickListDetail.AsEnumerable()
                    .Select(row => new QuickListDetailDTO
                    {
                        ID = row.Field<long>("ID"),
                        Quantity = row.Field<double?>("Quantity"),
                        QuickListGUID = row.Field<Guid>("QuickListGUID"),
                        CompanyID = row.Field<long?>("CompanyID"),
                        IsArchived = row.Field<bool>("IsArchived"),
                        IsDeleted = row.Field<bool>("IsDeleted"),
                        Created = row.Field<DateTime?>("Created"),
                        LastUpdated = row.Field<DateTime?>("LastUpdated"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        Room = row.Field<long?>("Room"),
                        GUID = row.Field<Guid>("GUID"),
                        ItemGUID = row.Field<Guid?>("ItemGUID"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        Action = string.Empty,
                        AverageCost = row.Field<double?>("AverageCost"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Cost = row.Field<double?>("Cost"),
                        Description = row.Field<string>("Description"),
                        HistoryID = 0,
                        IsHistory = false,
                        ItemID = row.Field<long>("ItemID"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ManufacturerName = row.Field<string>("ManufacturerName"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                        OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                        QuickListHistoryID = 0,
                        SupplierName = row.Field<string>("SupplierName"),
                        SupplierPartNo = row.Field<string>("SupplierPartNo"),
                        DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                        ItemType = row.Field<Int32>("ItemType"),
                        LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                        Markup = row.Field<double?>("Markup"),
                        Unit = row.Field<string>("Unit"),
                        PackingQuantity = row.Field<double?>("PackingQuantity"),
                        SellPrice = row.Field<double?>("SellPrice"),
                        SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                        DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                        AddedFrom = row.Field<string>("AddedFrom"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        ItemUDF1 = row.Field<string>("ItemUDF1"),
                        ItemUDF2 = row.Field<string>("ItemUDF2"),
                        ItemUDF3 = row.Field<string>("ItemUDF3"),
                        ItemUDF4 = row.Field<string>("ItemUDF4"),
                        ItemUDF5 = row.Field<string>("ItemUDF5"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        BinID = row.Field<long?>("BinID"),
                        BinName = row.Field<string>("BinName"),
                        ConsignedQuantity = row.Field<double?>("ConsignedQuantity"),
                        Consignment = row.Field<bool>("Consignment")
                    }).ToList();
                }
            }
            return lstQuickListDetail;

            #region Old Method            
            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    string strQuer = @"EXEC [QL_GetQLLineItemPagedRecord] " + StartRowIndex + "," + MaxRows + ",'" + SearchTerm + "','" + sortColumnName + "','" + QuickListGUID + "'," + (IsDeleted ? "1" : "0") + "," + (IsArchived ? "1" : "0") + "," + RoomID + "," + CompanyId;

            //    if (SuplierIds != null && SuplierIds.Any())
            //        strQuer += ", '" + string.Join(",", SuplierIds) + "'";
            //    else
            //        strQuer += ", " + "null";

            //    List<QuickListDetailDTO> obj = (from u in context.Database.SqlQuery<QuickListDetailDTO>(strQuer)
            //                                    select new QuickListDetailDTO
            //                                    {
            //                                        ID = u.ID,
            //                                        Quantity = u.Quantity,
            //                                        QuickListGUID = u.QuickListGUID,
            //                                        CompanyID = u.CompanyID,
            //                                        IsArchived = u.IsArchived,
            //                                        IsDeleted = u.IsDeleted,
            //                                        Created = u.Created,
            //                                        LastUpdated = u.LastUpdated,
            //                                        CreatedBy = u.CreatedBy,
            //                                        LastUpdatedBy = u.LastUpdatedBy,
            //                                        Room = u.Room,
            //                                        GUID = u.GUID,
            //                                        ItemGUID = u.ItemGUID,
            //                                        CreatedByName = u.CreatedByName,
            //                                        RoomName = u.RoomName,
            //                                        UpdatedByName = u.UpdatedByName,
            //                                        Action = string.Empty,
            //                                        AverageCost = u.AverageCost,
            //                                        CategoryName = u.CategoryName,
            //                                        Cost = u.Cost,
            //                                        Description = u.Description,
            //                                        HistoryID = 0,
            //                                        IsHistory = false,
            //                                        ItemID = u.ItemID,
            //                                        ItemNumber = u.ItemNumber,
            //                                        ManufacturerName = u.ManufacturerName,
            //                                        ManufacturerNumber = u.ManufacturerNumber,
            //                                        OnHandQuantity = u.OnHandQuantity,
            //                                        OnOrderQuantity = u.OnOrderQuantity,
            //                                        OnTransferQuantity = u.OnTransferQuantity,
            //                                        QuickListHistoryID = 0,
            //                                        SupplierName = u.SupplierName,
            //                                        SupplierPartNo = u.SupplierPartNo,
            //                                        TotalRecords = u.TotalRecords,
            //                                        DateCodeTracking = u.DateCodeTracking,
            //                                        ItemType = u.ItemType,
            //                                        LotNumberTracking = u.LotNumberTracking,
            //                                        Markup = u.Markup,
            //                                        Unit = u.Unit,
            //                                        PackingQuantity = u.PackingQuantity,
            //                                        SellPrice = u.SellPrice,
            //                                        SerialNumberTracking = u.SerialNumberTracking,
            //                                        DefaultPullQuantity = u.DefaultPullQuantity,
            //                                        DefaultReorderQuantity = u.DefaultReorderQuantity,
            //                                        ReceivedOn = u.ReceivedOn,
            //                                        ReceivedOnWeb = u.ReceivedOnWeb,
            //                                        AddedFrom = u.AddedFrom,
            //                                        EditedFrom = u.EditedFrom,
            //                                        ItemUDF1 = u.ItemUDF1,
            //                                        ItemUDF2 = u.ItemUDF2,
            //                                        ItemUDF3 = u.ItemUDF3,
            //                                        ItemUDF4 = u.ItemUDF4,
            //                                        ItemUDF5 = u.ItemUDF5,
            //                                        UDF1 = u.UDF1,
            //                                        UDF2 = u.UDF2,
            //                                        UDF3 = u.UDF3,
            //                                        UDF4 = u.UDF4,
            //                                        UDF5 = u.UDF5,
            //                                        BinID = u.BinID,
            //                                        BinName = u.BinName,
            //                                        ConsignedQuantity = u.ConsignedQuantity,
            //                                        Consignment = u.Consignment
            //                                    }).ToList();
            //    TotalCount = 0;
            //    if (obj != null && obj.Count > 0)
            //    {
            //        TotalCount = obj[0].TotalRecords;
            //    }
            //    return obj;

            #endregion

        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DB_DeleteQLLineITemRecords(string IDs, Int64 userid)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string strIDs = "";
                string[] strGUIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in strGUIDs)
                {
                    //strIDs += "''" + item + "'',";
                    strIDs += item + ",";
                }

                strIDs = strIDs.TrimEnd(',');

                strQuery += "Exec QL_DeleteQLLineItem " + userid.ToString() + ", '" + strIDs + "'";
                if (userid > 0 && !string.IsNullOrWhiteSpace(strQuery))
                {
                    context.Database.ExecuteSqlCommand(strQuery);
                    return true;
                }

            }
            return false;
        }

        #endregion

        public List<QuickListDetailDTO> GetQuickListItemsRequirePackslip(Int64 RoomID, Int64 CompanyID, string QuickListGUID, bool IsDelete, bool IsArchived)
        {
            List<QuickListDetailDTO> obj = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@QuickListGUID", QuickListGUID), 
                                                    new SqlParameter("@RoomID", RoomID),
                                                    new SqlParameter("@CompanyID", CompanyID) };

                obj = (from u in context.Database.SqlQuery<QuickListDetailDTO>("exec [GetQuickListItemsRequirePackslip] @QuickListGUID,@RoomID,@CompanyID", params1)
                       select new QuickListDetailDTO
                       {
                           ID = u.ID,
                           QuickListGUID = u.QuickListGUID,
                           CompanyID = u.CompanyID,
                           IsArchived = u.IsArchived,
                           IsDeleted = u.IsDeleted,
                           Room = u.Room,
                           GUID = u.GUID,
                           ItemGUID = u.ItemGUID,
                           RoomName = u.RoomName,
                           ReceivedOn = u.ReceivedOn,
                           ReceivedOnWeb = u.ReceivedOnWeb,
                           AddedFrom = u.AddedFrom,
                           EditedFrom = u.EditedFrom,
                       }).AsParallel().ToList();
            }
            return obj;
        }

        public bool isItemBinExistinQL(Guid QuickListGUID, Guid itemGUID, long? BinID, Guid? QuickListItemGUID = null)
        {
            bool Retval = false;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (QuickListItemGUID == null)
                {
                    var qry = (from qli in context.QuickListItems
                               where qli.QuickListGUID == QuickListGUID && qli.ItemGUID == itemGUID && qli.BinID == BinID && (qli.IsDeleted ?? false) == false
                               select qli);
                    if (qry.Any())
                    {
                        Retval = true;
                    }
                }
                else
                {
                    var qry = (from qli in context.QuickListItems
                               where qli.GUID != QuickListItemGUID
                                     && qli.QuickListGUID == QuickListGUID && qli.ItemGUID == itemGUID && qli.BinID == BinID && (qli.IsDeleted ?? false) == false
                               select qli);
                    if (qry.Any())
                    {
                        Retval = true;
                    }
                }
            }
            return Retval;
        }

        public bool CheckIfValueExistsInQLUDF(long CompanyId, long RoomId, Guid QuickListItemGUID, Int64 UDFID, string UDFColumnName, string Value, out long objUDFOptionId)
        {
            objUDFOptionId = 0;
            bool ReturnValue = false;
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var onjUdf = new
                    {
                        ID = (long)0,
                        UDFColumnName = "",
                        UDFControlType = "",
                        UDFDefaultValue = ""
                    };

                    if (UDFID != 0)
                    {
                        onjUdf = (from U in context.UDFs
                                  where U.UDFTableName.ToUpper() == "QUICKLISTITEMS"
                                  && U.CompanyID == CompanyId && U.Room == RoomId
                                        && U.ID == UDFID
                                        && (U.IsDeleted == false)
                                  select new
                                  {
                                      ID = U.ID,
                                      UDFColumnName = U.UDFColumnName,
                                      UDFControlType = U.UDFControlType,
                                      UDFDefaultValue = U.UDFDefaultValue
                                  }).FirstOrDefault();

                        UDFColumnName = onjUdf.UDFColumnName;
                    }
                    else
                    {
                        onjUdf = (from U in context.UDFs
                                  where U.UDFTableName.ToUpper() == "QUICKLISTITEMS"
                                  && U.CompanyID == CompanyId && U.Room == RoomId
                                        && U.UDFColumnName.ToUpper() == UDFColumnName.ToUpper()
                                        && (U.IsDeleted == false)
                                  select new
                                  {
                                      ID = U.ID,
                                      UDFColumnName = U.UDFColumnName,
                                      UDFControlType = U.UDFControlType,
                                      UDFDefaultValue = U.UDFDefaultValue
                                  }).FirstOrDefault();
                    }

                    if (onjUdf != null)
                    {
                        if (onjUdf.UDFControlType.ToUpper() == "TEXTBOX")
                        {
                            if (Value.Trim().ToUpper() == onjUdf.UDFDefaultValue.Trim().ToUpper())
                                ReturnValue = true;
                            else
                                ReturnValue = false;
                        }
                        else
                        {
                            objUDFOptionId = (from UO in context.UDFOptions
                                              where UO.UDFID == onjUdf.ID && (UO.IsDeleted == false)
                                                    && UO.UDFOption1.Trim().ToUpper() == Value.Trim().ToUpper()
                                              select UO.ID
                                             ).FirstOrDefault();

                            if (objUDFOptionId != 0)
                                ReturnValue = true;
                            else
                                ReturnValue = false;
                        }
                    }
                    else
                    {
                        ReturnValue = false;
                    }

                    if (ReturnValue == false)
                    {
                        var QLIUdfs = (from QLM in context.QuickListMasters
                                       join QLI in context.QuickListItems on QLM.GUID equals QLI.QuickListGUID
                                       where QLM.CompanyID == CompanyId && QLM.Room == RoomId && QLI.GUID == QuickListItemGUID
                                             && (QLM.IsDeleted == false)
                                             && (QLI.IsDeleted == null || QLI.IsDeleted == false)
                                       select new
                                       {
                                           UDF1 = QLI.UDF1,
                                           UDF2 = QLI.UDF2,
                                           UDF3 = QLI.UDF3,
                                           UDF4 = QLI.UDF4,
                                           UDF5 = QLI.UDF5,
                                       }).FirstOrDefault();

                        if (QLIUdfs != null)
                        {
                            if (UDFColumnName.ToUpper() == "UDF1" && Value.Trim().ToUpper() == QLIUdfs.UDF1.Trim().ToUpper())
                                ReturnValue = true;
                            else if (UDFColumnName.ToUpper() == "UDF2" && Value.Trim().ToUpper() == QLIUdfs.UDF2.Trim().ToUpper())
                                ReturnValue = true;
                            else if (UDFColumnName.ToUpper() == "UDF3" && Value.Trim().ToUpper() == QLIUdfs.UDF3.Trim().ToUpper())
                                ReturnValue = true;
                            else if (UDFColumnName.ToUpper() == "UDF4" && Value.Trim().ToUpper() == QLIUdfs.UDF4.Trim().ToUpper())
                                ReturnValue = true;
                            else if (UDFColumnName.ToUpper() == "UDF5" && Value.Trim().ToUpper() == QLIUdfs.UDF5.Trim().ToUpper())
                                return true;
                            else
                                ReturnValue = false;
                        }
                        else
                            ReturnValue = false;
                    }
                }

                return ReturnValue;
            }
            catch
            {
                return false;
            }
            finally
            {
            }
        }

        public Dictionary<string, string> GetUDFValueOrDefault(long CompanyId, long RoomId, Guid QuicklistGUID, Guid ItemGUID, Guid QuickListDetailGUID, out long? BinId)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    var lstDefUdf = (from U in context.UDFs
                                     where U.UDFTableName.ToUpper() == "QUICKLISTITEMS"
                                           && U.CompanyID == CompanyId && U.Room == RoomId
                                           && (U.IsDeleted == false)
                                     select new
                                     {
                                         UDFColumnName = U.UDFColumnName,
                                         UDFDefaultValue = U.UDFDefaultValue
                                     }).ToList();

                    var DefUdfs = new
                    {
                        UDF1 = (lstDefUdf == null ? "" : (from D in lstDefUdf where D.UDFColumnName.ToUpper() == "UDF1" select D.UDFDefaultValue).FirstOrDefault()),
                        UDF2 = (lstDefUdf == null ? "" : (from D in lstDefUdf where D.UDFColumnName.ToUpper() == "UDF2" select D.UDFDefaultValue).FirstOrDefault()),
                        UDF3 = (lstDefUdf == null ? "" : (from D in lstDefUdf where D.UDFColumnName.ToUpper() == "UDF3" select D.UDFDefaultValue).FirstOrDefault()),
                        UDF4 = (lstDefUdf == null ? "" : (from D in lstDefUdf where D.UDFColumnName.ToUpper() == "UDF4" select D.UDFDefaultValue).FirstOrDefault()),
                        UDF5 = (lstDefUdf == null ? "" : (from D in lstDefUdf where D.UDFColumnName.ToUpper() == "UDF5" select D.UDFDefaultValue).FirstOrDefault()),
                    };

                    var QLIUdfs = (from QLM in context.QuickListMasters
                                   join QLI in context.QuickListItems on QLM.GUID equals QLI.QuickListGUID
                                   where QLM.CompanyID == CompanyId && QLM.Room == RoomId && QLM.GUID == QuicklistGUID && QLI.ItemGUID == ItemGUID
                                         && QLI.GUID == QuickListDetailGUID
                                         && (QLM.IsDeleted == false)
                                         && (QLI.IsDeleted == null || QLI.IsDeleted == false)
                                   select new
                                   {
                                       UDF1 = ((QLI.UDF1 != null && QLI.UDF1.Trim() != "") ? QLI.UDF1 : ((DefUdfs.UDF1 != null && DefUdfs.UDF1.Trim() != "") ? DefUdfs.UDF1 : "")),
                                       UDF2 = ((QLI.UDF2 != null && QLI.UDF2.Trim() != "") ? QLI.UDF2 : ((DefUdfs.UDF2 != null && DefUdfs.UDF2.Trim() != "") ? DefUdfs.UDF2 : "")),
                                       UDF3 = ((QLI.UDF3 != null && QLI.UDF3.Trim() != "") ? QLI.UDF3 : ((DefUdfs.UDF3 != null && DefUdfs.UDF3.Trim() != "") ? DefUdfs.UDF3 : "")),
                                       UDF4 = ((QLI.UDF4 != null && QLI.UDF4.Trim() != "") ? QLI.UDF4 : ((DefUdfs.UDF4 != null && DefUdfs.UDF4.Trim() != "") ? DefUdfs.UDF4 : "")),
                                       UDF5 = ((QLI.UDF5 != null && QLI.UDF5.Trim() != "") ? QLI.UDF5 : ((DefUdfs.UDF5 != null && DefUdfs.UDF5.Trim() != "") ? DefUdfs.UDF5 : "")),
                                       BinId = QLI.BinID
                                   }).FirstOrDefault();

                    if (QLIUdfs == null)
                    {
                        if (DefUdfs == null)
                        {
                            QLIUdfs = new
                            {
                                UDF1 = "",
                                UDF2 = "",
                                UDF3 = "",
                                UDF4 = "",
                                UDF5 = "",
                                BinId = (long?)0
                            };
                        }
                        else
                        {
                            QLIUdfs = new
                            {
                                UDF1 = DefUdfs.UDF1,
                                UDF2 = DefUdfs.UDF2,
                                UDF3 = DefUdfs.UDF3,
                                UDF4 = DefUdfs.UDF4,
                                UDF5 = DefUdfs.UDF5,
                                BinId = (long?)0
                            };
                        }
                    }

                    BinId = QLIUdfs.BinId;
                    return (new Dictionary<string, string>() { { "UDF1", QLIUdfs.UDF1 }, { "UDF2", QLIUdfs.UDF2 }, { "UDF3", QLIUdfs.UDF3 }, { "UDF4", QLIUdfs.UDF4 }, { "UDF5", QLIUdfs.UDF5 } });
                }
            }
            catch
            {
                BinId = 0;
                return null;
            }
            finally
            {
            }
        }

        public string GetUDFValueOrDefault(long CompanyId, long RoomId, Guid QuickListItemGUID, Int64 UDFID, string UDFColumnName)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    if (UDFID != 0)
                    {
                        UDFColumnName = (from U in context.UDFs
                                         where U.CompanyID == CompanyId && U.Room == RoomId
                                               && U.ID == UDFID
                                               && (U.IsDeleted == false)
                                         select U.UDFColumnName).FirstOrDefault();
                    }

                    var lstDefUdf = (from U in context.UDFs
                                     where U.UDFTableName.ToUpper() == "QUICKLISTITEMS"
                                           && U.CompanyID == CompanyId && U.Room == RoomId
                                           && (U.IsDeleted == false)
                                     select new
                                     {
                                         UDFColumnName = U.UDFColumnName,
                                         UDFDefaultValue = U.UDFDefaultValue
                                     }).ToList();

                    var DefUdfs = new
                    {
                        UDF1 = (lstDefUdf == null ? "" : (from D in lstDefUdf where D.UDFColumnName.ToUpper() == "UDF1" select D.UDFDefaultValue).FirstOrDefault()),
                        UDF2 = (lstDefUdf == null ? "" : (from D in lstDefUdf where D.UDFColumnName.ToUpper() == "UDF2" select D.UDFDefaultValue).FirstOrDefault()),
                        UDF3 = (lstDefUdf == null ? "" : (from D in lstDefUdf where D.UDFColumnName.ToUpper() == "UDF3" select D.UDFDefaultValue).FirstOrDefault()),
                        UDF4 = (lstDefUdf == null ? "" : (from D in lstDefUdf where D.UDFColumnName.ToUpper() == "UDF4" select D.UDFDefaultValue).FirstOrDefault()),
                        UDF5 = (lstDefUdf == null ? "" : (from D in lstDefUdf where D.UDFColumnName.ToUpper() == "UDF5" select D.UDFDefaultValue).FirstOrDefault()),
                    };

                    var QLIUdfs = (from QLM in context.QuickListMasters
                                   join QLI in context.QuickListItems on QLM.GUID equals QLI.QuickListGUID
                                   where QLM.CompanyID == CompanyId && QLM.Room == RoomId && QLI.GUID == QuickListItemGUID
                                         && (QLM.IsDeleted == false)
                                         && (QLI.IsDeleted == null || QLI.IsDeleted == false)
                                   select new
                                   {
                                       UDF1 = ((QLI.UDF1 != null && QLI.UDF1.Trim() != "") ? QLI.UDF1 : ((DefUdfs.UDF1 != null && DefUdfs.UDF1.Trim() != "") ? DefUdfs.UDF1 : "")),
                                       UDF2 = ((QLI.UDF2 != null && QLI.UDF2.Trim() != "") ? QLI.UDF2 : ((DefUdfs.UDF2 != null && DefUdfs.UDF2.Trim() != "") ? DefUdfs.UDF2 : "")),
                                       UDF3 = ((QLI.UDF3 != null && QLI.UDF3.Trim() != "") ? QLI.UDF3 : ((DefUdfs.UDF3 != null && DefUdfs.UDF3.Trim() != "") ? DefUdfs.UDF3 : "")),
                                       UDF4 = ((QLI.UDF4 != null && QLI.UDF4.Trim() != "") ? QLI.UDF4 : ((DefUdfs.UDF4 != null && DefUdfs.UDF4.Trim() != "") ? DefUdfs.UDF4 : "")),
                                       UDF5 = ((QLI.UDF5 != null && QLI.UDF5.Trim() != "") ? QLI.UDF5 : ((DefUdfs.UDF5 != null && DefUdfs.UDF5.Trim() != "") ? DefUdfs.UDF5 : "")),
                                   }).FirstOrDefault();

                    if (QLIUdfs == null)
                    {
                        if (DefUdfs == null)
                        {
                            QLIUdfs = new
                            {
                                UDF1 = "",
                                UDF2 = "",
                                UDF3 = "",
                                UDF4 = "",
                                UDF5 = "",
                            };
                        }
                        else
                        {
                            QLIUdfs = new
                            {
                                UDF1 = DefUdfs.UDF1,
                                UDF2 = DefUdfs.UDF2,
                                UDF3 = DefUdfs.UDF3,
                                UDF4 = DefUdfs.UDF4,
                                UDF5 = DefUdfs.UDF5,
                            };
                        }
                    }

                    string UDFValue = "";
                    switch (UDFColumnName.Trim().ToUpper())
                    {
                        case ("UDF1"):
                            UDFValue = QLIUdfs.UDF1;
                            break;
                        case ("UDF2"):
                            UDFValue = QLIUdfs.UDF2;
                            break;
                        case ("UDF3"):
                            UDFValue = QLIUdfs.UDF3;
                            break;
                        case ("UDF4"):
                            UDFValue = QLIUdfs.UDF4;
                            break;
                        case ("UDF5"):
                            UDFValue = QLIUdfs.UDF5;
                            break;
                        default:
                            UDFValue = "";
                            break;
                    }

                    return UDFValue;
                }
            }
            catch
            {
                return null;
            }
            finally
            {
            }
        }
        public List<QuickListMasterDTO> GetQuickListMasterChangeLog(string IDs)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<QuickListMasterDTO>("exec [GetQuickListMasterChangeLog] @ID", params1).ToList();
            }
        }
    }

}
