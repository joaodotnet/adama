using ApplicationCore;
using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DamaWeb.Extensions;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;

namespace DamaWeb.Services
{
    public class CustomizeViewModelService : ICustomizeViewModelService
    {
        private readonly IAsyncRepository<Category> _categoryRepository;
        private readonly IAsyncRepository<CatalogItem> _catalogRepository;
        private readonly IAsyncRepository<CustomizeOrder> _customizeOrderRepository;
        private readonly IEmailSender _emailSender;
        private readonly CatalogSettings _settings;

        public CustomizeViewModelService(
            IAsyncRepository<Category> categoryRepository,
            IAsyncRepository<CatalogItem> catalogRepository,
            IAsyncRepository<CustomizeOrder> customizeRepository,
            IEmailSender emailSender,
            IOptions<CatalogSettings> settings)
        {
            _categoryRepository = categoryRepository;
            _catalogRepository = catalogRepository;
            _customizeOrderRepository = customizeRepository;
            _emailSender = emailSender;
            _settings = settings.Value;
        }

        public async Task<CustomizeViewModel> GetCustomizeItems(int? categoryid, int? catalogItemId)
        {
            var categorySpec = new CategorySpecification();
            var cats = await _categoryRepository.ListAsync(categorySpec);
            List<CatalogItem> products = new List<CatalogItem>();
            if (categoryid.HasValue)
            {
                var catalogSpec = new CatalogFilterSpecification(null, null, categoryid, true);
                products = await _catalogRepository.ListAsync(catalogSpec);
            }

            return new CustomizeViewModel
            {
                Categories = cats.Select(x => (x.Id, x.Name)).ToList(),
                ProductSelected = catalogItemId,
                CatalogItems = products.Select(x => new CatalogItemViewModel
                {
                    CatalogItemId = x.Id,
                    CatalogItemName = x.Name,
                    PictureUri = x.PictureUri,
                    Price = x.Price ?? x.CatalogType.Price,
                    ProductSku = x.Sku,                    
                }).ToList()
            };
        }

        public async Task SendCustomizeService(CustomizeViewModel request)
        {
            //Get Product Name and Sku
            var product = await _catalogRepository.GetByIdAsync(request.ProductSelected.Value);

            //Save Order
            var order = await _customizeOrderRepository.AddAsync(new CustomizeOrder
            {
                BuyerId = request.BuyerEmail,
                BuyerName = request.BuyerName,
                BuyerContact = request.BuyerPhone,
                Description = request.Description,
                Text = request.Text,
                Colors = request.Colors,
                ItemOrdered = new ApplicationCore.Entities.OrderAggregate.CatalogItemOrdered(product.Id, product.Name, product.PictureUri),
                AttachFileName = GetFileName(request.UploadFile?.FileName)
            });

            //Send mails to admins
            var body = $"Email: {request.BuyerEmail} <br>" +
                $"Nome: {request.BuyerName} <br>" +
                $"Telemóvel: {request.BuyerPhone} <br>" +
                $"Produto: {product.Name} ({product.Sku}) <br>" + //Nome de produto (SKU) 
                $"Descrição da Ilustração: {request.Description} <br>" +
                $"Frase ou nome: {request.Text} <br>" +
                $"Cores: {request.Colors} <br>";
            if (!string.IsNullOrEmpty(order.Colors))
            {
                var colors = order.Colors.Split(',');
                foreach (var rgb in colors)
                {
                    var rgbText = rgb.Replace(';', ',');
                    body += $@"<div style='width:20px;height:20px;display:inline;float:left;background-color: {rgbText}'>&nbsp;</div>";
                }
            }
            body += $"<br><br>Ficheiro em anexo: {(request.UploadFile != null ? "Sim" : "Não")}";
            if (request.UploadFile == null)
                await _emailSender.SendEmailAsync(_settings.FromOrderEmail, _settings.ToEmails, $"Dama no Jornal®: Novo Pedido de Encomenda Personalizada #{order.Id}", body);
            else
                await _emailSender.SendEmailAsync(_settings.FromOrderEmail, _settings.ToEmails, $"Dama no Jornal®: Novo Pedido de Encomenda Personalizada #{order.Id}", body, null, request.UploadFile);

            //send mails to buyer
            body = GetEmailBody(order);
            await _emailSender.SendEmailAsync(_settings.FromOrderEmail, order.BuyerId, $"Dama no Jornal®: Personalização nº{order.Id}", body);

        }

