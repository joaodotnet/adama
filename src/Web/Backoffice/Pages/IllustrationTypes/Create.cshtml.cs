using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Entities;
using Backoffice.ViewModels;
using AutoMapper;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;

namespace Backoffice.Pages.IllustrationTypes
{
    public class CreateModel : PageModel
    {
        private readonly IRepository<IllustrationType> _repository;
        private readonly IMapper _mapper;

        public CreateModel(IRepository<IllustrationType> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public IllustrationTypeViewModel IllustrationType { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //check if code exists
            if ((await _repository.CountAsync(new IllustrationTypeSpecification(IllustrationType.Code.ToUpper()))) > 0)
            {
                ModelState.AddModelError("", $"O código do Tipo de Ilustração '{IllustrationType.Code}' já existe!");
                return Page();
            }

            await _repository.AddAsync(_mapper.Map<IllustrationType>(IllustrationType));

            return RedirectToPage("./Index");
        }
    }
}