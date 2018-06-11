using ApplicationCore.Entities;
using AutoMapper;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Infrastructure.Identity;
using ApplicationCore.Interfaces;

namespace DamaWeb.Services
{
    public class ShopService : IShopService
    {
        private readonly DamaContext _db;
        private readonly AppIdentityDbContext _identityDb;
        private readonly IUriComposer _uriComposer;
        private readonly IMapper _mapper;
        public ShopService(DamaContext db, IMapper mapper, AppIdentityDbContext identity, IUriComposer uriComposer)
        {
            _db = db;
            _mapper = mapper;
            _identityDb = identity;
            _uriComposer = uriComposer;
        }

        //public async Task<CatalogType> GetCatalogType(string type)
        //{
        //    var allCatalogTypes = await _db.CatalogTypes.ToListAsync();
        //    foreach (var item in allCatalogTypes)
        //    {
        //        var typeName = item.Description.Replace(" ", "-").ToLower();
        //        if (Utils.RemoveDiacritics(typeName) == type)
        //            return item;
        //    }
        //    return null;
        //}

        //public async Task<Category> GetCategory(string name)
        //{
        //    var allCategories = await _db.Categories.ToListAsync();
        //    foreach (var item in allCategories)
        //    {
        //        var catName = item.Name.Replace(" ", "-").ToLower();
        //        if (Utils.RemoveDiacritics(catName) == name.ToLower())
        //            return item;
        //    }
        //    return null;
        //}

        public async Task<List<MainBannerViewModel>> GetMainBanners()
        {
            var bannerConfig = await _db.ShopConfigDetails
                .Include(x => x.ShopConfig)
                .Where(x => x.ShopConfig.Type == ShopConfigType.NEWS_BANNER && x.ShopConfig.IsActive && x.IsActive)
                .ToListAsync();

            bannerConfig.ForEach(x =>
            {
                x.PictureUri = _uriComposer.ComposePicUri(x.PictureUri);
            });

            return _mapper.Map<List<MainBannerViewModel>>(bannerConfig);
        }

        //public async Task<MenuComponentViewModel> GetMenuList()
        //{
        //    //TODO GET CACHE
        //    var categories = await _db.Categories
        //        .Include(x => x.Parent)
        //        .Include(x => x.CatalogTypes)                
        //        .ThenInclude(cts => cts.CatalogType)
        //        .ThenInclude(ct => ct.CatalogItems)
        //        .Where(x => x.CatalogTypes.Any(ct => ct.CatalogType.CatalogItems != null && ct.CatalogType.CatalogItems.Count > 0))
        //        .ToListAsync();

        //    MenuComponentViewModel menuViewModel = new MenuComponentViewModel();

        //    var parentsLeft = categories
        //        .Where(x => !x.ParentId.HasValue && x.Position == "left")
        //        .OrderBy(x => x.Order)
        //        .ToList();

        //    GetTopCategories(menuViewModel.Left, categories, parentsLeft);

        //    var parentsRight = categories
        //        .Where(x => !x.ParentId.HasValue && x.Position == "right")
        //        .OrderBy(x => x.Order)
        //        .ToList();

        //    GetTopCategories(menuViewModel.Right, categories, parentsRight);

        //    return menuViewModel;
        //}
        public async Task AddorUpdateUserAddress(ApplicationUser user, AddressViewModel addressModel, AddressType addressType = AddressType.SHIPPING)
        {
            if(user != null && addressModel != null)
            {
                //get user Addresses
                var addresses = await _identityDb.UserAddresses
                    .Include(x => x.User)
                    .Where(x => x.UserId == user.Id && x.AddressType == addressType)
                    .FirstOrDefaultAsync();

                user.BillingAddressSameAsShipping = addressModel.UseSameAsShipping;
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
            var addressViewModel = new AddressViewModel
            {
                UseSameAsShipping = addresses.First().User.BillingAddressSameAsShipping,
                Name = $"{addresses.First().User.FirstName} {addresses.First().User.LastName}".Trim(),
                InvoiceName = $"{addresses.First().User.FirstName} {addresses.First().User.LastName}".Trim()
            };
            if (addresses?.Count > 0)
            {
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
