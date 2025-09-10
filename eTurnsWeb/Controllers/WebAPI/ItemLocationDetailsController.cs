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
    public class ItemLocationDetailsController : ApiController
    {

        public ItemLocationDetailsDAL _repository = new ItemLocationDetailsDAL();

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<ItemLocationDetailsDTO> GetAllRecord(Int64 roomID, Int64 CompanyID, Int64 ItemID, Int64? OrderDetailID, string ColumnName)
        {
            return _repository.GetAllRecords(roomID, CompanyID, ItemID, OrderDetailID, ColumnName);
        }

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<ItemLocationDetailsDTO> GetAllRecord(Int64 roomID, Int64 CompanyID, Int64 ItemID, Int64? OrderDetailID, string ColumnName, Int64? OrderDetailID_Exclude)
        {
            return _repository.GetAllRecords(roomID, CompanyID, ItemID, OrderDetailID, ColumnName, OrderDetailID_Exclude);
        }

        public IEnumerable<ItemLocationDetailsDTO> GetAllRecords(Int64 CompanyID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, bool IsArchived, bool IsDeleted, Int64 ItemID, Int64? OrderDetailID)
        {
            return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, ItemID, OrderDetailID);
        }

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ItemLocationDetailsDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return _repository.GetRecord(id, RoomID, CompanyID);
        }

        public ItemLocationDetailsDTO GetHistoryRecord(Int64 id)
        {
            return _repository.GetHistoryRecord(id);
        }


        /// <summary>
        /// User for create/insert record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PostRecord(ItemLocationDetailsDTO objDTO)
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
        ///PUT api/ItemLocationDetails/5 
        ///use for Update Rocord
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public HttpResponseMessage PutRecord(Int64 id, ItemLocationDetailsDTO objDTO)
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

        //// GET api/ItemLocationDetails
        //[HttpGet]
        //public IEnumerable<ItemLocationDetailsDTO> GetAllRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID,Int64 CompanyID, bool IsArchived, bool IsDeleted)
        //{
        //    return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID,CompanyID, IsArchived,IsDeleted);
        //}

        [HttpDelete]
        public HttpResponseMessage DeleteRecords(string IDs, Int64 UserID, Int64 RoomID, Int64 CompanyId)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;

            try
            {
                if (ModelState.IsValid)
                {
                    _repository.DeleteRecords(IDs, UserID,RoomID, CompanyId);
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
        public string DuplicateCheckSrNumber(string Name, Int64 ID, Int64 RoomID, Int64 CompanyID)
        {
            string ActionMode = "";
            if (ID > 0)
            {
                ActionMode = "edit";
            }
            else
            {
                ActionMode = "add";
            }
            //return _repository.DuplicateCheck(LotNumber,ActionMode,ID);
            CommonDAL objCDal = new CommonDAL();
            return objCDal.DuplicateCheck(Name, ActionMode, ID, "ItemLocationDetails", "SerialNumber", RoomID, CompanyID);
        }


        [HttpGet]
        public string DuplicateCheckForCreditPull(string Name, Int64 ID, Int64 RoomID, Int64 CompanyID,string ExtraWhereCond)
        {
            string ActionMode = "";
            if (ID > 0)
            {
                ActionMode = "edit";
            }
            else
            {
                ActionMode = "add";
            }
            //return _repository.DuplicateCheck(LotNumber,ActionMode,ID);
            CommonDAL objCDal = new CommonDAL();
            return objCDal.DuplicateCheckForCreditPull(Name, ActionMode, ID, "ItemLocationDetails", "SerialNumber", RoomID, CompanyID,ExtraWhereCond);
        }

        [HttpGet]
        public string DuplicateCheckLotNumber(string Name, Int64 ID, Int64 RoomID, Int64 CompanyID)
        {
            string ActionMode = "";
            if (ID > 0)
            {
                ActionMode = "edit";
            }
            else
            {
                ActionMode = "add";
            }
            //return _repository.DuplicateCheck(LotNumber,ActionMode,ID);
            CommonDAL objCDal = new CommonDAL();
            return objCDal.DuplicateCheck(Name, ActionMode, ID, "ItemLocationDetails", "LotNumber", RoomID, CompanyID);
        }

        [HttpGet]
        public string DuplicateCheckUPC(string Name, Int64 ID, Int64 RoomID, Int64 CompanyID)
        {
            string ActionMode = "";
            if (ID > 0)
            {
                ActionMode = "edit";
            }
            else
            {
                ActionMode = "add";
            }
            //return _repository.DuplicateCheck(LotNumber,ActionMode,ID);
            CommonDAL objCDal = new CommonDAL();
            return objCDal.DuplicateCheck(Name, ActionMode, ID, "ItemMaster", "UPC", RoomID, CompanyID);
        }

        [HttpPost]
        public bool ItemLocationDetailsSave(List<ItemLocationDetailsDTO> objData)
        {
            ItemLocationDetailsDAL objDal = new ItemLocationDetailsDAL();
            return objDal.ItemLocationDetailsSave(objData);
        }

        [HttpPost]
        public bool ItemLocationDetailsSaveForCreditPull(List<ItemLocationDetailsDTO> objData)
        {
            ItemLocationDetailsDAL objDal = new ItemLocationDetailsDAL();
            return objDal.ItemLocationDetailsSaveForCreditPull(objData);
        }

        [HttpPost]
        public bool ItemManufacturerDetailsSave(List<ItemManufacturerDetailsDTO> objData)
        {
            ItemLocationDetailsDAL objDal = new ItemLocationDetailsDAL();
            return objDal.ItemManufacturerDetailsSave(objData);
        }
        [HttpPost]
        public bool ItemSupplierDetailsSave(List<ItemSupplierDetailsDTO> objData)
        {
            ItemLocationDetailsDAL objDal = new ItemLocationDetailsDAL();
            return objDal.ItemSupplierDetailsSave(objData);
        }
        #endregion
    }
}


