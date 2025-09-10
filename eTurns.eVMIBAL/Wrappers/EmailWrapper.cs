using eTurns.DAL;
using eTurns.DTO;
using eTurns.eVMIBAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace eTurns.eVMIBAL.Wrappers
{
    public static class EmailWrapper
    {
        static string ErrorStyle = "color:#FF0000;";
        static string SuccessStyle = "color:#228B22";

      



        #region Bad Poll Emails
        /// <summary>
        /// Send Bad Poll Email if email template found
        /// </summary>
        /// <param name="badPolls"></param>
        //public static void SendBadPollEmail(PollEmailDTOCollection<BadPollEmailDTO> badPolls, PollEmailDTOCollection<ComPortEmailDTO> comErrors)
        //{
        //    try
        //    {
        //        if (badPolls.Count > 0)
        //        {
        //            string EnterpriseDBName = badPolls.EnterpriseDBName;
        //            long EnterpriseID = badPolls.EnterpriseID;

        //            PollEmailDTOCollection<BadPollEmailDTO> binNotOnSensor = badPolls.Where(p => p.BadPollType.ContainsKey(BadPollTypeEnum.BinNotOnSensor));
        //            SendBinNotOnSensorEmail(binNotOnSensor);

        //            PollEmailDTOCollection<BadPollEmailDTO> wrongItemOnBin = badPolls.Where(p => p.BadPollType.ContainsKey(BadPollTypeEnum.WrongItemOnBin));
        //            SendWrongItemOnBinEmail(wrongItemOnBin);

        //            SendComPortErrorEmail(comErrors);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteExceptionLog("SendBadPollEmail", ex);
        //    }
        //}

        //private static void SendBinNotOnSensorEmail(PollEmailDTOCollection<BadPollEmailDTO> badPolls)
        //{
        //    try
        //    {
        //        if (badPolls != null && badPolls.Count > 0)
        //        {

        //            EmailTemplateDAL templateDAL = new EmailTemplateDAL(badPolls.EnterpriseDBName);
        //            var grp = badPolls.GetGroupByRoom();

        //            foreach (KeyValuePair<string, List<BadPollEmailDTO>> entry in grp)
        //            {
        //                string[] key = entry.Key.Split('_');

        //                long roomId = Convert.ToInt64(key[0]);
        //                long companyId = Convert.ToInt64(key[1]);

        //                EVMIEmailTemplateDTO objEmail = templateDAL.GetEVMIEmailTemplateForBinNotOnSensor(roomId, companyId, badPolls.EnterpriseID);
        //                if (objEmail != null && string.IsNullOrWhiteSpace(objEmail.MailBodyText) == false)
        //                {
        //                    string strCCAddress = "";
        //                    eMailMasterDAL objUtils = new eMailMasterDAL(badPolls.EnterpriseDBName);
        //                    string emailBody = GetBinNotOnSensorEmailBody(objEmail.MailBodyText, entry.Value);

        //                    objUtils.eMailToSend(objEmail.EmailAddress, strCCAddress, objEmail.MailSubject, emailBody, badPolls.EnterpriseID
        //                        , companyId, roomId, 0);

        //                    InsertMailLog(companyId, roomId, badPolls.EnterpriseDBName, objEmail);
        //                }

        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteExceptionLog("SendBadPollEmail", ex);
        //    }
        //}
        //private static string GetBinNotOnSensorEmailBody(string bodyTemplate, List<BadPollEmailDTO> emailDTOs)
        //{
        //    string emailBody = bodyTemplate;


        //    string companyName = emailDTOs[0].CompanyName;
        //    string roomName = emailDTOs[0].RoomName;

        //    //string logo = @"<img src='https://sapphire.eturns.com/Content/images/eturnslogo.png' width='162' height='55' alt='eTurns Powered'>";
        //    //emailBody = Regex.Replace(emailBody, "@@ETURNSLOGO@@", logo, RegexOptions.IgnoreCase);
        //    emailBody = Regex.Replace(emailBody, "@@COMPANYNAME@@", companyName, RegexOptions.IgnoreCase);
        //    emailBody = Regex.Replace(emailBody, "@@ROOMNAME@@", roomName, RegexOptions.IgnoreCase);
        //    //emailBody = Regex.Replace(emailBody, "@@MESSAGE@@", "eVMI poll has below errors: ", RegexOptions.IgnoreCase);
        //    emailBody = Regex.Replace(emailBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

        //    StringBuilder sbTable = new StringBuilder();

        //    string tdStyle = "border:1px solid #d0d0d0;padding:2px;";
        //    sbTable.Append(@"<table style='border: 1px solid #d0d0d0' cellpadding='2' cellspacing='0' style='width:98%;'>");
        //    sbTable.Append(@"<tr style='background-color:#dfdfdf'>");
        //    sbTable.Append(@"<th style='width:5%;" + tdStyle + "'>#</th>");
        //    sbTable.Append(@"<th style='width:19%;" + tdStyle + "'>Item #</th>");
        //    sbTable.Append(@"<th style='width:19%;" + tdStyle + "'>Bin #</th>");
        //    sbTable.Append(@"<th style='width:8%;" + tdStyle + "'>Sensor Id</th>");
        //    sbTable.Append(@"<th style='width:8%;" + tdStyle + "'>Sensor Weight</th>");
        //    sbTable.Append(@"<th style='width:8%;" + tdStyle + "'>Weight Per Piece</th>");
        //    sbTable.Append(@"<th style='width:8%;" + tdStyle + "'>Item Count</th>");
        //    sbTable.Append(@"<th  style='width:8%;" + tdStyle + "'>Reason</th>");
        //    sbTable.Append(@"</tr>");
        //    for (int i = 0; i < emailDTOs.Count; i++)
        //    {
        //        BadPollEmailDTO dto = emailDTOs[i];

        //        sbTable.Append(@"<tr>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + (i + 1).ToString() + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.ItemNumber + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.BinNumber + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.SensorId + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.WeightReading.ToString("0.000") + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.WeightPerPiece.ToString("0.000") + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.ItemCount + "</td>");
        //        var badPoll = dto.BadPollType.Where(p => p.Key == BadPollTypeEnum.BinNotOnSensor).First();
        //        string error = badPoll.Value;
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + error + "</td>");
        //        sbTable.Append(@"</tr>");
        //    }
        //    sbTable.Append(@"</table>");
        //    emailBody = Regex.Replace(emailBody, "@@TABLE@@", sbTable.ToString(), RegexOptions.IgnoreCase);


        //    return emailBody;
        //}

        //private static void SendWrongItemOnBinEmail(PollEmailDTOCollection<BadPollEmailDTO> badPolls)
        //{
        //    try
        //    {
        //        if (badPolls != null && badPolls.Count > 0)
        //        {

        //            EmailTemplateDAL templateDAL = new EmailTemplateDAL(badPolls.EnterpriseDBName);
        //            var grp = badPolls.GetGroupByRoom();

        //            foreach (KeyValuePair<string, List<BadPollEmailDTO>> entry in grp)
        //            {
        //                string[] key = entry.Key.Split('_');

        //                long roomId = Convert.ToInt64(key[0]);
        //                long companyId = Convert.ToInt64(key[1]);

        //                EVMIEmailTemplateDTO objEmail = templateDAL.GetEVMIEmailTemplateForComPortError(roomId, companyId, badPolls.EnterpriseID);
        //                if (objEmail != null && string.IsNullOrWhiteSpace(objEmail.MailBodyText) == false)
        //                {
        //                    string strCCAddress = "";
        //                    eMailMasterDAL objUtils = new eMailMasterDAL(badPolls.EnterpriseDBName);
        //                    string emailBody = GetWrongItemOnBinEmailBody(objEmail.MailBodyText, entry.Value);

        //                    objUtils.eMailToSend(objEmail.EmailAddress, strCCAddress, objEmail.MailSubject, emailBody, badPolls.EnterpriseID
        //                        , companyId, roomId, 0);

        //                    InsertMailLog(companyId, roomId, badPolls.EnterpriseDBName, objEmail);
        //                }

        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteExceptionLog("SendBadPollEmail", ex);
        //    }
        //}
        //private static string GetWrongItemOnBinEmailBody(string bodyTemplate, List<BadPollEmailDTO> emailDTOs)
        //{
        //    string emailBody = bodyTemplate;


        //    string companyName = emailDTOs[0].CompanyName;
        //    string roomName = emailDTOs[0].RoomName;

        //    //string logo = @"<img src='https://sapphire.eturns.com/Content/images/eturnslogo.png' width='162' height='55' alt='eTurns Powered'>";
        //    //emailBody = Regex.Replace(emailBody, "@@ETURNSLOGO@@", logo, RegexOptions.IgnoreCase);
        //    emailBody = Regex.Replace(emailBody, "@@COMPANYNAME@@", companyName, RegexOptions.IgnoreCase);
        //    emailBody = Regex.Replace(emailBody, "@@ROOMNAME@@", roomName, RegexOptions.IgnoreCase);
        //    //emailBody = Regex.Replace(emailBody, "@@MESSAGE@@", "eVMI poll has below errors: ", RegexOptions.IgnoreCase);
        //    emailBody = Regex.Replace(emailBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

        //    StringBuilder sbTable = new StringBuilder();

        //    string tdStyle = "border:1px solid #d0d0d0;padding:2px;";
        //    sbTable.Append(@"<table style='border: 1px solid #d0d0d0' cellpadding='2' cellspacing='0' style='width:98%;'>");
        //    sbTable.Append(@"<tr style='background-color:#dfdfdf'>");
        //    sbTable.Append(@"<th style='width:5%;" + tdStyle + "'>#</th>");
        //    sbTable.Append(@"<th style='width:19%;" + tdStyle + "'>Item #</th>");
        //    sbTable.Append(@"<th style='width:19%;" + tdStyle + "'>Bin #</th>");
        //    sbTable.Append(@"<th style='width:8%;" + tdStyle + "'>Sensor Id</th>");
        //    sbTable.Append(@"<th style='width:8%;" + tdStyle + "'>Sensor Weight</th>");
        //    sbTable.Append(@"<th style='width:8%;" + tdStyle + "'>Weight Per Piece</th>");
        //    sbTable.Append(@"<th style='width:8%;" + tdStyle + "'>Item Count</th>");
        //    sbTable.Append(@"<th  style='width:8%;" + tdStyle + "'>Reason</th>");
        //    sbTable.Append(@"</tr>");
        //    for (int i = 0; i < emailDTOs.Count; i++)
        //    {
        //        BadPollEmailDTO dto = emailDTOs[i];

        //        sbTable.Append(@"<tr>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + (i + 1).ToString() + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.ItemNumber + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.BinNumber + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.SensorId + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.WeightReading.ToString("0.000") + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.WeightPerPiece.ToString("0.000") + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.ItemCount + "</td>");
        //        var badPoll = dto.BadPollType.Where(p => p.Key == BadPollTypeEnum.BinNotOnSensor).First();
        //        string error = badPoll.Value;
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + error + "</td>");
        //        sbTable.Append(@"</tr>");
        //    }
        //    sbTable.Append(@"</table>");
        //    emailBody = Regex.Replace(emailBody, "@@TABLE@@", sbTable.ToString(), RegexOptions.IgnoreCase);


        //    return emailBody;
        //}

        #endregion

        #region Send Poll Done Email
        /// <summary>
        /// Send Poll Done Email
        /// </summary>
        /// <param name="polls"></param>
        public static void SendPollDoneEmail(PollEmailDTOCollection<PollDoneEmailDTO> polls)
        {
            try
            {
                if (polls.Count > 0)
                {

                    EmailTemplateDAL templateDAL = new EmailTemplateDAL(polls.EnterpriseDBName);

                    var grp = polls.GetGroupByRoom();

                    foreach (KeyValuePair<string, List<PollDoneEmailDTO>> entry in grp)
                    {
                        string[] key = entry.Key.Split('_');

                        long roomId = Convert.ToInt64(key[0]);
                        long companyId = Convert.ToInt64(key[1]);


                        EVMIEmailTemplateDTO objEmail = templateDAL.GetEVMIEmailTemplateForPollDone(roomId, companyId, polls.EnterpriseID);
                        if (objEmail != null && string.IsNullOrWhiteSpace(objEmail.MailBodyText) == false)
                        {
                            string strCCAddress = "";
                            eMailMasterDAL objUtils = new eMailMasterDAL(polls.EnterpriseDBName);
                            string emailBody = GetPollDoneEmailBody(objEmail.MailBodyText, entry.Value);

                            objUtils.eMailToSend(objEmail.EmailAddress, strCCAddress, objEmail.MailSubject, emailBody, polls.EnterpriseID
                                , companyId, roomId, 0);

                            InsertMailLog(companyId, roomId, polls.EnterpriseDBName, objEmail);
                        }


                    }

                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("SendPollDoneEmail", ex);
            }
        }
        private static string GetPollDoneEmailBody(string bodyTemplate, List<PollDoneEmailDTO> emailDTOs)
        {
            string emailBody = bodyTemplate;

            string companyName = emailDTOs[0].CompanyName;
            string roomName = emailDTOs[0].RoomName;
            //string logo = @"<img src='https://sapphire.eturns.com/Content/images/eturnslogo.png' width='162' height='55' alt='eTurns Powered'>";
            // <img alt="eTurns Powered" src="https://www.eturns.com/images/eturns_logo.png" style="height:55px; width:162px" />
            //emailBody = Regex.Replace(emailBody, "@@ETURNSLOGO@@", logo, RegexOptions.IgnoreCase);
            emailBody = Regex.Replace(emailBody, "@@COMPANYNAME@@", companyName, RegexOptions.IgnoreCase);
            emailBody = Regex.Replace(emailBody, "@@ROOMNAME@@", roomName, RegexOptions.IgnoreCase);
            emailBody = Regex.Replace(emailBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

            StringBuilder sbTable = new StringBuilder();

            string tdStyle = "border:1px solid #d0d0d0;padding:2px;";
            string tdErrorStyle = "color:#FF0000;";
            sbTable.Append(@"<div>");
            sbTable.Append(@"    <span style='font-weight:bold;'>Poll - Total Records : " + emailDTOs.Count + " </span>, ");
            sbTable.Append(@"    <span style='font-weight:bold;'>Success Count : </span><span style='font-weight:bold;color:#228B22'>" + emailDTOs.Where(s => s.IsError == false).Count() + " </span>, ");
            sbTable.Append(@"    <span style='font-weight:bold;'>Fail Count: </span><span style='font-weight:bold;" + tdErrorStyle + "'>" + emailDTOs.Where(s => s.IsError == true).Count() + " </span>");
            sbTable.Append(@"</div><br/>");
            sbTable.Append(@"<table style='border: 1px solid #d0d0d0' cellpadding='2' cellspacing='0' style='width:98%;'>");
            sbTable.Append(@"<tr style='background-color:#dfdfdf'>");
            sbTable.Append(@"   <th style='width:5%;" + tdStyle + "'>#</th>");
            sbTable.Append(@"   <th style='width:15%;" + tdStyle + "'>Item #</th>");
            sbTable.Append(@"   <th style='width:15%;" + tdStyle + "'>Bin #</th>");
            sbTable.Append(@"   <th style='width:6%;" + tdStyle + "'>Sensor Id</th>");
            sbTable.Append(@"   <th style='width:7%;" + tdStyle + "'>Sensor Weight</th>");
            sbTable.Append(@"   <th style='width:7%;" + tdStyle + "'>Weight Per Piece</th>");
            sbTable.Append(@"   <th style='width:7%;" + tdStyle + "'>New Quantity</th>");
            sbTable.Append(@"   <th style='width:7%;" + tdStyle + "'>Poll Action</th>");
            sbTable.Append(@"   <th style='width:7%;" + tdStyle + "'>Status</th>");
            sbTable.Append(@"   <th style='" + tdStyle + "'>Fail Reason</th>");
            sbTable.Append(@"</tr>");

            for (int i = 0; i < emailDTOs.Count; i++)
            {
                PollDoneEmailDTO dto = emailDTOs[i];

                sbTable.Append(@"<tr style='" + (dto.IsError ? tdErrorStyle : "") + "' >");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + (i + 1).ToString() + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.ItemNumber + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.BinNumber + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.SensorId + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.WeightReading.ToString("0.000") + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.WeightPerPiece.ToString("0.000") + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.NewQuantity + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.PollAction + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.Status + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + (string.IsNullOrWhiteSpace(dto.ErrorDescription) ? "&nbsp;" : dto.ErrorDescription) + "</td>");
                sbTable.Append(@"</tr>");
            }
            sbTable.Append(@"</table>");
            emailBody = Regex.Replace(emailBody, "@@TABLE@@", sbTable.ToString(), RegexOptions.IgnoreCase);

            return emailBody;
        }

        #endregion

        #region Send Tare Done Email
        /// <summary>
        /// Send Tare Done Email
        /// </summary>
        /// <param name="polls"></param>
        public static void SendTareDoneEmail(PollEmailDTOCollection<TareDoneEmailDTO> polls)
        {
            try
            {
                if (polls.Count > 0)
                {
                    EmailTemplateDAL templateDAL = new EmailTemplateDAL(polls.EnterpriseDBName);

                    var grp = polls.GetGroupByRoom();

                    foreach (KeyValuePair<string, List<TareDoneEmailDTO>> entry in grp)
                    {
                        string[] key = entry.Key.Split('_');

                        long roomId = Convert.ToInt64(key[0]);
                        long companyId = Convert.ToInt64(key[1]);

                        EVMIEmailTemplateDTO objEmail = templateDAL.GetEVMIEmailTemplateForTareDone(roomId, companyId, polls.EnterpriseID);
                        if (objEmail != null && string.IsNullOrWhiteSpace(objEmail.MailBodyText) == false)
                        {
                            string strCCAddress = "";
                            eMailMasterDAL objUtils = new eMailMasterDAL(polls.EnterpriseDBName);
                            string emailBody = GetTareDoneEmailBody(objEmail.MailBodyText, entry.Value);

                            objUtils.eMailToSend(objEmail.EmailAddress, strCCAddress, objEmail.MailSubject, emailBody, polls.EnterpriseID
                                , companyId, roomId, 0);

                            InsertMailLog(companyId, roomId, polls.EnterpriseDBName, objEmail);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("SendTareDoneEmail", ex);
            }


        }

        private static string GetTareDoneEmailBody(string bodyTemplate, List<TareDoneEmailDTO> emailDTOs)
        {
            string emailBody = bodyTemplate;

            string companyName = emailDTOs[0].CompanyName;
            string roomName = emailDTOs[0].RoomName;

            emailBody = Regex.Replace(emailBody, "@@COMPANYNAME@@", companyName, RegexOptions.IgnoreCase);
            emailBody = Regex.Replace(emailBody, "@@ROOMNAME@@", roomName, RegexOptions.IgnoreCase);
            emailBody = Regex.Replace(emailBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

            StringBuilder sbTable = new StringBuilder();

            string tdStyle = "border:1px solid #d0d0d0;padding:2px;";
            string tdErrorStyle = "color:#FF0000;";
            sbTable.Append(@"<div>");
            sbTable.Append(@"    <span style='font-weight:bold;'>Tare - Total Records : " + emailDTOs.Count + " </span>, ");
            sbTable.Append(@"    <span style='font-weight:bold;'>Success Count : </span><span style='font-weight:bold;color:#228B22'>" + emailDTOs.Where(s => s.IsError == false).Count() + " </span>, ");
            sbTable.Append(@"    <span style='font-weight:bold;'>Fail Count: </span><span style='font-weight:bold;" + tdErrorStyle + "'>" + emailDTOs.Where(s => s.IsError == true).Count() + " </span>");
            sbTable.Append(@"</div><br/>");

            sbTable.Append(@"<table style='border: 1px solid #d0d0d0' cellpadding='2' cellspacing='0' style='width:90%;'>");
            sbTable.Append(@"<tr style='background-color:#dfdfdf'>");
            sbTable.Append(@"   <th style='width:5%;" + tdStyle + "'>#</th>");
            sbTable.Append(@"   <th style='width:20%;" + tdStyle + "'>Item #</th>");
            sbTable.Append(@"   <th style='width:20%;" + tdStyle + "'>Bin #</th>");
            sbTable.Append(@"   <th style='width:10%;" + tdStyle + "'>Sensor Id</th>");
            sbTable.Append(@"   <th style='width:10%;" + tdStyle + "'>Status</th>");
            sbTable.Append(@"   <th style='" + tdStyle + "'>Fail Reason</th>");
            sbTable.Append(@"</tr>");

            for (int i = 0; i < emailDTOs.Count; i++)
            {
                TareDoneEmailDTO dto = emailDTOs[i];

                sbTable.Append(@"<tr style='" + (dto.IsError ? tdErrorStyle : "") + "' >");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + (i + 1).ToString() + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.ItemNumber + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.BinNumber + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.SensorId + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.Status + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + (string.IsNullOrWhiteSpace(dto.ErrorDescription) ? "&nbsp;" : dto.ErrorDescription) + "</td>");
                sbTable.Append(@"</tr>");
            }
            sbTable.Append(@"</table>");
            emailBody = Regex.Replace(emailBody, "@@TABLE@@", sbTable.ToString(), RegexOptions.IgnoreCase);

            return emailBody;
        }

        #endregion

        #region Send Tare Done Email

        internal static void SendResetDoneEmail(PollEmailDTOCollection<ResetDoneEmailDTO> polls)
        {

            try
            {
                if (polls.Count > 0)
                {
                    EmailTemplateDAL templateDAL = new EmailTemplateDAL(polls.EnterpriseDBName);

                    var grp = polls.GetGroupByRoom();

                    foreach (KeyValuePair<string, List<ResetDoneEmailDTO>> entry in grp)
                    {
                        string[] key = entry.Key.Split('_');

                        long roomId = Convert.ToInt64(key[0]);
                        long companyId = Convert.ToInt64(key[1]);

                        EVMIEmailTemplateDTO objEmail = templateDAL.GetEVMIEmailTemplateForResetDone(roomId, companyId, polls.EnterpriseID);
                        if (objEmail != null && string.IsNullOrWhiteSpace(objEmail.MailBodyText) == false)
                        {
                            string strCCAddress = "";
                            eMailMasterDAL objUtils = new eMailMasterDAL(polls.EnterpriseDBName);
                            string emailBody = GetResetDoneEmailBody(objEmail.MailBodyText, entry.Value);

                            objUtils.eMailToSend(objEmail.EmailAddress, strCCAddress, objEmail.MailSubject, emailBody, polls.EnterpriseID
                                , companyId, roomId, 0);

                            InsertMailLog(companyId, roomId, polls.EnterpriseDBName, objEmail);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("SendResetDoneEmail", ex);
            }


        }

        private static string GetResetDoneEmailBody(string bodyTemplate, List<ResetDoneEmailDTO> emailDTOs)
        {
            string emailBody = bodyTemplate;

            string companyName = emailDTOs[0].CompanyName;
            string roomName = emailDTOs[0].RoomName;

            emailBody = Regex.Replace(emailBody, "@@COMPANYNAME@@", companyName, RegexOptions.IgnoreCase);
            emailBody = Regex.Replace(emailBody, "@@ROOMNAME@@", roomName, RegexOptions.IgnoreCase);
            emailBody = Regex.Replace(emailBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

            StringBuilder sbTable = new StringBuilder();

            string tdStyle = "border:1px solid #d0d0d0;padding:2px;";
            string tdErrorStyle = "color:#FF0000;";
            sbTable.Append(@"<div>");
            sbTable.Append(@"    <span style='font-weight:bold;'>Reset Shelf - Total Records : " + emailDTOs.Count + " </span>, ");
            sbTable.Append(@"    <span style='font-weight:bold;'>Success Count : </span><span style='font-weight:bold;color:#228B22'>" + emailDTOs.Where(s => s.IsError == false).Count() + " </span>, ");
            sbTable.Append(@"    <span style='font-weight:bold;'>Fail Count: </span><span style='font-weight:bold;" + tdErrorStyle + "'>" + emailDTOs.Where(s => s.IsError == true).Count() + " </span>");
            sbTable.Append(@"</div><br/>");

            sbTable.Append(@"<table style='border: 1px solid #d0d0d0' cellpadding='2' cellspacing='0' style='width:90%;'>");
            sbTable.Append(@"<tr style='background-color:#dfdfdf'>");
            sbTable.Append(@"   <th style='width:5%;" + tdStyle + "'>#</th>");
            sbTable.Append(@"   <th style='width:20%;" + tdStyle + "'>Item #</th>");
            sbTable.Append(@"   <th style='width:20%;" + tdStyle + "'>Bin #</th>");
            sbTable.Append(@"   <th style='width:10%;" + tdStyle + "'>Sensor Id</th>");
            sbTable.Append(@"   <th style='width:10%;" + tdStyle + "'>Status</th>");
            sbTable.Append(@"   <th style='" + tdStyle + "'>Fail Reason</th>");
            sbTable.Append(@"</tr>");

            for (int i = 0; i < emailDTOs.Count; i++)
            {
                ResetDoneEmailDTO dto = emailDTOs[i];

                sbTable.Append(@"<tr style='" + (dto.IsError ? tdErrorStyle : "") + "' >");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + (i + 1).ToString() + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.ItemNumber + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.BinNumber + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.SensorId + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.Status + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + (string.IsNullOrWhiteSpace(dto.ErrorDescription) ? "&nbsp;" : dto.ErrorDescription) + "</td>");
                sbTable.Append(@"</tr>");
            }
            sbTable.Append(@"</table>");
            emailBody = Regex.Replace(emailBody, "@@TABLE@@", sbTable.ToString(), RegexOptions.IgnoreCase);

            return emailBody;
        }

        #endregion

        #region Send Calibrate Done Email
        internal static void SendCalibrateDoneEmail(PollEmailDTOCollection<CalibrateDoneEmail> polls)
        {
            try
            {
                if (polls.Count > 0)
                {
                    EmailTemplateDAL templateDAL = new EmailTemplateDAL(polls.EnterpriseDBName);

                    var grp = polls.GetGroupByRoom();

                    foreach (KeyValuePair<string, List<CalibrateDoneEmail>> entry in grp)
                    {
                        string[] key = entry.Key.Split('_');

                        long roomId = Convert.ToInt64(key[0]);
                        long companyId = Convert.ToInt64(key[1]);

                        EVMIEmailTemplateDTO objEmail = templateDAL.GetEVMIEmailTemplateForCalibrateDone(roomId, companyId, polls.EnterpriseID);
                        if (objEmail != null && string.IsNullOrWhiteSpace(objEmail.MailBodyText) == false)
                        {
                            string strCCAddress = "";
                            eMailMasterDAL objUtils = new eMailMasterDAL(polls.EnterpriseDBName);
                            string emailBody = GetCalibrateDoneEmailBody(objEmail.MailBodyText, entry.Value);

                            objUtils.eMailToSend(objEmail.EmailAddress, strCCAddress, objEmail.MailSubject, emailBody, polls.EnterpriseID
                                , companyId, roomId, 0);

                            InsertMailLog(companyId, roomId, polls.EnterpriseDBName, objEmail);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("SendCalibrateDoneEmail", ex);
            }


        }

        private static string GetCalibrateDoneEmailBody(string bodyTemplate, List<CalibrateDoneEmail> emailDTOs)
        {
            string emailBody = bodyTemplate;

            string companyName = emailDTOs[0].CompanyName;
            string roomName = emailDTOs[0].RoomName;
            string stepName = "";

            if (emailDTOs[0].StepNo == CalibrateStepEnum.Step1)
            {
                stepName = "Calibrate Step 1";
            }
            else if (emailDTOs[0].StepNo == CalibrateStepEnum.Step2)
            {
                stepName = "Calibrate Step 2";
            }
            else if (emailDTOs[0].StepNo == CalibrateStepEnum.Step3)
            {
                stepName = "Calibrate Step 3";
            }



            emailBody = Regex.Replace(emailBody, "@@COMPANYNAME@@", companyName, RegexOptions.IgnoreCase);
            emailBody = Regex.Replace(emailBody, "@@ROOMNAME@@", roomName, RegexOptions.IgnoreCase);
            emailBody = Regex.Replace(emailBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

            StringBuilder sbTable = new StringBuilder();

            string tdStyle = "border:1px solid #d0d0d0;padding:2px;";
            string tdErrorStyle = ErrorStyle;// "color:#FF0000;";


            sbTable.Append(@"<div>");
            sbTable.Append(@"    <span style='font-weight:bold;'>" + stepName + " - Total Records : " + emailDTOs.Count + " </span>, ");
            sbTable.Append(@"    <span style='font-weight:bold;'>Success Count : </span><span style='font-weight:bold;" + SuccessStyle + "'>" + emailDTOs.Where(s => s.IsError == false).Count() + " </span>, ");
            sbTable.Append(@"    <span style='font-weight:bold;'>Fail Count: </span><span style='font-weight:bold;" + tdErrorStyle + "'>" + emailDTOs.Where(s => s.IsError == true).Count() + " </span>");
            sbTable.Append(@"</div><br/>");

            sbTable.Append(@"<table style='border: 1px solid #d0d0d0' cellpadding='2' cellspacing='0' style='width:90%;'>");
            sbTable.Append(@"<tr style='background-color:#dfdfdf'>");
            sbTable.Append(@"   <th style='width:5%;" + tdStyle + "'>#</th>");
            sbTable.Append(@"   <th style='width:20%;" + tdStyle + "'>Item #</th>");
            sbTable.Append(@"   <th style='width:20%;" + tdStyle + "'>Bin #</th>");
            sbTable.Append(@"   <th style='width:10%;" + tdStyle + "'>Sensor Id</th>");
            sbTable.Append(@"   <th style='width:10%;" + tdStyle + "'>Status</th>");
            sbTable.Append(@"   <th style='" + tdStyle + "'>Fail Reason</th>");
            sbTable.Append(@"</tr>");

            for (int i = 0; i < emailDTOs.Count; i++)
            {
                CalibrateDoneEmail dto = emailDTOs[i];

                sbTable.Append(@"<tr style='" + (dto.IsError ? tdErrorStyle : "") + "' >");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + (i + 1).ToString() + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.ItemNumber + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.BinNumber + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.SensorId + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.Status + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + (string.IsNullOrWhiteSpace(dto.ErrorDescription) ? "&nbsp;" : dto.ErrorDescription) + "</td>");
                sbTable.Append(@"</tr>");
            }
            sbTable.Append(@"</table>");
            emailBody = Regex.Replace(emailBody, "@@TABLE@@", sbTable.ToString(), RegexOptions.IgnoreCase);

            return emailBody;
        }

        #endregion

        #region Send Weight Per Piece Done Email
        /// <summary>
        /// Send Tare Done Email
        /// </summary>
        /// <param name="polls"></param>
        public static void SendWeightPerPieceDoneEmail(PollEmailDTOCollection<ItemWeightPerPieceDoneEmailDTO> polls)
        {
            try
            {
                if (polls.Count > 0)
                {
                    EmailTemplateDAL templateDAL = new EmailTemplateDAL(polls.EnterpriseDBName);

                    var grp = polls.GetGroupByRoom();

                    foreach (KeyValuePair<string, List<ItemWeightPerPieceDoneEmailDTO>> entry in grp)
                    {
                        string[] key = entry.Key.Split('_');

                        long roomId = Convert.ToInt64(key[0]);
                        long companyId = Convert.ToInt64(key[1]);

                        EVMIEmailTemplateDTO objEmail = templateDAL.GetEVMIEmailTemplateForWeightPerPieceDone(roomId, companyId, polls.EnterpriseID);
                        if (objEmail != null && string.IsNullOrWhiteSpace(objEmail.MailBodyText) == false)
                        {
                            string strCCAddress = "";
                            eMailMasterDAL objUtils = new eMailMasterDAL(polls.EnterpriseDBName);
                            string emailBody = GetWeightPerPieceDoneEmailBody(objEmail.MailBodyText, entry.Value);

                            objUtils.eMailToSend(objEmail.EmailAddress, strCCAddress, objEmail.MailSubject, emailBody, polls.EnterpriseID
                                , companyId, roomId, 0);

                            InsertMailLog(companyId, roomId, polls.EnterpriseDBName, objEmail);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("SendWeightPerPieceDoneEmail", ex);
            }


        }

        private static string GetWeightPerPieceDoneEmailBody(string bodyTemplate, List<ItemWeightPerPieceDoneEmailDTO> emailDTOs)
        {
            string emailBody = bodyTemplate;

            string companyName = emailDTOs[0].CompanyName;
            string roomName = emailDTOs[0].RoomName;

            emailBody = Regex.Replace(emailBody, "@@COMPANYNAME@@", companyName, RegexOptions.IgnoreCase);
            emailBody = Regex.Replace(emailBody, "@@ROOMNAME@@", roomName, RegexOptions.IgnoreCase);
            emailBody = Regex.Replace(emailBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

            StringBuilder sbTable = new StringBuilder();

            string tdStyle = "border:1px solid #d0d0d0;padding:2px;";
            string tdErrorStyle = "color:#FF0000;";
            sbTable.Append(@"<div>");
            sbTable.Append(@"    <span style='font-weight:bold;'>Total Records : " + emailDTOs.Count + " </span>, ");
            sbTable.Append(@"    <span style='font-weight:bold;'>Success Count : </span><span style='font-weight:bold;color:#228B22'>" + emailDTOs.Where(s => s.IsError == false).Count() + " </span>, ");
            sbTable.Append(@"    <span style='font-weight:bold;'>Fail Count: </span><span style='font-weight:bold;" + tdErrorStyle + "'>" + emailDTOs.Where(s => s.IsError == true).Count() + " </span>");
            sbTable.Append(@"</div><br/>");
            sbTable.Append(@"<table style='border: 1px solid #d0d0d0' cellpadding='2' cellspacing='0' style='width:90%;'>");
            sbTable.Append(@"   <tr style='background-color:#dfdfdf'>");
            sbTable.Append(@"   <th style='width:5%;" + tdStyle + "'>#</th>");
            sbTable.Append(@"   <th style='width:20%;" + tdStyle + "'>Item #</th>");
            sbTable.Append(@"   <th style='width:20%;" + tdStyle + "'>Bin #</th>");
            sbTable.Append(@"   <th style='width:10%;" + tdStyle + "'>Sensor Id</th>");
            sbTable.Append(@"   <th style='width:12%;" + tdStyle + "'>Sensor Weight</th>");
            sbTable.Append(@"   <th style='width:10%;" + tdStyle + "'>Status</th>");
            sbTable.Append(@"   <th style='" + tdStyle + "'>Fail Reason</th>");
            sbTable.Append(@"</tr>");

            for (int i = 0; i < emailDTOs.Count; i++)
            {
                ItemWeightPerPieceDoneEmailDTO dto = emailDTOs[i];

                sbTable.Append(@"<tr style='" + (dto.IsError ? tdErrorStyle : "") + "' >");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + (i + 1).ToString() + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.ItemNumber + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.BinNumber + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.SensorId + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.WeightReading + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + dto.Status + "</td>");
                sbTable.Append(@"   <td style='" + tdStyle + "'>" + (string.IsNullOrWhiteSpace(dto.ErrorDescription) ? "&nbsp;" : dto.ErrorDescription) + "</td>");
                sbTable.Append(@"</tr>");
            }
            sbTable.Append(@"</table>");
            emailBody = Regex.Replace(emailBody, "@@TABLE@@", sbTable.ToString(), RegexOptions.IgnoreCase);


            return emailBody;
        }

        #endregion

        #region Send Get Or Set ShelfID Done Email
        internal static void SendGetOrSetShelfIDDoneEmail(ShelfIDEmailDTO shelfEmailDTO)
        {
            try
            {
                long roomId = shelfEmailDTO.RoomID;
                long companyId = shelfEmailDTO.CompanyID;
                EmailTemplateDAL templateDAL = new EmailTemplateDAL(shelfEmailDTO.EnterpriseDBName);
                EVMIEmailTemplateDTO objEmail = templateDAL.GetEVMIEmailTemplateShelfDone(roomId, companyId, shelfEmailDTO.EnterpriseID);
                if (objEmail != null && string.IsNullOrWhiteSpace(objEmail.MailBodyText) == false)
                {
                    string strCCAddress = "";
                    eMailMasterDAL objUtils = new eMailMasterDAL(shelfEmailDTO.EnterpriseDBName);

                    string emailBody = GetShelfIDDoneEmailEmailBody(objEmail.MailBodyText, shelfEmailDTO);

                    objUtils.eMailToSend(objEmail.EmailAddress, strCCAddress, objEmail.MailSubject, emailBody, shelfEmailDTO.EnterpriseID
                        , companyId, roomId, 0);

                    InsertMailLog(companyId, roomId, shelfEmailDTO.EnterpriseDBName, objEmail);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("SendGetOrSetShelfIDDoneEmail", ex);
            }
        }

        private static string GetShelfIDDoneEmailEmailBody(string bodyTemplate, ShelfIDEmailDTO emailDTO)
        {
            string emailBody = bodyTemplate;

            string companyName = emailDTO.CompanyName;
            string roomName = emailDTO.RoomName;

            emailBody = Regex.Replace(emailBody, "@@COMPANYNAME@@", companyName, RegexOptions.IgnoreCase);
            emailBody = Regex.Replace(emailBody, "@@ROOMNAME@@", roomName, RegexOptions.IgnoreCase);
            emailBody = Regex.Replace(emailBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
            string message = "";

            message += "<span>";
            if (emailDTO.IsError == false)
            {
                if (emailDTO.ShelfRequestType == eVMIShelfRequestType.SetShelfID)
                {
                    message += "<span style='" + SuccessStyle + "'>eVMI request to SET Shelf ID = " + emailDTO.ScaleID + " is processed successfully.</span>";
                }
                else if (emailDTO.ShelfRequestType == eVMIShelfRequestType.GetShelfID)
                {
                    message += "<span style='" + SuccessStyle + "'>eVMI request to GET Shelf ID = " + emailDTO.ScaleID + " is processed successfully.</span>";
                }
            }
            else
            {
                if (emailDTO.ShelfRequestType == eVMIShelfRequestType.SetShelfID)
                {
                    message += "<span style='" + ErrorStyle + "'>eVMI request to SET Shelf ID = " + emailDTO.ScaleID + " is processed with error.</span>";
                }
                else if (emailDTO.ShelfRequestType == eVMIShelfRequestType.GetShelfID)
                {
                    message += "<span style='" + ErrorStyle + "'>eVMI request to GET Shelf ID = " + emailDTO.ScaleID + " is processed with error.</span>";

                }

                message += "<br/> <span style='" + ErrorStyle + "'> Error : " + emailDTO.ErrorDescription + "</span>";
            }
            message += "</span>";

            emailBody = Regex.Replace(emailBody, "@@MESSAGE@@", message, RegexOptions.IgnoreCase);

            return emailBody;
        }

        #endregion

        #region Calibration weight email
        internal static void SendCalibrationGetWeightDoneEmail(PollEmailDTOCollection<GetCalibrationWeightDoneEmailDTO> emailsData)
        { 
        }

        internal static void SendCalibrationSetWeightDoneEmail(PollEmailDTOCollection<SetCalibrationWeightDoneEmailDTO> emailsData)
        {
        }

        #endregion

        internal static void SendCommonCommandDoneEmail(PollEmailDTOCollection<CommonRequestDoneEmailDTO> emailsData)
        {

        }


        //#region SendComPortErrorEmail
        //private static void SendComPortErrorEmail(PollEmailDTOCollection<ComPortEmailDTO> comErrors)
        //{
        //    try
        //    {
        //        if (comErrors != null && comErrors.Count > 0)
        //        {

        //            EmailTemplateDAL templateDAL = new EmailTemplateDAL(comErrors.EnterpriseDBName);
        //            var grp = comErrors.GetGroupByRoom();

        //            foreach (KeyValuePair<string, List<ComPortEmailDTO>> entry in grp)
        //            {
        //                string[] key = entry.Key.Split('_');

        //                long roomId = Convert.ToInt64(key[0]);
        //                long companyId = Convert.ToInt64(key[1]);

        //                EVMIEmailTemplateDTO objEmail = templateDAL.GetEVMIEmailTemplateForComPortError(roomId, companyId, comErrors.EnterpriseID);
        //                if (objEmail != null && string.IsNullOrWhiteSpace(objEmail.MailBodyText) == false)
        //                {
        //                    string strCCAddress = "";
        //                    eMailMasterDAL objUtils = new eMailMasterDAL(comErrors.EnterpriseDBName);
        //                    string emailBody = GetComPortErrorEmailBody(objEmail.MailBodyText, entry.Value);

        //                    objUtils.eMailToSend(objEmail.EmailAddress, strCCAddress, objEmail.MailSubject, emailBody, comErrors.EnterpriseID
        //                        , companyId, roomId, 0);

        //                    InsertMailLog(companyId, roomId, comErrors.EnterpriseDBName, objEmail);
        //                }

        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteExceptionLog("SendBadPollEmail", ex);
        //    }
        //}
        //private static string GetComPortErrorEmailBody(string bodyTemplate, List<ComPortEmailDTO> emailDTOs)
        //{
        //    string emailBody = bodyTemplate;

        //    string companyName = emailDTOs[0].CompanyName;
        //    string roomName = emailDTOs[0].RoomName;

        //    //string logo = @"<img src='https://sapphire.eturns.com/Content/images/eturnslogo.png' width='162' height='55' alt='eTurns Powered'>";
        //    //emailBody = Regex.Replace(emailBody, "@@ETURNSLOGO@@", logo, RegexOptions.IgnoreCase);
        //    emailBody = Regex.Replace(emailBody, "@@COMPANYNAME@@", companyName, RegexOptions.IgnoreCase);
        //    emailBody = Regex.Replace(emailBody, "@@ROOMNAME@@", roomName, RegexOptions.IgnoreCase);
        //    //emailBody = Regex.Replace(emailBody, "@@MESSAGE@@", "eVMI poll has below errors: ", RegexOptions.IgnoreCase);
        //    emailBody = Regex.Replace(emailBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

        //    StringBuilder sbTable = new StringBuilder();

        //    string tdStyle = "border:1px solid #d0d0d0;padding:2px;";
        //    sbTable.Append(@"<table style='border: 1px solid #d0d0d0' cellpadding='2' cellspacing='0' style='width:80%;'>");
        //    sbTable.Append(@"<tr style='background-color:#dfdfdf'>");
        //    sbTable.Append(@"<th style='width:10%;" + tdStyle + "'>#</th>");
        //    sbTable.Append(@"<th style='width:25%;" + tdStyle + "'>Item #</th>");
        //    sbTable.Append(@"<th style='width:25%;" + tdStyle + "'>Bin #</th>");
        //    sbTable.Append(@"<th style='width:10%;" + tdStyle + "'>Sensor Id</th>");
        //    sbTable.Append(@"<th  style='" + tdStyle + "'>Error</th>");
        //    sbTable.Append(@"</tr>");
        //    for (int i = 0; i < emailDTOs.Count; i++)
        //    {
        //        ComPortEmailDTO dto = emailDTOs[i];

        //        sbTable.Append(@"<tr>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + (i + 1).ToString() + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.ItemNumber + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.BinNumber + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.SensorId + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto + "</td>");
        //        sbTable.Append(@"<td style='" + tdStyle + "'>" + dto.Error + "</td>");
        //        sbTable.Append(@"</tr>");
        //    }
        //    sbTable.Append(@"</table>");
        //    emailBody = Regex.Replace(emailBody, "@@TABLE@@", sbTable.ToString(), RegexOptions.IgnoreCase);


        //    return emailBody;
        //}

        //#endregion
        private static void InsertMailLog(long companyId, long roomId, string EnterpriseDBName, EVMIEmailTemplateDTO objEmail)
        {

            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(EnterpriseDBName);
            ReportMailLogDTO objReportMailLogDTO = new ReportMailLogDTO()
            {
                AttachmentCount = 0,
                CompanyID = companyId,
                NotificationID = objEmail.NotificationID,
                RoomID = roomId,
                ScheduleID = objEmail.RoomScheduleID,
                SendEmailAddress = objEmail.EmailAddress,
                SendDate = DateTime.UtcNow,
                ReportID = 0,
                Id = 0
            };

            objReportMasterDAL.InsertMailLog(objReportMailLogDTO);
        }



    } // class

}
