using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
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
    }
}
