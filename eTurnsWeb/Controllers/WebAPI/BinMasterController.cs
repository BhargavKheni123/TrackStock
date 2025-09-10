using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTurns.DTO;
using Elmah;
using eTurns.DAL;

namespace eTurnsWeb.Controllers.WebAPI
{
    public class BinMasterController : ApiController
    {

        /// <summary>
        /// IRepository Object using dependecy injection
        /// </summary>
        eTurns.DAL.BinMasterDAL _repository = new eTurns.DAL.BinMasterDAL();


        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<BinMasterDTO> GetAllRecord(Int64 RoomID, Int64 CompanyId)
        {
            return _repository.GetAllRecords(RoomID, CompanyId, false, false);
        }

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<BinMasterDTO> GetAllRecord(Int64 ItemID, Int64 RoomID, Int64 CompanyId)
        {
            return _repository.GetAllRecords(ItemID, RoomID, CompanyId, false, false);
        }

        /// <summary>
        /// GET api/binmaster
        /// </summary>
        /// <returns></returns>
        public IQueryable<BinMasterDTO> GetRecords(Int64 RoomID, Int64 CompanyID)
        {
            return _repository.GetRecords(RoomID, CompanyID).AsQueryable();
        }

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BinMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return _repository.GetRecord(id, RoomID, CompanyId, IsArchived, IsDeleted);
        }

        /// <summary>
        /// User for create/insert record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PostRecord(BinMasterDTO objDTO)
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
        public HttpResponseMessage PutRecord(Int64 id, BinMasterDTO objDTO)
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

        /// <summary>
        /// Delete multiple record with passing by comma sapereated IDs
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpDelete]
        public HttpResponseMessage DeleteRecords(string IDs, Int64 UserID, Int64 CompanyId)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;

            try
            {
                if (ModelState.IsValid)
                {
                    _repository.DeleteRecords(IDs, UserID, CompanyId);
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

        /// <summary>
        /// Get Records for paging.
        /// </summary>
        /// <param name="StartRowIndex"></param>
        /// <param name="MaxRows"></param>
        /// <param name="TotalCount"></param>
        /// <param name="SearchTerm"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<BinMasterDTO> GetPagedRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortOrder, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortOrder, RoomID, CompanyId, IsArchived, IsDeleted);
        }

        /// <summary>
        /// Update Record.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        [HttpPut]
        public HttpResponseMessage PutUpdateData(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
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

        /// <summary>
        /// DuplicateCheck
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ActionMode"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet]
        public string DuplicateCheck(string Name, string ActionMode, Int64 ID, Int64 RoomID, Int64 EnterpriseID)
        {
            CommonDAL objCDal = new CommonDAL();
            return objCDal.DuplicateCheck(Name, ActionMode, ID, "BinMaster", "BinNumber",RoomID,EnterpriseID);
        }

        ///// <summary>
        ///// Dispose
        ///// </summary>
        ///// <param name="disposing"></param>
        //protected override void Dispose(bool disposing)
        //{
        //    _repository.Dispose();
        //    base.Dispose(disposing);
        //}

    }
}
