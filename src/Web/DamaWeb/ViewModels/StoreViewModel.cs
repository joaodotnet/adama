using System.ComponentModel.DataAnnotations;

namespace DamaWeb.ViewModels
{
    public class StoreViewModel
    {
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        public string Name { get; set; }
        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "O campo Telefone é obrigatório")]
        public string ContactPhone { get; set; }
        public string Street => "Mercado de Loulé - Banca nº 44, Praça da Republica";
        public string City => "Loulé";
        public string PostalCode => "8100-270";
        public string Country => "Portugal";

    }
}
