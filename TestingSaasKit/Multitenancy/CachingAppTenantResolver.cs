using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;

namespace TestingSaasKit.Multitenancy
{
    /// <summary>
    /// The base class performs the cache lookup for you. It requires that you override the following methods:
    /// ResolveAsync - Resolve a tenant context from the current request.This will only be executed on cache misses.
    /// GetContextIdentifier - Determines what information in the current request should be used to do a cache lookup e.g.the hostname.
    /// GetTenantIdentifiers - Determines the identifiers (keys) used to cache the tenant context.
    /// In our example tenants can have multiple domains, so we return each of the hostnames as identifiers.
    /// </summary>
    public class CachingAppTenantResolver : MemoryCacheTenantResolver<AppTenant>
    {
        private readonly IEnumerable<AppTenant> tenants;

        public CachingAppTenantResolver(IMemoryCache cache, ILoggerFactory loggerFactory, IOptions<MultitenancyOptions> options)
            : base(cache, loggerFactory)
        {
            this.tenants = options.Value.Tenants;
        }

        protected override string GetContextIdentifier(HttpContext context)
        {
            return context.Request.Host.Value.ToLower();
        }

        protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<AppTenant> context)
        {
            return context.Tenant.Hostnames;
        }

        protected override Task<TenantContext<AppTenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<AppTenant> tenantContext = null;

            var tenant = tenants.FirstOrDefault(t =>
                t.Hostnames.Any(h => h.Equals(context.Request.Host.Value.ToLower())));

            if (tenant != null)
            {
                tenantContext = new TenantContext<AppTenant>(tenant);
            }

            return Task.FromResult(tenantContext);
        }

        /// <summary>
        /// By default, tenant contexts are cached for an hour but you can control this by overriding 
        /// </summary>
        /// <returns></returns>
        protected override MemoryCacheEntryOptions CreateCacheEntryOptions()
        {
            return new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(new TimeSpan(0, 30, 0)); // Cache for 30 minutes
        }
    }
}
