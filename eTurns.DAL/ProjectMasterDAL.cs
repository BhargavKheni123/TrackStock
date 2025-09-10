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
    public partial class ProjectMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ProjectMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ProjectMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        #endregion

        public ProjectMasterDTO GetDefaultProjectSpendRecord(long RoomID, long CompanyId, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                return context.Database.SqlQuery<ProjectMasterDTO>("exec [GetDefaultProjectSpendRecord] @RoomID,@CompanyID,@IsDeleted,@IsArchived", params1).FirstOrDefault();
            }
        }

        public IEnumerable<ProjectMasterDTO> GetPagedProjectMasterRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyId, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<ProjectMasterDTO> lstItems = new List<ProjectMasterDTO>();
            TotalCount = 0;
            ProjectMasterDTO objItemDTO = new ProjectMasterDTO();
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string Creators = null;
            string Updators = null;
            string PercentSpend = null;
            string TotalSpendFrom = null;
            string TotalSpendTo = null;
            string TotalSpendRemainingFrom = null;
            string TotalSpendRemainingTo = null;
            string TotalSpendRemainingItemFrom = null;
            string TotalSpendRemainingItemTo = null;

            if ((SearchTerm ?? string.Empty).Contains("[###]"))
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

                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    Updators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    Creators = FieldsPara[0].TrimEnd(',');
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
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrEmpty(FieldsPara[16]) && !string.IsNullOrWhiteSpace(FieldsPara[16]))
                {
                    PercentSpend = FieldsPara[16];
                }
                if (!string.IsNullOrEmpty(FieldsPara[17]) && !string.IsNullOrWhiteSpace(FieldsPara[17]))
                {
                    var TotalSpend = FieldsPara[17].Split('-');
                    TotalSpendFrom = TotalSpend[0];
                    if (TotalSpend.Length > 1)
                    {
                        TotalSpendTo = TotalSpend[1];
                    }
                }
                if (!string.IsNullOrEmpty(FieldsPara[18]) && !string.IsNullOrWhiteSpace(FieldsPara[18]))
                {
                    var TotalSpendRemaining = FieldsPara[18].Split(',');
                    TotalSpendRemainingFrom = TotalSpendRemaining[0];
                    if (TotalSpendRemaining.Length > 1)
                    {
                        TotalSpendRemainingTo = TotalSpendRemaining[1];
                    }
                }
                if (!string.IsNullOrEmpty(FieldsPara[19]) && !string.IsNullOrWhiteSpace(FieldsPara[19]))
                {
                    var TotalSpendRemainingItem = FieldsPara[19].Split(',');
                    TotalSpendRemainingItemFrom = TotalSpendRemainingItem[0];
                    if (TotalSpendRemainingItem.Length > 1)
                    {
                        TotalSpendRemainingItemTo = TotalSpendRemainingItem[1];
                    }
                }
            }

            var params1 = new SqlParameter[] { new SqlParameter("@StartRowIndex", StartRowIndex), 
                                               new SqlParameter("@MaxRows", MaxRows), 
                                               new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value), 
                                               new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value), 
                                               new SqlParameter("@CreatedFrom", CreatedDateFrom ?? (object)DBNull.Value), 
                                               new SqlParameter("@CreatedTo", CreatedDateTo ?? (object)DBNull.Value), 
                                               new SqlParameter("@CreatedBy", Creators ?? (object)DBNull.Value), 
                                               new SqlParameter("@UpdatedFrom", UpdatedDateFrom ?? (object)DBNull.Value), 
                                               new SqlParameter("@UpdatedTo", UpdatedDateTo ?? (object)DBNull.Value), 
                                               new SqlParameter("@LastUpdatedBy", Updators ?? (object)DBNull.Value), 
                                               new SqlParameter("@Room", RoomID), 
                                               new SqlParameter("@IsDeleted", IsDeleted), 
                                               new SqlParameter("@IsArchived", IsArchived), 
                                               new SqlParameter("@CompanyID", CompanyId), 
                                               new SqlParameter("@UDF1", UDF1 ?? (object)DBNull.Value), 
                                               new SqlParameter("@UDF2", UDF2 ?? (object)DBNull.Value), 
                                               new SqlParameter("@UDF3", UDF3 ?? (object)DBNull.Value), 
                                               new SqlParameter("@UDF4", UDF4 ?? (object)DBNull.Value), 
                                               new SqlParameter("@UDF5", UDF5 ?? (object)DBNull.Value), 
                                               new SqlParameter("@PercentSpend", PercentSpend ?? (object)DBNull.Value), 
                                               new SqlParameter("@TotalSpendFrom", TotalSpendFrom ?? (object)DBNull.Value),
                                               new SqlParameter("@TotalSpendTo", TotalSpendTo ?? (object)DBNull.Value),
                                               new SqlParameter("@TotalSpendRemainingFrom", TotalSpendRemainingFrom ?? (object)DBNull.Value),
                                               new SqlParameter("@TotalSpendRemainingTo", TotalSpendRemainingTo ?? (object)DBNull.Value),
                                               new SqlParameter("@TotalSpendRemainingItemFrom", TotalSpendRemainingItemFrom ?? (object)DBNull.Value),
                                               new SqlParameter("@TotalSpendRemainingItemTo", TotalSpendRemainingItemTo ?? (object)DBNull.Value)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItems = context.Database.SqlQuery<ProjectMasterDTO>("EXEC dbo.GetPagedProjectMaster @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@CreatedBy,@UpdatedFrom,@UpdatedTo,@LastUpdatedBy,@Room,@IsDeleted,@IsArchived,@CompanyID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@PercentSpend,@TotalSpendFrom,@TotalSpendTo,@TotalSpendRemainingFrom,@TotalSpendRemainingTo,@TotalSpendRemainingItemFrom,@TotalSpendRemainingItemTo", params1).ToList();
                TotalCount = 0;
                if (lstItems != null && lstItems.Count > 0)
                {
                    TotalCount = lstItems.First().TotalRecords;
                }

                return lstItems;
            }
        }

        public ProjectMasterDTO GetProjectMasterByIDNormal(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", Id) };
                return context.Database.SqlQuery<ProjectMasterDTO>("exec [GetProjectMasterByIDNormal] @ID", params1).FirstOrDefault();
            }
        }
        
        public ProjectMasterDTO GetRecord(Guid ProjectSpendGUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", ProjectSpendGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                return context.Database.SqlQuery<ProjectMasterDTO>("exec [GetProjectMasterByGUID] @GUID,@RoomID,@CompanyID,@IsDeleted,@IsArchived", params1).FirstOrDefault();
            }
        }
        
        public IEnumerable<ProjectMasterDTO> GetProjectsForReport(long[] arrRoomIDs, long[] arrCompanyIds, string[] filterStatus, string fromDate, string ToDate)
        {
            string RoomIDs = string.Empty;
            string CompanyIds = string.Empty;
            string FilterStatuses = string.Empty;
            string strFromDate = string.Empty;
            string strToDate = string.Empty;

            if (arrRoomIDs != null && arrRoomIDs.Length > 0)
            {
                RoomIDs = string.Join(",", arrRoomIDs);
            }

            if (arrCompanyIds != null && arrCompanyIds.Length > 0)
            {
                CompanyIds = string.Join(",", arrCompanyIds);
            }

            if (filterStatus != null && filterStatus.Length > 0)
            {
                FilterStatuses = string.Join(",", filterStatus);
            }
            if (!string.IsNullOrEmpty(fromDate) && Convert.ToDateTime(fromDate) > DateTime.MinValue)
            {
                strFromDate = Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd HH:mm:ss");                
            }
            if (!string.IsNullOrEmpty(ToDate) && Convert.ToDateTime(ToDate) > DateTime.MinValue)
            {
                strToDate = Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd HH:mm:ss");
            }
            
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomIDs", RoomIDs ?? (object)DBNull.Value),
                                                   new SqlParameter("@CompanyIDs", CompanyIds ?? (object)DBNull.Value),
                                                   new SqlParameter("@StartDate", strFromDate ?? (object)DBNull.Value),
                                                   new SqlParameter("@EndDate", strToDate ?? (object)DBNull.Value),
                                                   new SqlParameter("@FilterStatuses", FilterStatuses ?? (object)DBNull.Value)
                                                   
                                                 };
                return context.Database.SqlQuery<ProjectMasterDTO>("exec [GetProjectsForReportWithFiltersPlain] @RoomIDs,@CompanyIDs,@StartDate,@EndDate,@FilterStatuses", params1).ToList();
            }          
            
        }

        public ProjectMasterDTO GetProjectHistoryByIdNormal(long id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", id) };
                return context.Database.SqlQuery<ProjectMasterDTO>("exec [GetProjectHistoryByIdNormal] @ID", params1).FirstOrDefault();
            }
        }
        public Int64 Insert(ProjectMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ProjectMaster obj = new ProjectMaster();
                obj.ID = 0;
                objDTO.GUID = Guid.NewGuid();
                obj.GUID = objDTO.GUID;

                if (objDTO.TrackAllUsageAgainstThis)
                {
                    objDTO.IsClosed = false;
                }

                obj.ProjectSpendName = objDTO.ProjectSpendName;
                obj.DollarLimitAmount = objDTO.DollarLimitAmount;
                obj.DollarUsedAmount = objDTO.DollarUsedAmount;
                obj.TrackAllUsageAgainstThis = objDTO.TrackAllUsageAgainstThis;
                obj.Description = objDTO.Description;
                obj.IsClosed = objDTO.IsClosed;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;

                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Project Spend";

                obj.WhatWhereAction = objDTO.WhatWhereAction;
                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                if (string.IsNullOrWhiteSpace(objDTO.AddedFrom))
                {
                    obj.AddedFrom = objDTO.AddedFrom = "Web";
                }
                else
                {
                    obj.AddedFrom = objDTO.AddedFrom;
                }

                if (string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                {
                    obj.EditedFrom = objDTO.EditedFrom = "Web";
                }
                else
                {
                    obj.EditedFrom = objDTO.EditedFrom;
                }

                context.ProjectMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                if (objDTO.ID > 0)
                {
                    new AutoSequenceDAL(base.DataBaseName).UpdateRoomDetailForNextAutoNumberByModule("NextProjectSpendNo", objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.ProjectSpendName);

                    if (objDTO.ProjectSpendItems != null && objDTO.ProjectSpendItems.Count > 0)
                    {
                        ProjectSpendItemsDAL objSPI = new ProjectSpendItemsDAL(base.DataBaseName);
                        foreach (var item in objDTO.ProjectSpendItems)
                        {
                            if (item.ID <= 0)
                            {
                                item.ProjectGUID = obj.GUID;
                                objSPI.Insert(item);
                            }
                        }
                    }
                }

                //unique tracking on for single project
                if (obj.ID > 0 && objDTO.TrackAllUsageAgainstThis)
                {
                    UpdateDefaultProjectSpend(objDTO.ID, objDTO.LastUpdatedBy.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Room.GetValueOrDefault(0));
                }

                return obj.ID;
            }
        }
        public bool Edit(ProjectMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            if (objDTO.TrackAllUsageAgainstThis)
                objDTO.IsClosed = false;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ProjectMaster obj = context.ProjectMasters.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                obj.ID = objDTO.ID;
                obj.ProjectSpendName = objDTO.ProjectSpendName;
                obj.IsClosed = objDTO.IsClosed;
                obj.Description = objDTO.Description;
                obj.DollarLimitAmount = objDTO.DollarLimitAmount;
                obj.DollarUsedAmount = objDTO.DollarUsedAmount;
                obj.TrackAllUsageAgainstThis = objDTO.TrackAllUsageAgainstThis;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.GUID = objDTO.GUID;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;

                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Project Spend";

                obj.WhatWhereAction = objDTO.WhatWhereAction;

                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                context.SaveChanges();


                if (objDTO.ProjectSpendItems != null && objDTO.ProjectSpendItems.Count > 0)
                {
                    foreach (var item in objDTO.ProjectSpendItems)
                    {
                        ProjectSpendItemsDAL objSPI = new ProjectSpendItemsDAL(base.DataBaseName);
                        if (item.ID <= 0)
                        {
                            item.ProjectGUID = obj.GUID;
                            objSPI.Insert(item);
                        }
                        else
                        {
                            item.ReceivedOn = DateTimeUtility.DateTimeNow;
                            item.EditedFrom = "Web";
                            item.ProjectGUID = obj.GUID;
                            objSPI.Edit(item);
                        }
                    }
                }

                //unique tracking on for single project
                if (objDTO.TrackAllUsageAgainstThis)
                {
                    UpdateDefaultProjectSpend(objDTO.ID, objDTO.LastUpdatedBy.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Room.GetValueOrDefault(0));
                }

                return true;
            }
        }

        public bool DeleteProjectSpend(string IDs, long UserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserId", UserId), new SqlParameter("@IDs", IDs) };
                context.Database.ExecuteSqlCommand("exec [DeleteProjectSpend] @UserId,@IDs", params1);
                return true;
            }
        }
        public string UpdateProjectSpend(long Id, string Value, string ColumnName, long UserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id), new SqlParameter("@UserId", UserId), new SqlParameter("@ColumnName", ColumnName), new SqlParameter("@ColumnValue", Value) };
                context.Database.ExecuteSqlCommand("exec [UpdateProjectMaster] @Id,@UserId,@ColumnName,@ColumnValue", params1);
            }
            return Value;
        }
        public ProjectMasterDTO GetProjectByName(string ProjectName, long RoomID, long CompanyID, bool? IsClosed)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter sqlParam;
                if (IsClosed.HasValue)
                {
                    sqlParam = new SqlParameter("@IsClosed", IsClosed);
                }
                else
                {
                    sqlParam = new SqlParameter("@IsClosed", (object)DBNull.Value);
                }
                var params1 = new SqlParameter[] { new SqlParameter("@ProjectName", ProjectName), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), sqlParam };
                return context.Database.SqlQuery<ProjectMasterDTO>("exec [GetProjectByName] @ProjectName,@RoomID,@CompanyID,@IsClosed", params1).FirstOrDefault();
            }
        }
        public List<ProjectMasterMain> GetProjectExport(Int64 RoomId, Int64 CompanyID, string Ids, List<long> UserSupplierIds)
        {
            List<ProjectMasterMain> objProjectMasterDTO = new List<ProjectMasterMain>();
            string strSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", UserSupplierIds);
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IDs", Ids), new SqlParameter("@UserSupplierIds", strSupplierIds) };
                return context.Database.SqlQuery<ProjectMasterMain>("exec Project_Export @RoomID,@CompanyID,@IDs,@UserSupplierIds", params1).ToList();
            }
        }

        /// <summary>
        /// This method is used to get the data for the narrow search for the Project list
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <param name="NarrowSearchKey"></param>
        /// <returns></returns>
        public List<NarrowSearchDTO> GetProjectListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? string.Empty) };
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetProjectListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }

        /// <summary>
        /// This method is used to get the all projects(plain - > without joining other tables) for the specified company and room.
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyId"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <returns></returns>
        public IEnumerable<ProjectMasterDTO> GetAllProjectMasterByRoomPlain(long RoomID, long CompanyId, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId) };
                return context.Database.SqlQuery<ProjectMasterDTO>("exec [GetAllUnclosedProjectsPlain] @RoomID,@CompanyID", params1).ToList();
            }
        }

        public long GetTotalProjectCountByRoom(long RoomID, long CompanyId, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                return context.Database.SqlQuery<long>("exec [GetTotalProjectCountByRoom] @RoomID,@CompanyID,@IsDeleted,@IsArchived", params1).FirstOrDefault();
            }
        }

        public void UpdateDefaultProjectSpend(long Id, long UserId, long CompanyId, long RoomId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id), new SqlParameter("@UserId", UserId), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@RoomId", RoomId) };
                context.Database.ExecuteSqlCommand("exec [UpdateDefaultProjectSpend] @Id,@UserId,@CompanyId,@RoomId", params1);
            }
        }
        public List<ProjectMasterDTO> GetProjectSpendMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ProjectMasterDTO>("exec [GetProjectSpendMasterChangeLog] @ID", params1).ToList();
            }
        }

        public ProjectMasterDTO GetProjectMasterByGuidNormal(Guid ProjectSpendGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", ProjectSpendGUID) };
                return context.Database.SqlQuery<ProjectMasterDTO>("exec [GetProjectMasterByGuidNormal] @GUID", params1).FirstOrDefault();
            }
        }

        public ProjectMasterDTO GetProjectspendByName(string ProjectName, long RoomID, long CompanyID, bool IsDeleted, bool IsArchived, bool? IsClosed)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SqlParameter sqlParam;
                if (IsClosed.HasValue)
                {
                    sqlParam = new SqlParameter("@IsClosed", IsClosed);
                }
                else
                {
                    sqlParam = new SqlParameter("@IsClosed", (object)DBNull.Value);
                }
                var params1 = new SqlParameter[] { new SqlParameter("@ProjectName", ProjectName), 
                              new SqlParameter("@RoomID", RoomID), 
                              new SqlParameter("@CompanyID", CompanyID),
                              new SqlParameter("@IsDeleted", IsDeleted), 
                              new SqlParameter("@IsArchived", IsArchived), sqlParam };
                return context.Database.SqlQuery<ProjectMasterDTO>("exec [GetProjectspendByName] @ProjectName,@RoomID,@CompanyID,@IsDeleted,@IsArchived,@IsClosed", params1).FirstOrDefault();
            }
        }
    }
}
