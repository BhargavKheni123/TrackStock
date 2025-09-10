using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
namespace eTurnsMaster.DAL
{
    public class eMailDAL
    {
        string strPath = System.Configuration.ConfigurationManager.AppSettings["DomainName"];

        /// <summary>
        /// GetEmailBodyWithReplaceTag
        /// </summary>
        /// <param name="Body"></param>
        /// <param name="EntID"></param>
        /// <param name="CompanyName"></param>
        /// <param name="RoomName"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private string GetEmailBodyWithReplaceTag(string Body, Int64 EntID, string CompanyName, string RoomName, string UserName)
        {
            if (EntID > 0 && Body.IndexOf("@@ETURNSLOGO@@") > 0)
            {
                EnterpriseMasterDAL objENTDAL = new EnterpriseMasterDAL();

                EnterpriseDTO objEntDTO = objENTDAL.GetEnterprise(EntID);
                if (objEntDTO != null && objEntDTO.EnterpriseLogo.Trim().Length > 0)
                {
                    Body = Body.Replace("@@ETURNSLOGO@@", GetImage(strPath, "Uploads/EnterpriseLogos/" + EntID.ToString() + "/" + objEntDTO.EnterpriseLogo));
                }
                else
                {
                    Body = Body.Replace("@@ETURNSLOGO@@", GetImage(strPath, "Content/images/logo.jpg"));
                }
            }
            else if (Body.IndexOf("@@ETURNSLOGO@@") > 0)
            {
                Body = Body.Replace("@@ETURNSLOGO@@", GetImage(strPath, "Content/images/logo.jpg"));
            }

            Body = Body.Replace("@@Year@@", DateTime.UtcNow.Year.ToString());
            if (!string.IsNullOrEmpty(CompanyName) && Body.IndexOf("@@COMPANYNAME@@") > 0)
            {
                Body = Body.Replace("@@COMPANYNAME@@", CompanyName);
            }

            if (!string.IsNullOrEmpty(RoomName) && Body.IndexOf("@@ROOMNAME@@") > 0)
            {
                Body = Body.Replace("@@ROOMNAME@@", RoomName);
            }

            if (!string.IsNullOrEmpty(UserName) && Body.IndexOf("@@USERNAME@@") > 0)
            {
                Body = Body.Replace("@@USERNAME@@", UserName);
            }

            Body = Body.Replace("@@Year@@", "");
            Body = Body.Replace("@@COMPANYNAME@@", "");
            Body = Body.Replace("@@ROOMNAME@@", "");
            Body = Body.Replace("@@USERNAME@@", "");
            return Body;
        }

        /// <summary>
        /// eMailToSend
        /// </summary>
        /// <param name="ToEmailID"></param>
        /// <param name="CCEmailID"></param>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>
        /// <param name="EntID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="RoomID"></param>
        /// <param name="UserID"></param>
        /// <param name="_Attachments"></param>
        /// <param name="Remarks"></param>
        public void eMailToSend(string ToEmailID, string CCEmailID, string Subject, string Body, Int64 EntID, Int64 CompanyID, Int64 RoomID, Int64 UserID, List<eMailAttachmentDTO> _Attachments = null, string Remarks = "")
        {
            eMailToSendDTO objMailToSend = null;
            EnterpriseDTO objEntDTO = null;
            EnterpriseMasterDAL objENTDAL = null;
            try
            {

                if (EntID > 0 && Body.IndexOf("@@ETURNSLOGO@@") > 0)
                {
                    objENTDAL = new EnterpriseMasterDAL();

                    objEntDTO = objENTDAL.GetEnterprise(EntID);
                    if (objEntDTO != null && !string.IsNullOrWhiteSpace(objEntDTO.EnterpriseLogo) && objEntDTO.EnterpriseLogo.Trim().Length > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(objEntDTO.EnterPriseDomainURL))
                        {
                            strPath = objEntDTO.EnterPriseDomainURL;
                        }
                        Body = Body.Replace("@@ETURNSLOGO@@", GetImage(strPath, "Uploads/EnterpriseLogos/" + EntID.ToString() + "/" + objEntDTO.EnterpriseLogo));
                    }
                    else
                    {
                        if (objEntDTO != null && (!string.IsNullOrWhiteSpace(objEntDTO.EnterPriseDomainURL)))
                        {
                            strPath = objEntDTO.EnterPriseDomainURL;
                        }
                        Body = Body.Replace("@@ETURNSLOGO@@", GetImage(strPath, "Content/images/logo.jpg"));
                    }
                }
                else
                {
                    Body = Body.Replace("@@ETURNSLOGO@@", GetImage(strPath, "Content/images/logo.jpg"));
                }
                Body = Body.Replace("@@Year@@", DateTime.UtcNow.Year.ToString());
                objMailToSend = new eMailToSendDTO();
                objMailToSend.ToAddress = ToEmailID;
                objMailToSend.CCAddress = CCEmailID;
                objMailToSend.CompanyID = CompanyID;
                objMailToSend.EnterpriseID = EntID;
                objMailToSend.RoomID = RoomID;
                objMailToSend.Subject = Subject;
                objMailToSend.MailBody = Body.ToString();
                objMailToSend.UserID = UserID;
                objMailToSend.Remarks = Remarks;
                if (_Attachments != null)
                {
                    objMailToSend.Attachments = _Attachments;
                }
                Insert_eMailToSend(objMailToSend);
            }
            finally
            {
                //if (_Attachments != null)
                //    _Attachments.Clear();

                _Attachments = null;
                objMailToSend = null;
            }
        }

