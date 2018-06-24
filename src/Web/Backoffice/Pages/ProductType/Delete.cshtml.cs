using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.ViewModels;
using AutoMapper;
using Backoffice.Interfaces;
using Microsoft.Extensions.Options;
using ApplicationCore;

namespace Backoffice.Pages.ProductType
{
    public class DeleteModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        protected readonly IMapper _mapper;
        private readonly IBackofficeService _service;
        private readonly BackofficeSettings _backofficeSettings;

        public DeleteModel(DamaContext context, IMapper mapper, IBackofficeService service, IOptions<BackofficeSettings> backofficeSettings)
        {
            _context = context;
            _mapper = mapper;
            _service = service;
            _backofficeSettings = backofficeSettings.Value;
        }

        [BindProperty]
        public ProductTypeViewModel ProductTypeModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var type = await _context.CatalogTypes
                .Include(p => p.Categories)
                .ThenInclude(c => c.Category)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (type == null)
            {
                return NotFound();
            }
            ProductTypeModel = _mapper.Map<ProductTypeViewModel>(type);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var type = await _context.CatalogTypes.FindAsync(id);

            if (type != null)
            {
                if (!string.IsNullOrEmpty(type.PictureUri))
                {
                    _service.DeleteFile(_backofficeSettings.WebProductTypesPictureFullPath, Utils.GetFileName(type.PictureUri));
                }

                _context.CatalogTypes.Remove(type);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
