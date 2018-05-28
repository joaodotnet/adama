using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dama.API.ViewModel
{
    public class OrderViewModel
    {
        [Required]
        public string BuyerId { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }        
        public string State { get; set; }
        [Required]
        public string Country { get; set; }

        public string PostalCode { get; set; }
        
        public string CardNumber { get; set; }
        
        public string CardHolderName { get; set; }
        
        public DateTime CardExpiration { get; set; }
        
        public string CardSecurityNumber { get; set; }

        public int CardTypeId { get; set; }

        public string Buyer { get; set; }
    }


}
