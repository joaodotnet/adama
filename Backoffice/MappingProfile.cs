using ApplicationCore.Entities;
using AutoMapper;
using Backoffice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<Category, CategoryViewModel>()
                 .ForMember(x => x.NrTypeProducts, o => o.MapFrom(x => x.ProductTypes.Count));
            CreateMap<CategoryViewModel, Category>();
            CreateMap<ProductType, ProductTypeViewModel>();
            CreateMap<ProductTypeViewModel, ProductType>();
            CreateMap<Illustration, IllustrationViewModel>();
            CreateMap<IllustrationViewModel, Illustration>();
            CreateMap<IllustrationType, IllustrationTypeViewModel>();
            CreateMap<IllustrationTypeViewModel, IllustrationType>();
            CreateMap<Product, ProductViewModel>()
                .ForMember(dest => dest.ProductSKU,
                opts => opts.MapFrom(src => $"{src.ProductType.Code}_{src.Illustation.Code}{src.Illustation.IllustrationType.Code}"));
            CreateMap<ProductViewModel, Product>()
                .ForMember(dest => dest.Personalized,
                opts => opts.UseValue(false));
            CreateMap<ProductAttribute, ProductAttributeViewModel>()
                .ForMember(dest => dest.ProductSKU,
                opts => opts.MapFrom(src => $"{src.Product.ProductType.Code}_{src.Product.Illustation.Code}{src.Product.Illustation.IllustrationType.Code}_{src.Code}"));
            CreateMap<ProductAttributeViewModel, ProductAttribute>();
        }
    }
}
