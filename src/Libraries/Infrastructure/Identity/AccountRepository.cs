using ApplicationCore.DTOs;
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
        public async Task AddOrUpdateUserAddress(string userId, string name, int? taxNumber, string contactNumber, bool useSameAsShipping, string street, string postalCode, string city, string country, AddressType addressType)
        {
            var user = _context.Users
                .Include(x => x.Addresses)
                .SingleOrDefault(x => x.Id == userId);
            if (user != null)
            {
                var addresses = user.Addresses.SingleOrDefault(x => x.AddressType == addressType);
                if (addresses == null)
                {
                    _context.UserAddresses.Add(new UserAddress
                    {
                        Name = name,
                        TaxNumber = taxNumber,
                        ContactNumber = contactNumber,
                        User = user,
                        AddressType = addressType,
                        Street = street,
                        City = city,
                        PostalCode = postalCode,
                        Country = country,
                        BillingAddressSameAsShipping = useSameAsShipping
                    });
                }
                else
                {
                    addresses.Name = name;
                    addresses.TaxNumber = taxNumber;
                    addresses.ContactNumber = contactNumber;
                    addresses.Street = street;
                    addresses.City = city;
                    addresses.PostalCode = postalCode;
                    addresses.Country = country;
                    addresses.BillingAddressSameAsShipping = useSameAsShipping;
                }

                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<UserAddressDTO>> GetUserAddressAsync(string userId)
        {
            return await _context.UserAddresses
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .Select(x => new UserAddressDTO
                {
                    Name = x.Name,
                    TaxNumber = x.TaxNumber,
                    ContactNumber = x.ContactNumber,
                    AddressType = x.AddressType,
                    Street = x.Street,
                    City = x.City,
                    PostalCode = x.PostalCode,
                    Country = x.Country,
                    BillingAddressSameAsShipping = x.BillingAddressSameAsShipping
                }).ToListAsync();
        }
    }
}
