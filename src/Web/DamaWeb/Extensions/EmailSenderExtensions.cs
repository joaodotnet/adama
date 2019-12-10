using ApplicationCore;
using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using DamaWeb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string fromEmail, string toEmail, string link)
        {
            return emailSender.SendGenericEmailAsync(fromEmail, toEmail, "Dama no Jornal® - Confirme o seu email",
                $"Por favor confirme a sua conta <a href='{HtmlEncoder.Default.Encode(link)}'>clicando aqui</a>.");
        }

        public static Task SendResetPasswordAsync(this IEmailSender emailSender, string fromEmail, string toEmail, string callbackUrl)
        {
            return emailSender.SendGenericEmailAsync(fromEmail, toEmail, "Dama no Jornal® - Recuperar Password",
                $"Por favor recupere a password <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");
        }

        public static async Task SendCreateOrderEmailAsync(this IEmailSender emailSender, string from, string cc, string buyerName, Order order, List<(int OrderItemId, List<CatalogAttribute> Attributes)> orderAttributes, bool pickupAtStore, DeliveryTimeDTO deliveryTime, bool wantInvoice)
        {
            string body;
            if (order.OrderItems.Any(x => x.CustomizeItem.CatalogTypeId.HasValue))
                body = GeCustomizeEmailBody(order.BuyerId, order, pickupAtStore, wantInvoice);
            else
                body = GetEmailBody(buyerName, order, orderAttributes, pickupAtStore, deliveryTime, wantInvoice);

            await emailSender.SendEmailAsync(from, order.BuyerId, $"Dama no Jornal® - Encomenda #{order.Id}", body, cc);
        }

        private static string GetEmailBody(string buyerName, Order order, List<(int OrderItemId, List<CatalogAttribute> Attributes)> orderAttributes, bool pickupAtStore, DeliveryTimeDTO deliveryTime, bool wantInvoice)
        {
            string body = $@"
<table style='width:550px;'>
        <tr>
            <td width='400px' style='vertical-align:bottom'>
                Olá <strong>{buyerName}</strong><br />
                Obrigada por escolheres a Dama no Jornal®.<br />
                A tua encomenda foi criada com <strong>Sucesso!</strong> <br />
                O próximo passo será efectuares o pagamento com os dados que vais encontrar a baixo. <br />
                <strong>Obrigada!</strong><br />
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
        ENCOMENDA #{order.Id}<br />
        ESTADO: {EnumHelper<OrderStateType>.GetDisplayValue(order.OrderState)} <br />
    </div>
    <div style='margin-top:20px;width:550px'>
        <table width='100%'>";

            foreach (var item in order.OrderItems)
            {
                //Get Attribtues
                var attributes = orderAttributes?.SingleOrDefault(x => x.OrderItemId == item.Id);
                body += $@"
            <tr>
                <td width='250px'>
                    <img width='250' src='{item.ItemOrdered.PictureUri}' />
                </td>
                <td style='padding-bottom:20px;vertical-align:bottom'>
                    <table>
                        <tr>
                            <td>{item.ItemOrdered.ProductName}</td>
                            <td>{item.UnitPrice} €</td>
                        </tr>";
                if (!string.IsNullOrEmpty(item.CustomizeName))
                {
                    var sideText = item.CustomizeSide;
                    if (item.CustomizeSide.LastIndexOf('-') > 0)
                        sideText = item.CustomizeSide.Substring(item.CustomizeSide.LastIndexOf('-') + 1);

                    body += $@"<tr>
                            <td>Personalização: {item.CustomizeName} ({sideText})</td>
                        </tr>";
                }
                if (attributes != null)
                {
                    foreach (var attr in attributes?.Attributes)
                    {
                        body += $@"<tr>
                            <td>{EnumHelper<AttributeType>.GetDisplayValue(attr.Type)}: {attr.Name}</td>
                        </tr>";
                    }
                }
                body += $@"<tr>
                            <td>Quantidade: {item.Units}</td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan='2'><hr /></td>
            </tr>";
            }

            body += $@"</table>
    </div>
    <div style='margin-top:20px;width:550px'>
        <table width='100%'>";            
            body += $@"<tr>
                <td>Subtotal</td>
                <td style='text-align:right'>{(order.SubTotal()).ToString("N2")} €</td>
            </tr>
            <tr>
                <td colspan='2'>
                    <hr />
                </td>
            </tr>
            <tr>
                <td>Envio</td>
                <td style='text-align:right'>{order.ShippingCost.ToString("N2")} €</td>
            </tr>
            <tr>
                <td colspan='2'>
                    <hr />
                </td>
            </tr>
            <tr>
                <td><strong>Total</strong></td>
                <td style='text-align:right'>{order.Total().ToString("N2")} €</td>
            </tr>
            <tr>
                <td colspan='2'>
                    <hr />
                </td>
            </tr>
        </table>
    </div>
    <div style='margin-top:20px;width:550px'>
        <img width='100%' src='https://www.damanojornal.com/loja/images/linha-coracao.png' />
    </div>";
            if (wantInvoice)
            {
                body += $@"<div style='background-color:#eeebeb;width:550px;padding: 5px;'>
        <h3 style='margin-top:20px;text-align:center;width:550px'>INFORMAÇÔES DE FACTURAÇÂO</h3>
        <div style='text-align:center;width:550px'>
            <strong>{order.BillingToAddress.Name}</strong>";
                if (order.TaxNumber.HasValue)
                    body += $"({order.TaxNumber})";
                body += $@"
            <br />
            {order.BillingToAddress.Street}<br />
            {order.BillingToAddress.PostalCode} {order.BillingToAddress.City}
        </div>
    </div>";
            }
            body += $@"<div style='margin-top:20px;background-color:#eeebeb;width:550px;padding: 5px;'>
        <h3 style='text-align:center'>INFORMAÇÕES DE ENVIO*</h3>
        <div style='text-align:center;width:550px'>
            <strong>{order.ShipToAddress.Name}</strong>";
            if (order.TaxNumber.HasValue)
                body += $"({order.TaxNumber})";

            body += $@"<br />
            Telefone: {order.PhoneNumber}<br/>
            {order.ShipToAddress.Street}<br />
            {order.ShipToAddress.PostalCode} {order.ShipToAddress.City}
        </div>
        <div style='margin-top:20px;text-align:center;width:550px'>
            <strong>Tempo de entrega:</strong> {deliveryTime.Min} a {deliveryTime.Max} {EnumHelper<DeliveryTimeUnitType>.GetDisplayValue(deliveryTime.Unit)} úteis para artigos em stock
        </div>
    </div>
    <div style='margin-top:20px;background-color:#eeebeb;width:550px;padding: 5px;'>
        <h3 style='text-align:center;width:550px'>MÉTODO DE ENVIO</h3>
        <div style='text-align:center;width:550px'>
            <strong>";
            if (pickupAtStore) body += "Recolha no nosso Ponto de Referência:"; else body += "Correio Registado em mão (CTT e CTT EXPRESSO)";
            body += "</strong><br />";
            if (pickupAtStore)
                body += @"
            Mercado Municipal de Loulé<br />
            <a href='https://goo.gl/maps/vHLacbNAqdo' style='color: #EF7C8D;text-decoration:underline'>Ver Mapa</a>";
            body += $@"</div>

    </div>";            
            if (!string.IsNullOrEmpty(order.Observations))
            {
                body += $@"
                <div style='margin-top:20px;background-color:#eeebeb;width:550px;padding: 5px;'>
                    <h3 style='text-align:center;width:550px'>OBSERVAÇÕES</h3>
                    <div style='text-align:center;width:550px'>
                    {order.Observations}
                    </div>
                </div>";
            }
            body += $@"<div style='margin-top:20px;background-color:#eeebeb;width:550px;padding: 5px;'>
        <h3 style='text-align:center;width:550px'>INFORMAÇÕES DE PAGAMENTO</h3>
        <div style='text-align:center;width:550px'>
            <h4>Para concluir a sua encomenda por favor faça uma transferência bancária com os seguintes dados:</h4>
        </div>
        <div style='margin-top:20px;text-align:center;width:550px'>
            Valor: {order.Total()} €<br />
            IBAN PT50004572114025360687172<br />
            NIB 004572114025360687172<br />
            CAIXA DE CRÉDITO AGRÍCOLA<br />
            <strong>Titular da conta:</strong> Susana Nunes<br />
        </div>
        <div style='margin-top:20px;text-align:center;width:550px'>
            <span>E envie o comprovativo de pagamento em resposta a este email, ou envie um mail para encomendas@damanojornal.com indicando a referência de encomenda nº {order.Id}.</span>
        </div>
    </div>
    <div style='margin-top:20px;text-align:center;width:550px'>
        <strong>Se tem alguma dúvida sobre a sua encomenda, por favor contacte-nos.</strong>
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

        private static string GeCustomizeEmailBody(string buyerName, Order order, bool pickupAtStore, bool wantInvoice)
        {
            string body = $@"
<table style='width:550px;'>
        <tr>
            <td width='400px' style='vertical-align:bottom'>
                Olá <strong>{buyerName}</strong><br />
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

            foreach (var item in order.OrderItems)
            {
                string pictureUri, productName, description, descriptionLabel;
                if (item.CustomizeItem.CatalogTypeId.HasValue)
                {
                    pictureUri = item.CustomizeItem.PictureUri;
                    productName = $"Personalização {item.CustomizeItem.ProductName}";
                    description = item.CustomizeItem.Description;
                    descriptionLabel = "Descrição";
                }
                else
                {
                    pictureUri = item.ItemOrdered.PictureUri;
                    productName = item.ItemOrdered.ProductName;
                    description = $"{item.UnitPrice} €";
                    descriptionLabel = "Preço";
                }


                body += $@"
            <tr>
                <td width='250px'>
                    <img width='250' src='{pictureUri}' />
                </td>
                <td style='padding-bottom:20px;vertical-align:bottom'>
                    <table>
                        <tr>
                            <td>{productName}</td>                            
                        </tr>
                        <tr>
                            <td><strong>{descriptionLabel}:</strong><br> {description}</td>
                        </tr>";

                if (!string.IsNullOrEmpty(item.CustomizeItem.Name))
                {
                    body += $@"<tr>
                            <td><strong>Texto ou Frase:</strong><br> {item.CustomizeItem.Name}</td>
                        </tr>";
                }
                if (!string.IsNullOrEmpty(item.CustomizeItem.Colors))
                {
                    var colors = item.CustomizeItem.Colors.Split(',');
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
                body += $@"                       
                    </table>
                </td>
            </tr>";
            }

            body += $@"</table>
    </div>
    <div style='margin-top:20px;width:550px'>
        <img width='100%' src='https://www.damanojornal.com/loja/images/linha-coracao.png' />
    </div>";
            if (wantInvoice)
            {
                body += $@"<div style='background-color:#eeebeb;width:550px;padding: 5px;'>
                    <h3 style='margin-top:20px;text-align:center;width:550px'>INFORMAÇÔES DE FACTURAÇÂO</h3>
                    <div style='text-align:center;width:550px'>
                        <strong>{order.BillingToAddress.Name}</strong>";
                if (order.TaxNumber.HasValue)
                    body += $"({order.TaxNumber})";
                body += $@"
                        <br />
                        {order.BillingToAddress.Street}<br />
                        {order.BillingToAddress.PostalCode} {order.BillingToAddress.City}
                    </div>
                </div>";
            }

            body += $@"<div style='margin-top:20px;background-color:#eeebeb;width:550px;padding: 5px;'>
                    <h3 style='text-align:center'>INFORMAÇÕES DE ENVIO*</h3>
                    <div style='text-align:center;width:550px'>
                        <strong>{order.ShipToAddress.Name}</strong>";
            if (order.TaxNumber.HasValue)
                body += $"({order.TaxNumber})";

            body += $@"<br />
                        Telefone: {order.PhoneNumber}<br/>
                        {order.ShipToAddress.Street}<br />
                        {order.ShipToAddress.PostalCode} {order.ShipToAddress.City}
                    </div>
             </div>

            <div style='margin-top:20px;background-color:#eeebeb;width:550px;padding: 5px;'>
                <h3 style='text-align:center;width:550px'>MÉTODO DE ENVIO</h3>
                <div style='text-align:center;width:550px'>
                    <strong>";
            if (pickupAtStore) body += "Recolha no nosso Ponto de Referência:"; else body += "Correio Registado em mão (CTT e CTT EXPRESSO)";
            body += "</strong><br />";
            if (pickupAtStore)
                body += @"
                    Mercado Municipal de Loulé<br />
                    <a href='https://goo.gl/maps/vHLacbNAqdo' style='color: #EF7C8D;text-decoration:underline'>Ver Mapa</a>";
            body += $@"</div>
                </div>
            </div>";            
            if (!string.IsNullOrEmpty(order.Observations))
            {
                body += $@"
                <div style='margin-top:20px;background-color:#eeebeb;width:550px;padding: 5px;'>
                    <h3 style='text-align:center;width:550px'>OBSERVAÇÕES</h3>
                    <div style='text-align:center;width:550px'>
                    {order.Observations}
                    </div>
                </div>";
            }
            body += $@"<div style='margin-top:20px;background-color:#eeebeb;width:550px;padding: 5px;'>
        <h3 style='text-align:center;width:550px'>INFORMAÇÕES</h3>
        <div style='text-align:center;width:550px'>
            <p>Vamos analisar o seu pedido e entraremos em contato consigo o mais breve possível.</p>
        </div>        
    </div>
    <div style='margin-top:20px;width:550px'>
        <img width='100%' src='https://www.damanojornal.com/loja/images/linha-coracao.png' />
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
