using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DAL
{
   public class ReceiveFileDetailsDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public ReceiveFileDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        #endregion

        #region Insert File Details
        public List<long> InsertReceiveFileData(ReceiveFileDetailDTO receiveFileDetail)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OrderDetailGuid", receiveFileDetail.OrderDetailsGUID),new SqlParameter("@ReceivedOrderTransferGuid", receiveFileDetail.ReceivedOrderTransferGuid), new SqlParameter("@FileName", receiveFileDetail.FileName),new SqlParameter("@FilePath",receiveFileDetail.FilePath), new SqlParameter("@CreatedBy", receiveFileDetail.CreatedBy)
            ,new SqlParameter("@LastUpdatedBy",receiveFileDetail.LastUpdatedBy),new SqlParameter("@Created",receiveFileDetail.Created),new SqlParameter("@LastUpdated",receiveFileDetail.LastUpdated),
            new SqlParameter("@ReceivedOn",receiveFileDetail.ReceivedOn), new SqlParameter("@ReceivedOnWeb",receiveFileDetail.ReceivedOnWeb),new SqlParameter("@IsDeleted",receiveFileDetail.IsDeleted),
            new SqlParameter("@IsArchieved",receiveFileDetail.IsArchived),new SqlParameter("@CompanyId",receiveFileDetail.CompanyID),new SqlParameter("@RoomId",receiveFileDetail.RoomId),new SqlParameter("@AddedFrom",receiveFileDetail.AddedFrom),new SqlParameter("@EditedFrom",receiveFileDetail.EditedFrom),
            new SqlParameter("@Guid",receiveFileDetail.GUID),new SqlParameter("@WhatWhereAction",DBNull.Value)};
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    return context.Database.SqlQuery<long>("exec [SaveReceiveFilesData] @OrderDetailGuid,@ReceivedOrderTransferGuid,@FileName,@FilePath,@CreatedBy,@LastUpdatedBy,@Created,@LastUpdated,@ReceivedOn,@ReceivedOnWeb,@IsDeleted,@IsArchieved,@CompanyId,@RoomId,@AddedFrom,@EditedFrom,@Guid,@WhatWhereAction", params1).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Receive File Details
        public List<ReceiveFileDetailDTO> GetReceiveFileByGuidPlain(Guid OrderDetailsGuid)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@orderDetailsGuid", OrderDetailsGuid) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ReceiveFileDetailDTO>("exec [GetReceiveFilesGuidPlain] @orderDetailsGuid", params1).ToList();
            }
        }
        #endregion

        #region Receive File Delete
        public bool DeleteReceiveFileByGuid(string fileGuid, Int64 userID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@fileGuid", Guid.Parse(fileGuid)), new SqlParameter("@userID", userID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    context.Database.ExecuteSqlCommand("EXEC [DeleteReceiveFilesByGuid] @fileGuid,@userID", params1);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
        }
        #endregion

        public ReceiveFileDetailDTO GetReceiveOrderDetailsImageByGUID(Guid? imageGUID, Guid? orderdetailsguid, long? RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", imageGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@orderdetailsguid", orderdetailsguid) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ReceiveFileDetailDTO>("exec [GetReceiveOrderImageByGUID] @GUID,@orderdetailsguid,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        #region Receive File Details
        public List<ReceiveFileDetailDTO> GetReceiveFileByRoom(long CompanyID, long RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ReceiveFileDetailDTO>("exec [GetReceiveFilesByRoom] @RoomId,@CompanyId", params1).ToList();
            }
        }
        #endregion
    }
}
