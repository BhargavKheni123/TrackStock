using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using System.IO;
using eTurnsMaster.DAL;
using eTurns.DAL;
using eTurns.DTO;
using eTurnsWeb.Helper;
using Ionic.Zip;
using System.Threading;
using System.Configuration;
using System.Xml.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

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
                            objEnterpriseMasterDAL.updateLogoName(id ?? 0, Path.GetFileName(info.FullName));
                            return "File uploaded as " + info.FullName + " (" + info.Length + ")";
                        });
                        return fileInfo;

                    });
                return task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));
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
                        return "File uploaded as " + info.FullName + " (" + info.Length + ")";
                    });
                    return fileInfo;

                });
                return task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));
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
                        return "File uploaded as " + info.FullName + " (" + info.Length + ")";
                    });
                    return fileInfo;
                });

                return task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));
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
                XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = Settinfile.Element("InventoryLink2").Value;
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
                    var filePath = (LogoPath + "\\" + Path.GetFileName(postedFile.FileName));
                    ItemMasterDAL objItemyMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    objItemyMasterDAL.updateLink2Name(id ?? 0, Path.GetFileName(postedFile.FileName));

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
            XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
            if (httpRequest.Files.Count > 0)
            {
                string uploadFor = "InventoryPhoto";
                string tableName = Convert.ToString(HttpContext.Current.Request["tableName"]);
                if (!string.IsNullOrEmpty(tableName))
                {
                    switch (tableName)
                    {
                        case "Items":
                            uploadFor = Settinfile.Element("InventoryPhoto").Value;
                            break;
                        case "Assets":
                            uploadFor = Settinfile.Element("AssetPhoto").Value;
                            break;
                        case "Tools":
                            uploadFor = Settinfile.Element("ToolPhoto").Value;
                            break;
                        case "Suppliers":
                            uploadFor = Settinfile.Element("SupplierPhoto").Value;
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
                string DestPath = HttpContext.Current.Server.MapPath(uploadFor + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID.ToString() + "/" + SessionHelper.RoomID.ToString() + "/");
                foreach (string item in ItemIDs)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        string Id = item.Split('#')[0];
                        string FileName = item.Split('#')[1];
                        string SourceP = DestPath + Id;

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
                            if (!File.Exists(SourceP + "/" + FileName))
                            {
                                string noImagePath = Settinfile.Element("NoImage").Value;
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
                File.Delete(path);// delete uploaded zip file
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
            XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
            string InventoryLink2 = Settinfile.Element("InventoryLink2").Value;
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
                string DestPath = HttpContext.Current.Server.MapPath(InventoryLink2 + "/" + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/");
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
                    var filePath = HttpContext.Current.Server.MapPath("~/Uploads/EnterpriseLogos/" + (id ?? 0).ToString() + "/" + Path.GetFileName(postedFile.FileName));
                    EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                    postedFile.SaveAs(filePath);
                    objEnterpriseMasterDAL.updateLogoName(id ?? 0, Path.GetFileName(postedFile.FileName));

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
                System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = Settinfile.Element("SupplierPhoto").Value;

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
                    var filePath = (LogoPath + Path.GetFileName(postedFile.FileName));
                    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    objSupplierMasterDAL.updateImageName(id ?? 0, Path.GetFileName(postedFile.FileName));

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
                System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = Settinfile.Element("InventoryPhoto").Value;
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
                    string FileName = Path.GetFileName(postedFile.FileName);
                    var filePath = (LogoPath + FileName);
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);

                    Image imgPhoto = Image.FromFile(filePath);

                    Int32 SmallImageHeight = Settinfile.Element("SmallImageHeight") != null ? Convert.ToInt32(Settinfile.Element("SmallImageHeight").Value) : 100;
                    Int32 SmallImageWidth = Settinfile.Element("SmallImageWidth") != null ? Convert.ToInt32(Settinfile.Element("SmallImageWidth").Value) : 100;
                    Int32 MediumImageHeight = Settinfile.Element("MediumImageHeight") != null ? Convert.ToInt32(Settinfile.Element("MediumImageHeight").Value) : 400;
                    Int32 MediumImageWidth = Settinfile.Element("MediumImageWidth") != null ? Convert.ToInt32(Settinfile.Element("MediumImageWidth").Value) : 400;
                    Int32 LargeImageHeight = Settinfile.Element("LargeImageHeight") != null ? Convert.ToInt32(Settinfile.Element("LargeImageHeight").Value) : 1000;
                    Int32 LargeImageWidth = Settinfile.Element("LargeImageWidth") != null ? Convert.ToInt32(Settinfile.Element("LargeImageWidth").Value) : 1000;
                    Int32 BarcodeImageHeight = Settinfile.Element("BarcodeImageHeight") != null ? Convert.ToInt32(Settinfile.Element("BarcodeImageHeight").Value) : 300;
                    Int32 BarcodeImageWidth = Settinfile.Element("BarcodeImageWidth") != null ? Convert.ToInt32(Settinfile.Element("BarcodeImageWidth").Value) : 600;

                    

                    if (!Directory.Exists(LogoPath + "Small\\"))
                    {
                        Directory.CreateDirectory(LogoPath + "Small\\");
                    }
                    if (!Directory.Exists(LogoPath + "Medium\\"))
                    {
                        Directory.CreateDirectory(LogoPath + "Medium\\");
                    }
                    if (!Directory.Exists(LogoPath + "Large\\"))
                    {
                        Directory.CreateDirectory(LogoPath + "Large\\");
                    }
                    if (!Directory.Exists(LogoPath + "Barcode\\"))
                    {
                        Directory.CreateDirectory(LogoPath + "Barcode\\");
                    }

                    Bitmap imageSmall = ResizeImage(imgPhoto, SmallImageWidth, SmallImageHeight);
                    imageSmall.Save(LogoPath+ "Small\\" + FileName);
                    Bitmap imageMedium = ResizeImage(imgPhoto, MediumImageWidth, MediumImageHeight);
                    imageMedium.Save(LogoPath + "Medium\\" + FileName);
                    Bitmap imageLasrge = ResizeImage(imgPhoto, LargeImageWidth, LargeImageHeight);
                    imageLasrge.Save(LogoPath + "Large\\" + FileName);
                    Bitmap imageBarcode = ResizeImage(imgPhoto, BarcodeImageWidth, BarcodeImageHeight);
                    imageBarcode.Save(LogoPath + "Barcode\\" + FileName);

                    objItemMasterDAL.updateImagePath(id ?? 0, FileName);

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
                System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = Settinfile.Element("ToolPhoto").Value;
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
                    var filePath = (LogoPath + "/" + Path.GetFileName(postedFile.FileName));
                    ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    objToolMasterDAL.updateImagePath(id ?? 0, Path.GetFileName(postedFile.FileName));

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
                XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = Settinfile.Element("AssetPhoto").Value;
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
                    var filePath = (LogoPath + "\\" + Path.GetFileName(postedFile.FileName));
                    AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    objAssetMasterDAL.updateImagePath(id ?? 0, Path.GetFileName(postedFile.FileName));

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
                    var filePath = HttpContext.Current.Server.MapPath("~/Uploads/CompanyLogos/" + (id ?? 0).ToString() + "/" + Path.GetFileName(postedFile.FileName));
                    CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                    postedFile.SaveAs(filePath);
                    objCompanyMasterDAL.updateLogoName(id ?? 0, Path.GetFileName(postedFile.FileName));

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

        public HttpResponseMessage PostHelpDocumentFile(long? id, string FileTypeName)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                // string FileTypeName = "enterpriselogo.jpg";
                string LogoPath = HttpContext.Current.Server.MapPath("~/Uploads/HelpDoc");
                if (Directory.Exists(LogoPath))
                {

                }
                else
                {
                    Directory.CreateDirectory(LogoPath);
                }
                foreach (string file in httpRequest.Files)
                {
                    if (httpRequest.Files[file].ContentLength > 0)
                    {
                        var postedFile = httpRequest.Files[file];
                        var filePath = HttpContext.Current.Server.MapPath("~/Uploads/HelpDoc/" + FileTypeName);
                        CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                        postedFile.SaveAs(filePath);
                        //objCompanyMasterDAL.updateLogoName(id ?? 0, Path.GetFileName(postedFile.FileName));
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
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
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
                Guid WorkOrderGuid = objWorkrderDAL.GetWorkOrderGuid(Id ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                //  string WorkOrderPath = HttpContext.Current.Server.MapPath("~/Uploads/" + WorkOrderFilePath + "/" + SessionHelper.CompanyID + "/" + (Id ?? 0).ToString());
                //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["WorkOrderFilePaths"].ToString();
                System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string UNCPathRoot = Settinfile.Element("WorkOrderFilePaths").Value;
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