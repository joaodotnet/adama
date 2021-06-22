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
        public async Task<IEnumerable<CategoryViewModel>> Get()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(_scopeRequiredByApi);

            var cats = await _categoryRepository.ListAsync(new CategorySpecification(new CategoryFilter{ IncludeCatalogTypes = true }));

            var model = _mapper.Map<IEnumerable<CategoryViewModel>>(cats);
            return model;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CategoryViewModel model)
        {
            await _categoryRepository.AddAsync(_mapper.Map<ApplicationCore.Entities.Category>(model));
            return Ok();
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
