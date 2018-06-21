using StackExchange.Redis;

namespace RedisConfiguration.Abstractions
{
    public interface IRedisConnectionFactory
    {
        ConnectionMultiplexer Connection { get; }
    }
}
