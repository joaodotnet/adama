using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Entities;
using AutoMapper;
using Backoffice.Interfaces;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;

namespace Backoffice.Pages.Products.Attributes
{
    public class DeleteModel : PageModel
    {
        private readonly IRepository<CatalogItem> _repository;
        private readonly IMapper _mapper;
        private readonly IBackofficeService _service;

        public DeleteModel(IRepository<CatalogItem> repository, IMapper mapper, IBackofficeService service)
        {
            _repository = repository;
            _mapper = mapper;
            _service = service;
        }

        [BindProperty]
        public AttributeViewModel CatalogAttributeModel { get; set; }

        public string ProductName { get; set; }

        public class AttributeViewModel
        {
            public int Id { get; set; }
            public AttributeType Type { get; set; }
            [Display(Name = "Nome")]
            [Required]
            [StringLength(100)]
            public string Name { get; set; }
            public int Stock { get; set; }
            public int CatalogItemId { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? catalogItemId, int? id)
        {
            if (id == null || catalogItemId == null)
            {
                return NotFound();
            }

            var prod = await _repository.GetBySpecAsync(new CatalogAttrFilterSpecification(catalogItemId.Value));

            if(prod == null)
                return NotFound();
            
            var ca = prod.Attributes.SingleOrDefault(x => x.Id == id);

            if (ca == null)
            {
                return NotFound();
            }

            CatalogAttributeModel = _mapper.Map<AttributeViewModel>(ca);
            ProductName = ca.Name;    
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? catalogItemId, int? id)
        {
            if (id == null || catalogItemId == null)
            {
                return NotFound();
            }

            var prod = await _repository.GetBySpecAsync(new CatalogAttrFilterSpecification(catalogItemId.Value));

            if(prod == null)
                return NotFound();
            
            var ca = prod.Attributes.SingleOrDefault(x => x.Id == id);
            if(ca != null)
            {
                prod.RemoveAttribute(ca);
                await _repository.UpdateAsync(prod);
            }

            return RedirectToPage("/Products/Edit", new { id = ca.CatalogItemId });
        }
    }
}
