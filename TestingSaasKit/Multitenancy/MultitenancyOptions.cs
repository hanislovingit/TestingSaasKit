using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace TestingSaasKit.Multitenancy
{
    public class MultitenancyOptions
    {
        public Collection<AppTenant> Tenants { get; set; }
    }
}
