using ApplicationCore;
using Backoffice.Pages;
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

            var fileNameWithExtension = formFile.FileName.Substring(formFile.FileName.LastIndexOf('\\') + 1).Trim('"');
            var extension = fileNameWithExtension.Substring(fileNameWithExtension.LastIndexOf('.'));
            var fileNameSuglify = Utils.URLFriendly(fileNameWithExtension.Substring(0, fileNameWithExtension.LastIndexOf('.')));
            return fileNameSuglify + extension;
        }

        public static string GetFileNameSimplify(this IFormFile formFile)
        {
            var fullFileName = GetFileName(formFile);
            return fullFileName.Substring(0, fullFileName.LastIndexOf('.'));
        }

        public static string GetExtension(this IFormFile formFile)
        {
            return Path.GetExtension(formFile.FileName).ToLowerInvariant();
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
