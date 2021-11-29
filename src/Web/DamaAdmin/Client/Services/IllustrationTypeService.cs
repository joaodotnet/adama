using System.Net.Http;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Shared.Models;

namespace DamaAdmin.Client.Services
{
    public class IllustrationTypeService : HttpService<IllustrationTypeViewModel>, IHttpService<IllustrationTypeViewModel>
    {
        public IllustrationTypeService(HttpClient client) : base(client, "illustrationtypes")
        {
        }
    }
}
