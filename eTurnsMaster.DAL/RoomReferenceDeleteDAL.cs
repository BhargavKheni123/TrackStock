using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;

namespace eTurnsMaster.DAL
{
    public class RoomReferenceDeleteDAL : eTurnsMasterBaseDAL
    {
        /// <summary>
        /// Insert Record in the DataBase User Setting
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(RoomReferenceDeleteDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                RoomReferencesDeleteScheduleDetail obj = new RoomReferencesDeleteScheduleDetail();
                obj.RoomIds = objDTO.RoomIds;
                obj.EnterpriseDBName = objDTO.EnterpriseDBName;
                obj.EnterpriseId = objDTO.EnterpriseId;
                obj.CompanyId = objDTO.CompanyId;
                obj.ProcessStatus = objDTO.ProcessStatus;
                context.RoomReferencesDeleteScheduleDetails.Add(obj);
                context.SaveChanges();
                return obj.Id;
            }
        }

        public void UpdatetStatusForSchedule(Int64 id, int Status)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                if (context.RoomReferencesDeleteScheduleDetails.Any(t => t.Id == id))
                {
                    context.RoomReferencesDeleteScheduleDetails.First(t => t.Id == id).ProcessStatus = Status;
                    context.SaveChanges();
                }
            }
        }

        public bool RoomReferenceDeleteScheduleStatus()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool isProgress = (from del in context.RoomReferencesDeleteScheduleDetails
                                   where del.ProcessStatus == 1
                                   select true).FirstOrDefault();

                return isProgress;
            }
        }

        public List<RoomReferenceDeleteDTO> GetRoomDeleteScheduleDetails()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                List<RoomReferenceDeleteDTO> lstRoomReferenceDeleteDTO = new List<RoomReferenceDeleteDTO>();

                lstRoomReferenceDeleteDTO = (from dtlRooms in context.RoomReferencesDeleteScheduleDetails
                                             where dtlRooms.ProcessStatus == 0
                                             orderby dtlRooms.EnterpriseDBName
                                             select new RoomReferenceDeleteDTO
                                             {
                                                 Id = dtlRooms.Id,
                                                 RoomIds = dtlRooms.RoomIds,
                                                 EnterpriseDBName = dtlRooms.EnterpriseDBName,
                                                 EnterpriseId = dtlRooms.EnterpriseId.Value,
                                                 CompanyId = dtlRooms.CompanyId.Value,
                                                 ProcessStatus = dtlRooms.ProcessStatus.Value
                                             }).ToList();

                return lstRoomReferenceDeleteDTO;
            }
        }
    }
}
