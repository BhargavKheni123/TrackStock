using eTurns.DTO;
using System;
using System.Collections.Generic;
using eTurnsMaster.DAL;
using eTurnsWeb.Helper;

namespace eTurnsWeb.BAL
{
    public class BillingRoomTypeModulesMapBAL : IDisposable
    {

        public List<BillingRoomTypeModulesMapDTO> GetBillingRoomTypeModulesMap(int billingRoomTypeID)
        {
            BillingRoomTypeModulesMapDAL dal = new BillingRoomTypeModulesMapDAL();

            return dal.GetBillingRoomTypeModulesMap(billingRoomTypeID);
        }

        public bool SaveBillingRoomModuleMap(List<BillingRoomTypeModulesMapDTO> list)
        {
            BillingRoomTypeModulesMapDAL dal = new BillingRoomTypeModulesMapDAL();

            return dal.SaveBillingRoomModuleMap(list, SessionHelper.UserID);
        }
        public int SaveBillingRoomTypeMaster(string newBillingRoomTypeName, long enterpriseID)
        {
            BillingRoomTypeModulesMapDAL dal = new BillingRoomTypeModulesMapDAL();

            return dal.SaveBillingRoomTypeMaster(newBillingRoomTypeName, enterpriseID);
        }

        /// <summary>
        /// Get Billing Room Modules by room id
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public List<BillingRoomTypeModulesMapDTO> GetBillingRoomModules(long roomID, long compId, long entId)
        {
            BillingRoomTypeModulesMapDAL dal = new BillingRoomTypeModulesMapDAL();

            return dal.GetBillingRoomModules(roomID, compId, entId);
        }

        //public void UpdateUserRoleModulePermissions(int billingRoomTypeId)
        //{
        //    BillingRoomTypeModulesMapDAL dal = new BillingRoomTypeModulesMapDAL();
        //    dal.UpdateUserRoleModulePermissions(billingRoomTypeId, SessionHelper.UserID);
        //}


        public void InsertJobForRoomUpdated(long roomID, long enterpriseID, long companyID)
        {
            BillingRoomTypeModulesMapDAL dal = new BillingRoomTypeModulesMapDAL();
            dal.InsertBillingTypeMapUpdateJob(null, SessionHelper.UserID, "RoomChange", roomID, enterpriseID, companyID);
        }

        public void InsertJobForConfigUpdated(int billingRoomTypeId)
        {
            BillingRoomTypeModulesMapDAL dal = new BillingRoomTypeModulesMapDAL();
            dal.InsertBillingTypeMapUpdateJob(billingRoomTypeId, SessionHelper.UserID, "ConfigChange", null, null, null);
        }


        private bool disposedValue;

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
        // ~BillingRoomTypeModulesMapBAL()
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