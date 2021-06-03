using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoMapper;
using Backoffice.ViewModels;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities;
using ApplicationCore.Specifications;

namespace Backoffice.Pages.Illustrations
{
    public class DeleteModel : PageModel
    {
        private readonly IRepository<CatalogIllustration> _repository;
        private readonly IMapper _mapper;

        public DeleteModel(IRepository<CatalogIllustration> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [BindProperty]
        public IllustrationViewModel IllustrationModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var illustration = await _repository.GetBySpecAsync(new CatalogIllustrationSpecification(id.Value));
            IllustrationModel = _mapper.Map<IllustrationViewModel>(illustration);

            if (IllustrationModel == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var illustration = await _repository.GetByIdAsync(id);

            if (illustration != null)
            {
                await _repository.DeleteAsync(illustration);
            }

            return RedirectToPage("./Index");
        }
    }
}
