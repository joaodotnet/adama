using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using AutoMapper;
using Backoffice.ViewModels;
using Backoffice.Interfaces;
using Microsoft.Extensions.Options;

namespace Backoffice.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;
        private readonly IBackofficeService _service;
        private readonly BackofficeSettings _backofficeSettings;

        public DeleteModel(DamaContext context, IMapper mapper, IOptions<BackofficeSettings> settings, IBackofficeService service)
        {
            _context = context;
            _mapper = mapper;
            _service = service;
            _backofficeSettings = settings.Value;
        }

        [BindProperty]
        public ProductViewModel ProductModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductModel = _mapper.Map<ProductViewModel>(await _context.CatalogItems
                .Include(p => p.CatalogIllustration)
                .Include(p => p.CatalogType)
                .SingleOrDefaultAsync(m => m.Id == id));

            if (ProductModel == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.CatalogItems
                .Include(x => x.CatalogPictures)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (product != null)
            {
                _context.CatalogItems.Remove(product);
                await _context.SaveChangesAsync();
            }

            //Delete the images of the product
            if(!string.IsNullOrEmpty(product.PictureUri))
                _service.DeleteFile(_backofficeSettings.WebProductsPictureFullPath, Utils.GetFileName(product.PictureUri));

            if (product.CatalogPictures != null)
            {
                foreach (var item in product.CatalogPictures)
                {
                    if (!string.IsNullOrEmpty(item.PictureUri))
                        _service.DeleteFile(_backofficeSettings.WebProductsPictureFullPath, Utils.GetFileName(item.PictureUri));
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
