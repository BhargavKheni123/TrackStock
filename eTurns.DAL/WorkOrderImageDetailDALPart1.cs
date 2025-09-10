using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DAL
{
    public partial class WorkOrderImageDetailDAL : eTurnsBaseDAL
    {
        public string DeleteRecords(string IDs, Int64 userid, Int64 CompanyID, Int64 RoomID, Guid WorkOrderGuid, string WhatWhereAction = "")
        {
            int ctrdelete = 0;
            string msg = string.Empty;
            if (string.IsNullOrEmpty(WhatWhereAction))
            {
                WhatWhereAction = "From Web";
            }
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                foreach (var item in IDs.Split(','))
                {
                    string strQuery = "";
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE WorkOrderImageDetail SET LastUpdated = getutcdate() ,WhatWhereAction='" + WhatWhereAction + "', LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='" + WhatWhereAction + "' WHERE GuID ='" + item.ToString() + "' And WorkOrderGuid='" + WorkOrderGuid.ToString() + "';";
                        context.ExecuteStoreCommand(strQuery);
                        ctrdelete += 1;

                    }
                }
                if (ctrdelete > 0)
                {
                    msg = ctrdelete + " record(s) deleted successfully.";
                }
                return msg;
            }
        }
        public string DeleteSingleRecord(string WhatWhereAction, string userid, string guid, string WorkOrderGUID)
        {
            string msg = string.Empty;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string strQuery = "";
                strQuery += "UPDATE WorkOrderImageDetail SET LastUpdated = getutcdate() ,WhatWhereAction='" + WhatWhereAction + "', LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='" + WhatWhereAction + "' WHERE GuID ='" + guid.ToString() + "' And WorkOrderGuid='" + WorkOrderGUID.ToString() + "';";
                int i = context.ExecuteStoreCommand(strQuery);
                if (i > 0)
                {
                    msg = "success";
                }
                else
                {
                    msg = "Fail:Record not found for provided GUID " + guid;
                }
                return msg;
            }
        }
    }
}
