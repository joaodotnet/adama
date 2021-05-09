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
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace DamaWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IAsyncRepository<CatalogItem> _catalogRepository;
        private readonly IAsyncRepository<CatalogType> _catalogTypeRepository;
        private readonly BackofficeSettings _backofficeSettings;

        public HomeController(
            IAsyncRepository<CatalogItem> catalogRepository,
            IAsyncRepository<CatalogType> catalogTypeRepository,
            IOptions<BackofficeSettings> settings)
        {
            _catalogRepository = catalogRepository;
            _catalogTypeRepository = catalogTypeRepository;
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
                        var name = Utils.URLFriendly(Path.GetFileNameWithoutExtension(uri.LocalPath));
                        var fileName = name + Path.GetExtension(uri.LocalPath);
                        var fullPath = Path.Combine(_backofficeSettings.WebProductsPictureV2FullPath, fileName);
                        picture.PictureUri = _backofficeSettings.WebProductsPictureV2Uri + fileName;

                        //Get Image
                        var originalImage = Path.Combine(_backofficeSettings.WebProductsPictureFullPath, Path.GetFileName(uri.LocalPath));
                        using (Image image = Image.Load(originalImage))
                        {
                            image.Mutate(x => x
                                 .Resize(469, 469));

                            image.Save(fullPath); // Automatic encoder selected based on extension.
                        }
                    }
                }
                await _catalogRepository.UpdateAsync(product);

            }

            //Product Types
            var typesSpec = new CatalogTypeSpecification(true);
            var types = await _catalogTypeRepository.ListAsync(typesSpec);
            foreach (var item in types)
            {
                //Main Picture
                if (!string.IsNullOrEmpty(item.PictureUri) && !item.PictureUri.Contains("v2"))
                {
                    Uri uri = new Uri(item.PictureUri);
                    var fileName = Path.GetFileName(uri.LocalPath);
                    var originalImagePath = Path.Combine(_backofficeSettings.WebProductTypesPictureFullPath, fileName);
                    var newImagePath = Path.Combine(_backofficeSettings.WebProductTypesPictureV2FullPath, fileName);
                    using (Image image = Image.Load(originalImagePath))
                    {
                        image.Mutate(x => x
                             .Resize(255, 116));

                        image.Save(newImagePath); // Automatic encoder selected based on extension.
                    }
                    item.PictureUri = _backofficeSettings.WebProductTypesPictureV2Uri + fileName;

                    if (item.PictureTextHelpers?.Count > 0)
                    {
                        foreach (var helper in item.PictureTextHelpers)
                        {
                            Uri uriHelper = new Uri(helper.PictureUri);
                            var fileNameHelper = Path.GetFileName(uriHelper.LocalPath);
                            var originalHelperPath = Path.Combine(_backofficeSettings.WebProductTypesPictureFullPath, fileNameHelper);
                            var newHelperPath = Path.Combine(_backofficeSettings.WebProductTypesPictureV2FullPath, fileNameHelper);
                            using (Image image = Image.Load(originalHelperPath))
                            {
                                image.Mutate(x => x
                                     .Resize(112, 96));

                                image.Save(newHelperPath); // Automatic encoder selected based on extension.
                            }
                            helper.PictureUri = _backofficeSettings.WebProductTypesPictureV2Uri + fileNameHelper;
                            helper.Location = newHelperPath;
                        }
                    }
                    await _catalogTypeRepository.UpdateAsync(item);
                }
            }

            return Ok();
        }

        [Route("updatemainpicture")]
        public async Task<IActionResult> UpdateMainPicturesAsync()
        {
            var spec = new CatalogFilterSpecification(false);
            var products = await _catalogRepository.ListAsync(spec);

            //Add main Picture
            foreach (var item in products)
            {
                var mainPic = item.CatalogPictures.SingleOrDefault(x => x.IsMain);
                if (mainPic != null && !string.IsNullOrEmpty(mainPic.PictureHighUri))
                {
                    Uri uriHelper = new Uri(mainPic.PictureHighUri);
                    var fileName = Path.GetFileName(uriHelper.LocalPath);
                    var originalPath = Path.Combine(_backofficeSettings.WebProductsPictureFullPath, fileName);
                    var newFileName = Utils.URLFriendly(Path.GetFileNameWithoutExtension(uriHelper.LocalPath)) + Path.GetExtension(uriHelper.LocalPath);
                    var newPath = Path.Combine(_backofficeSettings.WebProductsPictureV2FullPath, newFileName);
                    using (Image image = Image.Load(originalPath))
                    {
                        var options = new ResizeOptions
                        {
                            Mode = ResizeMode.Crop,
                            Size = new Size(700, 700)
                        };

                        image.Mutate(x => x.Resize(options));

                        image.Save(newPath, new JpegEncoder { Quality = 90 }); // Automatic encoder selected based on extension.
                    }
                    item.UpdateMainPicture(mainPic.PictureUri = _backofficeSettings.WebProductsPictureV2Uri + newFileName);
                    await _catalogRepository.UpdateAsync(item);
                }
            }
            return Ok();
        }

        [Route("updateotherpicture")]
        public async Task<IActionResult> UpdateOtherPicturesAsync()
        {
            var spec = new CatalogFilterSpecification(false);
            var products = await _catalogRepository.ListAsync(spec);

            foreach (var item in products)
            {
                foreach (var other in item.CatalogPictures.Where(x => !x.IsMain).ToList())
                {
                    if (!string.IsNullOrEmpty(other.PictureHighUri))
                    {
                        Uri uri = new Uri(other.PictureHighUri);
                        var fileName = Path.GetFileName(uri.LocalPath);
                        var originalPath = Path.Combine(_backofficeSettings.WebProductsPictureFullPath, fileName);
                        var newFileName = Utils.URLFriendly(Path.GetFileNameWithoutExtension(uri.LocalPath)) + Path.GetExtension(uri.LocalPath);
                        var newPath = Path.Combine(_backofficeSettings.WebProductsPictureV2FullPath, newFileName);
                        using (Image image = Image.Load(originalPath))
                        {
                            var options = new ResizeOptions
                            {
                                Mode = ResizeMode.Crop,
                                Size = new Size(700, 700)
                            };

                            image.Mutate(x => x.Resize(options));

                            image.Save(newPath, new JpegEncoder { Quality = 90 }); // Automatic encoder selected based on extension.
                        }
                        other.PictureUri = _backofficeSettings.WebProductsPictureV2Uri + newFileName;
                    }
                }
                await _catalogRepository.UpdateAsync(item);

            }
            return Ok();
        }

        [Route("resizephoto")]
        public IActionResult ResizePhoto(string name, int width, int height, int? quality = 0)
        {
            var sourceFile = Path.Combine(_backofficeSettings.WebPicturesFullPath, name);
            if (System.IO.File.Exists(sourceFile))
            {
                //Backup
                var backupFolder = Path.Combine(_backofficeSettings.WebPicturesFullPath, "backup");
                if (!System.IO.Directory.Exists(backupFolder))
                {
                    System.IO.Directory.CreateDirectory(backupFolder);
                }
                var backupFile = Path.Combine(backupFolder, name);
                if (!System.IO.File.Exists(backupFile))
                {
                    System.IO.File.Copy(sourceFile, backupFile, false);
                }

                //Resize
                using (Image image = Image.Load(sourceFile))
                {
                    var options = new ResizeOptions
                    {
                        Mode = ResizeMode.Crop,
                        Size = new Size(width, height)
                    };

                    image.Mutate(x => x.Resize(options));

                    if (sourceFile.ToLower().EndsWith(".jpg"))
                        image.Save(sourceFile, new JpegEncoder { Quality = quality });
                    else
                        image.Save(sourceFile);
                }
                return Ok();
            }
            return NotFound();
        }
    }
}
