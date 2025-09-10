using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurnsMaster.DAL
{
    public partial class EnterPriseUserMasterDAL : eTurnsMasterBaseDAL
    {

        /// <summary>
        /// Get Paged Records from the User Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<UserMasterDTO> GetAllUsers()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetAllUsersPlain]").ToList();
            }
        }

        public UserMasterDTO GetUserByIdForServiceNormal(long Id)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetUserByIdForServiceNormal] @Id", params1).FirstOrDefault();
            }
        }

        public UserMasterDTO AuthenticateUserByUserNameAndPassword(string Email, string Password)
        {
            UserMasterDTO objresult = new UserMasterDTO();

            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserName", Email), new SqlParameter("@Password", Password) };

                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    objresult =  context.Database.SqlQuery<UserMasterDTO>("exec [AuthenticateUserByUserNameAndPassword] @UserName,@Password", params1).FirstOrDefault();
                }
            }
            return objresult;
        }

        public UserMasterDTO CheckAuthanticationUserName(string UserName, string Password)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            EulaMasterDTO objEulaMaster = new EulaMasterDTO();

            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    objresult = AuthenticateUserWithUserNamePass(UserName, Password);
                    objEulaMaster = context.Database.SqlQuery<EulaMasterDTO>("exec [GetLatestEulaMaster]").FirstOrDefault();

                    if (objEulaMaster != null && objresult != null)
                    {
                        if (objresult.RoleID != -1)
                        {
                            if (objresult.LastLicenceAccept == null || objresult.LastLicenceAccept < objEulaMaster.CreatedOn)
                            {
                                objresult.NewEulaAccept = false;
                            }
                            else
                            {
                                objresult.NewEulaAccept = true;
                            }
                        }
                        else
                        {
                            objresult.NewEulaAccept = true;
                        }
                    }
                    else
                    {
                        if (objresult != null)
                        {
                            objresult.NewEulaAccept = true;
                        }
                    }
                    if (objresult != null)
                    {
                        objresult.HasChangedFirstPassword = context.UserPasswordChangeHistories.Any(t => t.UserId == objresult.ID);
                    }

                }

            }
            return objresult;
        }

        public UserMasterDTO AuthenticateUserWithUserNamePass(string UserName, string Password)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserName", UserName ?? (object)DBNull.Value), new SqlParameter("@Password", Password ?? (object)DBNull.Value) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery <UserMasterDTO>("exec [AuthenticateUserWithUserNamePass] @UserName,@Password", params1).FirstOrDefault();
            }
        }

        public UserMasterDTO AuthenticateUserWithUserNameOnly(string UserName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserName", UserName ?? (object)DBNull.Value) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserMasterDTO>("exec [AuthenticateUserWithUserNameOnly] @UserName", params1).FirstOrDefault();
            }
        }

        public string GetJwtToken(long UserId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserId", UserId) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<string>("exec [GetJwtTokenByUserId] @UserId", params1).FirstOrDefault();
            }
        }

        public UserMasterDTO CheckAuthanticationUserNameOnly(string UserName)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            EulaMasterDTO objEulaMaster = new EulaMasterDTO();
            if (!string.IsNullOrEmpty(UserName))
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    objresult = AuthenticateUserWithUserNameOnly(UserName);
                    objEulaMaster = context.Database.SqlQuery<EulaMasterDTO>("exec [GetLatestEulaMaster]").FirstOrDefault();

                    if (objEulaMaster != null && objresult != null)
                    {
                        if (objresult.RoleID != -1)
                        {
                            if (objresult.LastLicenceAccept == null || objresult.LastLicenceAccept < objEulaMaster.CreatedOn)
                            {
                                objresult.NewEulaAccept = false;
                            }
                            else
                            {
                                objresult.NewEulaAccept = true;
                            }
                        }
                        else
                        {
                            objresult.NewEulaAccept = true;
                        }
                    }
                    else
                    {
                        if (objresult != null)
                        {
                            objresult.NewEulaAccept = true;
                        }
                    }
                    if (objresult != null)
                    {
                        objresult.HasChangedFirstPassword = context.UserPasswordChangeHistories.Any(t => t.UserId == objresult.ID);
                    }

                }

            }
            return objresult;
        }

        public UserMasterDTO GetUserByRoleIdAndEntIdNormal(long RoleID, long EnterpriseId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoleId", RoleID), new SqlParameter("@EnterpriseId", EnterpriseId) };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetUserByRoleIdAndEntIdNormal] @RoleId,@EnterpriseId", params1).FirstOrDefault();
            }
        }

        public UserMasterDTO SaveUseronMasterDB(UserMasterDTO objUserMasterDTO)
        {
            using (var Dbcntc = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UserMaster objUserMaster = null;

                if (objUserMasterDTO.ID > 0)
                {
                    objUserMaster = Dbcntc.UserMasters.FirstOrDefault(t => t.ID == objUserMasterDTO.ID);
                    if (objUserMaster != null)
                    {
                        objUserMaster.CompanyID = objUserMasterDTO.CompanyID;
                        Dbcntc.SaveChanges();
                    }
                }
                else
                {
                    objUserMaster = new UserMaster();
                    objUserMaster.CompanyID = objUserMasterDTO.CompanyID;
                    objUserMaster.Created = DateTime.UtcNow;
                    objUserMaster.CreatedBy = objUserMasterDTO.CreatedBy;
                    objUserMaster.Email = objUserMasterDTO.Email;
                    objUserMaster.EnterpriseId = objUserMasterDTO.EnterpriseId;
                    objUserMaster.GUID = Guid.NewGuid();
                    objUserMaster.IsArchived = false;
                    objUserMaster.IsDeleted = false;
                    objUserMaster.IsEnterPriseUser = false;
                    objUserMaster.LastUpdatedBy = objUserMasterDTO.LastUpdatedBy;
                    objUserMaster.Password = objUserMasterDTO.Password;
                    objUserMaster.Phone = objUserMasterDTO.Phone;
                    objUserMaster.RoleId = objUserMasterDTO.RoleID;
                    objUserMaster.RoomId = objUserMasterDTO.Room;
                    objUserMaster.SyncDailyTime = TimeSpan.FromHours(2);
                    objUserMaster.SyncMins = 13;
                    objUserMaster.Updated = objUserMasterDTO.Updated;
                    objUserMaster.UserName = objUserMasterDTO.UserName;
                    objUserMaster.UserType = objUserMasterDTO.UserType;
                    Dbcntc.UserMasters.Add(objUserMaster);
                    Dbcntc.SaveChanges();
                    objUserMasterDTO.ID = objUserMaster.ID;

                }
                return objUserMasterDTO;

            }

        }
        public UserMasterDTO GetUserByNameNormal(string UserName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserName", UserName) };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetUserByNameNormal] @UserName", params1).SingleOrDefault();
            }
        }

        public List<UserRoomAccess> getUserPermission(long UserID)
        {
            //string Qry = "Select * from MstUserRoomAccess where UserID=" + UserID;
            string Qry = "EXEC GetUserPermission @UserID";
            List<SqlParameter> para = new List<SqlParameter>() {
                new SqlParameter("@UserID",UserID)
            };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Database.SqlQuery<UserRoomAccess>(Qry, para.ToArray())
                        select new UserRoomAccess
                        {
                            CompanyId = u.CompanyId,
                            EnterpriseId = u.EnterpriseId,
                            ID = u.ID,
                            RoleId = u.RoleId,
                            RoomId = u.RoomId,
                            UserId = u.UserId
                        }).ToList();
            }
        }

        public List<OLEDBConnectionInfo> getCons()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OLEDBConnectionInfo>("EXEC getCons").ToList();
            }

        }
        public List<OLEDBConnectionInfo> SaveCons(List<OLEDBConnectionInfo> lstUpdates)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {


                if (lstUpdates != null && lstUpdates.Count > 0)
                {
                    foreach (var item in lstUpdates)
                    {
                        var params1 = new SqlParameter[] { new SqlParameter("@ID", item.ID), new SqlParameter("@APP", item.APP ?? (object)DBNull.Value), new SqlParameter("@ApplicationIntent", item.ApplicationIntent ?? (object)DBNull.Value), new SqlParameter("@AppDatabase", item.AppDatabase ?? (object)DBNull.Value), new SqlParameter("@MarsConn", item.MarsConn ?? (object)DBNull.Value), new SqlParameter("@PacketSize", item.PacketSize ?? (object)DBNull.Value), new SqlParameter("@PWD", item.PWD ?? (object)DBNull.Value), new SqlParameter("@Server", item.Server ?? (object)DBNull.Value), new SqlParameter("@Timeout", item.Timeout ?? (object)DBNull.Value), new SqlParameter("@Trusted_Connection", item.Trusted_Connection ?? (object)DBNull.Value), new SqlParameter("@UID", item.UID ?? (object)DBNull.Value), new SqlParameter("@FailoverPartner", item.FailoverPartner ?? (object)DBNull.Value), new SqlParameter("@PersistSensitive", item.PersistSensitive ?? (object)DBNull.Value), new SqlParameter("@MultiSubnetFailover", item.MultiSubnetFailover ?? (object)DBNull.Value), new SqlParameter("@UserID", item.UpdatedBy) };
                        context.Database.ExecuteSqlCommand("exec [SaveCon] @ID,@APP,@ApplicationIntent,@AppDatabase,@MarsConn,@PacketSize,@PWD,@Server,@Timeout,@Trusted_Connection,@UID,@FailoverPartner,@PersistSensitive,@MultiSubnetFailover,@UserID", params1);
                    }

                    //,,@ApplicationIntent,@AppDatabase,@MarsConn,@PacketSize,@PWD,@Server,@Timeout,@Trusted_Connection,@UID,@FailoverPartner,@PersistSensitive,@UserID
                }
            }
            return getCons();
        }

        public string GetEnterpriseSSOMapConfigurationKeyByID(long userId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserId", userId) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<string>("exec [GetEnterpriseSSOMapConfigurationKeyByID] @UserId", params1).FirstOrDefault();
            }
        }
    }
}
