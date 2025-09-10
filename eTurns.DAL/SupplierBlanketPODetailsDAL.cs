using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class SupplierBlanketPODetailsDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public SupplierBlanketPODetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public SupplierBlanketPODetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public List<SupplierBlanketPODetailsDTO> GetBlanketPOBySupplierIDFull(Int64 SupplierID, Int64 RoomID, Int64 CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@SupplierID", SupplierID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierBlanketPODetailsDTO>("exec [GetBlanketPOBySupplierIDFull] @SupplierID,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<SupplierBlanketPODetailsDTO> GetBlanketPOBySupplierIDPlain(Int64 SupplierID, Int64 RoomID, Int64 CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@SupplierID", SupplierID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierBlanketPODetailsDTO>("exec [GetBlanketPOBySupplierIDPlain] @SupplierID,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<SupplierBlanketPODetailsDTO> GetBlanketPOBySupplierIDNormal(Int64 SupplierID, Int64 RoomID, Int64 CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@SupplierID", SupplierID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierBlanketPODetailsDTO>("exec [GetBlanketPOBySupplierIDNormal] @SupplierID,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public Int64 Insert(SupplierBlanketPODetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SupplierBlanketPODetail obj = new SupplierBlanketPODetail();
                obj.ID = 0;
                obj.SupplierID = objDTO.SupplierID;

                obj.StartDate = objDTO.StartDate;

                obj.Enddate = objDTO.Enddate;
                obj.BlanketPO = objDTO.BlanketPO;
                obj.GUID = Guid.NewGuid();
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.MaxLimit = objDTO.MaxLimit;
                obj.IsNotExceed = objDTO.IsNotExceed;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.MaxLimitQty = objDTO.MaxLimitQty;
                obj.IsNotExceedQty = objDTO.IsNotExceedQty;
                //obj.OrderedQty = objDTO.OrderedQty;
                obj.TotalOrder = objDTO.TotalOrder;
                obj.OrderUsed = objDTO.OrderUsed;
                context.SupplierBlanketPODetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;


                return obj.ID;
            }

        }

        public bool Edit(SupplierBlanketPODetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SupplierBlanketPODetail obj = context.SupplierBlanketPODetails.First(t => t.ID == objDTO.ID);
                if (obj != null)
                {
                    obj.BlanketPO = objDTO.BlanketPO;
                    obj.StartDate = objDTO.StartDate;
                    obj.Enddate = objDTO.Enddate;
                    obj.MaxLimit = objDTO.MaxLimit;
                    obj.IsNotExceed = objDTO.IsNotExceed;
                    obj.Updated = objDTO.Updated;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.EditedFrom = objDTO.EditedFrom;
                    obj.ReceivedOn = objDTO.ReceivedOn;
                    obj.IsDeleted = objDTO.IsDeleted;
                    obj.MaxLimitQty = objDTO.MaxLimitQty;
                    obj.IsNotExceedQty = objDTO.IsNotExceedQty;
                    obj.OrderUsed = objDTO.OrderUsed;
                    obj.TotalOrder = objDTO.TotalOrder;
                    context.SaveChanges();
                }
                return true;
            }
        }

        public IEnumerable<SupplierBlanketPODetailsDTO> GetBlanketPOBySupplierIDAndItemGUID(Int64 SupplierID, Int64 RoomID, Int64 CompanyID, Guid? ItemGUID)
        {

            var params1 = new SqlParameter[] { new SqlParameter("@SupplierID", SupplierID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierBlanketPODetailsDTO>("exec [GetBlanketPOBySupplierIDAndItemGUIDNormal] @SupplierID,@RoomID,@CompanyID,@ItemGUID", params1).ToList();
            }

        }

        public IEnumerable<SupplierBlanketPODetailsDTO> GetBlanketPOByItemSupplierID(Int64 SupplierID, Int64 RoomID, Int64 CompanyID, Guid? ItemGUID)
        {

            var params1 = new SqlParameter[] { new SqlParameter("@SupplierID", SupplierID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierBlanketPODetailsDTO>("exec [GetBlanketPOBySupplierIDAndItemDetailID] @SupplierID,@RoomID,@CompanyID,@ItemGUID", params1).ToList();
            }

        }
        public long SupplierBlanketPODetailsDuplicateCheck(long ID, string BlanketPO, long SupplierID, long RoomID, long CompanyID)
        {
            long id = 0;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SupplierBlanketPODetailsDTO qry = (from em in context.SupplierBlanketPODetails
                                                   where em.BlanketPO == BlanketPO && em.IsArchived == false && em.IsDeleted == false && em.ID != ID
                                                   && em.SupplierID == SupplierID && em.Room == RoomID && em.CompanyID == CompanyID
                                                   select new SupplierBlanketPODetailsDTO
                                                   {
                                                       ID = em.ID,
                                                   }).FirstOrDefault();

                if (qry != null)
                {
                    id = qry.ID;
                }
            }
            return id;
        }

        #endregion

    }
}


