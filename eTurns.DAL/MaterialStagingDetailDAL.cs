using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;


namespace eTurns.DAL
{
    public class MaterialStagingDetailDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public MaterialStagingDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public MaterialStagingDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class methods]

        public IEnumerable<MaterialStagingDetailDTO> GetAllRecordsRoomItemWise(Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived, Guid ItemGuid)
        {
            //Get Cached-Media


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //IEnumerable<MaterialStagingDetailDTO> obj = (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>(@"
                //        SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                //        A.Room as RoomId ,L.BinNumber as StagingBinName
                //        FROM MaterialStagingDetail A 
                //        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                //        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                //        left outer join BinMaster L on A.StagingBinID = L.ID
                //        LEFT OUTER JOIN Room D on A.Room = D.ID 
                //        WHERE A.CompanyID = " + CompanyID.ToString())
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@RoomId", RoomID), new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };

                IEnumerable<MaterialStagingDetailDTO> obj = (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("exec [GetItemStagingData] @CompanyId, @RoomId,@ItemGuid,@IsDeleted,@IsArchived", params1)
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
        
        public IEnumerable<MaterialStagingDetailDTO> GetAllRecordsRoomItemWisePositiveQTY(Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived, Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyId", CompanyID), 
                                                   new SqlParameter("@RoomId", RoomID), 
                                                   new SqlParameter("@ItemGuid", ItemGuid), 
                                                   new SqlParameter("@IsDeleted", IsDeleted), 
                                                   new SqlParameter("@IsArchived", IsArchived) };

                IEnumerable<MaterialStagingDetailDTO> obj = (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("EXEC [GetItemStagingDataPositiveQty] @CompanyId, @RoomId,@ItemGuid,@IsDeleted,@IsArchived", params1)
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
        public IEnumerable<MaterialStagingDetailDTO> GetMSDetailByRoomCompanyItemGUID(Int64 RoomID, Int64 CompanyID, string ItemGUID, bool IsArchived, bool IsDeleted, bool IsPositiveQTY)
        {
            List<MaterialStagingDetailDTO> lstMSDetail = new List<MaterialStagingDetailDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@ItemGUID", ItemGUID ?? string.Empty),
                                                   new SqlParameter("@IsDeleted", IsDeleted),
                                                   new SqlParameter("@IsArchived", IsArchived),
                                                   new SqlParameter("@IsPositiveQTY", IsPositiveQTY)
                                                };

                lstMSDetail = (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("EXEC dbo.GetMSDetailByRoomCompanyItemGUID @RoomID,@CompanyID,@ItemGUID,@IsDeleted,@IsArchived,@IsPositiveQTY", params1)
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
            }

            return lstMSDetail;
        }

        public IEnumerable<MaterialStagingDetailDTO> GetMaterialStagingDetailbyItemGUIDStagingBINID(string MaterialStagingGUID, string ItemGUID, Int64? StagingBinID, Int64 RoomID, Int64 CompanyID, bool? IsDeleted, bool? IsArchived)
        {
            List<MaterialStagingDetailDTO> lstMaterialStagingDetail = new List<MaterialStagingDetailDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@MaterialStagingGUID", MaterialStagingGUID ?? string.Empty),
                                                   new SqlParameter("@ItemGUID", ItemGUID ?? string.Empty),
                                                   new SqlParameter("@StagingBinID", StagingBinID ?? 0),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@IsDeleted", IsDeleted ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsArchived", IsArchived ?? (object)DBNull.Value)
                                                };

                lstMaterialStagingDetail = (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("EXEC dbo.GetMaterialStagingDetailbyItemGUIDStagingBINID @MaterialStagingGUID,@ItemGUID,@StagingBinID,@RoomID,@CompanyID,@IsDeleted,@IsArchived", params1)
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

            }

            return lstMaterialStagingDetail;

        }

        public MaterialStagingDetailDTO GetMaterialStagingDetailbyItemGUIDANDStagingBINID(Guid MSGUID, Int64 StagingBinID, Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@MaterialStagingGUID", Convert.ToString(MSGUID) ?? string.Empty),
                                                   new SqlParameter("@ItemGUID", Convert.ToString(ItemGUID)?? string.Empty),
                                                   new SqlParameter("@StagingBinID", StagingBinID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID)
                                                };

                return (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("EXEC dbo.GetMaterialStagingDetailbyItemGUIDANDStagingBINID @MaterialStagingGUID,@ItemGUID,@StagingBinID,@RoomID,@CompanyID", params1)
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
                var params1 = new SqlParameter[] { new SqlParameter("@MaterialStagingGUID", Convert.ToString(MSGUID) ?? string.Empty),
                                                   new SqlParameter("@ItemGUID", Convert.ToString(ItemGUID)?? string.Empty),
                                                   new SqlParameter("@StagingBinID", StagingBinID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID)
                                                };

                return (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("EXEC dbo.GetMaterialStagingDetailbyItemGUIDANDStagingBIN @MaterialStagingGUID,@ItemGUID,@StagingBinID,@RoomID,@CompanyID", params1)
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

        public IEnumerable<MaterialStagingDetailDTO> GetPagedRecordsByItem(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ItemGUId, Guid MSGGUID, Int64 MSLineItemStagingBinID, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string CreatedByName = "";
            string UpdatedByName = "";
            string CreatedDateFrom = "";
            string CreatedDateTo = "";
            string UpdatedDateFrom = "";
            string UpdatedDateTo = "";

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
                var sqlParams = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                    new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@CreatedFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedTo", CreatedDateTo),
                    new SqlParameter("@UpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedTo", UpdatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedByName),
                    new SqlParameter("@LastUpdatedBy", UpdatedByName),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@ItemGUID", ItemGUId),
                    new SqlParameter("@MaterialStagingGUID", MSGGUID),
                    new SqlParameter("@StagingBinID", MSLineItemStagingBinID)

                };

                List<MaterialStagingDetailDTO> lstcats = context.Database.SqlQuery<MaterialStagingDetailDTO>("exec [GetPagedRecordMaterialStagingDetailByItem] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@IsDeleted,@IsArchived,@CreatedFrom,@CreatedTo,@UpdatedFrom,@UpdatedTo,@CreatedBy,@LastUpdatedBy,@Room,@CompanyID,@ItemGUID,@MaterialStagingGUID,@StagingBinID", sqlParams).ToList();
                TotalCount = 0;
                lstcats.Where(w => w.StagingBinName == "[|EmptyStagingBin|]").ToList().ForEach(s => s.StagingBinName = string.Empty);
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;
            }

        }

        public MaterialStagingDetailDTO GetMaterialStagingDetailByGUID(Guid MSDetailGUID, Int64 RoomID, Int64 CompanyID)
        {
            MaterialStagingDetailDTO objMSDetailDTO = new MaterialStagingDetailDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@GUID", MSDetailGUID)
                                                };
                objMSDetailDTO = (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("EXEC dbo.GetMaterialStagingDetailByGUID @RoomID,@CompanyID,@GUID", params1)
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
            return objMSDetailDTO;
        }

        public MaterialStagingDetailDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", id) };

                return (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("EXEC dbo.GetMaterialStagingDetailHistoryByID @ID", params1)
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
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb
                        }).SingleOrDefault();
            }
        }

        public List<MaterialStagingDetailDTO> GetHistoryRecordbyMaterialStagingID(Guid MaterialStagingGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramsMSD1 = new SqlParameter[] { new SqlParameter("@MaterialStagingGUID", MaterialStagingGUID), new SqlParameter("@dbName", DataBaseName) };
                return context.Database.SqlQuery<MaterialStagingDetailDTO>("exec [GetMaterialStagingDetailChangeLog] @MaterialStagingGUID,@dbName", paramsMSD1).ToList();
            }
        }

        public IEnumerable<MaterialStagingDetailDTO> GetMaterialStagingDetailByStagingBinName(string StagingBinNumber, Int64 RoomID, Int64 CompanyId)
        {
            List<MaterialStagingDetailDTO> lstMSDtlDto = new List<MaterialStagingDetailDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@StagingBinName", StagingBinNumber)
                                                };

                lstMSDtlDto = (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("EXEC dbo.GetMaterialStagingDetailByStagingBinName @RoomID,@CompanyID,@StagingBinName", params1)
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
            }

            return lstMSDtlDto;
        }

        public IEnumerable<MaterialStagingDetailDTO> GetMaterialStagingDetailByRoomIDCompanyID(Int64 RoomID, Int64 CompanyId)
        {
            List<MaterialStagingDetailDTO> lstMSDetailDTO = new List<MaterialStagingDetailDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyId)
                                                 };

                lstMSDetailDTO = (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("EXEC dbo.GetMaterialStagingDetailByRoomIDCompanyID @RoomID,@CompanyID", params1)
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

            }

            return lstMSDetailDTO;
        }


        public Int64 Insert(MaterialStagingDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MaterialStagingDetail obj = new MaterialStagingDetail();
                obj.ID = 0;
                obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.StagingBinID = objDTO.StagingBinID;
                if (objDTO.BinID > 0)
                    obj.BinID = objDTO.BinID;

                obj.GUID = objDTO.GUID; //Guid.NewGuid();
                obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;
                obj.Quantity = objDTO.Quantity;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.RoomId;
                obj.CompanyID = objDTO.CompanyID;
                obj.AddedFrom = objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                context.MaterialStagingDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;


                //if (objDTO.ID > 0)
                //{
                //    //Get Cached-Media
                //    IEnumerable<MaterialStagingDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.GetCacheItem("Cached_MaterialStagingDetail_" + objDTO.CompanyID.ToString());
                //    if (ObjCache != null)
                //    {
                //        List<MaterialStagingDetailDTO> tempC = new List<MaterialStagingDetailDTO>();
                //        tempC.Add(objDTO);

                //        IEnumerable<MaterialStagingDetailDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //        CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.AppendToCacheItem("Cached_MaterialStagingDetail_" + objDTO.CompanyID.ToString(), NewCache);
                //    }
                //}

                return obj.ID;
            }

        }

        public bool Edit(MaterialStagingDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MaterialStagingDetail obj = new MaterialStagingDetail();
                obj.ID = objDTO.ID;
                obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;

                obj.ItemGUID = objDTO.ItemGUID;
                obj.StagingBinID = objDTO.StagingBinID;
                obj.BinID = objDTO.BinID;
                obj.GUID = objDTO.GUID;
                obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;
                obj.Quantity = objDTO.Quantity;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.RoomId;
                obj.CompanyID = objDTO.CompanyID;
                obj.AddedFrom = (objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom);
                obj.ReceivedOnWeb = (objDTO.ReceivedOnWeb);//objDTO.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow :

                obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                obj.ReceivedOn = (objDTO.ReceivedOn);

                if (objDTO.IsOnlyFromItemUI)
                {
                    obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                    obj.ReceivedOn = (objDTO.ReceivedOn);//objDTO.ReceivedOn == null ? DateTimeUtility.DateTimeNow :
                }
                context.MaterialStagingDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;

                context.SaveChanges();

                //Get Cached-Media
                //IEnumerable<MaterialStagingDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.GetCacheItem("Cached_MaterialStagingDetail_" + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<MaterialStagingDetailDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                //    ObjCache = objTemp.AsEnumerable();

                //    List<MaterialStagingDetailDTO> tempC = new List<MaterialStagingDetailDTO>();
                //    tempC.Add(objDTO);
                //    IEnumerable<MaterialStagingDetailDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //    CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.AppendToCacheItem("Cached_MaterialStagingDetail_" + objDTO.CompanyID.ToString(), NewCache);
                //}


                return true;
            }
        }

        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID)  
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs),
                                                   new SqlParameter("@UserID", userid),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@RoomID", RoomID) };

                context.Database.SqlQuery<int>("EXEC [DeleteMSDetail] @IDs,@UserID,@CompanyID,@RoomID", params1).FirstOrDefault();
                return true;
            }
        }


        public bool UnDeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs),
                                                   new SqlParameter("@UserID", userid),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@RoomID", RoomID) };

                context.Database.SqlQuery<int>("EXEC [UnDeleteMSDetail] @IDs,@UserID,@CompanyID,@RoomID", params1).FirstOrDefault();
                return true;
            }
        }

        public bool DeleteMSHeaderItems(string ids, Int64 UserID, Int64 RoomID, Int64 CompanyId, string RoomDateFormat, long SessionUserId,long EnterpriseId)
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

            } // 

            eTurns.DAL.CacheHelper<IEnumerable<MaterialStagingDTO>>.InvalidateCache();
            eTurns.DAL.CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.InvalidateCache();

            //IEnumerable<MaterialStagingDetailDTO> lstItemsToDelete = GetAllRecords(RoomID, CompanyId).Where(t => MSGUIDS.Contains(t.MaterialStagingGUID.GetValueOrDefault(Guid.Empty)) && t.IsArchived == false && t.IsDeleted == false);
            IEnumerable<MaterialStagingDetailDTO> lstItemsToDelete = GetMaterialStagingDetailbyItemGUIDStagingBINID(ids, string.Empty, null, RoomID, CompanyId, false, false);

            foreach (var item in lstItemsToDelete)
            {

                DeleteSingleMSDtlItem(item, UserID, RoomID, CompanyId, RoomDateFormat, SessionUserId,EnterpriseId);
            }
            return true;
        }

        public bool DeleteMSLineItems(List<MaterialStagingDetailDTO> lstToDelete, Int64 UserID, Int64 RoomID, Int64 CompanyId, string RoomDateFormat, long SessionUserId, long EnterpriseId)
        {
            foreach (var item in lstToDelete)
            {
                IEnumerable<MaterialStagingDetailDTO> lstItemsToDelete = GetMaterialStagingDetailbyItemGUIDANDStagingBIN(item.MaterialStagingGUID ?? Guid.Empty, item.StagingBinID, item.ItemGUID ?? Guid.Empty, RoomID, CompanyId); //GetAllRecords(RoomID, CompanyId).Where(t => t.MaterialStagingGUID == item.MaterialStagingGUID && t.ItemGUID == item.ItemGUID && t.StagingBinID == item.StagingBinID);
                foreach (var item1 in lstItemsToDelete)
                {
                    DeleteSingleMSDtlItem(item1, UserID, RoomID, CompanyId, RoomDateFormat, SessionUserId,EnterpriseId);
                }
            }
            return true;
        }

        public bool DeleteSingleMSDtlItem(MaterialStagingDetailDTO MaterialStagingDetailItem, long UserId, long RoomID, long CompanyID, string RoomDateFormat, long SessionUserId,long EnterpriseId)
        {
            if (DeleteRecords(MaterialStagingDetailItem.GUID.ToString(), UserId, CompanyID, RoomID))
            {
                string msg = "";
                //MSDeleteManageInventory(MaterialStagingDetailItem, RoomID, CompanyID, out msg);
                MSDeleteManageBackInventory(MaterialStagingDetailItem, RoomID, CompanyID, UserId, RoomDateFormat, out msg, SessionUserId,EnterpriseId);
            }
            return true;
        }

        public bool UnDeleteMSDetails(string ids, Int64 UserID, Int64 RoomID, Int64 CompanyId, long SessionUserId,long EnterpriseId)
        {
            MaterialStagingDAL objMaterialStagingDAL = new MaterialStagingDAL(base.DataBaseName);
            Guid[] MSGUIDS = new Guid[] { Guid.Empty };
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

            //IEnumerable<MaterialStagingDetailDTO> lstItemsToUnDelete = GetAllRecords(RoomID, CompanyId).Where(t => MSGUIDS.Contains(t.MaterialStagingGUID.GetValueOrDefault(Guid.Empty)) && t.IsArchived == false && t.IsDeleted == true);
            IEnumerable<MaterialStagingDetailDTO> lstItemsToUnDelete = GetMaterialStagingDetailbyItemGUIDStagingBINID(ids, string.Empty, null, RoomID, CompanyId, true, false);

            foreach (var item in lstItemsToUnDelete)
            {
                UnDeleteSingleMSDtlItem(item, UserID, RoomID, CompanyId, SessionUserId,EnterpriseId);
            }
            return true;
        }

        public bool UnDeleteSingleMSDtlItem(MaterialStagingDetailDTO MaterialStagingDetailItem, long UserId, long RoomID, long CompanyID, long SessionUserId,long EnterpriseId)
        {
            if (UnDeleteRecords(MaterialStagingDetailItem.ID.ToString(), UserId, CompanyID, RoomID))
            {
                string msg = "";
                MSUnDeleteManageInventory(MaterialStagingDetailItem, RoomID, CompanyID, out msg, SessionUserId,EnterpriseId);
            }
            return true;
        }

        public StagingActionResult AddQuantityToStagingBin(MaterialStagingDetailDTO objMaterialStagingDetailDTO, long SessionUserId, long EnterpriseId)
        {
            StagingActionResult objStagingActionResult = null;
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDTO();
            BinMasterDTO objBINDTO = new BinMasterDTO();
            ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
            BinMasterDAL objBINDAL = new BinMasterDAL(base.DataBaseName);
            PullMasterViewDTO obj = new PullMasterViewDTO();

            RoomDTO objRoomDTO = new RoomDTO();
            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            CommonDAL objDAL = new CommonDAL(base.DataBaseName);

            ItemMasterDTO objItemMaster = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, objMaterialStagingDetailDTO.ItemGUID);
            if (objItemMaster != null)
            {
                string columnList = "ID,RoomName,MethodOfValuingInventory";
                objRoomDTO = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + objMaterialStagingDetailDTO.RoomId.ToString() + "", "");

                //objRoomDTO = objRoomDAL.GetRoomByIDPlain(objMaterialStagingDetailDTO.RoomId);
                //lstLocDTO = objLocQTY.GetAllRecords(objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID).Where(x => x.BinID == objMaterialStagingDetailDTO.BinID && x.ItemGUID == objMaterialStagingDetailDTO.ItemGUID).SingleOrDefault();
                lstLocDTO = objLocQTY.GetItemLocationQTY(objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID, objMaterialStagingDetailDTO.BinID, Convert.ToString(objMaterialStagingDetailDTO.ItemGUID)).FirstOrDefault();
                objBINDTO = objBINDAL.GetBinByID(objMaterialStagingDetailDTO.BinID ?? 0, objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID);
                //objBINDTO = objBINDAL.GetRecord(objMaterialStagingDetailDTO.BinID.GetValueOrDefault(0), objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID, false, false);
                MaterialStagingPullDetailDTO objMaterialStagingPullDtl = GetAvailableQty(objMaterialStagingDetailDTO, objItemMaster);
                if (objMaterialStagingPullDtl.ActualAvailableQuantity >= objMaterialStagingDetailDTO.Quantity)
                {
                    MaterialStagingDetailDTO AddedSavedDTO = SaveMaterialStagingDetails(objMaterialStagingDetailDTO);
                    objMaterialStagingDetailDTO.ID = AddedSavedDTO.ID;
                    objMaterialStagingDetailDTO.StagingBinID = AddedSavedDTO.StagingBinID;

                    if (objItemMaster.SerialNumberTracking)
                    {
                        List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecordsAvailableAt((Int64)objMaterialStagingDetailDTO.BinID, objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID, objMaterialStagingDetailDTO.ItemGUID.Value, null, "CustomerOwnedQuantity DESC, ConsignedQuantity DESC, ID ASC").Take((int)objMaterialStagingDetailDTO.Quantity).ToList();
                        foreach (var itemoil in ObjItemLocation)
                        {
                            double loopCurrentTakenCustomer = 0;
                            double loopCurrentTakenConsignment = 0;

                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
                            if (objItemMaster.Consignment)
                            {
                                if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                                {
                                    loopCurrentTakenConsignment = 1;
                                    itemoil.ConsignedQuantity = 0;
                                }
                                else
                                {
                                    loopCurrentTakenCustomer = 1;
                                    itemoil.CustomerOwnedQuantity = 0;
                                }

                            }
                            else
                            {
                                loopCurrentTakenCustomer = 1;
                                itemoil.CustomerOwnedQuantity = 0;
                            }
                            itemoil.MoveQuantity = (loopCurrentTakenCustomer + loopCurrentTakenConsignment);
                            objLocationDAL.Edit(itemoil);
                            AddtoMSPullDetail(itemoil, objItemMaster.Cost, objMaterialStagingDetailDTO.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, objMaterialStagingDetailDTO, objRoomDTO.MethodOfValuingInventory, SessionUserId,EnterpriseId);

                        }
                    }
                    else
                    {
                        List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecordsAvailableAt((Int64)objMaterialStagingDetailDTO.BinID, objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID, objMaterialStagingDetailDTO.ItemGUID.Value, null, "CustomerOwnedQuantity DESC, ConsignedQuantity DESC, ID ASC").ToList();//.Take((int)objMaterialStagingDetailDTO.Quantity).ToList();
                        Double takenQunatity = 0;
                        foreach (var itemoil in ObjItemLocation)
                        {
                            Double loopCurrentTakenCustomer = 0;
                            Double loopCurrentTakenConsignment = 0;
                            /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
                            if (takenQunatity == objMaterialStagingDetailDTO.Quantity)
                            {
                                break;
                            }

                            if (objItemMaster.Consignment)
                            {
                                #region "Consignment Credit and Pull"


                                //Both's sum we have available.
                                if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objMaterialStagingDetailDTO.Quantity - takenQunatity))
                                {
                                    //loopCurrentTaken = ((Double)objDTO.TempPullQTY - takenQunatity);
                                    loopCurrentTakenConsignment = ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
                                    itemoil.ConsignedQuantity = (Double)itemoil.ConsignedQuantity - ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
                                    takenQunatity += ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
                                    goto Save;
                                }
                                else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objMaterialStagingDetailDTO.Quantity - takenQunatity))
                                {
                                    //loopCurrentTaken = ((Double)objDTO.TempPullQTY - takenQunatity);
                                    loopCurrentTakenCustomer = ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
                                    itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
                                    takenQunatity += (Double)objMaterialStagingDetailDTO.Quantity - takenQunatity;
                                    goto Save;
                                }
                                else
                                {
                                    takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    itemoil.CustomerOwnedQuantity = 0; // itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;
                                    // needs to write logic for break down deduction from consigned or customer quantity location wise ...
                                    if (itemoil.ConsignedQuantity >= ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity))
                                    {
                                        //loopCurrentTaken = ((Double)objDTO.TempPullQTY - takenQunatity);
                                        loopCurrentTakenConsignment = ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
                                        itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
                                        takenQunatity += (Double)objMaterialStagingDetailDTO.Quantity - takenQunatity;
                                        goto Save;
                                    }
                                    else
                                    {
                                        //loopCurrentTaken = itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                        loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                        takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                        itemoil.ConsignedQuantity = 0;
                                    }
                                    //PENDING -- loop by varialbe from mupliple locations...
                                }

                                #endregion
                            }
                            else
                            {

                                if (itemoil.CustomerOwnedQuantity >= (objMaterialStagingDetailDTO.Quantity - takenQunatity))
                                {
                                    //loopCurrentTaken = ((Double)objDTO.TempPullQTY - takenQunatity);
                                    loopCurrentTakenCustomer = ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
                                    itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity - ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
                                    takenQunatity += ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
                                    goto Save;
                                }
                                else
                                {
                                    //loopCurrentTaken = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - takenQunatity;
                                }


                            }
                        Save:
                            itemoil.MoveQuantity = (loopCurrentTakenCustomer + loopCurrentTakenConsignment);
                            objLocationDAL.Edit(itemoil);
                            AddtoMSPullDetail(itemoil, objItemMaster.Cost, objMaterialStagingDetailDTO.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, objMaterialStagingDetailDTO, objRoomDTO.MethodOfValuingInventory, SessionUserId,EnterpriseId);

                        }
                    }

                    #region "ItemLocation Quantity Deduction"
                    objItemMaster.OnHandQuantity = objItemMaster.OnHandQuantity - objMaterialStagingDetailDTO.Quantity;
                    if (objItemMaster.Consignment)
                    {
                        //Both's sum we have available.
                        if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                        {
                            obj.ConsignedQuantity = (Double)objMaterialStagingDetailDTO.Quantity;
                            lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - objMaterialStagingDetailDTO.Quantity;
                            lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)objMaterialStagingDetailDTO.Quantity;
                        }
                        else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) >= (Double)objMaterialStagingDetailDTO.Quantity)
                        {
                            obj.CustomerOwnedQuantity = (Double)objMaterialStagingDetailDTO.Quantity;
                            lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - (Double)objMaterialStagingDetailDTO.Quantity;
                            lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)objMaterialStagingDetailDTO.Quantity;
                        }
                        else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) < (Double)objMaterialStagingDetailDTO.Quantity)
                        {
                            Double cstqty = (Double)objMaterialStagingDetailDTO.Quantity - (Double)lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0); // -((Double)objDTO.TempPullQTY - cstqty);
                            Double consqty = cstqty;

                            lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - consqty;
                            obj.CustomerOwnedQuantity = (Double)lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                            obj.ConsignedQuantity = consqty;
                            lstLocDTO.CustomerOwnedQuantity = 0;
                            lstLocDTO.Quantity = lstLocDTO.Quantity - (obj.CustomerOwnedQuantity.GetValueOrDefault(0) + obj.ConsignedQuantity.GetValueOrDefault(0));
                        }

                    }
                    else
                    {
                        lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - objMaterialStagingDetailDTO.Quantity;
                        lstLocDTO.Quantity = lstLocDTO.Quantity - (Double)objMaterialStagingDetailDTO.Quantity;

                        obj.CustomerOwnedQuantity = (Double)objMaterialStagingDetailDTO.Quantity;
                    }


                    #endregion

                    #region "Saving Location and QTY data"
                    //new ItemMasterDAL(base.DataBaseName).Edit(objItemMaster);
                    List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
                    lstUpdate.Add(lstLocDTO);
                    objLocQTY.Save(lstUpdate, SessionUserId,EnterpriseId);
                    #endregion

                }
                else
                {
                    objStagingActionResult = new StagingActionResult();
                    objStagingActionResult.ReturnMessage = "Not Enough stock to stage";
                    return objStagingActionResult;
                }
            }
            else
            {
                objStagingActionResult = new StagingActionResult();
                objStagingActionResult.ReturnMessage = "Item not Exist";
                return objStagingActionResult;
            }
            objStagingActionResult = new StagingActionResult();
            objStagingActionResult.ReturnCode = 1;
            return objStagingActionResult;
        }

        //public StagingActionResult UpdateQuantityToStagingBin(MaterialStagingDetailDTO objMaterialStagingDetailDTO)
        //{
        //    StagingActionResult objStagingActionResult = null;
        //    ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
        //    ItemMasterDTO objItemMaster = new ItemMasterDAL(base.DataBaseName).GetItemWithoutJoins(null, objMaterialStagingDetailDTO.ItemGUID);

        //    RoomDTO objRoomDTO = new RoomDTO();
        //    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);

        //    if (objItemMaster != null)
        //    {
        //        objRoomDTO = objRoomDAL.GetRecord(objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID, false, false);
        //        MaterialStagingPullDetailDTO objMaterialStagingPullDtl = GetAvailableQty(objMaterialStagingDetailDTO, objItemMaster);
        //        if (objMaterialStagingPullDtl.ActualAvailableQuantity >= objMaterialStagingDetailDTO.Quantity)
        //        {
        //            objMaterialStagingDetailDTO.AddedFrom = "Web";
        //            objMaterialStagingDetailDTO.EditedFrom = "Web";
        //            objMaterialStagingDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
        //            objMaterialStagingDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
        //            Insert(objMaterialStagingDetailDTO);

        //            if (objItemMaster.SerialNumberTracking)
        //            {
        //                List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecordsAvailableAt((Int64)objMaterialStagingDetailDTO.BinID, objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID, objMaterialStagingDetailDTO.ItemGUID.Value, null, "CustomerOwnedQuantity DESC, ConsignedQuantity DESC, ID ASC").Take((int)objMaterialStagingDetailDTO.Quantity).ToList();
        //                foreach (var itemoil in ObjItemLocation)
        //                {
        //                    double loopCurrentTakenCustomer = 0;
        //                    double loopCurrentTakenConsignment = 0;

        //                    /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
        //                    if (objItemMaster.Consignment)
        //                    {
        //                        if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
        //                        {
        //                            loopCurrentTakenConsignment = 1;
        //                            itemoil.ConsignedQuantity = 0;
        //                        }
        //                        else
        //                        {
        //                            loopCurrentTakenCustomer = 1;
        //                            itemoil.CustomerOwnedQuantity = 0;
        //                        }

        //                    }
        //                    else
        //                    {
        //                        loopCurrentTakenCustomer = 1;
        //                        itemoil.CustomerOwnedQuantity = 0;
        //                    }
        //                    objLocationDAL.Edit(itemoil);
        //                    AddtoMSPullDetail(itemoil, objItemMaster.Cost, objMaterialStagingDetailDTO.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, objMaterialStagingDetailDTO, objRoomDTO.MethodOfValuingInventory);

        //                }
        //            }
        //            else
        //            {
        //                List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecordsAvailableAt((Int64)objMaterialStagingDetailDTO.BinID, objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID, objMaterialStagingDetailDTO.ItemGUID.Value, null, "CustomerOwnedQuantity DESC, ConsignedQuantity DESC, ID ASC").Take((int)objMaterialStagingDetailDTO.Quantity).ToList();
        //                Double takenQunatity = 0;
        //                foreach (var itemoil in ObjItemLocation)
        //                {
        //                    Double loopCurrentTakenCustomer = 0;
        //                    Double loopCurrentTakenConsignment = 0;
        //                    /*mail:- eTurns: Minutes of meeting as on 2/01/2013 5:00 PM (IST)*/
        //                    if (takenQunatity == objMaterialStagingDetailDTO.Quantity)
        //                    {
        //                        break;
        //                    }

        //                    if (objItemMaster.Consignment)
        //                    {
        //                        #region "Consignment Credit and Pull"


        //                        //Both's sum we have available.
        //                        if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objMaterialStagingDetailDTO.Quantity - takenQunatity))
        //                        {
        //                            //loopCurrentTaken = ((Double)objDTO.TempPullQTY - takenQunatity);
        //                            loopCurrentTakenConsignment = ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
        //                            itemoil.ConsignedQuantity = (Double)itemoil.ConsignedQuantity - ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
        //                            takenQunatity += ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
        //                            goto Save;
        //                        }
        //                        else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objMaterialStagingDetailDTO.Quantity - takenQunatity))
        //                        {
        //                            //loopCurrentTaken = ((Double)objDTO.TempPullQTY - takenQunatity);
        //                            loopCurrentTakenCustomer = ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
        //                            itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
        //                            takenQunatity += (Double)objMaterialStagingDetailDTO.Quantity - takenQunatity;
        //                            goto Save;
        //                        }
        //                        else
        //                        {
        //                            takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                            loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                            itemoil.CustomerOwnedQuantity = 0; // itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;
        //                            // needs to write logic for break down deduction from consigned or customer quantity location wise ...
        //                            if (itemoil.ConsignedQuantity >= ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity))
        //                            {
        //                                //loopCurrentTaken = ((Double)objDTO.TempPullQTY - takenQunatity);
        //                                loopCurrentTakenConsignment = ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
        //                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
        //                                takenQunatity += (Double)objMaterialStagingDetailDTO.Quantity - takenQunatity;
        //                                goto Save;
        //                            }
        //                            else
        //                            {
        //                                //loopCurrentTaken = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
        //                                itemoil.ConsignedQuantity = 0;
        //                            }
        //                            //PENDING -- loop by varialbe from mupliple locations...
        //                        }

        //                        #endregion
        //                    }
        //                    else
        //                    {

        //                        if (itemoil.CustomerOwnedQuantity >= (objMaterialStagingDetailDTO.Quantity - takenQunatity))
        //                        {
        //                            //loopCurrentTaken = ((Double)objDTO.TempPullQTY - takenQunatity);
        //                            loopCurrentTakenCustomer = ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
        //                            itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity - ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
        //                            takenQunatity += ((Double)objMaterialStagingDetailDTO.Quantity - takenQunatity);
        //                            goto Save;
        //                        }
        //                        else
        //                        {
        //                            //loopCurrentTaken = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                            loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                            takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
        //                            itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - takenQunatity;
        //                        }


        //                    }
        //                Save:
        //                    objLocationDAL.Edit(itemoil);
        //                    AddtoMSPullDetail(itemoil, objItemMaster.Cost, objMaterialStagingDetailDTO.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, objMaterialStagingDetailDTO, objRoomDTO.MethodOfValuingInventory);

        //                }
        //            }
        //        }
        //        else
        //        {
        //            objStagingActionResult = new StagingActionResult();
        //            objStagingActionResult.ReturnMessage = "Not Enough stock to stage";
        //            return objStagingActionResult;
        //        }
        //    }
        //    else
        //    {
        //        objStagingActionResult = new StagingActionResult();
        //        objStagingActionResult.ReturnMessage = "Item not Exist";
        //        return objStagingActionResult;
        //    }
        //    return objStagingActionResult;
        //}

        protected void AddtoMSPullDetail(ItemLocationDetailsDTO itemlocationdetail, Double? ItemCost, Int64? UserID, double loopCurrentTakenCustomer, double loopCurrentTakenConsinment,
            MaterialStagingDetailDTO objMaterialStagingDetailDTO, string MethodOfValuingInventory, long SessionUserId,long EnterpriseId)
        {
            if (MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                && loopCurrentTakenConsinment == 0)
            {
                ItemCost = itemlocationdetail.Cost;
            }
            MaterialStagingPullDetailDAL objDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
            MaterialStagingPullDetailDTO objDTO = new MaterialStagingPullDetailDTO();

            objDTO.MaterialStagingdtlGUID = objMaterialStagingDetailDTO.GUID;
            objDTO.ItemGUID = itemlocationdetail.ItemGUID ?? Guid.NewGuid();
            objDTO.GUID = Guid.NewGuid();
            objDTO.MaterialStagingGUID = objMaterialStagingDetailDTO.MaterialStagingGUID;
            objDTO.StagingBinId = objMaterialStagingDetailDTO.StagingBinID;
            if (itemlocationdetail.CustomerOwnedQuantity != null)
                objDTO.CustomerOwnedQuantity = loopCurrentTakenCustomer;

            if (itemlocationdetail.ConsignedQuantity != null)
                objDTO.ConsignedQuantity = loopCurrentTakenConsinment;

            objDTO.PoolQuantity = (loopCurrentTakenCustomer + loopCurrentTakenConsinment);

            if (itemlocationdetail.SerialNumber != null)
                objDTO.SerialNumber = itemlocationdetail.SerialNumber;

            if (itemlocationdetail.LotNumber != null)
                objDTO.LotNumber = itemlocationdetail.LotNumber;

            if (itemlocationdetail.Expiration != null)
                objDTO.Expiration = itemlocationdetail.Expiration;

            objDTO.Received = itemlocationdetail.Received;
            objDTO.BinID = itemlocationdetail.BinID;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = UserID;
            objDTO.LastUpdatedBy = UserID;
            objDTO.Room = itemlocationdetail.Room;
            objDTO.CompanyID = itemlocationdetail.CompanyID;
            objDTO.ItemLocationDetailGUID = itemlocationdetail.GUID;
            objDTO.ItemCost = ItemCost;
            objDTO.AddedFrom = "Web";
            objDTO.EditedFrom = "Web";
            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

            objDAL.Insert(objDTO);
            UpdateItemStageQuantity(objDTO.ItemGUID, objDTO.PoolQuantity.GetValueOrDefault(0), 1, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), SessionUserId,EnterpriseId);

        }

        public long DeleteStagingDetailItem()
        {
            return 0;
        }

        public MaterialStagingPullDetailDTO GetAvailableQty(MaterialStagingDetailDTO objMaterialStagingDetailDTO, ItemMasterDTO objItemMasterDTO)
        {
            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            MaterialStagingPullDetailDTO objMaterialStagingPullDtl = new MaterialStagingPullDetailDTO();
            List<ItemLocationDetailsDTO> lstitmqty = objItemLocationDetailsDAL.GetItemQuantityByLocation(objMaterialStagingDetailDTO.BinID.GetValueOrDefault(0), objMaterialStagingDetailDTO.ItemGUID.Value, objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID);
            if (lstitmqty != null && lstitmqty.Count > 0)
            {
                objMaterialStagingPullDtl.CustomerOwnedQuantity = lstitmqty.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                objMaterialStagingPullDtl.ConsignedQuantity = lstitmqty.Sum(t => (t.ConsignedQuantity ?? 0));

                if (objItemMasterDTO.Consignment)
                {
                    objMaterialStagingPullDtl.ActualAvailableQuantity = (objMaterialStagingPullDtl.CustomerOwnedQuantity ?? 0) + (objMaterialStagingPullDtl.ConsignedQuantity ?? 0);

                }
                else
                {
                    objMaterialStagingPullDtl.ActualAvailableQuantity = (objMaterialStagingPullDtl.CustomerOwnedQuantity ?? 0);
                }
            }

            return objMaterialStagingPullDtl;
        }

        public MaterialStagingDetailDTO SaveMaterialStagingDetails(MaterialStagingDetailDTO objMaterialStagingDetailDTO)
        {
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            MaterialStagingDAL objMaterialStagingDAL = new MaterialStagingDAL(base.DataBaseName);
            MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);

            MaterialStagingDTO objMaterialStagingDTO = objMaterialStagingDAL.GetRecord(objMaterialStagingDetailDTO.MaterialStagingGUID.Value, objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID);
            ItemMasterDTO objItem = objItemMasterDAL.GetItemWithoutJoins(null, objMaterialStagingDetailDTO.ItemGUID);
            BinMasterDTO objBin = objBinMasterDAL.GetBinByID(objMaterialStagingDetailDTO.BinID.GetValueOrDefault(0), objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID);
            //BinMasterDTO objBin = objBinMasterDAL.GetItemLocation( objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID, false, false,Guid.Empty, objMaterialStagingDetailDTO.BinID.GetValueOrDefault(0),null,null).FirstOrDefault();
            BinMasterDTO objStagingBin = new BinMasterDTO();
            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(base.DataBaseName);

            if (objMaterialStagingDTO != null && objBin != null && objItem != null)
            {
                if (!string.IsNullOrWhiteSpace(objMaterialStagingDetailDTO.StagingBinName))
                {
                    objStagingBin = objBinMasterDAL.GetItemBinPlain((objMaterialStagingDetailDTO.ItemGUID ?? Guid.Empty), objMaterialStagingDetailDTO.StagingBinName, objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID, objMaterialStagingDetailDTO.CreatedBy, true, materialStagingGUID: objMaterialStagingDetailDTO.MaterialStagingGUID);
                    //objStagingBin = objBinMasterDAL.GetAllRecords(objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID, false, false).Where(t => t.IsStagingLocation == true && t.BinNumber.ToLower() == objMaterialStagingDetailDTO.StagingBinName.Trim().ToLower() && t.ItemGUID == (objMaterialStagingDetailDTO.ItemGUID ?? Guid.Empty) && t.MaterialStagingGUID == (objMaterialStagingDetailDTO.MaterialStagingGUID ?? Guid.Empty)).FirstOrDefault();
                    //if (objStagingBin == null)
                    //{
                    //    objStagingBin = new BinMasterDTO();
                    //    objStagingBin.BinNumber = objMaterialStagingDetailDTO.StagingBinName.Trim();
                    //    objStagingBin.CompanyID = objMaterialStagingDetailDTO.CompanyID;
                    //    objStagingBin.Created = DateTimeUtility.DateTimeNow;
                    //    objStagingBin.CreatedBy = objMaterialStagingDetailDTO.CreatedBy;
                    //    objStagingBin.CreatedByName = objMaterialStagingDetailDTO.CreatedByName;
                    //    objStagingBin.GUID = Guid.NewGuid();
                    //    objStagingBin.IsArchived = false;
                    //    objStagingBin.IsDeleted = false;
                    //    objStagingBin.Room = objMaterialStagingDetailDTO.RoomId;
                    //    objStagingBin.RoomName = objMaterialStagingDetailDTO.RoomName;
                    //    objStagingBin.IsStagingLocation = true;
                    //    objStagingBin.MaterialStagingGUID = objMaterialStagingDetailDTO.MaterialStagingGUID;
                    //    objStagingBin.ItemGUID = objMaterialStagingDetailDTO.ItemGUID;
                    //    objStagingBin = objBinMasterDAL.InsertBin(objStagingBin);
                    //}

                }


                if (objStagingBin != null)
                {
                    if (objMaterialStagingDetailDTO.ID > 0)
                    {
                        //objMaterialStagingDetailDTO = objMaterialStagingDetailDAL.GetRecord(objMaterialStagingDetailDTO.GUID, objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID);
                        objMaterialStagingDetailDTO = objMaterialStagingDetailDAL.GetMaterialStagingDetailByGUID(objMaterialStagingDetailDTO.GUID, objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID);
                        if (objMaterialStagingDetailDTO != null)
                        {
                            objMaterialStagingDetailDTO.BinID = objBin.ID;
                            //objMaterialStagingDetailDTO.StagingBinID = objBin.ID;
                            objMaterialStagingDetailDTO.BinGUID = objBin.GUID;
                            objMaterialStagingDetailDTO.BinName = objBin.BinNumber;
                            objMaterialStagingDetailDTO.Created = DateTimeUtility.DateTimeNow;
                            objMaterialStagingDetailDTO.CreatedBy = objMaterialStagingDetailDTO.CreatedBy;
                            objMaterialStagingDetailDTO.CreatedByName = objMaterialStagingDetailDTO.CreatedByName;
                            objMaterialStagingDetailDTO.LastUpdatedBy = objMaterialStagingDetailDTO.LastUpdatedBy;
                            objMaterialStagingDetailDTO.Quantity = objMaterialStagingDetailDTO.Quantity;
                            objMaterialStagingDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                            objMaterialStagingDetailDTO.UpdatedByName = objMaterialStagingDetailDTO.UpdatedByName;
                            objMaterialStagingDetailDTO.StagingBinID = (objStagingBin.ID > 0 ? objStagingBin.ID : objMaterialStagingDetailDTO.StagingBinID);
                            objMaterialStagingDetailDTO.StagingBinGUID = (objStagingBin.GUID != Guid.Empty ? objStagingBin.GUID : objMaterialStagingDetailDTO.StagingBinGUID);
                            objMaterialStagingDetailDTO.EditedFrom = "Web";
                            objMaterialStagingDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objMaterialStagingDetailDTO.IsOnlyFromItemUI = true;

                            this.Edit(objMaterialStagingDetailDTO);
                            return objMaterialStagingDetailDTO;
                        }
                    }
                    else
                    {
                        objMaterialStagingDetailDTO.BinID = objBin.ID;
                        //objMaterialStagingDetailDTO.StagingBinID = objBin.ID;
                        objMaterialStagingDetailDTO.BinGUID = objBin.GUID;
                        objMaterialStagingDetailDTO.BinName = objBin.BinNumber;
                        objMaterialStagingDetailDTO.CompanyID = objMaterialStagingDetailDTO.CompanyID;
                        objMaterialStagingDetailDTO.Created = DateTimeUtility.DateTimeNow;
                        objMaterialStagingDetailDTO.CreatedBy = objMaterialStagingDetailDTO.CreatedBy;
                        objMaterialStagingDetailDTO.CreatedByName = objMaterialStagingDetailDTO.CreatedByName;
                        objMaterialStagingDetailDTO.GUID = Guid.NewGuid();
                        objMaterialStagingDetailDTO.ID = 0;
                        objMaterialStagingDetailDTO.IsArchived = false;
                        objMaterialStagingDetailDTO.IsDeleted = false;
                        objMaterialStagingDetailDTO.ItemGUID = objItem.GUID;
                        objMaterialStagingDetailDTO.ItemNumber = objItem.ItemNumber;
                        objMaterialStagingDetailDTO.LastUpdatedBy = objMaterialStagingDetailDTO.LastUpdatedBy;
                        objMaterialStagingDetailDTO.MaterialStagingGUID = objMaterialStagingDTO.GUID;
                        objMaterialStagingDetailDTO.MaterialStagingName = objMaterialStagingDTO.StagingName;
                        objMaterialStagingDetailDTO.Quantity = objMaterialStagingDetailDTO.Quantity;
                        objMaterialStagingDetailDTO.RoomId = objMaterialStagingDetailDTO.RoomId;
                        objMaterialStagingDetailDTO.RoomName = objMaterialStagingDetailDTO.RoomName;
                        objMaterialStagingDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                        objMaterialStagingDetailDTO.UpdatedByName = objMaterialStagingDetailDTO.UpdatedByName;
                        objMaterialStagingDetailDTO.StagingBinID = (objStagingBin.ID > 0 ? objStagingBin.ID : objMaterialStagingDetailDTO.StagingBinID);
                        objMaterialStagingDetailDTO.StagingBinGUID = (objStagingBin.GUID != Guid.Empty ? objStagingBin.GUID : objMaterialStagingDetailDTO.StagingBinGUID);
                        objMaterialStagingDetailDTO.EditedFrom = "Web";
                        objMaterialStagingDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objMaterialStagingDetailDTO.AddedFrom = "Web";
                        objMaterialStagingDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objMaterialStagingDetailDTO.ID = Insert(objMaterialStagingDetailDTO);
                        return objMaterialStagingDetailDTO;
                    }
                }
                else
                {

                }
            }
            else
            {

            }
            return null;
        }

        public bool MSDeleteManageInventory(MaterialStagingDetailDTO objDTO, Int64 RoomID, Int64 CompanyID, out string MSG, long SessionUserId,long EnterpriseId)
        {
            MSG = "";
            //get all pull details 
            MaterialStagingPullDetailDAL objMaterialStagingPullDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
            List<MaterialStagingPullDetailDTO> lstPullDetailsDTO = objMaterialStagingPullDetailDAL.GetMsPullDetailsByMsDetailsId(objDTO.GUID);

            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);

            ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);


            foreach (var item in lstPullDetailsDTO)
            {
                if (item.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                    continue;

                //item locations details
                if (ItemDTO.SerialNumberTracking)
                {
                    CommonDAL objCDal = new CommonDAL(base.DataBaseName);
                    string ExtraWhereCond = "";

                    ExtraWhereCond = " AND (ISNULL(CustomerOwnedQuantity,0) = 0 AND isnull(ConsignedQuantity,0) = 0)  AND IsDeleted = 0 AND IsArchived = 0 ";



                    if (objCDal.DuplicateCheckForCreditPull(item.SerialNumber, "add", 0, "ItemLocationDetails", "SerialNumber", RoomID, CompanyID, ExtraWhereCond) == "duplicate")
                    {
                        return false;
                    }
                    else
                    {
                        ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID, null, "ID ASC").Where(t => t.SerialNumber == item.SerialNumber && t.GUID == item.ItemLocationDetailGUID && t.IsDeleted == false).SingleOrDefault();
                        if (objItemDetail != null)
                        {

                            objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                            objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);

                            objItemDetail.Updated = System.DateTime.Now;
                            objLocationDAL.Edit(objItemDetail);
                            UpdateItemStageQuantity(ItemDTO.GUID, ((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0)), -1, RoomID, CompanyID, SessionUserId, EnterpriseId);
                        }
                    }
                }
                else if (ItemDTO.LotNumberTracking)
                {
                    ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID, null, "ID ASC").Where(t => t.LotNumber == item.LotNumber && t.BinID == item.BinID && t.GUID == item.ItemLocationDetailGUID && t.IsDeleted == false).SingleOrDefault();
                    if (objItemDetail != null)
                    {

                        objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                        objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity + item.ConsignedQuantity;

                        objItemDetail.Updated = System.DateTime.Now;
                        objLocationDAL.Edit(objItemDetail);
                        UpdateItemStageQuantity(ItemDTO.GUID, ((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0)), -1, RoomID, CompanyID, SessionUserId,EnterpriseId);
                    }
                }
                else if (ItemDTO.DateCodeTracking)
                {
                    ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID, null, "ID ASC").Where(t => t.BinID == item.BinID && t.Expiration == item.Expiration && t.GUID == item.ItemLocationDetailGUID && t.IsDeleted == false).Take(1).SingleOrDefault();
                    if (objItemDetail != null)
                    {

                        objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                        objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);

                        objItemDetail.Updated = System.DateTime.Now;
                        objLocationDAL.Edit(objItemDetail);
                        UpdateItemStageQuantity(ItemDTO.GUID, ((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0)), -1, RoomID, CompanyID, SessionUserId, EnterpriseId);
                    }
                }
                else
                {
                    ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID, null, "ID ASC").Where(t => t.BinID == item.BinID && t.Received == item.Received && t.IsDeleted == false && t.GUID == item.ItemLocationDetailGUID).Take(1).SingleOrDefault();
                    if (objItemDetail != null)
                    {

                        objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                        objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);
                        objItemDetail.Updated = System.DateTime.Now;
                        objLocationDAL.Edit(objItemDetail);
                        UpdateItemStageQuantity(ItemDTO.GUID, ((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0)), -1, RoomID, CompanyID, SessionUserId, EnterpriseId);
                    }
                }

                //item locations qty update
                //ItemLocationQTYDTO lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == item.BinID && x.ItemGUID == item.ItemGUID).SingleOrDefault();
                ItemLocationQTYDTO lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, item.BinID, Convert.ToString(item.ItemGUID)).FirstOrDefault();
                if (lstLocDTO != null)
                {

                    lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                    lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);

                    lstLocDTO.Quantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + lstLocDTO.ConsignedQuantity.GetValueOrDefault(0);
                    List<ItemLocationQTYDTO> lst = new List<ItemLocationQTYDTO>();
                    lst.Add(lstLocDTO);
                    objLocQTY.Save(lst, SessionUserId,EnterpriseId);
                }
                deleteMsPullDtlRec(item);

            }


            return true;
        }

        private void deleteMsPullDtlRec(MaterialStagingPullDetailDTO item)
        {
            MaterialStagingPullDetailDAL objMaterialStagingPullDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
            objMaterialStagingPullDetailDAL.DeleteRecords(item.ID.ToString(), item.CreatedBy ?? 0, item.CompanyID ?? 0);
        }

        private void UndeleteMsPullDtlRec(MaterialStagingPullDetailDTO item)
        {
            MaterialStagingPullDetailDAL objMaterialStagingPullDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
            objMaterialStagingPullDetailDAL.UnDeleteRecords(item.ID.ToString(), item.CreatedBy ?? 0, item.CompanyID ?? 0);
        }

        public bool MSUnDeleteManageInventory(MaterialStagingDetailDTO objDTO, Int64 RoomID, Int64 CompanyID, out string MSG, long SessionUserId,long EnterpriseId)
        {
            MSG = "";
            //get all pull details 
            MaterialStagingPullDetailDAL objMaterialStagingPullDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
            List<MaterialStagingPullDetailDTO> lstPullDetailsDTO = objMaterialStagingPullDetailDAL.GetMsPullDetailsByMsDetailsIdForUndelete(objDTO.GUID);

            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);

            ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);


            foreach (var item in lstPullDetailsDTO)
            {
                //item locations details
                if (ItemDTO.SerialNumberTracking)
                {
                    ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID, null, "ID ASC").Where(t => t.SerialNumber == item.SerialNumber && t.GUID == item.ItemLocationDetailGUID && t.IsDeleted == false).SingleOrDefault();
                    if (objItemDetail != null)
                    {

                        objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                        objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);

                        objItemDetail.Updated = System.DateTime.Now;
                        objLocationDAL.Edit(objItemDetail);
                        UpdateItemStageQuantity(ItemDTO.GUID, ((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0)), 1, RoomID, CompanyID, SessionUserId,EnterpriseId);
                    }
                }
                else if (ItemDTO.LotNumberTracking)
                {
                    ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID, null, "ID ASC").Where(t => t.LotNumber == item.LotNumber && t.BinID == item.BinID && t.GUID == item.ItemLocationDetailGUID && t.IsDeleted == false).SingleOrDefault();
                    if (objItemDetail != null)
                    {

                        objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                        objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity - item.ConsignedQuantity;

                        objItemDetail.Updated = System.DateTime.Now;
                        objLocationDAL.Edit(objItemDetail);
                        UpdateItemStageQuantity(ItemDTO.GUID, ((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0)), 1, RoomID, CompanyID, SessionUserId,EnterpriseId);
                    }
                }
                else if (ItemDTO.DateCodeTracking)
                {
                    ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID, null, "ID ASC").Where(t => t.BinID == item.BinID && t.Expiration == item.Expiration && t.GUID == item.ItemLocationDetailGUID && t.IsDeleted == false).Take(1).SingleOrDefault();
                    if (objItemDetail != null)
                    {

                        objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                        objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);

                        objItemDetail.Updated = System.DateTime.Now;
                        objLocationDAL.Edit(objItemDetail);
                        UpdateItemStageQuantity(ItemDTO.GUID, ((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0)), 1, RoomID, CompanyID, SessionUserId,EnterpriseId);
                    }
                }
                else
                {
                    ItemLocationDetailsDTO objItemDetail = objLocationDAL.GetAllRecords(RoomID, CompanyID, item.ItemGUID, null, "ID ASC").Where(t => t.BinID == item.BinID && t.Received == item.Received && t.IsDeleted == false && t.GUID == item.ItemLocationDetailGUID).Take(1).SingleOrDefault();
                    if (objItemDetail != null)
                    {

                        objItemDetail.CustomerOwnedQuantity = objItemDetail.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                        objItemDetail.ConsignedQuantity = objItemDetail.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);
                        objItemDetail.Updated = System.DateTime.Now;
                        objLocationDAL.Edit(objItemDetail);
                        UpdateItemStageQuantity(ItemDTO.GUID, ((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0)), 1, RoomID, CompanyID, SessionUserId,EnterpriseId);
                    }
                }

                //item locations qty update
                //ItemLocationQTYDTO lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == item.BinID && x.ItemGUID == item.ItemGUID).SingleOrDefault();
                ItemLocationQTYDTO lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, item.BinID,Convert.ToString(item.ItemGUID)).FirstOrDefault();
                if (lstLocDTO != null)
                {

                    lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - item.CustomerOwnedQuantity.GetValueOrDefault(0);
                    lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) - item.ConsignedQuantity.GetValueOrDefault(0);

                    lstLocDTO.Quantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + lstLocDTO.ConsignedQuantity.GetValueOrDefault(0);
                    List<ItemLocationQTYDTO> lst = new List<ItemLocationQTYDTO>();
                    lst.Add(lstLocDTO);
                    objLocQTY.Save(lst, SessionUserId,EnterpriseId);
                }
                UndeleteMsPullDtlRec(item);
            }
            return true;
        }

        public void UpdateItemStageQuantity(Guid ItemGUID, double Quantuty, int PlusMinus, long RoomId, long CompanyID, long SessionUserId,long EnterpriseId)
        {
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO StagedUPdateItem = objItemMasterDAL.GetItemWithoutJoins(null, ItemGUID);
            if (StagedUPdateItem != null)
            {
                StagedUPdateItem.StagedQuantity = StagedUPdateItem.StagedQuantity.GetValueOrDefault(0) + (Quantuty * PlusMinus);
                //StagedUPdateItem.OnHandQuantity = StagedUPdateItem.OnHandQuantity.GetValueOrDefault(0) + (Quantuty * (-1) * PlusMinus);
                StagedUPdateItem.WhatWhereAction = "Staging";
                objItemMasterDAL.Edit(StagedUPdateItem, SessionUserId,EnterpriseId);
            }
        }

        public double GetTotalQtyonStagingLineItem(Guid MSGUID, Guid ItemGUID, string StagingLocationName)
        {
            double retval = 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from msd in context.MaterialStagingDetails
                           join bm in context.BinMasters on msd.StagingBinID equals bm.ID
                           where msd.MaterialStagingGUID == MSGUID && msd.ItemGUID == ItemGUID && msd.IsArchived == false && msd.IsDeleted == false && bm.BinNumber == StagingLocationName
                           select msd);
                if (qry.Any())
                {
                    retval = qry.Sum(t => t.Quantity) ?? 0;
                }
            }
            return retval;
        }

        public void UpdateTotalQtyonStagingOfItem(Guid ItemGUID, long SessionUserId,long EnterpriseId)
        {
            double retval = 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from msd in context.MaterialStagingDetails
                           join bm in context.BinMasters on msd.StagingBinID equals bm.ID
                           where msd.ItemGUID == ItemGUID && msd.IsArchived == false && msd.IsDeleted == false
                           select msd);
                if (qry.Any())
                {
                    retval = qry.Sum(t => t.Quantity) ?? 0;
                }
            }
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO StagedUPdateItem = objItemMasterDAL.GetItemWithoutJoins(null, ItemGUID);
            if (StagedUPdateItem != null)
            {
                StagedUPdateItem.StagedQuantity = retval;
                StagedUPdateItem.WhatWhereAction = "Staging";
                objItemMasterDAL.Edit(StagedUPdateItem, SessionUserId,EnterpriseId);
            }

        }

        public IEnumerable<MaterialStagingDetailDTO> GetPagedStagingLineItems(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, Guid MSGUID, List<long> SupplierIds)
        {
            List<MaterialStagingDetailDTO> lstStagingLineItems = new List<MaterialStagingDetailDTO>();
            TotalCount = 0;
            MaterialStagingDetailDTO objMaterialStagingDetailDTO = new MaterialStagingDetailDTO();
            DataSet dsStagingLineItems = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (Connectionstring == "")
            {
                return lstStagingLineItems;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            dsStagingLineItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedStagingLineItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, IsDeleted, IsArchived, RoomID, CompanyID, MSGUID, strSupplierIds);

            if (dsStagingLineItems != null && dsStagingLineItems.Tables.Count > 0)
            {
                DataTable dtStaging = dsStagingLineItems.Tables[0];
                if (dtStaging.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtStaging.Rows[0]["TotalRecords"]);
                    foreach (DataRow dr in dtStaging.Rows)
                    {
                        Guid tempguid = Guid.Empty;
                        objMaterialStagingDetailDTO = new MaterialStagingDetailDTO();
                        objMaterialStagingDetailDTO.CompanyID = CompanyID;
                        objMaterialStagingDetailDTO.RoomId = RoomID;
                        objMaterialStagingDetailDTO.MaterialStagingGUID = MSGUID;
                        if (dtStaging.Columns.Contains("StagingName"))
                        {
                            objMaterialStagingDetailDTO.MaterialStagingName = Convert.ToString(dr["StagingName"]);
                        }
                        if (dtStaging.Columns.Contains("ItemNumber"))
                        {
                            objMaterialStagingDetailDTO.ItemNumber = Convert.ToString(dr["ItemNumber"]);
                        }
                        if (dtStaging.Columns.Contains("Description"))
                        {
                            objMaterialStagingDetailDTO.Description = Convert.ToString(dr["Description"]);
                        }
                        if (dtStaging.Columns.Contains("SerialNumberTracking"))
                        {
                            objMaterialStagingDetailDTO.SerialNumberTracking = Convert.ToBoolean(dr["SerialNumberTracking"]);
                        }

                        if (dtStaging.Columns.Contains("LotNumberTracking"))
                        {
                            objMaterialStagingDetailDTO.LotNumberTracking = Convert.ToBoolean(dr["LotNumberTracking"]);
                        }

                        if (dtStaging.Columns.Contains("DateCodeTracking"))
                        {
                            objMaterialStagingDetailDTO.DateCodeTracking = Convert.ToBoolean(dr["DateCodeTracking"]);
                        }

                        if (dtStaging.Columns.Contains("StagingBinName"))
                        {
                            objMaterialStagingDetailDTO.StagingBinName = Convert.ToString(dr["StagingBinName"]);
                        }
                        if (dtStaging.Columns.Contains("Quantity"))
                        {
                            double tmpdoble = 0;
                            double.TryParse(Convert.ToString(dr["Quantity"]), out tmpdoble);
                            objMaterialStagingDetailDTO.Quantity = tmpdoble;
                        }
                        if (dtStaging.Columns.Contains("ItemGUID"))
                        {
                            Guid tmpguid = Guid.Empty; ;
                            Guid.TryParse(Convert.ToString(dr["ItemGUID"]), out tmpguid);
                            objMaterialStagingDetailDTO.ItemGUID = tmpguid;
                        }

                        if (dtStaging.Columns.Contains("StagingDtlGUID"))
                        {
                            Guid tmpguid = Guid.Empty; ;
                            Guid.TryParse(Convert.ToString(dr["StagingDtlGUID"]), out tmpguid);
                            objMaterialStagingDetailDTO.GUID = tmpguid;
                        }

                        if (dtStaging.Columns.Contains("StagingBinID"))
                        {
                            long tmplng = 0;
                            long.TryParse(Convert.ToString(dr["StagingBinID"]), out tmplng);
                            objMaterialStagingDetailDTO.StagingBinID = tmplng;
                        }
                        if (dtStaging.Columns.Contains("BinID"))
                        {
                            long tmplng = 0;
                            long.TryParse(Convert.ToString(dr["BinID"]), out tmplng);
                            objMaterialStagingDetailDTO.BinID = tmplng;
                            objMaterialStagingDetailDTO.BinName = Convert.ToString(dr["BinName"]);
                        }
                        lstStagingLineItems.Add(objMaterialStagingDetailDTO);
                    }
                }
            }
            return lstStagingLineItems;
        }

        public Dictionary<string, int> GetPullStagingNarrowSerach(long RoomID, long CompanyId, List<long> SupplierIds, bool? Session_ConsignedAllowed)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (Session_ConsignedAllowed != null)
                {
                    bool ConsignedAllowed = Convert.ToBoolean(Session_ConsignedAllowed);
                    if (ConsignedAllowed == false)
                    {
                        if (SupplierIds != null && SupplierIds.Any())
                        {
                            var Q = (from msd in context.MaterialStagingDetails
                                     join msm in context.MaterialStagings on msd.MaterialStagingGUID equals msm.GUID
                                     join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                                     where (msd.IsDeleted ?? false) == false && (msd.IsArchived ?? false) == false && (msd.Quantity ?? 0) > 0 && msd.Room == RoomID && msd.CompanyID == CompanyId && (msm.IsDeleted ?? false) == false
                                     && im.Consignment == ConsignedAllowed && (SupplierIds.Contains((im.SupplierID ?? 0)))
                                     group msm by new { msm.StagingName, msguid = msm.GUID, itmguid = im.GUID } into groupedMSD
                                     select new
                                     {
                                         StagingName = groupedMSD.Key.StagingName,
                                         StagingGUID = groupedMSD.Key.msguid,
                                         ItemGUID = groupedMSD.Key.itmguid,
                                         count = groupedMSD.Count()
                                     });

                            return (from qq in Q
                                    group qq by new { qq.StagingName, qq.StagingGUID } into groupedqq
                                    select new
                                    {
                                        StagingName = groupedqq.Key.StagingName,
                                        StagingGUID = groupedqq.Key.StagingGUID,
                                        count = groupedqq.Count()
                                    }).OrderBy(t => t.StagingName).Select(t => t).ToDictionary(b => b.StagingName + "[###]" + b.StagingGUID, b => b.count); ;
                        }
                        else
                        {
                            var Q = (from msd in context.MaterialStagingDetails
                                     join msm in context.MaterialStagings on msd.MaterialStagingGUID equals msm.GUID
                                     join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                                     where (msd.IsDeleted ?? false) == false && (msd.IsArchived ?? false) == false && (msd.Quantity ?? 0) > 0 && msd.Room == RoomID && msd.CompanyID == CompanyId && (msm.IsDeleted ?? false) == false
                                     && im.Consignment == ConsignedAllowed && ((im.SupplierID ?? 0) > 0)
                                     group msm by new { msm.StagingName, msguid = msm.GUID, itmguid = im.GUID } into groupedMSD
                                     select new
                                     {
                                         StagingName = groupedMSD.Key.StagingName,
                                         StagingGUID = groupedMSD.Key.msguid,
                                         ItemGUID = groupedMSD.Key.itmguid,
                                         count = groupedMSD.Count()
                                     });

                            return (from qq in Q
                                    group qq by new { qq.StagingName, qq.StagingGUID } into groupedqq
                                    select new
                                    {
                                        StagingName = groupedqq.Key.StagingName,
                                        StagingGUID = groupedqq.Key.StagingGUID,
                                        count = groupedqq.Count()
                                    }).OrderBy(t => t.StagingName).Select(t => t).ToDictionary(b => b.StagingName + "[###]" + b.StagingGUID, b => b.count); ;
                        }
                    }
                    else
                    {
                        if (SupplierIds != null && SupplierIds.Any())
                        {
                            var Q = (from msd in context.MaterialStagingDetails
                                     join msm in context.MaterialStagings on msd.MaterialStagingGUID equals msm.GUID
                                     join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                                     where (msd.IsDeleted ?? false) == false && (msd.IsArchived ?? false) == false && (msd.Quantity ?? 0) > 0 && msd.Room == RoomID && msd.CompanyID == CompanyId && (msm.IsDeleted ?? false) == false
                                     && (SupplierIds.Contains((im.SupplierID ?? 0)))
                                     group msm by new { msm.StagingName, msguid = msm.GUID, itmguid = im.GUID } into groupedMSD
                                     select new
                                     {
                                         StagingName = groupedMSD.Key.StagingName,
                                         StagingGUID = groupedMSD.Key.msguid,
                                         ItemGUID = groupedMSD.Key.itmguid,
                                         count = groupedMSD.Count()
                                     });

                            return (from qq in Q
                                    group qq by new { qq.StagingName, qq.StagingGUID } into groupedqq
                                    select new
                                    {
                                        StagingName = groupedqq.Key.StagingName,
                                        StagingGUID = groupedqq.Key.StagingGUID,
                                        count = groupedqq.Count()
                                    }).OrderBy(t => t.StagingName).Select(t => t).ToDictionary(b => b.StagingName + "[###]" + b.StagingGUID, b => b.count); ;
                        }
                        else
                        {
                            var Q = (from msd in context.MaterialStagingDetails
                                     join msm in context.MaterialStagings on msd.MaterialStagingGUID equals msm.GUID
                                     join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                                     where (msd.IsDeleted ?? false) == false && (msd.IsArchived ?? false) == false && (msd.Quantity ?? 0) > 0 && msd.Room == RoomID && msd.CompanyID == CompanyId && (msm.IsDeleted ?? false) == false
                                     && ((im.SupplierID ?? 0) > 0)
                                     group msm by new { msm.StagingName, msguid = msm.GUID, itmguid = im.GUID } into groupedMSD
                                     select new
                                     {
                                         StagingName = groupedMSD.Key.StagingName,
                                         StagingGUID = groupedMSD.Key.msguid,
                                         ItemGUID = groupedMSD.Key.itmguid,
                                         count = groupedMSD.Count()
                                     });

                            return (from qq in Q
                                    group qq by new { qq.StagingName, qq.StagingGUID } into groupedqq
                                    select new
                                    {
                                        StagingName = groupedqq.Key.StagingName,
                                        StagingGUID = groupedqq.Key.StagingGUID,
                                        count = groupedqq.Count()
                                    }).OrderBy(t => t.StagingName).Select(t => t).ToDictionary(b => b.StagingName + "[###]" + b.StagingGUID, b => b.count); ;
                        }

                    }
                }
                else
                {
                    if (SupplierIds != null && SupplierIds.Any())
                    {
                        var Q = (from msd in context.MaterialStagingDetails
                                 join msm in context.MaterialStagings on msd.MaterialStagingGUID equals msm.GUID
                                 join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                                 where (msd.IsDeleted ?? false) == false && (msd.IsArchived ?? false) == false && (msd.Quantity ?? 0) > 0 && msd.Room == RoomID && msd.CompanyID == CompanyId && (msm.IsDeleted ?? false) == false
                                 && (SupplierIds.Contains((im.SupplierID ?? 0)))
                                 group msm by new { msm.StagingName, msguid = msm.GUID, itmguid = im.GUID } into groupedMSD
                                 select new
                                 {
                                     StagingName = groupedMSD.Key.StagingName,
                                     StagingGUID = groupedMSD.Key.msguid,
                                     ItemGUID = groupedMSD.Key.itmguid,
                                     count = groupedMSD.Count()
                                 });

                        return (from qq in Q
                                group qq by new { qq.StagingName, qq.StagingGUID } into groupedqq
                                select new
                                {
                                    StagingName = groupedqq.Key.StagingName,
                                    StagingGUID = groupedqq.Key.StagingGUID,
                                    count = groupedqq.Count()
                                }).OrderBy(t => t.StagingName).Select(t => t).ToDictionary(b => b.StagingName + "[###]" + b.StagingGUID, b => b.count);
                    }
                    else
                    {
                        var Q = (from msd in context.MaterialStagingDetails
                                 join msm in context.MaterialStagings on msd.MaterialStagingGUID equals msm.GUID
                                 join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                                 where (msd.IsDeleted ?? false) == false && (msd.IsArchived ?? false) == false && (msd.Quantity ?? 0) > 0 && msd.Room == RoomID && msd.CompanyID == CompanyId && (msm.IsDeleted ?? false) == false
                                 && ((im.SupplierID ?? 0) > 0)
                                 group msm by new { msm.StagingName, msguid = msm.GUID, itmguid = im.GUID } into groupedMSD
                                 select new
                                 {
                                     StagingName = groupedMSD.Key.StagingName,
                                     StagingGUID = groupedMSD.Key.msguid,
                                     ItemGUID = groupedMSD.Key.itmguid,
                                     count = groupedMSD.Count()
                                 });

                        return (from qq in Q
                                group qq by new { qq.StagingName, qq.StagingGUID } into groupedqq
                                select new
                                {
                                    StagingName = groupedqq.Key.StagingName,
                                    StagingGUID = groupedqq.Key.StagingGUID,
                                    count = groupedqq.Count()
                                }).OrderBy(t => t.StagingName).Select(t => t).ToDictionary(b => b.StagingName + "[###]" + b.StagingGUID, b => b.count);
                    }
                }
            }
        }

        public Dictionary<string, int> GetPullStagingLocationsNarrowSerach(long RoomID, long CompanyId, List<long> SupplierIds, bool? Session_ConsignedAllowed)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (Session_ConsignedAllowed != null)
                {
                    bool ConsignedAllowed = Convert.ToBoolean(Session_ConsignedAllowed);

                    if (ConsignedAllowed == false)
                    {
                        if (SupplierIds != null && SupplierIds.Any())
                        {
                            var Q = (from msd in context.MaterialStagingDetails
                                     join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                                     join bm in context.BinMasters on msd.StagingBinID equals bm.ID
                                     where (msd.IsDeleted ?? false) == false && (msd.IsArchived ?? false) == false && (msd.Quantity ?? 0) > 0 && msd.Room == RoomID && msd.CompanyID == CompanyId
                                     && im.Consignment == ConsignedAllowed && (SupplierIds.Contains((im.SupplierID ?? 0)))
                                     group msd by new { bm.BinNumber, itmguid = im.GUID } into groupedMSD
                                     select new
                                     {
                                         StagingBinNumber = groupedMSD.Key.BinNumber,
                                         ItemGUID = groupedMSD.Key.itmguid,
                                         count = groupedMSD.Count()
                                     });

                            return (from qq in Q
                                    group qq by new { qq.StagingBinNumber } into groupedqq
                                    select new
                                    {
                                        StagingBinNumber = groupedqq.Key.StagingBinNumber,
                                        count = groupedqq.Count()
                                    }).OrderBy(t => t.StagingBinNumber).Select(t => t).ToDictionary(b => b.StagingBinNumber.Replace("[|EmptyStagingBin|]", "") + "[###]" + b.StagingBinNumber, b => b.count);
                        }
                        else
                        {
                            var Q = (from msd in context.MaterialStagingDetails
                                     join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                                     join bm in context.BinMasters on msd.StagingBinID equals bm.ID
                                     where (msd.IsDeleted ?? false) == false && (msd.IsArchived ?? false) == false && (msd.Quantity ?? 0) > 0 && msd.Room == RoomID && msd.CompanyID == CompanyId
                                     && im.Consignment == ConsignedAllowed && ((im.SupplierID ?? 0) > 0)
                                     group msd by new { bm.BinNumber, itmguid = im.GUID } into groupedMSD
                                     select new
                                     {
                                         StagingBinNumber = groupedMSD.Key.BinNumber,
                                         ItemGUID = groupedMSD.Key.itmguid,
                                         count = groupedMSD.Count()
                                     });

                            return (from qq in Q
                                    group qq by new { qq.StagingBinNumber } into groupedqq
                                    select new
                                    {
                                        StagingBinNumber = groupedqq.Key.StagingBinNumber,
                                        count = groupedqq.Count()
                                    }).OrderBy(t => t.StagingBinNumber).Select(t => t).ToDictionary(b => b.StagingBinNumber.Replace("[|EmptyStagingBin|]", "") + "[###]" + b.StagingBinNumber, b => b.count);
                        }

                    }
                    else
                    {

                        if (SupplierIds != null && SupplierIds.Any())
                        {
                            var Q = (from msd in context.MaterialStagingDetails
                                     join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                                     join bm in context.BinMasters on msd.StagingBinID equals bm.ID
                                     where (msd.IsDeleted ?? false) == false && (msd.IsArchived ?? false) == false && (msd.Quantity ?? 0) > 0 && msd.Room == RoomID && msd.CompanyID == CompanyId
                                     && (SupplierIds.Contains((im.SupplierID ?? 0)))
                                     group msd by new { bm.BinNumber, itmguid = im.GUID } into groupedMSD
                                     select new
                                     {
                                         StagingBinNumber = groupedMSD.Key.BinNumber,
                                         ItemGUID = groupedMSD.Key.itmguid,
                                         count = groupedMSD.Count()
                                     });

                            return (from qq in Q
                                    group qq by new { qq.StagingBinNumber } into groupedqq
                                    select new
                                    {
                                        StagingBinNumber = groupedqq.Key.StagingBinNumber,
                                        count = groupedqq.Count()
                                    }).OrderBy(t => t.StagingBinNumber).Select(t => t).ToDictionary(b => b.StagingBinNumber.Replace("[|EmptyStagingBin|]", "") + "[###]" + b.StagingBinNumber, b => b.count);
                        }
                        else
                        {
                            var Q = (from msd in context.MaterialStagingDetails
                                     join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                                     join bm in context.BinMasters on msd.StagingBinID equals bm.ID
                                     where (msd.IsDeleted ?? false) == false && (msd.IsArchived ?? false) == false && (msd.Quantity ?? 0) > 0 && msd.Room == RoomID && msd.CompanyID == CompanyId
                                     && ((im.SupplierID ?? 0) > 0)
                                     group msd by new { bm.BinNumber, itmguid = im.GUID } into groupedMSD
                                     select new
                                     {
                                         StagingBinNumber = groupedMSD.Key.BinNumber,
                                         ItemGUID = groupedMSD.Key.itmguid,
                                         count = groupedMSD.Count()
                                     });

                            return (from qq in Q
                                    group qq by new { qq.StagingBinNumber } into groupedqq
                                    select new
                                    {
                                        StagingBinNumber = groupedqq.Key.StagingBinNumber,
                                        count = groupedqq.Count()
                                    }).OrderBy(t => t.StagingBinNumber).Select(t => t).ToDictionary(b => b.StagingBinNumber.Replace("[|EmptyStagingBin|]", "") + "[###]" + b.StagingBinNumber, b => b.count);
                        }


                    }
                }
                else
                {
                    if (SupplierIds != null && SupplierIds.Any())
                    {
                        var Q = (from msd in context.MaterialStagingDetails
                                 join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                                 join bm in context.BinMasters on msd.StagingBinID equals bm.ID
                                 where (msd.IsDeleted ?? false) == false && (msd.IsArchived ?? false) == false && (msd.Quantity ?? 0) > 0 && msd.Room == RoomID && msd.CompanyID == CompanyId
                                 && (SupplierIds.Contains((im.SupplierID ?? 0)))
                                 group msd by new { bm.BinNumber, itmguid = im.GUID } into groupedMSD
                                 select new
                                 {
                                     StagingBinNumber = groupedMSD.Key.BinNumber,
                                     ItemGUID = groupedMSD.Key.itmguid,
                                     count = groupedMSD.Count()
                                 });

                        return (from qq in Q
                                group qq by new { qq.StagingBinNumber } into groupedqq
                                select new
                                {
                                    StagingBinNumber = groupedqq.Key.StagingBinNumber,
                                    count = groupedqq.Count()
                                }).OrderBy(t => t.StagingBinNumber).Select(t => t).ToDictionary(b => b.StagingBinNumber.Replace("[|EmptyStagingBin|]", "") + "[###]" + b.StagingBinNumber, b => b.count);
                    }
                    else
                    {
                        var Q = (from msd in context.MaterialStagingDetails
                                 join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                                 join bm in context.BinMasters on msd.StagingBinID equals bm.ID
                                 where (msd.IsDeleted ?? false) == false && (msd.IsArchived ?? false) == false && (msd.Quantity ?? 0) > 0 && msd.Room == RoomID && msd.CompanyID == CompanyId
                                 && ((im.SupplierID ?? 0) > 0)
                                 group msd by new { bm.BinNumber, itmguid = im.GUID } into groupedMSD
                                 select new
                                 {
                                     StagingBinNumber = groupedMSD.Key.BinNumber,
                                     ItemGUID = groupedMSD.Key.itmguid,
                                     count = groupedMSD.Count()
                                 });

                        return (from qq in Q
                                group qq by new { qq.StagingBinNumber } into groupedqq
                                select new
                                {
                                    StagingBinNumber = groupedqq.Key.StagingBinNumber,
                                    count = groupedqq.Count()
                                }).OrderBy(t => t.StagingBinNumber).Select(t => t).ToDictionary(b => b.StagingBinNumber.Replace("[|EmptyStagingBin|]", "") + "[###]" + b.StagingBinNumber, b => b.count);
                    }
                }
            }
        }

        public ItemStagePullInfo AddItemQuantityToStage(ItemStagePullInfo objItemPullInfo, long SessionUserId,long EnterpriseId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                PullTransactionDAL pullTransDAL = new PullTransactionDAL(base.DataBaseName);
                BinMasterDAL objBINDal = new DAL.BinMasterDAL(base.DataBaseName);

                if (objItemPullInfo == null || objItemPullInfo.lstItemPullDetails == null)
                    return objItemPullInfo;

                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objItemPullInfo.ItemGUID);
                var ItemPullCost = objItem.Cost;

                //BinMasterDTO binDTO = null;
                //BinMasterDTO StagebinDTO = null;
                if (objItemPullInfo.BinID <= 0)
                    objItemPullInfo.BinID = objBINDal.GetOrInsertBinIDByName(objItemPullInfo.ItemGUID, objItemPullInfo.BinNumber, objItemPullInfo.LastUpdatedBy, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, false).GetValueOrDefault(0);

                //binDTO = objBINDal.GetRecord(objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, false, false);
                if (string.IsNullOrEmpty(objItemPullInfo.StageBinNumber))
                    objItemPullInfo.StageBinNumber = "[|EmptyStagingBin|]";
                if (objItemPullInfo.StageBinID <= 0)
                    objItemPullInfo.StageBinID = objBINDal.GetOrInsertBinIDByName(objItemPullInfo.ItemGUID, objItemPullInfo.StageBinNumber, objItemPullInfo.LastUpdatedBy, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, true).GetValueOrDefault(0);

                //StagebinDTO = objBINDal.GetRecord(objItemPullInfo.StageBinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, false, false);


                MaterialStagingDetail objMSDetail = new MaterialStagingDetail()
                {
                    BinID = objItemPullInfo.BinID,
                    CompanyID = objItemPullInfo.CompanyId,
                    GUID = Guid.NewGuid(),
                    IsArchived = false,
                    IsDeleted = false,
                    ItemGUID = objItemPullInfo.ItemGUID,
                    LastUpdatedBy = objItemPullInfo.LastUpdatedBy,
                    Quantity = objItemPullInfo.PullQuantity,
                    Room = objItemPullInfo.RoomId,
                    Updated = DateTimeUtility.DateTimeNow,
                    Created = DateTimeUtility.DateTimeNow,
                    CreatedBy = objItemPullInfo.CreatedBy,
                    ReceivedOn = DateTimeUtility.DateTimeNow,
                    ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                    AddedFrom = "Web",
                    EditedFrom = "Web",
                    // BinGUID = binDTO.GUID,
                    CountLineItemGuid = null,
                    MaterialStagingGUID = objItemPullInfo.MSGUID,
                    ID = 0,
                    //StagingBinGUID = StagebinDTO.GUID,
                    StagingBinID = objItemPullInfo.StageBinID,

                };

                context.MaterialStagingDetails.Add(objMSDetail);
                context.SaveChanges();
                objItemPullInfo.MSDetailGUID = objMSDetail.GUID;

                objItemPullInfo.lstItemPullDetails.ForEach(t =>
                {
                    string InventoryConsuptionMethod = string.Empty;
                    Room objRoomDTO = context.Rooms.FirstOrDefault(x => x.ID == objItemPullInfo.RoomId);
                    if (objRoomDTO != null && !string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod))
                        InventoryConsuptionMethod = objRoomDTO.InventoryConsuptionMethod;

                    if (string.IsNullOrEmpty(InventoryConsuptionMethod))
                        InventoryConsuptionMethod = "";

                    List<ItemLocationDetail> lstItemLocations = null;
                    switch (InventoryConsuptionMethod.ToLower())
                    {
                        case "lifo":
                        case "lifooverride":
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                          (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                              || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                              || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                          || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                          && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderByDescending(x => x.ReceivedDate).ToList();
                            break;
                        case "fifo":
                        case "fifooverride":
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                            (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                                || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                                || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                            || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                            && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderBy(x => x.ReceivedDate).ToList();
                            break;
                        default:
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                           (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                               || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                               || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                           || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                           && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderBy(x => x.ReceivedDate).ToList();
                            break;
                    }


                    if (lstItemLocations != null)
                    {
                        foreach (var objItemLocationDetail in lstItemLocations)
                        {
                            if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) + (objItemLocationDetail.ConsignedQuantity ?? 0) >= (t.ConsignedTobePulled + t.CustomerOwnedTobePulled))
                            {
                                MaterialStagingPullDetail objPullDetail = new MaterialStagingPullDetail()
                                {
                                    BinID = t.BinID,
                                    CompanyID = objItemPullInfo.CompanyId,
                                    ConsignedQuantity = t.ConsignedTobePulled,
                                    CustomerOwnedQuantity = t.CustomerOwnedTobePulled,
                                    GUID = Guid.NewGuid(),
                                    IsArchived = false,
                                    IsDeleted = false,
                                    ItemCost = objItemLocationDetail.Cost,
                                    ItemGUID = objItemPullInfo.ItemGUID,
                                    LastUpdatedBy = objItemPullInfo.LastUpdatedBy,
                                    LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty,
                                    MaterialStagingdtlGUID = objMSDetail.GUID,
                                    PoolQuantity = t.CustomerOwnedTobePulled + t.ConsignedTobePulled,
                                    Room = objItemPullInfo.RoomId,
                                    SerialNumber = t.SerialNumberTracking ? t.LotOrSerailNumber : string.Empty,

                                    Updated = DateTimeUtility.DateTimeNow,
                                    Created = DateTimeUtility.DateTimeNow,
                                    CreatedBy = objItemPullInfo.CreatedBy,
                                    ReceivedOn = DateTimeUtility.DateTimeNow,
                                    ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                                    AddedFrom = "Web",
                                    EditedFrom = "Web",
                                    ItemLocationDetailGUID = objItemLocationDetail.GUID,
                                    MaterialStagingGUID = objItemPullInfo.MSGUID,
                                    IsConsignedSerialLot = t.IsConsignedLotSerial,
                                    ExpirationDate = objItemLocationDetail.ExpirationDate,
                                    ReceivedDate = objItemLocationDetail.ReceivedDate,

                                    Expiration = objItem.DateCodeTracking && objItemLocationDetail.ExpirationDate.HasValue ? objItemLocationDetail.Expiration : null,
                                    Received = objItemLocationDetail.ReceivedDate.HasValue ? objItemLocationDetail.ReceivedDate.Value.ToString("MM/dd/yyyy") : null,

                                    StagingBinId = objItemPullInfo.StageBinID,
                                    InitialQuantityWeb = (t.ConsignedTobePulled + t.CustomerOwnedTobePulled),
                                    InitialQuantityPDA = 0
                                };

                                context.MaterialStagingPullDetails.Add(objPullDetail);

                                objItemLocationDetail.CustomerOwnedQuantity = (objItemLocationDetail.CustomerOwnedQuantity ?? 0) - t.CustomerOwnedTobePulled;
                                objItemLocationDetail.ConsignedQuantity = (objItemLocationDetail.ConsignedQuantity ?? 0) - t.ConsignedTobePulled;
                                break;
                            }
                            else if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) + (objItemLocationDetail.ConsignedQuantity ?? 0) > 0)
                            {

                                MaterialStagingPullDetail objMSPullDetail = new MaterialStagingPullDetail()
                                {
                                    BinID = t.BinID,
                                    CompanyID = objItemPullInfo.CompanyId,
                                    CustomerOwnedQuantity = 0,
                                    ConsignedQuantity = 0,
                                    GUID = Guid.NewGuid(),
                                    IsArchived = false,
                                    IsDeleted = false,
                                    ItemCost = objItemLocationDetail.Cost,
                                    ItemGUID = objItemPullInfo.ItemGUID,
                                    LastUpdatedBy = objItemPullInfo.LastUpdatedBy,
                                    LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty,
                                    Expiration = objItem.DateCodeTracking && objItemLocationDetail.ExpirationDate.HasValue ? objItemLocationDetail.Expiration : null,
                                    Received = objItemLocationDetail.ReceivedDate.HasValue ? objItemLocationDetail.ReceivedDate.Value.ToString("MM/dd/yyyy") : null,
                                    Room = objItemPullInfo.RoomId,
                                    SerialNumber = t.SerialNumberTracking ? t.LotOrSerailNumber : string.Empty,
                                    Updated = DateTimeUtility.DateTimeNow,
                                    Created = DateTimeUtility.DateTimeNow,
                                    CreatedBy = objItemPullInfo.CreatedBy,
                                    ReceivedOn = DateTimeUtility.DateTimeNow,
                                    ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                                    AddedFrom = "Web",
                                    EditedFrom = "Web",
                                    ItemLocationDetailGUID = objItemLocationDetail.GUID,
                                    StagingBinId = objItemPullInfo.StageBinID,
                                    MaterialStagingdtlGUID = objMSDetail.GUID,
                                    MaterialStagingGUID = objItemPullInfo.MSGUID,
                                    IsConsignedSerialLot = objItemLocationDetail.IsConsignedSerialLot,
                                    InitialQuantityWeb = objItemLocationDetail.InitialQuantityWeb,
                                    InitialQuantityPDA = 0,
                                    ExpirationDate = objItemLocationDetail.ExpirationDate,
                                    ReceivedDate = objItemLocationDetail.ReceivedDate,

                                };
                                if (objItemLocationDetail.CustomerOwnedQuantity != null && objItemLocationDetail.CustomerOwnedQuantity > 0)
                                {
                                    if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) < t.CustomerOwnedTobePulled)
                                    {
                                        objMSPullDetail.CustomerOwnedQuantity = objItemLocationDetail.CustomerOwnedQuantity;
                                        objItemLocationDetail.CustomerOwnedQuantity = 0;
                                        t.CustomerOwnedTobePulled = (t.CustomerOwnedTobePulled) - (objMSPullDetail.CustomerOwnedQuantity ?? 0);
                                    }
                                    else
                                    {
                                        objMSPullDetail.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                                        objItemLocationDetail.CustomerOwnedQuantity = objItemLocationDetail.CustomerOwnedQuantity - t.CustomerOwnedTobePulled;
                                        t.CustomerOwnedTobePulled = 0;
                                    }
                                }

                                if (objItemLocationDetail.ConsignedQuantity != null && objItemLocationDetail.ConsignedQuantity > 0)
                                {
                                    if ((objItemLocationDetail.ConsignedQuantity ?? 0) < t.ConsignedTobePulled)
                                    {
                                        objMSPullDetail.ConsignedQuantity = objItemLocationDetail.ConsignedQuantity;
                                        objItemLocationDetail.ConsignedQuantity = 0;
                                        t.ConsignedTobePulled = (t.ConsignedTobePulled) - (objMSPullDetail.ConsignedQuantity ?? 0);
                                    }
                                    else
                                    {
                                        objMSPullDetail.ConsignedQuantity = t.ConsignedTobePulled;
                                        objItemLocationDetail.ConsignedQuantity = objItemLocationDetail.ConsignedQuantity - t.ConsignedTobePulled;
                                        t.ConsignedTobePulled = 0;
                                    }
                                }

                                objMSPullDetail.PoolQuantity = t.ConsignedQuantity.GetValueOrDefault(0) + t.CustomerOwnedQuantity.GetValueOrDefault(0);





                                context.MaterialStagingPullDetails.Add(objMSPullDetail);

                                if (objItemLocationDetail.CustomerOwnedQuantity < 0)
                                    objItemLocationDetail.CustomerOwnedQuantity = 0;

                                if (objItemLocationDetail.ConsignedQuantity < 0)
                                    objItemLocationDetail.ConsignedQuantity = 0;

                            }
                        }
                    }

                });

                //objMSDetail.CustomerOwnedQuantity = objItemPullInfo.TotalCustomerOwnedTobePulled;
                //objMSDetail.ConsignedQuantity = objItemPullInfo.TotalConsignedTobePulled;


                context.SaveChanges();

                ItemLocationQTY objItemLocationQTY = context.ItemLocationQTies.FirstOrDefault(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID);
                if (objItemLocationQTY == null)
                {
                    objItemLocationQTY = new ItemLocationQTY();
                    objItemLocationQTY.BinID = objItemPullInfo.BinID;
                    objItemLocationQTY.CompanyID = objItemPullInfo.CompanyId;
                    objItemLocationQTY.ConsignedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.ConsignedQuantity ?? 0));
                    objItemLocationQTY.Created = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.CreatedBy = objItemPullInfo.CreatedBy;
                    objItemLocationQTY.CustomerOwnedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    objItemLocationQTY.GUID = Guid.NewGuid();
                    objItemLocationQTY.ItemGUID = objItemPullInfo.ItemGUID;
                    objItemLocationQTY.LastUpdated = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                    objItemLocationQTY.Quantity = (objItemLocationQTY.ConsignedQuantity ?? 0) + (objItemLocationQTY.CustomerOwnedQuantity ?? 0);
                    objItemLocationQTY.Room = objItemPullInfo.RoomId;

                    objItemLocationQTY.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.AddedFrom = "Web";
                    objItemLocationQTY.EditedFrom = "Web";

                    context.ItemLocationQTies.Add(objItemLocationQTY);
                }
                else
                {
                    objItemLocationQTY.CustomerOwnedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    objItemLocationQTY.ConsignedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.ConsignedQuantity ?? 0));
                    objItemLocationQTY.Quantity = (objItemLocationQTY.ConsignedQuantity ?? 0) + (objItemLocationQTY.CustomerOwnedQuantity ?? 0);
                }
                objItem.OnHandQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
                context.SaveChanges();
                //objCartItemDAL.AutoCartUpdateByCode(objItem.GUID, objItemPullInfo.CreatedBy, "web", "PullTransationDAL >> PullitemQuantity_OLD");
                objCartItemDAL.AutoCartUpdateByCode(objItem.GUID, objItemPullInfo.CreatedBy, "web", "Consume >> Add Quantity to Stage", SessionUserId);

                context.SaveChanges();

                //if (objItem.OnHandQuantity.GetValueOrDefault(0) <= 0)
                //    objItemMasterDAL.SendMailWhenItemStockOut(objItem.ID, objItem.CompanyID.GetValueOrDefault(0), objItem.Room.GetValueOrDefault(0), objItem.LastUpdatedBy.GetValueOrDefault(0), objItem.OnHandQuantity.GetValueOrDefault(0), objItem.ItemNumber, objItem.GUID);
                //else
                //    objItemMasterDAL.RemoveItemStockOutMailLog(objItem.ID, objItem.CompanyID.GetValueOrDefault(0), objItem.Room.GetValueOrDefault(0));

                UpdateTotalQtyonStagingOfItem(objItemPullInfo.ItemGUID, SessionUserId,EnterpriseId);

                return objItemPullInfo;
            }
        }

        #endregion

        #region [for service]

        public IEnumerable<MaterialStagingDetailDTO> GetStagingLocationByItem(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, Int64? StagingBinID, string MaterialStagingGUID, bool? IsPositiveQTY)
        {
            List<MaterialStagingDetailDTO> lstMSDetail = new List<MaterialStagingDetailDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", Convert.ToString(ItemGuid)),
                                                   new SqlParameter("@RoomID", RoomID), 
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@StagingBinID", StagingBinID ?? (object)DBNull.Value),
                                                   new SqlParameter("@MaterialStagingGUID", MaterialStagingGUID ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsPositiveQTY", IsPositiveQTY ?? (object) DBNull.Value),
                };

                lstMSDetail = (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("exec [GetMaterialStagingLocationByItemGUID] @ItemGUID,@RoomID,@CompanyID,@StagingBinID,@MaterialStagingGUID,@IsPositiveQTY", params1)
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
            }

            return lstMSDetail;
        }

        public IEnumerable<MaterialStagingDetailDTO> GetStagingLocationByItemWithQty(Guid ItemGuid, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGuid", ItemGuid) };
                IEnumerable<MaterialStagingDetailDTO> obj = (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("exec [GetStagLocItemQty] @RoomID,@CompanyID,@ItemGuid", params1)
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

        public IEnumerable<MaterialStagingDetailDTO> GetStagingLocationByItemAndStagingHeader(Guid MaterialStagingGuid, Guid ItemGuid, long BinId, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var materialStaging = (from ms in context.MaterialStagings
                                       join msd in context.MaterialStagingDetails on ms.GUID equals msd.MaterialStagingGUID
                                       join bm in context.BinMasters on msd.StagingBinID equals bm.ID
                                       join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                                       where ms.GUID == MaterialStagingGuid && msd.ItemGUID == ItemGuid
                                       && msd.Room == RoomID && msd.CompanyID == CompanyID
                                       && msd.IsArchived == false && msd.IsDeleted == false
                                       && ms.IsArchived == false && ms.IsDeleted == false
                                       && ms.Room == RoomID && ms.CompanyID == CompanyID
                                       && bm.ID == BinId

                                       select new MaterialStagingDetailDTO
                                       {
                                           ItemGUID = msd.ItemGUID,
                                           MaterialStagingGUID = ms.GUID,
                                           StagingBinID = msd.StagingBinID,
                                           ItemNumber = im.ItemNumber,
                                           MaterialStagingName = ms.StagingName,
                                           StagingBinName = bm.BinNumber,
                                           Quantity = msd.Quantity.HasValue ? msd.Quantity.Value : 0,
                                       }).ToList();

                return materialStaging;
            }
        }

        public IEnumerable<MaterialStagingDetailDTO> GetStagingLocationByItemOnlyOpen(Guid ItemGuid, Int64 RoomID, Int64 CompanyID, string MaterialStagingGUID)
        {
            List<MaterialStagingDetailDTO> lstMSDetailDTO = new List<MaterialStagingDetailDTO>();
            
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", Convert.ToString(ItemGuid)),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@MaterialStagingGUID", MaterialStagingGUID ?? (object)DBNull.Value) 
                };

                lstMSDetailDTO = (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("exec [GetStagingLocationByItemOnlyOpen] @ItemGUID,@RoomID,@CompanyID,@MaterialStagingGUID", params1)
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
            }

            return lstMSDetailDTO;
        }

        #endregion

        public MaterialStagingDetailDTO SaveMaterialStagingDetailsForMSCredit(MaterialStagingDetailDTO objMaterialStagingDetailDTO)
        {
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(base.DataBaseName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            MaterialStagingDAL objMaterialStagingDAL = new MaterialStagingDAL(base.DataBaseName);
            MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);

            MaterialStagingDTO objMaterialStagingDTO = objMaterialStagingDAL.GetRecord(objMaterialStagingDetailDTO.MaterialStagingGUID.GetValueOrDefault(Guid.Empty), objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID);
            ItemMasterDTO objItem = objItemMasterDAL.GetItemWithoutJoins(null, objMaterialStagingDetailDTO.ItemGUID);
            //BinMasterDTO objBin = objBinMasterDAL.GetRecord(objMaterialStagingDetailDTO.BinID.GetValueOrDefault(0), objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID, false, false);
            //BinMasterDTO objBin = objBinMasterDAL.GetItemLocation( objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID, false, false,Guid.Empty, objMaterialStagingDetailDTO.BinID.GetValueOrDefault(0),null,null).FirstOrDefault();
            BinMasterDTO objStagingBin = new BinMasterDTO();
            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(base.DataBaseName);

            if (objMaterialStagingDTO != null && objItem != null)
            {
                if (!string.IsNullOrWhiteSpace(objMaterialStagingDetailDTO.StagingBinName))
                {
                    objStagingBin = objBinMasterDAL.GetItemBinPlain((objMaterialStagingDetailDTO.ItemGUID ?? Guid.Empty), objMaterialStagingDetailDTO.StagingBinName, objMaterialStagingDetailDTO.RoomId, objMaterialStagingDetailDTO.CompanyID, objMaterialStagingDetailDTO.CreatedBy, true);
                }

                if (objStagingBin != null)
                {
                    objMaterialStagingDetailDTO.BinID = null;
                    objMaterialStagingDetailDTO.BinGUID = null;
                    objMaterialStagingDetailDTO.BinName = null;
                    objMaterialStagingDetailDTO.CompanyID = objMaterialStagingDetailDTO.CompanyID;
                    objMaterialStagingDetailDTO.Created = DateTimeUtility.DateTimeNow;
                    objMaterialStagingDetailDTO.CreatedBy = objMaterialStagingDetailDTO.CreatedBy;
                    objMaterialStagingDetailDTO.CreatedByName = objMaterialStagingDetailDTO.CreatedByName;
                    objMaterialStagingDetailDTO.GUID = Guid.NewGuid();
                    objMaterialStagingDetailDTO.ID = 0;
                    objMaterialStagingDetailDTO.IsArchived = false;
                    objMaterialStagingDetailDTO.IsDeleted = false;
                    objMaterialStagingDetailDTO.ItemGUID = objItem.GUID;
                    objMaterialStagingDetailDTO.ItemNumber = objItem.ItemNumber;
                    objMaterialStagingDetailDTO.LastUpdatedBy = objMaterialStagingDetailDTO.LastUpdatedBy;
                    objMaterialStagingDetailDTO.MaterialStagingGUID = objMaterialStagingDTO.GUID;
                    objMaterialStagingDetailDTO.MaterialStagingName = objMaterialStagingDTO.StagingName;
                    objMaterialStagingDetailDTO.Quantity = objMaterialStagingDetailDTO.Quantity;
                    objMaterialStagingDetailDTO.RoomId = objMaterialStagingDetailDTO.RoomId;
                    objMaterialStagingDetailDTO.RoomName = objMaterialStagingDetailDTO.RoomName;
                    objMaterialStagingDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                    objMaterialStagingDetailDTO.UpdatedByName = objMaterialStagingDetailDTO.UpdatedByName;
                    objMaterialStagingDetailDTO.StagingBinID = (objStagingBin.ID > 0 ? objStagingBin.ID : objMaterialStagingDetailDTO.StagingBinID);
                    objMaterialStagingDetailDTO.StagingBinGUID = (objStagingBin.GUID != Guid.Empty ? objStagingBin.GUID : objMaterialStagingDetailDTO.StagingBinGUID);
                    objMaterialStagingDetailDTO.EditedFrom = "Web";
                    objMaterialStagingDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objMaterialStagingDetailDTO.AddedFrom = "Web";
                    objMaterialStagingDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objMaterialStagingDetailDTO.ID = Insert(objMaterialStagingDetailDTO);
                    return objMaterialStagingDetailDTO;
                }
                else
                {

                }
            }
            else
            {

            }
            return null;
        }

        #region WI-4873

        public bool MSDeleteManageBackInventory(MaterialStagingDetailDTO objDTO, Int64 RoomID, Int64 CompanyID, long UserID, string RoomDateFormat, out string MSG, 
            long SessionUserId,long EnterpriseId)
        {
            MSG = "";
            //get all pull details 
            MaterialStagingPullDetailDAL objMsDetailDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
            List<MaterialStagingPullDetailDTO> lstMSPullDetailsDTO = objMsDetailDAL.GetMsPullDetailsByMsDetailsId(objDTO.GUID);

            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);

            ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);

            PullDetailsDAL objPullDetailsDAL = new PullDetailsDAL(base.DataBaseName);
            BinMasterDAL objBinDAL = new BinMasterDAL(base.DataBaseName);

            foreach (var item in lstMSPullDetailsDTO)
            {
                //item locations details
                if (ItemDTO.SerialNumberTracking)
                {
                    List<PullDetailsDTO> lstPullDetail = null;

                    if (!string.IsNullOrEmpty(item.SerialNumber))
                    {
                        lstPullDetail = objPullDetailsDAL.GetMSPullDetailsByMSPullDetailGuidAndSerialNoPlain(item.ItemGUID, item.GUID, item.SerialNumber);
                    }
                    else
                    {
                        lstPullDetail = objPullDetailsDAL.GetMSPullDetailsByMSPullDetailGuidPlain(item.ItemGUID, item.GUID);
                    }

                    if (lstPullDetail != null && lstPullDetail.Count > 0)
                    {
                        objMsDetailDAL.UpdateMaterialStagingPullDetailIntialQtyandPoolQty(RoomID, CompanyID, UserID, item.GUID, item.ItemGUID, lstPullDetail.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0))));
                    }

                    // if this Bin is delete, select same bin exist for this item
                    BinMasterDTO objBinDTO = objBinDAL.GetBinByID(Convert.ToInt64(item.BinID), RoomID, CompanyID);
                    if (objBinDTO != null && objBinDTO.ItemGUID.GetValueOrDefault(Guid.Empty) == item.ItemGUID && objBinDTO.IsDeleted.GetValueOrDefault(false) == true)
                    {
                        BinMasterDTO objExistBinDTO = objBinDAL.GetBinMasterByItemGUID(RoomID, CompanyID, false, false, objBinDTO.BinNumber, item.ItemGUID);
                        if (objExistBinDTO != null && objExistBinDTO.IsDeleted.GetValueOrDefault(false) == false)
                        {
                            item.BinID = objExistBinDTO.ID;
                        }
                    }

                    ItemLocationDetailsDTO objInsertItemLocationDetails = new ItemLocationDetailsDTO();

                    objInsertItemLocationDetails.BinID = item.BinID;
                    objInsertItemLocationDetails.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                    objInsertItemLocationDetails.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                    objInsertItemLocationDetails.MeasurementID = null;
                    objInsertItemLocationDetails.LotNumber = null;
                    objInsertItemLocationDetails.SerialNumber = item.SerialNumber;
                    objInsertItemLocationDetails.ExpirationDate = null;
                    objInsertItemLocationDetails.Cost = item.Cost;
                    objInsertItemLocationDetails.eVMISensorPort = null;
                    objInsertItemLocationDetails.eVMISensorID = null;
                    objInsertItemLocationDetails.UDF1 = null;
                    objInsertItemLocationDetails.UDF2 = null;
                    objInsertItemLocationDetails.UDF3 = null;
                    objInsertItemLocationDetails.UDF4 = null;
                    objInsertItemLocationDetails.UDF5 = null;
                    objInsertItemLocationDetails.GUID = Guid.NewGuid();
                    objInsertItemLocationDetails.ItemGUID = item.ItemGUID;
                    objInsertItemLocationDetails.CreatedBy = item.CreatedBy;
                    objInsertItemLocationDetails.LastUpdatedBy = item.LastUpdatedBy;
                    objInsertItemLocationDetails.CompanyID = item.CompanyID;
                    objInsertItemLocationDetails.Room = item.Room;
                    objInsertItemLocationDetails.OrderDetailGUID = null;
                    objInsertItemLocationDetails.KitDetailGUID = null;
                    objInsertItemLocationDetails.TransferDetailGUID = null;
                    objInsertItemLocationDetails.ReceivedDate = DateTime.UtcNow;
                    objInsertItemLocationDetails.InsertedFrom = "web";
                    objInsertItemLocationDetails.AddedFrom = "web";
                    objInsertItemLocationDetails.EditedFrom = "web";

                    objLocationDAL.Insert(objInsertItemLocationDetails);

                    UpdateItemStageQuantity(ItemDTO.GUID, ((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0)), -1, RoomID, CompanyID, SessionUserId,EnterpriseId);

                    if (item.BinID != null && item.BinID.GetValueOrDefault(0) > 0)
                        BinUnDelete(Convert.ToInt64(item.BinID), RoomID, CompanyID, item.ItemGUID, item.CustomerOwnedQuantity.GetValueOrDefault(0), item.ConsignedQuantity.GetValueOrDefault(0), SessionUserId,EnterpriseId);
                }
                else if (ItemDTO.LotNumberTracking)
                {
                    List<PullDetailsDTO> lstPullDetail = null;

                    if (!string.IsNullOrEmpty(item.LotNumber))
                    {
                        lstPullDetail = objPullDetailsDAL.GetMSPullDetailsByMSPullDetailGuidAndLotNoPlain(item.ItemGUID, item.GUID, item.LotNumber);
                    }
                    else
                    {
                        lstPullDetail = objPullDetailsDAL.GetMSPullDetailsByMSPullDetailGuidPlain(item.ItemGUID, item.GUID);
                    }

                    if (lstPullDetail != null && lstPullDetail.Count > 0)
                    {
                        objMsDetailDAL.UpdateMaterialStagingPullDetailIntialQtyandPoolQty(RoomID, CompanyID, UserID, item.GUID, item.ItemGUID, lstPullDetail.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0))));
                    }

                    // if this Bin is delete, select same bin exist for this item
                    BinMasterDTO objBinDTO = objBinDAL.GetBinByID(Convert.ToInt64(item.BinID), RoomID, CompanyID);
                    if (objBinDTO != null && objBinDTO.ItemGUID.GetValueOrDefault(Guid.Empty) == item.ItemGUID && objBinDTO.IsDeleted.GetValueOrDefault(false) == true)
                    {
                        BinMasterDTO objExistBinDTO = objBinDAL.GetBinMasterByItemGUID(RoomID, CompanyID, false, false, objBinDTO.BinNumber, item.ItemGUID);
                        if (objExistBinDTO != null && objExistBinDTO.IsDeleted.GetValueOrDefault(false) == false)
                        {
                            item.BinID = objExistBinDTO.ID;
                        }
                    }

                    ItemLocationDetailsDTO objInsertItemLocationDetails = new ItemLocationDetailsDTO();

                    objInsertItemLocationDetails.BinID = item.BinID;
                    objInsertItemLocationDetails.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                    objInsertItemLocationDetails.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                    objInsertItemLocationDetails.MeasurementID = null;
                    objInsertItemLocationDetails.LotNumber = item.LotNumber;
                    objInsertItemLocationDetails.SerialNumber = null;
                    objInsertItemLocationDetails.ExpirationDate = null;
                    objInsertItemLocationDetails.Cost = item.Cost;
                    objInsertItemLocationDetails.eVMISensorPort = null;
                    objInsertItemLocationDetails.eVMISensorID = null;
                    objInsertItemLocationDetails.UDF1 = null;
                    objInsertItemLocationDetails.UDF2 = null;
                    objInsertItemLocationDetails.UDF3 = null;
                    objInsertItemLocationDetails.UDF4 = null;
                    objInsertItemLocationDetails.UDF5 = null;
                    objInsertItemLocationDetails.GUID = Guid.NewGuid();
                    objInsertItemLocationDetails.ItemGUID = item.ItemGUID;
                    objInsertItemLocationDetails.CreatedBy = item.CreatedBy;
                    objInsertItemLocationDetails.LastUpdatedBy = item.LastUpdatedBy;
                    objInsertItemLocationDetails.CompanyID = item.CompanyID;
                    objInsertItemLocationDetails.Room = item.Room;
                    objInsertItemLocationDetails.OrderDetailGUID = null;
                    objInsertItemLocationDetails.KitDetailGUID = null;
                    objInsertItemLocationDetails.TransferDetailGUID = null;
                    objInsertItemLocationDetails.ReceivedDate = DateTime.UtcNow;
                    objInsertItemLocationDetails.InsertedFrom = "web";
                    objInsertItemLocationDetails.AddedFrom = "web";
                    objInsertItemLocationDetails.EditedFrom = "web";

                    objLocationDAL.Insert(objInsertItemLocationDetails);

                    UpdateItemStageQuantity(ItemDTO.GUID, ((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0)), -1, RoomID, CompanyID, SessionUserId,EnterpriseId);

                    if (item.BinID != null && item.BinID.GetValueOrDefault(0) > 0)
                        BinUnDelete(Convert.ToInt64(item.BinID), RoomID, CompanyID, item.ItemGUID, item.CustomerOwnedQuantity.GetValueOrDefault(0), item.ConsignedQuantity.GetValueOrDefault(0), SessionUserId,EnterpriseId);

                }
                else if (ItemDTO.DateCodeTracking)
                {
                    List<PullDetailsDTO> lstPullDetail = null;

                    if (!string.IsNullOrEmpty(item.Expiration))
                    {
                        lstPullDetail = objPullDetailsDAL.GetMSPullDetailsByMSPullDetailGuidAndExpirationPlain(item.ItemGUID, item.GUID, item.Expiration);
                    }
                    else
                    {
                        lstPullDetail = objPullDetailsDAL.GetMSPullDetailsByMSPullDetailGuidPlain(item.ItemGUID, item.GUID);
                    }

                    if (lstPullDetail != null && lstPullDetail.Count > 0)
                    {
                        objMsDetailDAL.UpdateMaterialStagingPullDetailIntialQtyandPoolQty(RoomID, CompanyID, UserID, item.GUID, item.ItemGUID, lstPullDetail.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0))));
                    }

                    // if this Bin is delete, select same bin exist for this item
                    BinMasterDTO objBinDTO = objBinDAL.GetBinByID(Convert.ToInt64(item.BinID), RoomID, CompanyID);
                    if (objBinDTO != null && objBinDTO.ItemGUID.GetValueOrDefault(Guid.Empty) == item.ItemGUID && objBinDTO.IsDeleted.GetValueOrDefault(false) == true)
                    {
                        BinMasterDTO objExistBinDTO = objBinDAL.GetBinMasterByItemGUID(RoomID, CompanyID, false, false, objBinDTO.BinNumber, item.ItemGUID);
                        if (objExistBinDTO != null && objExistBinDTO.IsDeleted.GetValueOrDefault(false) == false)
                        {
                            item.BinID = objExistBinDTO.ID;
                        }
                    }

                    ItemLocationDetailsDTO objInsertItemLocationDetails = new ItemLocationDetailsDTO();

                    if (!string.IsNullOrEmpty(item.Expiration))
                    {
                        item.Expiration = FnCommon.ConvertDateByTimeZone(Convert.ToDateTime(item.Expiration), true, true);

                        DateTime dt = new DateTime();
                        if (DateTime.TryParseExact(item.Expiration, RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out dt))
                            objInsertItemLocationDetails.ExpirationDate = dt;
                    }

                    objInsertItemLocationDetails.BinID = item.BinID;
                    objInsertItemLocationDetails.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                    objInsertItemLocationDetails.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                    objInsertItemLocationDetails.MeasurementID = null;
                    objInsertItemLocationDetails.LotNumber = null;
                    objInsertItemLocationDetails.SerialNumber = null;
                    objInsertItemLocationDetails.Expiration = item.Expiration;
                    objInsertItemLocationDetails.Cost = item.Cost;
                    objInsertItemLocationDetails.eVMISensorPort = null;
                    objInsertItemLocationDetails.eVMISensorID = null;
                    objInsertItemLocationDetails.UDF1 = null;
                    objInsertItemLocationDetails.UDF2 = null;
                    objInsertItemLocationDetails.UDF3 = null;
                    objInsertItemLocationDetails.UDF4 = null;
                    objInsertItemLocationDetails.UDF5 = null;
                    objInsertItemLocationDetails.GUID = Guid.NewGuid();
                    objInsertItemLocationDetails.ItemGUID = item.ItemGUID;
                    objInsertItemLocationDetails.CreatedBy = item.CreatedBy;
                    objInsertItemLocationDetails.LastUpdatedBy = item.LastUpdatedBy;
                    objInsertItemLocationDetails.CompanyID = item.CompanyID;
                    objInsertItemLocationDetails.Room = item.Room;
                    objInsertItemLocationDetails.OrderDetailGUID = null;
                    objInsertItemLocationDetails.KitDetailGUID = null;
                    objInsertItemLocationDetails.TransferDetailGUID = null;
                    objInsertItemLocationDetails.ReceivedDate = DateTime.UtcNow;
                    objInsertItemLocationDetails.InsertedFrom = "Staging-Delete";
                    objInsertItemLocationDetails.AddedFrom = "web";
                    objInsertItemLocationDetails.EditedFrom = "web";

                    objLocationDAL.Insert(objInsertItemLocationDetails);

                    UpdateItemStageQuantity(ItemDTO.GUID, ((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0)), -1, RoomID, CompanyID, SessionUserId,EnterpriseId);

                    if (item.BinID != null && item.BinID.GetValueOrDefault(0) > 0)
                        BinUnDelete(Convert.ToInt64(item.BinID), RoomID, CompanyID, item.ItemGUID, item.CustomerOwnedQuantity.GetValueOrDefault(0), item.ConsignedQuantity.GetValueOrDefault(0), SessionUserId,EnterpriseId);
                }
                else
                {
                    List<PullDetailsDTO> lstPullDetail = objPullDetailsDAL.GetMSPullDetailsByMSPullDetailGuidPlain(item.ItemGUID, item.GUID);

                    if (lstPullDetail != null && lstPullDetail.Count > 0)
                    {
                        objMsDetailDAL.UpdateMaterialStagingPullDetailIntialQtyandPoolQty(RoomID, CompanyID, UserID, item.GUID, item.ItemGUID, lstPullDetail.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0))));
                    }

                    // if this Bin is delete, select same bin exist for this item
                    BinMasterDTO objBinDTO = objBinDAL.GetBinByID(Convert.ToInt64(item.BinID), RoomID, CompanyID);
                    if (objBinDTO != null && objBinDTO.ItemGUID.GetValueOrDefault(Guid.Empty) == item.ItemGUID && objBinDTO.IsDeleted.GetValueOrDefault(false) == true)
                    {
                        BinMasterDTO objExistBinDTO = objBinDAL.GetBinMasterByItemGUID(RoomID, CompanyID, false, false, objBinDTO.BinNumber, item.ItemGUID);
                        if (objExistBinDTO != null && objExistBinDTO.IsDeleted.GetValueOrDefault(false) == false)
                        {
                            item.BinID = objExistBinDTO.ID;
                        }
                    }

                    ItemLocationDetailsDTO objInsertItemLocationDetails = new ItemLocationDetailsDTO();

                    objInsertItemLocationDetails.BinID = item.BinID;
                    objInsertItemLocationDetails.CustomerOwnedQuantity = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                    objInsertItemLocationDetails.ConsignedQuantity = item.ConsignedQuantity.GetValueOrDefault(0);
                    objInsertItemLocationDetails.MeasurementID = null;
                    objInsertItemLocationDetails.LotNumber = null;
                    objInsertItemLocationDetails.SerialNumber = null;
                    objInsertItemLocationDetails.ExpirationDate = null;
                    objInsertItemLocationDetails.Cost = item.Cost;
                    objInsertItemLocationDetails.eVMISensorPort = null;
                    objInsertItemLocationDetails.eVMISensorID = null;
                    objInsertItemLocationDetails.UDF1 = null;
                    objInsertItemLocationDetails.UDF2 = null;
                    objInsertItemLocationDetails.UDF3 = null;
                    objInsertItemLocationDetails.UDF4 = null;
                    objInsertItemLocationDetails.UDF5 = null;
                    objInsertItemLocationDetails.GUID = Guid.NewGuid();
                    objInsertItemLocationDetails.ItemGUID = item.ItemGUID;
                    objInsertItemLocationDetails.CreatedBy = item.CreatedBy;
                    objInsertItemLocationDetails.LastUpdatedBy = item.LastUpdatedBy;
                    objInsertItemLocationDetails.CompanyID = item.CompanyID;
                    objInsertItemLocationDetails.Room = item.Room;
                    objInsertItemLocationDetails.OrderDetailGUID = null;
                    objInsertItemLocationDetails.KitDetailGUID = null;
                    objInsertItemLocationDetails.TransferDetailGUID = null;
                    objInsertItemLocationDetails.ReceivedDate = DateTime.UtcNow;
                    objInsertItemLocationDetails.InsertedFrom = "Staging-Delete";
                    objInsertItemLocationDetails.AddedFrom = "web";
                    objInsertItemLocationDetails.EditedFrom = "web";

                    objLocationDAL.Insert(objInsertItemLocationDetails);

                    UpdateItemStageQuantity(ItemDTO.GUID, ((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0)), -1, RoomID, CompanyID, SessionUserId,EnterpriseId);

                    if (item.BinID != null && item.BinID.GetValueOrDefault(0) > 0)
                        BinUnDelete(Convert.ToInt64(item.BinID), RoomID, CompanyID, item.ItemGUID, item.CustomerOwnedQuantity.GetValueOrDefault(0), item.ConsignedQuantity.GetValueOrDefault(0), SessionUserId,EnterpriseId);
                }

                //item locations qty update
                //ItemLocationQTYDTO lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == item.BinID && x.ItemGUID == item.ItemGUID).SingleOrDefault();
                ItemLocationQTYDTO lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, item.BinID,Convert.ToString(item.ItemGUID)).FirstOrDefault();
                if (lstLocDTO != null)
                {
                    lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                    lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0);

                    lstLocDTO.Quantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + lstLocDTO.ConsignedQuantity.GetValueOrDefault(0);
                    List<ItemLocationQTYDTO> lst = new List<ItemLocationQTYDTO>();
                    lst.Add(lstLocDTO);
                    objLocQTY.Save(lst, SessionUserId,EnterpriseId);
                }
                deleteMsPullDtlRec(item);
            }

            return true;
        }

        public void BinUnDelete(long BinID, long RoomID, long CompanyID, Guid ItemGUID, double? CustomerOwnedQuantity, double? ConsignedQuantity, long SessionUserId,long EnterpriseId)
        {
            BinMasterDAL objBinDAL = new BinMasterDAL(base.DataBaseName);
            BinMasterDTO objBinDTO = objBinDAL.GetBinByID(BinID, RoomID, CompanyID);
            if (objBinDTO != null && objBinDTO.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemGUID && objBinDTO.IsDeleted.GetValueOrDefault(false) == true)
            {
                //objBinDTO.IsDeleted = false;
                //objBinDTO.EditedFrom = "Staging-Delete-BinUnDelete";
                //objBinDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                //objBinDAL.UndeleteLocation(objBinDTO);
                objBinDAL.UpdateData(BinID, "0", 0, 0, 0, "IsDeleted");

                ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
                //ItemLocationQTYDTO lstLocDTO = objLocQTY.GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == BinID && x.ItemGUID == ItemGUID).SingleOrDefault();
                ItemLocationQTYDTO lstLocDTO = objLocQTY.GetItemLocationQTY(RoomID, CompanyID, BinID, Convert.ToString(ItemGUID)).FirstOrDefault();
                if (lstLocDTO != null)
                {
                    lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + CustomerOwnedQuantity.GetValueOrDefault(0);
                    lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) + ConsignedQuantity.GetValueOrDefault(0);

                    lstLocDTO.Quantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + lstLocDTO.ConsignedQuantity.GetValueOrDefault(0);
                    List<ItemLocationQTYDTO> lst = new List<ItemLocationQTYDTO>();
                    lst.Add(lstLocDTO);
                    objLocQTY.Save(lst, SessionUserId,EnterpriseId);
                }
                else
                {
                    lstLocDTO = new ItemLocationQTYDTO();

                    lstLocDTO.BinID = BinID;
                    lstLocDTO.CustomerOwnedQuantity = CustomerOwnedQuantity.GetValueOrDefault(0);
                    lstLocDTO.ConsignedQuantity = ConsignedQuantity.GetValueOrDefault(0);
                    lstLocDTO.Quantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + lstLocDTO.ConsignedQuantity.GetValueOrDefault(0);
                    lstLocDTO.GUID = Guid.NewGuid();
                    lstLocDTO.ItemGUID = ItemGUID;
                    lstLocDTO.Created = DateTimeUtility.DateTimeNow;
                    lstLocDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    lstLocDTO.CreatedBy = objBinDTO.LastUpdatedBy;
                    lstLocDTO.LastUpdatedBy = objBinDTO.LastUpdatedBy;
                    lstLocDTO.Room = RoomID;
                    lstLocDTO.CompanyID = CompanyID;

                    List<ItemLocationQTYDTO> lst = new List<ItemLocationQTYDTO>();
                    lst.Add(lstLocDTO);
                    objLocQTY.Save(lst, SessionUserId,EnterpriseId);
                }

            }
        }
        #endregion
        public List<MaterialStagingDetailDTO> GetMaterialStagingDetailByMaterialStagingGUIDAndItemGUIDAndBinIDPlain(Guid MaterialStagingGUID, Guid ItemGUID, long? BinID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@MaterialStagingGUID", MaterialStagingGUID), new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@BinID", BinID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<MaterialStagingDetailDTO>("exec [GetMaterialStagingDetailByMaterialStagingGUIDAndItemGUIDAndBinIDPlain] @MaterialStagingGUID,@ItemGUID,@BinID", params1).ToList();
    }
}
        public List<MaterialStagingDetailDTO> GetMaterialStagingDetailByItemGUIDAndBinIDPlain(Guid ItemGUID, long? BinID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@BinID", BinID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<MaterialStagingDetailDTO>("exec [GetMaterialStagingDetailByItemGUIDAndBinIDPlain] @ItemGUID,@BinID", params1).ToList();
            }
        }


        public List<MaterialStagingDetailDTO> GetStagLimitedInfo(Guid ItemGuid, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGuid", ItemGuid) };
                List<MaterialStagingDetailDTO> obj = (from u in context.Database.SqlQuery<MaterialStagingDetailDTO>("exec [GetStagLimitedInfo] @RoomID,@CompanyID,@ItemGuid", params1)
                                                      select new MaterialStagingDetailDTO
                                                      {
                                                          MaterialStagingName = u.MaterialStagingName,
                                                          StagingBinName = u.StagingBinName,
                                                      }).AsParallel().ToList();
                return obj;
            }

        }

    }
}


