using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backoffice.Pages
{
    public static class Utils
    {
        public static string GetFileName(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return "";
            return uri.Substring(uri.LastIndexOf('/') + 1)
                .Trim('"');
        }
        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string StringToNormalizeString(string text)
        {
            return RemoveDiacritics(text.Replace(' ', '-').ToLower());
        }

        public static string EnsureValidSlug(string slug)
        {
            return RemoveDiacritics(slug.TrimEnd().Replace(' ', '-'));
        }
    }
}
