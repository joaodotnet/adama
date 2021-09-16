using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Shared.Models;
using DamaAdmin.Client.Services;
using Microsoft.JSInterop;

namespace DamaAdmin.Client.Pages.ProductTypes
{
    [Authorize]
    public partial class Create : ComponentBase
    {
        private ProductTypeViewModel model = new();
        private string statusMessage;
        private IEnumerable<CategoryViewModel> allCategories = new List<CategoryViewModel>();

        [Inject]
        public ProductTypeService ProductTypeService { get; set; }
        [Inject]
        public CategoryService CategoryService { get; set; }
        [Inject]
        public NavigationManager NavManager { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                allCategories = await CategoryService.ListAll();
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
        
        private async Task HandleValidSubmit()
        {
            // if (allCategories.Any(x => x.Name == categoryModel.Name))
            // {
            //     statusMessage = $"Erro: O nome da Categoria '{categoryModel.Name}' já existe!";
            //     return;
            // }

            // if (allCategories.Any(x => x.Slug == categoryModel.Slug))
            // {
            //     statusMessage = "Erro: Já existe um slug com o mesmo nome!";
            //     return;
            // }

            // statusMessage = null;
            // await CategoryService.Create(categoryModel);
            // var message = $"Categoria {categoryModel.Name} criada com sucesso!";
            // NavManager.NavigateTo($"/categorias/{message}");
        }

        private void NameChangeEvent()
        {
            model.Slug = ApplicationCore.Utils.URLFriendly(model.Name);
        }

        private async Task OnChangeCallback(ChangeEventArgs eventArgs)
        {
            var selection = await GetAllSelections(_elementReference);
            model.CategoriesId = selection;
        }

        private ElementReference _elementReference;

        public async Task<List<string>> GetAllSelections(ElementReference elementReference)
        {
            return  (await JSRuntime.InvokeAsync<List<string>>("getSelectedValues", elementReference)).ToList();
        }
    }
}