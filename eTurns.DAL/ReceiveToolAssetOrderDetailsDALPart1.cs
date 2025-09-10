using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Data.SqlClient;
using eTurns.DTO.Resources;
using System.Web;

namespace eTurns.DAL
{
    public partial class ReceiveToolAssetOrderDetailsDALPart1 : eTurnsBaseDAL
    {
        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(ReceiveOrderDetailsDTO objDTO)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReceiveOrderDetail obj = context.ReceiveOrderDetails.Single(t => t.ID == objDTO.ID && t.OrderDetailGUID == objDTO.OrderDetailGUID);

                double PreviousQuentity = obj.ReceiveQuantity.GetValueOrDefault(0);


                obj.ReceiveBin = objDTO.ReceiveBin;
                obj.ReceiveDate = objDTO.ReceiveDate;
                obj.ReceiveQuantity = objDTO.ReceiveQuantity;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Unchanged);
                context.ReceiveOrderDetails.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();




                OrderDetailsDAL OrdDetail = new OrderDetailsDAL(base.DataBaseName);
                OrderDetailsDTO ordDetailDTO = OrdDetail.GetRecord(obj.OrderDetailGUID.GetValueOrDefault(Guid.Empty), obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
                if (ordDetailDTO != null && ordDetailDTO.ItemGUID != Guid.Empty)
                {
                    double UpdateQutenty = (objDTO.ReceiveQuantity.GetValueOrDefault(0) - PreviousQuentity);
                    int UpdateType = UpdateQutenty < 0 ? -1 : 1;

                    CommonDAL commonDAL = new CommonDAL(base.DataBaseName);
                    commonDAL.UpdateItemQuentity(ordDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.ReceiveBin.GetValueOrDefault(0), UpdateQutenty, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.LastUpdatedBy.GetValueOrDefault(0), UpdateType);
                }

                //Get Cached-Media
                IEnumerable<ReceiveOrderDetailsDTO> ObjCache = CacheHelper<IEnumerable<ReceiveOrderDetailsDTO>>.GetCacheItem("Cached_ReceiveOrderDetails_" + objDTO.CompanyID.ToString());
                if (ObjCache != null)
                {

                    objDTO.ID = obj.ID;
                    objDTO.GUID = obj.GUID;
                    objDTO.Created = obj.Created;
                    objDTO.CreatedBy = obj.CreatedBy;
                    objDTO.CreatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Int64.Parse(Convert.ToString(obj.CreatedBy))).UserName;
                    objDTO.IsArchived = false;
                    objDTO.IsDeleted = false;
                    objDTO.OrderDetailGUID = obj.OrderDetailGUID;

                    objDTO.RoomName = new RoomDAL(base.DataBaseName).GetRecord(Int64.Parse(Convert.ToString(obj.Room)), Convert.ToInt64(objDTO.CompanyID), false, false).RoomName;
                    objDTO.UpdatedByName = new UserMasterDAL(base.DataBaseName).GetRecord(Int64.Parse(Convert.ToString(obj.LastUpdatedBy))).UserName;
                    objDTO.LastUpdated = obj.LastUpdated;
                    objDTO.LastUpdatedBy = obj.LastUpdatedBy;
                    objDTO.ReceiveBin = obj.ReceiveBin;
                    objDTO.ReceiveDate = obj.ReceiveDate;
                    objDTO.ReceiveQuantity = obj.ReceiveQuantity;
                    objDTO.Room = obj.Room;
                    objDTO.CompanyID = obj.CompanyID;


                    List<ReceiveOrderDetailsDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(x => x.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<ReceiveOrderDetailsDTO> tempC = new List<ReceiveOrderDetailsDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<ReceiveOrderDetailsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<ReceiveOrderDetailsDTO>>.AppendToCacheItem("Cached_ReceiveOrderDetails_" + objDTO.CompanyID.ToString(), NewCache);
                }


                return true;
            }
        }
    }
}
