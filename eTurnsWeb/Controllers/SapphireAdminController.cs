using eTurns.DTO;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class SapphireAdminController : eTurnsControllerBase
    {
        [HttpGet]
        public ActionResult BuildConnection()
        {
            if (SessionHelper.RoleID == -1)
            {
                List<OLEDBConnectionInfo> lstOLEDBConnectionsdecrypted = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName).GetAllConnectionparams();
                return View(lstOLEDBConnectionsdecrypted);
            }
            else
            {
                return RedirectToAction("MyProfile", "Master");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuildConnectionSave(List<OLEDBConnectionInfo> lstOLEDBConnections)
        {
            if (lstOLEDBConnections != null)
            {
                lstOLEDBConnections.ForEach(t => { t.CreatedBy = SessionHelper.UserID; t.UpdatedBy = SessionHelper.UserID; t.Created = DateTime.UtcNow; t.Updated = DateTime.UtcNow; });
                new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName).SaveConnectionParams(lstOLEDBConnections);
            }

            //SecHelper objSecHelper = new SecHelper();
            //EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL(SessionHelper.EnterPriseDBName);
            //List<OLEDBConnectionInfo> lstOLEDBConnectionsEncrypted = new List<OLEDBConnectionInfo>();
            //if (lstOLEDBConnections != null && lstOLEDBConnections.Count > 0)
            //{
            //    string EnKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["Enkey"]);
            //    OLEDBConnectionInfo objOLEDBConnectionInfo;
            //    foreach (var item in lstOLEDBConnections)
            //    {
            //        objOLEDBConnectionInfo = new OLEDBConnectionInfo();
            //        objOLEDBConnectionInfo.ID = item.ID;
            //        objOLEDBConnectionInfo.ConectionType = objSecHelper.EncryptData((item.ConectionType ?? string.Empty), EnKey);
            //        objOLEDBConnectionInfo.APP = objSecHelper.EncryptData(item.APP ?? string.Empty, EnKey);
            //        objOLEDBConnectionInfo.ApplicationIntent = objSecHelper.EncryptData(item.ApplicationIntent ?? string.Empty, EnKey);
            //        objOLEDBConnectionInfo.AppDatabase = objSecHelper.EncryptData(item.AppDatabase ?? string.Empty, EnKey);
            //        objOLEDBConnectionInfo.MarsConn = objSecHelper.EncryptData(item.MarsConn ?? string.Empty, EnKey);
            //        objOLEDBConnectionInfo.PacketSize = objSecHelper.EncryptData(item.PacketSize ?? string.Empty, EnKey);
            //        objOLEDBConnectionInfo.PWD = objSecHelper.EncryptData(item.PWD ?? string.Empty, EnKey);
            //        objOLEDBConnectionInfo.Server = objSecHelper.EncryptData(item.Server ?? string.Empty, EnKey);
            //        objOLEDBConnectionInfo.Timeout = objSecHelper.EncryptData(item.Timeout ?? string.Empty, EnKey);
            //        objOLEDBConnectionInfo.Trusted_Connection = objSecHelper.EncryptData(item.Trusted_Connection ?? string.Empty, EnKey);
            //        objOLEDBConnectionInfo.UID = objSecHelper.EncryptData(item.UID ?? string.Empty, EnKey);
            //        objOLEDBConnectionInfo.FailoverPartner = objSecHelper.EncryptData(item.FailoverPartner ?? string.Empty, EnKey);
            //        objOLEDBConnectionInfo.PersistSensitive = objSecHelper.EncryptData(item.PersistSensitive ?? string.Empty, EnKey);
            //        objOLEDBConnectionInfo.UpdatedBy = SessionHelper.UserID;
            //        objOLEDBConnectionInfo.Updated = DateTime.UtcNow;
            //        lstOLEDBConnectionsEncrypted.Add(objOLEDBConnectionInfo);
            //    }
            //    lstOLEDBConnections = objEnterPriseUserMasterDAL.SaveCons(lstOLEDBConnectionsEncrypted);
            //    List<OLEDBConnectionInfo> lstOLEDBConnectionsdecrypted = (from t in lstOLEDBConnections
            //                                                              select new OLEDBConnectionInfo
            //                                                              {
            //                                                                  ID = t.ID,
            //                                                                  ConectionType = t.ConectionType,
            //                                                                  APP = objSecHelper.DecryptData(t.APP, EnKey),
            //                                                                  ApplicationIntent = objSecHelper.DecryptData(t.ApplicationIntent, EnKey),
            //                                                                  AppDatabase = objSecHelper.DecryptData(t.AppDatabase, EnKey),
            //                                                                  MarsConn = objSecHelper.DecryptData(t.MarsConn, EnKey),
            //                                                                  PacketSize = objSecHelper.DecryptData(t.PacketSize, EnKey),
            //                                                                  PWD = objSecHelper.DecryptData(t.PWD, EnKey),
            //                                                                  Server = objSecHelper.DecryptData(t.Server, EnKey),
            //                                                                  Timeout = objSecHelper.DecryptData(t.Timeout, EnKey),
            //                                                                  Trusted_Connection = objSecHelper.DecryptData(t.Trusted_Connection, EnKey),
            //                                                                  UID = objSecHelper.DecryptData(t.UID, EnKey),
            //                                                                  FailoverPartner = objSecHelper.DecryptData(t.FailoverPartner, EnKey),
            //                                                                  PersistSensitive = objSecHelper.DecryptData(t.PersistSensitive, EnKey),
            //                                                                  Created = t.Created,
            //                                                                  Updated = t.Updated,
            //                                                                  CreatedBy = t.CreatedBy,
            //                                                                  UpdatedBy = t.UpdatedBy,
            //                                                                  GUID = t.GUID

            //                                                              }).ToList();



            //}
            return RedirectToAction("BuildConnection", "SapphireAdmin");
        }
    }
}
