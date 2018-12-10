using ApplicationCore;
using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using Infrastructure.Exceptions;
using Infrastructure.Services.SageOneHelpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SageService : SageServiceBase, ISageService
    {
        public SageService(
            IOptions<SageSettings> options,
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
            if(response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                JObject jObject = JObject.Parse(responseContent);
                string access_token = (string)jObject["access_token"];
                string refresh_token = (string)jObject["refresh_token"];
                return (access_token, refresh_token);
            }
            throw new SageException(response?.StatusCode.ToString(), await response?.Content?.ReadAsStringAsync());
        }

        public async Task<(string AccessToken, string RefreshToken)> GetAccessTokenByRefreshAsync(string refreshToken)
        {
            var httpClient = CreateHttpClient();
            AddDefaultHeaders(httpClient);
            var data = SageOneUtils.ConvertPostParams(SageOneUtils.GetRefreshTokenPostData(_settings.ClientId, _settings.ClientSecret, refreshToken));
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

        public async Task<SageResponseDTO> CreateAnonymousInvoice(string accessToken, string refreshToken, List<OrderItem> orderItems, int referenceId, decimal carriageAmount)
        {
            if (orderItems == null || orderItems.Count == 0)
                return new SageResponseDTO { Message = "Error Input Data", ResponseBody = "Error: No items" };

            List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>("sales_invoice[date]",DateTime.Now.ToString("dd-MM-yyyy")),
                new KeyValuePair<string,string>("sales_invoice[due_date]", DateTime.Now.AddMonths(1).ToString("dd-MM-yyyy")),
                //new KeyValuePair<string,string>("sales_invoice[carriage_tax_rate_id]", "4"),
                new KeyValuePair<string,string>("sales_invoice[vat_exemption_reason_id]", "10"),
                new KeyValuePair<string,string>("sales_invoice[reference]", referenceId.ToString())
            };

            

            var idx = AddLinesToBody(orderItems, body);
            SetCarriageAmount(idx, carriageAmount, body);

            return await CreateInvoice(accessToken, refreshToken, body);
        }        

        public async Task<SageResponseDTO> CreateInvoiceWithTaxNumber(string accessToken, string refreshToken, List<OrderItem> orderItems, string customerName, string taxNumber, string address, string address2, string postalCode, string city, int referenceId, decimal carriageAmount)
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
                new KeyValuePair<string,string>("sales_invoice[main_address_street_2]", address2),
                new KeyValuePair<string,string>("sales_invoice[main_address_postcode]", postalCode),
                new KeyValuePair<string,string>("sales_invoice[main_address_locality]", city),
                new KeyValuePair<string,string>("sales_invoice[main_address_country_id]", "175"),
                //new KeyValuePair<string,string>("sales_invoice[carriage_tax_rate_id]", "4"),
                new KeyValuePair<string,string>("sales_invoice[vat_exemption_reason_id]", "10"),
                new KeyValuePair<string,string>("sales_invoice[reference]", referenceId.ToString())
            };            

            var idx = AddLinesToBody(orderItems, body);
            SetCarriageAmount(idx, carriageAmount, body);

            return await CreateInvoice(accessToken, refreshToken, body);
        }

        private async Task<SageResponseDTO> CreateInvoice(string accessToken, string refreshToken, List<KeyValuePair<string, string>> body)
        {
            if(string.IsNullOrEmpty(accessToken))
            {
                return new SageResponseDTO
                {
                    Message = "Erro: Os acessos à Sage ainda não estão configurados, acede a https://backoffice.damanojornal.com/Sage/"
                };
            }
            try
            {
                var builder = new UriBuilder(_baseUrl)
                {
                    Path = $"accounts/v2/sales_invoices"
                };
                var uri = builder.Uri;

                var httpClient = CreateHttpClient(accessToken);
                HttpRequestMessage request = GenerateRequest(HttpMethod.Post, uri, body, httpClient, accessToken);
                var response = await httpClient.SendAsync(request);
                var result = await HandleResponse(response, HttpMethod.Post, uri, body, accessToken, refreshToken);
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
                _logger.LogWarning("CreateInvoice Error: {0}; StackTrace: {1}", ex.Message, ex.StackTrace);
                var responseError = new SageResponseDTO
                {
                    Message = $"Error exception: {ex.Message}",
                };
                return responseError;
            }
        }

        public async Task<SageResponseDTO> InvoicePayment(string accessToken, string refreshToken, long id, PaymentType paymentType, decimal amount)
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

                var httpClient = CreateHttpClient(accessToken);
                HttpRequestMessage request = GenerateRequest(HttpMethod.Post, uri, body, httpClient, accessToken);
                var response = await httpClient.SendAsync(request);

                var result = await HandleResponse(response, HttpMethod.Post, uri, body, accessToken, refreshToken);
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

        public async Task<string> GetAccountData(string accessToken, string refreshToken)
        {
            try
            {
                //var uri = new Uri(@"https://api.sageone.com/core/v2/business");
                var builder = new UriBuilder(_baseUrl)
                {
                    Path = $"/core/v2/business"
                };
                var uri = builder.Uri;
                var httpClient = CreateHttpClient(accessToken);

                HttpRequestMessage request = GenerateRequest(HttpMethod.Get, uri, null, httpClient, accessToken);
                var response = await httpClient.SendAsync(request);
                var result = await HandleResponse(response, HttpMethod.Get, uri, null, accessToken, refreshToken);
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

        public async Task<string> GetDataAsync(string accessToken, string refreshToken, string url)
        {
            try
            {
                var builder = new UriBuilder(_baseUrl)
                {
                    Path = url
                };
                var uri = builder.Uri;
                var httpClient = CreateHttpClient(accessToken);
                HttpRequestMessage request = GenerateRequest(HttpMethod.Get, uri, null, httpClient, accessToken);
                var response = await httpClient.SendAsync(request);
                var result = await HandleResponse(response, HttpMethod.Get, uri, null, accessToken, refreshToken);
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

        public async Task<byte[]> GetPDFInvoice(string accessToken, string refreshToken, long invoiceId)
        {
            var builder = new UriBuilder(_baseUrl)
            {
                Path = $"/accounts/v2/sales_invoices/{invoiceId}"
            };
            var uri = builder.Uri;
            var httpClient = CreateHttpClient(accessToken);
            HttpRequestMessage request = GenerateRequest(HttpMethod.Get, uri, null, httpClient, accessToken, true);
            var response = await httpClient.SendAsync(request);
            var result = await HandleResponse(response, HttpMethod.Get, uri, null, accessToken, refreshToken);
            return await result.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> GetPDFReceipt(string accessToken, string refreshToken, long invoiceId, long paymentId)
        {
            var builder = new UriBuilder(_baseUrl)
            {
                Path = $"/accounts/v2/sales_invoices/{invoiceId}/payments/{paymentId}"
            };
            var uri = builder.Uri;
            var httpClient = CreateHttpClient(accessToken);
            HttpRequestMessage request = GenerateRequest(HttpMethod.Get, uri, null, httpClient, accessToken, true);
            var response = await httpClient.SendAsync(request);
            var result = await HandleResponse(response, HttpMethod.Get, uri, null, accessToken, refreshToken);
            return await result.Content.ReadAsByteArrayAsync();
        }
        private static void SetCarriageAmount(int index, decimal carriageAmount, List<KeyValuePair<string, string>> body)
        {
            //Check carriage
            if (carriageAmount > 0)
            {
                //var carriageNet = Math.Ceiling(carriageAmount / 1.23m);
                //double multiplier = Math.Pow(10, Convert.ToDouble(2));
                //var carriageNetRoundUp = (Convert.ToDouble(carriageNet) * multiplier) / multiplier;
                //body.Add(new KeyValuePair<string, string>("sales_invoice[carriage]", carriageNetRoundUp.ToString(CultureInfo.InvariantCulture)));
                //body.Add(new KeyValuePair<string, string>("sales_invoice[carriage_tax_rate_id]", "1"));
                //body.Add(new KeyValuePair<string, string>("sales_invoice[vat_exemption_reason_id]", "10"));

                body.AddRange(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(
                        $"sales_invoice[line_items_attributes][{index}][description]", "Despesas de Envio"),
                    new KeyValuePair<string, string>(
                        $"sales_invoice[line_items_attributes][{index}][quantity]", $"1"),
                    new KeyValuePair<string, string>(
                        $"sales_invoice[line_items_attributes][{index}][unit_price]", $"{carriageAmount.ToString(CultureInfo.InvariantCulture)}"),
                    new KeyValuePair<string, string>($"sales_invoice[line_items_attributes][{index}][tax_rate_id]", "4"),
                    new KeyValuePair<string, string>($"sales_invoice[line_items_attributes][{index}][vat_exemption_reason_id]", "10")
                });
            }
        }


        private (string TypeId, string BankId) GetSagePaymentType(PaymentType paymentType)
        {
            var bankInfo = _settings.SageBankings.SingleOrDefault(x => x.Type == paymentType);
            if(bankInfo != null)
                return (bankInfo.SageTypeId, bankInfo.BankId);
            return ("5", "97574");
        }

        private static int AddLinesToBody(List<OrderItem> orderItems, List<KeyValuePair<string, string>> body)
        {
            int idx = 0;
            for (; idx < orderItems.Count; idx++)
            {
                body.AddRange(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(
                        $"sales_invoice[line_items_attributes][{idx}][description]",
                        $"{orderItems[idx].ItemOrdered.ProductName}"),
                    new KeyValuePair<string, string>($"sales_invoice[line_items_attributes][{idx}][quantity]",
                        $"{orderItems[idx].Units}"),
                    new KeyValuePair<string, string>(
                        $"sales_invoice[line_items_attributes][{idx}][unit_price]",
                        $"{orderItems[idx].UnitPrice.ToString(CultureInfo.InvariantCulture)}"),
                    new KeyValuePair<string, string>($"sales_invoice[line_items_attributes][{idx}][tax_rate_id]", "4"),
                    new KeyValuePair<string, string>($"sales_invoice[line_items_attributes][{idx}][vat_exemption_reason_id]", "10")
                });
            }
            return idx;
        }

        private async Task<HttpResponseMessage> HandleResponse(HttpResponseMessage response, HttpMethod httpMethod, Uri uri, List<KeyValuePair<string, string>> body, string accessToken, string refreshToken)
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
                        var tokens = await GetAccessTokenByRefreshAsync(refreshToken);
                        await _authRepository.AddOrUpdateAuthConfigAsync(_settings.ClientApp, tokens.AccessToken, tokens.RefreshToken);
                        //_authConfig = await _authRepository.GetAuthConfigAsync(_settings.ClientApp);
                        //Log new token
                        _logger.LogInformation($"Get NEW ACCESS TOKEN: StatusCode: {response.StatusCode}, ACCESS TOKEN: {tokens.AccessToken}, REFRESH TOKEN: {tokens.RefreshToken}");
                        //Try Request Again
                        var httpClient = CreateHttpClient(tokens.AccessToken);
                        HttpRequestMessage request = GenerateRequest(httpMethod, uri, body, httpClient, tokens.AccessToken);
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
