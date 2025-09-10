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
    public class PullMasterController : ApiController
    {

        public PullMasterDAL _repository = new PullMasterDAL();

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<PullMasterViewDTO> GetAllRecord(int roomID, Int32 EnterpriseID)
        {
            return _repository.GetAllRecords(roomID, EnterpriseID);
        }


        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PullMasterViewDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return _repository.GetRecord(id, RoomID, CompanyID);
        }

        public PullMasterDTO GetHistoryRecord(Int64 id)
        {
            return _repository.GetHistoryRecord(id);
        }


        /// <summary>
        /// User for create/insert record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PostRecord(PullMasterViewDTO objDTO)
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
        ///PUT api/PullMaster/5 
        ///use for Update Rocord
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PutRecord(Int64 id, PullMasterViewDTO objDTO)
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



        #region "GET Paginations"

        // GET api/PullMaster
        [HttpGet]
        public IEnumerable<PullMasterViewDTO> GetAllRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted);
        }

        [HttpDelete]
        //public HttpResponseMessage DeleteRecords(string IDs, Int64 UserID, Int64 CompanyId,Int64 RoomID, out string MSG)
        public HttpResponseMessage DeleteRecords(string IDs, Int64 UserID, Int64 CompanyId,Int64 RoomID)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;

            try
            {
                if (ModelState.IsValid)
                {
                    //_repository.DeleteRecords(IDs, UserID, CompanyId,RoomID, out MSG);
                    _repository.DeleteRecords(IDs, UserID, CompanyId, RoomID);
                }

                if (Request != null)
                {
                    //MSG = "ok";
                    return Request.CreateResponse(hStatusCode);
                }
                else
                {
                    //MSG = "error";
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
        public string DuplicateCheck(string Name, string ActionMode, Int64 ID, Int64 RoomID, Int64 CompanyID)
        {
            //return _repository.DuplicateCheck(UOI,ActionMode,ID);
            CommonDAL objCDal = new CommonDAL();
            return objCDal.DuplicateCheck(Name, ActionMode, ID, "PullMaster", "UOI", RoomID, CompanyID);

        }

        #endregion
    }
}


