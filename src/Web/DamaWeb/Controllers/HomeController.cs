using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
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

        public HomeController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }
        [Route("/sitemap.xml")]
        public async Task SitemapXml()
        {
            Response.ContentType = "application/xml";

            using (var xml = XmlWriter.Create(Response.Body, new XmlWriterSettings { Indent = true }))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                //Home page
                AddXmlElement(xml,"");

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
                    var url = Url.Page("/Product/Index", new { id = item.ProductSlug });
                    AddXmlElement(xml, url);
                }

                var urlCustomize = Url.Page("/Customize/Index");
                AddXmlElement(xml, urlCustomize);

                xml.WriteEndElement();
            }
        }

        private void AddXmlElement(XmlWriter xml, string url)
        {
            string host = Request.Scheme + "://" + Request.Host;

            xml.WriteStartElement("url");
            xml.WriteElementString("loc", host+url);
            xml.WriteElementString("lastmod", "2019-04-06");
            xml.WriteEndElement();
        }
    }
}
