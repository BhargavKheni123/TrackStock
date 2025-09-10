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
    public class GlobalUISettingsController : ApiController
    {
        public GlobalUISettingsDAL _repository = new GlobalUISettingsDAL();

        /// <summary>
        /// Get single record by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GlobalUISettingsDTO GetRecord(Int64 ID, String SearchType)
        {
            return _repository.GetRecord(ID, SearchType);
        }
    }
}