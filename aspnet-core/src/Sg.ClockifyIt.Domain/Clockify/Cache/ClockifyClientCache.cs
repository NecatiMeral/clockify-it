using System;
using System.Collections.Generic;
using Clockify.Net;
using Volo.Abp.DependencyInjection;

namespace Sg.ClockifyIt.ClockifyIt.Cache
{
    /// <summary>
    /// Serves as a internal cache for <see cref="ClockifyItClient"/> instances.
    /// </summary>
    public class ClockifyItClientCache : ISingletonDependency
    {
        private readonly IDictionary<string, ClockifyClient> _cache;

        public ClockifyItClientCache()
        {
            _cache = new Dictionary<string, ClockifyClient>();
        }

        public ClockifyClient GetOrAdd(string key, Func<ClockifyClient> factory)
        {
            return _cache.GetOrAdd(key, factory);
        }
    }
}
