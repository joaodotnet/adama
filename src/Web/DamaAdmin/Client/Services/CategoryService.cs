using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace DamaAdmin.Client.Services
{
    public class CategoryService : HttpService<CategoryViewModel>, IHttpService<CategoryViewModel>
    {
        public CategoryService(HttpClient client): base(client, "categories")
        {
        }

        public async Task<List<CatalogCategoryViewModel>> GetCatalogCategories(int catalogTypeId)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["catalogTypeId"] = catalogTypeId.ToString()
            };
            var response = await client.GetAsync(QueryHelpers.AddQueryString($"{endpointName}/catalog", queryStringParam));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var result = JsonSerializer.Deserialize<List<CatalogCategoryViewModel>>(content, SerlializerOptions);
            return result;
        }
    }
}
