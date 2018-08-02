using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Backoffice.Interfaces
{
    public interface ITenantIdentificationService
    {
        string GetCurrentTenant(HttpContext httpContext);
    }
}
