using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.Interfaces
{
    public interface IBackofficeService
    {
        Task<string> SaveFileAsync(IFormFile formFile, string fullPath, string uriPath);        
    }
}
