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
        private IEnumerable<CatalogReportDetailDTO> GetCachedData(Int64 CompanyID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<CatalogReportDetailDTO> obj = (from u in context.ExecuteStoreQuery<CatalogReportDetailDTO>(@"SELECT  A.ID,A.Name,A.SelectedFields
                                                                ,A.IncludeBin	 	,A.IncludeQuantity	 ,A.QuantityField   ,A.BarcodeKey       ,A.LabelHTML		
                                                                ,A.LabelXML	        ,A.FontSize          ,A.CompanyID    ,A.CreatedBy		, B.UserName AS 'CreatedByName'
                                                                ,A.UpdatedBy		,C.UserName AS 'UpdatedByName'		     ,A.CreatedOn		,A.UpdatedOn 
                                                                ,A.IsArchived		,A.IsDeleted          ,A.TextFont         ,A.BarcodeFont ,A.BarcodePattern
                                                                ,A.TemplateID ,A.RoomID             ,CRTM.TemplateName
                                                                FROM [CatalogReportDetail] A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                LEFT OUTER JOIN UserMaster C on A.UpdatedBy = C.ID  
                                                                LEFT OUTER JOIN CatalogReportTemplateMaster CRTM on ((A.TemplateID = CRTM.TemplateID AND A.CompanyID = CRTM.CompanyID))
                                                                WHERE (A.CompanyID = " + CompanyID.ToString() + ") ")
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
                                                           }).AsParallel().ToList();
                return obj;

            }

        }

        public IEnumerable<CatalogReportDetailDTO> GetAllRecords(Int64 CompanyId)
        {
            IEnumerable<CatalogReportDetailDTO> ObjCache = GetCachedData(CompanyId).OrderBy("ID DESC");
            return ObjCache;
        }

        public IEnumerable<CatalogReportDetailDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 CompanyID, bool IsArchived, bool IsDeleted, string Roomformat, TimeZoneInfo CurrentTimeZone)
        {
            List<CatalogReportDetailDTO> ObjCache = GetCachedData(CompanyID).Where(x => x.IsDeleted == IsDeleted && x.IsArchived == IsArchived).ToList();

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[^]"))
            {
                int idx = SearchTerm.IndexOf("[^]");
                SearchTerm = SearchTerm.Substring(0, idx);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media

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


                //if (FieldsVal != null && FieldsVal.Length > 0)
                //{
                //    if (FieldsVal[5].Contains("11"))
                //    {
                //        IEnumerable<CatalogReportDetailDTO> ObjCacheBase = GetCachedData(CompanyID).Where(x => x.IsDeleted == IsDeleted && x.IsArchived == IsArchived && x.CompanyID == -1);
                //        foreach (var item in ObjCacheBase)
                //        {
                //            ObjCache.Add(item);
                //        }
                //    }
                //    else if (FieldsVal[5].Contains("22"))
                //    {
                //        ObjCache = GetCachedData(CompanyID).Where(x => x.IsDeleted == IsDeleted && x.IsArchived == IsArchived && x.CompanyID == -1).ToList();
                //    }

                //}

                ObjCache = ObjCache.Where(t =>
                        ((FieldsVal[0] == "") || (FieldsVal[0].Split(',').Select(Int64.Parse).ToArray().Contains(t.CreatedBy)))
                    && ((FieldsVal[1] == "") || (FieldsVal[1].Split(',').Select(Int64.Parse).ToArray().Contains(t.UpdatedBy)))
                    && ((FieldsVal[2] == "") || (t.CreatedOn >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsVal[2].Split(',')[0], Roomformat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.CreatedOn <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsVal[2].Split(',')[1], Roomformat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))
                    && ((FieldsVal[3] == "") || (t.UpdatedOn >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsVal[3].Split(',')[0], Roomformat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.UpdatedOn <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsVal[3].Split(',')[1], Roomformat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))

                    ).ToList();

                ObjCache = ObjCache.Where(t => t.ID.ToString().Contains(newSearchValue) ||
                         (t.Name ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.BarcodeKey ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.QuantityField ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.FontSize.ToString() ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.CreatedByName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UpdatedByName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UpdatedOn.ToString() ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.CreatedOn.ToString() ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0
                     ).ToList();

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media

                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                              (t.Name ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||

                        (t.BarcodeKey ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.QuantityField ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.FontSize.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedOn.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedOn.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();

                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                              (t.Name ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.BarcodeKey ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.QuantityField ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.FontSize.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedOn.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedOn.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public CatalogReportDetailDTO GetRecord(Int64 id, Int64 CompanyID)
        {
            return GetCachedData(CompanyID).FirstOrDefault(t => t.ID == id);
        }

        public CatalogReportDetailDTO GetRecord(string Templatename, Int64 CompanyID)
        {
            return GetCachedData(CompanyID).FirstOrDefault(t => t.Name == Templatename);
        }

        public List<string> GetListByGrouping_Old(string GroupNameField, string GroupIDField, Int64 RoomID, string ItemIDs = "")
        {
            string strQuery = "";
            string strItemIDsQry = "";
            if (!string.IsNullOrWhiteSpace(ItemIDs))
            {
                ItemIDs = ItemIDs.TrimEnd(',');
                strItemIDsQry = " and ItemID in (" + ItemIDs + ")";
            }
            if (!string.IsNullOrEmpty(GroupIDField))
                strQuery = @"SELECT Distinct  " + GroupIDField + " As ID," + GroupNameField + " As Name FROM [RPT_CatalogItems_View]  where ItemRoomID = " + RoomID + " and  ISNULL(ItemIsDeletedBit,0)=0 and ISNULL(" + GroupNameField + ",'') <> ''  " + strItemIDsQry + " Order By " + GroupNameField;
            else
                strQuery = @"SELECT Distinct Convert(bigInt,0) As ID," + GroupNameField + " As Name FROM [RPT_CatalogItems_View]  where ItemRoomID = " + RoomID + " and  ISNULL(ItemIsDeletedBit,0)=0 and ISNULL(" + GroupNameField + ",'') <> '' " + strItemIDsQry + " Order By " + GroupNameField;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //List<Int64> obj = context.ExecuteStoreQuery<Int64>(strQuery).ToList<Int64>();
                List<IdNameDTO> objList = context.ExecuteStoreQuery<IdNameDTO>(strQuery).ToList<IdNameDTO>();
                List<string> obj = objList.Select(x => x.Name).ToList<string>();
                return obj;
            }
        }

        public List<CatalogReportFieldsDTO> GetItemsByGroupNameAndId_Old(string FilterBy, string GroupNameField, string GroupIDField, Int64 RoomID, string SortField, bool isInActiveItems = true, string ItemIDs = "", string ItemBinIDs = "")
        {
            string itemActiveCondition = isInActiveItems ? "" : "and isnull(ItemIsActiveBit,0) = 1";
            string strItemIDsQry = "";
            string strBinIDsQry = "";

            if (!string.IsNullOrWhiteSpace(ItemIDs))
            {
                ItemIDs = ItemIDs.TrimEnd(',');
                strItemIDsQry = " and ItemID in (" + ItemIDs + ")";
            }

            if (!string.IsNullOrWhiteSpace(ItemBinIDs))
            {
                ItemBinIDs = ItemBinIDs.TrimEnd(',');
                strBinIDsQry = " and BinID in (" + ItemBinIDs + ")";
            }

            if (string.IsNullOrEmpty(SortField))
                SortField = "ItemNumber";

            string strQuery = @"SELECT * FROM [RPT_CatalogItems_View]  where " + GroupNameField + " = '" + (FilterBy ?? string.Empty).Replace("'", "''") + "' and ItemRoomID=" + RoomID + " and  ISNULL(ItemIsDeletedBit,0)=0 " + itemActiveCondition + " " + strItemIDsQry + " " + strBinIDsQry + "  Order By " + SortField;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<CatalogReportFieldsDTO> obj = context.ExecuteStoreQuery<CatalogReportFieldsDTO>(strQuery).ToList<CatalogReportFieldsDTO>();

                return obj;
            }
        }

        public List<CatalogReportFieldsDTO> GetItemsByGroupNameAndId_Old(Int64 RoomID, string SortField, bool isInActiveItems = true, string ItemIDs = "", string ItemBinIDs = "")
        {
            string itemActiveCondition = isInActiveItems ? "" : "and isnull(ItemIsActiveBit,0) = 1";
            string strItemIDsQry = "";
            string strBinIDsQry = "";

            if (!string.IsNullOrWhiteSpace(ItemIDs))
            {
                ItemIDs = ItemIDs.TrimEnd(',');
                strItemIDsQry = " and ItemID in (" + ItemIDs + ")";
            }

            if (!string.IsNullOrWhiteSpace(ItemBinIDs))
            {
                ItemBinIDs = ItemBinIDs.TrimEnd(',');
                strBinIDsQry = " and BinID in (" + ItemBinIDs + ")";
            }

            if (string.IsNullOrEmpty(SortField))
                SortField = "ItemNumber";

            string strQuery = @"SELECT * FROM [RPT_CatalogItems_View]  where  ItemRoomID=" + RoomID + " and  ISNULL(ItemIsDeletedBit,0)=0 " + itemActiveCondition + " " + strItemIDsQry + " " + strBinIDsQry + "  Order By " + SortField;


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<CatalogReportFieldsDTO> obj = context.ExecuteStoreQuery<CatalogReportFieldsDTO>(strQuery).ToList<CatalogReportFieldsDTO>();

                return obj;
            }
        }

    }

    public partial class CatalogReportTemplateMasterDAL : eTurnsBaseDAL
    {
        public IEnumerable<CatalogReportTemplateMasterDTO> GetMasterDataPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<CatalogReportTemplateMasterDTO> ObjCache = GetAllRecords(CompanyID);


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

        private IEnumerable<CatalogReportTemplateMasterDTO> GetCachedData()
        {
            //Get Cached-Media
            IEnumerable<CatalogReportTemplateMasterDTO> ObjCache = null;// CacheHelper<IEnumerable<CatalogReportTemplateMasterDTO>>.GetCacheItem("Cached_CatalogReportTemplateMaster");
            if (ObjCache == null || ObjCache.Count() < 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<CatalogReportTemplateMasterDTO> obj = (from u in context.ExecuteStoreQuery<CatalogReportTemplateMasterDTO>(@" SELECT A.* FROM CatalogReportTemplateMaster A ")
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
                    ObjCache = CacheHelper<IEnumerable<CatalogReportTemplateMasterDTO>>.AddCacheItem("Cached_CatalogReportTemplateMaster", obj);
                }
            }

            return ObjCache;
        }

        private IEnumerable<CatalogReportTemplateMasterDTO> GetBaseCachedData()
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<CatalogReportTemplateMasterDTO> obj = (from u in context.ExecuteStoreQuery<CatalogReportTemplateMasterDTO>(@" SELECT A.* FROM [" + DbConnectionHelper.GeteTurnsDBName() + "].[dbo].[CatalogReportTemplateMaster] A ")
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
                return obj;
            }
        }

        public IEnumerable<CatalogReportTemplateMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<CatalogReportTemplateMasterDTO> ObjCache = GetAllRecords();


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

        public IEnumerable<CatalogReportTemplateMasterDTO> GetAllRecords()
        {
            //return GetCachedData().Where(x => x.CompanyID == CompanyID).OrderBy("ID asc");
            return GetCachedData().OrderBy("LabelType asc,TemplateID asc");
        }

        public IEnumerable<CatalogReportTemplateMasterDTO> GetAllRecords(long CompanyID)
        {
            //return GetCachedData().Where(x => x.CompanyID == CompanyID).OrderBy("ID asc");
            return GetCachedData().Where(x => x.CompanyID == CompanyID).OrderBy("LabelType asc,TemplateID asc");
        }

        public CatalogReportTemplateMasterDTO GetRecord(Int64 TemplateID)
        {
            return GetCachedData().FirstOrDefault(t => t.TemplateID == TemplateID);
        }

        public CatalogReportTemplateMasterDTO GetRecord(Int64 TemplateID, long CompanyID)
        {
            return GetCachedData().FirstOrDefault(t => t.TemplateID == TemplateID && t.CompanyID == CompanyID);
        }

        public bool DeleteRecords(string IDs)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "Delete From CatalogReportTemplateMaster WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<CatalogReportTemplateMasterDTO> ObjCache = CacheHelper<IEnumerable<CatalogReportTemplateMasterDTO>>.GetCacheItem("Cached_CatalogReportTemplateMaster");
                if (ObjCache != null)
                {
                    List<CatalogReportTemplateMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<CatalogReportTemplateMasterDTO>>.AppendToCacheItem("Cached_CatalogReportTemplateMaster", ObjCache);
                }

                return true;
            }
        }

        public bool EditInBaseDB(CatalogReportTemplateMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strUpdate = @" Update [TemplateDBName].[dbo].[CatalogReportTemplateMaster] SET 
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

                CacheHelper<IEnumerable<CatalogReportTemplateMasterDTO>>.AddCacheItem("Cached_CatalogReportTemplateMaster", new List<CatalogReportTemplateMasterDTO>());

                return true;
            }
        }

        public bool EditInCurrentDB(CatalogReportTemplateMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strUpdate = @" Update [dbo].[CatalogReportTemplateMaster] SET 
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

                CacheHelper<IEnumerable<CatalogReportTemplateMasterDTO>>.AddCacheItem("Cached_CatalogReportTemplateMaster", new List<CatalogReportTemplateMasterDTO>());

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
	
                                      from [" + DbConnectionHelper.GeteTurnsDBName() + "].[dbo].[CatalogReportTemplateMaster] s inner join [" + item + @"].[dbo].[CatalogReportTemplateMaster] u on u.id = s.id;";

                            System.Data.Objects.ObjectResult<object> obj = context.ExecuteStoreQuery<object>(strUpdate);

                        }
                        catch (Exception)
                        {

                            // Log Execption Here
                        }


                    }
                }
                CacheHelper<IEnumerable<CatalogReportTemplateMasterDTO>>.AddCacheItem("Cached_CatalogReportTemplateMaster", new List<CatalogReportTemplateMasterDTO>());

                return true;
            }
        }

    }
}
