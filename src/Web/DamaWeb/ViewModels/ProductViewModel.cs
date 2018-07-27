using ApplicationCore.Entities;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DamaWeb.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public List<string> ProductImagesUri { get; set; }
        public List<ProductAttributeViewModel> Attributes { get; set; }
        public int ProductQuantity { get; set; } = 1;
        [Display(Name = "Referência:")]
        public string ProductSKU { get; set; }
        public List<LinkViewModel> Categories { get; set; }
        public List<LinkViewModel> Tags { get; set; }
        public int DeliveryTimeMin { get; set; }
        public int DeliveryTimeMax { get; set; }
        public DeliveryTimeUnitType DeliveryTimeUnit { get; set; }
        public bool CanCustomize { get { return CanCustomizeName || CanCustomizeTotal; } }
        public bool CanCustomizeName { get { return CustomizePrice.HasValue && CustomizePrice.Value > 0; } }
        public bool CanCustomizeTotal { get; set; }
        [StringLength(100, ErrorMessage = "Por favor escolhe um nome ou frase até 100 caracteres")]
        public string NameInput { get; set; }
        public int FirstCategoryId { get; set; }
        public List<ProductReferenceViewModel> ProductReferences { get; set; } = new List<ProductReferenceViewModel>();
        //public IEnumerable<SelectListItem> ProductReferences { get; set; }
        public decimal? CustomizePrice { get; set; }
        public string CustomizePictureFileName { get; set; }
        [Required(ErrorMessage = "O endereço de Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O endereço de Email não é valido.")]
        [Display(Name = "Email")]
        public string ContactEmailAddress { get; set; }
        [Display(Name = "Mensagem")]
        public string Message { get; set; }
        public List<PictureHelperViewModel> PictureHelpers { get; set; }
    }

    public class PictureHelperViewModel
    {
        public string PictureUri { get; set; }
        public string PictureFileName { get; set; }
    }

    public class ProductReferenceViewModel
    {
        public string Label { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
    }

    public class LinkViewModel
    {
        public string Uri { get; set; }
        public string Name { get; set; }
        public string TagName { get; set; }
    }

    public class ProductAttributeViewModel
    {
        public AttributeType AttributeType { get; set; }
        public string Label { get; set; }
        public int? DefaultValue { get; set; }
        public string DefaultText { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
        [Required(ErrorMessage = "Por favor escolha uma opção")]
        [Range(1,999999999, ErrorMessage = "Por favor escolha uma opção")]
        public int Selected { get; set; }
        public List<AttributeViewModel> Attributes { get; set; }
    }

    public class AttributeViewModel
    {
        public int Id { get; set; }
        public decimal? Price { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string ReferenceCatalogSku { get; set; }
    }
}
