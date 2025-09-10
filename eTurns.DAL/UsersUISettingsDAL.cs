using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace eTurns.DAL
{
    public class UsersUISettingsDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public UsersUISettingsDAL(base.DataBaseName)
        //{

        //}

        public UsersUISettingsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public UsersUISettingsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        public UsersUISettingsDTO GetRecord(Int64 UserID, string ListName)
        {
            //Get Cached-Media - Userwise
            IEnumerable<UsersUISettingsDTO> ObjCache = CacheHelper<IEnumerable<UsersUISettingsDTO>>.GetCacheItem("Cached_UsersUISettingsDTO_" + UserID.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<UsersUISettingsDTO> obj = (from u in context.UsersUISettings
                                                           where u.UserID == UserID
                                                           select new UsersUISettingsDTO
                                                           {
                                                               ID = u.ID,
                                                               ListName = u.ListName,
                                                               UserID = u.UserID,
                                                               JSONDATA = u.JSONDATA
                                                           }).AsParallel().ToList();

                    ObjCache = CacheHelper<IEnumerable<UsersUISettingsDTO>>.AddCacheItem("Cached_UsersUISettingsDTO_" + UserID.ToString(), obj);
                }
            }

            return ObjCache.FirstOrDefault(t => t.ListName == ListName);
        }

        public Int64 Insert(UsersUISettingsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UsersUISetting obj = new UsersUISetting();
                obj.ID = objDTO.ID;
                obj.ListName = objDTO.ListName;
                obj.JSONDATA = objDTO.JSONDATA;
                obj.UserID = objDTO.UserID;
                context.UsersUISettings.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    IEnumerable<UsersUISettingsDTO> ObjCache = CacheHelper<IEnumerable<UsersUISettingsDTO>>.GetCacheItem("Cached_UsersUISettingsDTO_" + objDTO.UserID.ToString());
                    if (ObjCache != null)
                    {
                        List<UsersUISettingsDTO> tempC = new List<UsersUISettingsDTO>();
                        tempC.Add(objDTO);

                        IEnumerable<UsersUISettingsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                        CacheHelper<IEnumerable<UsersUISettingsDTO>>.AppendToCacheItem("Cached_UsersUISettingsDTO_" + objDTO.UserID.ToString(), NewCache);
                    }
                }
                return obj.ID;
            }
        }

        public bool Edit(UsersUISettingsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UsersUISetting obj = context.UsersUISettings.SingleOrDefault(t => t.UserID == objDTO.UserID && t.ListName == objDTO.ListName);
                if (obj == null)
                {
                    Int64 ID = Insert(objDTO);
                    obj = context.UsersUISettings.SingleOrDefault(t => t.UserID == objDTO.UserID && t.ListName == objDTO.ListName);
                }

                obj.ListName = objDTO.ListName;
                obj.JSONDATA = objDTO.JSONDATA;
                obj.UserID = objDTO.UserID;
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                objDTO.ID = obj.ID;

                //Get Cached-Media
                IEnumerable<UsersUISettingsDTO> ObjCache = CacheHelper<IEnumerable<UsersUISettingsDTO>>.GetCacheItem("Cached_UsersUISettingsDTO_" + objDTO.UserID.ToString());
                if (ObjCache != null)
                {
                    List<UsersUISettingsDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<UsersUISettingsDTO> tempC = new List<UsersUISettingsDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<UsersUISettingsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<UsersUISettingsDTO>>.AppendToCacheItem("Cached_UsersUISettingsDTO_" + objDTO.UserID.ToString(), NewCache);
                }

                return true;
            }

        }
        public UsersUISettingsDTO SaveUserListViewSettings(UsersUISettingsDTO objDTO)
        {
            UsersUISettingsDTO objUsersUISettingsDTO = new UsersUISettingsDTO();
            UsersUISetting objUsersUISetting = new UsersUISetting();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objUsersUISetting = context.UsersUISettings.FirstOrDefault(t => t.UserID == objDTO.UserID && t.ListName == objDTO.ListName);
                if (objUsersUISetting == null)
                {
                    objUsersUISetting = new UsersUISetting();
                    objUsersUISetting.ListName = objDTO.ListName;
                    objUsersUISetting.JSONDATA = objDTO.JSONDATA;
                    objUsersUISetting.UserID = objDTO.UserID;
                    context.UsersUISettings.Add(objUsersUISetting);
                    context.SaveChanges();
                    objDTO.ID = objUsersUISetting.ID;
                }
                else
                {
                    objUsersUISetting.JSONDATA = objDTO.JSONDATA;
                    context.SaveChanges();
                }
                IEnumerable<UsersUISettingsDTO> ObjCache = CacheHelper<IEnumerable<UsersUISettingsDTO>>.GetCacheItem("Cached_UsersUISettingsDTO_" + objDTO.UserID.ToString());
                if (ObjCache != null)
                {
                    List<UsersUISettingsDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.UserID == objDTO.UserID && i.ListName == objDTO.ListName);
                    ObjCache = objTemp.AsEnumerable();

                    List<UsersUISettingsDTO> tempC = new List<UsersUISettingsDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<UsersUISettingsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<UsersUISettingsDTO>>.AppendToCacheItem("Cached_UsersUISettingsDTO_" + objDTO.UserID.ToString(), NewCache);
                }
                return objDTO;
            }
        }

        public long InsertOverrideRoomsGridSetUpRequest(long EnterpriseId, long CompanyId, long RoomId, long UserId, long RoleId, int UserType,string UsersUISettingType)
        {
            var params1 = new SqlParameter[] { 
                                               new SqlParameter("@EnterpriseId", EnterpriseId),
                                               new SqlParameter("@CompanyId", CompanyId),
                                               new SqlParameter("@RoomId", RoomId),
                                               new SqlParameter("@UserId", UserId),
                                               new SqlParameter("@RoleId", RoleId),
                                               new SqlParameter("@UserType", UserType),
                                               new SqlParameter("@UsersUISettingType", UsersUISettingType)
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<long>("exec [InsertOverrideRoomsGridSetUpRequest] @EnterpriseId,@CompanyId,@RoomId,@UserId,@RoleId,@UserType,@UsersUISettingType", params1).FirstOrDefault();
                //return context.Database.SqlQuery<ZohoReponseDTO>("exec [" + DbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[UpdateABECRCreateWithUserIDRoleID] @ID,@RoleID,@UserID", params1).FirstOrDefault();
            }
        }

        public List<OverrideRoomsGridSetUpRequestDTO> GetOverrideRoomsGridSetUpToProcess()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OverrideRoomsGridSetUpRequestDTO>("exec [GetOverrideRoomsGridSetUpToProcess] ").ToList();
            }
        }

        public void SetOverrideRoomsGridSetUpRequestCompletedWithSuccess(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [SetOverrideRoomsGridSetUpRequestCompletedWithSuccess] @ID", params1);
            }
        }

        public void SetOverrideRoomsGridSetUpRequestCompletedWithError(long ID, string ErrorException)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@ErrorException", ErrorException ?? (object)DBNull.Value) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [SetOverrideRoomsGridSetUpRequestCompletedWithError] @ID,@ErrorException", params1);
            }
        }

    }
}
