using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApplicationCore.DTOs;
using DamaAdmin.Client.Features;
using Microsoft.AspNetCore.WebUtilities;

namespace DamaAdmin.Client.HttpRepositories
{
    public interface ICategoryHttpRepository
    {
        Task<PagingResponse<CategoryDTO>> GetCategories(PagingParameters parameters);
        Task<HttpResponseMessage> Delete(int categoryId);
    }
    public class CategoryHttpRepository : ICategoryHttpRepository
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;

        public CategoryHttpRepository(HttpClient client)
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

        public async Task<PagingResponse<CategoryDTO>> GetCategories(PagingParameters parameters)
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
            var pagingResponse = new PagingResponse<CategoryDTO>
            {
                Items = JsonSerializer.Deserialize<List<CategoryDTO>>(content, _options),
                MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
            };

            return pagingResponse;
        }
    }
}