using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DamaAdmin.Shared.Features;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace DamaAdmin.Client.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;

        public CategoryService(HttpClient client)
        {
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<HttpResponseMessage> Delete(int categoryId)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["id"] = categoryId.ToString()
            };
            return await _client.DeleteAsync(QueryHelpers.AddQueryString("categories", queryStringParam));
        }


        public async Task<PagingResponse<CategoryViewModel>> List(PagingParameters parameters)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = parameters.PageNumber.ToString()
            };
            var response = await _client.GetAsync(QueryHelpers.AddQueryString("categories", queryStringParam));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var pagingResponse = new PagingResponse<CategoryViewModel>
            {
                Items = JsonSerializer.Deserialize<List<CategoryViewModel>>(content, _options),
                MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
            };

            return pagingResponse;
        }

        public async Task<IEnumerable<CategoryViewModel>> ListAll()
        {
            return await _client.GetFromJsonAsync<IEnumerable<CategoryViewModel>>("categories/all", _options);
        }

        public async Task Create(CategoryViewModel categoryModel)
        {
            var response = await _client.PostAsJsonAsync("categories", categoryModel, _options);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            } 
        }

        public async Task Update(CategoryViewModel categoryModel)
        {
            var response = await _client.PutAsJsonAsync("categories", categoryModel, _options);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            }
        }
    }
}
