using eTurns.DTO;
using System;

namespace eTurns.DAL
{
    public class PollAllHistoryDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        //public PollAllHistoryDAL(base.DataBaseName)
        //{

        //}

        public PollAllHistoryDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public PollAllHistoryDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public Int64 Insert(PollAllHistoryDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PollAllHistory obj = new PollAllHistory();
                obj.ActionType = objDTO.ActionType;
                obj.CompanyID = objDTO.CompanyID;
                obj.CreatedOn = objDTO.CreatedOn;
                obj.LastPollAllTime = objDTO.LastPollAllTime;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.Remarks = objDTO.Remarks;
                obj.RoomID = objDTO.RoomID;
                obj.UpdatedOn = objDTO.UpdatedOn;
                obj.UserID = objDTO.UserId;

                context.PollAllHistories.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;




                return objDTO.ID;
            }
        }

    }
}
