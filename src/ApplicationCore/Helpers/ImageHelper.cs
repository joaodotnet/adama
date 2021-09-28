using System.IO;
using ApplicationCore.DTOs;
using ApplicationCore.Extensions;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ApplicationCore.Helpers
{
    public static class ImageHelper
    {
        public static PictureInfo SaveFile(FileData fileData, string fullPath, string uriPath, string addToFilename, bool resize = false, int width = 0, int height = 0)
        {
            var info = new PictureInfo
            {
                Filename = formFile.GetFileNameSimplify(),
                Extension = formFile.GetExtension()
            };
            var filename = formFile.GetFileName();

            if (!string.IsNullOrEmpty(addToFilename))
            {
                var name = filename.Substring(0, filename.LastIndexOf('.')) + $"-{addToFilename}";
                filename = name + filename.Substring(filename.LastIndexOf('.'));
            }

            //High
            var fileNameHigh = filename.Replace(".", "-high.");
            var fileHighPath = Path.Combine(
                fullPath,
                fileNameHigh);
            using (var stream = new FileStream(fileHighPath, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }

            var filePath = Path.Combine(
                fullPath,
                filename);

            //Medium
            if (resize)
            {
                using (Image image = Image.Load(formFile.OpenReadStream()))
                {
                    image.Mutate(x => x
                         .Resize(width, height));

                    image.Save(filePath); // Automatic encoder selected based on extension.

                    var options = new ResizeOptions
                    {
                        Mode = ResizeMode.Crop,
                        Size = new Size(width, height)
                    };                    

                    image.Mutate(x => x.Resize(options));      

                    image.Save(filePath, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = 90 }); // Automatic encoder selected based on extension.
                }
            }
            else
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }

            }

            info.Location = filePath;
            info.PictureUri = uriPath + filename;
            info.PictureHighUri = uriPath + fileNameHigh;

            return info;
        }
    }
}
