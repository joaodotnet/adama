using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Shared.Models;
using DamaAdmin.Client.Services;

namespace DamaAdmin.Client.Pages.Categories
{
    [Authorize]
    public partial class Create : ComponentBase
    {
        private CategoryViewModel categoryModel = new();
        private IEnumerable<CategoryViewModel> allCategories = new List<CategoryViewModel>();
        private string statusMessage;
        [Inject]
        public CategoryService CategoryService { get; set; }
        [Inject]
        public NavigationManager NavManager { get; set; }

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
            if (allCategories.Any(x => x.Name == categoryModel.Name))
            {
                statusMessage = $"Erro: O nome da Categoria '{categoryModel.Name}' já existe!";
                return;
            }

            if (allCategories.Any(x => x.Slug == categoryModel.Slug))
            {
                statusMessage = "Erro: Já existe um slug com o mesmo nome!";
                return;
            }

            statusMessage = null;
            await CategoryService.Create(categoryModel);
            var message = $"Categoria {categoryModel.Name} criada com sucesso!";
            NavManager.NavigateTo($"/categorias/{message}");
        }

        private void NameChangeEvent()
        {
            categoryModel.Slug = ApplicationCore.Utils.URLFriendly(categoryModel.Name);
        }
    }
}