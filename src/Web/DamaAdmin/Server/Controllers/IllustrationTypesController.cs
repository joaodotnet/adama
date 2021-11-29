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
    public class IllustrationTypesController : DamaAdminBase<IllustrationType>
    {
        public IllustrationTypesController(IRepository<IllustrationType> repository, IMapper mapper) : base(repository, mapper)
        {
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
    }
}
