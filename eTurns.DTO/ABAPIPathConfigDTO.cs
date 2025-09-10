using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class ABAPIPathConfigDTO
    {
        public long ID { get; set; }
        public string ABAPISection { get; set; }
        public string ABAPIModuleName { get; set; }
        public string ABAPIOperation { get; set; }
        public string ABAPIPath { get; set; }
        public string RequestType { get; set; }
        public DateTime Created { get; set; }
    }
}
