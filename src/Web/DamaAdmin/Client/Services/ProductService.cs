using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Shared.Models;

namespace DamaAdmin.Client.Services
{
    public class ProductService : HttpService<ProductViewModel>, IHttpService<ProductViewModel>
    {
        public ProductService(HttpClient client) : base(client, "products")
        {
        }

        public async Task UpdateFlag(int id, int checkboxType, bool checkValue)
        {
            var contentItem = new
            {
                CheckboxType = checkboxType,
                Value = checkValue
            };
            var contentJson = JsonSerializer.Serialize(contentItem);
            var response = await client.PutAsync($"{endpointName}/{id}/updateflag", new StringContent(contentJson, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
        }
    }
}
