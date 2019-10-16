using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? NIF { get; set; }
        public GenderType? Gender { get; set; }
        public int Score { get; set; }
        public ICollection<UserAddress> Addresses { get; set; }
    }
}
