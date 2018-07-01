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
        //[Range(1,2,ErrorMessage = "Por favor escolha o método de entrega.")]
        public int? UseUserAddress { get; set; }
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        public string Name { get; set; }
        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "O campo Telefone é obrigatório")]
        public int? ContactPhoneNumber { get; set; }
        [Display(Name = "Morada")]
        [StringLength(50, ErrorMessage = "O campo Morada têm que ter no máximo 50 caracteres!")]
        [Required(ErrorMessage = "O campo Morada é obrigatório")]       
        public string Street { get; set; }
        [Display(Name = "Morada (Linha 2)")]
        [StringLength(50, ErrorMessage = "O campo Morada (Linha 2) têm que ter no máximo 50 caracteres!")]
        public string StreetLine2 { get; set; }
        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "O campo Cidade é obrigatório")]
        public string City { get; set; }
        [Display(Name = "País")]
        [Required(ErrorMessage = "O campo País é obrigatório")]
        public string Country { get; set; }
        [Display(Name = "Código Postal")]
        [Required(ErrorMessage = "O campo Código Postal é obrigatório")]
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
