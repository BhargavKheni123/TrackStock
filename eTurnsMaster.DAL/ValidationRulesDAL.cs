using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using eTurns.DTO;

namespace eTurnsMaster.DAL
{
    public class ValidationRulesDAL : eTurnsMasterBaseDAL, IDisposable
    {

        public void InsertUpdate(ValidationRulesDTO masterDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@ID", masterDTO.ID),
                     new SqlParameter("@EnterpriseID", masterDTO.EnterpriseID),
                    new SqlParameter("@CompanyID", masterDTO.CompanyID),
                    new SqlParameter("@RoomID", masterDTO.RoomID),
                    //new SqlParameter("@DTOName", masterDTO.DTOName.Trim()),
                    new SqlParameter("@DTOProperty", masterDTO.DTOProperty.Trim()),
                    new SqlParameter("@IsRequired", masterDTO.IsRequired),
                    new SqlParameter("@ValidationModuleID", masterDTO.ValidationModuleID),
                    new SqlParameter("@DisplayOrder", masterDTO.DisplayOrder.ToDBNull()),
                    new SqlParameter("@CreatedBy", masterDTO.CreatedBy),
                    new SqlParameter("@Created", masterDTO.Created)
                };


                var result = context.Database.SqlQuery<object>("exec [InsertUpdateValidationRules] @ID,@EnterpriseID,@CompanyID,@RoomID,@DTOProperty,@RuleType,@ValidationModuleID,@DisplayOrder,@Created"
                    , para.ToArray()).FirstOrDefault();
            }
        }

        public ValidationRulesDTO GetById(long enterpriseID, long companyID, long roomID, long id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@EnterpriseID", enterpriseID),
                    new SqlParameter("@CompanyID", companyID),
                    new SqlParameter("@RoomID", roomID),
                    new SqlParameter("@DTOName", DBNull.Value),
                    new SqlParameter("@ID", id),
                    new SqlParameter("@ValidationModuleID", DBNull.Value),
                };

                var result = context.Database.SqlQuery<ValidationRulesDTO>("exec [GetValidationRulesByDTO] @EnterpriseID,@CompanyID,@RoomID,@DTOName,@ID,@ValidationModuleID"
                    , para.ToArray()).FirstOrDefault();

                return result;
            }
        }

        public List<ValidationRulesDTO> GetByModuleId(long enterpriseID, long companyID, long roomID, int moduleId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@EnterpriseID", enterpriseID),
                    new SqlParameter("@CompanyID", companyID),
                    new SqlParameter("@RoomID", roomID),
                    new SqlParameter("@DTOName", DBNull.Value),
                    new SqlParameter("@ID", DBNull.Value),
                    new SqlParameter("@ValidationModuleID", moduleId),
                };

                var result = context.Database.SqlQuery<ValidationRulesDTO>("exec [GetValidationRulesByDTO] @EnterpriseID,@CompanyID,@RoomID,@DTOName,@ID,@ValidationModuleID"
                    , para.ToArray()).ToList();

                return result;
            }
        }

        /// <summary>
        /// Insert or Update List of Rules
        /// </summary>
        /// <param name="rulesDTOs"></param>
        /// <param name="createdBy"></param>
        /// <param name="created"></param>
        /// <returns></returns>
        public bool SaveRules(List<ValidationRulesDTO> rulesDTOs,long createdBy,DateTime created)
        {
            bool isSave = false;
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {

                string rulesXML = rulesDTOs.ToXML<ValidationRulesDTO>();

                var para = new List<SqlParameter> {
                    new SqlParameter("@RulesXML", rulesXML),
                    new SqlParameter("@CreatedBy", createdBy),
                    new SqlParameter("@Created", created),
                };

                var result = context.Database.SqlQuery<string>("exec [SaveValidationRules] @RulesXML,@CreatedBy,@Created"
                    , para.ToArray()).FirstOrDefault();

                isSave = result == "ok";
            }

            return isSave;
        }

        public bool Delete(long id,long roomID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new SqlParameter[] {
                    new SqlParameter("@ID", id),
                    new SqlParameter("@RoomID", roomID),  
                };

                var result = context.Database.ExecuteSqlCommand("exec [DeleteValidationRules] @ID,@RoomID"
                    , para);

                return true;
            }
        }

        #region IDisposable

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ValidationRulesDAL()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
