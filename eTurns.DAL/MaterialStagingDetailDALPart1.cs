using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;
using System.Web;
using eTurns.DTO.Resources;
using System.Globalization;

namespace eTurns.DAL
{
    public class MaterialStagingDetailDAL : eTurnsBaseDAL
    {
        public IEnumerable<MaterialStagingDetailDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        }

        public MaterialStagingDetailDTO GetMaterialStagingDetailbyItemGUIDANDStagingBINID(Guid MSGUID, Int64 StagingBinID, Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<MaterialStagingDetailDTO>(@"
                            SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                            A.Room as RoomId ,L.BinNumber as StagingBinName
                            FROM MaterialStagingDetail A 
                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                            left outer join BinMaster L on A.StagingBinID = L.ID
                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                            WHERE A.ItemGUID = '" + ItemGUID.ToString() + "' AND A.MaterialStagingGUID = '" + MSGUID + "' And A.StagingBinID = " + StagingBinID + " AND  A.CompanyID = " + CompanyID.ToString() + " AND A.Room = " + RoomID.ToString())
                        select new MaterialStagingDetailDTO
                        {
                            ID = u.ID,
                            ItemGUID = u.ItemGUID,
                            StagingBinID = u.StagingBinID,
                            StagingBinName = u.StagingBinName,
                            BinID = u.BinID,
                            GUID = u.GUID,
                            MaterialStagingGUID = u.MaterialStagingGUID,
                            Quantity = u.Quantity,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            RoomId = u.RoomId,
                            CompanyID = u.CompanyID,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb
                        }).FirstOrDefault();
            }
        }

        public List<MaterialStagingDetailDTO> GetMaterialStagingDetailbyItemGUIDANDStagingBIN(Guid MSGUID, Int64 StagingBinID, Guid ItemGUID, Int64 RoomID, Int64 CompanyID) 
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<MaterialStagingDetailDTO>(@"
                            SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                            A.Room as RoomId ,L.BinNumber as StagingBinName
                            FROM MaterialStagingDetail A 
                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                            left outer join BinMaster L on A.StagingBinID = L.ID
                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                            WHERE ISNULL(A.IsDeleted,0) = 0 AND  A.ItemGUID = '" + ItemGUID.ToString() + "' AND A.MaterialStagingGUID = '" + MSGUID + "' And A.StagingBinID = " + StagingBinID + " AND  A.CompanyID = " + CompanyID.ToString() + " AND A.Room = " + RoomID.ToString())
                        select new MaterialStagingDetailDTO
                        {
                            ID = u.ID,
                            ItemGUID = u.ItemGUID,
                            StagingBinID = u.StagingBinID,
                            StagingBinName = u.StagingBinName,
                            BinID = u.BinID,
                            GUID = u.GUID,
                            MaterialStagingGUID = u.MaterialStagingGUID,
                            Quantity = u.Quantity,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            RoomId = u.RoomId,
                            CompanyID = u.CompanyID,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb
                        }).ToList();
            }
        }

        public IEnumerable<MaterialStagingDetailDTO> GetAllRecords(Guid MaterialStagingGUID, Int64 RoomID, Int64 CompanyId)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<MaterialStagingDetailDTO> obj = (from u in context.ExecuteStoreQuery<MaterialStagingDetailDTO>(@"
                            SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                            A.Room as RoomId ,L.BinNumber as StagingBinName
                            FROM MaterialStagingDetail A 
                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                            left outer join BinMaster L on A.StagingBinID = L.ID
                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                            WHERE A.Room = '" + Convert.ToString(RoomID) + "' AND A.MaterialStagingGUID = '" + Convert.ToString(MaterialStagingGUID) + "' AND A.CompanyID = '" + Convert.ToString(CompanyId) + "'")
                                                             select new MaterialStagingDetailDTO
                                                             {
                                                                 ID = u.ID,
                                                                 ItemGUID = u.ItemGUID,
                                                                 StagingBinID = u.StagingBinID,
                                                                 StagingBinName = u.StagingBinName,
                                                                 BinID = u.BinID,
                                                                 GUID = u.GUID,
                                                                 MaterialStagingGUID = u.MaterialStagingGUID,
                                                                 Quantity = u.Quantity,
                                                                 IsDeleted = u.IsDeleted,
                                                                 IsArchived = u.IsArchived,
                                                                 Created = u.Created,
                                                                 Updated = u.Updated,
                                                                 CreatedBy = u.CreatedBy,
                                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                                 RoomId = u.RoomId,
                                                                 CompanyID = u.CompanyID,
                                                                 CreatedByName = u.CreatedByName,
                                                                 UpdatedByName = u.UpdatedByName,
                                                                 RoomName = u.RoomName,
                                                                 AddedFrom = u.AddedFrom,
                                                                 EditedFrom = u.EditedFrom,
                                                                 ReceivedOn = u.ReceivedOn,
                                                                 ReceivedOnWeb = u.ReceivedOnWeb
                                                             }).AsParallel().ToList().OrderBy("ID DESC");
                return obj;
                //  ObjCache = CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.AddCacheItem("Cached_MaterialStagingDetail_" + CompanyID.ToString(), obj);
            }

            //  return GetCachedData(RoomID, CompanyId).Where(t => t.MaterialStagingGUID == MaterialStagingGUID).OrderBy("ID DESC");
        }

        public IEnumerable<MaterialStagingDetailDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<MaterialStagingDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
            IEnumerable<MaterialStagingDetailDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

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
                //IEnumerable<MaterialStagingDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))

                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<MaterialStagingDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm)).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public MaterialStagingDetailDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID) 
        {
            return GetCachedData(RoomID, CompanyID).SingleOrDefault(t => t.GUID == GUID);
        }

        public IEnumerable<MaterialStagingDetailDTO> GetAllRecords(string StagingBinNumber, Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId).Where(t => t.StagingBinName == StagingBinNumber).OrderBy("ID DESC");
        }

        public IEnumerable<MaterialStagingDetailDTO> GetAllRecordsWithoutCaching(string StagingBinNumber, Int64 RoomID, Int64 CompanyId)
        {
            //Get Cached-Media

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<MaterialStagingDetailDTO> obj = (from u in context.ExecuteStoreQuery<MaterialStagingDetailDTO>(@"
                            SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                            A.Room as RoomId ,L.BinNumber as StagingBinName
                            FROM MaterialStagingDetail A 
                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                            left outer join BinMaster L on A.StagingBinID = L.ID
                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                            WHERE A.CompanyID = " + CompanyId.ToString())
                                                             select new MaterialStagingDetailDTO
                                                             {
                                                                 ID = u.ID,
                                                                 ItemGUID = u.ItemGUID,
                                                                 StagingBinID = u.StagingBinID,
                                                                 StagingBinName = u.StagingBinName,
                                                                 BinID = u.BinID,
                                                                 GUID = u.GUID,
                                                                 MaterialStagingGUID = u.MaterialStagingGUID,
                                                                 Quantity = u.Quantity,
                                                                 IsDeleted = u.IsDeleted,
                                                                 IsArchived = u.IsArchived,
                                                                 Created = u.Created,
                                                                 Updated = u.Updated,
                                                                 CreatedBy = u.CreatedBy,
                                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                                 RoomId = u.RoomId,
                                                                 CompanyID = u.CompanyID,
                                                                 CreatedByName = u.CreatedByName,
                                                                 UpdatedByName = u.UpdatedByName,
                                                                 RoomName = u.RoomName,
                                                                 AddedFrom = u.AddedFrom,
                                                                 EditedFrom = u.EditedFrom,
                                                                 ReceivedOn = u.ReceivedOn,
                                                                 ReceivedOnWeb = u.ReceivedOnWeb
                                                             }).AsParallel().ToList();
                return obj.Where(t => t.RoomId == RoomID);
            }
        }

        public MaterialStagingDetailDTO GetRecordwithoutCaching(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            return GetDatawithoutcaching(RoomID, CompanyID).SingleOrDefault(t => t.GUID == GUID);
        }

        public IEnumerable<MaterialStagingDetailDTO> GetDatawithoutcaching(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media           
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<MaterialStagingDetailDTO> obj = (from u in context.ExecuteStoreQuery<MaterialStagingDetailDTO>(@"
                            SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                            A.Room as RoomId ,L.BinNumber as StagingBinName
                            FROM MaterialStagingDetail A 
                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                            left outer join BinMaster L on A.StagingBinID = L.ID
                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                            WHERE A.CompanyID = " + CompanyID.ToString())
                                                             select new MaterialStagingDetailDTO
                                                             {
                                                                 ID = u.ID,
                                                                 ItemGUID = u.ItemGUID,
                                                                 StagingBinID = u.StagingBinID,
                                                                 StagingBinName = u.StagingBinName,
                                                                 BinID = u.BinID,
                                                                 GUID = u.GUID,
                                                                 MaterialStagingGUID = u.MaterialStagingGUID,
                                                                 Quantity = u.Quantity,
                                                                 IsDeleted = u.IsDeleted,
                                                                 IsArchived = u.IsArchived,
                                                                 Created = u.Created,
                                                                 Updated = u.Updated,
                                                                 CreatedBy = u.CreatedBy,
                                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                                 RoomId = u.RoomId,
                                                                 CompanyID = u.CompanyID,
                                                                 CreatedByName = u.CreatedByName,
                                                                 UpdatedByName = u.UpdatedByName,
                                                                 RoomName = u.RoomName,
                                                                 AddedFrom = u.AddedFrom,
                                                                 EditedFrom = u.EditedFrom,
                                                                 ReceivedOn = u.ReceivedOn,
                                                                 ReceivedOnWeb = u.ReceivedOnWeb
                                                             }).AsParallel().ToList();

                return obj;
            }
        }

        public IEnumerable<MaterialStagingDetailDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted) 
        {
            IEnumerable<MaterialStagingDetailDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                ObjCache = GetCachedData(RoomID, CompanyID);
                ObjCache = ObjCache.Where(t => t.IsDeleted == false && t.IsArchived == false);
            }
            else
            {
                string sSQL = "";
                if (IsArchived && IsDeleted)
                {
                    sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
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
                    ObjCache = (from u in context.ExecuteStoreQuery<MaterialStagingDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM MaterialStagingDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + " AND A.Room=" + RoomID.ToString() + " AND " + sSQL)
                                select new MaterialStagingDetailDTO
                                {
                                    ID = u.ID,
                                    ItemGUID = u.ItemGUID,
                                    StagingBinID = u.StagingBinID,
                                    BinID = u.BinID,
                                    GUID = u.GUID,
                                    MaterialStagingGUID = u.MaterialStagingGUID,
                                    Quantity = u.Quantity,
                                    IsDeleted = u.IsDeleted,
                                    IsArchived = u.IsArchived,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    RoomId = u.RoomId,
                                    CompanyID = u.CompanyID,
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
            return ObjCache;
        }

        public List<MaterialStagingDetailDTO> GetHistoryRecordbyMaterialStagingID(Guid MaterialStagingGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //return (from u in context.ExecuteStoreQuery<MaterialStagingDetailDTO>(@"
                //SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                //I.ItemNumber,I.Description,L.BinNumber as 'StagingBinName' FROM 
                //MaterialStagingDetail_History A 
                //LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                //LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                //left outer join ItemMaster I on A.ItemGUID = I.GUID
                //left outer join BinMaster L on A.StagingBinID = L.ID
                //LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.MaterialStagingGUID='" + MaterialStagingGUID.ToString() + "'")
                var paramsMSD1 = new SqlParameter[] { new SqlParameter("@MaterialStagingGUID", MaterialStagingGUID), new SqlParameter("@dbName", DataBaseName) };
                return (from u in context.ExecuteStoreQuery<MaterialStagingDetailDTO>("exec [GetMaterialStagingDetailChangeLog] @MaterialStagingGUID,@dbName", paramsMSD1)
                        select new MaterialStagingDetailDTO
                        {
                            ID = u.ID,
                            MSHistoryID = u.MSHistoryID,
                            Action = u.Action,
                            ItemNumber = u.ItemNumber,
                            StagingBinName = u.StagingBinName,
                            ItemGUID = u.ItemGUID,
                            StagingBinID = u.StagingBinID,
                            BinID = u.BinID,
                            GUID = u.GUID,
                            MaterialStagingGUID = u.MaterialStagingGUID,
                            Quantity = u.Quantity,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            RoomId = u.RoomId,
                            CompanyID = u.CompanyID,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb
                        }).ToList();
            }
        }

        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE MaterialStagingDetail SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ",IsDeleted=1,EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE GUID ='" + item.ToString() + "';";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<MaterialStagingDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.GetCacheItem("Cached_MaterialStagingDetail_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<MaterialStagingDetailDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.GUID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.AppendToCacheItem("Cached_MaterialStagingDetail_" + CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        public bool UnDeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE MaterialStagingDetail SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=0,EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID ='" + item.ToString() + "';";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<MaterialStagingDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.GetCacheItem("Cached_MaterialStagingDetail_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<MaterialStagingDetailDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.GUID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.AppendToCacheItem("Cached_MaterialStagingDetail_" + CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        public bool DeleteMSHeaderItems(string ids, Int64 UserID, Int64 RoomID, Int64 CompanyId, string RoomDateFormat, long SessionUserId)
        {
            MaterialStagingDAL objMaterialStagingDAL = new MaterialStagingDAL(base.DataBaseName);
            Guid[] MSGUIDS = new Guid[] { Guid.Empty };
            if (objMaterialStagingDAL.DeleteRecords(ids, UserID, CompanyId, RoomID))
            {
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    if (ids.Contains(','))
                    {
                        ids = ids.TrimEnd(',');
                        MSGUIDS = Array.ConvertAll<string, Guid>(ids.Split(','), delegate (string intParameter) { return Guid.Parse(intParameter.ToString()); });
                    }
                    else
                    {
                        MSGUIDS[0] = Guid.Parse(ids);
                    }
                }

            }

            eTurns.DAL.CacheHelper<IEnumerable<MaterialStagingDTO>>.InvalidateCache();
            eTurns.DAL.CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.InvalidateCache();

            IEnumerable<MaterialStagingDetailDTO> lstItemsToDelete = GetAllRecords(RoomID, CompanyId).Where(t => MSGUIDS.Contains(t.MaterialStagingGUID.GetValueOrDefault(Guid.Empty)) && t.IsArchived == false && t.IsDeleted == false);
            foreach (var item in lstItemsToDelete)
            {

                DeleteSingleMSDtlItem(item, UserID, RoomID, CompanyId, RoomDateFormat, SessionUserId);
            }
            return true;
        }

        public IEnumerable<MaterialStagingDetailDTO> GetStagingLocationByItem(Guid ItemGuid, Int64 RoomID, Int64 CompanyID)
        {
            string query = @"SELECT  MD.ItemGUID as ItemGUID
		                            ,MD.MaterialStagingGUID as MaterialStagingGUID
		                            ,MD.StagingBinID	as StagingBinID
		                            ,IM.ItemNumber as ItemNumber
		                            ,MS.StagingName as MaterialStagingName
		                            ,BM.BinNumber as StagingBinName
		                            ,Sum(ISNULL(Quantity,0)) as Quantity
                            FROM MaterialStagingDetail MD Inner Join MaterialStaging MS ON MD.MaterialStagingGUID = MS.[GUID]
							                              Left outer Join ItemMaster IM ON MD.ItemGuid = IM.[GUID]
							                              Left outer join BinMaster BM ON MD.StagingBinID = BM.ID
                            WHERE ISNULL(MD.IsDeleted,0) =0 AND ISNULL(MD.IsArchived,0) =0
                            AND MD.Room = " + RoomID + @" and MD.CompanyID = " + CompanyID + @"
                            AND MD.ItemGUID  = '" + ItemGuid + @"'
                            Group by MD.ItemGUID,MD.MaterialStagingGUID, MD.StagingBinID,IM.ItemNumber,MS.StagingName,BM.BinNumber";

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<MaterialStagingDetailDTO> obj = (from u in context.ExecuteStoreQuery<MaterialStagingDetailDTO>(query)
                                                             select new MaterialStagingDetailDTO
                                                             {

                                                                 ItemGUID = u.ItemGUID,
                                                                 MaterialStagingGUID = u.MaterialStagingGUID,
                                                                 StagingBinID = u.StagingBinID,
                                                                 ItemNumber = u.ItemNumber,
                                                                 MaterialStagingName = u.MaterialStagingName,
                                                                 StagingBinName = u.StagingBinName,
                                                                 Quantity = u.Quantity,
                                                             }).AsParallel().ToList();
                return obj;
            }

        }

        public IEnumerable<MaterialStagingDetailDTO> GetStagingLocationByItemOnlyOpen(Guid ItemGuid, Int64 RoomID, Int64 CompanyID)
        {
            string query = @"SELECT  MD.ItemGUID as ItemGUID
		                            ,MD.MaterialStagingGUID as MaterialStagingGUID
		                            ,MD.StagingBinID	as StagingBinID
		                            ,IM.ItemNumber as ItemNumber
		                            ,MS.StagingName as MaterialStagingName
		                            ,BM.BinNumber as StagingBinName
		                            ,Sum(ISNULL(Quantity,0)) as Quantity
                            FROM MaterialStagingDetail MD Inner Join MaterialStaging MS ON MD.MaterialStagingGUID = MS.[GUID]
							                              Left outer Join ItemMaster IM ON MD.ItemGuid = IM.[GUID]
							                              Left outer join BinMaster BM ON MD.StagingBinID = BM.ID
                            WHERE ISNULL(MD.IsDeleted,0) =0 AND ISNULL(MD.IsArchived,0) =0
                            AND ISNULL(MS.StagingStatus,0)=1
                            AND MD.Room = " + RoomID + @" and MD.CompanyID = " + CompanyID + @"
                            AND MD.ItemGUID  = '" + ItemGuid + @"'
                            Group by MD.ItemGUID,MD.MaterialStagingGUID, MD.StagingBinID,IM.ItemNumber,MS.StagingName,BM.BinNumber";

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<MaterialStagingDetailDTO> obj = (from u in context.ExecuteStoreQuery<MaterialStagingDetailDTO>(query)
                                                             select new MaterialStagingDetailDTO
                                                             {

                                                                 ItemGUID = u.ItemGUID,
                                                                 MaterialStagingGUID = u.MaterialStagingGUID,
                                                                 StagingBinID = u.StagingBinID,
                                                                 ItemNumber = u.ItemNumber,
                                                                 MaterialStagingName = u.MaterialStagingName,
                                                                 StagingBinName = u.StagingBinName,
                                                                 Quantity = u.Quantity,
                                                             }).AsParallel().ToList();
                return obj;
            }

        }

        public IEnumerable<MaterialStagingDetailDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        }

        public IEnumerable<MaterialStagingDetailDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<MaterialStagingDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.GetCacheItem("Cached_MaterialStagingDetail_" + CompanyID.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<MaterialStagingDetailDTO> obj = (from u in context.ExecuteStoreQuery<MaterialStagingDetailDTO>(@"
                            SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                            A.Room as RoomId ,L.BinNumber as StagingBinName
                            FROM MaterialStagingDetail A 
                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                            left outer join BinMaster L on A.StagingBinID = L.ID
                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                            WHERE A.CompanyID = " + CompanyID.ToString())
                                                                 select new MaterialStagingDetailDTO
                                                                 {
                                                                     ID = u.ID,
                                                                     ItemGUID = u.ItemGUID,
                                                                     StagingBinID = u.StagingBinID,
                                                                     StagingBinName = u.StagingBinName,
                                                                     BinID = u.BinID,
                                                                     GUID = u.GUID,
                                                                     MaterialStagingGUID = u.MaterialStagingGUID,
                                                                     Quantity = u.Quantity,
                                                                     IsDeleted = u.IsDeleted,
                                                                     IsArchived = u.IsArchived,
                                                                     Created = u.Created,
                                                                     Updated = u.Updated,
                                                                     CreatedBy = u.CreatedBy,
                                                                     LastUpdatedBy = u.LastUpdatedBy,
                                                                     RoomId = u.RoomId,
                                                                     CompanyID = u.CompanyID,
                                                                     CreatedByName = u.CreatedByName,
                                                                     UpdatedByName = u.UpdatedByName,
                                                                     RoomName = u.RoomName,
                                                                     AddedFrom = u.AddedFrom,
                                                                     EditedFrom = u.EditedFrom,
                                                                     ReceivedOn = u.ReceivedOn,
                                                                     ReceivedOnWeb = u.ReceivedOnWeb
                                                                 }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.AddCacheItem("Cached_MaterialStagingDetail_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache.Where(t => t.RoomId == RoomID);
        }

    }
}
