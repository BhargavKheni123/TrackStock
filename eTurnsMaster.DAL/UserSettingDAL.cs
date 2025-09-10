using eTurns.DTO;
using System;
using System.Linq;
using eTurns.DTO;

namespace eTurnsMaster.DAL
{
    public class UserSettingDAL : eTurnsMasterBaseDAL
    {
        /// <summary>
        /// Insert Record in the DataBase User Setting
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(UserSettingDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UserSetting obj = new UserSetting();
                obj.UserId = objDTO.UserId;
                obj.IsRemember = objDTO.IsRemember;
                obj.RedirectURL = objDTO.RedirectURL;
                obj.IsRequistionReportDisplay = objDTO.IsRequistionReportDisplay ?? false;
                obj.IsNeverExpirePwd = objDTO.IsNeverExpirePwd;
                obj.ShowDateTime = objDTO.ShowDateTime;
                obj.IsAutoLogin = objDTO.IsAutoLogin;
                obj.SearchPattern = objDTO.SearchPattern;
                context.UserSettings.Add(obj);
                context.SaveChanges();
                return obj.Id;
            }
        }

        public Int64 Update(UserSettingDTO objDTO)
        {
            UserSetting objUserSetting = new UserSetting();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objUserSetting = context.UserSettings.FirstOrDefault(t => t.Id == objDTO.Id);
                if (objUserSetting != null)
                {
                    objUserSetting.IsRemember = objDTO.IsRemember;
                    objUserSetting.RedirectURL = objDTO.RedirectURL;
                    objUserSetting.IsNeverExpirePwd = objDTO.IsNeverExpirePwd;
                    objUserSetting.ShowDateTime = objDTO.ShowDateTime;
                    objUserSetting.IsRequistionReportDisplay = objDTO.IsRequistionReportDisplay;
                    objUserSetting.IsAutoLogin = objDTO.IsAutoLogin;
                    objUserSetting.SearchPattern = objDTO.SearchPattern;
                }

                context.SaveChanges();
                return objDTO.Id;
            }
        }

        public UserSettingDTO GetByUserId(long userId)
        {
            UserSetting objUserSetting = new UserSetting();
            UserSettingDTO objUserSettingDTO = new UserSettingDTO();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objUserSetting = context.UserSettings.FirstOrDefault(t => t.UserId == userId);
                if (objUserSetting != null)
                {
                    objUserSettingDTO.Id = objUserSetting.Id;
                    objUserSettingDTO.UserId = objUserSetting.UserId ?? 0;
                    objUserSettingDTO.IsRemember = objUserSetting.IsRemember.Value;
                    objUserSettingDTO.RedirectURL = objUserSetting.RedirectURL;
                    objUserSettingDTO.IsNeverExpirePwd = objUserSetting.IsNeverExpirePwd;
                    objUserSettingDTO.ShowDateTime = objUserSetting.ShowDateTime;
                    objUserSettingDTO.IsRequistionReportDisplay = objUserSetting.IsRequistionReportDisplay;
                    objUserSettingDTO.IsAutoLogin = objUserSetting.IsAutoLogin;
                    objUserSettingDTO.SearchPattern = objUserSetting.SearchPattern;
                    objUserSettingDTO.ShowABConsentRemider = objUserSetting.ShowABConsentRemider;
                }
                return objUserSettingDTO;
            }
        }
    }
}
