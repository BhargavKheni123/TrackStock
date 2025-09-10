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
    public class KitDetailController : ApiController
    {

        public KitDetailDAL _repository = new KitDetailDAL();

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<KitDetailDTO> GetAllRecord(Int64 roomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsNeededItemDetail)
        {
            return _repository.GetAllRecords(roomID, CompanyID, IsArchived, IsDeleted, IsNeededItemDetail);
        }


        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public KitDetailDTO GetRecord(string GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsNeededItemDetail)
        {
            return _repository.GetRecord(GUID, RoomID, CompanyID, IsArchived, IsDeleted, IsNeededItemDetail);
        }

        public KitDetailDTO GetHistoryRecord(Int64 HistoryID)
        {
            return _repository.GetHistoryRecord(HistoryID);
        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<KitDetailDTO> GetHistoryRecordsListByKitID(string KitGUID,Int64 RoomID,Int64 CompanyID)
        {
            return _repository.GetAllHistoryRecord(KitGUID, RoomID, CompanyID);
        }

        /// <summary>
        /// User for create/insert record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PostRecord(KitDetailDTO objDTO)
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
        ///PUT api/KitDetail/5 
        ///use for Update Rocord
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PutRecord(Int64 id, KitDetailDTO objDTO)
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

        public IEnumerable<KitDetailDTO> GetAllRecordsByKitID(Int64 KitID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, bool IsNeededItemDetail)
        {
            return _repository.GetAllRecordsByKitID(KitID, RoomID, CompanyId, IsArchived, IsDeleted, IsNeededItemDetail);
        }

        #region "GET Paginations"

        // GET api/KitDetail
        [HttpGet]
        public IEnumerable<KitDetailDTO> GetAllRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID,Int64 CompanyID, bool IsArchived, bool IsDeleted,Int64 KitMasterID)
        {
            return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID,CompanyID, IsArchived,IsDeleted,KitMasterID);
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
        public string DuplicateCheck(string Name,string ActionMode,Int64 ID,Int64 RoomID,Int64 CompanyID)
        {
            //return _repository.DuplicateCheck(,ActionMode,ID);
            CommonDAL objCDal = new CommonDAL();
            return objCDal.DuplicateCheck(Name, ActionMode, ID, "KitDetail", "",RoomID,CompanyID);
        }

        #endregion
    }
}


