using ApplicationCore.Interfaces;
using Infrastructure.ATInvoice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ATService : IATService
    {
        public async Task<bool> CreateInvoiceAsync()
        {
            try
            {
                var request = new RegisterInvoiceRequest
                {
                    RegisterInvoiceElem = new RegisterInvoiceType
                    {
                        TaxRegistrationNumber = "247609161",
                        InvoiceNo = "FT 1/1",
                        InvoiceDate = DateTime.Now,
                        InvoiceType = InvoiceType.FT,
                        InvoiceStatus = InvoiceStatus.N,
                        Item = "227940032", //Final Customer: 999999990                 
                        Line = new RegisterInvoiceTypeLine[]
                    {
                        new RegisterInvoiceTypeLine {
                            TaxExemptionReason = "M10",
                            Tax = new Tax
                            {
                                TaxType = "IVA",
                                TaxCountryRegion = "PT",
                                TaxPercentage = 23
                            },
                            Item = 1
                        }
                    },
                        DocumentTotals = new RegisterInvoiceTypeDocumentTotals
                        {
                            TaxPayable = 0,
                            NetTotal = 1,
                            GrossTotal = 1
                        }

                    }
                };
                //Certificado:
                string certificatePath = @"C:\projects\nsfw\Certificados\Teste\TesteWebServices.pfx";
                string certificatePass = "TESTEwebservice";

                X509Certificate2 cert = new X509Certificate2(certificatePath, certificatePass, X509KeyStorageFlags.Exportable);


                var binding = new BasicHttpsBinding();
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                var endpoint = new EndpointAddress(new Uri("https://servicos.portaldasfinancas.gov.pt:700/fews/faturas")); //PROD: https://servicos.portaldasfinancas.gov.pt:400/fews/faturas                               
                var channelFactory = new ChannelFactory<faturas>(binding, endpoint);
                channelFactory.Credentials.ClientCertificate.Certificate = cert;
                var serviceClient = channelFactory.CreateChannel();                
                var result = await serviceClient.RegisterInvoiceAsync(request);
                channelFactory.Close();
            }
            catch (WebException webExeption)
            {
                if (webExeption.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse resp = webExeption.Response;
                    StreamReader sr = new StreamReader(resp.GetResponseStream());
                    var msg =  sr.ReadToEnd();
                    System.Diagnostics.Debug.WriteLine($">>>>>>>>>>>>> {msg} <<<<<<<<<<<<<<<<<<<");
                }
                else
                    System.Diagnostics.Debug.WriteLine($">>>>>>>>>>>>> {webExeption.Message} <<<<<<<<<<<<<<<<<<<");
                throw;
            }
            catch (Exception ex)
            { 
                System.Diagnostics.Debug.WriteLine($">>>>>>>>>>>>> {ex.Message} <<<<<<<<<<<<<<<<< ");
                throw;
            }
            
            return true;
        }
    }
}
