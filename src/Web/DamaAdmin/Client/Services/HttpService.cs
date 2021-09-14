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
        private readonly string _endpointName;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;

        public HttpService(HttpClient client, string endpointName)
        {
            _client = client;
            _endpointName = endpointName;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["id"] = id.ToString()
            };
            return await _client.DeleteAsync(QueryHelpers.AddQueryString(_endpointName, queryStringParam));
        }


        public async Task<PagingResponse<T>> List(PagingParameters parameters)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = parameters.PageNumber.ToString()
            };
            var response = await _client.GetAsync(QueryHelpers.AddQueryString(_endpointName, queryStringParam));
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
            return await _client.GetFromJsonAsync<IEnumerable<T>>($"{_endpointName}/all", _options);
        }

        public async Task Create(T model)
        {
            var response = await _client.PostAsJsonAsync(_endpointName, model, _options);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            } 
        }

        public async Task Update(T model)
        {
            var response = await _client.PutAsJsonAsync(_endpointName, model, _options);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            }
        }
    }
}
