using Backoffice.Extensions;
using Backoffice.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        private readonly DamaContext _db;
        public BackofficeService(DamaContext context)
        {
            _db = context;
        }
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

        public async Task<string> GetSku(int typeId, int illustationId, int illustrationTypeId, int? attributeId = null)
        {
            var type = await _db.CatalogTypes                
                .Where(x => x.Id == typeId)
                .Select(x => x.Code)
                .SingleAsync();
            var illustration = await _db.CatalogIllustrations
                .Where(x => x.Id == illustationId)
                .Select(x => x.Code)
                .SingleAsync();
            var illustrationType = await _db.IllustrationTypes
                .Where(x => x.Id == illustrationTypeId)
                .Select(x => x.Code)
                .SingleAsync();
            string sku = $"{type}_{illustration}_{illustrationType}";
            if (attributeId.HasValue)
                sku += $"_{attributeId}";
            return sku;
        }
    }
}
