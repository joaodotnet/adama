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

namespace Backoffice.Pages.Illustrations
{
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        private readonly IMapper _mapper;

        public IndexModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IList<IllustrationViewModel> IllustrationModel { get;set; }

        public async Task OnGetAsync()
        {
            IllustrationModel = _mapper.Map<List<IllustrationViewModel>>(
                await _context.Illustrations
                .Include(x => x.IllustrationType)
                .OrderBy(x => x.Code)
                .ToListAsync());
        }
    }
}
