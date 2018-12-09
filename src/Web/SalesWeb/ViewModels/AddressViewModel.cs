using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWeb.ViewModels
{
    public class AddressViewModel
    {
        //[Range(1,2,ErrorMessage = "Por favor escolha o método de entrega.")]
        public int? UseUserAddress { get; set; }
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Display(Name = "Telefone")]
        public string ContactPhoneNumber { get; set; }
        [Display(Name = "Morada")]
        [StringLength(100, ErrorMessage = "O campo Morada têm que ter no máximo 100 caracteres!")]
        public string Street { get; set; }
        [Display(Name = "Morada (Linha 2)")]
        public string StreetLine2 { get; set; }
        [Display(Name = "Cidade")]
        public string City { get; set; }
        [Display(Name = "País")]
        public string Country { get; set; }
        [Display(Name = "Código Postal")]
        public string PostalCode { get; set; }
        [Display(Name = "Guardar sua morada na sua conta")]
        public bool SaveAddress { get; set; } = true;
        public bool UseSameAsShipping { get; set; } = true;
        [Display(Name = "Email do cliente")]
        [EmailAddress]
        public string InvoiceEmail { get; set; }        
        [Display(Name = "Nome")]
        public string InvoiceName { get; set; }
        [Display(Name = "NIF")]
        public int? InvoiceTaxNumber { get; set; }
        [Display(Name = "Morada")]
        [StringLength(100, ErrorMessage = "O campo Morada têm que ter no máximo 100 caracteres!")]
        public string InvoiceAddressStreet { get; set; }
        [Display(Name = "Cidade")]
        public string InvoiceAddressCity { get; set; }
        [Display(Name = "País")]
        public string InvoiceAddressCountry { get; set; }
        [Display(Name = "Código Postal")]
        [RegularExpression("^\\d{4}-\\d{3}?$", ErrorMessage = "O Código Postal (Faturação) deverá ter o formato 8000-100")]
        public string InvoiceAddressPostalCode { get; set; }
        [Display(Name = "Guardar sua morada na sua conta")]
        public bool InvoiceSaveAddress { get; set; } = true;
    }

    //public class RequiredIfAttribute : ValidationAttribute, IClientModelValidator
    //{

    //    public RequiredIfAttribute()
    //    {
    //    }

    //    public void AddValidation(ClientModelValidationContext context)
    //    {
    //        var error = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
    //        context.Attributes.Add("data-val", "true");
    //        context.Attributes.Add("data-val-error", error);
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        AddressViewModel address = (AddressViewModel)validationContext.ObjectInstance;

    //        string valueToCheck = value as string;

    //        if (address.UseUserAddress == 1 && !string.IsNullOrEmpty(valueToCheck))
    //        {
    //            return new ValidationResult("O campo Morada é obrigatório.");
    //        }

    //        return ValidationResult.Success;
    //    }
    //}
}
