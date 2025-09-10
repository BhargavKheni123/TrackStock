using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DAL
{
    public class ProjectSpendItemsDAL : eTurnsBaseDAL
    {
        public List<ProjectSpendItemsDTO> GetHistoryRecordByProjectID(Guid ProjectGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ProjectSpendItemsDTO>(@"
                SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, 
                D.RoomName,I.ItemNumber,I.Description, I.SerialNumberTracking 
                ,ISNULL(I.Cost,0) AS ItemCost
                FROM 
                ProjectSpendItems_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID 
                left outer join ItemMaster I on A.ItemGUID = I.GUID
                WHERE A.ProjectGUID='" + ProjectGUID.ToString() + "' Order by A.HistoryID desc")
                        select new ProjectSpendItemsDTO
                        {
                            ID = u.ID,
                            ProjectGUID = u.ProjectGUID,
                            HistoryID = u.HistoryID,
                            Action = u.Action,
                            ItemNumber = u.ItemNumber,
                            Description = u.Description,
                            ItemGUID = u.ItemGUID,
                            QuantityLimit = u.QuantityLimit,
                            QuantityUsed = u.QuantityUsed,
                            DollarLimitAmount = u.DollarLimitAmount,
                            DollarUsedAmount = u.DollarUsedAmount,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            SerialNumberTracking = u.SerialNumberTracking,
                            ItemCost = u.ItemCost,
                            UpdatedByName = u.UpdatedByName,
                            CreatedByName = u.CreatedByName,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                        }).ToList();
            }
        }

    }

    /// <summary>
    /// Get Paged Records from the ProjectSpendItems Table
    /// </summary>
    /// <param name="StartRowIndex">StartRowIndex</param>
    /// <param name="MaxRows">MaxRows</param>
    /// <param name="TotalCount">TotalCount</param>
    /// <param name="SearchTerm">SearchTerm</param>
    /// <param name="sortColumnName">sortColumnName</param>
    /// <returns></returns>
    public IEnumerable<ProjectSpendItemsDTO> GetAllRecords(Guid ProjectSpendGUID, Int64 RoomID, Int64 CompanyId, List<long> SupplierIds)
    {
        return GetCachedData(ProjectSpendGUID, RoomID, CompanyId, SupplierIds).Where(t => t.IsDeleted == false && t.IsArchived == false && t.ProjectGUID == ProjectSpendGUID).OrderBy("ID DESC");
    }

    /// <summary>
    /// Get Particullar Record from the ProjectSpendItems by ID
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns></returns>
    public ProjectSpendItemsDTO GetHistoryRecord(Int64 id)
    {
        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        {
            return (from u in context.ExecuteStoreQuery<ProjectSpendItemsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ProjectSpendItems_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                    select new ProjectSpendItemsDTO
                    {
                        ID = u.ID,
                        ProjectGUID = u.ProjectGUID,
                        ItemGUID = u.ItemGUID,
                        QuantityLimit = u.QuantityLimit,
                        QuantityUsed = u.QuantityUsed,
                        DollarLimitAmount = u.DollarLimitAmount,
                        DollarUsedAmount = u.DollarUsedAmount,
                        UDF1 = u.UDF1,
                        UDF2 = u.UDF2,
                        UDF3 = u.UDF3,
                        UDF4 = u.UDF4,
                        UDF5 = u.UDF5,
                        Created = u.Created,
                        LastUpdated = u.LastUpdated,
                        CreatedBy = u.CreatedBy,
                        LastUpdatedBy = u.LastUpdatedBy,
                        Room = u.Room,
                        IsDeleted = u.IsDeleted,
                        IsArchived = u.IsArchived,
                        CompanyID = u.CompanyID,
                        GUID = u.GUID,
                        ReceivedOn = u.ReceivedOn,
                        ReceivedOnWeb = u.ReceivedOnWeb,
                        AddedFrom = u.AddedFrom,
                        EditedFrom = u.EditedFrom,
                    }).SingleOrDefault();
        }
    }

    public IEnumerable<ProjectSpendItemsDTO> GetPagedRecords(Guid ProjectGUID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
    {
        var tmpsupplierIds = new List<long>();
        IEnumerable<ProjectSpendItemsDTO> ObjCache = GetCachedData(ProjectGUID, RoomID, CompanyID, tmpsupplierIds);
        IEnumerable<ProjectSpendItemsDTO> ObjGlobalCache = ObjCache;
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
            //IEnumerable<ProjectSpendItemsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
            string[] stringSeparators = new string[] { "[###]" };
            string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

            ObjCache = ObjCache.Where(t =>
                   ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
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
            //IEnumerable<ProjectSpendItemsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
            TotalCount = ObjCache.Where
                (
                    t => t.ID.ToString().Contains(SearchTerm) ||
                    (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                ).Count();
            return ObjCache.Where
                (t => t.ID.ToString().Contains(SearchTerm) ||
                    (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

        }
    }

    /// <summary>
    /// Common Method to Cache table wise data, if not Cached then insert it to Cache
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ProjectSpendItemsDTO> GetCachedData(Guid ProjectSpendGUID, Int64 RoomID, Int64 CompanyID, List<long> SupplierIds)
    {
        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        {
            if (SupplierIds != null && SupplierIds.Any())
            {
                return (from u in context.ExecuteStoreQuery<ProjectSpendItemsDTO>(@"SELECT A.*,I.ItemNumber,I.Description, I.SerialNumberTracking,  B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
                                    ,ISNULL(I.Cost,0) AS ItemCost
                                FROM ProjectSpendItems A 
                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                LEFT OUTER JOIN Room D on A.Room = D.ID 
                                left outer join ItemMaster I on A.ItemGUID = I.GUID 
                                Where A.ProjectGUID = '" + ProjectSpendGUID.ToString() + "'  AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + " AND I.SupplierID IN (" + string.Join(",", SupplierIds) + ")")
                        select new ProjectSpendItemsDTO
                        {
                            ID = u.ID,
                            ProjectGUID = u.ProjectGUID,
                            ItemGUID = u.ItemGUID,
                            QuantityLimit = u.QuantityLimit,
                            QuantityUsed = u.QuantityUsed,
                            DollarLimitAmount = u.DollarLimitAmount,
                            DollarUsedAmount = u.DollarUsedAmount,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            ItemNumber = u.ItemNumber,
                            Description = u.Description,
                            RoomName = u.RoomName,
                            SerialNumberTracking = u.SerialNumberTracking,
                            ItemCost = u.ItemCost,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                        }).AsParallel().ToList();
            }
            else
            {
                return (from u in context.ExecuteStoreQuery<ProjectSpendItemsDTO>(@"SELECT A.*,I.ItemNumber,I.Description, I.SerialNumberTracking,  B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
                                ,ISNULL(I.Cost,0) AS ItemCost
                                FROM ProjectSpendItems A 
                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                LEFT OUTER JOIN Room D on A.Room = D.ID 
                                left outer join ItemMaster I on A.ItemGUID = I.GUID 
                                Where A.ProjectGUID = '" + ProjectSpendGUID.ToString() + "'  AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString())
                        select new ProjectSpendItemsDTO
                        {
                            ID = u.ID,
                            ProjectGUID = u.ProjectGUID,
                            ItemGUID = u.ItemGUID,
                            QuantityLimit = u.QuantityLimit,
                            QuantityUsed = u.QuantityUsed,
                            DollarLimitAmount = u.DollarLimitAmount,
                            DollarUsedAmount = u.DollarUsedAmount,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            ItemNumber = u.ItemNumber,
                            Description = u.Description,
                            RoomName = u.RoomName,
                            SerialNumberTracking = u.SerialNumberTracking,
                            ItemCost = u.ItemCost,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                        }).AsParallel().ToList();
            }
        }

    }

    public ProjectSpendItemsDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID)
    {
        //return GetCachedData(RoomID, CompanyID).Single(t => t.GUID == GUID);
        //Get Cached-Media
        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        {
            return (from u in context.ExecuteStoreQuery<ProjectSpendItemsDTO>(@"SELECT A.*,I.ItemNumber,I.Description, I.SerialNumberTracking, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
                                ,ISNULL(I.Cost,0) AS ItemCost
                                FROM ProjectSpendItems A 
                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                LEFT OUTER JOIN Room D on A.Room = D.ID 
                                left outer join ItemMaster I on A.ItemGUID = I.GUID 
                                Where A.GUID = '" + GUID.ToString() + "' AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString())
                    select new ProjectSpendItemsDTO
                    {
                        ID = u.ID,
                        ProjectGUID = u.ProjectGUID,
                        ItemGUID = u.ItemGUID,
                        QuantityLimit = u.QuantityLimit,
                        QuantityUsed = u.QuantityUsed,
                        DollarLimitAmount = u.DollarLimitAmount,
                        DollarUsedAmount = u.DollarUsedAmount,
                        UDF1 = u.UDF1,
                        UDF2 = u.UDF2,
                        UDF3 = u.UDF3,
                        UDF4 = u.UDF4,
                        UDF5 = u.UDF5,
                        Created = u.Created,
                        LastUpdated = u.LastUpdated,
                        CreatedBy = u.CreatedBy,
                        LastUpdatedBy = u.LastUpdatedBy,
                        Room = u.Room,
                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                        CompanyID = u.CompanyID,
                        GUID = u.GUID,
                        CreatedByName = u.CreatedByName,
                        UpdatedByName = u.UpdatedByName,
                        ItemNumber = u.ItemNumber,
                        Description = u.Description,
                        RoomName = u.RoomName,
                        SerialNumberTracking = u.SerialNumberTracking,
                        ItemCost = u.ItemCost,
                        ReceivedOn = u.ReceivedOn,
                        ReceivedOnWeb = u.ReceivedOnWeb,
                        AddedFrom = u.AddedFrom,
                        EditedFrom = u.EditedFrom,
                    }).SingleOrDefault();
        }

        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE ProjectSpendItems SET LastUpdated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE GUID ='" + item.ToString() + "';";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                return true;
            }
        }
    }
}
