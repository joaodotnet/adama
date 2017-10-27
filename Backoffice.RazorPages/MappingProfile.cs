using ApplicationCore.Entities;
using AutoMapper;
using Backoffice.RazorPages.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.RazorPages
{
    public class MappingProfile : Profile    
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<Category, CategoryViewModel>()
                 .ForMember(x => x.NrTypeProducts, o => o.MapFrom(x => x.ProductTypes.Count ));
            CreateMap<CategoryViewModel, Category>();
            CreateMap<ProductType, ProductTypeViewModel>();
            CreateMap<ProductTypeViewModel, ProductType>();
            CreateMap<Illustration, IllustrationViewModel>();
            CreateMap<IllustrationViewModel, Illustration>();
        }
    }
}
