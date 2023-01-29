using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using DamaAdmin.Client.Services;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DamaAdmin.Client.Pages.Products
{
    public partial class Upsert
    {
        private ProductViewModel model = new();
        private IEnumerable<ProductTypeViewModel> allProductTypes = new List<ProductTypeViewModel>();
        private IEnumerable<IllustrationViewModel> allIllustrations = new List<IllustrationViewModel>();        
        private bool isSubmitting;
        private string statusMessage;
        long maxFileSize = 1024 * 2000;
        private int maxAllowedFiles = 10;

        [Parameter]
        public int? Id { get; set; }
        [Parameter]
        public string Message { get; set; }

        public bool IsNew => !Id.HasValue;

        [Inject]
        public ProductService ProductService { get; set; }
        [Inject]
        public ProductTypeService ProductTypeService { get; set; }
        [Inject]
        public IllustrationService IllustrationService { get; set; }
        [Inject]
        public CategoryService CategoryService{ get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public NavigationManager NavManager { get; set; }
        [Inject]
        public ILogger<Upsert> Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                allProductTypes = await ProductTypeService.ListAll();
                allIllustrations = await IllustrationService.ListAll();
                var allCategories = await CategoryService.GetCatalogCategories(0);
                if (Id.HasValue)
                {
                    model = await ProductService.GetById(Id.Value);
                }
                else
                    model.Categories = await CategoryService.GetCatalogCategories(allProductTypes.First().Id);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task HandleValidSubmit()
        {
            isSubmitting = true;
            try
            {
                if (await ProductService.CheckIfSlugExists(model.Slug, model.Id))
                {
                    statusMessage = $"Erro: O slug {model.Slug} já existe!";
                    return;
                }

                model.Sku = await ProductService.GetNewSku(model.CatalogTypeId, model.CatalogIllustrationId);
                model.Price = model.Price == 0 ? default : model.Price;

                //await ProductService.Upsert(model);
                var message = $"Produto {model.Name} atualizado com sucesso!";
                NavManager.NavigateTo($"/produtos/{message}");
            }
            finally
            {
                isSubmitting = false;
            }
        }

        private void NameChangeEvent()
        {
            model.Slug = ApplicationCore.Utils.URLFriendly(model.Name);
        }

        private void ProductTypeChangeEvent()
        {
            model.ProductTypePrice = allProductTypes.SingleOrDefault(x => x.Id == model.CatalogTypeId)?.Price;
        }

        private async Task OnImageSelection(InputFileChangeEventArgs e)
        {
            
            var upload = false;

            using var content = new MultipartFormDataContent();

            IBrowserFile file = e.File;
            if (model.PicturesToUpload.SingleOrDefault(
                f => f.FileName == file.Name) is null)
            {
                try
                {
                    var fileContent =
                        new StreamContent(file.OpenReadStream(maxFileSize));
                    fileContent.Headers.ContentType =
                        new MediaTypeHeaderValue(file.ContentType);
                    
                    content.Add(
                        content: fileContent,
                        name: "\"files\"",
                        fileName: file.Name);

                    upload = true;
                }
                catch (Exception ex)
                {
                    Logger.LogInformation(
                        "{FileName} not uploaded (Err: 6): {Message}",
                    file.Name, ex.Message);

                    model.PicturesToUpload.Add(
                        new()
                        {
                            FileName = file.Name,
                            ErrorCode = 6,
                            Uploaded = false,
                            IsPrincipal = true
                        });
                }
            }

            if (upload)
            {
                var newuploadResults = await ProductService.FileSaveAsync(content);

                model.PicturesToUpload = model.PicturesToUpload.Concat(newuploadResults).ToList();
            }
        }

        private async Task OnMoreImagesSelection(InputFileChangeEventArgs e)
        {
            var upload = false;

            using var content = new MultipartFormDataContent();

            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                if (model.PicturesToUpload.SingleOrDefault(
                    f => f.FileName == file.Name) is null)
                {
                    try
                    {
                        var fileContent =
                            new StreamContent(file.OpenReadStream(maxFileSize));
                        fileContent.Headers.ContentType =
                            new MediaTypeHeaderValue(file.ContentType);

                        content.Add(
                            content: fileContent,
                            name: "\"files\"",
                            fileName: file.Name);

                        upload = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogInformation(
                            "{FileName} not uploaded (Err: 6): {Message}",
                        file.Name, ex.Message);

                        model.PicturesToUpload.Add(
                            new()
                            {
                                FileName = file.Name,
                                ErrorCode = 6,
                                Uploaded = false
                            });
                    }
                }
            }

            if (upload)
            {
                var newuploadResults = await ProductService.FileSaveAsync(content);

                model.PicturesToUpload = model.PicturesToUpload.Concat(newuploadResults).ToList();
            }
        }

        private void HandleChildClick(CatalogCategoryViewModel parent, CatalogCategoryViewModel child)
        {
            //when call onclick the state is not yet change
            if(!child.Selected)
                parent.Selected = true;
        }
    }
}
