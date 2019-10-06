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
        Task<AddressViewModel> GetUserAddress(string userId);
    }
}
