using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.RedisCache_Remove
{
    class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new eTurnsRedisCache_Remove()
            };
            ServiceBase.Run(ServicesToRun);

            //RedisCacheRemoveHelper objRedisCacheRemoveHelper = new RedisCacheRemoveHelper();
            //objRedisCacheRemoveHelper.ProcessRemoveRedisCache();
        }
    }
}
