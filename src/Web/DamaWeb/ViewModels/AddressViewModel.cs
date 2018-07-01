using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DamaWeb.ViewModels
{
    public class AddressViewModel
    {
        [Required(ErrorMessage = "Por favor escolha o método de entrega.")]
        public int UseUserAddress { get; set; }
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O campo Nome (Morada de Entrega) é obrigatório")]
        public string Name { get; set; }
        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "O campo Telefone é obrigatório")]
        public int? ContactPhoneNumber { get; set; }
        [Display(Name = "Morada")]
        [StringLength(50, ErrorMessage = "O campo Morada têm que ter no máximo 50 caracteres!")]
        [Required(ErrorMessage = "O campo Morada (Morada de Entrega) é obrigatório")]
        public string Street { get; set; }
        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "O campo Cidade (Morada de Entrega) é obrigatório")]
        public string City { get; set; }
        [Display(Name = "País")]
        [Required(ErrorMessage = "O campo País (Morada de Entrega) é obrigatório")]
        public string Country { get; set; }
        [Display(Name = "Código Postal")]
        [Required(ErrorMessage = "O campo Código Postal (Morada de Entrega) é obrigatório")]
        public string PostalCode { get; set; }
        [Display(Name = "Guardar sua morada na sua conta")]
        public bool SaveAddress { get; set; } = true;
        public bool UseSameAsShipping { get; set; } = true;
        [Display(Name = "Nome")]
        public string InvoiceName { get; set; }
        [Display(Name = "NIF")]
        public int? InvoiceTaxNumber { get; set; }
        [Display(Name = "Morada")]
        [StringLength(50, ErrorMessage = "O campo Morada têm que ter no máximo 50 caracteres!")]
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
