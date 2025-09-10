using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class KitMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public KitMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public KitMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Method]

        public int GetWIPKitCountForRedCircle(long RoomID, long CompanyID, List<long> SupplierIDs)
        {
            string strSupplierIds = string.Empty;
            if (SupplierIDs != null && SupplierIDs.Any())
            {
                strSupplierIds = string.Join(",", SupplierIDs);
            }

            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID)
                                                , new SqlParameter("@CompanyID", CompanyID)
                                                , new SqlParameter("@SupplierIDs", strSupplierIds)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<int>("exec [GetWIPKitCountForRedCircle] @RoomID,@CompanyID,@SupplierIDs", params1).FirstOrDefault();
            }

        }

        #endregion

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KitMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, TimeZoneInfo CurrentTimeZone, string strImport = "")
        {
            IEnumerable<KitMasterDTO> ObjCache = null;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            KitDetailDAL objKitDetailDTO = new KitDetailDAL(base.DataBaseName);
            List<ItemMasterDTO> objItem = null;
            bool IsBuildBreak = true;
            int ItemType = 3;

            if (SupplierIds != null && SupplierIds.Any())
            {
                int TotalRecordCount = 0;
                objItem = objItemDAL.GetPagedItemsForModel(0, Int32.MaxValue, out TotalRecordCount, string.Empty, "ID ASC", RoomID, CompanyID, false, false, SupplierIds, true, true, true, 0, "", "", CurrentTimeZone, false, null, null);
                objItem = objItem.Where(x => x.IsBuildBreak == IsBuildBreak && x.ItemType == ItemType).ToList();
            }
            else
            {
                if (strImport == "Import")
                {
                    objItem = objItemDAL.GetAllRecordsForImport(IsBuildBreak, ItemType, RoomID, CompanyID, false, false); // .Where(x => x.IsBuildBreak.GetValueOrDefault(false) == true && x.ItemType == 3 && x.IsDeleted.GetValueOrDefault(false) == IsDeleted && x.IsArchived.GetValueOrDefault(false) == IsArchived).ToList();
                }
                else
                {
                    var tmpSupplierIds = new List<long>();
                    int TotalRecordCount = 0;
                    objItem = objItemDAL.GetPagedItemsForModel(0, Int32.MaxValue, out TotalRecordCount, string.Empty, "ID ASC", RoomID, CompanyID, false, false, tmpSupplierIds, true, true, true, 0, "", "", CurrentTimeZone, false, null, null);
                    objItem = objItem.Where(x => x.IsBuildBreak == IsBuildBreak && x.ItemType == ItemType).ToList();
                }

            }

            IEnumerable<KitMasterDTO> obj = (from u in objItem
                                             select new KitMasterDTO
                                             {
                                                 ID = u.ID,
                                                 KitPartNumber = u.ItemNumber,
                                                 Description = u.Description,
                                                 ReOrderType = u.IsPurchase,
                                                 KitCategory = 0,//u.KitCategory,
                                                 AvailableKitQuantity = u.OnHandQuantity.GetValueOrDefault(0),
                                                 KitDemand = u.SuggestedOrderQuantity.GetValueOrDefault(0),// u.OnTransferQuantity.GetValueOrDefault(0) + (u.MaximumQuantity - u.OnHandQuantity.GetValueOrDefault(0)) > 0 ? u.OnTransferQuantity.GetValueOrDefault(0) + (u.MaximumQuantity - u.OnHandQuantity.GetValueOrDefault(0)) : 0,
                                                 KitCost = u.Cost,
                                                 KitSellPrice = u.SellPrice,
                                                 MinimumKitQuantity = u.MinimumQuantity,
                                                 MaximumKitQuantity = u.MaximumQuantity,
                                                 UDF1 = u.UDF1,
                                                 UDF2 = u.UDF2,
                                                 UDF3 = u.UDF3,
                                                 UDF4 = u.UDF4,
                                                 UDF5 = u.UDF5,
                                                 GUID = u.GUID,
                                                 Created = u.Created.GetValueOrDefault(DateTime.MinValue),
                                                 Updated = u.Updated.GetValueOrDefault(DateTime.MinValue),
                                                 CreatedBy = u.CreatedBy.GetValueOrDefault(0),
                                                 LastUpdatedBy = u.LastUpdatedBy.GetValueOrDefault(0),
                                                 IsDeleted = u.IsDeleted,
                                                 IsArchived = u.IsArchived,
                                                 CompanyID = u.CompanyID.GetValueOrDefault(0),
                                                 Room = u.Room.GetValueOrDefault(0),
                                                 CreatedByName = u.CreatedByName,
                                                 UpdatedByName = u.UpdatedByName,
                                                 RoomName = u.RoomName,
                                                 IsNotAbleToDelete = false,
                                                 CriticalQuantity = u.CriticalQuantity,
                                                 SerialNumberTracking = u.SerialNumberTracking,
                                                 LotNumberTracking = u.LotNumberTracking,
                                                 DateCodeTracking = u.DateCodeTracking,
                                                 Consignment = u.Consignment,
                                                 ItemType = u.ItemType,
                                                 DefaultLocation = u.DefaultLocation,
                                                 ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTime.MinValue),
                                                 ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTime.MinValue),
                                                 AddedFrom = u.AddedFrom,
                                                 EditedFrom = u.EditedFrom,
                                                 // AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Kits")
                                             }).AsParallel().ToList();
            ObjCache = obj;

            //}

            ObjCache = ObjCache.Where(x => x.Room == RoomID);
            IEnumerable<KitDetailDTO> lstKitDetailDTO = objKitDetailDTO.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted, true);
            foreach (var item in ObjCache)
            {
                item.KitItemList = lstKitDetailDTO.Where(x => x.KitGUID == item.GUID).ToList();
                if (item.KitItemList != null && item.KitItemList.Count > 0)
                {
                    item.NoOfItemsInKit = item.KitItemList.Count;
                    item.AvailableWIPKit = item.KitItemList.Where(x => (x.ItemDetail ?? new ItemMasterDTO()).ItemType != 4).Min(x => x.QuantityReadyForAssembly.GetValueOrDefault(0));

                    if (item.AvailableKitQuantity.GetValueOrDefault(0) > 0 || item.AvailableWIPKit.GetValueOrDefault(0) > 0)
                    {
                        item.IsNotAbleToDelete = true;
                    }
                    else if (item.KitItemList != null && item.KitItemList.Count > 0)
                    {
                        item.IsNotAbleToDelete = item.KitItemList.FindIndex(x => x.AvailableItemsInWIP.GetValueOrDefault(0) > 0) >= 0;
                    }
                }
            }
            return ObjCache;
        }

        /// <summary>
        /// Get Paged Records from the KitMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<KitMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, List<long> SupplierIds, TimeZoneInfo CurrentTimeZone)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, SupplierIds, CurrentTimeZone).OrderBy("ID DESC");
        }

        /// <summary>
        /// Get Paged Records from the KitMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<KitMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Int64 UserID, List<long> SupplierIds, string RoomDateFormat, TimeZoneInfo CurrentTimeZone, string MainFilter = "")
        {
            return DB_GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, UserID, SupplierIds, RoomDateFormat, CurrentTimeZone, IsDeleted, IsArchived);
        }

        /// <summary>
        /// Get Particullar Record from the KitMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public KitMasterDTO GetRecord(string GUID)
        {
            return DB_GetKitRecord(Guid.Parse(GUID));
        }

        /// <summary>
        /// Get Particullar Record from the KitMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public KitMasterDTO GetKitHistoryByHistoryIdFull(long HistoryID)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@HistoryID", HistoryID)                                                
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<KitMasterDTO>("exec GetKitHistoryByHistoryIdFull @HistoryID ", params1).SingleOrDefault();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase KitMaster
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(KitMasterDTO objDTO)
        {
            return 0;
            //objDTO.Created = DateTimeUtility.DateTimeNow;
            //objDTO.Updated = DateTimeUtility.DateTimeNow;
            //objDTO.IsArchived = false;
            //objDTO.IsDeleted = false;

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    KitMaster obj = new KitMaster();
            //    obj.ID = 0;
            //    obj.KitPartNumber = objDTO.KitPartNumber;
            //    obj.Description = objDTO.Description;
            //    obj.ReOrderType = objDTO.ReOrderType;
            //    obj.KitCategory = objDTO.KitCategory;
            //    obj.AvailableKitQuentity = objDTO.AvailableKitQuantity;
            //    obj.AvailableWIPKit = objDTO.AvailableWIPKit;

            //    obj.KitCost = objDTO.KitCost;
            //    obj.KitSellPrice = objDTO.KitSellPrice;
            //    obj.MinimumKitQuentity = objDTO.MinimumKitQuantity;
            //    obj.MaximumKitQuentity = objDTO.MaximumKitQuantity;

            //    if (objDTO.KitDemand.GetValueOrDefault(0) <= 0)
            //        obj.KitDemand = 0;
            //    else
            //        obj.KitDemand = objDTO.KitDemand;

            //    obj.UDF1 = objDTO.UDF1;
            //    obj.UDF2 = objDTO.UDF2;
            //    obj.UDF3 = objDTO.UDF3;
            //    obj.UDF4 = objDTO.UDF4;
            //    obj.UDF5 = objDTO.UDF5;
            //    obj.GUID = Guid.NewGuid();
            //    obj.Created = objDTO.Created;
            //    obj.Updated = objDTO.Updated;
            //    obj.CreatedBy = objDTO.CreatedBy;
            //    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
            //    obj.IsDeleted = objDTO.IsDeleted;
            //    obj.IsArchived = objDTO.IsArchived;
            //    obj.CompanyID = objDTO.CompanyID;
            //    obj.Room = objDTO.Room;
            //    obj.CriticalQuantity = objDTO.CriticalQuantity;
            //    obj.ReceivedOn = DateTimeUtility.DateTimeNow;
            //    obj.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            //    obj.AddedFrom = "Web";
            //    obj.EditedFrom = "Web";
            //    context.KitMasters.Add(obj);
            //    context.SaveChanges();
            //    objDTO.ID = obj.ID;
            //    objDTO.GUID = obj.GUID;

            //    if (objDTO.ID > 0)
            //    {
            //        //Get Cached-Media
            //        IEnumerable<KitMasterDTO> ObjCache = CacheHelper<IEnumerable<KitMasterDTO>>.GetCacheItem("Cached_KitMaster_" + objDTO.CompanyID.ToString());
            //        if (ObjCache != null)
            //        {
            //            List<KitMasterDTO> tempC = new List<KitMasterDTO>();
            //            tempC.Add(objDTO);

            //            IEnumerable<KitMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
            //            CacheHelper<IEnumerable<KitMasterDTO>>.AppendToCacheItem("Cached_KitMaster_" + objDTO.CompanyID.ToString(), NewCache);
            //        }
            //    }

            //    return obj.ID;
            //}

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(KitMasterDTO objDTO)
        {
            return false;
            //objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            //objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            //objDTO.Updated = DateTimeUtility.DateTimeNow;

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    KitMaster obj = new KitMaster();
            //    obj.ID = objDTO.ID;
            //    obj.KitPartNumber = objDTO.KitPartNumber;
            //    obj.Description = objDTO.Description;
            //    obj.ReOrderType = objDTO.ReOrderType;
            //    obj.KitCategory = objDTO.KitCategory;
            //    obj.AvailableKitQuantity = objDTO.AvailableKitQuantity;
            //    obj.AvailableWIPKit = objDTO.AvailableWIPKit;// objDTO.KitItemList.Min(x => (x.AvailableItemsInWIP.GetValueOrDefault(0) / x.QuantityPerKit.GetValueOrDefault(0)));//objDTO.AvailableWIPKit;
            //    if (objDTO.KitDemand.GetValueOrDefault(0) <= 0)
            //        obj.KitDemand = 0;
            //    else
            //        obj.KitDemand = objDTO.KitDemand;

            //    obj.KitCost = objDTO.KitCost;
            //    obj.KitSellPrice = objDTO.KitSellPrice;
            //    obj.MinimumKitQuantity = objDTO.MinimumKitQuantity;
            //    obj.MaximumKitQuantity = objDTO.MaximumKitQuantity;
            //    obj.UDF1 = objDTO.UDF1;
            //    obj.UDF2 = objDTO.UDF2;
            //    obj.UDF3 = objDTO.UDF3;
            //    obj.UDF4 = objDTO.UDF4;
            //    obj.UDF5 = objDTO.UDF5;
            //    obj.GUID = objDTO.GUID;
            //    obj.Created = objDTO.Created;
            //    obj.Updated = objDTO.Updated;
            //    obj.CreatedBy = objDTO.CreatedBy;
            //    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
            //    obj.IsDeleted = false;
            //    obj.IsArchived = false;
            //    obj.CompanyID = objDTO.CompanyID;
            //    obj.Room = objDTO.Room;
            //    obj.CriticalQuantity = objDTO.CriticalQuantity;
            //    obj.ReceivedOn = objDTO.ReceivedOn;
            //    obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
            //    obj.AddedFrom = objDTO.AddedFrom;
            //    obj.EditedFrom = objDTO.EditedFrom;

            //    context.KitMasters.Attach(obj);
            //    context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
            //    context.SaveChanges();


            //    //Get Cached-Media
            //    IEnumerable<KitMasterDTO> ObjCache = CacheHelper<IEnumerable<KitMasterDTO>>.GetCacheItem("Cached_KitMaster_" + objDTO.CompanyID.ToString());
            //    if (ObjCache != null)
            //    {
            //        List<KitMasterDTO> objTemp = ObjCache.ToList();
            //        objTemp.RemoveAll(i => i.GUID == objDTO.GUID);
            //        ObjCache = objTemp.AsEnumerable();

            //        List<KitMasterDTO> tempC = new List<KitMasterDTO>();
            //        tempC.Add(objDTO);
            //        IEnumerable<KitMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
            //        CacheHelper<IEnumerable<KitMasterDTO>>.AppendToCacheItem("Cached_KitMaster_" + objDTO.CompanyID.ToString(), NewCache);
            //    }


            //    return true;
            //}
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string GUIDs, Int64 userid, Int64 RoomID, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                string[] arrGuids = GUIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in arrGuids)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        KitDetailDAL kitDetailDAL = new KitDetailDAL(base.DataBaseName);
                        IEnumerable<KitDetailDTO> lstKitDetailDTO = kitDetailDAL.GetAllRecords(RoomID, CompanyID, false, false, false).Where(x => x.KitGUID == Guid.Parse(item));
                        if (lstKitDetailDTO != null && lstKitDetailDTO.Count() > 0)
                        {
                            foreach (var KDDTO in lstKitDetailDTO)
                            {
                                kitDetailDAL.DeleteRecords(KDDTO.GUID.ToString(), userid, CompanyID);
                            }
                        }
                        strQuery += "UPDATE KitMaster SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE GUID = '" + item.ToString() + "';";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);

                return true;
            }
        }

        /// <summary>
        /// Get Available Quantity 
        /// </summary>
        /// <param name="objDetailList"></param>
        /// <returns></returns>
        private double GetAvailableQuantityInGeneralInventory(List<KitDetailDTO> objDetailList)
        {
            double TotalQty = objDetailList.Min(x => x.ItemDetail.OnHandQuantity.GetValueOrDefault(0) / x.QuantityPerKit.GetValueOrDefault(0));
            double TempQty = 0;
            int idx = 0;
            if (objDetailList != null)
            {
                foreach (var item in objDetailList)
                {
                    if (item != null && item.ItemDetail != null)
                    {

                        TempQty = item.ItemDetail.OnHandQuantity.GetValueOrDefault(0) / item.QuantityPerKit.GetValueOrDefault(0);

                        if (idx == 0 || TempQty < TotalQty)
                            TotalQty = TempQty;

                        idx += 1;
                    }
                }
            }

            return TotalQty;
        }

        /// <summary>
        /// Update KitCost
        /// </summary>
        /// <param name="KitID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public KitMasterDTO UpdateKitCost(KitMasterDTO objKitDTO, Int64 RoomID, Int64 CompanyID)
        {
            objKitDTO.KitItemList = new KitDetailDAL(base.DataBaseName).GetAllRecordsByKitGUID(objKitDTO.GUID, RoomID, CompanyID, objKitDTO.IsArchived.GetValueOrDefault(false), objKitDTO.IsDeleted.GetValueOrDefault(false), true).ToList();

            if (objKitDTO != null && objKitDTO.KitItemList.Count > 0)
            {
                //IEnumerable<ItemMasterDTO> Items = new ItemMasterDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyID);

                if (objKitDTO.KitItemList.FindIndex(x => x.ItemDetail.ItemType != 4) >= 0)
                {
                    objKitDTO.AvailableWIPKit = objKitDTO.KitItemList.Where(x => x.ItemDetail.ItemType != 4).Min(x => x.QuantityReadyForAssembly.GetValueOrDefault(0));
                }
                else
                    objKitDTO.AvailableWIPKit = 0;
            }
            return objKitDTO;
        }

        private string GetNarrowSearchString(string SearchTerm, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string NarrowSearchQry = "";
            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                string[] FieldsPara = Fields[1].Split('@');

                DateTime FromdDate = DateTime.Now;
                DateTime ToDate = DateTime.Now;

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
                    string[] arrVal = FieldsPara[0].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join(",", arrVal);

                    NarrowSearchQry += " CreatedBy IN (" + strVal.TrimEnd(',') + ")";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    string[] arrVal = FieldsPara[1].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join(",", arrVal);

                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += " LastUpdatedBy IN (" + strVal.TrimEnd(',') + ")";
                }


                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    FromdDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone);
                    ToDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone);
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += @" (Convert(Datetime,Created) Between Convert(Datetime,''" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "'',105) AND Convert(Datetime,''" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "'',105) )";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    FromdDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone);
                    ToDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone);
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += @" (Convert(Datetime,Updated) Between Convert(Datetime,''" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "'',105) AND Convert(Datetime,''" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "'',105) )";

                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrVal = FieldsPara[4].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join("'',''", arrVal);
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += " UDF1 IN (''" + strVal.TrimEnd(',') + "'')";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrVal = FieldsPara[5].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join("'',''", arrVal);
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += " UDF2 IN (''" + strVal.TrimEnd(',') + "'')";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrVal = FieldsPara[6].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join("'',''", arrVal);
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += " UDF3 IN (''" + strVal.TrimEnd(',') + "'')";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrVal = FieldsPara[7].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join("'',''", arrVal);
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += " UDF4 IN (''" + strVal.TrimEnd(',') + "'')";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrVal = FieldsPara[8].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join("'',''", arrVal);
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += " UDF5 IN (''" + strVal.TrimEnd(',') + "'')";
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[26]))
                {
                    string[] arrVal = FieldsPara[26].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join(",", arrVal);

                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += " ID IN (" + strVal.TrimEnd(',') + ")";
                }
            }
            return NarrowSearchQry;
        }


        /// <summary>
        /// Get Paged Records from the OrderMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public KitMasterDTO DB_GetKitRecord(Int64 KitID)
        {
            //string strQuer = @"EXEC [Kit_GetKitData] ";
            //strQuer += " @ID=" + KitID;
            //KitMasterDTO obj = ExecuteQuery(strQuer).FirstOrDefault();
            //return obj;

            KitMasterDTO obj = new KitMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.CommandTimeout = 120;
                var params1 = new SqlParameter[] {
                                new SqlParameter("@ID", KitID),
                                new SqlParameter("@GUID", (object)DBNull.Value),
                                new SqlParameter("@RoomID", (object)DBNull.Value),
                                new SqlParameter("@CompanyID", (object)DBNull.Value)};
                obj = (from u in context.Database.SqlQuery<KitMasterDTO>("exec Kit_GetKitData @ID,@GUID,@RoomID,@CompanyID", params1)
                       select new KitMasterDTO
                       {
                           ID = u.ID,
                           KitPartNumber = u.KitPartNumber,
                           Description = u.Description,
                           ReOrderType = u.ReOrderType,
                           KitCategory = 0,
                           AvailableKitQuantity = u.AvailableKitQuantity.GetValueOrDefault(0),
                           KitDemand = u.KitDemand.GetValueOrDefault(0),                                                                        
                           KitCost = u.KitCost.GetValueOrDefault(0),
                           KitSellPrice = u.KitSellPrice,
                           MinimumKitQuantity = u.MinimumKitQuantity,
                           MaximumKitQuantity = u.MaximumKitQuantity,

                           UDF1 = u.UDF1,
                           UDF2 = u.UDF2,
                           UDF3 = u.UDF3,
                           UDF4 = u.UDF4,
                           UDF5 = u.UDF5,
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
                           IsNotAbleToDelete = u.IsNotAbleToDelete,
                           CriticalQuantity = u.CriticalQuantity,
                           SerialNumberTracking = u.SerialNumberTracking,
                           LotNumberTracking = u.LotNumberTracking,
                           DateCodeTracking = u.DateCodeTracking,
                           Consignment = u.Consignment,
                           ItemType = u.ItemType,
                           DefaultLocation = u.DefaultLocation,
                           ReceivedOn = u.ReceivedOn,
                           ReceivedOnWeb = u.ReceivedOnWeb,
                           AddedFrom = u.AddedFrom,
                           EditedFrom = u.EditedFrom,
                           NoOfItemsInKit = u.NoOfItemsInKit,
                           AvailableWIPKit = u.AvailableWIPKit,
                           Action = "",
                           AppendedBarcodeString = "",
                           AvailableInGeneralInventory = u.AvailableInGeneralInventory,
                           IsKitBreakAction = u.IsKitBreakAction,
                           IsKitBuildAction = u.IsKitBuildAction,
                           QuantityToBuildBreak = u.QuantityToBuildBreak,
                           UpdatedDate = u.UpdatedDate
                       }).FirstOrDefault();
            }
            return obj;
        }

        /// <summary>
        /// Get Paged Records from the OrderMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public KitMasterDTO DB_GetKitRecord(Guid KitGuID)
        {
            //string strQuer = @"EXEC [Kit_GetKitData] ";
            //strQuer += " @GUID='" + KitGuID + "'";

            //KitMasterDTO obj = ExecuteQuery(strQuer).FirstOrDefault();
            //return obj;

            KitMasterDTO obj = new KitMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.CommandTimeout = 120;
                var params1 = new SqlParameter[] {
                                new SqlParameter("@ID", (object)DBNull.Value),
                                new SqlParameter("@GUID", KitGuID),
                                new SqlParameter("@RoomID", (object)DBNull.Value),
                                new SqlParameter("@CompanyID", (object)DBNull.Value)};
                obj = (from u in context.Database.SqlQuery<KitMasterDTO>("exec Kit_GetKitData @ID,@GUID,@RoomID,@CompanyID", params1)
                       select new KitMasterDTO
                       {
                           ID = u.ID,
                           KitPartNumber = u.KitPartNumber,
                           Description = u.Description,
                           ReOrderType = u.ReOrderType,
                           KitCategory = 0,
                           AvailableKitQuantity = u.AvailableKitQuantity.GetValueOrDefault(0),
                           KitDemand = u.KitDemand.GetValueOrDefault(0),
                           KitCost = u.KitCost.GetValueOrDefault(0),
                           KitSellPrice = u.KitSellPrice,
                           MinimumKitQuantity = u.MinimumKitQuantity,
                           MaximumKitQuantity = u.MaximumKitQuantity,

                           UDF1 = u.UDF1,
                           UDF2 = u.UDF2,
                           UDF3 = u.UDF3,
                           UDF4 = u.UDF4,
                           UDF5 = u.UDF5,
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
                           IsNotAbleToDelete = u.IsNotAbleToDelete,
                           CriticalQuantity = u.CriticalQuantity,
                           SerialNumberTracking = u.SerialNumberTracking,
                           LotNumberTracking = u.LotNumberTracking,
                           DateCodeTracking = u.DateCodeTracking,
                           Consignment = u.Consignment,
                           ItemType = u.ItemType,
                           DefaultLocation = u.DefaultLocation,
                           ReceivedOn = u.ReceivedOn,
                           ReceivedOnWeb = u.ReceivedOnWeb,
                           AddedFrom = u.AddedFrom,
                           EditedFrom = u.EditedFrom,
                           NoOfItemsInKit = u.NoOfItemsInKit,
                           AvailableWIPKit = u.AvailableWIPKit,
                           Action = "",
                           AppendedBarcodeString = "",
                           AvailableInGeneralInventory = u.AvailableInGeneralInventory,
                           IsKitBreakAction = u.IsKitBreakAction,
                           IsKitBuildAction = u.IsKitBuildAction,
                           QuantityToBuildBreak = u.QuantityToBuildBreak,
                           UpdatedDate = u.UpdatedDate
                       }).FirstOrDefault();
            }
            return obj;

        }


        public KitMasterDTO GetKitByGuidFull(Guid KitGuID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.CommandTimeout = 120;
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", KitGuID) };
                return context.Database.SqlQuery<KitMasterDTO>("exec GetKitByGuidFull @GUID", params1).FirstOrDefault();
            }

        }

        /// <summary>
        /// Get Paged Records from the OrderMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        private IEnumerable<KitMasterDTO> DB_GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, Int64 UserID, List<long> SupplierIds,string RoomDateFormat, TimeZoneInfo CurrentTimeZone, bool IsDeleted, bool IsArchived)
        {
            List<KitMasterDTO> lstKits = new List<KitMasterDTO>();
            TotalCount = 0;
            DataSet dsKits = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            string CreatedByIDs = null;
            string UpdatedByIDs = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string IDs = null;

            if (MaxRows < 1)
                MaxRows = 10;
            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());
            if (string.IsNullOrEmpty(sortColumnName))
                sortColumnName = "ID DESC";

            string strSupplierIds = string.Empty;
            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (!String.IsNullOrEmpty(SearchTerm)
                && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
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
                    string[] arrVal = FieldsPara[0].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join(",", arrVal);
                    CreatedByIDs = strVal.TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    string[] arrVal = FieldsPara[1].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join(",", arrVal);
                    UpdatedByIDs = strVal.TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrVal = FieldsPara[4].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join("'',''", arrVal);
                    UDF1 = strVal.TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrVal = FieldsPara[5].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join("'',''", arrVal);
                    UDF2 = strVal.TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrVal = FieldsPara[6].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join("'',''", arrVal);
                    UDF3 = strVal.TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrVal = FieldsPara[7].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join("'',''", arrVal);
                    UDF4 = strVal.TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrVal = FieldsPara[8].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join("'',''", arrVal);
                    UDF5 = strVal.TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[26]))
                {
                    string[] arrVal = FieldsPara[26].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string strVal = string.Join(",", arrVal);
                    IDs = strVal.TrimEnd(',');
                }
                dsKits = SqlHelper.ExecuteDataset(EturnsConnection, "Kit_GetKitPagedData", StartRowIndex, MaxRows, CompanyID, RoomID, sortColumnName, SearchTerm, CreatedByIDs, UpdatedByIDs, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5,IDs, UserID, strSupplierIds, IsDeleted, IsArchived);
            }
            else
            {
                dsKits = SqlHelper.ExecuteDataset(EturnsConnection, "Kit_GetKitPagedData", StartRowIndex, MaxRows, CompanyID, RoomID, sortColumnName, SearchTerm, CreatedByIDs, UpdatedByIDs, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IDs, UserID, strSupplierIds, IsDeleted, IsArchived);
            }
            if (dsKits != null && dsKits.Tables.Count > 0)
            {
                DataTable dtKit = dsKits.Tables[0];
                if (dtKit.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtKit.Rows[0]["TotalRecords"]);
                    lstKits = dtKit.AsEnumerable()
                    .Select(row => new KitMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        KitPartNumber = row.Field<string>("KitPartNumber"),
                        Description = row.Field<string>("Description"),
                        ReOrderType = row.Field<bool?>("ReOrderType").HasValue ? row.Field<bool?>("ReOrderType") : false,
                        KitCategory = 0,
                        AvailableKitQuantity = row.Field<double?>("AvailableKitQuantity").GetValueOrDefault(0),
                        KitDemand = row.Field<double?>("KitDemand").GetValueOrDefault(0),                     
                        KitCost = row.Field<double?>("KitCost").GetValueOrDefault(0),
                        KitSellPrice = row.Field<double?>("KitSellPrice").GetValueOrDefault(0),
                        MinimumKitQuantity = row.Field<double?>("MinimumKitQuantity").GetValueOrDefault(0),
                        MaximumKitQuantity = row.Field<double?>("MaximumKitQuantity").GetValueOrDefault(0),

                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        GUID = row.Field<Guid>("GUID"),
                        Created = row.Field<DateTime>("Created"),
                        Updated = row.Field<DateTime>("Updated"),
                        CreatedBy = row.Field<long>("CreatedBy"),
                        LastUpdatedBy = row.Field<long>("LastUpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),                        
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long>("CompanyID"),
                        Room = row.Field<long>("Room"),
                        CreatedByName =row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        IsNotAbleToDelete = row.Field<bool>("IsNotAbleToDelete"),
                        CriticalQuantity = row.Field<double?>("CriticalQuantity"),
                        SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                        LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                        DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                        Consignment = row.Field<bool>("Consignment"),
                        ItemType = row.Field<Int32>("ItemType"),
                        DefaultLocation = row.Field<long?>("DefaultLocation"),
                        ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                        AddedFrom = row.Field<string>("AddedFrom"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        NoOfItemsInKit = row.Field<Int32>("NoOfItemsInKit"),
                        AvailableWIPKit = row.Field<double?>("AvailableWIPKit"),
                        Action = "",
                        AppendedBarcodeString = "",
                        AvailableInGeneralInventory = row.Field<double>("AvailableInGeneralInventory"),
                        IsKitBreakAction =  row.Field<bool>("IsKitBreakAction"),
                        IsKitBuildAction = row.Field<bool>("IsKitBuildAction"),
                        QuantityToBuildBreak = row.Field<double?>("QuantityToBuildBreak")                        
                    }).ToList();
                }
            }
            return lstKits;

            #region OldMethod

            //string strQuer = @"EXEC [Kit_GetKitPagedData] ";
            ////string NarrowSearchQry = "";

            //if (MaxRows < 1)
            //    MaxRows = 10;

            //StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

            //strQuer += @"@StartRowIndex=" + StartRowIndex;
            //strQuer += ",@MaxRows=" + MaxRows;
            //strQuer += ",@CompnayID=" + CompanyID;
            //strQuer += ",@RoomID=" + RoomID;
            //if (string.IsNullOrEmpty(sortColumnName))
            //    sortColumnName = "ID DESC";

            //strQuer += ",@SortColumnName='" + sortColumnName + "'";

            //NarrowSearchQry = GetNarrowSearchString(SearchTerm, RoomDateFormat, CurrentTimeZone);

            //if (!string.IsNullOrEmpty(NarrowSearchQry))
            //    strQuer += ",@NarrowSearchTerm='" + NarrowSearchQry + "'";

            //string[] stringSeparators = new string[] { "[###]" };
            //string[] Fields = null;

            //if (!string.IsNullOrEmpty(SearchTerm))
            //    Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

            //string newSearchValue = string.Empty;
            ////WI-1461 related changes 
            //if (Fields != null && Fields.Length > 2)
            //{
            //    if (!string.IsNullOrEmpty(Fields[2]))
            //        newSearchValue = Fields[2];
            //    else
            //        newSearchValue = string.Empty;
            //}
            //else
            //{
            //    newSearchValue = string.Empty;
            //}

            //if (!string.IsNullOrEmpty(newSearchValue))
            //    strQuer += ",@SearchTerm='" + newSearchValue + "'";

            //if (UserID > 0)
            //    strQuer += ",@UserID=" + UserID;

            //if (SupplierIds != null && SupplierIds.Any())
            //{
            //    strQuer += ",@SupplierIds='" + string.Join(",", SupplierIds) + "'";
            //}
            //strQuer += ",@IsDeleted=" + IsDeleted;
            //strQuer += ",@IsArchived=" + IsArchived;
            //IEnumerable<KitMasterDTO> obj = ExecuteQuery(strQuer);

            //TotalCount = 0;
            //if (obj != null && obj.Count() > 0)
            //{
            //    TotalCount = obj.ElementAt(0).TotalRecords;
            //}
            //return obj;

            #endregion
        }
        public IEnumerable<ExportKitDTO> GetKitExport(string ItemGUIDs, Int64 RoomID, Int64 CompanyID)
        {
            List<Guid> arrids = new List<Guid>();
            if (!string.IsNullOrWhiteSpace(ItemGUIDs))
            {
                foreach (string item in ItemGUIDs.Split(','))
                {
                    Guid temp = Guid.Empty;
                    if (Guid.TryParse(item, out temp))
                    {
                        arrids.Add(temp);
                    }
                }
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ExportKitDTO> obj = null;
                var params1 = new SqlParameter[] { };
                List<ExportKitDTO> lstresult;
                params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@ItemGUIDs", ItemGUIDs) };
                lstresult = context.Database.SqlQuery<ExportKitDTO>("exec ExportKit @RoomId,@CompanyId,@ItemGUIDs", params1).ToList();

                obj = (from u in lstresult
                       select new ExportKitDTO
                       {
                           KitPartNumber = u.KitPartNumber,
                           ItemNumber = u.ItemNumber,
                           QuantityPerKit = u.QuantityPerKit,
                           IsDeleted = u.IsDeleted,
                           IsBuildBreak = u.IsBuildBreak,
                           OnHandQuantity = u.OnHandQuantity


                       }).ToList();
                return obj;
            }
        }

        /// <summary>
        /// Executer Query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private IEnumerable<KitMasterDTO> ExecuteQuery(string query)
        {
            IEnumerable<KitMasterDTO> obj = null;
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    context.Database.CommandTimeout = 120;
                    obj = (from u in context.Database.SqlQuery<KitMasterDTO>(query)
                           select new KitMasterDTO
                           {
                               ID = u.ID,
                               KitPartNumber = u.KitPartNumber,
                               Description = u.Description,
                               ReOrderType = u.ReOrderType,
                               KitCategory = 0,//u.KitCategory,
                               AvailableKitQuantity = u.AvailableKitQuantity.GetValueOrDefault(0),
                               KitDemand = u.KitDemand.GetValueOrDefault(0),// u.OnTransferQuantity.GetValueOrDefault(0) + (u.MaximumQuantity - u.OnHandQuantity.GetValueOrDefault(0)) > 0 ? u.OnTransferQuantity.GetValueOrDefault(0) + (u.MaximumQuantity - u.OnHandQuantity.GetValueOrDefault(0)) : 0,
                               //KitCost = u.KitCost.GetValueOrDefault(0)* u.AvailableKitQuantity.GetValueOrDefault(0),
                               KitCost = u.KitCost.GetValueOrDefault(0),// * u.AvailableKitQuantity.GetValueOrDefault(0),
                               KitSellPrice = u.KitSellPrice,
                               MinimumKitQuantity = u.MinimumKitQuantity,
                               MaximumKitQuantity = u.MaximumKitQuantity,

                               UDF1 = u.UDF1,
                               UDF2 = u.UDF2,
                               UDF3 = u.UDF3,
                               UDF4 = u.UDF4,
                               UDF5 = u.UDF5,
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
                               IsNotAbleToDelete = u.IsNotAbleToDelete,
                               CriticalQuantity = u.CriticalQuantity,
                               SerialNumberTracking = u.SerialNumberTracking,
                               LotNumberTracking = u.LotNumberTracking,
                               DateCodeTracking = u.DateCodeTracking,
                               Consignment = u.Consignment,
                               ItemType = u.ItemType,
                               DefaultLocation = u.DefaultLocation,
                               ReceivedOn = u.ReceivedOn,
                               ReceivedOnWeb = u.ReceivedOnWeb,
                               AddedFrom = u.AddedFrom,
                               EditedFrom = u.EditedFrom,
                               NoOfItemsInKit = u.NoOfItemsInKit,
                               AvailableWIPKit = u.AvailableWIPKit,
                               Action = "",
                               AppendedBarcodeString = "",
                               AvailableInGeneralInventory = u.AvailableInGeneralInventory,
                               IsKitBreakAction = u.IsKitBreakAction,
                               IsKitBuildAction = u.IsKitBuildAction,
                               QuantityToBuildBreak = u.QuantityToBuildBreak,
                               UpdatedDate = u.UpdatedDate,
                               TotalRecords = u.TotalRecords,
                           }).AsParallel().ToList();

                    return obj;
                }
            }
            finally
            {
                obj = null;
            }
        }

        /// <summary>
        /// This method is used to get the data for the narrow search for the Kit list
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <param name="NarrowSearchKey"></param>
        /// <returns></returns>
        public List<NarrowSearchDTO> GetKitListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? string.Empty) };
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetKitListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }

    }
}


