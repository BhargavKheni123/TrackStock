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
using System.Web;
using eTurns.DTO.Resources;

namespace eTurns.DAL
{
    public partial class CustomerMasterDAL : eTurnsBaseDAL
    {
        public IEnumerable<CustomerMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {

            IEnumerable<CustomerMasterDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                //Get Cached-Media
                ObjCache = CacheHelper<IEnumerable<CustomerMasterDTO>>.GetCacheItem("Cached_CustomerMaster_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<CustomerMasterDTO> obj = (from u in context.Database.SqlQuery<CustomerMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' As CreatedDate,'' AS UpdatedDate FROM CustomerMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                              select new CustomerMasterDTO
                                                              {
                                                                  Customer = u.Customer,
                                                                  Account = u.Account,
                                                                  Address = u.Address,
                                                                  City = u.City,
                                                                  State = u.State,
                                                                  Country = u.Country,
                                                                  ZipCode = u.ZipCode,
                                                                  Contact = u.Contact,
                                                                  Email = u.Email,
                                                                  Phone = u.Phone,
                                                                  Created = u.Created,
                                                                  CreatedBy = u.CreatedBy,
                                                                  ID = u.ID,
                                                                  LastUpdatedBy = u.LastUpdatedBy,
                                                                  Room = u.Room,
                                                                  Updated = u.Updated,
                                                                  CreatedByName = u.CreatedByName,
                                                                  UpdatedByName = u.UpdatedByName,
                                                                  RoomName = u.RoomName,
                                                                  CompanyID = u.CompanyID,
                                                                  IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                  IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                  GUID = u.GUID,
                                                                  UDF1 = u.UDF1,
                                                                  UDF2 = u.UDF2,
                                                                  UDF3 = u.UDF3,
                                                                  UDF4 = u.UDF4,
                                                                  UDF5 = u.UDF5,
                                                                  CreatedDate = u.CreatedDate,
                                                                  UpdatedDate = u.UpdatedDate,
                                                                  Remarks = u.Remarks,
                                                                  AddedFrom = u.AddedFrom,
                                                                  EditedFrom = u.EditedFrom,
                                                                  ReceivedOn = u.ReceivedOn,
                                                                  ReceivedOnWeb = u.ReceivedOnWeb
                                                              }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<CustomerMasterDTO>>.AddCacheItem("Cached_CustomerMaster_" + CompanyID.ToString(), obj);
                    }
                }
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
                    ObjCache = (from u in context.Database.SqlQuery<CustomerMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDAte,'' As UpdatedDate FROM CustomerMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + " AND " + sSQL)
                                select new CustomerMasterDTO
                                {
                                    Customer = u.Customer,
                                    Account = u.Account,
                                    Address = u.Address,
                                    City = u.City,
                                    State = u.State,
                                    Country = u.Country,
                                    ZipCode = u.ZipCode,
                                    Contact = u.Contact,
                                    Email = u.Email,
                                    Phone = u.Phone,
                                    Created = u.Created,
                                    CreatedBy = u.CreatedBy,
                                    ID = u.ID,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    Updated = u.Updated,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    CompanyID = u.CompanyID,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    GUID = u.GUID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    CreatedDate = u.CreatedDate,
                                    UpdatedDate = u.UpdatedDate,
                                    Remarks = u.Remarks,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb

                                }).AsParallel().ToList();
                }
            }

            return ObjCache.Where(t => t.Room == RoomID);
        }
        public IEnumerable<CustomerMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string sSQL = "A.IsDeleted = 0 AND A.IsArchived = 0";
                if (IsArchived && IsDeleted)
                {
                    sSQL = "A.IsDeleted = 1 AND A.IsArchived = 1";
                }
                else if (IsArchived)
                {
                    sSQL = "A.IsArchived = 1";
                }
                else if (IsDeleted)
                {
                    sSQL = "A.IsDeleted =1";
                }

                IEnumerable<CustomerMasterDTO> lstCustomerMaster = (from u in context.Database.SqlQuery<CustomerMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' As CreatedDate,'' AS UpdatedDate 
                                                                            FROM CustomerMaster A 
                                                                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                                            WHERE A.CompanyID = " + CompanyId.ToString() +
                                                                                  "AND A.Room = " + RoomID.ToString() +
                                                                                  (sSQL == "" ? "" : " AND " + sSQL))
                                                                    select new CustomerMasterDTO
                                                                    {
                                                                        Customer = u.Customer,
                                                                        Account = u.Account,
                                                                        Address = u.Address,
                                                                        City = u.City,
                                                                        State = u.State,
                                                                        Country = u.Country,
                                                                        ZipCode = u.ZipCode,
                                                                        Contact = u.Contact,
                                                                        Email = u.Email,
                                                                        Phone = u.Phone,
                                                                        Created = u.Created,
                                                                        CreatedBy = u.CreatedBy,
                                                                        ID = u.ID,
                                                                        LastUpdatedBy = u.LastUpdatedBy,
                                                                        Room = u.Room,
                                                                        Updated = u.Updated,
                                                                        CreatedByName = u.CreatedByName,
                                                                        UpdatedByName = u.UpdatedByName,
                                                                        RoomName = u.RoomName,
                                                                        CompanyID = u.CompanyID,
                                                                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                        GUID = u.GUID,
                                                                        UDF1 = u.UDF1,
                                                                        UDF2 = u.UDF2,
                                                                        UDF3 = u.UDF3,
                                                                        UDF4 = u.UDF4,
                                                                        UDF5 = u.UDF5,
                                                                        CreatedDate = u.CreatedDate,
                                                                        UpdatedDate = u.UpdatedDate,
                                                                        Remarks = u.Remarks,
                                                                        AddedFrom = u.AddedFrom,
                                                                        EditedFrom = u.EditedFrom,
                                                                        ReceivedOn = u.ReceivedOn,
                                                                        ReceivedOnWeb = u.ReceivedOnWeb
                                                                    }).AsParallel().ToList();

                return lstCustomerMaster;
            }
        }
        public IEnumerable<CustomerMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            //IEnumerable<CustomerMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted);
            IEnumerable<CustomerMasterDTO> ObjCache = GetCustomersByRoomID(RoomID, CompanyId);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<CustomerMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<CustomerMasterDTO> ObjCache = GetCachedData(RoomID,CompanyId);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                //CreatedBy,UpdatedBy,DateCreatedFrom,DateUpdatedFrom,UDF1,UDF2,UDF3,UDF4,UDF5,[###]admin,niraj$$$$$$$test2$$
                // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
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
                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Created.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Updated.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    && ((Fields[1].Split('@')[9] == "") || (Fields[1].Split('@')[9].Split(',').ToList().Contains(t.Remarks)))
                    && (SearchTerm == "" || (t.ID.ToString().Contains(SearchTerm) ||
                        t.Customer.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Account ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.City ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Country ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Contact ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Address ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Email ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Remarks ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<CustomerMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.Customer.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Account ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.City ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Country ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Contact ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Address ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Email ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Remarks ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.Customer.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Account ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.City ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Country ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Contact ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Address ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Email ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Remarks ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }


            #region Previous Code


            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{

            //    string strSortinitializer = "";

            //    if (sortColumnName.ToUpper().Contains("CREATEDBYNAME"))
            //    {
            //        strSortinitializer = "B";
            //        sortColumnName = "UserName";
            //    }
            //    else if (sortColumnName.ToUpper().Contains("UPDATEDBYNAME"))
            //    {
            //        strSortinitializer = "C";
            //        sortColumnName = "UserName";
            //    }
            //    else if (sortColumnName.ToUpper().Contains("ROOMNAME"))
            //    {
            //        strSortinitializer = "D";
            //    }
            //    else
            //    {
            //        strSortinitializer = "A";
            //    }

            //    strSortinitializer = strSortinitializer + "." + sortColumnName;

            //    if (String.IsNullOrEmpty(SearchTerm))
            //    {
            //        TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM CustomerMaster WHERE IsDeleted!=1 AND IsArchived != 1 AND CompanyID = " + CompanyId.ToString() + @" AND RoomID = " + RoomID.ToString()).ToList()[0]);

            //        return (from u in context.Database.SqlQuery<CustomerMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName , D.RoomName FROM CustomerMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.RoomID = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1  AND A.CompanyID = " + CompanyId.ToString() + @" AND A.RoomID = " + RoomID.ToString() + @" ORDER BY " + strSortinitializer)
            //                select new CustomerMasterDTO
            //                {
            //                    Customer = u.Customer,
            //                    Account = u.Account,
            //                    Address = u.Address,
            //                    City = u.City,
            //                    State = u.State,
            //                    Country = u.Country,
            //                    ZipCode = u.ZipCode,
            //                    Contact = u.Contact,
            //                    Email = u.Email,
            //                    Phone = u.Phone,
            //                    Created = u.Created,
            //                    CreatedBy = u.CreatedBy,
            //                    ID = u.ID,
            //                    LastUpdatedBy = u.LastUpdatedBy,
            //                    RoomID = u.RoomID,
            //                    Updated = u.Updated,
            //                    CreatedByName = u.CreatedByName,
            //                    UpdatedByName = u.UpdatedByName,
            //                    RoomName = u.RoomName,
            //                    UDF1 = u.UDF1,
            //                    UDF2 = u.UDF2,
            //                    UDF3 = u.UDF3,
            //                    UDF4 = u.UDF4,
            //                    UDF5 = u.UDF5
            //                }).Skip(StartRowIndex).Take(MaxRows).ToList();
            //    }

            //    else if (SearchTerm.Contains("STARTWITH#"))
            //    {
            //        string search = "";
            //        string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split('$');
            //        string[] stringSeparators = new string[] { "[###]" };

            //        if (dd != null && dd.Length > 0)
            //        {
            //            string[] Fields = dd[1].Split(stringSeparators, StringSplitOptions.None);
            //            if (Fields != null && Fields.Length > 0)
            //            {
            //                // 6 counts for fields based on that prepare the search string
            //                // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
            //                foreach (var item in Fields)
            //                {
            //                    if (item.Length > 0)
            //                    {
            //                        if (item.Contains("CreatedBy"))
            //                        {
            //                            search += " A.CreatedBy in (" + item.Split('#')[1] + ")";
            //                        }
            //                        if (item.Contains("UpdatedBy"))
            //                        {
            //                            if (search.Length > 0)
            //                                search += " AND ";
            //                            search += " A.LastUpdatedBy in (" + item.Split('#')[1] + ")";
            //                        }
            //                        if (item.Contains("DateCreatedFrom"))
            //                        {
            //                            if (search.Length > 0)
            //                                search += " AND ";
            //                            string[] CreatedDateFrom = item.Split('#');
            //                            search += " (A.Created >= DATETIME('" + CreatedDateFrom[1] + "') AND A.Created <= DATETIME('" + CreatedDateFrom[3] + "'))";
            //                        }
            //                        if (item.Contains("DateUpdatedFrom"))
            //                        {
            //                            if (search.Length > 0)
            //                                search += " AND ";
            //                            string[] UpdatedDateFrom = item.Split('#');
            //                            search += " (A.Updated >= DATETIME('" + UpdatedDateFrom[1] + "') AND A.Updated <= DATETIME('" + UpdatedDateFrom[3] + "'))";
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //        if (search.Length > 0)
            //        {
            //            search = " AND (" + search + " )";
            //        }
            //        TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(A.ID) FROM CustomerMaster as A WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyId.ToString() + @" AND A.RoomID = " + RoomID.ToString() + @" AND A.IsArchived != 1 " + search + "").ToList()[0]);

            //        return (from u in context.Database.SqlQuery<CustomerMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName , D.RoomName FROM CustomerMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.RoomID = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyId.ToString() + @" AND A.RoomID = " + RoomID.ToString() + @" AND A.IsArchived != 1 " + search + " ORDER BY " + strSortinitializer)
            //                select new CustomerMasterDTO
            //                {
            //                    Customer = u.Customer,
            //                    Account = u.Account,
            //                    Address = u.Address,
            //                    City = u.City,
            //                    State = u.State,
            //                    Country = u.Country,
            //                    ZipCode = u.ZipCode,
            //                    Contact = u.Contact,
            //                    Email = u.Email,
            //                    Phone = u.Phone,
            //                    Created = u.Created,
            //                    CreatedBy = u.CreatedBy,
            //                    ID = u.ID,
            //                    LastUpdatedBy = u.LastUpdatedBy,
            //                    RoomID = u.RoomID,
            //                    Updated = u.Updated,
            //                    UDF1 = u.UDF1,
            //                    UDF2 = u.UDF2,
            //                    UDF3 = u.UDF3,
            //                    UDF4 = u.UDF4,
            //                    UDF5 = u.UDF5
            //                }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
            //    }
            //    else
            //    {
            //        TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM CustomerMaster WHERE IsDeleted!=1  AND CompanyID = " + CompanyId.ToString() + @" AND RoomID =" + RoomID.ToString() + @" AND IsArchived != 1 AND ((Customer like '%" + SearchTerm + "%') OR (ID like '%" + SearchTerm + "%')) ").ToList()[0]);

            //        return (from u in context.Database.SqlQuery<CustomerMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName , D.RoomName FROM CustomerMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.RoomID = D.ID WHERE A.IsDeleted!=1 AND AND A.CompanyID = " + CompanyId.ToString() + @" AND A.RoomID =" + RoomID.ToString() + @" A.IsArchived != 1 AND ((A.Customer like '%" + SearchTerm + "%') OR (A.ID like '%" + SearchTerm + "%')) ORDER BY " + strSortinitializer)
            //                select new CustomerMasterDTO
            //                {
            //                    Customer = u.Customer,
            //                    Account = u.Account,
            //                    Address = u.Address,
            //                    City = u.City,
            //                    State = u.State,
            //                    Country = u.Country,
            //                    ZipCode = u.ZipCode,
            //                    Contact = u.Contact,
            //                    Email = u.Email,
            //                    Phone = u.Phone,
            //                    Created = u.Created,
            //                    CreatedBy = u.CreatedBy,
            //                    ID = u.ID,
            //                    LastUpdatedBy = u.LastUpdatedBy,
            //                    RoomID = u.RoomID,
            //                    Updated = u.Updated,
            //                    UDF1 = u.UDF1,
            //                    UDF2 = u.UDF2,
            //                    UDF3 = u.UDF3,
            //                    UDF4 = u.UDF4,
            //                    UDF5 = u.UDF5
            //                }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
            //    }
            //} 
            #endregion
        }
        public CustomerMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).SingleOrDefault(t => t.ID == id);
        }
        public CustomerMasterDTO GetRecordByGUID(Guid CustGUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).SingleOrDefault(t => t.GUID == CustGUID);
        }
        public bool Delete(Int64 id, Int64 userID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CustomerMaster obj = context.CustomerMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userID;
                obj.IsArchived = false;
                obj.EditedFrom = "Web";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                context.CustomerMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<CustomerMasterDTO> ObjCache = CacheHelper<IEnumerable<CustomerMasterDTO>>.GetCacheItem("Cached_CustomerMaster_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<CustomerMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<CustomerMasterDTO>>.AppendToCacheItem("Cached_CustomerMaster_" + obj.CompanyID.ToString(), ObjCache);
                }
                return true;

            }
        }
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        strQuery += "UPDATE CustomerMaster SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);

                //Get Cached-Media
                IEnumerable<CustomerMasterDTO> ObjCache = CacheHelper<IEnumerable<CustomerMasterDTO>>.GetCacheItem("Cached_CustomerMaster_" + CompanyId.ToString());
                if (ObjCache != null)
                {
                    List<CustomerMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<CustomerMasterDTO>>.AppendToCacheItem("Cached_CustomerMaster_" + CompanyId.ToString(), ObjCache);
                }
                return true;
            }
        }

        //public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {

        //        string strQuery = "UPDATE CustomerMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "'),EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID= '" + id + "'";
        //        context.Database.ExecuteSqlCommand(strQuery);
        //    }

        //    return value;
        //}
        //public CustomerMasterDTO GetCustomerByName(string CustomerName, long RoomID, long CompanyID)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        return context.CustomerMasters.Where(t => t.Room == RoomID && t.CompanyID == CompanyID && (t.IsDeleted ?? false) == false && t.Customer.Trim().ToUpper() == CustomerName.Trim().ToUpper()).Select(u => new CustomerMasterDTO()
        //        {
        //            Customer = u.Customer,
        //            Account = u.Account,
        //            Address = u.Address,
        //            City = u.City,
        //            State = u.State,
        //            Country = u.Country,
        //            ZipCode = u.ZipCode,
        //            Contact = u.Contact,
        //            Email = u.Email,
        //            Phone = u.Phone,
        //            Created = u.Created,
        //            CreatedBy = u.CreatedBy,
        //            ID = u.ID,
        //            LastUpdatedBy = u.LastUpdatedBy,
        //            Room = u.Room,
        //            Updated = u.Updated,
        //            CompanyID = u.CompanyID,
        //            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
        //            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
        //            GUID = u.GUID,
        //            UDF1 = u.UDF1,
        //            UDF2 = u.UDF2,
        //            UDF3 = u.UDF3,
        //            UDF4 = u.UDF4,
        //            UDF5 = u.UDF5,
        //            Remarks = u.Remarks,
        //            AddedFrom = u.AddedFrom,
        //            EditedFrom = u.EditedFrom,
        //            ReceivedOn = u.ReceivedOn,
        //            ReceivedOnWeb = u.ReceivedOnWeb
        //        }).FirstOrDefault();
        //    }

        //}
    }
}
