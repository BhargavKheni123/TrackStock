using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class ItemMasterBinDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ItemMasterBinDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ItemMasterBinDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public List<ItemMasterBinDTO> GetPagedItemMasterBin(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64? Room, Boolean? IsArchived, Boolean? IsDeleted, Int64? CompanyID, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string CreatedByName = "";
            string UpdatedByName = "";
            string CreatedDateFrom = "";
            string CreatedDateTo = "";
            string UpdatedDateFrom = "";
            string UpdatedDateTo = "";
            string ItemGuid = "";
            string BinGuid = "";

            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');

                if (Fields.Length > 2)
                {
                    if (Fields[2] != null)
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[99]))
                {
                    string[] arrReplenishTypes = FieldsPara[99].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        ItemGuid = ItemGuid + supitem + "','";
                    }
                    ItemGuid = ItemGuid.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[100]))
                {
                    string[] arrReplenishTypes = FieldsPara[100].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        BinGuid = BinGuid + supitem + "','";
                    }
                    BinGuid = BinGuid.TrimEnd('\'').TrimEnd(',').TrimEnd('\'').Replace("'","''");
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
                    new SqlParameter("@Room", Room),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@ItemGuid", ItemGuid),
                    new SqlParameter("@BinGuid", BinGuid),
                };

                List<ItemMasterBinDTO> lstcats = context.Database.SqlQuery<ItemMasterBinDTO>("exec [GetItemBinListRoomWise] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@CreatedBy,@UpdatedFrom,@UpdatedTo,@LastUpdatedBy,@Room,@IsArchived,@IsDeleted,@CompanyID,@ItemGuid,@BinGuid", params1).ToList();
                TotalCount = 0;

                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;
            }
        }

        public List<ItemMasterBinDTO> ItemBinListChangeImportExport(Int64 RoomID, Int64 CompanyID, string BinGuids)
        {
            string BinGuid = string.Empty;
            List<Guid> arrids = new List<Guid>();
            if (!string.IsNullOrWhiteSpace(BinGuids))
            {
                string[] arrReplenishTypes = BinGuids.Split(',');
                foreach (string item in arrReplenishTypes)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        BinGuid = BinGuid + item + "','";
                    }
                }
                BinGuid = BinGuid.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@BinGuids", BinGuid), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                List<ItemMasterBinDTO> lstcats = context.Database.SqlQuery<ItemMasterBinDTO>("exec ItemBinListChangeExport @BinGuids,@RoomID,@CompanyID", params1).ToList();
                return lstcats;
            }
        }

        public List<ItemMasterBinDTO> GetBinItemUsingGuid(Guid GUID, Int64 RoomId, Int64 CompanyId, Boolean IsDeleted, Boolean IsArchived)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@RoomId", RoomId), new SqlParameter("@CompanyId", CompanyId) };
                return context.Database.SqlQuery<ItemMasterBinDTO>("exec [GetItemBinByGUID] @GUID,@IsDeleted,@IsArchived,@RoomId,@CompanyId", params1).ToList();
            }
        }

        public bool GetBinItemIsDefault(Guid GUID, Int64 RoomId, Int64 CompanyId, Boolean IsDeleted, Boolean IsArchived)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    if ((from ILD in context.BinMasters
                         where ILD.GUID == GUID && ILD.Room == RoomId && ILD.CompanyID == CompanyId && (ILD.IsDeleted) == IsDeleted && (ILD.IsArchived) == IsArchived
                         select ILD).Any())
                    {
                        bool IsDefault = (from ILD in context.BinMasters
                                          where ILD.GUID == GUID && ILD.Room == RoomId && ILD.CompanyID == CompanyId && (ILD.IsDeleted) == IsDeleted && (ILD.IsArchived) == IsArchived
                                          select ILD).FirstOrDefault().IsDefault ?? false;
                        return IsDefault;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public string SaveItemMasterBin(Guid ItemGUID, Int64 BinID, string BinNumber, Int64 UserID, Int64 RoomID, Int64 CompanyID, bool IsDefault)
        {
            try
            {
                ItemMasterBinDTO objItemMasterBinDTO = new ItemMasterBinDTO();
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@BinID", BinID), new SqlParameter("@BinNumber", BinNumber), new SqlParameter("@UserID", UserID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDefault", IsDefault) };
                    objItemMasterBinDTO = context.Database.SqlQuery<ItemMasterBinDTO>("exec [UpdateInventoryLocation] @ItemGUID,@BinID,@BinNumber,@UserID,@RoomID,@CompanyID,@IsDefault", params1).FirstOrDefault();
                }
                if (objItemMasterBinDTO != null)
                {
                    return objItemMasterBinDTO.Message;
                }
                return "Item Bin not updated.";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public IEnumerable<RequisitionMasterNarrowSearchDTO> GetAllNarrowSearchRecords(string TextFieldName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TextFieldName", TextFieldName), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@Isdeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                return context.Database.SqlQuery<RequisitionMasterNarrowSearchDTO>("exec [GetItemBinNarrowSearch] @TextFieldName,@RoomId,@CompanyId,@Isdeleted,@IsArchived", params1).ToList();
            }
        }
    }
}
