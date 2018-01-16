using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.ViewModels;
using AutoMapper;

namespace Backoffice.Pages.Category
{
    public class CreateModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        private readonly IMapper _mapper;

        public CreateModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult OnGet()
        {
            PopulateList();
            return Page();
        }

        [BindProperty]
        public CategoryViewModel Category { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            PopulateList();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //check if name exists
            if(_context.Categories.Any(x => x.Name.ToUpper() == Category.Name.ToUpper()))
            {
                ModelState.AddModelError("", $"O nome da Categoria '{Category.Name}' já existe!");
                return Page();
            }
            _context.Categories.Add(_mapper.Map<ApplicationCore.Entities.Category>(Category));
            await _context.SaveChangesAsync();

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