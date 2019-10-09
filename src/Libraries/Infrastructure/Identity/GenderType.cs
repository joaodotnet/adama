using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Infrastructure.Identity
{
    public enum GenderType
    {
        [Display(Name= "Feminino")]
        FEMALE,
        [Display(Name = "Masculino")]
        MALE
    }
}
