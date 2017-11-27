using DamaSalesApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamaSalesApp.Helpers
{
    public static class StorageHelper
    {
        public static List<string> GetSpecialFolders()
        {
            return Xamarin.Forms.DependencyService.Get<IFileHelper>().GetSpecialFolders();
        }

        public static string GetLocalFilePath()
        {
            return Xamarin.Forms.DependencyService.Get<IFileHelper>().GetLocalFilePath("damasales.db3");
        }

    }
}
