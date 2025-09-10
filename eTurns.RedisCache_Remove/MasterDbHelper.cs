using eTurns.DTO;
using eTurnsMaster.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.RedisCache_Remove
{
    class MasterDbHelper
    {
        public List<EnterpriseDTO> GetEnterpriseByIds(string EnterpriseIds)
        {
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            return objEnterpriseMasterDAL.GetEnterprisesByIds(EnterpriseIds);
        }
    }
}