        private string GetFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            if (fileName.LastIndexOf('\\') > 0)
                return fileName.Substring(fileName.LastIndexOf('\\') + 1);
            else if (fileName.LastIndexOf('/') > 0)
                return fileName.Substring(fileName.LastIndexOf('/') + 1);
            return fileName;
        }

        private string GetEmailBody(CustomizeOrder order)
        {
            string body = $@"
<table style='width:550px;'>
        <tr>
            <td width='400px' style='vertical-align:bottom'>
                Olá <strong>{order.BuyerName}</strong><br />
                Obrigada por escolher a Dama no Jornal®.<br />
                O seu pedido foi enviado com <strong>Sucesso!</strong> <br />
            </td>
            <td>
                <img src='https://www.damanojornal.com/loja/images/dama_bird.png' width='150' />
            </td>
        </tr>
    </table>
    <div style='width:550px'>
        <img width='100%' src='https://www.damanojornal.com/loja/images/linha-coracao.png' />
    </div>
    <div>
        PERSONALIZAÇÃ0 #{order.Id}<br />
        ESTADO: {EnumHelper<OrderStateType>.GetDisplayValue(order.OrderState)} <br />
    </div>
    <div style='margin-top:20px;width:550px'>
        <table width='100%'>";


            body += $@"
            <tr>
                <td width='250px'>
                    <img width='250' src='{order.ItemOrdered.PictureUri}' />
                </td>
                <td style='padding-bottom:20px;vertical-align:bottom'>
                    <table>
                        <tr>
                            <td>{order.ItemOrdered.ProductName}</td>                            
                        </tr>
                        <tr>
                            <td><strong>Descrição:</strong><br> {order.Description}</td>
                        </tr>";
            if (!string.IsNullOrEmpty(order.Text))
            {
                body += $@"<tr>
                            <td><strong>Texto ou Frase:</strong><br> {order.Text}</td>
                        </tr>";
            }
            if (!string.IsNullOrEmpty(order.Colors))
            {
                var colors = order.Colors.Split(',');
                body += $@"<tr>
                            <td><strong>Cores:</strong><br>";
                foreach (var rgb in colors)
                {
                    var rgbText = rgb.Replace(';', ',');
                    body += $@"<div style='width:20px;height:20px;display:inline;float:left;background-color: {rgbText}'>&nbsp;</div>";
                }

                body += @"</td>
                        </tr>";
            }
            if (!string.IsNullOrEmpty(order.AttachFileName))
            {
                body += $@"<tr>
                            <td><strong>Ficheiro Anexo:</strong><br> {order.AttachFileName}</td>
                        </tr>";
            }
            body += $@"                       
                    </table>
                </td>
            </tr>";


            body += $@"</table>
    </div>
    <div style='margin-top:20px;width:550px'>
        <img width='100%' src='https://www.damanojornal.com/loja/images/linha-coracao.png' />
    </div>            
    <div style='margin-top:20px;background-color:#eeebeb;width:550px;padding: 5px;'>
        <h3 style='text-align:center;width:550px'>INFORMAÇÕES</h3>
        <div style='text-align:center;width:550px'>
            <p>Vamos analisar o seu pedido e entraremos em contato consigo o mais breve possível.</p>
        </div>        
    </div>
    <div style='margin-top:20px;text-align:center;width:550px'>
        <strong>Se tem alguma dúvida sobre o seu pedido, por favor contacte-nos.</strong>
    </div>
    <div style='margin-top:20px;text-align:center;width:550px'>
        <strong>Muito Obrigada,</strong>
    </div>
    <div style='color: #EF7C8D;text-align:center;width:550px'>
        ❤ Dama no Jornal®
    </div>
    <div style='text-align:center;width:550px'>
        <img width='100' src='https://www.damanojornal.com/loja/images/logo_name.png' />
    </div>";
            return body;
        }
    }
}
