using DamaWeb.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DamaWeb.Interfaces
{
    public interface IBasketViewModelService
    {
        Task<BasketViewModel> GetOrCreateBasketForUser(string userName);
    }
}
