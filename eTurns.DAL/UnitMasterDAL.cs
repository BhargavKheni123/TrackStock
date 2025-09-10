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
    public partial class UnitMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public UnitMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public UnitMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public UnitMasterDTO GetUnitByGUIDPlain(Guid GUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitByGUIDPlain] @GUID", params1).FirstOrDefault();
            }
        }

        public UnitMasterDTO GetUnitByGUIDNormal(Guid GUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitByGUIDNormal] @GUID", params1).FirstOrDefault();
            }
        }

        public UnitMasterDTO GetUnitByIDPlain(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitByIDPlain] @ID", params1).FirstOrDefault();
            }
        }

        public UnitMasterDTO GetUnitByIDNormal(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitByIDNormal] @ID", params1).FirstOrDefault();
            }
        }

        public List<UnitMasterDTO> GetUnitByGUIDsNormal(string GUIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUIDs", GUIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitByGUIDsNormal] @GUIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<UnitMasterDTO> GetUnitByIDsNormal(string IDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitByIDsNormal] @IDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<UnitMasterDTO> GetUnitByGUIDsPlain(string GUIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUIDs", GUIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitByGUIDsPlain] @GUIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<UnitMasterDTO> GetUnitByIDsPlain(string IDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitByIDsPlain] @IDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<UnitMasterDTO> GetUnitByRoomPlain(long RoomID, long CompanyID, bool IsForBom)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBom", IsForBom) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitByRoomPlain] @RoomID,@CompanyID,@IsForBom", params1).ToList();
            }
        }

        public List<UnitMasterDTO> GetUnitByRoomNormal(long RoomID, long CompanyID, bool IsForBom)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBom", IsForBom) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitByRoomNormal] @RoomID,@CompanyID,@IsForBom", params1).ToList();
            }
        }

        public UnitMasterDTO GetUnitByNamePlain(long RoomID, long CompanyID, bool IsForBom, string Name)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), 
                                               new SqlParameter("@CompanyID", CompanyID), 
                                               new SqlParameter("@IsForBom", IsForBom), 
                                               new SqlParameter("@Name", Name ?? (object)DBNull.Value) 
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitByNamePlain] @RoomID,@CompanyID,@IsForBom,@Name", params1).FirstOrDefault();
            }
        }


        public Int64 Insert(UnitMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.GetValueOrDefault(false);
            objDTO.IsArchived = objDTO.IsArchived.GetValueOrDefault(false);
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UnitMaster obj = new UnitMaster();
                obj.ID = 0;
                obj.GUID = Guid.NewGuid();
                obj.Unit = objDTO.Unit;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.Description = objDTO.Description;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.isForBOM = objDTO.isForBOM;
                obj.RefBomId = objDTO.RefBomId;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                
                if (objDTO.isForBOM)
                {
                    objDTO.Room = 0;
                    objDTO.RoomName = string.Empty;
                    obj.Room = 0;
                }
                
                context.UnitMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;              


                return obj.ID;
            }
        }

        public bool Edit(UnitMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.GetValueOrDefault(false);
            objDTO.IsArchived = objDTO.IsArchived.GetValueOrDefault(false);
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UnitMaster obj = context.UnitMasters.FirstOrDefault(x => x.ID == objDTO.ID);

                obj.Unit = objDTO.Unit;
                obj.Description = objDTO.Description;

                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.GUID = objDTO.GUID;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                
                if (objDTO.isForBOM)
                {
                    objDTO.Room = 0;
                    objDTO.RoomName = string.Empty;
                    obj.Room = 0;
                }
                
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.isForBOM = objDTO.isForBOM;
                obj.RefBomId = objDTO.RefBomId;
                obj.AddedFrom = objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                
                if (objDTO.IsOnlyFromItemUI)
                {
                    obj.EditedFrom = objDTO.EditedFrom;
                    obj.ReceivedOn = objDTO.ReceivedOn;
                }
                context.SaveChanges();

                return true;
            }
        }

        public List<NarrowSearchDTO> GetUnitListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, bool IsForBOM)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value), new SqlParameter("@IsForBOM", IsForBOM) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetUnitListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@IsForBOM", params1).ToList();
            }
        }

        public void InsertDefaultUOMByName(string Unit, long? CreatedUserID, long RoomID, long? CompanyID)
        {
            UnitMasterDTO objDTO = new UnitMasterDTO();
            objDTO.ID = 1;
            objDTO.Unit = Unit;
            objDTO.IsDeleted = false;
            objDTO.IsArchived = false;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.LastUpdatedBy = CreatedUserID;
            objDTO.Room = RoomID;
            objDTO.CompanyID = CompanyID;
            objDTO.CreatedBy = CreatedUserID;
            objDTO.GUID = Guid.NewGuid();
            objDTO.AddedFrom = "Web";
            objDTO.EditedFrom = "Web";
            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            Insert(objDTO);
        }

        public List<UnitMasterDTO> GetUnitByNameSearch(string Name, long RoomID, long CompanyID, bool IsForBOM)
        {
            var params1 = new SqlParameter[] { 
                                               new SqlParameter("@Name", Name ?? (object)DBNull.Value), 
                                               new SqlParameter("@RoomID", RoomID), 
                                               new SqlParameter("@CompanyID", CompanyID), 
                                               new SqlParameter("@IsForBOM", IsForBOM) 
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitByNameSearch] @Name,@RoomID,@CompanyID,@IsForBOM", params1).ToList();
            }
        }

        public List<UnitMasterDTO> GetPagedUnitMaster(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, bool IsForBom,string RoomDateFormat,TimeZoneInfo CurrentTimeZone)
        {
            string CreatedByName = "";
            string UpdatedByName = "";
            string CreatedDateFrom = "";
            string CreatedDateTo = "";
            string UpdatedDateFrom = "";
            string UpdatedDateTo = "";
            string UDF1 = "";
            string UDF2 = "";
            string UDF3 = "";
            string UDF4 = "";
            string UDF5 = "";

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

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    //Created = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    //Updated = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));
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
            }
            else
            {
                SearchTerm = "";
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm),
                    new SqlParameter("@sortColumnName", sortColumnName),
                    new SqlParameter("@CreatedFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedTo", CreatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedByName),
                    new SqlParameter("@UpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedTo", UpdatedDateTo),
                    new SqlParameter("@LastUpdatedBy", UpdatedByName),
                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@UDF1", UDF1),
                    new SqlParameter("@UDF2", UDF2),
                    new SqlParameter("@UDF3", UDF3),
                    new SqlParameter("@UDF4", UDF4),
                    new SqlParameter("@UDF5", UDF5),
                    new SqlParameter("@IsForBom", IsForBom)
                };
                List<UnitMasterDTO> lstcats = context.Database.SqlQuery<UnitMasterDTO>("exec [GetPagedUnitMaster] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@CreatedBy,@UpdatedFrom,@UpdatedTo,@LastUpdatedBy,@Room,@IsArchived,@IsDeleted,@CompanyID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@IsForBom", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords;
                }
                return lstcats;
            }
        }

        public List<UnitMasterDTO> GetUnitMasterChangeLog(string IDs)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UnitMasterDTO>("exec [GetUnitMasterChangeLog] @ID", params1).ToList();
            }
        }
        #endregion


    }
}
