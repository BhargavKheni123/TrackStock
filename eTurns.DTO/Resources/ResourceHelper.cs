using eTurns.DTO.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Linq;

namespace eTurns.DTO.Resources
{

    public class BaseResourceHelper
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
            XmlDocument loResource = null;
            XmlNode loRoot = null;
            try
            {
                string strFilePath = GetFullFileNamePath(ResourceFile);
                string SessinKey = "Res_BaseCompanyResource_" + ResourceFile + FileCulterExtension;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                    {
                        loResource.Load(strFilePath);
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else if (!string.IsNullOrEmpty(FileCulterExtension) && FileCulterExtension.ToLower() != "en-us") // This logic is for if no other culter file exist then default load english culter
                    {
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        strFilePath = strFilePath.Replace(FileCulterExtension, "");
                        if (System.IO.File.Exists(strFilePath))
                            loResource.Load(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else
                        return ResourceKey;
                }

                loRoot = loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + ResourceKey.ToLower() + "']/value");

                return (loRoot == null || (string.IsNullOrWhiteSpace(loRoot.InnerText))) ? ResourceKey : loRoot.InnerText;
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

        public static string GetBaseEnterPriseResourceValue(string ResourceKey, string ResourceFile)
        {
            XmlDocument loResource = null;
            XmlNode loRoot = null;
            try
            {
                string strFilePath = BaseEnterpriseResourceDirectoryPath + ResourceFile + FileCulterExtension + ResourceFileExt; ;
                string SessinKey = "Res_BaseEnterpriseResource_" + ResourceFile + FileCulterExtension;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                    {
                        loResource.Load(strFilePath);
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else if (!string.IsNullOrEmpty(FileCulterExtension) && FileCulterExtension.ToLower() != "en-us") // This logic is for if no other culter file exist then default load english culter
                    {
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        strFilePath = strFilePath.Replace(FileCulterExtension, "");
                        if (System.IO.File.Exists(strFilePath))
                            loResource.Load(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else
                        return ResourceKey;
                }

                loRoot = loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + ResourceKey.ToLower() + "']/value");

                return (loRoot == null || (string.IsNullOrWhiteSpace(loRoot.InnerText))) ? ResourceKey : loRoot.InnerText;
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

        public static List<KeyValDTO> GetBaseEnterpriseResourceData(string ResourceFile, string culter)
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

                string SessinKey = "Res_BaseEnterpriseResource_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    string strFilePath = string.Empty; //GetFullFileNamePath(ResourceFile, culter);
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

        public static List<KeyValDTO> GetBaseResourceData(string ResourceFile, string culter)
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

                string SessinKey = "Res_BaseCompanyResource_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    string strFilePath = GetFullFileNamePath(ResourceFile, culter);

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

        public bool SaveBaseResources(string ResourceFile, string culter, List<KeyValDTO> lstKeyVal, bool IsaddedinBase = false)
        {
            XmlDocument loResource = null;
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
                string SessinKey = "Res_BaseCompanyResource_" + ResourceFile + culter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                string strFilePath = GetFullFileNamePath(ResourceFile, culter);

                if (loResource == null)
                {

                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                        loResource.Load(strFilePath);
                }
                foreach (var item in lstKeyVal)
                {
                    if (loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + item.key.ToLower() + "']") != null)
                    {
                        loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + item.key.ToLower() + "']/value").InnerText = item.value;
                    }
                    else if (culter.Length > 0 || IsaddedinBase)
                    {
                        //TODO: do not delete below commneted code;

                        XmlNode child = null;
                        XmlNode xname = null;
                        XmlNode rootnode = null;

                        rootnode = loResource.SelectSingleNode("root");
                        if (rootnode == null)
                        {
                            //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                            string NewBaseResourceFileName = SiteSettingHelper.NewBaseResourceFileName; // Settinfile.Element("NewBaseResourceFileName").Value;
                            string strFileBasePath = GetFullFileNamePath(NewBaseResourceFileName, "en-US");

                            string strFilePathDefault = GetFullFileNamePath(ResourceFile, "en-US");
                            if (!System.IO.File.Exists(strFilePathDefault))
                                File.Copy(strFileBasePath, strFilePathDefault);

                            File.Copy(strFileBasePath, strFilePath);
                            loResource = new XmlDocument();
                            if (System.IO.File.Exists(strFilePath))
                                loResource.Load(strFilePath);
                            rootnode = loResource.SelectSingleNode("root");
                        }
                        xname = loResource.CreateNode(XmlNodeType.Element, "data", null);
                        XmlAttribute xa = loResource.CreateAttribute("name");
                        xa.Value = item.key;
                        XmlAttribute xa1 = loResource.CreateAttribute("xml", "space", "xml");
                        xa1.Value = "preserve";
                        xname.Attributes.Append(xa);
                        xname.Attributes.Append(xa1);
                        child = loResource.CreateNode(XmlNodeType.Element, "value", null);
                        child.InnerText = item.value;
                        xname.AppendChild(child);
                        rootnode.AppendChild(xname);
                    }
                }

                loResource.Save(strFilePath);
            }
            catch
            {
                return false;
                // Add exception log code here;
            }

            finally
            {
                loResource = null;
            }

            return true;
        }

        public bool SaveBaseEnterpriseResources(string ResourceFile, string culter, List<KeyValDTO> lstKeyVal, bool IsaddedinBase = false)
        {
            XmlDocument loResource = null;
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
                string SessinKey = "Res_BaseEnterpriseResource_" + ResourceFile + culter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                //string strFilePath = GetFullFileNamePath(ResourceFile, culter);
                string strFilePath = string.Empty; //GetFullFileNamePath(ResourceFile, culter);
                if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
                {
                    strFilePath = (BaseEnterpriseResourceDirectoryPath + ResourceFile + culter + ResourceFileExt).Replace("..", ".");
                }
                else
                {
                    strFilePath = (BaseEnterpriseResourceDirectoryPath + ResourceFile + ResourceFileExt).Replace("..", ".");
                }

                if (loResource == null)
                {

                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                        loResource.Load(strFilePath);
                }
                foreach (var item in lstKeyVal)
                {
                    //loResource.SelectSingleNode("root/data[@name='" + item.key + "']/value").InnerText = item.value;
                    if (loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + item.key.ToLower() + "']") != null)
                    {
                        loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + item.key.ToLower() + "']/value").InnerText = item.value;
                    }
                    else if (culter.Length > 0 || IsaddedinBase)
                    {
                        //TODO: do not delete below commneted code;

                        XmlNode child = null;
                        XmlNode xname = null;
                        XmlNode rootnode = null;

                        rootnode = loResource.SelectSingleNode("root");
                        xname = loResource.CreateNode(XmlNodeType.Element, "data", null);
                        XmlAttribute xa = loResource.CreateAttribute("name");
                        xa.Value = item.key;
                        XmlAttribute xa1 = loResource.CreateAttribute("xml", "space", "xml");
                        xa1.Value = "preserve";
                        xname.Attributes.Append(xa);
                        xname.Attributes.Append(xa1);
                        child = loResource.CreateNode(XmlNodeType.Element, "value", null);
                        child.InnerText = item.value;
                        xname.AppendChild(child);
                        rootnode.AppendChild(xname);
                    }
                }
                loResource.Save(strFilePath);
            }
            catch (Exception)
            {
                return false;
                // Add exception log code here;
            }

            finally
            {
                loResource = null;
            }

