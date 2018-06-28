using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.DTOs
{
    public enum PaymentType
    {
        [Display(Name ="Transferência")]
        TRANSFER,
        [Display(Name ="Dinheiro")]
        CASH
    }
}
