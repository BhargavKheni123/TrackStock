namespace eTurns.DTO
{
    public class RedisCacheKeyDTO
    {
        public long Id { get; set; }
        public long EnterpriseId { get; set; }
        public long CompanyId { get; set; }
        public long RoomId { get; set; }
        public string CacheKeyName { get; set; }
    }
}
