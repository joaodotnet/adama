using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
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
    public class IllustrationTypesController : DamaAdminBase<IllustrationType>
    {
        private readonly IRepository<IllustrationType> _repository;
        private readonly IMapper _mapper;

        public IllustrationTypesController(IRepository<IllustrationType> repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<PagedList<IllustrationTypeViewModel>> Get([FromQuery] PagingParameters parameters)
        {
            return await GetWithPaging<IllustrationTypeViewModel>(parameters,
                new IllustrationTypeSpecification(new()),
                new IllustrationTypeSpecification(
                    new IllustrationTypeFilter
                    {
                        PageIndex = parameters.PageNumber,
                        PageSize = parameters.PageSize
                    }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IllustrationTypeViewModel>> GetById(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }
            return _mapper.Map<IllustrationTypeViewModel>(entity);
        }

        [HttpGet("code/exists")]
        public async Task<bool> CheckIfCodeExists([FromQuery] string code, [FromQuery] int? id)
        {
            return (await _repository.CountAsync(new IllustrationTypeSpecification(new IllustrationTypeFilter { Code = code.ToUpper(), NotId = id }))) > 0;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IllustrationTypeViewModel model)
        {
            var entity = _mapper.Map<IllustrationType>(model);
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

        [HttpGet("all")]
        public async Task<List<IllustrationTypeViewModel>> GetAll()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(_scopeRequiredByApi);

            var illustrationTypes = await _repository.ListAsync();

            return _mapper.Map<List<IllustrationTypeViewModel>>(illustrationTypes);
        }
    }
}
