using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class EmailTemplateDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public EmailTemplateDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public EmailTemplateDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion


        public List<EmailTemplateDTO> GetAllEmailTemplate()
        {
            List<EmailTemplateDTO> obj = new List<EmailTemplateDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = (from u in context.EmailTemplates
                       select new EmailTemplateDTO
                       {
                           ID = u.ID,
                           TemplateName = u.TemplateName

                       }).AsParallel().ToList();

            }

            return obj;
        }
        public List<EmailTemplateDTO> GetAllEmailTemplateWithReport()
        {
            List<EmailTemplateDTO> obj = new List<EmailTemplateDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = (from u in context.ReportMasters
                       where (u.IsDeleted ?? false) == false && u.RoomID == 0 && u.CompanyID == 0 && u.IsBaseReport == true
                       select new EmailTemplateDTO
                       {
                           ID = u.ID,
                           TemplateName = u.ReportName

                       }).AsParallel().ToList();
                //obj = obj.Union(obj1).ToList();
            }

            return obj;
        }
        public List<EmailTemplateDTO> GetAllEmailTemplateToNotify()
        {

            List<EmailTemplateDTO> obj = new List<EmailTemplateDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = (from u in context.EmailTemplates
                       where u.TemplateType > 0
                       orderby u.TemplateName ascending
                       select new EmailTemplateDTO
                       {
                           ID = u.ID,
                           TemplateName = u.TemplateName,
                           ResourceKeyName = u.ResourceKeyName,
                           //IsSupplierRequired = u.IsSupplierRequired

                       }).AsParallel().ToList();

            }

            return obj;
        }
        public List<EmailTemplateDTO> GetAllEmailTemplateUC()
        {
            List<EmailTemplateDTO> obj = new List<EmailTemplateDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = (from u in context.EmailTemplates
                       where u.TemplateName.ToLower() != "forgotpassword" && u.TemplateName.ToLower() != "orderapproval" && u.TemplateName.ToLower() != "orderapprovereject"
                        && u.TemplateName.ToLower() != "returnorderapproval" && u.TemplateName.ToLower() != "returnorderapprovereject" && u.TemplateName.ToLower() != "transferapproval"
                        && u.TemplateName.ToLower() != "transferapprovereject" && u.TemplateName.ToLower() != "requisitionapproval" && u.TemplateName.ToLower() != "requisitionapprovereject"
                       select new EmailTemplateDTO
                       {
                           ID = u.ID,
                           TemplateName = u.TemplateName

                       }).AsParallel().ToList();

            }

            return obj;
        }
        public EmailTemplateDetail GetTemplateDetail(long TemplateID, long CultureID, long CompanyId, long RoomId)
        {
            EmailTemplateDetail objEmailTemplateDetail = new EmailTemplateDetail();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                objEmailTemplateDetail = context.EmailTemplateDetails.FirstOrDefault(u => u.EmailTemplateId == TemplateID && u.RoomId == RoomId && u.CompanyID == CompanyId && u.ResourceLaguageId == CultureID);
            }

            return objEmailTemplateDetail;
        }
        public EmailTemplateDetail GetReportTemplateDetail(long TemplateID, long CultureID, long CompanyId, long RoomId, long ReportId)
        {
            EmailTemplateDetail objEmailTemplateDetail = new EmailTemplateDetail();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                objEmailTemplateDetail = context.EmailTemplateDetails.FirstOrDefault(u => u.EmailTemplateId == 3 && u.RoomId == RoomId && u.CompanyID == CompanyId && u.ResourceLaguageId == CultureID && u.ReportId == ReportId);
            }

            return objEmailTemplateDetail;
        }
        public bool Edit(EmailTemplateDetailDTO objDTO)
        {

            //objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                EmailTemplateDetail obj = context.EmailTemplateDetails.FirstOrDefault(e => e.ID == objDTO.ID);
                //EmailTemplateDetail obj = new EmailTemplateDetail();
                if (obj != null)
                {
                    //obj.ID = objDTO.ID;
                    obj.EmailTemplateId = objDTO.EmailTempateId;
                    obj.MailBodyText = objDTO.MailBodyText;
                    obj.MailSubject = objDTO.MailSubject;
                    obj.ResourceLaguageId = objDTO.ResourceLaguageId;
                    obj.RoomId = objDTO.RoomId;
                    obj.CompanyID = objDTO.CompanyID;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.Updated = objDTO.Updated;
                    obj.ReportId = objDTO.ReportId;
                    //context.EmailTemplateDetails.Attach(obj);
                    //context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                    context.SaveChanges();
                }

                return true;
            }
        }
        public Int64 Insert(EmailTemplateDetailDTO objDTO)
        {

            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                EmailTemplateDetail obj = new EmailTemplateDetail();
                obj.ID = 0;
                obj.EmailTemplateId = objDTO.EmailTempateId;
                obj.MailBodyText = objDTO.MailBodyText;
                obj.MailSubject = objDTO.MailSubject;
                obj.ResourceLaguageId = objDTO.ResourceLaguageId;
                obj.RoomId = objDTO.RoomId;
                obj.CompanyID = objDTO.CompanyID;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.ReportId = objDTO.ReportId;
                context.EmailTemplateDetails.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;


                return obj.ID;
            }
        }
        public Int64 InsertEmailTemplateMaster(EmailTemplateDTO objDTO)
        {

            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                EmailTemplate obj = new EmailTemplate();
                obj.ID = 0;
                obj.TemplateName = objDTO.TemplateName;
                //obj.m = objDTO.MailSubject;
                obj.RoomId = objDTO.RoomId;
                obj.CompanyId = objDTO.CompanyId;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;

                context.EmailTemplates.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;


                return obj.ID;
            }
        }
        public bool SaveEmailTemplate(EmailTemplateDetailDTO objDTO)
        {
            EmailTemplateDetail obj = new EmailTemplateDetail();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = context.EmailTemplateDetails.FirstOrDefault(u => u.EmailTemplateId == objDTO.EmailTempateId && u.RoomId == objDTO.RoomId && u.CompanyID == objDTO.CompanyID && u.ResourceLaguageId == objDTO.ResourceLaguageId && (u.ReportId ?? 0) == (objDTO.ReportId ?? 0));

                if (obj != null && obj.ID > 0)
                {

                    obj.EmailTemplateId = objDTO.EmailTempateId;
                    obj.MailBodyText = objDTO.MailBodyText;
                    obj.MailSubject = objDTO.MailSubject;
                    obj.ResourceLaguageId = objDTO.ResourceLaguageId;
                    obj.RoomId = objDTO.RoomId;
                    obj.CompanyID = objDTO.CompanyID;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.Updated = objDTO.Updated;
                    obj.ReportId = objDTO.ReportId;

                    //context.EmailTemplateDetails.Attach(obj);
                    //context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    if (this.Insert(objDTO) > 0)
                        return true;
                }


            }
            return false;
        }
        public EmailTemplateDetailDTO GetEmailTemplate(string TemplateName, string Culture, long RoomId, long CompanyId)
        {
            EmailTemplateDetailDTO objEmailTemplateDetail = new EmailTemplateDetailDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                SqlParameter[] para = new SqlParameter[] {
                    new SqlParameter("@TemplateName",TemplateName),
                new SqlParameter("@Culture",Culture),
                new SqlParameter("@RoomId",RoomId),
                new SqlParameter("@CompanyId",CompanyId),
                };

                objEmailTemplateDetail = context.Database.SqlQuery<EmailTemplateDetailDTO>("EXEC uspGetEmailTemplate @TemplateName,@Culture,@RoomId,@CompanyId",
                    para).FirstOrDefault();

                //objEmailTemplateDetail = (from ET in context.EmailTemplates
                //                          join ETD in context.EmailTemplateDetails on ET.ID equals ETD.EmailTemplateId
                //                          join RL in context.ResourceLaguages on ETD.ResourceLaguageId equals RL.ID
                //                          where ET.TemplateName == TemplateName && RL.Culture == Culture && ETD.RoomId == RoomId && ETD.CompanyID == CompanyId
                //                          select new EmailTemplateDetailDTO
                //                          {
                //                              ID = ETD.ID,
                //                              EmailTempateId = ETD.EmailTemplateId,
                //                              MailBodyText = ETD.MailBodyText,
                //                              MailSubject = ETD.MailSubject,
                //                              ReportId = ETD.ReportId
                //                          }

                //                        ).FirstOrDefault();
            }

            return objEmailTemplateDetail;

        }
        public EmailTemplateDetailDTO GetEmailTemplate(string TemplateName, string Culture, long RoomId, long CompanyId, string DBConnectionstring)
        {
            EmailTemplateDetailDTO objEmailTemplateDetail = new EmailTemplateDetailDTO();
            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                objEmailTemplateDetail = (from ET in context.EmailTemplates
                                          join ETD in context.EmailTemplateDetails on ET.ID equals ETD.EmailTemplateId
                                          join RL in context.ResourceLaguages on ETD.ResourceLaguageId equals RL.ID
                                          where ET.TemplateName == TemplateName && RL.Culture == Culture && ETD.RoomId == RoomId && ETD.CompanyID == CompanyId
                                          select new EmailTemplateDetailDTO
                                          {
                                              ID = ETD.ID,
                                              EmailTempateId = ETD.EmailTemplateId,
                                              MailBodyText = ETD.MailBodyText,
                                              MailSubject = ETD.MailSubject,
                                              ReportId = ETD.ReportId
                                          }

                                        ).FirstOrDefault();
            }

            return objEmailTemplateDetail;

        }
        public long GeteturnsLanguageId(string CurCulture)
        {
            long languageid = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                languageid = Convert.ToInt64(context.ResourceLaguages.Where(u => u.Culture == CurCulture).Select(p => p.ID).SingleOrDefault());
            }
            return languageid;
        }
        public long GetTemplateId(string EmailTemplateName)
        {
            long TemplateId = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                TemplateId = Convert.ToInt64(context.EmailTemplates.Where(u => u.TemplateName == EmailTemplateName).Select(p => p.ID).SingleOrDefault());
            }
            return TemplateId;
        }
        public List<EmailTemplateDetailDTO> GetEmailTemplateDetails(long EmailTemplateId, long NotificationId, long CompanyID, long RoomID, long UserID, long ReportID, int ScheduleFor)
        {
            List<EmailTemplateDetailDTO> lstEmailDetials = new List<EmailTemplateDetailDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (EmailTemplateId > 0)
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@EmailTemplateId", EmailTemplateId), new SqlParameter("@NotificationId", NotificationId), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@UserID", UserID), new SqlParameter("@ReportID", ReportID), new SqlParameter("@ScheduleFor", ScheduleFor) };
                    lstEmailDetials = context.Database.SqlQuery<EmailTemplateDetailDTO>("EXEC [GetEmailTemplateDetails] @EmailTemplateId,@NotificationId,@CompanyID,@RoomID,@UserID,@ReportID,@ScheduleFor", params1).ToList();
                }
            }
            return lstEmailDetials;
        }

        public EVMIEmailTemplateDTO GetEVMIEmailTemplate(long RoomID, long CompanyID,
            long EnterpriseID, string templateName, Int16 ScheduleMODE)
        {
            EVMIEmailTemplateDTO objDTO = null;
            SqlParameter[] para = new SqlParameter[] {
                new SqlParameter("@RoomId",RoomID),
                new SqlParameter("@CompanyId",CompanyID),
                new SqlParameter("@EnterpriseID",EnterpriseID),
                new SqlParameter("@TemplateName",templateName),
                new SqlParameter("@ScheduleMODE",ScheduleMODE),
            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objDTO = context.Database.SqlQuery<EVMIEmailTemplateDTO>("EXEC GetEVMIEmailTemplate @RoomId,@CompanyId,@EnterpriseID,@TemplateName,@ScheduleMODE",
                para).FirstOrDefault();
            }

            return objDTO;
        }

       


        public EVMIEmailTemplateDTO GetEVMIEmailTemplateForPollDone(long RoomID, long CompanyID,
            long EnterpriseID)
        {
            EVMIEmailTemplateDTO objDTO = GetEVMIEmailTemplate(RoomID, CompanyID, EnterpriseID, MailTemplate.eVMIPollDone.ToString(), 5);
            return objDTO;
        }

        public EVMIEmailTemplateDTO GetEVMIEmailTemplateForTareDone(long RoomID, long CompanyID,
            long EnterpriseID)
        {
            EVMIEmailTemplateDTO objDTO = GetEVMIEmailTemplate(RoomID, CompanyID, EnterpriseID, MailTemplate.eVMITareDone.ToString(), 5);           
            return objDTO;

        }

        public EVMIEmailTemplateDTO GetEVMIEmailTemplateForWeightPerPieceDone(long RoomID, long CompanyID,
            long EnterpriseID)
        {
            EVMIEmailTemplateDTO objDTO = GetEVMIEmailTemplate(RoomID, CompanyID, EnterpriseID, MailTemplate.eVMIItemWeightPerPieceDone.ToString(), 5); 
            return objDTO;
        }

        public EVMIEmailTemplateDTO GetEVMIEmailTemplateForMissingPoll(long RoomID, long CompanyID,
           long EnterpriseID)
        {
            EVMIEmailTemplateDTO objDTO = GetEVMIEmailTemplate(RoomID, CompanyID, EnterpriseID, MailTemplate.eVMIMissingPoll.ToString(), 5); 
            return objDTO;
        }

        public EVMIEmailTemplateDTO GetEVMIEmailTemplateShelfDone(long RoomID, long CompanyID,
            long EnterpriseID)
        {
            EVMIEmailTemplateDTO objDTO = GetEVMIEmailTemplate(RoomID, CompanyID, EnterpriseID, MailTemplate.eVMIShelfIDDone.ToString(), 5);
            return objDTO;
        }
         
        public EVMIEmailTemplateDTO GetEVMIEmailTemplateForCalibrateDone(long RoomID, long CompanyID,
           long EnterpriseID)
        {
            EVMIEmailTemplateDTO objDTO = GetEVMIEmailTemplate(RoomID, CompanyID, EnterpriseID, MailTemplate.eVMICalibrateDone.ToString(), 5);
            return objDTO;

        }

        public EVMIEmailTemplateDTO GetEVMIEmailTemplateForResetDone(long roomId, long companyId, long enterpriseID)
        {
            EVMIEmailTemplateDTO objDTO = GetEVMIEmailTemplate(roomId, companyId, enterpriseID, 
                MailTemplate.eVMIResetDone.ToString(), 5);
            return objDTO;
        }
    }// class
}
