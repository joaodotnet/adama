using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Entities;
using AutoMapper;
using Backoffice.ViewModels;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;

namespace Backoffice.Pages.IllustrationTypes
{
    public class EditModel : PageModel
    {
        private readonly IRepository<IllustrationType> _repository;
        private readonly IMapper _mapper;

        public EditModel(IRepository<IllustrationType> repository, IMapper mapper)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //check if code exists
            if ((await _repository.CountAsync(new IllustrationTypeSpecification(IllustrationType.Code, IllustrationType.Id)) > 0))
            {
                ModelState.AddModelError("", $"O código do Tipo de Ilustração '{IllustrationType.Code}' já existe!");
                return Page();
            }

            var entity = await _repository.GetByIdAsync(IllustrationType.Id);
            _mapper.Map(IllustrationType, entity);
            await _repository.UpdateAsync(entity);

            return RedirectToPage("./Index");
        }
    }
}
