using Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Interfaces
{
    public interface IBasketViewModelService
    {
        Task<BasketViewModel> GetOrCreateBasketForUser(string userName);
    }
}
