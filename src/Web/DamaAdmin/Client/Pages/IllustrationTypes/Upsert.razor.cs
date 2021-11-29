using System.Threading.Tasks;
using DamaAdmin.Client.Services;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace DamaAdmin.Client.Pages.IllustrationTypes
{
    [Authorize]
    public partial class Upsert : ComponentBase
    {
        [Parameter]
        public int? Id { get; set; }

        private bool _isSubmitting;
        private IllustrationTypeViewModel model = new();
        private string statusMessage;

        [Inject]
        public IllustrationTypeService IllustrationTypeService { get; set; }
        [Inject]
        public NavigationManager NavManager { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }

        public bool IsNew => !Id.HasValue;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (Id.HasValue)
                {
                    model = await IllustrationTypeService.GetById(Id.Value);
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        private async Task HandleValidSubmit()
        {
            _isSubmitting = true;
            try
            {
                if (await IllustrationTypeService.CheckIfCodeExists(model.Code, model.Id))
                {
                    statusMessage = $"Erro: O nome do tipo de produto '{model.Code}' já existe!";
                    return;
                }

                await IllustrationTypeService.Upsert(model);
                var message = $"Tipo de illustração {model.Name} atualizado com sucesso!";
                NavManager.NavigateTo($"/tipos-de-ilustracoes/{message}");
            }
            finally
            {
                _isSubmitting = false;
            }
        }
    }
}
