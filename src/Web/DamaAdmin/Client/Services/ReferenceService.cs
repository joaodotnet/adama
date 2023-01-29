using System.Net.Http;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Shared.Models;

namespace DamaAdmin.Client.Services
{
    public class ReferenceService : HttpService<CatalogReferenceViewModel>, IHttpService<CatalogReferenceViewModel>
    {
        public ReferenceService(HttpClient client) : base(client, "references")
        {
        }
    }
}
