using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Xml;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Globalization;
using System.Collections;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Data;

namespace eTurns.DTO.Resources
{
    public class BaseResourceModuleHelper
    {
        private const string ResourceFileExt = ".resx";
        private const int ResouceCacheMaxTime = 104400;

        #region "Properties"

        public static CultureInfo CurrentCult
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session["CurrentCultBase"] != null)
                    return (CultureInfo)HttpContext.Current.Session["CurrentCultBase"];

                return new CultureInfo("en-US");
            }
            set
            {
                HttpContext.Current.Session["CurrentCultBase"] = value;
            }
        }

        public static string FileCulterExtension
        {
            get
            {
                if (CurrentCult.ToString().Contains("en-US"))
                    return "";

                return "." + CurrentCult.ToString();
            }
        }
        public static string BaseResourceDirectoryPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            }
        }
        public static string BaseCompanyResourceDirectoryPath
        {
            get
            {
                return BaseResourceDirectoryPath + @"\MasterResources\CompanyResources\";
            }
        }

        public static string BaseEnterpriseResourceDirectoryPath
        {
            get
            {
                return BaseResourceDirectoryPath + @"\MasterResources\EnterpriseResources\";
            }
        }

        public static DateTime CacheExpiryTime
        {
            get
            {
                return DateTime.Now.AddSeconds(ResouceCacheMaxTime);
            }
        }

        #endregion

        #region Functions

        public static string GetFullFileNamePath(string fileName)
        {
            return BaseCompanyResourceDirectoryPath + fileName + FileCulterExtension + ResourceFileExt;
        }

        public static string GetFullFileNamePath(string fileName, string culter)
        {

            if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
            {
                if (culter == "en-US")
                    culter = "";
                else
                    culter = "." + culter;


                return (BaseCompanyResourceDirectoryPath + fileName + culter + ResourceFileExt).Replace("..", ".");
            }

            return (BaseCompanyResourceDirectoryPath + fileName + ResourceFileExt).Replace("..", ".");

        }


        public static string GetBaseResourceValue(string ResourceKey, string ResourceFile)
        {
            List<BaseResourcesKeyValDTO> lstBaseResDTO = null;
            BaseResourcesKeyValDTO objBaseResDTO = null;
            string strRetResourceValue = ResourceKey;
            try
            {
                string strFilePath = GetFullFileNamePath(ResourceFile);
                string SessinKey = "ResDB_BaseCompanyResource_" + ResourceFile + FileCulterExtension;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstBaseResDTO = (List<BaseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstBaseResDTO == null)
                {
                    lstBaseResDTO = new List<BaseResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    lstBaseResDTO = objResDbHelper.GetBaseResourceByResFileCulture(ResourceFile, FileCulterExtension);

                    CacheDependency cacheDep = new CacheDependency(strFilePath);
                    HttpContext.Current.Cache.Add(SessinKey, lstBaseResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                    if (lstBaseResDTO != null && lstBaseResDTO.Count > 0)
                    {
                        objBaseResDTO = lstBaseResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                        strRetResourceValue = (objBaseResDTO == null || (string.IsNullOrWhiteSpace(objBaseResDTO.ResourceValue))) ? ResourceKey : objBaseResDTO.ResourceValue;
                    }
                }
                else
                {
                    if (lstBaseResDTO != null && lstBaseResDTO.Count > 0)
                    {
                        objBaseResDTO = lstBaseResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                        strRetResourceValue = (lstBaseResDTO == null || (string.IsNullOrWhiteSpace(objBaseResDTO.ResourceValue))) ? ResourceKey : objBaseResDTO.ResourceValue;
                    }
                }
                return strRetResourceValue; 
            }
            catch (Exception)
            {
                //throw ex;
                return ResourceKey;
                // Add exception log code here;
            }

            finally
            {
                lstBaseResDTO = null;
                objBaseResDTO = null;
            }
        }

        public static string GetBaseEnterPriseResourceValue(string ResourceKey, string ResourceFile)
        {
            List<BaseResourcesKeyValDTO> lstBaseResDTO = null;
            BaseResourcesKeyValDTO objBaseResDTO = null;
            string strRetResourceValue = ResourceKey;
            try
            {
                string strFilePath = BaseEnterpriseResourceDirectoryPath + ResourceFile + FileCulterExtension + ResourceFileExt;
                string SessinKey = "ResDB_BaseEnterpriseResource_" + ResourceFile + FileCulterExtension;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstBaseResDTO = (List<BaseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstBaseResDTO == null)
                {
                    lstBaseResDTO = new List<BaseResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    lstBaseResDTO = objResDbHelper.GetBaseResourceByResFileCulture(ResourceFile, FileCulterExtension);

                    CacheDependency cacheDep = new CacheDependency(strFilePath);
                    HttpContext.Current.Cache.Add(SessinKey, lstBaseResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                    if (lstBaseResDTO != null && lstBaseResDTO.Count > 0)
                    {
                        objBaseResDTO = lstBaseResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                        strRetResourceValue = (objBaseResDTO == null || (string.IsNullOrWhiteSpace(objBaseResDTO.ResourceValue))) ? ResourceKey : objBaseResDTO.ResourceValue;
                    }
                }
                else
                {
                    if (lstBaseResDTO != null && lstBaseResDTO.Count > 0)
                    {
                        objBaseResDTO = lstBaseResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                        strRetResourceValue = (lstBaseResDTO == null || (string.IsNullOrWhiteSpace(objBaseResDTO.ResourceValue))) ? ResourceKey : objBaseResDTO.ResourceValue;
                    }
                }
                return strRetResourceValue; 
            }
            catch (Exception)
            {
                //throw ex;
                return ResourceKey;
                // Add exception log code here;
            }
            finally
            {
                lstBaseResDTO = null;
                objBaseResDTO = null;
            }
        }

        public static List<KeyValDTO> GetBaseEnterpriseResourceData(string ResourceFile, string culter)
        {
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            List<BaseResourcesKeyValDTO> lstBaseResDTO = null;
            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                string SessinKey = "ResDB_BaseEnterpriseResource_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstBaseResDTO = (List<BaseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstBaseResDTO == null)
                {
                    string strFilePath = string.Empty; 
                    if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
                    {
                        if (culter == "en-US")
                            culter = "";
                        else
                            culter = "." + culter;


                        strFilePath = (BaseEnterpriseResourceDirectoryPath + ResourceFile + culter + ResourceFileExt).Replace("..", ".");
                    }
                    else
                    {
                        strFilePath = (BaseEnterpriseResourceDirectoryPath + ResourceFile + ResourceFileExt).Replace("..", ".");
                    }

                    lstBaseResDTO = new List<BaseResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    lstBaseResDTO = objResDbHelper.GetBaseResourceByResFileCulture(ResourceFile, culter);


                    if (lstBaseResDTO != null)
                    {
                        foreach (BaseResourcesKeyValDTO objBaseResDTO in lstBaseResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objBaseResDTO.ResourceKey;
                            kv.value = objBaseResDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                else
                {
                    if (lstBaseResDTO != null && lstBaseResDTO.Count > 0)
                    {
                        foreach (BaseResourcesKeyValDTO objBaseResDTO in lstBaseResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objBaseResDTO.ResourceKey;
                            kv.value = objBaseResDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                return objList;
            }
            catch (Exception)
            {
                return objList;
                // Add exception log code here;
            }

            finally
            {
                lstBaseResDTO = null;
            }

        }

        public static List<KeyValDTO> GetBaseResourceData(string ResourceFile, string culter)
        {
            List<BaseResourcesKeyValDTO> lstBaseResDTO = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                string SessinKey = "ResDB_BaseCompanyResource_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstBaseResDTO = (List<BaseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstBaseResDTO == null)
                {
                    string strFilePath = GetFullFileNamePath(ResourceFile, culter);

                    lstBaseResDTO = new List<BaseResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    lstBaseResDTO = objResDbHelper.GetBaseResourceByResFileCulture(ResourceFile, culter);

                    if (lstBaseResDTO != null && lstBaseResDTO.Count > 0)
                    {
                        foreach (BaseResourcesKeyValDTO objBaseResDTO in lstBaseResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objBaseResDTO.ResourceKey;
                            kv.value = objBaseResDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                else
                {
                    if (lstBaseResDTO != null && lstBaseResDTO.Count > 0)
                    {
                        foreach (BaseResourcesKeyValDTO objBaseResDTO in lstBaseResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objBaseResDTO.ResourceKey;
                            kv.value = objBaseResDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }


                return objList;
            }
            catch (Exception)
            {
                return objList;
                // Add exception log code here;
            }

            finally
            {
                lstBaseResDTO = null;
            }

        }


        #endregion
    }

    public class ResourceModuleHelper
    {
        private const string ResourceFileExt = ".resx";
        private const int ResouceCacheMaxTime = 104400;

        #region "Properties"
        private static CultureInfo _CurrentCult;
        public static CultureInfo CurrentCult
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentCult"] != null)
                    return (CultureInfo)HttpContext.Current.Session["CurrentCult"];
                else if (_CurrentCult != null)
                    return _CurrentCult;
                else
                    return new CultureInfo("en-US");
            }
            set
            {
                //if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentCult"] != null)
                if (HttpContext.Current != null && HttpContext.Current.Session != null )
                {
                    HttpContext.Current.Session["CurrentCult"] = value;
                    _CurrentCult = value;
                }
                else
                    _CurrentCult = value;
            }
        }

        public static string FileCulterExtension
        {
            get
            {
                if (CurrentCult.ToString().Contains("en-US"))
                    return "";

                return "." + CurrentCult.ToString();
            }
        }

        public static string EnterpriseResourceFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["EnterpriseResourceFolder"])) && !string.IsNullOrWhiteSpace(Convert.ToString(HttpContext.Current.Session["EnterpriseResourceFolder"])))
                {
                    return Convert.ToString(HttpContext.Current.Session["EnterpriseResourceFolder"]);
                }
                else
                    return "MasterResources";
            }
            set
            {
                HttpContext.Current.Session["EnterpriseResourceFolder"] = value;
            }
        }

        public static string CompanyResourceFolder
        {
            get
            {
                string entCompDir = Convert.ToString(HttpContext.Current.Session["CompanyResourceFolder"]);
                if (!string.IsNullOrEmpty(entCompDir))
                {
                    string fldr = ResourceDirectoryBasePath + "\\" + HttpContext.Current.Session["CompanyResourceFolder"];
                    if (!Directory.Exists(fldr))
                    {
                        entCompDir = Convert.ToString(HttpContext.Current.Session["CompanyResourceFolder"]);
                        string[] strSplit = entCompDir.Split(new char[1] { '\\' });
                        entCompDir = strSplit[0] + "\\CompanyResources";
                    }
                }
                return entCompDir;
            }
            set
            {
                HttpContext.Current.Session["CompanyResourceFolder"] = value;
            }
        }

        public static string RoomResourceFolder
        {
            get
            {
                string entCompDir = Convert.ToString(HttpContext.Current.Session["RoomResourceFolder"]);
                if (!string.IsNullOrEmpty(entCompDir))
                {
                    string fldr = ResourceDirectoryBasePath + "\\" + HttpContext.Current.Session["RoomResourceFolder"];
                    if (!Directory.Exists(fldr))
                    {
                        Directory.CreateDirectory(fldr);
                        //entCompDir = CompanyResourceFolder;
                    }
                }
                return entCompDir;
            }
            set
            {
                HttpContext.Current.Session["RoomResourceFolder"] = value;
            }
        }


        public static string ResourceDirectoryBasePath
        {
            get
            {
                //return HttpContext.Current.Server.MapPath(@"\Resources");
                return System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            }
        }
        public static string RDLReportDirectoryBasePath
        {
            get
            {
                //return HttpContext.Current.Server.MapPath(@"\RDLC_Reports");
                return System.Configuration.ConfigurationManager.AppSettings["RDLCBaseFilePath"];
            }
        }
        public static string CompanyResourceDirectoryPath
        {
            get
            {
                return ResourceDirectoryBasePath + @"\" + CompanyResourceFolder + @"\";
            }
        }

        public static string RoomResourceDirectoryPath
        {
            get
            {
                return ResourceDirectoryBasePath + @"\" + RoomResourceFolder + @"\";
            }
        }

        public static string EnterpriseResourceDirectoryPath
        {
            get
            {
                return ResourceDirectoryBasePath + @"\" + EnterpriseResourceFolder + @"\EnterpriseResources\";
            }
        }
        public static string RoomResourceDirectoryPathForEnterprise
        {
            get
            {
                return ResourceDirectoryBasePath + @"\" + EnterpriseResourceFolder + @"\RoomResources\";
            }
        }
        public static DateTime CacheExpiryTime
        {
            get
            {
                return DateTime.Now.AddSeconds(ResouceCacheMaxTime);
            }
        }
        #endregion

        #region Functions
        public static string GetResourceValue(string ResourceKey, string ResourceFile, bool isUDFName = false, bool IsFromWeb = true, bool OtherFromeTurns = true, bool ForEnterPriseSetup = false)
        {
            List<CompanyResourcesKeyValDTO> lstCmpResDTO = null;
            CompanyResourcesKeyValDTO objCmpResDTO = null;
            List<EnterpriseResourcesKeyValDTO> lstEntResDTO = null;
            EnterpriseResourcesKeyValDTO objEntResDTO = null;
            List<BaseResourcesKeyValDTO> lstBaseResDTO = null;
            BaseResourcesKeyValDTO objBaseResDTO = null;

            try
            {
                string strFilePath = string.Empty;
                string SessinKey = string.Empty;
                List<string> Ent_Cmp_Room = new List<string>();
                bool IsCompanyResource = false;
                string strRetResourceValue = ResourceKey;
                bool IsEnterpriseResource = false;
                bool IsBaseResource = false;

                if (IsFromWeb)
                {
                    if (OtherFromeTurns)
                    {
                        if (!ForEnterPriseSetup)
                        {
                            strFilePath = GetFullFileNamePathRoom(ResourceFile);
                            SessinKey = "ResDB_" + RoomResourceFolder + "_" + ResourceFile + FileCulterExtension;
                            Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            IsCompanyResource = true;
                        }
                        else
                        {
                            if (isUDFName)
                            {
                                strFilePath = RoomResourceDirectoryPathForEnterprise + ResourceFile + FileCulterExtension + ResourceFileExt;
                                SessinKey = "ResDB_" + RoomResourceDirectoryPathForEnterprise + "_" + ResourceFile + FileCulterExtension;
                                Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                IsEnterpriseResource = true;
                            }
                            else
                            {
                                if (ResourceFile.ToLower() == "resusermasterudf" || ResourceKey.ToLower().Contains("createdbyudf") || ResourceKey.ToLower().Contains("updatedbyudf") || ResourceKey.ToLower().Contains("createdby_udf") || ResourceKey.ToLower().Contains("updatedby_udf"))
                                {
                                    strFilePath = EnterpriseResourceDirectoryPath + ResourceFile + FileCulterExtension + ResourceFileExt;
                                    SessinKey = "ResDB_" + EnterpriseResourceDirectoryPath + "_" + ResourceFile + FileCulterExtension;
                                    //Ent_Cmp_Room = EnterpriseResourceFolder;
                                    Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    IsEnterpriseResource = true;
                                }
                            }

                        }
                    }
                    else
                    {
                        if (isUDFName)
                        {
                            strFilePath = strFilePath = ResourceDirectoryBasePath + @"\MasterResources\RoomResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                            SessinKey = @"ResDB_Resources\MasterResources\RoomResources_" + ResourceFile + FileCulterExtension;
                            Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            //IsCompanyResource = true;
                            IsBaseResource = true;
                        }
                        else
                        {
                            SessinKey = @"ResDB_Resources\MasterResources\EnterpriseResources_" + ResourceFile + FileCulterExtension;
                            strFilePath = ResourceDirectoryBasePath + @"\MasterResources\EnterpriseResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                            Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            IsBaseResource = true;
                        }
                    }

                }
                else
                {
                    if (OtherFromeTurns)
                    {
                        if (!ForEnterPriseSetup)
                        {
                            if (isUDFName)
                            {
                                strFilePath = ResourceDirectoryBasePath + @"\" + RoomResourceFolder + @"\" + ResourceFile + FileCulterExtension + ".resx";
                                //SessinKey = "ResDB_" + RoomResourceFolder + "_" + ResourceFile + FileCulterExtension;
                            }
                            else
                            {
                                strFilePath = ResourceDirectoryBasePath + @"\" + RoomResourceFolder + @"\" + ResourceFile + ".resx";
                                //SessinKey = "ResDB_" + CompanyResourceFolder + "_" + ResourceFile + FileCulterExtension;
                            }
                            SessinKey = "ResDB_" + RoomResourceFolder + "_" + ResourceFile + FileCulterExtension;
                            Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            IsCompanyResource = true;
                        }
                        else
                        {
                            if (isUDFName)
                            {
                                strFilePath = RoomResourceDirectoryPathForEnterprise + ResourceFile + FileCulterExtension + ResourceFileExt;
                                SessinKey = "ResDB_" + RoomResourceDirectoryPathForEnterprise + "_" + ResourceFile + FileCulterExtension;
                                Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                IsEnterpriseResource = true;
                            }

                        }
                    }
                    else
                    {
                        if (isUDFName)
                        {
                            strFilePath = strFilePath = ResourceDirectoryBasePath + @"\MasterResources\RoomResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                            SessinKey = @"ResDB_Resources\MasterResources\RoomResources_" + ResourceFile + FileCulterExtension;
                            Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            IsBaseResource = true;
                        }
                        else
                        {
                            SessinKey = @"ResDB_Resources\MasterResources\EnterpriseResources_" + ResourceFile + FileCulterExtension;
                            strFilePath = ResourceDirectoryBasePath + @"\MasterResources\EnterpriseResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                            Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            IsBaseResource = true;
                        }
                        
                    }
                }

                
                if (IsCompanyResource)
                {
                    #region IsCompanyResource
                    if (HttpContext.Current.Cache.Get(SessinKey) != null)
                        lstCmpResDTO = (List<CompanyResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                    if (lstCmpResDTO == null)
                    {
                        lstCmpResDTO = new List<CompanyResourcesKeyValDTO>();
                        ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                        lstCmpResDTO = objResDbHelper.GetCompanyResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), Convert.ToInt64(Ent_Cmp_Room[1]), Convert.ToInt64(Ent_Cmp_Room[2]), ResourceFile, FileCulterExtension);

                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, lstCmpResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                        if (lstCmpResDTO != null && lstCmpResDTO.Count > 0)
                        {
                            objCmpResDTO = lstCmpResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                            strRetResourceValue = (objCmpResDTO == null || (string.IsNullOrWhiteSpace(objCmpResDTO.ResourceValue))) ? ResourceKey : objCmpResDTO.ResourceValue;
                        }

                    }
                    else
                    {
                        if (lstCmpResDTO != null && lstCmpResDTO.Count > 0)
                        {
                            objCmpResDTO = lstCmpResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                            strRetResourceValue = (objCmpResDTO == null || (string.IsNullOrWhiteSpace(objCmpResDTO.ResourceValue))) ? ResourceKey : objCmpResDTO.ResourceValue;
                        }
                    }
                    #endregion
                } 
                else if (IsEnterpriseResource)
                {
                    #region IsEnterpriseResource
                    if (HttpContext.Current.Cache.Get(SessinKey) != null)
                        lstEntResDTO = (List<EnterpriseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                    if (lstEntResDTO == null)
                    {
                        lstEntResDTO = new List<EnterpriseResourcesKeyValDTO>();
                        ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                        lstEntResDTO = objResDbHelper.GetEnterpriseResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), ResourceFile, FileCulterExtension);

                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, lstEntResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                        if (lstEntResDTO != null && lstEntResDTO.Count > 0)
                        {
                            objEntResDTO = lstEntResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                            strRetResourceValue = (objEntResDTO == null || (string.IsNullOrWhiteSpace(objEntResDTO.ResourceValue))) ? ResourceKey : objEntResDTO.ResourceValue;
                        }

                    }
                    else
                    {
                        if (lstEntResDTO != null && lstEntResDTO.Count > 0)
                        {
                            objEntResDTO = lstEntResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                            strRetResourceValue = (objEntResDTO == null || (string.IsNullOrWhiteSpace(objEntResDTO.ResourceValue))) ? ResourceKey : objEntResDTO.ResourceValue;
                        }
                    }
                    #endregion
                }
                else if (IsBaseResource)
                {
                    #region IsBaseResource
                    if (HttpContext.Current.Cache.Get(SessinKey) != null)
                        lstBaseResDTO = (List<BaseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                    if (lstBaseResDTO == null)
                    {
                        lstBaseResDTO = new List<BaseResourcesKeyValDTO>();
                        ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                        lstBaseResDTO = objResDbHelper.GetBaseResourceByResFileCulture(ResourceFile, FileCulterExtension);

                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, lstBaseResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                        if (lstBaseResDTO != null && lstBaseResDTO.Count > 0)
                        {
                            objBaseResDTO = lstBaseResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                            strRetResourceValue = (objBaseResDTO == null || (string.IsNullOrWhiteSpace(objBaseResDTO.ResourceValue))) ? ResourceKey : objBaseResDTO.ResourceValue;
                        }

                    }
                    else
                    {
                        if (lstBaseResDTO != null && lstBaseResDTO.Count > 0)
                        {
                            objBaseResDTO = lstBaseResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                            strRetResourceValue = (lstBaseResDTO == null || (string.IsNullOrWhiteSpace(objBaseResDTO.ResourceValue))) ? ResourceKey : objBaseResDTO.ResourceValue;
                        }
                    }
                    #endregion
                }
                return strRetResourceValue; 
            }
            
            catch(Exception ex)
            {
                return ResourceKey;
            }
            finally
            {
                lstCmpResDTO = null;
                objCmpResDTO = null;
                lstEntResDTO = null;
                objEntResDTO = null;
                lstBaseResDTO = null;
                objBaseResDTO = null;
            }
        }
        public static string GetUserMasterResourceValue(string ResourceKey, string ResourceFile, bool isUDFName = false, bool IsFromWeb = true, bool OtherFromeTurns = true, bool ForEnterPriseSetup = false, Int64 EnterpriseID = 0)
        {
            List<CompanyResourcesKeyValDTO> lstCmpResDTO = null;
            CompanyResourcesKeyValDTO objCmpResDTO = null;
            List<EnterpriseResourcesKeyValDTO> lstEntResDTO = null;
            EnterpriseResourcesKeyValDTO objEntResDTO = null;
            List<BaseResourcesKeyValDTO> lstBaseResDTO = null;
            BaseResourcesKeyValDTO objBaseResDTO = null;
            try
            {
                string strFilePath = string.Empty;
                string SessinKey = string.Empty;
                List<string> Ent_Cmp_Room = new List<string>();
                bool IsCompanyResource = false;
                string strRetResourceValue = ResourceKey;
                bool IsEnterpriseResource = false;
                bool IsBaseResource = false;

                if (IsFromWeb)
                {
                    if (OtherFromeTurns)
                    {
                        if (!ForEnterPriseSetup)
                        {
                            strFilePath = GetFullFileNamePathRoom(ResourceFile);
                            SessinKey = "ResDB_" + RoomResourceFolder + "_" + ResourceFile + FileCulterExtension;
                            Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            IsCompanyResource = true;
                        }
                        else
                        {
                            if (isUDFName)
                            {
                                strFilePath = RoomResourceDirectoryPathForEnterprise + ResourceFile + FileCulterExtension + ResourceFileExt;
                                SessinKey = "ResDB_" + RoomResourceDirectoryPathForEnterprise + "_" + ResourceFile + FileCulterExtension;
                            }

                        }
                    }
                    else
                    {
                        if (isUDFName)
                        {
                            strFilePath = strFilePath = ResourceDirectoryBasePath + @"\MasterResources\RoomResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                            SessinKey = @"ResDB_Resources\MasterResources\RoomResources_" + ResourceFile + FileCulterExtension;
                            Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            IsCompanyResource = true;
                        }
                        else
                        {
                            SessinKey = @"ResDB_Resources\" + EnterpriseID + @"\EnterpriseResources_" + ResourceFile + FileCulterExtension;
                            strFilePath = ResourceDirectoryBasePath + @"\" + EnterpriseID + @"\EnterpriseResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                            IsBaseResource = true;
                        }
                    }

                }
                else
                {
                    if (OtherFromeTurns)
                    {
                        if (!ForEnterPriseSetup)
                        {
                            if (isUDFName)
                            {
                                strFilePath = ResourceDirectoryBasePath + @"\" + RoomResourceFolder + @"\" + ResourceFile + ".resx";
                                SessinKey = "ResDB_" + RoomResourceFolder + "_" + ResourceFile + FileCulterExtension;
                            }
                            else
                            {
                                strFilePath = ResourceDirectoryBasePath + @"\" + RoomResourceFolder + @"\" + ResourceFile + ".resx";
                                SessinKey = "ResDB_" + CompanyResourceFolder + "_" + ResourceFile + FileCulterExtension;
                            }
                            Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            IsCompanyResource = true;
                        }
                        else
                        {
                            if (isUDFName)
                            {
                                strFilePath = RoomResourceDirectoryPathForEnterprise + ResourceFile + FileCulterExtension + ResourceFileExt;
                                SessinKey = "ResDB_" + RoomResourceDirectoryPathForEnterprise + "_" + ResourceFile + FileCulterExtension;

                            }

                        }
                    }
                    else
                    {
                        if (isUDFName)
                        {
                            strFilePath = strFilePath = ResourceDirectoryBasePath + @"\MasterResources\RoomResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                            SessinKey = @"ResDB_Resources\MasterResources\RoomResources_" + ResourceFile + FileCulterExtension;
                            Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            IsCompanyResource = true;
                        }
                        else
                        {
                            SessinKey = @"ResDB_Resources\" + EnterpriseID + @"\EnterpriseResources_" + ResourceFile + FileCulterExtension;
                            strFilePath = ResourceDirectoryBasePath + @"\" + EnterpriseID + @"\EnterpriseResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                            IsEnterpriseResource = true;
                        }

                    }
                }

                if (IsCompanyResource)
                {
                    #region IsCompanyResource
                    if (HttpContext.Current.Cache.Get(SessinKey) != null)
                        lstCmpResDTO = (List<CompanyResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                    if (lstCmpResDTO == null)
                    {
                        lstCmpResDTO = new List<CompanyResourcesKeyValDTO>();
                        ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                        lstCmpResDTO = objResDbHelper.GetCompanyResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), Convert.ToInt64(Ent_Cmp_Room[1]), Convert.ToInt64(Ent_Cmp_Room[2]), ResourceFile, FileCulterExtension);

                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, lstCmpResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                        if (lstCmpResDTO != null && lstCmpResDTO.Count > 0)
                        {
                            objCmpResDTO = lstCmpResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                            strRetResourceValue = (objCmpResDTO == null || (string.IsNullOrWhiteSpace(objCmpResDTO.ResourceValue))) ? ResourceKey : objCmpResDTO.ResourceValue;
                        }

                    }
                    else
                    {
                        if (lstCmpResDTO != null && lstCmpResDTO.Count > 0)
                        {
                            objCmpResDTO = lstCmpResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                            strRetResourceValue = (objCmpResDTO == null || (string.IsNullOrWhiteSpace(objCmpResDTO.ResourceValue))) ? ResourceKey : objCmpResDTO.ResourceValue;
                        }
                    }
                    #endregion
                }
                else if (IsEnterpriseResource)
                {
                    #region IsEnterpriseResource
                    if (HttpContext.Current.Cache.Get(SessinKey) != null)
                        lstEntResDTO = (List<EnterpriseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                    if (lstEntResDTO == null)
                    {
                        lstEntResDTO = new List<EnterpriseResourcesKeyValDTO>();
                        ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                        lstEntResDTO = objResDbHelper.GetEnterpriseResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), ResourceFile, FileCulterExtension);

                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, lstEntResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                        if (lstEntResDTO != null && lstEntResDTO.Count > 0)
                        {
                            objEntResDTO = lstEntResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                            strRetResourceValue = (objEntResDTO == null || (string.IsNullOrWhiteSpace(objEntResDTO.ResourceValue))) ? ResourceKey : objEntResDTO.ResourceValue;
                        }

                    }
                    else
                    {
                        if (lstEntResDTO != null && lstEntResDTO.Count > 0)
                        {
                            objEntResDTO = lstEntResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                            strRetResourceValue = (objEntResDTO == null || (string.IsNullOrWhiteSpace(objEntResDTO.ResourceValue))) ? ResourceKey : objEntResDTO.ResourceValue;
                        }
                    }
                    #endregion
                }
                else if (IsBaseResource)
                {
                    #region IsBaseResource
                    if (HttpContext.Current.Cache.Get(SessinKey) != null)
                        lstBaseResDTO = (List<BaseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                    if (lstBaseResDTO == null)
                    {
                        lstBaseResDTO = new List<BaseResourcesKeyValDTO>();
                        ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                        lstBaseResDTO = objResDbHelper.GetBaseResourceByResFileCulture(ResourceFile, FileCulterExtension);

                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, lstBaseResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                        if (lstBaseResDTO != null && lstBaseResDTO.Count > 0)
                        {
                            objBaseResDTO = lstBaseResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                            strRetResourceValue = (objBaseResDTO == null || (string.IsNullOrWhiteSpace(objBaseResDTO.ResourceValue))) ? ResourceKey : objBaseResDTO.ResourceValue;
                        }

                    }
                    else
                    {
                        if (lstBaseResDTO != null && lstBaseResDTO.Count > 0)
                        {
                            objBaseResDTO = lstBaseResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                            strRetResourceValue = (lstBaseResDTO == null || (string.IsNullOrWhiteSpace(objBaseResDTO.ResourceValue))) ? ResourceKey : objBaseResDTO.ResourceValue;
                        }
                    }
                    #endregion
                }
                return strRetResourceValue; 
            }
            catch (Exception)
            {
                //throw ex;
                return ResourceKey;
            }
            finally
            {
                lstCmpResDTO = null;
                objCmpResDTO = null;
                lstEntResDTO = null;
                objEntResDTO = null;
                lstBaseResDTO = null;
                objBaseResDTO = null;
            }
        }

        public static string GetEnterPriseResourceValue(string ResourceKey, string ResourceFile)
        {
            List<EnterpriseResourcesKeyValDTO> lstEntResDTO = null;
            EnterpriseResourcesKeyValDTO objEntResDTO = null;
            try
            {
                List<string> Ent_Cmp_Room = new List<string>();
                string strRetResourceValue = ResourceKey;
                
                string strFilePath = EnterpriseResourceDirectoryPath + ResourceFile + FileCulterExtension + ResourceFileExt; ;
                string SessinKey = "ResDB_" + EnterpriseResourceFolder + "_" + ResourceFile + FileCulterExtension;
                Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstEntResDTO = (List<EnterpriseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstEntResDTO == null)
                {
                    lstEntResDTO = new List<EnterpriseResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    lstEntResDTO = objResDbHelper.GetEnterpriseResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), ResourceFile, FileCulterExtension);

                    CacheDependency cacheDep = new CacheDependency(strFilePath);
                    HttpContext.Current.Cache.Add(SessinKey, lstEntResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                    if (lstEntResDTO != null && lstEntResDTO.Count > 0)
                    {
                        objEntResDTO = lstEntResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                        strRetResourceValue = (objEntResDTO == null || (string.IsNullOrWhiteSpace(objEntResDTO.ResourceValue))) ? ResourceKey : objEntResDTO.ResourceValue;
                    }
                }
                else
                {
                    if (lstEntResDTO != null && lstEntResDTO.Count > 0)
                    {
                        objEntResDTO = lstEntResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                        strRetResourceValue = (objEntResDTO == null || (string.IsNullOrWhiteSpace(objEntResDTO.ResourceValue))) ? ResourceKey : objEntResDTO.ResourceValue;
                    }
                }
                return strRetResourceValue; 
                 
            }
            catch (Exception)
            {
                return ResourceKey;
            }
            finally
            {
                lstEntResDTO = null;
                objEntResDTO = null;
            }
        }

        public static string GetFullFileNamePath(string fileName)
        {
            return CompanyResourceDirectoryPath + fileName + FileCulterExtension + ResourceFileExt;
        }

        public static string GetFullFileNamePathRoom(string fileName)
        {
            return RoomResourceDirectoryPath + fileName + FileCulterExtension + ResourceFileExt;
        }

        public static string GetFullFileNamePathRoom(string fileName, string culter)
        {

            if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
            {
                if (culter == "en-US")
                    culter = "";
                else
                    culter = "." + culter;


                return (RoomResourceDirectoryPath + fileName + culter + ResourceFileExt).Replace("..", ".");
            }

            return (RoomResourceDirectoryPath + fileName + ResourceFileExt).Replace("..", ".");

        }
        public static string GetFullFileNamePath(string fileName, string culter)
        {

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

        public static List<KeyValDTO> GetResourceData(string ResourceFile, string culter)
        {
            List<CompanyResourcesKeyValDTO> lstCmpResDTO = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            List<string> Ent_Cmp_Room = new List<string>();

            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                //string SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + sessionculter;
                string SessinKey = "ResDB_" + RoomResourceFolder + "_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstCmpResDTO = (List<CompanyResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstCmpResDTO == null)
                {
                    //string strFilePath = GetFullFileNamePath(ResourceFile, culter);
                    string strFilePath = GetFullFileNamePathRoom(ResourceFile, culter);

                    lstCmpResDTO = new List<CompanyResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    lstCmpResDTO = objResDbHelper.GetCompanyResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), Convert.ToInt64(Ent_Cmp_Room[1]), Convert.ToInt64(Ent_Cmp_Room[2]), ResourceFile, culter);
                    if (lstCmpResDTO != null)
                    {
                        foreach (CompanyResourcesKeyValDTO objCmpDTO in lstCmpResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objCmpDTO.ResourceKey;
                            kv.value = objCmpDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                else
                {
                    if (lstCmpResDTO != null && lstCmpResDTO.Count > 0)
                    {
                        foreach (CompanyResourcesKeyValDTO objCmpDTO in lstCmpResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objCmpDTO.ResourceKey;
                            kv.value = objCmpDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                return objList;
            }
            catch (Exception)
            {
                return objList;
            }
            finally
            {
                lstCmpResDTO = null;
            }

        }
        public static List<KeyValDTO> GetUDFResourceData(string ResourceFile, string culter)
        {
            List<CompanyResourcesKeyValDTO> lstCmpResDTO = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            List<string> Ent_Cmp_Room = new List<string>();

            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                //string SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + sessionculter + "_UDF";
                string SessinKey = "ResDB_" + RoomResourceFolder + "_" + ResourceFile + sessionculter + "_UDF";

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstCmpResDTO = (List<CompanyResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstCmpResDTO == null)
                {
                    string strUDFFilePath = GetFullFileNamePathRoom(ResourceFile, culter);

                    lstCmpResDTO = new List<CompanyResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    lstCmpResDTO = objResDbHelper.GetCompanyResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), Convert.ToInt64(Ent_Cmp_Room[1]), Convert.ToInt64(Ent_Cmp_Room[2]), ResourceFile, culter);

                    if (lstCmpResDTO != null)
                    {
                        foreach (CompanyResourcesKeyValDTO objCmpDTO in lstCmpResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objCmpDTO.ResourceKey;
                            kv.value = objCmpDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                else
                {
                    if (lstCmpResDTO != null && lstCmpResDTO.Count > 0)
                    {
                        foreach (CompanyResourcesKeyValDTO objCmpDTO in lstCmpResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objCmpDTO.ResourceKey;
                            kv.value = objCmpDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                return objList;
            }
            catch (Exception)
            {
                return objList;
            }
            finally
            {
                lstCmpResDTO = null;
            }

        }

        public static List<KeyValDTO> GetResourceData(string ResourceFile, string culter, string SearchText)
        {
            List<CompanyResourcesKeyValDTO> lstCmpResDTO = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            List<string> Ent_Cmp_Room = new List<string>();

            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                //string SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + sessionculter;
                string SessinKey = "ResDB_" + RoomResourceFolder + "_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstCmpResDTO = (List<CompanyResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstCmpResDTO == null)
                {
                    //string strFilePath = GetFullFileNamePath(ResourceFile, culter);
                    string strFilePath = GetFullFileNamePathRoom(ResourceFile, culter);

                    lstCmpResDTO = new List<CompanyResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    lstCmpResDTO = objResDbHelper.GetCompanyResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), Convert.ToInt64(Ent_Cmp_Room[1]), Convert.ToInt64(Ent_Cmp_Room[2]), ResourceFile, culter);

                    if (lstCmpResDTO != null)
                    {
                        lstCmpResDTO = lstCmpResDTO.Where(x => x.ResourceKey.ToLower() == SearchText.ToLower() || x.ResourceValue.ToLower() == SearchText.ToLower()).ToList();
                        foreach (CompanyResourcesKeyValDTO objCmpDTO in lstCmpResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objCmpDTO.ResourceKey;
                            kv.value = objCmpDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                else
                {
                    if (lstCmpResDTO != null && lstCmpResDTO.Count > 0)
                    {
                        lstCmpResDTO = lstCmpResDTO.Where(x => x.ResourceKey.ToLower() == SearchText.ToLower() || x.ResourceValue.ToLower() == SearchText.ToLower()).ToList();
                        foreach (CompanyResourcesKeyValDTO objCmpDTO in lstCmpResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objCmpDTO.ResourceKey;
                            kv.value = objCmpDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                return objList;
            }
            catch (Exception)
            {
                return objList;
            }
            finally
            {
                lstCmpResDTO = null;
            }
        }
        public static List<KeyValDTO> GetEnterpriseResourceData(string ResourceFile, string culter)
        {
            List<EnterpriseResourcesKeyValDTO> lstEntResDTO = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            List<string> Ent_Cmp_Room = new List<string>();

            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                string SessinKey = "ResDB_" + EnterpriseResourceFolder + "_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstEntResDTO = (List<EnterpriseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstEntResDTO == null)
                {
                    lstEntResDTO = new List<EnterpriseResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    lstEntResDTO = objResDbHelper.GetEnterpriseResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), ResourceFile, culter);

                    if (lstEntResDTO != null)
                    {
                        foreach (EnterpriseResourcesKeyValDTO objEntDTO in lstEntResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objEntDTO.ResourceKey;
                            kv.value = objEntDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                else
                {
                    if (lstEntResDTO != null && lstEntResDTO.Count > 0)
                    {
                        foreach (EnterpriseResourcesKeyValDTO objEntDTO in lstEntResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objEntDTO.ResourceKey;
                            kv.value = objEntDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                return objList;
            }
            catch (Exception)
            {
                return objList;
            }
            finally
            {
                lstEntResDTO = null;
            }
        }
        public string getFilePath(string ResourceFile, bool isUDFName)
        {
            string strFilePath = string.Empty;
            if (isUDFName)
            {
                strFilePath = GetFullFileNamePathRoom(ResourceFile);
            }
            else
            {
                strFilePath = GetFullFileNamePath(ResourceFile);
            }
            return strFilePath;
        }

        public bool SaveResourcesByKey(string ResourceFile, string culter, string ResourceKey, string value, bool isUDFName, bool SaveIneTurns = false, bool ForEnterPriseSetup = false)
        {
            List<string> Ent_Cmp_Room = new List<string>();
            bool IsBaseResource = false;
            bool IsCompanyResource = false;
            List<CompanyResourcesKeyValDTO> lstCmpResDTO = null;
            List<BaseResourcesKeyValDTO> lstBaseResDTO = null;
            List<EnterpriseResourcesKeyValDTO> lstEntResDTO = null;
            bool IsEnterpriseResource = false;
            try
            {
                if (culter == "en-US")
                    culter = "";
                else
                    culter = "." + culter;

                string strFilePath = string.Empty;
                string SessinKey = string.Empty;
                Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (SaveIneTurns)
                {
                    if (isUDFName)
                    {
                        SessinKey = @"ResDB_Resources\MasterResources\RoomResources_" + ResourceFile + culter;
                        strFilePath = ResourceDirectoryBasePath + @"\MasterResources\RoomResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                        //IsCompanyResource = true;
                        IsBaseResource = true;
                    }
                    else
                    {
                        SessinKey = @"ResDB_Resources\MasterResources\EnterpriseResources_" + ResourceFile + culter;
                        strFilePath = ResourceDirectoryBasePath + @"\MasterResources\EnterpriseResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                        IsBaseResource = true;
                    }

                }
                else
                {
                    if (!ForEnterPriseSetup)
                    {
                        #region Comment
                        //if (isUDFName)
                        //{
                        //    SessinKey = "ResDB_" + RoomResourceFolder + "_" + ResourceFile + culter;
                        //    strFilePath = GetFullFileNamePathRoom(ResourceFile);
                        //}
                        //else
                        //{
                        //    SessinKey = "ResDB_" + CompanyResourceFolder + "_" + ResourceFile + culter;
                        //    strFilePath = GetFullFileNamePath(ResourceFile);
                        //} 
                        #endregion
                        SessinKey = "ResDB_" + RoomResourceFolder + "_" + ResourceFile + culter;
                        strFilePath = GetFullFileNamePathRoom(ResourceFile);
                        IsCompanyResource = true;
                    }
                    else
                    {
                        if (isUDFName)
                        {
                            strFilePath = RoomResourceDirectoryPathForEnterprise + ResourceFile + FileCulterExtension + ResourceFileExt;
                            SessinKey = "ResDB_" + RoomResourceDirectoryPathForEnterprise + "_" + ResourceFile + FileCulterExtension;
                            IsEnterpriseResource = true;
                        }
                    }
                }

                Int64 UserID = Convert.ToInt64(HttpContext.Current.Session["UserID"]);
                if (IsCompanyResource)
                {
                    #region IsCompanyResource
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    objResDbHelper.SaveCompanyResource(ResourceKey,value,ResourceFile,FileCulterExtension,Convert.ToInt64(Ent_Cmp_Room[0]),Convert.ToInt64(Ent_Cmp_Room[1]),Convert.ToInt64(Ent_Cmp_Room[2]),false,UserID,"Web");

                    if (HttpContext.Current.Cache.Get(SessinKey) != null)
                        lstCmpResDTO = (List<CompanyResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                    if (lstCmpResDTO != null)
                    {
                        lstCmpResDTO = objResDbHelper.GetCompanyResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), Convert.ToInt64(Ent_Cmp_Room[1]), Convert.ToInt64(Ent_Cmp_Room[2]), ResourceFile, FileCulterExtension);

                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, lstCmpResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }

                    if (HttpContext.Current.Cache.Get(SessinKey) != null)
                        HttpContext.Current.Cache.Remove(SessinKey);

                    #endregion
                }
                else if (IsEnterpriseResource)
                {
                    #region IsCompanyResource
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    objResDbHelper.SaveEnterpriseResource(ResourceKey, value, ResourceFile, FileCulterExtension, Convert.ToInt64(Ent_Cmp_Room[0]), UserID, "Web");

                    if (HttpContext.Current.Cache.Get(SessinKey) != null)
                        lstEntResDTO = (List<EnterpriseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                    if (lstEntResDTO != null)
                    {
                        lstEntResDTO = objResDbHelper.GetEnterpriseResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), ResourceFile, FileCulterExtension);

                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, lstCmpResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }

                    if (HttpContext.Current.Cache.Get(SessinKey) != null)
                        HttpContext.Current.Cache.Remove(SessinKey);

                    #endregion
                }
                else if (IsBaseResource)
                {
                    #region IsBaseResource
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    objResDbHelper.SaveBaseResourceDetail(ResourceKey, value, ResourceFile, FileCulterExtension, UserID, "Web");

                    if (HttpContext.Current.Cache.Get(SessinKey) != null)
                        lstBaseResDTO = (List<BaseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);


                    if (lstBaseResDTO != null)
                    {
                        lstBaseResDTO = objResDbHelper.GetBaseResourceByResFileCulture(ResourceFile, FileCulterExtension);

                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, lstBaseResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }

                    if (HttpContext.Current.Cache.Get(SessinKey) != null)
                        HttpContext.Current.Cache.Remove(SessinKey);

                    #endregion
                }
            }
            catch (Exception)
            {
                // Add exception log code here;
            }

            finally
            {
                lstCmpResDTO = null;
                lstBaseResDTO = null;
            }

            return false;
        }

        public bool SaveUserMasterResources(string ResourceFile, string culter, List<KeyValDTO> lstKeyVal, Int64 EnterpriseId)
        {
            //List<EnterpriseResourcesKeyValDTO> lstEntResDTO = null;
            List<string> Ent_Cmp_Room = new List<string>();
            if (lstKeyVal == null || lstKeyVal.Count <= 0)
                return false;
            try
            {
                if (culter == "en-US")
                    culter = "";
                else
                {
                    culter = "." + culter;
                }
                string SessinKey = "ResDB_" + EnterpriseResourceFolder + "_" + ResourceFile + culter;
                Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                //if (HttpContext.Current.Cache.Get(SessinKey) != null)
                //    lstEntResDTO = (List<EnterpriseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                Int64 UserID = Convert.ToInt64(HttpContext.Current.Session["UserID"]);
                foreach (var objKeyVal in lstKeyVal)
                {
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    objResDbHelper.SaveEnterpriseResource(objKeyVal.key, objKeyVal.value, ResourceFile, culter, Convert.ToInt64(Ent_Cmp_Room[0]), UserID, "Web");  
                }
                
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
            }
            return true;
        }

        public bool SaveEnterpriseResources(string ResourceFile, string culter, List<KeyValDTO> lstKeyVal)
        {
            //List<EnterpriseResourcesKeyValDTO> lstEntResDTO = null;
            List<string> Ent_Cmp_Room = new List<string>();
            if (lstKeyVal == null || lstKeyVal.Count <= 0)
                return false;
            try
            {
                if (culter == "en-US")
                    culter = "";
                else
                {
                    culter = "." + culter;
                }
                string SessinKey = "ResDB_" + EnterpriseResourceFolder + "_" + ResourceFile + culter;
                Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                //if (HttpContext.Current.Cache.Get(SessinKey) != null)
                //    lstEntResDTO = (List<EnterpriseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                Int64 UserID = Convert.ToInt64(HttpContext.Current.Session["UserID"]);
                foreach (var objKeyVal in lstKeyVal)
                {
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    objResDbHelper.SaveEnterpriseResource(objKeyVal.key, objKeyVal.value, ResourceFile, culter, Convert.ToInt64(Ent_Cmp_Room[0]), UserID, "Web");  
                }
                
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                //lstEntResDTO = null;
            }
            return true;
        }

        public static string GetResourceValueByKeyAndFullFilePath(string ResourceKey, string ResourceFilePath, Int64 EntID, Int64 CompanyID, Int64 RoomID, string ResourceFile, string strCulture)
        {
            CompanyResourcesKeyValDTO objCmpResDTO = null;
            string strRetResourceValue = ResourceKey;
            try
            {
                ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                objCmpResDTO = objResDbHelper.GetCompanyResourceByResFileCultureByKey(Convert.ToInt64(EntID), Convert.ToInt64(CompanyID), Convert.ToInt64(RoomID), ResourceFile, strCulture, ResourceKey);

                if (objCmpResDTO != null)
                {
                    strRetResourceValue = (objCmpResDTO == null || (string.IsNullOrWhiteSpace(objCmpResDTO.ResourceValue))) ? ResourceKey : objCmpResDTO.ResourceValue;
                }
                return strRetResourceValue; 
            }
            catch (Exception)
            {
                return ResourceKey;
            }
            finally
            {
                objCmpResDTO = null;
            }
        }
        
        private static void CreateRoomResourceFile(string strFilePath, string ResourceKey = "")
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

        public static string ColumnNameFromResource(string UDFColumnName, string UDFTableName, bool isUDFName, bool OtherFromeTurns, bool ForEnterPriseSetup)
        {
            string UDFTableResourceFileName = GetUDFResourceFromKey(UDFTableName);
            string val = string.Empty;
            val = GetResourceValue(UDFColumnName, UDFTableResourceFileName, isUDFName, OtherFromeTurns: OtherFromeTurns, ForEnterPriseSetup: ForEnterPriseSetup);
            if (!string.IsNullOrEmpty(val))
                UDFColumnName = val;

            return UDFColumnName;
        }

        private static string GetUDFResourceFromKey(string UDFTableName)
        {
            SortedList _UDFTables = GetUDFTablesResourceFile();

            int index = _UDFTables.IndexOfKey(UDFTableName);
            if (index >= 0)
                return (string)_UDFTables.GetByIndex(index);
            else
            {
                SortedList _UdfSmallList = new SortedList();
                foreach (DictionaryEntry kvp in _UDFTables)
                {
                    _UdfSmallList.Add(kvp.Key.ToString().ToLower(), kvp.Value);
                }
                index = _UdfSmallList.IndexOfKey(UDFTableName.ToLower());
                if (index >= 0)
                    return (string)_UdfSmallList.GetByIndex(index);
            }
            return string.Empty;
        }

        private static SortedList GetUDFTablesResourceFile()
        {
            SortedList _UDFTables;

            _UDFTables = new SortedList();
            _UDFTables.Add("BinMaster", "ResBin");
            _UDFTables.Add("CartItemList", "ResCartItem");
            _UDFTables.Add("CategoryMaster", "ResCategoryMaster");
            _UDFTables.Add("costuom", "ResCostUOMMaster");
            _UDFTables.Add("inventoryclassification", "ResInventoryClassificationMaster");
            _UDFTables.Add("CustomerMaster", "ResCustomer");
            _UDFTables.Add("FreightTypeMaster", "ResFreightType");
            _UDFTables.Add("GLAccountMaster", "ResGLAccount");
            _UDFTables.Add("GXPRConsigmentJobMaster", "ResGXPRConsignedJob");
            _UDFTables.Add("JobTypeMaster", "ResJobType");
            _UDFTables.Add("LocationMaster", "ResLocation");
            _UDFTables.Add("ManufacturerMaster", "ResManufacturer");
            _UDFTables.Add("MeasurementTerm", "ResMeasurementTerm");
            _UDFTables.Add("ProjectMaster", "ResProjectMaster");
            _UDFTables.Add("ShipViaMaster", "ResShipVia");
            _UDFTables.Add("SupplierBlankePOMaster", "");
            _UDFTables.Add("SupplierMaster", "ResSupplierMaster");
            _UDFTables.Add("TechnicianMaster", "ResTechnician");
            _UDFTables.Add("ToolCategoryMaster", "ResToolCategory");
            _UDFTables.Add("AssetCategoryMaster", "ResAssetCategory");
            _UDFTables.Add("ToolMaster", "ResToolMaster");
            _UDFTables.Add("UnitMaster", "ResUnitMaster");
            _UDFTables.Add("Enterprise", "ResEnterprise");
            _UDFTables.Add("CompanyMaster", "ResCompany");
            _UDFTables.Add("Room", "ResRoomMaster");
            _UDFTables.Add("QuickListMaster", "ResQuickList");
            _UDFTables.Add("ItemMaster", "ResItemMaster");
            _UDFTables.Add("PullMaster", "ResPullMaster");
            _UDFTables.Add("OrderMaster", "ResOrder");
            _UDFTables.Add("ProjectSpendItems", "ResProjectSpendItems");
            _UDFTables.Add("KitMaster", "ResKitMaster");
            _UDFTables.Add("RequisitionMaster", "ResRequisitionMaster");
            _UDFTables.Add("VenderMaster", "ResVenderMaster");
            _UDFTables.Add("AssetMaster", "ResAssetMaster");
            _UDFTables.Add("TransferMaster", "ResTransfer");
            _UDFTables.Add("WorkOrder", "ResWorkOrder");
            _UDFTables.Add("ToolsScheduler", "ResToolsScheduler");
            _UDFTables.Add("toolschedulemapping", "ResToolsSchedulerMapping");
            _UDFTables.Add("ReceivedOrderTransferDetail", "ResReceiveOrderDetails");
            _UDFTables.Add("InventoryCountList", "ResInventoryCount");
            _UDFTables.Add("InventoryCount", "ResInventoryCount");
            _UDFTables.Add("MaterialStaging", "ResMaterialStaging");
            _UDFTables.Add("ToolCheckInOutHistory", "ResToolCheckInOutHistory");
            _UDFTables.Add("ToolsMaintenanceList", "ResToolsMaintenance");
            _UDFTables.Add("OrderDetails", "ResOrderDetails");
            _UDFTables.Add("QuickListItems", "ResQuickListItems");
            _UDFTables.Add("UserMaster", "ResUserMasterUDF");

            return _UDFTables;
        }

        public static string GetModuleResource(string resourceKey, string resFileName)
        {
            string resValue = string.Empty;
            resValue = GetResourceValue(resourceKey, resFileName);
            return resValue;
        }

        public static string GetFullFileNamePathCompanyUtility(string fileName, string culter, string ResourcePath)
        {

            if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
            {
                if (culter == "en-US")
                    culter = "";
                else
                    culter = "." + culter;


                return (ResourcePath + fileName + culter + ResourceFileExt).Replace("..", ".");
            }

            return (ResourcePath + fileName + ResourceFileExt).Replace("..", ".");

        }

        public static List<KeyValDTO> GetResourceDataCompanyUtility(string ResourceFile, string culter, string ResourcePath)
        {
            XmlDocument loResource = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";

            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                if (loResource == null)
                {
                    string strFilePath = GetFullFileNamePathCompanyUtility(ResourceFile, culter, ResourcePath);

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
                            objList.Add(kv);
                        }
                    }
                }
                else
                {
                    XmlNodeList lstNodes = loResource.SelectNodes("root/data");
                    foreach (XmlNode item in lstNodes)
                    {
                        KeyValDTO kv = new KeyValDTO();
                        kv.key = item.Attributes["name"].Value;
                        kv.value = item.SelectSingleNode("value").InnerText;
                        objList.Add(kv);
                    }
                }
                return objList;
            }
            catch (Exception)
            {
                return objList;
            }

            finally
            {
                loResource = null;
            }

        }

        public static List<KeyValDTO> GetResourceDataRoomUDFUtility(string ResourceFile, string culter, string ResourcePath)
        {
            XmlDocument loResource = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";

            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                if (loResource == null)
                {
                    string strFilePath = GetFullFileNamePathCompanyUtility(ResourceFile, culter, ResourcePath);

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
                            objList.Add(kv);
                        }
                    }
                }
                else
                {
                    XmlNodeList lstNodes = loResource.SelectNodes("root/data");
                    foreach (XmlNode item in lstNodes)
                    {
                        KeyValDTO kv = new KeyValDTO();
                        kv.key = item.Attributes["name"].Value;
                        kv.value = item.SelectSingleNode("value").InnerText;
                        objList.Add(kv);
                    }
                }
                return objList;
            }
            catch (Exception)
            {
                return objList;
            }

            finally
            {
                loResource = null;
            }

        }

        public void RemoveCache(string ResourceFile, string culter, bool IsBaseResource, bool IsEntResource, bool IsCmpResource)
        {
            string SessinKey = string.Empty;

            if (culter == "en-US")
                culter = "";

            if (IsCmpResource)
            {
                SessinKey = "ResDB_" + RoomResourceFolder + "_" + ResourceFile + FileCulterExtension;
                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    HttpContext.Current.Cache.Remove(SessinKey);

               //CacheHelper<CompanyResourcesKeyValDTO>.InvalidateCache();
            }
            if (IsEntResource)
            {
                SessinKey = "ResDB_" + RoomResourceDirectoryPathForEnterprise + "_" + ResourceFile + FileCulterExtension;
                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    HttpContext.Current.Cache.Remove(SessinKey);

                SessinKey = "ResDB_" + EnterpriseResourceDirectoryPath + "_" + ResourceFile + FileCulterExtension;
                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    HttpContext.Current.Cache.Remove(SessinKey);

            }
            
        }

        //public void RemoveCache(Int64 ResourcePageID, Int64 LanguageID)
        //{

        //}

        #endregion
        
    }

    public class EnterPriseResourceModuleHelper
    {
        private const string ResourceFileExt = ".resx";
        private const int ResouceCacheMaxTime = 104400;

        #region "Properties"
        public static CultureInfo CurrentCult
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session["CurrentCultENT"] != null)
                    return (CultureInfo)HttpContext.Current.Session["CurrentCultENT"];

                return new CultureInfo("en-US");
            }
            set
            {
                HttpContext.Current.Session["CurrentCultENT"] = value;
            }
        }

        public static string FileCulterExtension
        {
            get
            {
                if (CurrentCult.ToString().Contains("en-US"))
                    return "";

                return "." + CurrentCult.ToString();
            }
        }

        public static string EnterpriseResourceFolder
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["EnterpriseResourceFolderENT"]);
            }
            set
            {
                HttpContext.Current.Session["EnterpriseResourceFolderENT"] = value;
            }
        }

        public static string CompanyResourceFolder
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Session["CompanyResourceFolderENT"]);
            }
            set
            {
                HttpContext.Current.Session["CompanyResourceFolderENT"] = value;
            }
        }

        public static string RoomResourceFolder
        {
            get
            {
                string entCompDir = Convert.ToString(HttpContext.Current.Session["RoomResourceFolder"]);
                if (!string.IsNullOrEmpty(entCompDir))
                {
                    string fldr = ResourceDirectoryBasePath + "\\" + HttpContext.Current.Session["RoomResourceFolder"];
                    if (!Directory.Exists(fldr))
                    {
                        Directory.CreateDirectory(fldr);
                        //entCompDir = CompanyResourceFolder;
                    }
                }
                return entCompDir;
            }
            set
            {
                HttpContext.Current.Session["RoomResourceFolder"] = value;
            }
        }
        public static string ResourceDirectoryBasePath
        {
            get
            {
                //return HttpContext.Current.Server.MapPath(@"\Resources");
                return System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            }
        }
        public static string RoomResourceDirectoryPath
        {
            get
            {
                return ResourceDirectoryBasePath + @"\" + RoomResourceFolder + @"\";
            }
        }
        public static string CompanyResourceDirectoryPath
        {
            get
            {
                return ResourceDirectoryBasePath + @"\" + CompanyResourceFolder + @"\";
            }
        }


        public static string EnterpriseResourceDirectoryPath
        {
            get
            {
                return ResourceDirectoryBasePath + @"\" + EnterpriseResourceFolder + @"\EnterpriseResources\";
            }
        }

        public static DateTime CacheExpiryTime
        {
            get
            {
                return DateTime.Now.AddSeconds(ResouceCacheMaxTime);
            }
        }
        #endregion

        #region Functions
        public static string GetResourceValue(string ResourceKey, string ResourceFile)
        {
            List<EnterpriseResourcesKeyValDTO> lstEntResDTO = null;
            EnterpriseResourcesKeyValDTO objEntResDTO = null;
            string strRetResourceValue = ResourceKey;
            try
            {
                List<string> Ent_Cmp_Room = new List<string>();
                string strFilePath = GetFullFileNamePath(ResourceFile);
                string SessinKey = "ResDB_" + CompanyResourceFolder + "_" + ResourceFile + FileCulterExtension;
                // this is enterprise company resource
                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstEntResDTO = (List<EnterpriseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstEntResDTO == null)
                {
                    lstEntResDTO = new List<EnterpriseResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    lstEntResDTO = objResDbHelper.GetEnterpriseResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), ResourceFile, FileCulterExtension);

                    CacheDependency cacheDep = new CacheDependency(strFilePath);
                    HttpContext.Current.Cache.Add(SessinKey, lstEntResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                    if (lstEntResDTO != null && lstEntResDTO.Count > 0)
                    {
                        objEntResDTO = lstEntResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                        strRetResourceValue = (objEntResDTO == null || (string.IsNullOrWhiteSpace(objEntResDTO.ResourceValue))) ? ResourceKey : objEntResDTO.ResourceValue;
                    }
                }
                else
                {
                    if (lstEntResDTO != null && lstEntResDTO.Count > 0)
                    {
                        objEntResDTO = lstEntResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                        strRetResourceValue = (objEntResDTO == null || (string.IsNullOrWhiteSpace(objEntResDTO.ResourceValue))) ? ResourceKey : objEntResDTO.ResourceValue;
                    }
                }
                return strRetResourceValue; 
            }
            catch (Exception)
            {
                //throw ex;
                return ResourceKey;
                // Add exception log code here;
            }

            finally
            {
                lstEntResDTO = null;
                objEntResDTO = null;
            }
        }

        public static string GetEnterPriseResourceValue(string ResourceKey, string ResourceFile)
        {
            //XmlDocument loResource = null;
            //XmlNode loRoot = null;
            List<EnterpriseResourcesKeyValDTO> lstEntResDTO = null;
            EnterpriseResourcesKeyValDTO objEntResDTO = null;
            string strRetResourceValue = ResourceKey;
            try
            {
                List<string> Ent_Cmp_Room = new List<string>();
                string strFilePath = EnterpriseResourceDirectoryPath + ResourceFile + FileCulterExtension + ResourceFileExt; ;
                string SessinKey = "ResDB_" + EnterpriseResourceFolder + "_" + ResourceFile + FileCulterExtension;
                // this is Enterprise resource
                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstEntResDTO = (List<EnterpriseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstEntResDTO == null)
                {
                    lstEntResDTO = new List<EnterpriseResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    lstEntResDTO = objResDbHelper.GetEnterpriseResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), ResourceFile, FileCulterExtension);

                    CacheDependency cacheDep = new CacheDependency(strFilePath);
                    HttpContext.Current.Cache.Add(SessinKey, lstEntResDTO, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

                    if (lstEntResDTO != null && lstEntResDTO.Count > 0)
                    {
                        objEntResDTO = lstEntResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                        strRetResourceValue = (objEntResDTO == null || (string.IsNullOrWhiteSpace(objEntResDTO.ResourceValue))) ? ResourceKey : objEntResDTO.ResourceValue;
                    }
                }
                else
                {
                    if (lstEntResDTO != null && lstEntResDTO.Count > 0)
                    {
                        objEntResDTO = lstEntResDTO.Where(x => x.ResourceKey == ResourceKey).FirstOrDefault();
                        strRetResourceValue = (objEntResDTO == null || (string.IsNullOrWhiteSpace(objEntResDTO.ResourceValue))) ? ResourceKey : objEntResDTO.ResourceValue;
                    }
                }
                return strRetResourceValue; 
            }
            catch (Exception)
            {
                //throw ex;
                return ResourceKey;
                // Add exception log code here;
            }

            finally
            {
                lstEntResDTO = null;
                objEntResDTO = null;
            }
        }

        public static string GetFullFileNamePath(string fileName)
        {
            return CompanyResourceDirectoryPath + fileName + FileCulterExtension + ResourceFileExt;
        }
        public static string GetFullFileNamePathRoom(string fileName)
        {
            return RoomResourceDirectoryPath + fileName + FileCulterExtension + ResourceFileExt;
        }
        public static string GetFullFileNamePathRoom(string fileName, string culter)
        {

            if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
            {
                if (culter == "en-US")
                    culter = "";
                else
                    culter = "." + culter;


                return (RoomResourceDirectoryPath + fileName + culter + ResourceFileExt).Replace("..", ".");
            }

            return (RoomResourceDirectoryPath + fileName + ResourceFileExt).Replace("..", ".");

        }
        public static string GetFullFileNamePath(string fileName, string culter)
        {

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
        public static string GetFullFileNamePathENTUtility(string fileName, string culter, string ResourcePath)
        {

            if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
            {
                if (culter == "en-US")
                    culter = "";
                else
                    culter = "." + culter;


                return (ResourcePath + fileName + culter + ResourceFileExt).Replace("..", ".");
            }

            return (ResourcePath + fileName + ResourceFileExt).Replace("..", ".");

        }
        public static List<KeyValDTO> GetResourceData(string ResourceFile, string culter)
        {
            List<CompanyResourcesKeyValDTO> lstCmpResDTO = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            
            try
            {
                List<string> Ent_Cmp_Room = new List<string>();
                Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                string SessinKey = "ResDB_" + "CompanyResources" + "_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstCmpResDTO = (List<CompanyResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstCmpResDTO == null)
                {
                    string strFilePath = GetFullFileNamePath(ResourceFile, culter);
                    // read from company resource

                    lstCmpResDTO = new List<CompanyResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    lstCmpResDTO = objResDbHelper.GetCompanyResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), Convert.ToInt64(Ent_Cmp_Room[1]), Convert.ToInt64(Ent_Cmp_Room[2]), ResourceFile, culter);

                    if (lstCmpResDTO != null && lstCmpResDTO.Count > 0)
                    {
                        foreach (CompanyResourcesKeyValDTO objCmpDTO in lstCmpResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objCmpDTO.ResourceKey;
                            kv.value = objCmpDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                else
                {
                    if (lstCmpResDTO != null && lstCmpResDTO.Count > 0)
                    {
                        foreach (CompanyResourcesKeyValDTO objCmpDTO in lstCmpResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objCmpDTO.ResourceKey;
                            kv.value = objCmpDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                return objList;
            }
            catch (Exception)
            {
                return objList;
                // Add exception log code here;
            }

            finally
            {
                lstCmpResDTO = null;
            }

        }
        public static List<KeyValDTO> GetResourceDataENTUtility(string ResourceFile, string culter, string ResourcePath)
        {
            XmlDocument loResource = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                string SessinKey = "Res_" + "CompanyResources" + "_" + ResourceFile + sessionculter;

                //if (HttpContext.Current.Cache.Get(SessinKey) != null)
                //    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    string strFilePath = GetFullFileNamePathENTUtility(ResourceFile, culter, ResourcePath);

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
                            objList.Add(kv);
                        }
                    }
                }
                else
                {
                    XmlNodeList lstNodes = loResource.SelectNodes("root/data");
                    foreach (XmlNode item in lstNodes)
                    {
                        KeyValDTO kv = new KeyValDTO();
                        kv.key = item.Attributes["name"].Value;
                        kv.value = item.SelectSingleNode("value").InnerText;
                        objList.Add(kv);
                    }
                }


                return objList;
            }
            catch (Exception)
            {
                return objList;
                // Add exception log code here;
            }

            finally
            {
                loResource = null;
            }

        }

        public static List<KeyValDTO> GetResourceData(string ResourceFile, string culter, string SearchText)
        {
            List<CompanyResourcesKeyValDTO> lstCmpResDTO = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            List<string> Ent_Cmp_Room = new List<string>();
            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                string SessinKey = "ResDB_" + CompanyResourceFolder + "_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstCmpResDTO = (List<CompanyResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstCmpResDTO == null)
                {
                    string strFilePath = GetFullFileNamePath(ResourceFile, culter);

                    lstCmpResDTO = new List<CompanyResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    lstCmpResDTO = objResDbHelper.GetCompanyResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), Convert.ToInt64(Ent_Cmp_Room[1]), Convert.ToInt64(Ent_Cmp_Room[2]), ResourceFile, culter);

                    if (lstCmpResDTO != null && lstCmpResDTO.Count > 0)
                    {
                        lstCmpResDTO = lstCmpResDTO.Where(x => x.ResourceKey.ToLower() == SearchText.ToLower() || x.ResourceValue.ToLower() == SearchText.ToLower()).ToList();
                        foreach (CompanyResourcesKeyValDTO objCmpDTO in lstCmpResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objCmpDTO.ResourceKey;
                            kv.value = objCmpDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                else
                {
                    if (lstCmpResDTO != null && lstCmpResDTO.Count > 0)
                    {
                        lstCmpResDTO = lstCmpResDTO.Where(x => x.ResourceKey.ToLower() == SearchText.ToLower() || x.ResourceValue.ToLower() == SearchText.ToLower()).ToList();
                        foreach (CompanyResourcesKeyValDTO objCmpDTO in lstCmpResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objCmpDTO.ResourceKey;
                            kv.value = objCmpDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                return objList;
            }
            catch (Exception)
            {
                return objList;
                // Add exception log code here;
            }

            finally
            {
                lstCmpResDTO = null;
            }

        }
        
        public static List<KeyValDTO> GetEnterpriseResourceData(string ResourceFile, string culter)
        {
            List<EnterpriseResourcesKeyValDTO> lstEntResDTO = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            try
            {
                List<string> Ent_Cmp_Room = new List<string>();
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                string SessinKey = "ResDB_" + EnterpriseResourceFolder + "_" + ResourceFile + sessionculter;
                
                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    lstEntResDTO = (List<EnterpriseResourcesKeyValDTO>)HttpContext.Current.Cache.Get(SessinKey);

                if (lstEntResDTO == null)
                {
                    //string strFilePath = string.Empty; //GetFullFileNamePath(ResourceFile, culter);
                    if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
                    {
                        if (culter == "en-US")
                            culter = "";
                        else
                            culter = "." + culter;

                        //strFilePath = (EnterpriseResourceDirectoryPath + ResourceFile + culter + ResourceFileExt).Replace("..", ".");
                    }
                    //else
                    //{
                    //    strFilePath = (EnterpriseResourceDirectoryPath + ResourceFile + ResourceFileExt).Replace("..", ".");
                    //}

                    lstEntResDTO = new List<EnterpriseResourcesKeyValDTO>();
                    ResourceDBHelper objResDbHelper = new ResourceDBHelper();
                    Ent_Cmp_Room = RoomResourceFolder.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    lstEntResDTO = objResDbHelper.GetEnterpriseResourceByResFileCulture(Convert.ToInt64(Ent_Cmp_Room[0]), ResourceFile, culter);

                    if (lstEntResDTO != null && lstEntResDTO.Count > 0)
                    {
                        foreach (EnterpriseResourcesKeyValDTO objEntDTO in lstEntResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objEntDTO.ResourceKey;
                            kv.value = objEntDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                else
                {
                    if (lstEntResDTO != null && lstEntResDTO.Count > 0)
                    {
                        foreach (EnterpriseResourcesKeyValDTO objEntDTO in lstEntResDTO)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = objEntDTO.ResourceKey;
                            kv.value = objEntDTO.ResourceValue;
                            objList.Add(kv);
                        }
                    }
                }
                return objList;
            }
            catch (Exception)
            {
                return objList;
                // Add exception log code here;
            }

            finally
            {
                lstEntResDTO = null;
            }

        }
        
        public static List<KeyValDTO> GetEnterpriseResourceDataUtility(string ResourceFile, string culter, string EnterpriseResourceDirectoryPath)
        {
            XmlDocument loResource = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                //string SessinKey = "Res_" + EnterpriseResourceFolder + "_" + ResourceFile + sessionculter;

                //if (HttpContext.Current.Cache.Get(SessinKey) != null)
                //    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    string strFilePath = string.Empty; //GetFullFileNamePath(ResourceFile, culter);
                    if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
                    {
                        if (culter == "en-US")
                            culter = "";
                        else
                            culter = "." + culter;


                        strFilePath = (EnterpriseResourceDirectoryPath + ResourceFile + culter + ResourceFileExt).Replace("..", ".");
                    }
                    else
                    {
                        strFilePath = (EnterpriseResourceDirectoryPath + ResourceFile + ResourceFileExt).Replace("..", ".");
                    }

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
                            objList.Add(kv);
                        }
                    }
                }
                else
                {
                    XmlNodeList lstNodes = loResource.SelectNodes("root/data");
                    foreach (XmlNode item in lstNodes)
                    {
                        KeyValDTO kv = new KeyValDTO();
                        kv.key = item.Attributes["name"].Value;
                        kv.value = item.SelectSingleNode("value").InnerText;
                        objList.Add(kv);
                    }
                }


                return objList;
            }
            catch (Exception)
            {
                return objList;
                // Add exception log code here;
            }

            finally
            {
                loResource = null;
            }

        }
         
        /*public static string GetResourceValueByKeyAndFullFilePath(string ResourceKey, string ResourceFilePath)
        {
            XmlDocument loResource = null;
            XmlNode loRoot = null;
            try
            {

                loResource = new XmlDocument();
                if (System.IO.File.Exists(ResourceFilePath))
                {
                    loResource.Load(ResourceFilePath);
                }
                else
                    return ResourceKey;


                loRoot = loResource.SelectSingleNode("root/data[translate(name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + ResourceKey.ToLower() + "']/value");

                return (loRoot == null || (string.IsNullOrWhiteSpace(loRoot.InnerText))) ? ResourceKey : loRoot.InnerText;
            }
            catch (Exception)
            {
                return ResourceKey;
            }

            finally
            {
                loResource = null;
                loRoot = null;
            }
        }*/

        #endregion
    }

    public class ResourceRead
    {
        public static string GetResourceValue(string ResourceKey, string ResourceFile, bool isUDFName = false, bool IsFromWeb = true, bool OtherFromeTurns = true, bool ForEnterPriseSetup = false)
        {
            int ResRead = (int)ResourceReadType.File;
            if (SiteSettingHelper.ResourceRead != string.Empty)
                ResRead = Convert.ToInt32(SiteSettingHelper.ResourceRead);

            if (ResRead == (int)ResourceReadType.File)
            {
                return ResourceHelper.GetResourceValue(ResourceKey, ResourceFile, isUDFName, IsFromWeb, OtherFromeTurns, ForEnterPriseSetup);
            }
            else
            {
                return ResourceModuleHelper.GetResourceValue(ResourceKey, ResourceFile, isUDFName, IsFromWeb, OtherFromeTurns, ForEnterPriseSetup);
            }
        }

        public static string GetEnterPriseResourceValue(string ResourceKey, string ResourceFile)
        {
            int ResRead = (int)ResourceReadType.File;
            if (SiteSettingHelper.ResourceRead != string.Empty)
                ResRead = Convert.ToInt32(SiteSettingHelper.ResourceRead);

            if (ResRead == (int)ResourceReadType.File)
            {
                return ResourceHelper.GetEnterPriseResourceValue(ResourceKey, ResourceFile);
            }
            else
            {
                return ResourceModuleHelper.GetEnterPriseResourceValue(ResourceKey, ResourceFile);
            }
        }

        public static string GetResourceValueByKeyAndFullFilePath(string ResourceKey, string ResourceFilePath, Int64 EntID = 0, Int64 CompanyID = 0, Int64 RoomID = 0, string ResFile = "", string strCulture = "")
        {
            int ResRead = (int)ResourceReadType.File;
            if (SiteSettingHelper.ResourceRead != string.Empty)
                ResRead = Convert.ToInt32(SiteSettingHelper.ResourceRead);

            if (ResRead == (int)ResourceReadType.File)
            {
                return ResourceHelper.GetResourceValueByKeyAndFullFilePath(ResourceKey, ResourceFilePath);
            }
            else
            {
                return ResourceModuleHelper.GetResourceValueByKeyAndFullFilePath(ResourceKey, ResourceFilePath, EntID, CompanyID, RoomID, ResFile, strCulture);
            }

        }

        public static string GetResourceFileFullPath(string ResourceBaseFilePath,string fileName, string Culture, long EntID, long CompanyID)
        {
            string path = ResourceBaseFilePath;
            path += "\\" + EntID + "\\" + CompanyID + "\\" + fileName;
            if (Culture != "en-US")
            {
                path += "." + Culture;
            }
            path += ".resx";

            return path;
        }

        public static string GetEnterPriseWiseCompanyResourceFileFullPath(string ResourceBaseFilePath, string fileName, string Culture, long EntID)
        {
            string path = ResourceBaseFilePath;
            path += "\\" + EntID + "\\CompanyResources\\" + fileName;
            if (Culture != "en-US")
            {
                path += "." + Culture;
            }
            path += ".resx";

            return path;
        }

        public static string GetCompanyResourceBaseFileFullPath(string ResourceBaseFilePath,string fileName, string Culture)
        {
            string path = ResourceBaseFilePath; //D:\WebSites\eTurns4040\Resources\MasterResources\CompanyResources
            path += "\\MasterResources\\CompanyResources" + "\\" + fileName;
            if (Culture != "en-US")
            {
                path += "." + Culture;
            }
            path += ".resx";

            return path;
        }
        public static string GetEnterPriseResourceBaseFileFullPath(string ResourceBaseFilePath, string fileName, string Culture)
        {
            string path = ResourceBaseFilePath; //D:\WebSites\eTurns4040\Resources\MasterResources\EnterpriseResources
            path += "\\MasterResources\\EnterpriseResources" + "\\" + fileName;
            if (Culture != "en-US")
            {
                path += "." + Culture;
            }
            path += ".resx";

            return path;
        }
    }

}
