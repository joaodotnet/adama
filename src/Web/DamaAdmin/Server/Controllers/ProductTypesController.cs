using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Entities;
using ApplicationCore.Helpers;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using DamaAdmin.Client.Pages.Categories;
using DamaAdmin.Shared.Features;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web.Resource;

namespace DamaAdmin.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductTypesController : DamaAdminBase<CatalogType>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<CatalogType> _catalogTypesRepository;
        private readonly ILogger<ProductTypesController> _logger;
        private readonly BackofficeSettings _backofficeSettings;

        public ProductTypesController(IMapper mapper,
                                      IRepository<CatalogType> catalogTypesRepository,
                                      ILogger<ProductTypesController> logger,
                                      IOptionsSnapshot<BackofficeSettings> backofficeSettings
                                      )
        : base(catalogTypesRepository, mapper)
        {
            _mapper = mapper;
            _catalogTypesRepository = catalogTypesRepository;
            _logger = logger;
            _backofficeSettings = backofficeSettings.Value;
        }

        [HttpGet]
        public async Task<PagedList<ProductTypeViewModel>> Get([FromQuery] PagingParameters parameters)
        {
            return await GetWithPaging<ProductTypeViewModel>(parameters,
                new CatalogTypeSpecification(new CatalogTypeFilter()),
                new CatalogTypeSpecification(
                    new CatalogTypeFilter
                    {
                        IncludeCategories = true,
                        PageIndex = parameters.PageNumber,
                        PageSize = parameters.PageSize
                    }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductTypeViewModel>> GetById(int id)
        {
            var productType = await _catalogTypesRepository.SingleOrDefaultAsync(
                new CatalogTypeSpecification(id));

            if (productType == null)
            {
                return NotFound();
            }
            var vm = _mapper.Map<ProductTypeViewModel>(productType);
            if (productType.Categories?.Count > 0)
            {
                vm.CategoriesId.AddRange(productType.Categories.Select(x => x.CategoryId.ToString()));
            }
            return vm;
        }

        [HttpGet("code/exists")]
        public async Task<bool> CheckIfCodeExists([FromQuery] string code, [FromQuery] int? id)
        {
            return (await _catalogTypesRepository.CountAsync(new CatalogTypeSpecification(new CatalogTypeFilter { Code = code.ToUpper(), NotProductTypeId = id }))) > 0;
        }

        [HttpGet("slug/exists")]
        public async Task<bool> CheckIfSlugExists([FromQuery] string slug, [FromQuery] int? id)
        {
            return (await _catalogTypesRepository.CountAsync(new CatalogTypeSpecification(new CatalogTypeFilter { Slug = slug.ToUpper(), NotProductTypeId = id }))) > 0;
        }

        [HttpGet("all")]
        public async Task<List<ProductTypeViewModel>> GetAll()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(_scopeRequiredByApi);
            var all = await _catalogTypesRepository.ListAsync(); //TODO: Create spec to return only data needed
            return _mapper.Map<List<ProductTypeViewModel>>(all);           
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductTypeViewModel model)
        {
            if (model.Id == 0)
            {
                FileDetailViewModel[] pictureTextHelpers = new FileDetailViewModel[model.PictureTextHelpers.Count];
                model.PictureTextHelpers.CopyTo(pictureTextHelpers, 0);
                model.PictureTextHelpers = null;
                var entity = _mapper.Map<CatalogType>(model);

                model.CategoriesId.ForEach(x => entity.AddCategory(new CatalogTypeCategory(int.Parse(x))));
                SaveTextHelpers(pictureTextHelpers, entity);

                await _catalogTypesRepository.AddAsync(entity);
            }
            else
            {
                var productTypeEntity = await _catalogTypesRepository.SingleOrDefaultAsync(
                    new CatalogTypeSpecification(
                        new CatalogTypeFilter
                        {
                            ProductTypeId = model.Id,
                            IncludeCategories = true,
                            IncludeHelpers = true
                        }));

                if (productTypeEntity == null)
                    return NotFound();

                if (!string.IsNullOrEmpty(model.PictureUri) && model.PictureUri != productTypeEntity.PictureUri) //Verificar se vem vazio se nao alterar
                {
                    //Delete picture
                    ImageHelper.DeleteFile(_backofficeSettings.WebProductTypesPictureV2FullPath, Utils.GetFileName(productTypeEntity.PictureUri));
                    productTypeEntity.UpdatePicture(model.PictureUri);
                }

                SaveTextHelpers(model.PictureTextHelpers.ToArray(), productTypeEntity, true);

                productTypeEntity.Update(
                    model.Code,
                    model.Name,
                    model.DeliveryTimeMin,
                    model.DeliveryTimeMax,
                    model.DeliveryTimeUnit,
                    model.Price,
                    model.AdditionalTextPrice,
                    model.Weight,
                    model.MetaDescription,
                    model.Title,
                    model.Slug);

                if (model.CategoriesId.Count > 0) //Verificar se vem vazio se nao alterar
                {
                    //Remove
                    var to_remove = productTypeEntity.Categories.Where(c => !model.CategoriesId.Any(c2 => int.Parse(c2) == c.CategoryId));
                    foreach (var item in to_remove)
                    {
                        productTypeEntity.RemoveCategory(item);
                    }
                    //Add
                    var to_add = model.CategoriesId.Where(c => !productTypeEntity.Categories.Any(c2 => c2.CategoryId == int.Parse(c)));
                    foreach (var item in to_add)
                    {
                        productTypeEntity.AddCategory(new CatalogTypeCategory(int.Parse(item)));
                    }
                }
                await _catalogTypesRepository.UpdateAsync(productTypeEntity);
            }

            return Ok();
        }

        [HttpDelete]
        public override async Task<IActionResult> Delete(int id)
        {
            var productTypeEntity = await _catalogTypesRepository.SingleOrDefaultAsync(
                    new CatalogTypeSpecification(
                        new CatalogTypeFilter
                        {
                            ProductTypeId = id,
                            IncludeCategories = true,
                            IncludeHelpers = true
                        }));

            if (productTypeEntity != null)
            {
                if(!string.IsNullOrEmpty(productTypeEntity.PictureUri))
                {
                    ImageHelper.DeleteFile(_backofficeSettings.WebProductTypesPictureV2FullPath, Utils.GetFileName(productTypeEntity.PictureUri));
                }
                foreach (var item in productTypeEntity.PictureTextHelpers)
                {
                    ImageHelper.DeleteFile(item.Location);
                }
                productTypeEntity.ClearCategories();
                productTypeEntity.ClearTextHelpers();
                await _catalogTypesRepository.UpdateAsync(productTypeEntity);
                await _catalogTypesRepository.DeleteAsync(productTypeEntity);
                return Ok();
            }

            return NotFound();
        }

        private void SaveTextHelpers(FileDetailViewModel[] pictureTextHelpers, CatalogType entity, bool deleteAll = false)
        {
            if (pictureTextHelpers.Length > 0)
            {
                if (deleteAll)
                {
                    foreach (var item in entity.PictureTextHelpers)
                    {
                        ImageHelper.DeleteFile(item.Location);
                    }
                    entity.ClearTextHelpers();
                }
                foreach (var item in pictureTextHelpers)
                {
                    entity.AddPictureTextHelper(_mapper.Map<FileDetail>(item));
                }

            }
        }
    }
}
