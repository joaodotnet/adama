using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using ApplicationCore;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace DamaWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IAsyncRepository<CatalogItem> _catalogRepository;
        private readonly BackofficeSettings _backofficeSettings;

        public HomeController(
            IAsyncRepository<CatalogItem> catalogRepository,
            IOptions<BackofficeSettings> settings)
        {
            _catalogRepository = catalogRepository;
            _backofficeSettings = settings.Value;
        }
        [Route("updatepictures")]
        public async Task<IActionResult> UpdatePicturesAsync()
        {
            var spec = new CatalogFilterSpecification(false);
            var products = await _catalogRepository.ListAsync(spec);

            //Add main Picture
            foreach (var item in products)
            {
                if (!string.IsNullOrEmpty(item.PictureUri) && !item.CatalogPictures.Any(x => x.IsMain))
                {
                    //item.CatalogPictures.ToList().ForEach(x => x.Order += 1);
                    item.CatalogPictures.Add(new CatalogPicture
                    {
                        IsActive = true,
                        IsMain = true,
                        PictureUri = item.PictureUri,
                        Order = 0
                    });
                    await _catalogRepository.UpdateAsync(item);
                }
            }
            //Set current Picture to PictureHighUri
            products = await _catalogRepository.ListAsync(spec);
            foreach (var product in products)
            {
                foreach (var picture in product.CatalogPictures)
                {
                    if (!string.IsNullOrEmpty(picture.PictureUri) && string.IsNullOrEmpty(picture.PictureHighUri))
                    {
                        //Set pictureHighUri
                        picture.PictureHighUri = picture.PictureUri;
                        Uri uri = new Uri(picture.PictureUri);
                        var path = Path.Combine(_backofficeSettings.WebProductsPictureFullPath, "v2");
                        var name = Utils.URLFriendly(Path.GetFileNameWithoutExtension(uri.LocalPath));
                        var fileName = name + Path.GetExtension(uri.LocalPath);
                        var fullPath = Path.Combine(path, fileName);
                        picture.PictureUri = _backofficeSettings.WebProductsPictureUri + "v2/" + fileName;

                        //Get Image
                        var originalImage = Path.Combine(_backofficeSettings.WebProductsPictureFullPath, Path.GetFileName(uri.LocalPath));
                        using (Image<Rgba32> image = Image.Load(originalImage))
                        {
                            image.Mutate(x => x
                                 .Resize(469, 469));

                            image.Save(fullPath); // Automatic encoder selected based on extension.
                        }
                    }
                }
                await _catalogRepository.UpdateAsync(product);
                
            }
            return Ok();
        }
    }
}
