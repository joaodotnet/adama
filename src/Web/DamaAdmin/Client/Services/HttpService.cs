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

        protected JsonSerializerOptions Options => new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public HttpService(HttpClient client, string endpointName)
        {
            this.client = client;
            this.endpointName = "api/" + endpointName;
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
            Console.WriteLine("url: " + response.RequestMessage.RequestUri.ToString());
            Console.WriteLine("response status:" + response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("response content: " + content);
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var pagingResponse = new PagingResponse<T>
            {
                Items = JsonSerializer.Deserialize<List<T>>(content, Options),
                MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), Options)
            };

            return pagingResponse;
        }

        public async Task<IEnumerable<T>> ListAll()
        {
            return await client.GetFromJsonAsync<IEnumerable<T>>($"{endpointName}/all", Options);
        }

        public async Task Upsert(T model)
        {
            var response = await client.PostAsJsonAsync(endpointName, model, Options);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            } 
        }

        public async Task Update(T model)
        {
            var response = await client.PutAsJsonAsync(endpointName, model, Options);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            }
        }
    }
}
