using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ApplicationCore.DTOs;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using DamaAdmin.Client.Shared;
using DamaAdmin.Client.HttpRepositories;
using Microsoft.AspNetCore.Authorization;

namespace DamaAdmin.Client.Pages.Categories
{
    [Authorize]
    public partial class List : ComponentBase
    {
        public List<CategoryDTO> CategoryList { get; set; }
        public MetaData MetaData { get; set; } = new();
        private PagingParameters _pagingParameters = new();
        private Modal Modal { get; set; }


        [Parameter]
        public string Message { get; set; }

        [Inject]
        public ICategoryHttpRepository CategoryRepo { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected async override Task OnInitializedAsync()
        {
            try
            {
                await GetCategories();
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        private async Task OnRemoveItem(CategoryDTO item)
        {
            if (!await JSRuntime.InvokeAsync<bool>("confirm", $"Tens a certeza que queres remover a categoria {item.Name}?"))
                return;
            var response = await CategoryRepo.Delete(item.Id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = $"Categoria {item.Name} foi removida!";
                await GetCategories();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Message = $"Erro ao remover Categoria {item.Name}: ({response.StatusCode}) {content}";
            }
        }

        private async Task SelectedPage(int page)
        {
            _pagingParameters.PageNumber = page;
            await GetCategories();
        }

        private async Task GetCategories()
        {
            var pagingResponse = await CategoryRepo.GetCategories(_pagingParameters);
            CategoryList = pagingResponse.Items;
            MetaData = pagingResponse.MetaData;
        }
    }
}