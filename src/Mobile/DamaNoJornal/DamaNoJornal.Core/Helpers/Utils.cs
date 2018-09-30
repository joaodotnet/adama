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
                    //iconPath = "Assets\\img-joao.png";
                    iconPath = "img_joao.png";
                    break;
                case "Susana":
                    //iconPath = "Assets\\img-sue.png";
                    iconPath = "img_sue.png";
                    break;
                case "Sónia":
                    //iconPath = "Assets\\img-sonia.png";
                    iconPath = "img_sonia.png";
                    break;
                case "Rute":
                    //iconPath = "Assets\\img-rute.png";
                    iconPath = "img_rute.png";
                    break;
                default:
                    break;
            }
            return iconPath;
        }

        public static string GetPlacePictureSource(string placeId)
        {
            //TODO Check Plataform
            string iconPath = "";
            switch (placeId)
            {
                case "1":
                    //iconPath = "Assets\\img-loule.jpg";
                    iconPath = "img_loule.jpg";
                    break;
                case "2":
                    //iconPath = "Assets\\img-serra.jpg";
                    iconPath = "img_serra.jpg";
                    break;
                case "3":
                    //iconPath = "Assets\\img-mercado.jpg";
                    iconPath = "img_mercado.jpg";
                    break;
                default:
                    break;
            }
            return iconPath;
        }
    }
}