        /// <summary>
        /// GetImage
        /// </summary>
        /// <param name="path"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public string GetImage(string path, string imagePath)
        {
            string str = string.Empty;
            str = @"<a href='" + path + @"' title=""E Turns Powered""> <img alt=""E Turns Powered"" width=""135"" height=""75"" src='" + (path + imagePath) + @"' style=""border: 0px currentColor; border-image: none;width:135px;height:75px;"" /></a>";
            return str;
        }

        /// <summary>
        /// Insert_eMailToSend
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private Int64 Insert_eMailToSend(eMailToSendDTO obj)
        {
            using (var context = new eTurns_MasterEntities())
            {
                eMailToSend objDB = null;
                eMailAttachment objAttDB = null;
                try
                {
                    objDB = new eMailToSend();
                    objDB.BCCAddress = obj.BCCAddress;
                    objDB.CCAddress = obj.CCAddress;
                    objDB.CompanyID = obj.CompanyID;
                    objDB.CreatedOn = DateTime.UtcNow;
                    objDB.EnterpriseID = obj.EnterpriseID;
                    objDB.ID = 0;
                    objDB.MailBody = obj.MailBody;
                    objDB.Remarks = obj.Remarks;
                    objDB.RoomID = obj.RoomID;
                    objDB.Subject = obj.Subject;
                    objDB.ToAddress = obj.ToAddress;
                    objDB.UserID = obj.UserID;


                    if (obj.Attachments != null && obj.Attachments.Count > 0)
                    {
                        objDB.HasAttachment = true;
                        foreach (var item in obj.Attachments)
                        {
                            objAttDB = new eMailAttachment()
                            {
                                AttachedFileName = item.AttachedFileName,
                                MimeType = item.MimeType,
                                EmailToSendID = objDB.ID,
                                FileData = item.FileData,
                                eMailToSend = objDB,
                            };
                            objDB.eMailAttachments.Add(objAttDB);
                        }
                    }
                    context.eMailToSends.Add(objDB);
                    context.SaveChanges();

                }
                finally
                {
                    objDB = null;
                    objAttDB = null;
                }


                return 0;
            }

        }

        /// <summary>
        /// Insert_eMailToSend
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public void Update_eMailToSend(eMailToSendDTO obj)
        {
            using (var context = new eTurns_MasterEntities())
            {
                eMailToSend objDB = null;
                try
                {
                    objDB = context.eMailToSends.FirstOrDefault(x => x.ID == obj.ID);
                    if ((objDB.SendingTried ?? 0) <= 2)
                    {
                        objDB.SendingTried = (objDB.SendingTried ?? 0) + 1;
                        context.SaveChanges();
                    }
                    else
                    {
                        string query = @"INSERT INTO [dbo].[eMailAttachment_FailHistory] SELECT * FROM eMailAttachment WHERE EmailToSendID = " + obj.ID + @";
                                         INSERT INTO [dbo].[eMailToSend_FailHistory] SELECT *,getutcdate(),'" + obj.ErrorDescription + @"' FROM [eMailToSend] WHERE ID = " + obj.ID;
                        context.Database.ExecuteSqlCommand(query);
                        DeleteMailToSend(obj.ID, false);
                    }
                }
                finally
                {
                    objDB = null;

                }
            }
        }

        /// <summary>
        /// Insert_eMailToSend
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private Int64 Insert_eMailExceptionLogs(eMailExceptionDTO obj)
        {

            return 0;
        }

