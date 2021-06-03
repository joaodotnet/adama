using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.ViewModels;
using AutoMapper;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;

namespace Backoffice.Pages.Category
{
    public class DetailsModel : PageModel
    {
        private readonly IRepository<ApplicationCore.Entities.Category> _categoryRepository;
        private readonly IRepository<CatalogTypeCategory> _typeRepository;
        protected readonly IMapper _mapper;

        public DetailsModel(IRepository<ApplicationCore.Entities.Category> categoryRepository, IRepository<ApplicationCore.Entities.CatalogTypeCategory> typeRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _typeRepository = typeRepository;
            _mapper = mapper;
        }

        public CategoryViewModel Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryRepository.GetBySpecAsync(new CategorySpecification(new CategoryFilter { Id = id.Value, IncludeParent = true }));

            if (category == null)
            {
                return NotFound();
            }

            //Prodcut Types
            var types = (await _typeRepository.ListAsync(new CatalogTypeCategorySpec(id.Value)))
                .Select(ct => ct.CatalogType)
                .ToList();

            Category = _mapper.Map<CategoryViewModel>(category);
            Category.ProductTypes = _mapper.Map<List<ProductTypeViewModel>>(types);
            
            return Page();
        }
    }
}
