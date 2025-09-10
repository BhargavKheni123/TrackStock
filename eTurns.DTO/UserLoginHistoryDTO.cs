using System;

namespace eTurns.DTO
{
    public class UserLoginHistoryDTO
    {
        public long ID { get; set; }
        public long UserId { get; set; }
        public DateTime EventDate { get; set; }
        public short EventType { get; set; }
        public string IpAddress { get; set; }
        public long RoomId { get; set; }
        public long CompanyId { get; set; }
        public long EnterpriseId { get; set; }
        public string CountryName { get; set; }
        public string RegionName { get; set; }
        public string CityName { get; set; }
        public string ZipCode { get; set; }
    }
}
