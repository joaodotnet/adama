using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DamaAdmin.Shared.Features;
using DamaAdmin.Shared.Interfaces;
using Microsoft.AspNetCore.WebUtilities;

namespace DamaAdmin.Client.Services
{
    public abstract class HttpService<T> : IHttpService<T> where T: class
    {
        protected readonly string endpointName;
        protected readonly HttpClient client;
        private readonly JsonSerializerOptions _options;

        public HttpService(HttpClient client, string endpointName)
        {
            this.client = client;
            this.endpointName = endpointName;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["id"] = id.ToString()
            };
            return await client.DeleteAsync(QueryHelpers.AddQueryString(endpointName, queryStringParam));
        }


        public async Task<PagingResponse<T>> List(PagingParameters parameters)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = parameters.PageNumber.ToString()
            };
            var response = await client.GetAsync(QueryHelpers.AddQueryString(endpointName, queryStringParam));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var pagingResponse = new PagingResponse<T>
            {
                Items = JsonSerializer.Deserialize<List<T>>(content, _options),
                MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
            };

            return pagingResponse;
        }

        public async Task<IEnumerable<T>> ListAll()
        {
            return await client.GetFromJsonAsync<IEnumerable<T>>($"{endpointName}/all", _options);
        }

        public async Task Create(T model)
        {
            var response = await client.PostAsJsonAsync(endpointName, model, _options);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            } 
        }

        public async Task Update(T model)
        {
            var response = await client.PutAsJsonAsync(endpointName, model, _options);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            }
        }
    }
}
