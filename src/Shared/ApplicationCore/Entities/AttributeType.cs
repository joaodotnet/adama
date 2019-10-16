using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public enum AttributeType
    {
        [Display(Name = "Tamanho")]
        SIZE,
        [Display(Name = "Formato")]
        BOOK_FORMAT,
        [Display(Name = "Cor")]
        Color,
        [Display(Name = "Animal de Estimação")]
        PET,
        [Display(Name = "Frase")]
        TEXT,
        [Display(Name = "Opção")]
        OPTION,
    }
}