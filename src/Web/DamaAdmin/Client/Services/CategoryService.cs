using System.Net.Http;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Shared.Models;

namespace DamaAdmin.Client.Services
{
    public class CategoryService : HttpService<CategoryViewModel>, IHttpService<CategoryViewModel>
    {
        public CategoryService(HttpClient client): base(client, "categories")
        {
        }
    }
}
