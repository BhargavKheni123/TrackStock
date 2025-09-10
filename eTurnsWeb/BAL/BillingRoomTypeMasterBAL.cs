using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTurnsMaster.DAL;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;

namespace eTurnsWeb.BAL
{
    public class BillingRoomTypeMasterBAL : IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EntId">0 => Global Billing types else Global + Local</param>
        /// <returns></returns>
        public List<BillingRoomTypeMasterDTO> GetBillingRoomTypeMaster(long entID)
        {
            BillingRoomTypeMasterDAL dAL = new BillingRoomTypeMasterDAL();
            var list = dAL.GetBillingRoomTypeMaster(entID);

            foreach (var obj in list)
            {
                obj.ResourceValue = ResourceHelper.GetResourceValue(obj.ResourceKey, "ResRoomMaster");
            }

            return list;
        }

        public List<BillingRoomTypeMasterDTO> GetAllBillingRoomTypeResourceKeys()
        {
            BillingRoomTypeMasterDAL dAL = new BillingRoomTypeMasterDAL();
            var list = dAL.GetAllBillingRoomTypeResourceKeys();

            foreach (var obj in list)
            {
                obj.ResourceValue = ResourceHelper.GetResourceValue(obj.ResourceKey, "ResRoomMaster");
            }

            return list;
        }

        public List<EnterpriseDTO> GetAllEnterprise()
        {
            EnterpriseMasterDAL enterpriseMasterDAL = new EnterpriseMasterDAL();

            List<EnterpriseDTO> list = enterpriseMasterDAL.GetAllEnterprisesPlain();
            list = list.OrderBy(e => e.Name).ToList();
            list.Insert(0,new EnterpriseDTO() { ID = 0, Name = ResRoleMaster.All });
            return list;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BillingRoomTypeMasterBAL()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}