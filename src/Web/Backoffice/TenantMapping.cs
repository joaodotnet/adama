using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice
{
    public class TenantMapping
    {
        public string Default { get; set; }
        public Dictionary<string, string> Tenants { get; } = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

    }
}
