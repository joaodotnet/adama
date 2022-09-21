using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using DamaAdmin.Client;
using DamaAdmin.Client.Components;
using DamaAdmin.Client.Services;
using DamaAdmin.Client.Shared;
using DamaAdmin.Shared.Models;

namespace DamaAdmin.Client.Pages.Products
{
    public partial class List
    {
        private ProductViewModel model = new();
        [Inject]
        public ProductService ProductService { get; set; }

        private async Task UpdateFlag(ChangeEventArgs e, ProductViewModel item, int checkboxType)
        {
            var checkValue = (bool)e.Value;
            await ProductService.UpdateFlag(item.Id, checkboxType, checkValue);
        }
    }
}