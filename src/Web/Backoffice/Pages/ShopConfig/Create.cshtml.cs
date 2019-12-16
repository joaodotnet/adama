using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.ViewModels;
using AutoMapper;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Backoffice.Extensions;
using Backoffice.Interfaces;
using Microsoft.EntityFrameworkCore;
using ApplicationCore;
using Microsoft.AspNetCore.Http;

namespace Backoffice.Pages.ShopConfig
{
    public class CreateModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;
        private readonly BackofficeSettings _backofficeSettings;
        private readonly IBackofficeService _service;

        public CreateModel(DamaContext context, IMapper mapper, IOptions<BackofficeSettings> settings, IBackofficeService service)
        {
            _context = context;
            _mapper = mapper;
            _backofficeSettings = settings.Value;
            _service = service;
        }

        public IActionResult OnGet(int? id)
        {
            var config = _context.ShopConfigs.SingleOrDefault(x => x.Id == id);
            if (config == null)
                return NotFound();
            ShopConfigDetailModel = new ShopConfigDetailViewModel { ShopConfigId = config.Id };
            return Page();
        }

        [BindProperty]
        public ShopConfigDetailViewModel ShopConfigDetailModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (IsImageSizeInvalid(ShopConfigDetailModel.Picture))
                return Page();

            if (ShopConfigDetailModel.PictureWebp?.Length > 0 && IsImageSizeInvalid(ShopConfigDetailModel.PictureWebp))
                return Page();

            var lastShopDetailId = (_context.ShopConfigDetails.Count() > 0 ? GetLastShopConfigDetailsId() : 0) + 1;
            if (ShopConfigDetailModel.Picture.Length > 0)
            {
                ShopConfigDetailModel.PictureUri = _service.SaveFile(ShopConfigDetailModel.Picture, _backofficeSettings.WebNewsPictureFullPath, _backofficeSettings.WebNewsPictureUri, (lastShopDetailId).ToString(), true, 1600).PictureUri;
                ShopConfigDetailModel.PictureMobileUri = _service.SaveFile(ShopConfigDetailModel.Picture, _backofficeSettings.WebNewsPictureFullPath, _backofficeSettings.WebNewsPictureUri, "Mobile" + (lastShopDetailId).ToString(), true, 525).PictureUri;
            }
            if (ShopConfigDetailModel.PictureWebp?.Length > 0)
                ShopConfigDetailModel.PictureWebpUri = _service.SaveFile(ShopConfigDetailModel.PictureWebp, _backofficeSettings.WebNewsPictureFullPath, _backofficeSettings.WebNewsPictureUri, (lastShopDetailId).ToString()).PictureUri;

            _context.ShopConfigDetails.Add(_mapper.Map<ShopConfigDetail>(ShopConfigDetailModel));
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private int GetLastShopConfigDetailsId()
        {
            return _context.ShopConfigDetails
                .AsEnumerable()
                .Last()
                .Id;
        }

        private bool IsImageSizeInvalid(IFormFile file)
        {
            if (file == null || file.Length == 0 || (file != null && file.Length > 4000000))
            {
                ModelState.AddModelError("", "A menina quer por favor escolher um tamanho entre 1kb e 4MB, obrigado! Ass.: O seu amor!");
                return true;
            }
            return false;
        }
    }
}
