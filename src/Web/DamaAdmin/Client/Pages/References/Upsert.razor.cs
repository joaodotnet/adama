using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using DamaAdmin.Client.Services;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using Ardalis.GuardClauses;

namespace DamaAdmin.Client.Pages.References
{
    public partial class Upsert : ComponentBase
    {
        [Parameter]
        public int? Id { get; set; }

        [Parameter]
        public int? CatalogItemId { get; set; }

        public bool IsNew => !Id.HasValue;

        private CatalogReferenceViewModel _model = new();
        private bool _isSubmitting;
        private string _statusMessage;
        private IEnumerable<ProductViewModel> _allProducts = Array.Empty<ProductViewModel>();

        [Inject]
        public ProductService ProductService { get; set; }
        [Inject]
        public ReferenceService ReferenceService { get; set; }
        [Inject]
        public NavigationManager NavManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (Id.HasValue)
                {
                    //_model = await ProductTypeService.GetById(Id.Value);
                }
                else
                {
                    Guard.Against.Null(CatalogItemId, nameof(CatalogItemId));
                    var product = await ProductService.GetById(CatalogItemId.Value);
                    Guard.Against.Null(product);
                    _model.CatalogItemId = product.Id;
                    _model.CatalogItemName = product.Name;
                }
                _allProducts = await ProductService.ListAll();
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
                if(!Id.HasValue)
                {
                    await ReferenceService.Upsert(_model);
                    var message = $"Relação {_model.LabelDescription} criada com sucesso!";
                    NavManager.NavigateTo($"/produtos/editar/{_model.CatalogItemId}/{message}");
                }
            }
            finally
            {
                _isSubmitting = false;
            }
        }
    }
}
