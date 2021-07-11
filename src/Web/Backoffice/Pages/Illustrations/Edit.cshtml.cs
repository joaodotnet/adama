using System;
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
    public class EditModel : PageModel
    {
        private readonly IRepository<CatalogIllustration> _repository;
        private readonly IRepository<IllustrationType> _illustrationTypeRepo;
        private readonly IMapper _mapper;

        public EditModel(IRepository<CatalogIllustration> repository,
            IRepository<IllustrationType> illustrationTypeRepo,
            IMapper mapper)
        {
            _repository = repository;
            _illustrationTypeRepo = illustrationTypeRepo;
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
            await PopulateListAsync();

            var illustrationDb = await _repository.GetBySpecAsync(new CatalogIllustrationSpecification(id.Value));
            IllustrationModel = _mapper.Map<IllustrationViewModel>(illustrationDb);

            if (illustrationDb.Image != null)
            {
                IllustrationModel.ImageBase64 = "data:image/png;base64," + Convert.ToBase64String(illustrationDb.Image, 0, illustrationDb.Image.Length);
            }

            if (IllustrationModel == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            await PopulateListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            //check if code exists
            if ((await _repository.CountAsync(new CatalogIllustrationSpecification(IllustrationModel.Code, IllustrationModel.Id)) > 0))
            {
                ModelState.AddModelError("", $"O código da Ilustração '{IllustrationModel.Code}' já existe!");
                return Page();
            }

            //var illustrationEntity = _mapper.Map<Illustration>(IllustrationModel);
            var illustrationEntity = await _repository.GetBySpecAsync(new CatalogIllustrationSpecification(IllustrationModel.Id));

            if(illustrationEntity == null)
            {
                ModelState.AddModelError("", "Ilustração não encontrada!");
                return Page();
            }
            illustrationEntity.UpdateData(IllustrationModel.Code,IllustrationModel.Name,IllustrationModel.IllustrationTypeId, IllustrationModel.InMenu);

            if (IllustrationModel.IllustrationImage?.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await IllustrationModel.IllustrationImage.CopyToAsync(memoryStream);
                    // validate file, then move to CDN or public folder
                    illustrationEntity.UpdateImage(memoryStream.ToArray());
                }
            }

            await _repository.UpdateAsync(illustrationEntity);

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
