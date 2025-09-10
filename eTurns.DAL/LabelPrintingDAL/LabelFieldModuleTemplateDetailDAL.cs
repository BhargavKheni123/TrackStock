using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.LabelPrinting;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace eTurns.DAL.LabelPrintingDAL
{
    public class LabelFieldModuleTemplateDetailDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public LabelFieldModuleTemplateDetailDAL(base.DataBaseName)
        //{

        //}

        public LabelFieldModuleTemplateDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public LabelFieldModuleTemplateDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        //        private IEnumerable<LabelFieldModuleTemplateDetailDTO> GetCachedData()
        //        {
        //            //Get Cached-Media
        //            IEnumerable<LabelFieldModuleTemplateDetailDTO> ObjCache = null;// CacheHelper<IEnumerable<LabelFieldModuleTemplateDetailDTO>>.GetCacheItem("Cached_LabelFieldModuleTemplateDetail_" + CompanyID.ToString());
        //            if (ObjCache == null || ObjCache.Count() <= 0)
        //            {
        //                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //                {
        //                    IEnumerable<LabelFieldModuleTemplateDetailDTO> obj = (from u in context.Database.SqlQuery<LabelFieldModuleTemplateDetailDTO>(@"SELECT  A.ID	,A.Name  ,A.TemplateID  ,T.TemplateName  ,A.ModuleID	,MM.ModuleName 
        //		                                                                                                                                                    ,A.FeildIDs  		
        //                                                                                                                                                            ,(SELECT STUFF((SELECT ', ' + t1.FieldName FROM LabelModuleFieldMaster t1 WHERE t1.ID  in (SELECT SplitValue FROM [dbo].[Split] ( A.FeildIDs  ,',')) FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)') ,1,2,'')) SelectedFieldsName
        //		                                                                                                                                                    ,A.IncludeBin	 	,A.IncludeQuantity	 ,A.QuantityField
        //		                                                                                                                                                    ,A.BarcodeKey		,MFM.FieldDisplayName as 'BarcodeKeyName'
        //		                                                                                                                                                    ,A.LabelHTML		,A.LabelXML	 ,A.FontSize ,A.CompanyID		
        //		                                                                                                                                                    ,A.CreatedBy		, B.UserName AS 'CreatedByName'
        //		                                                                                                                                                    ,A.UpdatedBy		,C.UserName AS 'UpdatedByName'		
        //		                                                                                                                                                    ,A.CreatedOn		,A.UpdatedOn ,A.IsArchived		,A.IsDeleted
        //    		                                                                                                                                                ,A.TextFont         ,A.BarcodeFont
        //                                                                                                                                                            --,Convert(Bit,Case When ISNULL(MTD.ID,0) >0 Then 1 else 0 end) AS IsSelectedInModuleConfig
        //                                                                                                                                                            ,Convert(Bit,Case When (SELECT Count(MTD1.ID) FROM LabelModuleTemplateDetail MTD1 WHERE A.ID=MTD1.TemplateDetailID and A.ModuleID = MTD1.ModuleID  AND (A.CompanyID = -1 OR MTD1.CompanyID = A.CompanyID) ) > 0 Then 1 else 0 end) AS IsSelectedInModuleConfig
        //                                                                                                                                                    FROM LabelFieldModuleTemplateDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
        //									                                                                                                                                                      LEFT OUTER JOIN UserMaster C on A.UpdatedBy = C.ID  
        //                                                                                                                                                                                          LEFT OUTER JOIN LabelTemplateMaster T on A.TemplateID = T.ID  
        //                                                                                                                                                                                          LEFT OUTER JOIN LabelModuleMaster MM On A.ModuleID = MM.ID 
        //									                                                                                                                                                      LEFT OUTER JOIN LabelModuleFieldMaster MFM on A.BarcodeKey = MFM.ID
        //                                                                                                                                                                                          --LEFT OUTER JOIN LabelModuleTemplateDetail MTD on A.ID = MTD.TemplateDetailID  AND A.ModuleID = MTD.ModuleID AND (A.CompanyID = MTD.CompanyID OR A.CompanyID =-1)")
        //                                                                          select new LabelFieldModuleTemplateDetailDTO
        //                                                                          {
        //                                                                              ID = u.ID,
        //                                                                              Name = u.Name,
        //                                                                              TemplateID = u.TemplateID,
        //                                                                              ModuleID = u.ModuleID,
        //                                                                              FeildIDs = u.FeildIDs,
        //                                                                              IncludeBin = u.IncludeBin,
        //                                                                              IncludeQuantity = u.IncludeQuantity,
        //                                                                              QuantityField = u.QuantityField,
        //                                                                              BarcodeKey = u.BarcodeKey,
        //                                                                              LabelHTML = u.LabelHTML,
        //                                                                              LabelXML = u.LabelXML,
        //                                                                              FontSize = u.FontSize,
        //                                                                              CompanyID = u.CompanyID,
        //                                                                              CreatedBy = u.CreatedBy,
        //                                                                              UpdatedBy = u.UpdatedBy,
        //                                                                              CreatedOn = u.CreatedOn,
        //                                                                              UpdatedOn = u.UpdatedOn,
        //                                                                              IsArchived = u.IsArchived,
        //                                                                              IsDeleted = u.IsDeleted,
        //                                                                              CreatedByName = u.CreatedByName,
        //                                                                              UpdatedByName = u.UpdatedByName,
        //                                                                              ModuleName = u.ModuleName,
        //                                                                              TemplateName = u.TemplateName,
        //                                                                              BarcodeKeyName = u.BarcodeKeyName,
        //                                                                              SelectedFieldsName = u.SelectedFieldsName,
        //                                                                              BarcodeFont = u.BarcodeFont,
        //                                                                              TextFont = u.TextFont,
        //                                                                              IsSelectedInModuleConfig = u.IsSelectedInModuleConfig,
        //                                                                          }).AsParallel().ToList();
        //                    ObjCache = obj.Where(x => x.CompanyID != 0);// CacheHelper<IEnumerable<LabelFieldModuleTemplateDetailDTO>>.AddCacheItem("Cached_LabelFieldModuleTemplateDetail_" + CompanyID.ToString(), obj);
        //                }
        //            }

        //            return ObjCache;
        //        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        

        /// <summary>
        /// Get Paged Records from the LabelFieldModuleTemplateDetail Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        //public IEnumerable<LabelFieldModuleTemplateDetailDTO> GetAllRecords()
        //{
        //    return GetCachedData().OrderBy("ID DESC");
        //}

        /// <summary>
        /// Get Paged Records from the LabelFieldModuleTemplateDetail Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        
       
        public IEnumerable<LabelFieldModuleTemplateDetailDTO> GetAllLabelFieldModuleTemplateDetail(Int64 CompanyId, Int64 RoomID, Int64? ModuleID, bool? IsDeleted, bool? IsArchived, string StartWith, bool IncludeEnterpriseLabels = true)
        {
            List<LabelFieldModuleTemplateDetailDTO> lstLblFldModuleTemplateDtl = new List<LabelFieldModuleTemplateDetailDTO>();
            
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyId),
                                               new SqlParameter("@ModuleID", ModuleID ?? (object)DBNull.Value),
                                               new SqlParameter("@IsDeleted", IsDeleted ?? (object)DBNull.Value),
                                               new SqlParameter("@IsArchived", IsArchived ?? (object)DBNull.Value),
                                               new SqlParameter("@StartWith", StartWith),
                                               new SqlParameter("@IncludeEnterpriseLabels", IncludeEnterpriseLabels)
                                             };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstLblFldModuleTemplateDtl = (from u in context.Database.SqlQuery<LabelFieldModuleTemplateDetailDTO>("EXEC dbo.GetAllLabelFieldModuleTemplateDetail  @RoomID,@CompanyID,@ModuleID,@IsDeleted,@IsArchived,@StartWith,@IncludeEnterpriseLabels", params1)
                                              select new LabelFieldModuleTemplateDetailDTO
                                              {
                                                  ID = u.ID,
                                                  Name = u.Name,
                                                  TemplateID = u.TemplateID,
                                                  ModuleID = u.ModuleID,
                                                  FeildIDs = u.FeildIDs,
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
                                                  ModuleName = u.ModuleName,
                                                  TemplateName = u.TemplateName,
                                                  BarcodeKeyName = u.BarcodeKeyName,
                                                  SelectedFieldsName = u.SelectedFieldsName,
                                                  BarcodeFont = u.BarcodeFont,
                                                  TextFont = u.TextFont,
                                                  IsSelectedInModuleConfig = u.IsSelectedInModuleConfig,
                                                  BarcodePattern = u.BarcodePattern,
                                                  arrFieldIds = u.arrFieldIds,
                                                  BaseLabelTemplateName = u.BaseLabelTemplateName,
                                                  CreatedDate = u.CreatedDate,
                                                  IsBaseLabelEdit = u.IsBaseLabelEdit,
                                                  IsSaveForEnterprise = u.IsSaveForEnterprise,
                                                  lstBarcodeKey = u.lstBarcodeKey,
                                                  lstModuleFields = u.lstModuleFields,
                                                  lstQuantityFields = u.lstQuantityFields,
                                                  lstSelectedModuleFields = u.lstSelectedModuleFields,
                                                  UpdatedDate = u.UpdatedDate,
                                                  
                                              }).AsParallel().ToList();
                
            }

            return lstLblFldModuleTemplateDtl;

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
        public IEnumerable<LabelFieldModuleTemplateDetailDTO> GetPagedRecordsDB(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, string Roomformat, TimeZoneInfo CurrentTimeZone, bool IncludeEnterpriseLabels = true)
        {
            List<LabelFieldModuleTemplateDetailDTO> lstLblFldModuleTemplateDtl = new List<LabelFieldModuleTemplateDetailDTO>();
            TotalCount = 0;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string Creators = null;
            string Updators = null;
            string ModuleIDs = null;

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[^]"))
            {
                int idx = SearchTerm.IndexOf("[^]");
                SearchTerm = SearchTerm.Substring(0, idx);
            }

            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                string[] FieldsPara = Fields[1].Split('~');
                string newSearchValue = string.Empty;

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
                if (FieldsPara != null && FieldsPara.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                    {
                        ModuleIDs = FieldsPara[0].TrimEnd(','); 
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                    {
                        Creators = FieldsPara[1].TrimEnd(','); 
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                    {
                        Updators = FieldsPara[2].TrimEnd(',');
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                    {
                        CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], Roomformat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                        CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], Roomformat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                    {
                        UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[4].Split(',')[0], Roomformat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                        UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[4].Split(',')[1], Roomformat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    }

                    //if (FieldsPara[5].Contains("11"))
                    //{
                    //    IEnumerable<LabelFieldModuleTemplateDetailDTO> ObjCacheBase = GetCachedData(CompanyID, RoomID).Where(x => x.IsDeleted == IsDeleted && x.IsArchived == IsArchived && x.CompanyID == -1);
                    //    foreach (var item in ObjCacheBase)
                    //    {
                    //        ObjCache.Add(item);
                    //    }
                    //}
                    //else if (FieldsPara[5].Contains("22"))
                    //{
                    //    ObjCache = GetCachedData(CompanyID, RoomID).Where(x => x.IsDeleted == IsDeleted && x.IsArchived == IsArchived && x.CompanyID == -1).ToList();
                    //}
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
                                               new SqlParameter("@Room", RoomID),
                                               new SqlParameter("@CompanyID", CompanyID),
                                               new SqlParameter("@IncludeEnterpriseLabels", IncludeEnterpriseLabels),
                                               new SqlParameter("@ModuleIDs", ModuleIDs ?? (object)DBNull.Value)
                                             };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                lstLblFldModuleTemplateDtl = (from u in context.Database.SqlQuery<LabelFieldModuleTemplateDetailDTO>("EXEC dbo.GetPagedLabelFieldModuleTemplateDetail @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@CreatedBy,@UpdatedFrom,@UpdatedTo,@LastUpdatedBy,@IsDeleted,@IsArchived,@Room,@CompanyID,@IncludeEnterpriseLabels,@ModuleIDs", params1)
                                                                      select new LabelFieldModuleTemplateDetailDTO
                                                                      {
                                                                          ID = u.ID,
                                                                          Name = u.Name,
                                                                          TemplateID = u.TemplateID,
                                                                          ModuleID = u.ModuleID,
                                                                          FeildIDs = u.FeildIDs,
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
                                                                          ModuleName = u.ModuleName,
                                                                          TemplateName = u.TemplateName,
                                                                          BarcodeKeyName = u.BarcodeKeyName,
                                                                          SelectedFieldsName = u.SelectedFieldsName,
                                                                          BarcodeFont = u.BarcodeFont,
                                                                          TextFont = u.TextFont,
                                                                          IsSelectedInModuleConfig = u.IsSelectedInModuleConfig,
                                                                          BarcodePattern = u.BarcodePattern,
                                                                          arrFieldIds = u.arrFieldIds,
                                                                          BaseLabelTemplateName = u.BaseLabelTemplateName,
                                                                          CreatedDate = u.CreatedDate,
                                                                          IsBaseLabelEdit = u.IsBaseLabelEdit,
                                                                          IsSaveForEnterprise = u.IsSaveForEnterprise,
                                                                          lstBarcodeKey = u.lstBarcodeKey,
                                                                          lstModuleFields = u.lstModuleFields,
                                                                          lstQuantityFields = u.lstQuantityFields,
                                                                          lstSelectedModuleFields = u.lstSelectedModuleFields,
                                                                          UpdatedDate = u.UpdatedDate,
                                                                          TotalRecords = u.TotalRecords,
                                                                      }).AsParallel().ToList();
                
            }

            TotalCount = 0;
            if (lstLblFldModuleTemplateDtl != null && lstLblFldModuleTemplateDtl.Count > 0)
            {
                TotalCount = lstLblFldModuleTemplateDtl.First().TotalRecords;
            }
            return lstLblFldModuleTemplateDtl;

        }

        /// <summary>
        /// Get Particullar Record from the LabelFieldModuleTemplateDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        
        public LabelFieldModuleTemplateDetailDTO GetLabelFieldModuleTemplateDetailByID(Int64 id, Int64 CompanyID, Int64 RoomID)
        {
            LabelFieldModuleTemplateDetailDTO objLblFldModuleTemplateDtl = new LabelFieldModuleTemplateDetailDTO();

            var params1 = new SqlParameter[] { new SqlParameter("@ID", id),
                                               new SqlParameter("@CompanyID", CompanyID),
                                               new SqlParameter("@RoomID", RoomID)
                                             };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objLblFldModuleTemplateDtl = (from u in context.Database.SqlQuery<LabelFieldModuleTemplateDetailDTO>("EXEC dbo.GetLabelFieldModuleTemplateDetailByID  @ID,@CompanyID,@RoomID", params1)
                                              select new LabelFieldModuleTemplateDetailDTO
                                              {
                                                  ID = u.ID,
                                                  Name = u.Name,
                                                  TemplateID = u.TemplateID,
                                                  ModuleID = u.ModuleID,
                                                  FeildIDs = u.FeildIDs,
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
                                                  ModuleName = u.ModuleName,
                                                  TemplateName = u.TemplateName,
                                                  BarcodeKeyName = u.BarcodeKeyName,
                                                  SelectedFieldsName = u.SelectedFieldsName,
                                                  BarcodeFont = u.BarcodeFont,
                                                  TextFont = u.TextFont,
                                                  IsSelectedInModuleConfig = u.IsSelectedInModuleConfig,
                                                  BarcodePattern = u.BarcodePattern,
                                                  arrFieldIds = u.arrFieldIds,
                                                  BaseLabelTemplateName = u.BaseLabelTemplateName,
                                                  CreatedDate = u.CreatedDate,
                                                  IsBaseLabelEdit = u.IsBaseLabelEdit,
                                                  IsSaveForEnterprise = u.IsSaveForEnterprise,
                                                  lstBarcodeKey = u.lstBarcodeKey,
                                                  lstModuleFields = u.lstModuleFields,
                                                  lstQuantityFields = u.lstQuantityFields,
                                                  lstSelectedModuleFields = u.lstSelectedModuleFields,
                                                  UpdatedDate = u.UpdatedDate,
                                              }).FirstOrDefault();
            }
            return objLblFldModuleTemplateDtl;
        }

        public LabelFieldModuleTemplateDetailDTO GetLabelFieldModuleTemplateDetailByName(string Templatename, Int64 CompanyID, Int64 RoomID)
        {
            LabelFieldModuleTemplateDetailDTO objLblFldModuleTemplateDtl = new LabelFieldModuleTemplateDetailDTO();

            var params1 = new SqlParameter[] { new SqlParameter("@Templatename", Templatename),
                                               new SqlParameter("@CompanyID", CompanyID),
                                               new SqlParameter("@RoomID", RoomID)
                                             };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objLblFldModuleTemplateDtl = (from u in context.Database.SqlQuery<LabelFieldModuleTemplateDetailDTO>("EXEC dbo.GetLabelFieldModuleTemplateDetailByName @Templatename,@CompanyID,@RoomID", params1)
                                              select new LabelFieldModuleTemplateDetailDTO
                                              {
                                                  ID = u.ID,
                                                  Name = u.Name,
                                                  TemplateID = u.TemplateID,
                                                  ModuleID = u.ModuleID,
                                                  FeildIDs = u.FeildIDs,
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
                                                  ModuleName = u.ModuleName,
                                                  TemplateName = u.TemplateName,
                                                  BarcodeKeyName = u.BarcodeKeyName,
                                                  SelectedFieldsName = u.SelectedFieldsName,
                                                  BarcodeFont = u.BarcodeFont,
                                                  TextFont = u.TextFont,
                                                  IsSelectedInModuleConfig = u.IsSelectedInModuleConfig,
                                                  BarcodePattern = u.BarcodePattern,
                                                  arrFieldIds = u.arrFieldIds,
                                                  BaseLabelTemplateName = u.BaseLabelTemplateName,
                                                  CreatedDate = u.CreatedDate,
                                                  IsBaseLabelEdit = u.IsBaseLabelEdit,
                                                  IsSaveForEnterprise = u.IsSaveForEnterprise,
                                                  lstBarcodeKey = u.lstBarcodeKey,
                                                  lstModuleFields = u.lstModuleFields,
                                                  lstQuantityFields = u.lstQuantityFields,
                                                  lstSelectedModuleFields = u.lstSelectedModuleFields,
                                                  UpdatedDate = u.UpdatedDate,
                                              }).FirstOrDefault();
            }
            return objLblFldModuleTemplateDtl;
        }

        /// <summary>
        /// Insert Record in the DataBase LabelFieldModuleTemplateDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(LabelFieldModuleTemplateDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                LabelFieldModuleTemplateDetail obj = new LabelFieldModuleTemplateDetail();
                if (string.IsNullOrEmpty(objDTO.FeildIDs))
                    objDTO.FeildIDs = "";

                obj.ID = 0;
                obj.Name = objDTO.Name;
                obj.TemplateID = objDTO.TemplateID;
                obj.ModuleID = objDTO.ModuleID;
                obj.FeildIDs = objDTO.FeildIDs;
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
                obj.CreatedOn = DateTimeUtility.DateTimeNow;
                obj.UpdatedOn = DateTimeUtility.DateTimeNow;
                obj.IsArchived = false;
                obj.IsDeleted = false;
                obj.BarcodeFont = objDTO.BarcodeFont;
                obj.TextFont = objDTO.TextFont;
                obj.BarcodePattern = objDTO.BarcodePattern;

                context.LabelFieldModuleTemplateDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;


                if (objDTO.ID > 0)
                {
                    LabelModuleTemplateDetailDAL objLMTDDAL = new LabelModuleTemplateDetailDAL(base.DataBaseName);
                    if (objDTO.CompanyID == -1)
                    {
                        /*Int64[] objExistList = (from x in objLMTDDAL.GetCachedData()
                                                where x.ModuleID == objDTO.ModuleID
                                                select x.CompanyID).ToArray();*/

                        Int64[] objExistList = (from x in objLMTDDAL.GetLabelModuleTemplateDetailByModuleID(objDTO.ModuleID)
                                                select x.CompanyID).ToArray();

                        /*Int64?[] objExistRoomList = (from x in objLMTDDAL.GetCachedData()
                                                     where x.ModuleID == objDTO.ModuleID
                                                     select x.RoomID).ToArray();*/

                        Int64?[] objExistRoomList = (from x in objLMTDDAL.GetLabelModuleTemplateDetailByModuleID(objDTO.ModuleID)
                                                     select x.RoomID).ToArray();

                        //IEnumerable<CompanyMasterDTO> lstCompanyDTO = new CompanyMasterDAL(base.DataBaseName).GetAllRecords().Where(x => objExistList.Contains(x.ID) == false);
                        IEnumerable<CompanyMasterDTO> lstCompanyDTO = new CompanyMasterDAL(base.DataBaseName).GetCompaniesByIds(string.Join(",", objExistList));
                        string strCompanyIDs = string.Join(",", lstCompanyDTO.Select(x => x.ID).ToArray());
                        List<RoomDTO> lstRoomDTO = new RoomDAL(base.DataBaseName).GetRoomByCompanyIDsPlain(strCompanyIDs).Where(x => objExistRoomList.Contains(x.ID) == false).ToList();

                        foreach (var item in lstCompanyDTO)
                        {
                            foreach (var itemroom in lstRoomDTO.Where(r => r.CompanyID == item.ID))
                            {
                                LabelModuleTemplateDetailDTO objLMTDDTO = new LabelModuleTemplateDetailDTO()
                                {
                                    CompanyID = item.ID,
                                    CreatedBy = objDTO.CreatedBy,
                                    UpdatedBy = objDTO.UpdatedBy,
                                    CreatedOn = DateTime.Now,
                                    UpdatedOn = DateTime.Now,
                                    ModuleID = objDTO.ModuleID,
                                    TemplateDetailID = objDTO.ID,
                                    RoomID = itemroom.ID
                                };
                                objLMTDDAL.Insert(objLMTDDTO);
                            }

                        }
                    }

                }

                return obj.ID;
            }


        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(LabelFieldModuleTemplateDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                LabelFieldModuleTemplateDetail obj = new LabelFieldModuleTemplateDetail();
                if (string.IsNullOrEmpty(objDTO.FeildIDs))
                    objDTO.FeildIDs = "";

                obj.ID = objDTO.ID;
                obj.Name = objDTO.Name;
                obj.TemplateID = objDTO.TemplateID;
                obj.ModuleID = objDTO.ModuleID;
                obj.FeildIDs = objDTO.FeildIDs;
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

                context.LabelFieldModuleTemplateDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
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
        public string DeleteSelectedRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID,string NoRecordSelected,string Ok, string DefaultTemplateCantBeDelete)
        {
            if (string.IsNullOrEmpty(IDs) || IDs.Trim().Length <= 0)
            {
                return NoRecordSelected;
            }
            string returnMsg = "";
            string setDefaultQuery = string.Empty;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //string strQuery = "";
                Int64[] intIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();
                foreach (var item in intIDs)
                {
                    if (item > 0)
                    {
                        LabelModuleTemplateDetail objMTD = context.LabelModuleTemplateDetails.FirstOrDefault(x => x.CompanyID == CompanyID && x.RoomID == RoomID && x.TemplateDetailID == item);
                        Int64 templateID = 0;
                        if (objMTD != null && objMTD.ID > 0)
                            templateID = GetDefaultTemplateIDByModule(objMTD.ModuleID, CompanyID, RoomID);

                        if (item != templateID) 
                        {
                            Int64 LblModTempDtlID = 0; Int64 ModuleID = 0;
                            if (objMTD != null)
                            {
                                if(objMTD.ID > 0)
                                    LblModTempDtlID = objMTD.ID;
                                if (objMTD.ModuleID > 0)
                                    ModuleID = objMTD.ModuleID;
                            }

                            var params1 = new SqlParameter[] { new SqlParameter("@ID", item)
                                                              ,new SqlParameter("@UserID", userid)
                                                              ,new SqlParameter("@LblModTempDtlID", LblModTempDtlID)
                                                              ,new SqlParameter("@TemplateID", templateID)
                                                              ,new SqlParameter("@ModuleID", ModuleID)
                                                              ,new SqlParameter("@CompanyID", CompanyID)
                                                              ,new SqlParameter("@RoomID", RoomID)
                            };
                            context.Database.SqlQuery<int>("EXEC [DeleteLabelFieldModuleTemplateDetail] @ID,@UserID,@LblModTempDtlID,@TemplateID,@ModuleID,@CompanyID,@RoomID", params1).FirstOrDefault();
                        }
                        else
                        {
                            returnMsg = DefaultTemplateCantBeDelete;
                        }
                    }
                }

                if (string.IsNullOrEmpty(returnMsg))
                    returnMsg = Ok;

                return returnMsg;
            }
        }

        public Int64 GetDefaultTemplateIDByModule(Int64 ModuleID, Int64 CompanyID, Int64 RoomID)
        {
            //List<LabelFieldModuleTemplateDetailDTO> lstFMTDetails = GetAllRecords(CompanyID, RoomID).Where(x => x.ModuleID == ModuleID && x.IsDeleted == false && x.IsArchived == false && x.Name.StartsWith("Default ")).OrderBy(x => x.Name).ToList();
            List<LabelFieldModuleTemplateDetailDTO> lstFMTDetails = GetAllLabelFieldModuleTemplateDetail(CompanyID,RoomID,ModuleID,false,false, "Default ").OrderBy(x => x.Name).ToList();
            Int64 templateID = 0;
            if (ModuleID == (int)LabelModule.Inventory)
            {
                templateID = lstFMTDetails.Where(x => x.Name == "Default Inventory Label").Select(x => x.ID).FirstOrDefault();
            }
            else if (ModuleID == (int)LabelModule.Asset)
            {
                templateID = lstFMTDetails.Where(x => x.Name == "Default Asset Label").Select(x => x.ID).FirstOrDefault();
            }
            else if (ModuleID == (int)LabelModule.Kitting)
            {
                templateID = lstFMTDetails.Where(x => x.Name == "Default Kitting Label").Select(x => x.ID).FirstOrDefault();
            }
            else if (ModuleID == (int)LabelModule.Locations)
            {
                templateID = lstFMTDetails.Where(x => x.Name == "Default Location Label").Select(x => x.ID).FirstOrDefault();
            }
            else if (ModuleID == (int)LabelModule.Orders)
            {
                templateID = lstFMTDetails.Where(x => x.Name == "Default Orders Label").Select(x => x.ID).FirstOrDefault();
            }
            else if (ModuleID == (int)LabelModule.QuickLists)
            {
                templateID = lstFMTDetails.Where(x => x.Name == "Default QuickLists Label").Select(x => x.ID).FirstOrDefault();
            }
            else if (ModuleID == (int)LabelModule.Receipts)
            {
                templateID = lstFMTDetails.Where(x => x.Name == "Default Receipts Label").Select(x => x.ID).FirstOrDefault();
            }
            else if (ModuleID == (int)LabelModule.Staging)
            {
                templateID = lstFMTDetails.Where(x => x.Name == "Default Staging Label").Select(x => x.ID).FirstOrDefault();
            }
            else if (ModuleID == (int)LabelModule.Technician)
            {
                templateID = lstFMTDetails.Where(x => x.Name == "Default Technician Label").Select(x => x.ID).FirstOrDefault();
            }
            else if (ModuleID == (int)LabelModule.Tools)
            {
                templateID = lstFMTDetails.Where(x => x.Name == "Default Tools Label").Select(x => x.ID).FirstOrDefault();
            }
            else if (ModuleID == (int)LabelModule.Transfer)
            {
                templateID = lstFMTDetails.Where(x => x.Name == "Default Transfer Label").Select(x => x.ID).FirstOrDefault();
            }
            else if (ModuleID == (int)LabelModule.WorkOrder)
            {
                templateID = lstFMTDetails.Where(x => x.Name == "Default WorkOrder Label").Select(x => x.ID).FirstOrDefault();
            }

            return templateID;
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
        public string DuplicateCheck(string Name, Int64 ID, Int64 CompanyID, Int64 RoomID)
        {
            Name = Name.Replace("'", "''");
            LabelFieldModuleTemplateDetailDTO dto = null;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                //IEnumerable<LabelFieldModuleTemplateDetailDTO> lst = GetAllRecords(CompanyID, RoomID).Where(x => !x.IsDeleted && !x.IsArchived);
                IEnumerable<LabelFieldModuleTemplateDetailDTO> lst = GetAllLabelFieldModuleTemplateDetail(CompanyID, RoomID, null, false, false, string.Empty);
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

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LabelFieldModuleTemplateDetailDTO> GetBaseTemplateList()
        {
            List<LabelFieldModuleTemplateDetailDTO> lstlblFieldModTempDtl = new List<LabelFieldModuleTemplateDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                lstlblFieldModTempDtl = (from u in context.Database.SqlQuery<LabelFieldModuleTemplateDetailDTO>("EXEC dbo.GetBaseTemplateList ")
                                                select new LabelFieldModuleTemplateDetailDTO
                                                {
                                                    ID = u.ID,
                                                    Name = u.Name,
                                                    TemplateID = u.TemplateID,
                                                    ModuleID = u.ModuleID,
                                                    FeildIDs = u.FeildIDs,
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
                                                    ModuleName = u.ModuleName,
                                                    TemplateName = u.TemplateName,
                                                    BarcodeKeyName = u.BarcodeKeyName,
                                                    SelectedFieldsName = u.SelectedFieldsName,
                                                    BarcodeFont = u.BarcodeFont,
                                                    TextFont = u.TextFont,
                                                }).AsParallel().ToList();
                
            }

            return lstlblFieldModTempDtl;
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
            int rowAffected = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@UserID", UserID)
                                                 };
                rowAffected = context.Database.SqlQuery<int>("EXEC [CopyAllBaseTemplate] @CompanyID,@RoomID,@UserID", params1).FirstOrDefault();
            }
            return rowAffected;
        }

        public int CopyAllBaseTemplateRoomLevel(Int64 CompanyID, Int64 UserID, Int64 RoomID)
        {
            int rowAffected = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@UserID", UserID)
                                                 };
                rowAffected = context.Database.SqlQuery<int>("EXEC [CopyAllBaseTemplateRoomLevel] @CompanyID,@RoomID,@UserID", params1).FirstOrDefault();
            }

            return rowAffected;

        }
        public int SetAsDefaultTemplateForModule(Int64 TemplateDetailID, Int64 CompanyID, Int64 UserID, Int64 ModuleID, Int64 RoomID)
        {
            int rowAffects = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@UserID", UserID),
                                                   new SqlParameter("@TemplateDetailID", TemplateDetailID),
                                                   new SqlParameter("@ModuleID", ModuleID)
                                                 };

                rowAffects = context.Database.SqlQuery<int>("EXEC [SetAsDefaultTemplateForModule] @CompanyID,@RoomID,@UserID,@TemplateDetailID,@ModuleID", params1).FirstOrDefault();
            }
            return rowAffects;
        }

        public Int32 GetBaseTemplateCountByCompanyID(Int64 CompanyID)
        {
            int rowAffected = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID)};
                rowAffected = context.Database.SqlQuery<Int32>("EXEC [GetBaseTemplateCountByCompanyID] @CompanyID", params1).FirstOrDefault();
            }
            return rowAffected;
        }        
    }
}


