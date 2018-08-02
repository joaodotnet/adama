using Backoffice.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.Services
{
    public class TenantService : ITenantService
    {
        private readonly HttpContext _httpContext;
        private readonly ITenantIdentificationService _service;

        public TenantService(IHttpContextAccessor accessor, ITenantIdentificationService service)
        {
            this._httpContext = accessor.HttpContext;
            this._service = service;
        }

        public string GetCurrentTenant()
        {
            return this._service.GetCurrentTenant(this._httpContext);
        }

    }
}
