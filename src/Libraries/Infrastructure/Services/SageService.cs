using ApplicationCore;
using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using Infrastructure.Services.SageOneHelpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SageService : SageServiceBase, ISageService
    {
        public SageService(
            IOptions<BackofficeSettings> options,
            IAuthConfigRepository authConfigRepository,
            IAppLogger<SageService> logger) : base(options, authConfigRepository, logger)
        {

        }

        public async Task<(string AccessToken, string RefreshToken)> GetAccessTokenAsync(string code)
        {
            var httpClient = CreateHttpClient();
            AddDefaultHeaders(httpClient);
            var data = SageOneUtils.ConvertPostParams(SageOneUtils.GetAccessTokenPostData(code, _settings.ClientId, _settings.ClientSecret, _settings.CallbackURL));
            var content = new StringContent(data);

            var uri = new Uri(_settings.AccessTokenURL);
            var response = await httpClient.PostAsync(uri, content);

            //await HandleResponse(response);
            var responseContent = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(responseContent);
            string access_token = (string)jObject["access_token"];
            string refresh_token = (string)jObject["refresh_token"];
            return (access_token, refresh_token);
        }

        public async Task<(string AccessToken, string RefreshToken)> GetAccessTokenByRefreshAsync()
        {
            var httpClient = CreateHttpClient();
            AddDefaultHeaders(httpClient);
            var data = SageOneUtils.ConvertPostParams(SageOneUtils.GetRefreshTokenPostData(_settings.ClientId, _settings.ClientSecret, _authConfig.RefreshToken));
            var content = new StringContent(data);

            var uri = new Uri(_settings.AccessTokenURL);
            var response = await httpClient.PostAsync(uri, content);

            //var responseObj = await HandleResponse(response);
            var responseContent = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(responseContent);
            string access_token = (string)jObject["access_token"];
            string refresh_token = (string)jObject["refresh_token"];
            return (access_token, refresh_token);
        }

        public async Task<SageResponseDTO> CreateAnonymousInvoice(List<OrderItem> orderItems, int referenceId)
        {
            if (orderItems == null || orderItems.Count == 0)
                return new SageResponseDTO { Message = "Error Input Data", ResponseBody = "Error: No items" };

            List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>("sales_invoice[date]",DateTime.Now.ToString("dd-MM-yyyy")),
                new KeyValuePair<string,string>("sales_invoice[due_date]", DateTime.Now.AddMonths(1).ToString("dd-MM-yyyy")),
                new KeyValuePair<string,string>("sales_invoice[carriage_tax_rate_id]", "4"),
                new KeyValuePair<string,string>("sales_invoice[vat_exemption_reason_id]", "10"),
                new KeyValuePair<string,string>("sales_invoice[reference]", referenceId.ToString())
            };
            AddLinesToBody(orderItems, body);
            return await CreateInvoice(body);
        }



        public async Task<SageResponseDTO> CreateInvoiceWithTaxNumber(List<OrderItem> orderItems, string customerName, string taxNumber, string address, string postalCode, string city, int referenceId)
        {
            if (orderItems == null || orderItems.Count == 0)
                return new SageResponseDTO { Message = "Error Input Data", ResponseBody = "Error: No items" };

            List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>("sales_invoice[customer_company]", customerName),
                new KeyValuePair<string,string>("sales_invoice[customer_tax_number]",taxNumber),
                new KeyValuePair<string,string>("sales_invoice[date]",DateTime.Now.ToString("dd-MM-yyyy")),
                new KeyValuePair<string,string>("sales_invoice[due_date]", DateTime.Now.AddMonths(1).ToString("dd-MM-yyyy")),
                new KeyValuePair<string,string>("sales_invoice[main_address_street_1]", address),
                new KeyValuePair<string,string>("sales_invoice[main_address_postcode]", postalCode),
                new KeyValuePair<string,string>("sales_invoice[main_address_locality]", city),
                new KeyValuePair<string,string>("sales_invoice[main_address_country_id]", "175"),
                new KeyValuePair<string,string>("sales_invoice[carriage_tax_rate_id]", "4"),
                new KeyValuePair<string,string>("sales_invoice[vat_exemption_reason_id]", "10"),
                new KeyValuePair<string,string>("sales_invoice[reference]", referenceId.ToString())
            };

            AddLinesToBody(orderItems, body);
            return await CreateInvoice(body);
        }

        private async Task<SageResponseDTO> CreateInvoice(List<KeyValuePair<string, string>> body)
        {
            try
            {
                var builder = new UriBuilder(_baseUrl)
                {
                    Path = $"accounts/v2/sales_invoices"
                };
                var uri = builder.Uri;

                var httpClient = CreateHttpClient(_authConfig.AccessToken);
                HttpRequestMessage request = GenerateRequest(HttpMethod.Post, uri, body, httpClient);
                var response = await httpClient.SendAsync(request);
                var result = await HandleResponse(response, HttpMethod.Post, uri, body);
                var responseObj = await FormatResponse(result);
                if (responseObj.StatusCode == HttpStatusCode.Created)
                {
                    JObject jObject = JObject.Parse(responseObj.ResponseBody);
                    responseObj.InvoiceId = (long)jObject["id"];
                    responseObj.InvoiceNumber = (string)jObject["artefact_number"];
                }
                return responseObj;
            }
            catch (Exception ex)
            {
                var responseError = new SageResponseDTO
                {
                    Message = $"Error exception: {ex.Message}",
                };
                return responseError;
            }
        }

        public async Task<SageResponseDTO> InvoicePayment(long id, PaymentType paymentType, decimal amount)
        {
            try
            {
                var sagePaymentConfig = GetSagePaymentType(paymentType);
                List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string,string>("sales_invoice_payment[date]",DateTime.Now.ToString("dd-MM-yyyy")),
                    new KeyValuePair<string,string>("sales_invoice_payment[amount]", amount.ToString(CultureInfo.InvariantCulture)),
                    new KeyValuePair<string,string>("sales_invoice_payment[bank_account_id]", sagePaymentConfig.BankId),
                    new KeyValuePair<string,string>("sales_invoice_payment[payment_type_id]", sagePaymentConfig.TypeId) 
                };

                var builder = new UriBuilder(_baseUrl)
                {
                    Path = $"accounts/v2/sales_invoices/{id}/payments"
                };

                var uri = builder.Uri;

                var httpClient = CreateHttpClient(_authConfig.AccessToken);
                HttpRequestMessage request = GenerateRequest(HttpMethod.Post, uri, body, httpClient);
                var response = await httpClient.SendAsync(request);

                var result = await HandleResponse(response, HttpMethod.Post, uri, body);
                var responseObj = await FormatResponse(result);
                if (responseObj.StatusCode == HttpStatusCode.Created)
                {
                    JObject jObject = JObject.Parse(responseObj.ResponseBody);
                    responseObj.PaymentId = (long)jObject["id"];
                }
                //var json = new { StatusCode = response.StatusCode, ResponseBody = await response.Content.ReadAsStringAsync() };
                return responseObj;
            }
            catch (Exception ex)
            {
                var responseError = new SageResponseDTO
                {
                    Message = $"Error exception: {ex.Message}",
                };
                return responseError;
            }
        }        

        public async Task<string> GetAccountData()
        {
            try
            {
                //var uri = new Uri(@"https://api.sageone.com/core/v2/business");
                var builder = new UriBuilder(_baseUrl)
                {
                    Path = $"/core/v2/business"
                };
                var uri = builder.Uri;
                var httpClient = CreateHttpClient(_authConfig.AccessToken);

                HttpRequestMessage request = GenerateRequest(HttpMethod.Get, uri, null, httpClient);
                var response = await httpClient.SendAsync(request);
                var result = await HandleResponse(response, HttpMethod.Get, uri, null);
                //var json = new { StatusCode = response.StatusCode, ResponseBody = await response.Content.ReadAsStringAsync() };
                return JsonConvert.SerializeObject(await FormatResponse(result), Formatting.Indented);
            }
            catch (Exception ex)
            {
                var json = new
                {
                    ExceptionMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerExceptionMessage = ex.InnerException?.Message,
                    InnerExceptionStackTrace = ex.InnerException?.StackTrace
                };
                return JsonConvert.SerializeObject(json, Formatting.Indented);
            }
        }

        public async Task<string> GetDataAsync(string url)
        {
            try
            {
                var builder = new UriBuilder(_baseUrl)
                {
                    Path = url
                };
                var uri = builder.Uri;
                var httpClient = CreateHttpClient(_authConfig.AccessToken);
                HttpRequestMessage request = GenerateRequest(HttpMethod.Get, uri, null, httpClient);
                var response = await httpClient.SendAsync(request);
                var result = await HandleResponse(response, HttpMethod.Get, uri, null);
                //var json = new { StatusCode = response.StatusCode, ResponseBody = await response.Content.ReadAsStringAsync() };
                return JsonConvert.SerializeObject(await FormatResponse(result), Formatting.Indented);
            }
            catch (Exception ex)
            {
                var json = new
                {
                    ExceptionMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerExceptionMessage = ex.InnerException?.Message,
                    InnerExceptionStackTrace = ex.InnerException?.StackTrace
                };
                return JsonConvert.SerializeObject(json, Formatting.Indented);
            }
        }

        public async Task<byte[]> GetPDFInvoice(long invoiceId)
        {
            var builder = new UriBuilder(_baseUrl)
            {
                Path = $"/accounts/v2/sales_invoices/{invoiceId}"
            };
            var uri = builder.Uri;
            var httpClient = CreateHttpClient(_authConfig.AccessToken);
            HttpRequestMessage request = GenerateRequest(HttpMethod.Get, uri, null, httpClient, true);
            var response = await httpClient.SendAsync(request);
            var result = await HandleResponse(response, HttpMethod.Get, uri, null);
            return await result.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> GetPDFReceipt(long invoiceId, long paymentId)
        {
            var builder = new UriBuilder(_baseUrl)
            {
                Path = $"/accounts/v2/sales_invoices/{invoiceId}/payments/{paymentId}"
            };
            var uri = builder.Uri;
            var httpClient = CreateHttpClient(_authConfig.AccessToken);
            HttpRequestMessage request = GenerateRequest(HttpMethod.Get, uri, null, httpClient, true);
            var response = await httpClient.SendAsync(request);
            var result = await HandleResponse(response, HttpMethod.Get, uri, null);
            return await result.Content.ReadAsByteArrayAsync();
        }

        private (string TypeId, string BankId) GetSagePaymentType(PaymentType paymentType)
        {
            switch (paymentType)
            {
                case PaymentType.TRANSFER:
                    return ("4","95588");
                case PaymentType.CASH:
                default:
                    return ("5","95589");
            }
        }

        private static void AddLinesToBody(List<OrderItem> orderItems, List<KeyValuePair<string, string>> body)
        {
            for (int i = 0; i < orderItems.Count; i++)
            {
                body.AddRange(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(
                        $"sales_invoice[line_items_attributes][{i}][description]",
                        $"{orderItems[i].ItemOrdered.ProductName}"),
                    new KeyValuePair<string, string>($"sales_invoice[line_items_attributes][{i}][quantity]",
                        $"{orderItems[i].Units}"),
                    new KeyValuePair<string, string>(
                        $"sales_invoice[line_items_attributes][{i}][unit_price]",
                        $"{orderItems[i].UnitPrice.ToString(CultureInfo.InvariantCulture)}"),
                    new KeyValuePair<string, string>($"sales_invoice[line_items_attributes][{i}][tax_rate_id]", "4"),
                    new KeyValuePair<string, string>($"sales_invoice[line_items_attributes][{i}][vat_exemption_reason_id]", "10")
                });
            }
        }

        private async Task<HttpResponseMessage> HandleResponse(HttpResponseMessage response, HttpMethod httpMethod, Uri uri, List<KeyValuePair<string, string>> body)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    JObject jObject = JObject.Parse(content);
                    string error = (string)jObject["error"];
                    if (error == "invalid_token")
                    {
                        //Request new token
                        var tokens = await GetAccessTokenByRefreshAsync();
                        await _authRepository.AddOrUpdateAuthConfigAsync(DamaApplicationId.DAMA_BACKOFFICE, tokens.AccessToken, tokens.RefreshToken);
                        _authConfig = await _authRepository.GetAuthConfigAsync(DamaApplicationId.DAMA_BACKOFFICE);
                        //Log new token
                        _logger.LogInformation($"Get NEW ACCESS TOKEN: StatusCode: {response.StatusCode}, ACCESS TOKEN: {tokens.AccessToken}, REFRESH TOKEN: {tokens.RefreshToken}");
                        //Try Request Again
                        var httpClient = CreateHttpClient(_authConfig.AccessToken);
                        HttpRequestMessage request = GenerateRequest(httpMethod, uri, body, httpClient);
                        response = await httpClient.SendAsync(request);
                    }
                }
            }
            return response;

        }

        private async Task<SageResponseDTO> FormatResponse(HttpResponseMessage message)
        {
            return new SageResponseDTO { StatusCode = message.StatusCode, Message = "Success", ResponseBody = await message.Content.ReadAsStringAsync() };
        }
    }
}
