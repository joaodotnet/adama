using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using DamaAdmin.Shared.Features;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
