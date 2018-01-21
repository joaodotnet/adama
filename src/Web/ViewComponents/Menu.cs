using ApplicationCore.Entities;
using DamaShopWeb.Web.Interfaces;
using DamaShopWeb.Web.ViewModels;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DamaShopWeb.Web.ViewComponents
{
    public class Menu : ViewComponent
    {
        private readonly IShopService _shopService;
        public Menu(IShopService service)
        {
            _shopService = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _shopService.GetMenuList());
        }
    }
}
