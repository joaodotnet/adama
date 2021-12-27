using System.Collections.Generic;
using System.Threading.Tasks;
using DamaAdmin.Client.Services;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace DamaAdmin.Client.Pages.Illustrations
{
    [Authorize]
    public partial class Upsert : ComponentBase
    {
        [Parameter]
        public int? Id { get; set; }

        private bool _isSubmitting;
        private IllustrationViewModel model = new();
        private string statusMessage;
        private IEnumerable<IllustrationTypeViewModel> allIllustrationTypes = new List<IllustrationTypeViewModel>();

        [Inject]
        public IllustrationService IllustrationService { get; set; }
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
                allIllustrationTypes = await IllustrationTypeService.ListAll();
                if (Id.HasValue)
                {
                    model = await IllustrationService.GetById(Id.Value);
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
                if (await IllustrationService.CheckIfCodeExists(model.Code, model.Id))
                {
                    statusMessage = $"Erro: O nome da ilustração '{model.Code}' já existe!";
                    return;
                }

                await IllustrationService.Upsert(model);
                var message = $"Illustração {model.Name} atualizado com sucesso!";
                NavManager.NavigateTo($"/ilustracoes/{message}");
            }
            finally
            {
                _isSubmitting = false;
            }
        }
    }
}
