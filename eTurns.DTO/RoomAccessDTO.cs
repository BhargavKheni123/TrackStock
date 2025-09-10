using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class RoomAccessDTO
    {
        public int RoomID { get; set; }
        public List<string> RoomNameList { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string User { get; set; }
        public string RoomName { get; set; }

        public RoomAccessDTO GetRoomInfo()
        {
            RoomAccessDTO objResult = new RoomAccessDTO();
            List<string> objList = new List<string>();

             objList.Add("Room 1");
            objList.Add("Room 2");
            objList.Add("Room 3");

            RoomAccessDTO obj1 = new RoomAccessDTO();
            obj1.RoomID = 1;
            obj1.RoomName = "Room 1";
            obj1.CreatedDate = DateTime.Now;
            obj1.UpdatedDate = DateTime.Now;
            obj1.User = "Virat";
            obj1.RoomNameList=objList;
            return obj1;                        
        }


    }
}
