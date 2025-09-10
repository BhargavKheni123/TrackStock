using System;

namespace eTurnsMaster.DAL
{
    public class UserLicenceDAL : eTurnsMasterBaseDAL
    {

        public void InsertLicenceAccept(long UserID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UserLicenceAccept obj = new UserLicenceAccept();

                obj.ID = 0;
                obj.UserID = UserID;
                obj.LicenceAcceptDate = DateTime.UtcNow;

                context.UserLicenceAccepts.Add(obj);
                context.SaveChanges();
            }

        }
    }
}
