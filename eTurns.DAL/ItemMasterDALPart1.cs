using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Web;
using System.Data.SqlClient;
using System.Data.Objects;
using eTurns.DAL;
using eTurns.DTO.Resources;
namespace eTurns.DAL
{
    public partial class ItemMasterDAL : eTurnsBaseDAL
    {
        public Int64 GetItemIDOnlyByItemNumber(string ItemNumber, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster objitem = (from im in context.ItemMasters
                                      where (im.IsDeleted ?? false) == false && (im.IsArchived ?? false) == false && im.Room == RoomID && im.ItemNumber == ItemNumber
                                      //&& im.IsActive == true && im.ItemType != 4
                                      select im).FirstOrDefault();
                if (objitem != null)
                {
                    return objitem.ID;
                }
                return 0;
            }
        }

        public ItemMasterDTO GetRecordBySupplierPartNo(string SupplierPartNo, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@SupplierPartNo", SupplierPartNo ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.ExecuteStoreQuery<ItemMasterDTO>("exec GetItemBySupPartWithInfo @SupplierPartNo,@RoomID,@CompanyID", params1).FirstOrDefault();

            }
        }

        public bool EditEDIService(ItemMasterDTO objDTO, long SessionUserId)
        {
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            RequisitionDetailsDAL objReqDetailDAL = new RequisitionDetailsDAL(DataBaseName);
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //ItemMaster obj = new ItemMaster();
                ItemMaster obj = context.ItemMasters.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                double? OldItemCost = obj.Cost;
                //obj.ID = objDTO.ID;
                obj.ItemNumber = objDTO.ItemNumber;
                if (objDTO.SupplierID > 0)
                    obj.SupplierID = objDTO.SupplierID;
                obj.SupplierPartNo = objDTO.SupplierPartNo;
                if (objDTO.UOMID > 0)
                    obj.UOMID = objDTO.UOMID;
                obj.DefaultReorderQuantity = (objDTO.DefaultReorderQuantity == null ? 0 : objDTO.DefaultReorderQuantity.Value);
                obj.DefaultPullQuantity = (objDTO.DefaultPullQuantity == null ? 0 : objDTO.DefaultPullQuantity.Value);
                obj.Cost = objDTO.Cost;
                obj.Markup = objDTO.Markup;
                obj.SellPrice = objDTO.SellPrice;
                obj.CriticalQuantity = objDTO.CriticalQuantity ?? 0;
                obj.MinimumQuantity = objDTO.MinimumQuantity ?? 0;
                obj.MaximumQuantity = objDTO.MaximumQuantity ?? 0;
                obj.DefaultLocation = objDTO.DefaultLocation;
                obj.ItemType = objDTO.ItemType;
                obj.GUID = objDTO.GUID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.IsItemLevelMinMaxQtyRequired = (objDTO.IsItemLevelMinMaxQtyRequired.HasValue ? objDTO.IsItemLevelMinMaxQtyRequired : false);
                CostDTO ObjCostDTO = GetExtCostAndAvgCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.WhatWhereAction);
                obj.ExtendedCost = ObjCostDTO.ExtCost;
                obj.AverageCost = ObjCostDTO.AvgCost;
                obj.PerItemCost = ObjCostDTO.PerItemCost;

                objDTO.ExtendedCost = ObjCostDTO.ExtCost;
                objDTO.PerItemCost = ObjCostDTO.PerItemCost;
                objDTO.AverageCost = ObjCostDTO.AvgCost;

                obj.IsActive = objDTO.IsActive;
                if (!string.IsNullOrEmpty(objDTO.ItemUniqueNumber))
                    obj.ItemUniqueNumber = objDTO.ItemUniqueNumber;
                else
                {
                    //  obj.ItemUniqueNumber = objCommonDAL.UniqueItemId();
                    obj.ItemUniqueNumber = objCommonDAL.GetItemUniqueIdByRoom(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                }
                obj.QtyToMeetDemand = (objDTO.QtyToMeetDemand == null ? 0 : objDTO.QtyToMeetDemand.Value);
                //Room oRoom = context.Rooms.Where(x => x.ID == obj.Room && x.CompanyID == obj.CompanyID).FirstOrDefault();
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                var oRoom = objRoomDAL.GetRoomByIDPlain(obj.Room.GetValueOrDefault());

                if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString() && !obj.Consignment)
                {
                    obj.Cost = objDTO.Cost = OldItemCost;
                }
                else if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !obj.Consignment
                    && objDTO.Cost != OldItemCost)
                {
                    new ItemMasterDAL(base.DataBaseName).UpdatePastReceiptCostByItemCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Cost.GetValueOrDefault(0));
                }

                if ((obj.Markup ?? 0) > 0)
                    obj.SellPrice = (obj.Cost ?? 0) + (((obj.Cost ?? 0) * (obj.Markup ?? 0)) / 100);
                else
                    obj.SellPrice = obj.Cost;
                objDTO.SellPrice = obj.SellPrice;
                obj.RequisitionedQuantity = objReqDetailDAL.GetItemCurrentOnRequisitionQty(obj.GUID);

                if (objDTO.IsOnlyFromItemUI) //Only Updated When Item Updated From Item Detail Page
                {
                    obj.ReceivedOn = (objDTO.ReceivedOn.HasValue ? Convert.ToDateTime(objDTO.ReceivedOn) : DateTimeUtility.DateTimeNow);
                    if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                        obj.EditedFrom = objDTO.EditedFrom;
                    else
                        obj.EditedFrom = "Web";
                }

                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Item";
                obj.WhatWhereAction = objDTO.WhatWhereAction;

                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.UDF6 = objDTO.UDF6;
                obj.UDF7 = objDTO.UDF7;
                obj.UDF8 = objDTO.UDF8;
                obj.UDF9 = objDTO.UDF9;
                obj.UDF10 = objDTO.UDF10;

                if (string.IsNullOrWhiteSpace(objDTO.ImageType))
                {
                    objDTO.ImageType = "ExternalImage";

                }
                obj.ImageType = objDTO.ImageType;
                obj.ItemImageExternalURL = objDTO.ItemImageExternalURL;
                obj.ImagePath = objDTO.ImagePath;

                obj.IsTransfer = objDTO.IsTransfer;
                obj.IsPurchase = objDTO.IsPurchase;
                obj.ItemIsActiveDate = objDTO.ItemIsActiveDate;
                obj.IsAllowOrderCostuom = objDTO.IsAllowOrderCostuom;
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();
                objDTO = FillWithExtraDetail(objDTO);
                //if (objDTO.ItemType != 4)
                //{
                //    if (objDTO.OnHandQuantity.GetValueOrDefault(0) <= 0)
                //    {
                //        try
                //        {
                //            SendMailWhenItemStockOut(objDTO.ID, objDTO.CompanyID.GetValueOrDefault(0), obj.Room.GetValueOrDefault(0), objDTO.LastUpdatedBy.GetValueOrDefault(0), objDTO.OnHandQuantity.GetValueOrDefault(0), objDTO.ItemNumber, objDTO.GUID);
                //        }
                //        catch (Exception ex)
                //        {
                //            throw;
                //        }
                //    }
                //    else
                //    {
                //        RemoveItemStockOutMailLog(objDTO.ID, objDTO.CompanyID.GetValueOrDefault(0), obj.Room.GetValueOrDefault(0));
                //    }
                //}
            }

            //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, objDTO.LastUpdatedBy ?? 0, "web", "ItemMasterDAL >> EDITEDIService");
            new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, objDTO.LastUpdatedBy ?? 0, "web", "Inventory >> Modified Item", SessionUserId);
            objDTO.SuggestedOrderQuantity = GetSuggestedOrderQty(objDTO.GUID);
            objDTO.SuggestedTransferQuantity = GetSuggestedTransferQty(objDTO.GUID);
            return true;
        }

        //public void SendMailWhenItemStockOut(Int64 ItemID, Int64 CompanyId, Int64 RoomID, Int64 UserId, double OnHandQuantity, string ItemNO, Guid ItemGUID)
        //{
        //    Int64 EnterPriceID = 0;
        //    string RoomName = string.Empty;
        //    string CompanyName = string.Empty;
        //    GetRoomEnterPriseComapnyDetail(RoomID, out EnterPriceID, out RoomName, out CompanyName);
        //    ItemStockOutMailLog objItemStockOutMailLog = new ItemStockOutMailLog();

        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        objItemStockOutMailLog = (from u in context.ItemStockOutMailLogs
        //                                  where u.ItemId == ItemID && u.RoomId == RoomID && u.CompanyId == CompanyId
        //                                  select u
        //                                   ).FirstOrDefault();
        //    }
        //    if (objItemStockOutMailLog == null || objItemStockOutMailLog.ItemId <= 0)
        //    {

        //        SendMailForItemStockOut(ItemID, RoomID, CompanyId, EnterPriceID, RoomName, CompanyName, ItemGUID, UserId);
        //        ItemStockOutMailLogDTO objItemStockOutMailLogDTo = new ItemStockOutMailLogDTO();
        //        objItemStockOutMailLogDTo.CompanyId = CompanyId;
        //        objItemStockOutMailLogDTo.RoomId = RoomID;
        //        objItemStockOutMailLogDTo.OnHandQuantity = OnHandQuantity;
        //        objItemStockOutMailLogDTo.ItemId = ItemID;
        //        objItemStockOutMailLogDTo.ItemNumber = ItemNO;
        //        objItemStockOutMailLogDTo.ItemGUID = ItemGUID;
        //        SaveItemStockOutMailLog(objItemStockOutMailLogDTo);

        //    }
        //}
        //public void GetRoomEnterPriseComapnyDetail(Int64 RoomID, out Int64 EnterPriceID, out string RoomName, out string CompanyName)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        Room roomObj = new Room();
        //        roomObj = (from u in context.Rooms
        //                   where u.ID == RoomID
        //                   select u
        //                                      ).FirstOrDefault();
        //        RoomName = roomObj.RoomName;
        //        Nullable<Int64> CompanyId = roomObj.CompanyID;

        //        CompanyMaster companyObj = new CompanyMaster();
        //        companyObj = (from u in context.CompanyMasters
        //                      where u.ID == CompanyId
        //                      select u
        //                                      ).FirstOrDefault();
        //        CompanyName = companyObj.Name;
        //        EnterpriseMaster enterpriseObj = new EnterpriseMaster();
        //        enterpriseObj = (from u in context.EnterpriseMasters

        //                         select u
        //                                      ).FirstOrDefault();
        //        EnterPriceID = enterpriseObj.ID;

        //    }
        //}

        //public void SaveItemStockOutMailLog(ItemStockOutMailLogDTO objItemStockOutMailLogDTo)
        //{
        //    ItemStockOutMailLog objItemStockOutMailLog = new ItemStockOutMailLog();
        //    ItemStockOutHistory objItemStockOutHistory = new ItemStockOutHistory();
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        objItemStockOutMailLog = (from u in context.ItemStockOutMailLogs
        //                                  where u.ItemId == objItemStockOutMailLogDTo.ItemId && u.RoomId == objItemStockOutMailLogDTo.RoomId && u.CompanyId == objItemStockOutMailLogDTo.CompanyId
        //                                  select u
        //                                   ).FirstOrDefault();
        //        if (objItemStockOutMailLog == null)
        //        {
        //            objItemStockOutMailLog = new ItemStockOutMailLog();
        //            objItemStockOutMailLog.Id = 0;
        //            objItemStockOutMailLog.ItemId = objItemStockOutMailLogDTo.ItemId;
        //            objItemStockOutMailLog.RoomId = objItemStockOutMailLogDTo.RoomId;
        //            objItemStockOutMailLog.CompanyId = objItemStockOutMailLogDTo.CompanyId;
        //            objItemStockOutMailLog.OnHandQuantity = objItemStockOutMailLogDTo.OnHandQuantity ?? 0;
        //            objItemStockOutMailLog.ItemNumber = objItemStockOutMailLogDTo.ItemNumber;
        //            objItemStockOutMailLog.ItemGUID = objItemStockOutMailLogDTo.ItemGUID;
        //            context.ItemStockOutMailLogs.AddObject(objItemStockOutMailLog);

        //            //objItemStockOutHistory.ItemId = objItemStockOutMailLogDTo.ItemId;
        //            //objItemStockOutHistory.RoomId = objItemStockOutMailLogDTo.RoomId;
        //            //objItemStockOutHistory.CompanyId = objItemStockOutMailLogDTo.CompanyId;
        //            //objItemStockOutHistory.ItemNumber = objItemStockOutMailLogDTo.ItemNumber;
        //            //objItemStockOutHistory.ItemGUID = objItemStockOutMailLogDTo.ItemGUID;
        //            //objItemStockOutHistory.StockOutDate = DateTimeUtility.DateTimeNow;
        //            //context.ItemStockOutHistories.AddObject(objItemStockOutHistory);
        //            context.SaveChanges();
        //        }
        //    }

        //}

        //private bool SendMailForItemStockOut(Int64 ItemID, Int64 RoomID, Int64 CompanyID, Int64 EnterPriceID, string RoomName, string CompanyName, Guid ItemGUID, Int64 UserId)
        //{

        //    try
        //    {
        //        //ItemMasterDAL objDAL = new ItemMasterDAL(base.);
        //        ItemMasterDTO item = GetItemWithoutJoins(ItemID, null);

        //        if (item != null)
        //        {
        //            SendItemStockouteMail(RoomID, CompanyID, string.Empty, EnterPriceID, RoomName, CompanyName, ItemID, ItemGUID, UserId);
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    finally
        //    {

        //    }
        //}

        //public void SendItemStockouteMail(Int64 RoomID, Int64 CompanyID, string MessageBodyPart, Int64 EnterPriceID, string RoomName, string CompanyName, Int64 ItemID, Guid ItemGUID, Int64 UserId)
        //{


        //    AlertMail objAlertMail = new AlertMail();
        //    //eTurnsUtility objUtils = null;
        //    RegionSettingDAL objRegionSettingDAL = null;
        //    //IEnumerable<EmailUserConfigurationDTO> extUsers = null;
        //    EmailTemplateDAL objEmailTemplateDAL = null;
        //    EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
        //    //string[] splitEmails = null;
        //    List<eMailAttachmentDTO> objeMailAttchList = null;
        //    //eMailAttachmentDTO objeMailAttch = null;
        //    //ArrayList arrAttchament = null;
        //    NotificationDAL objNotificationDAL = null;
        //    EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(base.DataBaseName).GetEnterprise(EnterPriceID);
        //    List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
        //    List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
        //    try
        //    {
        //        objRegionSettingDAL = new RegionSettingDAL(DataBaseName);
        //        eTurnsRegionInfo RegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, UserId);




        //        objNotificationDAL = new NotificationDAL(base.DataBaseName);
        //        if (HttpContext.Current != null)
        //        {
        //            lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.ItemStockOut, RoomID, CompanyID, ResourceHelper.CurrentCult.Name);
        //        }
        //        else
        //        {
        //            if (RegionInfo != null)
        //            {
        //                lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.ItemStockOut, RoomID, CompanyID, RegionInfo.CultureCode ?? "en-US");
        //            }
        //            else
        //            {
        //                lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.ItemStockOut, RoomID, CompanyID, "en-US");
        //            }

        //        }
        //        // lstNotifications = lstNotifications.Where(t => t.SupplierIds == Convert.ToString(ToSuppliers.ID)).ToList();
        //        lstNotifications.ForEach(t =>
        //        {
        //            if (t.SchedulerParams.ScheduleMode == 5)
        //            {
        //                lstNotificationsImidiate.Add(t);
        //            }
        //        });

        //        if (lstNotificationsImidiate.Count > 0)
        //        {
        //            lstNotificationsImidiate.ForEach(t =>
        //            {
        //                string StrSubject = string.Empty;
        //                if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
        //                {
        //                    StrSubject = t.EmailTemplateDetail.lstEmailTemplateDtls.First().MailSubject;
        //                }
        //                string strToAddress = t.EmailAddress;
        //                string strBCCAddress = ConfigurationManager.AppSettings["BCCAddress"];
        //                if (!string.IsNullOrEmpty(strToAddress))
        //                {

        //                    //string strCCAddress = "";
        //                    StringBuilder MessageBody = new StringBuilder();
        //                    objEmailTemplateDAL = new EmailTemplateDAL(base.DataBaseName);
        //                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
        //                    if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
        //                    {
        //                        objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.First();
        //                    }
        //                    if (objEmailTemplateDetailDTO != null)
        //                    {
        //                        MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
        //                        StrSubject = objEmailTemplateDetailDTO.MailSubject;
        //                    }
        //                    else
        //                    {
        //                        return;
        //                    }
        //                    // MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
        //                    // MessageBody.Replace("@@Supplier@@", objOrder.SupplierName);
        //                    //string stratatTABLEatatTag = MessageBodyPart;

        //                    string replacePart = string.Empty;
        //                    if (objEnterpriseDTO != null && (!string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterPriseDomainURL)))
        //                    {
        //                        replacePart = objEnterpriseDTO.EnterPriseDomainURL;
        //                    }
        //                    else if (HttpContext.Current != null && HttpContext.Current.Request == null)
        //                    {
        //                        replacePart = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
        //                    }
        //                    else
        //                    {
        //                        string urlPart = HttpContext.Current.Request.Url.ToString();
        //                        replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
        //                    }
        //                    ItemMasterDAL objDAL = new ItemMasterDAL(base.DataBaseName);
        //                    ItemMasterDTO item = objDAL.GetItemWithoutJoins(ItemID, null);
        //                    string trs = "";

        //                    if (item != null)
        //                    {
        //                        trs += @"<tr>
        //                            <td>
        //                                " + (string.IsNullOrEmpty(item.ItemNumber) ? "&nbsp;" : item.ItemNumber) + @"
        //                            </td>
        //                            <td>
        //                                " + item.OnHandQuantity.GetValueOrDefault(0) + @"
        //                            </td>

        //                        </tr>";
        //                        if (string.IsNullOrEmpty(StrSubject))
        //                            StrSubject = "Item \"" + item.ItemNumber + "\" stockout";

        //                    }

        //                    string TRsItemDescription = "";
        //                    string TRsItemMinMaxQuantity = "";
        //                    string TRsItemLocation = "";
        //                    if (item != null)
        //                    {
        //                        TRsItemDescription += @"<tr>
        //                            <td>
        //                                " + "Description :" + @"
        //                            </td>
        //                            <td>
        //                                " + item.Description + @"
        //                            </td>

        //                        </tr>";

        //                        TRsItemMinMaxQuantity += @"<tr>
        //                            <td>
        //                                " + "Minimum Quantity :" + @"
        //                            </td>
        //                            <td>
        //                                " + item.MinimumQuantity.GetValueOrDefault(0) + @"
        //                            </td>

        //                        </tr>";

        //                        TRsItemMinMaxQuantity += @"<tr>
        //                            <td>
        //                                " + "Maximum Quantity :" + @"
        //                            </td>
        //                            <td>
        //                                " + item.MaximumQuantity.GetValueOrDefault(0) + @"
        //                            </td>

        //                        </tr>";

        //                        BinMasterDAL objBinDAL = new BinMasterDAL(base.DataBaseName);
        //                        BinMasterDTO binDTO = objBinDAL.GetBinByID(item.DefaultLocation.GetValueOrDefault(0), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));

        //                        if (binDTO != null)
        //                        {
        //                            TRsItemLocation += @"<tr>
        //                            <td>
        //                                " + "Location :" + @"
        //                            </td>
        //                            <td>
        //                                " + binDTO.BinNumber + @"
        //                            </td>                         
        //                        </tr>";
        //                        }
        //                    }


        //                    MessageBody.Replace("@@TABLE@@", trs);
        //                    MessageBody.Replace("@@ItemDescription@@", TRsItemDescription);
        //                    MessageBody.Replace("@@MinMaxQuantity@@", TRsItemMinMaxQuantity);
        //                    MessageBody.Replace("@@ItemLocation@@", TRsItemLocation);
        //                    //}
        //                    //else if (!ToSuppliers.IsEmailPOInCSV && !ToSuppliers.IsEmailPOInPDF)
        //                    //{
        //                    //    MessageBody.Replace("@@TABLE@@", MessageBodyPart);
        //                    //}
        //                    //else
        //                    //{
        //                    //    string strReplText = "Please see the attached file(s) for order detail.";
        //                    //    //if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
        //                    //    //    strReplText = "Please see the attached file(s) for return order detail.";

        //                    //    //MessageBody.Replace("@@TABLE@@", strReplText);
        //                    //}

        //                    objeMailAttchList = new List<eMailAttachmentDTO>();


        //                    MessageBody.Replace("@@ROOMNAME@@", RoomName);
        //                    //MessageBody.Replace("@@USERNAME@@", UserName);
        //                    MessageBody.Replace("@@COMPANYNAME@@", CompanyName);
        //                    Dictionary<string, string> Params = new Dictionary<string, string>();
        //                    Params.Add("DataGuids", ItemGUID.ToString());
        //                    objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
        //                    objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO, UserId);
        //                    //objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, CompanyID, RoomID, SessionHelper.UserID, objeMailAttchList, "Web => Order => OrderToSupplier");
        //                }
        //            });
        //        }
        //    }
        //    catch (Exception)
        //    { }
        //}

        public bool UpdateItemCostAndUOMOnly(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, double Cost, long CostUOMID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                if (objItem != null)
                {
                    objItem.Cost = Cost;
                    objItem.CostUOMID = CostUOMID;
                    context.SaveChanges();
                }
            }

            return true;
        }

        public List<ItemMasterDTO> GetAllItemsNarrowSearch(long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.ExecuteStoreQuery<ItemMasterDTO>("exec GetAllItemsNarrowSearch @RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<ItemMasterDTO> GetItemsWithJoinsByGUIDs(string ItemGUIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGUIDs", ItemGUIDs) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<ItemMasterDTO>("exec GetItemsWithJoinsByGUIDs @RoomID,@CompanyID,@ItemGUIDs", params1).ToList();
            }
        }

        public Int32 GetItemStatusCountSlowMoving(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, DateTime FromDate, DateTime ToDate, bool IsMoving, string Criteria = "Critical")
        {
            IEnumerable<ItemMasterDTO> ObjCache = GetAllItemsWithJoins(RoomID, CompanyID, IsArchived, IsDeleted, null);
            Int32 TotalCount = 0;

            FromDate = FromDate.AddHours(23);
            ToDate = ToDate.AddHours(23);

            // Need to get data for Slow Moving Fast Moving and Stock Out data and Graph 
            //RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            //RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);
            CommonDAL objDAL = new CommonDAL(base.DataBaseName);
            string columnList = "ID,RoomName,SlowMovingValue,FastMovingValue";
            RoomDTO objRoomDTO = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");


            if (objRoomDTO != null)
            {
                double SlowMoving = objRoomDTO.SlowMovingValue;
                double FastMoving = objRoomDTO.FastMovingValue;
                List<InventoryDashboardDTO> ObjInventoryDATA = new List<InventoryDashboardDTO>();
                ObjInventoryDATA = GetTurnsValueOfItems(RoomID, CompanyID);
                if (Criteria == "Slow Moving")
                {
                    ObjCache = (from x in ObjCache
                                join y in ObjInventoryDATA on
                                    x.GUID equals y.GUID
                                where Convert.ToDouble(y.Turns.GetValueOrDefault(0)) <= SlowMoving
                                && (x.Created.Value.Date >= FromDate.Date && x.Created.Value.Date <= ToDate.Date)
                                select x
                                    ).ToList();
                }
                else if (Criteria == "Fast Moving")
                {
                    ObjCache = (from x in ObjCache
                                join y in ObjInventoryDATA on
                                    x.GUID equals y.GUID
                                where Convert.ToDouble(y.Turns.GetValueOrDefault(0)) >= FastMoving
                                && (x.Created.Value.Date >= FromDate.Date && x.Created.Value.Date <= ToDate.Date)
                                select x
                                    ).ToList();
                }
                else if (Criteria == "Stock Out")
                {
                    List<ItemMasterDTO> ObjStockOutCount = new List<ItemMasterDTO>();
                    ObjStockOutCount = GetStockOutData(RoomID, CompanyID, FromDate, ToDate);

                    ObjCache = (from x in ObjCache
                                where x.OnHandQuantity.GetValueOrDefault(0) <= 0
                                && (x.Created.Value.Date >= FromDate.Date && x.Created.Value.Date <= ToDate.Date)
                                select x
                                    ).ToList();

                    ObjCache = (from u in ObjCache
                                join y in ObjStockOutCount on u.ItemNumber equals y.ItemNumber
                                orderby y.StockOutCount descending
                                select u).ToList();
                }
            }


            TotalCount = ObjCache.Where(t => t.Room == RoomID && (t.Created.Value.Date >= FromDate.Date && t.Created.Value.Date <= ToDate.Date)).Count();
            return TotalCount;
        }
        public IEnumerable<ItemMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, DateTime FromDate, DateTime ToDate, string Criteria = "Critical")
        {
            IEnumerable<ItemMasterDTO> ObjCache = null;
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
            #region "Both False"
            //ObjCache = CacheHelper<IEnumerable<ItemMasterDTO>>.GetCacheItem("Cached_DashboartItemMaster_" + CompanyID.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.ExecuteStoreQuery<ItemMasterDTO>(@"select A.ItemNumber,ISNULL(A.OnOrderQuantity,0) AS OnOrderQuantity,ISNULL(A.OnOrderInTransitQuantity,0) as OnOrderInTransitQuantity,ISNULL(A.OnOrderInTransitQuantity,0) AS OnOrderInTransitQuantity,ISNULL(A.OnTransferQuantity,0) AS OnTransferQuantity, ISNULL(A.AverageUsage,0) AS AverageUsage, ISNULL(A.MinimumQuantity,0) AS MinimumQuantity,ISNULL(A.MaximumQuantity,0) AS MaximumQuantity,ISNULL(A.CriticalQuantity,0) AS CriticalQuantity,ISNULL(A.OnHandQuantity,0) AS OnHandQuantity, A.Created
                                                    ,A.GUID,A.Updated,A.CreatedBy,A.LastUpdatedBy,A.IsDeleted,A.IsArchived,A.CompanyID,A.Room,
                                                (CASE WHEN ISNULL(OnHandQuantity,0) < ISNULL(CriticalQuantity,0) THEN 'Critical'
                                                ELSE (CASE WHEN (ISNULL(OnHandQuantity,0) between ISNULL(CriticalQuantity,0) AND ISNULL(MinimumQuantity,0) ) THEN 'Minimum'
	                                                ELSE (CASE WHEN ISNULL(OnHandQuantity,0) > ISNULL(MinimumQuantity,0) OR (ISNULL(OnHandQuantity,0) >ISNULL(MaximumQuantity,0) ) THEN 'Maximum' END)
	                                                END)
                                                END) AS LongDescription,A.Description AS Description,ISNULL(A.IsAllowOrderCostuom,0) as IsAllowOrderCostuom,
                                                (SELECT ISNULL(SUM(ISNULL(Quantity,0)),0) from CartItem where ItemGUID=A.GUID) AS DefaultReorderQuantity
                                                FROM ItemMaster A WHERE A.Created IS NOT NULL AND ISNULL(A.IsDeleted,0)=0 and ISNULL(A.IsArchived,0) = 0 AND A.CompanyID = " + CompanyID.ToString())
                                select new ItemMasterDTO
                                {
                                    ItemNumber = u.ItemNumber,
                                    OnHandQuantity = u.OnHandQuantity,
                                    CriticalQuantity = u.CriticalQuantity,
                                    MinimumQuantity = u.MinimumQuantity,
                                    MaximumQuantity = u.MaximumQuantity,
                                    OnOrderQuantity = u.OnOrderQuantity,
                                    OnOrderInTransitQuantity = u.OnOrderInTransitQuantity,
                                    OnTransferQuantity = u.OnTransferQuantity,
                                    AverageUsage = u.AverageUsage,
                                    Created = u.Created,
                                    GUID = u.GUID,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    CompanyID = u.CompanyID,
                                    Room = u.Room,
                                    Description = u.Description,
                                    LongDescription = u.LongDescription,
                                    DefaultReorderQuantity = u.DefaultReorderQuantity, // Cart Quantit
                                    IsAllowOrderCostuom = u.IsAllowOrderCostuom
                                }).AsParallel().ToList();
                    //  ObjCache = CacheHelper<IEnumerable<ItemMasterDTO>>.AddCacheItem("Cached_DashboartItemMaster_" + CompanyID.ToString(), obj);
                    //return objReturn;
                }
            }
            #endregion

            return ObjCache.Where(t => t.Room == RoomID && (t.Created.Value.Date >= FromDate.Date && t.Created.Value.Date <= ToDate.Date) && t.IsDeleted == false && t.IsArchived == false && t.LongDescription == Criteria);
        }

        public IEnumerable<InventoryDashboardDTO> GetAllRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, int FromYear, int ToYear, int FromMonth, int ToMonth, int NOBackDays, int NODaysAve, decimal NOTimes, int MinPer, int MaxPer, int AutoMinPer, int AutoMaxPer, bool IsItemLevelMinMax)
        {
            IEnumerable<InventoryDashboardDTO> ObjCache = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //context
                //IEnumerable<GetInventoryDashboardItem_Result> obj = context.GetInventoryDashboardItem(CompanyID, RoomID, FromYear, ToYear, FromMonth, ToMonth, NOBackDays, NODaysAve, NOTimes, MinPer, MaxPer, AutoMinPer, AutoMaxPer, IsItemLevelMinMax).ToList();
                //ObjCache = (from m in obj
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@Room", RoomID), new SqlParameter("@FromYear", FromYear), new SqlParameter("@ToYear", ToYear)
                                                    , new SqlParameter("@FromMonth", FromMonth), new SqlParameter("@ToMonth", ToMonth), new SqlParameter("@NOBackDays", NOBackDays), new SqlParameter("@NODaysAve", NODaysAve)
                                                    , new SqlParameter("@NOTimes", NOTimes), new SqlParameter("@MinPer", MinPer), new SqlParameter("@MaxPer", MaxPer), new SqlParameter("@AutoMinPer", AutoMinPer), new SqlParameter("@AutoMaxPer", AutoMaxPer), new SqlParameter("@IsItemLevelMinMax", IsItemLevelMinMax)};

                List<InventoryDashboardDTO> lstCountLineItemDetail = (from m in context.ExecuteStoreQuery<InventoryDashboardDTO>("exec [GetInventoryDashboardItem] @CompanyID,@Room,@FromYear,@ToYear,@FromMonth,@ToMonth,@NOBackDays,@NODaysAve,@NOTimes,@MinPer,@MaxPer,@AutoMinPer,@AutoMaxPer,@IsItemLevelMinMax", params1)

                                                                      select new InventoryDashboardDTO
                                                                      {
                                                                          ItemNumber = m.ItemNumber,
                                                                          Description = m.Description,
                                                                          Category = m.Category,
                                                                          AvailableQty = m.AvailableQty,
                                                                          InventoryValue = m.InventoryValue,
                                                                          Turns = m.Turns,
                                                                          PullTurnsAmt = m.PullTurnsAmt,
                                                                          OrderTurnsAmt = m.OrderTurnsAmt,
                                                                          CurrentMin = m.CurrentMin,
                                                                          CalculatedMin = m.CalculatedMin,
                                                                          MinPercentage = m.MinPercentage,
                                                                          CurrentMax = m.CurrentMax,
                                                                          CalculatedMax = m.CalculatedMax,
                                                                          MaxPercentage = m.MaxPercentage,
                                                                          MinAnalysis = m.MinAnalysis,
                                                                          MaxAnalysis = m.MaxAnalysis,
                                                                          AutoCurrentMin = m.AutoCurrentMin,
                                                                          AutoMinPercentage = m.AutoMinPercentage,
                                                                          AutoCurrentMax = m.AutoCurrentMax,
                                                                          AutoMaxPercentage = m.AutoMaxPercentage,
                                                                          GUID = m.GUID,
                                                                          BinID = m.BinID,
                                                                          BinNumber = m.BinNumber
                                                                      }).ToList();
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(c => c.ItemNumber);//.Skip(StartRowIndex).Take(MaxRows);

            }
            // return ObjCache;
        }

        public IEnumerable<InventoryDashboardDTO> GetAllRecordsWithoutPaging(Int64 RoomID, Int64 CompanyID, int FromYear, int ToYear, int FromMonth, int ToMonth, int NOBackDays, int NODaysAve, decimal NOTimes, int MinPer, int MaxPer, int AutoMinPer, int AutoMaxPer, bool IsItemLevelMinMax)
        {
            IEnumerable<InventoryDashboardDTO> ObjCache = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //context
                //IEnumerable<GetInventoryDashboardItem_Result> obj = context.GetInventoryDashboardItem(CompanyID, RoomID, FromYear, ToYear, FromMonth, ToMonth, NOBackDays, NODaysAve, NOTimes, MinPer, MaxPer, AutoMinPer, AutoMaxPer, IsItemLevelMinMax).ToList();
                //ObjCache = (from m in obj
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@Room", RoomID), new SqlParameter("@FromYear", FromYear), new SqlParameter("@ToYear", ToYear)
                                                    , new SqlParameter("@FromMonth", FromMonth), new SqlParameter("@ToMonth", ToMonth), new SqlParameter("@NOBackDays", NOBackDays), new SqlParameter("@NODaysAve", NODaysAve)
                                                    , new SqlParameter("@NOTimes", NOTimes), new SqlParameter("@MinPer", MinPer), new SqlParameter("@MaxPer", MaxPer), new SqlParameter("@AutoMinPer", AutoMinPer), new SqlParameter("@AutoMaxPer", AutoMaxPer), new SqlParameter("@IsItemLevelMinMax", IsItemLevelMinMax)};

                ObjCache = (from m in context.ExecuteStoreQuery<InventoryDashboardDTO>("exec [GetInventoryDashboardItem] @CompanyID,@Room,@FromYear,@ToYear,@FromMonth,@ToMonth,@NOBackDays,@NODaysAve,@NOTimes,@MinPer,@MaxPer,@AutoMinPer,@AutoMaxPer,@IsItemLevelMinMax", params1)

                            select new InventoryDashboardDTO
                            {
                                //ItemNumber = m.ItemNumber,
                                //Description = m.Description,
                                //Category = m.Category,
                                //AvailableQty = m.AvailableQty,
                                //InventoryValue = m.InventoryValue,
                                //Turns = m.Turns,
                                //PullTurnsAmt = m.PullTurnsAmt,
                                //OrderTurnsAmt = m.OrderTurnsAmt,
                                //CurrentMin = m.CurrentMin,
                                //CalculatedMin = m.CalculatedMin,
                                //MinPercentage = m.MinPercentage,
                                //CurrentMax = m.CurrentMax,
                                //CalculatedMax = m.CalculatedMax,
                                //MaxPercentage = m.MaxPercentage,
                                MinAnalysis = m.MinAnalysis,
                                MaxAnalysis = m.MaxAnalysis,
                                //AutoCurrentMin = m.AutoCurrentMin,
                                //AutoMinPercentage = m.AutoMinPercentage,
                                //AutoCurrentMax = m.AutoCurrentMax,
                                //AutoMaxPercentage = m.AutoMaxPercentage,
                                //GUID = m.GUID
                            }).ToList();
                //TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(c => c.ItemNumber);

            }
            // return ObjCache;
        }

        public InventoryDashboardDTO GetHeaderCount(Int64 RoomID, Int64 CompanyID)
        {

            InventoryDashboardDTO ObjCache = null;

            string strQuery = @"SELECT case When (Sum(ISNULL(InventoryValue,0))/10) > 0 Then Convert(decimal(18,2), ((Sum(ISNULL(MonthlyPulledQty,0)) / (Sum(ISNULL(InventoryValue,0))/" + DateTime.Now.Month.ToString() + ")) * 12/" + DateTime.Now.Month.ToString() + " )) else 0 END AS Turns, Convert(decimal(18,2), Sum(ISNULL(InventoryValue,0))) AS [InventoryValue] FROM  DashboardItemCalculation WHERE Room = " + RoomID.ToString() + " AND CompanyID = " + CompanyID.ToString();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ObjCache = (from u in context.ExecuteStoreQuery<InventoryDashboardDTO>(strQuery)
                            select new InventoryDashboardDTO
                            {
                                InventoryValue = u.InventoryValue,
                                Turns = u.Turns,
                            }).SingleOrDefault();


            }
            return ObjCache;
        }
        public List<ItemMasterDTO> GetMovingData(Int64 RoomID, Int64 CompanyID, DateTime FromDate, DateTime ToDate, bool IsSlowMoving)
        {
            //RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            CommonDAL objDAL = new CommonDAL(base.DataBaseName);
            //RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);
            string columnList = "ID,RoomName,SlowMovingValue,FastMovingValue";
            RoomDTO objRoomDTO = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

            double SlowMoving = 0;
            double FastMoving = 0;
            if (objRoomDTO != null)
            {
                SlowMoving = objRoomDTO.SlowMovingValue;
                FastMoving = objRoomDTO.FastMovingValue;
            }
            List<InventoryDashboardDTO> ObjInventoryDATA = new List<InventoryDashboardDTO>();
            ObjInventoryDATA = GetTurnsValueOfItems(RoomID, CompanyID);
            IEnumerable<ItemMasterDTO> ObjCache = GetAllItemsWithJoins(RoomID, CompanyID, false, false, null);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (IsSlowMoving)
                {
                    return (from x in ObjCache
                            join y in ObjInventoryDATA on
                                x.GUID equals y.GUID
                            where Convert.ToDouble(y.Turns.GetValueOrDefault(0)) <= SlowMoving
                            && (x.Created.Value.Date >= FromDate.Date && x.Created.Value.Date <= ToDate.Date)
                            select x
                                    ).ToList();
                }
                else
                {
                    return (from x in ObjCache
                            join y in ObjInventoryDATA on
                                x.GUID equals y.GUID
                            where Convert.ToDouble(y.Turns.GetValueOrDefault(0)) >= FastMoving
                            && (x.Created.Value.Date >= FromDate.Date && x.Created.Value.Date <= ToDate.Date)
                            select x
                                    ).ToList();
                }
            }
        }

        public List<InventoryDashboardDTO> GetTurnsValueOfItems(Int64 RoomID, Int64 CompanyID)
        {
            List<InventoryDashboardDTO> ObjCache = null;
            string strQuery = @"SELECT ItemGUID as GUID, Convert(decimal(18,2), ((ISNULL(MonthlyPulledQty,0) / (ISNULL(InventoryValue,0)/" + DateTime.Now.Month.ToString() + ")) * 12/" + DateTime.Now.Month.ToString() + " )) AS Turns FROM  DashboardItemCalculation WHERE Room = " + RoomID.ToString() + " AND ISNULL(InventoryValue,0) > 0 AND ISNULL(MonthlyPulledQty,0) > 0 AND CompanyID = " + CompanyID.ToString() + "";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ObjCache = (from u in context.ExecuteStoreQuery<InventoryDashboardDTO>(strQuery)
                            select new InventoryDashboardDTO
                            {
                                GUID = u.GUID,
                                Turns = u.Turns,
                            }).ToList();
            }
            return ObjCache;
        }

        public InventoryDashboardDTO GetHeaderCountByItemID(Int64 RoomID, Int64 CompanyID, string ItemGUID)
        {

            InventoryDashboardDTO ObjCache = null;

            string strQuery = @"SELECT Convert(decimal(18,2), ((Sum(ISNULL(MonthlyPulledQty,0)) / (Sum(ISNULL(InventoryValue,0))/" + DateTime.Now.Month.ToString() + ")) * 12/" + DateTime.Now.Month.ToString() + " )) AS Turns , Convert(decimal(18,2), Sum(ISNULL(InventoryValue,0))) AS [InventoryValue] FROM  DashboardItemCalculation WHERE Room = " + RoomID.ToString() + " AND CompanyID = " + CompanyID.ToString() + " AND ItemGUID = '" + ItemGUID + "'";
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.ExecuteStoreQuery<InventoryDashboardDTO>(strQuery)
                                select new InventoryDashboardDTO
                                {
                                    InventoryValue = u.InventoryValue,
                                    Turns = u.Turns,
                                }).SingleOrDefault();
                }
            }
            catch (Exception)
            {
            }
            return ObjCache;
        }

        public Int32 GetYTDCount(Int64 RoomID, Int64 CompanyID)
        {
            Int32 iYTDCount = 0;
            string strQuery = @"SELECT ISNULL(COUNT(*),0) AS Cnt FROM (select ItemNumber,CONVERT(DATE, Updated) AS Updated from ItemMaster_History where OnHandQuantity=0  and Room = " + RoomID.ToString() + " AND CompanyID = " + CompanyID.ToString() + " AND ISNULL(IsDeleted,0)=0 and ISNULL(IsArchived,0) = 0 Group by ItemNumber,CONVERT(DATE, Updated)) AS fn";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                iYTDCount = (from u in context.ExecuteStoreQuery<Int32>(strQuery) select u).SingleOrDefault();
            }
            return iYTDCount;
        }
        public Int32 GetMTDCount(Int64 RoomID, Int64 CompanyID)
        {
            Int32 iMTDCount = 0;
            string strQuery = @"SELECT ISNULL(COUNT(*),0) AS Cnt FROM (select ItemNumber,CONVERT(DATE, Updated) AS Updated from ItemMaster_History where OnHandQuantity=0  and Room = " + RoomID.ToString() + " AND CompanyID = " + CompanyID.ToString() + " AND ISNULL(IsDeleted,0)=0 and ISNULL(IsArchived,0) = 0 Group by ItemNumber,CONVERT(DATE, Updated)) AS fn";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                iMTDCount = (from u in context.ExecuteStoreQuery<Int32>(strQuery) select u).SingleOrDefault();
            }
            return iMTDCount;
        }
        public List<ItemMasterDTO> GetStockOutData(Int64 RoomID, Int64 CompanyID, DateTime FromDate, DateTime ToDate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ItemMasterDTO>(@"
                                select ItemNumber,count(*) as LeadTimeInDays from itemmaster_history
                                where updated >= '" + FromDate + "' And Updated <= '" + ToDate + "' and OnHandQuantity = 0 and Room = " + RoomID + " and CompanyID = " + CompanyID + " group by ItemNumber")
                        select new ItemMasterDTO
                        {
                            ItemNumber = u.ItemNumber,
                            LeadTimeInDays = u.LeadTimeInDays.GetValueOrDefault(0),
                            StockOutCount = u.LeadTimeInDays.GetValueOrDefault(0),
                        }).ToList();
            }
        }

        public List<DashboardItem> GetBelowCriticalItems(Int64 RoomID, Int64 CompanyID, DateTime FromDate, DateTime ToDate, bool WithFilter, int? MaxItems)
        {
            IQueryable<DashboardItem> BelowCriticalItems = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                BelowCriticalItems = (from im in context.ItemMasters
                                      join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
                                      from im_sm in im_sm_join.DefaultIfEmpty()
                                      join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
                                      from im_icm in im_icm_join.DefaultIfEmpty()
                                      where (im.OnHandQuantity ?? 0) > 0 && im.CriticalQuantity > 0 && im.OnHandQuantity < im.CriticalQuantity && im.Room == RoomID && im.CompanyID == CompanyID && (im.IsArchived ?? false) == false && (im.IsDeleted ?? false) == false
                                      select new DashboardItem
                                      {
                                          AverageUsage = im.AverageUsage,
                                          Cost = im.Cost,
                                          CriticalQuantity = im.CriticalQuantity,
                                          DefaultReorderQuantity = im.DefaultReorderQuantity,
                                          Description = im.Description,
                                          GUID = im.GUID,
                                          ID = im.ID,
                                          InventoryClassification = im.InventoryClassification,
                                          InventoryClassificationName = im_icm.InventoryClassification,
                                          ItemNumber = im.ItemNumber,
                                          LongDescription = im.LongDescription,
                                          MaximumQuantity = im.MaximumQuantity,
                                          MinimumQuantity = im.MinimumQuantity,
                                          OnHandQuantity = im.OnHandQuantity,
                                          OnOrderQuantity = im.OnOrderQuantity,
                                          OnTransferQuantity = im.OnTransferQuantity,
                                          RationalFactor = (im.OnHandQuantity ?? 0) > 0 ? ((im.CriticalQuantity / (im.OnHandQuantity ?? 1)) * 100) : 0,
                                          RequisitionedQuantity = im.RequisitionedQuantity,
                                          StockOutCount = 0,
                                          SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                                          SupplierID = im.SupplierID,
                                          SupplierName = im_sm.SupplierName,
                                          SupplierPartNo = im.SupplierPartNo,
                                          Turns = im.Turns
                                      }).OrderBy(t => t.Turns);
                if ((MaxItems ?? 0) > 0 && BelowCriticalItems.Any())
                {
                    BelowCriticalItems = BelowCriticalItems.Take(MaxItems ?? 10);
                }

                return BelowCriticalItems.ToList();
            }
        }
        public List<DashboardItem> GetBelowMinimumItems(Int64 RoomID, Int64 CompanyID, DateTime FromDate, DateTime ToDate, bool WithFilter, int? MaxItems)
        {
            IQueryable<DashboardItem> BelowMinimumItems = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                BelowMinimumItems = (from im in context.ItemMasters
                                     join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
                                     from im_sm in im_sm_join.DefaultIfEmpty()
                                     join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
                                     from im_icm in im_icm_join.DefaultIfEmpty()
                                     where (im.OnHandQuantity ?? 0) > 0 && im.MinimumQuantity > 0 && im.OnHandQuantity < im.MinimumQuantity && im.OnHandQuantity >= im.CriticalQuantity && im.Room == RoomID && im.CompanyID == CompanyID && (im.IsArchived ?? false) == false && (im.IsDeleted ?? false) == false
                                     select new DashboardItem
                                     {
                                         AverageUsage = im.AverageUsage,
                                         Cost = im.Cost,
                                         CriticalQuantity = im.CriticalQuantity,
                                         DefaultReorderQuantity = im.DefaultReorderQuantity,
                                         Description = im.Description,
                                         GUID = im.GUID,
                                         ID = im.ID,
                                         InventoryClassification = im.InventoryClassification,
                                         InventoryClassificationName = im_icm.InventoryClassification,
                                         ItemNumber = im.ItemNumber,
                                         LongDescription = im.LongDescription,
                                         MaximumQuantity = im.MaximumQuantity,
                                         MinimumQuantity = im.MinimumQuantity,
                                         OnHandQuantity = im.OnHandQuantity,
                                         OnOrderQuantity = im.OnOrderQuantity,
                                         OnTransferQuantity = im.OnTransferQuantity,
                                         RationalFactor = (im.OnHandQuantity ?? 0) > 0 ? ((im.MinimumQuantity / (im.OnHandQuantity ?? 1)) * 100) : 0,
                                         RequisitionedQuantity = im.RequisitionedQuantity,
                                         StockOutCount = 0,
                                         SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                                         SupplierID = im.SupplierID,
                                         SupplierName = im_sm.SupplierName,
                                         SupplierPartNo = im.SupplierPartNo,
                                         Turns = im.Turns
                                     }).OrderBy(t => t.Turns);
                if ((MaxItems ?? 0) > 0 && BelowMinimumItems.Any())
                {
                    BelowMinimumItems = BelowMinimumItems.Take(MaxItems ?? 10);
                }
                return BelowMinimumItems.ToList();
            }
        }
        public List<DashboardItem> GetAboveMaximumItems(Int64 RoomID, Int64 CompanyID, DateTime FromDate, DateTime ToDate, bool WithFilter, int? MaxItems)
        {
            IQueryable<DashboardItem> AboveMaximumItems = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                AboveMaximumItems = (from im in context.ItemMasters
                                     join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
                                     from im_sm in im_sm_join.DefaultIfEmpty()
                                     join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
                                     from im_icm in im_icm_join.DefaultIfEmpty()
                                     where im.OnHandQuantity > 0 && im.MaximumQuantity > 0 && im.OnHandQuantity > im.MaximumQuantity && im.Room == RoomID && im.CompanyID == CompanyID && (im.IsArchived ?? false) == false && (im.IsDeleted ?? false) == false
                                     select new DashboardItem
                                     {
                                         AverageUsage = im.AverageUsage,
                                         Cost = im.Cost,
                                         CriticalQuantity = im.CriticalQuantity,
                                         DefaultReorderQuantity = im.DefaultReorderQuantity,
                                         Description = im.Description,
                                         GUID = im.GUID,
                                         ID = im.ID,
                                         InventoryClassification = im.InventoryClassification,
                                         InventoryClassificationName = im_icm.InventoryClassification,
                                         ItemNumber = im.ItemNumber,
                                         LongDescription = im.LongDescription,
                                         MaximumQuantity = im.MaximumQuantity,
                                         MinimumQuantity = im.MinimumQuantity,
                                         OnHandQuantity = im.OnHandQuantity,
                                         OnOrderQuantity = im.OnOrderQuantity,
                                         OnTransferQuantity = im.OnTransferQuantity,
                                         RationalFactor = (im.OnHandQuantity ?? 0) > 0 ? ((im.MaximumQuantity / (im.OnHandQuantity ?? 1)) * 100) : 0,
                                         RequisitionedQuantity = im.RequisitionedQuantity,
                                         StockOutCount = 0,
                                         SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                                         SupplierID = im.SupplierID,
                                         SupplierName = im_sm.SupplierName,
                                         SupplierPartNo = im.SupplierPartNo,
                                         Turns = im.Turns
                                     }).OrderBy(t => t.Turns);
                if ((MaxItems ?? 0) > 0 && AboveMaximumItems.Any())
                {
                    AboveMaximumItems = AboveMaximumItems.Take(MaxItems ?? 10);
                }
                return AboveMaximumItems.ToList();
            }
        }
        public List<DashboardItem> GetSlowMovings(Int64 RoomID, Int64 CompanyID, DateTime FromDate, DateTime ToDate, bool WithFilter, int? MaxItems)
        {
            IQueryable<DashboardItem> SlowMovings = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SlowMovings = (from im in context.ItemMasters
                               join rm in context.Rooms on im.Room equals rm.ID
                               join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
                               from im_sm in im_sm_join.DefaultIfEmpty()
                               join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
                               from im_icm in im_icm_join.DefaultIfEmpty()
                               where (im.Turns ?? 0) > 0 && (im.Turns ?? 0) < rm.SlowMovingValue && im.Room == RoomID && im.CompanyID == CompanyID && (im.IsArchived ?? false) == false && (im.IsDeleted ?? false) == false
                               select new DashboardItem
                               {
                                   AverageUsage = im.AverageUsage,
                                   Cost = im.Cost,
                                   CriticalQuantity = im.CriticalQuantity,
                                   DefaultReorderQuantity = im.DefaultReorderQuantity,
                                   Description = im.Description,
                                   GUID = im.GUID,
                                   ID = im.ID,
                                   InventoryClassification = im.InventoryClassification,
                                   InventoryClassificationName = im_icm.InventoryClassification,
                                   ItemNumber = im.ItemNumber,
                                   LongDescription = im.LongDescription,
                                   MaximumQuantity = im.MaximumQuantity,
                                   MinimumQuantity = im.MinimumQuantity,
                                   OnHandQuantity = im.OnHandQuantity,
                                   OnOrderQuantity = im.OnOrderQuantity,
                                   OnTransferQuantity = im.OnTransferQuantity,
                                   RationalFactor = 1,
                                   RequisitionedQuantity = im.RequisitionedQuantity,
                                   StockOutCount = 0,
                                   SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                                   SupplierID = im.SupplierID,
                                   SupplierName = im_sm.SupplierName,
                                   SupplierPartNo = im.SupplierPartNo,
                                   Turns = im.Turns
                               }).OrderBy(t => t.Turns);
                if ((MaxItems ?? 0) > 0 && SlowMovings.Any())
                {
                    SlowMovings = SlowMovings.Take(MaxItems ?? 10);
                }
                return SlowMovings.ToList();
            }
        }
        public List<DashboardItem> GetFastMovings(Int64 RoomID, Int64 CompanyID, DateTime FromDate, DateTime ToDate, bool WithFilter, int? MaxItems)
        {
            IQueryable<DashboardItem> FastMovings = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                FastMovings = (from im in context.ItemMasters
                               join rm in context.Rooms on im.Room equals rm.ID
                               join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
                               from im_sm in im_sm_join.DefaultIfEmpty()
                               join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
                               from im_icm in im_icm_join.DefaultIfEmpty()
                               where (im.Turns ?? 0) > 0 && (im.Turns ?? 0) > rm.FastMovingValue && im.Room == RoomID && im.CompanyID == CompanyID && (im.IsArchived ?? false) == false && (im.IsDeleted ?? false) == false
                               select new DashboardItem
                               {
                                   AverageUsage = im.AverageUsage,
                                   Cost = im.Cost,
                                   CriticalQuantity = im.CriticalQuantity,
                                   DefaultReorderQuantity = im.DefaultReorderQuantity,
                                   Description = im.Description,
                                   GUID = im.GUID,
                                   ID = im.ID,
                                   InventoryClassification = im.InventoryClassification,
                                   InventoryClassificationName = im_icm.InventoryClassification,
                                   ItemNumber = im.ItemNumber,
                                   LongDescription = im.LongDescription,
                                   MaximumQuantity = im.MaximumQuantity,
                                   MinimumQuantity = im.MinimumQuantity,
                                   OnHandQuantity = im.OnHandQuantity,
                                   OnOrderQuantity = im.OnOrderQuantity,
                                   OnTransferQuantity = im.OnTransferQuantity,
                                   RationalFactor = 1,
                                   RequisitionedQuantity = im.RequisitionedQuantity,
                                   StockOutCount = 0,
                                   SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                                   SupplierID = im.SupplierID,
                                   SupplierName = im_sm.SupplierName,
                                   SupplierPartNo = im.SupplierPartNo,
                                   Turns = im.Turns
                               }).OrderBy(t => t.Turns);

                if ((MaxItems ?? 0) > 0 && FastMovings.Any())
                {
                    FastMovings = FastMovings.Take(MaxItems ?? 10);
                }
                return FastMovings.ToList();
            }
        }
        public List<ItemMasterDTO> GetStockOutDataForInvValue(Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ItemMasterDTO>(@"
                                select datepart(month,Updated) as MonthValue,count(*) as LeadTimeInDays from itemmaster_history
                                where OnHandQuantity = 0 and Room = " + RoomID + " and CompanyID = " + CompanyID + " and datepart(year,updated) = " + DateTime.Now.Year + " group by datepart(month,Updated)")
                        select new ItemMasterDTO
                        {
                            ItemNumber = u.ItemNumber,
                            MonthValue = u.MonthValue,
                            LeadTimeInDays = u.LeadTimeInDays.GetValueOrDefault(0),
                            StockOutCount = u.LeadTimeInDays.GetValueOrDefault(0),
                        }).ToList();
            }
        }
        public List<InventoryDashboardDTO> GetInveValueData(Int64 RoomID, Int64 CompanyID, Int32 Month, Int32 Year)
        {
            List<InventoryDashboardDTO> ObjInventoryDATA = new List<InventoryDashboardDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ObjInventoryDATA = (from u in context.ExecuteStoreQuery<InventoryDashboardDTO>(@"
                                        select d.InventoryValue,d.Month as SelectedMonth,d.TurnsAmt as Turns
                                        ,i.itemNumber as ItemNumber
                                        from DashboardItemCalculation as d
                                        inner join itemmaster as i on i.GUID = d.ItemGUID
                                        where d.InventoryValue > 0 and d.Month = " + Month + " and d.year = " + Year +
                                        @" and d.Room = " + RoomID + " and d.CompanyID = " + CompanyID +
                                        @" order by d.InventoryValue desc")
                                    select new InventoryDashboardDTO
                                    {
                                        InventoryValue = u.InventoryValue.GetValueOrDefault(0),
                                        SelectedMonth = u.SelectedMonth.GetValueOrDefault(0),
                                        Turns = u.Turns.GetValueOrDefault(0),
                                        ItemNumber = u.ItemNumber
                                    }).ToList();

                return ObjInventoryDATA;
            }
        }

        public ItemMasterDTO GetRecordByItemNumberLight(string ItemNumber, Int64 RoomID, Int64 CompanyID)
        {

            ItemMasterDTO objItemMaterDTO = new ItemMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objItemMaterDTO = (from u in context.ItemMasters
                                   where (u.IsDeleted ?? false) == false && u.Room == RoomID && u.CompanyID == CompanyID && u.ItemNumber == ItemNumber
                                   select new ItemMasterDTO
                                   {
                                       ID = u.ID,
                                       ItemNumber = u.ItemNumber,
                                       ManufacturerID = u.ManufacturerID,
                                       ManufacturerNumber = u.ManufacturerNumber,
                                       SupplierID = u.SupplierID,
                                       SupplierPartNo = u.SupplierPartNo,
                                       UPC = u.UPC,
                                       UNSPSC = u.UNSPSC,
                                       Description = u.Description,
                                       LongDescription = u.LongDescription,
                                       CategoryID = u.CategoryID,
                                       GLAccountID = u.GLAccountID,
                                       UOMID = u.UOMID,
                                       PricePerTerm = u.PricePerTerm,
                                       CostUOMID = u.CostUOMID,
                                       DefaultReorderQuantity = u.DefaultReorderQuantity,
                                       DefaultPullQuantity = u.DefaultPullQuantity,
                                       Cost = u.Cost,
                                       Markup = u.Markup,
                                       SellPrice = u.SellPrice,
                                       ExtendedCost = u.ExtendedCost,
                                       AverageCost = u.AverageCost,
                                       LeadTimeInDays = u.LeadTimeInDays,
                                       Link1 = u.Link1,
                                       Link2 = u.Link2,
                                       Trend = u.Trend,
                                       Taxable = u.Taxable,
                                       Consignment = u.Consignment,
                                       StagedQuantity = u.StagedQuantity,
                                       InTransitquantity = u.InTransitquantity,
                                       OnOrderQuantity = u.OnOrderQuantity,
                                       OnReturnQuantity = u.OnReturnQuantity,
                                       OnTransferQuantity = u.OnTransferQuantity,
                                       SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                       SuggestedTransferQuantity = u.SuggestedTransferQuantity,
                                       RequisitionedQuantity = u.RequisitionedQuantity,
                                       PackingQuantity = u.PackingQuantity,
                                       AverageUsage = u.AverageUsage,
                                       Turns = u.Turns,
                                       OnHandQuantity = u.OnHandQuantity,
                                       CriticalQuantity = u.CriticalQuantity,
                                       MinimumQuantity = u.MinimumQuantity,
                                       MaximumQuantity = u.MaximumQuantity,
                                       WeightPerPiece = u.WeightPerPiece,
                                       ItemUniqueNumber = u.ItemUniqueNumber,
                                       IsPurchase = u.IsPurchase ?? false,
                                       IsTransfer = u.IsTransfer ?? false,
                                       DefaultLocation = u.DefaultLocation,
                                       InventoryClassification = u.InventoryClassification,
                                       SerialNumberTracking = u.SerialNumberTracking,
                                       LotNumberTracking = u.LotNumberTracking,
                                       DateCodeTracking = u.DateCodeTracking,
                                       ItemType = u.ItemType,
                                       ImagePath = u.ImagePath,
                                       UDF1 = u.UDF1,
                                       UDF2 = u.UDF2,
                                       UDF3 = u.UDF3,
                                       UDF4 = u.UDF4,
                                       UDF5 = u.UDF5,
                                       GUID = u.GUID,
                                       Created = u.Created,
                                       Updated = u.Updated,
                                       CreatedBy = u.CreatedBy,
                                       LastUpdatedBy = u.LastUpdatedBy,
                                       IsDeleted = u.IsDeleted,
                                       IsArchived = u.IsArchived,
                                       CompanyID = u.CompanyID,
                                       Room = u.Room,
                                       IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                                       IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
                                       IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                                       IsBuildBreak = u.IsBuildBreak,
                                       BondedInventory = u.BondedInventory,
                                       IsBOMItem = u.IsBOMItem,
                                       RefBomId = u.RefBomId,
                                       OnOrderInTransitQuantity = u.OnOrderInTransitQuantity,
                                       IsAllowOrderCostuom = u.IsAllowOrderCostuom
                                   }).FirstOrDefault();
            }
            return objItemMaterDTO;


        }

        public ItemMasterDTO GetRecordByItemGUIDLight(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {

            ItemMasterDTO objItemMaterDTO = new ItemMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objItemMaterDTO = (from u in context.ItemMasters
                                   where (u.IsDeleted ?? false) == false && u.Room == RoomID && u.CompanyID == CompanyID && u.GUID == ItemGUID
                                   select new ItemMasterDTO
                                   {
                                       ID = u.ID,
                                       ItemNumber = u.ItemNumber,
                                       ManufacturerID = u.ManufacturerID,
                                       ManufacturerNumber = u.ManufacturerNumber,
                                       SupplierID = u.SupplierID,
                                       SupplierPartNo = u.SupplierPartNo,
                                       UPC = u.UPC,
                                       UNSPSC = u.UNSPSC,
                                       Description = u.Description,
                                       LongDescription = u.LongDescription,
                                       CategoryID = u.CategoryID,
                                       GLAccountID = u.GLAccountID,
                                       UOMID = u.UOMID,
                                       PricePerTerm = u.PricePerTerm,
                                       CostUOMID = u.CostUOMID,
                                       DefaultReorderQuantity = u.DefaultReorderQuantity,
                                       DefaultPullQuantity = u.DefaultPullQuantity,
                                       Cost = u.Cost,
                                       Markup = u.Markup,
                                       SellPrice = u.SellPrice,
                                       ExtendedCost = u.ExtendedCost,
                                       AverageCost = u.AverageCost,
                                       LeadTimeInDays = u.LeadTimeInDays,
                                       Link1 = u.Link1,
                                       Link2 = u.Link2,
                                       Trend = u.Trend,
                                       Taxable = u.Taxable,
                                       Consignment = u.Consignment,
                                       StagedQuantity = u.StagedQuantity,
                                       InTransitquantity = u.InTransitquantity,
                                       OnOrderQuantity = u.OnOrderQuantity,
                                       OnReturnQuantity = u.OnReturnQuantity,
                                       OnTransferQuantity = u.OnTransferQuantity,
                                       SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                       SuggestedTransferQuantity = u.SuggestedTransferQuantity,
                                       RequisitionedQuantity = u.RequisitionedQuantity,
                                       PackingQuantity = u.PackingQuantity,
                                       AverageUsage = u.AverageUsage,
                                       Turns = u.Turns,
                                       OnHandQuantity = u.OnHandQuantity,
                                       CriticalQuantity = u.CriticalQuantity,
                                       MinimumQuantity = u.MinimumQuantity,
                                       MaximumQuantity = u.MaximumQuantity,
                                       WeightPerPiece = u.WeightPerPiece,
                                       ItemUniqueNumber = u.ItemUniqueNumber,
                                       IsPurchase = u.IsPurchase ?? false,
                                       IsTransfer = u.IsTransfer ?? false,
                                       DefaultLocation = u.DefaultLocation,
                                       InventoryClassification = u.InventoryClassification,
                                       SerialNumberTracking = u.SerialNumberTracking,
                                       LotNumberTracking = u.LotNumberTracking,
                                       DateCodeTracking = u.DateCodeTracking,
                                       ItemType = u.ItemType,
                                       ImagePath = u.ImagePath,
                                       UDF1 = u.UDF1,
                                       UDF2 = u.UDF2,
                                       UDF3 = u.UDF3,
                                       UDF4 = u.UDF4,
                                       UDF5 = u.UDF5,
                                       GUID = u.GUID,
                                       Created = u.Created,
                                       Updated = u.Updated,
                                       CreatedBy = u.CreatedBy,
                                       LastUpdatedBy = u.LastUpdatedBy,
                                       IsDeleted = u.IsDeleted,
                                       IsArchived = u.IsArchived,
                                       CompanyID = u.CompanyID,
                                       Room = u.Room,
                                       IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                                       IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
                                       IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                                       IsBuildBreak = u.IsBuildBreak,
                                       BondedInventory = u.BondedInventory,
                                       IsBOMItem = u.IsBOMItem,
                                       RefBomId = u.RefBomId,
                                       OnOrderInTransitQuantity = u.OnOrderInTransitQuantity,
                                       IsPackslipMandatoryAtReceive = u.IsPackslipMandatoryAtReceive,
                                       OrderUOMID = u.OrderUOMID,
                                       IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                   }).FirstOrDefault();
            }
            return objItemMaterDTO;


        }

        public Int64 getItemStockOutHistory(long CompanyId, long RoomId, string ItemGuids, DateTime FromDate)
        {
            Int32 StockOutCount = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string query = "select count(ID) as StockOuts from ItemStockOutHistory where RoomId = " + RoomId + " and CompanyId = " + CompanyId + " and StockOutDate >= '" + FromDate.ToString("yyyy-MM-dd") + "' and StockOutDate <= '" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "' and ItemGuid in (" + ItemGuids + ")";
                StockOutCount = context.ExecuteStoreQuery<Int32>(query).FirstOrDefault();
            }
            return StockOutCount;
        }
    }
}
