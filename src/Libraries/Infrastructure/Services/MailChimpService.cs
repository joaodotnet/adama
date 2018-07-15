using ApplicationCore;
using ApplicationCore.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class MailChimpService : IMailChimpService
    {
        private readonly CatalogSettings _settings;

        public MailChimpService(IOptions<CatalogSettings> settings)
        {
            _settings = settings.Value;
        }
        public async Task AddSubscriberAsync(string email)
        {
            var httpClient = CreateHttpClient(_settings.MailChimpBasicAuth);

            object data = new { email_address = email, status = "subscribed" };

            var content = new StringContent(JsonConvert.SerializeObject(data));
            var response = await httpClient.PostAsync($"{_settings.MailChimpBaseUrl}/lists/{_settings.MailChimpListId}/members", content);
        }

        protected HttpClient CreateHttpClient(string token = "")
        {
            var httpClient = new HttpClient(new LoggingHandler(new HttpClientHandler()));

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            }
            return httpClient;
        }
    }
}
