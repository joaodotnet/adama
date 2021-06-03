﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Entities;
using AutoMapper;
using Backoffice.ViewModels;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;

namespace Backoffice.Pages.Illustrations
{
    public class IndexModel : PageModel
    {
        private readonly IRepository<CatalogIllustration> _repository;
        private readonly IMapper _mapper;

        public IndexModel(IRepository<CatalogIllustration> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IList<IllustrationViewModel> IllustrationModel { get;set; }

        public async Task OnGetAsync()
        {
            IllustrationModel = _mapper.Map<IList<IllustrationViewModel>>(await _repository.ListAsync(new CatalogIllustrationSpecification()));
        }
    }
}
