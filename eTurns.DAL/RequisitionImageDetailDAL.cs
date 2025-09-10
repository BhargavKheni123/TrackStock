using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DAL
{
  public class RequisitionImageDetailDAL : eTurnsBaseDAL
  {
        #region [Class Constructor]
        public RequisitionImageDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        #endregion


        public List<long> InsertRequisitionImageData(RequisitionImageDetailDTO requisitionImageDetailDTO)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RequisitionGuid", requisitionImageDetailDTO.RequisitionGuid), new SqlParameter("@ImageName", requisitionImageDetailDTO.ImageName), new SqlParameter("@CreatedBy", requisitionImageDetailDTO.CreatedBy)
            ,new SqlParameter("@LastUpdatedBy",requisitionImageDetailDTO.LastUpdatedBy),new SqlParameter("@Created",requisitionImageDetailDTO.Created),new SqlParameter("@LastUpdated",requisitionImageDetailDTO.LastUpdated),
            new SqlParameter("@ReceivedOn",requisitionImageDetailDTO.ReceivedOn), new SqlParameter("@ReceivedOnWeb",requisitionImageDetailDTO.ReceivedOnWeb),new SqlParameter("@IsDeleted",requisitionImageDetailDTO.IsDeleted),
            new SqlParameter("@IsArchieved",requisitionImageDetailDTO.IsArchived),new SqlParameter("@CompanyId",requisitionImageDetailDTO.CompanyID),new SqlParameter("@RoomId",requisitionImageDetailDTO.RoomId),new SqlParameter("@AddedFrom",requisitionImageDetailDTO.AddedFrom),new SqlParameter("@EditedFrom",requisitionImageDetailDTO.EditedFrom),
            new SqlParameter("@Guid",requisitionImageDetailDTO.GUID),new SqlParameter("@WhatWhereAction",DBNull.Value)};
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    return context.Database.SqlQuery<long>("exec [SaveRequisitionImageData] @RequisitionGuid,@ImageName,@CreatedBy,@LastUpdatedBy,@Created,@LastUpdated,@ReceivedOn,@ReceivedOnWeb,@IsDeleted,@IsArchieved,@CompanyId,@RoomId,@AddedFrom,@EditedFrom,@Guid,@WhatWhereAction", params1).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<RequisitionImageDetailDTO> GetRequisitionImagesByGuidPlain(Guid RequisitionGuid)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@requisitionGuid", RequisitionGuid) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionImageDetailDTO>("exec [GetReqImagesByGuidPlain] @requisitionGuid", params1).ToList();
            }
        }

        public void DeleteRecords(string IDs, long UserId, Guid requisitionGuid, string WhatWhereAction = "")
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
                                                    new SqlParameter("@requisitionGuid", requisitionGuid),
                                                    new SqlParameter("@WhatWhereAction", WhatWhereAction)
                                                };
                context.Database.ExecuteSqlCommand("exec [DeleteRequisitionImageDetail] @GUIDs, @UserId,@requisitionGuid,@WhatWhereAction", params1);
            }
        }

        public RequisitionImageDetailDTO GetRequisitionFilesByGUID(Guid? imageGUID, long? RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@imageGUID", imageGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RequisitionImageDetailDTO>("exec [GetRequisitionFilesByGUID] @imageGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

    }
}
