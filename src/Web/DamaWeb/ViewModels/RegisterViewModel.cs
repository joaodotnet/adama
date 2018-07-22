using DamaWeb.ViewModels.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DamaWeb.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O {0} é obrigatório.")]
        [EmailAddress(ErrorMessage ="O endereço de Email não é valido.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O {0} é obrigatório.")]
        [StringLength(100, ErrorMessage = "A {0} deverá ter no mínimo {2} caractêres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar password")]
        [Compare("Password", ErrorMessage = "A password e a confirmação não coincidem.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "O {0} é obrigatório.")]
        [Display(Name = "Nome")]        
        public string FirstName { get; set; }
        [Required(ErrorMessage = "O {0} é obrigatório.")]
        [Display(Name = "Apelido")]
        public string LastName { get; set; }
        [Display(Name = "Telefone")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Aceito subscrever a newsletter da Dama no Jornal para ficar a par de todas as novidades.")]
        public bool SubscribeNewsletter { get; set; } = true;       
        [EnforceTrue(ErrorMessage = "Deves aceitar os Termos e Condições.")]
        public bool AgreeToTerms { get; set; } = false;
    }    
}