            return true;
        }

        public static string GetFullFileNamePathUtility(string fileName, string culter, string ResourcePath)
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

        public static List<KeyValDTO> GetBaseResourceRoomDataUtility(string ResourceFile, string culter, string ResourcePath)
        {
            XmlDocument loResource = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            try
            {
                if (loResource == null)
                {
                    string strFilePath = GetFullFileNamePathUtility(ResourceFile, culter, ResourcePath);

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

        #endregion
    }

    public class ResourceHelper
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
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
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
                return System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            }
        }
        public static string RDLReportDirectoryBasePath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["RDLCBaseFilePath"];
                //return HttpContext.Current.Server.MapPath(@"\RDLC_Reports");
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
            XmlDocument loResource = null;
            XmlNode loRoot = null;
            try
            {
                string strFilePath = string.Empty;
                string SessinKey = string.Empty;
                if (IsFromWeb)
                {
                    if (OtherFromeTurns)
                    {
                        if (!ForEnterPriseSetup)
                        {
                            if (isUDFName)
                            {
                                strFilePath = GetFullFileNamePathRoom(ResourceFile);
                                SessinKey = "Res_" + RoomResourceFolder + "_" + ResourceFile + FileCulterExtension;

                            }
                            else
                            {
                                strFilePath = GetFullFileNamePath(ResourceFile);
                                SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + FileCulterExtension;

                            }
                        }
                        else
                        {
                            if (isUDFName)
                            {
                                strFilePath = RoomResourceDirectoryPathForEnterprise + ResourceFile + FileCulterExtension + ResourceFileExt;
                                SessinKey = "Res_" + RoomResourceDirectoryPathForEnterprise + "_" + ResourceFile + FileCulterExtension;

                            }
                            else
                            {
                                if (ResourceFile.ToLower() == "resusermasterudf" || ResourceKey.ToLower().Contains("createdbyudf") || ResourceKey.ToLower().Contains("updatedbyudf") || ResourceKey.ToLower().Contains("createdby_udf") || ResourceKey.ToLower().Contains("updatedby_udf"))
                                {
                                    strFilePath = EnterpriseResourceDirectoryPath + ResourceFile + FileCulterExtension + ResourceFileExt;
                                    SessinKey = "Res_" + EnterpriseResourceDirectoryPath + "_" + ResourceFile + FileCulterExtension;

                                }
                            }

                        }
                    }
                    else
                    {
                        if (isUDFName)
                        {
                            strFilePath = strFilePath = ResourceDirectoryBasePath + @"\MasterResources\RoomResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                            SessinKey = @"Res_Resources\MasterResources\RoomResources_" + ResourceFile + FileCulterExtension;
                        }
                        else
                        {
                            SessinKey = @"Res_Resources\MasterResources\EnterpriseResources_" + ResourceFile + FileCulterExtension;
                            strFilePath = ResourceDirectoryBasePath + @"\MasterResources\EnterpriseResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                        }
                        // HttpContext.Current.Cache.Remove(SessinKey);
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
                                SessinKey = "Res_" + RoomResourceFolder + "_" + ResourceFile + FileCulterExtension;

                            }
                            else
                            {
                                strFilePath = ResourceDirectoryBasePath + @"\" + RoomResourceFolder + @"\" + ResourceFile + ".resx";
                                SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + FileCulterExtension;

                            }
                        }
                        else
                        {
                            if (isUDFName)
                            {
                                strFilePath = RoomResourceDirectoryPathForEnterprise + ResourceFile + FileCulterExtension + ResourceFileExt;
                                SessinKey = "Res_" + RoomResourceDirectoryPathForEnterprise + "_" + ResourceFile + FileCulterExtension;

                            }

                        }
                    }
                    else
                    {
                        if (isUDFName)
                        {
                            strFilePath = strFilePath = ResourceDirectoryBasePath + @"\MasterResources\RoomResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                            SessinKey = @"Res_Resources\MasterResources\RoomResources_" + ResourceFile + FileCulterExtension;
                        }
                        else
                        {
                            SessinKey = @"Res_Resources\MasterResources\EnterpriseResources_" + ResourceFile + FileCulterExtension;
                            strFilePath = ResourceDirectoryBasePath + @"\MasterResources\EnterpriseResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                        }

                    }
                }
                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                    {
                        loResource.Load(strFilePath);
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else if (!string.IsNullOrEmpty(FileCulterExtension) && FileCulterExtension.ToLower() != "en-us") // This logic is for if no other culter file exist then default load english culter
                    {
                        strFilePath = strFilePath.Replace(FileCulterExtension, "");
                        if (System.IO.File.Exists(strFilePath))
                            loResource.Load(strFilePath);
                        else if (isUDFName)
                        {
                            CreateRoomResourceFile(strFilePath, ResourceKey);
                            loResource.Load(strFilePath);
                        }
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else
                    {
                        if (isUDFName || ResourceFile.ToLower() == "resusermasterudf" || ResourceKey.ToLower().Contains("createdbyudf") || ResourceKey.ToLower().Contains("updatedbyudf") || ResourceKey.ToLower().Contains("createdby_udf") || ResourceKey.ToLower().Contains("updatedby_udf"))
                        {
                            CreateRoomResourceFile(strFilePath, ResourceKey);
                            loResource.Load(strFilePath);
                            CacheDependency cacheDep = new CacheDependency(strFilePath);
                            HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                        }
                        else
                            return ResourceKey;
                    }
                }
                //PullUDF1-->PULLUDF1

                //loRoot = loResource.SelectSingleNode("root/data[@name='" + ResourceKey + "']/value");

                loRoot = loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + ResourceKey.ToLower() + "']/value");



                return (loRoot == null || (string.IsNullOrWhiteSpace(loRoot.InnerText))) ? ResourceKey : loRoot.InnerText;
            }
            catch
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
        public static string GetUserMasterResourceValue(string ResourceKey, string ResourceFile, bool isUDFName = false, bool IsFromWeb = true, bool OtherFromeTurns = true, bool ForEnterPriseSetup = false, Int64 EnterpriseID = 0)
        {
            XmlDocument loResource = null;
            XmlNode loRoot = null;
            try
            {
                string strFilePath = string.Empty;
                string SessinKey = string.Empty;
                if (IsFromWeb)
                {
                    if (OtherFromeTurns)
                    {
                        if (!ForEnterPriseSetup)
                        {
                            if (isUDFName)
                            {
                                strFilePath = GetFullFileNamePathRoom(ResourceFile);
                                SessinKey = "Res_" + RoomResourceFolder + "_" + ResourceFile + FileCulterExtension;

                            }
                            else
                            {
                                strFilePath = GetFullFileNamePath(ResourceFile);
                                SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + FileCulterExtension;

                            }
                        }
                        else
                        {
                            if (isUDFName)
                            {
                                strFilePath = RoomResourceDirectoryPathForEnterprise + ResourceFile + FileCulterExtension + ResourceFileExt;
                                SessinKey = "Res_" + RoomResourceDirectoryPathForEnterprise + "_" + ResourceFile + FileCulterExtension;

                            }

                        }
                    }
                    else
                    {
                        if (isUDFName)
                        {
                            strFilePath = strFilePath = ResourceDirectoryBasePath + @"\MasterResources\RoomResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                            SessinKey = @"Res_Resources\MasterResources\RoomResources_" + ResourceFile + FileCulterExtension;
                        }
                        else
                        {
                            SessinKey = @"Res_Resources\" + EnterpriseID + @"\EnterpriseResources_" + ResourceFile + FileCulterExtension;
                            strFilePath = ResourceDirectoryBasePath + @"\" + EnterpriseID + @"\EnterpriseResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                        }
                        // HttpContext.Current.Cache.Remove(SessinKey);
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
                                SessinKey = "Res_" + RoomResourceFolder + "_" + ResourceFile + FileCulterExtension;

                            }
                            else
                            {
                                strFilePath = ResourceDirectoryBasePath + @"\" + RoomResourceFolder + @"\" + ResourceFile + ".resx";
                                SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + FileCulterExtension;

                            }
                        }
                        else
                        {
                            if (isUDFName)
                            {
                                strFilePath = RoomResourceDirectoryPathForEnterprise + ResourceFile + FileCulterExtension + ResourceFileExt;
                                SessinKey = "Res_" + RoomResourceDirectoryPathForEnterprise + "_" + ResourceFile + FileCulterExtension;

                            }

                        }
                    }
                    else
                    {
                        if (isUDFName)
                        {
                            strFilePath = strFilePath = ResourceDirectoryBasePath + @"\MasterResources\RoomResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                            SessinKey = @"Res_Resources\MasterResources\RoomResources_" + ResourceFile + FileCulterExtension;
                        }
                        else
                        {
                            SessinKey = @"Res_Resources\" + EnterpriseID + @"\EnterpriseResources_" + ResourceFile + FileCulterExtension;
                            strFilePath = ResourceDirectoryBasePath + @"\" + EnterpriseID + @"\EnterpriseResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                        }

                    }
                }
                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                    {
                        loResource.Load(strFilePath);
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else if (!string.IsNullOrEmpty(FileCulterExtension) && FileCulterExtension.ToLower() != "en-us") // This logic is for if no other culter file exist then default load english culter
                    {
                        strFilePath = strFilePath.Replace(FileCulterExtension, "");
                        if (System.IO.File.Exists(strFilePath))
                            loResource.Load(strFilePath);
                        else if (isUDFName)
                        {
                            CreateRoomResourceFile(strFilePath, ResourceKey);
                            loResource.Load(strFilePath);
                        }
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else
                    {
                        if (isUDFName || ResourceFile.ToLower() == "resusermasterudf" || ResourceKey.ToLower().Contains("createdbyudf") || ResourceKey.ToLower().Contains("updatedbyudf") || ResourceKey.ToLower().Contains("createdby_udf") || ResourceKey.ToLower().Contains("updatedby_udf"))
                        {
                            CreateRoomResourceFile(strFilePath, ResourceKey);
                            loResource.Load(strFilePath);
                            CacheDependency cacheDep = new CacheDependency(strFilePath);
                            HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                        }
                        else
                            return ResourceKey;
                    }
                }
                //PullUDF1-->PULLUDF1

                //loRoot = loResource.SelectSingleNode("root/data[@name='" + ResourceKey + "']/value");

                loRoot = loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + ResourceKey.ToLower() + "']/value");



                return (loRoot == null || (string.IsNullOrWhiteSpace(loRoot.InnerText))) ? ResourceKey : loRoot.InnerText;
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

        public static string GetEnterPriseResourceValue(string ResourceKey, string ResourceFile)
        {
            XmlDocument loResource = null;
            XmlNode loRoot = null;
            try
            {
                string strFilePath = EnterpriseResourceDirectoryPath + ResourceFile + FileCulterExtension + ResourceFileExt; ;
                string SessinKey = "Res_" + EnterpriseResourceFolder + "_" + ResourceFile + FileCulterExtension;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                    {
                        loResource.Load(strFilePath);
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else if (!string.IsNullOrEmpty(FileCulterExtension) && FileCulterExtension.ToLower() != "en-us") // This logic is for if no other culter file exist then default load english culter
                    {
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        strFilePath = strFilePath.Replace(FileCulterExtension, "");
                        if (System.IO.File.Exists(strFilePath))
                            loResource.Load(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else
                        return ResourceKey;
                }

                loRoot = loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + ResourceKey.ToLower() + "']/value");

                return (loRoot == null || (string.IsNullOrWhiteSpace(loRoot.InnerText))) ? ResourceKey : loRoot.InnerText;
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
            XmlDocument loResource = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                string SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    string strFilePath = GetFullFileNamePath(ResourceFile, culter);

                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                    {
                        loResource.Load(strFilePath);
                        XmlNodeList lstNodes = loResource.SelectNodes("root/data");
                        foreach (XmlNode item in lstNodes)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = item.Attributes["name"].Value;
                            if (kv.key.ToLower().Contains("udf"))
                            {
                                string RoomlevelUDF = GetFullFileNamePathRoom(ResourceFile, culter);
                                loResource = new XmlDocument();
                                if (!System.IO.File.Exists(RoomlevelUDF))
                                {
                                    CreateRoomResourceFile(RoomlevelUDF, kv.key);
                                }

                                XmlDocument loResourceUDF = new XmlDocument();
                                loResourceUDF.Load(RoomlevelUDF);
                                XmlNodeList lstNodesUDF = loResourceUDF.SelectNodes("root/data");
                                foreach (XmlNode itemUDF in lstNodesUDF)
                                {
                                    if (itemUDF.Attributes["name"].Value == kv.key)
                                    {
                                        kv.value = itemUDF.SelectSingleNode("value").InnerText;
                                    }
                                    else if (loResourceUDF.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + kv.key.ToLower() + "']") == null)
                                    {
                                        XmlNode child = null;
                                        XmlNode xname = null;
                                        XmlNode rootnode = null;

                                        rootnode = loResourceUDF.SelectSingleNode("root");
                                        xname = loResourceUDF.CreateNode(XmlNodeType.Element, "data", null);
                                        XmlAttribute xa = loResourceUDF.CreateAttribute("name");
                                        xa.Value = kv.key;
                                        XmlAttribute xa1 = loResourceUDF.CreateAttribute("xml", "space", "xml");
                                        xa1.Value = "preserve";
                                        xname.Attributes.Append(xa);
                                        xname.Attributes.Append(xa1);
                                        child = loResourceUDF.CreateNode(XmlNodeType.Element, "value", null);
                                        child.InnerText = kv.key;
                                        if (!string.IsNullOrEmpty(kv.value))
                                            child.InnerText = kv.value;

                                        if (xname != null && child != null)
                                        {
                                            xname.AppendChild(child);
                                        }
                                        if (rootnode != null && xname != null)
                                        {
                                            rootnode.AppendChild(xname);
                                        }
                                    }

                                }

                            }
                            else
                            {
                                kv.value = item.SelectSingleNode("value").InnerText;
                            }
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
        public static List<KeyValDTO> GetUDFResourceData(string ResourceFile, string culter)
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

                string SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + sessionculter + "_UDF";

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    //   string strFilePath = GetFullFileNamePath(ResourceFile, culter);
                    string strUDFFilePath = GetFullFileNamePathRoom(ResourceFile, culter);
                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strUDFFilePath))
                    {
                        loResource.Load(strUDFFilePath);
                        XmlNodeList lstNodes = loResource.SelectNodes("root/data");
                        foreach (XmlNode item in lstNodes)
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = item.Attributes["name"].Value;
                            if (kv.key.ToLower().Contains("udf"))
                            {
                                string RoomlevelUDF = strUDFFilePath;// GetFullFileNamePathRoom(ResourceFile);
                                loResource = new XmlDocument();
                                if (!System.IO.File.Exists(RoomlevelUDF))
                                {
                                    CreateRoomResourceFile(RoomlevelUDF, kv.key);
                                }

                                XmlDocument loResourceUDF = new XmlDocument();
                                loResourceUDF.Load(RoomlevelUDF);
                                XmlNodeList lstNodesUDF = loResourceUDF.SelectNodes("root/data");
                                foreach (XmlNode itemUDF in lstNodesUDF)
                                {
                                    if (itemUDF.Attributes["name"].Value == kv.key)
                                    {
                                        kv.value = itemUDF.SelectSingleNode("value").InnerText;
                                    }
                                    else if (loResourceUDF.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + kv.key.ToLower() + "']") == null)
                                    {
                                        XmlNode child = null;
                                        XmlNode xname = null;
                                        XmlNode rootnode = null;

                                        rootnode = loResourceUDF.SelectSingleNode("root");
                                        xname = loResourceUDF.CreateNode(XmlNodeType.Element, "data", null);
                                        XmlAttribute xa = loResourceUDF.CreateAttribute("name");
                                        xa.Value = kv.key;
                                        XmlAttribute xa1 = loResourceUDF.CreateAttribute("xml", "space", "xml");
                                        xa1.Value = "preserve";
                                        xname.Attributes.Append(xa);
                                        xname.Attributes.Append(xa1);
                                        child = loResourceUDF.CreateNode(XmlNodeType.Element, "value", null);
                                        child.InnerText = kv.key;
                                        if (!string.IsNullOrEmpty(kv.value))
                                            child.InnerText = kv.value;

                                        if (xname != null && child != null)
                                        {
                                            xname.AppendChild(child);
                                        }
                                        if (rootnode != null && xname != null)
                                        {
                                            rootnode.AppendChild(xname);
                                        }
                                    }

                                }

                            }
                            else
                            {
                                kv.value = item.SelectSingleNode("value").InnerText;
                            }
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
            XmlDocument loResource = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                string SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    string strFilePath = GetFullFileNamePath(ResourceFile, culter);

                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                    {
                        loResource.Load(strFilePath);
                        XmlNodeList lstNodes = loResource.SelectNodes("root/data");
                        foreach (XmlNode item in lstNodes)
                        {
                            if (item.Attributes["name"].Value.ToLower().Contains(SearchText.ToLower()) || item.SelectSingleNode("value").InnerText.ToLower().Contains(SearchText.ToLower()))
                            {
                                KeyValDTO kv = new KeyValDTO();
                                kv.key = item.Attributes["name"].Value;
                                kv.value = item.SelectSingleNode("value").InnerText;
                                objList.Add(kv);
                            }
                        }
                    }
                }
                else
                {
                    XmlNodeList lstNodes = loResource.SelectNodes("root/data");
                    foreach (XmlNode item in lstNodes)
                    {
                        if (item.Attributes["name"].Value.ToLower().Contains(SearchText.ToLower()) || item.SelectSingleNode("value").InnerText.ToLower().Contains(SearchText.ToLower()))
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = item.Attributes["name"].Value;
                            kv.value = item.SelectSingleNode("value").InnerText;
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
                loResource = null;
            }

        }
        public static List<KeyValDTO> GetEnterpriseResourceData(string ResourceFile, string culter)
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

                string SessinKey = "Res_" + EnterpriseResourceFolder + "_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

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
            XmlDocument loResource = null;

            try
            {
                if (culter == "en-US")
                    culter = "";
                else
                    culter = "." + culter;

                string strFilePath = string.Empty;
                string SessinKey = string.Empty;
                if (SaveIneTurns)
                {
                    if (isUDFName)
                    {
                        SessinKey = @"Res_Resources\MasterResources\RoomResources_" + ResourceFile + culter;
                        strFilePath = ResourceDirectoryBasePath + @"\MasterResources\RoomResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                    }
                    else
                    {
                        SessinKey = @"Res_Resources\MasterResources\EnterpriseResources_" + ResourceFile + culter;
                        strFilePath = ResourceDirectoryBasePath + @"\MasterResources\EnterpriseResources\" + ResourceFile + FileCulterExtension + ResourceFileExt;
                    }

                }
                else
                {
                    if (!ForEnterPriseSetup)
                    {
                        if (isUDFName)
                        {
                            SessinKey = "Res_" + RoomResourceFolder + "_" + ResourceFile + culter;
                            strFilePath = GetFullFileNamePathRoom(ResourceFile);
                        }
                        else
                        {
                            SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + culter;
                            strFilePath = GetFullFileNamePath(ResourceFile);
                        }
                    }
                    else
                    {
                        if (isUDFName)
                        {
                            strFilePath = RoomResourceDirectoryPathForEnterprise + ResourceFile + FileCulterExtension + ResourceFileExt;
                            SessinKey = "Res_" + RoomResourceDirectoryPathForEnterprise + "_" + ResourceFile + FileCulterExtension;

                        }
                    }
                }

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                        loResource.Load(strFilePath);
                }
                if (loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + ResourceKey.ToLower() + "']") != null)
                {
                    //loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + ResourceKey.ToLower() + "']/value");
                    loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + ResourceKey.ToLower() + "']/value").InnerText = value;

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

            return false;
        }

        public bool SaveResources(string ResourceFile, string culter, List<KeyValDTO> lstKeyVal)
        {
            XmlDocument loResource = null;
            XmlDocument loResourceUDF = null;
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
                string SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + culter;
                string SessinUDFKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + culter + "_UDF";
                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);
                if (HttpContext.Current.Cache.Get(SessinUDFKey) != null)
                    loResourceUDF = (XmlDocument)HttpContext.Current.Cache.Get(SessinUDFKey);

                string strFilePath = GetFullFileNamePath(ResourceFile, culter);

                string strUDFFilePath = GetFullFileNamePathRoom(ResourceFile, culter);

                if (loResource == null)
                {

                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                        loResource.Load(strFilePath);

                }
                if (loResourceUDF == null)
                {

                    loResourceUDF = new XmlDocument();
                    if (System.IO.File.Exists(strUDFFilePath))
                        loResourceUDF.Load(strUDFFilePath);
                    else
                    {
                        CreateRoomResourceFile(strUDFFilePath, "");
                        loResourceUDF.Load(strUDFFilePath);
                    }
                }
                foreach (var item in lstKeyVal)
                {
                    //loResource.SelectSingleNode("root/data[@name='" + item.key + "']/value").InnerText = item.value;
                    if (item.key.ToLower().Contains("udf"))
                    {
                        if (loResourceUDF.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + item.key.ToLower() + "']") != null)
                        {
                            loResourceUDF.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + item.key.ToLower() + "']/value").InnerText = item.value;
                        }
                        else //if (culter.Length > 0)
                        {
                            XmlNode child = null;
                            XmlNode xname = null;
                            XmlNode rootnode = null;

                            rootnode = loResourceUDF.SelectSingleNode("root");
                            xname = loResourceUDF.CreateNode(XmlNodeType.Element, "data", null);
                            XmlAttribute xa = loResourceUDF.CreateAttribute("name");
                            xa.Value = item.key;
                            XmlAttribute xa1 = loResourceUDF.CreateAttribute("xml", "space", "xml");
                            xa1.Value = "preserve";
                            xname.Attributes.Append(xa);
                            xname.Attributes.Append(xa1);
                            child = loResourceUDF.CreateNode(XmlNodeType.Element, "value", null);
                            child.InnerText = item.key;
                            if (!string.IsNullOrEmpty(item.value))
                                child.InnerText = item.value;

                            if (xname != null && child != null)
                            {
                                xname.AppendChild(child);
                            }
                            if (rootnode != null && xname != null)
                            {
                                rootnode.AppendChild(xname);
                            }
                        }
                    }
                    else
                    {


                        if (loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + item.key.ToLower() + "']") != null)
                        {
                            loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + item.key.ToLower() + "']/value").InnerText = item.value;
                        }
                        else if (culter.Length > 0)
                        {
                            //TODO: do not delete below commneted code;

                            XmlNode child = null;
                            XmlNode xname = null;
                            XmlNode rootnode = null;

                            rootnode = loResource.SelectSingleNode("root");
                            xname = loResource.CreateNode(XmlNodeType.Element, "data", null);
                            XmlAttribute xa = loResource.CreateAttribute("name");
                            xa.Value = item.key;
                            XmlAttribute xa1 = loResource.CreateAttribute("xml", "space", "xml");
                            xa1.Value = "preserve";
                            xname.Attributes.Append(xa);
                            xname.Attributes.Append(xa1);
                            child = loResource.CreateNode(XmlNodeType.Element, "value", null);
                            child.InnerText = item.value;
                            xname.AppendChild(child);
                            rootnode.AppendChild(xname);
                        }
                    }
                }
                loResource.Save(strFilePath);
                loResourceUDF.Save(strUDFFilePath);
            }
            catch (Exception)
            {
                return false;
                // Add exception log code here;
            }

            finally
            {
                loResource = null;
            }

            return true;
        }
        public bool SaveEnterpriseResources(string ResourceFile, string culter, List<KeyValDTO> lstKeyVal)
        {
            XmlDocument loResource = null;
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
                string SessinKey = "Res_" + EnterpriseResourceFolder + "_" + ResourceFile + culter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                //string strFilePath = GetFullFileNamePath(ResourceFile, culter);
                string strFilePath = string.Empty; //GetFullFileNamePath(ResourceFile, culter);
                if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
                {
                    strFilePath = (EnterpriseResourceDirectoryPath + ResourceFile + culter + ResourceFileExt).Replace("..", ".");
                }
                else
                {
                    strFilePath = (EnterpriseResourceDirectoryPath + ResourceFile + ResourceFileExt).Replace("..", ".");
                }

                if (loResource == null)
                {

                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                        loResource.Load(strFilePath);
                }
                foreach (var item in lstKeyVal)
                {
                    //loResource.SelectSingleNode("root/data[@name='" + item.key + "']/value").InnerText = item.value;
                    if (loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + item.key.ToLower() + "']") != null)
                    {
                        loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + item.key.ToLower() + "']/value").InnerText = item.value;
                    }
                    else if (culter.Length > 0)
                    {
                        //TODO: do not delete below commneted code;

                        XmlNode child = null;
                        XmlNode xname = null;
                        XmlNode rootnode = null;

                        rootnode = loResource.SelectSingleNode("root");
                        xname = loResource.CreateNode(XmlNodeType.Element, "data", null);
                        XmlAttribute xa = loResource.CreateAttribute("name");
                        xa.Value = item.key;
                        XmlAttribute xa1 = loResource.CreateAttribute("xml", "space", "xml");
                        xa1.Value = "preserve";
                        xname.Attributes.Append(xa);
                        xname.Attributes.Append(xa1);
                        child = loResource.CreateNode(XmlNodeType.Element, "value", null);
                        child.InnerText = item.value;
                        xname.AppendChild(child);
                        rootnode.AppendChild(xname);
                    }
                }
                loResource.Save(strFilePath);
            }
            catch (Exception)
            {
                return false;
                // Add exception log code here;
            }

            finally
            {
                loResource = null;
            }

            return true;
        }
        public bool SaveUserMasterResources(string ResourceFile, string culter, List<KeyValDTO> lstKeyVal, Int64 EnterpriseId)
        {
            XmlDocument loResource = null;
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
                string SessinKey = "Res_" + EnterpriseResourceFolder + "_" + ResourceFile + culter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                //string strFilePath = GetFullFileNamePath(ResourceFile, culter);
                string strFilePath = string.Empty; //GetFullFileNamePath(ResourceFile, culter);
                string EnterprisewiseResourcePath = ResourceDirectoryBasePath + @"\" + Convert.ToString(EnterpriseId) + @"\EnterpriseResources\";
                if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
                {
                    strFilePath = (EnterprisewiseResourcePath + ResourceFile + culter + ResourceFileExt).Replace("..", ".");
                }
                else
                {
                    strFilePath = (EnterprisewiseResourcePath + ResourceFile + ResourceFileExt).Replace("..", ".");
                }

                if (loResource == null)
                {

                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                        loResource.Load(strFilePath);
                }
                foreach (var item in lstKeyVal)
                {
                    //loResource.SelectSingleNode("root/data[@name='" + item.key + "']/value").InnerText = item.value;
                    if (loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + item.key.ToLower() + "']") != null)
                    {
                        loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + item.key.ToLower() + "']/value").InnerText = item.value;
                    }
                    else if (culter.Length > 0)
                    {
                        //TODO: do not delete below commneted code;

                        XmlNode child = null;
                        XmlNode xname = null;
                        XmlNode rootnode = null;

                        rootnode = loResource.SelectSingleNode("root");
                        xname = loResource.CreateNode(XmlNodeType.Element, "data", null);
                        XmlAttribute xa = loResource.CreateAttribute("name");
                        xa.Value = item.key;
                        XmlAttribute xa1 = loResource.CreateAttribute("xml", "space", "xml");
                        xa1.Value = "preserve";
                        xname.Attributes.Append(xa);
                        xname.Attributes.Append(xa1);
                        child = loResource.CreateNode(XmlNodeType.Element, "value", null);
                        child.InnerText = item.value;
                        xname.AppendChild(child);
                        rootnode.AppendChild(xname);
                    }
                }
                loResource.Save(strFilePath);
            }
            catch (Exception)
            {
                return false;
                // Add exception log code here;
            }

            finally
            {
                loResource = null;
            }

            return true;
        }
        public static string GetResourceValueByKeyAndFullFilePath(string ResourceKey, string ResourceFilePath)
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


                loRoot = loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + ResourceKey.ToLower() + "']/value");

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
        }
        public bool SaveResourcesAllCompany(string TargetResourceFile, List<KeyValCheckDTO> lstKeyVal)
        {
            try
            {
                ResXResourceReader rsxr = new ResXResourceReader(TargetResourceFile);
                Dictionary<string, string> dipropC = new Dictionary<string, string>();
                foreach (DictionaryEntry dd in rsxr)
                {
                    dipropC.Add(dd.Key.ToString(), dd.Value.ToString());

                }
                foreach (KeyValCheckDTO objitem in lstKeyVal)
                {
                    if (!dipropC.ContainsKey(objitem.key))
                    {
                        dipropC.Add(objitem.key, objitem.value);
                    }
                    else
                    {
                        dipropC[objitem.key] = objitem.value;
                    }
                }

                ResXResourceWriter resourceWriter = new ResXResourceWriter(TargetResourceFile);
                foreach (string ke in dipropC.Keys)
                {
                    resourceWriter.AddResource(ke, dipropC[ke]);
                }
                resourceWriter.Generate();
                resourceWriter.Close();
            }
            catch
            {
                return false;
                // Add exception log code here;
            }

            finally
            {
                //loResource = null;
            }

            return true;
        }
        public bool AddResourcesAllCompany(string TargetResourceFile, List<KeyValCheckDTO> lstKeyVal)
        {
            try
            {
                ResXResourceReader rsxr = new ResXResourceReader(TargetResourceFile);
                Dictionary<string, string> dipropC = new Dictionary<string, string>();
                foreach (DictionaryEntry dd in rsxr)
                {
                    dipropC.Add(dd.Key.ToString(), dd.Value.ToString());

                }
                foreach (KeyValCheckDTO objitem in lstKeyVal)
                {
                    if (!dipropC.ContainsKey(objitem.key))
                    {
                        dipropC.Add(objitem.key, objitem.value);
                    }
                }

                ResXResourceWriter resourceWriter = new ResXResourceWriter(TargetResourceFile);
                foreach (string ke in dipropC.Keys)
                {
                    resourceWriter.AddResource(ke, dipropC[ke]);
                }
                resourceWriter.Generate();
                resourceWriter.Close();
            }
            catch
            {
                return false;
                // Add exception log code here;
            }

            finally
            {
                //loResource = null;
            }

            return true;
        }

        public bool SaveResourcesAllCompany_ForUtility(string TargetResourceFile, List<KeyValCheckDTO> lstKeyVal)
        {
            try
            {
                ResXResourceReader rsxr = new ResXResourceReader(TargetResourceFile);
                Dictionary<string, string> dipropC = new Dictionary<string, string>();
                foreach (DictionaryEntry dd in rsxr)
                {
                    dipropC.Add(dd.Key.ToString(), dd.Value.ToString());

                }
                foreach (KeyValCheckDTO objitem in lstKeyVal)
                {
                    if (!dipropC.ContainsKey(objitem.key))
                    {
                        dipropC.Add(objitem.key, objitem.value);
                    }
                    else
                    {
                        if (dipropC[objitem.key] == objitem.key)
                        {
                            dipropC[objitem.key] = objitem.value;
                        }
                        else
                        {
                            if (dipropC[objitem.key] == objitem.engValue && objitem.engValue != objitem.value)
                            {
                                dipropC[objitem.key] = objitem.value;
                            }
                        }
                    }
                }

                ResXResourceWriter resourceWriter = new ResXResourceWriter(TargetResourceFile);
                foreach (string ke in dipropC.Keys)
                {
                    resourceWriter.AddResource(ke, dipropC[ke]);
                }
                resourceWriter.Generate();
                resourceWriter.Close();
            }
            catch
            {
                return false;
                // Add exception log code here;
            }

            finally
            {
                //loResource = null;
            }

            return true;
        }

        private static void CreateRoomResourceFile(string strFilePath, string ResourceKey = "")
        {
            using (System.Resources.ResXResourceWriter resx = new System.Resources.ResXResourceWriter(strFilePath))
            {
                if (SiteSettingHelper.LogResourceCalls == "yes")
                {
                    string LogMsg = (Environment.StackTrace ?? string.Empty) + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                    LogMsg = LogMsg + "strFilePath:" + (strFilePath ?? string.Empty) + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                    LogMsg = LogMsg + "ResourceKey:" + (ResourceKey ?? string.Empty) + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                    CommonFunctions.SaveLogInTextFile(LogMsg);
                }
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

        #endregion

        //Do not Delete Below function
        //public static string GetResorceDirectoryPath
        //{
        //    get
        //    {
        //        string path = typeof(ResourceHelper).Assembly.CodeBase;
        //        path = path.Substring(0, path.IndexOf("/bin")).Replace(@"file:///", "") + @"\Resources\";
        //        return path;
        //    }
        //}

        //public static string ResourceBaseClassPath { get; set; }


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
            _UDFTables.Add("BinUDF", "ResBinUDF");
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

        public static string GetReportModuleResource(string resourceKey)
        {
            string resValue = string.Empty;
            resValue = GetResourceValue(resourceKey, "ResReportModuleName");
            return resValue;
        }
        public static string GetReportNameByResource(string resourceKey)
        {
            string resValue = string.Empty;
            resValue = GetResourceValue(resourceKey, "ResReportResourceName");
            return resValue;
        }

        public static string GetAlertNameByResource(string resourceKey)
        {
            string resValue = string.Empty;
            resValue = GetResourceValue(resourceKey, "ResAlertResourceName");
            return resValue;
        }

        public static string GetResourceValueByKey(string filePath,string keyname) {
            try
            {
                var loResource = new XmlDocument();
                if (System.IO.File.Exists(filePath))
                {
                    loResource.Load(filePath);
                    if (loResource != null)
                    {
                        var loRoot = loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + keyname.ToLower() + "']/value");
                        return (loRoot == null || (string.IsNullOrWhiteSpace(loRoot.InnerText))) ? keyname : loRoot.InnerText;
                    }
                }
                return keyname;
            }
            catch (Exception ex)
            {
                return keyname;
            }
           
        }

        public static bool setInnerTextByKey(string filePath,string ResourceKey, string value)
        {
            try
            {
                var loResource = new XmlDocument();
                if (System.IO.File.Exists(filePath))
                {
                    loResource.Load(filePath);
                    if (loResource != null)
                    {
                        if (loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + ResourceKey.ToLower() + "']") != null)
                        {
                            loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + ResourceKey.ToLower() + "']/value").InnerText = value;
                        }
                        loResource.Save(filePath);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

            
        }
    }

    public class EnterPriseResourceHelper
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
                return System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
                //return HttpContext.Current.Server.MapPath(@"\Resources");
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
            XmlDocument loResource = null;
            XmlNode loRoot = null;
            try
            {
                string strFilePath = GetFullFileNamePath(ResourceFile);
                string SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + FileCulterExtension;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                    {
                        loResource.Load(strFilePath);
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else if (!string.IsNullOrEmpty(FileCulterExtension) && FileCulterExtension.ToLower() != "en-us") // This logic is for if no other culter file exist then default load english culter
                    {
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        strFilePath = strFilePath.Replace(FileCulterExtension, "");
                        if (System.IO.File.Exists(strFilePath))
                            loResource.Load(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else
                        return ResourceKey;
                }

                loRoot = loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + ResourceKey.ToLower() + "']/value");

                return (loRoot == null || (string.IsNullOrWhiteSpace(loRoot.InnerText))) ? ResourceKey : loRoot.InnerText;
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

        public static string GetEnterPriseResourceValue(string ResourceKey, string ResourceFile)
        {
            XmlDocument loResource = null;
            XmlNode loRoot = null;
            try
            {
                string strFilePath = EnterpriseResourceDirectoryPath + ResourceFile + FileCulterExtension + ResourceFileExt; ;
                string SessinKey = "Res_" + EnterpriseResourceFolder + "_" + ResourceFile + FileCulterExtension;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                    {
                        loResource.Load(strFilePath);
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else if (!string.IsNullOrEmpty(FileCulterExtension) && FileCulterExtension.ToLower() != "en-us") // This logic is for if no other culter file exist then default load english culter
                    {
                        CacheDependency cacheDep = new CacheDependency(strFilePath);
                        strFilePath = strFilePath.Replace(FileCulterExtension, "");
                        if (System.IO.File.Exists(strFilePath))
                            loResource.Load(strFilePath);
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else
                        return ResourceKey;
                }

                loRoot = loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + ResourceKey.ToLower() + "']/value");

                return (loRoot == null || (string.IsNullOrWhiteSpace(loRoot.InnerText))) ? ResourceKey : loRoot.InnerText;
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

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    string strFilePath = GetFullFileNamePath(ResourceFile, culter);

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
            XmlDocument loResource = null;
            List<KeyValDTO> objList = new List<KeyValDTO>();
            string sessionculter = "";
            try
            {
                if (culter == "en-US")
                    sessionculter = "";
                else
                    sessionculter = "." + culter;

                string SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    string strFilePath = GetFullFileNamePath(ResourceFile, culter);

                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                    {
                        loResource.Load(strFilePath);
                        XmlNodeList lstNodes = loResource.SelectNodes("root/data");
                        foreach (XmlNode item in lstNodes)
                        {
                            if (item.Attributes["name"].Value.ToLower().Contains(SearchText.ToLower()) || item.SelectSingleNode("value").InnerText.ToLower().Contains(SearchText.ToLower()))
                            {
                                KeyValDTO kv = new KeyValDTO();
                                kv.key = item.Attributes["name"].Value;
                                kv.value = item.SelectSingleNode("value").InnerText;
                                objList.Add(kv);
                            }
                        }
                    }
                }
                else
                {
                    XmlNodeList lstNodes = loResource.SelectNodes("root/data");
                    foreach (XmlNode item in lstNodes)
                    {
                        if (item.Attributes["name"].Value.ToLower().Contains(SearchText.ToLower()) || item.SelectSingleNode("value").InnerText.ToLower().Contains(SearchText.ToLower()))
                        {
                            KeyValDTO kv = new KeyValDTO();
                            kv.key = item.Attributes["name"].Value;
                            kv.value = item.SelectSingleNode("value").InnerText;
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
                loResource = null;
            }

        }
        public static List<KeyValDTO> GetEnterpriseResourceData(string ResourceFile, string culter)
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

                string SessinKey = "Res_" + EnterpriseResourceFolder + "_" + ResourceFile + sessionculter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

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
        public bool SaveResourcesByKey(string ResourceFile, string culter, string ResourceKey, string value)
        {
            XmlDocument loResource = null;

            try
            {
                if (culter == "en-US")
                    culter = "";
                else
                    culter = "." + culter;
                string SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + culter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                string strFilePath = GetFullFileNamePath(ResourceFile);

                if (loResource == null)
                {

                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                        loResource.Load(strFilePath);
                }

                loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + ResourceKey.ToLower() + "']/value").InnerText = value;
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

            return false;
        }

        public bool SaveResources(string ResourceFile, string culter, List<KeyValDTO> lstKeyVal)
        {
            XmlDocument loResource = null;
            XmlDocument loResourceUDF = null;
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
                string SessinKey = "Res_" + CompanyResourceFolder + "_" + ResourceFile + culter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                string strFilePath = GetFullFileNamePath(ResourceFile, culter);

                string strUDFFilePath = GetFullFileNamePathRoom(ResourceFile, culter);

                if (loResource == null)
                {

                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                        loResource.Load(strFilePath);
                }
                if (loResourceUDF == null)
                {
                    loResourceUDF = new XmlDocument();
                    if (System.IO.File.Exists(strUDFFilePath))
                        loResourceUDF.Load(strUDFFilePath);
                }
                foreach (var item in lstKeyVal)
                {
                    if (item.key.ToLower().Contains("udf"))
                    {
                        if (loResourceUDF.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + item.key.ToLower() + "']") != null)
                        {
                            loResourceUDF.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + item.key.ToLower() + "']/value").InnerText = item.value;
                        }
                        else if (culter.Length > 0)
                        {
                            XmlNode child = null;
                            XmlNode xname = null;
                            XmlNode rootnode = null;

                            rootnode = loResourceUDF.SelectSingleNode("root");
                            xname = loResourceUDF.CreateNode(XmlNodeType.Element, "data", null);
                            XmlAttribute xa = loResourceUDF.CreateAttribute("name");
                            xa.Value = item.key;
                            XmlAttribute xa1 = loResourceUDF.CreateAttribute("xml", "space", "xml");
                            xa1.Value = "preserve";
                            xname.Attributes.Append(xa);
                            xname.Attributes.Append(xa1);
                            child = loResourceUDF.CreateNode(XmlNodeType.Element, "value", null);
                            child.InnerText = item.value;
                            if (child != null && xname != null)
                            {
                                xname.AppendChild(child);
                            }
                            if (rootnode != null && xname != null)
                            {
                                rootnode.AppendChild(xname);
                            }
                        }
                    }
                    else
                    {
                        //loResource.SelectSingleNode("root/data[@name='" + item.key + "']/value").InnerText = item.value;
                        if (loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + item.key.ToLower() + "']") != null)
                        {
                            loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + item.key.ToLower() + "']/value").InnerText = item.value;
                        }
                        else if (culter.Length > 0)
                        {
                            //TODO: do not delete below commneted code;

                            XmlNode child = null;
                            XmlNode xname = null;
                            XmlNode rootnode = null;

                            rootnode = loResource.SelectSingleNode("root");
                            xname = loResource.CreateNode(XmlNodeType.Element, "data", null);
                            XmlAttribute xa = loResource.CreateAttribute("name");
                            xa.Value = item.key;
                            XmlAttribute xa1 = loResource.CreateAttribute("xml", "space", "xml");
                            xa1.Value = "preserve";
                            xname.Attributes.Append(xa);
                            xname.Attributes.Append(xa1);
                            child = loResource.CreateNode(XmlNodeType.Element, "value", null);
                            child.InnerText = item.value;
                            xname.AppendChild(child);
                            rootnode.AppendChild(xname);
                        }
                    }
                }
                loResource.Save(strFilePath);
                loResourceUDF.Save(strUDFFilePath);
            }
            catch (Exception)
            {
                return false;
                // Add exception log code here;
            }

            finally
            {
                loResource = null;
            }

            return true;
        }
        public bool SaveEnterpriseResources(string ResourceFile, string culter, List<KeyValDTO> lstKeyVal)
        {
            XmlDocument loResource = null;
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
                string SessinKey = "Res_" + EnterpriseResourceFolder + "_" + ResourceFile + culter;

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XmlDocument)HttpContext.Current.Cache.Get(SessinKey);

                //string strFilePath = GetFullFileNamePath(ResourceFile, culter);
                string strFilePath = string.Empty; //GetFullFileNamePath(ResourceFile, culter);
                if (!string.IsNullOrEmpty(culter) && culter.Trim().Length > 0)
                {
                    strFilePath = (EnterpriseResourceDirectoryPath + ResourceFile + culter + ResourceFileExt).Replace("..", ".");
                }
                else
                {
                    strFilePath = (EnterpriseResourceDirectoryPath + ResourceFile + ResourceFileExt).Replace("..", ".");
                }

                if (loResource == null)
                {

                    loResource = new XmlDocument();
                    if (System.IO.File.Exists(strFilePath))
                        loResource.Load(strFilePath);
                }
                foreach (var item in lstKeyVal)
                {
                    //loResource.SelectSingleNode("root/data[@name='" + item.key + "']/value").InnerText = item.value;

                    if (loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + item.key.ToLower() + "']") != null)
                    {
                        loResource.SelectSingleNode("root/data[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" + item.key.ToLower() + "']/value").InnerText = item.value;
                    }
                    else if (culter.Length > 0)
                    {
                        //TODO: do not delete below commneted code;

                        XmlNode child = null;
                        XmlNode xname = null;
                        XmlNode rootnode = null;

                        rootnode = loResource.SelectSingleNode("root");
                        xname = loResource.CreateNode(XmlNodeType.Element, "data", null);
                        XmlAttribute xa = loResource.CreateAttribute("name");
                        xa.Value = item.key;
                        XmlAttribute xa1 = loResource.CreateAttribute("xml", "space", "xml");
                        xa1.Value = "preserve";
                        xname.Attributes.Append(xa);
                        xname.Attributes.Append(xa1);
                        child = loResource.CreateNode(XmlNodeType.Element, "value", null);
                        child.InnerText = item.value;
                        xname.AppendChild(child);
                        rootnode.AppendChild(xname);
                    }
                }
                loResource.Save(strFilePath);
            }
            catch (Exception)
            {
                return false;
                // Add exception log code here;
            }

            finally
            {
                loResource = null;
            }

            return true;
        }
        public static string GetResourceValueByKeyAndFullFilePath(string ResourceKey, string ResourceFilePath)
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
        }

        #endregion
    }

    public class ResCommon
    {
        private static string ResourceFileName = "ResCommon";

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        /// 
        public static string DefaultSelect
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultSelect", ResourceFileName);
            }
        }
        public static string GenerateBarcodeForRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("GenerateBarcodeForRoom", ResourceFileName);
            }
        }

        public static string SelectAll
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectAll", ResourceFileName);
            }
        }
        public static string UnSelectAll
        {
            get
            {
                return ResourceRead.GetResourceValue("UnSelectAll", ResourceFileName);
            }
        }
        public static string Created
        {
            get
            {
                return ResourceRead.GetResourceValue("Created", ResourceFileName);
            }
        }
        public static string Hours
        {
            get
            {
                return ResourceRead.GetResourceValue("Hours", ResourceFileName);
            }
        }
        public static string Days
        {
            get
            {
                return ResourceRead.GetResourceValue("Days", ResourceFileName);
            }
        }
        public static string Weeks
        {
            get
            {
                return ResourceRead.GetResourceValue("Weeks", ResourceFileName);
            }
        }
        public static string Months
        {
            get
            {
                return ResourceRead.GetResourceValue("Months", ResourceFileName);
            }
        }
        public static string Years
        {
            get
            {
                return ResourceRead.GetResourceValue("Years", ResourceFileName);
            }
        }
        public static string IsDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDeleted", ResourceFileName);
            }
        }

        public static string ExcelExport
        {
            get
            {
                return ResourceRead.GetResourceValue("ExcelExport", ResourceFileName);
            }
        }
        public static string EmailTools
        {
            get
            {
                return ResourceRead.GetResourceValue("EmailTools", ResourceFileName);
            }
        }
        public static string EmailCheckoutTool
        {
            get
            {
                return ResourceRead.GetResourceValue("EmailCheckoutTool", ResourceFileName);
            }
        }
        public static string EmailToolMaint
        {
            get
            {
                return ResourceRead.GetResourceValue("EmailToolMaint", ResourceFileName);
            }
        }

        public static string ToolListExcel
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolListExcel", ResourceFileName);
            }
        }

        public static string ToolListPDF
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolListPDF", ResourceFileName);
            }
        }
        public static string checkoutToolListExcel
        {
            get
            {
                return ResourceRead.GetResourceValue("checkoutToolListExcel", ResourceFileName);
            }
        }

        public static string checkoutToolListPDF
        {
            get
            {
                return ResourceRead.GetResourceValue("checkoutToolListPDF", ResourceFileName);
            }
        }

        public static string MaintToolListExcel
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintToolListExcel", ResourceFileName);
            }
        }

        public static string MaintToolListPDF
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintToolListPDF", ResourceFileName);
            }
        }

        public static string EmailAssets
        {
            get
            {
                return ResourceRead.GetResourceValue("EmailAssets", ResourceFileName);
            }
        }
        public static string EmailAssetsMaint
        {
            get
            {
                return ResourceRead.GetResourceValue("EmailAssetsMaint", ResourceFileName);
            }
        }

        public static string RptAssets
        {
            get
            {
                return ResourceRead.GetResourceValue("RptAssets", ResourceFileName);
            }
        }
        public static string RptAssetsMaint
        {
            get
            {
                return ResourceRead.GetResourceValue("RptAssetsMaint", ResourceFileName);
            }
        }

        public static string AssetsExcel
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetsExcel", ResourceFileName);
            }
        }
        public static string AssetsPDF
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetsPDF", ResourceFileName);
            }
        }

        public static string AssetsMainExcel
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetsMainExcel", ResourceFileName);
            }
        }
        public static string AssetsMainPDF
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetsMainPDF", ResourceFileName);
            }
        }


        public static string ReportButton
        {
            get
            {
                return ResourceRead.GetResourceValue("ReportButton", ResourceFileName);
            }
        }

        public static string CSVExport
        {
            get
            {
                return ResourceRead.GetResourceValue("CSVExport", ResourceFileName);
            }
        }

        public static string ToolMasterCSVExport
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolMasterCSVExport", ResourceFileName);
            }
        }

        //
        public static string ToolCheckoutStatusExport
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCheckoutStatusExport", ResourceFileName);
            }
        }

        public static string ExportCheckoutHistory
        {
            get
            {
                return ResourceRead.GetResourceValue("ExportCheckoutHistory", ResourceFileName);
            }
        }

        public static string ItemLocExport
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocExport", ResourceFileName);
            }
        }
        public static string KitExport
        {
            get
            {
                return ResourceRead.GetResourceValue("KitExport", ResourceFileName);
            }
        }

        public static string ItemLocQtyExport
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocQtyExport", ResourceFileName);
            }
        }
        public static string BarCodeLabel
        {
            get
            {
                return ResourceRead.GetResourceValue("BarCodeLabel", ResourceFileName);
            }
        }
        public static string ExpandCollapse
        {
            get
            {
                return ResourceRead.GetResourceValue("ExpandCollapse", ResourceFileName);
            }
        }

        public static string CompanyID
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyID", ResourceFileName);
            }
        }

        public static string BulkEdit
        {
            get
            {
                return ResourceRead.GetResourceValue("BulkEdit", ResourceFileName);
            }
        }

        public static string SingleEdit
        {
            get
            {
                return ResourceRead.GetResourceValue("SingleEdit", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to CreatedBy.
        /// </summary>
        public static string CreatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedBy", ResourceFileName);
            }
        }
        public static string ActiveOn
        {
            get
            {
                return ResourceRead.GetResourceValue("ActiveOn", ResourceFileName);
            }
        }
        public static string CheckALL
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckALL", ResourceFileName);
            }
        }
        public static string UnCheckALL
        {
            get
            {
                return ResourceRead.GetResourceValue("UnCheckALL", ResourceFileName);
            }
        }

        public static string FirstLicenceAccept
        {
            get
            {
                return ResourceRead.GetResourceValue("FirstLicenceAccept", ResourceFileName);
            }
        }
        public static string LastLicenceAccept
        {
            get
            {
                return ResourceRead.GetResourceValue("LastLicenceAccept", ResourceFileName);
            }
        }
        public static string LicenceAcceptCount
        {
            get
            {
                return ResourceRead.GetResourceValue("LicenceAcceptCount", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Created On.
        /// </summary>
        public static string CreatedOn
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedOn", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room.
        /// </summary>
        public static string RoomName
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomName", ResourceFileName);
            }
        }

        public static string CompanyName
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyName", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UpdatedBy.
        /// </summary>
        public static string UpdatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("UpdatedBy", ResourceFileName);
            }
        }

        public static string ActionUpdate
        {
            get
            {
                return ResourceRead.GetResourceValue("ActionUpdate", ResourceFileName);
            }
        }
        public static string RevertRoleFailtMsg
        {
            get
            {
                return ResourceRead.GetResourceValue("RevertRoleFailtMsg", ResourceFileName);
            }
        }

        public static string AssignPrevRole
        {
            get
            {
                return ResourceRead.GetResourceValue("AssignPrevRole", ResourceFileName);
            }
        }
        public static string EnterpriseAdminRole
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterpriseAdminRole", ResourceFileName);
            }
        }
        public static string Room
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomName", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LastUpdatedBy.
        /// </summary>
        public static string LastUpdatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("LastUpdatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Updated On.
        /// </summary>
        public static string UpdatedOn
        {
            get
            {
                return ResourceRead.GetResourceValue("UpdatedOn", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Updated On.
        /// </summary>
        public static string Save
        {
            get
            {
                return ResourceRead.GetResourceValue("Save", ResourceFileName);
            }
        }

        public static string AddBlankColumns
        {
            get
            {
                return ResourceRead.GetResourceValue("AddBlankColumns", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Updated On.
        /// </summary>
        public static string Cancel
        {
            get
            {
                return ResourceRead.GetResourceValue("Cancel", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to add new.
        /// </summary>
        public static string AddNew
        {
            get
            {
                return ResourceRead.GetResourceValue("AddNew", ResourceFileName);
            }
        }
        public static string AddNonExistsKey
        {
            get
            {
                return ResourceRead.GetResourceValue("AddNonExistsKey", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to add new.
        /// </summary>
        public static string ApplyAll
        {
            get
            {
                return ResourceRead.GetResourceValue("ApplyAll", ResourceFileName);
            }
        }

        public static string AllItems
        {
            get
            {
                return ResourceRead.GetResourceValue("AllItems", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Updated On.
        /// </summary>
        public static string Search
        {
            get
            {
                return ResourceRead.GetResourceValue("Search", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Updated On.
        /// </summary>
        public static string ID
        {
            get
            {
                return ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Updated On.
        /// </summary>
        public static string RowNo
        {
            get
            {
                return ResourceRead.GetResourceValue("RowNo", ResourceFileName);
            }
        }

        public static string BarCode
        {
            get
            {
                return ResourceRead.GetResourceValue("BarCode", ResourceFileName);
            }
        }
        public static string BarCodeAdded
        {
            get
            {
                return ResourceRead.GetResourceValue("BarCodeAdded", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Updated On.
        /// </summary>
        public static string GUID
        {
            get
            {
                return ResourceRead.GetResourceValue("GUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Address.
        /// </summary>
        public static string Address
        {
            get
            {
                return ResourceRead.GetResourceValue("Address", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to City.
        /// </summary>
        public static string City
        {
            get
            {
                return ResourceRead.GetResourceValue("City", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to State.
        /// </summary>
        public static string State
        {
            get
            {
                return ResourceRead.GetResourceValue("State", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Country.
        /// </summary>
        public static string Country
        {
            get
            {
                return ResourceRead.GetResourceValue("Country", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phone.
        /// </summary>
        public static string Phone
        {
            get
            {
                return ResourceRead.GetResourceValue("Phone", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Email.
        /// </summary>
        public static string Email
        {
            get
            {
                return ResourceRead.GetResourceValue("Email", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PostalCode.
        /// </summary>
        public static string PostalCode
        {
            get
            {
                return ResourceRead.GetResourceValue("PostalCode", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ZipCode.
        /// </summary>
        public static string ZipCode
        {
            get
            {
                return ResourceRead.GetResourceValue("ZipCode", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to IsArchived.
        /// </summary>
        public static string IsArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IsArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsDeleted.
        /// </summary>


        /// <summary>
        ///   Looks up a localized string similar to IsDeleted.
        /// </summary>
        public static string IsActive
        {
            get
            {
                return ResourceRead.GetResourceValue("IsActive", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to ReorderColumnPopupHeader.
        /// </summary>
        public static string ReorderColumnPopupHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ReorderColumnPopupHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///  Common Resopurce HistoryID
        /// </summary>
        public static string HistoryID
        {
            get
            {
                return ResourceRead.GetResourceValue("HistoryID", ResourceFileName);
            }
        }

        /// <summary>
        ///  Common Resopurce HistoryAction
        /// </summary>
        public static string HistoryAction
        {
            get
            {
                return ResourceRead.GetResourceValue("HistoryAction", ResourceFileName);
            }
        }
        public static string Pull
        {
            get
            {
                return ResourceRead.GetResourceValue("Pull", ResourceFileName);
            }
        }
        public static string Credit
        {
            get
            {
                return ResourceRead.GetResourceValue("Credit", ResourceFileName);
            }
        }

        public static string New
        {
            get
            {
                return ResourceRead.GetResourceValue("New", ResourceFileName);
            }
        }
        public static string RecordPastMaintenance
        {
            get
            {
                return ResourceRead.GetResourceValue("RecordPastMaintenance", ResourceFileName);
            }
        }
        public static string Close
        {
            get
            {
                return ResourceRead.GetResourceValue("Close", ResourceFileName);
            }
        }
        public static string Edit
        {
            get
            {
                return ResourceRead.GetResourceValue("Edit", ResourceFileName);
            }
        }
        public static string mnuAdd
        {
            get
            {
                return ResourceRead.GetResourceValue("mnuAdd", ResourceFileName);
            }
        }
        public static string EulaList
        {
            get
            {
                return ResourceRead.GetResourceValue("EulaList", ResourceFileName);
            }
        }
        public static string ResourceLanguage
        {
            get
            {
                return ResourceRead.GetResourceValue("ResourceLanguage", ResourceFileName);
            }
        }
        public static string PullPOMasterList
        {
            get
            {
                return ResourceRead.GetResourceValue("PullPOMasterList", ResourceFileName);
            }
        }

        public static string mnuList
        {
            get
            {
                return ResourceRead.GetResourceValue("mnuList", ResourceFileName);
            }
        }
        public static string mnuItemBinList
        {
            get
            {
                return ResourceRead.GetResourceValue("mnuItemBinList", ResourceFileName);
            }
        }
        public static string mnuItemPictureView
        {
            get
            {
                return ResourceRead.GetResourceValue("mnuItemPictureView", ResourceFileName);
            }
        }
        public static string Reject
        {
            get
            {
                return ResourceRead.GetResourceValue("Reject", ResourceFileName);
            }
        }
        public static string Submitted
        {
            get
            {
                return ResourceRead.GetResourceValue("Submitted", ResourceFileName);
            }
        }
        public static string Unsubmitted
        {
            get
            {
                return ResourceRead.GetResourceValue("Unsubmitted", ResourceFileName);
            }
        }
        public static string Requested
        {
            get
            {
                return ResourceRead.GetResourceValue("Requested", ResourceFileName);
            }
        }
        public static string View
        {
            get
            {
                return ResourceRead.GetResourceValue("View", ResourceFileName);
            }
        }
        public static string Approved
        {
            get
            {
                return ResourceRead.GetResourceValue("Approve", ResourceFileName);
            }
        }
        public static string History
        {
            get
            {
                return ResourceRead.GetResourceValue("History", ResourceFileName);
            }
        }
        public static string ScheduleMappingHistory
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleMappingHistory", ResourceFileName);
            }
        }
        public static string ChangeLog
        {
            get
            {
                return ResourceRead.GetResourceValue("ChangeLog", ResourceFileName);
            }
        }
        public static string ReqWo
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqWo", ResourceFileName);
            }
        }
        public static string Requisition
        {
            get
            {
                return ResourceRead.GetResourceValue("Requisition", ResourceFileName);
            }
        }
        public static string WorkOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("WorkOrder", ResourceFileName);
            }
        }
        public static string MaintenancePastDue
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintenancePastDue", ResourceFileName);
            }
        }
        public static string MaintenanceDue
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintenanceDue", ResourceFileName);
            }
        }

        public static string List
        {
            get
            {
                return ResourceRead.GetResourceValue("List", ResourceFileName);
            }
        }

        #region "Scheduler/Maintenance tabs"

        public static string Maintenance
        {
            get
            {
                return ResourceRead.GetResourceValue("Maintenance", ResourceFileName);
            }
        }
        public static string AddScheduler
        {
            get
            {
                return ResourceRead.GetResourceValue("AddScheduler", ResourceFileName);
            }
        }
        public static string SchedulerList
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerList", ResourceFileName);
            }
        }
        public static string Odometer
        {
            get
            {
                return ResourceRead.GetResourceValue("Odometer", ResourceFileName);
            }
        }
        public static string UpdateMilesHours
        {
            get
            {
                return ResourceRead.GetResourceValue("UpdateMilesHours", ResourceFileName);
            }
        }
        public static string OdometerList
        {
            get
            {
                return ResourceRead.GetResourceValue("OdometerList", ResourceFileName);
            }
        }
        #endregion

        public static string Receivable
        {
            get
            {
                return ResourceRead.GetResourceValue("Receivable", ResourceFileName);
            }
        }
        public static string mnuRecPastMnts
        {
            get
            {
                return ResourceRead.GetResourceValue("mnuRecPastMnts", ResourceFileName);
            }
        }
        public static string mnuMntsDue
        {
            get
            {
                return ResourceRead.GetResourceValue("mnuMntsDue", ResourceFileName);
            }
        }
        public static string SaveConfirmation
        {
            get
            {
                return ResourceRead.GetResourceValue("SaveConfirmation", ResourceFileName);
            }
        }
        public static string ScheduleItems
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleItems", ResourceFileName);
            }
        }
        public static string Status
        {
            get
            {
                return ResourceRead.GetResourceValue("Status", ResourceFileName);
            }
        }
        public static string Reason
        {
            get
            {
                return ResourceRead.GetResourceValue("Reason", ResourceFileName);
            }
        }
        public static string ReportNew
        {
            get
            {
                return ResourceRead.GetResourceValue("ReportNew", ResourceFileName);
            }
        }
        public static string AlertNew
        {
            get
            {
                return ResourceRead.GetResourceValue("AlertNew", ResourceFileName);
            }
        }
        public static string ReportEdit
        {
            get
            {
                return ResourceRead.GetResourceValue("ReportEdit", ResourceFileName);
            }
        }
        public static string AlertEdit
        {
            get
            {
                return ResourceRead.GetResourceValue("AlertEdit", ResourceFileName);
            }
        }
        public static string StatusOpen
        {
            get
            {
                return ResourceRead.GetResourceValue("StatusOpen", ResourceFileName);
            }
        }
        public static string StatusClosed
        {
            get
            {
                return ResourceRead.GetResourceValue("StatusClosed", ResourceFileName);
            }
        }
        public static string mnuSuggestedOrders
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedOrders", ResourceFileName);
            }
        }
        public static string mnuSuggestedTransfers
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedTransfers", ResourceFileName);
            }
        }
        public static string mnuSuggestedReturn
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedReturn", ResourceFileName);
            }
        }
        public static string ReceivedOnWeb
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedOnWeb", ResourceFileName);
            }
        }

        public static string ReceivedOnWebDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedOnWebDate", ResourceFileName);
            }
        }
        public static string Technician
        {
            get
            {
                return ResourceRead.GetResourceValue("Technician", ResourceFileName);
            }
        }
        public static string TechnicianList
        {
            get
            {
                return ResourceRead.GetResourceValue("TechnicianList", ResourceFileName);
            }
        }
        public static string TechnicianErrorList
        {
            get
            {
                return ResourceRead.GetResourceValue("TechnicianErrorList", ResourceFileName);
            }
        }
        public static string SelectTechnicianText
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectTechnicianText", ResourceFileName);
            }
        }
        public static string TechnicianRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("TechnicianRequired", ResourceFileName);
            }
        }


        public static string SupplierImage
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierImage", ResourceFileName);
            }
        }
        public static string ReceivedOn
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedOn", ResourceFileName);
            }
        }

        public static string ReceivedOnDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedOnDate", ResourceFileName);
            }
        }

        public static string EditedFrom
        {
            get
            {
                return ResourceRead.GetResourceValue("EditedFrom", ResourceFileName);
            }
        }

        public static string AddedFrom
        {
            get
            {
                return ResourceRead.GetResourceValue("AddedFrom", ResourceFileName);
            }
        }
        public static string IsRequistionReportDisplay
        {
            get
            {
                return ResourceRead.GetResourceValue("IsRequistionReportDisplay", ResourceFileName);
            }
        }
        public static string IsNeverExpirePwd
        {
            get
            {
                return ResourceRead.GetResourceValue("IsNeverExpirePwd", ResourceFileName);
            }
        }
        public static string ShowDateTime
        {
            get
            {
                return ResourceRead.GetResourceValue("ShowDateTime", ResourceFileName);
            }
        }
        public static string IsAutoLogin
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAutoLogin", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Name.
        /// </summary>
        public static string Name
        {
            get
            {
                return ResourceRead.GetResourceValue("Name", ResourceFileName);
            }
        }

        public static string ToolHistoryList
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolHistoryList", ResourceFileName);
            }
        }

        public static string HistoryDate
        {
            get
            {
                return ResourceRead.GetResourceValue("HistoryDate", ResourceFileName);
            }
        }

        //
        public static string PermissionChanges
        {
            get
            {
                return ResourceRead.GetResourceValue("PermissionChanges", ResourceFileName);
            }
        }

        public static string OrderLimitType_AllOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderLimitType_AllOrder", ResourceFileName);
            }
        }

        public static string OrderLimitType_PerOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderLimitType_PerOrder", ResourceFileName);
            }
        }

        public static string DollarLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("DollarLimit", ResourceFileName);
            }
        }

        public static string DollarUsed
        {
            get
            {
                return ResourceRead.GetResourceValue("DollarUsed", ResourceFileName);
            }
        }

        public static string ItemOrderQuantityLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemOrderQuantityLimit", ResourceFileName);
            }
        }


        public static string OrderLimit_SetLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderLimit_SetLimit", ResourceFileName);
            }
        }

        public static string OrderLimit_NoLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderLimit_NoLimit", ResourceFileName);
            }
        }

        public static string UpdateCloseOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("UpdateCloseOrder", ResourceFileName);
            }
        }

        public static string LabelSize
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelSize", ResourceFileName);
            }
        }
        public static string LabelsPerSheet
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelsPerSheet", ResourceFileName);
            }
        }

        public static string Columns
        {
            get
            {
                return ResourceRead.GetResourceValue("Columns", ResourceFileName);
            }
        }

        public static string PageMarginLeft
        {
            get
            {
                return ResourceRead.GetResourceValue("PageMarginLeft", ResourceFileName);
            }
        }

        public static string PageMarginRight
        {
            get
            {
                return ResourceRead.GetResourceValue("PageMarginRight", ResourceFileName);
            }
        }
        public static string PageMarginTop
        {
            get
            {
                return ResourceRead.GetResourceValue("PageMarginTop", ResourceFileName);
            }
        }
        public static string PageMarginBottom
        {
            get
            {
                return ResourceRead.GetResourceValue("PageMarginBottom", ResourceFileName);
            }
        }

        public static string LabelWidth
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelWidth", ResourceFileName);
            }
        }
        public static string LabelHeight
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelHeight", ResourceFileName);
            }
        }

        public static string LabelPaddingLeft
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelPaddingLeft", ResourceFileName);
            }
        }

        public static string LabelPaddingRight
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelPaddingRight", ResourceFileName);
            }
        }

        public static string LabelPaddingTop
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelPaddingTop", ResourceFileName);
            }
        }

        public static string LabelPaddingBottom
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelPaddingBottom", ResourceFileName);
            }
        }

        public static string LabelSpacingHorizontal
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelSpacingHorizontal", ResourceFileName);
            }
        }

        public static string LabelSpacingVerticle
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelSpacingVerticle", ResourceFileName);
            }
        }

        public static string PageWidth
        {
            get
            {
                return ResourceRead.GetResourceValue("PageWidth", ResourceFileName);
            }
        }

        public static string PageHeight
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeight", ResourceFileName);
            }
        }

        public static string TemplateID
        {
            get
            {
                return ResourceRead.GetResourceValue("TemplateID", ResourceFileName);
            }
        }

        public static string CompanyIDForCatalogReport
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyIDForCatalogReport", ResourceFileName);
            }
        }

        public static string Qty
        {
            get
            {
                return ResourceRead.GetResourceValue("Qty", ResourceFileName);
            }
        }

        public static string Image
        {
            get
            {
                return ResourceRead.GetResourceValue("Image", ResourceFileName);
            }
        }

        public static string WrittenOffToolList
        {
            get
            {
                return ResourceRead.GetResourceValue("WrittenOffToolList", ResourceFileName);
            }
        }

        public static string UserProfile
        {
            get
            {
                return ResourceRead.GetResourceValue("UserProfile", ResourceFileName);
            }
        }
        public static string InvalidPhoneValue
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidPhoneValue", ResourceFileName);
            }
        }

        public static string CartItemReturnList
        {
            get
            {
                return ResourceRead.GetResourceValue("CartItemReturnList", ResourceFileName);
            }
        }

        public static string ModuleBreadCrumb
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleBreadCrumb", ResourceFileName);
            }
        }

        public static string DataArchivalInfo
        {
            get
            {
                return ResourceRead.GetResourceValue("DataArchivalInfo", ResourceFileName);
            }
        }

        public static string ArchivalDuration
        {
            get
            {
                return ResourceRead.GetResourceValue("ArchivalDuration", ResourceFileName);
            }
        }

        public static string ScheduleID
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleID", ResourceFileName);
            }
        }
        public static string IsScheduleActive
        {
            get
            {
                return ResourceRead.GetResourceValue("IsScheduleActive", ResourceFileName);
            }
        }
        public static string Module
        {
            get
            {
                return ResourceRead.GetResourceValue("Module", ResourceFileName);
            }
        }
        public static string Duration
        {
            get
            {
                return ResourceRead.GetResourceValue("Duration", ResourceFileName);
            }
        }

        public static string DurationType
        {
            get
            {
                return ResourceRead.GetResourceValue("DurationType", ResourceFileName);
            }
        }

        public static string Quantity
        {
            get
            {
                return ResourceRead.GetResourceValue("Quantity", ResourceFileName);
            }
        }

        public static string AddDetail
        {
            get
            {
                return ResourceRead.GetResourceValue("AddDetail", ResourceFileName);
            }
        }

        public static string DetailNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("DetailNumber", ResourceFileName);
            }
        }

        public static string Template
        {
            get
            {
                return ResourceRead.GetResourceValue("Template", ResourceFileName);
            }
        }

        public static string eMailToSendID
        {
            get
            {
                return ResourceRead.GetResourceValue("eMailToSendID", ResourceFileName);
            }
        }

        public static string ToAddress
        {
            get
            {
                return ResourceRead.GetResourceValue("ToAddress", ResourceFileName);
            }
        }

        public static string CCAddress
        {
            get
            {
                return ResourceRead.GetResourceValue("CCAddress", ResourceFileName);
            }
        }

        public static string EnterpriseID
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterpriseID", ResourceFileName);
            }
        }

        public static string BCCAddress
        {
            get
            {
                return ResourceRead.GetResourceValue("BCCAddress", ResourceFileName);
            }
        }

        public static string Subject
        {
            get
            {
                return ResourceRead.GetResourceValue("Subject", ResourceFileName);
            }
        }

        public static string HasAttachment
        {
            get
            {
                return ResourceRead.GetResourceValue("HasAttachment", ResourceFileName);
            }
        }

        public static string Remarks
        {
            get
            {
                return ResourceRead.GetResourceValue("Remarks", ResourceFileName);
            }
        }

        public static string SendingTried
        {
            get
            {
                return ResourceRead.GetResourceValue("SendingTried", ResourceFileName);
            }
        }

        public static string DownloadAttachment
        {
            get
            {
                return ResourceRead.GetResourceValue("DownloadAttachment", ResourceFileName);
            }
        }

        public static string ErrorDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorDescription", ResourceFileName);
            }
        }
        public static string msgSelectForViewHistory
        {
            get
            {
                return ResourceRead.GetResourceValue("msgSelectForViewHistory", ResourceFileName);
            }
        }
        public static string RecordDeletedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("RecordDeletedSuccessfully", ResourceFileName);
            }
        }
        public static string InvalidURL
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidURL", ResourceFileName);
            }
        }

        public static string DisconnectQuickBook
        {
            get
            {
                return ResourceRead.GetResourceValue("DisconnectQuickBook", ResourceFileName);
            }
        }

        public static string QuickBookCompanyName
        {
            get
            {
                return ResourceRead.GetResourceValue("QuickBookCompanyName", ResourceFileName);
            }
        }
        public static string ConnectToQuickBook
        {
            get
            {
                return ResourceRead.GetResourceValue("ConnectToQuickBook", ResourceFileName);
            }
        }

        public static string ValidFileName
        {
            get
            {
                return ResourceRead.GetResourceValue("ValidFileName", ResourceFileName);
            }
        }
        public static string NoChanges
        {
            get
            {
                return ResourceRead.GetResourceValue("NoChanges", ResourceFileName);
            }
        }
        public static string MsgRecordsUsedInOtherModule
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRecordsUsedInOtherModule", ResourceFileName);
            }
        }

        public static string MsgRecordsRestoredSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRecordsRestoredSuccessfully", ResourceFileName);
            }
        }

        public static string MsgMissing
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgMissing", ResourceFileName);
            }
        }

        public static string MsgInvalid
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalid", ResourceFileName);
            }
        }

        public static string MsgUpdatedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUpdatedSuccessfully", ResourceFileName);
            }
        }

        public static string MsgNoPermission
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNoPermission", ResourceFileName);
            }
        }

        public static string MsgNoPermissionContactAdmin
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNoPermissionContactAdmin", ResourceFileName);
            }
        }

        public static string SelectLanguage
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectLanguage", ResourceFileName);
            }
        }

        public static string MsgSelectLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectLocation", ResourceFileName);
            }
        }

        public static string MsgSomethingWrong
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSomethingWrong", ResourceFileName);
            }
        }
        public static string ErrorInProcess
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorInProcess", ResourceFileName);
            }
        }
        public static string success
        {
            get
            {
                return ResourceRead.GetResourceValue("success", ResourceFileName);
            }
        }
        public static string SelectRow
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectRow", ResourceFileName);
            }
        }
        public static string SelectRowToSave
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectRowToSave", ResourceFileName);
            }
        }
        public static string RecordUpdatedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("RecordUpdatedSuccessfully", ResourceFileName);
            }
        }
        public static string RecordClosedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("RecordClosedSuccessfully", ResourceFileName);
            }
        }
        public static string MsgRecordNotFound
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRecordNotFound", ResourceFileName);
            }
        }
        public static string MsgSaveBillingType
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSaveBillingType", ResourceFileName);
            }
        }
        public static string MsgDataSuccessfullyGet
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDataSuccessfullyGet", ResourceFileName);
            }
        }
        public static string MsgDataSaved
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDataSaved", ResourceFileName);
            }
        }

        public static string Critical
        {
            get
            {
                return ResourceRead.GetResourceValue("Critical", ResourceFileName);
            }
        }
        public static string Minimum
        {
            get
            {
                return ResourceRead.GetResourceValue("Minimum", ResourceFileName);
            }
        }
        public static string Maximum
        {
            get
            {
                return ResourceRead.GetResourceValue("Maximum", ResourceFileName);
            }
        }
        public static string RoomColon
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomColon", ResourceFileName);
            }
        }
        public static string PageOf
        {
            get
            {
                return ResourceRead.GetResourceValue("PageOf", ResourceFileName);
            }
        }
        public static string MsgDeletedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDeletedSuccessfully", ResourceFileName);
            }
        }
        public static string Approve
        {
            get
            {
                return ResourceRead.GetResourceValue("Approve", ResourceFileName);
            }
        }

        public static string FailToArchive
        {
            get
            {
                return ResourceRead.GetResourceValue("FailToArchive", ResourceFileName);
            }
        }
        public static string FailToUnarchive
        {
            get
            {
                return ResourceRead.GetResourceValue("FailToUnarchive", ResourceFileName);
            }
        }
        public static string ScriptSuccessfullyExecuted
        {
            get
            {
                return ResourceRead.GetResourceValue("ScriptSuccessfullyExecuted", ResourceFileName);
            }
        }
        public static string SelectOneEnterprise
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectOneEnterprise", ResourceFileName);
            }
        }
        public static string NoPermissionToExecuteScript
        {
            get
            {
                return ResourceRead.GetResourceValue("NoPermissionToExecuteScript", ResourceFileName);
            }
        }
        public static string RecordsDisplayed
        {
            get
            {
                return ResourceRead.GetResourceValue("RecordsDisplayed", ResourceFileName);
            }
        }
        public static string Ok
        {
            get
            {
                return ResourceRead.GetResourceValue("Ok", ResourceFileName);
            }
        }
        public static string NotOk
        {
            get
            {
                return ResourceRead.GetResourceValue("NotOk", ResourceFileName);
            }
        }

        public static string ScriptSuccess
        {
            get
            {
                return ResourceRead.GetResourceValue("ScriptSuccess", ResourceFileName);
            }
        }
        public static string FailedScript
        {
            get
            {
                return ResourceRead.GetResourceValue("FailedScript", ResourceFileName);
            }
        }

        public static string NoRights
        {
            get
            {
                return ResourceRead.GetResourceValue("NoRights", ResourceFileName);
            }
        }
        public static string FileUploaded
        {
            get
            {
                return ResourceRead.GetResourceValue("FileUploaded", ResourceFileName);
            }
        }

        public static string InvalidRequest
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidRequest", ResourceFileName);
            }
        }

        public static string CacheGeneratedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("CacheGeneratedSuccessfully", ResourceFileName);
            }
        }

        public static string ErrorGenaratingCache
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorGenaratingCache", ResourceFileName);
            }
        }
        public static string SyncCompleteMailSubject
        {
            get
            {
                return ResourceRead.GetResourceValue("SyncCompleteMailSubject", ResourceFileName);
            }
        }
        public static string DiscrepancyAfterSyncMailBody
        {
            get
            {
                return ResourceRead.GetResourceValue("DiscrepancyAfterSyncMailBody", ResourceFileName);
            }
        }
        public static string DiscrepancyReportFailSubject
        {
            get
            {
                return ResourceRead.GetResourceValue("DiscrepancyReportFailSubject", ResourceFileName);
            }
        }
        public static string DiscrepancyReportFailBodyNote
        {
            get
            {
                return ResourceRead.GetResourceValue("DiscrepancyReportFailBodyNote", ResourceFileName);
            }
        }

        public static string MsgGreaterThanZero
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgGreaterThanZero", ResourceFileName);
            }
        }
        public static string MsgNotAllowToInsert
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNotAllowToInsert", ResourceFileName);
            }
        }
        public static string NotUpdated
        {
            get
            {
                return ResourceRead.GetResourceValue("NotUpdated", ResourceFileName);
            }
        }

        public static string Incomplete
        {
            get
            {
                return ResourceRead.GetResourceValue("Incomplete", ResourceFileName);
            }
        }
        public static string MsgUndeleteSucessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUndeleteSucessfully", ResourceFileName);
            }
        }
        public static string MsgSelectRecordForArchieve
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectRecordForArchieve", ResourceFileName);
            }
        }

        public static string MsgSelectRecordForUnArchieve
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectRecordForUnArchieve", ResourceFileName);
            }
        }
        public static string MsgFailToArchieve
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgFailToArchieve", ResourceFileName);
            }
        }
        public static string MsgFailToUnArchieve
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgFailToUnArchieve", ResourceFileName);
            }
        }
        public static string AllItemsAddedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("AllItemsAddedSuccessfully", ResourceFileName);
            }
        }
        public static string ItemsAddedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemsAddedSuccessfully", ResourceFileName);
            }
        }
        public static string NoItemFound
        {
            get
            {
                return ResourceRead.GetResourceValue("NoItemFound", ResourceFileName);
            }
        }
        public static string EndDateShouldGreaterThanStartDate
        {
            get
            {
                return ResourceRead.GetResourceValue("EndDateShouldGreaterThanStartDate", ResourceFileName);
            }
        }
        public static string NoRecordUpdated
        {
            get
            {
                return ResourceRead.GetResourceValue("NoRecordUpdated", ResourceFileName);
            }
        }
        public static string NoRecordFound
        {
            get
            {
                return ResourceRead.GetResourceValue("NoRecordFound", ResourceFileName);
            }
        }

        public static string QtyCannotLessThanZeroForSerial
        {
            get
            {
                return ResourceRead.GetResourceValue("QtyCannotLessThanZeroForSerial", ResourceFileName);
            }
        }
        public static string QtyCannotGreaterThanOneForSerial
        {
            get
            {
                return ResourceRead.GetResourceValue("QtyCannotGreaterThanOneForSerial", ResourceFileName);
            }
        }
        public static string SomeErrorInDeleteRecord
        {
            get
            {
                return ResourceRead.GetResourceValue("SomeErrorInDeleteRecord", ResourceFileName);
            }
        }

        public static string MsgRecordSucessfullyUpdated
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRecordSucessfullyUpdated", ResourceFileName);
            }
        }

        public static string ReqRowSelection
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqRowSelection", ResourceFileName);
            }
        }
        public static string msgNoDataFound
        {
            get
            {
                return ResourceRead.GetResourceValue("msgNoDataFound", ResourceFileName);
            }
        }
        public static string ReqProperDara
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqProperDara", ResourceFileName);
            }
        }
        public static string ReqProperQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqProperQuantity", ResourceFileName);
            }
        }
        public static string Success
        {
            get
            {
                return ResourceRead.GetResourceValue("Success", ResourceFileName);
            }
        }

        public static string ErrorColon
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorColon", ResourceFileName);
            }
        }
        public static string RecordsUndeletedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("RecordsUndeletedSuccessfully", ResourceFileName);
            }
        }
        public static string DataNotAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("DataNotAvailable", ResourceFileName);
            }
        }

        public static string Fail
        {
            get
            {
                return ResourceRead.GetResourceValue("Fail", ResourceFileName);
            }
        }
        public static string ErrorinSave
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorinSave", ResourceFileName);
            }
        }

        public static string Comma
        {
            get
            {
                return ResourceRead.GetResourceValue("Comma", ResourceFileName);
            }
        }

        public static string msgActiveSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("msgActiveSuccessfully", ResourceFileName);
            }
        }
        public static string msgInActiveError
        {
            get
            {
                return ResourceRead.GetResourceValue("msgInActiveError", ResourceFileName);
            }
        }
        public static string msgInActiveSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("msgInActiveSuccessfully", ResourceFileName);
            }
        }
        public static string msgServerError
        {
            get
            {
                return ResourceRead.GetResourceValue("msgServerError", ResourceFileName);
            }
        }
        public static string RecordsSavedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("RecordsSavedSuccessfully", ResourceFileName);
            }
        }

        public static string MsgModuleInUseValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgModuleInUseValidation", ResourceFileName);
            }
        }
        public static string MsgCommonConfirmation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCommonConfirmation", ResourceFileName);
            }
        }

        public static string And
        {
            get
            {
                return ResourceRead.GetResourceValue("And", ResourceFileName);
            }
        }
        public static string Are
        {
            get
            {
                return ResourceRead.GetResourceValue("Are", ResourceFileName);
            }
        }

        public static string Is
        {
            get
            {
                return ResourceRead.GetResourceValue("Is", ResourceFileName);
            }
        }
        public static string Mandatory
        {
            get
            {
                return ResourceRead.GetResourceValue("Mandatory", ResourceFileName);
            }
        }
        public static string ErrorOccurred
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorOccurred", ResourceFileName);
            }
        }
        public static string MsgLostChangesConfirmation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLostChangesConfirmation", ResourceFileName);
            }
        }
        public static string MsgInvalidStringRemoved
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidStringRemoved", ResourceFileName);
            }
        }
        public static string CorrectErrorToProceed
        {
            get
            {
                return ResourceRead.GetResourceValue("CorrectErrorToProceed", ResourceFileName);
            }
        }
        public static string ReqLineItems
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqLineItems", ResourceFileName);
            }
        }
        public static string MsgSelectRowToDelete
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectRowToDelete", ResourceFileName);
            }
        }
        public static string SelectRecordstoUnDelete
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectRecordstoUnDelete", ResourceFileName);
            }
        }
        public static string FileUploadedSuccessfully { get { return ResourceRead.GetResourceValue("FileUploadedSuccessfully", ResourceFileName); } }
        public static string SelectPage { get { return ResourceRead.GetResourceValue("SelectPage", ResourceFileName); } }
        public static string MsgAddedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAddedSuccessfully", ResourceFileName);
            }
        }
        public static string ReqGreaterThanZeroQty
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqGreaterThanZeroQty", ResourceFileName);
            }
        }
        public static string MsgInvalidQty
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidQty", ResourceFileName);
            }
        }
        public static string MsgSavesuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSavesuccessfully", ResourceFileName);
            }
        }
        public static string ReqQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqQuantity", ResourceFileName);
            }
        }

        public static string InvalidDateToSave { get { return ResourceRead.GetResourceValue("InvalidDateToSave", ResourceFileName); } }
        public static string InvalidModuleId { get { return ResourceRead.GetResourceValue("InvalidModuleId", ResourceFileName); } }
        public static string RoomRecordsUpdated { get { return ResourceRead.GetResourceValue("RoomRecordsUpdated", ResourceFileName); } }

        public static string MsgSelectRecordExport { get { return ResourceRead.GetResourceValue("MsgSelectRecordExport", ResourceFileName); } }
        public static string MsgDeleteRecordConfirmation { get { return ResourceRead.GetResourceValue("MsgDeleteRecordConfirmation", ResourceFileName); } }

        public static string NotValidData { get { return ResourceRead.GetResourceValue("NotValidData", ResourceFileName); } }



        public static string MsgRecordArchievedSuccess { get { return ResourceRead.GetResourceValue("MsgRecordArchievedSuccess", ResourceFileName); } }

        public static string MsgFailtoArchieve { get { return ResourceRead.GetResourceValue("MsgFailtoArchieve", ResourceFileName); } }
        public static string StringToEncryptCantBeNull { get { return ResourceRead.GetResourceValue("StringToEncryptCantBeNull", ResourceFileName); } }
        public static string MsgNotDeleteSuccess { get { return ResourceRead.GetResourceValue("MsgNotDeleteSuccess", ResourceFileName); } }
        public static string StringToDecryptCantBeNull { get { return ResourceRead.GetResourceValue("StringToDecryptCantBeNull", ResourceFileName); } }
        public static string MsgUndeleteSuccess { get { return ResourceRead.GetResourceValue("MsgUndeleteSuccess", ResourceFileName); } }
        public static string MsgUndeleteFailure { get { return ResourceRead.GetResourceValue("MsgUndeleteFailure", ResourceFileName); } }
        public static string NoRooms { get { return ResourceRead.GetResourceValue("NoRooms", ResourceFileName); } }

        public static string MsgGuidExist { get { return ResourceRead.GetResourceValue("MsgGuidExist", ResourceFileName); } }
        public static string NotApplicable { get { return ResourceRead.GetResourceValue("NotApplicable", ResourceFileName); } }
        public static string BtnAddNew { get { return ResourceRead.GetResourceValue("BtnAddNew", ResourceFileName); } }
        public static string Add { get { return ResourceRead.GetResourceValue("Add", ResourceFileName); } }
        public static string AddAll { get { return ResourceRead.GetResourceValue("AddAll", ResourceFileName); } }
        public static string DeleteImage { get { return ResourceRead.GetResourceValue("DeleteImage", ResourceFileName); } }
        public static string NoImage { get { return ResourceRead.GetResourceValue("NoImage", ResourceFileName); } }


        public static string Usethis { get { return ResourceRead.GetResourceValue("Usethis", ResourceFileName); } }
        public static string Location { get { return ResourceRead.GetResourceValue("Location", ResourceFileName); } }
        public static string Open { get { return ResourceRead.GetResourceValue("Open", ResourceFileName); } }
        public static string AddLocation { get { return ResourceRead.GetResourceValue("AddLocation", ResourceFileName); } }
        public static string NewItem { get { return ResourceRead.GetResourceValue("NewItem", ResourceFileName); } }
        public static string NewToolCategory { get { return ResourceRead.GetResourceValue("NewToolCategory", ResourceFileName); } }
        public static string SelectItemFromCatalog { get { return ResourceRead.GetResourceValue("SelectItemFromCatalog", ResourceFileName); } }
        public static string SelectItemWithBins { get { return ResourceRead.GetResourceValue("SelectItemWithBins", ResourceFileName); } }
        public static string PrintLabel { get { return ResourceRead.GetResourceValue("PrintLabel", ResourceFileName); } }
        public static string ShowAllHidden { get { return ResourceRead.GetResourceValue("ShowAllHidden", ResourceFileName); } }
        public static string Yes { get { return ResourceRead.GetResourceValue("Yes", ResourceFileName); } }
        public static string No { get { return ResourceRead.GetResourceValue("No", ResourceFileName); } }
        public static string Or { get { return ResourceRead.GetResourceValue("Or", ResourceFileName); } }
        public static string AddRow { get { return ResourceRead.GetResourceValue("AddRow", ResourceFileName); } }
        public static string LoadAllItems { get { return ResourceRead.GetResourceValue("LoadAllItems", ResourceFileName); } }
        public static string Modules { get { return ResourceRead.GetResourceValue("Modules", ResourceFileName); } }
        public static string IsEnable { get { return ResourceRead.GetResourceValue("IsEnable", ResourceFileName); } }
        public static string BillingRoomTypeModules { get { return ResourceRead.GetResourceValue("BillingRoomTypeModules", ResourceFileName); } }
        public static string RedirectPage { get { return ResourceRead.GetResourceValue("RedirectPage", ResourceFileName); } }
        public static string Acceptcount { get { return ResourceRead.GetResourceValue("Acceptcount", ResourceFileName); } }
        public static string PasswordNote { get { return ResourceRead.GetResourceValue("PasswordNote", ResourceFileName); } }
        public static string ReleaseNo { get { return ResourceRead.GetResourceValue("ReleaseNo", ResourceFileName); } }
        public static string Date { get { return ResourceRead.GetResourceValue("Date", ResourceFileName); } }
        public static string Select { get { return ResourceRead.GetResourceValue("Select", ResourceFileName); } }
        public static string SendReportInEmail { get { return ResourceRead.GetResourceValue("SendReportInEmail", ResourceFileName); } }
        public static string SendTo { get { return ResourceRead.GetResourceValue("SendTo", ResourceFileName); } }
        public static string Body { get { return ResourceRead.GetResourceValue("Body", ResourceFileName); } }
        public static string BtnSendEmail { get { return ResourceRead.GetResourceValue("BtnSendEmail", ResourceFileName); } }
        public static string BtnSendPulls { get { return ResourceRead.GetResourceValue("BtnSendPulls", ResourceFileName); } }
        public static string BtnSendPull { get { return ResourceRead.GetResourceValue("BtnSendPull", ResourceFileName); } }
        public static string CancelAll { get { return ResourceRead.GetResourceValue("CancelAll", ResourceFileName); } }
        public static string ReqOneItem { get { return ResourceRead.GetResourceValue("ReqOneItem", ResourceFileName); } }
        public static string FromWhere { get { return ResourceRead.GetResourceValue("FromWhere", ResourceFileName); } }
        public static string Delete { get { return ResourceRead.GetResourceValue("Delete", ResourceFileName); } }
        public static string All { get { return ResourceRead.GetResourceValue("All", ResourceFileName); } }
        public static string RemoveRow { get { return ResourceRead.GetResourceValue("RemoveRow", ResourceFileName); } }
        public static string Reset { get { return ResourceRead.GetResourceValue("Reset", ResourceFileName); } }
        public static string SaveAs { get { return ResourceRead.GetResourceValue("SaveAs", ResourceFileName); } }
        public static string ChooseFile { get { return ResourceRead.GetResourceValue("ChooseFile", ResourceFileName); } }
        public static string Nofilechosen { get { return ResourceRead.GetResourceValue("Nofilechosen", ResourceFileName); } }
        public static string ListPageSettingPageTitle { get { return ResourceRead.GetResourceValue("ListPageSettingPageTitle", ResourceFileName); } }
        public static string ListPageSettingPageHeader { get { return ResourceRead.GetResourceValue("ListPageSettingPageHeader", ResourceFileName); } }
        public static string PageSize { get { return ResourceRead.GetResourceValue("PageSize", ResourceFileName); } }
        public static string SortingOn { get { return ResourceRead.GetResourceValue("SortingOn", ResourceFileName); } }
        public static string SortingType { get { return ResourceRead.GetResourceValue("SortingType", ResourceFileName); } }
        public static string SaveSetting { get { return ResourceRead.GetResourceValue("SaveSetting", ResourceFileName); } }
        public static string EturnsListPageSettingPageTitle { get { return ResourceRead.GetResourceValue("EturnsListPageSettingPageTitle", ResourceFileName); } }
        public static string EturnsListPageSettingPageHeader { get { return ResourceRead.GetResourceValue("EturnsListPageSettingPageHeader", ResourceFileName); } }
        public static string SelectedRoom { get { return ResourceRead.GetResourceValue("SelectedRoom", ResourceFileName); } }
        public static string SupplierPartNo { get { return ResourceRead.GetResourceValue("SupplierPartNo", ResourceFileName); } }

        public static string Apply { get { return ResourceRead.GetResourceValue("Apply", ResourceFileName); } }
        public static string RequiredFieldValidation { get { return ResourceRead.GetResourceValue("RequiredFieldValidation", ResourceFileName); } }
        public static string MsgRecordsDeleteSuccess { get { return ResourceRead.GetResourceValue("MsgRecordsDeleteSuccess", ResourceFileName); } }
        public static string Field { get { return ResourceRead.GetResourceValue("Field", ResourceFileName); } }
        public static string FileAttachment { get { return ResourceRead.GetResourceValue("FileAttachment", ResourceFileName); } }
        public static string AttachedFiles { get { return ResourceRead.GetResourceValue("AttachedFiles", ResourceFileName); } }
        public static string eTurnsTrackStock { get { return ResourceRead.GetResourceValue("eTurnsTrackStock", ResourceFileName); } }
        public static string eTurns { get { return ResourceRead.GetResourceValue("eTurns", ResourceFileName); } }
        public static string RecordUnClosedSuccessfully { get { return ResourceRead.GetResourceValue("RecordUnClosedSuccessfully", ResourceFileName); } }
        public static string eTurnsBilling { get { return ResourceRead.GetResourceValue("eTurnsBilling", ResourceFileName); } }
        public static string Individual { get { return ResourceRead.GetResourceValue("Individual", ResourceFileName); } }
        public static string Addon { get { return ResourceRead.GetResourceValue("Addon", ResourceFileName); } }
        public static string TieredBranch { get { return ResourceRead.GetResourceValue("TieredBranch", ResourceFileName); } }
        public static string TieredCompany { get { return ResourceRead.GetResourceValue("TieredCompany", ResourceFileName); } }
        public static string BinLocation { get { return ResourceRead.GetResourceValue("BinLocation", ResourceFileName); } }
        public static string NoDocsToDownload { get { return ResourceRead.GetResourceValue("NoDocsToDownload", ResourceFileName); } }

        

    }

    public class ResMessage
    {
        private static string ResourceFileName = "ResMessage";

        /// <summary>
        ///   Looks up a localized string similar to ({0}) Error! Record Not Saved..
        /// </summary>
        public static string SaveErrorMsg
        {
            get
            {
                return ResourceRead.GetResourceValue("SaveErrorMsg", ResourceFileName);
            }
        }

        public static string msgMultiemailEntry
        {
            get
            {
                return ResourceRead.GetResourceValue("msgMultiemailEntry", ResourceFileName);
            }
        }
        public static string MultiEmail
        {
            get
            {
                return ResourceRead.GetResourceValue("MultiEmail", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Record Saved Sucessfully....
        /// </summary>
        public static string SaveMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("SaveMessage", ResourceFileName);
            }
        }
        public static string SaveMessageWithReportFileError
        {
            get
            {
                return ResourceRead.GetResourceValue("SaveMessageWithReportFileError", ResourceFileName);
            }
        }
        public static string DuplicateSerialNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("DuplicateSerialNumber", ResourceFileName);
            }
        }
        public static string DuplicateToolName
        {
            get
            {
                return ResourceRead.GetResourceValue("DuplicateToolName", ResourceFileName);
            }
        }
        public static string DuplicateSupPartNo
        {
            get
            {
                return ResourceRead.GetResourceValue("DuplicateSupPartNo", ResourceFileName);
            }
        }
        public static string DuplicateManufacturerNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("DuplicateManufacturerNumber", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Please add quantity....
        /// </summary>
        public static string AddQuantityRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("AddQuantityRequired", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Ok.
        /// </summary>
        public static string DeleteConfirm
        {
            get
            {
                return ResourceRead.GetResourceValue("DeleteConfirm", ResourceFileName);
            }
        }
        public static string ChildReportList
        {
            get
            {
                return ResourceRead.GetResourceValue("ChildReportList", ResourceFileName);
            }
        }

        public static string UnDeleteConfirm
        {
            get
            {
                return ResourceRead.GetResourceValue("UnDeleteConfirm", ResourceFileName);
            }
        }

        public static string RequisitionClose
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionClose", ResourceFileName);
            }
        }
        public static string RequisitionApproveWithZero
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionApproveWithZero", ResourceFileName);
            }
        }

        public static string RequisitionToApproveQtyMsg
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionToApproveQtyMsg", ResourceFileName);
            }
        }

        public static string TransferToApproveQtyMsg
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferToApproveQtyMsg", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Ok.
        /// </summary>
        public static string ConfirmYes
        {
            get
            {
                return ResourceRead.GetResourceValue("ConfirmYes", ResourceFileName);
            }
        }
        public static string InformOk
        {
            get
            {
                return ResourceRead.GetResourceValue("InformOk", ResourceFileName);
            }
        }
        public static string ClosedRequistionConfirm
        {
            get
            {
                return ResourceRead.GetResourceValue("ClosedRequistionConfirm", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Ok.
        /// </summary>
        public static string ConfirmNo
        {
            get
            {
                return ResourceRead.GetResourceValue("ConfirmNo", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RequestProcessMessage.
        /// </summary>
        public static string RequestProcessMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestProcessMessage", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RequestProcessMessage.
        /// </summary>
        public static string InValidModelState
        {
            get
            {
                return ResourceRead.GetResourceValue("InValidModelState", ResourceFileName);
            }
        }

        public static string InvalidEmail
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidEmail", ResourceFileName);
            }
        }

        public static string DuplicateMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("DuplicateMessage", ResourceFileName);
            }
        }
        public static string InvalidModel
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidModel", ResourceFileName);
            }
        }
        public static string ActiveFailMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("ActiveFailMessage", ResourceFileName);
            }
        }

        public static string NotaSingleItemTransfer
        {
            get
            {
                return ResourceRead.GetResourceValue("NotaSingleItemTransfer", ResourceFileName);
            }
        }
        public static string NoRepliNeshRoomMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("NoRepliNeshRoomMessage", ResourceFileName);
            }
        }

        public static string ActionExecuted
        {
            get
            {
                return ResourceRead.GetResourceValue("ActionExecuted", ResourceFileName);
            }
        }

        public static string Required
        {
            get
            {
                return ResourceRead.GetResourceValue("Required", ResourceFileName);
            }
        }
        public static string MaxLength
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxLength", ResourceFileName);
            }
        }
        public static string InvalidValue
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidValue", ResourceFileName);
            }
        }
        public static string InvalidDataRange
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidDataRange", ResourceFileName);
            }
        }
        public static string NoDataAvailableInTable
        {
            get
            {
                return ResourceRead.GetResourceValue("NoDataAvailableInTable", ResourceFileName);
            }
        }


        public static string ShowingNoOfEntries
        {
            get
            {
                return ResourceRead.GetResourceValue("ShowingNoOfEntries", ResourceFileName);
            }
        }
        public static string ShowingErrorMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("ShowingErrorMessage", ResourceFileName);
            }
        }

        public static string ShowingZeroEntries
        {
            get
            {
                return ResourceRead.GetResourceValue("ShowingZeroEntries", ResourceFileName);
            }
        }


        public static string RequiredMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("RequiredMessage", ResourceFileName);
            }
        }

        public static string UDFRememberMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("UDFRememberMessage", ResourceFileName);
            }
        }

        public static string PasswordLengthMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("PasswordLengthMessage", ResourceFileName);
            }
        }
        public static string OldPasswordInvalid
        {
            get
            {
                return ResourceRead.GetResourceValue("OldPasswordInvalid", ResourceFileName);
            }
        }

        public static string NumberOnly
        {
            get
            {
                return ResourceRead.GetResourceValue("NumberOnly", ResourceFileName);
            }
        }

        public static string SpecialCharNotAllowed
        {
            get
            {
                return ResourceRead.GetResourceValue("SpecialCharNotAllowed", ResourceFileName);
            }
        }
        public static string InvalidPasswordExpiryMsg
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidPasswordExpiryMsg", ResourceFileName);
            }
        }
        public static string PassWordWarningMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("PassWordWarningMessage", ResourceFileName);
            }
        }
        public static string ChangePasswordConfirm
        {
            get
            {
                return ResourceRead.GetResourceValue("ChangePasswordConfirm", ResourceFileName);
            }
        }
        public static string Cancel
        {
            get
            {
                return ResourceRead.GetResourceValue("Cancel", ResourceFileName);
            }
        }
        public static string SelectAttachReport
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectAttachReport", ResourceFileName);
            }
        }
        public static string ConsingedCanNotchange
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsingedCanNotchange", ResourceFileName);
            }
        }


        public static string InvalidURL
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidURL", ResourceFileName);
            }
        }

        public static string InvalidFilename
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidFilename", ResourceFileName);
            }
        }

        //Number of Characters Left
        public static string NumberofCharactersLeft
        {
            get
            {
                return ResourceRead.GetResourceValue("NumberofCharactersLeft", ResourceFileName);
            }
        }

        public static string CanNotInsertActiveRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("CanNotInsertActiveRoom", ResourceFileName);
            }
        }

        //ActiveRoomAvailable
        public static string ActiveRoomAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("ActiveRoomAvailable", ResourceFileName);
            }
        }

        public static string KitItemAvailableInWIP
        {
            get
            {
                return ResourceRead.GetResourceValue("KitItemAvailableInWIP", ResourceFileName);
            }
        }

        //ExportCheckoutMessage
        public static string ExportCheckoutMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("ExportCheckoutMessage", ResourceFileName);
            }
        }
        //RepeatPassword
        public static string RepeatPassword
        {
            get
            {
                return ResourceRead.GetResourceValue("RepeatPassword", ResourceFileName);
            }
        }

        public static string UserCantWarnForEmptyDefaultBin
        {
            get
            {
                return ResourceRead.GetResourceValue("UserCantWarnForEmptyDefaultBin", ResourceFileName);
            }
        }
        //
        public static string PullOrderNumberRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("PullOrderNumberRequired", ResourceFileName);
            }
        }

        //
        public static string WarnNewBinCreated
        {
            get
            {
                return ResourceRead.GetResourceValue("WarnNewBinCreated", ResourceFileName);
            }
        }

        //PullInsertRights : You do not have insert pull rights 
        public static string DoNotPullInsertRights
        {
            get
            {
                return ResourceRead.GetResourceValue("DoNotPullInsertRights", ResourceFileName);
            }
        }

        public static string ZipFileIsRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("ZipFileIsRequired", ResourceFileName);
            }
        }

        public static string InvalidFileType
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidFileType", ResourceFileName);
            }
        }

        public static string OverrideOtherRoomsGridSetUp
        {
            get
            {
                return ResourceRead.GetResourceValue("OverrideOtherRoomsGridSetUp", ResourceFileName);
            }
        }

        public static string ConfirmOverrideOtherRoomsGridSetUp
        {
            get
            {
                return ResourceRead.GetResourceValue("ConfirmOverrideOtherRoomsGridSetUp", ResourceFileName);
            }
        }

        public static string GridSettingSavedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("GridSettingSavedSuccessfully", ResourceFileName);
            }
        }

        public static string FailToSaveGridSetting
        {
            get
            {
                return ResourceRead.GetResourceValue("FailToSaveGridSetting", ResourceFileName);
            }
        }

        public static string DeletedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("DeletedSuccessfully", ResourceFileName);
            }
        }

        public static string FailToDelete
        {
            get
            {
                return ResourceRead.GetResourceValue("FailToDelete", ResourceFileName);
            }
        }

        public static string SelectRowToDelete
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectRowToDelete", ResourceFileName);
            }
        }

        public static string PleaseWaitTransGoingOn
        {
            get
            {
                return ResourceRead.GetResourceValue("PleaseWaitTransGoingOn", ResourceFileName);
            }
        }

        public static string CantWrittenOffMoreThanAvailableQty
        {
            get
            {
                return ResourceRead.GetResourceValue("CantWrittenOffMoreThanAvailableQty", ResourceFileName);
            }
        }
        public static string SomeToolsAreNotWrittenOff
        {
            get
            {
                return ResourceRead.GetResourceValue("SomeToolsAreNotWrittenOff", ResourceFileName);
            }
        }

        public static string SelectRecordToWrittenOff
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectRecordToWrittenOff", ResourceFileName);
            }
        }

        public static string ReorderQtyAsPerCUOM
        {
            get
            {
                return ResourceRead.GetResourceValue("ReorderQtyAsPerCUOM", ResourceFileName);
            }
        }

        public static string ReorderQtyAsPerCUOMValue
        {
            get
            {
                return ResourceRead.GetResourceValue("ReorderQtyAsPerCUOMValue", ResourceFileName);
            }
        }

        public static string AttachmentTypeIsMandatory
        {
            get
            {
                return ResourceRead.GetResourceValue("AttachmentTypeIsMandatory", ResourceFileName);
            }
        }

        public static string AttachmentTypeIsMandatoryWithAttachmentReport
        {
            get
            {
                return ResourceRead.GetResourceValue("AttachmentTypeIsMandatoryWithAttachmentReport", ResourceFileName);
            }
        }

        public static string AutoClassificationItemsExistInRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("AutoClassificationItemsExistInRoom", ResourceFileName);
            }
        }


        public static string TrendingSettingItemExistInRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingSettingItemExistInRoom", ResourceFileName);
            }
        }

        public static string MinQtyCantBeGreaterThenMaxQty
        {
            get
            {
                return ResourceRead.GetResourceValue("MinQtyCantBeGreaterThenMaxQty", ResourceFileName);
            }
        }

        public static string StagingCantBeChanged
        {
            get
            {
                return ResourceRead.GetResourceValue("StagingCantBeChanged", ResourceFileName);
            }
        }
        //Please display available Order line items. Not deleted items.
        public static string RemoveDeletedItemFlag
        {
            get
            {
                return ResourceRead.GetResourceValue("RemoveDeletedItemFlag", ResourceFileName);
            }
        }

        public static string DuplicateArchiveSchedule
        {
            get
            {
                return ResourceRead.GetResourceValue("DuplicateArchiveSchedule", ResourceFileName);
            }
        }

        public static string SelectCheckOutTool
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectCheckOutTool", ResourceFileName);
            }
        }

        public static string CheckUseThis
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckUseThis", ResourceFileName);
            }
        }

        public static string InvalidTechnician
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidTechnician", ResourceFileName);
            }
        }

        public static string FailToReassign
        {
            get
            {
                return ResourceRead.GetResourceValue("FailToReassign", ResourceFileName);
            }
        }

        public static string BuildKitMinimumQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("BuildKitMinimumQuantity", ResourceFileName);
            }
        }

        public static string BuildKitMaximumQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("BuildKitMaximumQuantity", ResourceFileName);
            }
        }

        public static string MsgUDFMaxLengthRange
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUDFMaxLengthRange", ResourceFileName);
            }
        }

        public static string UnWrittenOffSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("UnWrittenOffSuccessfully", ResourceFileName);
            }
        }

        public static string FailToUnWrittenOff
        {
            get
            {
                return ResourceRead.GetResourceValue("FailToUnWrittenOff", ResourceFileName);
            }
        }

        public static string SelectRowToUnWrittenOff
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectRowToUnWrittenOff", ResourceFileName);
            }
        }

        public static string WorkordersClosedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("WorkordersClosedSuccessfully", ResourceFileName);
            }
        }

        public static string SelectOpenWorkOrderToClose
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectOpenWorkOrderToClose", ResourceFileName);
            }
        }

        public static string CloseWorkorderConfirmationMsg
        {
            get
            {
                return ResourceRead.GetResourceValue("CloseWorkorderConfirmationMsg", ResourceFileName);
            }
        }

        public static string NoRightsToInsertWorkorder
        {
            get
            {
                return ResourceRead.GetResourceValue("NoRightsToInsertWorkorder", ResourceFileName);
            }
        }

        public static string NoRightsToInsertTechnician
        {
            get
            {
                return ResourceRead.GetResourceValue("NoRightsToInsertTechnician", ResourceFileName);
            }
        }

        public static string EnterpriseQLSuccess
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterpriseQLSuccess", ResourceFileName);
            }
        }

        public static string EnterpriseQLFailure
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterpriseQLFailure", ResourceFileName);
            }
        }

        public static string EnterpriseQLDuplicate
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterpriseQLDuplicate", ResourceFileName);
            }
        }

        public static string RequisitionWasRejected
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionWasRejected", ResourceFileName);
            }
        }
        public static string RequisitionNotValidForRejected
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionNotValidForRejected", ResourceFileName);
            }
        }
        public static string RequisitionDoesntExist
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionDoesntExist", ResourceFileName);
            }
        }
        public static string RequisitionAlreadyRejected
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionAlreadyRejected", ResourceFileName);
            }
        }
        public static string NotValidData
        {
            get
            {
                return ResourceRead.GetResourceValue("NotValidData", ResourceFileName);
            }
        }
        public static string RequisitionApproved
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionApproved", ResourceFileName);
            }
        }
        public static string RequisitionAlredyApproved
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionAlredyApproved", ResourceFileName);
            }
        }
        public static string RequisitionNotValidToApproval
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionNotValidToApproval", ResourceFileName);
            }
        }

        public static string QuickListDetailNoRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("QuickListDetailNoRequired", ResourceFileName);
            }
        }
        public static string QuickListDetailNoAlreadyAdded
        {
            get
            {
                return ResourceRead.GetResourceValue("QuickListDetailNoAlreadyAdded", ResourceFileName);
            }
        }

        public static string ReportDataSelectionTypeRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("ReportDataSelectionTypeRequired", ResourceFileName);
            }
        }

        public static string DatasinceDaysRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("DatasinceDaysRequired", ResourceFileName);
            }
        }

        public static string QuantityTypeSelectionRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityTypeSelectionRequired", ResourceFileName);
            }
        }

        public static string MaxJsonLengthexceed
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxJsonLengthexceed", ResourceFileName);
            }
        }

        public static string MsgDoesNotExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDoesNotExist", ResourceFileName);
            }
        }

        public static string MsgDuplicateSerialFound
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDuplicateSerialFound", ResourceFileName);
            }
        }

        public static string MsgRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRequired", ResourceFileName);
            }
        }

        public static string MsgDeletedAndUsed
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDeletedAndUsed", ResourceFileName);
            }
        }
        public static string DuplicateSerialFound
        {
            get
            {
                return ResourceRead.GetResourceValue("DuplicateSerialFound", ResourceFileName);
            }
        }
        public static string DuplicateLotFound
        {
            get
            {
                return ResourceRead.GetResourceValue("DuplicateLotFound", ResourceFileName);
            }
        }
        public static string MsgQuickListDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQuickListDeleted", ResourceFileName);
            }
        }
        public static string MsgDuplicateSerialFoundForCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDuplicateSerialFoundForCredit", ResourceFileName);
            }
        }
        public static string DuplicateNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("DuplicateNumber", ResourceFileName);
            }
        }
        public static string msgQtyMustlessthanOHQ
        {
            get
            {
                return ResourceRead.GetResourceValue("msgQtyMustlessthanOHQ", ResourceFileName);
            }
        }
        public static string msgSelectRecordForDelete
        {
            get
            {
                return ResourceRead.GetResourceValue("msgSelectRecordForDelete", ResourceFileName);
            }
        }
        public static string msgSelectRecordToReSend
        {
            get
            {
                return ResourceRead.GetResourceValue("msgSelectRecordToReSend", ResourceFileName);
            }
        }
        public static string ReSendSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("ReSendSuccessfully", ResourceFileName);
            }
        }
        public static string MsgSerialNotFound
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSerialNotFound", ResourceFileName);
            }
        }
        public static string MsgLoading
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLoading", ResourceFileName);
            }
        }

        public static string ArchiveConfirm
        {
            get
            {
                return ResourceRead.GetResourceValue("ArchiveConfirm", ResourceFileName);
            }
        }
        public static string UnArchiveConfirm
        {
            get
            {
                return ResourceRead.GetResourceValue("UnArchiveConfirm", ResourceFileName);
            }
        }
        public static string NoRightsToInsertProjectSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("NoRightsToInsertProjectSpend", ResourceFileName);
            }
        }
        public static string NoRightsToInsertCustomer
        {
            get
            {
                return ResourceRead.GetResourceValue("NoRightsToInsertCustomer", ResourceFileName);
            }
        }
    }

    public class ResNarrowSearch
    {
        private static string ResourceFileName = "ResNarrowSearch";

        /// <summary>
        ///   Looks up a localized string similar to Clear.
        /// </summary>
        public static string Clear
        {
            get
            {
                return ResourceRead.GetResourceValue("Clear", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Created by: .
        /// </summary>
        public static string Createdby
        {
            get
            {
                return ResourceRead.GetResourceValue("Createdby", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Date Created.
        /// </summary>
        public static string DateCreated
        {
            get
            {
                return ResourceRead.GetResourceValue("DateCreated", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Date Updated.
        /// </summary>
        public static string DateUpdated
        {
            get
            {
                return ResourceRead.GetResourceValue("DateUpdated", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to From.
        /// </summary>
        public static string From
        {
            get
            {
                return ResourceRead.GetResourceValue("From", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to selected.
        /// </summary>
        public static string selected
        {
            get
            {
                return ResourceRead.GetResourceValue("selected", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to To.
        /// </summary>
        public static string To
        {
            get
            {
                return ResourceRead.GetResourceValue("To", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Updated by: .
        /// </summary>
        public static string Updatedby
        {
            get
            {
                return ResourceRead.GetResourceValue("Updatedby", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to User Created by.
        /// </summary>
        public static string UserCreatedby
        {
            get
            {
                return ResourceRead.GetResourceValue("UserCreatedby", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to User Updated by.
        /// </summary>
        public static string UserUpdatedby
        {
            get
            {
                return ResourceRead.GetResourceValue("UserUpdatedby", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to User Updated by.
        /// </summary>
        public static string NarrowSearchHeader1
        {
            get
            {
                return ResourceRead.GetResourceValue("NarrowSearchHeader1", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to User Updated by.
        /// </summary>
        public static string NarrowSearchHeader2
        {
            get
            {
                return ResourceRead.GetResourceValue("NarrowSearchHeader2", ResourceFileName);
            }
        }

        public static string Cost
        {
            get
            {
                return ResourceRead.GetResourceValue("Cost", ResourceFileName);
            }
        }

        public static string AverageUsage
        {
            get
            {
                return ResourceRead.GetResourceValue("AverageUsage", ResourceFileName);
            }
        }

        public static string AverageCost
        {
            get
            {
                return ResourceRead.GetResourceValue("AverageCost", ResourceFileName);
            }
        }

        public static string ActionType
        {
            get
            {
                return ResourceRead.GetResourceValue("ActionType", ResourceFileName);
            }
        }
        public static string Technician
        {
            get
            {
                return ResourceRead.GetResourceValue("Technician", ResourceFileName);
            }
        }
        public static string ToolsCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolsCategory", ResourceFileName);
            }
        }
        public static string ToolsLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolsLocation", ResourceFileName);
            }
        }
        public static string ToolsMaintenance
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolsMaintenance", ResourceFileName);
            }
        }


        public static string Category
        {
            get
            {
                return ResourceRead.GetResourceValue("Category", ResourceFileName);
            }
        }
        public static string ItemType
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemType", ResourceFileName);
            }
        }
        public static string ItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNumber", ResourceFileName);
            }
        }
        public static string BinNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("BinNumber", ResourceFileName);
            }
        }

        public static string InventoryClassification
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryClassification", ResourceFileName);
            }
        }
        public static string Manufacturer
        {
            get
            {
                return ResourceRead.GetResourceValue("Manufacturer", ResourceFileName);
            }
        }
        public static string StockStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("StockStatus", ResourceFileName);
            }
        }
        public static string Supplier
        {
            get
            {
                return ResourceRead.GetResourceValue("Supplier", ResourceFileName);
            }
        }

        public static string ItemLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocation", ResourceFileName);
            }
        }
        public static string ItemTrackingType
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemTrackingType", ResourceFileName);
            }
        }

        public static string ProjectSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpend", ResourceFileName);
            }
        }
        public static string WorkOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("WorkOrder", ResourceFileName);
            }
        }
        public static string Requisition
        {
            get
            {
                return ResourceRead.GetResourceValue("Requisition", ResourceFileName);
            }
        }
        public static string OrderNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderNumber", ResourceFileName);
            }
        }
        public static string SupplierAccountNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierAccountNumber", ResourceFileName);
            }
        }
        public static string ModuleType
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleType", ResourceFileName);
            }
        }
        public static string Items
        {
            get
            {
                return ResourceRead.GetResourceValue("Items", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Reorder:.
        /// </summary>
        public static string SchedulerItem
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerItem", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Reorder:.
        /// </summary>
        public static string SchedulerType
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerType", ResourceFileName);
            }
        }
        public static string Turns
        {
            get
            {
                return ResourceRead.GetResourceValue("Turns", ResourceFileName);
            }
        }
        public static string DateReceived
        {
            get
            {
                return ResourceRead.GetResourceValue("DateReceived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Role Type.
        /// </summary>
        public static string RoleType
        {
            get
            {
                return ResourceRead.GetResourceValue("RoleType", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Enterprise.
        /// </summary>
        public static string Enterprise
        {
            get
            {
                return ResourceRead.GetResourceValue("Enterprise", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Company.
        /// </summary>
        public static string Company
        {
            get
            {
                return ResourceRead.GetResourceValue("Company", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Company.
        /// </summary>
        public static string Room
        {
            get
            {
                return ResourceRead.GetResourceValue("Room", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to User Type.
        /// </summary>
        public static string UserType
        {
            get
            {
                return ResourceRead.GetResourceValue("UserType", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Role.
        /// </summary>
        public static string Role
        {
            get
            {
                return ResourceRead.GetResourceValue("Role", ResourceFileName);
            }
        }

        public static string BillingRoomType
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingRoomType", ResourceFileName);
            }
        }

        public static string InvoiceBranch
        {
            get
            {
                return ResourceRead.GetResourceValue("InvoiceBranch", ResourceFileName);
            }
        }

        //RequisitionStatus
        public static string RequisitionStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionStatus", ResourceFileName);
            }
        }


        public static string Consignment
        {
            get
            {
                return ResourceRead.GetResourceValue("Consignment", ResourceFileName);
            }
        }


        //AssetsCategory
        public static string AssetsCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetsCategory", ResourceFileName);
            }
        }

        public static string RequestType
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestType", ResourceFileName);
            }
        }

        public static string CountType
        {
            get
            {
                return ResourceRead.GetResourceValue("CountType", ResourceFileName);
            }
        }
        public static string CountStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("CountStatus", ResourceFileName);
            }
        }
        public static string StagingLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("StagingLocation", ResourceFileName);
            }
        }

        public static string FilterCriteriaSaved { get { return ResourceRead.GetResourceValue("FilterCriteriaSaved", ResourceFileName); } }
        public static string SomeErrorHasOccurred { get { return ResourceRead.GetResourceValue("SomeErrorHasOccurred", ResourceFileName); } }
        public static string MsgInvalidFilterCrtNotification { get { return ResourceRead.GetResourceValue("MsgInvalidFilterCrtNotification", ResourceFileName); } }
        public static string MsgFilterCriteriaApplied { get { return ResourceRead.GetResourceValue("MsgFilterCriteriaApplied", ResourceFileName); } }
        public static string MsgFilterCriteriaLoading { get { return ResourceRead.GetResourceValue("MsgFilterCriteriaLoading", ResourceFileName); } }
        public static string Check { get { return ResourceRead.GetResourceValue("Check", ResourceFileName); } }
        public static string UnCheck { get { return ResourceRead.GetResourceValue("UnCheck", ResourceFileName); } }
        public static string Selectoptions { get { return ResourceRead.GetResourceValue("Selectoptions", ResourceFileName); } }
        public static string Save
        {
            get
            {
                return ResourceRead.GetResourceValue("Save", ResourceFileName);
            }
        }
        public static string ReplenishType
        {
            get
            {
                return ResourceRead.GetResourceValue("ReplenishType", ResourceFileName);
            }
        }
        public static string Filter
        {
            get
            {
                return ResourceRead.GetResourceValue("Filter", ResourceFileName);
            }
        }
        public static string Enterkeywords
        {
            get
            {
                return ResourceRead.GetResourceValue("Enterkeywords", ResourceFileName);
            }
        }
        public static string IsBilling
        {
            get
            {
                return ResourceRead.GetResourceValue("IsBilling", ResourceFileName);
            }
        }
        public static string IsEDISent
        {
            get
            {
                return ResourceRead.GetResourceValue("IsEDISent", ResourceFileName);
            }
        }
        public static string Asset
        {
            get
            {
                return ResourceRead.GetResourceValue("Asset", ResourceFileName);
            }
        }
        public static string Customer
        {
            get
            {
                return ResourceRead.GetResourceValue("Customer", ResourceFileName);
            }
        }
        public static string WorkOrderType
        {
            get
            {
                return ResourceRead.GetResourceValue("WorkOrderType", ResourceFileName);
            }
        }
        public static string Tool
        {
            get
            {
                return ResourceRead.GetResourceValue("Tool", ResourceFileName);
            }
        }

        public static string LoadMore
        {
            get
            {
                return ResourceRead.GetResourceValue("LoadMore", ResourceFileName);
            }
        }

        public static string LoadAll
        {
            get
            {
                return ResourceRead.GetResourceValue("LoadAll", ResourceFileName);
            }
        }
    }

    public class ResGridHeader
    {
        private static string resourceFile = "ResGridHeader";

        /// <summary>
        ///   Looks up a localized string similar to Copy.
        /// </summary>
        public static string Copy
        {
            get
            {
                return ResourceRead.GetResourceValue("Copy", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CSV.
        /// </summary>
        public static string CSV
        {
            get
            {
                return ResourceRead.GetResourceValue("CSV", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Excel.
        /// </summary>
        public static string Excel
        {
            get
            {
                return ResourceRead.GetResourceValue("Excel", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Go.
        /// </summary>
        public static string Go
        {
            get
            {
                return ResourceRead.GetResourceValue("Go", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Go to page.
        /// </summary>
        public static string GoToPage
        {
            get
            {
                return ResourceRead.GetResourceValue("GoToPage", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PDF.
        /// </summary>
        public static string PDF
        {
            get
            {
                return ResourceRead.GetResourceValue("PDF", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Print.
        /// </summary>
        public static string Print
        {
            get
            {
                return ResourceRead.GetResourceValue("Print", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to records.
        /// </summary>
        public static string Records
        {
            get
            {
                return ResourceRead.GetResourceValue("Records", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Show.
        /// </summary>
        public static string Show
        {
            get
            {
                return ResourceRead.GetResourceValue("Show", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Show.
        /// </summary>
        public static string View
        {
            get
            {
                return ResourceRead.GetResourceValue("View", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Include Archived:.
        /// </summary>
        public static string IncludeArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeArchived", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Include Deleted:.
        /// </summary>
        public static string IncludeDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeDeleted", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AutoRefresh:.
        /// </summary>
        public static string AutoRefresh
        {
            get
            {
                return ResourceRead.GetResourceValue("AutoRefresh", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Refresh:.
        /// </summary>
        public static string Refresh
        {
            get
            {
                return ResourceRead.GetResourceValue("Refresh", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to UDFSetup:.
        /// </summary>
        public static string UDFSetup
        {
            get
            {
                return ResourceRead.GetResourceValue("UDFSetup", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to ViewHistory:.
        /// </summary>
        public static string ViewHistory
        {
            get
            {
                return ResourceRead.GetResourceValue("ViewHistory", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Reorder:.
        /// </summary>
        public static string Reorder
        {
            get
            {
                return ResourceRead.GetResourceValue("Reorder", resourceFile);
            }
        }
        public static string Download
        {
            get
            {
                return ResourceRead.GetResourceValue("Download", resourceFile);
            }
        }
        public static string AddfromCatalog
        {
            get
            {
                return ResourceRead.GetResourceValue("AddfromCatalog", resourceFile);
            }
        }
        public static string ItemPictureview
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemPictureview", resourceFile);
            }
        }
        public static string ItemListView
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemListView", resourceFile);
            }
        }
        public static string ItembinlistView
        {
            get
            {
                return ResourceRead.GetResourceValue("ItembinlistView", resourceFile);
            }
        }

        public static string AddNew
        {
            get
            {
                return ResourceRead.GetResourceValue("AddNew", resourceFile);
            }
        }

        public static string MsgUndeleteSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUndeleteSuccessfully", resourceFile);
            }
        }
        public static string Current { get { return ResourceRead.GetResourceValue("Current", resourceFile); } }
        public static string ReSend { get { return ResourceRead.GetResourceValue("ReSend", resourceFile); } }
        public static string DownloadDocs { get { return ResourceRead.GetResourceValue("DownloadDocs", resourceFile); } }
        public static string Kit { get { return ResourceRead.GetResourceValue("Kit", resourceFile); } }
        public static string SupplierListReport { get { return ResourceRead.GetResourceValue("SupplierListReport", resourceFile); } }
        public static string Projects { get { return ResourceRead.GetResourceValue("Projects", resourceFile); } }
        public static string PDFExport { get { return ResourceRead.GetResourceValue("PDFExport", resourceFile); } }
        public static string PrintChangedonly { get { return ResourceRead.GetResourceValue("PrintChangedonly", resourceFile); } }
        public static string WrittenOffToolsReport { get { return ResourceRead.GetResourceValue("WrittenOffToolsReport", resourceFile); } }
        public static string MaintenanceHistory { get { return ResourceRead.GetResourceValue("MaintenanceHistory", resourceFile); } }
        public static string UndeleteRecord { get { return ResourceRead.GetResourceValue("UndeleteRecord", resourceFile); } }
        public static string Archive { get { return ResourceRead.GetResourceValue("Archive", resourceFile); } }
        public static string UnArchive { get { return ResourceRead.GetResourceValue("UnArchive", resourceFile); } }
        public static string ChangeFontSize { get { return ResourceRead.GetResourceValue("ChangeFontSize", resourceFile); } }
        public static string Large { get { return ResourceRead.GetResourceValue("Large", resourceFile); } }
        public static string Medium { get { return ResourceRead.GetResourceValue("Medium", resourceFile); } }
        public static string Small { get { return ResourceRead.GetResourceValue("Small", resourceFile); } }
        public static string Mobile { get { return ResourceRead.GetResourceValue("Mobile", resourceFile); } }
        public static string Report { get { return ResourceRead.GetResourceValue("Report", resourceFile); } }
        public static string ViewPrint { get { return ResourceRead.GetResourceValue("ViewPrint", resourceFile); } }
        public static string DeSelectAll { get { return ResourceRead.GetResourceValue("DeSelectAll", resourceFile); } }
        public static string IL { get { return ResourceRead.GetResourceValue("IL", resourceFile); } }
        public static string SL { get { return ResourceRead.GetResourceValue("SL", resourceFile); } }
    }

    public class ResLayout
    {

        private static string resourceFile = "ResLayout";

        /// <summary>
        ///   Looks up a localized string similar to Administration.
        /// </summary>
        public static string Administration
        {
            get
            {
                return ResourceRead.GetResourceValue("Administration", resourceFile);
            }
        }
        public static string BillOfMaterial
        {
            get
            {
                return ResourceRead.GetResourceValue("BillOfMaterial", resourceFile);
            }
        }
        public static string BOMCategories
        {
            get
            {
                return ResourceRead.GetResourceValue("BOMCategories", resourceFile);
            }
        }
        public static string BOMInventoryClassification
        {
            get
            {
                return ResourceRead.GetResourceValue("BOMInventoryClassification", resourceFile);
            }
        }
        public static string BOMGLAccounts
        {
            get
            {
                return ResourceRead.GetResourceValue("BOMGLAccounts", resourceFile);
            }
        }
        public static string BOMSuppliers
        {
            get
            {
                return ResourceRead.GetResourceValue("BOMSuppliers", resourceFile);
            }
        }
        public static string BOMUnits
        {
            get
            {
                return ResourceRead.GetResourceValue("BOMUnits", resourceFile);
            }
        }
        public static string BOMManufacturers
        {
            get
            {
                return ResourceRead.GetResourceValue("BOMManufacturers", resourceFile);
            }
        }
        public static string BOMCostUOM
        {
            get
            {
                return ResourceRead.GetResourceValue("BOMCostUOM", resourceFile);
            }
        }
        public static string BOMItem
        {
            get
            {
                return ResourceRead.GetResourceValue("BOMItem", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Another Link.
        /// </summary>
        public static string AnotherLink
        {
            get
            {
                return ResourceRead.GetResourceValue("AnotherLink", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Asset.
        /// </summary>
        public static string Asset
        {
            get
            {
                return ResourceRead.GetResourceValue("Asset", resourceFile);
            }
        }
        public static string CustomiseReport
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomiseReport", resourceFile);
            }
        }
        public static string ReportList
        {
            get
            {
                return ResourceRead.GetResourceValue("ReportList", resourceFile);
            }
        }

        public static string ModuleWiseDefaultPrint
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleWiseDefaultPrint", resourceFile);
            }
        }

        public static string ViewReport
        {
            get
            {
                return ResourceRead.GetResourceValue("ViewReport", resourceFile);
            }
        }
        public static string ScheduleReport
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleReport", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Asset.
        /// </summary>
        public static string Scheduler
        {
            get
            {
                return ResourceRead.GetResourceValue("Scheduler", resourceFile);
            }
        }
        public static string SchedulerMapping
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerMapping", resourceFile);
            }
        }
        public static string Maintenance
        {
            get
            {
                return ResourceRead.GetResourceValue("Maintenance", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Authentication.
        /// </summary>
        public static string Authentication
        {
            get
            {
                return ResourceRead.GetResourceValue("Authentication", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Bins.
        /// </summary>
        public static string Bin
        {
            get
            {
                return ResourceRead.GetResourceValue("Bin", resourceFile);
            }
        }

        public static string FTP
        {
            get
            {
                return ResourceRead.GetResourceValue("FTP", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Categories.
        /// </summary>
        public static string Category
        {
            get
            {
                return ResourceRead.GetResourceValue("Category", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Categories.
        /// </summary>
        public static string CostUOM
        {
            get
            {
                return ResourceRead.GetResourceValue("CostUOM", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Categories.
        /// </summary>
        public static string OrderUOM
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderUOM", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Categories.
        /// </summary>
        public static string InventoryClassification
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryClassification", resourceFile);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Company.
        /// </summary>
        public static string Company
        {
            get
            {
                return ResourceRead.GetResourceValue("Company", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        public static string CompanyLogoAlt
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyLogoAlt", resourceFile);
            }
        }
        public static string BackToPreviousUser
        {
            get
            {
                return ResourceRead.GetResourceValue("BackToPreviousUser", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Consume.
        /// </summary>
        public static string Consume
        {
            get
            {
                return ResourceRead.GetResourceValue("Consume", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Copyright © 2012 eTurns.com. All rights reserved..
        /// </summary>
        public static string Copyright
        {
            get
            {
                return ResourceRead.GetResourceValue("Copyright", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Customers.
        /// </summary>
        public static string Customers
        {
            get
            {
                return ResourceRead.GetResourceValue("Customers", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Customers.
        /// </summary>
        public static string ToolWrittenOffCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolWrittenOffCategory", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Dashboard.
        /// </summary>
        public static string Dashboard
        {
            get
            {
                return ResourceRead.GetResourceValue("Dashboard", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Department.
        /// </summary>
        public static string Department
        {
            get
            {
                return ResourceRead.GetResourceValue("Department", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Enterprise.
        /// </summary>
        public static string Enterprise
        {
            get
            {
                return ResourceRead.GetResourceValue("Enterprise", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Freight Types.
        /// </summary>
        public static string FreightType
        {
            get
            {
                return ResourceRead.GetResourceValue("FreightType", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Freight Types.
        /// </summary>
        public static string ImportMaster
        {
            get
            {
                return ResourceRead.GetResourceValue("ImportMaster", resourceFile);
            }
        }
        public static string ToolCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCategory", resourceFile);
            }
        }

        public static string AssetCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetCategory", resourceFile);
            }
        }

        public static string ExternalUserConfiguration
        {
            get
            {
                return ResourceRead.GetResourceValue("ExternalUserConfiguration", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to G/L Accounts.
        /// </summary>
        public static string GLAccount
        {
            get
            {
                return ResourceRead.GetResourceValue("GLAccount", resourceFile);
            }
        }
        public static string ItemBinList
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemBinList", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to GXPR Consigned Jobs.
        /// </summary>
        public static string GXPRConsignedJob
        {
            get
            {
                return ResourceRead.GetResourceValue("GXPRConsignedJob", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Inventory.
        /// </summary>
        public static string Inventory
        {
            get
            {
                return ResourceRead.GetResourceValue("Inventory", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Item.
        /// </summary>
        public static string Item
        {
            get
            {
                return ResourceRead.GetResourceValue("Item", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Job Types.
        /// </summary>
        public static string JobType
        {
            get
            {
                return ResourceRead.GetResourceValue("JobType", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Kits.
        /// </summary>
        public static string Kit
        {
            get
            {
                return ResourceRead.GetResourceValue("Kit", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Language: .
        /// </summary>
        public static string Language
        {
            get
            {
                return ResourceRead.GetResourceValue("Language", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Locations.
        /// </summary>
        public static string Location
        {
            get
            {
                return ResourceRead.GetResourceValue("Location", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Welcome {0}.
        /// </summary>
        public static string LogInUser
        {
            get
            {
                return ResourceRead.GetResourceValue("LogInUser", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to E Turns Powered.
        /// </summary>
        public static string LogoImgAlt
        {
            get
            {
                return ResourceRead.GetResourceValue("LogoImgAlt", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to E Turns Powered.
        /// </summary>
        public static string LogoTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("LogoTitle", resourceFile);
            }
        }

        public static string eTurnsTrackStockMMTLogoTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("eTurnsTrackStockMMTLogoTitle", resourceFile);
            }
        }

        public static string eTurnsTrackStockMMTLogoImgAlt
        {
            get
            {
                return ResourceRead.GetResourceValue("eTurnsTrackStockMMTLogoImgAlt", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Logout.
        /// </summary>
        public static string Logout
        {
            get
            {
                return ResourceRead.GetResourceValue("Logout", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Manufacturers.
        /// </summary>
        public static string Manufacturer
        {
            get
            {
                return ResourceRead.GetResourceValue("Manufacturer", resourceFile);
            }
        }
        public static string Eula
        {
            get
            {
                return ResourceRead.GetResourceValue("Eula", resourceFile);
            }
        }
        public static string ResourceLanguage
        {
            get
            {
                return ResourceRead.GetResourceValue("ResourceLanguage", resourceFile);
            }
        }
        public static string PullPOMasterList
        {
            get
            {
                return ResourceRead.GetResourceValue("PullPOMasterList", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Masters.
        /// </summary>
        public static string Master
        {
            get
            {
                return ResourceRead.GetResourceValue("Master", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Measurement Terms.
        /// </summary>
        public static string MeasurementTerm
        {
            get
            {
                return ResourceRead.GetResourceValue("MeasurementTerm", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to My Profile.
        /// </summary>
        public static string MyProfile
        {
            get
            {
                return ResourceRead.GetResourceValue("MyProfile", resourceFile);
            }
        }
        public static string UserSetting
        {
            get
            {
                return ResourceRead.GetResourceValue("UserSetting", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OASIS.
        /// </summary>
        public static string OaLogoAlt
        {
            get
            {
                return ResourceRead.GetResourceValue("OaLogoAlt", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Poll All.
        /// </summary>
        public static string PollAll
        {
            get
            {
                return ResourceRead.GetResourceValue("PollAll", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Projects.
        /// </summary>
        public static string Project
        {
            get
            {
                return ResourceRead.GetResourceValue("Project", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Receive.
        /// </summary>
        public static string Receive
        {
            get
            {
                return ResourceRead.GetResourceValue("Receive", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Replenish.
        /// </summary>
        public static string Replenish
        {
            get
            {
                return ResourceRead.GetResourceValue("Replenish", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Reports.
        /// </summary>
        public static string Report
        {
            get
            {
                return ResourceRead.GetResourceValue("Report", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Requisitions.
        /// </summary>
        public static string Requisitions
        {
            get
            {
                return ResourceRead.GetResourceValue("Requisitions", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Resources.
        /// </summary>
        public static string Resource
        {
            get
            {
                return ResourceRead.GetResourceValue("Resource", resourceFile);
            }
        }

        public static string MobileResources
        {
            get
            {
                return ResourceRead.GetResourceValue("MobileResources", resourceFile);
            }
        }
        public static string Barcode
        {
            get
            {
                return ResourceRead.GetResourceValue("Barcode", resourceFile);
            }
        }
        public static string PDAColumnSetUp
        {
            get
            {
                return ResourceRead.GetResourceValue("PDAColumnSetUp", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Users.
        /// </summary>
        public static string Role
        {
            get
            {
                return ResourceRead.GetResourceValue("Role", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Rooms.
        /// </summary>
        public static string Room
        {
            get
            {
                return ResourceRead.GetResourceValue("Room", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Sales.
        /// </summary>
        public static string Sales
        {
            get
            {
                return ResourceRead.GetResourceValue("Sales", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Ship Vias.
        /// </summary>
        public static string ShipVia
        {
            get
            {
                return ResourceRead.GetResourceValue("ShipVia", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Ship Vias.
        /// </summary>
        public static string Vendors
        {
            get
            {
                return ResourceRead.GetResourceValue("Vendors", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Sub-Level Item.
        /// </summary>
        public static string SubLevelItem
        {
            get
            {
                return ResourceRead.GetResourceValue("SubLevelItem", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suppliers.
        /// </summary>
        public static string Supplier
        {
            get
            {
                return ResourceRead.GetResourceValue("Supplier", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Team.
        /// </summary>
        public static string Team
        {
            get
            {
                return ResourceRead.GetResourceValue("Team", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Technicians.
        /// </summary>
        public static string Technician
        {
            get
            {
                return ResourceRead.GetResourceValue("Technician", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tools.
        /// </summary>
        public static string Tool
        {
            get
            {
                return ResourceRead.GetResourceValue("Tool", resourceFile);
            }
        }

        public static string ToolList
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolList", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Units.
        /// </summary>
        public static string Unit
        {
            get
            {
                return ResourceRead.GetResourceValue("Unit", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to User.
        /// </summary>
        public static string User
        {
            get
            {
                return ResourceRead.GetResourceValue("User", resourceFile);
            }
        }

        public static string PermissionTemplate
        {
            get
            {
                return ResourceRead.GetResourceValue("PermissionTemplate", resourceFile);
            }
        }
        public static string NewUserLoginPage
        {
            get
            {
                return ResourceRead.GetResourceValue("NewUserLoginPage", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Units.
        /// </summary>
        public static string StockRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("StockRoom", resourceFile);
            }
        }
        public static string QuickList
        {
            get
            {
                return ResourceRead.GetResourceValue("QuickList", resourceFile);
            }
        }

        public static string ItemMasterList
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemMasterList", resourceFile);
            }
        }

        public static string PullMasterList
        {
            get
            {
                return ResourceRead.GetResourceValue("PullMasterList", resourceFile);
            }
        }
        public static string New
        {
            get
            {
                return ResourceRead.GetResourceValue("New", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to SupplierCatalog.
        /// </summary>
        public static string SupplierCatalog
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierCatalog", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Materialstaging.
        /// </summary>
        public static string Materialstaging
        {
            get
            {
                return ResourceRead.GetResourceValue("Materialstaging", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Materialstaging.
        /// </summary>
        public static string Count
        {
            get
            {
                return ResourceRead.GetResourceValue("Count", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Projects.
        /// </summary>
        public static string ProjectSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpend", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Projects.
        /// </summary>
        public static string Workorders
        {
            get
            {
                return ResourceRead.GetResourceValue("Workorders", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Projects.
        /// </summary>
        public static string Pull
        {
            get
            {
                return ResourceRead.GetResourceValue("Pull", resourceFile);
            }
        }
        public static string Cart
        {
            get
            {
                return ResourceRead.GetResourceValue("Cart", resourceFile);
            }
        }
        public static string Transfer
        {
            get
            {
                return ResourceRead.GetResourceValue("Transfer", resourceFile);
            }
        }
        public static string Orders
        {
            get
            {
                return ResourceRead.GetResourceValue("Orders", resourceFile);
            }
        }
        public static string ToolAssetOrders
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolAssetOrders", resourceFile);
            }
        }
        public static string ReturnOrders
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnOrders", resourceFile);
            }
        }
        public static string Quote
        {
            get
            {
                return ResourceRead.GetResourceValue("Quote", resourceFile);
            }
        }

        public static string QuoteToOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteToOrder", resourceFile);
            }
        }
        public static string Assets
        {
            get
            {
                return ResourceRead.GetResourceValue("Assets", resourceFile);
            }
        }
        public static string AssetMaintenance
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetMaintenance", resourceFile);
            }
        }
        public static string Kits
        {
            get
            {
                return ResourceRead.GetResourceValue("Kits", resourceFile);
            }
        }
        public static string ToolKits
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolKits", resourceFile);
            }
        }
        public static string KitTools
        {
            get
            {
                return ResourceRead.GetResourceValue("KitTools", resourceFile);
            }
        }
        public static string KitToolBuildBreak
        {
            get
            {
                return ResourceRead.GetResourceValue("KitToolBuildBreak", resourceFile);
            }
        }

        public static string WIP
        {
            get
            {
                return ResourceRead.GetResourceValue("WIP", resourceFile);
            }
        }
        public static string EmailConfiguration
        {
            get
            {
                return ResourceRead.GetResourceValue("EmailConfiguration", resourceFile);
            }
        }

        public static string CompanyConfig
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyConfig", resourceFile);
            }
        }

        public static string eVMISetup
        {
            get
            {
                return ResourceRead.GetResourceValue("eVMISetup", resourceFile);
            }
        }

        public static string ToolTipKitMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipKitMenu", resourceFile);
            }
        }

        public static string ToolTipKitSubmenuList
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipKitSubmenuList", resourceFile);
            }
        }


        public static string ToolTipOrderMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipOrderMenu", resourceFile);
            }
        }

        public static string ToolTipOrderSubMenuList
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipOrderSubMenuList", resourceFile);
            }
        }

        public static string ToolTipReqMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipReqMenu", resourceFile);
            }
        }

        public static string ToolTipReqSubMenuList
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipReqSubMenuList", resourceFile);
            }
        }

        public static string ToolTipTransferMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipTransferMenu", resourceFile);
            }
        }

        public static string ToolTipTransferSubMenuList
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipTransferSubMenuList", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Categories.
        /// </summary>
        public static string LabelPrinting
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelPrinting", resourceFile);
            }
        }

        public static string EnterPriseConfiguration
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterPriseConfiguration", resourceFile);
            }
        }



        public static object SiteConfiguration
        {
            get
            {
                return ResourceRead.GetResourceValue("SiteConfiguration", resourceFile);
            }
        }

        public static object RegionalSettings
        {
            get
            {
                return ResourceRead.GetResourceValue("RegionalSettings", resourceFile);
            }
        }

        public static object EnterpriseEmailConfiguration
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterpriseEmailConfiguration", resourceFile);
            }
        }

        public static object eTurnsEmailConfiguration
        {
            get
            {
                return ResourceRead.GetResourceValue("eTurnsEmailConfiguration", resourceFile);
            }
        }

        public static string EnterpriseWebResources
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterpriseWebResources", resourceFile);
            }
        }

        public static string EnterprisePDAResources
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterprisePDAResources", resourceFile);
            }
        }

        public static string eTurnsWebResources
        {
            get
            {
                return ResourceRead.GetResourceValue("eTurnsWebResources", resourceFile);
            }
        }

        public static string SiteSettings
        {
            get
            {
                return ResourceRead.GetResourceValue("SiteSettings", resourceFile);
            }
        }

        public static string DashBoardViewLog
        {
            get
            {
                return ResourceRead.GetResourceValue("DashBoardViewLog", resourceFile);
            }
        }

        public static string BillingTypeModulesMap
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingTypeModulesMap", resourceFile);
            }
        }

        public static string eTurnsBaseResources
        {
            get
            {
                return ResourceRead.GetResourceValue("eTurnsBaseResources", resourceFile);
            }
        }
        public static string eTurnsUDFSetup
        {
            get
            {
                return ResourceRead.GetResourceValue("eTurnsUDFSetup", resourceFile);
            }
        }
        public static string eTurnsUDFSetupEnterprise
        {
            get
            {
                return ResourceRead.GetResourceValue("eTurnsUDFSetupEnterprise", resourceFile);
            }
        }

        public static string eTurnsPDAResources
        {
            get
            {
                return ResourceRead.GetResourceValue("eTurnsPDAResources", resourceFile);
            }
        }

        public static string HelpDocument
        {
            get
            {
                return ResourceRead.GetResourceValue("HelpDocument", resourceFile);
            }
        }

        public static string CatalogReport
        {
            get
            {
                return ResourceRead.GetResourceValue("CatalogReport", resourceFile);
            }
        }

        public static string EturnsGridColumnSetup
        {
            get
            {
                return ResourceRead.GetResourceValue("EturnsGridColumnSetup", resourceFile);
            }
        }
        public static string EnterpriseGridColumnSetup
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterpriseGridColumnSetup", resourceFile);
            }
        }

        public static string SendEmailList
        {
            get
            {
                return ResourceRead.GetResourceValue("SendEmailList", resourceFile);
            }
        }

        public static string DataArchive
        {
            get
            {
                return ResourceRead.GetResourceValue("DataArchive", resourceFile);
            }
        }

        public static string RoomValidationRules
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomValidationRules", resourceFile);
            }
        }

        public static string ValidationRulesMaster
        {
            get
            {
                return ResourceRead.GetResourceValue("ValidationRulesMaster", resourceFile);
            }
        }

        public static string ToolTipReportMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipReportMenu", resourceFile);
            }
        }
        public static string ToolTipCustomiseReportMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipCustomiseReportMenu", resourceFile);
            }
        }
        public static string ToolTipViewReportMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipViewReportMenu", resourceFile);
            }
        }
        public static string ToolTipScheduleReportMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipScheduleReportMenu", resourceFile);
            }
        }
        public static string ToolTipCatalogReportMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipCatalogReportMenu", resourceFile);
            }
        }
        public static string ToolTipReportListMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipReportListMenu", resourceFile);
            }
        }
        public static string ToolTipModuleWiseDefaultPrintMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipModuleWiseDefaultPrintMenu", resourceFile);
            }
        }

        public static string ToolTipDashboardMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipDashboardMenu", resourceFile);
            }
        }
        public static string ToolTipAlertsMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipAlertsMenu", resourceFile);
            }
        }
        public static string ToolTipMinMaxMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipMinMaxMenu", resourceFile);
            }
        }
        public static string ToolTipAdministrationMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipAdministrationMenu", resourceFile);
            }
        }
        public static string ToolTipAuthenticationMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipAuthenticationMenu", resourceFile);
            }
        }
        public static string ToolTipSiteConfigurationMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipSiteConfigurationMenu", resourceFile);
            }
        }
        public static string ToolTipMasterMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipMasterMenu", resourceFile);
            }
        }

        public static string ToolTipBillOfMaterialMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBillOfMaterialMenu", resourceFile);
            }
        }

        public static string ToolTipPollAllMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipPollAllMenu", resourceFile);
            }
        }

        public static string ToolTipToolMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipToolMenu", resourceFile);
            }
        }

        public static string ToolTipAssetsMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipAssetsMenu", resourceFile);
            }
        }

        public static string ToolTipSchedulerMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipSchedulerMenu", resourceFile);
            }
        }

        public static string ToolTipSchedulerMappingMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipSchedulerMappingMenu", resourceFile);
            }
        }
        public static string ToolTipMaintenanceMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipMaintenanceMenu", resourceFile);
            }
        }

        public static string ToolTipToolAssetOrderMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipToolAssetOrderMenu", resourceFile);
            }
        }

        public static string ToolTipToolReceiveMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipToolReceiveMenu", resourceFile);
            }
        }

        public static string ToolTipReplenishMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipReplenishMenu", resourceFile);
            }
        }

        public static string ToolTipToolCartMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipToolCartMenu", resourceFile);
            }
        }

        public static string ToolTipReceiveMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipReceiveMenu", resourceFile);
            }
        }

        public static string ToolTipReturnOrderMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipReturnOrderMenu", resourceFile);
            }
        }
        public static string ToolTipQuoteMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipQuoteMenu", resourceFile);
            }
        }
        public static string ToolTipQuoteToOrderMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipQuoteToOrderMenu", resourceFile);
            }
        }
        public static string ToolTipConsumeMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipConsumeMenu", resourceFile);
            }
        }

        public static string ToolTipPullMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipPullMenu", resourceFile);
            }
        }

        public static string ToolTipWorkorderMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipWorkorderMenu", resourceFile);
            }
        }

        public static string ToolTipProjectSpendMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipProjectSpendMenu", resourceFile);
            }
        }

        public static string ToolTipInventoryMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipInventoryMenu", resourceFile);
            }
        }

        public static string ToolTipItemMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipItemMenu", resourceFile);
            }
        }

        public static string ToolTipQuickListMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipQuickListMenu", resourceFile);
            }
        }
        public static string ToolTipCountMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipCountMenu", resourceFile);
            }
        }

        public static string ToolTipSupplierCatalogMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipSupplierCatalogMenu", resourceFile);
            }
        }
        public static string ToolTipMaterialstagingMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipMaterialstagingMenu", resourceFile);
            }
        }
        public static string ToolTipMoveMaterialMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipMoveMaterialMenu", resourceFile);
            }
        }

        public static string ToolTipToolKitMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipToolKitMenu", resourceFile);
            }
        }

        public static string ToolTipEnterpriseSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipEnterpriseSubMenu", resourceFile);
            }
        }
        public static string ToolTipCompanySubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipCompanySubMenu", resourceFile);
            }
        }

        public static string ToolTipRoomSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipRoomSubMenu", resourceFile);
            }
        }
        public static string ToolTipRoleSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipRoleSubMenu", resourceFile);
            }
        }
        public static string ToolTipUserSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipUserSubMenu", resourceFile);
            }
        }

        public static string ToolTipPermissionTemplateSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipPermissionTemplateSubMenu", resourceFile);
            }
        }
        public static string ToolTipNewUserLoginPageSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipNewUserLoginPageSubMenu", resourceFile);
            }
        }

        public static string ToolTipSendEmailListSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipSendEmailListSubMenu", resourceFile);
            }
        }

        public static string ToolTipBarcodeSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBarcodeSubMenu", resourceFile);
            }
        }
        public static string ToolTipEnterpriseWebResourcesSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipEnterpriseWebResourcesSubMenu", resourceFile);
            }
        }
        public static string ToolTipEnterprisePDAResourcesSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipEnterprisePDAResourcesSubMenu", resourceFile);
            }
        }
        public static string ToolTipeTurnsUDFSetupEnterpriseSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipeTurnsUDFSetupEnterpriseSubMenu", resourceFile);
            }
        }
        public static string ToolTipEnterPriseConfigurationSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipEnterPriseConfigurationSubMenu", resourceFile);
            }
        }
        public static string ToolTipEnterpriseEmailConfigurationSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipEnterpriseEmailConfigurationSubMenu", resourceFile);
            }
        }
        public static string ToolTipEnterpriseGridColumnSetupSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipEnterpriseGridColumnSetupSubMenu", resourceFile);
            }
        }
        public static string ToolTipeTurnsWebResourcesSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipeTurnsWebResourcesSubMenu", resourceFile);
            }
        }
        public static string ToolTipeTurnsPDAResourcesSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipeTurnsPDAResourcesSubMenu", resourceFile);
            }
        }
        public static string ToolTipeTurnsUDFSetupSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipeTurnsUDFSetupSubMenu", resourceFile);
            }
        }
        public static string ToolTipeTurnsEmailConfigurationSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipeTurnsEmailConfigurationSubMenu", resourceFile);
            }
        }
        public static string ToolTipEturnsGridColumnSetupSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipEturnsGridColumnSetupSubMenu", resourceFile);
            }
        }
        public static string ToolTipeVMISetupSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipeVMISetupSubMenu", resourceFile);
            }
        }
        public static string ToolTipHelpDocumentSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipHelpDocumentSubMenu", resourceFile);
            }
        }
        public static string ToolTipImportMasterSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipImportMasterSubMenu", resourceFile);
            }
        }
        public static string ToolTipLabelPrintingSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipLabelPrintingSubMenu", resourceFile);
            }
        }

        public static string ToolTipBaseLabelTemplateSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBaseLabelTemplateSubMenu", resourceFile);
            }
        }
        public static string ToolTipBaseCatalogReportTemplateSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBaseCatalogReportTemplateSubMenu", resourceFile);
            }
        }
        public static string ToolTipMobileResourcesSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipMobileResourcesSubMenu", resourceFile);
            }
        }
        public static string ToolTipPDAColumnSetUpSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipPDAColumnSetUpSubMenu", resourceFile);
            }
        }
        public static string ToolTipRegionalSettingSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipRegionalSettingSubMenu", resourceFile);
            }
        }
        public static string ToolTipResourceSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipResourceSubMenu", resourceFile);
            }
        }
        public static string ToolTipCultureSetupSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipCultureSetupSubMenu", resourceFile);
            }
        }

        public static string ToolTipDataArchiveSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipDataArchiveSubMenu", resourceFile);
            }
        }

        public static string ToolTipRoomValidationRulesSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipRoomValidationRulesSubMenu", resourceFile);
            }
        }
        public static string ToolTipValidationRulesMasterSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipValidationRulesMasterSubMenu", resourceFile);
            }
        }
        public static string ToolTipSiteSettingsSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipSiteSettingsSubMenu", resourceFile);
            }
        }
        public static string ToolTipDashBoardViewLogSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipDashBoardViewLogSubMenu", resourceFile);
            }
        }
        public static string ToolTipBillingTypeModulesMapSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBillingTypeModulesMapSubMenu", resourceFile);
            }
        }
        public static string ToolTipBinSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBinSubMenu", resourceFile);
            }
        }
        public static string ToolTipCategorySubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipCategorySubMenu", resourceFile);
            }
        }
        public static string ToolTipCustomerSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipCustomerSubMenu", resourceFile);
            }
        }
        public static string ToolTipWrittenOffCategorySubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipWrittenOffCategorySubMenu", resourceFile);
            }
        }
        public static string ToolTipCostUOMSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipCostUOMSubMenu", resourceFile);
            }
        }
        public static string ToolTipInventoryClassificationSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipInventoryClassificationSubMenu", resourceFile);
            }
        }
        public static string ToolTipGLAccountSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipGLAccountSubMenu", resourceFile);
            }
        }
        public static string ToolTipGXPRConsignedJobSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipGXPRConsignedJobSubMenu", resourceFile);
            }
        }
        public static string ToolTipItemBinListSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipItemBinListSubMenu", resourceFile);
            }
        }
        public static string ToolTipJobTypeSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipJobTypeSubMenu", resourceFile);
            }
        }
        public static string ToolTipSupplierSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipSupplierSubMenu", resourceFile);
            }
        }
        public static string ToolTipShipViaSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipShipViaSubMenu", resourceFile);
            }
        }
        public static string ToolTipVendorSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipVendorSubMenu", resourceFile);
            }
        }
        public static string ToolTipTechnicianSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipTechnicianSubMenu", resourceFile);
            }
        }
        public static string ToolTipUnitSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipUnitSubMenu", resourceFile);
            }
        }
        public static string ToolTipLocationSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipLocationSubMenu", resourceFile);
            }
        }
        public static string ToolTipToolCategorySubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipToolCategorySubMenu", resourceFile);
            }
        }
        public static string ToolTipAssetCategorySubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipAssetCategorySubMenu", resourceFile);
            }
        }
        public static string ToolTipManufacturerSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipManufacturerSubMenu", resourceFile);
            }
        }

        public static string ToolTipEulaSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipEulaSubMenu", resourceFile);
            }
        }
        public static string ToolTipResourceLanguageSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipResourceLanguageSubMenu", resourceFile);
            }
        }
        public static string ToolTipPullPOMasterListSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipPullPOMasterListSubMenu", resourceFile);
            }
        }
        public static string ToolTipBOMCategoriesSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBOMCategoriesSubMenu", resourceFile);
            }
        }
        public static string ToolTipBOMInventoryClassificationSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBOMInventoryClassificationSubMenu", resourceFile);
            }
        }
        public static string ToolTipBOMGLAccountsSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBOMGLAccountsSubMenu", resourceFile);
            }
        }
        public static string ToolTipBOMSuppliersSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBOMSuppliersSubMenu", resourceFile);
            }
        }
        public static string ToolTipBOMUnitsSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBOMUnitsSubMenu", resourceFile);
            }
        }
        public static string ToolTipBOMManufacturersSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBOMManufacturersSubMenu", resourceFile);
            }
        }
        public static string ToolTipBOMCostUOMSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBOMCostUOMSubMenu", resourceFile);
            }
        }
        public static string ToolTipBOMItemSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipBOMItemSubMenu", resourceFile);
            }
        }

        public static string ToolTipFTPSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipFTPSubMenu", resourceFile);
            }
        }

        public static string ToolTipPollRequestMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipPollRequestMenu", resourceFile);
            }
        }

        public static string ToolTipTareRequestMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipTareRequestMenu", resourceFile);
            }
        }

        public static string PollRequest
        {
            get
            {
                return ResourceRead.GetResourceValue("PollRequest", resourceFile);
            }
        }

        public static string TareRequest
        {
            get
            {
                return ResourceRead.GetResourceValue("TareRequest", resourceFile);
            }
        }


        public static string ToolTipWeightRequestMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipWeightRequestMenu", resourceFile);
            }
        }

        public static string WeightRequest
        {
            get
            {
                return ResourceRead.GetResourceValue("WeightRequest", resourceFile);
            }
        }

        public static string ToolTipItemWrightPerPieceRequestMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipItemWrightPerPieceRequestMenu", resourceFile);
            }
        }

        public static string ItemWeightPerPieceRequest
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemWeightPerPieceRequest", resourceFile);
            }
        }
        public static string QuickBookSetup
        {
            get
            {
                return ResourceRead.GetResourceValue("QuickBookSetup", resourceFile);
            }
        }

        public static string TareAll
        {
            get
            {
                return ResourceRead.GetResourceValue("TareAll", resourceFile);
            }
        }

        public static string CalibrateAll
        {
            get
            {
                return ResourceRead.GetResourceValue("CalibrateAll", resourceFile);
            }
        }

        public static string eVMIRequest
        {
            get
            {
                return ResourceRead.GetResourceValue("eVMIRequest", resourceFile);
            }
        }

        public static string ToolTipeVMIRequestSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipeVMIRequestSubMenu", resourceFile);
            }
        }

        public static string ToolTipResetRequestMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipResetRequestMenu", resourceFile);
            }
        }

        public static string ResetRequest
        {
            get
            {
                return ResourceRead.GetResourceValue("ResetRequest", resourceFile);
            }
        }

        public static string ToolTipShelfRequestMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipShelfRequestMenu", resourceFile);
            }
        }

        public static string ShelfRequest
        {
            get
            {
                return ResourceRead.GetResourceValue("ShelfRequest", resourceFile);
            }
        }

        public static string ToolTipCalibrateRequestMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipCalibrateRequestMenu", resourceFile);
            }
        }

        public static string ToolTipeVMICOMCommonRequestMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipeVMICOMCommonRequestMenu", resourceFile);
            }
        }

        public static string ToolTipCalibrationWeightRequestMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipCalibrationWeightRequestMenu", resourceFile);
            }
        }
        public static string CalibrateRequest
        {
            get
            {
                return ResourceRead.GetResourceValue("CalibrateRequest", resourceFile);
            }
        }


        public static string CalibrationWeightRequest
        {
            get
            {
                return ResourceRead.GetResourceValue("CalibrationWeightRequest", resourceFile);
            }
        }

        public static string eVMICOMCommonRequest
        {
            get
            {
                return ResourceRead.GetResourceValue("eVMICOMCommonRequest", resourceFile);
            }
        }


        public static string EnterpriseQuickList
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterpriseQuickList", resourceFile);
            }
        }

        public static string ToolTipEnterpriseQuickListMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipEnterpriseQuickListMenu", resourceFile);
            }
        }

        public static string Poll
        {
            get
            {
                return ResourceRead.GetResourceValue("Poll", resourceFile);
            }
        }


        public static string Tare
        {
            get
            {
                return ResourceRead.GetResourceValue("Tare", resourceFile);
            }
        }

        public static string Calibrate
        {
            get
            {
                return ResourceRead.GetResourceValue("Calibrate", resourceFile);
            }
        }
        public static string GetWeightPerPiece
        {
            get
            {
                return ResourceRead.GetResourceValue("GetWeightPerPiece", resourceFile);
            }
        }
        public static string ResetScale
        {
            get
            {
                return ResourceRead.GetResourceValue("ResetScale", resourceFile);
            }
        }

        public static string GetShelfID
        {
            get
            {
                return ResourceRead.GetResourceValue("GetShelfID", resourceFile);
            }
        }
        public static string SetShelfID
        {
            get
            {
                return ResourceRead.GetResourceValue("SetShelfID", resourceFile);
            }
        }
        public static string GetVersion
        {
            get
            {
                return ResourceRead.GetResourceValue("GetVersion", resourceFile);
            }
        }

        public static string GetSerialNo
        {
            get
            {
                return ResourceRead.GetResourceValue("GetSerialNo", resourceFile);
            }
        }
        public static string GetModel
        {
            get
            {
                return ResourceRead.GetResourceValue("GetModel", resourceFile);
            }
        }


        public static string SetModel
        {
            get
            {
                return ResourceRead.GetResourceValue("SetModel", resourceFile);
            }
        }

        public static string GetCalibrateWeight
        {
            get
            {
                return ResourceRead.GetResourceValue("GetCalibrateWeight", resourceFile);
            }
        }
        public static string SetCalibrateWeight
        {
            get
            {
                return ResourceRead.GetResourceValue("SetCalibrateWeight", resourceFile);
            }
        }
        public static string Scripts
        {
            get
            {
                return ResourceRead.GetResourceValue("Scripts", resourceFile);
            }
        }
        public static string EmailTOCCValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("EmailTOCCValidation", resourceFile);
            }
        }
        public static string DatabaseScript
        {
            get
            {
                return ResourceRead.GetResourceValue("DatabaseScript", resourceFile);
            }
        }
        public static string ScriptExecution
        {
            get
            {
                return ResourceRead.GetResourceValue("ScriptExecution", resourceFile);
            }
        }
        public static string StoredProcedureExecution
        {
            get
            {
                return ResourceRead.GetResourceValue("StoredProcedureExecution", resourceFile);
            }
        }
        public static string DeployPackageSetup
        {
            get
            {
                return ResourceRead.GetResourceValue("DeployPackageSetup", resourceFile);
            }
        }
        public static string BaseLabelsTemplates
        {
            get
            {
                return ResourceRead.GetResourceValue("BaseLabelsTemplates", resourceFile);
            }
        }
        public static string BaseCatalogReportsTemplates
        {
            get
            {
                return ResourceRead.GetResourceValue("BaseCatalogReportsTemplates", resourceFile);
            }
        }

        public static string IndexGroupBy { get { return ResourceRead.GetResourceValue("IndexGroupBy", resourceFile); } }
        public static string SortCatalogBy { get { return ResourceRead.GetResourceValue("SortCatalogBy", resourceFile); } }
        public static string Ascending { get { return ResourceRead.GetResourceValue("Ascending", resourceFile); } }
        public static string Descending { get { return ResourceRead.GetResourceValue("Descending", resourceFile); } }
        public static string IncludeInactiveItems { get { return ResourceRead.GetResourceValue("IncludeInactiveItems", resourceFile); } }
        public static string CatalogReportTemplateList { get { return ResourceRead.GetResourceValue("CatalogReportTemplateList", resourceFile); } }
        public static string ReportTitle { get { return ResourceRead.GetResourceValue("ReportTitle", resourceFile); } }
        public static string PDAColumnsSettingsPageHeader { get { return ResourceRead.GetResourceValue("PDAColumnsSettingsPageHeader", resourceFile); } }
        public static string PDAColumnsSettingsPageTitle { get { return ResourceRead.GetResourceValue("PDAColumnsSettingsPageTitle", resourceFile); } }
        public static string ListName { get { return ResourceRead.GetResourceValue("ListName", resourceFile); } }
        public static string SelectList { get { return ResourceRead.GetResourceValue("SelectList", resourceFile); } }
        public static string PleaseSelectList { get { return ResourceRead.GetResourceValue("PleaseSelectList", resourceFile); } }
        public static string SourceColumnsList { get { return ResourceRead.GetResourceValue("SourceColumnsList", resourceFile); } }
        public static string DestinationColumnsList { get { return ResourceRead.GetResourceValue("DestinationColumnsList", resourceFile); } }
        public static string UserList { get { return ResourceRead.GetResourceValue("UserList", resourceFile); } }
        public static string CultureSettingsPageHeader { get { return ResourceRead.GetResourceValue("CultureSettingsPageHeader", resourceFile); } }
        public static string CultureSettingsPageTitle { get { return ResourceRead.GetResourceValue("CultureSettingsPageTitle", resourceFile); } }
        public static string QuickBookSetupPageTitle { get { return ResourceRead.GetResourceValue("QuickBookSetupPageTitle", resourceFile); } }
        public static string QuickBookSetupPageHeader { get { return ResourceRead.GetResourceValue("QuickBookSetupPageHeader", resourceFile); } }
        public static string SetupInfo { get { return ResourceRead.GetResourceValue("SetupInfo", resourceFile); } }
        public static string ConnectionSuccessful { get { return ResourceRead.GetResourceValue("ConnectionSuccessful", resourceFile); } }
        public static string ZeroProperties { get { return ResourceRead.GetResourceValue("ZeroProperties", resourceFile); } }
        public static string ResourceFileName { get { return ResourceRead.GetResourceValue("ResourceFileName", resourceFile); } }
        public static string Property { get { return ResourceRead.GetResourceValue("Property", resourceFile); } }
        public static string IsRequired { get { return ResourceRead.GetResourceValue("IsRequired", resourceFile); } }
        public static string DisplayOrder { get { return ResourceRead.GetResourceValue("DisplayOrder", resourceFile); } }
        public static string RefreshRoomRules { get { return ResourceRead.GetResourceValue("RefreshRoomRules", resourceFile); } }
        public static string DTOName { get { return ResourceRead.GetResourceValue("DTOName", resourceFile); } }
        public static string DTOProperty { get { return ResourceRead.GetResourceValue("DTOProperty", resourceFile); } }
        public static string RefreshRoomRulesConfirmationMsg { get { return ResourceRead.GetResourceValue("RefreshRoomRulesConfirmationMsg", resourceFile); } }
        public static string RegionInfo { get { return ResourceRead.GetResourceValue("RegionInfo", resourceFile); } }
        public static string ABIntegration { get { return ResourceRead.GetResourceValue("ABIntegration", resourceFile); } }
        public static string ProductList { get { return ResourceRead.GetResourceValue("ProductList", resourceFile); } }
        public static string ProductGrid { get { return ResourceRead.GetResourceValue("ProductGrid", resourceFile); } }
        public static string ToolTipProductListMenu { get { return ResourceRead.GetResourceValue("ToolTipProductListMenu", resourceFile); } }
        public static string ToolTipProductGridMenu { get { return ResourceRead.GetResourceValue("ToolTipProductGridMenu", resourceFile); } }
        public static string RoomProductList { get { return ResourceRead.GetResourceValue("RoomProductList", resourceFile); } }
        public static string RoomProductGrid { get { return ResourceRead.GetResourceValue("RoomProductGrid", resourceFile); } }
        public static string ToolTipRoomProductListMenu { get { return ResourceRead.GetResourceValue("ToolTipRoomProductListMenu", resourceFile); } }
        public static string ToolTipRoomProductGridMenu { get { return ResourceRead.GetResourceValue("ToolTipRoomProductGridMenu", resourceFile); } }

        public static string CopyrightBorderState
        {
            get
            {
                return ResourceRead.GetResourceValue("CopyrightBorderState", resourceFile);
            }
        }

        public static string ToolTipMultiRoomImportMasterSubMenu
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTipMultiRoomImportMasterSubMenu", resourceFile);
            }
        }
        public static string MultiRoomImportMaster
        {
            get
            {
                return ResourceRead.GetResourceValue("MultiRoomImportMaster", resourceFile);
            }
        }
    }

    public class ResResourceEditor
    {


        private static string resourceFile = "ResResourceEditor";



        /// <summary>
        ///   Looks up a localized string similar to Language: .
        /// </summary>
        public static string Language
        {
            get
            {
                return ResourceRead.GetResourceValue("Language", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Load.
        /// </summary>
        public static string Load
        {
            get
            {
                return ResourceRead.GetResourceValue("Load", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Mdodules: .
        /// </summary>
        public static string Mdodules
        {
            get
            {
                return ResourceRead.GetResourceValue("Mdodules", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Resource Editor.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }
        public static string ENTPageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterprisePageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Show and Edit Reources file.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Resource Editor.
        /// </summary>
        public static string PageHeaderMobile
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeaderMobile", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Show and Edit Reources file.
        /// </summary>
        public static string PageTitleMobile
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitleMobile", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Key.
        /// </summary>
        public static string ResourceKey
        {
            get
            {
                return ResourceRead.GetResourceValue("ResourceKey", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Resources: .
        /// </summary>
        public static string Resources
        {
            get
            {
                return ResourceRead.GetResourceValue("Resources", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Value.
        /// </summary>
        public static string AcrossEnterprise
        {
            get
            {
                return ResourceRead.GetResourceValue("AcrossEnterprise", resourceFile);
            }
        }
        public static string AcrossETurns
        {
            get
            {
                return ResourceRead.GetResourceValue("AcrossETurns", resourceFile);
            }
        }

        public static string ResourceValue
        {
            get
            {
                return ResourceRead.GetResourceValue("ResourceValue", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to ResetResourceConfirmMsg.
        /// </summary>
        public static string ResetResourceConfirmMsg
        {
            get
            {
                return ResourceRead.GetResourceValue("ResetResourceConfirmMsg", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Value.
        /// </summary>
        public static string UploadSuccess
        {
            get
            {
                return ResourceRead.GetResourceValue("UploadSuccess", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Value.
        /// </summary>
        public static string UploadNotSuccess
        {
            get
            {
                return ResourceRead.GetResourceValue("UploadNotSuccess", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ResetToOrginal.
        /// </summary>
        public static string ResetToOrginal
        {
            get
            {
                return ResourceRead.GetResourceValue("ResetToOrginal", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Value.
        /// </summary>
        public static string ImportSuccess
        {
            get
            {
                return ResourceRead.GetResourceValue("ImportSuccess", resourceFile);
            }
        }
        public static string msgfor
        {
            get
            {
                return ResourceRead.GetResourceValue("msgfor", resourceFile);
            }
        }
        public static string msgInsertforAllResources
        {
            get
            {
                return ResourceRead.GetResourceValue("msgInsertforAllResources", resourceFile);
            }
        }

        public static string DoubleClickToEditValue { get { return ResourceRead.GetResourceValue("DoubleClickToEditValue", resourceFile); } }
        public static string ReserCurrentPageResourceConfirmation { get { return ResourceRead.GetResourceValue("ReserCurrentPageResourceConfirmation", resourceFile); } }
        public static string Page { get { return ResourceRead.GetResourceValue("Page", resourceFile); } }
        public static string SelectResourceFile { get { return ResourceRead.GetResourceValue("SelectResourceFile", resourceFile); } }
        public static string Import { get { return ResourceRead.GetResourceValue("Import", resourceFile); } }
        public static string Export { get { return ResourceRead.GetResourceValue("Export", resourceFile); } }
        public static string ImportNewResourceFile { get { return ResourceRead.GetResourceValue("ImportNewResourceFile", resourceFile); } }
        public static string PageHeaderBaseResourceEditor { get { return ResourceRead.GetResourceValue("PageHeaderBaseResourceEditor", resourceFile); } }
        public static string PageHeaderBaseMobileResourceEditor { get { return ResourceRead.GetResourceValue("PageHeaderBaseMobileResourceEditor", resourceFile); } }
    }

    /*
public class MyLocalizationDisplayNameAttribute : DisplayNameAttribute
{
    private DisplayAttribute display;

    public MyLocalizationDisplayNameAttribute(string resourceName, string ResourceFileName)
    {
        Type t = Type.GetType(CommonDTO.RText + "." + ResourceFileName);
        this.display = new DisplayAttribute()
        {
            ResourceType = t,
            Name = resourceName
        };
    }

    public override string DisplayName
    {
        get
        {
            return display.GetName();
        }
    }
}

public class LocalizationDisplayNameAttribute : DisplayNameAttribute
{
    private string ResourceKey;
    private string ResourceFile;
    public LocalizationDisplayNameAttribute(string resourceName, string ResourceFileName)
    {
        ResourceKey = resourceName;
        ResourceFile = ResourceFileName;
    }

    public override string DisplayName
    {
        get
        {
            return ResourceHelper.GetResourceValue(CommonDTO.RText, ResourceFile, ResourceKey);
        }
    }
}

public class LocalizationStringLengthAttribute : StringLengthAttribute //,IClientValidatable
{
    private string ResourceKey;
    private string ResourceFile;

    public LocalizationStringLengthAttribute(int MaxNumber, string resourceFileName, string resourceKey)
        : base(MaxNumber)
    {
        ResourceKey = resourceKey;
        ResourceFile = resourceFileName;
        this.ErrorMessage = ResourceHelper.GetResourceValue(CommonDTO.RText, ResourceFile, ResourceKey);
    }

    public override bool IsValid(object value)
    {
        this.ErrorMessage = ResourceHelper.GetResourceValue(CommonDTO.RText, ResourceFile, ResourceKey);
        return base.IsValid(value);
    }


}

public sealed class RequiredLocalized : RequiredAttribute
{
    //private string ResourceKey;
    //private string ResourceFile;

    //public RequiredLocalized(string filename, string keyname)
    //{
    //    ResourceKey = keyname;
    //    ResourceFile = filename;
    //}

    public override bool IsValid(object value)
    {

        if (!(ErrorMessageResourceType == null || String.IsNullOrWhiteSpace(ErrorMessageResourceName)))
        {
            //this.ErrorMessage = ResourceHelper.GetResourceValue(CommonDTO.RText, ResourceFile, ResourceKey);
            this.ErrorMessage = ResourceHelper.GetResourceValue(CommonDTO.RText, "BinMaster", "ermsgMaxLength");
            this.ErrorMessageResourceType = null;
            this.ErrorMessageResourceName = null;
        }
        return base.IsValid(value);
    }
}



public class LocaleProviderFactory : ResourceProviderFactory
{
public override IResourceProvider CreateGlobalResourceProvider(string classKey)
{
    if (classKey != string.Empty)
    {
        if (classKey.Substring(0, 1) != "/")
        {
            classKey = "/" + classKey;
        }
    }
    GlobalLocaleProvider obj = new GlobalLocaleProvider(classKey);
    return obj;
}

public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
{
    virtualPath = virtualPath.ToLower();
    return new LocalLocaleProvider(virtualPath);
}
}

public class GlobalLocaleProvider : IResourceProvider
{
private readonly string _virtualPath = "n/a";

public GlobalLocaleProvider(string classKey)
{
    _virtualPath = classKey;
}

public object GetObject(string s, CultureInfo ci)
{
    return ResourceHelper.GetResourceValue(CommonDTO.RText, _virtualPath, s); //, Configuration.Websites.CurrentWebsite
}

public IResourceReader ResourceReader
{
    get
    {
        return null;
    }
}
}



public class LocalLocaleProvider : IResourceProvider, IImplicitResourceProvider
{
private readonly Hashtable ht = new Hashtable();

private readonly string _virtualPath = "n/a";

public LocalLocaleProvider(string virtualPath)
{

    _virtualPath = virtualPath;

    ht.Add("lVanResource1", "value");
}

public object GetObject(ImplicitResourceKey irk, CultureInfo ci)
{
    // REVIEW: Why? 

    return null;
}

public object GetObject(string s, CultureInfo ci)
{
    try
    {
        return ResourceHelper.GetResourceValue(CommonDTO.RText, _virtualPath, s); //, Configuration.Websites.CurrentWebsite
    }
    catch (Exception ex)
    {
        throw ex;
    }
}

public ICollection GetImplicitResourceKeys(string KeyPrefix)
{
    try
    {
        List<ImplicitResourceKey> keys = new List<ImplicitResourceKey>();

        IDictionaryEnumerator Enumerator = this.ResourceReader.GetEnumerator();
        if (Enumerator == null)
            return keys; // Cannot return null!


        foreach (DictionaryEntry dictentry in this.ResourceReader)
        {
            string key = (string)dictentry.Key;


            if (key.StartsWith(KeyPrefix + ".", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                string keyproperty = String.Empty;
                if (key.Length > (KeyPrefix.Length + 1))
                {
                    int pos = key.IndexOf('.');
                    if ((pos > 0) && (pos == KeyPrefix.Length))
                    {
                        keyproperty = key.Substring(pos + 1);
                        if (String.IsNullOrEmpty(keyproperty) == false)
                        {
                            ImplicitResourceKey implicitkey = new ImplicitResourceKey(String.Empty, KeyPrefix, keyproperty);
                            keys.Add(implicitkey);
                        }
                    }
                }
            }
        }
        return keys;
    }
    catch (Exception ex)
    {
        throw ex;
    }
}

private LocalResourceReader _ResourceReader = null;
public IResourceReader ResourceReader
{
    get
    {
        //return new LocalResourceReader(ht);
        if (this._ResourceReader != null)
            return this._ResourceReader as IResourceReader;

        this._ResourceReader = new LocalResourceReader(GetResourceCache(null));
        return this._ResourceReader as IResourceReader;
    }
}

private IDictionary _resourceCache;
private IDictionary GetResourceCache(string cultureName)
{
    try
    {
        //if (cultureName == null)
        //    cultureName = ((CultureInfo)Resource.CurrentUICulture()).Name;

        //if (this._resourceCache == null)
        //    this._resourceCache = new ListDictionary();

        //IDictionary Resources = this._resourceCache[cultureName] as IDictionary;
        //if (Resources == null)
        //{
        //    if (this._resourceCache.Contains(cultureName))
        //        Resources = this._resourceCache[cultureName] as IDictionary;
        //    else
        //        Resources = GetResourceSet(cultureName as string, this._virtualPath);
        //    this._resourceCache[cultureName] = Resources;

        //}

        return null;
    }
    catch (Exception ex)
    {
        throw ex;
    }
}

public IDictionary GetResourceSet(string CultureName, string ResourceSet)
{
    try
    {
        // return dictentry as IDictionary;
        return null;
    }
    catch (Exception ex)
    {
        throw ex;
    }
}

private Dictionary<string, XmlDocument> InsertCacheResources(string CultureName, string ResourceSet)
{
    try
    {
        //XmlDocument ds = null;
        //string RelativePath = ResourceSet;
        //SessionManager sessionmanager = new SessionManager();

        //Dictionary<string, XmlDocument> objDict = (Dictionary<string, XmlDocument>)sessionmanager.GetCache("ResxList");
        //if (objDict == null)
        //    objDict = new Dictionary<string, XmlDocument>();

        //ds = ResourceHelper.GetResourceValue(RelativePath);
        //if (ds != null)
        //{
        //    objDict.Add(RelativePath, ds);
        //    sessionmanager.Insert("ResxList", objDict);
        //}
        //return objDict;
    }
    catch (Exception ex)
    {
        throw ex;
    }
    return null;
}

}

public class LocalResourceReader : IResourceReader
{
private readonly IDictionary _resources;

public LocalResourceReader(IDictionary resources)
{
    _resources = resources;
}

IDictionaryEnumerator IResourceReader.GetEnumerator()
{
    return _resources.GetEnumerator();
}

void IResourceReader.Close()
{
}

IEnumerator IEnumerable.GetEnumerator()
{
    return _resources.GetEnumerator();
}

void IDisposable.Dispose()
{
}


}



public class Resource
{
private static string _virtualPath = "n/a";

public Resource() : base() { }

private string _KeyName = "";
public string KeyName
{
    get { return _KeyName; }
    set { _KeyName = value; }
}
private string _VirtualPath = "";
public string VirtualPath
{
    get { return _VirtualPath; }
    set { _VirtualPath = value; }
}


public Dictionary<string, string> _ValueList = new Dictionary<string, string>();
public Dictionary<string, string> ValuesList
{
    get { return _ValueList; }
    set { _ValueList = value; }
}


private const string GLOBALRESOURCEINDICATOR = "/App_GlobalResources";

public static Dictionary<string, Dictionary<string, Resource>> _resourceCollection = new Dictionary<string, Dictionary<string, Resource>>();

private static readonly object lockForPreloadResources = new object();

private static void preloadResources()
{
    lock (lockForPreloadResources)
    {

        if (_resourceCollection == null || _resourceCollection.Count <= 0)
        {
            _resourceCollection = new Dictionary<string, Dictionary<string, Resource>>(StringComparer.InvariantCultureIgnoreCase);

            ArrayList resourceList = new Resource().RetrieveList();

            // splits de resources uit naar VirtualPath (waar String.Empty een collection is voor de global resources)

            foreach (Resource rsrc in resourceList)
            {
                string virtualPath;

                if (string.IsNullOrEmpty(rsrc.VirtualPath))
                    virtualPath = GLOBALRESOURCEINDICATOR.ToLower();
                else
                    virtualPath = rsrc.VirtualPath.ToLower();

                if (_resourceCollection.ContainsKey(virtualPath) == false)
                {
                    // Maak een collection aan voor deze virtualpath
                    var newCollection = new Dictionary<string, Resource>(StringComparer.InvariantCultureIgnoreCase);
                    _resourceCollection.Add(virtualPath, newCollection);
                }

                Dictionary<string, Resource> collection = _resourceCollection[virtualPath];

                try
                {
                    collection.Add(rsrc.KeyName, rsrc);
                }
                catch (ArgumentException)
                {
                    // An item with the same name has already been added.

                    // TODO: Eigenlijk loggen.
                }
            }
        }
    }
}

private static void preloadResources(string vertualPath)
{
    lock (lockForPreloadResources)
    {

        if (_resourceCollection == null || _resourceCollection.Count <= 0)
        {
            _resourceCollection = new Dictionary<string, Dictionary<string, Resource>>(StringComparer.InvariantCultureIgnoreCase);
        }

        ArrayList resourceList = new Resource().RetrieveList(vertualPath);


        // splits de resources uit naar VirtualPath (waar String.Empty een collection is voor de global resources)

        foreach (Resource rsrc in resourceList)
        {
            string virtualPath;

            if (string.IsNullOrEmpty(rsrc.VirtualPath))
                virtualPath = GLOBALRESOURCEINDICATOR.ToLower();
            else
                virtualPath = rsrc.VirtualPath.ToLower();

            if (_resourceCollection.ContainsKey(virtualPath) == false)
            {
                // Maak een collection aan voor deze virtualpath
                var newCollection = new Dictionary<string, Resource>(StringComparer.InvariantCultureIgnoreCase);
                _resourceCollection.Add(virtualPath, newCollection);
            }

            Dictionary<string, Resource> collection = _resourceCollection[virtualPath];

            try
            {
                collection.Add(rsrc.KeyName, rsrc);
            }
            catch (ArgumentException)
            {
                // An item with the same name has already been added.

                // TODO: Eigenlijk loggen.
            }
        }

    }
}

///// <summary>
///// Add Resource to global resources
///// </summary>
///// <param name="resource"></param>
public static void AddGlobalResource(Resource resource)
{
    var global = GLOBALRESOURCEINDICATOR.ToLower();
    if (_resourceCollection == null)
        preloadResources();
    if (!_resourceCollection[global].ContainsKey(resource.KeyName))
        _resourceCollection[global].Add(resource.KeyName, resource);
}

public static void ClearResources()
{
    _resourceCollection.Clear();
    _resourceCollection = null;
}

public static string GetGlobalResource(string ObjectName, string VirtualPath, CultureInfo ci) //, Website w
{

    if (VirtualPath == "")
    {
        return GetLocalResource(ObjectName, null, ci);
    }
    else if (VirtualPath.ToLower().Contains("/webresource"))
    {
        return GetLocalResource(ObjectName, null, ci);
    }
    else if (VirtualPath.ToLower().Contains("/global"))
    {
        return GetLocalResource(ObjectName, null, ci);
    }
    else if (VirtualPath.ToLower().Contains("n/a"))
    {
        return GetLocalResource(ObjectName, null, ci);
    }
    else if (VirtualPath.ToLower().Contains("/app_globalresources"))
    {
        return GetLocalResource(ObjectName, null, ci);
    }
    else
    {
        return GetLocalResource(ObjectName, VirtualPath, ci);
    }

}

public static string GetLocalResource(string ObjectName, string VirtualPath, CultureInfo ci) //, Website w
{

    if (_resourceCollection == null || _resourceCollection.Count == 0)
        preloadResources();

    if (string.IsNullOrEmpty(VirtualPath))
        VirtualPath = GLOBALRESOURCEINDICATOR;

    VirtualPath = VirtualPath.ToLower();

    if (ci == null || ci.TwoLetterISOLanguageName == "iv")
        ci = CurrentUICulture();

    if (_resourceCollection.ContainsKey(VirtualPath.ToLower()) == false)
    {
        preloadResources();
        //throw new Exception(String.Format("No resource collection exists for '{0}'", VirtualPath));
    }
    Dictionary<string, Resource> collection = _resourceCollection[VirtualPath];

    if (collection.ContainsKey(ObjectName) == false)
        return suggestTranslation(ObjectName);
    // throw new Exception(String.Format("No resource '{0}' exists in collection '{1}'", ObjectName, VirtualPath));

    Resource resource;

    try
    {
        resource = collection[ObjectName];
    }
    catch
    {
        return ""; //suggestTranslation(ObjectName);
    }

    string Value = "";
    string CultName = string.Empty;
    if (ci.Name == "nl-NL")
        CultName = "trans_nl";
    else if (ci.Name == "de-DE")
        CultName = "trans_de";
    else if (ci.Name == "fr-FR")
        CultName = "trans_fr";
    else if (ci.Name == "fr-FR")
        CultName = "trans_fr";
    else if (ci.Name == "en-US")
        CultName = "trans_en";
    else
        CultName = "trans_nl";

    Value = resource.ValuesList[CultName];
    return Value;


}

private static string suggestTranslation(string ObjectName)
{
    return "[" + ObjectName + " nog vertalen]";
    // TODO: Loggen en aan db toevoegen
}

public ArrayList RetrieveList() //Website w
{
    ArrayList arr = new ArrayList();
    XmlDocument ds = null;
    string RelativePath = _virtualPath; //"/Default.aspx";
    if (RelativePath.Trim().Length > 0 && !RelativePath.Contains("n/a"))
        ds = ResourceHelper.GetResourceValue(RelativePath);


    if (ds != null)
    {
        foreach (XmlNodeList dr in ds.ChildNodes)
        {
        }
        //foreach (DataRow dr in ds.Tables[0].Rows)
        //{
        //    Resource resx = new Resource();
        //    resx.KeyName = dr["KeyName"].ToString();
        //    resx.VirtualPath = dr["VirtualPath"].ToString();
        //    Dictionary<string, string> dictCulture = new Dictionary<string, string>();
        //    foreach (DataColumn dc in ds.Tables[0].Columns)
        //    {
        //        if (dc.ColumnName != "KeyName" && dc.ColumnName != "VirtualPath")
        //        {
        //            if (!dictCulture.ContainsKey(dc.ColumnName))
        //                dictCulture.Add(dc.ColumnName, dr[dc.ColumnName].ToString());
        //        }
        //    }
        //    resx.ValuesList = dictCulture;

        //    arr.Add(resx);
        //}
    }

    return arr;
}

public ArrayList RetrieveList(string virtualPath) //Website w
{
    ArrayList arr = new ArrayList();
    XmlDocument ds = null;

    if (!string.IsNullOrEmpty(virtualPath) && virtualPath.Trim().Length > 0 && !virtualPath.Contains("n/a"))
        ds = ResourceHelper.GetResourceValue(virtualPath);

    if (ds != null)
    {
        foreach (var dr in ds.ChildNodes)
        {
            //Resource resx = new Resource();
            //resx.KeyName = dr["KeyName"].ToString();
            //resx.VirtualPath = dr["VirtualPath"].ToString();
            //Dictionary<string, string> dictCulture = new Dictionary<string, string>();
            //foreach (DataColumn dc in ds.Tables[0].Columns)
            //{
            //    if (dc.ColumnName != "KeyName" && dc.ColumnName != "VirtualPath")
            //    {
            //        if (!dictCulture.ContainsKey(dc.ColumnName))
            //            dictCulture.Add(dc.ColumnName, dr[dc.ColumnName].ToString());
            //    }
            //}
            //resx.ValuesList = dictCulture;

            //arr.Add(resx);
        }
    }

    return arr;
}

public static CultureInfo CurrentUICulture()
{
    string CurrentCulture = string.Empty;
    if (System.Threading.Thread.CurrentThread != null)
    {
        if (System.Threading.Thread.CurrentThread.CurrentUICulture != null)
        {
            CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToString();

        }
    }
    if (CurrentCulture == string.Empty)
    {
        CurrentCulture = "nl-NL";
    }
    return new CultureInfo(CurrentCulture, false);
}

public static string GetResource(string ObjectName, string VirtualPath, CultureInfo ci, Dictionary<string, string> variables)
{
    string resourceTextUpdated = GetGlobalResource(ObjectName, VirtualPath, ci);

    foreach (KeyValuePair<string, string> variable in variables)
    {
        resourceTextUpdated = resourceTextUpdated.Replace("[" + variable.Key.ToUpper() + "]", variable.Value);
    }

    return resourceTextUpdated;
}

}


public class SessionManager
{
public object GetCache(string key)
{
    if (HttpContext.Current.Cache[key] != null)
    {
        return HttpContext.Current.Cache[key];
    }
    return null;
}

/// <summary>
/// This is Use for the resource 
/// cash the resource key,Value
/// </summary>
/// <param name="key"></param>
/// <param name="value"></param>
public void Insert(string key, object value)
{
    if (HttpContext.Current.Cache[key] == null)
    {
        HttpContext.Current.Cache.Insert(key, value);
    }
}
}
*/


}
