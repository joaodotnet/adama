using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DamaAdmin.Shared.Features;
using DamaAdmin.Shared.Models;

namespace DamaAdmin.Shared.Interfaces
{
    public interface ICategoryService
    {
        Task<PagingResponse<CategoryViewModel>> List(PagingParameters parameters);
        Task<IEnumerable<CategoryViewModel>> ListAll();
        Task<HttpResponseMessage> Delete(int categoryId);
        Task Update(CategoryViewModel categoryModel);
    }
}