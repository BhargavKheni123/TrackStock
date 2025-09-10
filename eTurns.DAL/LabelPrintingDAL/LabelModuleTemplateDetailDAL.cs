using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO.LabelPrinting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class LabelModuleTemplateDetailDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        //public LabelModuleTemplateDetailDAL(base.DataBaseName)
        //{

        //}

        public LabelModuleTemplateDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public LabelModuleTemplateDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>

        public IEnumerable<LabelModuleTemplateDetailDTO> GetLabelModuleTemplateDetailByModuleID(Int64 ModuleID)
        {
            List<LabelModuleTemplateDetailDTO> lstDTO = new List<LabelModuleTemplateDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ModuleID", ModuleID) };

                lstDTO = (from u in context.Database.SqlQuery<LabelModuleTemplateDetailDTO>("EXEC dbo.GetLabelModuleTemplateDetailByModuleID @ModuleID", params1)
                          select new LabelModuleTemplateDetailDTO
                          {
                              ID = u.ID,
                              ModuleID = u.ModuleID,
                              TemplateDetailID = u.TemplateDetailID,
                              CompanyID = u.CompanyID,
                              CreatedBy = u.CreatedBy,
                              UpdatedBy = u.UpdatedBy,
                              CreatedOn = u.CreatedOn,
                              UpdatedOn = u.UpdatedOn,
                              CreatedByName = u.CreatedByName,
                              UpdatedByName = u.UpdatedByName,
                              RoomID = u.RoomID
                          }).AsParallel().ToList();
            }

            return lstDTO;

        }

        public IEnumerable<LabelModuleTemplateDetailDTO> GetLabelModuleTemplateDetailByRoomIDCompanyID(Int64 CompanyId, Int64 RoomID)
        {
            List<LabelModuleTemplateDetailDTO> lstDTO = new List<LabelModuleTemplateDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID)
                                                  ,new SqlParameter("@CompanyID", CompanyId) };

                lstDTO = (from u in context.Database.SqlQuery<LabelModuleTemplateDetailDTO>("EXEC dbo.GetLabelModuleTemplateDetailByRoomIDCompanyID @RoomID,@CompanyID", params1)
                                                                                      select new LabelModuleTemplateDetailDTO
                                                                                      {
                                                                                          ID = u.ID,
                                                                                          ModuleID = u.ModuleID,
                                                                                          TemplateDetailID = u.TemplateDetailID,
                                                                                          CompanyID = u.CompanyID,
                                                                                          CreatedBy = u.CreatedBy,
                                                                                          UpdatedBy = u.UpdatedBy,
                                                                                          CreatedOn = u.CreatedOn,
                                                                                          UpdatedOn = u.UpdatedOn,
                                                                                          CreatedByName = u.CreatedByName,
                                                                                          UpdatedByName = u.UpdatedByName,
                                                                                          RoomID = u.RoomID
                                                                                      }).AsParallel().ToList();
            }

            return lstDTO;

        }

        public IEnumerable<LabelModuleTemplateDetailDTO> GetLabelModuleTemplateDetailByRoomCompanyModuleID(Int64 CompanyId, Int64 RoomID, Int64 ModuleID)
        {
            List<LabelModuleTemplateDetailDTO> lstDTO = new List<LabelModuleTemplateDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID)
                                                  ,new SqlParameter("@CompanyID", CompanyId)
                                                  ,new SqlParameter("@ModuleID", ModuleID)};

                lstDTO = (from u in context.Database.SqlQuery<LabelModuleTemplateDetailDTO>("EXEC dbo.GetLabelModuleTemplateDetailByRoomCompanyModuleID @RoomID,@CompanyID,@ModuleID", params1)
                          select new LabelModuleTemplateDetailDTO
                          {
                              ID = u.ID,
                              ModuleID = u.ModuleID,
                              TemplateDetailID = u.TemplateDetailID,
                              CompanyID = u.CompanyID,
                              CreatedBy = u.CreatedBy,
                              UpdatedBy = u.UpdatedBy,
                              CreatedOn = u.CreatedOn,
                              UpdatedOn = u.UpdatedOn,
                              CreatedByName = u.CreatedByName,
                              UpdatedByName = u.UpdatedByName,
                              RoomID = u.RoomID
                          }).AsParallel().ToList();
            }

            return lstDTO;

        }

        /// <summary>
        /// Insert Record in the DataBase LabelModuleTemplateDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(LabelModuleTemplateDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                LabelModuleTemplateDetail obj = new LabelModuleTemplateDetail();
                obj.ID = 0;
                obj.ModuleID = objDTO.ModuleID;
                obj.TemplateDetailID = objDTO.TemplateDetailID;
                obj.CompanyID = objDTO.CompanyID;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.UpdatedBy = objDTO.CreatedBy;
                obj.CreatedOn = DateTimeUtility.DateTimeNow;
                obj.UpdatedOn = DateTimeUtility.DateTimeNow;
                obj.RoomID = objDTO.RoomID;
                context.LabelModuleTemplateDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                return obj.ID;
            }

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(LabelModuleTemplateDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                LabelModuleTemplateDetail obj = context.LabelModuleTemplateDetails.FirstOrDefault(x => x.ModuleID == objDTO.ModuleID && x.CompanyID == objDTO.CompanyID && x.RoomID == objDTO.RoomID);
                obj.TemplateDetailID = objDTO.TemplateDetailID;
                obj.UpdatedBy = objDTO.UpdatedBy;
                obj.UpdatedOn = DateTimeUtility.DateTimeNow;

                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.LabelModuleTemplateDetails.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return true;
            }
        }

        public bool Delete(Int64 ModuleID, Int64 UserID, Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                LabelModuleTemplateDetail obj = context.LabelModuleTemplateDetails.FirstOrDefault(x => x.ModuleID == ModuleID && x.CompanyID == CompanyID && x.RoomID == RoomID);
                if (obj != null)
                {
                    context.LabelModuleTemplateDetails.Remove(obj);
                    context.SaveChanges();
                }
            }
            return true;
        }


        public LabelModuleTemplateDetailDTO GetAllRecordByModuleID(Int64 CompanyID, Int64 RoomID, Int64 ModuleID)
        {
            LabelModuleTemplateDetailDTO objLabelModuleTemplateDetailDTO = new LabelModuleTemplateDetailDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@ModuleID", ModuleID) };
                objLabelModuleTemplateDetailDTO = (from u in context.Database.SqlQuery<LabelModuleTemplateDetailDTO>("exec GetAllRecordByModuleID @CompanyID,@RoomID,@ModuleID", params1)
                                                   select new LabelModuleTemplateDetailDTO
                                                   {
                                                       ID = u.ID,
                                                       ModuleID = u.ModuleID,
                                                       TemplateDetailID = u.TemplateDetailID,
                                                       CompanyID = u.CompanyID,
                                                       CreatedBy = u.CreatedBy,
                                                       UpdatedBy = u.UpdatedBy,
                                                       CreatedOn = u.CreatedOn,
                                                       UpdatedOn = u.UpdatedOn,
                                                       CreatedByName = u.CreatedByName,
                                                       UpdatedByName = u.UpdatedByName,
                                                       RoomID = u.RoomID
                                                   }).AsParallel().FirstOrDefault();
            }


            return objLabelModuleTemplateDetailDTO;
        }

        public Int64 ABSetAsDefaultTemplateForModule(long CompanyID, long RoomID, long UserID,string TemplateName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID),
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@UserID", UserID),
                                               new SqlParameter("@TemplateName", TemplateName)};
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<Int32>("exec [ABSetAsDefaultTemplateForModule] @CompanyID,@RoomID,@UserID,@TemplateName", params1).FirstOrDefault();
            }
        }
    }
}


