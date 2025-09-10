using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class PollAllFlagMasterDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Get Particullar Record from the PollAllFlagMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public PollAllFlagMasterDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<PollAllFlagMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM PollAllFlagMaster_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new PollAllFlagMasterDTO
                        {
                            ID = u.ID,
                            IsPollALL = u.IsPollALL,
                            RoomID = u.RoomID,
                            CompanyID = u.CompanyID,
                            CreatedBy = u.CreatedBy,
                            CreatedOn = u.CreatedOn,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            LastUpdatedOn = u.LastUpdatedOn,
                            LastUpdatedBy = u.LastUpdatedBy,
                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Get Paged Records from the PollAllFlagMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<PollAllFlagMasterDTO> GetAllRecords(Int64 CompanyId)
        {
            return GetCachedData(CompanyId).OrderBy("ID DESC");
        }

        /// <summary>
        /// Get Paged Records from the PollAllFlagMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<PollAllFlagMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<PollAllFlagMasterDTO> ObjCache = GetAllRecords(RoomID, CompanyID);
            IEnumerable<PollAllFlagMasterDTO> ObjGlobalCache = ObjCache;
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
                //IEnumerable<PollAllFlagMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.CreatedOn.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.CreatedOn.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.LastUpdatedOn.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdatedOn.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

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
                        strQuery += "UPDATE PollAllFlagMaster SET LastUpdatedOn = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                ////Get Cached-Media
                //IEnumerable<PollAllFlagMasterDTO> ObjCache = CacheHelper<IEnumerable<PollAllFlagMasterDTO>>.GetCacheItem("Cached_PollAllFlagMaster_" + CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<PollAllFlagMasterDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                //    ObjCache = objTemp.AsEnumerable();
                //    CacheHelper<IEnumerable<PollAllFlagMasterDTO>>.AppendToCacheItem("Cached_PollAllFlagMaster_" + CompanyID.ToString(), ObjCache);
                //}

                return true;
            }
        }

        /// <summary>
        /// If no data is received within 10 minutes of when a time Poll should have occurred, 
        /// then send a message: "eVMI Poll All not received. Communication is interrupted."
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PollAllFlagMasterDTO> eVMIPollAllNotReceived(Int64 CompanyId)
        {
            string connstr = base.DataBaseEntityConnectionString;
            IEnumerable<PollAllFlagMasterDTO> polls;
            try
            {
                polls = GetCachedData(CompanyId).Where(x => x.CreatedOn.AddMinutes(16) < DateTimeUtility.DateTimeNow && !x.IsDeleted && !x.IsArchived && x.IsPollALL);
                return polls;
            }
            finally
            {
                polls = null;
            }

        }
    }
}
