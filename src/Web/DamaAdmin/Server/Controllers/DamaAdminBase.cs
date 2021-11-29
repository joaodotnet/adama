using ApplicationCore.Interfaces;
using Ardalis.Specification;
using AutoMapper;
using DamaAdmin.Shared.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace DamaAdmin.Server.Controllers
{
    public abstract class DamaAdminBase<T> : ControllerBase
    where T: class, IAggregateRoot
    {        
        // The Web API will only accept tokens 1) for users, and 2) having the "API.Access" scope for this API
        protected static readonly string[] _scopeRequiredByApi = new string[] { "API.Access" };

        private readonly IRepository<T> _repository;
        private readonly IMapper _mapper;

        public DamaAdminBase(IRepository<T> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        protected async Task<PagedList<TOutput>> GetWithPaging<TOutput>(PagingParameters parameters, ISpecification<T> countSpecification, ISpecification<T> listSpecification)
        where TOutput: class
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(_scopeRequiredByApi);

            var total = await _repository.CountAsync(countSpecification);
            
            var list = await _repository.ListAsync(listSpecification);

            var model = _mapper.Map<IEnumerable<TOutput>>(list);

            var pagedlist = new PagedList<TOutput>(model, total, parameters.PageNumber , parameters.PageSize);
 
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedlist.MetaData));

            return pagedlist;
        }

        [HttpDelete]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
                return Ok();
            }

            return NotFound();
        }
    }
}
