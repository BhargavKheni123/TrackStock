using System;

namespace eTurns.DTO
{
    public class NarrowSearchDTO
    {
        public string NSColumnValue { get; set; }
        public string NSColumnText { get; set; }
        public int NSCount { get; set; }
    }

    public class NarrowSearchSceduleDTO
    {
        public Int64 NSColumnValue { get; set; }
        public string NSColumnText { get; set; }
        public int NSCount { get; set; }
        public string ResourceKeyName { get; set; }
    }
}
