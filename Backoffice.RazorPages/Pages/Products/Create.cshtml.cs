using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using Infrastructure.Data;
using AutoMapper;
using Backoffice.RazorPages.ViewModels;

namespace Backoffice.RazorPages.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;

        public CreateModel(DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult OnGet()
        {
            ViewData["IllustrationId"] = new SelectList(_context.Illustrations, "Id", "Code");
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Code");
            return Page();
        }

        [BindProperty]
        public ProductViewModel ProductModel { get; set; }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["IllustrationId"] = new SelectList(_context.Illustrations, "Id", "Code");
                ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Code");
                return Page();
            }

            //Remove model attributes with no id
            var to_remove = ProductModel.ProductAttributes.Where(x => x.ToRemove && x.Id == 0).ToList();
            foreach (var item in to_remove)
            {
                ProductModel.ProductAttributes.Remove(item);
            }
            //Validate Model
            if (!ValidateAttributesModel())
            {
                ViewData["IllustrationId"] = new SelectList(_context.Illustrations, "Id", "Code");
                ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Code");
            }

            //Save Changes            
            _context.Products.Add(_mapper.Map<Product>(ProductModel));
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAddAttributeAsync()
        {
            ViewData["IllustrationId"] = new SelectList(_context.Illustrations, "Id", "Code");
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Code");
            ProductModel.ProductAttributes.Add(new ProductAttributeViewModel());
            return Page();
        }

        private bool ValidateAttributesModel()
        {
            foreach (var item in ProductModel.ProductAttributes)
            {
                if (!item.ToRemove)
                {
                    //Validate
                    if (string.IsNullOrEmpty(item.Code))
                        ModelState.AddModelError("", "O código do atributo é obrigatório");
                    if (string.IsNullOrEmpty(item.Name))
                        ModelState.AddModelError("", "O nome do atributo é obrigatório");
                    if (!ModelState.IsValid)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}