using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Entities;
using AutoMapper;
using Backoffice.ViewModels;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;

namespace Backoffice.Pages.Illustrations
{
    public class DetailsModel : PageModel
    {
        private readonly IRepository<CatalogIllustration> _repository;
        private readonly IMapper _mapper;

        public DetailsModel(IRepository<CatalogIllustration> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IllustrationViewModel IllustrationModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var illustrationDb = await _repository.GetBySpecAsync(new CatalogIllustrationSpecification(id.Value));
            IllustrationModel = _mapper.Map<IllustrationViewModel>(illustrationDb);
            if(illustrationDb.Image != null)
            {
                IllustrationModel.ImageBase64 = "data:image/png;base64," + Convert.ToBase64String(illustrationDb.Image, 0, illustrationDb.Image.Length);
            }
            

            if (IllustrationModel == null)
            {
                return NotFound();
            }
            return Page();
        }       
    }
}
