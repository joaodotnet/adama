using System.Net.Http;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Shared.Models;

namespace DamaAdmin.Client.Services
{
    public class ProductTypeService : HttpService<ProductTypeViewModel>, IHttpService<ProductTypeViewModel>
    {
        public ProductTypeService(HttpClient client): base(client, "producttypes")
        {
        }
    }
}
