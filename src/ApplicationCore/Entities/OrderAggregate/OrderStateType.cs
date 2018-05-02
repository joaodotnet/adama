using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Entities.OrderAggregate
{
    public enum OrderStateType
    {
        [Display(Name = "Pendente")]
        PENDING,
        [Display(Name = "Submetido")]
        SUBMITTED, //Feito a transfência
        [Display(Name = "Em preparação")]
        PREPARING,
        [Display(Name = "Enviado")]
        SENT,
        [Display(Name = "Entregue")]
        DELIVERED,
        [Display(Name = "Cancelado")]
        CANCELED
    }
}
