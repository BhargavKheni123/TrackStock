using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTurns.DTO;
using eTurns.DAL;

namespace eTurnsWeb.Controllers.WebAPI
{
    public class ModuleMasterController : ApiController
    {

        public ModuleMasterDAL _repository = new ModuleMasterDAL();

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<ModuleMasterDTO> GetAllRecord(Int64 EnterpriseID)
        {
            return _repository.GetAllRecords(EnterpriseID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ParentModuleMasterDTO> GetParentRecord()
        {
            return _repository.GetParentRecord();
        }

         /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<ModuleMasterDTO> GetAllModuleRecords(Int32 EnterpriseID)
        {
            return _repository.GetAllModuleRecords(EnterpriseID);
        }
        
        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ModuleMasterDTO GetRecord(Int64 id)
        {
            return _repository.GetRecord(id);
        }

        /// <summary>
        /// User for create/insert record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PostRecord(ModuleMasterDTO objDTO)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;

            try
            {
                if (ModelState.IsValid)
                {
                    _repository.Insert(objDTO);
                }
                if (Request != null)
                {
                    return Request.CreateResponse(hStatusCode, objDTO.ID);
                }
                else
                {
                    return new HttpResponseMessage(hStatusCode);
                }
            }
            catch (Exception ex)
            {
                hStatusCode = HttpStatusCode.ExpectationFailed;
                throw ex;
            }

        }

        /// <summary>
        ///PUT api/binmaster/5 
        ///use for Update Rocord
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PutRecord(Int64 id, ModuleMasterDTO objDTO)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;

            try
            {
                if (ModelState.IsValid && id == objDTO.ID)
                {
                    _repository.Edit(objDTO);

                }

                if (Request != null)
                {
                    return Request.CreateResponse(hStatusCode, objDTO);
                }
                else
                {
                    return new HttpResponseMessage(hStatusCode);
                }
            }
            catch (Exception ex)
            {
                hStatusCode = HttpStatusCode.ExpectationFailed;
                throw ex;
            }


        }

        /// <summary>
        /// DELETE api/binmaster/5
        /// Delete record
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public HttpResponseMessage Delete(Int64 ID, Int64 UserID)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;

            try
            {
                if (ModelState.IsValid)
                {
                    _repository.Delete(ID, UserID);
                }

                if (Request != null)
                {
                    return Request.CreateResponse(hStatusCode);
                }
                else
                {
                    return new HttpResponseMessage(hStatusCode);
                }
            }
            catch (Exception ex)
            {
                hStatusCode = HttpStatusCode.ExpectationFailed;
                throw ex;
            }


        }

        public string GetModuleNotification(Int32 ModuleId, Int64 RoomId, Int64 UserId)
        {
            return _repository.GetModuleNotification(ModuleId, RoomId, UserId);
        }

        #region "GET Paginations"

        // GET api/Module
        [HttpGet]
        public IEnumerable<ModuleMasterDTO> GetAllRecords(int CompanyID, int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName)
        {
            return _repository.GetPagedRecords(CompanyID, StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName);
        }

        [HttpDelete]
        public HttpResponseMessage DeleteRecords(string IDs, int UserID)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;

            try
            {
                if (ModelState.IsValid)
                {
                    _repository.DeleteRecords(IDs, UserID);
                }

                if (Request != null)
                {
                    return Request.CreateResponse(hStatusCode);
                }
                else
                {
                    return new HttpResponseMessage(hStatusCode);
                }
            }
            catch (Exception ex)
            {
                hStatusCode = HttpStatusCode.ExpectationFailed;
                throw ex;
            }


        }

        [HttpPut]
        public HttpResponseMessage PutUpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {

            HttpStatusCode hStatusCode = HttpStatusCode.OK;

            try
            {
                _repository.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
                if (Request != null)
                {
                    return Request.CreateResponse(hStatusCode, value);
                }
                else
                {
                    return new HttpResponseMessage(hStatusCode);
                }
            }
            catch (Exception ex)
            {
                hStatusCode = HttpStatusCode.ExpectationFailed;
                throw ex;
            }
        }


        [HttpGet]
        public string DuplicateCheck(string ModuleName, string ActionMode, Int64 ID, Int64 RoomID, Int64 CompanyID)
        {
            //return _repository.DuplicateCheck(ModuleName, ActionMode, ID);
            CommonDAL objCDal = new CommonDAL();
            return objCDal.DuplicateCheck(ModuleName, ActionMode, ID, "ModuleMaster", "ModuleName",  RoomID, CompanyID);
        }

        #endregion
    }
}
