using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Client.Services;

namespace DamaAdmin.Client.Pages.Categories
{
    [Authorize]
    public partial class Edit : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        private CategoryViewModel categoryModel = null;
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
                categoryModel = allCategories.SingleOrDefault(x => x.Id == Id);
                if (categoryModel == null)
                {
                    var message = "Erro: Categoria não encontrada!";
                    NavManager.NavigateTo($"/categorias/{message}");
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        private void NameChangeEvent()
        {
            categoryModel.Slug = ApplicationCore.Utils.URLFriendly(categoryModel.Name);
        }

        private async Task HandleValidSubmit()
        {
            if (allCategories.Any(x => x.Name == categoryModel.Name && x.Id != Id))
            {
                statusMessage = $"Erro: O nome da Categoria '{categoryModel.Name}' já existe!";
                return;
            }

            if (allCategories.Any(x => x.Slug == categoryModel.Slug && x.Id != Id))
            {
                statusMessage = "Erro: Já existe um slug com o mesmo nome!";
                return;
            }

            statusMessage = null;
            await CategoryService.Update(categoryModel);
            var message = $"Categoria {categoryModel.Name} atualizada com sucesso!";
            NavManager.NavigateTo($"/categorias/{message}");
        }
    }
}