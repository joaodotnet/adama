using SalesWeb.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesWeb.Interfaces
{
    public interface IBasketViewModelService
    {
        Task<BasketViewModel> GetOrCreateBasketForUser(string userName);
    }
}
