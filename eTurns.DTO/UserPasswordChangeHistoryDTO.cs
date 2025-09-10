using System;

namespace eTurns.DTO
{
    public class UserPasswordChangeHistoryDTO
    {
        public System.Int64 ID { get; set; }
        public System.Int64 UserId { get; set; }
        public string oldPassword { get; set; }
        public string NewPassword { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
