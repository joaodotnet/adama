using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ApplicationCore.DTOs;
using DamaAdmin.Shared.Features;
using DamaAdmin.Shared.Interfaces;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace DamaAdmin.Client.Services
{
    public abstract class HttpService<T> : IHttpService<T> where T: class
    {
        protected readonly string endpointName;
        protected readonly HttpClient client;

        protected JsonSerializerOptions SerlializerOptions => new() { PropertyNameCaseInsensitive = true };

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
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var pagingResponse = new PagingResponse<T>
            {
                Items = JsonSerializer.Deserialize<List<T>>(content, SerlializerOptions),
                MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), SerlializerOptions)
            };

            return pagingResponse;
        }

        public async Task<IEnumerable<T>> ListAll()
        {
            return await client.GetFromJsonAsync<IEnumerable<T>>($"{endpointName}/all", SerlializerOptions);
        }

        public async Task Upsert(T model)
        {
            var response = await client.PostAsJsonAsync(endpointName, model, SerlializerOptions);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            } 
        }

        public async Task Update(T model)
        {
            var response = await client.PutAsJsonAsync(endpointName, model, SerlializerOptions);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            }
        }

        public async Task<T> GetById(int id)
        {
            Console.WriteLine($"url: {endpointName}/{id}");
            return await client.GetFromJsonAsync<T>($"{endpointName}/{id}", SerlializerOptions);
        }

        public virtual async Task<bool> CheckIfCodeExists(string code, int? id)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["code"] = code
            };
            if (id.HasValue && id != 0)
                queryStringParam.Add("id", id.ToString());

            var response = await client.GetAsync(QueryHelpers.AddQueryString($"{endpointName}/code/exists", queryStringParam));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            return bool.Parse(content);
        }

        public async Task<IList<FileData>> FileSaveAsync(MultipartFormDataContent content)
        {
            List<FileData> uploadResults = new();

            var response = await client.PostAsync($"api/filesave", content);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                using var responseStream =
                    await response.Content.ReadAsStreamAsync();

                var newUploadResults = await JsonSerializer
                    .DeserializeAsync<IList<FileData>>(responseStream, options);

                if (newUploadResults is not null)
                {
                    uploadResults = newUploadResults.ToList();
                }
            }
            return uploadResults;
        }
    }
}
