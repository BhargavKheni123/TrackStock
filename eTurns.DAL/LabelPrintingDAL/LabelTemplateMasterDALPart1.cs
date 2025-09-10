using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using eTurns.DTO.LabelPrinting;

namespace eTurns.DAL.LabelPrintingDAL
{
    public class LabelTemplateMasterDAL : eTurnsBaseDAL
    {
        private IEnumerable<LabelTemplateMasterDTO> GetCachedData()
        {
            //Get Cached-Media
            IEnumerable<LabelTemplateMasterDTO> ObjCache = null;// CacheHelper<IEnumerable<LabelTemplateMasterDTO>>.GetCacheItem("Cached_LabelTemplateMaster");
            if (ObjCache == null || ObjCache.Count() < 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<LabelTemplateMasterDTO> obj = (from u in context.ExecuteStoreQuery<LabelTemplateMasterDTO>(@" SELECT A.* FROM LabelTemplateMaster A ")
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
                    ObjCache = CacheHelper<IEnumerable<LabelTemplateMasterDTO>>.AddCacheItem("Cached_LabelTemplateMaster", obj);
                }
            }

            return ObjCache;
        }

        private IEnumerable<LabelTemplateMasterDTO> GetBaseCachedData()
        {


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<LabelTemplateMasterDTO> obj = (from u in context.ExecuteStoreQuery<LabelTemplateMasterDTO>(@" SELECT A.* FROM [" + DbConnectionHelper.GeteTurnsDBName() + "].[dbo].[LabelTemplateMaster] A ")
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
                return obj;
            }
        }

        public IEnumerable<LabelTemplateMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<LabelTemplateMasterDTO> ObjCache = GetAllRecords(CompanyID);


            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                if (SearchTerm.Contains("[###]"))
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
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.TemplateName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.LabelSize ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();

                return ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.TemplateName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.LabelSize ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
        }

        public IEnumerable<LabelTemplateMasterDTO> GetAllRecords(Int64? CompanyID)
        {
            //return GetCachedData().Where(x => x.CompanyID == CompanyID).OrderBy("ID asc");
            return GetCachedData().Where(x => x.CompanyID == CompanyID).OrderBy("LabelType asc,TemplateID asc");
        }

        public LabelTemplateMasterDTO GetRecord(Int64 TemplateID, Int64? CompanyID)
        {
            return GetCachedData().FirstOrDefault(t => t.TemplateID == TemplateID && t.CompanyID == CompanyID);
        }

        public LabelTemplateMasterDTO GetRecord(string TemplateName, Int64? CompanyID)
        {
            return GetCachedData().Single(t => t.TemplateName == TemplateName && t.CompanyID == CompanyID);
        }

