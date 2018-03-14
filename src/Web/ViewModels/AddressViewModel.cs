using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class AddressViewModel
    {
        [Required(ErrorMessage = "Por favor escolha o método de entrega.")]
        public int UseUserAddress { get; set; }
        [Display(Name = "Nome")]      
        public string Name { get; set; }
        [Display(Name = "Telefone")]
        public int? ShipAddressPhoneNumber { get; set; }
        [Display(Name = "Telefone")]
        public int? ContactPhoneNumber { get; set; }
        [Display(Name = "Morada")]
        public string Street { get; set; }
        [Display(Name = "Cidade")]
        public string City { get; set; }
        [Display(Name = "País")]
        public string Country { get; set; }
        [Display(Name = "Código Postal")]
        public string PostalCode { get; set; }
        [Display(Name = "Guardar sua morada na sua conta")]
        public bool SaveAddress { get; set; } = true;
        public bool UseSameAsShipping { get; set; } = true;
        [Display(Name = "Nome")]
        public string InvoiceName { get; set; }
        [Display(Name = "NIF")]
        public int? InvoiceTaxNumber { get; set; }
        [Display(Name = "Morada")]
        public string InvoiceAddressStreet { get; set; }
        [Display(Name = "Cidade")]
        public string InvoiceAddressCity { get; set; }
        [Display(Name = "País")]
        public string InvoiceAddressCountry { get; set; }
        [Display(Name = "Código Postal")]
        public string InvoiceAddressPostalCode { get; set; }
        [Display(Name = "Guardar sua morada na sua conta")]
        public bool InvoiceSaveAddress { get; set; } = true;
    }
}
