using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using AutoMapper;
using Backoffice.ViewModels;

namespace Backoffice.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;

        public EditModel(DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public ProductViewModel ProductModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductModel = _mapper.Map<ProductViewModel>(
                await _context.CatalogItems
                .Include(p => p.CatalogIllustration)
                .Include(p => p.CatalogType)
                .Include(p => p.CatalogAttributes)
                .SingleOrDefaultAsync(m => m.Id == id));

            if (ProductModel == null)
            {
                return NotFound();
            }
            await PopulateLists();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateLists();
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
                await PopulateLists();
                return Page();
            }

            //Save Changes            
            var prod = _mapper.Map<CatalogItem>(ProductModel);
            foreach (var item in prod.CatalogAttributes)
            {
                if (item.Id != 0)
                {
                    if (ProductModel.ProductAttributes.SingleOrDefault(x => x.Id == item.Id).ToRemove)
                        _context.Entry(item).State = EntityState.Deleted;
                    else
                        _context.Entry(item).State = EntityState.Modified;
                }
                else
                    _context.Entry(item).State = EntityState.Added;
            }


            _context.Attach(prod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return RedirectToPage("./Index");
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

        public async Task<IActionResult> OnPostAddAttributeAsync()
        {
            await PopulateLists();
            ProductModel.ProductAttributes.Add(new ProductAttributeViewModel
            {
                ProductId = ProductModel.Id
            });
            return Page();
        }

        private async Task PopulateLists()
        {
            var illustrations = await _context.CatalogIllustrations
                .Include(x => x.IllustrationType)
                .Select(s => new { Id = s.Id, Name = $"{s.IllustrationType.Code} - {s.Code}" })
                .OrderBy(x => x.Name)
                .ToListAsync();
            ViewData["IllustrationId"] = new SelectList(illustrations, "Id", "Name");
            ViewData["ProductTypeId"] = new SelectList(_context.CatalogTypes, "Id", "Code");
        }

        //public async Task<IActionResult> OnPostRemoveAttributeAsync(int id)
        //{
        //    ViewData["IllustrationId"] = new SelectList(_context.Illustrations, "Id", "Code");
        //    ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Code");
        //    if (id != 0)
        //    {
        //        //var attributeModel = ProductModel.ProductAttributes.SingleOrDefault(x => x.Id == id);
        //        //if (attributeModel != null)
        //        //    ProductModel.ProductAttributes.Remove(attributeModel);

        //        var attribute = await _context.ProductAttributes.FindAsync(id);
        //        var prodId = attribute.ProductId;
        //        if (attribute != null)
        //        {
        //            _context.ProductAttributes.Remove(attribute);
        //            await _context.SaveChangesAsync();
        //        }
        //        //var prod = await _context.Products
        //        //    .Include(x => x.ProductAttributes)
        //        //    .SingleOrDefaultAsync(x => x.Id == prodId);

        //        //ProductModel.ProductAttributes = _mapper.Map<List<ProductAttributeViewModel>>(prod.ProductAttributes);
        //    }            

        //    return Page();
        //}
    }
}
