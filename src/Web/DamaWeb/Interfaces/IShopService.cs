using ApplicationCore.Entities;
using DamaWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Identity;

namespace DamaWeb.Interfaces
{
    public interface IShopService
    {
        Task<DamaHomePageConfigViewModel> GetDamaHomePageConfig();
    }
}
