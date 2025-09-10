using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using eTurns.DTO;

namespace eTurnsMaster.DAL
{
    public class ValidationRulesMasterDAL : eTurnsMasterBaseDAL,IDisposable
    {
        private bool disposedValue;

        public void InsertUpdate(ValidationRulesMasterDTO masterDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@ID", masterDTO.ID),
                    new SqlParameter("@DTOProperty", masterDTO.DTOProperty.Trim()),
                    new SqlParameter("@IsRequired", masterDTO.IsRequired),
                    new SqlParameter("@ResourceFileName", masterDTO.ResourceFileName.ToDBNull()),
                    new SqlParameter("@DisplayOrder", masterDTO.DisplayOrder.ToDBNull()),
                    new SqlParameter("@ValidationModuleID", masterDTO.ValidationModuleID),                    
                    //new SqlParameter("@Created", masterDTO.Created)
                };


                var result = context.Database.SqlQuery<object>("exec [InsertUpdateValidationRulesMaster] @ID,@DTOProperty,@IsRequired,@ResourceFileName,@DisplayOrder,@ValidationModuleID"
                    , para.ToArray()).FirstOrDefault();
            }
        }

        //public List<ValidationRulesMasterDTO> GetByDTOName(EturnsDTOEnum DTOName)
        //{
        //    using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
        //    {
        //        var para = new List<SqlParameter> {
        //            new SqlParameter("@DTOName", DTOName.ToString()),
        //            new SqlParameter("@ID", DBNull.Value),
        //            new SqlParameter("@ValidationModuleID", DBNull.Value),
        //        };

        //        var result = context.Database.SqlQuery<ValidationRulesMasterDTO>("exec [GetValidationRulesMasterByDTO] @DTOName,@ID"
        //            , para.ToArray()).ToList();

        //        return result;
        //    }
        //}

        public ValidationRulesMasterDTO GetByID(long id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@DTOName", DBNull.Value),
                    new SqlParameter("@ID", id),
                    new SqlParameter("@ValidationModuleID", DBNull.Value),
                };

                var result = context.Database.SqlQuery<ValidationRulesMasterDTO>("exec [GetValidationRulesMasterByDTO] @DTOName,@ID,@ValidationModuleID"
                    , para.ToArray()).FirstOrDefault();

                return result;
            }
        }

        public List<ValidationRulesMasterDTO> GetByModuleID(long moduleId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@DTOName", DBNull.Value),
                    new SqlParameter("@ID", DBNull.Value),
                    new SqlParameter("@ValidationModuleID", moduleId),
                };

                var result = context.Database.SqlQuery<ValidationRulesMasterDTO>("exec [GetValidationRulesMasterByDTO] @DTOName,@ID,@ValidationModuleID"
                    , para.ToArray()).ToList();

                return result;
            }
        }

        public bool Delete(long id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new SqlParameter[] {
                    new SqlParameter("@ID", id),
                };

                var result = context.Database.ExecuteSqlCommand("exec [DeleteValidationRulesMaster] @ID"
                    , para);

                return true;
            }
        }

        public int SyncRoomRules(int validationModuleID,DateTime created,long createdBy)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new SqlParameter[] {
                    new SqlParameter("@ValidationModuleID",validationModuleID),
                    new SqlParameter("@Created",created),
                    new SqlParameter("@CreatedBy",createdBy)
                };

                int roomsUpdated = context.Database.SqlQuery<int>("exec [SyncRoomValidationRules] @ValidationModuleID,@Created,@CreatedBy"
                    , para).FirstOrDefault();

                return roomsUpdated;
            }
        }

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
        // ~ValidationRulesMasterDAL()
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
