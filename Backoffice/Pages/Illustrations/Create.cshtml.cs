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

        public IActionResult OnGet()
        {
            ViewData["IllustrationTypes"] = new SelectList(_context.IllustrationTypes, "Id", "Code");
            return Page();
        }

        [BindProperty]
        public IllustrationViewModel IllustrationModel { get; set; }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["IllustrationTypes"] = new SelectList(_context.IllustrationTypes, "Id", "Code");
                return Page();
            }

            var illustrationDB = _mapper.Map<Illustration>(IllustrationModel);

            if (IllustrationModel.IllustrationImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await IllustrationModel.IllustrationImage.CopyToAsync(memoryStream);
                    // validate file, then move to CDN or public folder
                    illustrationDB.Image = memoryStream.ToArray();
                }
            }

            _context.Illustrations.Add(illustrationDB);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostRefreshTypesAsync()
        {
            ViewData["IllustrationTypes"] = new SelectList(_context.IllustrationTypes, "Id", "Code");
            return Page();
        }
    }
}