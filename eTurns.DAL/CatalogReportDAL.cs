using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public partial class CatalogReportDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public CatalogReportDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public CatalogReportDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];

        #region CatalogReportDetailCrude

        public IEnumerable<CatalogReportDetailDTO> GetCatalogReportDetail(Int64 CompanyID, Int64 ID, bool? IsDeleted, bool? IsArchived)
        {
            List<CatalogReportDetailDTO> lstItems = new List<CatalogReportDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID),  
                                                   new SqlParameter("@ID", ID),
                                                   new SqlParameter("@IsDeleted", IsDeleted), 
                                                   new SqlParameter("@IsArchived", IsArchived) 
                                                 };

                lstItems = (from u in context.Database.SqlQuery<CatalogReportDetailDTO>("EXEC dbo.GetAllCatalogReportDetail @CompanyID,@ID,@IsDeleted,@IsArchived", params1)
                            select new CatalogReportDetailDTO
                            {
                                ID = u.ID,
                                Name = u.Name,
                                IncludeBin = u.IncludeBin,
                                IncludeQuantity = u.IncludeQuantity,
                                QuantityField = u.QuantityField,
                                BarcodeKey = u.BarcodeKey,
                                LabelHTML = u.LabelHTML,
                                LabelXML = u.LabelXML,
                                FontSize = u.FontSize,
                                CompanyID = u.CompanyID,
                                CreatedBy = u.CreatedBy,
                                UpdatedBy = u.UpdatedBy,
                                CreatedOn = u.CreatedOn,
                                UpdatedOn = u.UpdatedOn,
                                IsArchived = u.IsArchived,
                                IsDeleted = u.IsDeleted,
                                CreatedByName = u.CreatedByName,
                                UpdatedByName = u.UpdatedByName,
                                TemplateID = u.TemplateID,
                                SelectedFields = u.SelectedFields,
                                BarcodeFont = u.BarcodeFont,
                                TextFont = u.TextFont,
                                BarcodePattern = u.BarcodePattern,
                                arrFieldIds = u.arrFieldIds,
                                CreatedDate = u.CreatedDate,
                                lstSelectedModuleFields = u.lstSelectedModuleFields,
                                UpdatedDate = u.UpdatedDate,
                                RoomID = u.RoomID,
                                TemplateName = u.TemplateName,
                                TotalRecords = u.TotalRecords,
                            }).AsParallel().ToList();
                return lstItems;
            }

        }

        /// <summary>
        /// Get Paged Records from the LabelFieldModuleTemplateDetail Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<CatalogReportDetailDTO> GetPagedRecordsDB(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 CompanyID, bool IsArchived, bool IsDeleted, string Roomformat, TimeZoneInfo CurrentTimeZone)
        {
            List<CatalogReportDetailDTO> lstItems = new List<CatalogReportDetailDTO>();
            TotalCount = 0;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string Creators = null;
            string Updators = null;

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[^]"))
            {
                int idx = SearchTerm.IndexOf("[^]");
                SearchTerm = SearchTerm.Substring(0, idx);
            }

            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                string[] FieldsVal = Fields[1].Split('~');
                string newSearchValue = string.Empty;
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
                SearchTerm = newSearchValue;


                if (FieldsVal.Count() > 1 && !string.IsNullOrWhiteSpace(FieldsVal[0]))
                {
                    Creators = FieldsVal[0].TrimEnd(',');
                }
                if (FieldsVal.Count() > 2 && !string.IsNullOrWhiteSpace(FieldsVal[1]))
                {
                    Updators = FieldsVal[1].TrimEnd(',');
                }

                if (FieldsVal.Count() > 3 && !string.IsNullOrWhiteSpace(FieldsVal[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsVal[2].Split(',')[0], Roomformat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsVal[2].Split(',')[1], Roomformat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (FieldsVal.Count() > 4 && !string.IsNullOrWhiteSpace(FieldsVal[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsVal[3].Split(',')[0], Roomformat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsVal[3].Split(',')[1], Roomformat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
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
                                               new SqlParameter("@IsDeleted", IsDeleted), 
                                               new SqlParameter("@IsArchived", IsArchived), 
                                               new SqlParameter("@CompanyID", CompanyID), 
                                             };


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                lstItems = (from u in context.Database.SqlQuery<CatalogReportDetailDTO>("EXEC dbo.GetPagedCatalogReportDetail @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@CreatedBy,@UpdatedFrom,@UpdatedTo,@LastUpdatedBy,@IsDeleted,@IsArchived,@CompanyID", params1)
                            select new CatalogReportDetailDTO
                            {
                                ID = u.ID,
                                Name = u.Name,
                                IncludeBin = u.IncludeBin,
                                IncludeQuantity = u.IncludeQuantity,
                                QuantityField = u.QuantityField,
                                BarcodeKey = u.BarcodeKey,
                                LabelHTML = u.LabelHTML,
                                LabelXML = u.LabelXML,
                                FontSize = u.FontSize,
                                CompanyID = u.CompanyID,
                                CreatedBy = u.CreatedBy,
                                UpdatedBy = u.UpdatedBy,
                                CreatedOn = u.CreatedOn,
                                UpdatedOn = u.UpdatedOn,
                                IsArchived = u.IsArchived,
                                IsDeleted = u.IsDeleted,
                                CreatedByName = u.CreatedByName,
                                UpdatedByName = u.UpdatedByName,
                                TemplateID = u.TemplateID,
                                SelectedFields = u.SelectedFields,
                                BarcodeFont = u.BarcodeFont,
                                TextFont = u.TextFont,
                                BarcodePattern = u.BarcodePattern,
                                arrFieldIds = u.arrFieldIds,
                                CreatedDate = u.CreatedDate,
                                lstSelectedModuleFields = u.lstSelectedModuleFields,
                                UpdatedDate = u.UpdatedDate,
                                RoomID = u.RoomID,
                                TemplateName = u.TemplateName,
                                TotalRecords = u.TotalRecords,
                            }).AsParallel().ToList();


                TotalCount = 0;
                if (lstItems != null && lstItems.Count > 0)
                {
                    TotalCount = lstItems.First().TotalRecords;
                }

                return lstItems;
            }


        }

        /// <summary>
        /// Get Particullar Record from the LabelFieldModuleTemplateDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>


        public CatalogReportDetailDTO GetCatalogReportDetailByID(Int64 ID, Int64 CompanyID)
        {
            CatalogReportDetailDTO objDTO = new CatalogReportDetailDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", ID),
                                                   new SqlParameter("@CompanyID", CompanyID)  
                                                 };

                objDTO = (from u in context.Database.SqlQuery<CatalogReportDetailDTO>("EXEC dbo.GetCatalogReportDetailByID @ID,@CompanyID", params1)
                          select new CatalogReportDetailDTO
                          {
                              ID = u.ID,
                              Name = u.Name,
                              IncludeBin = u.IncludeBin,
                              IncludeQuantity = u.IncludeQuantity,
                              QuantityField = u.QuantityField,
                              BarcodeKey = u.BarcodeKey,
                              LabelHTML = u.LabelHTML,
                              LabelXML = u.LabelXML,
                              FontSize = u.FontSize,
                              CompanyID = u.CompanyID,
                              CreatedBy = u.CreatedBy,
                              UpdatedBy = u.UpdatedBy,
                              CreatedOn = u.CreatedOn,
                              UpdatedOn = u.UpdatedOn,
                              IsArchived = u.IsArchived,
                              IsDeleted = u.IsDeleted,
                              CreatedByName = u.CreatedByName,
                              UpdatedByName = u.UpdatedByName,
                              TemplateID = u.TemplateID,
                              SelectedFields = u.SelectedFields,
                              BarcodeFont = u.BarcodeFont,
                              TextFont = u.TextFont,
                              BarcodePattern = u.BarcodePattern,
                              arrFieldIds = u.arrFieldIds,
                              CreatedDate = u.CreatedDate,
                              lstSelectedModuleFields = u.lstSelectedModuleFields,
                              UpdatedDate = u.UpdatedDate,
                              RoomID = u.RoomID,
                              TemplateName = u.TemplateName,
                              TotalRecords = u.TotalRecords,
                          }).FirstOrDefault();
                return objDTO;
            }

        }

        /// <summary>
        /// GetRecord
        /// </summary>
        /// <param name="Templatename"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public CatalogReportDetailDTO GetCatalogReportDetailByTemplateName(string TemplateName, Int64 CompanyID)
        {
            CatalogReportDetailDTO objDTO = new CatalogReportDetailDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TemplateName", TemplateName),
                                                   new SqlParameter("@CompanyID", CompanyID)  
                                                 };

                objDTO = (from u in context.Database.SqlQuery<CatalogReportDetailDTO>("EXEC dbo.GetCatalogReportDetailByName @TemplateName,@CompanyID", params1)
                          select new CatalogReportDetailDTO
                          {
                              ID = u.ID,
                              Name = u.Name,
                              IncludeBin = u.IncludeBin,
                              IncludeQuantity = u.IncludeQuantity,
                              QuantityField = u.QuantityField,
                              BarcodeKey = u.BarcodeKey,
                              LabelHTML = u.LabelHTML,
                              LabelXML = u.LabelXML,
                              FontSize = u.FontSize,
                              CompanyID = u.CompanyID,
                              CreatedBy = u.CreatedBy,
                              UpdatedBy = u.UpdatedBy,
                              CreatedOn = u.CreatedOn,
                              UpdatedOn = u.UpdatedOn,
                              IsArchived = u.IsArchived,
                              IsDeleted = u.IsDeleted,
                              CreatedByName = u.CreatedByName,
                              UpdatedByName = u.UpdatedByName,
                              TemplateID = u.TemplateID,
                              SelectedFields = u.SelectedFields,
                              BarcodeFont = u.BarcodeFont,
                              TextFont = u.TextFont,
                              BarcodePattern = u.BarcodePattern,
                              arrFieldIds = u.arrFieldIds,
                              CreatedDate = u.CreatedDate,
                              lstSelectedModuleFields = u.lstSelectedModuleFields,
                              UpdatedDate = u.UpdatedDate,
                              RoomID = u.RoomID,
                              TemplateName = u.TemplateName,
                              TotalRecords = u.TotalRecords,
                          }).FirstOrDefault();
                return objDTO;
            }

        }

        /// <summary>
        /// Insert Record in the DataBase LabelFieldModuleTemplateDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(CatalogReportDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CatalogReportDetail obj = new CatalogReportDetail();
                if (string.IsNullOrEmpty(objDTO.SelectedFields))
                    objDTO.SelectedFields = "";

                obj.ID = 0;
                obj.Name = objDTO.Name;
                obj.SelectedFields = objDTO.SelectedFields;
                obj.IncludeBin = objDTO.IncludeBin;
                obj.TemplateID = objDTO.TemplateID;
                obj.IncludeQuantity = objDTO.IncludeQuantity;
                obj.QuantityField = objDTO.QuantityField;
                obj.BarcodeKey = objDTO.BarcodeKey;
                obj.LabelHTML = objDTO.LabelHTML;
                obj.LabelXML = objDTO.LabelXML;
                obj.FontSize = objDTO.FontSize;
                obj.CompanyID = objDTO.CompanyID;

                obj.CreatedBy = objDTO.CreatedBy;
                obj.UpdatedBy = objDTO.UpdatedBy;
                obj.CreatedOn = DateTimeUtility.DateTimeNow;
                obj.UpdatedOn = DateTimeUtility.DateTimeNow;
                obj.IsArchived = false;
                obj.IsDeleted = false;
                obj.BarcodeFont = objDTO.BarcodeFont;
                obj.TextFont = objDTO.TextFont;
                obj.BarcodePattern = objDTO.BarcodePattern;
                context.CatalogReportDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                return obj.ID;
            }


        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(CatalogReportDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CatalogReportDetail obj = new CatalogReportDetail();
                if (string.IsNullOrEmpty(objDTO.SelectedFields))
                    objDTO.SelectedFields = "";

                obj = context.CatalogReportDetails.FirstOrDefault(x => x.ID == objDTO.ID);
                obj.Name = objDTO.Name;
                obj.TemplateID = objDTO.TemplateID;
                obj.SelectedFields = objDTO.SelectedFields;
                obj.IncludeBin = objDTO.IncludeBin;
                obj.IncludeQuantity = objDTO.IncludeQuantity;
                obj.QuantityField = objDTO.QuantityField;
                obj.BarcodeKey = objDTO.BarcodeKey;
                obj.LabelHTML = objDTO.LabelHTML;
                obj.LabelXML = objDTO.LabelXML;
                obj.FontSize = objDTO.FontSize;
                obj.CompanyID = objDTO.CompanyID;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.UpdatedBy = objDTO.UpdatedBy;
                obj.CreatedOn = objDTO.CreatedOn;
                obj.UpdatedOn = DateTimeUtility.DateTimeNow;
                obj.IsArchived = false;
                obj.IsDeleted = false;
                obj.BarcodeFont = objDTO.BarcodeFont;
                obj.TextFont = objDTO.TextFont;
                obj.BarcodePattern = objDTO.BarcodePattern;
                context.SaveChanges();
                objDTO.ID = obj.ID;


                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string DeleteRecords(string IDs, Int64 userid, Int64 CompanyID,long RoomID,long UserId,long EnterpriseId)
        {
            if (string.IsNullOrEmpty(IDs) || IDs.Trim().Length <= 0)
            {
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }

                    }
                }
                else
                {
                    eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserId);
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileLabelPrinting = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResLabelPrinting", currentCulture, EnterpriseId, CompanyID);
                string NoRecordSelected = ResourceRead.GetResourceValueByKeyAndFullFilePath("NoRecordSelected", ResourceFileLabelPrinting,EnterpriseId,CompanyID,RoomID, "ResLabelPrinting",currentCulture);
                return NoRecordSelected;
            }
            string msgs = "";
            string msgf = "";
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                Int64[] intIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();

                foreach (var item in intIDs)
                {
                    if (item > 0)
                    {
                        CatalogReportDetailDTO objFDTO = GetCatalogReportDetailByID(Convert.ToInt64(item), CompanyID);

                        CatalogReportDetailDTO objDTO = GetCatalogReportDetailByID(Convert.ToInt64(item), CompanyID);
                        if (objDTO != null && objDTO.ID > 0)
                        {

                            var params1 = new SqlParameter[] { new SqlParameter("@ID", item),
                                                               new SqlParameter("@UserID", userid)
                                                             };
                            context.Database.SqlQuery<int>("exec [UpdateCatalogReportDetail] @ID,@UserID", params1).FirstOrDefault();
                            if (!string.IsNullOrEmpty(msgs))
                                msgs += ",";

                            msgs += objFDTO.Name;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(msgf))
                                msgf += ",";

                            msgf += objFDTO.Name;
                        }
                    }
                }
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }

                    }
                }
                else
                {
                    eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, UserId);
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResCommon", currentCulture, EnterpriseId, CompanyID);
                if (!string.IsNullOrEmpty(msgs))
                {
                    string MsgOk = ResourceRead.GetResourceValueByKeyAndFullFilePath("Ok", ResourceFileCommon,EnterpriseId,CompanyID,RoomID, "ResCommon",currentCulture);
                    msg = MsgOk;
                }
                if (!string.IsNullOrEmpty(msgf))
                {
                    string MsgNotDeleteSuccess = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgNotDeleteSuccess", ResourceFileCommon, EnterpriseId, CompanyID, RoomID, "ResCommon", currentCulture);
                    msg += msgf + " "+ MsgNotDeleteSuccess;
                }
                return msg;
            }
        }

        /// <summary>
        /// Check Duplicate 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ActionMode"></param>
        /// <param name="ID"></param>
        /// <param name="TableName"></param>
        /// <param name="FieldName"></param>
        /// <param name="RoomID"></param>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        public string DuplicateCheck(string Name, Int64 ID, Int64 CompanyID)
        {
            Name = Name.Replace("'", "''");
            CatalogReportDetailDTO dto = null;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                IEnumerable<CatalogReportDetailDTO> lst = GetCatalogReportDetail(CompanyID, 0, false, false);
                string Msg = "ok";
                if (ID <= 0)
                {
                    dto = lst.FirstOrDefault(x => x.Name == Name);
                }
                else
                {
                    dto = lst.FirstOrDefault(x => x.Name == Name && x.ID != ID);
                }


                if (dto != null && dto.ID > 0)
                {
                    Msg = "duplicate";
                }

                return Msg;


            }
        }


        public CatalogReportDetailDTO GetDefaultCatalogReport(long CompanyID)
        {

            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CatalogReportDetailDTO obj = (from u in context.Database.SqlQuery<CatalogReportDetailDTO>(@"exec GetDefaultCatalogReport @CompanyID", params1)
                                              select new CatalogReportDetailDTO
                                              {
                                                  ID = u.ID,
                                                  Name = u.Name,
                                                  IncludeBin = u.IncludeBin,
                                                  IncludeQuantity = u.IncludeQuantity,
                                                  QuantityField = u.QuantityField,
                                                  BarcodeKey = u.BarcodeKey,
                                                  LabelHTML = u.LabelHTML,
                                                  LabelXML = u.LabelXML,
                                                  FontSize = u.FontSize,
                                                  CompanyID = u.CompanyID,
                                                  CreatedBy = u.CreatedBy,
                                                  UpdatedBy = u.UpdatedBy,
                                                  CreatedOn = u.CreatedOn,
                                                  UpdatedOn = u.UpdatedOn,
                                                  IsArchived = u.IsArchived,
                                                  IsDeleted = u.IsDeleted,
                                                  CreatedByName = u.CreatedByName,
                                                  UpdatedByName = u.UpdatedByName,
                                                  TemplateID = u.TemplateID,
                                                  SelectedFields = u.SelectedFields,
                                                  BarcodeFont = u.BarcodeFont,
                                                  TextFont = u.TextFont,
                                                  BarcodePattern = u.BarcodePattern,
                                                  arrFieldIds = u.arrFieldIds,
                                                  CreatedDate = u.CreatedDate,
                                                  lstSelectedModuleFields = u.lstSelectedModuleFields,
                                                  UpdatedDate = u.UpdatedDate,
                                                  RoomID = u.RoomID,
                                                  TemplateName = u.TemplateName,
                                              }).AsParallel().FirstOrDefault();
                return obj;
            }
        }

        #endregion

        #region Get Items Data From View
        public List<string> GetListByGrouping(string GroupNameField, string GroupIDField, Int64 RoomID, string ItemIDs = "")
        {
            if (!string.IsNullOrWhiteSpace(ItemIDs))
            {
                ItemIDs = ItemIDs.TrimEnd(',');
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<IdNameDTO> objList = new List<IdNameDTO>();
                var params1 = new SqlParameter[] { new SqlParameter("@GroupIDField", GroupIDField),  
                                                   new SqlParameter("@GroupNameField", GroupNameField),
                                                   new SqlParameter("@RoomID", RoomID), 
                                                   new SqlParameter("@ItemIDs", ItemIDs) 
                                                 };

                objList = context.Database.SqlQuery<IdNameDTO>("EXEC [GetCatalogListByGrouping] @GroupIDField,@GroupNameField,@RoomID,@ItemIDs", params1).ToList();
                List<string> obj = objList.Select(x => x.Name).ToList<string>();
                return obj;
            }
        }

        public List<CatalogReportFieldsDTO> GetItemsByGroupNameAndId(string FilterBy, string GroupNameField, string GroupIDField, Int64 RoomID, string SortField, bool isInActiveItems = true, string ItemIDs = "", string ItemBinIDs = "")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GroupNameField", GroupNameField),  
                                               new SqlParameter("@FilterBy", FilterBy), 
                                               new SqlParameter("@RoomID", RoomID), 
                                               new SqlParameter("@ItemIDs", ItemIDs),
                                               new SqlParameter("@ItemBinIDs", ItemBinIDs),
                                               new SqlParameter("@SortField", SortField),
                                               new SqlParameter("@isInActiveItems", isInActiveItems),
                                             };

                List<CatalogReportFieldsDTO> obj = context.Database.SqlQuery<CatalogReportFieldsDTO>("EXEC [GetCatalogItemsByGroupNameAndId] @GroupNameField,@FilterBy,@RoomID,@ItemIDs,@ItemBinIDs,@SortField,@isInActiveItems", params1).ToList();
                return obj;
            }
        }

        public List<CatalogReportFieldsDTO> GetItemsByGroupNameAndId(Int64 RoomID, string SortField, bool isInActiveItems = true, string ItemIDs = "", string ItemBinIDs = "")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GroupNameField", DBNull.Value),  
                                               new SqlParameter("@FilterBy", DBNull.Value), 
                                               new SqlParameter("@RoomID", RoomID), 
                                               new SqlParameter("@ItemIDs", ItemIDs),
                                               new SqlParameter("@ItemBinIDs", ItemBinIDs),
                                               new SqlParameter("@SortField", SortField),
                                               new SqlParameter("@isInActiveItems", isInActiveItems),
                                             };

                List<CatalogReportFieldsDTO> obj = context.Database.SqlQuery<CatalogReportFieldsDTO>("EXEC [GetCatalogItemsByGroupNameAndId] @GroupNameField,@FilterBy,@RoomID,@ItemIDs,@ItemBinIDs,@SortField,@isInActiveItems", params1).ToList();
                return obj;
            }
        }


        #endregion
    }

    public partial class CatalogReportTemplateMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public CatalogReportTemplateMasterDAL(base.DataBaseName)
        //{

        //}

        public CatalogReportTemplateMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public CatalogReportTemplateMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        /// <summary>
        /// </summary>
        /// <returns></returns>
        

        public IEnumerable<CatalogReportTemplateMasterDTO> GetAllCatalogReportTemplateMaster()
        {
            List<CatalogReportTemplateMasterDTO> lstTemplateMaster = new List<CatalogReportTemplateMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<CatalogReportTemplateMasterDTO> obj = (from u in context.Database.SqlQuery<CatalogReportTemplateMasterDTO>("EXEC GetAllCatalogReportTemplateMaster")
                                                                   select new CatalogReportTemplateMasterDTO
                                                                   {
                                                                       ID = u.ID,
                                                                       TemplateName = u.TemplateName,
                                                                       LabelSize = u.LabelSize,
                                                                       NoOfLabelPerSheet = u.NoOfLabelPerSheet,
                                                                       NoOfColumns = u.NoOfColumns,
                                                                       PageWidth = u.PageWidth,
                                                                       PageHeight = u.PageHeight,
                                                                       LabelWidth = u.LabelWidth,
                                                                       LabelHeight = u.LabelHeight,
                                                                       PageMarginLeft = u.PageMarginLeft,
                                                                       PageMarginRight = u.PageMarginRight,
                                                                       PageMarginTop = u.PageMarginTop,
                                                                       PageMarginBottom = u.PageMarginBottom,
                                                                       LabelSpacingHorizontal = u.LabelSpacingHorizontal,
                                                                       LabelSpacingVerticle = u.LabelSpacingVerticle,
                                                                       LabelPaddingLeft = u.LabelPaddingLeft,
                                                                       LabelPaddingRight = u.LabelPaddingRight,
                                                                       LabelPaddingTop = u.LabelPaddingTop,
                                                                       LabelPaddingBottom = u.LabelPaddingBottom,
                                                                       LabelType = u.LabelType,
                                                                       TemplateNameWithSize = u.TemplateName + " - " + u.LabelSize,//.Replace("\"", "")
                                                                       CompanyID = u.CompanyID,
                                                                       CreatedByName = string.Empty,
                                                                       RoomName = string.Empty,
                                                                       TemplateID = u.TemplateID,
                                                                       UpdatedByName = string.Empty,
                                                                   }).AsParallel().ToList();
            }
            return lstTemplateMaster;
        }

        /// <summary>
        /// copy all Base Template
        /// Call This Function only one time When new Company Created
        /// </summary>
        /// <param name="CompanyID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public int CopyAllBaseTemplate(Int64 CompanyID, Int64 UserID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
                int rowAffected = context.Database.ExecuteSqlCommand("exec CopyAllCatalogBaseTemplate @CompanyID", params1);
                if (rowAffected > 0)
                {

                }

                return rowAffected;
            }
        }

        /// <summary>
        /// Get Paged Records from the CatalogReportTemplateMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<CatalogReportTemplateMasterDTO> GetCatalogReportTemplateMasterByCompanyID(long CompanyID)
        {
            List<CatalogReportTemplateMasterDTO> lstTemplateMaster = new List<CatalogReportTemplateMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
                lstTemplateMaster = (from u in context.Database.SqlQuery<CatalogReportTemplateMasterDTO>("EXEC [GetCatalogReportTemplateMasterByCompanyID] @CompanyID", params1)
                                                                   select new CatalogReportTemplateMasterDTO
                                                                   {
                                                                       ID = u.ID,
                                                                       TemplateName = u.TemplateName,
                                                                       LabelSize = u.LabelSize,
                                                                       NoOfLabelPerSheet = u.NoOfLabelPerSheet,
                                                                       NoOfColumns = u.NoOfColumns,
                                                                       PageWidth = u.PageWidth,
                                                                       PageHeight = u.PageHeight,
                                                                       LabelWidth = u.LabelWidth,
                                                                       LabelHeight = u.LabelHeight,
                                                                       PageMarginLeft = u.PageMarginLeft,
                                                                       PageMarginRight = u.PageMarginRight,
                                                                       PageMarginTop = u.PageMarginTop,
                                                                       PageMarginBottom = u.PageMarginBottom,
                                                                       LabelSpacingHorizontal = u.LabelSpacingHorizontal,
                                                                       LabelSpacingVerticle = u.LabelSpacingVerticle,
                                                                       LabelPaddingLeft = u.LabelPaddingLeft,
                                                                       LabelPaddingRight = u.LabelPaddingRight,
                                                                       LabelPaddingTop = u.LabelPaddingTop,
                                                                       LabelPaddingBottom = u.LabelPaddingBottom,
                                                                       LabelType = u.LabelType,
                                                                       TemplateNameWithSize = u.TemplateName + " - " + u.LabelSize,//.Replace("\"", "")
                                                                       CompanyID = u.CompanyID,
                                                                       CreatedByName = string.Empty,
                                                                       RoomName = string.Empty,
                                                                       TemplateID = u.TemplateID,
                                                                       UpdatedByName = string.Empty,
                                                                   }).AsParallel().ToList();


            }
            return lstTemplateMaster;
        }
        /// <summary>
        /// Get Paged Records from the CatalogReportTemplateMasterDTO Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<CatalogReportTemplateMasterDTO> GetMasterDataPagedRecordsDB(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            List<CatalogReportTemplateMasterDTO> lstItems = new List<CatalogReportTemplateMasterDTO>();
            TotalCount = 0;

            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                if (Fields.Length > 2)
                {
                    SearchTerm = Fields[2];
                }
                else
                {
                    SearchTerm = string.Empty;
                }
            }
            else
            {
                SearchTerm = string.Empty;
            }

            var params1 = new SqlParameter[] { new SqlParameter("@StartRowIndex", StartRowIndex), 
                                               new SqlParameter("@MaxRows", MaxRows), 
                                               new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value), 
                                               new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value), 
                                               new SqlParameter("@IsDeleted", IsDeleted), 
                                               new SqlParameter("@IsArchived", IsArchived), 
                                               new SqlParameter("@CompanyID", CompanyID), 
                                             };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                lstItems = (from u in context.Database.SqlQuery<CatalogReportTemplateMasterDTO>(@"EXEC dbo.GetPagedCatalogReportTemplateMaster @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@IsDeleted,@IsArchived,@CompanyID", params1)
                            select new CatalogReportTemplateMasterDTO
                            {
                                ID = u.ID,
                                TemplateName = u.TemplateName,
                                LabelSize = u.LabelSize,
                                NoOfLabelPerSheet = u.NoOfLabelPerSheet,
                                NoOfColumns = u.NoOfColumns,
                                PageWidth = u.PageWidth,
                                PageHeight = u.PageHeight,
                                LabelWidth = u.LabelWidth,
                                LabelHeight = u.LabelHeight,
                                PageMarginLeft = u.PageMarginLeft,
                                PageMarginRight = u.PageMarginRight,
                                PageMarginTop = u.PageMarginTop,
                                PageMarginBottom = u.PageMarginBottom,
                                LabelSpacingHorizontal = u.LabelSpacingHorizontal,
                                LabelSpacingVerticle = u.LabelSpacingVerticle,
                                LabelPaddingLeft = u.LabelPaddingLeft,
                                LabelPaddingRight = u.LabelPaddingRight,
                                LabelPaddingTop = u.LabelPaddingTop,
                                LabelPaddingBottom = u.LabelPaddingBottom,
                                LabelType = u.LabelType,
                                TemplateNameWithSize = u.TemplateName + " - " + u.LabelSize,//.Replace("\"", "")
                                CompanyID = u.CompanyID,
                                CreatedByName = string.Empty,
                                RoomName = string.Empty,
                                TemplateID = u.TemplateID,
                                UpdatedByName = string.Empty,
                                TotalRecords = u.TotalRecords,
                            }).AsParallel().ToList();


                TotalCount = 0;
                if (lstItems != null && lstItems.Count > 0)
                {
                    TotalCount = lstItems.First().TotalRecords;
                }
            }

            return lstItems;

        }
        
        /// <summary>
        /// Get Particullar Record from the CatalogReportTemplateMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public CatalogReportTemplateMasterDTO GetCatalogReportTemplateMasterByTemplateID(Int64 TemplateID)
        {
            CatalogReportTemplateMasterDTO objTemplateMaster = new CatalogReportTemplateMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TemplateID", TemplateID) };
                objTemplateMaster = (from u in context.Database.SqlQuery<CatalogReportTemplateMasterDTO>("EXEC GetCatalogReportTemplateMasterByTemplateID @TemplateID", params1)
                                     select new CatalogReportTemplateMasterDTO
                                     {
                                         ID = u.ID,
                                         TemplateName = u.TemplateName,
                                         LabelSize = u.LabelSize,
                                         NoOfLabelPerSheet = u.NoOfLabelPerSheet,
                                         NoOfColumns = u.NoOfColumns,
                                         PageWidth = u.PageWidth,
                                         PageHeight = u.PageHeight,
                                         LabelWidth = u.LabelWidth,
                                         LabelHeight = u.LabelHeight,
                                         PageMarginLeft = u.PageMarginLeft,
                                         PageMarginRight = u.PageMarginRight,
                                         PageMarginTop = u.PageMarginTop,
                                         PageMarginBottom = u.PageMarginBottom,
                                         LabelSpacingHorizontal = u.LabelSpacingHorizontal,
                                         LabelSpacingVerticle = u.LabelSpacingVerticle,
                                         LabelPaddingLeft = u.LabelPaddingLeft,
                                         LabelPaddingRight = u.LabelPaddingRight,
                                         LabelPaddingTop = u.LabelPaddingTop,
                                         LabelPaddingBottom = u.LabelPaddingBottom,
                                         LabelType = u.LabelType,
                                         TemplateNameWithSize = u.TemplateName + " - " + u.LabelSize,//.Replace("\"", "")
                                         CompanyID = u.CompanyID,
                                         CreatedByName = string.Empty,
                                         RoomName = string.Empty,
                                         TemplateID = u.TemplateID,
                                         UpdatedByName = string.Empty,
                                     }).FirstOrDefault();

            }
            return objTemplateMaster;
        }

        public CatalogReportTemplateMasterDTO GetCatalogReportTemplateMasterByCompanyTemplateID(Int64 TemplateID, long CompanyID)
        {
            CatalogReportTemplateMasterDTO objTemplateMaster = new CatalogReportTemplateMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TemplateID", TemplateID),
                                                   new SqlParameter("@CompanyID", CompanyID)};

                objTemplateMaster = (from u in context.Database.SqlQuery<CatalogReportTemplateMasterDTO>("EXEC GetCatalogReportTemplateMasterByCompanyTemplateID @TemplateID,@CompanyID", params1)
                                     select new CatalogReportTemplateMasterDTO
                                     {
                                         ID = u.ID,
                                         TemplateName = u.TemplateName,
                                         LabelSize = u.LabelSize,
                                         NoOfLabelPerSheet = u.NoOfLabelPerSheet,
                                         NoOfColumns = u.NoOfColumns,
                                         PageWidth = u.PageWidth,
                                         PageHeight = u.PageHeight,
                                         LabelWidth = u.LabelWidth,
                                         LabelHeight = u.LabelHeight,
                                         PageMarginLeft = u.PageMarginLeft,
                                         PageMarginRight = u.PageMarginRight,
                                         PageMarginTop = u.PageMarginTop,
                                         PageMarginBottom = u.PageMarginBottom,
                                         LabelSpacingHorizontal = u.LabelSpacingHorizontal,
                                         LabelSpacingVerticle = u.LabelSpacingVerticle,
                                         LabelPaddingLeft = u.LabelPaddingLeft,
                                         LabelPaddingRight = u.LabelPaddingRight,
                                         LabelPaddingTop = u.LabelPaddingTop,
                                         LabelPaddingBottom = u.LabelPaddingBottom,
                                         LabelType = u.LabelType,
                                         TemplateNameWithSize = u.TemplateName + " - " + u.LabelSize,//.Replace("\"", "")
                                         CompanyID = u.CompanyID,
                                         CreatedByName = string.Empty,
                                         RoomName = string.Empty,
                                         TemplateID = u.TemplateID,
                                         UpdatedByName = string.Empty,
                                     }).FirstOrDefault();

            }
            return objTemplateMaster;
        }

        /// <summary>
        /// Insert Record in the DataBase CatalogReportTemplateMaster
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(CatalogReportTemplateMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CatalogReportTemplateMaster obj = new CatalogReportTemplateMaster();
                obj.ID = 0;
                obj.TemplateID = objDTO.TemplateID;
                obj.CompanyID = objDTO.CompanyID;
                obj.TemplateName = objDTO.TemplateName;
                obj.LabelSize = objDTO.LabelSize;
                obj.NoOfLabelPerSheet = objDTO.NoOfLabelPerSheet;
                obj.NoOfColumns = objDTO.NoOfColumns;
                obj.PageWidth = objDTO.PageWidth;
                obj.PageHeight = objDTO.PageHeight;
                obj.LabelWidth = objDTO.LabelWidth;
                obj.LabelHeight = objDTO.LabelHeight;
                obj.PageMarginLeft = objDTO.PageMarginLeft;
                obj.PageMarginRight = objDTO.PageMarginRight;
                obj.PageMarginTop = objDTO.PageMarginTop;
                obj.PageMarginBottom = objDTO.PageMarginBottom;
                obj.LabelSpacingHorizontal = objDTO.LabelSpacingHorizontal;
                obj.LabelSpacingVerticle = objDTO.LabelSpacingVerticle;
                obj.LabelPaddingLeft = objDTO.LabelPaddingLeft;
                obj.LabelPaddingRight = objDTO.LabelPaddingRight;
                obj.LabelPaddingTop = objDTO.LabelPaddingTop;
                obj.LabelPaddingBottom = objDTO.LabelPaddingBottom;
                obj.LabelType = objDTO.LabelType;
                context.CatalogReportTemplateMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                return obj.ID;
            }

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(CatalogReportTemplateMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CatalogReportTemplateMaster obj = context.CatalogReportTemplateMasters.FirstOrDefault(x => x.ID == objDTO.ID);
                obj.TemplateName = objDTO.TemplateName;
                obj.LabelSize = objDTO.LabelSize;
                obj.NoOfLabelPerSheet = objDTO.NoOfLabelPerSheet;
                obj.NoOfColumns = objDTO.NoOfColumns;
                obj.PageWidth = objDTO.PageWidth;
                obj.PageHeight = objDTO.PageHeight;
                obj.LabelWidth = objDTO.LabelWidth;
                obj.LabelHeight = objDTO.LabelHeight;
                obj.PageMarginLeft = objDTO.PageMarginLeft;
                obj.PageMarginRight = objDTO.PageMarginRight;
                obj.PageMarginTop = objDTO.PageMarginTop;
                obj.PageMarginBottom = objDTO.PageMarginBottom;
                obj.LabelSpacingHorizontal = objDTO.LabelSpacingHorizontal;
                obj.LabelSpacingVerticle = objDTO.LabelSpacingVerticle;
                obj.LabelPaddingLeft = objDTO.LabelPaddingLeft;
                obj.LabelPaddingRight = objDTO.LabelPaddingRight;
                obj.LabelPaddingTop = objDTO.LabelPaddingTop;
                obj.LabelPaddingBottom = objDTO.LabelPaddingBottom;
                obj.LabelType = objDTO.LabelType;
                obj.CompanyID = objDTO.CompanyID;
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
        public bool DeleteRecords(string IDs)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs) };
                context.Database.SqlQuery<int>("EXEC [DeleteCatalogReportTemplateMaster] @IDs", params1).FirstOrDefault();
                return true;
            }
        }


        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool EditInBaseDB(CatalogReportTemplateMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@NoOfColumns", objDTO.NoOfColumns),  
                                                   new SqlParameter("@NoOfLabelPerSheet", objDTO.NoOfLabelPerSheet),
                                                   new SqlParameter("@LabelWidth", objDTO.LabelWidth), 
                                                   new SqlParameter("@LabelHeight", objDTO.LabelHeight),
                                                   new SqlParameter("@PageMarginLeft", objDTO.PageMarginLeft),
                                                   new SqlParameter("@PageMarginRight", objDTO.PageMarginRight),
                                                   new SqlParameter("@PageMarginTop", objDTO.PageMarginTop),
                                                   new SqlParameter("@PageMarginBottom", objDTO.PageMarginBottom),
                                                   new SqlParameter("@LabelSpacingHorizontal", objDTO.LabelSpacingHorizontal),
                                                   new SqlParameter("@LabelSpacingVerticle", objDTO.LabelSpacingVerticle),
                                                   new SqlParameter("@LabelPaddingLeft", objDTO.LabelPaddingLeft),
                                                   new SqlParameter("@LabelPaddingRight", objDTO.LabelPaddingRight),
                                                   new SqlParameter("@LabelPaddingTop", objDTO.LabelPaddingTop),
                                                   new SqlParameter("@LabelPaddingBottom", objDTO.LabelPaddingBottom),
                                                   new SqlParameter("@PageHeight", objDTO.PageHeight),
                                                   new SqlParameter("@PageWidth", objDTO.PageWidth),
                                                   new SqlParameter("@ID", objDTO.ID)
                                                 };
                context.Database.SqlQuery<int>("EXEC [UpdateCatalogReportTemplateMasterInBaseDB] @NoOfColumns,@NoOfLabelPerSheet,@LabelWidth,@LabelHeight,@PageMarginLeft,@PageMarginRight,@PageMarginTop,@PageMarginBottom,@LabelSpacingHorizontal,@LabelSpacingVerticle,@LabelPaddingLeft,@LabelPaddingRight,@LabelPaddingTop,@LabelPaddingBottom,@PageHeight,@PageWidth,@ID", params1).FirstOrDefault();
                return true;
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool EditInCurrentDB(CatalogReportTemplateMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@NoOfColumns", objDTO.NoOfColumns),  
                                                   new SqlParameter("@NoOfLabelPerSheet", objDTO.NoOfLabelPerSheet),
                                                   new SqlParameter("@LabelWidth", objDTO.LabelWidth), 
                                                   new SqlParameter("@LabelHeight", objDTO.LabelHeight),
                                                   new SqlParameter("@PageMarginLeft", objDTO.PageMarginLeft),
                                                   new SqlParameter("@PageMarginRight", objDTO.PageMarginRight),
                                                   new SqlParameter("@PageMarginTop", objDTO.PageMarginTop),
                                                   new SqlParameter("@PageMarginBottom", objDTO.PageMarginBottom),
                                                   new SqlParameter("@LabelSpacingHorizontal", objDTO.LabelSpacingHorizontal),
                                                   new SqlParameter("@LabelSpacingVerticle", objDTO.LabelSpacingVerticle),
                                                   new SqlParameter("@LabelPaddingLeft", objDTO.LabelPaddingLeft),
                                                   new SqlParameter("@LabelPaddingRight", objDTO.LabelPaddingRight),
                                                   new SqlParameter("@LabelPaddingTop", objDTO.LabelPaddingTop),
                                                   new SqlParameter("@LabelPaddingBottom", objDTO.LabelPaddingBottom),
                                                   new SqlParameter("@PageHeight", objDTO.PageHeight),
                                                   new SqlParameter("@PageWidth", objDTO.PageWidth),
                                                   new SqlParameter("@ID", objDTO.ID)
                                                 };
                context.Database.SqlQuery<int>("EXEC [UpdateCatalogReportTemplateMaster] @NoOfColumns,@NoOfLabelPerSheet,@LabelWidth,@LabelHeight,@PageMarginLeft,@PageMarginRight,@PageMarginTop,@PageMarginBottom,@LabelSpacingHorizontal,@LabelSpacingVerticle,@LabelPaddingLeft,@LabelPaddingRight,@LabelPaddingTop,@LabelPaddingBottom,@PageHeight,@PageWidth,@ID", params1).FirstOrDefault();
                return true;
                 
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool EditInAllEnterprise()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.SqlQuery<int>("Exec [UpdateCatalogReportTemplateMasterInAllEnterprise]").FirstOrDefault();
                return true;
            }
        }
        public Int32 GetCatalogBaseTemplateCountByCompanyID(Int64 CompanyID)
        {
            int rowAffected = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
                rowAffected = context.Database.SqlQuery<Int32>("EXEC [GetCatalogBaseTemplateCountByCompanyID] @CompanyID", params1).FirstOrDefault();
            }
            return rowAffected;
        }

    }
}
