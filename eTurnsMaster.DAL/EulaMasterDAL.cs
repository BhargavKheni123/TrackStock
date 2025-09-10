using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using eTurnsMaster.DAL;
using System.Data.SqlClient;


namespace eTurnsMaster.DAL
{
    public partial class EulaMasterDAL : eTurnsMasterBaseDAL
    {

        #region [Class Constructor]

        public EulaMasterDAL()
        {

        }

        #endregion
        
        public IEnumerable<EulaMasterDTO> GetAllEulaMaster(bool Isdeleted,bool IsArchieved)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IsDeleted", Isdeleted), new SqlParameter("@IsArchived", IsArchieved) };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EulaMasterDTO>("exec [GetEulaMaster] @IsDeleted,@IsArchived", params1).ToList();
            }
        }

        public List<EulaMasterDTO> GetEulaMasterByNamePlain(string EulaName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@EulaName", EulaName ?? (object)DBNull.Value) };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EulaMasterDTO>("exec [GetEulaMasterByNamePlain] @EulaName", params1).ToList();
            }
        }

        public List<EulaMasterDTO> GetEulaMasterByNameOrDescriptionPlain(string EulaName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@EulaName", EulaName ?? (object)DBNull.Value) };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EulaMasterDTO>("exec [GetEulaMasterByNameOrDescriptionPlain] @EulaName", params1).ToList();
            }
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public Int64 Insert(EulaMasterDTO objDTO)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                EulaMaster obj = new EulaMaster()
                {
                    EulaDescription = objDTO.EulaDescription,
                    EulaName = objDTO.EulaName,
                    CreatedBy = objDTO.CreatedBy,
                    Created = objDTO.CreatedOn,
                    ID = 0,
                    IsArchived = false,
                    IsDeleted = false,
                    LastUpdatedBy = objDTO.UpdatedBy,
                    Updated = objDTO.UpdatedOn,
                    AddedFrom = objDTO.AddedFrom,
                    EditedFrom = objDTO.EditedFrom,
                    ReceivedOn = objDTO.ReceivedOn,
                    ReceivedOnWeb = objDTO.ReceivedOnWeb,
                };
                context.EulaMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                return objDTO.ID;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string Ids, long UserId)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserId),
                                                    new SqlParameter("@Ids", Ids)
                                                };

                context.Database.ExecuteSqlCommand("exec [UpdateEulaMaster] @UserID,@Ids", params1);
                return true;
            }
        }

        public bool InsertFileName(long Id, string fileName)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                EulaMaster objEulaMaster = context.EulaMasters.FirstOrDefault(t => t.ID == Id && t.IsDeleted == false);
                if (objEulaMaster != null)
                {
                    objEulaMaster.EulaFileName = fileName;
                    context.SaveChanges();
                }
            }
            return true;
        }
        public string GetLastestFilePath()
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                EulaMaster objEulaMaster = context.EulaMasters.OrderByDescending(e => e.ID).Where(e => e.IsDeleted == false).FirstOrDefault();
                if (objEulaMaster != null)
                {
                    return objEulaMaster.EulaFileName;
                }
            }
            return string.Empty;
        }
    }
}
