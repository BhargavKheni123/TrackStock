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
    public class PullAPIController : ApiController
    {
        public PullMasterDAL _repository = new PullMasterDAL();

        #region "GET Paginations"

        // GET api/ItemMaster
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
                    //_repository.DeleteRecords(IDs, UserID, CompanyId, RoomID, out MSG);
                    _repository.DeleteRecords(IDs, UserID, CompanyId,RoomID);
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
            //return _repository.DuplicateCheck(ItemNumber,ActionMode,ID);
            CommonDAL objCDal = new CommonDAL();
            return objCDal.DuplicateCheck(Name, ActionMode, ID, "ItemMaster", "ItemNumber", RoomID, CompanyID);
        }


        // GET api/ItemMaster
        [HttpGet]
        public IEnumerable<PullMasterViewDTO> GetPagedRecordsForModel(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ItemIDs)
        {
            return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted);
        }
        /// <summary>
        ///PUT api/PullMaster/5 
        ///use for Update Rocord
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objDTO"></param>
        /// <param name="IsCreditPullNothing"></param> 1 = credit, 2 = pull, 3 = nothing
        /// <returns></returns>
        public HttpResponseMessage UpdatePullData(PullMasterViewDTO objDTO, int IsCreditPullNothing, Int64 RoomID, Int64 CompanyID, out string ItemLocationMSG, bool IsProjectSpendAllowed, out bool IsPSLimitExceed)
        {
            HttpStatusCode hStatusCode = HttpStatusCode.OK;

            try
            {
                _repository.UpdatePullData(objDTO, IsCreditPullNothing, RoomID, CompanyID, out ItemLocationMSG, IsProjectSpendAllowed, out IsPSLimitExceed);
                return new HttpResponseMessage(hStatusCode);
            }
            catch (Exception ex)
            {
                hStatusCode = HttpStatusCode.ExpectationFailed;
                throw ex;
            }


        }
        #endregion
    }
}
