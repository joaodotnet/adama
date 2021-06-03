using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using AutoMapper;
using Backoffice.ViewModels;
using System.IO;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;

namespace Backoffice.Pages.Illustrations
{
    public class CreateModel : PageModel
    {
        private readonly IRepository<CatalogIllustration> _repository;
        private readonly IRepository<IllustrationType> _illustrationTypeRepo;
        private readonly IMapper _mapper;

        public CreateModel(IRepository<CatalogIllustration> repository,
            IRepository<IllustrationType> illustrationTypeRepo,
            IMapper mapper)
        {
            _repository = repository;
            _illustrationTypeRepo = illustrationTypeRepo;
            _mapper = mapper;
        }

        public async Task<IActionResult> OnGet()
        {
            await PopulateListAsync();
            return Page();
        }

        [BindProperty]
        public IllustrationViewModel IllustrationModel { get; set; }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateListAsync();
                return Page();
            }

            //check if code exists
            if ((await _repository.CountAsync(new CatalogIllustrationSpecification(IllustrationModel.Code)) > 0))
            {
                await PopulateListAsync();
                ModelState.AddModelError("", $"O código da Ilustração '{IllustrationModel.Code}' já existe!");
                return Page();
            }

            var illustrationDB = _mapper.Map<CatalogIllustration>(IllustrationModel);

            if (IllustrationModel.IllustrationImage?.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await IllustrationModel.IllustrationImage.CopyToAsync(memoryStream);
                    // validate file, then move to CDN or public folder
                    illustrationDB.UpdateImage(memoryStream.ToArray());
                }
            }

            await _repository.AddAsync(illustrationDB);

            await _repository.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostRefreshTypesAsync()
        {
            await PopulateListAsync();
            return Page();
        }

        private async Task PopulateListAsync()
        {
            var list = (await _illustrationTypeRepo.ListAsync())
                .Select(x => new { Id = x.Id, Name = $"{x.Code} - {x.Name}" })
                .ToList();
            ViewData["IllustrationTypes"] = new SelectList(list, "Id", "Name");
        }
    }
}