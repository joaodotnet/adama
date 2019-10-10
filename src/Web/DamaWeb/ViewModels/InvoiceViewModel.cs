using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DamaWeb.ViewModels
{
    public class InvoiceViewModel
    {        
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        public string Name { get; set; }        
        [Display(Name = "NIF")]
        [Required(ErrorMessage = "O campo NIF é obrígatório")]
        public int? TaxNumber { get; set; }
        [Display(Name = "Morada")]
        [StringLength(100, ErrorMessage = "O campo Morada têm que ter no máximo 100 caracteres!")]
        [Required(ErrorMessage = "O campo Morada é obrígatório")]
        public string AddressStreet { get; set; }
        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "O campo Cidade é obrígatório")]
        public string AddressCity { get; set; }
        [Display(Name = "País")]
        [Required(ErrorMessage = "O campo País é obrígatório")]
        public string AddressCountry { get; set; }
        [Display(Name = "Código Postal")]
        [RegularExpression("^\\d{4}-\\d{3}?$", ErrorMessage = "O Código Postal (Faturação) deverá ter o formato 8000-100")]
        [Required(ErrorMessage = "O campo Código Postal é obrígatório")]
        public string AddressPostalCode { get; set; }
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
