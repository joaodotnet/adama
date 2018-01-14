using DamaShopWeb.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DamaShopWeb.Web.Interfaces
{
    public interface IBasketViewModelService
    {
        Task<BasketViewModel> GetOrCreateBasketForUser(string userName);
    }
}
