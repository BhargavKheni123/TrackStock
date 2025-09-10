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
    public class TransferInOutQtyDetailController : ApiController
    {
        public TransferInOutQtyDetailDAL _repository = new TransferInOutQtyDetailDAL();
        
        public IEnumerable<TransferInOutQtyDetailDTO> GetAllRecord(Int64 roomID, Int32 EnterpriseID)
        {
            return _repository.GetAllRecords(roomID, EnterpriseID);
        }

        public IEnumerable<TransferInOutQtyDetailDTO> GetAllRecordForGrid(Int64 DetailID, Int64 roomID, Int64 EnterpriseID)
        {
            return _repository.GetAllRecords(DetailID,roomID, EnterpriseID);
        }
        
        public TransferInOutQtyDetailDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return _repository.GetRecord(id, RoomID, CompanyID);
        }

        public HttpResponseMessage PostRecord(TransferInOutQtyDetailDTO objDTO)
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

        public HttpResponseMessage PutRecord(Int64 id, TransferInOutQtyDetailDTO objDTO)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;

            try
            {
                if (ModelState.IsValid && id == objDTO.ID)
                {
                   // _repository.Edit(objDTO);

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

        [HttpGet]
        public IEnumerable<TransferInOutQtyDetailDTO> GetAllRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted,Int64 DetailID)
        {
            return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, DetailID);
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

        public ResponseMessage TransferQuantity(TakenQtyDetail objMoveInQty, Int64 RoomID, Int64 CompanyID, Int64 UserID)
        {
            return _repository.TransferQuantity(objMoveInQty, RoomID, CompanyID, UserID);
        }

        public ResponseMessage ReceiveQuantity(Int64 ItemID, Int64 DetailId, Int64 LocationID, Int64 RoomID, Int64 CompanyID, Int64 UserID)
        {
            return _repository.ReceiveQuantity(ItemID, DetailId, LocationID,RoomID, CompanyID, UserID);
        }

    }
}


