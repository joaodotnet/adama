using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using DamaAdmin.Shared.Features;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace DamaAdmin.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductTypesController : DamaAdminBase<CatalogType>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<CatalogType> _catalogTypesRepository;
        private readonly ILogger<ProductTypesController> _logger;

        public ProductTypesController(IMapper mapper, IRepository<CatalogType> catalogTypesRepository, ILogger<ProductTypesController> logger)
        :base (catalogTypesRepository, mapper) 
        {
            _mapper = mapper;
            _catalogTypesRepository = catalogTypesRepository;
            _logger = logger;
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

        [HttpGet("code/exists")]
        public async Task<bool> CheckIfCodeExists([FromQuery] string code)
        {
            return (await _catalogTypesRepository.CountAsync(new CatalogTypeSpecification(new CatalogTypeFilter { Code = code.ToUpper() }))) > 0;
        }

        [HttpGet("slug/exists")]
        public async Task<bool> CheckIfSlugExists([FromQuery] string slug)
        {
            return (await _catalogTypesRepository.CountAsync(new CatalogTypeSpecification(new CatalogTypeFilter { Slug = slug.ToUpper() }))) > 0;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductTypeViewModel model)
        {
            FileDetailViewModel[] pictureTextHelpers = Array.Empty<FileDetailViewModel>();
            model.PictureTextHelpers.CopyTo(pictureTextHelpers, 0);
            model.PictureTextHelpers = null;
            var entity = _mapper.Map<CatalogType>(model);
            
            model.CategoriesId.ForEach(x => entity.AddCategory(new CatalogTypeCategory(int.Parse(x))));
            if(pictureTextHelpers.Length > 0)
            {
                foreach (var item in pictureTextHelpers)
                {
                    entity.PictureTextHelpers.Add(_mapper.Map<FileDetail>(item));
                }
            }

            await _catalogTypesRepository.AddAsync(entity);
            return Ok();
        }
    }
}
