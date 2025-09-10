using eTurns.DTO;
using eTurnsMaster.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace eTurns.eVMIBAL.Wrappers
{
    public static class SharedData
    {
        private static List<EVMIRoomDTO> _eVMIRooms = null;
        public static List<EVMIRoomDTO> eVMIRooms
        {
            get
            {
                if (_eVMIRooms == null || _eVMIRooms.Count == 0)
                {
                    UpdateEVMIRoomsSharedData();
                }
                return _eVMIRooms;
            }
        }

        /// <summary>
        /// Update eVMIRooms variable with data from DB
        /// </summary>
        public static void UpdateEVMIRoomsSharedData()
        {
            //default eVMIServer = 1 (sapphire sever= 72 ), 2= (trackstock server = 225)
            string eVMIServer = ConfigurationManager.AppSettings["eVMIServer"] != null ? ConfigurationManager.AppSettings["eVMIServer"].ToString() : "1";
            string eVMIEnterpriseList = ConfigurationManager.AppSettings["eVMIEnterpriseList"] != null ? ConfigurationManager.AppSettings["eVMIEnterpriseList"].ToString() : "1";
            EnterpriseMasterDAL objEntDAL = new EnterpriseMasterDAL();
            List<EVMIRoomDTO> lstEntDTO = objEntDAL.GetEVMIRooms(eVMIServer, eVMIEnterpriseList);
            _eVMIRooms = lstEntDTO;
        }



    }

    public class EVMICommon
    {
        public static bool IsOldeVMIRoom(Int64 RoomID)
        {
            string streVMIRooms = eTurns.DTO.SiteSettingHelper.eVMIRooms;
            bool _IsOldeVMIRoom = false;
            if (!string.IsNullOrWhiteSpace(streVMIRooms))
            {
                string[] arrEntCmpRoom = streVMIRooms.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrEntCmpRoom != null && arrEntCmpRoom.Length > 0)
                {
                    foreach (string strEntCmpRoom in arrEntCmpRoom)
                    {
                        string[] EntCmpRoom = strEntCmpRoom.Split(new char[1] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                        if (EntCmpRoom != null && EntCmpRoom.Length > 0 && RoomID == Convert.ToInt64(EntCmpRoom[2]))
                        {
                            _IsOldeVMIRoom = true;
                            break;
                        }
                    }
                }
            }

            return _IsOldeVMIRoom;
        }

        public static bool IsOldeVMIRoom(long EnterpriseID, long CompanyID, long RoomID)
        {
            bool _IsOldeVMIRoom = false;
            string CurrentRoomFullId = EnterpriseID + "_" + CompanyID + "_" + RoomID;
            if ((SiteSettingHelper.eVMIRooms ?? string.Empty).ToLower().Contains(CurrentRoomFullId.ToLower()))
            {
                _IsOldeVMIRoom = true;
            }

            return _IsOldeVMIRoom;
        }

    }

    public class SystemUser
    {

        private static string EnterpriseDBName { get; set; }
        private static UserMasterDTO User { get; set; }

        /// <summary>
        /// Get new user record only if db name is changed
        /// </summary>
        /// <param name="enterpriseDbName"></param>
        /// <returns></returns>
        public static UserMasterDTO GetSystemUser(string enterpriseDbName)
        {
            if (string.IsNullOrEmpty(EnterpriseDBName) || EnterpriseDBName != enterpriseDbName)
            {
                EnterpriseDBName = enterpriseDbName;
                UserMasterDTO objeturnsUserMasterDTO = new UserMasterDTO();
                eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(EnterpriseDBName);
                objeturnsUserMasterDTO = objUserMasterDAL.GetSystemUser(4);
                User = objeturnsUserMasterDTO;
            }
            return User;
            //return objeturnsUserMasterDTO;
        }

    }
}
