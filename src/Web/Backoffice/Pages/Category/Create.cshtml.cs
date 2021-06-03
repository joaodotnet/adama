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
using ApplicationCore;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;

namespace Backoffice.Pages.Category
{
    public class CreateModel : PageModel
    {
        private readonly IRepository<ApplicationCore.Entities.Category> _repository;
        private readonly IMapper _mapper;

        public CreateModel(IRepository<ApplicationCore.Entities.Category> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await PopulateListAsync();
            return Page();
        }

        [BindProperty]
        public CategoryViewModel Category { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            await PopulateListAsync();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //check if name exists
            if((await _repository.CountAsync(new CategorySpecification(new CategoryFilter { Name = Category.Name}))) > 0 )
            {
                ModelState.AddModelError("", $"O nome da Categoria '{Category.Name}' já existe!");
                return Page();
            }
            //Fix Slug
            Category.Slug = Utils.URLFriendly(Category.Slug);

            //Check if slug exists
            if ((await SlugExistsAsync(Category.Slug)))
            {
                ModelState.AddModelError("Category.Slug", "Já existe um slug com o mesmo nome!");
                return Page();
            }

            await _repository.AddAsync(_mapper.Map<ApplicationCore.Entities.Category>(Category));

            return RedirectToPage("./Index");
        }

        private async Task<bool> SlugExistsAsync(string slug)
        {
            return (await _repository.CountAsync(new CategorySpecification(new CategoryFilter { Slug = slug }))) > 0;
        }

        private async Task PopulateListAsync()
        {
            ViewData["CategoryList"] = new SelectList(await _repository.ListAsync(), "Id", "Name");
        }
    }
}