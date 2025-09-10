using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class ItemLocationeVMISetupDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public ItemLocationeVMISetupDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        #endregion

        #region [Class Methods]
        public Int64 Insert(ItemLocationeVMISetupDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationeVMISetup obj = new ItemLocationeVMISetup();
                obj.ID = 0;
                obj.BinID = objDTO.BinID;
                obj.eVMISensorPort = objDTO.eVMISensorPort;
                obj.eVMISensorID = objDTO.eVMISensorID;
                obj.PoundsPerPiece = objDTO.PoundsPerPiece;
                obj.Quantity = objDTO.Quantity;
                obj.CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity;
                obj.ConsignedQuantity = objDTO.ConsignedQuantity;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.ItemGUID = objDTO.ItemGUID;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ReceivedOn = objDTO.ReceivedOn ?? DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb ?? DateTimeUtility.DateTimeNow;
                context.ItemLocationeVMISetups.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                return obj.ID;
            }

        }
        public ItemLocationeVMISetupDTO UpdateSensorInfo(ItemLocationeVMISetupDTO objItemLocationeVMISetupDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationeVMISetup objItemLocationeVMISetup = context.ItemLocationeVMISetups.FirstOrDefault(t => t.ItemGUID == objItemLocationeVMISetupDTO.ItemGUID && t.BinID == objItemLocationeVMISetupDTO.BinID);
                if (objItemLocationeVMISetup != null)
                {
                    objItemLocationeVMISetup.eVMISensorID = objItemLocationeVMISetupDTO.eVMISensorID;
                    context.SaveChanges();
                }
            }
            return new ItemLocationeVMISetupDTO();
        }
        public List<ItemLocationeVMISetupDTO> GetItemWiseLocationListFull(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemLocationeVMISetupDTO>("exec [GetItemWiseLocationListFull] @RoomID,@CompanyID", params1).ToList();
            }
        }
        #endregion

    }
}


