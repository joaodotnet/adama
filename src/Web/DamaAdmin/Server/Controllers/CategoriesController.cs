using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using DamaAdmin.Client.Services;
using DamaAdmin.Shared.Features;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DamaAdmin.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : DamaAdminBase<Category>
    {        
        private readonly IMapper _mapper;
        private readonly IRepository<Category> _categoryRepository;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IMapper mapper, IRepository<Category> categoryRepository, ILogger<CategoriesController> logger)
        : base(categoryRepository, mapper)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }
        [HttpGet]
        public async Task<PagedList<CategoryViewModel>> Get([FromQuery] PagingParameters parameters)
        {
            return await GetWithPaging<CategoryViewModel>(
                parameters,
                new CategorySpecification(new CategoryFilter()),
                new CategorySpecification(new CategoryFilter { IncludeParent = true }, parameters.PageNumber, parameters.PageSize));
        }

        [HttpGet("all")]
        public async Task<List<CategoryViewModel>> GetAll()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(_scopeRequiredByApi);

            var categories = await _categoryRepository.ListAsync();

            return _mapper.Map<List<CategoryViewModel>>(categories);
        }

        [HttpGet("catalog")]
        public async Task<List<CatalogCategoryViewModel>> GetCatalogCategories(int catalogTypeId)
        {
            var allCats = await _categoryRepository.ListAsync(
                    new CategorySpecification(new CategoryFilter { IncludeParent = true, IncludeCatalogTypes = true }));
         
            //var catsId = allCats
            //    .Where(x => x.CatalogTypes.Any(ct => ct.CatalogTypeId == catalogTypeId))
            //    .Select(x => x.Id)
            //    .ToList();

            List<CatalogCategoryViewModel> catalogCategories = new List<CatalogCategoryViewModel>();
            foreach (var item in allCats.Where(x => x.Parent == null).ToList())
            {
                CatalogCategoryViewModel parent = new CatalogCategoryViewModel
                {
                    CategoryId = item.Id,
                    Label = item.Name,
                    Childs = new List<CatalogCategoryViewModel>(),
                    //Selected = catsId.Contains(item.Id)

                };
                parent.Childs.AddRange(allCats.Where(x => x.ParentId == item.Id).Select(s => new CatalogCategoryViewModel
                {
                    CategoryId = s.Id,
                    Label = s.Name,
                    //Selected = catsId.Contains(s.Id)
                }));
                catalogCategories.Add(parent);
            }
            return catalogCategories;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CategoryViewModel model)
        {
            await _categoryRepository.AddAsync(_mapper.Map<ApplicationCore.Entities.Category>(model));
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put(CategoryViewModel model)
        {
            var cat = await _categoryRepository.GetBySpecAsync(new CategorySpecification(new CategoryFilter { Id = model.Id, IncludeParent = true }));
            if (cat == null)
                return NotFound("Categoria inexistente!");
            _mapper.Map(model, cat);

            await _categoryRepository.UpdateAsync(cat);
            return Ok();
        }
    }
}
