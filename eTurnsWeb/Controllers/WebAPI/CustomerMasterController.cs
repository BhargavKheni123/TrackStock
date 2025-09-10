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
    public class CustomerMasterController : ApiController
    {

        public CustomerMasterDAL _repository = new CustomerMasterDAL();


        // GET api/Customer
        public IQueryable<CustomerMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return _repository.GetAllRecords(RoomID, CompanyId,false, false).AsQueryable();
        }


        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CustomerMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return _repository.GetRecord(id, RoomID, CompanyID, IsArchived, IsDeleted);
        }

        /// <summary>
        /// User for create/insert record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PostRecord(CustomerMasterDTO objDTO)
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
        public HttpResponseMessage PutRecord(Int64 id, CustomerMasterDTO objDTO)
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


        #region "GET Paginations"

        // GET api/Customer
        [HttpGet]
        public IEnumerable<CustomerMasterDTO> GetAllRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyId, IsArchived, IsDeleted);
        }

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
        public string DuplicateCheck(string CustomerName, string ActionMode, Int64 ID, Int64 RoomID, Int64 EnterpriseID)
        {
            //return _repository.DuplicateCheck(CustomerName, Action,ID);
            CommonDAL objCDal = new CommonDAL();
            return objCDal.DuplicateCheck(CustomerName, ActionMode, ID, "CustomerMaster", "Customer",RoomID,EnterpriseID);
        }

        #endregion
    }
}
