using ApplicationCore.Entities;
using SalesWeb.Interfaces;
using SalesWeb.ViewModels;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWeb.ViewComponents
{
    public class Menu : ViewComponent
    {
        private readonly IShopService _shopService;
        private readonly ICatalogService _catalogService;

        public Menu(IShopService service, ICatalogService catalogService)
        {
            _shopService = service;
            _catalogService = catalogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //return View(await _shopService.GetMenuList());
            return View(await _catalogService.GetMenuViewModel());
        }
    }
}
