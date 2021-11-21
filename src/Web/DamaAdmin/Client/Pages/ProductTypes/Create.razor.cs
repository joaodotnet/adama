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
using Microsoft.AspNetCore.Components.Forms;
using ApplicationCore.Helpers;
using ApplicationCore.DTOs;
using Microsoft.Extensions.Configuration;
using System;

namespace DamaAdmin.Client.Pages.ProductTypes
{
    [Authorize]
    public partial class Create : ComponentBase
    {
        [Parameter]
        public int? Id { get; set; }

        private ElementReference _elementReference;
        private bool _isSubmitting;
        private ProductTypeViewModel model = new();
        private string statusMessage;
        private IEnumerable<CategoryViewModel> allCategories = new List<CategoryViewModel>();
        private List<FileData> fileData = new List<FileData>();

        [Inject]
        public ProductTypeService ProductTypeService { get; set; }
        [Inject]
        public CategoryService CategoryService { get; set; }
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
                allCategories = await CategoryService.ListAll();
                if (Id.HasValue)
                    model = await ProductTypeService.GetById(Id.Value);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        private async Task OnTextHelpersImagesSelection(InputFileChangeEventArgs e)
        {
            model.FormFileTextHelpers.Clear();
            foreach (IBrowserFile imgFile in e.GetMultipleFiles())
            {
                var buffers = new byte[imgFile.Size];
                await imgFile.OpenReadStream().ReadAsync(buffers);
                model.FormFileTextHelpers.Add(new FileData
                {
                    Data = buffers,
                    FileType = imgFile.ContentType,
                    Size = imgFile.Size,
                    FileName = imgFile.Name
                });
            }
        }

        private async Task OnImageSelection(InputFileChangeEventArgs e)
        {
            IBrowserFile imgFile = e.File;
            var buffers = new byte[imgFile.Size];
            await imgFile.OpenReadStream().ReadAsync(buffers);
            model.Picture = new FileData
            {
                Data = buffers,
                FileType = imgFile.ContentType,
                Size = imgFile.Size,
                FileName = imgFile.Name
            };
        }

        private async Task HandleValidSubmit()
        {
            _isSubmitting = true;
            try
            {
                if (await ProductTypeService.CheckIfCodeExists(model.Code))
                {
                    statusMessage = $"Erro: O nome do tipo de produto '{model.Code}' já existe!";
                    return;
                }

                if (model.CategoriesId == null || model.CategoriesId.Count == 0)
                {
                    statusMessage = "Erro: O campo Categorias é obrigatório";
                    return;
                }

                if (model.Picture?.Size > 2097152)
                {
                    statusMessage = "Erro: O tamanho da imagem principal é muito grande, o máximo é 2MB";
                    return;
                }

                if (model.FormFileTextHelpers?.Count > 0 && model.FormFileTextHelpers.Any(x => x.Size > 2097152))
                {
                    statusMessage = "Erro: O tamanho das imagens do nome principal são muito grandes, o máximo é 2MB";
                    return;
                }

                if (await ProductTypeService.CheckIfSlugExists(model.Slug))
                {
                    statusMessage = $"Erro: O slug {model.Slug} já existe!";
                    return;
                }

                //Save Image
                if (model?.Picture?.Size > 0)
                {
                    //var lastId = _context.CatalogTypes.Count() > 0 ? GetLastCatalogTypeId() : 0;
                    model.PictureUri = (ImageHelper.SaveFile(
                        model.Picture,
                        Configuration["WebProductTypesPictureV2FullPath"],
                        Configuration["WebProductTypesPictureV2Uri"],
                        Guid.NewGuid().ToString(),
                         true, 300
                         )).PictureUri;

                    Console.WriteLine("Picture Uri: {0}", model.PictureUri);
                }

                //Save Images Text Helpers
                if (model?.FormFileTextHelpers?.Count > 0)
                {
                    foreach (var item in model.FormFileTextHelpers)
                    {
                        var pictureInfo = ImageHelper.SaveFile(
                            item,
                            Configuration["WebProductTypesPictureV2FullPath"],
                            Configuration["WebProductTypesPictureV2Uri"],
                            Guid.NewGuid().ToString(),
                            true,
                            150);

                        model.PictureTextHelpers.Add(new FileDetailViewModel
                        {
                            PictureUri = pictureInfo.PictureUri,
                            Extension = pictureInfo.Extension,
                            FileName = pictureInfo.Filename,
                            Location = pictureInfo.Location
                        });
                    }
                }

                await ProductTypeService.Create(model);
                var message = $"Tipo de produto {model.Name} criado com sucesso!";
                NavManager.NavigateTo($"/tipos-de-produto/{message}");
            }
            finally
            {
                _isSubmitting = false;
            }
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

        public async Task<List<string>> GetAllSelections(ElementReference elementReference)
        {
            return (await JSRuntime.InvokeAsync<List<string>>("getSelectedValues", elementReference)).ToList();
        }
    }
}
