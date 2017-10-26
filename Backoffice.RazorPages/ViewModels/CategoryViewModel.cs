﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backoffice.RazorPages.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Name { get; set; }
    }
}
