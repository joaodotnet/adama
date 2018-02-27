using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<UserAddress> Addresses { get; set; }
    }
}
