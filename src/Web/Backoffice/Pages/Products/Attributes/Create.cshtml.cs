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
    public class CreateModel : PageModel
    {
        private readonly IRepository<CatalogItem> _repository;
        private readonly IMapper _mapper;
        private readonly IBackofficeService _service;

        public CreateModel(IRepository<CatalogItem> repository, IMapper mapper, IBackofficeService service)
        {
            _repository = repository;
            _mapper = mapper;
            this._service = service;
        }

        [BindProperty]
        public AttributeCreateModel CatalogAttributeModel { get; set; } = new AttributeCreateModel();

        public string ProductName { get; set; }

        public class AttributeCreateModel
        {
            public AttributeType Type { get; set; }
            [Display(Name = "Nome")]
            [Required]
            [StringLength(100)]
            public string Name { get; set; }
            public int Stock { get; set; } = 0;
            public int CatalogItemId { get; set; }
        }

        public async Task<IActionResult> OnGet(int id)
        {
            var prod = await _repository.GetByIdAsync(id);
            if (prod == null)
                return NotFound();

            CatalogAttributeModel.CatalogItemId = id;
            ProductName = prod.Name;
            //CatalogAttributeModel.CatalogItem = _mapper.Map<ProductViewModel>(prod);

            //ViewData["ReferenceCatalogItemId"] = new SelectList(_context.CatalogItems, "Id", "Name");
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {           
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //CatalogAttributeModel.CatalogItem = null;
            var attribute = _mapper.Map<CatalogAttribute>(CatalogAttributeModel);
            var product = await _repository.GetByIdAsync(attribute.CatalogItemId);
            product.AddAttribute(attribute);
            await _repository.UpdateAsync(product);

            //Update Total Stock
            var prod = await _repository.GetBySpecAsync(new CatalogAttrFilterSpecification(CatalogAttributeModel.CatalogItemId));
            prod.UpdateStock(prod.Attributes.Sum(x => x.Stock));
            await _repository.SaveChangesAsync();

            return RedirectToPage("/Products/Edit", new { id = CatalogAttributeModel.CatalogItemId });
        }
    }
}