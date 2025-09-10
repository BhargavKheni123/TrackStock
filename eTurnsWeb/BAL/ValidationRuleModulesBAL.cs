using eTurns.DTO;
using eTurnsMaster.DAL;
using eTurnsWeb.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTurnsWeb.BAL
{
    public class ValidationRuleModulesBAL : IDisposable
    {
        private bool disposedValue;

        public List<ValidationRuleModulesDTO> GetAll()
        {
            using (ValidationRuleModulesDAL modulesBAL = new ValidationRuleModulesDAL())
            {
                return modulesBAL.GetAll();
            }
        }

        public ValidationRuleModulesDTO GetByDTOName(EturnsDTOEnum DTOName)
        {
            using (ValidationRuleModulesDAL modulesBAL = new ValidationRuleModulesDAL())
            {
                return modulesBAL.GetByDTOName(DTOName);
            }
        }

        public ValidationRuleModulesDTO GetByID(long id)
        {
            using (ValidationRuleModulesDAL modulesBAL = new ValidationRuleModulesDAL())
            {
                return modulesBAL.GetByID(id);
            }
        }

        public static int GetModuleIdFromEnum(string controller, string action, string ruleDTO)
        {
            EturnsDTOEnum eturnsDTO = EturnsDTOEnum.None;
            if (controller.ToLower() == "Kit".ToLower() && action.ToLower() == "CreateToolKit".ToLower() && ruleDTO == "ToolMasterDTO")
            {
                eturnsDTO = EturnsDTOEnum.ToolMasterDTO_17;
            }
            else 
            {
                bool b = Enum.TryParse<EturnsDTOEnum>(ruleDTO, out eturnsDTO);
            }
            
            return (int)eturnsDTO;
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
        // ~ValidationRuleModulesBAL()
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