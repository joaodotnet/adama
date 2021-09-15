using System.Linq;
using ApplicationCore.Entities;
using AutoMapper;
using DamaAdmin.Shared.Models;

namespace DamaAdmin.Server
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryViewModel, Category>();

            CreateMap<CatalogType, ProductTypeViewModel>()
            .ForMember(dest => dest.CategoriesName,
                opts => opts.MapFrom(src => src.Categories.Select(c => c.Category.Name)));
            CreateMap<ProductTypeViewModel, CatalogType>();
        }
    }
}
