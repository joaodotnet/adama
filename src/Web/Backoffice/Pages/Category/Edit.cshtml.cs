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
using Backoffice.ViewModels;
using ApplicationCore;

namespace Backoffice.Pages.Category
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        protected readonly IMapper _mapper;

        public EditModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public CategoryViewModel Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {            
            if (id == null)
            {
                return NotFound();
            }
            PopulateList();

            var category = await _context.Categories.Include(x=> x.Parent).SingleOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            Category = _mapper.Map<CategoryViewModel>(category);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            PopulateList();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //check if name exists
            if (_context.Categories.Any(x => x.Name.ToUpper() == Category.Name.ToUpper() && x.Id != Category.Id))
            {
                ModelState.AddModelError("", $"O nome da Categoria '{Category.Name}' já existe!");
                return Page();
            }
            //Fix Slug
            Category.Slug = Utils.URLFriendly(Category.Slug);

            var category = _mapper.Map<ApplicationCore.Entities.Category>(Category);

            _context.Attach(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }

            return RedirectToPage("./Index");
        }
        private void PopulateList()
        {
            List<(string, string)> list = new List<(string, string)>
            {
                ("left", "Esquerda"),
                ("right", "Direita")
            };

            ViewData["PositionList"] = new SelectList(list.Select(x => new { Id = x.Item1, Name = x.Item2 }), "Id", "Name");
            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name");
        }
    }
}
