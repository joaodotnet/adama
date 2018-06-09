using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public enum CatalogAttributeType
    {
        [Display(Name = "Tamanho")]
        SIZE,
        [Display(Name = "Formato")]
        BOOK_FORMAT,
        [Display(Name = "Côr")]
        Color,
    }
}