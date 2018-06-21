using RedisConfiguration.Abstractions;
using StackExchange.Redis;
using System;

namespace RedisConfiguration
{
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        private readonly Lazy<ConnectionMultiplexer> _connection;

        public RedisConnectionFactory(RedisConfigurationOptions options)
        {
            _connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options.Host));
        }

        public ConnectionMultiplexer Connection => _connection.Value;
    }
}
