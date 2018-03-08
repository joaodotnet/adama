using ApplicationCore.Entities;
using AutoMapper;
using Web.Interfaces;
using Web.ViewModels;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Infrastructure.Identity;

namespace Web.Services
{
    public class ShopService : IShopService
    {
        private readonly DamaContext _db;
        private readonly AppIdentityDbContext _identityDb;
        private readonly IMapper _mapper;
        public ShopService(DamaContext db, IMapper mapper, AppIdentityDbContext identity)
        {
            _db = db;
            _mapper = mapper;
            _identityDb = identity;
        }

        public async Task<CatalogType> GetCatalogType(string type)
        {
            var allCatalogTypes = await _db.CatalogTypes.ToListAsync();
            foreach (var item in allCatalogTypes)
            {
                var typeName = item.Description.Replace(" ", "-").ToLower();
                if (Utils.RemoveDiacritics(typeName) == type)
                    return item;
            }
            return null;
        }

        public async Task<Category> GetCategory(string name)
        {
            var allCategories = await _db.Categories.ToListAsync();
            foreach (var item in allCategories)
            {
                var catName = item.Name.Replace(" ", "-").ToLower();
                if (Utils.RemoveDiacritics(catName) == name.ToLower())
                    return item;
            }
            return null;
        }

        public async Task<List<MainBannerViewModel>> GetMainBanners()
        {
            var bannerConfig = await _db.ShopConfigDetails
                .Include(x => x.ShopConfig)
                .Where(x => x.ShopConfig.Type == ShopConfigType.NEWS_BANNER && x.ShopConfig.IsActive && x.IsActive)
                .ToListAsync();
            return _mapper.Map<List<MainBannerViewModel>>(bannerConfig);
        }

        public async Task<MenuComponentViewModel> GetMenuList()
        {
            //TODO GET CACHE
            var categories = await _db.Categories
                .Include(x => x.Parent)
                .Include(x => x.CatalogTypes)
                .ThenInclude(ct => ct.CatalogType)
                .ToListAsync();

            MenuComponentViewModel menuViewModel = new MenuComponentViewModel();

            var parentsLeft = categories
                .Where(x => !x.ParentId.HasValue && x.Position == "left")
                .OrderBy(x => x.Order)
                .ToList();

            GetTopCategories(menuViewModel.Left, categories, parentsLeft);

            var parentsRight = categories
                .Where(x => !x.ParentId.HasValue && x.Position == "right")
                .OrderBy(x => x.Order)
                .ToList();

            GetTopCategories(menuViewModel.Right, categories, parentsRight);

            return menuViewModel;
        }
        public async Task AddorUpdateUserAddress(ApplicationUser user, AddressViewModel addressModel, bool isInvoice = false)
        {
            if(user != null && addressModel != null)
            {
                //get user Addresses
                var addresses = await _identityDb.UserAddresses.Where(x => x.UserId == user.Id && x.IsInvoiceAddress == isInvoice).FirstOrDefaultAsync();
                if (addresses == null)
                {
                    var newAddress = new UserAddress
                    {
                        UserId = user.Id,                        
                        DefaultAddress = true,
                        UseInvoiceSameAsShipping = addressModel.UseSameAsShipping,
                        IsInvoiceAddress = isInvoice
                    };
                    if(!isInvoice)
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
                    addresses.DefaultAddress = true;
                    addresses.UseInvoiceSameAsShipping = addressModel.UseSameAsShipping;
                    addresses.IsInvoiceAddress = isInvoice;

                    if (!isInvoice)
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

                //if (addresses?.Count > 0 && addressModel.SaveAddress)
                //{
                //    addresses.ForEach(x => x.DefaultAddress = false);
                //}

                await _identityDb.SaveChangesAsync();
            }            
        }

        public async Task<AddressViewModel> GetUserAddress(string userId)
        {
            var addresses = await _identityDb.UserAddresses.Where(x => x.UserId == userId).ToListAsync();
            var addressViewModel = new AddressViewModel();
            if (addresses?.Count > 0)
            {
                foreach (var item in addresses)
                {
                    addressViewModel.UseSameAsShipping = item.UseInvoiceSameAsShipping;
                    if (!item.IsInvoiceAddress)
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

        private void GetTopCategories(List<MenuItemComponentViewModel> model, List<Category> categories, List<Category> parents)
        {
            model.AddRange(parents.Select(x => new MenuItemComponentViewModel
            {
                Id = x.Id,
                Name = x.Name.ToUpper(),
                NameUri = Utils.RemoveDiacritics(x.Name).Replace(" ", "-").ToLower()
            }));

            //SubCategories
            foreach (var item in model)
            {
                var childs = categories
                    .Where(x => x.ParentId == item.Id)
                    .OrderBy(x => x.Order)
                    .ToList();

                if(childs?.Count > 0)
                {
                    item.Childs.AddRange(childs.Select(x => new MenuItemComponentViewModel
                    {
                        Id = x.Id,
                        Name = x.Name.ToUpper(),
                        NameUri = Utils.RemoveDiacritics(x.Name).Replace(" ", "-").ToLower()
                    }));
                }
                else
                {
                    var types = categories
                        .Where(x => x.Id == item.Id)
                        .Select(x => x.CatalogTypes);

                    var catalogTypes = types.SelectMany(x => x.Select(t => t.CatalogType));

                    item.Childs.AddRange(catalogTypes.Select(x => new MenuItemComponentViewModel
                    {
                        Id = x.Id,
                        Name = x.Description.ToUpper(),
                        NameUri = Utils.RemoveDiacritics(item.Name).Replace(" ", "-").ToLower(),
                        TypeUri = Utils.RemoveDiacritics(x.Description).Replace(" ", "-").ToLower()
                    }));
                }
            }
        }
    }
}
