using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eTurns.DTO;
using eTurns.DAL;
using System.Net.Http;
using eTurnsWeb.Controllers.WebAPI;
using System.Data.Objects;
using System.Data;
using eTurnsWeb.Helper;
using System.Net;
using eTurns.DTO.Resources;
using System.IO;
using System.Xml;
using System.Resources;
using System.Collections;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class UDFController : eTurnsControllerBase
    {
        public ActionResult UDFList(long? id, string t)
        {
            if (t != null)
            {
                if (eTurnsWeb.Models.UDFDictionaryTables.IsVaidUDFTable(t.ToLower()))
                {
                    string UDFTableName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFTableFromKey(t.ToLower());
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
                    eTurnsMaster.DAL.UDFDAL objUDFDALMaster = new eTurnsMaster.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    if (UDFTableName == "Enterprise")
                    {
                        objUDFDALMaster.InsertDefaultUDFs(UDFTableName, SessionHelper.UserID, id ?? 0);
                    }
                    else
                    {
                        obj.InsertDefaultUDFs(UDFTableName, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomID);
                       
                    }
                    int ResourcePageId = SessionHelper.GetUDFListID(UDFTableName);
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
        public ActionResult UDFListEnterprise(long? id)
        {
            string t = "enterprises";
            if (t != null)
            {
                if (eTurnsWeb.Models.UDFDictionaryTables.IsVaidUDFTable(t.ToLower()))
                {
                    string UDFTableName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFTableFromKey(t.ToLower());
                    ViewBag.UDFTableName = UDFTableName;
                    ViewBag.UDFTableNameKey = t;
                    //UDFApiController obj = new UDFApiController();
                    UDFDAL obj = new UDFDAL(SessionHelper.EnterPriseDBName);
                    eTurnsMaster.DAL.UDFDAL objUDFDALMaster = new eTurnsMaster.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
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

            if (t.ToLower() == "companymaster")
                objDTO.CompanyID = 0;

            if (t != null)
            {
                objDTO.UDFTableName = t;
                ViewBag.UDFTableName = t;
            }
            return PartialView("_CreateUDF", objDTO);
        }

        [HttpPost]
        public JsonResult UDFSave(UDFDTO objDTO)
        {
            string message = "";
            string status = "";
            string t =objDTO.ValueString;
            JavaScriptSerializer objJSSerial = new JavaScriptSerializer();
           List<UDFOptionsDTO> jResult = GetUDFOptionsByUDFNew(objDTO.ID, objDTO.UDFTableName, SessionHelper.EnterPriceID);

           List<UDFOptionsDTO> NewString = JsonConvert.DeserializeObject<List<UDFOptionsDTO>>(t);
           //Dictionary<string, string> NewString = test.ToDictionary( g => g.Key, g => g.va    );
          bool isExists = true;
          int ResourcePageId = SessionHelper.GetUDFListID(objDTO.UDFTableName);
            foreach(UDFOptionsDTO u in jResult)
            {
                isExists = false;
                foreach (UDFOptionsDTO a in NewString)
                {
                    if (u.ID == a.ID)
                    {
                        isExists = true;
                        if (u.UDFOption.Trim() != a.UDFOption.Trim())
                        {
                            EditUDFOption(u.ID, u.UDFID, a.UDFOption, objDTO.UDFTableName, 0);
                        }
                    }
                }
                if(!isExists)
                {
                    DeleteUDFOption(u.ID);
                }
            }
            foreach (UDFOptionsDTO a in NewString)
            {
                if (a.ID == 0)
                {
                    InsertUDFOption(objDTO.ID, a.UDFOption, objDTO.UDFTableName, 0);
                }
            }
            //UDFApiController obj = new UDFApiController();
            if (objDTO.UDFTableName.ToLower() == "enterprise")
            {
                eTurnsMaster.DAL.UDFDAL obj = new eTurnsMaster.DAL.UDFDAL(SessionHelper.EnterPriseDBName);

                if (string.IsNullOrEmpty(objDTO.UDFColumnName))
                {
                    message = "UDFColumnName is required.";
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(objDTO.UDFDisplayColumnName))
                {
                    message = "UDFDisplayColumnName is required.";
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(objDTO.UDFPDADisplayColumnName) && ResourcePageId > 0)
                {
                    message = "UDFPDADisplayColumnName is required.";
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }

                if (objDTO.ID == 0)
                {
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.GUID = Guid.NewGuid();
                    long ReturnVal = obj.Insert(objDTO);

                    eTurns.DTO.Resources.ResourceHelper resHelper = new eTurns.DTO.Resources.ResourceHelper();
                    string culter = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                    resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false);
                    UDFDAL objUDF = new UDFDAL(SessionHelper.EnterPriseDBName);
                    List<string> ReportResourceFileNameList = objUDF.getReportResourceFileName(ResFileName);
                    if (ReportResourceFileNameList != null && ReportResourceFileNameList.Count() > 0)
                    {
                        foreach (string ReportResourceFileName in ReportResourceFileNameList)
                        {
                            resHelper.SaveResourcesByKey(ReportResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false);
                            string strFilePathWithFileName = resHelper.getFilePath(ReportResourceFileName, true);
                            string actulFilename = ReportResourceFileName + "." + culter + ".resx";
                            if (System.IO.File.Exists(strFilePathWithFileName))
                            {
                                System.IO.File.Delete(strFilePathWithFileName);
                            }
                            if (culter == "en-US")
                                culter = "";
                            else
                                culter = "." + culter;
                            string strFilePath = strFilePathWithFileName.Replace(actulFilename, ResFileName + culter + ".resx");
                            string FileTobeCopied = strFilePathWithFileName.Replace(ReportResourceFileName, ResFileName);
                            System.IO.File.Copy(FileTobeCopied, strFilePathWithFileName);

                        }
                    }
                   
                    if (ResourcePageId > 0)
                    {
                        int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                        MobileResourcesDAL objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                        List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(SessionHelper.RoomID, SessionHelper.CompanyID, languageId, ResourcePageId).ToList();
                        long UDfId = 0;
                        if (MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                        {
                            UDfId = MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ID;
                        }
                        
                        if (UDfId > 0)
                        {
                            MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();
                            mobileResourceDTO = MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault();
                            mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                            mobileResourceDTO.Roomid = SessionHelper.RoomID;
                            mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                            mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                            objMobileResourcesDAL.Edit(mobileResourceDTO);
                        }
                        else
                        {
                            MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();
                            mobileResourceDTO.Roomid = SessionHelper.RoomID;
                            mobileResourceDTO.ResourceKey = objDTO.UDFColumnName;
                            mobileResourceDTO.ResourcePageID = ResourcePageId;
                            mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                            mobileResourceDTO.LanguageID = 1;
                            mobileResourceDTO.CompanyID = SessionHelper.CompanyID;
                            mobileResourceDTO.CreatedBy = SessionHelper.UserID;
                            mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                            mobileResourceDTO.CreatedOn = DateTimeUtility.DateTimeNow;
                            mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                            mobileResourceDTO.LanguageID = languageId;
                            long ReturnmobileResourceVal = objMobileResourcesDAL.Insert(mobileResourceDTO);
                          
                        }
                    }
                    if (ReturnVal > 0 )
                    {
                        message = "Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
                else
                {
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    bool ReturnVal = obj.Edit(objDTO);
                    try
                    {
                        eTurns.DTO.Resources.ResourceHelper resHelper = new eTurns.DTO.Resources.ResourceHelper();
                        string culter = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                        //if (objDTO != null && objDTO.IsDeleted)
                        //{
                        //    objDTO.UDFDisplayColumnName = objDTO.UDFColumnName;//if udf is deleted then reset values
                        //}
                        resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false);
                        UDFDAL objUDF = new UDFDAL(SessionHelper.EnterPriseDBName);
                        List<string> ReportResourceFileNameList = objUDF.getReportResourceFileName(ResFileName);
                        if (ReportResourceFileNameList != null && ReportResourceFileNameList.Count() > 0)
                        {
                            foreach (string ReportResourceFileName in ReportResourceFileNameList)
                            {
                                resHelper.SaveResourcesByKey(ReportResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false);
                                string strFilePathWithFileName = resHelper.getFilePath(ReportResourceFileName, true);
                                string actulFilename = ReportResourceFileName + "." + culter + ".resx";
                                if (System.IO.File.Exists(strFilePathWithFileName))
                                {
                                    System.IO.File.Delete(strFilePathWithFileName);
                                }
                                if (culter == "en-US")
                                    culter = "";
                                else
                                    culter = "." + culter;
                                string strFilePath = strFilePathWithFileName.Replace(actulFilename, ResFileName + culter + ".resx");
                                string FileTobeCopied = strFilePathWithFileName.Replace(ReportResourceFileName, ResFileName);
                                System.IO.File.Copy(FileTobeCopied, strFilePathWithFileName);
                            }
                        }
                         ResourcePageId = SessionHelper.GetUDFListID(objDTO.UDFTableName);
                        if (ResourcePageId > 0)
                        {
                            int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                            MobileResourcesDAL objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                            List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(SessionHelper.RoomID, SessionHelper.CompanyID, languageId, ResourcePageId).ToList();
                            long UDfId = 0;
                            if (MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                            {
                                UDfId = MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ID;
                            }

                            if (UDfId > 0)
                            {
                                MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();
                                mobileResourceDTO = MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault();
                                mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                                mobileResourceDTO.Roomid = SessionHelper.RoomID;
                                mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                                mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                                objMobileResourcesDAL.Edit(mobileResourceDTO);
                            }
                            else
                            {
                                MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();
                                mobileResourceDTO.Roomid = SessionHelper.RoomID;
                                mobileResourceDTO.ResourceKey = objDTO.UDFColumnName;
                                mobileResourceDTO.ResourcePageID = ResourcePageId;
                                mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                                mobileResourceDTO.LanguageID = 1;
                                mobileResourceDTO.CompanyID = SessionHelper.CompanyID;
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
                        message = "Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {


                UDFDAL obj = new UDFDAL(SessionHelper.EnterPriseDBName);

                if (string.IsNullOrEmpty(objDTO.UDFColumnName))
                {
                    message = "UDFColumnName is required.";
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(objDTO.UDFDisplayColumnName))
                {
                    message = "UDFDisplayColumnName is required.";
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(objDTO.UDFPDADisplayColumnName) && ResourcePageId > 0)
                {
                    message = "UDFPDADisplayColumnName is required.";
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
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
                    long ReturnVal = obj.Insert(objDTO);

                    eTurns.DTO.Resources.ResourceHelper resHelper = new eTurns.DTO.Resources.ResourceHelper();
                    string culter = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                    resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, isUDFName);

                    UDFDAL objUDF = new UDFDAL(SessionHelper.EnterPriseDBName);
                    List<string> ReportResourceFileNameList = objUDF.getReportResourceFileName(ResFileName);
                    if (ReportResourceFileNameList != null && ReportResourceFileNameList.Count() > 0)
                    {
                        foreach (string ReportResourceFileName in ReportResourceFileNameList)
                        {
                            resHelper.SaveResourcesByKey(ReportResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false);
                            string strFilePathWithFileName = resHelper.getFilePath(ReportResourceFileName, true);
                            string actulFilename = ReportResourceFileName + "." + culter + ".resx";
                            if (System.IO.File.Exists(strFilePathWithFileName))
                            {
                                System.IO.File.Delete(strFilePathWithFileName);
                            }
                            if (culter == "en-US")
                                culter = "";
                            else
                                culter = "." + culter;
                            string strFilePath = strFilePathWithFileName.Replace(actulFilename, ResFileName + culter + ".resx");
                            string FileTobeCopied = strFilePathWithFileName.Replace(ReportResourceFileName, ResFileName);
                            System.IO.File.Copy(FileTobeCopied, strFilePathWithFileName);
                        }
                    }
                      ResourcePageId = SessionHelper.GetUDFListID(objDTO.UDFTableName);
                     if (ResourcePageId > 0)
                     {
                         int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                         MobileResourcesDAL objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                         List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(SessionHelper.RoomID, SessionHelper.CompanyID, languageId, ResourcePageId).ToList();
                         long UDfId = 0;
                         if (MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                         {
                             UDfId = MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ID;
                         }

                         if (UDfId > 0)
                         {
                             MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();
                             mobileResourceDTO = MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault();
                             mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                             mobileResourceDTO.Roomid = SessionHelper.RoomID;
                             mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                             mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                             objMobileResourcesDAL.Edit(mobileResourceDTO);
                         }
                         else
                         {
                             MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();
                             mobileResourceDTO.Roomid = SessionHelper.RoomID;
                             mobileResourceDTO.ResourceKey = objDTO.UDFColumnName;
                             mobileResourceDTO.ResourcePageID = ResourcePageId;
                             mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                             mobileResourceDTO.LanguageID = 1;
                             mobileResourceDTO.CompanyID = SessionHelper.CompanyID;
                             mobileResourceDTO.CreatedBy = SessionHelper.UserID;
                             mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                             mobileResourceDTO.CreatedOn = DateTimeUtility.DateTimeNow;
                             mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;

                             long ReturnmobileResourceVal = objMobileResourcesDAL.Insert(mobileResourceDTO);
                         }
                     }
                    if (ReturnVal > 0)
                    {
                        message = "Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
                else
                {
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    bool ReturnVal = obj.Edit(objDTO);
                    try
                    {
                        eTurns.DTO.Resources.ResourceHelper resHelper = new eTurns.DTO.Resources.ResourceHelper();
                        string culter = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        string ResFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                        //if (objDTO != null && objDTO.IsDeleted)
                        //{
                        //    objDTO.UDFDisplayColumnName = objDTO.UDFColumnName;//if udf is deleted then reset values
                        //}
                        resHelper.SaveResourcesByKey(ResFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, isUDFName);
                        UDFDAL objUDF = new UDFDAL(SessionHelper.EnterPriseDBName);
                        List<string> ReportResourceFileNameList = objUDF.getReportResourceFileName(ResFileName);
                        if (ReportResourceFileNameList != null && ReportResourceFileNameList.Count() > 0)
                        {
                            foreach (string ReportResourceFileName in ReportResourceFileNameList)
                            {
                                resHelper.SaveResourcesByKey(ReportResourceFileName, culter, objDTO.UDFColumnName, objDTO.UDFDisplayColumnName, false);
                                string strFilePathWithFileName = resHelper.getFilePath(ReportResourceFileName, true);
                                string actulFilename = ReportResourceFileName + "." + culter + ".resx";
                                if (System.IO.File.Exists(strFilePathWithFileName))
                                {
                                    System.IO.File.Delete(strFilePathWithFileName);
                                }
                                if (culter == "en-US")
                                    culter = "";
                                else
                                    culter = "." + culter;
                                string strFilePath = strFilePathWithFileName.Replace(actulFilename, ResFileName + culter + ".resx");
                                string FileTobeCopied = strFilePathWithFileName.Replace(ReportResourceFileName, ResFileName);
                                System.IO.File.Copy(FileTobeCopied, strFilePathWithFileName);
                            }
                        }
                         ResourcePageId = SessionHelper.GetUDFListID(objDTO.UDFTableName);
                        if (ResourcePageId > 0)
                        {
                            int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                            MobileResourcesDAL objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                            List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(SessionHelper.RoomID, SessionHelper.CompanyID, languageId, ResourcePageId).ToList();
                            long UDfId = 0;
                            if (MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                            {
                                UDfId = MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ID;
                            }

                            if (UDfId > 0)
                            {
                                MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();
                                mobileResourceDTO = MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault();
                                mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                                mobileResourceDTO.Roomid = SessionHelper.RoomID;
                                mobileResourceDTO.UpdatedBy = SessionHelper.UserID;
                                mobileResourceDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                                objMobileResourcesDAL.Edit(mobileResourceDTO);
                            }
                            else
                            {
                                MobileResourcesDTO mobileResourceDTO = new MobileResourcesDTO();
                                mobileResourceDTO.Roomid = SessionHelper.RoomID;
                                mobileResourceDTO.ResourceKey = objDTO.UDFColumnName;
                                mobileResourceDTO.ResourcePageID = ResourcePageId;
                                mobileResourceDTO.ResourceValue = objDTO.UDFPDADisplayColumnName;
                                mobileResourceDTO.LanguageID = 1;
                                mobileResourceDTO.CompanyID = SessionHelper.CompanyID;
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
                        message = "Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
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
        public ActionResult UDFEdit(Int64 ID, string UDFTableKeyName)
        {
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
                eTurnsMaster.DAL.UDFDAL obj = new eTurnsMaster.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                UDFDTO objDTO = obj.GetRecord(ID, SessionHelper.CompanyID);
                string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                //Type t = Type.GetType(UDFTableResourceFileName);
                string val = ResourceUtils.GetResource(UDFTableResourceFileName, objDTO.UDFColumnName, false);
                if (!string.IsNullOrEmpty(val))
                    objDTO.UDFDisplayColumnName = val;
                else
                    objDTO.UDFDisplayColumnName = objDTO.UDFColumnName;

                if (objDTO != null)
                {
                    objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                }
                return PartialView("_CreateUDF", objDTO);
            }
            else
            {
                long companyID = 0;
                if (UDFTableKeyName.ToLower() != "companies")
                    companyID = SessionHelper.CompanyID;

                bool isUDFName = false;
                if (UDFTableKeyName.ToLower() != "companies" && UDFTableKeyName.ToLower() != "room")
                    isUDFName = true;

                UDFDAL obj = new UDFDAL(SessionHelper.EnterPriseDBName);
                UDFDTO objDTO = obj.GetRecord(ID, companyID);
                string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(objDTO.UDFTableName);
                //Type t = Type.GetType(UDFTableResourceFileName);
                string val = ResourceUtils.GetResource(UDFTableResourceFileName, objDTO.UDFColumnName, isUDFName);
                if (!string.IsNullOrEmpty(val))
                    objDTO.UDFDisplayColumnName = val;
                else
                    objDTO.UDFDisplayColumnName = objDTO.UDFColumnName;

                if (objDTO != null)
                {
                    objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                     ResourcePageId = SessionHelper.GetUDFListID(objDTO.UDFTableName);
                    string UdfMobileResource = string.Empty;
                    if (ResourcePageId > 0)
                    {
                        MobileResourcesDAL objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                        int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                        List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(SessionHelper.RoomID, SessionHelper.CompanyID, languageId, ResourcePageId).ToList();
                        

                        if (MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).Any())
                        {
                            UdfMobileResource = MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == objDTO.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ResourceValue;
                        }
                        else
                        {
                            UdfMobileResource = objDTO.UDFColumnName;
                        }
                    }
                    else
                    {
                        UdfMobileResource = objDTO.UDFColumnName;
                    }
                    objDTO.UDFPDADisplayColumnName = UdfMobileResource;
                }

                return PartialView("_CreateUDF", objDTO);
            }
            //UDFApiController obj = new UDFApiController();

        }


        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult UDFListAjax(JQueryDataTableParamModel param, string t)
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
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            IEnumerable<UDFDTO> DataFromDB = null;
            if (!string.IsNullOrEmpty(t))
            {
                if (t.ToLower() == "enterprise")
                {
                    eTurnsMaster.DAL.UDFDAL objMasterUDFDAL = new eTurnsMaster.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    DataFromDB = objMasterUDFDAL.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.CompanyID, t, EID);
                }
                else
                {
                    DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.CompanyID, t, SessionHelper.RoomID);
                }
            }
            else
            {
                DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.CompanyID, t, SessionHelper.RoomID);
            }
            foreach (var item in DataFromDB)
            {
                if (t.ToLower() == "enterprise")
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(item.UDFTableName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, false);
                    if (!string.IsNullOrEmpty(val))
                        item.UDFDisplayColumnName = val;
                    else
                        item.UDFDisplayColumnName = item.UDFColumnName;
                }
                else if (t.ToLower() == "companymaster" || t.ToLower() == "room")
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(item.UDFTableName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, false);
                    if (!string.IsNullOrEmpty(val))
                        item.UDFDisplayColumnName = val;
                    else
                        item.UDFDisplayColumnName = item.UDFColumnName;
                }
                else
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(item.UDFTableName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, true);
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
              int ResourcePageId=SessionHelper.GetUDFListID(udf.UDFTableName);
                string UdfMobileResource = string.Empty;
              if (ResourcePageId > 0)
              {
                  MobileResourcesDAL objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                  int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                  List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(SessionHelper.RoomID, SessionHelper.CompanyID, languageId, ResourcePageId).ToList();
                

                  if (MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == udf.UDFColumnName && i.LanguageID == languageId).Any())
                  {
                      UdfMobileResource = MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == udf.UDFColumnName && i.LanguageID == languageId).FirstOrDefault().ResourceValue;
                  }
                  else
                  {
                      UdfMobileResource = udf.UDFColumnName;
                      udf.showPDAField = true;
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
                             showPDAField = c.showPDAField
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
                obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
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
        public JsonResult GetUDFOptionsByUDF(Int64 UDFID, string UDFTableName, long? EnterpriseID)
        {
            //UDFOptionApiController obj = new UDFOptionApiController();
            if (!string.IsNullOrEmpty(UDFTableName) && UDFTableName.ToLower() == "enterprise")
            {
                eTurnsMaster.DAL.UDFOptionDAL objUDFOptionDAL = new eTurnsMaster.DAL.UDFOptionDAL(SessionHelper.EnterPriseDBName);
                var data = objUDFOptionDAL.GetUDFOptionsByUDF(UDFID).OrderBy(e => e.UDFOption);
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
                else
                    obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);

                var data = obj.GetUDFOptionsByUDF(UDFID, SessionHelper.CompanyID).OrderBy(e => e.UDFOption);
                return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
            }
        }
        public List<UDFOptionsDTO> GetUDFOptionsByUDFNew(Int64 UDFID, string UDFTableName, long? EnterpriseID)
        {
            //UDFOptionApiController obj = new UDFOptionApiController();
            if (!string.IsNullOrEmpty(UDFTableName) && UDFTableName.ToLower() == "enterprise")
            {
                eTurnsMaster.DAL.UDFOptionDAL objUDFOptionDAL = new eTurnsMaster.DAL.UDFOptionDAL(SessionHelper.EnterPriseDBName);
                var data = objUDFOptionDAL.GetUDFOptionsByUDF(UDFID).OrderBy(e => e.UDFOption);
                return data.ToList();
            }
            else
            {
                string EnterpriseDBName = string.Empty;
                if (EnterpriseID != null && EnterpriseID > 0)
                    EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(EnterpriseID ?? 0);

                UDFOptionDAL obj;
                if (!string.IsNullOrWhiteSpace(EnterpriseDBName))
                    obj = new UDFOptionDAL(EnterpriseDBName);
                else
                    obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);

                var data = obj.GetUDFOptionsByUDF(UDFID, SessionHelper.CompanyID).OrderBy(e => e.UDFOption);

                return data.ToList();
            }
        }

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
            List<UDFOptionsDTO> data = obj.GetUDFOptionsByUDF(UDFID, SessionHelper.CompanyID).OrderBy(e => e.UDFOption).Where(t => !string.IsNullOrWhiteSpace(t.UDFOption) && t.UDFOption.ToLower().Contains(name_startsWith.ToLower().Trim())).Take(maxRows).ToList();

            if (data.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public List<UDFOptionsDTO> GetUDFOptionsByUDFForEditable(Int64 UDFID)
        {
            //UDFOptionApiController obj = new UDFOptionApiController();
            UDFOptionDAL obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
            var data = obj.GetUDFOptionsByUDF(UDFID, SessionHelper.CompanyID).OrderBy(e => e.UDFOption).ToList();
            return data;
        }

        public JsonResult InsertUDFOption(Int64 UDFID, string UDFOption, string UDFTableName, long? EnterpriseID)
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
                    eTurnsMaster.DAL.UDFOptionDAL obj = new eTurnsMaster.DAL.UDFOptionDAL(SessionHelper.EnterPriseDBName);
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
                CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDAL.DuplicateUFDOptionCheck(UDFOption, "add", 0, "UDFOptions", "UDFOption", UDFID);
                if (strOK == "duplicate")
                {
                    return Json(new { Response = strOK }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //UDFOptionApiController obj = new UDFOptionApiController();
                    UDFOptionDAL obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
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
        }

        public JsonResult EditUDFOption(Int64 ID, Int64 UDFID, string UDFOption, string UDFTableName, long? EnterpriseID)
        {
            //UDFOptionApiController obj = new UDFOptionApiController();
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string strOK = objCDAL.DuplicateUFDOptionCheck(UDFOption, "edit", ID, "UDFOptions", "UDFOption", UDFID);
            if (strOK == "duplicate")
            {
                return Json(new { Response = strOK }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                UDFOptionDAL obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
                var ResponseStatus = obj.Edit(ID, UDFOption, SessionHelper.UserID, SessionHelper.CompanyID);
                return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteUDFOption(Int64 ID)
        {
            //UDFOptionApiController obj = new UDFOptionApiController();
            UDFOptionDAL obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
            var ResponseStatus = obj.Delete(ID, SessionHelper.UserID);
            return Json(new { Response = ResponseStatus }, JsonRequestBehavior.AllowGet);
        }

        public object GetUDFDataPageWise(string PageName)
        {
            IEnumerable<UDFDTO> DataFromDB = null;
            if (!string.IsNullOrWhiteSpace(PageName) && PageName.ToLower() == "enterprise")
            {
                eTurnsMaster.DAL.UDFDAL objUDFApiController = new eTurnsMaster.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                DataFromDB = objUDFApiController.GetAllRecords(PageName).OrderBy(e => e.UDFColumnName);
            }
            else
            {
                UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                DataFromDB = objUDFApiController.GetAllRecords(SessionHelper.CompanyID, PageName, SessionHelper.RoomID).OrderBy(e => e.UDFColumnName);
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

        public object GetUDFDataPageWise(string PageName, long ID)
        {
            IEnumerable<UDFDTO> DataFromDB = null;
            if (!string.IsNullOrWhiteSpace(PageName) && PageName.ToLower() == "enterprise")
            {
                eTurnsMaster.DAL.UDFDAL objUDFApiController = new eTurnsMaster.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                DataFromDB = objUDFApiController.GetAllRecords(PageName, ID).OrderBy(e => e.UDFColumnName);
            }
            else
            {
                UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                DataFromDB = objUDFApiController.GetAllRecords(SessionHelper.CompanyID, PageName, SessionHelper.RoomID).OrderBy(e => e.UDFColumnName);
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
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(SessionHelper.CompanyID, PageName, SessionHelper.RoomID).OrderBy(e => e.UDFColumnName);

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

        public object GetCompanyMasterUDFDataPageWise(string DatabaseName, long CompanyID, string PageName)
        {
            IEnumerable<UDFDTO> DataFromDB = null;

            UDFDAL objUDFApiController = new UDFDAL(DatabaseName);
            DataFromDB = objUDFApiController.GetAllCompanyUDFRecords(0, PageName).OrderBy(e => e.UDFColumnName);

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

        public object GetRoomMasterUDFDataPageWise(string DatabaseName, long CompanyID, string PageName)
        {
            IEnumerable<UDFDTO> DataFromDB = null;
            UDFDAL objUDFApiController = new UDFDAL(DatabaseName);
            DataFromDB = objUDFApiController.GetAllCompanyUDFRecords(CompanyID, PageName).OrderBy(e => e.UDFColumnName);
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
                    return Json(new { Status = false, Message = "You are not able to update" }, JsonRequestBehavior.AllowGet);
                }
                string msg = string.Empty;
                eTurnsMaster.DAL.EnterpriseMasterDAL objDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                List<EnterpriseDTO> lstEnterprise = new List<EnterpriseDTO>();
                lstEnterprise = objDAL.GetAllEnterprise(false);
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
                return Json(new { Status = true, Message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);

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
                            Console.WriteLine("Key pair ({0}, {1}) is removed", de.Entry.Key.ToString(), de.Entry.Value.ToString());
                            isKeyExist = true;
                        }
                    }
                    if (!isKeyExist)
                    {
                        Console.WriteLine("No entry found for key: {0}", resEntryKey);
                    }
                }
            }
        }
        //public ActionResult UDFSetupList()
       // {
       //     SortedList UDFLIST= eTurnsWeb.Models.UDFDictionaryTables.GetUDFTables();
        //    ViewBag.UDFTableListName = UDFLIST;
        //    return View();
       // }
       
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

}
