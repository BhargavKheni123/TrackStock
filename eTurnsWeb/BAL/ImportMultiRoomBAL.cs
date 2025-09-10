using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using eTurns.DTO.Utils;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eTurnsWeb.validation;
using eTurnsWeb.Controllers;
using Microsoft.SqlServer.Management.Sdk.Differencing.SPI;
using System.Net.Mail;

namespace eTurnsWeb.BAL
{
    public class ImportMultiRoomBAL : IDisposable
    {
        #region Properties
        public Action ClearCurrentResourceList { get; set; }
        #endregion

        #region Constructor

        public ImportMultiRoomBAL(Action clearCurrentResourceList)
        {
            this.ClearCurrentResourceList = clearCurrentResourceList;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// ItemSupplierDetails
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="CurrentOptionList"></param>
        /// <param name="CurrentItemSupplierList"></param>
        /// <param name="TableName"></param>
        /// <param name="para"></param>
        /// <param name="HasMoreRecords"></param>
        /// <param name="IsFirstCall"></param>
        /// <param name="message"></param>
        /// <param name="status"></param>
        /// <param name="allSuccesfulRecords"></param>
        public void SaveItemSupplier(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
            List<ItemSupplier> CurrentItemSupplierList,
            ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
            bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            {
                List<ItemSupplier> CurrentBlankItemSuppliermain = new List<ItemSupplier>();
                ItemSupplier[] LstItemSuppliermain = serializer.Deserialize<ItemSupplier[]>(para);
                if (LstItemSuppliermain != null && LstItemSuppliermain.Length > 0)
                {
                    CurrentBlankItemSuppliermain = new List<ItemSupplier>();

                    ItemSupplier objItemSupplierDAL = new ItemSupplier();
                    CurrentItemSupplierList = new List<ItemSupplier>();
                    List<SupplierMasterDTO> objSupplierMasterDALList = new List<SupplierMasterDTO>();
                    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                    objSupplierMasterDALList = objSupplierMasterDAL.GetSupplierByRoomPlain(RoomID, SessionHelper.CompanyID, false).ToList();
                    foreach (ItemSupplier SupplierMasterList in LstItemSuppliermain.GroupBy(l => l.SupplierName).Select(g => g.First()).ToList())
                    {
                        if (SupplierMasterList.SupplierName.ToLower().Trim() != string.Empty)
                        {
                            if ((from p in objSupplierMasterDALList
                                 where (p.SupplierName.ToLower().Trim() == (SupplierMasterList.SupplierName.ToLower().Trim()) && p.isForBOM == false && p.IsDeleted == false && p.Room == RoomID && p.CompanyID == SessionHelper.CompanyID)
                                 select p).Any())
                            {

                            }
                            else
                            {
                                SupplierMasterDTO objSupplierMasterDTO = new SupplierMasterDTO();
                                objSupplierMasterDTO.SupplierName = SupplierMasterList.SupplierName.Trim();

                                objSupplierMasterDTO.Room = RoomID;
                                objSupplierMasterDTO.CompanyID = SessionHelper.CompanyID;
                                objSupplierMasterDTO.CreatedBy = SessionHelper.UserID;
                                objSupplierMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                objSupplierMasterDTO.Created = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.AddedFrom = "Web";
                                objSupplierMasterDTO.EditedFrom = "Web";
                                objSupplierMasterDTO.GUID = Guid.NewGuid();
                                objSupplierMasterDTO.IsArchived = false;
                                objSupplierMasterDTO.IsDeleted = false;
                                objSupplierMasterDTO.isForBOM = false;
                                objSupplierMasterDTO.RefBomId = null;
                                objSupplierMasterDTO.IsEmailPOInBody = false;
                                objSupplierMasterDTO.IsEmailPOInPDF = false;
                                objSupplierMasterDTO.IsEmailPOInCSV = false;
                                objSupplierMasterDTO.IsEmailPOInX12 = false;
                                objSupplierMasterDTO.IsSendtoVendor = false;
                                objSupplierMasterDTO.IsVendorReturnAsn = false;
                                objSupplierMasterDTO.IsSupplierReceivesKitComponents = false;
                                Int64 SupplierMasterID = objSupplierMasterDAL.Insert(objSupplierMasterDTO);

                                // Start -- insert into Room Schedule for PullSchedule type is Immediate 
                                if (SupplierMasterID > 0)
                                {
                                    SchedulerDTO objSchedulerDTO = objSupplierMasterDAL.GetRoomSchedule(SupplierMasterID, objSupplierMasterDTO.Room.GetValueOrDefault(0), 7);
                                    if (objSchedulerDTO == null)
                                    {
                                        // insert into Room Schedule for PullSchedule type is Immediate 
                                        SchedulerDTO objPullSchedulerDTO = new SchedulerDTO();
                                        objPullSchedulerDTO.SupplierId = SupplierMasterID;
                                        objPullSchedulerDTO.CompanyId = objSupplierMasterDTO.CompanyID.GetValueOrDefault(0);
                                        objPullSchedulerDTO.RoomId = objSupplierMasterDTO.Room.GetValueOrDefault(0);
                                        objPullSchedulerDTO.LoadSheduleFor = 7;
                                        objPullSchedulerDTO.ScheduleMode = 5;
                                        objPullSchedulerDTO.IsScheduleActive = true;
                                        objPullSchedulerDTO.MonthlyDayOfMonth = 2;
                                        objSupplierMasterDAL.SaveSupplierSchedule(objPullSchedulerDTO);
                                    }
                                }

                                /// End- logic for insert into Room Schedule for PullSchedule type is Immediate


                                objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByIDPlain(SupplierMasterID);
                                objSupplierMasterDALList.Add(objSupplierMasterDTO);

                            }
                        }
                    }
                    bool SaveToolList = true;
                    foreach (ItemSupplier item in LstItemSuppliermain)
                    {
                        SaveToolList = true;

                        ItemSupplier objDTO = new ItemSupplier();
                        Guid? ItemGUID = GetGUID(ImportMastersDTO.TableName.ItemMaster, item.ItemNumber, RoomID: RoomID);

                        if ((objDTO.SupplierName != null || objDTO.SupplierName != string.Empty))
                        {
                            if (ItemGUID.HasValue && ItemGUID != Guid.Empty)
                            {
                                objDTO.ItemGUID = ItemGUID.Value;
                                objDTO.SupplierName = item.SupplierName.Trim();
                                //ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                                //if (objItemSupplierDetailsDAL.CheckSupplierDuplicate(item.SupplierNumber.Trim(), RoomID, SessionHelper.CompanyID, objDTO.ItemGUID))
                                //{

                                objDTO.SupplierNumber = item.SupplierNumber;
                                //}
                                //else
                                //{
                                //    SaveToolList = false;
                                //    objDTO = item;
                                //    objDTO.Status = "Fail";

                                //    objDTO.Reason = item.SupplierNumber.Trim() + " SupplierNumber is already exists.";
                                //}

                                objDTO.IsDefault = item.IsDefault;
                                objDTO.SupplierID = objSupplierMasterDALList.ToList().Where(m => m.SupplierName.ToLower().Trim() == item.SupplierName.ToLower().Trim() && m.Room == RoomID && m.CompanyID == SessionHelper.CompanyID && m.IsDeleted == false && m.IsArchived == false && m.isForBOM == false).FirstOrDefault().ID;
                                objDTO.IsDeleted = false;
                                objDTO.IsArchived = false;
                                objDTO.Created = DateTimeUtility.DateTimeNow;
                                objDTO.Updated = DateTimeUtility.DateTimeNow;
                                objDTO.LastUpdatedBy = SessionHelper.UserID;
                                objDTO.Room = RoomID;
                                objDTO.CompanyID = SessionHelper.CompanyID;
                                objDTO.CreatedBy = SessionHelper.UserID;
                                objDTO.GUID = Guid.NewGuid();
                                objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                if (!string.IsNullOrEmpty(item.BlanketPOName))
                                {
                                    objDTO.BlanketPOID = GetIDs(ImportMastersDTO.TableName.SupplierBlanketPODetails, Convert.ToString(item.BlanketPOName.Trim()), objDTO.SupplierID, RoomID: RoomID);
                                }
                                else
                                    objDTO.BlanketPOID = null;
                                objDTO.AddedFrom = "Web";
                                objDTO.EditedFrom = "Web";
                            }
                            if (ItemGUID.HasValue && ItemGUID != Guid.Empty && SaveToolList)
                            {
                                var itemval = CurrentItemSupplierList.FirstOrDefault(x => x.ItemGUID == objDTO.ItemGUID && x.SupplierName == objDTO.SupplierName);
                                if (itemval != null)
                                    CurrentItemSupplierList.Remove(itemval);
                                CurrentItemSupplierList.Add(objDTO);
                            }
                            else
                            {
                                objDTO = item;
                                if (ItemGUID.HasValue && ItemGUID == Guid.Empty)
                                {
                                    objDTO.Status = "Fail";
                                    objDTO.Reason = ResItemMaster.MsgItemDoesNotExist;
                                }
                                CurrentBlankItemSuppliermain.Add(objDTO);
                            }
                        }
                        else
                        {
                            objDTO = item;
                            objDTO.Status = "Fail";

                            objDTO.Reason = ResItemMaster.MsgSupplierNameRequired;


                            CurrentBlankItemSuppliermain.Add(objDTO);
                        }
                    }

                    List<ItemSupplier> lstreturn = new List<ItemSupplier>();
                    if (CurrentItemSupplierList.Count > 0)
                    {
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.ItemSupplierDetails.ToString(), CurrentItemSupplierList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }

                    if (CurrentBlankItemSuppliermain.Count > 0)
                    {
                        foreach (ItemSupplier item in CurrentBlankItemSuppliermain)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;

                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion


                        //Session["importedData"] = lstreturn;
                    }
                    if (CurrentItemSupplierList.Count > 0)
                    {
                        eTurns.DAL.CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.InvalidateCache();

                        eTurns.DAL.CacheHelper<IEnumerable<ItemSupplier>>.InvalidateCache();
                        ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                        List<ItemSupplierDetailsDTO> objItemSupplierDetailsDTO = new List<ItemSupplierDetailsDTO>();
                        objItemSupplierDetailsDTO = objItemSupplierDetailsDAL.GetSuppliersByRoomNormal(RoomID, SessionHelper.CompanyID, "ID DESC");
                        foreach (ItemSupplier objCurrentItemManu in CurrentItemSupplierList.GroupBy(l => l.ItemGUID).Select(g => g.First()).ToList())
                        {
                            foreach (ItemSupplierDetailsDTO objItemManufacturer in objItemSupplierDetailsDTO.ToList().Where(l => l.IsDefault == true && l.ItemGUID == objCurrentItemManu.ItemGUID))
                            {
                                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                                if (objItemManufacturer.ItemGUID != Guid.Empty)
                                    objItemMasterDTO = objItemMasterDAL.GetRecordOnlyItemsFields(objItemManufacturer.ItemGUID.ToString(), RoomID, SessionHelper.CompanyID);

                                if (objItemMasterDTO != null)
                                {
                                    objItemMasterDTO.SupplierID = objItemManufacturer.SupplierID;
                                    objItemMasterDTO.SupplierPartNo = objItemManufacturer.SupplierNumber;
                                    objItemMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                    objItemMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objItemMasterDAL.Edit(objItemMasterDTO, SessionUserId, SessionHelper.EnterPriceID);
                                }
                            }
                        }
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}
                    if (isLastRoom && ErrorRows.Count() > 0)
                    {
                        int SrNo = 0;
                        foreach (var item in lstreturn)
                        {
                            SrNo++;
                            if (ErrorRows.ContainsKey(SrNo))
                            {
                                item.Reason = ErrorRows[SrNo];
                                item.Status = "fail";
                            }
                        }
                        allSuccesfulRecords = false;
                    }

                    if (isLastRoom)
                     SaveImportDataListSession<ItemSupplier>(HasMoreRecords, IsFirstCall, lstreturn);
                    

                    HttpContext.Current.Session["ItemSupplier"] = null;


                }

            }

        }


        public void SaveLocationMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
           List<LocationMasterMain> CurrentLocationList,
           ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
           bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, string currentCulture, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            {
                List<LocationMasterMain> CurrentBlankLocationList = new List<LocationMasterMain>();
                LocationMasterMain[] LstLocationMaster = serializer.Deserialize<LocationMasterMain[]>(para);
                if (LstLocationMaster != null && LstLocationMaster.Length > 0)
                {
                    CurrentLocationList = new List<LocationMasterMain>();
                    //lst = obj.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.BinMaster.ToString(), UDFControlTypes.Textbox.ToString());
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.LocationMaster.ToString());

                    eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("LocationMaster", RoomID, SessionHelper.CompanyID);

                    CurrentOptionList = new List<UDFOptionsMain>();

                    foreach (LocationMasterMain item in LstLocationMaster)
                    {
                        LocationMasterMain objDTO = new LocationMasterMain();
                        objDTO.ID = item.ID;
                        objDTO.Location = item.Location;

                        bool saveLoc = true;
                        string errorMsg = string.Empty;
                        CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, out errorMsg, SessionHelper.EnterPriceID, SessionHelper.CompanyID, RoomID, currentCulture);
                        if (!string.IsNullOrWhiteSpace(errorMsg))
                        {
                            objDTO.Status = "Fail";
                            if (!string.IsNullOrEmpty(objDTO.Reason))
                                objDTO.Reason += errorMsg;
                            else
                                objDTO.Reason = errorMsg;
                            saveLoc = false;
                            item.Status = objDTO.Status;
                            item.Reason = errorMsg;
                        }


                        objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);
                        objDTO.UDF6 = (item.UDF6 == null) ? null : (item.UDF6.Length > 255 ? item.UDF6.Substring(0, 255) : item.UDF6);
                        objDTO.UDF7 = (item.UDF7 == null) ? null : (item.UDF7.Length > 255 ? item.UDF7.Substring(0, 255) : item.UDF7);
                        objDTO.UDF8 = (item.UDF8 == null) ? null : (item.UDF8.Length > 255 ? item.UDF8.Substring(0, 255) : item.UDF8);
                        objDTO.UDF9 = (item.UDF9 == null) ? null : (item.UDF9.Length > 255 ? item.UDF9.Substring(0, 255) : item.UDF9);
                        objDTO.UDF10 = (item.UDF10 == null) ? null : (item.UDF10.Length > 255 ? item.UDF10.Substring(0, 255) : item.UDF10);
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        if (item.Location.Trim() != "" && saveLoc)
                        {
                            CurrentLocationList.Add(objDTO);

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportLocationColumn.UDF1.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportLocationColumn.UDF2.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportLocationColumn.UDF3.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportLocationColumn.UDF4.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportLocationColumn.UDF5.ToString(), RoomID);
                            //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportBinColumn.UDF6.ToString());

                        }
                        else
                        {
                            CurrentBlankLocationList.Add(objDTO);
                        }
                    }

