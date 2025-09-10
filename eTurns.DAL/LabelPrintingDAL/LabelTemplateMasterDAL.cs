using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO.LabelPrinting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace eTurns.DAL.LabelPrintingDAL
{
    public class LabelTemplateMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public LabelTemplateMasterDAL(base.DataBaseName)
        //{

        //}

        public LabelTemplateMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public LabelTemplateMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Get Paged Records from the LabelTemplateMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<LabelTemplateMasterDTO> GetLabelTemplateMasterByCompanyID(Int64? CompanyID)
        {
            List<LabelTemplateMasterDTO> lstLblTemplate = new List<LabelTemplateMasterDTO>();

            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstLblTemplate = (from u in context.Database.SqlQuery<LabelTemplateMasterDTO>(@"EXEC dbo.GetLabelTemplateMasterByCompanyID @CompanyID", params1)
                                  select new LabelTemplateMasterDTO
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
            return lstLblTemplate;

        }

        /// <summary>
        /// Get Paged Records from the LabelTemplateMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<LabelTemplateMasterDTO> GetPagedRecordsDB(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            List<LabelTemplateMasterDTO> lstLblTemplate = new List<LabelTemplateMasterDTO>();
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
                                               new SqlParameter("@CompanyID", CompanyID)
                                             };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                lstLblTemplate = (from u in context.Database.SqlQuery<LabelTemplateMasterDTO>(@"EXEC dbo.GetPagedLabelTemplateMaster @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@IsDeleted,@IsArchived,@CompanyID", params1)
                                  select new LabelTemplateMasterDTO
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
                if (lstLblTemplate != null && lstLblTemplate.Count > 0)
                {
                    TotalCount = lstLblTemplate.First().TotalRecords;
                }
            }

            return lstLblTemplate;

        }

        /// <summary>
        /// Get Particullar Record from the LabelTemplateMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public LabelTemplateMasterDTO GetLabelTemplateMasterByCompanyTemplateID(Int64 TemplateID, Int64? CompanyID)
        {
            LabelTemplateMasterDTO objLblTemplate = new LabelTemplateMasterDTO();
            var params1 = new SqlParameter[] { new SqlParameter("@TemplateID", TemplateID)
                                              ,new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objLblTemplate = (from u in context.Database.SqlQuery<LabelTemplateMasterDTO>(@"EXEC dbo.GetLabelTemplateMasterByCompanyTemplateID @TemplateID,@CompanyID", params1)
                                  select new LabelTemplateMasterDTO
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
            return objLblTemplate;

        }

        /// <summary>
        /// Get Particullar Record from the LabelTemplateMaster by TemplateName
        /// </summary>
        /// <param name="TemplateName">TemplateName</param>
        /// <returns></returns>
        public LabelTemplateMasterDTO GetLabelTemplateMasterByCompanyTemplateName(string TemplateName, Int64? CompanyID)
        {
            LabelTemplateMasterDTO objLblTemplate = new LabelTemplateMasterDTO();
            var params1 = new SqlParameter[] { new SqlParameter("@TemplateName", TemplateName)
                                              ,new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objLblTemplate = (from u in context.Database.SqlQuery<LabelTemplateMasterDTO>(@"EXEC dbo.GetLabelTemplateMasterByCompanyTemplateName @TemplateName,@CompanyID", params1)
                                  select new LabelTemplateMasterDTO
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
            return objLblTemplate;
        }

        /// <summary>
        /// Insert Record in the DataBase LabelTemplateMaster
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(LabelTemplateMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                LabelTemplateMaster obj = new LabelTemplateMaster();
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
                context.LabelTemplateMasters.Add(obj);
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
        public bool Edit(LabelTemplateMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                LabelTemplateMaster obj = context.LabelTemplateMasters.FirstOrDefault(x => x.ID == objDTO.ID);
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
                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.LabelTemplateMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
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
                context.Database.SqlQuery<int>("EXEC [DeleteLabelTemplateMaster] @IDs", params1).FirstOrDefault();
                return true;
            }
        }


        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool EditInBaseDB(LabelTemplateMasterDTO objDTO)
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

                context.Database.SqlQuery<int>(@"EXEC [UpdateLabelTemplateMasterInBaseDB] @NoOfColumns,@NoOfLabelPerSheet,@LabelWidth,@LabelHeight,@PageMarginLeft,@PageMarginRight,@PageMarginTop,@PageMarginBottom,@LabelSpacingHorizontal,@LabelSpacingVerticle,@LabelPaddingLeft,@LabelPaddingRight,@LabelPaddingTop,@LabelPaddingBottom,@PageHeight,@PageWidth,@ID", params1).FirstOrDefault();
                return true;
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool EditInCurrentDB(LabelTemplateMasterDTO objDTO)
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

                ////System.Data.Objects.ObjectResult<object> obj = context.Database.SqlQuery<object>(strUpdate);
                context.Database.SqlQuery<object>("EXEC [UpdateLabelTemplateMaster] @NoOfColumns,@NoOfLabelPerSheet,@LabelWidth,@LabelHeight,@PageMarginLeft,@PageMarginRight,@PageMarginTop,@PageMarginBottom,@LabelSpacingHorizontal,@LabelSpacingVerticle,@LabelPaddingLeft,@LabelPaddingRight,@LabelPaddingTop,@LabelPaddingBottom,@PageHeight,@PageWidth,@ID", params1).FirstOrDefault();
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
                context.Database.SqlQuery<int>("Exec [UpdateLabelTemplateMasterInAllEnterprise]").FirstOrDefault();
                return true;
            }
        }


    }
}


