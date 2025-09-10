using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace eTurnsWeb.Controllers
{
    public class EmailLinkController : eTurnsControllerBase
    {
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];

        private void UserLoginForEmailSend(long UserID)
        {
            eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.GetUserByIdForServiceNormal(UserID);
            if (objDTO != null)
            {
                SessionHelper.UserID = objDTO.ID;
                SessionHelper.RoleID = objDTO.RoleID;
                SessionHelper.UserType = objDTO.UserType;
                SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                SessionHelper.CompanyID = objDTO.CompanyID;
                SessionHelper.EnterPriseDBName = objDTO.EnterpriseDbName;
                SessionHelper.RoomID = objDTO.Room ?? 0;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.LoggedinUser = objDTO;
                SessionHelper.CompanyName = objDTO.CompanyName;

                MasterController objMaster = new MasterController();
                objMaster.SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);
            }
        }

        /// <summary>
        /// OrderStatus
        /// </summary>
        /// <param name="eKey"></param>
        /// <returns></returns>
        public ActionResult OrderStatus(string eKey)
        {
            string ResourceFilePath = GetResourceBaseFileFullPath("ResCommon", "en-US");
            string strMsg = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotValidData", ResourceFilePath, 0, 0, 0, "ResCommon", "en-US");
            if (!string.IsNullOrEmpty(eKey))
            {
                string data = StringCipher.Decrypt(eKey);
                string[] strData = data.Split('^');

                if (strData[7] == "APRV")
                {
                    strMsg = OrderApprove(strData);
                }
                else if (strData[7] == "RJKT")
                {
                    strMsg = OrderReject(strData);
                }
            }

            return View("OrderApproved", new CommonDTO() { Text = strMsg });
        }

        /// <summary>
        /// OrderApprove
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private string OrderApprove(string[] strData)
        {
            if (strData != null && strData.Length > 0)
            {
                //UserLoginForEmailSend(Int64.Parse(strData[3]));

                Int64 RoomID = 0;
                Int64 CompanyID = 0;
                EnterpriseDTO objEnterpriseDTO = null;
                long EntID = 0;
                long.TryParse(strData[4], out EntID);
                string EmailAddr = strData[6];
                UserMasterDTO objApprover = new UserMasterDTO();

                long.TryParse(strData[1], out RoomID);

                long.TryParse(strData[2], out CompanyID);

                if (EntID > 0)
                {
                    eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                    objEnterpriseDTO = objEnterpriseMasterDAL.GetEnterpriseByIdPlain(EntID);
                }
                if (!string.IsNullOrWhiteSpace(EmailAddr))
                {
                    UserMasterDAL objUserMasterDAL = new UserMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                    objApprover = objUserMasterDAL.GetUserDetailsByEmail(EmailAddr);
                    if (objApprover != null)
                    {
                        EmailAddr = objApprover.UserName;
                    }
                    else
                    {
                        objApprover = new UserMasterDTO();
                        objApprover.ID = SessionHelper.UserID;
                    }
                }

                OrderMasterDAL obj = new OrderMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                OrderMasterDTO objDTO = obj.GetOrderByIdNormal(long.Parse(strData[0]));
                var cultureCode = "en-US";
                var regionalSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
                if (regionInfo != null)
                {
                    cultureCode = regionInfo.CultureCode;
                }

                if (objDTO.OrderStatus == (int)eTurns.DTO.OrderStatus.Submitted)
                {
                    objDTO.OrderStatus = (int)eTurns.DTO.OrderStatus.Transmitted;
                    if (objDTO.Supplier.GetValueOrDefault(0) > 0)
                    {
                        SupplierMasterDTO objSupDTO = new SupplierMasterDAL(objEnterpriseDTO.EnterpriseDBName).GetSupplierByIDPlain(objDTO.Supplier.GetValueOrDefault(0));
                        if (objSupDTO != null && objSupDTO.IsSendtoVendor)
                            objDTO.OrderStatus = (int)eTurns.DTO.OrderStatus.Approved;
                    }

                    objDTO.LastUpdatedBy = objApprover.ID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;

                    if (objDTO.OrderStatus == (int)eTurns.DTO.OrderStatus.Approved
                        || objDTO.OrderStatus == (int)eTurns.DTO.OrderStatus.Transmitted)
                    {
                        objDTO.ApproverID = objDTO.LastUpdatedBy;
                    }
                    obj.Edit(objDTO);

                    OrderDetailsDAL objOrdDetailDAL = new OrderDetailsDAL(objEnterpriseDTO.EnterpriseDBName);
                    IEnumerable<OrderDetailsDTO> OrdDetails = objOrdDetailDAL.GetOrderDetailByOrderGUIDPlain(objDTO.GUID, RoomID, CompanyID);
                    long SessionUserId = objApprover.ID;
                    if (OrdDetails != null && OrdDetails.Count() > 0)
                    {
                        foreach (var item in OrdDetails)
                        {
                            item.LastUpdated = DateTimeUtility.DateTimeNow;
                            item.LastUpdatedBy = SessionHelper.UserID;
                            item.ApprovedQuantity = item.RequestedQuantity.GetValueOrDefault(0);
                            objOrdDetailDAL.Edit(item, SessionUserId, EntID);
                        }
                    }

                    OrderController objOrdContlr = new OrderController();
                    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                    eTurns.DAL.UserMasterDAL userMasterDAL = new eTurns.DAL.UserMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                    eTurnsMaster.DAL.UserMasterDAL objReqRequesterUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                    UserMasterDTO ReqUser = new UserMasterDTO();

                    string OrdRequesterEmailAddress = "";
                    string OrdApproverEmailAddress = "";
                    if (objDTO.RequesterID.GetValueOrDefault(0) > 0)
                    {
                        ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(objDTO.RequesterID.GetValueOrDefault(0));
                        if (ReqUser == null)
                        {
                            ReqUser = userMasterDAL.GetUserByIdPlain(objDTO.RequesterID.GetValueOrDefault(0));
                        }
                        if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                        {
                            OrdRequesterEmailAddress = ReqUser.Email;
                        }
                    }
                    if (objDTO.ApproverID.GetValueOrDefault(0) > 0)
                    {
                        ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(objDTO.ApproverID.GetValueOrDefault(0));
                        if (ReqUser == null)
                        {
                            ReqUser = userMasterDAL.GetUserByIdPlain(objDTO.ApproverID.GetValueOrDefault(0));
                        }
                        if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                        {
                            OrdApproverEmailAddress = ReqUser.Email;
                        }
                    }

                    string ResourceFilePath = GetResourceFileFullPath("ResOrder", cultureCode, objEnterpriseDTO.ID, CompanyID);
                    string msgYourOrderHasBeenApproved = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourOrderHasBeenApproved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    string approved = ResourceRead.GetResourceValueByKeyAndFullFilePath("Approved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    SendMailForApprovedOrReject(objDTO, objEnterpriseDTO, approved, RoomID, CompanyID, OrdRequesterEmailAddress, OrdApproverEmailAddress);

                    string ReturnString = msgYourOrderHasBeenApproved;
                    if (objDTO.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                    {
                        var msgYourReturnOrderHasBeenApproved = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourReturnOrderHasBeenApproved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                        ReturnString = msgYourReturnOrderHasBeenApproved;
                    }
                    if (objDTO.OrderStatus == (int)eTurns.DTO.OrderStatus.Approved)
                    {
                        SupplierMasterDTO objSupplier = objSupplierMasterDAL.GetSupplierByIDPlain(objDTO.Supplier ?? 0);
                        objOrdContlr.SendMailToSupplier(objSupplier, objDTO);
                    }                    

                    return ReturnString;
                }
                else if (objDTO.OrderStatus == (int)eTurns.DTO.OrderStatus.Approved)
                {
                    string ResourceFilePath = GetResourceFileFullPath("ResOrder", cultureCode, objEnterpriseDTO.ID, CompanyID);
                    string msgYourOrderHasAlreadyBeenApproved = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourOrderHasAlreadyBeenApproved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    string ReturnString = msgYourOrderHasAlreadyBeenApproved;
                    if (objDTO.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                    {
                        ReturnString = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourReturnOrderHasAlreadyBeenApproved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);

                    }
                    return ReturnString;
                }
                else
                {
                    string ResourceFilePath = GetResourceFileFullPath("ResOrder", cultureCode, objEnterpriseDTO.ID, CompanyID);
                    string msgYourOrderIsNotValidToApproval = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourOrderIsNotValidToApproval", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    string ReturnString = msgYourOrderIsNotValidToApproval;
                    if (objDTO.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                    {
                        ReturnString = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourReturnOrderIsNotValidToApproval", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);

                    }
                    return ReturnString;
                }
            }
            else
            {
                string ResourceFilePath = GetResourceBaseFileFullPath("ResCommon", "en-US");
                string msgNotValidData = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotValidData", ResourceFilePath, 0, 0, 0, "ResCommon", "en-US");
                return msgNotValidData;
            }

        }

        public void SendMailForApprovedOrReject(OrderMasterDTO objOrder, EnterpriseDTO objEnterpriseDTO, string AprvRejString, Int64 RoomID, Int64 CompanyID, string OrdRequesterEmailAddress, string OrdApproverEmailAddress)
        {
            List<eMailAttachmentDTO> lstAttachments = new List<eMailAttachmentDTO>();
            eTurnsWeb.Helper.AlertMail objAlertMail = new Helper.AlertMail();
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            NotificationDAL objNotificationDAL = null;
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            try
            {
                if ((objOrder.OrderType ?? 0) == (int)OrderType.Order)
                {
                    objNotificationDAL = new NotificationDAL(objEnterpriseDTO.EnterpriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.OrderApproveReject, RoomID, CompanyID, ResourceHelper.CurrentCult.Name);

                    lstNotifications.ForEach(t =>
                    {
                        if (t.SchedulerParams.ScheduleMode == 5)
                        {
                            lstNotificationsImidiate.Add(t);
                        }
                    });

                    if (lstNotificationsImidiate.Count > 0)
                    {
                        lstNotificationsImidiate.ForEach(t =>
                        {
                            string StrSubject = string.Empty;
                            string strToAddress = t.EmailAddress;
                            if (!string.IsNullOrEmpty(strToAddress))
                            {
                                if (strToAddress.Contains("[Requester]"))
                                {
                                    if (!string.IsNullOrWhiteSpace(OrdRequesterEmailAddress))
                                        strToAddress = strToAddress.Replace("[Requester]", OrdRequesterEmailAddress);
                                    else
                                        strToAddress = strToAddress.Replace("[Requester]", "");
                                }
                                if (strToAddress.Contains("[Approver]"))
                                {
                                    if (!string.IsNullOrWhiteSpace(OrdApproverEmailAddress))
                                        strToAddress = strToAddress.Replace("[Approver]", OrdApproverEmailAddress);
                                    else
                                        strToAddress = strToAddress.Replace("[Approver]", "");
                                }
                                List<string> EmailAddrs = strToAddress.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                EmailAddrs = EmailAddrs.Distinct().ToList();
                                strToAddress = string.Join(",", EmailAddrs);
                                //string strCCAddress = "";
                                MessageBody = new StringBuilder();
                                objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                                if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                                {
                                    objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.First();
                                }
                                if (objEmailTemplateDetailDTO != null)
                                {
                                    MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                                    StrSubject = objEmailTemplateDetailDTO.MailSubject;
                                }
                                else
                                {
                                    return;
                                }
                                if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                                {
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                                    {
                                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objOrder.ReleaseNumber).Replace("@@releaseno@@", objOrder.ReleaseNumber).Replace("@@Releaseno@@", objOrder.ReleaseNumber).Replace("@@ReleaseNo@@", objOrder.ReleaseNumber);
                                    }
                                    if (objEnterpriseDTO != null)
                                    {
                                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                        eTurnsRegionInfo objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
                                        string DateTimeFormat = "MM/dd/yyyy";
                                        DateTime TZDateTimeNow = DateTime.UtcNow;
                                        if (objeTurnsRegionInfoNew != null)
                                        {
                                            DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                                            TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                                        }
                                        if (StrSubject != null && StrSubject.ToLower().Contains("@@date@@"))
                                        {
                                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                                            StrSubject = StrSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                                        }
                                    }
                                }

                                MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                                if (objOrder != null)
                                {

                                    MessageBody.Replace("@@TABLE@@", GetOrderMailBody(objOrder, objEnterpriseDTO.EnterpriseDBName, RoomID, CompanyID, objEnterpriseDTO.ID));
                                }
                                else
                                {
                                    MessageBody.Replace("@@TABLE@@", string.Empty);

                                }
                                MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                                MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                                MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                                MessageBody.Replace("@@APRVREJ@@", AprvRejString);

                                Dictionary<string, string> Params = new Dictionary<string, string>();
                                Params.Add("DataGuids", objOrder.GUID.ToString());
                                lstAttachments = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                                objAlertMail.CreateAlertMail(lstAttachments, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                            }
                        });
                    }
                }
                if ((objOrder.OrderType ?? 0) == (int)OrderType.RuturnOrder)
                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.ReturnOrderApproveReject, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);

                    lstNotifications.ForEach(t =>
                    {
                        if (t.SchedulerParams.ScheduleMode == 5)
                        {
                            lstNotificationsImidiate.Add(t);
                        }
                    });

                    if (lstNotificationsImidiate.Count > 0)
                    {
                        lstNotificationsImidiate.ForEach(t =>
                        {
                            string StrSubject = t.EmailTemplateDetail.MailSubject;
                            string strToAddress = t.EmailAddress;
                            if (!string.IsNullOrEmpty(strToAddress))
                            {
                                if (strToAddress.Contains("[Requester]"))
                                {
                                    if (!string.IsNullOrWhiteSpace(OrdRequesterEmailAddress))
                                        strToAddress = strToAddress.Replace("[Requester]", OrdRequesterEmailAddress);
                                    else
                                        strToAddress = strToAddress.Replace("[Requester]", "");
                                }
                                if (strToAddress.Contains("[Approver]"))
                                {
                                    if (!string.IsNullOrWhiteSpace(OrdApproverEmailAddress))
                                        strToAddress = strToAddress.Replace("[Approver]", OrdApproverEmailAddress);
                                    else
                                        strToAddress = strToAddress.Replace("[Approver]", "");
                                }
                                List<string> EmailAddrs = strToAddress.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                EmailAddrs = EmailAddrs.Distinct().ToList();
                                strToAddress = string.Join(",", EmailAddrs);
                                //string strCCAddress = "";
                                MessageBody = new StringBuilder();
                                objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                                if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                                {
                                    objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.First();
                                }
                                if (objEmailTemplateDetailDTO != null)
                                {
                                    MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                                    StrSubject = objEmailTemplateDetailDTO.MailSubject;
                                }
                                else
                                {
                                    return;
                                }
                                if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                                {
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                                    {
                                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objOrder.ReleaseNumber).Replace("@@releaseno@@", objOrder.ReleaseNumber).Replace("@@Releaseno@@", objOrder.ReleaseNumber).Replace("@@ReleaseNo@@", objOrder.ReleaseNumber);
                                    }
                                    if (objEnterpriseDTO != null)
                                    {
                                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                        eTurnsRegionInfo objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                                        string DateTimeFormat = "MM/dd/yyyy";
                                        DateTime TZDateTimeNow = DateTime.UtcNow;
                                        if (objeTurnsRegionInfoNew != null)
                                        {
                                            DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                                            TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                                        }
                                        if (StrSubject != null && StrSubject.ToLower().Contains("@@date@@"))
                                        {
                                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                                            StrSubject = StrSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                                        }
                                    }
                                }
                                MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                                //MessageBody.Replace("@@TABLE@@", new OrderController().GetMailBody(objOrder));
                                MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                                MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                                MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                                MessageBody.Replace("@@APRVREJ@@", AprvRejString);

                                Dictionary<string, string> Params = new Dictionary<string, string>();
                                Params.Add("DataGuids", objOrder.GUID.ToString());
                                lstAttachments = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                                objAlertMail.CreateAlertMail(lstAttachments, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                            }
                        });
                    }
                }
            }
            finally
            {
                MessageBody = null;
                objNotificationDAL = null;
                objEmailTemplateDetailDTO = null;
            }
        }
        public string GetOrderMailBody(OrderMasterDTO obj, string DBName, Int64 RoomID, Int64 CompanyID, long EnterpriseId)
        {
            int PriseSelectionOption = 1;
            RoomDAL objRoomDAL = new RoomDAL(DBName);
            RoomModuleSettingsDTO objRoomModuleSettingsDTO = new RoomModuleSettingsDTO();
            objRoomModuleSettingsDTO = objRoomDAL.GetRoomModuleSettings(CompanyID, RoomID, (long)SessionHelper.ModuleList.Orders);

            if (objRoomModuleSettingsDTO != null && objRoomModuleSettingsDTO.ID > 0)
            {
                PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption.GetValueOrDefault(0);
            }

            var cultureCode = "en-US";
            var regionalSettingDAL = new RegionSettingDAL(DBName);
            var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
            if (regionInfo != null)
            {
                cultureCode = regionInfo.CultureCode;
            }
            string orderResourceFilePath = GetResourceFileFullPath("ResOrder", cultureCode, EnterpriseId, CompanyID);
            string itemResourceFilePath = GetResourceFileFullPath("ResItemMaster", cultureCode, EnterpriseId, CompanyID);
            string orderCostResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderCost", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string orderPriceResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderPrice", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string OrderNumberResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderNumber", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string RequiredDateResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequiredDate", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string OrderStatusResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderStatus", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string RequestedQuantityResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequestedQuantity", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string ReturnOrderNumberResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnOrderNumber", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string ReturnDateResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnDate", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string ReturnOrderStatusResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnOrderStatus", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string ReturnQuantityResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnQuantity", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string CommentResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("Comment", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string ShippingMethodResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ShippingMethod", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string SupplierResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("Supplier", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string BinResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("Bin", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string ItemNumberResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNumber", itemResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResItemMaster", cultureCode);
            string DescriptionResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("Description", itemResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResItemMaster", cultureCode);

            string OrderCostTitle = orderCostResourceValue;
            OrderCostTitle = (PriseSelectionOption == 1 ? orderPriceResourceValue : orderCostResourceValue);

            string mailBody = "";
            eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(DBName);
            List<UDFDTO> DataFromDB = objUDFDAL.GetNonDeletedUDFsByUDFTableNamePlain("OrderMaster", RoomID, CompanyID);
            string suppliername = "";
            string OrdNumber = OrderNumberResourceValue;
            string ReqDateCap = RequiredDateResourceValue;
            string OrdStatus = OrderStatusResourceValue;
            string OrdReqQty = RequestedQuantityResourceValue;

            if (obj.OrderType == (int)OrderType.RuturnOrder)
            {
                OrdNumber = ReturnOrderNumberResourceValue;
                ReqDateCap = ReturnDateResourceValue;
                OrdStatus = ReturnOrderStatusResourceValue;
                OrdReqQty = ReturnQuantityResourceValue;
            }

            if (obj.Supplier != null && obj.Supplier > 0)
            {
                SupplierMasterDTO objSupplierDTO = new SupplierMasterDAL(DBName).GetSupplierByIDPlain(Int64.Parse(Convert.ToString(obj.Supplier)));
                if (objSupplierDTO != null && objSupplierDTO.ID > 0)
                {
                    suppliername = objSupplierDTO.SupplierName;
                }
            }
            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                <tr>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label style=""font-weight: bold;"">
                                        " + OrdNumber + @": </label>
                                    <label style=""font-weight: bold;"">
                                        " + obj.OrderNumber + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + CommentResourceValue + @": </label>
                                    <label>
                                        " + obj.Comment + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + ShippingMethodResourceValue + @": </label>
                                    <label>
                                        " + obj.ShipViaName + @"</label>
                                </td>
                            </tr>";

            mailBody = mailBody + @"
                            <tr>
                                <td>
                                    <label>
                                        " + OrderCostTitle + @": </label>
                                    <label>
                                        " + (Convert.ToString(PriseSelectionOption) == "1" ? obj.OrderPrice.ToString("N" + SessionHelper.CurrencyDecimalDigits) : obj.OrderCost.GetValueOrDefault(0).ToString("N" + SessionHelper.CurrencyDecimalDigits)) + @"</label>                                        
                                </td>
                            </tr>";
            if (DataFromDB != null && DataFromDB.Count > 0)
            {
                for (int i = 0; i < DataFromDB.Count; i++)
                {
                    mailBody = mailBody + @"
                            <tr>
                                <td>
                                    <label>
                                        &nbsp; </label>
                                    <label>
                                        &nbsp;</label>
                                </td>
                            </tr>";
                }
            }

            mailBody = mailBody + @"
                        </table>
                    </td>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label>
                                       " + ReqDateCap + @": </label>
                                    <label>
                                        " + obj.RequiredDate.ToString(SessionHelper.DateTimeFormat) + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + SupplierResourceValue + @": </label>
                                    <label>
                                        " +
                                          suppliername
                                          + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + OrdStatus + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(OrderStatus), obj.OrderStatus.ToString()).ToString() + @"</label>
                                </td>
                            </tr>";


            if (DataFromDB != null && DataFromDB.Count > 0)
            {
                for (int i = 0; i < DataFromDB.Count; i++)
                {
                    if (DataFromDB[i].UDFColumnName == "UDF" + (i + 1))
                    {

                        string val = string.Empty;// ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, true, (OtherFromeTurns), ForEnterPriseSetup);
                        val = ResourceUtils.GetResource("ResOrder", DataFromDB[i].UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            DataFromDB[i].UDFDisplayColumnName = val;
                        else
                            DataFromDB[i].UDFDisplayColumnName = DataFromDB[i].UDFColumnName;

                        System.Reflection.PropertyInfo info = obj.GetType().GetProperty("UDF" + (i + 1));
                        string udfValue = (string)info.GetValue(obj, null);
                        if (string.IsNullOrEmpty(udfValue))
                            udfValue = "";

                        mailBody = mailBody + @"
                            <tr>
                                <td>
                                    <label>
                                        " + DataFromDB[i].UDFDisplayColumnName + @": </label>
                                    <label>
                                        " + udfValue + @"</label>
                                </td>
                            </tr>";
                    }
                }
            }



            mailBody = mailBody + @"
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan=""2"" style=""width: 99%"">
                        <table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ItemNumberResourceValue + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + BinResourceValue + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + DescriptionResourceValue + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + OrdReqQty + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ReqDateCap + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
            ";
            string trs = "";
            if (obj.OrderListItem == null || obj.OrderListItem.Count <= 0)
            {
                OrderDetailsDAL objOrdDetailDAL = new OrderDetailsDAL(DBName);
                obj.OrderListItem = objOrdDetailDAL.GetOrderDetailByOrderGUIDFull(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
            }

            if (obj.OrderListItem != null && obj.OrderListItem.Count > 0)
            {

                foreach (var item in obj.OrderListItem)
                {
                    string binname = "&nbsp;";
                    string ReqQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    if (item.Bin != null && item.Bin > 0)
                        //binname = new BinMasterController().GetRecord(Int64.Parse(Convert.ToString(item.Bin)), RoomID, CompanyID, false, false).BinNumber;
                        binname = new BinMasterDAL(DBName).GetBinByID(Int64.Parse(Convert.ToString(item.Bin)), RoomID, CompanyID).BinNumber;
                    //binname = new BinMasterDAL(DBName).GetItemLocation(RoomID, CompanyID, false, false,Guid.Empty, Int64.Parse(Convert.ToString(item.Bin)), null,null).FirstOrDefault().BinNumber;

                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        ReqQty = item.RequestedQuantity.ToString();


                    if (obj.OrderStatus == (int)eTurns.DTO.OrderStatus.Approved)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            ReqQty = item.ApprovedQuantity.ToString();
                    }

                    if (item.RequiredDate != null && item.RequiredDate.HasValue)
                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.DateTimeFormat);


                    trs += @"<tr>
                        <td>
                            " + item.ItemNumber + @"
                        </td>
                        <td>
                            " + binname + @"
                        </td>
                        <td>
                            " + item.ItemDescription + @"
                        </td>
                        <td>
                            " + ReqQty + @"
                        </td>
                        <td>
                            " + ReqDate + @"
                        </td>
                    </tr>";
                }
            }
            else
            {
                string NoItemForThisOrderResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("NoItemForThisOrder", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);

                trs += @"<tr>
                        <td colspan=""4"" style=""text-align:center"">
                           " + NoItemForThisOrderResourceValue + @"
                        </td>
                    </tr>";
            }
            mailBody = mailBody.Replace("##TRS##", trs);

            return mailBody;
        }
        public ActionResult ToolAssetOrderStatus(string eKey)
        {
            string ResourceFilePath = GetResourceBaseFileFullPath("ResCommon", "en-US");
            string strMsg = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotValidData", ResourceFilePath, 0, 0, 0, "ResCommon", "en-US");

            if (!string.IsNullOrEmpty(eKey))
            {
                string data = StringCipher.Decrypt(eKey);
                string[] strData = data.Split('^');

                if (strData[7] == "APRV")
                {
                    strMsg = ToolAssetOrderApprove(strData);
                }
                else if (strData[7] == "RJKT")
                {
                    strMsg = ToolAssetOrderReject(strData);
                }
            }

            return View("OrderApproved", new CommonDTO() { Text = strMsg });
        }
        /// <summary>
        /// OrderApprove
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private string ToolAssetOrderApprove(string[] strData)
        {
            if (strData != null && strData.Length > 0)
            {
                //UserLoginForEmailSend(Int64.Parse(strData[3]));

                Int64 RoomID = 0;
                Int64 CompanyID = 0;
                EnterpriseDTO objEnterpriseDTO = null;
                long EntID = 0;
                long.TryParse(strData[4], out EntID);
                string EmailAddr = strData[6];
                UserMasterDTO objApprover = new UserMasterDTO();

                long.TryParse(strData[1], out RoomID);

                long.TryParse(strData[2], out CompanyID);

                if (EntID > 0)
                {
                    eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                    objEnterpriseDTO = objEnterpriseMasterDAL.GetEnterpriseByIdPlain(EntID);
                }
                if (!string.IsNullOrWhiteSpace(EmailAddr))
                {
                    UserMasterDAL objUserMasterDAL = new UserMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                    objApprover = objUserMasterDAL.GetUserDetailsByEmail(EmailAddr);
                    if (objApprover != null)
                    {
                        EmailAddr = objApprover.UserName;
                    }
                    else
                    {
                        objApprover = new UserMasterDTO();
                        objApprover.ID = SessionHelper.UserID;
                    }
                }

                ToolAssetOrderMasterDAL obj = new ToolAssetOrderMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                ToolAssetOrderMasterDTO objDTO = obj.GetRecord(Int64.Parse(strData[0]), RoomID, CompanyID);
                var cultureCode = "en-US";
                var regionalSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
                if (regionInfo != null)
                {
                    cultureCode = regionInfo.CultureCode;
                }

                if (objDTO.OrderStatus == (int)eTurns.DTO.ToolAssetOrderStatus.Submitted)
                {
                    objDTO.OrderStatus = (int)eTurns.DTO.ToolAssetOrderStatus.Transmitted;
                    //if (objDTO.Supplier.GetValueOrDefault(0) > 0)
                    //{
                    //    SupplierMasterDTO objSupDTO = new SupplierMasterDAL(objEnterpriseDTO.EnterpriseDBName).GetRecord(objDTO.Supplier.GetValueOrDefault(0), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), null);
                    //    if (objSupDTO.IsSendtoVendor)
                    //        objDTO.OrderStatus = (int)eTurns.DTO.OrderStatus.Approved;
                    //}

                    objDTO.LastUpdatedBy = objApprover.ID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    obj.Edit(objDTO);

                    ToolAssetOrderDetailsDAL objOrdDetailDAL = new ToolAssetOrderDetailsDAL(objEnterpriseDTO.EnterpriseDBName);
                    IEnumerable<ToolAssetOrderDetailsDTO> OrdDetails = objOrdDetailDAL.GetOrderedRecord(objDTO.GUID, RoomID, CompanyID);
                    if (OrdDetails != null && OrdDetails.Count() > 0)
                    {
                        foreach (var item in OrdDetails)
                        {
                            item.LastUpdated = DateTimeUtility.DateTimeNow;
                            item.LastUpdatedBy = SessionHelper.UserID;
                            item.ApprovedQuantity = item.RequestedQuantity.GetValueOrDefault(0);
                            objOrdDetailDAL.Edit(item);
                        }
                    }

                    string ResourceFilePath = GetResourceFileFullPath("ResOrder", cultureCode, objEnterpriseDTO.ID, CompanyID);
                    string msgYourOrderHasBeenApproved = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourOrderHasBeenApproved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    string approved = ResourceRead.GetResourceValueByKeyAndFullFilePath("Approved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    SendMailForToolAssetApprovedOrReject(objDTO, objEnterpriseDTO, approved, RoomID, CompanyID);
                    string ReturnString = msgYourOrderHasBeenApproved;
                    return ReturnString;
                }
                else if (objDTO.OrderStatus == (int)eTurns.DTO.ToolAssetOrderStatus.Approved)
                {
                    string ResourceFilePath = GetResourceFileFullPath("ResOrder", cultureCode, objEnterpriseDTO.ID, CompanyID);
                    string msgYourOrderHasAlreadyBeenApproved = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourOrderHasAlreadyBeenApproved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    string ReturnString = msgYourOrderHasAlreadyBeenApproved;
                    return ReturnString;
                }
                else
                {
                    string ResourceFilePath = GetResourceFileFullPath("ResOrder", cultureCode, objEnterpriseDTO.ID, CompanyID);
                    string msgYourOrderIsNotValidToApproval = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourOrderIsNotValidToApproval", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    string ReturnString = msgYourOrderIsNotValidToApproval;
                    return ReturnString;
                }
            }
            else
            {
                string ResourceFilePath = GetResourceBaseFileFullPath("ResCommon", "en-US");
                string msgInvalidData = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotValidData", ResourceFilePath, 0, 0, 0, "ResCommon", "en-US");
                return msgInvalidData;
            }

        }

        public void SendMailForToolAssetApprovedOrReject(ToolAssetOrderMasterDTO objOrder, EnterpriseDTO objEnterpriseDTO, string AprvRejString, Int64 RoomID, Int64 CompanyID)
        {
            List<eMailAttachmentDTO> lstAttachments = new List<eMailAttachmentDTO>();
            eTurnsWeb.Helper.AlertMail objAlertMail = new Helper.AlertMail();
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            NotificationDAL objNotificationDAL = null;
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            try
            {
                if ((objOrder.OrderType ?? 0) == (int)ToolAssetOrderType.Order)
                {
                    objNotificationDAL = new NotificationDAL(objEnterpriseDTO.EnterpriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.ToolAssetOrderApproveReject, RoomID, CompanyID, ResourceHelper.CurrentCult.Name);

                    lstNotifications.ForEach(t =>
                    {
                        if (t.SchedulerParams.ScheduleMode == 5)
                        {
                            lstNotificationsImidiate.Add(t);
                        }
                    });

                    if (lstNotificationsImidiate.Count > 0)
                    {
                        lstNotificationsImidiate.ForEach(t =>
                        {
                            string StrSubject = string.Empty;
                            string strToAddress = t.EmailAddress;
                            if (!string.IsNullOrEmpty(strToAddress))
                            {
                                //string strCCAddress = "";
                                MessageBody = new StringBuilder();
                                objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                                if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                                {
                                    objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.First();
                                }
                                if (objEmailTemplateDetailDTO != null)
                                {
                                    MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                                    StrSubject = objEmailTemplateDetailDTO.MailSubject;
                                }
                                else
                                {
                                    return;
                                }
                                if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                                {
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.ToolAssetOrderNumber);
                                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                                    {
                                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objOrder.ReleaseNumber).Replace("@@releaseno@@", objOrder.ReleaseNumber).Replace("@@Releaseno@@", objOrder.ReleaseNumber).Replace("@@ReleaseNo@@", objOrder.ReleaseNumber);
                                    }
                                    if (objEnterpriseDTO != null)
                                    {
                                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                        eTurnsRegionInfo objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
                                        string DateTimeFormat = "MM/dd/yyyy";
                                        DateTime TZDateTimeNow = DateTime.UtcNow;
                                        if (objeTurnsRegionInfoNew != null)
                                        {
                                            DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                                            TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                                        }
                                        if (StrSubject != null && StrSubject.ToLower().Contains("@@date@@"))
                                        {
                                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                                            StrSubject = StrSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                                        }
                                    }
                                }

                                MessageBody.Replace("@@ORDERNO@@", objOrder.ToolAssetOrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                                if (objOrder != null)
                                {

                                    MessageBody.Replace("@@TABLE@@", GetToolAssetOrderMailBody(objOrder, objEnterpriseDTO.EnterpriseDBName, RoomID, CompanyID, objEnterpriseDTO.ID));
                                }
                                else
                                {
                                    MessageBody.Replace("@@TABLE@@", string.Empty);

                                }
                                MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                                MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                                MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                                MessageBody.Replace("@@APRVREJ@@", AprvRejString);

                                Dictionary<string, string> Params = new Dictionary<string, string>();
                                Params.Add("DataGuids", objOrder.GUID.ToString());
                                lstAttachments = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                                objAlertMail.CreateAlertMail(lstAttachments, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                            }
                        });
                    }
                }

            }
            finally
            {
                MessageBody = null;
                objNotificationDAL = null;
                objEmailTemplateDetailDTO = null;
            }
        }
        public string GetToolAssetOrderMailBody(ToolAssetOrderMasterDTO obj, string DBName, Int64 RoomID, Int64 CompanyID, long EnterpriseId)
        {
            string mailBody = "";
            eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(DBName);
            List<UDFDTO> DataFromDB = objUDFDAL.GetNonDeletedUDFsByUDFTableNamePlain("ToolAssetOrder", RoomID, CompanyID);
            var cultureCode = "en-US";
            var regionalSettingDAL = new RegionSettingDAL(DBName);
            var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
            if (regionInfo != null)
            {
                cultureCode = regionInfo.CultureCode;
            }
            string orderResourceFilePath = GetResourceFileFullPath("ResOrder", cultureCode, EnterpriseId, CompanyID);
            string toolAssetOrderResourceFilePath = GetResourceFileFullPath("ResToolAssetOrder", cultureCode, EnterpriseId, CompanyID);
            string toolResourceFilePath = GetResourceFileFullPath("ResToolMaster", cultureCode, EnterpriseId, CompanyID);
            string itemMasterResourceFilePath = GetResourceFileFullPath("ResItemMaster", cultureCode, EnterpriseId, CompanyID);
            string orderCostResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderCost", toolAssetOrderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolAssetOrder", cultureCode);
            string OrderNumberResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ToolAssetOrderNumber", toolAssetOrderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolAssetOrder", cultureCode);
            string RequiredDateResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequiredDate", toolAssetOrderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolAssetOrder", cultureCode);
            string OrderStatusResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderStatus", toolAssetOrderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolAssetOrder", cultureCode);
            string RequestedQuantityResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequestedQuantity", toolAssetOrderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolAssetOrder", cultureCode);
            string ReturnOrderNumberResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnOrderNumber", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string ReturnDateResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnDate", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string ReturnOrderStatusResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnOrderStatus", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string ReturnQuantityResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnQuantity", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string CommentResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("Comment", toolAssetOrderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolAssetOrder", cultureCode);
            string BinResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("Bin", toolAssetOrderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolAssetOrder", cultureCode);
            string ToolNameResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ToolName", toolResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolMaster", cultureCode);
            string DescriptionResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("Description", itemMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResItemMaster", cultureCode);

            string OrdNumber = OrderNumberResourceValue;
            string ReqDateCap = RequiredDateResourceValue;
            string OrdStatus = OrderStatusResourceValue;
            string OrdReqQty = RequestedQuantityResourceValue;

            if (obj.OrderType == (int)OrderType.RuturnOrder)
            {
                OrdNumber = ReturnOrderNumberResourceValue;
                ReqDateCap = ReturnDateResourceValue;
                OrdStatus = ReturnOrderStatusResourceValue;
                OrdReqQty = ReturnQuantityResourceValue;
            }

            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                <tr>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label style=""font-weight: bold;"">
                                        " + OrdNumber + @": </label>
                                    <label style=""font-weight: bold;"">
                                        " + obj.ToolAssetOrderNumber + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + CommentResourceValue + @": </label>
                                    <label>
                                        " + obj.Comment + @"</label>
                                </td>
                            </tr>
                            ";

            mailBody = mailBody + @"
                            <tr>
                                <td>
                                    <label>
                                        " + orderCostResourceValue + @": </label>
                                    <label>
                                        " + obj.OrderCost.GetValueOrDefault(0).ToString("N" + SessionHelper.CurrencyDecimalDigits) + @"</label>
                                </td>
                            </tr>";
            if (DataFromDB != null && DataFromDB.Count > 0)
            {
                for (int i = 0; i < DataFromDB.Count; i++)
                {
                    mailBody = mailBody + @"
                            <tr>
                                <td>
                                    <label>
                                        &nbsp; </label>
                                    <label>
                                        &nbsp;</label>
                                </td>
                            </tr>";
                }
            }

            mailBody = mailBody + @"
                        </table>
                    </td>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label>
                                       " + ReqDateCap + @": </label>
                                    <label>
                                        " + obj.RequiredDate.ToString(SessionHelper.DateTimeFormat) + @"</label>
                                </td>
                            </tr>
                            
                            <tr>
                                <td>
                                    <label>
                                        " + OrdStatus + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(ToolAssetOrderStatus), obj.OrderStatus.ToString()).ToString() + @"</label>
                                </td>
                            </tr>";


            if (DataFromDB != null && DataFromDB.Count > 0)
            {
                for (int i = 0; i < DataFromDB.Count; i++)
                {
                    if (DataFromDB[i].UDFColumnName == "UDF" + (i + 1))
                    {

                        string val = string.Empty;// ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, true, (OtherFromeTurns), ForEnterPriseSetup);
                        val = ResourceUtils.GetResource("ResToolAssetOrder", DataFromDB[i].UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            DataFromDB[i].UDFDisplayColumnName = val;
                        else
                            DataFromDB[i].UDFDisplayColumnName = DataFromDB[i].UDFColumnName;

                        System.Reflection.PropertyInfo info = obj.GetType().GetProperty("UDF" + (i + 1));
                        string udfValue = (string)info.GetValue(obj, null);
                        if (string.IsNullOrEmpty(udfValue))
                            udfValue = "";

                        mailBody = mailBody + @"
                            <tr>
                                <td>
                                    <label>
                                        " + DataFromDB[i].UDFDisplayColumnName + @": </label>
                                    <label>
                                        " + udfValue + @"</label>
                                </td>
                            </tr>";
                    }
                }
            }



            mailBody = mailBody + @"
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan=""2"" style=""width: 99%"">
                        <table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ToolNameResourceValue + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + BinResourceValue + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + DescriptionResourceValue + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + OrdReqQty + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ReqDateCap + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
            ";
            string trs = "";
            if (obj.OrderListItem == null || obj.OrderListItem.Count <= 0)
            {
                ToolAssetOrderDetailsDAL objOrdDetailDAL = new ToolAssetOrderDetailsDAL(DBName);
                obj.OrderListItem = objOrdDetailDAL.GetOrderedRecord(obj.GUID, obj.RoomID.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
            }

            if (obj.OrderListItem != null && obj.OrderListItem.Count > 0)
            {

                foreach (var item in obj.OrderListItem)
                {
                    string binname = "&nbsp;";
                    string ReqQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    if (item.ToolBinID != null && item.ToolBinID > 0)
                    {

                        LocationMasterDTO objDTO = new LocationMasterDAL(DBName).GetLocationByIDPlain(Int64.Parse(Convert.ToString(item.ToolBinID)), RoomID, CompanyID);
                        if (objDTO != null && objDTO.ID > 0)
                            binname = objDTO.Location;
                        //binname = new BinMasterDAL(DBName).GetItemLocation(RoomID, CompanyID, false, false,Guid.Empty, Int64.Parse(Convert.ToString(item.Bin)), null,null).FirstOrDefault().BinNumber;
                    }
                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        ReqQty = item.RequestedQuantity.ToString();


                    if (obj.OrderStatus == (int)eTurns.DTO.ToolAssetOrderStatus.Approved)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            ReqQty = item.ApprovedQuantity.ToString();
                    }

                    if (item.RequiredDate != null && item.RequiredDate.HasValue)
                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.DateTimeFormat);


                    trs += @"<tr>
                        <td>
                            " + item.ToolName + @"
                        </td>
                        <td>
                            " + binname + @"
                        </td>
                        <td>
                            " + item.ToolDescription + @"
                        </td>
                        <td>
                            " + ReqQty + @"
                        </td>
                        <td>
                            " + ReqDate + @"
                        </td>
                    </tr>";
                }
            }
            else
            {
                string NoItemForThisOrderResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("NoItemForThisOrder", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
                trs += @"<tr>
                        <td colspan=""4"" style=""text-align:center"">
                           " + NoItemForThisOrderResourceValue + @"
                        </td>
                    </tr>";
            }
            mailBody = mailBody.Replace("##TRS##", trs);

            return mailBody;
        }
        public string GetMailTransferBody(TransferMasterDTO obj, string DBName, Int64 RoomID, Int64 CompanyID, long EnterpriseId)
        {
            var cultureCode = "en-US";
            var regionalSettingDAL = new RegionSettingDAL(DBName);
            var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
            if (regionInfo != null)
            {
                cultureCode = regionInfo.CultureCode;
            }
            string orderResourceFilePath = GetResourceFileFullPath("ResOrder", cultureCode, EnterpriseId, CompanyID);
            string transferResourceFilePath = GetResourceFileFullPath("ResTransfer", cultureCode, EnterpriseId, CompanyID);
            string itemMasterResourceFilePath = GetResourceFileFullPath("ResItemMaster", cultureCode, EnterpriseId, CompanyID);
            string TransferNumberResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("TransferNumber", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode);
            string RequireDateResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequireDate", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode);
            string TransferStatusResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("TransferStatus", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode);
            string RequiredDateResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequiredDate", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string RequestedQuantityResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequestedQuantity", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string CommentResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("Comment", transferResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResTransfer", cultureCode);
            string BinResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("Bin", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string ItemNumberResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNumber", itemMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResItemMaster", cultureCode);

            string mailBody = "";

            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                <tr>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label style=""font-weight: bold;"">
                                        " + TransferNumberResourceValue + @": </label>
                                    <label style=""font-weight: bold;"">
                                        " + obj.TransferNumber + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + CommentResourceValue + @": </label>
                                    <label>
                                        " + obj.Comment + @"</label>
                                </td>
                            </tr>                           
                        </table>
                    </td>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label>
                                       " + RequireDateResourceValue + @": </label>
                                    <label>
                                        " + obj.RequireDate.ToString("MM/dd/yyyy") + @"</label>
                                </td>
                            </tr>                            
                            <tr>
                                <td>
                                    <label>
                                        " + TransferStatusResourceValue + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(TransferStatus), obj.TransferStatus.ToString()).ToString() + @"</label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan=""2"" style=""width: 99%"">
                        <table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ItemNumberResourceValue + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + BinResourceValue + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + RequestedQuantityResourceValue + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + RequiredDateResourceValue + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
            ##TRS##
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
            ";
            string trs = "";
            if (obj.TransferDetailList == null || obj.TransferDetailList.Count <= 0)
            {
                TransferDetailDAL objTransferDetailDAL = new TransferDetailDAL(DBName);
                obj.TransferDetailList = objTransferDetailDAL.GetTransferDetailNormal(obj.GUID, RoomID, CompanyID, SessionHelper.UserSupplierIds);
            }

            if (obj.TransferDetailList != null && obj.TransferDetailList.Count > 0)
            {

                foreach (var item in obj.TransferDetailList)
                {
                    string binname = "&nbsp;";
                    string ReqQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    if (item.Bin != null && item.Bin > 0)
                        //binname = new BinMasterController().GetRecord(Int64.Parse(Convert.ToString(item.Bin)), SessionHelper.RoomID, SessionHelper.CompanyID, false, false).BinNumber;
                        binname = new BinMasterDAL(DBName).GetBinByID(Int64.Parse(Convert.ToString(item.Bin)), RoomID, CompanyID).BinNumber;
                    //                    binname = new BinMasterDAL(DBName).GetItemLocation(RoomID, CompanyID, false, false,Guid.Empty, Int64.Parse(Convert.ToString(item.Bin)), null,null).FirstOrDefault().BinNumber;

                    //if (item.RequestedQuantity != null)
                    ReqQty = item.RequestedQuantity.ToString();

                    if (item.RequiredDate != null)
                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.DateTimeFormat);
                    //ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.CompanyConfig.DateFormatCSharp);


                    trs += @"<tr>
                        <td>
                            " + item.ItemNumber + @"
                        </td>
                        <td>
                            " + binname + @"
                        </td>
                        <td>
                            " + ReqQty + @"
                        </td>
                        <td>
                            " + ReqDate + @"
                        </td>
                    </tr>";
                }
            }
            else
            {
                string NoItemForThisOrderResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("NoItemForThisOrder", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
                trs += @"<tr>
                        <td colspan=""4"" style=""text-align:center"">
                           " + NoItemForThisOrderResourceValue + @"
                        </td>
                    </tr>";
            }
            mailBody = mailBody.Replace("##TRS##", trs);

            return mailBody;
        }
        /// <summary>
        /// OrderReject
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private string OrderReject(string[] strData)
        {
            if (strData != null && strData.Length > 0)
            {
                //UserLoginForEmailSend(Int64.Parse(strData[3]));

                long EntID = 0;
                Int64 CompanyID = 0;
                Int64 RoomID = 0;
                long.TryParse(strData[4], out EntID);
                long.TryParse(strData[1], out RoomID);
                long.TryParse(strData[2], out CompanyID);
                EnterpriseDTO objEnterpriseDTO = null;
                string EmailAddr = strData[6];
                UserMasterDTO objApprover = new UserMasterDTO();
                if (EntID > 0)
                {
                    eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                    objEnterpriseDTO = objEnterpriseMasterDAL.GetEnterpriseByIdPlain(EntID);
                }
                if (!string.IsNullOrWhiteSpace(EmailAddr))
                {
                    UserMasterDAL objUserMasterDAL = new UserMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                    objApprover = objUserMasterDAL.GetUserDetailsByEmail(EmailAddr);
                    if (objApprover != null)
                    {
                        EmailAddr = objApprover.UserName;
                    }
                    else
                    {
                        objApprover = new UserMasterDTO();
                        objApprover.ID = SessionHelper.UserID;
                    }
                }


                OrderMasterDAL obj = new OrderMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                OrderMasterDTO objDTO = obj.GetOrderByIdNormal(long.Parse(strData[0]));
                var cultureCode = "en-US";
                var regionalSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
                if (regionInfo != null)
                {
                    cultureCode = regionInfo.CultureCode;
                }
                string ResourceFilePath = GetResourceFileFullPath("ResOrder", cultureCode, objEnterpriseDTO.ID, CompanyID);
                if (objDTO.OrderStatus == (int)eTurns.DTO.OrderStatus.UnSubmitted)
                {
                    if (objDTO.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                    {
                        string msgReturnOrderHasAlreadyBeenRejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnOrderHasAlreadyBeenRejected", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                        return msgReturnOrderHasAlreadyBeenRejected;
                    }
                    else
                    {
                        string msgOrderHasAlreadyBeenRejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderHasAlreadyBeenRejected", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                        return msgOrderHasAlreadyBeenRejected;
                    }

                }
                else if (objDTO.OrderStatus == (int)eTurns.DTO.OrderStatus.Submitted)
                {
                    objDTO.OrderStatus = (int)eTurns.DTO.OrderStatus.UnSubmitted;
                    objDTO.RejectionReason = "Rejected by email.";
                    objDTO.LastUpdatedBy = objApprover.ID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.ApproverID = objDTO.LastUpdatedBy;
                    obj.Edit(objDTO);

                    eTurns.DAL.UserMasterDAL userMasterDAL = new eTurns.DAL.UserMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                    eTurnsMaster.DAL.UserMasterDAL objReqRequesterUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                    UserMasterDTO ReqUser = new UserMasterDTO();

                    string OrdRequesterEmailAddress = "";
                    string OrdApproverEmailAddress = "";
                    if (objDTO.RequesterID.GetValueOrDefault(0) > 0)
                    {
                        ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(objDTO.RequesterID.GetValueOrDefault(0));
                        if (ReqUser == null)
                        {
                            ReqUser = userMasterDAL.GetUserByIdPlain(objDTO.RequesterID.GetValueOrDefault(0));
                        }
                        if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                        {
                            OrdRequesterEmailAddress = ReqUser.Email;
                        }
                    }
                    if (objDTO.ApproverID.GetValueOrDefault(0) > 0)
                    {
                        ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(objDTO.ApproverID.GetValueOrDefault(0));
                        if (ReqUser == null)
                        {
                            ReqUser = userMasterDAL.GetUserByIdPlain(objDTO.ApproverID.GetValueOrDefault(0));
                        }
                        if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                        {
                            OrdApproverEmailAddress = ReqUser.Email;
                        }
                    }
                    //Rejected
                    string msgOrderHasRejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderHasRejected", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    string rejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("Rejected", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    SendMailForApprovedOrReject(objDTO, objEnterpriseDTO, rejected, Int64.Parse(strData[1]), Int64.Parse(strData[2]), OrdRequesterEmailAddress, OrdApproverEmailAddress);

                    string ReturnString = msgOrderHasRejected;
                    if (objDTO.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                    {
                        string msgReturnOrderHasRejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnOrderHasRejected", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                        ReturnString = msgReturnOrderHasRejected;

                    }

                    return ReturnString;
                }
                else
                {
                    string msgOrderIsNotValidToRejection = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderIsNotValidToRejection", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    string ReturnString = msgOrderIsNotValidToRejection;
                    if (objDTO.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                    {
                        string msgReturnOrderIsNotValidToRejection = ResourceRead.GetResourceValueByKeyAndFullFilePath("ReturnOrderIsNotValidToRejection", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                        ReturnString = msgReturnOrderIsNotValidToRejection;

                    }
                    return ReturnString;
                }
            }
            string ResourceBaseFilePath = GetResourceBaseFileFullPath("ResCommon", "en-US");
            string msgInvalidData = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotValidData", ResourceBaseFilePath, 0, 0, 0, "ResCommon", "en-US");
            return msgInvalidData;
        }
        private string ToolAssetOrderReject(string[] strData)
        {
            if (strData != null && strData.Length > 0)
            {
                //UserLoginForEmailSend(Int64.Parse(strData[3]));

                long EntID = 0;
                Int64 CompanyID = 0;
                Int64 RoomID = 0;
                long.TryParse(strData[4], out EntID);
                long.TryParse(strData[1], out RoomID);
                long.TryParse(strData[2], out CompanyID);
                EnterpriseDTO objEnterpriseDTO = null;
                string EmailAddr = strData[6];
                UserMasterDTO objApprover = new UserMasterDTO();
                if (EntID > 0)
                {
                    eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                    objEnterpriseDTO = objEnterpriseMasterDAL.GetEnterpriseByIdPlain(EntID);
                }
                if (!string.IsNullOrWhiteSpace(EmailAddr))
                {
                    UserMasterDAL objUserMasterDAL = new UserMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                    objApprover = objUserMasterDAL.GetUserDetailsByEmail(EmailAddr);
                    if (objApprover != null)
                    {
                        EmailAddr = objApprover.UserName;
                    }
                    else
                    {
                        objApprover = new UserMasterDTO();
                        objApprover.ID = SessionHelper.UserID;
                    }
                }

                var cultureCode = "en-US";
                var regionalSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
                if (regionInfo != null)
                {
                    cultureCode = regionInfo.CultureCode;
                }
                ToolAssetOrderMasterDAL obj = new ToolAssetOrderMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                ToolAssetOrderMasterDTO objDTO = obj.GetRecord(Int64.Parse(strData[0]), Int64.Parse(strData[1]), Int64.Parse(strData[2]));
                string ResourceFilePath = GetResourceFileFullPath("ResOrder", cultureCode, objEnterpriseDTO.ID, CompanyID);

                if (objDTO.OrderStatus == (int)eTurns.DTO.ToolAssetOrderStatus.UnSubmitted)
                {
                    string msgOrderHasAlreadyBeenRejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderHasAlreadyBeenRejected", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    return msgOrderHasAlreadyBeenRejected;
                }
                else if (objDTO.OrderStatus == (int)eTurns.DTO.ToolAssetOrderStatus.Submitted)
                {
                    objDTO.OrderStatus = (int)eTurns.DTO.ToolAssetOrderStatus.UnSubmitted;
                    objDTO.RejectionReason = "Rejected by email.";
                    objDTO.LastUpdatedBy = objApprover.ID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    obj.Edit(objDTO);

                    string msgOrderHasRejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderHasRejected", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    string rejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("Rejected", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    SendMailForToolAssetApprovedOrReject(objDTO, objEnterpriseDTO, rejected, Int64.Parse(strData[1]), Int64.Parse(strData[2]));
                    string ReturnString = msgOrderHasRejected;

                    return ReturnString;
                }
                else
                {
                    string msgOrderIsNotValidToRejection = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderIsNotValidToRejection", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResOrder", cultureCode);
                    string ReturnString = msgOrderIsNotValidToRejection;

                    return ReturnString;
                }
            }
            string ResourceBaseFilePath = GetResourceBaseFileFullPath("ResCommon", "en-US");
            string msgInvalidData = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotValidData", ResourceBaseFilePath, 0, 0, 0, "ResCommon", "en-US");
            return msgInvalidData;
        }
        /// <summary>
        /// AddExternalUser
        /// </summary>
        /// <param name="objUsers"></param>
        /// <param name="EmailTemplate"></param>
        private static void AddExternalUser(List<UserMasterDTO> objUsers, string EmailTemplate)
        {

            EmailUserConfigurationDAL objExternalUser = new EmailUserConfigurationDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<EmailUserConfigurationDTO> lstExternalUser = objExternalUser.GetAllExternalUserRecords(EmailTemplate, true, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (lstExternalUser != null)
            {
                foreach (EmailUserConfigurationDTO item in lstExternalUser)
                {
                    UserMasterDTO objExtUser = new UserMasterDTO();
                    objExtUser.Email = item.Email;
                    objUsers.Add(objExtUser);
                }
            }
        }

        /// <summary>
        /// AddExternalUser
        /// </summary>
        /// <param name="objUsers"></param>
        /// <param name="EmailTemplate"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="DatabaseName"></param>
        private static void AddExternalUser(List<UserMasterDTO> objUsers, string EmailTemplate, Int64 RoomID, Int64 CompanyID, string DatabaseName)
        {
            EmailUserConfigurationDAL objExternalUser = new EmailUserConfigurationDAL(DatabaseName);
            IEnumerable<EmailUserConfigurationDTO> lstExternalUser = objExternalUser.GetAllExternalUserRecords(EmailTemplate, true, RoomID, CompanyID);
            if (lstExternalUser != null)
            {
                foreach (EmailUserConfigurationDTO item in lstExternalUser)
                {
                    UserMasterDTO objExtUser = new UserMasterDTO();
                    objExtUser.Email = item.Email;
                    objUsers.Add(objExtUser);
                }
            }
        }

        /// <summary>
        /// SendMailForOrderAprOrRej
        /// </summary>
        /// <param name="UserList"></param>
        /// <param name="objOrder"></param>
        /// <param name="AprvRej"></param>
        private void SendMailForOrderAprOrRej(List<UserMasterDTO> UserList, OrderMasterDTO objOrder, string AprvRej)
        {
            StringBuilder MessageBody = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            eTurnsUtility objUtils = null;
            eTurns.DAL.eMailMasterDAL objEmailDAL = null;
            try
            {
                string StrSubject = "Order " + AprvRej;// "Order Approval Request";
                string strTemplateName = "OrderApproveReject";
                string strOrdAppTempName = "OrderApproval";
                if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                {
                    StrSubject = "Return " + StrSubject;
                    strTemplateName = "ReturnOrderApproveReject";
                    strOrdAppTempName = "ReturnOrderApproval";
                }

                string strCCAddress = CommonUtility.GetEmailToAddress(UserList, strOrdAppTempName);
                string strToAddress = CommonUtility.GetEmailToAddress(null, strTemplateName);
                if (string.IsNullOrEmpty(strToAddress))
                    strToAddress = strCCAddress;

                MessageBody = new StringBuilder();
                objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
                objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate(strTemplateName, eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objEmailTemplateDetailDTO != null)
                {
                    MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                    StrSubject = objEmailTemplateDetailDTO.MailSubject;
                }
                else
                {
                    return;
                }

                MessageBody.Replace("@@APRVREJ@@", AprvRej);
                if (objOrder != null)
                {
                    MessageBody.Replace("@@TABLE@@", GetOrderMailBody(objOrder, string.Empty, 0, 0, 0));
                }
                else
                {
                    MessageBody.Replace("@@TABLE@@", string.Empty);
                }
                string DateTimeFormat = "MM/dd/yyyy";
                DateTime TZDateTimeNow = DateTime.UtcNow;
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                DateTimeFormat = "MM/dd/yyyy";

                if (objeTurnsRegionInfo != null)
                {
                    DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                    TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                }
                if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                {
                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                    {
                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objOrder.ReleaseNumber).Replace("@@releaseno@@", objOrder.ReleaseNumber).Replace("@@Releaseno@@", objOrder.ReleaseNumber).Replace("@@ReleaseNo@@", objOrder.ReleaseNumber);
                    }
                    string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                    // EmailSubject = EmailSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                    StrSubject = Regex.Replace(StrSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                    {
                        StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                    }
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                    {
                        StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", SessionHelper.RoomName, RegexOptions.IgnoreCase);
                    }
                    StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

                }


                objUtils = new eTurnsUtility();
                objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
                objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);


                if (MessageBody != null)
                {
                    MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                    MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                    string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                    //EmailBody = EmailBody.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                    MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                    {
                        MessageBody = MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                    }
                    if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                    {
                        MessageBody = MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                    }
                    MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));

                }
                objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), 0, objOrder.CompanyID.GetValueOrDefault(0), objOrder.Room.GetValueOrDefault(0), 0, null, "Web => EmailLink => SendMailForOrderAprOrRej");

            }
            finally
            {
                MessageBody = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                objUtils = null;
                objEmailDAL = null;
            }
        }

        /// <summary>
        /// SendMailForOrderAprOrRej
        /// </summary>
        /// <param name="TOUserList"></param>
        /// <param name="CCUserList"></param>
        /// <param name="objOrder"></param>
        /// <param name="AprvRej"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="DataBaseName"></param>
        private void SendMailForOrderAprOrRej(List<UserMasterDTO> TOUserList, List<UserMasterDTO> CCUserList, OrderMasterDTO objOrder, string AprvRej, Int64 RoomID, Int64 CompanyID, string DataBaseName)
        {
            StringBuilder MessageBody = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            //eTurnsUtility objUtils = null;
            eMailMasterDAL objEmailDAL = null;
            try
            {
                string StrSubject = "Order " + AprvRej;// "Order Approval Request";
                string strTemplateName = "OrderApproveReject";
                string strOrdAppTempName = "OrderApproval";
                if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                {
                    StrSubject = "Return " + StrSubject;
                    strTemplateName = "ReturnOrderApproveReject";
                    strOrdAppTempName = "ReturnOrderApproval";
                }

                string strCCAddress = CommonUtility.GetEmailToAddress(CCUserList, strOrdAppTempName, RoomID, CompanyID, DataBaseName);
                string strToAddress = CommonUtility.GetEmailToAddress(TOUserList, strTemplateName, RoomID, CompanyID, DataBaseName);
                if (string.IsNullOrEmpty(strToAddress))
                    strToAddress = strCCAddress;

                MessageBody = new StringBuilder();
                objEmailTemplateDAL = new EmailTemplateDAL(DataBaseName);
                objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate(strTemplateName, eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString(), RoomID, CompanyID);
                if (objEmailTemplateDetailDTO != null)
                {
                    MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                    StrSubject = objEmailTemplateDetailDTO.MailSubject;
                }
                else
                {
                    return;
                }

                MessageBody.Replace("@@APRVREJ@@", AprvRej);
                if (objOrder != null)
                {
                    MessageBody.Replace("@@TABLE@@", GetOrderMailBody(objOrder, DataBaseName, RoomID, CompanyID, 0));
                }
                else
                {
                    MessageBody.Replace("@@TABLE@@", string.Empty);
                }
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                eTurnsRegionInfo objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
                string DateTimeFormat = "MM/dd/yyyy";
                DateTime TZDateTimeNow = DateTime.UtcNow;
                if (objeTurnsRegionInfoNew != null)
                {
                    DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                    TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                }
                if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                {

                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                    {
                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objOrder.ReleaseNumber).Replace("@@releaseno@@", objOrder.ReleaseNumber).Replace("@@Releaseno@@", objOrder.ReleaseNumber).Replace("@@ReleaseNo@@", objOrder.ReleaseNumber);
                    }



                    string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                    // EmailSubject = EmailSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                    StrSubject = Regex.Replace(StrSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                    {
                        StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                    }
                    if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                    {
                        StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", SessionHelper.RoomName, RegexOptions.IgnoreCase);
                    }

                    StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                }
                MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                //MessageBody.Replace("@@ETURNSLOGO@@", CommonUtility.GeteTurnsImage("http://" + System.Web.HttpContext.Current.Request.Url.Authority, "/Content/images/logo.jpg"));

                //objUtils = new eTurnsUtility();
                //objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
                objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);


                if (MessageBody != null)
                {
                    string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                    //EmailBody = EmailBody.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                    MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                    {
                        MessageBody = MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                    }
                    if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                    {
                        MessageBody = MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                    }
                    MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));

                }
                objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), 0, objOrder.CompanyID.GetValueOrDefault(0), objOrder.Room.GetValueOrDefault(0), 0, null, "Web => EmailLink => SendMailForOrderAprOrRej");

            }
            finally
            {
                MessageBody = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                //objUtils = null;
                objEmailDAL = null;
            }
        }

        /// <summary>
        /// EmailTemplateReplaceURL
        /// </summary>
        /// <param name="EmailTemplateName"></param>
        /// <returns></returns>
        private StringBuilder EmailTemplateReplaceURL(string EmailTemplateName)
        {
            StringBuilder MessageBody = new StringBuilder();
            if (System.IO.File.Exists(Server.MapPath("../") + "Content\\EmailTemplates\\" + EmailTemplateName))
            {
                MessageBody.Append(System.IO.File.ReadAllText(Server.MapPath("../") + "Content\\EmailTemplates/" + EmailTemplateName));
            }

            string urlPart = Request.Url.ToString();
            string replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];

            //MessageBody = MessageBody.Replace("../CommonImages/logo.gif", replacePart + "CommonImages/logo.gif");
            if (Request.ApplicationPath != "/")
                MessageBody = MessageBody.Replace("src=\"", "src=\"" + replacePart + Request.ApplicationPath);
            else
                MessageBody = MessageBody.Replace("src=\"", "src=\"" + replacePart);
            return MessageBody;
        }

        /// <summary>
        /// TransferStatus
        /// </summary>
        /// <param name="eKey"></param>
        /// <returns></returns>
        public ActionResult TransferStatus(string eKey)
        {
            string ResourceFilePath = GetResourceBaseFileFullPath("ResCommon", "en-US");
            string strMsg = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotValidData", ResourceFilePath, 0, 0, 0, "ResCommon", "en-US");
            if (!string.IsNullOrEmpty(eKey))
            {
                string data = StringCipher.Decrypt(eKey);
                string[] strData = data.Split('^');

                if (strData[6] == "APRV")
                {
                    strMsg = TransferApprove(strData);
                }
                else if (strData[6] == "RJKT")
                {
                    strMsg = TransferReject(strData);
                }
            }

            return View("TransferApproved", new CommonDTO() { Text = strMsg });
        }

        /// <summary>
        /// TransferApprove
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private string TransferApprove(string[] strData)
        {
            if (strData != null && strData.Length > 0)
            {

                Int64 RoomID = 0;
                Int64 CompanyID = 0;
                EnterpriseDTO objEnterpriseDTO = null;
                long EntID = 0;
                long.TryParse(strData[3], out EntID);
                long.TryParse(strData[1], out RoomID);
                long.TryParse(strData[2], out CompanyID);
                string EmailAddr = strData[5];
                UserMasterDTO objApprover = new UserMasterDTO();

                if (EntID > 0)
                {
                    eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                    objEnterpriseDTO = objEnterpriseMasterDAL.GetEnterpriseByIdPlain(EntID);
                }

                if (!string.IsNullOrWhiteSpace(EmailAddr))
                {
                    UserMasterDAL objUserMasterDAL = new UserMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                    objApprover = objUserMasterDAL.GetUserDetailsByEmail(EmailAddr);
                    if (objApprover != null)
                    {
                        EmailAddr = objApprover.UserName;
                    }
                    else
                    {
                        objApprover = new UserMasterDTO();
                        objApprover.ID = Int64.Parse(strData[4]);
                    }

                }

                TransferMasterDAL obj = new TransferMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                // SessionHelper.UserID = Int64.Parse(strData[4]);
                long transferId = 0;
                long.TryParse(strData[0], out transferId);
                TransferMasterDTO objDTO = obj.GetTransferByIdPlain(transferId);
                long SessionUserId = objApprover.ID;
                var cultureCode = "en-US";
                var regionalSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
                if (regionInfo != null)
                {
                    cultureCode = regionInfo.CultureCode;
                }
                string ResourceFilePath = GetResourceFileFullPath("ResTransfer", cultureCode, objEnterpriseDTO.ID, CompanyID);

                if (objDTO.TransferStatus == (int)eTurns.DTO.TransferStatus.Submitted)
                {
                    objDTO.TransferStatus = (int)eTurns.DTO.TransferStatus.Transmitted;
                    if (objDTO.RequestType == (int)eTurns.DTO.RequestType.Out)
                    {
                        objDTO.TransferStatus = (int)eTurns.DTO.TransferStatus.Approved;
                    }
                    objDTO.LastUpdatedBy = objApprover.ID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    obj.DB_Edit(objDTO);

                    RoomID = objDTO.RoomID;
                    CompanyID = objDTO.CompanyID;

                    TransferDetailDAL objDetailDAL = new TransferDetailDAL(objEnterpriseDTO.EnterpriseDBName);
                    var lstDetails = objDetailDAL.GetTransferDetailByTrfGuidPlain(objDTO.GUID);

                    if (lstDetails != null && lstDetails.Count() > 0)
                    {
                        foreach (var item in lstDetails)
                        {
                            item.LastUpdated = DateTimeUtility.DateTimeNow;
                            item.LastUpdatedBy = objApprover.ID;
                            item.ApprovedQuantity = item.RequestedQuantity;
                            objDetailDAL.Edit(item, SessionUserId, EntID);
                        }
                    }

                    string msgYourTransferHasBeenApproved = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourTransferHasBeenApproved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResTransfer", cultureCode);
                    string approved = ResourceRead.GetResourceValueByKeyAndFullFilePath("Approved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResTransfer", cultureCode);
                    SendMailToUserForTransfer(objDTO, objEnterpriseDTO, approved, RoomID, CompanyID);
                    return msgYourTransferHasBeenApproved;
                }
                else if (objDTO.TransferStatus == (int)eTurns.DTO.TransferStatus.Approved)
                {
                    string msgYourTransferHasAlreadyBeenApproved = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourTransferHasAlreadyBeenApproved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResTransfer", cultureCode);
                    return msgYourTransferHasAlreadyBeenApproved;
                }
                else
                {
                    string msgYourTransferIsNotValidToApproval = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourTransferIsNotValidToApproval", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResTransfer", cultureCode);
                    return msgYourTransferIsNotValidToApproval;
                }

            }
            else
            {
                string ResourceFilePath = GetResourceBaseFileFullPath("ResCommon", "en-US");
                string msgInvalidData = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotValidData", ResourceFilePath, 0, 0, 0, "ResCommon", "en-US");
                return msgInvalidData;
            }
        }

        /// <summary>
        /// TransferReject
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private string TransferReject(string[] strData)
        {
            if (strData != null && strData.Length > 0)
            {

                EnterpriseDTO objEnterpriseDTO = null;
                Int64 RoomID = 0;
                Int64 CompanyID = 0;

                long EntID = 0;
                long.TryParse(strData[3], out EntID);
                long.TryParse(strData[1], out RoomID);
                long.TryParse(strData[2], out CompanyID);
                if (EntID > 0)
                {
                    eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                    objEnterpriseDTO = objEnterpriseMasterDAL.GetEnterpriseByIdPlain(EntID);
                }

                string EmailAddr = strData[5];
                UserMasterDTO objApprover = new UserMasterDTO();

                TransferMasterDAL obj = new TransferMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                //SessionHelper.UserID = Int64.Parse(strData[3]);
                if (!string.IsNullOrWhiteSpace(EmailAddr))
                {
                    UserMasterDAL objUserMasterDAL = new UserMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                    objApprover = objUserMasterDAL.GetUserDetailsByEmail(EmailAddr);
                    if (objApprover != null)
                    {
                        EmailAddr = objApprover.UserName;
                    }
                    else
                    {
                        objApprover = new UserMasterDTO();
                        objApprover.ID = Int64.Parse(strData[3]);
                    }
                }

                long tmptransferId = 0;
                long.TryParse(strData[0], out tmptransferId);
                TransferMasterDTO objDTO = obj.GetTransferByIdPlain(tmptransferId);
                var cultureCode = "en-US";
                var regionalSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
                if (regionInfo != null)
                {
                    cultureCode = regionInfo.CultureCode;
                }
                string ResourceFilePath = GetResourceFileFullPath("ResTransfer", cultureCode, objEnterpriseDTO.ID, CompanyID);

                if (objDTO.TransferStatus == (int)eTurns.DTO.TransferStatus.UnSubmitted)
                {
                    string msgTransferHasAlreadyBeenRejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("TransferHasAlreadyBeenRejected", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResTransfer", cultureCode);
                    return msgTransferHasAlreadyBeenRejected;
                }
                else if (objDTO.TransferStatus == (int)eTurns.DTO.TransferStatus.Submitted)
                {

                    objDTO.TransferStatus = (int)eTurns.DTO.TransferStatus.UnSubmitted;
                    objDTO.RejectionReason = "Rejected by email";
                    objDTO.LastUpdatedBy = objApprover.ID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;

                    obj.DB_Edit(objDTO);
                    string msgTransferHasRejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("TransferHasRejected", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResTransfer", cultureCode);
                    string rejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("Rejected", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResTransfer", cultureCode);
                    SendMailToUserForTransfer(objDTO, objEnterpriseDTO, rejected, RoomID, CompanyID);
                    return msgTransferHasRejected;

                }
                else
                {
                    string msgTransferIsNotValidForRejection = ResourceRead.GetResourceValueByKeyAndFullFilePath("TransferIsNotValidForRejection", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResTransfer", cultureCode);
                    return msgTransferIsNotValidForRejection;
                }

            }

            string ResourceBaseFilePath = GetResourceBaseFileFullPath("ResCommon", "en-US");
            string msgInvalidData = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotValidData", ResourceBaseFilePath, 0, 0, 0, "ResCommon", "en-US");
            return msgInvalidData;
        }


        public void SendMailToUserForTransfer(TransferMasterDTO objOrder, EnterpriseDTO objEnterpriseDTO, string AprvRejString, Int64 RoomID, Int64 CompanyID)
        {
            List<eMailAttachmentDTO> lstAttachments = new List<eMailAttachmentDTO>();
            eTurnsWeb.Helper.AlertMail objAlertMail = new Helper.AlertMail();
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            NotificationDAL objNotificationDAL = null;
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            try
            {

                objNotificationDAL = new NotificationDAL(objEnterpriseDTO.EnterpriseDBName);
                lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.TransferApproveReject, RoomID, CompanyID, ResourceHelper.CurrentCult.Name);

                lstNotifications.ForEach(t =>
                {
                    if (t.SchedulerParams.ScheduleMode == 5)
                    {
                        lstNotificationsImidiate.Add(t);
                    }
                });

                if (lstNotificationsImidiate.Count > 0)
                {
                    lstNotificationsImidiate.ForEach(t =>
                    {
                        string StrSubject = string.Empty;
                        string strToAddress = t.EmailAddress;
                        if (!string.IsNullOrEmpty(strToAddress))
                        {
                            //string strCCAddress = "";
                            MessageBody = new StringBuilder();
                            objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                            if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                            {
                                objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.First();
                            }
                            if (objEmailTemplateDetailDTO != null)
                            {
                                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                                StrSubject = objEmailTemplateDetailDTO.MailSubject;
                            }
                            else
                            {
                                return;
                            }

                            MessageBody.Replace("@@ORDERNO@@", objOrder.TransferNumber);
                            MessageBody.Replace("@@TRANSFERNO@@", objOrder.TransferNumber);
                            if (objOrder != null)
                            {
                                MessageBody.Replace("@@TABLE@@", GetMailTransferBody(objOrder, objEnterpriseDTO.EnterpriseDBName, RoomID, CompanyID, objEnterpriseDTO.ID));
                            }
                            else
                            {
                                MessageBody.Replace("@@TABLE@@", string.Empty);
                            }
                            MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                            MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                            MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                            MessageBody.Replace("@@APRVREJ@@", AprvRejString);

                            Dictionary<string, string> Params = new Dictionary<string, string>();
                            Params.Add("DataGuids", objOrder.GUID.ToString());
                            lstAttachments = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                            objAlertMail.CreateAlertMail(lstAttachments, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                        }
                    });
                }


            }
            finally
            {
                MessageBody = null;
                objNotificationDAL = null;
                objEmailTemplateDetailDTO = null;
            }
        }


        /// <summary>
        /// SendMailForTransferAprOrRej
        /// </summary>
        /// <param name="UserList"></param>
        /// <param name="objOrder"></param>
        /// <param name="AprvRej"></param>
        //private void SendMailForTransferAprOrRej(List<UserMasterDTO> UserList, TransferMasterDTO objOrder, string AprvRej)
        //{
        //    StringBuilder MessageBody = null;
        //    EmailTemplateDAL objEmailTemplateDAL = null;
        //    EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
        //    eTurnsMaster.DAL.eMailDAL objEmailDAL = null;
        //    eTurnsUtility objUtils = null;
        //    try
        //    {
        //        string StrSubject = "Transfer " + AprvRej;// "Order Approval Request";
        //        string strCCAddress = CommonUtility.GetEmailToAddress(UserList, "TransferApproval");
        //        string strToAddress = CommonUtility.GetEmailToAddress(null, "TransferApproveReject");
        //        if (string.IsNullOrEmpty(strToAddress))
        //            strToAddress = strCCAddress;

        //        MessageBody = new StringBuilder();
        //        objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
        //        objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate("TransferApproveReject", eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
        //        if (objEmailTemplateDetailDTO != null)
        //        {
        //            MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
        //            StrSubject = objEmailTemplateDetailDTO.MailSubject;
        //        }
        //        else
        //        {
        //            return;
        //        }

        //        MessageBody.Replace("@@APRVREJ@@", AprvRej);
        //        MessageBody.Replace("@@ORDERNO@@", objOrder.TransferNumber);

        //        objUtils = new eTurnsUtility();
        //        objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
        //        objEmailDAL = new eTurnsMaster.DAL.eMailDAL();
        //        objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), 0, objOrder.CompanyID, objOrder.RoomID, 0, null, "Web => EmailLink => SendMailForTransferAprOrRej");
        //    }
        //    finally
        //    {
        //        MessageBody = null;
        //        objEmailTemplateDAL = null;
        //        objEmailTemplateDetailDTO = null;
        //        objEmailDAL = null;
        //        objUtils = null;
        //    }
        //}

        /// <summary>
        /// SendMailForTransferAprOrRej
        /// </summary>
        /// <param name="ToUserList"></param>
        /// <param name="CCUserList"></param>
        /// <param name="objOrder"></param>
        /// <param name="AprvRej"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="DataBaseName"></param>
        //private void SendMailForTransferAprOrRej(List<UserMasterDTO> ToUserList, List<UserMasterDTO> CCUserList, TransferMasterDTO objOrder, string AprvRej, Int64 RoomID, Int64 CompanyID, string DataBaseName)
        //{
        //    StringBuilder MessageBody = null;
        //    EmailTemplateDAL objEmailTemplateDAL = null;
        //    EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
        //    eTurnsMaster.DAL.eMailDAL objEmailDAL = null;
        //    //eTurnsUtility objUtils = null;
        //    try
        //    {
        //        string StrSubject = "Transfer " + AprvRej;// "Order Approval Request";
        //        string strCCAddress = CommonUtility.GetEmailToAddress(CCUserList, "TransferApproval", RoomID, CompanyID, DataBaseName);
        //        string strToAddress = CommonUtility.GetEmailToAddress(ToUserList, "TransferApproveReject", RoomID, CompanyID, DataBaseName);
        //        if (string.IsNullOrEmpty(strToAddress))
        //            strToAddress = strCCAddress;


        //        MessageBody = new StringBuilder();
        //        objEmailTemplateDAL = new EmailTemplateDAL(DataBaseName);
        //        objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
        //        objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate("TransferApproveReject", eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString(), RoomID, CompanyID);
        //        if (objEmailTemplateDetailDTO != null)
        //        {
        //            MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
        //            StrSubject = objEmailTemplateDetailDTO.MailSubject;
        //        }
        //        else
        //        {
        //            return;
        //        }


        //        MessageBody.Replace("@@APRVREJ@@", AprvRej);
        //        MessageBody.Replace("@@ORDERNO@@", objOrder.TransferNumber);
        //        MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
        //        MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
        //        MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
        //        //MessageBody.Replace("@@ETURNSLOGO@@", CommonUtility.GeteTurnsImage("http://" + System.Web.HttpContext.Current.Request.Url.Authority, "/Content/images/logo.jpg"));

        //        //objUtils = new eTurnsUtility();
        //        //objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
        //        objEmailDAL = new eTurnsMaster.DAL.eMailDAL();
        //        objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), 0, objOrder.CompanyID, objOrder.RoomID, 0, null, "Web => EmailLink => SendMailForTransferAprOrRej");

        //    }
        //    finally
        //    {
        //        MessageBody = null;
        //        objEmailTemplateDAL = null;
        //        objEmailTemplateDetailDTO = null;
        //        objEmailDAL = null;
        //        //objUtils = null;
        //    }
        //}

        /// <summary>
        /// RequisitionStatus
        /// </summary>
        /// <param name="eKey"></param>
        /// <returns></returns>
        public ActionResult RequisitionStatus(string eKey)
        {
            string ResourceFilePath = GetResourceBaseFileFullPath("ResCommon", "en-US");
            string strMsg = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotValidData", ResourceFilePath, 0, 0, 0, "ResCommon", "en-US");
            if (!string.IsNullOrEmpty(eKey))
            {
                string data = StringCipher.Decrypt(eKey);
                string[] strData = data.Split('^');

                if (strData[7] == "APRV")
                {
                    strMsg = RequisitionApprove(strData);
                }
                else if (strData[7] == "RJKT")
                {
                    strMsg = RequisitionReject(strData);
                }
            }

            return View("RequisitionApproved", new CommonDTO() { Text = strMsg });
        }

        /// <summary>
        /// RequisitionApprove
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private string RequisitionApprove(string[] strData)
        {
            if (strData != null && strData.Length > 0)
            {
                string requisitionDataLog = string.Join(",", strData) + "; Method - RequisitionApprove; Controller-EmailLinkController : on " + DateTime.UtcNow.ToString();
                Int64 RoomID = 0;
                Int64 CompanyID = 0;
                EnterpriseDTO objEnterpriseDTO = null;
                long EntID = 0;
                long.TryParse(strData[4], out EntID);
                string EmailAddr = strData[6];
                UserMasterDTO objApprover = new UserMasterDTO();
                if (EntID > 0)
                {
                    eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                    objEnterpriseDTO = objEnterpriseMasterDAL.GetEnterpriseByIdPlain(EntID);
                }
                if (!string.IsNullOrWhiteSpace(EmailAddr))
                {
                    UserMasterDAL objUserMasterDAL = new UserMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                    objApprover = objUserMasterDAL.GetUserDetailsByEmail(EmailAddr);
                    if (objApprover != null)
                    {
                        EmailAddr = objApprover.UserName;
                    }
                    else
                    {
                        objApprover = new UserMasterDTO();
                    }

                }
                RequisitionMasterDAL obj = new RequisitionMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                SessionHelper.UserID = Int64.Parse(strData[3]);
                RequisitionMasterDTO objDTO = obj.GetRequisitionByIDFull(Int64.Parse(strData[0]));
                if (objDTO != null && objDTO.ID > 0)
                {
                    var cultureCode = "en-US";
                    var regionalSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                    var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
                    if (regionInfo != null)
                    {
                        cultureCode = regionInfo.CultureCode;
                    }
                    string ResourceFilePath = GetResourceFileFullPath("ResRequisitionMaster", cultureCode, objEnterpriseDTO.ID, objDTO.CompanyID.GetValueOrDefault(0));

                    if (objDTO.RequisitionStatus == "Submitted")
                    {
                        CommonDAL objCommonDAL = new CommonDAL(objEnterpriseDTO.EnterpriseDBName);
                        string strMsg = objCommonDAL.ValidateToolCategory(objDTO, objEnterpriseDTO.EnterpriseDBName, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                        if (!string.IsNullOrEmpty(strMsg) || !string.IsNullOrWhiteSpace(strMsg))
                        {
                            return strMsg;
                        }
                        objDTO.RequisitionStatus = "Approved";
                        if (objApprover.ID > 0)
                        {
                            objDTO.LastUpdatedBy = objApprover.ID;
                        }

                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.RequisitionApprover = EmailAddr;
                        objDTO.RequisitionDataLog = requisitionDataLog;
                        if (objDTO.RequisitionStatus.ToUpper() == "APPROVED")
                        {
                            objDTO.ApproverID = objDTO.LastUpdatedBy;
                        }
                        //obj.Edit(objDTO);

                        bool isSuccess = obj.UpdateRequisitionStatusByEmailLink(objDTO.ID, objDTO.LastUpdatedBy, objDTO.ApproverID, "Approved", DateTimeUtility.DateTimeNow, EmailAddr, requisitionDataLog);
                        if (isSuccess)
                        {
                            obj.SetRequisitionReleaseNumber(objDTO);
                            RoomID = objDTO.Room.GetValueOrDefault(0);
                            CompanyID = objDTO.CompanyID.GetValueOrDefault(0);

                            RequisitionDetailsDAL objDetailDAL = new RequisitionDetailsDAL(objEnterpriseDTO.EnterpriseDBName);
                            IEnumerable<RequisitionDetailsDTO> lstDetails = objDetailDAL.GetReqLinesByReqGUIDPlain(objDTO.GUID, 0, RoomID, CompanyID);//.Where(x => x.RequisitionGUID == objDTO.GUID && x.IsDeleted == false && x.IsArchived == false)
                            long SessionUserId = SessionHelper.UserID;
                            if (lstDetails != null && lstDetails.Count() > 0)
                            {
                                foreach (var item in lstDetails)
                                {
                                    item.LastUpdated = DateTimeUtility.DateTimeNow;
                                    item.LastUpdatedBy = Int64.Parse(strData[2]);
                                    item.QuantityApproved = item.QuantityRequisitioned.GetValueOrDefault(0);
                                    objDetailDAL.Edit(item, SessionUserId);
                                }
                            }

                            eTurns.DAL.UserMasterDAL userMasterDAL = new eTurns.DAL.UserMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                            eTurnsMaster.DAL.UserMasterDAL objReqRequesterUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                            UserMasterDTO ReqUser = new UserMasterDTO();

                            string ReqRequesterEmailAddress = "";
                            string ReqApproverEmailAddress = "";
                            if (objDTO.RequesterID.GetValueOrDefault(0) > 0)
                            {
                                ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(objDTO.RequesterID.GetValueOrDefault(0));
                                if (ReqUser == null)
                                {
                                    ReqUser = userMasterDAL.GetUserByIdPlain(objDTO.RequesterID.GetValueOrDefault(0));
                                }
                                if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                                {
                                    ReqRequesterEmailAddress = ReqUser.Email;
                                }
                            }
                            if (objDTO.ApproverID.GetValueOrDefault(0) > 0)
                            {
                                ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(objDTO.ApproverID.GetValueOrDefault(0));
                                if (ReqUser == null)
                                {
                                    ReqUser = userMasterDAL.GetUserByIdPlain(objDTO.ApproverID.GetValueOrDefault(0));
                                }
                                if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                                {
                                    ReqApproverEmailAddress = ReqUser.Email;
                                }
                            }

                            string msgYourRequisitionHasBeenApproved = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourRequisitionHasBeenApproved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResRequisitionMaster", cultureCode);
                            string approved = ResourceRead.GetResourceValueByKeyAndFullFilePath("Approved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResRequisitionMaster", cultureCode);
                            SendMailForRequisitionAprOrRej(objDTO, approved, objEnterpriseDTO, ReqRequesterEmailAddress, ReqApproverEmailAddress);

                            return msgYourRequisitionHasBeenApproved;
                        }
                        else
                        {
                            string msgYourRequisitionIsNotValidToApproval = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourRequisitionIsNotValidToApproval", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResRequisitionMaster", cultureCode);
                            return msgYourRequisitionIsNotValidToApproval;
                        }
                    }
                    else if (objDTO.RequisitionStatus.ToLower() == "approved")
                    {
                        string msgYourRequisitionHasAlreadyBeenApproved = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourRequisitionHasAlreadyBeenApproved", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResRequisitionMaster", cultureCode);
                        return msgYourRequisitionHasAlreadyBeenApproved;
                    }
                    else
                    {
                        string msgYourRequisitionIsNotValidToApproval = ResourceRead.GetResourceValueByKeyAndFullFilePath("YourRequisitionIsNotValidToApproval", ResourceFilePath, objEnterpriseDTO.ID, CompanyID, RoomID, "ResRequisitionMaster", cultureCode);
                        return msgYourRequisitionIsNotValidToApproval;
                    }

                }
                else
                {
                    string ResourceFilePath = GetResourceBaseFileFullPath("ResCommon", "en-US");
                    string msgInvalidData = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotValidData", ResourceFilePath, 0, 0, 0, "ResCommon", "en-US");
                    return msgInvalidData;
                }
            }
            else
            {
                string ResourceFilePath = GetResourceBaseFileFullPath("ResCommon", "en-US");
                string msgInvalidData = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotValidData", ResourceFilePath, 0, 0, 0, "ResCommon", "en-US");
                return msgInvalidData;
            }
        }

        /// <summary>
        /// RequisitionReject
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private string RequisitionReject(string[] strData)
        {
            if (strData != null && strData.Length > 0)
            {
                string requisitionDataLog = string.Join(",", strData) + "; Method - RequisitionReject; Controller-EmailLinkController : on " + DateTime.UtcNow.ToString();

                EnterpriseDTO objEnterpriseDTO = null;
                long EntID = 0;
                long.TryParse(strData[4], out EntID);
                string EmailAddr = strData[6];
                UserMasterDTO objApprover = new UserMasterDTO();
                eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                objEnterpriseDTO = objEnterpriseMasterDAL.GetEnterpriseByIdPlain(EntID);
                RequisitionMasterDAL obj = new RequisitionMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                RequisitionMasterDTO objDTO = obj.GetRequisitionByIDFull(Int64.Parse(strData[0]));
                if (!string.IsNullOrWhiteSpace(EmailAddr))
                {
                    UserMasterDAL objUserMasterDAL = new UserMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                    objApprover = objUserMasterDAL.GetUserDetailsByEmail(EmailAddr);
                    if (objApprover != null)
                    {
                        EmailAddr = objApprover.UserName;
                    }
                    else
                    {
                        objApprover = new UserMasterDTO();
                    }
                }
                if (objDTO != null && objDTO.ID > 0)
                {
                    var cultureCode = "en-US";
                    var regionalSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                    var regionInfo = regionalSettingDAL.GetRegionSettingsById(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), -1);
                    if (regionInfo != null)
                    {
                        cultureCode = regionInfo.CultureCode;
                    }
                    string ResourceFilePath = GetResourceFileFullPath("ResRequisitionMaster", cultureCode, objEnterpriseDTO.ID, objDTO.CompanyID.GetValueOrDefault(0));
                    string orderResourceFilePath = GetResourceFileFullPath("ResOrder", cultureCode, objEnterpriseDTO.ID, objDTO.CompanyID.GetValueOrDefault(0));

                    if (objDTO.RequisitionStatus.ToLower() == "unsubmitted")
                    {
                        string msgRequisitionHasAlreadyBeenRejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequisitionHasAlreadyBeenRejected", ResourceFilePath, objEnterpriseDTO.ID, objDTO.CompanyID.GetValueOrDefault(0), objDTO.Room.GetValueOrDefault(0), "ResRequisitionMaster", cultureCode);
                        return msgRequisitionHasAlreadyBeenRejected;
                    }
                    else if (objDTO.RequisitionStatus.ToLower() == "submitted")
                    {
                        if (objApprover.ID > 0)
                        {
                            objDTO.LastUpdatedBy = objApprover.ID;
                        }
                        if (objDTO.RequisitionStatus.ToLower() == "submitted")
                        {
                            objDTO.ApproverID = objDTO.LastUpdatedBy;
                        }
                        objDTO.RequisitionStatus = "Unsubmitted";
                        ////objDTO.re = "Rejected by email.";                       

                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.RequisitionApprover = EmailAddr;
                        objDTO.RequisitionDataLog = requisitionDataLog;
                        //obj.Edit(objDTO);

                        bool isSuccess = obj.UpdateRequisitionStatusByEmailLink(objDTO.ID, objDTO.LastUpdatedBy, objDTO.ApproverID, "Unsubmitted", DateTimeUtility.DateTimeNow, EmailAddr, requisitionDataLog);
                        if (isSuccess)
                        {
                            obj.SetRequisitionReleaseNumber(objDTO);
                            eTurns.DAL.UserMasterDAL userMasterDAL = new eTurns.DAL.UserMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                            eTurnsMaster.DAL.UserMasterDAL objReqRequesterUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                            UserMasterDTO ReqUser = new UserMasterDTO();

                            string ReqRequesterEmailAddress = "";
                            string ReqApproverEmailAddress = "";
                            if (objDTO.RequesterID.GetValueOrDefault(0) > 0)
                            {
                                ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(objDTO.RequesterID.GetValueOrDefault(0));
                                if (ReqUser == null)
                                {
                                    ReqUser = userMasterDAL.GetUserByIdPlain(objDTO.RequesterID.GetValueOrDefault(0));
                                }
                                if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                                {
                                    ReqRequesterEmailAddress = ReqUser.Email;
                                }
                            }
                            if (objDTO.ApproverID.GetValueOrDefault(0) > 0)
                            {
                                ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(objDTO.ApproverID.GetValueOrDefault(0));
                                if (ReqUser == null)
                                {
                                    ReqUser = userMasterDAL.GetUserByIdPlain(objDTO.ApproverID.GetValueOrDefault(0));
                                }
                                if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                                {
                                    ReqApproverEmailAddress = ReqUser.Email;
                                }
                            }
                            string msgRequisitionWasRejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequisitionWasRejected", ResourceFilePath, objEnterpriseDTO.ID, objDTO.CompanyID.GetValueOrDefault(0), objDTO.Room.GetValueOrDefault(0), "ResRequisitionMaster", cultureCode);
                            string rejected = ResourceRead.GetResourceValueByKeyAndFullFilePath("Rejected", orderResourceFilePath, objEnterpriseDTO.ID, objDTO.CompanyID.GetValueOrDefault(0), objDTO.Room.GetValueOrDefault(0), "ResOrder", cultureCode);
                            SendMailForRequisitionAprOrRej(objDTO, rejected, objEnterpriseDTO, ReqRequesterEmailAddress, ReqApproverEmailAddress);
                            return msgRequisitionWasRejected;
                        }
                        else
                        {
                            string msgRequisitionIsNotValidForRejection = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequisitionIsNotValidForRejection", ResourceFilePath, objEnterpriseDTO.ID, objDTO.CompanyID.GetValueOrDefault(0), objDTO.Room.GetValueOrDefault(0), "ResRequisitionMaster", cultureCode);
                            return msgRequisitionIsNotValidForRejection;
                        }
                    }
                    else
                    {
                        string msgRequisitionIsNotValidForRejection = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequisitionIsNotValidForRejection", ResourceFilePath, objEnterpriseDTO.ID, objDTO.CompanyID.GetValueOrDefault(0), objDTO.Room.GetValueOrDefault(0), "ResRequisitionMaster", cultureCode);
                        return msgRequisitionIsNotValidForRejection;
                    }
                }
                else
                {
                    string ResourceBaseFilePathCommon = GetResourceBaseFileFullPath("ResMessage", "en-US");
                    string msgDoesntExist = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgDoesNotExist", ResourceBaseFilePathCommon, 0, 0, 0, "ResCommon", "en-US");
                    string requisitionBasePath = GetResourceBaseFileFullPath("ResRequisitionMaster", "en-US");
                    string msgRequisition = ResourceRead.GetResourceValueByKeyAndFullFilePath("Requisition", requisitionBasePath, 0, 0, 0, "ResCommon", "en-US");
                    return string.Format(msgDoesntExist, msgRequisition);
                }
            }


            string ResourceBaseFilePath = GetResourceBaseFileFullPath("ResCommon", "en-US");
            string msgInvalidData = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotValidData", ResourceBaseFilePath, 0, 0, 0, "ResCommon", "en-US");
            return msgInvalidData;
        }

        /// <summary>
        /// SendMailForRequisitionAprOrRej
        /// </summary>
        /// <param name="UserList"></param>
        /// <param name="objOrder"></param>
        /// <param name="AprvRej"></param>
        //private void SendMailForRequisitionAprOrRej(RequisitionMasterDTO objOrder, string AprvRej, EnterpriseDTO objEnterprise)
        //{
        //    StringBuilder MessageBody = null;
        //    EmailTemplateDAL objEmailTemplateDAL = null;
        //    EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
        //    eTurnsUtility objUtils = null;
        //    eTurnsMaster.DAL.eMailDAL objEmailDAL = null;
        //    try
        //    {
        //        string StrSubject = "Requisition " + AprvRej;// "Order Approval Request";

        //        string strToAddress = string.Empty;
        //        string strCCAddress = string.Empty;
        //        if (!string.IsNullOrEmpty(strToAddress))
        //        {
        //            MessageBody = new StringBuilder();
        //            objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
        //            objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
        //            objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate("RequisitionApproveReject", eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
        //            if (objEmailTemplateDetailDTO != null)
        //            {
        //                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
        //                StrSubject = objEmailTemplateDetailDTO.MailSubject;
        //            }
        //            else
        //            {
        //                return;
        //            }
        //            MessageBody.Replace("@@APRVREJ@@", AprvRej);
        //            MessageBody.Replace("@@ORDERNO@@", objOrder.RequisitionNumber);
        //            objUtils = new eTurnsUtility();
        //            objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
        //            objEmailDAL = new eTurnsMaster.DAL.eMailDAL();
        //            objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), 0, objOrder.CompanyID.GetValueOrDefault(0), objOrder.Room.GetValueOrDefault(0), 0, null, "Web => EmailLink => SendMailForRequisitionaprOrRej");
        //        }
        //    }
        //    finally
        //    {
        //        MessageBody = null;
        //        objEmailTemplateDAL = null;
        //        objEmailTemplateDetailDTO = null;
        //        objUtils = null;
        //        objEmailDAL = null;
        //    }
        //}

        /// <summary>
        /// SendMailForRequisitionAprOrRej
        /// </summary>
        /// <param name="ToUsers"></param>
        /// <param name="CCUserList"></param>
        /// <param name="objOrder"></param>
        /// <param name="AprvRej"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="DataBaseName"></param>
        private void SendMailForRequisitionAprOrRejOLD(List<UserMasterDTO> ToUsers, List<UserMasterDTO> CCUserList, RequisitionMasterDTO objOrder, string AprvRej, Int64 RoomID, Int64 CompanyID, string DataBaseName)
        {
            StringBuilder MessageBody = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            eTurnsUtility objUtils = null;
            Helper.AlertMail objAlertMail = new Helper.AlertMail();
            eMailMasterDAL objEmailDAL = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            EnterpriseDTO objEnterpriseDTO = null;
            try
            {
                string StrSubject = "Requisition " + AprvRej;// "Order Approval Request";
                string strCCAddress = CommonUtility.GetEmailToAddress(CCUserList, "RequisitionApproval", RoomID, CompanyID, DataBaseName);
                string strToAddress = CommonUtility.GetEmailToAddress(ToUsers, "RequisitionApproveReject", RoomID, CompanyID, DataBaseName);
                if (string.IsNullOrEmpty(strToAddress))
                    strToAddress = strCCAddress;

                List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
                List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
                NotificationDAL objNotificationDAL = new NotificationDAL(DataBaseName);
                lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.RequisitionApproval, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);
                lstNotifications.ForEach(t =>
                {
                    if (t.SchedulerParams.ScheduleMode == 5)
                    {
                        lstNotificationsImidiate.Add(t);
                    }
                });

                EnterpriseDAL objEntDAL = new EnterpriseDAL(DataBaseName);
                objEnterpriseDTO = objEntDAL.GetEnterpriseByDbName(DataBaseName);


                if (!string.IsNullOrEmpty(strToAddress))
                {

                    MessageBody = new StringBuilder();
                    objEmailTemplateDAL = new EmailTemplateDAL(DataBaseName);
                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                    objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate("RequisitionApproveReject", eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString(), RoomID, CompanyID);
                    if (objEmailTemplateDetailDTO != null)
                    {
                        MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                        StrSubject = objEmailTemplateDetailDTO.MailSubject;
                    }
                    else
                    {
                        return;
                    }
                    MessageBody.Replace("@@APRVREJ@@", AprvRej);
                    MessageBody.Replace("@@ORDERNO@@", objOrder.RequisitionNumber);
                    MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                    MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                    MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                    // MessageBody.Replace("@@ETURNSLOGO@@", CommonUtility.GeteTurnsImage("http://" + System.Web.HttpContext.Current.Request.Url.Authority, "/Content/images/logo.jpg"));
                    //CommonUtility.SendMail(FromAddress, strToAddress, strCCAddress, strBCCAddress, strNotificationAddress, StrSubject, MessageBody.ToString(), true);
                    if (lstNotificationsImidiate.Count > 0)
                    {
                        lstNotificationsImidiate.ForEach(t =>
                        {
                            Dictionary<string, string> Params = new Dictionary<string, string>();
                            Params.Add("DataGuids", objOrder.GUID.ToString());
                            objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                        });
                    }

                    objUtils = new eTurnsUtility();
                    objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
                    objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                    eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                    string DateTimeFormat = "MM/dd/yyyy";
                    DateTime TZDateTimeNow = DateTime.UtcNow;
                    if (objeTurnsRegionInfo != null)
                    {
                        DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                        TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                    }
                    if (StrSubject != null)
                    {
                        string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                        // EmailSubject = EmailSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                        StrSubject = Regex.Replace(StrSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                        if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                        {
                            StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                        }
                        if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                        {
                            StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", SessionHelper.RoomName, RegexOptions.IgnoreCase);
                        }

                        StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                    }

                    if (MessageBody != null)
                    {
                        string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                        //EmailBody = EmailBody.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                        MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);
                        if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                        {
                            MessageBody = MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                        }
                        if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                        {
                            MessageBody = MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                        }
                        MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));

                    }
                    objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), objEnterpriseDTO.ID, objOrder.CompanyID.GetValueOrDefault(0), objOrder.Room.GetValueOrDefault(0), 0, objeMailAttchList, "Web => EmailLink => SendMailForRequisitionaprOrRej");
                }
            }
            finally
            {
                MessageBody = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                objUtils = null;
                objEmailDAL = null;
            }
        }


        private void SendMailForRequisitionAprOrRej(RequisitionMasterDTO objRequisition, string AprvRej, EnterpriseDTO objEnterpriseDTO, string OrdRequesterEmailAddress, string OrdApproverEmailAddress)
        {
            List<eMailAttachmentDTO> objeMailAttchList = new List<eMailAttachmentDTO>();
            Helper.AlertMail objAlertMail = new Helper.AlertMail();
            NotificationDAL objNotificationDAL = new NotificationDAL(objEnterpriseDTO.EnterpriseDBName);
            StringBuilder MessageBody = null;
            //eTurnsMaster.DAL.eMailDAL objEmailDAL = null;
            ////EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            //eTurnsUtility objUtils = null;
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();

            string Roomname = string.Empty;
            string Companyname = string.Empty;
            RoomDTO objroom = new RoomDAL(objEnterpriseDTO.EnterpriseDBName).GetRoomByIDNormal(objRequisition.Room ?? 0);
            //CompanyMasterDTO objcompany = new CompanyMasterDAL(objEnterpriseDTO.EnterpriseDBName).GetCompanyByID(objRequisition.CompanyID ?? 0);
            if (objroom != null)
            {
                Roomname = objroom.RoomName;
                Companyname = objroom.CompanyName;
            }

            try
            {
                lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.RequisitionApproveReject, objRequisition.Room ?? 0, objRequisition.CompanyID ?? 0, ResourceHelper.CurrentCult.Name);
                lstNotifications.ForEach(t =>
                {
                    if (t.SchedulerParams.ScheduleMode == 5)
                    {
                        lstNotificationsImidiate.Add(t);
                    }
                });

                if (lstNotificationsImidiate.Count > 0)
                {
                    lstNotificationsImidiate.ForEach(t =>
                    {
                        string StrSubject = t.EmailSubject;// "Order Approval Request";
                        string strToAddress = t.EmailAddress;
                        if (!string.IsNullOrEmpty(strToAddress))
                        {
                            if (strToAddress.Contains("[Requester]"))
                            {
                                if (!string.IsNullOrWhiteSpace(OrdRequesterEmailAddress))
                                    strToAddress = strToAddress.Replace("[Requester]", OrdRequesterEmailAddress);
                                else
                                    strToAddress = strToAddress.Replace("[Requester]", "");
                            }
                            if (strToAddress.Contains("[Approver]"))
                            {
                                if (!string.IsNullOrWhiteSpace(OrdApproverEmailAddress))
                                    strToAddress = strToAddress.Replace("[Approver]", OrdApproverEmailAddress);
                                else
                                    strToAddress = strToAddress.Replace("[Approver]", "");
                            }
                            List<string> EmailAddrs = strToAddress.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            EmailAddrs = EmailAddrs.Distinct().ToList();
                            strToAddress = string.Join(",", EmailAddrs);
                            //string FromAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                            //string strCCAddress = "";
                            MessageBody = new StringBuilder();

                            objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                            if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                            {
                                objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.First();
                            }

                            if (objEmailTemplateDetailDTO != null)
                            {
                                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                                StrSubject = objEmailTemplateDetailDTO.MailSubject + "-" + AprvRej;
                            }
                            else
                            {
                                return;
                            }
                            MessageBody.Replace("@@APRVREJ@@", AprvRej);
                            MessageBody.Replace("@@ORDERNO@@", objRequisition.RequisitionNumber);
                            MessageBody.Replace("@@REQUISITIONNO@@", objRequisition.RequisitionNumber);
                            if (objRequisition != null)
                            {
                                MessageBody.Replace("@@TABLE@@", GetMailRequistionBody(objRequisition, objEnterpriseDTO.EnterpriseDBName, objRequisition.Room ?? 0, objRequisition.CompanyID ?? 0, objEnterpriseDTO.ID));
                            }
                            else
                            {
                                MessageBody.Replace("@@TABLE@@", string.Empty);
                            }
                            MessageBody.Replace("@@ROOMNAME@@", Roomname);
                            MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                            MessageBody.Replace("@@COMPANYNAME@@", Companyname);
                            //MessageBody.Replace("@@ETURNSLOGO@@", CommonUtility.GeteTurnsImage("http://" + System.Web.HttpContext.Current.Request.Url.Authority, "/Content/images/logo.jpg"));

                            string replacePart = string.Empty;
                            if (Request == null)
                            {
                                replacePart = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                            }
                            else
                            {
                                string urlPart = Request.Url.ToString();
                                replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
                            }

                            Dictionary<string, string> Params = new Dictionary<string, string>();
                            Params.Add("DataGuids", objRequisition.GUID.ToString());
                            objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                            objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                        }
                    });
                }

            }
            finally
            {
                MessageBody = null;
                //objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                //objUtils = null;
                //objEmailDAL = null;
            }
        }
        private string GetMailRequistionBody(RequisitionMasterDTO obj, string DBName, Int64 RoomID, Int64 CompanyID, long EnterpriseId)
        {
            int PriseSelectionOption = 1;
            RoomDAL objRoomDAL = new RoomDAL(DBName);
            RoomModuleSettingsDTO objRoomModuleSettingsDTO = new RoomModuleSettingsDTO();
            objRoomModuleSettingsDTO = objRoomDAL.GetRoomModuleSettings(CompanyID, RoomID, (long)ModuleInfo.Requisitions);

            if (objRoomModuleSettingsDTO != null && objRoomModuleSettingsDTO.ID > 0)
            {
                PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption.GetValueOrDefault(0);
            }

            var cultureCode = "en-US";
            var regionalSettingDAL = new RegionSettingDAL(DBName);
            var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
            if (regionInfo != null)
            {
                cultureCode = regionInfo.CultureCode;
            }
            string ResourceFilePath = GetResourceFileFullPath("ResRequisitionMaster", cultureCode, EnterpriseId, CompanyID);
            string orderResourceFilePath = GetResourceFileFullPath("ResOrder", cultureCode, EnterpriseId, CompanyID);
            string itemResourceFilePath = GetResourceFileFullPath("ResItemMaster", cultureCode, EnterpriseId, CompanyID);
            string RequisitionDetailResourceFilePath = GetResourceFileFullPath("ResRequisitionDetails", cultureCode, EnterpriseId, CompanyID);

            string TotalCostResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("TotalCost", ResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResRequisitionMaster", cultureCode);
            string TotalSellPriceResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("TotalSellPrice", ResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResRequisitionMaster", cultureCode);
            string RequisitionNumberResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequisitionNumber", ResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResRequisitionMaster", cultureCode);
            string DescriptionResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("Description", ResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResRequisitionMaster", cultureCode);
            string WorkorderNameResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("WorkorderName", ResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResRequisitionMaster", cultureCode);
            string RequiredDateResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequiredDate", ResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResRequisitionMaster", cultureCode);
            string RequisitionStatusResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequisitionStatus", ResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResRequisitionMaster", cultureCode);
            string ItemNumberResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNumber", itemResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResItemMaster", cultureCode);
            string OnHandQuantityResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("OnHandQuantity", itemResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResItemMaster", cultureCode);
            string BinResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("Bin", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
            string QuantityRequisitionedResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("QuantityRequisitioned", RequisitionDetailResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResRequisitionDetails", cultureCode);
            string ReqDetailRequiredDateResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequiredDate", RequisitionDetailResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResRequisitionDetails", cultureCode);
            string CostTitle = TotalCostResourceValue;
            CostTitle = (PriseSelectionOption == 1 ? TotalSellPriceResourceValue : TotalCostResourceValue);

            string mailBody = "";

            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                <tr>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label style=""font-weight: bold;"">
                                        " + RequisitionNumberResourceValue + @": </label>
                                    <label  style=""font-weight: bold;"">
                                        " + obj.RequisitionNumber + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + DescriptionResourceValue + @": </label>
                                    <label>
                                        " + obj.Description + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + WorkorderNameResourceValue + @": </label>
                                    <label>
                                        " + obj.WorkorderName + @"</label>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label>
                                       " + RequiredDateResourceValue + @": </label>
                                    <label>
                                        " + (obj.RequiredDate == null ? "&nbsp;" : obj.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy")) + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + CostTitle + @": </label>
                                    <label>
                                        " +
                                             (Convert.ToString(PriseSelectionOption) == "1" ? obj.TotalSellPrice.ToString("#.##") : obj.TotalCost.GetValueOrDefault(0).ToString("#.##"))
                                          + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + RequisitionStatusResourceValue + @": </label>
                                    <label>
                                        " + obj.RequisitionStatus + @"</label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan=""2"" style=""width: 99%"">
                        <table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ItemNumberResourceValue + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + BinResourceValue + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + QuantityRequisitionedResourceValue + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + OnHandQuantityResourceValue + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ReqDetailRequiredDateResourceValue + @"
                                    </th>
                                    
                                </tr>
                            </thead>
                            <tbody>
            ##TRS##
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
            ";
            string trs = "";
            if (obj.RequisitionListItem == null || obj.RequisitionListItem.Count <= 0)
            {
                RequisitionDetailsDAL objDetailDAL = new RequisitionDetailsDAL(DBName);
                obj.RequisitionListItem = objDetailDAL.GetReqLinesByReqGUIDFull(obj.GUID, 0, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0)).ToList();
            }

            if (obj.RequisitionListItem != null && obj.RequisitionListItem.Count > 0)
            {

                foreach (var item in obj.RequisitionListItem)
                {
                    string binname = "&nbsp;";
                    string ReqQty = "&nbsp;";
                    string ItemOHQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    if (item.BinID != null && item.BinID > 0)
                        //binname = new BinMasterController().GetRecord(Int64.Parse(Convert.ToString(item.Bin)), SessionHelper.RoomID, SessionHelper.CompanyID, false, false).BinNumber;
                        binname = new BinMasterDAL(DBName).GetBinByID(Int64.Parse(Convert.ToString(item.BinID)), RoomID, CompanyID).BinNumber;
                    //binname = new BinMasterDAL(DBName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, Int64.Parse(Convert.ToString(item.BinID)),null,null).FirstOrDefault().BinNumber;

                    if (item.QuantityRequisitioned != null)
                        //ReqQty = item.QuantityApproved.ToString();
                        ReqQty = item.QuantityRequisitioned.ToString();

                    if (item.RequiredDate != null)
                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");

                    ItemMasterDTO objItem = new ItemMasterDAL(DBName).GetItemByGuidPlain(item.ItemGUID ?? Guid.Empty, obj.Room ?? 0, obj.CompanyID ?? 0);
                    if (objItem != null)
                    {
                        ItemOHQty = (objItem.OnHandQuantity ?? 0).ToString();
                    }
                    trs += @"<tr>
                        <td>
                            " + (!string.IsNullOrEmpty(item.ItemNumber) ? item.ItemNumber : "&nbsp;") + @"
                        </td>
                        <td>
                            " + binname + @"
                        </td>
                        <td>
                            " + ReqQty + @"
                        </td>
                        <td>
                            " + ItemOHQty + @"
                        </td>
                        <td>
                            " + ReqDate + @"
                        </td>
                        
                    </tr>";
                }
            }
            else
            {
                string NoItemForThisOrderResourceValue = ResourceRead.GetResourceValueByKeyAndFullFilePath("NoItemForThisOrder", orderResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResOrder", cultureCode);
                trs += @"<tr>
                        <td colspan=""4"" style=""text-align:center"">
                           " + NoItemForThisOrderResourceValue + @"
                        </td>
                    </tr>";
            }
            mailBody = mailBody.Replace("##TRS##", trs);

            return mailBody;
        }

        /// <summary>
        /// SendPassword
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>


        /// <summary>
        /// Send Mail For Item Stockout
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="EnterpriceID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [HttpPost]
        public string SendMailForBinSensorError(Int64 EnterPriceID, Int64 CompanyID, Int64 RoomID, string UserName, string Password, string Message, string Subject)
        {
            MasterController mstController = new MasterController();
            mstController.UserLoginAuthanticateMasterDataBase(UserName, Password, false);
            SendMailForBinSensorNotWorking(RoomID, CompanyID, Message, Subject);
            return string.Empty;
        }

        /// <summary>
        /// SendMailForBinSensorNotWorking
        /// </summary>
        /// <param name="CompanyID"></param>
        /// <param name="RoomID"></param>
        /// <param name="SensorMassage"></param>
        /// <param name="strSubject"></param>
        private void SendMailForBinSensorNotWorking(Int64 CompanyID, Int64 RoomID, string SensorMassage, string strSubject)
        {
            StringBuilder MessageBody = null;
            CompanyMasterDAL objCompDAL = null;
            CompanyMasterDTO objCompany = null;
            RoomDAL objRoomDAL = null;
            CommonDAL objDAL = null;
            RoomDTO objRoom = null;
            eTurnsUtility objUtils = null;
            eMailMasterDAL objEmailDAL = null;
            try
            {
                string strToAddress = ConfigurationManager.AppSettings["OverrideToEmail"];
                string strCCAddress = "";

                MessageBody = EmailTemplateReplaceURL("SensorNoWorking-" + eTurns.DTO.Resources.ResourceHelper.CurrentCult + ".html");
                objCompDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                objCompany = objCompDAL.GetCompanyByID(CompanyID);
                objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                objDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                //objRoom = objRoomDAL.GetRoomByIDPlain(RoomID);

                string columnList = "ID,RoomName,Email";
                objRoom = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

                if (string.IsNullOrEmpty(strToAddress))
                {
                    string ExternalUsers = GetExternalUser("SensorNoWorking");

                    if (!string.IsNullOrEmpty(objRoom.Email) || !string.IsNullOrEmpty(ExternalUsers))
                    {
                        strToAddress = objRoom.Email;
                        if (!string.IsNullOrEmpty(strToAddress))
                        {
                            strToAddress += ",";
                        }

                        strToAddress += ExternalUsers;
                        if (!string.IsNullOrEmpty(objCompany.ContactEmail))
                        {
                            strCCAddress = objCompany.ContactEmail;
                        }
                    }
                    else
                        return;

                }

                string StrSubject = strSubject;

                MessageBody.Replace("@@TABLE@@", "");
                MessageBody.Replace("@@MESSAGE@@", SensorMassage);
                MessageBody.Replace("@@COMPANYNAME@@", objCompany.Name);
                MessageBody.Replace("@@ROOMNAME@@", objRoom.RoomName);

                //CommonUtility.SendMail(FromAddress, strToAddress, strCCAddress, strBCCAddress, strNotificationAddress, StrSubject, MessageBody.ToString(), true);
                objUtils = new eTurnsUtility();
                objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
                objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                string DateTimeFormat = "MM/dd/yyyy";
                DateTime TZDateTimeNow = DateTime.UtcNow;
                if (objeTurnsRegionInfo != null)
                {
                    DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                    TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                }
                if (StrSubject != null)
                {
                    string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                    // EmailSubject = EmailSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                    StrSubject = Regex.Replace(StrSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                    {
                        StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                    }
                    if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                    {
                        StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", SessionHelper.RoomName, RegexOptions.IgnoreCase);
                    }

                    StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                }

                if (MessageBody != null)
                {
                    string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                    //EmailBody = EmailBody.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                    MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                    {
                        MessageBody = MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                    }
                    if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                    {
                        MessageBody = MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                    }
                    MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));

                }
                objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), objCompany.EnterPriseId, objCompany.ID, objRoom.ID, 0, null, "Web => EmailLink => SendMailForBinSensorNotWorking");
            }
            finally
            {
                MessageBody = null;
                objCompDAL = null;
                objCompany = null;
                objRoomDAL = null;
                objDAL = null;
                objRoom = null;
                objUtils = null;
                objEmailDAL = null;
            }
        }

        /// <summary>
        /// GetExternalUser
        /// </summary>
        /// <param name="EmailTemplate"></param>
        /// <returns></returns>
        private string GetExternalUser(string EmailTemplate)
        {
            string emails = string.Empty;
            EmailUserConfigurationDAL objExternalUser = new EmailUserConfigurationDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<EmailUserConfigurationDTO> lstExternalUser = objExternalUser.GetAllExternalUserRecords(EmailTemplate, true, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (lstExternalUser != null)
            {
                foreach (EmailUserConfigurationDTO item in lstExternalUser)
                {
                    if (!string.IsNullOrEmpty(emails))
                        emails += ",";
                    emails += item.Email;
                }
            }

            return emails;
        }

        public string GetResourceFileFullPath(string fileName, string Culture, long EntID, long CompanyID)
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

        public string GetResourceBaseFileFullPath(string fileName, string Culture)
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

    }

    public static class StringCipher
    {
        static byte[] bytes = ASCIIEncoding.ASCII.GetBytes("ZeroCool");

        public static string Encrypt(string originalString)
        {
            if (String.IsNullOrEmpty(originalString))
            {
                var ResourceBaseFilePath = ResourceRead.GetCompanyResourceBaseFileFullPath(System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"], "ResCommon", "en-US");
                string stringToEncryptCantBeNull = ResourceRead.GetResourceValueByKeyAndFullFilePath("StringToEncryptCantBeNull", ResourceBaseFilePath, 0, 0, 0, "ResCommon", "en-US");
                throw new ArgumentNullException
                       (stringToEncryptCantBeNull);
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return HttpContext.Current.Server.UrlEncode(Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length));
        }

        public static string Decrypt(string cryptedString)
        {
            if (String.IsNullOrEmpty(cryptedString))
            {
                var ResourceBaseFilePath = ResourceRead.GetCompanyResourceBaseFileFullPath(System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"], "ResCommon", "en-US");
                string stringToDecryptCantBeNull = ResourceRead.GetResourceValueByKeyAndFullFilePath("StringToDecryptCantBeNull", ResourceBaseFilePath, 0, 0, 0, "ResCommon", "en-US");
                throw new ArgumentNullException
                   (stringToDecryptCantBeNull);
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString));
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }

        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string initVector = "tu89geji340t89u2";

        // This constant is used to determine the keysize of the encryption algorithm.
        private const int keysize = 256;

        public static string Encrypt(string plainText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
    }

    public class EncryptDecryptQueryString
    {
        private byte[] key = { };
        private byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
        public string Decrypt(string stringToDecrypt, string sEncryptionKey)
        {
            byte[] inputByteArray = new byte[stringToDecrypt.Length + 1];
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(sEncryptionKey);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms,
                  des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string Encrypt(string stringToEncrypt, string SEncryptionKey)
        {
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(SEncryptionKey);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms,
                  des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
