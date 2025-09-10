using eTurns.DAL;
using eTurns.DTO;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTurnsWeb.BAL
{
    public class RoomBAL : IDisposable
    {

        public RoomViewModel GetRoomByIDForModuleAccessList(string EnterpriseDBName, long RoomID)
        {
            RoomDAL objRoom = new RoomDAL(EnterpriseDBName);
            List<long> arrValidModules = new List<long> { (long)SessionHelper.ModuleList.Requisitions, (long)SessionHelper.ModuleList.WorkOrders, (long)SessionHelper.ModuleList.Orders };
            RoomDTO objDTO = objRoom.GetRoomByIDForModuleAccessList(RoomID, arrValidModules);
            RoomViewModel obj = new RoomViewModel(objDTO);

            return obj;
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
        // ~RoomBAL()
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