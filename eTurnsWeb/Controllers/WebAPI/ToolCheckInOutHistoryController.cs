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
    public class ToolCheckInOutHistoryController : ApiController
    {

        public ToolCheckInOutHistoryDAL _repository = new ToolCheckInOutHistoryDAL();

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<ToolCheckInOutHistoryDTO> GetAllRecord(int roomID,Int32 EnterpriseID)
        {
            return _repository.GetAllRecords(roomID,EnterpriseID);
        }


        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ToolCheckInOutHistoryDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return _repository.GetRecord(id, RoomID, CompanyID);
        }

        public ToolCheckInOutHistoryDTO GetHistoryRecord(Int64 id)
        {
            return _repository.GetHistoryRecord(id);
        }


        /// <summary>
        /// User for create/insert record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PostRecord(ToolCheckInOutHistoryDTO objDTO)
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
        ///PUT api/ToolCheckInOutHistory/5 
        ///use for Update Rocord
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PutRecord(Int64 id, ToolCheckInOutHistoryDTO objDTO)
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

        // GET api/ToolCheckInOutHistory
        [HttpGet]
        public IEnumerable<ToolCheckInOutHistoryDTO> GetAllRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID,Int64 CompanyID, bool IsArchived, bool IsDeleted,Int64 ToolID)
        {
            return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID,CompanyID, IsArchived,IsDeleted,ToolID);
        }

        //[HttpDelete]
        //public HttpResponseMessage DeleteRecords(string IDs, Int64 UserID, Int64 CompanyId,Int64 RoomID,out string MSG)
        //{
        //    HttpStatusCode hStatusCode = HttpStatusCode.OK;

        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _repository.DeleteRecords(IDs, UserID, CompanyId, RoomID, out MSG);
        //        }

        //        if (Request != null)
        //        {
        //            return Request.CreateResponse(hStatusCode);
        //        }
        //        else
        //        {
        //            return new HttpResponseMessage(hStatusCode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        hStatusCode = HttpStatusCode.ExpectationFailed;
        //        throw ex;
        //    }


        //}


        [HttpGet]
        public string DuplicateCheck(string Name,string ActionMode,Int64 ID,Int64 RoomID,Int64 CompanyID)
        {
            //return _repository.DuplicateCheck(CheckOutStatus,ActionMode,ID);
            CommonDAL objCDal = new CommonDAL();
            return objCDal.DuplicateCheck(Name, ActionMode, ID, "ToolCheckInOutHistory", "CheckOutStatus",RoomID,CompanyID);
        }

        #endregion
    }
}


