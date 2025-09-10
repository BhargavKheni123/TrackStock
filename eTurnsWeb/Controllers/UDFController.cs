using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using eTurnsWeb.Controllers.AngularApp;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class UDFController : eTurnsControllerBase
    {



        public ActionResult UDFList(long? id, string t)
        {
            //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
            int iUDFMaxLength = 200;

            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (t != null)
            {
                if (eTurnsWeb.Models.UDFDictionaryTables.IsVaidUDFTable(t.ToLower()))
                {
                    int ModuleId = 0;
                    string UDFTableName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFTableFromKey(t.ToLower(), out ModuleId);
                    bool isAllowUDF = eTurnsWeb.Helper.SessionHelper.GetModulePermission((SessionHelper.ModuleList)ModuleId, eTurnsWeb.Helper.SessionHelper.PermissionType.ShowUDF);
                    if (!isAllowUDF)
                    {
                        return RedirectToAction("Dashboard", "Master");
                    }

                    ViewBag.UDFTableName = UDFTableName;
                    ViewBag.UDFTableNameKey = t;
                    if (Request.QueryString["UDFHeader"] == null)
                    {
                        ViewBag.UDFHeader = "";
                    }
                    else
                    {
                        ViewBag.UDFHeader = Request.QueryString["UDFHeader"];
                    }
                    ViewBag.EnterpriseID = id;
                    //UDFApiController obj = new UDFApiController();
                    UDFDAL obj = new UDFDAL(SessionHelper.EnterPriseDBName);
                    eTurnsMaster.DAL.UDFDAL objUDFDALMaster = new eTurnsMaster.DAL.UDFDAL();
                    int ResourcePageId = SessionHelper.GetUDFListID(UDFTableName);
                    if (UDFTableName == "Enterprise")
                    {
                        objUDFDALMaster.InsertDefaultUDFs(UDFTableName, SessionHelper.UserID, id ?? 0, iUDFMaxLength);
                    }
                    else if (UDFTableName.ToLower() == "usermaster")
                    {

                        //string OldUserMasterUDF = Convert.ToString(Settinfile.Element("OldUserMasterUDF").Value);
                        string OldUserMasterUDF = Convert.ToString(SiteSettingHelper.OldUserMasterUDF);

                        if (OldUserMasterUDF == "1")
                        {
                            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterPriseDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                            List<EnterpriseDTO> lstEnterpriseList = objEnterPriseDAL.GetAllEnterpriseForExecutionWithDemo();

                            foreach (EnterpriseDTO objEnt in lstEnterpriseList)
                            {
                                UDFDAL objUDFDAL = new UDFDAL(objEnt.EnterpriseDBName);
                                objUDFDAL.InsertDefaultUDFs(UDFTableName, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomID, ResourcePageId, iUDFMaxLength);
                            }
                        }
                        else
                        {
                            obj.InsertUserMasterUDFs(UDFTableName, 0, SessionHelper.UserID, 0, ResourcePageId, iUDFMaxLength);
                        }
                    }
                    else
                    {
                        var udfCountBeforeInserted = obj.GetNonDeletedUDFCountByUDFTableName(UDFTableName, SessionHelper.RoomID, SessionHelper.CompanyID);
                        obj.InsertDefaultUDFs(UDFTableName, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomID, ResourcePageId, iUDFMaxLength);
                        var udfCountAfterInserted = obj.GetNonDeletedUDFCountByUDFTableName(UDFTableName, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (udfCountAfterInserted > udfCountBeforeInserted)
                        {
                            UpdateUserUISetting((udfCountAfterInserted - udfCountBeforeInserted), UDFTableName);
                        }
                    }

                    if (ResourcePageId > 0)
                    {
                        ViewBag.ShowPDAField = true;
                    }
                    else
                    {
                        ViewBag.ShowPDAField = false;
                    }

                    return View();
                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }
        }

        private void UpdateUserUISetting(int udfCount, string UDFTableName)
        {
            eTurnsMaster.DAL.CommonMasterDAL objUDF = new eTurnsMaster.DAL.CommonMasterDAL();
            Dictionary<int, string> GridListName = objUDF.getGridResourceListName(UDFTableName);
            long ExtraCount = 0;

            if (GridListName != null && GridListName.Count() > 0)
            {
                UDFDAL obj1 = new UDFDAL(SessionHelper.EnterPriseDBName);
                foreach (KeyValuePair<int, string> ReportResourceFileName in GridListName)
                {
                    string[] Values = ReportResourceFileName.Value.Split('$');

                    if (Values != null && Values.Count() > 0)
                    {
                        long ExtraUDFinGrid = 0;

                        if (Values[1] == "Yes")
                        {
                            //IEnumerable<UDFDTO> DataFromDB = null;
                            //int TotalRecordCount = 0;
                            ExtraUDFinGrid = obj1.GetNonDeletedUDFCountByUDFTableName(Values[2], SessionHelper.RoomID, SessionHelper.CompanyID);
                            //DataFromDB = obj1.GetPagedRecords(0, 10, out TotalRecordCount, string.Empty, "ID asc", SessionHelper.CompanyID, Values[2], SessionHelper.RoomID);

                            //if (DataFromDB != null && DataFromDB.Count() > 0)
                            //{
                            //    ExtraUDFinGrid = DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
                            //}
                        }
                        ExtraCount += ExtraUDFinGrid + Convert.ToInt64(Values[3]);

                        if (UDFTableName.ToLower() == "toolmaster" && SessionHelper.AllowToolOrdering)
                        {
                            Values[0] = "ToolListNew";
                        }

                        UpdateUserUISettingForUDFColumns(udfCount, UDFTableName, Values[0], ExtraCount);

                        if (UDFTableName.ToLower() == "pullmaster" && Values[0].ToLower() != "pullmaster")
                        {
                            //IEnumerable<UDFDTO> DataFromDB = null;
                            //int TotalRecordCount = 0;
                            var udfTables = new string[] { "RequisitionMaster", "WorkOrder" };
                            long extraUDFinGrid = 0;
                            long extraCount = 0;

                            foreach (var tableName in udfTables)
                            {
                                extraUDFinGrid += obj1.GetNonDeletedUDFCountByUDFTableName(tableName, SessionHelper.RoomID, SessionHelper.CompanyID);

                                //DataFromDB = obj1.GetPagedRecords(0, 10, out TotalRecordCount, string.Empty, "ID asc", SessionHelper.CompanyID, tableName, SessionHelper.RoomID);

                                //if (DataFromDB != null && DataFromDB.Count() > 0)
                                //{
                                //    extraUDFinGrid += DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
                                //}
                            }
                            extraCount += extraUDFinGrid + Convert.ToInt64(Values[3]);
                            UpdateUserUISettingForUDFColumns(udfCount, UDFTableName, "PullMaster", extraCount);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// This method is used to update user ui setting entry for grid when UDF records are inserted from enterprise setup.
        /// </summary>
        /// <param name="UDFTableName"></param>
        /// <param name="ListName"></param>
        /// <param name="ExtraCount"></param>
        private void UpdateUserUISettingForUDFColumns(int udfCount, string UDFTableName, string ListName, long ExtraCount)
        {
            if (udfCount > 0 && !string.IsNullOrEmpty(ListName))
            {
                eTurnsMaster.DAL.UsersUISettingsDAL obj = new eTurnsMaster.DAL.UsersUISettingsDAL();
                UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
                objDTO = obj.GetRecord(SessionHelper.UserID, ListName, SiteSettingHelper.UsersUISettingType, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);

                if (objDTO == null || string.IsNullOrEmpty(objDTO.JSONDATA))
                {
                    SiteListMasterDAL siteListMasterDAL = new SiteListMasterDAL(SessionHelper.EnterPriseDBName);
                    string siteListMasterJson = siteListMasterDAL.GetSiteListMasterDataByListName(ListName, SiteSettingHelper.UsersUISettingType);

                    objDTO = new UsersUISettingsDTO
                    {
                        ListName = ListName,
                        JSONDATA = siteListMasterJson
                    };
                }

                if (objDTO != null)
                {
                    if (!string.IsNullOrEmpty(objDTO.JSONDATA))
                    {
                        JObject gridStateJS = new JObject();
                        // jsonData = objDTO.JSONDATA;
                        /*////////CODE FOR UPDATE JSON STRING/////////*/
                        // JObject gridStaeJS = new JObject();
                        gridStateJS = JObject.Parse(objDTO.JSONDATA);
                        /*////////CODE FOR UPDATE JSON STRING/////////*/

                        JToken orderCols = gridStateJS["ColReorder"];
                        JArray arrOCols = (JArray)orderCols;
                        JArray arrONewCols = new JArray();

                        if (arrOCols != null)
                        {
                            int orderClength = arrOCols.Count;

                            if (orderClength > 4)
                            {
                                JToken abVisCols = gridStateJS["abVisCols"];
                                JArray visCols = (JArray)abVisCols;
                                JToken aoSearchCols = gridStateJS["aoSearchCols"];
                                JArray arrSCols = (JArray)aoSearchCols;

                                if (arrSCols != null)
                                {
                                    JObject UpdateAccProfile = new JObject(
                                           new JProperty("bCaseInsensitive", true),
                                           new JProperty("sSearch", ""),
                                           new JProperty("bRegex", false),
                                           new JProperty("bSmart", true));
                                    for (int count = 0; count < udfCount; count++)
                                    {
                                        arrSCols.Add((object)UpdateAccProfile);
                                    }
                                }

                                if (visCols != null)
                                {
                                    int visClength = visCols.Count;
                                    for (int count = 0; count < udfCount; count++)
                                    {
                                        visCols.Add(true);
                                    }
                                }

                                JToken widthCols = gridStateJS["ColWidth"];
                                JArray arrWCols = (JArray)widthCols;

                                if (arrWCols != null)
                                {
                                    //int widthlength = arrWCols.Count;
                                    for (int count = 0; count < udfCount; count++)
                                    {
                                        arrWCols.Insert(arrWCols.Count, "48px");
                                    }
                                }

                                int maxOrder = arrOCols.Select(c => (int)c).ToList().Max();
                                long currentUDFVAl = (Convert.ToInt32(maxOrder) + 1); //(Convert.ToInt32(maxOrder) - Convert.ToInt32(ExtraCount) + 1);

                                for (int count = 0; count < udfCount; count++)
                                {
                                    for (int i = 0; i < orderClength; i++)
                                    {
                                        if (Convert.ToInt32(((JValue)(arrOCols[i])).Value) >= currentUDFVAl + count)
                                        {
                                            ((JValue)(arrOCols[i])).Value = Convert.ToInt32(((JValue)(arrOCols[i])).Value) + 1;
                                        }
                                    }
                                    arrOCols.Insert(arrOCols.Count, currentUDFVAl + count);
                                }

                                gridStateJS["ColReorder"] = arrOCols;
                                string updatedJSON = gridStateJS.ToString();

                                /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                                objDTO = new UsersUISettingsDTO();
                                objDTO.UserID = SessionHelper.UserID;
                                objDTO.EnterpriseID = SessionHelper.EnterPriceID;
                                objDTO.CompanyID = SessionHelper.CompanyID;
                                objDTO.RoomID = SessionHelper.RoomID;
                                objDTO.JSONDATA = updatedJSON;
                                objDTO.ListName = ListName;
                                obj.SaveUserListViewSettings(objDTO, SiteSettingHelper.UsersUISettingType, true);
                                /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                            }
                        }
                    }
                }
            }
        }

        public ActionResult UDFListEnterprise(long? id)
        {
            string t = "enterprises";
            if (t != null)
            {
                if (eTurnsWeb.Models.UDFDictionaryTables.IsVaidUDFTable(t.ToLower()))
                {
                    int ModuleId = 0;
                    string UDFTableName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFTableFromKey(t.ToLower(), out ModuleId);
                    ViewBag.UDFTableName = UDFTableName;
                    ViewBag.UDFTableNameKey = t;
                    //UDFApiController obj = new UDFApiController();
                    UDFDAL obj = new UDFDAL(SessionHelper.EnterPriseDBName);
                    eTurnsMaster.DAL.UDFDAL objUDFDALMaster = new eTurnsMaster.DAL.UDFDAL();
                    if (UDFTableName == "Enterprise")
                    {
                        objUDFDALMaster.InsertDefaultUDFs(UDFTableName, SessionHelper.UserID, id ?? 0);
                    }
                    else
                    {
                        obj.InsertDefaultUDFs(UDFTableName, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomID);
                    }


                    return View();
                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }
        }
        public PartialViewResult _CreateUDF()
        {

            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult UDFCreate(string t)
        {
            int ResourcePageId = SessionHelper.GetUDFListID(t);
            if (ResourcePageId > 0)
            {
                ViewBag.ShowPDAField = true;
            }
            else
            {
                ViewBag.ShowPDAField = false;
            }
            UDFDTO objDTO = new UDFDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.GUID = Guid.NewGuid();
            if (t == "PullMaster" && ResourcePageId == 30)
            {
                objDTO.ShowEncryptionCheckBox = true;
            }
            else
            {
                objDTO.ShowEncryptionCheckBox = false;
            }

            if (t.ToLower() == "companymaster")
                objDTO.CompanyID = 0;

            if (t != null)
            {
                objDTO.UDFTableName = t;
                ViewBag.UDFTableName = t;
            }
            return PartialView("_CreateUDF", objDTO);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult UDFSave(UDFDTO objDTO)
        {
            if (Session["ControlType"] != null)
            {
                UDFDTO oldObj = Session["ControlType"] as UDFDTO;
                if (string.IsNullOrEmpty(oldObj.UDFControlType) || (oldObj.IsDeleted && objDTO.IsDeleted == false))
                {
                    Session["ControlType"] = null;
                }
            }

            //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
            int iUDFMaxLength = 200;

            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            string message = "";
            string status = "";
            string t = objDTO.ValueString;
            JavaScriptSerializer objJSSerial = new JavaScriptSerializer();
            List<UDFOptionsDTO> jResult = GetUDFOptionsByUDFNew(objDTO.ID, objDTO.UDFTableName, SessionHelper.EnterPriceID, (!objDTO.OtherFromeTurns) ?? false);
            long RoomId = 0;
            long companyId = 0;
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterPriseDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            List<EnterpriseDTO> lstEnterpriseList = objEnterPriseDAL.GetAllEnterpriseForExecutionWithDemo();
            if (objDTO.UDFTableName.ToLower() != "usermaster")
            {
                if ((objDTO.OtherFromeTurns) ?? true)
                {
                    if (!(objDTO.SetUpForEnterpriseLevel ?? false))
                    {
                        RoomId = SessionHelper.RoomID;
                        companyId = SessionHelper.CompanyID;
                    }

                }
            }
            if (objDTO.IsEncryption == null)
            {
                objDTO.IsEncryption = false;
            }

            List<UDFOptionsDTO> NewString = JsonConvert.DeserializeObject<List<UDFOptionsDTO>>(t);

            if (NewString != null && NewString.Count > 0)
            {
                var maxLengthFromList = NewString.Select(x => (x.UDFOption ?? string.Empty).Length).Max();

                if (objDTO.UDFMaxLength.GetValueOrDefault(0) > 0 && objDTO.UDFMaxLength.GetValueOrDefault(0) < maxLengthFromList)
                {
                    message = string.Format(ResUDFSetup.UDFOptionValueLengthCantMoreThanUDFMaxLength, objDTO.UDFMaxLength); 
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);

                }

            }

            //Dictionary<string, string> NewString = test.ToDictionary( g => g.Key, g => g.va    );
            bool isExists = true;
            int ResourcePageId = SessionHelper.GetUDFListID(objDTO.UDFTableName);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);



            foreach (UDFOptionsDTO u in jResult)
            {
                isExists = false;

                foreach (UDFOptionsDTO a in NewString)
                {
                    if (u.ID == a.ID)
                    {
                        isExists = true;
                        if (!string.IsNullOrEmpty(u.UDFOption) && !string.IsNullOrEmpty(a.UDFOption) && u.UDFOption.Trim() != a.UDFOption.Trim())
                        {
                            if ((objDTO.IsEncryption ?? false) == true)
                            {
                                a.UDFOption = objCommonDAL.GetEncryptValue(a.UDFOption);
                            }
                            EditUDFOption(u.ID, u.UDFID, a.UDFOption, objDTO.UDFTableName, 0, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false, u.UDFOption.Trim(), objDTO.UDFColumnName.Trim());
                        }
                    }
                }
                if (!isExists)
                {

                    DeleteUDFOption(u.ID, objDTO.UDFTableName, (!objDTO.OtherFromeTurns) ?? false, (string.IsNullOrEmpty(u.UDFOption) ? string.Empty : u.UDFOption.Trim()), objDTO.UDFColumnName.Trim());
                }
            }

            foreach (UDFOptionsDTO a in NewString)
            {
                if (a.ID == 0)
                {
                    var obj = InsertUDFOption(objDTO.ID, a.UDFOption, objDTO.UDFTableName, 0, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false, objDTO.UDFColumnName.Trim());

                    if (obj != null && obj.Data != null)
                    {
                        long udfOptionId = 0;
                        long.TryParse((obj.Data.ToString().Replace("Response", "").Replace("{", "").Replace("}", "").Replace("=", "").Replace("duplicate", "").Trim()), out udfOptionId);
                        a.ID = udfOptionId;
                    }
                    a.UDFID = objDTO.ID;
                }
            }

            if (objDTO.ID > 0 && objDTO.UDFTableName.ToLower() == "pullmaster")
            {
                // NewString = JsonConvert.DeserializeObject<List<UDFOptionsDTO>>(t);
                UDFDAL objUDFDAL = new UDFDAL(SessionHelper.EnterPriseDBName);
                UDFDTO objUDFDTO = new UDFDTO();
                objUDFDTO = objUDFDAL.GetUDFByIDPlain(objDTO.ID);
                if (objUDFDTO != null && (objDTO.IsEncryption ?? false) != (objUDFDTO.IsEncryption ?? false) && objDTO.IsEncryption == true)
                {

                    foreach (UDFOptionsDTO u in NewString)
                    {
                        if (u.ID > 0)
                        {
                            //Update in UDfoption Table   
                            string OldDecryptValue = u.UDFOption;
                            u.UDFOption = objCommonDAL.GetEncryptValue(u.UDFOption);
                            EditUDFOption(u.ID, u.UDFID, u.UDFOption, objDTO.UDFTableName, 0, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                        }
                        //Update Relevant Table Data

                    }
                    PullMasterDAL objPullMaster = new PullMasterDAL(SessionHelper.EnterPriseDBName);
                    //objUdf.UDFGeneralDataSave(OldDecryptValue, u.UDFOption, objDTO.UDFTableName, objDTO.UDFColumnName, SessionHelper.RoomID, SessionHelper.CompanyID);
                    objPullMaster.UpdateUDF(objDTO.UDFColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.IsEncryption ?? false);
                    //if (objUDFDTO.UDFControlType == "TextBox")
                    //{
                    //    UDFDAL objUdf = new UDFDAL(SessionHelper.EnterPriseDBName);
                    //    objUdf.UDFGeneralDataEncryptTextSave(objDTO.UDFTableName, objDTO.UDFColumnName, SessionHelper.RoomID, SessionHelper.CompanyID);

                    //}
                }
                else if (objUDFDTO != null && (objDTO.IsEncryption ?? false) != (objUDFDTO.IsEncryption ?? false) && objDTO.IsEncryption == false)
                {
                    foreach (UDFOptionsDTO u in NewString)
                    {
                        //Update in UDfoption Table   
                        if (u.ID > 0)
                        {
                            EditUDFOption(u.ID, u.UDFID, u.UDFOption, objDTO.UDFTableName, 0, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                        }
                        //   string OldEncryptValue = objCommonDAL.GetEncryptValue(u.UDFOption);
                    }
                    PullMasterDAL objPullMaster = new PullMasterDAL(SessionHelper.EnterPriseDBName);
                    objPullMaster.UpdateUDF(objDTO.UDFColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.IsEncryption ?? false);
                    //if (objUDFDTO.UDFControlType == "TextBox")
                    //{
                    //    UDFDAL objUdf = new UDFDAL(SessionHelper.EnterPriseDBName);
                    //    objUdf.UDFGeneralDataDecryptTextSave(objDTO.UDFTableName, objDTO.UDFColumnName, SessionHelper.RoomID, SessionHelper.CompanyID);

                    //}
                }
            }
            //UDFApiController obj = new UDFApiController();
            if (objDTO.UDFTableName.ToLower() == "enterprise")
            {
                eTurnsMaster.DAL.UDFDAL obj = new eTurnsMaster.DAL.UDFDAL();

                if (string.IsNullOrEmpty(objDTO.UDFColumnName))
                {
                    message = ResUDFSetup.UDFColumnNameRequired; 
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(objDTO.UDFDisplayColumnName))
                {
                    message = ResUDFSetup.UDFDisplayColumnNameRequired;
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
                if (objDTO.showPDAField && string.IsNullOrEmpty(objDTO.UDFPDADisplayColumnName) && ResourcePageId > 0)
                {
                    message = ResUDFSetup.UDFPDADisplayColumnNameRequired;
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }

                //if (objDTO.UDFControlType != "Dropdown" && (objDTO.UDFMaxLength.GetValueOrDefault(0) > iUDFMaxLength || objDTO.UDFMaxLength.GetValueOrDefault(0) == 0))
                //{
                //    message = "UDFMaxLength cannot more then " + iUDFMaxLength + " and must be greater than Zero.";
                //    status = "fail";
                //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                //}

                objDTO.UDFDefaultValue = CommonUtility.htmlEscape(objDTO.UDFDefaultValue);
                if (objDTO.ID == 0)
                {
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.GUID = Guid.NewGuid();

                    long ReturnVal = 0;
                    if ((!objDTO.OtherFromeTurns) ?? true)
                        ReturnVal = obj.Insert(objDTO);
                    else
                        ReturnVal = obj.InserteTurns(objDTO);

                    eTurns.DTO.Resources.ResourceHelper resHelper = new eTurns.DTO.Resources.ResourceHelper();
                    eTurns.DTO.Resources.ResourceModuleHelper resModuleHelper = new eTurns.DTO.Resources.ResourceModuleHelper();
                    string culter = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);

                    resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, (!objDTO.OtherFromeTurns) ?? false);
                    resModuleHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, (!objDTO.OtherFromeTurns) ?? false);
                    UDFDAL objUDF = new UDFDAL(SessionHelper.EnterPriseDBName);
                    List<string> ReportResourceFileNameList = objUDF.GetReportResourceFilesByPageResourceFile(ResFileName);
                    if ((objDTO.OtherFromeTurns) ?? true)
                    {
                        if (ReportResourceFileNameList != null && ReportResourceFileNameList.Count() > 0)
                        {
                            foreach (string ReportResourceFileName in ReportResourceFileNameList)
                            {
                                if (ReportResourceFileName == "Res_RPT_Tools" && ResFileName == "ResToolCheckInOutHistory")
                                {
                                    objDTO.UDFColumnName = "ToolCheckOut" + objDTO.UDFColumnName;
                                }
                                if ((ReportResourceFileName == "ResReportWorkOrder" || ReportResourceFileName == "ResReportRequisition") && ResFileName == "ResPullMaster")
                                {
                                    objDTO.UDFColumnName = "Pull" + objDTO.UDFColumnName;
                                }

                                if ((ReportResourceFileName == "ResReportItem" ||
                                    ReportResourceFileName == "Res_RPT_InStock" ||
                                    ReportResourceFileName == "RES_RPT_ReqWithItems" ||
                                    ReportResourceFileName == "RES_RPT_PullMaster" ||
                                    ReportResourceFileName == "Res_RPT_SuggestedOrders" ||
                                    ReportResourceFileName == "Res_RPT_ProjectSpend" ||
                                    ReportResourceFileName == "Res_RPT_eVMI_Usage" ||
                                    ReportResourceFileName == "Res_RPT_eVMI_Usage_ManualCount" ||
                                    ReportResourceFileName == "Res_RPT_CountMaster" || ReportResourceFileName == "ResReportOrder" ||
                                        ReportResourceFileName == "RES_RPT_ItemList" || ReportResourceFileName == "ResReportRequisition"
                                        || ReportResourceFileName == "Res_RPT_Transfer"
                                        || ReportResourceFileName == "ResReportQuote")
                                    && ResFileName == "ResItemMaster")
                                {
                                    objDTO.UDFColumnName = "Item" + objDTO.UDFColumnName;
                                }
                                if (ReportResourceFileName == "RES_RPT_ItemReceived" && ResFileName == "ResItemMaster")
                                {
                                    objDTO.UDFColumnName = "Received" + objDTO.UDFColumnName;
                                }
                                ResourceUtils.GetResource(ReportResourceFileName, objDTO.UDFColumnName, true, true, objDTO.SetUpForEnterpriseLevel ?? false);
                                resHelper.SaveResourcesByKey(ReportResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, true, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                                resModuleHelper.SaveResourcesByKey(ReportResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, true, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                                objDTO.UDFColumnName = objDTO.UDFColumnName.Replace("ToolCheckOut", "").Replace("Item", "").Replace("Received", "");
                                if (ResFileName == "ResPullMaster")
                                {
                                    objDTO.UDFColumnName = objDTO.UDFColumnName.Replace("Pull", "");
                                }
                                string strFilePathWithFileName = resHelper.getFilePath(ReportResourceFileName, true);
                                string actulFilename = ReportResourceFileName + "." + culter + ".resx";
                                if (System.IO.File.Exists(strFilePathWithFileName))
                                {
                                    System.IO.File.Delete(strFilePathWithFileName);
                                }
                                if (culter.ToLower() == "en-us")
                                    culter = "";
                                else
                                    culter = "." + culter;
                                string strFilePath = strFilePathWithFileName.Replace(actulFilename, ResFileName + culter + ".resx");
                                string FileTobeCopied = strFilePathWithFileName.Replace(ReportResourceFileName, ResFileName);
                                System.IO.File.Copy(FileTobeCopied, strFilePathWithFileName);

                            }
                        }
                    }
                    if (ResourcePageId > 0)
                    {
                        int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                        MobileResourcesDAL objMobileResourcesDAL;
                        if ((objDTO.OtherFromeTurns) ?? true)
                            objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                        else
                            objMobileResourcesDAL = new MobileResourcesDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                        List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(RoomId, companyId, languageId, ResourcePageId).ToList();
                        long UDfId = 0;

                        if (MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                        {
                            UDfId = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ID;
                        }


                        if (UDfId > 0)
                        {
                            MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();
                            mobileResourceDTO = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault();


                            mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;

                            mobileResourceDTO.Roomid = RoomId;

                            mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                            mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                            objMobileResourcesDAL.Edit(mobileResourceDTO);
                        }
                        else
                        {
                            MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();

                            mobileResourceDTO.Roomid = RoomId;

                            mobileResourceDTO.ResourceKey = objDTO.UDFColumnName;
                            mobileResourceDTO.ResourcePageID = ResourcePageId;
                            mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                            mobileResourceDTO.LanguageID = 1;
                            mobileResourceDTO.CompanyID = companyId;

                            mobileResourceDTO.CreatedBy = SessionHelper.UserID;
                            mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                            mobileResourceDTO.CreatedOn = DateTimeUtility.DateTimeNow;
                            mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                            mobileResourceDTO.LanguageID = languageId;
                            long ReturnmobileResourceVal = objMobileResourcesDAL.Insert(mobileResourceDTO);

                        }
                    }
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage;
                        status = "ok";
                        ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                        string SessinKey = "Res_" + ResourceHelper.CompanyResourceFolder + "_" + ResFileName + ResourceHelper.CurrentCult;

                        System.Web.HttpContext.Current.Cache.Remove(SessinKey);
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                        status = "fail";
                    }
                }
                else
                {
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.UDFDefaultValue = CommonUtility.htmlEscape(objDTO.UDFDefaultValue);
                    bool ReturnVal = false;
                    if (objDTO.OtherFromeTurns ?? true)
                    {
                        ReturnVal = obj.Edit(objDTO);
                    }
                    else
                    {
                        UDFDAL objeTurns = new UDFDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                        ReturnVal = objeTurns.Edit(objDTO);
                    }
                    try
                    {
                        eTurns.DTO.Resources.ResourceHelper resHelper = new eTurns.DTO.Resources.ResourceHelper();
                        eTurns.DTO.Resources.ResourceModuleHelper resModuleHelper = new eTurns.DTO.Resources.ResourceModuleHelper();
                        string culter = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                        //if (objDTO != null && objDTO.IsDeleted)
                        //{
                        //    objDTO.UDFDisplayColumnName = objDTO.UDFColumnName;//if udf is deleted then reset values
                        //}
                        resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, (!objDTO.OtherFromeTurns) ?? false);
                        resModuleHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, (!objDTO.OtherFromeTurns) ?? false);
                        UDFDAL objUDF = new UDFDAL(SessionHelper.EnterPriseDBName);
                        List<string> ReportResourceFileNameList = objUDF.GetReportResourceFilesByPageResourceFile(ResFileName);
                        if ((objDTO.OtherFromeTurns) ?? true)
                        {
                            if (ReportResourceFileNameList != null && ReportResourceFileNameList.Count() > 0)
                            {
                                foreach (string ReportResourceFileName in ReportResourceFileNameList)
                                {
                                    ResourceUtils.GetResource(ReportResourceFileName, objDTO.UDFColumnName, true, true, objDTO.SetUpForEnterpriseLevel ?? false);
                                    resHelper.SaveResourcesByKey(ReportResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, (!objDTO.OtherFromeTurns) ?? false);
                                    resModuleHelper.SaveResourcesByKey(ReportResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, (!objDTO.OtherFromeTurns) ?? false);
                                    string strFilePathWithFileName = resHelper.getFilePath(ReportResourceFileName, true);
                                    string actulFilename = ReportResourceFileName + "." + culter + ".resx";
                                    if (System.IO.File.Exists(strFilePathWithFileName))
                                    {
                                        System.IO.File.Delete(strFilePathWithFileName);
                                    }
                                    if (culter.ToLower() == "en-us")
                                        culter = "";
                                    else
                                        culter = "." + culter;
                                    string strFilePath = strFilePathWithFileName.Replace(actulFilename, ResFileName + culter + ".resx");
                                    string FileTobeCopied = strFilePathWithFileName.Replace(ReportResourceFileName, ResFileName);
                                    System.IO.File.Copy(FileTobeCopied, strFilePathWithFileName);
                                }
                            }
                        }
                        ResourcePageId = SessionHelper.GetUDFListID(objDTO.UDFTableName);
                        if (ResourcePageId > 0)
                        {
                            int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                            MobileResourcesDAL objMobileResourcesDAL;
                            if ((objDTO.OtherFromeTurns) ?? true)
                                objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                            else
                                objMobileResourcesDAL = new MobileResourcesDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                            List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(RoomId, companyId, languageId, ResourcePageId).ToList();
                            long UDfId = 0;

                            if (MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                            {
                                UDfId = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ID;
                            }


                            if (UDfId > 0)
                            {
                                MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();

                                mobileResourceDTO = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault();

                                mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;

                                mobileResourceDTO.Roomid = RoomId;

                                mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                                mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                                objMobileResourcesDAL.Edit(mobileResourceDTO);
                            }
                            else
                            {
                                MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();

                                mobileResourceDTO.Roomid = RoomId;

                                mobileResourceDTO.ResourceKey = objDTO.UDFColumnName;
                                mobileResourceDTO.ResourcePageID = ResourcePageId;
                                mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                                mobileResourceDTO.LanguageID = 1;

                                mobileResourceDTO.CompanyID = companyId;

                                mobileResourceDTO.CreatedBy = SessionHelper.UserID;
                                mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                                mobileResourceDTO.CreatedOn = DateTimeUtility.DateTimeNow;
                                mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;

                                long ReturnmobileResourceVal = objMobileResourcesDAL.Insert(mobileResourceDTO);

                            }
                        }
                    }
                    catch (Exception)
                    {
                    }

                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage;
                        status = "ok";
                        string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                        string SessinKey = "Res_" + ResourceHelper.CompanyResourceFolder + "_" + ResFileName + ResourceHelper.CurrentCult;

                        System.Web.HttpContext.Current.Cache.Remove(SessinKey);

                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                        status = "fail";
                    }
                }
            }
            else if (objDTO.UDFTableName.ToLower() == "usermaster")
            {
                objEnterPriseDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                lstEnterpriseList = objEnterPriseDAL.GetAllEnterpriseForExecutionWithDemo();
                foreach (EnterpriseDTO objEnt in lstEnterpriseList)
                {

                    UDFOptionDAL objUdfOptionDAL = new UDFOptionDAL(objEnt.EnterpriseDBName);
                    UDFDTO objudfDTO = new UDFDTO();
                    UDFDAL objUDfDAL = new UDFDAL(objEnt.EnterpriseDBName);
                    Int64 UDFID = 0;
                    var tmpudf = objUDfDAL.GetUDFByCompanyAndColumnNamePlain(objDTO.UDFColumnName, "UserMaster", 0);

                    if (tmpudf != null && tmpudf.ID > 0)
                    {
                        objudfDTO = tmpudf;
                        UDFID = objudfDTO.ID;
                    }
                    objDTO.ID = UDFID;
                    UDFDAL obj = new UDFDAL(objEnt.EnterpriseDBName);

                    if (string.IsNullOrEmpty(objDTO.UDFColumnName))
                    {
                        message = ResUDFSetup.UDFColumnNameRequired;
                        status = "fail";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }

                    if (string.IsNullOrEmpty(objDTO.UDFDisplayColumnName))
                    {
                        message = ResUDFSetup.UDFDisplayColumnNameRequired;
                        status = "fail";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                    if (objDTO.showPDAField && string.IsNullOrEmpty(objDTO.UDFPDADisplayColumnName) && ResourcePageId > 0)
                    {
                        message = ResUDFSetup.UDFPDADisplayColumnNameRequired;
                        status = "fail";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }


                    //if (objDTO.UDFControlType != "Dropdown" && (objDTO.UDFMaxLength.GetValueOrDefault(0) > iUDFMaxLength || objDTO.UDFMaxLength.GetValueOrDefault(0) == 0))
                    //{
                    //    message = "UDFMaxLength cannot more then " + iUDFMaxLength + " and must be greater than Zero.";
                    //    status = "fail";
                    //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    //}

                    bool isUDFName = false;


                    if (objDTO.ID == 0)
                    {
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.UDFDefaultValue = CommonUtility.htmlEscape(objDTO.UDFDefaultValue);
                        long ReturnVal = obj.Insert(objDTO);

                        eTurns.DTO.Resources.ResourceHelper resHelper = new eTurns.DTO.Resources.ResourceHelper();
                        eTurns.DTO.Resources.ResourceModuleHelper resModuleHelper = new eTurns.DTO.Resources.ResourceModuleHelper();
                        string culter = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);

                        resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, isUDFName, (!objDTO.OtherFromeTurns) ?? false);
                        resModuleHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, isUDFName, (!objDTO.OtherFromeTurns) ?? false);

                        UDFDAL objUDF = new UDFDAL(objEnt.EnterpriseDBName);

                        Dictionary<int, string> ReportResourceFileNameList = objUDF.getReportResourceFileNameWithPrefix(ResFileName);

                        if (ReportResourceFileNameList != null && ReportResourceFileNameList.Count() > 0)
                        {
                            foreach (KeyValuePair<int, string> ReportResourceFileName in ReportResourceFileNameList)
                            {
                                string ResourceFileName = ReportResourceFileName.Value.Split('$')[0];
                                string Prefix = ReportResourceFileName.Value.Split('$')[1];

                                objDTO.UDFColumnName = Prefix + objDTO.UDFColumnName;

                                string val = ResourceUtils.GetResource(ResourceFileName, objDTO.UDFColumnName, true, true, objDTO.SetUpForEnterpriseLevel ?? false);
                                resHelper.SaveResourcesByKey(ResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, (!objDTO.OtherFromeTurns) ?? false, true);
                                resModuleHelper.SaveResourcesByKey(ResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, (!objDTO.OtherFromeTurns) ?? false, true);
                                if (!string.IsNullOrWhiteSpace(Prefix))
                                {
                                    objDTO.UDFColumnName = objDTO.UDFColumnName.Replace(Prefix, "");
                                }

                            }
                        }

                        ResourcePageId = SessionHelper.GetUDFListID(objDTO.UDFTableName);
                        if (ResourcePageId > 0)
                        {
                            int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                            MobileResourcesDAL objMobileResourcesDAL = new MobileResourcesDAL(objEnt.EnterpriseDBName);


                            List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(RoomId, companyId, languageId, ResourcePageId).ToList();
                            long UDfId = 0;

                            if (MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                            {
                                UDfId = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ID;
                            }

                            if (UDfId > 0)
                            {
                                MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();

                                mobileResourceDTO = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault();
                                mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;

                                mobileResourceDTO.Roomid = RoomId;

                                mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                                mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                                objMobileResourcesDAL.Edit(mobileResourceDTO);
                            }
                            else
                            {
                                MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();

                                mobileResourceDTO.Roomid = RoomId;
                                mobileResourceDTO.ResourceKey = objDTO.UDFColumnName;
                                mobileResourceDTO.ResourcePageID = ResourcePageId;
                                mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                                mobileResourceDTO.LanguageID = 1;

                                mobileResourceDTO.CompanyID = companyId;

                                mobileResourceDTO.CreatedBy = SessionHelper.UserID;
                                mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                                mobileResourceDTO.CreatedOn = DateTimeUtility.DateTimeNow;
                                mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;

                                long ReturnmobileResourceVal = objMobileResourcesDAL.Insert(mobileResourceDTO);
                            }
                        }
                        if (ReturnVal > 0)
                        {
                            message = ResMessage.SaveMessage;
                            status = "ok";
                            ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                            string SessinKey = "Res_" + ResourceHelper.CompanyResourceFolder + "_" + ResFileName + ResourceHelper.CurrentCult;

                            System.Web.HttpContext.Current.Cache.Remove(SessinKey);
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                            status = "fail";
                        }
                    }
                    else
                    {
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.UDFDefaultValue = CommonUtility.htmlEscape(objDTO.UDFDefaultValue);
                        bool ReturnVal = obj.Edit(objDTO);
                        Int64 EnterPriseId = 0;
                        if (objEnt.EnterpriseDBName.ToLower() != "eturns" && objEnt.EnterpriseDBName.ToLower() != "eturnsdemo")
                        {
                            EnterPriseId = Convert.ToInt64(objEnt.EnterpriseDBName.Split('_')[0]);
                        }
                        try
                        {
                            eTurns.DTO.Resources.ResourceHelper resHelper = new eTurns.DTO.Resources.ResourceHelper();
                            eTurns.DTO.Resources.ResourceModuleHelper resModuleHelper = new eTurns.DTO.Resources.ResourceModuleHelper();
                            string culter = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                            string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                            //if (objDTO != null && objDTO.IsDeleted)
                            //{
                            //    objDTO.UDFDisplayColumnName = objDTO.UDFColumnName;//if udf is deleted then reset values
                            //}
                            if (objEnt.EnterpriseDBName.ToLower() == "eturns" || objEnt.EnterpriseDBName.ToLower() == "eturnsdemo")
                            {
                                string valNew = ResourceUtils.GetResource(ResFileName, objDTO.UDFColumnName, isUDFName, false, objDTO.SetUpForEnterpriseLevel ?? false);
                            }
                            else
                            {
                                string valNew1 = ResourceHelper.GetUserMasterResourceValue(objDTO.UDFColumnName, ResFileName, isUDFName, false, objDTO.SetUpForEnterpriseLevel ?? false, false, EnterPriseId);

                            }
                            //resHelper.GetEnterPriseResourceValue(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, isUDFName, true, objDTO.SetUpForEnterpriseLevel ?? false);
                            List<KeyValDTO> objListKey = new List<KeyValDTO>();
                            KeyValDTO objKeyValue = new KeyValDTO();
                            objKeyValue.key = objDTO.UDFColumnName;
                            objKeyValue.value = objDTO.UDFDisplayColumnName;
                            objListKey.Add(objKeyValue);

                            if (objEnt.EnterpriseDBName.ToLower() != "eturns" && objEnt.EnterpriseDBName.ToLower() != "eturnsdemo" && EnterPriseId != Convert.ToInt64(SessionHelper.EnterPriceID))
                            {
                                resHelper.SaveUserMasterResources(ResFileName, culter, objListKey, EnterPriseId);
                                resModuleHelper.SaveUserMasterResources(ResFileName, culter, objListKey, EnterPriseId);
                            }
                            else if (EnterPriseId == Convert.ToInt64(SessionHelper.EnterPriceID))
                            {
                                resHelper.SaveEnterpriseResources(ResFileName, culter, objListKey);
                                resModuleHelper.SaveEnterpriseResources(ResFileName, culter, objListKey);
                            }
                            else
                            {
                                resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, isUDFName, true, objDTO.SetUpForEnterpriseLevel ?? false);
                                resModuleHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, isUDFName, true, objDTO.SetUpForEnterpriseLevel ?? false);
                            }
                            UDFDAL objUDF = new UDFDAL(objEnt.EnterpriseDBName);
                            Dictionary<int, string> ReportResourceFileNameList = objUDF.getReportResourceFileNameWithPrefix(ResFileName);
                            if ((objDTO.OtherFromeTurns) ?? true)
                            {
                                if (ReportResourceFileNameList != null && ReportResourceFileNameList.Count() > 0)
                                {
                                    List<KeyValDTO> objListKeyNew = new List<KeyValDTO>();
                                    KeyValDTO objKeyValueNew = new KeyValDTO();
                                    objKeyValueNew.key = objDTO.UDFColumnName;
                                    objKeyValueNew.value = objDTO.UDFDisplayColumnName;
                                    objListKeyNew.Add(objKeyValueNew);
                                    foreach (KeyValuePair<int, string> ReportResourceFileName in ReportResourceFileNameList)
                                    {
                                        string ResourceFileName = ReportResourceFileName.Value.Split('$')[0];
                                        string Prefix = ReportResourceFileName.Value.Split('$')[1];

                                        objDTO.UDFColumnName = Prefix + objDTO.UDFColumnName;
                                        if (objEnt.EnterpriseDBName.ToLower() != "eturns" && objEnt.EnterpriseDBName.ToLower() != "eturnsdemo")
                                        {
                                            // string val = ResourceHelper.GetUserMasterResourceValue(ResourceFileName, objDTO.UDFColumnName, false, true, objDTO.SetUpForEnterpriseLevel ?? false,);
                                            string val = ResourceHelper.GetUserMasterResourceValue(objDTO.UDFColumnName, ResourceFileName, false, true, objDTO.SetUpForEnterpriseLevel ?? false, false, EnterPriseId);
                                            //string valNew1 = ResourceHelper.GetUserMasterResourceValue(objDTO.UDFColumnName, ResFileName, isUDFName, false, objDTO.SetUpForEnterpriseLevel ?? false, false, EnterPriseId);
                                            resHelper.SaveUserMasterResources(ResourceFileName, culter, objListKeyNew, EnterPriseId);
                                            resModuleHelper.SaveUserMasterResources(ResourceFileName, culter, objListKeyNew, EnterPriseId);
                                        }
                                        else
                                        {
                                            string val = ResourceUtils.GetResource(ResourceFileName, objDTO.UDFColumnName, false, false, objDTO.SetUpForEnterpriseLevel ?? false);
                                            resHelper.SaveResourcesByKey(ResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, true, objDTO.SetUpForEnterpriseLevel ?? false);
                                            resModuleHelper.SaveResourcesByKey(ResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, true, objDTO.SetUpForEnterpriseLevel ?? false);
                                        }
                                        if (!string.IsNullOrWhiteSpace(Prefix))
                                        {
                                            objDTO.UDFColumnName = objDTO.UDFColumnName.Replace(Prefix, "");
                                        }
                                    }
                                }
                            }

                            ResourcePageId = SessionHelper.GetUDFListID(objDTO.UDFTableName);
                            if (ResourcePageId > 0)
                            {
                                int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                                MobileResourcesDAL objMobileResourcesDAL = new MobileResourcesDAL(objEnt.EnterpriseDBName);

                                List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(RoomId, companyId, languageId, ResourcePageId).ToList();
                                long UDfId = 0;

                                if (MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                                {
                                    UDfId = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ID;
                                }


                                if (UDfId > 0)
                                {
                                    MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();

                                    mobileResourceDTO = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault();

                                    mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;

                                    mobileResourceDTO.Roomid = RoomId;
                                    mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                                    mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                                    objMobileResourcesDAL.Edit(mobileResourceDTO);
                                }
                                else
                                {
                                    MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();

                                    mobileResourceDTO.Roomid = RoomId;

                                    mobileResourceDTO.ResourceKey = objDTO.UDFColumnName;
                                    mobileResourceDTO.ResourcePageID = ResourcePageId;
                                    mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                                    mobileResourceDTO.LanguageID = 1;

                                    mobileResourceDTO.CompanyID = companyId;

                                    mobileResourceDTO.CreatedBy = SessionHelper.UserID;
                                    mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                                    mobileResourceDTO.CreatedOn = DateTimeUtility.DateTimeNow;
                                    mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                                    mobileResourceDTO.LanguageID = languageId;
                                    long ReturnmobileResourceVal = objMobileResourcesDAL.Insert(mobileResourceDTO);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }

                        if (ReturnVal)
                        {
                            message = ResMessage.SaveMessage;
                            status = "ok";

                            string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                            string SessinKey = "Res_" + ResourceHelper.CompanyResourceFolder + "_" + ResFileName + ResourceHelper.CurrentCult;
                            System.Web.HttpContext.Current.Cache.Remove(SessinKey);
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                            status = "fail";
                        }
                    }
                }
            }
            else
            {


                UDFDAL obj;
                if ((objDTO.OtherFromeTurns) ?? true)
                    obj = new UDFDAL(SessionHelper.EnterPriseDBName);
                else
                    obj = new UDFDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());

                if (string.IsNullOrEmpty(objDTO.UDFColumnName))
                {
                    message = ResUDFSetup.UDFColumnNameRequired;
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(objDTO.UDFDisplayColumnName))
                {
                    message = ResUDFSetup.UDFDisplayColumnNameRequired;
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
                if (objDTO.showPDAField && string.IsNullOrEmpty(objDTO.UDFPDADisplayColumnName) && ResourcePageId > 0)
                {
                    message = ResUDFSetup.UDFPDADisplayColumnNameRequired;
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }

                //if (objDTO.UDFControlType != "Dropdown" && (objDTO.UDFMaxLength.GetValueOrDefault(0) > iUDFMaxLength || objDTO.UDFMaxLength.GetValueOrDefault(0) == 0))
                //{
                //    message = "UDFMaxLength cannot more then "+iUDFMaxLength+ " and must be greater than Zero.";
                //    status = "fail";
                //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                //}

                bool isUDFName = false;
                if (objDTO.UDFTableName.ToLower() != "companymaster" && objDTO.UDFTableName.ToLower() != "room")
                {
                    isUDFName = true;
                }

                if (objDTO.ID == 0)
                {
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.UDFDefaultValue = CommonUtility.htmlEscape(objDTO.UDFDefaultValue);
                    long ReturnVal = obj.Insert(objDTO);

                    eTurns.DTO.Resources.ResourceHelper resHelper = new eTurns.DTO.Resources.ResourceHelper();
                    eTurns.DTO.Resources.ResourceModuleHelper resModuleHelper = new eTurns.DTO.Resources.ResourceModuleHelper();
                    string culter = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);


                    if (!objDTO.UDFTableName.ToLower().Contains("bom"))
                    {
                        resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, isUDFName, (!objDTO.OtherFromeTurns) ?? false);
                        resModuleHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, isUDFName, (!objDTO.OtherFromeTurns) ?? false);
                    }
                    else
                    {
                        resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, (!objDTO.OtherFromeTurns) ?? false);
                        resModuleHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, (!objDTO.OtherFromeTurns) ?? false);
                    }
                    UDFDAL objUDF;
                    if ((objDTO.OtherFromeTurns) ?? true)
                        objUDF = new UDFDAL(SessionHelper.EnterPriseDBName);
                    else
                        objUDF = new UDFDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                    List<string> ReportResourceFileNameList = objUDF.GetReportResourceFilesByPageResourceFile(ResFileName);
                    if ((objDTO.OtherFromeTurns) ?? true)
                    {
                        if (ReportResourceFileNameList != null && ReportResourceFileNameList.Count() > 0)
                        {
                            foreach (string ReportResourceFileName in ReportResourceFileNameList)
                            {
                                if (ReportResourceFileName == "Res_RPT_Tools" && ResFileName == "ResToolCheckInOutHistory")
                                {
                                    objDTO.UDFColumnName = "ToolCheckOut" + objDTO.UDFColumnName;
                                }
                                if ((ReportResourceFileName == "ResReportWorkOrder" || ReportResourceFileName == "ResReportRequisition") && ResFileName == "ResPullMaster")
                                {
                                    objDTO.UDFColumnName = "Pull" + objDTO.UDFColumnName;
                                }

                                if ((ReportResourceFileName == "ResReportItem" ||
                                    ReportResourceFileName == "Res_RPT_InStock" ||
                                    ReportResourceFileName == "RES_RPT_ReqWithItems" ||
                                    ReportResourceFileName == "RES_RPT_PullMaster" ||
                                    ReportResourceFileName == "Res_RPT_SuggestedOrders" ||
                                    ReportResourceFileName == "Res_RPT_ProjectSpend" ||
                                    ReportResourceFileName == "Res_RPT_eVMI_Usage" ||
                                    ReportResourceFileName == "Res_RPT_eVMI_Usage_ManualCount" ||
                                    ReportResourceFileName == "Res_RPT_CountMaster" || ReportResourceFileName == "ResReportOrder" ||
                                        ReportResourceFileName == "RES_RPT_ItemList" || ReportResourceFileName == "ResReportRequisition"
                                        || ReportResourceFileName == "Res_RPT_Transfer"
                                        || ReportResourceFileName == "ResReportQuote"
                                        )
                                    && ResFileName == "ResItemMaster")
                                {
                                    objDTO.UDFColumnName = "Item" + objDTO.UDFColumnName;
                                }
                                if (ReportResourceFileName == "RES_RPT_ItemReceived" && ResFileName == "ResItemMaster")
                                {
                                    objDTO.UDFColumnName = "Received" + objDTO.UDFColumnName;
                                }
                                ResourceUtils.GetResource(ReportResourceFileName, objDTO.UDFColumnName, true, true, objDTO.SetUpForEnterpriseLevel ?? false);
                                resHelper.SaveResourcesByKey(ReportResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, true, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                                resModuleHelper.SaveResourcesByKey(ReportResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, true, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                                objDTO.UDFColumnName = objDTO.UDFColumnName.Replace("ToolCheckOut", "").Replace("Item", "").Replace("Received", "");
                                if (ResFileName == "ResPullMaster")
                                {
                                    objDTO.UDFColumnName = objDTO.UDFColumnName.Replace("Pull", "");
                                }
                                string strFilePathWithFileName = resHelper.getFilePath(ReportResourceFileName, true);
                                string actulFilename = ReportResourceFileName + "." + culter + ".resx";
                                if (System.IO.File.Exists(strFilePathWithFileName))
                                {
                                    System.IO.File.Delete(strFilePathWithFileName);
                                }
                                if (culter.ToLower() == "en-us")
                                    culter = "";
                                else
                                    culter = "." + culter;
                                string strFilePath = strFilePathWithFileName.Replace(actulFilename, ResFileName + culter + ".resx");
                                string FileTobeCopied = strFilePathWithFileName.Replace(ReportResourceFileName, ResFileName);
                                System.IO.File.Copy(FileTobeCopied, strFilePathWithFileName);
                            }
                        }
                    }
                    ResourcePageId = SessionHelper.GetUDFListID(objDTO.UDFTableName);
                    if (ResourcePageId > 0)
                    {
                        int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                        MobileResourcesDAL objMobileResourcesDAL;
                        if ((objDTO.OtherFromeTurns) ?? true)
                            objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                        else
                            objMobileResourcesDAL = new MobileResourcesDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());

                        List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(RoomId, companyId, languageId, ResourcePageId).ToList();
                        long UDfId = 0;

                        if (MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                        {
                            UDfId = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ID;
                        }

                        if (UDfId > 0)
                        {
                            MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();

                            mobileResourceDTO = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault();
                            mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;

                            mobileResourceDTO.Roomid = RoomId;

                            mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                            mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                            objMobileResourcesDAL.Edit(mobileResourceDTO);
                        }
                        else
                        {
                            MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();

                            mobileResourceDTO.Roomid = RoomId;
                            mobileResourceDTO.ResourceKey = objDTO.UDFColumnName;
                            mobileResourceDTO.ResourcePageID = ResourcePageId;
                            mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                            mobileResourceDTO.LanguageID = 1;

                            mobileResourceDTO.CompanyID = companyId;

                            mobileResourceDTO.CreatedBy = SessionHelper.UserID;
                            mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                            mobileResourceDTO.CreatedOn = DateTimeUtility.DateTimeNow;
                            mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;

                            long ReturnmobileResourceVal = objMobileResourcesDAL.Insert(mobileResourceDTO);
                        }
                    }
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage;
                        status = "ok";
                        ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                        string SessinKey = "Res_" + ResourceHelper.CompanyResourceFolder + "_" + ResFileName + ResourceHelper.CurrentCult;

                        System.Web.HttpContext.Current.Cache.Remove(SessinKey);

                        
                        string SessinKeyDB = "ResDB_" + SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID + "_" + ResFileName + (ResourceHelper.CurrentCult.Name=="en-US"?"":ResourceHelper.CurrentCult.Name);
                        AngularApp.AppConfigController.InvalidateCache(SessinKeyDB);
                        if (System.Web.HttpContext.Current.Cache.Get(SessinKeyDB) != null)
                            System.Web.HttpContext.Current.Cache.Remove(SessinKeyDB);

                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                        status = "fail";
                    }
                }
                else
                {
                    IEnumerable<UDFDTO> existingData = null;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.UDFDefaultValue = CommonUtility.htmlEscape(objDTO.UDFDefaultValue);
                    UDFDTO objOldData = obj.GetUDFByIDPlain(objDTO.ID);
                    bool ReorderNeedsToChange = false;
                    bool RemoveUDFToChange = false;
                    Int64 ExtraCount = 0;
                    Int64 TotalSetupCount = 0;

                    if (objOldData == null)
                    {
                        ReorderNeedsToChange = true;
                    }
                    if (objOldData != null && string.IsNullOrWhiteSpace(objOldData.UDFControlType))
                    {
                        ReorderNeedsToChange = true;
                    }
                    if (objOldData != null && objOldData.UDFControlType == null)
                    {
                        ReorderNeedsToChange = true;
                    }
                    if (objOldData != null && objOldData.IsDeleted == true && objDTO.IsDeleted == false)
                    {
                        ReorderNeedsToChange = true;
                    }
                    if (objOldData != null && objOldData.IsDeleted == false && objDTO.IsDeleted == true)
                    {

                        RemoveUDFToChange = true;
                    }
                    if (ReorderNeedsToChange || RemoveUDFToChange)
                    {
                        IEnumerable<UDFDTO> DataFromDB = null;
                        int TotalRecordCount = 0;
                        UDFDAL obj1 = new UDFDAL(SessionHelper.EnterPriseDBName);
                        DataFromDB = obj1.GetPagedUDFsByUDFTableNamePlain(0, 10, out TotalRecordCount, "ID asc", SessionHelper.CompanyID, objDTO.UDFTableName, SessionHelper.RoomID);
                        existingData = DataFromDB;
                        if (DataFromDB != null && DataFromDB.Count() > 0)
                        {
                            //if (!ReorderNeedsToChange)
                            //{
                            //    ExtraCount = DataFromDB.Where(c => c.ID > objDTO.ID && c.UDFControlType != null && c.IsDeleted == false).Count();
                            //}
                            TotalSetupCount = DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
                        }
                    }
                    bool ReturnVal = obj.Edit(objDTO);
                    try
                    {
                        eTurns.DTO.Resources.ResourceHelper resHelper = new eTurns.DTO.Resources.ResourceHelper();
                        eTurns.DTO.Resources.ResourceModuleHelper resModuleHelper = new eTurns.DTO.Resources.ResourceModuleHelper();
                        string culter = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                        //if (objDTO != null && objDTO.IsDeleted)
                        //{
                        //    objDTO.UDFDisplayColumnName = objDTO.UDFColumnName;//if udf is deleted then reset values
                        //}
                        if (!objDTO.UDFTableName.ToLower().Contains("bom"))
                        {
                            if (ResFileName == "ResRoomMaster")
                            {
                                //save at room level
                                resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, true, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                                //save at company level
                                //resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, isUDFName, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                                //resModuleHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, true, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                                //resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, false, objDTO.SetUpForEnterpriseLevel ?? false);
                            }
                            else
                            {
                                resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, isUDFName, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                                resModuleHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, isUDFName, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                            }
                        }
                        else
                        {
                            resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                            resModuleHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                        }
                        UDFDAL objUDF = new UDFDAL(SessionHelper.EnterPriseDBName);
                        Dictionary<int, string> ReportResourceFileNameList = objUDF.getReportResourceFileNameWithPrefix(ResFileName);
                        if ((objDTO.OtherFromeTurns) ?? true)
                        {
                            if (ReportResourceFileNameList != null && ReportResourceFileNameList.Count() > 0)
                            {
                                foreach (KeyValuePair<int, string> ReportResourceFileName in ReportResourceFileNameList)
                                {
                                    string ResourceFileName = ReportResourceFileName.Value.Split('$')[0];
                                    string Prefix = ReportResourceFileName.Value.Split('$')[1];

                                    objDTO.UDFColumnName = Prefix + objDTO.UDFColumnName;

                                    string val = ResourceUtils.GetResource(ResourceFileName, objDTO.UDFColumnName, true, true, objDTO.SetUpForEnterpriseLevel ?? false);
                                    resHelper.SaveResourcesByKey(ResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, true, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                                    resModuleHelper.SaveResourcesByKey(ResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, true, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false);
                                    if (!string.IsNullOrWhiteSpace(Prefix))
                                    {
                                        objDTO.UDFColumnName = objDTO.UDFColumnName.Replace(Prefix, "");
                                    }

                                }
                            }
                        }

                        ResourcePageId = SessionHelper.GetUDFListID(objDTO.UDFTableName);
                        if (ResourcePageId > 0)
                        {
                            int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                            MobileResourcesDAL objMobileResourcesDAL;
                            if ((objDTO.OtherFromeTurns) ?? true)
                                objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                            else
                                objMobileResourcesDAL = new MobileResourcesDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                            List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(RoomId, companyId, languageId, ResourcePageId).ToList();
                            long UDfId = 0;

                            if (MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                            {
                                UDfId = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ID;
                            }


                            if (UDfId > 0)
                            {
                                MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();

                                mobileResourceDTO = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == companyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault();

                                mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;

                                mobileResourceDTO.Roomid = RoomId;
                                mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                                mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                                objMobileResourcesDAL.Edit(mobileResourceDTO);
                            }
                            else
                            {
                                MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();

                                mobileResourceDTO.Roomid = RoomId;

                                mobileResourceDTO.ResourceKey = objDTO.UDFColumnName;
                                mobileResourceDTO.ResourcePageID = ResourcePageId;
                                mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                                mobileResourceDTO.LanguageID = 1;

                                mobileResourceDTO.CompanyID = companyId;

                                mobileResourceDTO.CreatedBy = SessionHelper.UserID;
                                mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                                mobileResourceDTO.CreatedOn = DateTimeUtility.DateTimeNow;
                                mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                                mobileResourceDTO.LanguageID = languageId;
                                long ReturnmobileResourceVal = objMobileResourcesDAL.Insert(mobileResourceDTO);
                            }
                        }
                    }
                    catch
                    {
                    }

                    if (ReturnVal)
                    {
                        try
                        {
                            if (ReorderNeedsToChange)
                            {
                                /*////////////////CODE FOR SAVE UDF ON GRID ORDER////////////////*/
                                //if (isFirstTimeSave)
                                //{
                                //    if (objDTO.UDFTableName.ToLower() == "pullmaster")
                                //    {
                                //        SaveUDFColumnOrderInGrid("NewPullItemMaster");
                                //        SaveUDFColumnOrderInGrid("NewConsumePullItemMaster");
                                //        SaveUDFColumnOrderInGrid("WorkOrderDetails");
                                //        SaveUDFColumnOrderInGrid("RequisitionDetails");
                                //        SaveUDFColumnOrderInGrid("ItemMasterModelList_RQ");

                                //    }
                                //    if (objDTO.UDFTableName.ToLower() == "toolcheckinouthistory")
                                //    {
                                //        SaveUDFColumnOrderInGrid("CheckinCheckOutList");
                                //        SaveUDFColumnOrderInGrid("RequisitionDetails");
                                //        SaveUDFColumnOrderInGrid("ToolListInModel");
                                //    }
                                //    if (objDTO.UDFTableName.ToLower() == "toolmaster")
                                //    {
                                //        SaveUDFColumnOrderInGrid("ToolListInModel");

                                //    }
                                string ListName = GetListNameFromTable(objDTO.UDFTableName);
                                //SaveUDFColumnOrderInGrid(ListName, ExtraCount);
                                eTurnsMaster.DAL.CommonMasterDAL objUDF = new eTurnsMaster.DAL.CommonMasterDAL();
                                Dictionary<int, string> GridListName = objUDF.getGridResourceListName(objDTO.UDFTableName);

                                if (GridListName != null && GridListName.Count() > 0)
                                {
                                    UDFDAL obj1 = new UDFDAL(SessionHelper.EnterPriseDBName);
                                    foreach (KeyValuePair<int, string> ReportResourceFileName in GridListName)
                                    {
                                        string[] Values = ReportResourceFileName.Value.Split('$');
                                        int currentTableUDFCount = 0;
                                        if (Values != null && Values.Count() > 0)
                                        {
                                            Int64 ExtraUDFinGrid = 0;
                                            if (Values[1] == "Yes")
                                            {
                                                ExtraUDFinGrid = obj1.GetNonDeletedUDFCountByUDFTableName(Values[2], SessionHelper.RoomID, SessionHelper.CompanyID);
                                                //var currentTableUDFCount = obj1.GetNonDeletedUDFCountByUDFTableName(objDTO.UDFTableName, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                
                                                //if (currentTableUDFCount > 0)
                                                //{
                                                //    ExtraUDFinGrid += (currentTableUDFCount - 1);
                                                //}
                                                
                                                if (Values[0].ToLower() == "receiveditemdetailgrid")
                                                {
                                                    ExtraUDFinGrid += obj1.GetNonDeletedUDFCountByUDFTableName("ReceivedOrderTransferDetail", SessionHelper.RoomID, SessionHelper.CompanyID);
                                                }
                                                //DataFromDB = obj1.GetPagedRecords(0, 10, out TotalRecordCount, string.Empty, "ID asc", SessionHelper.CompanyID, Values[2], SessionHelper.RoomID);
                                                //if (DataFromDB != null && DataFromDB.Count() > 0)
                                                //{
                                                //    ExtraUDFinGrid = DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
                                                //}
                                            }
                                            ExtraCount += ExtraUDFinGrid + Convert.ToInt64(Values[3]);

                                            if (ExtraCount > 0)
                                            {
                                                currentTableUDFCount = obj1.GetNonDeletedUDFCountByUDFTableName(objDTO.UDFTableName, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                if (currentTableUDFCount > 0)
                                                {
                                                    currentTableUDFCount -= 1;
                                                }
                                            }

                                            if (objDTO.UDFTableName.ToLower() == "toolmaster" && SessionHelper.AllowToolOrdering)
                                            {
                                                Values[0] = "ToolListNew";
                                            }

                                            SaveUDFColumnOrderInGrid(Values[0], ExtraCount,currentTableUDFCount);
                                            bool isInvalidateCache = false;
                                            switch(Values[0])
                                            {
                                                case "NewPullItemMaster":
                                                    SaveUDFColumnOrderInNewGrid("NewPullItemMaster", ExtraCount);
                                                    isInvalidateCache = true;
                                                    break;
                                                case "OrderMasterList":
                                                    SaveUDFColumnOrderInNewGrid("OrderMasterListNg", ExtraCount);
                                                    isInvalidateCache = true;
                                                    break;
                                                case "RequisitionMaster":
                                                    SaveUDFColumnOrderInNewGrid("RequisitionListNg", ExtraCount);
                                                    isInvalidateCache = true;
                                                    break;
                                                case "ItemMasterList":
                                                    SaveUDFColumnOrderInNewGrid("ItemMasterListNg", ExtraCount);
                                                    //SaveUDFColumnOrderInNewGrid("ItemMasterListPVNg", ExtraCount);
                                                    isInvalidateCache = true;
                                                    break;
                                                case "ItemBinMasterList":                                                    
                                                    SaveUDFColumnOrderInNewGrid("ItemBinMasterViewNg", ExtraCount);
                                                    
                                                    isInvalidateCache = true;
                                                    break;
                                                case "InventoryCountList":
                                                    SaveUDFColumnOrderInNewGrid("InventoryCountListNg", ExtraCount);                                                    
                                                    isInvalidateCache = true;
                                                    break;
                                                case "WorkOrder":
                                                    SaveUDFColumnOrderInNewGrid("WorkOrderListNg", ExtraCount);
                                                    isInvalidateCache = true;
                                                    break;
                                                case "ToolList":
                                                    SaveUDFColumnOrderInNewGrid("ToolListNg", ExtraCount);
                                                    isInvalidateCache = true;
                                                    break;
                                                case "ManufacturerMasterList":
                                                    SaveUDFColumnOrderInNewGrid("ManufacturerListNg", ExtraCount);
                                                    isInvalidateCache = true;
                                                    break;
                                                case "VenderMasterList":
                                                    SaveUDFColumnOrderInNewGrid("VenderListNg", ExtraCount);
                                                    isInvalidateCache = true;
                                                    break;
                                                case "ShipViaMasterList":
                                                    SaveUDFColumnOrderInNewGrid("ShipViaListNg", ExtraCount);
                                                    isInvalidateCache = true;
                                                    break;
                                                case "ToolCategoryList":
                                                    SaveUDFColumnOrderInNewGrid("ToolCategoryListNg", ExtraCount);
                                                    isInvalidateCache = true;
                                                    break;
                                                case "LocationMasterList":
                                                    SaveUDFColumnOrderInNewGrid("ToolLocationListNg", ExtraCount);
                                                    isInvalidateCache = true;
                                                    break;

                                            }

                                            if (isInvalidateCache)
                                            {
                                                AppConfigController.InvalidateCache("Cached_UsersUISettingsDTO_" + SessionHelper.UserID + "" + SessionHelper.EnterPriceID + "" + SessionHelper.CompanyID + "" + SessionHelper.RoomID);
                                            }
                                            
                                            if (objDTO.UDFTableName.ToLower() == "pullmaster" && Values[0].ToLower() != "pullmaster")
                                            {
                                                //IEnumerable<UDFDTO> DataFromDB = null;
                                                //int TotalRecordCount = 0;
                                                var udfTables = new string[] { "RequisitionMaster", "WorkOrder" };
                                                long extraUDFinGrid = 0;
                                                long extraCount = 0;

                                                foreach (var tableName in udfTables)
                                                {
                                                    extraUDFinGrid += obj1.GetNonDeletedUDFCountByUDFTableName(tableName, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                    //DataFromDB = obj1.GetPagedRecords(0, 10, out TotalRecordCount, string.Empty, "ID asc", SessionHelper.CompanyID, tableName, SessionHelper.RoomID);

                                                    //if (DataFromDB != null && DataFromDB.Count() > 0)
                                                    //{
                                                    //    extraUDFinGrid += DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
                                                    //}
                                                }
                                                
                                                extraCount += extraUDFinGrid + Convert.ToInt64(Values[3]);
                                                
                                                if (extraCount > 0)
                                                {
                                                    currentTableUDFCount = obj1.GetNonDeletedUDFCountByUDFTableName(objDTO.UDFTableName, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                    if (currentTableUDFCount > 0)
                                                    {
                                                        currentTableUDFCount -= 1;
                                                    }
                                                }

                                                SaveUDFColumnOrderInGrid("PullMaster", extraCount,currentTableUDFCount);
                                                SaveUDFColumnOrderInNewGrid("PullMaster", extraCount, currentTableUDFCount);
                                                AppConfigController.InvalidateCache("Cached_UsersUISettingsDTO_" + SessionHelper.UserID + "" + SessionHelper.EnterPriceID + "" + SessionHelper.CompanyID + "" + SessionHelper.RoomID);
                                            }
                                        }
                                    }
                                }

                                //}
                                /*////////////////CODE FOR SAVE UDF ON GRID ORDER////////////////*/

                            }
                            if (RemoveUDFToChange)
                            {
                                string ListName = GetListNameFromTable(objDTO.UDFTableName);
                                //RemoveUDFColumnOrderInGrid(ListName, ExtraCount, TotalSetupCount);
                                eTurnsMaster.DAL.CommonMasterDAL objUDF = new eTurnsMaster.DAL.CommonMasterDAL();
                                Dictionary<int, string> GridListName = objUDF.getGridResourceListName(objDTO.UDFTableName);

                                if (GridListName != null && GridListName.Count() > 0)
                                {
                                    foreach (KeyValuePair<int, string> ReportResourceFileName in GridListName)
                                    {
                                        string[] Values = ReportResourceFileName.Value.Split('$');
                                        if (Values != null && Values.Count() > 0)
                                        {
                                            Int64 ExtraUDFinGrid = 0;
                                            if (Values[1] == "Yes")
                                            {
                                                //IEnumerable<UDFDTO> DataFromDB = null;
                                                //int TotalRecordCount = 0;
                                                UDFDAL obj1 = new UDFDAL(SessionHelper.EnterPriseDBName);
                                                ExtraUDFinGrid = obj1.GetNonDeletedUDFCountByUDFTableName(Values[2], SessionHelper.RoomID, SessionHelper.CompanyID);
                                                //DataFromDB = obj1.GetPagedRecords(0, 10, out TotalRecordCount, string.Empty, "ID asc", SessionHelper.CompanyID, Values[2], SessionHelper.RoomID);

                                                //if (DataFromDB != null && DataFromDB.Count() > 0)
                                                //{
                                                //    ExtraUDFinGrid = DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
                                                //}
                                            }
                                            //ExtraCount += ExtraUDFinGrid + (ReportResourceFileName.Key);
                                            ExtraCount += ExtraUDFinGrid + Convert.ToInt64(Values[3]);
                                            //TotalSetupCount += ExtraUDFinGrid + Convert.ToInt64(Values[3]);
                                            if (objDTO.UDFTableName.ToLower() == "toolmaster" && SessionHelper.AllowToolOrdering)
                                            {
                                                Values[0] = "ToolListNew";
                                            }
                                            RemoveUDFColumnOrderInGrid(Values[0], ExtraCount, TotalSetupCount, existingData.ToList(), objDTO);
                                            switch (Values[0])
                                            {
                                                case "ItemMasterList":
                                                    RemoveUDFColumnOrderInGridNg("ItemMasterListNg", ExtraCount, TotalSetupCount, existingData.ToList(), objDTO);
                                                    break;
                                                case "ManufacturerMasterList":
                                                    RemoveUDFColumnOrderInGridNg("ManufacturerListNg", ExtraCount, TotalSetupCount, existingData.ToList(), objDTO);                                                    
                                                    break;
                                                case "VenderMasterList":
                                                    RemoveUDFColumnOrderInGridNg("VenderListNg", ExtraCount, TotalSetupCount, existingData.ToList(), objDTO);                                                    
                                                    break;
                                                case "ShipViaMasterList":
                                                    RemoveUDFColumnOrderInGridNg("ShipViaListNg", ExtraCount, TotalSetupCount, existingData.ToList(), objDTO);                                                    
                                                    break;
                                                case "ToolCategoryList":
                                                    RemoveUDFColumnOrderInGridNg("ToolCategoryListNg", ExtraCount, TotalSetupCount, existingData.ToList(), objDTO);                                                    
                                                    break;
                                                case "LocationMasterList":
                                                    RemoveUDFColumnOrderInGridNg("ToolLocationListNg", ExtraCount, TotalSetupCount, existingData.ToList(), objDTO);
                                                    break;
                                                case "NewPullItemMaster":
                                                    RemoveUDFColumnOrderInGridNg("NewPullItemMaster", ExtraCount, TotalSetupCount, existingData.ToList(), objDTO);
                                                    break;
                                            }
                                            if (objDTO.UDFTableName.ToLower() == "pullmaster" && Values[0].ToLower() != "pullmaster")
                                            {
                                                //IEnumerable<UDFDTO> DataFromDB = null;
                                                //int TotalRecordCount = 0;
                                                var udfTables = new string[] { "RequisitionMaster", "WorkOrder" };
                                                long extraUDFinGrid = 0;
                                                long extraCount = 0;
                                                UDFDAL obj1 = new UDFDAL(SessionHelper.EnterPriseDBName);
                                                foreach (var tableName in udfTables)
                                                {
                                                    extraUDFinGrid += obj1.GetNonDeletedUDFCountByUDFTableName(tableName, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                    //DataFromDB = obj1.GetPagedRecords(0, 10, out TotalRecordCount, string.Empty, "ID asc", SessionHelper.CompanyID, tableName, SessionHelper.RoomID);

                                                    //if (DataFromDB != null && DataFromDB.Count() > 0)
                                                    //{
                                                    //    extraUDFinGrid += DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
                                                    //}
                                                }
                                                extraCount += extraUDFinGrid + Convert.ToInt64(Values[3]);
                                                RemoveUDFColumnOrderInGrid("PullMaster", extraCount, TotalSetupCount, existingData.ToList(), objDTO);
                                                RemoveUDFColumnOrderInGridNg("PullMaster", ExtraCount, TotalSetupCount, existingData.ToList(), objDTO);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                        message = ResMessage.SaveMessage;
                        status = "ok";

                        string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                        string SessinKey = "Res_" + ResourceHelper.CompanyResourceFolder + "_" + ResFileName + ResourceHelper.CurrentCult;
                        System.Web.HttpContext.Current.Cache.Remove(SessinKey);

                        string SessinKeyDB = "ResDB_" + SessionHelper.EnterPriceID+"_"+SessionHelper.CompanyID+"_"+SessionHelper.RoomID + "_" + ResFileName + (ResourceHelper.CurrentCult.Name == "en-US" ? "" : ResourceHelper.CurrentCult.Name);
                        AngularApp.AppConfigController.InvalidateCache(SessinKeyDB);
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                        status = "fail";
                    }
                }
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult UDFEdit(Int64 ID, string UDFTableKeyName, bool OtherFromeTurns = true, bool ForEnterPriseSetup = false)
        {
            Session["ControlType"] = null;
            int ResourcePageId = SessionHelper.GetUDFListID(UDFTableKeyName);
            if (ResourcePageId > 0)
            {
                ViewBag.ShowPDAField = true;
            }
            else
            {
                ViewBag.ShowPDAField = false;
            }

            if (UDFTableKeyName.ToLower() == "enterprises")
            {
                if (OtherFromeTurns)
                {
                    eTurnsMaster.DAL.UDFDAL obj = new eTurnsMaster.DAL.UDFDAL();
                    UDFDTO objDTO = obj.GetRecord(ID, SessionHelper.CompanyID);
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                    //Type t = Type.GetType(UDFTableResourceFileName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, objDTO.UDFColumnName, false, OtherFromeTurns);
                    if (!string.IsNullOrEmpty(val))
                        objDTO.UDFDisplayColumnName = val;
                    else
                        objDTO.UDFDisplayColumnName = objDTO.UDFColumnName;
                    objDTO.OtherFromeTurns = OtherFromeTurns;
                    objDTO.SetUpForEnterpriseLevel = ForEnterPriseSetup;
                    if (objDTO != null)
                    {
                        objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                        objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    }
                    objDTO.showPDAField = ViewBag.ShowPDAField;
                    objDTO.showPDAField = ViewBag.ShowPDAField;

                    Session["ControlType"] = objDTO;

                    if (OtherFromeTurns)
                        return PartialView("_CreateUDF", objDTO);
                    else
                        return PartialView("_CreateUDFeTurns", objDTO);
                }
                else
                {
                    UDFDAL obj = new UDFDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                    UDFDTO objDTO = obj.GetUDFByIDNormal(ID);
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                    //Type t = Type.GetType(UDFTableResourceFileName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, objDTO.UDFColumnName, false, OtherFromeTurns);
                    if (!string.IsNullOrEmpty(val))
                        objDTO.UDFDisplayColumnName = val;
                    else
                        objDTO.UDFDisplayColumnName = objDTO.UDFColumnName;
                    objDTO.OtherFromeTurns = OtherFromeTurns;
                    objDTO.SetUpForEnterpriseLevel = ForEnterPriseSetup;
                    if (objDTO != null)
                    {
                        objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                        objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    }
                    objDTO.showPDAField = ViewBag.ShowPDAField;
                    objDTO.showPDAField = ViewBag.ShowPDAField;

                    Session["ControlType"] = objDTO;

                    if (OtherFromeTurns)
                        return PartialView("_CreateUDF", objDTO);
                    else
                        return PartialView("_CreateUDFeTurns", objDTO);
                }
            }
            else
            {
                long companyID = 0;
                if (UDFTableKeyName.ToLower() != "companies" && UDFTableKeyName.ToLower() != "usermaster")
                    companyID = SessionHelper.CompanyID;
                if (ForEnterPriseSetup)
                {
                    companyID = 0;
                }
                bool isUDFName = false;
                if (UDFTableKeyName.ToLower() != "companies" && UDFTableKeyName.ToLower() != "rooms" && UDFTableKeyName.ToLower() != "usermaster")
                    isUDFName = true;


                UDFDTO objDTO = new UDFDTO();
                if (OtherFromeTurns)
                {
                    UDFDAL obj = new UDFDAL(SessionHelper.EnterPriseDBName);
                    objDTO = obj.GetUDFByIDNormal(ID);
                }
                else
                {
                    UDFDAL obj = new UDFDAL(CommonDAL.GeteTurnsDatabase());
                    objDTO = obj.GetUDFByIDNormal(ID);
                }
                if (UDFTableKeyName == "PullMaster" && ResourcePageId == 30)
                {
                    objDTO.ShowEncryptionCheckBox = true;
                }
                else
                {
                    objDTO.ShowEncryptionCheckBox = false;
                }
                string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                //Type t = Type.GetType(UDFTableResourceFileName);
                string val = string.Empty;
                if (UDFTableResourceFileName.ToLower() != "resusermasterudf")
                {

                    if (!UDFTableResourceFileName.ToLower().Contains("bom"))
                    {
                        if (UDFTableResourceFileName.ToLower() == "resroommaster" && (!ForEnterPriseSetup) && SessionHelper.RoomID > 0)
                        {
                            val = ResourceUtils.GetResource(UDFTableResourceFileName, objDTO.UDFColumnName, true, OtherFromeTurns, ForEnterPriseSetup);
                        }
                        else
                        {
                            val = ResourceUtils.GetResource(UDFTableResourceFileName, objDTO.UDFColumnName, isUDFName, OtherFromeTurns, ForEnterPriseSetup);
                        }
                        //val = ResourceUtils.GetResource(UDFTableResourceFileName, objDTO.UDFColumnName, isUDFName, OtherFromeTurns, ForEnterPriseSetup);
                    }
                    else
                    {
                        val = ResourceUtils.GetResource(UDFTableResourceFileName, objDTO.UDFColumnName, false, OtherFromeTurns, ForEnterPriseSetup);
                    }
                }
                else
                {
                    val = ResourceUtils.GetResource(UDFTableResourceFileName, objDTO.UDFColumnName, isUDFName, OtherFromeTurns, true);
                }
                if (!string.IsNullOrEmpty(val))
                    objDTO.UDFDisplayColumnName = val;
                else
                    objDTO.UDFDisplayColumnName = objDTO.UDFColumnName;
                objDTO.OtherFromeTurns = OtherFromeTurns;
                objDTO.SetUpForEnterpriseLevel = ForEnterPriseSetup;
                if (objDTO != null)
                {
                    objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    ResourcePageId = SessionHelper.GetUDFListID(objDTO.UDFTableName);
                    string UdfMobileResource = string.Empty;
                    if (ResourcePageId > 0)
                    {
                        MobileResourcesDAL objMobileResourcesDAL;
                        if (OtherFromeTurns)
                            objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                        else
                            objMobileResourcesDAL = new MobileResourcesDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                        int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                        if (OtherFromeTurns)
                        {
                            if (!ForEnterPriseSetup)
                            {
                                long RoomId = 0;
                                long CompanyId = 0;
                                if (UDFTableKeyName.ToLower() != "usermaster")
                                {
                                    RoomId = SessionHelper.RoomID;
                                    CompanyId = SessionHelper.CompanyID;
                                }
                                List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(RoomId, CompanyId, languageId, ResourcePageId).ToList();


                                if (MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == CompanyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                                {
                                    UdfMobileResource = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == CompanyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ResourceValue;
                                }
                                else
                                {
                                    UdfMobileResource = objDTO.UDFColumnName;
                                }
                            }
                            else
                            {
                                List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(0, 0, languageId, ResourcePageId).ToList();


                                if (MobileResourcesList.Where(i => i.Roomid == 0 && i.CompanyID == 0 && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                                {
                                    UdfMobileResource = MobileResourcesList.Where(i => i.Roomid == 0 && i.CompanyID == 0 && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ResourceValue;
                                }
                                else
                                {
                                    UdfMobileResource = objDTO.UDFColumnName;
                                }
                            }
                        }
                        else
                        {
                            List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(0, 0, languageId, ResourcePageId).ToList();


                            if (MobileResourcesList.Where(i => i.Roomid == 0 && i.CompanyID == 0 && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                            {
                                UdfMobileResource = MobileResourcesList.Where(i => i.Roomid == 0 && i.CompanyID == 0 && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ResourceValue;
                            }
                            else
                            {
                                UdfMobileResource = objDTO.UDFColumnName;
                            }
                        }
                    }
                    else
                    {
                        UdfMobileResource = objDTO.UDFColumnName;
                    }
                    objDTO.UDFPDADisplayColumnName = UdfMobileResource;
                }
                objDTO.showPDAField = ViewBag.ShowPDAField;

                Session["ControlType"] = objDTO;

                if (OtherFromeTurns)
                    if (!ForEnterPriseSetup)
                        return PartialView("_CreateUDF", objDTO);
                    else
                        return PartialView("_CreateUDFEnterprise", objDTO);
                else
                    return PartialView("_CreateUDFeTurns", objDTO);
            }
            //UDFApiController obj = new UDFApiController();

        }


        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult UDFListAjax(JQueryDataTableParamModel param, string t, bool OtherFromeTurns = true, bool ForEnterPriseSetup = false)
        {

            //UDFApiController obj = new UDFApiController();
            UDFDAL obj = new UDFDAL(SessionHelper.EnterPriseDBName);
            long EID = 0;
            if (Request["eid"] != null && t.ToLower() == "enterprise")
            {
                long.TryParse(Convert.ToString(Request["eid"]), out EID);
            }
            //LoadTestEntities entity = new LoadTestEntities();
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "UDFColumnName";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            int TotalRecordCount = 0;
            IEnumerable<UDFDTO> DataFromDB = null;
            
            if (OtherFromeTurns)
            {
                if (!ForEnterPriseSetup)
                {
                    if (!string.IsNullOrEmpty(t))
                    {
                        if (t.ToLower() == "enterprise")
                        {
                            eTurnsMaster.DAL.UDFDAL objMasterUDFDAL = new eTurnsMaster.DAL.UDFDAL();
                            DataFromDB = objMasterUDFDAL.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.CompanyID, t, EID);
                        }
                        else
                        {
                            if (!t.ToLower().Contains("bom"))
                            {
                                DataFromDB = obj.GetPagedUDFsByUDFTableNameNormal(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, t,
                                    SessionHelper.RoomID, SessionHelper.CompanyID, RoomDateFormat, CurrentTimeZone);
                            }
                            else
                            {
                                DataFromDB = obj.GetPagedUDFsByUDFTableNameNormal(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, t,
                                    0, SessionHelper.CompanyID, RoomDateFormat, CurrentTimeZone);
                            }
                        }
                    }
                    else
                    {
                        DataFromDB = obj.GetPagedUDFsByUDFTableNameNormal(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, t,
                                    SessionHelper.RoomID, SessionHelper.CompanyID, RoomDateFormat, CurrentTimeZone);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(t))
                    {
                        if (t.ToLower() == "enterprise")
                        {
                            eTurnsMaster.DAL.UDFDAL objMasterUDFDAL = new eTurnsMaster.DAL.UDFDAL();
                            DataFromDB = objMasterUDFDAL.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, 0, t, EID);
                        }
                        else
                        {
                            DataFromDB = obj.GetPagedUDFsByUDFTableNameNormal(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, t,
                                    0, 0, RoomDateFormat, CurrentTimeZone);
                        }
                    }
                    else
                    {
                        DataFromDB = obj.GetPagedUDFsByUDFTableNameNormal(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, t,
                                    0, 0, RoomDateFormat, CurrentTimeZone);
                    }
                }
            }
            else
            {
                obj = new UDFDAL(CommonDAL.GeteTurnsDatabase());
                

                if (!string.IsNullOrEmpty(t))
                {
                    if (t.ToLower() == "enterprise")
                    {
                        eTurnsMaster.DAL.UDFDAL objMasterUDFDAL = new eTurnsMaster.DAL.UDFDAL();
                        DataFromDB = obj.GetPagedUDFsByUDFTableNameForEturns(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, t, RoomDateFormat, CurrentTimeZone);
                    }
                    else
                    {
                        DataFromDB = obj.GetPagedUDFsByUDFTableNameForEturns(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, t, RoomDateFormat, CurrentTimeZone);
                    }
                }
                else
                {
                    DataFromDB = obj.GetPagedUDFsByUDFTableNameForEturns(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, t, RoomDateFormat, CurrentTimeZone);
                }
            }
            foreach (var item in DataFromDB)
            {
                if (t.ToLower() == "enterprise")
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(item.UDFTableName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, false, (OtherFromeTurns), ForEnterPriseSetup);
                    if (!string.IsNullOrEmpty(val))
                        item.UDFDisplayColumnName = val;
                    else
                        item.UDFDisplayColumnName = item.UDFColumnName;
                }
                else if (t.ToLower() == "companymaster")
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(item.UDFTableName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, false, (OtherFromeTurns), ForEnterPriseSetup);
                    if (!string.IsNullOrEmpty(val))
                        item.UDFDisplayColumnName = val;
                    else
                        item.UDFDisplayColumnName = item.UDFColumnName;
                }
                else
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(item.UDFTableName);
                    string val = string.Empty;// ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, true, (OtherFromeTurns), ForEnterPriseSetup);
                    if (UDFTableResourceFileName.ToLower() != "resusermasterudf" && (!UDFTableResourceFileName.ToLower().Contains("bom")))
                    {
                        if (UDFTableResourceFileName.ToLower() == "resroommaster" && (!ForEnterPriseSetup) && SessionHelper.RoomID <= 0)
                        {
                            val = ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, false, OtherFromeTurns, ForEnterPriseSetup);
                        }
                        else
                        {
                            val = ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, true, OtherFromeTurns, ForEnterPriseSetup);
                        }
                    }
                    else
                    {
                        val = ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, false, OtherFromeTurns, ForEnterPriseSetup);
                    }
                    if (!string.IsNullOrEmpty(val))
                        item.UDFDisplayColumnName = val;
                    else
                        item.UDFDisplayColumnName = item.UDFColumnName;
                }
                item.UDFPDADisplayColumnName = item.UDFColumnName; // To Do Get the value from Mobileresource table 
            }
            List<UDFDTO> objUDFList = DataFromDB.ToList();
            foreach (UDFDTO udf in objUDFList)
            {
                int ResourcePageId = SessionHelper.GetUDFListID(udf.UDFTableName);
                string UdfMobileResource = string.Empty;
                if (ResourcePageId > 0)
                {
                    MobileResourcesDAL objMobileResourcesDAL;
                    if (OtherFromeTurns)
                    {
                        objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                    }
                    else
                    {
                        objMobileResourcesDAL = new MobileResourcesDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                    }
                    int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                    if (OtherFromeTurns)
                    {
                        if (!ForEnterPriseSetup)
                        {
                            long RoomId = 0;
                            long CompanyId = 0;
                            if (udf.UDFTableName.ToLower() != "usermaster")
                            {
                                RoomId = SessionHelper.RoomID;
                                CompanyId = SessionHelper.CompanyID;
                            }
                            List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(RoomId, CompanyId, languageId, ResourcePageId).ToList();


                            if (MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == CompanyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == udf.UDFColumnName && i.LanguageID == languageId).Any())
                            {
                                UdfMobileResource = MobileResourcesList.Where(i => i.Roomid == RoomId && i.CompanyID == CompanyId && i.ResourcePageID == ResourcePageId && i.ResourceKey == udf.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ResourceValue;
                            }
                            else
                            {
                                UdfMobileResource = udf.UDFColumnName;
                                udf.showPDAField = true;
                            }
                        }
                        else
                        {
                            List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(0, 0, languageId, ResourcePageId).ToList();


                            if (MobileResourcesList.Where(i => i.Roomid == 0 && i.CompanyID == 0 && i.ResourcePageID == ResourcePageId && i.ResourceKey == udf.UDFColumnName && i.LanguageID == languageId).Any())
                            {
                                UdfMobileResource = MobileResourcesList.Where(i => i.Roomid == 0 && i.CompanyID == 0 && i.ResourcePageID == ResourcePageId && i.ResourceKey == udf.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ResourceValue;
                            }
                            else
                            {
                                UdfMobileResource = udf.UDFColumnName;
                                udf.showPDAField = true;
                            }
                        }
                    }
                    else
                    {
                        List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(0, 0, languageId, ResourcePageId).ToList();


                        if (MobileResourcesList.Where(i => i.Roomid == 0 && i.CompanyID == 0 && i.ResourcePageID == ResourcePageId && i.ResourceKey == udf.UDFColumnName && i.LanguageID == languageId).Any())
                        {
                            UdfMobileResource = MobileResourcesList.Where(i => i.Roomid == 0 && i.CompanyID == 0 && i.ResourcePageID == ResourcePageId && i.ResourceKey == udf.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ResourceValue;
                        }
                        else
                        {
                            UdfMobileResource = udf.UDFColumnName;
                            udf.showPDAField = true;
                        }
                    }
                }
                else
                {
                    UdfMobileResource = udf.UDFColumnName;
                    udf.showPDAField = false;
                }
                udf.UDFPDADisplayColumnName = UdfMobileResource;
            }
            var result = from c in DataFromDB
                         select new UDFDTO
                         {
                             ID = c.ID,
                             CompanyID = c.CompanyID,
                             Room = c.Room,
                             UDFTableName = c.UDFTableName,
                             UDFColumnName = c.UDFColumnName,
                             UDFDefaultValue = c.UDFDefaultValue,
                             UDFOptionsCSV = c.UDFOptionsCSV,
                             UDFControlType = c.UDFControlType,
                             UDFIsRequired = c.UDFIsRequired,
                             UDFIsSearchable = c.UDFIsSearchable,
                             Created = c.Created,
                             Updated = c.Updated,
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             IsDeleted = c.IsDeleted,
                             UDFDisplayColumnName = c.UDFDisplayColumnName,
                             UDFPDADisplayColumnName = c.UDFPDADisplayColumnName,
                             CreatedDate = CommonUtility.ConvertDateByTimeZone(c.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDate = CommonUtility.ConvertDateByTimeZone(c.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             showPDAField = c.showPDAField,
                             UDFMaxLength = c.UDFMaxLength
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string DeleteUDFRecords(string ids)
        {
            try
            {
                //UDFApiController obj = new UDFApiController();
                UDFDAL obj = new UDFDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteRecords(ids, SessionHelper.UserID);
                return "ok";

                //return "not ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Get UDF Options By UDFID
        /// </summary>
        /// <param name="UDFID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public JsonResult GetUDFOptionsByUDF(Int64 UDFID, string UDFTableName, long? EnterpriseID, bool OthereTurns = true)
        {
            //UDFOptionApiController obj = new UDFOptionApiController();
            if (!string.IsNullOrEmpty(UDFTableName) && UDFTableName.ToLower() == "enterprise")
            {
                eTurnsMaster.DAL.UDFOptionDAL objUDFOptionDAL = new eTurnsMaster.DAL.UDFOptionDAL();
                var data = objUDFOptionDAL.GetUDFOptionsByUDFIDPlain(UDFID).OrderBy(e => e.UDFOption);
                return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string EnterpriseDBName = string.Empty;
                if (EnterpriseID != null && EnterpriseID > 0)
                    EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(EnterpriseID ?? 0);

                UDFOptionDAL obj;
                if (!string.IsNullOrWhiteSpace(EnterpriseDBName))
                    obj = new UDFOptionDAL(EnterpriseDBName);
                else if (!OthereTurns)
                    obj = new UDFOptionDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                else
                    obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
                if (OthereTurns)
                {
                    UDFDAL objUDF = new UDFDAL(SessionHelper.EnterPriseDBName);
                    // UDFDTO objUDfDTO = new UDFDTO();
                    //objUDfDTO= objUDF.GetUDfDataById(SessionHelper.RoomID, SessionHelper.CompanyID, UDFID);
                    //var data = obj.GetUDFOptionsByUDF(UDFID, SessionHelper.CompanyID).OrderBy(e => e.UDFOption);
                    var data = obj.GetUDFOptionsByUDFIDPlain(UDFID).OrderBy(e => e.UDFOption).ToList();
                    if (data != null && data.Count() > 0)
                    {
                        foreach (var item in data)
                        {
                            item.UDFOption = CommonUtility.htmlEscape(item.UDFOption);
                        }
                    }
                    //if ((objUDfDTO.IsEncryption ?? false) == true)
                    //{
                    //    foreach(UDFOptionsDTO u in data)
                    //    {
                    //        u.UDFOption = GetDecryptValue(u.UDFOption);
                    //    }
                    //}
                    return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var data = obj.GetUDFOptionsByUDFeTurns(UDFID, SessionHelper.CompanyID).OrderBy(e => e.UDFOption);
                    obj = new UDFOptionDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                    var data = obj.GetUDFOptionsByUDFIDPlain(UDFID).OrderBy(e => e.UDFOption);
                    return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
                }

            }
        }
        public List<UDFOptionsDTO> GetUDFOptionsByUDFNew(Int64 UDFID, string UDFTableName, long? EnterpriseID, bool FromeTurns = false)
        {
            //UDFOptionApiController obj = new UDFOptionApiController();
            if (!string.IsNullOrEmpty(UDFTableName) && UDFTableName.ToLower() == "enterprise")
            {
                eTurnsMaster.DAL.UDFDAL objUDFDAL = new eTurnsMaster.DAL.UDFDAL();
                eTurnsMaster.DAL.UDFOptionDAL objUDFOptionDAL = new eTurnsMaster.DAL.UDFOptionDAL();
                var data = objUDFOptionDAL.GetUDFOptionsByUDFIDPlain(UDFID).OrderBy(e => e.UDFOption);
                //UDFDTO objUDFDTO = objUDFDAL.GetUDfDataById(SessionHelper.RoomID, SessionHelper.CompanyID, UDFID);
                //if (objUDFDTO != null && (objUDFDTO.IsEncryption ?? false) == true)
                //{
                //    foreach (UDFOptionsDTO objUdf in data)
                //    {
                //        objUdf.UDFOption = GetDecryptValue(objUdf.UDFOption);
                //    }
                //}
                return data.ToList();
            }
            else
            {
                string EnterpriseDBName = string.Empty;
                if (EnterpriseID != null && EnterpriseID > 0)
                    EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(EnterpriseID ?? 0);

                UDFOptionDAL obj;
                if (!FromeTurns)
                {
                    if (!string.IsNullOrWhiteSpace(EnterpriseDBName))
                        obj = new UDFOptionDAL(EnterpriseDBName);
                    else
                        obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
                }
                else
                {
                    obj = new UDFOptionDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                }
                //UDFDAL objUDFDAL = new UDFDAL(SessionHelper.EnterPriseDBName);
                //var data = obj.GetUDFOptionsByUDF(UDFID, SessionHelper.CompanyID).OrderBy(e => e.UDFOption);
                var data = obj.GetUDFOptionsByUDFIDPlain(UDFID).OrderBy(e => e.UDFOption);
                //UDFDTO objUDFDTO = objUDFDAL.GetUDfDataById(SessionHelper.RoomID, SessionHelper.CompanyID, UDFID);
                //if (objUDFDTO != null && (objUDFDTO.IsEncryption?? false) == true)
                //{
                //    foreach(UDFOptionsDTO objUdf in data)
                //    {
                //        objUdf.UDFOption=GetDecryptValue(objUdf.UDFOption);
                //    }
                //}

                return data.ToList();
            }
        }
        //public string GetDecryptValue(string UDFOption)
        //{
        //    try
        //    {
        //        string EncryptionKey = "eturns@@123";
        //        byte[] cipherBytes = Convert.FromBase64String(UDFOption);
        //        using (Aes encryptor = Aes.Create())
        //        {
        //            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        //            encryptor.Key = pdb.GetBytes(32);
        //            encryptor.IV = pdb.GetBytes(16);
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
        //                {
        //                    cs.Write(cipherBytes, 0, cipherBytes.Length);
        //                    cs.Close();
        //                }
        //                UDFOption = Encoding.Unicode.GetString(ms.ToArray());
        //            }
        //        }
        //        return UDFOption;
        //    }
        //    catch
        //    {
        //        return UDFOption;
        //    }
        //}
        //public string GetEncryptValue(string UDFOption)
        //{
        //    try
        //    {
        //        string EncryptionKey = "eturns@@123";
        //        byte[] clearBytes = Encoding.Unicode.GetBytes(UDFOption);
        //        using (Aes encryptor = Aes.Create())
        //        {
        //            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        //            encryptor.Key = pdb.GetBytes(32);
        //            encryptor.IV = pdb.GetBytes(16);
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
        //                {
        //                    cs.Write(clearBytes, 0, clearBytes.Length);
        //                    cs.Close();
        //                }
        //                UDFOption = Convert.ToBase64String(ms.ToArray());
        //            }
        //        }
        //        return UDFOption;
        //    }
        //    catch
        //    {
        //        return UDFOption;
        //    }
        //}
        public JsonResult GetUDFEditableOptionsByUDF(int maxRows, string name_startsWith, Int64 UDFID, long? EnterpriseID)
        {
            string EnterpriseDBName = string.Empty;
            if (EnterpriseID != null && EnterpriseID > 0)
                EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(EnterpriseID ?? 0);

            UDFOptionDAL obj;
            if (!string.IsNullOrWhiteSpace(EnterpriseDBName))
                obj = new UDFOptionDAL(EnterpriseDBName);
            else
                obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
            //List<UDFOptionsDTO> data = obj.GetUDFOptionsByUDF(UDFID, SessionHelper.CompanyID).OrderBy(e => e.UDFOption).Where(t => !string.IsNullOrWhiteSpace(t.UDFOption) && t.UDFOption.ToLower().Contains(name_startsWith.ToLower().Trim())).Take(maxRows).ToList();
            List<UDFOptionsDTO> data = obj.UDFOptionsByUDFIDNameSearchPlain(UDFID, name_startsWith).OrderBy(e => e.UDFOption).Where(t => !string.IsNullOrWhiteSpace(t.UDFOption)).Take(maxRows).ToList();

            return data.Count == 0 ? Json("", JsonRequestBehavior.AllowGet) : Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUDFEditableOptionsByUDFCombineQL(int maxRows, string name_startsWith, Int64 UDFID, long? EnterpriseID, string SelectedValue, Guid QuickListItemGUID)
        {
            string EnterpriseDBName = string.Empty;
            if (EnterpriseID != null && EnterpriseID > 0)
                EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(EnterpriseID ?? 0);

            UDFOptionDAL obj;
            if (!string.IsNullOrWhiteSpace(EnterpriseDBName))
                obj = new UDFOptionDAL(EnterpriseDBName);
            else
                obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
            //List<UDFOptionsDTO> data = obj.GetUDFOptionsByUDF(UDFID, SessionHelper.CompanyID).OrderBy(e => e.UDFOption).Where(t => !string.IsNullOrWhiteSpace(t.UDFOption) && t.UDFOption.ToLower().Contains(name_startsWith.ToLower().Trim())).Take(maxRows).ToList();
            List<UDFOptionsDTO> data = obj.UDFOptionsByUDFIDNameSearchPlain(UDFID, name_startsWith).OrderBy(e => e.UDFOption).Where(t => !string.IsNullOrWhiteSpace(t.UDFOption)).Take(maxRows).ToList();

            if (QuickListItemGUID != null && QuickListItemGUID != Guid.Empty)
            {
                QuickListDAL objQuickListDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
                string QLUDFValue = objQuickListDAL.GetUDFValueOrDefault(SessionHelper.CompanyID, SessionHelper.RoomID, QuickListItemGUID, UDFID, "");
                if (!string.IsNullOrEmpty(QLUDFValue) && !string.IsNullOrWhiteSpace(QLUDFValue)
                    && !data.Select(x => x.UDFOption).Contains(QLUDFValue)
                    && QLUDFValue.Contains(name_startsWith.Trim()))
                {
                    data.Add(new UDFOptionsDTO()
                    {
                        UDFOption = QLUDFValue,
                        ID = 0
                    });
                }
            }

            if (data.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetUDFEditableOptionsByUDFTrnsAndOrd(int maxRows, string name_startsWith, Int64 UDFID, long? EnterpriseID, string SelectedValue)
        {
            string EnterpriseDBName = string.Empty;
            if (EnterpriseID != null && EnterpriseID > 0)
                EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(EnterpriseID ?? 0);

            UDFOptionDAL obj;
            if (!string.IsNullOrWhiteSpace(EnterpriseDBName))
                obj = new UDFOptionDAL(EnterpriseDBName);
            else
                obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
            //List<UDFOptionsDTO> data = obj.GetUDFOptionsByUDF(UDFID, SessionHelper.CompanyID).OrderBy(e => e.UDFOption).Where(t => !string.IsNullOrWhiteSpace(t.UDFOption) && t.UDFOption.ToLower().Contains(name_startsWith.ToLower().Trim())).Take(maxRows).ToList();
            List<UDFOptionsDTO> data = obj.UDFOptionsByUDFIDNameSearchPlain(UDFID, name_startsWith).OrderBy(e => e.UDFOption).Where(t => !string.IsNullOrWhiteSpace(t.UDFOption)).Take(maxRows).ToList();

            if (data.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public List<UDFOptionsDTO> GetUDFOptionsByUDFForEditable(Int64 UDFID)
        {
            UDFOptionDAL obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
            return obj.GetUDFOptionsByUDFIDPlain(UDFID).OrderBy(e => e.UDFOption).ToList();
        }

        public JsonResult InsertUDFOptionNew(Int64 UDFID, string UDFOption, string UDFTableName, long? EnterpriseID, bool fromeTurns = false, bool SetUpForEnterpriseLevel = false)
        {
            if (!string.IsNullOrWhiteSpace(UDFTableName) && UDFTableName.ToLower() == "enterprise")
            {
                eTurnsMaster.DAL.CommonMasterDAL objCDAL = new eTurnsMaster.DAL.CommonMasterDAL();
                string strOK = objCDAL.DuplicateUFDOptionCheck(UDFOption, "add", 0, "UDFOptions", "UDFOption", UDFID);
                if (strOK == "duplicate")
                {
                    return Json(new { Response = "ok" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //UDFOptionApiController obj = new UDFOptionApiController();
                    eTurnsMaster.DAL.UDFOptionDAL obj = new eTurnsMaster.DAL.UDFOptionDAL();
                    UDFOptionsDTO objDTO = new UDFOptionsDTO();
                    objDTO.ID = 0;
                    objDTO.UDFOption = UDFOption;
                    objDTO.UDFID = UDFID;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.GUID = Guid.NewGuid();

                    var ResponseStatus = obj.Insert(objDTO);
                    return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                CommonDAL objCDAL;
                if (!fromeTurns)
                    objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                else
                    objCDAL = new CommonDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                string strOK = objCDAL.DuplicateUFDOptionCheck(UDFOption, "add", 0, "UDFOptions", "UDFOption", UDFID);
                if (strOK == "duplicate")
                {
                    return Json(new { Response = "ok" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //UDFOptionApiController obj = new UDFOptionApiController();
                    UDFOptionDAL obj;
                    if (!fromeTurns)
                    {
                        obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
                    }
                    else
                    {
                        obj = new UDFOptionDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                    }
                    UDFOptionsDTO objDTO = new UDFOptionsDTO();
                    objDTO.ID = 0;
                    UDFDAL objUDfDAL = new UDFDAL(SessionHelper.EnterPriseDBName);
                    UDFDTO objUDFDTO = objUDfDAL.GetUDFByIDPlain(UDFID);
                    if (objUDFDTO != null && ((objUDFDTO.IsEncryption ?? false) == true))
                    {
                        CommonDAL objcommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                        UDFOption = objcommonDAL.GetEncryptValue(UDFOption);
                    }
                    objDTO.UDFOption = UDFOption;
                    objDTO.UDFID = UDFID;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    if (!SetUpForEnterpriseLevel)
                    {
                        objDTO.CompanyID = SessionHelper.CompanyID;
                    }
                    else
                    {
                        objDTO.CompanyID = 0;
                    }
                    objDTO.GUID = Guid.NewGuid();

                    var ResponseStatus = obj.Insert(objDTO);
                    return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        
        [HttpPost]
        public JsonResult InsertUDFOptionBulk(UDFBulkInsertRequest request)
            {
            double totalInserted = 0;

            if (request.Items == null || request.Items.Count == 0)
                return Json(new { Response = "No items to insert" }, JsonRequestBehavior.AllowGet);

            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            var filteredItems = new List<UDFBulkItem>();

            filteredItems = objCDAL.DuplicateUFDOptionCheckWithList(request.Items, "add", 0, "UDFOptions");

            // Check for duplicates before bulk insert
            //foreach (var item in request.Items)
            //{
            //    string strOK = objCDAL.DuplicateUFDOptionCheck(item.UDFOption, "add", 0, "UDFOptions", "UDFOption", item.UDFID);
            //    if (strOK == "duplicate")
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        filteredItems.Add(item);
            //    }
            //}
            request.Items = filteredItems;

            if (request.Items != null && request.Items.Count > 0)
            {
                var objEnterPriseDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                List<EnterpriseDTO> lstEnterpriseList = objEnterPriseDAL.GetAllEnterpriseForExecutionWithDemo();

                foreach (EnterpriseDTO objEnt in lstEnterpriseList)
                {
                    UDFDAL objUDfDAL = new UDFDAL(objEnt.EnterpriseDBName);
                    var dtoList = new List<UDFOptionsDTO>();
                    totalInserted += objUDfDAL.BulkInsertForUDFData(request.Items, "UserMaster", 0);
                    //foreach (var item in request.Items)
                    //{
                    //    if (!string.IsNullOrWhiteSpace(item.UDFColumnName))
                    //    {
                    //            var tmpudf = objUDfDAL.GetUDFByCompanyAndColumnNamePlain(item.UDFColumnName, "UserMaster", 0);
                    //            if (tmpudf != null && tmpudf.ID > 0)
                    //            {
                    //                dtoList.Add(new UDFOptionsDTO
                    //                {
                    //                    ID = 0,
                    //                    UDFOption = item.UDFOption,
                    //                    UDFID = tmpudf.ID,
                    //                    Created = DateTimeUtility.DateTimeNow,
                    //                    CreatedBy = SessionHelper.UserID,
                    //                    Updated = DateTimeUtility.DateTimeNow,
                    //                    UpdatedByName = SessionHelper.UserName,
                    //                    LastUpdatedBy = SessionHelper.UserID,
                    //                    CompanyID = SessionHelper.CompanyID,
                    //                    GUID = Guid.NewGuid()
                    //                });
                    //            }
                    //    }
                    //}

                    //if (dtoList.Count > 0)
                    //{
                    //    var udfOptionDAL = new UDFOptionDAL(objEnt.EnterpriseDBName);
                    //    totalInserted += udfOptionDAL.BulkInsert(dtoList); // New DAL method
                    //}
                }
            }
            return Json(new { Response = totalInserted }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertUDFOption(Int64 UDFID, string UDFOption, string UDFTableName, long? EnterpriseID, bool fromeTurns = false, bool SetUpForEnterpriseLevel = false, string UDFColumnName = "", long RoomId = 0)
        {
            if (!string.IsNullOrWhiteSpace(UDFTableName) && UDFTableName.ToLower() == "enterprise")
            {
                eTurnsMaster.DAL.CommonMasterDAL objCDAL = new eTurnsMaster.DAL.CommonMasterDAL();
                string strOK = objCDAL.DuplicateUFDOptionCheck(UDFOption, "add", 0, "UDFOptions", "UDFOption", UDFID);
                if (strOK == "duplicate")
                {
                    return Json(new { Response = strOK }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //UDFOptionApiController obj = new UDFOptionApiController();
                    eTurnsMaster.DAL.UDFOptionDAL obj = new eTurnsMaster.DAL.UDFOptionDAL();
                    UDFOptionsDTO objDTO = new UDFOptionsDTO();
                    objDTO.ID = 0;
                    objDTO.UDFOption = UDFOption;
                    objDTO.UDFID = UDFID;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.GUID = Guid.NewGuid();

                    var ResponseStatus = obj.Insert(objDTO);
                    return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (!string.IsNullOrWhiteSpace(UDFTableName) && UDFTableName.ToLower() == "usermaster")
            {
                CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDAL.DuplicateUFDOptionCheck(UDFOption, "add", 0, "UDFOptions", "UDFOption", UDFID);
                if (strOK == "duplicate")
                {
                    return Json(new { Response = strOK }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var ResponseStatus = 0.0;
                    eTurnsMaster.DAL.EnterpriseMasterDAL objEnterPriseDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                    List<EnterpriseDTO> lstEnterpriseList = objEnterPriseDAL.GetAllEnterpriseForExecutionWithDemo();
                    foreach (EnterpriseDTO objEnt in lstEnterpriseList)
                    {
                        UDFOptionDAL objUdfOptionDAL = new UDFOptionDAL(objEnt.EnterpriseDBName);
                        UDFDTO objudfDTO = new UDFDTO();
                        UDFDAL objUDfDAL = new UDFDAL(objEnt.EnterpriseDBName);

                        if (!string.IsNullOrWhiteSpace(UDFColumnName))
                        {
                            var tmpudf = objUDfDAL.GetUDFByCompanyAndColumnNamePlain(UDFColumnName, "UserMaster", 0);

                            if (tmpudf != null && tmpudf.ID > 0)
                            {
                                objudfDTO = tmpudf;
                                UDFID = objudfDTO.ID;
                                //UDFOptionApiController obj = new UDFOptionApiController();
                                UDFOptionDAL obj = new UDFOptionDAL(objEnt.EnterpriseDBName);
                                UDFOptionsDTO objDTO = new UDFOptionsDTO();
                                objDTO.ID = 0;
                                objDTO.UDFOption = UDFOption;
                                objDTO.UDFID = UDFID;
                                objDTO.Created = DateTimeUtility.DateTimeNow;
                                objDTO.CreatedBy = SessionHelper.UserID;
                                objDTO.Updated = DateTimeUtility.DateTimeNow;
                                objDTO.UpdatedByName = SessionHelper.UserName;
                                objDTO.LastUpdatedBy = SessionHelper.UserID;
                                objDTO.CompanyID = SessionHelper.CompanyID;
                                objDTO.GUID = Guid.NewGuid();

                                ResponseStatus = obj.Insert(objDTO);
                            }
                        }
                    }
                    return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                CommonDAL objCDAL;
                UDFDAL objUDfDAL;
                if (!fromeTurns)
                {
                    objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    objUDfDAL = new UDFDAL(SessionHelper.EnterPriseDBName);
                }
                else
                {
                    objCDAL = new CommonDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                    objUDfDAL = new UDFDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                }
                //UDFDTO objUDFDTO = objUDfDAL.GetUDfDataById(SessionHelper.RoomID, SessionHelper.CompanyID, UDFID);
                UDFDTO objUDFDTO = objUDfDAL.GetUDFByIDPlain(UDFID);
                if (objUDFDTO != null && ((objUDFDTO.IsEncryption ?? false) == true))
                {
                    CommonDAL objcommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    UDFOption = objcommonDAL.GetEncryptValue(UDFOption);
                }
                string strOK = objCDAL.DuplicateUFDOptionCheck(UDFOption, "add", 0, "UDFOptions", "UDFOption", UDFID);
                if (strOK == "duplicate")
                {
                    return Json(new { Response = strOK }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //UDFOptionApiController obj = new UDFOptionApiController();
                    UDFOptionDAL obj;
                    if (!fromeTurns)
                    {
                        obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
                    }
                    else
                    {
                        obj = new UDFOptionDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                    }

                    UDFOptionsDTO objDTO = new UDFOptionsDTO();
                    objDTO.ID = 0;
                    objDTO.UDFOption = UDFOption;
                    objDTO.UDFID = UDFID;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    if (!SetUpForEnterpriseLevel)
                    {
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.Room = RoomId == 0 ? SessionHelper.RoomID : RoomId;
                    }
                    else
                    {
                        objDTO.CompanyID = 0;
                    }
                    objDTO.GUID = Guid.NewGuid();

                    var ResponseStatus = obj.Insert(objDTO);
                    return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public JsonResult EditUDFOption(Int64 ID, Int64 UDFID, string UDFOption, string UDFTableName, long? EnterpriseID, bool fromeTurns = false, bool SetUpForEnterpriseLevel = false, string OldUDFOption = "", string UDFColumnName = "")
        {
            //UDFOptionApiController obj = new UDFOptionApiController();
            CommonDAL objCDAL;
            if (!string.IsNullOrWhiteSpace(UDFTableName) && UDFTableName.ToLower() == "enterprise")
            {
                objCDAL = new CommonDAL("eTurnsMaster");
            }
            else
            {
                if (fromeTurns)
                {
                    objCDAL = new CommonDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                }
                else
                {
                    objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                }
            }
            string strOK = objCDAL.DuplicateUFDOptionCheck(UDFOption, "edit", ID, "UDFOptions", "UDFOption", UDFID);
            if (strOK == "duplicate")
            {
                return Json(new { Response = strOK }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                UDFOptionDAL obj;
                if (!string.IsNullOrWhiteSpace(UDFTableName) && UDFTableName.ToLower() == "enterprise")
                {
                    eTurnsMaster.DAL.UDFOptionDAL objEturnsMaster = new eTurnsMaster.DAL.UDFOptionDAL();
                    var ResponseStatus = objEturnsMaster.Edit(ID, UDFOption, SessionHelper.UserID, SessionHelper.CompanyID);
                    return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (fromeTurns)
                    {
                        obj = new UDFOptionDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                    }
                    else
                    {
                        obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
                    }
                    if (UDFTableName.ToLower() != "usermaster")
                    {
                        if (!SetUpForEnterpriseLevel)
                        {
                            var ResponseStatus = obj.Edit(ID, UDFOption, SessionHelper.UserID, SessionHelper.CompanyID, fromeTurns);
                            return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var ResponseStatus = obj.Edit(ID, UDFOption, SessionHelper.UserID, 0, fromeTurns);
                            return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        eTurnsMaster.DAL.EnterpriseMasterDAL objEnterPriseDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                        List<EnterpriseDTO> lstEnterpriseList = objEnterPriseDAL.GetAllEnterpriseForExecutionWithDemo();
                        var ResponseStatus = false;
                        foreach (EnterpriseDTO objEnt in lstEnterpriseList)
                        {
                            obj = new UDFOptionDAL(objEnt.EnterpriseDBName);
                            UDFOptionDAL objUdfOptionDAL = new UDFOptionDAL(objEnt.EnterpriseDBName);
                            UDFDTO objudfDTO = new UDFDTO();
                            UDFDAL objUDfDAL = new UDFDAL(objEnt.EnterpriseDBName);
                            
                            if (!string.IsNullOrWhiteSpace(UDFColumnName))
                            {
                                var tmpudf = objUDfDAL.GetUDFByCompanyAndColumnNamePlain(UDFColumnName, "UserMaster", 0);

                                if (tmpudf != null && tmpudf.ID > 0)
                                {
                                    objudfDTO = tmpudf;
                                    UDFID = objudfDTO.ID;
                                    UDFOptionsDTO objUDFOption = obj.GetUDFOptionByUDFIDAndOptionNamePlain(UDFID, OldUDFOption);
                                    if (objUDFOption != null && objUDFOption.ID > 0 && (!string.IsNullOrWhiteSpace(OldUDFOption)))
                                    {
                                        //UDFOptionsDTO objUDFOption = obj.GetUDFOptionsByUDF(UDFID, 0).Where(u => u.UDFOption == OldUDFOption).FirstOrDefault();

                                        UDFDAL objUDFDAL = new UDFDAL(objEnt.EnterpriseDBName);
                                        ResponseStatus = obj.EditUserMaster(objUDFOption.ID, UDFOption, SessionHelper.UserID, 0, false, Convert.ToString(objEnt.EnterpriseDBName));
                                    }
                                }                                
                            }
                        }
                        return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        public JsonResult DeleteUDFOption(Int64 ID, string UDFTableName, bool fromeTurns = false, string OldUDFOption = "", string UDFColumnName = "")
        {
            //UDFOptionApiController obj = new UDFOptionApiController();
            UDFOptionDAL obj;
            if (!string.IsNullOrWhiteSpace(UDFTableName) && UDFTableName.ToLower() == "enterprise")
            {
                obj = new UDFOptionDAL("eTurnsMaster");
                var ResponseStatus = obj.Delete(ID, SessionHelper.UserID, !fromeTurns);
                return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
            }
            else if (!string.IsNullOrWhiteSpace(UDFTableName) && UDFTableName.ToLower() == "usermaster")
            {

                eTurnsMaster.DAL.EnterpriseMasterDAL objEnterPriseDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                List<EnterpriseDTO> lstEnterpriseList = objEnterPriseDAL.GetAllEnterpriseForExecutionWithDemo();
                var ResponseStatus = false;
                foreach (EnterpriseDTO objEnt in lstEnterpriseList)
                {
                    obj = new UDFOptionDAL(objEnt.EnterpriseDBName);
                    UDFDAL objUDfDAL = new UDFDAL(objEnt.EnterpriseDBName);
                    Int64 UDFID = 0;
                    UDFDTO objudfDTO = new UDFDTO();
                    
                    if ((!string.IsNullOrWhiteSpace(UDFColumnName)))
                    {
                        var tmpudf = objUDfDAL.GetUDFByCompanyAndColumnNamePlain(UDFColumnName, "UserMaster", 0);
                        if (tmpudf != null && tmpudf.ID > 0)
                        {
                            objudfDTO = tmpudf;
                            UDFID = objudfDTO.ID;
                        }                        
                    }
                    if (!string.IsNullOrWhiteSpace(OldUDFOption) && UDFID > 0)
                    {
                        //UDFOptionsDTO objUDFOption = obj.GetUDFOptionsByUDF(UDFID, 0).Where(u => u.UDFOption == OldUDFOption).FirstOrDefault();
                        UDFOptionsDTO objUDFOption = obj.GetUDFOptionByUDFIDAndOptionNamePlain(UDFID, OldUDFOption);
                        if (objUDFOption != null)
                        {
                            ID = objUDFOption.ID;
                            ResponseStatus = obj.Delete(ID, SessionHelper.UserID, true);
                        }
                    }
                }
                return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (!fromeTurns)
                {
                    obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
                }
                else
                {
                    obj = new UDFOptionDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                }
                var ResponseStatus = obj.Delete(ID, SessionHelper.UserID, !fromeTurns);
                return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
            }

        }

        public object GetUDFDataPageWise(string PageName)
        {
            IEnumerable<UDFDTO> DataFromDB = null;
            if (!string.IsNullOrWhiteSpace(PageName) && PageName.ToLower() == "enterprise")
            {
                eTurnsMaster.DAL.UDFDAL objUDFApiController = new eTurnsMaster.DAL.UDFDAL();
                DataFromDB = objUDFApiController.GetAllRecords(PageName).OrderBy(e => e.UDFColumnName);
            }
            else
            {
                long RoomId = 0;
                long CompanyId = 0;
                if (PageName.ToLower() != "usermaster")
                {
                    RoomId = SessionHelper.RoomID;
                    CompanyId = SessionHelper.CompanyID;
                }
                if (PageName != null && PageName.ToLower().Contains("bom"))
                {
                    RoomId = 0;
                }
                UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                DataFromDB = objUDFApiController.GetUDFsByUDFTableNamePlain(PageName, RoomId, CompanyId).OrderBy(e => e.UDFColumnName);
            }
            //UDFApiController objUDFApiController = new UDFApiController();

            //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, PageName);


            var result = from c in DataFromDB
                         select new UDFDTO
                         {
                             ID = c.ID,
                             CompanyID = c.CompanyID,
                             Room = c.Room,
                             UDFTableName = c.UDFTableName,
                             UDFColumnName = c.UDFColumnName,
                             UDFDefaultValue = c.UDFDefaultValue,
                             UDFOptionsCSV = c.UDFOptionsCSV,
                             UDFControlType = c.UDFControlType,
                             UDFIsRequired = c.UDFIsRequired == null ? false : c.UDFIsRequired,
                             UDFIsSearchable = c.UDFIsSearchable,
                             Created = c.Created,
                             Updated = c.Updated,
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             IsDeleted = c.IsDeleted,
                             UDFMaxLength = c.UDFMaxLength
                         };
            return result;
        }

        public object GetUDFDataPageAndRoomCompanyWise(string PageName,int RoomID, int CompanyID)
        {
            IEnumerable<UDFDTO> DataFromDB = null;
            if (!string.IsNullOrWhiteSpace(PageName) && PageName.ToLower() == "enterprise")
            {
                eTurnsMaster.DAL.UDFDAL objUDFApiController = new eTurnsMaster.DAL.UDFDAL();
                DataFromDB = objUDFApiController.GetAllRecords(PageName).OrderBy(e => e.UDFColumnName);
            }
            else
            {
                long RoomId = 0;
                long CompanyId = 0;
                if (PageName.ToLower() != "usermaster")
                {
                    RoomId = RoomID;
                    CompanyId = CompanyID;
                }
                if (PageName != null && PageName.ToLower().Contains("bom"))
                {
                    RoomId = 0;
                }
                UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                DataFromDB = objUDFApiController.GetUDFsByUDFTableNamePlain(PageName, RoomId, CompanyId).OrderBy(e => e.UDFColumnName);
            }
            //UDFApiController objUDFApiController = new UDFApiController();

            //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, PageName);


            var result = from c in DataFromDB
                         select new UDFDTO
                         {
                             ID = c.ID,
                             CompanyID = c.CompanyID,
                             Room = c.Room,
                             UDFTableName = c.UDFTableName,
                             UDFColumnName = c.UDFColumnName,
                             UDFDefaultValue = c.UDFDefaultValue,
                             UDFOptionsCSV = c.UDFOptionsCSV,
                             UDFControlType = c.UDFControlType,
                             UDFIsRequired = c.UDFIsRequired,
                             UDFIsSearchable = c.UDFIsSearchable,
                             Created = c.Created,
                             Updated = c.Updated,
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             IsDeleted = c.IsDeleted,
                             UDFMaxLength = c.UDFMaxLength
                         };
            return result;
        }

        public object GetUDFDataPageWise(string PageName, long ID)
        {
            IEnumerable<UDFDTO> DataFromDB = null;
            if (!string.IsNullOrWhiteSpace(PageName) && PageName.ToLower() == "enterprise")
            {
                eTurnsMaster.DAL.UDFDAL objUDFApiController = new eTurnsMaster.DAL.UDFDAL();
                DataFromDB = objUDFApiController.GetAllRecords(PageName, ID).OrderBy(e => e.UDFColumnName);
            }
            else
            {
                UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                DataFromDB = objUDFApiController.GetUDFsByUDFTableNamePlain(PageName, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(e => e.UDFColumnName);
            }
            //UDFApiController objUDFApiController = new UDFApiController();

            //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, PageName);


            var result = from c in DataFromDB
                         select new UDFDTO
                         {
                             ID = c.ID,
                             CompanyID = c.CompanyID,
                             Room = c.Room,
                             UDFTableName = c.UDFTableName,
                             UDFColumnName = c.UDFColumnName,
                             UDFDefaultValue = c.UDFDefaultValue,
                             UDFOptionsCSV = c.UDFOptionsCSV,
                             UDFControlType = c.UDFControlType,
                             UDFIsRequired = c.UDFIsRequired,
                             UDFIsSearchable = c.UDFIsSearchable,
                             Created = c.Created,
                             Updated = c.Updated,
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             IsDeleted = c.IsDeleted,
                         };
            return result;
        }


        public JsonResult GetUDFDataPageWiseJSON(string PageName)
        {
            //UDFApiController objUDFApiController = new UDFApiController();
            try
            {


                UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetUDFsByUDFTableNamePlain( PageName, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(e => e.UDFColumnName);

                var result = from c in DataFromDB
                             select new UDFDTO
                             {
                                 ID = c.ID,
                                 //CompanyID = c.CompanyID,
                                 //UDFTableName = c.UDFTableName,
                                 UDFColumnName = c.UDFColumnName,
                                 UDFDefaultValue = c.UDFDefaultValue,
                                 //UDFOptionsCSV = c.UDFOptionsCSV,
                                 UDFControlType = c.UDFControlType,
                                 //UDFIsRequired = c.UDFIsRequired,
                                 //UDFIsSearchable = c.UDFIsSearchable,
                                 //Created = c.Created,
                                 //Updated = c.Updated,
                                 //UpdatedByName = c.UpdatedByName,
                                 //CreatedByName = c.CreatedByName,
                                 //IsDeleted = c.IsDeleted,
                             };

                return Json(new { Response = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { Response = new List<UDFDTO>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public object GetCompanyMasterUDFDataPageWise(string DatabaseName, long CompanyID, string PageName)
        {
            UDFDAL objUDFApiController = new UDFDAL(DatabaseName);
            var result = objUDFApiController.GetUDFsByCompanyAndTableNamePlain(0, PageName);
            return result;
        }

        public object GetRoomMasterUDFDataPageWise(string DatabaseName, long CompanyID, string PageName)
        {
            UDFDAL objUDFApiController = new UDFDAL(DatabaseName);
            var result = objUDFApiController.GetUDFsByCompanyAndTableNamePlain(CompanyID, PageName);
            return result;
        }
        public ActionResult RemoveUDF()
        {
            return View();
        }
        public JsonResult DeleteUDF(bool chkDeleteUDF)
        {
            try
            {
                if (SessionHelper.UserType != 1 || (!chkDeleteUDF) || SessionHelper.RoleID != -1)
                {
                    return Json(new { Status = false, Message = ResReportMaster.YouAreNotAbleToUpdate }, JsonRequestBehavior.AllowGet);
                }
                string msg = string.Empty;
                eTurnsMaster.DAL.EnterpriseMasterDAL objDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                List<EnterpriseDTO> lstEnterprise = new List<EnterpriseDTO>();
                lstEnterprise = objDAL.GetAllEnterprisesPlain();
                foreach (EnterpriseDTO item in lstEnterprise)
                {
                    string BaseReportPath = string.Empty;
                    string EntReportPath = string.Empty;
                    BaseReportPath = ResourceHelper.ResourceDirectoryBasePath + @"\MasterResources" + @"\CompanyResources";
                    EntReportPath = ResourceHelper.ResourceDirectoryBasePath + @"\" + item.ID + @"\CompanyResources";

                    if (System.IO.Directory.Exists(BaseReportPath))
                    {
                        foreach (var file in System.IO.Directory.GetFiles(BaseReportPath))
                        {
                            for (int i = 1; i < 11; i++)
                                RemoveResEntry(file, "UDF" + i);
                        }
                        foreach (var file in System.IO.Directory.GetFiles(EntReportPath))
                        {
                            for (int i = 1; i < 11; i++)
                                RemoveResEntry(file, "UDF" + i);
                        }

                        EntReportPath = EntReportPath.Replace("CompanyResources", "EnterpriseResources");
                        foreach (var file in System.IO.Directory.GetFiles(EntReportPath))
                        {
                            for (int i = 1; i < 11; i++)
                                RemoveResEntry(file, "UDF" + i);
                        }
                        EntReportPath = EntReportPath.Replace("EnterpriseResources", "");
                        foreach (var file in System.IO.Directory.GetFiles(EntReportPath))
                        {
                            for (int i = 1; i < 11; i++)
                                RemoveResEntry(file, "UDF" + i);
                        }
                        foreach (string d in Directory.GetDirectories(EntReportPath))
                        {
                            foreach (var file in System.IO.Directory.GetFiles(d))
                            {
                                for (int i = 1; i < 11; i++)
                                    RemoveResEntry(file, "UDF" + i);
                            }
                        }
                    }
                }
                return Json(new { Status = true, Message = ResMessage.DeletedSuccessfully }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        private static void RemoveResEntry(string resFileName, string resEntryKey)
        {
            bool isKeyExist = false;
            using (ResXResourceReader reader = new ResXResourceReader(resFileName))
            {
                using (ResXResourceWriter writer = new ResXResourceWriter(resFileName))
                {
                    IDictionaryEnumerator de = reader.GetEnumerator();
                    while (de.MoveNext())
                    {
                        if (!de.Entry.Key.ToString().Equals(resEntryKey, StringComparison.InvariantCultureIgnoreCase))
                        {
                            writer.AddResource(de.Entry.Key.ToString(), de.Entry.Value.ToString());
                        }
                        else
                        {
                            Console.WriteLine(string.Format(ResUDFSetup.KeyPairRemoved, de.Entry.Key.ToString(), de.Entry.Value.ToString()));
                            isKeyExist = true;
                        }
                    }
                    if (!isKeyExist)
                    {
                        Console.WriteLine(string.Format(ResUDFSetup.NoEntryFoundForKey, resEntryKey));
                    }
                }
            }
        }
        public ActionResult eTurnsUDfSetup()
        {
            if (SessionHelper.UserType == 1 && SessionHelper.RoleID == -1)
            {
                Dictionary<string, string[]> _UDFLIST = eTurnsWeb.Models.UDFDictionaryTables.GetUDFTables();
                _UDFLIST.Remove("room");
                SortedList UDFLIST = new SortedList();
                if (_UDFLIST != null && _UDFLIST.Count > 0)
                {
                    foreach (string Key in _UDFLIST.Keys)
                    {
                        UDFLIST.Add(Key, _UDFLIST[Key]);
                    }
                }
                ViewBag.UDFTableListName = UDFLIST;
                return View();
            }
            else
            {
                string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
                string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);
                return RedirectToAction(ActName, CtrlName);
            }
        }
        public ActionResult eTurnsUDfSetupEnterpirse()
        {
            var isEnterpriseUDFSetup = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.EnterpriseUDFSetup);

            if (isEnterpriseUDFSetup)
            {
                Dictionary<string, string[]> _UDFLIST = eTurnsWeb.Models.UDFDictionaryTables.GetUDFTables();
                SortedList UDFLIST = new SortedList();
                if (_UDFLIST != null && _UDFLIST.Count > 0)
                {
                    foreach (string Key in _UDFLIST.Keys)
                    {
                        UDFLIST.Add(Key, _UDFLIST[Key]);
                    }
                }
                string appPath1 = ResourceHelper.ResourceDirectoryBasePath + @"\";
                string resourceDirectory = appPath1 + SessionHelper.EnterPriceID;
                if (!System.IO.Directory.Exists(resourceDirectory + "\\RoomResources"))
                {
                    System.IO.Directory.CreateDirectory(resourceDirectory + "\\RoomResources");
                }
                ViewBag.UDFTableListName = UDFLIST;
                return View();
            }
            else
            {
                return RedirectToAction("MyProfile", "Master");
            }

        }

        public ActionResult GetUDFValue(string currentTable)
        {
            if (string.IsNullOrEmpty(currentTable))
            {
                currentTable = Request.QueryString["currentTable"];
            }
            string result = string.Empty;
            try
            {
                if (eTurnsWeb.Models.UDFDictionaryTables.IsVaidUDFTable(currentTable.ToLower()))
                {
                    int ModuleId = 0;
                    string UDFTableName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFTableFromKey(currentTable.ToLower(), out ModuleId);
                    ViewBag.UDFTableName = UDFTableName;
                    ViewBag.UDFTableNameKey = currentTable;
                    ViewBag.UDFHeader = UDFTableName;

                    ViewBag.EnterpriseID = 0;
                    //UDFApiController obj = new UDFApiController();
                    UDFDAL obj = new UDFDAL(SessionHelper.EnterPriseDBName);
                    eTurnsMaster.DAL.UDFDAL objUDFDALMaster = new eTurnsMaster.DAL.UDFDAL();
                    int ResourcePageId = SessionHelper.GetUDFListID(UDFTableName);
                    if (UDFTableName == "Enterprise")
                    {
                        objUDFDALMaster.InsertDefaultUDFs(UDFTableName, SessionHelper.UserID, 0);
                    }
                    else if (UDFTableName.ToLower() == "usermaster")
                    {
                        eTurnsMaster.DAL.EnterpriseMasterDAL objEnterPriseDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                        List<EnterpriseDTO> lstEnterpriseList = objEnterPriseDAL.GetAllEnterpriseForExecutionWithDemo();
                        foreach (EnterpriseDTO objEnt in lstEnterpriseList)
                        {
                            UDFDAL objUDFDAL = new UDFDAL(objEnt.EnterpriseDBName);
                            objUDFDAL.InsertDefaultUDFs(UDFTableName, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomID, ResourcePageId);
                        }
                    }
                    else
                    {
                        obj.InsertDefaultUDFs(UDFTableName, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomID, ResourcePageId);

                    }

                    if (ResourcePageId > 0)
                    {
                        ViewBag.ShowPDAField = true;
                    }
                    else
                    {
                        ViewBag.ShowPDAField = false;
                    }

                    return PartialView();
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception)
            {
                return View("Error");
            }

        }
        public ActionResult GetUDFValueEnterprise(string currentTable)
        {
            if (string.IsNullOrEmpty(currentTable))
            {
                currentTable = Request.QueryString["currentTable"];
            }
            string result = string.Empty;
            try
            {
                if (eTurnsWeb.Models.UDFDictionaryTables.IsVaidUDFTable(currentTable.ToLower()))
                {
                    int ModuleId = 0;
                    string UDFTableName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFTableFromKey(currentTable.ToLower(), out ModuleId);
                    ViewBag.UDFTableName = UDFTableName;
                    ViewBag.UDFTableNameKey = currentTable;
                    ViewBag.UDFHeader = UDFTableName;

                    ViewBag.EnterpriseID = 0;
                    //UDFApiController obj = new UDFApiController();
                    UDFDAL obj = new UDFDAL(SessionHelper.EnterPriseDBName);
                    eTurnsMaster.DAL.UDFDAL objUDFDALMaster = new eTurnsMaster.DAL.UDFDAL();
                    int ResourcePageId = SessionHelper.GetUDFListID(UDFTableName);
                    if (UDFTableName == "Enterprise")
                    {
                        objUDFDALMaster.InsertDefaultUDFs(UDFTableName, SessionHelper.UserID, 0);
                    }
                    else
                    {
                        obj.InsertDefaultUDFs(UDFTableName, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomID, ResourcePageId);

                    }

                    if (ResourcePageId > 0)
                    {
                        ViewBag.ShowPDAField = true;
                    }
                    else
                    {
                        ViewBag.ShowPDAField = false;
                    }

                    return PartialView();
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception)
            {
                return View("Error");
            }

        }
        public ActionResult UDFResource()
        {
            string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
            string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);
            if (SessionHelper.RoleID == -1)
            {
                eTurnsMaster.DAL.EnterpriseMasterDAL obj = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                List<EnterpriseDTO> lstEnterpriseList = obj.GetAllEnterpriseForExecutionWithDemo().Where(e => e.Name.ToLower() != "eturns" && e.Name.ToLower() != "eturnsmaster" && e.Name.ToLower() != "eturnsdemo" && e.Name.ToLower() != "eturnsmasterdemo").ToList();

                ViewBag.EnterPriseList = lstEnterpriseList;
                ScriptTemplate scriptTemplate = new ScriptTemplate();
                scriptTemplate.Message = string.Empty;
                scriptTemplate.EnterPriceDB = "0";
                ViewBag.SelectedDBs = null;
                return View("UDFResource", scriptTemplate);
            }
            else
            {

                return RedirectToAction(ActName, CtrlName);
            }
        }
        public string SaveNewlyAddedUDFKey(string EnterpriseIDNew)
        {
            try
            {
                string[] DBNameList = EnterpriseIDNew.Split(',');
                ResourceDAL objDAL = null;
                IEnumerable<ResourceLanguageDTO> resLangDTO = null;
                List<SelectListItem> lstItem = null;
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resLangDTO = objDAL.GetCachedResourceLanguageData(SessionHelper.EnterPriceID);
                lstItem = new List<SelectListItem>();



                foreach (string EnterpriseID in DBNameList)
                {
                    UDFDAL obj = new UDFDAL(EnterpriseID);
                    List<UDFDTO> DataFromDB = null;
                    RoomDAL objRoom = new RoomDAL(EnterpriseID);
                    List<RoomDTO> DataRoom = null;
                    DataRoom = objRoom.GetRoomByCompanyIDsPlain(string.Empty).ToList();
                    Int64 enterpriseid = Convert.ToInt64(EnterpriseID.Split('_')[0]);
                    foreach (RoomDTO r in DataRoom)
                    {
                        DataFromDB = obj.GetUDFsByRoomPlain(r.ID).ToList();

                        foreach (UDFDTO objUDF in DataFromDB)
                        {
                            foreach (var item in resLangDTO)
                            {
                                string ResourceBaseFilePath = CommonUtility.ResourceBaseFilePath;
                                if (objUDF.UDFTableName.ToLower() == "enterprise")
                                {
                                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objUDF.UDFTableName);
                                    string val = GetResourceValue(UDFTableResourceFileName, objUDF.UDFColumnName, false, (true), false, Convert.ToString(ResourceBaseFilePath + "/" + enterpriseid + "/" + r.CompanyID + "/" + r.ID + "/"));
                                    if (!string.IsNullOrEmpty(val))
                                        objUDF.UDFDisplayColumnName = val;
                                    else
                                        objUDF.UDFDisplayColumnName = objUDF.UDFColumnName;
                                }
                                else if (objUDF.UDFTableName.ToLower() == "companymaster" || objUDF.UDFTableName.ToLower() == "room")
                                {
                                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objUDF.UDFTableName);
                                    string val = GetResourceValue(UDFTableResourceFileName, objUDF.UDFColumnName, false, (true), false, Convert.ToString(ResourceBaseFilePath  + "/" + enterpriseid + "/" + r.CompanyID + "/" + r.ID + "/"));
                                    if (!string.IsNullOrEmpty(val))
                                        objUDF.UDFDisplayColumnName = val;
                                    else
                                        objUDF.UDFDisplayColumnName = objUDF.UDFColumnName;
                                }
                                else
                                {
                                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objUDF.UDFTableName);

                                    string culter = item.Culture;//eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                                    if (culter.ToLower() == "en-us")
                                    {
                                        culter = string.Empty;
                                    }
                                    else
                                    {
                                        UDFTableResourceFileName = UDFTableResourceFileName + "." + culter;
                                    }
                                    string val = GetResourceValue(UDFTableResourceFileName, objUDF.UDFColumnName, true, (true), false, Convert.ToString(ResourceBaseFilePath + "/" + enterpriseid + "/" + r.CompanyID + "/" + r.ID + "/"));
                                    if (!string.IsNullOrEmpty(val))
                                        objUDF.UDFDisplayColumnName = val;
                                    else
                                        objUDF.UDFDisplayColumnName = objUDF.UDFColumnName;
                                }
                                objUDF.UDFPDADisplayColumnName = objUDF.UDFColumnName; // To Do Get the value from Mobileresource table 

                                SaveReportResource(objUDF, Convert.ToString(ResourceBaseFilePath + "/" + enterpriseid + "/" + r.CompanyID + "/" + r.ID + "/"), item.Culture);
                            }
                        }
                    }
                }
                return "Sucess";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public void SaveReportResource(UDFDTO objDTO, string Path, string Culture)
        {
            string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);

            string culter = Culture;// eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
            UDFDAL objUDF;
            if ((objDTO.OtherFromeTurns) ?? true)
                objUDF = new UDFDAL(SessionHelper.EnterPriseDBName);
            else
                objUDF = new UDFDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
            //List<string> ReportResourceFileNameList = objUDF.getReportResourceFileName(ResFileName);
            Dictionary<int, string> ReportResourceFileNameList = objUDF.getReportResourceFileNameWithPrefix(ResFileName);
            eTurns.DTO.Resources.ResourceHelper resHelper = new eTurns.DTO.Resources.ResourceHelper();
            if ((objDTO.OtherFromeTurns) ?? true)
            {
                if (ReportResourceFileNameList != null && ReportResourceFileNameList.Count() > 0)
                {
                    foreach (KeyValuePair<int, string> ReportResourceFileName in ReportResourceFileNameList)
                    {
                        string ResourceFileName = ReportResourceFileName.Value.Split('$')[0];
                        string Prefix = ReportResourceFileName.Value.Split('$')[1];

                        objDTO.UDFColumnName = Prefix + objDTO.UDFColumnName;

                        string val = GetResource(ResourceFileName, objDTO.UDFColumnName, true, true, objDTO.SetUpForEnterpriseLevel ?? false, Path, Culture);
                        SaveResourcesByKey(ResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, true, (!objDTO.OtherFromeTurns) ?? false, objDTO.SetUpForEnterpriseLevel ?? false, Path);
                        if (!string.IsNullOrWhiteSpace(Prefix))
                        {
                            objDTO.UDFColumnName = objDTO.UDFColumnName.Replace(Prefix, "");
                        }
                        string strFilePathWithFileName = resHelper.getFilePath(ResourceFileName, true);
                        string actulFilename = ResourceFileName + "." + culter + ".resx";

                    }
                }
            }

        }
        public string GetResourceValue(string ResourceFileName, string ResourceKey, bool isUDFName, bool OtherFromeTurns = true, bool ForEnterPriseSetup = false, string filepath = null)
        {
            XmlDocument loResource = null;
            XmlNode loRoot = null;
            try
            {
                string strFilePath = string.Empty;
                string SessinKey = string.Empty;

                strFilePath = filepath + ResourceFileName + ".resx";

                if (loResource == null)
                {
                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                    {
                        loResource.Load(strFilePath);
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Cache.Add(SessinKey, loResource, cacheDep, ResourceHelper.CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else if (!string.IsNullOrEmpty(ResourceHelper.FileCulterExtension) && ResourceHelper.FileCulterExtension.ToLower() != "en-us") // This logic is for if no other culter file exist then default load english culter
                    {
                        strFilePath = strFilePath.Replace(ResourceHelper.FileCulterExtension, "");
                        if (System.IO.File.Exists(strFilePath))
                            loResource.Load(strFilePath);
                        else if (isUDFName)
                        {
                            CreateRoomResourceFile(strFilePath, ResourceKey);
                            loResource.Load(strFilePath);
                        }
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Cache.Add(SessinKey, loResource, cacheDep, ResourceHelper.CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else
                    {
                        if (isUDFName)
                        {
                            CreateRoomResourceFile(strFilePath, ResourceKey);
                            loResource.Load(strFilePath);
                            CacheDependency cacheDep = new CacheDependency(strFilePath);
                            HttpContext.Cache.Add(SessinKey, loResource, cacheDep, ResourceHelper.CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                        }
                        else
                            return ResourceKey;
                    }
                }

                loRoot = loResource.SelectSingleNode("root/data[@name='" + ResourceKey + "']/value");

                return loRoot == null ? ResourceKey : loRoot.InnerText;
            }
            catch (Exception)
            {
                //throw ex;
                return ResourceKey;
                // Add exception log code here;
            }

            finally
            {
                loResource = null;
                loRoot = null;
            }
        }
        public string GetResource(string ResourceFileName, string ResourceKey, bool isUDFName, bool OtherFromeTurns = true, bool ForEnterPriseSetup = false, string filepath = null, string Culture = "en-us")
        {
            XmlDocument loResource = null;
            XmlNode loRoot = null;
            try
            {
                string strFilePath = string.Empty;
                string SessinKey = string.Empty;
                string culter = Culture;// ResourceHelper.FileCulterExtension;
                if (culter.ToLower() == "en-us")
                {
                    culter = string.Empty;
                }
                else
                {
                    culter = "." + culter;
                }

                strFilePath = filepath + ResourceFileName + culter + ".resx";
                if (loResource == null)
                {
                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                    {
                        loResource.Load(strFilePath);
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Cache.Add(SessinKey, loResource, cacheDep, ResourceHelper.CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else if (!string.IsNullOrEmpty(ResourceHelper.FileCulterExtension) && ResourceHelper.FileCulterExtension.ToLower() != "en-us") // This logic is for if no other culter file exist then default load english culter
                    {
                        //  strFilePath = strFilePath.Replace(ResourceHelper.FileCulterExtension, "");
                        if (System.IO.File.Exists(strFilePath))
                            loResource.Load(strFilePath);
                        else if (isUDFName)
                        {
                            CreateRoomResourceFile(strFilePath, ResourceKey);
                            loResource.Load(strFilePath);
                        }
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Cache.Add(SessinKey, loResource, cacheDep, ResourceHelper.CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else
                    {
                        if (isUDFName)
                        {
                            CreateRoomResourceFile(strFilePath, ResourceKey);
                            loResource.Load(strFilePath);
                            CacheDependency cacheDep = new CacheDependency(strFilePath);
                            HttpContext.Cache.Add(SessinKey, loResource, cacheDep, ResourceHelper.CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                        }
                        else
                            return ResourceKey;
                    }
                }

                loRoot = loResource.SelectSingleNode("root/data[@name='" + ResourceKey + "']/value");

                return loRoot == null ? ResourceKey : loRoot.InnerText;

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

            finally
            {
                loResource = null;
                loRoot = null;
            }
        }
        private void CreateRoomResourceFile(string strFilePath, string ResourceKey = "")
        {
            using (System.Resources.ResXResourceWriter resx = new System.Resources.ResXResourceWriter(strFilePath))
            {
                if (string.IsNullOrEmpty(ResourceKey))
                {
                    resx.AddResource("UDF1", "UDF1");
                    resx.AddResource("UDF2", "UDF2");
                    resx.AddResource("UDF3", "UDF3");
                    resx.AddResource("UDF4", "UDF4");
                    resx.AddResource("UDF5", "UDF5");
                    resx.AddResource("UDF6", "UDF6");
                    resx.AddResource("UDF7", "UDF7");
                    resx.AddResource("UDF8", "UDF8");
                    resx.AddResource("UDF9", "UDF9");
                    resx.AddResource("UDF10", "UDF10");
                }
                else
                {
                    ResourceKey = ResourceKey.Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "").Replace("5", "");

                    resx.AddResource(ResourceKey + "1", ResourceKey + "1");
                    resx.AddResource(ResourceKey + "2", ResourceKey + "2");
                    resx.AddResource(ResourceKey + "3", ResourceKey + "3");
                    resx.AddResource(ResourceKey + "4", ResourceKey + "4");
                    resx.AddResource(ResourceKey + "5", ResourceKey + "5");
                    resx.AddResource(ResourceKey + "6", ResourceKey + "6");
                    resx.AddResource(ResourceKey + "7", ResourceKey + "7");
                    resx.AddResource(ResourceKey + "8", ResourceKey + "8");
                    resx.AddResource(ResourceKey + "9", ResourceKey + "9");
                    resx.AddResource(ResourceKey + "10", ResourceKey + "10");
                }
            }
        }
        public void SaveResourcesByKey(string ResourceFile, string culter, string ResourceKey, string value, bool isUDFName, bool SaveIneTurns = false, bool ForEnterPriseSetup = false, string filepath = null)
        {
            XmlDocument loResource = null;

            try
            {
                if (culter.ToLower() == "en-us")
                    culter = "";
                else
                    culter = "." + culter;

                string strFilePath = string.Empty;
                string SessinKey = string.Empty;

                strFilePath = filepath + ResourceFile + culter + ".resx";



                if (loResource == null)
                {
                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                        loResource.Load(strFilePath);
                }
                if (loResource.SelectSingleNode("root/data[@name='" + ResourceKey + "']") != null)
                {
                    loResource.SelectSingleNode("root/data[@name='" + ResourceKey + "']/value").InnerText = value;

                }
                else
                {
                    if (!isUDFName)
                    {
                        XmlNode child = null;
                        XmlNode xname = null;
                        XmlNode rootnode = null;

                        rootnode = loResource.SelectSingleNode("root");
                        xname = loResource.CreateNode(XmlNodeType.Element, "data", null);
                        XmlAttribute xa = loResource.CreateAttribute("name");
                        xa.Value = ResourceKey;
                        XmlAttribute xa1 = loResource.CreateAttribute("xml", "space", "xml");
                        xa1.Value = "preserve";
                        xname.Attributes.Append(xa);
                        xname.Attributes.Append(xa1);
                        child = loResource.CreateNode(XmlNodeType.Element, "value", null);
                        child.InnerText = value;
                        xname.AppendChild(child);
                        rootnode.AppendChild(xname);
                    }
                    else
                    {
                        if (loResource != null)
                        {
                            XmlNode child = null;
                            XmlNode xname = null;
                            XmlNode rootnode = null;

                            rootnode = loResource.SelectSingleNode("root");
                            xname = loResource.CreateNode(XmlNodeType.Element, "data", null);
                            XmlAttribute xa = loResource.CreateAttribute("name");
                            xa.Value = ResourceKey;
                            XmlAttribute xa1 = loResource.CreateAttribute("xml", "space", "xml");
                            xa1.Value = "preserve";
                            xname.Attributes.Append(xa);
                            xname.Attributes.Append(xa1);
                            child = loResource.CreateNode(XmlNodeType.Element, "value", null);
                            child.InnerText = value;
                            if (xname != null)
                                xname.AppendChild(child);
                            if (xname != null && rootnode != null)
                                rootnode.AppendChild(xname);
                        }
                    }
                }
                loResource.Save(strFilePath);

            }
            catch (Exception)
            {

                // Add exception log code here;
            }

            finally
            {
                loResource = null;
            }


        }

        private void SaveUDFColumnOrderInNewGrid(string ListName, Int64 ExtraCount, int currentTableUDFCount = 0)
        {
            try { 
            if (!string.IsNullOrEmpty(ListName))
            {
                eTurnsMaster.DAL.UsersUISettingsDAL obj = new eTurnsMaster.DAL.UsersUISettingsDAL();
                UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
                objDTO = obj.GetRecordWithoutCacheNg(SessionHelper.UserID, ListName, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                SiteListColumnDetailDAL siteListColumnDetailDAL = new SiteListColumnDetailDAL(SessionHelper.EnterPriseDBName);
                int maximumColumnOrderWithoutUDF = siteListColumnDetailDAL.GetMaximumColumnOrderByListName(ListName, SiteSettingHelper.UsersUISettingType);

                if (objDTO == null || string.IsNullOrEmpty(objDTO.JSONDATA))
                {
                    SiteListMasterDAL siteListMasterDAL = new SiteListMasterDAL(SessionHelper.EnterPriseDBName);
                    string siteListMasterJson = siteListMasterDAL.GetSiteListMasterDataByListName(ListName, SiteSettingHelper.UsersUISettingType);

                    objDTO = new UsersUISettingsDTO
                    {
                        ListName = ListName,
                        JSONDATA = siteListMasterJson
                    };
                }

                if (objDTO != null)
                {
                    if (!string.IsNullOrEmpty(objDTO.JSONDATA))
                    {

                        JObject gridStateJS = new Newtonsoft.Json.Linq.JObject();


                        // jsonData = objDTO.JSONDATA;
                        /*////////CODE FOR UPDATE JSON STRING/////////*/
                        // JObject gridStaeJS = new JObject();
                        gridStateJS = JObject.Parse(objDTO.JSONDATA);
                        /*////////CODE FOR UPDATE JSON STRING/////////*/

                        JToken orderCols = gridStateJS["ColReorder"];
                        JArray arrOCols = (JArray)orderCols;

                        JArray arrONewCols = new JArray();
                        if (arrOCols != null)
                        {
                            int orderClength = arrOCols.Count;

                            if (orderClength > 4)
                            {

                                


                                JToken aoSearchCols = gridStateJS["columns"];
                                JArray arrSCols = (JArray)aoSearchCols;
                                if (arrSCols != null)
                                {
                                    JObject UpdateAccProfile = new JObject(
                                            new JProperty("search", ""),
                                            new JProperty("smart", true),
                                            new JProperty("regex", false),
                                            new JProperty("caseInsensitive", true)
                                           );

                                    JObject UpdateAccProfileNew = new JObject(
                                           new JProperty("visible", true),
                                           new JProperty("search", UpdateAccProfile),
                                           new JProperty("width", 48));
                                    arrSCols.Add((object)UpdateAccProfileNew);
                                }


                                int maxOrder = arrOCols.Select(c => (int)c).ToList().Max();
                                //int jCount = 0;
                                //for (int i = 0; i < orderClength + 1; i++)
                                //{
                                //    if (i == 3)
                                //    {
                                //        arrONewCols.Add(maxOrder + 1);
                                //    }
                                //    else
                                //    {
                                //        arrONewCols.Add(arrOCols[jCount]);
                                //        jCount++;
                                //    }
                                //}
                                Int32 currentUDFVAl = 0;// (Convert.ToInt32(maxOrder) - Convert.ToInt32(ExtraCount) + 1);
                                if (ExtraCount > 0)
                                {
                                    currentUDFVAl = maxOrder < (maximumColumnOrderWithoutUDF + Convert.ToInt32(ExtraCount) + currentTableUDFCount) ? (Convert.ToInt32(maxOrder) + 1) : (Convert.ToInt32(maxOrder) - Convert.ToInt32(ExtraCount) + 1);
                                }
                                else
                                {
                                    currentUDFVAl = (Convert.ToInt32(maxOrder) + 1);
                                }
                                for (int i = 0; i < orderClength; i++)
                                {
                                    if (Convert.ToInt32(((Newtonsoft.Json.Linq.JValue)(arrOCols[i])).Value) >= currentUDFVAl)
                                    {
                                        ((Newtonsoft.Json.Linq.JValue)(arrOCols[i])).Value = Convert.ToInt32(((Newtonsoft.Json.Linq.JValue)(arrOCols[i])).Value) + 1;
                                    }
                                }
                                arrOCols.Insert(3, currentUDFVAl);

                                gridStateJS["ColReorder"] = arrOCols;
                                
                                gridStateJS["columns"] = arrSCols;
                                

                                string updatedJSON = gridStateJS.ToString(Newtonsoft.Json.Formatting.None);

                                /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                                objDTO = new UsersUISettingsDTO();
                                objDTO.UserID = SessionHelper.UserID;

                                objDTO.EnterpriseID = SessionHelper.EnterPriceID;
                                objDTO.CompanyID = SessionHelper.CompanyID;
                                objDTO.RoomID = SessionHelper.RoomID;

                                objDTO.JSONDATA = updatedJSON;
                                objDTO.ListName = ListName;
                                obj.SaveUserListViewSettings(objDTO, "Angular", true);
                                /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                            }
                        }
                    }

                }

            }
            }
            catch { }
        }
        private void SaveUDFColumnOrderInGrid(string ListName, Int64 ExtraCount,int currentTableUDFCount)
        {
            try { 
            if (!string.IsNullOrEmpty(ListName))
            {
                eTurnsMaster.DAL.UsersUISettingsDAL obj = new eTurnsMaster.DAL.UsersUISettingsDAL();
                UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
                objDTO = obj.GetRecord(SessionHelper.UserID, ListName, SiteSettingHelper.UsersUISettingType, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                SiteListColumnDetailDAL siteListColumnDetailDAL = new SiteListColumnDetailDAL(SessionHelper.EnterPriseDBName);
                int maximumColumnOrderWithoutUDF = siteListColumnDetailDAL.GetMaximumColumnOrderByListName(ListName, SiteSettingHelper.UsersUISettingType);

                if (objDTO == null || string.IsNullOrEmpty(objDTO.JSONDATA))
                {
                    SiteListMasterDAL siteListMasterDAL = new SiteListMasterDAL(SessionHelper.EnterPriseDBName);
                    string siteListMasterJson = siteListMasterDAL.GetSiteListMasterDataByListName(ListName, SiteSettingHelper.UsersUISettingType);
                    objDTO = new UsersUISettingsDTO
                    {
                        ListName = ListName,
                        JSONDATA = siteListMasterJson
                    };
                }

                

                if (objDTO != null)
                {
                    if (!string.IsNullOrEmpty(objDTO.JSONDATA))
                    {

                        JObject gridStateJS = new Newtonsoft.Json.Linq.JObject();


                        // jsonData = objDTO.JSONDATA;
                        /*////////CODE FOR UPDATE JSON STRING/////////*/
                        // JObject gridStaeJS = new JObject();
                        gridStateJS = JObject.Parse(objDTO.JSONDATA);
                        /*////////CODE FOR UPDATE JSON STRING/////////*/

                        JToken orderCols = gridStateJS["ColReorder"];
                        JArray arrOCols = (JArray)orderCols;

                        JArray arrONewCols = new JArray();
                        if (arrOCols != null)
                        {
                            int orderClength = arrOCols.Count;

                            if (orderClength > 4)
                            {

                                JToken abVisCols = gridStateJS["abVisCols"];
                                JArray visCols = (JArray)abVisCols;


                                JToken aoSearchCols = gridStateJS["aoSearchCols"];
                                JArray arrSCols = (JArray)aoSearchCols;
                                if (arrSCols != null)
                                {
                                    JObject UpdateAccProfile = new JObject(
                                           new JProperty("bCaseInsensitive", true),
                                           new JProperty("sSearch", ""),
                                           new JProperty("bRegex", false),
                                           new JProperty("bSmart", true));
                                    arrSCols.Add((object)UpdateAccProfile);
                                }

                                if (visCols != null)
                                {
                                    int visClength = visCols.Count;
                                    visCols.Add(true);
                                }
                                JToken widthCols = gridStateJS["ColWidth"];
                                JArray arrWCols = (JArray)widthCols;
                                if (arrWCols != null)
                                {
                                    int widthlength = arrWCols.Count;
                                    arrWCols.Insert(3, "48px");
                                }

                                int maxOrder = arrOCols.Select(c => (int)c).ToList().Max();
                                //int jCount = 0;
                                //for (int i = 0; i < orderClength + 1; i++)
                                //{
                                //    if (i == 3)
                                //    {
                                //        arrONewCols.Add(maxOrder + 1);
                                //    }
                                //    else
                                //    {
                                //        arrONewCols.Add(arrOCols[jCount]);
                                //        jCount++;
                                //    }
                                //}
                                int currentUDFVAl = 0;
                                if (ExtraCount > 0)
                                {
                                    currentUDFVAl = maxOrder < (maximumColumnOrderWithoutUDF + Convert.ToInt32(ExtraCount) + currentTableUDFCount)  ? (Convert.ToInt32(maxOrder) + 1) : (Convert.ToInt32(maxOrder) - Convert.ToInt32(ExtraCount) + 1);
                                }
                                else
                                {
                                    currentUDFVAl =  (Convert.ToInt32(maxOrder) + 1);
                                }
                                
                                for (int i = 0; i < orderClength; i++)
                                {
                                    if (Convert.ToInt32(((Newtonsoft.Json.Linq.JValue)(arrOCols[i])).Value) >= currentUDFVAl)
                                    {
                                        ((Newtonsoft.Json.Linq.JValue)(arrOCols[i])).Value = Convert.ToInt32(((Newtonsoft.Json.Linq.JValue)(arrOCols[i])).Value) + 1;
                                    }
                                }
                                arrOCols.Insert(3, currentUDFVAl);

                                gridStateJS["ColReorder"] = arrOCols;
                                gridStateJS["abVisCols"] = visCols;
                                gridStateJS["aoSearchCols"] = arrSCols;
                                gridStateJS["ColWidth"] = arrWCols;

                                string updatedJSON = gridStateJS.ToString();

                                /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                                objDTO = new UsersUISettingsDTO();
                                objDTO.UserID = SessionHelper.UserID;

                                objDTO.EnterpriseID = SessionHelper.EnterPriceID;
                                objDTO.CompanyID = SessionHelper.CompanyID;
                                objDTO.RoomID = SessionHelper.RoomID;

                                objDTO.JSONDATA = updatedJSON;
                                objDTO.ListName = ListName;
                                obj.SaveUserListViewSettings(objDTO, SiteSettingHelper.UsersUISettingType, true);
                                /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                            }
                        }
                    }

                }

            }
            }
            catch { }
        }


        private void RemoveUDFColumnOrderInGrid(string ListName, long ExtraCount, long TotalSetupCount, List<UDFDTO> DataFromDB, UDFDTO udf)
        {
            try { 
            if (!string.IsNullOrEmpty(ListName))
            {
                eTurnsMaster.DAL.UsersUISettingsDAL obj = new eTurnsMaster.DAL.UsersUISettingsDAL();
                UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
                objDTO = obj.GetRecord(SessionHelper.UserID, ListName, SiteSettingHelper.UsersUISettingType, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);

                if (objDTO != null)
                {
                    if (!string.IsNullOrEmpty(objDTO.JSONDATA))
                    {
                        JObject gridStateJS = new Newtonsoft.Json.Linq.JObject();
                        // jsonData = objDTO.JSONDATA;
                        /*////////CODE FOR UPDATE JSON STRING/////////*/
                        // JObject gridStaeJS = new JObject();
                        gridStateJS = JObject.Parse(objDTO.JSONDATA);
                        /*////////CODE FOR UPDATE JSON STRING/////////*/
                        JToken orderCols = gridStateJS["ColReorder"];
                        JArray arrOCols = (JArray)orderCols;
                        JArray arrONewCols = new JArray();

                        if (arrOCols != null)
                        {
                            int orderClength = arrOCols.Count;

                            if (orderClength > 4)
                            {
                                JToken abVisCols = gridStateJS["abVisCols"];
                                JArray visCols = (JArray)abVisCols;
                                JToken aoSearchCols = gridStateJS["aoSearchCols"];
                                JArray arrSCols = (JArray)aoSearchCols;
                                JToken widthCols = gridStateJS["ColWidth"];
                                JArray arrWCols = (JArray)widthCols;
                                int CurrentUDFPosition = 0;
                                int maxOrder = arrOCols.Select(c => (int)c).ToList().Max();
                                long UDFStartPosition = maxOrder - (TotalSetupCount + ExtraCount);
                                long columnOrderValueToDelete = UDFStartPosition;
                                UDFStartPosition++;

                                if (DataFromDB != null && DataFromDB.Count() > 0)
                                {
                                    DataFromDB = DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).OrderBy(e => e.UpdatedDate).ToList();
                                    for (int udfCount = 0; udfCount < DataFromDB.Count(); udfCount++)
                                    {
                                        columnOrderValueToDelete++;
                                        if (DataFromDB[udfCount].UDFColumnName == udf.UDFColumnName)
                                        {
                                            break;
                                        }
                                    }
                                }

                                for (int i = 0; i < orderClength; i++)
                                {
                                    if (Convert.ToInt32(((JValue)(arrOCols[i])).Value) == columnOrderValueToDelete)
                                    {
                                        CurrentUDFPosition = i;
                                        break;
                                    }
                                }

                                for (int i = 0; i < orderClength; i++)
                                {
                                    if (Convert.ToInt32(((JValue)(arrOCols[i])).Value) > columnOrderValueToDelete)
                                    {
                                        ((JValue)(arrOCols[i])).Value = Convert.ToInt32(((JValue)(arrOCols[i])).Value) - 1;
                                    }
                                }

                                arrOCols.RemoveAt(CurrentUDFPosition);
                                visCols.RemoveAt(Convert.ToInt32(columnOrderValueToDelete)); // for visCols if column 24 is at 3rd position then in visibility we have to remove 24 not 3rd.
                                arrSCols.RemoveAt(CurrentUDFPosition);
                                arrWCols.RemoveAt(CurrentUDFPosition);
                                gridStateJS["ColReorder"] = arrOCols;
                                gridStateJS["abVisCols"] = visCols;
                                gridStateJS["aoSearchCols"] = arrSCols;
                                gridStateJS["ColWidth"] = arrWCols;
                                string updatedJSON = gridStateJS.ToString();

                                /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                                objDTO = new UsersUISettingsDTO();
                                objDTO.UserID = SessionHelper.UserID;
                                objDTO.EnterpriseID = SessionHelper.EnterPriceID;
                                objDTO.CompanyID = SessionHelper.CompanyID;
                                objDTO.RoomID = SessionHelper.RoomID;
                                objDTO.JSONDATA = updatedJSON;
                                objDTO.ListName = ListName;
                                obj.SaveUserListViewSettings(objDTO, SiteSettingHelper.UsersUISettingType, true);
                                /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                            }
                        }
                    }

                }

            }
            }
            catch { }
        }
        private void RemoveUDFColumnOrderInGridNg(string ListName, long ExtraCount, long TotalSetupCount, List<UDFDTO> DataFromDB, UDFDTO udf)
        {
            try
            {
                if (!string.IsNullOrEmpty(ListName))
            {
                eTurnsMaster.DAL.UsersUISettingsDAL obj = new eTurnsMaster.DAL.UsersUISettingsDAL();
                UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
                objDTO = obj.GetRecordWithoutCacheNg(SessionHelper.UserID, ListName, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);

                if (objDTO != null)
                {
                    if (!string.IsNullOrEmpty(objDTO.JSONDATA))
                    {
                        JObject gridStateJS = new Newtonsoft.Json.Linq.JObject();
                        // jsonData = objDTO.JSONDATA;
                        /*////////CODE FOR UPDATE JSON STRING/////////*/
                        // JObject gridStaeJS = new JObject();
                        gridStateJS = JObject.Parse(objDTO.JSONDATA);
                        /*////////CODE FOR UPDATE JSON STRING/////////*/
                        JToken orderCols = gridStateJS["ColReorder"];
                        JArray arrOCols = (JArray)orderCols;
                        JArray arrONewCols = new JArray();

                        if (arrOCols != null)
                        {
                            int orderClength = arrOCols.Count;

                            if (orderClength > 4)
                            {
                                JToken aoSearchCols = gridStateJS["columns"];
                                JArray arrSCols = (JArray)aoSearchCols;
                                int CurrentUDFPosition = 0;
                                int maxOrder = arrOCols.Select(c => (int)c).ToList().Max();
                                long UDFStartPosition = maxOrder - (TotalSetupCount + ExtraCount);
                                long columnOrderValueToDelete = UDFStartPosition;
                                UDFStartPosition++;

                                if (DataFromDB != null && DataFromDB.Count() > 0)
                                {
                                    DataFromDB = DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).OrderBy(e => e.UpdatedDate).ToList();
                                    for (int udfCount = 0; udfCount < DataFromDB.Count(); udfCount++)
                                    {
                                        columnOrderValueToDelete++;
                                        if (DataFromDB[udfCount].UDFColumnName == udf.UDFColumnName)
                                        {
                                            break;
                                        }
                                    }
                                }

                                for (int i = 0; i < orderClength; i++)
                                {
                                    if (Convert.ToInt32(((JValue)(arrOCols[i])).Value) == columnOrderValueToDelete)
                                    {
                                        CurrentUDFPosition = i;
                                        break;
                                    }
                                }

                                for (int i = 0; i < orderClength; i++)
                                {
                                    if (Convert.ToInt32(((JValue)(arrOCols[i])).Value) > columnOrderValueToDelete)
                                    {
                                        ((JValue)(arrOCols[i])).Value = Convert.ToInt32(((JValue)(arrOCols[i])).Value) - 1;
                                    }
                                }

                                arrOCols.RemoveAt(CurrentUDFPosition);
                                arrSCols.RemoveAt(CurrentUDFPosition);
                                gridStateJS["ColReorder"] = arrOCols;
                                gridStateJS["columns"] = arrSCols;
                                string updatedJSON = gridStateJS.ToString(Newtonsoft.Json.Formatting.None);

                                /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                                objDTO = new UsersUISettingsDTO();
                                objDTO.UserID = SessionHelper.UserID;
                                objDTO.EnterpriseID = SessionHelper.EnterPriceID;
                                objDTO.CompanyID = SessionHelper.CompanyID;
                                objDTO.RoomID = SessionHelper.RoomID;
                                objDTO.JSONDATA = updatedJSON;
                                objDTO.ListName = ListName;
                                obj.SaveUserListViewSettings(objDTO, "Angular", true);
                                /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                            }
                        }
                    }

                }

            }
            }
            catch { }
        }
        private string GetListNameFromTable(string TableName)
        {
            string ListName = string.Empty;
            switch (TableName.ToLower())
            {
                case "assetcategorymaster":
                    ListName = "AssetCategoryList";
                    break;
                case "assetmaster":
                    ListName = "AssetMasterList";
                    break;
                case "binmaster":
                    ListName = "BinMasterList";
                    break;
                case "cartitem":
                    ListName = "CartItemDetails";
                    break;
                case "cartitemlist":
                    ListName = "CartItemList";
                    break;
                case "categorymaster":
                    ListName = "CategoryMasterList";
                    break;
                case "companymaster":
                    ListName = "CompanyList";
                    break;
                case "costuommaster":
                    ListName = "CostUOMMasterList";
                    break;
                case "freighttypemaster":
                    ListName = "FreightTypeMasterList";
                    break;
                case "glaccountmaster":
                    ListName = "GLAccountMasterList";
                    break;
                case "gxprconsigmentjobmaster":
                    ListName = "GXPRConsignedJobMasterList";
                    break;
                case "inventoryclassificationmaster":
                    ListName = "InventoryClassificationMasterList";
                    break;
                case "inventorycount":
                    ListName = "InventoryCountList";
                    break;
                case "receivedordertransferdetail":
                    ListName = "itemlocationlist";
                    break;
                case "itemmaster":
                    ListName = "ItemMasterList";
                    break;
                case "kitmaster":
                    ListName = "KitMasterList";
                    break;
                case "locationmaster":
                    ListName = "LocationMasterList";
                    break;
                case "manufacturermaster":
                    ListName = "ManufacturerMasterList";
                    break;
                case "materialstaging":
                    ListName = "MaterialStaging";
                    break;
                case "measurementterm":
                    ListName = "MeasurementTermList";
                    break;
                case "orderdetails":
                    ListName = "OrderLineItemList";
                    break;
                case "ordermaster":
                    ListName = "OrderMasterList";
                    break;
                case "projectmaster":
                    ListName = "ProjectList";
                    break;
                case "pullmaster":
                    ListName = "PullMaster";
                    break;
                case "newpullitemmaster":
                    ListName = "NewPullItemMaster";
                    break;
                case "newconsumepullitemmaster":
                    ListName = "NewConsumePullItemMaster";
                    break;
                case "workorderdetails":
                    ListName = "WorkOrderDetails";
                    break;
                case "itemmastermodellist_rq":
                    ListName = "ItemMasterModelList_RQ";
                    break;
                case "requisitiondetails":
                    ListName = "RequisitionDetails";
                    break;
                case "PullPoMaster":
                    ListName = "NewPullItemMaster";
                    break;
                case "quicklistmaster":
                    ListName = "QuickList";
                    break;
                case "quicklistitems":
                    ListName = "QuickListDetail";
                    break;
                case "requisitionmaster":
                    ListName = "RequisitionMaster";
                    break;
                case "room":
                    ListName = "RoomList";
                    break;
                case "shipviamaster":
                    ListName = "ShipViaMasterList";
                    break;
                case "suppliermaster":
                    ListName = "SupplierMasterList";
                    break;
                case "technicianmaster":
                    ListName = "TechnicianList";
                    break;
                case "toolcategorymaster":
                    ListName = "ToolCategoryList";
                    break;
                case "toolmaster":
                case "toolcheckinouthistory":
                    ListName = "ToolList";
                    break;
                case "checkincheckoutlist":
                    ListName = "CheckinCheckOutList";
                    break;
                case "toollistinmodel":
                    ListName = "ToolListInModel";
                    break;
                case "toolschedulemapping":
                    ListName = "ToolScheduleMappingList";
                    break;
                case "toolsmaintenancelist":
                    ListName = "ToolsMaintenanceList";
                    break;
                case "toolsmaintenancelistdue":
                    ListName = "ToolsMaintenanceListDue";
                    break;
                case "transfermaster":
                    ListName = "TransferMasterList";
                    break;
                case "unitmaster":
                    ListName = "UnitMasterList";
                    break;
                case "usermaster":
                    ListName = "UserList";
                    break;
                case "vendermaster":
                    ListName = "VenderMasterList";
                    break;
                case "workorder":
                    ListName = "WorkOrder";
                    break;
                case "quotemaster":
                    ListName = "QuoteMasterList";
                    break;
                case "quotedetails":
                    ListName = "QuoteDetailList";
                    break;
                default:
                    ListName = string.Empty;
                    break;
            }

            return ListName;
        }


    }

    /// <summary>
    /// UDFControlTypes
    /// </summary>
    public enum UDFControlTypes
    {
        Textbox = 0,
        Dropdown = 1,
        Dropdown_Editable = 2
    }
    public class UDFBulkInsertRequest
    {
        public string UDFTableName { get; set; }
        public List<UDFBulkItem> Items { get; set; }
    }

    //public class UDFBulkItem
    //{
    //    public long UDFID { get; set; }
    //    public string UDFOption { get; set; }
    //    public string UDFColumnName { get; set; }
    //}
}
