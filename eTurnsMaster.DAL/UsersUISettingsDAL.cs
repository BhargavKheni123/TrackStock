using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace eTurnsMaster.DAL
{
    public class UsersUISettingsDAL : eTurnsMasterBaseDAL
    {

        public UsersUISettingsDTO GetRecord(Int64 UserID, string ListName, string UserUISettingType, long EnterpriseID = 0, long CompanyID = 0, long RoomID = 0)
        {
            //Get Cached-Media - Userwise
            IEnumerable<UsersUISettingsDTO> ObjCache = CacheHelper<IEnumerable<UsersUISettingsDTO>>.GetCacheItem("Cached_UsersUISettingsDTO_" + UserID.ToString() + EnterpriseID.ToString() + CompanyID.ToString() + RoomID.ToString() + (ListName ?? string.Empty));
            if (ObjCache == null || ObjCache.Count() == 0)
            {
                using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {

                    var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID), 
                                                       new SqlParameter("@CompanyID", CompanyID), 
                                                       new SqlParameter("@RoomID", RoomID), 
                                                       new SqlParameter("@UserID", UserID),
                                                       new SqlParameter("@ListName", ListName),
                                                       new SqlParameter("@UserUISettingType", UserUISettingType)};
                    IEnumerable<UsersUISettingsDTO> obj = context.Database.SqlQuery<UsersUISettingsDTO>("exec [GetUsersUISettingsByUserID] @EnterpriseID,@CompanyID,@RoomID,@UserID,@ListName,@UserUISettingType", params1).ToList();
                    if (obj != null && obj.Count() > 0)
                    {
                        ObjCache = CacheHelper<IEnumerable<UsersUISettingsDTO>>.AddCacheItem("Cached_UsersUISettingsDTO_" + UserID.ToString() + EnterpriseID.ToString() + CompanyID.ToString() + RoomID.ToString() + (ListName ?? string.Empty), obj);
                        return obj.FirstOrDefault();
                    } 
                    else
                    {
                        return null;
                    }

                    //IEnumerable<UsersUISettingsDTO> obj = (from u in db.UsersUISettings
                    //                                       where u.UserID == UserID && (u.EnterpriseID ?? 0) == EnterpriseID && (u.CompanyID ?? 0) == CompanyID && (u.RoomID ?? 0) == RoomID
                    //                                       select new UsersUISettingsDTO
                    //                                       {
                    //                                           ID = u.ID,
                    //                                           ListName = u.ListName,
                    //                                           UserID = u.UserID,
                    //                                           JSONDATA = u.JSONDATA,
                    //                                           EnterpriseID = u.EnterpriseID,
                    //                                           CompanyID = u.CompanyID,
                    //                                           RoomID = u.RoomID,
                    //                                       }).AsParallel().ToList();

                    //ObjCache = CacheHelper<IEnumerable<UsersUISettingsDTO>>.AddCacheItem("Cached_UsersUISettingsDTO_" + UserID.ToString() + "" + EnterpriseID.ToString() + "" + CompanyID.ToString() + "" + RoomID.ToString(), obj);
                }
            }
            else
            {
                return ObjCache.FirstOrDefault();
            }
            //UsersUISettingsDTO setting;
            //setting = ObjCache.FirstOrDefault(t => t.ListName == ListName && t.EnterpriseID == EnterpriseID && t.CompanyID == CompanyID && t.RoomID == RoomID);
            //if (setting == null || string.IsNullOrWhiteSpace(setting.JSONDATA))
            //{
            //    setting = ObjCache.FirstOrDefault(t => t.ListName == ListName);
            //}


            //return setting;
        }

        public UsersUISettingsDTO GetRecordWithoutCache(Int64 UserID, string ListName, long EnterpriseID = 0, long CompanyID = 0, long RoomID = 0)
        {
            //Get Cached-Media - Userwise
            
            using (eTurns_MasterEntities db = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {


                return (from u in db.UsersUISettings
                                                        where u.UserID == UserID && (u.EnterpriseID ?? 0) == EnterpriseID && (u.CompanyID ?? 0) == CompanyID && (u.RoomID ?? 0) == RoomID
                                                        && ListName==u.ListName
                                                        select new UsersUISettingsDTO
                                                        {
                                                            ID = u.ID,
                                                            ListName = u.ListName,
                                                            UserID = u.UserID,
                                                            JSONDATA = u.JSONDATA,
                                                            EnterpriseID = u.EnterpriseID,
                                                            CompanyID = u.CompanyID,
                                                            RoomID = u.RoomID,
                                                        }).AsParallel().FirstOrDefault();

            }
           
        }

        public UsersUISettingsDTO GetRecordWithoutCacheNg(Int64 UserID, string ListName, long EnterpriseID = 0, long CompanyID = 0, long RoomID = 0)
        {
            //Get Cached-Media - Userwise

            using (eTurns_MasterEntities db = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {


                return (from u in db.UsersUISettingsNGs
                        where u.UserID == UserID && (u.EnterpriseID ?? 0) == EnterpriseID && (u.CompanyID ?? 0) == CompanyID && (u.RoomID ?? 0) == RoomID
                        && ListName == u.ListName
                        select new UsersUISettingsDTO
                        {
                            ID = u.ID,
                            ListName = u.ListName,
                            UserID = u.UserID,
                            JSONDATA = u.JSONDATA,
                            EnterpriseID = u.EnterpriseID,
                            CompanyID = u.CompanyID,
                            RoomID = u.RoomID,
                        }).AsParallel().FirstOrDefault();

            }

        }

        //public Int64 Insert(UsersUISettingsDTO objDTO)
        //{
        //    using (eTurns_MasterEntities db = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
        //    {
        //        UsersUISetting obj = new UsersUISetting();
        //        obj.ID = objDTO.ID;
        //        obj.ListName = objDTO.ListName;
        //        obj.JSONDATA = objDTO.JSONDATA;
        //        obj.UserID = objDTO.UserID;
        //        db.UsersUISettings.Add(obj);
        //        db.SaveChanges();

        //        objDTO.ID = obj.ID;
        //        if (objDTO.ID > 0)
        //        {
        //            //Get Cached-Media
        //            IEnumerable<UsersUISettingsDTO> ObjCache = CacheHelper<IEnumerable<UsersUISettingsDTO>>.GetCacheItem("Cached_UsersUISettingsDTO_" + objDTO.UserID.ToString());
        //            if (ObjCache != null)
        //            {
        //                List<UsersUISettingsDTO> tempC = new List<UsersUISettingsDTO>();
        //                tempC.Add(objDTO);

        //                IEnumerable<UsersUISettingsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //                CacheHelper<IEnumerable<UsersUISettingsDTO>>.AppendToCacheItem("Cached_UsersUISettingsDTO_" + objDTO.UserID.ToString(), NewCache);
        //            }
        //        }
        //        return obj.ID;
        //    }
        //}

        //public bool Edit(UsersUISettingsDTO objDTO)
        //{
        //    using (eTurns_MasterEntities db = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
        //    {
        //        UsersUISetting obj = db.UsersUISettings.SingleOrDefault(t => t.UserID == objDTO.UserID && t.ListName == objDTO.ListName);
        //        if (obj == null)
        //        {
        //            Int64 ID = Insert(objDTO);
        //            obj = db.UsersUISettings.SingleOrDefault(t => t.UserID == objDTO.UserID && t.ListName == objDTO.ListName);
        //        }

        //        obj.ListName = objDTO.ListName;
        //        obj.JSONDATA = objDTO.JSONDATA;
        //        obj.UserID = objDTO.UserID;
        //        db.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
        //        db.SaveChanges();

        //        objDTO.ID = obj.ID;

        //        //Get Cached-Media
        //        IEnumerable<UsersUISettingsDTO> ObjCache = CacheHelper<IEnumerable<UsersUISettingsDTO>>.GetCacheItem("Cached_UsersUISettingsDTO_" + objDTO.UserID.ToString());
        //        if (ObjCache != null)
        //        {
        //            List<UsersUISettingsDTO> objTemp = ObjCache.ToList();
        //            objTemp.RemoveAll(i => i.ID == objDTO.ID);
        //            ObjCache = objTemp.AsEnumerable();

        //            List<UsersUISettingsDTO> tempC = new List<UsersUISettingsDTO>();
        //            tempC.Add(objDTO);
        //            IEnumerable<UsersUISettingsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
        //            CacheHelper<IEnumerable<UsersUISettingsDTO>>.AppendToCacheItem("Cached_UsersUISettingsDTO_" + objDTO.UserID.ToString(), NewCache);
        //        }

        //        return true;
        //    }

        //}

        public UsersUISettingsDTO SaveUserListViewSettings(UsersUISettingsDTO objDTO, string UserUISettingType,bool isNeedtoInsertInQueue = false)
        {
            UsersUISettingsDTO objUsersUISettingsDTO = new UsersUISettingsDTO();
            UsersUISetting objUsersUISetting = new UsersUISetting();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@UserID", objDTO.UserID),
                    new SqlParameter("@ListName", objDTO.ListName),
                    new SqlParameter("@JSONDATA", objDTO.JSONDATA),
                    new SqlParameter("@EnterpriseID", objDTO.EnterpriseID),
                    new SqlParameter("@CompanyID", objDTO.CompanyID),
                    new SqlParameter("@RoomID", objDTO.RoomID),
                    new SqlParameter("@UserUISettingType", UserUISettingType),
                    new SqlParameter("@isNeedtoInsertInQueue", isNeedtoInsertInQueue)
                };

                long id = context.Database
                    .SqlQuery<long>("exec InsertUpdateUsersUISetting " +
                    "@UserID,@ListName,@JSONDATA,@EnterpriseID,@CompanyID,@RoomID,@UserUISettingType,@isNeedtoInsertInQueue"
                    , para.ToArray()).FirstOrDefault();

                objDTO.ID = id;

                //objUsersUISetting = context.UsersUISettings.FirstOrDefault(t => t.UserID == objDTO.UserID && t.ListName == objDTO.ListName
                //                                                                && t.EnterpriseID == objDTO.EnterpriseID
                //                                                                && t.CompanyID == objDTO.CompanyID
                //                                                                && t.RoomID == objDTO.RoomID);
                //if (objUsersUISetting == null)
                //{
                //    objUsersUISetting = new UsersUISetting();
                //    objUsersUISetting.ListName = objDTO.ListName;
                //    objUsersUISetting.JSONDATA = objDTO.JSONDATA;
                //    objUsersUISetting.UserID = objDTO.UserID;

                //    objUsersUISetting.EnterpriseID = objDTO.EnterpriseID;
                //    objUsersUISetting.CompanyID = objDTO.CompanyID;
                //    objUsersUISetting.RoomID = objDTO.RoomID;
                //    context.UsersUISettings.AddObject(objUsersUISetting);
                //    context.SaveChanges();
                //    objDTO.ID = objUsersUISetting.ID;
                //}
                //else
                //{
                //    objUsersUISetting.JSONDATA = objDTO.JSONDATA;
                //    context.SaveChanges();
                //}
            }
            if (UserUISettingType != "Angular")
            {
                IEnumerable<UsersUISettingsDTO> ObjCache = CacheHelper<IEnumerable<UsersUISettingsDTO>>.GetCacheItem("Cached_UsersUISettingsDTO_" + objDTO.UserID.ToString() + objDTO.EnterpriseID.ToString() + objDTO.CompanyID.ToString() + objDTO.RoomID.ToString() + (objDTO.ListName ?? string.Empty));
                if (ObjCache != null)
                {
                    List<UsersUISettingsDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.UserID == objDTO.UserID && i.ListName == objDTO.ListName && i.EnterpriseID == objDTO.EnterpriseID
                                                                                && i.CompanyID == objDTO.CompanyID
                                                                                && i.RoomID == objDTO.RoomID);
                    ObjCache = objTemp.AsEnumerable();

                    List<UsersUISettingsDTO> tempC = new List<UsersUISettingsDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<UsersUISettingsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<UsersUISettingsDTO>>.AppendToCacheItem("Cached_UsersUISettingsDTO_" + objDTO.UserID.ToString() + objDTO.EnterpriseID.ToString() + objDTO.CompanyID.ToString() + objDTO.RoomID.ToString() + (objDTO.ListName ?? string.Empty), NewCache);
                }
            }
            return objDTO;

        }

        public List<UsersUISettingsDTO> GetUserUISettingForCurrentRoom(long UserId, long EnterpriseId, long CompanyId, long RoomId)
        {
            using (eTurns_MasterEntities db = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                List<UsersUISettingsDTO> usersUISettings = (from u in db.UsersUISettings
                                                            where u.UserID == UserId && u.EnterpriseID == EnterpriseId && u.CompanyID == CompanyId && u.RoomID == RoomId && !string.IsNullOrEmpty(u.ListName)
                                                            select new UsersUISettingsDTO
                                                            {
                                                                ID = u.ID,
                                                                ListName = u.ListName,
                                                                UserID = u.UserID,
                                                                JSONDATA = u.JSONDATA,
                                                                EnterpriseID = u.EnterpriseID,
                                                                CompanyID = u.CompanyID,
                                                                RoomID = u.RoomID,
                                                            }).AsParallel().ToList();

                return usersUISettings;
            }
        }

        public List<UsersUISettingsDTO> GetUsersUISettings(string ListName)
        {
            using (eTurns_MasterEntities db = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                List<UsersUISettingsDTO> lstUsersUISettings =
                        (from u in db.UsersUISettings
                         join em in db.EnterpriseMasters on
                          u.EnterpriseID equals em.ID
                         join cmp in db.MstCompanyMasters on
                            new { entID = em.ID, cmpId = u.CompanyID }
                            equals new { entID = cmp.EnterpriseID, cmpId = cmp.CompanyID }
                         join rm in db.MstRooms on
                            new { entID = em.ID, cmpId = cmp.CompanyID, u.RoomID } 
                            equals new { entID = rm.EnterpriseID, cmpId = rm.CompanyID, rm.RoomID }
                         where em.IsDeleted == false
                               && cmp.IsDeleted == false
                               && rm.IsDeleted == false
                               && ListName == u.ListName
                         orderby u.UserID, u.EnterpriseID, u.CompanyID, u.RoomID
                         select new UsersUISettingsDTO
                         {
                             ID = u.ID,
                             ListName = u.ListName,
                             UserID = u.UserID,
                             JSONDATA = u.JSONDATA,
                             EnterpriseID = u.EnterpriseID,
                             CompanyID = u.CompanyID,
                             RoomID = u.RoomID,
                         }).ToList();
                return lstUsersUISettings;
            }
        }

        public UsersUISettingsDTO GetUsersUISettingsRecord(Int64 UserID, string ListName, string UserUISettingType, long EnterpriseID = 0, long CompanyID = 0, long RoomID = 0)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID),
                                                       new SqlParameter("@CompanyID", CompanyID),
                                                       new SqlParameter("@RoomID", RoomID),
                                                       new SqlParameter("@UserID", UserID),
                                                       new SqlParameter("@ListName", ListName),
                                                       new SqlParameter("@UserUISettingType", UserUISettingType)};
                return context.Database.SqlQuery<UsersUISettingsDTO>("exec [GetUsersUISettingsRecord] @EnterpriseID,@CompanyID,@RoomID,@UserID,@ListName,@UserUISettingType", params1).FirstOrDefault();
            }
        }
        public List<UsersUISettingsDTO> GetAllUsersUISettingsRecordByUserID(Int64 UserID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] {new SqlParameter("@UserID", UserID)};
                return context.Database.SqlQuery<UsersUISettingsDTO>("exec [GetAllUsersUISettingsRecordByUserID] @UserID", params1).ToList();
            }
        }

        public void MigrateSiteListColumnDetailToNLF()
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC MigrateSiteListColumnDetailToNLF");
            }
        }
    }
}
