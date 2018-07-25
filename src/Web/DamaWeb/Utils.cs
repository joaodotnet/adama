using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamaWeb
{
    public static class Utils
    {
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

        public static string StringToUri(string text)
        {
            return RemoveDiacritics(text.Replace(' ', '-').ToLower());
        }

        public static string FixBasePath(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.LastIndexOf("/loja") >= 0)
            {
                var url = returnUrl.Substring(returnUrl.LastIndexOf("/loja") + 5);
                if (string.IsNullOrEmpty(url) || url == "/")
                    url = "/Index";
                return url;
            }
                
            return returnUrl;
        }
    }
}
