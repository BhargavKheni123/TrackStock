using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsMaster.DAL;
using eTurnsWeb.Helper;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class ResourcesController : eTurnsControllerBase
    {

        string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
        string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);
        List<ResourceLanguageKey> lstResourceLanguageKey = new List<ResourceLanguageKey>();
        #region Upload & download

        public ActionResult Upload()
        {
            ViewBag.DDLanguage = GetLanguage();
            ViewBag.DDLResourceFile = GetResourceFiles(0);
            return View();
        }

        [HttpPost]
        public ActionResult Upload(FormCollection collection, HttpPostedFileBase fuResource)
        {
            string language = collection["ddlLanguage"];
            string fileName = collection["ddlResourceFile"];

            string filePath = "";

            if (language == "en-US")
                fileName = fileName + ".resx";
            else
                fileName = fileName + "." + language + ".resx";

            filePath = ResourceHelper.CompanyResourceDirectoryPath + fileName;

            if (fuResource != null)
            {
                fuResource.SaveAs(filePath);
            }

            ViewBag.DDLLanguage = GetLanguage();
            ViewBag.DLLResourceFile = GetResourceFiles(0);
            ViewBag.ShowMessage = "1";
            ViewBag.MessageToDisplay = ResCommon.FileUploadedSuccessfully;
            //return Json(new { Message = "Success", Status = "Ok" }, JsonRequestBehavior.AllowGet);
            return View();

        }

        public ActionResult DownLoad()
        {
            CurrentResourceList = null;
            ViewBag.DDLanguage = GetLanguage();
            ViewBag.DDLResourceFile = GetResourceFiles(0);
            return View();
        }


        [HttpPost]
        public FileResult Download(FormCollection frm)
        {
            string language = frm["ddlLanguage"];
            string fileName = frm["ddlResourceFile"];

            //string appPath1 = eTurns.DTO.Resources.ResourceHelper.GetResorceDirectoryPath;
            //string resourceDirectory = appPath1 + SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID;

            string filePath = "";

            if (language == "en-US")
                fileName = fileName + ".resx";
            else
                fileName = fileName + "." + language + ".resx";

            filePath = ResourceHelper.CompanyResourceDirectoryPath + fileName;
            string contentType = "text/microsoft-resx";
            return File(filePath, contentType, fileName);

        }

        [HttpPost]
        public ActionResult SetLanguage(string url, string cult)
        {
            Session["CompnayCulter"] = cult;
            string[] splitURL = url.Split(new char[] { '/' });
            string action = splitURL[splitURL.Length - 1];
            string controller = splitURL[splitURL.Length - 2];
            string[] splitCult = cult.Split(new char[] { '.' });

            ResourceHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\" + splitCult[0];
            ResourceHelper.RoomResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\" + splitCult[0] + @"\" + SessionHelper.RoomID.ToString();
            System.Globalization.CultureInfo c = new System.Globalization.CultureInfo(splitCult[1]);
            System.Threading.Thread.CurrentThread.CurrentCulture = c;
            ResourceHelper.CurrentCult = c;
            ResourceModuleHelper.CurrentCult = c;


            return RedirectToAction(action, controller);

        }


        #endregion


        #region Other Important Methods

        /// <summary>
        /// change Current language.
        /// </summary>
        /// <param name="SelectedCulture"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CompanyLanguageChanged(string SelectedCulture)
        {
            System.Globalization.CultureInfo c = new System.Globalization.CultureInfo(SelectedCulture);
            System.Threading.Thread.CurrentThread.CurrentCulture = c;
            ResourceHelper.CurrentCult = c;
            ResourceModuleHelper.CurrentCult = c;
            return Json(true);
        }

        /// <summary>
        /// GetLanguage
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetLanguage()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resLangDTO = objDAL.GetCachedResourceLanguageData(SessionHelper.EnterPriceID);
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.Culture;
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
                resLangDTO = null;
                lstItem = null;

            }

        }
        private List<SelectListItem> GetLanguageWithDataBase(string DBName)
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(DBName);
                resLangDTO = objDAL.GeteTurnsLanguages();
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.Culture;
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
                resLangDTO = null;
                lstItem = null;

            }

        }
        private List<SelectListItem> GeteTurnsLanguage()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resLangDTO = objDAL.OnlyeTurnsLanguages();
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.Culture;
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
                resLangDTO = null;
                lstItem = null;

            }

        }

        /// <summary>
        /// GetLanguage
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetLanguageWithID()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resLangDTO = objDAL.GetCachedResourceLanguageData(SessionHelper.EnterPriceID);
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.ID.ToString();
                    if (ResourceHelper.CurrentCult.ToString().ToLower() == item.Culture.ToLower())
                        obj.Selected = true;

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
                resLangDTO = null;
                lstItem = null;

            }

        }

        /// <summary>
        /// GetLanguage
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCompanyLanguage()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resLangDTO = objDAL.GetCachedResourceLanguageData(SessionHelper.EnterPriceID);
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.Culture;
                    lstItem.Add(obj);
                }
                return Json(lstItem, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(lstItem);
                throw ex;
            }
            finally
            {
                objDAL = null;
                resLangDTO = null;
                lstItem = null;

            }

        }

        /// <summary>
        /// GetLanguage
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetResourceModule()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleMasterDTO> resModule = null;
            List<SelectListItem> lstItem = null;
            try
            {
                string resFileName = "ResModuleResource";
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resModule = objDAL.GetCachedResourceModuleMasterData(SessionHelper.CompanyID);
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
                    //obj.Text = item.ModuleName;
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

        /// <summary>
        /// GetResourceFiles
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        private List<SelectListItem> GetResourceFiles(int ModuleID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleDetailsDTO> ResModDet = null;
            try
            {
                string resFileName = "ResModuleResource";
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                ResModDet = objDAL.GetCachedResourceModuleDetailData(SessionHelper.CompanyID, ModuleID);
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

        /// <summary>
        /// GetResourceFiles
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        private List<SelectListItem> GetMobileResourcePages(Int64 ModuleID)
        {
            string resFileName = "ResModuleResource";
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleDetailsDTO> ResModDet = null;
            try
            {
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                ResModDet = objDAL.GetCachedResourceModuleDetailData_Mobile(SessionHelper.CompanyID, ModuleID);
                lstItem = new List<SelectListItem>();
                if (ResModDet != null && ResModDet.Count() > 0)
                {
                    foreach (var item in ResModDet)
                    {
                        SelectListItem obj = new SelectListItem();
                        string resValue = string.Empty;
                        if (!string.IsNullOrEmpty(item.ResMobModuleDetailsKey))
                        {
                            resValue = eTurns.DTO.Resources.ResourceHelper.GetModuleResource(item.ResMobModuleDetailsKey, resFileName);
                            if (!string.IsNullOrEmpty(resValue) && resValue != item.ResMobModuleDetailsKey)
                            {
                                obj.Text = resValue;
                            }
                            else
                            {
                                obj.Text = item.DisplayPageName;
                            }
                        }
                        else
                        {
                            obj.Text = item.DisplayPageName;
                        }
                        //obj.Text = item.DisplayPageName;
                        obj.Value = item.ID.ToString();
                        lstItem.Add(obj);
                    }
                }

                if (lstItem == null || lstItem.Count < 0)
                    lstItem = new List<SelectListItem>();

                SelectListItem Item = new SelectListItem() { Text = ResCommon.SelectPage, Value = "0", Selected = true };
                lstItem.Insert(0, Item);

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

        private List<SelectListItem> GetENTMobileResourcePages(Int64 ModuleID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleDetailsDTO> ResModDet = null;
            try
            {
                string resFileName = "ResModuleResource";
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                ResModDet = objDAL.GetCachedResourceModuleDetailData_Mobile(0, ModuleID);
                lstItem = new List<SelectListItem>();
                if (ResModDet != null && ResModDet.Count() > 0)
                {
                    foreach (var item in ResModDet)
                    {
                        SelectListItem obj = new SelectListItem();
                        string resValue = string.Empty;

                        if (!string.IsNullOrEmpty(item.ResMobModuleDetailsKey))
                        {
                            resValue = eTurns.DTO.Resources.ResourceHelper.GetModuleResource(item.ResMobModuleDetailsKey, resFileName);
                            if (!string.IsNullOrEmpty(resValue) && resValue != item.ResMobModuleDetailsKey)
                            {
                                obj.Text = resValue;
                            }
                            else
                            {
                                obj.Text = item.DisplayPageName;
                            }
                        }
                        else
                        {
                            obj.Text = item.DisplayPageName;
                        }

                        //obj.Text = item.DisplayPageName;
                        obj.Value = item.ID.ToString();
                        lstItem.Add(obj);
                    }
                }

                if (lstItem == null || lstItem.Count < 0)
                    lstItem = new List<SelectListItem>();

                SelectListItem Item = new SelectListItem() { Text = ResCommon.SelectPage, Value = "0", Selected = true };
                lstItem.Insert(0, Item);

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
        /// <summary>
        /// GetResourceFiles
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public JsonResult GetResourceFilesData(string ModuleID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();

            try
            {
                lstItem = GetResourceFiles(int.Parse(ModuleID));

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


        /// <summary>
        /// GetResourceFiles
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public JsonResult GetResourcePageData(Int64 ModuleID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();

            try
            {
                lstItem = GetMobileResourcePages(ModuleID);

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
        public JsonResult GetENTResourcePageData(Int64 ModuleID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();

            try
            {
                lstItem = GetENTMobileResourcePages(ModuleID);

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
        [HttpPost]
        public JsonResult RewriteAllResourceFilesToCompanyDirectory(string EnterpriceID, string CompanyID)
        {
            string[] MasterFiles = null;
            List<string> AllDirs = null;
            try
            {
                string masterResourceDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\MasterResources";
                AllDirs = Directory.GetDirectories(eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath).ToList();
                MasterFiles = Directory.GetFiles(masterResourceDirPath);
                AllDirs.RemoveAt(AllDirs.IndexOf(masterResourceDirPath));
                foreach (string dirPath in AllDirs)
                {
                    foreach (var file in MasterFiles)
                    {
                        System.IO.File.Copy(file, System.IO.Path.Combine(dirPath, System.IO.Path.GetFileName(file)), true);
                    }
                }
            }
            catch (Exception)
            {
                //  throw ex;
                return Json("false");
            }
            finally
            {
                MasterFiles = null;
                AllDirs = null;
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult ResetResourceFile(string SelectedResourceFile, string SelectedCulter, string SelectedModule)
        {
            string culter = string.Empty;
            string fileName = string.Empty;
            string message = "";
            string status = "";
            try
            {

                if (!SelectedCulter.ToLower().Contains("en-us"))
                    culter = "." + SelectedCulter;

                fileName = SelectedResourceFile + culter + ".resx";
                string masterResourceDirPath = string.Empty;
                string CompanyFullDirPath = string.Empty;
                if (SelectedModule == "Enterprise")
                {
                    masterResourceDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\MasterResources\EnterpriseResources";
                    CompanyFullDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\" + SessionHelper.EnterPriceID.ToString() + @"\EnterpriseResources";
                }
                else
                {
                    masterResourceDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\" + SessionHelper.EnterPriceID.ToString() + @"\CompanyResources";
                    CompanyFullDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\" + SessionHelper.EnterPriceID.ToString() + @"\" + SessionHelper.CompanyID.ToString();
                }

                string masterResourceFilePath = masterResourceDirPath + @"\" + fileName;

                if (!System.IO.Directory.Exists(CompanyFullDirPath))
                {
                    System.IO.Directory.CreateDirectory(CompanyFullDirPath);
                }

                System.IO.File.Copy(masterResourceFilePath, System.IO.Path.Combine(CompanyFullDirPath, fileName), true);
                message = string.Format(ResResourceEditor.UploadSuccess, HttpStatusCode.OK);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = ResMessage.SaveMessage;

            }
            catch (Exception)
            {
                //  throw ex;
                return Json(new { Message = ResResourceEditor.UploadNotSuccess, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {

            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CopyNewResourceToFile(string SelectedResourceFile, string SelectedCulter, string SelectedModule)
        {
            //XmlDocument loResource = null;
            string culter = string.Empty;
            string fileName = string.Empty;
            string message = "";
            string status = "";
            try
            {

                if (!SelectedCulter.ToLower().Contains("en-us"))
                    culter = "." + SelectedCulter;

                fileName = SelectedResourceFile + culter + ".resx";
                string masterResourceDirPath = string.Empty;
                string CompanyFullDirPath = string.Empty;
                if (SelectedModule == "Enterprise")
                {
                    masterResourceDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\MasterResources\EnterpriseResources";
                    CompanyFullDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\" + SessionHelper.EnterPriceID.ToString() + @"\EnterpriseResources";
                }
                else
                {
                    masterResourceDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\MasterResources\CompanyResources";
                    CompanyFullDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\" + SessionHelper.EnterPriceID.ToString() + @"\" + SessionHelper.CompanyID.ToString();
                }

                string masterResourceFilePath = masterResourceDirPath + @"\" + fileName;
                string ChildResourceFilePath = CompanyFullDirPath + @"\" + fileName;

                if (!System.IO.Directory.Exists(CompanyFullDirPath))
                {
                    System.IO.Directory.CreateDirectory(CompanyFullDirPath);
                }
                if (!System.IO.File.Exists(ChildResourceFilePath))
                {
                    System.IO.File.Copy(masterResourceFilePath, ChildResourceFilePath);
                }
                else
                {
                    List<KeyValDTO> lstMasterResource = new List<KeyValDTO>();
                    List<KeyValDTO> lstChildResource = new List<KeyValDTO>();
                    lstMasterResource = GetKeyValResourceList(masterResourceFilePath);
                    lstChildResource = GetKeyValResourceList(ChildResourceFilePath);
                    foreach (KeyValDTO item in lstMasterResource)
                    {
                        if (!lstChildResource.Exists(x => x.key.Contains(item.key)))
                        {
                            lstChildResource.Add(item);
                        }
                    }


                    ResXResourceWriter resourceWriter = new ResXResourceWriter(ChildResourceFilePath);
                    foreach (KeyValDTO item in lstChildResource)
                    {
                        resourceWriter.AddResource(item.key, item.value);
                    }
                    resourceWriter.Generate();
                    resourceWriter.Close();
                }



                message = string.Format(ResResourceEditor.UploadSuccess, HttpStatusCode.OK);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = ResMessage.SaveMessage;

            }
            catch (Exception)
            {
                //  throw ex;
                return Json(new { Message = ResResourceEditor.UploadNotSuccess, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {

            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }
        public List<KeyValDTO> GetKeyValResourceList(string strFilePath)
        {
            XmlDocument loResource = null;
            List<KeyValDTO> lstResourceList = new List<KeyValDTO>();


            loResource = new XmlDocument();
            if (System.IO.File.Exists(strFilePath))
            {
                loResource.Load(strFilePath);
                XmlNodeList lstNodes = loResource.SelectNodes("root/data");
                foreach (XmlNode item in lstNodes)
                {
                    KeyValDTO kv = new KeyValDTO();
                    kv.key = item.Attributes["name"].Value;
                    kv.value = item.SelectSingleNode("value").InnerText;
                    lstResourceList.Add(kv);
                }
            }
            return lstResourceList;
        }


        [HttpPost]
        public JsonResult ResetMobileResource(Int64 ResourcePageID, Int64 LanguageID)
        {
            string culter = string.Empty;
            string fileName = string.Empty;
            string message = "";
            string status = "";
            try
            {
                MobileResourcesDAL mobResDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);

                mobResDAL.ResetMobileResource(ResourcePageID, LanguageID, SessionHelper.UserID, SessionHelper.CompanyID);
                message = string.Format(ResResourceEditor.UploadSuccess, HttpStatusCode.OK);
                status = ResMessage.SaveMessage;
            }
            catch (Exception)
            {
                //  throw ex;
                return Json(new { Message = ResResourceEditor.UploadNotSuccess, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {

            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region EditResources

        private const string _CURRENRESOURCESESSIONKEY = "CURRENTRESOURCELIST";

        /// <summary>
        /// Set Current Resource in Session
        /// </summary>
        private List<KeyValDTO> CurrentResourceList
        {
            get
            {
                if (HttpContext.Session[_CURRENRESOURCESESSIONKEY] != null)
                    return (List<KeyValDTO>)HttpContext.Session[_CURRENRESOURCESESSIONKEY];
                return new List<KeyValDTO>();
            }
            set
            {
                HttpContext.Session[_CURRENRESOURCESESSIONKEY] = value;
            }
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetResourceList(JQueryDataTableParamModelForResource param)
        {
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            string sortDirection = "none";

            if (!string.IsNullOrEmpty(Request["sSortDir_0"]))
                sortDirection = Request["sSortDir_0"];

            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            //sortColumnName = Request["SortingField"].ToString();

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";



            string searchQuery = string.Empty;

            int TotalRecordCount = 0;


            List<KeyValDTO> lstKeyVal = null;
            List<KeyValDTO> CurrentResourceUDFList = null;

            if (CurrentResourceList == null || CurrentResourceList.Count <= 0)
            {
                if (param.resourcemodule == "Enterprise")
                {
                    CurrentResourceList = ResourceHelper.GetEnterpriseResourceData(param.resourcefile, param.resourcelang);
                }
                else
                {
                    CurrentResourceList = ResourceHelper.GetResourceData(param.resourcefile, param.resourcelang);
                    CurrentResourceUDFList = ResourceHelper.GetUDFResourceData(param.resourcefile, param.resourcelang);
                    if (CurrentResourceList != null && CurrentResourceList.Count > 0)
                    {
                        foreach (KeyValDTO k in CurrentResourceUDFList)
                        {
                            KeyValDTO k1 = CurrentResourceList.Where(w => w.key == k.key).FirstOrDefault();
                            if (k1 != null)
                            {
                                k1.value = k.value;
                            }
                            else
                            {
                                KeyValDTO NewUDFKey = new KeyValDTO();
                                NewUDFKey.key = k.key;
                                NewUDFKey.value = k.value;
                                CurrentResourceList.Add(NewUDFKey);
                            }
                        }

                    }
                }
            }

            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch.Trim().Length > 0)
            {
                lstKeyVal = (from y in CurrentResourceList
                             where y.key.ToLower().Contains(param.sSearch.ToLower()) || y.value.ToLower().Contains(param.sSearch.ToLower())
                             select y).ToList<KeyValDTO>();
            }
            else
                lstKeyVal = CurrentResourceList;



            if (sortDirection == "asc")
            {
                lstKeyVal = lstKeyVal.OrderBy(x => x.key).ToList<KeyValDTO>();
            }
            else if (sortDirection == "desc")
            {
                //sortColumnName = sortColumnName + " desc"; 
                lstKeyVal = lstKeyVal.OrderByDescending(x => x.key).ToList<KeyValDTO>();
            }

            TotalRecordCount = lstKeyVal.Count();
            var result = from c in lstKeyVal
                         select new KeyValDTO
                         {
                             key = c.key,
                             value = c.value
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
        /// Edit Resource View
        /// </summary>
        /// <returns></returns>
        public ActionResult EditResource(string lang = "", string module = "", string page = "")
        {

            CurrentResourceList = null;
            List<SelectListItem> lstLang = GetLanguage();
            List<SelectListItem> lstModMast = GetResourceModule();
            if (SessionHelper.UserType != 2 && lstModMast != null && lstModMast.Count > 0)
                lstModMast = lstModMast.Where(t => t.Text != "Enterprise").ToList();

            List<SelectListItem> lstResPage = GetResourceFiles(int.Parse(lstModMast[0].Value));

            if (!string.IsNullOrEmpty(lang))
            {
                foreach (var item in lstLang)
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
                        lstResPage = GetResourceFiles(int.Parse(item.Value));
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

        ///// <summary>
        ///// Edit Resource View
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult EditResourceWithModule(int ModuleID)
        //{
        //    CurrentResourceList = null;
        //    ViewBag.DDLanguage = GetLanguage();
        //    List<SelectListItem> lstModMast = GetResourceModule();
        //    ViewBag.DDLModuleMaster = lstModMast;
        //    ViewBag.DDLResourceFile = GetResourceFiles(ModuleID);
        //    return View();
        //}

        /// <summary>
        /// Clear Resource from Session
        /// </summary>
        public JsonResult ClearCurrentResourceList()
        {
            if (CurrentResourceList != null)
                CurrentResourceList.Clear();

            if (CurrentENTResourceList != null)
                CurrentENTResourceList.Clear();

            if (CurrentBaseResourceList != null)
                CurrentBaseResourceList.Clear();

            if (CurrentMobileResourceList != null)
                CurrentMobileResourceList.Clear();

            if (CurrentMobileENTResourceList != null)
                CurrentMobileENTResourceList.Clear();

            if (CurrentMobileBaseResourceList != null)
                CurrentMobileBaseResourceList.Clear();

            CurrentResourceList = null;
            CurrentENTResourceList = null;
            CurrentBaseResourceList = null;
            CurrentMobileResourceList = null;
            CurrentMobileENTResourceList = null;
            CurrentMobileBaseResourceList = null;

            return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Clear Resource from Session
        /// </summary>
        //public ActionResult ClearCurrentResourceList(string selectedResourceFile, string selectedLanguage)
        //{
        //    CurrentResourceList = null;

        //    JQueryDataTableParamModelForResource dd = new JQueryDataTableParamModelForResource();
        //    dd.iDisplayLength = 10;
        //    dd.iDisplayStart = 0;
        //    dd.sEcho = "1";

        //    dd.resourcefile = selectedResourceFile;
        //    dd.resourcelang = selectedLanguage;
        //    return GetResourceList(dd);


        //}

        /// <summary>
        /// Save Resource into Resource file from Current Session
        /// </summary>
        /// <param name="SelectedResourceFile"></param>
        /// <param name="SelectedCulter"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveResources(string SelectedResourceFile, string SelectedCulter, string SelectedModule, string arrItems = "")
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            KeyValCheckDTO[] LstKeyValCheckDTO = s.Deserialize<KeyValCheckDTO[]>(arrItems);
            ResourceHelper resHelper = new ResourceHelper();
            try
            {
                if (SelectedModule == "Enterprise")
                {
                    resHelper.SaveEnterpriseResources(SelectedResourceFile, SelectedCulter, CurrentResourceList);
                    eTurns.DAL.UDFDAL objUDF = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    List<string> PageResourceFileNameList = objUDF.GetPageResourceFilesByReportResourceFile(SelectedResourceFile);
                    if (PageResourceFileNameList != null && PageResourceFileNameList.Count() > 0)
                    {
                        foreach (string PageResourceFileName in PageResourceFileNameList)
                        {
                            resHelper.SaveResources(PageResourceFileName, SelectedCulter, CurrentResourceList);
                        }
                    }
                }
                else
                {
                    resHelper.SaveResources(SelectedResourceFile, SelectedCulter, CurrentResourceList);
                    if (LstKeyValCheckDTO != null && LstKeyValCheckDTO.Count() > 0)
                    {
                        UpdateAllCompanyResource(LstKeyValCheckDTO.Where(t => t.chkvalue == true).ToList(), SelectedResourceFile, SelectedCulter);
                    }
                    if (SelectedModule.ToLower() == "reports")
                    {
                        //eTurns.DAL.UDFDAL objUDF = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                        //List<string> PageResourceFileNameList = objUDF.getPageResourceFile(SelectedResourceFile);
                        //if (PageResourceFileNameList != null && PageResourceFileNameList.Count() > 0)
                        //{
                        //    foreach (string PageResourceFileName in PageResourceFileNameList)
                        //    {
                        //        resHelper.SaveResources(PageResourceFileName, SelectedCulter, CurrentResourceList);
                        //    }
                        //}
                        //if (LstKeyValCheckDTO != null && LstKeyValCheckDTO.Count() > 0)
                        //{
                        //    if (PageResourceFileNameList != null && PageResourceFileNameList.Count() > 0)
                        //    {
                        //        foreach (string PageResourceFileName in PageResourceFileNameList)
                        //        {
                        //            UpdateAllCompanyResource(LstKeyValCheckDTO.Where(t => t.chkvalue == true).ToList(), PageResourceFileName, SelectedCulter);
                        //        }
                        //    }
                        //}
                    }
                    else if (SelectedModule.ToLower() == "consume")
                    {
                        eTurns.DAL.UDFDAL objUDF = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                        List<string> ReportResourceFileNameList = objUDF.GetReportResourceFilesByPageResourceFile(SelectedResourceFile);
                        if (ReportResourceFileNameList != null && ReportResourceFileNameList.Count() > 0)
                        {
                            foreach (string PageResourceFileName in ReportResourceFileNameList)
                            {
                                resHelper.SaveResources(PageResourceFileName, SelectedCulter, CurrentResourceList);
                                string strFilePathWithFileName = resHelper.getFilePath(PageResourceFileName, true);
                                string actulFilename = PageResourceFileName + "." + SelectedCulter + ".resx";
                                if (System.IO.File.Exists(strFilePathWithFileName))
                                {
                                    System.IO.File.Delete(strFilePathWithFileName);
                                }
                                if (SelectedCulter == "en-US")
                                    SelectedCulter = "";
                                else
                                    SelectedCulter = "." + SelectedCulter;
                                string strFilePath = strFilePathWithFileName.Replace(actulFilename, SelectedResourceFile + SelectedCulter + ".resx");
                                string FileTobeCopied = strFilePathWithFileName.Replace(PageResourceFileName, SelectedResourceFile);
                                System.IO.File.Copy(FileTobeCopied, strFilePathWithFileName);
                            }
                        }
                        if (LstKeyValCheckDTO != null && LstKeyValCheckDTO.Count() > 0)
                        {
                            if (ReportResourceFileNameList != null && ReportResourceFileNameList.Count() > 0)
                            {
                                foreach (string PageResourceFileName in ReportResourceFileNameList)
                                {
                                    UpdateAllCompanyResource(LstKeyValCheckDTO.Where(t => t.chkvalue == true).ToList(), PageResourceFileName, SelectedCulter);
                                }
                            }
                        }
                    }
                }


                message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = ResMessage.SaveMessage;//"fail";
                                                //ClearCurrentResourceList();
            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = "fail";
            }
            finally
            {
                resHelper = null;
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }
        public void UpdateAllCompanyResource(List<KeyValCheckDTO> lstKeyValCheckDTO, string filename, string culter)
        {
            try
            {

                if (lstKeyValCheckDTO != null && lstKeyValCheckDTO.Count > 0)
                {
                    CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                    List<CompanyMasterDTO> lstCompanyMasterDTO = new List<CompanyMasterDTO>();
                    lstCompanyMasterDTO = objCompanyMasterDAL.GetAllCompaniesByEnterpriseID(SessionHelper.EnterPriceID);
                    ResourceHelper resHelper = new ResourceHelper();
                    foreach (CompanyMasterDTO item in lstCompanyMasterDTO)
                    {
                        string TargetFilePath = GetCRFilepath(culter, filename, Convert.ToString(SessionHelper.EnterPriceID), Convert.ToString(item.ID));
                        resHelper.SaveResourcesAllCompany(TargetFilePath, lstKeyValCheckDTO);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public void UpdateAllCompanyMobileResource(MobileResourcesDTO objItem)
        {
            try
            {
                CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                List<CompanyMasterDTO> lstCompanyMasterDTO = new List<CompanyMasterDTO>();
                lstCompanyMasterDTO = objCompanyMasterDAL.GetAllCompaniesByEnterpriseID(SessionHelper.EnterPriceID);
                MobileResourcesDAL objMobileResDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);

                foreach (CompanyMasterDTO item in lstCompanyMasterDTO)
                {
                    //MobileResourcesDTO obj = objMobileResDAL.GetAllRecords(item.ID).Where(x => x.ResourcePageID == objItem.ResourcePageID && x.LanguageID == objItem.LanguageID && x.ResourceKey == objItem.ResourceKey).FirstOrDefault();
                    MobileResourcesDTO obj = objMobileResDAL.GetAllMobileResources(item.ID, objItem.ResourcePageID, objItem.LanguageID, objItem.ResourceKey).FirstOrDefault();
                    if (obj != null)
                    {
                        obj.ResourceValue = objItem.ResourceValue;
                        objMobileResDAL.Edit(obj);
                    }
                }

            }
            catch (Exception)
            {

            }
        }
        public string GetCRFilepath(string culter, string fileName, string ENTID, string CompanyID)
        {
            string ResourceFileExt = ".resx";
            string ResourceBaseFilePath  = CommonUtility.ResourceBaseFilePath;
            string CompanyResourceDirectoryPath = ResourceBaseFilePath+ @"\" + ENTID + @"\" + CompanyID + @"\";
            if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
            {
                if (culter == "en-US")
                    culter = "";
                else
                    culter = "." + culter;


                return (CompanyResourceDirectoryPath + fileName + culter + ResourceFileExt).Replace("..", ".");
            }

            return (CompanyResourceDirectoryPath + fileName + ResourceFileExt).Replace("..", ".");
        }
        public string GetERFilepath(string culter, string fileName, string ENTID)
        {
            string ResourceFileExt = ".resx";
            string ResourceBaseFilePath = CommonUtility.ResourceBaseFilePath;
            string EnterpriseCompanyResourceDirectoryPath = ResourceBaseFilePath + @"\" + ENTID + @"\CompanyResources\";
            if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
            {
                if (culter == "en-US")
                    culter = "";
                else
                    culter = "." + culter;


                return (EnterpriseCompanyResourceDirectoryPath + fileName + culter + ResourceFileExt).Replace("..", ".");
            }

            return (EnterpriseCompanyResourceDirectoryPath + fileName + ResourceFileExt).Replace("..", ".");
        }
        /// <summary>
        /// Update Data In Current Resource Session
        /// </summary> 
        /// <param name="changedKey"></param>
        /// <param name="changedValue"></param>
        /// <param name="Lang"></param>
        /// <param name="ResFile"></param>
        /// <returns></returns>
        public string UpdateDataResourceSession(string changedKey, string changedValue)
        {
            if (!string.IsNullOrEmpty(changedValue) && !string.IsNullOrWhiteSpace(changedValue) && CurrentResourceList != null && CurrentResourceList.Count > 0)
            {
                int indx = CurrentResourceList.FindIndex(x => x.key == changedKey);
                CurrentResourceList[indx] = new KeyValDTO() { key = changedKey, value = changedValue };
            }
            return changedValue;
        }

        /// <summary>
        /// // below function used to load the User wise saved filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <returns></returns>
        public ActionResult LoadGridState(string ListName)
        {
            string jsonData = @"{""iCreate"":1350639486123,""iStart"":0,""iEnd"":0,"",""aaSorting"":[[0,""none"",0,""Name""]]
                                   ,""oSearch"":{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,""aoSearchCols"":[{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}]
                                    ,""abVisCols"":[true,true],""ColReorder"":[0,1]}";
            return Json(new { jsonData }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// // below function used to Save the User wise filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <param name="Data"></param> will have json string of filter criteria 
        /// <returns></returns>
        public ActionResult SaveGridState(string Data, string ListName)
        {
            string jsonData = @"{""iCreate"":1350639486123,""iStart"":0,""iEnd"":0,""iLength"":10,""aaSorting"":[[0,""asc"",0,""Name""]]
                                   ,""oSearch"":{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,""aoSearchCols"":[{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}]
                                    ,""abVisCols"":[false,true,true,true,true],""ColReorder"":[0,1]}";
            return Json(new { jsonData }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region EditMobileResources

        private const string _CURRENTMOBILERESOURCESESSIONKEY = "CURRENTMOBILERESOURCELIST";

        /// <summary>
        /// Set Current Resource in Session
        /// </summary>
        private List<MobileResourcesDTO> CurrentMobileResourceList
        {
            get
            {
                if (HttpContext.Session[_CURRENTMOBILERESOURCESESSIONKEY] != null)
                    return (List<MobileResourcesDTO>)HttpContext.Session[_CURRENTMOBILERESOURCESESSIONKEY];
                return new List<MobileResourcesDTO>();
            }
            set
            {
                HttpContext.Session[_CURRENTMOBILERESOURCESESSIONKEY] = value;
            }
        }


        /// <summary>
        /// Edit Resource View
        /// </summary>
        /// <returns></returns>
        public ActionResult EditMobileResource(string lang = "", string module = "", string PageID = "")
        {
            CurrentResourceList = null;
            List<SelectListItem> lstLang = GetLanguageWithID();
            List<SelectListItem> lstModMast = GetResourceModule();
            List<SelectListItem> lstResourceFiles = GetMobileResourcePages(int.Parse(lstModMast[0].Value));

            if (!string.IsNullOrEmpty(lang))
            {
                foreach (var item in lstLang)
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
                        lstResourceFiles = GeteTurnsBaseMobileResourcePages(int.Parse(item.Value)); //GetBaseResourceFiles(int.Parse(item.Value));
                        break;
                    }
                }

                foreach (var item in lstResourceFiles)
                {
                    if (item.Value.ToLower() == PageID.ToLower())
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }

            ViewBag.DDLanguage = lstLang;
            ViewBag.DDLModuleMaster = lstModMast;
            ViewBag.DDLResourceFile = lstResourceFiles;

            return View();
        }


        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetMobileResourceList(JQueryDataTableParamModelForResource param)
        {
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            string sortDirection = "none";

            if (!string.IsNullOrEmpty(Request["sSortDir_0"]))
                sortDirection = Request["sSortDir_0"];

            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;


            string searchQuery = string.Empty;

            int TotalRecordCount = 0;


            List<MobileResourcesDTO> lstKeyVal = null;
            MobileResourcesDAL mobileResDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);


            //if (CurrentMobileResourceList == null || CurrentMobileResourceList.Count <= 0)
            //    CurrentMobileResourceList = ResourceHelper.GetResourceData(param.resourcefile, param.resourcelang);

            Int64 resourceID = 0;
            Int64 landID = 0;

            Int64.TryParse(param.resourcefile, out resourceID);
            Int64.TryParse(param.resourcelang, out landID);

            if (CurrentMobileResourceList == null || CurrentMobileResourceList.Count <= 0)
            {
                //IEnumerable<MobileResourcesDTO> lstMobileResData = mobileResDAL.GetAllRecords(SessionHelper.CompanyID).ToList();
                //CurrentMobileResourceList = lstMobileResData.Where(x => x.ResourcePageID == resourceID && x.LanguageID == landID).ToList();
                CurrentMobileResourceList = mobileResDAL.GetAllMobileResources(SessionHelper.CompanyID, resourceID, landID, string.Empty).ToList();
            }

            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch.Trim().Length > 0)
            {
                lstKeyVal = (from y in CurrentMobileResourceList
                             where y.ResourceKey.ToLower().Contains(param.sSearch.ToLower()) || y.ResourceValue.ToLower().Contains(param.sSearch.ToLower())
                             select y).ToList<MobileResourcesDTO>();
            }
            else
                lstKeyVal = CurrentMobileResourceList;



            if (sortDirection == "asc")
            {
                lstKeyVal = lstKeyVal.OrderBy(x => x.ResourceKey).ToList<MobileResourcesDTO>();
            }
            else if (sortDirection == "desc")
            {
                //sortColumnName = sortColumnName + " desc"; 
                lstKeyVal = lstKeyVal.OrderByDescending(x => x.ResourceKey).ToList<MobileResourcesDTO>();
            }

            TotalRecordCount = lstKeyVal.Count();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = lstKeyVal
            }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Save Resource into Resource file from Current Session
        /// </summary>
        /// <param name="SelectedResourceFile"></param>
        /// <param name="SelectedCulter"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveMobileResources(string SelectedResourceFile, string SelectedCulter)
        {
            string message = "";
            string status = "";

            ResourceHelper resHelper = new ResourceHelper();
            MobileResourcesDAL objMobileResDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
            try
            {
                //resHelper.SaveResources(SelectedResourceFile, SelectedCulter, CurrentResourceList);
                foreach (var item in CurrentMobileResourceList)
                {
                    item.UpdatedBy = SessionHelper.UserID;
                    objMobileResDAL.Edit(item);

                }

                message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = ResMessage.SaveMessage;//"fail";
                ClearCurrentResourceList();
            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = "fail";
            }
            finally
            {
                resHelper = null;
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update Data In Current Resource Session
        /// </summary> 
        /// <param name="changedKey"></param>
        /// <param name="changedValue"></param>
        /// <param name="Lang"></param>
        /// <param name="ResFile"></param>
        /// <returns></returns>
        public JsonResult UpdateDataInMobileResourceSession(string ID, string changedKey, string changedValue)
        {
            if (!string.IsNullOrEmpty(changedValue) && !string.IsNullOrWhiteSpace(changedValue) && CurrentMobileResourceList != null && CurrentMobileResourceList.Count > 0)
            {
                int indx = CurrentMobileResourceList.FindIndex(x => x.ResourceKey == changedKey);
                CurrentMobileResourceList[indx] = new MobileResourcesDTO() { ID = Int64.Parse(ID), ResourceKey = changedKey, ResourceValue = changedValue };
            }
            return Json(new { Message = "sucess", Status = "ok", Value = changedValue }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region EditENTResources

        private const string _CURRENENTRESOURCESESSIONKEY = "CURRENTENTRESOURCELIST";

        /// <summary>
        /// Set Current Resource in Session
        /// </summary>
        private List<KeyValDTO> CurrentENTResourceList
        {
            get
            {
                if (HttpContext.Session[_CURRENENTRESOURCESESSIONKEY] != null)
                    return (List<KeyValDTO>)HttpContext.Session[_CURRENENTRESOURCESESSIONKEY];
                return new List<KeyValDTO>();
            }
            set
            {
                HttpContext.Session[_CURRENENTRESOURCESESSIONKEY] = value;
            }
        }

        private const string _CURRENENTERPRISERESOURCESESSIONKEY = "CURRENTENTERPRISERESOURCELIST";

        private List<EnterpriseResourcesDTO> CurrentEnterpriseResourceList
        {
            get
            {
                if (HttpContext.Session[_CURRENENTERPRISERESOURCESESSIONKEY] != null)
                    return (List<EnterpriseResourcesDTO>)HttpContext.Session[_CURRENENTERPRISERESOURCESESSIONKEY];
                return new List<EnterpriseResourcesDTO>();
            }
            set
            {
                HttpContext.Session[_CURRENENTERPRISERESOURCESESSIONKEY] = value;
            }
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetENTResourceList(JQueryDataTableParamModelForResource param)
        {
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            string sortDirection = "none";

            if (!string.IsNullOrEmpty(Request["sSortDir_0"]))
                sortDirection = Request["sSortDir_0"];

            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            //sortColumnName = Request["SortingField"].ToString();

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";



            string searchQuery = string.Empty;

            int TotalRecordCount = 0;


            List<KeyValDTO> lstKeyVal = null;


            if (CurrentENTResourceList == null || CurrentENTResourceList.Count <= 0)
            {
                if (param.resourcemodule == "Enterprise")
                {
                    CurrentENTResourceList = EnterPriseResourceHelper.GetEnterpriseResourceData(param.resourcefile, param.resourcelang);
                }
                else
                {
                    CurrentENTResourceList = EnterPriseResourceHelper.GetResourceData(param.resourcefile, param.resourcelang);
                }
            }

            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch.Trim().Length > 0)
            {
                lstKeyVal = (from y in CurrentENTResourceList
                             where y.key.ToLower().Contains(param.sSearch.ToLower()) || y.value.ToLower().Contains(param.sSearch.ToLower())
                             select y).ToList<KeyValDTO>();
            }
            else
                lstKeyVal = CurrentENTResourceList;



            if (sortDirection == "asc")
            {
                lstKeyVal = lstKeyVal.OrderBy(x => x.key).ToList<KeyValDTO>();
            }
            else if (sortDirection == "desc")
            {
                //sortColumnName = sortColumnName + " desc"; 
                lstKeyVal = lstKeyVal.OrderByDescending(x => x.key).ToList<KeyValDTO>();
            }

            TotalRecordCount = lstKeyVal.Count();
            var result = from c in lstKeyVal
                         select new KeyValDTO
                         {
                             key = c.key,
                             value = c.value
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
        /// Edit Resource View
        /// </summary>
        /// <returns></returns>
        public ActionResult EditENTResource(string lang = "", string module = "", string page = "")
        {
            List<string> allowedentIds = (SiteSettingHelper.AllowedEntIdsResources ?? String.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!allowedentIds.Contains(SessionHelper.EnterPriceID.ToString()))
            {
                return RedirectToAction(ActName, CtrlName);
            }
            CurrentENTResourceList = null;
            List<SelectListItem> lstLang = GetENTLanguage();
            List<SelectListItem> lstModMast = GetENTResourceModule();
            if (SessionHelper.UserType > 2 && lstModMast != null && lstModMast.Count > 0)
                lstModMast = lstModMast.Where(t => t.Text != "Enterprise").ToList();

            List<SelectListItem> lstResPage = GetENTResourceFiles(int.Parse(lstModMast[0].Value));

            if (!string.IsNullOrEmpty(lang))
            {
                foreach (var item in lstLang)
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
                        lstResPage = GetENTResourceFiles(int.Parse(item.Value));
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

        private List<SelectListItem> GetENTResourceFiles(int ModuleID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleDetailsDTO> ResModDet = null;
            try
            {
                string resFileName = "ResModuleResource";
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
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
        private List<SelectListItem> GetENTResourceModule()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleMasterDTO> resModule = null;
            List<SelectListItem> lstItem = null;
            try
            {
                string resFileName = "ResModuleResource";
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
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
                    //obj.Text = item.ModuleName;
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
        private List<SelectListItem> GetENTLanguage()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resLangDTO = objDAL.GetCachedResourceLanguageData(0);
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.Culture;
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
                resLangDTO = null;
                lstItem = null;

            }

        }

        /// <summary>
        /// Clear Resource from Session
        /// </summary>
        public JsonResult ClearCurrentENTResourceList()
        {
            CurrentENTResourceList = null;
            return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ClearCurrentEnterpriseResourceList()
        {
            CurrentEnterpriseResourceList = null;
            return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        private const string _CURRENCOMPANYRESOURCESESSIONKEY = "CURRENCOMPANYRESOURCESESSIONKEY";

        private List<CompanyResourcesDTO> CurrentCompanyResourceList
        {
            get
            {
                if (HttpContext.Session[_CURRENCOMPANYRESOURCESESSIONKEY] != null)
                    return (List<CompanyResourcesDTO>)HttpContext.Session[_CURRENCOMPANYRESOURCESESSIONKEY];
                return new List<CompanyResourcesDTO>();
            }
            set
            {
                HttpContext.Session[_CURRENCOMPANYRESOURCESESSIONKEY] = value;
            }
        }

        public JsonResult ClearCurrentCompanyResourceList()
        {
            CurrentCompanyResourceList = null;
            return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Save Resource into Resource file from Current Session
        /// </summary>
        /// <param name="SelectedResourceFile"></param>
        /// <param name="SelectedCulter"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveENTResources(string SelectedResourceFile, string SelectedCulter, string SelectedModule, string arrItems = "")
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            KeyValCheckDTO[] LstKeyValCheckDTO = s.Deserialize<KeyValCheckDTO[]>(arrItems);

            EnterPriseResourceHelper resHelper = new EnterPriseResourceHelper();
            try
            {
                if (SelectedModule == "Enterprise")
                {
                    resHelper.SaveEnterpriseResources(SelectedResourceFile, SelectedCulter, CurrentENTResourceList);
                }
                else
                {
                    resHelper.SaveResources(SelectedResourceFile, SelectedCulter, CurrentENTResourceList);

                    if (LstKeyValCheckDTO != null && LstKeyValCheckDTO.Count() > 0)
                    {
                        UpdateAllCompanyResource(LstKeyValCheckDTO.Where(t => t.chkvalue == true).ToList(), SelectedResourceFile, SelectedCulter);
                    }
                }


                message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = ResMessage.SaveMessage;//"fail";
                ClearCurrentENTResourceList();
            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = "fail";
            }
            finally
            {
                resHelper = null;
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update Data In Current Resource Session
        /// </summary> 
        /// <param name="changedKey"></param>
        /// <param name="changedValue"></param>
        /// <param name="Lang"></param>
        /// <param name="ResFile"></param>
        /// <returns></returns>
        public string UpdateDataENTResourceSession(string changedKey, string changedValue)
        {
            if (!string.IsNullOrEmpty(changedValue) && !string.IsNullOrWhiteSpace(changedValue) && CurrentENTResourceList != null && CurrentENTResourceList.Count > 0)
            {
                int indx = CurrentENTResourceList.FindIndex(x => x.key == changedKey);
                CurrentENTResourceList[indx] = new KeyValDTO() { key = changedKey, value = changedValue };
            }
            return changedValue;
        }

        /// <summary>
        /// // below function used to load the User wise saved filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <returns></returns>
        public ActionResult LoadENTGridState(string ListName)
        {
            string jsonData = @"{""iCreate"":1350639486123,""iStart"":0,""iEnd"":0,"",""aaSorting"":[[0,""none"",0,""Name""]]
                                   ,""oSearch"":{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,""aoSearchCols"":[{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}]
                                    ,""abVisCols"":[true,true],""ColReorder"":[0,1]}";
            return Json(new { jsonData }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// // below function used to Save the User wise filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <param name="Data"></param> will have json string of filter criteria 
        /// <returns></returns>
        public ActionResult SaveENTGridState(string Data, string ListName)
        {
            string jsonData = @"{""iCreate"":1350639486123,""iStart"":0,""iEnd"":0,""iLength"":10,""aaSorting"":[[0,""asc"",0,""Name""]]
                                   ,""oSearch"":{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,""aoSearchCols"":[{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}]
                                    ,""abVisCols"":[false,true,true,true,true],""ColReorder"":[0,1]}";
            return Json(new { jsonData }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetENTResourceFilesData(string ModuleID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();

            try
            {
                lstItem = GetENTResourceFiles(int.Parse(ModuleID));

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
        [HttpPost]
        public JsonResult ResetENTResourceFile(string SelectedResourceFile, string SelectedCulter, string SelectedModule)
        {
            string culter = string.Empty;
            string fileName = string.Empty;
            string message = "";
            string status = "";
            try
            {

                if (!SelectedCulter.ToLower().Contains("en-us"))
                    culter = "." + SelectedCulter;

                fileName = SelectedResourceFile + culter + ".resx";
                string masterResourceDirPath = string.Empty;
                string CompanyFullDirPath = string.Empty;
                if (SelectedModule == "Enterprise")
                {
                    masterResourceDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\MasterResources\EnterpriseResources";
                    CompanyFullDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\" + SessionHelper.EnterPriceID.ToString() + @"\EnterpriseResources";
                }
                else
                {
                    masterResourceDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\MasterResources\CompanyResources";
                    CompanyFullDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\" + SessionHelper.EnterPriceID.ToString() + @"\CompanyResources";
                }

                string masterResourceFilePath = masterResourceDirPath + @"\" + fileName;

                if (!System.IO.Directory.Exists(CompanyFullDirPath))
                {
                    System.IO.Directory.CreateDirectory(CompanyFullDirPath);
                }

                System.IO.File.Copy(masterResourceFilePath, System.IO.Path.Combine(CompanyFullDirPath, fileName), true);
                message = string.Format(ResResourceEditor.UploadSuccess, HttpStatusCode.OK);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = ResMessage.SaveMessage;

            }
            catch (Exception)
            {
                //  throw ex;
                return Json(new { Message = ResResourceEditor.UploadNotSuccess, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {

            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region EditENTMobileResources

        private const string _CURRENTMOBILEENTRESOURCESESSIONKEY = "CURRENTMOBILEENTRESOURCELIST";

        /// <summary>
        /// Set Current Resource in Session
        /// </summary>
        private List<MobileResourcesDTO> CurrentMobileENTResourceList
        {
            get
            {
                if (HttpContext.Session[_CURRENTMOBILEENTRESOURCESESSIONKEY] != null)
                    return (List<MobileResourcesDTO>)HttpContext.Session[_CURRENTMOBILEENTRESOURCESESSIONKEY];
                return new List<MobileResourcesDTO>();
            }
            set
            {
                HttpContext.Session[_CURRENTMOBILEENTRESOURCESESSIONKEY] = value;
            }
        }


        /// <summary>
        /// Edit Resource View
        /// </summary>
        /// <returns></returns>
        public ActionResult EditENTMobileResource(string lang = "", string module = "", string PageID = "")
        {
            CurrentMobileENTResourceList = null;

            List<SelectListItem> lstLang = GetLanguageWithID();
            List<SelectListItem> lstModMast = GetResourceModule();
            List<SelectListItem> lstResourceFiles = GetMobileResourcePages(int.Parse(lstModMast[0].Value));

            if (!string.IsNullOrEmpty(lang))
            {
                foreach (var item in lstLang)
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
                        lstResourceFiles = GeteTurnsBaseMobileResourcePages(int.Parse(item.Value)); //GetBaseResourceFiles(int.Parse(item.Value));
                        break;
                    }
                }

                foreach (var item in lstResourceFiles)
                {
                    if (item.Value.ToLower() == PageID.ToLower())
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }

            ViewBag.DDLanguage = lstLang;
            ViewBag.DDLModuleMaster = lstModMast;
            ViewBag.DDLResourceFile = lstResourceFiles;

            return View();
        }


        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetENTMobileResourceList(JQueryDataTableParamModelForResource param)
        {
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            string sortDirection = "none";

            if (!string.IsNullOrEmpty(Request["sSortDir_0"]))
                sortDirection = Request["sSortDir_0"];

            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;


            string searchQuery = string.Empty;

            int TotalRecordCount = 0;


            List<MobileResourcesDTO> lstKeyVal = null;
            MobileResourcesDAL mobileResDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);


            //if (CurrentMobileResourceList == null || CurrentMobileResourceList.Count <= 0)
            //    CurrentMobileResourceList = ResourceHelper.GetResourceData(param.resourcefile, param.resourcelang);

            Int64 resourceID = 0;
            Int64 landID = 0;

            Int64.TryParse(param.resourcefile, out resourceID);
            Int64.TryParse(param.resourcelang, out landID);

            if (CurrentMobileENTResourceList == null || CurrentMobileENTResourceList.Count <= 0)
            {
                //IEnumerable<MobileResourcesDTO> lstMobileResData = mobileResDAL.GetAllRecords(0).ToList();
                //CurrentMobileENTResourceList = lstMobileResData.Where(x => x.ResourcePageID == resourceID && x.LanguageID == landID).ToList();
                CurrentMobileENTResourceList = mobileResDAL.GetAllMobileResources(0, resourceID, landID, string.Empty).ToList();
            }

            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch.Trim().Length > 0)
            {
                lstKeyVal = (from y in CurrentMobileENTResourceList
                             where y.ResourceKey.ToLower().Contains(param.sSearch.ToLower()) || y.ResourceValue.ToLower().Contains(param.sSearch.ToLower())
                             select y).ToList<MobileResourcesDTO>();
            }
            else
                lstKeyVal = CurrentMobileENTResourceList;



            if (sortDirection == "asc")
            {
                lstKeyVal = lstKeyVal.OrderBy(x => x.ResourceKey).ToList<MobileResourcesDTO>();
            }
            else if (sortDirection == "desc")
            {
                //sortColumnName = sortColumnName + " desc"; 
                lstKeyVal = lstKeyVal.OrderByDescending(x => x.ResourceKey).ToList<MobileResourcesDTO>();
            }

            TotalRecordCount = lstKeyVal.Count();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = lstKeyVal
            }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Save Resource into Resource file from Current Session
        /// </summary>
        /// <param name="SelectedResourceFile"></param>
        /// <param name="SelectedCulter"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveENTMobileResources(string SelectedResourceFile, string SelectedCulter, string arrItems = "")
        {
            string message = "";
            string status = "";

            JavaScriptSerializer s = new JavaScriptSerializer();
            KeyValCheckDTO[] LstKeyValCheckDTO = s.Deserialize<KeyValCheckDTO[]>(arrItems);

            ResourceHelper resHelper = new ResourceHelper();
            MobileResourcesDAL objMobileResDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
            try
            {
                //resHelper.SaveResources(SelectedResourceFile, SelectedCulter, CurrentResourceList);
                foreach (var item in CurrentMobileENTResourceList)
                {
                    item.UpdatedBy = SessionHelper.UserID;
                    objMobileResDAL.Edit(item);

                    if (LstKeyValCheckDTO != null && LstKeyValCheckDTO.Count() > 0 && LstKeyValCheckDTO.Where(x => x.key == item.ResourceKey && x.chkvalue == true).Count() > 0)
                    {
                        UpdateAllCompanyMobileResource(item);
                    }

                }

                message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = ResMessage.SaveMessage;//"fail";
                ClearCurrentResourceList();
            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = "fail";
            }
            finally
            {
                resHelper = null;
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update Data In Current Resource Session
        /// </summary> 
        /// <param name="changedKey"></param>
        /// <param name="changedValue"></param>
        /// <param name="Lang"></param>
        /// <param name="ResFile"></param>
        /// <returns></returns>
        public JsonResult UpdateDataInENTMobileResourceSession(string ID, string changedKey, string changedValue)
        {
            if (!string.IsNullOrEmpty(changedValue) && !string.IsNullOrWhiteSpace(changedValue) && CurrentMobileENTResourceList != null && CurrentMobileENTResourceList.Count > 0)
            {
                int indx = CurrentMobileENTResourceList.FindIndex(x => x.ResourceKey == changedKey);
                CurrentMobileENTResourceList[indx] = new MobileResourcesDTO() { ID = Int64.Parse(ID), ResourceKey = changedKey, ResourceValue = changedValue };
            }
            return Json(new { Message = "sucess", Status = "ok", Value = changedValue }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ResetENTMobileResource(Int64 ResourcePageID, Int64 LanguageID)
        {
            string culter = string.Empty;
            string fileName = string.Empty;
            string message = "";
            string status = "";
            try
            {
                MobileResourcesDAL mobResDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);

                mobResDAL.ResetMobileResource(ResourcePageID, LanguageID, SessionHelper.UserID, SessionHelper.CompanyID);
                message = string.Format(ResResourceEditor.UploadSuccess, HttpStatusCode.OK);
                status = ResMessage.SaveMessage;
            }
            catch (Exception)
            {
                //  throw ex;
                return Json(new { Message = ResResourceEditor.UploadNotSuccess, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {

            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region eTurns Base Resources

        private const string _CURRENTBASERESOURCESESSIONKEY = "CURRENTBASERESOURCESESSIONKEY";

        /// <summary>
        /// Set Current Resource in Session
        /// </summary>
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


        private const string _CURRENTETURNSBASERESOURCESESSIONKEY = "CURRENTETURNSBASERESOURCESESSIONKEY";

        /// <summary>
        /// Set Current Resource in Session
        /// </summary>
        private List<BaseResourcesDTO> CurrenteTurnsBaseResourceList
        {
            get
            {
                if (HttpContext.Session[_CURRENTETURNSBASERESOURCESESSIONKEY] != null)
                    return (List<BaseResourcesDTO>)HttpContext.Session[_CURRENTETURNSBASERESOURCESESSIONKEY];
                return new List<BaseResourcesDTO>();
            }
            set
            {
                HttpContext.Session[_CURRENTETURNSBASERESOURCESESSIONKEY] = value;
            }
        }
        //
        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetBaseResourceList(JQueryDataTableParamModelForResource param)
        {
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            string sortDirection = "none";

            if (!string.IsNullOrEmpty(Request["sSortDir_0"]))
                sortDirection = Request["sSortDir_0"];

            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            List<KeyValDTO> lstKeyVal = null;


            if (CurrentBaseResourceList == null || CurrentBaseResourceList.Count <= 0)
            {
                if (param.resourcemodule == "Enterprise")
                {
                    CurrentBaseResourceList = BaseResourceHelper.GetBaseEnterpriseResourceData(param.resourcefile, param.resourcelang);
                }
                else
                {
                    CurrentBaseResourceList = BaseResourceHelper.GetBaseResourceData(param.resourcefile, param.resourcelang);
                }
            }



            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch.Trim().Length > 0)
            {
                lstKeyVal = (from y in CurrentBaseResourceList
                             where y.key.ToLower().Contains(param.sSearch.ToLower()) || y.value.ToLower().Contains(param.sSearch.ToLower())
                             select y).ToList<KeyValDTO>();
            }
            else
                lstKeyVal = CurrentBaseResourceList;



            if (sortDirection == "asc")
            {
                lstKeyVal = lstKeyVal.OrderBy(x => x.key).ToList<KeyValDTO>();
            }
            else if (sortDirection == "desc")
            {
                //sortColumnName = sortColumnName + " desc"; 
                lstKeyVal = lstKeyVal.OrderByDescending(x => x.key).ToList<KeyValDTO>();
            }

            TotalRecordCount = lstKeyVal.Count();
            var result = from c in lstKeyVal
                         select new KeyValDTO
                         {
                             key = c.key,
                             value = c.value
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBaseResourceListNew(JQueryDataTableParamModelForResource param)
        {
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            string sortDirection = "asc";

            if (!string.IsNullOrEmpty(Request["sSortDir_0"]))
                sortDirection = Request["sSortDir_0"];

            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            string searchText = string.Empty;
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch.Trim().Length > 0)
                searchText = param.sSearch.Trim();
            sortColumnName = Request["SortingField"].ToString();
            if (sortColumnName == string.Empty)
                sortColumnName = "ResourceKey";
            sortColumnName += " " + sortDirection;

            int TotalRecordCount = 0;

            List<BaseResourcesDTO> lstBaseResources = null;
            eTurnsBaseResourceDAL BaseResDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());

            Int64 resourcePageID = 0;
            Int64 languageID = 0;

            Int64.TryParse(param.resourcefile, out resourcePageID);
            Int64.TryParse(param.resourcelang, out languageID);

            ////lstBaseResources = BaseResDAL.GetBaseResourceByResoucePageIDLanguageID(resourcePageID, languageID, sortColumnName);
            lstBaseResources = BaseResDAL.GetPagedBaseResource(resourcePageID, languageID, sortColumnName, searchText);
            if (string.IsNullOrWhiteSpace(searchText) || string.IsNullOrEmpty(searchText))
            {
                CurrenteTurnsBaseResourceList = lstBaseResources;
            }
            //if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch.Trim().Length > 0)
            //{
            //    lstBaseResources = (from y in lstBaseResources
            //                 where y.ResourceKey.ToLower().Contains(param.sSearch.ToLower()) || y.ResourceValue.ToLower().Contains(param.sSearch.ToLower())
            //                 select y).ToList<BaseResourcesDTO>();
            //}

            #region Comment
            /*CurrenteTurnsBaseResourceList = BaseResDAL.GetBaseResourceByResoucePageIDLanguageID(resourcePageID, languageID, sortColumnName);

            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch.Trim().Length > 0)
            {
                lstBaseResources = (from y in CurrenteTurnsBaseResourceList
                             where y.ResourceKey.ToLower().Contains(param.sSearch.ToLower()) || y.ResourceValue.ToLower().Contains(param.sSearch.ToLower())
                             select y).ToList<BaseResourcesDTO>();
            }
            else
                lstBaseResources = CurrenteTurnsBaseResourceList; */

            #endregion

            TotalRecordCount = lstBaseResources.Count();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = lstBaseResources
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Edit Resource View
        /// </summary>
        /// <returns></returns>
        public ActionResult EditBaseResource(string lang = "", string module = "", string page = "")
        {
            if ((SiteSettingHelper.EnforceResourcePagesRestriction ?? String.Empty) == "yes")
            {
                List<string> alloweduserIds = (SiteSettingHelper.AllowedUserIdsResources ?? String.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!alloweduserIds.Contains(SessionHelper.UserID.ToString()))
                {
                    return RedirectToAction(ActName, CtrlName);
                }
            }

            if (SessionHelper.UserType == 1 && SessionHelper.RoleID == -1)
            {
                CurrentBaseResourceList = null;
                List<SelectListItem> lstLang = GetETurnsBaseLanguage(); //GetBaseLanguage();
                GenerateBaseFilesOfNonExistsOrNewCulture("");


                List<SelectListItem> lstModMast = GetETurnsBaseResourceModule(); //GetBaseResourceModule();
                List<SelectListItem> lstResPage = GetETurnsBaseResourceFiles(int.Parse(lstModMast[0].Value)); //GetBaseResourceFiles(int.Parse(lstModMast[0].Value));

                if (!string.IsNullOrEmpty(lang))
                {
                    foreach (var item in lstLang)
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
                            lstResPage = GetETurnsBaseResourceFiles(int.Parse(item.Value)); //GetBaseResourceFiles(int.Parse(item.Value));
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

        public ActionResult EditBaseResourceNew(string lang = "", string module = "", string page = "")
        {
            if ((SiteSettingHelper.EnforceResourcePagesRestriction ?? String.Empty) == "yes")
            {
                List<string> alloweduserIds = (SiteSettingHelper.AllowedUserIdsResources ?? String.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!alloweduserIds.Contains(SessionHelper.UserID.ToString()))
                {
                    return RedirectToAction(ActName, CtrlName);
                }
            }
            if (SessionHelper.UserType == 1 && SessionHelper.RoleID == -1)
            {
                CurrenteTurnsBaseResourceList = null;
                List<SelectListItem> lstLang = GeteTurnsResourceLanguage();
                GenerateBaseFilesOfNonExistsOrNewCulture("");


                List<SelectListItem> lstModMast = GetResourceModuleData();
                List<SelectListItem> lstResPage = GetResourceModuleDetail(int.Parse(lstModMast[0].Value));

                if (!string.IsNullOrEmpty(lang))
                {
                    foreach (var item in lstLang)
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
                            lstResPage = GetResourceModuleDetail(int.Parse(lstModMast[0].Value));
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

        private List<SelectListItem> GetBaseResourceFiles(int ModuleID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleDetailsDTO> ResModDet = null;
            try
            {
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                ResModDet = objDAL.GetCachedResourceModuleDetailData(0, ModuleID);
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
        private List<SelectListItem> GetBaseResourceModule()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleMasterDTO> resModule = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resModule = objDAL.GetCachedResourceModuleMasterData(0);
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
        private List<SelectListItem> GetBaseLanguage()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resLangDTO = objDAL.GetCachedResourceLanguageData(0);
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.Culture;
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
                resLangDTO = null;
                lstItem = null;

            }

        }



        /// <summary>
        /// Clear Resource from Session
        /// </summary>
        public JsonResult ClearCurrentBaseResourceList()
        {
            CurrentBaseResourceList = null;
            return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ClearCurrenteTurnsBaseResourceList()
        {
            CurrenteTurnsBaseResourceList = null;
            return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Save Resource into Resource file from Current Session
        /// </summary>
        /// <param name="SelectedResourceFile"></param>
        /// <param name="SelectedCulter"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveBaseResources(string SelectedResourceFile, string SelectedCulter, string SelectedModule, string arrItems = "")
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            KeyValCheckDTO[] LstKeyValCheckDTO = s.Deserialize<KeyValCheckDTO[]>(arrItems);
            BaseResourceHelper resHelper = new BaseResourceHelper();
            try
            {
                if (SelectedModule == "Enterprise")
                {
                    resHelper.SaveBaseEnterpriseResources(SelectedResourceFile, SelectedCulter, CurrentBaseResourceList);
                }
                else
                {
                    resHelper.SaveBaseResources(SelectedResourceFile, SelectedCulter, CurrentBaseResourceList);
                    if (LstKeyValCheckDTO != null && LstKeyValCheckDTO.Count() > 0)
                    {
                        UpdateAllEnterpriseAndCompanyResource(LstKeyValCheckDTO.Where(t => t.chkvalue == true).ToList(), SelectedResourceFile, SelectedCulter);
                    }
                }

                message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = ResMessage.SaveMessage;//"fail";
                ClearCurrentBaseResourceList();
            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = "fail";
            }
            finally
            {
                resHelper = null;
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }
        public void UpdateAllEnterpriseAndCompanyResource(List<KeyValCheckDTO> lstKeyValCheckDTO, string filename, string culter)
        {
            try
            {

                if (lstKeyValCheckDTO != null && lstKeyValCheckDTO.Count > 0)
                {
                    List<EnterpriseDTO> lstEnterpriseDTO = new List<EnterpriseDTO>();
                    EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                    lstEnterpriseDTO = objEnterpriseMasterDAL.GetAllEnterprisesPlain();
                    foreach (EnterpriseDTO itemEnterprise in lstEnterpriseDTO)
                    {

                        List<SelectListItem> lstLang = GetLanguageWithDataBase(itemEnterprise.EnterpriseDBName);
                        foreach (var itemLang in lstLang)
                        {
                            culter = itemLang.Value;
                            BaseResourceHelper BaseresHelper = new BaseResourceHelper();
                            eTurns.DTO.Resources.Translator obj = new eTurns.DTO.Resources.Translator();
                            List<KeyValCheckDTO> objKeyText = new List<KeyValCheckDTO>();
                            string translatedText = string.Empty;

                            if (lstKeyValCheckDTO != null && lstKeyValCheckDTO.Count > 0)
                            {
                                foreach (var item in lstKeyValCheckDTO)
                                {
                                    //translatedText = obj.Translate((item.value ?? string.Empty).Trim(), "en-US", itemLang.Value);
                                    ResourceLanguageKey objResourceLanguageKey = new ResourceLanguageKey();
                                    if (lstResourceLanguageKey != null)
                                    {
                                        objResourceLanguageKey = lstResourceLanguageKey.Where(c => c.Language == itemLang.Value && c.Key == item.value).FirstOrDefault();
                                        if (objResourceLanguageKey == null)
                                        {
                                            eTurns.DTO.Resources.Translator objTRanslte = new eTurns.DTO.Resources.Translator();
                                            translatedText = objTRanslte.Translate((item.value ?? string.Empty), "en-US", itemLang.Value);
                                            objResourceLanguageKey = new ResourceLanguageKey();
                                            objResourceLanguageKey.Key = (item.value ?? string.Empty);
                                            objResourceLanguageKey.ConvertValue = translatedText;
                                            objResourceLanguageKey.Language = (itemLang.Value);
                                            lstResourceLanguageKey.Add(objResourceLanguageKey);
                                        }
                                        else
                                        {
                                            translatedText = objResourceLanguageKey.ConvertValue;
                                        }
                                    }
                                    else
                                    {
                                        lstResourceLanguageKey = new List<ResourceLanguageKey>();
                                        eTurns.DTO.Resources.Translator objTRanslte = new eTurns.DTO.Resources.Translator();
                                        translatedText = objTRanslte.Translate((item.value ?? string.Empty), "en-US", itemLang.Value);
                                        objResourceLanguageKey = new ResourceLanguageKey();
                                        objResourceLanguageKey.Key = (item.value ?? string.Empty);
                                        objResourceLanguageKey.ConvertValue = translatedText;
                                        objResourceLanguageKey.Language = (itemLang.Value);
                                        lstResourceLanguageKey.Add(objResourceLanguageKey);
                                    }
                                    if (!string.IsNullOrEmpty(translatedText))
                                    {
                                        objKeyText.Add(new KeyValCheckDTO { key = item.key, value = translatedText });
                                    }
                                    else
                                    {
                                        objKeyText.Add(new KeyValCheckDTO { key = item.key, value = (item.value ?? string.Empty).Trim() });
                                    }
                                }
                                //lstKeyValCheckDTO[0].value = translatedText;

                            }
                            CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                            List<CompanyMasterDTO> lstCompanyMasterDTO = new List<CompanyMasterDTO>();
                            lstCompanyMasterDTO = objCompanyMasterDAL.GetAllCompaniesByEnterpriseID(itemEnterprise.ID);
                            ResourceHelper resHelper = new ResourceHelper();
                            string TargetEnterpriseFilePath = GetERFilepath(culter, filename, Convert.ToString(itemEnterprise.ID));
                            string ResourceBaseFilePath = CommonUtility.ResourceBaseFilePath;
                            if (!(System.IO.File.Exists(TargetEnterpriseFilePath)))
                            {
                                string cul = string.Empty;
                                if (culter == "en-US")
                                    cul = "";
                                else
                                    cul = "." + culter;
                                string ResourceFileExt = ".resx";
                                if (!(System.IO.File.Exists(ResourceBaseFilePath + @"\MasterResources\CompanyResources\" + filename + cul + ResourceFileExt)))
                                {
                                    System.IO.File.Copy(ResourceBaseFilePath + @"\MasterResources\CompanyResources\" + filename + ResourceFileExt, ResourceBaseFilePath + @"\MasterResources\CompanyResources\" + filename + cul + ResourceFileExt);
                                }
                                if (!System.IO.Directory.Exists(ResourceBaseFilePath + @"\" + itemEnterprise.ID))
                                {
                                    System.IO.Directory.CreateDirectory(ResourceBaseFilePath + @"\" + itemEnterprise.ID);
                                }
                                //if (!(System.IO.File.Exists(Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\" + filename + cul + ResourceFileExt))))
                                //{
                                //    System.IO.File.Copy(Server.MapPath(@"\Resources\MasterResources\CompanyResources\" + filename + cul + ResourceFileExt), Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\" + filename + cul + ResourceFileExt));
                                //}
                                if (!System.IO.Directory.Exists(ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\CompanyResources"))
                                {
                                    System.IO.Directory.CreateDirectory(ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\CompanyResources");
                                }
                                if (!(System.IO.File.Exists(ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\CompanyResources\" + filename + cul + ResourceFileExt)))
                                {
                                    System.IO.File.Copy(ResourceBaseFilePath + @"\MasterResources\CompanyResources\" + filename + cul + ResourceFileExt, ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\CompanyResources\" + filename + cul + ResourceFileExt);
                                }
                                if (!(System.IO.File.Exists(ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\CompanyResources\" + filename + ResourceFileExt)))
                                {
                                    System.IO.File.Copy(ResourceBaseFilePath + @"\MasterResources\CompanyResources\" + filename + cul + ResourceFileExt, ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\CompanyResources\" + filename + ResourceFileExt);
                                }

                            }

                            resHelper.SaveResourcesAllCompany(TargetEnterpriseFilePath, objKeyText);
                            foreach (CompanyMasterDTO item in lstCompanyMasterDTO)
                            {

                                string TargetFilePath = GetCRFilepath(culter, filename, Convert.ToString(itemEnterprise.ID), Convert.ToString(item.ID));
                                // ResourceHelper.CheckAndCreateResourceFile(TargetFilePath, Convert.ToString(lstKeyValCheckDTO.FirstOrDefault().key), false);
                                if (!(System.IO.File.Exists(TargetFilePath)))
                                {
                                    string cul = string.Empty;
                                    if (culter == "en-US")
                                        cul = "";
                                    else
                                        cul = "." + culter;
                                    string ResourceFileExt = ".resx";
                                    if (!(System.IO.File.Exists(ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\CompanyResources\" + filename + cul + ResourceFileExt)))
                                    {
                                        if (!System.IO.File.Exists(ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\CompanyResources\" + filename + ResourceFileExt))
                                        {
                                            System.IO.File.Copy(ResourceBaseFilePath + @"\MasterResources\CompanyResources\" + filename + ResourceFileExt, ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\CompanyResources\" + filename + ResourceFileExt);
                                        }

                                        System.IO.File.Copy(ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\CompanyResources\" + filename + ResourceFileExt, ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\CompanyResources\" + filename + cul + ResourceFileExt);
                                    }
                                    if (!System.IO.Directory.Exists(ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\" + item.ID))
                                    {
                                        System.IO.Directory.CreateDirectory(ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\" + item.ID);
                                    }
                                    if (!(System.IO.File.Exists(ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\" + item.ID + @"\" + filename + cul + ResourceFileExt)))
                                    {
                                        System.IO.File.Copy(ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\CompanyResources\" + filename + ResourceFileExt, ResourceBaseFilePath + @"\" + itemEnterprise.ID + @"\" + item.ID + @"\" + filename + cul + ResourceFileExt);
                                    }
                                }
                                resHelper.SaveResourcesAllCompany(TargetFilePath, objKeyText);
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }





        public void EnterpriseAndCompanyOnlyAddResource(List<KeyValCheckDTO> lstKeyValCheckDTO, string filename, string culter)
        {
            //try
            //{
            //    //List<KeyValDTO> CurrentBaseResourceList = BaseResourceHelper.GetBaseResourceData(pageResourceFileName, "en-US");
            //    if (lstKeyValCheckDTO != null && lstKeyValCheckDTO.Count > 0)
            //    {
            //        List<EnterpriseDTO> lstEnterpriseDTO = new List<EnterpriseDTO>();
            //        EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            //        lstEnterpriseDTO = objEnterpriseMasterDAL.GetAllEnterprises(false, false).ToList();
            //        foreach (EnterpriseDTO itemEnterprise in lstEnterpriseDTO)
            //        {

            //            List<SelectListItem> lstLang = GetLanguageWithDataBase(itemEnterprise.EnterpriseDBName);
            //            foreach (var itemLang in lstLang)
            //            {
            //                culter = itemLang.Value;
            //                BaseResourceHelper BaseresHelper = new BaseResourceHelper();
            //                eTurns.DTO.Resources.Translator obj = new eTurns.DTO.Resources.Translator();
            //                string translatedText = obj.Translate(lstKeyValCheckDTO.FirstOrDefault().value, lstKeyValCheckDTO.FirstOrDefault().key, itemLang.Value);
            //                List<KeyValCheckDTO> objKeyText = new List<KeyValCheckDTO>();
            //                if (lstKeyValCheckDTO != null && (!string.IsNullOrWhiteSpace(translatedText)) && lstKeyValCheckDTO.Count > 0)
            //                {
            //                    //lstKeyValCheckDTO[0].value = translatedText;
            //                    objKeyText.Add(new KeyValCheckDTO { key = lstKeyValCheckDTO[0].key, value = translatedText });
            //                }
            //                CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
            //                List<CompanyMasterDTO> lstCompanyMasterDTO = new List<CompanyMasterDTO>();
            //                lstCompanyMasterDTO = objCompanyMasterDAL.GetAllCompaniesByEnterpriseID(itemEnterprise.ID);
            //                ResourceHelper resHelper = new ResourceHelper();
            //                string TargetEnterpriseFilePath = GetERFilepath(culter, filename, Convert.ToString(itemEnterprise.ID));
            //                if (!(System.IO.File.Exists(TargetEnterpriseFilePath)))
            //                {
            //                    string cul = string.Empty;
            //                    if (culter == "en-US")
            //                        cul = "";
            //                    else
            //                        cul = "." + culter;
            //                    string ResourceFileExt = ".resx";
            //                    if (!(System.IO.File.Exists(Server.MapPath(@"\Resources\MasterResources\CompanyResources\" + filename + cul + ResourceFileExt))))
            //                    {
            //                        System.IO.File.Copy(Server.MapPath(@"\Resources\MasterResources\CompanyResources\" + filename + ResourceFileExt), Server.MapPath(@"\Resources\MasterResources\CompanyResources\" + filename + cul + ResourceFileExt));
            //                    }
            //                    if (!System.IO.Directory.Exists(Server.MapPath(@"\Resources\" + itemEnterprise.ID)))
            //                    {
            //                        System.IO.Directory.CreateDirectory(Server.MapPath(@"\Resources\" + itemEnterprise.ID));
            //                    }
            //                    if (!(System.IO.File.Exists(Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\" + filename + cul + ResourceFileExt))))
            //                    {
            //                        System.IO.File.Copy(Server.MapPath(@"\Resources\MasterResources\CompanyResources\" + filename + cul + ResourceFileExt), Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\" + filename + cul + ResourceFileExt));
            //                    }
            //                    if (!(System.IO.File.Exists(Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\CompanyResources\" + filename + cul + ResourceFileExt))))
            //                    {
            //                        System.IO.File.Copy(Server.MapPath(@"\Resources\MasterResources\CompanyResources\" + filename + cul + ResourceFileExt), Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\CompanyResources\" + filename + cul + ResourceFileExt));
            //                    }
            //                    if (!(System.IO.File.Exists(Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\CompanyResources\" + filename + ResourceFileExt))))
            //                    {
            //                        System.IO.File.Copy(Server.MapPath(@"\Resources\MasterResources\CompanyResources\" + filename + cul + ResourceFileExt), Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\CompanyResources\" + filename + ResourceFileExt));
            //                    }

            //                }

            //                resHelper.AddResourcesAllCompany(TargetEnterpriseFilePath, objKeyText);
            //                foreach (CompanyMasterDTO item in lstCompanyMasterDTO)
            //                {

            //                    string TargetFilePath = GetCRFilepath(culter, filename, Convert.ToString(itemEnterprise.ID), Convert.ToString(item.ID));
            //                    // ResourceHelper.CheckAndCreateResourceFile(TargetFilePath, Convert.ToString(lstKeyValCheckDTO.FirstOrDefault().key), false);
            //                    if (!(System.IO.File.Exists(TargetFilePath)))
            //                    {
            //                        string cul = string.Empty;
            //                        if (culter == "en-US")
            //                            cul = "";
            //                        else
            //                            cul = "." + culter;
            //                        string ResourceFileExt = ".resx";
            //                        if (!(System.IO.File.Exists(Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\CompanyResources\" + filename + cul + ResourceFileExt))))
            //                        {
            //                            if (!System.IO.File.Exists(Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\CompanyResources\" + filename + ResourceFileExt)))
            //                            {
            //                                System.IO.File.Copy(Server.MapPath(@"\Resources\MasterResources\CompanyResources\" + filename + ResourceFileExt), Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\CompanyResources\" + filename + ResourceFileExt));
            //                            }

            //                            System.IO.File.Copy(Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\CompanyResources\" + filename + ResourceFileExt), Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\CompanyResources\" + filename + cul + ResourceFileExt));
            //                        }
            //                        if (!System.IO.Directory.Exists(Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\" + item.ID)))
            //                        {
            //                            System.IO.Directory.CreateDirectory(Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\" + item.ID));
            //                        }
            //                        if (!(System.IO.File.Exists(Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\" + item.ID + @"\" + filename + cul + ResourceFileExt))))
            //                        {
            //                            System.IO.File.Copy(Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\CompanyResources\" + filename + ResourceFileExt), Server.MapPath(@"\Resources\" + itemEnterprise.ID + @"\" + item.ID + @"\" + filename + cul + ResourceFileExt));
            //                        }
            //                    }
            //                    resHelper.AddResourcesAllCompany(TargetFilePath, objKeyText);
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
        }

        /// <summary>
        /// Update Data In Current Resource Session
        /// </summary> 
        /// <param name="changedKey"></param>
        /// <param name="changedValue"></param>
        /// <param name="Lang"></param>
        /// <param name="ResFile"></param>
        /// <returns></returns>
        public string UpdateDataBaseResourceSession(string changedKey, string changedValue)
        {
            if (!string.IsNullOrEmpty(changedValue) && !string.IsNullOrWhiteSpace(changedValue) && CurrentBaseResourceList != null && CurrentBaseResourceList.Count > 0)
            {
                int indx = CurrentBaseResourceList.FindIndex(x => x.key == changedKey);
                CurrentBaseResourceList[indx] = new KeyValDTO() { key = changedKey, value = changedValue };
            }
            return changedValue;
        }

        /// <summary>
        /// // below function used to load the User wise saved filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <returns></returns>
        public ActionResult LoadBaseGridState(string ListName)
        {
            string jsonData = @"{""iCreate"":1350639486123,""iStart"":0,""iEnd"":0,"",""aaSorting"":[[0,""none"",0,""Name""]]
                                   ,""oSearch"":{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,""aoSearchCols"":[{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}]
                                    ,""abVisCols"":[true,true],""ColReorder"":[0,1]}";
            return Json(new { jsonData }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// // below function used to Save the User wise filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <param name="Data"></param> will have json string of filter criteria 
        /// <returns></returns>
        public ActionResult SaveBaseGridState(string Data, string ListName)
        {
            string jsonData = @"{""iCreate"":1350639486123,""iStart"":0,""iEnd"":0,""iLength"":10,""aaSorting"":[[0,""asc"",0,""Name""]]
                                   ,""oSearch"":{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,""aoSearchCols"":[{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}
                                   ,{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}]
                                    ,""abVisCols"":[false,true,true,true,true],""ColReorder"":[0,1]}";
            return Json(new { jsonData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBaseResourceFilesData(string ModuleID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();

            try
            {
                lstItem = GetETurnsBaseResourceFiles(int.Parse(ModuleID)); //GetBaseResourceFiles(int.Parse(ModuleID));

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

        private List<SelectListItem> GetETurnsBaseResourceModuleData()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleMasterDTO> resModule = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
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

        private List<SelectListItem> GetResourceModuleData()
        {
            eTurnsBaseResourceDAL objDAL = null;
            IEnumerable<ResourceModuleMasterDTO> resModule = null;
            List<SelectListItem> lstItem = null;
            try
            {
                string resFileName = "ResModuleResource";
                objDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
                resModule = objDAL.GetResourceModuleMasterData();
                lstItem = new List<SelectListItem>();
                foreach (var item in resModule)
                {
                    SelectListItem obj = new SelectListItem();
                    string resValue = string.Empty;
                    if (!string.IsNullOrEmpty(item.ResModuleKey))
                    {
                        resValue = eTurns.DTO.Resources.ResourceModuleHelper.GetModuleResource(item.ResModuleKey, resFileName);
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
                    //obj.Text = item.ModuleName;
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

        private List<SelectListItem> GetEnterPriseResourceModuleData()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleMasterDTO> resModule = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
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


        private FileInfo[] GetOnlyenUSBaseFiles(FileInfo[] BaseAllFiles)
        {
            ResourceDAL objResourceDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
            List<ResourceLanguageDTO> objResourceLanguageList = new List<ResourceLanguageDTO>();
            objResourceLanguageList = objResourceDAL.GetResourceLanguageDataFromXML(Convert.ToString(Server.MapPath("/Content/LanguageResource.xml"))).Where(x => x.Culture != "en-US").ToList();
            string[] cultures = objResourceLanguageList.Select(x => x.Culture).ToArray();

            return BaseAllFiles = BaseAllFiles.Where(x => x.Extension.ToLower().Equals(".resx")
                                                && x.Name.Replace(x.Extension, "").IndexOf('.') < 0).ToArray();

            //return BaseAllFiles = BaseAllFiles.Where(x =>
            //{
            //    if (Array.Exists(cultures, s => x.Name.Contains(s)) || x.Name.EndsWith("..resx"))
            //        return false;
            //    else
            //        return true;
            //}).ToArray();
        }

        private bool IsCultureFilesExists(string cultureCode)
        {
            string appPath1 = ResourceHelper.ResourceDirectoryBasePath + @"\";

            string BasePathResource = CommonUtility.ResourceBaseFilePath + @"\";

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

        public void GenerateBaseFilesOfNonExistsOrNewCulture(string cultureCode)
        {
            if (string.IsNullOrEmpty(cultureCode))
                return;

            string appPath1 = ResourceHelper.ResourceDirectoryBasePath + @"\";

            string BasePathResource = CommonUtility.ResourceBaseFilePath + @"\";
            string BaseCompanyResourcesFolder = BasePathResource + "MasterResources\\CompanyResources";
            string BaseEnterpriseResourcesFolder = BasePathResource + "MasterResources\\EnterpriseResources";
            string BaseRoomResourcesFolder = BasePathResource + "MasterResources\\RoomResources";

            CopyBaseFilesByCultureCode(cultureCode, BaseCompanyResourcesFolder);
            CopyBaseFilesByCultureCode(cultureCode, BaseEnterpriseResourcesFolder);
            CopyBaseFilesByCultureCode(cultureCode, BaseRoomResourcesFolder);

        }



        #endregion

        #region Edit Base Mobile Resources

        private const string _CURRENTMOBILEBASERESOURCESESSIONKEY = "CURRENTMOBILEBASERESOURCESESSIONKEY";

        /// <summary>
        /// Set Current Resource in Session
        /// </summary>
        private List<MobileResourcesDTO> CurrentMobileBaseResourceList
        {
            get
            {
                if (HttpContext.Session[_CURRENTMOBILEBASERESOURCESESSIONKEY] != null)
                    return (List<MobileResourcesDTO>)HttpContext.Session[_CURRENTMOBILEBASERESOURCESESSIONKEY];
                return new List<MobileResourcesDTO>();
            }
            set
            {
                HttpContext.Session[_CURRENTMOBILEBASERESOURCESESSIONKEY] = value;
            }
        }

        /// <summary>
        /// Get Base Language
        /// </summary>
        /// <returns></returns>
        /*private List<SelectListItem> GeteTurnsMasterBaseLanguageWithID()
        {
            eTurnsMasterMobileResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new eTurnsMasterMobileResourceDAL(SessionHelper.EnterPriseDBName);
                resLangDTO = objDAL.GetResourceLanguages();
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.ID.ToString();
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

        }*/

        /// <summary>
        /// GeteTurnsBaseLanguageWithID
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GeteTurnsBaseLanguageWithID()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
                resLangDTO = objDAL.GetETurnsResourceLanguageData(); //GetResourceLanguages();
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.ID.ToString();
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

        private List<SelectListItem> GeteTurnsBaseLanguage()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
                resLangDTO = objDAL.GetETurnsResourceLanguage();
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.ID.ToString();
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
                    obj.Value = item.ID.ToString();
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

        /// <summary>
        /// GetLanguage
        /// </summary>
        /// <returns></returns>
        /*private List<SelectListItem> GeteTurnsMasterBaseResourceModule()
        {
            eTurnsMasterMobileResourceDAL objDAL = null;
            IEnumerable<ResourceModuleMasterDTO> resModule = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new eTurnsMasterMobileResourceDAL(SessionHelper.EnterPriseDBName);
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

            finally
            {
                objDAL = null;
                resModule = null;
                lstItem = null;

            }

        }*/

        /// <summary>
        /// GetResourceFiles
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        /*private List<SelectListItem> GeteTurnsMasterBaseMobileResourcePages(Int64 ModuleID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            eTurnsMasterMobileResourceDAL objDAL = null;
            IEnumerable<ResourceModuleDetailsDTO> ResModDet = null;
            try
            {
                objDAL = new eTurnsMasterMobileResourceDAL(SessionHelper.EnterPriseDBName);
                ResModDet = objDAL.GetResourceModuleDetailData_Mobile(ModuleID);
                lstItem = new List<SelectListItem>();
                if (ResModDet != null && ResModDet.Count() > 0)
                {
                    foreach (var item in ResModDet)
                    {
                        SelectListItem obj = new SelectListItem();
                        obj.Text = item.DisplayPageName;
                        obj.Value = item.ID.ToString();
                        lstItem.Add(obj);
                    }
                }

                if (lstItem == null || lstItem.Count < 0)
                    lstItem = new List<SelectListItem>();

                SelectListItem Item = new SelectListItem() { Text = "Select Page", Value = "0", Selected = true };
                lstItem.Insert(0, Item);

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



        }*/

        /// <summary>
        /// GeteTurnsBaseMobileResourcePages
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        private List<SelectListItem> GeteTurnsBaseMobileResourcePages(Int64 ModuleID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleDetailsDTO> ResModDet = null;
            try
            {
                string resFileName = "ResModuleResource";
                objDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
                ResModDet = objDAL.GetResourceModuleDetailData_Mobile(ModuleID);
                lstItem = new List<SelectListItem>();
                if (ResModDet != null && ResModDet.Count() > 0)
                {
                    foreach (var item in ResModDet)
                    {
                        SelectListItem obj = new SelectListItem();
                        string resValue = string.Empty;
                        if (!string.IsNullOrEmpty(item.ResMobModuleDetailsKey))
                        {
                            resValue = eTurns.DTO.Resources.ResourceHelper.GetModuleResource(item.ResMobModuleDetailsKey, resFileName);
                            if (!string.IsNullOrEmpty(resValue) && resValue != item.ResMobModuleDetailsKey)
                            {
                                obj.Text = resValue;
                            }
                            else
                            {
                                obj.Text = item.DisplayPageName;
                            }
                        }
                        else
                        {
                            obj.Text = item.DisplayPageName;
                        }
                        //obj.Text = item.DisplayPageName;
                        obj.Value = item.ID.ToString();
                        lstItem.Add(obj);
                    }
                }

                if (lstItem == null || lstItem.Count < 0)
                    lstItem = new List<SelectListItem>();

                SelectListItem Item = new SelectListItem() { Text = ResCommon.SelectPage, Value = "0", Selected = true };
                lstItem.Insert(0, Item);

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

        private List<SelectListItem> GeteTurnsBaseResourcePages(Int64 ModuleID)
        {
            List<SelectListItem> lstItem = null;
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleDetailsDTO> ResModDet = null;
            try
            {
                string resFileName = "ResModuleResource";
                objDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
                ResModDet = objDAL.GetResourceModuleDetailData_Mobile(ModuleID);
                lstItem = new List<SelectListItem>();
                if (ResModDet != null && ResModDet.Count() > 0)
                {
                    foreach (var item in ResModDet)
                    {
                        SelectListItem obj = new SelectListItem();
                        string resValue = string.Empty;
                        if (!string.IsNullOrEmpty(item.ResMobModuleDetailsKey))
                        {
                            resValue = eTurns.DTO.Resources.ResourceHelper.GetModuleResource(item.ResMobModuleDetailsKey, resFileName);
                            if (!string.IsNullOrEmpty(resValue) && resValue != item.ResMobModuleDetailsKey)
                            {
                                obj.Text = resValue;
                            }
                            else
                            {
                                obj.Text = item.DisplayPageName;
                            }
                        }
                        else
                        {
                            obj.Text = item.DisplayPageName;
                        }
                        //obj.Text = item.DisplayPageName;
                        obj.Value = item.ID.ToString();
                        lstItem.Add(obj);
                    }
                }

                if (lstItem == null || lstItem.Count < 0)
                    lstItem = new List<SelectListItem>();

                SelectListItem Item = new SelectListItem() { Text = ResCommon.SelectPage, Value = "0", Selected = true };
                lstItem.Insert(0, Item);

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

        private List<SelectListItem> GeteTurnsBaseResourceModuleDetail(Int64 ModuleID)
        {
            List<SelectListItem> lstItem = null;
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleDetailsDTO> ResModDet = null;
            try
            {

                objDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
                ResModDet = objDAL.GetResourceModuleDetailData(ModuleID);
                lstItem = new List<SelectListItem>();
                if (ResModDet != null && ResModDet.Count() > 0)
                {
                    foreach (var item in ResModDet)
                    {
                        SelectListItem obj = new SelectListItem();
                        obj.Text = item.DisplayFileName;
                        obj.Value = item.ID.ToString();
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
                        obj.Value = item.ID.ToString();
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

        private List<SelectListItem> GetEnterpriseResourceModuleDetail(Int64 ModuleID)
        {
            List<SelectListItem> lstItem = null;
            ResourceDAL objDAL = null;
            IEnumerable<ResourceModuleDetailsDTO> ResModDet = null;
            try
            {

                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                ResModDet = objDAL.GetResourceModuleDetailData(ModuleID);
                lstItem = new List<SelectListItem>();
                if (ResModDet != null && ResModDet.Count() > 0)
                {
                    foreach (var item in ResModDet)
                    {
                        SelectListItem obj = new SelectListItem();
                        obj.Text = item.DisplayFileName;
                        obj.Value = item.ID.ToString();
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

        /// <summary>
        /// Edit Resource View
        /// </summary>
        /// <returns></returns>
        public ActionResult EditBaseMobileResource(string lang = "", string module = "", string PageID = "")
        {
            if (SessionHelper.UserType == 1 && SessionHelper.RoleID == -1)
            {
                CurrentMobileBaseResourceList = null;
                List<SelectListItem> lstLang = GeteTurnsBaseLanguageWithID();
                List<SelectListItem> lstModMast = GetETurnsBaseResourceModule(); //GeteTurnsMasterBaseResourceModule();
                List<SelectListItem> lstResourceFiles = GeteTurnsBaseMobileResourcePages(int.Parse(lstModMast[0].Value)); // GeteTurnsMasterBaseMobileResourcePages(int.Parse(lstModMast[0].Value));

                if (!string.IsNullOrEmpty(lang))
                {
                    foreach (var item in lstLang)
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
                            lstResourceFiles = GeteTurnsBaseMobileResourcePages(int.Parse(item.Value)); //GetBaseResourceFiles(int.Parse(item.Value));
                            break;
                        }
                    }

                    foreach (var item in lstResourceFiles)
                    {
                        if (item.Value.ToLower() == PageID.ToLower())
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }

                ViewBag.DDLResourceFile = lstResourceFiles;
                ViewBag.DDLanguage = lstLang;
                ViewBag.DDLModuleMaster = lstModMast;
                return View();
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetBaseMobileResourceList(JQueryDataTableParamModelForResource param)
        {
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            string sortDirection = "none";

            if (!string.IsNullOrEmpty(Request["sSortDir_0"]))
                sortDirection = Request["sSortDir_0"];

            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;


            string searchQuery = string.Empty;

            int TotalRecordCount = 0;


            List<MobileResourcesDTO> lstKeyVal = null;
            //eTurnsMasterMobileResourceDAL mobileResDAL = new eTurnsMasterMobileResourceDAL(SessionHelper.EnterPriseDBName);
            ResourceDAL mobileResDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());

            //if (CurrentMobileResourceList == null || CurrentMobileResourceList.Count <= 0)
            //    CurrentMobileResourceList = ResourceHelper.GetResourceData(param.resourcefile, param.resourcelang);

            Int64 resourceID = 0;
            Int64 landID = 0;

            Int64.TryParse(param.resourcefile, out resourceID);
            Int64.TryParse(param.resourcelang, out landID);

            if (CurrentMobileBaseResourceList == null || CurrentMobileBaseResourceList.Count <= 0)
            {
                IEnumerable<MobileResourcesDTO> lstMobileResData = mobileResDAL.GetAllMobileResourceRecords().ToList();
                CurrentMobileBaseResourceList = lstMobileResData.Where(x => x.ResourcePageID == resourceID && x.LanguageID == landID).ToList();
            }

            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch.Trim().Length > 0)
            {
                lstKeyVal = (from y in CurrentMobileBaseResourceList
                             where y.ResourceKey.ToLower().Contains(param.sSearch.ToLower()) || y.ResourceValue.ToLower().Contains(param.sSearch.ToLower())
                             select y).ToList<MobileResourcesDTO>();
            }
            else
                lstKeyVal = CurrentMobileBaseResourceList;



            if (sortDirection == "asc")
            {
                lstKeyVal = lstKeyVal.OrderBy(x => x.ResourceKey).ToList<MobileResourcesDTO>();
            }
            else if (sortDirection == "desc")
            {
                //sortColumnName = sortColumnName + " desc"; 
                lstKeyVal = lstKeyVal.OrderByDescending(x => x.ResourceKey).ToList<MobileResourcesDTO>();
            }

            TotalRecordCount = lstKeyVal.Count();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = lstKeyVal
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save Resource into Resource file from Current Session
        /// </summary>
        /// <param name="SelectedResourceFile"></param>
        /// <param name="SelectedCulter"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveBaseMobileResources(string SelectedResourceFile, string SelectedCulter)
        {
            string message = "";
            string status = "";

            ResourceHelper resHelper = new ResourceHelper();
            // eTurnsMasterMobileResourceDAL objMobileResDAL = new eTurnsMasterMobileResourceDAL(SessionHelper.EnterPriseDBName);
            ResourceDAL objMobileResDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
            try
            {
                //resHelper.SaveResources(SelectedResourceFile, SelectedCulter, CurrentResourceList);
                foreach (var item in CurrentMobileBaseResourceList)
                {
                    item.UpdatedBy = SessionHelper.UserID;
                    objMobileResDAL.Edit(item);

                }

                message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = ResMessage.SaveMessage;//"fail";
                ClearCurrentResourceList();
            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = "fail";
            }
            finally
            {
                resHelper = null;
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update Data In Current Resource Session
        /// </summary> 
        /// <param name="changedKey"></param>
        /// <param name="changedValue"></param>
        /// <param name="Lang"></param>
        /// <param name="ResFile"></param>
        /// <returns></returns>
        public JsonResult UpdateDataInBaseMobileResourceSession(string ID, string changedKey, string changedValue)
        {
            if (!string.IsNullOrEmpty(changedValue) && !string.IsNullOrWhiteSpace(changedValue) && CurrentMobileBaseResourceList != null && CurrentMobileBaseResourceList.Count > 0)
            {
                int indx = CurrentMobileBaseResourceList.FindIndex(x => x.ResourceKey == changedKey);
                CurrentMobileBaseResourceList[indx] = new MobileResourcesDTO() { ID = Int64.Parse(ID), ResourceKey = changedKey, ResourceValue = changedValue };
            }
            return Json(new { Message = "sucess", Status = "ok", Value = changedValue }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult UpdateDataIneTurnsBaseResource(string ID, string changedKey, string changedValue, string AccrossEnt, string IsEdit)
        {
            //System.Xml.Linq.XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            int ResourceSaveMethod = (int)ResourceSave.ButtonSave;
            //if (Settinfile != null && Settinfile.Element("ResourceSave") != null)
            //    ResourceSaveMethod = Convert.ToInt32(Settinfile.Element("ResourceSave").Value);

            if (SiteSettingHelper.ResourceSave != string.Empty)
                ResourceSaveMethod = Convert.ToInt32(SiteSettingHelper.ResourceSave);

            if (ResourceSaveMethod == (int)ResourceSave.OnChange || !string.IsNullOrWhiteSpace(IsEdit))
            {
                changedValue = changedValue.Trim();
                bool IsAcrossAll = false;
                if (!string.IsNullOrEmpty(changedValue) && !string.IsNullOrWhiteSpace(changedValue))
                {
                    eTurnsBaseResourceDAL BaseResDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
                    if (!string.IsNullOrEmpty(AccrossEnt) && !string.IsNullOrWhiteSpace(AccrossEnt))
                        IsAcrossAll = Convert.ToBoolean(AccrossEnt);
                    BaseResDAL.UpdateBaseResourceByID(Int64.Parse(ID), changedKey, changedValue, SessionHelper.UserID, "Web", IsAcrossAll);
                    if (CurrenteTurnsBaseResourceList != null && CurrenteTurnsBaseResourceList.Count > 0)
                    {
                        int indx = CurrenteTurnsBaseResourceList.FindIndex(x => x.ResourceKey == changedKey);
                        if (indx > 0)
                        {
                            BaseResourcesDTO objDTO = CurrenteTurnsBaseResourceList.Where(x => x.ID == Convert.ToInt64(ID)).FirstOrDefault();
                            if (objDTO != null)
                            {
                                objDTO.ResourceValue = changedValue;
                                objDTO.EditedFrom = "Web";
                                objDTO.LastUpdatedBy = SessionHelper.UserID;
                                objDTO.UpdatedByName = SessionHelper.UserName;
                                objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                                CurrenteTurnsBaseResourceList[indx] = objDTO;
                            }
                        }
                    }
                }

                eTurns.DAL.CacheHelper<IEnumerable<BaseResourcesKeyValDTO>>.InvalidateCacheByKeyStartWith("ResDB");
                AngularApp.AppConfigController.InvalidateCacheByKeyStartWith("ResDB");
            }
            return Json(new { Message = "sucess", Status = "ok", Value = changedValue }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GetBaseMobileResourcePageData
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public JsonResult GetBaseMobileResourcePageData(Int64 ModuleID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();

            try
            {
                lstItem = GeteTurnsBaseMobileResourcePages(ModuleID);

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

        /// <summary>
        /// GetBaseMobileResourcePages
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        /*private List<SelectListItem> GetBaseMobileResourcePages(Int64 ModuleID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            eTurnsMasterMobileResourceDAL objDAL = null;
            IEnumerable<ResourceModuleDetailsDTO> ResModDet = null;
            try
            {
                objDAL = new eTurnsMasterMobileResourceDAL(SessionHelper.EnterPriseDBName);
                ResModDet = objDAL.GetResourceModuleDetailData_Mobile(ModuleID);
                lstItem = new List<SelectListItem>();
                if (ResModDet != null && ResModDet.Count() > 0)
                {
                    foreach (var item in ResModDet)
                    {
                        SelectListItem obj = new SelectListItem();
                        obj.Text = item.DisplayPageName;
                        obj.Value = item.ID.ToString();
                        lstItem.Add(obj);
                    }
                }

                if (lstItem == null || lstItem.Count < 0)
                    lstItem = new List<SelectListItem>();

                SelectListItem Item = new SelectListItem() { Text = "Select Page", Value = "0", Selected = true };
                lstItem.Insert(0, Item);

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



        }*/

        #endregion


        #region CultureSetup

        public ActionResult CultureSetup()
        {
            CurrentBaseResourceList = null;
            ResourceDAL objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
            eTurnsMaster.DAL.CommonMasterDAL objMaster = new eTurnsMaster.DAL.CommonMasterDAL();
            List<ResourceLanguageDTO> lstgridcolumnssource = objMaster.GetCachedResourceLanguageData(0).ToList();
            List<ResourceLanguageDTO> lstgridcolumnsdestination = objDAL.GetETurnsResourceLanguageData().ToList();

            var Source = from e in lstgridcolumnssource select e.Language;
            var Destination = from f in lstgridcolumnsdestination select f.Language;
            var NewSource = Source.Except(Destination);

            List<ResourceLanguageDTO> lstNewSource = new List<ResourceLanguageDTO>();
            ResourceLanguageDTO objResource;
            foreach (var item in NewSource)
            {
                objResource = lstgridcolumnssource.Where(x => x.Language == item).FirstOrDefault();
                lstNewSource.Add(objResource);
            }

            ViewBag.SourceList = lstNewSource; // lstgridcolumnssource;
            ViewBag.DestinationList = lstgridcolumnsdestination;
            TempData["DestColumnOld"] = lstgridcolumnsdestination;

            return View();

        }

        [HttpPost]
        public ActionResult SaveCultureSettings(string datacollections)
        {
            var OldColumn = (IEnumerable<string>)null;
            var NewColumn = (IEnumerable<string>)null;
            var RemoveColumn = (IEnumerable<string>)null;
            var AddColumn = (IEnumerable<string>)null;
            var DeserializedObjList = (List<eTurns.DTO.Resources.ResourceLanguageDTO>)null;
            //string status = "";

            List<ResourceLanguageDTO> lstgridcolumnsdestinationOld = null;
            if (TempData["DestColumnOld"] != null)
            {
                lstgridcolumnsdestinationOld = (List<ResourceLanguageDTO>)TempData["DestColumnOld"];
                OldColumn = from e in lstgridcolumnsdestinationOld select e.Culture;
            }
            datacollections = System.Web.HttpUtility.UrlDecode(datacollections);
            if (!string.IsNullOrEmpty(datacollections) && Convert.ToString(datacollections) != "[]") // New Destionation Column not null
            {
                DeserializedObjList = (List<eTurns.DTO.Resources.ResourceLanguageDTO>)JsonConvert.DeserializeObject(datacollections, typeof(List<eTurns.DTO.Resources.ResourceLanguageDTO>));
                if (DeserializedObjList.Any())
                {
                    NewColumn = from f in DeserializedObjList select f.Culture;

                    if (OldColumn != (IEnumerable<string>)null)
                        RemoveColumn = OldColumn.Except(NewColumn);

                    if (OldColumn != (IEnumerable<string>)null)
                        AddColumn = NewColumn.Except(OldColumn);
                    else
                        AddColumn = NewColumn;
                }
            }
            else
            {
                RemoveColumn = OldColumn;
            }

            if (RemoveColumn != (IEnumerable<string>)null || AddColumn != (IEnumerable<string>)null)
            {
                if (RemoveColumn != (IEnumerable<string>)null && RemoveColumn.Contains("en-US")) // English Not allowed to delete.
                {
                    RemoveColumn = from item in RemoveColumn
                                   where item != "en-US"
                                   select item;
                }
                string ResourceBaseFilePath = CommonUtility.ResourceBaseFilePath;
                ResourceDAL objDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
                string BasePathResource = ResourceBaseFilePath + @"//MasterResources/";
                string CompanyResourcesFolder = BasePathResource + "CompanyResources";
                string EnterpriseResourcesFolder = BasePathResource + "EnterpriseResources";
                string EmailTemplateFolder = System.Web.HttpContext.Current.Server.MapPath(@"/Content//EmailTemplates");

                DirectoryInfo CompanyResource = new DirectoryInfo(CompanyResourcesFolder);
                DirectoryInfo EnterpriseResource = new DirectoryInfo(EnterpriseResourcesFolder);
                DirectoryInfo EmailTemplateResource = new DirectoryInfo(EmailTemplateFolder);

                FileInfo[] CompanyFiles = CompanyResource.GetFiles("*.resx");
                FileInfo[] EnterPriseFiles = EnterpriseResource.GetFiles("*.resx");
                FileInfo[] EmailtemplateFiles = EmailTemplateResource.GetFiles("*.htm");

                foreach (var item in RemoveColumn) // Remove Operation
                {
                    foreach (var CFiles in CompanyFiles) // Remove CompanyResources
                    {
                        if (CFiles.Name.Contains(item))
                        {
                            if (System.IO.File.Exists(CompanyResource + "//" + CFiles))
                            {
                                System.IO.File.Delete(CompanyResource + "//" + CFiles);
                            }
                        }
                    }

                    foreach (var EFiles in EnterPriseFiles) // Remove EnterpriseResources
                    {
                        if (EFiles.Name.Contains(item))
                        {
                            if (System.IO.File.Exists(EnterpriseResource + "//" + EFiles))
                            {
                                System.IO.File.Delete(EnterpriseResource + "//" + EFiles);
                            }
                        }
                    }

                    //foreach (var emailfiles in emailtemplatefiles) // remove emailtemplatefiles 
                    //{
                    //    if (emailfiles.name.contains(item))
                    //    {
                    //        if (system.io.file.exists(emailtemplateresource + "//" + emailfiles))
                    //        {
                    //            system.io.file.delete(emailtemplateresource + "//" + emailfiles);
                    //        }
                    //    }
                    //}

                    ResourceLanguageDTO RLDTO = lstgridcolumnsdestinationOld.Where(x => x.Culture == item).FirstOrDefault();
                    objDAL.DeleteETurnsResourceLanguageData(RLDTO);
                }

                foreach (var item in AddColumn) // Add Operation 
                {
                    string strSource, strDest;

                    foreach (var CFiles in CompanyFiles) // Add CompanyResources
                    {
                        if (!CFiles.Name.Contains("-"))
                        {
                            strSource = CompanyResource + "\\" + CFiles;
                            strDest = CompanyResource + "\\" + CFiles.Name.Replace(".", "." + item + ".");
                            if (System.IO.File.Exists(strDest))
                            {
                                System.IO.File.Delete(strDest);
                            }
                            System.IO.File.Copy(strSource, strDest);
                        }
                    }

                    foreach (var EFiles in EnterPriseFiles) // Add EnterpriseResources
                    {
                        if (!EFiles.Name.Contains("-"))
                        {
                            strSource = EnterpriseResource + "\\" + EFiles;
                            strDest = EnterpriseResource + "\\" + EFiles.Name.Replace(".", "." + item + ".");
                            if (System.IO.File.Exists(strDest))
                            {
                                System.IO.File.Delete(strDest);
                            }
                            System.IO.File.Copy(strSource, strDest);
                        }
                    }

                    foreach (var EmailFiles in EmailtemplateFiles) // Add EmailTemplate 
                    {
                        if (EmailFiles.Name.Contains("-en-US"))
                        {
                            strSource = EmailTemplateResource + "\\" + EmailFiles;
                            strDest = EmailTemplateResource + "\\" + EmailFiles.Name.Replace("-en-US", "-" + item);
                            if (System.IO.File.Exists(strDest) == false)
                            {
                                System.IO.File.Copy(strSource, strDest);
                            }

                        }
                    }

                    ResourceLanguageDTO RLDTO = DeserializedObjList.Where(x => x.Culture == item).FirstOrDefault();
                    objDAL.AddResourceLanguageData(RLDTO);

                }

            }
            return RedirectToAction("CultureSetup");
        }


        #endregion


        #region Export Import Resources

        string separator = ConfigurationManager.AppSettings["CSVseparator"].ToString();

        /// <summary>
        /// Get Langauges For Import Export
        /// </summary>
        /// <returns></returns>
        private List<KeyValDTO> GetLangaugesForImportExport(string resType)
        {
            List<KeyValDTO> lstLanguageKeyVal = new List<KeyValDTO>();
            List<SelectListItem> lstLanguage = null;

            if (resType.ToUpper() == "ETURNSBASE")
            {
                lstLanguage = GetETurnsBaseLanguage(); //GetBaseLanguage();
            }
            else if (resType.ToUpper() == "ENTERPRISEBASE")
            {
                lstLanguage = GetENTLanguage();
            }
            else if (resType.ToUpper() == "COMPANYBASE")
            {
                lstLanguage = GetLanguage();
            }


            foreach (var item in lstLanguage)
            {
                KeyValDTO objKeyVal = new KeyValDTO();
                objKeyVal.key = item.Value;
                objKeyVal.value = item.Text;

                if (item.Value.Contains("en-US"))
                {
                    lstLanguageKeyVal.Insert(0, objKeyVal);
                }
                else
                {
                    lstLanguageKeyVal.Add(objKeyVal);
                }
            }

            return lstLanguageKeyVal;
        }

        /// <summary>
        /// Get Langauges For Import Export
        /// </summary>
        /// <returns></returns>
        private List<ResourceLanguageDTO> GetLangaugesForMobileImportExport(string resType)
        {
            List<KeyValDTO> lstLanguageKeyVal = new List<KeyValDTO>();
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<ResourceLanguageDTO> NewResLangDTO = new List<ResourceLanguageDTO>();
            ResourceDAL objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
            //ResourceDAL objeTurnsDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
            if (resType.ToUpper() == "ETURNSBASE")
            {
                //resLangDTO = objDAL.GetCachedResourceLanguageData(0);
                resLangDTO = objDAL.GetETurnsResourceLanguageData();
            }
            else if (resType.ToUpper() == "ENTERPRISEBASE")
            {
                resLangDTO = objDAL.GetCachedResourceLanguageData(0);
            }
            else if (resType.ToUpper() == "COMPANYBASE")
            {
                resLangDTO = objDAL.GetCachedResourceLanguageData(0);
            }


            foreach (var item in resLangDTO)
            {
                ResourceLanguageDTO objDTO = new ResourceLanguageDTO()
                {
                    ID = item.ID,
                    Culture = item.Culture,
                    Language = item.Language,
                };

                if (item.Culture.Contains("en-US"))
                {
                    NewResLangDTO.Insert(0, objDTO);
                }
                else
                {
                    NewResLangDTO.Add(objDTO);
                }
            }

            return NewResLangDTO;
        }

        /// <summary>
        /// Export Resources
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <param name="ResourcePageId"></param>
        //[HttpPost]
        public ActionResult ExportResources(Int64 ModuleId, string FileName, string ResType)
        {
            string stringCsvKeyField = "* KEY";
            string stringCsvData = string.Empty;

            ResourceDAL objDAL = null;
            ResourceModuleDetailsDTO dtoModuleDetail = null;
            List<KeyValDTO> lstResources = null;
            List<KeyValDTO> lstLanguageKeyVal = null;
            ASCIIEncoding objASCIEncoding = null;
            Dictionary<string, string> dictResource = null;
            byte[] data = null;

            try
            {
                if (ModuleId > 0 && !string.IsNullOrEmpty(FileName))
                {
                    objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                    dtoModuleDetail = objDAL.GetResourceModuleDetail(ModuleId, FileName);

                    lstLanguageKeyVal = GetLangaugesForImportExport(ResType);

                    dictResource = new Dictionary<string, string>();
                    dictResource.Add(stringCsvKeyField, "");

                    foreach (var lng in lstLanguageKeyVal)
                    {
                        if (ResType.ToUpper() == "ETURNSBASE")
                        {
                            if (dtoModuleDetail.ResourceModuleName == "Enterprise")
                                lstResources = BaseResourceHelper.GetBaseEnterpriseResourceData(dtoModuleDetail.FileName, lng.key).OrderBy(x => x.key).ToList();
                            else
                                lstResources = BaseResourceHelper.GetBaseResourceData(dtoModuleDetail.FileName, lng.key).OrderBy(x => x.key).ToList();
                        }
                        else if (ResType.ToUpper() == "ENTERPRISEBASE")
                        {
                            if (dtoModuleDetail.ResourceModuleName == "Enterprise")
                                lstResources = EnterPriseResourceHelper.GetEnterpriseResourceData(dtoModuleDetail.FileName, lng.key).OrderBy(x => x.key).ToList();
                            else
                                lstResources = EnterPriseResourceHelper.GetResourceData(dtoModuleDetail.FileName, lng.key).OrderBy(x => x.key).ToList();
                        }
                        else if (ResType.ToUpper() == "COMPANYBASE")
                            lstResources = ResourceHelper.GetResourceData(dtoModuleDetail.FileName, lng.key).OrderBy(x => x.key).ToList();

                        if (dictResource[stringCsvKeyField].Length > 0)
                            dictResource[stringCsvKeyField] += string.Format(separator + "{0}", lng.value);
                        else
                            dictResource[stringCsvKeyField] = string.Format("* {0}", lng.value);

                        foreach (var item in lstResources)
                        {
                            if (dictResource.ContainsKey(item.key))
                                dictResource[item.key] += string.Format(separator + "{0}", item.value.Replace(",", ".").Trim());
                            else
                                dictResource.Add(item.key, string.Format("{0}", item.value.Replace(",", ".").Trim()));
                        }
                    }

                    int index = 0;
                    foreach (var item in dictResource)
                    {
                        if (index > 0)
                            stringCsvData += "\r\n";
                        stringCsvData += string.Format("{0}" + separator + "{1}", item.Key, item.Value);

                        index += 1;
                    }
                    index = 0;
                    objASCIEncoding = new ASCIIEncoding();
                    data = objASCIEncoding.GetBytes(stringCsvData);
                }
                return File(data, "application/csv", dtoModuleDetail.ResourceModuleName + "_" + dtoModuleDetail.FileName + "_" + DateTimeUtility.DateTimeNow.ToString("yyyyMMddHHmmss") + ".csv");
                //return Json(new { Message = "ok", Status = true, ModuleDetailData = dtoModuleDetail }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objDAL = null;
                dtoModuleDetail = null;

                lstResources = null;
                objASCIEncoding = null;
                dictResource = null;
                data = null;

            }

        }

        /// <summary>
        /// Export Resources
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <param name="ResourcePageId"></param>
        //[HttpPost]
        public ActionResult ExportMobileResources(Int64 ModuleId, Int64 PageID, string ResType)
        {
            IEnumerable<MobileResourcesDTO> lstMobileResData = null;
            List<ResourceLanguageDTO> lstLanguageKeyVal = null;
            MobileResourcesDAL mobileResDAL = null;
            //eTurnsMasterMobileResourceDAL mobileResDAL = null;
            MobileResourcesDAL objMobResDAL = null;
            ResourceModuleDetailsDTO dtoModuleDetail = null;
            ASCIIEncoding objASCIEncoding = null;
            Dictionary<string, string> dictResource = null;
            byte[] data = null;
            try
            {
                string stringCsvKeyField = "* KEY";
                string stringCsvData = string.Empty;

                if (ModuleId > 0 && PageID > 0)
                {
                    //mobileResDAL = new eTurnsMasterMobileResourceDAL(SessionHelper.EnterPriseDBName);
                    mobileResDAL = new MobileResourcesDAL(CommonDAL.GeteTurnsDatabase());
                    //dtoModuleDetail = mobileResDAL.GetResourceModuleDetailData_Mobile(ModuleId).FirstOrDefault(x => x.ID == PageID);
                    dtoModuleDetail = mobileResDAL.GetResourceModuleDetailData_Mobile(ModuleId, PageID);

                    lstLanguageKeyVal = GetLangaugesForMobileImportExport(ResType);
                    dictResource = new Dictionary<string, string>();
                    dictResource.Add(stringCsvKeyField, "");

                    foreach (var lng in lstLanguageKeyVal)
                    {
                        if (ResType.ToUpper() == "ETURNSBASE")
                        {
                            //lstMobileResData = mobileResDAL.GetAllMobileResourceRecords().Where(x => x.ResourcePageID == PageID && x.LanguageID == lng.ID).ToList();
                            lstMobileResData = mobileResDAL.GetAllMobileResourceRecords(PageID, lng.ID).ToList();
                        }
                        else if (ResType.ToUpper() == "ENTERPRISEBASE")
                        {
                            objMobResDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                            //lstMobileResData = objMobResDAL.GetAllRecords(0).Where(x => x.ResourcePageID == PageID && x.LanguageID == lng.ID).ToList();
                            lstMobileResData = objMobResDAL.GetAllMobileResources(0, PageID, lng.ID, string.Empty).ToList();
                        }
                        else if (ResType.ToUpper() == "COMPANYBASE")
                        {
                            objMobResDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                            //lstMobileResData = objMobResDAL.GetAllRecords(SessionHelper.CompanyID).Where(x => x.ResourcePageID == PageID && x.LanguageID == lng.ID).ToList();
                            lstMobileResData = objMobResDAL.GetAllMobileResources(SessionHelper.CompanyID, PageID, lng.ID, string.Empty).ToList();
                        }

                        if (dictResource[stringCsvKeyField].Length > 0)
                            dictResource[stringCsvKeyField] += string.Format(separator + "{0}", lng.Language);
                        else
                            dictResource[stringCsvKeyField] = string.Format("* {0}", lng.Language);

                        foreach (var item in lstMobileResData)
                        {
                            if (dictResource.ContainsKey(item.ResourceKey))
                                dictResource[item.ResourceKey] += string.Format(separator + "{0}", item.ResourceValue.Replace(",", ".").Trim());
                            else
                                dictResource.Add(item.ResourceKey, string.Format("{0}", item.ResourceValue.Replace(",", ".").Trim()));
                        }
                    }

                    int index = 0;
                    foreach (var item in dictResource)
                    {
                        if (index > 0)
                            stringCsvData += "\r\n";
                        stringCsvData += string.Format("{0}" + separator + "{1}", item.Key, item.Value);

                        index += 1;
                    }
                    index = 0;
                    objASCIEncoding = new ASCIIEncoding();
                    data = objASCIEncoding.GetBytes(stringCsvData);
                }
                return File(data, "application/csv", dtoModuleDetail.ResourceModuleName + "_" + dtoModuleDetail.PageName + "_MobileRes_" + DateTimeUtility.DateTimeNow.ToString("yyyyMMddHHmmss") + ".csv");
                //return Json(new { Message = "ok", Status = true, ModuleDetailData = dtoModuleDetail }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstMobileResData = null;
                lstLanguageKeyVal = null;
                mobileResDAL = null;
                dtoModuleDetail = null;
                objASCIEncoding = null;
                dictResource = null;
                data = null;
            }

        }

        private string GetResourceFileName(string ResourceFileName, string DBResourceFileName, string ModuleName)
        {
            string fileName = ResourceFileName.Replace(ModuleName + "_", "");
            string ExtraChars = fileName.Replace(DBResourceFileName, "");
            string returFileName = fileName.Replace(ExtraChars, "");


            return returFileName;
        }
        /// <summary>
        /// Import Resources
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="fuResource"></param>
        /// <returns></returns>
        public ActionResult ImportResources(FormCollection collection, HttpPostedFileBase fuResource)
        {

            Dictionary<string, List<KeyValDTO>> dictKeyVal = null;
            DataTable dtCSV = null;
            List<KeyValDTO> lstLanguageKeyVal = null;
            string PostedFileName = string.Empty;
            List<KeyValDTO> lstNewUpdatedResources = null;
            string actualResourceFileName = string.Empty;
            List<KeyValDTO> lstEngResources = null;
            try
            {
                string Selectedlanguage = collection["hdnSelectedLanguage"];
                string ResourceFileName = collection["hdnResourceFileName"];
                string ModuleId = collection["hdnModuleID"];
                string ModuleName = collection["hdnModuleName"];
                string returnURL = collection["hdnReturnURL"];
                string ResType = collection["hdnResourceType"];

                if (ResType.ToUpper() == "ETURNSBASE")
                {
                    if (ModuleName == "Enterprise")
                        lstEngResources = BaseResourceHelper.GetBaseEnterpriseResourceData(ResourceFileName, "en-US").OrderBy(x => x.key).ToList();
                    else
                        lstEngResources = BaseResourceHelper.GetBaseResourceData(ResourceFileName, "en-US").OrderBy(x => x.key).ToList();
                }
                else if (ResType.ToUpper() == "ENTERPRISEBASE")
                {
                    if (ModuleName == "Enterprise")
                        lstEngResources = EnterPriseResourceHelper.GetEnterpriseResourceData(ResourceFileName, "en-US").OrderBy(x => x.key).ToList();
                    else
                        lstEngResources = EnterPriseResourceHelper.GetResourceData(ResourceFileName, "en-US").OrderBy(x => x.key).ToList();
                }
                else if (ResType.ToUpper() == "COMPANYBASE")
                {
                    lstEngResources = ResourceHelper.GetResourceData(ResourceFileName, "en-US").OrderBy(x => x.key).ToList();
                }

                if (fuResource != null)
                {
                    PostedFileName = Path.GetFileName(fuResource.FileName);

                    string[] strFileNamePartbyExt = PostedFileName.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strFileNamePartbyExt != null && strFileNamePartbyExt.Length > 1 && strFileNamePartbyExt[strFileNamePartbyExt.Length - 1].ToUpper() == "CSV")
                    {

                        string[] strFNamePart = strFileNamePartbyExt[0].Split(new char[1] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                        string fileNameInImport = GetResourceFileName(strFileNamePartbyExt[0], ResourceFileName, ModuleName);

                        //if (!strFileNamePartbyExt[0].Contains("_MobileRes_") && strFNamePart != null && strFNamePart.Length > 1)
                        if (!strFileNamePartbyExt[0].Contains("_MobileRes_") && strFNamePart != null && strFNamePart.Length > 1 && !string.IsNullOrEmpty(fileNameInImport))
                        {
                            //if (strFNamePart[0] == ModuleName && strFNamePart[1] == ResourceFileName)
                            if (strFNamePart[0] == ModuleName && fileNameInImport == ResourceFileName)
                            {
                                actualResourceFileName = ResourceFileName;

                                lstLanguageKeyVal = GetLangaugesForImportExport(ResType);
                                string RequFields = "KEY," + lstLanguageKeyVal[0].value;

                                string Fields = "KEY," + string.Join(",", lstLanguageKeyVal.Select(x => x.value));
                                dtCSV = GetDataTableFromCSVData(Fields, RequFields, fuResource);

                                if (dtCSV != null && dtCSV.Rows.Count > 0)
                                {
                                    dictKeyVal = new Dictionary<string, List<KeyValDTO>>();
                                    for (int j = 1; j < dtCSV.Columns.Count; j++)
                                    {
                                        string dickey = lstLanguageKeyVal.FirstOrDefault(x => x.value.ToLower() == dtCSV.Columns[j].ColumnName.ToLower()).key;
                                        lstNewUpdatedResources = new List<KeyValDTO>();
                                        lstNewUpdatedResources.Clear();
                                        for (int i = 0; i < dtCSV.Rows.Count; i++)
                                        {
                                            KeyValDTO objKey = lstEngResources.FirstOrDefault(x => x.key == Convert.ToString(dtCSV.Rows[i][0]));
                                            if (objKey != null)
                                            {
                                                objKey.key = Convert.ToString(dtCSV.Rows[i][0]);
                                                objKey.value = Convert.ToString(dtCSV.Rows[i][j]).Trim();
                                                lstNewUpdatedResources.Add(objKey);
                                            }
                                        }

                                        IEnumerable<KeyValDTO> lst = DeepCopy(lstNewUpdatedResources);
                                        dictKeyVal.Add(dickey, lst.ToList());

                                    }
                                }
                            }
                        }
                    }
                }

                if (dictKeyVal != null && dictKeyVal.Count > 0)
                {

                    foreach (var item in dictKeyVal)
                    {
                        if (ResType == "ETURNSBASE" && !string.IsNullOrEmpty(actualResourceFileName))
                        {
                            BaseResourceHelper resHelper = new BaseResourceHelper();
                            if (ModuleName == "Enterprise")
                                resHelper.SaveBaseEnterpriseResources(actualResourceFileName, item.Key, item.Value);
                            else
                                resHelper.SaveBaseResources(actualResourceFileName, item.Key, item.Value);
                        }
                        else if (ResType == "ENTERPRISEBASE" && !string.IsNullOrEmpty(actualResourceFileName))
                        {
                            EnterPriseResourceHelper resHelper = new EnterPriseResourceHelper();
                            if (ModuleName == "Enterprise")
                                resHelper.SaveEnterpriseResources(actualResourceFileName, item.Key, item.Value);
                            else
                                resHelper.SaveResources(actualResourceFileName, item.Key, item.Value);
                        }
                        else if (ResType == "COMPANYBASE" && !string.IsNullOrEmpty(actualResourceFileName))
                        {
                            ResourceHelper resHelper = new ResourceHelper();
                            resHelper.SaveResources(actualResourceFileName, item.Key, item.Value);
                        }
                    }

                }

                string redirectAction = "";

                if (ResType == "ETURNSBASE")
                {
                    redirectAction = "EditBaseResource";
                }
                if (ResType == "ENTERPRISEBASE")
                {
                    redirectAction = "EditENTResource";
                }
                else if (ResType == "COMPANYBASE")
                {
                    redirectAction = "EditResource";
                }

                ClearCurrentResourceList();
                return RedirectToAction(redirectAction, "Resources", new { lang = Selectedlanguage, module = ModuleName, page = actualResourceFileName });
                //return View("EditBaseResource");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dictKeyVal = null;
                dtCSV = null;
                lstLanguageKeyVal = null;
            }
        }


        /// <summary>
        /// Import Resources
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="fuResource"></param>
        /// <returns></returns>
        public ActionResult ImportMobileResources(FormCollection collection, HttpPostedFileBase fuResource)
        {

            Dictionary<string, List<MobileResourcesDTO>> dictKeyVal = null;
            DataTable dtCSV = null;
            List<ResourceLanguageDTO> lstLanguageKeyVal = null;
            string PostedFileName = string.Empty;
            List<MobileResourcesDTO> lstNewUpdatedResources = null;
            string actualResourceFileName = string.Empty;
            List<MobileResourcesDTO> lstEngResources = null;
            //eTurnsMasterMobileResourceDAL mobileResDAL = null;
            MobileResourcesDAL mobileResDAL = null;
            MobileResourcesDAL objMobResDAL = null;
            Int64 companyID = 0;
            try
            {
                string Selectedlanguage = collection["hdnSelectedLanguage"];
                string ResourceFileName = collection["hdnResourceFileName"];
                string ResourcePageID = collection["hdnResourcePageID"];
                string ModuleId = collection["hdnModuleID"];
                string ModuleName = collection["hdnModuleName"];
                string returnURL = collection["hdnReturnURL"];
                string ResType = collection["hdnResourceType"];
                //mobileResDAL = new eTurnsMasterMobileResourceDAL(SessionHelper.EnterPriseDBName);
                mobileResDAL = new MobileResourcesDAL(CommonDAL.GeteTurnsDatabase());
                objMobResDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                lstLanguageKeyVal = GetLangaugesForMobileImportExport(ResType);

                if (ResType.ToUpper() == "ETURNSBASE")
                {
                    //lstEngResources = mobileResDAL.GetAllMobileResourceRecords().Where(x => x.ResourcePageID == Int64.Parse(ResourcePageID) && x.LanguageID == lstLanguageKeyVal[0].ID).OrderBy(x => x.ResourceKey).ToList();
                    lstEngResources = mobileResDAL.GetAllMobileResourceRecords(Int64.Parse(ResourcePageID), lstLanguageKeyVal[0].ID).ToList();
                }
                else if (ResType.ToUpper() == "ENTERPRISEBASE")
                {
                    //lstEngResources = objMobResDAL.GetAllRecords(companyID).Where(x => x.ResourcePageID == Int64.Parse(ResourcePageID) && x.LanguageID == lstLanguageKeyVal[0].ID).OrderBy(x => x.ResourceKey).ToList();
                    lstEngResources = objMobResDAL.GetAllMobileResources(companyID, Int64.Parse(ResourcePageID), lstLanguageKeyVal[0].ID, string.Empty).OrderBy(x => x.ResourceKey).ToList();
                }
                else if (ResType.ToUpper() == "COMPANYBASE")
                {
                    companyID = SessionHelper.CompanyID;
                    //lstEngResources = objMobResDAL.GetAllRecords(companyID).Where(x => x.ResourcePageID == Int64.Parse(ResourcePageID) && x.LanguageID == lstLanguageKeyVal[0].ID).OrderBy(x => x.ResourceKey).ToList();
                    lstEngResources = objMobResDAL.GetAllMobileResources(companyID, Int64.Parse(ResourcePageID), lstLanguageKeyVal[0].ID, string.Empty).OrderBy(x => x.ResourceKey).ToList();
                }

                if (fuResource != null)
                {
                    PostedFileName = fuResource.FileName;

                    string[] strFileNamePartbyExt = PostedFileName.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strFileNamePartbyExt != null && strFileNamePartbyExt.Length > 1 && strFileNamePartbyExt[strFileNamePartbyExt.Length - 1].ToUpper() == "CSV")
                    {
                        string[] strFNamePart = strFileNamePartbyExt[0].Split(new char[1] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                        if (strFNamePart != null && strFNamePart.Length > 2)
                        {
                            if (strFNamePart[0] == ModuleName && strFNamePart[1] == ResourceFileName && strFNamePart[2] == "MobileRes")
                            {
                                actualResourceFileName = ResourceFileName;
                                string RequFields = "KEY," + lstLanguageKeyVal[0].Language;
                                string Fields = "KEY," + string.Join(",", lstLanguageKeyVal.Select(x => x.Language));
                                dtCSV = GetDataTableFromCSVData(Fields, RequFields, fuResource);

                                if (dtCSV != null && dtCSV.Rows.Count > 0)
                                {
                                    dictKeyVal = new Dictionary<string, List<MobileResourcesDTO>>();
                                    for (int j = 1; j < dtCSV.Columns.Count; j++)
                                    {
                                        lstNewUpdatedResources = new List<MobileResourcesDTO>();
                                        lstNewUpdatedResources.Clear();
                                        for (int i = 0; i < dtCSV.Rows.Count; i++)
                                        {
                                            MobileResourcesDTO objKey = lstEngResources.FirstOrDefault(x => x.ResourceKey == Convert.ToString(dtCSV.Rows[i][0]));
                                            if (objKey != null)
                                            {
                                                objKey.LanguageID = lstLanguageKeyVal.FirstOrDefault(x => x.Language.ToLower() == dtCSV.Columns[j].ColumnName.ToLower()).ID;
                                                objKey.CompanyID = companyID;
                                                objKey.ResourcePageID = Int64.Parse(ResourcePageID);
                                                objKey.ResourceKey = Convert.ToString(dtCSV.Rows[i][0]);
                                                objKey.ResourceValue = Convert.ToString(dtCSV.Rows[i][j]);
                                                lstNewUpdatedResources.Add(objKey);
                                            }
                                        }
                                        string dickey = lstLanguageKeyVal.FirstOrDefault(x => x.Language.ToLower() == dtCSV.Columns[j].ColumnName.ToLower()).Culture;
                                        IEnumerable<MobileResourcesDTO> lst = DeepCopy(lstNewUpdatedResources);
                                        dictKeyVal.Add(dickey, lst.ToList());
                                    }
                                }
                            }
                        }
                    }
                }

                if (dictKeyVal != null && dictKeyVal.Count > 0)
                {
                    foreach (var item in dictKeyVal)
                    {
                        if (ResType == "ETURNSBASE" && !string.IsNullOrEmpty(actualResourceFileName))
                        {
                            foreach (var mobRes in item.Value)
                            {
                                mobileResDAL.UpdateResourceFromImport(mobRes);
                            }
                        }
                        else if ((ResType == "ENTERPRISEBASE" || ResType == "COMPANYBASE") && !string.IsNullOrEmpty(actualResourceFileName))
                        {
                            foreach (var mobRes in item.Value)
                            {
                                objMobResDAL.UpdateResourceFromImport(mobRes);
                            }
                        }
                    }
                }

                string redirectAction = "";

                if (ResType == "ETURNSBASE")
                    redirectAction = "EditBaseMobileResource";
                if (ResType == "ENTERPRISEBASE")
                    redirectAction = "EditENTMobileResource";
                else if (ResType == "COMPANYBASE")
                    redirectAction = "EditMobileResource";

                ClearCurrentResourceList();

                return RedirectToAction(redirectAction, "Resources", new { lang = Selectedlanguage, module = ModuleName, PageID = ResourcePageID });
                //return View("EditBaseResource");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dictKeyVal = null;
                dtCSV = null;
                lstLanguageKeyVal = null;
            }
        }

        public IEnumerable<KeyValDTO> DeepCopy(List<KeyValDTO> obj)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);

            ms.Position = 0;
            return (IEnumerable<KeyValDTO>)bf.Deserialize(ms);
        }

        public IEnumerable<MobileResourcesDTO> DeepCopy(List<MobileResourcesDTO> obj)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);

            ms.Position = 0;
            return (IEnumerable<MobileResourcesDTO>)bf.Deserialize(ms);
        }

        public IEnumerable<BaseResourcesDTO> DeepCopy(List<BaseResourcesDTO> obj)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);

            ms.Position = 0;
            return (IEnumerable<BaseResourcesDTO>)bf.Deserialize(ms);
        }

        /// <summary>
        /// GetCSVData
        /// </summary>
        /// <param name="Fields"></param>
        /// <param name="RequiredField"></param>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        private DataTable GetDataTableFromCSVData(string Fields, string RequiredField, HttpPostedFileBase uploadFile)
        {
            DataTable dtCSVTemp = null;
            DataRow dr;
            Stream objstr = null;
            StreamReader sr = null;
            DataColumn dc = null;
            try
            {
                string[] DBReqField = RequiredField.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] DBField = Fields.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (uploadFile.ContentLength > 0)
                {
                    string[] value = { "" };
                    dtCSVTemp = new DataTable();
                    objstr = uploadFile.InputStream;
                    //TODO: Added encoding for the Issue of Spanish(Maxico) langauge is not displaying proper in import.
                    //sr = new StreamReader(objstr);
                    sr = new StreamReader(objstr, Encoding.GetEncoding("iso-8859-1"));
                    objstr.Position = 0;
                    sr.DiscardBufferedData();
                    string separator = ConfigurationManager.AppSettings["CSVseparator"].ToString();
                    string headerLine = sr.ReadLine();
                    foreach (string item in DBReqField)
                    {
                        if (!headerLine.ToLower().Contains(item.ToLower()))
                            return null;
                    }

                    string[] strHeaderArray = headerLine.Replace("* ", "").ToLower().Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string item in strHeaderArray)
                    {
                        dc = new DataColumn();
                        dc.ColumnName = item.ToString();
                        dtCSVTemp.Columns.Add(dc);
                    }

                    using (TextFieldParser parser = new TextFieldParser(sr))
                    {
                        parser.Delimiters = new string[] { separator };
                        while (true)
                        {
                            value = parser.ReadFields();
                            if (value == null)
                                break;
                            else
                            {
                                if (value.Length > strHeaderArray.Length)
                                {
                                    List<string> val = new List<string>();
                                    for (int i = 0; i < strHeaderArray.Length; i++)
                                    {
                                        val.Add(value[i]);
                                    }
                                    value = val.ToArray<string>();
                                }

                                dr = dtCSVTemp.NewRow();
                                dr.ItemArray = value;
                                dtCSVTemp.Rows.Add(dr);
                            }
                        }
                    }

                    if (dtCSVTemp != null && dtCSVTemp.Rows.Count > 0)
                    {
                        foreach (DataColumn col in dtCSVTemp.Columns)
                        {
                            if (!Fields.ToLower().Contains(col.ToString().ToLower()))
                                dtCSVTemp.Columns.Remove(col.ToString().ToLower());
                        }
                    }
                }
                return dtCSVTemp;
            }
            finally
            {
                if (objstr != null)
                {
                    objstr.Flush();
                    objstr.Close();
                    objstr.Dispose();
                }

                objstr = null;

                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }

                sr = null;
                dc = null;
                dr = null;
                dtCSVTemp = null;

            }


        }

        #endregion

        #region Import New Resource File



        /// <summary>
        /// Import Resources
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="fuResource"></param>
        /// <returns></returns>
        public ActionResult ImportNewResourcesFile(FormCollection collection, HttpPostedFileBase fuResource)
        {
            string PostedFileName = string.Empty;
            string actualResourceFileName = string.Empty;
            try
            {
                string Selectedlanguage = collection["hdnSelectedLanguage"];
                string ResourceFileName = collection["hdnResourceFileName"];
                string ModuleId = collection["hdnModuleID"];
                string ModuleName = collection["hdnModuleName"];
                string returnURL = collection["hdnReturnURL"];
                string ResType = collection["hdnResourceType"];
                string btnClicked = collection["hdnResourceType"];
                if (fuResource != null)
                {
                    if (!string.IsNullOrEmpty(collection["btnSubmitNewCompanyResourceFile"]))
                    {
                        List<string> lstSavedResources = SaveBaseCompanyResource(fuResource);
                    }
                }

                ClearCurrentResourceList();
                return RedirectToAction("EditBaseResource", "Resources", new { lang = Selectedlanguage, module = ModuleName, page = actualResourceFileName });
            }
            catch
            {
                throw;
            }
            finally
            {

            }
        }
        private void GenerateTargetResourceFile(string TargetResourceFile, string culture, Dictionary<string, string> diSourceENRes)
        {
            if (System.IO.File.Exists(TargetResourceFile))
            {
                //Update key's value
                ResXResourceReader rsxr = new ResXResourceReader(TargetResourceFile);
                Dictionary<string, string> diTarget = new Dictionary<string, string>();
                foreach (DictionaryEntry dd in rsxr)
                {
                    diTarget.Add(dd.Key.ToString(), dd.Value.ToString());

                }

                string value = string.Empty;
                string valueWOSpace = string.Empty;
                string oldValue = string.Empty;
                string oldValueWOSpace = string.Empty;
                foreach (string keySource in diSourceENRes.Keys)
                {

                    if (!diTarget.ContainsKey(keySource))
                    {
                        diSourceENRes.TryGetValue(keySource, out value);
                        value = TranslatorUtitlity.GetTranslatedText(value, culture);
                        diTarget.Add(keySource, value);
                    }
                    else
                    {
                        diSourceENRes.TryGetValue(keySource, out value);
                        oldValue = diTarget[keySource];
                        oldValueWOSpace = (oldValue ?? string.Empty).Replace(" ", "");
                        valueWOSpace = (value ?? string.Empty).Replace(" ", "");
                        value = TranslatorUtitlity.GetTranslatedText(value, culture);
                        diTarget[keySource] = value;
                    }
                }

                ResXResourceWriter resourceWriter = new ResXResourceWriter(TargetResourceFile);
                foreach (string ke in diTarget.Keys)
                {
                    resourceWriter.AddResource(ke, diTarget[ke]);
                }
                resourceWriter.Generate();
                resourceWriter.Close();
            }
            else
            {
                //create new file value
                ResXResourceWriter resourceWriter = new ResXResourceWriter(TargetResourceFile);
                foreach (string ke in diSourceENRes.Keys)
                {
                    string value = string.Empty;
                    diSourceENRes.TryGetValue(ke, out value);
                    // convert value into culture and translatedText
                    value = TranslatorUtitlity.GetTranslatedText(value, culture);
                    resourceWriter.AddResource(ke, value);
                }
                resourceWriter.Generate();
                resourceWriter.Close();
            }
        }


        #region Copy Company Resource


        private List<string> SaveBaseCompanyResource(HttpPostedFileBase fuResource)
        {
            string BaseResourcePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];

            string BaseCompanyResourcePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"] + "\\MasterResources\\CompanyResources";
            List<string> CreatedBaseResourceFiles = new List<string>();
            Dictionary<string, string> lstKeyValue = new Dictionary<string, string>();
            string PostedFileName = Path.GetFileName(fuResource.FileName);

            fuResource.SaveAs(Path.Combine(BaseCompanyResourcePath, fuResource.FileName));
            CreatedBaseResourceFiles.Add(Path.Combine(BaseCompanyResourcePath, fuResource.FileName));

            SaveCompanyResourcesFromSourceToTarget(BaseCompanyResourcePath, BaseCompanyResourcePath, fuResource.FileName, CreatedBaseResourceFiles);
            MoveTranslatedCompanyResourcesToEnterprise(CreatedBaseResourceFiles, BaseResourcePath);

            return CreatedBaseResourceFiles;
        }

        private void SaveCompanyResourcesFromSourceToTarget(string sourceDirectory, string targetDirectory, string newfileName, List<string> ListOfCreatedFiles)
        {
            List<string> BasecultureList = new List<string>() { "nl-NL", "fr-FR", "de-DE", "ru-RU", "es-MX", "zh-CN" };
            List<string> cultureList = new List<string>() { "nl-NL", "fr-FR", "de-DE", "ru-RU", "es-MX", "zh-CN" };
            List<string> CopyCultureList = new List<string>() { "en-AU", "en-GB", "en-IN", "nl-BE" };
            List<SelectListItem> lstCulter = GetBaseLanguage();
            if (lstCulter != null && lstCulter.Count > 0)
                cultureList = lstCulter.Select(x => x.Value).ToList();

            List<string> notExistLang = BasecultureList.Where(x => !cultureList.Contains(x)).ToList();
            if (notExistLang != null && notExistLang.Count > 0)
            {
                foreach (var item in notExistLang)
                    cultureList.Add(item);
            }

            if (Directory.Exists(sourceDirectory))
            {
                System.IO.DirectoryInfo diEnt = new DirectoryInfo(sourceDirectory);

                FileInfo file = diEnt.GetFiles(newfileName).FirstOrDefault();
                string fileName = Path.GetFileNameWithoutExtension(file.FullName);
                foreach (string item in cultureList)
                {
                    string destiFileName = fileName + "." + item + ".resx";

                    if (!CopyCultureList.Contains(item))
                    {
                        ResXResourceReader rsxr = new ResXResourceReader(file.FullName);
                        Dictionary<string, string> diSourceENRes = new Dictionary<string, string>();
                        foreach (DictionaryEntry dd in rsxr)
                        {
                            diSourceENRes.Add(dd.Key.ToString(), dd.Value.ToString());
                        }
                        GenerateTargetResourceFile(System.IO.Path.Combine(targetDirectory, destiFileName), item, diSourceENRes);
                        ListOfCreatedFiles.Add(System.IO.Path.Combine(targetDirectory, destiFileName));
                    }
                }


                foreach (string item in CopyCultureList)
                {
                    string targetCulture = item.Split('-')[0];
                    string baseFileName = file.FullName;
                    if (targetCulture.ToLower() == "nl")
                        baseFileName = targetDirectory + "\\" + fileName + ".nl-NL" + ".resx";

                    string destiFileName = targetDirectory + "\\" + fileName + "." + item + ".resx";
                    System.IO.File.Copy(baseFileName, destiFileName, true);
                    ListOfCreatedFiles.Add(destiFileName);
                }

            }
        }

        private static void MoveTranslatedCompanyResourcesToEnterprise(List<string> sourceFiles, string ResourcesPath)
        {
            /* loop for enterprise */
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            List<EnterpriseDTO> enterpriseList = objEnterpriseMasterDAL.GetAllEnterprisesPlain();

            List<eTurnsMaster.DAL.MstCompanyMaster> companyList = null;

            string str_MasterResources = "MasterResources";
            string str_CompanyResources = "CompanyResources";

            ResourcesPath = ResourcesPath + "\\";

            string EnterpriseMasterResourcesPath = ResourcesPath + str_MasterResources;

            string CompanyIDCompanyResources = string.Empty;
            string CompanyIDRoomResources = string.Empty;
            string EnterpriseIDDirectory = string.Empty;
            string CompanyIDDirectory = string.Empty;
            string RoomIDDirectory = string.Empty;

            /*get all enterprices*/
            foreach (var item in enterpriseList)
            {
                System.IO.DirectoryInfo diSource = new DirectoryInfo(EnterpriseMasterResourcesPath);
                foreach (DirectoryInfo dir in diSource.GetDirectories())
                {
                    if (!Directory.Exists(Path.Combine(ResourcesPath, item.ID.ToString())))
                        Directory.CreateDirectory(Path.Combine(ResourcesPath, item.ID.ToString()));

                    EnterpriseIDDirectory = Path.Combine(ResourcesPath, item.ID.ToString());
                    if (!Directory.Exists(Path.Combine(EnterpriseIDDirectory, dir.Name)))
                        Directory.CreateDirectory(Path.Combine(EnterpriseIDDirectory, dir.Name));
                }

                foreach (string newPath in sourceFiles)
                {
                    string EnterpriseResourcesPath = EnterpriseIDDirectory + "\\" + str_CompanyResources;
                    string fileName = newPath.Remove(0, newPath.LastIndexOf("\\"));
                    System.IO.File.Copy(newPath, EnterpriseResourcesPath + fileName, true);


                }

                companyList = objEnterpriseMasterDAL.GetCompaniesByEnterPriseID(item.ID);

                foreach (var company in companyList)
                {
                    if (!Directory.Exists(Path.Combine(EnterpriseIDDirectory, company.ID.ToString())))
                        Directory.CreateDirectory(Path.Combine(EnterpriseIDDirectory, company.ID.ToString()));

                    CompanyIDDirectory = Path.Combine(EnterpriseIDDirectory, company.ID.ToString());

                    foreach (string newPath in sourceFiles)
                    {
                        string fileName = newPath.Remove(0, newPath.LastIndexOf("\\"));
                        System.IO.File.Copy(newPath, CompanyIDDirectory + fileName, true);


                    }
                }

            }
            /* loop for enterprise */
        }

        #endregion

        #endregion



        public ActionResult AddResourceOpen(string lang = "", string module = "", string page = "")
        {

            ViewBag.DDLanguage = lang;
            ViewBag.DDLModuleMaster = module;
            ViewBag.DDLResourceFile = page;
            return View("AddResourceOpen");
        }

        public string AddNewResources(List<BaseResourceDTO> newResources)
        {
            if (newResources != null && newResources.Count > 0)
            {
                var resourceDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
                var resModules = resourceDAL.GetCachedResourceModuleMasterData(0);
                string enterpriseModuleIdStr = string.Empty;

                if (resModules != null && resModules.Any() && resModules.Count() > 0)
                {
                    var enterpriseModuleId = resModules.Where(e => e.ModuleName.Trim().ToLower() == "enterprise").FirstOrDefault().ID;
                    if (enterpriseModuleId > 0)
                    {
                        enterpriseModuleIdStr = Convert.ToString(enterpriseModuleId);
                    }
                }
                foreach (var itemresource in newResources)
                {
                    string lang = itemresource.Language; string module = itemresource.Module;
                    string page = itemresource.Page; string key = itemresource.Key; string value = itemresource.Value;
                    try
                    {
                        List<SelectListItem> lstLang = GeteTurnsLanguage();
                        List<SelectListItem> lstModMast = GetResourceModule();

                        if (SessionHelper.UserType != 2 && lstModMast != null && lstModMast.Count > 0)
                            lstModMast = lstModMast.Where(t => t.Text != "Enterprise").ToList();

                        List<SelectListItem> lstResPage = GetResourceFiles(int.Parse(lstModMast[0].Value));
                        List<KeyValCheckDTO> CurrentBaseResourceListDTO = null;
                        CurrentBaseResourceList = new List<KeyValDTO>();
                        CurrentBaseResourceListDTO = new List<KeyValCheckDTO>();
                        List<KeyValCheckDTO> CurrentBaseResourceListForAll = new List<KeyValCheckDTO>();
                        CurrentBaseResourceList.Add(new KeyValDTO { key = key, value = value });

                        CurrentBaseResourceListDTO.Add(new KeyValCheckDTO { key = key, value = value });

                        CurrentBaseResourceListForAll.Add(new KeyValCheckDTO { key = key, value = value });
                        if (!string.IsNullOrEmpty(lang))
                        {
                            foreach (var item in lstLang)
                            {

                                string message = "";
                                string status = "";

                                BaseResourceHelper resHelper = new BaseResourceHelper();
                                try
                                {

                                    string translatedText = string.Empty;
                                    ResourceLanguageKey objResourceLanguageKey = new ResourceLanguageKey();
                                    if (lstResourceLanguageKey != null)
                                    {
                                        objResourceLanguageKey = lstResourceLanguageKey.Where(c => c.Language == item.Value && c.Key == value).FirstOrDefault();
                                        if (objResourceLanguageKey == null)
                                        {
                                            eTurns.DTO.Resources.Translator obj = new eTurns.DTO.Resources.Translator();
                                            translatedText = obj.Translate(value, lang, item.Value);
                                            objResourceLanguageKey = new ResourceLanguageKey();
                                            objResourceLanguageKey.Key = value;
                                            objResourceLanguageKey.ConvertValue = translatedText;
                                            objResourceLanguageKey.Language = item.Value;
                                            lstResourceLanguageKey.Add(objResourceLanguageKey);
                                        }
                                        else
                                        {
                                            translatedText = objResourceLanguageKey.ConvertValue;
                                        }
                                    }
                                    else
                                    {
                                        lstResourceLanguageKey = new List<ResourceLanguageKey>();
                                        eTurns.DTO.Resources.Translator obj = new eTurns.DTO.Resources.Translator();
                                        translatedText = obj.Translate(value, lang, item.Value);
                                        objResourceLanguageKey = new ResourceLanguageKey();
                                        objResourceLanguageKey.Key = value;
                                        objResourceLanguageKey.ConvertValue = translatedText;
                                        objResourceLanguageKey.Language = item.Value;
                                        lstResourceLanguageKey.Add(objResourceLanguageKey);
                                    }
                                    if (CurrentBaseResourceList != null && (!string.IsNullOrWhiteSpace(translatedText)) && CurrentBaseResourceList.Count > 0)
                                    {
                                        CurrentBaseResourceList[0].value = translatedText;
                                        CurrentBaseResourceListDTO[0].value = translatedText;
                                    }
                                    if (module == "Enterprise" || (!string.IsNullOrEmpty(enterpriseModuleIdStr) && !string.IsNullOrWhiteSpace(enterpriseModuleIdStr) && module == enterpriseModuleIdStr))
                                    {
                                        resHelper.SaveBaseEnterpriseResources(page, item.Value, CurrentBaseResourceList, true);
                                    }
                                    else
                                    {
                                        resHelper.SaveBaseResources(page, item.Value, CurrentBaseResourceList, true);
                                    }

                                    message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                                    status = ResMessage.SaveMessage;//"fail";
                                }
                                catch
                                {

                                }
                            }
                            UpdateAllEnterpriseAndCompanyResource(CurrentBaseResourceListForAll, page, string.Empty);
                        }
                    }
                    catch
                    {
                        //return ex.Message.ToString(); ;
                    }
                }

                return "success";
            }
            return "fail";
        }

        public ActionResult AddBaseResourceOpen(string lang = "", string module = "", string page = "")
        {
            ViewBag.DDLanguage = lang;
            ViewBag.DDLModuleMaster = module;
            ViewBag.DDLResourceFile = page;
            return View("AddBaseResourceOpen");
        }

        public string AddNewBaseResources(List<BaseResourceDTO> newResources)
        {
            if (newResources != null && newResources.Count > 0)
            {
                eTurnsBaseResourceDAL objBaseResourceDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());

                //ResourceDAL objResourceDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
                //IEnumerable<ResourceLanguageDTO> lstLangDTO = objResourceDAL.GetETurnsResourceLanguage();

                IEnumerable<ResourceLanguageDTO> lstLangDTO = objBaseResourceDAL.GetResourceLanguage();
                long UserID = SessionHelper.UserID;
                foreach (var itemresource in newResources)
                {
                    Int64 ResourcePageID = Convert.ToInt64(itemresource.Page);
                    string key = itemresource.Key;
                    string value = itemresource.Value;
                    Int64 LanguageID = Convert.ToInt64(itemresource.Language);
                    string module = itemresource.Module;

                    ResourceLanguageDTO objResourceSaveLanguageDTO = lstLangDTO.Where(x => x.ID == LanguageID).FirstOrDefault();
                    if (LanguageID > 0)
                    {

                        //foreach (ResourceLanguageDTO objResLangDTO in lstLangDTO)
                        Parallel.ForEach(lstLangDTO, objResLangDTO =>
                       {
                           BaseResourcesDTO objExistDTO = objBaseResourceDAL.GetBaseResourceByResouceKeyPageLanguageID(key, ResourcePageID, objResLangDTO.ID);
                           string translatedText = string.Empty;


                           if (objResourceSaveLanguageDTO.ID == objResLangDTO.ID)
                           {
                               translatedText = value;
                           }
                           else
                           {
                               eTurns.DTO.Resources.Translator obj = new eTurns.DTO.Resources.Translator();
                               translatedText = obj.Translate(value, objResourceSaveLanguageDTO.Culture, objResLangDTO.Culture);
                           }

                           if (objExistDTO == null)
                           {
                               BaseResourcesDTO objDTO = new BaseResourcesDTO();
                               objDTO.ResourcePageID = ResourcePageID;
                               objDTO.ResourceKey = key;
                               objDTO.ResourceValue = translatedText;
                               objDTO.LanguageID = objResLangDTO.ID;
                               //objDTO.RoomID = 0;
                               //objDTO.CompanyID = 0;
                               objDTO.LastUpdatedBy = UserID;
                               objDTO.EditedFrom = "Web";
                               objBaseResourceDAL.InsertBaseResource(objDTO, true);
                           }
                           else
                           {
                               objBaseResourceDAL.UpdateBaseResourceByID(objExistDTO.ID, key, translatedText, UserID, "Web", false);
                           }

                       });
                    }

                }

                eTurns.DAL.CacheHelper<IEnumerable<BaseResourcesKeyValDTO>>.InvalidateCacheByKeyStartWith("ResDB");
                AngularApp.AppConfigController.InvalidateCacheByKeyStartWith("ResDB");
                return "success";
            }
            return "fail";
        }

        public ActionResult ExportBaseResources(Int64 ModuleId, Int64 ModuleDetailID, string ResType)
        {
            IEnumerable<BaseResourcesDTO> lstResData = null;
            List<ResourceLanguageDTO> lstResLangDTO = null;
            ResourceModuleDetailsDTO dtoModuleDetail = null;
            ASCIIEncoding objASCIEncoding = null;
            Dictionary<string, string> dictResource = null;
            eTurnsBaseResourceDAL BaseResDAL = null;

            byte[] data = null;
            try
            {
                string stringCsvKeyField = "* KEY";
                //string strResCulture = "en-US";
                string stringCsvData = string.Empty;
                BaseResDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());

                if (ModuleId > 0 && ModuleDetailID > 0)
                {

                    dtoModuleDetail = BaseResDAL.GetResourceModuleDetailDataByID(ModuleDetailID);

                    lstResLangDTO = GetResourceLangaugesForImportExport(ResType);

                    dictResource = new Dictionary<string, string>();
                    dictResource.Add(stringCsvKeyField, "");

                    foreach (var lng in lstResLangDTO)
                    {
                        if (ResType.ToUpper() == "ETURNSBASE")
                        {
                            lstResData = BaseResDAL.GetBaseResourceByResoucePageIDLanguageID(ModuleDetailID, lng.ID, string.Empty);
                        }

                        if (dictResource[stringCsvKeyField].Length > 0)
                            dictResource[stringCsvKeyField] += string.Format(separator + "{0}", lng.Language);
                        else
                            dictResource[stringCsvKeyField] = string.Format("* {0}", lng.Language);

                        foreach (var item in lstResData)
                        {
                            if (dictResource.ContainsKey(item.ResourceKey))
                                dictResource[item.ResourceKey] += string.Format(separator + "{0}", item.ResourceValue.Replace(",", ".").Trim());
                            else
                                dictResource.Add(item.ResourceKey, string.Format("{0}", item.ResourceValue.Replace(",", ".").Trim()));
                        }
                    }

                    int index = 0;
                    foreach (var item in dictResource)
                    {
                        if (index > 0)
                            stringCsvData += "\r\n";
                        stringCsvData += string.Format("{0}" + separator + "{1}", item.Key, item.Value);

                        index += 1;
                    }
                    index = 0;
                    objASCIEncoding = new ASCIIEncoding();
                    data = objASCIEncoding.GetBytes(stringCsvData);
                }
                return File(data, "application/csv", dtoModuleDetail.ResourceModuleName + "_" + dtoModuleDetail.DisplayFileName + "_" + DateTimeUtility.DateTimeNow.ToString("yyyyMMddHHmmss") + ".csv");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstResData = null;
                lstResLangDTO = null;
                dtoModuleDetail = null;
                objASCIEncoding = null;
                dictResource = null;
                data = null;
            }

        }

        //private List<ResourceLanguageDTO> GetLangaugesForResourceImportExport(string resType)  
        //{
        //    List<ResourceLanguageDTO> lstResLangDTO = null;
        //    ResourceDAL objResDAL = new ResourceDAL(CommonDAL.GeteTurnsDatabase());
        //    lstResLangDTO = objResDAL.GetETurnsResourceLanguage().ToList();
        //    string strResCulture = "en-US";

        //    if (lstResLangDTO != null && lstResLangDTO.Count() > 0)
        //    {
        //        ResourceLanguageDTO objEnUSDTO = lstResLangDTO.Where(x => x.Culture == strResCulture).FirstOrDefault();
        //        lstResLangDTO = lstResLangDTO.Where(x => x.ID != objEnUSDTO.ID).ToList();
        //        lstResLangDTO.Insert(0, objEnUSDTO);
        //    }

        //    return lstResLangDTO;
        //}

        private List<ResourceLanguageDTO> GetResourceLangaugesForImportExport(string resType)
        {
            List<ResourceLanguageDTO> lstResLangDTO = null;
            eTurnsBaseResourceDAL objResDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
            lstResLangDTO = objResDAL.GetResourceLanguage().ToList();
            string strResCulture = "en-US";

            if (lstResLangDTO != null && lstResLangDTO.Count() > 0)
            {
                ResourceLanguageDTO objEnUSDTO = lstResLangDTO.Where(x => x.Culture == strResCulture).FirstOrDefault();
                lstResLangDTO = lstResLangDTO.Where(x => x.ID != objEnUSDTO.ID).ToList();
                lstResLangDTO.Insert(0, objEnUSDTO);
            }

            return lstResLangDTO;
        }

        public ActionResult ImportBaseResources(FormCollection collection, HttpPostedFileBase fuResource)
        {

            Dictionary<string, List<BaseResourcesDTO>> dictKeyVal = null;
            DataTable dtCSV = null;
            List<ResourceLanguageDTO> lstLanguageKeyVal = null;
            string PostedFileName = string.Empty;
            List<BaseResourcesDTO> lstNewUpdatedResources = null;
            string actualResourceFileName = string.Empty;
            List<BaseResourcesDTO> lstEngResources = null;
            eTurnsBaseResourceDAL objBaseResourceDAL = null;
            Int64 companyID = 0;
            try
            {
                string SelectedlanguageID = collection["hdnSelectedLanguageID"];
                string Selectedlanguage = collection["hdnSelectedLanguage"];
                string ModuleId = collection["hdnModuleID"];
                string ModuleName = collection["hdnModuleName"];
                string ResourcePageID = collection["hdnResourcePageID"];
                string ResourceFileName = collection["hdnResourceFileName"];
                string returnURL = collection["hdnReturnURL"];
                string ResType = collection["hdnResourceType"];

                objBaseResourceDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
                lstLanguageKeyVal = GetResourceLangaugesForImportExport(ResType);

                if (ResType.ToUpper() == "ETURNSBASE")
                {
                    lstEngResources = objBaseResourceDAL.GetBaseResourceByResoucePageIDLanguageID(Int64.Parse(ResourcePageID), lstLanguageKeyVal[0].ID, string.Empty);
                }

                if (fuResource != null)
                {
                    PostedFileName = fuResource.FileName;

                    string[] strFileNamePartbyExt = PostedFileName.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                    if (strFileNamePartbyExt != null && strFileNamePartbyExt.Length > 1 && strFileNamePartbyExt[strFileNamePartbyExt.Length - 1].ToUpper() == "CSV")
                    {
                        string[] strFNamePart = strFileNamePartbyExt[0].Split(new char[1] { '_' }, StringSplitOptions.RemoveEmptyEntries);

                        if (strFNamePart != null && strFNamePart.Length > 2)
                        {
                            if (strFNamePart[0] == ModuleName && strFNamePart[1] == ResourceFileName)
                            {
                                actualResourceFileName = ResourceFileName;
                                string RequFields = "KEY," + lstLanguageKeyVal[0].Language;
                                string Fields = "KEY," + string.Join(",", lstLanguageKeyVal.Select(x => x.Language));
                                dtCSV = GetDataTableFromCSVData(Fields, RequFields, fuResource);

                                if (dtCSV != null && dtCSV.Rows.Count > 0)
                                {
                                    dictKeyVal = new Dictionary<string, List<BaseResourcesDTO>>();
                                    for (int j = 1; j < dtCSV.Columns.Count; j++)
                                    {
                                        lstNewUpdatedResources = new List<BaseResourcesDTO>();
                                        lstNewUpdatedResources.Clear();
                                        for (int i = 0; i < dtCSV.Rows.Count; i++)
                                        {
                                            BaseResourcesDTO objKey = lstEngResources.FirstOrDefault(x => x.ResourceKey == Convert.ToString(dtCSV.Rows[i][0]));
                                            if (objKey != null)
                                            {
                                                objKey.LanguageID = lstLanguageKeyVal.FirstOrDefault(x => x.Language.ToLower() == dtCSV.Columns[j].ColumnName.ToLower()).ID;
                                                //objKey.CompanyID = companyID;
                                                objKey.ResourcePageID = Int64.Parse(ResourcePageID);
                                                objKey.ResourceKey = Convert.ToString(dtCSV.Rows[i][0]);
                                                objKey.ResourceValue = Convert.ToString(dtCSV.Rows[i][j]);
                                                if (!string.IsNullOrWhiteSpace(objKey.ResourceValue))
                                                    lstNewUpdatedResources.Add(objKey);
                                            }
                                        }
                                        string dickey = lstLanguageKeyVal.FirstOrDefault(x => x.Language.ToLower() == dtCSV.Columns[j].ColumnName.ToLower()).Culture;
                                        IEnumerable<BaseResourcesDTO> lst = DeepCopy(lstNewUpdatedResources);
                                        dictKeyVal.Add(dickey, lst.ToList());
                                    }
                                }
                            }
                        }
                    }
                }

                if (dictKeyVal != null && dictKeyVal.Count > 0)
                {
                    foreach (var item in dictKeyVal)
                    {
                        if (ResType == "ETURNSBASE" && !string.IsNullOrEmpty(actualResourceFileName))
                        {
                            foreach (var baseRes in item.Value)
                            {
                                UpdateBaseResourceFromImport(baseRes);
                            }
                        }
                    }
                }

                string redirectAction = "";

                if (ResType == "ETURNSBASE")
                    redirectAction = "EditBaseResourceNew";

                ClearCurrentResourceList();
                eTurns.DAL.CacheHelper<IEnumerable<BaseResourcesKeyValDTO>>.InvalidateCacheByKeyStartWith("ResDB");
                AngularApp.AppConfigController.InvalidateCacheByKeyStartWith("ResDB");
                return RedirectToAction(redirectAction, "Resources", new { lang = Selectedlanguage, module = ModuleName, page = ResourcePageID });
                //return View("EditBaseResource");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dictKeyVal = null;
                dtCSV = null;
                lstLanguageKeyVal = null;
            }
        }

        public ActionResult EditEnterPriseResource(string lang = "", string module = "", string page = "")
        {
            List<string> allowedentIds = (SiteSettingHelper.AllowedEntIdsResources ?? String.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!allowedentIds.Contains(SessionHelper.EnterPriceID.ToString()))
            {
                return RedirectToAction(ActName, CtrlName);
            }
            CurrentENTResourceList = null;
            List<SelectListItem> lstLang = GeteTurnsResourceLanguage();
            List<SelectListItem> lstModMast = GetResourceModuleData();
            if (SessionHelper.UserType > 2 && lstModMast != null && lstModMast.Count > 0)
                lstModMast = lstModMast.Where(t => t.Text != "Enterprise").ToList();

            List<SelectListItem> lstResPage = GetResourceModuleDetail(int.Parse(lstModMast[0].Value));

            if (!string.IsNullOrEmpty(lang))
            {
                foreach (var item in lstLang)
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
                        lstResPage = GetResourceModuleDetail(int.Parse(item.Value));
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

        private List<SelectListItem> GetEnterPriseLanguage()
        {
            ResourceDAL objDAL = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resLangDTO = objDAL.GetETurnsResourceLanguage();
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.ID.ToString();
                    if (ResourceHelper.CurrentCult.ToString().ToLower() == item.Culture.ToLower())
                        obj.Selected = true;
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
                resLangDTO = null;
                lstItem = null;

            }

        }

        public ActionResult GetEnterPriseResourceList(JQueryDataTableParamModelForResource param)
        {
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            string sortDirection = "asc";

            if (!string.IsNullOrEmpty(Request["sSortDir_0"]))
                sortDirection = Request["sSortDir_0"];

            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            string searchText = string.Empty;
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch.Trim().Length > 0)
                searchText = param.sSearch.Trim();
            sortColumnName = Request["SortingField"].ToString();
            if (sortColumnName == string.Empty)
                sortColumnName = "ResourceKey";
            sortColumnName += " " + sortDirection;

            int TotalRecordCount = 0;

            List<EnterpriseResourcesDTO> lstEnterpriseResources = null;
            EnterpriseResourceDAL EntResDAL = new EnterpriseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());

            Int64 resourcePageID = 0;
            Int64 languageID = 0;

            Int64.TryParse(param.resourcefile, out resourcePageID);
            Int64.TryParse(param.resourcelang, out languageID);

            lstEnterpriseResources = EntResDAL.GetPagedEnterpriseResource(SessionHelper.EnterPriceID, resourcePageID, languageID, sortColumnName, searchText);

            TotalRecordCount = lstEnterpriseResources.Count();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = lstEnterpriseResources
            },
                        JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDataInEnterpriseResource(string ID, string changedKey, string changedValue, string AccrossEnt, string IsEdit)
        {
            //System.Xml.Linq.XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            int ResourceSaveMethod = (int)ResourceSave.ButtonSave;
            //if (Settinfile != null && Settinfile.Element("ResourceSave") != null)
            //    ResourceSaveMethod = Convert.ToInt32(Settinfile.Element("ResourceSave").Value);

            if (SiteSettingHelper.ResourceSave != string.Empty)
                ResourceSaveMethod = Convert.ToInt32(SiteSettingHelper.ResourceSave);

            if (ResourceSaveMethod == (int)ResourceSave.OnChange || !string.IsNullOrWhiteSpace(IsEdit))
            {
                changedValue = changedValue.Trim();
                bool IsAcrossAll = false;
                if (!string.IsNullOrEmpty(changedValue) && !string.IsNullOrWhiteSpace(changedValue))
                {
                    EnterpriseResourceDAL EntResDAL = new EnterpriseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
                    if (!string.IsNullOrEmpty(AccrossEnt) && !string.IsNullOrWhiteSpace(AccrossEnt))
                        IsAcrossAll = Convert.ToBoolean(AccrossEnt);
                    EntResDAL.UpdateEnterpriseResourceByID(Int64.Parse(ID), changedKey, changedValue, SessionHelper.UserID, "Web", IsAcrossAll);

                }
                eTurns.DAL.CacheHelper<IEnumerable<EnterpriseResourcesKeyValDTO>>.InvalidateCacheByKeyStartWith("ResDB");
                AngularApp.AppConfigController.InvalidateCacheByKeyStartWith("ResDB");
            }

            return Json(new { Message = "sucess", Status = "ok", Value = changedValue }, JsonRequestBehavior.AllowGet);
        }


        public void UpdateBaseResourceFromImport(BaseResourcesDTO objDTO)
        {
            eTurnsBaseResourceDAL objBaseResourceDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
            BaseResourcesDTO objExistDTO = objBaseResourceDAL.GetBaseResourceByResouceKeyPageLanguageID(objDTO.ResourceKey, objDTO.ResourcePageID, objDTO.LanguageID);
            if (objExistDTO != null)
            {
                objBaseResourceDAL.UpdateBaseResourceByID(objExistDTO.ID, objExistDTO.ResourceKey, objDTO.ResourceValue, SessionHelper.UserID, "Web", false);
            }
            else
            {
                BaseResourcesDTO objResDTO = new BaseResourcesDTO();
                objResDTO.ResourcePageID = objDTO.ResourcePageID;
                objResDTO.ResourceKey = objDTO.ResourceKey;
                objResDTO.ResourceValue = objDTO.ResourceValue;
                objResDTO.LanguageID = objDTO.LanguageID;
                //objResDTO.RoomID = 0;
                //objResDTO.CompanyID = 0;
                objResDTO.LastUpdatedBy = SessionHelper.UserID;
                objResDTO.EditedFrom = "Web";
                objBaseResourceDAL.InsertBaseResource(objDTO, false);
            }
        }

        public JsonResult GetEnterPriseResourceModule(string ModuleID)
        {
            List<SelectListItem> lstItem = null;

            try
            {
                lstItem = GetResourceModuleDetail(int.Parse(ModuleID));  //GetEnterpriseResourceModuleDetail(int.Parse(ModuleID)); //GeteTurnsBaseResourceModuleDetail(int.Parse(ModuleID));

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

        [HttpPost]
        public JsonResult ResetEnterPriseResource(string SelectedLanguage, string ModuleID, string ModuleName, string ResPageID, string ResPageName)
        {
            string culter = string.Empty;
            string fileName = string.Empty;
            string message = "";
            string status = "";

            try
            {
                string masterResourceDirPath = string.Empty;
                string CompanyFullDirPath = string.Empty;
                ResourceDAL objEntResDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                ResourceDAL objResDAL = new ResourceDAL(DbConnectionHelper.GeteTurnsDBName());

                //if (SelectedModule == "Enterprise")
                //{
                //    masterResourceDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\MasterResources\EnterpriseResources";
                //    CompanyFullDirPath = eTurns.DTO.Resources.ResourceHelper.ResourceDirectoryBasePath + @"\" + SessionHelper.EnterPriceID.ToString() + @"\EnterpriseResources";
                //}
                //else
                //{
                //ResourceLanguageDTO SourcelangDTO = null;
                //ResourceLanguageDTO resDTO = objEntResDAL.GetResourceLanguageByID(Convert.ToInt64(SelectedLanguage));
                //if (resDTO != null)
                //    SourcelangDTO = objResDAL.GetResourceLanguageByCulture(resDTO.Culture);

                EnterpriseResourceDAL objEntDAL = new EnterpriseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
                objEntDAL.ResetEnterpriseResource(SessionHelper.EnterPriceID, Convert.ToInt64(SelectedLanguage), Convert.ToInt64(ModuleID), ModuleName, Convert.ToInt64(ResPageID), ResPageName, SessionHelper.UserID, "Web");
                //}

                message = string.Format(ResResourceEditor.UploadSuccess, HttpStatusCode.OK);
                status = ResMessage.SaveMessage;

            }
            catch (Exception)
            {
                return Json(new { Message = ResResourceEditor.UploadNotSuccess, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {

            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SaveETurnsBaseResources(string SelectedLanguage, string SelectedModule, string SelectedResourcePage, string arrItems = "")
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            //BaseResourcesDTO[] LstKeyValCheckDTO = s.Deserialize<BaseResourcesDTO[]>(arrItems);
            List<BaseResourcesDTO> lstBaseResourcesDTO = s.Deserialize<List<BaseResourcesDTO>>(arrItems);
            BaseResourceHelper resHelper = new BaseResourceHelper();
            try
            {
                eTurnsBaseResourceDAL BaseResDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
                foreach (BaseResourcesDTO item in lstBaseResourcesDTO)
                {
                    //bool IsAcrossAll = false;
                    //if (!string.IsNullOrEmpty(item.chkvalue) && !string.IsNullOrWhiteSpace(AccrossEnt))
                    //    IsAcrossAll = Convert.ToBoolean(AccrossEnt);
                    BaseResDAL.UpdateBaseResourceByID(item.ID, item.ResourceKey, item.ResourceValue.Trim(), SessionHelper.UserID, "Web", item.chkvalue);
                }
                eTurns.DAL.CacheHelper<IEnumerable<BaseResourcesKeyValDTO>>.InvalidateCacheByKeyStartWith("ResDB");
                AngularApp.AppConfigController.InvalidateCacheByKeyStartWith("ResDB");
                message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = ResMessage.SaveMessage;//"fail";
                ClearCurrenteTurnsBaseResourceList();
            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = "fail";
            }
            finally
            {
                resHelper = null;
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveEnterpriseResources(string SelectedLanguage, string SelectedModule, string SelectedResourcePage, string arrItems = "")
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            List<EnterpriseResourcesDTO> lstEntResourcesDTO = s.Deserialize<List<EnterpriseResourcesDTO>>(arrItems);
            EnterpriseResourceDAL EntResDAL = new EnterpriseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());

            try
            {
                foreach (EnterpriseResourcesDTO item in lstEntResourcesDTO)
                {
                    EntResDAL.UpdateEnterpriseResourceByID(item.ID, item.ResourceKey, item.ResourceValue.Trim(), SessionHelper.UserID, "Web", item.chkvalue);
                }

                eTurns.DAL.CacheHelper<IEnumerable<EnterpriseResourcesKeyValDTO>>.InvalidateCacheByKeyStartWith("ResDB");
                AngularApp.AppConfigController.InvalidateCacheByKeyStartWith("ResDB");
                message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                status = ResMessage.SaveMessage;
                ClearCurrentEnterpriseResourceList();
            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                status = "fail";
            }
            finally
            {
                EntResDAL = null;
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditResources(string lang = "", string module = "", string page = "")
        {
            if ((SiteSettingHelper.EnforceResourcePagesRestriction ?? String.Empty) == "yes")
            {
                List<string> alloweduserIds = (SiteSettingHelper.AllowedUserIdsResources ?? String.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!alloweduserIds.Contains(SessionHelper.UserID.ToString()))
                {
                    return RedirectToAction(ActName, CtrlName);
                }
            }
            CurrentCompanyResourceList = null;
            List<SelectListItem> lstLang = GeteTurnsResourceLanguage();
            List<SelectListItem> lstModMast = GetResourceModuleData();
            if (SessionHelper.UserType != 2 && lstModMast != null && lstModMast.Count > 0)
                lstModMast = lstModMast.Where(t => t.Text != "Enterprise").ToList();

            List<SelectListItem> lstResPage = GetResourceModuleDetail(int.Parse(lstModMast[0].Value));

            if (!string.IsNullOrEmpty(lang))
            {
                foreach (var item in lstLang)
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
                        lstResPage = GetResourceModuleDetail(int.Parse(lstModMast[0].Value));
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

        public JsonResult GetCompanyResourceModule(string ModuleID)
        {
            List<SelectListItem> lstItem = null;

            try
            {
                lstItem = GetResourceModuleDetail(int.Parse(ModuleID));

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

        [HttpPost]
        public JsonResult SaveCompanyResources(string SelectedLanguage, string SelectedModule, string SelectedResourcePage, string arrItems = "")
        {
            string message = "";
            string status = "";
            string strModule = string.Empty;
            JavaScriptSerializer s = new JavaScriptSerializer();
            CompanyResourceDAL ResDAL = new CompanyResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
            EnterpriseResourceDAL EntResDAL = new EnterpriseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
            eTurnsBaseResourceDAL objDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
            ResourceModuleMasterDTO objResModuleDTO = objDAL.GetResourceModuleMasterDataByID(Convert.ToInt64(SelectedModule));
            ResModuleMaster resHelper = new ResModuleMaster();
            ResourceModuleDetailsDTO dtoModuleDetail = null;

            try
            {
                ResourceLanguageDTO objLangDTO = objDAL.GetResourceLanguageByID(Convert.ToInt64(SelectedLanguage));
                dtoModuleDetail = objDAL.GetResourceModuleDetailDataByID(Convert.ToInt64(SelectedResourcePage));
                ResourceModuleHelper objResModuleHelper = new ResourceModuleHelper();

                if (objResModuleDTO.ModuleName.Trim() == "Enterprise")
                {
                    List<EnterpriseResourcesDTO> lstEntResourcesDTO = s.Deserialize<List<EnterpriseResourcesDTO>>(arrItems);
                    foreach (EnterpriseResourcesDTO item in lstEntResourcesDTO)
                    {
                        EntResDAL.UpdateEnterpriseResourceByID(item.ID, item.ResourceKey, item.ResourceValue.Trim(), SessionHelper.UserID, "Web", item.chkvalue);
                    }
                    objResModuleHelper.RemoveCache(dtoModuleDetail.FileName, objLangDTO.Culture, false, true, false);
                    eTurns.DAL.CacheHelper<IEnumerable<EnterpriseResourcesKeyValDTO>>.InvalidateCacheByKeyStartWith("ResDB");
                    AngularApp.AppConfigController.InvalidateCacheByKeyStartWith("ResDB");
                }
                else
                {
                    List<CompanyResourcesDTO> lstCmpResourcesDTO = s.Deserialize<List<CompanyResourcesDTO>>(arrItems);
                    foreach (CompanyResourcesDTO item in lstCmpResourcesDTO)
                    {
                        //ResDAL.UpdateCompanyResourceByID(item.ID, item.ResourceKey, item.ResourceValue.Trim(), SessionHelper.UserID, "Web");
                        ResDAL.UpdateCompanyResource(item.ID, item.ResourceKey, item.ResourceValue.Trim(), SessionHelper.UserID, "Web", SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                    }

                    objResModuleHelper.RemoveCache(dtoModuleDetail.FileName, objLangDTO.Culture, false, false, true);
                    eTurns.DAL.CacheHelper<IEnumerable<CompanyResourcesKeyValDTO>>.InvalidateCacheByKeyStartWith("ResDB");
                    AngularApp.AppConfigController.InvalidateCacheByKeyStartWith("ResDB");
                }
                message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                status = ResMessage.SaveMessage;
                ClearCurrentCompanyResourceList();
            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                status = "fail";
            }
            finally
            {
                ResDAL = null;
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDataInCompanyResource(string ID, string changedKey, string changedValue, string AccrossEnt, string IsEdit, string SelectedModule)
        {
            int ResourceSaveMethod = (int)ResourceSave.ButtonSave;
            eTurnsBaseResourceDAL objBaseDAL = new eTurnsBaseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
            CompanyResourceDAL objCmpDAL = new CompanyResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
            ResourceModuleDetailsDTO dtoModuleDetail = null;
            //CompanyResourcesDTO objCmpDTO = objCmpDAL.GetCompanyResourceByID(Convert.ToInt64(ID));
            CompanyResourcesDTO objCmpDTO = objCmpDAL.GetCompanyResource(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, Convert.ToInt64(ID));

            ResourceLanguageDTO objLangDTO = objBaseDAL.GetResourceLanguageByID(Convert.ToInt64(objCmpDTO.LanguageID));
            dtoModuleDetail = objBaseDAL.GetResourceModuleDetailDataByID(Convert.ToInt64(objCmpDTO.ResourcePageID));
            ResourceModuleHelper objResModuleHelper = new ResourceModuleHelper();

            if (SiteSettingHelper.ResourceSave != string.Empty)
            {
                ResourceSaveMethod = Convert.ToInt32(SiteSettingHelper.ResourceSave);
            }
            if (ResourceSaveMethod == (int)ResourceSave.OnChange || !string.IsNullOrWhiteSpace(IsEdit))
            {
                changedValue = changedValue.Trim();
                if (!string.IsNullOrEmpty(changedValue) && !string.IsNullOrWhiteSpace(changedValue) && !string.IsNullOrWhiteSpace(SelectedModule.Trim()))
                {
                    if (SelectedModule.Trim() == "Enterprise")
                    {
                        EnterpriseResourceDAL EntResDAL = new EnterpriseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
                        EntResDAL.UpdateEnterpriseResourceByID(Int64.Parse(ID), changedKey, changedValue, SessionHelper.UserID, "Web", false);

                        objResModuleHelper.RemoveCache(dtoModuleDetail.FileName, objLangDTO.Culture, false, true, false);
                        eTurns.DAL.CacheHelper<IEnumerable<EnterpriseResourcesKeyValDTO>>.InvalidateCacheByKeyStartWith("ResDB");
                        AngularApp.AppConfigController.InvalidateCacheByKeyStartWith("ResDB");
                    }
                    else
                    {
                        CompanyResourceDAL ResDAL = new CompanyResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
                        //ResDAL.UpdateCompanyResourceByID(Int64.Parse(ID), changedKey, changedValue, SessionHelper.UserID, "Web");
                        ResDAL.UpdateCompanyResource(Int64.Parse(ID), changedKey, changedValue, SessionHelper.UserID, "Web", SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);

                        objResModuleHelper.RemoveCache(dtoModuleDetail.FileName, objLangDTO.Culture, false, false, true);
                        eTurns.DAL.CacheHelper<IEnumerable<CompanyResourcesKeyValDTO>>.InvalidateCacheByKeyStartWith("ResDB");
                        AngularApp.AppConfigController.InvalidateCacheByKeyStartWith("ResDB");
                    }
                }
            }

            return Json(new { Message = "sucess", Status = "ok", Value = changedValue }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCompanyResourceList(JQueryDataTableParamModelForResource param)
        {
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            string sortDirection = "asc";

            if (!string.IsNullOrEmpty(Request["sSortDir_0"]))
                sortDirection = Request["sSortDir_0"];

            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            string searchText = string.Empty;
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch.Trim().Length > 0)
                searchText = param.sSearch.Trim();
            sortColumnName = Request["SortingField"].ToString();
            if (sortColumnName == string.Empty)
                sortColumnName = "ResourceKey";
            sortColumnName += " " + sortDirection;

            int TotalRecordCount = 0;

            List<CompanyResourcesDTO> lstResources = null;
            CompanyResourceDAL ResDAL = new CompanyResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());
            EnterpriseResourceDAL EntResDAL = new EnterpriseResourceDAL(DbConnectionHelper.GeteTurnsResourceDBName());

            Int64 resourcePageID = 0;
            Int64 languageID = 0;

            Int64.TryParse(param.resourcefile, out resourcePageID);
            Int64.TryParse(param.resourcelang, out languageID);

            if (param.resourcemodule == "Enterprise")
            {
                lstResources = ResDAL.GetPagedEnterpriseResourceForCompany(SessionHelper.EnterPriceID, resourcePageID, languageID, sortColumnName, searchText);
            }
            else
            {
                lstResources = ResDAL.GetPagedCompanyResource(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, resourcePageID, languageID, sortColumnName, searchText);
            }
            TotalRecordCount = lstResources.Count();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = lstResources
            },
                        JsonRequestBehavior.AllowGet);
        }

        //
        public string CheckResourcesAllEnterprise(string language, string module, string pageResourceFileName)
        {
            //try
            //{
            //    List<KeyValDTO> CurrentBaseResourceList = BaseResourceHelper.GetBaseResourceData(pageResourceFileName, "en-US");
            //    //BaseResourceHelper resHelper = new BaseResourceHelper();
            //    //resHelper.SaveBaseResources(page, item.Value, CurrentBaseResourceList, true);
            //    //foreach (KeyValDTO k in CurrentBaseResourceList)
            //    //{
            //    //    List<KeyValCheckDTO> CurrentBaseResourceListForAll = new List<KeyValCheckDTO>();
            //    //    CurrentBaseResourceListForAll.Add(new KeyValCheckDTO { key = k.key, value = k.value });
            //   // EnterpriseAndCompanyOnlyAddResource(CurrentBaseResourceList, pageResourceFileName, string.Empty);
            //    //}

            //    //var findMe = CurrentBaseResourceList;

            //    //List<KeyValDTO> strings = new List<KeyValDTO>();
            //    //strings.Add(new KeyValDTO { key = "ImagePath", value = "Action" });
            //    //strings.Add(new KeyValDTO { key = "01", value = "Action" });
            //    //strings.Add(new KeyValDTO { key = "Action", value = "Action" });

            //    //var result = findMe.Where(p => !strings.Any(p2 => p2.key == p.key)); ;
            //    return "success";
            //}
            //catch (Exception ex)
            //{
            //    return ex.Message.ToString();
            //}
            return "success";
        }
        public string AddNewResource(string lang = "", string module = "", string page = "", string key = "", string value = "")
        {

            try
            {
                List<SelectListItem> lstLang = GetLanguage();
                List<SelectListItem> lstModMast = GetResourceModule();

                if (SessionHelper.UserType != 2 && lstModMast != null && lstModMast.Count > 0)
                    lstModMast = lstModMast.Where(t => t.Text != "Enterprise").ToList();

                List<SelectListItem> lstResPage = GetResourceFiles(int.Parse(lstModMast[0].Value));
                List<KeyValCheckDTO> CurrentBaseResourceListDTO = null;
                CurrentBaseResourceList = new List<KeyValDTO>();
                CurrentBaseResourceListDTO = new List<KeyValCheckDTO>();
                CurrentBaseResourceList.Add(new KeyValDTO { key = key, value = value });

                CurrentBaseResourceListDTO.Add(new KeyValCheckDTO { key = key, value = value });
                if (!string.IsNullOrEmpty(lang))
                {
                    foreach (var item in lstLang)
                    {

                        string message = "";
                        string status = "";

                        BaseResourceHelper resHelper = new BaseResourceHelper();
                        try
                        {
                            if (module == "Enterprise")
                            {
                                resHelper.SaveBaseEnterpriseResources(page, item.Value, CurrentBaseResourceList, true);
                            }
                            else
                            {
                                resHelper.SaveBaseResources(page, item.Value, CurrentBaseResourceList, true);
                            }
                            UpdateAllEnterpriseAndCompanyResource(CurrentBaseResourceListDTO, page, item.Value);
                            message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                            status = ResMessage.SaveMessage;//"fail";

                        }

                        catch (Exception)
                        {

                        }
                    }
                }


                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString(); ;
            }
        }



    }

    /// <summary>
    /// Class that encapsulates most common parameters sent by DataTables plugin
    /// </summary>
    public class JQueryDataTableParamModelForResource
    {
        /// <summary>
        /// Request sequence number sent by DataTable, same value must be returned in response
        /// </summary>       
        public string sEcho { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }

        public string resourcefile { get; set; }

        public string resourcelang { get; set; }
        public string resourcemodule { get; set; }


    }

    public class ResourceLanguageKey
    {
        public string Key { get; set; }
        public string Language { get; set; }
        public string ConvertValue { get; set; }
    }

}
