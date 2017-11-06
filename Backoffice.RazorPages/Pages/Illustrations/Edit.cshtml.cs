using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using AutoMapper;
using Backoffice.RazorPages.ViewModels;
using System.IO;

namespace Backoffice.RazorPages.Pages.Illustrations
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        private readonly IMapper _mapper;

        public EditModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public IllustrationViewModel IllustrationModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["IllustrationTypes"] = new SelectList(_context.IllustrationTypes, "Id", "Code");

            var illustrationDb = await _context.Illustrations.Include(x => x.IllustrationType).SingleOrDefaultAsync(m => m.Id == id);
            IllustrationModel = _mapper.Map<IllustrationViewModel>(illustrationDb);

            if (illustrationDb.Image != null)
            {
                IllustrationModel.ImageBase64 = "data:image/png;base64," + Convert.ToBase64String(illustrationDb.Image, 0, illustrationDb.Image.Length);
            }

            if (IllustrationModel == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var illustrationEntity = _mapper.Map<Illustration>(IllustrationModel);

            if (IllustrationModel.IllustrationImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await IllustrationModel.IllustrationImage.CopyToAsync(memoryStream);
                    // validate file, then move to CDN or public folder
                    illustrationEntity.Image = memoryStream.ToArray();
                }
            }


            _context.Attach(illustrationEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }

            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostRefreshTypesAsync()
        {
            ViewData["IllustrationTypes"] = new SelectList(_context.IllustrationTypes, "Id", "Code");
            return Page();
        }
    }
}
