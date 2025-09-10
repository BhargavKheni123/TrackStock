using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class EmailUserConfigurationDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public EmailUserConfigurationDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        //public EmailUserConfigurationDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}
        #endregion

        #region [class Methods]
        
        public IEnumerable<EmailUserConfigurationDTO> GetAllExternalUserRecords(string EmailTemplateName, bool IsSelectedOnly, long RoomId, long CompanyId)
        {
            var param = new SqlParameter[] {
                                                new SqlParameter("@IsSelectedOnly", IsSelectedOnly),
                                                new SqlParameter("@RoomId", RoomId),
                                                new SqlParameter("@CompanyId", CompanyId),                                                
                                                new SqlParameter("@EmailTemplateName", EmailTemplateName)                                               
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EmailUserConfigurationDTO>("exec GetAllExternalUserRecords @IsSelectedOnly,@RoomId,@CompanyId,@EmailTemplateName ", param).ToList();
            }
        }

        public IEnumerable<EmailUserConfigurationDTO> GetAllEmailUserMaster(string EmailTemplateName, bool IsSelectedOnly, long CompanyId, long RoomId)
        {
            var param = new SqlParameter[] {
                                                new SqlParameter("@IsSelectedOnly", IsSelectedOnly),
                                                new SqlParameter("@RoomId", RoomId),
                                                new SqlParameter("@CompanyId", CompanyId),
                                                new SqlParameter("@EmailTemplateName", EmailTemplateName)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EmailUserConfigurationDTO>("exec GetAllEmailUserMaster @IsSelectedOnly,@RoomId,@CompanyId,@EmailTemplateName ", param).ToList();
            }
        }
        
        public Int64 Insert(EmailUserConfigurationDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                EmailUserMaster objcheck = (from c in context.EmailUserMasters
                                            where c.Email == objDTO.Email && c.RoomId == objDTO.RoomId && c.IsDeleted == false
                                            select c).SingleOrDefault();
                if (objcheck == null)
                {
                    EmailUserMaster obj = new EmailUserMaster();
                    obj.ID = 0;
                    obj.Name = objDTO.Name;
                    obj.Email = objDTO.Email;
                    obj.RoomId = objDTO.RoomId;
                    obj.CompanyId = objDTO.CompanyId;
                    obj.Created = objDTO.CreatedOn;
                    context.EmailUserMasters.Add(obj);
                    context.SaveChanges();

                    EmailUserMasterDetail objDetail = new EmailUserMasterDetail();
                    objDetail.ID = 0;
                    objDetail.EmailUserID = obj.ID;
                    objDetail.TemplateName = objDTO.TemplateName;
                    objDetail.RoomId = objDTO.RoomId;
                    objDetail.CompanyId = objDTO.CompanyId;
                    context.EmailUserMasterDetails.Add(objDetail);
                    context.SaveChanges();

                    return obj.ID;

                }
                return 0;

            }
        }
        
        public void InsertUserInTemplate(List<EmailUserMasterDetailDTO> lst, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (lst.Count > 0)
                {
                    DeleteUserTemplte(lst[0].Name.ToString());
                    foreach (EmailUserMasterDetailDTO item in lst)
                    {
                        EmailUserMasterDetail objDetail = new EmailUserMasterDetail();
                        objDetail.ID = item.ID;
                        objDetail.EmailUserID = item.UserID;
                        objDetail.TemplateName = item.Name;
                        objDetail.RoomId = RoomId;
                        objDetail.CompanyId = CompanyId;
                        context.EmailUserMasterDetails.Add(objDetail);
                    }
                    context.SaveChanges();
                }
            }
        }
        
        public void DeleteUserTemplte(string TempalateName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<EmailUserMasterDetail> lst = (from m in context.EmailUserMasterDetails
                                                   where m.TemplateName == TempalateName
                                                   select m).ToList();

                foreach (EmailUserMasterDetail Deleteitem in lst)
                {
                    context.EmailUserMasterDetails.Remove(Deleteitem);
                }
                context.SaveChanges();
            }
        }
        
        public string UpdateEmailUserMaster(long Id, string value, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                new SqlParameter("@ID", Id),
                                                new SqlParameter("@ColumnName", columnName),
                                                new SqlParameter("@Value", value)
                                            };
                context.Database.ExecuteSqlCommand("exec UpdateEmailUserMaster @ID,@ColumnName,@Value", params1);
            }
            return value;
        }
        
        public bool Delete(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                EmailUserMasterDetail objEmailUserMstDetail = context.EmailUserMasterDetails.Single(x => x.EmailUserID == id);
                EmailUserMaster objEmailUserMstr = context.EmailUserMasters.Single(x => x.ID == id);
                context.EmailUserMasterDetails.Remove(objEmailUserMstDetail);
                context.EmailUserMasters.Remove(objEmailUserMstr);
                context.SaveChanges();
                return true;
            }
        }

        public List<EnterpriseUserDetailDTO> GetEnterpriseUserList(string EntId)
        {
            List<EnterpriseUserDetailDTO> lstEnterpriseUserDetailDTO = new List<EnterpriseUserDetailDTO>();
            List<EnterpriseUserDetailDTO> lstExtUsers = new List<EnterpriseUserDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstEnterpriseUserDetailDTO = (from usr in context.UserMasters
                                              where usr.IsDeleted == false && usr.IsArchived == false && usr.RoleId != -1 && usr.RoleId != -2
                                              select new EnterpriseUserDetailDTO
                                              {
                                                  Company = string.Empty,
                                                  CreatedOn = usr.Created ?? DateTime.MinValue,
                                                  email = usr.Email,
                                                  Phone = usr.Phone,
                                                  Stockroom = string.Empty,
                                                  UserName = usr.UserName
                                              }).ToList();

                lstExtUsers = (from usr in context.EmailUserMasters
                               join rm in context.Rooms on usr.RoomId equals rm.ID
                               join cm in context.CompanyMasters on usr.CompanyId equals cm.ID
                               select new EnterpriseUserDetailDTO
                               {
                                   Company = cm.Name,
                                   CreatedOn = usr.Created ?? DateTime.MinValue,
                                   email = usr.Email,
                                   Phone = string.Empty,
                                   Stockroom = rm.RoomName,
                                   UserName = usr.Email
                               }).ToList();
            }

            lstEnterpriseUserDetailDTO = lstEnterpriseUserDetailDTO.Union(lstExtUsers).ToList();
            return lstEnterpriseUserDetailDTO;
        }
        public bool DeleteEmailUserConfig(int id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                EmailUserMaster objEmailUserMst = context.EmailUserMasters.Single(x => x.ID == id);
                objEmailUserMst.IsDeleted = true;
                context.SaveChanges();
                return true;
            }
        }
        #endregion
    }
}
