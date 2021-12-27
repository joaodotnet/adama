using System.Net.Http;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Shared.Models;

namespace DamaAdmin.Client.Services
{
    public class IllustrationService : HttpService<IllustrationViewModel>, IHttpService<IllustrationViewModel>
    {
        public IllustrationService(HttpClient client) : base(client, "illustrations")
        {
        }
    }
}
