using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
