using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Configuration;

namespace eTurns.DAL
{
    public partial class eMailMasterDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Insert_eMailToSend
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public void Update_eMailToSend(eMailToSendDTO obj)
        {
            using (var context = new eTurnsEntities(base.GetEmailDBConnectionString))
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
                        context.ExecuteStoreCommand(query);
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
        /// GetEmailToSend
        /// </summary>
        /// <returns></returns>
        public List<eMailToSendDTO> GetEmailToSend(int TakeRecord)
        {
            using (var context = new eTurnsEntities(base.GetEmailDBConnectionString))
            {
                List<eMailToSendDTO> listMailToSend = null;
                IEnumerable<eMailToSend> obj = null;
                List<eMailAttachmentDTO> objMailAttachmentList = null;
                eMailToSendDTO objMailToSend = null;
                eMailAttachmentDTO objMailAttachment = null;
                EnterpriseDTO objDTO = null;
                EnterpriseDAL objEntDAL = null;
                try
                {
                    obj = context.eMailToSends.OrderByDescending(x => x.CreatedOn).Take<eMailToSend>(TakeRecord);
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
                            objMailToSend.MailBody = objMailToSend.MailBody.Replace("@@Year@@", DateTime.Now.Year.ToString());
                            if (objMailToSend.EnterpriseID.GetValueOrDefault(0) > 0)
                            {
                                objEntDAL = new EnterpriseDAL(base.DataBaseName);
                                objDTO = objEntDAL.GetEnterpriseFromeTurnsMasterDB(objMailToSend.EnterpriseID.GetValueOrDefault(0));
                                string CompanyName = string.Empty;
                                string RoomName = string.Empty;
                                string UserName = string.Empty;
                                if (objMailToSend.CompanyID.GetValueOrDefault(0) > 0 && objMailToSend.MailBody.IndexOf("@@COMPANYNAME@@") > 0)
                                {
                                    CompanyName = context.ExecuteStoreQuery<string>("Select Name From [" + objDTO.EnterpriseDBName + "].[dbo].[CompanyMaster] Where ID = " + objMailToSend.CompanyID.GetValueOrDefault(0)).FirstOrDefault();
                                }
                                if (objMailToSend.RoomID.GetValueOrDefault(0) > 0 && objMailToSend.MailBody.IndexOf("@@ROOMNAME@@") > 0)
                                {
                                    RoomName = context.ExecuteStoreQuery<string>("Select RoomName From [" + objDTO.EnterpriseDBName + "].[dbo].[Room] Where ID = " + objMailToSend.RoomID.GetValueOrDefault(0)).FirstOrDefault();
                                }
                                if (objMailToSend.UserID.GetValueOrDefault(0) > 0 && objMailToSend.MailBody.IndexOf("@@USERNAME@@") > 0)
                                {
                                    UserName = context.ExecuteStoreQuery<string>("Select UserName From [" + objDTO.EnterpriseDBName + "].[dbo].[UserMaster] Where ID = " + objMailToSend.UserID.GetValueOrDefault(0)).FirstOrDefault();
                                }

                                objMailToSend.MailBody = GetEmailBodyWithReplaceTag(objMailToSend.MailBody, objDTO.ID, CompanyName, RoomName, UserName, objDTO.EnterpriseDBName);
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
            using (var context = new eTurnsEntities(base.GetEmailDBConnectionString))
            {
                string strQuery = "";
                if (IsMailSendSuccess)
                {
                    strQuery = @"INSERT INTO [dbo].[eMailAttachment_SuccessHistory] SELECT * FROM eMailAttachment WHERE EmailToSendID = " + ID + @"; 
                                 INSERT INTO [dbo].[eMailToSend_SuccessHistory] SELECT *,getutcdate() FROM [eMailToSend] WHERE ID = " + ID + ";";
                    context.ExecuteStoreCommand(strQuery);
                }
                //strQuery += @"DELETE FROM eMailAttachment WHERE EmailToSendID = " + ID
                //             + @"; DELETE FROM eMailToSend WHERE ID = " + ID;

                eMailToSend objEmailToSend = context.eMailToSends.FirstOrDefault(x => x.ID == ID);
                if ((objEmailToSend.HasAttachment ?? false) && objEmailToSend.eMailAttachments != null && objEmailToSend.eMailAttachments.Count > 0)
                {
                    objEmailToSend.eMailAttachments.ToList().ForEach(r => context.eMailAttachments.DeleteObject(r));
                }
                context.DeleteObject(objEmailToSend);
                context.SaveChanges();

            }
        }
    }
}
