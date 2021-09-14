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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DamaAdmin.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
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

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var catinDb = await _categoryRepository.GetByIdAsync(id);

            if (catinDb != null)
            {
                await _categoryRepository.DeleteAsync(catinDb);
                return Ok();
            }

            return NotFound();
        }
    }
}
