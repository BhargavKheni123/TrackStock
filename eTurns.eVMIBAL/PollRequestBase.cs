using eTurns.DTO;
using eTurns.eVMIBAL.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.eVMIBAL
{
    public class PollRequestBase
    {
        protected UserMasterDTO GetSystemUser(string EnterpriseDbName)
        {
            //UserMasterDTO objeturnsUserMasterDTO = new UserMasterDTO();
            //eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(EnterpriseDbName);
            //objeturnsUserMasterDTO = objUserMasterDAL.GetSystemUser(4);
            //return objeturnsUserMasterDTO;
            return SystemUser.GetSystemUser(EnterpriseDbName);
        }

        protected static void WriteComErrorLog(string methodName, long EnterpriseID, long CompanyID, long RoomID,
          Guid? ItemGUID, string comPortName, string evmiError, Exception ex = null
          )
        {
            string sItemGUID = ItemGUID.HasValue ? "" : ItemGUID.ToString();
            string log = string.Format(Environment.NewLine
                + "{0} - EnterpriseID : {1} , CompanyID : {2} , Room : {3} , ItemGUID : {4} , Com port : {5} " + Environment.NewLine + " Error : {6}",
                    methodName, EnterpriseID, CompanyID, RoomID,
                    sItemGUID, comPortName, evmiError);

            if (ex == null)
            {
                Logger.WriteLog(log);
            }
            else
            {
                Logger.WriteExceptionLog(log, ex);
            }
        }

    }
}
