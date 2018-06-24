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
        public string WebProductsPictureFullPath { get; set; }
        public string WebProductTypesPictureUri { get; set; }
        public string WebProductTypesPictureFullPath { get; set; }        
    }
}
