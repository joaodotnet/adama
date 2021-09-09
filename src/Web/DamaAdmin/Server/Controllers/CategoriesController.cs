using ApplicationCore;
using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DamaAdmin.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        // The Web API will only accept tokens 1) for users, and 2) having the "API.Access" scope for this API
        private static readonly string[] _scopeRequiredByApi = new string[] { "API.Access" };
        private readonly IMapper _mapper;
        private readonly IRepository<Category> _categoryRepository;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IMapper mapper, IRepository<Category> categoryRepository, ILogger<CategoriesController> logger)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }
        [HttpGet]
        public async Task<PaginatedList<CategoryDTO>> Get(int? pageIndex)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(_scopeRequiredByApi);

            var total = await _categoryRepository.CountAsync(new CategorySpecification(new CategoryFilter { IncludeCatalogTypes = true }));
            var cats = await _categoryRepository.ListAsync(new CategorySpecification(new CategoryFilter{ IncludeCatalogTypes = true }, pageIndex ?? 1, 2));

            var model = _mapper.Map<IEnumerable<CategoryDTO>>(cats);
            return new PaginatedList<CategoryDTO>(model, total, pageIndex ?? 1, 2);
        }

        [HttpGet("all")]
        public async Task<List<CategoryDTO>> GetAll()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(_scopeRequiredByApi);

            var categories = await _categoryRepository.ListAsync();

            return _mapper.Map<List<CategoryDTO>>(categories);
        }


        [HttpPost]
        public async Task<IActionResult> Post(CategoryDTO model)
        {
            await _categoryRepository.AddAsync(_mapper.Map<ApplicationCore.Entities.Category>(model));
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put(CategoryDTO model)
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

    // [Authorize]
    // [ApiController]
    // [Route("[controller]")]
    // public class WeatherForecastController : ControllerBase
    // {
    //     private static readonly string[] Summaries = new[]
    //     {
    //         "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    //     };

    //     private readonly ILogger<WeatherForecastController> _logger;

    //     // The Web API will only accept tokens 1) for users, and 2) having the "API.Access" scope for this API
    //     static readonly string[] scopeRequiredByApi = new string[] { "API.Access" };

    //     public WeatherForecastController(ILogger<WeatherForecastController> logger)
    //     {
    //         _logger = logger;
    //     }

    //     [HttpGet]
    //     public IEnumerable<WeatherForecast> Get()
    //     {
    //         HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

    //         var rng = new Random();
    //         return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //         {
    //             Date = DateTime.Now.AddDays(index),
    //             TemperatureC = rng.Next(-20, 55),
    //             Summary = Summaries[rng.Next(Summaries.Length)]
    //         })
    //         .ToArray();
    //     }
    // }
}
