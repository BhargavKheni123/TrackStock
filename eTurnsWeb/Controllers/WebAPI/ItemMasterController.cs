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
    public class ItemMasterController : ApiController
    {

        public ItemMasterDAL _repository = new ItemMasterDAL();

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<ItemMasterDTO> GetAllRecord(Int64 roomID,Int64 EnterpriseID)
        {
            return _repository.GetAllRecords(roomID,EnterpriseID);
        }


        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ItemMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return _repository.GetRecord(id, RoomID, CompanyID,IsArchived,IsDeleted);
        }

        public ItemMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return _repository.GetRecord(id, RoomID, CompanyID);
        }

        public ItemMasterDTO GetHistoryRecord(Int64 id)
        {
            return _repository.GetHistoryRecord(id);
        }


        /// <summary>
        /// User for create/insert record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PostRecord(ItemMasterDTO objDTO, out Int64 ItemID)
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
                    ItemID = objDTO.ID;
                    return Request.CreateResponse(hStatusCode, objDTO.ID);
                }
                else
                {
                    ItemID = objDTO.ID;
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
        ///PUT api/ItemMaster/5 
        ///use for Update Rocord
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PutRecord(Int64 id, ItemMasterDTO objDTO)
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

        // GET api/ItemMaster
        [HttpGet]
        public IEnumerable<ItemMasterDTO> GetAllRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID,Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID,CompanyID, IsArchived,IsDeleted);
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
        public string DuplicateCheck(string Name, string ActionMode, Int64 ID, Int64 RoomID, Int64 CompanyID)
        {
            //return _repository.DuplicateCheck(ItemNumber,ActionMode,ID);
            CommonDAL objCDal = new CommonDAL();
            return objCDal.DuplicateCheck(Name, ActionMode, ID, "ItemMaster", "ItemNumber",  RoomID, CompanyID);
        }


        // GET api/ItemMaster
        [HttpGet]
        public IEnumerable<ItemMasterDTO> GetPagedRecordsForModel(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ItemIDs)
        {
            return _repository.GetPagedRecordsForModel(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, ItemIDs);
        }
        [HttpGet]
        public IEnumerable<ItemMasterDTO> GetPagedRecordsForModel(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ItemIDs,Int64 SupplierID)
        {
            return _repository.GetPagedRecordsForModel(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, ItemIDs, SupplierID);
        }

        [HttpGet]
        public IEnumerable<ItemMasterDTO> GetPagedRecordsForTransferItemModel(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ItemIDs, Int64 ReplinishRoomID)
        {
            return _repository.GetPagedRecordsForTransferItemModel(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, ItemIDs, ReplinishRoomID);
        }
        #endregion
    }
}


