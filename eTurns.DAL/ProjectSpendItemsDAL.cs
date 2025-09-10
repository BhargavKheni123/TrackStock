using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Web;
using eTurns.DTO.Resources;

namespace eTurns.DAL
{
    public class ProjectSpendItemsDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public ProjectSpendItemsDAL(base.DataBaseName)
        //{

        //}

        public ProjectSpendItemsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ProjectSpendItemsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public IEnumerable<ProjectSpendItemsDTO> GetProjectSpendItem(Guid ProjectSpendGUID, Int64 RoomID, Int64 CompanyId, List<long> SupplierIds, string ItemGUID)
        {
            List<ProjectSpendItemsDTO> lstProjectSpendItems = new List<ProjectSpendItemsDTO>();
            string strSupplierIds = string.Empty;
            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ProjectSpendGUID", ProjectSpendGUID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@SupplierIds", strSupplierIds),
                                                   new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value) };

                lstProjectSpendItems = (from u in context.Database.SqlQuery<ProjectSpendItemsDTO>("exec [GetProjectSpendItem] @ProjectSpendGUID,@RoomID,@CompanyID,@SupplierIds,@ItemGUID", params1)
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
            return lstProjectSpendItems;

        }

        public IEnumerable<ProjectSpendItemsDTO> GetProjectSpendItemCompareDate(Guid ProjectSpendGUID, Int64 RoomID, Int64 CompanyId, List<long> SupplierIds, string ItemGUID, string CompareDate)
        {
            List<ProjectSpendItemsDTO> lstProjectSpendItems = new List<ProjectSpendItemsDTO>();
            string strSupplierIds = string.Empty;
            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ProjectSpendGUID", ProjectSpendGUID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@SupplierIds", strSupplierIds),
                                                   new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value),
                                                   new SqlParameter("@CompareDate", CompareDate)};

                lstProjectSpendItems = (from u in context.Database.SqlQuery<ProjectSpendItemsDTO>("exec [GetProjectSpendItemCompareDate] @ProjectSpendGUID,@RoomID,@CompanyID,@SupplierIds,@ItemGUID,@CompareDate", params1)
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
            return lstProjectSpendItems;

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
        public IEnumerable<ProjectSpendItemsDTO> GetPagedProjectSpendItems(Guid ProjectGUID, int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName,
                                                 long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, string CurrentTimeZone)
        {
            TotalCount = 0;
            string strQuer = "";
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string ItemCreaters = null;
            string ItemUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                string[] FieldsPara = Fields[0].Split('~');
                DateTime FromdDate = DateTime.Now;
                DateTime ToDate = DateTime.Now;

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    ItemCreaters = FieldsPara[0];//.TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ItemUpdators = FieldsPara[1];//.TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[4].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[4].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    string[] arrReplenishTypes = FieldsPara[9].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    string[] arrReplenishTypes = FieldsPara[10].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }

                if (Fields.Length > 1)
                {
                    if (!string.IsNullOrEmpty(Fields[1]))
                        SearchTerm = Fields[1];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }
            }

            strQuer = @"EXEC [GetPagedProjectSpendItems] @ProjectGUID,@CompnayID,@RoomID,@IsDeleted,@IsArchived,@StartRowIndex,@MaxRows,@sortColumnName,@SearchTerm,@ItemCreaters,@ItemUpdators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5";

            List<SqlParameter> sqlParas = new List<SqlParameter>();
            sqlParas.Add(new SqlParameter("@ProjectGUID", ProjectGUID)); //
            sqlParas.Add(new SqlParameter("@CompnayID", CompanyID));
            sqlParas.Add(new SqlParameter("@RoomID", RoomID));
            sqlParas.Add(new SqlParameter("@IsDeleted", IsDeleted));
            sqlParas.Add(new SqlParameter("@IsArchived", IsArchived));
            sqlParas.Add(new SqlParameter("@StartRowIndex", StartRowIndex));
            sqlParas.Add(new SqlParameter("@MaxRows", MaxRows));

            if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName != "null")
                sqlParas.Add(new SqlParameter("@sortColumnName", sortColumnName));
            else
                sqlParas.Add(new SqlParameter("@sortColumnName", DBNull.Value));

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm != "null")
                sqlParas.Add(new SqlParameter("@SearchTerm", SearchTerm));
            else
                sqlParas.Add(new SqlParameter("@SearchTerm", DBNull.Value));


            sqlParas.Add(new SqlParameter("@ItemCreaters", ItemCreaters ?? string.Empty));
            sqlParas.Add(new SqlParameter("@ItemUpdators", ItemUpdators ?? string.Empty));
            sqlParas.Add(new SqlParameter("@CreatedDateFrom", CreatedDateFrom ?? string.Empty));
            sqlParas.Add(new SqlParameter("@CreatedDateTo", CreatedDateTo ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UpdatedDateTo", UpdatedDateTo ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UDF1", UDF1 ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UDF2", UDF2 ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UDF3", UDF3 ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UDF4", UDF4 ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UDF5", UDF5 ?? string.Empty));

            var params1 = sqlParas.ToArray();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var projectspendItems = context.Database.SqlQuery<ProjectSpendItemsDTO>(strQuer, params1).ToList();

                if (projectspendItems != null && projectspendItems.Any() && projectspendItems.Count() > 0)
                {
                    TotalCount = projectspendItems.ElementAt(0).TotalRecords;
                }
                return projectspendItems;
            }
        }

        public ProjectSpendItemsDTO GetProjectSpendItemByGuidPlain(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
                return context.Database.SqlQuery<ProjectSpendItemsDTO>("exec [GetProjectSpendItemByGuidPlain] @GUID", params1).FirstOrDefault();
            }
        }

        public List<ProjectSpendItemsDTO> GetHistoryRecordByProjectID(Guid ProjectGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramskts = new SqlParameter[] { new SqlParameter("@ProjectGUID", Convert.ToString(ProjectGUID)) };

                return (from u in context.Database.SqlQuery<ProjectSpendItemsDTO>("exec [GetProjectSpendItemHistory] @ProjectGUID", paramskts)
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

        /// <summary>
        /// Insert Record in the DataBase ProjectSpendItems
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(ProjectSpendItemsDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ProjectSpendItem obj = new ProjectSpendItem();
                obj.ID = 0;
                obj.ProjectGUID = objDTO.ProjectGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.QuantityLimit = objDTO.QuantityLimit;
                obj.QuantityUsed = objDTO.QuantityUsed;
                obj.DollarLimitAmount = objDTO.DollarLimitAmount;
                obj.DollarUsedAmount = objDTO.DollarUsedAmount;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.Created = objDTO.Created;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = objDTO.IsDeleted;
                obj.IsArchived = objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                objDTO.GUID = Guid.NewGuid();
                obj.GUID = objDTO.GUID;

                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.AddedFrom = objDTO.AddedFrom = "Web";
                obj.EditedFrom = objDTO.EditedFrom = "Web";

                context.ProjectSpendItems.Add(obj);
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
        public bool Edit(ProjectSpendItemsDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue && (bool)objDTO.IsDeleted;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ProjectSpendItem obj = context.ProjectSpendItems.Where(x => x.GUID == objDTO.GUID).FirstOrDefault();//GetRecord(objDTO.GUID, (Int64)objDTO.Room, (Int64)objDTO.CompanyID);

                if (objDTO.QuantityLimit != null)
                {
                    obj.QuantityLimit = objDTO.QuantityLimit;
                }

                if (objDTO.QuantityUsed != null)
                {
                    obj.QuantityUsed = objDTO.QuantityUsed;
                }

                if (objDTO.DollarLimitAmount != null)
                {
                    obj.DollarLimitAmount = objDTO.DollarLimitAmount;
                }

                if (objDTO.DollarUsedAmount != null)
                {
                    obj.DollarUsedAmount = objDTO.DollarUsedAmount;
                }

                obj.LastUpdated = objDTO.LastUpdated;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;

                if (objDTO.IsDeleted != null)
                {
                    obj.IsDeleted = objDTO.IsDeleted;
                }

                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.EditedFrom = objDTO.EditedFrom;

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
        public bool DeleteProjectSpendItemsByGuids(string IDs, long UserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string Guids = string.Empty;

                if (!string.IsNullOrEmpty(IDs) && !string.IsNullOrWhiteSpace(IDs))
                {
                    var arr = IDs.Split(',');

                    if (arr != null && arr.Any())
                    {
                        Guids = string.Join(",", arr);
                    }
                }

                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserId),
                                                    new SqlParameter("@Guids", Guids)
                                                };

                context.Database.ExecuteSqlCommand("exec [DeleteProjectSpendItemsByGuids] @UserID,@Guids", params1);
                return true;
            }
        }

        public int GetProjectSpendItemsCount(Guid ProjectSpendGUID, long RoomId, long CompanyId, Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@ProjectSpendGUID", ProjectSpendGUID),
                                                    new SqlParameter("@RoomId", RoomId),
                                                    new SqlParameter("@CompanyId", CompanyId),
                                                    new SqlParameter("@ItemGUID", ItemGuid)
                                                 };
                return context.Database.SqlQuery<int>("exec [GetProjectSpendItemsCount] @ProjectSpendGUID,@RoomId,@CompanyId,@ItemGUID", params1).FirstOrDefault();
            }
        }

        public ProjectSpendItemsDTO GetSingleProjectSpendItemByItemGuidPlain(Guid ProjectSpendGUID, long RoomId, long CompanyId, Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@ProjectSpendGUID", ProjectSpendGUID),
                                                    new SqlParameter("@RoomId", RoomId),
                                                    new SqlParameter("@CompanyId", CompanyId),
                                                    new SqlParameter("@ItemGUID", ItemGuid)
                                                 };
                return context.Database.SqlQuery<ProjectSpendItemsDTO>("exec [GetSingleProjectSpendItemByItemGuidPlain] @ProjectSpendGUID,@RoomId,@CompanyId,@ItemGUID", params1).FirstOrDefault();
            }
        }
    }
}


