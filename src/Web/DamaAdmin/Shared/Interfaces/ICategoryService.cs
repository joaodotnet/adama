using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DamaAdmin.Shared.Features;
using DamaAdmin.Shared.Models;

namespace DamaAdmin.Shared.Interfaces
{
    public interface IHttpService<T> where T: class
    {
        Task<PagingResponse<T>> List(PagingParameters parameters);
        Task<IEnumerable<T>> ListAll();
        Task<HttpResponseMessage> Delete(int categoryId);
        Task Upsert(T categoryModel);
        Task Update(T categoryModel);
    }
}