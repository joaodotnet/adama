using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ApplicationCore.DTOs;
using DamaAdmin.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DamaAdmin.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileSaveController : ControllerBase
    {
        private readonly ILogger<FileSaveController> _logger;
        private readonly IWebHostEnvironment _env;

        public FileSaveController(ILogger<FileSaveController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }
        [HttpPost]
        public async Task<ActionResult<IList<FileData>>> PostFile(
        [FromForm] IEnumerable<IFormFile> files)
        {
            var maxAllowedFiles = 10;
            long maxFileSize = 1024 * 2000;
            var filesProcessed = 0;
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            List<FileData> uploadResults = new();

            foreach (var file in files)
            {
                var uploadResult = new FileData();
                string trustedFileNameForFileStorage;
                var untrustedFileName = file.FileName;
                uploadResult.FileName = untrustedFileName;
                var trustedFileNameForDisplay =
                    WebUtility.HtmlEncode(untrustedFileName);

                if (filesProcessed < maxAllowedFiles)
                {
                    if (file.Length == 0)
                    {
                        _logger.LogInformation("{FileName} length is 0 (Err: 1)",
                            trustedFileNameForDisplay);
                        uploadResult.ErrorCode = 1;
                    }
                    else if (file.Length > maxFileSize)
                    {
                        _logger.LogInformation("{FileName} of {Length} bytes is " +
                            "larger than the limit of {Limit} bytes (Err: 2)",
                            trustedFileNameForDisplay, file.Length, maxFileSize);
                        uploadResult.ErrorCode = 2;
                    }
                    else
                    {
                        try
                        {
                            trustedFileNameForFileStorage = Path.GetRandomFileName();
                            var path = Path.Combine(_env.ContentRootPath, "unsafe_uploads",
                                trustedFileNameForFileStorage);

                            await using FileStream fs = new(path, FileMode.Create);
                            await file.CopyToAsync(fs);

                            _logger.LogInformation("{FileName} saved at {Path}",
                                trustedFileNameForDisplay, path);
                            uploadResult.Uploaded = true;
                            uploadResult.StoredFileName = path;
                        }
                        catch (IOException ex)
                        {
                            _logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                                trustedFileNameForDisplay, ex.Message);
                            uploadResult.ErrorCode = 3;
                        }
                    }

                    filesProcessed++;
                }
                else
                {
                    _logger.LogInformation("{FileName} not uploaded because the " +
                        "request exceeded the allowed {Count} of files (Err: 4)",
                        trustedFileNameForDisplay, maxAllowedFiles);
                    uploadResult.ErrorCode = 4;
                }

                uploadResults.Add(uploadResult);
            }

            return new CreatedResult(resourcePath, uploadResults);
        }
    }
}
