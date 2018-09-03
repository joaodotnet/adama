using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DamaNoJornal.Core.Helpers
{
    public static class Utils
    {
        public static string GetLoginPicturiSource(string name)
        {
            string iconPath = "";
            switch (name)
            {
                case "João":
                    iconPath = "Assets\\img-joao.png";
                    break;
                case "Susana":
                    iconPath = "Assets\\img-sue.png";
                    break;
                case "Sónia":
                    iconPath = "Assets\\img-sonia.png";
                    break;
                case "Rute":
                    iconPath = "Assets\\img-rute.png";
                    break;
                default:
                    break;
            }
            return iconPath;
        }

        public static string GetPlacePictureSource(string placeId)
        {

            string iconPath = "";
            switch (placeId)
            {
                case "1":
                    iconPath = "Assets\\img-loule.jpg";
                    break;
                case "2":
                    iconPath = "Assets\\img-serra.jpg";
                    break;
                case "3":
                    iconPath = "Assets\\img-mercado.jpg";
                    break;
                default:
                    break;
            }
            return iconPath;
        }
    }
}
