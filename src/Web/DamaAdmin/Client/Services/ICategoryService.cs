using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ApplicationCore.DTOs;
using DamaAdmin.Client.Features;

namespace DamaAdmin.Client.Services
{
    public interface ICategoryService
    {
        Task<PagingResponse<CategoryDTO>> List(PagingParameters parameters);
        Task<IEnumerable<CategoryDTO>> ListAll();
        Task<HttpResponseMessage> Delete(int categoryId);
        Task Update(CategoryDTO categoryModel);
    }
}