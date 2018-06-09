using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppIdentityDbContext _context;

        public AccountRepository(AppIdentityDbContext context)
        {
            this._context = context;
        }
        public async Task AddOrUpdateUserAddress(string userId, bool useSameAsShipping, string street, string postalCode, string city, string country, AddressType addressType)
        {
            var user = _context.Users
                .Include(x => x.Addresses)
                .SingleOrDefault(x => x.Id == userId);
            if (user != null)
            {
                user.BillingAddressSameAsShipping = useSameAsShipping;
                var addresses = user.Addresses.SingleOrDefault(x => x.AddressType == addressType);
                if (addresses == null)
                {
                    _context.UserAddresses.Add(new UserAddress
                    {
                        User = user,
                        AddressType = addressType,
                        Street = street,
                        City = city,
                        PostalCode = postalCode,
                        Country = country
                    });
                }
                else
                {
                    addresses.Street = street;
                    addresses.City = city;
                    addresses.PostalCode = postalCode;
                    addresses.Country = country;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
