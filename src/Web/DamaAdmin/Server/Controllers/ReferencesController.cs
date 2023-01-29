using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DamaAdmin.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReferencesController : DamaAdminBase<CatalogItem>
    {
        private readonly IRepository<CatalogItem> _repository;

        public ReferencesController(IRepository<CatalogItem> repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
        }
        [HttpPost]
        public async Task<IActionResult> Post(CatalogReferenceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(model.Id == 0)
            {
                var product = await _repository.GetByIdAsync(model.CatalogItemId);
                product.AddReference(model.LabelDescription, model.ReferenceCatalogItemId);
                await _repository.UpdateAsync(product);

                //Check reference
                var referenceB = await _repository.GetByIdAsync(model.ReferenceCatalogItemId);
                if(referenceB.References.SingleOrDefault(x =>
                    x.ReferenceCatalogItemId == model.CatalogItemId) == null)
                {
                    referenceB.AddReference(model.LabelDescription, model.CatalogItemId);
                    await _repository.UpdateAsync(referenceB);
                }
            }

            return Ok();
        }
    }
}
