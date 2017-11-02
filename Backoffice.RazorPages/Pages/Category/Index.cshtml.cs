﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Backoffice.RazorPages.ViewModels;
using AutoMapper;

namespace Backoffice.RazorPages.Pages.Category
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

        public IList<CategoryViewModel> Categories { get;set; }

        public async Task OnGetAsync()
        {
            var cats = await _context.Categories
                .Include(x => x.ProductTypes)
                .ToListAsync();
            Categories = _mapper.Map<List<CategoryViewModel>>(cats);            
        }
    }
}