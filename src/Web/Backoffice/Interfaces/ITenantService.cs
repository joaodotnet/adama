using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.Interfaces
{
    interface ITenantService
    {
        string GetCurrentTenant();
    }
}
