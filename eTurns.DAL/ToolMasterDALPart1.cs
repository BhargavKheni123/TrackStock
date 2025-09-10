using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;

using System.Data.Objects;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using eTurns.DTO.Resources;
using System.Web;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class ToolMasterDAL : eTurnsBaseDAL
    {

        public IEnumerable<RequisitionMasterNarrowSearchDTO> GetAllNarrowSearchRecordsForHistory(string TextFieldName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@Isdeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                return context.ExecuteStoreQuery<RequisitionMasterNarrowSearchDTO>("exec [GetToolBinNarrowSearch_History] @TextFieldName,@RoomId,@CompanyId,@Isdeleted,@IsArchived", params1).ToList();
            }
        }

        public IEnumerable<ToolMasterDTO> GetCachedDataNew(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<ToolMasterDTO> ObjCache;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchieved", IsArchived) };
                ObjCache = context.ExecuteStoreQuery<ToolMasterDTO>("exec GetToolList @RoomId,@CompanyId,@IsDeleted,@IsArchieved", params1).ToList();
            }
            return ObjCache;
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
        public IEnumerable<ToolMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return GetCachedDataNew(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        }



        public IEnumerable<RequisitionMasterNarrowSearchDTO> GetAllNarrowSearchCheckOutUDFRecords(string TextFieldName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ExcludeToolGuid, string Type, string ToolCurrentTab)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@Isdeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ExcludeToolGuid", ExcludeToolGuid), new SqlParameter("@Type", Type), new SqlParameter("@ToolCurrentTab", ToolCurrentTab) };
                return context.ExecuteStoreQuery<RequisitionMasterNarrowSearchDTO>("exec [GetToolNarrowSearchToolCheckOutUDF] @TextFieldName,@RoomId,@CompanyId,@Isdeleted,@IsArchived,@ExcludeToolGuid,@Type,@ToolCurrentTab", params1).ToList();
            }
        }

        public IEnumerable<RequisitionMasterNarrowSearchDTO> GetAllNarrowSearchRecordsForUDF(string TextFieldName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ExcludeToolGuid, string Type, bool NotIncludeDeletedUDF)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@Isdeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ExcludeToolGuid", ExcludeToolGuid), new SqlParameter("@Type", Type), new SqlParameter("@NotIncludeDeletedUDF", NotIncludeDeletedUDF) };
                return context.ExecuteStoreQuery<RequisitionMasterNarrowSearchDTO>("exec [GetToolNarrowSearchUDF] @TextFieldName,@RoomId,@CompanyId,@Isdeleted,@IsArchived,@ExcludeToolGuid,@Type,@NotIncludeDeletedUDF", params1).ToList();
            }
        }

        public IEnumerable<RequisitionMasterNarrowSearchDTO> GetAllNarrowSearchRecords(string TextFieldName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ExcludeToolGuid, string Type)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@Isdeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ExcludeToolGuid", ExcludeToolGuid), new SqlParameter("@Type", Type) };
                return context.ExecuteStoreQuery<RequisitionMasterNarrowSearchDTO>("exec [GetToolBinNarrowSearch] @TextFieldName,@RoomId,@CompanyId,@Isdeleted,@IsArchived,@ExcludeToolGuid,@Type", params1).ToList();
            }
        }


        public IEnumerable<ToolMasterDTO> GetCachedData_New(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<ToolMasterDTO> ObjCache;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchieved", IsArchived) };
                ObjCache = context.ExecuteStoreQuery<ToolMasterDTO>("exec GetToolListNew @RoomId,@CompanyId,@IsDeleted,@IsArchieved", params1).ToList();
            }
            return ObjCache;
        }


        public IEnumerable<ToolMasterDTO> GetSerialUsingToolAutoComplete(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ToolName, string NameStartsWith)
        {
            List<int> ScheduleTypes = new List<int>();
            IEnumerable<ToolMasterDTO> IEToolsSchedulerDTO = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (!string.IsNullOrWhiteSpace(NameStartsWith))
                {
                    IEToolsSchedulerDTO = (from u in context.ToolMasters
                                           where u.Serial.Contains(NameStartsWith) && u.ToolName == ToolName && u.Room == RoomID && u.CompanyID == CompanyID && (u.IsDeleted ?? false) == false && (u.IsArchived ?? false) == false
                                           select new ToolMasterDTO
                                           {
                                               Serial = u.Serial,
                                               ID = u.ID,
                                               GUID = u.GUID,

                                           }).AsParallel().ToList();
                }
                else
                {
                    IEToolsSchedulerDTO = (from u in context.ToolMasters
                                           where u.ToolName == ToolName && u.Room == RoomID && u.CompanyID == CompanyID && (u.IsDeleted ?? false) == false && (u.IsArchived ?? false) == false
                                           select new ToolMasterDTO
                                           {
                                               Serial = u.Serial,
                                               ID = u.ID,
                                               GUID = u.GUID,

                                           }).AsParallel().ToList();
                }
            }
            return IEToolsSchedulerDTO;
        }

        /// <summary>
        /// Get Tools Records fot the passed parameter collections
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ToolMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string RoomDateFormat)
        {
            IEnumerable<ToolMasterDTO> ObjCache = GetCachedDataNew(RoomID, CompanyId, IsArchived, IsDeleted);
            string newSearchValue = string.Empty;

            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<ToolMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //WI-1461 related changes 
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        newSearchValue = Fields[2];
                    else
                        newSearchValue = string.Empty;
                }
                else
                {
                    newSearchValue = string.Empty;
                }

                if (SearchTerm.Contains("1000_1000"))
                {

                    ObjCache = ObjCache.Where(t =>
                           ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                        && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                        && ((Fields[1].Split('@')[2] == "") || (t.Created >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Created <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                        && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Updated.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                        && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                        && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                        && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                        && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                        && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                        && ((Fields[1].Split('@')[27] == "") || (Fields[1].Split('@')[27].Split(',').ToList().Contains(t.LocationID.ToString())))
                        //&& ((Fields[1].Split('@')[28] == "") || (Fields[1].Split('@')[28].Split(',').ToList().Contains(t.WorkOrderID.ToString())))
                        && ((Fields[1].Split('@')[29] == "") || (Fields[1].Split('@')[29].Split(',').ToList().Contains(t.ToolCategoryID.ToString())))
                        //&& ((Fields[1].Split('@')[30] == "") || (Fields[1].Split('@')[30].Split(',').ToList().Contains(t.ToolMaintanenceID.ToString())))
                        && ((Fields[1].Split('@')[31] == "") || (t.Cost >= Convert.ToDouble(Fields[1].Split('@')[31].Split('_')[0])))
                        );

                    ObjCache = ObjCache.Where
                     (t => t.ID.ToString().Contains(newSearchValue) ||
                         (t.Description ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.Location ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.Serial ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.ToolCategory ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.ToolName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF1 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF2 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF3 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF4 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF5 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.AppendedBarcodeString ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0
                     );

                    TotalCount = ObjCache.Count();
                    return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
                }
                else
                {
                    ObjCache = ObjCache.Where(t =>
                                              ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                                           && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                                           && ((Fields[1].Split('@')[2] == "") || (t.Created >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Created <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                                           && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Updated.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                                           && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                                           && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                                           && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                                           && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                                           && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                                           && ((Fields[1].Split('@')[27] == "") || (Fields[1].Split('@')[27].Split(',').ToList().Contains(t.LocationID.ToString())))
                                            //&& ((Fields[1].Split('@')[28] == "") || (Fields[1].Split('@')[28].Split(',').ToList().Contains(t.WorkOrderID.ToString())))
                                            && ((Fields[1].Split('@')[29] == "") || (Fields[1].Split('@')[29].Split(',').ToList().Contains(t.ToolCategoryID.ToString())))
                                            //&& ((Fields[1].Split('@')[30] == "") || (Fields[1].Split('@')[30].Split(',').ToList().Contains(t.ToolMaintanenceID.ToString())))
                                            && ((Fields[1].Split('@')[31] == "") || (t.Cost >= Convert.ToDouble(Fields[1].Split('@')[31].Split('_')[0]) && t.Cost <= Convert.ToDouble(Fields[1].Split('@')[31].Split('_')[1])))
                                           );

                    ObjCache = ObjCache.Where
                     (t => t.ID.ToString().Contains(newSearchValue) ||
                         (t.Description ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.Location ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.Serial ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.ToolCategory ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.ToolName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF1 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF2 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF3 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF4 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF5 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.AppendedBarcodeString ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0
                     );

                    TotalCount = ObjCache.Count();
                    return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
                }
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<ToolMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Location ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Serial ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ToolCategory ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ToolName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Location ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Serial ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ToolCategory ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ToolName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
            #region Previous Code

            //            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //            {
            //                string strSortinitializer = "";

            //                if (sortColumnName.ToUpper().Contains("CREATEDBYNAME"))
            //                {
            //                    strSortinitializer = "B";
            //                    sortColumnName = "UserName";
            //                }
            //                else if (sortColumnName.ToUpper().Contains("UPDATEDBYNAME"))
            //                {
            //                    strSortinitializer = "C";
            //                    sortColumnName = "UserName";
            //                }
            //                else if (sortColumnName.ToUpper().Contains("ROOMNAME"))
            //                {
            //                    strSortinitializer = "D";
            //                }
            //                else if (sortColumnName.ToUpper().Contains("TOOLCATEGORY"))
            //                {
            //                    strSortinitializer = "E";
            //                }
            //                else if (sortColumnName.ToUpper().Contains("LOCATION"))
            //                {
            //                    strSortinitializer = "L";
            //                }
            //                else
            //                {
            //                    strSortinitializer = "A";
            //                }

            //                strSortinitializer = strSortinitializer + "." + sortColumnName;


            //                if (String.IsNullOrEmpty(SearchTerm))
            //                {
            //                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM ToolMaster WHERE IsDeleted!=1 AND  CompanyID = " + CompanyId.ToString() + @" AND  RoomID = " + RoomID.ToString() + @" AND IsArchived != 1").ToList()[0]);

            //                                return (from u in context.ExecuteStoreQuery<ToolMasterDTO>(@"SELECT A.ID,A.ToolName, A.Serial, A.Description, A.Cost
            //                                                                                                    ,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5
            //                                                                                                    ,A.IsCheckedOut, A.Created, A.Updated, A.ToolCategoryID
            //                                                                                                    ,A.LocationID
            //                                                                                                    ,B.UserName AS 'CreatedByName'
            //                                                                                                    ,C.UserName AS 'UpdatedByName'
            //                                                                                                    ,D.RoomName, E.ToolCategory 
            //                                                                                                    ,L.Location
            //                                                                                            FROM ToolMaster A 
            //                                                                                                       LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
            //                                                                                                       LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
            //                                                                                                       LEFT OUTER JOIN Room D on A.RoomID = D.ID 
            //                                                                                                       LEFT OUTER JOIN ToolCategoryMaster E on A.ToolCategoryID = E.ID  
            //                                                                                                       LEFT OUTER JOIN LocationMaster L on A.LocationID = L.ID 
            //                                                                                            WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyId.ToString() + @" AND A.RoomID = " + RoomID.ToString() + @" AND A.IsArchived != 1 ORDER BY " + strSortinitializer)
            //                                        select new ToolMasterDTO
            //                                        {
            //                                            ID = u.ID,
            //                                            ToolName = u.ToolName,
            //                                            Serial = u.Serial,
            //                                            Description = u.Description,
            //                                            Cost = u.Cost,
            //                                            IsCheckedOut = u.IsCheckedOut,
            //                                            ToolCategoryID = u.ToolCategoryID,
            //                                            ToolCategory = u.ToolCategory,
            //                                            Created = u.Created,
            //                                            Updated = u.Updated,
            //                                            CreatedByName = u.CreatedByName,
            //                                            UpdatedByName = u.UpdatedByName,
            //                                            RoomID = u.RoomID,
            //                                            RoomName = u.RoomName,
            //                                            Location = u.Location,
            //                                            LocationID = u.LocationID,
            //                                            UDF1 = u.UDF1,
            //                                            UDF2 = u.UDF2,
            //                                            UDF3 = u.UDF3,
            //                                            UDF4 = u.UDF4,
            //                                            UDF5 = u.UDF5
            //                                        }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
            //                }
            //                else
            //                {
            //                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM ToolMaster WHERE IsDeleted!=1 AND  CompanyID = " + CompanyId.ToString() + @" AND  RoomID = " + RoomID.ToString() + @" AND IsArchived != 1 AND ((ToolName like '%" + SearchTerm + "%') OR (Serial like '%" + SearchTerm + "%') OR (Description like '%" + SearchTerm + "%') OR (ToolValue like '%" + SearchTerm + "%')  OR (ID like '%" + SearchTerm + "%'))").ToList()[0]);

            //                    return (from u in context.ExecuteStoreQuery<ToolMasterDTO>(@"SELECT A.ID,A.ToolName, A.Serial, A.Description, A.Cost
            //                                                                                        ,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5
            //                                                                                        ,A.IsCheckedOut, A.Created, A.Updated, A.ToolCategoryID
            //                                                                                        ,A.LocationID
            //                                                                                        ,B.UserName AS 'CreatedByName'
            //                                                                                        ,C.UserName AS 'UpdatedByName'
            //                                                                                        ,D.RoomName, E.ToolCategory 
            //                                                                                        ,L.Location
            //                                                                                 FROM ToolMaster A 
            //                                                                                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
            //                                                                                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
            //                                                                                            LEFT OUTER JOIN Room D on A.RoomID = D.ID 
            //                                                                                            LEFT OUTER JOIN ToolCategoryMaster E on A.ToolCategoryID = E.ID 
            //                                                                                            LEFT OUTER JOIN LocationMaster L on A.LocationID = L.ID 
            //                                                                                WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyId.ToString() + @" AND A.RoomID = " + RoomID.ToString() + @" AND A.IsArchived != 1 
            //                                                                                  AND ((A.ToolName like '%" + SearchTerm + @"%') OR 
            //                                                                                       (Serial like '%" + SearchTerm + @"%') OR 
            //                                                                                       (Description like '%" + SearchTerm + @"%') OR 
            //                                                                                       (ToolValue like '%" + SearchTerm + @"%') OR 
            //                                                                                       (A.ID like '%" + SearchTerm + @"%')) 
            //                                                                                ORDER BY " + strSortinitializer)
            //                            select new ToolMasterDTO
            //                            {
            //                                ID = u.ID,
            //                                ToolName = u.ToolName,
            //                                Serial = u.Serial,
            //                                Description = u.Description,
            //                                Cost = u.Cost,
            //                                IsCheckedOut = u.IsCheckedOut,
            //                                ToolCategoryID = u.ToolCategoryID,
            //                                ToolCategory = u.ToolCategory,
            //                                Created = u.Created,
            //                                Updated = u.Updated,
            //                                CreatedByName = u.CreatedByName,
            //                                UpdatedByName = u.UpdatedByName,
            //                                RoomID = u.RoomID,
            //                                RoomName = u.RoomName,
            //                                Location = u.Location,
            //                                LocationID = u.LocationID,
            //                                UDF1 = u.UDF1,
            //                                UDF2 = u.UDF2,
            //                                UDF3 = u.UDF3,
            //                                UDF4 = u.UDF4,
            //                                UDF5 = u.UDF5
            //                            }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
            //                }
            //            } 
            #endregion
        }

        public IEnumerable<RequisitionMasterNarrowSearchDTO> GetAllNarrowSearchRecordsNew(string TextFieldName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ExcludeToolGuid, string Type)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@Isdeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ExcludeToolGuid", ExcludeToolGuid), new SqlParameter("@Type", Type) };
                return context.ExecuteStoreQuery<RequisitionMasterNarrowSearchDTO>("exec [GetToolBinNarrowSearch] @TextFieldName,@RoomId,@CompanyId,@Isdeleted,@IsArchived,@ExcludeToolGuid,@Type", params1).ToList();
            }
        }
        public IEnumerable<RequisitionMasterNarrowSearchDTO> GetAllNarrowSearchRecordsForUDFNew(string TextFieldName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ExcludeToolGuid, string Type, bool NotIncludeDeletedUDF)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@Isdeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ExcludeToolGuid", ExcludeToolGuid), new SqlParameter("@Type", Type), new SqlParameter("@NotIncludeDeletedUDF", NotIncludeDeletedUDF) };
                return context.ExecuteStoreQuery<RequisitionMasterNarrowSearchDTO>("exec [GetToolNarrowSearchUDFNew] @TextFieldName,@RoomId,@CompanyId,@Isdeleted,@IsArchived,@ExcludeToolGuid,@Type,@NotIncludeDeletedUDF", params1).ToList();
            }
        }
        public IEnumerable<RequisitionMasterNarrowSearchDTO> GetAllNarrowSearchCheckOutUDFRecordsNew(string TextFieldName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ExcludeToolGuid, string Type)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@Isdeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ExcludeToolGuid", ExcludeToolGuid), new SqlParameter("@Type", Type) };
                return context.ExecuteStoreQuery<RequisitionMasterNarrowSearchDTO>("exec [GetToolNarrowSearchToolCheckOutUDFNew] @TextFieldName,@RoomId,@CompanyId,@Isdeleted,@IsArchived,@ExcludeToolGuid,@Type", params1).ToList();
            }
        }


        //public ToolMasterDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        //{
        //    ToolMasterDTO ObjDTO = GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.GUID == GUID).SingleOrDefault();
        //    if (ObjDTO != null && ObjDTO.ID > 0)
        //        return ObjDTO;
        //    else
        //        return new ToolMasterDTO();
        //}



        //public List<ToolMasterDTO> GetPagedToolsForService(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string ToolsIDs, string ToolGUIDs, string ExcludeToolGuids = "", string Type = "1")
        //{
        //    List<ToolMasterDTO> lstTools = new List<ToolMasterDTO>();
        //    TotalCount = 0;
        //    ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
        //    DataSet dsTools = new DataSet();

        //    //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
        //    //if (string.IsNullOrWhiteSpace(Connectionstring))
        //    //{
        //    //    Connectionstring = base.DataBaseConnectionString;
        //    //}
        //    //if (Connectionstring == "")
        //    //{
        //    //    return lstTools;
        //    //}

        //    SqlConnection EturnsConnection = new SqlConnection(base.DataBaseConnectionString);
        //    string ToolLocs = null;
        //    string ToolCats = null;
        //    string ToolCost = null;
        //    string ToolCreaters = null;
        //    string ToolUpdators = null;
        //    string CreatedDateFrom = null;
        //    string CreatedDateTo = null;
        //    string UpdatedDateFrom = null;
        //    string UpdatedDateTo = null;
        //    string UDF1 = null;
        //    string UDF2 = null;
        //    string UDF3 = null;
        //    string UDF4 = null;
        //    string UDF5 = null;
        //    string ToolCheckOutUDF1 = null;
        //    string ToolCheckOutUDF2 = null;
        //    string ToolCheckOutUDF3 = null;
        //    string ToolCheckOutUDF4 = null;
        //    string ToolCheckOutUDF5 = null;
        //    string ToolMaintence = null;
        //    string TechnicianList = null;

        //    if (String.IsNullOrEmpty(SearchTerm))
        //    {
        //        dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedTools", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, Type);
        //    }
        //    else if (SearchTerm.Contains("[###]"))
        //    {
        //        string[] stringSeparators = new string[] { "[###]" };
        //        string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
        //        string[] FieldsPara = Fields[1].Split('@');
        //        if (Fields.Length > 2)
        //        {
        //            if (!string.IsNullOrEmpty(Fields[2]))
        //            {
        //                SearchTerm = Fields[2];
        //            }
        //            else
        //            {
        //                SearchTerm = string.Empty;
        //            }
        //        }
        //        else
        //        {
        //            SearchTerm = string.Empty;
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
        //        {
        //            ToolCreaters = FieldsPara[0].TrimEnd(',');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
        //        {
        //            ToolUpdators = FieldsPara[1].TrimEnd(',');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
        //        {
        //            CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
        //            CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
        //        {
        //            UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
        //            UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))).ToString("dd-MM-yyyy HH:mm:ss");
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[4].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                UDF1 = UDF1 + supitem + "','";
        //            }
        //            UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');

        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[5].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                UDF2 = UDF2 + supitem + "','";
        //            }
        //            UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[6].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                UDF3 = UDF3 + supitem + "','";
        //            }
        //            UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[7].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                UDF4 = UDF4 + supitem + "','";
        //            }
        //            UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[8].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                UDF5 = UDF5 + supitem + "','";
        //            }
        //            UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[71]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[71].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                ToolCheckOutUDF1 = ToolCheckOutUDF1 + supitem + "','";
        //            }
        //            ToolCheckOutUDF1 = ToolCheckOutUDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');

        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[72]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[72].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                ToolCheckOutUDF2 = ToolCheckOutUDF2 + supitem + "','";
        //            }
        //            ToolCheckOutUDF2 = ToolCheckOutUDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[73]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[73].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                ToolCheckOutUDF3 = ToolCheckOutUDF3 + supitem + "','";
        //            }
        //            ToolCheckOutUDF3 = ToolCheckOutUDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[74]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[74].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                ToolCheckOutUDF4 = ToolCheckOutUDF4 + supitem + "','";
        //            }
        //            ToolCheckOutUDF4 = ToolCheckOutUDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[75]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[75].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                ToolCheckOutUDF5 = ToolCheckOutUDF5 + supitem + "','";
        //            }
        //            ToolCheckOutUDF5 = ToolCheckOutUDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }

        //        if (!string.IsNullOrWhiteSpace(FieldsPara[27]))
        //        {
        //            ToolLocs = FieldsPara[27].TrimEnd(',');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[29]))
        //        {
        //            ToolCats = FieldsPara[29].TrimEnd(',');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[31]))
        //        {
        //            ToolCost = FieldsPara[31].TrimEnd(',');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[101]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[101].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                ToolMaintence = ToolMaintence + supitem + ",";
        //            }
        //            ToolMaintence = ToolMaintence.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }
        //        if (!string.IsNullOrWhiteSpace(FieldsPara[77]))
        //        {
        //            string[] arrReplenishTypes = FieldsPara[77].Split(',');
        //            foreach (string supitem in arrReplenishTypes)
        //            {
        //                TechnicianList = TechnicianList + supitem + "','";
        //            }
        //            TechnicianList = TechnicianList.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
        //        }
        //        //&& ((Fields[1].Split('@')[27] == "") || (Fields[1].Split('@')[27].Split(',').ToList().Contains(t.LocationID.ToString())))
        //        //       && ((Fields[1].Split('@')[29] == "") || (Fields[1].Split('@')[29].Split(',').ToList().Contains(t.ToolCategoryID.ToString())))
        //        //       && ((Fields[1].Split('@')[31] == "") || (t.Cost >= Convert.ToDouble(Fields[1].Split('@')[31].Split('_')[0])))

        //        dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedTools", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, Type);
        //    }
        //    else
        //    {
        //        dsTools = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedTools", StartRowIndex, MaxRows, (SearchTerm ?? string.Empty).Trim(), sortColumnName, ToolLocs, ToolCats, ToolCost, ToolCreaters, ToolUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyId, ToolsIDs, ToolGUIDs, ToolCheckOutUDF1, ToolCheckOutUDF2, ToolCheckOutUDF3, ToolCheckOutUDF4, ToolCheckOutUDF5, ToolMaintence, TechnicianList, ExcludeToolGuids, Type);
        //    }
        //    if (dsTools != null && dsTools.Tables.Count > 0)
        //    {
        //        DataTable dtTools = dsTools.Tables[0];
        //        if (dtTools.Rows.Count > 0)
        //        {
        //            TotalCount = Convert.ToInt32(dtTools.Rows[0]["TotalRecords"]);
        //            lstTools = dtTools.AsEnumerable().Select(row => new ToolMasterDTO()
        //            {
        //                AddedFrom = row.Field<string>("AddedFrom"),
        //                CheckedOutMQTY = row.Field<double?>("CheckedOutMQTY"),
        //                CheckedOutQTY = row.Field<double?>("CheckedOutQTY"),
        //                CompanyID = row.Field<long?>("CompanyID"),
        //                Cost = row.Field<double?>("Cost"),
        //                Created = row.Field<DateTime>("Created"),
        //                CreatedBy = row.Field<long?>("CreatedBy"),
        //                Description = row.Field<string>("Description"),
        //                EditedFrom = row.Field<string>("EditedFrom"),
        //                GUID = row.Field<Guid>("GUID"),
        //                ID = row.Field<long>("ID"),
        //                IsArchived = row.Field<bool?>("IsArchived"),
        //                IsAutoMaintain = row.Field<bool>("IsAutoMaintain"),
        //                IsCheckedOut = row.Field<bool?>("IscheckedOut"),
        //                IsDeleted = row.Field<bool?>("IsDeleted"),
        //                IsGroupOfItems = row.Field<int?>("IsGroupOfItems"),
        //                LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
        //                LocationID = row.Field<long?>("LocationID"),
        //                MaintenanceType = row.Field<int>("MaintenanceType"),
        //                NoOfPastMntsToConsider = row.Field<int?>("NoOfPastMntsToConsider"),
        //                MaintenanceDueNoticeDays = row.Field<int?>("MaintenanceDueNoticeDays"),
        //                Quantity = row.Field<double>("Quantity"),
        //                ReceivedOn = row.Field<DateTime>("ReceivedOn"),
        //                ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
        //                Room = row.Field<long?>("Room"),
        //                Serial = row.Field<string>("Serial"),
        //                ToolCategoryID = row.Field<long?>("ToolCategoryID"),
        //                ToolName = row.Field<string>("ToolName"),
        //                ToolUDF1 = row.Field<string>("UDF1"),
        //                ToolUDF2 = row.Field<string>("UDF2"),
        //                ToolUDF3 = row.Field<string>("UDF3"),
        //                ToolUDF4 = row.Field<string>("UDF4"),
        //                ToolUDF5 = row.Field<string>("UDF5"),
        //                Updated = row.Field<DateTime?>("Updated"),
        //                Location = row.Field<string>("Location"),
        //                ToolCategory = row.Field<string>("ToolCategory"),
        //                CreatedByName = row.Field<string>("CreatedByName"),
        //                UpdatedByName = row.Field<string>("UpdatedByName"),
        //                Technician = row.Field<string>("Technician"),
        //                ImageType = row.Field<string>("ImageType"),
        //                CheckedOutQTYTotal = row.Field<int?>("CheckedOutQTYTotal"),
        //                ImagePath = row.Field<string>("ImagePath"),
        //                ToolImageExternalURL =  row.Field<string>("ToolImageExternalURL"),
        //                Type = row.Field<Int64?>("Type"),
        //                IsBuildBreak = row.Field<bool>("IsBuildBreak"),
        //                AvailableToolQty = row.Field<double?>("AvailableToolQty"),
        //                ToolTypeTracking = row.Field<string>("ToolTypeTracking"),
        //                SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
        //                LotNumberTracking = row.Field<bool>("LotNumberTracking"),
        //                DateCodeTracking = row.Field<bool>("DateCodeTracking"),
        //            }).ToList();

        //        }
        //    }
        //    return lstTools;
        //}

        //public bool ToolCheckOutOrNot(string ToolName, Int64 RoomID, Int64 CompanyID)
        //{
        //    ToolMasterDTO ObjDTO = GetCachedData(RoomID, CompanyID, false, false).Where(t => t.ToolName.ToLower() == ToolName.ToLower()).SingleOrDefault();
        //    bool IsCheckOutTool = false;
        //    if (ObjDTO != null)
        //    {
        //        double TotalQty = 0;
        //        double CheckOutQty = 0;
        //        double CheckOutMainQty = 0;

        //        //if (ObjDTO.Quantity != null)
        //        TotalQty = ObjDTO.Quantity;

        //        if (ObjDTO.CheckedOutQTY != null)
        //            CheckOutQty = (double)ObjDTO.CheckedOutQTY;

        //        if (ObjDTO.CheckedOutMQTY != null)
        //            CheckOutMainQty = (double)ObjDTO.CheckedOutMQTY;

        //        if (CheckOutQty > 0)
        //        {
        //            IsCheckOutTool = true;
        //            return IsCheckOutTool;
        //        }

        //        if (CheckOutMainQty > 0)
        //        {
        //            IsCheckOutTool = true;
        //            return IsCheckOutTool;
        //        }
        //    }
        //    return IsCheckOutTool;
        //}


        public ToolMasterDTO GetToolListByID(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid? ToolGuid, Int64? ToolId)
        {
            ToolMasterDTO ObjCache;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchieved", IsArchived), new SqlParameter("@ToolGuid", ToolGuid ?? (object)DBNull.Value), new SqlParameter("@ToolId", ToolId ?? (object)DBNull.Value) };
                ObjCache = context.ExecuteStoreQuery<ToolMasterDTO>("exec GetToolListByID @RoomId,@CompanyId,@IsDeleted,@IsArchieved,@ToolGuid,@ToolId", params1).FirstOrDefault();

                //BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
                //IList<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID).ToList();

                //ToolCheckInOutHistoryDAL objTOCDAL = new ToolCheckInOutHistoryDAL(base.DataBaseName);
                //IList<ToolCheckInOutHistoryDTO> ObjTOCCache = objTOCDAL.GetAllRecords(RoomID, CompanyID).ToList();


            }
            return ObjCache;
        }

        public ToolMasterDTO GetToolListByIDNew(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid? ToolGuid, Int64? ToolId)
        {
            ToolMasterDTO ObjCache;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchieved", IsArchived), new SqlParameter("@ToolGuid", ToolGuid ?? (object)DBNull.Value), new SqlParameter("@ToolId", ToolId ?? (object)DBNull.Value) };
                ObjCache = context.ExecuteStoreQuery<ToolMasterDTO>("exec GetToolListByID @RoomId,@CompanyId,@IsDeleted,@IsArchieved,@ToolGuid,@ToolId", params1).FirstOrDefault();



            }
            return ObjCache;
        }



        public List<ToolMasterDTO> GetToolForNarrowSearch(long RoomID, long CompanyID, bool IsDeleted, bool IsArchived)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ToolMasters.Where(t => t.Room == RoomID && t.CompanyID == CompanyID && (t.IsDeleted ?? false) == IsDeleted && (t.IsArchived ?? false) == IsArchived).Select(u => new ToolMasterDTO()
                {
                    ToolName = u.ToolName,
                    Created = u.Created ?? DateTime.MinValue,
                    CreatedBy = u.CreatedBy,
                    ID = u.ID,
                    LastUpdatedBy = u.LastUpdatedBy,
                    Room = u.Room,
                    Updated = u.Updated,
                    CompanyID = u.CompanyID,
                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                    GUID = u.GUID,
                    UDF1 = u.UDF1,
                    UDF2 = u.UDF2,
                    UDF3 = u.UDF3,
                    UDF4 = u.UDF4,
                    UDF5 = u.UDF5,
                    ToolCategoryID = u.ToolCategoryID,
                    AddedFrom = u.AddedFrom,
                    EditedFrom = u.EditedFrom,
                    ReceivedOn = u.ReceivedOn,
                    ReceivedOnWeb = u.ReceivedOnWeb,
                    Type = u.Type,
                    IsBuildBreak = u.IsBuildBreak,
                    AvailableToolQty = u.AvailableToolQty,
                    ToolTypeTracking = u.ToolTypeTracking ?? "1",
                    SerialNumberTracking = u.SerialNumberTracking,
                    LotNumberTracking = u.LotNumberTracking,
                    DateCodeTracking = u.DateCodeTracking
                }).AsParallel().ToList();
            }

        }
        public List<DTOForAutoComplete> GetToolCategoryNarrowSearch(long RoomID, long CompanyID, bool IsDeleted, bool IsArchived)
        {
            string sSQL = "";
            if (IsArchived && IsDeleted)
            {
                sSQL += "TM.IsDeleted = 1 AND TM.IsArchived = 1";
            }
            else if (IsArchived)
            {
                sSQL += "TM.IsArchived = 1";
            }
            else if (IsDeleted)
            {
                sSQL += "TM.IsDeleted =1";
            }
            else
            {
                sSQL += "TM.IsDeleted = 0 AND TM.IsArchived = 0";
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<DTOForAutoComplete> obj = (from u in context.ExecuteStoreQuery<DTOForAutoComplete>(@"
                                    select isnull(TM.ToolCategoryID,0) as ID,TCM.ToolCategory as [Key],cast(Count(TM.ID) as float)  as Quantity  from toolmaster TM
                                    Left join ToolCategoryMaster TCM on TCM.ID = TM.Toolcategoryid and TCM.isdeleted=0
                                    where TM.Room=" + RoomID.ToString() + "  and TM.CompanyID=" + CompanyID.ToString() + " and " + sSQL + " " +
                                    "group by TM.Toolcategoryid,TCM.ToolCategory")
                                                select new DTOForAutoComplete
                                                {
                                                    ID = u.ID,
                                                    Key = u.Key,
                                                    Quantity = (u.Quantity),

                                                }).AsParallel().ToList();

                return obj;

            }

        }

        public void UpdateToolAvailableQty(Guid ToolGUID, Int64 RoomID, Int64 CompanyID, string FromWhere = "")
        {
            ToolMasterDAL objItem = new ToolMasterDAL(base.DataBaseName);
            ToolMasterDTO ItemDTO = objItem.GetToolByGUIDPlain(ToolGUID);
            ToolAssetQuantityDetailDAL objToolAssetQuantityDetail = new ToolAssetQuantityDetailDAL(base.DataBaseName);
            List<ToolAssetQuantityDetailDTO> lstToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDAL(base.DataBaseName).GetToolAssetQuantityByAssetToolGuid(RoomID, CompanyID, false, false, ToolGUID).OrderByDescending(t => t.ReceivedDate).ThenByDescending(t => t.ID).ToList();
            if (lstToolAssetQuantityDetailDTO != null && lstToolAssetQuantityDetailDTO.Any() && lstToolAssetQuantityDetailDTO.Count() > 0)
            {
                ItemDTO.AvailableToolQty = lstToolAssetQuantityDetailDTO.Sum(t => t.Quantity);
            }
            if (ItemDTO != null)
            {
                ItemDTO.AvailableToolQty = ItemDTO.AvailableToolQty;
                ItemDTO.WhatWhereAction = "Update AvailableToolQty";
                objItem.Edit(ItemDTO);
            }
        }


        public List<DTOForAutoComplete> GetToolLocationNarrowSearch(long RoomID, long CompanyID, bool IsDeleted, bool IsArchived)
        {
            string sSQL = "";
            if (IsArchived && IsDeleted)
            {
                sSQL += "TM.IsDeleted = 1 AND TM.IsArchived = 1";
            }
            else if (IsArchived)
            {
                sSQL += "TM.IsArchived = 1";
            }
            else if (IsDeleted)
            {
                sSQL += "TM.IsDeleted =1";
            }
            else
            {
                sSQL += "TM.IsDeleted = 0 AND TM.IsArchived = 0";
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<DTOForAutoComplete> obj = (from u in context.ExecuteStoreQuery<DTOForAutoComplete>(@"
                                    select isnull(TM.LocationID,0) as ID,L.Location as [Key],cast(Count(TM.ID) as float) as Quantity from toolmaster TM
                                    Left join LocationMaster L on L.ID = TM.LocationID and L.isdeleted=0
                                    where TM.Room=" + RoomID.ToString() + "  and TM.CompanyID=" + CompanyID.ToString() + " and " + sSQL + " " +
                                    "group by TM.LocationID,L.Location")
                                                select new DTOForAutoComplete
                                                {
                                                    ID = u.ID,
                                                    Key = u.Key,
                                                    Quantity = (u.Quantity),
                                                }).AsParallel().ToList();

                return obj;

            }

        }

        public ToolMasterDTO GetRecordByToolGuid(Int64 RoomID, Int64 CompanyID, Guid ToolGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.ExecuteStoreQuery<ToolMasterDTO>("exec [GetToolRecordByGuid] @ToolGUID,@RoomId,@CompanyID", params1).FirstOrDefault();
            }
        }

        public ToolMasterDTO GetToolByToolNameSerial(string ToolNameSerialNumber, long RoomID, long CompanyID)
        {
            ToolMasterDTO ObjCache;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", false), new SqlParameter("@IsArchieved", false), new SqlParameter("@ToolNameSerialNumber", ToolNameSerialNumber ?? (object)DBNull.Value) };
                ObjCache = context.ExecuteStoreQuery<ToolMasterDTO>("exec GetToolByToolNameSerial @RoomId,@CompanyId,@IsDeleted,@IsArchieved,@ToolNameSerialNumber", params1).FirstOrDefault();
            }
            return ObjCache;
        }


        public ToolMasterDTO GetRecordBySerialAndTool(string ToolName, long RoomID, long CompanyID, string serial)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ToolMasters.Where(t => t.Room == RoomID && t.CompanyID == CompanyID
                                                        && t.Serial == serial && (t.IsDeleted ?? false) == false
                                                        && t.ToolName.Trim().ToUpper() == ToolName.Trim().ToUpper()).Select(u => new ToolMasterDTO()
                                                        {
                                                            ToolName = u.ToolName,
                                                            Created = u.Created ?? DateTime.MinValue,
                                                            CreatedBy = u.CreatedBy,
                                                            ID = u.ID,
                                                            LastUpdatedBy = u.LastUpdatedBy,
                                                            Room = u.Room,
                                                            Updated = u.Updated,
                                                            CompanyID = u.CompanyID,
                                                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                            GUID = u.GUID,
                                                            UDF1 = u.UDF1,
                                                            UDF2 = u.UDF2,
                                                            UDF3 = u.UDF3,
                                                            UDF4 = u.UDF4,
                                                            UDF5 = u.UDF5,
                                                            Serial = u.Serial,
                                                            AddedFrom = u.AddedFrom,
                                                            EditedFrom = u.EditedFrom,
                                                            ReceivedOn = u.ReceivedOn,
                                                            ReceivedOnWeb = u.ReceivedOnWeb,
                                                            Type = u.Type,
                                                            IsBuildBreak = u.IsBuildBreak,
                                                            AvailableToolQty = u.AvailableToolQty,
                                                            ToolTypeTracking = u.ToolTypeTracking ?? "1",
                                                            SerialNumberTracking = u.SerialNumberTracking,
                                                            LotNumberTracking = u.LotNumberTracking,
                                                            DateCodeTracking = u.DateCodeTracking
                                                        }).FirstOrDefault();
            }

        }


        public ToolMasterDTO GetToolByToolName(string ToolName, long RoomID, long CompanyID)
        {
            ToolMasterDTO ObjCache;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", false), new SqlParameter("@IsArchieved", false), new SqlParameter("@ToolName", ToolName ?? (object)DBNull.Value) };
                ObjCache = context.ExecuteStoreQuery<ToolMasterDTO>("exec GetToolByToolName @RoomId,@CompanyId,@IsDeleted,@IsArchieved,@ToolName", params1).FirstOrDefault();
            }
            return ObjCache;
        }


        public ToolMasterDTO GetToolNameBySerial(string SerailNumber, long RoomID, long CompanyID)
        {
            ToolMasterDTO ObjCache;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", false), new SqlParameter("@IsArchieved", false), new SqlParameter("@SerialNumber", SerailNumber ?? (object)DBNull.Value) };
                ObjCache = context.ExecuteStoreQuery<ToolMasterDTO>("exec GetToolBySerial @RoomId,@CompanyId,@IsDeleted,@IsArchieved,@SerialNumber", params1).FirstOrDefault();
            }
            return ObjCache;
        }

        /// <summary>
        /// Update Tool Record By ID From Within GRID itself 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE ToolMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "'),EditedFrom='Web', WhatWhereAction='', ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;
        }

        /// <summary>
        /// Delete Tool Record By ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolMaster obj = context.ToolMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                obj.EditedFrom = "Web";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.WhatWhereAction = "Delete Tool";
                context.ToolMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<ToolMasterDTO> ObjCache = CacheHelper<IEnumerable<ToolMasterDTO>>.GetCacheItem("Cached_ToolMaster_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<ToolMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<ToolMasterDTO>>.AppendToCacheItem("Cached_ToolMaster_" + obj.CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        public IEnumerable<ToolMasterDTO> GetAllRecordsOnlyImages()
        {
            IEnumerable<ToolMasterDTO> ObjCache = null;
            //Get Cached-Media


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<ToolMasterDTO> obj = (from u in context.ExecuteStoreQuery<ToolMasterDTO>(@"SELECT A.*
                        FROM ToolMaster A 
                        where  Isnull(A.ImagePath,'') != ''
                        ")
                                                  select new ToolMasterDTO
                                                  {
                                                      ID = u.ID,
                                                      Room = u.Room,
                                                      GUID = u.GUID,
                                                      CompanyID = u.CompanyID,
                                                      ImagePath = u.ImagePath,
                                                      ImageType = u.ImageType,
                                                      Type = u.Type,
                                                      IsBuildBreak = u.IsBuildBreak,
                                                      AvailableToolQty = u.AvailableToolQty
                                                  }).AsParallel().ToList();
                ObjCache = obj;
                return obj;
            }




        }

        /// <summary>
        /// Get Tools Record By ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ToolMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, Guid ToolGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", id), new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.ExecuteStoreQuery<ToolMasterDTO>("exec [GetToolRecordByIdGuid] @ID,@ToolGUID,@RoomId,@CompanyID", params1).FirstOrDefault();
            }
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        //public IEnumerable<ToolMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        //{
        //    IEnumerable<ToolMasterDTO> ObjCache;
        //    if (IsArchived == false && IsDeleted == false)
        //    {
        //        #region "both false"
        //        //Get Cached-Media
        //        ObjCache = CacheHelper<IEnumerable<ToolMasterDTO>>.GetCacheItem("Cached_ToolMaster_" + CompanyID.ToString());
        //        // if (ObjCache == null || ObjCache.Count() == 0)
        //        {
        //            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //            {
        //                IEnumerable<ToolMasterDTO> obj = (from u in context.ExecuteStoreQuery<ToolMasterDTO>(@"SELECT A.*
        //                                                ,B.UserName AS 'CreatedByName'
        //                                                ,C.UserName AS 'UpdatedByName'
        //                                                ,D.RoomName, E.ToolCategory 
        //                                                ,L.Location ,
        //                                                Tm.Technician,
        //						sum(isnull(TCIOH.checkedOutQty,0)) as CheckedOutQuantity,(sum(isnull(TCIOH.checkedOutQty,0))  - isnull(A.CheckedOutQty,0)) as CheckedInQuantity                                                   
        //                                                FROM ToolMaster A 
        //                                                LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
        //                                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
        //                                                LEFT OUTER JOIN Room D on A.Room = D.ID 
        //                                                LEFT OUTER JOIN ToolCategoryMaster E on A.ToolCategoryID = E.ID  
        //                                                LEFT OUTER JOIN LocationMaster L on A.LocationID = L.ID   
        //                                                left outer join TechnicianMaster  tm on Tm.guid = A.technicianGuid     
        //						 Left join ToolCheckInOutHistory TCIOH on TCIOH.toolguid=A.guid                                                 
        //                                                WHERE A.IsDeleted!=1 AND A.IsArchived!=1 AND A.CompanyID = " + CompanyID.ToString() +
        //                                                 "group by A.ID,A.GUID,A.ToolName,A.Serial,A.Description,A.ToolCategoryID,A.Cost,A.IscheckedOut,A.IsGroupOfItems,A.Quantity,A.CheckedOutQTY,A.CheckedOutMQTY,A.Created" +
        //                                                ",A.CreatedBy,A.Updated,A.LastUpdatedBy,A.Room,A.IsArchived,A.IsDeleted,A.LocationID,A.CompanyID,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.UDF6,A.UDF7,A.UDF8" +
        //                                                ",A.UDF9,A.UDF10,A.WhatWhereAction,A.MaintenanceCount,A.ReceivedOn,A.ReceivedOnWeb,A.AddedFrom,A.EditedFrom,A.NoOfPastMntsToConsider,A.IsAutoMaintain" +
        //                                                ",A.MaintenanceType,A.TechnicianGUID,B.UserName ,C.UserName,D.RoomName,E.ToolCategory, L.Location,Tm.Technician" +
        //                                                ",A.ImageType,A.ImagePath,A.ToolImageExternalURL,A.SuggestedMaintenanceDate")
        //                                                  select new ToolMasterDTO
        //                                                  {
        //                                                      ID = u.ID,
        //                                                      ToolName = u.ToolName,
        //                                                      Serial = u.Serial,
        //                                                      Description = u.Description,
        //                                                      Cost = u.Cost,
        //                                                      Quantity = u.Quantity,
        //                                                      IsCheckedOut = u.IsCheckedOut,
        //                                                      IsGroupOfItems = u.IsGroupOfItems,
        //                                                      ToolCategoryID = u.ToolCategoryID,
        //                                                      ToolCategory = u.ToolCategory,
        //                                                      Created = u.Created,
        //                                                      Updated = u.Updated,
        //                                                      CreatedBy = u.CreatedBy,
        //                                                      LastUpdatedBy = u.LastUpdatedBy,
        //                                                      CreatedByName = u.CreatedByName,
        //                                                      UpdatedByName = u.UpdatedByName,
        //                                                      Room = u.Room,
        //                                                      RoomName = u.RoomName,
        //                                                      Location = u.Location,
        //                                                      LocationID = u.LocationID,
        //                                                      GUID = u.GUID,
        //                                                      CompanyID = u.CompanyID,
        //                                                      IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
        //                                                      IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
        //                                                      CheckedOutQTY = u.CheckedOutQTY.GetValueOrDefault(0),
        //                                                      CheckedOutMQTY = u.CheckedOutMQTY.GetValueOrDefault(0),
        //                                                      UDF1 = u.UDF1,
        //                                                      UDF2 = u.UDF2,
        //                                                      UDF3 = u.UDF3,
        //                                                      UDF4 = u.UDF4,
        //                                                      UDF5 = u.UDF5,
        //                                                      ToolUDF1 = u.UDF1,
        //                                                      ToolUDF2 = u.UDF2,
        //                                                      ToolUDF3 = u.UDF3,
        //                                                      ToolUDF4 = u.UDF4,
        //                                                      ToolUDF5 = u.UDF5,
        //                                                      AddedFrom = u.AddedFrom,
        //                                                      EditedFrom = u.EditedFrom,
        //                                                      ReceivedOn = u.ReceivedOn,
        //                                                      ReceivedOnWeb = u.ReceivedOnWeb,
        //                                                      NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
        //                                                      MaintenanceType = u.MaintenanceType,
        //                                                      Technician = u.Technician,
        //                                                      TechnicianGuID = u.TechnicianGuID,
        //                                                      CheckedOutQuantity = u.CheckedOutQuantity.GetValueOrDefault(0),
        //                                                      CheckedInQuantity = u.CheckedInQuantity.GetValueOrDefault(0),
        //                                                      ImageType = u.ImageType,
        //                                                      ImagePath = u.ImagePath,
        //                                                      ToolImageExternalURL = u.ToolImageExternalURL
        //                                                  }).AsParallel().ToList();
        //                ObjCache = CacheHelper<IEnumerable<ToolMasterDTO>>.AddCacheItem("Cached_ToolMaster_" + CompanyID.ToString(), obj);
        //            }
        //        }
        //        #endregion
        //    }
        //    else
        //    {


        //        #region "Conditional"
        //        string sSQL = "";
        //        if (IsArchived && IsDeleted)
        //        {
        //            sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
        //        }
        //        else if (IsArchived)
        //        {
        //            sSQL += "A.IsArchived = 1";
        //        }
        //        else if (IsDeleted)
        //        {
        //            sSQL += "A.IsDeleted =1";
        //        }

        //        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //        {
        //            ObjCache = (from u in context.ExecuteStoreQuery<ToolMasterDTO>(
        //                        @"SELECT A.ID,A.ToolName, A.Serial, A.Description, A.Cost, A.IsDeleted
        //                        , A.IsArchived,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.IsCheckedOut,A.IsGroupOfItems
        //                        , A.Created, A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Quantity,A.GUID                                
        //                        , A.ToolCategoryID,A.LocationID,A.Room,A.CompanyId,B.UserName AS 'CreatedByName'
        //                        ,C.UserName AS 'UpdatedByName',D.RoomName, E.ToolCategory ,L.Location
        //                        ,A.CheckedOutQTY,A.CheckedOutMQTY   ,A.AddedFrom,A.EditedFrom,A.ReceivedOn,A.ReceivedOnWeb  ,
        //                                                Tm.Technician ,
        //                            A.ImageType,
        //                            A.ImagePath ,
        //                            A.ToolImageExternalURL                            
        //                        FROM ToolMaster A LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
        //                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
        //                        LEFT OUTER JOIN Room D on A.Room = D.ID 
        //                        LEFT OUTER JOIN ToolCategoryMaster E on A.ToolCategoryID = E.ID  
        //                        LEFT OUTER JOIN LocationMaster L on A.LocationID = L.ID         
        //                        left outer join TechnicianMaster  tm on Tm.guid = A.technicianGuid                             
        //                        WHERE A.CompanyID = " + CompanyID.ToString() + @" AND " + sSQL)
        //                        select new ToolMasterDTO
        //                        {
        //                            ID = u.ID,
        //                            ToolName = u.ToolName,
        //                            Serial = u.Serial,
        //                            Description = u.Description,
        //                            Cost = u.Cost,
        //                            Quantity = u.Quantity,
        //                            IsCheckedOut = u.IsCheckedOut,
        //                            IsGroupOfItems = u.IsGroupOfItems,
        //                            ToolCategoryID = u.ToolCategoryID,
        //                            ToolCategory = u.ToolCategory,
        //                            Created = u.Created,
        //                            Updated = u.Updated,
        //                            CreatedByName = u.CreatedByName,
        //                            UpdatedByName = u.UpdatedByName,
        //                            CreatedBy = u.CreatedBy,
        //                            LastUpdatedBy = u.LastUpdatedBy,
        //                            Room = u.Room,
        //                            RoomName = u.RoomName,
        //                            Location = u.Location,
        //                            LocationID = u.LocationID,
        //                            GUID = u.GUID,
        //                            CompanyID = u.CompanyID,
        //                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
        //                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
        //                            //CheckOutStatus = u.CheckOutStatus,
        //                            CheckedOutQTY = u.CheckedOutQTY.GetValueOrDefault(0),
        //                            CheckedOutMQTY = u.CheckedOutMQTY.GetValueOrDefault(0),
        //                            //CheckOutDate = u.CheckOutDate,
        //                            //CheckInDate = u.CheckInDate,
        //                            //CheckInCheckOutID = u.CheckInCheckOutID.GetValueOrDefault(0),
        //                            UDF1 = u.UDF1,
        //                            UDF2 = u.UDF2,
        //                            UDF3 = u.UDF3,
        //                            UDF4 = u.UDF4,
        //                            UDF5 = u.UDF5,
        //                            ToolUDF1 = u.UDF1,
        //                            ToolUDF2 = u.UDF2,
        //                            ToolUDF3 = u.UDF3,
        //                            ToolUDF4 = u.UDF4,
        //                            ToolUDF5 = u.UDF5,
        //                            AddedFrom = u.AddedFrom,
        //                            EditedFrom = u.EditedFrom,
        //                            ReceivedOn = u.ReceivedOn,
        //                            ReceivedOnWeb = u.ReceivedOnWeb,
        //                            NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
        //                            MaintenanceType = u.MaintenanceType,
        //                            Technician = u.Technician,
        //                            ImageType = u.ImageType,
        //                            ImagePath = u.ImagePath,
        //                            ToolImageExternalURL = u.ToolImageExternalURL
        //                        }).AsParallel().ToList();
        //        }
        //        #endregion
        //    }

        //    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
        //    IList<RoomDTO> ObjRoomCache = objRoomDAL.GetCachedData(CompanyID, false, false).ToList();
        //    UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);
        //    //IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllRecords(RoomID, CompanyID).ToList();

        //    //make changes for WI-304 issue
        //    IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllUsers().ToList();
        //    // //IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllUsers().ToList();

        //    ToolCategoryMasterDAL objToolMDAL = new ToolCategoryMasterDAL(base.DataBaseName);
        //    IList<ToolCategoryMasterDTO> ObjToolCateMCache = objToolMDAL.GetAllRecords(RoomID, CompanyID, false, false).ToList();
        //    LocationMasterDAL objLocMDAL = new LocationMasterDAL(base.DataBaseName);
        //    IList<LocationMasterDTO> ObjLocaMCache = objLocMDAL.GetAllRecords(RoomID, CompanyID, false, false).ToList();
        //    BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
        //    IList<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID).ToList();

        //    //ToolCheckInOutHistoryDAL objTOCDAL = new ToolCheckInOutHistoryDAL(base.DataBaseName);
        //    //IList<ToolCheckInOutHistoryDTO> ObjTOCCache = objTOCDAL.GetAllRecords(RoomID, CompanyID).ToList();

        //    ObjCache = (from u in ObjCache
        //                join im in ObjToolCateMCache on u.ToolCategoryID equals im.ID into u_IM_Join
        //                from u_IM in u_IM_Join.DefaultIfEmpty()
        //                join ps in ObjLocaMCache on u.LocationID equals ps.ID into u_PS_Join
        //                from u_PS in u_PS_Join.DefaultIfEmpty()
        //                join ur in ObjUserCache on u.CreatedBy equals ur.ID into u_UR_Join
        //                from u_UR in u_UR_Join.DefaultIfEmpty()
        //                join ur1 in ObjUserCache on u.LastUpdatedBy equals ur1.ID into u_UR1_Join
        //                from u_UR1 in u_UR1_Join.DefaultIfEmpty()
        //                join r in ObjRoomCache on u.Room equals r.ID into u_R_Join
        //                from u_R in u_R_Join.DefaultIfEmpty()
        //                //join u_toc in ObjTOCCache on u.ID equals u_toc.ToolID into u_toc_Join
        //                //from u_TOC in u_toc_Join.DefaultIfEmpty()
        //                join br in lstBarcodeDTO on u.GUID equals br.RefGUID into u_BR_Join
        //                from u_BR in u_BR_Join.DefaultIfEmpty()

        //                where u.CompanyID == CompanyID
        //                select new ToolMasterDTO
        //                {
        //                    ID = u.ID,
        //                    ToolName = u.ToolName,
        //                    Serial = u.Serial,
        //                    Description = u.Description,
        //                    Cost = u.Cost,
        //                    IsCheckedOut = u.IsCheckedOut,
        //                    IsGroupOfItems = u.IsGroupOfItems,
        //                    ToolCategoryID = u.ToolCategoryID,
        //                    Quantity = u.Quantity,
        //                    ToolCategory = u_IM != null ? u_IM.ToolCategory : string.Empty,
        //                    Created = u.Created,
        //                    Updated = u.Updated,
        //                    CreatedBy = u.CreatedBy,
        //                    LastUpdatedBy = u.LastUpdatedBy,
        //                    CreatedByName = u_UR != null ? u_UR.UserName : string.Empty,//ur.CreatedByName,
        //                    UpdatedByName = u_UR1 != null ? u_UR1.UserName : string.Empty,//ur1.UpdatedByName,
        //                    RoomName = u_R != null ? u_R.RoomName : string.Empty,//r.RoomName,
        //                    Room = u.Room,
        //                    Location = u_PS != null ? u_PS.Location : string.Empty,
        //                    LocationID = u.LocationID,
        //                    GUID = u.GUID,
        //                    CompanyID = u.CompanyID,
        //                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
        //                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
        //                    CheckedOutQTY = u.CheckedOutQTY,//u_TOC != null ? u_TOC.CheckedOutQTY.GetValueOrDefault(0) : 0,
        //                    CheckedOutMQTY = u.CheckedOutMQTY,//u_TOC != null ? u_TOC.CheckedOutMQTY.GetValueOrDefault(0): 0,
        //                    UDF1 = u.UDF1,
        //                    UDF2 = u.UDF2,
        //                    UDF3 = u.UDF3,
        //                    UDF4 = u.UDF4,
        //                    UDF5 = u.UDF5,
        //                    ToolUDF1 = u.UDF1,
        //                    ToolUDF2 = u.UDF2,
        //                    ToolUDF3 = u.UDF3,
        //                    ToolUDF4 = u.UDF4,
        //                    ToolUDF5 = u.UDF5,
        //                    AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Tool Master"),
        //                    AddedFrom = u.AddedFrom,
        //                    EditedFrom = u.EditedFrom,
        //                    ReceivedOn = u.ReceivedOn,
        //                    ReceivedOnWeb = u.ReceivedOnWeb,
        //                    NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
        //                    MaintenanceType = u.MaintenanceType,
        //                    Technician = u.Technician,
        //                    CheckedOutQuantity = u.CheckedOutQuantity,
        //                    CheckedInQuantity = u.CheckedInQuantity,
        //                    ImageType = u.ImageType,
        //                    ImagePath = u.ImagePath,
        //                    ToolImageExternalURL = u.ToolImageExternalURL
        //                }).AsParallel().ToList();

        //    return ObjCache.Where(t => t.Room == RoomID);
        //}


        //public IEnumerable<ToolMasterDTO> GetCachedCheckOutData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        //{
        //    IEnumerable<ToolMasterDTO> ObjCache;
        //    if (IsArchived == false && IsDeleted == false)
        //    {
        //        #region "both false"
        //        //Get Cached-Media
        //        ObjCache = CacheHelper<IEnumerable<ToolMasterDTO>>.GetCacheItem("Cached_ToolMaster_" + CompanyID.ToString());
        //        if (ObjCache == null || ObjCache.Count() == 0)
        //        {
        //            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //            {
        //                IEnumerable<ToolMasterDTO> obj = (from u in context.ExecuteStoreQuery<ToolMasterDTO>(@"SELECT A.ID,A.ToolName, A.Serial, A.Description, A.Cost, A.IsDeleted, A.IsArchived
        //                                            ,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.CreatedBy,A.LastUpdatedBy,A.Quantity,A.GUID
        //                                            ,A.IsCheckedOut,A.IsGroupOfItems, A.Created, A.Updated, A.ToolCategoryID
        //                                            ,A.LocationID,A.Room,A.CompanyId
        //                                            ,B.UserName AS 'CreatedByName'
        //                                            ,C.UserName AS 'UpdatedByName'
        //                                            ,D.RoomName, E.ToolCategory 
        //                                            ,L.Location
        //                                            ,A.CheckedOutQTY,A.CheckedOutMQTY  ,A.AddedFrom,A.EditedFrom,A.ReceivedOn,A.ReceivedOnWeb
        //                                            ,TC.udf1 as toolUDF1
        //                                            ,TC.udf2 as toolUDF2
        //                                            ,TC.udf3 as toolUDF3
        //                                            ,TC.udf4 as toolUDF4
        //                                            ,TC.udf5 as toolUDF5
        //                                            , A.ImageType 
        //                                            ,A.ImagePath 
        //                                            ,A.ToolImageExternalURL
        //                                            ,isnull(A.[Type],1) as [Type]
        //                                            ,isnull(A.[IsBuildBreak],0) as [IsBuildBreak]
        //                                            ,isnull(A.AvailableToolQty,0) as AvailableToolQty
        //                                            ,isnull(A.ToolTypeTracking,'') as ToolTypeTracking
        //                                            ,isnull(A.SerialNumberTracking,0) as SerialNumberTracking
        //                                ,isnull(A.LotNumberTracking,0) as LotNumberTracking
        //                                ,isnull(A.DateCodeTracking,0) as DateCodeTracking
        //                                    FROM ToolMaster A 
        //                                                LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
        //                                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
        //                                                LEFT OUTER JOIN Room D on A.Room = D.ID 
        //                                                LEFT OUTER JOIN ToolCategoryMaster E on A.ToolCategoryID = E.ID  
        //                                                LEFT OUTER JOIN LocationMaster L on A.LocationID = L.ID   
        //                                                left outer join ToolCheckInOutHistory TC on Tc.toolguid = A.guid                                                                                                  
        //                                WHERE A.IsDeleted!=1 AND A.IsArchived!=1 AND A.CompanyID = " + CompanyID.ToString() +
        //                                            "group by  A.ID,A.ToolName, A.Serial, A.Description, A.Cost, A.IsDeleted, A.IsArchived " +
        //                                            ",A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.CreatedBy,A.LastUpdatedBy,A.Quantity,A.GUID " +
        //                                            ",A.IsCheckedOut,A.IsGroupOfItems, A.Created, A.Updated, A.ToolCategoryID " +
        //                                            ",A.LocationID,A.Room,A.CompanyId " +
        //                                            ",B.UserName " +
        //                                            ",C.UserName " +
        //                                            ",D.RoomName, E.ToolCategory " +
        //                                            ",L.Location " +
        //                                            ",A.CheckedOutQTY,A.CheckedOutMQTY  ,A.AddedFrom,A.EditedFrom,A.ReceivedOn,A.ReceivedOnWeb " +
        //                                            ",TC.udf1,TC.udf2,TC.udf3,TC.udf4,TC.udf5 " +
        //                                          ", A.ImageType,A.ImagePath ,A.ToolImageExternalURL,A.SuggestedMaintenanceDate,A.[Type],A.IsBuildBreak,A.AvailableToolQty,A.ToolTypeTracking,A.SerialNumberTracking,A.LotNumberTracking,A.DateCodeTracking")
        //                                                  select new ToolMasterDTO
        //                                                  {
        //                                                      ID = u.ID,
        //                                                      ToolName = u.ToolName,
        //                                                      Serial = u.Serial,
        //                                                      Description = u.Description,
        //                                                      Cost = u.Cost,
        //                                                      Quantity = u.Quantity,
        //                                                      IsCheckedOut = u.IsCheckedOut,
        //                                                      IsGroupOfItems = u.IsGroupOfItems,
        //                                                      ToolCategoryID = u.ToolCategoryID,
        //                                                      ToolCategory = u.ToolCategory,
        //                                                      Created = u.Created,
        //                                                      Updated = u.Updated,
        //                                                      CreatedBy = u.CreatedBy,
        //                                                      LastUpdatedBy = u.LastUpdatedBy,
        //                                                      CreatedByName = u.CreatedByName,
        //                                                      UpdatedByName = u.UpdatedByName,
        //                                                      Room = u.Room,
        //                                                      RoomName = u.RoomName,
        //                                                      Location = u.Location,
        //                                                      LocationID = u.LocationID,
        //                                                      GUID = u.GUID,
        //                                                      CompanyID = u.CompanyID,
        //                                                      IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
        //                                                      IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
        //                                                      CheckedOutQTY = u.CheckedOutQTY.GetValueOrDefault(0),
        //                                                      CheckedOutMQTY = u.CheckedOutMQTY.GetValueOrDefault(0),
        //                                                      UDF1 = u.UDF1,
        //                                                      UDF2 = u.UDF2,
        //                                                      UDF3 = u.UDF3,
        //                                                      UDF4 = u.UDF4,
        //                                                      UDF5 = u.UDF5,
        //                                                      ToolUDF1 = u.ToolUDF1,
        //                                                      ToolUDF2 = u.ToolUDF2,
        //                                                      ToolUDF3 = u.ToolUDF3,
        //                                                      ToolUDF4 = u.ToolUDF4,
        //                                                      ToolUDF5 = u.ToolUDF5,
        //                                                      AddedFrom = u.AddedFrom,
        //                                                      EditedFrom = u.EditedFrom,
        //                                                      ReceivedOn = u.ReceivedOn,
        //                                                      ReceivedOnWeb = u.ReceivedOnWeb,
        //                                                      NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
        //                                                      MaintenanceType = u.MaintenanceType,
        //                                                      ImageType = u.ImageType,
        //                                                      ImagePath = u.ImagePath,
        //                                                      ToolImageExternalURL = u.ToolImageExternalURL,
        //                                                      MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
        //                                                      Type = u.Type,
        //                                                      IsBuildBreak=u.IsBuildBreak,
        //                                                      AvailableToolQty=u.AvailableToolQty,
        //                                                      ToolTypeTracking=u.ToolTypeTracking,
        //                                                      SerialNumberTracking=u.SerialNumberTracking,
        //                                                      LotNumberTracking=u.LotNumberTracking,
        //                                                      DateCodeTracking=u.DateCodeTracking
        //                                                  }).AsParallel().ToList();
        //                ObjCache = CacheHelper<IEnumerable<ToolMasterDTO>>.AddCacheItem("Cached_ToolMaster_" + CompanyID.ToString(), obj);
        //            }
        //        }
        //        #endregion
        //    }
        //    else
        //    {


        //        #region "Conditional"
        //        string sSQL = "";
        //        if (IsArchived && IsDeleted)
        //        {
        //            sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
        //        }
        //        else if (IsArchived)
        //        {
        //            sSQL += "A.IsArchived = 1";
        //        }
        //        else if (IsDeleted)
        //        {
        //            sSQL += "A.IsDeleted =1";
        //        }

        //        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //        {
        //            ObjCache = (from u in context.ExecuteStoreQuery<ToolMasterDTO>(
        //                        @"SELECT A.ID,A.ToolName, A.Serial, A.Description, A.Cost, A.IsDeleted
        //                        , A.IsArchived,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.IsCheckedOut,A.IsGroupOfItems
        //                        , A.Created, A.Updated,A.CreatedBy,A.LastUpdatedBy,A.Quantity,A.GUID                                
        //                        , A.ToolCategoryID,A.LocationID,A.Room,A.CompanyId,B.UserName AS 'CreatedByName'
        //                        ,C.UserName AS 'UpdatedByName',D.RoomName, E.ToolCategory ,L.Location
        //                        ,A.CheckedOutQTY,A.CheckedOutMQTY   ,A.AddedFrom,A.EditedFrom,A.ReceivedOn,A.ReceivedOnWeb  
        //                        ,TC.udf1 as toolUDF1
        //                        ,TC.udf2 as toolUDF2
        //                        ,TC.udf3 as toolUDF3
        //                        ,TC.udf4 as toolUDF4
        //                        ,TC.udf5 as toolUDF5      
        //                        , A.ImageType 
        //                                            ,A.ImagePath 
        //                                            ,A.ToolImageExternalURL    
        //                        ,isnull(A.[Type],1) as [Type]                 
        //                        ,isnull(A.[IsBuildBreak],0) as [IsBuildBreak]   
        //                        ,isnull(A.AvailableToolQty,0) as AvailableToolQty
        //                        ,isnull(A.ToolTypeTracking,'') as ToolTypeTracking
        //                        ,isnull(A.SerialNumberTracking,0) as SerialNumberTracking
        //            ,isnull(A.LotNumberTracking,0) as LotNumberTracking
        //            ,isnull(A.DateCodeTracking,0) as DateCodeTracking

        //                        FROM ToolMaster A LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
        //                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
        //                        LEFT OUTER JOIN Room D on A.Room = D.ID 
        //                        LEFT OUTER JOIN ToolCategoryMaster E on A.ToolCategoryID = E.ID  
        //                        LEFT OUTER JOIN LocationMaster L on A.LocationID = L.ID      
        //                        left outer join ToolCheckInOutHistory TC on Tc.toolguid = A.guid                             
        //                        WHERE A.CompanyID = " + CompanyID.ToString() + @" AND " + sSQL +
        //                                               " group by  A.ID,A.ToolName, A.Serial, A.Description, A.Cost, A.IsDeleted, A.IsArchived " +
        //                                            ",A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.CreatedBy,A.LastUpdatedBy,A.Quantity,A.GUID " +
        //                                            ",A.IsCheckedOut,A.IsGroupOfItems, A.Created, A.Updated, A.ToolCategoryID " +
        //                                            ",A.LocationID,A.Room,A.CompanyId " +
        //                                            ",B.UserName " +
        //                                            ",C.UserName " +
        //                                            ",D.RoomName, E.ToolCategory " +
        //                                            ",L.Location " +
        //                                            ",A.CheckedOutQTY,A.CheckedOutMQTY  ,A.AddedFrom,A.EditedFrom,A.ReceivedOn,A.ReceivedOnWeb " +
        //                                            ",TC.udf1,TC.udf2,TC.udf3,TC.udf4,TC.udf5 " +
        //                                          ", A.ImageType,A.ImagePath ,A.ToolImageExternalURL,A.SuggestedMaintenanceDate,A.[Type],A.IsBuildBreak,A.AvailableToolQty,A.ToolTypeTracking,A.SerialNumberTracking,A.LotNumberTracking,A.DateCodeTracking")
        //                        select new ToolMasterDTO
        //                        {
        //                            ID = u.ID,
        //                            ToolName = u.ToolName,
        //                            Serial = u.Serial,
        //                            Description = u.Description,
        //                            Cost = u.Cost,
        //                            Quantity = u.Quantity,
        //                            IsCheckedOut = u.IsCheckedOut,
        //                            IsGroupOfItems = u.IsGroupOfItems,
        //                            ToolCategoryID = u.ToolCategoryID,
        //                            ToolCategory = u.ToolCategory,
        //                            Created = u.Created,
        //                            Updated = u.Updated,
        //                            CreatedByName = u.CreatedByName,
        //                            UpdatedByName = u.UpdatedByName,
        //                            CreatedBy = u.CreatedBy,
        //                            LastUpdatedBy = u.LastUpdatedBy,
        //                            Room = u.Room,
        //                            RoomName = u.RoomName,
        //                            Location = u.Location,
        //                            LocationID = u.LocationID,
        //                            GUID = u.GUID,
        //                            CompanyID = u.CompanyID,
        //                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
        //                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
        //                            //CheckOutStatus = u.CheckOutStatus,
        //                            CheckedOutQTY = u.CheckedOutQTY.GetValueOrDefault(0),
        //                            CheckedOutMQTY = u.CheckedOutMQTY.GetValueOrDefault(0),
        //                            //CheckOutDate = u.CheckOutDate,
        //                            //CheckInDate = u.CheckInDate,
        //                            //CheckInCheckOutID = u.CheckInCheckOutID.GetValueOrDefault(0),
        //                            UDF1 = u.UDF1,
        //                            UDF2 = u.UDF2,
        //                            UDF3 = u.UDF3,
        //                            UDF4 = u.UDF4,
        //                            UDF5 = u.UDF5,
        //                            ToolUDF1 = u.ToolUDF1,
        //                            ToolUDF2 = u.ToolUDF2,
        //                            ToolUDF3 = u.ToolUDF3,
        //                            ToolUDF4 = u.ToolUDF4,
        //                            ToolUDF5 = u.ToolUDF5,
        //                            AddedFrom = u.AddedFrom,
        //                            EditedFrom = u.EditedFrom,
        //                            ReceivedOn = u.ReceivedOn,
        //                            ReceivedOnWeb = u.ReceivedOnWeb,
        //                            NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
        //                            MaintenanceType = u.MaintenanceType,
        //                            ImageType = u.ImageType,
        //                            ImagePath = u.ImagePath,
        //                            ToolImageExternalURL = u.ToolImageExternalURL,
        //                            MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
        //                            Type = u.Type,
        //                            IsBuildBreak=u.IsBuildBreak,
        //                            AvailableToolQty=u.AvailableToolQty,
        //                            ToolTypeTracking = u.ToolTypeTracking,
        //                            SerialNumberTracking=u.SerialNumberTracking,
        //                            LotNumberTracking=u.LotNumberTracking,
        //                            DateCodeTracking=u.DateCodeTracking
        //                        }).AsParallel().ToList();
        //        }
        //        #endregion
        //    }

        //    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
        //    IList<RoomDTO> ObjRoomCache = objRoomDAL.GetCachedData(CompanyID, false, false).ToList();
        //    UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);
        //    //IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllRecords(RoomID, CompanyID).ToList();

        //    //make changes for WI-304 issue
        //    IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllUsers().ToList();
        //    // //IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllUsers().ToList();

        //    ToolCategoryMasterDAL objToolMDAL = new ToolCategoryMasterDAL(base.DataBaseName);
        //    IList<ToolCategoryMasterDTO> ObjToolCateMCache = objToolMDAL.GetAllRecords(RoomID, CompanyID, false, false).ToList();
        //    LocationMasterDAL objLocMDAL = new LocationMasterDAL(base.DataBaseName);
        //    IList<LocationMasterDTO> ObjLocaMCache = objLocMDAL.GetAllRecords(RoomID, CompanyID, false, false).ToList();
        //    BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
        //    IList<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID).ToList();

        //    //ToolCheckInOutHistoryDAL objTOCDAL = new ToolCheckInOutHistoryDAL(base.DataBaseName);
        //    //IList<ToolCheckInOutHistoryDTO> ObjTOCCache = objTOCDAL.GetAllRecords(RoomID, CompanyID).ToList();

        //    ObjCache = (from u in ObjCache
        //                join im in ObjToolCateMCache on u.ToolCategoryID equals im.ID into u_IM_Join
        //                from u_IM in u_IM_Join.DefaultIfEmpty()
        //                join ps in ObjLocaMCache on u.LocationID equals ps.ID into u_PS_Join
        //                from u_PS in u_PS_Join.DefaultIfEmpty()
        //                join ur in ObjUserCache on u.CreatedBy equals ur.ID into u_UR_Join
        //                from u_UR in u_UR_Join.DefaultIfEmpty()
        //                join ur1 in ObjUserCache on u.LastUpdatedBy equals ur1.ID into u_UR1_Join
        //                from u_UR1 in u_UR1_Join.DefaultIfEmpty()
        //                join r in ObjRoomCache on u.Room equals r.ID into u_R_Join
        //                from u_R in u_R_Join.DefaultIfEmpty()
        //                    //join u_toc in ObjTOCCache on u.ID equals u_toc.ToolID into u_toc_Join
        //                    //from u_TOC in u_toc_Join.DefaultIfEmpty()
        //                join br in lstBarcodeDTO on u.GUID equals br.RefGUID into u_BR_Join
        //                from u_BR in u_BR_Join.DefaultIfEmpty()

        //                where u.CompanyID == CompanyID
        //                select new ToolMasterDTO
        //                {
        //                    ID = u.ID,
        //                    ToolName = u.ToolName,
        //                    Serial = u.Serial,
        //                    Description = u.Description,
        //                    Cost = u.Cost,
        //                    IsCheckedOut = u.IsCheckedOut,
        //                    IsGroupOfItems = u.IsGroupOfItems,
        //                    ToolCategoryID = u.ToolCategoryID,
        //                    Quantity = u.Quantity,
        //                    ToolCategory = u_IM != null ? u_IM.ToolCategory : string.Empty,
        //                    Created = u.Created,
        //                    Updated = u.Updated,
        //                    CreatedBy = u.CreatedBy,
        //                    LastUpdatedBy = u.LastUpdatedBy,
        //                    CreatedByName = u_UR != null ? u_UR.UserName : string.Empty,//ur.CreatedByName,
        //                    UpdatedByName = u_UR1 != null ? u_UR1.UserName : string.Empty,//ur1.UpdatedByName,
        //                    RoomName = u_R != null ? u_R.RoomName : string.Empty,//r.RoomName,
        //                    Room = u.Room,
        //                    Location = u_PS != null ? u_PS.Location : string.Empty,
        //                    LocationID = u.LocationID,
        //                    GUID = u.GUID,
        //                    CompanyID = u.CompanyID,
        //                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
        //                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
        //                    CheckedOutQTY = u.CheckedOutQTY,//u_TOC != null ? u_TOC.CheckedOutQTY.GetValueOrDefault(0) : 0,
        //                    CheckedOutMQTY = u.CheckedOutMQTY,//u_TOC != null ? u_TOC.CheckedOutMQTY.GetValueOrDefault(0): 0,
        //                    UDF1 = u.UDF1,
        //                    UDF2 = u.UDF2,
        //                    UDF3 = u.UDF3,
        //                    UDF4 = u.UDF4,
        //                    UDF5 = u.UDF5,
        //                    ToolUDF1 = u.ToolUDF1,
        //                    ToolUDF2 = u.ToolUDF2,
        //                    ToolUDF3 = u.ToolUDF3,
        //                    ToolUDF4 = u.ToolUDF4,
        //                    ToolUDF5 = u.ToolUDF5,
        //                    AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Tool Master"),
        //                    AddedFrom = u.AddedFrom,
        //                    EditedFrom = u.EditedFrom,
        //                    ReceivedOn = u.ReceivedOn,
        //                    ReceivedOnWeb = u.ReceivedOnWeb,
        //                    NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
        //                    MaintenanceType = u.MaintenanceType,
        //                    ImageType = u.ImageType,
        //                    ImagePath = u.ImagePath,
        //                    ToolImageExternalURL = u.ToolImageExternalURL,
        //                    MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
        //                    Type = u.Type,
        //                    IsBuildBreak=u.IsBuildBreak,
        //                    AvailableToolQty=u.AvailableToolQty,
        //                    ToolTypeTracking=u.ToolTypeTracking,
        //                    SerialNumberTracking=u.SerialNumberTracking,
        //                    LotNumberTracking=u.LotNumberTracking,
        //                    DateCodeTracking=u.DateCodeTracking
        //                }).AsParallel().ToList();

        //    return ObjCache.Where(t => t.Room == RoomID);
        //}


        //public IEnumerable<ToolMasterDTO> GetAllCheckOutRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        //{
        //    return GetCachedCheckOutData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        //}


        /// <summary>
        /// Get Paged Records from the Bin Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        //public IEnumerable<ToolMasterDTO> GetToolMaintainceDue(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, int DueDay)
        //{
        //    IList<ToolsMaintenanceDTO> lstTools = null;
        //    ToolsMaintenanceDAL objTools = new ToolsMaintenanceDAL(base.DataBaseName);
        //    lstTools = objTools.GetCachedData(RoomID, CompanyId, DueDay).ToList(); //.Where(x => x.AssetGUID.HasValue).Take(10).ToList();

        //    IEnumerable<ToolMasterDTO> ObjCache;
        //    ObjCache = GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC").ToList();

        //    return (from c in ObjCache where (from x in lstTools select x.ToolGUID).Contains(c.GUID) select c).ToList();

        //    // return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        //}
        //public IEnumerable<ToolMasterDTO> GetToolMaintainceDue(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, int DueDay, string DBConnectionstring)
        //{
        //    IList<ToolsMaintenanceDTO> lstTools = null;
        //    ToolsMaintenanceDAL objTools = new ToolsMaintenanceDAL(base.DataBaseName);
        //    lstTools = objTools.GetCachedData(RoomID, CompanyId, DueDay, DBConnectionstring).ToList(); //.Where(x => x.AssetGUID.HasValue).Take(10).ToList();

        //    IEnumerable<ToolMasterDTO> ObjCache;
        //    ObjCache = GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, DBConnectionstring).OrderBy("ID DESC").ToList();

        //    return (from c in ObjCache where (from x in lstTools select x.ToolGUID).Contains(c.GUID) select c).ToList();

        //    // return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        //}

        public ToolAssetPullInfo PullItemQuantity(ToolAssetPullInfo objToolPullInfo, long ModuleId, string PullCredit = "CheckOut")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(base.DataBaseName);
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);

                //---------------------------------------------------------------------
                //
                if (objToolPullInfo == null || objToolPullInfo.lstToolPullDetails == null)
                {
                    return objToolPullInfo;
                }



                List<ToolLocationDetail> objToolLocationDetail = context.ToolLocationDetails.Where(t => t.ToolGuid == objToolPullInfo.ToolGUID && t.IsDeleted == false && t.ID == objToolPullInfo.BinID).ToList();

                List<ToolAssetQuantityDetail> objToolAssetQuantityDetail = context.ToolAssetQuantityDetails.Where(t => t.ToolGUID == objToolPullInfo.ToolGUID && t.ToolBinID == objToolPullInfo.BinID).ToList();

                LocationMasterDTO objLocDTO = new LocationMasterDTO();
                if (objToolLocationDetail != null && objToolLocationDetail.Count() > 0)
                {
                    objLocDTO = new LocationMasterDAL(base.DataBaseEntityConnectionString).GetLocationsByGUID(objToolLocationDetail[0].LocationGuid ?? Guid.Empty, objToolPullInfo.RoomId, objToolPullInfo.CompanyId, false, false);
                }
                else
                {
                    objLocDTO = null;
                }
                ToolMaster objTool = context.ToolMasters.FirstOrDefault(t => t.GUID == objToolPullInfo.ToolGUID);

                //---------------------------------------------------------------------
                //
                double? ItemPullCost = 0;
                //if (((objItemPullInfo.RequisitionDetailsGUID != null && objItemPullInfo.RequisitionDetailsGUID != Guid.Empty)
                //        || (objItemPullInfo.WorkOrderDetailGUID != null && objItemPullInfo.WorkOrderDetailGUID != Guid.Empty)
                //     ) && ModuleId != 0)
                //{
                //    int? PriseSelectionOption = 0;
                //    RoomModuleSettingsDTO objRoomModuleSettingsDTO = objRoomDAL.GetRoomModuleSettings((long)objItem.CompanyID, (long)objItem.Room, ModuleId);
                //    if (objRoomModuleSettingsDTO == null || objRoomModuleSettingsDTO.PriseSelectionOption == null || (objRoomModuleSettingsDTO.PriseSelectionOption != 1 && objRoomModuleSettingsDTO.PriseSelectionOption != 2))
                //        PriseSelectionOption = 1;
                //    else
                //        PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption;

                //    ItemPullCost = (PriseSelectionOption == 1 ? objItem.SellPrice : objItem.Cost);
                //}
                //else
                {
                    if (objTool != null)
                    {
                        ItemPullCost = objTool.Cost;
                    }

                }
                double? ToolPullPrice = 0;
                ToolPullPrice = objTool.Cost;
                //---------------------------------------------------------------------
                //

                // objItemPullInfo.PullGUID = objPullMaster.GUID;

                objToolPullInfo.lstToolPullDetails.ForEach(t =>
                {
                    string InventoryConsuptionMethod = string.Empty;
                    Room objRoomDTO = context.Rooms.FirstOrDefault(x => x.ID == objToolPullInfo.RoomId);
                    if (objRoomDTO != null && !string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod))
                    {
                        InventoryConsuptionMethod = objRoomDTO.InventoryConsuptionMethod;
                    }

                    if (string.IsNullOrEmpty(InventoryConsuptionMethod))
                        InventoryConsuptionMethod = "";

                    //ItemLocationDetail objItemLocationDetail = null;
                    List<ToolAssetQuantityDetail> lstToolAssetQuantityDetails = null;
                    //switch (InventoryConsuptionMethod.ToLower())
                    //{
                    //    case "lifo":
                    //    case "lifooverride":
                    //        lstItemLocations = context.ItemLocationDetails.Where(x => (
                    //                      (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                    //                          || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                    //                      || (!t.LotNumberTracking && !t.SerialNumberTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                    //                      && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                    //        lstItemLocations = lstItemLocations.OrderByDescending(x => x.ReceivedDate).ToList();
                    //        break;
                    //    case "fifo":
                    //    case "fifooverride":
                    lstToolAssetQuantityDetails = context.ToolAssetQuantityDetails.Where(x => (
                                                 //(x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking) ||
                                                 (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                            || (!t.SerialNumberTracking)) && x.ToolGUID == t.ToolGUID && x.ToolBinID == t.BinID
                                            && (((x.Quantity ?? 0)) > 0) && (x.IsDeleted) == false).ToList();
                    lstToolAssetQuantityDetails = lstToolAssetQuantityDetails.OrderBy(x => x.ReceivedDate).ToList();
                    //    break;
                    //default:
                    //    lstItemLocations = context.ItemLocationDetails.Where(x => (
                    //                   (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                    //                       || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                    //                   || (!t.LotNumberTracking && !t.SerialNumberTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                    //                   && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                    //    lstItemLocations = lstItemLocations.OrderBy(x => x.ReceivedDate).ToList();
                    //    break;
                    // }


                    if (lstToolAssetQuantityDetails != null)
                    {
                        foreach (var objToolAssetQuantityDetail1 in lstToolAssetQuantityDetails)
                        {
                            if ((objToolAssetQuantityDetail1.Quantity ?? 0) >= (t.TobePulled))
                            {
                                //PullDetail objPullDetail = new PullDetail();
                                //objPullDetail.BinID = t.BinID;
                                //objPullDetail.CompanyID = objItemPullInfo.CompanyId;
                                //objPullDetail.ConsignedQuantity = t.ConsignedTobePulled;
                                //objPullDetail.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                                //objPullDetail.GUID = Guid.NewGuid();
                                //objPullDetail.IsArchived = false;
                                //objPullDetail.IsDeleted = false;
                                //objPullDetail.ItemCost = t.Cost;
                                //objPullDetail.ItemGUID = objItemPullInfo.ItemGUID;
                                //objPullDetail.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                                //objPullDetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                                //objPullDetail.MaterialStagingPullDetailGUID = null;
                                //objPullDetail.PoolQuantity = t.CustomerOwnedTobePulled + t.ConsignedTobePulled;
                                //objPullDetail.ProjectSpendGUID = objItemPullInfo.ProjectSpendGUID;
                                //objPullDetail.PullCredit = PullCredit;
                                //objPullDetail.PULLGUID = objItemPullInfo.PullGUID;

                                //if (!string.IsNullOrWhiteSpace(objItemLocationDetail.Received))
                                //    objPullDetail.Received = objItemLocationDetail.Received;
                                //else if (objItemLocationDetail.ReceivedDate.HasValue)
                                //    objPullDetail.Received = objItemLocationDetail.ReceivedDate.Value.ToString("MM/dd/yyyy");

                                //objPullDetail.Room = objItemPullInfo.RoomId;
                                //objPullDetail.SerialNumber = t.SerialNumberTracking ? t.LotOrSerailNumber : string.Empty;

                                //if (objItem.DateCodeTracking && !string.IsNullOrWhiteSpace(objItemLocationDetail.Expiration))
                                //    objPullDetail.Expiration = objItemLocationDetail.Expiration;
                                //else if (objItem.DateCodeTracking && objItemLocationDetail.ExpirationDate.HasValue)
                                //    objPullDetail.Expiration = objItemLocationDetail.ExpirationDate.Value.ToString("MM/dd/yyyy");

                                //objPullDetail.Updated = DateTimeUtility.DateTimeNow;
                                //objPullDetail.Created = DateTimeUtility.DateTimeNow;
                                //objPullDetail.CreatedBy = objItemPullInfo.CreatedBy;
                                //objPullDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                                //objPullDetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                //objPullDetail.AddedFrom = "Web";
                                //objPullDetail.EditedFrom = "Web";
                                //objPullDetail.ItemLocationDetailGUID = objItemLocationDetail.GUID;

                                //double? itemCost = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullCost, objItemLocationDetail.Cost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                //double? itemSellprice = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullPrice, objItemLocationDetail.Cost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                //objPullDetail.ItemCost = itemCost;
                                //objPullDetail.ItemPrice = itemSellprice;
                                //objPullMaster.PULLCost = (objPullMaster.PULLCost.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemCost.GetValueOrDefault(0));
                                //objPullMaster.PullPrice = (objPullMaster.PullPrice.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemSellprice.GetValueOrDefault(0));
                                //context.PullDetails.AddObject(objPullDetail);

                                objToolAssetQuantityDetail1.Quantity = (objToolAssetQuantityDetail1.Quantity) - t.TobePulled;


                                //Get Cached-Media Create PullDetails Cache
                                //CreatePullDetailsCache(objPullDetail);
                                //Get Cached-Media Create ItemLocationDetails Cache
                                //CreateItemLocationDetailsCache(objItemLocationDetail);
                                break;
                            }
                            else if ((objToolAssetQuantityDetail1.Quantity ?? 0) > 0)
                            {

                                //PullDetail objPullDetail = new PullDetail();
                                //objPullDetail.BinID = t.BinID;
                                //objPullDetail.CompanyID = objItemPullInfo.CompanyId;

                                //-----------------------SET CUSTOMER OWNED QUANTITY-----------------------
                                //
                                if (objToolAssetQuantityDetail1.Quantity != null && objToolAssetQuantityDetail1.Quantity > 0)
                                {
                                    if ((objToolAssetQuantityDetail1.Quantity ?? 0) < t.TobePulled)
                                    {
                                        // objPullDetail.CustomerOwnedQuantity = objItemLocationDetail.CustomerOwnedQuantity;
                                        t.TobePulled = (t.TobePulled) - (objToolAssetQuantityDetail1.Quantity ?? 0);
                                        objToolAssetQuantityDetail1.Quantity = 0;

                                    }
                                    else
                                    {
                                        //objPullDetail.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                                        objToolAssetQuantityDetail1.Quantity = objToolAssetQuantityDetail1.Quantity - t.TobePulled;
                                        t.TobePulled = 0;
                                    }
                                }
                                else
                                {
                                    //objPullDetail.Quantity = 0;
                                }



                                //objPullDetail.GUID = Guid.NewGuid();
                                //objPullDetail.IsArchived = false;
                                //objPullDetail.IsDeleted = false;
                                //objPullDetail.ItemCost = t.Cost;
                                //objPullDetail.ItemGUID = objItemPullInfo.ItemGUID;
                                //objPullDetail.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                                //objPullDetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                                //objPullDetail.MaterialStagingPullDetailGUID = null;
                                ////objPullDetail.PoolQuantity = t.CustomerOwnedTobePulled + t.ConsignedTobePulled;
                                //objPullDetail.PoolQuantity = (objPullDetail.CustomerOwnedQuantity ?? 0) + (objPullDetail.ConsignedQuantity ?? 0);
                                //objPullDetail.ProjectSpendGUID = objItemPullInfo.ProjectSpendGUID;
                                //objPullDetail.PullCredit = PullCredit;
                                //objPullDetail.PULLGUID = objItemPullInfo.PullGUID;

                                //if (!string.IsNullOrWhiteSpace(objItemLocationDetail.Received))
                                //    objPullDetail.Received = objItemLocationDetail.Received;
                                //else if (objItemLocationDetail.ReceivedDate.HasValue)
                                //    objPullDetail.Received = objItemLocationDetail.ReceivedDate.Value.ToString("MM/dd/yyyy");

                                //objPullDetail.Room = objItemPullInfo.RoomId;
                                //objPullDetail.SerialNumber = t.SerialNumberTracking ? t.LotOrSerailNumber : string.Empty;

                                //if (objItem.DateCodeTracking && !string.IsNullOrWhiteSpace(objItemLocationDetail.Expiration))
                                //    objPullDetail.Expiration = objItemLocationDetail.Expiration;
                                //else if (objItem.DateCodeTracking && objItemLocationDetail.ExpirationDate.HasValue)
                                //    objPullDetail.Expiration = objItemLocationDetail.ExpirationDate.Value.ToString("MM/dd/yyyy");

                                //objPullDetail.Updated = DateTimeUtility.DateTimeNow;
                                //objPullDetail.Created = DateTimeUtility.DateTimeNow;
                                //objPullDetail.CreatedBy = objItemPullInfo.CreatedBy;
                                //objPullDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                                //objPullDetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                //objPullDetail.AddedFrom = "Web";
                                //objPullDetail.EditedFrom = "Web";
                                //objPullDetail.ItemLocationDetailGUID = objItemLocationDetail.GUID;

                                //double? itemCost = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullCost, objItemLocationDetail.Cost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                //double? itemSellprice = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullPrice, objItemLocationDetail.Cost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                //objPullDetail.ItemCost = itemCost;
                                //objPullDetail.ItemPrice = itemSellprice;
                                //objPullMaster.PULLCost = (objPullMaster.PULLCost.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemCost.GetValueOrDefault(0));
                                //objPullMaster.PullPrice = (objPullMaster.PullPrice.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemCost.GetValueOrDefault(0));
                                //context.PullDetails.AddObject(objPullDetail);

                                if (objToolAssetQuantityDetail1.Quantity < 0)
                                    objToolAssetQuantityDetail1.Quantity = 0;



                                //Get Cached-Media Create PullDetails Cache
                                // CreatePullDetailsCache(objPullDetail);
                                //Get Cached-Media Create ItemLocationDetails Cache
                                // CreateItemLocationDetailsCache(objItemLocationDetail);
                            }
                        }
                    }

                });

                // objPullMaster.CustomerOwnedQuantity = objItemPullInfo.TotalCustomerOwnedTobePulled;
                // objPullMaster.ConsignedQuantity = objItemPullInfo.TotalConsignedTobePulled;




                context.SaveChanges();

                objTool.CheckedOutQTY =
                objTool.AvailableToolQty = context.ToolAssetQuantityDetails.Where(t => t.ToolGUID == objToolPullInfo.ToolGUID && (t.IsDeleted) == false && (t.IsArchived) == false).Sum(t => (t.Quantity ?? 0));
                objTool.WhatWhereAction = string.Empty;
                context.SaveChanges();





                //CostDTO ItemCostDTO = objItemMasterDAL.GetExtCostAndAvgCost(objItem.GUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                //objItem.Cost = ItemCostDTO.Cost;
                //objItem.ExtendedCost = ItemCostDTO.ExtCost;
                //objItem.AverageCost = ItemCostDTO.AvgCost;

                context.SaveChanges();




                ToolMasterDTO oItemDTO = objToolMasterDAL.GetRecord(objTool.ID, objTool.Room.GetValueOrDefault(0), objTool.CompanyID.GetValueOrDefault(0), Guid.Empty);
                //CreateItemMasterCache(oItemDTO);

                //if (objPullMaster.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                //{
                //    WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
                //    objWOLDAL.UpdateWOItemAndTotalCost(objPullMaster.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), objPullMaster.Room.GetValueOrDefault(0), objPullMaster.CompanyID.GetValueOrDefault(0));
                //}

                //if (objPullMaster.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                //{
                //    RequisitionDetailsDAL objReqDDAL = new RequisitionDetailsDAL(base.DataBaseName);
                //    RequisitionDetailsDTO objReqDTO = objReqDDAL.GetRecord(objPullMaster.RequisitionDetailGUID.Value, (Int64)objPullMaster.Room, (Int64)objPullMaster.CompanyID);
                //    //RequisitionDetailsDTO ObjOldReqDTO = objReqDDAL.GetRecord(objPullMaster.RequisitionDetailGUID.Value, (Int64)objPullMaster.Room, (Int64)objPullMaster.CompanyID);

                //    objReqDTO.QuantityPulled = objReqDTO.QuantityPulled.GetValueOrDefault(0) + objPullMaster.PoolQuantity;
                //    objReqDDAL.Edit(objReqDTO);
                //}

                return objToolPullInfo;
            }
        }

        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId, Int64 RoomID, out string MSG)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                MSG = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        ToolMasterDTO tempDTO = GetRecord(Int64.Parse(item), RoomID, CompanyId, Guid.Empty);
                        if (tempDTO != null)
                        {
                            if (tempDTO.CheckedOutMQTY.GetValueOrDefault(0) > 0 || tempDTO.CheckedOutQTY.GetValueOrDefault(0) > 0)
                                MSG += tempDTO.ToolName.ToString() + ",";
                            else
                            {
                                strQuery = "UPDATE ToolMaster SET Updated = '" + DateTime.Now.ToString() + "' ,LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,EditedFrom='Web', WhatWhereAction='Delete Tool' , ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID =" + item.ToString() + ";";
                                context.ExecuteStoreCommand(strQuery);
                                //Get Cached-Media
                                IEnumerable<ToolMasterDTO> ObjCache = CacheHelper<IEnumerable<ToolMasterDTO>>.GetCacheItem("Cached_ToolMaster_" + CompanyId.ToString());
                                if (ObjCache != null)
                                {
                                    List<ToolMasterDTO> objTemp = ObjCache.ToList();
                                    objTemp.RemoveAll(i => i.ID == Int64.Parse(item));
                                    ObjCache = objTemp.AsEnumerable();
                                    CacheHelper<IEnumerable<ToolMasterDTO>>.AppendToCacheItem("Cached_ToolMaster_" + CompanyId.ToString(), ObjCache);
                                }
                            }
                        }
                        else
                            MSG += "Invalid Operation!";
                    }
                }
                return true;
            }
        }

        public ToolMasterDTO GetRecordNew(Int64 id, Int64 RoomID, Int64 CompanyID, Guid ToolGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", id), new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.ExecuteStoreQuery<ToolMasterDTO>("exec [GetToolRecordByIdGuidNew] @ID,@ToolGUID,@RoomId,@CompanyID", params1).FirstOrDefault();
            }
        }

        public IEnumerable<ToolMasterDTO> GetAllRecordsByGUID(Guid ToolGUID)
        {
            IEnumerable<ToolMasterDTO> ObjCache = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                IEnumerable<ToolMasterDTO> obj = (from u in context.ExecuteStoreQuery<ToolMasterDTO>(@"SELECT A.*
                        FROM ToolMaster A where A.GUID = '" + ToolGUID + "'")
                                                  select new ToolMasterDTO
                                                  {
                                                      ID = u.ID,
                                                      Room = u.Room,
                                                      GUID = u.GUID,
                                                      CompanyID = u.CompanyID,
                                                      ImagePath = u.ImagePath,
                                                      ImageType = u.ImageType,
                                                      Type = u.Type,
                                                      IsBuildBreak = u.IsBuildBreak,
                                                      AvailableToolQty = u.AvailableToolQty
                                                  }).AsParallel().ToList();
                ObjCache = obj;
                return obj;
            }
        }

        public Guid? GetUsedToolGuidinQuantity(Int64 RoomId, Int64 CompanyID, Guid PassedGUID, string ToolName)
        {
            Guid? ReturnResult = Guid.Empty;
            ReturnResult = PassedGUID;
            return ReturnResult;

        }

        public IEnumerable<ToolMasterDTO> ToolExportData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<ToolMasterDTO> ObjCache;
            // if (IsArchived == false && IsDeleted == false)
            {
                #region Get All Data
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                    List<ToolMasterDTO> lstresult = context.ExecuteStoreQuery<ToolMasterDTO>("exec Tool_AllData @CompanyId,@RoomId,@IsDeleted,@IsArchived", params1).ToList();
                    ObjCache = (from u in lstresult
                                select new ToolMasterDTO
                                {
                                    ID = u.ID,
                                    ToolName = u.ToolName,
                                    Serial = u.Serial,
                                    Description = u.Description,
                                    Cost = u.Cost,
                                    Quantity = u.Quantity,
                                    IsCheckedOut = u.IsCheckedOut,
                                    IsGroupOfItems = u.IsGroupOfItems,
                                    ToolCategoryID = u.ToolCategoryID,
                                    ToolCategory = u.ToolCategory,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    Room = u.Room,
                                    RoomName = u.RoomName,
                                    Location = u.Location,
                                    LocationID = u.LocationID,
                                    GUID = u.GUID,
                                    CompanyID = u.CompanyID,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    CheckedOutQTY = u.CheckedOutQTY.GetValueOrDefault(0),
                                    CheckedOutMQTY = u.CheckedOutMQTY.GetValueOrDefault(0),
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    ToolUDF1 = u.UDF1,
                                    ToolUDF2 = u.UDF2,
                                    ToolUDF3 = u.UDF3,
                                    ToolUDF4 = u.UDF4,
                                    ToolUDF5 = u.UDF5,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
                                    MaintenanceType = u.MaintenanceType,
                                    Technician = u.Technician,
                                    TechnicianGuID = u.TechnicianGuID,
                                    CheckedOutQuantity = u.CheckedOutQuantity.GetValueOrDefault(0),
                                    CheckedInQuantity = u.CheckedInQuantity.GetValueOrDefault(0),
                                    ImageType = u.ImageType,
                                    ImagePath = u.ImagePath,
                                    ToolImageExternalURL = u.ToolImageExternalURL,
                                    MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                    Type = u.Type,
                                    IsBuildBreak = u.IsBuildBreak,
                                    AvailableToolQty = u.AvailableToolQty,
                                    ToolTypeTracking = u.ToolTypeTracking,
                                    SerialNumberTracking = u.SerialNumberTracking,
                                    LotNumberTracking = u.LotNumberTracking,
                                    DateCodeTracking = u.DateCodeTracking
                                }).AsParallel().ToList();
                    //   ObjCache = CacheHelper<IEnumerable<ToolMasterDTO>>.AddCacheItem("Cached_ToolMaster_" + CompanyID.ToString(), obj);
                }
                #endregion
            }

            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            IList<RoomDTO> ObjRoomCache = objRoomDAL.GetCachedData(CompanyID, false, false).ToList();
            UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);
            //IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllRecords(RoomID, CompanyID).ToList();

            //make changes for WI-304 issue
            IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllUsers().ToList();
            // //IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllUsers().ToList();

            ToolCategoryMasterDAL objToolMDAL = new ToolCategoryMasterDAL(base.DataBaseName);
            IList<ToolCategoryMasterDTO> ObjToolCateMCache = objToolMDAL.GetToolCategoryByRoomIDPlain(RoomID, CompanyID).ToList();
            LocationMasterDAL objLocMDAL = new LocationMasterDAL(base.DataBaseName);
            IList<LocationMasterDTO> ObjLocaMCache = objLocMDAL.GetAllRecords(RoomID, CompanyID, false, false).ToList();
            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
            IList<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID).ToList();

            //ToolCheckInOutHistoryDAL objTOCDAL = new ToolCheckInOutHistoryDAL(base.DataBaseName);
            //IList<ToolCheckInOutHistoryDTO> ObjTOCCache = objTOCDAL.GetAllRecords(RoomID, CompanyID).ToList();

            ObjCache = (from u in ObjCache
                        join im in ObjToolCateMCache on u.ToolCategoryID equals im.ID into u_IM_Join
                        from u_IM in u_IM_Join.DefaultIfEmpty()
                        join ps in ObjLocaMCache on u.LocationID equals ps.ID into u_PS_Join
                        from u_PS in u_PS_Join.DefaultIfEmpty()
                        join ur in ObjUserCache on u.CreatedBy equals ur.ID into u_UR_Join
                        from u_UR in u_UR_Join.DefaultIfEmpty()
                        join ur1 in ObjUserCache on u.LastUpdatedBy equals ur1.ID into u_UR1_Join
                        from u_UR1 in u_UR1_Join.DefaultIfEmpty()
                        join r in ObjRoomCache on u.Room equals r.ID into u_R_Join
                        from u_R in u_R_Join.DefaultIfEmpty()
                            //join u_toc in ObjTOCCache on u.ID equals u_toc.ToolID into u_toc_Join
                            //from u_TOC in u_toc_Join.DefaultIfEmpty()
                        join br in lstBarcodeDTO on u.GUID equals br.RefGUID into u_BR_Join
                        from u_BR in u_BR_Join.DefaultIfEmpty()

                        where u.CompanyID == CompanyID
                        select new ToolMasterDTO
                        {
                            ID = u.ID,
                            ToolName = u.ToolName,
                            Serial = u.Serial,
                            Description = u.Description,
                            Cost = u.Cost,
                            IsCheckedOut = u.IsCheckedOut,
                            IsGroupOfItems = u.IsGroupOfItems,
                            ToolCategoryID = u.ToolCategoryID,
                            Quantity = u.Quantity,
                            ToolCategory = u_IM != null ? u_IM.ToolCategory : string.Empty,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedByName = u_UR != null ? u_UR.UserName : string.Empty,//ur.CreatedByName,
                            UpdatedByName = u_UR1 != null ? u_UR1.UserName : string.Empty,//ur1.UpdatedByName,
                            RoomName = u_R != null ? u_R.RoomName : string.Empty,//r.RoomName,
                            Room = u.Room,
                            Location = u_PS != null ? u_PS.Location : string.Empty,
                            LocationID = u.LocationID,
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            CheckedOutQTY = u.CheckedOutQTY,//u_TOC != null ? u_TOC.CheckedOutQTY.GetValueOrDefault(0) : 0,
                            CheckedOutMQTY = u.CheckedOutMQTY,//u_TOC != null ? u_TOC.CheckedOutMQTY.GetValueOrDefault(0): 0,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            ToolUDF1 = u.UDF1,
                            ToolUDF2 = u.UDF2,
                            ToolUDF3 = u.UDF3,
                            ToolUDF4 = u.UDF4,
                            ToolUDF5 = u.UDF5,
                            AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Tool Master"),
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
                            MaintenanceType = u.MaintenanceType,
                            Technician = u.Technician,
                            CheckedOutQuantity = u.CheckedOutQuantity,
                            CheckedInQuantity = u.CheckedInQuantity,
                            ImageType = u.ImageType,
                            ImagePath = u.ImagePath,
                            ToolImageExternalURL = u.ToolImageExternalURL,
                            MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                            Type = u.Type,
                            IsBuildBreak = u.IsBuildBreak,
                            AvailableToolQty = u.AvailableToolQty,
                            ToolTypeTracking = u.ToolTypeTracking,
                            SerialNumberTracking = u.SerialNumberTracking,
                            LotNumberTracking = u.LotNumberTracking,
                            DateCodeTracking = u.DateCodeTracking
                        }).AsParallel().ToList();

            return ObjCache;
        }


        //ToolCheckoutStatusExportData
        public IEnumerable<ToolMasterDTO> ToolCheckoutStatusExportData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<ToolMasterDTO> ObjCache;

            #region Get All Data
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                List<ToolMasterDTO> lstresult = context.ExecuteStoreQuery<ToolMasterDTO>("exec ToolCheckoutStatus_Data @CompanyId,@RoomId,@IsDeleted,@IsArchived", params1).ToList();
                ObjCache = (from u in lstresult
                            select new ToolMasterDTO
                            {
                                ID = u.ID,
                                ToolName = u.ToolName,
                                Serial = u.Serial,
                                Description = u.Description,
                                Cost = u.Cost,
                                Quantity = u.Quantity,
                                IsCheckedOut = u.IsCheckedOut,
                                IsGroupOfItems = u.IsGroupOfItems,
                                ToolCategoryID = u.ToolCategoryID,
                                ToolCategory = u.ToolCategory,
                                Created = u.Created,
                                Updated = u.Updated,
                                CreatedBy = u.CreatedBy,
                                LastUpdatedBy = u.LastUpdatedBy,
                                CreatedByName = u.CreatedByName,
                                UpdatedByName = u.UpdatedByName,
                                Room = u.Room,
                                RoomName = u.RoomName,
                                Location = u.Location,
                                LocationID = u.LocationID,
                                GUID = u.GUID,
                                CompanyID = u.CompanyID,
                                IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                CheckedOutQTY = u.CheckedOutQTY.GetValueOrDefault(0),
                                CheckedOutMQTY = u.CheckedOutMQTY.GetValueOrDefault(0),
                                UDF1 = u.UDF1,
                                UDF2 = u.UDF2,
                                UDF3 = u.UDF3,
                                UDF4 = u.UDF4,
                                UDF5 = u.UDF5,
                                ToolUDF1 = u.UDF1,
                                ToolUDF2 = u.UDF2,
                                ToolUDF3 = u.UDF3,
                                ToolUDF4 = u.UDF4,
                                ToolUDF5 = u.UDF5,
                                AddedFrom = u.AddedFrom,
                                EditedFrom = u.EditedFrom,
                                ReceivedOn = u.ReceivedOn,
                                ReceivedOnWeb = u.ReceivedOnWeb,
                                NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
                                MaintenanceType = u.MaintenanceType,
                                Technician = u.Technician,
                                TechnicianGuID = u.TechnicianGuID,
                                CheckedOutQuantity = u.CheckedOutQuantity.GetValueOrDefault(0),
                                CheckedInQuantity = u.CheckedInQuantity.GetValueOrDefault(0),
                                ImageType = u.ImageType,
                                ImagePath = u.ImagePath,
                                ToolImageExternalURL = u.ToolImageExternalURL,
                                MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                Type = u.Type,
                                IsBuildBreak = u.IsBuildBreak,
                                AvailableToolQty = u.AvailableToolQty,
                                ToolTypeTracking = u.ToolTypeTracking,
                                SerialNumberTracking = u.SerialNumberTracking,
                                LotNumberTracking = u.LotNumberTracking,
                                DateCodeTracking = u.DateCodeTracking
                            }).AsParallel().ToList();
                //   ObjCache = CacheHelper<IEnumerable<ToolMasterDTO>>.AddCacheItem("Cached_ToolMaster_" + CompanyID.ToString(), obj);
            }
            #endregion


            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            IList<RoomDTO> ObjRoomCache = objRoomDAL.GetCachedData(CompanyID, false, false).ToList();
            UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);
            //IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllRecords(RoomID, CompanyID).ToList();

            //make changes for WI-304 issue
            IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllUsers().ToList();
            // //IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllUsers().ToList();

            ToolCategoryMasterDAL objToolMDAL = new ToolCategoryMasterDAL(base.DataBaseName);
            IList<ToolCategoryMasterDTO> ObjToolCateMCache = objToolMDAL.GetToolCategoryByRoomIDPlain(RoomID, CompanyID).ToList();
            LocationMasterDAL objLocMDAL = new LocationMasterDAL(base.DataBaseName);
            IList<LocationMasterDTO> ObjLocaMCache = objLocMDAL.GetAllRecords(RoomID, CompanyID, false, false).ToList();
            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
            IList<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID).ToList();

            //ToolCheckInOutHistoryDAL objTOCDAL = new ToolCheckInOutHistoryDAL(base.DataBaseName);
            //IList<ToolCheckInOutHistoryDTO> ObjTOCCache = objTOCDAL.GetAllRecords(RoomID, CompanyID).ToList();

            ObjCache = (from u in ObjCache
                        join im in ObjToolCateMCache on u.ToolCategoryID equals im.ID into u_IM_Join
                        from u_IM in u_IM_Join.DefaultIfEmpty()
                        join ps in ObjLocaMCache on u.LocationID equals ps.ID into u_PS_Join
                        from u_PS in u_PS_Join.DefaultIfEmpty()
                        join ur in ObjUserCache on u.CreatedBy equals ur.ID into u_UR_Join
                        from u_UR in u_UR_Join.DefaultIfEmpty()
                        join ur1 in ObjUserCache on u.LastUpdatedBy equals ur1.ID into u_UR1_Join
                        from u_UR1 in u_UR1_Join.DefaultIfEmpty()
                        join r in ObjRoomCache on u.Room equals r.ID into u_R_Join
                        from u_R in u_R_Join.DefaultIfEmpty()
                            //join u_toc in ObjTOCCache on u.ID equals u_toc.ToolID into u_toc_Join
                            //from u_TOC in u_toc_Join.DefaultIfEmpty()
                        join br in lstBarcodeDTO on u.GUID equals br.RefGUID into u_BR_Join
                        from u_BR in u_BR_Join.DefaultIfEmpty()

                        where u.CompanyID == CompanyID
                        select new ToolMasterDTO
                        {
                            ID = u.ID,
                            ToolName = u.ToolName,
                            Serial = u.Serial,
                            Description = u.Description,
                            Cost = u.Cost,
                            IsCheckedOut = u.IsCheckedOut,
                            IsGroupOfItems = u.IsGroupOfItems,
                            ToolCategoryID = u.ToolCategoryID,
                            Quantity = u.Quantity,
                            ToolCategory = u_IM != null ? u_IM.ToolCategory : string.Empty,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedByName = u_UR != null ? u_UR.UserName : string.Empty,//ur.CreatedByName,
                            UpdatedByName = u_UR1 != null ? u_UR1.UserName : string.Empty,//ur1.UpdatedByName,
                            RoomName = u_R != null ? u_R.RoomName : string.Empty,//r.RoomName,
                            Room = u.Room,
                            Location = u_PS != null ? u_PS.Location : string.Empty,
                            LocationID = u.LocationID,
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            CheckedOutQTY = u.CheckedOutQTY,//u_TOC != null ? u_TOC.CheckedOutQTY.GetValueOrDefault(0) : 0,
                            CheckedOutMQTY = u.CheckedOutMQTY,//u_TOC != null ? u_TOC.CheckedOutMQTY.GetValueOrDefault(0): 0,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            ToolUDF1 = u.UDF1,
                            ToolUDF2 = u.UDF2,
                            ToolUDF3 = u.UDF3,
                            ToolUDF4 = u.UDF4,
                            ToolUDF5 = u.UDF5,
                            AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Tool Master"),
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
                            MaintenanceType = u.MaintenanceType,
                            Technician = u.Technician,
                            CheckedOutQuantity = u.CheckedOutQuantity,
                            CheckedInQuantity = u.CheckedInQuantity,
                            ImageType = u.ImageType,
                            ImagePath = u.ImagePath,
                            ToolImageExternalURL = u.ToolImageExternalURL,
                            MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                            Type = u.Type,
                            IsBuildBreak = u.IsBuildBreak,
                            AvailableToolQty = u.AvailableToolQty,
                            ToolTypeTracking = u.ToolTypeTracking,
                            SerialNumberTracking = u.SerialNumberTracking,
                            LotNumberTracking = u.LotNumberTracking,
                            DateCodeTracking = u.DateCodeTracking
                        }).AsParallel().ToList();

            return ObjCache;
        }



        public IEnumerable<ToolMasterDTO> ToolCheckoutStatusExportDataNew(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<ToolMasterDTO> ObjCache;

            #region Get All Data
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                List<ToolMasterDTO> lstresult = context.ExecuteStoreQuery<ToolMasterDTO>("exec ToolCheckoutStatus_DataNew @CompanyId,@RoomId,@IsDeleted,@IsArchived", params1).ToList();
                ObjCache = (from u in lstresult
                            select new ToolMasterDTO
                            {
                                ID = u.ID,
                                ToolName = u.ToolName,
                                Serial = u.Serial,
                                Description = u.Description,
                                Cost = u.Cost,
                                Quantity = u.Quantity,
                                IsCheckedOut = u.IsCheckedOut,
                                IsGroupOfItems = u.IsGroupOfItems,
                                ToolCategoryID = u.ToolCategoryID,
                                ToolCategory = u.ToolCategory,
                                Created = u.Created,
                                Updated = u.Updated,
                                CreatedBy = u.CreatedBy,
                                LastUpdatedBy = u.LastUpdatedBy,
                                CreatedByName = u.CreatedByName,
                                UpdatedByName = u.UpdatedByName,
                                Room = u.Room,
                                RoomName = u.RoomName,
                                Location = u.Location,
                                LocationID = u.LocationID,
                                GUID = u.GUID,
                                CompanyID = u.CompanyID,
                                IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                CheckedOutQTY = u.CheckedOutQTY.GetValueOrDefault(0),
                                CheckedOutMQTY = u.CheckedOutMQTY.GetValueOrDefault(0),
                                UDF1 = u.UDF1,
                                UDF2 = u.UDF2,
                                UDF3 = u.UDF3,
                                UDF4 = u.UDF4,
                                UDF5 = u.UDF5,
                                ToolUDF1 = u.UDF1,
                                ToolUDF2 = u.UDF2,
                                ToolUDF3 = u.UDF3,
                                ToolUDF4 = u.UDF4,
                                ToolUDF5 = u.UDF5,
                                AddedFrom = u.AddedFrom,
                                EditedFrom = u.EditedFrom,
                                ReceivedOn = u.ReceivedOn,
                                ReceivedOnWeb = u.ReceivedOnWeb,
                                NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
                                MaintenanceType = u.MaintenanceType,
                                Technician = u.Technician,
                                TechnicianGuID = u.TechnicianGuID,
                                CheckedOutQuantity = u.CheckedOutQuantity.GetValueOrDefault(0),
                                CheckedInQuantity = u.CheckedInQuantity.GetValueOrDefault(0),
                                ImageType = u.ImageType,
                                ImagePath = u.ImagePath,
                                ToolImageExternalURL = u.ToolImageExternalURL,
                                MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                Type = u.Type,
                                IsBuildBreak = u.IsBuildBreak,
                                AvailableToolQty = u.AvailableToolQty,
                                ToolTypeTracking = u.ToolTypeTracking,
                                SerialNumberTracking = u.SerialNumberTracking,
                                LotNumberTracking = u.LotNumberTracking,
                                DateCodeTracking = u.DateCodeTracking,
                                ToolCheckoutGUID = u.ToolCheckoutGUID
                            }).AsParallel().ToList();
                //   ObjCache = CacheHelper<IEnumerable<ToolMasterDTO>>.AddCacheItem("Cached_ToolMaster_" + CompanyID.ToString(), obj);
            }
            #endregion


            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            IList<RoomDTO> ObjRoomCache = objRoomDAL.GetCachedData(CompanyID, false, false).ToList();
            UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);
            //IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllRecords(RoomID, CompanyID).ToList();

            //make changes for WI-304 issue
            IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllUsers().ToList();
            // //IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllUsers().ToList();

            ToolCategoryMasterDAL objToolMDAL = new ToolCategoryMasterDAL(base.DataBaseName);
            IList<ToolCategoryMasterDTO> ObjToolCateMCache = objToolMDAL.GetToolCategoryByRoomIDPlain(RoomID, CompanyID).ToList();
            LocationMasterDAL objLocMDAL = new LocationMasterDAL(base.DataBaseName);
            IList<LocationMasterDTO> ObjLocaMCache = objLocMDAL.GetAllRecords(RoomID, CompanyID, false, false).ToList();
            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
            IList<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID).ToList();

            //ToolCheckInOutHistoryDAL objTOCDAL = new ToolCheckInOutHistoryDAL(base.DataBaseName);
            //IList<ToolCheckInOutHistoryDTO> ObjTOCCache = objTOCDAL.GetAllRecords(RoomID, CompanyID).ToList();

            ObjCache = (from u in ObjCache
                        join im in ObjToolCateMCache on u.ToolCategoryID equals im.ID into u_IM_Join
                        from u_IM in u_IM_Join.DefaultIfEmpty()
                        join ps in ObjLocaMCache on u.LocationID equals ps.ID into u_PS_Join
                        from u_PS in u_PS_Join.DefaultIfEmpty()
                        join ur in ObjUserCache on u.CreatedBy equals ur.ID into u_UR_Join
                        from u_UR in u_UR_Join.DefaultIfEmpty()
                        join ur1 in ObjUserCache on u.LastUpdatedBy equals ur1.ID into u_UR1_Join
                        from u_UR1 in u_UR1_Join.DefaultIfEmpty()
                        join r in ObjRoomCache on u.Room equals r.ID into u_R_Join
                        from u_R in u_R_Join.DefaultIfEmpty()
                            //join u_toc in ObjTOCCache on u.ID equals u_toc.ToolID into u_toc_Join
                            //from u_TOC in u_toc_Join.DefaultIfEmpty()
                        join br in lstBarcodeDTO on u.GUID equals br.RefGUID into u_BR_Join
                        from u_BR in u_BR_Join.DefaultIfEmpty()

                        where u.CompanyID == CompanyID
                        select new ToolMasterDTO
                        {
                            ID = u.ID,
                            ToolName = u.ToolName,
                            Serial = u.Serial,
                            Description = u.Description,
                            Cost = u.Cost,
                            IsCheckedOut = u.IsCheckedOut,
                            IsGroupOfItems = u.IsGroupOfItems,
                            ToolCategoryID = u.ToolCategoryID,
                            Quantity = u.Quantity,
                            ToolCategory = u_IM != null ? u_IM.ToolCategory : string.Empty,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedByName = u_UR != null ? u_UR.UserName : string.Empty,//ur.CreatedByName,
                            UpdatedByName = u_UR1 != null ? u_UR1.UserName : string.Empty,//ur1.UpdatedByName,
                            RoomName = u_R != null ? u_R.RoomName : string.Empty,//r.RoomName,
                            Room = u.Room,
                            Location = u_PS != null ? u_PS.Location : string.Empty,
                            LocationID = u.LocationID,
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            CheckedOutQTY = u.CheckedOutQTY,//u_TOC != null ? u_TOC.CheckedOutQTY.GetValueOrDefault(0) : 0,
                            CheckedOutMQTY = u.CheckedOutMQTY,//u_TOC != null ? u_TOC.CheckedOutMQTY.GetValueOrDefault(0): 0,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            ToolUDF1 = u.UDF1,
                            ToolUDF2 = u.UDF2,
                            ToolUDF3 = u.UDF3,
                            ToolUDF4 = u.UDF4,
                            ToolUDF5 = u.UDF5,
                            AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Tool Master"),
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
                            MaintenanceType = u.MaintenanceType,
                            Technician = u.Technician,
                            CheckedOutQuantity = u.CheckedOutQuantity,
                            CheckedInQuantity = u.CheckedInQuantity,
                            ImageType = u.ImageType,
                            ImagePath = u.ImagePath,
                            ToolImageExternalURL = u.ToolImageExternalURL,
                            MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                            Type = u.Type,
                            IsBuildBreak = u.IsBuildBreak,
                            AvailableToolQty = u.AvailableToolQty,
                            ToolTypeTracking = u.ToolTypeTracking,
                            SerialNumberTracking = u.SerialNumberTracking,
                            LotNumberTracking = u.LotNumberTracking,
                            DateCodeTracking = u.DateCodeTracking,
                            ToolCheckoutGUID = u.ToolCheckoutGUID
                        }).AsParallel().ToList();

            return ObjCache;
        }





        public IEnumerable<ToolMasterDTO> ToolMasterExportData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<ToolMasterDTO> ObjCache;
            // if (IsArchived == false && IsDeleted == false)
            {
                #region Get All Data
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                    List<ToolMasterDTO> lstresult = context.ExecuteStoreQuery<ToolMasterDTO>("exec ToolMaster_AllData @CompanyId,@RoomId,@IsDeleted,@IsArchived", params1).ToList();
                    ObjCache = (from u in lstresult
                                select new ToolMasterDTO
                                {
                                    ID = u.ID,
                                    ToolName = u.ToolName,
                                    Serial = u.Serial,
                                    Description = u.Description,
                                    Cost = u.Cost,
                                    Quantity = u.Quantity,
                                    IsCheckedOut = u.IsCheckedOut,
                                    IsGroupOfItems = u.IsGroupOfItems,
                                    ToolCategoryID = u.ToolCategoryID,
                                    ToolCategory = u.ToolCategory,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    Room = u.Room,
                                    RoomName = u.RoomName,
                                    Location = u.Location,
                                    LocationID = u.LocationID,
                                    GUID = u.GUID,
                                    CompanyID = u.CompanyID,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    CheckedOutQTY = u.CheckedOutQTY.GetValueOrDefault(0),
                                    CheckedOutMQTY = u.CheckedOutMQTY.GetValueOrDefault(0),
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    ToolUDF1 = u.UDF1,
                                    ToolUDF2 = u.UDF2,
                                    ToolUDF3 = u.UDF3,
                                    ToolUDF4 = u.UDF4,
                                    ToolUDF5 = u.UDF5,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
                                    MaintenanceType = u.MaintenanceType,
                                    Technician = u.Technician,
                                    TechnicianGuID = u.TechnicianGuID,
                                    CheckedOutQuantity = u.CheckedOutQuantity.GetValueOrDefault(0),
                                    CheckedInQuantity = u.CheckedInQuantity.GetValueOrDefault(0),
                                    ImageType = u.ImageType,
                                    ImagePath = u.ImagePath,
                                    ToolImageExternalURL = u.ToolImageExternalURL,
                                    MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                    Type = u.Type,
                                    IsBuildBreak = u.IsBuildBreak,
                                    AvailableToolQty = u.AvailableToolQty,
                                    ToolTypeTracking = u.ToolTypeTracking,
                                    SerialNumberTracking = u.SerialNumberTracking,
                                    LotNumberTracking = u.LotNumberTracking,
                                    DateCodeTracking = u.DateCodeTracking
                                }).AsParallel().ToList();
                    //   ObjCache = CacheHelper<IEnumerable<ToolMasterDTO>>.AddCacheItem("Cached_ToolMaster_" + CompanyID.ToString(), obj);
                }
                #endregion
            }

            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            IList<RoomDTO> ObjRoomCache = objRoomDAL.GetCachedData(CompanyID, false, false).ToList();
            UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);
            //IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllRecords(RoomID, CompanyID).ToList();

            //make changes for WI-304 issue
            IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllUsers().ToList();
            // //IList<UserMasterDTO> ObjUserCache = objUserDAL.GetAllUsers().ToList();

            ToolCategoryMasterDAL objToolMDAL = new ToolCategoryMasterDAL(base.DataBaseName);
            IList<ToolCategoryMasterDTO> ObjToolCateMCache = objToolMDAL.GetToolCategoryByRoomIDPlain(RoomID, CompanyID).ToList();
            LocationMasterDAL objLocMDAL = new LocationMasterDAL(base.DataBaseName);
            IList<LocationMasterDTO> ObjLocaMCache = objLocMDAL.GetAllRecords(RoomID, CompanyID, false, false).ToList();
            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
            IList<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID).ToList();

            //ToolCheckInOutHistoryDAL objTOCDAL = new ToolCheckInOutHistoryDAL(base.DataBaseName);
            //IList<ToolCheckInOutHistoryDTO> ObjTOCCache = objTOCDAL.GetAllRecords(RoomID, CompanyID).ToList();

            ObjCache = (from u in ObjCache
                        join im in ObjToolCateMCache on u.ToolCategoryID equals im.ID into u_IM_Join
                        from u_IM in u_IM_Join.DefaultIfEmpty()
                        join ps in ObjLocaMCache on u.LocationID equals ps.ID into u_PS_Join
                        from u_PS in u_PS_Join.DefaultIfEmpty()
                        join ur in ObjUserCache on u.CreatedBy equals ur.ID into u_UR_Join
                        from u_UR in u_UR_Join.DefaultIfEmpty()
                        join ur1 in ObjUserCache on u.LastUpdatedBy equals ur1.ID into u_UR1_Join
                        from u_UR1 in u_UR1_Join.DefaultIfEmpty()
                        join r in ObjRoomCache on u.Room equals r.ID into u_R_Join
                        from u_R in u_R_Join.DefaultIfEmpty()
                            //join u_toc in ObjTOCCache on u.ID equals u_toc.ToolID into u_toc_Join
                            //from u_TOC in u_toc_Join.DefaultIfEmpty()
                        join br in lstBarcodeDTO on u.GUID equals br.RefGUID into u_BR_Join
                        from u_BR in u_BR_Join.DefaultIfEmpty()

                        where u.CompanyID == CompanyID
                        select new ToolMasterDTO
                        {
                            ID = u.ID,
                            ToolName = u.ToolName,
                            Serial = u.Serial,
                            Description = u.Description,
                            Cost = u.Cost,
                            IsCheckedOut = u.IsCheckedOut,
                            IsGroupOfItems = u.IsGroupOfItems,
                            ToolCategoryID = u.ToolCategoryID,
                            Quantity = u.Quantity,
                            ToolCategory = u_IM != null ? u_IM.ToolCategory : string.Empty,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedByName = u_UR != null ? u_UR.UserName : string.Empty,//ur.CreatedByName,
                            UpdatedByName = u_UR1 != null ? u_UR1.UserName : string.Empty,//ur1.UpdatedByName,
                            RoomName = u_R != null ? u_R.RoomName : string.Empty,//r.RoomName,
                            Room = u.Room,
                            Location = u_PS != null ? u_PS.Location : string.Empty,
                            LocationID = u.LocationID,
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            CheckedOutQTY = u.CheckedOutQTY,//u_TOC != null ? u_TOC.CheckedOutQTY.GetValueOrDefault(0) : 0,
                            CheckedOutMQTY = u.CheckedOutMQTY,//u_TOC != null ? u_TOC.CheckedOutMQTY.GetValueOrDefault(0): 0,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            ToolUDF1 = u.UDF1,
                            ToolUDF2 = u.UDF2,
                            ToolUDF3 = u.UDF3,
                            ToolUDF4 = u.UDF4,
                            ToolUDF5 = u.UDF5,
                            AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, u.GUID, "Tool Master"),
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            NoOfPastMntsToConsider = u.NoOfPastMntsToConsider,
                            MaintenanceType = u.MaintenanceType,
                            Technician = u.Technician,
                            CheckedOutQuantity = u.CheckedOutQuantity,
                            CheckedInQuantity = u.CheckedInQuantity,
                            ImageType = u.ImageType,
                            ImagePath = u.ImagePath,
                            ToolImageExternalURL = u.ToolImageExternalURL,
                            MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                            Type = u.Type,
                            IsBuildBreak = u.IsBuildBreak,
                            AvailableToolQty = u.AvailableToolQty,
                            ToolTypeTracking = u.ToolTypeTracking,
                            SerialNumberTracking = u.SerialNumberTracking,
                            LotNumberTracking = u.LotNumberTracking,
                            DateCodeTracking = u.DateCodeTracking
                        }).AsParallel().ToList();

            return ObjCache;
        }
    }
}
