using eTurnsMaster.DAL;
using eTurns.DTO;
using eTurnsWeb.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTurnsWeb.BAL
{
    public class ValidationRulesMasterBAL : IDisposable
    {
        public void InsertUpdate(ValidationRulesMasterDTO masterDTO)
        {
            
            if (masterDTO.DTOName == "None")
            {
                masterDTO.DTOName = "";
            }

            CacheHelper<IEnumerable<ValidationRulesDTO>>.InvalidateCache();
            using (ValidationRulesMasterDAL masterDAL = new ValidationRulesMasterDAL())
            {
                masterDAL.InsertUpdate(masterDTO);
            }
        }

        //public List<ValidationRulesMasterDTO> GetByDTOName(EturnsDTOEnum DTOName)
        //{
        //    using (ValidationRulesMasterDAL masterDAL = new ValidationRulesMasterDAL())
        //    {
        //        return masterDAL.GetByDTOName(DTOName);
        //    }
        //}

        public List<ValidationRulesMasterDTO> GetByModuleID(int moduleId)
        {
            using (ValidationRulesMasterDAL masterDAL = new ValidationRulesMasterDAL())
            {
                return masterDAL.GetByModuleID(moduleId);
            }
        }

        public ValidationRulesMasterDTO GetByID(long id)
        {
            using (ValidationRulesMasterDAL masterDAL = new ValidationRulesMasterDAL())
            {
                return masterDAL.GetByID(id);
            }
        }

        public bool Delete(long id)
        {
            CacheHelper<IEnumerable<ValidationRulesDTO>>.InvalidateCache();
            using (ValidationRulesMasterDAL masterDAL = new ValidationRulesMasterDAL())
            {
                return masterDAL.Delete(id);
            }
        }

        public int SyncRoomRules(int moduleId, DateTime created, long createdBy)
        {
            CacheHelper<IEnumerable<ValidationRulesDTO>>.InvalidateCache();
            using (ValidationRulesMasterDAL masterDAL = new ValidationRulesMasterDAL())
            {
                return masterDAL.SyncRoomRules(moduleId, created, createdBy);
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
        // ~ValidationRulesMasterBAL()
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