using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using DamaWeb.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DamaWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly IAsyncRepository<CatalogItem> _catalogRepository;

        public HomeController(ICatalogService catalogService,
            IAsyncRepository<CatalogItem> catalogRepository)
        {
            _catalogService = catalogService;
            _catalogRepository = catalogRepository;
        }
        [Route("/sitemap.xml")]
        public async Task<IActionResult> SitemapXml()
        {
            StringBuilder builder = new StringBuilder();

            using (var xml = XmlWriter.Create(builder, new XmlWriterSettings { Indent = true }))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                //Home page
                AddXmlElement(xml, "");

                //Home page /Loja
                AddXmlElement(xml, "/loja");

                var categories = await _catalogService.GetMenuViewModel();
                foreach (var item in categories)
                {
                    //Categories
                    var url = Url.Page("/Category/Index", new { id = item.NameUri });
                    AddXmlElement(xml, url);

                    foreach (var child in item.Childs)
                    {
                        if (!string.IsNullOrEmpty(child.TypeUri))
                        {
                            //Prouct Types
                            url = Url.Page("/Category/Type/Index", new { cat = child.NameUri, type = child.TypeUri });
                            AddXmlElement(xml, url);
                        }
                        else
                        {
                            //SubCategories
                            url = Url.Page("/Category/Index", new { id = child.NameUri });
                            AddXmlElement(xml, url);
                        }
                    }
                }

                var products = await _catalogService.GetCatalogItems(0, null, null, null, null);
                foreach (var item in products.CatalogItems)
                {
                    var url = Url.Page("/Product", new { id = item.ProductSlug });
                    AddXmlElement(xml, Uri.EscapeUriString(url));
                }

                var urlCustomize = Url.Page("/Customize/Index");
                AddXmlElement(xml, urlCustomize);

                xml.WriteEndElement();
                xml.WriteEndDocument();
                //xmlString = sw.ToString();
            }

            return new ContentResult
            {
                ContentType = "application/xml",
                Content = builder.ToString(),
                StatusCode = 200
            };
            //byte[] data = Encoding.UTF8.GetBytes(xmlString);
            //Response.ContentType = "application/xml";
            //await Response.Body.WriteAsync(data, 0, data.Length);
        }

        private void AddXmlElement(XmlWriter xml, string url)
        {
            string host = Request.Scheme + "://" + Request.Host;

            xml.WriteStartElement("url");
            xml.WriteElementString("loc", host + url);
            xml.WriteElementString("lastmod", "2019-04-07");
            xml.WriteEndElement();
        }
    }
}
