﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.RazorPages.ViewModels;
using AutoMapper;

namespace Backoffice.RazorPages.Pages.ProductType
{
    public class DeleteModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        protected readonly IMapper _mapper;

        public DeleteModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public ProductTypeViewModel ProductTypeModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var type = await _context.ProductTypes
                .Include(p => p.Category).SingleOrDefaultAsync(m => m.Id == id);

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

            var type = await _context.ProductTypes.FindAsync(id);

            if (type != null)
            {
                _context.ProductTypes.Remove(type);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}