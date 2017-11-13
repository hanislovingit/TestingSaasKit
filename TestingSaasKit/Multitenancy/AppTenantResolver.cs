using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;

namespace TestingSaasKit.Multitenancy
{
    public class AppTenantResolver: ITenantResolver<AppTenant>
    {
        private readonly IEnumerable<AppTenant> _tenants;

        public AppTenantResolver(IOptions<MultitenancyOptions> options)
        {
            this._tenants = options.Value.Tenants;
        }

        public async Task<TenantContext<AppTenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<AppTenant> tenantContext = null;

            var tenant = _tenants.FirstOrDefault(t =>
                t.Hostnames.Any(h => h.Equals(context.Request.Host.Value.ToLower())));

            if (tenant != null)
            {
                tenantContext = new TenantContext<AppTenant>(tenant);
            }

            return await Task.FromResult(tenantContext);
        }
    }
}
