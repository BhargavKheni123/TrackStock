using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace eTurnsWeb.Controllers.WebAPI.EturnsMaster
{
    public class UserMasterController : ApiController
    {

        public eTurnsMaster.DAL.UserMasterDAL _repository = new eTurnsMaster.DAL.UserMasterDAL();

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<UserMasterDTO> GetAllUsers()
        {
            return new List<UserMasterDTO>();
        }
        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<UserMasterDTO> GetAllRecord()
        {
            return new List<UserMasterDTO>();//return _repository.GetAllRecords();
        }


        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserMasterDTO GetRecord(Int64 id)
        {
            return new UserMasterDTO();//return _repository.GetRecord(id);
        }

        public UserMasterDTO CheckAuthantication(string Email, string Password)
        {
            return new UserMasterDTO();//return _repository.CheckAuthantication(Email, Password);
        }

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<UserRoleModuleDetailsDTO> GetUserRoleModuleDetailsRecord(Int64 id, Int64 RoleID, long UserType)
        {
            return new List<UserRoleModuleDetailsDTO>();//return _repository.GetUserRoleModuleDetailsRecord(id, RoleID, UserType);
        }
        //public UserMasterDTO GetHistoryRecord(Int64 id)
        //{
        //    return _repository.GetHistoryRecord(id);
        //}


        /// <summary>
        /// User for create/insert record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PostRecord(UserMasterDTO objDTO)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;
            return new HttpResponseMessage(hStatusCode);
        }

        /// <summary>
        ///PUT api/binmaster/5 
        ///use for Update Rocord
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PutRecord(Int64 id, UserMasterDTO objDTO)
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
            return new HttpResponseMessage(hStatusCode);
        }

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<UserMasterDTO> GetModuleWiseUsers(Int64 ModuleID)
        {
            return new List<UserMasterDTO>(); //return _repository.GetModuleWiseUsers(ModuleID);
        }

        #region "GET Paginations"

        // GET api/User
        [HttpGet]
        public IEnumerable<UserMasterDTO> GetAllRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName)
        {
            TotalCount = 0;
            return new List<UserMasterDTO>(); //return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName);
        }

        [HttpDelete]
        public HttpResponseMessage DeleteRecords(string IDs, Int64 UserID)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;
            return new HttpResponseMessage(hStatusCode);
        }

        [HttpPut]
        public HttpResponseMessage PutUpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;
            return new HttpResponseMessage(hStatusCode);
        }


        [HttpGet]
        public string DuplicateCheck(string UserName, string ActionMode, string FieldName, Int64 ID)
        {
            //return _repository.DuplicateCheck(UserName, ActionMode, ID);
            eTurnsMaster.DAL.CommonMasterDAL objCDal = new eTurnsMaster.DAL.CommonMasterDAL();
            return objCDal.DuplicateCheck(UserName, ActionMode, ID, "UserMaster", FieldName);
        }

        #endregion
    }
}
