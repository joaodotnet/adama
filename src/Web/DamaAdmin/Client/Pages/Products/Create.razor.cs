using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.DTOs;
using DamaAdmin.Client.Services;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace DamaAdmin.Client.Pages.Products
{
    public partial class Create
    {
        private ProductViewModel model = new();
        private IEnumerable<ProductTypeViewModel> allProductTypes = new List<ProductTypeViewModel>();
        private IEnumerable<IllustrationViewModel> allIllustrations = new List<IllustrationViewModel>();
        private List<CatalogCategoryViewModel> catalogCategoryModel = new();
        private bool isSubmitting;
        private string statusMessage;

        [Parameter]
        public int? Id { get; set; }

        public bool IsNew => !Id.HasValue;

        [Inject]
        public ProductTypeService ProductTypeService { get; set; }
        [Inject]
        public IllustrationService IllustrationService { get; set; }
        [Inject]
        public CategoryService CategoryService{ get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                //if (Id.HasValue)
                //{
                //    model = await ProductTypeService.GetById(Id.Value);
                //}
                allProductTypes = await ProductTypeService.ListAll();
                allIllustrations = await IllustrationService.ListAll();
                catalogCategoryModel = await CategoryService.GetCatalogCategories(allProductTypes.First().Id);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
        private async Task HandleValidSubmit()
        {
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
            IBrowserFile imgFile = e.File;
            var buffers = new byte[imgFile.Size];
            await imgFile.OpenReadStream().ReadAsync(buffers);
            model.Picture = new ApplicationCore.DTOs.FileData{Data = buffers, FileType = imgFile.ContentType, Size = imgFile.Size, FileName = imgFile.Name};
        }

        private async Task OnMoreImagesSelection(InputFileChangeEventArgs e)
        {
            model.OtherPicturesFormFiles.Clear();
            foreach (IBrowserFile imgFile in e.GetMultipleFiles())
            {
                var buffers = new byte[imgFile.Size];
                await imgFile.OpenReadStream().ReadAsync(buffers);
                model.OtherPicturesFormFiles.Add(new FileData
                {
                    Data = buffers,
                    FileType = imgFile.ContentType,
                    Size = imgFile.Size,
                    FileName = imgFile.Name
                });
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