        /// <summary>
        /// GetEmailToSend
        /// </summary>
        /// <returns></returns>
        public List<eMailToSendDTO> GetEmailToSend(int TakeRecord)
        {
            using (var context = new eTurns_MasterEntities())
            {
                List<eMailToSendDTO> listMailToSend = null;
                IEnumerable<eMailToSend> obj = null;
                List<eMailAttachmentDTO> objMailAttachmentList = null;
                eMailToSendDTO objMailToSend = null;
                eMailAttachmentDTO objMailAttachment = null;
                EnterpriseDTO objDTO = null;
                EnterpriseMasterDAL objEntDAL = null;
                try
                {
                    obj = context.eMailToSends.Where(x => (x.SendingTried ?? 0) < 3).OrderByDescending(x => x.CreatedOn).Take<eMailToSend>(TakeRecord);
                    if (obj != null && obj.Count() > 0)
                    {
                        listMailToSend = new List<eMailToSendDTO>();

                        foreach (var item in obj)
                        {
                            objMailAttachmentList = new List<eMailAttachmentDTO>();

                            if (item.eMailAttachments != null && item.eMailAttachments.Count > 0)
                            {
                                foreach (var itemAttch in item.eMailAttachments)
                                {
                                    objMailAttachment = new eMailAttachmentDTO()
                                    {
                                        AttachedFileName = itemAttch.AttachedFileName,
                                        eMailToSendID = itemAttch.EmailToSendID,
                                        FileData = itemAttch.FileData,
                                        ID = itemAttch.ID,
                                        MimeType = itemAttch.MimeType,
                                    };
                                    objMailAttachmentList.Add(objMailAttachment);
                                }
                            }

                            objMailToSend = new eMailToSendDTO()
                            {
                                Attachments = objMailAttachmentList,
                                BCCAddress = item.BCCAddress,
                                CCAddress = item.CCAddress,
                                CompanyID = item.CompanyID,
                                CreatedOn = item.CreatedOn,
                                EnterpriseID = item.EnterpriseID,
                                ID = item.ID,
                                MailBody = item.MailBody,
                                Remarks = item.Remarks,
                                RoomID = item.RoomID,
                                Subject = item.Subject,
                                ToAddress = item.ToAddress,
                                UserID = item.UserID,
                            };
                            objMailToSend.MailBody = objMailToSend.MailBody.Replace("@@Year@@", DateTime.UtcNow.Year.ToString());
                            if (objMailToSend.EnterpriseID.GetValueOrDefault(0) > 0)
                            {
                                objEntDAL = new EnterpriseMasterDAL();
                                objDTO = objEntDAL.GetEnterprise(objMailToSend.EnterpriseID.GetValueOrDefault(0));
                                string CompanyName = string.Empty;
                                string RoomName = string.Empty;
                                string UserName = string.Empty;
                                if (objMailToSend.CompanyID.GetValueOrDefault(0) > 0 && objMailToSend.MailBody.IndexOf("@@COMPANYNAME@@") > 0)
                                {
                                    CompanyName = context.Database.SqlQuery<string>("Select Name From [" + objDTO.EnterpriseDBName + "].[dbo].[CompanyMaster] Where ID = " + objMailToSend.CompanyID.GetValueOrDefault(0)).FirstOrDefault();
                                }
                                if (objMailToSend.RoomID.GetValueOrDefault(0) > 0 && objMailToSend.MailBody.IndexOf("@@ROOMNAME@@") > 0)
                                {
                                    RoomName = context.Database.SqlQuery<string>("Select RoomName From [" + objDTO.EnterpriseDBName + "].[dbo].[Room] Where ID = " + objMailToSend.RoomID.GetValueOrDefault(0)).FirstOrDefault();
                                }
                                if (objMailToSend.UserID.GetValueOrDefault(0) > 0 && objMailToSend.MailBody.IndexOf("@@USERNAME@@") > 0)
                                {
                                    UserName = context.Database.SqlQuery<string>("Select UserName From [" + objDTO.EnterpriseDBName + "].[dbo].[UserMaster] Where ID = " + objMailToSend.UserID.GetValueOrDefault(0)).FirstOrDefault();
                                }

                                objMailToSend.MailBody = GetEmailBodyWithReplaceTag(objMailToSend.MailBody, objDTO.ID, CompanyName, RoomName, UserName);
                            }
                            listMailToSend.Add(objMailToSend);
                        }
                    }
                    return listMailToSend;
                }
                finally
                {
                    obj = null;
                    listMailToSend = null;
                    objMailAttachmentList = null;
                    objMailToSend = null;
                    objMailAttachment = null;
                }
            }





        }

        /// <summary>
        /// DeleteMailToSend
        /// </summary>
        /// <param name="ID"></param>
        public void DeleteMailToSend(Int64 ID, bool IsMailSendSuccess)
        {
            using (var context = new eTurns_MasterEntities())
            {
                string strQuery = "";
                if (IsMailSendSuccess)
                {
                    strQuery = @"INSERT INTO [dbo].[eMailAttachment_SuccessHistory] SELECT * FROM eMailAttachment WHERE EmailToSendID = " + ID + @"; 
                                 INSERT INTO [dbo].[eMailToSend_SuccessHistory] SELECT *,getutcdate() FROM [eMailToSend] WHERE ID = " + ID + ";";
                    context.Database.ExecuteSqlCommand(strQuery);
                }
                //strQuery += @"DELETE FROM eMailAttachment WHERE EmailToSendID = " + ID
                //             + @"; DELETE FROM eMailToSend WHERE ID = " + ID;

                eMailToSend objEmailToSend = context.eMailToSends.FirstOrDefault(x => x.ID == ID);
                if ((objEmailToSend.HasAttachment ?? false) && objEmailToSend.eMailAttachments != null && objEmailToSend.eMailAttachments.Count > 0)
                {
                    objEmailToSend.eMailAttachments.ToList().ForEach(r => context.eMailAttachments.Remove(r));
                }
                context.eMailToSends.Remove(objEmailToSend);
                context.SaveChanges();

            }
        }




    }
}
