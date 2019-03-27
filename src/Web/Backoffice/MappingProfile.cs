using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using AutoMapper;
using Backoffice.Extensions;
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
            CreateMap<Pages.Products.CreateModel.ProductCreateViewModel, CatalogItem>();
            CreateMap<CatalogItem,Pages.Products.EditModel.ProductEditViewModel>()
                .ForMember(x => x.Picture, opt => opt.Ignore())
                .ForMember(x => x.OtherPictures, opt => opt.Ignore());
            CreateMap<CatalogAttribute, ProductAttributeViewModel>();
            //.ForMember(dest => dest.AttributeName,
            //opts => opts.MapFrom(src => $"{EnumHelper<AttributeType>.GetDisplayValue(src.Attribute.Type)} {src.Attribute.Name}"));
            CreateMap<Pages.Products.Attributes.CreateModel.AttributeCreateModel, CatalogAttribute>();
            CreateMap<Pages.Products.Attributes.EditModel.AttributeEditModel, CatalogAttribute>();
            CreateMap<ProductAttributeViewModel, CatalogAttribute>();
            CreateMap<ShopConfigViewModel, ShopConfig>();
            CreateMap<ShopConfig, ShopConfigViewModel>();
            CreateMap<ShopConfigDetail, ShopConfigDetailViewModel>();
            CreateMap<ShopConfigDetailViewModel, ShopConfigDetail>();
            CreateMap<CatalogPicture, ProductPictureViewModel>();
            CreateMap<ProductPictureViewModel, CatalogPicture>();
            CreateMap<CatalogCategory, CatalogCategoryViewModel>()
                //.ForMember(dest => dest.CategoryId, opts => opts.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Selected, opts => opts.MapFrom(src => true))
                .ForMember(dest => dest.Label, opts => opts.MapFrom(src => src.Category.Name));
            CreateMap<Order, OrderViewModel>()
                .ForMember(dest => dest.ItemsCount, opts => opts.MapFrom(src => src.OrderItems.Sum(x => x.Units)))
                .ForMember(dest => dest.ShipToAddress, opts => opts.MapFrom(src => string.IsNullOrEmpty(src.ShipToAddress.Street) ? "" : $"{src.ShipToAddress.Street}, {src.ShipToAddress.PostalCode}, {src.ShipToAddress.City}"))
                .ForMember(dest => dest.BillingToAddress, opts => opts.MapFrom(src => string.IsNullOrEmpty(src.BillingToAddress.Street) ? "" : $"{src.BillingToAddress.Street}, {src.BillingToAddress.PostalCode}, {src.BillingToAddress.City}"))
                .ForMember(dest => dest.HasCustomizeProducts, opts => opts.MapFrom(src => src.OrderItems.Any(i => i.CustomizeItem.CatalogTypeId.HasValue)));
            CreateMap<OrderItem, OrderItemViewModel>()
                //.ForMember(dest => dest.Attributes, opts => opts.MapFrom(src => src.Details))
                .ForMember(dest => dest.ProductId, opts => opts.MapFrom(src => src.ItemOrdered.CatalogItemId))
                .ForMember(dest => dest.PictureUri, opts => opts.MapFrom(src => !src.CustomizeItem.CatalogTypeId.HasValue ? src.ItemOrdered.PictureUri : src.CustomizeItem.PictureUri))
                .ForMember(dest => dest.ProductName, opts => opts.MapFrom(src => !src.CustomizeItem.CatalogTypeId.HasValue ? src.ItemOrdered.ProductName : src.CustomizeItem.ProductName))
                //.ForMember(dest => dest.CustomizeName, opts => opts.MapFrom(src => $"{src.CustomizeName} ({src.CustomizeSide})"))
                .ForMember(dest => dest.CustomizeItemCatalogTypeId, opts => opts.MapFrom(src => src.CustomizeItem.CatalogTypeId))
                .ForMember(dest => dest.CustomizeItemDescription, opts => opts.MapFrom(src => src.CustomizeItem.Description))
                .ForMember(dest => dest.CustomizeItemNameOrText, opts => opts.MapFrom(src => src.CustomizeItem.Name))
                .ForMember(dest => dest.CustomizeItemColors, opts => opts.MapFrom(src => src.CustomizeItem.Colors));
            CreateMap<OrderItemDetail, OrderItemAttributeViewModel>();
            CreateMap<CustomizeOrder, CustomizeOrderViewModel>()
                .ForMember(dest => dest.ProductId, opts => opts.MapFrom(src => src.ItemOrdered.CatalogItemId))
                .ForMember(dest => dest.ProductName, opts => opts.MapFrom(src => src.ItemOrdered.ProductName))
                .ForMember(dest => dest.ProductPictureUri, opts => opts.MapFrom(src => src.ItemOrdered.PictureUri));
            CreateMap<FileDetail, FileDetailViewModel>();
            CreateMap<FileDetailViewModel, FileDetail>();
            CreateMap<ShippingPriceWeight, ShippingPriceViewModel>();
            CreateMap<ShippingPriceViewModel, ShippingPriceWeight>();
        }
    }
}
