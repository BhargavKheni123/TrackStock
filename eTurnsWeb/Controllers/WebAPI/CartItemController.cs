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
    public class CartItemAPIController : ApiController
    {

        public CartItemDAL _repository = new CartItemDAL();

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<CartItemDTO> GetAllRecord(int roomID, Int32 EnterpriseID)
        {
            return _repository.GetAllRecords(roomID, EnterpriseID, true);
        }

        public CartItemDTO GetItemsCart(Int64 ItemId, Int64 RoomID, Int64 CompanyID)
        {
            return _repository.GetRecord(ItemId, RoomID, CompanyID, true);
        }

        public IEnumerable<CartItemDTO> GetCartItemsByItemId(Int64 RoomID, Int64 CompanyId, Int64 ItemID, string ColumnName)
        {
            return _repository.GetCartItemsByItemId(RoomID, CompanyId, ItemID, ColumnName);
        }
        public IEnumerable<CartItemDTO> GetCartItemsByItemId(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, Int64 ItemID, bool IsArchived, bool IsDeleted)
        {
            return _repository.GetCartItemsByItemId(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, ItemID, IsArchived, IsDeleted);
        }

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CartItemDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return _repository.GetRecord(id, RoomID, CompanyID, true);
        }

        public CartItemDTO GetHistoryRecord(Int64 id)
        {
            return _repository.GetHistoryRecord(id);
        }


        /// <summary>
        /// User for create/insert record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PostRecord(CartItemDTO objDTO)
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
        ///PUT api/CartItem/5 
        ///use for Update Rocord
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PutRecord(Int64 id, CartItemDTO objDTO)
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

        // GET api/CartItem
        [HttpGet]
        public IEnumerable<CartItemDTO> GetAllRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted);
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
            //return _repository.DuplicateCheck(Status,ActionMode,ID);
            CommonDAL objCDal = new CommonDAL();
            return objCDal.DuplicateCheck(Name, ActionMode, ID, "CartItem", "Status", RoomID, CompanyID);
        }

        /// <summary>
        ///PUT api/PullMaster/5 
        ///use for Update Rocord
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objDTO"></param>
        /// <param name="IsCreditPullNothing"></param> 1 = credit, 2 = pull, 3 = nothing
        /// <returns></returns>
        public HttpResponseMessage SaveCartItem(CartItemDTO objDTO)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;

            try
            {
                _repository.SaveCartItem(objDTO);
                return new HttpResponseMessage(hStatusCode);
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
        /// <param name="IsCreditPullNothing"></param> 1 = credit, 2 = pull, 3 = nothing
        /// <returns></returns>
        public HttpResponseMessage SaveCartItems(List<CartItemDTO> lstCartItems)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;

            try
            {
                _repository.SaveCartItems(lstCartItems);
                return new HttpResponseMessage(hStatusCode);
            }
            catch (Exception ex)
            {
                hStatusCode = HttpStatusCode.ExpectationFailed;
                throw ex;
            }


        }

        public IEnumerable<CartItemDTO> GetPagedUniqueCartItems(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<CartItemDTO> lstCartitems = _repository.GetUniqueCartItemsPaged(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted);
            return lstCartitems;

        }
        public IEnumerable<CartItemDTO> GetUniqueCartItemsPagedByItemId(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, long ItemID)
        {
            IEnumerable<CartItemDTO> lstCartitems = _repository.GetUniqueCartItemsPagedByItemId(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, ItemID);
            return lstCartitems;
        }
        #endregion
    }
}


