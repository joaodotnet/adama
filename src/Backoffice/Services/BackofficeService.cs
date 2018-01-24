using Backoffice.Extensions;
using Backoffice.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.Services
{
    public class BackofficeService : IBackofficeService
    {
        public bool CheckIfFileExists(string fullpath, string fileName)
        {
            return System.IO.File.Exists(Path.Combine(fullpath,fileName));
        }

        public void DeleteFile(string fullpath, string fileName)
        {
            if (System.IO.File.Exists(Path.Combine(fullpath, fileName)))
                System.IO.File.Delete(Path.Combine(fullpath, fileName));
        }

        public async Task<string> SaveFileAsync(IFormFile formFile, string fullPath, string uriPath)
        {
            var filename = formFile.GetFileName();

            var filePath = Path.Combine(
                fullPath,
                filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            return uriPath + filename; 
        }


    }
}
