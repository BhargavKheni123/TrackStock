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
    public class ReceiveMasterController : ApiController
    {
        public ReceiveOrderDetailsDAL _repository = new ReceiveOrderDetailsDAL();

        // GET api/OrderMaster
        [HttpGet]
        public IEnumerable<ReceiveOrderLineItemDetailsDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return _repository.GetPagedPendigLineItemsRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted);
        }

        // GET api/OrderMaster
        [HttpGet]
        public IEnumerable<ReceiveOrderLineItemDetailsDTO> GetLineItemsOrderRecords(Int64 RoomID, Int64 CompanyId, Int64 ItemID, Int64 OrderID)
        {
            return _repository.GetLineItemsOrderRecords(RoomID, CompanyId, ItemID, OrderID);
        }
          // GET api/OrderMaster
        [HttpGet]
        public ReceiveOrderLineItemDetailsDTO GetLineItemsOrderDetails(Int64 OrderDetailID)
        {
            return _repository.GetLineItemsOrderDetails(OrderDetailID);
        }


        // GET api/OrderMaster
        [HttpGet]
        public IEnumerable<ReceiveOrderLineItemDetailsDTO> GetStatuswiseOrderRecords(Int64 RoomID, Int64 CompanyId, Int64 ItemID, Int64 OrderStatus)
        {
            return _repository.GetStatuswiseOrderRecords(RoomID, CompanyId, ItemID, OrderStatus);
        }
        
    }
}
