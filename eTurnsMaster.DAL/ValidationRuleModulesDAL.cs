using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace eTurnsMaster.DAL
{
    public class ValidationRuleModulesDAL : eTurnsMasterBaseDAL, IDisposable
    {

        public List<ValidationRuleModulesDTO> GetAll()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@DTOName", DBNull.Value),
                    new SqlParameter("@ID", DBNull.Value),
                };

                var result = context.Database.SqlQuery<ValidationRuleModulesDTO>("exec [GetValidationRuleModules] @DTOName,@ID"
                    , para.ToArray()).ToList();

                return result;
            }
        }

        public ValidationRuleModulesDTO GetByDTOName(EturnsDTOEnum DTOName)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@DTOName", DTOName.ToString()),
                    new SqlParameter("@ID", DBNull.Value),
                };

                var result = context.Database.SqlQuery<ValidationRuleModulesDTO>("exec [GetValidationRuleModules] @DTOName,@ID"
                    , para.ToArray()).FirstOrDefault();

                return result;
            }
        }

        public ValidationRuleModulesDTO GetByID(long id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@DTOName", DBNull.Value),
                    new SqlParameter("@ID", id),
                };

                var result = context.Database.SqlQuery<ValidationRuleModulesDTO>("exec [GetValidationRuleModules] @DTOName,@ID"
                    , para.ToArray()).FirstOrDefault();

                return result;
            }
        }

        


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
        // ~ValidationModuleDAL()
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
    }
}
