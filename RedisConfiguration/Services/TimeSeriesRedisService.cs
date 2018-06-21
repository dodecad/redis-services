using Newtonsoft.Json;
using RedisConfiguration.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace RedisConfiguration.Services
{
    public class TimeSeriesRedisService<T> : BaseRedisService<T>
    {
        public TimeSeriesRedisService(IRedisConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }

        public string Key => Name.ToLower();

        public void Add(T obj, double score)
        {
            if (obj == null)
                return;

            var member = JsonConvert.SerializeObject(obj);

            Db.SortedSetAdd(Key, member, score);
        }

        public IEnumerable<T> GetRange(double start, double stop)
        {
            var members = Db.SortedSetRangeByScore(Key, start, stop);

            if (members == null)
                return Enumerable.Empty<T>();

            return members.Select(m => JsonConvert.DeserializeObject<T>(m));
        }
    }
}
