using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestingSaasKit.Multitenancy
{
    public class AppTenant
    {
        public string Name { get; set; }
        public string[] Hostnames { get; set; }
        public string Theme { get; set; }

    }
}
