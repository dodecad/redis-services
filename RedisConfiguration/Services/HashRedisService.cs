using RedisConfiguration.Abstractions;
using StackExchange.Redis;
using System;
using System.Linq;

namespace RedisConfiguration.Services
{
    public class HashRedisService<T> : BaseRedisService<T>
    {
        public HashRedisService(IRedisConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }

        public void Delete(string key)
        {
            var redisKey = GenerateKey(key);
            Db.KeyDelete(redisKey);
        }

        public T Get(string key)
        {
            var redisKey = GenerateKey(key);
            var hash = Db.HashGetAll(redisKey);

            return MapFromHash(hash);
        }

        public void Save(string key, T obj)
        {
            if (obj == null)
                return;

            var hash = GenerateHash(obj);
            var redisKey = GenerateKey(key);

            Db.HashSet(redisKey, hash);
        }

        private string GenerateKey(string key)
        {
            return $"{Name.ToLower()}:{key.ToLower()}";
        }

        private HashEntry[] GenerateHash(T obj)
        {
            var hash = new HashEntry[Properties.Count()];

            for (var i = 0; i < Properties.Count(); i++)
            {
                var name = Properties[i].Name;
                var value = Properties[i].GetValue(obj).ToString();

                hash[i] = new HashEntry(name, value);
            }

            return hash;
        }

        private T MapFromHash(HashEntry[] hash)
        {
            var obj = (T)Activator.CreateInstance(Type);

            for (var i = 0; i < Properties.Count(); i++)
            {
                for (var j = 0; j < hash.Count(); j++)
                {
                    if (Properties[i].Name != hash[j].Name)
                        continue;

                    var value = hash[j].Value;
                    var type = Properties[i].PropertyType;

                    if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            Properties[i].SetValue(obj, null);
                            continue;
                        }
                    }

                    if (type.IsAssignableFrom(typeof(Guid)))
                    {
                        var guid = new Guid(value.ToString());
                        Properties[i].SetValue(obj, guid);
                        continue;
                    }

                    Properties[i].SetValue(obj, Convert.ChangeType(value, type));
                }
            }

            return obj;
        }
    }
}
