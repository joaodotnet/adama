using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Helpers;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using DamaAdmin.Client.Pages.Categories;
using DamaAdmin.Shared.Features;
using DamaAdmin.Shared.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DamaAdmin.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : DamaAdminBase<CatalogItem>
    {
        private readonly IRepository<CatalogItem> _repository;
        private readonly IRepository<CatalogType> _catalogTypeRepository;
        private readonly IRepository<CatalogIllustration> _catalogIllustrationRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ProductsController(
            IRepository<CatalogItem> repository,
            IRepository<CatalogType> catalogTypeRepository,
            IRepository<CatalogIllustration> catalogIllustrationRepository,
            IMapper mapper,
            IConfiguration configuration) : base(repository, mapper)
        {
            _repository = repository;
            _catalogTypeRepository = catalogTypeRepository;
            _catalogIllustrationRepository = catalogIllustrationRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<PagedList<ProductViewModel>> Get([FromQuery] PagingParameters parameters)
        {
            return await GetWithPaging<ProductViewModel>(parameters,
                new CatalogItemSpecification(new CatalogItemFiler()),
                new CatalogItemSpecification(
                    new CatalogItemFiler
                    {
                        AddCategories = true,
                        PageIndex = parameters.PageNumber,
                        PageSize = parameters.PageSize
                    }
                ));
        }

        [HttpPut("{id}/updateflag")]
        public async Task<IActionResult> UpdateFlag(int id, [FromBody] UpdateFlagRequest request)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            product.UpdateFlags(request.CheckboxType, request.Value);
            await _repository.UpdateAsync(product);

            return new JsonResult("OK");
        }

        [HttpGet("slug/exists")]
        public async Task<bool> CheckIfSlugExists([FromQuery] string slug, [FromQuery] int? id)
        {
            return (await _repository.CountAsync(new CatalogItemSpecification(new CatalogItemFiler { Slug = slug.ToUpper(), NotProductId = id }))) > 0;
        }

        [HttpGet("sku/new")]
        public async Task<string> GetSku([FromQuery] int typeId, [FromQuery] int illustrationId, [FromQuery] int? attributeId = null)
        {
            var catalogType = await _catalogTypeRepository.GetByIdAsync(typeId);

            var illustration = await _catalogIllustrationRepository.SingleOrDefaultAsync(
                new CatalogIllustrationSpecification(
                    new IllustrationFilter
                    {
                        Id = illustrationId
                    }));

            string sku = $"{catalogType.Code}_{illustration.Code}_{illustration.IllustrationType.Code}";
            if (attributeId.HasValue)
                sku += $"_{attributeId}";
            return sku;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductViewModel model)
        {
            var picturesToAdd = new List<CatalogPicture>();
            //Main Image
            if (model.PicturesToUpload.Count(x => x.Uploaded) > 0)
            {
                foreach (var item in model.PicturesToUpload.Where(x => x.Uploaded))
                {
                    var order = 0;
                    var info = ImageHelper.SaveFile(
                        item,
                        _configuration["WebProductsPictureV2FullPath"],
                        _configuration["WebProductsPictureV2Uri"],
                        Guid.NewGuid().ToString(),
                        true, 700
                    );
                    if(item.IsPrincipal)
                        model.PictureUri = info.PictureUri;

                    picturesToAdd.Add(new CatalogPicture(
                        true,
                        item.IsPrincipal,
                        info.PictureUri,
                        ++order,
                        info.PictureHighUri));
                }
            }

            if (model.Id == 0)
            {
                model.Price = model.Price == 0 ? default : model.Price;
                var entity = _mapper.Map<CatalogItem>(model);
                
                foreach (var item in model.Categories.Where(x => x.Selected).ToList())
                {
                    entity.AddCategory(item.CategoryId);
                    foreach (var child in item.Childs.Where(x => x.Selected).ToList())
                    {
                        entity.AddCategory(child.CategoryId);
                    }
                }

                foreach (var item in picturesToAdd)
                {
                    entity.AddPicture(item);
                }
                await _repository.AddAsync(entity);
                entity.UpdateSku(entity.Sku + "_" + entity.Id);
                await _repository.UpdateAsync(entity);

            }
            else
            {
                throw new NotImplementedException();
            }

            return Ok();
        }
    }

    public class UpdateFlagRequest
    {
        public int CheckboxType { get; set; }
        public bool Value { get; set; }
    }
}
