using System.Runtime.InteropServices;
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

            CreateMap<FileDetail, FileDetailViewModel>();
            CreateMap<FileDetailViewModel, FileDetail>();

            CreateMap<IllustrationType, IllustrationTypeViewModel>();
            CreateMap<IllustrationTypeViewModel, IllustrationType>();

            CreateMap<CatalogIllustration, IllustrationViewModel>();
            CreateMap<IllustrationViewModel, CatalogIllustration>();

            CreateMap<CatalogItem, ProductViewModel>()
                .ForMember(dest => dest.CategoriesName,
                    opts => opts.MapFrom(src => src.Categories.Select(c => c.Category.Name)))
                .ForMember(dest => dest.Categories, opts => opts.Ignore());
            CreateMap<ProductViewModel, CatalogItem>()
                .ForMember(dest => dest.Pictures, opts => opts.Ignore())
                .ForMember(dest => dest.Categories, opts => opts.Ignore());

            CreateMap<CatalogPicture, ProductPictureViewModel>();

            CreateMap<CatalogReference, CatalogReferenceViewModel>()
                .ForMember(dest => dest.ReferenceCatalogItemName, opts => opts.MapFrom(src => src.ReferenceCatalogItem.Name))
                .ForMember(dest => dest.CatalogItemName, opts => opts.MapFrom(src => src.CatalogItem.Name));
        }
    }
}
