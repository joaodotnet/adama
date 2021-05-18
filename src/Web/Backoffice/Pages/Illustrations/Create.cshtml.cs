using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using Infrastructure.Data;
using AutoMapper;
using Backoffice.Extensions;
using Backoffice.ViewModels;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Pages.Illustrations
{
    public class CreateModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;

        public CreateModel(DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> OnGet()
        {
            await PopulateListAsync();
            return Page();
        }

        [BindProperty]
        public IllustrationViewModel IllustrationModel { get; set; }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateListAsync();
                return Page();
            }

            //check if code exists
            if (_context.CatalogIllustrations.Any(x => x.Code.ToUpper() == IllustrationModel.Code.ToUpper()))
            {
                await PopulateListAsync();
                ModelState.AddModelError("", $"O código da Ilustração '{IllustrationModel.Code}' já existe!");
                return Page();
            }

            var illustrationDB = _mapper.Map<CatalogIllustration>(IllustrationModel);

            if (IllustrationModel.IllustrationImage?.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await IllustrationModel.IllustrationImage.CopyToAsync(memoryStream);
                    // validate file, then move to CDN or public folder
                    illustrationDB.UpdateImage(memoryStream.ToArray());
                }
            }

            _context.CatalogIllustrations.Add(illustrationDB);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostRefreshTypesAsync()
        {
            await PopulateListAsync();
            return Page();
        }

        private async Task PopulateListAsync()
        {
            var list = await _context.IllustrationTypes
                .Select(x => new { Id = x.Id, Name = $"{x.Code} - {x.Name}" })
                .ToListAsync();
            ViewData["IllustrationTypes"] = new SelectList(list, "Id", "Name");
        }
    }
}