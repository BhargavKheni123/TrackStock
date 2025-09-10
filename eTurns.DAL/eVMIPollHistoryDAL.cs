using eTurns.DTO;
using System;

namespace eTurns.DAL
{
    public class eVMIPollHistoryDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public eVMIPollHistoryDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public eVMIPollHistoryDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion


        /// <summary>
        /// Insert Record in the DataBase ItemLocationDetails
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public eVMIPollHistoryDTO Insert(eVMIPollHistoryDTO objDTO)
        {
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                eVMIPollHistory obj = new eVMIPollHistory()
                {
                    BegingConsQty = objDTO.BegingConsQty,
                    BegingCustOwnQty = objDTO.BegingCustOwnQty,
                    BeginingQty = objDTO.BeginingQty,
                    BinID = objDTO.BinID,
                    CompanyID = objDTO.CompanyID,
                    Created = objDTO.Created,
                    CreatedBy = objDTO.CreatedBy,
                    EndingConsQty = objDTO.EndingQty,
                    EndingCustOwnQty = objDTO.EndingCustOwnQty,
                    EndingQty = objDTO.EndingQty,
                    ID = 0,
                    GUID = Guid.NewGuid(),
                    ItemGUID = objDTO.ItemGUID,
                    PollDate = objDTO.PollDate,
                    PollType = objDTO.PollType,
                    RoomID = objDTO.RoomID,
                    NewQuantity = objDTO.NewQuantity,
                    PollStatus = objDTO.PollStatus,
                    PollStatusDesc = objDTO.PollStatusDesc,
                };
                context.eVMIPollHistories.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                return objDTO;
            }
        }

    }
}
