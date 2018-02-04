using ApplicationCore.Entities;
using Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Interfaces
{
    public interface IShopService
    {
        Task<MenuComponentViewModel> GetMenuList();
        Task<List<MainBannerViewModel>> GetMainBanners();
        Task<Category> GetCategory(string name);
    }
}
