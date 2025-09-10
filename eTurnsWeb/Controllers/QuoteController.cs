using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurns.DTO.Utils;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class QuoteController : eTurnsControllerBase
    {
        bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowanquotetobeSubmitted);
        bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowanquotetobeApproved);
        bool IsChangeQuote = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Allowanchangequote);
        //bool AllowConsignedItemToOrder = SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem);
        long RoomID = SessionHelper.RoomID;
        long CompanyID = SessionHelper.CompanyID;
        long EnterpriseId = SessionHelper.EnterPriceID;

        /// <summary>
        /// Quote List
        /// </summary>
        /// <returns></returns>
        public ActionResult QuoteList()
        {
            
            return View();
        }

        public ActionResult QuoteMasterListAjax(JQueryDataTableParamModel param)
        {
            string sortColumnName = string.Empty;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "quotenumber ASC";
            }

            int TotalRecordCount = 0;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            QuoteMasterDAL quoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
            var DataFromDB = quoteMasterDAL.GetQuoteMasterPagedData(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.RoomDateFormat, CurrentTimeZone).ToList();

            DataFromDB.ForEach(x =>
            {
                x.IsAbleToDelete = IsQuoteRecorddDeletable(x);
                x.RequiredDateStr = x.RequiredDate.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
                x.QuoteStatusText = ResQuoteMaster.GetQuoteStatusText(((eTurns.DTO.OrderStatus)x.QuoteStatus).ToString());
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult QuoteMasterHistory()
        {
            return PartialView("_QuoteListHistory");
        }

        public ActionResult QuoteMasterHistoryListAjax(JQueryDataTableParamModel param)
        {
            long QuoteId;
            long.TryParse(Convert.ToString(Request["QuoteId"]), out QuoteId);
            string sortColumnName = string.Empty;
            sortColumnName = Request["SortingField"].ToString();

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "quotenumber ASC";
            }

            int TotalRecordCount = 0;
            QuoteMasterDAL quoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
            var DataFromDB = quoteMasterDAL.GetPagedQuoteMasterChangeLog(QuoteId, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName).ToList();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }
        #region Unsubmitted Email Send

        [HttpPost]
        public JsonResult SetQuoteMailUnsubmitted(Int64 QuoteID)
        {
            try
            {
                QuoteDetailDAL objDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
                QuoteMasterDTO objQuoteDTO = new QuoteMasterDAL(SessionHelper.EnterPriseDBName).GetQuoteByIdFull(QuoteID);
                if (objQuoteDTO != null && objQuoteDTO.QuoteStatus == (int)QuoteStatus.UnSubmitted && objQuoteDTO.NoOfLineItems != null &&
                    objQuoteDTO.NoOfLineItems.GetValueOrDefault(0) > 0)
                {
                    List<QuoteDetailDTO> lstOrdDtlDTO = objDAL.GetDeletedOrUnDeletedQuoteDetailByQuoteGUIDPlain(objQuoteDTO.GUID, objQuoteDTO.Room, objQuoteDTO.CompanyID, false);
                    if (lstOrdDtlDTO != null && lstOrdDtlDTO.Count > 0)
                    {
                        SendMailQuoteUnSubmitted(objQuoteDTO);
                    }
                }
                return Json(new { Message = ResOrder.MailSendSuccessfully, Status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.ToString(), Status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Send Mail Quote UnSubmitted
        /// </summary>

        /// <param name="objQuoteDTO"></param>
        public void SendMailQuoteUnSubmitted(QuoteMasterDTO objQuoteDTO,long RoomId = 0,string RoomName = "")
        {
            Helper.AlertMail objAlertMail = new Helper.AlertMail();
            eMailMasterDAL objEmailDAL = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            NotificationDAL objNotificationDAL = null;
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            long roomid = RoomId == 0 ? SessionHelper.RoomID : RoomId;
            RoomName = string.IsNullOrEmpty(RoomName) ? SessionHelper.RoomName : RoomName;
            try
            {
                #region [Quote]

                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.QuoteUnSubmitted, roomid, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);

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

                            if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                            {
                                StrSubject = t.EmailTemplateDetail.lstEmailTemplateDtls.First().MailSubject;
                            }

                            string strToAddress = t.EmailAddress;
                            string strBCCAddress = ConfigurationManager.AppSettings["BCCAddress"];
                            eTurnsRegionInfo objeTurnsRegionInfoNew = null;

                            if (objEnterpriseDTO != null)
                            {
                                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(roomid, SessionHelper.CompanyID, -1);
                            }

                            string DateTimeFormat = "MM/dd/yyyy";
                            DateTime TZDateTimeNow = DateTime.UtcNow;

                            if (objeTurnsRegionInfoNew != null)
                            {
                                DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                                TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                            }

                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);

                            if (!string.IsNullOrEmpty(strToAddress))
                            {
                                objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
                                StringBuilder MessageBody = new StringBuilder();
                                objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
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
                                    StrSubject = StrSubject.Replace("@@QUOTENO@@", objQuoteDTO.QuoteNumber);

                                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                                    {
                                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objQuoteDTO.ReleaseNumber).Replace("@@releaseno@@", objQuoteDTO.ReleaseNumber).Replace("@@Releaseno@@", objQuoteDTO.ReleaseNumber).Replace("@@ReleaseNo@@", objQuoteDTO.ReleaseNumber);
                                    }

                                    if (objEnterpriseDTO != null)
                                    {
                                        DateTimeFormat = "MM/dd/yyyy";
                                        if (objeTurnsRegionInfoNew != null)
                                        {
                                            DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                                            TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                                        }
                                        if (StrSubject != null && StrSubject.ToLower().Contains("@@date@@"))
                                        {

                                            StrSubject = StrSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                                        }

                                        StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                                        StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", RoomName, RegexOptions.IgnoreCase);
                                        StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                                    }
                                }

                                MessageBody.Replace("@@QUOTENO@@", objQuoteDTO.QuoteNumber);
                                MessageBody.Replace("@@RELEASENO@@", objQuoteDTO.ReleaseNumber);
                                MessageBody.Replace("@@Supplier@@", objQuoteDTO.SupplierName);
                                string stratatTABLEatatTag = GetMailBodyQuoteForUnSubmitted(objQuoteDTO, DateTimeFormat);
                                string replacePart = string.Empty;

                                if (objEnterpriseDTO != null && (!string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterPriseDomainURL)))
                                {
                                    replacePart = objEnterpriseDTO.EnterPriseDomainURL;
                                }
                                else if (Request == null)
                                {
                                    replacePart = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                                }
                                else
                                {
                                    string urlPart = Request.Url.ToString();
                                    replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
                                }



                                {
                                    string strReplText = ResQuoteMaster.SeeAttachedFilesForQuoteDetail; 
                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                }

                                objeMailAttchList = new List<eMailAttachmentDTO>();
                                MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);

                                if (!string.IsNullOrWhiteSpace(RoomName))
                                {
                                    MessageBody.Replace("@@ROOMNAME@@", RoomName);
                                }

                                MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);

                                if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                                {
                                    MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                                }

                                MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));
                                Dictionary<string, string> Params = new Dictionary<string, string>();
                                Params.Add("DataGuids", objQuoteDTO.GUID.ToString());
                                objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                                objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                            }
                        });
                    }

                }
                #endregion
            }
            catch (Exception e)
            {
                Exception e1 = e;
            }
            finally
            {
                objEmailDAL = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                objeMailAttchList = null;
            }
        }

        private string GetMailBodyQuoteForUnSubmitted(QuoteMasterDTO obj, string DateTimeFormat = "MM/dd/yyyy")
        {
            string mailBody = "";
            string suppliername = "";
            string SupplierPartNo = string.Empty;
            string QuoteNumber = ResQuoteMaster.QuoteNumber;
            string ReqDateCap = ResQuoteMaster.RequiredDate;
            string QutStatus = ResQuoteMaster.QuoteStatus;
            string QtyReqQty = ResQuoteMaster.RequestedQuantity;
            SupplierMasterDTO objSupplierMasterDTP = null;
            string strRequiredDate = obj.RequiredDate.ToString(DateTimeFormat);

            if (obj.Supplier != null && obj.Supplier > 0)
            {
                objSupplierMasterDTP = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetSupplierByIDPlain(Int64.Parse(Convert.ToString(obj.Supplier)));
                if (objSupplierMasterDTP != null && objSupplierMasterDTP.ID > 0)
                    suppliername = objSupplierMasterDTP.SupplierName;
            }

            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                        <tr>
                            <td style=""width: 48%"">
                                <table style=""margin-left: 0px; width: 99%;"">
                                <tr>
                                    <td>
                                        <label style=""font-weight: bold;"">
                                            " + QuoteNumber + @": </label>
                                        <label style=""font-weight: bold;"">
                                            " + obj.QuoteNumber + @"</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>
                                            " + ResQuoteMaster.Comment + @": </label>
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
                                       " + ReqDateCap + @": </label>
                                    <label>
                                        " + strRequiredDate + @"</label>
                                </td>
                            </tr>
                           
                            <tr>
                                <td>
                                    <label>
                                        " + QutStatus + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(QuoteStatus), obj.QuoteStatus.ToString()).ToString() + @"</label>
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
                                        " + ResItemMaster.ItemNumber + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResQuoteMaster.Bin + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.Description + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + QtyReqQty + @"
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

            if (obj.QuoteListItem == null || obj.QuoteListItem.Count <= 0)
            {
                QuoteDetailDAL objQuoteDetailDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
                obj.QuoteListItem = PreparedQuoteLiteItemWithProperData(obj); //GetLineItemsFromSession(obj.ID);
                if (obj.QuoteStatus == (int)QuoteStatus.UnSubmitted)
                {
                    if (obj.QuoteListItem == null || obj.QuoteListItem.Count <= 0)
                    {
                        obj.QuoteListItem = objQuoteDetailDAL.GetQuoteDetailByQuoteGUIDFull(obj.GUID, obj.Room, obj.CompanyID, false);

                    }
                }
            }



            if (obj.QuoteListItem != null && obj.QuoteListItem.Count > 0)
            {

                foreach (var item in obj.QuoteListItem)
                {
                    string BinNumber = string.Empty;
                    if (item.BinName != null && !string.IsNullOrWhiteSpace(item.BinName))
                    {
                        BinNumber = item.BinName;
                    }
                    if (item.BinNumber != null && !string.IsNullOrWhiteSpace(item.BinNumber))
                    {
                        BinNumber = item.BinNumber;
                    }
                    string ReqQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    string desc = "&nbsp;";
                    int QuoteUOM = 1;

                    if (item.QuoteUOMValue != null && item.QuoteUOMValue >= 1)
                        QuoteUOM = Convert.ToInt32(item.QuoteUOMValue);

                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        ReqQty = item.RequestedQuantity.ToString();

                    if (obj.QuoteStatus == (int)QuoteStatus.Approved)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            ReqQty = item.ApprovedQuantity.ToString();
                    }

                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        ReqQty = item.RequestedQuantity.ToString();

                    if (obj.QuoteStatus == (int)QuoteStatus.Approved)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            ReqQty = item.ApprovedQuantity.ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(ReqQty) && ReqQty != "&nbsp;" && Convert.ToInt32(ReqQty) >= 0) //  && item.SerialNumberTracking == false
                    {
                        int intReqQty = 0;
                        if (item.IsAllowQuoteCostuom)
                            intReqQty = Convert.ToInt32(ReqQty) / QuoteUOM;
                        else
                            intReqQty = Convert.ToInt32(ReqQty);

                        if (intReqQty <= 0)
                            intReqQty = 0;

                        ReqQty = intReqQty.ToString();
                    }
                    if (item.RequiredDate != null && item.RequiredDate.HasValue)
                        ReqDate = strRequiredDate;

                    if (!string.IsNullOrEmpty(item.ItemDescription))
                    {
                        desc = item.ItemDescription;
                    }

                    trs += @"<tr>
                        <td>
                            " + item.ItemNumber + @"
                        </td>
                        <td>
                            " + BinNumber + @"
                        </td>
                        <td>
                            " + desc + @"
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
                trs += @"<tr>
                        <td colspan=""4"" style=""text-align:center"">
                           There is no item for this Quote
                        </td>
                    </tr>";
            }
            mailBody = mailBody.Replace("##TRS##", trs);

            return mailBody;
        }
        private List<QuoteDetailDTO> PreparedQuoteLiteItemWithProperData(QuoteMasterDTO objOrd)
        {
            ItemMasterDTO objItemMaster = null;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<QuoteDetailDTO> lstDetails = (List<QuoteDetailDTO>)SessionHelper.Get(GetSessionKey(objOrd.ID));

            if (lstDetails != null && lstDetails.Count > 0)
            {
                foreach (QuoteDetailDTO item in lstDetails)
                {
                    objItemMaster = null;
                    objItemMaster = objItemDAL.GetRecordByItemNumber(item.ItemNumber, objOrd.Room, objOrd.CompanyID);

                    if (objItemMaster != null)
                    {
                        item.ItemDescription = objItemMaster.Description;
                        item.SupplierPartNo = objItemMaster.SupplierPartNo;

                    }
                    item.RequiredDate = objOrd.RequiredDate;

                }
            }

            return lstDetails;
        }

        #endregion

        #region Quote Approval notification
        public void SendMailToApprovalAuthority(QuoteMasterDTO objQuote,long RoomID = 0,string RoomName = "")
        {
            List<eMailAttachmentDTO> lstAttachments = new List<eMailAttachmentDTO>();
            eTurnsWeb.Helper.AlertMail objAlertMail = new Helper.AlertMail();
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            NotificationDAL objNotificationDAL = null;
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);
            long roomid = RoomID == 0 ? SessionHelper.RoomID : RoomID;
            RoomName = string.IsNullOrEmpty(RoomName) ? SessionHelper.RoomName : RoomName;
            try
            {

                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.QuoteApproval, roomid, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);

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
                                    StrSubject = StrSubject.Replace("@@QUOTENO@@", objQuote.QuoteNumber);
                                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                                    {
                                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objQuote.ReleaseNumber).Replace("@@releaseno@@", objQuote.ReleaseNumber).Replace("@@Releaseno@@", objQuote.ReleaseNumber).Replace("@@ReleaseNo@@", objQuote.ReleaseNumber);
                                    }
                                    if (objEnterpriseDTO != null)
                                    {
                                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                        eTurnsRegionInfo objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(roomid, SessionHelper.CompanyID, -1);
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
                                MessageBody.Replace("@@QUOTENO@@", objQuote.QuoteNumber);
                                MessageBody.Replace("@@RELEASENO@@", objQuote.ReleaseNumber);
                                MessageBody.Replace("@@TABLE@@", GetMailBodyQuote(objQuote));
                                MessageBody.Replace("@@ROOMNAME@@", RoomName);
                                MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                                MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                                string replacePart = string.Empty;

                                if (objEnterpriseDTO != null && (!string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterPriseDomainURL)))
                                {
                                    replacePart = objEnterpriseDTO.EnterPriseDomainURL;
                                }
                                else if (Request == null)
                                {
                                    replacePart = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                                }
                                else
                                {
                                    string urlPart = Request.Url.ToString();
                                    replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
                                }

                                Dictionary<string, string> Params = new Dictionary<string, string>();
                                Params.Add("DataGuids", objQuote.GUID.ToString());
                                lstAttachments = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);

                                if (!string.IsNullOrWhiteSpace(strToAddress))
                                {
                                    List<string> EmailAddrs = strToAddress.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

                                    if (EmailAddrs != null && EmailAddrs.Count > 0)
                                    {
                                        foreach (var emailitem in EmailAddrs)
                                        {
                                            string strdata = objQuote.ID + "^" + objQuote.Room + "^" + objQuote.CompanyID + "^" + (objQuote.LastUpdatedBy) + "^" + SessionHelper.EnterPriceID.ToString() + "^" + objQuote.LastUpdatedBy + "^" + emailitem; ;
                                            string approvalURLData = StringCipher.Encrypt(strdata + "^APRV");
                                            string rejectURLData = StringCipher.Encrypt(strdata + "^RJKT");
                                            List<eMailAttachmentDTO> objeMailAttchListNew = new List<eMailAttachmentDTO>();

                                            if (lstAttachments != null)
                                            {
                                                foreach (var item in lstAttachments)
                                                {
                                                    objeMailAttchListNew.Add(item);
                                                }
                                            }

                                            MessageBody.Replace("@@APPROVEREJECT@@", @"<a href='" + replacePart + "/EmailLink/QuoteStatus?eKey=" + approvalURLData + "'>'" + ResCommon.Approve + "'</a> &nbsp;&nbsp;<a href='" + replacePart + "/EmailLink/QuoteStatus?eKey=" + rejectURLData + "'>'" + ResCommon.Reject + "'</a>");
                                            objAlertMail.CreateAlertMail(objeMailAttchListNew, StrSubject, MessageBody.ToString(), emailitem, t, objEnterpriseDTO);
                                        }
                                    }
                                }
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
        public string GetMailBodyQuote(QuoteMasterDTO obj)
        {
            string mailBody = "";
            int PriseSelectionOption = 1;
            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            RoomModuleSettingsDTO objRoomModuleSettingsDTO = new RoomModuleSettingsDTO();
            objRoomModuleSettingsDTO = objRoomDAL.GetRoomModuleSettings(SessionHelper.CompanyID, SessionHelper.RoomID, (long)SessionHelper.ModuleList.Quote);

            if (objRoomModuleSettingsDTO != null && objRoomModuleSettingsDTO.ID > 0)
            {
                PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption.GetValueOrDefault(0);
            }

            string QuoteCostTitle = ResQuoteMaster.QuoteCost;
            QuoteCostTitle = (ResQuoteMaster.QuoteCost); // PriseSelectionOption == 1 ? ResQuoteMaster.QuotePrice :
            string udfRequier = string.Empty;
            eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
            List<UDFDTO> DataFromDB = objUDFDAL.GetNonDeletedUDFsByUDFTableNamePlain("QuoteMaster", SessionHelper.RoomID, SessionHelper.CompanyID);
            string suppliername = "";
            string QutNumber = ResQuoteMaster.QuoteNumber;
            string ReqDateCap = ResQuoteMaster.RequiredDate;
            string QutStatus = ResQuoteMaster.QuoteStatus;
            string QutReqQty = ResQuoteMaster.RequestedQuantity;




            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                <tr>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label style=""font-weight: bold;"">
                                        " + QutNumber + @": </label>
                                    <label style=""font-weight: bold;"">
                                        " + obj.QuoteNumber + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + ResQuoteMaster.Comment + @": </label>
                                    <label>
                                        " + obj.Comment + @"</label>
                                </td>
                            </tr>
                            ";

            mailBody = mailBody + @"
                            <tr>
                                <td>
                                    <label>
                                        " + QuoteCostTitle + @": </label>
                                    <label>
                                        " + obj.QuoteCost.GetValueOrDefault(0).ToString("N" + SessionHelper.CurrencyDecimalDigits) + @"</label>
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

            mailBody = mailBody + @"</table>
                    </td>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label>
                                       " + ReqDateCap + @": </label>
                                    <label>
                                        " + obj.RequiredDate.ToString(SessionHelper.RoomDateFormat) + @"</label>
                                </td>
                            </tr>
                           
                            <tr>
                                <td>
                                    <label>
                                        " + QutStatus + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(QuoteStatus), obj.QuoteStatus.ToString()).ToString() + @"</label>
                                </td>
                            </tr>";


            if (DataFromDB != null && DataFromDB.Count > 0)
            {
                for (int i = 0; i < DataFromDB.Count; i++)
                {
                    if (DataFromDB[i].UDFColumnName == "UDF" + (i + 1))
                    {
                        string val = string.Empty;
                        val = ResourceUtils.GetResource("ResQuoteMaster", DataFromDB[i].UDFColumnName, true);

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

            mailBody = mailBody + @"</table>
                    </td>
                </tr>
                <tr>
                    <td colspan=""2"" style=""width: 99%"">
                        <table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.ItemNumber + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResQuoteMaster.Bin + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.Description + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + QutReqQty + @"
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
            if (obj.QuoteListItem == null || obj.QuoteListItem.Count <= 0)
            {
                QuoteDetailDAL objOrdDetailDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
                obj.QuoteListItem = objOrdDetailDAL.GetQuoteDetailByQuoteGUIDFull(obj.GUID, obj.Room, obj.CompanyID, false);
            }

            if (obj.QuoteListItem != null && obj.QuoteListItem.Count > 0)
            {

                foreach (var item in obj.QuoteListItem)
                {
                    string binname = "&nbsp;";
                    string ReqQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    if (item.BinID != null && item.BinID > 0)
                        binname = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(Int64.Parse(Convert.ToString(item.BinID)), SessionHelper.RoomID, SessionHelper.CompanyID).BinNumber;

                    int QuoteUOM = 1;
                    if (item.QuoteUOMValue != null && item.QuoteUOMValue >= 1)
                        QuoteUOM = Convert.ToInt32(item.QuoteUOMValue);

                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        ReqQty = item.RequestedQuantity.ToString();


                    if (obj.QuoteStatus == (int)QuoteStatus.Approved)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            ReqQty = item.ApprovedQuantity.ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(ReqQty) && ReqQty != "&nbsp;" && Convert.ToInt32(ReqQty) >= 0) //  && item.SerialNumberTracking == false
                    {
                        int intReqQty = Convert.ToInt32(ReqQty) / QuoteUOM;
                        if (intReqQty <= 0)
                            intReqQty = 0;

                        ReqQty = intReqQty.ToString();
                    }

                    if (item.RequiredDate != null && item.RequiredDate.HasValue)
                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture);


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
                trs += @"<tr>
                        <td colspan=""4"" style=""text-align:center"">
                           There is no item for this Quote
                        </td>
                    </tr>";
            }
            mailBody = mailBody.Replace("##TRS##", trs);

            return mailBody;
        }

        #endregion

        #region Quote Approve Reject Notification
        public void SendMailForQuoteApprovedOrReject(QuoteMasterDTO objQuote, string AprvRejString, string QuoteRequesterEmailAddress, string QuoteApproverEmailAddress,long RoomID = 0, string RoomName = "")
        {
            List<eMailAttachmentDTO> lstAttachments = new List<eMailAttachmentDTO>();
            eTurnsWeb.Helper.AlertMail objAlertMail = new Helper.AlertMail();
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            NotificationDAL objNotificationDAL = null;
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            long roomid = RoomID == 0 ? SessionHelper.RoomID : RoomID;
            RoomName = RoomName == "" ? SessionHelper.RoomName : RoomName;
            try
            {
                EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);

                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.QuoteApproveReject, roomid, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);

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
                                    if (!string.IsNullOrWhiteSpace(QuoteRequesterEmailAddress))
                                        strToAddress = strToAddress.Replace("[Requester]", QuoteRequesterEmailAddress);
                                    else
                                        strToAddress = strToAddress.Replace("[Requester]", "");
                                }
                                if (strToAddress.Contains("[Approver]"))
                                {
                                    if (!string.IsNullOrWhiteSpace(QuoteApproverEmailAddress))
                                        strToAddress = strToAddress.Replace("[Approver]", QuoteApproverEmailAddress);
                                    else
                                        strToAddress = strToAddress.Replace("[Approver]", "");
                                }
                                List<string> EmailAddrs = strToAddress.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                EmailAddrs = EmailAddrs.Distinct().ToList();
                                strToAddress = string.Join(",", EmailAddrs);

                                MessageBody = new StringBuilder();
                                objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();

                                if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Count() > 0)
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
                                    StrSubject = StrSubject.Replace("@@QUOTENO@@", objQuote.QuoteNumber);
                                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                                    {
                                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objQuote.ReleaseNumber).Replace("@@releaseno@@", objQuote.ReleaseNumber).Replace("@@Releaseno@@", objQuote.ReleaseNumber).Replace("@@ReleaseNo@@", objQuote.ReleaseNumber);
                                    }
                                    if (objEnterpriseDTO != null)
                                    {
                                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                        eTurnsRegionInfo objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(roomid, SessionHelper.CompanyID, -1);
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

                                MessageBody.Replace("@@QUOTENO@@", objQuote.QuoteNumber);
                                MessageBody.Replace("@@RELEASENO@@", objQuote.ReleaseNumber);
                                MessageBody.Replace("@@ROOMNAME@@", RoomName);
                                MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                                MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                                MessageBody.Replace("@@APRVREJ@@", AprvRejString);
                                Dictionary<string, string> Params = new Dictionary<string, string>();
                                Params.Add("DataGuids", objQuote.GUID.ToString());
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


        #endregion
        #region Quote To Supplier Notification
        public void SendMailToSupplier(QuoteMasterDTO objQuote, long RoomID = 0, string RoomName = "",bool IsCallNewFuction=false)
        {

            /// <summary>
            ///  GET: /Master/ for Create
            Helper.AlertMail objAlertMail = new Helper.AlertMail();
            /// </summary>
            eMailMasterDAL objEmailDAL = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            NotificationDAL objNotificationDAL = null;
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsNew = new List<NotificationDTO>();
            List<SupplierMasterDTO> ToSuppliers = new List<SupplierMasterDTO>();
            SupplierMasterDAL objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            long roomid = RoomID == 0 ? SessionHelper.RoomID : RoomID;
            RoomName = RoomName == "" ? SessionHelper.RoomName : RoomName;
            try
            {
                if (objQuote != null)
                {
                    if (objQuote.QuoteSupplierIdsCSV != null && (!string.IsNullOrWhiteSpace(objQuote.QuoteSupplierIdsCSV)))
                    {
                        ToSuppliers = objSupplierDAL.GetNonDeletedSupplierByIDsNormal(objQuote.QuoteSupplierIdsCSV, roomid, SessionHelper.CompanyID);
                    }
                }
                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplateQuoteandOrder((long)MailTemplate.QuoteToSupplier, roomid, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);
                    if (ToSuppliers != null && ToSuppliers.Count > 0)
                    {
                        foreach (SupplierMasterDTO supplierDTO in ToSuppliers)
                        {
                            List<NotificationDTO> objNotificationDTO = new List<NotificationDTO>();

                            //foreach (NotificationDTO obj in lstNotifications)
                            //{
                            objNotificationDTO = lstNotifications.Where(t => (t.SupplierIds ?? String.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(Convert.ToString(supplierDTO.ID))).ToList();
                            if (objNotificationDTO != null)
                            {
                                foreach (NotificationDTO obj in objNotificationDTO)
                                {
                                    if (lstNotificationsNew.Where(t => t.ID == obj.ID).Count() == 0)
                                    {
                                        lstNotificationsNew.Add(obj);
                                    }
                                }
                            }
                            //}

                        }
                    }
                    else
                    {

                        foreach (NotificationDTO obj in lstNotifications)
                        {

                            if (obj != null)
                            {
                                if (lstNotificationsNew.Where(t => t.ID == obj.ID).Count() == 0)
                                {
                                    lstNotificationsNew.Add(obj);
                                }
                            }
                        }
                    }
                    lstNotificationsNew.ForEach(t =>
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
                            if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                            {
                                StrSubject = t.EmailTemplateDetail.lstEmailTemplateDtls.First().MailSubject;
                            }
                            string strToAddress = t.EmailAddress;
                            string strBCCAddress = ConfigurationManager.AppSettings["BCCAddress"];
                            eTurnsRegionInfo objeTurnsRegionInfoNew = null;
                            if (objEnterpriseDTO != null)
                            {
                                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(roomid, SessionHelper.CompanyID, -1);
                            }
                            string DateTimeFormat = "MM/dd/yyyy";
                            DateTime TZDateTimeNow = DateTime.UtcNow;
                            if (objeTurnsRegionInfoNew != null)
                            {
                                DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                                TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                            }
                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                            if (!string.IsNullOrEmpty(strToAddress))
                            {
                                objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
                                StringBuilder MessageBody = new StringBuilder();
                                objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
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
                                    StrSubject = StrSubject.Replace("@@QUOTENO@@", objQuote.QuoteNumber);

                                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                                    {
                                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objQuote.ReleaseNumber).Replace("@@releaseno@@", objQuote.ReleaseNumber).Replace("@@Releaseno@@", objQuote.ReleaseNumber).Replace("@@ReleaseNo@@", objQuote.ReleaseNumber);
                                    }

                                    if (objEnterpriseDTO != null)
                                    {
                                        DateTimeFormat = "MM/dd/yyyy";
                                        if (objeTurnsRegionInfoNew != null)
                                        {
                                            DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                                            TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                                        }
                                        if (StrSubject != null && StrSubject.ToLower().Contains("@@date@@"))
                                        {

                                            StrSubject = StrSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                                        }

                                        StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                                        StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", RoomName, RegexOptions.IgnoreCase);
                                        StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                                    }
                                }

                                string SupplierName = objQuote.SupplierName;

                                if (string.IsNullOrEmpty(objQuote.SupplierName) && string.IsNullOrWhiteSpace(objQuote.SupplierName)
                                    && objQuote.Supplier.GetValueOrDefault(0) > 0)
                                {
                                    var QuoteSupplier = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetSupplierByIDPlain(objQuote.Supplier.GetValueOrDefault(0));
                                    if (QuoteSupplier != null && QuoteSupplier.ID > 0)
                                    {
                                        SupplierName = QuoteSupplier.SupplierName;
                                        objQuote.SupplierName = SupplierName;
                                    }
                                }

                                MessageBody.Replace("@@QUOTENO@@", objQuote.QuoteNumber);
                                MessageBody.Replace("@@RELEASENO@@", objQuote.ReleaseNumber);
                                MessageBody.Replace("@@Supplier@@", SupplierName);
                                MessageBody.Replace("@@COMMENT@@", objQuote.Comment);
                                MessageBody.Replace("@@SHIPPINGMETHOD@@", objQuote.ShipViaName);
                                MessageBody.Replace("@@REQUIREDDATE@@", objQuote.RequiredDate.ToString(DateTimeFormat));
                                MessageBody.Replace("@@QUOTESTATUS@@", Enum.Parse(typeof(QuoteStatus), objQuote.QuoteStatus.ToString()).ToString());
                                string stratatTABLEatatTag = GetMailBodySupplierQuote(objQuote, DateTimeFormat);
                                string replacePart = string.Empty;

                                if (objEnterpriseDTO != null && (!string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterPriseDomainURL)))
                                {
                                    replacePart = objEnterpriseDTO.EnterPriseDomainURL;
                                }
                                else if (Request == null)
                                {
                                    replacePart = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                                }
                                else
                                {
                                    string urlPart = Request.Url.ToString();
                                    replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
                                }
                                StringBuilder strMailBody = new StringBuilder();
                                strMailBody = MessageBody;

                                {
                                    string strReplText = ResQuoteMaster.SeeAttachedFilesForQuoteDetail;


                                    MessageBody.Replace("@@TABLE@@", strReplText);
                                }


                                objeMailAttchList = new List<eMailAttachmentDTO>();
                                MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);

                                if (!string.IsNullOrWhiteSpace(RoomName))
                                {
                                    MessageBody.Replace("@@ROOMNAME@@", RoomName);
                                }

                                MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);

                                if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                                {
                                    MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                                }

                                MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));
                                Dictionary<string, string> Params = new Dictionary<string, string>();
                                Params.Add("DataGuids", objQuote.GUID.ToString());
                                List<eMailAttachmentDTO> objeSupplierMailAttchList = new List<eMailAttachmentDTO>();
                                if(IsCallNewFuction)
                                objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlertQuoteandOrder(t, objEnterpriseDTO, Params);
                                else
                                    objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                                if (objeMailAttchList != null)
                                {
                                    objeSupplierMailAttchList.AddRange(objeMailAttchList);
                                }
                                t.RoomID = roomid;
                                objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                                foreach (SupplierMasterDTO objSupplier in ToSuppliers)
                                {
                                    objeMailAttchList = new List<eMailAttachmentDTO>();
                                    objeMailAttchList.AddRange(objeSupplierMailAttchList);

                                    MessageBody = strMailBody;
                                    MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);

                                    if (!string.IsNullOrWhiteSpace(RoomName))
                                    {
                                        MessageBody.Replace("@@ROOMNAME@@", RoomName);
                                    }

                                    MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);

                                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                                    {
                                        MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                                    }

                                    MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));
                                    if (objSupplier.IsEmailPOInBody)
                                    {
                                        if (MessageBody != null && !string.IsNullOrEmpty(MessageBody.ToString()) && !string.IsNullOrWhiteSpace(MessageBody.ToString()))
                                        {
                                            GetMailBodySupplierDynamicContent(MessageBody, objQuote, objSupplier, DateTimeFormat);
                                        }
                                        MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                    }
                                    else if (!objSupplier.IsEmailPOInCSV && !objSupplier.IsEmailPOInPDF)
                                    {
                                        if (MessageBody != null && !string.IsNullOrEmpty(MessageBody.ToString()) && !string.IsNullOrWhiteSpace(MessageBody.ToString()))
                                        {
                                            GetMailBodySupplierDynamicContent(MessageBody, objQuote, objSupplier, DateTimeFormat);
                                        }
                                        MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                    }
                                    else
                                    {
                                        string strReplText = ResQuoteMaster.SeeAttachedFilesForQuoteDetail;
                                        MessageBody.Replace("@@TABLE@@", strReplText);
                                    }
                                    if (objSupplier.Email != null && (!string.IsNullOrWhiteSpace(objSupplier.Email)))
                                    {
                                        strToAddress = objSupplier.Email;
                                        objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                                    }
                                }
                            }


                        });
                    }
                }


            }
            finally
            {
                objEmailDAL = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                objeMailAttchList = null;
            }
        }
        private string GetMailBodySupplierQuote(QuoteMasterDTO obj, string DateTimeFormat = "MM/dd/yyyy")
        {
            string mailBody = "";
            string suppliername = "";
            string SupplierPartNo = string.Empty;
            string QuoteNumber = ResQuoteMaster.QuoteNumber;
            string ReqDateCap = ResQuoteMaster.RequiredDate;
            string QuteStatus = ResQuoteMaster.QuoteStatus;
            string QuoteReqQty = ResQuoteMaster.RequestedQuantity;
            SupplierMasterDTO objSupplierMasterDTP = null;

            string strReqDate = obj.RequiredDate.ToString(DateTimeFormat);//obj.RequiredDate.ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture);




            if (obj.Supplier != null && obj.Supplier > 0)
            {
                objSupplierMasterDTP = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetSupplierByIDPlain(Int64.Parse(Convert.ToString(obj.Supplier)));
                if (objSupplierMasterDTP != null && objSupplierMasterDTP.ID > 0)
                    suppliername = objSupplierMasterDTP.SupplierName;
            }

            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                        <tr>
                            <td style=""width: 48%"">
                                <table style=""margin-left: 0px; width: 99%;"">
                                <tr>
                                    <td>
                                        <label style=""font-weight: bold;"">
                                            " + QuoteNumber + @": </label>
                                        <label style=""font-weight: bold;"">
                                            " + obj.QuoteNumber + @"</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>
                                            " + ResQuoteMaster.Comment + @": </label>
                                        <label>
                                            " + obj.Comment + @"</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>
                                            " + ResQuoteMaster.ShippingMethod + @": </label>
                                        <label>
                                            " + obj.ShipViaName + @"</label>
                                    </td>
                                </tr>
                            </table>
                    </td>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label>
                                       " + ReqDateCap + @": </label>
                                    <label>
                                        " + strReqDate + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + ResQuoteMaster.Supplier + @": </label>
                                    <label>
                                        " +
                                          suppliername
                                          + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + QuteStatus + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(QuoteStatus), obj.QuoteStatus.ToString()).ToString() + @"</label>
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
                                        " + ResItemMaster.ItemNumber + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResQuoteMaster.Bin + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.Description + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + QuoteReqQty + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ReqDateCap + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.SupplierPartNo + @"
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
            if (obj.QuoteListItem == null || obj.QuoteListItem.Count <= 0)
            {
                QuoteDetailDAL objQuoteDetailDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
                obj.QuoteListItem = objQuoteDetailDAL.GetQuoteDetailByQuoteGUIDFull(obj.GUID, obj.Room, obj.CompanyID, false);

            }


            if (obj.QuoteListItem != null && obj.QuoteListItem.Count > 0)
            {
                foreach (var item in obj.QuoteListItem)
                {
                    string binname = "&nbsp;";
                    string ReqQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    if (item.BinID != null && item.BinID > 0)
                        binname = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(Int64.Parse(Convert.ToString(item.BinID)), SessionHelper.RoomID, SessionHelper.CompanyID).BinNumber;

                    int QuoteUOM = 1;

                    if (item.QuoteUOMValue != null && item.QuoteUOMValue >= 1)
                        QuoteUOM = Convert.ToInt32(item.QuoteUOMValue);

                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        ReqQty = item.RequestedQuantity.ToString();

                    if (obj.QuoteStatus == (int)QuoteStatus.Approved)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            ReqQty = item.ApprovedQuantity.ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(ReqQty) && ReqQty != "&nbsp;" && Convert.ToInt32(ReqQty) >= 0) //  && item.SerialNumberTracking == false
                    {
                        int intReqQty = 0;
                        if (item.IsAllowQuoteCostuom)
                            intReqQty = Convert.ToInt32(ReqQty) / QuoteUOM;
                        else
                            intReqQty = Convert.ToInt32(ReqQty);
                        if (intReqQty <= 0)
                            intReqQty = 0;

                        ReqQty = intReqQty.ToString();
                    }

                    if (item.RequiredDate != null && item.RequiredDate.HasValue)
                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(DateTimeFormat);

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
                         <td>
                            " + item.SupplierPartNo + @"
                        </td>
                    </tr>";

                }
            }
            else
            {
                trs += @"<tr>
                        <td colspan=""4"" style=""text-align:center"">
                           There is no item for this Quote
                        </td>
                    </tr>";
            }
            mailBody = mailBody.Replace("##TRS##", trs);

            return mailBody;
        }

        private void GetMailBodySupplierDynamicContent(StringBuilder MailBody, QuoteMasterDTO obj, SupplierMasterDTO lstSupplier, string DateTimeFormat = "MM/dd/yyyy")
        {
            string suppliername = "";
            SupplierMasterDTO objSupplierMasterDTP = null;

            try
            {
                if (lstSupplier != null)
                {
                    objSupplierMasterDTP = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetSupplierByIDPlain(Int64.Parse(Convert.ToString(lstSupplier.ID)));
                    if (objSupplierMasterDTP != null && objSupplierMasterDTP.ID > 0)
                        suppliername = lstSupplier.SupplierName;

                }

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(MailBody.ToString());
                var tableNodes = doc.DocumentNode.SelectNodes("//table");

                if (tableNodes != null && tableNodes.Count() > 0)
                {
                    string[] quoteLineItemHeaders = new string[6];
                    quoteLineItemHeaders[0] = "@~ITEMNUMBER~@";
                    quoteLineItemHeaders[1] = "@~BIN~@";
                    quoteLineItemHeaders[2] = "@~DESCRIPTION~@";
                    quoteLineItemHeaders[3] = "@~REQUESTEDQUANTITY~@";
                    quoteLineItemHeaders[4] = "@~REQUIREDDATE~@";
                    quoteLineItemHeaders[5] = "@~SUPPLIERPARTNO~@";
                    Dictionary<int, string> quoteLineItemsHeader = new Dictionary<int, string>();
                    bool isCorrectTableFound = false;
                    int startingIndexToReplace = 0;
                    int lengthToReplace = 0;

                    if (obj.QuoteListItem == null || obj.QuoteListItem.Count <= 0)
                    {
                        QuoteDetailDAL objQutDetailDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
                        obj.QuoteListItem = objQutDetailDAL.GetQuoteDetailByQuoteGUIDFull(obj.GUID, obj.Room, obj.CompanyID, false);
                    }
                    if (lstSupplier != null)
                    {

                        {
                            objSupplierMasterDTP = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetSupplierByIDPlain(Int64.Parse(Convert.ToString(lstSupplier.ID)));
                            if (objSupplierMasterDTP != null && objSupplierMasterDTP.ID > 0)
                            {
                                if (objSupplierMasterDTP.IsSupplierReceivesKitComponents && obj.QuoteListItem != null && obj.QuoteListItem.Count > 0)
                                {
                                    List<QuoteDetailDTO> objQuoteItemList = new List<QuoteDetailDTO>();

                                    foreach (var item in obj.QuoteListItem)
                                    {
                                        objQuoteItemList.Add(item);

                                        if (item.ItemType == 3)
                                        {
                                            IEnumerable<KitDetailDTO> objKitDeailList = new KitDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByKitGUID(item.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true);

                                            foreach (var KitCompitem in objKitDeailList)
                                            {
                                                QuoteDetailDTO objQuoteDetailDTO = new QuoteDetailDTO()
                                                {
                                                    ItemNumber = KitCompitem.ItemNumber + "&nbsp;" + " <I>"+ string.Format(ResOrder.ComponentOfKit, item.ItemNumber) + "</I>",
                                                    BinNumber = item.BinNumber,
                                                    ApprovedQuantity = KitCompitem.QuantityPerKit.GetValueOrDefault(0) * item.ApprovedQuantity.GetValueOrDefault(),
                                                    RequiredDate = item.RequiredDate,
                                                };
                                                objQuoteItemList.Add(objQuoteDetailDTO);
                                            }
                                        }
                                    }
                                    obj.QuoteListItem.Clear();
                                    obj.QuoteListItem = objQuoteItemList;
                                }
                            }
                        }
                        foreach (var node in tableNodes)
                        {
                            var rows = node.SelectNodes(node.XPath + "//tr"); //node.SelectNodes(node.FirstChild.XPath + "//tr");//node.SelectNodes("//tr");
                            var headerRows = node.SelectNodes(node.XPath + "//th"); //node.SelectNodes(node.FirstChild.XPath + "//th");

                            if ((rows != null && rows.Count() > 0 && rows.Count() < 2) || (headerRows != null && headerRows.Count() > 0 && headerRows.Count() < 2))
                            {
                                foreach (var raw in rows)
                                {
                                    var tds = raw.SelectNodes(raw.XPath + "//td");
                                    if (tds != null && tds.Count() > 0 && tds.Count() <= 6)
                                    {
                                        bool isCorrectTable = true;
                                        foreach (var td in tds)
                                        {
                                            if (string.IsNullOrEmpty(td.InnerText) || string.IsNullOrWhiteSpace(td.InnerText) ||
                                                    !quoteLineItemHeaders.Contains(td.InnerText.Trim()))
                                            {
                                                isCorrectTable = false;
                                                break;
                                            }
                                        }

                                        if (isCorrectTable)
                                        {
                                            isCorrectTableFound = true;
                                            for (int cnt = 0; cnt < tds.Count(); cnt++)
                                            {
                                                quoteLineItemsHeader.Add(cnt, tds[cnt].InnerText.Trim());
                                            }
                                            break;
                                        }
                                    }
                                }

                                if (isCorrectTableFound)
                                {
                                    startingIndexToReplace = node.InnerStartIndex;//node.OuterStartIndex;//node.InnerStartIndex;
                                    lengthToReplace = node.InnerLength;//node.OuterLength; //node.InnerLength;
                                                                       //startingTag = node.Name;
                                    break;
                                }
                            }
                        }

                        if (lengthToReplace > 0)
                        {
                            var stringToReplace = MailBody.ToString().Substring(startingIndexToReplace, lengthToReplace);

                            if (quoteLineItemsHeader != null && quoteLineItemsHeader.Any() && quoteLineItemsHeader.Count() > 0)
                            {
                                var QuoteLineItemBodyHtml = @"<thead><tr role=""row"">";
                                foreach (var key in quoteLineItemsHeader.OrderBy(e => e.Key))
                                {
                                    var tmpColName = GetQuoteColumnHeaderResourceValue(key.Value.ToLower(), obj);
                                    QuoteLineItemBodyHtml += @"<th  style=""width: 10%; text-align: left;"">" + tmpColName + "</th>";
                                }
                                QuoteLineItemBodyHtml += "</tr></thead>";

                                if (obj.QuoteListItem != null && obj.QuoteListItem.Any() && obj.QuoteListItem.Count > 0)
                                {
                                    QuoteLineItemBodyHtml += "<tbody>";
                                    foreach (var item in obj.QuoteListItem)
                                    {
                                        string binname = "&nbsp;";
                                        string ReqQty = "&nbsp;";
                                        string ReqDate = "&nbsp;";
                                        if (item.BinID != null && item.BinID > 0)
                                            binname = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(Int64.Parse(Convert.ToString(item.BinID)), SessionHelper.RoomID, SessionHelper.CompanyID).BinNumber;

                                        int QuoteUOM = 1;

                                        if (item.QuoteUOMValue != null && item.QuoteUOMValue >= 1)
                                            QuoteUOM = Convert.ToInt32(item.QuoteUOMValue);

                                        if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                                            ReqQty = item.RequestedQuantity.ToString();

                                        if (obj.QuoteStatus == (int)QuoteStatus.Approved)
                                        {
                                            if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                                                ReqQty = item.ApprovedQuantity.ToString();
                                        }

                                        if (!string.IsNullOrWhiteSpace(ReqQty) && ReqQty != "&nbsp;" && Convert.ToInt32(ReqQty) >= 0) //  && item.SerialNumberTracking == false
                                        {
                                            int intReqQty = 0;
                                            if (item.IsAllowQuoteCostuom)
                                                intReqQty = Convert.ToInt32(ReqQty) / QuoteUOM;
                                            else
                                                intReqQty = Convert.ToInt32(ReqQty);
                                            if (intReqQty <= 0)
                                                intReqQty = 0;

                                            ReqQty = intReqQty.ToString();
                                        }

                                        if (item.RequiredDate != null && item.RequiredDate.HasValue)
                                            ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(DateTimeFormat);

                                        QuoteLineItemBodyHtml += "<tr>";
                                        foreach (var key in quoteLineItemsHeader.OrderBy(e => e.Key))
                                        {
                                            switch (key.Value.ToLower())
                                            {
                                                case "@~itemnumber~@":
                                                    QuoteLineItemBodyHtml += "<td>" + item.ItemNumber + "</td>";
                                                    break;
                                                case "@~bin~@":
                                                    QuoteLineItemBodyHtml += "<td>" + binname + "</td>";
                                                    break;
                                                case "@~description~@":
                                                    QuoteLineItemBodyHtml += "<td>" + item.ItemDescription + "</td>";
                                                    break;
                                                case "@~requestedquantity~@":
                                                    QuoteLineItemBodyHtml += "<td>" + ReqQty + "</td>";
                                                    break;
                                                case "@~requireddate~@":
                                                    QuoteLineItemBodyHtml += "<td>" + ReqDate + "</td>";
                                                    break;
                                                case "@~supplierpartno~@":
                                                    QuoteLineItemBodyHtml += "<td>" + item.SupplierPartNo + "</td>";
                                                    break;
                                                default:
                                                    QuoteLineItemBodyHtml += "<td>&nbsp;</td>";
                                                    break;
                                            }
                                        }
                                        QuoteLineItemBodyHtml += "</tr>";
                                    }
                                    QuoteLineItemBodyHtml += "</tbody>";
                                }
                                else
                                {
                                    QuoteLineItemBodyHtml += "<tbody><tr><td colspan='" + quoteLineItemsHeader.Count() + "'  style='text-align:center'>There is no item for this Quote</td><tr></tbody>";
                                }
                                MailBody = MailBody.Replace(stringToReplace, QuoteLineItemBodyHtml);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
            }

        }
        private string GetQuoteColumnHeaderResourceValue(string ColumnName, QuoteMasterDTO obj)
        {
            switch (ColumnName)
            {
                case "@~itemnumber~@":
                    return ResItemMaster.ItemNumber;
                case "@~bin~@":
                    return ResQuoteMaster.Bin;
                case "@~description~@":
                    return ResItemMaster.Description;
                case "@~requestedquantity~@":
                    return ResQuoteMaster.RequestedQuantity;
                case "@~requireddate~@":
                    return ResQuoteMaster.RequiredDate;
                case "@~supplierpartno~@":
                    return ResItemMaster.SupplierPartNo;
                default:
                    return string.Empty;
            }

        }
        #endregion


        /// <returns></returns>
        public ActionResult QuoteCreate()
        {
            AutoQuoteNumberGenerate objAutoNumber = null;
            AutoSequenceDAL objAutoSeqDAL = null;
            QuoteMasterDAL objOrderDAL = null;
            QuoteMasterDTO objDTO = null;
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);

            try
            {
                SessionHelper.RomoveSessionByKey(GetSessionKey(0));
                SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(0));
                SessionHelper.RomoveSessionByKey("AddItemToQuote_" + RoomID + "_" + CompanyID);
                objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
                objAutoNumber = objAutoSeqDAL.GetNextQuoteNumber(RoomID, CompanyID,EnterpriseId,0);
                string quoteNumber = objAutoNumber.QuoteNumber;

                if (quoteNumber != null && (!string.IsNullOrEmpty(quoteNumber)))
                {
                    quoteNumber = quoteNumber.Length > 22 ? quoteNumber.Substring(0, 22) : quoteNumber;
                }
                int ReleaseNo = 1;

                if (!string.IsNullOrWhiteSpace(quoteNumber))
                {
                    objOrderDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
                    ReleaseNo = objOrderDAL.GetNextQuoteReleaseNumber(quoteNumber, null, RoomID, CompanyID);
                }

                objDTO = new QuoteMasterDTO()
                {
                    RequiredDate = datetimetoConsider,
                    QuoteNumber = quoteNumber,
                    QuoteStatus = (int)QuoteStatus.UnSubmitted,
                    ReleaseNumber = ReleaseNo.ToString(),
                    LastUpdated = DateTimeUtility.DateTimeNow,
                    Created = DateTimeUtility.DateTimeNow,
                    CreatedBy = SessionHelper.UserID,
                    CreatedByName = SessionHelper.UserName,
                    LastUpdatedBy = SessionHelper.UserID,
                    CompanyID = CompanyID,
                    Room = RoomID,
                    RoomName = SessionHelper.RoomName,
                    UpdatedByName = SessionHelper.UserName,
                    QuoteDate = datetimetoConsider,
                    //AutoOrderNumber = objAutoNumber,
                    QuoteNumber_ForSorting = objAutoNumber.QuoteNumberForSorting,
                    IsOnlyFromUI = true,
                };

                objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);
                ViewBag.UDFs = GetUDFDataPageWise("QuoteMaster");

                foreach (var i in ViewBag.UDFs)
                {
                    string _UDFColumnName = (string)i.UDFColumnName;
                    ViewData[_UDFColumnName] = i.UDFDefaultValue;
                }

                ViewBag.QuoteStatusList = GetQuoteStatusList(objDTO, "create");

                return PartialView("_CreateQuote", objDTO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAutoNumber = null;
                objAutoSeqDAL = null;
                objOrderDAL = null;
                objDTO = null;
            }

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult SaveQuote(QuoteMasterDTO objDTO)
        {
            if (Request.Form["hdnIsEditOrderLineItems"] != "true")
            {
                var ignoreProperty = new List<string>() { };
                var valResult = DTOCommonUtils.ValidateDTO<QuoteMasterDTO>(objDTO, ControllerContext, ignoreProperty);

                if (valResult.HasErrors())
                {
                    return Json(new { Message = ResMessage.InvalidModel, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }
            }

            QuoteMasterDAL quoteMasterDAL = null;
            CommonDAL objCDAL = null;
            List<QuoteDetailDTO> lstQuoteDetailDTO = null;

            //try
            //{
                long SessionUserId = SessionHelper.UserID;
                string message = "";
                string status = "";

                quoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
                objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                if (string.IsNullOrEmpty(objDTO.QuoteNumber))
                {
                    message = string.Format(ResMessage.Required, ResQuoteMaster.QuoteNumber);
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }

                if (!string.IsNullOrEmpty(objDTO.QuoteNumber) && objDTO.QuoteNumber != null)
                {
                    if (objDTO.QuoteNumber.Length > 22)
                    {
                        message = ResQuoteMaster.QuoteNumberLengthUpto22Char;
                        status = "fail";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                }

                UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                CommonDAL commonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                string quoteUDFRequier = string.Empty;
                commonDAL.CheckUDFIsRequired("QuoteMaster", objDTO.UDF1, objDTO.UDF2, objDTO.UDF3, objDTO.UDF4, objDTO.UDF5, out quoteUDFRequier, CompanyID, RoomID,SessionHelper.EnterPriceID,SessionHelper.UserID);

                if (!string.IsNullOrEmpty(quoteUDFRequier))
                {
                    return Json(new { Message = quoteUDFRequier, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }

                objDTO.LastUpdatedBy = SessionHelper.UserID;
                objDTO.Room = RoomID;
                objDTO.RequiredDate = DateTime.ParseExact(objDTO.RequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);

                if (string.IsNullOrEmpty(objDTO.QuoteNumber_ForSorting))
                {
                    objDTO.QuoteNumber_ForSorting = objDTO.QuoteNumber;
                }

                //----------------------Check For Order Number Duplication----------------------
                //
                string strOK = string.Empty;
                //RoomDTO roomDTO = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);

                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                string columnList = "ID,RoomName,IsAllowQuoteDuplicate,QuoteAutoSequence,DoSendQuotetoVendor";
                RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

                if (roomDTO.IsAllowQuoteDuplicate != true)
                {
                    if (quoteMasterDAL.IsQuoteNumberDuplicateById(objDTO.QuoteNumber, objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID))
                    {
                        strOK = "duplicate";
                    }
                }

                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResQuoteMaster.QuoteNumber, objDTO.QuoteNumber);
                    status = "duplicate";
                }
                else
                {
                    if (objDTO.ID == 0)
                    {
                        if (roomDTO.QuoteAutoSequence.GetValueOrDefault(0) == 0)
                        {
                            var orderCount = quoteMasterDAL.GetCountOfQuoteByQuoteNumber(RoomID, CompanyID, objDTO.QuoteNumber);
                            objDTO.ReleaseNumber = Convert.ToString(orderCount + 1);
                        }

                        objDTO.GUID = Guid.NewGuid();
                        objDTO.WhatWhereAction = "Quote";
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                        if (objDTO.QuoteStatus == (int)QuoteStatus.Submitted)
                        {
                            objDTO.RequesterID = SessionHelper.UserID;
                        }
                        else if (objDTO.QuoteStatus == (int)QuoteStatus.Approved)
                        {
                            objDTO.ApproverID = SessionHelper.UserID;
                        }

                        objDTO = quoteMasterDAL.InsertQuoteMaster(objDTO);
                        long ReturnVal = objDTO.ID;

                        if (ReturnVal > 0)
                        {
                            message = ResMessage.SaveMessage;
                            status = "ok";
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                            status = "fail";
                        }
                    }
                    else
                    {
                        string QuoteDetailsUDFRequier = string.Empty;
                        lstQuoteDetailDTO = GetLineItemsFromSession(objDTO.ID);
                        double QuoteCost = 0;
                        double QuotePrice = 0;
                        int? PriseSelectionOption = 2;

                        if (lstQuoteDetailDTO != null && lstQuoteDetailDTO.Count > 0)
                        {

                            eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                            List<UDFDTO> UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("QuoteDetails", SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                            List<Guid> lstItemGUID = lstQuoteDetailDTO.Where(d => d.ItemGUID != Guid.Empty).Select(x => x.ItemGUID).ToList();
                            string strItemGUIDs = string.Join(",", lstItemGUID.ToArray());
                            List<ItemMasterDTO> lstOfQuoteLineItem = new List<ItemMasterDTO>();
                            lstOfQuoteLineItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByGuidsNormal(strItemGUIDs, SessionHelper.RoomID, SessionHelper.CompanyID); //orderuomvalue and costuomvalue
                            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                            List<BinMasterDTO> lstOfOrderLineItemBin = new List<BinMasterDTO>();
                            string strBinNumbers = string.Join(",", lstQuoteDetailDTO.Where(x => (x.BinName ?? string.Empty) != string.Empty).Select(b => b.BinName).Distinct());
                            lstOfOrderLineItemBin = objBinMasterDAL.GetAllBinMastersByBinList(strBinNumbers, SessionHelper.RoomID, SessionHelper.CompanyID);

                            foreach (var quoteDetail in lstQuoteDetailDTO)
                            {
                                string ordDetailUDFReq = string.Empty;
                                commonDAL.CheckUDFIsRequiredLight(UDFDataFromDB, quoteDetail.UDF1, quoteDetail.UDF2, quoteDetail.UDF3, quoteDetail.UDF4, quoteDetail.UDF5, out ordDetailUDFReq, CompanyID, RoomID,EnterpriseId,SessionHelper.UserID);

                                if (!string.IsNullOrEmpty(ordDetailUDFReq)) 
                                    QuoteDetailsUDFRequier = QuoteDetailsUDFRequier + string.Format(ResQuoteMaster.RequiredFor, ordDetailUDFReq, quoteDetail.ItemNumber);

                                if (!string.IsNullOrEmpty(QuoteDetailsUDFRequier))
                                    break;

                                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                                if (lstOfQuoteLineItem != null && lstOfQuoteLineItem.Any())
                                {
                                    objItemMasterDTO = lstOfQuoteLineItem.Where(x => x.GUID == (quoteDetail.ItemGUID)).FirstOrDefault();
                                }

                                if (objItemMasterDTO != null)
                                {
                                    if (objItemMasterDTO.OrderUOMValue == null || objItemMasterDTO.OrderUOMValue <= 0)
                                        objItemMasterDTO.OrderUOMValue = 1;
                                }

                                if (objItemMasterDTO != null)
                                {
                                    BinMasterDTO objBinMasterDTO = new BinMasterDTO();
                                    objBinMasterDTO = lstOfOrderLineItemBin.Where(x => x.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty && x.ItemGUID == objItemMasterDTO.GUID && (x.BinNumber ?? string.Empty).ToLower() == (quoteDetail.BinName ?? string.Empty).ToLower()).FirstOrDefault();

                                    if (objBinMasterDTO == null || objBinMasterDTO.ID <= 0)
                                    {
                                        objBinMasterDTO = objBinMasterDAL.GetItemBinPlain(objItemMasterDTO.GUID, quoteDetail.BinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                                    }

                                    double Modulo = 0;
                                    string strMsg = ResOrderDetails.SaveErrorMsgApprovedQTY;

                                    if (objBinMasterDTO != null && objBinMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                    {
                                        objBinMasterDTO.DefaultReorderQuantity = (objBinMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0) <= 0 ? 1 : objBinMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0));
                                    }

                                    if (objItemMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                    {
                                        objItemMasterDTO.DefaultReorderQuantity = (objItemMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0) <= 0 ? 1 : objItemMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0));
                                    }

                                    if (quoteDetail != null && quoteDetail.ApprovedQuantity != null && quoteDetail.ApprovedQuantity > 0)
                                    {
                                        if (objBinMasterDTO != null && objBinMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                        {
                                            double binDefault = objBinMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0);
                                            if (objItemMasterDTO.IsAllowOrderCostuom)
                                            {
                                                Modulo = ((quoteDetail.ApprovedQuantity ?? 0) * (objItemMasterDTO.OrderUOMValue ?? 0)) % (objBinMasterDTO.DefaultReorderQuantity ?? 1);
                                                binDefault = objBinMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0) / objItemMasterDTO.OrderUOMValue.GetValueOrDefault(0);
                                            }
                                            else
                                                Modulo = (quoteDetail.ApprovedQuantity ?? 0) % (objBinMasterDTO.DefaultReorderQuantity ?? 1);

                                            if (Modulo != 0)
                                            {
                                                return Json(new { Message = string.Format(ResOrder.ApprovedQtyNotMatchedWithLocationDefaultReOrderQty, objBinMasterDTO.BinNumber, binDefault, objItemMasterDTO.ItemNumber), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                            }
                                        }
                                        else if (objItemMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                        {
                                            if (objItemMasterDTO.IsAllowOrderCostuom)
                                                Modulo = ((quoteDetail.ApprovedQuantity ?? 0) * (objItemMasterDTO.OrderUOMValue ?? 0)) % (objItemMasterDTO.DefaultReorderQuantity ?? 1);
                                            else
                                                Modulo = (quoteDetail.ApprovedQuantity ?? 0) % (objItemMasterDTO.DefaultReorderQuantity ?? 1);
                                            if (Modulo != 0)
                                            {
                                                // return Json(new { Message = "Approved Quantity is not matched with Item Default Reorder Quantity. Please update Approved Quantity in multiple of " + objItemMasterDTO.DefaultReorderQuantity + " for Item#" + objItemMasterDTO.ItemNumber, Status = "fail" }, JsonRequestBehavior.AllowGet);
                                                return Json(new { Message = strMsg, Status = "fail" }, JsonRequestBehavior.AllowGet);
                                            }
                                        }
                                    }
                                    else if (quoteDetail != null && quoteDetail.RequestedQuantity != null)
                                    {
                                        if (objBinMasterDTO != null && objBinMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                        {
                                            double binDefault = objBinMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0);
                                            if (objItemMasterDTO.IsAllowOrderCostuom)
                                            {
                                                Modulo = ((quoteDetail.RequestedQuantity ?? 0) * (objItemMasterDTO.OrderUOMValue ?? 0)) % (objBinMasterDTO.DefaultReorderQuantity ?? 1);
                                                binDefault = objBinMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0) / objItemMasterDTO.OrderUOMValue.GetValueOrDefault(0);
                                            }
                                            else
                                                Modulo = (quoteDetail.RequestedQuantity ?? 0) % (objBinMasterDTO.DefaultReorderQuantity ?? 1);

                                            if (Modulo != 0) 
                                                return Json(new { Message = string.Format(ResOrder.RequestedQtyNotMatchedWithLocationDefaultReOrderQty, objBinMasterDTO.BinNumber, binDefault, objItemMasterDTO.ItemNumber), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                        }
                                        else if (objItemMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                        {
                                            if (objItemMasterDTO.IsAllowOrderCostuom)
                                                Modulo = ((quoteDetail.RequestedQuantity ?? 0) * (objItemMasterDTO.OrderUOMValue ?? 0)) % (objItemMasterDTO.DefaultReorderQuantity ?? 1);
                                            else
                                                Modulo = (quoteDetail.RequestedQuantity ?? 0) % (objItemMasterDTO.DefaultReorderQuantity ?? 1);
                                            if (Modulo != 0) 
                                                return Json(new { Message = string.Format(ResOrder.RequestedQtyNotMatchedWithDefaultReOrderQty, objItemMasterDTO.DefaultReorderQuantity, objItemMasterDTO.ItemNumber), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                }

                                CostUOMMasterDTO costUOM = new CostUOMMasterDTO();
                                if (objItemMasterDTO == null || objItemMasterDTO.CostUOMValue.GetValueOrDefault(0) <= 0)
                                {
                                    costUOM.CostUOMValue = 1;
                                }
                                else
                                {
                                    costUOM.CostUOMValue = objItemMasterDTO.CostUOMValue;
                                }

                                #region WI-6215 and Other Relevant order cost related jira

                                if (objDTO.QuoteStatus == (int)QuoteStatus.UnSubmitted)
                                    if (objDTO.QuoteStatus == (int)QuoteStatus.UnSubmitted)
                                    {
                                        if (quoteDetail.ItemCostUOMValue == null
                                            || quoteDetail.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                                        {
                                            if (objItemMasterDTO != null)
                                            {
                                                if (objItemMasterDTO.CostUOMValue == null
                                                    || objItemMasterDTO.CostUOMValue.GetValueOrDefault(0) <= 0)
                                                    quoteDetail.ItemCostUOMValue = 1;
                                                else
                                                {
                                                    quoteDetail.ItemCostUOMValue = objItemMasterDTO.CostUOMValue.GetValueOrDefault(1);
                                                }
                                            }
                                            else
                                            {
                                                quoteDetail.ItemCostUOMValue = 1;
                                            }
                                            //objOrderDetail.ItemCostUOMValue = objItemMasterDTO.CostUOMValue.GetValueOrDefault(1);
                                        }
                                        if (quoteDetail.ItemMarkup.GetValueOrDefault(0) == 0)
                                            quoteDetail.ItemMarkup = objItemMasterDTO.Markup.GetValueOrDefault(0);
                                    }
                                if (quoteDetail.ItemCost.GetValueOrDefault(0) > 0)
                                {
                                    if (quoteDetail.ItemMarkup > 0)
                                    {
                                        quoteDetail.ItemSellPrice = quoteDetail.ItemCost + ((quoteDetail.ItemCost * quoteDetail.ItemMarkup) / 100);
                                    }
                                    else
                                    {
                                        quoteDetail.ItemSellPrice = quoteDetail.ItemCost;
                                    }
                                }
                                else
                                {
                                    quoteDetail.ItemSellPrice = 0;
                                }

                                #endregion
                                if (quoteDetail.ItemCostUOMValue == null
                                    || quoteDetail.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                                {
                                    quoteDetail.ItemCostUOMValue = 1;
                                }
                                if (objItemMasterDTO != null && quoteDetail != null)
                                {
                                    quoteDetail.QuoteLineItemExtendedCost = double.Parse(Convert.ToString((objDTO.QuoteStatus <= 2 ? (quoteDetail.RequestedQuantity.GetValueOrDefault(0) * (quoteDetail.ItemCost.GetValueOrDefault(0) / quoteDetail.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                                : (quoteDetail.ApprovedQuantity.GetValueOrDefault(0) * (quoteDetail.ItemCost.GetValueOrDefault(0) / quoteDetail.ItemCostUOMValue.GetValueOrDefault(1))))));

                                    quoteDetail.QuoteLineItemExtendedPrice = double.Parse(Convert.ToString((objDTO.QuoteStatus <= 2 ? (quoteDetail.RequestedQuantity.GetValueOrDefault(0) * (quoteDetail.ItemSellPrice.GetValueOrDefault(0) / quoteDetail.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                                : (quoteDetail.ApprovedQuantity.GetValueOrDefault(0) * (quoteDetail.ItemSellPrice.GetValueOrDefault(0) / quoteDetail.ItemCostUOMValue.GetValueOrDefault(1))))));

                                }

                                if (objItemMasterDTO != null && quoteDetail.ApprovedQuantity != null && quoteDetail.ApprovedQuantity > 0)
                                {

                                    if (PriseSelectionOption == 1)
                                    {
                                        QuotePrice += (quoteDetail.ItemSellPrice.GetValueOrDefault(0) * quoteDetail.ApprovedQuantity.GetValueOrDefault(0))
                                              / (quoteDetail.ItemCostUOMValue.GetValueOrDefault(0) > 0 ? quoteDetail.ItemCostUOMValue.GetValueOrDefault(1) : 1);
                                    }
                                    else
                                    {
                                        QuoteCost += (quoteDetail.ItemCost.GetValueOrDefault(0) * quoteDetail.ApprovedQuantity.GetValueOrDefault(0))
                                               / (quoteDetail.ItemCostUOMValue.GetValueOrDefault(0) > 0 ? quoteDetail.ItemCostUOMValue.GetValueOrDefault(1) : 1);
                                    }
                                }
                            }

                        }

                        if (!string.IsNullOrEmpty(QuoteDetailsUDFRequier))
                            return Json(new { Message = QuoteDetailsUDFRequier, Status = "fail" }, JsonRequestBehavior.AllowGet);

                        QuoteMasterDTO tempQuote = new QuoteMasterDTO();
                        tempQuote = quoteMasterDAL.GetQuoteByIdPlain(objDTO.ID);

                        if (tempQuote != null)
                        {
                            objDTO.AddedFrom = tempQuote.AddedFrom;
                            objDTO.ReceivedOnWeb = tempQuote.ReceivedOnWeb;

                            if (tempQuote.QuoteStatus == (int)QuoteStatus.UnSubmitted && objDTO.QuoteStatus == (int)QuoteStatus.Closed)
                                TempData["IsQuoteClosedFromUnSubmitted"] = true;

                        }

                        if (objDTO.QuoteStatus >= (int)QuoteStatus.Submitted && objDTO.QuoteStatus != (int)QuoteStatus.Closed && (lstQuoteDetailDTO == null || lstQuoteDetailDTO.Count <= 0))
                        {
                            message = ResQuoteMaster.QuoteMustHaveOneLineItem; 
                            status = "fail";
                        }
                        else
                        {
                            objDTO.EditedFrom = "Web";
                            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;

                            if (tempQuote != null && tempQuote.RequesterID.GetValueOrDefault(0) > 0)
                            {
                                objDTO.RequesterID = tempQuote.RequesterID;
                            }
                            if (tempQuote != null && tempQuote.ApproverID.GetValueOrDefault(0) > 0)
                            {
                                objDTO.ApproverID = tempQuote.ApproverID;
                            }
                            if (tempQuote.QuoteStatus == (int)QuoteStatus.UnSubmitted
                                && objDTO.QuoteStatus == (int)QuoteStatus.Submitted)
                            {
                                objDTO.RequesterID = SessionHelper.UserID;
                            }
                            else if (tempQuote.QuoteStatus == (int)QuoteStatus.Submitted
                                && objDTO.QuoteStatus == (int)QuoteStatus.Approved)
                            {
                                objDTO.ApproverID = SessionHelper.UserID;
                            }
                            else if (tempQuote.QuoteStatus == (int)QuoteStatus.UnSubmitted
                                && objDTO.QuoteStatus == (int)QuoteStatus.Approved)
                            {
                                objDTO.RequesterID = SessionHelper.UserID;
                                objDTO.ApproverID = SessionHelper.UserID;
                            }

                            var ReturnVal = quoteMasterDAL.UpdateQuoteMaster(objDTO);

                            if (ReturnVal != null && ReturnVal.ID > 0)
                            {
                                if (objDTO.QuoteStatus <= (int)QuoteStatus.Approved)
                                {
                                    AutoSequenceDAL objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
                                    var QuoteSuppliers = objDTO.QuoteSupplierIdsCSV.Split(',');
                                    QuoteSuppliers = QuoteSuppliers.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                                    foreach (var quoteSupp in QuoteSuppliers)
                                    {
                                        objAutoSeqDAL.UpdateNextQuoteNumber(objDTO.Room, objDTO.CompanyID, objDTO.QuoteNumber, Convert.ToInt64(quoteSupp), SessionHelper.UserID, objDTO.ReleaseNumber);
                                    }
                                   
                                }

                                // As per client Mail :Short term To Dos, committed completion dates, and specific fixes needed (point :6  Eliminate “Transmit” on Order History)
                                if (objDTO.QuoteStatus == (int)QuoteStatus.Approved && !roomDTO.DoSendQuotetoVendor)
                                {
                                    objDTO.QuoteStatus = (int)QuoteStatus.Transmitted;
                                    objDTO.EditedFrom = "Web";
                                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    UpdateQuoteToTransmited(objDTO.GUID.ToString(), "Web", "SaveQuote.ToTransmitFromApproved");
                                }
                                message = ResMessage.SaveMessage;
                                status = "ok";
                            }
                            else
                            {
                                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); //string.Format(ResMessage.SaveErrorMsg, hrmResult.StatusCode); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                                status = "fail";
                            }
                        }
                    }
                }

                Session["IsInsert"] = "True";
                return Json(new { Message = message, Status = status, ID = objDTO.ID }, JsonRequestBehavior.AllowGet);

            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    quoteMasterDAL = null;
            //    objCDAL = null;
            //}
        }

        public ActionResult QuoteEdit(long ID, string callfor = "")
        {
            QuoteMasterDAL quoteMasterDAL = null;
            QuoteMasterDTO quote = null;
            List<SelectListItem> lstQuoteStatus = null;

            try
            {
                if (!string.IsNullOrEmpty(callfor) && !string.IsNullOrWhiteSpace(callfor))
                {
                    if ((callfor ?? string.Empty).ToLower().Contains("editquoteitem"))
                    {
                        ViewBag.CallFor = "editQuoteItem";
                    }
                    else if ((callfor ?? string.Empty).ToLower().Contains("unclosequote"))
                    {
                        ViewBag.CallFor = "UnCloseQuote";
                    }
                    else
                    {
                        ViewBag.CallFor = "";
                    }
                }
                else
                {
                    ViewBag.CallFor = "";
                }

                SessionHelper.RomoveSessionByKey(GetSessionKey(ID));
                SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(ID));
                SessionHelper.RomoveSessionByKey("AddItemToQuote_" + RoomID + "_" + CompanyID);
                quoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
                quote = quoteMasterDAL.GetQuoteByIdFull(ID);
                quote.IsRecordNotEditable = IsRecordNotEditable(quote);
                quote.IsOnlyFromUI = true;
                bool isQuoteHeaderEdit = true;

                if (quote.QuoteStatus >= (int)QuoteStatus.Submitted)
                    isQuoteHeaderEdit = false;

                ViewBag.UDFs = GetUDFDataPageWise("QuoteMaster", isQuoteHeaderEdit);
                ViewData["UDF1"] = quote.UDF1;
                ViewData["UDF2"] = quote.UDF2;
                ViewData["UDF3"] = quote.UDF3;
                ViewData["UDF4"] = quote.UDF4;
                ViewData["UDF5"] = quote.UDF5;

                if (quote.IsDeleted)
                {
                    quote.IsRecordNotEditable = true;
                    quote.IsOnlyStatusUpdate = false;
                    quote.IsAbleToDelete = true;
                }

                if (quote.QuoteStatus >= (int)QuoteStatus.Approved || quote.IsRecordNotEditable)
                {
                    lstQuoteStatus = GetQuoteStatusList(quote, "");
                }
                else
                    lstQuoteStatus = GetQuoteStatusList(quote, "edit");

                if (IsChangeQuote && Convert.ToString(Request["IsChangeQuote"]) == "true")
                {
                    long NewChangeOrderRevisionNo = 0;
                    long.TryParse(Convert.ToString(Request["ChangeQuoteRevisionNo"]), out NewChangeOrderRevisionNo);

                    if (quote.ChangeQuoteRevisionNo.GetValueOrDefault(0) < NewChangeOrderRevisionNo)
                    {
                        quote.IsRecordNotEditable = false;
                        quote.ChangeQuoteRevisionNo = NewChangeOrderRevisionNo;
                        quote.IsChangeQuoteClick = true;
                        lstQuoteStatus = lstQuoteStatus.Where(x => x.Value == "1").ToList();
                    }
                }

                bool IsableToUnApprove = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.AllowanquotetobeUnapprove);

                if (IsableToUnApprove && quote.QuoteStatus == (int)QuoteStatus.Approved)
                {
                    quote.IsOnlyStatusUpdate = true;
                    lstQuoteStatus = lstQuoteStatus.Where(x => x.Value == "1" || x.Value == "3" || x.Value == "8").ToList();
                }

                ViewBag.QuoteStatusList = lstQuoteStatus;
                quote.RequiredDateStr = quote.RequiredDate.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
                return PartialView("_CreateQuote", quote);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                quoteMasterDAL = null;
                quote = null;
                lstQuoteStatus = null;
            }
        }

        /// <summary>
        /// LoadQuoteLineItems
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public ActionResult LoadQuoteLineItems(long quoteId, bool IsShowDeleted = false)
        {
            QuoteMasterDTO objDTO = null;

            if (quoteId <= 0)
                objDTO = new QuoteMasterDTO() { ID = quoteId, QuoteListItem = new List<QuoteDetailDTO>() };
            else
            {
                objDTO = new QuoteMasterDAL(SessionHelper.EnterPriseDBName).GetQuoteByIdNormal(quoteId);

                if (IsShowDeleted)
                {
                    objDTO.QuoteListItem = new QuoteDetailDAL(SessionHelper.EnterPriseDBName).GetQuoteDetailByQuoteGUIDFullWithSupFltr(objDTO.GUID, RoomID, CompanyID, true, SessionHelper.UserSupplierIds);
                }
                else
                    objDTO.QuoteListItem = GetLineItemsFromSession(quoteId);

                if (objDTO.QuoteListItem == null || objDTO.QuoteListItem.Count <= 0)
                {
                    if (IsShowDeleted)

                        objDTO.QuoteListItem = new QuoteDetailDAL(SessionHelper.EnterPriseDBName).GetQuoteDetailByQuoteGUIDFullWithSupFltr(objDTO.GUID, RoomID, CompanyID, true, SessionHelper.UserSupplierIds);
                    else
                        objDTO.QuoteListItem = new QuoteDetailDAL(SessionHelper.EnterPriseDBName).GetQuoteDetailByQuoteGUIDFullWithSupFltr(objDTO.GUID, RoomID, CompanyID, false, SessionHelper.UserSupplierIds);

                    var lineItemsFromDB = objDTO.QuoteListItem;
                    var dataToBeAddInSession = new List<QuoteDetailDTO>();
                    var quoteLIFromSession = GetLineItemsFromSession(quoteId);

                    if (lineItemsFromDB != null && lineItemsFromDB.Any() && lineItemsFromDB.Count > 0)
                    {
                        dataToBeAddInSession.AddRange(lineItemsFromDB);
                    }

                    if (quoteLIFromSession != null && quoteLIFromSession.Any() && quoteLIFromSession.Count > 0)
                    {
                        dataToBeAddInSession.AddRange(quoteLIFromSession);
                    }
                    SessionHelper.Add(GetSessionKey(quoteId), dataToBeAddInSession);
                }
                if (objDTO.QuoteListItem != null && objDTO.QuoteListItem.Count > 0)
                {
                  //  objDTO.QuoteListItem = objDTO.QuoteListItem.OrderBy(x => x.ItemNumber).ToList();
                    if (objDTO.QuoteStatus < (int)OrderStatus.Submitted)
                    {
                        objDTO.QuoteListItem.ToList().Where(x => x.hasPOItemNumber == false).ToList().ForEach(o => o.POItemLineNumber = null);
                    }
                    var LineItemsWithoutPOLineNumber = objDTO.QuoteListItem.Where(x => x.POItemLineNumber == null || x.POItemLineNumber < 0).ToList();
                    var LineItemsWithPOLineNumber = objDTO.QuoteListItem.Where(x => x.POItemLineNumber != null && x.POItemLineNumber > 0).ToList();
                    if (LineItemsWithPOLineNumber.Count > 0 && LineItemsWithoutPOLineNumber.Count > 0)
                    {
                        List<QuoteDetailDTO> UpdatedLineItems = new List<QuoteDetailDTO>();
                        int ItemLineCounter = 1, j = 0;
                        for (int i = 0; i < objDTO.QuoteListItem.Count; i++)
                        {
                            var list = LineItemsWithPOLineNumber.Where(x => x.POItemLineNumber == ItemLineCounter).ToList();
                            if (list != null && list.Count > 0)
                            {
                                UpdatedLineItems.AddRange(list);
                                i = i + list.Count - 1;
                            }
                            else
                            {
                                if (j < LineItemsWithoutPOLineNumber.Count)
                                {
                                    UpdatedLineItems.Add(LineItemsWithoutPOLineNumber[j]);
                                    j++;
                                }
                                else
                                {
                                    UpdatedLineItems.AddRange(LineItemsWithPOLineNumber.Where(x => x.POItemLineNumber > ItemLineCounter).ToList());
                                    break;
                                }

                            }
                            ItemLineCounter++;
                        }
                        objDTO.QuoteListItem = UpdatedLineItems;
                    }
                }
                objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);

                if (objDTO.IsDeleted || IsShowDeleted)
                {
                    objDTO.IsRecordNotEditable = true;
                    objDTO.IsOnlyStatusUpdate = false;
                    objDTO.IsAbleToDelete = true;
                }

                if (IsShowDeleted == false)
                {
                    if (objDTO.QuoteListItem != null && objDTO.QuoteListItem.Count > 0)
                    {
                        objDTO.NoOfLineItems = objDTO.QuoteListItem.Count;

                        if (objDTO.QuoteStatus < (int)QuoteStatus.Closed)
                        {
                            double? OrderCost = objDTO.QuoteCost;
                            objDTO.QuoteCost = objDTO.QuoteListItem.Sum(x => x.IsCloseItem.GetValueOrDefault(false) ? x.QuoteLineItemExtendedCost : double.Parse((x.ApprovedQuantity.GetValueOrDefault(0) == 0 ? x.RequestedQuantity.GetValueOrDefault(0) : x.ApprovedQuantity.GetValueOrDefault(0)).ToString()) * ((x.ItemCost.GetValueOrDefault(0)) / ((x.ItemCostUOMValue ?? 0) == 0 ? 1 : (x.ItemCostUOMValue ?? 1))));
                            double? OrderPrice = objDTO.QuotePrice;
                            objDTO.QuotePrice = objDTO.QuoteListItem.Sum(x => x.IsCloseItem.GetValueOrDefault(false) ? x.QuoteLineItemExtendedPrice : double.Parse((x.ApprovedQuantity.GetValueOrDefault(0) == 0 ? x.RequestedQuantity.GetValueOrDefault(0) : x.ApprovedQuantity.GetValueOrDefault(0)).ToString()) * ((x.ItemSellPrice.GetValueOrDefault(0)) / ((x.ItemCostUOMValue ?? 0) == 0 ? 1 : (x.ItemCostUOMValue ?? 1)))) ?? 0;

                            if (OrderCost != objDTO.QuoteCost || OrderPrice != objDTO.QuotePrice)
                                new QuoteMasterDAL(SessionHelper.EnterPriseDBName).UpdateQuoteMaster(objDTO);
                        }
                    }
                }
            }

            ItemMasterDAL ItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            bool IsCalQuoteCost = false;
            objDTO.QuoteListItem.ForEach(x =>
            {
                double TempReqQTY = 0;
                ItemMasterDTO Imdto = ItemDAL.GetItemByGuidNormal(x.ItemGUID, RoomID, CompanyID);
                if (Imdto != null)
                {
                    if (Imdto.OrderUOMValue == null || Imdto.OrderUOMValue <= 0)
                        Imdto.OrderUOMValue = 1;

                    x.QuoteUOMValue_LineItem = Imdto.OrderUOMValue;
                    x.IsAllowQuoteCostuom_LineItem = Imdto.IsAllowOrderCostuom;
                }

                x.RequiredDateStr = x.RequiredDate != null ? x.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture) : string.Empty;
                x.BinName = Convert.ToString(x.BinName) == "[|EmptyStagingBin|]" ? string.Empty : x.BinName;

                if ((x.RequestedQuantityUOM == null || x.RequestedQuantityUOM <= 0))
                {
                    if (Imdto != null && Imdto.IsAllowOrderCostuom)
                        TempReqQTY = Convert.ToInt64(x.RequestedQuantity * Imdto.OrderUOMValue);
                    else
                        TempReqQTY = Convert.ToInt64(x.RequestedQuantity);
                    IsCalQuoteCost = true;
                }

                if (x.CostUOMValue == null || x.CostUOMValue <= 0)
                {
                    x.CostUOMValue = 1;
                }

                if (objDTO.QuoteStatus < (int)QuoteStatus.Closed && !x.IsCloseItem.GetValueOrDefault(false))
                {
                    if (x.ItemCostUOMValue == null
                     || x.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                    {
                        x.ItemCostUOMValue = 1;
                    }

                    x.QuoteLineItemExtendedCost = double.Parse(Convert.ToString((objDTO.QuoteStatus <= 2 ? (x.RequestedQuantity.GetValueOrDefault(0) * (x.ItemCost.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                     : (x.ApprovedQuantity.GetValueOrDefault(0) * (x.ItemCost.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1))))));

                    x.QuoteLineItemExtendedPrice = double.Parse(Convert.ToString((objDTO.QuoteStatus <= 2 ? (x.RequestedQuantity.GetValueOrDefault(0) * (x.ItemSellPrice.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                     : (x.ApprovedQuantity.GetValueOrDefault(0) * (x.ItemSellPrice.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1))))));

                    if (x.RequestedQuantityUOM == null || x.RequestedQuantityUOM <= 0)
                    {
                        x.QuoteLineItemExtendedCost = double.Parse(Convert.ToString((objDTO.QuoteStatus <= 2 ? (TempReqQTY * (x.ItemCost.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                     : (x.ApprovedQuantity.GetValueOrDefault(0) * (x.ItemCost.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1))))));

                        x.QuoteLineItemExtendedPrice = double.Parse(Convert.ToString((objDTO.QuoteStatus <= 2 ? (TempReqQTY * (x.ItemSellPrice.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                         : (x.ApprovedQuantity.GetValueOrDefault(0) * (x.ItemSellPrice.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1))))));
                    }
                }

                if (x.RequestedQuantityUOM != null && x.RequestedQuantityUOM > 0)
                    x.RequestedQuantity = x.RequestedQuantityUOM;

                if (x.ApprovedQuantityUOM != null && x.ApprovedQuantityUOM > 0)
                    x.ApprovedQuantity = x.ApprovedQuantityUOM;

                if (x.OrderedQuantityUOM != null && x.OrderedQuantityUOM > 0)
                    x.OrderedQuantity = x.OrderedQuantityUOM;

                if (x.InTransitQuantityUOM != null && x.InTransitQuantityUOM > 0)
                    x.InTransitQuantity = x.InTransitQuantityUOM;
                else if (x.InTransitQuantityUOM != null && x.InTransitQuantityUOM < 0 && x.InTransitQuantityUOM != null)
                    x.InTransitQuantity = x.InTransitQuantityUOM;
            });

            if (IsShowDeleted == false && IsCalQuoteCost && quoteId > 0 && objDTO.QuoteStatus < (int)QuoteStatus.Closed)
            {
                objDTO.QuoteCost = objDTO.QuoteListItem.Sum(x => x.QuoteLineItemExtendedCost);
                objDTO.QuotePrice = objDTO.QuoteListItem.Sum(x => x.QuoteLineItemExtendedPrice) ?? 0;
            }

            ViewBag.IsShowDeleted = IsShowDeleted;

            return PartialView("_QuoteLineItems", objDTO);
        }

        /// <summary>
        /// LoadItemMasterModel
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public ActionResult LoadItemMasterModel(long ParentId)
        {
            QuoteMasterDTO objDTO = new QuoteMasterDAL(SessionHelper.EnterPriseDBName).GetQuoteByIdPlain(ParentId);
            string modelHeader = ResOrder.ItemModelHeader;
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Quote/AddItemsToQuote/", // Not Used
                PerentID = ParentId.ToString(),
                PerentGUID = objDTO.GUID.ToString(),
                ModelHeader = modelHeader,
                AjaxURLAddMultipleItemToSession = "~/Quote/AddItemsToQuote/",
                AjaxURLToFillItemGrid = "~/Quote/GetItemsModelMethod/",
                CallingFromPageName = "Quote",
                QuoteRequeredDate = objDTO.RequiredDate.ToString("MM/dd/yyyy"),
                QuoteStatus = objDTO.QuoteStatus
            };

            return PartialView("ItemMasterModel", obj);
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetItemsModelMethod(QuickListJQueryDataTableParamModel param)
        {
            string sortColumnName = string.Empty;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.ToLower().Contains("itemudf"))
                sortColumnName = sortColumnName.ToLower().Replace("item", "");

            if (Request["sSearch_0"] != null && !string.IsNullOrEmpty(Request["sSearch_0"]))
            {
                param.sSearch = Request["sSearch_0"].Trim(',');
            }

            int TotalRecordCount = 0;
            long QuoteID = 0;
            long.TryParse(Request["ParentID"], out QuoteID);
            QuoteMasterDTO objMasterDTO = new QuoteMasterDAL(SessionHelper.EnterPriseDBName).GetQuoteByIdPlain(QuoteID);
            string ExclueBinMasterGUIDs = "";
            string ItemsIDs = "";
            string modelPopupFor = "quote";
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetPagedItemsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierIds, true, true, true, SessionHelper.UserID, modelPopupFor, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone, true, ItemsIDs, null, ExclueBinMasterGUIDs).ToList();

            foreach (var item in DataFromDB)
            {
                List<DTOForAutoComplete> locations = GetBinsOfItemByQuoteId(string.Empty, item.GUID.ToString(), false, false, QuoteID);
                item.BinAutoComplete = new List<BinAutoComplete>();

                foreach (var binlist in locations)
                {
                    BinAutoComplete bin = new BinAutoComplete();
                    bin.ID = binlist.ID;
                    bin.BinNumber = binlist.Key;
                    item.BinAutoComplete.Add(bin);
                }

                if (item.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                {
                    item.DefaultReorderQuantity = (item.DefaultReorderQuantity.GetValueOrDefault(0) <= 0 ? 1 : item.DefaultReorderQuantity.GetValueOrDefault(0));
                    item.ItemDefaultReorderQuantity = (item.ItemDefaultReorderQuantity.GetValueOrDefault(0) <= 0 ? 1 : item.ItemDefaultReorderQuantity.GetValueOrDefault(0));
                }

                if (modelPopupFor == "quote" && item.IsAllowOrderCostuom)
                {
                    item.DefaultReorderQuantity = item.DefaultReorderQuantity / item.OrderUOMValue;
                    item.ItemDefaultReorderQuantity = item.DefaultReorderQuantity;
                }
            }

            var jsonResult = Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = DataFromDB }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public List<DTOForAutoComplete> GetBinsOfItemByQuoteId(string NameStartWith, string ItemGUID, bool QtyRequired = false, bool OnlyHaveQty = false, long QuoteId = 0)
        {
            List<string> dtoList = new List<string>();
            List<DTOForAutoComplete> locations = new List<DTOForAutoComplete>();
            ItemMasterDAL ItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

            if (QtyRequired == false)
            {
                IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(RoomID, CompanyID, false, false, ItemGUID, false, string.Empty, null, null).OrderBy(x => x.BinNumber);

                if (objBinDTOList != null && objBinDTOList.Count() > 0)
                {
                    dtoList = objBinDTOList.Select(x => x.BinNumber).ToList();

                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        objBinDTOList = objBinDTOList.Where(x => x.BinNumber.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }

                    foreach (var item in objBinDTOList)
                    {
                        ItemMasterDTO IMDTO = ItemDAL.GetItemByGuidNormal(item.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                        DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                        objAutoDTO.Key = item.BinNumber;
                        objAutoDTO.Value = item.BinNumber;
                        objAutoDTO.ID = item.ID;

                        if (item.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true && IMDTO.IsAllowOrderCostuom)
                        {
                            objAutoDTO.OtherInfo2 = Convert.ToString(item.DefaultReorderQuantity.GetValueOrDefault(0) / IMDTO.OrderUOMValue);
                        }

                        if (!string.IsNullOrEmpty(item.BinNumber) && item.BinNumber.Trim().Length > 0)
                            locations.Add(objAutoDTO);
                    }
                }
            }

            return locations;
        }

        /// <summary>
        /// Add New Item to Order
        /// </summary>
        /// <param name="objNewItems"></param>
        /// <param name="QuoteID"></param>
        /// <returns></returns>
        public JsonResult AddItemsToQuote(QuoteDetailDTO[] objNewItems, long QuoteID)
        {
            string message = ResOrder.NotInsert;
            string status = "fail";
            QuoteMasterDTO objQuoteDTO = null;
            QuoteMasterDAL objQuoteDAL = null;
            BinMasterDAL objBinDAL = null;
            ItemMasterDTO objItemDTO = null;
            ItemMasterDAL objItemDAL = null;
            QuoteDetailDTO objNewDetailDTO = null;
            List<QuoteDetailDTO> lstReturnsForSameItemWithBin = null;
            List<DTOForAutoComplete> lstAddedItemsBin = null;
            //QuoteDetailDAL objQuoteDetailDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);

            try
            {
                objQuoteDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
                List<QuoteDetailDTO> lstDetails = GetLineItemsFromSession(QuoteID);
                objQuoteDTO = objQuoteDAL.GetQuoteByIdPlain(QuoteID);
                string binName = string.Empty;
                long? binID = null;

                objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                if (objNewItems != null && objNewItems.Length > 0)
                {
                    lstReturnsForSameItemWithBin = new List<QuoteDetailDTO>();
                    lstAddedItemsBin = new List<DTOForAutoComplete>();

                    foreach (var item in objNewItems)
                    {
                        binID = item.BinID;
                        binName = item.BinName;

                        if (string.IsNullOrEmpty(binName))
                            binName = item.BinName;

                        objItemDTO = objItemDAL.GetItemWithMasterTableJoins(null, item.ItemGUID, RoomID, CompanyID);

                        if (objItemDTO.IsAllowOrderCostuom)
                        {
                            if (objItemDTO.OrderUOMValue == null || objItemDTO.OrderUOMValue <= 0)
                                objItemDTO.OrderUOMValue = 1;
                            objItemDTO.DefaultReorderQuantity = objItemDTO.DefaultReorderQuantity / objItemDTO.OrderUOMValue;
                        }
                        List<BinMasterDTO> lstItemBins = objBinDAL.GetItemLocations(objItemDTO.GUID, RoomID, CompanyID).OrderBy(x => x.BinNumber).ToList();

                        if (lstItemBins != null && lstItemBins.Where(x => x.ID == binID).Count() > 0 && String.IsNullOrEmpty(binName))
                        {
                            item.BinName = lstItemBins.Where(x => x.ID == binID).ToList()[0].BinNumber;
                            binName = item.BinName;
                        }

                        if (binID == null && string.IsNullOrEmpty(binName))
                        {
                            binID = objItemDTO.DefaultLocation;
                            binName = objItemDTO.DefaultLocationName;
                        }
                        else if (binID.GetValueOrDefault(0) <= 0 && string.IsNullOrEmpty(binName))
                        {
                            binID = objItemDTO.DefaultLocation;
                            binName = objItemDTO.DefaultLocationName;
                        }
                        else if (binID.GetValueOrDefault(0) <= 0 && !string.IsNullOrEmpty(binName))
                        {
                            binID = objBinDAL.GetOrInsertBinIDByName(item.ItemGUID, binName, SessionHelper.UserID, RoomID, CompanyID, false);
                        }

                        item.BinID = binID;
                        item.BinName = binName;

                        if (objItemDTO.CostUOMValue == null || objItemDTO.CostUOMValue <= 0)
                        {
                            objItemDTO.CostUOMValue = 1;
                        }

                        if (item.BinID.HasValue && lstItemBins != null)
                        {
                            var itemBin = lstItemBins.Where(x => x.ID == item.BinID.GetValueOrDefault(0)).FirstOrDefault();

                            if (itemBin != null && itemBin.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true && itemBin.DefaultReorderQuantity.GetValueOrDefault(0) > 0)
                            {
                                objItemDTO.DefaultReorderQuantity = (!objItemDTO.IsAllowOrderCostuom)
                                    ? itemBin.DefaultReorderQuantity.GetValueOrDefault(0)
                                    : (itemBin.DefaultReorderQuantity.GetValueOrDefault(0) / objItemDTO.OrderUOMValue.GetValueOrDefault(0));
                            }
                        }

                        objNewDetailDTO = UpdateQuoteDetailWithFullInfo(item, objItemDTO, objQuoteDTO.QuoteStatus);

                        DTOForAutoComplete objAdd = new DTOForAutoComplete()
                        {
                            ItemGuid = objItemDTO.GUID,
                            Key = objItemDTO.ItemNumber,
                            ID = 0,
                            Value = "",
                            OtherInfo2 = objItemDTO.DefaultReorderQuantity.GetValueOrDefault(0) > 0 ? objItemDTO.DefaultReorderQuantity.GetValueOrDefault(0).ToString() : ""
                        };

                        if (lstItemBins.Count() > 0)
                        {
                            BinMasterDTO binDTO = lstItemBins.Where(x => (x.IsDefault ?? false) == true).FirstOrDefault();
                            if (binDTO == null)
                            {
                                binDTO = lstItemBins.FirstOrDefault();
                            }

                            objAdd.ID = binDTO.ID;
                            objAdd.Value = binDTO.BinNumber;
                            objAdd.OtherInfo2 = binDTO.DefaultReorderQuantity.GetValueOrDefault(0) > 0 ? binDTO.DefaultReorderQuantity.GetValueOrDefault(0).ToString() : "";

                            if (!string.IsNullOrEmpty(objAdd.OtherInfo2) && Convert.ToInt64(objAdd.OtherInfo2) > 0 && binDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true && objItemDTO.IsAllowOrderCostuom)
                            {
                                objAdd.OtherInfo2 = Convert.ToString(binDTO.DefaultReorderQuantity.GetValueOrDefault(0) / objItemDTO.OrderUOMValue);
                            }
                        }
                        objNewDetailDTO.hasPOItemNumber = (objNewDetailDTO.POItemLineNumber != null && objNewDetailDTO.POItemLineNumber > 0) ? true : false;
                        lstAddedItemsBin.Add(objAdd);
                        lstDetails.Add(objNewDetailDTO);
                    }

                    SessionHelper.Add(GetSessionKey(QuoteID), lstDetails);
                    if (lstReturnsForSameItemWithBin.Count <= 0)
                    { 
                        message = (message == ResOrder.NotInsert
                            ? string.Format(ResQuoteMaster.ItemsAddedToQuote, objNewItems.Length)
                            : message + string.Format(ResQuoteMaster.ItemsAddedToQuote, objNewItems.Length));
                    }
                    else if (objNewItems.Length - (lstReturnsForSameItemWithBin.Count) > 0)
                    {
                        var msg = string.Format(ResQuoteMaster.ItemsAddedAndExistInQuote, (objNewItems.Length - lstReturnsForSameItemWithBin.Count), lstReturnsForSameItemWithBin.Count);
                        message = (message == ResOrder.NotInsert
                            ? msg
                            : message + msg);
                    }
                    else
                    {
                        message = (message == ResOrder.NotInsert
                            ? ResQuoteMaster.NotAddedItemsExistInQuote
                            : message + ResQuoteMaster.NotAddedItemsExistInQuote);
                    }


                    status = "ok";
                    return Json(new { Message = message, Status = status, AlreadyExistsItems = lstReturnsForSameItemWithBin, AddedBins = lstAddedItemsBin }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                message = "Error";
                status = "fail";
                return Json(new { Message = ex.Message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objQuoteDTO = null;
                objQuoteDAL = null;
                objBinDAL = null;
                objItemDTO = null;
                objItemDAL = null;
                objNewDetailDTO = null;
            }
        }

        /// <summary>
        /// AddUpdateDeleteItemToOrder
        /// </summary>
        /// <param name="arrDetails"></param>
        /// <param name="QuoteID"></param>
        /// <returns></returns>
        public JsonResult AddUpdateDeleteItemsToQuote(QuoteDetailDTO[] arrDetails, long QuoteID)
        {
            string message = "";
            string status = "";
            bool IsQuoteClosedFromUnSubmitted = false;
            bool isQuoteApproved = false;

            try
            {
                QuoteDetailDAL objDetailDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
                List<QuoteDetailDTO> lstQuoteDetailsForItemCostUpdate = new List<QuoteDetailDTO>();

                if (TempData["IsQuoteClosedFromUnSubmitted"] != null)
                {
                    IsQuoteClosedFromUnSubmitted = Convert.ToBoolean(TempData["IsQuoteClosedFromUnSubmitted"]);
                }

                long SessionUserId = SessionHelper.UserID;
                BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                List<QuoteDetailDTO> lstDeletedItems = GetDeletedLineItemsFromSession(0);
                string strDeletedIDs = string.Join(",", lstDeletedItems.Select(x => x.ID));

                if (!string.IsNullOrEmpty(strDeletedIDs))
                {
                    objDetailDAL.DeleteQuoteDetail(strDeletedIDs, SessionHelper.UserID, RoomID, CompanyID,SessionHelper.EnterPriceID);
                }

                bool IsStagingLoc = false;
                QuoteMasterDTO quote = new QuoteMasterDTO();
                var enterpriseId = SessionHelper.EnterPriceID;

                if (QuoteID > 0)
                {
                    quote = new QuoteMasterDAL(SessionHelper.EnterPriseDBName).GetQuoteByIdPlain(QuoteID);
                }

                if (arrDetails != null && arrDetails.Length > 0)
                {
                    BinMasterDTO objBinMasterDTO;
                    List<Guid> lstItemGUID = arrDetails.Where(d => d.ItemGUID != Guid.Empty).Select(x => x.ItemGUID).ToList();
                    string strItemGUIDs = string.Join(",", lstItemGUID.ToArray());
                    List<ItemMasterDTO> lstOfOrderLineItem = new List<ItemMasterDTO>();
                    lstOfOrderLineItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByGuidsNormal(strItemGUIDs, SessionHelper.RoomID, SessionHelper.CompanyID);
                    BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    List<BinMasterDTO> lstOfOrderLineItemBin = new List<BinMasterDTO>();
                    string strBinNumbers = string.Join(",", arrDetails.Where(x => (x.BinName ?? string.Empty) != string.Empty).Select(b => b.BinName).Distinct());
                    lstOfOrderLineItemBin = objBinMasterDAL.GetAllBinMastersByBinList(strBinNumbers, SessionHelper.RoomID, SessionHelper.CompanyID);
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                    if (QuoteID > 0 && quote != null && (quote.QuoteStatus == (int)QuoteStatus.Approved
                            || quote.QuoteStatus == (int)QuoteStatus.Transmitted))
                    {
                        isQuoteApproved = true;
                    }

                    foreach (QuoteDetailDTO item in arrDetails)
                    {
                        if (IsQuoteClosedFromUnSubmitted == true)
                            item.ApprovedQuantity = 0;

                        objBinMasterDTO = new BinMasterDTO();
                        item.Room = RoomID;
                        item.RequiredDate = item.RequiredDateStr != null ? Convert.ToDateTime(DateTime.ParseExact(item.RequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture)) : Convert.ToDateTime(item.RequiredDateStr);
                        item.CompanyID = CompanyID;
                        objBinMasterDTO = lstOfOrderLineItemBin.Where(x => (x.BinNumber ?? string.Empty).ToLower() == (item.BinName ?? string.Empty).ToLower() && x.ItemGUID == item.ItemGUID).FirstOrDefault();

                        if (objBinMasterDTO == null || objBinMasterDTO.ID <= 0)
                        {
                            objBinMasterDTO = objBinMasterDAL.GetItemBinPlain(item.ItemGUID, item.BinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, IsStagingLoc);
                        }

                        if (objBinMasterDTO != null)
                        {
                            item.BinID = objBinMasterDTO.ID;
                        }

                        item.LastUpdatedBy = SessionHelper.UserID;
                        ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                        objItemMasterDTO = lstOfOrderLineItem.Where(x => x.GUID == item.ItemGUID).FirstOrDefault();

                        if (quote != null && objItemMasterDTO != null && item != null)
                        {
                            CostUOMMasterDTO costUOM = new CostUOMMasterDTO();

                            if (objItemMasterDTO.CostUOMValue <= 0)
                            {
                                costUOM.CostUOMValue = 1;
                            }
                            else
                            {
                                costUOM.CostUOMValue = objItemMasterDTO.CostUOMValue;
                            }

                            if (costUOM.CostUOMValue == null || costUOM.CostUOMValue <= 0)
                            {
                                costUOM.CostUOMValue = 1;
                            }

                            if (objItemMasterDTO.CostUOMID != null)
                                item.ItemCostUOM = objItemMasterDTO.CostUOMID;

                            if (item.ItemCost.GetValueOrDefault(0) > 0)
                            {
                                if (item.ItemMarkup > 0)
                                {
                                    item.ItemSellPrice = item.ItemCost + ((item.ItemCost * item.ItemMarkup) / 100);
                                }
                                else
                                {
                                    item.ItemSellPrice = item.ItemCost;
                                }
                            }
                            else
                            {
                                item.ItemSellPrice = 0;
                            }

                            //item.OrderLineItemExtendedCost = double.Parse(Convert.ToString((objOrder.QuoteStatus <= 2 ? (item.RequestedQuantity.GetValueOrDefault(0) * (item.ItemCost.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1)))
                            //                                            : (item.ApprovedQuantity.GetValueOrDefault(0) * (item.ItemCost.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1))))));

                            //item.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((objOrder.QuoteStatus <= 2 ? (item.RequestedQuantity.GetValueOrDefault(0) * (item.ItemSellPrice.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1)))
                            //                                            : (item.ApprovedQuantity.GetValueOrDefault(0) * (item.ItemSellPrice.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1))))));

                            OrderUOMMasterDAL objOrderUOMDAL = new OrderUOMMasterDAL(SessionHelper.EnterPriseDBName);

                            if (item.RequestedQuantity != null && item.RequestedQuantity >= 0)
                            {
                                item.RequestedQuantityUOM = item.RequestedQuantity;
                                item.RequestedQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, item.RequestedQuantity);    //item.RequestedQuantity * costUOM.CostUOMValue;
                            }

                            if (item.ApprovedQuantity != null && item.ApprovedQuantity >= 0)
                            {
                                item.ApprovedQuantityUOM = item.ApprovedQuantity;
                                item.ApprovedQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, item.ApprovedQuantity); //item.ApprovedQuantity * costUOM.CostUOMValue;
                            }

                            if (item.OrderedQuantity != null && item.OrderedQuantity >= 0)
                            {
                                item.OrderedQuantityUOM = item.OrderedQuantity;
                                item.OrderedQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, item.OrderedQuantity); //item.ReceivedQuantity * costUOM.CostUOMValue;
                            }

                            if (item.InTransitQuantity != null && item.InTransitQuantity >= 0)
                            {
                                item.InTransitQuantityUOM = item.InTransitQuantity;
                                item.InTransitQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, item.InTransitQuantity); //item.InTransitQuantity * costUOM.CostUOMValue;
                            }

                            if (item.ItemCostUOMValue == null
                             || item.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                            {
                                item.ItemCostUOMValue = 1;
                            }

                            item.QuoteLineItemExtendedCost = double.Parse(Convert.ToString((quote.QuoteStatus <= 2 ? (item.RequestedQuantity.GetValueOrDefault(0) * (item.ItemCost.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                        : (item.ApprovedQuantity.GetValueOrDefault(0) * (item.ItemCost.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1))))));

                            item.QuoteLineItemExtendedPrice = double.Parse(Convert.ToString((quote.QuoteStatus <= 2 ? (item.RequestedQuantity.GetValueOrDefault(0) * (item.ItemSellPrice.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                        : (item.ApprovedQuantity.GetValueOrDefault(0) * (item.ItemSellPrice.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1))))));

                            //if ((quote.QuoteStatus <= (int)QuoteStatus.Transmitted || item.ID < 1) && quote.Supplier.GetValueOrDefault(0) > 0 && item.ItemGUID != Guid.Empty)
                            //{
                            //    var supplierForOrderLineItem = objItemMasterDAL.GetItemSupplierForOrder(item.ItemGUID, quote.Supplier.GetValueOrDefault(0));

                            //    if (supplierForOrderLineItem != null && supplierForOrderLineItem.SupplierID.GetValueOrDefault(0) > 0)
                            //    {
                            //        item.SupplierID = supplierForOrderLineItem.SupplierID;
                            //        item.SupplierPartNo = supplierForOrderLineItem.SupplierPartNo;
                            //    }
                            //}
                        }

                        if (item.ID > 0)
                        {
                            item.EditedFrom = "Web";
                            item.ReceivedOn = DateTimeUtility.DateTimeNow;
                            item.LastUpdated = DateTimeUtility.DateTimeNow;
                            objDetailDAL.UpdateQuoteDetail(item, SessionUserId,SessionHelper.EnterPriceID);

                            if (isQuoteApproved)
                            {
                                lstQuoteDetailsForItemCostUpdate.Add(item);
                            }
                        }
                        else
                        {
                            item.CreatedBy = SessionHelper.UserID;
                            if (item.RequiredDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue)
                                item.RequiredDate = DateTimeUtility.DateTimeNow;
                            item.AddedFrom = "Web";
                            item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            item.EditedFrom = "Web";
                            item.ReceivedOn = DateTimeUtility.DateTimeNow;

                            QuoteDetailDTO insertedOrderDetail = new QuoteDetailDTO();
                            insertedOrderDetail = objDetailDAL.Insert(item, SessionUserId,enterpriseId);
                            if (isQuoteApproved && insertedOrderDetail != null
                                && insertedOrderDetail.ID > 0)
                            {
                                lstQuoteDetailsForItemCostUpdate.Add(insertedOrderDetail);
                            }
                        }
                    }
                }

                if (isQuoteApproved && lstQuoteDetailsForItemCostUpdate != null && lstQuoteDetailsForItemCostUpdate.Count > 0)
                {
                    QuoteMasterDAL orderMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
                    DataTable dtOrdDetails = objDetailDAL.GetQuoteDetailTableFromList(lstQuoteDetailsForItemCostUpdate);
                    orderMasterDAL.Qut_UpdateItemCostBasedonQuoteDetailCost(SessionHelper.UserID, "Web-QuoteApprove", SessionHelper.RoomID, SessionHelper.CompanyID, dtOrdDetails);
                }

                if (QuoteID > 0 && quote != null && quote.QuoteStatus == (int)QuoteStatus.UnSubmitted)
                {
                    QuoteMasterDTO objQuoteDTO = new QuoteMasterDAL(SessionHelper.EnterPriseDBName).GetQuoteByIdFull(QuoteID);

                    if (objQuoteDTO != null && objQuoteDTO.QuoteStatus == (int)QuoteStatus.UnSubmitted && objQuoteDTO.NoOfLineItems != null &&
                        objQuoteDTO.NoOfLineItems.GetValueOrDefault(0) > 0)
                    {
                        List<QuoteDetailDTO> lstOrdDtlDTO = objDetailDAL.GetDeletedOrUnDeletedQuoteDetailByQuoteGUIDPlain(objQuoteDTO.GUID, objQuoteDTO.Room, objQuoteDTO.CompanyID, false);

                        if (lstOrdDtlDTO != null && lstOrdDtlDTO.Count > 0)
                        {
                            SendMailQuoteUnSubmitted(objQuoteDTO);
                        }
                    }
                }

                SessionHelper.RomoveSessionByKey(GetSessionKey(0));
                SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(0));

                message = ResCommon.RecordsSavedSuccessfully;
                status = "ok";

                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                message = "Error";
                status = "fail";
                return Json(new { Message = ex.Message, Status = status }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Send Mail After Save;
        /// </summary>
        /// <param name="QuoteID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetQuoteMail(long QuoteID)
        {
            QuoteMasterDTO QuoteMasterDTO = null;
            QuoteMasterDAL objOrdDAL = null;
            List<SupplierMasterDTO> lstSuppliers = null;
            SupplierMasterDTO objSupplierDTO = null;
            SupplierMasterDAL objSupplierDAL = null;

            try
            {
                objOrdDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
                QuoteMasterDTO = objOrdDAL.GetQuoteByIdNormal(QuoteID);

                if (QuoteMasterDTO.QuoteStatus == (int)QuoteStatus.Submitted)
                {
                    SendMailToApprovalAuthority(QuoteMasterDTO);
                }
                else if (QuoteMasterDTO.QuoteStatus == (int)QuoteStatus.Approved || QuoteMasterDTO.QuoteStatus == (int)QuoteStatus.Transmitted)
                {
                    UserMasterDAL userMasterDAL = new UserMasterDAL(SessionHelper.EnterPriseDBName);
                    eTurnsMaster.DAL.UserMasterDAL objReqRequesterUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                    UserMasterDTO ReqUser = new UserMasterDTO();
                    string QuoteRequesterEmailAddress = "";
                    string QuoteApproverEmailAddress = "";

                    if (QuoteMasterDTO.RequesterID.GetValueOrDefault(0) > 0)
                    {
                        ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(QuoteMasterDTO.RequesterID.GetValueOrDefault(0));
                        if (ReqUser == null)
                        {
                            ReqUser = userMasterDAL.GetUserByIdPlain(QuoteMasterDTO.RequesterID.GetValueOrDefault(0));
                        }
                        if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                        {
                            QuoteRequesterEmailAddress = ReqUser.Email;
                        }
                    }
                    if (QuoteMasterDTO.ApproverID.GetValueOrDefault(0) > 0)
                    {
                        ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(QuoteMasterDTO.ApproverID.GetValueOrDefault(0));
                        if (ReqUser == null)
                        {
                            ReqUser = userMasterDAL.GetUserByIdPlain(QuoteMasterDTO.ApproverID.GetValueOrDefault(0));
                        }
                        if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                        {
                            QuoteApproverEmailAddress = ReqUser.Email;
                        }
                    }

                    // objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                    lstSuppliers = new List<SupplierMasterDTO>();
                    //objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(QuoteMasterDTO.Supplier.GetValueOrDefault(0));
                    SendMailToSupplier(QuoteMasterDTO,0,"",true);//objSupplierDTO,
                    SendMailForQuoteApprovedOrReject(QuoteMasterDTO, "approved", QuoteRequesterEmailAddress, QuoteApproverEmailAddress);
                }

                return Json(new { Message = ResOrder.MailSendSuccessfully, Status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.ToString(), Status = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                QuoteMasterDTO = null;
                objOrdDAL = null;
                lstSuppliers = null;
                objSupplierDTO = null;
                objSupplierDAL = null;
            }

        }

        /// <summary>
        /// Update Quote To Transmited or unclosed quote
        /// </summary>
        /// <param name="QuoteGUID"></param>
        /// <returns></returns>
        public JsonResult UpdateQuoteToTransmited(string QuoteGUID, string editedFrom, string whateWhereAction)
        {
            QuoteMasterDAL obj = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
            QuoteMasterDTO objDTO = new QuoteMasterDTO() { GUID = Guid.Parse(QuoteGUID), Room = RoomID, CompanyID = CompanyID, EditedFrom = editedFrom, WhatWhereAction = whateWhereAction };
            var quoteId = obj.TransmitQuote(objDTO);
            return Json(new { Message = ResMessage.SaveMessage, Status = "ok", ID = quoteId }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveLatestValueInSession(QuoteDetailDTO[] arrDetails, long QuoteID)
        {
            try
            {
                List<QuoteDetailDTO> objSessionQuoteList = (List<QuoteDetailDTO>)SessionHelper.Get(GetSessionKey(QuoteID));
                List<QuoteDetailDTO> objQuoteList = arrDetails.ToList();

                if (objSessionQuoteList != null && objSessionQuoteList.Count > 0)
                {
                    foreach (var item in objQuoteList)
                    {
                        QuoteDetailDTO detailFromSession = objSessionQuoteList.FirstOrDefault(x => x.ItemGUID == item.ItemGUID);
                        if (detailFromSession != null)
                        {
                            item.ItemNumber = detailFromSession.ItemNumber;
                            item.Cost = detailFromSession.Cost;
                            item.SellPrice = detailFromSession.SellPrice;
                            item.OnHandQuantity = detailFromSession.OnHandQuantity;
                            item.StagedQuantity = detailFromSession.StagedQuantity;
                            item.ItemDescription = detailFromSession.ItemDescription;
                            item.Unit = detailFromSession.Unit;
                            item.GLAccount = detailFromSession.GLAccount;
                            item.Markup = detailFromSession.Markup;
                            item.SupplierID = detailFromSession.SupplierID;
                            item.SupplierName = detailFromSession.SupplierName;
                            item.SupplierPartNo = detailFromSession.SupplierPartNo;
                            item.OnOrderQuantity = detailFromSession.OnOrderQuantity;
                            item.OnQuotedQuantity = detailFromSession.OnQuotedQuantity;
                            item.OnOrderInTransitQuantity = detailFromSession.OnOrderInTransitQuantity;
                            item.MinimumQuantity = detailFromSession.MinimumQuantity;
                            item.MaximumQuantity = detailFromSession.MaximumQuantity;
                            item.DefaultReorderQuantity = detailFromSession.DefaultReorderQuantity;
                            item.DefaultReorderQuantity = detailFromSession.DefaultReorderQuantity;
                            item.ManufacturerNumber = detailFromSession.ManufacturerNumber;
                            item.Manufacturer = detailFromSession.Manufacturer;
                            item.ManufacturerID = detailFromSession.ManufacturerID;
                            item.SuggestedOrderQuantity = detailFromSession.SuggestedOrderQuantity;
                            item.ItemCost = detailFromSession.ItemCost;
                            item.ItemCostUOM = detailFromSession.ItemCostUOM;
                            item.ItemCostUOMValue = detailFromSession.ItemCostUOMValue;
                            item.ItemMarkup = detailFromSession.ItemMarkup;
                            item.ItemSellPrice = detailFromSession.ItemSellPrice;
                        }
                    }
                    SessionHelper.Add(GetSessionKey(QuoteID), objQuoteList);
                }
            }
            catch
            {
            }

            return Json(new { Message = ResOrder.AddedSuccessfully, Success = true }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult QuoteLineItemsDelete(QuoteDetailDTO[] objDeletedItems, long QuoteID)
        {
            string message = "";
            string status = "";
            string quoteItems = "0";
            string quotecost = "0";
            string quoteprice = "0";

            try
            {
                if (objDeletedItems != null && objDeletedItems.Length > 0)
                {
                    List<QuoteDetailDTO> lstDetailDTO = GetLineItemsFromSession(QuoteID);
                    List<QuoteDetailDTO> lstDeletedItems = GetDeletedLineItemsFromSession(QuoteID);

                    foreach (var item in objDeletedItems)
                    {
                        if (item.ID > 0)
                        {
                            lstDeletedItems.Add(item);
                        }
                        if (item.ID > 0)
                        {
                            lstDetailDTO.RemoveAll(x => x.ID == item.ID && x.ItemGUID == item.ItemGUID);
                        }
                        else if (item.tempDetailsGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            lstDetailDTO.RemoveAll(x => x.tempDetailsGUID == item.tempDetailsGUID && x.ItemGUID == item.ItemGUID);
                        }
                    }

                    SessionHelper.Add(GetSessionKeyForDeletedRecord(QuoteID), lstDeletedItems);
                    SessionHelper.Add(GetSessionKey(QuoteID), lstDetailDTO);
                    message = string.Format(ResCommon.RecordDeletedSuccessfully, objDeletedItems.Length); 
                    status = "ok";

                    if (lstDetailDTO != null && lstDetailDTO.Count > 0)
                    {
                        quoteItems = lstDetailDTO.Count.ToString();
                        double ordCost = 0;
                        double ordPrice = 0;

                        foreach (var item in lstDetailDTO)
                        {
                            int quoteItemcostUOMValue = 1;

                            if (item.ItemCostUOMValue != null && item.ItemCostUOMValue.GetValueOrDefault(0) > 0)
                            {
                                quoteItemcostUOMValue = item.ItemCostUOMValue.GetValueOrDefault(1);
                            }

                            if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            {
                                ordCost += ((double)item.ApprovedQuantity.GetValueOrDefault(0) * (double)item.Cost.GetValueOrDefault(0))
                                            / (quoteItemcostUOMValue);
                            }
                            else if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                            {
                                ordCost += ((double)item.RequestedQuantity.GetValueOrDefault(0) * (double)item.Cost.GetValueOrDefault(0))
                                            / (quoteItemcostUOMValue);
                            }

                            if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            {
                                ordPrice += ((double)item.ApprovedQuantity.GetValueOrDefault(0) * (double)item.SellPrice.GetValueOrDefault(0))
                                             / (quoteItemcostUOMValue);
                            }
                            else if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                            {
                                ordPrice += ((double)item.RequestedQuantity.GetValueOrDefault(0) * (double)item.SellPrice.GetValueOrDefault(0))
                                             / (quoteItemcostUOMValue);
                            }
                        }

                        RegionSettingDAL objRegDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                        eTurnsRegionInfo objRegionInfo = objRegDAL.GetRegionSettingsById(RoomID, CompanyID, SessionHelper.UserID);

                        if (objRegionInfo != null)
                        {
                            quotecost = ordCost.ToString("N" + objRegionInfo.CurrencyDecimalDigits.ToString(), SessionHelper.RoomCulture);
                            quoteprice = ordPrice.ToString("N" + objRegionInfo.CurrencyDecimalDigits.ToString(), SessionHelper.RoomCulture);
                        }
                        else
                        {
                            quotecost = ordCost.ToString("N" + "0", SessionHelper.RoomCulture);
                            quoteprice = ordPrice.ToString("N" + "0", SessionHelper.RoomCulture);
                        }
                    }
                }
                else
                {
                    message = ResOrder.PleaseSelectRecord;
                    status = "fail";
                }

                return Json(new { Message = message, Status = status, QuoteItems = quoteItems, QuoteCost = quotecost, QuotePrice = quoteprice }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                status = "fail";
                return Json(new { Message = ex.Message, Status = status }, JsonRequestBehavior.AllowGet);
                //throw;
            }
        }

        [HttpPost]
        public JsonResult UpdateQuoteAndLineItemComment(QuoteMasterDTO Quote)
        {
            string message = "fail";
            bool status = false;

            if (Quote != null && Quote.ID > 0)
            {
                QuoteMasterDAL quoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
                quoteMasterDAL.UpdateQuoteComment(Quote.Comment, Quote.ID, SessionHelper.UserID);

                if (Quote.ID > 0 && Quote.QuoteListItem != null)
                {
                    QuoteDetailDAL quoteDetailDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);

                    foreach (var item in Quote.QuoteListItem)
                    {
                        quoteDetailDAL.UpdateQuoteDetailCommentAndUDF(item, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    }
                }

                message = "Success";
                status = true;
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// IsQuoteRecorddDeletable
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool IsQuoteRecorddDeletable(QuoteMasterDTO objDTO)
        {
            bool IsDeletable = false;
            bool IsDelete = SessionHelper.GetModulePermission(SessionHelper.ModuleList.Quote, SessionHelper.PermissionType.Delete);

            if ((objDTO.QuoteStatus < (int)QuoteStatus.Approved || objDTO.QuoteStatus == (int)QuoteStatus.Closed) && IsDelete)
            {
                IsDeletable = true;
            }

            return IsDeletable;
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteQuoteMasterRecords(string ids)
        {
            try
            {
                long SessionUserId = SessionHelper.UserID;
                QuoteMasterDAL quoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
                quoteMasterDAL.DeleteQuoteByIds(ids, SessionHelper.UserID, RoomID, CompanyID,SessionHelper.EnterPriceID);
                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CloseQuotes(string Ids)
        {
            string message = string.Empty, status = string.Empty;
            QuoteMasterDAL quoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
            QuoteDetailDAL quoteDetailDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
            List<QuoteDetailDTO> lstQuoteDetailDTO = null;
            QuoteMasterDTO quote = null;
            int errorCount = 0;
            long SessionUserId = SessionHelper.UserID;

            foreach (var item in Ids.Split(','))
            {
                if (!string.IsNullOrEmpty(item.Trim()))
                {
                    long QuoteID = 0;

                    if (long.TryParse(item.Trim(), out QuoteID))
                    {
                        try
                        {
                            quote = quoteMasterDAL.GetQuoteByIdPlain(QuoteID);

                            if (quote.QuoteStatus != (int)QuoteStatus.Closed)
                            {
                                quote.QuoteStatus = (int)QuoteStatus.Closed;
                                quote.IsOnlyFromUI = true;
                                quote.EditedFrom = "Web";
                                quote.ReceivedOn = DateTimeUtility.DateTimeNow;
                                var qt = quoteMasterDAL.UpdateQuoteMaster(quote);
                                bool isSave = (qt != null && qt.ID > 0);

                                if (isSave)
                                {
                                    lstQuoteDetailDTO = quoteDetailDAL.GetDeletedOrUnDeletedQuoteDetailByQuoteGUIDPlain(quote.GUID, RoomID, CompanyID, false);

                                    foreach (var itemdetail in lstQuoteDetailDTO)
                                    {
                                        itemdetail.EditedFrom = "Web";
                                        itemdetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        quoteDetailDAL.UpdateQuoteDetail(itemdetail, SessionUserId,SessionHelper.EnterPriceID);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                            errorCount++;
                        }
                    }
                }
            }

            if (errorCount > 0)
            {
                status = "fail";
                message = "";
            }
            else
            {
                status = "ok";
                message = ResQuoteMaster.SelectedQuotesClosedSuccessfully;
            }

            return Json(new { Message = message, Status = status });
        }

        public JsonResult GetSupplierList(string SelectedSupplierIds, long Id)
        {
            List<KeyValDTO> lstSupplierDTO = new List<KeyValDTO>();
            string strSupplierIds = string.Empty;

            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SessionHelper.UserSupplierIds);
            }

            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            List<SupplierMasterDTO> lstSuppliers = new List<SupplierMasterDTO>();
            lstSuppliers = objSupplierMasterDAL.GetNonDeletedSupplierByIDsNormal(strSupplierIds, SessionHelper.RoomID, SessionHelper.CompanyID);
            long SelectedRoom = Id > 0 ? 0 : SessionHelper.RoomID;

            if (lstSuppliers != null && lstSuppliers.Any() && !string.IsNullOrEmpty(SelectedSupplierIds) && !string.IsNullOrWhiteSpace(SelectedSupplierIds) &&
                (SelectedSupplierIds ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Count() > 0)
            {
                long convertedInt;
                var EntListToShowReleaseNo = SelectedSupplierIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => new { didConvert = long.TryParse(s.Trim(), out convertedInt), convertedValue = convertedInt })
                        .Where(w => w.didConvert)
                        .Select(s => s.convertedValue)
                        .ToList();

                if (EntListToShowReleaseNo != null && EntListToShowReleaseNo.Any() && EntListToShowReleaseNo.Count() > 0)
                {
                    var selectedRooms = lstSuppliers.Where(t => EntListToShowReleaseNo.Contains(t.ID));
                    if (selectedRooms != null && selectedRooms.Any() && selectedRooms.Count() > 0)
                    {
                        lstSupplierDTO = (from c in selectedRooms
                                      select new KeyValDTO
                                      {
                                          key = c.ID.ToString(),
                                          value = c.SupplierName
                                      }).OrderBy(x => x.value).ToList();
                    }

                    var nonSelectedRooms = lstSuppliers.Where(t => !EntListToShowReleaseNo.Contains(t.ID));

                    if (nonSelectedRooms != null && nonSelectedRooms.Any() && nonSelectedRooms.Count() > 0)
                    {
                        lstSupplierDTO.AddRange((from c in nonSelectedRooms
                                             select new KeyValDTO
                                             {
                                                 key = c.ID.ToString(),
                                                 value = c.SupplierName
                                             }).OrderBy(x => x.value).ToList());
                    }
                }
                else
                {
                    lstSupplierDTO = (from c in lstSuppliers
                                  select new KeyValDTO
                                  {
                                      key = c.ID.ToString(),
                                      value = c.SupplierName
                                  }).OrderBy(x => x.value).ToList();
                }
            }
            else
            {
                    lstSupplierDTO = (from c in lstSuppliers
                                  select new KeyValDTO
                                  {
                                      key = c.ID.ToString(),
                                      value = c.SupplierName
                                  }).OrderBy(x => x.value).ToList();
                                

            }

            return Json(new { Status = true, SupplierList = lstSupplierDTO}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is used to close quote line items
        /// </summary>
        /// <param name="Ids"></param>
        /// <param name="CallFrom"></param>
        /// <returns></returns>
        public JsonResult CloseQuoteDetailLineItems(string Ids, string CallFrom)
        {
            var quoteDetailDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
            QuoteDetailDTO quoteDetail = null;
            QuoteMasterDTO quote = null;
            var quoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
            int quoteStatus = 0;

            try
            {
                
                long SessionUserId = SessionHelper.UserID;
                quoteDetailDAL.CloseQuoteDetailItem(Ids, SessionHelper.UserID, RoomID, CompanyID,SessionHelper.EnterPriceID);

                if (!string.IsNullOrEmpty(CallFrom) && CallFrom.ToLower() == "quote")
                {
                    string[] quoteDetailIDs = Ids.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    if (quoteDetailIDs != null && quoteDetailIDs.Length > 0)
                    {
                        long quoteDetailId = 0;
                        long.TryParse(quoteDetailIDs[0],out quoteDetailId);
                        quoteDetail = quoteDetailDAL.GetQuoteDetailbyIDPlain(quoteDetailId);
                        
                        if(quoteDetail != null && quoteDetail.ID > 0)
                        {
                            quote = quoteMasterDAL.GetQuoteByGuidPlain(quoteDetail.QuoteGUID);
                            
                            if(quote != null && quote.ID > 0)
                            {
                                quoteStatus = quote.QuoteStatus;
                            }
                            
                            if (quoteStatus != (int)QuoteStatus.Closed)
                            {
                                var quoteLIFromSession = GetLineItemsFromSession(quote.ID);
                                
                                if (quoteLIFromSession != null && quoteLIFromSession.Any() && quoteLIFromSession.Count > 0)
                                {
                                    long[] idList = Array.ConvertAll(quoteDetailIDs, long.Parse);
                                    var closedLineItems = quoteLIFromSession.Where(t => idList.Contains(t.ID));
                                    closedLineItems.Select(c => { c.IsCloseItem = true; return c; }).ToList();
                                    SessionHelper.Add(GetSessionKey(quote.ID), quoteLIFromSession);
                                }                               

                            }
                        }                        
                    }
                }

                return Json(new { Message = ResOrder.LineItemClosed, Status = "ok", QuoteStatus = quoteStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                quoteDetailDAL = null;
                quoteMasterDAL=null;
                quoteDetail = null;
                quote = null;
            }
        }

        #region For Edit Required Date and comment for Close Quote

        [HttpPost]
        public JsonResult UpdateReqDateandCommentToQuoteLineItems(QuoteDetailDTO[] arrDetails, long QuoteID, bool isCommentUpdate, bool isReqDateUpdate)
        {
            try
            {
                List<QuoteDetailDTO> quoteDetails = arrDetails.ToList();

                foreach (var item in quoteDetails)
                {
                    QuoteDetailDTO objOrderDetailsDTO = new QuoteDetailDTO();
                    objOrderDetailsDTO.ID = item.ID;
                    objOrderDetailsDTO.ItemGUID = item.ItemGUID;
                    objOrderDetailsDTO.QuoteGUID = item.QuoteGUID;
                    objOrderDetailsDTO.Comment = item.Comment;
                    
                    if (!string.IsNullOrEmpty(item.RequiredDateStr))
                    {
                        objOrderDetailsDTO.RequiredDate = !string.IsNullOrEmpty(item.RequiredDateStr) ? Convert.ToDateTime(DateTime.ParseExact(item.RequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture)) : Convert.ToDateTime(item.RequiredDateStr);
                    }
                    
                    var quoteDetailDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
                    quoteDetailDAL.UpdateReqDateandCommentToQuoteLineItems(objOrderDetailsDTO, SessionHelper.UserID, isCommentUpdate, isReqDateUpdate);

                    var objSessionOrderList = (List<QuoteDetailDTO>)SessionHelper.Get(GetSessionKey(QuoteID));

                    if (objSessionOrderList != null && objSessionOrderList.Count > 0)
                    {
                        var detailFromSession = objSessionOrderList.FirstOrDefault(x => x.ItemGUID == item.ItemGUID && x.ID == item.ID);
                        
                        if (detailFromSession != null)
                        {
                            if (isCommentUpdate)
                            {
                                detailFromSession.Comment = item.Comment;
                            }
                            if (isReqDateUpdate)
                            {
                                detailFromSession.RequiredDateStr = item.RequiredDateStr;
                                detailFromSession.RequiredDate = item.RequiredDateStr != null
                                    ? Convert.ToDateTime(DateTime.ParseExact(item.RequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture))
                                    : Convert.ToDateTime(item.RequiredDateStr);
                            }
                        }
                    }
                }
                return Json(new { Status = "ok", Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Status = "No", Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
            }
        }

        #endregion

        #region Private methods

        private string GetSessionKey(long QuoteID = 0)
        {
            string strKey = "QuoteLineItem_" + SessionHelper.EnterPriceID + "_" + CompanyID + "_" + RoomID;
            return strKey;
        }

        private string GetSessionKeyForDeletedRecord(long QuoteID = 0)
        {
            string strKey = "DeletedQuoteLineItem_" + SessionHelper.EnterPriceID + "_" + CompanyID + "_" + RoomID;
            return strKey;
        }

        /// <summary>
        /// GetQuoteStatusList
        /// </summary>
        /// <param name="objDTO"></param>
        /// <param name="Mode"></param>
        /// <returns></returns>
        private List<SelectListItem> GetQuoteStatusList(QuoteMasterDTO objDTO, string Mode)
        {
            int CurrentStatus = objDTO.QuoteStatus;
            List<SelectListItem> returnList = new List<SelectListItem>();

            if (Mode.ToLower() == "create")
            {
                returnList.Add(new SelectListItem() { Text = ResQuoteMaster.GetQuoteStatusText(QuoteStatus.UnSubmitted.ToString()), Value = ((int)QuoteStatus.UnSubmitted).ToString() });
            }
            else if (Mode.ToLower() == "edit")
            {
                foreach (var item in Enum.GetValues(typeof(QuoteStatus)))
                {
                    string itemText = item.ToString();
                    int itemValue = (int)(Enum.Parse(typeof(QuoteStatus), itemText));
                    if (itemValue < (int)QuoteStatus.Transmitted && itemValue >= CurrentStatus)
                    {
                        if (returnList.FindIndex(x => int.Parse(x.Value) == itemValue) < 0)
                            returnList.Add(new SelectListItem() { Text = ResQuoteMaster.GetQuoteStatusText(item.ToString()), Value = itemValue.ToString() });
                    }
                    else if (CurrentStatus == (int)QuoteStatus.Transmitted && IsChangeQuote)
                    {
                        if (returnList.FindIndex(x => int.Parse(x.Value) == (int)QuoteStatus.UnSubmitted) < 0)
                        {
                            returnList.Add(new SelectListItem() { Text = ResQuoteMaster.GetQuoteStatusText(QuoteStatus.UnSubmitted.ToString()), Value = ((int)QuoteStatus.UnSubmitted).ToString() });
                            string strQuoteNumber = objDTO.QuoteNumber;
                            string[] ordRevNumber = strQuoteNumber.Split(new char[1] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                            if (ordRevNumber.Length == 1)
                            {
                                strQuoteNumber = strQuoteNumber + "_R1";

                            }
                            else if (ordRevNumber.Length > 1)
                            {
                                int RevNo = int.Parse(ordRevNumber[ordRevNumber.Length - 1].Replace("R", ""));
                                if (ordRevNumber.Length > 2)
                                {
                                    for (int i = 0; i < ordRevNumber.Length - 2; i++)
                                    {
                                        if (i > 0)
                                            strQuoteNumber += "_";
                                        strQuoteNumber += ordRevNumber[i];
                                    }
                                    strQuoteNumber += "_R" + (RevNo + 1);
                                }
                                else
                                {
                                    strQuoteNumber = ordRevNumber[0] + "_R" + (RevNo + 1);
                                }
                            }
                            objDTO.QuoteNumber = strQuoteNumber;
                            break;
                        }
                    }
                }

                if (objDTO.QuoteStatus != (int)QuoteStatus.Approved && returnList.FindIndex(x => x.Value == ((int)QuoteStatus.Approved).ToString()) >= 0 && !IsApprove)
                {
                    returnList.RemoveAt(returnList.FindIndex(x => x.Value == ((int)QuoteStatus.Approved).ToString()));
                }

                if (objDTO.QuoteStatus != (int)QuoteStatus.Submitted && returnList.FindIndex(x => x.Value == ((int)QuoteStatus.Submitted).ToString()) >= 0 && !IsSubmit)
                {
                    returnList.RemoveAt(returnList.FindIndex(x => x.Value == ((int)QuoteStatus.Submitted).ToString()));
                }

                if (returnList.FindIndex(x => x.Value == ((int)QuoteStatus.Approved).ToString()) >= 0)
                {
                    returnList.Add(new SelectListItem() { Text = ResQuoteMaster.GetQuoteStatusText(QuoteStatus.Closed.ToString()), Value = ((int)QuoteStatus.Closed).ToString() });
                }
            }
            else
            {
                foreach (var item in Enum.GetValues(typeof(QuoteStatus)))
                {
                    string itemText = item.ToString();
                    int itemValue = (int)(Enum.Parse(typeof(QuoteStatus), itemText));
                    returnList.Add(new SelectListItem() { Text = ResQuoteMaster.GetQuoteStatusText(item.ToString()), Value = itemValue.ToString() });
                }
            }

            if (Mode.ToLower() != "create" && !(returnList.FindIndex(x => x.Value == ((int)QuoteStatus.Closed).ToString()) >= 0))
            {
                returnList.Add(new SelectListItem() { Text = ResQuoteMaster.GetQuoteStatusText(QuoteStatus.Closed.ToString()), Value = ((int)QuoteStatus.Closed).ToString() });
            }

            return returnList;
        }

        /// <summary>
        /// GetUDFDataPageWise
        /// </summary>
        /// <param name="PageName"></param>
        /// <returns></returns>
        private object GetUDFDataPageWise(string PageName, bool IsQuoteHeaderCanEdit = true)
        {
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetUDFsByUDFTableNamePlain(PageName, RoomID, CompanyID);

            var result = from c in DataFromDB
                         select new UDFDTO
                         {
                             ID = c.ID,
                             CompanyID = c.CompanyID,
                             Room = c.Room,
                             UDFTableName = c.UDFTableName,
                             UDFColumnName = c.UDFColumnName,
                             UDFDefaultValue = c.UDFDefaultValue,
                             UDFOptionsCSV = c.UDFOptionsCSV,
                             UDFControlType = c.UDFControlType,
                             UDFIsRequired = (IsQuoteHeaderCanEdit ? c.UDFIsRequired : false),
                             UDFIsSearchable = c.UDFIsRequired = c.UDFIsRequired,
                             Created = c.Created,
                             Updated = c.Updated,
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             IsDeleted = c.IsDeleted,
                             UDFMaxLength = c.UDFMaxLength
                         };

            return result;
        }

        /// <summary>
        /// IsRecordNotEditable
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool IsRecordNotEditable(QuoteMasterDTO objDTO)
        {
            bool isNotEditable = false;
            bool IsInsert = SessionHelper.GetModulePermission(SessionHelper.ModuleList.Quote, SessionHelper.PermissionType.Insert);
            bool IsUpdate = SessionHelper.GetModulePermission(SessionHelper.ModuleList.Quote, SessionHelper.PermissionType.Update);
            bool IsDelete = SessionHelper.GetModulePermission(SessionHelper.ModuleList.Quote, SessionHelper.PermissionType.Delete);

            if (!(IsInsert || IsUpdate || IsDelete || IsSubmit || IsApprove || IsChangeQuote))
            {
                isNotEditable = true;
                return isNotEditable;
            }

            if (objDTO.ID <= 0 && !IsInsert)
            {
                isNotEditable = true;
                return isNotEditable;
            }

            if (IsChangeQuote && objDTO.QuoteStatus == (int)QuoteStatus.Transmitted)
            {
                isNotEditable = false;
            }
            else if (IsUpdate || IsSubmit || IsApprove || Convert.ToString(Session["IsInsert"]) == "True" || IsInsert)
            {
                if (IsApprove || IsSubmit)
                    objDTO.IsOnlyStatusUpdate = true;
                else
                    objDTO.IsOnlyStatusUpdate = false;


                if (objDTO.QuoteStatus < (int)QuoteStatus.Submitted)
                {
                    if (Convert.ToString(Session["IsInsert"]) == "")
                    {
                        if (objDTO.ID > 0 && !IsUpdate)// Edit mode with View only 
                        {
                            isNotEditable = true;
                        }
                    }
                    else if (!IsUpdate && Convert.ToString(Session["IsInsert"]) != "True")
                        isNotEditable = true;
                }
                else if (objDTO.QuoteStatus >= (int)QuoteStatus.Submitted)
                {
                    if (objDTO.QuoteStatus == (int)QuoteStatus.Submitted)
                    {
                        if (IsSubmit && !IsApprove)
                            isNotEditable = false;
                        else if (!IsApprove)
                            isNotEditable = true;
                        else if (IsApprove && !IsUpdate)
                        {
                            objDTO.IsOnlyStatusUpdate = true;
                            isNotEditable = true;
                        }
                    }
                    else if (objDTO.QuoteStatus > (int)QuoteStatus.Submitted)
                    {
                        isNotEditable = true;
                        if (objDTO.QuoteStatus == (int)QuoteStatus.Transmitted && IsChangeQuote)
                            isNotEditable = false;
                        objDTO.IsOnlyStatusUpdate = false;
                    }
                }
            }

            return isNotEditable;
        }

        private List<QuoteDetailDTO> GetDeletedLineItemsFromSession(long QuoteID)
        {
            List<QuoteDetailDTO> lstDetailDTO = new List<QuoteDetailDTO>();
            List<QuoteDetailDTO> lstDetails = (List<QuoteDetailDTO>)SessionHelper.Get(GetSessionKeyForDeletedRecord(QuoteID));

            if (lstDetails != null && lstDetails.Count > 0)
            {
                lstDetailDTO = lstDetails;
            }

            return lstDetailDTO;
        }

        private List<QuoteDetailDTO> GetLineItemsFromSession(long QuoteID)
        {
            List<QuoteDetailDTO> lstDetailDTO = new List<QuoteDetailDTO>();
            List<QuoteDetailDTO> lstDetails = (List<QuoteDetailDTO>)SessionHelper.Get(GetSessionKey(QuoteID));

            if (lstDetails != null && lstDetails.Count > 0)
            {
                lstDetailDTO = lstDetails;
            }

            return lstDetailDTO;
        }

        private QuoteDetailDTO UpdateQuoteDetailWithFullInfo(QuoteDetailDTO item, ItemMasterDTO objItemDTO, int intQuoteStatus)
        {
            Guid? temp_DetailGUID = null;

            if (item.tempDetailsGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
            {
                temp_DetailGUID = Guid.NewGuid();
            }
            else
            {
                temp_DetailGUID = item.tempDetailsGUID;
            }

            QuoteDetailDTO objNewDetailDTO = new QuoteDetailDTO()
            {
                ID = 0,
                RequestedQuantity = item.RequestedQuantity,
                ApprovedQuantity = item.ApprovedQuantity.GetValueOrDefault(0),
                QuoteGUID = item.QuoteGUID,
                ItemGUID = item.ItemGUID,
                ItemNumber = objItemDTO.ItemNumber,
                BinID = item.BinID,
                CostUOMValue = objItemDTO.CostUOMValue,
                BinName = item.BinName,
                RequiredDate = item.RequiredDate,
                OrderedQuantity = item.OrderedQuantity,
                Room = RoomID,
                CompanyID = CompanyID,
                Consignment = objItemDTO.Consignment,
                Cost = objItemDTO.Cost,
                Created = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                IsDeleted = false,
                IsEDIRequired = true,
                IsEDISent = false,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                AddedFrom = "Web",
                EditedFrom = "Web",
                ASNNumber = string.Empty,
                LastEDIDate = null,
                CreatedByName = SessionHelper.UserName,
                CostUOM = objItemDTO.CostUOMName,
                CategoryID = objItemDTO.CategoryID,
                Category = objItemDTO.CategoryName,
                CriticalQuantity = objItemDTO.CriticalQuantity.GetValueOrDefault(0),
                AverageCost = objItemDTO.AverageCost,
                AverageUsage = objItemDTO.AverageUsage,
                //Action = string.Empty,
                DateCodeTracking = objItemDTO.DateCodeTracking,
                DefaultLocation = objItemDTO.DefaultLocation,
                DefaultLocationName = objItemDTO.DefaultLocationName,
                DefaultPullQuantity = objItemDTO.DefaultPullQuantity.GetValueOrDefault(0),
                DefaultReorderQuantity = objItemDTO.DefaultReorderQuantity.GetValueOrDefault(0),
                ExtendedCost = objItemDTO.ExtendedCost,
                GLAccount = objItemDTO.GLAccount,
                GLAccountID = objItemDTO.GLAccountID,
                GUID = Guid.Empty,
                //HistoryID = 0,
                ImagePath = objItemDTO.ImagePath,
                InTransitQuantity = 0,
                InventoryClassification = objItemDTO.InventoryClassification,
                IsBuildBreak = objItemDTO.IsBuildBreak,
                IsEnforceDefaultReorderQuantity = objItemDTO.IsEnforceDefaultReorderQuantity,
                //IsHistory = false,
                IsItemLevelMinMaxQtyRequired = objItemDTO.IsItemLevelMinMaxQtyRequired,
                IsLotSerialExpiryCost = objItemDTO.IsLotSerialExpiryCost,
                IsPurchase = objItemDTO.IsPurchase,
                IsTransfer = objItemDTO.IsTransfer,
                ItemCreated = objItemDTO.Created,
                ItemCreatedByName = objItemDTO.CreatedByName,
                ItemDescription = objItemDTO.Description,
                ItemID = objItemDTO.ID,
                ItemInTransitQuantity = objItemDTO.InTransitquantity,
                ItemIsArchived = objItemDTO.IsArchived,
                ItemIsDeleted = objItemDTO.IsDeleted,
                ItemLastUpdatedBy = objItemDTO.LastUpdatedBy,
                ItemRoom = objItemDTO.Room,
                ItemRoomName = objItemDTO.RoomName,
                ItemType = objItemDTO.ItemType,
                ItemUDF1 = objItemDTO.UDF1,
                ItemUDF2 = objItemDTO.UDF2,
                ItemUDF3 = objItemDTO.UDF3,
                ItemUDF4 = objItemDTO.UDF4,
                ItemUDF5 = objItemDTO.UDF5,
                ItemUDF6 = objItemDTO.UDF6,
                ItemUDF7 = objItemDTO.UDF7,
                ItemUDF8 = objItemDTO.UDF8,
                ItemUDF9 = objItemDTO.UDF9,
                ItemUDF10 = objItemDTO.UDF10,
                ItemUniqueNumber = objItemDTO.ItemUniqueNumber,
                ItemUpdated = objItemDTO.Updated,
                ItemUpdatedByName = objItemDTO.UpdatedByName,
                ItemViewGUID = objItemDTO.GUID,
                LastUpdated = DateTimeUtility.DateTimeNow,
                LastUpdatedBy = SessionHelper.UserID,
                LeadTimeInDays = objItemDTO.LeadTimeInDays,
                Link1 = objItemDTO.Link1,
                Link2 = objItemDTO.Link2,
                LongDescription = objItemDTO.LongDescription,
                LotNumberTracking = objItemDTO.LotNumberTracking,
                Manufacturer = objItemDTO.ManufacturerName,
                ManufacturerID = objItemDTO.ManufacturerID,
                ManufacturerNumber = objItemDTO.ManufacturerNumber,
                Markup = objItemDTO.Markup,
                MaximumQuantity = objItemDTO.MaximumQuantity.GetValueOrDefault(0),
                MinimumQuantity = objItemDTO.MinimumQuantity.GetValueOrDefault(0),
                OnHandQuantity = objItemDTO.OnHandQuantity,
                OnOrderQuantity = objItemDTO.OnOrderQuantity,
                OnTransferQuantity = objItemDTO.OnTransferQuantity,
                OnQuotedQuantity = objItemDTO.OnQuotedQuantity,
                PackingQuantity = objItemDTO.PackingQuantity,
                PricePerTerm = objItemDTO.PricePerTerm,
                RequisitionedQuantity = objItemDTO.RequisitionedQuantity,
                //RoomName = SessionHelper.RoomName,
                SellPrice = objItemDTO.SellPrice,
                SerialNumberTracking = objItemDTO.SerialNumberTracking,
                StagedQuantity = objItemDTO.StagedQuantity,
                SuggestedOrderQuantity = objItemDTO.SuggestedOrderQuantity,
                SupplierID = item.SupplierID.GetValueOrDefault(0) > 0 ? item.SupplierID.GetValueOrDefault(0) : objItemDTO.SupplierID,
                SupplierName = !string.IsNullOrEmpty(item.SupplierName) && !string.IsNullOrWhiteSpace(item.SupplierName) ? item.SupplierName : objItemDTO.SupplierName,
                SupplierPartNo = !string.IsNullOrEmpty(item.SupplierPartNo) && !string.IsNullOrWhiteSpace(item.SupplierPartNo) ? item.SupplierPartNo : objItemDTO.SupplierPartNo,
                Taxable = objItemDTO.Taxable,
                TotalRecords = 0,
                Trend = objItemDTO.Trend,
                Turns = objItemDTO.Turns,
                Unit = objItemDTO.Unit,
                UNSPSC = objItemDTO.UNSPSC,
                UOMID = objItemDTO.UOMID,
                UPC = objItemDTO.UPC,
                UpdatedByName = SessionHelper.UserName,
                WeightPerPiece = objItemDTO.WeightPerPiece,
                //ItemBlanketPO = objSupplierBlkPO != null ? objSupplierBlkPO.BlanketPO : "",
                UDF1 = item.UDF1,
                UDF2 = item.UDF2,
                UDF3 = item.UDF3,
                UDF4 = item.UDF4,
                UDF5 = item.UDF5,
                Comment = item.Comment,
                tempDetailsGUID = temp_DetailGUID,
                OnOrderInTransitQuantity = objItemDTO.OnOrderInTransitQuantity,
                QuoteLineItemExtendedCost = double.Parse(Convert.ToString((intQuoteStatus <= 2 ? (item.RequestedQuantity.GetValueOrDefault(0) * (item.ItemCost.GetValueOrDefault(0) / objItemDTO.CostUOMValue.GetValueOrDefault(1)))
                                                                 : (item.ApprovedQuantity.GetValueOrDefault(0) * (item.ItemCost.GetValueOrDefault(0) / objItemDTO.CostUOMValue.GetValueOrDefault(1)))))),

                QuoteLineItemExtendedPrice = double.Parse(Convert.ToString((intQuoteStatus <= 2 ? (item.RequestedQuantity.GetValueOrDefault(0) * ((item.ItemCost.GetValueOrDefault(0) + ((item.ItemCost.GetValueOrDefault(0) * objItemDTO.Markup.GetValueOrDefault(0)) / 100)) / objItemDTO.CostUOMValue.GetValueOrDefault(1)))
                                                                 : (item.ApprovedQuantity.GetValueOrDefault(0) * ((item.ItemCost.GetValueOrDefault(0) + ((item.ItemCost.GetValueOrDefault(0) * objItemDTO.Markup.GetValueOrDefault(0)) / 100)) / objItemDTO.CostUOMValue.GetValueOrDefault(1)))))),
                ItemCost = item.ItemCost,
                ItemCostUOM = objItemDTO.CostUOMID,
                ItemCostUOMValue = objItemDTO.CostUOMValue.GetValueOrDefault(1),
                ItemSellPrice = item.ItemCost.GetValueOrDefault(0) + ((item.ItemCost.GetValueOrDefault(0) * objItemDTO.Markup.GetValueOrDefault(0)) / 100),
                ItemMarkup = objItemDTO.Markup.GetValueOrDefault(0),
                IsItemActive = objItemDTO.IsActive,
                IsItemOrderable = objItemDTO.IsOrderable,
                POItemLineNumber = objItemDTO.POItemLineNumber
            };

            return objNewDetailDTO;
        }

        #endregion

        #region Quote to Order

        public ActionResult QuoteToOrders()
        {
            return View();
        }
        public JsonResult GetNarrowQuotetoOrderDDData(string TextFieldName, bool IsArchived, bool IsDeleted, string QuoteMasterGuid)
        {
            Guid QuoteGuid = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(QuoteMasterGuid))
            {
                Guid.TryParse(QuoteMasterGuid, out QuoteGuid);
            }
            Dictionary<string, int> retData = new Dictionary<string, int>();

            int LoadDataCount = 0;
            if (LoadDataCount == 0)
            {
                LoadDataCount = SiteSettingHelper.LoadNarrowDataCount != string.Empty ? Convert.ToInt32(SiteSettingHelper.LoadNarrowDataCount) : 0;
            }

            List<CommonDTO> ReuiredDateList = null;
            List<CommonDTO> lstUDF1 = null;
            List<CommonDTO> lstUDF2 = null;
            List<CommonDTO> lstUDF3 = null;
            List<CommonDTO> lstUDF4 = null;
            List<CommonDTO> lstUDF5 = null;

            QuoteDetailDAL objQuoteDetailDALDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<CommonDTO> lstCommonDTO = objQuoteDetailDALDAL.GetQuoteDetailForNarrowSearch(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, TextFieldName, SessionHelper.UserSupplierIds, LoadDataCount, QuoteGuid);
            NarrowSearchData objNarrowSearch = new NarrowSearchData();

            objNarrowSearch.ItemLocationList = lstCommonDTO.Where(t => t.PageName == "LocationByCount").ToList();
            objNarrowSearch.SupplierByList = lstCommonDTO.Where(t => t.PageName == "SupplierByCount").ToList();
            objNarrowSearch.CategoryByList = lstCommonDTO.Where(t => t.PageName == "CategoryByCount").ToList();
            objNarrowSearch.ManufactureByList = lstCommonDTO.Where(t => t.PageName == "ManufacturerByCount").ToList();
            objNarrowSearch.CreatedByList = lstCommonDTO.Where(t => t.PageName == "CreatedByCount").ToList();
            objNarrowSearch.UpdatedByList = lstCommonDTO.Where(t => t.PageName == "LastUpdateByCount").ToList();
            lstUDF1 = lstCommonDTO.Where(t => t.PageName == "UDF1ByCount").ToList();
            lstUDF2 = lstCommonDTO.Where(t => t.PageName == "UDF2ByCount").ToList();
            lstUDF3 = lstCommonDTO.Where(t => t.PageName == "UDF3ByCount").ToList();
            lstUDF4 = lstCommonDTO.Where(t => t.PageName == "UDF4ByCount").ToList();
            lstUDF5 = lstCommonDTO.Where(t => t.PageName == "UDF5ByCount").ToList();
            if (lstUDF1.Count > 0)
                objNarrowSearch.UDF1List = lstUDF1;
            if (lstUDF2.Count > 0)
                objNarrowSearch.UDF2List = lstUDF2;
            if (lstUDF3.Count > 0)
                objNarrowSearch.UDF3List = lstUDF3;
            if (lstUDF4.Count > 0)
                objNarrowSearch.UDF4List = lstUDF4;
            if (lstUDF5.Count > 0)
                objNarrowSearch.UDF5List = lstUDF5;

            return Json(new { Success = true, Message = ResCommon.MsgDataSuccessfullyGet, Data = objNarrowSearch }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QuoteLineItemsListAjax(JQueryDataTableParamModel param)
        {
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            string QuoteGUID = Convert.ToString(Request["QuoteGUID"].ToString());

            Guid QuoteMasterGuid = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(QuoteGUID))
            {
                Guid.TryParse(QuoteGUID, out QuoteMasterGuid);
            }

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID desc";

                //if (sortDirection == "asc")
                //    sortColumnName = sortColumnName + " asc";
                //else
                //    sortColumnName = sortColumnName + " desc";
            }
            else
                sortColumnName = "ID desc";
            //if (sortColumnName.ToLower().Contains("quotestatustext"))
            //    sortColumnName = sortColumnName.Replace("QuoteStatusText", "QuoteStatusName");

            if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.ToLower().Contains("quotenumber"))
                sortColumnName = sortColumnName.Replace("QuoteNumber", "QuoteNumber_ForSorting");

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            QuoteDetailDAL objQuoteDetailDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<long> SupplierIds = null;
            if (SessionHelper.UserSupplierIds != null
                && SessionHelper.UserSupplierIds.Count > 0)
            {
                SupplierIds = SessionHelper.UserSupplierIds;
            }
            List<QuoteDetailDTO> DataFromDB = objQuoteDetailDAL.GetPagedQuoteDetailByQuoteGUID(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, QuoteMasterGuid, SupplierIds, RoomDateFormat, CurrentTimeZone);
            DataFromDB.ForEach(x =>
            {
                x.RequiredDateStr = (x.RequiredDate != null ? x.RequiredDate.GetValueOrDefault(DateTime.Now).ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture)
                                    : string.Empty);
            });
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApprovedQuotes()
        {
            List<QuoteMasterDTO> lstQuoteMasterDTO = new List<QuoteMasterDTO>();
            QuoteMasterDAL objQuoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);

            lstQuoteMasterDTO = objQuoteMasterDAL.GetApprovedQuotesList(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds).ToList();
            return Json(lstQuoteMasterDTO, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateQuoteOrderPopup(Guid QuoteGuid, string Ids, string QuoteLineItemUDF1, string QuoteLineItemUDF2, string QuoteLineItemUDF3, string QuoteLineItemUDF4, string QuoteLineItemUDF5)
        {
            bool isInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            if (isInsert)
            {
                List<SelectListItem> returnList = new List<SelectListItem>();
                returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.UnSubmitted.ToString()), Value = ((int)OrderStatus.UnSubmitted).ToString() });
                bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderSubmit);
                bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderApproval);
                if (IsApprove)
                {
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Submitted.ToString()), Value = ((int)OrderStatus.Submitted).ToString() });
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Approved.ToString()), Value = ((int)OrderStatus.Approved).ToString() });
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Transmitted.ToString()), Value = ((int)OrderStatus.Transmitted).ToString() });
                }
                else if (IsSubmit)
                {
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Submitted.ToString()), Value = ((int)OrderStatus.Submitted).ToString() });
                }
                ViewBag.OrderStatusList = returnList;

                string strSupplierIds = string.Empty;
                if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SessionHelper.UserSupplierIds);
                }

                SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                List<SupplierMasterDTO> lstSuppliers = new List<SupplierMasterDTO>();
                lstSuppliers = objSupplierMasterDAL.GetNonDeletedSupplierByIDsNormal(strSupplierIds, SessionHelper.RoomID, SessionHelper.CompanyID);

                ViewBag.lstRoomSuppliers = lstSuppliers;

                bool DoGroupSupplierQuoteToOrder = false;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                string columnList = "ID,RoomName,DoGroupSupplierQuoteToOrder";
                RoomDTO objRoom = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
                if (objRoom != null && objRoom.ID > 0)
                {
                    DoGroupSupplierQuoteToOrder = objRoom.DoGroupSupplierQuoteToOrder;
                }
                ViewBag.DoGroupSupplierQuoteToOrder = DoGroupSupplierQuoteToOrder;
                ViewBag.QuoteGuid = QuoteGuid;
                CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
                var tmpsupplierIds = new List<long>();
                IList<OrderMasterDTO> lstOrders = objCartItemDAL.GetOrdersBySelectedQuote(QuoteGuid, Ids, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriseDBName, QuoteLineItemUDF1, QuoteLineItemUDF2, QuoteLineItemUDF3, QuoteLineItemUDF4, QuoteLineItemUDF5, SessionHelper.UserSupplierIds,SessionHelper.EnterPriceID);

                if (lstOrders != null && lstOrders.Count > 0)
                {
                    lstOrders.ToList().ForEach(t =>
                    {
                        if (IsApprove)
                        {
                            t.OrderStatus = (int)OrderStatus.Approved;
                        }
                        else if (IsSubmit)
                        {
                            t.OrderStatus = (int)OrderStatus.Submitted;
                        }
                        else
                        {

                            t.OrderStatus = (int)OrderStatus.UnSubmitted;
                        }
                        SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
                        System.Collections.Generic.List<SupplierAccountDetailsDTO> objSupplierAccount = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(Convert.ToInt64(t.Supplier), SessionHelper.RoomID, SessionHelper.CompanyID).Where(s => s.IsDefault == true).ToList();
                        if (objSupplierAccount != null && objSupplierAccount.Count() > 0)
                        {
                            ViewBag.SupplierAccount = objSupplierAccount;
                            t.SupplierAccountGuid = objSupplierAccount.Where(s => s.IsDefault == true).FirstOrDefault().GUID;
                        }
                    });

                }
                return PartialView("CreateQuoteOrder", lstOrders);
            }
            else
            {
                return null;
            }
        }

        public JsonResult GetOrderDetailsBySupplier(Int64 SupplierID)
        {
            bool isSameSuppier = false;
            string orderNumber = string.Empty;
            int ReleaseNo = 1;
            AutoOrderNumberGenerate objAutoNumber = null;
            SupplierMasterDTO objSupplierDTO = new SupplierMasterDTO();
            SupplierMasterDAL objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

            objAutoNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextOrderNumber(SessionHelper.RoomID, SessionHelper.CompanyID, SupplierID, EnterpriseId, objAutoNumber, isSameSuppier);
            orderNumber = objAutoNumber.OrderNumber;
            isSameSuppier = true;

            //if (orderNumber == null || string.IsNullOrWhiteSpace(orderNumber))
            //{
            //    orderNumber = Convert.ToDateTime(DateTime.Now.ToShortDateString()).ToString("yyyy-MM-dd").Replace("-", "");
            //}

            if (!string.IsNullOrWhiteSpace(orderNumber))
            {
                OrderMasterDAL objOrderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                ReleaseNo = objOrderDAL.GetNextReleaseNumber(orderNumber, null, SessionHelper.RoomID, SessionHelper.CompanyID);
            }

            objSupplierDTO = objSupDAL.GetSupplierByIDPlain(SupplierID);
            if (objSupplierDTO != null)
            {
                if (objSupplierDTO.POAutoSequence.GetValueOrDefault(0) == 1)
                {
                    if (!string.IsNullOrWhiteSpace(objSupplierDTO.POAutoNrReleaseNumber) && objSupplierDTO.NextOrderNo.Trim() == orderNumber.Trim())
                    {
                        ReleaseNo = Convert.ToInt32(objSupplierDTO.POAutoNrReleaseNumber) + 1;
                    }
                }
            }

            #region OrderStatus
            List<SelectListItem> OrderStatusList = new List<SelectListItem>();
            OrderStatusList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.UnSubmitted.ToString()), Value = ((int)OrderStatus.UnSubmitted).ToString() });
            bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderSubmit);
            bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderApproval);
            if (IsApprove)
            {
                OrderStatusList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Submitted.ToString()), Value = ((int)OrderStatus.Submitted).ToString() });
                OrderStatusList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Approved.ToString()), Value = ((int)OrderStatus.Approved).ToString() });
                OrderStatusList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Transmitted.ToString()), Value = ((int)OrderStatus.Transmitted).ToString() });
            }
            else if (IsSubmit)
            {
                OrderStatusList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Submitted.ToString()), Value = ((int)OrderStatus.Submitted).ToString() });
            }
            if (objSupplierDTO != null && OrderStatusList != null && OrderStatusList.Count > 0)
            {
                if (objSupplierDTO.IsSendtoVendor
                    && OrderStatusList.FindIndex(x => x.Value == "4") >= 0)
                {
                    OrderStatusList.RemoveAll(x => x.Value == "4");
                }
                else if (IsApprove && OrderStatusList.FindIndex(x => x.Value == "4") < 0
                    && OrderStatusList.FindIndex(x => x.Value == "3") >= 0)
                {
                    OrderStatusList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Transmitted.ToString()), Value = ((int)OrderStatus.Transmitted).ToString() });
                }
            }
            int SelectedOrderStatus = (int)OrderStatus.UnSubmitted;
            if (IsApprove)
            {
                SelectedOrderStatus = (int)OrderStatus.Approved;
            }
            else if (IsSubmit)
            {
                SelectedOrderStatus = (int)OrderStatus.Submitted;
            }
            #endregion

            int DReqOrderDays = 0;
            DateTime? RequiredDate = null;
            string RequiredDateStr = "";
            if (objSupplierDTO != null)
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                DateTime CurrentDateTime = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

                DReqOrderDays = objSupplierDTO.DefaultOrderRequiredDays.GetValueOrDefault(0);
                RequiredDate = CurrentDateTime.AddDays(DReqOrderDays);
                RequiredDateStr = Convert.ToString(FnCommon.ConvertDateByTimeZone(RequiredDate, false, true).Split(' ')[0]);
            }

           //OrderNumber_ForSorting = objAutoNumber.OrderNumberForSorting,
            return Json(new { OrderNumber = orderNumber, SelectedOrderStatus = SelectedOrderStatus, ReleaseNo = ReleaseNo, OrderStatusList = OrderStatusList, RequiredDate = RequiredDateStr,AutoOrderNumber = objAutoNumber , IsBlanketOrder = objAutoNumber.IsBlanketPO }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult CreateOrdersfromQuoteItems(List<OrderMasterDTO> lstOrders, Guid QuoteGuid)
        {
            try
            {
                #region check dynamic validation rules
                List<OrderMasterDTO> inValidOrders = new List<OrderMasterDTO>();
                if (lstOrders != null && lstOrders.Count > 0)
                {
                    string validationMsg = "";
                    int cnt = 0;
                    foreach (OrderMasterDTO objOrderMasterDTO in lstOrders)
                    {
                        if (!string.IsNullOrWhiteSpace(objOrderMasterDTO.RequiredDateString))
                        {
                            objOrderMasterDTO.RequiredDate = DateTime.ParseExact(objOrderMasterDTO.RequiredDateString, Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult);
                        }

                        var validationResultList = DTOCommonUtils.ValidateDTO<OrderMasterDTO>(objOrderMasterDTO, ControllerContext, new List<string>() { "PackSlipNumber" });
                        if (validationResultList.HasErrors())
                        {
                            if (!string.IsNullOrWhiteSpace(validationMsg))
                            {
                                validationMsg += "<br/><br/>";
                            }

                            string msg = validationResultList.GetShortErrorMessage(typeof(RequiredAttributeAdapter));
                            validationMsg += (string.Format(ResQuoteMaster.ValidationFailedFor, objOrderMasterDTO.SupplierName) +  "<br/>" + msg); 
                            cnt++;
                        }

                        if (cnt >= 5)
                        {
                            // display message for 5 grid rows to shorten message
                            break;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(validationMsg))
                    {
                        return Json(new { Message = validationMsg, Status = "fail", lstOrders = lstOrders });
                    }
                }
                #endregion

                QuoteMasterDAL objQuoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
                CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
                OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderSubmit);
                bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderApproval);

                int? PriseSelectionOption = 0;
                eTurns.DAL.RoomDAL onjRoomDAL = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName);
                RoomModuleSettingsDTO objRoomModuleSettingsDTO = onjRoomDAL.GetRoomModuleSettings(eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Orders);
                if (objRoomModuleSettingsDTO != null)
                    PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption;

                if (PriseSelectionOption != 1 && PriseSelectionOption != 2)
                    PriseSelectionOption = 1;

                if (lstOrders != null && lstOrders.Count > 0)
                {

                    //---------------------- Check For Order Number Duplication ----------------------
                    //
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,IsAllowOrderDuplicate";
                    RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

                    if (roomDTO.IsAllowOrderDuplicate != true)
                    {
                        var OrdrGroup = (from O in lstOrders
                                         group O by new { OrderNumberGB = O.OrderNumber.Trim().ToLower() } into OGB
                                         select new
                                         {
                                             OrderNumber = OGB.Key.OrderNumberGB,
                                             TotalCount = OGB.Count()
                                         }).Where(x => x.TotalCount > 1).ToList();
                        
                        var orderDuplicateInListMsg = ResOrder.OrderNumberDuplicateInList;
                        if (OrdrGroup.Count > 0)
                        { 
                            return Json(new { Message = string.Format(ResOrder.OrderNumberDuplicateInList, OrdrGroup[0].OrderNumber), Status = "fail", lstOrders = lstOrders });
                        }
                        var orderNoExistMsg = ResOrder.OrderNumberAlreadyExist;
                        foreach (OrderMasterDTO objOrderMasterDTO in lstOrders)
                        {
                            if (objOrderMasterDAL.IsOrderNumberDuplicateById(objOrderMasterDTO.OrderNumber, objOrderMasterDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID))
                            {
                                return Json(new { Message = string.Format(orderNoExistMsg,objOrderMasterDTO.OrderNumber), Status = "fail", lstOrders = lstOrders });
                            }
                        }
                    }
                    DollerApprovalLimitDTO objDollarLimt = null;
                    eTurns.DAL.UserMasterDAL userDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                    string approvalErrorMsg = string.Empty;
                    string OrdapprovalSuppErrorMsg = string.Empty;
                    CommonDAL commonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    double AllOrderCost = 0;
                    double AllOrderPrice = 0;
                    foreach (OrderMasterDTO objOrderMasterDTO in lstOrders)
                    {
                        if (SessionHelper.AllowABIntegration)
                        {
                            List<Guid> lstABids = new List<Guid>();
                            if (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemsIds))
                            {
                                foreach (var item in objOrderMasterDTO.OrderLineItemsIds.Split(','))
                                {
                                    Guid tempid = Guid.Empty;
                                    if (Guid.TryParse(item, out tempid))
                                        lstABids.Add(tempid);
                                }
                            }
                            List<QuoteDetailDTO> lstABQuoteItems = objCartItemDAL.GetQuoteItemsByGuids(QuoteGuid, lstABids, SessionHelper.RoomID, SessionHelper.CompanyID, true);
                            lstABQuoteItems = lstABQuoteItems.Where(x => !x.IsOrdered).ToList();

                            List<string> ASINs = new List<string>();
                            if (lstABQuoteItems != null && lstABQuoteItems.Count > 0)
                            {
                                #region WI-7318	AB Integration | Sync Item cost when an item Added to Order line item.
                                if (SessionHelper.AllowABIntegration)
                                {
                                    foreach (var ABCartitem in lstABQuoteItems)
                                    {
                                        ItemMasterDTO objABItemDTO = new ItemMasterDTO();
                                        objABItemDTO = objItemMasterDAL.GetItemWithMasterTableJoins(null, ABCartitem.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                                        if (objABItemDTO != null && !string.IsNullOrWhiteSpace(objABItemDTO.SupplierPartNo))
                                        {
                                            ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
                                            Int64 ABItemMappingID = objProductDetailsDAL.CheckItemAddedFromAB(objABItemDTO.SupplierPartNo, objABItemDTO.GUID, SessionHelper.CompanyID, SessionHelper.RoomID);
                                            if (ABItemMappingID > 0)
                                            {
                                                ASINs.Add(objABItemDTO.SupplierPartNo);
                                            }
                                        }
                                    }
                                    if (ASINs != null && ASINs.Count > 0)
                                    {
                                        eTurns.ABAPIBAL.Helper.ABAPIHelper.ItemSyncToRoom(ASINs, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
                                    }
                                }
                                #endregion
                            }
                        }

                        objOrderMasterDTO.RequiredDate = DateTime.ParseExact(objOrderMasterDTO.RequiredDateString, Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult);
                        if (IsApprove)
                        {
                            double? UserApprovalLimit = null;
                            double UserUsedLimit = 0;
                            double OrderCost = 0;
                            double OrderPrice = 0;
                            objDollarLimt = null;

                            double? ItemApprovedQuantity = null;

                            objDollarLimt = userDAL.GetOrderLimitByUserId(SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID);
                            if (objDollarLimt != null)
                            {
                                UserApprovalLimit = objDollarLimt.DollerLimit > 0 ? objDollarLimt.DollerLimit : null;
                                UserUsedLimit = objDollarLimt.UsedLimit;
                                ItemApprovedQuantity = objDollarLimt.ItemApprovedQuantity > 0 ? objDollarLimt.ItemApprovedQuantity : null;
                            }

                            if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Submitted)
                            {
                                if (objOrderMasterDTO.Supplier != null && objOrderMasterDTO.Supplier.GetValueOrDefault(0) > 0)
                                {
                                    OrderController ordCntrl = new OrderController();
                                    bool IsSuppApprove = ordCntrl.CheckSupplierAproveRight(objOrderMasterDTO.Supplier.GetValueOrDefault(0));
                                    if (IsSuppApprove)
                                        objOrderMasterDTO.OrderStatus = (int)OrderStatus.Approved;
                                }
                            }

                            if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Approved || objOrderMasterDTO.OrderStatus == (int)OrderStatus.Transmitted)
                            {
                                List<Guid> lstids = new List<Guid>();
                                if (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemsIds))
                                {
                                    foreach (var item in objOrderMasterDTO.OrderLineItemsIds.Split(','))
                                    {
                                        Guid tempid = Guid.Empty;
                                        if (Guid.TryParse(item, out tempid))
                                            lstids.Add(tempid);
                                    }
                                    var tmpsupplierIds = new List<long>();
                                    List<QuoteDetailDTO> lstQuoteItems = objCartItemDAL.GetQuoteItemsByGuids(QuoteGuid, lstids, SessionHelper.RoomID, SessionHelper.CompanyID, true);
                                    lstQuoteItems = lstQuoteItems.Where(x => !x.IsOrdered).ToList();
                                    foreach (QuoteDetailDTO quoteitem in lstQuoteItems)
                                    {
                                        objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain(quoteitem.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                                        if (quoteitem != null && quoteitem.RequestedQuantity != null
                                                && quoteitem.RequestedQuantity > 0)
                                        {
                                            if (objDollarLimt != null && objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && IsApprove && objOrderMasterDTO.OrderStatus == (int)OrderStatus.Approved
                                               && ItemApprovedQuantity > 0 && quoteitem.RequestedQuantity.GetValueOrDefault(0) > (ItemApprovedQuantity))
                                            {
                                                return Json(new { Message = string.Format(ResOrder.CantApproveMoreThanPerOrderItemQtyApprovalLimit, quoteitem.RequestedQuantity.GetValueOrDefault(0), ItemApprovedQuantity), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                            }
                                        }

                                        CostUOMMasterDTO costUOM = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName).GetCostUOMByID(objItemMasterDTO.CostUOMID.GetValueOrDefault(0));
                                        if (costUOM == null)
                                            costUOM = new CostUOMMasterDTO() { CostUOMValue = 1 };

                                        if (objItemMasterDTO != null && quoteitem.RequestedQuantity.GetValueOrDefault(0) > 0)
                                        {
                                            OrderCost += (objItemMasterDTO.Cost.GetValueOrDefault(0) * quoteitem.RequestedQuantity.GetValueOrDefault(0))
                                                     / (costUOM.CostUOMValue.GetValueOrDefault(0) > 0 ? costUOM.CostUOMValue.GetValueOrDefault(1) : 1);

                                            OrderPrice += (objItemMasterDTO.SellPrice.GetValueOrDefault(0) * quoteitem.RequestedQuantity.GetValueOrDefault(0))
                                                   / (costUOM.CostUOMValue.GetValueOrDefault(0) > 0 ? costUOM.CostUOMValue.GetValueOrDefault(1) : 1);

                                            AllOrderCost += OrderCost;
                                            AllOrderPrice += OrderPrice;

                                            objOrderMasterDTO.OrderPrice += OrderPrice;
                                            objOrderMasterDTO.OrderCost = objOrderMasterDTO.OrderCost.GetValueOrDefault(0) + OrderCost;

                                        }

                                        if (SessionHelper.RoleID > 0 && objDollarLimt != null)
                                        {
                                            if (PriseSelectionOption == 1)
                                            {
                                                if (objDollarLimt.OrderLimitType == OrderLimitType.All && (OrderPrice > (UserApprovalLimit - UserUsedLimit) || AllOrderPrice > (UserApprovalLimit - UserUsedLimit))) 
                                                    approvalErrorMsg += "<br/>" + string.Format(ResOrder.SupplierOrderCantApproveMTRemainingOrderApprovalLimit, objOrderMasterDTO.SupplierName, objOrderMasterDTO.OrderNumber, OrderPrice, (UserApprovalLimit - (UserUsedLimit + (AllOrderPrice - OrderPrice)))) + "<br/>";
                                                else if (objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && OrderPrice > (UserApprovalLimit)) 
                                                    approvalErrorMsg += "<br/>" + string.Format(ResOrder.SupplierOrderCantApproveMTPerOrderApprovalLimit, objOrderMasterDTO.SupplierName, objOrderMasterDTO.OrderNumber, OrderPrice, UserApprovalLimit) + "<br/>";
                                            }
                                            else
                                            {
                                                if (objDollarLimt.OrderLimitType == OrderLimitType.All && (OrderCost > (UserApprovalLimit - UserUsedLimit) || AllOrderCost > (UserApprovalLimit - UserUsedLimit))) 
                                                    approvalErrorMsg += "<br/>" + string.Format(ResOrder.SupplierOrderCantApproveMTRemainingOrderApprovalLimit, objOrderMasterDTO.SupplierName, objOrderMasterDTO.OrderNumber, OrderCost, (UserApprovalLimit - (UserUsedLimit + (AllOrderCost - OrderCost)))) + "<br/>";
                                                else if (objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && OrderCost > (UserApprovalLimit)) 
                                                    approvalErrorMsg += "<br/>" + string.Format(ResOrder.SupplierOrderCantApproveMTPerOrderApprovalLimit, objOrderMasterDTO.SupplierName, objOrderMasterDTO.OrderNumber, OrderCost, UserApprovalLimit) + "<br/>";
                                            }
                                        }
                                    }
                                }
                                if (objOrderMasterDTO.Supplier != null && objOrderMasterDTO.Supplier.GetValueOrDefault(0) > 0)
                                {
                                    UserAccessDTO objUserAccess = null;
                                    if (SessionHelper.UserType == 1)
                                    {
                                        eTurnsMaster.DAL.UserMasterDAL objUserdal = new eTurnsMaster.DAL.UserMasterDAL();
                                        objUserAccess = objUserdal.GetUserRoomAccessesByUserId(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                                    }
                                    else
                                    {
                                        eTurns.DAL.UserMasterDAL objUserdal = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                                        objUserAccess = objUserdal.GetUserRoomAccessesByUserId(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                                    }

                                    if (objUserAccess != null && !string.IsNullOrWhiteSpace(objUserAccess.SupplierIDs))
                                    {
                                        List<string> strSupplier = objUserAccess.SupplierIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        if (strSupplier != null && strSupplier.Count > 0)
                                        {
                                            if (strSupplier.Contains(objOrderMasterDTO.Supplier.ToString()) == false)
                                            {
                                                if (string.IsNullOrEmpty(OrdapprovalSuppErrorMsg))
                                                    OrdapprovalSuppErrorMsg = ResOrder.OrderNo +  " " + objOrderMasterDTO.OrderNumber + ", " + ResOrder.SupplierApprove;
                                                else
                                                    OrdapprovalSuppErrorMsg += "<br/>" + ResOrder.OrderNo +  " " + objOrderMasterDTO.OrderNumber + ", " + ResOrder.SupplierApprove;
                                            }
                                        }
                                    }
                                }
                            }

                            //WI-8417
                            if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Approved && objOrderMasterDTO.OrderStatus != (int)OrderStatus.Closed)
                            {
                                List<Guid> lstids = new List<Guid>();
                                List<QuoteDetailDTO> lstQuoteItems = null;
                                OrderController objOrdContlr = new OrderController();

                                if (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemsIds))
                                {
                                    foreach (var item in objOrderMasterDTO.OrderLineItemsIds.Split(','))
                                    {
                                        Guid tempid = Guid.Empty;
                                        if (Guid.TryParse(item, out tempid))
                                            lstids.Add(tempid);
                                    }
                                    var tmpsupplierIds = new List<long>();
                                    lstQuoteItems = objCartItemDAL.GetQuoteItemsByGuids(QuoteGuid, lstids, SessionHelper.RoomID, SessionHelper.CompanyID, true);
                                    lstQuoteItems = lstQuoteItems.Where(x => !x.IsOrdered).ToList();
                                }

                                if (lstQuoteItems != null && lstQuoteItems.Count > 0)
                                {
                                    string validationMsgForOrder = string.Empty;
                                    string[] AllGuid = objOrderMasterDTO.OrderLineItemsIds.Split(',').ToArray();
                                    
                                    List<OrderDetailsDTO> lstOfItems = new List<OrderDetailsDTO>();

                                    foreach (QuoteDetailDTO quoteitem in lstQuoteItems)
                                    {
                                        objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain(quoteitem.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                                        CostUOMMasterDTO costUOM = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName).GetCostUOMByID(objItemMasterDTO.CostUOMID.GetValueOrDefault(0));

                                        int Index = Array.FindIndex(AllGuid, row => row.Contains(quoteitem.GUID.ToString()));
                                       
                                        lstOfItems.Add(new OrderDetailsDTO
                                        {
                                            ApprovedQuantity = quoteitem.RequestedQuantity.GetValueOrDefault(0),
                                            ItemGUID = quoteitem.ItemGUID,
                                            OrderLineItemExtendedCost = (quoteitem.RequestedQuantity.GetValueOrDefault(0) * (objItemMasterDTO.Cost.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1)))
                                        });
                                    }

                                    var isValidOrderLineItems = commonDAL.ValidateOrderItemOnSupplierBlanketPO(objOrderMasterDTO.GUID, lstOfItems, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, SessionHelper.UserID, out validationMsgForOrder);
                                    if (!isValidOrderLineItems)
                                    {
                                        objOrderMasterDTO.IsValid = false;
                                        objOrderMasterDTO.Status = "fail";
                                        objOrderMasterDTO.Message = validationMsgForOrder;
                                        inValidOrders.Add(objOrderMasterDTO);
                                    }
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(approvalErrorMsg))
                    {
                        return Json(new { Message = approvalErrorMsg, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }
                    if (!string.IsNullOrEmpty(OrdapprovalSuppErrorMsg))
                    {
                        return Json(new { Message = OrdapprovalSuppErrorMsg, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }
                }

                Dictionary<string, string> rejectedOrderLineItems = new Dictionary<string, string>();
                Dictionary<string, string> rejectedOrders = new Dictionary<string, string>();
                var validateQuoteItem = ValidateQuoteLineItemsForMaxOrderQty(QuoteGuid, lstOrders, out rejectedOrderLineItems, out rejectedOrders);
                Dictionary<string, string> rejectedOrderLineItemstest = new Dictionary<string, string>();
                long SessionUserId = SessionHelper.UserID;
                if (lstOrders != null && lstOrders.Count > 0 && lstOrders.Any(x => x.IsValid))
                {
                    lstOrders = objQuoteMasterDAL.CreateOrdersByQuote(QuoteGuid, lstOrders, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriseDBName, 1, SessionHelper.EnterPriceID, out rejectedOrderLineItemstest, SessionUserId, "");
                }
                if (lstOrders != null && lstOrders.Count > 0 && lstOrders.Any(x => x.IsValid))
                {
                    OrderController objOrdContlr = new OrderController();
                    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                    eTurns.DAL.UserMasterDAL objUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                    OrderMasterDAL orderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);

                    foreach (OrderMasterDTO Orditem in lstOrders.Where(x => x.IsValid))
                    {
                        if (Orditem.OrderStatus == (int)OrderStatus.UnSubmitted)
                        {
                            SupplierMasterDTO objSupplier = objSupplierMasterDAL.GetSupplierByIDPlain(Orditem.Supplier ?? 0);
                            objOrdContlr.SendMailOrderUnSubmitted(objSupplier, Orditem);
                        }
                        if (Orditem.OrderStatus == (int)OrderStatus.Submitted)
                        {
                            objOrdContlr.SendMailToApprovalAuthority(Orditem);
                        }

                        if (Orditem.OrderStatus == (int)OrderStatus.Approved || Orditem.OrderStatus == (int)OrderStatus.Transmitted)
                        {
                            SupplierMasterDTO objSupplier = objSupplierMasterDAL.GetSupplierByIDPlain(Orditem.Supplier ?? 0);
                            objOrdContlr.SendMailToSupplier(objSupplier, Orditem);
                            eTurns.DAL.UserMasterDAL userMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);

                            eTurnsMaster.DAL.UserMasterDAL objReqRequesterUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                            UserMasterDTO ReqUser = new UserMasterDTO();

                            string OrdRequesterEmailAddress = "";
                            string OrdApproverEmailAddress = "";
                            if (Orditem.RequesterID.GetValueOrDefault(0) > 0)
                            {
                                ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(Orditem.RequesterID.GetValueOrDefault(0));
                                if (ReqUser == null)
                                {
                                    ReqUser = userMasterDAL.GetUserByIdPlain(Orditem.RequesterID.GetValueOrDefault(0));
                                }
                                if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                                {
                                    OrdRequesterEmailAddress = ReqUser.Email;
                                }
                            }
                            if (Orditem.ApproverID.GetValueOrDefault(0) > 0)
                            {
                                ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(Orditem.ApproverID.GetValueOrDefault(0));
                                if (ReqUser == null)
                                {
                                    ReqUser = userMasterDAL.GetUserByIdPlain(Orditem.ApproverID.GetValueOrDefault(0));
                                }
                                if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                                {
                                    OrdApproverEmailAddress = ReqUser.Email;
                                }
                            }
                            objOrdContlr.SendMailForApprovedOrReject(Orditem, "approved", OrdRequesterEmailAddress, OrdApproverEmailAddress);
                            double? UserApprovalLimit = null;
                            double UserUsedLimit = 0;
                            double OrderCost = 0;
                            double OrderPrice = 0;
                            DollerApprovalLimitDTO objDollarLimt = null;
                            eTurns.DAL.UserMasterDAL userDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                            objDollarLimt = userDAL.GetOrderLimitByUserId(SessionHelper.UserID, Orditem.CompanyID ?? 0, Orditem.Room ?? 0);

                            if (SessionHelper.RoleID > 0 && objDollarLimt != null && objDollarLimt.OrderLimitType == OrderLimitType.All)
                            {
                                UserApprovalLimit = objDollarLimt.DollerLimit;
                                UserUsedLimit = objDollarLimt.UsedLimit;

                                if (PriseSelectionOption == 1)
                                {
                                    OrderPrice = Orditem.OrderPrice;

                                    if (OrderPrice <= (UserApprovalLimit - UserUsedLimit))
                                    {
                                        userDAL.SaveDollerUsedLimt(OrderPrice, SessionHelper.UserID, Orditem.CompanyID ?? 0, Orditem.Room ?? 0);
                                    }
                                }
                                else
                                {
                                    OrderCost = Orditem.OrderCost.GetValueOrDefault(0);

                                    if (OrderCost <= (UserApprovalLimit - UserUsedLimit))
                                    {
                                        userDAL.SaveDollerUsedLimt(OrderCost, SessionHelper.UserID, Orditem.CompanyID ?? 0, Orditem.Room ?? 0);
                                    }
                                }
                            }
                        }
                    }
                }
                string message = string.Empty;
                string status = string.Empty;
                if (inValidOrders != null && inValidOrders.Count > 0)
                {
                    foreach (var Order in inValidOrders.Where(x => x.IsValid == false))
                    {
                        return Json(new { Message = Order.Message, Status = Order.Status, ID = Order.ID }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (!rejectedOrderLineItems.Any()
                    && !rejectedOrders.Any())
                {
                    message = ResMessage.SaveMessage;
                    status = "ok";
                }
                else
                {
                    if (rejectedOrderLineItems.Any())
                    {
                        message = ResQuoteMaster.BelowGivenItemsRejected;
                        var itemNamberResource = ResItemMaster.ItemNumber;
                        var reasonResource = ResCommon.Reason;
                        foreach (KeyValuePair<string, string> entry in rejectedOrderLineItems)
                        {
                            message +=  (itemNamberResource + ": " + entry.Key + " ," + reasonResource + ": " + entry.Value);
                        }
                    }
                    if (rejectedOrders.Any())
                    {
                        if (!string.IsNullOrWhiteSpace(message)) 
                        {
                            message += ResQuoteMaster.BelowGivenSuppliersRejected;
                        }
                        else
                        {
                            message = ResQuoteMaster.BelowGivenSuppliersRejected;

                        }
                        var supplierNamberResource = ResItemSupplierDetails.SupplierNumber;
                        var reasonResourceValue = ResCommon.Reason;
                        foreach (KeyValuePair<string, string> entry in rejectedOrders)
                        {
                            message += (supplierNamberResource +  ": " + entry.Key + " ," + reasonResourceValue + ": " + entry.Value);
                        }
                    }

                    status = "fail";
                }

                GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID).UpdateRedCircleCountInClients();
                return Json(new { Message = message, Status = status, lstOrders = lstOrders });
            }
            catch
            {
                string message = ResMessage.SaveErrorMsg;
                string status = "fail";
                return Json(new { Message = message, Status = status, lstOrders = lstOrders });
            }
        }
        private bool ValidateQuoteLineItemsForMaxOrderQty(Guid QuoteGuid, List<OrderMasterDTO> lstOrders, out Dictionary<string, string> rejectedOrderLineItems, out Dictionary<string, string> rejectedOrders)
        {
            CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDAL objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            OrderMasterDTO objOrder = new OrderMasterDTO();
            OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            Dictionary<Guid, double> itemsOrderQtyForItemMinMax = new Dictionary<Guid, double>();
            Dictionary<string, double> itemsOrderQtyForBinMinMax = new Dictionary<string, double>();
            rejectedOrderLineItems = new Dictionary<string, string>();
            rejectedOrders = new Dictionary<string, string>();
            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,PreventMaxOrderQty";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            List<int> ordertoBeDelete = new List<int>();

            if (objRoomDTO.PreventMaxOrderQty != (int)PreventMaxOrderQty.OnOrder)
            {
                return true;
            }
            int rejectedDueToPreventMaxOrderSizeValidation = 0;
            List<string> rejectedOrderGuids = new List<string>();

            List<OrderMasterDTO> lstOrdersmaxSize = new List<OrderMasterDTO>();
            lstOrdersmaxSize = lstOrders;
            for (int orderCount = 0; orderCount < lstOrdersmaxSize.Count; orderCount++)
            {
                var order = lstOrdersmaxSize[orderCount];
                if (order.IsValid)
                {
                    List<Guid> lstids = new List<Guid>();

                    if (!string.IsNullOrWhiteSpace(order.OrderLineItemsIds))
                    {
                        foreach (var item in order.OrderLineItemsIds.Split(','))
                        {
                            Guid tempid = Guid.Empty;
                            if (Guid.TryParse(item, out tempid))
                            {
                                lstids.Add(tempid);
                            }
                        }
                    }

                    SupplierMasterDTO objSupplierDTO = new SupplierMasterDTO();
                    objSupplierDTO = objSupDAL.GetSupplierByIDPlain(order.Supplier.GetValueOrDefault(0));
                    if (objSupplierDTO != null
                        && objSupplierDTO.MaximumOrderSize != null
                        && objSupplierDTO.MaximumOrderSize > 0
                        && lstids.Count > objSupplierDTO.MaximumOrderSize.GetValueOrDefault(0))
                    {
                        if (!(rejectedOrders.ContainsKey(Convert.ToString(order.SupplierName))))
                        {
                            rejectedOrders[Convert.ToString(order.SupplierName)] = ResQuoteMaster.NotAddedSupplierMaxOrderSizeReached;
                        }
                        rejectedOrderGuids.Add(Convert.ToString(order.SupplierName));
                        rejectedDueToPreventMaxOrderSizeValidation++;
                        lstOrders.Remove(order);
                        continue;
                    }
                    //else
                    //{
                    //    lstOrdersmaxSize.Add(order);
                    //}
                }
            }
            //lstOrders = lstOrdersmaxSize;
            for (int orderCount = 0; orderCount < lstOrders.Count; orderCount++)
            {
                var order = lstOrders[orderCount];
                if (order.IsValid)
                {
                    List<Guid> lstids = new List<Guid>();

                    if (!string.IsNullOrWhiteSpace(order.OrderLineItemsIds))
                    {
                        foreach (var item in order.OrderLineItemsIds.Split(','))
                        {
                            Guid tempid = Guid.Empty;
                            if (Guid.TryParse(item, out tempid))
                            {
                                lstids.Add(tempid);
                            }
                        }
                    }
                    if (lstids.Count > 0)
                    {
                        var tmpsupplierIds = new List<long>();
                        List<QuoteDetailDTO> lstQuoteItems = objCartItemDAL.GetQuoteItemsByGuids(QuoteGuid, lstids, SessionHelper.RoomID, SessionHelper.CompanyID, true);

                        int rejectedDueToPreventMaxValidation = 0;
                        List<string> rejectedOrderLineItemsGuids = new List<string>();

                        foreach (QuoteDetailDTO quoteitem in lstQuoteItems)
                        {
                            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                            objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain(quoteitem.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                            if (objItemMasterDTO.IsItemLevelMinMaxQtyRequired.HasValue && objItemMasterDTO.IsItemLevelMinMaxQtyRequired.Value)
                            {
                                var tmpItemOnOrderQty = objItemMasterDTO.OnOrderQuantity.GetValueOrDefault(0);
                                double itemOrderQtySoFar = 0;

                                if (itemsOrderQtyForItemMinMax.ContainsKey(objItemMasterDTO.GUID))
                                {
                                    itemOrderQtySoFar += itemsOrderQtyForItemMinMax[objItemMasterDTO.GUID];
                                }

                                if (objItemMasterDTO.MaximumQuantity.HasValue && objItemMasterDTO.MaximumQuantity.Value > 0 && (tmpItemOnOrderQty + itemOrderQtySoFar + quoteitem.RequestedQuantity.GetValueOrDefault(0)) > objItemMasterDTO.MaximumQuantity.Value)
                                {
                                    if (!(rejectedOrderLineItems.ContainsKey(objItemMasterDTO.ItemNumber)))
                                    {
                                        rejectedOrderLineItems[objItemMasterDTO.ItemNumber] = ResOrder.NotAddedItemMaxQtyReached;
                                    }
                                    rejectedOrderLineItemsGuids.Add(Convert.ToString(quoteitem.GUID));
                                    rejectedDueToPreventMaxValidation++;
                                    continue;
                                }
                                else
                                {
                                    itemsOrderQtyForItemMinMax[objItemMasterDTO.GUID] = (
                                            itemsOrderQtyForItemMinMax.ContainsKey(objItemMasterDTO.GUID)
                                            ? (itemsOrderQtyForItemMinMax[objItemMasterDTO.GUID] + (quoteitem.RequestedQuantity.GetValueOrDefault(0)))
                                            : quoteitem.RequestedQuantity.GetValueOrDefault(0)
                                            );
                                }
                            }
                            else
                            {
                                List<BinMasterDTO> lstItemBins = objBinDAL.GetItemLocations(objItemMasterDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.BinNumber).ToList();
                                var maxQtyAtBinLevel = lstItemBins.Where(e => e.BinNumber.Equals(quoteitem.BinName)).FirstOrDefault();
                                var tmpBinId = (maxQtyAtBinLevel != null && maxQtyAtBinLevel.ID > 0) ? maxQtyAtBinLevel.ID : quoteitem.BinID.GetValueOrDefault(0);
                                var onOrderQtyAtBin = objDAL.GetOrderdQtyOfItemBinWise(SessionHelper.RoomID, SessionHelper.CompanyID, objItemMasterDTO.GUID, tmpBinId);
                                var tmponOrderQtyAtBin = onOrderQtyAtBin;
                                double itemOrderQtySoFar = 0;

                                if (itemsOrderQtyForBinMinMax.ContainsKey(Convert.ToString(objItemMasterDTO.GUID) + "_" + quoteitem.BinName))
                                {
                                    itemOrderQtySoFar += itemsOrderQtyForBinMinMax[Convert.ToString(objItemMasterDTO.GUID) + "_" + quoteitem.BinName];
                                }

                                if (maxQtyAtBinLevel != null && maxQtyAtBinLevel.MaximumQuantity.HasValue && maxQtyAtBinLevel.MaximumQuantity.Value > 0 && (tmponOrderQtyAtBin + itemOrderQtySoFar + (quoteitem.RequestedQuantity.GetValueOrDefault(0))) > maxQtyAtBinLevel.MaximumQuantity.Value)
                                {
                                    if (!(rejectedOrderLineItems.ContainsKey(objItemMasterDTO.ItemNumber)))
                                    {
                                        rejectedOrderLineItems[objItemMasterDTO.ItemNumber] = ResOrder.NotAddedBinMaxQtyReached;
                                    }
                                    rejectedOrderLineItemsGuids.Add(Convert.ToString(quoteitem.GUID));
                                    rejectedDueToPreventMaxValidation++;
                                    continue;
                                }
                                else
                                {
                                    itemsOrderQtyForBinMinMax[Convert.ToString(objItemMasterDTO.GUID) + "_" + quoteitem.BinName] =
                                        (itemsOrderQtyForBinMinMax.ContainsKey(Convert.ToString(objItemMasterDTO.GUID) + "_" + quoteitem.BinName)
                                        ? (itemsOrderQtyForBinMinMax[Convert.ToString(objItemMasterDTO.GUID) + "_" + quoteitem.BinName] + (quoteitem.RequestedQuantity.GetValueOrDefault(0)))
                                        : quoteitem.RequestedQuantity.GetValueOrDefault(0)
                                        );
                                }
                            }
                        }

                        if (objRoomDTO.PreventMaxOrderQty == (int)PreventMaxOrderQty.OnOrder && rejectedOrderLineItemsGuids.Any())
                        {
                            List<string> Items = order.OrderLineItemsIds.Split(',').Select(i => i.Trim()).Where(i => i != string.Empty).ToList(); //Split them all and remove spaces
                            foreach (var guid in rejectedOrderLineItemsGuids)
                            {
                                Items.Remove(guid);
                            }
                            order.OrderLineItemsIds = string.Join(", ", Items.ToArray());
                        }

                        if (rejectedDueToPreventMaxValidation > 0)
                        {
                            if (rejectedDueToPreventMaxValidation == lstids.Count)
                            {
                                ordertoBeDelete.Add(orderCount);
                            }
                        }
                    }
                }
            }

            if (rejectedDueToPreventMaxOrderSizeValidation > 0 && lstOrders.Count > 0)
            {
                lstOrders.RemoveAt(lstOrders.FindIndex(x => rejectedOrderGuids.Contains(Convert.ToString(x.SupplierName))));
            }

            if (ordertoBeDelete != null && ordertoBeDelete.Any() && lstOrders.Count > 0)
            {
                foreach (var index in ordertoBeDelete)
                {
                    lstOrders.RemoveAt(index);
                }
            }

            return true;
        }

        #endregion

        #region unclose quote

        public JsonResult UnCloseQuoteToEdit(List<Guid> QuoteGuid, string EditType)
        {
            QuoteMasterDAL quoteDal = null;
            QuoteMasterDTO quoteDTO = null;
            List<QuoteDetailDTO> quotelineItems = null;
            QuoteDetailDAL quoteDetailDal = null;
            string message = "Error";
            string status = "fail";
            var orderLineItemsAreInvalid = false;
            var validationMessageForOrderLineItems = string.Empty;

            try
            {
                long SessionUserId = SessionHelper.UserID;
                
                if (QuoteGuid != null && QuoteGuid.Count > 0)
                {
                    quoteDal = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
                    quoteDetailDal = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
                    
                    foreach (var quoteGuid in QuoteGuid)
                    {
                        quoteDTO = quoteDal.GetQuoteByGuidPlain(quoteGuid);

                        if (!quoteDTO.IsEDIQuote.GetValueOrDefault(false))
                        {
                            quotelineItems = quoteDetailDal.GetDeletedOrUnDeletedQuoteDetailByQuoteGUIDPlain(quoteGuid, RoomID, CompanyID,false);
                            
                            if (EditType == "EditLineItems")
                            {
                                bool isOrderLineItemsValid = false;
                                
                                if (quotelineItems != null && quotelineItems.Count > 0)
                                {
                                    var validationMessage = string.Empty;
                                    isOrderLineItemsValid = true;//ValidateQuoteLineItemsForUncloseQuote(quoteGuid, out validationMessage);
                                    validationMessageForOrderLineItems += validationMessage;

                                    if (isOrderLineItemsValid)
                                    {
                                        foreach (var lineItem in quotelineItems)
                                        {
                                            lineItem.OrderedQuantity = 0;
                                            lineItem.IsEDISent = false;
                                            lineItem.EditedFrom = "Web";
                                            lineItem.LastUpdatedBy = SessionHelper.UserID;
                                            lineItem.LastUpdated = DateTimeUtility.DateTimeNow;
                                            lineItem.IsCloseItem = false;
                                            lineItem.IsOrdered = false;
                                            quoteDetailDal.UpdateQuoteDetail(lineItem, SessionUserId,SessionHelper.EnterPriceID);
                                        }
                                    }
                                    else
                                    {
                                        orderLineItemsAreInvalid = true;
                                    }
                                }

                                if (isOrderLineItemsValid)
                                {
                                    quoteDTO.QuoteStatus = (int)QuoteStatus.UnSubmitted;
                                    quoteDal.UpdateQuoteMaster(quoteDTO);
                                }
                            }
                        }
                    }

                    if (EditType == "EditLineItems" && orderLineItemsAreInvalid)
                    {
                        message = validationMessageForOrderLineItems;
                        status = "fail";
                    }
                    else
                    {
                        message = ResQuoteMaster.QuoteUnclosedSuccessfully;
                        status = "ok";
                    }
                }
                else
                {
                    message = ResOrder.NoOrdersFound;
                    status = "fail";
                }
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                message = ex.Message;
                status = "fail";
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
