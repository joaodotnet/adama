using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.Extensions
{
    public static class ConfigurationExtensions
    {
        public static TenantMapping GetTenantMapping(this IConfiguration configuration)
        {
            return configuration.GetSection("Tenants").Get<TenantMapping>();
        }

    }
}
