using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace eTurns.DAL
{
    public class PDAEmailsToSendDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public PDAEmailsToSendDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public PDAEmailsToSendDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public List<PDAEmailsToSendDTO> GetPDAPendingEmails(Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from A in context.PDAEmailsToSends
                        where A.IsSent == false && A.RoomId == RoomID && A.CompanyId == CompanyID
                        select new PDAEmailsToSendDTO
                        {
                            GUID = A.GUID,
                            CompanyId = A.CompanyId,
                            RoomId = A.RoomId,
                            RecordGUID = A.RecordGUID,
                            RecordFromTable = A.RecordFromTable,
                            EmailTemplateType = A.EmailTemplateType,
                            IsSent = A.IsSent,
                            CreatedByUserId = A.CreatedByUserId,
                            CreatedOn = A.CreatedOn,
                        }).ToList();

                //                return (from u in context.Database.SqlQuery<PDAEmailsToSendDTO>(@"
                //                        SELECT *
                //                        FROM PDAEmailsToSend
                //                        WHERE IsSent = 0"
                //                        )
                //                        select new PDAEmailsToSendDTO
                //                        {
                //                            GUID = u.GUID,
                //                            CompanyId = u.CompanyId,
                //                            RoomId = u.RoomId,
                //                            RecordGUID = u.RecordGUID,
                //                            RecordFromTable = u.RecordFromTable,
                //                            EmailTemplateType = u.EmailTemplateType,
                //                            IsSent = u.IsSent,
                //                            CreatedByUserId = u.CreatedByUserId,
                //                            CreatedOn = u.CreatedOn,
                //                        }).AsParallel().ToList();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase PDAEmailsToSend
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>GUID</returns>
        public Guid Insert(PDAEmailsToSendDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PDAEmailsToSend obj = new PDAEmailsToSend();
                obj.GUID = objDTO.GUID;
                obj.CompanyId = objDTO.CompanyId;
                obj.RoomId = objDTO.RoomId;
                obj.RecordGUID = objDTO.RecordGUID;
                obj.RecordFromTable = objDTO.RecordFromTable;
                obj.EmailTemplateType = objDTO.EmailTemplateType;
                obj.IsSent = objDTO.IsSent;
                obj.CreatedByUserId = objDTO.CreatedByUserId;
                obj.CreatedOn = objDTO.CreatedOn;
                context.PDAEmailsToSends.Add(obj);
                context.SaveChanges();
                objDTO.GUID = obj.GUID;

                return obj.GUID;
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(PDAEmailsToSendDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PDAEmailsToSend obj = context.PDAEmailsToSends.Where(x => x.GUID == objDTO.GUID).FirstOrDefault();
                obj.RecordGUID = objDTO.RecordGUID;
                obj.RecordFromTable = objDTO.RecordFromTable;
                obj.EmailTemplateType = objDTO.EmailTemplateType;
                obj.IsSent = objDTO.IsSent;
                obj.CreatedByUserId = objDTO.CreatedByUserId;
                obj.CreatedOn = objDTO.CreatedOn;
                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.PDAEmailsToSends.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return true;
            }
        }
    }
}
