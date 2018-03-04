using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<UserAddress> Addresses { get; set; }
    }
}
