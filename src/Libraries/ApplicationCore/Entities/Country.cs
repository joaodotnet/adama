using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Country : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
