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
    public class BarcodeMasterController : ApiController
    {

        BarcodeMasterDAL _repository = new BarcodeMasterDAL();

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<BarcodeMasterDTO> GetAllRecord(Int64 RoomID, Int64 CompanyId)
        {
            return _repository.GetAllRecords(RoomID, CompanyId);
        }

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<BarcodeMasterDTO> GetAllActiveRecordsByModuleID(string ModuleGuid, Int64 RoomID, Int64 CompanyID)
        {
            return _repository.GetAllActiveRecordsByModuleID(ModuleGuid, RoomID, CompanyID);
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public Int64 Insert(BarcodeMasterDTO objDTO)
        {

            return _repository.Insert(objDTO);
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId)
        {

            return _repository.DeleteRecords(IDs, userid, CompanyId);
        }
    }
}
