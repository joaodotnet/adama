using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string Slug { get; set; }

        [Required]        
        [Display(Name = "Ordem")]
        public int Order { get; set; }

        [Display(Name = "Meta Description")]
        [StringLength(160)]
        public string MetaDescription { get; set; }

        [Display(Name = "Title")]
        [StringLength(43)]
        public string Title { get; set; }

        [Display(Name = "Texto H1")]
        [StringLength(50)]
        public string H1Text { get; set; }
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Display(Name="ID Categoria Pai")]
        public int? ParentId { get; set; }
        [Display(Name = "Nome da Categoria Pai")]
        public CategoryDTO Parent { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
