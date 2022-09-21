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
    public class ProductsController : DamaAdminBase<CatalogItem>
    {
        private readonly IRepository<CatalogItem> _repository;
        private readonly IMapper _mapper;

        public ProductsController(IRepository<CatalogItem> repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
            if(product == null)
                return NotFound();

            product.UpdateFlags(request.CheckboxType,request.Value);
            await _repository.UpdateAsync(product);

            return new JsonResult("OK");
        }
    }

    public class UpdateFlagRequest
    {
        public int CheckboxType { get; set; }
        public bool Value { get; set; }
    }
}
