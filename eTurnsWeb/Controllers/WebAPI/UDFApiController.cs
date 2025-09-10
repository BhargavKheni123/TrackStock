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
    public class UDFApiController : ApiController
    {
        public UDFDAL _repository = new UDFDAL();

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UDFDTO GetRecord(Int64 id, Int64 CompanyID)
        {
            return _repository.GetRecord(id, CompanyID);
        }

        /// <summary>
        /// User for create/insert record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PostRecord(UDFDTO objDTO)
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
        ///PUT api/toolmaster/5 
        ///use for Update Rocord
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PutRecord(Int64 id, UDFDTO objDTO)
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
        /// DELETE api/toolmaster/5
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


        // GET api/tool
        [HttpGet]
        public IEnumerable<UDFDTO> GetAllRecords(Int64 CompanyID, string UDFTableName)
        {
            return _repository.GetAllRecords(CompanyID, UDFTableName);
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


        [HttpGet]
        public IEnumerable<UDFDTO> GetPagedRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, Int64 CompanyID, string UDFTableName)
        {
            return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, CompanyID, UDFTableName);
        }

        [HttpGet]
        public bool InsertDefaultUDFs(string UDFTableName, Int64 CompanyID, Int64 UserID)
        {
            return _repository.InsertDefaultUDFs(UDFTableName, CompanyID, UserID);
        }
    }

    public class UDFOptionApiController : ApiController
    {
        public UDFOptionDAL _repository = new UDFOptionDAL();

        public IEnumerable<UDFOptionsDTO> GetUDFOptionsByUDF(Int64 UDFID, Int64 CompanyID)
        {
            return _repository.GetUDFOptionsByUDF(UDFID, CompanyID);
        }

        /// <summary>
        /// User for create/insert record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PostRecord(UDFOptionsDTO objDTO)
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
        ///PUT api/toolmaster/5 
        ///use for Update Rocord
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PutRecord(Int64 id, UDFOptionsDTO objDTO)
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
        /// DELETE api/toolmaster/5
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
        /// Edit Method
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UDFOption"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public bool Edit(Int64 ID, string UDFOption, Int64 UserID, Int64 CompanyID)
        {
            return _repository.Edit(ID, UDFOption, UserID, CompanyID);
        }

    }
}