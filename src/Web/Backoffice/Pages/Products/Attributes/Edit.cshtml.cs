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
    public class EditModel : PageModel
    {
        private readonly IRepository<CatalogItem> _repository;
        private readonly IMapper _mapper;
        private readonly IBackofficeService _service;

        public EditModel(IRepository<CatalogItem> repository, IMapper mapper, IBackofficeService service)
        {
            _repository = repository;
            _mapper = mapper;
            _service = service;
        }

        [BindProperty]
        public AttributeEditModel CatalogAttributeModel { get; set; }

        public string ProductName { get; set; }

        public class AttributeEditModel
        {
            public int Id { get; set; }
            public AttributeType Type { get; set; }
            [Display(Name = "Nome")]
            [Required]
            [StringLength(100)]
            public string Name { get; set; }
            public int Stock { get; set; } = 0;
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
            CatalogAttributeModel = _mapper.Map<AttributeEditModel>(ca);
            ProductName = ca.CatalogItem.Name;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var prod = await _repository.GetBySpecAsync(new CatalogAttrFilterSpecification(CatalogAttributeModel.CatalogItemId));

            prod.UpdateAttribute(CatalogAttributeModel.Id, CatalogAttributeModel.Type, CatalogAttributeModel.Name, CatalogAttributeModel.Stock);

            //Update Total Stock
            prod.UpdateStock(prod.Attributes.Sum(x => x.Stock));

            await _repository.UpdateAsync(prod);

            return RedirectToPage("/Products/Edit", new { id = CatalogAttributeModel.CatalogItemId });
        }
    }
}
