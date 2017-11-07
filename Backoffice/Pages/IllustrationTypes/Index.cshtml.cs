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

namespace Backoffice.Pages.IllustrationTypes
{
    public class IndexModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;

        public IndexModel(DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IList<IllustrationTypeViewModel> IllustrationType { get;set; }

        public async Task OnGetAsync()
        {
            IllustrationType = _mapper.Map<List<IllustrationTypeViewModel>>(await _context.IllustrationTypes.ToListAsync());
        }
    }
}
