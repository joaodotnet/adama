using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using DamaAdmin.Client.Pages.Categories;
using DamaAdmin.Shared.Features;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace DamaAdmin.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IllustrationsController : DamaAdminBase<CatalogIllustration>
    {
        private readonly IRepository<CatalogIllustration> _repository;
        private readonly IMapper _mapper;

        public IllustrationsController(IRepository<CatalogIllustration> repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<PagedList<IllustrationViewModel>> Get([FromQuery] PagingParameters parameters)
        {
            return await GetWithPaging<IllustrationViewModel>(parameters,
                new CatalogIllustrationSpecification(new()),
                new CatalogIllustrationSpecification(
                    new IllustrationFilter
                    {
                        PageIndex = parameters.PageNumber,
                        PageSize = parameters.PageSize
                    }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IllustrationViewModel>> GetById(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }
            return _mapper.Map<IllustrationViewModel>(entity);
        }

        [HttpGet("code/exists")]
        public async Task<bool> CheckIfCodeExists([FromQuery] string code, [FromQuery] int? id)
        {
            return (await _repository.CountAsync(new CatalogIllustrationSpecification(new IllustrationFilter { Code = code.ToUpper(), NotId = id }))) > 0;
        }

        [HttpGet("all")]
        public async Task<List<IllustrationViewModel>> GetAll()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(_scopeRequiredByApi);
            var all = await _repository.ListAsync(new CatalogIllustrationSpecification(new())); //TODO: create a spec to return only data needed
            return _mapper.Map<List<IllustrationViewModel>>(all);
        }

        [HttpPost]
        public async Task<IActionResult> Post(IllustrationViewModel model)
        {
            var entity = _mapper.Map<CatalogIllustration>(model);
            if (model.Id == 0)
            {
                await _repository.AddAsync(entity);
            }
            else
            {
                await _repository.UpdateAsync(entity);
            }

            return Ok();
        }
    }
}
