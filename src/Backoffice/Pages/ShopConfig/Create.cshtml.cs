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

namespace Backoffice.Pages.ShopConfig
{
    public class CreateModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;
        private readonly BackofficeSettings _backofficeSettings;

        public CreateModel(DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

           

            var shopConfigAdded = _context.ShopConfigDetails.Add(_mapper.Map<ShopConfigDetail>(ShopConfigDetailModel));
            await _context.SaveChangesAsync();

            if (ShopConfigDetailModel.Picture.Length > 0)
            {
                var type = ContentDispositionHeaderValue
                        .Parse(ShopConfigDetailModel.Picture.ContentDisposition)
                        .DispositionType;

                //Check if is jpg on png                
                var filename = $"news_banner_{shopConfigAdded.Entity.Id}.jpg";
                shopConfigAdded.Entity.PictureUri = _backofficeSettings.WebNewsPictureUri + filename;

                var filePath = Path.Combine(
                    _backofficeSettings.WebNewsPictureFullPath,
                    filename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ShopConfigDetailModel.Picture.CopyToAsync(stream);
                }
                //using (FileStream fs = System.IO.File.Create(filePath))
                //{
                //    ShopConfigDetailModel.Picture.CopyTo(fs);
                //    fs.Flush();
                //}
                await _context.SaveChangesAsync();
            }

            //using (var memoryStream = new MemoryStream())
            //{
            //    await ShopConfigDetailModel.Picture.CopyToAsync(memoryStream);
            //    // validate file, then move to CDN or public folder

            //}

            

            return RedirectToPage("./Index");
        }
    }
}