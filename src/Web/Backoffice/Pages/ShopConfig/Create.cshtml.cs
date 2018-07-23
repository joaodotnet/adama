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

            if (ShopConfigDetailModel.Picture == null || ShopConfigDetailModel.Picture.Length == 0)
            {
                ModelState.AddModelError("", "A menina quer por favor escolher uma imagem, obrigado! Ass.: O seu amor!");
                return Page();
            }

            if (ShopConfigDetailModel.Picture.Length > 2097152)
            {
                ModelState.AddModelError("", "A menina quer por favor diminuir o tamanho do ficheiro? O máximo é 2MB, obrigado! Ass.: O seu amor!");
                return Page();
            }

            if (ShopConfigDetailModel.Picture.Length > 0)
            {
                var lastShopDetailId = _context.ShopConfigDetails.Count() > 0 ? (await _context.ShopConfigDetails.LastAsync())?.Id : 0;
                ShopConfigDetailModel.PictureUri = (await _service.SaveFileAsync(ShopConfigDetailModel.Picture, _backofficeSettings.WebNewsPictureFullPath, _backofficeSettings.WebNewsPictureUri, (++lastShopDetailId).ToString())).PictureUri;
            }

            _context.ShopConfigDetails.Add(_mapper.Map<ShopConfigDetail>(ShopConfigDetailModel));
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}