using ApplicationCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore
{
    public class BackofficeSettings : AppSettings
    {
        public string WebNewsPictureUri { get; set; }
        public string WebNewsPictureFullPath { get; set; }
        public string WebProductsPictureUri { get; set; }
        public string WebProductsPictureV2Uri { get; set; }
        public string WebProductsPictureFullPath { get; set; }
        public string WebProductsPictureV2FullPath { get; set; }
        public string WebProductTypesPictureUri { get; set; }
        public string WebProductTypesPictureV2Uri { get; set; }
        public string WebProductTypesPictureFullPath { get; set; }
        public string WebProductTypesPictureV2FullPath { get; set; }
        public string InvoicesFolderFullPath { get; set; }
        public string InvoiceGroceryNameFormat { get; set; }
        public string GroceryProductsPictureFullPath { get; set; }
        public string GroceryProductsPictureUri { get; set; }
        
    }
}
