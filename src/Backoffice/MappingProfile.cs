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
            CreateMap<Category, CategoryViewModel>();
                 //.ForMember(x => x.NrTypeProducts, o => o.MapFrom(x => x.CatalogTypes.Count));
            CreateMap<CategoryViewModel, Category>();
            CreateMap<CatalogType, ProductTypeViewModel>()
                .ForMember(dest => dest.CategoriesName,
                opts => opts.MapFrom(src => src.Categories.Select(c => c.Category.Name)));
            CreateMap<ProductTypeViewModel, CatalogType>();
            CreateMap<CatalogIllustration, IllustrationViewModel>();
            CreateMap<IllustrationViewModel, CatalogIllustration>();
            CreateMap<IllustrationType, IllustrationTypeViewModel>();
            CreateMap<IllustrationTypeViewModel, IllustrationType>();
            CreateMap<CatalogItem, ProductViewModel>();
            CreateMap<ProductViewModel, CatalogItem>();
            CreateMap<CatalogAttribute, ProductAttributeViewModel>();
                //.ForMember(dest => dest.Sku,
                //opts => opts.MapFrom(src => $"{src.CatalogItem.CatalogType.Code}_{src.CatalogItem.CatalogIllustration.Code}{src.CatalogItem.CatalogIllustration.IllustrationType.Code}_{src.Code}"));
            CreateMap<ProductAttributeViewModel, CatalogAttribute>();
            CreateMap<ShopConfigViewModel, ShopConfig>();
            CreateMap<ShopConfig, ShopConfigViewModel>();
            CreateMap<ShopConfigDetail, ShopConfigDetailViewModel>();
            CreateMap<ShopConfigDetailViewModel, ShopConfigDetail>();
            CreateMap<CatalogPicture, ProductPictureViewModel>();
            CreateMap<ProductPictureViewModel, CatalogPicture>();
            CreateMap<CatalogCategory, CatalogCategoryViewModel>()
                //.ForMember(dest => dest.CategoryId, opts => opts.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Selected, opts => opts.UseValue(true))
                .ForMember(dest => dest.Label, opts => opts.MapFrom(src => src.Category.Name));
        }
    }
}
