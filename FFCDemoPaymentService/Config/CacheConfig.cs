using System.Data.Common;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Core;

namespace FFCDemoPaymentService.Data
{
    public class CacheConfig
    {

        public string RedisPassword { get; set; }
        public string RedisDb { get; set; }
        public string RedisHost { get; set; }
        public string RedisPort { get; set; }        
        public string ConnectionString
        {
            get => $"{RedisHost}:{RedisPort},password={RedisPassword},ssl=True,abortConnect=False";
        }
    }
}