                    List<LocationMasterMain> lstreturn = new List<LocationMasterMain>();
                    if (CurrentLocationList.Count > 0)
                    {
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.LocationMaster.ToString(), CurrentLocationList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }
                    if (CurrentBlankLocationList.Count > 0)
                    {
                        foreach (LocationMasterMain item in CurrentBlankLocationList)
                        {
                            lstreturn.Add(item);
                        }
                    }
                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                       
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                        SaveImportDataListSession<LocationMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        
                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}
                }
            }
        }


        public void SaveManufacturerMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
   List<ManufacturerMasterMain> CurrentManufacturerList,
   ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
   bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, string currentCulture, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            {

                List<ManufacturerMasterMain> CurrentBlankManufacturerList = new List<ManufacturerMasterMain>();
                ManufacturerMasterMain[] LstManufacturerMaster = serializer.Deserialize<ManufacturerMasterMain[]>(para);
                if (LstManufacturerMaster != null && LstManufacturerMaster.Length > 0)
                {
                    CurrentManufacturerList = new List<ManufacturerMasterMain>();
                    //lst = obj.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.ManufacturerMaster.ToString(), UDFControlTypes.Textbox.ToString());
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.ManufacturerMaster.ToString());

                    eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("Manufacturers", RoomID, SessionHelper.CompanyID);

                    CurrentOptionList = new List<UDFOptionsMain>();

                    foreach (ManufacturerMasterMain item in LstManufacturerMaster)
                    {
                        ManufacturerMasterMain objDTO = new ManufacturerMasterMain();
                        objDTO.ID = item.ID;

                        bool saveMan = true;
                        string errorMsg = string.Empty;
                        CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, out errorMsg, SessionHelper.EnterPriceID, SessionHelper.CompanyID, RoomID, currentCulture);
                        if (!string.IsNullOrWhiteSpace(errorMsg))
                        {
                            objDTO.Status = "Fail";
                            if (!string.IsNullOrEmpty(objDTO.Reason))
                                objDTO.Reason += errorMsg;
                            else
                                objDTO.Reason = errorMsg;
                            saveMan = false;
                            item.Status = objDTO.Status;
                            item.Reason = errorMsg;
                        }

                        objDTO.Manufacturer = (item.Manufacturer == null) ? null : (item.Manufacturer.Length > 128 ? item.Manufacturer.Substring(0, 128) : item.Manufacturer);
                        objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);
                        objDTO.UDF6 = (item.UDF6 == null) ? null : (item.UDF6.Length > 255 ? item.UDF6.Substring(0, 255) : item.UDF6);
                        objDTO.UDF7 = (item.UDF7 == null) ? null : (item.UDF7.Length > 255 ? item.UDF7.Substring(0, 255) : item.UDF7);
                        objDTO.UDF8 = (item.UDF8 == null) ? null : (item.UDF8.Length > 255 ? item.UDF8.Substring(0, 255) : item.UDF8);
                        objDTO.UDF9 = (item.UDF9 == null) ? null : (item.UDF9.Length > 255 ? item.UDF9.Substring(0, 255) : item.UDF9);
                        objDTO.UDF10 = (item.UDF10 == null) ? null : (item.UDF10.Length > 255 ? item.UDF10.Substring(0, 255) : item.UDF10);
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        string errors = ValidateModuleDTO<ManufacturerMasterMain, ManufacturerMasterDTO>(objDTO, "Master", "ManufacturerCreate"
                                                                            , new List<string>() { "ID" }
                                                                            );

                        if (string.IsNullOrWhiteSpace(errors) == false)
                        {
                            objDTO.Status = "Fail";
                            objDTO.Reason += errors;
                            item.Status = "Fail";
                            item.Reason += errors;
                        }

                        if (string.IsNullOrWhiteSpace(errors) && item.Manufacturer.Trim() != "" && saveMan)
                        {
                            CurrentManufacturerList.Add(objDTO);

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportManufacturerColumn.UDF1.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportManufacturerColumn.UDF2.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportManufacturerColumn.UDF3.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportManufacturerColumn.UDF4.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportManufacturerColumn.UDF5.ToString(), RoomID);
                            // CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportManufacturerColumn.UDF6.ToString());

                        }
                        else
                        {
                            CurrentBlankManufacturerList.Add(objDTO);
                        }
                    }

                    List<ManufacturerMasterMain> lstreturn = new List<ManufacturerMasterMain>();
                    if (CurrentManufacturerList.Count > 0)
                    {
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.ManufacturerMaster.ToString(), CurrentManufacturerList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }

                    if (CurrentBlankManufacturerList.Count > 0)
                    {
                        foreach (ManufacturerMasterMain item in CurrentBlankManufacturerList)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<ManufacturerMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        
                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}
                    //CacheHelper<IEnumerable<ManufacturerMasterDTO>>.InvalidateCache();
                    //(new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);
                }

            }
        }

        /// <summary>
        /// TechnicianMaster
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="CurrentOptionList"></param>
        /// <param name="CurrentTechnicianList"></param>
        /// <param name="TableName"></param>
        /// <param name="para"></param>
        /// <param name="HasMoreRecords"></param>
        /// <param name="IsFirstCall"></param>
        /// <param name="message"></param>
        /// <param name="status"></param>
        /// <param name="allSuccesfulRecords"></param>
        public void SaveTechnicianMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
            List<TechnicianMasterMain> CurrentTechnicianList,
            ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
            bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, string currentCulture, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            //else if (ImportMastersDTO.TableName.TechnicianMaster.ToString() == TableName)
            {

                List<TechnicianMasterMain> CurrentBlankTechnicianList = new List<TechnicianMasterMain>();
                TechnicianMasterMain[] LstTechnicianMaster = serializer.Deserialize<TechnicianMasterMain[]>(para);
                if (LstTechnicianMaster != null && LstTechnicianMaster.Length > 0)
                {
                    CurrentTechnicianList = new List<TechnicianMasterMain>();
                    //lst = obj.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.TechnicianMaster.ToString(), UDFControlTypes.Textbox.ToString());
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.TechnicianMaster.ToString());

                    eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("Technicians", RoomID, SessionHelper.CompanyID);

                    CurrentOptionList = new List<UDFOptionsMain>();

                    foreach (TechnicianMasterMain item in LstTechnicianMaster)
                    {
                        TechnicianMasterMain objDTO = new TechnicianMasterMain();
                        objDTO.ID = item.ID;
                        objDTO.Technician = (item.Technician == null) ? null : (item.Technician.Length > 128 ? item.Technician.Substring(0, 128) : item.Technician);
                        objDTO.TechnicianCode = (item.TechnicianCode.Replace(" ", "") == null) ? null : (item.TechnicianCode.Replace(" ", "").Length > 128 ? item.TechnicianCode.Replace(" ", "").Substring(0, 128) : item.TechnicianCode.Replace(" ", ""));

                        bool saveTech = true;
                        string errorMsg = string.Empty;
                        CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, out errorMsg, SessionHelper.EnterPriceID, SessionHelper.CompanyID, RoomID, currentCulture);
                        if (!string.IsNullOrWhiteSpace(errorMsg))
                        {
                            objDTO.Status = "Fail";
                            if (!string.IsNullOrEmpty(objDTO.Reason))
                                objDTO.Reason += errorMsg;
                            else
                                objDTO.Reason = errorMsg;
                            saveTech = false;
                            item.Status = objDTO.Status;
                            item.Reason = errorMsg;
                        }


                        objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);
                        objDTO.UDF6 = (item.UDF6 == null) ? null : (item.UDF6.Length > 255 ? item.UDF6.Substring(0, 255) : item.UDF6);
                        objDTO.UDF7 = (item.UDF7 == null) ? null : (item.UDF7.Length > 255 ? item.UDF7.Substring(0, 255) : item.UDF7);
                        objDTO.UDF8 = (item.UDF8 == null) ? null : (item.UDF8.Length > 255 ? item.UDF8.Substring(0, 255) : item.UDF8);
                        objDTO.UDF9 = (item.UDF9 == null) ? null : (item.UDF9.Length > 255 ? item.UDF9.Substring(0, 255) : item.UDF9);
                        objDTO.UDF10 = (item.UDF10 == null) ? null : (item.UDF10.Length > 255 ? item.UDF10.Substring(0, 255) : item.UDF10);
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();

                        string errors = ValidateModuleDTO<TechnicianMasterMain, TechnicianMasterDTO>(objDTO, "Master", "TechnicianCreate"
                                                                            , new List<string>() { "ID" }
                                                                            );

                        if (string.IsNullOrWhiteSpace(errors) == false)
                        {
                            objDTO.Status = "Fail";
                            objDTO.Reason += errors;
                            item.Status = "Fail";
                            item.Reason += errors;
                        }

                        if (string.IsNullOrWhiteSpace(errors) && item.TechnicianCode.Trim() != "" && saveTech)
                        {
                            CurrentTechnicianList.Add(objDTO);

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportTechnicianColumn.UDF1.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportTechnicianColumn.UDF2.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportTechnicianColumn.UDF3.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportTechnicianColumn.UDF4.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportTechnicianColumn.UDF5.ToString(), RoomID);
                            // CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportTechnicianColumn.UDF6.ToString());

                        }
                        else
                        {
                            CurrentBlankTechnicianList.Add(objDTO);
                        }
                    }

                    List<TechnicianMasterMain> lstreturn = new List<TechnicianMasterMain>();
                    if (CurrentTechnicianList.Count > 0)
                    {
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.TechnicianMaster.ToString(), CurrentTechnicianList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }
                    if (CurrentBlankTechnicianList.Count > 0)
                    {
                        foreach (TechnicianMasterMain item in CurrentBlankTechnicianList)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<TechnicianMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        
                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}
                }

            }


        }

        public void SaveToolCategoryMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
            List<ToolCategoryMasterMain> CurrentToolCategoryList,
            ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
            bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, string currentCulture,long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            //if (ImportMastersDTO.TableName.ToolCategoryMaster.ToString() == TableName)
            {
                List<ToolCategoryMasterMain> CurrentBlankToolCategoryList = new List<ToolCategoryMasterMain>();
                ToolCategoryMasterMain[] LstToolCategoryMaster = serializer.Deserialize<ToolCategoryMasterMain[]>(para);
                if (LstToolCategoryMaster != null && LstToolCategoryMaster.Length > 0)
                {
                    CurrentToolCategoryList = new List<ToolCategoryMasterMain>();
                    //lst = obj.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.ToolCategoryMaster.ToString(), UDFControlTypes.Textbox.ToString());
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.ToolCategoryMaster.ToString());

                    eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("ToolCategories", RoomID, SessionHelper.CompanyID);
                    CurrentOptionList = new List<UDFOptionsMain>();

                    foreach (ToolCategoryMasterMain item in LstToolCategoryMaster)
                    {
                        ToolCategoryMasterMain objDTO = new ToolCategoryMasterMain();
                        objDTO.ID = item.ID;
                        objDTO.ToolCategory = item.ToolCategory;
                        bool saveToolCat = true;
                        string errorMsg = string.Empty;
                        CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, out errorMsg, SessionHelper.EnterPriceID, SessionHelper.CompanyID, RoomID, currentCulture);
                        if (!string.IsNullOrWhiteSpace(errorMsg))
                        {
                            objDTO.Status = "Fail";
                            if (!string.IsNullOrEmpty(objDTO.Reason))
                                objDTO.Reason += errorMsg;
                            else
                                objDTO.Reason = errorMsg;
                            saveToolCat = false;
                            item.Status = objDTO.Status;
                            item.Reason = errorMsg;
                        }

                        objDTO.UDF1 = item.UDF1;
                        objDTO.UDF2 = item.UDF2;
                        objDTO.UDF3 = item.UDF3;
                        objDTO.UDF4 = item.UDF4;
                        objDTO.UDF5 = item.UDF5;
                        objDTO.UDF6 = item.UDF6;
                        objDTO.UDF7 = item.UDF7;
                        objDTO.UDF8 = item.UDF8;
                        objDTO.UDF9 = item.UDF9;
                        objDTO.UDF10 = item.UDF10;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        if (item.ToolCategory.Trim() != "" && saveToolCat)
                        {
                            CurrentToolCategoryList.Add(objDTO);

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportToolCategoryColumn.UDF1.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportToolCategoryColumn.UDF2.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportToolCategoryColumn.UDF3.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportToolCategoryColumn.UDF4.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportToolCategoryColumn.UDF5.ToString(), RoomID);
                            //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportBinColumn.UDF6.ToString());

                        }
                        else
                            CurrentBlankToolCategoryList.Add(objDTO);
                    }

                    List<ToolCategoryMasterMain> lstreturn = new List<ToolCategoryMasterMain>();
                    if (CurrentToolCategoryList.Count > 0)
                    {
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.ToolCategoryMaster.ToString(), CurrentToolCategoryList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }
                    if (CurrentBlankToolCategoryList.Count > 0)
                    {
                        foreach (ToolCategoryMasterMain item in CurrentBlankToolCategoryList)
                        {
                            lstreturn.Add(item);
                        }
                    }
                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<ToolCategoryMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        
                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}
                }
            }

        }


        public void SaveShipViaMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
            List<ShipViaMasterMain> CurrentShipViaList,
            ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
            bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, string currentCulture, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            {

                List<ShipViaMasterMain> CurrentBlankShipViaList = new List<ShipViaMasterMain>();
                ShipViaMasterMain[] LstShipViaMaster = serializer.Deserialize<ShipViaMasterMain[]>(para);
                if (LstShipViaMaster != null && LstShipViaMaster.Length > 0)
                {
                    CurrentShipViaList = new List<ShipViaMasterMain>();
                    //lst = obj.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.ShipViaMaster.ToString(), UDFControlTypes.Textbox.ToString());
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.ShipViaMaster.ToString());

                    //ShipVias
                    eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("ShipVias", RoomID, SessionHelper.CompanyID);
                    CurrentOptionList = new List<UDFOptionsMain>();

                    foreach (ShipViaMasterMain item in LstShipViaMaster)
                    {
                        ShipViaMasterMain objDTO = new ShipViaMasterMain();
                        bool saveShip = true;

                        objDTO.ID = item.ID;
                        string errorMsg = string.Empty;
                        CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, out errorMsg, SessionHelper.EnterPriceID, SessionHelper.CompanyID, RoomID, currentCulture);
                        if (!string.IsNullOrWhiteSpace(errorMsg))
                        {
                            objDTO.Status = "Fail";
                            if (!string.IsNullOrEmpty(item.Reason))
                                objDTO.Reason += errorMsg;
                            else
                                objDTO.Reason = errorMsg;
                            saveShip = false;
                            item.Status = objDTO.Status;
                            item.Reason = objDTO.Reason;
                            objDTO = item;

                        }
                        objDTO.ShipVia = (item.ShipVia == null) ? null : (item.ShipVia.Length > 128 ? item.ShipVia.Substring(0, 128) : item.ShipVia);
                        objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);
                        objDTO.UDF6 = (item.UDF6 == null) ? null : (item.UDF6.Length > 255 ? item.UDF6.Substring(0, 255) : item.UDF6);
                        objDTO.UDF7 = (item.UDF7 == null) ? null : (item.UDF7.Length > 255 ? item.UDF7.Substring(0, 255) : item.UDF7);
                        objDTO.UDF8 = (item.UDF8 == null) ? null : (item.UDF8.Length > 255 ? item.UDF8.Substring(0, 255) : item.UDF8);
                        objDTO.UDF9 = (item.UDF9 == null) ? null : (item.UDF9.Length > 255 ? item.UDF9.Substring(0, 255) : item.UDF9);
                        objDTO.UDF10 = (item.UDF10 == null) ? null : (item.UDF10.Length > 255 ? item.UDF10.Substring(0, 255) : item.UDF10);
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";

                        string errors = ValidateModuleDTO<ShipViaMasterMain, ShipViaDTO>(objDTO, "Master", "ShipViaCreate"
                                                    , new List<string>() { "ID" }
                                                    );

                        if (string.IsNullOrWhiteSpace(errors) == false)
                        {
                            objDTO.Status = "Fail";
                            objDTO.Reason += errors;
                            item.Status = "Fail";
                            item.Reason += errors;
                        }

                        if (string.IsNullOrWhiteSpace(errors) && item.ShipVia.Trim() != "" && saveShip == true)
                        {
                            CurrentShipViaList.Add(objDTO);

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportShipViaColumn.UDF1.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportShipViaColumn.UDF2.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportShipViaColumn.UDF3.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportShipViaColumn.UDF4.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportShipViaColumn.UDF5.ToString(), RoomID);
                            // CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportShipViaColumn.UDF6.ToString());

                        }
                        else
                        {
                            CurrentBlankShipViaList.Add(objDTO);
                        }
                    }

                    List<ShipViaMasterMain> lstreturn = new List<ShipViaMasterMain>();
                    if (CurrentShipViaList.Count > 0)
                    {
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.ShipViaMaster.ToString(), CurrentShipViaList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }
                    if (CurrentBlankShipViaList.Count > 0)
                    {
                        foreach (ShipViaMasterMain item in CurrentBlankShipViaList)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<ShipViaMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        
                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}

                }

            }

        }

        /// <summary>
        /// ItemManufacturerDetails
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="CurrentOptionList"></param>
        /// <param name="CurrentItemManufacturerList"></param>
        /// <param name="TableName"></param>
        /// <param name="para"></param>
        /// <param name="HasMoreRecords"></param>
        /// <param name="IsFirstCall"></param>
        /// <param name="message"></param>
        /// <param name="status"></param>
        /// <param name="allSuccesfulRecords"></param>
        public void SaveItemManufacturer(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
            List<ItemManufacturer> CurrentItemManufacturerList,
            ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
            bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            {
                List<ItemManufacturer> CurrentBlankItemManufacturermain = new List<ItemManufacturer>();
                ItemManufacturer[] LstItemManufacturermain = serializer.Deserialize<ItemManufacturer[]>(para);
                if (LstItemManufacturermain != null && LstItemManufacturermain.Length > 0)
                {
                    CurrentBlankItemManufacturermain = new List<ItemManufacturer>();

                    ItemManufacturer objItemManufacturerDAL = new ItemManufacturer();
                    CurrentItemManufacturerList = new List<ItemManufacturer>();
                    List<ManufacturerMasterDTO> objManufacturerMasterDALList = new List<ManufacturerMasterDTO>();
                    ManufacturerMasterDAL objManufacturerMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                    objManufacturerMasterDALList = objManufacturerMasterDAL.GetManufacturerByRoomNormal(RoomID, SessionHelper.CompanyID, false, false, false, "ID DESC");
                    foreach (ItemManufacturer ManufacturerMasterList in LstItemManufacturermain.GroupBy(l => l.ManufacturerName).Select(g => g.First()).ToList())
                    {
                        if (ManufacturerMasterList.ManufacturerName.ToLower().Trim() != string.Empty)
                        {
                            if ((from p in objManufacturerMasterDALList
                                 where ((p.Manufacturer ?? string.Empty).ToLower().Trim() == (ManufacturerMasterList.ManufacturerName.ToLower().Trim()) && p.isForBOM == false && p.IsDeleted == false && p.Room == RoomID && p.CompanyID == SessionHelper.CompanyID)
                                 select p).Any())
                            {

                            }
                            else
                            {
                                ManufacturerMasterDTO objManufacturerMasterDTO = new ManufacturerMasterDTO();
                                objManufacturerMasterDTO.Manufacturer = ManufacturerMasterList.ManufacturerName;
                                objManufacturerMasterDTO.Room = RoomID;
                                objManufacturerMasterDTO.CompanyID = SessionHelper.CompanyID;
                                objManufacturerMasterDTO.CreatedBy = SessionHelper.UserID;
                                objManufacturerMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                objManufacturerMasterDTO.Created = DateTimeUtility.DateTimeNow;
                                objManufacturerMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                                objManufacturerMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objManufacturerMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objManufacturerMasterDTO.AddedFrom = "Web";
                                objManufacturerMasterDTO.EditedFrom = "Web";
                                objManufacturerMasterDTO.GUID = Guid.NewGuid();
                                objManufacturerMasterDTO.IsArchived = false;
                                objManufacturerMasterDTO.IsDeleted = false;
                                objManufacturerMasterDTO.isForBOM = false;
                                objManufacturerMasterDTO.RefBomId = null;
                                Int64 ManufacturerMasterID = objManufacturerMasterDAL.Insert(objManufacturerMasterDTO);
                                objManufacturerMasterDTO = objManufacturerMasterDAL.GetManufacturerByIDNormal(ManufacturerMasterID, RoomID, SessionHelper.CompanyID, false);
                                objManufacturerMasterDALList.Add(objManufacturerMasterDTO);

                            }
                        }
                    }
                    bool SaveList = true;
                    foreach (ItemManufacturer item in LstItemManufacturermain)
                    {
                        SaveList = true;
                        ItemManufacturer objDTO = new ItemManufacturer();
                        Guid? ItemGUID = GetGUID(ImportMastersDTO.TableName.ItemMaster, item.ItemNumber, RoomID: RoomID);

                        if ((objDTO.ManufacturerName != null || objDTO.ManufacturerName != string.Empty))
                        {
                            if (ItemGUID.HasValue && ItemGUID != Guid.Empty)
                            {
                                objDTO.ItemGUID = ItemGUID.Value;
                                objDTO.ManufacturerName = item.ManufacturerName;

                                ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                                if (objItemManufacturerDetailsDAL.CheckManufacturerNoDuplicate(item.ManufacturerNumber.Trim(), RoomID, SessionHelper.CompanyID, objDTO.ItemGUID))
                                {

                                    objDTO.ManufacturerNumber = item.ManufacturerNumber;
                                }
                                else
                                {
                                    SaveList = false;
                                    objDTO = item;
                                    objDTO.Status = "Fail";
                                    objDTO.Reason = item.ManufacturerNumber.Trim() + " " + ResItemMaster.MsgManufacturerNumberExist;
                                }
                                objDTO.IsDefault = item.IsDefault;
                                objDTO.ManufacturerID = objManufacturerMasterDALList.ToList().Where(m => m.Manufacturer == item.ManufacturerName && m.Room == RoomID && m.CompanyID == SessionHelper.CompanyID && m.IsDeleted == false && m.IsArchived == false && m.isForBOM == false).FirstOrDefault().ID;
                                objDTO.IsDeleted = false;
                                objDTO.IsArchived = false;
                                objDTO.Created = DateTimeUtility.DateTimeNow;
                                objDTO.Updated = DateTimeUtility.DateTimeNow;
                                objDTO.LastUpdatedBy = SessionHelper.UserID;
                                objDTO.Room = RoomID;
                                objDTO.CompanyID = SessionHelper.CompanyID;
                                objDTO.CreatedBy = SessionHelper.UserID;
                                objDTO.GUID = Guid.NewGuid();
                                objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objDTO.AddedFrom = "Web";
                                objDTO.EditedFrom = "Web";
                            }
                            if (ItemGUID.HasValue && ItemGUID != Guid.Empty && SaveList)
                            {
                                var itemval = CurrentItemManufacturerList.FirstOrDefault(x => x.ItemGUID == objDTO.ItemGUID && x.ManufacturerName == objDTO.ManufacturerName);
                                if (itemval != null)
                                {
                                    CurrentItemManufacturerList.Remove(itemval);
                                }
                                CurrentItemManufacturerList.Add(objDTO);
                            }
                            else
                            {
                                objDTO = item;
                                if (ItemGUID.HasValue && ItemGUID == Guid.Empty)
                                {
                                    objDTO.Status = "Fail";
                                    objDTO.Reason = ResItemMaster.MsgItemDoesNotExist;
                                }
                                CurrentBlankItemManufacturermain.Add(objDTO);
                            }
                        }
                        else
                        {
                            objDTO = item;
                            objDTO.Status = "Fail";

                            objDTO.Reason = ResItemMaster.MsgManufacturerNameRequired;


                            CurrentBlankItemManufacturermain.Add(objDTO);
                        }

                    }

                    List<ItemManufacturer> lstreturn = new List<ItemManufacturer>();
                    if (CurrentItemManufacturerList.Count > 0)
                    {
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.ItemManufacturerDetails.ToString(), CurrentItemManufacturerList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }
                    if (CurrentBlankItemManufacturermain.Count > 0)
                    {
                        foreach (ItemManufacturer item in CurrentBlankItemManufacturermain)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (isLastRoom && ErrorRows.Count == 0 && lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in LstItemManufacturermain)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }

                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                        {
                            SaveImportDataListSession<ItemManufacturer>(HasMoreRecords, IsFirstCall, LstItemManufacturermain.ToList());
                        }
                        
                        //Session["importedData"] = lstreturn;
                    }
                    if (CurrentItemManufacturerList.Count > 0)
                    {
                        eTurns.DAL.CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.InvalidateCache();
                        ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                        List<ItemManufacturerDetailsDTO> objItemManufacturerDetailsDTO = new List<ItemManufacturerDetailsDTO>();
                        objItemManufacturerDetailsDTO = objItemManufacturerDetailsDAL.GetItemManufacturerByRoomNormal(RoomID, SessionHelper.CompanyID);
                        foreach (ItemManufacturer objCurrentItemManu in CurrentItemManufacturerList.GroupBy(l => l.ItemGUID).Select(g => g.First()).ToList())
                        {
                            foreach (ItemManufacturerDetailsDTO objItemManufacturer in objItemManufacturerDetailsDTO.ToList().Where(l => l.IsDefault == true && l.ItemGUID == objCurrentItemManu.ItemGUID))
                            {
                                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                                if (objItemManufacturer.ItemGUID != Guid.Empty)
                                    objItemMasterDTO = objItemMasterDAL.GetRecordOnlyItemsFields(objItemManufacturer.ItemGUID.ToString(), RoomID, SessionHelper.CompanyID);

                                if (objItemMasterDTO != null)
                                {
                                    objItemMasterDTO.ManufacturerID = objItemManufacturer.ManufacturerID;
                                    objItemMasterDTO.ManufacturerNumber = objItemManufacturer.ManufacturerNumber;
                                    objItemMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                    objItemMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objItemMasterDAL.Edit(objItemMasterDTO, SessionUserId, SessionHelper.EnterPriceID);
                                }
                            }
                        }
                    }
                    if (ErrorRows.Count() != 0)
                    {
                        allSuccesfulRecords = false;
                    }

                    HttpContext.Current.Session["ItemManufacture"] = null;


                }

            }

        }

        public void SaveItemLocation(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
        List<InventoryLocationQuantityMain> CurrentInventoryLocationQuantityList,
        ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
        bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, string currentCulture,long RoomID,string RoomName, Dictionary<int, string> ErrorRows,bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            //if (ImportMastersDTO.TableName.ItemLocationeVMISetup.ToString() == TableName)
            {
                //SessionHelper.RoomID, SessionHelper.CompanyID
                List<InventoryLocationQuantityMain> CurrentBlankInventoryLocationQtyList = new List<InventoryLocationQuantityMain>();
                InventoryLocationQuantityMain[] LstInventoryLocation = serializer.Deserialize<InventoryLocationQuantityMain[]>(para);
                if (LstInventoryLocation != null && LstInventoryLocation.Length > 0)
                {
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.BinUDF.ToString());

                    eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("BinUDF", RoomID, SessionHelper.CompanyID);
                    CurrentOptionList = new List<UDFOptionsMain>();

                    List<BinMasterDTO> lstBinMaster = objItemMasterDAL.GetItemLocationsByItemNumberList(LstInventoryLocation.Select(x => x.ItemNumber).Distinct().ToList(), RoomID, SessionHelper.CompanyID);

                    List<BinForDefaultCheckDTO> lstCompleteBinMaster2 = (from I3 in LstInventoryLocation //.Where(y => !lstBinMaster.Select(x => x.ItemNumber.Trim().ToUpper() + "_" + x.BinNumber.Trim().ToUpper()).Contains(y.ItemNumber.Trim().ToUpper() + "_" + y.BinNumber.Trim().ToUpper()))

                                                                         join _I1 in lstBinMaster on new { ItemNumber = I3.ItemNumber.Trim().ToUpper(), BinNumber = I3.BinNumber.Trim().ToUpper() } equals new { ItemNumber = _I1.ItemNumber.Trim().ToUpper(), BinNumber = _I1.BinNumber.Trim().ToUpper() } into _I11
                                                                         from I1 in _I11.DefaultIfEmpty()

                                                                         where I1 == null
                                                                         select new BinForDefaultCheckDTO
                                                                         {
                                                                             ItemNumber = I3.ItemNumber,
                                                                             BinNumber = I3.BinNumber,
                                                                             IsDeleted = (I3.IsDeleted == null ? false : I3.IsDeleted.Value),
                                                                             IsDefault = (I3.IsDefault == null ? false : I3.IsDefault.Value),
                                                                             IsStagingLocation = (I3.IsStagingLocation == null ? false : I3.IsStagingLocation.Value)
                                                                         }).OrderBy(x => x.ItemNumber).GroupBy(x => new { x.ItemNumber, x.BinNumber, x.IsDeleted, x.IsDefault, x.IsStagingLocation }).Select(x => new BinForDefaultCheckDTO
                                                                         {
                                                                             ItemNumber = x.Key.ItemNumber,
                                                                             BinNumber = x.Key.BinNumber,
                                                                             IsDeleted = x.Key.IsDeleted,
                                                                             IsDefault = x.Key.IsDefault,
                                                                             IsStagingLocation = x.Key.IsStagingLocation
                                                                         }).ToList();

                    var lstNewDefaultBins = (from I in lstCompleteBinMaster2
                                             where I.IsDeleted == false && I.IsDefault == true
                                             group I.BinNumber by I.ItemNumber into g
                                             select new { ItemNumber = g.Key, Count = g.Count() }).ToList();

                    List<BinForDefaultCheckDTO> lstCompleteBinMaster1 = (from I1 in lstBinMaster

                                                                         join I2 in LstInventoryLocation
                                                                         on new { ItemNumber = I1.ItemNumber.Trim().ToUpper(), BinNumber = I1.BinNumber.Trim().ToUpper() } equals new { ItemNumber = I2.ItemNumber.Trim().ToUpper(), BinNumber = I2.BinNumber.Trim().ToUpper() }

                                                                         //join I3 in lstNewDefaultBins
                                                                         //on new { ItemNumber = I1.ItemNumber.Trim().ToUpper() } equals new { ItemNumber = I3.ItemNumber.Trim().ToUpper() }

                                                                         select new BinForDefaultCheckDTO
                                                                         {
                                                                             ItemNumber = I1.ItemNumber,
                                                                             BinNumber = I1.BinNumber,
                                                                             IsDeleted = (I2 != null ? (I2.IsDeleted == null ? false : I2.IsDeleted.Value) : (I1.IsDeleted == null ? false : I1.IsDeleted.Value)),
                                                                             IsDefault = (I2 != null ? (I2.IsDefault == null ? false : I2.IsDefault.Value) : (I1.IsDefault == null ? false : I1.IsDefault.Value)),
                                                                             IsStagingLocation = (I2 != null ? (I2.IsStagingLocation == null ? false : I2.IsStagingLocation.Value) : I1.IsStagingLocation)
                                                                             //IsDefault = (I2 != null ? (I2.IsDefault == null ? false : I2.IsDefault.Value) :
                                                                             //                          (I3 != null ? false : (I1.IsDefault == null ? false : I1.IsDefault.Value)))
                                                                         }).OrderBy(x => x.ItemNumber).GroupBy(x => new { x.ItemNumber, x.BinNumber, x.IsDeleted, x.IsDefault, x.IsStagingLocation }).Select(x => new BinForDefaultCheckDTO
                                                                         {
                                                                             ItemNumber = x.Key.ItemNumber,
                                                                             BinNumber = x.Key.BinNumber,
                                                                             IsDeleted = x.Key.IsDeleted,
                                                                             IsDefault = x.Key.IsDefault,
                                                                             IsStagingLocation = x.Key.IsStagingLocation
                                                                         }).ToList();

                    List<BinForDefaultCheckDTO> lstCompleteBinMaster3 = (from I4 in lstBinMaster.Where(y => !LstInventoryLocation.Select(x => x.ItemNumber.Trim().ToUpper() + "_" + x.BinNumber.Trim().ToUpper()).Contains(y.ItemNumber.Trim().ToUpper() + "_" + y.BinNumber.Trim().ToUpper()))

                                                                         join _I3 in lstNewDefaultBins on new { ItemNumber = I4.ItemNumber.Trim().ToUpper() } equals new { ItemNumber = _I3.ItemNumber.Trim().ToUpper() } into _I31
                                                                         from I3 in _I31.DefaultIfEmpty()

                                                                         select new BinForDefaultCheckDTO
                                                                         {
                                                                             ItemNumber = I4.ItemNumber,
                                                                             BinNumber = I4.BinNumber,
                                                                             IsDeleted = (I4.IsDeleted == null ? false : I4.IsDeleted.Value),
                                                                             //IsDefault = (I4.IsDefault == null ? false : I4.IsDefault.Value)
                                                                             IsDefault = (I3 != null ? false : (I4.IsDefault == null ? false : I4.IsDefault.Value)),
                                                                             IsStagingLocation = I4.IsStagingLocation
                                                                         }).OrderBy(x => x.ItemNumber).GroupBy(x => new { x.ItemNumber, x.BinNumber, x.IsDeleted, x.IsDefault, x.IsStagingLocation }).Select(x => new BinForDefaultCheckDTO
                                                                         {
                                                                             ItemNumber = x.Key.ItemNumber,
                                                                             BinNumber = x.Key.BinNumber,
                                                                             IsDeleted = x.Key.IsDeleted,
                                                                             IsDefault = x.Key.IsDefault,
                                                                             IsStagingLocation = x.Key.IsStagingLocation
                                                                         }).ToList();

                    List<BinForDefaultCheckDTO> lstCompleteBinMaster = lstCompleteBinMaster1.Union(lstCompleteBinMaster2).Union(lstCompleteBinMaster3).OrderBy(x => x.ItemNumber).ToList();

                    var lstValidItemNumbers = (from I in lstCompleteBinMaster
                                               where I.IsDeleted == false && I.IsDefault == true && I.IsStagingLocation == false
                                               group I.BinNumber by I.ItemNumber into g
                                               select new { ItemNumber = g.Key, Count = g.Count() }).ToList();

                    CurrentInventoryLocationQuantityList = new List<InventoryLocationQuantityMain>();
                    bool saveData = true;
                    foreach (InventoryLocationQuantityMain item in LstInventoryLocation)
                    {
                        saveData = true;
                        InventoryLocationQuantityMain objDTO = new InventoryLocationQuantityMain();
                        objDTO.ID = item.ID;
                        objDTO.ItemNumber = item.ItemNumber;
                        objDTO.ItemGUID = item.ItemNumber == "" ? Guid.NewGuid() : GetGUID(ImportMastersDTO.TableName.ItemMaster, item.ItemNumber, RoomID : RoomID);
                        objDTO.BinNumber = item.BinNumber;
                        objDTO.CriticalQuantity = item.CriticalQuantity;
                        objDTO.MinimumQuantity = item.MinimumQuantity;
                        objDTO.MaximumQuantity = item.MaximumQuantity;
                        objDTO.IsDefault = item.IsDefault;
                        objDTO.IsStagingLocation = item.IsStagingLocation;

                        objDTO.IsEnforceDefaultPullQuantity = item.IsEnforceDefaultPullQuantity;
                        objDTO.IsEnforceDefaultReorderQuantity = item.IsEnforceDefaultReorderQuantity;
                        objDTO.DefaultPullQuantity = item.DefaultPullQuantity;
                        objDTO.DefaultReorderQuantity = item.DefaultReorderQuantity;

                        string errorMsg = string.Empty;
                        CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, item.BinUDF1, item.BinUDF2, item.BinUDF3, item.BinUDF4, item.BinUDF5, out errorMsg, SessionHelper.EnterPriceID, SessionHelper.CompanyID, RoomID, currentCulture);
                        if (!string.IsNullOrWhiteSpace(errorMsg))
                        {
                            objDTO.Status = "UDF_REQUIRED";
                            if (!string.IsNullOrEmpty(objDTO.Reason))
                                objDTO.Reason += errorMsg;
                            else
                                objDTO.Reason = errorMsg;
                            saveData = false;
                            item.Status = objDTO.Status;
                            item.Reason = errorMsg;
                        }

                        objDTO.BinUDF1 = item.BinUDF1;
                        objDTO.BinUDF2 = item.BinUDF2;
                        objDTO.BinUDF3 = item.BinUDF3;
                        objDTO.BinUDF4 = item.BinUDF4;
                        objDTO.BinUDF5 = item.BinUDF5;

                        objDTO.SensorId = item.SensorId;
                        objDTO.SensorPort = item.SensorPort;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        if (objDTO.IsStagingLocation == true)
                        {
                            objDTO.IsDefault = false;
                        }

                        var ValidItemNumber = lstValidItemNumbers.Where(x => x.ItemNumber == item.ItemNumber).FirstOrDefault();

                        if (ValidItemNumber == null)
                        {
                            ValidItemNumber = (from I in lstBinMaster
                                               where I.IsDeleted == false && I.IsDefault == true && I.IsStagingLocation == false
                                               group I.BinNumber by I.ItemNumber into g
                                               select new { ItemNumber = g.Key, Count = g.Count() }).FirstOrDefault();

                            if (ValidItemNumber != null && lstNewDefaultBins.Count == 0 && lstValidItemNumbers.Count == 0 && objDTO.IsStagingLocation == false)
                            {
                                BinMasterDTO objBinDto = lstBinMaster.Where(x => x.BinNumber == objDTO.BinNumber && x.ItemNumber == objDTO.ItemNumber &&
                                                         x.IsDeleted == false && x.IsDefault == true && x.IsStagingLocation == false).FirstOrDefault();

                                if (objBinDto != null && objDTO.IsDefault == false)
                                {
                                    objDTO.IsDefault = true;
                                }
                            }
                        }

                        if (ValidItemNumber == null)
                        {
                            objDTO.Status = "Fail";
                            objDTO.Reason = "ONE_DEFAULT_NEEDED";
                            saveData = false;
                        }
                        else if (ValidItemNumber.Count != 1)
                        {
                            objDTO.Status = "Fail";
                            objDTO.Reason = "MOTE_THEN_ONE_DEFAULT";
                            saveData = false;
                        }

                        if (!(item.IsDeleted ?? false) && saveData)
                        {
                            if (CheckBinStatus(item.BinNumber,RoomID))
                            {
                                objDTO.IsDeleted = item.IsDeleted;
                            }
                            else
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = ResItemMaster.MsgLocationDeleted;
                                saveData = false;
                            }
                        }
                        else if (saveData)
                        {
                            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                            //BinMasterDTO objBin = objBinMasterDAL.GetRecordByItemGuid(item.BinNumber, objDTO.ItemGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                            BinMasterDTO objBin = objBinMasterDAL.GetBinByLocationNameItemGuid(RoomID, SessionHelper.CompanyID, false, false, item.BinNumber, objDTO.ItemGUID ?? Guid.Empty).FirstOrDefault();
                            if (objBin != null)
                            {
                                List<BinMasterDTO> objBinMasterList = objBinMasterDAL.CheckBinInUse_New(RoomID, SessionHelper.CompanyID, objBin.ID, objDTO.ItemGUID ?? Guid.Empty);
                                foreach (var objBinMaster in objBinMasterList)
                                {
                                    if (((objBinMaster.MinimumQuantity ?? 0) + (objBinMaster.MaximumQuantity ?? 0) + (objBinMaster.CriticalQuantity ?? 0) + (objBinMaster.SuggestedOrderQuantity ?? 0) + (objBinMaster.CountQuantity ?? 0) + (objBinMaster.KitMoveInOutQuantity ?? 0)) != 0)
                                    {
                                        string ErrorMessage = "";
                                        if ((objBinMaster.MinimumQuantity ?? 0) > 0)
                                        {
                                            status = "referencecount";
                                            if (string.IsNullOrWhiteSpace(ErrorMessage))
                                            {
                                                ErrorMessage = eTurns.DTO.ResBin.RequisitionedQuantity;
                                            }
                                            else
                                            {
                                                ErrorMessage += ", " + eTurns.DTO.ResBin.RequisitionedQuantity;
                                            }
                                        }
                                        if ((objBinMaster.MaximumQuantity ?? 0) > 0)
                                        {
                                            status = "referencecount";
                                            if (string.IsNullOrWhiteSpace(ErrorMessage))
                                            {
                                                ErrorMessage = ResItemMaster.RequestedTransferQuantity;
                                            }
                                            else
                                            {
                                                ErrorMessage += ", " + ResItemMaster.RequestedTransferQuantity;
                                            }
                                        }
                                        if ((objBinMaster.CriticalQuantity ?? 0) > 0)
                                        {
                                            status = "referencecount";
                                            if (string.IsNullOrWhiteSpace(ErrorMessage))
                                            {
                                                ErrorMessage = eTurns.DTO.ResBin.RequestedQuantity;
                                            }
                                            else
                                            {
                                                ErrorMessage += ", " + eTurns.DTO.ResBin.RequestedQuantity;
                                            }

                                        }
                                        if ((objBinMaster.SuggestedOrderQuantity ?? 0) > 0)
                                        {
                                            status = "referencecount";
                                            if (string.IsNullOrWhiteSpace(ErrorMessage))
                                            {
                                                ErrorMessage = ResItemMaster.SuggestedOrderQuantity;
                                            }
                                            else
                                            {
                                                ErrorMessage += ", " + ResItemMaster.SuggestedOrderQuantity;
                                            }
                                        }
                                        if ((objBinMaster.CountQuantity ?? 0) > 0)
                                        {
                                            status = "referencecount";
                                            if (string.IsNullOrWhiteSpace(ErrorMessage))
                                            {
                                                ErrorMessage = ResItemMaster.CountQuantity;
                                            }
                                            else
                                            {
                                                ErrorMessage += ", " + ResItemMaster.CountQuantity;
                                            }
                                        }
                                        if ((objBinMaster.KitMoveInOutQuantity ?? 0) > 0)
                                        {
                                            status = "referencecount";
                                            if (string.IsNullOrWhiteSpace(ErrorMessage))
                                            {
                                                ErrorMessage = ResItemMaster.KitMoveInOutQuantity;
                                            }
                                            else
                                            {
                                                ErrorMessage += ", " + ResItemMaster.KitMoveInOutQuantity;
                                            }
                                        }

                                        objDTO.Status = "ITEM_LOCATION_IN_USE";
                                        objDTO.Reason = ErrorMessage;
                                        saveData = false;
                                    }
                                    else
                                    {

                                        objDTO.IsDeleted = item.IsDeleted;
                                        if (objDTO.IsDeleted == true)
                                        {
                                            objDTO.IsDefault = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                            //if (ValidateToDeleteLocation(item.BinNumber, objDTO.ItemGUID ?? Guid.Empty))
                            //{
                            //    objDTO.IsDeleted = item.IsDeleted;
                            //}
                            //else
                            //{
                            //    objDTO.Status = "Fail";
                            //    objDTO.Reason = "Location Already in use.";
                            //    saveData = false;
                            //}
                        }

                        if (objDTO.ItemGUID.ToString() != Guid.Empty.ToString() && item.BinNumber.Trim() != "" && saveData)
                        {
                            bool Flag = false;
                            var itemval = CurrentInventoryLocationQuantityList.FirstOrDefault(x => x.ItemGUID == objDTO.ItemGUID && x.BinNumber == objDTO.BinNumber && x.IsStagingLocation == objDTO.IsStagingLocation);
                            if (itemval != null)
                            {
                                if (itemval.IsDefault == false)
                                    CurrentInventoryLocationQuantityList.Remove(itemval);
                                else
                                    Flag = true;
                            }

                            if (Flag == false)
                                CurrentInventoryLocationQuantityList.Add(objDTO);

                            CheckUDF(lst, CurrentOptionList, objDTO.BinUDF1 == null ? "" : objDTO.BinUDF1, CommonUtility.ImportLocationColumn.UDF1.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.BinUDF2 == null ? "" : objDTO.BinUDF2, CommonUtility.ImportLocationColumn.UDF2.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.BinUDF3 == null ? "" : objDTO.BinUDF3, CommonUtility.ImportLocationColumn.UDF3.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.BinUDF4 == null ? "" : objDTO.BinUDF4, CommonUtility.ImportLocationColumn.UDF4.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.BinUDF5 == null ? "" : objDTO.BinUDF5, CommonUtility.ImportLocationColumn.UDF5.ToString(), RoomID);


                        }
                        else
                        {
                            CurrentBlankInventoryLocationQtyList.Add(objDTO);
                        }
                    }

                    List<InventoryLocationQuantityMain> lstreturn = new List<InventoryLocationQuantityMain>();
                    if (CurrentInventoryLocationQuantityList.Count > 0)
                    {
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.ItemLocationeVMISetup.ToString(), CurrentInventoryLocationQuantityList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }

                    List<Guid> lstItemGuidsLoc = new List<Guid>();
                    if (lstreturn != null && lstreturn.Count > 0)
                    {
                        List<Guid> lstItemGuids = lstreturn.Where(x => (x.Status ?? string.Empty).ToLower() != "fail" && x.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).Select(b => b.ItemGUID.GetValueOrDefault(Guid.Empty)).Distinct().ToList();
                        lstItemGuidsLoc = lstItemGuids;
                        if (lstItemGuids != null && lstItemGuids.Count > 0)
                        {
                            QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(SessionHelper.EnterPriseDBName);
                            foreach (Guid itemGUID in lstItemGuids)
                            {
                                CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
                                objCartItemDAL.AutoCartUpdateByCode(itemGUID, SessionHelper.UserID, "Web", "BulkImport >> ItemLocation", SessionUserId);
                                //objQBItemDAL.InsertQuickBookItem(itemGUID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, "Update", false, SessionHelper.UserID, "Web", null, "BulkImport >> ItemLocation");
                            }

                        }
                    }

                    if (lstItemGuidsLoc != null && lstItemGuidsLoc.Count() > 0)
                    {
                        string strItemGUIDs = string.Join(",", lstItemGuidsLoc);
                        BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                        objBinDAL.UpdateDefaultBin(strItemGUIDs, SessionHelper.UserID);
                    }

                    if (CurrentBlankInventoryLocationQtyList.Count > 0)
                    {
                        foreach (InventoryLocationQuantityMain item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status == null))
                        {
                            item.Status = "Fail";
                            item.Reason = ResItemMaster.MsgItemDoesNotExist;
                            lstreturn.Add(item);
                        }
                        foreach (InventoryLocationQuantityMain item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status.ToLower() == "fail" && l.Reason.IndexOf("use") >= 0))
                        {
                            item.Status = "Fail";
                            item.Reason = ResItemMaster.MsgLocationInUse;
                            lstreturn.Add(item);
                        }
                        foreach (InventoryLocationQuantityMain item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status.ToLower() == "fail" && l.Reason.IndexOf("deleted") >= 0))
                        {
                            item.Status = "Fail";
                            item.Reason = ResItemMaster.MsgLocationDeleted;
                            lstreturn.Add(item);
                        }
                        foreach (InventoryLocationQuantityMain item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status.ToLower() == "fail" && l.Reason == "ONE_DEFAULT_NEEDED"))
                        {
                            item.Status = "Fail";
                            item.Reason = ResItemMaster.MsgItemMustHaveOneLocation;
                            lstreturn.Add(item);
                        }
                        foreach (InventoryLocationQuantityMain item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status.ToLower() == "fail" && l.Reason == "MOTE_THEN_ONE_DEFAULT"))
                        {
                            item.Status = "Fail";
                            item.Reason = ResItemMaster.MsgMultipleDefaultLocation;
                            lstreturn.Add(item);
                        }
                        foreach (InventoryLocationQuantityMain item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status.ToLower() == "fail" && l.Reason == "MOTE_THEN_ONE_DEFAULT_DELETED"))
                        {
                            item.Status = "Fail";
                            item.Reason = ResItemMaster.MsgOneLocationMustBe;
                            lstreturn.Add(item);
                        }
                        foreach (InventoryLocationQuantityMain item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status.ToLower() == "item_location_in_use"))
                        {
                            item.Status = "Fail";
                            lstreturn.Add(item);
                        }

                        foreach (InventoryLocationQuantityMain item in CurrentBlankInventoryLocationQtyList.Where(l => l.Status.ToLower() == "udf_required"))
                        {
                            item.Status = "Fail";
                            lstreturn.Add(item);
                        }
                    }

                    //if (lstreturn.Count == 0)
                    //{
                    //    message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                    //    status = ResMessage.SaveMessage;
                    //    ClearCurrentResourceList();
                    //    Session["importedData"] = null;
                    //}
                    //else
                    //{
                    message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                    status = ResMessage.SaveMessage;

 
                    #region track errors if any - Row Wise For Every Room
                    int RowNumber = 0;
                    foreach (var item in lstreturn)
                    {
                        RowNumber++;
                        if (item.Status != "" && item.Status.ToLower() == "fail")
                        {
                            if (ErrorRows.ContainsKey(RowNumber))
                            {
                                item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                ErrorRows[RowNumber] = item.Reason;
                            }
                            else
                            {
                                ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                            }
                        }
                    }
                    #endregion

                    if (isLastRoom && ErrorRows.Count() > 0)
                    {
                        int SrNo = 0;
                        foreach (var item in lstreturn)
                        {
                            SrNo++;
                            if (ErrorRows.ContainsKey(SrNo))
                            {
                                item.Reason = ErrorRows[SrNo];
                                item.Status = "fail";
                            }
                        }
                        
                        allSuccesfulRecords = false;
                    }
                    if (isLastRoom)
                    SaveImportDataListSession<InventoryLocationQuantityMain>(HasMoreRecords, IsFirstCall, lstreturn);
                    //Session["importedData"] = lstreturn;

                    //Session["importedData"] = lstreturn;

                    //}
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}
                    //CacheHelper<IEnumerable<inventory>>.InvalidateCache();
                    //(new CategoryMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);

                }

            }
        }

        public void SaveItemLocationChange(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
            List<ItemLocationChangeImport> CurrentLocationChangeList,
            ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
            bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            {
                List<ItemMasterBinDTO> objItemMasterBinDTOList = new List<ItemMasterBinDTO>();
                List<ItemLocationChangeImport> CurrentBlankItemLocationChangeImportList = new List<ItemLocationChangeImport>();
                ItemLocationChangeImport[] LstItemLocationChangeMain = serializer.Deserialize<ItemLocationChangeImport[]>(para);
                if (LstItemLocationChangeMain != null && LstItemLocationChangeMain.Length > 0)
                {
                    CurrentBlankItemLocationChangeImportList = new List<ItemLocationChangeImport>();
                    CurrentLocationChangeList = new List<ItemLocationChangeImport>();
                    //bool IsEmailPOInBody = false; bool IsEmailPOInPDF = false; bool IsEmailPOInCSV = false; bool IsEmailPOInX12 = false;
                    foreach (ItemLocationChangeImport item in LstItemLocationChangeMain)
                    {
                        ItemLocationChangeImport objDTO = new ItemLocationChangeImport();

                        Guid? ItemGUID = GetGUID(ImportMastersDTO.TableName.ItemMaster, item.ItemNumber,RoomID: RoomID); //(ImportMastersDTO.TableName.SupplierMaster, item.SupplierName);
                        if (ItemGUID != null && ItemGUID != Guid.Empty)
                        {
                            BinMasterDAL objItemLocationLevelQuanityDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

                            BinMasterDTO objBinMasterDTO = objItemLocationLevelQuanityDAL.GetItemLocationsWithBinName((ItemGUID ?? Guid.Empty), RoomID, SessionHelper.CompanyID, item.OldLocationName);
                            //BinMasterDTO objBinMasterDTO = null;
                            //if (lstBinReplanish.Where(c => c.BinNumber == item.OldLocationName).Any())
                            //{
                            //    objBinMasterDTO = lstBinReplanish.Where(c => c.BinNumber == item.OldLocationName).FirstOrDefault();
                            //}
                            if (objBinMasterDTO == null)
                            {
                                objDTO = item;
                                //if (ItemGUID.HasValue && ItemGUID == Guid.Empty)
                                {
                                    objDTO.Status = "Fail";
                                    objDTO.Reason = string.Format(ResItemMaster.MsgBinNotExistForItem, item.OldLocationName, item.ItemNumber);
                                }
                                CurrentBlankItemLocationChangeImportList.Add(objDTO);
                            }
                            else
                            {
                                ItemMasterBinDAL objItemMasterBinDAL = new ItemMasterBinDAL(SessionHelper.EnterPriseDBName);
                                // ItemMasterBinDTO objItemMasterBinDTO = objItemMasterBinDAL.GetBinItemUsingGuid((objBinMasterDTO.GUID), RoomID, SessionHelper.CompanyID, false, false).ToList().FirstOrDefault();
                                bool Isdefault = objItemMasterBinDAL.GetBinItemIsDefault((objBinMasterDTO.GUID), RoomID, SessionHelper.CompanyID, false, false);
                                objDTO.ID = item.ID;
                                objDTO.ItemNumber = item.ItemNumber;
                                objDTO.OldLocationName = item.OldLocationName;
                                objDTO.NewLocationName = (item.NewLocationName);
                                objDTO.ItemGuid = ItemGUID;
                                objDTO.IsDefault = Isdefault;
                                objDTO.Room = RoomID;
                                objDTO.CompanyID = SessionHelper.CompanyID;
                                objDTO.LastUpdatedBy = SessionHelper.UserID;
                                objDTO.OldLocation = objBinMasterDTO.ID;
                                if ((!string.IsNullOrWhiteSpace(item.ItemNumber)) && (!string.IsNullOrWhiteSpace(item.OldLocationName)) && (!string.IsNullOrWhiteSpace(item.NewLocationName)))
                                {
                                    ItemLocationChangeImport itemval = null;

                                    if (item.ItemNumber != null && (!string.IsNullOrWhiteSpace(item.ItemNumber)) && item.OldLocationName != null && (!string.IsNullOrWhiteSpace(item.OldLocationName)))
                                    {
                                        itemval = CurrentLocationChangeList.FirstOrDefault(x => x.ItemNumber == item.ItemNumber && x.OldLocationName == item.OldLocationName);
                                    }

                                    if (itemval != null)
                                    {
                                        CurrentLocationChangeList.Remove(itemval);
                                    }
                                    CurrentLocationChangeList.Add(objDTO);

                                    //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportSupplierColumn.UDF6.ToString());

                                }
                                else
                                {
                                    CurrentBlankItemLocationChangeImportList.Add(objDTO);
                                }
                            }
                        }
                        else
                        {
                            objDTO = item;
                            if (ItemGUID.HasValue && ItemGUID == Guid.Empty)
                            {
                                objDTO.Status = "Fail";
                                objDTO.Reason = item.ItemNumber + " " + ResItemMaster.MsgItemDoesNotExist;
                            }
                            CurrentBlankItemLocationChangeImportList.Add(objDTO);
                        }


                    }

                    List<ItemLocationChangeImport> lstreturn = new List<ItemLocationChangeImport>();
                    if (CurrentLocationChangeList.Count > 0)
                    {
                        foreach (ItemLocationChangeImport i in CurrentLocationChangeList)
                        {

                            string Message = string.Empty;
                            if (i.NewLocationName != i.OldLocationName)
                            {
                                ItemMasterBinDAL objItemMasterBinDAL = new ItemMasterBinDAL(SessionHelper.EnterPriseDBName);
                                Message = objItemMasterBinDAL.SaveItemMasterBin(i.ItemGuid ?? Guid.Empty, i.OldLocation ?? 0, i.NewLocationName, SessionHelper.UserID, RoomID, SessionHelper.CompanyID, i.IsDefault ?? false);
                                if (Message != "Success")
                                {
                                    i.Status = "Fail";
                                    i.Reason = Message;
                                    lstreturn.Add(i);
                                }
                                else
                                {
                                    i.Status = "success";
                                    i.Reason = "";
                                }
                            }

                        }

                    }
                    //lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.SupplierMaster.ToString(), CurrentSupplierList, RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                    //lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.ItemLocationChange.ToString(), CurrentLocationChangeList, RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                    if (CurrentBlankItemLocationChangeImportList.Count > 0)
                    {
                        foreach (ItemLocationChangeImport item in CurrentBlankItemLocationChangeImportList)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;

                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<ItemLocationChangeImport>(HasMoreRecords, IsFirstCall, lstreturn);

                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}

                }

            }

        }

        public void SaveInventoryClassificationMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
            List<InventoryClassificationMasterMain> CurrentInventoryClassificationList,
            ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
            bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, string currentCulture, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            {
                List<InventoryClassificationMasterMain> CurrentBlankCategoryList = new List<InventoryClassificationMasterMain>();
                InventoryClassificationMasterMain[] LstCategoryMaster = serializer.Deserialize<InventoryClassificationMasterMain[]>(para);
                if (LstCategoryMaster != null && LstCategoryMaster.Length > 0)
                {
                    CurrentInventoryClassificationList = new List<InventoryClassificationMasterMain>();
                    //lst = obj.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.InventoryClassificationMaster.ToString(), UDFControlTypes.Textbox.ToString());
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.InventoryClassificationMaster.ToString());


                    eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("InventoryClassification", RoomID, SessionHelper.CompanyID);

                    CurrentOptionList = new List<UDFOptionsMain>();

                    foreach (InventoryClassificationMasterMain item in LstCategoryMaster)
                    {
                        InventoryClassificationMasterMain objDTO = new InventoryClassificationMasterMain();
                        objDTO.ID = item.ID;
                        objDTO.BaseOfInventory = item.BaseOfInventory;
                        objDTO.InventoryClassification = item.InventoryClassification;
                        objDTO.RangeStart = item.RangeStart;
                        objDTO.RangeEnd = item.RangeEnd;

                        bool saveInvClas = true;
                        string errorMsg = string.Empty;
                        CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, out errorMsg, SessionHelper.EnterPriceID, SessionHelper.CompanyID, RoomID, currentCulture);
                        if (!string.IsNullOrWhiteSpace(errorMsg))
                        {
                            objDTO.Status = "Fail";
                            if (!string.IsNullOrEmpty(objDTO.Reason))
                                objDTO.Reason += errorMsg;
                            else
                                objDTO.Reason = errorMsg;
                            saveInvClas = false;
                            item.Status = objDTO.Status;
                            item.Reason = errorMsg;
                        }

                        objDTO.UDF1 = item.UDF1;
                        objDTO.UDF2 = item.UDF2;
                        objDTO.UDF3 = item.UDF3;
                        objDTO.UDF4 = item.UDF4;
                        objDTO.UDF5 = item.UDF5;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";

                        string errors = ValidateModuleDTO<InventoryClassificationMasterMain, InventoryClassificationMasterDTO>(objDTO, "Master", "InventoryClassificationCreate"
                                                    , new List<string>() { "ID" }
                                                    );

                        if (string.IsNullOrWhiteSpace(errors) == false)
                        {
                            objDTO.Status = "Fail";
                            objDTO.Reason += errors;
                            item.Status = "Fail";
                            item.Reason += errors;
                        }

                        if (string.IsNullOrWhiteSpace(errors) && item.InventoryClassification.Trim() != "" && saveInvClas)
                        {
                            CurrentInventoryClassificationList.Add(objDTO);

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportInventoryClassificationColumn.UDF1.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportInventoryClassificationColumn.UDF2.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportInventoryClassificationColumn.UDF3.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportInventoryClassificationColumn.UDF4.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportInventoryClassificationColumn.UDF5.ToString(), RoomID);
                            //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportCategoryColumn.UDF6.ToString());
                        }
                        else
                        {
                            CurrentBlankCategoryList.Add(objDTO);
                        }
                    }

                    List<InventoryClassificationMasterMain> lstreturn = new List<InventoryClassificationMasterMain>();
                    if (CurrentInventoryClassificationList.Count > 0)
                    {
                        //GetValidateList(CurrentInventoryClassificationList, TableName);
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.InventoryClassificationMaster.ToString(), CurrentInventoryClassificationList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }
                    if (CurrentBlankCategoryList.Count > 0)
                    {
                        foreach (InventoryClassificationMasterMain item in CurrentBlankCategoryList)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<InventoryClassificationMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}

                }

            }
        }

        public void SaveGLAccountMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
            List<GLAccountMasterMain> CurrentGLAccountList,
            ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
            bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            {

                List<GLAccountMasterMain> CurrentBlankGLAccountList = new List<GLAccountMasterMain>();
                GLAccountMasterMain[] LstGLAccountMaster = serializer.Deserialize<GLAccountMasterMain[]>(para);
                if (LstGLAccountMaster != null && LstGLAccountMaster.Length > 0)
                {
                    CurrentGLAccountList = new List<GLAccountMasterMain>();
                    //lst = obj.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.GLAccountMaster.ToString(), UDFControlTypes.Textbox.ToString());
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.GLAccountMaster.ToString());

                    CurrentOptionList = new List<UDFOptionsMain>();

                    foreach (GLAccountMasterMain item in LstGLAccountMaster)
                    {
                        GLAccountMasterMain objDTO = new GLAccountMasterMain();
                        objDTO.ID = item.ID;
                        objDTO.GLAccount = (item.GLAccount == null) ? null : (item.GLAccount.Length > 128 ? item.GLAccount.Substring(0, 128) : item.GLAccount);
                        objDTO.Description = (item.Description == null) ? null : (item.Description.Length > 1024 ? item.Description.Substring(0, 1024) : item.Description);
                        objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);
                        objDTO.UDF6 = (item.UDF6 == null) ? null : (item.UDF6.Length > 255 ? item.UDF6.Substring(0, 255) : item.UDF6);
                        objDTO.UDF7 = (item.UDF7 == null) ? null : (item.UDF7.Length > 255 ? item.UDF7.Substring(0, 255) : item.UDF7);
                        objDTO.UDF8 = (item.UDF8 == null) ? null : (item.UDF8.Length > 255 ? item.UDF8.Substring(0, 255) : item.UDF8);
                        objDTO.UDF9 = (item.UDF9 == null) ? null : (item.UDF9.Length > 255 ? item.UDF9.Substring(0, 255) : item.UDF9);
                        objDTO.UDF10 = (item.UDF10 == null) ? null : (item.UDF10.Length > 255 ? item.UDF10.Substring(0, 255) : item.UDF10);
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();

                        string errors = ValidateModuleDTO<GLAccountMasterMain, GLAccountMasterDTO>(objDTO, "Master", "GLAccountCreate"
                                                    , new List<string>() { "ID" }
                                                    );

                        if (string.IsNullOrWhiteSpace(errors) == false)
                        {
                            objDTO.Status = "Fail";
                            objDTO.Reason += errors;
                            item.Status = "Fail";
                            item.Reason += errors;
                        }

                        if (string.IsNullOrWhiteSpace(errors) && item.GLAccount.Trim() != "")
                        {
                            CurrentGLAccountList.Add(objDTO);

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportGLAccountColumn.UDF1.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportGLAccountColumn.UDF2.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportGLAccountColumn.UDF3.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportGLAccountColumn.UDF4.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportGLAccountColumn.UDF5.ToString(), RoomID);
                            // CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportGLAccountColumn.UDF6.ToString());

                        }
                        else
                        {
                            CurrentBlankGLAccountList.Add(objDTO);
                        }
                    }

                    List<GLAccountMasterMain> lstreturn = new List<GLAccountMasterMain>();
                    if (CurrentGLAccountList.Count > 0)
                    {
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.GLAccountMaster.ToString(), CurrentGLAccountList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }
                    if (CurrentBlankGLAccountList.Count > 0)
                    {
                        foreach (GLAccountMasterMain item in CurrentBlankGLAccountList)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<GLAccountMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}
                    //CacheHelper<IEnumerable<GLAccountMasterDTO>>.InvalidateCache();
                    //(new GLAccountMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);
                }

            }

        }
        public void SaveBarcodeMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
        List<ImportBarcodeMaster> CurrentBarcodeList, List<ItemManufacturer> CurrentItemManufacturerList,
        ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
        bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            {
                List<ImportBarcodeMaster> CurrentBlankBarcodemain = new List<ImportBarcodeMaster>();
                ImportBarcodeMaster[] LstItemManufacturermain = serializer.Deserialize<ImportBarcodeMaster[]>(para);
                if (LstItemManufacturermain != null && LstItemManufacturermain.Length > 0)
                {
                    CurrentBlankBarcodemain = new List<ImportBarcodeMaster>();

                    ImportBarcodeMaster objBarcodeDAL = new ImportBarcodeMaster();
                    CurrentItemManufacturerList = new List<ItemManufacturer>();
                    List<BarcodeMasterDTO> objManufacturerMasterDALList = new List<BarcodeMasterDTO>();
                    CurrentBarcodeList = new List<ImportBarcodeMaster>();

                    //List<BinMasterDTO> objBinMasterListDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                    List<BinMasterDTO> objBinMasterListDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinMasterByRoomID(RoomID, SessionHelper.CompanyID, false, false).ToList();
                    foreach (ImportBarcodeMaster item in LstItemManufacturermain)
                    {

                        ImportBarcodeMaster objDTO = new ImportBarcodeMaster();

                        ModuleMasterDTO objModuleMasterDTO = new ModuleMasterDAL(SessionHelper.EnterPriseDBName).GetModuleByNameNormal(item.ModuleName);
                        BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
                        BarcodeMasterDTO objBarcodeMasterDTO = new BarcodeMasterDTO();



                        if ((item.ItemNumber != null && item.ModuleName != string.Empty && item.BarcodeString != string.Empty))
                        {
                            Guid? ItemGUID;
                            if (item.ModuleName.ToLower().Trim() == "item master")
                            {
                                ItemMasterDAL objItemmasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                                ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                                ItemGUID = objItemmasterDAL.GetGuidByItemNumber(item.ItemNumber, RoomID, SessionHelper.CompanyID);
                            }
                            else if (item.ModuleName.ToLower().Trim() == "assets")
                            {
                                AssetMasterDAL objAssetmasterDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                                List<AssetMasterDTO> objAssetmasterDTO = new List<AssetMasterDTO>();
                                objAssetmasterDTO = objAssetmasterDAL.GetAllAssetsByRoom(RoomID, SessionHelper.CompanyID, false, false);
                                if (objAssetmasterDTO != null)
                                {
                                    //AssetMasterDTO objAssetMasterDTO = objAssetmasterDTO.Where(a => a.AssetName == item.ItemNumber).FirstOrDefault();
                                    AssetMasterDTO objAssetMasterDTO = objAssetmasterDAL.GetAssetsByName(item.ItemNumber, RoomID, SessionHelper.CompanyID).FirstOrDefault();
                                    if (objAssetMasterDTO != null)
                                    {
                                        ItemGUID = objAssetMasterDTO.GUID;
                                    }
                                    else
                                    {
                                        ItemGUID = Guid.Empty;
                                    }
                                }
                                else
                                {
                                    ItemGUID = Guid.Empty;
                                }
                            }
                            else if (item.ModuleName.ToLower().Trim() == "tool master")
                            {
                                ToolMasterDAL objToolmasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                                ToolMasterDTO objToolMasterDTO = objToolmasterDAL.GetToolBySerialPlain(item.ItemNumber, RoomID, SessionHelper.CompanyID);
                                if (objToolMasterDTO != null)
                                {
                                    ItemGUID = objToolMasterDTO.GUID;
                                }
                                else
                                {
                                    ItemGUID = Guid.Empty;
                                }

                            }
                            else
                            {
                                ItemGUID = Guid.Empty;
                            }
                            if (ItemGUID.HasValue && ItemGUID != Guid.Empty)
                            {
                                //objBarcodeMasterDTO = objBarcodeMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).ToList().Where(b => b.BarcodeString == item.BarcodeString && b.RefGUID == ItemGUID.Value).FirstOrDefault();
                                objBarcodeMasterDTO = objBarcodeMasterDAL.GetBarcodeByBarcodeStringRefGUID(RoomID, SessionHelper.CompanyID, item.BarcodeString, Convert.ToString(ItemGUID.Value)).FirstOrDefault();
                            }
                            else
                            {
                                objBarcodeMasterDTO = null;
                            }
                            if (objBarcodeMasterDTO != null)
                            {
                                objDTO = item;
                                objDTO.Status = "Fail";
                                objDTO.Reason = ResBarcodeMaster.BarcodeStringExists;
                                CurrentBlankBarcodemain.Add(objDTO);
                            }
                            else
                            {
                                if (ItemGUID.HasValue && ItemGUID != Guid.Empty)
                                {
                                    objDTO.RefGuid = ItemGUID.Value;
                                    objDTO.BarcodeString = item.BarcodeString;
                                    objDTO.ModuleGuid = objModuleMasterDTO.GUID;
                                    if (item.ModuleName == "Item Master")
                                    {


                                        if (!string.IsNullOrEmpty(item.BinNumber))
                                        {
                                            BinMasterDTO objBinMasterDTO = objBinMasterListDTO.Where(b => b.BinNumber.ToLower().Trim() == item.BinNumber.ToLower().Trim() && b.ItemGUID == ItemGUID.Value).FirstOrDefault();
                                            if (objBinMasterDTO != null)
                                            {
                                                objDTO.BinGuid = objBinMasterDTO.GUID;
                                            }
                                            else
                                            {
                                                BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                                                BinMasterDTO objBinMasterDTOToInsert = null;
                                                {
                                                    objBinMasterDTOToInsert = new BinMasterDTO();
                                                    objBinMasterDTOToInsert.BinNumber = item.BinNumber.Trim();
                                                    objBinMasterDTOToInsert.ParentBinId = null;
                                                    objBinMasterDTOToInsert.CreatedBy = SessionHelper.UserID;
                                                    objBinMasterDTOToInsert.LastUpdatedBy = SessionHelper.UserID;
                                                    objBinMasterDTOToInsert.Created = DateTimeUtility.DateTimeNow;
                                                    objBinMasterDTOToInsert.LastUpdated = DateTimeUtility.DateTimeNow;
                                                    objBinMasterDTOToInsert.Room = RoomID;
                                                    objBinMasterDTOToInsert.CompanyID = SessionHelper.CompanyID;
                                                    objBinMasterDTOToInsert.AddedFrom = "Web";
                                                    objBinMasterDTOToInsert.EditedFrom = "Web";
                                                    objBinMasterDTOToInsert.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                                    objBinMasterDTOToInsert.ReceivedOn = DateTimeUtility.DateTimeNow;
                                                    objBinMasterDTOToInsert.IsOnlyFromItemUI = true;
                                                    objBinMasterDTOToInsert = objBinMasterDAL.InsertBin(objBinMasterDTOToInsert);

                                                }
                                                objDTO.BinGuid = (objBinMasterDTOToInsert.GUID);
                                                BinMasterDTO objInventoryLocation = objBinMasterDAL.GetInventoryLocation(objBinMasterDTOToInsert.ID, ItemGUID.Value, RoomID, SessionHelper.CompanyID);

                                                if (objInventoryLocation == null)
                                                {
                                                    objInventoryLocation = new BinMasterDTO();
                                                    objInventoryLocation.BinNumber = objBinMasterDTOToInsert.BinNumber;
                                                    objInventoryLocation.ParentBinId = objBinMasterDTOToInsert.ID;
                                                    objInventoryLocation.CreatedBy = SessionHelper.UserID;
                                                    objInventoryLocation.LastUpdatedBy = SessionHelper.UserID;
                                                    objInventoryLocation.Created = DateTimeUtility.DateTimeNow;
                                                    objInventoryLocation.LastUpdated = DateTimeUtility.DateTimeNow;
                                                    objInventoryLocation.MinimumQuantity = objBinMasterDTOToInsert.MinimumQuantity;
                                                    objInventoryLocation.MaximumQuantity = objBinMasterDTOToInsert.MaximumQuantity;
                                                    objInventoryLocation.CriticalQuantity = objBinMasterDTOToInsert.CriticalQuantity;
                                                    objInventoryLocation.eVMISensorID = objBinMasterDTOToInsert.eVMISensorID;
                                                    objInventoryLocation.eVMISensorPort = objBinMasterDTOToInsert.eVMISensorPort;
                                                    objInventoryLocation.IsDefault = objBinMasterDTOToInsert.IsDefault;
                                                    objInventoryLocation.ItemGUID = ItemGUID.Value;
                                                    objInventoryLocation.Room = RoomID;
                                                    objInventoryLocation.CompanyID = SessionHelper.CompanyID;
                                                    objInventoryLocation.AddedFrom = "Web";
                                                    objInventoryLocation.EditedFrom = "Web";
                                                    objInventoryLocation.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                                    objInventoryLocation.ReceivedOn = DateTimeUtility.DateTimeNow;
                                                    objInventoryLocation.IsOnlyFromItemUI = true;
                                                    objInventoryLocation = objBinMasterDAL.InsertBin(objInventoryLocation);
                                                    objDTO.BinGuid = (objInventoryLocation.GUID);
                                                }
                                                else
                                                {
                                                    objInventoryLocation.LastUpdated = DateTimeUtility.DateTimeNow;
                                                    objInventoryLocation.LastUpdatedBy = SessionHelper.UserID;
                                                    objInventoryLocation.MinimumQuantity = objBinMasterDTOToInsert.MinimumQuantity;
                                                    objInventoryLocation.MaximumQuantity = objBinMasterDTOToInsert.MaximumQuantity;
                                                    objInventoryLocation.CriticalQuantity = objBinMasterDTOToInsert.CriticalQuantity;
                                                    objInventoryLocation.eVMISensorID = objBinMasterDTOToInsert.eVMISensorID;
                                                    objInventoryLocation.eVMISensorPort = objBinMasterDTOToInsert.eVMISensorPort;
                                                    objInventoryLocation.IsDefault = objBinMasterDTOToInsert.IsDefault;
                                                    objInventoryLocation.EditedFrom = "Web";
                                                    objInventoryLocation.IsOnlyFromItemUI = true;
                                                    objInventoryLocation.ReceivedOn = DateTimeUtility.DateTimeNow;
                                                    objBinMasterDAL.Edit(objInventoryLocation);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            BinMasterDTO objBinMasterDTO = objBinMasterListDTO.Where(b => b.IsDefault == true && b.ItemGUID == ItemGUID.Value).FirstOrDefault();
                                            if (objBinMasterDTO != null)
                                                objDTO.BinGuid = objBinMasterDTO.GUID;
                                        }

                                    }

                                    objDTO.ModuleName = item.ModuleName;
                                    objDTO.ItemNumber = item.ItemNumber;
                                    objDTO.IsDeleted = false;
                                    objDTO.IsArchived = false;
                                    objDTO.CreatedOn = DateTimeUtility.DateTimeNow;
                                    objDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                                    objDTO.UpdatedBy = SessionHelper.UserID;
                                    objDTO.RoomID = RoomID;
                                    objDTO.CompanyID = SessionHelper.CompanyID;
                                    objDTO.CreatedBy = SessionHelper.UserID;
                                    objDTO.GUID = Guid.NewGuid();
                                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objDTO.AddedFrom = "Web";
                                    objDTO.EditedFrom = "Web";
                                    objDTO.BarcodeAdded = "Manual";
                                }


                                string errors = ValidateModuleDTOBarcode<ImportBarcodeMaster, BarcodeMasterDTO>(objDTO, "Master", "BarcodeCreate"
                                                            , new List<string>() { "ID", "items" }
                                                            );

                                if (string.IsNullOrWhiteSpace(errors) == false)
                                {
                                    objDTO.Status = "Fail";
                                    objDTO.Reason += errors;
                                    item.Status = "Fail";
                                    item.Reason += errors;
                                }

                                if (string.IsNullOrWhiteSpace(errors) &&
                                    ItemGUID.HasValue && ItemGUID != Guid.Empty)
                                {
                                    var itemval = CurrentBarcodeList.FirstOrDefault(x => x.RefGuid == objDTO.RefGuid && x.ModuleGuid == objDTO.ModuleGuid && x.BarcodeString == objDTO.BarcodeString);
                                    if (itemval != null)
                                    {
                                        CurrentBarcodeList.Remove(itemval);
                                    }
                                    CurrentBarcodeList.Add(objDTO);
                                }
                                else
                                {
                                    objDTO = item;

                                    objDTO.Status = "Fail";
                                    if (item.ModuleName.ToLower().Trim() == "item master")
                                    {
                                        objDTO.Reason = string.Format(ResMessage.MsgDoesNotExist, ResItemMaster.ItemNumber);
                                    }
                                    else if (item.ModuleName.ToLower().Trim() == "assets")
                                    {
                                        objDTO.Reason = ResAssetMaster.MsgAssetDoesNotExist;
                                    }
                                    else if (item.ModuleName.ToLower().Trim() == "tool master")
                                    {
                                        objDTO.Reason = ResToolMaster.MsgToolDoesNotExist;
                                    }

                                    CurrentBlankBarcodemain.Add(objDTO);
                                }
                            }
                        }
                        else
                        {
                            objDTO = item;
                            objDTO.Status = "Fail";

                            objDTO.Reason = ResBarcodeMaster.BarcodeSaveRequiredMsg;


                            CurrentBlankBarcodemain.Add(objDTO);
                        }

                    }

                    List<ImportBarcodeMaster> lstreturn = new List<ImportBarcodeMaster>();
                    if (CurrentBarcodeList.Count > 0)
                    {
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.BarcodeMaster.ToString(), CurrentBarcodeList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }
                    if (CurrentBlankBarcodemain.Count > 0)
                    {
                        foreach (ImportBarcodeMaster item in CurrentBlankBarcodemain)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<ImportBarcodeMaster>(HasMoreRecords, IsFirstCall, lstreturn);
                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}

                    HttpContext.Current.Session["ItemManufacture"] = null;


                }

            }


        }


        public void SaveCategoryMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
        List<CategoryMasterMain> CurrentCategoryList,
        ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
        bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, string currentCulture,long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            {

                List<CategoryMasterMain> CurrentBlankCategoryList = new List<CategoryMasterMain>();
                CategoryMasterMain[] LstCategoryMaster = serializer.Deserialize<CategoryMasterMain[]>(para);
                if (LstCategoryMaster != null && LstCategoryMaster.Length > 0)
                {
                    CurrentCategoryList = new List<CategoryMasterMain>();
                    //lst = obj.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.CategoryMaster.ToString(), UDFControlTypes.Textbox.ToString());
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.CategoryMaster.ToString());
                    eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("CategoryMaster", RoomID, SessionHelper.CompanyID);
                    CurrentOptionList = new List<UDFOptionsMain>();

                    foreach (CategoryMasterMain item in LstCategoryMaster)
                    {
                        CategoryMasterMain objDTO = new CategoryMasterMain();
                        objDTO.ID = item.ID;
                        objDTO.Category = item.Category;
                        objDTO.UDF1 = item.UDF1;
                        objDTO.UDF2 = item.UDF2;
                        objDTO.UDF3 = item.UDF3;
                        objDTO.UDF4 = item.UDF4;
                        objDTO.UDF5 = item.UDF5;
                        objDTO.UDF6 = item.UDF6;
                        objDTO.UDF7 = item.UDF7;
                        objDTO.UDF8 = item.UDF8;
                        objDTO.UDF9 = item.UDF9;
                        objDTO.UDF10 = item.UDF10;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        item.CategoryColor = "";

                        /*/////CODE FOR CHECK UDF IS REQUIRED///////*/
                        string errorMsg = string.Empty;
                        CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, out errorMsg, SessionHelper.EnterPriceID, SessionHelper.CompanyID, RoomID, currentCulture);
                        if (!string.IsNullOrWhiteSpace(errorMsg))
                        {
                            item.Status = "Fail";
                            if (!string.IsNullOrEmpty(item.Reason))
                                item.Reason += errorMsg;
                            else
                                item.Reason = errorMsg;


                        }
                        /*/////CODE FOR CHECK UDF IS REQUIRED///////*/

                        string errors = ValidateModuleDTO<CategoryMasterMain, CategoryMasterDTO>(objDTO, "Master", "CategoryCreate"
                            , new List<string>() { "ID", "CategoryColor" }
                            );

                        if (string.IsNullOrWhiteSpace(errors) == false)
                        {
                            objDTO.Status = "Fail";
                            objDTO.Reason += errors;
                            item.Status = "Fail";
                            item.Reason += errors;
                        }

                        if (string.IsNullOrWhiteSpace(errors) &&
                            item.Category.Trim() != "" && string.IsNullOrWhiteSpace(item.Reason))
                        {
                            CurrentCategoryList.Add(objDTO);

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportCategoryColumn.UDF1.ToString(),RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportCategoryColumn.UDF2.ToString(),RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportCategoryColumn.UDF3.ToString(),RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportCategoryColumn.UDF4.ToString(),RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportCategoryColumn.UDF5.ToString(),RoomID);
                            //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportCategoryColumn.UDF6.ToString());

                        }
                        else
                        {
                            CurrentBlankCategoryList.Add(item);
                        }
                    }

                    List<CategoryMasterMain> lstreturn = new List<CategoryMasterMain>();
                    if (CurrentCategoryList.Count > 0)
                    {
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.CategoryMaster.ToString(), CurrentCategoryList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }
                    if (CurrentBlankCategoryList.Count > 0)
                    {
                        foreach (CategoryMasterMain item in CurrentBlankCategoryList)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                       // SaveImportDataListSession<CategoryMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);

                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<CategoryMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        //Session["importedData"] = lstreturn;

                    }
                    //CacheHelper<IEnumerable<CategoryMasterDTO>>.InvalidateCache();
                    //(new CategoryMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false, false);

                }

            }
        }

        public void SaveCostUOMMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
            List<CostUOMMasterMain> CurrentCostUOMList,
            ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
            bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, string currentCulture, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            {
                List<CostUOMMasterMain> CurrentBlankCategoryList = new List<CostUOMMasterMain>();
                CostUOMMasterMain[] LstCategoryMaster = serializer.Deserialize<CostUOMMasterMain[]>(para);
                if (LstCategoryMaster != null && LstCategoryMaster.Length > 0)
                {
                    CurrentCostUOMList = new List<CostUOMMasterMain>();
                    //lst = obj.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.CostUOMMaster.ToString(), UDFControlTypes.Textbox.ToString());
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.CostUOMMaster.ToString());

                    eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("CostUOM", RoomID, SessionHelper.CompanyID);
                    CurrentOptionList = new List<UDFOptionsMain>();
                    bool saveRecord = true;
                    foreach (CostUOMMasterMain item in LstCategoryMaster)
                    {
                        saveRecord = true;
                        CostUOMMasterMain objDTO = new CostUOMMasterMain();
                        objDTO.ID = item.ID;
                        objDTO.CostUOM = item.CostUOM;
                        if (item.CostUOMValue.GetValueOrDefault(0) > 0)
                        {
                            objDTO.CostUOMValue = item.CostUOMValue;
                        }
                        else
                        {
                            objDTO.Status = "Fail";
                            objDTO.Reason = ResCostUOMMaster.MsgCostUOMMinimumValue;
                            saveRecord = false;
                        }

                        string errorMsg = string.Empty;
                        CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, out errorMsg, SessionHelper.EnterPriceID, SessionHelper.CompanyID, RoomID, currentCulture);
                        if (!string.IsNullOrWhiteSpace(errorMsg))
                        {
                            objDTO.Status = "Fail";
                            if (!string.IsNullOrEmpty(objDTO.Reason))
                                objDTO.Reason += errorMsg;
                            else
                                objDTO.Reason = errorMsg;
                            saveRecord = false;
                            item.Status = objDTO.Status;
                            item.Reason = objDTO.Reason;

                        }

                        objDTO.UDF1 = item.UDF1;
                        objDTO.UDF2 = item.UDF2;
                        objDTO.UDF3 = item.UDF3;
                        objDTO.UDF4 = item.UDF4;
                        objDTO.UDF5 = item.UDF5;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";


                        string errors = ValidateModuleDTO<CostUOMMasterMain, CostUOMMasterDTO>(objDTO, "Master", "CostUOMCreate"
                            , new List<string>() { "ID" }
                            );

                        if (string.IsNullOrWhiteSpace(errors) == false)
                        {
                            objDTO.Status = "Fail";
                            objDTO.Reason += errors;
                            item.Status = "Fail";
                            item.Reason += errors;
                        }


                        if (string.IsNullOrWhiteSpace(errors) && item.CostUOM.Trim() != "" && saveRecord)
                        {
                            CurrentCostUOMList.Add(objDTO);

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportCostUOMColumn.UDF1.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportCostUOMColumn.UDF2.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportCostUOMColumn.UDF3.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportCostUOMColumn.UDF4.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportCostUOMColumn.UDF5.ToString(), RoomID);
                            //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportCategoryColumn.UDF6.ToString());

                        }
                        else
                        {
                            CurrentBlankCategoryList.Add(objDTO);
                        }
                    }

                    List<CostUOMMasterMain> lstreturn = new List<CostUOMMasterMain>();
                    if (CurrentCostUOMList.Count > 0)
                    {
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.CostUOMMaster.ToString(), CurrentCostUOMList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }
                    if (CurrentBlankCategoryList.Count > 0)
                    {
                        foreach (CostUOMMasterMain item in CurrentBlankCategoryList.Where(c => c.CostUOM.Trim() == ""))
                        {
                            item.Status = "Fail";
                            item.Reason = ResCostUOMMaster.MsgCostUOMshouldNotBlank;
                            lstreturn.Add(item);
                        }
                        foreach (CostUOMMasterMain item in CurrentBlankCategoryList.Where(c => c.CostUOMValue.GetValueOrDefault(0) <= 0))
                        {
                            item.Status = "Fail";
                            item.Reason = ResCostUOMMaster.MsgCostUOMMinimumValue;
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<CostUOMMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}
                    //CacheHelper<IEnumerable<CostUOMMasterDTO>>.InvalidateCache();
                    //(new CostUOMMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);

                }

            }
        }

        public void SaveCustomerMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
            List<CustomerMasterMain> CurrentCustomerList,
            ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
            bool IsFirstCall, ref string message, ref string status, ref bool allSuccesfulRecords, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {

            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            //else if (ImportMastersDTO.TableName.CustomerMaster.ToString() == TableName)
            {

                List<CustomerMasterMain> CurrentBlankCustomerList = new List<CustomerMasterMain>();
                CustomerMasterMain[] LstCustomerMaster = serializer.Deserialize<CustomerMasterMain[]>(para);
                if (LstCustomerMaster != null && LstCustomerMaster.Length > 0)
                {
                    CurrentCustomerList = new List<CustomerMasterMain>();
                    //  lst = obj.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.CustomerMaster.ToString(), UDFControlTypes.Textbox.ToString());
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.CustomerMaster.ToString());
                    CurrentOptionList = new List<UDFOptionsMain>();


                    foreach (CustomerMasterMain item in LstCustomerMaster)
                    {
                        CustomerMasterMain objDTO = new CustomerMasterMain();
                        objDTO.ID = item.ID;
                        objDTO.Customer = item.Customer;
                        objDTO.Account = item.Account;
                        objDTO.Contact = item.Contact;
                        objDTO.Address = item.Address;
                        objDTO.City = item.City;
                        objDTO.State = item.State;
                        objDTO.ZipCode = item.ZipCode;
                        objDTO.Country = item.Country;
                        objDTO.Phone = item.Phone;
                        objDTO.Email = item.Email;
                        objDTO.UDF1 = item.UDF1;
                        objDTO.UDF2 = item.UDF2;
                        objDTO.UDF3 = item.UDF3;
                        objDTO.UDF4 = item.UDF4;
                        objDTO.UDF5 = item.UDF5;
                        objDTO.UDF6 = item.UDF6;
                        objDTO.UDF7 = item.UDF7;
                        objDTO.UDF8 = item.UDF8;
                        objDTO.UDF9 = item.UDF9;
                        objDTO.UDF10 = item.UDF10;
                        objDTO.Remarks = item.Remarks;
                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        //MailAddress m = new MailAddress(item.Email); 

                        string errors = ValidateModuleDTO<CustomerMasterMain, CustomerMasterDTO>(objDTO, "Master", "CustomerMasterCreate"
                                                    , new List<string>() { "ID" }
                                                    , new Dictionary<string, string>() { }
                                                    );

                        if (string.IsNullOrWhiteSpace(errors) == false)
                        {
                            objDTO.Status = "Fail";
                            objDTO.Reason += errors;
                            item.Status = "Fail";
                            item.Reason += errors;
                        }


                        if (string.IsNullOrWhiteSpace(errors) && item.Customer.Trim() != "" && item.Account.Trim() != "" && IsValidEmail(item.Email))
                        {
                            CurrentCustomerList.Add(objDTO);

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportCustomerColumn.UDF1.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportCustomerColumn.UDF2.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportCustomerColumn.UDF3.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportCustomerColumn.UDF4.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportCustomerColumn.UDF5.ToString(), RoomID);
                            // CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportCustomerColumn.UDF6.ToString());

                        }
                        else
                        {
                            CurrentBlankCustomerList.Add(objDTO);
                        }
                    }

                    List<CustomerMasterMain> lstreturn = new List<CustomerMasterMain>();
                    if (CurrentCustomerList.Count > 0)
                    {
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.CustomerMaster.ToString(), CurrentCustomerList, RoomID, SessionHelper.CompanyID,
                            HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList);
                    }
                    if (CurrentBlankCustomerList.Count > 0)
                    {
                        foreach (CustomerMasterMain item in CurrentBlankCustomerList)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;


                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                          
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<CustomerMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}
                    //CacheHelper<IEnumerable<CustomerMasterDTO>>.InvalidateCache();
                    //(new CustomerMasterDAL(SessionHelper.EnterPriseDBName)).GetCachedData(0, SessionHelper.CompanyID, false, false);
                }

            }
        }


        public void SaveSupplierMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList,
        List<SupplierMasterMain> CurrentSupplierList,
        ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
        bool IsFirstCall, bool isImgZipAvail,
         ref string message, ref string status, ref string savedOnlyitemIds
        , ref bool allSuccesfulRecords,long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom)
        {
            long SessionUserId = SessionHelper.UserID;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);

            //else if (ImportMastersDTO.TableName.SupplierMaster.ToString() == TableName)
            {

                List<SupplierMasterMain> CurrentBlankSupplierList = new List<SupplierMasterMain>();
                SupplierMasterMain[] LstSupplierMaster = serializer.Deserialize<SupplierMasterMain[]>(para);
                if (LstSupplierMaster != null && LstSupplierMaster.Length > 0)
                {
                    CurrentSupplierList = new List<SupplierMasterMain>();
                    //lst = obj.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.SupplierMaster.ToString(), UDFControlTypes.Textbox.ToString());
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.SupplierMaster.ToString());

                    CurrentOptionList = new List<UDFOptionsMain>();

                    //bool IsEmailPOInBody = false; bool IsEmailPOInPDF = false; bool IsEmailPOInCSV = false; bool IsEmailPOInX12 = false;
                    foreach (SupplierMasterMain item in LstSupplierMaster)
                    {
                        SupplierMasterMain objDTO = new SupplierMasterMain();
                        objDTO.ID = item.ID;
                        objDTO.SupplierName = (item.SupplierName == null) ? null : (item.SupplierName.Length > 255 ? item.SupplierName.Substring(0, 255) : item.SupplierName);
                        objDTO.SupplierColor = (item.SupplierColor == null) ? null : (item.SupplierColor.Length > 255 ? item.SupplierColor.Substring(0, 255) : item.SupplierColor);
                        objDTO.Description = (item.Description == null) ? null : (item.Description.Length > 1024 ? item.Description.Substring(0, 1024) : item.Description);
                        objDTO.BranchNumber = (item.BranchNumber == null) ? null : (item.BranchNumber.Length > 255 ? item.BranchNumber.Substring(0, 255) : item.BranchNumber);
                        objDTO.MaximumOrderSize = item.MaximumOrderSize;
                        //objDTO.AccountNo = (item.AccountNo == null) ? null : (item.AccountNo.Length > 64 ? item.AccountNo.Substring(0, 64) : item.AccountNo);
                        //objDTO.ReceiverID = (item.ReceiverID == null) ? null : (item.ReceiverID.Length > 64 ? item.ReceiverID.Substring(0, 64) : item.ReceiverID);
                        objDTO.Address = (item.Address == null) ? null : (item.Address.Length > 1027 ? item.Address.Substring(0, 1027) : item.Address);
                        objDTO.City = (item.City == null) ? null : (item.City.Length > 127 ? item.City.Substring(0, 127) : item.City);
                        objDTO.State = (item.State == null) ? null : (item.State.Length > 255 ? item.State.Substring(0, 255) : item.State);
                        objDTO.ZipCode = (item.ZipCode == null) ? null : (item.ZipCode.Length > 20 ? item.ZipCode.Substring(0, 20) : item.ZipCode);
                        objDTO.Country = (item.Country == null) ? null : (item.Country.Length > 127 ? item.Country.Substring(0, 127) : item.Country);
                        objDTO.Contact = (item.Contact == null) ? null : (item.Contact.Length > 127 ? item.Contact.Substring(0, 127) : item.Contact);
                        objDTO.Phone = (item.Phone == null) ? null : (item.Phone.Length > 20 ? item.Phone.Substring(0, 20) : item.Phone);
                        objDTO.Fax = (item.Fax == null) ? null : (item.Fax.Length > 20 ? item.Fax.Substring(0, 20) : item.Fax);
                        objDTO.Email = (item.Email == null) ? null : (item.Email.Length > 255 ? item.Email.Substring(0, 255) : item.Email);

                        objDTO.IsSendtoVendor = item.IsSendtoVendor;
                        objDTO.IsVendorReturnAsn = item.IsVendorReturnAsn;
                        objDTO.IsSupplierReceivesKitComponents = item.IsSupplierReceivesKitComponents;

                        objDTO.IsEmailPOInBody = item.IsEmailPOInBody;
                        objDTO.IsEmailPOInPDF = item.IsEmailPOInPDF;
                        objDTO.IsEmailPOInCSV = item.IsEmailPOInCSV;
                        objDTO.IsEmailPOInX12 = item.IsEmailPOInX12;

                        objDTO.OrderNumberTypeBlank = item.OrderNumberTypeBlank;
                        objDTO.OrderNumberTypeFixed = item.OrderNumberTypeFixed;
                        objDTO.OrderNumberTypeBlanketOrderNumber = item.OrderNumberTypeBlanketOrderNumber;
                        objDTO.OrderNumberTypeIncrementingOrderNumber = item.OrderNumberTypeIncrementingOrderNumber;
                        objDTO.OrderNumberTypeIncrementingbyDay = item.OrderNumberTypeIncrementingbyDay;
                        objDTO.OrderNumberTypeDateIncrementing = item.OrderNumberTypeDateIncrementing;
                        objDTO.OrderNumberTypeDate = item.OrderNumberTypeDate;

                        if (objDTO.OrderNumberTypeBlank)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeBlank;
                        else if (objDTO.OrderNumberTypeFixed)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeFixed;
                        else if (objDTO.OrderNumberTypeBlanketOrderNumber)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeBlanketOrderNumber;
                        else if (objDTO.OrderNumberTypeIncrementingOrderNumber)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeIncrementingOrderNumber;
                        else if (objDTO.OrderNumberTypeIncrementingbyDay)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeIncrementingbyDay;
                        else if (objDTO.OrderNumberTypeDateIncrementing)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeDateIncrementing;
                        else if (objDTO.OrderNumberTypeDate)
                            objDTO.POAutoSequence = (int)CommonUtility.POAutoSequence.OrderNumberTypeDate;
                        else
                            objDTO.POAutoSequence = null;

                        objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);
                        objDTO.UDF6 = (item.UDF6 == null) ? null : (item.UDF6.Length > 255 ? item.UDF6.Substring(0, 255) : item.UDF6);
                        objDTO.UDF7 = (item.UDF7 == null) ? null : (item.UDF7.Length > 255 ? item.UDF7.Substring(0, 255) : item.UDF7);
                        objDTO.UDF8 = (item.UDF8 == null) ? null : (item.UDF8.Length > 255 ? item.UDF8.Substring(0, 255) : item.UDF8);
                        objDTO.UDF9 = (item.UDF9 == null) ? null : (item.UDF9.Length > 255 ? item.UDF9.Substring(0, 255) : item.UDF9);
                        objDTO.UDF10 = (item.UDF10 == null) ? null : (item.UDF10.Length > 255 ? item.UDF10.Substring(0, 255) : item.UDF10);

                        objDTO.AccountNumber = (item.AccountNumber == null) ? null : (item.AccountNumber.Length > 255 ? item.AccountNumber.Substring(0, 255) : item.AccountNumber);
                        objDTO.AccountName = (item.AccountName == null) ? null : (item.AccountName.Length > 255 ? item.AccountName.Substring(0, 255) : item.AccountName);
                        objDTO.AccountAddress = (item.AccountAddress == null) ? null : (item.AccountAddress.Length > 1027 ? item.AccountAddress.Substring(0, 1027) : item.AccountAddress);
                        objDTO.AccountCity = (item.AccountCity == null) ? null : (item.AccountCity.Length > 128 ? item.AccountCity.Substring(0, 128) : item.AccountCity);
                        objDTO.AccountState = (item.AccountState == null) ? null : (item.AccountState.Length > 256 ? item.AccountState.Substring(0, 256) : item.AccountState);
                        objDTO.AccountZip = (item.AccountZip == null) ? null : (item.AccountZip.Length > 20 ? item.AccountZip.Substring(0, 20) : item.AccountZip);

                        objDTO.AccountCountry = (item.AccountCountry == null) ? null : (item.AccountCountry.Length > 127 ? item.AccountCountry.Substring(0, 127) : item.AccountCountry);
                        objDTO.AccountShipToID = (item.AccountShipToID == null) ? null : (item.AccountShipToID.Length > 127 ? item.AccountShipToID.Substring(0, 127) : item.AccountShipToID);


                        if (item.AccountIsDefault == true)
                            objDTO.AccountIsDefault = item.AccountIsDefault;

                        objDTO.BlanketPONumber = (item.BlanketPONumber == null) ? null : (item.BlanketPONumber.Length > 255 ? item.BlanketPONumber.Substring(0, 255) : item.BlanketPONumber);
                        if (item.IsBlanketDeleted == true)
                            objDTO.IsBlanketDeleted = item.IsBlanketDeleted;
                        objDTO.StartDate = item.StartDate;
                        objDTO.EndDate = item.EndDate;

                        objDTO.MaxLimit = item.MaxLimit;
                        objDTO.IsNotExceed = item.IsNotExceed;

                        objDTO.MaxLimitQty = item.MaxLimitQty;
                        objDTO.IsNotExceedQty = item.IsNotExceedQty;

                        objDTO.IsDeleted = false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        //Wi-1195 added PullPurchaseNumberType fields in import of supplier
                        objDTO.PullPurchaseNumberFixed = item.PullPurchaseNumberFixed;
                        objDTO.PullPurchaseNumberBlanketOrder = item.PullPurchaseNumberBlanketOrder;
                        objDTO.PullPurchaseNumberDateIncrementing = item.PullPurchaseNumberDateIncrementing;
                        objDTO.PullPurchaseNumberDate = item.PullPurchaseNumberDate;

                        if (objDTO.PullPurchaseNumberFixed)
                            objDTO.PullPurchaseNumberType = (int)CommonUtility.PullPurchaseNumberType.PullPurchaseNumberFixed;
                        else if (objDTO.PullPurchaseNumberBlanketOrder)
                            objDTO.PullPurchaseNumberType = (int)CommonUtility.PullPurchaseNumberType.PullPurchaseNumberBlanketOrder;
                        else if (objDTO.PullPurchaseNumberDateIncrementing)
                            objDTO.PullPurchaseNumberType = (int)CommonUtility.PullPurchaseNumberType.PullPurchaseNumberDateIncrementing;
                        else if (objDTO.PullPurchaseNumberDate)
                            objDTO.PullPurchaseNumberType = (int)CommonUtility.PullPurchaseNumberType.PullPurchaseNumberDate;
                        else
                            objDTO.PullPurchaseNumberType = null;

                        objDTO.LastPullPurchaseNumberUsed = item.LastPullPurchaseNumberUsed;

                        objDTO.SupplierImage = item.SupplierImage;
                        objDTO.ImageExternalURL = item.ImageExternalURL;
                        objDTO.ImageType = "ExternalImage";
                        if (string.IsNullOrWhiteSpace(item.ImageExternalURL) && (!string.IsNullOrEmpty(item.SupplierImage)))
                        {
                            objDTO.ImageType = "ImagePath";
                        }

                        string errors = ValidateModuleDTO<SupplierMasterMain, SupplierMasterDTO>(objDTO, "Master", "SupplierCreate"
                            , new List<string>() { "ID", "NextOrderNo", "DefaultOrderRequiredDays", "LastPullPurchaseNumberUsed", "PullPurchaseNumberType", "POAutoSequence" }
                            , new Dictionary<string, string>() { { "MaximumOrderSize", "MaxOrderSize" } }
                            );

                        if (string.IsNullOrWhiteSpace(errors) == false)
                        {
                            objDTO.Status = "Fail";
                            objDTO.Reason += errors;
                            item.Status = "Fail";
                            item.Reason += errors;
                        }

                        if (string.IsNullOrWhiteSpace(errors) && item.SupplierName.Trim() != "" && item.Contact.Trim() != "" && item.Phone.Trim() != "")
                        {
                            CurrentSupplierList.Add(objDTO);

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportSupplierColumn.UDF1.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportSupplierColumn.UDF2.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportSupplierColumn.UDF3.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportSupplierColumn.UDF4.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportSupplierColumn.UDF5.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportSupplierColumn.UDF6.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF7 == null ? "" : objDTO.UDF7, CommonUtility.ImportSupplierColumn.UDF7.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF8 == null ? "" : objDTO.UDF8, CommonUtility.ImportSupplierColumn.UDF8.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF9 == null ? "" : objDTO.UDF9, CommonUtility.ImportSupplierColumn.UDF9.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF10 == null ? "" : objDTO.UDF10, CommonUtility.ImportSupplierColumn.UDF10.ToString(), RoomID);
                            //CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportSupplierColumn.UDF6.ToString());

                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(item.SupplierName))
                            {
                                objDTO.Reason += string.Format(ResMessage.Required, ResSupplierMaster.SupplierName);
                            }

                            if (string.IsNullOrWhiteSpace(item.Contact))
                            {
                                objDTO.Reason += string.Format(ResMessage.Required, ResSupplierMaster.Contact);
                            }

                            if (string.IsNullOrWhiteSpace(item.Phone))
                            {
                                objDTO.Reason += string.Format(ResMessage.Required, ResSupplierMaster.Phone);
                            }

                            objDTO.Status = "Fail";
                            CurrentBlankSupplierList.Add(objDTO);
                        }

                    } // foreach

                    List<SupplierMasterMain> lstreturn = new List<SupplierMasterMain>();
                    if (CurrentSupplierList.Count > 0)
                    {
                        bool IsInsertSupplierAccount = SessionHelper.GetModulePermission(SessionHelper.ModuleList.SupplierAccountDetail, SessionHelper.PermissionType.Insert);
                        bool IsUpdateSupplierAccount = SessionHelper.GetModulePermission(SessionHelper.ModuleList.SupplierAccountDetail, SessionHelper.PermissionType.Update);

                        //lstreturn = objImport.BulkInsert(ImportMastersDTO.TableName.SupplierMaster.ToString(), CurrentSupplierList, SessionHelper.RoomID, SessionHelper.CompanyID, Session["ColuumnList"].ToString(), SessionHelper.UserID, CurrentOptionList);
                        lstreturn = objImportDAL.BulkInsertWithChiled(ImportMastersDTO.TableName.SupplierMaster.ToString(), CurrentSupplierList, RoomID, SessionHelper.CompanyID,
                                    HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, IsInsertSupplierAccount, IsUpdateSupplierAccount, SessionHelper.EnterPriceID, CurrentOptionList, isImgZipAvail);
                    }

                    if (CurrentBlankSupplierList.Count > 0)
                    {
                        foreach (SupplierMasterMain item in CurrentBlankSupplierList)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {

                        /*----------CODE FOR SAVE ID AND IMAGE MAPPING FOR RETURN-------------- */

                        List<SupplierMasterMain> distSupplierList = lstreturn.GroupBy(l => l.SupplierName).Select(g => g.First()).ToList();

                        savedOnlyitemIds = string.Join(",", distSupplierList.Where(i => i.ID != 0 && (!string.IsNullOrEmpty(i.SupplierImage)) && i.Status != null && i.Status.ToLower() == "success").Select(p => p.ID.ToString() + "#" + p.SupplierImage.ToString()));

                        string SupplierNameList = string.Join("@", distSupplierList.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.SupplierImage)) && i.Status != null && i.Status.ToLower() == "success").Select(a => a.SupplierName));

                        string SupplierColorList = string.Join("@", distSupplierList.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.SupplierImage)) && i.Status != null && i.Status.ToLower() == "success").Select(a => a.SupplierColor));

                        List<SupplierMasterDTO> supplierList = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetRecordByNameAndColor(SupplierNameList, SupplierColorList, RoomID, SessionHelper.CompanyID);

                        foreach (SupplierMasterDTO b in supplierList)
                        {
                            SupplierMasterMain objSupplierMaster = distSupplierList.Where(i => i.SupplierName == b.SupplierName && i.SupplierColor == b.SupplierColor && (!string.IsNullOrEmpty(i.SupplierImage))).FirstOrDefault();
                            if (objSupplierMaster != null)
                            {
                                if (string.IsNullOrEmpty(savedOnlyitemIds))
                                {
                                    savedOnlyitemIds = b.ID + "#" + objSupplierMaster.SupplierImage.ToString();
                                }
                                else
                                {
                                    savedOnlyitemIds += "," + b.ID + "#" + objSupplierMaster.SupplierImage.ToString();
                                }
                            }
                        }
                        /*----------CODE FOR SAVE ID AND IMAGE MAPPING FOR RETURN-------------- */

                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<SupplierMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}

                }

            }
        }


        public void SaveItemMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList, List<Guid> lstItemGUID,
            List<BOMItemMasterMain> CurrentItemList,
            ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
            bool IsFirstCall, bool isImgZipAvail, bool isLink2ZipAvail
            , ref string message, ref string status, ref string savedOnlyitemIds
            , ref string savedItemIdsWithLink2, ref string savedItemGuids
            , ref bool allSuccesfulRecords, string currentCulture, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom
            )
        {
            {
                long SessionUserId = SessionHelper.UserID;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                //ImportDAL objImport = new ImportDAL(SessionHelper.EnterPriseDBName);
                ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);
                var itemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                var BinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                //List<UDFOptionsCheckDTO> lst = new List<UDFOptionsCheckDTO>();
                //List<UDFOptionsMain> CurrentOptionList = null;
                //List<BOMItemMasterMain> CurrentErrorItemList = new List<BOMItemMasterMain>();

                List<BOMItemMasterMain> CurrentBlankItemList = new List<BOMItemMasterMain>();
                BOMItemMasterMain[] LstItemMaster = serializer.Deserialize<BOMItemMasterMain[]>(para);

                if (LstItemMaster != null && LstItemMaster.Length > 0)
                {

                    CurrentItemList = new List<BOMItemMasterMain>();
                    //lst = obj.GetUDFList(SessionHelper.RoomID, ImportMastersDTO.TableName.ItemMaster.ToString(), UDFControlTypes.Textbox.ToString());
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.ItemMaster.ToString());

                    eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("ItemMaster", RoomID, SessionHelper.CompanyID);
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    eTurns.DAL.RoomDAL objRoomDAL = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName);
                    //RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(SessionHelper.RoomID);
                    string columnList = "ID,RoomName,IsAllowOrderCostuom,ReplenishmentType,GlobMarkupParts,GlobMarkupLabor";
                    RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

                    if (objRoomDTO == null || objRoomDTO.ID <= 0)
                    {
                        objRoomDTO = new RoomDTO();
                    }

                    DashboardParameterDTO objDashboardParameterDTO = new DashboardDAL(SessionHelper.EnterPriseDBName).GetDashboardParameters(RoomID, SessionHelper.CompanyID);

                    if (objDashboardParameterDTO == null || objDashboardParameterDTO.ID <= 0)
                    {
                        objDashboardParameterDTO = new DashboardParameterDTO();
                    }

                    bool IsAllowOrderCostuomRoom = true;
                    if (objRoomDTO.IsAllowOrderCostuom == false)
                    {
                        IsAllowOrderCostuomRoom = false;
                    }
                    bool CurrentRoomConsigned = objRoomDAL.checkRoomConsignedOrNotbyRoomId(RoomID);

                    CurrentOptionList = new List<UDFOptionsMain>();
                    bool SaveToolList = true;
                    List<CostUOMMasterDTO> lstCostUOMMasterDTO = new List<CostUOMMasterDTO>();
                    lstCostUOMMasterDTO = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName).GetCostUOMsByRoomID(RoomID, SessionHelper.CompanyID).ToList();
                    List<ItemMasterDTO> lstItemMaster = itemMasterDAL.GetAllItemsWithJoins(RoomID, SessionHelper.CompanyID, false, false, string.Empty).ToList();
                    //bool IsEmailPOInBody = false; bool IsEmailPOInPDF = false; bool IsEmailPOInCSV = false; bool IsEmailPOInX12 = false;
                    foreach (BOMItemMasterMain item in LstItemMaster)
                    {
                        SaveToolList = true;

                        BOMItemMasterMain objDTO = new BOMItemMasterMain();
                        objDTO.ID = item.ID;
                        objDTO.ItemNumber = (item.ItemNumber == null) ? null : (item.ItemNumber.Length > 255 ? item.ItemNumber.Substring(0, 255) : item.ItemNumber);
                        objDTO.ManufacturerName = item.ManufacturerName;
                        //Wi-1505
                        if (!string.IsNullOrWhiteSpace(item.ManufacturerName))
                        {
                            objDTO.ManufacturerID = item.ManufacturerName == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.ManufacturerMaster, item.ManufacturerName, RoomID: RoomID);
                        }
                        else
                        {
                            long ManuID = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName).GetORInsertBlankManuFacID(RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                            if (!string.IsNullOrWhiteSpace(item.ManufacturerNumber))
                            {
                                objDTO.ManufacturerID = ManuID;
                            }

                        }
                        if (!CurrentRoomConsigned)
                        {
                            item.Consignment = false;
                        }
                        objDTO.ManufacturerNumber = (item.ManufacturerNumber == null) ? null : (item.ManufacturerNumber.Length > 50 ? item.ManufacturerNumber.Substring(0, 50) : item.ManufacturerNumber);
                        objDTO.Link1 = item.Link1;
                        objDTO.Link2 = item.Link2;
                        if (IsAllowOrderCostuomRoom == false)
                        {
                            objDTO.IsAllowOrderCostuom = false;
                        }
                        else
                        {
                            objDTO.IsAllowOrderCostuom = item.IsAllowOrderCostuom;
                        }
                        ItemMasterDTO objItem = lstItemMaster.Where(t => t.ItemNumber == objDTO.ItemNumber).FirstOrDefault();
                        if (objItem != null)
                        {
                            if (((objItem.OnOrderQuantity ?? 0) > 0 || ((objItem.OnOrderInTransitQuantity ?? 0) > 0)) && (objItem.IsAllowOrderCostuom) != objDTO.IsAllowOrderCostuom)
                            {
                                SaveToolList = false;
                                objDTO.Status = "Fail";
                                objDTO.Reason = string.Format(ResItemMaster.CostUOMOnOrderInTransitQuantity, ResItemMaster.OnOrderQuantity, objDTO.ItemNumber);
                                item.Status = "Fail";
                                item.Reason = string.Format(ResItemMaster.CostUOMOnOrderInTransitQuantity, ResItemMaster.OnOrderQuantity, objDTO.ItemNumber);
                            }
                        }
                        objDTO.ItemLink2ImageType = "InternalLink";
                        objDTO.ImageType = "ExternalImage";
                        objDTO.ItemImageExternalURL = item.ItemImageExternalURL;
                        objDTO.ItemLink2ExternalURL = item.ItemLink2ExternalURL;

                        //if (!string.IsNullOrWhiteSpace(objDTO.ItemImageExternalURL))
                        //{
                        //    bool Isvalid = CommonUtility.ValidateExternalImage(objDTO.ItemImageExternalURL);
                        //    if (!Isvalid)
                        //    {
                        //        SaveToolList = false;
                        //        objDTO.Status = "Fail";
                        //        objDTO.Reason = string.Format(ResItemMaster.InvalidImageURL, "ItemImageExternalURL");
                        //        item.Status = "Fail";
                        //        item.Reason = string.Format(ResItemMaster.InvalidImageURL, "ItemImageExternalURL");
                        //    }
                        //}
                        if (string.IsNullOrWhiteSpace(item.ItemImageExternalURL) && (!string.IsNullOrEmpty(item.ImagePath)))
                        {
                            objDTO.ImageType = "ImagePath";
                        }
                        if (string.IsNullOrWhiteSpace(item.Link2) && (!string.IsNullOrEmpty(item.ItemLink2ExternalURL)))
                        {
                            objDTO.ItemLink2ImageType = "ExternalURL";
                        }
                        objDTO.ItemDocExternalURL = item.ItemDocExternalURL;
                        objDTO.IsActive = item.IsActive;
                        objDTO.IsOrderable = item.IsOrderable;
                        if (item.ItemTypeName == "Labor")
                        {
                            objDTO.SupplierName = string.Empty;
                            objDTO.SupplierPartNo = null;
                            objDTO.SupplierID = null;
                            objDTO.BlanketOrderNumber = string.Empty;
                            objDTO.BlanketPOID = null;
                        }
                        else
                        {
                            objDTO.SupplierName = item.SupplierName;
                            objDTO.SupplierPartNo = (item.SupplierPartNo == null) ? null : (item.SupplierPartNo.Length > 50 ? item.SupplierPartNo.Substring(0, 50) : item.SupplierPartNo);
                            objDTO.SupplierID = GetIDs(ImportMastersDTO.TableName.SupplierMaster, item.SupplierName, RoomID: RoomID);

                            // Start -- insert into Room Schedule for PullSchedule type is Immediate 
                            if (objDTO.SupplierID > 0)
                            {
                                SchedulerDTO objSchedulerDTO = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetRoomSchedule(objDTO.SupplierID.GetValueOrDefault(0), RoomID, 7);
                                if (objSchedulerDTO == null)
                                {
                                    // insert into Room Schedule for PullSchedule type is Immediate 
                                    SchedulerDTO objPullSchedulerDTO = new SchedulerDTO();
                                    objPullSchedulerDTO.SupplierId = objDTO.SupplierID.GetValueOrDefault(0);
                                    objPullSchedulerDTO.CompanyId = SessionHelper.CompanyID;
                                    objPullSchedulerDTO.RoomId = RoomID;
                                    objPullSchedulerDTO.LoadSheduleFor = 7;
                                    objPullSchedulerDTO.ScheduleMode = 5;
                                    objPullSchedulerDTO.IsScheduleActive = true;
                                    objPullSchedulerDTO.MonthlyDayOfMonth = 2;
                                    new SupplierMasterDAL(SessionHelper.EnterPriseDBName).SaveSupplierSchedule(objPullSchedulerDTO);
                                }
                            }

                            /// End- logic for insert into Room Schedule for PullSchedule type is Immediate

                            objDTO.BlanketOrderNumber = item.BlanketOrderNumber;
                            if (objDTO.SupplierID.HasValue && objDTO.SupplierID.Value > 0 && !string.IsNullOrWhiteSpace(item.BlanketOrderNumber))
                                objDTO.BlanketPOID = GetIDs(ImportMastersDTO.TableName.SupplierBlanketPODetails, item.BlanketOrderNumber, objDTO.SupplierID.Value, RoomID: RoomID);
                        }

                        objDTO.UPC = item.UPC;
                        objDTO.UNSPSC = item.UNSPSC;
                        objDTO.Description = item.Description;
                        objDTO.LongDescription = item.LongDescription;
                        objDTO.CategoryID = item.CategoryName == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.CategoryMaster, item.CategoryName, RoomID: RoomID);
                        objDTO.GLAccountID = item.GLAccount == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.GLAccountMaster, item.GLAccount, RoomID: RoomID);
                        objDTO.CategoryName = item.CategoryName;
                        objDTO.GLAccount = item.GLAccount;
                        objDTO.UOMID = GetIDs(ImportMastersDTO.TableName.UnitMaster, item.Unit, RoomID: RoomID);
                        objDTO.Unit = item.Unit;
                        objDTO.PricePerTerm = item.PricePerTerm;
                        objDTO.CostUOMID = item.CostUOMName == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.CostUOMMaster, item.CostUOMName, RoomID: RoomID);
                        objDTO.CostUOMName = item.CostUOMName;

                        objDTO.DefaultReorderQuantity = item.DefaultReorderQuantity;
                        objDTO.DefaultPullQuantity = item.DefaultPullQuantity;


                        item.Cost = item.Cost ?? 0;
                        item.SellPrice = item.SellPrice ?? 0;
                        item.Markup = item.Markup ?? 0;

                        objDTO.Cost = item.Cost;
                        objDTO.SellPrice = item.SellPrice;
                        objDTO.Markup = item.Markup;

                        if (item.Cost != 0 && item.SellPrice != 0 && item.Markup != 0)
                        {
                            objDTO.Markup = Convert.ToDouble(((Convert.ToDecimal(objDTO.SellPrice) * Convert.ToDecimal(100)) / Convert.ToDecimal(objDTO.Cost)) - Convert.ToDecimal(100));
                            // Calculate MArkup based on price and cost
                        }
                        else if (item.Cost != 0 && item.SellPrice == 0 && item.Markup == 0)
                        {
                            // Prise will become same as cost
                            objDTO.SellPrice = objDTO.Cost;
                        }
                        else if (item.Cost != 0 && item.SellPrice != 0 && item.Markup == 0)
                        {
                            // Calculate MArkup based on price and cost
                            objDTO.Markup = Convert.ToDouble(((Convert.ToDecimal(objDTO.SellPrice) * Convert.ToDecimal(100)) / Convert.ToDecimal(objDTO.Cost)) - Convert.ToDecimal(100));
                        }
                        else if (item.Cost != 0 && item.SellPrice == 0 && item.Markup != 0)
                        {
                            //Calculate prise based on cost and markup
                            objDTO.SellPrice = Convert.ToDouble(Convert.ToDecimal(objDTO.Cost) + ((Convert.ToDecimal(objDTO.Cost) * Convert.ToDecimal(objDTO.Markup)) / Convert.ToDecimal(100)));
                        }
                        else if (item.Cost == 0 && item.SellPrice != 0 && item.Markup != 0)
                        {
                            // Calculate cost based on prise and markup                                 
                            //objDTO.Cost = objDTO.SellPrice - ((objDTO.SellPrice * objDTO.Markup) / 100);
                            objDTO.Cost = Convert.ToDouble((Convert.ToDecimal(100) * Convert.ToDecimal(item.SellPrice)) / (Convert.ToDecimal(item.Markup) + Convert.ToDecimal(100)));
                            //Please follow Markup= (((sell-cost)/cost)*100)
                        }
                        else if (item.Cost == 0 && item.SellPrice == 0 && item.Markup == 0)
                        {
                            // All are zero so no calc
                        }
                        else if (item.Cost == 0 && item.SellPrice != 0 && item.Markup == 0)
                        {
                            // cost will become same as prise
                            objDTO.Cost = objDTO.SellPrice;
                        }
                        else if (item.Cost == 0 && item.SellPrice == 0 && item.Markup != 0)
                        {
                            bool isSetMarkupZero = true;
                            if (objRoomDTO != null)
                            {
                                if (item.ItemType == 4)
                                {
                                    if (objRoomDTO.GlobMarkupLabor > 0)
                                    {
                                        isSetMarkupZero = false;
                                    }
                                }
                                else if (objRoomDTO.GlobMarkupParts > 0)
                                {
                                    isSetMarkupZero = false;
                                }
                            }
                            if (isSetMarkupZero == false)
                            {
                                objDTO.Markup = item.Markup;
                            }
                            else
                                objDTO.Markup = 0;
                            // Cost and prise will remain zero and save markup only Or make markup zero because no prise and cost
                        }

                        objDTO.ExtendedCost = 0;// item.ExtendedCost;
                        objDTO.DispExtendedCost = item.ExtendedCost;

                        objDTO.LeadTimeInDays = item.LeadTimeInDays;
                        objDTO.Trend = item.Trend;
                        objDTO.Taxable = item.Taxable;
                        objDTO.Consignment = item.Consignment;
                        objDTO.StagedQuantity = 0;// item.StagedQuantity;
                        objDTO.InTransitquantity = 0;// item.InTransitquantity;
                        objDTO.OnOrderQuantity = 0;// item.OnOrderQuantity;
                        objDTO.OnTransferQuantity = 0;// item.OnTransferQuantity;
                        objDTO.SuggestedOrderQuantity = 0;// item.SuggestedOrderQuantity;
                        objDTO.RequisitionedQuantity = 0;// item.RequisitionedQuantity;
                        objDTO.AverageUsage = 0;// item.AverageUsage;
                        objDTO.Turns = 0;// item.Turns;
                        if (item.ItemType != 4)
                        {
                            objDTO.OnHandQuantity = item.OnHandQuantity;
                        }

                        objDTO.DispStagedQuantity = item.StagedQuantity;
                        objDTO.DispInTransitquantity = item.InTransitquantity;
                        objDTO.DispOnOrderQuantity = item.OnOrderQuantity;
                        objDTO.DispOnTransferQuantity = item.OnTransferQuantity;
                        objDTO.DispSuggestedOrderQuantity = item.SuggestedOrderQuantity;
                        objDTO.DispRequisitionedQuantity = item.RequisitionedQuantity;
                        objDTO.DispAverageUsage = item.AverageUsage;
                        objDTO.DispTurns = item.Turns;

                        objDTO.IsPackslipMandatoryAtReceive = item.IsPackslipMandatoryAtReceive;
                        if (!string.IsNullOrWhiteSpace(objRoomDTO.ReplenishmentType))
                        {
                            if (objRoomDTO.ReplenishmentType == "1")
                            {
                                objDTO.IsItemLevelMinMaxQtyRequired = true;
                            }
                            else if (objRoomDTO.ReplenishmentType == "2")
                            {
                                objDTO.IsItemLevelMinMaxQtyRequired = false;
                            }
                            else if (objRoomDTO.ReplenishmentType == "3")
                            {
                                objDTO.IsItemLevelMinMaxQtyRequired = item.IsItemLevelMinMaxQtyRequired;
                            }
                        }
                        objDTO.CriticalQuantity = item.CriticalQuantity;
                        objDTO.MinimumQuantity = item.MinimumQuantity;
                        objDTO.MaximumQuantity = item.MaximumQuantity;
                        objDTO.WeightPerPiece = item.WeightPerPiece;
                        if (objDTO.IsAllowOrderCostuom == true)
                        {
                            item.IsEnforceDefaultReorderQuantity = true;

                            if (lstCostUOMMasterDTO.Where(t => t.CostUOM == item.CostUOMName).Count() == 0)
                            {
                                lstCostUOMMasterDTO = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName).GetCostUOMsByRoomID(RoomID, SessionHelper.CompanyID).ToList();
                            }
                            if (lstCostUOMMasterDTO.Where(t => t.CostUOM == item.CostUOMName).Count() > 0)
                            {
                                CostUOMMasterDTO cuom = lstCostUOMMasterDTO.Where(t => t.CostUOM == item.CostUOMName).FirstOrDefault();
                                bool value = objCommonDAL.CheckItemCostUOM(cuom.ID, Convert.ToInt64(item.DefaultReorderQuantity ?? 0));
                                if (!value)
                                {
                                    objDTO.DefaultReorderQuantity = (cuom.CostUOMValue ?? 0);
                                }
                                else
                                {
                                    Double Remainder = (item.DefaultReorderQuantity ?? 0) % (cuom.CostUOMValue ?? 0);
                                    if (Remainder != 0)
                                    {
                                        objDTO.DefaultReorderQuantity = (cuom.CostUOMValue ?? 0);

                                    }
                                }


                            }
                        }


                        objDTO.ItemUniqueNumber = objCommonDAL.GetItemUniqueIdByRoom(item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));

                        objDTO.IsTransfer = item.IsTransfer;
                        objDTO.IsPurchase = item.IsPurchase;
                        if ((item.IsDeleted ?? false) == false)
                        {
                            objDTO.DefaultLocation = objImportDAL.GetBinForImportItem(item.InventryLocation, objDTO.ItemNumber, RoomID, SessionHelper.CompanyID, SessionHelper.UserID); // GetIDs(ImportMastersDTO.TableName.BinMaster, item.InventryLocation);
                        }
                        objDTO.InventryLocation = item.InventryLocation;
                        objDTO.InventoryClassification = Convert.ToInt32(GetIDs(ImportMastersDTO.TableName.InventoryClassificationMaster, item.InventoryClassificationName, RoomID: RoomID));
                        objDTO.InventoryClassificationName = item.InventoryClassificationName;
                        objDTO.ItemTypeName = item.ItemTypeName;
                        objDTO.SerialNumberTracking = item.SerialNumberTracking;
                        objDTO.LotNumberTracking = item.LotNumberTracking;
                        objDTO.DateCodeTracking = item.DateCodeTracking;
                        objDTO.ItemType = item.ItemTypeName == "Item" ? 1 : item.ItemTypeName == "Quick List" ? 2 : item.ItemTypeName == "Kit" ? 3 : item.ItemTypeName == "Labor" ? 4 : 1;
                        objDTO.ImagePath = item.ImagePath;
                        objDTO.OnHandQuantity = (!objDTO.SerialNumberTracking && !objDTO.LotNumberTracking && !objDTO.DateCodeTracking) ? objDTO.OnHandQuantity : 0;
                        objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);

                        objDTO.UDF6 = (item.UDF6 == null) ? null : (item.UDF6.Length > 255 ? item.UDF6.Substring(0, 255) : item.UDF6);
                        objDTO.UDF7 = (item.UDF7 == null) ? null : (item.UDF7.Length > 255 ? item.UDF7.Substring(0, 255) : item.UDF7);
                        objDTO.UDF8 = (item.UDF8 == null) ? null : (item.UDF8.Length > 255 ? item.UDF8.Substring(0, 255) : item.UDF8);
                        objDTO.UDF9 = (item.UDF9 == null) ? null : (item.UDF9.Length > 255 ? item.UDF9.Substring(0, 255) : item.UDF9);
                        objDTO.UDF10 = (item.UDF10 == null) ? null : (item.UDF10.Length > 255 ? item.UDF10.Substring(0, 255) : item.UDF10);

                        objDTO.CategoryColor = "";
                        objDTO.IsLotSerialExpiryCost = item.IsLotSerialExpiryCost;
                        if (objDTO.ItemType == 3)
                        {
                            objDTO.IsBuildBreak = item.IsBuildBreak;
                        }
                        objDTO.IsAutoInventoryClassification = item.IsAutoInventoryClassification;
                        objDTO.PullQtyScanOverride = item.PullQtyScanOverride;
                        objDTO.IsEnforceDefaultReorderQuantity = item.IsEnforceDefaultReorderQuantity ?? false;
                        if (objDashboardParameterDTO.IsTrendingEnabled)
                        {
                            objDTO.TrendingSetting = getTrendingID(item.TrendingSettingName);
                            objDTO.TrendingSettingName = item.TrendingSettingName;
                        }
                        else
                        {
                            item.TrendingSetting = 0;
                            item.TrendingSettingName = "None";
                            objDTO.TrendingSetting = 0;
                            objDTO.TrendingSettingName = "None";
                        }

                        objDTO.IsDeleted = item.IsDeleted ?? false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.WhatWhereAction = "Import";
                        objDTO.eLabelKey = (item.eLabelKey == null ? null : item.eLabelKey);
                        objDTO.EnrichedProductData = (item.EnrichedProductData == null ? null : item.EnrichedProductData);
                        objDTO.EnhancedDescription = (item.EnhancedDescription == null ? null : item.EnhancedDescription);
                        if (objDTO.ItemType == 4 && (objDTO.DefaultPullQuantity == null || objDTO.DefaultPullQuantity <= 0))
                        {
                            objDTO.DefaultPullQuantity = 1;
                        }
                        objDTO.POItemLineNumber = item.POItemLineNumber;
                        lstItemGUID.Add(objDTO.GUID);

                        if (!string.IsNullOrWhiteSpace((item.SupplierPartNo ?? string.Empty).Trim()) || ((!string.IsNullOrWhiteSpace(item.ItemTypeName)) && item.ItemTypeName == "Labor"))
                        {
                            //ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                            //bool Result = objItemSupplierDetailsDAL.CheckSupplierDuplicateByItemNumber(item.SupplierPartNo.Trim(), SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.ItemNumber, false);
                            //if (!Result)
                            //{
                            //    SaveToolList = false;
                            //    objDTO.Status = "Fail";
                            //    objDTO.Reason = ResMessage.DuplicateSupPartNo;
                            //    item.Status = "Fail";
                            //    item.Reason = ResMessage.DuplicateSupPartNo;
                            //}
                        }
                        else
                        {
                            SaveToolList = false;
                            objDTO.Status = "Fail";
                            objDTO.Reason = string.Format(ResMessage.Required, "SupplierPartNo");
                            item.Status = "Fail";
                            item.Reason = string.Format(ResMessage.Required, "SupplierPartNo"); ;
                        }



                        /*/////CODE FOR CHECK UDF IS REQUIRED///////*/
                        string errorMsg = string.Empty;
                        CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, out errorMsg, SessionHelper.EnterPriceID, SessionHelper.CompanyID, RoomID, currentCulture, null, item.UDF6, item.UDF7, item.UDF8, item.UDF9, item.UDF10);
                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            item.Status = "Fail";
                            if (!string.IsNullOrEmpty(item.Reason))
                                item.Reason += errorMsg;
                            else
                                item.Reason = errorMsg;
                            SaveToolList = false;
                        }
                        /*/////CODE FOR CHECK UDF IS REQUIRED///////*/

                        string errors = ValidateModuleDTO<BOMItemMasterMain, ItemMasterDTO>(objDTO, "Inventory", "ItemCreate", new List<string>() { "ID" });

                        if (string.IsNullOrWhiteSpace(errors) == false)
                        {
                            SaveToolList = false;
                            objDTO.Status = "Fail";
                            objDTO.Reason += errors;
                            item.Status = "Fail";
                            item.Reason += errors;
                        }

                        if (item.ItemNumber.Trim() != "" && SaveToolList)
                        {
                            var itemval = CurrentItemList.FirstOrDefault(x => x.ItemNumber.ToLower() == item.ItemNumber.ToLower());
                            if (itemval != null)
                                CurrentItemList.Remove(itemval);
                            CurrentItemList.Add(objDTO);
                            //CurrentItemDTOList.Add(objDTO);

                            item.Status = "Success";
                            item.Reason = "N/A";

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportItemColumn.UDF1.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportItemColumn.UDF2.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportItemColumn.UDF3.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportItemColumn.UDF4.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportItemColumn.UDF5.ToString(), RoomID);

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportItemColumn.UDF6.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF7 == null ? "" : objDTO.UDF7, CommonUtility.ImportItemColumn.UDF7.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF8 == null ? "" : objDTO.UDF8, CommonUtility.ImportItemColumn.UDF8.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF9 == null ? "" : objDTO.UDF9, CommonUtility.ImportItemColumn.UDF9.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF10 == null ? "" : objDTO.UDF10, CommonUtility.ImportItemColumn.UDF10.ToString(), RoomID);

                        }
                        else
                            CurrentBlankItemList.Add(item);
                    }

                    List<BOMItemMasterMain> lstreturn = new List<BOMItemMasterMain>();


                    if (CurrentItemList.Count > 0)
                    {
                        HttpContext.Current.Session["SaveBinCreatedDate"] = itemMasterDAL.GetDBUTCTime();// DateTimeUtility.DateTimeNow.AddMinutes(-1); //Note: difference in db utc time and c# utc time was causing issue in display bin change history warning that's why DB utc time is used at both the end.
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.ItemMaster.ToString(), CurrentItemList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList, isImgZipAvail, isLink2ZipAvail);
                    }


                    //if (CurrentErrorItemList.Count > 0)
                    //{
                    //    foreach (BOMItemMasterMain item in CurrentErrorItemList)
                    //    {
                    //        lstreturn.Add(item);
                    //    }
                    //}

                    if (CurrentBlankItemList.Count > 0)
                    {
                        foreach (BOMItemMasterMain item in CurrentBlankItemList)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        this.ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        savedOnlyitemIds = string.Join(",", lstreturn.Where(i => i.ID != 0 && (!string.IsNullOrEmpty(i.ImagePath))).Select(p => p.ID.ToString() + "#" + p.ImagePath.ToString()));
                        savedItemIdsWithLink2 = string.Join(",", lstreturn.Where(i => i.ID != 0 && (!string.IsNullOrEmpty(i.Link2))).Select(p => p.ID.ToString() + "#" + p.Link2.ToString()));
                        savedItemGuids = string.Join(",", lstreturn.Where(i => i.GUID != Guid.Empty).Select(p => p.GUID.ToString()));
                        //objDTO.Link2 = item.Link2;
                        //objDTO.ItemLink2ImageType = "InternalLink";

                        foreach (BOMItemMasterMain b in lstreturn.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.ImagePath))))
                        {
                            //long id = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetIDByItemNumber(Convert.ToString(b.ItemNumber), Convert.ToInt64(b.Room), Convert.ToInt64(b.CompanyID));
                            ItemMasterDTO objItemMaster = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByItemNumberPlain(Convert.ToString(b.ItemNumber), Convert.ToInt64(b.Room), Convert.ToInt64(b.CompanyID));
                            if (objItemMaster != null)
                            {
                                if (string.IsNullOrEmpty(savedOnlyitemIds))
                                {
                                    savedOnlyitemIds = objItemMaster.ID + "#" + objItemMaster.ImagePath.ToString();
                                }
                                else
                                {
                                    savedOnlyitemIds += "," + objItemMaster.ID + "#" + objItemMaster.ImagePath.ToString();
                                }
                            }
                        }

                        foreach (BOMItemMasterMain b in lstreturn.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.Link2))))
                        {
                            ItemMasterDTO objItemMaster = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByItemNumberPlain(Convert.ToString(b.ItemNumber), Convert.ToInt64(b.Room), Convert.ToInt64(b.CompanyID));
                            if (objItemMaster != null)
                            {
                                if (string.IsNullOrEmpty(savedItemIdsWithLink2))
                                {
                                    savedItemIdsWithLink2 = objItemMaster.ID + "#" + objItemMaster.Link2.ToString();
                                }
                                else
                                {
                                    savedItemIdsWithLink2 += "," + objItemMaster.ID + "#" + objItemMaster.Link2.ToString();
                                }
                            }
                        }

                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<BOMItemMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}
                    CostUOMMasterDAL objCostUOMDAL = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);

                    if (lstreturn.Count() > 0 && lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").ToList().Count() > 0)
                    {
                        foreach (var item in lstreturn)
                        {
                            if (item.SupplierPartNo != String.Empty && item.SupplierPartNo != null)
                            {
                                //string costUOMValue = objCostUOMDAL.GetCostUOMByID(item.CostUOMID.GetValueOrDefault(0)).CostUOMValue.ToString();
                                string DefaultLocationName = BinMasterDAL.GetBinByID(Int64.Parse(Convert.ToString(item.DefaultLocation)), RoomID, SessionHelper.CompanyID).BinNumber;
                                itemMasterDAL.AddUpdateSolumnProduct(item.SupplierPartNo, item.ItemNumber, item.GUID, item.Description, item.MinimumQuantity.ToString(), item.MaximumQuantity.ToString(), item.DefaultReorderQuantity, item.CostUOMID, DefaultLocationName, item.OnOrderQuantity, string.Empty, SessionHelper.EnterPriceID, SessionHelper.CompanyID, RoomID,item.eLabelKey);
                            }
                        }
                    }

                    lstItemGUID = new List<Guid>();
                    foreach (BOMItemMasterMain objitemguid in lstreturn.Where(t => t.IsDeleted == false))
                    {
                        if (objitemguid.Status.ToLower() == "success")
                        {
                            lstItemGUID.Add(objitemguid.GUID);
                        }
                    }

                }

            }
        }
        public void EditItemMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList, List<Guid> lstItemGUID,
         List<BOMItemMasterMain> CurrentItemList,
         ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
         bool IsFirstCall, bool isImgZipAvail, bool isLink2ZipAvail
         , ref string message, ref string status, ref string savedOnlyitemIds
         , ref string savedItemIdsWithLink2, ref string savedItemGuids
         , ref bool allSuccesfulRecords, string currentCulture, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom
         )
        {
            {
                long SessionUserId = SessionHelper.UserID;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);
                var itemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                var BinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

                List<BOMItemMasterMain> CurrentBlankItemList = new List<BOMItemMasterMain>();
                BOMItemMasterMain[] LstItemMaster = serializer.Deserialize<BOMItemMasterMain[]>(para);

                if (LstItemMaster != null && LstItemMaster.Length > 0)
                {

                    CurrentItemList = new List<BOMItemMasterMain>();
                    lst = objImportDAL.GetAllUDFList(RoomID, ImportMastersDTO.TableName.ItemMaster.ToString());

                    eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<UDFDTO> UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("ItemMaster", RoomID, SessionHelper.CompanyID);
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    eTurns.DAL.RoomDAL objRoomDAL = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,IsAllowOrderCostuom,ReplenishmentType,GlobMarkupParts,GlobMarkupLabor";
                    RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

                    if (objRoomDTO == null || objRoomDTO.ID <= 0)
                    {
                        objRoomDTO = new RoomDTO();
                    }

                    DashboardParameterDTO objDashboardParameterDTO = new DashboardDAL(SessionHelper.EnterPriseDBName).GetDashboardParameters(RoomID, SessionHelper.CompanyID);

                    if (objDashboardParameterDTO == null || objDashboardParameterDTO.ID <= 0)
                    {
                        objDashboardParameterDTO = new DashboardParameterDTO();
                    }

                    bool IsAllowOrderCostuomRoom = true;
                    if (objRoomDTO.IsAllowOrderCostuom == false)
                    {
                        IsAllowOrderCostuomRoom = false;
                    }
                    bool CurrentRoomConsigned = objRoomDAL.checkRoomConsignedOrNotbyRoomId(RoomID);

                    CurrentOptionList = new List<UDFOptionsMain>();
                    bool SaveToolList = true;
                    List<CostUOMMasterDTO> lstCostUOMMasterDTO = new List<CostUOMMasterDTO>();
                    lstCostUOMMasterDTO = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName).GetCostUOMsByRoomID(RoomID, SessionHelper.CompanyID).ToList();
                    List<ItemMasterDTO> lstItemMaster = itemMasterDAL.GetAllItemsWithJoins(RoomID, SessionHelper.CompanyID, false, false, string.Empty).ToList();
                    foreach (BOMItemMasterMain item in LstItemMaster)
                    {
                        SaveToolList = true;

                        BOMItemMasterMain objDTO = new BOMItemMasterMain();
                        objDTO.ID = item.ID;
                        objDTO.ItemNumber = (item.ItemNumber == null) ? null : (item.ItemNumber.Length > 255 ? item.ItemNumber.Substring(0, 255) : item.ItemNumber);
                        objDTO.ManufacturerName = item.ManufacturerName;
                        //Wi-1505
                        if (!string.IsNullOrWhiteSpace(item.ManufacturerName))
                        {
                            objDTO.ManufacturerID = item.ManufacturerName == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.ManufacturerMaster, item.ManufacturerName, RoomID: RoomID);
                        }
                        else
                        {
                            long ManuID = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName).GetORInsertBlankManuFacID(RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                            if (!string.IsNullOrWhiteSpace(item.ManufacturerNumber))
                            {
                                objDTO.ManufacturerID = ManuID;
                            }

                        }
                        if (!CurrentRoomConsigned)
                        {
                            item.Consignment = false;
                        }
                        objDTO.ManufacturerNumber = (item.ManufacturerNumber == null) ? null : (item.ManufacturerNumber.Length > 50 ? item.ManufacturerNumber.Substring(0, 50) : item.ManufacturerNumber);
                        objDTO.Link1 = item.Link1;
                        objDTO.Link2 = item.Link2;
                        if (IsAllowOrderCostuomRoom == false)
                        {
                            objDTO.IsAllowOrderCostuom = false;
                        }
                        else
                        {
                            objDTO.IsAllowOrderCostuom = item.IsAllowOrderCostuom;
                        }
                        ItemMasterDTO objItem = lstItemMaster.Where(t => t.ItemNumber == objDTO.ItemNumber).FirstOrDefault();
                        if (objItem != null)
                        {
                            if (((objItem.OnOrderQuantity ?? 0) > 0 || ((objItem.OnOrderInTransitQuantity ?? 0) > 0)) && (objItem.IsAllowOrderCostuom) != objDTO.IsAllowOrderCostuom)
                            {
                                SaveToolList = false;
                                objDTO.Status = "Fail";
                                objDTO.Reason = string.Format(ResItemMaster.CostUOMOnOrderInTransitQuantity, ResItemMaster.OnOrderQuantity, objDTO.ItemNumber);
                                item.Status = "Fail";
                                item.Reason = string.Format(ResItemMaster.CostUOMOnOrderInTransitQuantity, ResItemMaster.OnOrderQuantity, objDTO.ItemNumber);
                            }
                        }
                        objDTO.ItemLink2ImageType = "InternalLink";
                        objDTO.ImageType = "ExternalImage";
                        objDTO.ItemImageExternalURL = item.ItemImageExternalURL;
                        objDTO.ItemLink2ExternalURL = item.ItemLink2ExternalURL;

                        //if (!string.IsNullOrWhiteSpace(objDTO.ItemImageExternalURL))
                        //{
                        //    bool Isvalid = CommonUtility.ValidateExternalImage(objDTO.ItemImageExternalURL);
                        //    if (!Isvalid)
                        //    {
                        //        SaveToolList = false;
                        //        objDTO.Status = "Fail";
                        //        objDTO.Reason = string.Format(ResItemMaster.InvalidImageURL, "ItemImageExternalURL");
                        //        item.Status = "Fail";
                        //        item.Reason = string.Format(ResItemMaster.InvalidImageURL, "ItemImageExternalURL");
                        //    }
                        //}
                        if (string.IsNullOrWhiteSpace(item.ItemImageExternalURL) && (!string.IsNullOrEmpty(item.ImagePath)))
                        {
                            objDTO.ImageType = "ImagePath";
                        }
                        if (string.IsNullOrWhiteSpace(item.Link2) && (!string.IsNullOrEmpty(item.ItemLink2ExternalURL)))
                        {
                            objDTO.ItemLink2ImageType = "ExternalURL";
                        }
                        objDTO.ItemDocExternalURL = item.ItemDocExternalURL;
                        objDTO.IsActive = item.IsActive;
                        objDTO.IsOrderable = item.IsOrderable;
                        if (item.ItemTypeName == "Labor")
                        {
                            objDTO.SupplierName = string.Empty;
                            objDTO.SupplierPartNo = null;
                            objDTO.SupplierID = null;
                            objDTO.BlanketOrderNumber = string.Empty;
                            objDTO.BlanketPOID = null;
                        }
                        else
                        {
                            objDTO.SupplierName = item.SupplierName;
                            objDTO.SupplierPartNo = (item.SupplierPartNo == null) ? null : (item.SupplierPartNo.Length > 50 ? item.SupplierPartNo.Substring(0, 50) : item.SupplierPartNo);
                            objDTO.SupplierID = GetIDs(ImportMastersDTO.TableName.SupplierMaster, item.SupplierName, RoomID: RoomID);

                            // Start -- insert into Room Schedule for PullSchedule type is Immediate 
                            if (objDTO.SupplierID > 0)
                            {
                                SchedulerDTO objSchedulerDTO = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetRoomSchedule(objDTO.SupplierID.GetValueOrDefault(0), RoomID, 7);
                                if (objSchedulerDTO == null)
                                {
                                    // insert into Room Schedule for PullSchedule type is Immediate 
                                    SchedulerDTO objPullSchedulerDTO = new SchedulerDTO();
                                    objPullSchedulerDTO.SupplierId = objDTO.SupplierID.GetValueOrDefault(0);
                                    objPullSchedulerDTO.CompanyId = SessionHelper.CompanyID;
                                    objPullSchedulerDTO.RoomId = RoomID;
                                    objPullSchedulerDTO.LoadSheduleFor = 7;
                                    objPullSchedulerDTO.ScheduleMode = 5;
                                    objPullSchedulerDTO.IsScheduleActive = true;
                                    objPullSchedulerDTO.MonthlyDayOfMonth = 2;
                                    new SupplierMasterDAL(SessionHelper.EnterPriseDBName).SaveSupplierSchedule(objPullSchedulerDTO);
                                }
                            }

                            /// End- logic for insert into Room Schedule for PullSchedule type is Immediate

                            objDTO.BlanketOrderNumber = item.BlanketOrderNumber;
                            if (objDTO.SupplierID.HasValue && objDTO.SupplierID.Value > 0 && !string.IsNullOrWhiteSpace(item.BlanketOrderNumber))
                                objDTO.BlanketPOID = GetIDs(ImportMastersDTO.TableName.SupplierBlanketPODetails, item.BlanketOrderNumber, objDTO.SupplierID.Value, RoomID: RoomID);
                        }

                        objDTO.UPC = item.UPC;
                        objDTO.UNSPSC = item.UNSPSC;
                        objDTO.Description = item.Description;
                        objDTO.LongDescription = item.LongDescription;
                        objDTO.CategoryID = item.CategoryName == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.CategoryMaster, item.CategoryName, RoomID: RoomID);
                        objDTO.GLAccountID = item.GLAccount == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.GLAccountMaster, item.GLAccount, RoomID: RoomID);
                        objDTO.CategoryName = item.CategoryName;
                        objDTO.GLAccount = item.GLAccount;
                        objDTO.UOMID = GetIDs(ImportMastersDTO.TableName.UnitMaster, item.Unit, RoomID: RoomID);
                        objDTO.Unit = item.Unit;
                        objDTO.PricePerTerm = item.PricePerTerm;
                        objDTO.CostUOMID = item.CostUOMName == "" ? (long?)null : GetIDs(ImportMastersDTO.TableName.CostUOMMaster, item.CostUOMName, RoomID: RoomID);
                        objDTO.CostUOMName = item.CostUOMName;

                        objDTO.DefaultReorderQuantity = item.DefaultReorderQuantity;
                        objDTO.DefaultPullQuantity = item.DefaultPullQuantity;


                        item.Cost = item.Cost ?? 0;
                        item.SellPrice = item.SellPrice ?? 0;
                        item.Markup = item.Markup ?? 0;

                        objDTO.Cost = item.Cost;
                        objDTO.SellPrice = item.SellPrice;
                        objDTO.Markup = item.Markup;

                        if (item.Cost != 0 && item.SellPrice != 0 && item.Markup != 0)
                        {
                            objDTO.Markup = Convert.ToDouble(((Convert.ToDecimal(objDTO.SellPrice) * Convert.ToDecimal(100)) / Convert.ToDecimal(objDTO.Cost)) - Convert.ToDecimal(100));
                            // Calculate MArkup based on price and cost
                        }
                        else if (item.Cost != 0 && item.SellPrice == 0 && item.Markup == 0)
                        {
                            // Prise will become same as cost
                            objDTO.SellPrice = objDTO.Cost;
                        }
                        else if (item.Cost != 0 && item.SellPrice != 0 && item.Markup == 0)
                        {
                            // Calculate MArkup based on price and cost
                            objDTO.Markup = Convert.ToDouble(((Convert.ToDecimal(objDTO.SellPrice) * Convert.ToDecimal(100)) / Convert.ToDecimal(objDTO.Cost)) - Convert.ToDecimal(100));
                        }
                        else if (item.Cost != 0 && item.SellPrice == 0 && item.Markup != 0)
                        {
                            //Calculate prise based on cost and markup
                            objDTO.SellPrice = Convert.ToDouble(Convert.ToDecimal(objDTO.Cost) + ((Convert.ToDecimal(objDTO.Cost) * Convert.ToDecimal(objDTO.Markup)) / Convert.ToDecimal(100)));
                        }
                        else if (item.Cost == 0 && item.SellPrice != 0 && item.Markup != 0)
                        {
                            // Calculate cost based on prise and markup                                 
                            //objDTO.Cost = objDTO.SellPrice - ((objDTO.SellPrice * objDTO.Markup) / 100);
                            objDTO.Cost = Convert.ToDouble((Convert.ToDecimal(100) * Convert.ToDecimal(item.SellPrice)) / (Convert.ToDecimal(item.Markup) + Convert.ToDecimal(100)));
                            //Please follow Markup= (((sell-cost)/cost)*100)
                        }
                        else if (item.Cost == 0 && item.SellPrice == 0 && item.Markup == 0)
                        {
                            // All are zero so no calc
                        }
                        else if (item.Cost == 0 && item.SellPrice != 0 && item.Markup == 0)
                        {
                            // cost will become same as prise
                            objDTO.Cost = objDTO.SellPrice;
                        }
                        else if (item.Cost == 0 && item.SellPrice == 0 && item.Markup != 0)
                        {
                            bool isSetMarkupZero = true;
                            if (objRoomDTO != null)
                            {
                                if (item.ItemType == 4)
                                {
                                    if (objRoomDTO.GlobMarkupLabor > 0)
                                    {
                                        isSetMarkupZero = false;
                                    }
                                }
                                else if (objRoomDTO.GlobMarkupParts > 0)
                                {
                                    isSetMarkupZero = false;
                                }
                            }
                            if (isSetMarkupZero == false)
                            {
                                objDTO.Markup = item.Markup;
                            }
                            else
                                objDTO.Markup = 0;
                            // Cost and prise will remain zero and save markup only Or make markup zero because no prise and cost
                        }

                        objDTO.ExtendedCost = 0;// item.ExtendedCost;
                        objDTO.DispExtendedCost = item.ExtendedCost;

                        objDTO.LeadTimeInDays = item.LeadTimeInDays;
                        objDTO.Trend = item.Trend;
                        objDTO.Taxable = item.Taxable;
                        objDTO.Consignment = item.Consignment;
                        objDTO.StagedQuantity = 0;// item.StagedQuantity;
                        objDTO.InTransitquantity = 0;// item.InTransitquantity;
                        objDTO.OnOrderQuantity = 0;// item.OnOrderQuantity;
                        objDTO.OnTransferQuantity = 0;// item.OnTransferQuantity;
                        objDTO.SuggestedOrderQuantity = 0;// item.SuggestedOrderQuantity;
                        objDTO.RequisitionedQuantity = 0;// item.RequisitionedQuantity;
                        objDTO.AverageUsage = 0;// item.AverageUsage;
                        objDTO.Turns = 0;// item.Turns;
                        if (item.ItemType != 4)
                        {
                            objDTO.OnHandQuantity = item.OnHandQuantity;
                        }

                        objDTO.DispStagedQuantity = item.StagedQuantity;
                        objDTO.DispInTransitquantity = item.InTransitquantity;
                        objDTO.DispOnOrderQuantity = item.OnOrderQuantity;
                        objDTO.DispOnTransferQuantity = item.OnTransferQuantity;
                        objDTO.DispSuggestedOrderQuantity = item.SuggestedOrderQuantity;
                        objDTO.DispRequisitionedQuantity = item.RequisitionedQuantity;
                        objDTO.DispAverageUsage = item.AverageUsage;
                        objDTO.DispTurns = item.Turns;

                        objDTO.IsPackslipMandatoryAtReceive = item.IsPackslipMandatoryAtReceive;
                        if (!string.IsNullOrWhiteSpace(objRoomDTO.ReplenishmentType))
                        {
                            if (objRoomDTO.ReplenishmentType == "1")
                            {
                                objDTO.IsItemLevelMinMaxQtyRequired = true;
                            }
                            else if (objRoomDTO.ReplenishmentType == "2")
                            {
                                objDTO.IsItemLevelMinMaxQtyRequired = false;
                            }
                            else if (objRoomDTO.ReplenishmentType == "3")
                            {
                                objDTO.IsItemLevelMinMaxQtyRequired = item.IsItemLevelMinMaxQtyRequired;
                            }
                        }
                        objDTO.CriticalQuantity = item.CriticalQuantity;
                        objDTO.MinimumQuantity = item.MinimumQuantity;
                        objDTO.MaximumQuantity = item.MaximumQuantity;
                        objDTO.WeightPerPiece = item.WeightPerPiece;
                        if (objDTO.IsAllowOrderCostuom == true)
                        {
                            item.IsEnforceDefaultReorderQuantity = true;

                            if (lstCostUOMMasterDTO.Where(t => t.CostUOM == item.CostUOMName).Count() == 0)
                            {
                                lstCostUOMMasterDTO = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName).GetCostUOMsByRoomID(RoomID, SessionHelper.CompanyID).ToList();
                            }
                            if (lstCostUOMMasterDTO.Where(t => t.CostUOM == item.CostUOMName).Count() > 0)
                            {
                                CostUOMMasterDTO cuom = lstCostUOMMasterDTO.Where(t => t.CostUOM == item.CostUOMName).FirstOrDefault();
                                bool value = objCommonDAL.CheckItemCostUOM(cuom.ID, Convert.ToInt64(item.DefaultReorderQuantity ?? 0));
                                if (!value)
                                {
                                    objDTO.DefaultReorderQuantity = (cuom.CostUOMValue ?? 0);
                                }
                                else
                                {
                                    Double Remainder = (item.DefaultReorderQuantity ?? 0) % (cuom.CostUOMValue ?? 0);
                                    if (Remainder != 0)
                                    {
                                        objDTO.DefaultReorderQuantity = (cuom.CostUOMValue ?? 0);

                                    }
                                }


                            }
                        }


                        objDTO.ItemUniqueNumber = objCommonDAL.GetItemUniqueIdByRoom(item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));

                        objDTO.IsTransfer = item.IsTransfer;
                        objDTO.IsPurchase = item.IsPurchase;
                        objDTO.InventryLocation = item.InventryLocation;
                        objDTO.InventoryClassification = Convert.ToInt32(GetIDs(ImportMastersDTO.TableName.InventoryClassificationMaster, item.InventoryClassificationName, RoomID: RoomID));
                        objDTO.InventoryClassificationName = item.InventoryClassificationName;
                        objDTO.ItemTypeName = item.ItemTypeName;
                        objDTO.SerialNumberTracking = item.SerialNumberTracking;
                        objDTO.LotNumberTracking = item.LotNumberTracking;
                        objDTO.DateCodeTracking = item.DateCodeTracking;
                        objDTO.ItemType = item.ItemTypeName == "Item" ? 1 : item.ItemTypeName == "Quick List" ? 2 : item.ItemTypeName == "Kit" ? 3 : item.ItemTypeName == "Labor" ? 4 : 1;
                        objDTO.ImagePath = item.ImagePath;
                        objDTO.OnHandQuantity = (!objDTO.SerialNumberTracking && !objDTO.LotNumberTracking && !objDTO.DateCodeTracking) ? objDTO.OnHandQuantity : 0;
                        objDTO.UDF1 = (item.UDF1 == null) ? null : (item.UDF1.Length > 255 ? item.UDF1.Substring(0, 255) : item.UDF1);
                        objDTO.UDF2 = (item.UDF2 == null) ? null : (item.UDF2.Length > 255 ? item.UDF2.Substring(0, 255) : item.UDF2);
                        objDTO.UDF3 = (item.UDF3 == null) ? null : (item.UDF3.Length > 255 ? item.UDF3.Substring(0, 255) : item.UDF3);
                        objDTO.UDF4 = (item.UDF4 == null) ? null : (item.UDF4.Length > 255 ? item.UDF4.Substring(0, 255) : item.UDF4);
                        objDTO.UDF5 = (item.UDF5 == null) ? null : (item.UDF5.Length > 255 ? item.UDF5.Substring(0, 255) : item.UDF5);

                        objDTO.UDF6 = (item.UDF6 == null) ? null : (item.UDF6.Length > 255 ? item.UDF6.Substring(0, 255) : item.UDF6);
                        objDTO.UDF7 = (item.UDF7 == null) ? null : (item.UDF7.Length > 255 ? item.UDF7.Substring(0, 255) : item.UDF7);
                        objDTO.UDF8 = (item.UDF8 == null) ? null : (item.UDF8.Length > 255 ? item.UDF8.Substring(0, 255) : item.UDF8);
                        objDTO.UDF9 = (item.UDF9 == null) ? null : (item.UDF9.Length > 255 ? item.UDF9.Substring(0, 255) : item.UDF9);
                        objDTO.UDF10 = (item.UDF10 == null) ? null : (item.UDF10.Length > 255 ? item.UDF10.Substring(0, 255) : item.UDF10);

                        objDTO.CategoryColor = "";
                        objDTO.IsLotSerialExpiryCost = item.IsLotSerialExpiryCost;
                        if (objDTO.ItemType == 3)
                        {
                            objDTO.IsBuildBreak = item.IsBuildBreak;
                        }
                        objDTO.IsAutoInventoryClassification = item.IsAutoInventoryClassification;
                        objDTO.PullQtyScanOverride = item.PullQtyScanOverride;
                        objDTO.IsEnforceDefaultReorderQuantity = item.IsEnforceDefaultReorderQuantity ?? false;
                        if (objDashboardParameterDTO.IsTrendingEnabled)
                        {
                            objDTO.TrendingSetting = getTrendingID(item.TrendingSettingName);
                            objDTO.TrendingSettingName = item.TrendingSettingName;
                        }
                        else
                        {
                            item.TrendingSetting = 0;
                            item.TrendingSettingName = "None";
                            objDTO.TrendingSetting = 0;
                            objDTO.TrendingSettingName = "None";
                        }

                        objDTO.IsDeleted = item.IsDeleted ?? false;
                        objDTO.IsArchived = false;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Room = RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.WhatWhereAction = "Import";
                        if (objDTO.ItemType == 4 && (objDTO.DefaultPullQuantity == null || objDTO.DefaultPullQuantity <= 0))
                        {
                            objDTO.DefaultPullQuantity = 1;
                        }
                        objDTO.eLabelKey = (item.eLabelKey == null ? null : item.eLabelKey);
                        objDTO.EnrichedProductData = (item.EnrichedProductData == null ? null : item.EnrichedProductData);
                        objDTO.EnhancedDescription = (item.EnhancedDescription == null ? null : item.EnhancedDescription);
                        lstItemGUID.Add(objDTO.GUID);

                        if (!string.IsNullOrWhiteSpace((item.SupplierPartNo ?? string.Empty).Trim()) || ((!string.IsNullOrWhiteSpace(item.ItemTypeName)) && item.ItemTypeName == "Labor"))
                        {
                            //ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                            //bool Result = objItemSupplierDetailsDAL.CheckSupplierDuplicateByItemNumber(item.SupplierPartNo.Trim(), SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.ItemNumber, false);
                            //if (!Result)
                            //{
                            //    SaveToolList = false;
                            //    objDTO.Status = "Fail";
                            //    objDTO.Reason = ResMessage.DuplicateSupPartNo;
                            //    item.Status = "Fail";
                            //    item.Reason = ResMessage.DuplicateSupPartNo;
                            //}
                        }
                        else
                        {
                            SaveToolList = false;
                            objDTO.Status = "Fail";
                            objDTO.Reason = string.Format(ResMessage.Required, "SupplierPartNo");
                            item.Status = "Fail";
                            item.Reason = string.Format(ResMessage.Required, "SupplierPartNo"); ;
                        }



                        /*/////CODE FOR CHECK UDF IS REQUIRED///////*/
                        string errorMsg = string.Empty;
                        CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, out errorMsg, SessionHelper.EnterPriceID, SessionHelper.CompanyID, RoomID, currentCulture, null, item.UDF6, item.UDF7, item.UDF8, item.UDF9, item.UDF10);
                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            item.Status = "Fail";
                            if (!string.IsNullOrEmpty(item.Reason))
                                item.Reason += errorMsg;
                            else
                                item.Reason = errorMsg;
                            SaveToolList = false;
                        }
                        /*/////CODE FOR CHECK UDF IS REQUIRED///////*/

                        string errors = ValidateModuleDTO<BOMItemMasterMain, ItemMasterDTO>(objDTO, "Inventory", "ItemCreate", new List<string>() { "ID" });

                        if (string.IsNullOrWhiteSpace(errors) == false)
                        {
                            SaveToolList = false;
                            objDTO.Status = "Fail";
                            objDTO.Reason += errors;
                            item.Status = "Fail";
                            item.Reason += errors;
                        }

                        if (item.ItemNumber.Trim() != "" && SaveToolList)
                        {
                            var itemval = CurrentItemList.FirstOrDefault(x => x.ItemNumber.ToLower() == item.ItemNumber.ToLower());
                            if (itemval != null)
                                CurrentItemList.Remove(itemval);
                            CurrentItemList.Add(objDTO);
                            //CurrentItemDTOList.Add(objDTO);

                            item.Status = "Success";
                            item.Reason = "N/A";

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF1 == null ? "" : objDTO.UDF1, CommonUtility.ImportItemColumn.UDF1.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF2 == null ? "" : objDTO.UDF2, CommonUtility.ImportItemColumn.UDF2.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF3 == null ? "" : objDTO.UDF3, CommonUtility.ImportItemColumn.UDF3.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF4 == null ? "" : objDTO.UDF4, CommonUtility.ImportItemColumn.UDF4.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF5 == null ? "" : objDTO.UDF5, CommonUtility.ImportItemColumn.UDF5.ToString(), RoomID);

                            CheckUDF(lst, CurrentOptionList, objDTO.UDF6 == null ? "" : objDTO.UDF6, CommonUtility.ImportItemColumn.UDF6.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF7 == null ? "" : objDTO.UDF7, CommonUtility.ImportItemColumn.UDF7.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF8 == null ? "" : objDTO.UDF8, CommonUtility.ImportItemColumn.UDF8.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF9 == null ? "" : objDTO.UDF9, CommonUtility.ImportItemColumn.UDF9.ToString(), RoomID);
                            CheckUDF(lst, CurrentOptionList, objDTO.UDF10 == null ? "" : objDTO.UDF10, CommonUtility.ImportItemColumn.UDF10.ToString(), RoomID);

                        }
                        else
                            CurrentBlankItemList.Add(item);
                    }

                    List<BOMItemMasterMain> lstreturn = new List<BOMItemMasterMain>();


                    if (CurrentItemList.Count > 0)
                    {
                        HttpContext.Current.Session["SaveBinCreatedDate"] = itemMasterDAL.GetDBUTCTime();// DateTimeUtility.DateTimeNow.AddMinutes(-1); //Note: difference in db utc time and c# utc time was causing issue in display bin change history warning that's why DB utc time is used at both the end.
                        lstreturn = objImportDAL.BulkInsert(ImportMastersDTO.TableName.EditItemMaster.ToString(), CurrentItemList, RoomID, SessionHelper.CompanyID, HttpContext.Current.Session["ColuumnList"].ToString(), SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, CurrentOptionList, isImgZipAvail, isLink2ZipAvail);
                    }

                    if (CurrentBlankItemList.Count > 0)
                    {
                        foreach (BOMItemMasterMain item in CurrentBlankItemList)
                        {
                            lstreturn.Add(item);
                        }
                    }

                    if (lstreturn.Count == 0)
                    {
                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        this.ClearCurrentResourceList();
                        if (HasMoreRecords == false)
                        {
                            HttpContext.Current.Session["importedData"] = null;
                        }
                    }
                    else
                    {
                        savedOnlyitemIds = string.Join(",", lstreturn.Where(i => i.ID != 0 && (!string.IsNullOrEmpty(i.ImagePath))).Select(p => p.ID.ToString() + "#" + p.ImagePath.ToString()));
                        savedItemIdsWithLink2 = string.Join(",", lstreturn.Where(i => i.ID != 0 && (!string.IsNullOrEmpty(i.Link2))).Select(p => p.ID.ToString() + "#" + p.Link2.ToString()));
                        savedItemGuids = string.Join(",", lstreturn.Where(i => i.GUID != Guid.Empty).Select(p => p.GUID.ToString()));
                        //objDTO.Link2 = item.Link2;
                        //objDTO.ItemLink2ImageType = "InternalLink";

                        foreach (BOMItemMasterMain b in lstreturn.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.ImagePath))))
                        {
                            //long id = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetIDByItemNumber(Convert.ToString(b.ItemNumber), Convert.ToInt64(b.Room), Convert.ToInt64(b.CompanyID));
                            ItemMasterDTO objItemMaster = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByItemNumberPlain(Convert.ToString(b.ItemNumber), Convert.ToInt64(b.Room), Convert.ToInt64(b.CompanyID));
                            if (objItemMaster != null)
                            {
                                if (string.IsNullOrEmpty(savedOnlyitemIds))
                                {
                                    savedOnlyitemIds = objItemMaster.ID + "#" + objItemMaster.ImagePath.ToString();
                                }
                                else
                                {
                                    savedOnlyitemIds += "," + objItemMaster.ID + "#" + objItemMaster.ImagePath.ToString();
                                }
                            }
                        }

                        foreach (BOMItemMasterMain b in lstreturn.Where(i => i.ID == 0 && (!string.IsNullOrEmpty(i.Link2))))
                        {
                            ItemMasterDTO objItemMaster = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByItemNumberPlain(Convert.ToString(b.ItemNumber), Convert.ToInt64(b.Room), Convert.ToInt64(b.CompanyID));
                            if (objItemMaster != null)
                            {
                                if (string.IsNullOrEmpty(savedItemIdsWithLink2))
                                {
                                    savedItemIdsWithLink2 = objItemMaster.ID + "#" + objItemMaster.Link2.ToString();
                                }
                                else
                                {
                                    savedItemIdsWithLink2 += "," + objItemMaster.ID + "#" + objItemMaster.Link2.ToString();
                                }
                            }
                        }

                        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
                        status = ResMessage.SaveMessage;
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in lstreturn)
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason + " " + RoomName;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason + " " + RoomName);
                                }
                            }
                        }
                        #endregion

                        if (isLastRoom && ErrorRows.Count() > 0)
                        {
                            int SrNo = 0;
                            foreach (var item in lstreturn)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                            }
                            allSuccesfulRecords = false;
                        }
                        if (isLastRoom)
                            SaveImportDataListSession<BOMItemMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        //Session["importedData"] = lstreturn;
                    }
                    //if (lstreturn.Count() != lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").Count())
                    //{
                    //    allSuccesfulRecords = false;
                    //}
                    CostUOMMasterDAL objCostUOMDAL = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);

                    if (lstreturn.Count() > 0 && lstreturn.Where(l => l.Status != null && l.Status.ToLower() == "success").ToList().Count() > 0)
                    {
                        foreach (var item in lstreturn)
                        {
                            if (item.SupplierPartNo != String.Empty && item.SupplierPartNo != null)
                            {
                                //string costUOMValue = objCostUOMDAL.GetCostUOMByID(item.CostUOMID.GetValueOrDefault(0)).CostUOMValue.ToString();
                                string DefaultLocationName = BinMasterDAL.GetBinByID(Int64.Parse(Convert.ToString(item.DefaultLocation)), RoomID, SessionHelper.CompanyID).BinNumber;
                                itemMasterDAL.AddUpdateSolumnProduct(item.SupplierPartNo, item.ItemNumber, item.GUID, item.Description, item.MinimumQuantity.ToString(), item.MaximumQuantity.ToString(), item.DefaultReorderQuantity, item.CostUOMID, DefaultLocationName, item.OnOrderQuantity, string.Empty, SessionHelper.EnterPriceID, SessionHelper.CompanyID, RoomID,item.eLabelKey);
                            }
                        }
                    }

                    lstItemGUID = new List<Guid>();
                    foreach (BOMItemMasterMain objitemguid in lstreturn.Where(t => t.IsDeleted == false))
                    {
                        if (objitemguid.Status.ToLower() == "success")
                        {
                            lstItemGUID.Add(objitemguid.GUID);
                        }
                    }

                }

            }
        }

        public void SaveBOMItemTOItemMaster(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList, List<Guid> lstItemGUID,
        List<BOMItemMasterMain> CurrentItemList,
        ImportMastersDTO.TableName TableName, string para, bool HasMoreRecords,
        bool IsFirstCall, bool isImgZipAvail, bool isLink2ZipAvail
        , ref string message, ref string status, ref string savedOnlyitemIds
        , ref string savedItemIdsWithLink2, ref string savedItemGuids
        , ref bool allSuccesfulRecords, string currentCulture, long RoomID, string RoomName, Dictionary<int, string> ErrorRows, bool isLastRoom
        )
        {
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;

                List<BOMItemMasterMain> CurrentBlankItemList = new List<BOMItemMasterMain>();
                BOMItemMasterMain[] LstItemMaster = serializer.Deserialize<BOMItemMasterMain[]>(para);
                BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                CompanyMasterDAL objCompanyDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                ItemMasterDAL itemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);
                ItemSupplierDetailsDAL itemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                List<BOMItemMasterMain> lstreturn = new List<BOMItemMasterMain>();
                List<string> lstRooms = LstItemMaster.Select(x => x.RoomName).Distinct().ToList();
                bool IsAtleatOneValidRoom = false;
                foreach (string room in lstRooms)
                {

                    int TotalRecordCountRM = 0;
                    TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                    List<BOMItemDTO> lstBOMItems = new List<BOMItemDTO>();
                    RoomDTO roomFound = null;
                    List<BOMItemMasterMain> lstBOMItemsExcel = new List<BOMItemMasterMain>();
                    List<CompanyMasterDTO> companyList = new List<CompanyMasterDTO>();
                    lstBOMItemsExcel = LstItemMaster.Where(x => x.RoomName == room).ToList();
                    if (!string.IsNullOrEmpty(room))
                    {
                        companyList = objCompanyDAL.GetCompanyListByEnterpriseANDRoom(SessionHelper.EnterPriceID, room);
                        if (companyList != null && companyList.Count > 0)
                        {
                            roomFound = objRoomDAL.GetRoomByNamePlain(companyList[0].CompanyID, room);
                        }
                    }
                    if (roomFound != null)
                    {
                        lstBOMItems = objBOMItemMasterDAL.GetPagedRecords(0, int.MaxValue, out TotalRecordCountRM, string.Empty, string.Empty, companyList[0].CompanyID, false, false, roomFound.ID, "popup", string.Empty, CurrentTimeZone);
                    }
                    else
                    {
                        foreach (var item in lstBOMItemsExcel)
                        {
                            var excelItem = LstItemMaster.Where(x => x.ItemNumber == item.ItemNumber && x.RoomName == item.RoomName).FirstOrDefault();
                            if (excelItem != null)
                            {
                                excelItem.Status = "fail";
                                excelItem.Reason = ResCommon.NoRooms;
                            }
                        }
                        continue;
                    }
                    if (lstBOMItemsExcel != null && lstBOMItemsExcel.Count() > 0)
                    {
                        IsAtleatOneValidRoom = true;
                        foreach (var item in lstBOMItemsExcel)
                        {
                            try
                            {
                                item.IsBlankConsignment = item.ISNullConsignment;
                                if (item.ItemNumber == null || item.ItemNumber == string.Empty)
                                {
                                    item.Status = "Fail";
                                    item.Reason = ResItemMaster.MsgItemNumberRequired;
                                    continue;
                                }
                                else if (item.RoomName == null || item.RoomName == string.Empty)
                                {
                                    item.Status = "Fail";
                                    item.Reason = ResRoomMaster.MsgRoomNameRequired;
                                    continue;
                                }
                                else if (item.InventryLocation == null || item.InventryLocation == string.Empty)
                                {
                                    item.Status = "Fail";
                                    item.Reason = ResItemMaster.EnterInventoryLocation;
                                    continue;
                                }
                                if (companyList.Count > 1)
                                {
                                    item.Status = "Fail";
                                    item.Reason = "Room name is associated with multiple company.";
                                    continue;
                                }
                                else
                                {
                                    if (roomFound != null)
                                    {
                                        //var lstBOMItems = objBOMItemMasterDAL.GetPagedRecords(0, int.MaxValue, out TotalRecordCountRM, string.Empty, string.Empty, companyList[0].CompanyID, false, false, roomFound.ID, "popup", string.Empty, CurrentTimeZone);
                                        var itemFound = lstBOMItems.FindAll(x => x.ItemNumber == item.ItemNumber).ToList().FirstOrDefault();
                                        var itemEdit = itemMasterDAL.GetItemByItemNumberNormal(item.ItemNumber, roomFound.ID, companyList[0].CompanyID);

                                        if (itemFound != null && roomFound != null && itemEdit == null)
                                        {
                                            if (item.Status != "Fail")
                                            {
                                                objBOMItemMasterDAL.AddItemsToRoomWithoutAutoSot(itemFound.ID.ToString(), roomFound.ID,item, SessionHelper.UserID);
                                                if (roomFound.IsELabel)
                                                {
                                                    try
                                                    {
                                                        var newlyitemEditDTO = itemMasterDAL.GetItemByItemNumberNormal(item.ItemNumber, roomFound.ID, companyList[0].CompanyID);
                                                        if (newlyitemEditDTO != null)
                                                        {
                                                            BomToItemAddUpdateSolumnProduct(newlyitemEditDTO);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        item.Status = "fail";
                                                        item.Reason = ex.StackTrace;
                                                    }
                                                }
                                                

                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(itemFound.ImagePath) || !string.IsNullOrEmpty(itemFound.Link2))
                                                    {
                                                        CommonUtility.SaveBOMImageToRoomItem(itemFound.ID.ToString(), "both");
                                                    }
                                                }
                                                catch (Exception ex) { }
                                            }
                                        }
                                        else if (itemEdit != null)
                                        {
                                            EditBOMItem(itemFound, roomFound, item, companyList[0].CompanyID);
                                            try
                                            {
                                                if (!string.IsNullOrEmpty(itemEdit.ImagePath) || !string.IsNullOrEmpty(itemEdit.Link2))
                                                {
                                                    CommonUtility.SaveBOMImageToRoomItem(item.ID.ToString(), "both");
                                                }
                                            }
                                            catch (Exception ex) { }
                                        }
                                        else
                                        {
                                            item.Status = "fail";
                                            item.Reason = "Item not available on BOM Items or Item already available on Room.";
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                item.Status = "fail";
                                item.Reason = ex.StackTrace;
                            }

                            var excelItem = LstItemMaster.Where(x => x.ItemNumber == item.ItemNumber && x.RoomName == item.RoomName).FirstOrDefault();
                            if (excelItem != null)
                            {
                                excelItem.Status = item.Status;
                                excelItem.Reason = item.Reason;
                            }
                        }
                        #region track errors if any - Row Wise For Every Room
                        int RowNumber = 0;
                        foreach (var item in LstItemMaster.Where(x => x.RoomName == room).ToList())
                        {
                            RowNumber++;
                            if (item.Status != "" && item.Status.ToLower() == "fail")
                            {
                                if (ErrorRows.ContainsKey(RowNumber))
                                {
                                    item.Reason = ErrorRows[RowNumber] + "," + item.Reason;
                                    ErrorRows[RowNumber] = item.Reason;
                                }
                                else
                                {
                                    ErrorRows.Add(RowNumber, item.Reason);
                                }
                            }
                        }

                        if (isLastRoom)
                        {
                            int SrNo = 0;
                            foreach (var item in LstItemMaster)
                            {
                                SrNo++;
                                if (ErrorRows.ContainsKey(SrNo))
                                {
                                    item.Reason = ErrorRows[SrNo];
                                    item.Status = "fail";
                                }
                                lstreturn.Add(item);
                            }
                            allSuccesfulRecords = false;
                        }

                        if (isLastRoom)
                            SaveImportDataListSession<BOMItemMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                        #endregion


                    }
                }//room foreach
                if (IsAtleatOneValidRoom == false)
                {
                    if (isLastRoom)
                    {
                        int SrNo = 0;
                        foreach (var item in LstItemMaster)
                        {
                            SrNo++;
                            if (item.Status != null && item.Status == "fail")
                            {
                                lstreturn.Add(item);
                                allSuccesfulRecords = false;
                            }
                            else
                            {
                                allSuccesfulRecords = true;
                            }
                            
                        }
                        
                    }

                    if (isLastRoom)
                        SaveImportDataListSession<BOMItemMasterMain>(HasMoreRecords, IsFirstCall, lstreturn);
                }
            }
        }


        public void EditBOMItem(BOMItemDTO itemFound, RoomDTO roomFound, BOMItemMasterMain item, Int64 CompanyID)
        {
            ItemMasterDAL itemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ImportDAL objImportDAL = new ImportDAL(SessionHelper.EnterPriseDBName);
            ItemSupplierDetailsDAL itemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);

            var itemEdit = itemMasterDAL.GetItemByItemNumberNormal((itemFound != null && itemFound.ItemNumber != null ? itemFound.ItemNumber : item.ItemNumber), roomFound.ID, CompanyID);

            itemEdit.DefaultReorderQuantity = item.DefaultReorderQuantity;
            itemEdit.DefaultPullQuantity = item.DefaultPullQuantity;
            itemEdit.LeadTimeInDays = item.LeadTimeInDays;
            itemEdit.Link1 = item.Link1;
            itemEdit.Link2 = item.Link2;
            itemEdit.Taxable = item.Taxable;
            if (item.ISNullConsignment)
            {
                itemEdit.Consignment = roomFound.IsConsignment;
            }
            else
            {
                itemEdit.Consignment = item.Consignment;
            }
            itemEdit.OnHandQuantity = item.OnHandQuantity;
            itemEdit.CriticalQuantity = item.CriticalQuantity;
            itemEdit.MinimumQuantity = item.MinimumQuantity;
            itemEdit.MaximumQuantity = item.MaximumQuantity;
            itemEdit.WeightPerPiece = item.WeightPerPiece;
            itemEdit.ItemUniqueNumber = item.ItemUniqueNumber;
            itemEdit.IsTransfer = item.IsTransfer;
            itemEdit.IsPurchase = item.IsPurchase;
            if ((item.IsDeleted ?? false) == false && item.InventryLocation != null && item.InventryLocation != "")
            {
                itemEdit.DefaultLocation = objImportDAL.GetBinForImportItem(item.InventryLocation, itemEdit.ItemNumber, roomFound.ID, CompanyID, SessionHelper.UserID); // GetIDs(ImportMastersDTO.TableName.BinMaster, item.InventryLocation);
            }
            itemEdit.InventryLocation = item.InventryLocation;
            itemEdit.InventoryClassification = Convert.ToInt32(GetIDs(ImportMastersDTO.TableName.InventoryClassificationMaster, item.InventoryClassificationName, RoomID: roomFound.ID));
            itemEdit.InventoryClassificationName = item.InventoryClassificationName;
            itemEdit.SerialNumberTracking = item.SerialNumberTracking;
            itemEdit.LotNumberTracking = item.LotNumberTracking;
            itemEdit.DateCodeTracking = item.DateCodeTracking;
            itemEdit.ItemType = item.ItemTypeName == "Item" ? 1 : item.ItemTypeName == "Quick List" ? 2 : item.ItemTypeName == "Kit" ? 3 : item.ItemTypeName == "Labor" ? 4 : 1;
            if (item.ItemTypeName == "Labor")
            {
                itemEdit.SupplierName = string.Empty;
                itemEdit.SupplierPartNo = null;
                itemEdit.SupplierID = null;
                itemEdit.BlanketOrderNumber = string.Empty;
            }
            else
            {
                if (itemFound != null)
                {
                    itemEdit.SupplierName = itemFound.SupplierName;
                    itemEdit.SupplierPartNo = (itemFound.SupplierPartNo == null) ? null : (itemFound.SupplierPartNo.Length > 50 ? itemFound.SupplierPartNo.Substring(0, 50) : item.SupplierPartNo);
                    itemEdit.SupplierID = GetIDs(ImportMastersDTO.TableName.SupplierMaster, itemFound.SupplierName, RoomID: roomFound.ID);
                }
                itemEdit.BlanketOrderNumber = item.BlanketOrderNumber;
                if (itemEdit.SupplierID.HasValue && itemEdit.SupplierID.Value > 0 && !string.IsNullOrWhiteSpace(item.BlanketOrderNumber))
                {
                    //Add blanket PO details if not attached
                    long BlanketPOID = GetIDs(ImportMastersDTO.TableName.SupplierBlanketPODetails, item.BlanketOrderNumber, itemEdit.SupplierID.Value, RoomID: roomFound.ID);
                    itemSupplierDetailsDAL.UpdateBlanketPOID(itemEdit.GUID, BlanketPOID, itemEdit.SupplierID.Value);
                }
            }
            itemEdit.ImagePath = item.ImagePath;
            itemEdit.UDF1 = item.UDF1;
            itemEdit.UDF2 = item.UDF2;
            itemEdit.UDF3 = item.UDF3;
            itemEdit.UDF4 = item.UDF4;
            itemEdit.UDF5 = item.UDF5;
            itemEdit.UDF6 = item.UDF6;
            itemEdit.UDF7 = item.UDF7;
            itemEdit.UDF8 = item.UDF8;
            itemEdit.UDF9 = item.UDF9;
            itemEdit.UDF10 = item.UDF10;
            itemEdit.IsLotSerialExpiryCost = item.IsLotSerialExpiryCost;
            itemEdit.IsItemLevelMinMaxQtyRequired = item.IsItemLevelMinMaxQtyRequired;
            //itemEdit.isEnforceDefaultPullQuantity = item.Enf
            if (!string.IsNullOrEmpty(item.TrendingSettingName))
            {
                itemEdit.TrendingSetting = getTrendingID(item.TrendingSettingName);
            }
            else
            {
                itemEdit.TrendingSetting = 0;
            }
            itemEdit.IsEnforceDefaultReorderQuantity = item.IsEnforceDefaultReorderQuantity;
            itemEdit.IsAutoInventoryClassification = item.IsAutoInventoryClassification;
            itemEdit.IsBuildBreak = item.IsBuildBreak;
            itemEdit.IsPackslipMandatoryAtReceive = item.IsPackslipMandatoryAtReceive;
            itemEdit.IsDeleted = item.IsDeleted;
            itemEdit.IsOrderable = item.IsActive;
            itemEdit.IsAllowOrderCostuom = item.IsAllowOrderCostuom;
            itemEdit.eLabelKey = item.eLabelKey;
            itemEdit.POItemLineNumber = item.POItemLineNumber;
            itemEdit.ImageType = "ExternalImage";
            itemEdit.ItemImageExternalURL = item.ItemImageExternalURL;
            itemEdit.ItemLink2ExternalURL = item.ItemLink2ExternalURL;
            itemEdit.ItemDocExternalURL = item.ItemDocExternalURL;
            itemEdit.PullQtyScanOverride = item.PullQtyScanOverride;
            if (string.IsNullOrWhiteSpace(item.ItemImageExternalURL) && (!string.IsNullOrEmpty(item.ImagePath)))
            {
                itemEdit.ImageType = "ImagePath";
            }
            if (string.IsNullOrWhiteSpace(item.Link2) && (!string.IsNullOrEmpty(item.ItemLink2ExternalURL)))
            {
                itemEdit.ItemLink2ImageType = "ExternalURL";
            }

            itemMasterDAL.Edit(itemEdit, SessionHelper.UserID, SessionHelper.EnterPriceID, IsAutoSOTLater: true);
            item.Status = "Success";
        }





        public static Int64 GetIDs(ImportMastersDTO.TableName TableName, string strVal, long longID = 0, long RoomID = 0)
        {
            Int64 returnID = 0;
            CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            if (string.IsNullOrEmpty(strVal)) return returnID;

            #region Get Manufacture ID
            if (TableName == ImportMastersDTO.TableName.ManufacturerMaster)
            {
                ManufacturerMasterDAL objDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.ManufacturerMaster.ToString(), "Manufacturer",RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    ManufacturerMasterDTO obj = null;
                    obj = objDAL.GetManufacturerByNameNormal(strVal.ToLower(), RoomID, SessionHelper.CompanyID, false);
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    ManufacturerMasterDTO objDTO = new ManufacturerMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Manufacturer = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion

            #region Get Supplier ID
            else if (TableName == ImportMastersDTO.TableName.SupplierMaster)
            {
                SupplierMasterDAL objDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

                //string strOK = objCDal.SupplierDuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.SupplierMaster.ToString(), "SupplierName", SessionHelper.RoomID, SessionHelper.CompanyID);
                string strOK = objDAL.SupplierDuplicateCheck(0, strVal, RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    SupplierMasterDTO obj = null;
                    obj = objDAL.GetSupplierByNamePlain(RoomID, SessionHelper.CompanyID, false, strVal);
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    SupplierMasterDTO objDTO = new SupplierMasterDTO();
                    objDTO.ID = 1;
                    objDTO.SupplierName = strVal.Length > 255 ? strVal.Trim().Substring(0, 255) : strVal.Trim();
                    objDTO.IsEmailPOInBody = false;
                    objDTO.IsEmailPOInPDF = false;
                    objDTO.IsEmailPOInCSV = false;
                    objDTO.IsEmailPOInX12 = false;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get SupplierBlanketPODetails ID
            else if (TableName == ImportMastersDTO.TableName.SupplierBlanketPODetails)
            {
                SupplierBlanketPODetailsDAL objDAL = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName);
                returnID = objDAL.SupplierBlanketPODetailsDuplicateCheck(0, strVal, longID, RoomID, SessionHelper.CompanyID);
                if (returnID == 0)
                {
                    SupplierBlanketPODetailsDTO oSupplierBlanketPODetailsDTO = new SupplierBlanketPODetailsDTO();
                    oSupplierBlanketPODetailsDTO.SupplierID = longID;
                    oSupplierBlanketPODetailsDTO.BlanketPO = strVal;
                    oSupplierBlanketPODetailsDTO.GUID = Guid.NewGuid();
                    oSupplierBlanketPODetailsDTO.Created = DateTime.Now;
                    oSupplierBlanketPODetailsDTO.CreatedBy = SessionHelper.UserID;
                    oSupplierBlanketPODetailsDTO.Updated = DateTime.Now;
                    oSupplierBlanketPODetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                    oSupplierBlanketPODetailsDTO.CompanyID = SessionHelper.CompanyID;
                    oSupplierBlanketPODetailsDTO.Room = RoomID;
                    oSupplierBlanketPODetailsDTO.IsArchived = false;
                    oSupplierBlanketPODetailsDTO.IsDeleted = false;
                    oSupplierBlanketPODetailsDTO.AddedFrom = "Web";
                    oSupplierBlanketPODetailsDTO.EditedFrom = "Web";
                    oSupplierBlanketPODetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    oSupplierBlanketPODetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName).Insert(oSupplierBlanketPODetailsDTO);
                }
            }
            #endregion
            #region Get Category ID
            else if (TableName == ImportMastersDTO.TableName.CategoryMaster)
            {
                CategoryMasterDAL objDAL = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.CategoryMaster.ToString(), "Category", RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    CategoryMasterDTO obj = null;
                    //obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(c => (c.Category ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    obj = objDAL.GetSingleCategoryByNameByRoomID(strVal, RoomID, SessionHelper.CompanyID);
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    CategoryMasterDTO objDTO = new CategoryMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Category = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get GLAccount ID
            else if (TableName == ImportMastersDTO.TableName.GLAccountMaster)
            {
                GLAccountMasterDAL objDAL = new GLAccountMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.GLAccountMaster.ToString(), "GLAccount", RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    GLAccountMasterDTO obj = null;
                    //obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(c => (c.GLAccount ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    obj = objDAL.GetGLAccountByName(RoomID, SessionHelper.CompanyID, false, strVal);
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    GLAccountMasterDTO objDTO = new GLAccountMasterDTO();
                    objDTO.ID = 1;
                    objDTO.GLAccount = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();

                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get Unit ID
            else if (TableName == ImportMastersDTO.TableName.UnitMaster)
            {
                UnitMasterDAL objDAL = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.UnitMaster.ToString(), "Unit", RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    UnitMasterDTO obj = null;
                    obj = objDAL.GetUnitByNamePlain(RoomID, SessionHelper.CompanyID, false, strVal);
                    if (obj != null && obj.ID > 0)
                        returnID = obj.ID;
                }
                else
                {
                    UnitMasterDTO objDTO = new UnitMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Unit = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion           
            #region Get LocationMaster ID
            else if (TableName == ImportMastersDTO.TableName.LocationMaster)
            {
                LocationMasterDAL objDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.LocationMaster.ToString(), "Location", RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    LocationMasterDTO obj = null;
                    obj = objDAL.GetLocationByNamePlain(strVal ?? string.Empty, RoomID, SessionHelper.CompanyID);
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    LocationMasterDTO objDTO = new LocationMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Location = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = Guid.NewGuid();

                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get ToolCategoryMaster ID
            else if (TableName == ImportMastersDTO.TableName.ToolCategoryMaster)
            {
                ToolCategoryMasterDAL objDAL = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.ToolCategoryMaster.ToString(), "ToolCategory", RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    ToolCategoryMasterDTO obj = null;
                    obj = objDAL.GetToolCategoryByNamePlain(strVal, RoomID, SessionHelper.CompanyID);
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    ToolCategoryMasterDTO objDTO = new ToolCategoryMasterDTO();
                    objDTO.ID = 1;
                    objDTO.ToolCategory = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get InventoryClassificationMaster ID
            else if (TableName == ImportMastersDTO.TableName.InventoryClassificationMaster)
            {
                InventoryClassificationMasterDAL objInventoryClassificationMasterDAL = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.InventoryClassificationMaster.ToString(), "InventoryClassification", RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    InventoryClassificationMasterDTO obj = null;
                    obj = objInventoryClassificationMasterDAL.GetInventoryClassificationByNamePlain(RoomID, SessionHelper.CompanyID, false, strVal);
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    InventoryClassificationMasterDTO objDTO = new InventoryClassificationMasterDTO();
                    objDTO.ID = 1;
                    objDTO.InventoryClassification = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;

                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    returnID = objInventoryClassificationMasterDAL.Insert(objDTO);
                }
            }
            #endregion
            #region Get CostUOM ID
            if (TableName == ImportMastersDTO.TableName.CostUOMMaster)
            {
                CostUOMMasterDAL objDAL = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.CostUOMMaster.ToString(), "CostUOM", RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    CostUOMMasterDTO obj = null;
                    //obj = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(c => (c.CostUOM ?? string.Empty).ToLower() == strVal.ToLower()).FirstOrDefault();
                    obj = objDAL.GetCostUOMByName(strVal, RoomID, SessionHelper.CompanyID);
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    CostUOMMasterDTO objDTO = new CostUOMMasterDTO();
                    objDTO.ID = 0;
                    objDTO.CostUOM = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.CostUOMValue = 1;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.Room = RoomID;
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.isForBOM = false;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion

            #region Get AssetCategoryMaster ID
            else if (TableName == ImportMastersDTO.TableName.AssetCategoryMaster)
            {
                AssetCategoryMasterDAL objDAL = new AssetCategoryMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, ImportMastersDTO.TableName.AssetCategoryMaster.ToString(), "AssetCategory", RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    AssetCategoryMasterDTO obj = null;
                    obj = objDAL.GetAssetCategoryByName(strVal, RoomID, SessionHelper.CompanyID);
                    if (obj != null)
                        returnID = obj.ID;
                }
                else
                {
                    AssetCategoryMasterDTO objDTO = new AssetCategoryMasterDTO();
                    objDTO.ID = 1;
                    objDTO.AssetCategory = strVal.Length > 128 ? strVal.Substring(0, 128) : strVal;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    returnID = objDAL.Insert(objDTO);
                }
            }
            #endregion
            return returnID;
        }

        public static byte? getTrendingID(string name)
        {
            byte id = 0;
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            else
            {
                if (name == "None")
                {
                    id = 0;
                }
                else if (name == "Manual")
                {
                    id = 1;
                }
                else if (name == "Automatic")
                {
                    id = 2;
                }
                else
                {
                    return null;
                }

            }
            return id;
        }

        public static void CheckUDF(List<UDFOptionsCheckDTO> lst, List<UDFOptionsMain> CurrentOptionList, string objDTOUDF, string UDFs,long RoomID)
        {

            if (objDTOUDF.Trim() != "")
            {
                List<UDFOptionsCheckDTO> lstcount = new List<UDFOptionsCheckDTO>();
                lstcount = lst.Where(c => c.UDFColumnName == UDFs.ToString()).ToList();
                if (lstcount.Count > 0)
                {
                    UDFOptionsCheckDTO objcheck = new UDFOptionsCheckDTO();
                    objcheck = lst.Where(c => c.UDFColumnName == UDFs.ToString() && c.UDFOption == objDTOUDF && c.UDFID == lstcount[0].UDFID).FirstOrDefault();
                    int objcheckCount = 0;
                    if (CurrentOptionList != null)
                    {
                        objcheckCount = CurrentOptionList.Where(c => c.UDFOption == objDTOUDF && c.UDFID == lstcount[0].UDFID).Count();
                        if ((objcheck == null && objcheckCount == 0))
                        {
                            UDFOptionsMain objoptionDTO = new UDFOptionsMain();
                            objoptionDTO.ID = 0;
                            objoptionDTO.Created = DateTimeUtility.DateTimeNow;
                            objoptionDTO.CreatedBy = SessionHelper.UserID;
                            objoptionDTO.Updated = DateTimeUtility.DateTimeNow;
                            objoptionDTO.LastUpdatedBy = SessionHelper.UserID;
                            objoptionDTO.IsDeleted = false;

                            objoptionDTO.UDFOption = objDTOUDF;
                            objoptionDTO.UDFID = lstcount[0].UDFID;
                            objoptionDTO.GUID = Guid.NewGuid();

                            objoptionDTO.CompanyID = SessionHelper.CompanyID;
                            objoptionDTO.Room = RoomID;

                            CurrentOptionList.Add(objoptionDTO);
                        }
                    }


                }
            }
        }

        public static void SaveImportDataListSession<t>(bool HasMoreRecords, bool IsFirstCall, object lstreturn)
        {
            try
            {

            if (HasMoreRecords == true)
            {
                List<t> lst1 = null;
                List<t> lst2 = (List<t>)lstreturn;

                if (IsFirstCall == true)
                    lst1 = new List<t>();
                else
                    lst1 = (List<t>)HttpContext.Current.Session["importedData"];

                lst1 = lst1.Union(lst2).ToList();
                foreach (var obj in lst1)
                {
                    string value = string.Empty;
                    foreach (PropertyInfo pi in ((t)obj).GetType().GetProperties().Where(x => x.PropertyType == typeof(string)))
                    {
                        value = pi.GetValue(obj, null) as string;
                        if (!string.IsNullOrEmpty(value))
                            pi.SetValue(obj, Convert.ChangeType(HttpUtility.HtmlEncode(value), pi.PropertyType), null);
                    }
                }
                
                HttpContext.Current.Session["importedData"] = lst1.ToList();
            }
            else
            {
                foreach (var obj in (List<t>)lstreturn)
                {
                    string value = string.Empty;
                    foreach (PropertyInfo pi in ((t)obj).GetType().GetProperties().Where(x => x.PropertyType == typeof(string)))
                    {
                        value = pi.GetValue(obj, null) as string;
                        if (!string.IsNullOrEmpty(value))
                            pi.SetValue(obj, Convert.ChangeType(HttpUtility.HtmlEncode(value), pi.PropertyType), null);
                    }
                }
                HttpContext.Current.Session["importedData"] = lstreturn;
            }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static bool IsValidEmail(string emailaddress)
        {
            try
            {
                if (emailaddress != "")
                {
                    MailAddress m = new MailAddress(emailaddress);
                }
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static Guid GetGUID(ImportMastersDTO.TableName TableName, string strVal, string optValue = "", QuickListType QLType = QuickListType.General,long RoomID = 0)
        {
            Guid returnID = Guid.NewGuid();
            CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            #region Get QuickList GUID
            if (TableName == ImportMastersDTO.TableName.QuickListItems)
            {
                QuickListDAL objDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDal.DuplicateCheck(strVal, "add", 1, "QuickListMaster", "Name", RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    QuickListMasterDTO obj = null;
                    //obj = objDAL.GetQuickListMasterCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(c => c.Name == strVal).FirstOrDefault();
                    obj = objDAL.GetQuickListMasterByName(strVal, RoomID, SessionHelper.CompanyID, false, false);
                    if (obj != null)
                    {
                        //obj.Type = (int)QLType;
                        objDAL.Edit(obj);
                        returnID = obj.GUID;
                    }
                }
                else
                {
                    QuickListMasterDTO objDTO = new QuickListMasterDTO();
                    objDTO.ID = 1;
                    objDTO.Name = strVal;
                    objDTO.Comment = optValue;
                    objDTO.Type = (int)QLType;
                    objDTO.IsDeleted = false;
                    objDTO.IsArchived = false;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Room = RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();

                    returnID = objDAL.InsertQuickList(objDTO);
                }
            }
            else if (TableName == ImportMastersDTO.TableName.ItemMaster)
            {
                ItemMasterDAL objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                returnID = objDAL.GetGuidByItemNumber(strVal, RoomID, SessionHelper.CompanyID) ?? Guid.Empty;
            }
            else if (TableName == ImportMastersDTO.TableName.AssetMaster)
            {
                AssetMasterDAL objDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                AssetMasterDTO objDTO = objDAL.GetAssetsByName(strVal, RoomID, SessionHelper.CompanyID).FirstOrDefault();
                if (objDTO != null)
                {
                    returnID = objDTO.GUID;
                }
            }
            #endregion
            return returnID;
        }

        public static bool CheckBinStatus(string BinNumber,long RoomID)
        {
            bool result = false;
            try
            {
                BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                List<BinMasterDTO> objBinMasterList = objBinMasterDAL.GetListBinByName(BinNumber, RoomID, SessionHelper.CompanyID, false);

                if (objBinMasterList != null)
                {
                    int deletedCount = objBinMasterList.Where(b => b.IsDeleted == true).Count();
                    int totalCount = objBinMasterList.Count();
                    if (totalCount > 0 && totalCount == deletedCount)
                    {

                        long binId = objBinMasterList.OrderByDescending(t => t.ID).Select(t => t.ID).First();
                        objBinMasterDAL.UndeleteparentLocation(binId);
                        result = true;
                    }
                    else
                    {
                        result = true;
                    }
                }
                else
                {
                    result = true;
                }
            }
            catch
            {
                return false;
            }
            return result;
        }

        public void BomToItemAddUpdateSolumnProduct(ItemMasterDTO itemMaster)
        {
            string DefaultLocationName = Convert.ToString(itemMaster.DefaultLocationName);
            if (itemMaster.DefaultLocation.GetValueOrDefault(0) > 0)
            {
                DefaultLocationName = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(Int64.Parse(Convert.ToString(itemMaster.DefaultLocation)), itemMaster.Room.GetValueOrDefault(), itemMaster.CompanyID.GetValueOrDefault()).BinNumber;
                ItemMasterDAL itemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                itemMasterDAL.AddUpdateSolumnProduct(itemMaster.SupplierPartNo, itemMaster.ItemNumber, itemMaster.GUID, itemMaster.Description, itemMaster.MinimumQuantity.ToString(), itemMaster.MaximumQuantity.ToString(), itemMaster.DefaultReorderQuantity, itemMaster.CostUOMID, DefaultLocationName, itemMaster.OnOrderQuantity, String.Empty, SessionHelper.EnterPriceID, itemMaster.CompanyID.GetValueOrDefault(0), itemMaster.Room.GetValueOrDefault(0), itemMaster.eLabelKey);
            }
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// Validate DTO using data annotation attributes
        /// </summary>
        /// <typeparam name="TImportDTO"></typeparam>
        /// <typeparam name="TModuleDTO"></typeparam>
        /// <param name="itemMain"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="ignoreProperties"> property to ignore in validation </param>
        /// <param name="popertyMap"> Mapping of property name in case name are different in TImportDTO and TModuleDTO </param>
        /// <returns></returns>
        private string ValidateModuleDTO<TImportDTO, TModuleDTO>(TImportDTO itemMain, string controller, string action, List<string> ignoreProperties, Dictionary<string, string> popertyMap = null) where TModuleDTO : class, new()
        {
            string errorMsg = "";

            try
            {
                TModuleDTO masterDTO = new TModuleDTO();
                // copy property from ImportDTO to ModuleDto. e.g. For Item Import , copy from BOMItemMasterMain to ItemMasterDTO object
                DTOCommonUtils.CopyProperty<TImportDTO, TModuleDTO>(itemMain, ref masterDTO, popertyMap);

                //DynamicModelValidatorProvider provider = new DynamicModelValidatorProvider();

                //get metadata of Model
                ModelMetadata metadataForType = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TModuleDTO));

                foreach (var p in metadataForType.Properties)
                {
                    if (ignoreProperties.Contains(p.PropertyName))
                    {
                        continue;
                    }

                    // get MetaData of property in model
                    ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(TModuleDTO), p.PropertyName);

                    ControllerContext context = new ControllerContext();

                    context.HttpContext = new HttpContextWrapper(HttpContext.Current);
                    context.RequestContext = HttpContext.Current.Request.RequestContext;

                    context.RouteData.Values["controller"] = controller;
                    context.RouteData.Values["action"] = action;


                    //var validators = provider.GetValidators(metadata,context).ToList();
                    // get validation rules of property
                    var validators = metadata.GetValidators(context).ToList();

                    // validate property for each rules
                    if (validators.Count > 0)
                    {
                        foreach (var v in validators)
                        {
                            //var valResult = v.Validate(masterDTO);

                            var valResult = v.ValidateDynamic<TModuleDTO>(masterDTO, metadata);
                            foreach (var r in valResult)
                            {
                                errorMsg += r.Message + " \n";
                            }
                        }
                    }

                }

                #region MVC InBuild Validation - Not working for Dynamic rules here
                //var validationContext = new ValidationContext(masterDTO, serviceProvider: null, items: null);

                //var validationResults = new List<ValidationResult>();

                //var isValid = Validator.TryValidateObject(masterDTO, validationContext, validationResults, true);

                //// If there any exception return them in the return result
                //if (!isValid)
                //{
                //    foreach (ValidationResult message in validationResults)
                //    {
                //        errorMsg += message.ErrorMessage;
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                return errorMsg;
            }

            return errorMsg;

        }

     

        private string ValidateModuleDTOBarcode<TImportDTO, TModuleDTO>(TImportDTO itemMain, string controller, string action, List<string> ignoreProperties, Dictionary<string, string> popertyMap = null) where TModuleDTO : class, new()
        {
            string errorMsg = "";

            try
            {
                TModuleDTO masterDTO = new TModuleDTO();
                // copy property from ImportDTO to ModuleDto. e.g. For Item Import , copy from BOMItemMasterMain to ItemMasterDTO object
                DTOCommonUtils.CopyProperty<TImportDTO, TModuleDTO>(itemMain, ref masterDTO, popertyMap);

                //DynamicModelValidatorProvider provider = new DynamicModelValidatorProvider();

                //get metadata of Model
                ModelMetadata metadataForType = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TModuleDTO));

                foreach (var p in metadataForType.Properties)
                {
                    if (ignoreProperties.Contains(p.PropertyName))
                    {
                        continue;
                    }

                    // get MetaData of property in model
                    ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(TModuleDTO), p.PropertyName);

                    ControllerContext context = new ControllerContext();

                    context.HttpContext = new HttpContextWrapper(HttpContext.Current);
                    context.RequestContext = HttpContext.Current.Request.RequestContext;

                    context.RouteData.Values["controller"] = controller;
                    context.RouteData.Values["action"] = action;


                    //var validators = provider.GetValidators(metadata,context).ToList();
                    // get validation rules of property
                    var validators = metadata.GetValidators(context).ToList();

                    // validate property for each rules
                    if (validators.Count > 0)
                    {
                        foreach (var v in validators)
                        {
                            //var valResult = v.Validate(masterDTO);

                            var valResult = v.ValidateDynamic<TModuleDTO>(masterDTO, metadata);
                            foreach (var r in valResult)
                            {
                                if (r.MemberName != "BinGuid")
                                    errorMsg += r.Message + " \n";
                            }
                        }
                    }

                }

                #region MVC InBuild Validation - Not working for Dynamic rules here
                //var validationContext = new ValidationContext(masterDTO, serviceProvider: null, items: null);

                //var validationResults = new List<ValidationResult>();

                //var isValid = Validator.TryValidateObject(masterDTO, validationContext, validationResults, true);

                //// If there any exception return them in the return result
                //if (!isValid)
                //{
                //    foreach (ValidationResult message in validationResults)
                //    {
                //        errorMsg += message.ErrorMessage;
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                return errorMsg;
            }

            return errorMsg;

        }
        #endregion

        #region IDisposable
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ImportBAL()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}