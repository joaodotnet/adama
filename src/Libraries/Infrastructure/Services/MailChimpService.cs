using ApplicationCore;
using ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;
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
        private readonly HttpClient _client;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<MailChimpService> _logger;

        public MailChimpService(HttpClient client,
            IOptions<CatalogSettings> settings,
            IEmailSender emailSender,
            ILogger<MailChimpService> logger)
        {
            _settings = settings.Value;
            _client = client;
            _emailSender = emailSender;
            _logger = logger;
        }
        public async Task AddSubscriberAsync(string email)
        {
            //var httpClient = CreateHttpClient(_settings.MailChimpBasicAuth);
            object data = new { email_address = email, status = "subscribed" };
            var content = new StringContent(JsonConvert.SerializeObject(data));
            var response = await _client.PostAsync($"/3.0/lists/{_settings.MailChimpListId}/members", content);
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error while subscribing mailchimp ({0}) {1}", response.StatusCode, message);
                await _emailSender.SendEmailAsync(_settings.Email.FromInfoEmail, _settings.Email.CCEmails, "Erro ao subscrever na newsletter",
                        $"O endereço {email} tentou subscrever na loja mas o servidor de mailchimp de erro ({response.StatusCode}) {message}");
            }
            else
                _logger.LogInformation("Successuful subscribing email {0} to mailchimp", email);
            //var response = await httpClient.PostAsync($"{_settings.MailChimpBaseUrl}/lists/{_settings.MailChimpListId}/members", content);
        }
    }
}
