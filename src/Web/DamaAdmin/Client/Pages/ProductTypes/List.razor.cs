using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using DamaAdmin.Client;
using DamaAdmin.Client.Shared;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using DamaAdmin.Client.Services;
using DamaAdmin.Shared.Features;

namespace DamaAdmin.Client.Pages.ProductTypes
{
    [Authorize]
    public partial class List : ComponentBase
    {
        private PagingParameters _pagingParameters = new();     

        public Modal Modal { get; set; }
        public List<ProductTypeViewModel> ProductTypeList { get; set; }
        public MetaData MetaData { get; set; } = new();           

        [Parameter]
        public string Message { get; set; }

        [Inject]
        public ProductTypeService ProductTypeService { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await GetProductTypes();
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        private async Task OnRemoveItem(ProductTypeViewModel item)
        {
            if (!await JSRuntime.InvokeAsync<bool>("confirm", $"Tens a certeza que queres remover o tipo de produto {item.Name}?"))
                return;
            var response = await ProductTypeService.Delete(item.Id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = $"Tipo de Producto {item.Name} foi removido!";
                await GetProductTypes();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Message = $"Erro ao remover tipo de produto {item.Name}: ({response.StatusCode}) {content}";
            }
        }
        private async Task SelectedPage(int page)
        {
            _pagingParameters.PageNumber = page;
            await GetProductTypes();
        }

        private async Task GetProductTypes()
        {
            var pagingResponse = await ProductTypeService.List(_pagingParameters);
            ProductTypeList = pagingResponse.Items;
            MetaData = pagingResponse.MetaData;
        }
    }
}