using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using eTurns.DTO;

namespace eTurnsMaster.DAL
{
    public class UserNarrowSearchSettingsDAL : eTurnsMasterBaseDAL, IDisposable
    {
        private bool disposedValue;

        public UserNarrowSearchSettingsDTO GetUserNarrowSearchSettings(long userId, string listName, long enterpriseID, long companyID, long roomID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@RoomID", roomID),
                    new SqlParameter("@CompanyID", companyID),
                    new SqlParameter("@EnterpriseID", enterpriseID),
                    new SqlParameter("@ListName", listName),
                };

                var result = context.Database.SqlQuery<UserNarrowSearchSettingsDTO>("exec GetUserNarrowSearchSettings @UserID,@RoomID,@CompanyID,@EnterpriseID,@ListName"
                    , para.ToArray()).FirstOrDefault();

                if (result == null)
                {
                    result = new UserNarrowSearchSettingsDTO();
                }

                result.EnterpriseID = enterpriseID;
                result.CompanyID = companyID;
                result.RoomID = roomID;
                result.UserID = userId;
                result.ListName = listName;

                return result;
            }
        }


        public Tuple<bool,long> SaveUserNarrowSearchSettings(UserNarrowSearchSettingsDTO obj)
        {
            bool isSave = false;
            long retId = 0;
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@ID", obj.ID),
                    new SqlParameter("@UserID", obj.UserID),
                    new SqlParameter("@RoomID", obj.RoomID),
                    new SqlParameter("@CompanyID", obj.CompanyID),
                    new SqlParameter("@EnterpriseID", obj.EnterpriseID),
                    new SqlParameter("@ListName", obj.ListName),
                    new SqlParameter("@SettingsJson", obj.SettingsJson),
                    new SqlParameter("@CreatedBy", obj.CreatedBy),
                };

                SaveUserNarrowSearchRespDTO result = context.Database.SqlQuery<SaveUserNarrowSearchRespDTO>("exec SaveUserNarrowSearchSettings " +
                    "@ID,@UserID,@RoomID,@CompanyID,@EnterpriseID,@ListName, @SettingsJson,@CreatedBy"
                    , para.ToArray()).FirstOrDefault();

                isSave = result.Status == "success";
                retId = result.ID;
            }

            return Tuple.Create<bool,long>(isSave, retId);
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
        // ~UserNarrowSearchSettingsDAL()
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
    }// class

    
} // ns
