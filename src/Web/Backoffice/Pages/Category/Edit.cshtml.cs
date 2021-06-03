using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper;
using Backoffice.ViewModels;
using ApplicationCore;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;

namespace Backoffice.Pages.Category
{
    public class EditModel : PageModel
    {
        private readonly IRepository<ApplicationCore.Entities.Category> _repository;
        protected readonly IMapper _mapper;

        public EditModel(IRepository<ApplicationCore.Entities.Category> repository, IMapper mapper)
        {
            _repository = repository;
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
            await PopulateListAsync();

            var category = await _repository.GetBySpecAsync(new CategorySpecification(new CategoryFilter { Id = id, IncludeParent = true }));

            if (category == null)
            {
                return NotFound();
            }

            Category = _mapper.Map<CategoryViewModel>(category);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await PopulateListAsync();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //check if name exists
            if ((await _repository.CountAsync(new CategorySpecification(new CategoryFilter { Name = Category.Name, Id = Category.Id, NotTheSameId = true }))) > 0)
            {
                ModelState.AddModelError("", $"O nome da Categoria '{Category.Name}' já existe!");
                return Page();
            }
            //Fix Slug
            Category.Slug = Utils.URLFriendly(Category.Slug);
            
            //Check if slug exists
            if ((await SlugExistsAsync(Category.Id,Category.Slug)))
            {
                ModelState.AddModelError("Category.Slug", "Já existe um slug com o mesmo nome!");
                return Page();
            }
            var cat = await _repository.GetBySpecAsync(new CategorySpecification(new CategoryFilter { Id = Category.Id, IncludeParent = true }));
            _mapper.Map(Category,cat);

            await _repository.UpdateAsync(cat);

            return RedirectToPage("./Index");
        }

        private async Task<bool> SlugExistsAsync(int id, string slug)
        {
            return (await _repository.CountAsync(new CategorySpecification(new CategoryFilter { Id = id, Slug = slug, NotTheSameId = true }))) > 0;
        }

        private async Task PopulateListAsync()
        {
            List<(string, string)> list = new List<(string, string)>
            {
                ("left", "Esquerda"),
                ("right", "Direita")
            };

            ViewData["PositionList"] = new SelectList(list.Select(x => new { Id = x.Item1, Name = x.Item2 }), "Id", "Name");
            ViewData["CategoryList"] = new SelectList(await _repository.ListAsync(), "Id", "Name");
        }
    }
}
