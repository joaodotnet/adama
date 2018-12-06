using ApplicationCore.Entities;
using SalesWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Identity;

namespace SalesWeb.Interfaces
{
    public interface IShopService
    {
        //Task<MenuComponentViewModel> GetMenuList();
        //Task<Category> GetCategory(string name);
        //Task<CatalogType> GetCatalogType(string type);
        Task AddorUpdateUserAddress(ApplicationUser user, AddressViewModel addressModel, AddressType addressType = AddressType.SHIPPING);
        Task<AddressViewModel> GetUserAddress(string userId);
    }
}
