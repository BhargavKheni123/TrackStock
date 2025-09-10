using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO.LabelPrinting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using eTurns.DTO.Resources;

namespace eTurns.DAL.LabelPrintingDAL
{
    public class LabelModuleMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        //public LabelModuleMasterDAL(base.DataBaseName)
        //{

        //}

        public LabelModuleMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public LabelModuleMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public IEnumerable<LabelModuleMasterDTO> GetAllLabelModuleMaster()
        {
            List<LabelModuleMasterDTO> lstModuleMaster = new List<LabelModuleMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstModuleMaster = (from u in context.Database.SqlQuery<LabelModuleMasterDTO>("EXEC dbo.GetLabelModuleMaster")
                                                         select new LabelModuleMasterDTO
                                                         {
                                                             ID = u.ID,
                                                             ModuleName = ResourceRead.GetResourceValue(u.ModuleName, "ResModuleName"),
                                                             ModuleDTOName = u.ModuleDTOName,
                                                         }).AsParallel().ToList();
            }
            return lstModuleMaster;
        }

        /// <summary>
        /// Get Particullar Record from the LabelModuleMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public LabelModuleMasterDTO GetLabelModuleMasterByID(Int64 id)
        {
            LabelModuleMasterDTO  objModuleMasterDTO = new LabelModuleMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", id) };

                objModuleMasterDTO = (from u in context.Database.SqlQuery<LabelModuleMasterDTO>("EXEC dbo.GetLabelModuleMasterByID @ID", params1)
                                   select new LabelModuleMasterDTO
                                   {
                                       ID = u.ID,
                                       ModuleName = u.ModuleName,
                                       ModuleDTOName = u.ModuleDTOName,
                                   }).FirstOrDefault();
            }
            return objModuleMasterDTO;
        }

        /// <summary>
        /// GetRecord
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public LabelModuleMasterDTO GetLabelModuleMasterByName(string moduleName)
        {
            LabelModuleMasterDTO objModuleMasterDTO = new LabelModuleMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ModuleName", moduleName) };

                objModuleMasterDTO = (from u in context.Database.SqlQuery<LabelModuleMasterDTO>("EXEC dbo.GetLabelModuleMasterByName @ModuleName", params1)
                                      select new LabelModuleMasterDTO
                                      {
                                          ID = u.ID,
                                          ModuleName = u.ModuleName,
                                          ModuleDTOName = u.ModuleDTOName,
                                      }).FirstOrDefault();
            }
            return objModuleMasterDTO;
        }

        /// <summary>
        /// Insert Record in the DataBase LabelModuleMaster
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(LabelModuleMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                LabelModuleMaster obj = new LabelModuleMaster();
                obj.ID = 0;
                obj.ModuleName = objDTO.ModuleName;
                obj.ModuleDTOName = objDTO.ModuleDTOName;
                context.LabelModuleMasters.Add(obj);
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
        public bool Edit(LabelModuleMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                LabelModuleMaster obj = new LabelModuleMaster();
                obj.ID = objDTO.ID;
                obj.ModuleName = objDTO.ModuleName;
                obj.ModuleDTOName = objDTO.ModuleDTOName;
                context.LabelModuleMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();


                return true;
            }
        }

    }
}


