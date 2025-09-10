using System;

namespace eTurns.DTO
{
    public class UserTokenInfoDTO
    {
        public long ID { get; set; }

        public Guid? Guid { get; set; }

        public string Username { get; set; }

        public string BearerToken { get; set; }

        public DateTime? ExpiredOn { get; set; }

        public DateTime Created { get; set; }

        public bool IsActive { get; set; }

    }
}
