using eTurns.DAL;
using eTurns.DTO;
using eTurnsMaster.DAL;
using eTurnsWeb.Helper;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using eTurns.DTO.Resources;

namespace eTurnsWeb.Controllers
{
    public class FileUploadController : ApiController
    {
        [System.Web.Http.HttpPost]
        public Task<IEnumerable<string>> EnterPriseLogoUpload(long? id)
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                string FileTypeName = "enterpriselogo.jpg";
                string LogoPath = HttpContext.Current.Server.MapPath("~/Uploads/EnterpriseLogos/" + (id ?? 0).ToString());
                if (Directory.Exists(LogoPath))
                { }
                else
                {
                    Directory.CreateDirectory(LogoPath);
                }
                string fullPath = LogoPath;
                MyMultipartFormDataStreamProvider streamProvider = new MyMultipartFormDataStreamProvider(fullPath, FileTypeName);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);

                        var fileInfo = streamProvider.FileData.Select(i =>
                        {
                            var info = new FileInfo(i.LocalFileName);
                            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                            objEnterpriseMasterDAL.UpdateLogoName(id ?? 0, Path.GetFileName(info.FullName));
                            return string.Format(ResCommon.FileUploaded, info.FullName, info.Length);
                        });
                        return fileInfo;

                    });
                return task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, ResCommon.InvalidRequest));
            }
        }
        [System.Web.Http.HttpPost]
        public Task<IEnumerable<string>> CompanyLogoUpload(long? id)
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                string FileTypeName = "companylogo.jpg";
                string LogoPath = HttpContext.Current.Server.MapPath("~/Uploads/CompanyLogos/" + (id ?? 0).ToString());
                if (Directory.Exists(LogoPath))
                {

                }
                else
                {
                    Directory.CreateDirectory(LogoPath);

                }
                string fullPath = LogoPath;
                MyMultipartFormDataStreamProvider streamProvider = new MyMultipartFormDataStreamProvider(fullPath, FileTypeName);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);

                    var fileInfo = streamProvider.FileData.Select(i =>
                    {
                        var info = new FileInfo(i.LocalFileName);
                        CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                        objCompanyMasterDAL.updateLogoName(id ?? 0, Path.GetFileName(info.FullName));
                        return string.Format(ResCommon.FileUploaded, info.FullName, info.Length);
                    });
                    return fileInfo;

                });
                return task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, ResCommon.InvalidRequest));
            }
        }
        //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["ItemImagePath"].ToString();
        //string LogoPath = HttpContext.Current.Server.MapPath("~/" + UNCPathRoot + "/Link2/");
        [System.Web.Http.HttpPost]
        public Task<IEnumerable<string>> HelpDocumentSave(long? id, string FileTypeName)
        {
            string filename = string.Empty;
            if (Request.Content.IsMimeMultipartContent())
            {
                string LogoPath = HttpContext.Current.Server.MapPath("~/Uploads/HelpDoc");
                if (Directory.Exists(LogoPath))
                { }
                else
                {
                    Directory.CreateDirectory(LogoPath);
                }
                string fullPath = LogoPath;
                MyMultipartFormDataStreamProvider streamProvider = new MyMultipartFormDataStreamProvider(fullPath, FileTypeName);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);

                    var fileInfo = streamProvider.FileData.Select(i =>
                    {
                        var info = new FileInfo(i.LocalFileName);
                        filename = i.LocalFileName;
                        return string.Format(ResCommon.FileUploaded, info.FullName, info.Length);
                    });
                    return fileInfo;
                });

                return task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, ResCommon.InvalidRequest));
            }
        }
        [System.Web.Http.HttpPost]
        public HttpResponseMessage ItemLink2(long? id)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                //string FileTypeName = "no-image.jpg";
                //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = SiteSettingHelper.InventoryLink2; //Settinfile.Element("InventoryLink2").Value;
                //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["ItemImageLink2"].ToString();
                Int64 EnterpriseId = SessionHelper.EnterPriceID;
                Int64 CompanyID = SessionHelper.CompanyID;
                Int64 RoomID = SessionHelper.RoomID;
                string LogoPath = HttpContext.Current.Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyID + "/" + RoomID + "/" + id + "/");

                if (!Directory.Exists(LogoPath))
                {
                    Directory.CreateDirectory(LogoPath);
                }
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    string PostedInventoryLink2FileNewName = Path.GetFileName(postedFile.FileName);
                    string PostedInventoryLink2Fileext = Path.GetExtension(postedFile.FileName);
                    PostedInventoryLink2FileNewName = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + (id ?? 0) + PostedInventoryLink2Fileext;
                    var filePath = (LogoPath + "/" + PostedInventoryLink2FileNewName);
                    ItemMasterDAL objItemyMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    objItemyMasterDAL.updateLink2Name(id ?? 0, PostedInventoryLink2FileNewName, false);

                }
                result = Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
        [System.Web.Http.HttpPost]
        public HttpResponseMessage uploadZIPFILE()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            string filename = httpRequest.Files["uploadZIPFile"].FileName;
            //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
            ToolImageDetailDAL toolImageDetailDAL = new ToolImageDetailDAL(SessionHelper.EnterPriseDBName);

            if (httpRequest.Files.Count > 0)
            {
                string uploadFor = "InventoryPhoto";
                string tableName = Convert.ToString(HttpContext.Current.Request["tableName"]);
                int selectedModuleId = 0;
                var selectedModule = int.TryParse(tableName, out selectedModuleId);
                //if (!string.IsNullOrEmpty(tableName) && tableName == ResImportModule.ToolCertificationImages)
                //{
                //    tableName = "ToolCertificationImages";
                //}
                if (selectedModuleId > 0)
                {
                    switch (selectedModuleId)
                    {
                        case (int)SessionHelper.ModuleList.ItemMaster:
                            uploadFor = SiteSettingHelper.InventoryPhoto; // Settinfile.Element("InventoryPhoto").Value;
                            break;
                        case (int)SessionHelper.ModuleList.Assets:
                            uploadFor = SiteSettingHelper.AssetPhoto; //Settinfile.Element("AssetPhoto").Value;
                            break;
                        case (int)SessionHelper.ModuleList.ToolMaster:
                            uploadFor = SiteSettingHelper.ToolPhoto; // Settinfile.Element("ToolPhoto").Value;
                            break;
                        case (int)SessionHelper.ModuleList.SupplierMaster:
                            uploadFor = SiteSettingHelper.SupplierPhoto; // Settinfile.Element("SupplierPhoto").Value;
                            break;
                        case (int)SessionHelper.ModuleList.BOMItemMaster:
                            uploadFor = SiteSettingHelper.BOMInventoryPhoto; // Settinfile.Element("BOMInventoryPhoto").Value;
                            break;
                        case (int)SessionHelper.ModuleList.ToolCertificationImages:
                            uploadFor = SiteSettingHelper.ToolPhoto; // Settinfile.Element("ToolPhoto").Value;
                            break;
                        case (int)SessionHelper.ModuleList.Suppliercatalog:
                            uploadFor = "~/Uploads/CatalogItemImage/";
                            break;
                        default:
                            break;
                    }
                }
                string extractPath = HttpContext.Current.Server.MapPath(uploadFor + SessionHelper.CompanyID + "/");

                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                foreach (string file in httpRequest.Files)
                {
                    httpRequest.Files[file].SaveAs(HttpContext.Current.Server.MapPath("~/Uploads/" + filename));
                }
                string path = HttpContext.Current.Server.MapPath("~/Uploads/" + filename);
                using (ZipFile zip = ZipFile.Read(path))
                {
                    zip.ExtractAll(extractPath, ExtractExistingFileAction.DoNotOverwrite);
                }
                result = Request.CreateResponse(HttpStatusCode.Created);
                string FolderName = filename.Replace(".zip", string.Empty);
                string SourcePath = HttpContext.Current.Server.MapPath(uploadFor + SessionHelper.CompanyID.ToString() + "/" + FolderName);
                //if (!Directory.Exists(SourcePath))
                //{
                //    SourcePath = HttpContext.Current.Server.MapPath("~/Uploads/" + uploadFor + "/" + SessionHelper.CompanyID.ToString());
                //}

                string[] ItemIDs = Convert.ToString(HttpContext.Current.Request["ItemIDs"]).Split(',');
                string DestPath = string.Empty;
                if (selectedModuleId > 0)
                {
                    if (selectedModuleId == (int)SessionHelper.ModuleList.Suppliercatalog)
                    {
                        DestPath = HttpContext.Current.Server.MapPath(uploadFor + SessionHelper.EnterPriceID.ToString() + "/" );
                    }
                    else if (selectedModuleId != (int)SessionHelper.ModuleList.BOMItemMaster)
                    {
                        DestPath = HttpContext.Current.Server.MapPath(uploadFor + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID.ToString() + "/" + SessionHelper.RoomID.ToString() + "/");
                    }
                    else
                    {
                        DestPath = HttpContext.Current.Server.MapPath(uploadFor + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID.ToString() + "/");
                    }
                }
                foreach (string item in ItemIDs)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        string Id = item.Split('#')[0];
                        string FileName = item.Split('#')[1];
                        string SourceP = string.Empty;

                        if (selectedModuleId == (int)SessionHelper.ModuleList.ToolCertificationImages)
                        {
                            long imageId = 0;
                            long.TryParse(Id, out imageId);
                            var toolId = toolImageDetailDAL.GetToolIdBasedOnImageId(imageId);
                            SourceP = DestPath + toolId + "/" + Id;
                        }
                        else
                        {
                            SourceP = DestPath + Id;
                        }


                        if (!Directory.Exists(SourceP))
                        {
                            Directory.CreateDirectory(SourceP);
                        }
                        if (File.Exists(SourcePath + "/" + FileName))
                        {
                            File.Copy(SourcePath + "/" + FileName, SourceP + "/" + FileName, true);
                        }
                        else
                        {
                            //string noImagePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["NoImage"]); 
                            if (selectedModuleId == (int)SessionHelper.ModuleList.ToolCertificationImages)
                            {
                                long imageId = 0;
                                long.TryParse(Id, out imageId);
                                bool isRecordMarkAsDelete = toolImageDetailDAL.DeleteToolImageBasedOnId(imageId, SessionHelper.UserID);
                            }
                            else
                            {
                                string noImagePath = SiteSettingHelper.NoImage; // Settinfile.Element("NoImage").Value;
                                if (!string.IsNullOrEmpty(noImagePath))
                                {
                                    string noImage = HttpContext.Current.Server.MapPath(noImagePath);
                                    File.Copy(noImage, SourceP + "/" + FileName, true);
                                }
                            }
                        }

                        //foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
                        //{
                        //    File.Copy(newPath, newPath.Replace(SourcePath, extractPath), true);
                        //    File.Delete(newPath);
                        //}

                    }
                }

                Thread.Sleep(5000);
                if (Directory.Exists(SourcePath))
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(SourcePath);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }

                    Directory.Delete(SourcePath);
                }
                //File.Delete(path);// delete uploaded zip file
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
        [System.Web.Http.HttpPost]
        public HttpResponseMessage uploadZIPFileLink2()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            string filename = httpRequest.Files["uploadZIPFileForLink2"].FileName;
            //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
            string tableName = Convert.ToString(HttpContext.Current.Request["tableName"]);
            int selectedModuleId = 0;
            var selectedModule = int.TryParse(tableName, out selectedModuleId);
            string InventoryLink2 = string.Empty;
            if (selectedModuleId > 0)
            {
                if (selectedModuleId != (int)SessionHelper.ModuleList.BOMItemMaster)
                {
                    InventoryLink2 = SiteSettingHelper.InventoryLink2; // Settinfile.Element("InventoryLink2").Value;
                }
                else
                {
                    InventoryLink2 = SiteSettingHelper.BOMInventoryLink2; // Settinfile.Element("BOMInventoryLink2").Value;
                }
            }
            if (httpRequest.Files.Count > 0)
            {
                string extractPath = HttpContext.Current.Server.MapPath(InventoryLink2 + "/" + SessionHelper.CompanyID + "/");

                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                foreach (string file in httpRequest.Files)
                {
                    httpRequest.Files[file].SaveAs(HttpContext.Current.Server.MapPath("~/Uploads/" + filename));
                }
                string path = HttpContext.Current.Server.MapPath("~/Uploads/" + filename);
                using (ZipFile zip = ZipFile.Read(path))
                {
                    zip.ExtractAll(extractPath, ExtractExistingFileAction.DoNotOverwrite);
                }
                //result = Request.CreateResponse(HttpStatusCode.Created);
                string FolderName = filename.Replace(".zip", string.Empty);
                string SourcePath = HttpContext.Current.Server.MapPath(InventoryLink2 + "/" + SessionHelper.CompanyID.ToString() + "/" + FolderName);
                if (!Directory.Exists(SourcePath))
                {
                    SourcePath = HttpContext.Current.Server.MapPath(InventoryLink2 + "/" + SessionHelper.CompanyID.ToString());
                }

                string[] IDsWithLink2 = Convert.ToString(HttpContext.Current.Request["IDsWithLink2"]).Split(',');
                string itemIds = string.Empty;
                //foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
                string DestPath = string.Empty;
                if (selectedModuleId > 0)
                {
                    if (selectedModuleId != (int)SessionHelper.ModuleList.BOMItemMaster)
                    {
                        DestPath = HttpContext.Current.Server.MapPath(InventoryLink2 + "/" + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/");
                    }
                    else
                    {
                        DestPath = HttpContext.Current.Server.MapPath(InventoryLink2 + "/" + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/");
                    }
                }
                foreach (string IdFileName in IDsWithLink2)
                {
                    if (!string.IsNullOrWhiteSpace(IdFileName))
                    {
                        string Id = IdFileName.Split('#')[0];
                        string FileName = IdFileName.Split('#')[1];
                        if (!string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(FileName))
                        {
                            string SourceP = DestPath + Id;

                            if (File.Exists(SourcePath + "/" + FileName))
                            {
                                if (!Directory.Exists(SourceP))
                                {
                                    Directory.CreateDirectory(SourceP);
                                }

                                File.Copy(SourcePath + "/" + FileName, SourceP + "/" + FileName, true);
                            }
                            else
                            {
                                ////string noImagePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["NoImage"]);
                                //string noImagePath = Settinfile.Element("NoImage").Value;
                                //if (!string.IsNullOrEmpty(noImagePath))
                                //{
                                //    string noImage = HttpContext.Current.Server.MapPath(noImagePath);
                                //    File.Copy(noImage, SourceP + "/" + FileName, true);
                                //}
                                if (string.IsNullOrEmpty(itemIds))
                                {
                                    itemIds = Id;
                                }
                                else
                                {
                                    itemIds += "," + Id;
                                }
                            }

                        }
                    }
                }

                /******** CODE FOR UPDATE IMAGEPATH IN RESPECTIVE ITEM IN WHICH IMAGES NOT UPLOADED **********/
                if (!string.IsNullOrEmpty(itemIds))
                {
                    bool isUpdate = new ItemMasterDAL(SessionHelper.EnterPriseDBName).UpdateItemAsPerIDList(itemIds, SessionHelper.RoomID, SessionHelper.CompanyID);
                }
                /******** CODE FOR UPDATE IMAGEPATH IN RESPECTIVE ITEM IN WHICH IMAGES NOT UPLOADED **********/

                Thread.Sleep(5000);
                //foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
                //{
                //    File.Delete(newPath);
                //}
                //if (Directory.Exists(SourcePath))
                //{
                //    Directory.Delete(SourcePath);
                //}
                //File.Delete(path);// delete uploaded zip file
                if (Directory.Exists(SourcePath))
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(SourcePath);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }

                    Directory.Delete(SourcePath);
                }
                File.Delete(path);// delete uploaded zip file
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
        public HttpResponseMessage PostEnterpriseFile(long? id)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                //string FileTypeName = "enterpriselogo.jpg";
                string LogoPath = HttpContext.Current.Server.MapPath("~/Uploads/EnterpriseLogos/" + (id ?? 0).ToString());
                if (Directory.Exists(LogoPath))
                {

                }
                else
                {
                    Directory.CreateDirectory(LogoPath);
                }
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    string PostedEnterpriseFileNewName = Path.GetFileName(postedFile.FileName);
                    string PostedEnterpriseFileext = Path.GetExtension(postedFile.FileName);
                    PostedEnterpriseFileNewName = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + (id ?? 0) + PostedEnterpriseFileext;
                    var filePath = (LogoPath + "/" + PostedEnterpriseFileNewName);
                    EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                    postedFile.SaveAs(filePath);
                    objEnterpriseMasterDAL.UpdateLogoName(id ?? 0, PostedEnterpriseFileNewName);

                }
                result = Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
        public HttpResponseMessage PostSupplierFile(long? id)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                //string FileTypeName = "enterpriselogo.jpg";
                //string LogoPath = HttpContext.Current.Server.MapPath("~/Uploads/SupplierLogos/" + (id ?? 0).ToString());
                //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["SupplierPhoto"].ToString();
                //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = SiteSettingHelper.SupplierPhoto; // Settinfile.Element("SupplierPhoto").Value;

                Int64 EnterpriseId = SessionHelper.EnterPriceID;
                Int64 CompanyID = SessionHelper.CompanyID;
                Int64 RoomID = SessionHelper.RoomID;
                string LogoPath = HttpContext.Current.Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyID + "/" + RoomID + "/" + id + "/");

                if (!Directory.Exists(LogoPath))
                {
                    Directory.CreateDirectory(LogoPath);
                }
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    string PostedSupplierFileNewName = Path.GetFileName(postedFile.FileName);
                    string PostedSupplierFileext = Path.GetExtension(postedFile.FileName);
                    PostedSupplierFileNewName = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + (id ?? 0) + PostedSupplierFileext;
                    var filePath = (LogoPath + "/" + PostedSupplierFileNewName);

                    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    //objSupplierMasterDAL.updateImageName(id ?? 0, Path.GetFileName(postedFile.FileName));
                    objSupplierMasterDAL.UpdateSupplierData(id ?? 0, PostedSupplierFileNewName, "SupplierImage", SessionHelper.UserID);

                }
                result = Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
        public HttpResponseMessage PostItemFile(long? id)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                //string FileTypeName = "enterpriselogo.jpg";
                // string LogoPath = HttpContext.Current.Server.MapPath("~/Uploads/InventoryPhoto/" + SessionHelper.CompanyID);
                // System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = SiteSettingHelper.InventoryPhoto; // Settinfile.Element("InventoryPhoto").Value;
                //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["ItemImage"].ToString();
                Int64 EnterpriseId = SessionHelper.EnterPriceID;
                Int64 CompanyID = SessionHelper.CompanyID;
                Int64 RoomID = SessionHelper.RoomID;
                string LogoPath = HttpContext.Current.Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyID + "/" + RoomID + "/" + id + "/");
                if (!Directory.Exists(LogoPath))
                {
                    Directory.CreateDirectory(LogoPath);
                }
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    string PostedInventoryFileNewName = Path.GetFileName(postedFile.FileName);
                    string PostedInventoryFileext = Path.GetExtension(postedFile.FileName);
                    PostedInventoryFileNewName = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + (id ?? 0) + PostedInventoryFileext;
                    var filePath = (LogoPath + "/" + PostedInventoryFileNewName);
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    //Image image  =Image.FromFile(filePath);

                    //Bitmap s= ResizeImage(image, 100, 100);
                    //s.Save("D:\\project\\eTurns\\Trunk\\eTurns5050\\eTurnsWeb\\Uploads\\InventoryPhoto\\10059\\201180050\\20132\\542020\\test.jpg", ImageFormat.Jpeg);
                    objItemMasterDAL.updateImagePath(id ?? 0, PostedInventoryFileNewName, false);

                    ItemMasterDTO ExistItemDTO = objItemMasterDAL.GetItemWithoutJoins(id ?? 0, null);
                    if(ExistItemDTO != null)
                    {
                        QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(SessionHelper.EnterPriseDBName);
                        objQBItemDAL.InsertQuickBookItem(ExistItemDTO.GUID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, "Update", ExistItemDTO.IsDeleted, SessionHelper.UserID, "Web", null, "Item Edit");
                    }


                }
                result = Request.CreateResponse(HttpStatusCode.Created);
                //ItemMasterDAL objItemyMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                //objItemyMasterDAL.updateZipName(Convert.ToInt64(id), string.Empty);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
        public HttpResponseMessage PostWOSignatureFile(long? id)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                long CompanyID = SessionHelper.CompanyID;
                string UNCPathRoot = "~/Uploads/WorkOrderSignature/" + Convert.ToString(CompanyID);//SiteSettingHelper.InventoryPhoto; // Settinfile.Element("InventoryPhoto").Value;
                string LogoPath = HttpContext.Current.Server.MapPath(UNCPathRoot);

                if (!Directory.Exists(LogoPath))
                {
                    Directory.CreateDirectory(LogoPath);
                }

                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var filePath = (LogoPath + "/" + postedFile.FileName);
                    WorkOrderDAL workorderDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    workorderDAL.UpdateWOSignatureById(id ?? 0, postedFile.FileName, SessionHelper.UserID);
                }
                result = Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
        public HttpResponseMessage PostBOMItemFile(long? id)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                //string FileTypeName = "enterpriselogo.jpg";
                // string LogoPath = HttpContext.Current.Server.MapPath("~/Uploads/InventoryPhoto/" + SessionHelper.CompanyID);
                //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = SiteSettingHelper.BOMInventoryPhoto; // Settinfile.Element("BOMInventoryPhoto").Value;
                //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["ItemImage"].ToString();
                Int64 EnterpriseId = SessionHelper.EnterPriceID;
                Int64 CompanyID = SessionHelper.CompanyID;
                Int64 RoomID = SessionHelper.RoomID;
                string LogoPath = HttpContext.Current.Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyID + "/" + id + "/");
                if (!Directory.Exists(LogoPath))
                {
                    Directory.CreateDirectory(LogoPath);
                }
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    string PostedBOMInventoryFileNewName = Path.GetFileName(postedFile.FileName);
                    string PostedBOMInventoryFileext = Path.GetExtension(postedFile.FileName);
                    PostedBOMInventoryFileNewName = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + (id ?? 0) + PostedBOMInventoryFileext;
                    var filePath = (LogoPath + "/" + PostedBOMInventoryFileNewName);

                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    //Image image  =Image.FromFile(filePath);

                    //Bitmap s= ResizeImage(image, 100, 100);
                    //s.Save("D:\\project\\eTurns\\Trunk\\eTurns5050\\eTurnsWeb\\Uploads\\InventoryPhoto\\10059\\201180050\\20132\\542020\\test.jpg", ImageFormat.Jpeg);
                    objItemMasterDAL.updateImagePath(id ?? 0, PostedBOMInventoryFileNewName, true);

                }

                if (id.GetValueOrDefault(0) > 0)
                {
                    CommonUtility.SaveBOMImageToRoomItem(id.GetValueOrDefault(0).ToString(), "image");
                }

                result = Request.CreateResponse(HttpStatusCode.Created);
                //ItemMasterDAL objItemyMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                //objItemyMasterDAL.updateZipName(Convert.ToInt64(id), string.Empty);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
        [System.Web.Http.HttpPost]
        public HttpResponseMessage BOMItemLink2(long? id)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                //string FileTypeName = "no-image.jpg";
                //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = SiteSettingHelper.BOMInventoryLink2; // Settinfile.Element("BOMInventoryLink2").Value;
                //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["ItemImageLink2"].ToString();
                Int64 EnterpriseId = SessionHelper.EnterPriceID;
                Int64 CompanyID = SessionHelper.CompanyID;
                Int64 RoomID = SessionHelper.RoomID;
                string LogoPath = HttpContext.Current.Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyID + "/" + id + "/");

                if (!Directory.Exists(LogoPath))
                {
                    Directory.CreateDirectory(LogoPath);
                }

                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];

                    string PostedBOMInvLink2FileNewName = Path.GetFileName(postedFile.FileName);
                    string PostedBOMInvLink2Fileext = Path.GetExtension(postedFile.FileName);
                    PostedBOMInvLink2FileNewName = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + (id ?? 0) + PostedBOMInvLink2Fileext;
                    var filePath = (LogoPath + "/" + PostedBOMInvLink2FileNewName);
                    ItemMasterDAL objItemyMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    objItemyMasterDAL.updateLink2Name(id ?? 0, PostedBOMInvLink2FileNewName, true);

                }

                if (id.GetValueOrDefault(0) > 0)
                {
                    CommonUtility.SaveBOMImageToRoomItem(id.GetValueOrDefault(0).ToString(), "link2");
                }

                result = Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        public HttpResponseMessage PostToolFile(long? id)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                //string FileTypeName = "enterpriselogo.jpg";
                // string LogoPath = HttpContext.Current.Server.MapPath("~/Uploads/ToolPhoto/" + SessionHelper.CompanyID);
                //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["ToolPhoto"].ToString();
                //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = SiteSettingHelper.ToolPhoto; //Settinfile.Element("ToolPhoto").Value;
                Int64 EnterpriseId = SessionHelper.EnterPriceID;
                Int64 CompanyID = SessionHelper.CompanyID;
                Int64 RoomID = SessionHelper.RoomID;
                string LogoPath = HttpContext.Current.Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyID + "/" + RoomID + "/" + id + "/");
                if (Directory.Exists(LogoPath))
                {

                }
                else
                {
                    Directory.CreateDirectory(LogoPath);
                }
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    //var filePath = HttpContext.Current.Server.MapPath("~/Uploads/ToolPhoto/" + SessionHelper.CompanyID + "/" + Path.GetFileName(postedFile.FileName));

                    string PostedToolFileNewName = Path.GetFileName(postedFile.FileName);
                    string PostedToolFileext = Path.GetExtension(postedFile.FileName);
                    PostedToolFileNewName = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + (id ?? 0) + PostedToolFileext;

                    var filePath = (LogoPath + "/" + PostedToolFileNewName);
                    ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    objToolMasterDAL.updateImagePath(id ?? 0, PostedToolFileNewName);

                }
                result = Request.CreateResponse(HttpStatusCode.Created);
                //ItemMasterDAL objItemyMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                //objItemyMasterDAL.updateZipName(Convert.ToInt64(id), string.Empty);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
        public HttpResponseMessage PostAssetFile(long? id)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                //string FileTypeName = "enterpriselogo.jpg";
                //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = SiteSettingHelper.AssetPhoto; // Settinfile.Element("AssetPhoto").Value;
                //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["AssetPhoto"].ToString();
                Int64 EnterpriseId = SessionHelper.EnterPriceID;
                Int64 CompanyID = SessionHelper.CompanyID;
                Int64 RoomID = SessionHelper.RoomID;
                string LogoPath = HttpContext.Current.Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyID + "/" + RoomID + "/" + id + "/");
                if (Directory.Exists(LogoPath))
                {

                }
                else
                {
                    Directory.CreateDirectory(LogoPath);
                }
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    //  var filePath = HttpContext.Current.Server.MapPath("~/Uploads/AssetPhoto/" + SessionHelper.CompanyID + "/" + Path.GetFileName(postedFile.FileName));
                    string PostedAssetFileNewName = Path.GetFileName(postedFile.FileName);
                    string PostedAssetFileext = Path.GetExtension(postedFile.FileName);
                    PostedAssetFileNewName = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + (id ?? 0) + PostedAssetFileext;
                    var filePath = (LogoPath + "/" + PostedAssetFileNewName);

                    AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    objAssetMasterDAL.updateImagePath(id ?? 0, PostedAssetFileNewName);

                }
                result = Request.CreateResponse(HttpStatusCode.Created);
                //ItemMasterDAL objItemyMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                //objItemyMasterDAL.updateZipName(Convert.ToInt64(id), string.Empty);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
        public HttpResponseMessage PostCompanyFile(long? id)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                //string FileTypeName = "enterpriselogo.jpg";
                string LogoPath = HttpContext.Current.Server.MapPath("~/Uploads/CompanyLogos/" + (id ?? 0).ToString());
                if (Directory.Exists(LogoPath))
                {

                }
                else
                {
                    Directory.CreateDirectory(LogoPath);
                }
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    //var filePath = HttpContext.Current.Server.MapPath("~/Uploads/CompanyLogos/" + (id ?? 0).ToString() + "/" + Path.GetFileName(postedFile.FileName));

                    string PostedCompanyFileNewName = Path.GetFileName(postedFile.FileName);
                    string PostedCompanyFileext = Path.GetExtension(postedFile.FileName);
                    PostedCompanyFileNewName = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + (id ?? 0) + PostedCompanyFileext;

                    var filePath = (LogoPath + "/" + PostedCompanyFileNewName);
                    CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    objCompanyMasterDAL.updateLogoName(id ?? 0, PostedCompanyFileNewName);

                }
                result = Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
        public HttpResponseMessage PostEulaFile(long? id)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                //string FileTypeName = "enterpriselogo.jpg";
                string LogoPath = HttpContext.Current.Server.MapPath("~/Uploads/EulaFiles/");
                if (Directory.Exists(LogoPath))
                {

                }
                else
                {
                    Directory.CreateDirectory(LogoPath);
                }
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    string filename = Path.GetFileName(postedFile.FileName);
                    filename = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + filename;
                    var filePath = HttpContext.Current.Server.MapPath("~/Uploads/EulaFiles/" + filename);
                    EulaMasterDAL objEulaMasterDAL = new EulaMasterDAL();
                    postedFile.SaveAs(filePath);
                    objEulaMasterDAL.InsertFileName(id ?? 0, filename);

                }
                result = Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }

        public HttpResponseMessage PostHelpDocumentFile(long? id, string FileTypeName, string VideoName, bool IsDocCheck, bool IsVideoCheck, int HelpDocTypeFilter, string FileNames)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                eTurnsMaster.DAL.HelpDocumentDAL objHelpDocDAL = new eTurnsMaster.DAL.HelpDocumentDAL();
                eTurnsMaster.DAL.HelpDocumentDetailDAL objHelpDocDetailDAL = new eTurnsMaster.DAL.HelpDocumentDetailDAL();

                string HelpModuleDocPath = "~/Uploads/HelpDoc/"+Convert.ToString(id);
                string HelpModuleDocPathDir = HttpContext.Current.Server.MapPath(HelpModuleDocPath);

                if (!Directory.Exists(HelpModuleDocPathDir))
                    Directory.CreateDirectory(HelpModuleDocPathDir);

                int Count = -1;
                foreach (string file in httpRequest.Files)
                {
                    Count += 1;
                    bool IsFile = false;
                    bool IsVideo = false;
                    //if (httpRequest.Files[file].ContentLength > 0)
                    if (httpRequest.Files[Count].ContentLength > 0)
                    {
                        //var postedFile = httpRequest.Files[file];
                        var postedFile = httpRequest.Files[Count];
                        string strDocPath = "../Uploads/HelpDoc/" + Convert.ToString(id)+"/";
                        string filePath = "";
                        string UploadFileName = postedFile.FileName;

                        if (!string.IsNullOrWhiteSpace(FileNames))
                        {
                            string[] split = FileNames.Split(new string[] { "!" }, StringSplitOptions.None);
                            if (split.Length >= 1)
                            {
                                foreach (string s in split)
                                {
                                    if (s.Contains(UploadFileName))
                                    {
                                        string[] CustomeName = s.Split(new string[] { "~" }, StringSplitOptions.None);
                                        if (CustomeName.Length == 2)
                                        {
                                            FileTypeName = CustomeName[0].Trim() + "." + CustomeName[1].Trim().Split('.')[1];
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (postedFile.ContentType == "video/mp4")
                        {
                            IsVideo = true;
                            if (HelpDocTypeFilter == (int)HelpDocType.Module || HelpDocTypeFilter == (int)HelpDocType.Report || HelpDocTypeFilter == (int)HelpDocType.Mobile)
                            {
                                filePath = HttpContext.Current.Server.MapPath("~/Uploads/HelpDoc/" + Convert.ToString(id) + "//" + FileTypeName);
                            }
                        }
                        else
                        {
                            IsFile = true;
                            if (HelpDocTypeFilter == (int)HelpDocType.Module || HelpDocTypeFilter == (int)HelpDocType.Report || HelpDocTypeFilter == (int)HelpDocType.Mobile)
                            {
                                filePath = HttpContext.Current.Server.MapPath("~/Uploads/HelpDoc/" + Convert.ToString(id) + "//" + FileTypeName);
                            }
                        }
                        postedFile.SaveAs(filePath);
                        
                        HelpDocumentMasterDTO objHelpDTO = objHelpDocDAL.GetHelpDocumentMasterByID(Convert.ToInt64(id));
                        HelpDocumentDetailDTO objHelpDocDtlDTO = objHelpDocDetailDAL.GetHelpDocumentDetailByMasterIDModName(Convert.ToInt64(id), FileTypeName,IsFile, IsVideo);
                        if (objHelpDocDtlDTO == null)
                        {
                            objHelpDocDtlDTO = new HelpDocumentDetailDTO();
                            objHelpDocDtlDTO.HelpDocMasterID = Convert.ToInt64(id);
                            if (IsFile)
                            {
                                objHelpDocDtlDTO.ModuleDocName = FileTypeName;
                                objHelpDocDtlDTO.ModuleDocPath = strDocPath + FileTypeName;
                                objHelpDocDtlDTO.IsDoc = true;
                            }
                            else if(IsVideo)
                            {
                                objHelpDocDtlDTO.ModuleVideoName = FileTypeName;
                                objHelpDocDtlDTO.ModuleVideoPath = strDocPath + FileTypeName;
                                objHelpDocDtlDTO.IsVideo = true;
                            }
                            objHelpDocDtlDTO.CreatedBy = SessionHelper.UserID;
                            objHelpDocDtlDTO.IsDeleted = false;
                            objHelpDocDtlDTO.IsArchived = false;
                            objHelpDocDetailDAL.InsertHelpDocumentDetail(objHelpDocDtlDTO);
                        }
                        else
                        {
                            if (IsFile) 
                            {
                                objHelpDocDtlDTO.ModuleDocName = FileTypeName;
                                objHelpDocDtlDTO.ModuleDocPath = strDocPath + FileTypeName;
                                objHelpDocDtlDTO.IsDoc = true;
                            }
                            else if (IsVideo)
                            {
                                objHelpDocDtlDTO.ModuleVideoName = FileTypeName;
                                objHelpDocDtlDTO.ModuleVideoPath = strDocPath + FileTypeName;
                                objHelpDocDtlDTO.IsVideo = true;
                            }
                            
                            objHelpDocDtlDTO.LastUpdatedBy = SessionHelper.UserID;
                            objHelpDocDetailDAL.UpdateHelpDocumentDetail(objHelpDocDtlDTO);
                        }
                        if (objHelpDTO != null)
                        {
                            #region Comment
                            //if (postedFile.ContentType.Contains("video"))
                            //{
                            //    objHelpDTO.ModuleVideoName = FileTypeName;
                            //    if (HelpDocTypeFilter == (int)HelpDocType.Module)
                            //        objHelpDTO.ModuleVideoPath = strVideoPath + FileTypeName;
                            //    else if (HelpDocTypeFilter == (int)HelpDocType.Report)
                            //        objHelpDTO.ModuleVideoPath = strVideoReportPath + FileTypeName;
                            //}
                            //else
                            //{
                            //    objHelpDTO.ModuleDocName = FileTypeName;
                            //    if (HelpDocTypeFilter == (int)HelpDocType.Module)
                            //        objHelpDTO.ModuleDocPath = strDocPath + FileTypeName;
                            //    else if (HelpDocTypeFilter == (int)HelpDocType.Report)
                            //        objHelpDTO.ModuleDocPath = strDocReportPath + FileTypeName;
                            //} 
                            #endregion

                            objHelpDTO.IsDoc = IsDocCheck;
                            objHelpDTO.IsVideo = IsVideoCheck;
                            objHelpDTO.LastUpdatedBy = SessionHelper.UserID;
                            objHelpDocDAL.UpdateHelpDocument(objHelpDTO);
                        }
                    }
                    else
                    {
                        result = Request.CreateResponse(HttpStatusCode.BadRequest);
                        return result;
                    }
                }
                result = Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                if(string.IsNullOrWhiteSpace(FileTypeName) && string.IsNullOrWhiteSpace(VideoName))
                {
                    eTurnsMaster.DAL.HelpDocumentDAL objHelpDocDAL = new eTurnsMaster.DAL.HelpDocumentDAL();
                    HelpDocumentMasterDTO objHelpDTO = objHelpDocDAL.GetHelpDocumentMasterByID(Convert.ToInt64(id));
                    if (objHelpDTO != null)
                    {
                        objHelpDTO.IsDoc = IsDocCheck;
                        objHelpDTO.IsVideo = IsVideoCheck;
                        objHelpDTO.LastUpdatedBy = SessionHelper.UserID;
                        objHelpDocDAL.UpdateHelpDocument(objHelpDTO);
                    }
                    result = Request.CreateResponse(HttpStatusCode.Created);
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                
            }

            return result;
        }
        [System.Web.Http.HttpPost]
        public HttpResponseMessage WorkOrderFileUpload(long? Id)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                //long? id = Convert.ToInt64(IdGuid.Split('#')[0]);
                //Guid WorkOrderGuid = Guid.Parse(IdGuid.Split('#')[1]);
                //string FileTypeName = "no-image.jpg";
                //string WorkOrderFilePath = System.Configuration.ConfigurationManager.AppSettings["WorkOrderFilePath"].ToString();
                WorkOrderDAL objWorkrderDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
                Guid WorkOrderGuid = objWorkrderDAL.GetWorkOrderGuid(Id ?? 0);
                //  string WorkOrderPath = HttpContext.Current.Server.MapPath("~/Uploads/" + WorkOrderFilePath + "/" + SessionHelper.CompanyID + "/" + (Id ?? 0).ToString());
                //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["WorkOrderFilePaths"].ToString();
                //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = SiteSettingHelper.WorkOrderFilePaths; // Settinfile.Element("WorkOrderFilePaths").Value;
                Int64 EnterpriseId = SessionHelper.EnterPriceID;
                Int64 CompanyID = SessionHelper.CompanyID;
                Int64 RoomID = SessionHelper.RoomID;
                string WorkOrderPath = HttpContext.Current.Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyID + "/" + RoomID + "/" + (Id ?? 0).ToString() + "/");
                if (!Directory.Exists(WorkOrderPath))
                {

                    Directory.CreateDirectory(WorkOrderPath);
                }
                WorkOrderImageDetailDTO objWorkOrderImageDetailDTO = new WorkOrderImageDetailDTO();
                objWorkOrderImageDetailDTO.WorkOrderGUID = WorkOrderGuid;
                objWorkOrderImageDetailDTO.IsDeleted = false;
                objWorkOrderImageDetailDTO.AddedFrom = "Web";
                objWorkOrderImageDetailDTO.EditedFrom = "Web";

                objWorkOrderImageDetailDTO.CreatedBy = SessionHelper.UserID;
                objWorkOrderImageDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                objWorkOrderImageDetailDTO.IsArchived = false;

                objWorkOrderImageDetailDTO.RoomId = SessionHelper.RoomID;
                objWorkOrderImageDetailDTO.CompanyID = SessionHelper.CompanyID;
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    string Imagename = Path.GetFileName(postedFile.FileName);

                    List<WorkOrderImageDetail> objWorkOrderImageDetail = new List<WorkOrderImageDetail>();
                    objWorkOrderImageDetail = objWorkrderDAL.GetWorkorderImagesByWOGuidPlain(WorkOrderGuid).ToList();
                    bool imgCanInsert = true;
                    if (objWorkOrderImageDetail != null && objWorkOrderImageDetail.Count > 0)
                    {
                        if (objWorkOrderImageDetail.Where(x => x.WOImageName == Imagename).Count() > 0)
                        {
                            imgCanInsert = false;
                        }
                    }
                    if (imgCanInsert)
                    {
                        var filePath = (WorkOrderPath + "/" + Imagename);
                        WorkOrderImageDetailDAL objWorkOrderImageDetailDAL = new WorkOrderImageDetailDAL(SessionHelper.EnterPriseDBName);
                        postedFile.SaveAs(filePath);
                        objWorkOrderImageDetailDTO.WOImageName = Imagename;
                        objWorkOrderImageDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objWorkOrderImageDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objWorkOrderImageDetailDTO.Created = DateTimeUtility.DateTimeNow;
                        objWorkOrderImageDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objWorkOrderImageDetailDAL.Insert(objWorkOrderImageDetailDTO);
                    }

                }
                result = Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage OrderFileUpload(long? Id,string OrderGUID)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
              
                string UNCPathRoot = SiteSettingHelper.OrderFilePaths; // Settinfile.Element("WorkOrderFilePaths").Value;
                Int64 EnterpriseId = SessionHelper.EnterPriceID;
                Int64 CompanyID = SessionHelper.CompanyID;
                Int64 RoomID = SessionHelper.RoomID;
                string OrderPath = HttpContext.Current.Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyID + "/" + RoomID + "/" + (Id ?? 0).ToString() + "/");
                if (!Directory.Exists(OrderPath))
                {

                    Directory.CreateDirectory(OrderPath);
                }
                OrderImageDetailDTO objOrderImageDetailDTO = new OrderImageDetailDTO();
                objOrderImageDetailDTO.OrderGUID = Guid.Parse(OrderGUID);
                objOrderImageDetailDTO.IsDeleted = false;
                objOrderImageDetailDTO.AddedFrom = "Web";
                objOrderImageDetailDTO.EditedFrom = "Web";

                objOrderImageDetailDTO.CreatedBy = SessionHelper.UserID;
                objOrderImageDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                objOrderImageDetailDTO.IsArchived = false;

                objOrderImageDetailDTO.RoomId = SessionHelper.RoomID;
                objOrderImageDetailDTO.CompanyID = SessionHelper.CompanyID;
                OrderImageDetailDAL objWorkOrderImageDetailDAL = new OrderImageDetailDAL(SessionHelper.EnterPriseDBName);
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    string Imagename = Path.GetFileName(postedFile.FileName);
                    List<OrderImageDetail> objWorkOrderImageDetail = new List<OrderImageDetail>();
                    objWorkOrderImageDetail = objWorkOrderImageDetailDAL.GetorderImagesByGuidPlain(Guid.Parse(OrderGUID)).ToList();
                    bool imgCanInsert = true;
                    if (objWorkOrderImageDetail != null && objWorkOrderImageDetail.Count > 0)
                    {
                        if (objWorkOrderImageDetail.Where(x => x.ImageName == Imagename).Count() > 0)
                        {
                            imgCanInsert = false;
                        }
                    }
                    if (imgCanInsert)
                    {
                        var filePath = (OrderPath + "/" + Imagename);
                        postedFile.SaveAs(filePath);
                        objOrderImageDetailDTO.ImageName = Imagename;
                        objOrderImageDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objOrderImageDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objOrderImageDetailDTO.Created = DateTimeUtility.DateTimeNow;
                        objOrderImageDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objOrderImageDetailDTO.GUID = Guid.NewGuid();
                        objWorkOrderImageDetailDAL.InsertOrderImageData(objOrderImageDetailDTO);
                    }
                }
                result = Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage ReceiveFileUpload(long? Id, string orderDetailsGUID, string receivedDetailGuid)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {

                string UNCPathRoot = string.IsNullOrEmpty(SiteSettingHelper.ReceiveFilePaths) ? "~/Uploads/ReceiveFile/" : SiteSettingHelper.ReceiveFilePaths;
                Int64 EnterpriseId = SessionHelper.EnterPriceID;
                Int64 CompanyID = SessionHelper.CompanyID;
                Int64 RoomID = SessionHelper.RoomID;
                string ReceiveFilePaths = HttpContext.Current.Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyID + "/" + RoomID + "/" + (Id ?? 0).ToString() + "/");
                if (!Directory.Exists(ReceiveFilePaths))
                {
                    Directory.CreateDirectory(ReceiveFilePaths);
                }
                ReceiveFileDetailDTO receiveFileDetail = new ReceiveFileDetailDTO();
                receiveFileDetail.OrderDetailsGUID = Guid.Parse(orderDetailsGUID);
                receiveFileDetail.ReceivedOrderTransferGuid = Guid.Parse(receivedDetailGuid);
                receiveFileDetail.IsDeleted = false;
                receiveFileDetail.AddedFrom = "web";
                receiveFileDetail.EditedFrom = "web";
                receiveFileDetail.CreatedBy = SessionHelper.UserID;
                receiveFileDetail.LastUpdatedBy = SessionHelper.UserID;
                receiveFileDetail.IsArchived = false;
                receiveFileDetail.RoomId = SessionHelper.RoomID;
                receiveFileDetail.CompanyID = SessionHelper.CompanyID;

                ReceiveFileDetailsDAL receiveFileDetailsDAL = new ReceiveFileDetailsDAL(SessionHelper.EnterPriseDBName);
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    string FileName = Path.GetFileName(postedFile.FileName);
                    List<ReceiveFileDetailDTO> objOrderImageDetail = new List<ReceiveFileDetailDTO>();
                    objOrderImageDetail = receiveFileDetailsDAL.GetReceiveFileByGuidPlain(Guid.Parse(orderDetailsGUID)).ToList();
                    bool imgCanInsert = true;
                    if (objOrderImageDetail != null && objOrderImageDetail.Count > 0)
                    {
                        if (objOrderImageDetail.Where(x => x.FileName == FileName).Count() > 0)
                        {
                            imgCanInsert = false;
                        }
                    }
                    if (imgCanInsert)
                    {
                        var filePath = (ReceiveFilePaths + "/" + FileName);
                        postedFile.SaveAs(filePath);
                        receiveFileDetail.FileName = FileName;
                        receiveFileDetail.FilePath = filePath;
                        receiveFileDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                        receiveFileDetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        receiveFileDetail.Created = DateTimeUtility.DateTimeNow;
                        receiveFileDetail.LastUpdated = DateTimeUtility.DateTimeNow;
                        receiveFileDetail.GUID = Guid.NewGuid();
                        receiveFileDetailsDAL.InsertReceiveFileData(receiveFileDetail);
                    }
                }
                result = Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage RequisitionFileUpload(long? Id, string RequisitionGUID)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {

                string UNCPathRoot = SiteSettingHelper.RequisitionFilePaths; // Settinfile.Element("WorkOrderFilePaths").Value;
                Int64 EnterpriseId = SessionHelper.EnterPriceID;
                Int64 CompanyID = SessionHelper.CompanyID;
                Int64 RoomID = SessionHelper.RoomID;
                string ReqFilePath = HttpContext.Current.Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyID + "/" + RoomID + "/" + (Id ?? 0).ToString() + "/");
                if (!Directory.Exists(ReqFilePath))
                {
                    Directory.CreateDirectory(ReqFilePath);
                }
                RequisitionImageDetailDTO objOrderImageDetailDTO = new RequisitionImageDetailDTO();
                objOrderImageDetailDTO.RequisitionGuid = Guid.Parse(RequisitionGUID);
                objOrderImageDetailDTO.IsDeleted = false;
                objOrderImageDetailDTO.AddedFrom = "Web";
                objOrderImageDetailDTO.EditedFrom = "Web";

                objOrderImageDetailDTO.CreatedBy = SessionHelper.UserID;
                objOrderImageDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                objOrderImageDetailDTO.IsArchived = false;

                objOrderImageDetailDTO.RoomId = SessionHelper.RoomID;
                objOrderImageDetailDTO.CompanyID = SessionHelper.CompanyID;
                RequisitionImageDetailDAL objReqImageDetailDAL = new RequisitionImageDetailDAL(SessionHelper.EnterPriseDBName);
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    string Imagename = Path.GetFileName(postedFile.FileName);
                    List<RequisitionImageDetailDTO> objWorkOrderImageDetail = new List<RequisitionImageDetailDTO>();
                    objWorkOrderImageDetail = objReqImageDetailDAL.GetRequisitionImagesByGuidPlain(Guid.Parse(RequisitionGUID)).ToList();
                    bool imgCanInsert = true;
                    if (objWorkOrderImageDetail != null && objWorkOrderImageDetail.Count > 0)
                    {
                        if (objWorkOrderImageDetail.Where(x => x.ImageName == Imagename).Count() > 0)
                        {
                            imgCanInsert = false;
                        }
                    }
                    if (imgCanInsert)
                    {
                        var filePath = (ReqFilePath + "/" + Imagename);
                        postedFile.SaveAs(filePath);
                        objOrderImageDetailDTO.ImageName = Imagename;
                        objOrderImageDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objOrderImageDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objOrderImageDetailDTO.Created = DateTimeUtility.DateTimeNow;
                        objOrderImageDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objOrderImageDetailDTO.GUID = Guid.NewGuid();
                        objReqImageDetailDAL.InsertRequisitionImageData(objOrderImageDetailDTO);
                    }
                }
                result = Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage ToolFileUpload(long? Id)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            string ToolGuid = httpRequest.Params["ToolGUID"];
            string SerialNumber = httpRequest.Params["Serial"];
            string GuidForSeparator = httpRequest.Params["GuidForSeparator"];

            if (httpRequest.Files.Count > 0)
            {
                //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = SiteSettingHelper.ToolPhoto; // Settinfile.Element("ToolPhoto").Value;
                long EnterpriseId = SessionHelper.EnterPriceID;
                long CompanyID = SessionHelper.CompanyID;
                long RoomID = SessionHelper.RoomID;
                ToolImageDetailDAL toolImageDetailDAL = new ToolImageDetailDAL(SessionHelper.EnterPriseDBName);
                ToolImageDetailDTO ToolImageDetails = new ToolImageDetailDTO();
                Guid toolGuid = Guid.Empty;
                Guid.TryParse(ToolGuid, out toolGuid);
                ToolImageDetails.ToolGuid = toolGuid;
                ToolImageDetails.IsDeleted = false;
                ToolImageDetails.IsArchived = false;
                ToolImageDetails.AddedFrom = "Web";
                ToolImageDetails.EditedFrom = "Web";
                ToolImageDetails.CreatedBy = SessionHelper.UserID;
                ToolImageDetails.LastUpdatedBy = SessionHelper.UserID;
                ToolImageDetails.RoomId = SessionHelper.RoomID;
                ToolImageDetails.CompanyId = SessionHelper.CompanyID;

                foreach (string file in httpRequest.Files)
                {

                    var postedFile = httpRequest.Files[file];
                    string Imagename = Path.GetFileName(postedFile.FileName);
                    if (!string.IsNullOrEmpty(GuidForSeparator))
                    {
                        SerialNumber = Imagename.Substring(Imagename.LastIndexOf(GuidForSeparator) + (GuidForSeparator.Length + 1));
                        Imagename = Imagename.Substring(0, Imagename.LastIndexOf("_" + GuidForSeparator));
                        //postedFile.FileName = Imagename;
                    }

                    if (!string.IsNullOrEmpty(GuidForSeparator))
                    {
                        long imageId;
                        bool IsRecordExistForCurrentSerial = toolImageDetailDAL.IsToolImageRecordExistForSerial(SessionHelper.CompanyID, SessionHelper.RoomID, toolGuid, SerialNumber);

                        if (IsRecordExistForCurrentSerial)
                        {
                            imageId = toolImageDetailDAL.UpdateToolImageDetailForSerial(SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, toolGuid, SerialNumber, Imagename, "ImagePath");
                        }
                        else
                        {
                            ToolImageDetails.SerialNumber = SerialNumber;
                            ToolImageDetails.ImagePath = Imagename;
                            ToolImageDetails.ImageType = "ImagePath"; // TODO: at the time of implementing External image, this value should be dynamic(ImagePath/ExternalImage).
                            ToolImageDetails.ReceivedOn = DateTimeUtility.DateTimeNow;
                            ToolImageDetails.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            ToolImageDetails.Created = DateTimeUtility.DateTimeNow;
                            ToolImageDetails.Updated = DateTimeUtility.DateTimeNow;
                            imageId = toolImageDetailDAL.Insert(ToolImageDetails);
                        }

                        string toolImagePath = HttpContext.Current.Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyID + "/" + RoomID + "/" + (Id ?? 0).ToString() + "/" + imageId.ToString() + "/");

                        if (!Directory.Exists(toolImagePath))
                        {
                            Directory.CreateDirectory(toolImagePath);
                        }
                        var filePath = (toolImagePath + "/" + Imagename);
                        postedFile.SaveAs(filePath);
                    }
                    else
                    {
                        ToolImageDetails.SerialNumber = SerialNumber;
                        ToolImageDetails.ImagePath = Imagename;
                        ToolImageDetails.ImageType = "ImagePath"; // TODO: at the time of implementing External image, this value should be dynamic(ImagePath/ExternalImage).
                        ToolImageDetails.ReceivedOn = DateTimeUtility.DateTimeNow;
                        ToolImageDetails.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        ToolImageDetails.Created = DateTimeUtility.DateTimeNow;
                        ToolImageDetails.Updated = DateTimeUtility.DateTimeNow;
                        var imageId = toolImageDetailDAL.Insert(ToolImageDetails);
                        string toolImagePath = HttpContext.Current.Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyID + "/" + RoomID + "/" + (Id ?? 0).ToString() + "/" + imageId.ToString() + "/");

                        if (!Directory.Exists(toolImagePath))
                        {
                            Directory.CreateDirectory(toolImagePath);
                        }
                        var filePath = (toolImagePath + "/" + Imagename);
                        postedFile.SaveAs(filePath);
                    }

                }
                result = Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }


    }
    public class MyMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        static string Name = "";
        public MyMultipartFormDataStreamProvider(string path)
            : base(path)
        {

        }

        public MyMultipartFormDataStreamProvider(string path, string name)
            : base(path)
        {
            Name = name;
        }
        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            string fileName;
            fileName = Name;
            return fileName.Replace("\"", string.Empty);
        }
    }


}