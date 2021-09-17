using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace DamaAdmin.Client.Services
{
    public class ProductTypeService : HttpService<ProductTypeViewModel>, IHttpService<ProductTypeViewModel>
    {
        public ProductTypeService(HttpClient client): base(client, "producttypes")
        {
        }

        public async Task<bool> CheckIfExists(string code)
        {
             var queryStringParam = new Dictionary<string, string>
            {
                ["code"] = code
            };

            var response = await client.GetAsync(QueryHelpers.AddQueryString($"{endpointName}/exists", queryStringParam));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            return bool.Parse(content);
        }
    }
}
