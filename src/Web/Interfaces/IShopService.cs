using ApplicationCore.Entities;
using DamaShopWeb.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DamaShopWeb.Web.Interfaces
{
    public interface IShopService
    {
        Task<MenuComponentViewModel> GetMenuList();
    }
}
