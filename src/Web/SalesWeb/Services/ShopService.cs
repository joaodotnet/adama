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
                    addressViewModel.UseSameAsShipping = defaultAddress.User?.BillingAddressSameAsShipping ?? false;
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
