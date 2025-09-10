using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class PdaColumnsSetupController : eTurnsControllerBase
    {
        public ActionResult PdaColumnSetup()
        {
            var gridlist = new PdaColumnsetup(SessionHelper.EnterPriseDBName);
            List<PdaGridColumsDto> lstgridcolumns = gridlist.GetPdaGridList().OrderBy(o => o.TableName).ToList();
            ViewBag.GridColumnList = lstgridcolumns;
            return View();
        }
        public ActionResult GetGridListColumns(string listname, string tablename)
        {
            var gridlist = new PdaColumnsetup(SessionHelper.EnterPriseDBName);

            PdaGridColumsDto objsettings = gridlist.GetSettingrecordbytable(tablename, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
            string listids = string.Empty;
            if (objsettings != null)
            {
                listids = objsettings.ListIds;
            }

            List<UserMasterDTO> AllUser = new List<UserMasterDTO>();
            List<UserMasterDTO> TableUser = new List<UserMasterDTO>();
            if (eTurnsWeb.Helper.SessionHelper.UserType <= 2)
            {
                AllUser = gridlist.GetUsersByRoomAndCompany(SessionHelper.CompanyID, SessionHelper.RoomID);
                TableUser = gridlist.GetUsersByPDASearchSettingsTableName(tablename, SessionHelper.CompanyID, SessionHelper.RoomID);
            }

            if (listids == string.Empty && eTurnsWeb.Helper.SessionHelper.UserType <= 2 && TableUser.Count > 0)
            {
                objsettings = gridlist.GetSettingrecordbytable(tablename, SessionHelper.CompanyID, SessionHelper.RoomID, TableUser[0].ID);
                if (objsettings != null)
                {
                    listids = objsettings.ListIds;
                }
            }

            List<PdaGridColumsDto> lstgridcolumnssource = gridlist.GetPDAGridColumnListByListNamePlain(System.Web.HttpUtility.UrlDecode(listname), listids).OrderBy(t => t.GridColumnName).ToList();
            List<PdaGridColumsDto> lstgridcolumnsdestination = gridlist.GetRecordbylistnameDestination(System.Web.HttpUtility.UrlDecode(listname), tablename, listids);
            if (lstgridcolumnssource != null)
            {
                foreach (PdaGridColumsDto source in lstgridcolumnssource)
                {
                    if (source.GridColumnName.Contains("UDF"))
                    {
                        string Table = source.GridColumnName.Replace("UDF1", "").Replace("UDF2", "").Replace("UDF2", "").Replace("UDF3", "").Replace("UDF4", "").Replace("UDF5", "");
                        string UDFField = source.GridColumnName;
                        int ResourcePageId = 0;
                        if (string.IsNullOrEmpty(Table.Trim()))
                            ResourcePageId = SessionHelper.GetUDFListID(tablename);
                        else
                        {
                            switch (Table.Trim())
                            {
                                case "Item":
                                    Table = "ItemMaster";
                                    break;
                                case "Pull":
                                    Table = "PullMaster";
                                    break;


                            }
                            UDFField = UDFField.Replace("Item", "").Replace("Pull", "").Trim();
                            ResourcePageId = SessionHelper.GetUDFListID(Table);
                        }
                        if (ResourcePageId > 0)
                        {
                            MobileResourcesDAL objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                            int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                            List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(SessionHelper.RoomID, SessionHelper.CompanyID, languageId, ResourcePageId).ToList();


                            if (MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == UDFField && i.LanguageID == languageId).Any())
                            {
                                source.GridColumnName = MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == UDFField && i.LanguageID == languageId).FirstOrDefault().ResourceValue;
                            }

                        }

                    }
                }
            }
            if (lstgridcolumnsdestination != null)
            {
                foreach (PdaGridColumsDto source in lstgridcolumnsdestination)
                {
                    if (source.GridColumnName.Contains("UDF"))
                    {
                        string Table = source.GridColumnName.Replace("UDF1", "").Replace("UDF2", "").Replace("UDF2", "").Replace("UDF3", "").Replace("UDF4", "").Replace("UDF5", "");
                        string UDFField = source.GridColumnName;
                        int ResourcePageId = 0;
                        if (string.IsNullOrEmpty(Table.Trim()))
                            ResourcePageId = SessionHelper.GetUDFListID(tablename);
                        else
                        {
                            switch (Table.Trim())
                            {
                                case "Item":
                                    Table = "ItemMaster";
                                    break;
                                case "Pull":
                                    Table = "PullMaster";
                                    break;


                            }
                            UDFField = UDFField.Replace("Item", "").Replace("Pull", "").Trim();
                            ResourcePageId = SessionHelper.GetUDFListID(Table);
                        }
                        if (ResourcePageId > 0)
                        {
                            MobileResourcesDAL objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                            int languageId = SessionHelper.GetLanguageListID(Convert.ToString(ResourceHelper.CurrentCult));
                            List<MobileResourcesDTO> MobileResourcesList = objMobileResourcesDAL.GetAllMobileResourceRecordsFiltered(SessionHelper.RoomID, SessionHelper.CompanyID, languageId, ResourcePageId).ToList();


                            if (MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == UDFField && i.LanguageID == languageId).Any())
                            {
                                source.GridColumnName = MobileResourcesList.Where(i => i.Roomid == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID && i.ResourcePageID == ResourcePageId && i.ResourceKey == UDFField && i.LanguageID == languageId).FirstOrDefault().ResourceValue;
                            }

                        }

                    }
                }
            }

            if (lstgridcolumnssource != null && lstgridcolumnssource.Count > 0)
            {
                lstgridcolumnssource = lstgridcolumnssource.OrderBy(t => t.GridColumnName).ToList();
            }

            ViewBag.SourceList = lstgridcolumnssource;
            ViewBag.DestinationList = lstgridcolumnsdestination;
            ViewBag.DestinationListOrder = listids;
            ViewBag.UserList = AllUser; // All usertype = 3
            ViewBag.TableUser = TableUser; // Only User which is from Searchsettings

            return PartialView("PdaColumnList");
        }
        [HttpPost]
        public ActionResult SavePdaSettings(string datacollections, string listname, string tablename, string UserValues, string PreviousUserValues)
        {
            var savedata = new PdaColumnsetup(SessionHelper.EnterPriseDBName);
            datacollections = System.Web.HttpUtility.UrlDecode(datacollections);
            if (!string.IsNullOrEmpty(tablename))
            {
                var myDeserializedObjList = (List<PdaGridColumsDto>)JsonConvert.DeserializeObject(datacollections, typeof(List<PdaGridColumsDto>));
                if (myDeserializedObjList.Any())
                {
                    // Remove unSelected user
                    if (!string.IsNullOrEmpty(PreviousUserValues))
                    {
                        string strRemove = string.Empty;
                        List<string> lstPrevUsers = PreviousUserValues.Split(',').ToList<string>();
                        foreach (string strPrevUsers in lstPrevUsers)
                        {
                            if (UserValues.Contains(strPrevUsers) == false && strPrevUsers != SessionHelper.UserID.ToString())
                                strRemove += strPrevUsers + ",";
                        }
                        if (strRemove != string.Empty)
                        {
                            strRemove = strRemove.Substring(0, strRemove.Length - 1);
                            savedata.RemoveSearchSettingColumns(tablename, SessionHelper.CompanyID, SessionHelper.RoomID, strRemove);
                        }
                    }
                    // end
                    List<string> lstUsers = new List<string>();
                    if (!string.IsNullOrEmpty(UserValues))
                    {
                        lstUsers = UserValues.Split(',').ToList<string>();
                    }
                    lstUsers.Add(Convert.ToString(SessionHelper.UserID));

                    foreach (string strUser in lstUsers)
                    {
                        myDeserializedObjList[0].CompanyID = SessionHelper.CompanyID;
                        myDeserializedObjList[0].Room = SessionHelper.RoomID;
                        myDeserializedObjList[0].UserID = Convert.ToInt64(strUser); //SessionHelper.UserID;

                        savedata.SaveData(myDeserializedObjList, tablename);
                    }
                }
            }
            listname = System.Web.HttpUtility.UrlDecode(listname);
            return RedirectToAction("GetGridListColumns", new { listname, tablename });
        }
    }
}
