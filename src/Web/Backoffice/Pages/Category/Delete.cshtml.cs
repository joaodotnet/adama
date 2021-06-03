using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Backoffice.ViewModels;
using AutoMapper;
using ApplicationCore.Interfaces;

namespace Backoffice.Pages.Category
{
    public class DeleteModel : PageModel
    {
        private readonly IRepository<ApplicationCore.Entities.Category> _repository;
        protected readonly IMapper _mapper;

        public DeleteModel(IRepository<ApplicationCore.Entities.Category> repository, IMapper mapper)
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

            var catInDB = await _repository.GetByIdAsync(id);

            if (catInDB == null)
            {
                return NotFound();
            }

            Category = _mapper.Map<CategoryViewModel>(catInDB);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catinDb = await _repository.GetByIdAsync(id);

            if (catinDb != null)
            {
                await _repository.DeleteAsync(catinDb);
            }

            return RedirectToPage("./Index");
        }
    }
}
