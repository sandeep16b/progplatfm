using System;
using System.Collections.Generic;
using NLog;
using WebApi.OutputCache.Core.Cache;

namespace Abim.Platform.Program.WebApi.Objects.Logging
{
    /// <summary>
    /// Logs the fact that a cache response was returned to client
    /// </summary>
    public class LoggingCacheOutputProviderDecorator : IApiOutputCache
    {
        /// <summary>
        /// The cache
        /// </summary>
        private readonly IApiOutputCache _cache;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingCacheOutputProviderDecorator"/> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        public LoggingCacheOutputProviderDecorator(IApiOutputCache cache)
        {
            _cache = cache;
            _logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingCacheOutputProviderDecorator"/> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="logger">The logger.</param>
        public LoggingCacheOutputProviderDecorator(IApiOutputCache cache, ILogger logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Removes the starts with.
        /// </summary>
        /// <param name="key">The key.</param>
        public void RemoveStartsWith(string key)
        {
            _cache.RemoveStartsWith(key);
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            _logger.Debug("{0} was fetched from the cache. Object type {1}", key, typeof(T));
            return _cache.Get<T>(key);
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object Get(string key)
        {
            _logger.Debug("{0} was fetched from the cache.", key);
            return _cache.Get(key);
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            _logger.Debug("The key {0} was removed from the cache", key);
            _cache.Remove(key);
        }

        /// <summary>
        /// Determines whether [contains] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return _cache.Contains(key);
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="o">The o.</param>
        /// <param name="expiration">The expiration.</param>
        /// <param name="dependsOnKey">The depends on key.</param>
        public void Add(string key, object o, DateTimeOffset expiration, string dependsOnKey = null)
        {
            _logger.Debug("");
            _cache.Add(key, o, expiration, dependsOnKey);
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <value>
        /// All keys.
        /// </value>
        public IEnumerable<string> AllKeys
        {
            get { return _cache.AllKeys; }
            private set
            {
                var propInfo = _cache.GetType().GetProperty("AllKeys");
                var setMethodInfo = propInfo.GetSetMethod(true);
                setMethodInfo.Invoke(_cache, new object[] { value });
            }
        }
    }
}