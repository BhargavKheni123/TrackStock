using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class PullPOMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public PullPOMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public PullPOMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}
        #endregion

        public long Update(PullPOMasterDTO objDTO)
        {
            PullPOMaster objPullPOMaster = new PullPOMaster();

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objPullPOMaster = context.PullPOMasters.FirstOrDefault(t => t.Id == objDTO.ID);

                if (objPullPOMaster != null)
                {
                    objPullPOMaster.PullOrderNumber = objDTO.PullOrderNumber;
                    objPullPOMaster.IsActive = objDTO.IsActive;
                    objPullPOMaster.Updated = DateTimeUtility.DateTimeNow;
                    objPullPOMaster.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objPullPOMaster.EditedFrom = "Web";
                    objPullPOMaster.UpdatedBy = objDTO.UpdatedBy;
                }

                context.SaveChanges();
                return objDTO.ID;
            }
        }

        public long Insert(PullPOMasterDTO objDTO)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    PullPOMaster obj = new PullPOMaster()
                    {
                        PullOrderNumber = objDTO.PullOrderNumber,
                        CompanyID = objDTO.CompanyID,
                        CreatedBy = objDTO.CreatedBy,
                        Created = DateTimeUtility.DateTimeNow,
                        Id = 0,
                        IsActive = objDTO.IsActive,
                        Isdeleted = false,
                        RoomId = objDTO.RoomId,
                        UpdatedBy = objDTO.UpdatedBy,
                        Updated = DateTimeUtility.DateTimeNow,
                        AddedFrom = objDTO.AddedFrom,
                        EditedFrom = objDTO.EditedFrom,
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow,

                    };

                    context.PullPOMasters.Add(obj);
                    context.SaveChanges();
                    objDTO.ID = obj.Id;
                }

                return objDTO.ID;
            }
            catch
            {
                return 0;
            }
        }

        public PullPOMasterDTO GetPullPOMasterByIdFull(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<PullPOMasterDTO>("exec [GetPullPOMasterByIdFull] @Id", params1).FirstOrDefault();
            }
        }

        public long UpdateStatus(long ID, bool IsActive, long userid)
        {
            PullPOMaster objPullPOMaster = new PullPOMaster();

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objPullPOMaster = context.PullPOMasters.FirstOrDefault(t => t.Id == ID);

                if (objPullPOMaster != null)
                {
                    objPullPOMaster.IsActive = IsActive;
                    objPullPOMaster.Updated = DateTimeUtility.DateTimeNow;
                    objPullPOMaster.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objPullPOMaster.UpdatedBy = userid;
                    objPullPOMaster.EditedFrom = "Web";
                }
                else
                {
                    return 0;
                }

                context.SaveChanges();
                return ID;
            }
        }

        /// <summary>
        /// This method is used to delete or undelete Pull PO Master record
        /// </summary>
        /// <param name="IDs">List of Ids to Delete or UnDelete</param>
        /// <param name="UserId">UserId</param>
        /// <param name="IsDeleted">Whether to delete or undelete the record.(value of IsDeleted Column)</param>
        public void DeleteOrUnDeletePullPOMaster(string IDs, long UserId, bool IsDeleted)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Ids", IDs),
                                                   new SqlParameter("@UserId", UserId),
                                                   new SqlParameter("@IsDeleted", IsDeleted)
                                                };
                context.Database.ExecuteSqlCommand("EXEC [DeleteOrUnDeletePullPOMaster] @Ids,@UserId,@IsDeleted", params1);
            }
        }

        public IEnumerable<PullPOMasterDTO> GetPagedRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string SortColumnName, long RoomId, long CompanyId, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<PullPOMasterDTO> lstPullPO = new List<PullPOMasterDTO>();
            TotalCount = 0;
            string CreatedByName = null;
            string UpdatedByName = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string Status = null;

            if (!string.IsNullOrWhiteSpace(SearchTerm) && SearchTerm.Contains("[###]"))
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
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[85]))
                {
                    Status = FieldsPara[85];
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
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@StartRowIndex",StartRowIndex)
                                                    , new SqlParameter("@MaxRows",MaxRows)
                                                    , new SqlParameter("@SearchTerm",SearchTerm ?? (object)DBNull.Value)
                                                    , new SqlParameter("@sortColumnName",SortColumnName)
                                                    , new SqlParameter("@CreatedByName",CreatedByName ?? (object)DBNull.Value)
                                                    , new SqlParameter("@UpdatedByName",UpdatedByName ?? (object)DBNull.Value)
                                                    , new SqlParameter("@CreatedDateFrom",CreatedDateFrom ?? (object)DBNull.Value)
                                                    , new SqlParameter("@CreatedDateTo",CreatedDateTo ?? (object)DBNull.Value)
                                                    , new SqlParameter("@UpdatedDateFrom",UpdatedDateFrom ?? (object)DBNull.Value)
                                                    , new SqlParameter("@UpdatedDateTo",UpdatedDateTo ?? (object)DBNull.Value)
                                                    , new SqlParameter("@IsDeleted",IsDeleted)
                                                    , new SqlParameter("@IsArchived",IsArchived)
                                                    , new SqlParameter("@RoomID",RoomId )
                                                    , new SqlParameter("@CompanyID",CompanyId)
                                                    , new SqlParameter("@Status",Status ?? (object)DBNull.Value)
                                                };

                lstPullPO = context.Database.SqlQuery<PullPOMasterDTO>("EXEC dbo.GetPagedPullPOMaster @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedByName,@UpdatedByName,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@IsDeleted,@IsArchived,@RoomID,@CompanyID,@Status", params1).ToList();

                if (lstPullPO != null && lstPullPO.Count() > 0)
                {
                    TotalCount = lstPullPO.First().TotalRecords ?? 0;
                }
            }

            return lstPullPO;
        }

        public bool DuplicateRecordCheck(long Id, string PullOrderNumber, long RoomId, long CompanyId)
        {
            PullPOMaster objPullPOMaster = new PullPOMaster();
            try
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    objPullPOMaster = context.PullPOMasters.FirstOrDefault(t => t.Id != Id && t.RoomId == RoomId && t.CompanyID == CompanyId && t.Isdeleted == false && t.PullOrderNumber == PullOrderNumber);
                    return (objPullPOMaster != null) ? false : true;
                }
            }
            catch
            {
                return false;
            }

        }
    }
}
