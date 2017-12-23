using Cauldron.Interception;
using System;
using System.ComponentModel;
using System.Runtime.Caching;
using System.Text;
using System.Threading;

namespace Cauldron.Core.Interceptors
{
    /// <summary>
    /// Provides a timed global caching for the intercepted method. The caching is implemented using
    /// <see cref="MemoryCache.Default"/>
    /// <para/>
    /// The cache is dependent to the passed arguments. The arguments requires a proper
    /// implementation of <see cref="object.GetHashCode"/> and a unique <see cref="object.ToString"/> value.
    /// <para/>
    /// The cache of the <see cref="TimedCacheAttribute"/> can be forcefully cleared by <see cref="TimedCacheChangeMonitor.Clear"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class TimedCacheAttribute : Attribute, IInterceptor
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private TimeSpan decayPeriod;

        /// <summary>
        /// Initializes a new instance of <see cref="TimedCacheAttribute"/>
        /// </summary>
        /// <param name="decayPeriodInSeconds">The maximum period of cache lifetime in seconds</param>
        public TimedCacheAttribute(uint decayPeriodInSeconds)
        {
            this.decayPeriod = TimeSpan.FromSeconds(decayPeriodInSeconds);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="TimedCacheAttribute"/>
        /// <para/>
        /// Default cache decay period is 30 minutes.
        /// </summary>
        public TimedCacheAttribute() : this(1800)
        {
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string CreateKey(string methodName, object[] arguments)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < arguments.Length; i++)
                if (arguments[i] == null)
                    sb.Append(i.ToString() + "null");
                else
                    sb.Append(arguments[i].GetHashCode() + arguments[i].ToString());

            sb.Append(methodName);
            return sb.ToString().GetSHA256Hash();
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object GetCache(string key)
        {
            semaphore.Wait();

            try
            {
                var cache = MemoryCache.Default;
                return cache[key];
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool HasCache(string key)
        {
            var cache = MemoryCache.Default;
            var result = cache[key] != null;
            return result;
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetCache(string key, object content)
        {
            if (content == null)
                return;

            semaphore.Wait();

            try
            {
                var cache = MemoryCache.Default;

                var policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTimeOffset.Now.Add(this.decayPeriod);
                policy.ChangeMonitors.Add(new TimedCacheChangeMonitor());

                cache.Set(key, content, policy);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }

    /// <summary>
    /// A custom type that monitors changes in the state of the data which a cache item depends on.
    /// </summary>
    public sealed class TimedCacheChangeMonitor : ChangeMonitor
    {
        private string uniqueId;

        internal TimedCacheChangeMonitor()
        {
            this.uniqueId = Guid.NewGuid().ToString();
            ClearCache += TimedCacheChangeMonitor_ClearCache;
            base.InitializationComplete();
        }

        /// <summary>
        /// Occures if the cache is cleared
        /// </summary>
        public static event EventHandler ClearCache;

        /// <summary>
        /// Gets a unique id
        /// </summary>
        public override string UniqueId => this.uniqueId;

        /// <summary>
        /// Clears the timed cache.
        /// </summary>
        public static void Clear() => ClearCache?.Invoke(null, EventArgs.Empty);

        /// <exclude/>
        protected override void Dispose(bool disposing)
        {
            ClearCache -= TimedCacheChangeMonitor_ClearCache;
        }

        private void TimedCacheChangeMonitor_ClearCache(object sender, EventArgs e) => base.OnChanged(null);
    }
}