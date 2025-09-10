using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;

namespace eTurns.DAL
{
    public class UserSettingDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]
        //public AssetMasterDAL(base.DataBaseName)
        //{

        //}
        public UserSettingDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        public UserSettingDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        {
            DataBaseName = DbName;
            DBServerName = DBServerNm;
            DBUserName = DBUserNm;
            DBPassword = DBPswd;
        }
        #endregion

        /// <summary>
        /// Insert Record in the DataBase User Setting
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(UserSettingDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UserSetting obj = new UserSetting();
                obj.UserId = objDTO.UserId;
                obj.IsRemember = objDTO.IsRemember;
                obj.RedirectURL = objDTO.RedirectURL;
                context.UserSettings.Add(obj);
                context.SaveChanges();
                return obj.Id;
            }
        }

        public Int64 Update(UserSettingDTO objDTO)
        {
            UserSetting objUserSetting = new UserSetting();
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objUserSetting = context.UserSettings.FirstOrDefault(t => t.Id == objDTO.Id);
                if (objUserSetting != null)
                {
                    objUserSetting.IsRemember = objDTO.IsRemember;
                    objUserSetting.RedirectURL = objDTO.RedirectURL;
                }
                context.SaveChanges();
                return objDTO.Id;
            }
        }

        public UserSettingDTO GetByUserId(long userId)
        {
            UserSetting objUserSetting = new UserSetting();
            UserSettingDTO objUserSettingDTO = new UserSettingDTO();
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objUserSetting = context.UserSettings.FirstOrDefault(t => t.UserId == userId);
                if (objUserSetting != null)
                {
                    objUserSettingDTO.Id = objUserSetting.Id;
                    objUserSettingDTO.IsRemember = objUserSetting.IsRemember.Value;
                    objUserSettingDTO.RedirectURL = objUserSetting.RedirectURL;
                }
                return objUserSettingDTO;
            }
        }
    }
}
