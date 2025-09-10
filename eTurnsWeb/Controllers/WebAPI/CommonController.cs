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
    public class CommonController : ApiController
    {
        public CommonDAL _repository = new CommonDAL();

        public IEnumerable<CommonDTO> GetDDData(string TableName, string TextFieldName, Int64 CompanyID, Int64 RoomID)
        {
            return _repository.GetDDData(TableName, TextFieldName, CompanyID, RoomID);
        }

        public IEnumerable<CommonDTO> GetDDData(string TableName, string TextFieldName, string WhereCondition, Int64 CompanyID, Int64 RoomID)
        {
            return _repository.GetDDData(TableName, TextFieldName, WhereCondition, CompanyID, RoomID);
        }

        public Dictionary<string, int> GetNarrowDDData(string TableName, string TextFieldName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, string RequisitionCurrentTab)
        {
            return _repository.GetNarrowDDData(TableName, TextFieldName, CompanyID, RoomID, IsArchived, IsDeleted, RequisitionCurrentTab);
        }

        public Dictionary<string, int> GetUDFDDData(string TableName, string UDFName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, string RequisitionCurrentTab)
        {
            return _repository.GetUDFDDData(TableName, UDFName, CompanyID, RoomID, IsArchived, IsDeleted, RequisitionCurrentTab);
        }

        // GET api/technician
        [HttpGet]
        public IEnumerable<object> GetAllRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string TableName, Int64 ID)
        {
            return _repository.GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, TableName, ID);
        }
        public IEnumerable<object> GetAllRecordsGUID(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string TableName, string GUID)
        {
            return _repository.GetPagedRecordsGUID(StartRowIndex, MaxRows, out TotalCount, SearchTerm, TableName, GUID);
        }

        public AutoSequenceNumbersDTO GetLastGeneratedID(Int64 roomID, Int64 companyID, string prefix)
        {
            return new AutoSequenceDAL().GetLastGeneratedID(roomID, companyID, prefix);
        }

        public ResponseMessage CheckQuantityByLocation(Int64 LocationID, Guid ItemGUID, double Quantity, Int64 RoomID, Int64 CompanyID)
        {
            return _repository.CheckQuantityByLocation(LocationID, ItemGUID, Quantity, RoomID, CompanyID);
        }
        public List<BinMasterDTO> GetLocationWithQuantity(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            return _repository.GetLocationWithQuantity(ItemGUID, RoomID, CompanyID);
        }

        public NarrowSearchData GetNarrowSearchDataFromCache(string PageName, Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, string Status)
        {
            return _repository.GetNarrowSearchDataFromCache(PageName, CompanyID, RoomID, IsArchived, IsDeleted, Status);
        }

        public List<CommonDTO> GetLocationListWithQuntity(Guid ItemGUID, Int64 RoomID, Int64 CompanyId)
        {
            return _repository.GetLocationListWithQuntity(ItemGUID, RoomID, CompanyId);
        }
    }
}
