using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.WebUtilities;

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

        public async Task<bool> CheckIfSlugExists(string slug, int? id)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["slug"] = slug
            };

            if (id.HasValue)
                queryStringParam.Add("id", id.ToString());

            var response = await client.GetAsync(QueryHelpers.AddQueryString($"{endpointName}/slug/exists", queryStringParam));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            return bool.Parse(content);
        }

        public async Task<string> GetNewSku(int typeId, int illustrationId, int? attributeId = null)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["typeId"] = typeId.ToString(),
                ["illustrationId"]  = illustrationId.ToString()                
            };
            if(attributeId.HasValue)
                queryStringParam.Add("attributeId", attributeId.Value.ToString());

            var response = await client.GetAsync(QueryHelpers.AddQueryString($"{endpointName}/sku/new", queryStringParam));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            return content;
        }
    }
}