        public bool EditInBaseDB(LabelTemplateMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strUpdate = @" Update [TemplateDBName].[dbo].[LabelTemplateMaster] SET 
                                        NoOfColumns		= " + objDTO.NoOfColumns + @",
                                        NoOfLabelPerSheet	= " + objDTO.NoOfLabelPerSheet + @",
                                        LabelWidth = " + objDTO.LabelWidth + @",
                                        LabelHeight = " + objDTO.LabelHeight + @",
                                        PageMarginLeft = " + objDTO.PageMarginLeft + @",
                                        PageMarginRight = " + objDTO.PageMarginRight + @",
                                        PageMarginTop = " + objDTO.PageMarginTop + @",
                                        PageMarginBottom = " + objDTO.PageMarginBottom + @",
                                        LabelSpacingHorizontal = " + objDTO.LabelSpacingHorizontal + @",
                                        LabelSpacingVerticle = " + objDTO.LabelSpacingVerticle + @",
                                        LabelPaddingLeft = " + objDTO.LabelPaddingLeft + @",
                                        LabelPaddingRight = " + objDTO.LabelPaddingRight + @",
                                        LabelPaddingTop = " + objDTO.LabelPaddingTop + @",
                                        LabelPaddingBottom = " + objDTO.LabelPaddingBottom + @",
                                        PageHeight = " + objDTO.PageHeight + @",
                                        PageWidth = " + objDTO.PageWidth + @"
				                        WHERE ID = " + objDTO.ID + ";";
                strUpdate = strUpdate.Replace("TemplateDBName", DbConnectionHelper.GeteTurnsDBName());
                System.Data.Objects.ObjectResult<object> obj = context.ExecuteStoreQuery<object>(strUpdate);

                CacheHelper<IEnumerable<LabelTemplateMasterDTO>>.AddCacheItem("Cached_LabelTemplateMaster", new List<LabelTemplateMasterDTO>());

                return true;
            }
        }

        public bool EditInCurrentDB(LabelTemplateMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strUpdate = @" Update [dbo].[LabelTemplateMaster] SET 
                                        NoOfColumns		= " + objDTO.NoOfColumns + @",
                                        NoOfLabelPerSheet	= " + objDTO.NoOfLabelPerSheet + @",
                                        LabelWidth = " + objDTO.LabelWidth + @",
                                        LabelHeight = " + objDTO.LabelHeight + @",
                                        PageMarginLeft = " + objDTO.PageMarginLeft + @",
                                        PageMarginRight = " + objDTO.PageMarginRight + @",
                                        PageMarginTop = " + objDTO.PageMarginTop + @",
                                        PageMarginBottom = " + objDTO.PageMarginBottom + @",
                                        LabelSpacingHorizontal = " + objDTO.LabelSpacingHorizontal + @",
                                        LabelSpacingVerticle = " + objDTO.LabelSpacingVerticle + @",
                                        LabelPaddingLeft = " + objDTO.LabelPaddingLeft + @",
                                        LabelPaddingRight = " + objDTO.LabelPaddingRight + @",
                                        LabelPaddingTop = " + objDTO.LabelPaddingTop + @",
                                        LabelPaddingBottom = " + objDTO.LabelPaddingBottom + @",
                                        PageHeight = " + objDTO.PageHeight + @",
                                        PageWidth = " + objDTO.PageWidth + @"
				                        WHERE ID = " + objDTO.ID + ";";

                System.Data.Objects.ObjectResult<object> obj = context.ExecuteStoreQuery<object>(strUpdate);

                CacheHelper<IEnumerable<LabelTemplateMasterDTO>>.AddCacheItem("Cached_LabelTemplateMaster", new List<LabelTemplateMasterDTO>());

                return true;
            }
        }

        public bool EditInAllEnterprise()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string strGet = @"SELECT EnterpriseDBName FROM [" + DbConnectionHelper.GetETurnsMasterDBName() + "].[dbo].[EnterpriseMaster]";

                List<string> strEnterPrises = (from u in context.ExecuteStoreQuery<string>(strGet)
                                               select u).ToList<string>();

                if (strEnterPrises != null && strEnterPrises.Count() > 0)
                {
                    foreach (var item in strEnterPrises)
                    {
                        try
                        {
                            string strUpdate = @" UPDATE u SET 
                                       u.NoOfColumns		=  s.NoOfColumns
                                      ,u.NoOfLabelPerSheet	=  s.NoOfLabelPerSheet	
                                      ,u.LabelWidth         =  s.LabelWidth			  	
                                      ,u.LabelHeight		=  s.LabelHeight			
                                      ,u.PageMarginLeft		=  s.PageMarginLeft		
                                      ,u.PageMarginRight	=  s.PageMarginRight		
                                      ,u.PageMarginTop		=  s.PageMarginTop			
                                      ,u.PageMarginBottom	=  s.PageMarginBottom		
                                      ,u.LabelSpacingHorizontal	=  s.LabelSpacingHorizontal
                                      ,u.LabelSpacingVerticle	=  s.LabelSpacingVerticle	
                                      ,u.LabelPaddingLeft	=  s.LabelPaddingLeft		
                                      ,u.LabelPaddingRight	=  s.LabelPaddingRight		
                                      ,u.LabelPaddingTop	=  s.LabelPaddingTop		
                                      ,u.LabelPaddingBottom	=  s.LabelPaddingBottom	
                                      ,u.PageHeight	=  s.PageHeight	
                                      ,u.PageWidth	=  s.PageWidth
	
                                      from [" + DbConnectionHelper.GeteTurnsDBName() + "].[dbo].[LabelTemplateMaster] s inner join [" + item + @"].[dbo].[LabelTemplateMaster] u on u.id = s.id;";

                            System.Data.Objects.ObjectResult<object> obj = context.ExecuteStoreQuery<object>(strUpdate);

                        }
                        catch (Exception)
                        {

                            // Log Execption Here
                        }


                    }
                }
                CacheHelper<IEnumerable<LabelTemplateMasterDTO>>.AddCacheItem("Cached_LabelTemplateMaster", new List<LabelTemplateMasterDTO>());

                return true;
            }
        }

    }
}
