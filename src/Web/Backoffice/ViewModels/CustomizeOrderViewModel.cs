using ApplicationCore.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.ViewModels
{
    public class CustomizeOrderViewModel
    {
        [Display(Name="#")]
        public int Id { get; set; }
        [Display(Name = "Email")]
        public string BuyerId { get; set; }
        [Display(Name = "Nome")]
        public string BuyerName { get; set; }
        [Display(Name = "Telefone")]
        public string BuyerContact { get; set; }
        [Display(Name = "Data")]
        public DateTimeOffset OrderDate { get; set; }
        [Display(Name = "Estado")]
        public OrderStateType OrderState { get; set; }
        [Display(Name = "Produto")]
        public string ProductName { get; set; }
        [Display(Name = "Imagem Produto")]
        public string ProductPictureUri { get; set; }        
        public int ProductId { get; set; }
        [Display(Name = "Descrição")]
        public string Description { get; set; }
        [Display(Name = "Frase/Texto")]
        public string Text { get; set; }
        [Display(Name = "Anexo")]
        public string AttachFileName { get; set; }
        [Display(Name = "Cores")]
        public string Colors { get; set; }
    }
}
