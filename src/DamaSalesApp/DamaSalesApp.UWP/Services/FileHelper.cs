 
using System.IO;
using Windows.Storage;
using System.Collections.Generic;
using Xamarin.Forms;
using System;
using DamaSalesApp.UWP.Services;
using DamaSalesApp.Interfaces;

[assembly: Dependency(typeof(FileHelper))]
namespace DamaSalesApp.UWP.Services
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, filename);
        }

        public List<string> GetSpecialFolders()
        {
            return new List<string>();
        }
    }
}