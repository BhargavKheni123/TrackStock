using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;

namespace eTurns.DAL
{
    public partial class EmailUserConfigurationDAL : eTurnsBaseDAL
    {
        public IEnumerable<EmailUserConfigurationDTO> GetAllUsers(string EmailTemplateName)
        {
            IEnumerable<EmailUserConfigurationDTO> ObjCache = null;
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<EmailUserConfigurationDTO> obj = (from u in context.ExecuteStoreQuery<EmailUserConfigurationDTO>
                                                                  (@"SELECT EM.*,ISNULL(ED.TemplateName,'') AS TemplateName, 
                           CAST( (CASE WHEN ED.TemplateName IS NULL THEN 0 ELSE 1 END) AS BIT ) AS ISChecked 
                            FROM EmailUserMaster EM WITH(NOLOCK) LEFT OUTER JOIN EmailUserMasterDetail ED  WITH(NOLOCK) ON EM.ID=ED.EmailUserID AND ED.TemplateName='" + EmailTemplateName + "'")
                                                                  select new EmailUserConfigurationDTO
                                                                  {
                                                                      ID = u.ID,
                                                                      Name = u.Name,
                                                                      Email = u.Email,
                                                                      IsChecked = u.IsChecked,
                                                                      TemplateName = u.TemplateName
                                                                  }).ToList();
                    ObjCache = obj;
                }
            }
            return ObjCache;
        }

        public IEnumerable<EmailUserConfigurationDTO> GetAllExternalUserRecords(string EmailTemplateName, bool IsSelectedOnly, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strquery = "";
                if (IsSelectedOnly)
                {
                    strquery = @"SELECT EM.ID,EM.Name,EM.Email,Convert(bit,(CASE WHEN ED.TemplateName IS NULL THEN 0 ELSE 1 END)) AS 'IsChecked', ISNULL(ED.TemplateName,'') AS 'TemplateName' FROM EmailUserMaster EM LEFT OUTER JOIN EmailUserMasterDetail ED ON EM.ID=ED.EmailUserID AND ED.TemplateName='" + EmailTemplateName + "' and ED.RoomId=" + RoomId + " and ED.CompanyId =" + CompanyId + " WHERE Convert(bit,(CASE WHEN ED.TemplateName IS NULL THEN 0 ELSE 1 END)) = 1";
                }
                else
                {
                    strquery = @"SELECT EM.ID,EM.Name,EM.Email,Convert(bit,(CASE WHEN ED.TemplateName IS NULL THEN 0 ELSE 1 END)) AS 'IsChecked', ISNULL(ED.TemplateName,'') AS 'TemplateName' FROM EmailUserMaster EM LEFT OUTER JOIN EmailUserMasterDetail ED ON EM.ID=ED.EmailUserID AND ED.TemplateName='" + EmailTemplateName + "' and ED.RoomId=" + RoomId + " and ED.CompanyId =" + CompanyId + "";
                }

                IEnumerable<EmailUserConfigurationDTO> obj = (from u in context.ExecuteStoreQuery<EmailUserConfigurationDTO>(strquery)
                                                              select new EmailUserConfigurationDTO
                                                              {
                                                                  ID = u.ID,
                                                                  Name = u.Name,
                                                                  Email = u.Email,
                                                                  IsChecked = u.IsChecked,
                                                                  TemplateName = u.TemplateName
                                                              }).ToList();
                return obj;
            }
        }

        public IEnumerable<EmailUserConfigurationDTO> GetAllRecords(string EmailTemplateName, bool IsSelectedOnly, string DBConnectionstring, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                string strquery = "";
                if (IsSelectedOnly)
                {
                    strquery = @"SELECT EM.ID,EM.Name,EM.Email,Convert(bit,(CASE WHEN ED.TemplateName IS NULL THEN 0 ELSE 1 END)) AS 'IsChecked', ISNULL(ED.TemplateName,'') AS 'TemplateName' FROM EmailUserMaster EM LEFT OUTER JOIN EmailUserMasterDetail ED ON EM.ID=ED.EmailUserID AND ED.TemplateName='" + EmailTemplateName + "' and ED.RoomId=" + RoomId + " and ED.CompanyId =" + CompanyId + " WHERE Convert(bit,(CASE WHEN ED.TemplateName IS NULL THEN 0 ELSE 1 END)) = 1";
                }
                else
                {
                    strquery = @"SELECT EM.ID,EM.Name,EM.Email,Convert(bit,(CASE WHEN ED.TemplateName IS NULL THEN 0 ELSE 1 END)) AS 'IsChecked', ISNULL(ED.TemplateName,'') AS 'TemplateName' FROM EmailUserMaster EM LEFT OUTER JOIN EmailUserMasterDetail ED ON EM.ID=ED.EmailUserID AND ED.TemplateName='" + EmailTemplateName + "' and ED.RoomId=" + RoomId + " and ED.CompanyId =" + CompanyId + "";
                }

                IEnumerable<EmailUserConfigurationDTO> obj = (from u in context.ExecuteStoreQuery<EmailUserConfigurationDTO>(strquery)
                                                              select new EmailUserConfigurationDTO
                                                              {
                                                                  ID = u.ID,
                                                                  Name = u.Name,
                                                                  Email = u.Email,
                                                                  IsChecked = u.IsChecked,
                                                                  TemplateName = u.TemplateName
                                                              }).ToList();
                return obj;
            }
        }

        public IEnumerable<EmailUserConfigurationDTO> GetAllRecords(string EmailTemplateName, bool IsSelectedOnly, long CompanyId, long RoomId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strquery = "";
                if (IsSelectedOnly)
                {
                    strquery = @"SELECT EM.ID,EM.Name,EM.Email,Convert(bit,(CASE WHEN ED.TemplateName IS NULL THEN 0 ELSE 1 END)) AS 'IsChecked', ISNULL(ED.TemplateName,'') AS 'TemplateName' FROM EmailUserMaster EM LEFT OUTER JOIN EmailUserMasterDetail ED ON EM.ID=ED.EmailUserID AND ED.TemplateName='" + EmailTemplateName + "' WHERE Convert(bit,(CASE WHEN ED.TemplateName IS NULL THEN 0 ELSE 1 END)) = 1 and EM.RoomId=" + RoomId + " and EM.CompanyId=" + CompanyId + " and EM.IsDeleted = 0 ";
                }
                else
                {
                    strquery = @"SELECT EM.ID,EM.Name,EM.Email,Convert(bit,(CASE WHEN ED.TemplateName IS NULL THEN 0 ELSE 1 END)) AS 'IsChecked', ISNULL(ED.TemplateName,'') AS 'TemplateName' FROM EmailUserMaster EM LEFT OUTER JOIN EmailUserMasterDetail ED ON EM.ID=ED.EmailUserID AND ED.TemplateName='" + EmailTemplateName + "' where EM.RoomId=" + RoomId + " and EM.CompanyId=" + CompanyId + " and EM.IsDeleted = 0 ";
                }

                IEnumerable<EmailUserConfigurationDTO> obj = (from u in context.ExecuteStoreQuery<EmailUserConfigurationDTO>(strquery)
                                                              select new EmailUserConfigurationDTO
                                                              {
                                                                  ID = u.ID,
                                                                  Name = u.Name,
                                                                  Email = u.Email,
                                                                  IsChecked = u.IsChecked,
                                                                  TemplateName = u.TemplateName
                                                              }).ToList();
                return obj;
            }
        }

        public string UpdateData(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE EmailUserMaster SET " + columnName + " = '" + value + "' WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;
        }
    }
}
