using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Entities;
using AutoMapper;
using Backoffice.ViewModels;
using ApplicationCore.Interfaces;

namespace Backoffice.Pages.IllustrationTypes
{
    public class DeleteModel : PageModel
    {
        private readonly IRepository<IllustrationType> _repository;
        private readonly IMapper _mapper;

        public DeleteModel(IRepository<IllustrationType> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [BindProperty]
        public IllustrationTypeViewModel IllustrationType { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IllustrationType = _mapper.Map<IllustrationTypeViewModel>(await _repository.GetByIdAsync(id.Value));

            if (IllustrationType == null)
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

            var illustrationType = await _repository.GetByIdAsync(id);

            if (illustrationType != null)
            {
                await _repository.DeleteAsync(illustrationType);
            }

            return RedirectToPage("./Index");
        }
    }
}
