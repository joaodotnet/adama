using System.IO;
using System.Threading.Tasks;
using ApplicationCore.DTOs;
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
                Filename = Utils.URLFriendly(Path.GetFileNameWithoutExtension(fileData.FileName)),
                Extension = Path.GetExtension(fileData.FileName)
            };
            var filename = fileData.FileName;

            if (!string.IsNullOrEmpty(addToFilename))
            {
                var name = filename.Substring(0, filename.LastIndexOf('.')) + $"-{addToFilename}";
                filename = name + filename.Substring(filename.LastIndexOf('.'));
            }

            //High
            var fileNameHigh = filename.Replace(".", "-high.");
            var fileHighPath = fullPath + fileNameHigh;
            using (var stream = new FileStream(fileHighPath, FileMode.Create))
            {
                stream.Write(fileData.Data,0, fileData.Data.Length);
                stream.Close();
            }

            var filePath = fullPath + filename;

            //Medium
            if (resize)
            {
                using (Image image = Image.Load(fileData.Data))
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
                    stream.Write(fileData.Data, 0, fileData.Data.Length);
                    stream.Close();
                }

            }

            info.Location = filePath;
            info.PictureUri = uriPath + filename;
            info.PictureHighUri = uriPath + fileNameHigh;

            return info;
        }

        public static void DeleteFile(string fullpath)
        {
            if (File.Exists(fullpath))
                File.Delete(fullpath);
        }

        public static void DeleteFile(string fullpath, string fileName)
        {
            if (File.Exists(Path.Combine(fullpath, fileName)))
                File.Delete(Path.Combine(fullpath, fileName));
        }
    }
}
