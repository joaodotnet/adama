using ApplicationCore.Entities;
using AutoMapper;
using SalesWeb.Interfaces;
using SalesWeb.ViewModels;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Infrastructure.Identity;
using ApplicationCore.Interfaces;

namespace SalesWeb.Services
{
    public class ShopService : IShopService
    {
        private readonly GroceryContext _db;
        private readonly AppIdentityDbContext _identityDb;
        private readonly IUriComposer _uriComposer;
        private readonly IMapper _mapper;
        public ShopService(GroceryContext db, IMapper mapper, AppIdentityDbContext identity, IUriComposer uriComposer)
        {
            _db = db;
            _mapper = mapper;
            _identityDb = identity;
            _uriComposer = uriComposer;
        }

        public async Task AddorUpdateUserAddress(ApplicationUser user, AddressViewModel addressModel, AddressType addressType = AddressType.SHIPPING)
        {
            if(user != null && addressModel != null)
            {
                //get user Addresses
                var addresses = await _identityDb.UserAddresses
                    .Include(x => x.User)
                    .Where(x => x.UserId == user.Id && x.AddressType == addressType)
                    .FirstOrDefaultAsync();

                //user.BillingAddressSameAsShipping = addressModel.UseSameAsShipping;
                if (addresses == null)
                {                    
                    var newAddress = new UserAddress
                    {
                        User = user,
                        AddressType = addressType
                    };
                    if(addressType == AddressType.SHIPPING)
                    {                        
                        newAddress.Street = addressModel.Street;
                        newAddress.City = addressModel.City;
                        newAddress.PostalCode = addressModel.PostalCode;
                        newAddress.Country = addressModel.Country;
                    }
                    else
                    {
                        newAddress.Street = addressModel.InvoiceAddressStreet;
                        newAddress.City = addressModel.InvoiceAddressCity;
                        newAddress.PostalCode = addressModel.InvoiceAddressPostalCode;
                        newAddress.Country = addressModel.InvoiceAddressCountry;
                    }
                    _identityDb.UserAddresses.Add(newAddress);
                }
                else
                {                                                            

                    if (addressType == AddressType.SHIPPING)
                    {
                        addresses.Street = addressModel.Street;
                        addresses.City = addressModel.City;
                        addresses.PostalCode = addressModel.PostalCode;
                        addresses.Country = addressModel.Country;                        
                    }
                    else
                    {
                        addresses.Street = addressModel.InvoiceAddressStreet;
                        addresses.City = addressModel.InvoiceAddressCity;
                        addresses.PostalCode = addressModel.InvoiceAddressPostalCode;
                        addresses.Country = addressModel.InvoiceAddressCountry;
                    }
                    
                }

                await _identityDb.SaveChangesAsync();
            }            
        }

        public async Task<AddressViewModel> GetUserAddress(string userId)
        {
            var addresses = await _identityDb.UserAddresses
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var addressViewModel = new AddressViewModel();            
            if (addresses?.Count > 0)
            {
                var defaultAddress = addresses.FirstOrDefault();
                if (defaultAddress != null)
                {
                    //addressViewModel.UseSameAsShipping = defaultAddress.User?.BillingAddressSameAsShipping ?? false;
                    addressViewModel.Name = $"{defaultAddress.User?.FirstName} {defaultAddress.User?.LastName}";
                    addressViewModel.InvoiceName = $"{defaultAddress.User.FirstName} {defaultAddress.User?.LastName}";
                }
           
                foreach (var item in addresses)
                {
                    
                    if (item.AddressType == AddressType.SHIPPING)
                    {
                        addressViewModel.Street = item.Street;
                        addressViewModel.City = item.City;
                        addressViewModel.PostalCode = item.PostalCode;
                        addressViewModel.Country = item.Country;                        
                    }
                    else
                    {
                        addressViewModel.InvoiceAddressStreet = item.Street;
                        addressViewModel.InvoiceAddressCity = item.City;
                        addressViewModel.InvoiceAddressPostalCode = item.PostalCode;
                        addressViewModel.InvoiceAddressCountry = item.Country;
                    }
                }                                
            }
            return addressViewModel;
        }        
    }
}
