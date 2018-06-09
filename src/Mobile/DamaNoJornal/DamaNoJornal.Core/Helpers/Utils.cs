using System;
using System.Collections.Generic;
using System.Text;

namespace DamaNoJornal.Core.Helpers
{
    public static class Utils
    {
        public static (string Title, string PictureUri) GetLoginProfileInfo(string authAccessToken)
        {
            string title = "Olá ";
            string iconPath = "";
            switch (authAccessToken)
            {
                case GlobalSetting.JueAuthToken:
                    title += "João";
                    iconPath = "Assets\\img-joao.png";
                    break;
                case GlobalSetting.SueAuthToken:
                    title += "Susana";
                    iconPath = "Assets\\img-sue.png";
                    break;
                case GlobalSetting.SoniaAuthToken:
                    title += "Sónia";
                    iconPath = "Assets\\img-sonia.png";
                    break;
                default:
                    break;
            }
            return (title, iconPath);
        }
    }
}
