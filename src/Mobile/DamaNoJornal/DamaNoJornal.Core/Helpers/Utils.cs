using System;
using System.Collections.Generic;
using System.Text;

namespace DamaNoJornal.Core.Helpers
{
    public static class Utils
    {
        public static string GetLoginPicturiSource(string email)
        {
            string iconPath = "";
            switch (email)
            {
                case "jue@damanojornal.com":
                    iconPath = "Assets\\img-joao.png";
                    break;
                case "sue@damanojornal.com":
                    iconPath = "Assets\\img-sue.png";
                    break;
                case "sonia@damanojornal.com":
                    iconPath = "Assets\\img-sonia.png";
                    break;
                default:
                    break;
            }
            return iconPath;
        }
    }
}
