using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.Extensions
{
    public static class FormFileExtensions
    {
        public static string GetFileName(this IFormFile formFile)
        {
            if (string.IsNullOrEmpty(formFile.FileName))
                return "";
            return formFile.FileName.Substring(formFile.FileName.LastIndexOf('\\') + 1)
                .Trim('"');            
        }

        public static string GetContentType(this IFormFile formFile)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();
            return types[ext];
        }

        private static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

    }
}
