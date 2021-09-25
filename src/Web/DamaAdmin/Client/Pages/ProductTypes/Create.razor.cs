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

namespace DamaAdmin.Client.Pages.ProductTypes
{
    [Authorize]
    public partial class Create : ComponentBase
    {
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
            if(await ProductTypeService.CheckIfExists(model.Code))
            {
                statusMessage = $"Erro: O nome do tipo de produto '{model.Code}' já existe!";
                return;
            }

            if(model.CategoriesId == null || model.CategoriesId.Count == 0)
            {
                statusMessage = "Erro: O campo Categorias é obrigatório";
                return;
            }

            if(model.Picture?.Size > 2097152)
            {
                statusMessage = "Erro: O tamanho da imagem principal é muito grande, o máximo é 2MB";
                return;
            }

            if(model.FormFileTextHelpers?.Count > 0 && model.FormFileTextHelpers.Any(x => x.Size > 2097152))
            {
                statusMessage = "Erro: O tamanho das imagens do nome principal são muito grandes, o máximo é 2MB";
                return;
            }

            // ProductTypeModel.Slug = Utils.URLFriendly(ProductTypeModel.Slug);
            // if((await CheckIfSlugExistsAsync(ProductTypeModel.Slug)))
            // {
            //     ModelState.AddModelError("ProductTypeModel.Slug", "Este slug já existe!");
            //     return Page();
            // }

            // //Save Image
            // if (ProductTypeModel?.Picture?.Length > 0)
            // {
            //     var lastId = _context.CatalogTypes.Count() > 0 ? GetLastCatalogTypeId() : 0;
            //     ProductTypeModel.PictureUri = _service.SaveFile(ProductTypeModel.Picture, _backofficeSettings.WebProductTypesPictureV2FullPath, _backofficeSettings.WebProductTypesPictureV2Uri, (++lastId).ToString(), true, 300).PictureUri;
            // }

            // //Save Images Text Helpers
            // if (ProductTypeModel?.FormFileTextHelpers?.Count > 0)
            // {
            //     foreach (var item in ProductTypeModel.FormFileTextHelpers)
            //     {
            //         var lastId = _context.FileDetails.Count() > 0 ? GetLastFileDetailsId() : 0;
            //         var pictureInfo = _service.SaveFile(item, _backofficeSettings.WebProductTypesPictureV2FullPath, _backofficeSettings.WebProductTypesPictureV2Uri, (++lastId).ToString(), true, 150);
            //         ProductTypeModel.PictureTextHelpers.Add(new FileDetailViewModel
            //         {
            //             PictureUri = pictureInfo.PictureUri,
            //             Extension = pictureInfo.Extension,
            //             FileName = pictureInfo.Filename,
            //             Location = pictureInfo.Location
            //         });
            //     }
            // }

            // var catalogType = _mapper.Map<ApplicationCore.Entities.CatalogType>(ProductTypeModel);

            // foreach (var item in ProductTypeModel.CategoriesId)
            // {
            //     catalogType.AddCategory(new CatalogTypeCategory(item));
            // }

            // _context.CatalogTypes.Add(catalogType);
            // await _context.SaveChangesAsync();

            // return RedirectToPage("./Index");
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