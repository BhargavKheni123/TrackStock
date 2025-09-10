using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DAL
{
   public class OrderImageDetailDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public OrderImageDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        #endregion


        public List<long> InsertOrderImageData(OrderImageDetailDTO orderimage)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OrderGuid", orderimage.OrderGUID), new SqlParameter("@ImageName", orderimage.ImageName), new SqlParameter("@CreatedBy", orderimage.CreatedBy)
            ,new SqlParameter("@LastUpdatedBy",orderimage.LastUpdatedBy),new SqlParameter("@Created",orderimage.Created),new SqlParameter("@LastUpdated",orderimage.LastUpdated),
            new SqlParameter("@ReceivedOn",orderimage.ReceivedOn), new SqlParameter("@ReceivedOnWeb",orderimage.ReceivedOnWeb),new SqlParameter("@IsDeleted",orderimage.IsDeleted),
            new SqlParameter("@IsArchieved",orderimage.IsArchived),new SqlParameter("@CompanyId",orderimage.CompanyID),new SqlParameter("@RoomId",orderimage.RoomId),new SqlParameter("@AddedFrom",orderimage.AddedFrom),new SqlParameter("@EditedFrom",orderimage.EditedFrom),
            new SqlParameter("@Guid",orderimage.GUID),new SqlParameter("@WhatWhereAction",DBNull.Value)};
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    return context.Database.SqlQuery<long>("exec [SaveOrderImageData] @OrderGuid,@ImageName,@CreatedBy,@LastUpdatedBy,@Created,@LastUpdated,@ReceivedOn,@ReceivedOnWeb,@IsDeleted,@IsArchieved,@CompanyId,@RoomId,@AddedFrom,@EditedFrom,@Guid,@WhatWhereAction", params1).ToList();
                }
            }catch (Exception ex)
            {
                return null;
            }
        }

        public List<OrderImageDetail> GetorderImagesByGuidPlain(Guid OrderGuid)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@orderGuid", OrderGuid) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderImageDetail>("exec [GetorderImagesByWOGuidPlain] @orderGuid", params1).ToList();
            }
        }

        public void DeleteRecords(string IDs, long UserId, Guid OrderGuid, string WhatWhereAction = "")
        {
            if (string.IsNullOrEmpty(WhatWhereAction))
            {
                WhatWhereAction = "From Web";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@GUIDs", IDs),
                                                    new SqlParameter("@UserId", UserId),
                                                    new SqlParameter("@OrderGuid", OrderGuid),
                                                    new SqlParameter("@WhatWhereAction", WhatWhereAction)
                                                };
                context.Database.ExecuteSqlCommand("exec [DeleteOrderImageDetail] @GUIDs, @UserId,@OrderGuid,@WhatWhereAction", params1);
            }
        }


        public OrderImageDetailDTO GetOrderImageByGUID(Guid? imageGUID, long? RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@imageGUID", imageGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderImageDetailDTO>("exec [GetOrderImageByGUID] @imageGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }



    }
}
