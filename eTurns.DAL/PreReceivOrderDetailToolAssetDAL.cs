using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class PreReceivOrderDetailToolAssetDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public PreReceivOrderDetailToolAssetDAL(base.DataBaseName)
        //{

        //}

        public PreReceivOrderDetailToolAssetDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public PreReceivOrderDetailToolAssetDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PreReceivOrderDetailToolDTO> GetAllRecordsByOrderDetailTool(Guid OrderDetailGuid, Guid ToolGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@OrderDetailGuid", OrderDetailGuid), new SqlParameter("@ToolGuid", ToolGuid) };
                IEnumerable<PreReceivOrderDetailToolDTO> obj = (from u in context.Database.SqlQuery<PreReceivOrderDetailToolDTO>("exec GetPreReceivOrderToolDetail @OrderDetailGuid,@ToolGuid", params1)
                                                                select new PreReceivOrderDetailToolDTO
                                                                {
                                                                    ID = u.ID,
                                                                    ToolBinID = u.ToolBinID,
                                                                    Quantity = u.Quantity,

                                                                    SerialNumber = u.SerialNumber,
                                                                    LotNumber = u.LotNumber,
                                                                    ReceivedOnDate = u.ReceivedOnDate,
                                                                    Cost = u.Cost,
                                                                    GUID = u.GUID,
                                                                    ToolGUID = u.ToolGUID,
                                                                    Created = u.Created,
                                                                    Updated = u.Updated,
                                                                    CreatedBy = u.CreatedBy,
                                                                    LastUpdatedBy = u.LastUpdatedBy,
                                                                    IsDeleted = u.IsDeleted,
                                                                    IsArchived = u.IsArchived,
                                                                    CompanyID = u.CompanyID,
                                                                    Room = u.Room,
                                                                    CreatedByName = u.CreatedByName,
                                                                    UpdatedByName = u.UpdatedByName,
                                                                    RoomName = u.RoomName,
                                                                    BinNumber = u.BinNumber,
                                                                    ToolName = u.ToolName,
                                                                    SerialNumberTracking = u.SerialNumberTracking,

                                                                    ToolAssetOrderDetailGUID = u.ToolAssetOrderDetailGUID,
                                                                    PackSlipNumber = u.PackSlipNumber,
                                                                    UDF1 = u.UDF1,
                                                                    UDF2 = u.UDF2,
                                                                    UDF3 = u.UDF3,
                                                                    UDF4 = u.UDF4,
                                                                    UDF5 = u.UDF5,
                                                                    AddedFrom = u.AddedFrom,
                                                                    EditedFrom = u.EditedFrom,
                                                                    ReceivedOn = u.ReceivedOn,
                                                                    ReceivedOnWeb = u.ReceivedOnWeb,

                                                                    IsReceived = u.IsReceived,
                                                                    ToolType = u.ToolType,
                                                                }).AsParallel().ToList();


                return obj;
            }
        }


        /// <summary>
        /// Insert Record in the DataBase ReceiveOrderDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(PreReceivOrderDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PreReceivOrderDetail obj = new PreReceivOrderDetail()
                {
                    ItemGUID = objDTO.ItemGUID,
                    OrderDetailGUID = objDTO.OrderDetailGUID,

                    SerialNumber = objDTO.SerialNumber,
                    LotNumber = objDTO.LotNumber,
                    ExpirationDate = objDTO.ExpirationDate,

                    Quantity = objDTO.Quantity,
                    Cost = objDTO.Cost,
                    ReceivedDate = objDTO.ReceivedDate,

                    CompanyID = objDTO.CompanyID,
                    Room = objDTO.Room,
                    Created = DateTimeUtility.DateTimeNow,
                    Updated = DateTimeUtility.DateTimeNow,
                    CreatedBy = objDTO.CreatedBy,

                    GUID = Guid.NewGuid(),
                    ID = 0,
                    IsDeleted = false,
                    IsArchived = false,
                    IsReceived = false,

                    LastUpdatedBy = objDTO.LastUpdatedBy,
                    PackSlipNumber = objDTO.PackSlipNumber,
                    AddedFrom = (objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom),
                    EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom),
                    ReceivedOn = Convert.ToDateTime(objDTO.ReceivedOn),
                    ReceivedOnWeb = Convert.ToDateTime(objDTO.ReceivedOnWeb),

                    BinID = objDTO.BinID,
                    UDF1 = objDTO.UDF1,
                    UDF2 = objDTO.UDF2,
                    UDF3 = objDTO.UDF3,
                    UDF4 = objDTO.UDF4,
                    UDF5 = objDTO.UDF5,
                };

                context.PreReceivOrderDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                return obj.ID;
            }

        }

        /// <summary>
        /// Insert Record in the DataBase ReceiveOrderDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public bool Edit(PreReceivOrderDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PreReceivOrderDetail obj = context.PreReceivOrderDetails.FirstOrDefault(x => x.ID == objDTO.ID);

                obj.SerialNumber = objDTO.SerialNumber;
                obj.LotNumber = objDTO.LotNumber;
                obj.ExpirationDate = objDTO.ExpirationDate;

                obj.Quantity = objDTO.Quantity;
                obj.Cost = objDTO.Cost;
                obj.ReceivedDate = objDTO.ReceivedDate;

                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;

                obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                obj.ReceivedOn = Convert.ToDateTime(objDTO.ReceivedOn);

                context.SaveChanges();

                objDTO.ID = obj.ID;
                return true;
            }

        }

        public void UpdateIsReceived(ReceivedOrderTransferDetailDTO rotd)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                double qty = rotd.CustomerOwnedQuantity.GetValueOrDefault(0) + rotd.ConsignedQuantity.GetValueOrDefault(0);

                var obj = context.PreReceivOrderDetails.FirstOrDefault(x => ((x.Quantity ?? 0) == qty) && x.LotNumber == rotd.LotNumber && x.SerialNumber == x.SerialNumber && x.ExpirationDate == rotd.ExpirationDate && x.ItemGUID == rotd.ItemGUID && x.OrderDetailGUID == rotd.OrderDetailGUID);
                if (obj != null && obj.ID > 0)
                {
                    obj.IsReceived = true;
                    obj.Updated = DateTimeUtility.DateTimeNow;
                    context.SaveChanges();
                }
            }
        }
    }
}
