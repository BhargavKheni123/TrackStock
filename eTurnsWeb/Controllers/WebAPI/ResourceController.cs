using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTurns.DTO.Resources;
using Elmah;
using eTurns.DAL;

namespace eTurnsWeb.Controllers.WebAPI
{
    public class ResourceController : ApiController
    {
        /// <summary>
        /// IRepository Object using dependecy injection
        /// </summary>
        //eTurns.BLL.ResourceBLL _repository = new eTurns.BLL.ResourceBLL();
        ResourceDAL objDAL = new ResourceDAL();

        public IEnumerable<ResourceLanguageDTO> GetResourceLanguageData(Int64 CompanyID)
        {
            return objDAL.GetCachedResourceLanguageData(CompanyID).ToList();
        }

        public IEnumerable<ResourceModuleMasterDTO> GetResourceModuleMasterData(Int64 CompanyID)
        {
            return objDAL.GetCachedResourceModuleMasterData(CompanyID).ToList();
        }

        public IEnumerable<ResourceModuleDetailsDTO> GetResourceModuleDetailData(Int64 CompanyID, int ResourceModuleID)
        {
            return objDAL.GetCachedResourceModuleDetailData(CompanyID, ResourceModuleID).ToList();
        }
    }
}
