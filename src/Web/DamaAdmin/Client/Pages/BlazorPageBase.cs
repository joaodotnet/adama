using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using DamaAdmin.Client.Services;
using DamaAdmin.Shared.Features;
using DamaAdmin.Client.Components;

namespace DamaAdmin.Client.Pages
{
    [Authorize]
    public class BlazorPageBase<TModel, TService> : ComponentBase
    where TModel: BaseViewModel
    where TService: HttpService<TModel>
    {
        public Modal Modal { get; set; } = new();
        public List<TModel> Items { get; set; }      
        public MetaData MetaData { get; set; } = new();   
        protected PagingParameters _pagingParameters = new();

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public TService Service { get; set; }

        [Parameter]
        public string Message { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await GetData();
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        protected async Task OnRemoveItem(TModel item)
        {
            if (!await JSRuntime.InvokeAsync<bool>("confirm", $"Tens a certeza que queres remover {item.Name}?"))
                return;
            var response = await Service.Delete(item.Id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = $"{item.Name} foi removido!";
                await GetData();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Message = $"Erro ao remover {item.Name}: ({response.StatusCode}) {content}";
            }
        }
        protected async Task SelectedPage(int page)
        {
            _pagingParameters.PageNumber = page;
            await GetData();
        }

        protected async Task GetData()
        {
            var pagingResponse = await Service.List(_pagingParameters);
            Items = pagingResponse.Items;
            MetaData = pagingResponse.MetaData;
        }
    }
    // [Authorize]
    // public partial class List : ComponentBase
    // {
    //     private PagingParameters _pagingParameters = new();     

    //     public Modal Modal { get; set; }
    //     public List<ProductTypeViewModel> ProductTypeList { get; set; }
    //     public MetaData MetaData { get; set; } = new();           

    //     [Parameter]
    //     public string Message { get; set; }

    //     [Inject]
    //     public ProductTypeService ProductTypeService { get; set; }

    //     [Inject]
    //     public IJSRuntime JSRuntime { get; set; }

    //     protected override async Task OnInitializedAsync()
    //     {
    //         try
    //         {
    //             await GetProductTypes();
    //         }
    //         catch (AccessTokenNotAvailableException exception)
    //         {
    //             exception.Redirect();
    //         }
    //     }

    //     private async Task OnRemoveItem(ProductTypeViewModel item)
    //     {
    //         if (!await JSRuntime.InvokeAsync<bool>("confirm", $"Tens a certeza que queres remover o tipo de produto {item.Name}?"))
    //             return;
    //         var response = await ProductTypeService.Delete(item.Id);
    //         if (response.StatusCode == System.Net.HttpStatusCode.OK)
    //         {
    //             Message = $"Tipo de Producto {item.Name} foi removido!";
    //             await GetProductTypes();
    //         }
    //         else
    //         {
    //             var content = await response.Content.ReadAsStringAsync();
    //             Message = $"Erro ao remover tipo de produto {item.Name}: ({response.StatusCode}) {content}";
    //         }
    //     }
    //     private async Task SelectedPage(int page)
    //     {
    //         _pagingParameters.PageNumber = page;
    //         await GetProductTypes();
    //     }

    //     private async Task GetProductTypes()
    //     {
    //         var pagingResponse = await ProductTypeService.List(_pagingParameters);
    //         ProductTypeList = pagingResponse.Items;
    //         MetaData = pagingResponse.MetaData;
    //     }
    // }
}