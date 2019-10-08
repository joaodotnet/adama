using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApplicationCore
{
    public static class Utils
    {
        private static readonly Regex reSlugCharactersToBeDashes = new Regex(@"([\s,.//\\-_=])+");
        private static readonly Regex reSlugCharactersToRemove = new Regex(@"([^0-9a-z\-])+");
        private static readonly Regex reSlugDashes = new Regex(@"([\-])+");
        private static readonly Regex reSlugCharacters = new Regex(@"([\s,.//\\-_=])+");

        public static string URLFriendly(string title)
        {
            if (string.IsNullOrEmpty(title)) return "";

            var newTitle = RemoveDiacritics(title);

            newTitle = Regex.Replace(newTitle, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", @"-$1");

            newTitle = reSlugCharactersToBeDashes.Replace(newTitle, "-");

            newTitle = newTitle.ToLowerInvariant();

            newTitle = reSlugCharactersToRemove.Replace(newTitle, "");

            newTitle = reSlugDashes.Replace(newTitle, "-");

            newTitle = newTitle.Trim('-');

            return newTitle;
        }

        public static string GetFileName(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return "";
            return uri.Substring(uri.LastIndexOf('/') + 1)
                .Trim('"');
        }

        static string RemoveDiacritics(string text)
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

        //public static string RemoveDiacritics(string text)
        //{
        //    var normalizedString = text.Normalize(NormalizationForm.FormD);
        //    var stringBuilder = new StringBuilder();

        //    foreach (var c in normalizedString)
        //    {
        //        var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
        //        if (unicodeCategory != UnicodeCategory.NonSpacingMark)
        //        {
        //            stringBuilder.Append(c);
        //        }
        //    }

        //    return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        //}

        public static string FixBasePath(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.LastIndexOf("/loja") >= 0)
            {
                returnUrl = returnUrl.Substring(returnUrl.LastIndexOf("/loja") + 5);
            }

            if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/")
                returnUrl = "/Index";
            return returnUrl;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }

        public static string TryTranslate(string description, string email)
        {
            if (description.LastIndexOf("is already taken.") > 0)
            {
                return $"O email '{email}' já se encontra registado.";
            }
            return description;

        }
    }
}
