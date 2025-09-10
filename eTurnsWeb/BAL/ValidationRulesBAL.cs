using eTurnsMaster.DAL;
using eTurns.DTO;
using eTurnsWeb.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace eTurnsWeb.BAL
{
    public class ValidationRulesBAL : IDisposable
    {
        private bool disposedValue;

        public void InsertUpdate(ValidationRulesDTO masterDTO)
        {
           
            if (masterDTO.DTOName == "None")
            {
                masterDTO.DTOName = "";
            }
            CacheHelper<IEnumerable<ValidationRulesDTO>>.InvalidateCache();
            using (ValidationRulesDAL rulesDAL = new ValidationRulesDAL())
            {
                rulesDAL.InsertUpdate(masterDTO);
            }
        }

        /// <summary>
        /// Get room wise dto , if not found then get from master list
        /// </summary>
        /// <param name="enterpriseID"></param>
        /// <param name="companyID"></param>
        /// <param name="roomID"></param>
        /// <param name="DTOName"></param>
        /// <returns></returns>
        public List<ValidationRulesDTO> GetByModuleId(long enterpriseID, long companyID, long roomID, int moduleId)
        {
            List<ValidationRulesDTO> objList = null;
            using (ValidationRulesDAL rulesDAL = new ValidationRulesDAL())
            {
                objList = rulesDAL.GetByModuleId(enterpriseID, companyID, roomID, moduleId);
            }

            if (objList == null || objList.Count() == 0)
            {
                // if room wise dto list not found then get from master list
                using (ValidationRulesMasterBAL rulesMasBAL = new ValidationRulesMasterBAL())
                {
                    var objMasList = rulesMasBAL.GetByModuleID(moduleId);
                    if (objMasList != null && objMasList.Count > 0)
                    {
                        objList = new List<ValidationRulesDTO>();

                        foreach (var objM in objMasList)
                        {
                            ValidationRulesDTO rulesDTO = new ValidationRulesDTO(objM, enterpriseID, companyID, roomID);
                            objList.Add(rulesDTO);
                        }
                    }
                }
            }

            if (objList == null)
            {
                objList = new List<ValidationRulesDTO>();
            }

            return objList;
        }


        public bool SaveRules(long enterpriseID, long companyID, long roomID, List<ValidationRulesDTO> rulesDTOs, long createdBy, DateTime created)
        {
            CacheHelper<IEnumerable<ValidationRulesDTO>>.InvalidateCache();
            using (ValidationRulesDAL rulesDAL = new ValidationRulesDAL())
            {

                foreach (var rule in rulesDTOs)
                {
                    rule.EnterpriseID = enterpriseID;
                    rule.CompanyID = companyID;
                    rule.RoomID = roomID;
                }

                return rulesDAL.SaveRules(rulesDTOs, createdBy, created);
            }
        }

        public bool Delete(long id, long roomID)
        {
            CacheHelper<IEnumerable<ValidationRulesDTO>>.InvalidateCache();
            using (ValidationRulesDAL rulesDAL = new ValidationRulesDAL())
            {
                return rulesDAL.Delete(id,roomID);
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
        // ~ValidationRulesBAL()
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