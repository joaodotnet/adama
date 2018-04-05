using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Services
{
    public class CustomizeViewModelService : ICustomizeViewModelService
    {
        private readonly IAsyncRepository<Category> _categoryRepository;
        private readonly ICatalogService _catalogService;

        public CustomizeViewModelService(IAsyncRepository<Category> categoryRepository, ICatalogService catalogService)
        {
            _categoryRepository = categoryRepository;
            _catalogService = catalogService;
        }

        public async Task<CustomizeViewModel> GetCustomizeItems(int? categoryid)
        {
            var categorySpec = new CategorySpecification();
            var cats = await _categoryRepository.ListAsync(categorySpec);

            return new CustomizeViewModel
            {
                Categories = cats.Select(x => (x.Id, x.Name)).ToList()
            };
        }
    }
}
