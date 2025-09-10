using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using eTurnsWeb.Helper;
using System.Threading;
using System.Globalization;
using eTurns.DTO.Resources;
using eTurns.DTO;
using System.Net;
using eTurnsWeb.Controllers.WebAPI;
using eTurns.DAL;
using System.Reflection;
using System.Xml;
using System.Resources;
using eTurnsMaster.DAL;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Text;
using System.Data;
using System.Configuration;
using Microsoft.VisualBasic.FileIO;
using System.Data.Odbc;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace eTurnsWeb.Controllers
{
    //[AuthorizeHelper]
    public class SaveResourcesController : eTurnsControllerBase
    {
        string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
        string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);
        List<ResourceLanguageKey> lstResourceLanguageKey = new List<ResourceLanguageKey>();
        private const string _CURRENTBASERESOURCESESSIONKEY = "CURRENTBASERESOURCESESSIONKEY";
        private List<KeyValDTO> CurrentBaseResourceList
        {
            get
            {
                if (HttpContext.Session[_CURRENTBASERESOURCESESSIONKEY] != null)
                    return (List<KeyValDTO>)HttpContext.Session[_CURRENTBASERESOURCESESSIONKEY];
                return new List<KeyValDTO>();
            }
            set
            {
                HttpContext.Session[_CURRENTBASERESOURCESESSIONKEY] = value;
            }
        }
        public ActionResult SaveBaseResource(string lang = "", string module = "", string page = "")
        {
            if (SessionHelper.UserType == 1 && SessionHelper.RoleID == -1)
            {
                CurrentBaseResourceList = null;
                eTurnsBaseResourceDAL BRDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
                List<SelectListItem> lstLang = GeteTurnsResourceLanguage(); //GetBaseLanguage();
                GenerateBaseFilesOfNonExistsOrNewCulture("");


                List<SelectListItem> lstModMast = GetResourceModuleData();
                List<SelectListItem> lstResPage = GetResourceModuleDetail(Convert.ToInt32(lstModMast[0].Value)); //GetBaseResourceFiles(int.Parse(lstModMast[0].Value));

                if (!string.IsNullOrEmpty(lang))
                {
                    foreach (var item in lstLang.ToList())
                    {
                        if (item.Text.ToLower() == lang.ToLower())
                        {
                            item.Selected = true;
                            break;
                        }
                    }

                    foreach (var item in lstModMast)
                    {
                        if (item.Text.ToLower() == module.ToLower())
                        {
                            item.Selected = true;
                            lstResPage = GetResourceModuleDetail(int.Parse(lstModMast[0].Value));  // GeteTurnsBaseResourceModuleDetail(int.Parse(lstModMast[0].Value)); 
                            break;
                        }
                    }

                    foreach (var item in lstResPage)
                    {
                        if (item.Value.ToLower() == page.ToLower())
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }

                ViewBag.DDLanguage = lstLang;
                ViewBag.DDLModuleMaster = lstModMast;
                ViewBag.DDLResourceFile = lstResPage;

                return View();
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }
        private List<SelectListItem> GetETurnsBaseLanguage()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resLangDTO = objDAL.GetETurnsResourceLanguageData();
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.Culture;
                    lstItem.Add(obj);
                    if (!IsCultureFilesExists(item.Culture))
                    {
                        GenerateBaseFilesOfNonExistsOrNewCulture(item.Culture);
                    }
                }
                return lstItem;
            }
            catch (Exception ex)
            {
                return lstItem;
                throw ex;
            }
            finally
            {
                objDAL = null;
                resLangDTO = null;
                lstItem = null;

            }

        }
        private List<SelectListItem> GeteTurnsResourceLanguage()
        {
            eTurnsBaseResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {

                objDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
                resLangDTO = objDAL.GetResourceLanguage();
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.Culture;
                    if (ResourceHelper.CurrentCult.ToString().ToLower() == item.Culture.ToLower())
                        obj.Selected = true;

                    lstItem.Add(obj);
                }
                return lstItem;
            }
            finally
            {
                objDAL = null;
                resLangDTO = null;
                lstItem = null;

            }

        }
        private List<SelectListItem> GetETurnsBaseResourceModule()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleMasterDTO> resModule = null;
            List<SelectListItem> lstItem = null;
            try
            {
                string resFileName = "ResModuleResource";
                objDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
                resModule = objDAL.GetCachedResourceModuleMasterData(0);
                lstItem = new List<SelectListItem>();
                foreach (var item in resModule)
                {
                    SelectListItem obj = new SelectListItem();
                    string resValue = string.Empty;
                    if (!string.IsNullOrEmpty(item.ResModuleKey))
                    {
                        resValue = eTurns.DTO.Resources.ResourceHelper.GetModuleResource(item.ResModuleKey, resFileName);
                        if (!string.IsNullOrEmpty(resValue) && resValue != item.ResModuleKey)
                        {
                            obj.Text = resValue;
                        }
                        else
                        {
                            obj.Text = item.ModuleName;
                        }
                    }
                    else
                    {
                        obj.Text = item.ModuleName;
                    }
                    obj.Value = item.ID.ToString();
                    lstItem.Add(obj);
                }


                return lstItem;
            }
            catch (Exception ex)
            {
                return lstItem;
                throw ex;
            }
            finally
            {
                objDAL = null;
                resModule = null;
                lstItem = null;

            }

        }
        private List<SelectListItem> GetResourceModuleDetail(Int64 ModuleID)
        {
            List<SelectListItem> lstItem = null;
            eTurnsBaseResourceDAL objDAL = null;
            IEnumerable<ResourceModuleDetailsDTO> ResModDet = null;
            try
            {

                objDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
                ResModDet = objDAL.GetResourceModuleDetailData(ModuleID);
                lstItem = new List<SelectListItem>();
                if (ResModDet != null && ResModDet.Count() > 0)
                {
                    foreach (var item in ResModDet)
                    {
                        SelectListItem obj = new SelectListItem();
                        obj.Text = item.DisplayFileName;
                        obj.Value = item.FileName;
                        lstItem.Add(obj);
                    }
                }

                if (lstItem == null || lstItem.Count < 0)
                    lstItem = new List<SelectListItem>();

                return lstItem;

            }
            catch (Exception ex)
            {
                return lstItem;
                throw ex;
            }
            finally
            {
                objDAL = null;
                ResModDet = null;
                lstItem = null;
            }



        }

        private List<SelectListItem> GetETurnsBaseResourceFiles(int ModuleID)
        {
            string resFileName = "ResModuleResource";
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleDetailsDTO> ResModDet = null;
            try
            {
                objDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
                ResModDet = objDAL.GetCachedResourceModuleDetailData(0, ModuleID);
                lstItem = new List<SelectListItem>();
                if (ResModDet != null && ResModDet.Count() > 0)
                {
                    foreach (var item in ResModDet)
                    {
                        SelectListItem obj = new SelectListItem();
                        string resValue = string.Empty;
                        if (!string.IsNullOrEmpty(item.ResModuleDetailsKey))
                        {
                            resValue = eTurns.DTO.Resources.ResourceHelper.GetModuleResource(item.ResModuleDetailsKey, resFileName);
                            if (!string.IsNullOrEmpty(resValue) && resValue != item.ResModuleDetailsKey)
                            {
                                obj.Text = resValue;
                            }
                            else
                            {
                                obj.Text = item.DisplayFileName;
                            }
                        }
                        else
                        {
                            obj.Text = item.DisplayFileName;
                        }
                        //obj.Text = item.DisplayFileName;
                        obj.Value = item.FileName;
                        lstItem.Add(obj);
                    }
                }
                return lstItem;

            }
            catch (Exception ex)
            {
                return lstItem;
                throw ex;
            }
            finally
            {
                objDAL = null;
                ResModDet = null;
                lstItem = null;
            }

        }

        private List<SelectListItem> GetResourceModuleData()
        {
            eTurnsBaseResourceDAL objDAL = null;
            IEnumerable<ResourceModuleMasterDTO> resModule = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
                resModule = objDAL.GetResourceModuleMasterData();
                lstItem = new List<SelectListItem>();
                foreach (var item in resModule)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.ModuleName;
                    obj.Value = item.ID.ToString();
                    lstItem.Add(obj);
                }


                return lstItem;
            }
            catch (Exception ex)
            {
                return lstItem;
                throw ex;
            }
            finally
            {
                objDAL = null;
                resModule = null;
                lstItem = null;

            }

        }


        private bool IsCultureFilesExists(string cultureCode)
        {
            string appPath1 = ResourceHelper.ResourceDirectoryBasePath + @"\";
            string ResourceBaseFilePath = CommonUtility.ResourceBaseFilePath;
            string BasePathResource = ResourceBaseFilePath + @"\";

            string BaseEnterpriseResourcesFolder = BasePathResource + "MasterResources\\EnterpriseResources";
            string BaseCompanyResourcesFolder = BasePathResource + "MasterResources\\CompanyResources";
            string BaseRoomResourcesFolder = BasePathResource + "MasterResources\\RoomResources";

            DirectoryInfo BaseEntResourceDir = new DirectoryInfo(BaseEnterpriseResourcesFolder);
            DirectoryInfo BaseCompResourceDir = new DirectoryInfo(BaseCompanyResourcesFolder);
            DirectoryInfo BaseRoomResourceDir = new DirectoryInfo(BaseRoomResourcesFolder);

            FileInfo[] BaseEntResFiles = BaseEntResourceDir.GetFiles("*." + cultureCode + ".resx");
            FileInfo[] BaseCompResFiles = BaseCompResourceDir.GetFiles("*." + cultureCode + ".resx");
            FileInfo[] BaseRoomResFiles = BaseRoomResourceDir.GetFiles("*." + cultureCode + ".resx");

            if (BaseEntResFiles != null && BaseEntResFiles.Length > 0 && BaseCompResFiles != null && BaseCompResFiles.Length > 0
                        && BaseRoomResFiles != null && BaseRoomResFiles.Length > 0)
                return true;

            return false;
        }
        public void GenerateBaseFilesOfNonExistsOrNewCulture(string cultureCode)
        {
            if (string.IsNullOrEmpty(cultureCode))
                return;

            string appPath1 = ResourceHelper.ResourceDirectoryBasePath + @"\";
            string ResourceBaseFilePath = CommonUtility.ResourceBaseFilePath;
            string BasePathResource = ResourceBaseFilePath + @"\";
            string BaseCompanyResourcesFolder = BasePathResource + "MasterResources\\CompanyResources";
            string BaseEnterpriseResourcesFolder = BasePathResource + "MasterResources\\EnterpriseResources";
            string BaseRoomResourcesFolder = BasePathResource + "MasterResources\\RoomResources";

            CopyBaseFilesByCultureCode(cultureCode, BaseCompanyResourcesFolder);
            CopyBaseFilesByCultureCode(cultureCode, BaseEnterpriseResourcesFolder);
            CopyBaseFilesByCultureCode(cultureCode, BaseRoomResourcesFolder);

        }
        private void CopyBaseFilesByCultureCode(string cultureCode, string BaseResourceDirPath)
        {
            DirectoryInfo BaseResourceDir = new DirectoryInfo(BaseResourceDirPath);
            FileInfo[] BaseAllFiles = BaseResourceDir.GetFiles("*.resx");

            BaseAllFiles = GetOnlyenUSBaseFiles(BaseAllFiles);
            foreach (var CFiles in BaseAllFiles)
            {
                string FileName = CFiles.Name.Replace(CFiles.Extension, "");
                if (!string.IsNullOrEmpty(FileName) && !(System.IO.File.Exists(BaseResourceDir + "\\" + FileName + "." + cultureCode + ".resx")))
                    System.IO.File.Copy(BaseResourceDir + "\\" + FileName + ".resx", BaseResourceDir + "\\" + FileName + "." + cultureCode + ".resx");
            }
        }
        private FileInfo[] GetOnlyenUSBaseFiles(FileInfo[] BaseAllFiles)
        {
            ResourceDAL objResourceDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
            List<ResourceLanguageDTO> objResourceLanguageList = new List<ResourceLanguageDTO>();
            objResourceLanguageList = objResourceDAL.GetResourceLanguageDataFromXML(Convert.ToString(Server.MapPath("/Content/LanguageResource.xml"))).Where(x => x.Culture != "en-US").ToList();
            string[] cultures = objResourceLanguageList.Select(x => x.Culture).ToArray();

            return BaseAllFiles = BaseAllFiles.Where(x =>
            {
                if (Array.Exists(cultures, s => x.Name.Contains(s)) || x.Name.EndsWith("..resx"))
                    return false;
                else
                    return true;
            }).ToArray();
        }
        public string InsertBaseResource(string resourcefile, string resourcelang, string resourceModule)
        {
            eTurnsBaseResourceDAL DAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
            DAL.InsertHistoryLog("eTurnsBaseResource", false);
            if (!string.IsNullOrEmpty(resourcefile) && !string.IsNullOrWhiteSpace(resourcefile))
            {
                string ResourceKey = "";
                string ResourceValue = "";

                if (resourceModule == "Enterprise")
                {
                    // Base Reource for Enterprise Module read from BaseEnterpriseResourceData
                    CurrentBaseResourceList = BaseResourceHelper.GetBaseEnterpriseResourceData(resourcefile, resourcelang);
                }
                else
                {
                    CurrentBaseResourceList = BaseResourceHelper.GetBaseResourceData(resourcefile, resourcelang);
                }

                try
                {
                    for (int i = 0; i < CurrentBaseResourceList.Count(); i++)
                    {
                        ResourceKey = CurrentBaseResourceList[i].key;
                        ResourceValue = CurrentBaseResourceList[i].value;
                        DAL.SaveBaseResourceDetail(ResourceKey, ResourceValue, resourcefile, resourcelang, SessionHelper.UserID, "Web");
                    }
                }
                catch
                {
                    DAL.InsertHistoryLog("eTurnsBaseResource", true);
                }
            }
            DAL.InsertHistoryLog("eTurnsBaseResource", true);
            return ResCommon.MsgDataSaved;
        }
        public string InsertAllBaseResource()
        {
            //ResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            eTurnsBaseResourceDAL BRDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
            IEnumerable<ResourceModuleMasterDTO> lstModMast = BRDAL.GetResourceModuleMasterData();
            string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            string BaseRoomResPath = ResourceBaseFilePath + @"\MasterResources\RoomResources\";
            BRDAL.InsertHistoryLog("eTurnsBaseResource", false);

            for (int m = 0; m < lstModMast.ToList().Count; m++)
            {
                IEnumerable<ResourceModuleDetailsDTO> lstResPage = BRDAL.GetResourceModuleDetailData(lstModMast.ToList()[m].ID);
                for (int F = 0; F < lstResPage.ToList().Count; F++)
                {
                    resLangDTO = BRDAL.GetResourceLanguage();
                    for (int L = 0; L < resLangDTO.ToList().Count; L++)
                    {
                        string ResourceKey = "";
                        string ResourceValue = "";
                        if (lstModMast.ToList()[m].ModuleName == "Enterprise")
                        {
                            // Base Reource for Enterprise Module read from BaseEnterpriseResourceData
                            CurrentBaseResourceList = BaseResourceHelper.GetBaseEnterpriseResourceData(lstResPage.ToList()[F].FileName, resLangDTO.ToList()[L].Culture);
                        }
                        else
                        {
                            CurrentBaseResourceList = BaseResourceHelper.GetBaseResourceData(lstResPage.ToList()[F].FileName, resLangDTO.ToList()[L].Culture);
                        }
                        //////CurrentBaseResourceList = BaseResourceHelper.GetBaseResourceData(lstResPage.ToList()[F].FileName, resLangDTO.ToList()[L].Culture);                
                        try
                        {

                            for (int i = 0; i < CurrentBaseResourceList.Count(); i++)
                            {
                                ResourceKey = CurrentBaseResourceList[i].key;
                                ResourceValue = CurrentBaseResourceList[i].value;
                                BRDAL.SaveBaseResourceDetail(ResourceKey, ResourceValue, lstResPage.ToList()[F].FileName, resLangDTO.ToList()[L].Culture, SessionHelper.UserID, "Web");
                            }

                            // Master Room UDF Resources
                            CurrentBaseResourceList = new List<KeyValDTO>();
                            CurrentBaseResourceList = BaseResourceHelper.GetBaseResourceRoomDataUtility(lstResPage.ToList()[F].FileName, resLangDTO.ToList()[L].Culture, BaseRoomResPath);
                            for (int i = 0; i < CurrentBaseResourceList.Count(); i++)
                            {
                                ResourceKey = CurrentBaseResourceList[i].key;
                                ResourceValue = CurrentBaseResourceList[i].value;
                                BRDAL.SaveBaseResourceDetail(ResourceKey, ResourceValue, lstResPage.ToList()[F].FileName, resLangDTO.ToList()[L].Culture, SessionHelper.UserID, "Web");
                            }
                        }
                        catch 
                        {
                            BRDAL.InsertHistoryLog("eTurnsBaseResource", true);
                        }
                    }
                }
            }
            BRDAL.InsertHistoryLog("eTurnsBaseResource", true);
            return ResCommon.MsgDataSaved;
        }
        public JsonResult GetBaseResourceModule(string ModuleID)
        {
            List<SelectListItem> lstItem = null;

            try
            {
                lstItem = GetResourceModuleDetail(int.Parse(ModuleID)); // GeteTurnsBaseResourceModuleDetail(int.Parse(ModuleID));

                return Json(lstItem, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(lstItem, JsonRequestBehavior.AllowGet);
                throw ex;
            }
            finally
            {
                lstItem = null;
            }
        }
    }
    public class SaveResourceLanguageKey
    {
        public string Key { get; set; }
        public string Language { get; set; }
        public string ConvertValue { get; set; }
    }

}
