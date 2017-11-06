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
using Backoffice.RazorPages.ViewModels;
using System.IO;

namespace Backoffice.RazorPages.Pages.Illustrations
{
    public class DetailsModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        private readonly IMapper _mapper;

        public DetailsModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IllustrationViewModel IllustrationModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var illustrationDb = await _context.Illustrations.Include(x => x.IllustrationType).SingleOrDefaultAsync(m => m.Id == id);
            IllustrationModel = _mapper.Map<IllustrationViewModel>(illustrationDb);
            if(illustrationDb.Image != null)
            {
                IllustrationModel.ImageBase64 = "data:image/png;base64," + Convert.ToBase64String(illustrationDb.Image, 0, illustrationDb.Image.Length);
            }
            

            if (IllustrationModel == null)
            {
                return NotFound();
            }
            return Page();
        }       
    }
}
