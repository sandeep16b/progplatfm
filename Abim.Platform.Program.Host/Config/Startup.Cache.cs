extern alias RedisBase;             //remember to change the reference Alias to RedisBase
using Abim.Platform.Program.App.Services;
using CacheManager.Core;
using NHibernate.Caches.Redis;

namespace Abim.Platform.Program.Host.Config
{
    public partial class Startup
    {
        /// <summary>
        /// UseServiceCacheManagement method.
        /// </summary>
     
        public static CacheManagerConfiguration UseServiceCacheManagement()
        {
            Logger.Trace("UseServiceCacheManagement: Initializing cache.");
            
            var cfg = new System.Configuration.Abstractions.ConfigurationManager();
            
            var config = ConfigurationBuilder.BuildConfiguration(settings =>
            {
                settings.WithSystemRuntimeCacheHandle("inprocess")
                .And
                .WithRedisConfiguration("redis", c =>
                {
                    c.WithAllowAdmin()
                        .WithDatabase(0)
                        .WithEndpoint(cfg.AppSettings["Redis.Host"], int.Parse(cfg.AppSettings["Redis.Port"]));
                })
                .WithMaxRetries(100)
                .WithRetryTimeout(50)
                .WithRedisBackplane("redis")
                .WithRedisCacheHandle("redis", true);
            });

            Logger.Trace("UseServiceCacheManagement: Cache Initialized.");
            return config;
        }
        
        /// <summary>
        /// UseOrmCache method.
        /// </summary>
        public static void UseOrmCache()
        {
            var cfg = new System.Configuration.Abstractions.ConfigurationManager();
            var connectionMultiplexer = RedisBase::StackExchange.Redis.ConnectionMultiplexer.Connect("localhost:32768,allowAdmin=true");
            
            connectionMultiplexer.GetServer(cfg.AppSettings["Redis.Host"], int.Parse(cfg.AppSettings["Redis.Port"])).FlushAllDatabases();
            
            RedisCacheProvider.SetConnectionMultiplexer(connectionMultiplexer);
            RedisCacheProvider.SetOptions(new RedisCacheProviderOptions()
            {
                Serializer = new NetDataContractCacheSerializer()
            });
            
            //TODO: Dispose connectionMultiplexor on application exit.
        }
        
        /// <summary>
        /// Preloads lookup table data into NCache.
        /// </summary>
        public static void NCachePreLoad()
        {
            var comfortAidService = Container.GetInstance<ICertificationService>();
            comfortAidService.Search();
            
            var planService = Container.GetInstance<ISourceService>();
            planService.Search();
        }
    }
}
