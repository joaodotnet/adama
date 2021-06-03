using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Backoffice.ViewModels;
using AutoMapper;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using ApplicationCore.Entities;

namespace Backoffice.Pages.Category
{
    public class IndexModel : PageModel
    {
        private readonly IRepository<ApplicationCore.Entities.Category> _categoryRepository;
        private readonly IRepository<CatalogTypeCategory> _typeRepository;
        private readonly IMapper _mapper;

        public IndexModel(IRepository<ApplicationCore.Entities.Category> categoryRepository, 
            IRepository<ApplicationCore.Entities.CatalogTypeCategory> typeRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _typeRepository = typeRepository;
            _mapper = mapper;
        }

        public IList<CategoryViewModel> Categories { get;set; }

        public async Task OnGetAsync()
        {
            var cats = await _categoryRepository.ListAsync(new CategorySpecification(new CategoryFilter{ IncludeCatalogTypes = true }));

            Categories = _mapper.Map<List<CategoryViewModel>>(cats);

            foreach (var item in Categories)
            {
                //Prodcut Types
                item.NrTypeProducts = (await _typeRepository.ListAsync(new CatalogTypeCategorySpec(item.Id)))
                    .Select(ct => ct.CatalogType)
                    .Count();
            }
        }
    }
}
