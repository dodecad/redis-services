using RedisConfiguration.Abstractions;
using StackExchange.Redis;
using System;
using System.Reflection;

namespace RedisConfiguration.Services
{
    public abstract class BaseRedisService<T>
    {
        protected BaseRedisService(IRedisConnectionFactory connectionFactory)
        {
            Db = connectionFactory.Connection.GetDatabase();
        }

        protected IDatabase Db { get; }

        protected string Name => Type.Name;

        protected PropertyInfo[] Properties => Type.GetProperties();

        protected Type Type => typeof(T);
    }
}
