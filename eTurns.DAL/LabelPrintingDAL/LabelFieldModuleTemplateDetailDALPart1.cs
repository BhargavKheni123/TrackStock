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
using System.Web;
using eTurns.DTO.Resources;

namespace eTurns.DAL.LabelPrintingDAL
{
    public class LabelFieldModuleTemplateDetailDAL : eTurnsBaseDAL
    {
        private IEnumerable<LabelFieldModuleTemplateDetailDTO> GetCachedData(Int64 CompanyID, Int64 RoomID)
        {
            //Get Cached-Media
            IEnumerable<LabelFieldModuleTemplateDetailDTO> ObjCache = null;// CacheHelper<IEnumerable<LabelFieldModuleTemplateDetailDTO>>.GetCacheItem("Cached_LabelFieldModuleTemplateDetail_" + CompanyID.ToString());
            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<LabelFieldModuleTemplateDetailDTO> obj = (from u in context.ExecuteStoreQuery<LabelFieldModuleTemplateDetailDTO>(@"SELECT  A.ID	,A.Name  ,A.TemplateID  ,A.ModuleID	,MM.ModuleName ,A.FeildIDs  		
                                                                                                                                                            ,(CASE WHEN T.TemplateName IS NULL THEN (SELECT TMI.TemplateName FROM LabelTemplateMaster TMI WHERE TMI.CompanyID= " + CompanyID + @" AND TMI.TemplateID = A.TemplateID) ELSE T.TemplateName END)  AS TemplateName 
                                                                                                                                                            ,(SELECT STUFF((SELECT ', ' + t1.FieldName FROM LabelModuleFieldMaster t1 WHERE t1.ID  in (SELECT SplitValue FROM [dbo].[Split] ( A.FeildIDs  ,',')) FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)') ,1,2,'')) SelectedFieldsName
		                                                                                                                                                    ,A.IncludeBin	 	,A.IncludeQuantity	 ,A.QuantityField
		                                                                                                                                                    ,A.BarcodeKey		,MFM.FieldDisplayName as 'BarcodeKeyName'
		                                                                                                                                                    ,A.LabelHTML		,A.LabelXML	 ,A.FontSize ,A.CompanyID		
		                                                                                                                                                    ,A.CreatedBy		, B.UserName AS 'CreatedByName'
		                                                                                                                                                    ,A.UpdatedBy		,C.UserName AS 'UpdatedByName'		
		                                                                                                                                                    ,A.CreatedOn		,A.UpdatedOn ,A.IsArchived		,A.IsDeleted
    		                                                                                                                                                ,A.TextFont         ,A.BarcodeFont
                                                                                                                                                            --,Convert(Bit,Case When ISNULL(MTD.ID,0) >0 Then 1 else 0 end) AS IsSelectedInModuleConfig                                                                                                                                                                            
                                                                                                                                                            ,Convert(Bit,Case When (SELECT Count(MTD1.ID) FROM LabelModuleTemplateDetail MTD1 WHERE A.ID=MTD1.TemplateDetailID and A.ModuleID = MTD1.ModuleID and MTD1.CompanyID=" + CompanyID + @" AND MTD1.RoomID=" + RoomID + @" ) > 0 Then 1 else 0 end) AS IsSelectedInModuleConfig
                                                                                                                                                            ,A.BarcodePattern
                                                                                                                                                    FROM LabelFieldModuleTemplateDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
									                                                                                                                                                      LEFT OUTER JOIN UserMaster C on A.UpdatedBy = C.ID  
                                                                                                                                                                                         -- LEFT OUTER JOIN LabelTemplateMaster T on (A.TemplateID = T.ID OR (A.TemplateID = T.TemplateID AND A.CompanyID=T.CompanyID))
                                                                                                                                                                                          LEFT OUTER JOIN LabelTemplateMaster T on ((A.TemplateID = T.TemplateID AND A.CompanyID=T.CompanyID))
                                                                                                                                                                                          LEFT OUTER JOIN LabelModuleMaster MM On A.ModuleID = MM.ID 
									                                                                                                                                                      LEFT OUTER JOIN LabelModuleFieldMaster MFM on A.BarcodeKey = MFM.ID
                                                                                                                                                                                          --LEFT OUTER JOIN LabelModuleTemplateDetail MTD on A.ID = MTD.TemplateDetailID  AND A.ModuleID = MTD.ModuleID AND (A.CompanyID = MTD.CompanyID OR A.CompanyID =-1)  
                                                                                                                                                   WHERE (A.CompanyID = -1 OR A.CompanyID = " + CompanyID.ToString() + ") ")
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
                    ObjCache = obj.Where(x => x.CompanyID != 0);// CacheHelper<IEnumerable<LabelFieldModuleTemplateDetailDTO>>.AddCacheItem("Cached_LabelFieldModuleTemplateDetail_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache;
        }

        public IEnumerable<LabelFieldModuleTemplateDetailDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, string Roomformat, TimeZoneInfo CurrentTimeZone, bool IncludeEnterpriseLabels = true)
        {
            //Get Cached-Media
            //List<LabelFieldModuleTemplateDetailDTO> ObjCache = GetCachedData(CompanyID).Where(x => x.IsDeleted == IsDeleted && x.IsArchived == IsArchived).ToList();
            List<LabelFieldModuleTemplateDetailDTO> ObjCache = GetCachedData(CompanyID, RoomID).Where(x => x.IsDeleted == IsDeleted && x.IsArchived == IsArchived).ToList();
            if (!IncludeEnterpriseLabels)
            {
                ObjCache = ObjCache.Where(x => x.CompanyID > 0).ToList();
            }
            //else
            //{
            //    ObjCache = ObjCache.Where(x => x.CompanyID == -1 || x.CompanyID == CompanyID).ToList();
            //}

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


                if (FieldsVal != null && FieldsVal.Length > 0)
                {
                    if (FieldsVal[5].Contains("11"))
                    {
                        IEnumerable<LabelFieldModuleTemplateDetailDTO> ObjCacheBase = GetCachedData(CompanyID, RoomID).Where(x => x.IsDeleted == IsDeleted && x.IsArchived == IsArchived && x.CompanyID == -1);
                        foreach (var item in ObjCacheBase)
                        {
                            ObjCache.Add(item);
                        }
                    }
                    else if (FieldsVal[5].Contains("22"))
                    {
                        ObjCache = GetCachedData(CompanyID, RoomID).Where(x => x.IsDeleted == IsDeleted && x.IsArchived == IsArchived && x.CompanyID == -1).ToList();
                    }

                }

                ObjCache = ObjCache.Where(t =>
                       ((FieldsVal[0] == "") || (FieldsVal[0].Split(',').Select(Int64.Parse).ToArray().Contains(t.ModuleID)))
                    && ((FieldsVal[1] == "") || (FieldsVal[1].Split(',').Select(Int64.Parse).ToArray().Contains(t.CreatedBy)))
                    && ((FieldsVal[2] == "") || (FieldsVal[2].Split(',').Select(Int64.Parse).ToArray().Contains(t.UpdatedBy)))
                    //&& ((FieldsVal[3] == "") || (t.CreatedOn >= Convert.ToDateTime(FieldsVal[3].Split(',')[0]).Date && t.CreatedOn <= Convert.ToDateTime(FieldsVal[2].Split(',')[1]).Date))
                    //&& ((FieldsVal[4] == "") || (t.UpdatedOn >= Convert.ToDateTime(FieldsVal[4].Split(',')[0]).Date && t.UpdatedOn <= Convert.ToDateTime(FieldsVal[3].Split(',')[1]).Date))
                    && ((FieldsVal[3] == "") || (t.CreatedOn >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsVal[3].Split(',')[0], Roomformat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.CreatedOn <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsVal[3].Split(',')[1], Roomformat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))
                    && ((FieldsVal[4] == "") || (t.UpdatedOn >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsVal[4].Split(',')[0], Roomformat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.UpdatedOn <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsVal[4].Split(',')[1], Roomformat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))

                    ).ToList();

                ObjCache = ObjCache.Where(t => t.ID.ToString().Contains(newSearchValue) ||
                               (t.Name ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.TemplateName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.ModuleName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.BarcodeKeyName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
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
                        (t.TemplateName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ModuleName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ModuleName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.BarcodeKeyName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
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
                        (t.TemplateName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ModuleName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.BarcodeKeyName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.QuantityField ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.FontSize.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedOn.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedOn.ToString() ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public IEnumerable<LabelFieldModuleTemplateDetailDTO> GetAllRecords(Int64 CompanyId, Int64 RoomID, bool IncludeEnterpriseLabels = true)
        {
            //return GetCachedData(CompanyId).OrderBy("ID DESC");
            IEnumerable<LabelFieldModuleTemplateDetailDTO> ObjCache = GetCachedData(CompanyId, RoomID);
            if (!IncludeEnterpriseLabels)
            {
                ObjCache = ObjCache.Where(x => x.CompanyID > 0).OrderBy("ID DESC");
            }
            //else
            //{
            //    ObjCache = ObjCache.Where(x => x.CompanyID == -1 || x.CompanyID == CompanyId).OrderBy("ID DESC");
            //}

            return ObjCache;
        }

        public LabelFieldModuleTemplateDetailDTO GetRecord(Int64 id, Int64 CompanyID, Int64 RoomID)
        {
            return GetCachedData(CompanyID, RoomID).FirstOrDefault(t => t.ID == id);
        }

        public LabelFieldModuleTemplateDetailDTO GetRecord(string Templatename, Int64 CompanyID, Int64 RoomID)
        {
            return GetCachedData(CompanyID, RoomID).FirstOrDefault(t => t.Name == Templatename);
        }

        public string DeleteSelectedRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID)
        {
            if (string.IsNullOrEmpty(IDs) || IDs.Trim().Length <= 0)
            {
                return "No record selected";
            }
            string returnMsg = "";
            string setDefaultQuery = string.Empty;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
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
                            strQuery += "UPDATE LabelFieldModuleTemplateDetail SET UpdatedOn = getutcdate() , UpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                            if (objMTD != null && objMTD.ID > 0 && templateID > 0)
                            {
                                setDefaultQuery += @" DELETE FROM LabelModuleTemplateDetail WHERE ModuleID= " + objMTD.ModuleID + " AND CompanyID = " + CompanyID + " And RoomID = " + RoomID + "; ";
                                setDefaultQuery += @" INSERT INTO LabelModuleTemplateDetail (ModuleID,TemplateDetailID,CompanyID,CreatedBy,UpdatedBy,CreatedOn,UpdatedOn,RoomID) ";
                                setDefaultQuery += @" Values (" + objMTD.ModuleID + "," + templateID + "," + CompanyID + "," + userid + "," + userid + ",getutcdate(),getutcdate()," + RoomID + "); ";
                            }
                        }
                        else
                        {
                            returnMsg = "Default template can not be delete.";
                        }
                    }
                }

                if (!string.IsNullOrEmpty(strQuery))
                {
                    strQuery += " ";
                    if (!string.IsNullOrEmpty(setDefaultQuery))
                        strQuery += setDefaultQuery;

                    context.ExecuteStoreCommand(strQuery);
                }

                if (string.IsNullOrEmpty(returnMsg))
                    returnMsg = "ok";

                return returnMsg;
            }
        }

        public string DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID)
        {
            if (string.IsNullOrEmpty(IDs) || IDs.Trim().Length <= 0)
            {
                return "No record selected";
            }
            string msgs = "";
            string msgf = "";
            string msg = "";
            //LabelFieldModuleTemplateDetailDTO objFMTDDTO = null;
            //LabelModuleTemplateDetailDTO objMTDDTO = null;
            //LabelFieldModuleTemplateDetailDAL objFMTDDAL = new LabelFieldModuleTemplateDetailDAL(base.DataBaseName);
            //LabelModuleTemplateDetailDAL objMTDDAL = new LabelModuleTemplateDetailDAL(base.DataBaseName);

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";

                Int64[] intIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();

                LabelModuleTemplateDetail objMTD = null;
                LabelFieldModuleTemplateDetail objFMTD = null;
                foreach (var item in intIDs)
                {
                    if (item > 0)
                    {
                        //objFMTDDTO = objFMTDDAL.GetRecord(item, CompanyID,RoomID);
                        objFMTD = context.LabelFieldModuleTemplateDetails.FirstOrDefault(x => x.ID == item);
                        objMTD = context.LabelModuleTemplateDetails.FirstOrDefault(x => x.TemplateDetailID == item);

                        //objMTDDTO = objMTDDAL.GetAllRecords(CompanyID, RoomID).FirstOrDefault(x => x.TemplateDetailID == item);
                        if (!(objMTD != null && objMTD.ID > 0))
                        {
                            strQuery += "UPDATE LabelFieldModuleTemplateDetail SET UpdatedOn = getutcdate() , UpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                            if (!string.IsNullOrEmpty(msgs))
                                msgs += ",";

                            msgs += objFMTD.Name;
                        }
                        else
                        {
                            //msg += objFDTO.TemplateName + " Template is not deleted becuase it is used by anothor module. ";
                            if (!string.IsNullOrEmpty(msgf))
                                msgf += ",";

                            msgf += objFMTD.Name;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(strQuery))
                    context.ExecuteStoreCommand(strQuery);

                ////Get Cached-Media
                //IEnumerable<LabelFieldModuleTemplateDetailDTO> ObjCache = CacheHelper<IEnumerable<LabelFieldModuleTemplateDetailDTO>>.GetCacheItem("Cached_LabelFieldModuleTemplateDetail_" + CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<LabelFieldModuleTemplateDetailDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                //    ObjCache = objTemp.AsEnumerable();
                //    CacheHelper<IEnumerable<LabelFieldModuleTemplateDetailDTO>>.AppendToCacheItem("Cached_LabelFieldModuleTemplateDetail_" + CompanyID.ToString(), ObjCache);
                //}

                if (!string.IsNullOrEmpty(msgs))
                {
                    //msg += msgs + " deleted successfully. ";
                    msg = "ok";
                }
                if (!string.IsNullOrEmpty(msgf))
                {
                    msg += msgf + " not deleted successfully. ";
                }
                return msg;
            }
        }

        public IEnumerable<LabelFieldModuleTemplateDetailDTO> GetBaseTemplateList()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<LabelFieldModuleTemplateDetailDTO> obj = (from u in context.ExecuteStoreQuery<LabelFieldModuleTemplateDetailDTO>(@"SELECT  A.ID	,A.Name  ,A.TemplateID  ,T.TemplateName  ,A.ModuleID	,MM.ModuleName 
		                                                                                                                                                    ,A.FeildIDs  		
                                                                                                                                                            ,(SELECT STUFF((SELECT ', ' + t1.FieldName FROM LabelModuleFieldMaster t1 WHERE t1.ID  in (SELECT SplitValue FROM [dbo].[Split] ( A.FeildIDs  ,',')) FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)') ,1,2,'')) SelectedFieldsName
		                                                                                                                                                    ,A.IncludeBin	 	,A.IncludeQuantity	 ,A.QuantityField
		                                                                                                                                                    ,A.BarcodeKey		,MFM.FieldDisplayName as 'BarcodeKeyName'
		                                                                                                                                                    ,A.LabelHTML		,A.LabelXML	 ,A.FontSize ,A.CompanyID		
		                                                                                                                                                    ,A.CreatedBy		, B.UserName AS 'CreatedByName'
		                                                                                                                                                    ,A.UpdatedBy		,C.UserName AS 'UpdatedByName'		
		                                                                                                                                                    ,A.CreatedOn		,A.UpdatedOn ,A.IsArchived		,A.IsDeleted
    		                                                                                                                                                ,A.TextFont         ,A.BarcodeFont
                                                                                                                                                    FROM LabelFieldModuleTemplateDetail A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
									                                                                                                                                                      LEFT OUTER JOIN UserMaster C on A.UpdatedBy = C.ID  
                                                                                                                                                                                          LEFT OUTER JOIN LabelTemplateMaster T on A.TemplateID = T.ID  
                                                                                                                                                                                          LEFT OUTER JOIN LabelModuleMaster MM On A.ModuleID = MM.ID 
									                                                                                                                                                      LEFT OUTER JOIN LabelModuleFieldMaster MFM on A.BarcodeKey = MFM.ID
                                                                                                                                                                                          
                                                                                                                                                   WHERE A.CompanyID = 0")
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
                return obj;
            }



        }

        public int CopyAllBaseTemplate(Int64 CompanyID, Int64 UserID, Int64 RoomID)
        {
            string query = @"INSERT INTO [dbo].[LabelTemplateMaster]
				                    ([TemplateID]			,[TemplateName]				,[LabelSize]			,[NoOfLabelPerSheet]
				                    ,[NoOfColumns]			,[PageWidth]				,[PageHeight]			,[LabelWidth]
				                    ,[LabelHeight]			,[PageMarginLeft]			,[PageMarginRight]		,[PageMarginTop]
				                    ,[PageMarginBottom]		,[LabelSpacingHorizontal]	,[LabelSpacingVerticle]	,[LabelPaddingLeft]
				                    ,[LabelPaddingRight]	,[LabelPaddingTop]          ,[LabelPaddingBottom]   ,[LabelType]	         ,[CompanyID])
		                    SELECT [TemplateID]				,[TemplateName]				,[LabelSize]			,[NoOfLabelPerSheet]
				                    ,[NoOfColumns]			,[PageWidth]				,[PageHeight]			,[LabelWidth]
				                    ,[LabelHeight]			,[PageMarginLeft]			,[PageMarginRight]		,[PageMarginTop]
				                    ,[PageMarginBottom]		,[LabelSpacingHorizontal]	,[LabelSpacingVerticle]	,[LabelPaddingLeft]
				                    ,[LabelPaddingRight]	,[LabelPaddingTop]          ,[LabelPaddingBottom]   ,[LabelType]	         ," + CompanyID + @" AS [CompanyID]
		                    FROM [LabelTemplateMaster] WHERE ISNULL(CompanyID,0) =0; 


                            INSERT INTO LabelFieldModuleTemplateDetail
                            SELECT Name		,TemplateID		,ModuleID		,FeildIDs	,IncludeBin		,IncludeQuantity	
				                            ,QuantityField	,BarcodeKey		,LabelHTML	,LabelXML		,FontSize
				                            ,TextFont		,BarcodeFont	," + CompanyID + @"	," + UserID + @"	," + UserID + @"
				                            ,getutcdate()	,getutcdate()		,IsArchived	,IsDeleted, BarcodePattern
                            FROM LabelFieldModuleTemplateDetail
                            WHERE CompanyID = 0 AND ISNULL(IsDeleted,0) = 0 and ISNULL(IsArchived,0) = 0";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int rowAffected = context.ExecuteStoreCommand(query);
                if (rowAffected > 0)
                {
                    //var obj = context.ExecuteStoreQuery<object>(query);
                    // string detailQuery = string.Empty;
                    // IEnumerable<LabelFieldModuleTemplateDetailDTO> lstBaseTemplates = GetCachedData(CompanyID).Where(x => x.CompanyID == CompanyID);
                    //foreach (var item in lstBaseTemplates)
                    //{
                    //    detailQuery += @" DELETE FROM LabelModuleTemplateDetail WHERE ModuleID= " + item.ModuleID + " AND CompanyID = " + CompanyID + " and RoomID=" + RoomID + @"; ";
                    //    detailQuery += @" INSERT INTO LabelModuleTemplateDetail (ModuleID,TemplateDetailID,CompanyID,CreatedBy,UpdatedBy,CreatedOn,UpdatedOn,RoomID) ";
                    //    detailQuery += @"                                     Values (" + item.ModuleID + "," + item.ID + "," + CompanyID + "," + UserID + "," + UserID + ",getutcdate(),getutcdate()," + RoomID + "); ";
                    //    detailQuery += " ";
                    //}
                    // int rowAffects = context.ExecuteStoreCommand(detailQuery);
                }

                return rowAffected;
            }
        }

        public int CopyAllBaseTemplateRoomLevel(Int64 CompanyID, Int64 UserID, Int64 RoomID)
        {
            int rowAffected = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string detailQuery = "";
                detailQuery += @"DELETE FROM LabelModuleTemplateDetail WHERe RoomID=" + RoomID + ";";
                detailQuery += @"INSERT INTO LabelModuleTemplateDetail 
                                       (ModuleID,  TemplateDetailID,CompanyID,  CreatedBy,UpdatedBy,CreatedOn,UpdatedOn,RoomID)
                                SELECT A.ModuleID, A.ID,            A.CompanyID," + UserID + @"," + UserID + @",GETUTCDATE(),GETUTCDATE()," + RoomID + @" 
                                FROM LabelFieldModuleTemplateDetail A 
                                WHERE A.CompanyID = " + CompanyID + @" and A.IsDeleted=0 
                                AND A.Name IN (SELECT B.Name FROM LabelFieldModuleTemplateDetail B 
                                                WHERE B.CompanyId=0 AND A.ModuleID=B.ModuleID AND A.TemplateID=B.TemplateID);";

                //IEnumerable<LabelFieldModuleTemplateDetailDTO> lstBaseTemplates = GetAllBaseTemplateCompanyWise(CompanyID);
                //foreach (var item in lstBaseTemplates)
                //{
                //    detailQuery += @" DELETE FROM LabelModuleTemplateDetail WHERE ModuleID= " + item.ModuleID + " AND CompanyID = " + CompanyID + " and RoomID=" + RoomID + @" and TemplateDetailID=" + item.ID + "; ";
                //    detailQuery += @" INSERT INTO LabelModuleTemplateDetail (ModuleID,TemplateDetailID,CompanyID,CreatedBy,UpdatedBy,CreatedOn,UpdatedOn,RoomID) ";
                //    detailQuery += @"                                     Values (" + item.ModuleID + "," + item.ID + "," + CompanyID + "," + UserID + "," + UserID + ",getutcdate(),getutcdate()," + RoomID + "); ";
                //    detailQuery += " ";
                //}
                int rowAffects = context.ExecuteStoreCommand(detailQuery);
            }

            return rowAffected;

        }

        public int SetAsDefaultTemplateForModule(Int64 TemplateDetailID, Int64 CompanyID, Int64 UserID, Int64 ModuleID, Int64 RoomID)
        {
            int rowAffects = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string detailQuery = string.Empty;
                detailQuery += @" DELETE FROM LabelModuleTemplateDetail WHERE ModuleID= " + ModuleID + " AND CompanyID = " + CompanyID + " And RoomID = " + RoomID + "; ";
                detailQuery += @" INSERT INTO LabelModuleTemplateDetail (ModuleID,TemplateDetailID,CompanyID,CreatedBy,UpdatedBy,CreatedOn,UpdatedOn,RoomID) ";
                detailQuery += @"                                     Values (" + ModuleID + "," + TemplateDetailID + "," + CompanyID + "," + UserID + "," + UserID + ",getutcdate(),getutcdate()," + RoomID + "); ";
                detailQuery += " ";

                rowAffects = context.ExecuteStoreCommand(detailQuery);
            }
            return rowAffects;
        }


    }
}
